using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New Character Menu Panel", menuName = "paciFIST/ui/CharacterMenuPanel")]
public class WheelerPlayerCharacterMenuPanel : ScriptableObject
{
    [SerializeField][Required] public RectTransform header;
    [SerializeField][Required] public RectTransform body;
}
