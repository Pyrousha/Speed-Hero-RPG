using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtkCubeRemover : MonoBehaviour
{
    public LayerMask atkCubelayer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void removeCube()
    {
        int xPos = Mathf.RoundToInt(transform.localPosition.x);
        int zPos = Mathf.RoundToInt(transform.localPosition.z);
        transform.localPosition = new Vector3(xPos, 0, zPos);

        gameObject.layer = 0; //change layer so this cube isn't checked for in collision test

        Collider[] collisions = Physics.OverlapSphere(transform.position, 0.2f, atkCubelayer);
        foreach (Collider go in collisions)
        {
            Destroy(go.gameObject);
        }
        Destroy(gameObject);
    }
}
