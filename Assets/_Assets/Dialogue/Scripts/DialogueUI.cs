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

    [SerializeField] private DialogueObject testDialogue;
    [SerializeField] private GameObject dialogueParent;

    private TypewriterEffect typewriterEffect;

    private void Start()
    {
        typewriterEffect = GetComponent<TypewriterEffect>();
        ShowDialogue(testDialogue);
    }

    public void ShowDialogue(DialogueObject dialogueObject)
    {
        dialogueParent.SetActive(true);
        StartCoroutine(StepThroughDialogue(dialogueObject));
    }

    private IEnumerator StepThroughDialogue(DialogueObject dialogueObject)
    {
        yield return new WaitForSeconds(1.5f);

        for (int i = 0; i< dialogueObject.Dialogue.Length; i++)
        {
            CharacterObject newSpeaker = dialogueObject.Characters[i];
            if (newSpeaker != null)
            {
                nameLabel.text = newSpeaker.CharacterName;
                nameSpacing.text = newSpeaker.CharacterName;
                speakerImage.sprite = newSpeaker.PortraitSprite;
            }


            string dialogue = dialogueObject.Dialogue[i];
            yield return typewriterEffect.Run(dialogue, textLabel);
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        }

        CloseDialogueBox();
    }

    private void CloseDialogueBox()
    {
        dialogueParent.SetActive(false);
        textLabel.text = string.Empty;
    }
}
