using System;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "paciFIST/TabeledSpriteList")]
public class TabeledSpriteList : ScriptableObject
{
    [TableList]
    [SerializeField]
    List<SpriteVariable> TheElements = new List<SpriteVariable>();
}
