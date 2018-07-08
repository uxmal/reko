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

namespace Reko.Arch.Arm.AArch64
{
    public partial class AArch64Disassembler : DisassemblerBase<AArch64Instruction>
    {
        private static readonly Decoder rootDecoder;
        private static readonly Decoder invalid;

        private Arm64Architecture arch;
        private EndianImageReader rdr;
        private Address addr;

        public AArch64Disassembler(Arm64Architecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
        }

        public override AArch64Instruction DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadLeUInt32(out var wInstr))
                return null;
            var instr = rootDecoder.Decode(wInstr, this);
            instr.Address = addr;
            instr.Length = 4;
            return instr;
        }

        private AArch64Instruction Decode(uint wInstr, Opcode opcode, string format)
        {
            int i = 0;
            RegisterStorage reg;
            Opcode shiftCode = Opcode.Invalid;
            MachineOperand shiftAmount = null;
            int n;
            var ops = new List<MachineOperand>();
            while (i < format.Length)
            {
                switch (format[i++])
                {
                case ',':
                case ' ':
                    break;
                case 'W':
                    // 32-bit register.
                    n = ReadSignedBitfield(wInstr, format, ref i);
                    reg = Registers.GpRegs32[n];
                    ops.Add(new RegisterOperand(reg));
                    break;
                case 'X':
                    // 64-bit register.
                    n = ReadSignedBitfield(wInstr, format, ref i);
                    reg = Registers.GpRegs64[n];
                    ops.Add(new RegisterOperand(reg));
                    break;
                case 'U':
                    ImmediateOperand op = DecodeImmediateOperand(wInstr, format, ref i);
                    ops.Add(op);
                    break;
                case 'J':
                    // Jump displacement from address of current instruction
                    n = ReadSignedBitfield(wInstr, format, ref i);
                    n = (int)Bits.SignExtend(wInstr, 26);
                    AddressOperand aop = AddressOperand.Create(addr + (n << 2));
                    ops.Add(aop);
                    break;
                case 's':
                    // Shift type
                    n = ReadSignedBitfield(wInstr, format, ref i);
                    switch (n)
                    {
                    case 1:
                        shiftCode = Opcode.lsl;
                        shiftAmount = ImmediateOperand.Int32(12);
                        break;
                    }
                    break;
                default:
                    NotYetImplemented($"Unknown format character '{format[i - 1]}' decoding {opcode}", wInstr);
                    return Invalid();
                }
            }
            var instr = new AArch64Instruction
            {
                opcode = opcode,
                ops = ops.ToArray(),
                shiftCode = shiftCode,
                shiftAmount = shiftAmount
            };
            return instr;
        }

        private ImmediateOperand DecodeImmediateOperand(uint wInstr, string format, ref int i)
        {
            // Unsigned immediate field.
            ulong? imm;
            DataType dt;
            if (PeekAndDiscard('l', format, ref i))
            {
                // Logical immediates have really complex formats.
                var offset = ReadNumber(format, ref i);
                dt = ReadBitSize(format, ref i);
                imm = DecodeLogicalImmediate(wInstr >> offset, dt.BitSize);
            }
            else
            {
                imm = (uint)ReadSignedBitfield(wInstr, format, ref i);
                dt = ReadBitSize(format, ref i);
            }
            if (imm == null)
                return null;
            var op = new ImmediateOperand(Constant.Create(dt, imm.Value));
            return op;
        }

        private DataType ReadBitSize(string format, ref int i)
        {
            switch (format[i++])
            {
            case 'h': return PrimitiveType.Word16;
            case 'w': return PrimitiveType.Word32;
            case 'l': return PrimitiveType.Word64;
            }
            throw new NotImplementedException();
        }

        /// Decode a logical immediate value in the form
        /// "N:immr:imms" (where the immr and imms fields are each 6 bits) into the
        /// integer value it represents with regSize bits.
        private ulong? DecodeLogicalImmediate(uint val, int bitSize)
        {
            // Extract the N, imms, and immr fields.
            uint N = (val >> 12) & 1;
            uint immr = (val >> 6) & 0x3f;
            uint imms = val & 0x3f;

            if (bitSize != 64 && N == 1)
                return null;
            int len = 6 - Bits.CountLeadingZeros(7, (N << 6) | (~imms & 0x3f));
            if (len < 0)
                return null;
            int size = 1 << len;
            int R = (int) (immr & (size - 1));
            int S = (int) (imms & (size - 1));
            if (S == size - 1)
                return null;
            ulong pattern = (1UL << (S + 1)) -1;
            pattern = Bits.RotateR(size, pattern, R);

            // Replicate the pattern to fill the regSize.
            while (size != bitSize)
            {
                pattern |= pattern << size;
                size *= 2;
            }
            return pattern;
        }

        private int ReadSignedBitfield(uint word, string format, ref int i)
        {
            uint n = 0;
            do
            {
                int shift = ReadNumber(format, ref i);
                Expect(':', format, ref i);
                int maskSize = ReadNumber(format, ref i);
                uint mask = (1u << maskSize) - 1u;
                n = (n << maskSize) | ((word >> shift) & mask);
            } while (PeekAndDiscard(':', format, ref i));
            return  (int)n;
        }

        private void Expect(char c, string format, ref int i)
        {
            if (format[i] != c)
                throw new InvalidOperationException();
            ++i;
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

        private int ReadNumber(string format, ref int i)
        {
            int n = 0;
            while (i < format.Length)
            {
                char c = format[i];
                if (!Char.IsDigit(c))
                    break;
                n = n * 10 + (c - '0');
                ++i;
            }
            return n;
        }

        private static Decoder Instr(Opcode opcode, string format)
        {
            return new InstrDecoder(opcode, format);
        }

        private static Decoder Mask(int pos, uint mask, params Decoder[] decoders)
        {
            return new MaskDecoder(pos, mask, decoders);
        }

        private static Decoder Sparse(int pos, uint mask, Decoder @default, params (uint, Decoder)[] decoders)
        {
            return new SparseMaskDecoder(pos, mask, decoders.ToDictionary(k => k.Item1, v => v.Item2), @default);
        }

        private static Decoder Select(string bitfields, Predicate<int> predicate, Decoder trueDecoder, Decoder falseDecoder)
        {
            return new SelectDecoder(bitfields, predicate, trueDecoder, falseDecoder);
        }

        private static NyiDecoder Nyi(string str)
        {
            return new NyiDecoder(str);
        }

        private AArch64Instruction NotYetImplemented(string message, uint wInstr)
        {
            Console.WriteLine($"An AArch64 decoder for the instruction {wInstr:X} ({message}) has not been implemented yet.");
            Console.WriteLine("[Test]");
            Console.WriteLine($"public void ThumbDis_{wInstr:X}()");
            Console.WriteLine("{");
            if (wInstr > 0xFFFF)
            {
                Console.WriteLine($"    Given_Instructions(0x{wInstr >> 16:X4}, 0x{wInstr & 0xFFFF:X4});");
            }
            else
            {
                Console.WriteLine($"    Given_Instructions(0x{wInstr:X4});");
            }
            Console.WriteLine("    Expect_Code(\"@@@\");");

            Console.WriteLine("}");
            Console.WriteLine();

#if !DEBUG
                throw new NotImplementedException($"A T32 decoder for the instruction {wInstr:X} ({message}) has not been implemented yet.");
#else
            return Invalid();
#endif
        }

        private AArch64Instruction Invalid()
        {
            return new AArch64Instruction
            {
                opcode = Opcode.Invalid,
                ops = new MachineOperand[0]
            };
        }


        static AArch64Disassembler()
        {
            invalid = new InstrDecoder(Opcode.Invalid, "");

            Decoder LoadsAndStores;
            {
                LoadsAndStores = new MaskDecoder(31, 1,
                    new MaskDecoder(28, 3,          // op0 = 0 
                        new MaskDecoder(26, 1,      // op0 = 0 op1 = 0
                            new MaskDecoder(23, 3,  // op0 = 0 op1 = 0 op2 = 0
                                Nyi("LoadStoreExclusive"),
                                Nyi("LoadStoreExclusive"),
                                invalid,
                                invalid),
                            new MaskDecoder(23, 3,  // op0 = 0 op1 = 0 op2 = 1
                                Nyi("AdvancedSimdLdStMultiple"),
                                Nyi("AdvancedSimdLdStMultiple"),
                                invalid,
                                invalid)),
                        new MaskDecoder(23, 3,      // op0 = 0, op1 = 1
                            Nyi("LoadRegLit"),
                            Nyi("LoadRegLit"),
                            invalid,
                            invalid),
                        new MaskDecoder(23, 3,      // op0 = 0, op1 = 2
                            Nyi("LdStNoallocatePair"),
                            Nyi("LdStRegPairPost"),
                            Nyi("LdStRegPairOffset"),
                            Nyi("LdStRegPairPre")),
                        Nyi(" op0 = 0, op1 = 3")),
                    invalid);
            }

            var AddSubImmediate = Mask(23, 1,
                Mask(29, 0x7,
                    Instr(Opcode.add, "W0:5,W5:5,U10:12w s22:2"),
                    Instr(Opcode.adds, "W0:5,W5:5,U10:12w s22:2"),
                    Instr(Opcode.sub, "W0:5,W5:5,U10:12w s22:2"),
                    Instr(Opcode.subs, "W0:5,W5:5,U10:12w s22:2"),
                    
                    Instr(Opcode.add, "X0:5,X5:5,U10:12l s22:2"),
                    Instr(Opcode.adds, "X0:5,X5:5,U10:12l s22:2"),
                    Instr(Opcode.sub, "X0:5,X5:5,U10:12l s22:2"),
                    Instr(Opcode.subs, "X0:5,X5:5,U10:12l s22:2")),
                invalid);

            var LogicalImmediate = Mask(29, 7, // size + op flag
                Mask(22, 1, // N bit
                    Instr(Opcode.and, "W0:5,W5:5,Ul10w"),
                    invalid),
                Mask(22, 1, // N bit
                    Instr(Opcode.orr, "W0:5,W5:5,Ul10w"),
                    invalid),
                Mask(22, 1, // N bit
                    Instr(Opcode.eor, "W0:5,W5:5,Ul10w"),
                    invalid),
                Mask(22, 1, // N bit
                    Instr(Opcode.ands, "W0:5,W5:5,Ul10w"),
                    invalid),

                Instr(Opcode.and, "X0:5,X5:5,Ul10l"),
                Instr(Opcode.orr, "X0:5,X5:5,Ul10l"),
                Instr(Opcode.eor, "X0:5,X5:5,Ul10l"),
                Instr(Opcode.ands, "X0:5,X5:5,Ul10l"));


                Nyi("LogicalImmediate");

            var MoveWideImmediate = Mask(29, 7,
                Mask(22, 1,
                    Instr(Opcode.movn, "* - 32 bit variant"),
                    invalid),
                invalid,
                Mask(22, 1,
                    Instr(Opcode.movz, "* - 32 bit variant"),
                    invalid),
                Mask(22, 1,
                    Instr(Opcode.movk, "W0:5,U5:16 S21:2"),
                    invalid),

                Instr(Opcode.movn, "* - 64 bit variant"),
                    invalid,
                Instr(Opcode.movz, "* - 64 bit variant"),
                Instr(Opcode.movk, "X0:5,U5:16 S21:2"));


            var DataProcessingImm = new MaskDecoder(23, 0x7,
                Nyi("PC-Rel addressing"),
                Nyi("PC-Rel addressing"),
                AddSubImmediate,
                AddSubImmediate,

                LogicalImmediate,
                MoveWideImmediate,
                Nyi("Bitfield"),
                Nyi("Extract"));

            var UncondBranchImm = Mask(31, 1,
                Instr(Opcode.b, "J0:26"),
                Instr(Opcode.bl, "J0:26"));

            var UncondBranchReg = Select("16:5", n => n != 0x1F,
                invalid,
                Mask(21, 0xF,
                    Sparse(10, 6,
                        invalid,
                        (0, Select("0:5", n => n == 0, Instr(Opcode.br, "X5:5"), invalid)),
                        (2, Select("0:5", n => n == 0x1F, Nyi("BRAA,BRAAZ... Key A"), invalid)),
                        (3, Select("0:5", n => n == 0x1F, Nyi("BRAA,BRAAZ... Key B"), invalid))),
                    Sparse(10, 6,
                        invalid,
                        (0, Select("0:5", n => n == 0, Instr(Opcode.blr, "*X5:5"), invalid)),
                        (2, Select("0:5", n => n == 0x1F, Nyi("BlRAA,BlRAAZ... Key A"), invalid)),
                        (3, Select("0:5", n => n == 0x1F, Nyi("BlRAA,BlRAAZ... Key B"), invalid))),
                    Sparse(10, 6,
                        invalid,
                        (0, Select("0:5", n => n == 0, Instr(Opcode.ret, "*X5:5"), invalid)),
                        (2, Select("0:5", n => n == 0x1F, Nyi("RETAA,RETAAZ... Key A"), invalid)),
                        (3, Select("0:5", n => n == 0x1F, Nyi("RETAA,RETAAZ... Key B"), invalid))),
                    invalid,

                    Select("5:5", n => n == 0x1F,
                        Sparse(10, 6,
                            invalid,
                            (0, Select("0:5", n => n == 0, Instr(Opcode.eret, "*X5:5"), invalid)),
                            (2, Select("0:5", n => n == 0x1F, Nyi("ERETAA,RETAAZ... Key A"), invalid)),
                            (3, Select("0:5", n => n == 0x1F, Nyi("ERETAA,RETAAZ... Key B"), invalid))),
                        invalid),
                    Select("10:6:5:5:0:5", n => n == 0b000000_11111_00000,
                        Instr(Opcode.drps, "*"), invalid),
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    invalid,
                    invalid));


            var CompareBranchImm = Nyi("CompareBranchImm");
            var TestBranchImm = Nyi("TestBranchImm");
            var CondBranchImm = Nyi("CondBranchImm");
            var System = Nyi("System");
            var ExceptionGeneration = Nyi("ExceptionGeneration");

            var BranchesExceptionsSystem = Mask(29, 0x7,
                UncondBranchImm,
                Mask(25, 1,
                    CompareBranchImm,
                    TestBranchImm),
                Mask(25, 1,
                    CondBranchImm,
                    invalid),
                invalid,

                UncondBranchImm,
                Mask(25, 1,
                    CompareBranchImm,
                    TestBranchImm),
                Mask(22, 0xF,
                    ExceptionGeneration,
                    ExceptionGeneration,
                    ExceptionGeneration,
                    ExceptionGeneration,

                    System,
                    invalid,
                    invalid,
                    invalid,

                    UncondBranchReg,
                    UncondBranchReg,
                    UncondBranchReg,
                    UncondBranchReg,

                    UncondBranchReg,
                    UncondBranchReg,
                    UncondBranchReg,
                    UncondBranchReg),
                invalid);

            rootDecoder = new MaskDecoder(25, 0x0F,
                invalid,
                invalid,
                invalid,
                invalid,

                LoadsAndStores,
                Nyi("DataProcessingReg"),
                Nyi("LoadsAndStores"),
                Nyi("DataProcessingScalarFpAdvancedSimd"),
                
                DataProcessingImm,
                DataProcessingImm,
                BranchesExceptionsSystem,
                BranchesExceptionsSystem,
                
                Nyi("LoadsAndStores"),
                Nyi("DataProcessingReg"),
                Nyi("LoadsAndStores"),
                Nyi("DataProcessingScalarFpAdvancedSimd"));
        }
    }
}
