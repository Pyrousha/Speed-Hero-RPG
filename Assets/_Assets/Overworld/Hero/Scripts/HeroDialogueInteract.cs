using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroDialogueInteract : MonoBehaviour
{
    public PriorityQueue<IInteractable> Interactables { get; private set; } = new PriorityQueue<IInteractable>();
    private IInteractable highestPrioInteractable = null;

    // Update is called once per frame
    void Update()
    {
        if (DialogueUI.Instance.isOpen == false)
        {
            if (highestPrioInteractable != null)
            {
                Interactables.Peek().TryInteract(this);
            }
        }
    }

    internal void AddInteractable(DialogueActivator dialogueActivator)
    {
        if (Interactables.Contains(dialogueActivator))
            return;

        Interactables.Put(dialogueActivator, dialogueActivator.Priority);

        highestPrioInteractable = Interactables.Peek();
    }

    internal void RemoveInteractable(DialogueActivator dialogueActivator)
    {
        Interactables.TryRemoveElement(dialogueActivator);

        if(Interactables.Count > 0)
        {
            highestPrioInteractable = Interactables.Peek();
        }
        else
        {
            highestPrioInteractable = null;
        }
    }
}
