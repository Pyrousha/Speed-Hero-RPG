using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatFloorMover : MonoBehaviour
{
    public GameObject bg_1;
    public GameObject bg_2;

    public Vector3 move_1;
    public Vector3 move_2;

    // Update is called once per frame
    void Update()
    {
        bg_1.transform.position += move_1 * Time.deltaTime;
        if (bg_1.transform.localPosition.x > 1)
            bg_1.transform.localPosition += new Vector3(-1, 0, 0);
        
        bg_2.transform.position += move_2 * Time.deltaTime;
        if ((bg_2.transform.localPosition.x > 1) && (bg_2.transform.localPosition.y > 1))
            bg_2.transform.localPosition += new Vector3(-1, -1, 0);
    }
}
