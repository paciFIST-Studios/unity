using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hacker : MonoBehaviour
{
    void ShowMainMenu() {
        Terminal.ClearScreen();
        Terminal.WriteLine("What would you like to hack into?");
        Terminal.WriteLine("\n");
        Terminal.WriteLine("[1] local library");
        Terminal.WriteLine("[2] police station");
        Terminal.WriteLine("[3] nasa");
        Terminal.WriteLine("\n");
        Terminal.WriteLine("Enter Selection:");
    }

    void Start () {
        ShowMainMenu();
    }

    void Update () {

	}
}
