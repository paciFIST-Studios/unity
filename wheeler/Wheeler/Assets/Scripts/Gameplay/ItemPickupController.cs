using UnityEngine;

using Sirenix.OdinInspector;

public class ItemPickupController : MonoBehaviour
{
    [OnValueChanged("LoadData")]
    [InlineEditor]
    public InventoryItem data;
    
    private void LoadData(InventoryItem data)
    {
        print("ItemLoaded: " + data.name);
    }
 
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<WheelerPlayerController>() == null) { return; }

        if(data)
        {
            print("Pickup: " + data.GetCurrentResearchLevelDisplayName());
            data = null;
            Destroy(this.gameObject);
        }       
    }
}
