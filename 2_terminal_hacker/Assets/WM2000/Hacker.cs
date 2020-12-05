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

    string Server1Name = "library.local";
    string Server2Name = "police.local";
    string Server3Name = "nasa";

    string[] Server1Passwords = { "cat", "dog", "ant", "ape", "asp", "bee", "doe", "sow"  };
    string[] Server2Passwords = { "beauty", "battle", "bottle", "career", "client", "castle", "forget", "ground" };
    string[] Server3Passwords = { "abilities", "admission", "candidate", "childhood", "documents", "education", "implement", "portfolio"  };

    string CurrentServer = "";
    string CurrentPassword = "";

    void ShowMainMenu()
    {
        CurrentScreen = Screen.MainMenu;
        Terminal.ClearScreen();
        Terminal.WriteLine("Available Connections:");
        Terminal.WriteLine("**************************************");
        Terminal.WriteLine("[1] " + Server1Name);
        Terminal.WriteLine("[2] " + Server2Name);
        Terminal.WriteLine("[3] " + Server3Name);
        Terminal.WriteLine("\n");
        Terminal.WriteLine(">> ");
    }

    void ShowPasswordAccepted()
    {
        CurrentScreen = Screen.Win;
        Terminal.ClearScreen();
        Terminal.WriteLine("Password Acccepted");
        Terminal.WriteLine("**************************************");
    }


    void ShowLevelReward()
    {
        if(CurrentLevel == 1)
        {
            Terminal.WriteLine("Have a book");
        }
        else if (CurrentLevel == 2)
        {
            Terminal.WriteLine("Open the jail? Y/N");
        }
        else if (CurrentLevel == 3)
        {
            Terminal.WriteLine("Launch Rocket? Y/N");
        }
    }

    string GetScrambledLine(string str)
    {
        return "OVERFLOW:" + ".,;:1@![]{}<>@#$%^&()alphabetsatan".Anagram() + "......psswrd={" + str.Anagram() + "}";
    }

    string GetHackResult(string str)
    {
        return "psswrd={" + str.Anagram() +"}";
    }

    void StartGame()
    {
        CurrentScreen = Screen.Password;
        Terminal.ClearScreen();
        Terminal.WriteLine("<Server=\"" + CurrentServer + "\">\n...");
        Terminal.WriteLine("ERROR: 404 FORBIDDEN; ACCESS DENIED");
        Terminal.WriteLine(GetScrambledLine(CurrentPassword));
        Terminal.WriteLine("**************************************");
        Terminal.WriteLine("{menu, hack}; Enter Password: ");
        //Terminal.WriteLine("Enter Password:");
    }

    string PickRandomElement(string[] array)
    {
        int val = Random.Range(0, array.Length);
        return array[val];
    }

    void ParseMainMenuInput(string input)
    {
        if (input == "1")
        {
            CurrentLevel = 1;
            CurrentServer = Server1Name;
            CurrentPassword = PickRandomElement(Server1Passwords);
        }
        else if (input == "2")
        {
            CurrentLevel = 2;
            CurrentServer = Server2Name;
            CurrentPassword = PickRandomElement(Server2Passwords);
        }
        else if (input == "3")
        {
            CurrentLevel = 3;
            CurrentServer = Server3Name;
            CurrentPassword = PickRandomElement(Server3Passwords);
        }
        else
        {
            Terminal.WriteLine("Command not recognized");
            return;
        }

        StartGame();
    }


    void ParsePasswordInput(string input)
    {
        if(input == "hack")
        {
            Terminal.WriteLine(GetHackResult(CurrentPassword));
            return;
        }

        if (input == CurrentPassword)
        {
            ShowPasswordAccepted();
            ShowLevelReward();
        }
        else
        {
            Terminal.WriteLine("\nInvalid.  Enter Password:");
        }
    }

    void ParseWinInput(string input)
    {
        Terminal.WriteLine("\n");
        Terminal.WriteLine("type \"menu\" to restart");
    }

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
