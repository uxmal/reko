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
    public partial class A32Disassembler : DisassemblerBase<AArch32Instruction>
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
            ArmVectorData vectorData = ArmVectorData.INVALID;
            bool useQ = false;

            for (int i = 0; i < format.Length; ++i)
            {
                MachineOperand op;
                int offset;
                uint imm;
                switch (format[i])
                {
                case ',':
                case ' ':
                    continue;

                case 'v':   // Set the element size of a SIMD instruction
                    ++i;
                    if (PeekAndDiscard('W', format, ref i))
                    {
                        imm = ReadBitfields(wInstr, format, ref i);
                        vectorData = VectorElementUntypedReverse(imm);
                        --i;
                        continue;
                    }
                    else if (PeekAndDiscard('i', format, ref i))
                    {
                        offset = ReadDecimal(format, ref i);
                        vectorData = VectorElementInteger(offset);
                        --i;
                        continue;
                    }
                    else if (PeekAndDiscard('f', format, ref i))
                    {
                        offset = ReadDecimal(format, ref i);
                        vectorData = VectorElementFloat(offset);
                        --i;
                        continue;
                    }
                    return NotYetImplemented($"Unknown SIMD format {format}", wInstr);
                case 'q': // bit which determines whether or not to use Qx or Dx registers in SIMD
                    ++i;
                    offset = ReadDecimal(format, ref i);
                    useQ = bit(wInstr, offset);
                    continue;
                case 'w': // sets the writeback bit.
                    ++i;
                    offset = ReadDecimal(format, ref i);
                    writeback = bit(wInstr, offset);
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
                    ++i;
                    imm = (wInstr & 0xFFF) | ((wInstr >> 4) & 0xF000);
                    if (PeekAndDiscard('h', format, ref i))
                    {
                        op = ImmediateOperand.Word16((ushort)imm);
                    }
                    else
                    {
                        op = ImmediateOperand.Word32(imm);
                    }
                    break;
                case 'r':   // register at a 4-bit multiple offset
                    offset = (format[++i] - '0') * 4;
                    op = new RegisterOperand(Registers.GpRegs[bitmask(wInstr, offset, 0xF)]);
                    break;
                case 'W':   // "wector" register
                    ++i;
                    imm = ReadBitfields(wInstr, format, ref i);
                    if (useQ)
                    {
                        if ((imm & 1) == 1)
                            return Invalid();
                        op = new RegisterOperand(Registers.QRegs[imm >> 1]);
                    }
                    else
                    {
                        op = new RegisterOperand(Registers.DRegs[imm]);
                    }
                    break;
                case '[':
                    {
                        int shift = 0;
                        ++i;
                        var memType = format[i];
                        ++i;
                        if (PeekAndDiscard('<', format, ref i))
                        {
                            shift = ReadDecimal(format, ref i);
                        }
                        Expect(':', format, ref i);
                        var dom = format[i];
                        ++i;
                        var size = format[i] - '0';
                        ++i;
                        var dt = GetDataType(dom, size);
                        (op, writeback) = DecodeMemoryAccess(wInstr, memType, shift, dt);
                    }
                    break;
                case 'M': // Multiple registers
                    ++i;
                    if (PeekAndDiscard('r', format, ref i))
                    {
                        imm = ReadBitfields(wInstr, format, ref i);
                        op = new MultiRegisterOperand(Registers.GpRegs, PrimitiveType.Word16, (ushort)imm);
                    }
                    else if (PeekAndDiscard('d', format, ref i))
                    {
                        // double-precision VLDM{IA|DB} arguments.
                        //$PERF: put this in a static field
                        var baseRegFields = new[]
                        {
                            new Bitfield(22, 1), new Bitfield(12, 4)
                        };
                        var baseReg = (int) Bitfield.ReadFields(baseRegFields, wInstr);
                        var regs = SBitfield(wInstr, 1, 7);
                        var bitmask = (((1u << regs) - 1u) << baseReg);
                        op = new MultiRegisterOperand(Registers.DRegs, PrimitiveType.Word64, bitmask);
                    }
                    else
                    {
                        return NotYetImplemented($"SIMD LDRM mode {format[i]} not implemented", wInstr);
                    }
                    break;
                case 'S':   // 'SR' = special register
                    ++i;
                    if (PeekAndDiscard('R', format, ref i))
                    {
                        ++i;
                        var sr = bit(wInstr, 22) ? Registers.spsr : Registers.cpsr;
                        op = new RegisterOperand(sr);
                    }
                    else
                    {
                        // 'S13' = fpu register.
                        offset = (int)ReadBitfields(wInstr, format, ref i);
                        op = new RegisterOperand(Registers.SRegs[offset]);
                    }
                    break;
                case 'D':
                    // 'D13' = fpu register.
                    ++i;
                    offset = (int)ReadBitfields(wInstr, format, ref i);
                    op = new RegisterOperand(Registers.DRegs[offset]);
                    break;
                case 'E':   // Endianness
                    ++i;
                    imm = ReadBitfields(wInstr, format, ref i);
                    op = new EndiannessOperand(imm!=0);
                    break;
                case 'i':   // General purpose immediate
                    ++i;
                    op = ReadImmediate(wInstr, format, ref i);
                    break;
                case 's':   // use bit 20 to determine 
                    updateFlags = ((wInstr >> 20) & 1) != 0;
                    continue;
                case 'C': // coprocessor 
                    ++i;
                    switch (format[i])
                    {
                    case 'P':   // Coprocessor #
                        ++i;
                        if (PeekAndDiscard('#', format, ref i))   // Literal
                        {
                            offset = ReadDecimal(format, ref i);
                            var cp = Registers.Coprocessors[offset];
                            op = new RegisterOperand(cp);
                        }
                        else
                        {
                            offset = ReadDecimal(format, ref i);
                            op = Coprocessor(wInstr, offset);
                        }
                        break;
                    case 'R':   // Coprocessor register
                        ++i;
                        offset = ReadDecimal(format, ref i);
                        op = CoprocessorRegister(wInstr, offset);
                        break;
                    default:
                        return NotYetImplemented($"Unknown format specifier C{format[i]} in {format} when decoding {opcode}", wInstr);
                    }
                    break;
                case '>':   // shift
                    ++i;
                    if (format[i] == 'i')
                        (shiftOp, shiftValue) = DecodeImmShift(wInstr);
                    else
                        (shiftOp, shiftValue) = DecodeRegShift(wInstr);
                    continue;
                default:
                    throw new NotImplementedException($"Found unknown format character '{format[i]}' in '{format}' while decoding {opcode}.");
                }
                ops.Add(op);
            }
            var instr = new AArch32Instruction
            {
                opcode = opcode,
                ops = ops.ToArray(),
                ShiftType = shiftOp,
                ShiftValue = shiftValue,
                SetFlags = updateFlags,
                Writeback = writeback,
                vector_data = vectorData,
            };
            return instr;
        }

        private ArmVectorData VectorElementInteger(int bitSize)
        {
            switch (bitSize)
            {
            default:  throw new ArgumentException(nameof(bitSize), "Bit size must be 8, 16, or 32.");
            case 8: return ArmVectorData.I8;
            case 16: return ArmVectorData.I16;
            case 32: return ArmVectorData.I32;
            case 64: return ArmVectorData.I64;
            }
        }

        private ArmVectorData VectorElementFloat(int bitSize)
        {
            switch (bitSize)
            {
            default: throw new ArgumentException(nameof(bitSize), "Bit size must be 8, 16, or 32.");
            case 16: return ArmVectorData.F16;
            case 32: return ArmVectorData.F32;
            case 64: return ArmVectorData.F64;
            }
        }


        private ArmVectorData VectorElementUntypedReverse(uint imm)
        {
            switch (imm)
            {
            case 0: return ArmVectorData.I32;
            case 1: return ArmVectorData.I16;
            case 2: return ArmVectorData.I8;
            default: return ArmVectorData.INVALID;
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

        private int SBitfield(uint wInstr, int bitPos, int size)
        {
            return (int)((wInstr >> bitPos) & ((1u << size) - 1));
        }

        private MachineOperand ReadImmediate(uint wInstr, string format, ref int i)
        {
            var simdFormat = PeekAndDiscard('s', format, ref i);
            var imm = (int)ReadBitfields(wInstr, format, ref i);
            if (PeekAndDiscard('+', format, ref i))
            {
                var adj = ReadDecimal(format, ref i);
                imm += adj;
            }
            if (PeekAndDiscard('<', format, ref i))
            {
                var sh = ReadDecimal(format, ref i);
                imm <<= sh;
            }
            if (simdFormat)
            {
                var cmode = (wInstr >> 8) & 0xF;
                var op = (wInstr >> 5) & 1;
                return ImmediateOperand.Word64(SimdExpandImm(op, cmode, (uint)imm));
            }
            if (PeekAndDiscard('h', format, ref i))
            {
                return ImmediateOperand.Word16((ushort)imm);
            }
            return ImmediateOperand.Word32(imm);
        }

        private ulong SimdExpandImm(uint op, uint cmode, uint imm)
        {
            ulong imm64 = imm;
            switch (cmode)
            {

            case 0:
            case 1:
                imm64 |= imm64 << 32;
                break;
            case 2:
            case 3:
                imm64 = imm64 << 8;
                imm64 |= imm64 << 32;
                break;
            case 4:
            case 5:
                imm64 = imm64 << 16;
                imm64 |= imm64 << 32;
                break;
            case 6:
            case 7:
                imm64 = imm64 << 16;
                imm64 |= imm64 << 32;
                break;
            case 8:
            case 9:
                imm64 |= imm64 << 16;
                imm64 |= imm64 << 32;
                break;
            case 10:
            case 11:
                imm64 = imm64 << 8;
                imm64 |= imm64 << 16;
                imm64 |= imm64 << 32;
                break;
            case 12:
                imm64 = (imm64 << 8) | 0xFF;
                imm64 |= imm64 << 32;
                break;
            case 13:
                imm64 = (imm64 << 16) | 0xFFFF;
                imm64 |= imm64 << 32;
                break;
            case 14:
                if (op == 0)
                {
                    imm64 |= imm64 << 8;
                    imm64 |= imm64 << 16;
                    imm64 |= imm64 << 32;
                }
                else
                {
                    throw new NotImplementedException();
                }
                break;
            case 15:
            default:
                throw new NotImplementedException();
            }
            return imm64;
        }

        private static HashSet<uint> seen = new HashSet<uint>();

        private AArch32Instruction NotYetImplemented(string message, uint wInstr)
        {
            if (!seen.Contains(wInstr))
            {
                seen.Add(wInstr);
                Console.WriteLine($"// An A32 decoder for the instruction {wInstr:X8} ({message}) has not been implemented yet.");
                Console.WriteLine("[Test]");
                Console.WriteLine($"public void ArmDasm_{wInstr:X8}()");
                Console.WriteLine("{");
                Console.WriteLine($"    Disassemble32(0x{wInstr:X8});");
                Console.WriteLine("    Expect_Code(\"@@@\");");
                Console.WriteLine("}");
                Console.WriteLine();
            }
#if !DEBUG
                throw new NotImplementedException($"An A32 decoder for the instruction {wInstr:X} ({message}) has not been implemented yet.");
#else
            return Invalid();
#endif
        }

        private AArch32Instruction Invalid()
        {
            return new AArch32Instruction
            {
                opcode = Opcode.Invalid,
                ops = new MachineOperand[0]
            };
        }

        /// <summary>
        /// Reads and concatenates bitfields out of <paramref name="wInstr"/>.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        private static uint ReadBitfields(uint wInstr, string format, ref int i)
        {
            uint n = 0;
            do
            {
                int pos = ReadDecimal(format, ref i);
                Expect(':', format, ref i);
                int size = ReadDecimal(format, ref i);
                n = (n << size) | ((wInstr >> pos) & (1u << size) - 1);
            } while (PeekAndDiscard(':', format, ref i));
            return n;
        }

        private static int ReadDecimal(string format, ref int i)
        {
            int n = 0;
            while (i < format.Length)
            {
                var c = format[i];
                if (!Char.IsDigit(c))
                    break;
                n = n * 10 + (c - '0');
                ++i;
            }
            return n;
        }

        private static void Expect(char c, string format, ref int i)
        {
            if (i < format.Length && format[i] == c)
            {
                ++i;
                return;
            }
            throw new InvalidOperationException($"Unexpected character '{c}' at position {i} in format string '{format}'.");
        }

        private static bool PeekAndDiscard(char c, string format, ref int i)
        {
            if (i >= format.Length)
                return false;
            if (format[i] != c)
                return false;
            ++i;
            return true;
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

        private (Opcode, MachineOperand) DecodeRegShift(uint wInstr)
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

        private (MemoryOperand, bool) DecodeMemoryAccess(uint wInstr, char memType, int shift, PrimitiveType dtAccess)
        {
            var n = Registers.GpRegs[bitmask(wInstr, 16, 0xF)];
            var m = Registers.GpRegs[bitmask(wInstr, 0, 0x0F)];
            Constant offset = null;
            int shiftAmt = 0;
            Opcode shiftType = Opcode.Invalid;
            switch (memType)
            {
            case 'o':   // offset 12 bits
                offset = Constant.Int32((int)bitmask(wInstr, 0, 0xFFF)<<shift);
                m = null;
                break;
            case 'i':   // offset 8 bits
                offset = Constant.Int32((int)bitmask(wInstr, 0, 0xFF)<<shift);
                m = null;
                break;
            case 'h':   // offset split in hi-lo nybbles.
                offset = Constant.Int32(
                    (int)(((wInstr >> 4) & 0xF0) | (wInstr & 0x0F))<<shift);
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

        private static Decoder Instr(Opcode opcode, string format)
        {
            return new InstrDecoder(opcode, format);
        }
         
        private static NyiDecoder nyi(string str)
        {
            return new NyiDecoder(str);
        }

        private static Decoder Mask(int sh, uint mask, params Decoder []decoders)
        { 
            return new MaskDecoder(sh, mask, decoders);
        }

        /// <summary>
        /// Create a decoder for 2 bitfields.
        /// </summary>
        private static Decoder Mask(
            int sh1, int len1,
            int sh2, int len2,
            params Decoder[] decoders)
        {
            return new BitfieldDecoder(
                new Bitfield[] {
                    new Bitfield(sh1, len1),
                    new Bitfield(sh2, len2),
                }, decoders);
        }

        /// <summary>
        /// Create a decoder for 3 bitfields.
        /// </summary>
        private static Decoder Mask(
            int sh1, int len1,
            int sh2, int len2,
            int sh3, int len3,
            params Decoder[] decoders)
        {
            return new BitfieldDecoder(
                new Bitfield[] {
                    new Bitfield(sh1, len1),
                    new Bitfield(sh2, len2),
                    new Bitfield(sh3, len3),
                }, decoders);
        }

        /// <summary>
        /// Create a decoder for 4 bitfields.
        /// </summary>
        private static Decoder Mask(
            int sh1, int len1, 
            int sh2, int len2,
            int sh3, int len3,
            int sh4, int len4,
            params Decoder [] decoders)
        {
            return new BitfieldDecoder(
                new Bitfield[] {
                    new Bitfield(sh1, len1),
                    new Bitfield(sh2, len2),
                    new Bitfield(sh3, len3),
                    new Bitfield(sh4, len4)
                }, decoders);
        }

        private static Decoder SparseMask(int shift, uint mask, Dictionary<uint, Decoder> decoders)
        {
            return new SparseMaskDecoder(shift, mask, decoders, invalid);
        }

        private static Decoder SparseMask(int shift, uint mask, Dictionary<uint, Decoder> decoders, Decoder @default)
        {
            return new SparseMaskDecoder(shift, mask, decoders, @default);
        }

        private static Decoder Select(int shift, uint mask, Predicate<uint> predicate, Decoder trueDecoder, Decoder falseDecoder)
        {
            return new SelectDecoder(shift, mask, predicate, trueDecoder, falseDecoder);
        }

        static A32Disassembler()
        {
            invalid = new InstrDecoder(Opcode.Invalid, "");

            var LoadStoreExclusive = nyi("LoadStoreExclusive");

            var Stl = new InstrDecoder(Opcode.stl, "*");
            var Stlex = new InstrDecoder(Opcode.stlex, "*");
            var Strex = new InstrDecoder(Opcode.strex, "*");
            var Lda = new InstrDecoder(Opcode.lda, "*");
            var Ldaex = new InstrDecoder(Opcode.ldaex, "*");
            var Ldrex = new InstrDecoder(Opcode.ldrex, "*");
            var Stlexd = new InstrDecoder(Opcode.stlexd, "*");
            var Strexd = new InstrDecoder(Opcode.strexd, "*");
            var Ldaexd = new InstrDecoder(Opcode.ldaexd, "*");
            var Ldrexd = new InstrDecoder(Opcode.ldrexd, "*");
            var Stlb = new InstrDecoder(Opcode.stlb, "*");
            var Stlexb = new InstrDecoder(Opcode.stlexb, "*");
            var Strexb = new InstrDecoder(Opcode.strexb, "*");
            var Ldab = new InstrDecoder(Opcode.ldab, "*");
            var Ldaexb = new InstrDecoder(Opcode.ldrexb, "*");
            var Ldrexb = new InstrDecoder(Opcode.ldaexb, "*");
            var Stlh = new InstrDecoder(Opcode.stlh, "*");
            var Stlexh = new InstrDecoder(Opcode.stlexh, "*");
            var Strexh = new InstrDecoder(Opcode.strexh, "*");
            var Ldah = new InstrDecoder(Opcode.ldah, "*");
            var Ldaexh = new InstrDecoder(Opcode.ldrexh, "*");
            var Ldrexh = new InstrDecoder(Opcode.ldaexh, "*");

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
            var Ldrht = new InstrDecoder(Opcode.ldrht, "*");
            var Ldrsbt = new InstrDecoder(Opcode.ldrsbt, "*");
            var Ldrsht = new InstrDecoder(Opcode.ldrsht, "*");
            var StrdRegister = new InstrDecoder(Opcode.strd, "r3,[x:w8]");
            var StrhRegister = new InstrDecoder(Opcode.strh, "r3,[_:w2]");
            var Strht = new InstrDecoder(Opcode.strht, "*");

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

            var LdrdLiteral = new InstrDecoder(Opcode.ldrd, "*");
            var LdrhLiteral = new InstrDecoder(Opcode.ldrh, "*");
            var LdrsbLiteral = new InstrDecoder(Opcode.ldrsb, "*");
            var LdrshLiteral = new InstrDecoder(Opcode.ldrsh, "*");
            var StrhImmediate = new InstrDecoder(Opcode.strh, "*");
            var LdrdImmediate = new InstrDecoder(Opcode.ldrd, "*");
            var StrdImmediate = new InstrDecoder(Opcode.strd, "*");
            var LdrhImmediate = new InstrDecoder(Opcode.ldrh, "*");
            var LdrsbImmediate = new InstrDecoder(Opcode.ldrsb, "r3,[h:s1]");
            var LdrshImmediate = new InstrDecoder(Opcode.ldrsh, "*");

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
            var Msr = new InstrDecoder(Opcode.msr, "*");
            var MrsBanked = new InstrDecoder(Opcode.mrs, "*");
            var MsrBanked = new InstrDecoder(Opcode.msr, "*");
            var MoveSpecialRegister = new MaskDecoder(21, 1,
                new MaskDecoder(9, 1,
                    Mrs,
                    MrsBanked),
                new MaskDecoder(9, 1,
                    Msr,
                    MsrBanked));

            var Crc32 = new InstrDecoder(Opcode.crc32w, "*");
            var Crc32C = new InstrDecoder(Opcode.crc32cw, "*");

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

            var Qadd = new InstrDecoder(Opcode.qadd, "*");
            var Qsub = new InstrDecoder(Opcode.qsub, "*");
            var Qdadd = new InstrDecoder(Opcode.qdadd, "*");
            var Qdsub = new InstrDecoder(Opcode.qdsub, "*");
            var IntegerSaturatingArithmetic = new MaskDecoder(21, 3,
                Qadd,
                Qsub,
                Qdadd,
                Qsub);


            var Hlt = new InstrDecoder(Opcode.hlt, "*");
            var Bkpt = new InstrDecoder(Opcode.bkpt, "*");
            var Hvc = new InstrDecoder(Opcode.hvc, "*");
            var Smc = new InstrDecoder(Opcode.smc, "*");
            var ExceptionGeneration = new MaskDecoder(21, 3,
                Hlt,
                Bkpt,
                Hvc,
                Smc);

            var Bx = new InstrDecoder(Opcode.bx, "*");
            var Bxj = new InstrDecoder(Opcode.bxj, "*");
            var Blx = new InstrDecoder(Opcode.blx, "*");
            var Clz = new InstrDecoder(Opcode.blx, "*");
            var Eret = new InstrDecoder(Opcode.eret, "*");

            var ChangeProcessState = new MaskDecoder(16, 1, // op
                nyi("CPS,CPSID,CPSIE"),
                Select(4, 1, n => n == 0, Instr(Opcode.setend, "E9:1"), invalid));

            var Miscellaneous = new MaskDecoder(22, 7,   // op0
                invalid,
                invalid,
                invalid,
                invalid,

                new MaskDecoder(20, 3,
                    Select(5, 1, n => n == 0, ChangeProcessState, invalid),
                    Select(4, 0xF, n => n == 0, new InstrDecoder(Opcode.setpan, "*"), invalid),
                    invalid,
                    invalid),
                invalid,
                invalid,
                invalid);

            /*
        var Miscellaneous = new MaskDecoder(21, 3,   // op0
            new MaskDecoder(24, 1, // op1
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
                */
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
                new InstrDecoder(Opcode.tst, "r4,r0,>i"),
                new InstrDecoder(Opcode.teq, "r4,r0,>i"),
                new InstrDecoder(Opcode.cmp, "r4,r0,>i"),
                new InstrDecoder(Opcode.cmn, "r4,r0,>i"));

            var LogicalArithmeticImmShift = new MaskDecoder(21, 3,
                new InstrDecoder(Opcode.orr, "sr3,r4,r0,>i"),
                new MovDecoder(Opcode.mov, "sr3,r0,>i"),
                new InstrDecoder(Opcode.bic, "sr3,r4,r0,>i"),
                new InstrDecoder(Opcode.mvn, "sr3,r0,>i"));

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
                new InstrDecoder(Opcode.tst, "r4,r0,>r"),
                new InstrDecoder(Opcode.teq, "r4,r0,>r"),
                new InstrDecoder(Opcode.cmp, "r4,r0,>r"),
                new InstrDecoder(Opcode.cmn, "r4,r0,>r"));

            var LogicalArithmeticRegShift = new MaskDecoder(21, 3,
                new InstrDecoder(Opcode.orr, "sr3,r4,r0,>r"),
                new InstrDecoder(Opcode.mov, "sr4,r0,>r"),
                new InstrDecoder(Opcode.bic, "sr3,r4,r0,>r"),
                new InstrDecoder(Opcode.mvn, "sr4,r0,>r"));

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
               new InstrDecoder(Opcode.movt, "r3,Yh"));

            var IntegerTestAndCompareOneRegImm = new MaskDecoder(21, 3,
                new InstrDecoder(Opcode.tst, "r4,I"),
                new InstrDecoder(Opcode.teq, "r4,I"),
                new InstrDecoder(Opcode.cmp, "r4,I"),
                new InstrDecoder(Opcode.cmn, "r4,I"));

            var MsrImmediate = new InstrDecoder(Opcode.msr, "*");
            var Nop = new InstrDecoder(Opcode.nop, "");
            var Yield = new InstrDecoder(Opcode.yield, "*");
            var Wfe = new InstrDecoder(Opcode.wfe, "*");
            var Wfi = new InstrDecoder(Opcode.wfi, "*");
            var Sev = new InstrDecoder(Opcode.sevl, "*");
            var Sevl = new InstrDecoder(Opcode.sevl, "*");
            var ReservedNop = new InstrDecoder(Opcode.nop, "");
            var Esb = new InstrDecoder(Opcode.esb, "*");
            var Dbg = new InstrDecoder(Opcode.dbg, "*");

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

            var LdrLiteral = new InstrDecoder(Opcode.ldr, "r3,[o:w4]");
            var LdrbLiteral = new InstrDecoder(Opcode.ldrb, "r3,[0:w1]");
            var StrImm = new InstrDecoder(Opcode.str, "r3,[o:w4]");
            var LdrImm = new InstrDecoder(Opcode.ldr, "r3,[o:w4]");
            var StrbImm = new InstrDecoder(Opcode.strb, "r3,[o:w1]");
            var LdrbImm = new InstrDecoder(Opcode.ldrb, "r3,[o:w1]");
            
            var LoadStoreWordUnsignedByteImmLit = Mask(24, 1, 21, 1, 22, 1, 20, 1,
                // PW=0b00 00
                Instr(Opcode.str, "r3,[o:w4]"),
                Select(16, 0xF, n => n != 0xF,
                    Instr(Opcode.ldr, "r3,[o:w4]"),
                    Instr(Opcode.ldr, "r3,[o:w4]")),
                Instr(Opcode.strb, "r3,[o:w1]"),
                Select(16, 0xF, n => n != 0xF,
                    Instr(Opcode.ldrb, "r3,[o:w1]"),
                    Instr(Opcode.ldrb, "r3,[o:w1]")),

                Instr(Opcode.strt, "*"),
                Instr(Opcode.ldrt, "*"),
                Instr(Opcode.strbt, "*"),
                Instr(Opcode.ldrbt, "*"),

                Instr(Opcode.str, "r3,[o:w4]"),
                Select(16, 0xF, n => n != 0xF,
                    Instr(Opcode.ldr, "r3,[o:w4]"),
                    Instr(Opcode.ldr, "r3,[o:w4]")),
                Instr(Opcode.strb, "r3,[o:w1]"),
                Select(16, 0xF, n => n != 0xF,
                    Instr(Opcode.ldrb, "r3,[o:w1]"),
                    Instr(Opcode.ldrb, "r3,[o:w1]")),

                Instr(Opcode.str, "r3,[o:w4]"),
                Select(16, 0xF, n => n != 0xF,
                    Instr(Opcode.ldr, "r3,[o:w4]"),
                    Instr(Opcode.ldr, "r3,[o:w4]")),
                Instr(Opcode.strb, "r3,[o:w1]"),
                Select(16, 0xF, n => n != 0xF,
                    Instr(Opcode.ldrb, "r3,[o:w1]"),
                    Instr(Opcode.ldrb, "r3,[o:w1]")));

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
                case 2: throw new NotImplementedException(" return Strt");
                case 3: throw new NotImplementedException(" return ldrt");

                case 4: return StrbReg;
                case 5: return LdrbReg;
                case 6: throw new NotImplementedException(" return Strnt");
                case 7: throw new NotImplementedException(" return ldrbt");

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

            var Sadd16 = new InstrDecoder(Opcode.sadd16, "*");
            var Sasx = new InstrDecoder(Opcode.sasx, "*");
            var Ssax = new InstrDecoder(Opcode.ssax, "*");
            var Ssub16 = new InstrDecoder(Opcode.ssub16, "*");
            var Sadd8 = new InstrDecoder(Opcode.sadd8, "*");
            var Ssub8 = new InstrDecoder(Opcode.ssub8, "*");
            var Qadd16 = new InstrDecoder(Opcode.qadd16, "*");
            var Qasx = new InstrDecoder(Opcode.qasx, "*");
            var Qsax = new InstrDecoder(Opcode.qsax, "*");
            var Qsub16 = new InstrDecoder(Opcode.qsub16, "*");
            var Qadd8 = new InstrDecoder(Opcode.qadd8, "*");
            var QSub8 = new InstrDecoder(Opcode.qsub8, "*");
            var Shadd16 = new InstrDecoder(Opcode.shadd16, "*");
            var Shasx = new InstrDecoder(Opcode.shasx, "*");
            var Shsax = new InstrDecoder(Opcode.shsax, "*");
            var Shsub16 = new InstrDecoder(Opcode.shsub16, "*");
            var Shadd8 = new InstrDecoder(Opcode.shadd8, "*");
            var Shsub8 = new InstrDecoder(Opcode.shsub8, "*");
            var Uadd16 = new InstrDecoder(Opcode.uadd16, "*");
            var Uasx = new InstrDecoder(Opcode.uasx, "*");
            var Usax = new InstrDecoder(Opcode.usax, "*");
            var Usub16 = new InstrDecoder(Opcode.usub16, "*");
            var Uadd8 = new InstrDecoder(Opcode.uadd8, "*");
            var Usub8 = new InstrDecoder(Opcode.usub8, "*");
            var Uqadd16 = new InstrDecoder(Opcode.uqadd16, "*");
            var Uqasx = new InstrDecoder(Opcode.uqasx, "*");
            var Uqsax = new InstrDecoder(Opcode.uqsax, "*");
            var Uqsub16 = new InstrDecoder(Opcode.uqsub16, "*");
            var Uqadd8 = new InstrDecoder(Opcode.uqadd8, "*");
            var Uqsub8 = new InstrDecoder(Opcode.uqsub8, "*");
            var Uhadd16 = new InstrDecoder(Opcode.uhadd16, "*");
            var Uhasx = new InstrDecoder(Opcode.uhasx, "*");
            var Uhsax = new InstrDecoder(Opcode.uhsax, "*");
            var Uhsub16 = new InstrDecoder(Opcode.uhsub16, "*");
            var Uhadd8 = new InstrDecoder(Opcode.uhadd8, "*");
            var Uhsub8 = new InstrDecoder(Opcode.uhsub8, "*");

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

            var BitfieldInsert = Select(28, 0xF, n => n != 0xF,
                Instr(Opcode.bfi, "r2,r4,i12:3:6:2,i0:5"),
                Instr(Opcode.bfc, "*"));

            var BitfieldExtract = Mask(22, 1,
                Instr(Opcode.sbfx, "*"),
                Instr(Opcode.ubfx, "r3,r0,i7:5,i16:5+1"));

            var Saturate16Bit = nyi("Saturate16Bit");
            var Saturate32Bit = nyi("Saturate32Bit");
            var ExtendAndAdd = Mask(20, 7,
                Select(16, 0xF, n => n != 0xF, Instr(Opcode.sxtab16, "*"), Instr(Opcode.sxtb16, "*")),
                invalid,
                Select(16, 0xF, n => n != 0xF, Instr(Opcode.sxtab, "*"), Instr(Opcode.sxtb, "r3,r0,i10:2<3")),
                Select(16, 0xF, n => n != 0xF, Instr(Opcode.sxtah, "*"), Instr(Opcode.sxth, "r3,r0,i10:2<3")),
                
                Select(16, 0xF, n => n != 0xF, Instr(Opcode.uxtab16, "*"), Instr(Opcode.uxtb16, "*")),
                invalid,
                Select(16, 0xF, n => n != 0xF, Instr(Opcode.uxtab, "r3,r4,r0,i10:2<3"), Instr(Opcode.uxtb, "r3,r0,i10:2<3")),
                Select(16, 0xF, n => n != 0xF, Instr(Opcode.uxtah, "r3,r4,r0,i10:2<3"), Instr(Opcode.uxth, "r3,r0,i10:2<3")));
            var ReverseBitByte = Mask(22, 1,
                Mask(7, 1,
                    Instr(Opcode.rev, "r3,r0"),
                    Instr(Opcode.rev16, "r3,r0")),
                Mask(7, 1,
                    Instr(Opcode.rbit, "r3,r0"),
                    Instr(Opcode.revsh, "r3,r0")));


            var PermanentlyUndefined = nyi("PermanentlyUndefined");

            var Media = Mask(23, 3,
                ParallelArithmetic,
                Mask(20, 7, 
                    nyi("media1 - 0b01000"),
                    nyi("media1 - 0b01001"),
                    Mask(5, 7,  // op0=0b01010
                        Saturate32Bit,
                        Saturate16Bit,
                        Saturate32Bit,
                        ExtendAndAdd,

                        Saturate32Bit,
                        invalid,
                        Saturate32Bit,
                        invalid),
                    Mask(5, 7,  // media op0=0b01011
                        Saturate32Bit,
                        ReverseBitByte,
                        Saturate32Bit,
                        ExtendAndAdd,

                        Saturate32Bit,
                        ReverseBitByte,
                        Saturate32Bit,
                        invalid),

                    nyi("media1 - 0b01110"),
                    nyi("media1 - 0b01101"),
                    Mask(5, 7,      // media - 0b01110
                        nyi("media1 - 0b01110 - 000"),
                        nyi("media1 - 0b01110 - 001"),
                        nyi("media1 - 0b01110 - 010"),
                        ExtendAndAdd,

                        nyi("media1 - 0b01110 - 100"),
                        nyi("media1 - 0b01110 - 101"),
                        nyi("media1 - 0b01110 - 110"),
                        invalid),
                    Mask(5, 7,
                        Saturate32Bit,
                        ReverseBitByte,
                        Saturate32Bit,
                        ExtendAndAdd,

                        Saturate32Bit,
                        ReverseBitByte,
                        Saturate32Bit,
                        invalid)),
                nyi("media2"),
                Mask(20, 7,
                    nyi("media - 0b11000"),
                    nyi("media - 0b11001"),
                    nyi("media - 0b11010"),
                    nyi("media - 0b11011"),

                    Mask(5, 7, 
                        BitfieldInsert,
                        nyi("media - 0b11100 - 001"),
                        nyi("media - 0b11100 - 010"),
                        nyi("media - 0b11100 - 011"),

                        BitfieldInsert,
                        nyi("media - 0b11100 - 101"),
                        nyi("media - 0b11100 - 110"),
                        invalid),
                    Mask(5, 7, // media - 0b11101
                        BitfieldInsert,
                        invalid,
                        invalid,
                        invalid,

                        BitfieldInsert,
                        invalid,
                        invalid,
                        invalid),
                    Mask(5, 7,
                        nyi("media - 0b11110 - 000"),
                        nyi("media - 0b11110 - 001"),
                        BitfieldExtract,
                        nyi("media - 0b11110 - 011"),

                        nyi("media - 0b11110 - 100"),
                        nyi("media - 0b11110 - 101"),
                        BitfieldExtract,
                        nyi("media - 0b11110 - 111")),
                    Mask(5, 7, // media - 0b11111
                        invalid,
                        invalid,
                        BitfieldExtract,
                        invalid,

                        invalid,
                        invalid,
                        BitfieldExtract,
                        PermanentlyUndefined)));


            var StmdaStmed = new InstrDecoder(Opcode.stmda, "w21 r4,Mr0:16");
            var LdmdaLdmfa = new InstrDecoder(Opcode.ldmda, "w21 r4,Mr0:16");
            var Stm =        new InstrDecoder(Opcode.stm, "w21 r4,Mr0:16");
            var Ldm =        new InstrDecoder(Opcode.ldm, "w21 r4,Mr0:16");
            var StmStmia =   new InstrDecoder(Opcode.stm, "w21 r4,Mr0:16");
            var LdmLdmia =   new InstrDecoder(Opcode.ldm, "w21 r4,Mr0:16");
            var StmdbStmfd = new InstrDecoder(Opcode.stmdb, "w21 r4,Mr0:16");
            var LdmdbLDmea = new InstrDecoder(Opcode.ldmdb, "w21 r4,Mr0:16");
            var StmibStmfa = new InstrDecoder(Opcode.stmib, "w21 r4,Mr0:16");
            var LdmibLdmed = new InstrDecoder(Opcode.ldmib, "w21 r4,Mr0:16");

            var LoadStoreMultiple = new MaskDecoder(22, 7, // P U op
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

            var SystemRegister_64bitMove = new PcDecoder(28, 
                Mask(22, 1,
                    invalid,
                    Mask(20, 1,
                        Instr(Opcode.mcrr, "CP8,i4:4,r3,r4,CR0"),
                        Instr(Opcode.mrrc, "*"))),
                invalid);

            var SystemRegister_LdSt = Mask(23, 2, 21, 1,
                invalid,
                nyi("SystemRegister_LdSt puw=001"),
                nyi("SystemRegister_LdSt puw=010"),
                nyi("SystemRegister_LdSt puw=011"),

                nyi("SystemRegister_LdSt puw=100"),
                nyi("SystemRegister_LdSt puw=101"),
                nyi("SystemRegister_LdSt puw=110"),
                nyi("SystemRegister_LdSt puw=111"));

            var SystemRegister_LdSt_64bitMove = Select(21, 0b1101, n => n == 0,
                SystemRegister_64bitMove,
                SystemRegister_LdSt);

            var FloatingPointDataProcessing3regs = Mask(23, 1, 20, 2, 6, 1,
                Instr(Opcode.vmla, "* floating-point"),
                Instr(Opcode.vmls, "* floating-point"),
                Instr(Opcode.vnmls, "*"),
                Instr(Opcode.vnmla, "*"),

                Instr(Opcode.vmul, "* floating-point"),
                Instr(Opcode.vnmul, "*"),
                Instr(Opcode.vadd, "* floating-point"),
                Mask(8, 0x3,
                    invalid,
                    Instr(Opcode.vsub, "* floating-point size=01"),
                    Instr(Opcode.vsub, "* floating-point size=10"),
                    Instr(Opcode.vsub, "vf64 D22:1:12:4,D7:1:16:4,D5:1:0:4")),

                Instr(Opcode.vdiv, "* vdiv"),
                invalid,
                Instr(Opcode.vfnms, "* vfnms"),
                Instr(Opcode.vfnma, "* vfnma"),

                Instr(Opcode.vfma, "*"),
                Instr(Opcode.vfms, "*"),
                invalid,
                invalid);

            var FloatingPointDataProcessing = new PcDecoder(28,
                Select(20, 0b1011, n => n != 0b1011,
                    FloatingPointDataProcessing3regs,
                    Mask(6, 1,
                        nyi("FloatingPointMoveImmediate"),
                        nyi("FloatingPointDataProcessing - 2 regs"))),
                Select(8, 0b11, n => n == 0,
                    invalid,
                    Select(23, 1, n => n == 0,  // op0 = 0b0xxx
                        Mask(6, 1,
                            invalid,
                            nyi("floating point conditional select")),
                        Select(20, 3, n => n == 0,  // op0 = 0b1x00
                            nyi("floating point minNum/maxNum"),
                            nyi("floating point data processing")))));

            var AdvancedSIMDElementMovDuplicate = Mask(20, 1,
                Mask(23, 1,
                    Instr(Opcode.vmov, "*GP to scalar"),
                    Mask(6, 1,
                        Instr(Opcode.vdup, "vW22:1:5:1q21 W7:1:16:4,r3"),
                        invalid)),
                Instr(Opcode.vmov, "*Scalar to GP"));

            var AdvancedSIMDandFloatingPoint32bitMove = Mask(8, 1,
                    Mask(21, 7,
                        Instr(Opcode.vmov, "S16:4:7:1,r3"),
                        invalid,
                        invalid,
                        invalid,

                        invalid,
                        invalid,
                        invalid,
                        nyi("FloatingPointMoveSpecialReg")),
                    AdvancedSIMDElementMovDuplicate);

            var AdvancedSimd_and_floatingpoint_LdSt = Mask(23, 2, 20, 2,
                invalid,
                invalid,
                nyi("AdvancedSimd_and_floatingpoint_LdSt - PUWL: 0b0010"),
                nyi("AdvancedSimd_and_floatingpoint_LdSt - PUWL: 0b0011"),

                nyi("AdvancedSimd_and_floatingpoint_LdSt - PUWL: 0b0100"),
                Mask(8, 3,
                    nyi("AdvancedSimd_and_floatingpoint_LdSt - PUWL: 0b0101 size: 0b00"),
                    nyi("AdvancedSimd_and_floatingpoint_LdSt - PUWL: 0b0101 size: 0b01"),
                    nyi("AdvancedSimd_and_floatingpoint_LdSt - PUWL: 0b0101 size: 0b10"),
                    Mask(0, 1,
                        Instr(Opcode.vldmia, "w21 r4,Md"),
                        nyi("AdvancedSimd_and_floatingpoint_LdSt - PUWL: 0b0101 size: 0b11 xxxxxx1"))),
                nyi("AdvancedSimd_and_floatingpoint_LdSt - PUWL: 0b0110"),
                Mask(8, 3,
                    nyi("AdvancedSimd_and_floatingpoint_LdSt - PUWL: 0b0111 size: 0b00"),
                    nyi("AdvancedSimd_and_floatingpoint_LdSt - PUWL: 0b0111 size: 0b01"),
                    nyi("AdvancedSimd_and_floatingpoint_LdSt - PUWL: 0b0111 size: 0b10"),
                    Mask(0, 1,
                        Instr(Opcode.vldmia, "w21 r4,Md"),
                        nyi("AdvancedSimd_and_floatingpoint_LdSt - PUWL: 0b0111 size: 0b11 xxxxxx1"))),
                
                Instr(Opcode.vstr, "*"),
                nyi("AdvancedSimd_and_floatingpoint_LdSt - PUWL: 0b1001"),
                Mask(8, 3,
                    nyi("AdvancedSimd_and_floatingpoint_LdSt - PUWL: 0b1010 size: 0b00"),
                    nyi("AdvancedSimd_and_floatingpoint_LdSt - PUWL: 0b1010 size: 0b01"),
                    nyi("AdvancedSimd_and_floatingpoint_LdSt - PUWL: 0b1010 size: 0b10"),
                    Mask(0, 1,
                        Instr(Opcode.vstmdb, "w21 r4,Md"),
                        nyi("AdvancedSimd_and_floatingpoint_LdSt - PUWL: 0b1010 size: 0b11 xxxxxx1"))),
                nyi("AdvancedSimd_and_floatingpoint_LdSt - PUWL: 0b1011"),

                Instr(Opcode.vstr, "*"),
                Mask(8, 3, // size
                    invalid,
                    Instr(Opcode.vldr, "vi16 S12:4:22:1,[i<1:w2]"),
                    Instr(Opcode.vldr, "S12:4:22:1,[i<2:w4]"),
                    Instr(Opcode.vldr, "D22:1:12:4,[i<2:w8]")),
                invalid,
                invalid);

            var AdvancedSimd_and_floatingpoint64bitmove = Mask(22, 1,
                invalid,
                Mask(4, 1, // o3
                    invalid,
                    Select(6, 0x3, n => n != 0, // opc2
                        invalid,
                        Mask(20, 1, // op
                            Mask(8, 3, // size
                                invalid,
                                invalid,
                                nyi("vmov from 2 gp regs to 2 single floats"),
                                Instr(Opcode.vmov, "D5:1:0:4,r3,r4")),
                            Mask(8, 3, // size
                                invalid,
                                invalid,
                                nyi("vmov to 2 gp regs from 2 single floats"),
                                nyi("vmov to 2 gp regs from 1 double float"))))));

            var AdvancedSimd_LdSt_64bitmove = Select(21, 0b1101, n => n == 0,
                AdvancedSimd_and_floatingpoint64bitmove,
                AdvancedSimd_and_floatingpoint_LdSt);

            var SystemRegister32BitMove = nyi("SystemRegister32BitMove");

            var AdvancedSimd_ThreeRegisters = nyi("AdvancedSimd_ThreeRegisters");

            var AdvancedSimd_TwoRegistersScalarExtension = nyi("AdvancedSimd_TwoRegistersScalarExtension");

            Decoder SystemRegister_AdvancedSimd_FloatingPoint = Mask(24, 3,
                Select(9, 7, n => n == 7,
                    SystemRegister_LdSt_64bitMove,
                    new PcDecoder(28, 
                        Select(10, 3, n => n == 2, AdvancedSimd_LdSt_64bitmove, invalid),
                        AdvancedSimd_ThreeRegisters)),
                Select(9, 7, n => n == 7,
                    SystemRegister_LdSt_64bitMove,
                    new PcDecoder(28,
                        Select(10, 3, n => n == 2, AdvancedSimd_LdSt_64bitmove, invalid),
                        AdvancedSimd_ThreeRegisters)),
                Select(9, 7, n => n == 7,
                    Mask(4, 1, invalid, SystemRegister32BitMove),
                    new PcDecoder(28,
                        Select(10, 3, n => n == 2, 
                            Mask(4, 1,
                                FloatingPointDataProcessing,
                                AdvancedSIMDandFloatingPoint32bitMove),
                            invalid),
                        Select(10, 3, n => n == 2, AdvancedSimd_TwoRegistersScalarExtension, invalid))),
                    Instr(Opcode.svc, "i0:24"));
                //SparseMask(9, 0x7, new Dictionary<uint, Decoder>
                //{
                //    { 4, SystemRegister_LdSt_64bitMove },
                //    { 5, SystemRegister_LdSt_64bitMove },
                //    { 7, SystemRegister_LdSt_64bitMove },
                //}),
                //SparseMask(9, 0x7, new Dictionary<uint, Decoder>
                //{
                //    { 4, SystemRegister_LdSt_64bitMove },
                //    { 5, SystemRegister_LdSt_64bitMove },
                //    { 7, SystemRegister_LdSt_64bitMove },
                //}),
                //SparseMask(9, 0x7, new Dictionary<uint, Decoder>
                //{
                //    //$TODO cond fields.
                //    { 4, new MaskDecoder(4, 1,
                //        FloatingPointDataProcessing,
                //        AdvancedSIMDandFloatingPoint32bitMove)
                //    },
                //    { 5, new MaskDecoder(4, 1,
                //        FloatingPointDataProcessing,
                //        AdvancedSIMDandFloatingPoint32bitMove)
                //    },
                //    { 7, new MaskDecoder(4, 1,
                //        FloatingPointDataProcessing,
                //        SystemRegister32BitMove)
                //    }
                //}),
                //new InstrDecoder(Opcode.svc, "V"));

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

            var AdvancedSimd_TwoRegisterOrThreeRegisters = nyi("AdvancedSimd_TwoRegisterOrThreeRegisters");

            var AdvancedSimd_OneRegisterModifiedImmediate = Mask(8, 3, 5, 1,
                Instr(Opcode.vmov, "q6 vi32 W22:1:12:4,is24:1:16:3:0:4"),
                Instr(Opcode.vmvn, "*immediate - A1"),
                Instr(Opcode.vorr, "*immediate - A1"),
                Instr(Opcode.vbic, "*immediate - A1"),

                Instr(Opcode.vmov, "q6 vi32 W22:1:12:4,is24:1:16:3:0:4"),
                Instr(Opcode.vmvn, "*immediate - A1"),
                Instr(Opcode.vorr, "*immediate - A1"),
                Instr(Opcode.vbic, "*immediate - A1"),

                Instr(Opcode.vmov, "q6 vi32 W22:1:12:4,is24:1:16:3:0:4"),
                Instr(Opcode.vmvn, "*immediate - A1"),
                Instr(Opcode.vorr, "*immediate - A1"),
                Instr(Opcode.vbic, "*immediate - A1"),

                Instr(Opcode.vmov, "q6 vi32 W22:1:12:4,is24:1:16:3:0:4"),
                Instr(Opcode.vmvn, "*immediate - A1"),
                Instr(Opcode.vorr, "*immediate - A1"),
                Instr(Opcode.vbic, "*immediate - A1"),

                Instr(Opcode.vmov, "*immediate - A3"),
                Instr(Opcode.vmvn, "*immediate - A2"),
                Instr(Opcode.vorr, "*immediate - A2"),
                Instr(Opcode.vbic, "*immediate - A2"),

                Instr(Opcode.vmov, "*immediate - A3"),
                Instr(Opcode.vmvn, "*immediate - A2"),
                Instr(Opcode.vorr, "*immediate - A2"),
                Instr(Opcode.vbic, "*immediate - A2"),

                Instr(Opcode.vmov, "*immediate - A4"),
                Instr(Opcode.vmvn, "*immediate - A3"),
                Instr(Opcode.vmov, "*immediate - A4"),
                Instr(Opcode.vmvn, "*immediate - A3"),

                Instr(Opcode.vmov, "*immediate - A4"),
                Instr(Opcode.vmov, "*immediate - A5"),
                Instr(Opcode.vmov, "*immediate - A4"),
                invalid);



            var AdvancedSimd_TwoRegisterShiftAmount = nyi("AdvancedSimd_TwoRegisterShiftAmount");

            var AdvancedSimd_ShiftsAndImmediate = Select(7, 0b111000000000001, n => n == 0,
                AdvancedSimd_OneRegisterModifiedImmediate,
                AdvancedSimd_TwoRegisterShiftAmount);

            var AdvancedSimd = Mask(23, 1,
                AdvancedSimd_ThreeRegisters,
                Mask(4, 1,
                    AdvancedSimd_TwoRegisterOrThreeRegisters,
                    AdvancedSimd_ShiftsAndImmediate));

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
        }
    }
}
