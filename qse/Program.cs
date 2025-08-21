using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;

namespace qse
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Stopwatch sw  = new Stopwatch();
            
            
            
            sw.Start();
            
            int left = Console.WindowWidth-1;
            int top = Console.WindowHeight-2;
            int num = 0;
            string filename = "/home/qseft/test";
            string originalfile = File.ReadAllText(filename).Replace("\t", "    ");
            string filestr = "";
            List<char> file = new List<char>();
            foreach (char c in originalfile)
            {
                file.Add(c);
            }
            
            

            List<int> filelenghts = new List<int>();

            
            
            Console.CursorVisible = false;
            
            Console.Clear();
            Console.CursorVisible = true;
            int line = 1;
            int column = 0;
            int scroll = 0;
            int hscroll = 0;
            bool run = true;
            while (true)
            {
                ConsoleKeyInfo keyInfo1 = new ConsoleKeyInfo('\u001b', ConsoleKey.Escape, shift: false, alt: false, control: false);
                left = Console.WindowWidth-1;
                top = Console.WindowHeight-2;
                num = 0;
                filelenghts.Add(0);
                filestr = string.Concat(file);
                filelenghts = lenghts(filestr);
                    
                while (run)
                {
                    

                    write(scroll, hscroll, top, left, filelenghts, file, filename, filestr, line, column);

                    Console.SetCursorPosition(column, line);

                    keyInfo1 = Console.ReadKey(true);
                    
                    if (((keyInfo1.Modifiers & ConsoleModifiers.Alt) != 0))
                    {
                        run = false;
                        break;
                    }
                    
                    switch (keyInfo1.Key)
                    {
                        case ConsoleKey.UpArrow:
                            line--;
                            break;
                        case ConsoleKey.DownArrow:
                            line++;
                            break;
                        case ConsoleKey.LeftArrow:
                            column--;
                            break;
                        case ConsoleKey.RightArrow:
                            column++;
                            break;
                        case ConsoleKey.Backspace:
                            file.RemoveAt(filelenghts[(line + scroll) - 1] + column - 2 + (line + scroll) + hscroll);
                            column--;
                            break;
                        case ConsoleKey.Enter:
                            file.Insert(filelenghts[(line + scroll) - 1] + column - 1 + (line + scroll) + hscroll,
                                '\n');
                            column = 0;
                            line++;
                            break;
                        case ConsoleKey.Delete:
                            file.RemoveAt(filelenghts[(line + scroll) - 1] + column - 1 + (line + scroll) + hscroll);
                            break;
                        case ConsoleKey.PageUp:
                            scroll = scroll - top + (top / 5);
                            break;
                        case ConsoleKey.PageDown:
                            scroll = scroll + top - (top / 5);
                            break;
                        default:
                            file.Insert(filelenghts[(line + scroll) - 1] + column - 1 + (line + scroll) + hscroll,
                                keyInfo1.KeyChar);
                            column++;
                            break;
                    }

                    
                    left = Console.WindowWidth-1;
                    top = Console.WindowHeight-2;
                    num = 0;
                    filelenghts.Add(0);
                    filestr = string.Concat(file);
                    filelenghts = lenghts(filestr);
                    
                    //where is if(hscroll > lenght of a line) {stuck at end of it}

                    if (line <= 0)
                    {
                        line = 1;
                        scroll--;
                    }

                    if (column < 0 && hscroll == 0 && line + scroll >= 2 )
                    {
                        line--;
                        column = filelenghts[line + scroll] - filelenghts[line - 1 + scroll];
                    }

                    if (column < 0 && hscroll > 0)
                    {
                        column = 0;
                        hscroll--;
                    }

                    if (column < 0 && hscroll == 0 && (line + scroll < 2))
                    {
                        column = 0;
                    }

                    if (line <= 0)
                    {
                        line = 1;
                        scroll--;
                    }


                    


                    if (line >= top)
                    {
                        line = top - 1;
                        scroll++;
                    }

                    if (scroll <= 0)
                    {
                        scroll = 0;
                    }

                    if (line >= 1 && (line + scroll < filelenghts.Count-1))
                    {
                        
                        if (column > filelenghts[line + scroll] - filelenghts[line - 1 + scroll])
                        {
                            column = 0;
                            hscroll = 0;
                            line++;
                            if (line >= top)
                            {
                                line = top - 1;
                                column = filelenghts[line + scroll] - filelenghts[line - 1 + scroll];
                            }
                        }
                    }
                    if (scroll + top >= filelenghts.Count && filelenghts.Count - top - 1 > 0)
                    {
                        if (line < top)
                            line++;
                        scroll = filelenghts.Count - top - 1;
                    }
                    if (line >= top)
                    {
                        line = top - 1;
                    }
                    if (filelenghts[(line + scroll) - 1] + column - 1 + (line + scroll) + hscroll > filelenghts[filelenghts.Count - 1]+(filelenghts.Count - 3))
                    {
                        column = filelenghts[line + scroll] - filelenghts[line - 1 + scroll];
                    }

                    if (column > left)
                    {
                        hscroll = column - left + hscroll;
                        column = left;
                        if (hscroll > (filelenghts[line + scroll] - filelenghts[line - 1 + scroll]) - left)
                        {
                            hscroll = 0;
                            column = 0;
                            line++;
                        }
                    }

                    if (hscroll + column > (filelenghts[line + scroll] - filelenghts[line - 1 + scroll]))
                    {
                        column = (filelenghts[line + scroll] - filelenghts[line - 1 + scroll]) - hscroll;

                    }

                    
                    
                    



                }

                
                Console.SetCursorPosition(0, top-1);
                
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.ForegroundColor = ConsoleColor.DarkGray;
                
                for (int i = 0; i < left; i++)
                {
                    Console.Write(" ");
                }
                
                Console.SetCursorPosition(0, top-1);
                
                if (keyInfo1.Key == ConsoleKey.S)
                {
                    Console.Write("SAVING, DO NOT EXIT!!!");
                    File.WriteAllText(filename, filestr);
                    Console.CursorLeft = 0;
                    Console.Write("SAVED, PRESS ANY KEY TO RETURN");
                    Console.ReadKey(true);
                }
                if (keyInfo1.Key == ConsoleKey.G)
                {
                    Console.Write("got line: ");
                    Console.ForegroundColor = ConsoleColor.Black;
                    int lne = int.Parse(Console.ReadLine());
                    scroll = lne - line;
                }
                if (keyInfo1.Key == ConsoleKey.O)
                {
                    Console.Write("enter filepath: ");
                    Console.ForegroundColor = ConsoleColor.Black;
                    filename = Console.ReadLine();
                   originalfile = File.ReadAllText(filename).Replace("\t", "    ");
                    filestr = "";
                    file = new List<char>();
                    foreach (char c in originalfile)
                    {
                        file.Add(c);
                    }
                }

                if (keyInfo1.Key == ConsoleKey.C)
                {
                    Console.Write("enter command: ");
                    Console.ForegroundColor = ConsoleColor.Black;
                    string command = Console.ReadLine();
                }
                
                run = true;
                
            }
        }

        public static List<int> lenghts(string file)
        {
            int num = 0;
            List<int> filelenghts = new List<int>();
            filelenghts.Add(0);
            foreach (string lne in file.Split('\n'))
            {
                num = num + lne.Length;
                filelenghts.Add(num);
            }
            return filelenghts;
        }

        public static void write(int scroll, int hscroll, int top, int left, List<int> filelenghts, List<char> file, string filename, string filestr,int line, int column)
        {
            Console.CursorVisible = false;
            Console.WriteLine("\u001b]0;My Custom Console Title\u0007");
            Console.Title = "Qseft's simple editor - editing " + filename;
            
            
            Console.ResetColor();
            
            StringWriter stringWriter = new StringWriter();
            
                Console.SetOut(stringWriter);
                
                int neededoutputlines = filelenghts.Count - 1;
                if (neededoutputlines > top) neededoutputlines = top;
            
                for (int i = scroll; i < neededoutputlines+scroll-1; i++)
                {
                    if (filelenghts[i + 1] - filelenghts[i] - hscroll <= left && filelenghts[i + 1] - filelenghts[i] - hscroll >= 0)
                    {
                        for (int j = filelenghts[i]+i+ hscroll ; j <= filelenghts[i + 1]+i; j++)
                        {
                            Console.Write(file[j]);
                        }
                    }
                    else if (filelenghts[i + 1] - filelenghts[i] - hscroll < 0)
                    {
                        Console.Write("\n");
                    }
                    else
                    {
                        for (int j = filelenghts[i]+i + hscroll ; j < filelenghts[i]+left+1+i+ hscroll - 1; j++)
                        {
                            Console.Write(file[j]);
                        }
                        Console.Write(">\n");
                    }

                

                }
                
                
                string output = stringWriter.ToString();
                
                Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
                Console.Clear();
                
                Console.SetCursorPosition(0, 1);
                Console.WriteLine(output);
            Console.SetCursorPosition(0, 0);
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write(" QSE");
            for (int i = 0; i < left-(4+filename.Length); i++)
                Console.Write(" ");
            Console.Write(filename+" ");
            Console.SetCursorPosition(0, top);
            Console.Write(" " + filelenghts.Count.ToString() + " lines loaded");
            for (int i = 0; i < left-(14+filelenghts.Count.ToString().Count()+(filelenghts[line-1]+column-1+line).ToString().Count()); i++)
                Console.Write(" ");
            Console.Write(filelenghts[line - 1] + column - 1 + line + " ");
            Console.ResetColor();
            Console.CursorVisible = true;
        }

    }
}