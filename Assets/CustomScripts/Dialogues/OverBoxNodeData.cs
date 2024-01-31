using System;
using UnityEngine;

[Serializable]
public class OverBoxNodeData : ScriptableObject
{
    public CharacterType character;

    [NonReorderable]
    public DialogueChoice[] choices = new DialogueChoice [3];

    public Vector2 nodePosition;

    public void SetupData(string ID, Vector2 nodePosition)
    {
        character = CharacterType.Daniel;

        this.nodePosition = nodePosition;

        for (int i = 0; i < choices.Length; i++)
            choices[i] = new DialogueChoice();
    }
}
