using UnityEngine;

public class Hacker : MonoBehaviour
{
    // game data
    string Server1Name = "library.local";
    string Server2Name = "emergency.local";
    string Server3Name = "nasa.gov";
    string[] Server1Passwords = { "book", "shelf", "read", "trust", "public", "stack"  };
    string[] Server2Passwords = { "ambulance", "hospital", "medic", "rescue", "crisis", "hazard",};
    string[] Server3Passwords = { "kepler", "telescope", "methane", "satellite", "galileo", "scientist",};

    // game state
    enum Screen { MainMenu, Password, Win };
    Screen CurrentScreen = Screen.MainMenu;
    string CurrentServer   = "";
    string CurrentPassword = "";
    int    HackCountdown   = 0;

    // - Display fns -------------------------------------------------------------

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
        Terminal.WriteLine("<Server=\"" + CurrentServer + "\">\n...");
        Terminal.WriteLine("ERROR: 404 FORBIDDEN; ACCESS DENIED");
        Terminal.WriteLine(GetScrambledLine(CurrentPassword));
        Terminal.WriteLine("**************************************");
        Terminal.WriteLine("{menu, hack}; Enter Password: ");
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
        if(CurrentServer == Server1Name)
        {
            Terminal.WriteLine("Reading Program Attendance:");
            Terminal.WriteLine("1  Jun  ( 7)");
            Terminal.WriteLine("3  Jul  (15)");
            Terminal.WriteLine("1  Aug  (37)");
        }
        else if (CurrentServer == Server2Name)
        {
            Terminal.WriteLine("Vehicles in transit: 17");
            Terminal.WriteLine("Incidents awaiting pickup: 4");
            Terminal.WriteLine("Mass casualty: none");
        }
        else if (CurrentServer == Server3Name)
        {
            Terminal.WriteLine("Crewed   satellites: 5");
            Terminal.WriteLine("Uncrewed satellites: 8762");
            Terminal.WriteLine("Tracked  satellites: 101993");
        }
    }

    // - Utility fns ------------------------------------------------------------

    string PickRandomElement(string[] array)
    {
        int val = Random.Range(0, array.Length);
        return array[val];
    }

    string GetScrambledLine(string str)
    {
        return "OVERFLOW:" + ".,;:1@![]{}<>@#$%^&()alphabetsatan".Anagram() + "......psswrd={" + str.Anagram() + "}";
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
        Terminal.WriteLine("\n");
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
