#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.MSP430
{
    public class MSP430Disassembler : DisassemblerBase<MSP430Instruction>
    {
        private EndianImageReader rdr;
        private MSP430Architecture arch;

        public MSP430Disassembler(MSP430Architecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
        }

        public override MSP430Instruction DisassembleInstruction()
        {
            ushort uInstr;
            if (!rdr.TryReadLeUInt16(out uInstr))
                return null;
            return s_decoders[uInstr >> 12].Decode(this, uInstr);
        }

        private MSP430Instruction Decode(uint uInstr, Opcode opcode, string fmt)
        {
            PrimitiveType dataWidth = null;
            return new MSP430Instruction
            {
                opcode = opcode,
                dataWidth = dataWidth,
                op1 = new MemoryOperand(dataWidth)
                {
                    Base = Registers.GpRegisters[4],
                    Offset = 0,
                },
                op2 = new RegisterOperand (Registers.GpRegisters[2]),
            };
        }

        private abstract class OpRecBase
        {
            public abstract MSP430Instruction Decode(MSP430Disassembler dasm, ushort uInstr);
        }

        private class OpRec : OpRecBase
        {
            private string fmt;
            private Opcode opcode;

            public OpRec(Opcode opcode, string fmt)
            {
                this.opcode = opcode;
                this.fmt = fmt;
            }

            public override MSP430Instruction Decode(MSP430Disassembler dasm, ushort uInstr)
            {
                return dasm.Decode(uInstr, opcode, fmt);
            }
        }

        private static OpRecBase[] s_decoders = new OpRecBase[16]
        {
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.xor, ""),
            new OpRec(Opcode.invalid, ""),
        };
    }
}
