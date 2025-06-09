#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

using Reko.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.ImageLoaders.OdbgScript
{
    /// <summary>
    /// Represents loaded Odbg script state.
    /// </summary>
    public class OllyScript
    {
        public class Line
        {
            public int LineNumber;
            public string? Label;
            public string? RawLine;
            public bool IsCommand;
            public string? Command;
            public Func<Expression[], bool>? CommandPtr;
            public Expression[] Args = Array.Empty<Expression>();

            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.AppendFormat("{0}", LineNumber + 1);
                var sep = ": ";
                if (Label is not null)
                {
                    sb.AppendFormat("{0}{1}: ", sep, Label);
                    sep = "";
                }
                if (Command is not null)
                {
                    sb.AppendFormat("{0}{1}", sep, Command);
                    sep = " ";
                    foreach (var arg in Args)
                    {
                        sb.Append(sep);
                        sep = ", ";
                        sb.Append(arg);
                    }
                }
                return sb.ToString();
            }
        }

        public OllyScript()
        {
            this.Log = false;
            this.Lines = [];
            this.Labels = [];
        }

        public Dictionary<string, int> Labels { get; }
        public List<Line> Lines { get; }
        public bool Log { get; private set; }

        public int NextCommandIndex(int from)
        {
            while (from < Lines.Count && !Lines[from].IsCommand)
            {
                from++;
            }
            return from;
        }

        public bool TryGetLabel(Expression exp, out int lineNumber)
        {
            if (exp is Identifier id && Labels.TryGetValue(id.Name, out lineNumber))
                return true;
            lineNumber = -1;
            return false;
        }
    }
}
