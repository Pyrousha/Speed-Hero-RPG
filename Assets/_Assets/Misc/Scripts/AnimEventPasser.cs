using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimEventPasser : MonoBehaviour
{
    [System.Serializable] private struct EventStruct
    {
        public string name;
        public UnityEvent eventToDo;
    }

    [SerializeField] private EventStruct[] eventArray;

    public void DoEvent(int eventIndex)
    {
        eventArray[eventIndex].eventToDo.Invoke();
    }
}
