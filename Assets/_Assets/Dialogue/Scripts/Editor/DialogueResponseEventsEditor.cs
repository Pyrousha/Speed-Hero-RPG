using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DialogueResponseEvents))]
public class DialogueResponseEventsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DialogueResponseEvents responseEvents = (DialogueResponseEvents)target;

        if (GUILayout.Button("Refresh"))
        {
            responseEvents.OnValidate();
        }
    }
}

[CustomEditor(typeof(DialogueEvents))]
public class DialogueEventsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DialogueEvents dialogueEvents = (DialogueEvents)target;

        if (GUILayout.Button("Refresh"))
        {
            dialogueEvents.OnValidate();
        }
    }
}
