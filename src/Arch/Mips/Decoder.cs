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

using Reko.Core;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using static Reko.Arch.Mips.MipsDisassembler;

namespace Reko.Arch.Mips
{
    abstract class Decoder 
    {
        internal abstract MipsInstruction Decode(uint wInstr, MipsDisassembler dasm);
    }

    class InstrDecoder : Decoder
    {
        private readonly InstrClass iclass;
        private readonly Opcode opcode;
        private readonly Mutator<MipsDisassembler> [] mutators;

        public InstrDecoder(Opcode opcode, params Mutator<MipsDisassembler> [] mutators)
        {
            this.iclass = InstrClass.Linear;
            this.opcode = opcode;
            this.mutators = mutators;
        }

        public InstrDecoder(InstrClass iclass, Opcode opcode, params Mutator<MipsDisassembler>[] mutators)
        {
            this.iclass = iclass;
            this.opcode = opcode;
            this.mutators = mutators;
        }

        internal override MipsInstruction Decode(uint wInstr, MipsDisassembler dasm)
        {
            foreach (var m in mutators)
            {
                if (!m(wInstr, dasm))
                {
                    return new MipsInstruction
                    {
                        InstructionClass = InstrClass.Invalid,
                        opcode = Opcode.illegal
                    };
                }
            }
            return new MipsInstruction
            {
                opcode = opcode,
                InstructionClass = iclass,
                Address = dasm.addr,
                Length = 4,
                op1 = dasm.ops.Count > 0 ? dasm.ops[0] : null,
                op2 = dasm.ops.Count > 1 ? dasm.ops[1] : null,
                op3 = dasm.ops.Count > 2 ? dasm.ops[2] : null,
            };
        }
    }

    /// <summary>
    /// This instruction encoding is only valid on 64-bit MIPS architecture.
    /// </summary>
    class A64Decoder : InstrDecoder
    {
        private readonly Opcode opcode;
        private readonly Mutator<MipsDisassembler>[] mutators;

        public A64Decoder(Opcode opcode, params Mutator<MipsDisassembler>[] mutators) : base(opcode, mutators)
        {
            this.opcode = opcode;
            this.mutators = mutators;
        }

        internal override MipsInstruction Decode(uint wInstr, MipsDisassembler dasm)
        {
            if (dasm.arch.PointerType.Size == 8)
                return base.Decode(wInstr, dasm);
            else
                return new MipsInstruction
                {
                    InstructionClass = InstrClass.Invalid,
                    opcode = Opcode.illegal
                };
        }
    }

    class SllDecoder : InstrDecoder
    {
        public SllDecoder(Opcode opcode, params Mutator<MipsDisassembler>[] mutators) : base(opcode, mutators) { }

        internal override MipsInstruction Decode(uint wInstr, MipsDisassembler dasm)
        {
            var instr = base.Decode(wInstr, dasm);
            if (instr.op3 is ImmediateOperand imm && imm.Value.IsIntegerZero)
                return new MipsInstruction {
                    Address = instr.Address,
                    Length = instr.Length,
                    opcode = Opcode.nop };
            else
                return instr;
        }
    }

    class MaskDecoder : Decoder
    {
        private readonly int shift;
        private readonly uint mask;
        private readonly Decoder[] decoders;

        public MaskDecoder(int shift, uint mask, params Decoder[] decoders)
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

    class SparseMaskDecoder : Decoder
    {
        private readonly int shift;
        private readonly uint mask;
        private readonly Dictionary<uint, Decoder> decoders;

        public SparseMaskDecoder(int shift, uint mask, Dictionary<uint, Decoder> decoders)
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
    class SpecialDecoder : Decoder
    {
        private static Decoder[] specialDecoders = new Decoder[] 
        {
            new SllDecoder(Opcode.sll, R3,R2,s),
            new MaskDecoder(16, 1, 
                new InstrDecoder(Opcode.movf, R2,R1,C18),
                new InstrDecoder(Opcode.movt, R2,R1,C18)),
            new InstrDecoder(Opcode.srl, R3,R2,s),
            new InstrDecoder(Opcode.sra, R3,R2,s),
 
            new InstrDecoder(Opcode.sllv, R3,R2,R1),
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.srlv, R3,R2,R1),
            new InstrDecoder(Opcode.srav, R3,R2,R1),

