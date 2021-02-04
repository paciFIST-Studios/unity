using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class DialogueConversation : ScriptableObject
{
    public bool isFinished;
    public bool isRepeatable;
    public Queue<DialogueStatement> statements;
}
