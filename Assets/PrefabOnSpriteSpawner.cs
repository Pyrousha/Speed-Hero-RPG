using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabOnSpriteSpawner : MonoBehaviour
{
    float rayDistance = 99;

    Color currColor;

    public Color[] colors;
    public string[] texts;

    public LayerMask terrainLayer;

    [Header("Location Spawning")]
    public Texture2D imageMap;

    public Transform topLeft;
    public Transform bottomRight;
    public OffsetGrid[] gridsToPass;

    [Header("GameObjects + Prefabs")]
    public Transform parentObj;
    public GameObject prefabToSpawn;
    public Vector3 spawnRotation;

    float xPos;
    float zPos;
    float raycastStartY;

    [System.Serializable]
    public struct OffsetGrid
    {
        public float gridsize;
        public float startXOffset;
        public float startZOffset;
    }

    private void Start()
    {
        float startX = topLeft.position.x;
        float endX = bottomRight.position.x;

        float startZ = bottomRight.position.z;
        float endZ = topLeft.position.z;

        raycastStartY = topLeft.position.y;

        //Debug.Log("sX, eX, sZ, eZ: " + startX + ", " + endX + ", " + startZ + ", " + endZ + ", ");
        
        foreach(OffsetGrid grid in gridsToPass)
        {
            for (float x = startX + grid.startXOffset; x <= endX; x += grid.gridsize)
            {
                for (float z = startZ + grid.startZOffset; z <= endZ; z += grid.gridsize)
                {
                    CheckSpawnTree(x, z);
                }
            }
        }

        /*for (float x = startX; x<= endX; x+= gridSize)
        {
            for (float z = startZ; z <= endZ; z+= gridSize)
            {
                CheckSpawnTree(x-startX, z-startZ);
            }
        }

        for (float x = startX+1; x <= endX; x += gridSize)
        {
            for (float z = startZ+1; z <= endZ; z += gridSize)
            {
                CheckSpawnTree(x - startX, z - startZ);
            }
        }*/

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

    private void CheckSpawnTree(float x, float z)
    {
        //Debug.Log("checking position " + x + "," + z);
        RaycastHit hit;
        if (Physics.Raycast(new Vector3(x,raycastStartY,z), Vector3.down, out hit, rayDistance, terrainLayer))
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
                //Debug.Log("placing obj");
                GameObject newTree = Instantiate(prefabToSpawn) as GameObject;
                newTree.transform.position = new Vector3(x, parentObj.position.y, z);
                newTree.transform.eulerAngles = spawnRotation;
                newTree.transform.parent = parentObj;
            }
            else
            {
                //Debug.Log("not valid spot");
            }
        }
    }
}
