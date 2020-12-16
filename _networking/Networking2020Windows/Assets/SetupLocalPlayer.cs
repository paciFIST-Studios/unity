using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SetupLocalPlayer : NetworkBehaviour
{
    [SerializeField] public Text namePrefab;
    [SerializeField] private Text playerNameLabel;
    [SerializeField] private Transform namePlatePosition;

    private string textboxName = "";
    private string colorboxName = "";

    [SyncVar (hook = "OnChangeName")]
    [SerializeField] private string playerName = "Player";

    [SyncVar(hook = "OnChangeColor")]
    [SerializeField] private string playerColor = "#ffffff";

    private void OnChangeName(string name)
    {
        playerName = name;
        playerNameLabel.text = playerName;
    }

    private void OnChangeColor(string color)
    {
        playerColor = color;

        Renderer[] renderers = this.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            if(r.name == "BODY")
            {
                r.material.SetColor("_Color", ColorFromHex(playerColor));
            }
        }

    }

    [Command]
    private void CmdChangeName(string name)
    {
        playerName = name;
        playerNameLabel.text = playerName;
    }

    [Command]
    private void CmdChangeColor(string name)
    {
        playerColor = name;

        Renderer[] renderers = this.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            if (r.name == "BODY")
            {
                r.material.SetColor("_Color", ColorFromHex(playerColor));
            }
        }
    }

    private void OnGUI()
    {
        if(isLocalPlayer)
        {
            textboxName = GUI.TextField(new Rect(25, 15, 100, 25), textboxName);
            if(GUI.Button(new Rect(130,13,60,25), "Submit")){
                CmdChangeName(textboxName);}

            colorboxName = GUI.TextField(new Rect(200, 15, 100, 25), colorboxName);
            if(GUI.Button(new Rect(305,13,60,25), "Submit")){
                CmdChangeColor(colorboxName);}
        }
    }

    Color ColorFromHex(string hex)
    {
        if(hex == "red")
        {
            return new Color32(255, 0, 0, 255);
        }
        else if (hex == "green")
        {
            return new Color32(0, 255,  51, 230);
        }
        else if (hex == "blue")
        {
            return new Color32(0, 51, 255, 200);
        }
        else
        {
            hex = hex.Replace("0x", "");
            hex = hex.Replace("#", "");

            byte a = 255;
            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            if(hex.Length == 8)
            {
                a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
            }

            return new Color32(r, g, b, a);
        }
    }


    void Start()
    {
        namePlatePosition.GetComponent<MeshRenderer>().enabled = false;

        if(isLocalPlayer)
        {
            this.GetComponent<PlayerController>().enabled = true;
            CameraFollow360.player = this.transform;
        }
        else
        {
            this.GetComponent<PlayerController>().enabled = false;
        }

        GameObject canvas = GameObject.FindWithTag("MainCanvas");
        playerNameLabel = (Text)Instantiate(namePrefab, Vector3.zero, Quaternion.identity);
        playerNameLabel.transform.SetParent(canvas.transform);
    }

    void Update()
    {
        Vector3 playerNameLabelPos = Camera.main.WorldToScreenPoint(namePlatePosition.position);
        playerNameLabel.transform.position = playerNameLabelPos;
    }
}
