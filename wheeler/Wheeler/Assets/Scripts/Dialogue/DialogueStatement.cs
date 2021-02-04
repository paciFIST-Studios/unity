using UnityEngine;

using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New Dialogue Statement", menuName = "paciFIST/DialogueStatement")]
[InlineEditor]
public class DialogueStatement : ScriptableObject
{
    public bool isLeftSpeaker;

    [HideLabel] public StringReference text;

    [HideLabel] public SpriteReference image;

    public DialogueDisplayTiming timings;

    public DialogueDisplayShake shake;

    public DialoguePlaySound sound;  
}
