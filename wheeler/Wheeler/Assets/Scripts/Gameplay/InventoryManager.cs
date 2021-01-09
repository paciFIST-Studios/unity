using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

// todo: see if we can make this a more generic inventory manager, that'll work for
// research, literal inventory, and social relationships

[CreateAssetMenu(fileName = "New Research Manager", menuName = "paciFIST/Gameplay/ResearchManager")]
public class InventoryManager : ScriptableObject
{
    [SerializeField][InlineEditor][AssetList]
    private List<InventoryItem> researchTopics = new List<InventoryItem>();

    // add

    public void AddResearchTopic(InventoryItem item)
    {
        if(!ResearchTopicsContainsItem(item))
        {
            researchTopics.Add(item);
        }
        else
        {
            Debug.LogWarning(string.Format($"WARNING: Duplicate research entry: {0}", item.name));
        }
    }

    // get/set

    public InventoryItem[] GetResearchTopics() { return researchTopics.ToArray(); }

    public void SetResearchTopics(InventoryItem[] topics)
    {
        researchTopics.Clear();
        researchTopics = new List<InventoryItem>(topics.Length);
        researchTopics.AddRange(topics);
    }

    // util

    private bool ResearchTopicsContainsItem(InventoryItem item)
    {
        for(int i = 0; i < researchTopics.Count; i++)
        {
            if(researchTopics[i].name == item.name)
            {
                return true;
            }
        }
        return false;
    }
}
