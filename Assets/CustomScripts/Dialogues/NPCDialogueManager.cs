using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class NPCDialogueManager : MonoBehaviour
{
    public GameObject bubble;

    public CharacterType character;

    private void Start()
    {
        GameEvents.OnDialogueShown += ShowDialogue;
    }

    private void ShowDialogue()
    {
        if (DialogueManager.Instance.currentDialogue.character != character)
            return;

        bubble.gameObject.SetActive(true);
    }

    private void ShowNextDialogue(OverBoxNodeData nextDialogue, bool fireImmediateEvent)
    {
        StartCoroutine(ShowNextDialogueCoroutine(nextDialogue, fireImmediateEvent));
    }

    private IEnumerator ShowNextDialogueCoroutine(OverBoxNodeData nextDialogue, bool fireImmediateEvent)
    {
        yield return new WaitForSeconds(1);     
        
        bubble.SetActive(false);

        DialogueManager.Instance.currentDialogue = nextDialogue;

        if (fireImmediateEvent)
            GameEvents.DialogueShown();
    }
}
