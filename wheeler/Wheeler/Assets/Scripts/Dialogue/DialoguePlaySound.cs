using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialoguePlaySound : ScriptableObject
{
    public enum DialogueSound
    {
          PerLetter
        , PerWord
        , AtBeginning
        , AtEnd
        , OnLetter
    }

    private DialogueSound type;
}
