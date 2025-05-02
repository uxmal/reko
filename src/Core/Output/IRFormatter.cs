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

using System.IO;

namespace Reko.Core.Output
{
    /// <summary>
    /// Formats intermediate representation (IR) code for output.
    /// </summary>
    public class IRFormatter
    {
        private TextWriter w;

        /// <summary>
        /// Constructs an instance of <see cref="IRFormatter"/>.
        /// </summary>
        /// <param name="w">Output sink.</param>
        public IRFormatter(TextWriter w)
        {
            this.w = w;
        }

        /// <summary>
        /// Formats the program to the output.
        /// </summary>
        /// <param name="program">Program to format.</param>
        public void WriteProgram(Program program)
        {
            w.WriteLine("// reko-IR");
            foreach (var proc in program.Procedures.Values)
            {

            }
        }

        /// <summary>
        /// Formats the procedure to the output.
        /// </summary>
        /// <param name="procedure">Procedure to format.</param>
        public void WriteProcedure(Procedure procedure)
        {
            //@"define void @empty() {
            var arch = procedure.Architecture;
            var pd = new ProcedureFormatter(procedure, new CodeFormatter(new TextFormatter(this.w)));
            pd.WriteProcedureBlocks();
        }
    }
}
