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
        public static string colour(string str, string[][] efs, string[] colours, char[] ignclr, int h, int i, List<char> file, char strng, string filestring, List<int>[] overrides, string[] exps)
        {
            if(overrides.Length > h)
                if(overrides[h].Count > i)
                {
                    if(overrides[h][i] == 1 ) { return colours[19];}
                    if(overrides[h][i] == 2 ) { return colours[18];}
                }
            
            return colourString(str, efs, colours, ignclr, exps);
        }
        
        public static string colourString(string str, string[][] efs, string[] colours, char[] ignclr, string[] exps)
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
            
            else if(exps.Contains(str))
            {
                return colours[29];
            }
            
            else
            {
                return colours[16];
            }
        }
        
        
        public static string listColourAndCutoff(List<char> file, int maxWidth, string dcolour, string[] colours, int top, int height, int hscroll, string[] exps, int[] mark, int[] pos, bool marked, Settings settings)
        {
            //s-o-t
            string[][] efs = [ settings.colours["black"],
                                 settings.colours["red"],
                               settings.colours["green"],
                              settings.colours["yellow"],
                                settings.colours["blue"],
                             settings.colours["magenta"],
                                settings.colours["cyan"],
                               settings.colours["white"],
                        settings.colours["bright black"],
                          settings.colours["bright red"],
                        settings.colours["bright green"],
                       settings.colours["bright yellow"],
                         settings.colours["bright blue"],
                      settings.colours["bright magenta"],
                         settings.colours["bright cyan"],
                        settings.colours["bright white"] ];
            
            char strng = settings.str;
            bool code = settings.code;
            char[] ignclr = settings.ignclr;
            //e-o-t
            
            
            // maxWidth, dcolour, []ignclr, [][]efs, []colours, top, height, hscroll
            
            List<string> filestr = string.Concat(file).Split('\n').Skip(top).Take(height).ToList();
            List<int>[] overrides = ColourOverrides(file, strng);
            List<char> output = new List<char>();
            
            //make each line length correct
            for(int i = 0; i < filestr.Count(); i++)
            {
                List<char> lsch = filestr[i].ToCharArray().ToList();
                while(lsch.Count() < maxWidth + hscroll)
                    lsch.Add(' ');
                filestr[i] = String.Concat(lsch);
            }
            
            
            for(int h = 0; h < filestr.Count(); h++)
            {
                int k = hscroll;
                string lne = filestr[h];
                
                bool isfirst = false;
                int i = hscroll;
                
                if(lne.Length > i) while(i > 0 && !ignclr.Contains(lne[i])) {  i--;  isfirst = true;  }
                if(isfirst) while( ignclr.Contains(lne[i]) && i < lne.Length-1) {i++;}
                int j = i;
                
                //top = hscroll
                //k = current index
                //mark[line+scroll, column+hscroll]
                
                if(marked && (mark[0] <= h+top+1 && pos[0] >= h+top+1) && (mark[1] <= k || (mark[0] < h+top+1 && pos[0] >= h+top+1))) foreach(char c in colours[20]) output.Add(c);
                if(marked && pos[0] == h+top+1 && pos[1] == k) foreach(char ch in colours[21]) output.Add(ch);
                
                if(i < lne.Length) while(ignclr.Contains(lne[i]) && i < lne.Length)
                {
                    foreach(char c in ( colour(lne[i] + "", efs, colours, ignclr, h+top, i, file, strng, String.Join("\n", filestr), overrides, exps) ))
                    {
                        output.Add(c);
                    }
                    
                    
                    output.Add(lne[i]);
                    
                    if(marked && mark[0] == h+top+1 && mark[1]-1 == k) foreach(char ch in colours[20]) output.Add(ch);
                    if(marked && pos[0] == h+top+1 && pos[1] == k) foreach(char ch in colours[21]) output.Add(ch);
                    
                    k++;
                    
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
                    
                    
                    foreach( char c in (colour(expr, efs, colours, ignclr, h+top, i, file, strng, String.Join("\n", filestr), overrides, exps)))
                        output.Add(c);
                    foreach( char c in addi)
                    {
                        output.Add(c);    //here
                        if(marked && mark[0] == h+top+1 && mark[1]-1 == k) foreach(char ch in colours[20]) output.Add(ch);
                        if(marked && pos[0] == h+top+1 && pos[1] == k) foreach(char ch in colours[21]) output.Add(ch);
                        k++;
                    }
                    
                    
                    isfirst=false;
                    
                    i++;
                    
                    if(i < lne.Length) while(ignclr.Contains(lne[i]) && i < lne.Length && i < hscroll+maxWidth)    //yes, this is the exact same code
                    {
                        foreach(char c in ( colour(lne[i] + "", efs, colours, ignclr, h+top, i, file, strng, String.Join("\n", filestr), overrides, exps) )) output.Add(c);
                        
                        output.Add(lne[i]); //here
                        
                        if(marked && mark[0] == h+top+1 && mark[1]-1 == k) foreach(char ch in colours[20]) output.Add(ch);
                        if(marked && pos[0] == h+top+1 && pos[1] == k) foreach(char ch in colours[21]) output.Add(ch);
                        
                        k++;
                        
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
        
        public static string write(int scroll, int hscroll, int top, int left, List<int> filelenghts, List<char> file, string filename, string filestr,int line,int column,  string currentproject, bool marked, int[] mark, int mode, char prevch, string[] exps, int[] pos, string previous, Settings settings, string[] colours)
        {
            if(top > Console.WindowHeight-1)
                top = Console.WindowHeight-1;
            
            
            int max = top;
            if (max > filelenghts.Count - 1)
                max = filelenghts.Count - 1;
            
            int height = max - 1;
            
            string write = listColourAndCutoff(file, Console.WindowWidth-((filelenghts.Count).ToString().Length)-1, colours[16], colours, scroll,  height, hscroll, exps, mark, pos, marked, settings);
            
            List<string> writel = write.Split('\n').ToList();
            
            
            List<char>[] outp = new List<char>[ Console.WindowHeight ];        
            for (int i = 0; i < outp.Length; i++) outp[i] = new List<char>();  
            
            for (int i = 0; i < writel.Count-1; i++)
            {
                foreach(char c in writel[i]) outp[i+1].Add(c);
            }
            
            
            //linunumbers
            
            int lastlnlen = (scroll + max - 1).ToString().Length;
            
            for(int i = 1; i < max; i++)
            {
                //NOTICE! this is in reverse order, so that it is possible to insert at [0]
                
                
                //space between line numbers
                outp[i].Insert(0, ' ');
                
                //reset colour
                foreach(char c in  colours[21].Reverse()    ) outp[i].Insert(0, c);
                
                //numbers
                foreach(char c in (""+(i+scroll)).Reverse() ) outp[i].Insert(0, c);
                foreach(char c in colours[24].Reverse()     ) outp[i].Insert(0, c);
                
                //leading zero
                for(int j = 0; j < lastlnlen - (i+scroll).ToString().Length; j++)
                    outp[i].Insert(j, '0');
                
                foreach( char c in (colours[22] + colours[23]).Reverse() ) outp[i].Insert(0, c);
            }
            
            
            //UI
            foreach(char c in (colours[25]+colours[26])) outp[0].Add(c);
            
            
            string topleft = " QSE";
            if(mode == 1)
            {
                topleft = " QSE-nav  ";
                if(prevch != '\0') topleft = topleft + prevch;
                
            }
            
            foreach(char c in topleft) outp[0].Add(c);
            
            for (int i = 0; i < left-(topleft.Length+filename.Length); i++)
                outp[0].Add(' ');
            
            
            foreach(char c in filename+" ") outp[0].Add(c);
            
            if(outp.Length < top) top = outp.Length - 1;
            foreach(char c in (colours[25]+colours[26])) outp[top].Add(c);
            
            foreach(char c in " " + (filelenghts.Count-2).ToString() + " lines loaded") outp[top].Add(c);
            for (int i = 0; i < left-(14+(filelenghts.Count-2).ToString().Count()+currentproject.ToString().Count()); i++)
                outp[top].Add(' ');
                
            foreach(char c in currentproject + " ") outp[top].Add(c);
            
            foreach(char c in (colours[21]+colours[16])) outp[top].Add(c);
            
            
            //line filling
            if(previous == "") for(int i = 0; i < outp.Length; i++) if(outp[i].Count() < 1)
            {
                for(int j = 0; j < left; j++)
                    outp[i].Insert(0, ' ');
                foreach(char c in  (colours[21]).Reverse()    ) outp[i].Insert(0, c);
            }
            
            
            List<string> outplst = new List<string>();
            foreach(List<char> l in outp) outplst.Add(String.Concat(l));
            string outpstr = String.Join("\n", outplst);
            
            Console.CursorVisible = false;
            
            quickWrite(previous, outpstr);
            
            
            //last line clearing
            Console.SetCursorPosition(0, Console.WindowHeight-1);
            Console.Write(new string(' ', Console.WindowWidth-1));
            
            Console.SetCursorPosition(0, 0);
            
            
            //cutoffs            
            for(int i = scroll; i < scroll+Console.WindowHeight-3; i++)
            {
                Console.SetCursorPosition(Console.WindowWidth-1, 1+i-scroll);
                if(filelenghts.Count() > i+1) if(filelenghts[i+1] - filelenghts[i] > Console.WindowWidth+hscroll-lastlnlen-2)
                {
                    Console.Write(colours[15] + ">");
                }
                else
                {
                    Console.Write(colours[15] + " ");
                }
            }
            
            Console.CursorVisible = true;
            
            return outpstr;
        }
        
        public static void quickWrite(string prevwrite, string write, int offset=0)
        {
            
            if(prevwrite == "")
            {
                Console.SetCursorPosition(0, 0);
                Console.Write(write);
                return;
            }
            
            List<string> prevwritelst = prevwrite.Split('\n').ToList();
            string[] writearr = write.Split('\n');
            
            for(int i = 0; i < writearr.Length; i++)
                    if(writearr[i].Length < 1)
                        writearr[i] = new string(' ', Console.WindowWidth);
            
            while( writearr.Length >  prevwritelst.Count )
            {
                prevwritelst.Add("");
            }
            
            for(int i = 0; i < writearr.Length; i++)
            {
                if( prevwritelst[i] != writearr[i]) 
                {
                    
                    Console.SetCursorPosition(0, i + offset);
                    Console.Write(writearr[i]);
                }
            }
            
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
