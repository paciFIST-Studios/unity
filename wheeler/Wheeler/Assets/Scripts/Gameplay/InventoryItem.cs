using UnityEngine;

using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New Inventory Item", menuName = "paciFIST/InventoryItem")]
public class InventoryItem : ScriptableObject
{
    [BoxGroup] [HideLabel]
    public SpriteReference sprite;

    // increasing research level increments the display name
    public StringReference[] ResearchableNames;
    public StringReference[] ResearchableDescriptions;

    [Range(0, 5)]
    public int ResearchLevel;

    public float ItemExchangeValue;


    public string GetCurrentResearchLevelDisplayName()
    {
        if(ResearchableNames.Length == 0)
        {
            return "ERROR: NO Researchable Names PRESENT";
        }

        if(ResearchableDescriptions.Length == 0)
        {
            return "ERROR: NO Researchable Descriptions PRESENT";
        }

        return (ResearchLevel > ResearchableNames.Length - 1) 
            ? ResearchableNames[ResearchableNames.Length - 1].value 
            : ResearchableNames[ResearchLevel].value;
    }
}
