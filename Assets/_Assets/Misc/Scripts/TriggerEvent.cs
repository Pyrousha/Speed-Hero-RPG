using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : MonoBehaviour
{
    [SerializeField] private bool showDebug;
    [SerializeField] private LayerMask interactionLayer;
    [SerializeField] private bool doOnce;
    [SerializeField] private UnityEvent onEnteredEvent;
    [SerializeField] private UnityEvent afterDoneEvent;

    public void OnTriggerEnter(Collider other)
    {
        if (showDebug)
            Debug.Log("Collided with object: " + other.gameObject.name + ", with layer: "+other.gameObject.layer);

        //if other.layer is within layermask
        if(interactionLayer == (interactionLayer | (1 << other.gameObject.layer)))
        {
            DoEvent();
            if (showDebug)
                Debug.Log("Invoking event");
        }
        else
        {
            if (showDebug)
                Debug.Log("Wrong layer: ");
        }
    }

    public void DoEvent()
    {
        onEnteredEvent.Invoke();

        if (afterDoneEvent != null)
            afterDoneEvent.Invoke();

        if (doOnce)
            gameObject.SetActive(false);
    }
}
