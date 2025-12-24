using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;
using TextCopy;

namespace qse
{
    class PM
    {
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
                proj = File.ReadAllText(homeDirectory + "" + Path.DirectorySeparatorChar + ".qse" + Path.DirectorySeparatorChar + "projects" + Path.DirectorySeparatorChar + "projects.list");
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

                int editingproject = Utils.writeMenu(projectsnames, 0, texto, textt, true, bg);
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
                    int whattodo = Utils.writeMenu(pfnames ?? [""], projectsnames.OrderByDescending(s =>  s?.Length ?? 0).FirstOrDefault()?.Length+5 ?? 5,texto, textt, false, bg);
                    if(whattodo == -1)
                        continue;



                    if(whattodo != pfnames?.Length-1)
                    {
                        int oped = Utils.writeMenu(["Open", "Delete"], (projectsnames.OrderByDescending(s => s?.Length ?? 0).FirstOrDefault()?.Length+5 ?? 5) + (pfnames?.OrderByDescending(s => s?.Length ?? 0).FirstOrDefault()?.Length+5 ?? 5), texto, textt, false, bg);
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
                            string r = Console.ReadLine() ?? string.Empty;
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
                        Console.CursorLeft = projectsnames.OrderByDescending(s => s.Length).FirstOrDefault()?.Length+10 ?? 10;
                        string rl = "";
                        do{
                            Console.Write("name          ");
                            Console.CursorLeft -= 14;
                            rl = Console.ReadLine() ?? string.Empty;
                            if(rl == "" || rl == "exit")
                                break;
                            if(rl.Contains(" "))
                                continue;
                            projects[editingproject].Add(rl);

                            Console.CursorLeft = projectsnames.OrderByDescending(s => s.Length).FirstOrDefault()?.Length+10 ?? 10;
                            Console.CursorTop -= 1;
                            Console.Write("path"); for(int i = 4; i < rl.Length; i++) Console.Write(" ");
                            Console.CursorLeft = projectsnames.OrderByDescending(s => s.Length).FirstOrDefault()?.Length+10 ?? 10;
                            rl = Console.ReadLine() ?? string.Empty;
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
                    string name = Console.ReadLine() ?? string.Empty;
                    string rl = "";
                    string other = "";
                    do{
                        Console.Write("What is the file name: [exit]");
                        rl = Console.ReadLine() ?? string.Empty;
                        if(rl == "" || rl == "exit")
                            break;
                        if(rl.Contains(" "))
                            continue;
                        other = other + " " + rl;
                        Console.Write("What is the file path ");
                        rl = Console.ReadLine() ?? string.Empty;
                        if(rl.Contains(" "))
                            continue;
                        other = other + " " + rl;

                    }while(true);
                    proj = proj + name + other + "\n";

                }
                File.WriteAllText(homeDirectory + "" + Path.DirectorySeparatorChar + ".qse" + Path.DirectorySeparatorChar + "projects" + Path.DirectorySeparatorChar + "projects.list", proj);
            }

            return proj;
        }
    }
}
