using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private TMP_Text textLabel;
    [SerializeField] private TMP_Text nameLabel;
    [SerializeField] private TMP_Text nameSpacing;
    [SerializeField] private Image speakerImage;

    [SerializeField] private GameObject dialogueParent;
    [SerializeField] private Animator anim;

    public bool isOpen { get; private set; }

    public OverworldInputHandler overworldInputHandler;
    private TypewriterEffect typewriterEffect;
    private ResponseHandler responseHandler;

    private void Start()
    {
        typewriterEffect = GetComponent<TypewriterEffect>();
        responseHandler = GetComponent<ResponseHandler>();

        CloseDialogueBox();
    }

    public void ShowDialogue(DialogueObject dialogueObject)
    {
        isOpen = true;

        dialogueParent.SetActive(true);

        if (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name != "FadeIn")
            anim.SetTrigger("FadeIn");

        StartCoroutine(StepThroughDialogue(dialogueObject));
    }

    public void AddResponseEvents(ResponseEvent[] responseEvents)
    {
        responseHandler.AddResponseEvents(responseEvents);
    }

    private IEnumerator StepThroughDialogue(DialogueObject dialogueObject)
    {
        //yield return new WaitForSeconds(1.5f);

        for (int i = 0; i< dialogueObject.Dialogue.Length; i++)
        {
            //Set speaker labels + icon
            CharacterObject newSpeaker = dialogueObject.Characters[i];
            if (newSpeaker != null)
            {
                nameLabel.text = newSpeaker.CharacterName;
                nameSpacing.text = newSpeaker.CharacterName;
                speakerImage.sprite = newSpeaker.PortraitSprite;
            }

            //show text
            string dialogue = dialogueObject.Dialogue[i];
            yield return RunTypingEffect(dialogue);
            textLabel.text = dialogue;

            //if responses exist, don't let player close text box
            if ((i == dialogueObject.Dialogue.Length - 1) && (dialogueObject.HasResponses))
                break;

            //Wait for input to show next slide
            yield return null;
            yield return new WaitUntil(() => overworldInputHandler.pressedDownConfirm);
            overworldInputHandler.pressedDownConfirm = false;
        }

        if (dialogueObject.HasResponses)
        {
            responseHandler.ShowResponses(dialogueObject.Responses);
        }
        else
        {
            CloseDialogueBox();
        }
    }

    private IEnumerator RunTypingEffect(string dialogue)
    {
        typewriterEffect.Run(dialogue, textLabel);

        while (typewriterEffect.isRunning)
        {
            yield return null;

            if (overworldInputHandler.pressedDownConfirm)
            {
                overworldInputHandler.pressedDownConfirm = false;
                typewriterEffect.Stop();
            }
        }
    }

    public void CloseDialogueBox()
    {
        anim.SetTrigger("FadeOut");

        textLabel.text = string.Empty;

        isOpen = false;
    }

    public void DisableDialogueBox()
    {
        dialogueParent.SetActive(false);
    }
}
