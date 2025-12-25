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
        public static string[] sug(string prevstr, string str, string[] match, string[] vars)
        {

            List<string> sugg = new List<string>();
            List<string> sugtwo = new List<string>();
            List<string> classes = new List<string>();
            List<string> sugfour = new List<string>();
            List<string> sugthree = new List<string>();
            List<string> classsug = new List<string>();
            List<string> vnms = new List<string>();
            
            string sep = match[0];
            match = match.Skip(1).ToArray();
            
            List<string> matc = match.ToList();
            foreach(string s in vars) vnms.Insert(0, s.Split(sep)[1]);
            
            vnms.Sort((x, y) => y.Length.CompareTo(x.Length));
            
            foreach(string s in vnms) matc.Insert(0, s);
            match = matc.ToArray();
            
            for(int i = 0; i < match.Length; i++)
            {
                if(match[i].Contains(sep))
                {
                    string[] singleclass = match[i].Split(sep);
                    classes.Add(singleclass[0]);
                    for (int j = 1; j < singleclass.Length; j++)
                    {
                        classsug.Add(singleclass[j] + "::" + singleclass[0]);
                    }
                }
            }

            if(classes.Contains(prevstr) || str.Length > 0)
            {

                for(int i = 0; i < match.Length; i++)
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
                return sugg.ToArray().Concat(sugtwo.ToArray()).ToArray().Concat(sugthree.ToArray()).ToArray().Concat(sugfour.ToArray()).ToArray();

            }
            return [];
        }
    }
}
