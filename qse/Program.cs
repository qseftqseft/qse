
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
            
            int mode = 0;
            
            string homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            
            string settingsfile = "settings";
            Files.MakeSureConfDirExists(settingsfile, homeDirectory);
            
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
            
            List<char> file = Files.OpenFile(filename, out string originalfile);
            string filestr = "";
            bool marked = false;
            int mark = 0;
            int[] marka = [0, 0];
            
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
            
            string[] tpes = settings[22].Split('Æ');
            
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
            string[] colours = new string[30];
            for(int i = 0; i < 30; i++)
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
            char prevtf = '\0';
            int prevtfm = -1;
            
            Console.Write(colours[21]);
            Console.Write("\x1b[2J");
            
            
            while (true)
            {
                ConsoleKeyInfo keyInfo1 = new ConsoleKeyInfo('\u001b', ConsoleKey.Escape, shift: false, alt: false,
                    control: false);
                left = Console.WindowWidth - 1;
                top = Console.WindowHeight - 2;
                filestr = string.Concat(file);
                filelenghts.Add(0);
                filelenghts = Utils.lenghts(filestr);
                char prevch = '\0';
                string autocomp = "";
                string prev = "";
                
                while (run)
                {
                    
                    
                    autocomp = "";
                    
                    
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
                    
                    
                    filestr = string.Concat(file);
                    List<string> exps = new List<string>();
                    string[] lnes = filestr.Split('\n');
                    List<string> vars = new List<string>();
                    
                    foreach(string str in lnes)
                    {
                        string[] st = str.Split(ignclr, StringSplitOptions.RemoveEmptyEntries);
                        for(int i = 1; i < st.Length; i++)
                        {
                            if( tpes.Contains(st[i-1]) )
                            {
                                exps.Add(st[i]);
                                if(dosug) vars.Add(st[i-1] + match[0] + st[i]);
                            }
                        }
                    }
                    
                    if(dosug)
                    {
                        suggest = Suggest.sug(prevstr, nowstr, match, vars.ToArray() );
                        if(prevnowstr != nowstr)
                        {
                            sugsc = 0;
                        }
                    }
                    
                    
                    
                    
                    
                    prev = Write.write(scroll, hscroll, top, left, filelenghts, file, filename, filestr, line, column, currentproject, strng, ignclr, efs,  marked, marka, colours, mode, prevch, exps.ToArray(), [line+scroll, column+hscroll], prev);
                    
                    
                    
                    if(dosug)
                    {
                        if(sugsc > suggest.Length-1)
                            sugsc = suggest.Length-1;
                        if(sugsc < 0)
                            sugsc = 0;
                        Console.SetCursorPosition(column, line);
                        autocomp = Utils.ArrayBlackBox(suggest, colours[27] + colours[28], colours[21], nowstr.Length, sugsc, ignclr);
                        prevnowstr = nowstr;
                    }
                    
                    Console.SetCursorPosition(((scroll + Console.WindowHeight - 3).ToString().Length)+1+column, line);
                    
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
                            
                            Console.SetCursorPosition(0, top + 1);
                            Console.Write(colours[21]+colours[15]);//change to 27 and 28
                            for (int i = 0; i <= left; i++)
                            {
                                Console.Write(" ");
                            }
                            Console.SetCursorPosition(0, top + 1);
                            Console.Write("Copied text: " + pclip);
                        }
                    };
                    
                    
                    
                    
                    
                    
                    bool iterate = false;
                    ressw.Start();
                    
                    while(!Console.KeyAvailable)
                    {
                        if(left != Console.WindowWidth - 1)
                        {
                            left = Console.WindowWidth - 1;
                            iterate = true;
                            break;
                        }
                        if(top != Console.WindowHeight - 2)
                        {
                            top = Console.WindowHeight - 2;
                            iterate = true;
                            break;
                        }
                    }
                    if(iterate)
                        continue;
                    
                    
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
                    
                    int curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);
                    
                    
                    if(mode == 0) Input.modeone(keyInfo1, line, column, r, autocomp, file, scroll, hscroll,  curchar, filelenghts, tab, ignclr, code, top, sugsc, suggest, mode, 
                    out file, out line, out column, out scroll, out hscroll, out sugsc, out r, out mode);
                    
                    if(mode == 1) NavMode.modetwo(keyInfo1, line, column, r, autocomp, file, scroll, hscroll,  curchar, filelenghts, tab, ignclr, code, top, sugsc, suggest, mode, prevch, prevtf, prevtfm,
                    out file, out line, out column, out scroll, out hscroll, out sugsc, out r, out mode, out prevch, out prevtf, out prevtfm);
                    
                    
                    
                    Input.HandleRC(line, column, scroll, hscroll, file, r, out line, out column, out scroll, out hscroll, out file, out filelenghts);
                    
                    
                }
                
                
                if ((((keyInfo1.Modifiers & ConsoleModifiers.Alt) != 0)) && (keyInfo1.Modifiers & ConsoleModifiers.Shift) == 0)
                {
                    Console.SetCursorPosition(0, top + 1);
                    
                    Console.Write(colours[21]+colours[15]);//change to 27 and 28
                    
                    for (int i = 0; i <= left; i++)
                    {
                        Console.Write(" ");
                    }
                    
                    Console.SetCursorPosition(0, top + 1);
                    
                    if (keyInfo1.Key == ConsoleKey.S || keyInfo1.Key == ConsoleKey.R)
                    {
                        if(keyInfo1.Key == ConsoleKey.S)
                        filename = Utils.prompt("save to: ", filename);
                        Console.SetCursorPosition(0, top + 1);
                        Console.Write("SAVING, DO NOT EXIT!!!");
                        File.WriteAllText(filename, filestr);
                        Console.CursorLeft = 0;
                        for(int i = 0; i <= left; i++)
                        {
                            Console.Write(" ");
                        }
                    }
                    
                    if (keyInfo1.Key == ConsoleKey.A)
                    {
                        marked = !marked;
                        mark = filelenghts[(line + scroll) - 1] + column - 1 + (line + scroll) + hscroll;
                        marka = [line+scroll, column+hscroll];
                    }
                    
                    if (keyInfo1.Key == ConsoleKey.F)
                    {
                        mode = 1;
                    }
                    else if (keyInfo1.Key == ConsoleKey.D)
                    {
                        mode = 0;
                    }
                    
                    if (keyInfo1.Key == ConsoleKey.G)
                    {
                        Console.Write("got line: ");
                        Console.Write(colours[21]+colours[15]);//change to 27 and 28
                        string inp = Console.ReadLine() ?? string.Empty;
                        int lne = scroll+line;
                        int col = column + hscroll;
                        
                        string[] got = inp.Split(' ');
                        
                        if(got.Length > 0) if(int.TryParse(got[0], out lne))
                            scroll = lne - line;
                        if(got.Length > 1) if(int.TryParse(got[1], out col))
                            hscroll = col - column;
                    }
                    
                    if (keyInfo1.Key == ConsoleKey.O)
                    {
                        string dfromf = "";
                        do
                        {
                            dfromf = "";
                            
                            Console.Write(colours[21]+colours[15]);//change to 27 and 28
                            Console.SetCursorPosition(0, top+1);
                            Console.Write("enter filepath: ");
                            
                            filename = Utils.prompt("", filename);
                                                        
                            for (int i = 0; i < filename.Split(Path.DirectorySeparatorChar).Length - 1; i++)
                            {
                                dfromf = dfromf + filename.Split(Path.DirectorySeparatorChar)[i] + "" + Path.DirectorySeparatorChar + "";
                            }
                            
                            
                        } while (!Directory.Exists(dfromf));
                        if(!File.Exists(filename))
                        {
                            File.WriteAllText(filename, "\n");
                        }
                        
                        
                        file = Files.OpenFile(filename, out originalfile);
                        
                        dosug = false;
                        sugfile = "";
                        match = [""];
                        if(filename.Split(Path.DirectorySeparatorChar)[filename.Split(Path.DirectorySeparatorChar).Length-1].Contains('.'))
                        {
                            if(File.Exists(homeDirectory + "" + Path.DirectorySeparatorChar + ".qse" + Path.DirectorySeparatorChar + "suggestions" + Path.DirectorySeparatorChar + ""+Path.GetExtension(filename)))
                            {
                                sugfile = Path.GetExtension(filename);
                                match = File.ReadAllText(homeDirectory + Path.DirectorySeparatorChar + "" + Path.DirectorySeparatorChar + ".qse" + Path.DirectorySeparatorChar + "suggestions" + Path.DirectorySeparatorChar + "" + Path.GetExtension(filename)).Split('\n');
                                dosug = true;
                            }
                        }
                        
                    }
                    
                    if (keyInfo1.Key == ConsoleKey.C)
                    {
                        Console.Write("enter command: ");
                        Console.Write(colours[21]+colours[15]);//change to 27 and 28
                        string command = Console.ReadLine() ?? string.Empty;
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
                                    if(File.Exists(homeDirectory + "" + Path.DirectorySeparatorChar + ".qse" + Path.DirectorySeparatorChar + "settings" + Path.DirectorySeparatorChar + "" + cmdinpt))
                                    {
                                        settingsfile = cmdinpt;

                                        settings = File.ReadAllText(homeDirectory + "" + Path.DirectorySeparatorChar + ".qse" + Path.DirectorySeparatorChar + "settings" + Path.DirectorySeparatorChar + "" + settingsfile).Split('\n');
                                        
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
                                        tpes = settings[22].Split('Æ');
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
                        using (Process proc = Process.Start(psi) ?? new Process())
                        {
                             proc.WaitForExit();
                        }
                        Input.HandleRC(line, column, scroll, hscroll, file, false, out line, out column, out scroll, out hscroll, out file, out filelenghts);
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
                            Console.SetCursorPosition(0, top + 1);
                            Console.Write(colours[21]+colours[15]);//change to 27 and 28
                            Console.Write("enter theme name: ");
                            themefile = Utils.prompt("", "");
                            Console.SetCursorPosition(0, top + 1);
                            for (int i = 0; i < left; i++)
                            {
                                Console.Write(" ");
                            }
                        } while (!File.Exists(homeDirectory + Path.DirectorySeparatorChar + ".qse" + Path.DirectorySeparatorChar + "themes" + Path.DirectorySeparatorChar + themefile));
                        theme = File.ReadAllText(homeDirectory + Path.DirectorySeparatorChar + ".qse" + Path.DirectorySeparatorChar + "themes" + Path.DirectorySeparatorChar + themefile).Split('\n');
                        colours = new string[30];
                        for(int i = 0; i < 30; i++)
                        {
                            colours[i] = "\x1b[" + theme[i] + "m";
                        }
                        Console.Clear();
                    }
                    if(keyInfo1.Key == ConsoleKey.P)
                    {
                        Console.ResetColor();
                        Console.Write(colours[21]);
                        Console.Write("\x1b[2J");
                        int pf = -1;
                        int lp = -1;
                        string proj = PM.ProjectManagementTUI(out lp, out pf, colours[21]);
                        File.WriteAllText(homeDirectory + "" + Path.DirectorySeparatorChar + ".qse" + Path.DirectorySeparatorChar + "projects" + Path.DirectorySeparatorChar + "projects.list", proj);
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
                                dfromf = dfromf + filename.Split(Path.DirectorySeparatorChar)[i] + "" + Path.DirectorySeparatorChar + "";
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
                                    if(File.Exists(homeDirectory + "" + Path.DirectorySeparatorChar + ".qse" + Path.DirectorySeparatorChar + "suggestions" + Path.DirectorySeparatorChar + ""+Path.GetExtension(filename)))
                                    {
                                        sugfile = Path.GetExtension(filename);
                                        match = File.ReadAllText(homeDirectory + Path.DirectorySeparatorChar + "" + Path.DirectorySeparatorChar + ".qse" + Path.DirectorySeparatorChar + "suggestions" + Path.DirectorySeparatorChar + "" + Path.GetExtension(filename)).Split('\n');
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
                        string pclip = ClipboardService.GetText() ?? string.Empty;
                        for (int i = 0; i < pclip.Length; i++)
                        {
                            file.Insert(filelenghts[(line + scroll) - 1] + column - 1 + (line + scroll) + hscroll, pclip[i]);
                            column++;
                        }
                        
                        filestr = string.Concat(file);
                        filelenghts = Utils.lenghts(filestr);
                        
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
                Input.HandleRC(line, column, scroll, hscroll, file, false, out line, out column, out scroll, out hscroll, out file, out filelenghts);
                run = true;
                
            }
        }
        
    }
}
