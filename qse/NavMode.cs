using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;
using TextCopy;

namespace qse
{
    class NavMode
    {
        public static void modetwo(ConsoleKeyInfo keyInfo1, int line, int column, bool r, string autocomp, List<char> file, int scroll, int hscroll, int curchar, List<int> filelenghts, int tab, int top, int sugsc, string[] suggest, int mode, char prevch, char prevtf, int prevtfm, Settings settings,
        out List<char> file2, out int line2, out int column2, out int scroll2, out int hscroll2, out int sugsc2, out bool r2, out int mode2, out char prevch2, out char prevtf2, out int prevtfm2)
        {
            //s-o-t
            char[] ignclr = settings.ignclr;
            bool code = settings.code;
            //e-o-t
            
            if (prevch == 'f')
            {
                //jump to next occurrence of character x
                if(file[curchar] != '\n' && curchar < file.Count())
                {
                    column++;
                    curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);
                    while(file[curchar] != keyInfo1.KeyChar && file[curchar] != '\n' && curchar < file.Count())
                    {
                        column++;
                        curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);
                    }
                }
                prevtf = keyInfo1.KeyChar;
                prevtfm = 0;
                prevch = '\0';
            }
            else if (prevch == 't')
            {
                //jump to before next occurrence of character x

                if(file[curchar] != '\n' && curchar < file.Count())
                {
                    column++;
                    curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);
                    while(file[curchar+1] != keyInfo1.KeyChar && file[curchar+1] != '\n' && curchar+1 < file.Count())
                    {
                        column++;
                        curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);
                    }
                }
                prevtf = keyInfo1.KeyChar;
                prevtfm = 1;
                prevch = '\0';
            }
            else if (prevch == 'F')
            {
                // jump to the previous occurrence of character x
                if(curchar > 0) if(file[curchar-1] != '\n')
                {
                    column--;
                    curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);
                    while(file[curchar] != keyInfo1.KeyChar && file[curchar] != '\n' && curchar < file.Count())
                    {
                        column--;
                        curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);
                    }
                }
                prevtf = keyInfo1.KeyChar;
                prevtfm = 2;
                prevch = '\0';
            }
            else if (prevch == 'T')
            {
                //jump to after previous occurrence of character x
                if(curchar > 0) if(file[curchar-1] != '\n')
                {
                    column--;
                    curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);
                    if(curchar > 0) while(file[curchar-1] != keyInfo1.KeyChar && file[curchar-1] != '\n' && curchar-1 < file.Count())
                    {
                        column--;
                        curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);
                    }
                }
                prevtf = keyInfo1.KeyChar;
                prevtfm = 3;
                prevch = '\0';
            }
            else switch (keyInfo1.KeyChar)
            {
                case 'h':
                    column--;
                    break;
                case 'j':
                    line++;
                    break;
                case 'k':
                    line--;
                    break;
                case 'l':
                    column++;
                    r = true;
                    break;
                case 'H':
                    line = 0;
                    break;
                case 'M':
                    line = -1 + Console.WindowHeight / 2;
                    break;
                case 'L':
                    line = Console.WindowHeight - 3;
                    break;
                case 'w':
                    //jump forwards to the start of a word
                    if(curchar < file.Count())
                    {
                        if(file[curchar] == '\n'){ line++; column=0; hscroll=0;}
                        else column++;

                        curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);
                        while(!ignclr.Contains(file[curchar]))
                        {
                            column++;
                            curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);
                        }
                        while(ignclr.Contains(file[curchar]))
                        {
                            if(file[curchar] == '\n'){ line++; column=0; hscroll=0;}
                            else column++;
                            curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);
                        }
                    }
                    break;
                case 'W':
                    //jump forwards to the start of a word (words can contain punctuation)
                    if(curchar < file.Count())
                    {
                        if(file[curchar] == '\n'){ line++; column=0; hscroll=0;}
                        else column++;

                        curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);

                        while((file[curchar] != ' ' && file[curchar] != '\n'))
                        {
                            if(file[curchar] == '\n'){ line++; column=0; hscroll=0;}
                            else column++;
                            curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);
                        }

                        while((file[curchar] == ' ') || file[curchar] == '\n')
                        {
                            if(file[curchar] == '\n'){ line++; column=0; hscroll=0;}
                            else column++;
                            curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);
                        }
                    }
                    break;
                case 'e':
                    if(prevch == 'g')
                    {
                        //jump backwards to the end of a word
                        if(curchar > 0)
                        {
                            column--;
                            curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);
                            while( !ignclr.Contains(file[curchar]) && curchar > 0)
                            {
                                column--;
                                curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);
                            }
                            while( ignclr.Contains(file[curchar]) && curchar > 0)
                            {
                                column--;
                                curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);
                            }
                        }
                    }
                    else
                    {
                        //jump forwards to the end of a word
                        if(curchar < file.Count())
                        {
                            if(file[curchar] == '\n'){ line++; column=0; hscroll=0;}
                            else column++;
                            curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);

                            while(ignclr.Contains(file[curchar])&& curchar < file.Count())
                            {
                                if(file[curchar] == '\n'){ line++; column=0; hscroll=0;}
                                else column++;
                                curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);
                            }

                            while(!ignclr.Contains(file[curchar+1])  && curchar+1 < file.Count())
                            {
                                if(file[curchar] == '\n'){ line++; column=0; hscroll=0;}
                                else column++;
                                curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);
                            }
                        }
                    }
                    break;
                case 'E':
                    if(prevch == 'g')
                    {
                        //jump backwards to the end of a word (words can contain punctuation)
                        if(curchar > 0)
                        {
                            column--;
                            curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);
                            while( file[curchar] != ' '  && curchar > 0)
                            {
                                column--;
                                curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);
                            }
                            while( file[curchar] == ' ' && curchar > 0)
                            {
                                column--;
                                curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);
                            }
                        }
                    }
                    else
                    {
                        //jump forwards to the end of a word (words can contain punctuation)
                        if(curchar < file.Count())
                        {
                            if(file[curchar] == '\n'){ line++; column=0; hscroll=0;}
                            else column++;
                            curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);

                            while((file[curchar] == ' ' || file[curchar] == '\n')&& curchar < file.Count())
                            {
                                if(file[curchar] == '\n'){ line++; column=0; hscroll=0;}
                                else column++;
                                curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);
                            }

                            while((file[curchar+1] != ' ' && file[curchar+1] != '\n')  && curchar+1 < file.Count())
                            {
                                if(file[curchar] == '\n'){ line++; column=0; hscroll=0;}
                                else column++;
                                curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);
                            }
                        }
                    }
                    break;
                case 'b':
                    if (prevch == 'z')
                    {
                        //position cursor on bottom of the screen

                        scroll = (scroll - (top - 1 - line));
                        line = top - 1;

                    }
                    else
                    {
                        //jump backwards to the start of a word
                        if(curchar > 0)
                        {
                            column--;
                            curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);

                            while(ignclr.Contains(file[curchar])&& curchar < file.Count())
                            {
                                column--;
                                curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);
                            }

                            while(!ignclr.Contains(file[curchar-1])  && curchar-1 > 0)
                            {
                                column--;
                                curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);
                            }
                        }
                    }
                    break;
                case 'B':
                    //jump backwards to the start of a word (words can contain punctuation)
                    if(curchar > 0)
                    {
                        column--;
                        curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);

                        while(( file[curchar] == ' ' || file[curchar] == '\n')&& curchar < file.Count())
                        {
                            column--;
                            curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);
                        }

                        while( file[curchar-1] != ' ' && file[curchar-1] != '\n'  && curchar-1 > 0)
                        {
                            column--;
                            curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);
                        }
                    }
                    break;
                case '%':
                    //move cursor to matching character (default supported pairs: '()', '{}', '[]')
                    int numc = 0;
                    char cho = '\0';
                    char cht = '\0';
                    column = column + hscroll;
                    hscroll = 0;

                    switch(file[curchar])
                    {
                        case '(':
                            cho = '(';
                            cht = ')';
                            break;
                        case ')':
                            cho = ')';
                            cht = '(';
                            break;
                        case '{':
                            cho = '{';
                            cht = '}';
                            break;
                        case '}':
                            cho = '}';
                            cht = '{';
                            break;
                        case '[':
                            cho = '[';
                            cht = ']';
                            break;
                        case ']':
                            cho = ']';
                            cht = '[';
                            break;
                    }

                    if(cho == '(' || cho == '{' || cho == '[')
                    {
                        do
                        {
                            while(file[curchar] != '\n')
                            {
                                if(file[curchar] == cho) numc++;
                                else if (file[curchar] == cht){ numc--;}
                                if(numc < 1) break;
                                column++;
                                curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);
                            }

                            if(numc < 1) break;
                            hscroll = 0;
                            column = 0;
                            line++;
                            curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);

                        }while(numc > 0);
                    }
                    else if(cho == ')' || cho == '}' || cho == ']')
                    {
                        do
                        {
                            curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);
                            if(file[curchar] == cho) {numc++; }
                            else if (file[curchar] == cht){ numc--; Console.Write("suc");}

                            if(numc < 1) break;
                            if(column > 0) column--; else {if(scroll > 0) scroll--; else if (line > 1) line--; else break; column = filelenghts[line+scroll] - filelenghts[line+scroll-1];}

                        }while(numc > 0);
                    }


                    break;
                        case '0':
                            //jump to the start of the line
                            column = 0;
                            hscroll = 0;
                            break;
                        case '^':
                            //jump to the first non-blank character of the line
                            column = 0;
                            hscroll = 0;
                            curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);
                            while(file[curchar] == ' ')
                            {
                                column++;
                                curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);
                            }
                            break;
                        case '$':
                            //jump to the end of the line
                            hscroll = 0;
                            column = filelenghts[line + scroll] -  filelenghts[line - 1 + scroll];
                            curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);

                            if(file[curchar-1] != '\n')
                                column--;

                break;
                        case '_':
                            if(prevch == 'g')
                            {
                                //jump to the last non-blank character of the line
                                hscroll = 0;
                                column = filelenghts[line + scroll] -  filelenghts[line - 1 + scroll];
                                curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);

                                if(file[curchar-1] != '\n')
                                    column--;

                                curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);
                                while(file[curchar] == ' ')
                                {
                                    column--;
                                    curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);
                                }
                            }

                            break;
                        case 'G':
                            //go to the last line of the document
                            scroll = 0;
                            line = filelenghts.Count;

                            break;
                        case ';':
                            //repeat previous f, t, F or T movement
                            switch(prevtfm)
                            {
                                case 0:
                                    if(file[curchar] != '\n' && curchar < file.Count())
                                    {
                                        column++;
                                        curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);
                                        while(file[curchar] != prevtf && file[curchar] != '\n' && curchar < file.Count())
                                        {
                                            column++;
                                            curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);
                                        }
                                    }
                                    break;
                                case 1:
                                    if(file[curchar] != '\n' && curchar < file.Count())
                                    {
                                        column++;
                                        curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);
                                        while(file[curchar+1] != prevtf && file[curchar+1] != '\n' && curchar+1 < file.Count())
                                        {
                                            column++;
                                            curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);
                                        }
                                    }
                                    break;
                                case 2:
                                    if(curchar > 0) if(file[curchar-1] != '\n')
                                    {
                                        column--;
                                        curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);
                                        while(file[curchar] != prevtf && file[curchar] != '\n' && curchar < file.Count())
                                        {
                                            column--;
                                            curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);
                                        }
                                    }
                                    break;
                                case 3:
                                    if(curchar > 0) if(file[curchar-1] != '\n')
                                    {
                                        column--;
                                        curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);
                                        if(curchar > 0) while(file[curchar-1] != prevtf && file[curchar-1] != '\n' && curchar-1 < file.Count())
                                        {
                                            column--;
                                            curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);
                                        }
                                    }
                                    break;
                            }
                            break;
                                case ',':
                                    //repeat previous f, t, F or T movement, backwards
                                    switch(prevtfm)
                                    {
                                        case 2:
                                            if(file[curchar] != '\n' && curchar < file.Count())
                                            {
                                                column++;
                                                curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);
                                                while(file[curchar] != prevtf && file[curchar] != '\n' && curchar < file.Count())
                                                {
                                                    column++;
                                                    curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);
                                                }
                                            }
                                            break;
                                        case 3:
                                            if(file[curchar] != '\n' && curchar < file.Count())
                                            {
                                                column++;
                                                curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);
                                                while(file[curchar+1] != prevtf && file[curchar+1] != '\n' && curchar+1 < file.Count())
                                                {
                                                    column++;
                                                    curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);
                                                }
                                            }
                                            break;
                                        case 0:
                                            if(curchar > 0) if(file[curchar-1] != '\n')
                                            {
                                                column--;
                                                curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);
                                                while(file[curchar] != prevtf && file[curchar] != '\n' && curchar < file.Count())
                                                {
                                                    column--;
                                                    curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);
                                                }
                                            }
                                            break;
                                        case 1:
                                            if(curchar > 0) if(file[curchar-1] != '\n')
                                            {
                                                column--;
                                                curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);
                                                if(curchar > 0) while(file[curchar-1] != prevtf && file[curchar-1] != '\n' && curchar-1 < file.Count())
                                                {
                                                    column--;
                                                    curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);
                                                }
                                            }
                                            break;
                                    }
                                    break;
                                        case '}':
                                            //jump to next paragraph (or function/block, when editing code)
                                            bool tfal = true;
                                            while(tfal)
                                            {
                                                while(file[curchar] != '\n')
                                                {
                                                    column++;
                                                    curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);
                                                }



                                                if(scroll >= String.Concat(file).Split('\n').Count() - Console.WindowHeight ) line++;
                                                else scroll++;

                                                column = 0;
                                                hscroll = 0;
                                                curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);

                                                if(curchar >= file.Count()){line--; break;}

                                                while(file[curchar] != '\n')
                                                {
                                                    column++;
                                                    curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);

                                                    if(file[curchar] != '\n' && file[curchar] != ' ') break;
                                                    else if(file[curchar] == '\n') { tfal = false; break;}
                                                    else if (curchar+1 >= file.Count()) break;
                                                }
                                                if(file[curchar] == '\n') break;
                                            }

                                            break;
                                        case '{':
                                            //jump to previous paragraph (or function/block, when editing code)
                                            tfal = true;

                                            if(scroll <= 0 && line > 1) line--;
                                            else if (scroll > 0) scroll--;
                                            else break;
                                            column = 0;
                hscroll = 0;
                curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);

                do
                {
                    if(file[curchar] == '\n'){if(scroll <= 0 && line > 1) line--; else if (scroll > 0) scroll--; else break; curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);}

                    column++;
                    curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);

                    if(file[curchar] == '\n') {tfal = false; break;}

                    else if(file[curchar] != ' ')
                    {
                        if(scroll <= 0 && line > 1) line--;
                        else if (scroll > 0) scroll--;
                        else break;

                        column = 0;
                        hscroll = 0;
                        curchar = Utils.curchar(filelenghts, line, scroll, column, hscroll);
                        continue;
                    }
                    else if (curchar+1 >= file.Count()) break;
                }while(file[curchar] != '\n');

                    break;




                                        case 'z':
                                            if(prevch == 'z')
                                            {
                                                //center cursor on screen
                                                scroll = (scroll - (top/2 - 1 - line));
                                                line = top/2 - 1;


                                                prevch = '\0';
                                            }
                                            else
                                            {
                                                prevch = 'z';
                                            }
                                            break;
                                        case 'f':
                                            prevch = 'f';
                                            break;
                                        case 't':
                                            if (prevch == 'z')
                                            {
                                                //position cursor on top of the screen
                                                scroll = (scroll + line );
                                                line = 0;
                                                prevch = '\0';
                                            }
                                            else
                                            {
                                                prevch = 't';
                                            }
                                            break;
                                        case 'F':
                                            prevch = 'F';
                                            break;
                                        case 'T':
                                            prevch = 'T';
                                            break;
                                        case 'g':
                                            if(prevch == 'g')
                                            {
                                                //go to the first line of the document
                                                line=0;
                                                scroll=0;
                                                prevch = '\0';
                                            }
                                            else
                                            {
                                                prevch = 'g';
                                            }
                                            break;
                                        case 'i':
                                            mode = 0;
                                            break;
            }
            if(keyInfo1.KeyChar != 'g' && keyInfo1.KeyChar != 'f' && keyInfo1.KeyChar != 't' && keyInfo1.KeyChar != 'F' && keyInfo1.KeyChar != 'T' && keyInfo1.KeyChar != 'z')
                prevch = '\0';
            
            file2 = file;
            line2 = line;
            column2 = column;
            scroll2 = scroll;
            hscroll2 = hscroll;
            sugsc2 = sugsc;
            r2 = r;
            mode2 = mode;
            prevch2 = prevch;
            prevtf2 = prevtf;
            prevtfm2 = prevtfm;
        }

    }
}
