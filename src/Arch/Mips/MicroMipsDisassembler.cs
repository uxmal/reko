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
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Mips
{
    using Decoder = Reko.Core.Machine.Decoder<MicroMipsDisassembler, Opcode, MipsInstruction>;

    public class MicroMipsDisassembler : DisassemblerBase<MipsInstruction>
    {
        private static Decoder rootDecoder;

        private readonly MipsProcessorArchitecture arch;
        private readonly EndianImageReader rdr;
        private Address addr;
        private List<MachineOperand> ops;

        public MicroMipsDisassembler(MipsProcessorArchitecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.ops = new List<MachineOperand>();
        }

        public override MipsInstruction DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadUInt16(out ushort uInstr))
                return null;
            var instr = rootDecoder.Decode(uInstr, this);
            instr.Address = addr;
            instr.Length = (int) (rdr.Address - addr);
            return instr;
        }

        protected override MipsInstruction CreateInvalidInstruction()
        {
            var instr = new MipsInstruction
            {
                InstructionClass = InstrClass.Invalid,
                opcode = Opcode.illegal,
            };
            ops.Clear();
            return instr;
        }

        public override MipsInstruction NotYetImplemented(uint wInstr, string message)
        {
            var hex = $"{wInstr:X8}";
            EmitUnitTest("uMips", hex, message, "uMipsDis", this.addr, w =>
            {
                w.WriteLine("           AssertCode(\"@@@\", \"{0}\");", hex);
            });
            return base.NotYetImplemented(wInstr, message);
        }

        private static Decoder Instr(Opcode opcode, params Mutator<MicroMipsDisassembler> [] mutators)
        {
            return new InstrDecoder(InstrClass.Linear, opcode, mutators);
        }

        private static Decoder Instr(Opcode opcode, InstrClass iclass, params Mutator<MicroMipsDisassembler>[] mutators)
        {
            return new InstrDecoder(iclass, opcode, mutators);
        }

        private static Decoder Nyi(string message)
        {
            return new NyiDecoder<MicroMipsDisassembler, Opcode, MipsInstruction>(message);
        }

        private class InstrDecoder : Decoder
        {
            private readonly InstrClass iclass;
            private readonly Opcode opcode;
            private readonly Mutator<MicroMipsDisassembler>[] mutators;

            public InstrDecoder(InstrClass iclass, Opcode opcode, Mutator<MicroMipsDisassembler>[] mutators)
            {
                this.iclass = iclass;
                this.opcode = opcode;
                this.mutators = mutators;
            }

            public override MipsInstruction Decode(uint wInstr, MicroMipsDisassembler dasm)
            {
                foreach (var mutator in mutators)
                {
                    if (!mutator(wInstr, dasm))
                        return dasm.CreateInvalidInstruction();
                }
                var ops = dasm.ops;
                var instr = new MipsInstruction
                {
                    opcode = this.opcode,
                    InstructionClass = this.iclass,
                    op1 = ops.Count > 0 ? ops[0] : null,
                    op2 = ops.Count > 1 ? ops[1] : null,
                    op3 = ops.Count > 2 ? ops[2] : null,
                };
                ops.Clear();
                return instr;
            }
        }

        private static readonly int[] threeBitRegisterEncodings = new int[8] { 16, 17, 2, 3, 4, 5, 6, 7 };
        private static readonly int[] encodedByteOffsets = new int[16] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, -1 };

        // r - 3-bit register encoding.
        private static Mutator<MicroMipsDisassembler> r(int bitpos)
        {
            var field = new Bitfield(bitpos, 3);
            return (u, d) =>
            {
                var iReg = field.Read(u);
                var iReg32 = threeBitRegisterEncodings[iReg];
                d.ops.Add(new RegisterOperand(d.arch.GetRegister(iReg32)));
                return true;
            };
        }

        private static readonly Mutator<MicroMipsDisassembler> r7 = r(7);

        // m - memory access: base + offset
        private static Mutator<MicroMipsDisassembler> m(PrimitiveType dt, int [] encodedOffsets)
        {
            var baseField = new Bitfield(4, 3);
            var offsetField = new Bitfield(0, 4);
            return (u, d) =>
            {
                var iBase = baseField.Read(u);
                var iBase32 = threeBitRegisterEncodings[iBase];
                var baseReg = d.arch.GetRegister(iBase32);
                var encOffset = offsetField.Read(u);
                var offset = encodedOffsets[encOffset];
                var mop = new IndirectOperand(dt, offset, baseReg);
                d.ops.Add(mop);
                return true;
            };
        }
        private static readonly Mutator<MicroMipsDisassembler> mb = m(PrimitiveType.Byte, encodedByteOffsets);

        static MicroMipsDisassembler()
        {
            var invalid = Instr(Opcode.illegal, InstrClass.Invalid);

            rootDecoder = Mask(10, 6,
                Nyi("pool32a"),
                Nyi("pool16a"),
                Instr(Opcode.lbu16, r7, mb),
                Nyi("move16 "),

                Nyi("aui / lui"),
                Nyi("lbu32"),
                Nyi("sb32"),
                Nyi("lb32"),

                // 
                Nyi("pool32b"),
                Nyi("pool16b"),
                Nyi("lhu16"),
                Nyi("andi16"),

                Nyi("addiu32"),
                Nyi("lhu32"),
                Nyi("sh32"),
                Nyi("lh32"),

                // 10
                Nyi("pool32i"),
                Nyi("pool16c"),
                Nyi("lwsp16"),
                Nyi("pool16d"),

                Nyi("ori32"),
                Nyi("pool32f"),
                Nyi("pool32s"),
                Nyi("daddiu32"),

                //
                Nyi("pool32c"),
                Nyi("lwgp"),
                Nyi("lw16"),
                Nyi("pool16e"),

                Nyi("xori32"),
                Nyi("bovc /beqzalc /beqc"),
                Nyi("addiupc /auipc / aluipc / ldpc /lwpc / lwupc"),
                Nyi("bnvc /bnezalc /bnec"),
                
                // 20
                Nyi("beqzc/jic"),
                Nyi("pool16f"),
                Nyi("sb16"),
                Nyi("beqzc16"),

                Nyi("slti32"),
                Nyi("bc"),
                Nyi("swc132"),
                Nyi("lwc132"),

                //
                Nyi("bnezc/jialc"),
                invalid,
                Nyi("sh16"),
                Nyi("bnezc16"),

                Nyi("sltiu32"),
                Nyi("balc"),
                Nyi("sdc132"),
                Nyi("ldc132"),

                // 30

                Nyi("blezalc/bgezalc/bgeuc"),
                invalid,
                Nyi("swsp16"),
                Nyi("bc16"),

                Nyi("andi32"),
                Nyi("bgtzc /bltzc /bltc"),
                Nyi("sd32 - 64bit"),
                Nyi("ld32 - 64bit "),

                Nyi("bgtzalc/bltzalc/bltuc"),
                invalid,
                Nyi("sw16"),
                Nyi("li16"),

                Nyi("daui"),
                Nyi("blezc /bgezc /bgec"),
                Nyi("sw32"),
                Nyi("lw32"));
        }
    }
}
