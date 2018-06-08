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
            bool writeback = false;
            Opcode shiftType = Opcode.Invalid;
            MachineOperand shiftValue = null;
            uint n;
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
                    }
                    else // Signed immediate (in bitfields)
                    {
                        n = ReadBitfields(wInstr, format, ref i);
                        op = ImmediateOperand.Int32((int)n);
                    }
                    break;
                case 'S':   // shift amount in bitfield.
                    ++i;
                    if (PeekAndDiscard('r', format, ref i))
                    {
                        // 'Sr' = rotate
                        n = ReadBitfields(wInstr, format, ref i);
                        shiftType = Opcode.ror;
                        shiftValue = ImmediateOperand.Int32((int)n);
                        continue;
                    }
                    else
                    {
                        offset = this.ReadDecimal(format, ref i);
                        Expect(':', format, ref i);
                        size = this.ReadDecimal(format, ref i);
                        op = ImmediateOperand.Int32(SBitfield(wInstr, offset, size));
                    }
                    break;
                case 'i':   // immediate value in bitfield(s)
                    ++i;
                    n = ReadBitfields(wInstr, format, ref i);
                    if (PeekAndDiscard('h', format, ref i))
                    {
                        op = ImmediateOperand.Word16((ushort)n);
                    }
                    else
                    {
                        op = ImmediateOperand.Word32(n);
                    }
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
                        // Only 3 bits for register
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
                            // Offset, shifted by 2
                            offset = ReadDecimal(format, ref i);
                            Expect(':', format, ref i);
                            size = ReadDecimal(format, ref i);
                            offset = SBitfield(wInstr, offset, size) << 2;
                            add = true;
                        }
                        else
                        {
                            // Unshifted offset.
                            Expect('i', format, ref i);
                            offset = ReadDecimal(format, ref i);
                            Expect(':', format, ref i);
                            size = ReadDecimal(format, ref i);
                            offset = SBitfield(wInstr, offset, size);
                            add = true;
                        }
                        Expect(':', format, ref i);
                        var dt = DataType(format, ref i);
                        var preindex = false;
                        if (PeekAndDiscard('x', format, ref i))
                        {
                            // Indexing bits in P=10, W=8
                            // Negative bit in U=9
                            preindex = SBitfield(wInstr, 10, 1) != 0;
                            add = (SBitfield(wInstr, 9, 1) != 0);
                            writeback = SBitfield(wInstr, 8, 1) != 0;
                        }
                        else if (PeekAndDiscard('X', format, ref i))
                        {
                            preindex = SBitfield(wInstr, 24, 1) != 0;
                            add = SBitfield(wInstr, 23, 1) != 0;
                            writeback = SBitfield(wInstr, 21, 1) != 0;
                        }

                        Expect(']', format, ref i);
                        op = new MemoryOperand(dt)
                        {
                            BaseRegister = baseReg,
                            Offset = Constant.Int32(offset),
                            PreIndex = preindex,
                            Add = add,
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
                case 'c':  // Condition code
                    ++i;
                    offset = ReadDecimal(format, ref i);
                    cc = (ArmCondition) SBitfield(wInstr, offset, 4);
                    --i;
                    continue;
                case 'C':   // Coprocessor
                    ++i;
                    switch (format[i])
                    {
                    case 'P':   // Coprocessor #
                        ++i;
                        offset = ReadDecimal(format, ref i);
                        op = Coprocessor(wInstr, offset);
                        break;
                    case 'R':   // Coprocessor register
                        ++i;
                        offset = ReadDecimal(format, ref i);
                        op = CoprocessorRegister(wInstr, offset);
                        break;
                    default:
                        throw new NotImplementedException($"Unknown format specifier C{format[i]} when decoding {opcode}.");
                    }
                    break;
                default:
                    throw new NotImplementedException($"Unknown format specifier {format[i]} when decoding {opcode}.");
                }
                ops.Add(op);
            }

            return new Arm32InstructionNew
            {
                opcode = opcode,
                condition = cc,
                UpdateFlags = updateFlags,
                ops = ops.ToArray(),
                Writeback = writeback,
                ShiftType = shiftType,
                ShiftValue = shiftValue,
            };
        }

        /// <summary>
        /// Concatenate the value in 1 or more bit fields and then optionally
        /// shift it to the left by a given amount.
        /// </summary>
        /// <param name="wInstr"></param>
        /// <param name="format"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        private uint ReadBitfields(uint wInstr, string format, ref int i)
        {
            uint n = 0u;
            do
            {
                var offset = this.ReadDecimal(format, ref i);
                Expect(':', format, ref i);
                var size = this.ReadDecimal(format, ref i);
                n = (n << size) | ((wInstr >> offset) & ((1u << size) - 1));
            } while (PeekAndDiscard(':', format, ref i));
            if (PeekAndDiscard('<', format, ref i))
            {
                var shift = this.ReadDecimal(format, ref i);
                n <<= shift;
            }
            return n;
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

        private RegisterOperand Coprocessor(uint wInstr, int bitPos)
        {
            var cp = Registers.Coprocessors[SBitfield(wInstr, bitPos, 4)];
            return new RegisterOperand(cp);
        }

        private RegisterOperand CoprocessorRegister(uint wInstr, int bitPos)
        {
            var cr = Registers.CoprocessorRegisters[SBitfield(wInstr, bitPos, 4)];
            return new RegisterOperand(cr);
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
            case 'd': return PrimitiveType.Word64;
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
                    ops = new MachineOperand[] { AddressOperand.Create(dasm.addr + off) }
                };
            }
        }

        // Decodes 16-bit LDM* STM* instructions
        private class LdmStmDecoder16 : Decoder
        {
            public override Arm32InstructionNew Decode(T32Disassembler dasm, uint wInstr)
            {
                var rn = Registers.GpRegs[SBitfield(wInstr, 8, 3)];
                var registers = (byte)wInstr;
                var st = SBitfield(wInstr, 11, 1) == 0;
                var w = st || (registers & (1 << rn.Number)) == 0;
                return new Arm32InstructionNew
                {
                    opcode = st
                        ? Opcode.stm
                        : Opcode.ldm,
                    Writeback = w,
                    ops = new MachineOperand[] {
                            new RegisterOperand(rn),
                            new MultiRegisterOperand(PrimitiveType.Word16, (registers)) }
                };
            }
        }

        // Decodes 32-bit LDM* STM* instructions
        private class LdmStmDecoder32 : Decoder
        {
            public override Arm32InstructionNew Decode(T32Disassembler dasm, uint wInstr)
            {
                var rn = Registers.GpRegs[SBitfield(wInstr, 16, 4)];
                var registers = (ushort)wInstr;
                var w = SBitfield(wInstr, 16 + 5, 1) != 0;
                var l = SBitfield(wInstr, 16 + 4, 1);
                // writeback
                if (rn == Registers.sp)
                {
                    return new Arm32InstructionNew
                    {
                        opcode = l != 0 ? Opcode.pop_w : Opcode.push_w,
                        Writeback = w,
                        ops = new MachineOperand[] { new MultiRegisterOperand(PrimitiveType.Word16, (registers)) }
                    };
                }
                else
                {
                    return new Arm32InstructionNew
                    {
                        opcode = l != 0 ? Opcode.stm : Opcode.stm,
                        Writeback = w,
                        ops = new MachineOperand[] {
                            new RegisterOperand(rn),
                            new MultiRegisterOperand(PrimitiveType.Word16, (registers))
                        }
                    };

                }
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
                if (instr.ops[2] is ImmediateOperand imm && imm.Value.IsIntegerZero)
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
                ImmediateOperand opMsb;
                if (instr.ops.Length > 3)
                {
                    opMsb = (ImmediateOperand)instr.ops[3];
                    opLsb = (ImmediateOperand)instr.ops[2];
                }
                else
                {
                    opMsb = (ImmediateOperand)instr.ops[2];
                    opLsb = (ImmediateOperand)instr.ops[1];
                }
                var opWidth = ImmediateOperand.Word32(opMsb.Value.ToInt32() - opLsb.Value.ToInt32() + 1);
                if (instr.ops.Length > 3)
                    instr.ops[3] = opWidth;
                else
                    instr.ops[2] = opWidth;
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
                Instr(Opcode.add, "r8,sp,s0:8<2"));
            var decMisc16Bit = CreateMisc16bitDecoder();
            var decLdmStm = new LdmStmDecoder16();
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

                Instr(Opcode.orr, ".r0,r3"),
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
                    Instr(Opcode.add, "sp,s0:7<2"),
                    Instr(Opcode.sub, "sp,s0:7<2")),

                cbnzCbz,
                Mask(6, 3,
                    Instr(Opcode.sxth, "r0,r3"),
                    Instr(Opcode.sxtb, "r0,r3"),
                    Instr(Opcode.uxth, "r0,r3"),
                    Instr(Opcode.uxtb, "r0,r3")),
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
                    Instr(Opcode.rev, "r0,r3"),
                    Instr(Opcode.rev, "r0,r3"),
                    Instr(Opcode.hlt, ""),
                    Instr(Opcode.rev, "r0,r3")),
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
                    Instr(Opcode.mov, "R8,M")),
                Select(wInstr => SBitfield(wInstr, 16, 4) != 0xF,
                    Instr(Opcode.orr, ".R8,R16,M"),
                    Instr(Opcode.mov, ".R8,M")),
                Select(wInstr => SBitfield(wInstr, 16, 4) != 0xF,
                    Instr(Opcode.orn, "R8,R16,M"),
                    Instr(Opcode.mvn, "R8,M")),
                Select(wInstr => SBitfield(wInstr, 16, 4) != 0xF,
                    Instr(Opcode.orn, ".R8,R16,M"),
                    Instr(Opcode.mvn, ".R8,M")),
                // 8
                Instr(Opcode.eor, "R8,R16,M"),
                Select(wInstr => SBitfield(wInstr, 8, 4) != 0xF,
                    Instr(Opcode.eor, ".R8,R16,M"),
                    Instr(Opcode.teq, ".R8,M")),
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
                        Mask(10+16, 1,
                            Instr(Opcode.add, "R8,R16,i26:1:12:3:0:8"),
                            Instr(Opcode.add, ".R8,R16,i26:1:12:3:0:8")),
                        Mask(17, 1,
                            Instr(Opcode.add, "R8,R16,i26:1:12:3:0:8"),
                            Nyi("ADR - T3"))),
                    invalid),
                Mask(5 + 16, 1,
                    invalid,
                    Select(w => (SBitfield(w, 16, 4) & 0xD) != 0xD,
                        Mask(10 + 16, 1,
                            Instr(Opcode.sub, "R8,R16,i26:1:12:3:0:8"),
                            Instr(Opcode.sub, ".R8,R16,i26:1:12:3:0:8")),
                        Mask(17, 1,
                            Instr(Opcode.sub, "R8,R16,i26:1:12:3:0:8"),
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

            var MoveWide16BitImm = Mask(7 + 16, 1,
                Instr(Opcode.mov, "R8,i16:4:26:1:12:3:0:8"),
                Instr(Opcode.movt, "R8,i16:4:26:1:12:3:0:8h"));

            var DataProcessingPlainImm = Mask(8 + 16, 1,
                Mask(5 + 16, 3,
                    DataProcessingSimpleImm,
                    DataProcessingSimpleImm,
                    MoveWide16BitImm,
                    invalid),
                SaturateBitfield);

            var LoadStoreSignedPositiveImm = Select(w => SBitfield(w, 12, 4) != 0xF,
                Mask(5 + 16, 3,
                    Instr(Opcode.ldrsb, "R12,[R16,i0:12:Bx]"),
                    Instr(Opcode.ldrsh, "R12,[R16,i0:12:Hx]"),
                    invalid,
                    invalid),
                Mask(5 + 16, 3,
                    Nyi("PLI"),
                    Instr(Opcode.nop, ""),
                    invalid,
                    invalid));   // reserved hint

            var LoadStoreSignedImmediatePostIndexed = Mask(5 + 16, 3,
                Instr(Opcode.ldrsb, "R12,[R16,i0:8:Bx]"),
                Instr(Opcode.ldrsh, "R12,[R16,i0:8:Hx]"),
                invalid,
                invalid);

            var LoadStoreUnsignedImmediatePostIndexed = Mask(4 + 16, 7,
                Instr(Opcode.strb, "R12,[R16,i0:8:bx]"),
                Instr(Opcode.ldrb, "R12,[R16,i0:8:bx]"),
                Instr(Opcode.strh, "R12,[R16,i0:8:hx]"),
                Instr(Opcode.ldrh, "R12,[R16,i0:8:hx]"),
                Instr(Opcode.str, "R12,[R16,i0:8:wx]"),
                Instr(Opcode.ldr, "R12,[R16,i0:8:wx]"),
                invalid,
                invalid);

            var LoadStoreUnsignedPositiveImm = Mask(4 + 16, 7,
                Instr(Opcode.strb, "R12,[R16,i0:12:b]"),
                Select(w => SBitfield(w, 12, 4) != 0xF,
                    Instr(Opcode.ldrb, "*immediate"),
                    Nyi("PLD,PLDW immediate preloadread")),
                Instr(Opcode.strh, "*immediate"),
                Select(w => SBitfield(w, 12, 4) != 0xF,
                    Instr(Opcode.ldrh, "*immediate"),
                    Nyi("PLD,PLDW immediate preloadwrite")),
                // 4
                Instr(Opcode.str, "R12,[R16,i0:12:w]"),
                Instr(Opcode.ldr, "R12,[R16,i0:12:w]"),
                invalid,
                invalid);

            var LoadStoreUnsignedImmediatePreIndexed = Mask(4 + 16, 7,
                Instr(Opcode.strb, "R12,[R16,i0:8:bx]"),
                Instr(Opcode.ldrb, "R12,[R16,i0:8:bx]"),
                Instr(Opcode.strh, "R12,[R16,i0:8:hx]"),
                Instr(Opcode.ldrh, "R12,[R16,i0:8:hx]"),
                Instr(Opcode.str, "R12,[R16,i0:8:wx]"),
                Instr(Opcode.str, "R12,[R16,i0:8:wx]"),
                invalid,
                invalid);

            var LoadStoreUnsignedUnprivileged = Mask(4 + 16, 7,
                Instr(Opcode.strbt, "R12,[R16,i0:8:b]"),
                Instr(Opcode.ldrbt, "R12,[R16,i0:8:b]"),
                Instr(Opcode.strht, "R12,[R16,i0:8:h]"),
                Instr(Opcode.ldrht, "R12,[R16,i0:8:h]"),
                Instr(Opcode.strt, "R12,[R16,i0:8:w]"),
                Instr(Opcode.ldrt, "R12,[R16,i0:8:w]"),
                invalid,
                invalid);

            var LoadStoreSingle = Mask(7 + 16, 3,
                Select(w => SBitfield(w, 16, 4) != 0xF,
                    Mask(10, 3,
                        Select(w => SBitfield(w, 6, 6) == 0,
                            Nyi("LoadStoreUnsignedRegisterOffset"),
                            invalid),
                        invalid,
                        Select(w => SBitfield(w, 8, 1) == 0,
                            invalid,
                            LoadStoreUnsignedImmediatePostIndexed),
                        Mask(8, 3,
                            Nyi("LoadStoreUnsignedNegativeImm"),
                            LoadStoreUnsignedImmediatePreIndexed,
                            LoadStoreUnsignedUnprivileged,
                            LoadStoreUnsignedImmediatePreIndexed)),
                    Nyi("LoadUnsignedLiteral")),
                Select(w => SBitfield(w, 16, 4) != 0xF,
                    LoadStoreUnsignedPositiveImm,
                    Nyi("LoadUnsignedLiteral")),
                Select(w => SBitfield(w, 16, 4) != 0xF,
                    Mask(10, 3,
                        Select(w => SBitfield(w, 6, 6) == 0,
                            Nyi("LoadStoreSignedRegisterOffset"),
                            invalid),
                        invalid,
                        Select(w => SBitfield(w, 8, 1) == 0,
                            invalid,
                            LoadStoreSignedImmediatePostIndexed),
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
            var SystemRegister32bitMove = Mask(12 + 16, 1, 
                Mask(4 + 16, 1,
                    Instr(Opcode.mcr, "CP8,i21:3,R12,CR16,CR0,i5:3"),
                    Instr(Opcode.mrc, "CP8,i21:3,R12,CR16,CR0,i5:3")),
                invalid);
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

            var RegisterExtends = Mask(4 + 16, 7,
                Select(w => SBitfield(w, 16, 4) != 0xF,
                    Instr(Opcode.sxtah, "R8,R16,R0,Sr4:2<3"),
                    Instr(Opcode.sxth, "R8,R0,Sr4:2<3")),
                Select(w => SBitfield(w, 16, 4) != 0xF,
                    Instr(Opcode.uxtah, "R8,R16,R0,Sr4:2<3"),
                    Instr(Opcode.uxth, "R8,R0,Sr4:2<3")),
                Select(w => SBitfield(w, 16, 4) != 0xF,
                    Instr(Opcode.sxtab16, "R8,R16,R0,Sr4:2<3"),
                    Instr(Opcode.sxtb16, "R8,R0,Sr4:2<3")),
                Select(w => SBitfield(w, 16, 4) != 0xF,
                    Instr(Opcode.uxtab16, "R8,R16,R0,Sr4:2<3"),
                    Instr(Opcode.uxtb16, "R8,R0,Sr4:2<3")),

                Select(w => SBitfield(w, 16, 4) != 0xF,
                    Instr(Opcode.sxtab, "R8,R16,R0,Sr4:2<3"),
                    Instr(Opcode.sxtb, "R8,R0,Sr4:2<3")),
                Select(w => SBitfield(w, 16, 4) != 0xF,
                    Instr(Opcode.uxtab, "R8,R16,R0,Sr4:2<3"),
                    Instr(Opcode.uxtb, "R8,R0,Sr4:2<3")),
                invalid,
                invalid);

            var ParallelAddSub = Mask(4 + 16, 7,
                Mask(4, 7,
                    Instr(Opcode.sadd8, "R8,R16,R0"),
                    Instr(Opcode.qadd8, "R8,R16,R0"),
                    Instr(Opcode.shadd8, "R8,R16,R0"),
                    invalid,
                    Instr(Opcode.uadd8, "R8,R16,R0"),
                    Instr(Opcode.uqadd8, "R8,R16,R0"),
                    Instr(Opcode.uhadd8, "R8,R16,R0"),
                    invalid),
                Mask(4, 7,
                    Instr(Opcode.sadd16, "R8,R16,R0"),
                    Instr(Opcode.qadd16, "R8,R16,R0"),
                    Instr(Opcode.shadd16, "R8,R16,R0"),
                    invalid,
                    Instr(Opcode.uadd16, "R8,R16,R0"),
                    Instr(Opcode.uqadd16, "R8,R16,R0"),
                    Instr(Opcode.uhadd16, "R8,R16,R0"),
                    invalid),
                Mask(4, 7,
                    Instr(Opcode.sasx, "R8,R16,R0"),
                    Instr(Opcode.qasx, "R8,R16,R0"),
                    Instr(Opcode.shasx, "R8,R16,R0"),
                    invalid,
                    Instr(Opcode.uasx, "R8,R16,R0"),
                    Instr(Opcode.uqasx, "R8,R16,R0"),
                    Instr(Opcode.uhasx, "R8,R16,R0"),
                    invalid),
                invalid,

                Mask(4, 7,
                    Instr(Opcode.ssub8, "R8,R16,R0"),
                    Instr(Opcode.qsub8, "R8,R16,R0"),
                    Instr(Opcode.shsub8, "R8,R16,R0"),
                    invalid,
                    Instr(Opcode.usub8, "R8,R16,R0"),
                    Instr(Opcode.uqsub8, "R8,R16,R0"),
                    Instr(Opcode.uhsub8, "R8,R16,R0"),
                    invalid),
                Mask(4, 7,
                    Instr(Opcode.ssub16, "R8,R16,R0"),
                    Instr(Opcode.qsub16, "R8,R16,R0"),
                    Instr(Opcode.shsub16, "R8,R16,R0"),
                    invalid,
                    Instr(Opcode.usub16, "R8,R16,R0"),
                    Instr(Opcode.uqsub16, "R8,R16,R0"),
                    Instr(Opcode.uhsub16, "R8,R16,R0"),
                    invalid),
                Mask(4, 7,
                    Instr(Opcode.ssax, "R8,R16,R0"),
                    Instr(Opcode.qsax, "R8,R16,R0"),
                    Instr(Opcode.shsax, "R8,R16,R0"),
                    invalid,
                    Instr(Opcode.usax, "R8,R16,R0"),
                    Instr(Opcode.uqsax, "R8,R16,R0"),
                    Instr(Opcode.uhsax, "R8,R16,R0"),
                    invalid),
                invalid);

            var DataProcessingRegister = Mask(7 + 16, 1,
                Mask(7, 1,
                    Select(w => SBitfield(w, 4, 4) == 0,
                        Nyi("MovMovsRegisterShiftedRegister"),
                        invalid),
                    RegisterExtends),
                Mask(6, 3,
                    ParallelAddSub,
                    ParallelAddSub,
                    DataProcessing2srcRegs,
                    invalid));

            var MultiplyAbsDifference = Mask(4 + 16, 7,
                Mask(4, 3,
                    Select(w => SBitfield(w, 12, 4) != 0xF,
                        Instr(Opcode.mla, "R8,R16,R0,R12"),
                        Instr(Opcode.mul, "R8,R16,R0")),
                    Instr(Opcode.mls, "R8,R16,R0,R12"),
                    invalid,
                    invalid),
                Mask(4, 3,      // op1 = 0b001
                    Select(w => SBitfield(w, 12, 4) != 0xF,
                        Instr(Opcode.smlabb, "R8,R16,R0,R12"),
                        Instr(Opcode.smulbb, "R8,R16,R0")),
                    Select(w => SBitfield(w, 12, 4) != 0xF,
                        Instr(Opcode.smlabt, "R8,R16,R0,R12"),
                        Instr(Opcode.smulbt, "R8,R16,R0")),
                    Select(w => SBitfield(w, 12, 4) != 0xF,
                        Instr(Opcode.smlatb, "R8,R16,R0,R12"),
                        Instr(Opcode.smultb, "R8,R16,R0")),
                    Select(w => SBitfield(w, 12, 4) != 0xF,
                        Instr(Opcode.smlatt, "R8,R16,R0,R12"),
                        Instr(Opcode.smultt, "R8,R16,R0"))),
                Mask(4, 3,      // op1 = 0b010
                    Select(w => SBitfield(w, 12, 4) != 0xF,
                        Instr(Opcode.smlad, "R8,R16,R0,R12"),
                        Instr(Opcode.smuad, "R8,R16,R0")),
                    Select(w => SBitfield(w, 12, 4) != 0xF,
                        Instr(Opcode.smladx, "R8,R16,R0,R12"),
                        Instr(Opcode.smuadx, "R8,R16,R0")),
                    invalid,
                    invalid),
                Mask(4, 3,      // op1 = 0b011
                    Select(w => SBitfield(w, 12, 4) != 0xF,
                        Instr(Opcode.smlawb, "*"),
                        Instr(Opcode.smulwb, "*")),
                    Select(w => SBitfield(w, 12, 4) != 0xF,
                        Instr(Opcode.smlawt, "*"),
                        Instr(Opcode.smulwt, "*")),
                    invalid,
                    invalid),
                Mask(4, 3,      // op1 = 0b100
                    Select(w => SBitfield(w, 12, 4) != 0xF,
                        Instr(Opcode.smlsd, "*"),
                        Instr(Opcode.smusd, "*")),
                    Select(w => SBitfield(w, 12, 4) != 0xF,
                        Instr(Opcode.smlsdx, "*"),
                        Instr(Opcode.smusdx, "*")),
                    invalid,
                    invalid),
                Mask(4, 3,      // op1 = 0b101
                    Select(w => SBitfield(w, 12, 4) != 0xF,
                        Instr(Opcode.smmla, "*"),
                        Instr(Opcode.smmul, "*")),
                    Select(w => SBitfield(w, 12, 4) != 0xF,
                        Instr(Opcode.smmlar, "*"),
                        Instr(Opcode.smmulr, "*")),
                    invalid,
                    invalid),
                Mask(4, 3,      // op1 = 0b110
                    Instr(Opcode.smmls, "*"),
                    Instr(Opcode.smmlsr, "*"),
                    invalid,
                    invalid),
                Mask(4, 3,      // op1 = 0b111
                    Select(w => SBitfield(w, 12, 4) != 0xF,
                        Instr(Opcode.usada8, "*"),
                        Instr(Opcode.usad8, "*")),
                    invalid,
                    invalid,
                    invalid));

            var MultiplyRegister = Select(w => SBitfield(w, 6, 2) == 0,
                MultiplyAbsDifference,
                invalid);

            var LongMultiplyDivide = Mask(4 + 16, 7,
                Select(w => SBitfield(w, 4, 4) != 0,
                    invalid,
                    Instr(Opcode.smull, "R12,R8,R16,R0")),
                Select(w => SBitfield(w, 4, 4) != 0xF,
                    invalid,
                    Instr(Opcode.sdiv, "R8,R16,R0")),
                Select(w => SBitfield(w, 4, 4) != 0,
                    invalid,
                    Instr(Opcode.umull, "R12,R8,R16,R0")),
                Select(w => SBitfield(w, 4, 4) != 0xF,
                    invalid,
                    Instr(Opcode.udiv, "R8,R16,R0")),
                // 4
                Mask(4, 0xF,
                    Instr(Opcode.smlal, "R12,R8,R16,R0"),
                    invalid,
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    Instr(Opcode.smlalbb, "R12,R8,R16,R0"),
                    Instr(Opcode.smlalbt, "R12,R8,R16,R0"),
                    Instr(Opcode.smlaltb, "R12,R8,R16,R0"),
                    Instr(Opcode.smlaltt, "R12,R8,R16,R0"),

                    Instr(Opcode.smlald, "R12,R8,R16,R0"),
                    Instr(Opcode.smlaldx, "R12,R8,R16,R0"),
                    invalid,
                    invalid),
                Mask(4, 0x0F,
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

                    Instr(Opcode.smlsld, "R12,R8,R16,R0"),
                    Instr(Opcode.smlsldx, "R12,R8,R16,R0"),
                    invalid,
                    invalid),
                Mask(4, 0x0F,   // op1 = 0b110
                    Instr(Opcode.umlal, "R12,R8,R16,R0"),
                    invalid,
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    Instr(Opcode.umaal, "*"),
                    invalid,

                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    invalid,
                    invalid),
                invalid);   // op1 = 0b111

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
                    MultiplyRegister,
                    LongMultiplyDivide),
                SystemRegisterAccessAdvSimdFpu,
                SystemRegisterAccessAdvSimdFpu
            });
        }

        private static MaskDecoder CreateLoadStoreMultipleBranchDecoder()
        {
            var ldmStm32 = new LdmStmDecoder32();
            var ldStExclusive = Nyi("Load/store exclusive, load-acquire/store-release, table branch");
            var ldStDual = Nyi("Load/store dual (post-indexed)");
            var ldStDualImm = Mask(4 + 16, 1,
                Instr(Opcode.strd, "R12,R8,[R16,I0:8:dX]"),
                Instr(Opcode.ldrd, "R12,R8,[R16,I0:8:dX]"));
            var ldStDualPre = Nyi("Load/store dual (immediate pre-indexed)");
            return Mask(5 + 16, 0xF, // Load/store (multiple, dual, exclusive) table branch");
                ldmStm32,
                ldmStm32,
                ldStExclusive,
                ldStDual,

                ldmStm32,
                ldmStm32,
                ldStExclusive,
                ldStDual,

                ldmStm32,
                ldmStm32,
                ldStDualImm,
                ldStDualPre,

                ldmStm32,
                ldmStm32,
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
