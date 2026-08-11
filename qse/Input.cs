using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;
using TextCopy;

namespace qse
{
    class Input
    {
        public static void modeone(ConsoleKeyInfo keyInfo1, int line, int column, bool r, List<char> file, int scroll, int hscroll, int curchar, List<int> filelenghts, int tab, int top, int sugsc, string[] suggest, int mode, Settings settings, string prevstr, string nowstr, string[] match, string[] vars, bool dosug
        , out List<char> file2, out int line2, out int column2, out int scroll2, out int hscroll2, out int sugsc2, out bool r2, out int mode2, out bool sug)
        {
            //s-o-t
            char[] ignclr = settings.ignclr;
            bool code = settings.code;
            //e-o-t
            
            sug = false;
            
            switch(keyInfo1.Key)
            {
                case ConsoleKey.Escape:
                    mode = 1;
                    break;
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
                    string autocomp = "";
                    
                    if(dosug)
                    {
                        autocomp = Suggest.sugo(prevstr, nowstr, match, vars, sugsc)[0];
                    }
                    
                    if(autocomp == "")
                    {
                        for(int i = 0; i < 4; i++)
                        file.Insert(curchar, ' ');
                        column = column + 4;
                    }
                    else
                    {
                        autocomp = autocomp.Remove(0, nowstr.Length);
                        char[] cha = autocomp.ToCharArray();
                        Array.Reverse(cha);
                        foreach(char c in cha)
                        {
                            file.Insert(curchar, c);
                            column++;
                        }
                    }
                    break;
                case ConsoleKey.Backspace:
                    if(curchar - 1 >= 0)
                    {
                        file.RemoveAt(curchar - 1);
                        column--;
                        if (column < 0 && hscroll == 0 && line + scroll >= 2)
                        {
                            line--;
                            column = filelenghts[line + scroll] - filelenghts[line - 1 + scroll];
                        }
                        sug = true;
                    }
                    break;
                case ConsoleKey.Enter:
                    tab = 0;
                    while (file[filelenghts[line + scroll - 1] + line + scroll - 1 + tab] == ' ')
                    {
                        tab++;
                    }
                    if (curchar - 1 > 0) if(!ignclr.Contains(file[curchar - 1]) && code)
                    tab=tab+4;

                    file.Insert(curchar, '\n');

                    for(int i = 0; i < tab; i++)
                    {
                        file.Insert(curchar + 1, ' ');
                    }
                    column = 0;
                    line++;
                    for(int i = 0; i < tab; i++)
                    {
                        column++;
                    }
                    break;
                case ConsoleKey.Delete:
                    if(curchar+1 < file.Count)
                    {
                        file.RemoveAt(curchar);
                    }
                    break;
                case ConsoleKey.PageUp:
                    scroll = scroll - top + (top / 5);
                    break;
                case ConsoleKey.PageDown:
                    scroll = scroll + top - (top / 5);
                    break;
                case ConsoleKey.Home:
                    if(sugsc > 0) sugsc--;
                    sug = true;
                    break;
                case ConsoleKey.End:
                    if(sugsc < suggest.Length + 5) sugsc++;
                    sug = true;
                    break;
                default:
                    if (!char.IsControl(keyInfo1.KeyChar) && keyInfo1.KeyChar != '\0')
                    {
                        file.Insert(curchar ,keyInfo1.KeyChar);
                        column++;
                        sug = true;
                    }
                    break;
            }

            file2 = file;
            line2 = line;
            column2 = column;
            scroll2 = scroll;
            hscroll2 = hscroll;
            sugsc2 = sugsc;
            r2 = r;
            mode2 = mode;

        }
        public static void HandleRC(int line, int column, int scroll, int hscroll, int max, List<char> file, bool r, out int line1, out int column1, out int scroll1, out int hscroll1, out List<char> file1, out List<int> filelenghts1)
        {
            int left = Console.WindowWidth - 1;
            int top = Console.WindowHeight - 2;
            List<int> filelenghts = new List<int>();
            filelenghts.Add(0);
            string filestr = string.Concat(file);
            filelenghts = Utils.lenghts(filestr);
            
            int lastlnlen = (scroll + max - 1).ToString().Length;
            
            
            
            //cursor at end of view
            if (column >= left - lastlnlen - 2)
            {
                hscroll = column - left + lastlnlen + 2 + hscroll;
                column = left - lastlnlen - 2;
            }
            
            
            //cursor at start of view
            if(line < 1 && scroll > 0)
            {
                scroll = scroll + line - 1;
                line = 1;
            }
            
            
            //cursor at end of view
            if(line >= top)
            {
                scroll = scroll + (line - top + 1);
                line = top - 1;
                
                if(scroll + top >= filelenghts.Count())
                {
                    scroll = filelenghts.Count - top - 1;
                }
            }
            
            
            //view at end of file
            if(line >= filelenghts.Count)
            {
                line = filelenghts.Count - 1;
            }
            
            
            //scroll at end of file*/
            if(scroll + top > filelenghts.Count - 1)
            {
                line++;
                scroll = filelenghts.Count - 1 - top;
                
            }
            
            
            //scroll at start of file
            if (scroll < 0)
            {
                scroll = 0;
                line--;
            }
            
            
            //length of file does not allow scrolling
            if (scroll + top >= filelenghts.Count && top > filelenghts.Count)
            {
                scroll = 0;
            }
            
            
            //cursor at start of file
            if (column < 0 && hscroll == 0 && (line + scroll < 2))
            {
                column = 0;
            }
            
            
            //cursor at start of file
            if(line + scroll < 1)
            {
                scroll = 0;
                line = 1;
            }
            
            
            //cursor at end of line
            if ((column + hscroll == filelenghts[line + scroll] - filelenghts[line - 1 + scroll] + 1 && line + scroll < filelenghts.Count - 2) && r)
            {
                column = 0;
                hscroll = 0;
                line++;
            }
            
            
            //cursor past the end of line
            if (column + hscroll > filelenghts[line + scroll] - filelenghts[line - 1 + scroll])
            {
                column = filelenghts[line + scroll] - filelenghts[line - 1 + scroll] - hscroll;
            }
            
            
            //cursor at end of file
            if (line + scroll > filelenghts.Count - 2 && filelenghts.Count - scroll - 2 > 0)
            {
                line = filelenghts.Count - scroll - 2;
            }
            
            
            //cursor at start of line (no hscroll, line subtraction possible)
            if (column < 0 && hscroll == 0 && line + scroll >= 2)
            {
                line--;
                
                if (line <= 0)
                {
                    scroll += line - 1;
                    line = 1;
                }
                
                column = filelenghts[line + scroll] - filelenghts[line - 1 + scroll];
            }
            
            
            //cursor at start of line (hscroll subtraction possible)
            if(column < 0 && hscroll + column >= 0)
            {
                hscroll += column;
                column = 0;
            }
            
            
            line1 = line;
            column1 = column;
            scroll1 = scroll;
            hscroll1 = hscroll;
            file1 = file;
            filelenghts1 = filelenghts;
        }
    }
}
