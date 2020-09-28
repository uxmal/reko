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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Mips
{
    using Decoder = Decoder<NanoMipsDisassembler, Mnemonic, MipsInstruction>;
    using WideDecoder = WideDecoder<NanoMipsDisassembler, Mnemonic, MipsInstruction>;

    /// <summary>
    /// Disassembler for the nanoMips instruction set encoding.
    /// </summary>
    public class NanoMipsDisassembler : DisassemblerBase<MipsInstruction, Mnemonic>
    {
        private static readonly Decoder rootDecoder;

        private readonly MipsProcessorArchitecture arch;
        private readonly EndianImageReader rdr;
        private readonly RegisterStorage gp;
        private readonly List<MachineOperand> ops;
        private Address addr;

        public NanoMipsDisassembler(MipsProcessorArchitecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.ops = new List<MachineOperand>();
            this.gp = arch.GetRegister(28);
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

        public override MipsInstruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            //$REVIEW: this is shared code btw InstrDecoder and WideInstrDecoder
            var instr = new MipsInstruction
            {
                InstructionClass = iclass,
                Mnemonic = mnemonic,
                Operands = this.ops.ToArray()
            };
            return instr;
        }

        public override MipsInstruction CreateInvalidInstruction()
        {
            return new MipsInstruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.illegal,
                Operands = MachineInstruction.NoOperands
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

        // Rn: hard-wired register number.
        private static Mutator<NanoMipsDisassembler> Rn(int regNumber)
        {
            return (u, d) =>
            {
                var reg = d.arch.GetRegister(regNumber);
                d.ops.Add(new RegisterOperand(reg));
                return true;
            };
        }

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
        private static readonly int[] gpr1_encoding = new[] { 4, 5 };
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
        private static readonly Mutator<NanoMipsDisassembler> r7_st = r(7, 3, gpr3_store_encoding);

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

        // euLi: encoded unsigned for li[16]
        private static Mutator<NanoMipsDisassembler> euLi()
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

        // euAndi: encoded unsigned for andi[16]
        private static Mutator<NanoMipsDisassembler> euAndi()
        {
            var encField = new Bitfield(0, 4);
            return (u, d) =>
            {
                var uVal = encField.Read(u);
                if (uVal == 12)
                    uVal = 0x00FF;
                else if (uVal == 13)
                    uVal = 0xFFFF;
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
        // Sf_w: signed immediate extracted from fields
        private static WideMutator<NanoMipsDisassembler> Sf_w(Bitfield[] fields)
        {
            return (u, d) =>
            {
                var uVal = Bitfield.ReadSignedFields(fields, (uint) u);
                d.ops.Add(new ImmediateOperand(Constant.Create(d.arch.WordWidth, uVal)));
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
        private static readonly Mutator<NanoMipsDisassembler> Ilswm3 = Ie(PrimitiveType.Int32, 12, 3, new[] { 8, 1, 2, 3, 4, 5, 6, 7 });

        // ms: compact memory access with shifted offset.
        private static Mutator<NanoMipsDisassembler> ms(PrimitiveType dt,  int offset, int length)
        {
            var baseField = new Bitfield(4, 3);
            var offsetField = new Bitfield(offset, length);
            return (u, d) =>
            {
                var iEncodedReg = (int) baseField.Read(u);
                var iReg = gpr3_encoding[iEncodedReg];
                var baseReg = d.arch.GetRegister(iReg);
                var nOffset = offsetField.Read(u) * dt.Size;
                d.ops.Add(new IndirectOperand(dt, (int) nOffset, baseReg));
                return true;
            };
        }
        private static readonly Mutator<NanoMipsDisassembler> msw = ms(PrimitiveType.Word32, 0, 4);
        private static readonly Mutator<NanoMipsDisassembler> msh = ms(PrimitiveType.Int16, 1, 2);
        private static readonly Mutator<NanoMipsDisassembler> mshu = ms(PrimitiveType.Word16, 1, 2);
        private static readonly Mutator<NanoMipsDisassembler> msb = ms(PrimitiveType.SByte, 0, 2);
        private static readonly Mutator<NanoMipsDisassembler> msbu = ms(PrimitiveType.Byte, 0, 2);

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
        
        // Memory reference from GP register
        private static Mutator<NanoMipsDisassembler> Mgp(PrimitiveType dt, int pos, int len)
        {
            var offsetField = new Bitfield(pos, len);
            return (u, d) =>
            {
                var offset = (int) offsetField.Read(u);
                offset *= dt.Size;
                d.ops.Add(new IndirectOperand(dt, offset, d.gp));
                return true;
            };
        }
        private static readonly Mutator<NanoMipsDisassembler> Mgpw = Mgp(PrimitiveType.Word32, 2, 19);
        private static readonly Mutator<NanoMipsDisassembler> Mgph = Mgp(PrimitiveType.Int16, 1, 17);
        private static readonly Mutator<NanoMipsDisassembler> Mgphu = Mgp(PrimitiveType.Word16, 1, 17);
        private static readonly Mutator<NanoMipsDisassembler> Mgpb = Mgp(PrimitiveType.SByte, 0, 18);
        private static readonly Mutator<NanoMipsDisassembler> Mgpbu = Mgp(PrimitiveType.Byte, 0, 18);


        // mgp: compact memory reference from GP register
        private static Mutator<NanoMipsDisassembler> mgp(PrimitiveType dt, int pos, int len)
        {
            var offsetField = new Bitfield(pos, len);
            return (u, d) =>
            {
                var offset = (int) offsetField.Read(u);
                offset *= dt.Size;
                d.ops.Add(new IndirectOperand(dt, offset, d.gp));
                return true;
            };
        }
        private static readonly Mutator<NanoMipsDisassembler> mgpw = mgp(PrimitiveType.Word32, 0, 7);

        // Mu: memory access - unsigned offset
        private static Mutator<NanoMipsDisassembler> Mu(PrimitiveType dt, int posBase, int lenOffset)
        {
            var baseField = new Bitfield(posBase, 5);
            var offsetField = new Bitfield(0, lenOffset);
            return (u, d) =>
            {
                var iBase = (int) baseField.Read(u);
                var off = (int)offsetField.Read(u);
                var rBase = d.arch.GetRegister(iBase);
                d.ops.Add(new IndirectOperand(dt, off, rBase));
                return true;
            };
        }

        // Ms: memory access - signed offset
        private static Mutator<NanoMipsDisassembler> Ms(PrimitiveType dt, int posBase, Bitfield[] offsetFields)
        {
            var baseField = new Bitfield(posBase, 5);
            return (u, d) =>
            {
                var iBase = (int) baseField.Read(u);
                var off = Bitfield.ReadSignedFields(offsetFields, u);
                var rBase = d.arch.GetRegister(iBase);
                d.ops.Add(new IndirectOperand(dt, off, rBase));
                return true;
            };
        }
        private static readonly Mutator<NanoMipsDisassembler> Msw9 = Ms(PrimitiveType.Word32, 16, Bf((15, 1), (0, 8)));
        private static readonly Mutator<NanoMipsDisassembler> Msh9 = Ms(PrimitiveType.Int16, 16, Bf((15, 1), (0, 8)));
        private static readonly Mutator<NanoMipsDisassembler> Mshu9 = Ms(PrimitiveType.Word16, 16, Bf((15, 1), (0, 8)));
        private static readonly Mutator<NanoMipsDisassembler> Msb9 = Ms(PrimitiveType.SByte, 16, Bf((15, 1), (0, 8)));
        private static readonly Mutator<NanoMipsDisassembler> Msbu9 = Ms(PrimitiveType.Byte, 16, Bf((15, 1), (0, 8)));

        // Mx: memory access - indexed
        private static Mutator<NanoMipsDisassembler> Mx(PrimitiveType dt, int posBase, int posIdx)
        {
            var baseField = new Bitfield(posBase, 5);
            var idxField = new Bitfield(posIdx, 5);
            return (u, d) =>
            {
                var iBase = (int) baseField.Read(u);
                var iIndex = (int) idxField.Read(u);
                var rBase = d.arch.GetRegister(iBase);
                var rIndex = d.arch.GetRegister(iIndex);
                d.ops.Add(new IndexedOperand(dt, rBase, rIndex));
                return true;
            };
        }
        private static readonly Mutator<NanoMipsDisassembler> Mxbu = Mx(PrimitiveType.Byte, 21, 16);
        private static readonly Mutator<NanoMipsDisassembler> Mxh = Mx(PrimitiveType.Word16, 21, 16);
        private static readonly Mutator<NanoMipsDisassembler> Mxw = Mx(PrimitiveType.Word32, 21, 16);

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

        // pcRel: PC-relative signed displacement (used in branches / calls)
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
        private static readonly Mutator<NanoMipsDisassembler> pcrel_0_20 = pcrel(Bf((0, 1), (1, 20)));
        private static readonly Mutator<NanoMipsDisassembler> pcrel_0_24 = pcrel(Bf((0, 1), (1, 24)));

        // pcRel_w: PC-relative signed displacement (used in branches / calls) - wide version
        private static WideMutator<NanoMipsDisassembler> pcrel_w(Bitfield[] offsetfields)
        {
            return (u, d) =>
            {
                var offset = Bitfield.ReadSignedFields(offsetfields, u);
                var addr = d.rdr.Address + offset;
                d.ops.Add(AddressOperand.Create(addr));
                return true;
            };
        }

        // pcRelu: PC-relative unsigned displacement (used in branches / calls)
        private static Mutator<NanoMipsDisassembler> pcrelu(Bitfield[] offsetfields)
        {
            return (u, d) =>
            {
                var offset = Bitfield.ReadFields(offsetfields, u) << 1;
                var addr = d.rdr.Address + offset;
                d.ops.Add(AddressOperand.Create(addr));
                return true;
            };
        }
        private static readonly Mutator<NanoMipsDisassembler> pcrelu_0_4 = pcrelu(Bf((0, 4)));

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

        /// <summary>
        /// Mutator used for the LW[4x4] and SW[4x4] encodings.
        /// </summary>
        /// <returns></returns>
        private static Mutator<NanoMipsDisassembler> Mut4x4()
        {
            var rtFields = Bf((9, 1), (5, 3));
            var rsFields = Bf((4, 1), (0, 3));
            var offFields = Bf((3, 1), (8, 1));
            return (u, d) =>
            {
                var iRtCode = (int) Bitfield.ReadFields(rtFields, u);
                var iRsCode = (int) Bitfield.ReadFields(rsFields, u);
                var offset = (int) Bitfield.ReadFields(offFields, u);
                var rt = d.arch.GetRegister(gpr4_encoding[iRtCode]);
                var rs = d.arch.GetRegister(gpr4_encoding[iRsCode]);
                d.ops.Add(new RegisterOperand(rt));
                d.ops.Add(new IndirectOperand(PrimitiveType.Word32, offset, rs));
                return true;
            };
        }

        #endregion

        #region Predicates
    private static bool Eq0(uint u)
        {
            return u == 0;
        }

        private static readonly Bitfield rs3Field = new Bitfield(4, 3);
        private static readonly Bitfield rt3Field = new Bitfield(7, 3);
        private static bool Rs3LtRt3(uint u)
        {
            var rs3 = rs3Field.Read(u);
            var rt3 = rt3Field.Read(u);
            return rs3 < rt3;
        }

        #endregion

        #region Decoder subclasses

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

        #endregion

        #region Factory methods

        private static Decoder Instr(Mnemonic mnemonic,  params Mutator<NanoMipsDisassembler>[] mutators)
        {
            return new InstrDecoder<NanoMipsDisassembler, Mnemonic, MipsInstruction>(InstrClass.Linear, mnemonic, mutators);
        }

        private static Decoder Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<NanoMipsDisassembler> [] mutators)
        {
            return new InstrDecoder<NanoMipsDisassembler, Mnemonic, MipsInstruction>(iclass, mnemonic, mutators);
        }

        private static WideDecoder WInstr(Mnemonic mnemonic, params WideMutator<NanoMipsDisassembler>[] mutators)
        {
            return new WideInstrDecoder<NanoMipsDisassembler, Mnemonic, MipsInstruction>(InstrClass.Linear, mnemonic, mutators);
        }

        private static Decoder Nyi(string message)
        {
            return new NyiDecoder<NanoMipsDisassembler, Mnemonic, MipsInstruction>(message);
        }

        #endregion

        static NanoMipsDisassembler()
        {
            var invalid = Instr(Mnemonic.illegal, InstrClass.Invalid);

            var p16_ri = Nyi("P16.RI");

            var p16_mv = Select((5, 5), Eq0, "P16.MV",
                p16_ri,
                Instr(Mnemonic.move, R5, R0));

            var p16Pool = Mask(Bf((13, 3), (10, 2)), "P16 Pool",
                p16_mv,
                Instr(Mnemonic.lw, r7, msw),
                Instr(Mnemonic.bc, InstrClass.Transfer, pcrel_0_9),
                Mask(8, 1, "P16.SR",
                    Instr(Mnemonic.save, Us(4, 4, 4), r(9, 1, gpr1_save_restore), U(0, 4)),
                    Instr(Mnemonic.restore_jrc, InstrClass.Transfer, Us(4, 4, 4), r(9, 1, gpr1_save_restore), U(0, 4))),

                Mask(3, 1, "P16.SHIFT",
                    Instr(Mnemonic.sll, r7, r4, Ish3),
                    Instr(Mnemonic.srl, r7, r4, Ish3)),
                Instr(Mnemonic.lw, r7, Mspu5w),
                Instr(Mnemonic.balc, InstrClass.Transfer | InstrClass.Call, pcrel_0_9),
                Mask(Bf((8, 1), (3, 1)), "P16.4x4",
                    Instr(Mnemonic.addu, rf5, rf5, rf0),
                    Instr(Mnemonic.mul, rf5, rf5, rf0),
                    invalid,
                    invalid),

                Mask(0, 2, "  P16C",
                    Mask(2, 2, "  POOL16C_00",
                        Instr(Mnemonic.not, r7, r4),
                        Instr(Mnemonic.xor, r7, r7, r4),
                        Instr(Mnemonic.and, r7, r7, r4),
                        Instr(Mnemonic.or, r7, r7, r4)),
                    Instr(Mnemonic.lwxs, r1, mxw),
                    invalid,
                    Instr(Mnemonic.lwxs, r1, mxw)),
                Instr(Mnemonic.lw, r7, mgpw),
                invalid,
                Mask(2, 2, "  P16.LB",
                    Instr(Mnemonic.lb, r7, msb),
                    Instr(Mnemonic.sb, r7_st, msbu),
                    Instr(Mnemonic.lbu, r7, msbu),
                    invalid),

                Mask(6, 1, "  P16.A1",
                    invalid,
                    Instr(Mnemonic.addiu, r7, Rn(29), Us(0, 6, 2))),
                Instr(Mnemonic.lw, Mut4x4()),
                invalid,
                Mask(Bf((3, 1), (1, 1)), "  P16.LH",
                    Instr(Mnemonic.lh, r7, msh),
                    Instr(Mnemonic.sh, r7_st, mshu),
                    Instr(Mnemonic.lhu, r7, mshu),
                    invalid),

                Mask(3, 1, "  P16.A2",
                    Instr(Mnemonic.addiu, r7, r4, Us(0, 3, 2)),
                    Select((5, 5), Eq0, "  P.ADDIU[RS5]",
                        Instr(Mnemonic.nop),
                        Instr(Mnemonic.addiu, R5, R5, sf(Bf((4, 1), (0, 3)))))),
                Instr(Mnemonic.sw, r7_st, Mspu5w),
                Instr(Mnemonic.beqzc, InstrClass.ConditionalTransfer, r7, pcrel_0_6),
                invalid,

                Mask(0, 1, "  P16.ADDU",
                    Instr(Mnemonic.addu, r1, r4, r7),
                    Instr(Mnemonic.subu, r1, r4, r7)),
                Instr(Mnemonic.sw, R5, Mspu5w),
                Instr(Mnemonic.bnezc, InstrClass.ConditionalTransfer, r7, pcrel_0_6),
                Instr(Mnemonic.movep,
                    rf(Bf((3, 1), (8, 1)), gpr2_reg1_encoding),
                    rf(Bf((3, 1), (8, 1)), gpr2_reg2_encoding),
                    rf(Bf((4, 1), (0, 3)), gpr4_zero_encoding),
                    rf(Bf((9, 1), (5, 3)), gpr4_zero_encoding)),

                Instr(Mnemonic.li, r7, euLi()),
                Instr(Mnemonic.sw, r7_st, mgpw),
                Select((0, 4), Eq0, "P16.BR",
                    Mask(4, 1, "P16.JRC",
                        Instr(Mnemonic.jrc, InstrClass.Transfer, R5),
                        Instr(Mnemonic.jalrc, InstrClass.Transfer|InstrClass.Call, Rn(31), R5)),
                    Select(Rs3LtRt3, //"  P16.BR1",
                        Instr(Mnemonic.beqc, r4,r7, pcrelu_0_4),
                        Instr(Mnemonic.bnec, r7,r4, pcrelu_0_4))),
                invalid,

                Instr(Mnemonic.andi, r7,r4, euAndi()),
                Instr(Mnemonic.sw, Mut4x4()),
                invalid,
                Instr(Mnemonic.movep,
                    rf(Bf((4, 1), (0, 3)), gpr4_encoding),
                    rf(Bf((9, 1), (5, 3)), gpr4_encoding),
                    rf(Bf((3, 1), (8, 1)), gpr2_reg1_encoding),
                    rf(Bf((3, 1), (8, 1)), gpr2_reg2_encoding)));

            var pool32a0_0 = Mask(Bf((6, 4), (3, 2)), "  POOL32A0_0",
                Nyi("P.TRAP"),
                Instr(Mnemonic.seb, R21,R16),
                Instr(Mnemonic.sllv, R11,R16,R21), 
                Instr(Mnemonic.mul, R11,R16,R21),

                invalid,
                Instr(Mnemonic.seh, R21,R16),
                Instr(Mnemonic.srlv, R11,R16,R21), 
                Nyi("MUH"),

                invalid,
                invalid,
                Instr(Mnemonic.srav, R11, R16, R21),
                Nyi("MULU"),

                invalid,
                invalid,
                Nyi("ROTRV"),
                Nyi("MUHU"),

                // 10
                invalid,
                invalid,
                Nyi("ADD"),
                Instr(Mnemonic.div, R11,R16,R21),

                invalid,
                invalid,
                Instr(Mnemonic.addu, R11,R16,R21),
                Nyi("MOD"),

                invalid,
                invalid,
                Nyi("SUB"),
                Nyi("DIVU"),

                Nyi("RDHWR"),
                invalid,
                Instr(Mnemonic.subu, R11,R16,R21),
                Instr(Mnemonic.modu, R11,R16,R21),

                // 20
                invalid,
                invalid,
                Mask(10, 1, "  P.CMOVE",
                    Instr(Mnemonic.movz, R11,R16,R21),
                    Instr(Mnemonic.movn, R11,R16,R21)),
                invalid,

                invalid,
                invalid,
                Instr(Mnemonic.and, R11,R16,R21),
                invalid,

                invalid,
                invalid,
                Instr(Mnemonic.or, R11,R16,R21),
                invalid,
 
                invalid,
                invalid,
                Instr(Mnemonic.nor, R11,R16,R21),
                invalid,
 
                // 30
                invalid,
                invalid,
                Instr(Mnemonic.xor, R11,R16,R21),
                invalid,
 
                invalid,
                invalid,
                Instr(Mnemonic.slt, R11,R16,R21),
                invalid,

                invalid,
                invalid,
                Select((11, 5), Eq0, "  P.SLTU",
                    Nyi("P.DVP"),
                    Instr(Mnemonic.sltu, R11,R16,R21)),
                invalid,

                invalid,
                invalid,
                Nyi("SOV"),
                Nyi("invalid"));
            var pool32axf_4 = Sparse(9, 7, "Pool32Axf_4",
                invalid, // dsp
                (0b0100_101, Instr(Mnemonic.clo, R21,R16)),
                (0b0101_101, Instr(Mnemonic.clz, R21,R16)));
            
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
                (0, new Read48Decoder(WInstr(Mnemonic.li, R37w, Uf_w(Bf((0,16),(16,16)))))),
                (1, Nyi("ADDIU[48]")),
                (2, Nyi("ADDIU[GP48]")),
                (3, new Read48Decoder(WInstr(Mnemonic.addiupc, R37w, Sf_w(Bf((0,16),(16,16)))))),
                (0b010_11, new Read48Decoder(WInstr(Mnemonic.lwpc, R37w, pcrel_w(Bf((0,16),(16,16)))))),
                (0b011_11, Nyi("SWPC[48]")));

            var p_lsx = Mask(6, 1, "  P.LSX",
                Mask(7, 4,
                    Nyi("LBX"),
                    Nyi("SBX"),
                    Instr(Mnemonic.lbux, R11, Mxbu),
                    invalid,

                    Nyi("LHX"),
                    Nyi("SHX"),
                    Nyi("LHUX"),
                    invalid, // (MIPS64)

                    Instr(Mnemonic.lwx, R11, Mxw),
                    Instr(Mnemonic.swx, R11, Mxw),
                    invalid,    // (CP1)
                    invalid,    // (CP1)

                    invalid,    // (MIPS64)
                    invalid,    // (MIPS64)
                    invalid,    // (CP1)
                    invalid),   // (CP1)

                Mask(7, 4, "  PP.LSXS",
                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    Nyi("LHXS"),
                    Nyi("SHXS"),
                    Nyi("LHUXS"),
                    invalid,    // (MIPS64)

                    Instr(Mnemonic.lwxs,R11,Mxw),
                    Instr(Mnemonic.swxs,R11,Mxw),
                    invalid,    // (CP1)
                    invalid,    // (CP1)

                    invalid,    // (MIPS64)
                    invalid,    // (MIPS64)
                    invalid,    // (CP1)
                    invalid));  // (CP1)

            var p_ls_s9 = Mask(8, 3, "  P.LS.S9",
                Mask(11, 4, "  P.LS.S0",
                    Instr(Mnemonic.lb, R21, Msb9),
                    Instr(Mnemonic.sb, R21, Msb9),
                    Instr(Mnemonic.lbu, R21, Msbu9),
                    Nyi("P.PREF[s9]"),

                    Instr(Mnemonic.lh, R21, Msh9),
                    Instr(Mnemonic.sh, R21, Msh9),
                    Instr(Mnemonic.lhu, R21, Mshu9),
                    invalid,    // (MIPS64)

                    Instr(Mnemonic.lw, R21, Msw9),
                    Instr(Mnemonic.sw, R21, Msw9),
                    invalid,    // (CP1)
                    invalid,    // (CP1)

                    invalid,    // (MIPS64)
                    invalid,    // (MIPS64)
                    invalid,    // (CP1)
                    invalid),  // (CP1)
                Nyi("P.LS.S1"),
                Mask(11, 4, "  P.LS.E0",
                    Nyi("LBE"),
                    Nyi("SBE"),
                    Nyi("LBUE"),
                    Nyi("P.PREFE"),

                    Instr(Mnemonic.lhe, R21,Ms(PrimitiveType.Int16, 16, Bf((15,1), (0, 8)))),
                    Nyi("SHE"),
                    Nyi("LHUE"),
                    Nyi("CACHEE"),

                    Instr(Mnemonic.lwe, R21, Ms(PrimitiveType.Word32, 16, Bf((15, 1), (0, 8)))),
                    Nyi("SWE"),
                    Nyi("P.LLE"),
                    Nyi("P.SCE"),

                    invalid,
                    invalid,
                    invalid,
                    invalid),
                invalid,

                Mask(11, 1, "  P.LS.WM",
                    Instr(Mnemonic.lwm, R21,Ms(PrimitiveType.Word32, 16, Bf((15,1),(0,8))), Ilswm3),
                    Instr(Mnemonic.swm, R21,Ms(PrimitiveType.Word32, 16, Bf((15,1),(0,8))), Ilswm3)),
                Mask(11, 1, "  P.LS.UAWM",
                    Instr(Mnemonic.ualwm, R21, Ms(PrimitiveType.Word32, 16, Bf((15, 1), (0, 8))), Ilswm3),
                    Instr(Mnemonic.uaswm, R21, Ms(PrimitiveType.Word32, 16, Bf((15, 1), (0, 8))), Ilswm3)),
                invalid,    // (MIPS64)
                invalid);   // (MIPS64)

            var p_br3a = Sparse(16, 5, "  P.BR3A",
                invalid,
                (0, invalid), // (CP1)
                (1, invalid), // (CP1)
                (2, invalid), // (CP2)
                (3, invalid), // (CP2)
                (4, invalid)); // (DSP)

            var p32Pool = Mask(Bf((29, 3), (26, 2)), "P32 Pool",
                Select((0, 21), Eq0, "  P.ADDIU",
                    Mask(19, 2, "  P.RI",
                        Instr(Mnemonic.sigrie, InstrClass.Padding|InstrClass.Terminates, U(0, 19)),
                        Nyi("P.SYSCALL"),
                        Nyi("BREAK[32]"),
                        Nyi("SDBBP[32]")),
                    Instr(Mnemonic.addiu, R21, R16, U(0, 16))),
                Instr(Mnemonic.addiupc, R21, sf(Bf((0,1),(1,20)))),
                Instr(Mnemonic.move_balc, InstrClass.Transfer|InstrClass.Call,
                    r(24,1,gpr1_encoding),
                    rf(Bf((25,1),(21,3)), gpr4_zero_encoding),
                    pcrel_0_20),
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
                        p_lsx,
                        Instr(Mnemonic.lsa, R11,R16,R21,U(9,2)),
                        invalid,
                        Nyi("EXTW"),

                        invalid,
                        invalid,
                        invalid,
                        pool32axf)),
                invalid,
                Mask(25, 1, "  P.BAL",
                    Instr(Mnemonic.bc, InstrClass.Transfer, pcrel_0_24),
                    Instr(Mnemonic.balc, InstrClass.Transfer|InstrClass.Call, pcrel_0_24)),
                invalid,

                Mask(0, 2, "  P.GP.W",
                    Instr(Mnemonic.addiu, R21, Rn(28), U(0, 21)),
                    invalid,    // (MIPS64)
                    Instr(Mnemonic.lw, R21, Mgpw),
                    Instr(Mnemonic.sw, R21, Mgpw)),
                Mask(18, 3, "  P.GP.BH",
                    Instr(Mnemonic.lb, R21, Mgpb),
                    Instr(Mnemonic.sb, R21, Mgpbu),
                    Instr(Mnemonic.lbu, R21, Mgpbu),
                    Nyi("ADDIU[GP.B]"),

                    Mask(0, 1, "  P.GP.LH",
                        Instr(Mnemonic.lh, R21, Mgph),
                        Instr(Mnemonic.lhu, R21, Mgphu)),
                    Mask(0, 1, "  P.GP.SH",
                        Instr(Mnemonic.sh, R21, Mgphu),
                        invalid),
                    invalid,    // (CP1)
                    invalid),   // (MIPS64)
                Sparse(12, 4, "  P.J",
                    invalid,
                    (0, Nyi("JALRC[32]")),
                    (1, Nyi("JALRC.HB")),
                    (8, Nyi("P.BALRSC"))),
                invalid,

                p48i,
                invalid,
                invalid,
                invalid,

                Mask(12, 4, "  P.U12",
                    Instr(Mnemonic.ori, R21, R16, U(0, 12)),
                    Instr(Mnemonic.xori, R21, R16, U(0, 12)),
                    Instr(Mnemonic.andi, R21, R16, U(0, 12)),
                    Mask(20, 1, "  P.SR",
                        Mask(0, 2, "  PP.SR",
                            Instr(Mnemonic.save,Us(3, 9, 3),R21,U(16,4)),
                            invalid,
                            Instr(Mnemonic.restore,Us(3, 9, 3),R21,U(16,4)),
                            Instr(Mnemonic.restore_jrc,Us(3, 9, 3),R21,U(16,4))),
                        invalid),   // (CR1)

                    Instr(Mnemonic.slti, R21, R16, U(0, 12)),
                    Instr(Mnemonic.sltiu, R21, R16, U(0, 12)),
                    Nyi(" SEQI"),
                    invalid,

                    Instr(Mnemonic.addiu, R21, R16, Un(0, 12)),
                    invalid, // (MIPS64)
                    invalid, // (MIPS64) 
                    invalid,  //(MIPS64)

                    Mask(5, 4, "  P.SHIFT",
                        Select((21,5), Eq0, "  P.SLL",
                            Sparse(0, 5, invalid,
                                (0, Instr(Mnemonic.nop)),
                                (3, Nyi("  EHB")),
                                (5, Nyi("  PAUSE")),
                                (6, Nyi("  SYNC"))),
                            Instr(Mnemonic.sll, R21,R16, U(0,5))),
                        invalid,
                        Instr(Mnemonic.srl, R21,R16, U(0,5)),
                        invalid,

                        Instr(Mnemonic.sra, R21,R16, U(0,5)),
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
                        Instr(Mnemonic.ins, R21,R16,U(0,4),UinsSize()), 
                        invalid, // (MIPS64)
                        invalid, // (MIPS64)
                        invalid), // (MIPS64)
                    Mask(Bf((11,1),(5,1)), "  P.EXT",
                        Instr(Mnemonic.ext,R21,R16,U(0,4),Up1(6, 5)),
                        invalid, // (MIPS64)
                        invalid, // (MIPS64)
                        invalid)), // (MIPS64)
                Mask(12, 4, "  P.LS.U12",
                    Instr(Mnemonic.lb, R21,Mu(PrimitiveType.Byte,16,12)),
                    Instr(Mnemonic.sb, R21,Mu(PrimitiveType.Byte,16,12)),
                    Instr(Mnemonic.lbu, R21,Mu(PrimitiveType.Byte,16,12)),
                    Nyi("P.PREF[U12]"),

                    Instr(Mnemonic.lh, R21, Mu(PrimitiveType.Byte, 16, 12)),
                    Instr(Mnemonic.sh, R21, Mu(PrimitiveType.Byte, 16, 12)),
                    Instr(Mnemonic.lhu, R21, Mu(PrimitiveType.Byte, 16, 12)),
                    invalid,    // (MIPS64)

                    Instr(Mnemonic.lw, R21, Mu(PrimitiveType.Byte, 16, 12)),
                    Instr(Mnemonic.sw, R21, Mu(PrimitiveType.Byte, 16, 12)),
                    invalid,    // (CP1)
                    invalid,    // (CP1)

                    invalid,    // (MIPS64)
                    invalid,    // (MIPS64)
                    invalid,    // (CP1)
                    invalid),    // (CP1)

                Mask(14, 2, "P.BR1",
                    Instr(Mnemonic.beqc, InstrClass.ConditionalTransfer, R16,R21,pcrel_0_13),
                    p_br3a,
                    Instr(Mnemonic.bgec, InstrClass.ConditionalTransfer, R16,R21,pcrel_0_13),
                    Instr(Mnemonic.bgeuc, InstrClass.ConditionalTransfer, R16,R21,pcrel_0_13)),
                invalid,

                invalid, // (CP1)
                p_ls_s9,
                Mask(14, 2, "  P.BR2",
                    Instr(Mnemonic.bnec, InstrClass.ConditionalTransfer, R16,R21, pcrel_0_13),
                    invalid,
                    Instr(Mnemonic.bltc, InstrClass.ConditionalTransfer, R16,R21,pcrel_0_13),
                    Instr(Mnemonic.bltuc, InstrClass.ConditionalTransfer, R16,R21,pcrel_0_13)),
                invalid,

                invalid, // (MIPS64)
                invalid,
                Mask(18, 3, "  P.BRI",
                    Instr(Mnemonic.beqic, InstrClass.ConditionalTransfer, R21, U(11,6), pcrel_0_10),
                    Instr(Mnemonic.bbeqzc, InstrClass.ConditionalTransfer, R21, U(11,6), pcrel_0_10),
                    Instr(Mnemonic.bgeic, InstrClass.ConditionalTransfer, R21, U(11,6), pcrel_0_10),
                    Instr(Mnemonic.bgeiuc, InstrClass.ConditionalTransfer, R21, U(11,6), pcrel_0_10),

                    Instr(Mnemonic.bneiuc, InstrClass.ConditionalTransfer, R21, U(11,6), pcrel_0_10),
                    Instr(Mnemonic.bbnezc, InstrClass.ConditionalTransfer, R21, U(11,6), pcrel_0_10),
                    Instr(Mnemonic.bltic, InstrClass.ConditionalTransfer, R21, U(11,6), pcrel_0_10),
                    Instr(Mnemonic.bltiuc, InstrClass.ConditionalTransfer, R21, U(11,6), pcrel_0_10)),
                invalid,
                
                Mask(1, 1, "  P.LUI",
                    Instr(Mnemonic.lui, R21, sf(Bf((0,1), (2,10), (12,9)))),
                    Instr(Mnemonic.aluipc, R21, Aluipc())),
                invalid,
                invalid,
                invalid);

            rootDecoder = Mask(12, 1, "nanoMips",
                new Read32Decoder(p32Pool),
                p16Pool);
        }
    }
}
