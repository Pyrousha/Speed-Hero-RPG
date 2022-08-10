using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTargetMover : MonoBehaviour
{
    [SerializeField] private Transform parentTransform;
    [SerializeField] private float maxXOffset;
    [SerializeField] private float maxYOffset;
    //value between 0 and 1, where 0 is on top of the player, and 1 is the edge of the screen
    //Determines how far the mouse has to be away to start moving the camera towards it
    [SerializeField] private float minXPercent;
    [SerializeField] private float maxXPercent;

    [SerializeField] private float minYPercent;
    [SerializeField] private float maxYPercent;

    private float mouseXPercent;
    private float mouseYPercent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = parentTransform.position + GetNewOffsetFromParent();
    }

    private Vector3 GetNewOffsetFromParent()
    {
        Vector3 mousePos = Input.mousePosition;
        mouseXPercent = mousePos.x / Screen.width;
        mouseYPercent = mousePos.y / Screen.height;

        float mouseXPercWeighted = (2 * mouseXPercent) - 1; //value from -1 (far left) to 1 (far right)
        float mouseYPercWeighted = (2 * mouseYPercent) - 1;

        Vector3 toReturn = new Vector3();

        toReturn.x = GetNormalizedOffset(mouseXPercWeighted, minXPercent, maxXPercent) * maxXOffset;
        toReturn.z = GetNormalizedOffset(mouseYPercWeighted, minYPercent, maxYPercent) * maxYOffset;

        return toReturn;
    }

    private float GetNormalizedOffset(float x, float minPerc, float maxPerc)
    {
        if (x <= -maxPerc)
            return -1;

        if (x< -minPerc)
            return (x + minPerc) / (maxPerc - minPerc);

        if (x <= minPerc)
            return 0;

        if(x < maxPerc)
            return (x - minPerc) / (maxPerc - minPerc);

        //if(x >= maxPercent)
            return 1;
    }
}
