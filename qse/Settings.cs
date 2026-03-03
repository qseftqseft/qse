using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;
using TextCopy;

namespace qse
{
    class Settings
    {
        public char[]                         ignclr {get; set;}
        public char                              str {get; set;}
        public Dictionary<string, string[]>  colours {get; set;}
        public bool                             code {get; set;}
        public bool                          curfile {get; set;}
        public string[]                        types {get; set;}
        public string                        runexec {get;}
        public string                       runflags {get;}
        public string                     runcommand {get;}
        
        public Settings(string file)
        {
            colours = new Dictionary<string, string[]>();
            
            if(!File.Exists(file))
                throw new FileNotFoundException();
            
            string[] settings = File.ReadAllText(file).Split('\n');
            ignclr = (settings[0] + "Æ\n").Split('Æ').SelectMany(s => s.ToCharArray()).ToArray();
            
            string[] coloursstr = settings.Skip(1).Take(16).ToArray();
            string[] colnames = ["black", "red", "green", "yellow", "blue", "magenta", "cyan", "white", "bright black", "bright red", "bright green", "bright yellow", "bright blue", "bright magenta", "bright cyan", "bright white"];
            
            
            
            for (int i = 0; i < coloursstr.Length; i++)
                colours.Add(colnames[i], coloursstr[i].Split('Æ') );
            
            str = char.Parse(settings[17]);
            
            runexec =    settings[19];
            runflags =   settings[20];
            runcommand = settings[21];
            
            code = false;
            curfile = false;
            
            if (settings[18][0] == '1')
            {
                curfile = true;
            }
            if (settings[18][1] == '1')
            {
                code = true;
            }
            
            types = settings[22].Split('Æ');
            
        }
        
    }
}
            
           
//

