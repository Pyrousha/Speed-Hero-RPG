using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/DialogueObject")]
public class DialogueObject : ScriptableObject
{
    [SerializeField] private CharacterObject[] characters;
    [SerializeField] [TextArea] private string[] dialogue;

    [SerializeField] private Response[] responses;

    public string[] Dialogue => dialogue;
    public CharacterObject[] Characters => characters;

    public bool HasResponses => ((Responses != null) && (Responses.Length > 0));

    public Response[] Responses => responses;
}
