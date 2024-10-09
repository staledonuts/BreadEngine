using System;
using System.Diagnostics;
namespace BreadEngine;

[System.Serializable]
public class LayoutMissingException : System.Exception
{
    public LayoutMissingException() { }
    public LayoutMissingException(string message) : base(message) 
    {
        Console.WriteLine("Cannot find layout: " +message);
    }
    public LayoutMissingException(string message, System.Exception inner) : base(message, inner) { }
    
}