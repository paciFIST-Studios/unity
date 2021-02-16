using UnityEngine;

using Sirenix.OdinInspector;

public class WheelerPlayerCharacterMenu : MonoBehaviour
{   
    [FoldoutGroup("References")][SerializeField][Required]
    private RectTransform BasePanel;

    [FoldoutGroup("References")][SerializeField][Required]
    private RectTransform MissionHeader;

    [FoldoutGroup("References")][SerializeField][Required]
    private RectTransform ResearchHeader;

    [FoldoutGroup("References")][SerializeField][Required]
    private RectTransform SocialHeader;

    [FoldoutGroup("References")][SerializeField][Required]
    private RectTransform MissionPanel;

    [FoldoutGroup("References")][SerializeField][Required]
    private RectTransform ResearchPanel;

    [FoldoutGroup("References")][SerializeField][Required]
    private RectTransform SocialPanel;

    [FoldoutGroup("References")][SerializeField][Required]
    private RectTransform Selector;
    private RectTransform SelectorStartPosition;

    [FoldoutGroup("References")][SerializeField][Required]
    private GameObject contentHolder;

    [FoldoutGroup("References")][SerializeField][Required]
    private ResearchListController researchListController;

    public struct SelectorData
    {
        public Vector3 current;
        public Vector3 mission;
        public Vector3 research;
        public Vector3 social;
    }
    private SelectorData selector;

    //private int selectorIdx;
    //private Vector3[] selectorPositions;


    private void Start()
    {
        SelectorStartPosition = Selector;

        selector = new SelectorData()
        {
             current = ResearchHeader.position
            , mission = MissionHeader.position
            , research = ResearchHeader.position
            , social = SocialHeader.position
        };

        SetMenuVisibility(false);
    }

    public void ToggleSelectorPosition(bool goForwards)
    {
        // Note: can't use old style switch(case) for Vec3 values

        // these are the forwards and backwards order for the three positions we support here
        // forward:  mission -> research -> social -> loop to mission
        // backward: mission <- research <- social <- loop from mission

        if(selector.current == selector.mission)
        {
            selector.current = (goForwards) ? selector.research : selector.social;
        }
        else if (selector.current == selector.research)
        {
            selector.current = (goForwards) ? selector.social : selector.mission;
        }
        else if (selector.current == selector.social)
        {
            selector.current = (goForwards) ? selector.mission : selector.research;
        }
        else
        {
            // error, possibly we have added positions, 
            // but the current position does not match a position we handle
        }

        SetSelectorPosition(selector.current);
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
    
    public void SetSelectorPosition(Vector3 pos)
    {
        Selector.position = pos;
    }

    public void SetMenuVisibility(bool isVisible)
    {
        BasePanel.gameObject.SetActive(isVisible);
    }

}
