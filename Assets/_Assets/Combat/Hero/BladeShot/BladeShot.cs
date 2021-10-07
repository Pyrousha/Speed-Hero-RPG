using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BladeShot : MonoBehaviour
{
    Rigidbody RB;
    public float initialSpeed;
    public float decellerateSpeed;
    public int attackNum;

    
    // Start is called before the first frame update
    void Start()
    {
        if (transform.childCount > 0)
        {
            RB = transform.GetChild(0).GetComponent<Rigidbody>();
            RB.velocity = RB.transform.right * initialSpeed;
        }
        else
            RB = null;
    }

    private void FixedUpdate()
    {
        if (RB != null)
            RB.velocity = RB.velocity * decellerateSpeed;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "EnemyProjectile")
        {
            //Debug.Log("HIT!");
            other.GetComponent<Enemy_Attack>().TryDestroy(attackNum);
        }
    }

    public void DestroySelf()
    {
        RB.velocity = new Vector2(0, 0);
        Destroy(gameObject);
    }
}
