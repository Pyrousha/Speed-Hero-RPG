using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformPuzzleController : MonoBehaviour
{
    [SerializeField] private List<FallingPlatform> platforms;

    private List<BoxCollider> topPlatforms = new List<BoxCollider>();

    // Start is called before the first frame update
    void Start()
    {
        foreach(FallingPlatform platform in platforms)
        {
            topPlatforms.Add(platform.GetComponent<BoxCollider>());
        }

        DisablePlatforms();
    }


    public void RespawnAllPlatforms()
    {
        foreach(FallingPlatform platform in platforms)
        {
            platform.RiseAndReset();
            //platform.Rise();
        }
    }

    public void EnablePlatforms()
    {
        foreach(BoxCollider col in topPlatforms)
        {
            col.enabled = true;
        }
    }

    public void DisablePlatforms()
    {
        foreach (BoxCollider col in topPlatforms)
        {
            col.enabled = false;
        }
    }
}
