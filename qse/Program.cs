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
            

            List<int> filelenghts = new List<int>();

            
            
            Console.CursorVisible = false;
            
            Console.Clear();
            Console.CursorVisible = true;
            int line = 1;
            int column = 0;
            int scroll = 0;
            
            while (true)
            {
                num = 0;
                filelenghts.Add(0);
                filestr = string.Concat(file);
                filelenghts = lenghts(filestr);
                
                write(scroll, top, left,filelenghts, file, filename, filestr, line, column);
                
                Console.SetCursorPosition(column, line);
                
                ConsoleKeyInfo keyInfo1 = Console.ReadKey(true);
                switch (keyInfo1.Key)
                {
                    case ConsoleKey.UpArrow:
                        line--;
                        if (line >= 1)
                        {
                            if (column > filelenghts[line + scroll] - filelenghts[line - 1 + scroll])
                            {
                                column = filelenghts[line + scroll] - filelenghts[line - 1 + scroll];
                                
                            }
                        }
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
                        file.RemoveAt(filelenghts[(line+scroll)-1]+column-2+(line+scroll));
                        column--;
                        break;
                    case ConsoleKey.Enter:
                        file.Insert(filelenghts[(line+scroll)-1]+column-1+(line+scroll),  '\n');
                        column=0;
                        line++;
                        break;
                    case ConsoleKey.Delete:
                        file.RemoveAt(filelenghts[(line+scroll)-1]+column-1+(line+scroll));
                        break;
                    case ConsoleKey.Tab:
                        file.Insert(filelenghts[(line+scroll)-1]+column-1+(line+scroll),  ' ');
                        column++;
                        file.Insert(filelenghts[(line+scroll)-1]+column-1+(line+scroll),  ' ');
                        column++;
                        file.Insert(filelenghts[(line+scroll)-1]+column-1+(line+scroll),  ' ');
                        column++;
                        file.Insert(filelenghts[(line+scroll)-1]+column-1+(line+scroll),  ' ');
                        column++;
                        break;
                    case ConsoleKey.PageUp:
                        scroll = scroll - top+4;
                        break;
                    case ConsoleKey.PageDown:
                        scroll = scroll + top-4;
                        break;
                    
                    default:
                        file.Insert(filelenghts[(line+scroll)-1]+column-1+(line+scroll),  keyInfo1.KeyChar);
                        column++;
                        break;
                }

                if (column < 0 && line > 1)
                {
                    line--;
                    column = filelenghts[line + scroll] - filelenghts[line - 1 + scroll];
                }
                
                if (column < 0 )
                    column = 0;

                

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
                
                if (scroll < 0)
                {
                    scroll = 0;
                }

                if (scroll + top >= filelenghts.Count)
                {
                    scroll = filelenghts.Count - top - 1;
                }

                if (line >= 1)
                {
                    if (column > filelenghts[line + scroll] - filelenghts[line - 1 + scroll])
                    {
                        column = 0;
                        line++;

                    }
                }
                if (line > top - 1)
                    line = top - 1;
                    
                
                
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

        public static void write(int scroll, int top, int left, List<int> filelenghts, List<char> file, string filename, string filestr,int line, int column)
        {
            Stopwatch sw =  new Stopwatch();
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
            
            //for (int i = 0; i < (filelenghts[scroll+top-1] + scroll + top-1) - (filelenghts[scroll] + scroll) ; i++)
                //Console.Write(filestr[i+filelenghts[scroll]+scroll]);
                
                for (int i = 0; i < top - 1; i++)
                {
                    sw.Start();
                    for (int j = 0; j < left-3; j++)
                    {
                        Console.Write((filestr.Split("\n")[i + scroll])[j]);
                    }
                    Console.Write(sw.ElapsedMilliseconds);
                    sw.Reset();
                    Console.Write("\n");
                }

                Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.SetCursorPosition(0, top);
            Console.Write(" " + filelenghts.Count.ToString() + " lines loaded");
            
            for (int i = 0; i < left-(14+filelenghts.Count.ToString().Count()+(filelenghts[(line+scroll)-1]+column-1+(line+scroll)).ToString().Count()); i++) 
                Console.Write(" ");  Console.Write(filelenghts[(line+scroll)-1]+column-1+(line+scroll) + " ");
            
            Console.ResetColor();
        }

    }
    
}