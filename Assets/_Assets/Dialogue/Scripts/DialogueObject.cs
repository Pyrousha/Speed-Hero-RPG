using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/DialogueObject")]
public class DialogueObject : ScriptableObject
{
    [SerializeField] private bool stopPlayerMovement;
    [SerializeField] [TextArea] private string[] dialogue;
    [SerializeField] private CharacterObject[] characters;

    public string[] Dialogue => dialogue;
    public CharacterObject[] Characters => characters;
}
