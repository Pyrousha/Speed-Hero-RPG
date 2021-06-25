using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSkyboxOnStart : MonoBehaviour
{
    public Material newSkybox;
    // Start is called before the first frame update
    void Start()
    {
        RenderSettings.skybox = newSkybox;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
