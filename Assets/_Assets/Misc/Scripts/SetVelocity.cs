using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetVelocity : MonoBehaviour
{
    public Vector3 newVelocity;
    [SerializeField] private bool applyOnStart;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (applyOnStart)
            ApplyVelocity();
    }

    public void ApplyVelocity()
    {
        rb.velocity = newVelocity;
        this.enabled = false;
    }
}
