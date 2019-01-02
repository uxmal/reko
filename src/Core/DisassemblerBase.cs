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
    }
}