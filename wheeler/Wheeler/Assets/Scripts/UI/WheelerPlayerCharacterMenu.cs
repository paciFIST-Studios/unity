﻿using UnityEngine;

using Sirenix.OdinInspector;

public class WheelerPlayerCharacterMenu : MonoBehaviour
{   
    [FoldoutGroup("References")][SerializeField]
    private RectTransform BasePanel;

    [FoldoutGroup("References")][SerializeField] 
    private RectTransform MissionHeader;

    [FoldoutGroup("References")][SerializeField] 
    private RectTransform ResearchHeader;

    [FoldoutGroup("References")][SerializeField]
    private RectTransform SocialHeader;

    [FoldoutGroup("References")][SerializeField]
    private RectTransform MissionPanel;

    [FoldoutGroup("References")][SerializeField]
    private RectTransform ResearchPanel;

    [FoldoutGroup("References")][SerializeField]
    private RectTransform SocialPanel;

    [FoldoutGroup("References")][SerializeField]
    private RectTransform Selector;
    private RectTransform SelectorStartPosition;

    [FoldoutGroup("References")][SerializeField]
    private GameObject contentHolder;

    private ResearchListController researchListController;

    private int currentSelectorIdx;
    private Vector3[] selectorPositions;

    private void Start()
    {
        SelectorStartPosition = Selector;

        selectorPositions = new Vector3[3];
        selectorPositions[0] = MissionHeader.position;
        selectorPositions[1] = ResearchHeader.position;
        selectorPositions[2] = SocialHeader.position;

        researchListController = this.GetComponentInChildren<ResearchListController>();

        SetMenuVisibility(false);
    }

    public void ToggleSelectorPosition(bool goForwards)
    {
        if(currentSelectorIdx == selectorPositions.Length)
        {
            if(goForwards) currentSelectorIdx = 0;
        }
        else if (currentSelectorIdx == 0)
        {
            if (!goForwards) currentSelectorIdx = selectorPositions.Length - 1;
        }

        var pos = selectorPositions[currentSelectorIdx];
        SetSelectorPosition(pos);
    }


    //public void UpdateResearchInventory(InventoryItem[] inventory)
    //{
    //    researchListController.SetInventory(inventory);
    //}

    public void AddInventoryItem(InventoryItem item)
    {
        researchListController.AddInventoryItem(item);
    }
    
    public void SetSelectorPosition(Vector3 pos)
    {
        Selector.position = pos;
    }

    public void SetMenuVisibility(bool isVisible)
    {
        BasePanel.gameObject.SetActive(isVisible);
    }

}