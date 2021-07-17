using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSpawner : MonoBehaviour
{
    public float rayDistance;

    public Color[] colors;
    public string[] texts;

    public Texture2D imageMap;

    public Color currColor;

    public Transform topLeft;
    public Transform bottomRight;

    public Transform treeParent;
    public GameObject treePrefab;

    public LayerMask terrainLayer;

    float xPos;
    float zPos;

    private void Start()
    {
        float startX = topLeft.position.x;
        float endX = bottomRight.position.x;

        float startZ = bottomRight.position.z;
        float endZ = topLeft.position.z;

        Debug.Log("sX, eX, sZ, eZ: " + startX + ", " + endX + ", " + startZ + ", " + endZ + ", ");

        //CheckSpawnTree(startX, startZ);

        
        for (float x = startX; x<= endX; x+= 2)
        {
            for (float z = startZ; z <= endZ; z+= 2)
            {
                CheckSpawnTree(x, z);
            }
        }

        for (float x = startX+1; x <= endX; x += 2)
        {
            for (float z = startZ+1; z <= endZ; z += 2)
            {
                CheckSpawnTree(x, z);
            }
        }

        gameObject.SetActive(false);
    }


    private int FindIndexFromColor(Color color)
    {
        for (int i = 0; i < colors.Length; i++)
        {
            if (colors[i] == color)
                return i;
        }

        return -1;
    }

    /*
    private void Update()
    {
        RaycastHit hit;
        if (Input.GetMouseButtonDown(0) && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, rayDistance))
        {
            Renderer renderer = hit.transform.GetComponent<MeshRenderer>();
            Texture2D texture = renderer.material.mainTexture as Texture2D;
            Vector2 pixelUV = hit.textureCoord;
            pixelUV.x *= texture.width;
            pixelUV.y *= texture.height;
            Vector2 tiling = renderer.material.mainTextureScale;
            Color color = imageMap.GetPixel(Mathf.FloorToInt(pixelUV.x * tiling.x), Mathf.FloorToInt(pixelUV.y * tiling.y));

            currColor = color;

            int index = FindIndexFromColor(color);
            if (index >= 0)
            {
                Debug.Log(texts[index]);
            }
        }
    }
    */

    private void CheckSpawnTree(float x, float z)
    {
        Debug.Log("checking position " + x + "," + "z");
        RaycastHit hit;
        if (Physics.Raycast(new Vector3(x,1,z), Vector3.down, out hit, rayDistance, terrainLayer))
        {
            Renderer renderer = hit.transform.GetComponent<MeshRenderer>();
            Texture2D texture = renderer.material.mainTexture as Texture2D;
            Vector2 pixelUV = hit.textureCoord;
            pixelUV.x *= texture.width;
            pixelUV.y *= texture.height;
            Vector2 tiling = renderer.material.mainTextureScale;
            Color color = imageMap.GetPixel(Mathf.FloorToInt(pixelUV.x * tiling.x), Mathf.FloorToInt(pixelUV.y * tiling.y));

            currColor = color;

            int index = FindIndexFromColor(color);
            if (index >= 0)
            {
                Debug.Log("placing tree");
                GameObject newTree = Instantiate(treePrefab, treeParent) as GameObject;
                newTree.transform.position = new Vector3(x, 0, z);
                newTree.transform.eulerAngles = new Vector3(0, 0, 0);
            }
            else
            {
                Debug.Log("not valid tree spot");
            }
        }
    }
}
