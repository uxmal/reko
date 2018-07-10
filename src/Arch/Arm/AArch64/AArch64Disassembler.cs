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
                    n = ReadUnsignedBitField(wInstr, format, ref i);
                    reg = Registers.GpRegs32[n];
                    ops.Add(new RegisterOperand(reg));
                    break;
                case 'X':
                    // 64-bit register.
                    n = ReadUnsignedBitField(wInstr, format, ref i);
                    reg = Registers.GpRegs64[n];
                    ops.Add(new RegisterOperand(reg));
                    break;
                case 'S':
                    // 32-bit SIMD/FPU register.
                    n = ReadUnsignedBitField(wInstr, format, ref i);
                    reg = Registers.SimdRegs32[n];
                    ops.Add(new RegisterOperand(reg));
                    break;
                case 'U':
                    ImmediateOperand op = DecodeImmediateOperand(wInstr, format, ref i);
                    ops.Add(op);
                    break;
                case 'I':
                    ops.Add(DecodeSignedImmediateOperand(wInstr, format, ref i));
                    break;
                case 'J':
                    // Jump displacement from address of current instruction
                    n = ReadUnsignedBitField(wInstr, format, ref i);
                    n = (int)Bits.SignExtend(wInstr, 26);
                    AddressOperand aop = AddressOperand.Create(addr + (n << 2));
                    ops.Add(aop);
                    break;

                case '[':
                    // Memory access
                    ops.Add(ReadMemoryAccess(wInstr, format, ref i));
                    break;
                    
                case 's':
                    // Shift type
                    switch (format[i++])
                    {
                    case 'c': // code
                        n = ReadUnsignedBitField(wInstr, format, ref i);
                        switch (n)
                        {
                        case 1:
                            shiftCode = Opcode.lsl;
                            shiftAmount = ImmediateOperand.Int32(12);
                            break;
                        }
                        break;
                    case 'h': // 16-bit shifts
                        n = ReadUnsignedBitField(wInstr, format, ref i);
                        shiftCode = Opcode.lsl;
                        shiftAmount = ImmediateOperand.Int32(16 * n);
                        break;
                    case 'i': // code + immediate 
                        n = ReadUnsignedBitField(wInstr, format, ref i);
                        switch (n)
                        {
                        case 0: shiftCode = Opcode.lsl; break;
                        case 1: shiftCode = Opcode.lsr; break;
                        case 2: shiftCode = Opcode.asr; break;
                        case 3: shiftCode = Opcode.ror;  break;
                        }
                        Expect(',', format, ref i);
                        n = ReadUnsignedBitField(wInstr, format, ref i);
                        shiftAmount = ImmediateOperand.Int32(n);
                        break;
                    default:
                        NotYetImplemented($"Unknown format character '{format[i - 1]}' in '{format}' decoding {opcode} shift", wInstr);
                        break;
                    }
                    break;
                default:
                    NotYetImplemented($"Unknown format character '{format[i - 1]}' in '{format}' decoding {opcode}", wInstr);
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

        private MemoryOperand ReadMemoryAccess(uint wInstr, string format, ref int i)
        {
            Expect('X', format, ref i);
            int n = ReadUnsignedBitField(wInstr, format, ref i);
            RegisterStorage regBase = Registers.GpRegs64[n];
            Constant offset = null;

            if (PeekAndDiscard(',', format, ref i))
            {
                if (PeekAndDiscard('I', format, ref i))
                {
                    var imm = DecodeSignedImmediateOperand(wInstr, format, ref i);
                    offset = imm.Value;
                } else if (PeekAndDiscard('U', format, ref i))
                {
                    var imm = DecodeImmediateOperand(wInstr, format, ref i);
                    offset = imm.Value;
                }
            }
            Expect(',', format, ref i);
            var dt = ReadBitSize(format, ref i);
            Expect(']', format, ref i);
            return new MemoryOperand(dt)
            {
                Base = regBase,
                Offset = offset
            };
        }

        private ImmediateOperand DecodeSignedImmediateOperand(uint wInstr, string format, ref int i)
        {
            int n = ReadSignedBitField(wInstr, format, ref i);
            if (PeekAndDiscard('<', format, ref i))
            {
                int sh = ReadNumber(format, ref i);
                n <<= sh;
            }
            var dt = ReadBitSize(format, ref i);
            return new ImmediateOperand(Constant.Create(dt, n));
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
                imm = (uint)ReadUnsignedBitField(wInstr, format, ref i);
                dt = ReadBitSize(format, ref i);
                if (PeekAndDiscard('<', format, ref i))
                {
                    var sh = ReadNumber(format, ref i);
                    imm = imm.Value << sh;
                }
            }
            if (imm == null)
                return null;
            var op = new ImmediateOperand(Constant.Create(dt, imm.Value));
            return op;
        }

        private PrimitiveType ReadBitSize(string format, ref int i)
        {
            switch (format[i++])
            {
            case 'h': return PrimitiveType.Word16;
            case 'w': return PrimitiveType.Word32;
            case 'l': return PrimitiveType.Word64;
            }
            NotYetImplemented($"Unknown bit size format character '{format[i - 1]}'", 0);
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

        private int ReadUnsignedBitField(uint word, string format, ref int i)
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

        private int ReadSignedBitField(uint word, string format, ref int i)
        {
            uint n = 0;
            int totalBits = 0;
            do
            {
                int shift = ReadNumber(format, ref i);
                Expect(':', format, ref i);
                int maskSize = ReadNumber(format, ref i);
                totalBits += maskSize;
                uint mask = (1u << maskSize) - 1u;
                n = (n << maskSize) | ((word >> shift) & mask);
            } while (PeekAndDiscard(':', format, ref i));
            return (int) Bits.SignExtend(n, totalBits);
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

        private static Decoder Mask(string bitfields, params Decoder[] decoders)
        {
            return new BitfieldDecoder(bitfields, decoders);
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
            Console.WriteLine($"// An AArch64 decoder for the instruction {wInstr:X} ({message}) has not been implemented yet.");
            Console.WriteLine("[Test]");
            Console.WriteLine($"public void AArch64Dis_{wInstr:X8}()");
            Console.WriteLine("{");
            Console.WriteLine($"    Given_Instruction(0x{wInstr:X8});");
            Console.WriteLine("    Expect_Code(\"@@@\");");
            Console.WriteLine("}");
            Console.WriteLine();

#if !DEBUG
                throw new NotImplementedException($"An AArch64 decoder for the instruction {wInstr:X} ({message}) has not been implemented yet.");
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

            Decoder LdStRegUImm;
            {
                LdStRegUImm = Mask(30, 3, // size
                    Mask(26, 1, // V
                        Mask(22, 3,
                           Instr(Opcode.strb, "*imm"),
                           Instr(Opcode.ldrb, "*imm"),
                           Instr(Opcode.ldrsb, "*imm 64-bit"),
                           Instr(Opcode.ldrsb, "*imm 32-bit")),
                        Nyi("LdStRegUImm size = 0, V = 1")),
                    Nyi("LdStRegUImm size = 1"),
                    Mask(26, 1, // V
                        Mask(22, 3,
                            Instr(Opcode.str, "W0:5,[X5:5,U10:12l<2,l]"),
                            Instr(Opcode.ldr, "W0:5,[X5:5,U10:12l<2,l]"),
                            Instr(Opcode.ldrsw, "X0:5,[X5:5,U10:12l<2,l]"),
                            invalid),
                        Mask(22, 3,
                            Instr(Opcode.str, "*immediate SIMD&FP 32-bit"),
                            Instr(Opcode.ldr, "*immediate SIMD&FP 32-bit"),
                            invalid,
                            invalid)),
                    Mask(26, 1, // V
                        Mask(22, 3,
                            Instr(Opcode.str, "X0:5,[X5:5,U10:12l<3,l]"),
                            Instr(Opcode.ldr, "X0:5,[X5:5,U10:12l<3,l]"),
                            Instr(Opcode.prfm, "*"),
                            invalid),
                        Nyi("LdStRegUImm size = 3, V = 1")));
            }
            Decoder LdStRegPairOffset;
            {
                LdStRegPairOffset = Mask(30, 3,
                    Mask(26, 1, // V
                        Nyi("LdStRegPairOffset - 00 V = 0"),
                        Mask(22, 1,  // L
                            Instr(Opcode.stp, "*SIMD&FP - 32bit"),
                            Instr(Opcode.ldp, "S0:5,S10:5,[X5:5,I15:7<2l,l]"))),

                    Nyi("LdStRegPairOffset - 01"),
                    Nyi("LdStRegPairOffset - 10"),
                    invalid);
            }
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
                            LdStRegPairOffset,
                            Nyi("LdStRegPairPre")),
                        Nyi(" op0 = 0, op1 = 3")),
                    new MaskDecoder(28, 3,          // op0 = 0 
                        Nyi("op0 = 0"),
                        Nyi("op0 = 1"),
                        Nyi("op0 = 2"),
                        Mask(24, 1,
                            Nyi("op0 = 3, op3 = 0x"),
                            LdStRegUImm)));
            }

            var AddSubImmediate = Mask(23, 1,
                Mask(29, 0x7,
                    Instr(Opcode.add, "W0:5,W5:5,U10:12w sc22:2"),
                    Instr(Opcode.adds, "W0:5,W5:5,U10:12w sc22:2"),
                    Instr(Opcode.sub, "W0:5,W5:5,U10:12w sc22:2"),
                    Instr(Opcode.subs, "W0:5,W5:5,U10:12w sc22:2"),
                    
                    Instr(Opcode.add, "X0:5,X5:5,U10:12l sc22:2"),
                    Instr(Opcode.adds, "X0:5,X5:5,U10:12l sc22:2"),
                    Instr(Opcode.sub, "X0:5,X5:5,U10:12l sc22:2"),
                    Instr(Opcode.subs, "X0:5,X5:5,U10:12l sc22:2")),
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
                    Instr(Opcode.movz, "W0:5,U5:16w sh21:2"),
                    invalid),
                Mask(22, 1,
                    Instr(Opcode.movk, "W0:5,U5:16h sh21:2"),
                    invalid),

                Instr(Opcode.movn, "* - 64 bit variant"),
                    invalid,
                Instr(Opcode.movz, "X0:5,U5:16l sh21:2"),
                Instr(Opcode.movk, "X0:5,U5:16h sh21:2"));


            var PcRelativeAddressing = Mask(31, 1,
                Instr(Opcode.adr, "*"),
                Instr(Opcode.adrp, "X0:5,I5:19:29:2<12w"));

            var DataProcessingImm = new MaskDecoder(23, 0x7,
                PcRelativeAddressing,
                PcRelativeAddressing,
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

            var TestBranchImm = Mask(24, 1,
                Mask(31, 1,
                    Instr(Opcode.tbz, "W0:5,I19:5w,J5:14"),
                    Instr(Opcode.tbnz, "W0:5,I19:5w,J5:14")),
                Mask(31, 1,
                    Instr(Opcode.tbz, "W0:5,I19:5w,J5:14"),
                    Instr(Opcode.tbnz, "W0:5,I19:5w,J5:14")));

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

            Decoder LogicalShiftedRegister;
            {
                LogicalShiftedRegister = Mask(31, 1,
                    Select("15:1", n => n == 1,
                        invalid,
                        Mask("29:2:21:1",
                            Instr(Opcode.and, "*shifted register, 32-bit"),
                            Instr(Opcode.bic, "*shifted register, 32-bit"),
                            Select("22:2:10:6:5:5", n => n == 0x1F,
                                Instr(Opcode.mov, "W0:5,W16:5 si22:2,10:6"),
                                Instr(Opcode.orr, "W0:5,W5:5,W16:5 si22:2,10:6")),
                            Instr(Opcode.orn, "*shifted register, 32-bit"),

                            Instr(Opcode.eor, "*shifted register, 32-bit"),
                            Instr(Opcode.eon, "*shifted register, 32-bit"),
                            Instr(Opcode.ands, "*shifted register, 32-bit"),
                            Instr(Opcode.bics, "*shifted register, 32-bit"))),
                    Mask("29:2:21:1",
                        Instr(Opcode.and, "*shifted register, 64-bit"),
                        Instr(Opcode.bic, "*shifted register, 64-bit"),
                        Select("22:2:10:6:5:5", n => n == 0x1F,
                            Instr(Opcode.mov, "X0:5,X16:5 si22:2,10:6"),
                            Instr(Opcode.orr, "X0:5,X5:5,X16:5 si22:2,10:6")),
                        Instr(Opcode.orn, "*shifted register, 64-bit"),

                        Instr(Opcode.eor, "*shifted register, 64-bit"),
                        Instr(Opcode.eon, "*shifted register, 64-bit"),
                        Instr(Opcode.ands, "*shifted register, 64-bit"),
                        Instr(Opcode.bics, "*shifted register, 64-bit")));
            }
            var AddSubShiftedRegister = Nyi("AddSubShiftedRegister");
            var AddSubExtendedRegister = Nyi("AddSubExtendedRegister");
            var DataProcessing3Source = Nyi("DataProcessing3Source");

            Decoder DataProcessingReg;
            {
                DataProcessingReg =  Mask(28, 1,         // op1
                    Mask(21, 0xF,           // op2
                        LogicalShiftedRegister,
                        LogicalShiftedRegister,
                        LogicalShiftedRegister,
                        LogicalShiftedRegister,

                        LogicalShiftedRegister,
                        LogicalShiftedRegister,
                        LogicalShiftedRegister,
                        LogicalShiftedRegister,

                        AddSubShiftedRegister,
                        AddSubExtendedRegister,
                        AddSubShiftedRegister,
                        AddSubExtendedRegister,

                        AddSubShiftedRegister,
                        AddSubExtendedRegister,
                        AddSubShiftedRegister,
                        AddSubExtendedRegister),
                    Mask(21, 0xF,           // op1 = 1, op2
                        Nyi("AddSubWithCarry"),
                        invalid,
                        Mask(11, 1,         // op1 = 1, op2 = 2,
                            Nyi("ConditionalCompareReg"),
                            Nyi("ConditionalCompareImm")),
                        invalid,

                        Nyi("ConditionalSelect"),
                        invalid,
                        Mask(30, 1,         // op1 = 1, op2 = 6, op0
                            Nyi("DataProcessing 2 source"),
                            Nyi("DataProcessing 1 source")),
                        invalid,

                        DataProcessing3Source,
                        DataProcessing3Source,
                        DataProcessing3Source,
                        DataProcessing3Source,

                        DataProcessing3Source,
                        DataProcessing3Source,
                        DataProcessing3Source,
                        DataProcessing3Source));
            }
            rootDecoder = new MaskDecoder(25, 0x0F,
                invalid,
                invalid,
                invalid,
                invalid,

                LoadsAndStores,
                DataProcessingReg,
                LoadsAndStores,
                Nyi("DataProcessingScalarFpAdvancedSimd"),
                
                DataProcessingImm,
                DataProcessingImm,
                BranchesExceptionsSystem,
                BranchesExceptionsSystem,
                
                LoadsAndStores,
                Nyi("DataProcessingReg"),
                LoadsAndStores,
                Nyi("DataProcessingScalarFpAdvancedSimd"));
        }
    }
}