            new InstrDecoder(Opcode.jr, R1),
            new InstrDecoder(Opcode.jalr, R3,R1),
            new InstrDecoder(Opcode.movz, R3,R1,R2),
            new InstrDecoder(Opcode.movn, R3,R1,R2),
            new InstrDecoder(Opcode.syscall, B),
            new InstrDecoder(Opcode.@break, B),
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.sync, s), 
            // 10
            new InstrDecoder(Opcode.mfhi, R3),
            new InstrDecoder(Opcode.mthi, R1),
            new InstrDecoder(Opcode.mflo, R3),
            new InstrDecoder(Opcode.mtlo, R1),
            new A64Decoder(Opcode.dsllv, R3,R2,R1),
            new InstrDecoder(Opcode.illegal),
            new A64Decoder(Opcode.dsrlv, R3,R2,R1),
            new A64Decoder(Opcode.dsrav, R3,R2,R1),

            new InstrDecoder(Opcode.mult, R1,R2),
            new InstrDecoder(Opcode.multu, R1,R2),
            new InstrDecoder(Opcode.div, R1,R2),
            new InstrDecoder(Opcode.divu, R1,R2),
            new A64Decoder(Opcode.dmult, R1,R2),
            new A64Decoder(Opcode.dmultu, R1,R2),
            new A64Decoder(Opcode.ddiv, R1,R2),
            new A64Decoder(Opcode.ddivu, R1,R2),
            // 20
            new InstrDecoder(Opcode.add, R3,R1,R2),
            new InstrDecoder(Opcode.addu, R3,R1,R2),
            new InstrDecoder(Opcode.sub, R3,R1,R2),
            new InstrDecoder(Opcode.subu, R3,R1,R2),
            new InstrDecoder(Opcode.and, R3,R1,R2),
            new InstrDecoder(Opcode.or, R3,R1,R2),
            new InstrDecoder(Opcode.xor, R3,R1,R2),
            new InstrDecoder(Opcode.nor, R3,R1,R2),
 
            new InstrDecoder(Opcode.illegal), 
            new InstrDecoder(Opcode.illegal), 
            new InstrDecoder(Opcode.slt, R3,R1,R2),
            new InstrDecoder(Opcode.sltu, R3,R1,R2),
            new A64Decoder(Opcode.dadd, R3,R1,R2),
            new A64Decoder(Opcode.daddu, R3,R1,R2),
            new A64Decoder(Opcode.dsub, R3,R1,R2),
            new A64Decoder(Opcode.dsubu, R3,R1,R2),
            // 30
            new InstrDecoder(Opcode.tge, R1,R2,T),
            new InstrDecoder(Opcode.tgeu, R1,R2,T),
            new InstrDecoder(Opcode.tlt, R1,R2,T),
            new InstrDecoder(Opcode.tltu, R1,R2,T),
            new InstrDecoder(Opcode.teq, R1,R2,T),
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.tne, R1,R2,T),
            new InstrDecoder(Opcode.illegal),

