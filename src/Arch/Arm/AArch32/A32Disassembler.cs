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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Types;
using static Reko.Arch.Arm.AArch32.A32Disassembler.Decoder;

namespace Reko.Arch.Arm.AArch32
{
    public class A32Disassembler : DisassemblerBase<AArch32Instruction>
    {
        private static readonly Decoder rootDecoder;
        private static readonly Decoder invalid; 

        private Arm32Architecture arch;
        private EndianImageReader rdr;
        private Address addr;

        public A32Disassembler(Arm32Architecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
        }

        public override AArch32Instruction DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadUInt32(out uint wInstr))
                return null;
            var instr = rootDecoder.Decode(wInstr, this);
            instr.Address = addr;
            instr.Length = 4;
            return instr;
        }

        private AArch32Instruction Decode(uint wInstr, Opcode opcode, string format)
        {
            var ops = new List<MachineOperand>();
            bool updateFlags = false;
            bool writeback = false;
            Opcode shiftOp = Opcode.Invalid;
            MachineOperand shiftValue = null;
            for (int i = 0; i < format.Length; ++i)
            {
                MachineOperand op;
                int offset;
                uint imm;
                switch (format[i])
                {
                case ',':
                    continue;
                case 'I':   // 12-bit encoded immediate at offset 0;
                    op = DecodeImm12(wInstr);
                    break;
                case 'J':   // 24-bits at offset 0.
                    offset = 8 + (((int)wInstr << 8) >> 6);
                    op = AddressOperand.Create(addr + offset);
                    break;
                case 'V':   // 24-bits at offset 0.
                    imm = wInstr & 0x00FFFFFF;
                    op = ImmediateOperand.Word32(imm);
                    break;
                case 'X':   // 24-bits + extra H bit
                    offset = 8 + (((int)wInstr << 8) >> 6);
                    offset |= ((int)wInstr >> 23) & 2;
                    op = AddressOperand.Create(addr + offset);
                    break;
                case 'Y':   // immediate low 12 bits + extra 4 bits
                    imm = (wInstr & 0xFFF) | ((wInstr >> 4) & 0xF000);
                    op = ImmediateOperand.Word32(imm);
                    break;
                case 'r':   // register at a 4-bit multiple offset
                    offset = (format[++i] - '0') * 4;
                    op = new RegisterOperand(Registers.GpRegs[bitmask(wInstr, offset, 0xF)]);
                    break;
                case '[':
                    {
                        ++i;
                        var memType = format[i];
                        ++i;
                        Debug.Assert(format[i] == ':');
                        ++i;
                        var dom = format[i];
                        ++i;
                        var size = format[i] - '0';
                        ++i;
                        var dt = GetDataType(dom, size);
                        (op, writeback) = DecodeMemoryAccess(wInstr, memType, dt);
                    }
                    break;
                case 'M': // Multiple registers
                    op = new MultiRegisterOperand(PrimitiveType.Word16, (ushort)wInstr);
                    break;
                case 'S':   // 'SR' = special register
                    if (i < format.Length - 1 && format[i+1] == 'R')
                    {
                        ++i;
                        var sr = bit(wInstr, 22) ? Registers.spsr : Registers.cpsr;
                        op = new RegisterOperand(sr);
                    }
                    else 
                        goto default;
                    break;
                case 's':   // use bit 20 to determine 
                    updateFlags = ((wInstr >> 20) & 1) != 0;
                    continue;
                case '>':   // shift
                    ++i;
                    if (format[i] == 'i')
                        (shiftOp, shiftValue) = DecodeImmShift(wInstr);
                    else
                        (shiftOp, shiftValue) = DecodeRegShift(wInstr);
                    continue;
                default:
                    throw new NotImplementedException($"Format character '{format[i]}' is unknown.");
                }
                ops.Add(op);
            }
            var instr = new AArch32Instruction
            {
                opcode = opcode,
                ops = ops.ToArray(),
                ShiftType = shiftOp,
                ShiftValue = shiftValue,
                UpdateFlags = updateFlags,
                Writeback = writeback,
            };
            return instr;
        }

        private PrimitiveType GetDataType(char dom, int size)
        {
            switch (dom)
            {
            case 'w':
                switch (size)
                {
                case 1: return PrimitiveType.Byte;
                case 2: return PrimitiveType.Word16;
                case 4: return PrimitiveType.Word32;
                case 8: return PrimitiveType.Word64;
                }
                break;
            case 's':
                switch (size)
                {
                case 1: return PrimitiveType.SByte;
                case 2: return PrimitiveType.Int16;
                case 4: return PrimitiveType.Int32;
                case 8: return PrimitiveType.Int64;
                }
                break;
            case 'u':
                switch (size)
                {
                case 1: return PrimitiveType.UInt8;
                case 2: return PrimitiveType.UInt16;
                case 4: return PrimitiveType.UInt32;
                case 8: return PrimitiveType.UInt64;
                }
                break;
            }
            throw new InvalidOperationException($"Unknown size specifier {dom}{size}.");
        }

        private (Opcode, MachineOperand) DecodeImmShift(uint wInstr)
        {
            uint type = bitmask(wInstr, 5, 0x3);
            int shift_n = (int)bitmask(wInstr, 7, 0x1F);
            Opcode shift_t;
            switch (type)
            {
            case 0:
                shift_t = shift_n > 0 ? Opcode.lsl : Opcode.Invalid;
                break;
            case 1:
                shift_t = Opcode.lsr;
                shift_n = shift_n == 0 ? 32 : shift_n;
                break;
            case 2:
                shift_t = Opcode.asr;
                shift_n = shift_n == 0 ? 32 : shift_n;
                break;
            case 3:
                shift_t = shift_n > 0 ? Opcode.ror : Opcode.rrx;
                shift_n = shift_n == 0 ? 1 : shift_n;
                break;
            default:
                throw new InvalidOperationException("impossiburu");
            }
            return (shift_t != Opcode.Invalid)
                ? (shift_t, ImmediateOperand.Int32(shift_n))
                : (shift_t, null);
        }

        private (Opcode,MachineOperand) DecodeRegShift(uint wInstr)
        {
            uint type = bitmask(wInstr, 5, 0x3);
            var shift_n = Registers.GpRegs[(int)bitmask(wInstr, 8, 0xF)];
            Opcode shift_t;
            switch (type)
            {
            case 0:
                shift_t = Opcode.lsl;
                break;
            case 1:
                shift_t = Opcode.lsr;
                break;
            case 2:
                shift_t = Opcode.asr;
                break;
            case 3:
                shift_t = Opcode.ror;
                break;
            default:
                throw new InvalidOperationException("impossiburu");
            }
            return (shift_t, new RegisterOperand(shift_n));
        }


        private ImmediateOperand DecodeImm12(uint wInstr)
        {
            var imm12 = bitmask(wInstr, 0, 0xFFF);
            var unrotated_value = imm12 & 0xF;
            var n = Bits.RotateR32(unrotated_value, 2 * (int)bitmask(wInstr, 8, 0xF));
            return ImmediateOperand.Word32(n);
        }

        private (MemoryOperand, bool) DecodeMemoryAccess(uint wInstr, char memType, PrimitiveType dtAccess)
        {
            var n = Registers.GpRegs[bitmask(wInstr, 16, 0xF)];
            var m = Registers.GpRegs[bitmask(wInstr, 0, 0x0F)];
            Constant offset = null;
            int shiftAmt = 0;
            Opcode shiftType = Opcode.Invalid;
            switch (memType)
            {
            case 'o':   // offset
                offset = Constant.Int32((int)bitmask(wInstr, 0, 0xFFF));
                m = null;
                break;
            case 'h':   // offset split in hi-lo nybbles.
                offset = Constant.Int32(
                    (int)(((wInstr >> 4) & 0xF0) | (wInstr & 0x0F)));
                m = null;
                break;
            case 'x':   // wide shift amt.
                shiftAmt = (int)bitmask(wInstr, 7, 0x1F);
                shiftType = shiftAmt > 0 ? Opcode.lsl : Opcode.Invalid;
                //$TODO exotic shifts rotates etc
                break;
            }
            bool add = bit(wInstr, 23);
            bool preIndex = bit(wInstr, 24);
            bool wback = bit(wInstr, 21);
            bool writeback = !preIndex | wback;
            return
                (new MemoryOperand(dtAccess)
                {
                    BaseRegister = n,
                    Offset = offset,
                    Index = m,
                    Add = add,
                    PreIndex = preIndex,
                    ShiftType = shiftType,
                    Shift = shiftAmt,
                },
                writeback);
        }

        public abstract class Decoder
        {
            public abstract AArch32Instruction Decode(uint wInstr, A32Disassembler dasm);

            public static uint bitmask(uint u, int shift, uint mask)
            {
                return (u >> shift) & mask;
            }

            public static bool bit(uint u, int shift)
            {
                return ((u >> shift) & 1) != 0;
            }
        }

        public class MaskDecoder : Decoder
        {
            private int shift;
            private uint mask;
            private Decoder[] decoders;

            public MaskDecoder(int shift, uint mask, params Decoder[] decoders)
            {
                this.shift = shift;
                this.mask = mask;
                Debug.Assert(decoders.Length == mask + 1);
                this.decoders = decoders;
            }

            public override AArch32Instruction Decode(uint wInstr, A32Disassembler dasm)
            {
                TraceDecoder(wInstr);
                uint op = (wInstr >> shift) & mask;
                return decoders[op].Decode(wInstr, dasm);
            }

            [Conditional("DEBUG")]
            public void TraceDecoder(uint wInstr)
            {
                var shMask = this.mask << shift;
                var hibit = 0x80000000u;
                var sb = new StringBuilder();
                for (int i = 0; i < 32; ++i)
                {
                    if ((shMask & hibit) != 0)
                    {
                        sb.Append((wInstr & hibit) != 0 ? '1' : '0');
                    }
                    else
                    {
                        sb.Append((wInstr & hibit) != 0 ? ':' : '.');
                    }
                    shMask <<= 1;
                    wInstr <<= 1;
                }
                Debug.Print(sb.ToString());
            }
        }

        public class SparseMaskDecoder : Decoder
        {
            private int shift;
            private uint mask;
            private Dictionary<uint, Decoder> decoders;
            private Decoder @default;

            public SparseMaskDecoder(int shift, uint mask, Dictionary<uint, Decoder> decoders, Decoder @default)
            {
                this.shift = shift;
                this.mask = mask;
                this.decoders = decoders;
                this.@default = @default;
            }

            public override AArch32Instruction Decode(uint wInstr, A32Disassembler dasm)
            {
                var op = (wInstr >> shift) & mask;
                if (!decoders.TryGetValue(op, out Decoder decoder))
                    decoder = @default;
                return decoder.Decode(wInstr, dasm);
            }
        }

        public class NyiDecoder : Decoder
        {
            private string message;

            public NyiDecoder(string message)
            {
                this.message = message;
            }

            public override AArch32Instruction Decode(uint wInstr, A32Disassembler dasm)
            {
                throw new NotImplementedException($"An A32 decoder for the instruction {wInstr:X} ({message}) has not been implemented.");
            }
        }

        private class InstrDecoder : Decoder
        {
            private Opcode opcode;
            private string format;

            public InstrDecoder(Opcode opcode, string format)
            {
                this.opcode = opcode;
                this.format = format;
            }

            public override AArch32Instruction Decode(uint wInstr, A32Disassembler dasm)
            {
                if (format == null)
                    throw new NotImplementedException($"Format missing for ARM instruction {opcode}.");
                return dasm.Decode(wInstr, opcode, format);
            }
        }

        private class CondMaskDecoder : MaskDecoder
        {
            public CondMaskDecoder(int shift, uint mask, params Decoder[] decoders)
                : base(shift, mask, decoders)
            { }

            public override AArch32Instruction Decode(uint wInstr, A32Disassembler dasm)
            {
                var instr = base.Decode(wInstr, dasm);
                instr.condition = (ArmCondition)(wInstr >> 28);
                return instr;
            }
        }

        // Special decoder for when a 4-bit field has the bit pattern 1111 or not.
        private class PcDecoder : Decoder
        {
            private int shift;
            private Decoder not1111;
            private Decoder is1111;

            public PcDecoder(int shift, Decoder not1111, Decoder is1111)
            {
                this.shift = shift;
                this.not1111 = not1111;
                this.is1111 = is1111;
            }

            public override AArch32Instruction Decode(uint wInstr, A32Disassembler dasm)
            {
                var op = (wInstr >> shift) & 0xF;
                if (op == 0xF)
                    return is1111.Decode(wInstr, dasm);
                else
                    return not1111.Decode(wInstr, dasm);
            }
        }

        class CustomDecoder : Decoder
        {
            private Func<uint, A32Disassembler, Decoder> decode;

            public CustomDecoder(Func<uint, A32Disassembler, Decoder> decode)
            {
                this.decode = decode;
            }

            public override AArch32Instruction Decode(uint wInstr, A32Disassembler dasm)
            {
                return decode(wInstr, dasm).Decode(wInstr, dasm);
            }
        }

        private class MovDecoder : InstrDecoder
        {
            public MovDecoder(Opcode opcode, string format) : base(opcode, format) { }

            public override AArch32Instruction Decode(uint wInstr, A32Disassembler dasm)
            {
                var instr = base.Decode(wInstr, dasm);
                if (instr.ShiftType != Opcode.Invalid)
                {
                    instr.opcode = instr.ShiftType;
                }
                return instr;
            }
        }

        private static NyiDecoder nyi(string str)
        {
            return new NyiDecoder(str);
        }

        private static Decoder SparseMask(int shift, uint mask, Dictionary<uint, Decoder> decoders)
        {
            return new SparseMaskDecoder(shift, mask, decoders, invalid);
        }

        private static Decoder SparseMask(int shift, uint mask, Dictionary<uint, Decoder> decoders, Decoder @default)
        {
            return new SparseMaskDecoder(shift, mask, decoders, @default);
        }

        static A32Disassembler()
        {
            invalid = new InstrDecoder(Opcode.Invalid, "");

            var LoadStoreExclusive = nyi("LoadStoreExclusive");

            var Stl = new InstrDecoder(Opcode.stl, null);
            var Stlex = new InstrDecoder(Opcode.stlex, null);
            var Strex = new InstrDecoder(Opcode.strex, null);
            var Lda = new InstrDecoder(Opcode.lda, null);
            var Ldaex = new InstrDecoder(Opcode.ldaex, null);
            var Ldrex = new InstrDecoder(Opcode.ldrex, null);
            var Stlexd = new InstrDecoder(Opcode.stlexd, null);
            var Strexd = new InstrDecoder(Opcode.strexd, null);
            var Ldaexd = new InstrDecoder(Opcode.ldaexd, null);
            var Ldrexd = new InstrDecoder(Opcode.ldrexd, null);
            var Stlb = new InstrDecoder(Opcode.stlb, null);
            var Stlexb = new InstrDecoder(Opcode.stlexb, null);
            var Strexb = new InstrDecoder(Opcode.strexb, null);
            var Ldab = new InstrDecoder(Opcode.ldab, null);
            var Ldaexb = new InstrDecoder(Opcode.ldrexb, null);
            var Ldrexb = new InstrDecoder(Opcode.ldaexb, null);
            var Stlh = new InstrDecoder(Opcode.stlh, null);
            var Stlexh = new InstrDecoder(Opcode.stlexh, null);
            var Strexh = new InstrDecoder(Opcode.strexh, null);
            var Ldah = new InstrDecoder(Opcode.ldah, null);
            var Ldaexh = new InstrDecoder(Opcode.ldrexh, null);
            var Ldrexh = new InstrDecoder(Opcode.ldaexh, null);

            var SynchronizationPrimitives = new MaskDecoder(23, 1,
                invalid,
                new MaskDecoder(20, 7,  // type || L
                    new MaskDecoder(8, 3,   // ex ord
                        Stl,
                        invalid,
                        Stlex,
                        Strex),
                    new MaskDecoder(8, 3,   // ex ord
                        Lda,
                        invalid,
                        Ldaex,
                        Ldrex),
                    new MaskDecoder(8, 3,   // ex ord
                        invalid,
                        invalid,
                        Stlexd,
                        Strexd),
                    new MaskDecoder(8, 3,   // ex ord
                        invalid,
                        invalid,
                        Ldaexd,
                        Ldrexd),

                    new MaskDecoder(8, 3,   // ex ord
                        Stlb,
                        invalid,
                        Stlexb,
                        Strexb),
                    new MaskDecoder(8, 3,   // ex ord
                        Ldab,
                        invalid,
                        Ldaexb,
                        Ldrexb),
                    new MaskDecoder(8, 3,   // ex ord
                        Stlh,
                        invalid,
                        Stlexh,
                        Strexh),
                    new MaskDecoder(8, 3,   // ex ord
                        Ldah,
                        invalid,
                        Ldaexh,
                        Ldrexh)));

            var Mul = new InstrDecoder(Opcode.mul, "sr4,r0,r2");
            var Mla = new InstrDecoder(Opcode.mla, "sr4,r0,r2,r3");
            var Umaal = new InstrDecoder(Opcode.umaal, "sr3,r4,r0,r2");
            var Mls = new InstrDecoder(Opcode.mls, "sr4,r0,r2,r3");
            var Umull = new InstrDecoder(Opcode.umull, "sr3,r4,r0,r2");
            var Umlal = new InstrDecoder(Opcode.umlal, "sr4,r0,r2,r3");
            var Smull = new InstrDecoder(Opcode.smull, "sr4,r0,r2,r3");
            var Smlal = new InstrDecoder(Opcode.smlal, "sr4,r0,r2,r3");

            var MultiplyAndAccumulate = new MaskDecoder(20, 0xF,
               Mul,
               Mul,
               Mla,
               Mla,

               Umaal,
               invalid,
               Mls,
               invalid,

               Umull,
               Umull,
               Umlal,
               Umlal,

               Smull,
               Smull,
               Smlal,
               Smlal);

            // --
            var LdrdRegister = new InstrDecoder(Opcode.ldrd, "r3,[_:w8]");
            var LdrhRegister = new InstrDecoder(Opcode.ldrh, "r3,[_:u2]");
            var LdrsbRegister = new InstrDecoder(Opcode.ldrsb, "r3,[_:s1]");
            var LdrshRegister = new InstrDecoder(Opcode.ldrsh, "r3,[_:s2]");
            var Ldrht = new InstrDecoder(Opcode.ldrht, null);
            var Ldrsbt = new InstrDecoder(Opcode.ldrsbt, null);
            var Ldrsht = new InstrDecoder(Opcode.ldrsht, null);
            var StrdRegister = new InstrDecoder(Opcode.strd, null);
            var StrhRegister = new InstrDecoder(Opcode.strh, null);
            var Strht = new InstrDecoder(Opcode.strht, null);

            var LoadStoreDualHalfSbyteRegister = new MaskDecoder(24, 1,
                new MaskDecoder(20, 0x3,
                   new MaskDecoder(5, 3,
                        invalid,
                        StrhRegister,
                        LdrdRegister,
                        StrdRegister),
                    new MaskDecoder(5, 3,
                        invalid,
                        LdrhRegister,
                        LdrsbRegister,
                        LdrshRegister),
                    new MaskDecoder(5, 3,
                        invalid,
                        Strht,
                        invalid,
                        invalid),
                    new MaskDecoder(5, 3,
                        invalid,
                        Ldrht,
                        Ldrsbt,
                        Ldrsht)),
                new MaskDecoder(20, 1,
                    new MaskDecoder(5, 3,
                        invalid,
                        StrhRegister,
                        LdrdRegister,
                        StrdRegister),
                    new MaskDecoder(5, 3,
                        invalid,
                        LdrhRegister,
                        LdrsbRegister,
                        LdrshRegister)));

            var LdrdLiteral = new InstrDecoder(Opcode.ldrd, null);
            var LdrhLiteral = new InstrDecoder(Opcode.ldrh, null);
            var LdrsbLiteral = new InstrDecoder(Opcode.ldrsb, null);
            var LdrshLiteral = new InstrDecoder(Opcode.ldrsh, null);
            var StrhImmediate = new InstrDecoder(Opcode.strh, null);
            var LdrdImmediate = new InstrDecoder(Opcode.ldrd, null);
            var StrdImmediate = new InstrDecoder(Opcode.strd, null);
            var LdrhImmediate = new InstrDecoder(Opcode.ldrh, null);
            var LdrsbImmediate = new InstrDecoder(Opcode.ldrsb, "r3,[h:s1]");
            var LdrshImmediate = new InstrDecoder(Opcode.ldrsh, null);

            var LoadStoreDualHalfSbyteImmediate = new CustomDecoder((wInstr, dasm) =>
            {
                var rn = bitmask(wInstr, 16, 0xF);
                var pw = bitmask(wInstr, 23, 2) | bitmask(wInstr, 21, 1);
                var o1 = bitmask(wInstr, 20, 1);
                var op2 = bitmask(wInstr, 5, 3);
                if (rn == 0xF)
                {
                    if (o1 == 0)
                    {
                        if (op2 == 2)
                            return LdrdLiteral;
                    }
                    else
                    {
                        if (pw != 1)
                        {
                            new MaskDecoder(5, 3,
                                invalid,
                                LdrhLiteral,
                                LdrsbLiteral,
                                LdrshLiteral);
                        }
                        else
                        {
                            return invalid;
                        }
                    }
                }
                switch ((pw << 1) | o1)
                {
                case 0:
                    return new MaskDecoder(5, 3,
                        invalid,
                        StrhImmediate,
                        LdrdImmediate,
                        StrdImmediate);
                case 1:
                    return new MaskDecoder(5, 3,
                        invalid,
                        LdrhImmediate,
                        LdrsbImmediate,
                        LdrshImmediate);
                case 2:
                    return new MaskDecoder(5, 3,
                        invalid,
                        Strht,
                        invalid,
                        invalid);
                case 3:
                    return new MaskDecoder(5, 3,
                        invalid,
                        Strht,
                        invalid,
                        invalid);
                case 4:
                    return new MaskDecoder(5, 3,
                        invalid,
                        StrhImmediate,
                        LdrdImmediate,
                        StrdImmediate);
                case 5:
                    return new MaskDecoder(5, 3,
                        invalid,
                        LdrhImmediate,
                        LdrsbImmediate,
                        LdrshImmediate);
                case 6:
                    return new MaskDecoder(5, 3,
                        invalid,
                        StrhImmediate,
                        LdrdImmediate,
                        StrdImmediate);
                case 7:
                    return new MaskDecoder(5, 3,
                        invalid,
                        LdrhImmediate,
                        LdrsbImmediate,
                        LdrshImmediate);
                }
                throw new InvalidOperationException("Impossible");
            });

            var ExtraLoadStore = new MaskDecoder(22, 1,
                LoadStoreDualHalfSbyteRegister,
                LoadStoreDualHalfSbyteImmediate);

            var Mrs = new InstrDecoder(Opcode.mrs, "r3,SR");
            var Msr = new InstrDecoder(Opcode.msr, null);
            var MrsBanked = new InstrDecoder(Opcode.mrs, null);
            var MsrBanked = new InstrDecoder(Opcode.msr, null);
            var MoveSpecialRegister = new MaskDecoder(21, 1,
                new MaskDecoder(9, 1,
                    Mrs,
                    MrsBanked),
                new MaskDecoder(9, 1,
                    Msr,
                    MsrBanked));

            var Crc32 = new InstrDecoder(Opcode.crc32w, null);
            var Crc32C = new InstrDecoder(Opcode.crc32cw, null);

            var CyclicRedundancyCheck = new MaskDecoder(21, 3,
                new MaskDecoder(9, 1,
                    Crc32,
                    Crc32C),  
                new MaskDecoder(9, 1,
                    Crc32,
                    Crc32C),  
                new MaskDecoder(9, 1,
                    Crc32,
                    Crc32C),  
                new MaskDecoder(9, 1,
                    Crc32,
                    Crc32C));

            var Qadd = new InstrDecoder(Opcode.qadd, null);
            var Qsub = new InstrDecoder(Opcode.qsub, null);
            var Qdadd = new InstrDecoder(Opcode.qdadd, null);
            var Qdsub = new InstrDecoder(Opcode.qdsub, null);
            var IntegerSaturatingArithmetic = new MaskDecoder(21, 3,
                Qadd,
                Qsub,
                Qdadd,
                Qsub);


            var Hlt = new InstrDecoder(Opcode.hlt, null);
            var Bkpt = new InstrDecoder(Opcode.bkpt, null);
            var Hvc = new InstrDecoder(Opcode.hvc, null);
            var Smc = new InstrDecoder(Opcode.smc, null);
            var ExceptionGeneration = new MaskDecoder(21, 3,
                Hlt,
                Bkpt,
                Hvc,
                Smc);

            var Bx = new InstrDecoder(Opcode.bx, null);
            var Bxj = new InstrDecoder(Opcode.bxj, null);
            var Blx = new InstrDecoder(Opcode.blx, null);
            var Clz = new InstrDecoder(Opcode.blx, null);
            var Eret = new InstrDecoder(Opcode.eret, null);
            var Miscellaneous = new MaskDecoder(21, 3,   // op0
                new MaskDecoder(4, 7, // op1
                    MoveSpecialRegister,
                    invalid,
                    invalid,
                    invalid,

                    CyclicRedundancyCheck,
                    IntegerSaturatingArithmetic,
                    invalid,
                    ExceptionGeneration),
                new MaskDecoder(4, 7, // op1
                    MoveSpecialRegister,
                    Bx,
                    Bxj,
                    Blx,

                    CyclicRedundancyCheck,
                    IntegerSaturatingArithmetic,
                    invalid,
                    ExceptionGeneration),
                new MaskDecoder(4, 7, // op1
                    MoveSpecialRegister,
                    invalid,
                    invalid,
                    invalid,

                    CyclicRedundancyCheck,
                    IntegerSaturatingArithmetic,
                    invalid,
                    ExceptionGeneration),
                new MaskDecoder(4, 7, // op1
                    MoveSpecialRegister,
                    Clz,
                    invalid,
                    invalid,

                    CyclicRedundancyCheck,
                    IntegerSaturatingArithmetic,
                    Eret,
                    ExceptionGeneration));

            var HalfwordMultiplyAndAccumulate = new MaskDecoder(21, 0x3,
                nyi("SmlabbSmlabtSmlatbSmlatt"),
                new MaskDecoder(5, 3,
                    nyi("SmlawbSmlawt"),
                    nyi("SmulwbSmulwt"),
                    nyi("SmlawbSmlawt"),
                    nyi("SmulwbSmulwt")),
                nyi("SmlalbbSmlalbt"),
                nyi("SmulbbSmulbt"));

            var IntegerDataProcessingImmShift = new MaskDecoder(21, 7,
                new InstrDecoder(Opcode.and, "sr3,r4,r0,>i"),
                new InstrDecoder(Opcode.eor, "sr3,r4,r0,>i"),
                new InstrDecoder(Opcode.sub, "sr3,r4,r0,>i"),
                new InstrDecoder(Opcode.rsb, "sr3,r4,r0,>i"),
                new InstrDecoder(Opcode.add, "sr3,r4,r0,>i"),
                new InstrDecoder(Opcode.adc, "sr3,r4,r0,>i"),
                new InstrDecoder(Opcode.sbc, "sr3,r4,r0,>i"),
                new InstrDecoder(Opcode.rsc, "sr3,r4,r0,>i"));

            var IntegerTestAndCompareImmShift = new MaskDecoder(21, 3,
                new InstrDecoder(Opcode.tst, null),
                new InstrDecoder(Opcode.teq, null),
                new InstrDecoder(Opcode.cmp, null),
                new InstrDecoder(Opcode.cmn, null));

            var LogicalArithmeticImmShift = new MaskDecoder(21, 3,
                new InstrDecoder(Opcode.orr, null),
                new MovDecoder(Opcode.mov, "sr3,r0,>i"),
                new InstrDecoder(Opcode.bic, null),
                new InstrDecoder(Opcode.mvn, null));

            var DataProcessingImmediateShift = new MaskDecoder(23, 3,
                IntegerDataProcessingImmShift, // 3 reg, imm shift
                IntegerDataProcessingImmShift,
                IntegerTestAndCompareImmShift,
                LogicalArithmeticImmShift);

            var IntegerDataProcessingRegShift = new MaskDecoder(21, 7,
               new InstrDecoder(Opcode.and, "sr3,r4,r0,>r"),
               new InstrDecoder(Opcode.eor, "sr3,r4,r0,>r"),
               new InstrDecoder(Opcode.sub, "sr3,r4,r0,>r"),
               new InstrDecoder(Opcode.rsb, "sr3,r4,r0,>r"),
               new InstrDecoder(Opcode.add, "sr3,r4,r0,>r"),
               new InstrDecoder(Opcode.adc, "sr3,r4,r0,>r"),
               new InstrDecoder(Opcode.sbc, "sr3,r4,r0,>r"),
               new InstrDecoder(Opcode.rsc, "sr3,r4,r0,>r"));

            var IntegerTestAndCompareRegShift = new MaskDecoder(21, 3,
                new InstrDecoder(Opcode.tst, null),
                new InstrDecoder(Opcode.teq, null),
                new InstrDecoder(Opcode.cmp, null),
                new InstrDecoder(Opcode.cmn, null));

            var LogicalArithmeticRegShift = new MaskDecoder(21, 3,
                new InstrDecoder(Opcode.orr, null),
                new InstrDecoder(Opcode.mov, "@"),
                new InstrDecoder(Opcode.bic, null),
                new InstrDecoder(Opcode.mvn, null));

            var DataProcessingRegisterShift = new MaskDecoder(23, 3,
                IntegerDataProcessingRegShift,
                IntegerDataProcessingRegShift,
                IntegerTestAndCompareRegShift,
                LogicalArithmeticRegShift);

            var IntegerDataProcessingTwoRegImm = new MaskDecoder(21, 7,
               new InstrDecoder(Opcode.and, "sr3,r4,I"),
               new InstrDecoder(Opcode.eor, "sr3,r4,I"),
               new InstrDecoder(Opcode.sub, "sr3,r4,I"),
               new InstrDecoder(Opcode.rsb, "sr3,r4,I"),
               new InstrDecoder(Opcode.add, "sr3,r4,I"),
               new InstrDecoder(Opcode.adc, "sr3,r4,I"),
               new InstrDecoder(Opcode.sbc, "sr3,r4,I"),
               new InstrDecoder(Opcode.rsc, "sr3,r4,I"));

            var LogicalArithmeticTwoRegImm = new MaskDecoder(21, 3,
                new InstrDecoder(Opcode.orr, "sr3,r4,I"),
                new InstrDecoder(Opcode.mov, "sr3,I"),
                new InstrDecoder(Opcode.bic, "sr3,r4,I"),
                new InstrDecoder(Opcode.mvn, "sr3,I"));

            var MoveHalfwordImm = new MaskDecoder(22, 1,
               new InstrDecoder(Opcode.mov, "r3,Y"),
               new InstrDecoder(Opcode.movt, null));

            var IntegerTestAndCompareOneRegImm = new MaskDecoder(21, 3,
                new InstrDecoder(Opcode.tst, "r4,I"),
                new InstrDecoder(Opcode.teq, "r4,I"),
                new InstrDecoder(Opcode.cmp, "r4,I"),
                new InstrDecoder(Opcode.cmn, "r4,I"));

            var MsrImmediate = new InstrDecoder(Opcode.msr, null);
            var Nop = new InstrDecoder(Opcode.nop, "");
            var Yield = new InstrDecoder(Opcode.yield, null);
            var Wfe = new InstrDecoder(Opcode.wfe, null);
            var Wfi = new InstrDecoder(Opcode.wfi, null);
            var Sev = new InstrDecoder(Opcode.sevl, null);
            var Sevl = new InstrDecoder(Opcode.sevl, null);
            var ReservedNop = new InstrDecoder(Opcode.nop, "");
            var Esb = new InstrDecoder(Opcode.esb, null);
            var Dbg = new InstrDecoder(Opcode.dbg, null);

            var MoveSpecialRegisterAndHints = new CustomDecoder((wInstr, dasm) =>
            {
                var imm12 = bitmask(wInstr, 0, 0xFF);
                var imm4 = bitmask(wInstr, 16, 0xF);
                var r_iim4 = (bitmask(wInstr, 22, 1) << 4) | imm4;
                if (r_iim4 != 0)
                    return MsrImmediate;
                switch (imm12 >> 4)
                {
                case 0:
                    switch (imm12 & 0xF)
                    {
                    case 0: return Nop;
                    case 1: return Yield;
                    case 2: return Wfe;
                    case 3: return Wfi;
                    case 4: return Sev;
                    case 5: return Sevl;
                    default: return ReservedNop;
                    }
                case 1:
                    switch (imm12 & 0x0F)
                    {
                    case 0: return Esb;
                    default: return ReservedNop;
                    }
                case 0xF: return Dbg;
                default: return ReservedNop;
                }
            });

            var DataProcessingImmediate = new MaskDecoder(23, 3,
                IntegerDataProcessingTwoRegImm,
                IntegerDataProcessingTwoRegImm,
                new MaskDecoder(20, 3,
                    MoveHalfwordImm,
                    IntegerTestAndCompareOneRegImm,
                    MoveSpecialRegisterAndHints,
                    IntegerTestAndCompareOneRegImm),
                LogicalArithmeticTwoRegImm);

            var DataProcessingAndMisc = new CustomDecoder((wInstr, dasm) =>
            {
                if (bitmask(wInstr, 25, 1) == 0)
                {
                    var op1 = bitmask(wInstr, 20, 0x1F);
                    var op2 = bitmask(wInstr, 7, 1);
                    var op3 = bitmask(wInstr, 5, 3);
                    var op4 = bitmask(wInstr, 4, 1);
                    if (op2 == 1 && op4 == 1)
                    {
                        if (op3 == 0)
                        {
                            if (op1 < 0x10)
                                return MultiplyAndAccumulate;
                            else
                                return SynchronizationPrimitives;
                        }
                        else
                        {
                            return ExtraLoadStore;
                        }
                    }
                    if ((op1 & 0x19) == 0x10)
                    {
                        if (op2 == 0)
                            return Miscellaneous;
                        else if (op2 == 1 && op4 == 0)
                            return HalfwordMultiplyAndAccumulate;
                        else
                            return nyi("DataProcessingAndMisc");
                    }
                    else
                    {
                        if (op4 == 0)
                            return DataProcessingImmediateShift;
                        else if (op2 == 0 && op4 == 1)
                            return DataProcessingRegisterShift;
                        else
                            return nyi("DataProcessingAndMisc");
                    }
                }
                else
                {
                    return DataProcessingImmediate;
                }
            });

            var LdrLiteral = new InstrDecoder(Opcode.ldr, null);
            var LdrbLiteral = new InstrDecoder(Opcode.ldrb, null);
            var StrImm = new InstrDecoder(Opcode.str, "r3,[o:w4]");
            var LdrImm = new InstrDecoder(Opcode.ldr, "r3,[o:w4]");
            var StrbImm = new InstrDecoder(Opcode.strb, null);
            var LdrbImm = new InstrDecoder(Opcode.ldrb, "r3,[o:w1]");
            
            var Strt = new InstrDecoder(Opcode.strt, null);
            var Ldrt = new InstrDecoder(Opcode.ldrt, null);
            var Strbt = new InstrDecoder(Opcode.strbt, null);
            var Ldrbt = new InstrDecoder(Opcode.ldrbt, null);

            var LoadStoreWordUnsignedByteImmLit = new CustomDecoder((wInstr, dasm) => {
                var rn = bitmask(wInstr, 16, 0xF);
                var pw = bitmask(wInstr, 23, 0x10) | bitmask(wInstr, 21, 1);
                var o2_1 = bitmask(wInstr, 21, 2) | bitmask(wInstr, 20, 1);
                if (rn == 0xF)
                {
                    if (pw != 1)
                    {
                        if (o2_1 == 0x01)
                        {
                            return LdrLiteral;
                        }
                        else if (o2_1 == 0x03)
                        {
                            return LdrbLiteral;
                        }
                    }
                }
                switch ((pw << 2) | o2_1)
                {
                case 0: return StrImm;
                case 1: return LdrImm;
                case 2: return StrbImm;
                case 3: return LdrbImm;

                case 4: return Strt;
                case 5: return Ldrt;
                case 6: return Strbt;
                case 7: return Ldrbt;

                case 8: return StrImm;
                case 9: return StrImm;
                case 10: return StrImm;
                case 11: return StrImm;

                case 12: return StrImm;
                case 13: return LdrImm;
                case 14: return StrbImm;
                case 15: return LdrbImm;
                }
                throw new InvalidOperationException("impossible");
            });

            var StrReg = new InstrDecoder(Opcode.str, "r3,[x:w4]");
            var LdrReg = new InstrDecoder(Opcode.ldr, "r3,[x:w4]");
            var StrbReg = new InstrDecoder(Opcode.strb, "r3,[x:w1]");
            var LdrbReg = new InstrDecoder(Opcode.ldrb, "r3,[x:w1]");

            var LoadStoreWordUnsignedByteRegister = new CustomDecoder((wInstr, dasm) =>
            {
                var po2w01 = bitmask(wInstr, 21, 8) | bitmask(wInstr, 20, 7);
                switch (po2w01)
                {
                case 0: return StrReg;
                case 1: return LdrReg;
                case 2: return Strt;
                case 3: return Ldrt;

                case 4: return StrbReg;
                case 5: return LdrbReg;
                case 6: return Strbt;
                case 7: return Ldrbt;

                case 8: return StrReg;
                case 9: return LdrReg;
                case 10: return StrReg;
                case 11: return LdrReg;

                case 12: return StrbReg;
                case 13: return LdrbReg;
                case 14: return StrbReg;
                case 15: return LdrbReg;
                }
                throw new InvalidOperationException();
            });

            var Sadd16 = new InstrDecoder(Opcode.sadd16, null);
            var Sasx = new InstrDecoder(Opcode.sasx, null);
            var Ssax = new InstrDecoder(Opcode.ssax, null);
            var Ssub16 = new InstrDecoder(Opcode.ssub16, null);
            var Sadd8 = new InstrDecoder(Opcode.sadd8, null);
            var Ssub8 = new InstrDecoder(Opcode.ssub8, null);
            var Qadd16 = new InstrDecoder(Opcode.qadd16, null);
            var Qasx = new InstrDecoder(Opcode.qasx, null);
            var Qsax = new InstrDecoder(Opcode.qsax, null);
            var Qsub16 = new InstrDecoder(Opcode.qsub16, null);
            var Qadd8 = new InstrDecoder(Opcode.qadd8, null);
            var QSub8 = new InstrDecoder(Opcode.qsub8, null);
            var Shadd16 = new InstrDecoder(Opcode.shadd16, null);
            var Shasx = new InstrDecoder(Opcode.shasx, null);
            var Shsax = new InstrDecoder(Opcode.shsax, null);
            var Shsub16 = new InstrDecoder(Opcode.shsub16, null);
            var Shadd8 = new InstrDecoder(Opcode.shadd8, null);
            var Shsub8 = new InstrDecoder(Opcode.shsub8, null);
            var Uadd16 = new InstrDecoder(Opcode.uadd16, null);
            var Uasx = new InstrDecoder(Opcode.uasx, null);
            var Usax = new InstrDecoder(Opcode.usax, null);
            var Usub16 = new InstrDecoder(Opcode.usub16, null);
            var Uadd8 = new InstrDecoder(Opcode.uadd8, null);
            var Usub8 = new InstrDecoder(Opcode.usub8, null);
            var Uqadd16 = new InstrDecoder(Opcode.uqadd16, null);
            var Uqasx = new InstrDecoder(Opcode.uqasx, null);
            var Uqsax = new InstrDecoder(Opcode.uqsax, null);
            var Uqsub16 = new InstrDecoder(Opcode.uqsub16, null);
            var Uqadd8 = new InstrDecoder(Opcode.uqadd8, null);
            var Uqsub8 = new InstrDecoder(Opcode.uqsub8, null);
            var Uhadd16 = new InstrDecoder(Opcode.uhadd16, null);
            var Uhasx = new InstrDecoder(Opcode.uhasx, null);
            var Uhsax = new InstrDecoder(Opcode.uhsax, null);
            var Uhsub16 = new InstrDecoder(Opcode.uhsub16, null);
            var Uhadd8 = new InstrDecoder(Opcode.uhadd8, null);
            var Uhsub8 = new InstrDecoder(Opcode.uhsub8, null);

            var ParallelArithmetic = new MaskDecoder(20, 7,
                invalid,
                new MaskDecoder(5, 7,
                    Sadd16,
                    Sasx,
                    Ssax,
                    Ssub16,

                    Sadd8,
                    invalid,
                    invalid,
                    Ssub8),
                new MaskDecoder(5, 7,
                    Qadd16,
                    Qasx,
                    Qsax,
                    Qsub16,

                    Qadd8,
                    invalid,
                    invalid,
                    QSub8),
                new MaskDecoder(5, 7,
                    Shadd16,
                    Shasx,
                    Shsax,
                    Shsub16,

                    Shadd8,
                    invalid,
                    invalid,
                    Shsub8),
                invalid,
                new MaskDecoder(5, 7,
                    Uadd16,
                    Uasx,
                    Usax,
                    Usub16,

                    Uadd8,
                    invalid,
                    invalid,
                    Usub8),
                new MaskDecoder(5, 7,
                    Uqadd16,
                    Uqasx,
                    Uqsax,
                    Uqsub16,

                    Uqadd8,
                    invalid,
                    invalid,
                    Uqsub8),
                new MaskDecoder(5, 7,
                    Uhadd16,
                    Uhasx,
                    Uhsax,
                    Uhsub16,

                    Uhadd8,
                    invalid,
                    invalid,
                    Uhsub8));

            var Media = new MaskDecoder(23, 3,
                ParallelArithmetic,
                nyi("media1"),
                nyi("media2"),
                nyi("media3"));

            var StmdaStmed = new InstrDecoder(Opcode.stmda, "r4,M");
            var LdmdaLdmfa = new InstrDecoder(Opcode.ldmda, "r4,M");
            var Stm =        new InstrDecoder(Opcode.stm, "r4,M");
            var Ldm =        new InstrDecoder(Opcode.ldm, "r4,M");
            var StmStmia =   new InstrDecoder(Opcode.stm, "r4,M");
            var LdmLdmia =   new InstrDecoder(Opcode.ldm, "r4,M");
            var StmdbStmfd = new InstrDecoder(Opcode.stmdb, "r4,M");
            var LdmdbLDmea = new InstrDecoder(Opcode.ldmdb, "r4,M");
            var StmibStmfa = new InstrDecoder(Opcode.stmib, "r4,M");
            var LdmibLdmed = new InstrDecoder(Opcode.ldmib, "r4,M");

            var LoadStoreMultiple = new MaskDecoder(22, 7, // PUop
                new MaskDecoder(20, 1, // L
                    StmdaStmed,
                    LdmdaLdmfa),
                new MaskDecoder(20, 1, // L
                    Stm,
                    Ldm),
                new MaskDecoder(20, 1, // L
                    StmStmia,
                    LdmLdmia),
                new MaskDecoder(20, 1, // L
                    Stm,
                    Ldm),

                new MaskDecoder(20, 1, // L
                    StmdbStmfd,
                    LdmdbLDmea),
                new MaskDecoder(20, 1, // L
                    Stm,
                    Ldm),
                new MaskDecoder(20, 1, // L
                    StmibStmfa,
                    LdmibLdmed),
                new MaskDecoder(20, 1, // L
                    Stm,
                    Ldm));

            var RfeRfeda = nyi("RfeRefda");
            var SrcSrsda = nyi("SrcSrsda");
            var ExceptionSaveRestore = new MaskDecoder(22, 7, // PUS
                new MaskDecoder(20, 1, // L
                    invalid,
                    RfeRfeda),
                new MaskDecoder(20, 1, // L
                    SrcSrsda,
                    invalid),
                new MaskDecoder(20, 1, // L
                    invalid,
                    RfeRfeda),
                new MaskDecoder(20, 1, // L
                    SrcSrsda,
                    invalid),

                new MaskDecoder(20, 1, // L
                    invalid,
                    RfeRfeda),
                new MaskDecoder(20, 1, // L
                    SrcSrsda,
                    invalid),
                new MaskDecoder(20, 1, // L
                    invalid,
                    RfeRfeda),
                new MaskDecoder(20, 1, // L
                    SrcSrsda,
                    invalid));

            var BranchImmediate = new PcDecoder(28,
                new MaskDecoder(24, 1,
                    new InstrDecoder(Opcode.b, "J"),
                    new InstrDecoder(Opcode.bl, "J")),
                new InstrDecoder(Opcode.blx, "X"));

            var Branch_BranchLink_BlockDataTransfer = new MaskDecoder(25, 1,
                new PcDecoder(28,
                    LoadStoreMultiple,
                    ExceptionSaveRestore),
                BranchImmediate);

            var SystemRegister_LdSt_64bitMove = nyi("SystemRegister_LdSt_64bitMove");

            var FloatingPointDataProcessing = nyi("FloatingPointDataProcessing");

            var AdvancedSIMDandFloatingPoint32bitMove = nyi("AdvancedSIMDandFloatingPoint32bitMove");

            Decoder SystemRegister_AdvancedSimd_FloatingPoint = new MaskDecoder(24, 3,
                SparseMask(9, 0x7, new Dictionary<uint, Decoder>
                {
                    { 4, SystemRegister_LdSt_64bitMove },
                    { 5, SystemRegister_LdSt_64bitMove },
                    { 7, SystemRegister_LdSt_64bitMove },
                }),
                SparseMask(9, 0x7, new Dictionary<uint, Decoder>
                {
                    { 4, SystemRegister_LdSt_64bitMove },
                    { 5, SystemRegister_LdSt_64bitMove },
                    { 7, SystemRegister_LdSt_64bitMove },
                }),
                SparseMask(9, 0x7, new Dictionary<uint, Decoder>
                {
                    //$TODO cond fields.
                    { 4, new MaskDecoder(4, 1,
                        FloatingPointDataProcessing,
                        AdvancedSIMDandFloatingPoint32bitMove)
                    },
                    { 5, new MaskDecoder(4, 1,
                        FloatingPointDataProcessing,
                        AdvancedSIMDandFloatingPoint32bitMove)
                    },
                }),
                new InstrDecoder(Opcode.svc, "V"));

            //}nyi("SystemRegister_AdvancedSimd_FloatingPoint");


            var ConditionalDecoder = new CondMaskDecoder(25, 0x7,
                DataProcessingAndMisc,
                DataProcessingAndMisc,
                LoadStoreWordUnsignedByteImmLit,
                new MaskDecoder(4, 1,
                    LoadStoreWordUnsignedByteRegister,
                    Media),
                Branch_BranchLink_BlockDataTransfer,
                Branch_BranchLink_BlockDataTransfer,
                SystemRegister_AdvancedSimd_FloatingPoint,
                SystemRegister_AdvancedSimd_FloatingPoint);

            var AdvancedSimd = nyi("AdvancedSimd");
            var AdvancedSimdElementLoadStore = nyi("AdvancedSimdElementLoadStore");
            var MemoryHintsAndBarriers = nyi("MemoryHintsAndBarries");

            var unconditionalDecoder = new MaskDecoder(25, 7,
                Miscellaneous,
                AdvancedSimd,
                new MaskDecoder(20, 1,
                    AdvancedSimdElementLoadStore,
                    MemoryHintsAndBarriers),
                new MaskDecoder(20, 1,
                    MemoryHintsAndBarriers,
                    invalid),
                
                Branch_BranchLink_BlockDataTransfer,
                Branch_BranchLink_BlockDataTransfer,
                SystemRegister_AdvancedSimd_FloatingPoint,
                SystemRegister_AdvancedSimd_FloatingPoint);


            rootDecoder = new MaskDecoder(28, 0x0F,
                ConditionalDecoder,
                ConditionalDecoder,
                ConditionalDecoder,
                ConditionalDecoder,

                ConditionalDecoder,
                ConditionalDecoder,
                ConditionalDecoder,
                ConditionalDecoder,

                ConditionalDecoder,
                ConditionalDecoder,
                ConditionalDecoder,
                ConditionalDecoder,

                ConditionalDecoder,
                ConditionalDecoder,
                ConditionalDecoder,
                unconditionalDecoder);






















            /*

            Saturate16Bit = new MaskDecoder(22, 1,
                Ssat16,
                Usat16);

            ReverseBitByte = new MaskDecoder(22, 1,
                new MaskDecoder(7, 1,
                    Rev,
                    Rev16),
                new MaskDecoder(7, 1,
                    Rbit,
                    Revsh));

            Saturate32Bit = new MaskDecoder(22, 1)
                Ssat,
                Usat);

            ExtendAndAdd = new PcDecoder(16, 0xF,
                // not pc 
                MaskDecoder(20, 7,
                    Sxtab16,
                    invalid,
                    Sxtab,
                    Sxtah,

                    Uxtab16,
                    invalid,
                    Uxtab,
                    Uxtah),
                MaskDecoder(20, 7,
                    Sxtb16,
                    invalid,
                    Sxtb,
                    Sxth,

                    Uxtb16,
                    invalid,
                    Uxtb,
                    Uxth));
            SignedMultiplyDivide = nyi,

            UnsignedSumOfAbsDifferences = new PcDecoder(12, 0xF,
                Usada8,
                Usad8);

            BitfieldInsert = new PcDecoder(0, 0x0F,
                Bfi,
                Bfc);

            PermanentlyUndefined = MaskDecoder(28, 0xF,
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
                invalid,

                invalid,
                invalid,
                Udf,
                invalid);

            BitfieldExtract = MaskDecoder(22, 1,
                Sbfx,
                Ubfx);







            var SystemRegister32bitMove = new PcDecoder(28,
                MaskDecoder(20, 1,
                    Mcr,
                    Mrc)),
                invalid);
                
                    */


        }
    }
}
