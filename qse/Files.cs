using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;
using TextCopy;

namespace qse
{
    class Files
    {
        public static List<char> OpenFile(string filename, out string originalfile)
        {
            List<char> file = new List<char>();
            originalfile = File.ReadAllText(filename).Replace("\t", "    ");
            foreach (char c in originalfile)
            {
                file.Add(c);
            }
            return file;

        }
        public static bool MakeSureConfDirExists(string settingsfile, string homeDirectory)
        {
            bool r = true;
            if (!Directory.Exists(homeDirectory + "" + Path.DirectorySeparatorChar + ".qse"))
            {
                Directory.CreateDirectory(homeDirectory + "" + Path.DirectorySeparatorChar + ".qse");
                r=false;
            }

            if (!Directory.Exists(homeDirectory + Path.DirectorySeparatorChar + ".qse" + Path.DirectorySeparatorChar + "projects"))
            {
                Directory.CreateDirectory(homeDirectory + "" + Path.DirectorySeparatorChar + ".qse" + Path.DirectorySeparatorChar + "projects");
                r=false;
            }
            if (!File.Exists(homeDirectory + "" + Path.DirectorySeparatorChar + ".qse" + Path.DirectorySeparatorChar + "projects" + Path.DirectorySeparatorChar + "projects.list"))
            {
                File.WriteAllText(homeDirectory + "" + Path.DirectorySeparatorChar + ".qse" + Path.DirectorySeparatorChar + "projects" + Path.DirectorySeparatorChar + "projects.list", "\n");
                r=false;
            }


            if (!Directory.Exists(homeDirectory + "" + Path.DirectorySeparatorChar + ".qse" + Path.DirectorySeparatorChar + "themes"))
            {
                Directory.CreateDirectory(homeDirectory + "" + Path.DirectorySeparatorChar + ".qse" + Path.DirectorySeparatorChar + "themes");
                r=false;
            }
            if (!File.Exists(homeDirectory + "" + Path.DirectorySeparatorChar + ".qse" + Path.DirectorySeparatorChar + "themes" + Path.DirectorySeparatorChar + "theme"))
            {
                File.WriteAllText(homeDirectory + "" + Path.DirectorySeparatorChar + ".qse" + Path.DirectorySeparatorChar + "themes" + Path.DirectorySeparatorChar + "theme", "38;2;080;080;080\n38;2;150;025;075\n38;2;025;150;100\n38;2;175;175;025\n38;2;075;050;175\n38;2;125;050;125\n38;2;050;125;125\n38;2;125;125;150\n38;2;100;100;100\n38;2;200;075;125\n38;2;075;200;150\n38;2;225;225;075\n38;2;125;100;225\n38;2;175;100;175\n38;2;075;175;175\n38;2;175;175;200\n38;2;115;115;150\n38;2;200;100;200\n38;2;175;050;075\n38;2;050;175;100\n48;2;175;175;025\n48;2;000;000;020\n48;2;000;000;050\n38;2;025;025;075\n38;2;075;075;150\n");
                r=false;
            }


            if (!Directory.Exists(homeDirectory + "" + Path.DirectorySeparatorChar + ".qse" + Path.DirectorySeparatorChar + "settings"))
            {
                Directory.CreateDirectory(homeDirectory + "" + Path.DirectorySeparatorChar + ".qse" + Path.DirectorySeparatorChar + "settings");
                r=false;
            }
            if (!File.Exists(homeDirectory + "" + Path.DirectorySeparatorChar + ".qse" + Path.DirectorySeparatorChar + "settings" + Path.DirectorySeparatorChar + "" + settingsfile))
            {
                File.WriteAllText(homeDirectory + "" + Path.DirectorySeparatorChar + ".qse" + Path.DirectorySeparatorChar + "settings" +Path.DirectorySeparatorChar+ settingsfile, " \n \n \n \n \n \n \n \n \n \n \n \n \n \n \n \n \n \n \n \n \n");
                r=false;
            }

            if (!Directory.Exists(homeDirectory + "" + Path.DirectorySeparatorChar + ".qse" + Path.DirectorySeparatorChar + "open"))
            {
                Directory.CreateDirectory(homeDirectory + "" + Path.DirectorySeparatorChar + ".qse" + Path.DirectorySeparatorChar + "open");
                r=false;
            }
            if (!File.Exists(homeDirectory + "" + Path.DirectorySeparatorChar + ".qse" + Path.DirectorySeparatorChar + "open" + Path.DirectorySeparatorChar + "file"))
            {
                File.WriteAllText(homeDirectory + "" + Path.DirectorySeparatorChar + ".qse" + Path.DirectorySeparatorChar + "open" + Path.DirectorySeparatorChar + "file","QSE - qseft's simple editor - the C# console-based text editor\nCopyright (C) 2025 Václav Ulrich\n\n    This program is free software: you can redistribute it and/or modify\n    it under the terms of the GNU General Public License as published by\n    the Free Software Foundation, either version 3 of the License, or\n    (at your option) any later version.\n\n    This program is distributed in the hope that it will be useful,\n    but WITHOUT ANY WARRANTY; without even the implied warranty of\n    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the\n    GNU General Public License for more details.\n\n    You should have received a copy of the GNU General Public License\n    along with this program.  If not, see <https://www.gnu.org/licenses/>.\n\nWelcome to QSE, here's a list of basic shortcuts:\n\nCTRL shortcuts\n    CTRL+L/R arrow -> jump to next thing\n    CTRL+U/D arrow -> scrolling\n    CTRL+V -> paste\n    CTRT+C -> copy\n    CTRL+X -> cut\n    CTRL+BACKSPACE/DELETE -> remove next thing\n    CTRL+SHIFT+U/D arrow -> fast scrolling\n    CTRL+SHIFT+R arrow -> jump to end of line\n    CTRL+SHIFT+L arrow -> jump to start of line\n\n\nALT shortcuts\n    ALT+M -> scroll right\n    ALT+N -> scroll left\n    ALT+S -> save file\n    ALT+G -> go to line\n    ALT+O -> open file\n    ALT+A -> set mark (for selecting text to copy/cut)\n    ALT+C -> Command mode\n    ALT+R -> Run\n    ALT+Q -> Quit (must confirm with ENTER)\n\nOther shortcuts\n    Page Up/Down -> scrolls a page (surprisingly)\n\nBe sure to check out the wiki\nqseftweb.wz.cz/qse/wiki\n\n");
                r=false;
            }
            if (!Directory.Exists(homeDirectory + "" + Path.DirectorySeparatorChar + ".qse" + Path.DirectorySeparatorChar + "suggestions" + Path.DirectorySeparatorChar ))
            {
                Directory.CreateDirectory(homeDirectory + "" + Path.DirectorySeparatorChar + ".qse" + Path.DirectorySeparatorChar + "suggestions" + Path.DirectorySeparatorChar);
                r=false;
            }
            return r;
        }
    }
}
