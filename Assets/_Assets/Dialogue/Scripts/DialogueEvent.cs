using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class DialogueEvent
{
    [HideInInspector] public string name;
    [SerializeField] private UnityEvent afterTextSpoken;

    public UnityEvent AfterTextSpoken => afterTextSpoken;
}
