using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Sirenix.OdinInspector;

[System.Serializable]
public class MainMenuController : MonoBehaviour
{
    [BoxGroup("UI Elements")]
    [LabelWidth(120)]
    [SerializeField] Transform backgroundImage;

    [BoxGroup("UI Elements")]
    [LabelWidth(120)]
    [SerializeField] Transform titleText;

    [BoxGroup("UI Elements")]
    [LabelWidth(120)]
    [SerializeField] Transform startButton;

    [BoxGroup("UI Elements")]
    [LabelWidth(120)]
    [SerializeField] Transform settingsButton;

    [BoxGroup("UI Elements")]
    [LabelWidth(120)]
    [SerializeField] Transform buildInfoButton;
         
    [BoxGroup("UI Elements")]
    [LabelWidth(120)]
    [SerializeField] Transform exitButton;




    public void OnStart()
    {
        SceneManager.LoadScene(1);
    }

    public void OnSettings()
    {

    }

    public void OnBuildInfo()
    {

    }

    public void OnExit()
    {
        Application.Quit();
    }

}
