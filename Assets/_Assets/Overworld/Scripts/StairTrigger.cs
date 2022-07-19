using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairTrigger : MonoBehaviour
{
    [System.Serializable] public enum Dir
    {
        left, 
        right
    }

    [SerializeField] private Dir direction;

    [SerializeField] private Vector3 targPos;

    private void OnTriggerEnter(Collider other)
    {
        StairCanvas.Instance.TriggerHit(direction, targPos);
    }
}
