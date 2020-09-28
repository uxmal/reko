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
using Reko.Core.Expressions;
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
    using Decoder = Reko.Core.Machine.Decoder<MicroMipsDisassembler, Mnemonic, MipsInstruction>;

    public class MicroMipsDisassembler : DisassemblerBase<MipsInstruction, Mnemonic>
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
            ops.Clear();
            var instr = rootDecoder.Decode(uInstr, this);
            instr.Address = addr;
            instr.Length = (int) (rdr.Address - addr);
            return instr;
        }

        public override MipsInstruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            var instr = new MipsInstruction
            {
                Mnemonic = mnemonic,
                InstructionClass = iclass,
                Operands = this.ops.ToArray()
            };
            return instr;
        }

        public override MipsInstruction CreateInvalidInstruction()
        {
            var instr = new MipsInstruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.illegal,
                Operands = MachineInstruction.NoOperands
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

        // Factory methods for decoders

        private static Decoder Instr(Mnemonic opcode, params Mutator<MicroMipsDisassembler> [] mutators)
        {
            return new InstrDecoder<MicroMipsDisassembler, Mnemonic, MipsInstruction>(InstrClass.Linear, opcode, mutators);
        }

        private static Decoder Instr(Mnemonic opcode, InstrClass iclass, params Mutator<MicroMipsDisassembler>[] mutators)
        {
            return new InstrDecoder<MicroMipsDisassembler, Mnemonic, MipsInstruction>(iclass, opcode, mutators);
        }

        //
        private static Decoder Low16(Decoder decoder)
        {
            return new ReadLow16Decoder(decoder);
        }

        /// 
        /// 

        private static Decoder Nyi(string message)
        {
            return new NyiDecoder<MicroMipsDisassembler, Mnemonic, MipsInstruction>(message);
        }

        private class ReadLow16Decoder : Decoder
        {
            private readonly Decoder decoder;

            public ReadLow16Decoder(Decoder decoder)
            {
                this.decoder = decoder;
            }

            public override MipsInstruction Decode(uint wInstr, MicroMipsDisassembler dasm)
            {
                if (!dasm.rdr.TryReadUInt16(out ushort uLow16Bits))
                    return dasm.CreateInvalidInstruction();
                uint uInstrNew = (wInstr << 16) | uLow16Bits;
                return decoder.Decode(uInstrNew, dasm);
            }
        }

        private static readonly int[] threeBitRegisterEncodings = new int[8] { 16, 17, 2, 3, 4, 5, 6, 7 };
        private static readonly int[] threeBitRegisterEncodingsWithZero = new int[8] { 0, 17, 2, 3, 4, 5, 6, 7 };
        private static readonly int[] encodedByteOffsets = new int[16] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, -1 };

        // R - 5-bit register encoding.
        private static Mutator<MicroMipsDisassembler> R(int bitpos)
        {
            var field = new Bitfield(bitpos, 5);
            return (u, d) =>
            {
                var iReg = (int)field.Read(u);
                d.ops.Add(new RegisterOperand(d.arch.GetRegister(iReg)));
                return true;
            };
        }
        private static readonly Mutator<MicroMipsDisassembler> R16 = R(16);
        private static readonly Mutator<MicroMipsDisassembler> R21 = R(21);

        // r - 3-bit register encoding.
        private static Mutator<MicroMipsDisassembler> r(int bitpos, int [] encoding)
        {
            var field = new Bitfield(bitpos, 3);
            return (u, d) =>
            {
                var iReg = field.Read(u);
                var iReg32 = encoding[iReg];
                d.ops.Add(new RegisterOperand(d.arch.GetRegister(iReg32)));
                return true;
            };
        }

        private static readonly Mutator<MicroMipsDisassembler> r7 = r(7, threeBitRegisterEncodings);
        private static readonly Mutator<MicroMipsDisassembler> rz7 = r(7, threeBitRegisterEncodingsWithZero);

        // F - 5-bit floating point register encoding.
        private static Mutator<MicroMipsDisassembler> F(int bitpos)
        {
            var field = new Bitfield(bitpos, 5);
            return (u, d) =>
            {
                var iReg = (int) field.Read(u);
                d.ops.Add(new RegisterOperand(d.arch.fpuRegs[iReg]));
                return true;
            };
        }
        private static readonly Mutator<MicroMipsDisassembler> F21 = F(21);


        // SI - signed immediate
        private static Mutator<MicroMipsDisassembler> SI(int bitPos, int length)
        {
            var field = new Bitfield(bitPos, length);
            return (u, d) =>
            {
                var i = field.ReadSigned(u);
                var imm = new ImmediateOperand(Constant.Create(d.arch.WordWidth, i));
                d.ops.Add(imm);
                return true;
            };
        }
        private static readonly Mutator<MicroMipsDisassembler> SI16 = SI(0, 16);

        // UI - unsigned immediate
        private static Mutator<MicroMipsDisassembler> UI(int bitPos, int length)
        {
            var field = new Bitfield(bitPos, length);
            return (u, d) =>
            {
                var i = field.Read(u);
                var imm = new ImmediateOperand(Constant.Create(d.arch.WordWidth, i));
                d.ops.Add(imm);
                return true;
            };
        }
        private static readonly Mutator<MicroMipsDisassembler> UI16 = UI(0, 16);

        // UIs - unsigned immediate, then shifted left.
        private static Mutator<MicroMipsDisassembler> UIs(int bitPos, int length, int shift)
        {
            var field = new Bitfield(bitPos, length);
            return (u, d) =>
            {
                var i = field.Read(u) << shift;
                var imm = new ImmediateOperand(Constant.Create(d.arch.WordWidth, i));
                d.ops.Add(imm);
                return true;
            };
        }
        private static readonly Mutator<MicroMipsDisassembler> UIs5_sh2 = UIs(5, 5, 2);

        // pcRel - short PC-relative
        private static Mutator<MicroMipsDisassembler> pcRel(int bitLength)
        {
            var field = new Bitfield(0, bitLength);
            return (u, d) =>
            {
                var offset = field.ReadSigned(u);
                offset <<= 1;
                var addrDst = d.rdr.Address + offset;
                d.ops.Add(AddressOperand.Create(addrDst));
                return true;
            };
        }
        private static readonly Mutator<MicroMipsDisassembler> pcRel7 = pcRel(7);
        private static readonly Mutator<MicroMipsDisassembler> pcRel10 = pcRel(10);
        private static readonly Mutator<MicroMipsDisassembler> pcRel26 = pcRel(26);

        // Ms - memory access: base + offset with 5-bit register encodings; offset scaled
        private static Bitfield baseField = new Bitfield(16, 5);
        private static Bitfield offsetField = new Bitfield(0, 16);

        private static Mutator<MicroMipsDisassembler> Ms(PrimitiveType dt)
        {
            return (u, d) =>
            {
                var iBase = (int) baseField.Read(u);
                var baseReg =  d.arch.GetRegister(iBase);

                var offset = offsetField.ReadSigned(u) * dt.Size;
                var mop = new IndirectOperand(dt, offset, baseReg);
                d.ops.Add(mop);
                return true;
            };
        }
        private static readonly Mutator<MicroMipsDisassembler> Mb = Ms(PrimitiveType.Byte);
        private static readonly Mutator<MicroMipsDisassembler> Mh = Ms(PrimitiveType.Word16);

        // M - memory access: base + offset with 5-bit register encodings

        private static Mutator<MicroMipsDisassembler> M(PrimitiveType dt)
        {
            return (u, d) =>
            {
                var iBase = (int) baseField.Read(u);
                var baseReg = d.arch.GetRegister(iBase);
                var offset = offsetField.ReadSigned(u);
                var mop = new IndirectOperand(dt, offset, baseReg);
                d.ops.Add(mop);
                return true;
            };
        }
        private static readonly Mutator<MicroMipsDisassembler> Mw = M(PrimitiveType.Word32);
        private static readonly Mutator<MicroMipsDisassembler> Mq = M(PrimitiveType.Word64);
        private static readonly Mutator<MicroMipsDisassembler> Md = M(PrimitiveType.Real64);

        private static readonly Bitfield baseField16 = new Bitfield(4, 3);
        private static readonly Bitfield offsetField16 = new Bitfield(0, 4);

        // m - memory access: base + offset with 3-bit register encoding
        private static bool mb(uint uInstr, MicroMipsDisassembler dasm)
        {
            var encodedBase = baseField16.Read(uInstr);
            var iBase = threeBitRegisterEncodings[encodedBase];
            var baseReg = dasm.arch.GetRegister(iBase);
            var encOffset = offsetField16.Read(uInstr);
            var offset = encodedByteOffsets[encOffset];
            var mop = new IndirectOperand(PrimitiveType.Byte, offset, baseReg);
            dasm.ops.Add(mop);
            return true;
        }

        private static Mutator<MicroMipsDisassembler> m(PrimitiveType dt)
        {
            var baseField = new Bitfield(4, 3);
            var offsetField = new Bitfield(0, 4);
            return (u, d) =>
            {
                var iBase = baseField.Read(u);
                var iBase32 = threeBitRegisterEncodings[iBase];
                var baseReg = d.arch.GetRegister(iBase32);
                var offset = offsetField.ReadSigned(u) * dt.Size;
                var mop = new IndirectOperand(dt, offset, baseReg);
                d.ops.Add(mop);
                return true;
            };
        }
        private static readonly Mutator<MicroMipsDisassembler> mw = m(PrimitiveType.Word32);

        private static bool Is64Bit(uint uInstr, MicroMipsDisassembler dasm)
        {
            return dasm.arch.WordWidth.BitSize == 64;
        }

        static MicroMipsDisassembler()
        {
            var invalid = Instr(Mnemonic.illegal, InstrClass.Invalid);

            var pool16a = Nyi("pool16a");
            var pool16b = Nyi("pool16b");
            var pool16c = Mask(0, 6, "POOL16c",
                Nyi("not16"),
                Nyi("and16"),
                Nyi("lwm16"),
                Nyi("jrc16"),
                Nyi("movep"),
                Nyi("movep"),
                Nyi("movep"),
                Nyi("movep"),

                Nyi("xor16"),
                Nyi("or16"),
                Nyi("swm16"),
                Nyi("jalrc16"),
                Nyi("movep"),
                Nyi("movep"),
                Nyi("movep"),
                Nyi("movep"),
                
                // 10
                Nyi("not16"),
                Nyi("and16"),
                Nyi("lwm16"),
                Instr(Mnemonic.jrcaddiusp, UIs5_sh2),
                Nyi("movep"),
                Nyi("movep"),
                Nyi("movep"),
                Nyi("movep"),

                Nyi("xor16"),
                Nyi("or16"),
                Nyi("swm16"),
                Nyi("break16"),
                Nyi("movep"),
                Nyi("movep"),
                Nyi("movep"),
                Nyi("movep"),

                // 20
                Nyi("not16"),
                Nyi("and16"),
                Nyi("lwm16"),
                Nyi("jrc16"),
                Nyi("movep"),
                Nyi("movep"),
                Nyi("movep"),
                Nyi("movep"),

                Nyi("xor16"),
                Nyi("or16"),
                Nyi("swm16"),
                Nyi("jalrc16"),
                Nyi("movep"),
                Nyi("movep"),
                Nyi("movep"),
                Nyi("movep"),

                // 30
                Nyi("not16"),
                Nyi("and16"),
                Nyi("lwm16"),
                Instr(Mnemonic.jrcaddiusp, UIs5_sh2),
                Nyi("movep"),
                Nyi("movep"),
                Nyi("movep"),
                Nyi("movep"),

                Nyi("xor16"),
                Nyi("or16"),
                Nyi("swm16"),
                Nyi("sdbbp16"),
                Nyi("movep"),
                Nyi("movep"),
                Nyi("movep"),
                Nyi("movep"));

            var pool16d = Nyi("pool16d");

            var pool16e = Mask(0, 1,
                Nyi("addiur2"),
                Instr(Mnemonic.addiur1sp, r7,UIs(1, 6, 2)));

            var pool16f = Mask(0, 1,
                invalid,
                invalid);

            var pool32a = Nyi("pool32a");

            var pool32b = Mask(15, 4, "POOL32B",
                Nyi("lwc2"),
                Nyi("lwp"),
                Nyi("ldc2"),
                invalid,

                Nyi("ldp"),
                Nyi("lwm32"),
                Nyi("cache"),
                Nyi("ldm"),

                Nyi("swc2"),
                Nyi("swp"),
                Nyi("sdc2"),
                invalid,

                Nyi("sdp"),
                Nyi("swm32"),
                invalid,
                Nyi("sdm"));

            var pool32c = Nyi("pool32c");
            var pool32f = Nyi("pool32f");

            var pool32i = Mask(21, 5, "POOL32i",
                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                Nyi("bc1eqz"),
                Nyi("bc1nezc"),
                Nyi("bc2eqzc"),
                Nyi("bc2nezc"),

                Nyi("synci"),
                invalid,
                invalid,
                invalid,

                Nyi("dati"),
                Nyi("dahi"),
                Nyi("bnz.v"),
                Nyi("bz.v"),

                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid);

            var pool32s = Nyi("pool32s");

            rootDecoder = Mask(10, 6,
                pool32a,
                pool16a,
                Instr(Mnemonic.lbu16, r7, mb),
                Nyi("move16 "),

                Low16(Instr(Mnemonic.aui, R21, R16, SI16)),
                Low16(Instr(Mnemonic.lbu32, R21, Mb)),
                Low16(Instr(Mnemonic.sb32, R21, Mb)),
                Nyi("lb32"),

                // 
                Low16(pool32b),
                pool16b,
                Nyi("lhu16"),
                Nyi("andi16"),

                Nyi("addiu32"),
                Nyi("lhu32"),
                Nyi("sh32"),
                Nyi("lh32"),

                // 10
                Low16(pool32i),
                pool16c,
                Nyi("lwsp16"),
                pool16d,

                Low16(Instr(Mnemonic.ori32, R21, R16, UI16)),
                pool32f,
                pool32s,
                Nyi("daddiu32"),

                //
                pool32c,
                Nyi("lwgp"),
                Nyi("lw16"),
                pool16e,

                Low16(Instr(Mnemonic.xori32, R21, R16, UI16)),
                Nyi("bovc /beqzalc /beqc"),
                Nyi("addiupc /auipc / aluipc / ldpc /lwpc / lwupc"),
                Nyi("bnvc /bnezalc /bnec"),

                // 20
                Nyi("beqzc/jic"),
                pool16f,
                Nyi("sb16"),
                Nyi("beqzc16"),

                Nyi("slti32"),
                Low16(Instr(Mnemonic.bc, InstrClass.Transfer, pcRel26)),
                Nyi("swc132"),
                Nyi("lwc132"),

                //
                Instr(Mnemonic.bnezc16, InstrClass.ConditionalTransfer, r7, pcRel7),
                invalid,
                Instr(Mnemonic.sh16, rz7, Mh),
                Nyi("bnezc16"),

                Nyi("sltiu32"),
                Nyi("balc"),
                Low16(Instr(Mnemonic.sdc132,F21,Md)),
                Low16(Instr(Mnemonic.ldc132,F21,Md)),

                // 30

                Nyi("blezalc/bgezalc/bgeuc"),
                invalid,
                Nyi("swsp16"),
                Instr(Mnemonic.bc16, InstrClass.Transfer, pcRel10),

                Low16(Instr(Mnemonic.andi32, R21,R16,UI16)),
                Nyi("bgtzc /bltzc /bltc"),
                Low16(Instr(Mnemonic.sd32, Is64Bit, R21,Mq)),
                Low16(Instr(Mnemonic.ld32, Is64Bit, R21,Mq)),

                Nyi("bgtzalc/bltzalc/bltuc"),
                invalid,
                Instr(Mnemonic.sw16, rz7, mw),
                Nyi("li16"),

                Nyi("daui"),
                Nyi("blezc /bgezc /bgec"),
                Low16(Instr(Mnemonic.sw32, R21,Mw)),
                Nyi("lw32"));
        }
    }
}
