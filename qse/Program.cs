
/*
    QSE - qseft's simple editor - the C# console-based text editor
    Copyright (C) 2025 Václav Ulrich

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
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
            Stopwatch swone  = new Stopwatch();
            Stopwatch swtwo  = new Stopwatch();
            Stopwatch swthree  = new Stopwatch();
            
            sw.Start();
            
            int left = Console.WindowWidth-1;
            int top = Console.WindowHeight-2;
            int num = 0;
            string homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            
            string settingsfile = "settings";
            MakeSureConfDirExists(settingsfile, homeDirectory);
            
            string filename = homeDirectory + Path.DirectorySeparatorChar + ".qse" + Path.DirectorySeparatorChar + "open" + Path.DirectorySeparatorChar + "file";
            
            if (args.Length > 0)
            {
                filename = "";
                if(args[0][0] == '.')
                {
                    filename=Directory.GetCurrentDirectory() + args[0].Substring(1);
                }
                else if (args[0][0] == Path.DirectorySeparatorChar)
                {
                    filename = args[0];
                }
                else
                {
                    filename = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + args[0];
                }
            }
            
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
            
            string[] settings = File.ReadAllText(homeDirectory + "" + Path.DirectorySeparatorChar + ".qse" + Path.DirectorySeparatorChar + "settings" + Path.DirectorySeparatorChar + settingsfile).Split('\n');
            
            char[] ignclr = (settings[0] + "Æ\n").Split('Æ').SelectMany(s => s.ToCharArray()).ToArray();
            
            string[][] efs = [ settings[1].Split('Æ'), settings[2].Split('Æ'), settings[3].Split('Æ'), settings[4].Split('Æ'),  settings[5].Split('Æ'), settings[6].Split('Æ'), settings[7].Split('Æ'), settings[8].Split('Æ'), settings[9].Split('Æ'), settings[10].Split('Æ'), settings[11].Split('Æ'), settings[12].Split('Æ'), settings[13].Split('Æ'), settings[14].Split('Æ'), settings[15].Split('Æ'), settings[16].Split('Æ') ];
            
            char strng = char.Parse(settings[17]);
            
            string term = settings[19];
            string tflags = settings[20];
            string tcommand = settings[21];
            bool code = false;
            
            if (settings[18][0] != '0')
            {
                tcommand = filename + tcommand;
            }
            if (settings[18][1] == '1')
            {
                code = true;
            }
            
            bool dosug = false;
            string sugfile = "";
            string[] match = [""];
            if(filename.Split(Path.DirectorySeparatorChar)[filename.Split(Path.DirectorySeparatorChar).Length-1].Contains('.'))
            {
                if(File.Exists(homeDirectory + Path.DirectorySeparatorChar+".qse"+Path.DirectorySeparatorChar+"suggestions"+Path.DirectorySeparatorChar+Path.GetExtension(filename)))
                {
                    sugfile = Path.GetExtension(filename);
/* end of checks*/                    match = File.ReadAllText(homeDirectory + Path.DirectorySeparatorChar+".qse"+Path.DirectorySeparatorChar+"suggestions"+Path.DirectorySeparatorChar+Path.GetExtension(filename)).Split('\n');
                    dosug = true;
                }
            }
            
            string themefile = "theme";
            string[] theme = File.ReadAllText(homeDirectory + "" + Path.DirectorySeparatorChar + ".qse" + Path.DirectorySeparatorChar + "themes" + Path.DirectorySeparatorChar + themefile).Split('\n');
            string[] colours = new string[29];
            for(int i = 0; i < 29; i++)
            {
                colours[i] = "\x1b[" + theme[i] + "m";
            }
            
            string projectsstr = File.ReadAllText(homeDirectory + "" + Path.DirectorySeparatorChar + ".qse" + Path.DirectorySeparatorChar + "projects" + Path.DirectorySeparatorChar + "projects.list");
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
            
            Stopwatch ressw = new Stopwatch();
            
            string prevstr = "";
            string nowstr = "";
            string[] suggest = [""];
            int sugsc = 0;
            string prevnowstr = "";
            
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
                    
                    
                    
                    bool r = false;
                    
                    int iindx = 0;
                    prevstr = "";
                    nowstr = "";
                    do
                    {
                        iindx++;
                        if(filelenghts[(line + scroll) - 1] + column - 1 + (line + scroll) + hscroll-iindx < 0)
                            break;
                        if(ignclr.Contains(file[filelenghts[(line + scroll) - 1] + column - 1 + (line + scroll) + hscroll-iindx]))
                            break;
                        nowstr = file[filelenghts[(line + scroll) - 1] + column - 1 + (line + scroll) + hscroll-iindx] + nowstr;
                    }while(true);
                    
                    do
                    {
                        iindx++;
                        if(filelenghts[(line + scroll) - 1] + column - 1 + (line + scroll) + hscroll-iindx < 0)
                            break;
                        if(ignclr.Contains(file[filelenghts[(line + scroll) - 1] + column - 1 + (line + scroll) + hscroll-iindx]))
                            break;
                        prevstr = file[filelenghts[(line + scroll) - 1] + column - 1 + (line + scroll) + hscroll-iindx] + prevstr;
                    }while(true);
                    
                    
                    
                    if(dosug)
                    {
                        suggest = sug(prevstr, nowstr, match);
                        if(prevnowstr != nowstr)
                        {
                            sugsc = 0;
                        }
                    }
                    
                    
                    write(scroll, hscroll, top, left, filelenghts, file, filename, filestr, line, column, currentproject, strng, ignclr, efs,  marked, mark, colours);
                    
                    
                    
                    if(dosug)
                    {
                        if(sugsc > suggest.Length-6)
                            sugsc = suggest.Length-6;
                        if(sugsc < 0)
                            sugsc = 0;
                        Console.SetCursorPosition(column, line);
                        ArrayBlackBox(suggest, colours[27]+colours[28], colours[21], nowstr.Length, sugsc);
                        prevnowstr = nowstr;
                    }
                    
                    Console.SetCursorPosition(column, line);
                    
                    //
                    //debug(swone.ElapsedMilliseconds + " " + swtwo.ElapsedMilliseconds + " " + swthree.ElapsedMilliseconds);                    
                    
                    Console.CancelKeyPress += (sender, e) => 
                    {
                     e.Cancel = true;
                     if(marked)
                        {
                            string pclip = "";
                            for(int i = mark; i <= filelenghts[(line + scroll) - 1] + column - 1 + (line + scroll) + hscroll; i++)
                            {
                                pclip = pclip + file[i];
                            }
                            ClipboardService.SetText(pclip);
                            marked = false;
                        }
                    };
                    
                    
                    ressw.Start();
                    
                    while(!Console.KeyAvailable)
                    {
                        if(left != Console.WindowWidth - 1)
                        {
                            left = Console.WindowWidth - 1;
                        }
                        if(top != Console.WindowHeight - 2)
                        {
                            top = Console.WindowHeight - 2;
                        }
                    }
                    
                    
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
                            if (filelenghts[(line + scroll) - 1] + column - 2 + (line + scroll) + hscroll > 0) if(!ignclr.Contains(file[filelenghts[(line + scroll) - 1] + column - 2 + (line + scroll) + hscroll]) && code)
                                tab=tab+4;
                            
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
                            if(filelenghts[(line + scroll) - 1] + column - 1 + (line + scroll) + hscroll < file.Count && !(scroll + line + 3 > filelenghts.Count())){
                            file.RemoveAt(filelenghts[(line + scroll) - 1] + column - 1 + (line + scroll) + hscroll);}
                            break;
                        case ConsoleKey.PageUp:
                            scroll = scroll - top + (top / 5);
                            break;
                        case ConsoleKey.PageDown:
                            scroll = scroll + top - (top / 5);
                            break;
                        case ConsoleKey.Home:
                            if(sugsc > 0) sugsc--;
                            break;
                        case ConsoleKey.End:
                            if(sugsc < suggest.Length - 6) sugsc++;
                            break;
                        default:
                            if (!char.IsControl(keyInfo1.KeyChar) && keyInfo1.KeyChar != '\0')
                            {
                                file.Insert(filelenghts[(line + scroll) - 1] + column - 1 + (line + scroll) + hscroll,keyInfo1.KeyChar);
                                column++;
                            }
                            break;
                    }
                    
                    
                    
                    HandleRC(line, column, scroll, hscroll, file, r, out line, out column, out scroll, out hscroll, out file, out filelenghts);
                    
                    
                }
                
                
                if ((((keyInfo1.Modifiers & ConsoleModifiers.Alt) != 0)) && (keyInfo1.Modifiers & ConsoleModifiers.Shift) == 0)
                {
                    Console.SetCursorPosition(0, top - 1);
                    
                    Console.Write(colours[25]+colours[26]);//change to 27 and 28
                    
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
                        Console.Write(colours[25]+colours[26]);//change to 27 and 28
                        string inp = Console.ReadLine();
                        int lne = scroll+line;
                        
                        if(int.TryParse(inp, out lne))                        
                            scroll = lne - line;
                    }
                    
                    if (keyInfo1.Key == ConsoleKey.O)
                    {
                        string dfromf = "";
                        do
                        {
                            dfromf = "";
                            Console.SetCursorPosition(0, top - 1);
                            Console.Write(colours[25]+colours[26]);//change to 27 and 28
                            Console.Write("enter filepath: ");
                            filename = AutoPrompt.PromptForInput("", filename);
                            Console.SetCursorPosition(0, top - 1);
                            for (int i = 0; i < left; i++)
                            {
                                Console.Write(" ");
                            }
                            
                            
                            for (int i = 0; i < filename.Split(Path.DirectorySeparatorChar).Length - 1; i++)
                            {
                                dfromf = dfromf + filename.Split(Path.DirectorySeparatorChar)[i] + "/";
                            }
                            
                            
                        } while (!Directory.Exists(dfromf));
                        if(!File.Exists(filename))
                        {
                            File.WriteAllText(filename, "\n");
                        }
                        
                        
                        file = OpenFile(filename, out originalfile);
                        
                        dosug = false;
                        sugfile = "";
                        match = [""];
                        if(filename.Split(Path.DirectorySeparatorChar)[filename.Split(Path.DirectorySeparatorChar).Length-1].Contains('.'))
                        {
                            if(File.Exists(homeDirectory + "/.qse/suggestions/"+Path.GetExtension(filename)))
                            {
                                sugfile = Path.GetExtension(filename);
                                match = File.ReadAllText("/home/qseft/.qse/suggestions/" + Path.GetExtension(filename)).Split('\n');
                                dosug = true;
                            }
                        }
                        
                    }
                    
                    if (keyInfo1.Key == ConsoleKey.C)
                    {
                        Console.Write("enter command: ");
                        Console.Write(colours[25]+colours[26]);//change to 27 and 28
                        string command = Console.ReadLine();
                        if(command.Length > 1)
                        {
                            string cmdinpt = "";
                            if(command.Length > 3)
                                cmdinpt = command.Remove(0, 3);
                            
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
                                    if(File.Exists(homeDirectory + "/.qse/settings/" + cmdinpt))
                                    {
                                        settingsfile = cmdinpt;
                                        settings = File.ReadAllText(homeDirectory + "/.qse/settings/" + settingsfile).Split('\n');
                                        
                                        ignclr = (settings[0] + "Æ\n").Split('Æ').SelectMany(s => s.ToCharArray()).ToArray();
                                        
                                        efs = [ settings[1].Split('Æ'), settings[2].Split('Æ'), settings[3].Split('Æ'), settings[4].Split('Æ'),  settings[5].Split('Æ'), settings[6].Split('Æ'), settings[7].Split('Æ'), settings[8].Split('Æ'), settings[9].Split('Æ'), settings[10].Split('Æ'), settings[11].Split('Æ'), settings[12].Split('Æ'), settings[13].Split('Æ'), settings[14].Split('Æ'), settings[15].Split('Æ'), settings[16].Split('Æ') ];
                                        
                                        strng = char.Parse(settings[17]);
                                        
                                        term = settings[19];
                                        tflags = settings[20];
                                        tcommand = settings[21];
                                        
                                        if (settings[18][0] != '0')
                                        {
                                            tcommand = filename + tcommand;
                                        }
                                        code = false;
                                        if (settings[18][1] == '1')
                                        {
                                            code = true;
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
                        
                        term = settings[19];
                        tflags = settings[20];
                        tcommand = settings[21];
                        
                        if (settings[18][0] != '0')
                        {
                            tcommand = filename + tcommand;
                        }
                        
                        ProcessStartInfo psi = new ProcessStartInfo
                        {
                            FileName = term,
                            Arguments = tflags + " " + tcommand + " && sleep 10",
                            UseShellExecute = false
                        };
                        Console.ResetColor();
                        Console.Clear();
                        Console.Write("\x1b[92m" + Environment.UserName + "@" + Environment.MachineName + " \x1b[34m" + System.IO.Directory.GetCurrentDirectory() + " $ \x1b[37m" + term + tflags +  " " +  tcommand + "\n\x1b[90m");
                        using (Process proc = Process.Start(psi))
                        {
                             proc.WaitForExit();
                        }
                        HandleRC(line, column, scroll, hscroll, file, false, out line, out column, out scroll, out hscroll, out file, out filelenghts);
                    }
                    if (keyInfo1.Key == ConsoleKey.Q)
                    {
                        Console.Write("u sure?");
                        if ("" == Console.ReadLine())
                        {
                            Console.CursorLeft = 0;
                            Console.Write("you can press ctrl+c to exit now");
                            Console.CancelKeyPress += (sender, e) => { e.Cancel = false;};
                            Console.ReadKey();
                        }
                        
                    }
                    if (keyInfo1.Key == ConsoleKey.T)
                    {
                        do{
                            Console.SetCursorPosition(0, top - 1);
                            Console.Write(colours[25]+colours[26]);//change to 27 and 28
                            Console.Write("enter theme name: ");
                            themefile = AutoPrompt.PromptForInput("", themefile);
                            Console.SetCursorPosition(0, top - 1);
                            for (int i = 0; i < left; i++)
                            {
                                Console.Write(" ");
                            }
                        } while (!File.Exists(homeDirectory + Path.DirectorySeparatorChar + ".qse" + Path.DirectorySeparatorChar + "themes" + Path.DirectorySeparatorChar + themefile));
                        themefile = "theme";
                        theme = File.ReadAllText(homeDirectory + Path.DirectorySeparatorChar + ".qse" + Path.DirectorySeparatorChar + "themes" + Path.DirectorySeparatorChar + themefile).Split('\n');
                        colours = new string[27];
                        for(int i = 0; i < 27; i++)
                        {
                            colours[i] = "\x1b[" + theme[i] + "m";
                        }
                    }
                    if(keyInfo1.Key == ConsoleKey.P)
                    {
                        Console.ResetColor();
                        Console.Write(colours[21]);
                        Console.Write("\x1b[2J");
                        int pf = -1;
                        int lp = -1;
                        string proj = ProjectManagementTUI(out lp, out pf, colours[21]);
                        File.WriteAllText(homeDirectory + "/.qse/projects/projects.list", proj);
                        projectsstr = File.ReadAllText(homeDirectory + "" + Path.DirectorySeparatorChar + ".qse" + Path.DirectorySeparatorChar + "projects" + Path.DirectorySeparatorChar + "projects.list");
                        project = projectsstr.Split('\n');
                        projects = Enumerable.Range(0, project.Length).Select(_ => new List<string>()).ToArray();
                        
                        if (project.Length > 0)
                        {
                            for(int i = 0; i < projects.Length; i++)
                            {
                                for(int j = 0; j < project[i].Split(' ').Length; j++)
                                {
                                    projects[i].Add(project[i].Split(' ')[j]);
                                }
                            }
                        }
                        
                        if(pf>=0 && lp >=0)
                        {
                            currentproject = projects[lp][0];
                            currentprojectindx = lp;
                            
                            filename = projects[lp][2+pf*2];
                            
                            string dfromf = "";
                            
                            for (int i = 0; i < filename.Split(Path.DirectorySeparatorChar).Length - 1; i++)
                            {
                                dfromf = dfromf + filename.Split(Path.DirectorySeparatorChar)[i] + "/";
                            }
                            
                            if(Directory.Exists(dfromf))
                            {
                                if(!File.Exists(filename))
                                    File.WriteAllText(filename, "\n");
                                
                                originalfile = File.ReadAllText(filename).Replace("\t", "    ");
                                filestr = "";
                                file = new List<char>();
                                foreach (char c in originalfile)
                                {
                                    file.Add(c);
                                }
                                
                                dosug = false;
                                sugfile = "";
                                match = [""];
                                if(filename.Split(Path.DirectorySeparatorChar)[filename.Split(Path.DirectorySeparatorChar).Length-1].Contains('.'))
                                {
                                    if(File.Exists(homeDirectory + "/.qse/suggestions/"+Path.GetExtension(filename)))
                                    {
                                        sugfile = Path.GetExtension(filename);
                                        match = File.ReadAllText("/home/qseft/.qse/suggestions/" + Path.GetExtension(filename)).Split('\n');
                                        dosug = true;
                                    }
                                }
                            }
                            else
                            {
                                Console.Clear();
                                Console.WriteLine("ERROR: wrong directory, try deleting from the list and adding it again");
                                Console.ReadKey();
                            }
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
                        if (column < 0 && line + scroll > 1)
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
                    else if (keyInfo1.Key == ConsoleKey.X)
                    {
                        if(marked)
                        {
                            string pclip = "";
                            for(int i = mark; i <= filelenghts[(line + scroll) - 1] + column - 1 + (line + scroll) + hscroll; i++)
                            {
                                pclip = pclip + file[i];
                            }
                            ClipboardService.SetText(pclip);
                            for(int i = mark; i <= filelenghts[(line + scroll) - 1] + column - 1 + (line + scroll) + hscroll; i++)
                            {
                                file.RemoveAt(mark);
                            }
                            marked = false;
                        }
                    }
                    else if (keyInfo1.Key == ConsoleKey.Delete)
                    {
                        if(!(scroll + line + 3 > filelenghts.Count()))
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
                                    if(filelenghts[(line + scroll) - 1] + column - 1 + (line + scroll) + hscroll >= file.Count())
                                        break;
                                } while (file[filelenghts[ (line + scroll) - 1] + column - 1 + (line + scroll) + hscroll] == ' ');
                            }
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
                        if(scroll + 4 > 0) scroll = scroll - 4;
                    }
                    else if (keyInfo1.Key == ConsoleKey.DownArrow)
                    {
                        if (scroll - 4 < filelenghts.Count - top - 1) scroll = scroll + 4;
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
                num = num + lne.Count(c => !char.IsControl(c));
                filelenghts.Add(num);
            }
            
            return filelenghts;
        }
        
        public static string colour(string str, string[][] efs, string[] colours, char[] ignclr, int h, int i, List<char> file, char strng, string filestring, List<int>[] overrides)
        {
            
            if(overrides[h][i] == 1 ) { return colours[19];}
            if(overrides[h][i] == 2 ) { return colours[18];}
            
            return colourString(str, efs, colours, ignclr);
        }
        
        public static string colourString(string str, string[][] efs, string[] colours, char[] ignclr)
        {
            if(str == "" )
            {
                return colours[15];
            }
            if(char.TryParse(str, out _)) if(ignclr.Contains(char.Parse(str)))
            {
                return colours[15];
            }
            
            for(int i = 0; i <= 15; i++)
            {
                if (Array.Exists(efs[i], x => x == str))
                {
                    return colours[i];
                }
            }
            
            if (int.TryParse(str, out _))
            {
                return colours[17];
            }
            
            else
            {
                return colours[16];
            }
        } 
        
        
        public static string listColourAndCutoff(List<char> file, int maxWidth, string dcolour, char[] ignclr, string[][] efs, string[] colours, int top, int height, int hscroll, char strng)
        {
            // maxWidth, dcolour, []ignclr, [][]efs, []colours, top, height, hscroll
            
            List<string> filestr = string.Concat(file).Split('\n').Skip(top).Take(height).ToList();
            List<int>[] overrides = ColourOverrides(file, strng);
            List<char> output = new List<char>();
            
            for(int h = 0; h < filestr.Count(); h++)
            {
                string lne = filestr[h];
                
                bool isfirst = false;
                int i = hscroll;
                
                if(lne.Length > i) while(i > 0 && !ignclr.Contains(lne[i])) {  i--;  isfirst = true;  }
                if(isfirst) while( ignclr.Contains(lne[i]) && i < lne.Length-1) {i++;}
                int j = i;
                
                if(i < lne.Length) while(ignclr.Contains(lne[i]) && i < lne.Length)
                {
                    foreach(char c in ( colour(lne[i] + "", efs, colours, ignclr, h+top, i, file, strng, String.Join("\n", filestr), overrides) )) output.Add(c);
                    output.Add(lne[i]);
                    if(i+1 < lne.Length) i++;
                    else break;
                }
                
                while(i < lne.Length && i < hscroll+maxWidth)
                {
                    List<char> expression = new List<char>();
                    while(!ignclr.Contains(lne[i]) && i < lne.Length){    expression.Add(lne[i]);    if(i < lne.Length-1 && !ignclr.Contains(lne[i+1])) i++; else break;    }
                    
                    string expr = "";
                    string addi = "";
                    
                    
                    expr = string.Concat(expression);
                    
                    if(isfirst && (hscroll-j) < expression.Count())
                        addi = expr.Substring(hscroll - j);
                    else if ((hscroll-j) < expression.Count())
                        addi = expr;
                    
                    
                    foreach( char c in (colour(expr, efs, colours, ignclr, h+top, i, file, strng, String.Join("\n", filestr), overrides) + addi)) output.Add(c);
                    
                    
                    isfirst=false;
                    
                    i++;
                    
                    if(i < lne.Length) while(ignclr.Contains(lne[i]) && i < lne.Length && i < hscroll+maxWidth)    //yes, this is the exact same code
                    {
                        foreach(char c in ( colour(lne[i] + "", efs, colours, ignclr, h+top, i, file, strng, String.Join("\n", filestr), overrides) )) output.Add(c);
                        output.Add(lne[i]);
                        
                        if(i+1 < lne.Length) i++;
                        else break;
                    }
                    
                    //i++;
                    
                }
                
                while(i > hscroll+maxWidth && output.Count() > 0 ) {i--; output.RemoveAt(output.Count()-1); }
                
                foreach(char c in colours[15]) output.Add(c);
                output.Add('\n');
                
                
            }
            
            return String.Join("", output);
        }
        
        public static void writeCutoffs(int scroll, int hscroll, int maxLeft, List<int> filelenghts, int height, string colour)
        {
            Console.SetCursorPosition(0, 1);
            
            for(int i = scroll; i < scroll+height; i++)
            {
                if(filelenghts.Count() > i+1) if(filelenghts[i+1] - filelenghts[i] > maxLeft+hscroll)
                {
                    Console.SetCursorPosition(maxLeft-1, 1+i-scroll);
                    Console.Write(colour+">");
                }
            }
        }
        
        public static string write(int scroll, int hscroll, int top, int left, List<int> filelenghts, List<char> file, string filename, string filestr,int line,int column,  string currentproject, char strng, char[] ignclr, string[][] efs, bool marked, int mark, string[] colours)
        {    
            string write = listColourAndCutoff(file, Console.WindowWidth-((filelenghts.Count).ToString().Length), colours[16], ignclr, efs, colours, scroll,  Console.WindowHeight - 3, hscroll, strng);
            
            
            Console.Write(colours[21]);
            Console.Write("\x1b[2J");
            
            Console.SetCursorPosition (0, 1);
            
            Console.Write(write);
            
            writeCutoffs(scroll, hscroll, Console.WindowWidth-((filelenghts.Count).ToString().Length), filelenghts, Console.WindowHeight - 3, colours[15]);
            
            //linunumbers
            Console.SetCursorPosition (0, 1);
            int max = top;
            if (max > filelenghts.Count - 1)
                max = filelenghts.Count - 1;
            
            for(int i = 1; i < max; i++)
            {
                Console.SetCursorPosition(left-((filelenghts.Count).ToString().Length) + 1 , i);
                for(int j = 0; j < ((filelenghts.Count).ToString().Length) - (i+scroll).ToString().Length; j++)
                    Console.Write(colours[22] + colours[23] + "0");

                Console.Write(colours[22] +colours[24] +(i+scroll));
            }
            
            //UI
            Console.SetCursorPosition(0, 0);
            Console.Write(colours[25]+colours[26]);
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
            
            return write;
        }
        
        public static List<int>[] ColourOverrides(List<char> file, char strng)
        {
            List<int>[] filespec = new List<int>[file.Where(s=>s!=null && s == '\n').Count()+1];
            for(int i = 0; i < filespec.Length; i++) filespec[i] = new List<int>();
            List<string> filet = String.Concat(file).Split('\n').ToList();
            
            bool mlcomment = false;
            bool comment = false;
            bool bstrng = false;
            
            for (int h = 0; h < filet.Count(); h++)
            {
                string str = filet[h];
                for(int i = 0; i < str.Length; i++)
                {
                    if(i+1 < str.Length) if(str[i] == '/' && str[i + 1] == '*' && !bstrng) mlcomment = true;
                    if (i > 0) if(str[i] == '/' && str[i - 1] == '*' && !bstrng) mlcomment = false;
                    if(i+1 < str.Length)if(str[i] == '/' && str[i + 1] == '/' && !bstrng) comment = true;
                    if(str[i] == strng && !(comment || mlcomment)) bstrng = !bstrng;
                    if (i > 0){if((str[i-1] == '\\' || str[i-1] == '\'') && str[i] == strng) bstrng = !bstrng;}
                    
                    if (bstrng)
                        filespec[h].Add(2);
                    else if (mlcomment == true || comment == true)
                        filespec[h].Add(1);
                    else
                        filespec[h].Add(0);
                }
                
                comment = false;
            }
            
            return filespec;
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
        public static string ProjectManagementTUI(out int lp, out int pf, string bg)
        {
            string texto = "\x1b[47;30m QSE ";
            string textt = "\x1b[47;30m Project editor ";
            for(int i = 5; i < Console.WindowWidth; i++) texto = texto + " ";
            for(int i = 16; i < Console.WindowWidth; i++) textt = textt + " ";
            texto = texto + "\x1b[0m";
            textt = textt + "\x1b[0m";
            
            lp = -1;
            pf = -1;
            string homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            bool one = true;
            string proj = "";
            while(one)
            {
            proj = File.ReadAllText(homeDirectory + "/.qse/projects/projects.list");
            Console.Clear();
            Console.WriteLine(proj);
            string[] project = proj.Split('\n');
            
            List<string>[] projects = Enumerable.Range(0, project.Length).Select(_ => new List<string>()).ToArray();
            
            for(int i = 0; i < projects.Length; i++)
            {
                for(int j = 0; j < project[i].Split(' ').Length; j++)
                {
                    projects[i].Add(project[i].Split(' ')[j]);
                }
            }
            
            string[] projectsnames = new string[projects.Length];
            
            
            for(int i = 0; i < projects.Length; i++)
            {
                projectsnames[i] = projects[i][0];
            }
            
            projectsnames[projectsnames.Length-1] = "Add a new project";
            
            int editingproject = writeMenu(projectsnames, 0, texto, textt, true, bg);
            if (editingproject == -1)
                return proj;
            
            string[] pfnames  = new string[(projects[editingproject].Count()-1) / 2 + 1];
            
            if(editingproject != projects.Length-1)
            {
                for(int i = 1; i + 1  < projects[editingproject].Count(); i=i+2)
                {
                    pfnames[Array.FindIndex(pfnames, j => j == null || j.Length == 0)] = projects[editingproject][i];
                }
                pfnames[pfnames.Length-1] = "Add a new file";
                int whattodo = writeMenu(pfnames, projectsnames.OrderByDescending(s => s.Length).FirstOrDefault().Length+5,texto, textt, false, bg);
                if(whattodo == -1)
                    continue;
                
                
                
                if(whattodo != pfnames.Length-1)
                {
                    int oped = writeMenu(["Open", "Delete"], (projectsnames.OrderByDescending(s => s.Length).FirstOrDefault().Length+5) + (pfnames.OrderByDescending(s => s.Length).FirstOrDefault().Length+5), texto, textt, false, bg);
                    Console.SetCursorPosition(0, Console.WindowHeight/2);
                    if(oped == 0)
                    {
                        pf = whattodo;
                        lp = editingproject;
                        return proj;
                    }
                    else if (oped == 1)
                    {
                        Console.Write("Are you sure you want to delete " + projects[editingproject][whattodo*2+1] + " "+ projects[editingproject][whattodo*2+2]+" from this list? [y, N] ");
                        string r = Console.ReadLine();
                        if (r == "y" || r == "Y")
                        {
                            projects[editingproject].RemoveAt(whattodo*2+2);
                            projects[editingproject].RemoveAt(whattodo*2+1);
                            
                            proj = "";
                            for(int i = 0; i < projects.Length; i++)
                            {   for(int j = 0; j < projects[i].Count; j++)
                                {
                                    if(j != projects[i].Count-1)
                                        proj = proj + projects[i][j] + " ";
                                    else
                                        proj = proj + projects[i][j];
                                }
                                if(i!=projects.Length-1)
                                    proj = proj + "\n";
                            }
                            
                            Console.Write("Project file deleted!\n");
                            Stopwatch sw = new Stopwatch();
                            sw.Start();
                            do{}while(sw.ElapsedMilliseconds <= 500);
                        }
                    }
                }
                else
                {
                    Console.CursorLeft = projectsnames.OrderByDescending(s => s.Length).FirstOrDefault().Length+10;
                    string rl = "";
                    do{
                        Console.Write("name          ");
                        Console.CursorLeft -= 14;
                        rl = Console.ReadLine();
                        if(rl == "" || rl == "exit")
                            break;
                        if(rl.Contains(" "))
                            continue;
                        projects[editingproject].Add(rl);
                        
                        Console.CursorLeft = projectsnames.OrderByDescending(s => s.Length).FirstOrDefault().Length+10;
                        Console.CursorTop -= 1;
                        Console.Write("path"); for(int i = 4; i < rl.Length; i++) Console.Write(" ");
                        Console.CursorLeft = projectsnames.OrderByDescending(s => s.Length).FirstOrDefault().Length+10;
                        rl = Console.ReadLine();
                        if(rl.Contains(" "))
                            continue;
                        projects[editingproject].Add(rl);
                        Console.Write("\n*"+projects[editingproject][projects[editingproject].Count()-1]+"*\n");
                    }
                    while(false);
                    
                    proj = "";
                    for(int i = 0; i < projects.Length; i++)
                    {   for(int j = 0; j < projects[i].Count; j++)
                        {
                            if(j != projects[i].Count-1)
                                proj = proj + projects[i][j] + " ";
                            else
                                proj = proj + projects[i][j];
                        }
                        if(i!=projects.Length-1)
                            proj = proj + "\n";
                    }
                }
            }
            
            else
            {
                Console.Write("What is the project name: ");
                string name = Console.ReadLine();
                string rl = "";
                string other = "";
                do{
                    Console.Write("What is the file name: [exit]");
                    rl = Console.ReadLine();
                    if(rl == "" || rl == "exit")
                        break;
                    if(rl.Contains(" "))
                        continue;
                    other = other + " " + rl;
                    Console.Write("What is the file path ");
                    rl = Console.ReadLine();
                    if(rl.Contains(" "))
                        continue;
                    other = other + " " + rl;
                    
                }while(true);
                proj = proj + name + other + "\n";
                
            }
            File.WriteAllText(homeDirectory + "/.qse/projects/projects.list", proj);
            }
            
            return proj;
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
        
        public static void ArrayBlackBox(string[] arr,string bgcol, string defcol, int offset, int start)
        {
            int cl = Console.CursorLeft-offset;
            if(cl >= 0)
            {
                int ct = Console.CursorTop;
                arr = arr.Skip(start).ToArray();
                for (int i = 0; i < arr.Length; i++)
                    if(arr[i].Length > Console.WindowWidth-cl) arr[i] = arr[i].Substring(0, Console.WindowWidth-cl);
                if(arr.Length > 6) arr = [arr[0], arr[1], arr[2], arr[3], arr[4], arr[5]];
                int width = arr.Aggregate(string.Empty, (seed, f) => (f == null ? 0 : f.Length) > seed.Length ? f : seed).Length;
                int height = arr.Length;
                
                
                
                
                for (int i = 0; i < height; i++)
                {
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
        }
        
        public static void debug(string info)
        {
            int ct = Console.CursorTop;
            int cl = Console.CursorLeft;
            
            Console.SetCursorPosition(Console.WindowWidth-info.Length, Console.WindowHeight);
            
            Console.Write(info);
            
            Console.SetCursorPosition(cl, ct);
        }
        
        public static string[] sug(string prevstr, string str, string[] match)
        {
            
            List<string> sugg = new List<string>();
            List<string> sugtwo = new List<string>();
            List<string> classes = new List<string>();
            List<string> sugfour = new List<string>();
            List<string> sugthree = new List<string>();
            List<string> classsug = new List<string>();
            string sep = match[0];
            match = match.Skip(1).ToArray();
            
            for(int i = 0; i < match.Length; i++)
            {
                if(match[i].Contains(sep))
                {
                    string[] singleclass = match[i].Split(sep);
                    classes.Add(singleclass[0]);
                    for (int j = 1; j < singleclass.Length; j++)
                    {
                        classsug.Add(singleclass[j] + "::" + singleclass[0]);
                    }
                }
            }
            
            if(classes.Contains(prevstr) || str.Length > 0)
            {
            
                for(int i = 0; i < match.Length; i++)
                {
                    if(match[i].Contains(sep))
                    {
                        string[] arr = match[i].Split(sep);
                        if (prevstr == arr[0])
                        {
                            for(int j = 1; j < arr.Length; j++)
                            {
                                if(StartsWith(arr[j], str))      sugg.Add(arr[j]);
                                else if(arr[j].Contains(str))    sugtwo.Add(" "+arr[j]);
                            }
                            break;
                        }
                    }
                    else if (!classes.Contains(prevstr))
                    {
                        if(StartsWith(match[i], str))      sugg.Add(match[i]);
                        else if(match[i].Contains(str))    sugtwo.Add(" "+match[i]);
                    }
                }
                for(int i = 0; i < classsug.Count; i++)
                {
                    if(StartsWith(classsug[i], str))      sugthree.Add(classsug[i]);
                    else if(classsug[i].Contains(str))    sugfour.Add(" "+classsug[i]);
                }
                return sugg.ToArray().Concat(sugtwo.ToArray()).ToArray().Concat(sugthree.ToArray()).ToArray().Concat(sugfour.ToArray()).ToArray();
            
            }
            return [];
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
            
            if (!Directory.Exists(homeDirectory + "/.qse/projects"))
            {
                Directory.CreateDirectory(homeDirectory + "/.qse/projects");
                r=false;
            }
            if (!File.Exists(homeDirectory + "/.qse/projects/projects.list"))
            {
                File.WriteAllText(homeDirectory + "/.qse/projects/projects.list", "\n");
                r=false;
            }
            
            
            if (!Directory.Exists(homeDirectory + "/.qse/themes"))
            {
                Directory.CreateDirectory(homeDirectory + "/.qse/themes");
                r=false;
            }
            if (!File.Exists(homeDirectory + "/.qse/themes/theme"))
            {
                File.WriteAllText(homeDirectory + "/.qse/themes/theme", "38;2;080;080;080\n38;2;150;025;075\n38;2;025;150;100\n38;2;175;175;025\n38;2;075;050;175\n38;2;125;050;125\n38;2;050;125;125\n38;2;125;125;150\n38;2;100;100;100\n38;2;200;075;125\n38;2;075;200;150\n38;2;225;225;075\n38;2;125;100;225\n38;2;175;100;175\n38;2;075;175;175\n38;2;175;175;200\n38;2;115;115;150\n38;2;200;100;200\n38;2;175;050;075\n38;2;050;175;100\n48;2;175;175;025\n48;2;000;000;020\n48;2;000;000;050\n38;2;025;025;075\n38;2;075;075;150\n");
                r=false;
            }
            
            
            if (!Directory.Exists(homeDirectory + "/.qse/settings"))
            {
                Directory.CreateDirectory(homeDirectory + "/.qse/settings");
                r=false;
            }
            if (!File.Exists(homeDirectory + "/.qse/settings/" + settingsfile))
            {
                File.WriteAllText(homeDirectory + "/.qse/settings" + settingsfile, " \n \n \n \n \n \n \n \n \n \n \n \n \n \n \n \n \n \n \n \n \n");
                r=false;
            }
            
            if (!Directory.Exists(homeDirectory + "/.qse/open"))
            {
                Directory.CreateDirectory(homeDirectory + "/.qse/open");
                r=false;
            }
            if (!File.Exists(homeDirectory + "/.qse/open/file"))
            {
                File.WriteAllText(homeDirectory + "/.qse/open/file","QSE - qseft's simple editor - the C# console-based text editor\nCopyright (C) 2025 Václav Ulrich\n\n    This program is free software: you can redistribute it and/or modify\n    it under the terms of the GNU General Public License as published by\n    the Free Software Foundation, either version 3 of the License, or\n    (at your option) any later version.\n\n    This program is distributed in the hope that it will be useful,\n    but WITHOUT ANY WARRANTY; without even the implied warranty of\n    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the\n    GNU General Public License for more details.\n\n    You should have received a copy of the GNU General Public License\n    along with this program.  If not, see <https://www.gnu.org/licenses/>.\n\nWelcome to QSE, here's a list of basic shortcuts:\n\nCTRL shortcuts\n    CTRL+L/R arrow -> jump to next thing\n    CTRL+U/D arrow -> scrolling\n    CTRL+V -> paste\n    CTRT+C -> copy\n    CTRL+X -> cut\n    CTRL+BACKSPACE/DELETE -> remove next thing\n    CTRL+SHIFT+U/D arrow -> fast scrolling\n    CTRL+SHIFT+R arrow -> jump to end of line\n    CTRL+SHIFT+L arrow -> jump to start of line\n\n\nALT shortcuts\n    ALT+M -> scroll right\n    ALT+N -> scroll left\n    ALT+S -> save file\n    ALT+G -> go to line\n    ALT+O -> open file\n    ALT+A -> set mark (for selecting text to copy/cut)\n    ALT+C -> Command mode\n    ALT+R -> Run\n    ALT+Q -> Quit (must confirm with ENTER)\n\nOther shortcuts\n    Page Up/Down -> scrolls a page (surprisingly)\n\nBe sure to check out the wiki\nqseftweb.wz.cz/qse/wiki\n\n");
                r=false;
            }
            if (!Directory.Exists(homeDirectory + "/.qse/suggestions/"))
            {
                Directory.CreateDirectory(homeDirectory + "/.qse/suggestions/");
                r=false;
            }
            return r;
        }
    }
}
