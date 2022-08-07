using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformPuzzleController : MonoBehaviour
{
    [SerializeField] private List<FallingPlatform> platforms;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RespawnAllPlatforms()
    {
        foreach(FallingPlatform platform in platforms)
        {
            platform.RiseAndReset();
            //platform.Rise();
        }
    }
}
