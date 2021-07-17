using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteUnderClick : MonoBehaviour
{
    public Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                GameObject hitObj = hit.collider.gameObject;

                if (hitObj.layer == 0)
                {
                    Debug.Log(hitObj.name);

                    Destroy(hitObj);
                }
            }
        }
    }
}
