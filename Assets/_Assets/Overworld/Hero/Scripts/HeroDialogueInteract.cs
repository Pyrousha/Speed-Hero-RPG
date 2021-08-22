using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroDialogueInteract : MonoBehaviour
{
    [Header("Dialogue References")]
    [SerializeField] private OverworldInputHandler overworldInputHandler;
    [SerializeField] private DialogueUI dialogueUI;

    public DialogueUI DialogueUI => dialogueUI;

    public IInteractable Interactable { get; set; }

    // Update is called once per frame
    void Update()
    {
        if (!dialogueUI.isOpen)
        {
            if (Interactable != null)
            {
                Interactable.TryInteract(this);
                overworldInputHandler.pressedDownConfirm = false;
            }
        }
    }
}
