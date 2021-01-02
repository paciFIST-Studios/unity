using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using pacifist.core;

using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New Inventory Item", menuName = "paciFIST/InventoryItem")]
public class InventoryItem : ScriptableObject
{
    [BoxGroup][HideLabel]
    public SpriteReference sprite;

    // increasing research level increments the display name
    public StringReference[] ResearchableNames;
    public StringReference[] ResearchableDescriptions;

    [Range(0, 5)]
    public int ResearchLevel;

    public float ItemExchangeValue;

}
