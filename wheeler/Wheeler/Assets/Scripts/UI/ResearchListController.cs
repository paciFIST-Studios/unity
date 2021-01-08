using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResearchListController : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab;

    private List<string> inventory = new List<string>();

    private void CreateButton(string name)
    {
        var button = (GameObject)Instantiate(buttonPrefab);
        button.SetActive(true);
        button.GetComponent<ResearchItemListButton>().SetText(name);
        button.transform.SetParent(buttonPrefab.transform.parent, false);
    }

    public void AddInventoryItem(InventoryItem item)
    {
        inventory.Add(item.name);
        CreateButton(item.name);
    }

    public void SetInventory(InventoryItem[] inventory)
    {
        this.inventory = new List<string>(inventory.Length);

        for(int i = 0; i < inventory.Length; i++)
        {
            this.inventory.Add(inventory[i].name);
        }

        //StartCoroutine(RebuildInventoryAsync());

        RebuildInventory();
    }

    IEnumerator RebuildInventoryAsync()
    {
        print("Rebuild Inventory Async");
        for(int i = 0; i < this.inventory.Count; i++)
        {
            CreateButton(inventory[i]);
            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }

    public void RebuildInventory()
    {
        print("Rebuild Inventory Sync");
        for(int i = 0; i < this.inventory.Count; i++)
        {
            CreateButton(inventory[i]);
        }
    }

}
