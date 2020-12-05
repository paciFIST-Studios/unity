using UnityEngine;

public class Hacker : MonoBehaviour
{
    const int SCREEN_WIDTH = 38;

    // game data
    string Server1Name = "library.local";
    string Server2Name = "emergency.local";
    string Server3Name = "nasa.gov";
    string[] Server1Passwords = { "book", "shelf", "read", "trust", "public", "stack"  };
    string[] Server2Passwords = { "ambulance", "hospital", "medic", "rescue", "crisis", "hazard",};
    string[] Server3Passwords = { "kepler", "telescope", "methane", "satellite", "galileo", "scientist",};

    // game state
    enum Screen { Intro1, Intro2, Intro3, MainMenu, Password, Win };
    Screen CurrentScreen = Screen.MainMenu;
    string CurrentServer   = "";
    string CurrentPassword = "";
    int    HackCountdown   = 0;

    // - Display fns -------------------------------------------------------------

    void ShowIntro1Screen()
    {
        CurrentScreen = Screen.Intro1;
        Terminal.ClearScreen();
        Terminal.WriteLine("\n\n\n");
        Terminal.WriteLine(CenterString("HACKER"));
        Terminal.WriteLine("\n\n");
        Terminal.WriteLine("\n\n\n\n[enter]");
    }

    void ShowIntro2Screen()
    {
        CurrentScreen = Screen.Intro2;
        Terminal.ClearScreen();
        Terminal.WriteLine("This is a spelling-guessing game.");
        Terminal.WriteLine("Unscramble the letters to make words.");
        Terminal.WriteLine("");
        Terminal.WriteLine("look for password hints: psswrd={ABC}");
        Terminal.WriteLine("");
        Terminal.WriteLine("Type \"hack\", to attack the system");
        Terminal.WriteLine("Type \"menu\", to go back");
        Terminal.WriteLine("\n\n\n\n[enter]");
    }

    void ShowIntro3Screen()
    {
        CurrentScreen = Screen.Intro3;
        Terminal.ClearScreen();
        Terminal.WriteLine("\n\n\n");
        Terminal.WriteLine(CenterString("DONT"));
        Terminal.WriteLine(CenterString("KILL"));
        Terminal.WriteLine(CenterString("NE1"));
        Terminal.WriteLine("\n\n\n\n[enter]");
    }

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

    void ShowConnectionAttempt()
    {
        CurrentScreen = Screen.Password;
        Terminal.ClearScreen();
        Terminal.WriteLine("<Server=\"" + CurrentServer + "\">");
        Terminal.WriteLine("ERROR: 404 FORBIDDEN; ACCESS DENIED");
        Terminal.WriteLine(GetScrambledLine(CurrentPassword));
        Terminal.WriteLine("**************************************");
        Terminal.WriteLine("cmds={menu, hack}; \nEnter Password: ");
    }

    void ShowPasswordAccepted()
    {
        CurrentScreen = Screen.Win;
        Terminal.ClearScreen();
        Terminal.WriteLine("Password Accepted");
        Terminal.WriteLine("<Server=\"" + CurrentServer + "\">");
        Terminal.WriteLine("**************************************");
    }
    
    void ShowLevelReward()
    {
        if(CurrentServer == Server1Name)
        {
            Terminal.WriteLine("Reading Program Attendance:");
            Terminal.WriteLine("1  Jun  ( 7)");
            Terminal.WriteLine("3  Jul  (15)");
            Terminal.WriteLine("1  Aug  (37)");
            Terminal.WriteLine("\n\n\n[enter]");
        }
        else if (CurrentServer == Server2Name)
        {
            Terminal.WriteLine("Vehicles in transit: 17");
            Terminal.WriteLine("Incidents awaiting pickup: 4");
            Terminal.WriteLine("Mass casualty: none");
            Terminal.WriteLine("\n\n\n\n[enter]");
        }
        else if (CurrentServer == Server3Name)
        {
            Terminal.WriteLine("Crewed   satellites: 5");
            Terminal.WriteLine("Uncrewed satellites: 8762");
            Terminal.WriteLine("Tracked  satellites: 101993");
            Terminal.WriteLine("\n\n\n\n[enter]");
        }
    }

    // - Utility fns ------------------------------------------------------------

    string PickRandomElement(string[] array)
    {
        Random.InitState((int)Time.time);
        int val = Random.Range(0, array.Length);
        return array[val];
    }

    string GetScrambledLine(string str)
    {
        return "OVERFLOW:" + ".,;:1@![]{}<>@#$%^&()alphabetsatan".Anagram() + "......psswrd={" + str.Anagram() + "}";
    }

    string CenterString(string str)
    {
        if (str.Length >= SCREEN_WIDTH)
        {
            return str;
        }

        int leading_space = (int)((SCREEN_WIDTH - str.Length) * 0.5);

        string result = "";
        for(int i = 0; i < leading_space; i++)
        {
            result += " ";
        }

        return result + str;
    }

    string GetHackResult(string str, bool unscrambled=false)
    {
        if(unscrambled)
        {
            return "psswrd={" + str + "}";
        }

        return "psswrd={" + str.Anagram() +"}";
    }

    void PerformHack()
    {
        --HackCountdown;

        if (HackCountdown > 0)
        {
            Terminal.WriteLine(GetHackResult(CurrentPassword));
        }
        else
        {
            Terminal.WriteLine(GetHackResult(CurrentPassword, true));
        }
    }

    // - Parse fns --------------------------------------------------------------

    void ParseMainMenuInput(string input)
    {
        if (input == "1")
        {
            CurrentServer = Server1Name;
            CurrentPassword = PickRandomElement(Server1Passwords);
        }
        else if (input == "2")
        {
            CurrentServer = Server2Name;
            CurrentPassword = PickRandomElement(Server2Passwords);
        }
        else if (input == "3")
        {
            CurrentServer = Server3Name;
            CurrentPassword = PickRandomElement(Server3Passwords);
        }
        else
        {
            Terminal.WriteLine("Command not recognized");
            return;
        }

        // however long the word is, half that many guesses will just tell you the password
        HackCountdown = (int)(CurrentPassword.Length * 0.5f);
        ShowConnectionAttempt();
    }
    
    void ParsePasswordInput(string input)
    {
        if(input == "hack")
        {
            PerformHack();
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
        Terminal.ClearScreen();
        Terminal.WriteLine("type \"menu\" to restart");
    }

    // --------------------------------------------------------------------------

    void OnUserInput(string input)
    {
        if (input == "menu")
        {
            ShowMainMenu();
            return;
        }
        else if(input == "quit" || input == "exit" || input == "close")
        {
            Application.Quit();
            Terminal.WriteLine("Close Tab (ctrl+w) to exit web build");
            return;
        }


        if (CurrentScreen == Screen.Intro1)
        {
            ShowIntro2Screen();
        }
        else if (CurrentScreen == Screen.Intro2)
        {
            ShowIntro3Screen();
        }
        else if (CurrentScreen == Screen.Intro3)
        { 
            ShowMainMenu();
        }
        else if (CurrentScreen == Screen.MainMenu)
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
        ShowIntro1Screen();
    }

}
