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
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Mips
{
    using Decoder = Reko.Core.Machine.Decoder<NanoMipsDisassembler, Opcode, MipsInstruction>;

    /// <summary>
    /// Disassembler for the nanoMips instruction set encoding.
    /// </summary>
    public class NanoMipsDisassembler : DisassemblerBase<MipsInstruction>
    {
        private static readonly Decoder rootDecoder;

        private readonly MipsProcessorArchitecture arch;
        private readonly EndianImageReader rdr;
        private readonly List<MachineOperand> ops;
        private Address addr;

        public NanoMipsDisassembler(MipsProcessorArchitecture arch, EndianImageReader rdr)
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
            this.ops.Clear();
            var instr = rootDecoder.Decode(uInstr, this);
            if (uInstr == 0)
                instr.InstructionClass |= InstrClass.Zero;
            instr.Address = addr;
            instr.Length = (int) (rdr.Address - addr);
            return instr;
        }

        protected override MipsInstruction CreateInvalidInstruction()
        {
            return new MipsInstruction
            {
                InstructionClass = InstrClass.Invalid,
                opcode = Opcode.illegal
            };
        }

        public override MipsInstruction NotYetImplemented(uint wInstr, string message)
        {
            var hex = $"{wInstr:X8}";
            base.EmitUnitTest("nanoMips", hex, message, "NanoMipsDis", this.addr, w =>
            {
                w.WriteLine("           AssertCode(\"@@@\", \"{0}\");", hex);
            });
            return base.NotYetImplemented(wInstr, message);
        }

        #region Mutators

        // R: 5-bit register identifier
        private static Mutator<NanoMipsDisassembler> R(int pos)
        {
            var field = new Bitfield(pos, 5);
            return (u, d) =>
            {
                var iReg = (int) field.Read(u);
                var reg = d.arch.GetRegister(iReg);
                d.ops.Add(new RegisterOperand(reg));
                return true;
            };
        }
        private static readonly Mutator<NanoMipsDisassembler> R0 = R(0);
        private static readonly Mutator<NanoMipsDisassembler> R5 = R(5);
        private static readonly Mutator<NanoMipsDisassembler> R11 = R(11);
        private static readonly Mutator<NanoMipsDisassembler> R16 = R(16);
        private static readonly Mutator<NanoMipsDisassembler> R21 = R(21);

        // Rw: 5-bit register identifier from a wide instruction
        private static WideMutator<NanoMipsDisassembler> Rw(int pos)
        {
            var field = new Bitfield(pos, 5);
            return (u, d) =>
            {
                var iReg = (int) field.Read(u);
                var reg = d.arch.GetRegister(iReg);
                d.ops.Add(new RegisterOperand(reg));
                return true;
            };
        }
        private static readonly WideMutator<NanoMipsDisassembler> R37w = Rw(37);


        private static readonly int[] gpr3_encoding = new[] { 16, 17, 18, 19, 4, 5, 6, 7 };
        private static readonly int[] gpr3_store_encoding = new[] { 0, 17, 18, 19, 4, 5, 6, 7 };
        private static readonly int[] gpr1 = new[] { 4, 5 };
        private static readonly int[] gpr2_reg1_encoding = new[] { 4, 5, 6, 7 };
        private static readonly int[] gpr2_reg2_encoding = new[] { 5, 6, 7, 8 };
        private static readonly int[] gpr4_encoding = new[] { 8, 9, 10, 11, 4, 5, 6, 7, 16, 17, 18, 19, 20, 21, 22, 23 };
        private static readonly int[] gpr4_zero_encoding = new[] { 8, 9, 10, 0, 4, 5, 6, 7, 16, 17, 18, 19, 20, 21, 22, 23 };
        private static readonly int[] gpr1_save_restore = new[] { 30, 31 };

        // r: encoded register identifier
        private static Mutator<NanoMipsDisassembler> r(int pos, int length, int [] gprEncoding)
        {
            var field = new Bitfield(pos, length);
            return (u, d) =>
            {
                var iEncodedReg = (int) field.Read(u);
                var iReg = gprEncoding[iEncodedReg];
                var reg = d.arch.GetRegister(iReg);
                d.ops.Add(new RegisterOperand(reg));
                return true;
            };
        }
        private static readonly Mutator<NanoMipsDisassembler> r1 = r(1, 3, gpr3_encoding);
        private static readonly Mutator<NanoMipsDisassembler> r4 = r(4, 3, gpr3_encoding);
        private static readonly Mutator<NanoMipsDisassembler> r7 = r(7, 3, gpr3_encoding);

        // rf: encoded register identifier in multiple fields.
        private static Mutator<NanoMipsDisassembler> rf(Bitfield[] fields, int[] gprEncoding)
        {
            return (u, d) =>
            {
                var iEncodedReg = (int) Bitfield.ReadFields(fields, u);
                var iReg = gprEncoding[iEncodedReg];
                var reg = d.arch.GetRegister(iReg);
                d.ops.Add(new RegisterOperand(reg));
                return true;
            };
        }
        // register fields in 4x4 instructions.
        private static readonly Mutator<NanoMipsDisassembler> rf0 = rf(Bf((4, 1), (0, 3)), gpr4_encoding);
        private static readonly Mutator<NanoMipsDisassembler> rf5 = rf(Bf((9, 1), (5, 3)), gpr4_encoding);

        // U: signed immediate
        private static Mutator<NanoMipsDisassembler> I(int pos, int len)
        {
            var field = new Bitfield(pos, len);
            return (u, d) =>
            {
                var uVal = field.ReadSigned(u);
                d.ops.Add(new ImmediateOperand(Constant.Create(d.arch.WordWidth, uVal)));
                return true;
            };
        }

        // U: unsigned immediate
        private static Mutator<NanoMipsDisassembler> U(int pos, int len)
        {
            var field = new Bitfield(pos, len);
            return (u, d) =>
            {
                var uVal = field.Read(u);
                d.ops.Add(new ImmediateOperand(Constant.Create(d.arch.WordWidth, uVal)));
                return true;
            };
        }

        // Uf_w: unsigned immediate, in fields
        private static WideMutator<NanoMipsDisassembler> Uf_w(Bitfield[] fields)
        {
            return (u, d) =>
            {
                var uVal = Bitfield.ReadFields(fields, (uint)u);
                d.ops.Add(new ImmediateOperand(Constant.Create(d.arch.WordWidth, uVal)));
                return true;
            };
        }

        // Um: unsigned immediate, negated
        private static Mutator<NanoMipsDisassembler> Un(int pos, int len)
        {
            var field = new Bitfield(pos, len);
            return (u, d) =>
            {
                var uVal = 0u - field.Read(u);
                d.ops.Add(new ImmediateOperand(Constant.Create(d.arch.WordWidth, uVal)));
                return true;
            };
        }

        // Up1: unsigned integer, add 1
        private static Mutator<NanoMipsDisassembler> Up1(int pos, int len)
        {
            var field = new Bitfield(pos, len);
            return (u, d) =>
            {
                var uVal = field.Read(u) + 1;
                d.ops.Add(new ImmediateOperand(Constant.Create(d.arch.WordWidth, uVal)));
                return true;
            };
        }
        private static Mutator<NanoMipsDisassembler> UinsSize()
        {
            var lsbField = new Bitfield(0, 5);
            var msbdField = new Bitfield(6, 5);
            return (u, d) =>
            {
                var lsb = lsbField.Read(u);
                var msbd = lsbField.Read(u);
                var size = 1 + msbd - lsb;
                d.ops.Add(new ImmediateOperand(Constant.Create(d.arch.WordWidth, size)));
                return true;
            };
        }

        // Us: unsigned immediate, shifted
        private static Mutator<NanoMipsDisassembler> Us(int pos, int len, int shift)
        {
            var field = new Bitfield(pos, len);
            return (u, d) =>
            {
                var uVal = field.Read(u);
                uVal <<= shift;
                d.ops.Add(new ImmediateOperand(Constant.Create(d.arch.WordWidth, uVal)));
                return true;
            };
        }

        // eu: encoded unsigned
        private static Mutator<NanoMipsDisassembler> eu()
        {
            var encField = new Bitfield(0, 7);
            return (u, d) =>
            {
                var uVal = encField.Read(u);
                if (uVal == 0x7F)
                    uVal = ~0u;
                d.ops.Add(new ImmediateOperand(Constant.Create(d.arch.WordWidth, uVal)));
                return true;
            };
        }

        // sf: signed integer extracted from fields.
        private static Mutator<NanoMipsDisassembler> sf(Bitfield[] fields)
        {
            return (u, d) =>
            {
                var sVal = Bitfield.ReadSignedFields(fields, u);
                d.ops.Add(new ImmediateOperand(Constant.Create(d.arch.WordWidth, sVal)));
                return true;
            };
        }

        // Ie: integer immediate, encoded
        private static Mutator<NanoMipsDisassembler> Ie(PrimitiveType dt, int pos, int len, int [] encoding)
        {
            var field = new Bitfield(pos, len);
            return (u, d) =>
            {
                var encodedVal = field.Read(u);
                var val = encoding[encodedVal]; 
                d.ops.Add(new ImmediateOperand(Constant.Create(d.arch.WordWidth, val)));
                return true;
            };
        }
        // immediate shift amount encoded in the LSB's.
        private static readonly Mutator<NanoMipsDisassembler> Ish3 = Ie(PrimitiveType.Int32, 0, 3, new[] { 8, 1, 2, 3, 4, 5, 6, 7 });


        // ms: compact memory access with shifted offset.
        private static Mutator<NanoMipsDisassembler> ms(PrimitiveType dt, int length)
        {
            var baseField = new Bitfield(4, 3);
            var offsetField = new Bitfield(0, length);
            return (u, d) =>
            {
                var iEncodedReg = (int) baseField.Read(u);
                var iReg = gpr3_encoding[iEncodedReg];
                var baseReg = d.arch.GetRegister(iReg);
                var offset = offsetField.Read(u) * dt.Size;
                d.ops.Add(new IndirectOperand(dt, (int) offset, baseReg));
                return true;
            };
        }
        private static readonly Mutator<NanoMipsDisassembler> msw = ms(PrimitiveType.Word32, 4);

        // Mspu: memory access - shifted unsigned offset from stack register
        private static Mutator<NanoMipsDisassembler> Mspu(PrimitiveType dt, int length)
        {
            var offsetField = new Bitfield(0, length);
            return (u, d) =>
            {
                var offset = (int) offsetField.Read(u);
                offset *= dt.Size;
                d.ops.Add(new IndirectOperand(dt, offset, d.arch.StackRegister));
                return true;
            };
        }
        private static readonly Mutator<NanoMipsDisassembler> Mspu5w = Mspu(PrimitiveType.Word32, 5);

        // mx: compact memory access - indexed
        private static Mutator<NanoMipsDisassembler> mx(PrimitiveType dt)
        {
            var baseField = new Bitfield(7, 3);
            var ixField = new Bitfield(4, 3);
            return (u, d) =>
            {
                var encBase = (int) baseField.Read(u);
                var encIndex = (int) ixField.Read(u);
                var regBase = d.arch.GetRegister(gpr3_encoding[encBase]);
                var regIndex = d.arch.GetRegister(gpr3_encoding[encIndex]);
                d.ops.Add(new IndexedOperand(dt, regBase, regIndex));
                return true;
            };
        }
        private static readonly Mutator<NanoMipsDisassembler> mxw = mx(PrimitiveType.Word32);

        // pcRel: PC-relative displacement (used in branches / calls)
        private static Mutator<NanoMipsDisassembler> pcrel(Bitfield[] offsetfields)
        {
            return (u, d) =>
            {
                var offset = Bitfield.ReadSignedFields(offsetfields, u) << 1;
                var addr = d.rdr.Address + offset;
                d.ops.Add(AddressOperand.Create(addr));
                return true;
            };
        }
        private static readonly Mutator<NanoMipsDisassembler> pcrel_0_6 = pcrel(Bf((0, 1), (1, 6)));
        private static readonly Mutator<NanoMipsDisassembler> pcrel_0_9 = pcrel(Bf((0, 1), (1, 9)));
        private static readonly Mutator<NanoMipsDisassembler> pcrel_0_10 = pcrel(Bf((0, 1), (1, 10)));
        private static readonly Mutator<NanoMipsDisassembler> pcrel_0_13 = pcrel(Bf((0, 1), (1, 13)));
        private static readonly Mutator<NanoMipsDisassembler> pcrel_0_24 = pcrel(Bf((0, 1), (1, 24)));

        private static Mutator<NanoMipsDisassembler> Aluipc()
        {
            Bitfield[] aluipcFields = Bf((0, 1), (2, 10), (12, 9));
            return (u, d) =>
            {
                var s = (uint) Bitfield.ReadSignedFields(aluipcFields, u);
                var uAddr = d.rdr.Address.ToUInt32() + (ulong)s;
                uAddr >>= 12;
                d.ops.Add(new ImmediateOperand(Constant.Create(d.arch.WordWidth, uAddr)));
                return true;
            };
        }

    #endregion

    #region Predicates
    private static bool Eq0(uint u)
        {
            return u == 0;
        }
        #endregion

        #region Decoder subclasses

        private class InstrDecoder : Decoder<NanoMipsDisassembler, Opcode, MipsInstruction>
        {
            private readonly InstrClass iclass;
            private readonly Opcode mnemonic;
            private readonly Mutator<NanoMipsDisassembler>[] mutators;

            public InstrDecoder(InstrClass iclass, Opcode mnemonic, Mutator<NanoMipsDisassembler>[] mutators)
            {
                this.iclass = iclass;
                this.mnemonic = mnemonic;
                this.mutators = mutators;
            }

            public override MipsInstruction Decode(uint wInstr, NanoMipsDisassembler dasm)
            {
                foreach (var m in mutators)
                {
                    if (!m(wInstr, dasm))
                        return dasm.CreateInvalidInstruction();
                }
                //$REVIEW: this is shared code btw InstrDecoder and WideInstrDecoder
                var instr = new MipsInstruction
                {
                    InstructionClass = this.iclass,
                    opcode = this.mnemonic,
                    op1 = dasm.ops.Count > 0 ? dasm.ops[0] : null,
                    op2 = dasm.ops.Count > 1 ? dasm.ops[1] : null,
                    op3 = dasm.ops.Count > 2 ? dasm.ops[2] : null,
                    op4 = dasm.ops.Count > 3 ? dasm.ops[3] : null,
                };
                return instr;
            }
        }

        /// <summary>
        /// This decoder is used to read the second 16-bit chunk of a 32-bit instruction;
        /// the previously read bits are then concatenated with the read chunk and 
        /// control of the newly minted 32-bit instruction is handed to the inner decoder.
        /// </summary>
        private class Read32Decoder : Decoder
        {
            private readonly Decoder decoder;

            public Read32Decoder(Decoder decoder)
            {
                this.decoder = decoder;
            }

            public override MipsInstruction Decode(uint wInstr, NanoMipsDisassembler dasm)
            {
                if (!dasm.rdr.TryReadUInt16(out ushort uLow16bits))
                    return dasm.CreateInvalidInstruction();
                var uInstr32 = (wInstr << 16) | uLow16bits;
                return decoder.Decode(uInstr32, dasm);
            }
        }

        /// <summary>
        /// This decoder is used to read an additional 16-bit chunk and append it at the
        /// LSB end of a 32-bit instruction fragment, making it 48-bits. This newly minted
        /// 48-bit instruction wide is then passed in a 64-bit uint to a <see cref="WideDecoder"/>
        /// instance.
        /// </summary>
        private class Read48Decoder : Decoder
        {
            private readonly WideDecoder decoder;

            public Read48Decoder(WideDecoder decoder)
            {
                this.decoder = decoder;
            }

            public override MipsInstruction Decode(uint wInstr, NanoMipsDisassembler dasm)
            {
                if (!dasm.rdr.TryReadUInt16(out ushort uLowBits))
                    return dasm.CreateInvalidInstruction();
                ulong ulInstr = wInstr;
                ulInstr = (ulInstr << 16) | uLowBits;
                return this.decoder.Decode(ulInstr, dasm);
            }
        }

        //$REVIEW: consider moving the 'wide' classes and delegates to Reko.Core for other architectures like Risc-V
        public delegate bool WideMutator<TDasm>(ulong ulInstr, TDasm dasm);

        private abstract class WideDecoder : Decoder
        {
            public override MipsInstruction Decode(uint wInstr, NanoMipsDisassembler dasm)
            {
                throw new InvalidOperationException("32-bit decoding is not allowed with wide decoders.");
            }

            public abstract MipsInstruction Decode(ulong ulInstr, NanoMipsDisassembler dasm);
        }


        private class WideInstrDecoder : WideDecoder
        {
            private readonly InstrClass iclass;
            private readonly Opcode mnemonic;
            private readonly WideMutator<NanoMipsDisassembler>[] mutators;

            public WideInstrDecoder(InstrClass iclass, Opcode mnemonic, WideMutator<NanoMipsDisassembler>[] mutators)
            {
                this.iclass = iclass;
                this.mnemonic = mnemonic;
                this.mutators = mutators;
            }

            public override MipsInstruction Decode(ulong ulInstr, NanoMipsDisassembler dasm)
            {
                foreach (var m in mutators)
                {
                    if (!m(ulInstr, dasm))
                        return dasm.CreateInvalidInstruction();
                }
                //$REVIEW: this code is copied from InstrDecoder
                var instr = new MipsInstruction
                {
                    InstructionClass = this.iclass,
                    opcode = this.mnemonic,
                    op1 = dasm.ops.Count > 0 ? dasm.ops[0] : null,
                    op2 = dasm.ops.Count > 1 ? dasm.ops[1] : null,
                    op3 = dasm.ops.Count > 2 ? dasm.ops[2] : null,
                    op4 = dasm.ops.Count > 3 ? dasm.ops[3] : null,
                };
                return instr;
            }
        }
        #endregion

        #region Factory methods
        private static Decoder Instr(Opcode mnemonic,  params Mutator<NanoMipsDisassembler>[] mutators)
        {
            return new InstrDecoder(InstrClass.Linear, mnemonic, mutators);
        }

        private static Decoder Instr(Opcode mnemonic, InstrClass iclass, params Mutator<NanoMipsDisassembler> [] mutators)
        {
            return new InstrDecoder(iclass, mnemonic, mutators);
        }

        private static WideDecoder WInstr(Opcode mnemonic, params WideMutator<NanoMipsDisassembler>[] mutators)
        {
            return new WideInstrDecoder(InstrClass.Linear, mnemonic, mutators);
        }

        private static Decoder Nyi(string message)
        {
            return new NyiDecoder<NanoMipsDisassembler, Opcode, MipsInstruction>(message);
        }

        #endregion

        static NanoMipsDisassembler()
        {
            var invalid = Instr(Opcode.illegal, InstrClass.Invalid);

            var p16_ri = Nyi("P16.RI");

            var p16_mv = Select((5, 5), Eq0, "P16.MV",
                p16_ri,
                Instr(Opcode.move, R5, R0));

            var p16Pool = Mask(Bf((13, 3), (10, 2)), "P16 Pool",
                p16_mv,
                Instr(Opcode.lw, r7, msw),
                Instr(Opcode.bc, pcrel_0_9),
                Mask(8, 1, "P16.SR",
                    Instr(Opcode.save, Us(4, 4, 4), r(9, 1, gpr1_save_restore), U(0, 4)),
                    Instr(Opcode.restore_jrc, InstrClass.Transfer, Us(4, 4, 4), r(9, 1, gpr1_save_restore), U(0, 4))),

                Mask(3, 1, "P16.SHIFT",
                    Instr(Opcode.sll, r7, r4, Ish3),
                    Instr(Opcode.srl, r7, r4, Ish3)),
                Instr(Opcode.lw, r7, Mspu5w),
                Instr(Opcode.balc, pcrel_0_9),
                Mask(Bf((8, 1), (3, 1)), "P16.4x4",
                    Instr(Opcode.addu, rf5, rf5, rf0),
                    Instr(Opcode.mul, rf5, rf5, rf0),
                    invalid,
                    invalid),

                Mask(0, 2, "P16C",
                    Mask(2, 2, "POOL16C_00",
                        Instr(Opcode.not, r7, r4),
                        Instr(Opcode.xor, r7, r7, r4),
                        Instr(Opcode.and, r7, r7, r4),
                        Instr(Opcode.or, r7, r7, r4)),
                    Instr(Opcode.lwxs, r1, mxw),
                    invalid,
                    Instr(Opcode.lwxs, r1, mxw)),
                Nyi("lw[gp16]"),
                invalid,
                Nyi("p16.lb"),

                Nyi("p16.a1"),
                Nyi("lw[4x4]"),
                invalid,
                Nyi("p16.lh"),

                Mask(3, 1, "  P16.A2",
                    Nyi("ADDIU[R2]"),
                    Select((5, 5), Eq0, "  P.ADDIU[RS5]",
                        Instr(Opcode.nop),
                        Instr(Opcode.addiu, R5, R5, sf(Bf((4, 1), (0, 3)))))),
                Nyi("sw[16]"),
                Instr(Opcode.beqzc, r7, pcrel_0_6),
                invalid,

                Mask(0, 1, "  P16.ADDU",
                    Instr(Opcode.addu, r1, r4, r7),
                    Instr(Opcode.subu, r1, r4, r7)),
                Nyi("sw[sp]"),
                Instr(Opcode.bnezc, r7, pcrel_0_6),
                Instr(Opcode.movep,
                    rf(Bf((3, 1), (8, 1)), gpr2_reg1_encoding),
                    rf(Bf((3, 1), (8, 1)), gpr2_reg2_encoding),
                    rf(Bf((4, 1), (0, 3)), gpr4_zero_encoding),
                    rf(Bf((9, 1), (5, 3)), gpr4_zero_encoding)),

                Instr(Opcode.li, r7, eu()),
                Nyi("sw[gp16]"),
                Select((0, 4), Eq0, "P16.BR",
                    Mask(4, 1, "P16.JRC",
                        Instr(Opcode.jrc, InstrClass.Transfer, R5),
                        Nyi("jalrc")),
                    Nyi("P16.BR1")),
                invalid,

                Nyi("andi[16]"),
                Nyi("sw[4x4]"),
                invalid,
                Instr(Opcode.movep,
                    rf(Bf((4, 1), (0, 3)), gpr4_encoding),
                    rf(Bf((9, 1), (5, 3)), gpr4_encoding),
                    rf(Bf((3, 1), (8, 1)), gpr2_reg1_encoding),
                    rf(Bf((3, 1), (8, 1)), gpr2_reg2_encoding)));

            var pool32a0_0 = Mask(Bf((6, 4), (3, 2)), "  POOL32A0_0",
                Nyi("P.TRAP"),
                Nyi("SEB"),
                Instr(Opcode.sllv, R11,R16,R21), 
                Nyi("MUL[32]"),

                invalid,
                Nyi("SEH"),
                Instr(Opcode.srlv, R11,R16,R21), 
                Nyi("MUH"),

                invalid,
                invalid,
                Nyi("SRAV"),
                Nyi("MULU"),

                invalid,
                invalid,
                Nyi("ROTRV"),
                Nyi("MUHU"),

                invalid,
                invalid,
                Nyi("ADD"),
                Nyi("DIV"),

                invalid,
                invalid,
                Instr(Opcode.addu, R11,R16,R21),
                Nyi("MOD"),

                invalid,
                invalid,
                Nyi("SUB"),
                Nyi("DIVU"),

                Nyi("RDHWR"),
                invalid,
                Instr(Opcode.subu, R11,R16,R21),
                Nyi("MODU"),

                invalid,
                invalid,
                Mask(10, 1, "  P.CMOVE",
                    Instr(Opcode.movz, R11,R16,R21),
                    Nyi("MOVN")),
                invalid,

                invalid,
                invalid,
                Nyi("AND[32]"),
                invalid,

                invalid,
                invalid,
                Instr(Opcode.or, R11,R16,R21),
                invalid,
 
                invalid,
                invalid,
                Instr(Opcode.nor, R11,R16,R21),
                invalid,
 
                invalid,
                invalid,
                Nyi("XOR[32]"),
                invalid,
 
                invalid,
                invalid,
                Nyi("SLT"),
                invalid,

                invalid,
                invalid,
                Select((11, 5), Eq0, "  P.SLTU",
                    Nyi("P.DVP"),
                    Instr(Opcode.sltu, R11,R16,R21)),
                invalid,

                invalid,
                invalid,
                Nyi("SOV"),
                Nyi("invalid"));
            var pool32axf_4 = Sparse(9, 7, "Pool32Axf_4",
                invalid, // dsp
                (0b0100_101, Instr(Opcode.clo, R21,R16)),
                (0b0101_101, Instr(Opcode.clz, R21,R16)));
            
            var pool32axf = Mask(6, 3, "  POOL32Axf",
                invalid,
                invalid,    // DSP
                invalid,    // DSP
                invalid,

                pool32axf_4,
                Nyi("Pool32Axf_5"),
                invalid,
                invalid     // DSP
                );

            var p48i = Sparse(16, 5, "  P48I",
                invalid, // (MIPS64)
                (0, new Read48Decoder(WInstr(Opcode.li, R37w, Uf_w(Bf((0,16),(16,16)))))),
                (1, Nyi("ADDIU[48]")),
                (2, Nyi("ADDIU[GP48]")),
                (3, Nyi("ADDIUPC[48]")),
                (0b010_11, Nyi("LWPC[48]")),
                (0b011_11, Nyi("SWPC[48]")));

            var p32Pool = Mask(Bf((29, 3), (26, 2)), "P32 Pool",
                Select((0, 21), Eq0, "  P.ADDIU",
                    Mask(19, 2, "  P.RI",
                        Instr(Opcode.sigrie, InstrClass.Padding|InstrClass.Terminates, U(0, 19)),
                        Nyi("P.SYSCALL"),
                        Nyi("BREAK[32]"),
                        Nyi("SDBBP[32]")),
                    Instr(Opcode.addiu, R21, R16, U(0, 16))),
                Nyi("addiupc[32]"),
                Nyi("move.balc"),
                invalid,

                Mask(0, 3, "  P32A",
                    Mask(5, 1, "  _POOL32A0",
                        pool32a0_0,
                        Nyi("_POOL32A0_1")),
                    invalid, // (UDI)
                    invalid, // (CP2)
                    invalid, // (UDI)

                    invalid,
                    invalid, // (DSP)
                    invalid,
                    Mask(3, 3, "  _POOL32A7",
                        Nyi("P.LSX"),
                        Nyi("LSA"),
                        invalid,
                        Nyi("EXTW"),

                        invalid,
                        invalid,
                        invalid,
                        pool32axf)),
                invalid,
                Mask(25, 1, "  P.BAL",
                    Instr(Opcode.bc, pcrel_0_24),
                    Instr(Opcode.balc, pcrel_0_24)),
                invalid,

                Nyi("p.gp.w"),
                Nyi("p.gp.bh"),
                Nyi("p.j"),
                invalid,

                p48i,
                invalid,
                invalid,
                invalid,

                Mask(12, 4, "  P.U12",
                    Nyi("ORI "),
                    Instr(Opcode.xori, R21, R16, U(0, 12)),
                    Instr(Opcode.andi, R21, R16, U(0, 12)),
                    Nyi("P.SR"),

                    Instr(Opcode.slti, R21, R16, U(0, 12)),
                    Instr(Opcode.sltiu, R21, R16, U(0, 12)),
                    Nyi(" SEQI"),
                    invalid,

                    Instr(Opcode.addiu, R21, R16, Un(0, 12)),
                    invalid, // (MIPS64)
                    invalid, // (MIPS64) 
                    invalid,  //(MIPS64)

                    Mask(5, 4, "  P.SHIFT",
                        Select((21,5), Eq0, "  P.SLL",
                            Sparse(0, 5, invalid,
                                (0, Instr(Opcode.nop)),
                                (3, Nyi("  EHB")),
                                (5, Nyi("  PAUSE")),
                                (6, Nyi("  SYNC"))),
                            Instr(Opcode.sll, R21,R16, U(0,5))),
                        invalid,
                        Instr(Opcode.srl, R21,R16, U(0,5)),
                        invalid,

                        Nyi("SRA"),
                        invalid,
                        Nyi("ROTR"),
                        invalid,

                        invalid, // (MIPS64)
                        invalid, // (MIPS64)
                        invalid, // (MIPS64)
                        invalid, // (MIPS64)

                        invalid, // (MIPS64)
                        invalid, // (MIPS64)
                        invalid, // (MIPS64)
                        invalid  // (MIPS64)
                    ),
                    Nyi("P.ROTX "),
                    Mask(Bf((11,1),(5,1)), "  P.INS ",
                        Instr(Opcode.ins, R21,R16,U(0,4),UinsSize()), 
                        invalid, // (MIPS64)
                        invalid, // (MIPS64)
                        invalid), // (MIPS64)
                    Mask(Bf((11,1),(5,1)), "  P.EXT",
                        Instr(Opcode.ext,R21,R16,U(0,4),Up1(6, 5)),
                        invalid, // (MIPS64)
                        invalid, // (MIPS64)
                        invalid)), // (MIPS64)
                Nyi("p.ls.u12"),
                Mask(14, 2, "P.BR1",
                    Instr(Opcode.beqc, R16,R21,pcrel_0_13),
                    Nyi("P.BR3A"),
                    Instr(Opcode.bgec, R16,R21,pcrel_0_13),
                    Nyi("BGEUC")),
                invalid,

                Nyi("invalid(cp1)"),
                Nyi("p.ls.s9"),
                Mask(14, 2, "  P.BR2",
                    Instr(Opcode.bnec, R16,R21, pcrel_0_13),
                    invalid,
                    Instr(Opcode.bltc, R16,R21,pcrel_0_13),
                    Instr(Opcode.bltuc, R16,R21,pcrel_0_13)),
                invalid,

                Nyi("invalid(mips64)"),
                invalid,
                Mask(18, 3, "  P.BRI",
                    Instr(Opcode.beqic, R21, U(11,6), pcrel_0_10),
                    Instr(Opcode.bbeqzc, R21, U(11,6), pcrel_0_10),
                    Instr(Opcode.bgeic, R21, U(11,6), pcrel_0_10),
                    Instr(Opcode.bgeiuc, R21, U(11,6), pcrel_0_10),

                    Instr(Opcode.bneiuc, R21, U(11,6), pcrel_0_10),
                    Instr(Opcode.bbnezc, R21, U(11,6), pcrel_0_10),
                    Instr(Opcode.bltic, R21, U(11,6), pcrel_0_10),
                    Instr(Opcode.bltiuc, R21, U(11,6), pcrel_0_10)),
                invalid,
                
                Mask(1, 1, "  P.LUI",
                    Instr(Opcode.lui, R21, sf(Bf((0,1), (2,10), (12,9)))),
                    Instr(Opcode.aluipc, R21, Aluipc())),
                invalid,
                invalid,
                invalid);

            rootDecoder = Mask(12, 1,
                new Read32Decoder(p32Pool),
                p16Pool);
        }

    }
}
