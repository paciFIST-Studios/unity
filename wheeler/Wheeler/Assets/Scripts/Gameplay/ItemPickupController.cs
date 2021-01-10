using UnityEngine;

using Sirenix.OdinInspector;

public class ItemPickupController : MonoBehaviour
{
    [OnValueChanged("LoadData")][InlineEditor]
    public InventoryItem data;
    
    private void LoadData(InventoryItem data)
    {
        this.GetComponentInChildren<SpriteRenderer>().sprite = data.sprite;
    }
 
    private void OnTriggerEnter(Collider other)
    {
        var wheeler = other.gameObject.GetComponent<WheelerPlayerController>();
        if(wheeler == null) { return; }
        if(data == null) { return; }

        wheeler.AddInventoryItem(data);

        data = null;
        Destroy(this.gameObject);        
    }
}
