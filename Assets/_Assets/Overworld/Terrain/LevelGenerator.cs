using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private string levelName;
    [SerializeField] private Sprite[] floorSprites;
    [SerializeField] private float floorHeight;
    [SerializeField] private float gridSize;

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

    public GameObject testObj;

    private void InitializeArrays()
    {
        colors = new Color[terrainPieces.Length];
        objs = new GameObject[terrainPieces.Length];

        for (int i =0; i< terrainPieces.Length; i++)
        {
            colors[i] = terrainPieces[i].color;
            objs[i] = terrainPieces[i].terrainObj;
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

        for (int z = 0; z< floorSprites.Length; z++)
        {
            GameObject floorParent =  Instantiate(emptyObj);
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
                    Color pixelColor = sprTexture.GetPixel(x, height - y -1);

                    Debug.Log(x+","+y+": " + pixelColor);

                    //GameObject obj2 = Instantiate(testObj, floorParent.transform);
                    //obj2.transform.localPosition = new Vector3(x * gridSize, z * floorHeight, -y * gridSize);
                    //obj2.GetComponent<SpriteRenderer>().color = pixelColor;

                    int colorIndex = -1;
                    for (int i = 0; i<colors.Length; i++)
                    {
                        if (pixelColor == colors[i])
                        {
                            //Debug.Log("MATCH! " + i);
                            colorIndex = i;
                        }
                    }

                    if(colorIndex > -1)
                    {
                        //Valid color, spawn corresponding object;
                        GameObject obj = Instantiate(objs[colorIndex], floorParent.transform);

                        obj.transform.localPosition = new Vector3(x*gridSize, z*floorHeight, -y*gridSize); 
                    }
                }
            }
        }
    }
}
