using System.Diagnostics;
using System.Text;

namespace qse
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Stopwatch sw  = new Stopwatch();
            
            
            sw.Start();
            
            int left = Console.WindowWidth-1;
            int top = Console.WindowHeight-1;
            int num = 0;
            string filename = "/home/qseft/test";
            string originalfile = File.ReadAllText(filename).Replace("\t", "    ");
            string filestr = "";
            List<char> file = new List<char>();
            foreach (char c in originalfile)
            {
                file.Add(c);
            }
            
            string[] display = new string[top-2];
            for (int i = 0; i < top-2; i++)
            {
                for (int j = 0; j < left; j++)
                    display[i] = " ";
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
                while (run)
                {
                    num = 0;
                    filelenghts.Add(0);
                    filestr = string.Concat(file);
                    filelenghts = lenghts(filestr);

                    write(scroll, hscroll, top, left, filelenghts, file, filename, filestr, line, column);

                    Console.SetCursorPosition(column, line);

                    ConsoleKeyInfo keyInfo1 = Console.ReadKey(true);
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
                            scroll--;
                            break;
                        case ConsoleKey.PageDown:
                            scroll++;
                            break;
                        case ConsoleKey.End:
                            run = false;
                            break;
                        default:
                            file.Insert(filelenghts[(line + scroll) - 1] + column - 1 + (line + scroll) + hscroll,
                                keyInfo1.KeyChar);
                            column++;
                            break;
                    }


                    //where is if(hscroll > lenght of a line) {stuck at end of it}

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

                    if (column < 0 && hscroll == 0 && line + scroll < 2)
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

                    if (line >= 1)
                    {
                        if (column > filelenghts[line + scroll] - filelenghts[line - 1 + scroll])
                        {
                            column = 0;
                            hscroll = 0;
                            line++;

                        }
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

                    if (scroll + top >= filelenghts.Count)
                    {
                        scroll = filelenghts.Count - top - 1;
                    }




                }
                Console.Clear();
                Console.WriteLine("SAVING, DO NOT EXIT!!!");
                File.WriteAllText(filename, filestr);
                Console.Write("SAVED, PRESS ANY KEY TO RETURN");
                Console.ReadKey();
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
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.SetCursorPosition(0, 0);
            Console.Write(" QSE");
            for (int i = 0; i < left-(4+filename.Length); i++)
                Console.Write(" ");
            Console.Write(filename+" ");
            Console.ResetColor();
            
            Console.SetCursorPosition(0, 1);
            
            /*  top     filelenghts[scroll + 1]
             *  bottom  filelenghts[scroll + top + 1]
             *  
             *  
             *  
             */
            
            int neededoutputlines = filelenghts.Count - 1;
            if (neededoutputlines > top) neededoutputlines = top;
            
            for (int i = scroll; i < neededoutputlines+scroll-1; i++)
            {
                if (filelenghts[i + 1] - filelenghts[i] - hscroll <= left)
                {
                    for (int j = filelenghts[i]+i+ hscroll; j <= filelenghts[i + 1]+i; j++)
                    {
                        Console.Write(file[j]);
                    }
                }
                else
                {
                    for (int j = filelenghts[i]+i + hscroll ; j < filelenghts[i]+left+1+i+ hscroll; j++)
                    {
                        Console.Write(file[j]);
                    }

                    Console.CursorLeft = left;
                    Console.Write(">\n");
                }

            }

            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.SetCursorPosition(0, top);
            Console.Write(" " + filelenghts.Count.ToString() + " lines loaded");
            for (int i = 0; i < left-(14+filelenghts.Count.ToString().Count()+(filelenghts[line-1]+column-1+line).ToString().Count()); i++)
                Console.Write(" ");
            Console.Write(filelenghts[line - 1] + column - 1 + line + " ");
            Console.ResetColor();
        }

    }
}