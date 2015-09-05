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

using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.Arch.Mips
{
    abstract class OpRec 
    {
        internal abstract MipsInstruction Decode(uint wInstr, MipsDisassembler dasm);
    }

    class AOpRec : OpRec
    {
        internal Opcode opcode;
        internal string format;

        public AOpRec(Opcode opcode, string format)
        {
            this.opcode = opcode;
            this.format = format;
        }

        internal override MipsInstruction Decode(uint wInstr, MipsDisassembler dasm)
        {
            return dasm.DecodeOperands(opcode, wInstr, format);
        }
    }

    class SllOprec : AOpRec
    {
        public SllOprec(Opcode opcode, string format) : base(opcode, format) { }

        internal override MipsInstruction Decode(uint wInstr, MipsDisassembler dasm)
        {
            var instr = base.Decode(wInstr, dasm);
            var imm = instr.op3 as ImmediateOperand;
            if (imm != null && imm.Value.IsIntegerZero)
                return new MipsInstruction {
                    Address = instr.Address,
                    Length = instr.Length,
                    opcode = Opcode.nop };
            else
                return instr;
        }
    }
    class SpecialOpRec : OpRec
    {
        private static OpRec[] specialOpRecs = new OpRec[] 
        {
            new SllOprec(Opcode.sll, "R3,R2,s"),
            null,
            new AOpRec(Opcode.srl, "R3,R2,s"),
            new AOpRec(Opcode.sra, "R3,R2,s"),
 
            new AOpRec(Opcode.sllv, "R3,R2,R1"),
            null,
            new AOpRec(Opcode.srlv, "R3,R2,R1"),
            new AOpRec(Opcode.srav, "R3,R2,R1"),

            new AOpRec(Opcode.jr, "R1"),
            new AOpRec(Opcode.jalr, "R3,R1"),
            new AOpRec(Opcode.movz, "R3,R1,R2"),
            new AOpRec(Opcode.movn, "R3,R1,R2"),
            new AOpRec(Opcode.syscall, null),
            new AOpRec(Opcode.@break, "B"),
            null,
            new AOpRec(Opcode.sync, null), 
            // 10
            new AOpRec(Opcode.mfhi, "R2"),
            new AOpRec(Opcode.mthi, "R2"),
            new AOpRec(Opcode.mflo, "R2"),
            new AOpRec(Opcode.mtlo, "R2"),
            new AOpRec(Opcode.dsllv, "R3,R2,R1"),
            null,
            new AOpRec(Opcode.dsrlv, "R3,R2,R1"),
            new AOpRec(Opcode.dsrav, "R3,R2,R1"),

            new AOpRec(Opcode.mult, "R1,R2"),
            new AOpRec(Opcode.multu, "R1,R2"),
            new AOpRec(Opcode.div, "R1,R2"),
            new AOpRec(Opcode.divu, "R1,R2"),
            new AOpRec(Opcode.dmult, "R1,R2"),
            new AOpRec(Opcode.dmultu, "R1,R2"),
            new AOpRec(Opcode.ddiv, "R1,R2"),
            new AOpRec(Opcode.ddivu, "R1,R2"),
            // 20
            new AOpRec(Opcode.add, "R3,R1,R2"),
            new AOpRec(Opcode.addu, "R3,R1,R2"),
            new AOpRec(Opcode.sub, "R3,R1,R2"),
            new AOpRec(Opcode.subu, "R3,R1,R2"),
            new AOpRec(Opcode.and, "R3,R1,R2"),
            new AOpRec(Opcode.or, "R3,R1,R2"),
            new AOpRec(Opcode.xor, "R3,R1,R2"),
            new AOpRec(Opcode.nor, "R3,R1,R2"),
 
            null, null, null,
            new AOpRec(Opcode.sltu, "R3,R1,R2"),
            new AOpRec(Opcode.dadd, "R3,R1,R2"),
            new AOpRec(Opcode.daddu, "R3,R1,R2"),
            new AOpRec(Opcode.dsub, "R3,R1,R2"),
            new AOpRec(Opcode.dsubu, "R3,R1,R2"),
            // 30
            null, null, null, null,
            null, null, null, null, 
            new AOpRec(Opcode.dsll, "R3,R2,s"),
            null,
            new AOpRec(Opcode.dsrl, "R3,R2,s"),
            new AOpRec(Opcode.dsra, "R3,R2,s"),
            new AOpRec(Opcode.dsll32, "R3,R2,s"),
            new AOpRec(Opcode.illegal, ""), 
            new AOpRec(Opcode.dsrl32, "R3,R2,s"),
            new AOpRec(Opcode.dsra32, "R3,R2,s"),
        };
        internal override MipsInstruction Decode(uint wInstr, MipsDisassembler dasm)
        {
            Debug.Assert(specialOpRecs.Length == 64, specialOpRecs.Length.ToString());
            var opRec = specialOpRecs[wInstr & 0x3F];
            Debug.Print("  SpecialOpRec {0:X8} => oprec {1} {2}", wInstr, wInstr & 0x3F, opRec == null ? "(null!)" : "");
            return opRec.Decode(wInstr, dasm);
        }
    }

    class CondOpRec : OpRec
    {
        Opcode[] opcodes = 
        {
            Opcode.bltz,
            Opcode.bgez,
            Opcode.bltzl,
            Opcode.bgezl,

            Opcode.illegal,
            Opcode.illegal,
            Opcode.illegal,
            Opcode.illegal,

            Opcode.illegal,
            Opcode.illegal,
            Opcode.illegal,
            Opcode.illegal,

            Opcode.illegal,
            Opcode.illegal,
            Opcode.illegal,
            Opcode.illegal,

            Opcode.bltzal,
            Opcode.bgezal,
            Opcode.bltzall,
            Opcode.bgezall,

            Opcode.illegal,
            Opcode.illegal,
            Opcode.illegal,
            Opcode.illegal,

            Opcode.illegal,
            Opcode.illegal,
            Opcode.illegal,
            Opcode.illegal,

            Opcode.illegal,
            Opcode.illegal,
            Opcode.illegal,
            Opcode.illegal,
        };

        internal override MipsInstruction Decode(uint wInstr, MipsDisassembler dasm)
        {
            var opcode = opcodes[(wInstr >> 16) & 0x1F];
            return dasm.DecodeOperands(opcode, wInstr, "R1,j");
        }
    }
}
