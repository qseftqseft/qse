using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;
using TextCopy;

namespace qse
{
    class Utils
    {
        public static string prompt(string tex, string pr, string dftex = "")
        {
            Console.Write(tex);
            int defcl = Console.CursorLeft;
            List<char> inp = pr.ToList();
            int col = pr.Length;
            int prevln = pr.Length;
            Console.Write(dftex);
            ConsoleKeyInfo keyInfo1 = new ConsoleKeyInfo();

            while(keyInfo1.Key != ConsoleKey.Enter)
            {
                Console.CursorLeft = defcl;
                for(int i = 0; i < prevln; i++) Console.Write(" ");
                Console.CursorLeft = defcl;
                Console.Write(String.Concat(inp));
                Console.CursorLeft = col+defcl;

                prevln = inp.Count();
                keyInfo1 = Console.ReadKey(true);

                switch (keyInfo1.Key)
                {
                    case ConsoleKey.LeftArrow:
                        if(col > 0) col--;
                        break;

                    case ConsoleKey.RightArrow:
                        if(col < inp.Count) col++;
                        break;

                    case ConsoleKey.Backspace:
                        if(col > 0)
                        {
                            inp.RemoveAt(col-1);
                            col--;
                        }
                        break;

                    case ConsoleKey.Enter:
                        continue;


                    case ConsoleKey.Delete:
                        if(col < inp.Count) inp.RemoveAt(col);
                        break;

                    default:
                        if (!char.IsControl(keyInfo1.KeyChar) && keyInfo1.KeyChar != '\0')
                        {
                            inp.Insert(col, keyInfo1.KeyChar);
                            col++;
                        }
                        break;
                }
            }
            return String.Concat(inp);
        }
        public static List<int> lenghts(string file)
        {
            int num = 0;
            List<int> filelenghts = new List<int>();
            filelenghts.Add(0);

            foreach (string lne in file.Split('\n'))
            {
                num = num + lne.Count(c => !char.IsControl(c));
                filelenghts.Add(num);
            }

            return filelenghts;
        }

        public static int writeMenu(string[] array, int pfx, string top, string bottom, bool clrsc, string bg)
        {
            if(clrsc) Console.Write(bg+"\x1b[2J");
            Console.SetCursorPosition(0,Console.WindowHeight-2);
            Console.WriteLine(bottom);
            Console.SetCursorPosition(0,0);
            Console.WriteLine(top);
            int indx = 0;
            bool e = true;
            while(e)
            {
                Console.CursorLeft=pfx;
                Console.CursorTop=2;
                for (int i = 0; i < array.Length; i++)
                {
                    Console.CursorLeft=pfx;
                    if(i != indx)
                        Console.Write(bg+"\x1b[0;90m [ ] "+ array[i] + "\n");
                    if(i == indx)
                        Console.Write(bg+"\x1b[1;37m [*] \x1b[0m\x1b[1;37m" + array[i] + "\n");
                    Console.CursorLeft=pfx;
                    Console.Write("\x1b[0m");
                }

                Console.CursorLeft=pfx + 2;
                Console.CursorTop=indx+2;
                ConsoleKeyInfo k = Console.ReadKey();

                switch(k.Key)
                {
                    case ConsoleKey.DownArrow:
                        indx++;
                        break;
                    case ConsoleKey.UpArrow:
                        indx--;
                        break;
                    case ConsoleKey.Enter:
                        e=false;
                        break;
                }
                if(indx >= array.Length)
                    indx = array.Length - 1;
                if(indx < 0)
                    indx = 0;
                if(k.Key == ConsoleKey.Escape)
                {
                    Console.CursorTop=indx+2;
                    for (int i = 0; i < array.Length; i++)
                    {
                        Console.CursorLeft=pfx;
                        Console.Write("\x1b[0m"+bg+"     ");
                        if(array[i] != null) for (int j = 0; j < array[i].Length; j++) Console.Write(" ");
                        Console.Write("\n");
                    }
                    e=false;
                    indx = -1;
                    break;
                }

            }


            return indx;
        }

        public static void debug(string info)
        {
            int ct = Console.CursorTop;
            int cl = Console.CursorLeft;

            Console.SetCursorPosition(Console.WindowWidth-info.Length, Console.WindowHeight);

            Console.Write(info);

            Console.SetCursorPosition(cl, ct);
        }

        public static bool StartsWith(string what, string s)
        {
            if(what.Length >= s.Length)
            {
                for(int i = 0; i < s.Length; i++)
                {
                    if(s[i] != what[i]) return false;
                }
                return true;
            }
            return false;
        }
        public static string ArrayBlackBox(string[] arr,string bgcol, string defcol, int offset, int start, char[] ignclr)
        {
            int cl = Console.CursorLeft-offset;
            string outp = "";

            if(cl >= 0)
            {
                int ct = Console.CursorTop;
                arr = arr.Skip(start).ToArray();
                for (int i = 0; i < arr.Length; i++)
                    if(arr[i].Length > Console.WindowWidth-cl) arr[i] = arr[i].Substring(0, Console.WindowWidth-cl);
                    if(arr.Length > 6) arr = [arr[0], arr[1], arr[2], arr[3], arr[4], arr[5]];
                    int width = arr.Aggregate(string.Empty, (seed, f) => f.Length > seed.Length ? f : seed).Length;
                int height = arr.Length;




                for (int i = 0; i < height; i++)
                {
                    while(cl+width >= Console.WindowWidth) width--;
                    Console.CursorLeft = cl+width;
                    
                    Console.CursorTop++;
                    Console.Write(defcol);

                    Console.CursorLeft = cl;

                    for (int j = 0; j < width; j++)
                    {
                        Console.Write(bgcol+" ");
                    }

                    Console.CursorLeft = cl;

                    Console.Write(arr[i]);

                }
                Console.Write(defcol);
            }
            if(arr.Length > 0) outp=arr[0];
            List<int> endarr = new List<int>();

            foreach(char c in ignclr)
            {
                int indxof = outp.IndexOf(c);
                if(indxof > 0) endarr.Add(indxof);
            }

            endarr.Add(outp.Length);
            outp = outp.Substring(0, endarr.Min());
            if(offset <= outp.Length) outp =  outp.Substring(offset);
            return outp;
        }
        public static int curchar(List<int> filelenghts, int line, int scroll, int column, int hscroll)
        {
            return filelenghts[(line + scroll) - 1] + column - 1 + (line + scroll) + hscroll;
        }
        
        
        
        
        
    }
}
