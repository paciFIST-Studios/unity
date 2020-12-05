using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hacker : MonoBehaviour
{
    int CurrentLevel = 0;

    void ShowMainMenu()
    {
        Terminal.ClearScreen();
        Terminal.WriteLine("What would you like to hack into?");
        Terminal.WriteLine("\n");
        Terminal.WriteLine("[1] local library");
        Terminal.WriteLine("[2] police station");
        Terminal.WriteLine("[3] nasa");
        Terminal.WriteLine("\n");
        Terminal.WriteLine("Enter Selection:");
    }

    void StartGame()
    {
        if(CurrentLevel < 0)
        {
            ShowMainMenu();
            return;
        }
        else if (CurrentLevel == 0)
        {
            return;
        }

        Terminal.WriteLine("Level " + CurrentLevel);
    }

    void OnUserInput(string input)
    {
        if (input == "1")
        {
            CurrentLevel = 1;
        }
        else if (input == "2")
        {
            CurrentLevel = 2;
        }
        else if (input == "3")
        {
            CurrentLevel = 3;
        }
        else if (input == "menu")
        {
            CurrentLevel = -1;
        }
        else
        {
            Terminal.WriteLine("Command not recognized");
            CurrentLevel = 0;
        }

        StartGame();
    }

    void Start ()
    {
        ShowMainMenu();
    }

}
