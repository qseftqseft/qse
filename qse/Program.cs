/*
    
                  __________    _________    _________
                 /  ____   /   /  ______/   /  ______/
                /  /   /  /   /  /_____    /  /___
               /  /   /  /   /_____   /   /  ____/
              /  /___/  /   ______/  /   /  /_____
             /_____   _/   /________/   /________/
                  /__/
    
            qseft's       simple       editor
    
    
    
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
    
          ________   _________   _______      ___   _____     ___   ___________
         /  _____/  /  _ _   /  /  __   \    /  /  /    |    /  /  /  ________/
        /  /       /  /  /  /  /  /  |  |   /  /  /  /| |   /  /  /  /
       /  /       /  /  /  /  /  /   /  /  /  /  /  / | |  /  /  /  /  ______
      /  /       /  /  /  /  /  /   /  /  /  /  /  /  | | /  /  /  /  /__   /
     /  /____   /  /__/  /  /  /__ ╱  /  /  /  /  /   | |/  /  /  /_____/  /
    /_______/  /________/  /_________╱  /__/  /__/    |____/  /___________/
                          _______      __    _______    __________   ___   ___
                         /  __   \    /  /  /  __   \  /___   ___/  /  /  /  /
                        /  /  |  |   /  /  /  /  |  /     /  /     /  /  /  /
                       /  /   /  /  /  /  /  /__╱  /     /  /     /  /__/  /
                      /  /   /  /  /  /  /      __╱     /  /     /_____   /
                     /  /__ ╱  /  /  /  /  / \  \      /  /     ______/  /
                    /_________╱  /__/  /__/   \__\    /__/     /________/
    
                                                                      be prepared
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
            int left = Console.WindowWidth-1;
            int top = Console.WindowHeight-2;
            
            int mode = 0;
            
            string homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            
            string themefile = "theme";
            string[] theme = File.ReadAllText(homeDirectory + "" + Path.DirectorySeparatorChar + ".qse" + Path.DirectorySeparatorChar + "themes" + Path.DirectorySeparatorChar + themefile).Split('\n');
            string[] colours = new string[30];
            
            for(int i = 0; i < 30; i++)
            {
                colours[i] = "\x1b[" + theme[i] + "m";
            }
            
            
            Console.CursorVisible = false;
            
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
            
            
            Settings settings = new Settings(homeDirectory + "" + Path.DirectorySeparatorChar + ".qse" + Path.DirectorySeparatorChar + "settings" + Path.DirectorySeparatorChar + settingsfile);
            
            
            
            bool dosug = false;
            string sugfile = "";
            string[] match = [""];
            if(filename.Split(Path.DirectorySeparatorChar)[filename.Split(Path.DirectorySeparatorChar).Length-1].Contains('.'))
            {
                if(File.Exists(homeDirectory + Path.DirectorySeparatorChar+".qse"+Path.DirectorySeparatorChar+"suggestions"+Path.DirectorySeparatorChar+Path.GetExtension(filename)))
                {
                    Console.CursorVisible = false;
                    Screen.start(colours[21] + colours[16]);
                    sugfile = Path.GetExtension(filename);
/* end of checks*/  match = File.ReadAllText(homeDirectory + Path.DirectorySeparatorChar+".qse"+Path.DirectorySeparatorChar+"suggestions"+Path.DirectorySeparatorChar+Path.GetExtension(filename)).Split('\n').Concat(cs.open(filename).Split('\n')).ToArray();
                    dosug = true;
                    
                    List<string> wrds = new List<string>();
                    foreach(string s in match)
                    {
                        foreach(string st in s.Split('-'))
                        {
                            wrds.Add(st.Split('(')[0]);
                        }
                    }
                    wrds = wrds.Distinct().ToList();
                    
                    settings.colours["bright white"] = settings.colours["bright white"].Concat(wrds.ToArray()).ToArray();
                }
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
            
            char prevch = '\0';
            string autocomp = "";
            string prev = "";
            
            List<char> prevfile = new List<char>();
            int prevscroll = scroll;
            int prevhscroll = hscroll;
            int prevsugcnt = 0;
            bool sug = false;
            
            
            while (true)
            {
                ConsoleKeyInfo keyInfo1 = new ConsoleKeyInfo('\u001b', ConsoleKey.Escape, shift: false, alt: false,
                    control: false);
                left = Console.WindowWidth - 1;
                top = Console.WindowHeight - 2;
                filestr = string.Concat(file);
                filelenghts.Add(0);
                filelenghts = Utils.lenghts(filestr);
                prevch = '\0';
                autocomp = "";
                
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
                        if(settings.ignclr.Contains(file[filelenghts[(line + scroll) - 1] + column - 1 + (line + scroll) + hscroll-iindx]))
                            break;
                        nowstr = file[filelenghts[(line + scroll) - 1] + column - 1 + (line + scroll) + hscroll-iindx] + nowstr;
                    }while(true);
                    
                    do
                    {
                        iindx++;
                        if(filelenghts[(line + scroll) - 1] + column - 1 + (line + scroll) + hscroll-iindx < 0)
                            break;
                        if(settings.ignclr.Contains(file[filelenghts[(line + scroll) - 1] + column - 1 + (line + scroll) + hscroll-iindx]))
                            break;
                        prevstr = file[filelenghts[(line + scroll) - 1] + column - 1 + (line + scroll) + hscroll-iindx] + prevstr;
                    }while(true);
                    
                    
                    filestr = string.Concat(file);
                    List<string> exps = new List<string>();
                    string[] lnes = filestr.Split('\n');
                    List<string> vars = new List<string>();
                    
                    foreach(string str in lnes)
                    {
                        string[] st = str.Split(settings.ignclr, StringSplitOptions.RemoveEmptyEntries);
                        for(int i = 1; i < st.Length; i++)
                        {
                            if( settings.types.Contains(st[i-1]) )
                            {
                                exps.Add(st[i]);
                                if(dosug) vars.Add(st[i-1] + match[0] + st[i]);
                            }
                        }
                    }
                    
                    if(dosug && sug)
                    {
                        suggest = Suggest.sug(prevstr, nowstr, match, vars.ToArray(), sugsc);
                        if(prevnowstr != nowstr)
                        {
                            sugsc = 0;
                        }
                    }
                                        
                    
                    
                    if(!(prevfile.Count() == file.Count() && scroll == prevscroll && prevhscroll == hscroll && !marked && prevsugcnt <2 && prev != ""))
                    {
                        prev = Write.write(scroll, hscroll, top, left, filelenghts, file, filename, filestr, line, column, currentproject/**/ /**/,  marked, marka/**/ /**/, mode, prevch, exps.ToArray(), [line+scroll, column+hscroll], prev, settings, colours);
                    }
                    
                    
                    prevfile = String.Concat(file).ToList();
                    prevscroll = scroll;
                    prevhscroll = hscroll;
                    prevsugcnt = suggest.Length;
                    
                    string[] prevs = prev.Split('\n');
                    
                    int max = top;
                    if (max > filelenghts.Count - 1)
                        max = filelenghts.Count - 1;
            

                    
                    Console.SetCursorPosition(((scroll + Console.WindowHeight - 3).ToString().Length)+1+column, line);
                    
                    
                    
                    if(dosug && sug)
                    {
                        if(sugsc > suggest.Length-1)
                            sugsc = suggest.Length-1;
                        if(sugsc < 0)
                            sugsc = 0;
                        autocomp = Utils.ArrayBlackBox(suggest, colours[27] + colours[28], colours[21], nowstr.Length, settings.ignclr, 0);
                        prevnowstr = nowstr;
                        
                        for(int i = 1; i <= 7 && i + line < prevs.Length; i++)
                            prevs[i+line] = "";
                        Console.SetCursorPosition(((scroll + Console.WindowHeight - 3).ToString().Length)+1+column, line);
                    }
                    
                    
                    prev = String.Join('\n', prevs);
                    
                    
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
                            Console.Write("Copied!");
                            Console.SetCursorPosition(((scroll + Console.WindowHeight - 3).ToString().Length)+1+column, line);
                            prev="";
                        }
                        
                    };
                    
                    
                    bool iterate = false;
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
                    if(iterate){
                        iterate = false;
                        prev = "";
                        continue;}
                    
                    
                    sug = false;
                    
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
                    
                    
                    if(mode == 0) Input.modeone(keyInfo1, line, column, r, autocomp, file, scroll, hscroll,  curchar, filelenghts, tab, top, sugsc, suggest, mode, settings,
                    out file, out line, out column, out scroll, out hscroll, out sugsc, out r, out mode, out sug);
                    
                    if(mode == 1) NavMode.modetwo(keyInfo1, line, column, r, autocomp, file, scroll, hscroll,  curchar, filelenghts, tab, top, sugsc, suggest, mode, prevch, prevtf, prevtfm, settings,
                    out file, out line, out column, out scroll, out hscroll, out sugsc, out r, out mode, out prevch, out prevtf, out prevtfm);
                    
                    
                    Input.HandleRC(line, column, scroll, hscroll, file, r, out line, out column, out scroll, out hscroll, out file, out filelenghts);
                    
                    
                    
                    
                    
                }
                
                
                if ((((keyInfo1.Modifiers & ConsoleModifiers.Alt) != 0)) && (keyInfo1.Modifiers & ConsoleModifiers.Shift) == 0)
                {
                    Console.SetCursorPosition(0, top + 1);
                    
                    Console.Write(colours[21]+colours[15]);//change to 27 and 28
                    
                    prev="";
                    prevfile = new List<char>();
                    prevscroll = scroll;
                    prevhscroll = hscroll;
                    prevsugcnt = 0;
                    
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
                        //mode = 1;
                    }
                    else if (keyInfo1.Key == ConsoleKey.D)
                    {
                        //mode = 0;
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
                                Console.CursorVisible = false;
                                Screen.start(colours[21] + colours[16]);
                                sugfile = Path.GetExtension(filename);
                                match = File.ReadAllText(homeDirectory + Path.DirectorySeparatorChar + "" + Path.DirectorySeparatorChar + ".qse" + Path.DirectorySeparatorChar + "suggestions" + Path.DirectorySeparatorChar + "" + Path.GetExtension(filename)).Split('\n').Concat(cs.open(filename).Split('\n')).ToArray();
                                dosug = true;
                                
                                List<string> wrds = new List<string>();
                                foreach(string s in match)
                                {
                                    foreach(string st in s.Split('-'))
                                    {
                                        wrds.Add(st.Split('(')[0]);
                                    }
                                }
                                wrds = wrds.Distinct().ToList();
                                
                                settings.colours["bright white"] = settings.colours["bright white"].Concat(wrds.ToArray()).ToArray();
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
                                        
                                        settings = new Settings(homeDirectory + "" + Path.DirectorySeparatorChar + ".qse" + Path.DirectorySeparatorChar + "settings" + Path.DirectorySeparatorChar + settingsfile);
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
                            FileName = settings.runexec,
                            Arguments = settings.runflags + " " + settings.runcommand + " && sleep 10",
                            UseShellExecute = true
                        };
                        if (settings.curfile)
                        {
                            psi = new ProcessStartInfo
                            {
                                FileName = settings.runexec,
                                Arguments = settings.runflags + " " + filename + settings.runcommand + " && sleep 10",
                                UseShellExecute = true
                            };
                        }
                        
                        
                        
                        Console.ResetColor();
                        Console.Clear();
                        Console.Write("\x1b[92m" + Environment.UserName + "@" + Environment.MachineName + " \x1b[34m" + System.IO.Directory.GetCurrentDirectory() + " $ \x1b[37m" + settings.runexec + psi.Arguments + "\n\x1b[90m");
                        
                        using (Process proc = Process.Start(psi) ?? new Process())
                        {
                            proc.WaitForExit();
                        }
                        
                        Input.HandleRC(line, column, scroll, hscroll, file, false, out line, out column, out scroll, out hscroll, out file, out filelenghts);
                        Console.Clear();
                        prev="";
                        
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
                                
                                file = Files.OpenFile(filename, out originalfile);
                                
                                dosug = false;
                                sugfile = "";
                                match = [""];
                                if(filename.Split(Path.DirectorySeparatorChar)[filename.Split(Path.DirectorySeparatorChar).Length-1].Contains('.'))
                                {
                                    if(File.Exists(homeDirectory + "" + Path.DirectorySeparatorChar + ".qse" + Path.DirectorySeparatorChar + "suggestions" + Path.DirectorySeparatorChar + ""+Path.GetExtension(filename)))
                                    {
                                        Console.CursorVisible = false;
                                        Screen.start(colours[21] + colours[16]);
                                        sugfile = Path.GetExtension(filename);
                                        match = File.ReadAllText(homeDirectory + Path.DirectorySeparatorChar + "" + Path.DirectorySeparatorChar + ".qse" + Path.DirectorySeparatorChar + "suggestions" + Path.DirectorySeparatorChar + "" + Path.GetExtension(filename)).Split('\n').Concat(cs.open(filename).Split('\n')).ToArray();
                                        dosug = true;
                                        List<string> wrds = new List<string>();
                                        foreach(string s in match)
                                        {
                                            foreach(string st in s.Split('-'))
                                            {
                                                wrds.Add(st.Split('(')[0]);
                                            }
                                        }
                                        wrds = wrds.Distinct().ToList();
                                        
                                        settings.colours["bright white"] = settings.colours["bright white"].Concat(wrds.ToArray()).ToArray();
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
                        if (file[aposition] == '\n') {line++; column = 0; hscroll = 0; slashn=true;}
                        
                        if (!slashn)
                        {
                            do
                            {
                                aposition++;
                                while(file[aposition] == ' ' && file[aposition + 1] == ' ') aposition++;    
                            } while (!settings.ignclr.Contains(file[aposition]));
                            
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
                        } while (!settings.ignclr.Contains(file[aposition]));
                         
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
                        prev="";
                    }
                    else if (keyInfo1.Key == ConsoleKey.UpArrow)
                    {
                        if(scroll > 0)
                            scroll--;
                        prev="";
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
                    
                    else if (keyInfo1.Key == ConsoleKey.Y)
                    {
                        scroll -= 2;
                        prev="";
                    }
                    
                    else if (keyInfo1.Key == ConsoleKey.E)
                    {
                        scroll += 2;
                        prev="";
                    }
                    
                    else if (keyInfo1.Key == ConsoleKey.Delete)
                    {
                        if(!(scroll + line + 3 > filelenghts.Count()))
                        {
                            if(!settings.ignclr.Contains(file[filelenghts[(line + scroll) - 1] + column - 1 + (line + scroll) + hscroll]))
                            {
                                do
                                {
                                    file.RemoveAt(filelenghts[(line + scroll) - 1] + column - 1 + (line + scroll) + hscroll);
                                } while (!settings.ignclr.Contains(file[filelenghts[(line + scroll) - 1] + column - 1 + (line + scroll) + hscroll]));
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
                            if(!settings.ignclr.Contains(file[filelenghts[(line + scroll) - 1] + column - 2 + (line + scroll) + hscroll]))
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
                                
                                
                                
                            } while (!(settings.ignclr.Contains(file[filelenghts[(line + scroll) - 1] + column - 2 + (line + scroll) + hscroll]) || (column == 0 && line + scroll == 1)));
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
                        prev="";
                    }
                    else if (keyInfo1.Key == ConsoleKey.DownArrow)
                    {
                        if (scroll - 4 < filelenghts.Count - top - 1) scroll = scroll + 4;
                        prev="";
                    }
                }
                Input.HandleRC(line, column, scroll, hscroll, file, false, out line, out column, out scroll, out hscroll, out file, out filelenghts);
                run = true;
                
            }
        }
        
    }
}
