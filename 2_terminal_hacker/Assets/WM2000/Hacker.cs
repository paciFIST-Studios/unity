using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hacker : MonoBehaviour
{
    int CurrentLevel = 0;

    enum Screen
    {
        // this is the primary menu and game screen, serving as level select
          MainMenu
        // this is the "game is playing" state
        , Password
        // player has just won
        , Win        
    };

    Screen CurrentScreen = Screen.MainMenu;

    string Level1Password = "first";
    string Level2Password = "second";
    string Level3Password = "third";


    void ShowMainMenu()
    {
        CurrentScreen = Screen.MainMenu;
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
        CurrentScreen = Screen.Password;
        Terminal.ClearScreen();
        Terminal.WriteLine("<Level " + CurrentLevel + ">");
        Terminal.WriteLine("ACCESS DENIED");
        Terminal.WriteLine("***************************************");
        Terminal.WriteLine("Enter Password");
    }

    void ParseMainMenuInput(string input)
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
        else
        {
            Terminal.WriteLine("Command not recognized");
            return;
        }

        StartGame();
    }

    bool isCorrectPasswordForLevel(int level, string password)
    {
        if (level == 1)
        {
            return password == Level1Password;
        }
        else if (level == 2)
        {
            return password == Level2Password;
        }
        else if(level == 3)
        {
            return password == Level3Password;
        }

        return false;
    }

    void ParsePasswordInput(string input)
    {
        if (isCorrectPasswordForLevel(CurrentLevel, input))
        {
            Terminal.WriteLine("Password Acccepted");
            CurrentScreen = Screen.Win;
        }
        else
        {
            Terminal.WriteLine("\nInvalid.  Enter Password:");
        }
    }

    void ParseWinInput(string input)
    { }

    void OnUserInput(string input)
    {
        if (input == "menu")
        {
            ShowMainMenu();
            return;
        }


        if (CurrentScreen == Screen.MainMenu)
        {
            ParseMainMenuInput(input);
        }
        else if (CurrentScreen == Screen.Password)
        {
            ParsePasswordInput(input);
        }
        else if (CurrentScreen == Screen.Win)
        {
            ParseWinInput(input);
        }
    }

    void Start ()
    {
        ShowMainMenu();
    }

}
