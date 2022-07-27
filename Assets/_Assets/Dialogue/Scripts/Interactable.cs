using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour, IInteractable
{
    public int Priority { get; set; }

    private bool played = false;
    private bool playOnce = false;

    [SerializeField] private bool playWithoutInput;
    [SerializeField] private DialogueActivator.PlayOptions playOption;

    [SerializeField] private UnityEvent interactEvent;

    [Header("Optional References For Events")]
    [SerializeField] private List<GameObject> objs;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out HeroDialogueInteract player))
        {
            player.AddInteractable(this);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out HeroDialogueInteract player))
        {
            player.RemoveInteractable(this);
        }
    }

    public void TryInteract(HeroDialogueInteract player)
    {
        if (played && playOnce)
            return;

        if (playWithoutInput || InputHandler.Instance.DialogueInteractPressed)
        {
            interactEvent.Invoke();

            switch (playOption)
            {
                case DialogueActivator.PlayOptions.playOnce:
                    {
                        played = true;

                        playOnce = true;
                        break;
                    }

                case DialogueActivator.PlayOptions.playOnceIfSucceeds:
                    {
                        break;
                    }

                case DialogueActivator.PlayOptions.playAgainWithInput:
                    {
                        played = true;

                        playWithoutInput = false;
                        break;
                    }
            }
        }
    }

    private void OnPlayOnceConditionPassed()
    {
        played = true;

        playOnce = true;
    }

    #region Misc Interaction Events
    public void IntEvent_CheckUseKey()
    {
        if (HeroInventory.Instance.TryUseKey())
        {
            objs[0].GetComponent<Animation>().Play();
            OnPlayOnceConditionPassed();
        }
    }
    #endregion
}

