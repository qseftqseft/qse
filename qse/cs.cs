using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;
using TextCopy;

namespace qse
{
    class cs
    {
        public static string open(string filename)
        {
            if(Path.GetExtension(filename) != ".cs" )
                return "";
            Console.Write("\n");
            string outp = "";
            
            if(File.Exists(Path.GetDirectoryName(filename) + "/obj/project.assets.json"))
                outp = File.ReadAllText(Path.GetDirectoryName(filename) + "/obj/project.assets.json").Replace("\r\n", "\n");
            else
                return "";
            outp = outp.Split("\"libraries\": {")[1];
            
            
            
            int brc = 1;
            int ptr = 0;
            
            string outpf = "";
            
            while(brc > 0 && ptr+1 < outp.Count())
            {
                outpf += outp[ptr];  //add character at ptr to final output
                ptr++;
                
                
                if(outp[ptr] == '{')
                    brc++;
                if(outp[ptr] == '}')
                    brc--;
            }
            
            string outpft = "";
            string[] outpfa = outpf.Split('{');
            
            if(System.Environment.OSVersion.Platform != PlatformID.Unix)
                return "";
            
            string path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/.nuget/packages/";
            foreach(string s in outpft.Split('\n'))
            {
                string pth = path + s;
            }
            
            List<string> names = new List<string>();
            
            
            foreach(string str in outpfa)
            {
                if(str.Split("\"path\": \"").Length > 1)
                {
                    string pth = str.Split("\"path\": \"")[1].Split('"')[0]+"/";
                    
                    if(str.Split("\"files\": [").Length > 1)
                    {
                        if(str.Split("\"files\": [\n").Length < 2)
                            continue;
                        //not here
                        string sl = str.Split("\"files\": [\n")[1].Split("]")[0];
                        foreach(string s in sl.Split('\n'))
                        {
                            if(s.Split('"').Length > 1)
                            {
                                string st = pth + s.Split('"')[1];
                                if(Path.GetExtension(st) == ".xml" && !(names.Contains(Path.GetFileName(st))) && st != "")
                                {
                                    outpft += path + st+"\n";
                                    names.Add(Path.GetFileName(st));
                                    //outpft += String.Concat(names)+"\n";
                                }
                            }
                        }
                    }
                }
            }
            string files = "";
            
            List<string> notfound = new List<string>();
            
            foreach(string s in outpft.Split('\n'))
            {
                if(s == "")
                    break;
                
                if(!File.Exists(s))
                {
                    notfound.Add(s);
                    continue;
                }
                
                string file = File.ReadAllText(s);
                string[] filet = file.Split("<member name=\"");
                
                for(int i = 1; i < filet.Count(); i++)
                {
                    files += filet[i].Split('"')[0].Split(':')[1] + "\n";
                }
            }
            
            
            
            Dictionary<string, List<string>> sug = new Dictionary<string, List<string>>();
            
            foreach(string s in files.Split('\n'))
            {
                //List<string> sa = s.Split('.').ToList();    //TODO detect for unnecessary dots in comments for methods
                List<string> sa = new List<string>();
                sa.Add("");
                bool cmnt = false;
                
                for(int i = 0; i < s.Length; i++)
                {
                    if(s[i] == '.' && !cmnt)
                        sa.Add("");
                    else if (s[i] == '(')
                    {
                        sa[sa.Count()-1] += s[i];
                        cmnt = true;
                    }
                    else
                        sa[sa.Count()-1] += s[i];
                }
                
                
                for(int i = 0; i < sa.Count(); i++)
                {    if(!sug.ContainsKey(sa[i]))
                        sug.Add(sa[i], new List<string>());
                    if(i+1 < sa.Count())
                        if(!sug[sa[i]].Contains(sa[i+1]))
                            sug[sa[i]].Add(sa[i+1]);
                }
                
            }
            string sugg = "";
            for(int i = 0; i < sug.Count(); i++)
            {
                string s = sug.ElementAt(i).Key;
                List<string> sl = sug.ElementAt(i).Value;
                
                sugg += s + "\n";
                if(sl.Count > 0)
                    sugg += s + "-" + String.Join("-", sl);
                sugg+="\n";
            }
            
            foreach(string s in notfound)
            {
                Console.Write("Unable to find " + s + ", press any key to continue.\n");
                Console.ReadKey();
            }
            
            return sugg;
        }
        
        public static string run()
        {
            return "non";
        }
    }
}
