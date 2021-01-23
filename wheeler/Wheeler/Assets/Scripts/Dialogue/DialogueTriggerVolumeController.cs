using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct DialogueData
{
    // the id of this conversation
    [SerializeField] public string ID;
    // the id of your conversation partner
    [SerializeField] public string partnerID;
    // transform of your current partner
    [SerializeField] public Transform partner;

    [SerializeField] public bool isInterruptible;
    [SerializeField] public bool isRunning;

    // this data can be replaced at any time
    [SerializeField] public bool isUnseatable;
}

public class DialogueTriggerVolumeController : MonoBehaviour
{
    [SerializeField] private DialogueData data;
    [SerializeField] private bool hideMeshDuringPlay = true;

    // Start is called before the first frame update
    void Start()
    {
        if (hideMeshDuringPlay)
        {
            this.GetComponent<MeshRenderer>().enabled = false;
        }

        if(data.partner == null)
        {
            data.partner = this.transform;
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        var player = collider.gameObject.GetComponent<WheelerPlayerController>();
        if(player)
        {
            player.SetDialogueData(data);
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        var player = collider.gameObject.GetComponent<WheelerPlayerController>();
        if(player)
        {
            player.RemoveDialogueData(data);
        }
    }
}
