using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hacker : MonoBehaviour
{
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

    void OnUserInput(string input)
    {
        if (input == "1")
        {
            Terminal.WriteLine("level 1");
        }
        else if (input == "2")
        {
            Terminal.WriteLine("level 2");
        }
        else if (input == "3")
        {
            Terminal.WriteLine("level 3");
        }
        else if (input == "menu")
        {
            print("reload menu");
            ShowMainMenu();
        }
        else
        {
            Terminal.WriteLine("Command not recognized");
        }        
    }

    void Start ()
    {
        ShowMainMenu();
    }

}
