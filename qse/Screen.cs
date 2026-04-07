using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;
using TextCopy;

namespace qse
{
    class Screen
    {
        public static void start(string s)
        {
            Console.Write(s);
            Console.Clear();
            Console.Write(s);
            int sizex = 42;
            int sizey = 9;
            
            int off  = (Console.WindowWidth - sizex)/2;
            Console.CursorTop  = (Console.WindowHeight - sizey)/2-1;
            
            string w = "      __________    _________    _________\n     /  ____   /   /  ______/   /  ______/\n    /  /   /  /   /  /_____    /  /___\n   /  /   /  /   /_____   /   /  ____/\n  /  /___/  /   ______/  /   /  /_____\n /_____   _/   /________/   /________/\n      /__/\n\nqseft's       simple       editor";
            
            foreach(string st in w.Split('\n'))
            {
                for(int i = 0; i < off; i++)
                    Console.Write(" ");
                Console.Write(st+"\n");
            }
            Console.CursorLeft = Console.WindowWidth - 10;
            Console.CursorTop = Console.WindowHeight - 1;
            Console.Write("loading...");
            Console.SetCursorPosition(0, 0);
            
            
        }
    }
}

