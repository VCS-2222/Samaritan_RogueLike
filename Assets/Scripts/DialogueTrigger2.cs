using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger2 : MonoBehaviour
{
    public Dialogue dialogue;
    public DialogueManager2 manager;
    private void Start()
    {
        manager.StartDialogue(dialogue);
    }
}
