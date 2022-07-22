using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private string levelName;
    [SerializeField] private Sprite[] floorSprites;
    [SerializeField] private List<Transform> floorObjParents;
    [SerializeField] private float floorHeight;
    [SerializeField] private float gridSize;
    [SerializeField] private bool zOffset;

    [System.Serializable]
    private struct TerrainStruct
    {
        public Color color;
        public GameObject terrainObj;
    }

    [SerializeField] private TerrainStruct[] terrainPieces;

    private Color[] colors;
    private GameObject[] objs;

    [SerializeField] private GameObject emptyObj;

    [System.Serializable]
    private struct CombineMeshStruct
    {
        public string name;
        public int terrainPieceIndex;
        public int[] childIndices;
        public LayerMask layer;
        public PhysicMaterial physicMaterial;
    }

    [SerializeField] private CombineMeshStruct[] meshesToCombine;

    private List<List<MeshFilter>> meshFilters = new List<List<MeshFilter>>();

    private void InitializeArrays()
    {
        colors = new Color[terrainPieces.Length];
        objs = new GameObject[terrainPieces.Length];

        for (int i =0; i< terrainPieces.Length; i++)
        {
            colors[i] = terrainPieces[i].color;
            objs[i] = terrainPieces[i].terrainObj;
        }

        for (int i = 0; i< meshesToCombine.Length; i++)
        {
            meshFilters.Add(new List<MeshFilter>());
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        InitializeArrays();

        GameObject levelParent = Instantiate(emptyObj);
        levelParent.transform.parent = transform;
        levelParent.transform.localPosition = Vector3.zero;
        levelParent.name = levelName;

        for (int z = 0; z < floorSprites.Length; z++)
        {
            GameObject floorParent = Instantiate(emptyObj);
            floorParent.transform.parent = levelParent.transform;
            floorParent.transform.localPosition = Vector3.zero;
            floorParent.name = "Floor " + z.ToString();

            Texture2D sprTexture = floorSprites[z].texture;

            int width = sprTexture.width;
            int height = sprTexture.height;

            Debug.Log("Sprite named " + floorSprites[z].name + " has size of " + width + "x" + height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color pixelColor = sprTexture.GetPixel(x, height - y - 1);

                    //Debug.Log(x+","+y+": " + pixelColor);

                    //GameObject obj2 = Instantiate(testObj, floorParent.transform);
                    //obj2.transform.localPosition = new Vector3(x * gridSize, z * floorHeight, -y * gridSize);
                    //obj2.GetComponent<SpriteRenderer>().color = pixelColor;

                    int colorIndex = -1;
                    for (int i = 0; i < colors.Length; i++)
                    {
                        if (pixelColor == colors[i])
                        {
                            //Debug.Log("MATCH! " + i);
                            colorIndex = i;
                        }
                    }

                    if (colorIndex > -1)
                    {
                        //Valid color, spawn corresponding object;
                        GameObject obj = Instantiate(objs[colorIndex], floorParent.transform);

                        obj.transform.localPosition = new Vector3(x * gridSize, z * floorHeight, -y * gridSize);
                        if(zOffset)
                            obj.transform.localPosition += new Vector3(0, 0, z * floorHeight);
                        //diag offset


                        //Check if this object should have its mesh combined
                        for (int i = 0; i < meshesToCombine.Length; i++)
                        {
                            if (meshesToCombine[i].terrainPieceIndex == colorIndex)
                            {
                                foreach (int childIndex in meshesToCombine[i].childIndices)
                                {
                                    meshFilters[i].Add(obj.transform.GetChild(childIndex).GetComponent<MeshFilter>());
                                }
                            }
                        }
                    }
                }
            }
        }

        CombineMeshes();

        for (int i = 0; i< floorObjParents.Count; i++)
        {
            floorObjParents[i].position += new Vector3(0, floorHeight * i, 
                (zOffset == false ? 0 : floorHeight * i));
        }
    }

    public void CombineMeshes()
    {
        for (int j = 0; j < meshesToCombine.Length; j++)
        {
            GameObject meshParent = Instantiate(emptyObj);
            meshParent.transform.parent = transform;
            meshParent.gameObject.name = "CombineMesh: " + meshesToCombine[j].name;
            meshParent.layer = (int) Mathf.Log(meshesToCombine[j].layer.value, 2);

            MeshFilter parentMesh = meshParent.AddComponent<MeshFilter>();
            MeshCollider meshCollider = meshParent.AddComponent<MeshCollider>();

            List<MeshFilter> meshFilterList = meshFilters[j];

            CombineInstance[] combine = new CombineInstance[meshFilterList.Count];

            for (int i = 0; i < meshFilterList.Count; i++)
            {
                combine[i].mesh = meshFilterList[i].sharedMesh;
                combine[i].transform = meshFilterList[i].transform.localToWorldMatrix;
                meshFilterList[i].gameObject.GetComponent<MeshCollider>().enabled = false;
            }

            parentMesh.mesh = new Mesh();
            parentMesh.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            parentMesh.mesh.CombineMeshes(combine);

            //-AutoWeld(parentMesh.mesh, 50f, 1f);

            meshCollider.sharedMesh = parentMesh.mesh;
            meshCollider.material = meshesToCombine[j].physicMaterial;
        }
    }

    public static void AutoWeld(Mesh mesh, float threshold, float bucketStep)
    {
        Vector3[] oldVertices = mesh.vertices;
        Vector3[] newVertices = new Vector3[oldVertices.Length];
        int[] old2new = new int[oldVertices.Length];
        int newSize = 0;

        // Find AABB
        Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
        for (int i = 0; i < oldVertices.Length; i++)
        {
            if (oldVertices[i].x < min.x) min.x = oldVertices[i].x;
            if (oldVertices[i].y < min.y) min.y = oldVertices[i].y;
            if (oldVertices[i].z < min.z) min.z = oldVertices[i].z;
            if (oldVertices[i].x > max.x) max.x = oldVertices[i].x;
            if (oldVertices[i].y > max.y) max.y = oldVertices[i].y;
            if (oldVertices[i].z > max.z) max.z = oldVertices[i].z;
        }

        // Make cubic buckets, each with dimensions "bucketStep"
        int bucketSizeX = Mathf.FloorToInt((max.x - min.x) / bucketStep) + 1;
        int bucketSizeY = Mathf.FloorToInt((max.y - min.y) / bucketStep) + 1;
        int bucketSizeZ = Mathf.FloorToInt((max.z - min.z) / bucketStep) + 1;
        List<int>[,,] buckets = new List<int>[bucketSizeX, bucketSizeY, bucketSizeZ];

        // Make new vertices
        for (int i = 0; i < oldVertices.Length; i++)
        {
            // Determine which bucket it belongs to
            int x = Mathf.FloorToInt((oldVertices[i].x - min.x) / bucketStep);
            int y = Mathf.FloorToInt((oldVertices[i].y - min.y) / bucketStep);
            int z = Mathf.FloorToInt((oldVertices[i].z - min.z) / bucketStep);

            // Check to see if it's already been added
            if (buckets[x, y, z] == null)
                buckets[x, y, z] = new List<int>(); // Make buckets lazily

            for (int j = 0; j < buckets[x, y, z].Count; j++)
            {
                Vector3 to = newVertices[buckets[x, y, z][j]] - oldVertices[i];
                if (Vector3.SqrMagnitude(to) < threshold)
                {
                    old2new[i] = buckets[x, y, z][j];
                    goto skip; // Skip to next old vertex if this one is already there
                }
            }

            // Add new vertex
            newVertices[newSize] = oldVertices[i];
            buckets[x, y, z].Add(newSize);
            old2new[i] = newSize;
            newSize++;

        skip:;
        }

        // Make new triangles
        int[] oldTris = mesh.triangles;
        int[] newTris = new int[oldTris.Length];
        for (int i = 0; i < oldTris.Length; i++)
        {
            newTris[i] = old2new[oldTris[i]];
        }

        Vector3[] finalVertices = new Vector3[newSize];
        for (int i = 0; i < newSize; i++)
            finalVertices[i] = newVertices[i];

        mesh.Clear();
        mesh.vertices = finalVertices;
        mesh.triangles = newTris;
        mesh.RecalculateNormals();
        mesh.Optimize();
    }
}
