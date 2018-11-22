#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
            return dasm.DecodeOperands(wInstr, opcode, InstrClass.Linear, format);
        }
    }

    /// <summary>
    /// This instruction encoding is only valid on 64-bit MIPS architecture.
    /// </summary>
    class A64OpRec : OpRec
    {
        internal Opcode opcode;
        internal string format;

        public A64OpRec(Opcode opcode, string format)
        {
            this.opcode = opcode;
            this.format = format;
        }

        internal override MipsInstruction Decode(uint wInstr, MipsDisassembler dasm)
        {
            if (dasm.arch.PointerType.Size == 8)
                return dasm.DecodeOperands(wInstr, opcode, InstrClass.Linear, format);
            else
                return dasm.DecodeOperands(wInstr, Opcode.illegal, InstrClass.Invalid, "");
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

    class MaskDecoder : OpRec
    {
        private int shift;
        private uint mask;
        private OpRec[] decoders;

        public MaskDecoder(int shift, uint mask, params OpRec[] decoders)
        {
            this.shift = shift;
            this.mask = mask;
            this.decoders = decoders;
        }

        internal override MipsInstruction Decode(uint wInstr, MipsDisassembler dasm)
        {
            int iDecoder = (int)((wInstr >> shift) & mask);
            return decoders[iDecoder].Decode(wInstr, dasm);
        }
    }

    class SparseMaskDecoder : OpRec
    {
        private int shift;
        private uint mask;
        private Dictionary<uint, OpRec> decoders;

        public SparseMaskDecoder(int shift, uint mask, Dictionary<uint, OpRec> decoders)
        {
            this.shift = shift;
            this.mask = mask;
            this.decoders = decoders;
        }

        internal override MipsInstruction Decode(uint wInstr, MipsDisassembler dasm)
        {
            uint iDecoder = (wInstr >> shift) & mask;
            if (decoders.TryGetValue(iDecoder, out var decoder))
                return decoder.Decode(wInstr, dasm);
            else
                return new MipsInstruction { opcode = Opcode.illegal };
        }
    }
    class SpecialOpRec : OpRec
    {
        private static OpRec[] specialOpRecs = new OpRec[] 
        {
            new SllOprec(Opcode.sll, "R3,R2,s"),
            new MaskDecoder(16, 1, 
                new AOpRec(Opcode.movf, "R2,R1,C18"),
                new AOpRec(Opcode.movt, "R2,R1,C18")),
            new AOpRec(Opcode.srl, "R3,R2,s"),
            new AOpRec(Opcode.sra, "R3,R2,s"),
 
            new AOpRec(Opcode.sllv, "R3,R2,R1"),
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.srlv, "R3,R2,R1"),
            new AOpRec(Opcode.srav, "R3,R2,R1"),

            new AOpRec(Opcode.jr, "R1"),
            new AOpRec(Opcode.jalr, "R3,R1"),
            new AOpRec(Opcode.movz, "R3,R1,R2"),
            new AOpRec(Opcode.movn, "R3,R1,R2"),
            new AOpRec(Opcode.syscall, "B"),
            new AOpRec(Opcode.@break, "B"),
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.sync, "s"), 
            // 10
            new AOpRec(Opcode.mfhi, "R3"),
            new AOpRec(Opcode.mthi, "R1"),
            new AOpRec(Opcode.mflo, "R3"),
            new AOpRec(Opcode.mtlo, "R1"),
            new A64OpRec(Opcode.dsllv, "R3,R2,R1"),
            new AOpRec(Opcode.illegal, ""),
            new A64OpRec(Opcode.dsrlv, "R3,R2,R1"),
            new A64OpRec(Opcode.dsrav, "R3,R2,R1"),

            new AOpRec(Opcode.mult, "R1,R2"),
            new AOpRec(Opcode.multu, "R1,R2"),
            new AOpRec(Opcode.div, "R1,R2"),
            new AOpRec(Opcode.divu, "R1,R2"),
            new A64OpRec(Opcode.dmult, "R1,R2"),
            new A64OpRec(Opcode.dmultu, "R1,R2"),
            new A64OpRec(Opcode.ddiv, "R1,R2"),
            new A64OpRec(Opcode.ddivu, "R1,R2"),
            // 20
            new AOpRec(Opcode.add, "R3,R1,R2"),
            new AOpRec(Opcode.addu, "R3,R1,R2"),
            new AOpRec(Opcode.sub, "R3,R1,R2"),
            new AOpRec(Opcode.subu, "R3,R1,R2"),
            new AOpRec(Opcode.and, "R3,R1,R2"),
            new AOpRec(Opcode.or, "R3,R1,R2"),
            new AOpRec(Opcode.xor, "R3,R1,R2"),
            new AOpRec(Opcode.nor, "R3,R1,R2"),
 
            new AOpRec(Opcode.illegal, ""), 
            new AOpRec(Opcode.illegal, ""), 
            new AOpRec(Opcode.slt, "R3,R1,R2"),
            new AOpRec(Opcode.sltu, "R3,R1,R2"),
            new A64OpRec(Opcode.dadd, "R3,R1,R2"),
            new A64OpRec(Opcode.daddu, "R3,R1,R2"),
            new A64OpRec(Opcode.dsub, "R3,R1,R2"),
            new A64OpRec(Opcode.dsubu, "R3,R1,R2"),
            // 30
            new AOpRec(Opcode.tge, "R1,R2,T"),
            new AOpRec(Opcode.tgeu, "R1,R2,T"),
            new AOpRec(Opcode.tlt, "R1,R2,T"),
            new AOpRec(Opcode.tltu, "R1,R2,T"),
            new AOpRec(Opcode.teq, "R1,R2,T"),
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.tne, "R1,R2,T"),
            new AOpRec(Opcode.illegal, ""),

            new A64OpRec(Opcode.dsll, "R3,R2,s"),
            new AOpRec(Opcode.illegal, ""),
            new A64OpRec(Opcode.dsrl, "R3,R2,s"),
            new A64OpRec(Opcode.dsra, "R3,R2,s"),
            new A64OpRec(Opcode.dsll32, "R3,R2,s"),
            new AOpRec(Opcode.illegal, ""), 
            new A64OpRec(Opcode.dsrl32, "R3,R2,s"),
            new A64OpRec(Opcode.dsra32, "R3,R2,s"),
        };
        internal override MipsInstruction Decode(uint wInstr, MipsDisassembler dasm)
        {
            Debug.Assert(specialOpRecs.Length == 64, specialOpRecs.Length.ToString());
            var decoder = specialOpRecs[wInstr & 0x3F];
            // Debug.Print("  SpecialOpRec {0:X8} => oprec {1} {2}", wInstr, wInstr & 0x3F, opRec == null ? "(null!)" : "");
            return decoder.Decode(wInstr, dasm);
        }
    }

    class Special3OpRec : OpRec
    {
        private static OpRec[] specialOpRecs = new OpRec[]
        {
            // 00
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),

            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),

            // 10
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),

            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),

            // 20
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),

            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),

            // 30
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),
            new Version6OpRec(
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.ll, "R2,ew")),
            new Version6OpRec(
                new AOpRec(Opcode.illegal, ""),
                new A64OpRec(Opcode.lld, "R2,el")),

            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.rdhwr, "R2,H"),
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),

        };

        internal override MipsInstruction Decode(uint wInstr, MipsDisassembler dasm)
        {
            Debug.Assert(specialOpRecs.Length == 64, specialOpRecs.Length.ToString());
            var decoder = specialOpRecs[wInstr & 0x3F];
            // Debug.Print("  Special3OpRec {0:X8} => oprec {1} {2}", wInstr, wInstr & 0x3F, opRec == null ? "(null!)" : "");
            return decoder.Decode(wInstr, dasm);
        }
    }

    class CondOpRec : OpRec
    {
        static OpRec[] decoders = 
        {
            new AOpRec(Opcode.bltz,    "R1,j"),
            new AOpRec(Opcode.bgez,    "R1,j"),
            new AOpRec(Opcode.bltzl,   "R1,j"),
            new AOpRec(Opcode.bgezl,   "R1,j"),

            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),

            new AOpRec(Opcode.tgei,    "R1,I"),
            new AOpRec(Opcode.tgeiu,   "R1,I"),
            new AOpRec(Opcode.tlti,    "R1,I"),
            new AOpRec(Opcode.tltiu,   "R1,I"),

            new AOpRec(Opcode.teqi,     "R1,I"),
            new AOpRec(Opcode.illegal,  ""),
            new AOpRec(Opcode.tnei,     "R1,I"),
            new AOpRec(Opcode.illegal,  ""),
            
            new AOpRec(Opcode.bltzal,  "R1,j"),
            new AOpRec(Opcode.bgezal,  "R1,j"),
            new AOpRec(Opcode.bltzall, "R1,j"),
            new AOpRec(Opcode.bgezall, "R1,j"),

            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),

            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),

            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),
            new AOpRec(Opcode.illegal, ""),
        };

        internal override MipsInstruction Decode(uint wInstr, MipsDisassembler dasm)
        {
            var decoder = decoders[(wInstr >> 16) & 0x1F];
            return decoder.Decode(wInstr, dasm);
        }
    }

    internal class CoprocessorOpRec : OpRec
    {
        private OpRec[] oprecs;

        public CoprocessorOpRec(params OpRec[] oprecs)
        {
            this.oprecs = oprecs;
        }

        internal override MipsInstruction Decode(uint wInstr, MipsDisassembler dasm)
        {
            return oprecs[(wInstr>>21) & 0x1F].Decode(wInstr, dasm);
        }
    }

    internal class FpuOpRec : OpRec
    {
        private OpRec[] oprecs;
        private PrimitiveType size;

        public FpuOpRec(PrimitiveType size, params OpRec[] oprecs)
        {
            this.size = size;
            this.oprecs = oprecs;
        }

        internal override MipsInstruction Decode(uint wInstr, MipsDisassembler dasm)
        {
            return oprecs[wInstr & 0x3F].Decode(wInstr, dasm);
        }
    }

    internal class BcNRec : OpRec
    {
        private Opcode opFalse;
        private Opcode opTrue;

        public BcNRec(Opcode f, Opcode t)
        {
            this.opFalse = f;
            this.opTrue = t;
        }

        internal override MipsInstruction Decode(uint wInstr, MipsDisassembler dasm)
        {
            var opcode = ((wInstr & (1u << 16)) != 0) ? opTrue : opFalse;
            return dasm.DecodeOperands(wInstr, opcode, InstrClass.ConditionalTransfer|InstrClass.Delay, "c18,j");
        }
    }

    internal class Version6OpRec : OpRec
    {
        OpRec preV6Oprec;
        OpRec v6Oprec;

        public Version6OpRec(OpRec preOprec, OpRec postOprec)
        {
            this.preV6Oprec = preOprec;
            this.v6Oprec = postOprec;
        }

        internal override MipsInstruction Decode(uint wInstr, MipsDisassembler dasm)
        {
            if (dasm.isVersion6OrLater)
                return v6Oprec.Decode(wInstr, dasm);
            else
                return preV6Oprec.Decode(wInstr, dasm);
        }
    }
}
