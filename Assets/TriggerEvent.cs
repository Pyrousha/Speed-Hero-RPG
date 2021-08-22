using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : MonoBehaviour
{

    [SerializeField] private LayerMask interactionLayer;
    [SerializeField] private bool doOnce;
    [SerializeField] private UnityEvent onEnteredEvent;

    public void OnTriggerEnter(Collider other)
    {
        //if other.layer is within layermask
        if(interactionLayer == (interactionLayer | (1 << other.gameObject.layer)))
        {
            DoEvent();
        }
    }

    void DoEvent()
    {
        onEnteredEvent.Invoke();

        if (doOnce)
            gameObject.SetActive(false);
    }
}
