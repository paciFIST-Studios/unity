using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerableDialogue : MonoBehaviour
{
    [SerializeField] private DialogueWindowManager manager;

    [SerializeField] private DialogueConversation conversation;


    public void OnTriggerStay(Collider other)
    {
        // if other is player
            // and if trigger button is pressed
    }



    public void TriggerDialogue()
    {
        // do this as a coroutine?
        manager.StartConversation(conversation);
    }
}
