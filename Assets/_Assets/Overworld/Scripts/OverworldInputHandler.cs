using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldInputHandler : MonoBehaviour
{
    public bool pressedDownConfirm;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        pressedDownConfirm = Input.GetKeyDown(KeyCode.Space);
    }
}
