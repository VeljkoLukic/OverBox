using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public OverBoxNodeData currentDialogue;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            GameEvents.DialogueShown();
        }
    }

    public void SetNewDialogue (OverBoxNodeData nextDialogue)
    {
        currentDialogue = nextDialogue;
    }
}
