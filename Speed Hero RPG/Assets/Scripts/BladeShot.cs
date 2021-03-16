using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BladeShot : MonoBehaviour
{
    Rigidbody RB;
    public float initialSpeed;
    public float decellerateSpeed;

    // Start is called before the first frame update
    void Start()
    {
        RB = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        RB.velocity = RB.velocity * decellerateSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "EnemyProjectile")
        {
            Destroy(other.gameObject);
        }
    }

    public void DestroySelf()
    {
        RB.velocity = new Vector2(0, 0);
        Destroy(gameObject);
    }
}
