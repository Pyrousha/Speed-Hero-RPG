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
        if (events != null && events.Length == dialogueObject.GetDialogue().Length)
            return;

        if (events == null)
        {
            events = new DialogueEvent[dialogueObject.GetDialogue().Length];
        }
        else
        {
            Array.Resize(ref events, dialogueObject.GetDialogue().Length);
        }

        for(int i = 0; i< dialogueObject.GetDialogue().Length; i++)
        {
            string dialogueText = dialogueObject.GetDialogue()[i];

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
            events[copyIndex] = new DialogueEvent() { name = dialogueObject.GetDialogue()[copyIndex] };
    }
}
