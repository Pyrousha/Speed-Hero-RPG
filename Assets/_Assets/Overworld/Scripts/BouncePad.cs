using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncePad : MonoBehaviour
{
    [SerializeField] private Vector3 direction;
    Rigidbody rbToThrow;

    public void SetDirection(Vector3 newDir)
    {
        direction = newDir;
    }

    public void SetVelocity(Rigidbody rbToThrow)
    {
        rbToThrow.velocity = direction;
    }
}