            new A64Decoder(Opcode.dsll, R3,R2,s),
            new InstrDecoder(Opcode.illegal),
            new A64Decoder(Opcode.dsrl, R3,R2,s),
            new A64Decoder(Opcode.dsra, R3,R2,s),
            new A64Decoder(Opcode.dsll32, R3,R2,s),
            new InstrDecoder(Opcode.illegal), 
            new A64Decoder(Opcode.dsrl32, R3,R2,s),
            new A64Decoder(Opcode.dsra32, R3,R2,s),
        };
        internal override MipsInstruction Decode(uint wInstr, MipsDisassembler dasm)
        {
            Debug.Assert(specialDecoders.Length == 64, specialDecoders.Length.ToString());
            var decoder = specialDecoders[wInstr & 0x3F];
            // Debug.Print("  SpecialDecoder {0:X8} => decoder {1} {2}", wInstr, wInstr & 0x3F, decoder == null ? "(null!)" : "");
            return decoder.Decode(wInstr, dasm);
        }
    }

    class Special3Decoder : Decoder
    {
        private static Decoder[] specialDecoders = new Decoder[]
        {
            // 00
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),

            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),

            // 10
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),

            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),

            // 20
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),

            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),

            // 30
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),
            new Version6Decoder(
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.ll, R2,ew)),
            new Version6Decoder(
                new InstrDecoder(Opcode.illegal),
                new A64Decoder(Opcode.lld, R2,el)),

            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.rdhwr, R2,H),
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),
        };

        internal override MipsInstruction Decode(uint wInstr, MipsDisassembler dasm)
        {
            Debug.Assert(specialDecoders.Length == 64, specialDecoders.Length.ToString());
            var decoder = specialDecoders[wInstr & 0x3F];
            // Debug.Print("  Special3Decoder {0:X8} => decoder {1} {2}", wInstr, wInstr & 0x3F, decoder == null ? "(null!)" : "");
            return decoder.Decode(wInstr, dasm);
        }
    }

    class CondDecoder : Decoder
    {
        static Decoder[] decoders = 
        {
            new InstrDecoder(Opcode.bltz,  R1,j),
            new InstrDecoder(Opcode.bgez,  R1,j),
            new InstrDecoder(Opcode.bltzl, R1,j),
            new InstrDecoder(Opcode.bgezl, R1,j),

            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),

            new InstrDecoder(Opcode.tgei,    R1,I),
            new InstrDecoder(Opcode.tgeiu,   R1,I),
            new InstrDecoder(Opcode.tlti,    R1,I),
            new InstrDecoder(Opcode.tltiu,   R1,I),

            new InstrDecoder(Opcode.teqi,    R1,I),
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.tnei,    R1,I),
            new InstrDecoder(Opcode.illegal),
            
            new InstrDecoder(Opcode.bltzal,  R1,j),
            new InstrDecoder(Opcode.bgezal,  R1,j),
            new InstrDecoder(Opcode.bltzall, R1,j),
            new InstrDecoder(Opcode.bgezall, R1,j),

            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),

            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),

            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),
            new InstrDecoder(Opcode.illegal),
        };

        internal override MipsInstruction Decode(uint wInstr, MipsDisassembler dasm)
        {
            var decoder = decoders[(wInstr >> 16) & 0x1F];
            return decoder.Decode(wInstr, dasm);
        }
    }

    internal class CoprocessorDecoder : Decoder
    {
        private readonly Decoder[] decoders;

        public CoprocessorDecoder(params Decoder[] decoders)
        {
            this.decoders = decoders;
        }

        internal override MipsInstruction Decode(uint wInstr, MipsDisassembler dasm)
        {
            return decoders[(wInstr>>21) & 0x1F].Decode(wInstr, dasm);
        }
    }

    internal class FpuDecoder : Decoder
    {
        private readonly Decoder[] decoders;

        public FpuDecoder(PrimitiveType size, params Decoder[] decoders)
        {
            this.decoders = decoders;
        }

        internal override MipsInstruction Decode(uint wInstr, MipsDisassembler dasm)
        {
            return decoders[wInstr & 0x3F].Decode(wInstr, dasm);
        }
    }

    internal class BcNDecoder : Decoder
    {
        private readonly Decoder opFalse;
        private readonly Decoder opTrue;

        public BcNDecoder(Decoder f, Decoder t)
        {
            this.opFalse = f;
            this.opTrue = t;
        }

        internal override MipsInstruction Decode(uint wInstr, MipsDisassembler dasm)
        {
            var decoder = ((wInstr & (1u << 16)) != 0) ? opTrue : opFalse;
            return decoder.Decode(wInstr, dasm);
        }
    }

    internal class Version6Decoder : Decoder
    {
        private readonly Decoder preV6Odecoder;
        private readonly Decoder v6Odecoder;

        public Version6Decoder(Decoder preDecoder, Decoder postDecoder)
        {
            this.preV6Odecoder = preDecoder;
            this.v6Odecoder = postDecoder;
        }

        internal override MipsInstruction Decode(uint wInstr, MipsDisassembler dasm)
        {
            if (dasm.isVersion6OrLater)
                return v6Odecoder.Decode(wInstr, dasm);
            else
                return preV6Odecoder.Decode(wInstr, dasm);
        }
    }
}
