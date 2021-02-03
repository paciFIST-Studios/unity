using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueDisplayShake : ScriptableObject
{
    public enum DialogueShake
    {
          LeftRight
        , UpDown
        , AtAngle
        , Series
    }

    [SerializeField] private DialogueShake type;

    [SerializeField] private Vector2[] movementDirections;
    [SerializeField] private float travelTime;
    [SerializeField] private float travelSpeed;

}
