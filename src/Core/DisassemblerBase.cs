#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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

using Reko.Core.Machine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Reko.Core
{
    /// <summary>
    /// A disassembler can be considered an enumerator of disassembled
    /// instructions. This convenience class lets implementors focus on the
    /// important method, DisassembleInstruction.
    /// </summary>
    /// <typeparam name="TInstr"></typeparam>
    public abstract class DisassemblerBase<TInstr> : IDisposable, IEnumerable<TInstr>
    {
        protected delegate bool Mutator<TDasm>(uint uInstr, TDasm dasm);

        public IEnumerator<TInstr> GetEnumerator()
        {
            for (;;)
            {
                TInstr instr = DisassembleInstruction();
                if (instr == null)
                    break;
                yield return instr;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Disassemble a single instruction. 
        /// </summary>
        /// <remarks>
        /// Implementors should make an effort to make sure this method doesn't
        /// throw an exception. Instead, they should return an invalid instruction
        /// and possibly spew a diagnostic message.
        /// </remarks>
        /// <returns>Return a disassembled machine instruction, or null
        /// if the end of the reader has been reached</returns>
        public abstract TInstr DisassembleInstruction();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~DisassemblerBase()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {

        }

        private static Dictionary<string, HashSet<string>> seen =
            new Dictionary<string, HashSet<string>>();

        /// <summary>
        /// Emits the text of a unit test that can be pasted into the unit tests 
        /// for a disassembler.
        /// </summary>
        [Conditional("DEBUG")]
        protected void EmitUnitTest(
            string archName, 
            string instrHexBytes,
            string message,
            string testPrefix, 
            Address addrInstr,
            Action<TextWriter> testBodyGenerator)
        {
            //$REVIEW: not thread safe.
            if (!seen.TryGetValue(archName, out var archSeen))
            {
                archSeen = new HashSet<string>();
                seen.Add(archName, archSeen);
            }
            if (archSeen.Contains(instrHexBytes))
                return;
            archSeen.Add(instrHexBytes);

            var writer = new StringWriter();
            writer.Write("// Reko: a decoder for {0} instruction {1} at address {2} has not been implemented.", archName, instrHexBytes, addrInstr);
            if (!string.IsNullOrEmpty(message))
            {
                writer.Write(" ({0})", message);
            }
            writer.WriteLine();
            writer.WriteLine("[Test]");
            writer.WriteLine("public void {0}_{1}()", testPrefix, instrHexBytes);
            writer.WriteLine("{");
            testBodyGenerator(writer);
            writer.WriteLine("}");

            Console.Out.WriteLine(writer.ToString());
        }
    }
}