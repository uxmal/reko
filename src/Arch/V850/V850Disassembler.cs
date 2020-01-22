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

using System.Collections.Generic;
using Reko.Core;
using Reko.Core.Machine;

namespace Reko.Arch.V850
{
    public class V850Disassembler : DisassemblerBase<V850Instruction, Mnemonic>
    {
        private readonly V850Architecture arch;
        private readonly EndianImageReader rdr;

        public V850Disassembler(V850Architecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
        }

        public override V850Instruction DisassembleInstruction()
        {
            if (!rdr.TryReadLeUInt16(out ushort uInstr))
                return null;
            return new V850Instruction
            {
                Mnemonic = Mnemonic.nop,
                Operands = MachineInstruction.NoOperands,
            };
        }

        public override V850Instruction CreateInvalidInstruction()
        {
            throw new System.NotImplementedException();
        }
    }
}