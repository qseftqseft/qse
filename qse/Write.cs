using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;
using TextCopy;

namespace qse
{
    class Write
    {
        public static string colour(string str, string[][] efs, string[] colours, char[] ignclr, int h, int i, List<char> file, char strng, string filestring, List<int>[] overrides, string prevexp = "")
        {

            if(overrides[h][i] == 1 ) { return colours[19];}
            if(overrides[h][i] == 2 ) { return colours[18];}

            return colourString(str, efs, colours, ignclr, prevexp);
        }

        public static string colourString(string str, string[][] efs, string[] colours, char[] ignclr, string prevexp)
        {
            string[] prevexps = ["string", "int", "char"];
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
            
            else if (prevexps.Contains(prevexp))
            {
                return colours[10];
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
                string prevexp = "";
                
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
                    else
                        addi = expr;


                    foreach( char c in (colour(expr, efs, colours, ignclr, h+top, i, file, strng, String.Join("\n", filestr), overrides, prevexp) + addi)) output.Add(c);
                    
                    prevexp = expr;
                    
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

        public static string write(int scroll, int hscroll, int top, int left, List<int> filelenghts, List<char> file, string filename, string filestr,int line,int column,  string currentproject, char strng, char[] ignclr, string[][] efs, bool marked, int mark, string[] colours, int mode, char prevch)
        {
            string write = listColourAndCutoff(file, Console.WindowWidth-((filelenghts.Count).ToString().Length)-1, colours[16], ignclr, efs, colours, scroll,  Console.WindowHeight - 3, hscroll, strng);
            
            List<string> writel = write.Split('\n').ToList();
            
            int lnlen = (scroll + Console.WindowHeight - 3).ToString().Length;
            
            
            Console.Write(colours[21]);
            Console.Write("\x1b[2J");
            
            Console.SetCursorPosition( lnlen + 1, 1);
            foreach (string s in writel)
            {
                Console.WriteLine(s);
                Console.CursorLeft = lnlen + 1;
            }
            writeCutoffs(scroll, hscroll-lnlen-1, Console.WindowWidth, filelenghts, Console.WindowHeight-3, colours[15]);
            
            
            
            //linunumbers
            Console.SetCursorPosition (0, 1);
            int max = top;
            if (max > filelenghts.Count - 1)
                max = filelenghts.Count - 1;

            for(int i = 1; i < max; i++)
            {
                Console.SetCursorPosition(0 , i);
                for(int j = 0; j < lnlen - (i+scroll).ToString().Length; j++)
                    Console.Write(colours[22] + colours[23] + "0");

                Console.Write(colours[22] +colours[24] +(i+scroll));
            }
            
            
            
            
            
            //UI
            Console.SetCursorPosition(0, 0);
            Console.Write(colours[25]+colours[26]);
            if(mode == 0)
            {
                Console.Write(" QSE");
                for (int i = 0; i < left-(4+filename.Length); i++)
                    Console.Write(" ");
            }
            else if(mode == 1)
            {
                string topleft = " QSE-nav  ";
                if(prevch != '\0') topleft = topleft + prevch;
                Console.Write(topleft);
                for (int i = 0; i < left-(topleft.Length+filename.Length); i++)
                    Console.Write(" ");
            }

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
            List<int>[] filespec = new List<int>[file.Where(s=>s == '\n').Count()+1];
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


    }
}
