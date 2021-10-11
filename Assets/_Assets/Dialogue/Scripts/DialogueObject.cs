using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/DialogueObject")]
public class DialogueObject : ScriptableObject
{
    private CharacterObject[] characters;
    private string[] dialogue;

    public Dialogue[] dialogueLines;
    public DialogueObject nextDialogueObject;
    [SerializeField] private Response[] responses;

    public CharacterObject[] Characters => characters;

    public bool HasResponses => ((Responses != null) && (Responses.Length > 0));

    public Response[] Responses => responses;


    public void OnValidate()
    {
        if ((nextDialogueObject != null) && (responses != null && responses.Length > 0))
            Debug.LogError("ERROR ON \""+name+".asset\" : NextDialogueObject set while also using responses.\nPlease set the Responses length to 0, or remove the NextDialogueObject\n");
        Convert();
    }

    public void Convert()
    {
        if (dialogueLines == null || dialogueLines.Length == 0)
            return;

        dialogue = new string[dialogueLines.Length];
        characters = new CharacterObject[dialogueLines.Length];

        CharacterObject lastValidSpeaker = dialogueLines[0].speaker;

        for (int i = 0; i < dialogueLines.Length; i++)
        {
            dialogue[i] = dialogueLines[i].text;

            if (dialogueLines[i].speaker != null)
            {
                lastValidSpeaker = dialogueLines[i].speaker;
            }

            characters[i] = lastValidSpeaker;
        }
    }

    public string[] GetDialogue()
    {
        if (dialogue == null)
            Convert();
        return dialogue;
    }
}

[System.Serializable]
public class Dialogue
{
    [SerializeField] public CharacterObject speaker;
    [SerializeField] [TextArea] public string text;
}
