using System;
using System.Collections.Generic;
using System.IO;
using BreadEngine;

class Program 
{
    private static readonly string AppDir = AppDomain.CurrentDomain.BaseDirectory;
    private static Dictionary<int, LayoutData> _scenes = [];
    private static void Main(string[] args)
    {
        FastConsole.Clear();
        LoadLayouts();
        SetLayoutData(0);
        
        UIManager.addUniversalKeyBind(ConsoleKey.Escape, () => exit());

        Button testButton = (Button) UIManager.GetComponent("test");
        testButton.SetCallback(() => 
        {
            testButton.text = "Loading...";
            SetLayoutData(0);
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
    private static void LoadLayouts()
    {
        int currentIndex = 0;
        foreach(string s in Directory.GetFiles(AppDir+"/Layouts/"))
        {
            if(Path.GetExtension(s) == ".lyt")
            {
                _scenes.Add(currentIndex, LayoutReader.Read(s));
                currentIndex++;
            }
        }
    }
    private static void SetLayoutData(int data)
    {
        if(_scenes.TryGetValue(data, out LayoutData lyt))
        {
            UIManager.setLayout(lyt);
        }
        else
        {
            throw new LayoutMissingException(data.ToString());
        }
    }
    private static bool exit() 
    {
        Console.ResetColor();
        Console.Clear();
        Console.SetCursorPosition(0, 0);
        Console.WriteLine("Program stopped execution");
        Environment.Exit(0);
        return false;
    }
}