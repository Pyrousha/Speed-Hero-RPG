using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BladeShot : MonoBehaviour
{
    Rigidbody2D RB;
    public float initialSpeed;
    public float decellerateSpeed;

    // Start is called before the first frame update
    void Start()
    {
        RB = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        RB.velocity = RB.velocity * decellerateSpeed;
    }

    public void Destroy()
    {
        RB.velocity = new Vector2(0, 0);
        Destroy(gameObject);
    }
}
