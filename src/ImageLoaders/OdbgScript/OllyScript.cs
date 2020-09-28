#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.OdbgScript
{
    /// <summary>
    /// Represents loaded Odbg script state
    /// </summary>
    public class OllyScript
    {
        public class Line
        {
            public uint LineNumber;
            public string RawLine;
            public bool IsCommand;
            public string Command;
            public Func<string[], bool> CommandPtr;
            public string[] args = new string[0];
        }

        private string path;
        private OllyLang interpreter;

        public OllyScript(OllyLang interpreter)
        {
            this.interpreter = interpreter;
            this.IsLoaded = false;
            this.Log = false;
            this.Lines = new List<Line>();
            this.Labels = new Dictionary<string, uint>();
        }

        public bool IsLoaded { get; private set; }
        public Dictionary<string, uint> Labels { get; private set; }
        public List<Line> Lines {get; private set; }
        public bool Log { get; private set; }

        public void Clear()
        {
            IsLoaded = false;
            path = "";
            Lines.Clear();
            Labels.Clear();
        }

        private void InsertLines(List<string> toInsert, string currentdir)
        {
            uint curline = 1;
            bool in_comment = false, in_asm = false;

            IsLoaded = true;

            for (int i = 0; i < toInsert.Count; i++, curline++)
            {
                string scriptline = Helper.trim(toInsert[i]);
                bool nextline = false;
                int curpos = 0; // for skipping string literals

                while (!nextline)
                {
                    // Handle comments and string literals
                    int linecmt = -1, spancmt = -1, strdel = -1;

                    if (curpos < scriptline.Length)
                    {
                        if (in_comment)
                        {
                            spancmt = 0;
                        }
                        else
                        {
                            int min = scriptline.Length, tmp;
                            tmp = scriptline.IndexOf("//", curpos);
                            if (tmp >= 0 && tmp < min)
                                min = linecmt = tmp;
                            tmp = scriptline.IndexOf(';', curpos);
                            if (tmp >= 0 && tmp < min)
                                min = linecmt = tmp;
                            tmp = scriptline.IndexOf("/*", curpos);
                            if (tmp >= 0 && tmp < min)
                                min = spancmt = tmp;
                            tmp = scriptline.IndexOf('\"', curpos);
                            if (tmp >= 0 && tmp < min)
                                min = strdel = tmp;

                            curpos = min;

                            if (linecmt != min)
                                linecmt = -1;
                            if (spancmt != min)
                                spancmt = -1;
                            if (strdel != min)
                                strdel = -1;
                        }
                    }

                    if (strdel >= 0)
                    {
                        curpos = scriptline.IndexOf('\"', strdel + 1); // find end of string
                        if (curpos >= 0)
                            curpos++;
                    }
                    else if (linecmt >= 0)
                    {
                        scriptline = scriptline.Remove(linecmt);
                    }
                    else if (spancmt >= 0)
                    {
                        int start = in_comment ? spancmt : spancmt + 2;
                        int end = scriptline.IndexOf("*/", start);
                        in_comment = (end < 0);
                        if (in_comment)
                            scriptline = scriptline.Remove(spancmt);
                        else
                            scriptline = scriptline.Remove(spancmt) + scriptline.Substring(end - spancmt + 2);
                    }
                    else
                    {
                        scriptline = Helper.trim(scriptline);
                        int len = scriptline.Length;

                        if (len != 0)
                        {
                            string lcline = scriptline.ToLowerInvariant();

                            // Check for label
                            if (!in_asm && len > 1 && scriptline[len - 1] == ':')
                            {
                                scriptline = scriptline.Remove(len - 1);
                                Labels[Helper.trim(scriptline)] = (uint)(Lines.Count);
                            }
                            // Check for #inc and include file if it exists
                            else if (0 == lcline.IndexOf("#inc"))
                            {
                                if (len > 5 && Char.IsWhiteSpace(lcline[4]))
                                {
                                    string args = Helper.trim(scriptline.Substring(5));
                                    if (args.Length > 2 && args[0] == '\"' && args.EndsWith("\""))
                                    {
                                        string dir;
                                        string philename = Helper.pathfixup(args.Substring(1, args.Length - 2), false);
                                        if (!Path.IsPathRooted(philename))
                                        {
                                            philename = currentdir + philename;
                                            dir = currentdir;
                                        }
                                        else
											dir = Path.GetDirectoryName(philename);

                                        InsertLines(Helper.ReadLinesFromFile(philename), dir);
                                    }
                                    else interpreter.Host.MsgError("Bad #inc directive!");
                                }
                                else this.interpreter.Host.MsgError("Bad #inc directive!");
                            }
                            // Logging
                            else if (!in_asm && lcline == "#log")
                            {
                                Log = true;
                            }
                            // Add line
                            else
                            {
                                Line cur = new Line();

                                if (in_asm && lcline == "ende")
                                    in_asm = false;

                                cur.RawLine = scriptline;
                                cur.LineNumber = curline;
                                cur.IsCommand = !in_asm;

                                if (!in_asm && lcline == "exec")
                                    in_asm = true;

                                ParseArgumentsIntoLine(scriptline, cur);

                                Lines.Add(cur);
                            }
                        }
                        nextline = true;
                    }
                }
            }
        }

        public static void ParseArgumentsIntoLine(string scriptline, Line cur)
        {
            int pos = scriptline.IndexOfAny(Helper.whitespaces.ToCharArray());
            if (pos >= 0)
            {
                cur.Command = scriptline.Substring(0, pos).ToLowerInvariant();
                cur.args = Helper.split(',', scriptline.Substring(pos + 1))
                    .Select(s => s.Trim())
                    .ToArray();
            }
            else
            {
                cur.Command = scriptline.ToLowerInvariant();
            }
        }

        public int NextCommandIndex(int from)
        {
            while (from < Lines.Count && !Lines[from].IsCommand)
            {
                from++;
            }
            return from;
        }

        public bool LoadFile(string file, string dir = null)
        {
            Clear();

            string cdir = Environment.CurrentDirectory;
            string curdir = Helper.pathfixup(cdir, true);
            string sdir;

            path = Helper.pathfixup(file, false);
            if (!Path.IsPathRooted(path))
            {
                path = Path.Combine(cdir, path);
            }
            if (string.IsNullOrEmpty(dir))
				sdir = Path.GetDirectoryName(path);
            else
                sdir = dir;

            List<string> unparsedScript = Helper.ReadLinesFromFile(path);
            InsertLines(unparsedScript, sdir);
            return true;
        }

        public bool LoadScriptFromString(string buff, string dir = null)
        {
            Clear();

            string curdir = Helper.pathfixup(Environment.CurrentDirectory, true);
            string sdir;

            this.path = "";
            if (dir == null)
            {
                sdir = curdir;
            }
            else
                sdir = dir;

            List<string> unparsedScript = Helper.ReadLines(new StringReader(buff));
            InsertLines(unparsedScript, sdir);
            return true;
        }

        public bool IsLabel(string s)
        {
            return (Labels.ContainsKey(s));
        }
    }
}
