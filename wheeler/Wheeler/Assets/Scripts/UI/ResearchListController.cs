using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResearchListController : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab;

    private List<InventoryItem> researchTopics = new List<InventoryItem>();

    private void CreateButton(string name)
    {
        var button = (GameObject)Instantiate(buttonPrefab);
        button.SetActive(true);
        button.GetComponent<ResearchItemListButton>().SetText(name);
        button.transform.SetParent(buttonPrefab.transform.parent, false);
    }

    public void AddResearchTopic(InventoryItem item)
    {
        researchTopics.Add(item);
        CreateButton(item.name);
    }

    public void SetResearchTopics(InventoryItem[] inventory)
    {
        researchTopics.Clear();
        researchTopics.AddRange(inventory);

        RebuildInventory();
    }

    public void RebuildInventory()
    {
        for(int i = 0; i < this.researchTopics.Count; i++)
        {
            CreateButton(researchTopics[i].name);
        }
    }

}
