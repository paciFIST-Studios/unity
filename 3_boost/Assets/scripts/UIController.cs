using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    [SerializeField] Transform title;
    [SerializeField] Transform startButton;
    [SerializeField] Transform settingsButton;
    [SerializeField] Transform aboutButton;
    [SerializeField] Transform exitButton;

    [SerializeField] Transform settingsDialogue;
    [SerializeField] Transform aboutDialogue;

    private void Start()
    {
        SetSettingsDialogueIsVisible(false);
        SetAboutDialogueIsVisible(false);
        SetButtonsAreVisible(true);
    }

    private void SetButtonsAreVisible(bool isVisible)
    {
        title.gameObject.SetActive(isVisible);
        startButton.gameObject.SetActive(isVisible);
        settingsButton.gameObject.SetActive(isVisible);
        aboutButton.gameObject.SetActive(isVisible);
        exitButton.gameObject.SetActive(isVisible);
    }

    public void SetSettingsDialogueIsVisible(bool isVisisble)
    {
        SetButtonsAreVisible(!isVisisble);
        settingsDialogue.gameObject.SetActive(isVisisble);
    }

    public void SetAboutDialogueIsVisible(bool isVisisble)
    {
        SetButtonsAreVisible(!isVisisble);
        aboutDialogue.gameObject.SetActive(isVisisble);
    }

    public void StartNewGame()
    {
        // the main menu is scene 0
        SceneManager.LoadScene(1);
    }

    public void ContinueGame() { }

    public void Settings() { }


    public void ExitProgram()
    {
        // todo: ellie: save data
        Application.Quit();
    }



}
