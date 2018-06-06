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
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Arm
{
    /// <summary>
    /// Disassembles machine code in the ARM T32 encoding into 
    /// ARM32 instructions.
    /// </summary>
    public class T32Disassembler : DisassemblerBase<Arm32InstructionNew>
    {
        private static readonly Decoder[] decoders;
        private static readonly Decoder invalid;

        private readonly ImageReader rdr;
        private readonly ThumbArchitecture arch;
        private Address addr;
        private int itState;
        private ArmCondition itCondition;

        public T32Disassembler(ThumbArchitecture arch, ImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.itState = 0;
            this.itCondition = ArmCondition.AL;
        }

        public override Arm32InstructionNew DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadLeUInt16(out var wInstr))
                return null;
            var instr = decoders[wInstr >> 13].Decode(this, wInstr);
            instr.Address = addr;
            instr.Length = (int)(rdr.Address - addr);
            if ((itState & 0x1F) == 0x10)
            {
                // No more IT bits, reset condition back to normal.
                itCondition = ArmCondition.AL;
                itState = 0;
            }
            else if (itState != 0 && instr.opcode != Opcode.it)
            {
                // We're still under the influence of the IT instruction.
                var bit = ((itState >> 4) ^ ((int)this.itCondition)) & 1;
                instr.condition = (ArmCondition) ((int)this.itCondition ^ bit );
                itState <<= 1;
            }
            return instr;
        }

        private Arm32InstructionNew DecodeFormat16(uint wInstr, Opcode opcode, string format)
        {
            var ops = new List<MachineOperand>();
            ArmCondition cc = ArmCondition.AL;
            bool updateFlags = false;
            for (int i = 0; i < format.Length; ++i)
            {
                int offset;
                int size;
                RegisterStorage baseReg;
                MachineOperand op = null;
                switch (format[i])
                {
                case ',':
                    continue;
                case '.':
                    updateFlags = true;
                    continue;
                case 's':
                    ++i;
                    if (Peek('p',format,i))
                    {
                        op = new RegisterOperand(arch.StackRegister);
                    } else
                    {
                        throw new NotImplementedException(format);
                    }
                    break;
                case 'S':   // shift amount in bitfield.
                    ++i;
                    offset = this.ReadDecimal(format, ref i);
                    Expect(':', format, ref i);
                    size = this.ReadDecimal(format, ref i);
                    op = ImmediateOperand.Int32(SBitfield(wInstr, offset, size));
                    break;
                case 'i':   // immediate value in bitfield(s)
                    ++i;
                    var n = 0u;
                    do
                    {
                        offset = this.ReadDecimal(format, ref i);
                        Expect(':', format, ref i);
                        size = this.ReadDecimal(format, ref i);
                        n = (n << size) | ((wInstr >> offset) & ((1u << size) - 1));
                    } while (PeekAndDiscard(':', format, ref i));
                    op = ImmediateOperand.Word32(n);
                    break;
                case 'I':   // immediate 7-bit value shifted left 2.
                    op = ImmediateOperand.Int32(((int)wInstr & 0x7F) << 2);
                    break;
                case 'M':
                    op = ModifiedImmediate(wInstr);
                    break;
                case 'x':   // Jump displacement in bits 9:3..7, shifted left by 1.
                    offset = (SBitfield(wInstr, 9, 1) << 6) |
                             (SBitfield(wInstr, 3, 5) << 1);
                    op = AddressOperand.Create(addr + (offset + 4));
                    break;
                case 'Y':   // Immediate value encoding in bits 26:12..14:0..7
                    offset = (SBitfield(wInstr, 26, 1) << 11) |
                             (SBitfield(wInstr, 12, 3) << 8) |
                             SBitfield(wInstr, 0, 8);
                    op = ImmediateOperand.Word32(offset);
                    break;
                case 'r':   // register specified by 3 bits (r0..r7)
                    offset = format[++i] - '0';
                    op = new RegisterOperand(Registers.GpRegs[SBitfield(wInstr, offset, 3)]);
                    break;
                case 'Q':   // register specified by 7:2..0:
                    op = new RegisterOperand(Registers.GpRegs[
                        ((((int)wInstr >> 7) & 1) << 3) |
                        ((int)wInstr & 0x03)]);
                    break;
                case 'R':   // 4-bit register.
                    ++i;
                    offset = ReadDecimal(format, ref i);
                    op = new RegisterOperand(Registers.GpRegs[
                        ((int)wInstr >> offset) & 0x0F]);
                    break;
                case 'T':   // 4-bit register, specified by bits 7 || 2..0
                    var tReg = ((wInstr & 0x80) >> 4) | (wInstr & 7);
                    op = new RegisterOperand(Registers.GpRegs[tReg]);
                    break;
                case '[':   // Memory access
                    ++i;
                    bool add = true;
                    if (PeekAndDiscard('s', format, ref i))
                    {
                        baseReg = arch.StackRegister;
                    }
                    else if (PeekAndDiscard('r', format, ref i))
                    {
                        var reg = ReadDecimal(format, ref i);
                        baseReg = Registers.GpRegs[SBitfield(wInstr, reg, 3)];
                    }
                    else if (PeekAndDiscard('R', format, ref i))
                    {
                        var reg = ReadDecimal(format, ref i);
                        baseReg = Registers.GpRegs[SBitfield(wInstr, reg, 4)];
                    }
                    else 
                    { 
                        throw new NotImplementedException();
                    }
                    if (PeekAndDiscard(',', format, ref i))
                    {
                        if (PeekAndDiscard('I', format, ref i))
                        {
                            offset = ReadDecimal(format, ref i);
                            Expect(':', format, ref i);
                            size = ReadDecimal(format, ref i);
                            offset = SBitfield(wInstr, offset, size) << 2;
                            add = true;
                        }
                        else
                        {
                            Expect('i', format, ref i);
                            offset = ReadDecimal(format, ref i);
                            Expect(':', format, ref i);
                            size = ReadDecimal(format, ref i);
                            offset = SBitfield(wInstr, offset, size);
                            add = true;
                        }
                        Expect(':', format, ref i);
                        var dt = DataType(format, ref i);
                        Expect(']', format, ref i);
                        op = new MemoryOperand(dt)
                        {
                            BaseRegister = baseReg,
                            Add = add,
                            Offset = Constant.Int32(offset)
                        };
                    }
                    break;
                case 'P': // PC-relative offset, aligned by 4 bytes
                    ++i;
                    offset = ReadDecimal(format, ref i);
                    Expect(':', format, ref i);
                    size = ReadDecimal(format, ref i);
                    op = AddressOperand.Create(addr.Align(4) + (SBitfield(wInstr, offset, size) << 2));
                    break;
                case 'p':   // PC-relative offset, shift by 1 bit.
                    ++i;
                    offset = ReadDecimal(format, ref i);
                    Expect(':', format, ref i);
                    size = ReadDecimal(format, ref i);
                    op = AddressOperand.Create(addr + (SBitfield(wInstr, offset, size) << 1));
                    break;
                case 'c':
                    ++i;
                    offset = ReadDecimal(format, ref i);
                    cc = (ArmCondition) SBitfield(wInstr, offset, 4);
                    --i;
                    continue;
                default:
                    throw new NotImplementedException($"Unknown format character {format[i]} when decoding {opcode}.");
                }
                ops.Add(op);
            }

            return new Arm32InstructionNew
            {
                opcode = opcode,
                condition = cc,
                UpdateFlags = updateFlags,
                op1 = ops.Count > 0 ? ops[0] : null,
                op2 = ops.Count > 1 ? ops[1] : null,
                op3 = ops.Count > 2 ? ops[2] : null,
                op4 = ops.Count > 3 ? ops[3] : null,
            };
        }

        private static ImmediateOperand ModifiedImmediate(uint wInstr)
        {
            var i_imm3_a = (SBitfield(wInstr, 10 + 16, 1) << 4) |
                (SBitfield(wInstr, 12, 3) << 1) |
                (SBitfield(wInstr, 7, 1));
            var abcdefgh = wInstr & 0xFF;
            switch (i_imm3_a)
            {
            case 0:
            case 1:
                return ImmediateOperand.Word32(abcdefgh);
            case 2:
            case 3:
                return ImmediateOperand.Word32((abcdefgh << 16) | abcdefgh);
            case 4:
            case 5:
                return ImmediateOperand.Word32((abcdefgh << 24) | (abcdefgh << 8));
            case 6:
            case 7:
                return ImmediateOperand.Word32(
                    (abcdefgh << 24) |
                    (abcdefgh << 16) |
                    (abcdefgh << 8) |
                    (abcdefgh));
            default:
                abcdefgh |= 0x80;
                return ImmediateOperand.Word32(abcdefgh << (0x20 - i_imm3_a));
            }
        }

        private static int SBitfield(uint word, int offset, int size)
        {
            return ((int)word >> offset) & ((1 << size) - 1);
        }

        private bool Peek(char c, string format, int i)
        {
            if (i >= format.Length)
                return false;
            return format[i] == c;
        }

        private bool PeekAndDiscard(char c, string format, ref int i)
        {
            if (i >= format.Length)
                return false;
            if (format[i] != c)
                return false;
            ++i;
            return true;
        }

        private void Expect(char c, string format, ref int i)
        {
            Debug.Assert(format[i] == c);
            ++i;
        }

        private int ReadDecimal(string format, ref int i)
        {
            int n = 0;
            while (i < format.Length)
            {
                char c = format[i];
                if (!char.IsDigit(c))
                    break;
                ++i;
                n = n * 10 + (c - '0');
            }
            return n;
        }

        private PrimitiveType DataType(string format, ref int i)
        {
            switch (format[i++])
            {
            case 'w': return PrimitiveType.Word32;
            case 'h': return PrimitiveType.Word16;
            case 'H': return PrimitiveType.Int16;
            case 'b': return PrimitiveType.Byte;
            case 'B': return PrimitiveType.SByte;
            default: throw new InvalidOperationException($"{format[i]}");
            }
        }

        private static Decoder DecodeBfcBfi(Opcode opcode, string format)
        {
            return new BfcBfiDecoder(opcode, format);
        }

        // Factory methods
        private static Instr16Decoder Instr(Opcode opcode, string format)
        {
            return new Instr16Decoder(opcode, format);
        }

        private static MaskDecoder Mask(int shift, uint mask, params Decoder [] decoders)
        {
            return new MaskDecoder(shift, mask, decoders);
        }

        private static SelectDecoder Select(Func<uint, bool> predicate, Decoder decoderTrue, Decoder decoderFalse)
        {
            return new SelectDecoder(predicate, decoderTrue, decoderFalse);
        }

        private static NyiDecoder Nyi(string msg)
        {
            return new NyiDecoder(msg);
        }

        #region Decoder classes

        /// <summary>
        /// A decoder is responsible for picking apart the bits of a machine language instruction
        /// and interpreting in the correct way.
        /// </summary>
        private abstract class Decoder
        {
            public abstract Arm32InstructionNew Decode(T32Disassembler dasm, uint wInstr);

            [Conditional("DEBUG")]
            public static void TraceDecoder(uint wInstr, int shift, uint mask)
            {
                var shMask = mask << shift;
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

        /// <summary>
        /// Shifts the machine code instruction by a given shift amount and masks it
        /// with a bit mask. The result extracted bitfield is then used as an index into
        /// sub-decoders. 
        /// </summary>
        private class MaskDecoder : Decoder
        {
            private readonly Decoder[] decoders;
            private readonly int shift;
            private readonly uint mask;

            public MaskDecoder(int shift, uint mask, params Decoder [] decoders)
            {
                Debug.Assert(decoders == null || (int) (mask + 1) == decoders.Length, $"shift: {shift}, mask: {mask}, decoders: {decoders.Length}");
                this.shift = shift;
                this.mask = mask;
                this.decoders = decoders;
            }

            public override Arm32InstructionNew Decode(T32Disassembler dasm, uint wInstr)
            {
                TraceDecoder(wInstr, shift, mask);
                var op = (wInstr >> shift) & mask;
                return decoders[op].Decode(dasm, wInstr);
            }
        }

        /// <summary>
        /// This decoder selected one of two sub-decoders based on the outcome of the predicate.
        /// </summary>
        private class SelectDecoder : Decoder
        {
            private readonly Func<uint, bool> predicate;
            private readonly Decoder trueDecoder;
            private readonly Decoder falseDecoder;

            public SelectDecoder(Func<uint, bool> predicate, 
                Decoder trueDecoder,
                Decoder falseDecoder)
            {
                this.predicate = predicate;
                this.trueDecoder = trueDecoder;
                this.falseDecoder = falseDecoder;
            }

            public override Arm32InstructionNew Decode(T32Disassembler dasm, uint wInstr)
            {
                return (predicate(wInstr) ? trueDecoder : falseDecoder).Decode(dasm, wInstr);
            }
        }

        /// <summary>
        /// Reads in the extra 16 bits needed for a 32-bit T32 instruction.
        /// </summary>
        private class LongDecoder : Decoder
        {
            private readonly Decoder[] decoders;

            public LongDecoder(Decoder [] decoders)
            {
                this.decoders = decoders;
            }

            public override Arm32InstructionNew Decode(T32Disassembler dasm, uint wInstr)
            {
                if (!dasm.rdr.TryReadLeUInt16(out var wNext))
                    return null;
                wInstr = (wInstr << 16) | wNext;
                TraceDecoder(wInstr, 9 + 16, 0xF);
                return decoders[SBitfield(wInstr, 9 + 16, 4)].Decode(dasm, wInstr);
            }
        }

        /// <summary>
        /// This decoder hands control back to the disassembler, passing the 
        /// deduced opcode and a format string describing the encoding of the 
        /// instruction operands.
        /// </summary>
        private class Instr16Decoder : Decoder
        {
            private readonly Opcode opcode;
            private readonly string format;

            public Instr16Decoder(Opcode opcode, string format)
            {
                this.opcode = opcode;
                this.format = format;
            }

            public override Arm32InstructionNew Decode(T32Disassembler dasm, uint wInstr)
            {
                return dasm.DecodeFormat16(wInstr, opcode, format);
            }
        }

        // Decodes the T1 BL instruction
        private class BlDecoder : Decoder
        {
            public override Arm32InstructionNew Decode(T32Disassembler dasm, uint wInstr)
            {
                var s = SBitfield(wInstr, 10 + 16, 1);
                var i1 = 1 & ~(SBitfield(wInstr, 13, 1) ^ s);
                var i2 = 1 & ~(SBitfield(wInstr, 11, 1) ^ s);
                var off = (int) Bits.SignExtend((uint)s, 1);
                off = (off << 2) | (i1 << 1) | i2;
                off = (off << 10) | SBitfield(wInstr, 16, 10);
                off = (off << 11) | SBitfield(wInstr, 0, 11);
                off <<= 1;
                return new Arm32InstructionNew
                {
                    opcode = Opcode.bl,
                    op1 = AddressOperand.Create(dasm.addr + off)
                };
            }
        }

        // Decodes LDM* STM* instructions
        private class LdmStmDecoder : Decoder
        {
            public override Arm32InstructionNew Decode(T32Disassembler dasm, uint wInstr)
            {
                var rn = Registers.GpRegs[SBitfield(wInstr, 16, 4)];
                var registers = (ushort)wInstr;
                var w = SBitfield(wInstr, 16 + 5, 1) != 0;
                var l = SBitfield(wInstr, 16 + 4, 1);
                if (w)
                {
                    // writeback
                    if (rn == Registers.sp)
                    {
                        return new Arm32InstructionNew
                        {
                            opcode = l != 0 ? Opcode.pop_w : Opcode.push_w,
                            op1 = new MultiRegisterOperand(PrimitiveType.Word16, (registers))
                        };
                    }
                }
                throw new NotImplementedException();
            }
        }

        // Decodes Mov/Movs instructions with optional shifts
        private class MovMovsDecoder : Instr16Decoder
        {
            private readonly string format;

            public MovMovsDecoder(Opcode opcode, string format) : base(opcode, format)
            {
                this.format = format;
            }

            public override Arm32InstructionNew Decode(T32Disassembler dasm, uint wInstr)
            {
                var instr = base.Decode(dasm, wInstr);
                if (instr.op3 is ImmediateOperand imm && imm.Value.IsIntegerZero)
                {
                    instr.opcode = Opcode.movs;
                }
                return instr;
            }
        }

        // Decodes IT instructions
        private class ItDecoder : Decoder
        {
            public override Arm32InstructionNew Decode(T32Disassembler dasm, uint wInstr)
            {
                var instr = new Arm32InstructionNew
                {
                    opcode = Opcode.it,
                    condition = (ArmCondition)SBitfield(wInstr, 4, 4),
                    itmask = (byte) SBitfield(wInstr, 0, 4)
                };
                // Add an extra bit for the 't' in 'it'.
                dasm.itState = instr.itmask | (SBitfield(wInstr, 4, 1) << 4);
                dasm.itCondition = instr.condition;
                return instr;
            }
        }

        // Decode BFC and BFI instructions, which display their immediate constants
        // differently from how they are repesented in the word.
        private class BfcBfiDecoder : Instr16Decoder
        {
            public BfcBfiDecoder(Opcode opcode, string format) : base(opcode, format)
            {
            }

            public override Arm32InstructionNew Decode(T32Disassembler dasm, uint wInstr)
            {
                // A hack -- we patch up the output of the regular decoder.
                var instr = base.Decode(dasm, wInstr);
                ImmediateOperand opLsb;
                ImmediateOperand opMsb = (ImmediateOperand)instr.op4;
                if (opMsb != null)
                {
                    opLsb = (ImmediateOperand)instr.op3;
                }
                else
                {
                    opMsb = (ImmediateOperand)instr.op3;
                    opLsb = (ImmediateOperand)instr.op2;
                }
                var opWidth = ImmediateOperand.Word32(opMsb.Value.ToInt32() - opLsb.Value.ToInt32() + 1);
                if (instr.op4 != null)
                    instr.op4 = opWidth;
                else
                    instr.op3 = opWidth;
                return instr;
            }
        }

        private class NyiDecoder : Decoder
        {
            private readonly string message;

            public NyiDecoder(string message)
            {
                this.message = message;
            }

            public override Arm32InstructionNew Decode(T32Disassembler dasm, uint wInstr)
            {
                throw new NotImplementedException($"A T32 decoder for the instruction {wInstr:X} ({message}) has not been implemented yet.");
            }
        }

        #endregion

        static T32Disassembler()
        {
            invalid = Instr(Opcode.Invalid, "");

            // Build the decoder decision tree.
            var dec16bit = Create16bitDecoders();
            var decB_T32 = Nyi("B-T32 variant");
            var dec32bit = CreateLongDecoder();
            decoders = new Decoder[8] {
                dec16bit,
                dec16bit,
                dec16bit,
                dec16bit,

                dec16bit,
                dec16bit,
                dec16bit,
                Mask(11, 0x03,
                    decB_T32,
                    dec32bit,
                    dec32bit,
                    dec32bit)
            };
        }

        private static MaskDecoder Create16bitDecoders()
        {
            var decAlu = CreateAluDecoder();
            var decDataLowRegisters = CreateDataLowRegisters();
            var decDataHiRegisters = Mask(8, 0x03, // Add, subtract, compare, move (two high registers)
                Nyi("add, adds"), // add, adds (register);
                Instr(Opcode.cmp, ".T,R3"),
                Instr(Opcode.mov, "Q,R3"), // mov,movs
                invalid);
            var decLdrLiteral = Nyi("Ldr literal");
            var decLdStRegOffset = Nyi("LdStRegOffset");
            var decLdStWB = Nyi("LdStWB");
            var decLdStHalfword = Nyi("LdStHalfWord");
            var decLdStSpRelative = Nyi("LdStSpRelative");
            var decAddPcSp = Mask(11, 1,
                Instr(Opcode.adr, "r8,P0:8"),
                Instr(Opcode.add, "r8,sp,I0:8"));
            var decMisc16Bit = CreateMisc16bitDecoder();
            var decLdmStm = Nyi("LdmStm");
            var decCondBranch = Mask(8, 0xF, // "CondBranch"
                Instr(Opcode.b, "c8p0:8"),
                Instr(Opcode.b, "c8p0:8"),
                Instr(Opcode.b, "c8p0:8"),
                Instr(Opcode.b, "c8p0:8"),

                Instr(Opcode.b, "c8p0:8"),
                Instr(Opcode.b, "c8p0:8"),
                Instr(Opcode.b, "c8p0:8"),
                Instr(Opcode.b, "c8p0:8"),

                Instr(Opcode.b, "c8p0:8"),
                Instr(Opcode.b, "c8p0:8"),
                Instr(Opcode.b, "c8p0:8"),
                Instr(Opcode.b, "c8p0:8"),

                Instr(Opcode.b, "c8p0:8"),
                Instr(Opcode.b, "c8p0:8"),
                Instr(Opcode.udf, "i0:8"),
                Instr(Opcode.svc, "i0:8"));

            return Mask(13, 0x07,
                decAlu,
                decAlu,
                Mask(10, 0x07,
                    decDataLowRegisters,
                    Mask(8, 3, // Special data and branch exchange 
                        decDataHiRegisters,
                        decDataHiRegisters,
                        decDataHiRegisters,
                        Mask(7,1,
                            Instr(Opcode.bx, "R3"),
                            Instr(Opcode.blx, "R3"))),
                    decLdrLiteral,
                    decLdrLiteral,

                    decLdStRegOffset,
                    decLdStRegOffset,
                    decLdStRegOffset,
                    decLdStRegOffset),
                Mask(11, 0x03,   // decLdStWB,
                    Instr(Opcode.str, "r0,[r3,I6:5:w]"),
                    Instr(Opcode.ldr, "r0,[r3,I6:5:w]"),
                    Instr(Opcode.strb, "r0,[r3,i6:5:b]"),
                    Instr(Opcode.ldrb, "r0,[r3,i6:5:b]")),

                Mask(12, 0x01,
                    Mask(11, 0x01,
                        Instr(Opcode.strh, "r0,[r3,I6:5:h]"),
                        Instr(Opcode.ldrh, "r0,[r3,I6:5:h]")),
                    Mask(11, 0x01,   // load store SP-relative
                        Instr(Opcode.str, "r8,[s,I0:8:w]"),
                        Instr(Opcode.ldr, "r8,[s,I0:8:w]"))),
                Mask(12, 0x01,
                    decAddPcSp,
                    decMisc16Bit),
                Mask(12, 0x01,
                    decLdmStm,
                    decCondBranch),
                Instr(Opcode.Invalid, ""));
        }

        private static Decoder CreateAluDecoder()
        {
            var decAddSub3 = Nyi("addsub3");
            var decAddSub3Imm = Nyi("AddSub3Imm");
            var decMovMovs = Mask(11, 3,
                new MovMovsDecoder(Opcode.lsls, ".r0,r3,S6:5"),
                new MovMovsDecoder(Opcode.lsrs, ".r0,r3,S6:5"),
                Instr(Opcode.asrs, ".r0,r3,S6:5"),
                invalid);
            var decAddSub = Nyi("AddSub");
            return Mask(10, 0xF,
                decMovMovs,
                decMovMovs,
                decMovMovs,
                decMovMovs,

                decMovMovs,
                decMovMovs,
                Mask(9, 1,
                    Instr(Opcode.add, "r0,r3,r6"),
                    Instr(Opcode.sub, "r0,r3,r6")),
                Mask(9, 1,
                    Instr(Opcode.add, "r0,r3,i6:3"),
                    Instr(Opcode.sub, "r0,r3,i6:3")),
                decAddSub,
                decAddSub,
                decAddSub,
                decAddSub,

                decAddSub,
                decAddSub,
                decAddSub,
                decAddSub);
        }

        private static Decoder CreateDataLowRegisters()
        {
            return Mask(6, 0xF,
                Instr(Opcode.and, ".r0,r3"),
                Instr(Opcode.eor, ".r0,r3"),
                Nyi("MOV,MOVS"),
                Nyi("MOV,MOVS"),

                Nyi("MOV,MOVS"),
                Instr(Opcode.adc, ".r0,r3"),
                Instr(Opcode.sbc, ".r0,r3"),
                Nyi("MOV,MOVS"),

                Instr(Opcode.adc, ".r0,r3"),
                Instr(Opcode.rsb, ".r0,r3"),
                Instr(Opcode.cmp, ".r0,r3"),
                Instr(Opcode.cmn, ".r0,r3"),

                invalid,
                invalid,
                invalid,
                invalid);
        }

        private static Decoder CreateMisc16bitDecoder()
        {
            var cbnzCbz = Mask(11, 1,
                Instr(Opcode.cbz, "r0,x"),
                Instr(Opcode.cbnz, "r0,x"));
            return Mask(8, 0xF,
                Mask(7, 1,  // Adjust SP
                    Instr(Opcode.add, "sp,I"),
                    Instr(Opcode.sub, "sp,I")),

                cbnzCbz,
                Nyi("Extend"),
                cbnzCbz,

                invalid,
                invalid,
                Mask(5, 0x7,
                    Nyi("SETPAN"),        // SETPAN
                    invalid,
                    Nyi("Change processor state"),        // Change processor state
                    Nyi("WUT"),

                    invalid,
                    invalid,
                    invalid,
                    invalid),
                invalid,

                invalid,
                cbnzCbz,
                Mask(6, 0x3,
                    Nyi("Reverse bytes"),        // reverse bytes
                    Nyi("Reverse bytes"),        // reverse bytes
                    Instr(Opcode.hlt, ""),
                    Nyi("Reverse bytes")),        // reverse bytes),
                cbnzCbz,

                invalid,
                invalid,
                Instr(Opcode.bkpt, ""),
                Select(w => (w & 0xF) == 0,
                    Mask(4, 0xF, // Hints
                        Instr(Opcode.nop, ""),
                        Instr(Opcode.yield, ""),
                        Instr(Opcode.wfe, ""),
                        Instr(Opcode.wfi, ""),

                        Instr(Opcode.sev, ""),
                        Instr(Opcode.nop, ""), // Reserved hints, behaves as NOP.
                        Instr(Opcode.nop, ""),
                        Instr(Opcode.nop, ""),

                        Instr(Opcode.nop, ""), // Reserved hints, behaves as NOP.
                        Instr(Opcode.nop, ""),
                        Instr(Opcode.nop, ""),
                        Instr(Opcode.nop, ""),

                        Instr(Opcode.nop, ""),
                        Instr(Opcode.nop, ""),
                        Instr(Opcode.nop, ""),
                        Instr(Opcode.nop, "")),
                    new ItDecoder()));

        }

        private static LongDecoder CreateLongDecoder()
        {
            var branchesMiscControl = CreateBranchesMiscControl();
            var loadStoreMultipleTableBranch = CreateLoadStoreMultipleBranchDecoder();

            var DataProcessingModifiedImmediate = Mask(4 + 16, 0x1F,
                Instr(Opcode.and, "R8,R16,M"),
                Select(wInstr => SBitfield(wInstr, 8, 4) != 0xF,
                    Instr(Opcode.and, ".R8,R16,M"),
                    Instr(Opcode.tst, "R16,M")),
                Instr(Opcode.bic, "R8,R16,M"),
                Instr(Opcode.bic, ".R8,R16,M"),
                // 4
                Select(wInstr => SBitfield(wInstr, 16, 4) != 0xF,
                    Instr(Opcode.orr, "R8,R16,M"),
                    Instr(Opcode.mov, "R16,M")),
                Select(wInstr => SBitfield(wInstr, 16, 4) != 0xF,
                    Instr(Opcode.orr, ".R8,R16,M"),
                    Instr(Opcode.mov, ".R16,M")),
                Select(wInstr => SBitfield(wInstr, 16, 4) != 0xF,
                    Instr(Opcode.orn, "R8,R16,M"),
                    Instr(Opcode.mvn, "R16,M")),
                Select(wInstr => SBitfield(wInstr, 16, 4) != 0xF,
                    Instr(Opcode.orn, ".R8,R16,M"),
                    Instr(Opcode.mvn, ".R16,M")),
                // 8
                Instr(Opcode.eor, "R8,R16,M"),
                Select(wInstr => SBitfield(wInstr, 8, 4) != 0xF,
                    Instr(Opcode.eor, ".R8,R16,M"),
                    Instr(Opcode.teq, ".R16,M")),
                invalid,
                invalid,
                // C
                invalid,
                invalid,
                invalid,
                invalid,
                // 10
                Select(wInstr => SBitfield(wInstr, 16, 4) != 0xD,
                    Instr(Opcode.add, "R8,R16,M"),
                    Instr(Opcode.add, "R9,R16,M")), //$REVIEW: check this
                Select(wInstr => SBitfield(wInstr, 8, 4) != 0xF,
                    Select(wInstr => SBitfield(wInstr, 16, 4) != 0xD,
                        Instr(Opcode.add, ".R8,R16,M"),
                        Instr(Opcode.add, ".R9,R16,M")), //$REVIEW: check this
                    Instr(Opcode.cmn, "R16,M")),
                invalid,
                invalid,
                // 14
                Instr(Opcode.adc, "R8,R16,M"),
                Instr(Opcode.adc, ".R9,R16,M"),
                Instr(Opcode.sbc, "R8,R16,M"),
                Instr(Opcode.sbc, ".R9,R16,M"),
                // 18
                invalid,
                invalid,
                Select(wInstr => SBitfield(wInstr, 16, 4) != 0xD,
                    Instr(Opcode.sub, "R8,R16,M"),
                    Instr(Opcode.sub, "R9,R16,M")), //$REVIEW: check this
                Select(wInstr => SBitfield(wInstr, 8, 4) != 0xF,
                    Select(wInstr => SBitfield(wInstr, 16, 4) != 0xD,
                        Instr(Opcode.sub, ".R8,R16,M"),
                        Instr(Opcode.sub, ".R9,R16,M")), //$REVIEW: check this
                    Instr(Opcode.cmp, "R16,M")),
                // 1C
                Instr(Opcode.rsb, "R8,R16,M"),
                Instr(Opcode.rsb, ".R9,R16,M"),
                invalid,
                invalid);

            var DataProcessingSimpleImm = Mask(7 + 16, 1,
                Mask(5 + 16, 1,
                    Select(w => (SBitfield(w, 16, 4) & 0xD) != 0xD,
                        Nyi("AddAddsImm"),
                        Mask(17, 1,
                            Instr(Opcode.add, "R8,R16,Y"),
                            Nyi("ADR - T3"))),
                    invalid),
                Mask(5 + 16, 1,
                    invalid,
                    Select(w => (SBitfield(w, 16, 4) & 0xD) != 0xD,
                        Nyi("SubSubsImm"),
                        Mask(17, 1,
                            Nyi("SubSubsSpImm"),
                            Nyi("ADR - T2")))));

            var SaturateBitfield = Mask(5 + 16, 0x7,
                Nyi("SsatLslVariant"),
                Select(w => SBitfield(w, 12, 3) != 0 || SBitfield(w, 6, 2) != 0,
                    Nyi("ssatAsrVariant"),
                    Nyi("ssat16")),
                Nyi("sfbx"),
                Select(w => SBitfield(w, 16, 4) != 0xF,
                    DecodeBfcBfi(Opcode.bfi, "R8,R16,i12:3:6:2,i0:5"),
                    DecodeBfcBfi(Opcode.bfc, "R8,i12:3:6:2,i0:5")),
                // 4
                Nyi("usatLslVariant"),
                Select(w => SBitfield(w, 12, 3) != 0 || SBitfield(w, 6, 2) != 0,
                    Nyi("usatAsrVariant"),
                    Nyi("usat16")),
                Nyi("ufbx"),
                invalid);

            var DataProcessingPlainImm = Mask(8 + 16, 1,
                Mask(5 + 16, 3,
                    DataProcessingSimpleImm,
                    DataProcessingSimpleImm,
                    Nyi("MoveWide16BitImm"),
                    invalid),
                SaturateBitfield);

            var LoadStoreSignedPositiveImm = Select(w => SBitfield(w, 12, 4) != 0xF,
                Mask(5 + 16, 3,
                    Instr(Opcode.ldrsb, "R12,[R16,i0:12:B]"),
                    Instr(Opcode.ldrsh, "R12,[R16,i0:12:H]"),
                    invalid,
                    invalid),
                Mask(5 + 16, 3,
                    Nyi("PLI"),
                    Instr(Opcode.nop, ""),
                    invalid,
                    invalid));   // reserved hint

            var LoadStoreSingle = Mask(7 + 16, 3,
                Select(w => SBitfield(w, 16, 4) != 0xF,
                    Mask(10, 3,
                        Select(w => SBitfield(w, 6, 6) == 0,
                            Nyi("LoadStoreUnsignedRegisterOffset"),
                            invalid),
                        invalid,
                        Select(w => SBitfield(w, 8, 1) == 0,
                            invalid,
                            Nyi("LoadStoreUnsignedImmediatePostIndexed")),
                        Mask(8, 3,
                            Nyi("LoadStoreUnsignedNegativeImm"),
                            Nyi("LoadStoreUnsignedImmediatePreIndexed"),
                            Nyi("LoadStoreUnsignedUnprivileged"),
                            Nyi("LoadStoreUnsignedImmediatePreIndexed"))),
                    Nyi("LoadUnsignedLiteral")),
                Select(w => SBitfield(w, 16, 4) != 0xF,
                    Nyi("LoadStoreUnsignedPositiveImm"),
                    Nyi("LoadUnsignedLiteral")),
                Select(w => SBitfield(w, 16, 4) != 0xF,
                    Mask(10, 3,
                        Select(w => SBitfield(w, 6, 6) == 0,
                            Nyi("LoadStoreSignedRegisterOffset"),
                            invalid),
                        invalid,
                        Select(w => SBitfield(w, 8, 1) == 0,
                            invalid,
                            Nyi("LoadStoreSignedImmediatePostIndexed")),
                        Mask(8, 3,
                            Nyi("LoadStoreSignedNegativeImm"),
                            Nyi("LoadStoreSignedImmediatePreIndexed"),
                            Nyi("LoadStoreSignedUnprivileged"),
                            Nyi("LoadStoreSignedImmediatePreIndexed"))),
                    Nyi("LoadSignedLiteral")),
                Select(w => SBitfield(w, 16, 4) != 0xF,
                    LoadStoreSignedPositiveImm,
                    Nyi("LoadSignedLiteral")));


            var SystemRegisterLdStAnd64bitMove = Nyi("SystemRegisterLdStAnd64bitMove");
            var AvancedSimdLdStAnd64bitMove = Nyi("AvancedSimdLdStAnd64bitMove");
            var FloatingPointDataProcessing = Nyi("FloatingPointDataProcessing");
            var AdvancedSimdAndFloatingPoint32bitMove = Nyi("AdvancedSimdAndFloatingPoint32bitMove");
            var SystemRegister32bitMove = Nyi("SystemRegister32bitMove");
            var AdvancedSimdDataProcessing = Nyi("AdvancedSimdDataProcessing");
            var AdvancedSimd3RegistersSameLength = Nyi("AdvancedSimd3RegistersSameLength");
            var AdvancedSimdTwoScalarsAndExtension = Nyi("AdvancedSimdTwoScalarsAndExtension");
            var SystemRegisterAccessAdvSimdFpu = Mask(12 + 16, 1,
                Mask(8 + 16, 3, // op0 = 0
                    Mask(9, 7,  // op1 = 0b00
                        invalid,
                        invalid,
                        invalid,
                        invalid,
                        // 4
                        AvancedSimdLdStAnd64bitMove,
                        AvancedSimdLdStAnd64bitMove,
                        invalid,
                        SystemRegisterLdStAnd64bitMove),
                    Mask(9, 7,  // op1 = 0b01
                        invalid,
                        invalid,
                        invalid,
                        invalid,
                        // 4
                        AvancedSimdLdStAnd64bitMove,
                        AvancedSimdLdStAnd64bitMove,
                        invalid,
                        SystemRegisterLdStAnd64bitMove),
                    Mask(9, 7,  // op1 = 0b10
                        invalid,
                        invalid,
                        invalid,
                        invalid,
                        // 4
                        Mask(4, 1,
                            FloatingPointDataProcessing,
                            AdvancedSimdAndFloatingPoint32bitMove),
                        Mask(4, 1,
                            FloatingPointDataProcessing,
                            AdvancedSimdAndFloatingPoint32bitMove),
                        invalid,
                        Mask(4, 1,
                            invalid,
                            SystemRegister32bitMove)),
                    AdvancedSimdDataProcessing), // op1 = 0b11
                Mask(8 + 16, 3, // op0 = 1
                    Mask(9, 7,  // op1 = 0b00
                        invalid,
                        invalid,
                        invalid,
                        invalid,
                        // 4
                        AdvancedSimd3RegistersSameLength,
                        invalid,
                        AdvancedSimd3RegistersSameLength,
                        SystemRegisterLdStAnd64bitMove),
                    Mask(9, 7,  // op1 = 0b01
                        invalid,
                        invalid,
                        invalid,
                        invalid,
                        // 4
                        AdvancedSimd3RegistersSameLength,
                        invalid,
                        AdvancedSimd3RegistersSameLength,
                        SystemRegisterLdStAnd64bitMove),
                    Mask(9, 7,  // op1 = 0b10
                        invalid,
                        invalid,
                        invalid,
                        invalid,
                        // 4
                        Mask(4, 1,
                            FloatingPointDataProcessing,
                            AdvancedSimdTwoScalarsAndExtension),
                        Mask(4, 1,
                            FloatingPointDataProcessing,
                            invalid),
                        AdvancedSimdTwoScalarsAndExtension,
                        Mask(4, 1,
                            invalid,
                            SystemRegister32bitMove)),
                    AdvancedSimdDataProcessing) // op1 = 0b11
                );

            var DataProcessing2srcRegs = Mask(4 + 16, 7,
                Mask(4, 3,
                    Instr(Opcode.qadd, "*"),
                    Instr(Opcode.qdadd, "*"),
                    Instr(Opcode.qsub, "*"),
                    Instr(Opcode.qdsub, "*")),
                Mask(4, 3,
                    Instr(Opcode.rev, "*"),
                    Instr(Opcode.rev16, "*"),
                    Instr(Opcode.rbit, "*"),
                    Instr(Opcode.revsh, "*")),
                Mask(4, 3,
                    Instr(Opcode.sel, "*"),
                    invalid,
                    invalid,
                    invalid),
                Mask(4, 3,
                    Instr(Opcode.clz, "R8,R0"),
                    invalid,
                    invalid,
                    invalid),
                Mask(4, 3,
                    Nyi("crc32-crc32b"),
                    Nyi("crc32-crc32h"),
                    Nyi("crc32-crc32w"),
                    invalid),
                Mask(4, 3,
                    Nyi("crc32c-crc32cb"),
                    Nyi("crc32c-crc32ch"),
                    Nyi("crc32c-crc32cw"),
                    invalid),
                invalid,
                invalid);

            var DataProcessingRegister = Mask(7 + 16, 1,
                Mask(7, 1,
                    Select(w => SBitfield(w, 4, 4) == 0,
                        Nyi("MovMovsRegisterShiftedRegister"),
                        invalid),
                    Nyi("RegisterExtends")),
                Mask(6, 3,
                    Nyi("ParallelAddSub"),
                    Nyi("ParallelAddSub"),
                    DataProcessing2srcRegs,
                    invalid));

            return new LongDecoder(new Decoder[16]
            {
                invalid,
                invalid,
                invalid,
                invalid,

                loadStoreMultipleTableBranch,
                Nyi("Data processing (shifted register)"),
                SystemRegisterAccessAdvSimdFpu,
                SystemRegisterAccessAdvSimdFpu,

                Mask(15, 1,
                    DataProcessingModifiedImmediate,
                    branchesMiscControl),
                Mask(15, 1,
                    DataProcessingPlainImm,
                    branchesMiscControl),
                Mask(15, 1,
                    DataProcessingModifiedImmediate,
                    branchesMiscControl),
                Mask(15, 1,
                    DataProcessingPlainImm,
                    branchesMiscControl),

                LoadStoreSingle,
                Mask(7 + 16, 3,
                    DataProcessingRegister,
                    DataProcessingRegister,
                    Nyi("Multiply, multiply accumulate (register)"),
                    Nyi("Long multiply and divide")),
                SystemRegisterAccessAdvSimdFpu,
                SystemRegisterAccessAdvSimdFpu
            });
        }

        private static MaskDecoder CreateLoadStoreMultipleBranchDecoder()
        {
            var ldmStm = new LdmStmDecoder();
            var ldStExclusive = Nyi("Load/store exclusive, load-acquire/store-release, table branch");
            var ldStDual = Nyi("Load/store dual (post-indexed)");
            var ldStDualImm = Nyi("Load/store dual (literal and immediate)");
            var ldStDualPre = Nyi("Load/store dual (literal and immediate)");
            return Mask(5 + 16, 0xF, // Load/store (multiple, dual, exclusive) table branch");
                ldmStm,
                ldmStm,
                ldStExclusive,
                ldStDual,

                ldmStm,
                ldmStm,
                ldStExclusive,
                ldStDual,

                ldmStm,
                ldmStm,
                ldStDualImm,
                ldStDualPre,

                ldmStm,
                ldmStm,
                ldStDualImm,
                ldStDualPre);
        }

        private static Decoder CreateBranchesMiscControl()
        {
            var branch_T3_variant = Nyi("B - T3 variant");
            var branch_T4_variant = Nyi("B - T4 variant");
            var branch = Nyi("Branch");
            var nonbranch = Nyi("nonbranch");
            var mixedDecoders = Mask(7 + 16, 0xF,
                branch_T3_variant,
                branch_T3_variant,
                branch_T3_variant,
                branch_T3_variant,

                branch_T3_variant,
                branch_T3_variant,
                branch_T3_variant,
                nonbranch,

                branch_T3_variant,
                branch_T3_variant,
                branch_T3_variant,
                branch_T3_variant,

                branch_T3_variant,
                branch_T3_variant,
                branch_T3_variant,
                nonbranch);
            var bl = new BlDecoder();
            return Mask(12, 7,
                mixedDecoders,
                branch_T4_variant,
                mixedDecoders,
                branch_T4_variant,

                invalid,
                bl,
                invalid,
                bl);
        }
    }
}
