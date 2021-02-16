using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

public class WheelerPlayerCharacterMenu : MonoBehaviour
{   
    [SerializeField][Required]
    private List<WheelerPlayerCharacterMenuPanel> panels;
    private int activePanelIdx;

    [SerializeField] private RectTransform headerLeft;
    [SerializeField] private RectTransform headerCenter;
    [SerializeField] private RectTransform headerRight;

    [FoldoutGroup("References")][SerializeField][Required]
    private RectTransform BasePanel;


    [FoldoutGroup("References")][SerializeField][Required]
    private RectTransform Selector;

    [FoldoutGroup("References")][SerializeField][Required]
    private ResearchListController researchListController;


    private void Start()
    {
        ActivatePanel(0);
        SetMenuVisibility(false);
    }

    // here, the !goForwards, means backwards
    public void ToggleActivePanel(bool goForwards)
    {        
        if(goForwards)
        {
            activePanelIdx = (activePanelIdx == panels.Count - 1) ? 0 : activePanelIdx + 1;
        }
        else
        {
            activePanelIdx = (activePanelIdx == 0) ? panels.Count - 1 : activePanelIdx - 1;
        }

        print($"Idx:{activePanelIdx}, len:{panels.Count}");
        ActivatePanel(activePanelIdx);
    }
    
    //public void UpdateResearchInventory(InventoryItem[] inventory)
    //{
    //    researchListController.SetInventory(inventory);
    //}

    public void AddInventoryItem(InventoryItem item)
    {
        if(researchListController)
        {
            researchListController.AddResearchTopic(item);
        }
    }
    
    public void ActivatePanel(int idx)
    {
        // here, we touch every panel, but we also ensure that all the off things are really off
        for(int i = 0; i < panels.Count; i++)
        {
            bool set = (i == idx) ? true : false;
            //panels[i].header.gameObject.SetActive(set);
            panels[i].body.gameObject.SetActive(set);

            // also, position the selection marker over the correct header
            if(set)
            {
                Selector.position = panels[i].header.position;
            }
        }
    }

    public void SetMenuVisibility(bool isVisible)
    {
        BasePanel.gameObject.SetActive(isVisible);
    }

}
