using UnityEngine;

public class DialogueActivator : MonoBehaviour, IInteractable
{
    [Header("Special Case stuff (can leave blank)")]
    [SerializeField] private bool playOnStart;
    [SerializeField] private DialogueUI dialogueUI;

    [Header("Objects + Options")]
    [SerializeField] private DialogueObject dialogueObject;
    [SerializeField] private bool playWithoutInput;
    [SerializeField] private PlayOptions playOption;
    private OverworldInputHandler overworldInputHandler;

    private enum PlayOptions
    {
        playOnce,
        playAgainWithInput,
        playInfinite
    }

    private bool played = false;
    private bool playOnce = false;

    private void Start()
    {
        overworldInputHandler = FindObjectOfType<OverworldInputHandler>();
        if (playOnStart)
            TryInteract();
    }

    public void UpdateDialogueObject(DialogueObject dialogueObject)
    {
        this.dialogueObject = dialogueObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out PlayerMove2D player))
        {
            player.Interactable = this;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out PlayerMove2D player))
        {
            if (player.Interactable is DialogueActivator dialogueActivator && dialogueActivator == this)
            {
                player.Interactable = null;
            }
        }
    }

    public void TryInteract()
    {
        if (played && playOnce)
            return;

        dialogueUI.ShowDialogue(dialogueObject, gameObject);
    }

    public void TryInteract(PlayerMove2D player)
    {
        if (played && playOnce)
            return;

        if (playWithoutInput || overworldInputHandler.pressedDownConfirm)
        {
            played = true;
            player.DialogueUI.ShowDialogue(dialogueObject, gameObject);
            overworldInputHandler.pressedDownConfirm = false;

            switch (playOption)
            {
                case PlayOptions.playOnce:
                    {
                        playOnce = true;
                        break;
                    }

                case PlayOptions.playAgainWithInput:
                    {
                        playWithoutInput = false;
                        break;
                    }

                case PlayOptions.playInfinite:
                    {
                        break;
                    }
            }
        }
    }
}
