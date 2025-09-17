using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;
using rohankapoor.AutoPrompt;
using TextCopy;

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
            string homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string filename = homeDirectory + "/test";
            List<char> file = OpenFile(filename, out string originalfile);
            string filestr = "";
            bool marked = false;
            int mark = 0;
            
            
            List<int> filelenghts = new List<int>();

            Console.CursorVisible = false;
            
            Console.Clear();
            Console.CursorVisible = true;
            int line = 1;
            int column = 0;
            int scroll = 0;
            int hscroll = 0;
            int tab = 0;
            bool run = true;
            
            string settingsfile = "settings";
            
            MakeSureConfDirExists(settingsfile, homeDirectory);
            
            string[] settings = File.ReadAllText(homeDirectory + "/.qse/" + settingsfile).Split('\n');
            
            char[] ignclr = (settings[0] + "Æ\n").Split('Æ').SelectMany(s => s.ToCharArray()).ToArray();
            
            string[] black = settings[1].Split('Æ');
            string[] red = settings[2].Split('Æ');
            string[] green = settings[3].Split('Æ');
            string[] yellow = settings[4].Split('Æ');
            string[] blue = settings[5].Split('Æ');
            string[] magenta = settings[6].Split('Æ');
            string[] cyan = settings[7].Split('Æ');
            string[] white = settings[8].Split('Æ');
            string[] bblack = settings[9].Split('Æ');
            string[] bred = settings[10].Split('Æ');
            string[] bgreen = settings[11].Split('Æ');
            string[] byellow = settings[12].Split('Æ');
            string[] bblue = settings[13].Split('Æ');
            string[] bmagenta = settings[14].Split('Æ');
            string[] bcyan = settings[15].Split('Æ');
            string[] bwhite = settings[16].Split('Æ');
            
            string normal = "\x1b[90m";
            string number = "\x1b[95m";
            char strng = char.Parse(settings[17]);
            string strgclr = "\x1b[91m";
            string cmntclr = "\x1b[32m";
                        
            string term = settings[19];
            string tflags = settings[20];
            string tcommand = settings[21];
            
            if (settings[18] == "1")
            {
                tcommand = filename + tcommand;
            }
            
            
            string projectsstr = File.ReadAllText(homeDirectory + "/.qse/projects.list");
            string[] project = projectsstr.Split('\n');
            
            
            List<string>[] projects = Enumerable.Range(0, project.Length).Select(_ => new List<string>()).ToArray();
            //projects[project_no.] = list_of_files
            
            if(project.Length > 0)
            {
                for(int i = 0; i < projects.Length; i++)
                {
                    for(int j = 0; j < project[i].Split(' ').Length; j++)
                    {
                        projects[i].Add(project[i].Split(' ')[j]);
                    }
                }
            }    
            string currentproject = projects[0][0];
            int currentprojectindx = 0;
            
            
            while (true)
            {
                ConsoleKeyInfo keyInfo1 = new ConsoleKeyInfo('\u001b', ConsoleKey.Escape, shift: false, alt: false,
                    control: false);
                left = Console.WindowWidth - 1;
                top = Console.WindowHeight - 2;
                num = 0;
                filelenghts.Add(0);
                filestr = string.Concat(file);
                filelenghts = lenghts(filestr);
                
                while (run)
                {
                    left = Console.WindowWidth - 1;
                    top = Console.WindowHeight - 2;
                    bool r = false;
                    
                    write(scroll, hscroll, top, left, filelenghts, file, filename, filestr, line, column, currentproject, strng, strgclr, cmntclr, ignclr, black, red, green, yellow, blue, magenta, cyan, white, bblack, bred, bgreen, byellow, bblue, bmagenta, bcyan, bwhite, normal, number, marked, mark);
                
                    Console.SetCursorPosition(column, line);
                    
                    Console.CancelKeyPress += (sender, e) => { e.Cancel = true;};
                    
                    
                        keyInfo1 = Console.ReadKey(true);
                    
                                        
                    if (((keyInfo1.Modifiers & ConsoleModifiers.Alt) != 0))
                    {
                        run = false;
                        break;
                    }

                    if (((keyInfo1.Modifiers & ConsoleModifiers.Control) != 0))
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
                            r = true;
                            break;
                        case ConsoleKey.Tab:
                            file.Insert(filelenghts[(line + scroll) - 1] + column - 1 + (line + scroll) + hscroll, ' ');
                            file.Insert(filelenghts[(line + scroll) - 1] + column - 1 + (line + scroll) + hscroll, ' ');
                            file.Insert(filelenghts[(line + scroll) - 1] + column - 1 + (line + scroll) + hscroll, ' ');
                            file.Insert(filelenghts[(line + scroll) - 1] + column - 1 + (line + scroll) + hscroll, ' ');
                            column = column + 4;
                            break;
                        case ConsoleKey.Backspace:
                            if(filelenghts[(line + scroll) - 1] + column - 2 + (line + scroll) + hscroll >= 0)
                            {
                                file.RemoveAt(filelenghts[(line + scroll) - 1] + column - 2 + (line + scroll) + hscroll);
                                column--;
                                if (column < 0 && hscroll == 0 && line + scroll >= 2)
                                {
                                    line--;
                                    column = filelenghts[line + scroll] - filelenghts[line - 1 + scroll];
                                }
                            }
                            break;
                        case ConsoleKey.Enter:
                            tab = 0;
                            while (file[filelenghts[line + scroll - 1] + line + scroll - 1 + tab] == ' ')
                            {
                                tab++;
                            }
                            
                            file.Insert(filelenghts[(line + scroll) - 1] + column - 1 + (line + scroll) + hscroll, '\n');
                                                        
                            for(int i = 0; i < tab; i++)
                            {
                                file.Insert(filelenghts[(line + scroll) - 1] + column - 1 + (line + scroll) + hscroll + 1, ' ');
                            }
                            column = 0;
                            line++;
                            for(int i = 0; i < tab; i++)
                            {
                                column++;
                            }
                            break;
                        case ConsoleKey.Delete:
                            if(filelenghts[(line + scroll) - 1] + column - 1 + (line + scroll) + hscroll < file.Count){
                            file.RemoveAt(filelenghts[(line + scroll) - 1] + column - 1 + (line + scroll) + hscroll);}
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

                    
                    HandleRC(line, column, scroll, hscroll, file, r, out line, out column, out scroll, out hscroll, out file, out filelenghts);
                    
                    
                }

                if ((((keyInfo1.Modifiers & ConsoleModifiers.Alt) != 0)) && (keyInfo1.Modifiers & ConsoleModifiers.Shift) == 0)
                {
                    Console.SetCursorPosition(0, top - 1);
                    
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    
                    for (int i = 0; i <= left; i++)
                    {
                        Console.Write(" ");
                    }

                    Console.SetCursorPosition(0, top - 1);
                    
                    if (keyInfo1.Key == ConsoleKey.S || keyInfo1.Key == ConsoleKey.R)
                    {
                        if(keyInfo1.Key == ConsoleKey.S)
                            filename = AutoPrompt.PromptForInput("save to: ", filename);
                        Console.SetCursorPosition(0, top - 1);
                        Console.Write("SAVING, DO NOT EXIT!!!");
                        File.WriteAllText(filename, filestr);
                        Console.CursorLeft = 0;
                        for(int i = 0; i <= left; i++)
                        {
                            Console.Write(" ");
                        }
                        Console.CursorLeft = 0;
                        if(keyInfo1.Key == ConsoleKey.S)
                        {
                            Console.Write("SAVED, PRESS ANY KEY TO RETURN");
                            Console.ReadKey(true);
                        }
                    }
                    
                    if (keyInfo1.Key == ConsoleKey.A)
                    {
                        marked = !marked;
                        mark = filelenghts[(line + scroll) - 1] + column - 1 + (line + scroll) + hscroll;
                    }
                    
                    if (keyInfo1.Key == ConsoleKey.G)
                    {
                        Console.Write("got line: ");
                        Console.ForegroundColor = ConsoleColor.Black;
                        string inp = Console.ReadLine();
                        int lne = scroll+line;
                        do{}while(!int.TryParse(inp, out lne));
                        
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
                                dfromf = dfromf + filename.Split('/')[i] + "/";
                            }
                            
                            
                        } while (!Directory.Exists(dfromf));
                        if(!File.Exists(filename))
                        {
                            File.WriteAllText(filename, "\n");
                        }
                        
                        
                        file = OpenFile(filename, out originalfile);
                    }

                    if (keyInfo1.Key == ConsoleKey.C)
                    {
                        Console.Write("enter command: ");
                        Console.ForegroundColor = ConsoleColor.Black;
                        string command = Console.ReadLine();
                        if(command.Length > 3)
                        {
                            string cmdinpt = command.Remove(0, 3);
                            switch(command.Substring(0, 2))
                            {
                                case "lp":
                                    if(projects.Any(list => list.Count > 0 && list[0] == cmdinpt))
                                    {
                                        currentproject = cmdinpt;
                                        currentprojectindx = Array.FindIndex(projects, list => list.Count > 0 && list[0] == currentproject);
                                     }
                                     break;
                                case "pf":
                                    if(projects[currentprojectindx].Contains(cmdinpt))
                                    {
                                        if(projects[currentprojectindx].IndexOf(cmdinpt) > 0)
                                        {
                                            filename = projects[currentprojectindx][projects[currentprojectindx].IndexOf(cmdinpt) + 1];
                                            //
                                            if(File.Exists(filename))
                                            {
                                                originalfile = File.ReadAllText(filename).Replace("\t", "    ");
                                                filestr = "";
                                                file = new List<char>();
                                                foreach (char c in originalfile)
                                                {
                                                    file.Add(c);
                                                }
                                            }
                                        }
                                    }
                                    break;
                                case "ls":
                                    if(File.Exists(homeDirectory + "/.qse/" + cmdinpt))
                                    {
                                        settingsfile = cmdinpt;
                                        settings = File.ReadAllText(homeDirectory + "/.qse/" + settingsfile).Split('\n');
                                        
                                        ignclr = (settings[0] + "Æ\n").Split('Æ').SelectMany(s => s.ToCharArray()).ToArray();
                                        
                                        black = settings[1].Split('Æ');
                                        red = settings[2].Split('Æ');
                                        green = settings[3].Split('Æ');
                                        yellow = settings[4].Split('Æ');
                                        blue = settings[5].Split('Æ');
                                        magenta = settings[6].Split('Æ');
                                        cyan = settings[7].Split('Æ');
                                        white = settings[8].Split('Æ');
                                        bblack = settings[9].Split('Æ');
                                        bred = settings[10].Split('Æ');
                                        bgreen = settings[11].Split('Æ');
                                        byellow = settings[12].Split('Æ');
                                        bblue = settings[13].Split('Æ');
                                        bmagenta = settings[14].Split('Æ');
                                        bcyan = settings[15].Split('Æ');
                                        bwhite = settings[16].Split('Æ');
                                        
                                        normal = "\x1b[90m";
                                        number = "\x1b[95m";
                                        strng = char.Parse(settings[17]);
                                        strgclr = "\x1b[91m";
                                        cmntclr = "\x1b[32m";
                                        
                                        term = settings[19];
                                        tflags = settings[20];
                                        tcommand = settings[21];
                                        
                                        if (settings[18] == "1")
                                        {
                                            tcommand = filename + tcommand;
                                        }
                                    }
                                    break;
                                    
                            }
                        }                        
                    }
                    if (keyInfo1.Key == ConsoleKey.M)
                    {
                        hscroll = hscroll + 2;
                    }

                    if (keyInfo1.Key == ConsoleKey.N)
                    {
                        if (hscroll > 2)
                        {
                            hscroll = hscroll - 2;
                        }
                        else
                        {
                            hscroll = 0;
                        }
                    }
                    if (keyInfo1.Key == ConsoleKey.R)
                    {
                        filestr = string.Concat(file);
                        File.WriteAllText(filename, filestr);
                        do{}while(File.ReadAllText(filename) != filestr);
                        ProcessStartInfo psi = new ProcessStartInfo
                        {
                            FileName = term,
                            Arguments = tflags + " " + tcommand,
                            UseShellExecute = false
                        };
                        Console.ResetColor();
                        Console.Clear();
                        Console.Write("\x1b[92m" + Environment.UserName + "@" + Environment.MachineName + " \x1b[34m" + System.IO.Directory.GetCurrentDirectory() + " $ \x1b[37m" + term + tflags +  " " +  tcommand + "\n\x1b[90m");
                        using (Process proc = Process.Start(psi))
                        {
                             proc.WaitForExit();
                        }
                        
                    }
                    if (keyInfo1.Key == ConsoleKey.Q)
                    {
                        Console.Write("u sure?");
                        ConsoleKeyInfo r = Console.ReadKey();
                        if (r.Key == ConsoleKey.Enter)
                        {
                            Console.CursorLeft = 0;
                            Console.Write("you can press ctrl+c to exit now");
                            Console.CancelKeyPress += (sender, e) => { e.Cancel = false;};
                            Console.ReadKey();
                        }
                        
                    }


                }
                else if ((((keyInfo1.Modifiers & ConsoleModifiers.Shift) == 0)) && (keyInfo1.Modifiers & ConsoleModifiers.Control) != 0)
                {
                    int position = (filelenghts[(line + scroll) - 1] + column - 1 + (line + scroll) + hscroll);
                    
                    if (keyInfo1.Key == ConsoleKey.RightArrow)
                    {
                        bool slashn = false;
                        int aposition = position;
                        if (file[aposition] == '\n') {line++; column = 0; hscroll = 0;slashn=true;}

                        if (!slashn)
                        {
                            do
                            {
                                aposition++;
                                while(file[aposition] == ' ' && file[aposition + 1] == ' ') aposition++;
                                
                            } while (!ignclr.Contains(file[aposition]));
                            if(file[aposition] == ' ') aposition++;
                            
                            column = column + (aposition - position);
                            while (column >= left - (left/10)) { column--; hscroll++; }
                            
                        }

                        while (line >= top - (top/10)) { line--; scroll++; }
                    }
                    else if (keyInfo1.Key == ConsoleKey.LeftArrow)
                    {
                        bool slashn = false;
                        int aposition = position;
                        do
                        {
                            aposition--;
                            if(aposition > 0) while (file[aposition] == ' ' && file[aposition - 1] == ' ') aposition--;
                            if(aposition < 0){ aposition = 0; break;}
                        } while (!ignclr.Contains(file[aposition]));
                         
                        column = column + (aposition - position);
                        while (column >= left - (left / 5))
                        {
                            column--;
                            hscroll++;
                        }
                        if (column < 0)
                        {
                            hscroll = 0;
                            line--;
                            column = filelenghts[line + scroll] - filelenghts[line - 1 + scroll];
                        }
                            
                        if (line < 1) {scroll--; line++; }
                    }
                    else if (keyInfo1.Key == ConsoleKey.DownArrow)
                    {
                        if(scroll + top < filelenghts.Count - 1)
                            scroll++;
                    }
                    else if (keyInfo1.Key == ConsoleKey.UpArrow)
                    {
                        if(scroll > 0)
                            scroll--;
                    }
                    else if (keyInfo1.Key == ConsoleKey.V)
                    {
                        string pclip = ClipboardService.GetText();
                        for (int i = 0; i < pclip.Length; i++)
                        {
                            file.Insert(filelenghts[(line + scroll) - 1] + column - 1 + (line + scroll) + hscroll, pclip[i]);
                            column++;
                        }
                        
                        filestr = string.Concat(file);
                        filelenghts = lenghts(filestr);
                        
                        while (column > filelenghts[line + scroll] - filelenghts[line + scroll - 1])
                        {
                            line++;
                            column = filelenghts[line + scroll] - filelenghts[line + scroll - 1];
                        }
                    }
                    else if (keyInfo1.Key == ConsoleKey.Delete)
                    {
                        if(!ignclr.Contains(file[filelenghts[(line + scroll) - 1] + column - 1 + (line + scroll) + hscroll]))
                        {
                            do
                            {
                                file.RemoveAt(filelenghts[(line + scroll) - 1] + column - 1 + (line + scroll) + hscroll);
                            } while (!ignclr.Contains(file[filelenghts[(line + scroll) - 1] + column - 1 + (line + scroll) + hscroll]));
                        }  
                        else
                        { 
                            do
                            {
                                file.RemoveAt(filelenghts[(line + scroll) - 1] + column - 1 + (line + scroll) + hscroll);
                            } while (file[filelenghts[(line + scroll) - 1] + column - 1 + (line + scroll) + hscroll] == ' ');
                        } 
                    }
                    else if (keyInfo1.Key == ConsoleKey.Backspace)
                    {
                        if(filelenghts[(line + scroll) - 1] + column - 2 + (line + scroll) + hscroll > 0)
                        {
                            if(!ignclr.Contains(file[filelenghts[(line + scroll) - 1] + column - 2 + (line + scroll) + hscroll]))
                            {
                                do
                                {
                                    file.RemoveAt(filelenghts[(line + scroll) - 1] + column - 2 + (line + scroll) + hscroll);
                                    column--;
                                    
                                    
                                    if (column < 0 && hscroll == 0 && line + scroll >= 2)
                                    {
                                        line--;
                                        column = filelenghts[line + scroll] - filelenghts[line - 1 + scroll];
                                        if (line+scroll <= 1)
                                        {
                                            line = 1;
                                            scroll = 0;
                                        }
                                        
                                        if (column <= 0)
                                            column = 0;
                                    
                                    }
                                if(filelenghts[(line + scroll) - 1] + column - 2 + (line + scroll) + hscroll < 0)
                                {
                                    break;
                                }
                                
                                
                                
                            } while (!(ignclr.Contains(file[filelenghts[(line + scroll) - 1] + column - 2 + (line + scroll) + hscroll]) || (column == 0 && line + scroll == 1)));
                            }
                        
                            else
                            { 
                                do
                                {
                                
                                    file.RemoveAt(filelenghts[(line + scroll) - 1] + column - 2 + (line + scroll) + hscroll);
                                    column--;
                                    if (column < 0 && hscroll == 0 && line + scroll >= 2)
                                    {
                                        line--;
                                        column = filelenghts[line + scroll] - filelenghts[line - 1 + scroll];
                                    }
                                
                                
                            } while (file[filelenghts[(line + scroll) - 1] + column - 2 + (line + scroll) + hscroll] == ' ');
                            }
                        }
                    }
                }
                else
                {
                    if (keyInfo1.Key == ConsoleKey.RightArrow)
                    {
                        hscroll = 0;
                        column = filelenghts[line + scroll] -  filelenghts[line - 1 + scroll];
                        while (column >= left - (left/10)) { column--; hscroll++; }
                    }
                    else if (keyInfo1.Key == ConsoleKey.LeftArrow)
                    {
                        column = 0;
                        hscroll = 0;
                    }
                    else if (keyInfo1.Key == ConsoleKey.UpArrow)
                    {
                        scroll = scroll - 4;
                        if(scroll < 0) scroll = 0;
                    }
                    else if (keyInfo1.Key == ConsoleKey.DownArrow)
                    {
                        scroll = scroll + 4;
                        if(scroll > filelenghts.Count - top - 1 && filelenghts.Count > top) scroll = filelenghts.Count - top - 1;
                    }
                }
                HandleRC(line, column, scroll, hscroll, file, false, out line, out column, out scroll, out hscroll, out file, out filelenghts);
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

        public static string colour(string str, string[] black, string[] red, string[] green, string[] yellow, string[] blue, string[] magenta, string[] cyan, string[] white, string[] bblack, string[] bred, string[] bgreen, string[] byellow, string[] bblue, string[] bmagenta, string[] bcyan, string[] bwhite, string normal, string number)
        {
            if (Array.Exists(black, x => x == str))
            {
                return "\x1b[30m";
            }
            else if (Array.Exists(red, x => x == str))
            {
                return "\x1b[31m";
            }
            else if (Array.Exists(green, x => x == str))
            {
                return "\x1b[32m";
            }
            else if (Array.Exists(yellow, x => x == str))
            {
                return "\x1b[33m";
            }
            else if (Array.Exists(blue, x => x == str))
            {
                return "\x1b[34m";
            }
            else if (Array.Exists(magenta, x => x == str))
            {
                return "\x1b[35m";
            }
            else if (Array.Exists(cyan, x => x == str))
            {
                return "\x1b[36m";
            }
            else if (Array.Exists(white, x => x == str))
            {
                return "\x1b[37m";
            }
            else if (Array.Exists(bblack, x => x == str))
            {
                return "\x1b[90m";
            }
            else if (Array.Exists(bred, x => x == str))
            {
                return "\x1b[91m";
            }
            else if (Array.Exists(bgreen, x => x == str))
            {
                return "\x1b[92m";
            }
            else if (Array.Exists(byellow, x => x == str))
            {
                return "\x1b[93m";
            }
            else if (Array.Exists(bblue, x => x == str))
            {
                return "\x1b[94m";
            }
            else if (Array.Exists(bmagenta, x => x == str))
            {
                return "\x1b[95m";
            }
            else if (Array.Exists(bcyan, x => x == str))
            {
                return "\x1b[96m";
            }
            else if (Array.Exists(bwhite, x => x == str))
            {
                return "\x1b[97m";
            }
            
            else if (int.TryParse(str, out var num))
            {
                return number;
            }

            else
            {
                return normal;
            }



        }

        public static void write(int scroll, int hscroll, int top, int left, List<int> filelenghts, List<char> file, string filename, string filestr,int line, int column, string currentproject, char strng, string strgclr, string cmntclr, char[] ignclr, string[] black, string[] red, string[] green, string[] yellow, string[] blue, string[] magenta, string[] cyan, string[] white, string[] bblack, string[] bred, string[] bgreen, string[] byellow, string[] bblue, string[] bmagenta, string[] bcyan, string[] bwhite, string normal, string number, bool marked, int mark)
        {
            
            
            
            
            int strngs = 0;
            bool mlcomment = false;
            
            Console.CursorVisible = false;
            Console.Title = "Qseft's simple editor - editing " + filename;
            
            
            Console.ResetColor();
            
            StringWriter stringWriter = new StringWriter();
            
            Console.SetOut(stringWriter);
            
            int neededoutputlines = filelenghts.Count - 1;
            if (neededoutputlines > top) neededoutputlines = top;
            
            for (int i = scroll; i < neededoutputlines+scroll - 1; i++)
            {
                string writeline = "";
                
                for (int j = filelenghts[i]+i ; j <= filelenghts[i + 1]+i; j++)
                {
                    if(j < file.Count)
                        writeline = writeline + file[j];
                }
                
                string expression = "";
                string chcklne = "";
                int indx = 0;
                string outp = "";
                bool comment = false;
                while (chcklne.Length < writeline.Length)
                {
                    expression = "";
                    
                    if (indx < writeline.Length)
                    {
                        while (!ignclr.Contains(writeline[indx]))
                        {
                            expression = expression + writeline[indx];
                            chcklne = chcklne + writeline[indx];
                            indx++;
                        }
                        
                        if (comment|| mlcomment) { outp = outp + cmntclr; }
                        else if(strngs % 2 == 0) {outp = outp + colour(expression, black, red, green, yellow, blue, magenta, cyan, white, bblack, bred, bgreen, byellow, bblue, bmagenta, bcyan, bwhite, normal, number);}
                        else outp = outp + strgclr;
                        outp = outp + expression;
                        
                    }
                    if (indx < writeline.Length)
                    {
                        if(writeline[indx] == '/' && writeline[indx + 1] == '/' && strngs % 2 == 0) comment = true;
                        if(writeline[indx] == '/' && writeline[indx + 1] == '*' && strngs % 2 == 0) mlcomment = true;
                        if(indx>0) if(writeline[indx] == '/' && writeline[indx - 1] == '*' && strngs % 2 == 0) mlcomment = false;
                        
                        
                        if (comment || mlcomment) { outp = outp + cmntclr; }
                        else if(strngs % 2 == 0 && writeline[indx] != strng) {outp = outp + "\x1b[00m";}
                        else outp = outp + strgclr;
                        
                        outp = outp + writeline[indx];
                        chcklne = chcklne + writeline[indx];
                        if (indx > 0){if((writeline[indx-1] == '\\' || writeline[indx-1] == '\'') && writeline[indx] == strng) strngs--;}
                        if (writeline[indx] == strng)
                        {
                            strngs++;
                        }
                        indx++;
                    }
                }
                
                int windx = hscroll;
                while (windx > 0)
                {
                    if (outp.Length > 0)
                    {
                        if (outp[0] != '\x1b')
                        {
                            outp = outp.Remove(0, 1);
                        }
                        else
                        {
                            while (outp[5] == '\x1b')
                            {
                                outp = outp.Remove(0, 5);
                            }
                            if (outp.Length > 6)
                            {
                                outp = outp.Remove(5, 1);
                            }
                        }
                    }
                    else {outp = outp + "\n";}
                    windx--;
                }
                
                int indx1 = (filelenghts[i + 1] - filelenghts[i]) - hscroll;
                
                if (indx1 > left - filelenghts.Count.ToString().Length)
                {
                    while (indx1 > left - 1 - filelenghts.Count.ToString().Length)
                    {
                        if (outp[outp.Length - 5] == '\x1b')
                            indx1 = indx1 + 5;
                        outp = outp.Substring(0, outp.Length - 1);
                        indx1--;
                    }
                    outp = outp + "\x1b[00m>\n";
                }
                 
                Console.Write(outp);
            }
            
            
            string output = stringWriter.ToString();
            
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
            Console.Clear();
            
            Console.SetCursorPosition (0, 1);
            Console.WriteLine(output);
            
            Console.SetCursorPosition (0, 1);
            int max = top;
            if (max > filelenghts.Count - 1)
                max = filelenghts.Count - 1;
            
            for(int i = 1; i < max; i++)
            {
                Console.SetCursorPosition(left-((filelenghts.Count).ToString().Length) + 1 , i);
                for(int j = 0; j < ((filelenghts.Count).ToString().Length) - (i+scroll).ToString().Length; j++)
                    Console.Write("\x1b[38;5;245m" + "\x1b[48;5;234m" + "0");
                
                Console.Write("\x1b[38;5;245m" + "\x1b[48;5;234m" +(i+scroll));
            }
            
            Console.SetCursorPosition(0, 0);
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write(" QSE");
            for (int i = 0; i < left-(4+filename.Length); i++)
                Console.Write(" ");
            Console.Write(filename+" ");
            Console.SetCursorPosition(0, top);
            Console.Write(" " + (filelenghts.Count-2).ToString() + " lines loaded");
            for (int i = 0; i < left-(14+(filelenghts.Count-2).ToString().Count()+currentproject.ToString().Count()); i++)
                Console.Write(" ");
            Console.Write(currentproject + " ");
            Console.ResetColor();
            Console.CursorVisible = true;
        }
        
        public static void HandleRC(int line, int column, int scroll, int hscroll, List<char> file, bool r, out int line1, out int column1, out int scroll1, out int hscroll1, out List<char> file1, out List<int> filelenghts1)
        {
            int left = Console.WindowWidth - 1;
            int top = Console.WindowHeight - 2;
            int num = 0;
            List<int> filelenghts = new List<int>();
            filelenghts.Add(0);
            string filestr = string.Concat(file);
            filelenghts = lenghts(filestr);


            if(line >= filelenghts.Count)
                line = filelenghts.Count - 1;



            while (line >= top) { line--; scroll++; }

            if (line <= 0)
            {
                line = 1;
                scroll--;
            }

            if (column < 0 && hscroll == 0 && line + scroll >= 2)
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

            if (line >= 1 && (line + scroll < filelenghts.Count - 1))
            {
                if ((column + hscroll == filelenghts[line + scroll] - filelenghts[line - 1 + scroll] + 1/*  || column  >= left*/) && r)
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
            
            while(line + scroll >= filelenghts.Count - 1)
                {
                    if(line > 1){
                        line--;
                    }
                    else if (scroll > 0){
                        scroll--;
                    }
                    else{
                        file.Insert(file.Count(), '\n');
                        break;
                    }
                }

            while (column >=  (left-((filelenghts.Count).ToString().Length)))
            {
            column--; hscroll++;
            }
            
            if (scroll + top >= filelenghts.Count && filelenghts.Count - top > 0)
            {
                if (line < top)
                    line++;
                scroll = filelenghts.Count - top - 1;
            }
            
            if (line >= top)
            {
                line = top - 1;
            }
            
            if (filelenghts[(line + scroll) - 1] + column - 1 + (line + scroll) + hscroll >
                filelenghts[filelenghts.Count - 1] + (filelenghts.Count - 3))
            {
                column = filelenghts[line + scroll] - filelenghts[line - 1 + scroll];
            }
            
            if (column >= left)
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
            
            while (column < 0)
            {
                column++;
                if (hscroll > 0)
                    hscroll--;
            }
            
            
            line1 = line;
            column1 = column;
            scroll1 = scroll;
            hscroll1 = hscroll;
            file1 = file;
            filelenghts1 = filelenghts;
        }
        public static List<char> OpenFile(string filename, out string originalfile)
        {
            List<char> file = new List<char>();
            originalfile = File.ReadAllText(filename).Replace("\t", "    ");
            foreach (char c in originalfile)
            {
                file.Add(c);
            }
            return file;

        }
        public static bool MakeSureConfDirExists(string settingsfile, string homeDirectory)
        {
            bool r = true;
            if (!Directory.Exists(homeDirectory + "/.qse"))
            {
                Directory.CreateDirectory(homeDirectory + "/.qse");
                r=false;
            }
            if (!File.Exists(homeDirectory + "/.qse/projects.list"))
            {
                File.WriteAllText(homeDirectory + "/.qse/projects.list", "lol");
                r=false;
            }
            if (!File.Exists(homeDirectory + "/.qse/" + settingsfile))
            {
                File.WriteAllText(homeDirectory + "/.qse/" + settingsfile, " \n \n \n \n \n \n \n \n \n \n \n \n \n \n \n \n \n \n \n \n");
                r=false;
            }
            return r;
        }
        
        
        
        
        
        
        

    }
}
