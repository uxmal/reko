#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using System.Linq;
using System.Text;

namespace Reko.UserInterfaces.AvaloniaUI.ViewModels.Documents
{
    public class ProcedureViewModel
    {
        private int i;

        public ProcedureViewModel(int i)
        {

            this.i = i;
            var rnd = new Random(i);
            int Rnd(int mod) => rnd.Next() % mod;
            var sb = new StringBuilder();
            int lines = Rnd(6) + Rnd(6) + Rnd(6) + 1;
            for (int iline = 0; iline < lines; ++iline)
            {
                int linelength = (rnd.Next() % 15) + (rnd.Next() % 15) + (rnd.Next() % 15);
                for (int j = 0; j < linelength; ++j)
                {
                    sb.Append((char)((rnd.Next() % 40) + 'A'));
                }
                sb.AppendLine();
            }
            this.Body = sb.ToString();
        }

        public string Name => $"proc{i:X8}";

        public string Body { get; }

        public string Markdown
        {
            get
            {
                var sb = new StringBuilder();
                sb.AppendLine("%{color:#0088FF}void% fn010010(%{color:#0088FF}char% * arg04) {  ");
                sb.AppendLine("\t%{color:#000090}if% (x % 10 == 0) {  ");
                sb.AppendLine("\t\t[fn424242()](reko:424242)  ");
                sb.AppendLine("\t}  ");
                sb.AppendLine("}  ");
                sb.AppendLine("  ");
                sb.AppendLine("```c");
                sb.AppendLine("int foo() {");
                sb.AppendLine("\tif (x % 10 == 0) {  ");
                sb.AppendLine("\t\tfoo();  ");
                sb.AppendLine("\t}  ");
                sb.AppendLine("}");
                sb.AppendLine("```");
                return sb.ToString();
            }
        }
    }
}