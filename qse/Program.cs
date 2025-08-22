using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;
using rohankapoor.AutoPrompt;
using WindowsInput;

namespace qse
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            /* blue
             * if, else, switch, for, while, do, break, continue, return, throw, try, catch, finally, public, private, protected, internal,static, abstract, sealed, virtual, override, readonly, const, volatile, namespace, using, typeof, sizeof, is, as, ref, out, in, params, operator, implicit, explicit, async, await, var, dynamic, nameof, record, init, global, required, scoped, with
             * cyan
             * int, string, bool, float, double, char, object, decimal, void
             * purple
             * true, false, null, 42, 3.14
             * gray
             * #region, #define, #if, #endif, #else, #pragma, #warning, #error
             * green
             * //, /* * /, ///
             * red
             * "Hello"
             */

            
            
            Stopwatch sw  = new Stopwatch();
            
            
            
            sw.Start();
            
            int left = Console.WindowWidth-1;
            int top = Console.WindowHeight-2;
            int num = 0;
            string homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string filename = homeDirectory + "/test";
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
            var keyboard = new InputSimulator();

            if (!Directory.Exists(homeDirectory + "/.qse"))
            {
                Directory.CreateDirectory(homeDirectory + "/.qse");
            }
            
            
            
            
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
                    string ogfilename = filename;
                    filename = AutoPrompt.PromptForInput("save to: ", filename);
                    Console.SetCursorPosition(0, top-1);
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
                    string dfromf = "";
                    do
                    {
                        Console.SetCursorPosition(0, top - 1);
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write("enter filepath: ");
                        Console.ForegroundColor = ConsoleColor.Black;
                        filename = AutoPrompt.PromptForInput("", filename);
                        Console.SetCursorPosition(0, top - 1);
                        for (int i = 0; i < left; i++)
                        {
                            Console.Write(" ");
                        }
                        

                        for (int i = 0; i < filename.Split('/').Length - 1; i++)
                        {
                            dfromf = dfromf +  filename.Split('/')[i] + "/";
                        }
                        
                        
                    }while(!Directory.Exists(dfromf));


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
            string[] blue = {"if","else", "switch", "for", "while", "do", "break", "continue", "return", "throw", "try", "catch", "finally", "public", "private", "protected", "internal","static", "abstract", "sealed", "virtual", "override", "readonly", "const", "volatile", "namespace", "using", "typeof", "sizeof", "is", "as", "ref", "out", "in", "params", "operator", "implicit", "explicit", "async", "await", "var", "dynamic", "nameof", "record", "init", "global", "required", "scoped", "with", "new", "true", "false"};
            string[] cyan = {"int", "string", "bool", "float", "double", "char", "object", "decimal", "void", "Stopwatch" };
            string[] purple = {"true", "false", "null" };
            string[] gray = {"#" };
            string[] green = {"//" };
            string[] red = {" "};
            
            
            
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
                    string writeline = "";
                    
                    if (filelenghts[i + 1] - filelenghts[i] - hscroll <= left && filelenghts[i + 1] - filelenghts[i] - hscroll >= 0)
                    {
                        for (int j = filelenghts[i]+i+ hscroll ; j <= filelenghts[i + 1]+i; j++)
                        {
                            writeline = writeline + file[j];
                            //Console.Write(file[j]);
                        }
                    }
                    else if (filelenghts[i + 1] - filelenghts[i] - hscroll < 0)
                    {
                        writeline = writeline + "\n";
                        //Console.Write("\n");
                    }
                    else
                    {
                        for (int j = filelenghts[i]+i + hscroll ; j < filelenghts[i]+left+1+i+ hscroll - 1; j++)
                        {
                            writeline = writeline + file[j];
                            //Console.Write(file[j]);
                        }
                        writeline = writeline + ">\n";
                        //Console.Write(">\n");
                    }
                    
                    string[] writeword =  writeline.Split(' ');

                    for (int j = 0; j < writeword.Length; j++)
                    {
                        
                        
                        Console.Write("\x1b[0m");
                        
                        if(j!=0)
                            Console.Write(" ");
                        
                        if(Array.Exists(blue, x => x == writeword[j]))
                        {
                            Console.Write("\x1b[34m");
                        }
                        else if(Array.Exists(cyan, x => x == writeword[j]))
                        {
                            Console.Write("\x1b[36m");
                        }
                        else if(Array.Exists(purple, x => x == writeword[j]) || float.TryParse(writeword[j], out float num))
                        {
                            Console.Write("\x1b[35m");
                        }

                        if (writeword[j].Length > 1)
                        {
                            if (writeword[j][0] == '#')
                            {
                                Console.Write("\x1b[90m");
                            }
                            else if (writeword[j][0] == '/' && writeword[j][1] == '/')
                            {
                                Console.Write("\x1b[32m");
                            }
                            else if (writeword[j][0] == '"' && writeword[j][writeword[j].Length - 1] == '"')
                            {
                                Console.Write("\x1b[31m");
                            }
                        }

                        if (writeword[j].Length >= 4)
                        {
                            if (writeword[j][0] == 'L' && writeword[j][1] == 'i' && writeword[j][2] == 's' && writeword[j][3] == 't')
                            {
                                Console.Write("\x1b[92m");
                            }
                        }

                        if (writeword[j].Length >= 7)
                        {
                            if (writeword[j][0] == 'C' && writeword[j][1] == 'o' && writeword[j][2] == 'n' && writeword[j][3] == 's'&& writeword[j][4] == 'o' && writeword[j][5] == 'l' && writeword[j][6] == 'e')
                            {
                                Console.Write("\x1b[33m");
                            }
                        }


                       
                        
                        
                        Console.Write(writeword[j]);
                        
                        
                        
                    }
                    
                    
                    
                    
                }
                
                
                string output = stringWriter.ToString();
                
                Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
                Console.Clear();
                
                Console.SetCursorPosition (0, 1);
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