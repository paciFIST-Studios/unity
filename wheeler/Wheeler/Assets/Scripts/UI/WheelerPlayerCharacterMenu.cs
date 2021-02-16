using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

public class WheelerPlayerCharacterMenu : MonoBehaviour
{
    [FoldoutGroup("Header Scale")][SerializeField] private Vector3 SideHeaderScale;
    [FoldoutGroup("Header Scale")][SerializeField] private Vector3 CenterHeaderScale;

    [SerializeField][Required] private List<WheelerPlayerCharacterMenuPanel> panels;
    private int leftIdx;
    private int centerIdx;
    private int rightIdx;

    [SerializeField][Required] private RectTransform BasePanel;

    [FoldoutGroup("PositionReferences")][SerializeField][Required] private RectTransform headerLeft;
    [FoldoutGroup("PositionReferences")][SerializeField][Required] private RectTransform headerCenter;
    [FoldoutGroup("PositionReferences")][SerializeField][Required] private RectTransform headerRight;
    [FoldoutGroup("PositionReferences")][SerializeField][Required] private RectTransform bodyLeft;
    [FoldoutGroup("PositionReferences")][SerializeField][Required] private RectTransform bodyCenter;
    [FoldoutGroup("PositionReferences")][SerializeField][Required] private RectTransform bodyRight;

    [SerializeField][Required] private ResearchListController researchListController;





    private void Start()
    {
        leftIdx   = 0;
        centerIdx = 1;
        rightIdx  = 2;
        SetLeftPanel(leftIdx);
        SetCenterPanel(centerIdx);
        SetRightPanel(rightIdx);

        SetMenuVisibility(false);
    }

    private int IndexRingNextPosition(int current, int max, int shift)
    {
        var result = current + shift;

        if (result > max)
        {
            return 0;
        }
        else if (result < 0)
        {
            return max;
        }

        return result;
    }

    private void SetLeftPanel(int idx)
    {
        panels[idx].header.position = headerLeft.position;
        panels[idx].header.localScale = SideHeaderScale;
        //panels[idx].header.GetComponent<TextMesh>().fontStyle = FontStyle.Normal;
        panels[idx].body.position = bodyLeft.position;
    }

    private void SetCenterPanel(int idx)
    {
        panels[idx].header.position = headerCenter.position;
        panels[idx].header.localScale = CenterHeaderScale;
        //panels[idx].header.GetComponent<TextMesh>().fontStyle = FontStyle.Bold;
        panels[idx].body.position = bodyCenter.position;
    }

    private void SetRightPanel(int idx)
    {
        panels[idx].header.position = headerRight.position;
        panels[idx].header.localScale = SideHeaderScale;
        //panels[idx].header.GetComponent<TextMesh>().fontStyle = FontStyle.Normal;
        panels[idx].body.position = bodyRight.position;
    }

    private void HidePanel(int idx)
    {
        panels[idx].header.gameObject.SetActive(false);
        panels[idx].body.gameObject.SetActive(false);
    }

    // here, the !goForwards, means backwards
    public void RotateActivePanel(bool goForwards)
    {
        // all of these indices increment or decrement together,
        // but they will reach the end and need to loop over individually
        int dir = (goForwards) ? +1 : -1;
        leftIdx   = IndexRingNextPosition(leftIdx,   panels.Count - 1, dir);
        centerIdx = IndexRingNextPosition(centerIdx, panels.Count - 1, dir);
        rightIdx  = IndexRingNextPosition(rightIdx,  panels.Count - 1, dir);

        SetLeftPanel(leftIdx);
        SetCenterPanel(centerIdx);
        SetRightPanel(rightIdx);

        for(int i = 0; i < panels.Count; i++)
        {
            if(i != leftIdx && i != centerIdx && i != rightIdx)
            {
                HidePanel(i);
            }
        }
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
    

    public void SetMenuVisibility(bool isVisible)
    {
        BasePanel.gameObject.SetActive(isVisible);
    }

}
