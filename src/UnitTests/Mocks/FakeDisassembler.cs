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
 
using Reko.Core;
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.UnitTests.Mocks
{
    public class FakeDisassembler : DisassemblerBase<MachineInstruction, Mnemonic>
    {
        private IEnumerator<MachineInstruction> instrs;
        private Address addr;
        private MachineInstruction instr;

        public FakeDisassembler(Address a, IEnumerator<MachineInstruction> e)
        {
            this.addr = a;
            this.instrs = e;
        }

        public override MachineInstruction DisassembleInstruction()
        {
            if (!instrs.MoveNext())
                return null;
            instr = instrs.Current;
            instr.Address = addr;
            instr.Length = 4;
            addr += 4;
            return instr;
        }

        public override MachineInstruction CreateInvalidInstruction()
        {
            throw new NotImplementedException();
        }
    }
}
