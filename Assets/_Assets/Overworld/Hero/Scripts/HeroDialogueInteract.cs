using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroDialogueInteract : MonoBehaviour
{
    public IInteractable Interactable { get; set; }

    // Update is called once per frame
    void Update()
    {
        if (DialogueUI.Instance.isOpen == false)
        {
            if (Interactable != null)
            {
                Interactable.TryInteract(this);
            }
        }
    }
}
