using UnityEngine;

public class DialogueDisplayTiming : ScriptableObject
{
    public enum DialogueTiming
    {
          WaitBeforeNextLetter
        , WaitBeforeNextWord
        , ArrayByLetter
        , ArrayByWord
    }

    [SerializeField] private DialogueTiming timing;

    [SerializeField] private float[] timings;
}
