using UnityEngine;
using System;

public class DialogueEvents : MonoBehaviour
{
    [SerializeField] private DialogueObject dialogueObject;
    [SerializeField] private DialogueEvent[] events;

    public DialogueObject DialogueObject => dialogueObject;

    public DialogueEvent[] Events => events;

    [Header("Stuff For Moving Events")]
    public int copyIndex;
    public int pasteIndex;
    public bool deleteAfterCopy;

    public void OnValidate()
    {
        if (dialogueObject == null)
            return;
        if (events != null && events.Length == dialogueObject.Dialogue.Length)
            return;

        if (events == null)
        {
            events = new DialogueEvent[dialogueObject.Dialogue.Length];
        }
        else
        {
            Array.Resize(ref events, dialogueObject.Dialogue.Length);
        }

        for(int i = 0; i< dialogueObject.Dialogue.Length; i++)
        {
            string dialogueText = dialogueObject.Dialogue[i];

            if (events[i] != null)
            {
                events[i].name = dialogueText;
                continue;
            }

            events[i] = new DialogueEvent() { name = dialogueText };
        }
    }

    public void Copy()
    {
        events[pasteIndex] = events[copyIndex];

        if(deleteAfterCopy)
            events[copyIndex] = new DialogueEvent() { name = dialogueObject.Dialogue[copyIndex] };
    }
}
