using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGStarMove : MonoBehaviour
{
    public Material mat;
    public float starSizeAdd;
    public float minStarSize;
    public float maxStarSize;
    float startStarSize = 50;
    float starSize;

    // Start is called before the first frame update
    void Start()
    {
        mat.SetFloat("_StarSize", 50);
    }

    // Update is called once per frame
    void Update()
    {
        starSize += starSizeAdd*Time.deltaTime;

        if (starSize > maxStarSize) //reached upper cap, revese
        {
            starSize = maxStarSize;
            starSizeAdd *= -1;
        }
        else
        {
            if (starSize < minStarSize) //reached lower cap, revese
            {
                starSize = minStarSize;
                starSizeAdd *= -1;
            }
        }

        mat.SetFloat("_StarSize", starSize);
    }

    void OnApplicationQuit()
    {
        mat.SetFloat("_StarSize", 50);
    }
}
