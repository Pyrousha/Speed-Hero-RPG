using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairTrigger : MonoBehaviour
{
    [SerializeField] private StairParent stairParent;

    private void OnTriggerEnter(Collider other)
    {
        stairParent.TriggerEnter();
    }
}
