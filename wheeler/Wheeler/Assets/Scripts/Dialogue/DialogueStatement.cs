using UnityEngine;

public class DialogueStatement : ScriptableObject
{
    public bool isLeftSpeaker;

    public StringReference text;

    public SpriteReference image;

    public DialogueDisplayTiming timings;

    public DialogueDisplayShake shake;

    public DialoguePlaySound sound;  
}
