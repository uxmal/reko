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
    public class T32Disassembler : DisassemblerBase<Arm32InstructionNew>
    {
        private static Decoder[] decoders;
        private static Decoder invalid;

        private ImageReader rdr;
        private ThumbArchitecture arch;

        public T32Disassembler(ThumbArchitecture arch, ImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
        }

        public override Arm32InstructionNew DisassembleInstruction()
        {
            var addr = rdr.Address;
            if (!rdr.TryReadLeUInt16(out var wInstr))
                return null;
            var instr = decoders[wInstr >> 13].Decode(this, wInstr);
            instr.Address = addr;
            return instr;
        }

        private Arm32InstructionNew DecodeFormat16(uint wInstr, Opcode opcode, string format)
        {
            var ops = new List<MachineOperand>();
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
                case 'i':   // immediate value in bitfield
                    offset = this.ReadDecimal(format, ref i);
                    Expect(':', format, ref i);
                    size = this.ReadDecimal(format, ref i);
                    op = ImmediateOperand.Word32((wInstr >> offset) & ((1u << size) - 1));
                    break;
                case 'I':   // immediate 7-bit value shifted left 2.
                    op = ImmediateOperand.Int32(((int)wInstr & 0x7F) << 2);
                    break;
                case 'r':   // register specified by 3 bits (r0..r7)
                    offset = format[++i] - '0';
                    op = new RegisterOperand(arch.GetGpRegister(SBitfield(wInstr, offset, 3)));
                    break;
                case 'Q':   // register specified by 7:2..0:
                    op = new RegisterOperand(arch.GetGpRegister(
                        ((((int)wInstr >> 7) & 1) << 3) |
                        ((int)wInstr & 0x03)));
                    break;
                case 'R':   // 4-bit register.
                    offset = format[++i] - '0';
                    op = new RegisterOperand(arch.GetGpRegister(
                        ((int)wInstr >> offset) & 0x0F));
                    break;
                case '[':   // Memory access
                    ++i;
                    if (PeekAndDiscard('s', format, ref i))
                    {
                        baseReg = arch.StackRegister;
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
                        }
                        else
                        {
                            Expect('i', format, ref i);
                            offset = ReadDecimal(format, ref i);
                            Expect(':', format, ref i);
                            size = ReadDecimal(format, ref i);
                            offset = SBitfield(wInstr, offset, size);
                        }
                        Expect(':', format, ref i);
                        var dt = DataType(format, ref i);
                        Expect(']', format, ref i);
                        op = new MemoryOperand(dt)
                        {
                            BaseRegister = baseReg,
                            Offset = Constant.Int32(offset)
                        };
                    }
                    break;
                }
                ops.Add(op);
            }

            return new Arm32InstructionNew
            {
                opcode = opcode,
                op1 = ops.Count > 0 ? ops[0] : null,
                op2 = ops.Count > 1 ? ops[1] : null,
                op3 = ops.Count > 2 ? ops[2] : null,
                op4 = ops.Count > 3 ? ops[3] : null,
            };
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

        private abstract class Decoder
        {
            public abstract Arm32InstructionNew Decode(T32Disassembler dasm, uint wInstr);
        }

        private class MaskDecoder : Decoder
        {
            private Decoder[] decoders;
            private int shift;
            private uint mask;



            public MaskDecoder(int shift, uint mask, params Decoder [] decoders)
            {
                Debug.Assert(decoders == null || (int) (mask + 1) == decoders.Length);
                this.shift = shift;
                this.mask = mask;
                this.decoders = decoders;
            }

            public override Arm32InstructionNew Decode(T32Disassembler dasm, uint wInstr)
            {
                var op = (wInstr >> shift) & mask;
                return decoders[op].Decode(dasm, wInstr);
            }
        }

        private class LongDecoder : Decoder
        {
            public LongDecoder()
            {
            }

            public override Arm32InstructionNew Decode(T32Disassembler dasm, uint wInstr)
            {
                if (!dasm.rdr.TryReadLeUInt16(out var wHi))
                    return null;
                wInstr |= (uint)wHi << 16;
                throw new NotImplementedException();
            }
        }

        private class Instr16Decoder : Decoder
        {
            private Opcode opcode;
            private string format;

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

        private class Nyi : Decoder
        {
            private string message;

            public Nyi(string message)
            {
                this.message = message;
            }

            public override Arm32InstructionNew Decode(T32Disassembler dasm, uint wInstr)
            {
                throw new NotImplementedException($"A T32 decoder for the instruction {wInstr:X} ({message}) has not been implemented.");
            }
        }

        static T32Disassembler()
        {
            invalid = new Instr16Decoder(Opcode.Invalid, "");

            var dec16bit = Create16bitDecoders();
            var decB_T32 = new Nyi("B-T32 variant");
            var dec32bit = new LongDecoder();
            decoders = new Decoder[8] {
                dec16bit,
                dec16bit,
                dec16bit,
                dec16bit,

                dec16bit,
                dec16bit,
                dec16bit,
                new MaskDecoder(11, 0x03,
                    decB_T32,
                    dec32bit,
                    dec32bit,
                    dec32bit)
            };
        }

        private static MaskDecoder Create16bitDecoders()
        {
            var decAlu = CreateAluDecoder();
            var decDataLowRegisters = new MaskDecoder(1, 0, null);
            var decDataHiRegisters = new MaskDecoder(8, 0x03,
                new Nyi("add, adds"), // add, adds (register);
                new Nyi ("cmp (register)"), // cmp (register);
                new Instr16Decoder(Opcode.mov, "Q,R3"), // mov,movs
                invalid);
            var decLdrLiteral = new MaskDecoder(1, 0, null);
            var decLdStRegOffset = new MaskDecoder(1, 0, null);
            var decLdStWB = new MaskDecoder(1, 0, null);
            var decLdStHalfword = new MaskDecoder(1, 0, null);
            var decLdStSpRelative = new MaskDecoder(1, 0, null);
            var decAddPcSp = new MaskDecoder(1, 0, null);
            var decMisc16Bit = CreateMisc16bitDecoder();
            var decLdmStm = new MaskDecoder(1, 0, null);
            var decCondBranch = new MaskDecoder(1, 0, null);

            return new MaskDecoder(13, 0x07,
                decAlu,
                decAlu,
                new MaskDecoder(10, 0x07,
                    decDataLowRegisters,
                    new MaskDecoder(8, 3, // Special data and branch exchange 
                        decDataHiRegisters,
                        decDataHiRegisters,
                        decDataHiRegisters,
                        new MaskDecoder(7,1,
                            new Instr16Decoder(Opcode.bx, "r3"),
                            new Instr16Decoder(Opcode.blx, "r3"))),
                    decLdrLiteral,
                    decLdrLiteral,

                    decLdStRegOffset,
                    decLdStRegOffset,
                    decLdStRegOffset,
                    decLdStRegOffset),
                decLdStWB,

                new MaskDecoder(12, 0x01,
                    decLdStHalfword,
                    new MaskDecoder(11, 0x01,   // load store SP-relative
                        new Instr16Decoder(Opcode.str, "r8,[s,I0:8:w]"),
                        new Instr16Decoder(Opcode.ldr, "r8,[s,I0:8:w]"))),
                new MaskDecoder(12, 0x01,
                    decAddPcSp,
                    decMisc16Bit),
                new MaskDecoder(12, 0x01,
                    decLdmStm,
                    decCondBranch),
                new Instr16Decoder(Opcode.Invalid, ""));
        }

        private static Decoder CreateAluDecoder()
        {
            var decAddSub3 = new MaskDecoder(3, 0, null);
            var decAddSub3Imm = new MaskDecoder(3, 0, null);
            var decMovMovs = new MaskDecoder(3, 0, null);
            var decAddSub = new MaskDecoder(3, 0, null);
            return new MaskDecoder(10, 0xF,
                decMovMovs,
                decMovMovs,
                decMovMovs,
                decMovMovs,

                decMovMovs,
                decMovMovs,
                new MaskDecoder(9, 1,
                    new Instr16Decoder(Opcode.add, "r0,r3,r6"),
                    new Instr16Decoder(Opcode.sub, "r0,r3,r6")),
                new MaskDecoder(9, 1,
                    new Instr16Decoder(Opcode.add, "r0,r3,i6:3"),
                    new Instr16Decoder(Opcode.sub, "r0,r3,i6:3")),
                decAddSub,
                decAddSub,
                decAddSub,
                decAddSub,

                decAddSub,
                decAddSub,
                decAddSub,
                decAddSub);
        }

        private static Decoder CreateMisc16bitDecoder()
        {
            return new MaskDecoder(8, 0xF,
                new MaskDecoder(7, 1,  // Adjust SP
                    new Instr16Decoder(Opcode.add, "sp,I"),
                    new Instr16Decoder(Opcode.sub, "sp,I")),

                invalid,
                new MaskDecoder(3, 0, null), // Extend
                invalid,

                invalid,
                invalid,
                new MaskDecoder(5, 0x7,
                    new Nyi("SETPAN"),        // SETPAN
                    invalid,
                    new Nyi("Change processor state"),        // Change processor state
                    new Nyi("WUT"),

                    invalid,
                    invalid,
                    invalid,
                    invalid),
                invalid,

                invalid,
                invalid,
                new MaskDecoder(6, 0x3,
                    new Nyi("Reverse bytes"),        // reverse bytes
                    new Nyi("Reverse bytes"),        // reverse bytes
                    new Instr16Decoder(Opcode.hlt, ""),
                    new Nyi("Reverse bytes")),        // reverse bytes),
                invalid,

                invalid,
                invalid,
                new Instr16Decoder(Opcode.bkpt, ""),
                new Nyi("Hints"));            // hints
        }
    }
}
