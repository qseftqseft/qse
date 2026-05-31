using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;
using TextCopy;

namespace qse
{
    class Suggest
    {
        public static string[] sug(string prevstr, string str, string[] match, string[] vars, int scroll)
        {
            if(str.Length < 1 && prevstr.Length < 1)
                return [""];
            
            
            List<string> sugg = new List<string>();
            List<string> sugtwo = new List<string>();
            List<string> classes = new List<string>();
            List<string> sugfour = new List<string>();
            List<string> sugthree = new List<string>();
            List<string> classsug = new List<string>();
            List<string> vnms = new List<string>();
            
            int sugcount = 6+scroll; //important
            
            string sep = match[0];
            match = match.Skip(1).ToArray();
            
            Dictionary<string, string[]> matd = new Dictionary<string, string[]>();
            
            foreach(string s in vars) vnms.Insert(0, s.Split(sep)[1]);
            foreach(string s in vars) vnms.Insert(0, s.Split(sep)[1]);
            
            vnms.Sort((x, y) => y.Length.CompareTo(x.Length));
            
            foreach(string s in vnms)
            {
                if(matd.ContainsKey(s))
                {
                    matd.Remove(s);
                }
                matd.Add(s, [s]);
            }
            
            
            foreach(string s in match)
            {
                string[] sa = s.Split(sep);
                if(matd.ContainsKey(sa[0]))
                {
                    matd.Remove(sa[0]);
                }
                matd.Add(sa[0], sa);
            }
            
            foreach(var s in matd)
            {
                if(s.Value.Length > 1)
                {
                    for (int j = 1; j < s.Value.Length; j++)
                    {
                        classsug.Add(s.Value[j] + "::" + s.Value[0]);
                    }
                }
            }
            
            /*
            str, prevstr
            
            
            */
            
            if(matd.ContainsKey(prevstr))
            {
                string[] wherez = matd[prevstr].Where(s => s.StartsWith(str)).ToArray();
                foreach(string s in wherez)
                {
                    if(sugcount > 0 && scroll < 1)
                    {
                        sugg.Add(s);
                        sugcount--;
                    }
                    else if(sugcount > 0)
                    {
                        scroll--;
                    }
                    else return sugg.ToArray();
                        
                }
            }
            
            string[] where = matd.Keys.Where( key => key.StartsWith(str)).ToArray();
            foreach(string s in where)
                if(String.Concat(matd[s]) == s)
                    if(sugcount > 0 && scroll < 1)
                    {
                        sugg.Add(s);
                        sugcount--;
                    }
                    else if(sugcount > 0)
                    {
                        scroll--;
                    }
                    else return sugg.ToArray();
            
            
            List<string> vals = new List<string>();
            matd.Values.ToList().ForEach(x => x.ToList().ForEach(y => vals.Add(y)));
            string[] atk = vals.Distinct().ToArray();                                    //all the keywords
            
            List<string> kwds = atk.Where(x => Utils.StartsWith(x, str)).ToList();
            
            foreach(string s in sugg)
                if(kwds.Contains(s))
                    kwds.Remove(s);
            
            foreach(string s in kwds)
            {
                string cls = "";
                foreach(var x in matd)
                {
                    foreach(string y in x.Value)
                        if(y == s)
                            cls = x.Key;
                }
                
                if(sugcount > 0 && scroll < 1)
                {
                    sugg.Add(s+"::"+cls);
                    sugcount--;
                }
                else if(sugcount > 0)
                {
                    scroll--;
                }
                else return sugg.ToArray();
            }
            
            
            if(sugg.Count() > 0)
                return sugg.ToArray();
            
            return [""];
                
            /*
            if(classes.Contains(prevstr) || str.Length > 0)
            {

                for(int i = 0; i < match.Length; i++)    //does match contain match[i]
                {
                    if(match[i].Contains(sep))
                    {
                        string[] arr = match[i].Split(sep);
                        if (prevstr == arr[0])
                        {
                            for(int j = 1; j < arr.Length; j++)
                            {
                                if(Utils.StartsWith(arr[j], str))      sugg.Add(arr[j]);
                                else if(arr[j].Contains(str))    sugtwo.Add(" "+arr[j]);
                            }
                            break;
                        }
                    }
                    else if (!classes.Contains(prevstr))
                    {
                        if(Utils.StartsWith(match[i], str))      sugg.Add(match[i]);
                        else if(match[i].Contains(str))    sugtwo.Add(" "+match[i]);
                    }
                }
                for(int i = 0; i < classsug.Count; i++)
                {
                    if(Utils.StartsWith(classsug[i], str))      sugthree.Add(classsug[i]);
                    else if(classsug[i].Contains(str))    sugfour.Add(" "+classsug[i]);
                }
                return sugg.ToArray().Concat(sugthree.ToArray()).ToArray().Concat(sugtwo.ToArray()).ToArray().Concat(sugfour.ToArray()).ToArray();

            }
            */
            
            
        }
    }
}
