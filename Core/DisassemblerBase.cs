#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Core.Machine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Decompiler.Core
{
    /// <summary>
    /// A disassembler can be considered an enumerator of disassembled instructions.
    /// </summary>
    /// <typeparam name="TInstr"></typeparam>
    public abstract class DisassemblerBase<TInstr> : IEnumerable<TInstr>
    {
        public IEnumerator<TInstr> GetEnumerator()
        {
            for (;;)
            {
                var instr = DisassembleInstruction();
                if (instr == null)
                    break;
                yield return instr;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public abstract TInstr DisassembleInstruction();
    }
}