using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{

    public void StartNewGame()
    {
        // the main menu is scene 0
        SceneManager.LoadScene(1);
    }

    public void ContinueGame() { }

    public void Settings() { }

    public void About() { }

    public void ExitProgram()
    {
        // todo: ellie: save data
        Application.Quit();
    }



}
