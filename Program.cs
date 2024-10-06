﻿using System;
using System.IO;
using BreadEngine;

class Program 
{
    static void Main(string[] args)
    {
        FastConsole.Clear();

        LayoutData scene1 = LayoutReader.Read(AppDomain.CurrentDomain.BaseDirectory+"/.Layout");
        LayoutData scene2 = LayoutReader.Read(AppDomain.CurrentDomain.BaseDirectory+"/.Layout2");
        UIManager.setLayout(scene1);

        UIManager.addUniversalKeyBind(ConsoleKey.Escape, () => exit());

        Button testButton = (Button) UIManager.GetComponent("test");
        testButton.SetCallback(() => 
        {
            testButton.text = "Loading...";
            UIManager.setLayout(scene1);
        });

        Console.CancelKeyPress += delegate 
        {
            Console.Clear();
            Console.ResetColor();
        };

        UIManager.StartLoop();

        //Called when we exit the ui loop
        exit();
    }

    static bool exit() 
    {
        Console.ResetColor();
        Console.Clear();
        Console.SetCursorPosition(0, 0);
        Console.WriteLine("Program stopped execution");
        Environment.Exit(0);
        return false;
    }
}