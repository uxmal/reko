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
using static Reko.Arch.Arm.AArch32.ArmVectorData;

namespace Reko.Arch.Arm.AArch32
{
    using Mutator = System.Func<uint, A32Disassembler, bool>;

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
            this.state = new DasmState();
            var instr = rootDecoder.Decode(wInstr, this);
            instr.Address = this.addr;
            instr.Length = 4;
            return instr;
        }

        public class DasmState
        {
            public Opcode opcode;
            public List<MachineOperand> ops = new List<MachineOperand>();
            public bool updateFlags = false;
            public bool writeback = false;
            public Opcode shiftOp = Opcode.Invalid;
            public MachineOperand shiftValue = null;
            public bool useQ = false;
            public int? vector_index = null;
            public ArmVectorData vectorData;

            public void Clear()
            {
                ops.Clear();
                updateFlags = false;
                writeback = false;
                shiftOp = Opcode.Invalid;
                shiftValue = null;
                useQ = false;
                vector_index = null;
                vectorData = ArmVectorData.INVALID;
            }

            public void Invalid()
            {
                Clear();
                opcode = Opcode.Invalid;
            }

            public AArch32Instruction MakeInstruction()
            {
                var instr = new AArch32Instruction
                {
                    opcode = opcode,
                    ops = ops.ToArray(),
                    ShiftType = shiftOp,
                    ShiftValue = shiftValue,
                    SetFlags = updateFlags,
                    Writeback = writeback,
                    vector_data = vectorData,
                    vector_index = vector_index,
                };
                return instr;
            }
        }

        private AArch32Instruction Decode(uint wInstr, Opcode opcode, ArmVectorData vectorData, string format)
        {
            var state = new DasmState();
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
                    state.useQ = bit(wInstr, offset);
                    continue;
                case 'w': // sets the writeback bit.
                    ++i;
                    offset = ReadDecimal(format, ref i);
                    state.writeback = bit(wInstr, offset);
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
                    ++i;
                    bool isRegisterPair = PeekAndDiscard('p', format, ref i);
                    offset = (format[i] - '0') * 4;
                    imm = bitmask(wInstr, offset, 0xF);
                    if (isRegisterPair && ((imm & 1) != 0))
                        return Invalid();
                    op = new RegisterOperand(Registers.GpRegs[imm]);
                    if (isRegisterPair)
                    {
                        state.ops.Add(op);
                        op = new RegisterOperand(Registers.GpRegs[imm + 1]);
                    }
                    break;
                case 'W':   // "wector" register
                    ++i;
                    imm = ReadBitfields(wInstr, format, ref i);
                    if (state.useQ)
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
                        (op, state.writeback) = DecodeMemoryAccess(wInstr, memType, shift, dt);
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
                        var baseReg = (int)Bitfield.ReadFields(baseRegFields, wInstr);
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
                    if (PeekAndDiscard('[', format, ref i))
                    {
                        // D13[3] - index into sub-element
                        state.vector_index = (int)ReadBitfields(wInstr, format, ref i);
                        Expect(']', format, ref i);
                    }
                    op = new RegisterOperand(Registers.DRegs[offset]);
                    break;
                case 'E':   // Endianness
                    ++i;
                    imm = ReadBitfields(wInstr, format, ref i);
                    op = new EndiannessOperand(imm != 0);
                    break;
                case 'i':   // General purpose immediate
                    ++i;
                    op = ReadImmediate(wInstr, format, ref i);
                    break;
                case 's':   // use bit 20 to determine if sets flags
                    state.updateFlags = ((wInstr >> 20) & 1) != 0;
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
                    {
                        int sh;
                        (state.shiftOp, sh) = DecodeImmShift(wInstr);
                        if (state.shiftOp != Opcode.Invalid)
                        {
                            state.shiftValue = ImmediateOperand.Int32(sh);
                        }
                    }
                    else if (format[i] == 'R') // rotation as encoded in uxtb / stxb and  friends
                    {
                        ++i;
                        offset = (int)ReadBitfields(wInstr, format, ref i);
                        if (offset == 0)
                        {
                            state.shiftOp = Opcode.Invalid;
                        }
                        else
                        {
                            state.shiftOp = Opcode.ror;
                            state.shiftValue = ImmediateOperand.Int32(offset << 3);
                        }
                    }
                    else
                    {
                        (state.shiftOp, state.shiftValue) = DecodeRegShift(wInstr);
                    }
                    continue;
                case 'B': // Bitfield or Barrier
                    ++i;
                    if (PeekAndDiscard('a', format, ref i))
                    {
                        imm = ReadBitfields(wInstr, format, ref i);
                        op = new BarrierOperand((BarrierOption)imm);
                    }
                    else
                    {
                        // BFI / BFC bit field pair. It's encoded as lsb,msb but needs to be 
                        // decoded as lsb,width
                        var lsb = ReadBitfields(wInstr, format, ref i);
                        state.ops.Add(ImmediateOperand.Int32((int)lsb));
                        Expect(';', format, ref i);
                        var msb = ReadBitfields(wInstr, format, ref i);
                        op = ImmediateOperand.Int32((int)(msb - lsb + 1));
                    }
                    break;
                default:
                    return NotYetImplemented($"Found unknown format character '{format[i]}' in '{format}' while decoding {opcode}.", wInstr);
                }
                state.ops.Add(op);
            }
            var instr = new AArch32Instruction
            {
                opcode = opcode,
                ops = state.ops.ToArray(),
                ShiftType = state.shiftOp,
                ShiftValue = state.shiftValue,
                SetFlags = state.updateFlags,
                Writeback = state.writeback,
                vector_data = vectorData,
                vector_index = state.vector_index,
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
            default: throw new ArgumentException(nameof(bitSize),"Bit size must be 8, 16, or 32.");
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
        private DasmState state;

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
        private uint ReadBitfields(uint wInstr, string format, ref int i)
        {
            uint n = 0;
            string sep = "";
            do
            {
                sep = ",";
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

        private (Opcode, int) DecodeImmShift(uint wInstr)
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
                ? (shift_t, shift_n)
                : (shift_t, 0);
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
            var unrotated_value = wInstr & 0xFF;
            var n = Bits.RotateR32(unrotated_value, 2 * (int)bitmask(wInstr, 8, 0xF));
            return ImmediateOperand.Word32(n);
        }

        /// <summary>
        /// Decode an A32 memory access from the instruction <paramref name="wInstr"/>
        /// </summary>
        /// <returns>A value tuple of the resulting MemoryOperand and whether or not the 
        /// index register should be written back after dereference.
        /// </returns>
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
                offset = Constant.Int32((int)bitmask(wInstr, 0, 0xFFF) << shift);
                m = null;
                break;
            case 'i':   // offset 8 bits
                offset = Constant.Int32((int)bitmask(wInstr, 0, 0xFF) << shift);
                m = null;
                break;
            case 'h':   // offset split in hi-lo nybbles.
                offset = Constant.Int32(
                    (int)(((wInstr >> 4) & 0xF0) | (wInstr & 0x0F)) << shift);
                m = null;
                break;
            case 'x':   // wide shift amt.
                (shiftType, shiftAmt) = DecodeImmShift(wInstr);
                break;
            }
            bool add = bit(wInstr, 23);
            bool preIndex = bit(wInstr, 24);
            bool wback = bit(wInstr, 21);
            bool writeback = !preIndex | wback;
            var mem = new MemoryOperand(dtAccess)
                {
                    BaseRegister = n,
                    Offset = offset,
                    Index = m,
                    Add = add,
                    PreIndex = preIndex,
                    ShiftType = shiftType,
                    Shift = shiftAmt,
            };
            return (mem, writeback);
        }



        private static Mutator vW(int pos1, int size1, int pos2, int size2)
        {
            var fields = new[]
            {
                new Bitfield(pos1, size1),
                new Bitfield(pos2, size2)
            };
            return (u, d) =>
            {
                var imm = Bitfield.ReadFields(fields, u);
                d.state.vectorData = d.VectorElementUntypedReverse(imm);
                return true;
            };
        }

        private static Mutator vi(int offset)
        {
            return (u, d) => { d.state.vectorData = d.VectorElementInteger(offset); return true; };
        }

        private static Mutator vf(int offset)
        {
            return (u, d) => { d.state.vectorData = d.VectorElementFloat(offset); return true; };
        }

        // bit which determines whether or not to use Qx or Dx registers in SIMD
        private static Mutator q(int offset)
        {
            return (u, d) => { d.state.useQ = bit(u, offset); return true; };
        }

        // sets the writeback bit.
        private static Mutator w(int offset)
        {
            return (u, d) => { d.state.writeback = bit(u, offset); return true; };
        }

        // 12-bit encoded immediate at offset 0
        private static Mutator I =>
            (u, d) => { d.state.ops.Add(d.DecodeImm12(u)); return true; };

        // 24-bits at offset 0.
        private static Mutator J =>
            (u, d) =>
            {
                var offset = 8 + (((int)u << 8) >> 6);
                d.state.ops.Add(AddressOperand.Create(d.addr + offset));
                return true;
            };

        // 24-bits at offset 0.
        private static Mutator V =>
            (u, d) =>
            {
                var imm = u & 0x00FFFFFF;
                d.state.ops.Add(ImmediateOperand.Word32(imm));
                return true;
            };

        // 24-bits + extra H bit
        private static Mutator X =>
            (u, d) =>
            {
                var offset = 8 + (((int)u << 8) >> 6);
                offset |= ((int)u >> 23) & 2;
                d.state.ops.Add(AddressOperand.Create(d.addr + offset));
                return true;
            };

        // immediate low 12 bits + extra 4 bits
        private static Mutator Y =>
            (u, d) =>
            {
                var imm = (u & 0xFFF) | ((u >> 4) & 0xF000);
                d.state.ops.Add(ImmediateOperand.Word32(imm));
                return true;
            };

        // immediate low 12 bits + extra 4 bits
        private static Mutator Yh =>
            (u, d) =>
            {
                var imm = (u & 0xFFF) | ((u >> 4) & 0xF000);
                d.state.ops.Add(ImmediateOperand.Word16((ushort)imm));
                return true;
            };

        // register at a 4-bit multiple offset
        private static Mutator r(int offset)
        {
            offset *= 4;
            return (u, d) => {
                var imm = bitmask(u, offset, 0xF);
                d.state.ops.Add(new RegisterOperand(Registers.GpRegs[imm]));
                return true;
            };
        }

        // register pair ]
        private static Mutator rp(int offset)
        {
            offset *= 4;
            return (u, d) =>
            {
                var imm = bitmask(u, offset, 0xF);
                if ((imm & 1) != 0)
                {
                    d.state.Invalid();
                    return false;
                }
                else
                {
                    d.state.ops.Add(new RegisterOperand(Registers.GpRegs[imm]));
                    d.state.ops.Add(new RegisterOperand(Registers.GpRegs[imm + 1]));
                    return true;
                }
            };
        }

        // Vector register
        private static Mutator W(int pos1, int size1, int pos2, int size2)
        {
            var fields = new[]
            {
                new Bitfield(pos1, size1),
                new Bitfield(pos2, size2)
            };
            return (u, d) =>
            {
                var imm = Bitfield.ReadFields(fields, u);
                if (d.state.useQ)
                {
                    if ((imm & 1) == 1)
                    {
                        d.state.Invalid();
                        return false;
                    }
                    else
                    {
                        d.state.ops.Add(new RegisterOperand(Registers.QRegs[imm >> 1]));
                    }
                }
                else
                {
                    d.state.ops.Add(new RegisterOperand(Registers.DRegs[imm]));
                }
                return true;
            };
        }

        private static Mutator Ix(int pos, int size)
        {
            var fields = new[]
            {
                new Bitfield(pos, size)
            };
            return (u, d) =>
            {
                var imm = Bitfield.ReadFields(fields, u);
                d.state.vector_index = (int)imm;
                return true;
            };
        }
        private (MemoryOperand,bool) MakeMemoryOperand(
            uint wInstr, 
            RegisterStorage n,
            RegisterStorage m,
            Constant offset, 
            Opcode shiftType,
            int shiftAmt,
            PrimitiveType dt)
        { 
            bool add = bit(wInstr, 23);
            bool preIndex = bit(wInstr, 24);
            bool wback = bit(wInstr, 21);
            bool writeback = !preIndex | wback;
            var mem = new MemoryOperand(dt)
            {
                BaseRegister = n,
                Offset = offset,
                Index = m,
                Add = add,
                PreIndex = preIndex,
                ShiftType = shiftType,
                Shift = shiftAmt,
            };
            return (mem, writeback);
        }

        // Simple base register access.
        private static Mutator M(int offset, PrimitiveType dt)
        {
            return (u, d) =>
            {
                var iReg = bitmask(u, offset * 4, 0xF);
                var n = Registers.GpRegs[bitmask(u, 16, 0xF)];
                MemoryOperand mem;
                (mem, d.state.writeback) = d.MakeMemoryOperand(u, n, null, null, Opcode.Invalid, 0, dt);
                d.state.ops.Add(mem);
                return true;
            };
        }


        // 12-bit offset
        private static Mutator Mo(PrimitiveType dt)
        {
            return (u, d) =>
            {
                var n = Registers.GpRegs[bitmask(u, 16, 0xF)];
                var offset = Constant.Int32((int)bitmask(u, 0, 0xFFF));
                MemoryOperand mem;
                (mem, d.state.writeback) = d.MakeMemoryOperand(u, n, null, offset, Opcode.Invalid, 0, dt);
                d.state.ops.Add(mem);
                return true;
            };
        }

        private static Mutator M_(PrimitiveType dt)
        {
            return (u, d) =>
            {
                var n = Registers.GpRegs[bitmask(u, 16, 0xF)];
                var m = Registers.GpRegs[bitmask(u, 0, 0x0F)];
                MemoryOperand mem;
                (mem, d.state.writeback) = d.MakeMemoryOperand(u, n, m, null, Opcode.Invalid, 0, dt);
                d.state.ops.Add(mem);
                return true;
            };
        }

        // offset split in hi-lo nybbles.
        private static Mutator Mh(PrimitiveType dt)
        {
            return (u, d) =>
            {
                var n = Registers.GpRegs[bitmask(u, 16, 0xF)];
                var offset = Constant.Int32(
                    (int)(((u >> 4) & 0xF0) | (u & 0x0F)));
                MemoryOperand mem;
                (mem, d.state.writeback) = d.MakeMemoryOperand(u, n, null, offset, Opcode.Invalid, 0, dt);
                d.state.ops.Add(mem);
                return true;
            };
        }

        private static Mutator Mx(PrimitiveType dt)
        {
            return (wInstr, d) =>
            {
                var n = Registers.GpRegs[bitmask(wInstr, 16, 0xF)];
                var m = Registers.GpRegs[bitmask(wInstr, 0, 0x0F)];
                int shiftAmt;
                Opcode shiftType = Opcode.Invalid;
                (shiftType, shiftAmt) = d.DecodeImmShift(wInstr);
                MemoryOperand mem;
                (mem, d.state.writeback) = d.MakeMemoryOperand(wInstr, n, m, null, shiftType, shiftAmt, dt);
                d.state.ops.Add(mem);
                return true;
            };
        }

        private static Mutator Mi(int shift, PrimitiveType dt)
        {
            return (u, d) =>
            {
                var n = Registers.GpRegs[bitmask(u, 16, 0xF)];
                var offset = Constant.Int32((int)bitmask(u, 0, 0xFF) << shift);
                MemoryOperand mem;
                (mem, d.state.writeback) = d.MakeMemoryOperand(u, n, null, offset, Opcode.Invalid, 0, dt);
                d.state.ops.Add(mem);
                return true;
            };
        }

        private static  PrimitiveType w1 => PrimitiveType.Byte;
        private static PrimitiveType w2 => PrimitiveType.Word16;
        private static PrimitiveType w4 => PrimitiveType.Word32;
        private static PrimitiveType w8 => PrimitiveType.Word64;
        private static PrimitiveType s1 => PrimitiveType.SByte;
        private static PrimitiveType s2 => PrimitiveType.Int16;
        private static PrimitiveType s4 => PrimitiveType.Int32;
        private static PrimitiveType s8 => PrimitiveType.Int64;

        //case '[':
        //    {
        //        int shift = 0;
        //        ++i;
        //        var memType = format[i];
        //        ++i;
        //        if (PeekAndDiscard('<', format, ref i))
        //        {
        //            shift = ReadDecimal(format, ref i);
        //        }
        //        Expect(':', format, ref i);
        //        var dom = format[i];
        //        ++i;
        //        var size = format[i] - '0';
        //        ++i;
        //        var dt = GetDataType(dom, size);
        //        (op, writeback) = DecodeMemoryAccess(u, memType, shift, dt);
        //    }
        //    break;


        // Multiple registers
        private static Mutator Mr(int pos, int size) {
            var bitfields = new[]
            {
                new Bitfield(pos, size)
            };
            return (u, d) =>
            {
                var imm = Bitfield.ReadFields(bitfields, u);
                d.state.ops.Add(new MultiRegisterOperand(Registers.GpRegs, PrimitiveType.Word16, (ushort)imm));
                return true;
            };
        }

        private static Bitfield[] baseRegFields = new[]
{
                            new Bitfield(22, 1), new Bitfield(12, 4)
                        };

        private static Mutator Md(int pos, int size)
        {
            var bitfields = new[]
            {
                new Bitfield(pos, size)
            };
            return (u, d) =>
            {
                var imm = Bitfield.ReadFields(bitfields, u);
                var baseReg = (int)Bitfield.ReadFields(baseRegFields, u);
                var regs = d.SBitfield(u, 1, 7);
                var bitmask = (((1u << regs) - 1u) << baseReg);
                d.state.ops.Add(new MultiRegisterOperand(Registers.DRegs, PrimitiveType.Word64, bitmask));
                return true;
            };
        }

        private static Mutator SR =>
            (u, d) =>
            {
                var sr = bit(u, 22) ? Registers.spsr : Registers.cpsr;
                d.state.ops.Add(new RegisterOperand(sr));
                return true;
            };

        // Single precision register
        private static Mutator S(int pos1, int size1, int pos2, int size2)
        {
            var fields = new[]
            {
                new Bitfield(pos1, size1),
                new Bitfield(pos2, size2)
            };
            return (u, d) =>
            {
                var iReg = Bitfield.ReadFields(fields, u);
                d.state.ops.Add(new RegisterOperand(Registers.SRegs[iReg]));
                return true;
            };
        }

        private static Mutator D(int pos1, int size1, int pos2, int size2)
        {
            var fields = new[]
            {
                new Bitfield(pos1, size1),
                new Bitfield(pos2, size2)
            };
            return (u, d) =>
            {
                var iReg = Bitfield.ReadFields(fields, u);
                d.state.ops.Add(new RegisterOperand(Registers.DRegs[iReg]));
                return true;
            };
        }

        //if (PeekAndDiscard('[', format, ref i))
        //{
        //    // D13[3] - index into sub-element
        //    vector_index = (int)ReadBitfields(u, format, ref i);
        //    Expect(']', format, ref i);
        //}

        // Endianness
        private static Mutator E(int pos, int size)
        {
            var fields = new[]
            {
                new Bitfield(pos, size),
            };
            return (u, d) =>
            {
                var imm = Bitfield.ReadFields(fields, u);
                d.state.ops.Add(new EndiannessOperand(imm != 0));
                return true;
            };
        }

        private static Mutator i(int pos, int size)
        {
            var fields = new[]
            {
                new Bitfield(pos, size),
            };
            return (u, d) =>
            {
                var imm = Bitfield.ReadFields(fields, u);
                d.state.ops.Add(ImmediateOperand.Word32(imm));
                return true;
            };
        }

        private static Mutator i_p1(int pos, int size)
        {
            var fields = new[]
            {
                new Bitfield(pos, size),
            };
            return (u, d) =>
            {
                var imm = Bitfield.ReadFields(fields, u);
                d.state.ops.Add(ImmediateOperand.Word32(imm+1));
                return true;
            };
        }

        private static Mutator ih(int pos, int size)
        {
            var fields = new[]
            {
                new Bitfield(pos, size),
            };
            return (u, d) =>
            {
                var imm = Bitfield.ReadFields(fields, u);
                d.state.ops.Add(ImmediateOperand.Word16((ushort)imm));
                return true;
            };
        }

        private static Mutator Is(int pos1, int size1, int pos2, int size2, int pos3, int size3)
        {
            var fields = new[]
            {
                new Bitfield(pos1, size1),
                new Bitfield(pos2, size2),
                new Bitfield(pos3, size3),
            };
            return (u, d) =>
            {
                var imm = Bitfield.ReadFields(fields, u);
                var cmode = (u >> 8) & 0xF;
                var op = (u >> 5) & 1;
                d.state.ops.Add(ImmediateOperand.Word64(d.SimdExpandImm(op, cmode, (uint)imm)));
                return true;
            };
        }

        // use bit 20 to determine if sets flags
        private static Mutator s =>
            (u, d) => {
                d.state.updateFlags = ((u >> 20) & 1) != 0;
                return true;
            };


        // Coprocessor #
        private static Mutator CP(int n)
        {
            return (u, d) =>
            {
                d.state.ops.Add(d.Coprocessor(u, n));
                return true;
            };
        }

        // Coprocessor register
        private static Mutator CR(int offset)
        {
            return (u, d) => {
                d.state.ops.Add(d.CoprocessorRegister(u, offset));
                return true;
            };
        }

        // '>i' immediate shift
        private static Mutator Shi =>
            (u, d) =>
            {
                int sh;
                (d.state.shiftOp, sh) = d.DecodeImmShift(u);
                if (d.state.shiftOp != Opcode.Invalid)
                {
                    d.state.shiftValue = ImmediateOperand.Int32(sh);
                }
                return true;
            };

        // >R:  rotation as encoded in uxtb / stxb and  friends
        private static Mutator ShR(int pos, int size)
        {
            var bitfields = new Bitfield[]
            {
                new Bitfield(pos, size)
            };
            return (u, d) =>
            {
                var offset = (int)Bitfield.ReadFields(bitfields, u);
                if (offset == 0)
                {
                    d.state.shiftOp = Opcode.Invalid;
                }
                else
                {
                    d.state.shiftOp = Opcode.ror;
                    d.state.shiftValue = ImmediateOperand.Int32(offset << 3);
                }
                return true;
            };
        }

        private static Mutator x(string message)
        {
            return (u, d) =>
            {
                var op = d.state.opcode.ToString();
                if (message == "")
                    message = op;
                else
                    message = $"{op} - {message}";
                d.NotYetImplemented(message, u);
                d.Invalid();
                return false;
            };
        }

        // >r : register shift
        private static Mutator Shr =>
            (u, d) => {
                (d.state.shiftOp, d.state.shiftValue) = d.DecodeRegShift(u);
                return true;
            };


        // Ba => barrier
        private static Mutator Ba(int pos, int size)
        {
            var bitfield = new[] { new Bitfield(pos, size) };
            return (u, d) =>
            {
                var imm = Bitfield.ReadFields(bitfield, u);
                d.state.ops.Add(new BarrierOperand((BarrierOption)imm));
                return true;
            };
        }

        // BFI / BFC bit field pair. It's encoded as lsb,msb but needs to be 
        // decoded as lsb,width
        private static Mutator B(int lsbPos, int lsbSize, int msbPos, int msbSize)
        {
            var lsbField = new[]
            {
                new Bitfield(lsbPos, lsbSize),
            };
            var msbField = new[]
            {
                new Bitfield(msbPos, msbSize)
            };
            return (u, d) =>
            {
                var lsb = Bitfield.ReadFields(lsbField, u);
                var msb = Bitfield.ReadFields(msbField, u);
                d.state.ops.Add(ImmediateOperand.Int32((int)lsb));
                d.state.ops.Add(ImmediateOperand.Int32((int)(msb - lsb + 1)));
                return true;
            };
         }
















        private static Decoder Instr(Opcode opcode, params Mutator[] mutators)
        {
            return new InstrDecoder(opcode, ArmVectorData.INVALID, mutators);
        }

        private static Decoder Instr(Opcode opcode, ArmVectorData vec, params Mutator[] mutators)
        {
            return new InstrDecoder(opcode, vec, mutators);
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
            invalid = new InstrDecoder(Opcode.Invalid, ArmVectorData.INVALID);

            var LoadStoreExclusive = nyi("LoadStoreExclusive");

            var Stl = Instr(Opcode.stl, x(""));
            var Stlex = Instr(Opcode.stlex, x(""));
            var Strex = Instr(Opcode.strex, x(""));
            var Lda = Instr(Opcode.lda, x(""));
            var Ldaex = Instr(Opcode.ldaex, x(""));
            var Ldrex = Instr(Opcode.ldrex, x(""));
            var Stlexd = Instr(Opcode.stlexd, x(""));
            var Strexd = Instr(Opcode.strexd, x(""));
            var Ldaexd = Instr(Opcode.ldaexd, x(""));
            var Ldrexd = Instr(Opcode.ldrexd, x(""));
            var Stlb = Instr(Opcode.stlb, x(""));
            var Stlexb = Instr(Opcode.stlexb, x(""));
            var Strexb = Instr(Opcode.strexb, x(""));
            var Ldab = Instr(Opcode.ldab, x(""));
            var Ldaexb = Instr(Opcode.ldrexb, x(""));
            var Ldrexb = Instr(Opcode.ldaexb, x(""));
            var Stlh = Instr(Opcode.stlh, x(""));
            var Stlexh = Instr(Opcode.stlexh, x(""));
            var Strexh = Instr(Opcode.strexh, x(""));
            var Ldah = Instr(Opcode.ldah, x(""));
            var Ldaexh = Instr(Opcode.ldrexh, x(""));
            var Ldrexh = Instr(Opcode.ldaexh, x(""));

            var SynchronizationPrimitives = new MaskDecoder(23, 1,
                Mask(22, 1,
                    Instr(Opcode.swp, r(3), r(0), M(4,w4)),     //$TODO: deprecated in ARMv6 and later.
                    Instr(Opcode.swpb, r(3), r(0), M(4,w1))),   //$TODO: deprecated in ARMv6 and later.
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

            var Mul = Instr(Opcode.mul, s,r(4),r(0),r(2));
            var Mla = Instr(Opcode.mla, s,r(4),r(0),r(2),r(3));
            var Mls = Instr(Opcode.mls, s,r(4),r(0),r(2),r(3));
            var Umaal = Instr(Opcode.umaal, s,r(3),r(4),r(0),r(2));
            var Umull = Instr(Opcode.umull, s,r(3),r(4),r(0),r(2));
            var Umlal = Instr(Opcode.umlal, s,r(3),r(4),r(0),r(2));
            var Smull = Instr(Opcode.smull, s,r(3),r(4),r(0),r(2));
            var Smlal = Instr(Opcode.smlal, s,r(3),r(4),r(0),r(2));

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
            var LdrdRegister = Instr(Opcode.ldrd, rp(3),M_(w8));
            var LdrhRegister = Instr(Opcode.ldrh, r(3),M_(w2));
            var LdrsbRegister = Instr(Opcode.ldrsb, r(3),M_(s1));
            var LdrshRegister = Instr(Opcode.ldrsh, r(3),M_(s2));
            var Ldrht = Instr(Opcode.ldrht, r(3),Mh(w2));
            var Ldrsbt = Instr(Opcode.ldrsbt, r(3),Mh(s1));
            var Ldrsht = Instr(Opcode.ldrsht, r(3),Mh(s2));
            var StrdRegister = Instr(Opcode.strd, rp(3),Mx(w8));
            var StrhRegister = Instr(Opcode.strh, r(3),M_(w2));
            var Strht = Instr(Opcode.strht, r(3),Mh(w2));

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

            var LdrdLiteral = Instr(Opcode.ldrd, rp(3), x("*)"));
            var LdrhLiteral = Instr(Opcode.ldrh, x(""));
            var LdrsbLiteral = Instr(Opcode.ldrsb, x(""));
            var LdrshLiteral = Instr(Opcode.ldrsh, x(""));
            var StrhImmediate = Instr(Opcode.strh, x(""));
            var LdrdImmediate = Instr(Opcode.ldrd, rp(3),Mh(w8));
            var StrdImmediate = Instr(Opcode.strd, rp(3),Mh(w8));
            var LdrhImmediate = Instr(Opcode.ldrh, x(""));
            var LdrsbImmediate = Instr(Opcode.ldrsb, r(3),Mh(s1));
            var LdrshImmediate = Instr(Opcode.ldrsh, x(""));

            var LoadStoreDualHalfSbyteImmediate = Mask(24, 1, 20, 2, // LoadStoreDualHalfSbyteImmediate Rn != pc P:W:op1
                    Mask(5, 3, // LoadStoreDualHalfSbyteImmediate Rn != pc P:W:op1=000 op2
                        invalid,
                        StrhImmediate,
                        new PcDecoder(16, LdrdImmediate, LdrdLiteral),
                        StrdImmediate),
                    Mask(5, 3, // LoadStoreDualHalfSbyteImmediate Rn != pc P:W:op1=001 op2
                        invalid,
                        new PcDecoder(16, LdrhImmediate, LdrhLiteral),
                        new PcDecoder(16, LdrsbImmediate, LdrsbLiteral),
                        new PcDecoder(16, LdrshImmediate, LdrshLiteral)),
                    Mask(5, 3, // LoadStoreDualHalfSbyteImmediate Rn != pc P:W:op1=010 op2
                        invalid,
                        Strht,
                        new PcDecoder(16, LdrdImmediate, LdrdLiteral),
                        invalid),
                    Mask(5, 3, // LoadStoreDualHalfSbyteImmediate Rn != pc P:W:op1=011 op2
                        invalid,
                        Ldrht,
                        Ldrsbt,
                        Ldrsht),
                    Mask(5, 3, // LoadStoreDualHalfSbyteImmediate Rn != pc P:W:op1=100 op2
                        invalid,
                        StrhImmediate,
                        new PcDecoder(16, LdrdImmediate, LdrdLiteral),
                        StrdImmediate),
                    Mask(5, 3, // LoadStoreDualHalfSbyteImmediate Rn != pc P:W:op1=101 op2
                        invalid,
                        new PcDecoder(16, LdrhImmediate, LdrhLiteral),
                        new PcDecoder(16, LdrsbImmediate, LdrsbLiteral),
                        new PcDecoder(16, LdrshImmediate, LdrshLiteral)),
                    Mask(5, 3, // LoadStoreDualHalfSbyteImmediate Rn != pc P:W:op1=110 op2
                        invalid,
                        StrhImmediate,
                        new PcDecoder(16, LdrdImmediate, LdrdLiteral),
                        new PcDecoder(16, LdrshImmediate, LdrshLiteral)),
                    Mask(5, 3, // LoadStoreDualHalfSbyteImmediate Rn != pc P:W:op1=111 op2
                        invalid,
                        new PcDecoder(16, LdrhImmediate, LdrhLiteral),
                        new PcDecoder(16, LdrsbImmediate, LdrsbLiteral),
                        new PcDecoder(16, LdrshImmediate, LdrshLiteral)));

            var LoadStoreDualHalfSbyteImmediate2 = new CustomDecoder((wInstr, dasm) =>
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

            var Mrs = Instr(Opcode.mrs, r(3),SR);
            var Msr = Instr(Opcode.msr, SR,r(0));
            var MrsBanked = Instr(Opcode.mrs, x(""));
            var MsrBanked = Instr(Opcode.msr, x(""));
            var MoveSpecialRegister = new MaskDecoder(21, 1,
                new MaskDecoder(9, 1,
                    Mrs,
                    MrsBanked),
                new MaskDecoder(9, 1,
                    Msr,
                    MsrBanked));

            var Crc32 = Instr(Opcode.crc32w, x(""));
            var Crc32C = Instr(Opcode.crc32cw, x(""));

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

            var Qadd = Instr(Opcode.qadd, r(3),r(0),r(4));
            var Qsub = Instr(Opcode.qsub, r(3),r(0),r(4));
            var Qdadd = Instr(Opcode.qdadd, r(3),r(0),r(4));
            var Qdsub = Instr(Opcode.qdsub, r(3),r(0),r(4));
            var IntegerSaturatingArithmetic = new MaskDecoder(21, 3,
                Qadd,
                Qsub,
                Qdadd,
                Qdsub);


            var Hlt = Instr(Opcode.hlt, x(""));
            var Bkpt = Instr(Opcode.bkpt, x(""));
            var Hvc = Instr(Opcode.hvc, x(""));
            var Smc = Instr(Opcode.smc, x(""));
            var ExceptionGeneration = new MaskDecoder(21, 3,
                Hlt,
                Bkpt,
                Hvc,
                Smc);

            var Bx = Instr(Opcode.bx, r(0));
            var Bxj = Instr(Opcode.bxj, r(0));
            var Blx = Instr(Opcode.blx, J);
            var Clz = Instr(Opcode.clz, r(3),r(0));
            var Eret = Instr(Opcode.eret, x(""));

            var ChangeProcessState = new MaskDecoder(16, 1, // op
                Mask(18, 0x3,
                    nyi("CPS,CPSID,CPSIE mask=0b00"),
                    nyi("CPS,CPSID,CPSIE mask=0b01"),
                    nyi("CPS,CPSID,CPSIE mask=0b10"),
                    Mask(17, 1,  // mask=0b11 M
                        Instr(Opcode.cps, x("*")),
                        nyi("CPS,CPSID,CPSIE mask=0b11 m=1"))),
                Select(4, 1, n => n == 0, Instr(Opcode.setend, E(9,1)), invalid));

            var UncMiscellaneous = new MaskDecoder(22, 7,   // op0
                invalid,
                invalid,
                invalid,
                invalid,

                new MaskDecoder(20, 3,
                    Select(5, 1, n => n == 0, ChangeProcessState, invalid),
                    Select(4, 0xF, n => n == 0, Instr(Opcode.setpan, x("")), invalid),
                    invalid,
                    invalid),
                invalid,
                invalid,
                invalid);

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
                Mask(5, 3,      // M:N
                    Instr(Opcode.smlabb, r(4),r(0),r(2),r(3)),
                    Instr(Opcode.smlatb, r(4),r(0),r(2),r(3)),
                    Instr(Opcode.smlabt, r(4),r(0),r(2),r(3)),
                    Instr(Opcode.smlatt, r(4),r(0),r(2),r(3))),
                Mask(5, 3,
                    Instr(Opcode.smlawb, r(4),r(0),r(2),r(3)),
                    Instr(Opcode.smulwb, r(4),r(0),r(2)),
                    Instr(Opcode.smlawt, r(4),r(0),r(2),r(3)),
                    Instr(Opcode.smulwt, r(4),r(0),r(2))),
                Mask(5, 3,
                    Instr(Opcode.smlalbb, r(3),r(4),r(0),r(2)),
                    Instr(Opcode.smlaltb, r(3),r(4),r(0),r(2)),
                    Instr(Opcode.smlalbt, r(3),r(4),r(0),r(2)),
                    Instr(Opcode.smlaltt, r(3),r(4),r(0),r(2))),
                Mask(5, 3,
                    Instr(Opcode.smulbb, r(4),r(0),r(2)),
                    Instr(Opcode.smultb, r(4),r(0),r(2)),
                    Instr(Opcode.smulbt, r(4),r(0),r(2)),
                    Instr(Opcode.smultt, r(4),r(0),r(2))));

            var IntegerDataProcessingImmShift = new MaskDecoder(21, 7,
                Instr(Opcode.and, s,r(3),r(4),r(0),Shi),
                Instr(Opcode.eor, s,r(3),r(4),r(0),Shi),
                Instr(Opcode.sub, s,r(3),r(4),r(0),Shi),
                Instr(Opcode.rsb, s,r(3),r(4),r(0),Shi),
                Instr(Opcode.add, s,r(3),r(4),r(0),Shi),
                Instr(Opcode.adc, s,r(3),r(4),r(0),Shi),
                Instr(Opcode.sbc, s,r(3),r(4),r(0),Shi),
                Instr(Opcode.rsc, s,r(3),r(4),r(0),Shi));

            var IntegerTestAndCompareImmShift = new MaskDecoder(21, 3,
                Instr(Opcode.tst, r(4),r(0),Shi),
                Instr(Opcode.teq, r(4),r(0),Shi),
                Instr(Opcode.cmp, r(4),r(0),Shi),
                Instr(Opcode.cmn, r(4),r(0),Shi));

            var LogicalArithmeticImmShift = new MaskDecoder(21, 3,
                Instr(Opcode.orr, s,r(3),r(4),r(0),Shi),
                new MovDecoder(Opcode.mov, s,r(3),r(0),Shi),
                Instr(Opcode.bic, s,r(3),r(4),r(0),Shi),
                Instr(Opcode.mvn, s,r(3),r(0),Shi));

            var DataProcessingImmediateShift = new MaskDecoder(23, 3,
                IntegerDataProcessingImmShift, // 3 reg, imm shift
                IntegerDataProcessingImmShift,
                IntegerTestAndCompareImmShift,
                LogicalArithmeticImmShift);

            var IntegerDataProcessingRegShift = new MaskDecoder(21, 7,
               Instr(Opcode.and, s,r(3),r(4),r(0),Shr),
               Instr(Opcode.eor, s,r(3),r(4),r(0),Shr),
               Instr(Opcode.sub, s,r(3),r(4),r(0),Shr),
               Instr(Opcode.rsb, s,r(3),r(4),r(0),Shr),
               Instr(Opcode.add, s,r(3),r(4),r(0),Shr),
               Instr(Opcode.adc, s,r(3),r(4),r(0),Shr),
               Instr(Opcode.sbc, s,r(3),r(4),r(0),Shr),
               Instr(Opcode.rsc, s,r(3),r(4),r(0),Shr));

            var IntegerTestAndCompareRegShift = new MaskDecoder(21, 3,
                Instr(Opcode.tst, r(4),r(0),Shr),
                Instr(Opcode.teq, r(4),r(0),Shr),
                Instr(Opcode.cmp, r(4),r(0),Shr),
                Instr(Opcode.cmn, r(4),r(0),Shr));

            var LogicalArithmeticRegShift = new MaskDecoder(21, 3,
                Instr(Opcode.orr, s,r(3),r(4),r(0),Shr),
                Instr(Opcode.mov, s,r(4),r(0),Shr),
                Instr(Opcode.bic, s,r(3),r(4),r(0),Shr),
                Instr(Opcode.mvn, s,r(4),r(0),Shr));

            var DataProcessingRegisterShift = new MaskDecoder(23, 3,
                IntegerDataProcessingRegShift,
                IntegerDataProcessingRegShift,
                IntegerTestAndCompareRegShift,
                LogicalArithmeticRegShift);

            var IntegerDataProcessingTwoRegImm = new MaskDecoder(21, 7,
               Instr(Opcode.and, s,r(3),r(4),I),
               Instr(Opcode.eor, s,r(3),r(4),I),
               Instr(Opcode.sub, s,r(3),r(4),I),
               Instr(Opcode.rsb, s,r(3),r(4),I),
               Instr(Opcode.add, s,r(3),r(4),I),
               Instr(Opcode.adc, s,r(3),r(4),I),
               Instr(Opcode.sbc, s,r(3),r(4),I),
               Instr(Opcode.rsc, s,r(3),r(4),I));

            var LogicalArithmeticTwoRegImm = new MaskDecoder(21, 3,
                Instr(Opcode.orr, s,r(3),r(4),I),
                Instr(Opcode.mov, s,r(3),I),
                Instr(Opcode.bic, s,r(3),r(4),I),
                Instr(Opcode.mvn, s,r(3),I));

            var MoveHalfwordImm = new MaskDecoder(22, 1,
               Instr(Opcode.mov, r(3),Y),
               Instr(Opcode.movt, r(3),Yh));

            var IntegerTestAndCompareOneRegImm = new MaskDecoder(21, 3,
                Instr(Opcode.tst, r(4),I),
                Instr(Opcode.teq, r(4),I),
                Instr(Opcode.cmp, r(4),I),
                Instr(Opcode.cmn, r(4),I));

            var MsrImmediate = Instr(Opcode.msr, SR,i(0,12));
            var Nop = Instr(Opcode.nop);
            var Yield = Instr(Opcode.yield, x(""));
            var Wfe = Instr(Opcode.wfe, x(""));
            var Wfi = Instr(Opcode.wfi, x(""));
            var Sev = Instr(Opcode.sevl, x(""));
            var Sevl = Instr(Opcode.sevl, x(""));
            var ReservedNop = Instr(Opcode.nop);
            var Esb = Instr(Opcode.esb, x(""));
            var Dbg = Instr(Opcode.dbg, x(""));

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

            var DataProcessingAndMisc = Mask(25, 1,
                Mask(7, 1, 4, 1, // DataProcessingAndMisc op0=0 op2:op4
                    Select(20, 0b11001, n => n == 0b10000,
                        Miscellaneous,
                        DataProcessingImmediateShift),
                    Select(20, 0b11001, n => n == 0b10000,
                        Miscellaneous,
                        DataProcessingRegisterShift),
                    Select(20, 0b11001, n => n == 0b10000,
                        HalfwordMultiplyAndAccumulate,
                        DataProcessingImmediateShift),
                    Mask(5, 3, // DataProcessingAndMisc op0=0 op2:op4=11 op3
                        Mask(24, 1, // DataProcessingAndMisc op0=0 op2=1 op4=1 op3=0b00 op1
                            MultiplyAndAccumulate,
                            SynchronizationPrimitives),
                        ExtraLoadStore,
                        ExtraLoadStore,
                        ExtraLoadStore)),
                DataProcessingImmediate);

            var LdrLiteral = Instr(Opcode.ldr, r(3),Mo(w4));
            var LdrbLiteral = Instr(Opcode.ldrb, r(3),Mo(w1));
            var StrImm = Instr(Opcode.str, r(3),Mo(w4));
            var LdrImm = Instr(Opcode.ldr, r(3),Mo(w4));
            var StrbImm = Instr(Opcode.strb, r(3),Mo(w1));
            var LdrbImm = Instr(Opcode.ldrb, r(3),Mo(w1));
            
            var LoadStoreWordUnsignedByteImmLit = Mask(24, 1, 21, 1, 22, 1, 20, 1,
                // PW=0b00 00
                Instr(Opcode.str, r(3),Mo(w4)),
                Select(16, 0xF, n => n != 0xF,
                    Instr(Opcode.ldr, r(3),Mo(w4)),
                    Instr(Opcode.ldr, r(3),Mo(w4))),
                Instr(Opcode.strb, r(3),Mo(w1)),
                Select(16, 0xF, n => n != 0xF,
                    Instr(Opcode.ldrb, r(3),Mo(w1)),
                    Instr(Opcode.ldrb, r(3),Mo(w1))),

                Instr(Opcode.strt, r(3),Mo(w4)),
                Instr(Opcode.ldrt, r(3),Mo(w4)),
                Instr(Opcode.strbt, r(3),Mo(w1)),
                Instr(Opcode.ldrbt, r(3),Mo(w1)),

                Instr(Opcode.str, r(3),Mo(w4)),
                Select(16, 0xF, n => n != 0xF,
                    Instr(Opcode.ldr, r(3),Mo(w4)),
                    Instr(Opcode.ldr, r(3),Mo(w4))),
                Instr(Opcode.strb, r(3),Mo(w1)),
                Select(16, 0xF, n => n != 0xF,
                    Instr(Opcode.ldrb, r(3),Mo(w1)),
                    Instr(Opcode.ldrb, r(3),Mo(w1))),

                Instr(Opcode.str, r(3),Mo(w4)),
                Select(16, 0xF, n => n != 0xF,
                    Instr(Opcode.ldr, r(3),Mo(w4)),
                    Instr(Opcode.ldr, r(3),Mo(w4))),
                Instr(Opcode.strb, r(3),Mo(w1)),
                Select(16, 0xF, n => n != 0xF,
                    Instr(Opcode.ldrb, r(3),Mo(w1)),
                    Instr(Opcode.ldrb, r(3),Mo(w1))));

            var StrReg = Instr(Opcode.str, r(3),Mx(w4));
            var LdrReg = Instr(Opcode.ldr, r(3),Mx(w4));
            var StrbReg = Instr(Opcode.strb, r(3),Mx(w1));
            var LdrbReg = Instr(Opcode.ldrb, r(3),Mx(w1));
            var StrtReg = Instr(Opcode.strt, r(3),Mx(w4));
            var LdrtReg = Instr(Opcode.strt, r(3),Mx(w4));
            var StrbtReg = Instr(Opcode.strbt, r(3),Mx(w1));
            var LdrbtReg = Instr(Opcode.strbt, r(3),Mx(w1));
            var LoadStoreWordUnsignedByteRegister = Mask(24, 1, 20, 3,  // P:o2:W:o1
                 StrReg,
                 LdrReg,
                 StrtReg,
                 LdrtReg,

                 StrbReg,
                 LdrbReg,
                 StrbtReg,
                 LdrbtReg,

                 StrReg,
                 LdrReg,
                 StrReg,
                 LdrReg,

                 StrbReg,
                 LdrbReg,
                 StrbReg,
                 LdrbReg);

            var Sadd16 = Instr(Opcode.sadd16, x(""));
            var Sasx = Instr(Opcode.sasx, x(""));
            var Ssax = Instr(Opcode.ssax, x(""));
            var Ssub16 = Instr(Opcode.ssub16, x(""));
            var Sadd8 = Instr(Opcode.sadd8, x(""));
            var Ssub8 = Instr(Opcode.ssub8, x(""));
            var Qadd16 = Instr(Opcode.qadd16, x(""));
            var Qasx = Instr(Opcode.qasx, x(""));
            var Qsax = Instr(Opcode.qsax, x(""));
            var Qsub16 = Instr(Opcode.qsub16, x(""));
            var Qadd8 = Instr(Opcode.qadd8, x(""));
            var QSub8 = Instr(Opcode.qsub8, x(""));
            var Shadd16 = Instr(Opcode.shadd16, x(""));
            var Shasx = Instr(Opcode.shasx, x(""));
            var Shsax = Instr(Opcode.shsax, x(""));
            var Shsub16 = Instr(Opcode.shsub16, x(""));
            var Shadd8 = Instr(Opcode.shadd8, x(""));
            var Shsub8 = Instr(Opcode.shsub8, x(""));
            var Uadd16 = Instr(Opcode.uadd16, x(""));
            var Uasx = Instr(Opcode.uasx, x(""));
            var Usax = Instr(Opcode.usax, x(""));
            var Usub16 = Instr(Opcode.usub16, x(""));
            var Uadd8 = Instr(Opcode.uadd8, x(""));
            var Usub8 = Instr(Opcode.usub8, x(""));
            var Uqadd16 = Instr(Opcode.uqadd16, x(""));
            var Uqasx = Instr(Opcode.uqasx, x(""));
            var Uqsax = Instr(Opcode.uqsax, x(""));
            var Uqsub16 = Instr(Opcode.uqsub16, x(""));
            var Uqadd8 = Instr(Opcode.uqadd8, x(""));
            var Uqsub8 = Instr(Opcode.uqsub8, x(""));
            var Uhadd16 = Instr(Opcode.uhadd16, x(""));
            var Uhasx = Instr(Opcode.uhasx, x(""));
            var Uhsax = Instr(Opcode.uhsax, x(""));
            var Uhsub16 = Instr(Opcode.uhsub16, x(""));
            var Uhadd8 = Instr(Opcode.uhadd8, x(""));
            var Uhsub8 = Instr(Opcode.uhsub8, x(""));

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

            var BitfieldInsert = Select(0, 0xF, n => n != 0xF,
                Instr(Opcode.bfi, r(3),r(0),B(7,5,16,5)),
                Instr(Opcode.bfc, r(3),B(7,5,16,5)));

            var BitfieldExtract = Mask(22, 1,
                Instr(Opcode.sbfx, r(3),r(0),i(7,5),i_p1(16,5)),
                Instr(Opcode.ubfx, r(3),r(0),i(7,5),i_p1(16,5)));

            var Saturate16Bit = nyi("Saturate16Bit");
            var Saturate32Bit = nyi("Saturate32Bit");
            var ExtendAndAdd = Mask(20, 7,
                Select(16, 0xF, n => n != 0xF, Instr(Opcode.sxtab16, x("*")), Instr(Opcode.sxtb16, x("*"))),
                invalid,
                Select(16, 0xF, n => n != 0xF, Instr(Opcode.sxtab, r(3),r(4),r(0),ShR(10,2)), Instr(Opcode.sxtb, r(3),r(0),ShR(10,2))),
                Select(16, 0xF, n => n != 0xF, Instr(Opcode.sxtah, r(3),r(4),r(0),ShR(10,2)), Instr(Opcode.sxth, r(3),r(0),ShR(10,2))),
                
                Select(16, 0xF, n => n != 0xF, Instr(Opcode.uxtab16, x("*")), Instr(Opcode.uxtb16, x("*"))),
                invalid,
                Select(16, 0xF, n => n != 0xF, Instr(Opcode.uxtab, r(3),r(4),r(0),ShR(10,2)), Instr(Opcode.uxtb, r(3),r(0),ShR(10,2))),
                Select(16, 0xF, n => n != 0xF, Instr(Opcode.uxtah, r(3),r(4),r(0),ShR(10,2)), Instr(Opcode.uxth, r(3),r(0),ShR(10,2))));
            var ReverseBitByte = Mask(22, 1,
                Mask(7, 1,
                    Instr(Opcode.rev, r(3),r(0)),
                    Instr(Opcode.rev16, r(3),r(0))),
                Mask(7, 1,
                    Instr(Opcode.rbit, r(3),r(0)),
                    Instr(Opcode.revsh, r(3),r(0))));


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
                    Mask(5, 7, 
                        nyi("media - 0b11010 - 000"),
                        nyi("media - 0b11010 - 001"),
                        BitfieldExtract,
                        nyi("media - 0b11010 - 011"),

                        nyi("media - 0b11010 - 100"),
                        nyi("media - 0b11010 - 101"),
                        BitfieldExtract,
                        nyi("media - 0b11010 - 111")),
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


            var StmdaStmed = Instr(Opcode.stmda, w(21), r(4),Mr(0,16));
            var LdmdaLdmfa = Instr(Opcode.ldmda, w(21), r(4),Mr(0,16));
            var Stm =        Instr(Opcode.stm, w(21), r(4),Mr(0,16));
            var Ldm =        Instr(Opcode.ldm, w(21), r(4),Mr(0,16));
            var StmStmia =   Instr(Opcode.stm, w(21), r(4),Mr(0,16));
            var LdmLdmia =   Instr(Opcode.ldm, w(21), r(4),Mr(0,16));
            var StmdbStmfd = Instr(Opcode.stmdb, w(21), r(4),Mr(0,16));
            var LdmdbLDmea = Instr(Opcode.ldmdb, w(21), r(4),Mr(0,16));
            var StmibStmfa = Instr(Opcode.stmib, w(21), r(4),Mr(0,16));
            var LdmibLdmed = Instr(Opcode.ldmib, w(21), r(4),Mr(0,16));
            var StmUser = nyi("StmUser");
            var LdmUser = nyi("LdmUser");
            var LoadStoreMultiple = Mask(22, 3, 20, 1, // P U op L
                    StmdaStmed,
                    LdmdaLdmfa,
                    Stm,
                    Ldm,
                    StmStmia,
                    LdmLdmia,
                    StmUser,
                    LdmUser,

                    StmdbStmfd,
                    LdmdbLDmea,
                    StmUser,
                    LdmUser,
                    
                    StmibStmfa,
                    LdmibLdmed,
                    StmUser,
                    LdmUser);

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
                    Instr(Opcode.b, J),
                    Instr(Opcode.bl, J)),
                Instr(Opcode.blx, X));

            var Branch_BranchLink_BlockDataTransfer = new MaskDecoder(25, 1,
                new PcDecoder(28,
                    LoadStoreMultiple,
                    ExceptionSaveRestore),
                BranchImmediate);

            var SystemRegister_64bitMove = new PcDecoder(28, 
                Mask(22, 1,
                    invalid,
                    Mask(20, 1,
                        Instr(Opcode.mcrr, CP(8),i(4,4),r(3),r(4),CR(0)),
                        Instr(Opcode.mrrc, x("")))),
                invalid);

            var SystemRegister_LdSt = Mask(20, 1,         // L (load)
                Mask(23, 2, 21, 1,
                    invalid,
                    nyi("SystemRegister_LdSt puw=001"),
                    Instr(Opcode.stc, CP(8),CR(12),M(4,w4)),
                    nyi("SystemRegister_LdSt puw=011"),

                    nyi("SystemRegister_LdSt puw=100"),
                    nyi("SystemRegister_LdSt puw=101"),
                    nyi("SystemRegister_LdSt puw=110"),
                    nyi("SystemRegister_LdSt puw=111")),
                Mask(23, 2, 21, 1,
                    invalid,
                    nyi("SystemRegister_LdSt puw=001"),
                    Instr(Opcode.ldc, CP(8),CR(12),M(4,w4)),
                    nyi("SystemRegister_LdSt puw=011"),

                    nyi("SystemRegister_LdSt puw=100"),
                    nyi("SystemRegister_LdSt puw=101"),
                    nyi("SystemRegister_LdSt puw=110"),
                    nyi("SystemRegister_LdSt puw=111")));

            var SystemRegister_LdSt_64bitMove = Select(21, 0b1101, n => n == 0,
                SystemRegister_64bitMove,
                SystemRegister_LdSt);

            var FloatingPointDataProcessing2regs = Mask(19, 1, 16, 3,
                Mask(7, 0b111,  // size:o3
                    invalid,
                    invalid,
                    invalid,
                    Instr(Opcode.vabs, F16, S(12,4,22,1),S(0,4,5,1)),

                    Instr(Opcode.vmov, x("(register) - single precision")),
                    Instr(Opcode.vabs, F32, S(12,4,22,1),S(0,4,5,1)),
                    Instr(Opcode.vmov, x("(register) - double precision")),
                    Instr(Opcode.vabs, F64, D(22,1,12,4),D(5,1,0,4))),
                Mask(7, 1,
                    Mask(8, 3,
                        invalid,
                        Instr(Opcode.vneg, F16, S(12,4,22,1),S(0,4,5,1)),
                        Instr(Opcode.vneg, F32, S(12,4,22,1),S(0,4,5,1)),
                        Instr(Opcode.vneg, F64, D(22,1,12,4),D(5,1,0,4))),
                    Mask(8, 3,
                        invalid,
                        Instr(Opcode.vsqrt, F16, S(12,4,22,1),S(0,4,5,1)),
                        Instr(Opcode.vsqrt, F32, S(12,4,22,1),S(0,4,5,1)),
                        Instr(Opcode.vsqrt, F64, D(22,1,12,4),D(5,1,0,4)))),
                nyi("Floating-point data-procesing (two registers) 0 010"),
                nyi("Floating-point data-procesing (two registers) 0 011"),

                Mask(7, 1,
                    Mask(8, 3,
                        invalid,
                        Instr(Opcode.vcmp, F16, S(12,4,22,1),S(0,4,5,1)),
                        Instr(Opcode.vcmp, F32, S(12,4,22,1),S(0,4,5,1)),
                        Instr(Opcode.vcmp, F64, D(22,1,12,4),D(5,1,0,4))),
                    Mask(8, 3, 
                        invalid,
                        Instr(Opcode.vcmpe, F16, S(12,4,22,1),S(0,4,5,1)),
                        Instr(Opcode.vcmpe, F32, S(12,4,22,1),S(0,4,5,1)),
                        Instr(Opcode.vcmpe, F64, D(22,1,12,4),D(5,1,0,4)))),
                nyi("Floating-point data-procesing (two registers) 0 101"),
                nyi("Floating-point data-procesing (two registers) 0 110"),
                nyi("Floating-point data-procesing (two registers) 0 111"),

                Mask(7, 1,
                    nyi("Floating-point data-procesing (two registers) 1 000 o3=0"),
                    Mask(8, 3,
                        invalid,
                        Instr(Opcode.vcvt, F16S16, S(12,4,22,1),S(0,4,5,1)),
                        Instr(Opcode.vcvt, F32S32, S(12,4,22,1),S(0,4,5,1)),
                        Instr(Opcode.vcvt, F64S32, D(22,1,12,4),S(0,4,5,1)))),
                nyi("Floating-point data-procesing (two registers) 1 001"),
                nyi("Floating-point data-procesing (two registers) 1 010"),
                nyi("Floating-point data-procesing (two registers) 1 011"),

                nyi("Floating-point data-procesing (two registers) 1 100"),
                nyi("Floating-point data-procesing (two registers) 1 101"),
                nyi("Floating-point data-procesing (two registers) 1 110"),
                nyi("Floating-point data-procesing (two registers) 1 111"));

            var FloatingPointDataProcessing3regs = Mask(23, 1, 20, 2, 6, 1,
                Mask(8, 0x3,
                    invalid,
                    Instr(Opcode.vmla, F16, S(12,4,22,1),S(16,4,7,1),S(0,4,5,1)),
                    Instr(Opcode.vmla, F32, S(12,4,22,1),S(16,4,7,1),S(0,4,5,1)),
                    Instr(Opcode.vmla, F64, D(22,1,12,4),D(7,1,16,4),D(5,1,0,4))),
                Mask(8, 0x3,
                    invalid,
                    Instr(Opcode.vmls, F16, S(12,4,22,1),S(16,4,7,1),S(0,4,5,1)),
                    Instr(Opcode.vmls, F32, S(12,4,22,1),S(16,4,7,1),S(0,4,5,1)),
                    Instr(Opcode.vmls, F64, D(22,1,12,4),D(7,1,16,4),D(5,1,0,4))),
                Mask(8, 0x3,
                    invalid,
                    Instr(Opcode.vnmls, F16, S(12,4,22,1),S(16,4,7,1),S(0,4,5,1)),
                    Instr(Opcode.vnmls, F32, S(12,4,22,1),S(16,4,7,1),S(0,4,5,1)),
                    Instr(Opcode.vnmls, F64, D(22,1,12,4),D(7,1,16,4),D(5,1,0,4))),
                Mask(8, 0x3,
                    invalid,
                    Instr(Opcode.vnmla, F16, S(12,4,22,1),S(16,4,7,1),S(0,4,5,1)),
                    Instr(Opcode.vnmla, F32, S(12,4,22,1),S(16,4,7,1),S(0,4,5,1)),
                    Instr(Opcode.vnmla, F64, D(22,1,12,4),D(7,1,16,4),D(5,1,0,4))),

                Mask(8, 0x3, 
                    invalid,
                    Instr(Opcode.vmul, F16, S(12,4,22,1),S(16,4,7,1),S(0,4,5,1)),
                    Instr(Opcode.vmul, F32, S(12,4,22,1),S(16,4,7,1),S(0,4,5,1)),
                    Instr(Opcode.vmul, F64, D(22,1,12,4),D(7,1,16,4),D(5,1,0,4))),
                Mask(8, 0x3,
                    invalid,
                    Instr(Opcode.vnmul, F16, S(12,4,22,1),S(16,4,7,1),S(0,4,5,1)),
                    Instr(Opcode.vnmul, F32, S(12,4,22,1),S(16,4,7,1),S(0,4,5,1)),
                    Instr(Opcode.vnmul, F64, D(22,1,12,4),D(7,1,16,4),D(5,1,0,4))),
                Mask(8, 0x3,
                    invalid,
                    Instr(Opcode.vadd, F16, S(12,4,22,1),S(16,4,7,1),S(0,4,5,1)),
                    Instr(Opcode.vadd, F32, S(12,4,22,1),S(16,4,7,1),S(0,4,5,1)),
                    Instr(Opcode.vadd, F64, D(22,1,12,4),D(7,1,16,4),D(5,1,0,4))),
                Mask(8, 0x3,
                    invalid,
                    Instr(Opcode.vsub, F16, S(12,4,22,1),S(16,4,7,1),S(0,4,5,1)),
                    Instr(Opcode.vsub, F32, S(12,4,22,1),S(16,4,7,1),S(0,4,5,1)),
                    Instr(Opcode.vsub, F64, D(22,1,12,4),D(7,1,16,4),D(5,1,0,4))),

                Mask(8, 0x3,
                    invalid,
                    Instr(Opcode.vdiv, F16, S(12,4,22,1),S(16,4,7,1),S(0,4,5,1)),
                    Instr(Opcode.vdiv, F32, S(12,4,22,1),S(16,4,7,1),S(0,4,5,1)),
                    Instr(Opcode.vdiv, F64, D(22,1,12,4),D(7,1,16,4),D(5,1,0,4))),
                invalid,
                Instr(Opcode.vfnms, x(" vfnms")),
                Instr(Opcode.vfnma, x(" vfnma")),

                Instr(Opcode.vfma, x("")),
                Instr(Opcode.vfms, x("")),
                invalid,
                invalid);

            var FloatingPointDataProcessing = new PcDecoder(28,
                Select(20, 0b1011, n => n != 0b1011,
                    FloatingPointDataProcessing3regs,
                    Mask(6, 1,
                        nyi("FloatingPointMoveImmediate"),
                        FloatingPointDataProcessing2regs)),
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
                    Instr(Opcode.vmov, x("GP to scalar")),
                    Mask(6, 1,
                        Instr(Opcode.vdup, vW(22,1,5,1),q(21), W(7,1,16,4),r(3)),
                        invalid)),
                Mask(21,2,5,2,
                    Instr(Opcode.vmov, I32, r(3),D(7,1,16,4), Ix(21,1)),
                    Instr(Opcode.vmov, r(3), x("*Scalar to GP op1:op2=0b00 01)")),
                    Instr(Opcode.vmov, r(3), x("*Scalar to GP op1:op2=0b00 10)")),
                    Instr(Opcode.vmov, r(3), x("*Scalar to GP op1:op2=0b00 11)")),

                    Instr(Opcode.vmov, r(3), x("*Scalar to GP op1:op2=0b01 00)")),
                    Instr(Opcode.vmov, r(3), x("*Scalar to GP op1:op2=0b01 01)")),
                    Instr(Opcode.vmov, r(3), x("*Scalar to GP op1:op2=0b01 10)")),
                    Instr(Opcode.vmov, r(3), x("*Scalar to GP op1:op2=0b01 11)")),

                    Instr(Opcode.vmov, r(3), x("*Scalar to GP op1:op2=0b10 00)")),
                    Instr(Opcode.vmov, r(3), x("*Scalar to GP op1:op2=0b10 01)")),
                    Instr(Opcode.vmov, r(3), x("*Scalar to GP op1:op2=0b10 10)")),
                    Instr(Opcode.vmov, r(3), x("*Scalar to GP op1:op2=0b10 11)")),

                    Instr(Opcode.vmov, r(3), x("*Scalar to GP op1:op2=0b11 00)")),
                    Instr(Opcode.vmov, r(3), x("*Scalar to GP op1:op2=0b11 01)")),
                    Instr(Opcode.vmov, r(3), x("*Scalar to GP op1:op2=0b11 10)")),
                    Instr(Opcode.vmov, r(3), x("*Scalar to GP op1:op2=0b11 11)))"))));

            var FloatingPointMoveSpecialReg = Mask(20, 1,
                Instr(Opcode.vmsr, i(16,4),r(3)),
                Instr(Opcode.vmrs, r(3),i(16,4)));

            var AdvancedSIMDandFloatingPoint32bitMove = Mask(8, 1,
                    Mask(21, 7,
                        Instr(Opcode.vmov, S(16,4,7,1),r(3)),
                        invalid,
                        invalid,
                        invalid,

                        invalid,
                        invalid,
                        invalid,
                        FloatingPointMoveSpecialReg),
                    AdvancedSIMDElementMovDuplicate);

            var AdvancedSimd_and_floatingpoint_LdSt = Mask(23, 2, 20, 2,
                invalid,
                invalid,
                invalid,
                invalid,

                nyi("AdvancedSimd_and_floatingpoint_LdSt - PUWL: 0b0100"),
                Mask(8, 3,
                    nyi("AdvancedSimd_and_floatingpoint_LdSt - PUWL: 0b0101 size: 0b00"),
                    nyi("AdvancedSimd_and_floatingpoint_LdSt - PUWL: 0b0101 size: 0b01"),
                    nyi("AdvancedSimd_and_floatingpoint_LdSt - PUWL: 0b0101 size: 0b10"),
                    Mask(0, 1,
                        Instr(Opcode.vldmia, w(21), r(4),Md(0,16)),
                        nyi("AdvancedSimd_and_floatingpoint_LdSt - PUWL: 0b0101 size: 0b11 xxxxxx1"))),
                Mask(8, 3,
                    nyi("AdvancedSimd_and_floatingpoint_LdSt - PUWL: 0b0110 size: 0b00"),
                    nyi("AdvancedSimd_and_floatingpoint_LdSt - PUWL: 0b0110 size: 0b01"),
                    nyi("AdvancedSimd_and_floatingpoint_LdSt - PUWL: 0b0110 size: 0b10"),
                    Mask(0, 1,
                        Instr(Opcode.vstmia, w(21), r(4),Md(0,16)),
                        nyi("AdvancedSimd_and_floatingpoint_LdSt - PUWL: 0b0110 size: 0b11 xxxxxx1"))),
                Mask(8, 3,
                    nyi("AdvancedSimd_and_floatingpoint_LdSt - PUWL: 0b0111 size: 0b00"),
                    nyi("AdvancedSimd_and_floatingpoint_LdSt - PUWL: 0b0111 size: 0b01"),
                    nyi("AdvancedSimd_and_floatingpoint_LdSt - PUWL: 0b0111 size: 0b10"),
                    Mask(0, 1,
                        Instr(Opcode.vldmia, w(21), r(4),Md(0,16)),
                        nyi("AdvancedSimd_and_floatingpoint_LdSt - PUWL: 0b0111 size: 0b11 xxxxxx1"))),

                Mask(8, 3, // size
                    invalid,
                    Instr(Opcode.vstr, x("h")),
                    Instr(Opcode.vstr, x("s")),
                    Instr(Opcode.vstr, x("d*"))),
                nyi("AdvancedSimd_and_floatingpoint_LdSt - PUWL: 0b1001"),
                Mask(8, 3,
                    nyi("AdvancedSimd_and_floatingpoint_LdSt - PUWL: 0b1010 size: 0b00"),
                    nyi("AdvancedSimd_and_floatingpoint_LdSt - PUWL: 0b1010 size: 0b01"),
                    nyi("AdvancedSimd_and_floatingpoint_LdSt - PUWL: 0b1010 size: 0b10"),
                    Mask(0, 1,
                        Instr(Opcode.vstmdb, w(21), r(4),Md(0,16)),
                        nyi("AdvancedSimd_and_floatingpoint_LdSt - PUWL: 0b1010 size: 0b11 xxxxxx1"))),
                nyi("AdvancedSimd_and_floatingpoint_LdSt - PUWL: 0b1011"),

                Mask(8, 3, // size
                    invalid,    
                    Instr(Opcode.vstr, I16, S(12,4,22,1),Mi(1,w2)),
                    Instr(Opcode.vstr, S(12,4,22,1),Mi(2,w4)),
                    Instr(Opcode.vstr, D(22,1,12,4),Mi(2,w8))),
                Mask(8, 3, // size
                    invalid,
                    Instr(Opcode.vldr, I16, S(12,4,22,1),Mi(1,w2)),
                    Instr(Opcode.vldr, S(12,4,22,1),Mi(2,w4)),
                    Instr(Opcode.vldr, D(22,1,12,4),Mi(2,w8))),
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
                                Instr(Opcode.vmov, D(5,1,0,4),r(3),r(4))),
                            Mask(8, 3, // size
                                invalid,
                                invalid,
                                nyi("vmov to 2 gp regs from 2 single floats"),
                                nyi("vmov to 2 gp regs from 1 double float"))))));

            var AdvancedSimd_LdSt_64bitmove = Select(21, 0b1101, n => n == 0,
                AdvancedSimd_and_floatingpoint64bitmove,
                AdvancedSimd_and_floatingpoint_LdSt);

            var SystemRegister32BitMove = new PcDecoder(28,
                Mask(20, 1,
                    Instr(Opcode.mcr, CP(8),i(21,3),r(3),CR(16),CR(0),i(5,3)),
                    Instr(Opcode.mrc, CP(8),i(21,3),r(3),CR(16),CR(0),i(5,3))),
                invalid);

            var AdvancedSimd_ThreeRegisters = Mask(24, 1,
                Mask(8, 0xF, // AdvancedSimd_ThreeRegisters - U = 0
                    Mask(20, 3, // AdvancedSimd_ThreeRegisters - U = 0, opc=0b0000
                        Mask(4, 1, // AdvancedSimd_ThreeRegisters - U = 0, opc=0b0000 size=00
                            invalid,
                            Instr(Opcode.vand, x(""))),
                        nyi("AdvancedSimd_ThreeRegisters - U = 0, opc=0b0000 size=01"),
                        nyi("AdvancedSimd_ThreeRegisters - U = 0, opc=0b0000 size=10"),
                        nyi("AdvancedSimd_ThreeRegisters - U = 0, opc=0b0000 size=11")),
                    Mask(20, 3, // AdvancedSimd_ThreeRegisters - U = 0, opc=0b0001
                        Mask(4, 1, // AdvancedSimd_ThreeRegisters - U = 0, opc=0b0001 size=00 o1
                            nyi("AdvancedSimd_ThreeRegisters - U = 0, opc=0b0001 size=00 o1=0"),
                            Instr(Opcode.vand, q(6), W(22,1,12,4),W(7,1,16,4),W(5,1,0,4))),
                        nyi("AdvancedSimd_ThreeRegisters - U = 0, opc=0b0001 size=01"),
                        Mask(4, 1, // AdvancedSimd_ThreeRegisters - U = 0, opc=0b0001 size=10 o1
                            nyi("AdvancedSimd_ThreeRegisters - U = 0, opc=0b0001 size=10 o1=0"),
                            Instr(Opcode.vorr, q(6), W(22,1,12,4),W(7,1,16,4),W(5,1,0,4))),
                        nyi("AdvancedSimd_ThreeRegisters - U = 1, opc=0b0001 size=11")),
                    nyi("AdvancedSimd_ThreeRegisters - U = 0, opc=0b0010"),
                    nyi("AdvancedSimd_ThreeRegisters - U = 0, opc=0b0011"),

                    Mask(4, 1, // AdvancedSimd_ThreeRegisters - U=0, opc=0b0100
                        Mask(20, 3, // AdvancedSimd_ThreeRegisters - U=0, opc=0b0100 o1=0
                            Instr(Opcode.vshl, S8, q(6), W(22,1,12,4),W(5,1,0,4),W(7,1,16,4)),
                            Instr(Opcode.vshl, S16, q(6), W(22,1,12,4),W(5,1,0,4),W(7,1,16,4)),
                            Instr(Opcode.vshl, S32, q(6), W(22,1,12,4),W(5,1,0,4),W(7,1,16,4)),
                            Instr(Opcode.vshl, S64, q(6), W(22,1,12,4),W(5,1,0,4),W(7,1,16,4))),
                        Mask(20, 3, // AdvancedSimd_ThreeRegisters - U=0, opc=0b0100 o1=1
                            Instr(Opcode.vqshl, S8, q(6), W(22,1,12,4),W(7,1,16,4),W(5,1,0,4)),
                            Instr(Opcode.vqshl, S16, q(6), W(22,1,12,4),W(7,1,16,4),W(5,1,0,4)),
                            Instr(Opcode.vqshl, S32, q(6), W(22,1,12,4),W(7,1,16,4),W(5,1,0,4)),
                            Instr(Opcode.vqshl, S64, q(6), W(22,1,12,4),W(7,1,16,4),W(5,1,0,4)))), 
                    nyi("AdvancedSimd_ThreeRegisters - U = 0, opc=0b0101"),
                    Mask(4, 1, // AdvancedSimd_ThreeRegisters - U = 0, opc=0b0110 u=0  
                        Mask(20, 3, 
                            Instr(Opcode.vmax, S8, q(6), W(22,1,12,4),W(7,1,16,4),W(5,1,0,4)),
                            Instr(Opcode.vmax, S16, q(6), W(22,1,12,4),W(7,1,16,4),W(5,1,0,4)),
                            Instr(Opcode.vmax, S32, q(6), W(22,1,12,4),W(7,1,16,4),W(5,1,0,4)),
                            invalid),
                        Mask(20, 3,
                            Instr(Opcode.vmin, S8, q(6), W(22,1,12,4),W(7,1,16,4),W(5,1,0,4)),
                            Instr(Opcode.vmin, S16, q(6), W(22,1,12,4),W(7,1,16,4),W(5,1,0,4)),
                            Instr(Opcode.vmin, S32, q(6), W(22,1,12,4),W(7,1,16,4),W(5,1,0,4)),
                            invalid)),
                    nyi("AdvancedSimd_ThreeRegisters - U = 0, opc=0b0111"),

                    nyi("AdvancedSimd_ThreeRegisters - U = 0, opc=0b1000"),
                    nyi("AdvancedSimd_ThreeRegisters - U = 0, opc=0b1001"),
                    Mask(6, 1, // AdvancedSimd_ThreeRegisters - U = 0, opc=0b1010
                        Mask(4, 1, // AdvancedSimd_ThreeRegisters - U = 0, opc=0b1010 Q=0 
                            Mask(20, 3, // AdvancedSimd_ThreeRegisters - U = 0, opc=0b1010 Q=0 o1=0
                                Instr(Opcode.vpmax, S8, D(22,1,12,4),D(7,1,16,4),D(5,1,0,4)),
                                Instr(Opcode.vpmax, S16, D(22,1,12,4),D(7,1,16,4),D(5,1,0,4)),
                                Instr(Opcode.vpmax, S32, D(22,1,12,4),D(7,1,16,4),D(5,1,0,4)),
                                invalid),
                            Mask(20, 3, // AdvancedSimd_ThreeRegisters - U = 0, opc=0b1010 Q=0 o1=1
                                Instr(Opcode.vpmin, S8, D(22,1,12,4),D(7,1,16,4),D(5,1,0,4)),
                                Instr(Opcode.vpmin, S16, D(22,1,12,4),D(7,1,16,4),D(5,1,0,4)),
                                Instr(Opcode.vpmin, S32, D(22,1,12,4),D(7,1,16,4),D(5,1,0,4)),
                                invalid)),
                        invalid),
                    Mask(4, 1,
                        nyi("AdvancedSimd_ThreeRegisters - U = 0, opc=0b1101 o1=0"),
                        Mask(20, 3,
                            Instr(Opcode.vpadd, I8, D(22,1,12,4),D(7,1,16,4),D(5,1,0,4)),
                            Instr(Opcode.vpadd, I16, D(22,1,12,4),D(7,1,16,4),D(5,1,0,4)),
                            Instr(Opcode.vpadd, I32, D(22,1,12,4),D(7,1,16,4),D(5,1,0,4)),
                            invalid)),

                    nyi("AdvancedSimd_ThreeRegisters - U = 0, opc=0b1100"),
                    nyi("AdvancedSimd_ThreeRegisters - U = 0, opc=0b1101"),
                    nyi("AdvancedSimd_ThreeRegisters - U = 0, opc=0b1110"),
                    nyi("AdvancedSimd_ThreeRegisters - U = 0, opc=0b1111")),

                Mask(8, 0xF, // AdvancedSimd_ThreeRegisters - U = 1
                    nyi("AdvancedSimd_ThreeRegisters - U = 1, opc=0b0000"),
                    Mask(20, 3, // AdvancedSimd_ThreeRegisters - U = 1, opc=0b0001
                        Mask(4, 1, // AdvancedSimd_ThreeRegisters - U = 1, opc=0b0001 size=00 o1
                            nyi("AdvancedSimd_ThreeRegisters - U = 1, opc=0b0001 size=00 o1=0"),
                            Instr(Opcode.veor, q(6), W(22,1,12,4),W(7,1,16,4),W(5,1,0,4))),
                        nyi("AdvancedSimd_ThreeRegisters - U = 1, opc=0b0001 size=01"),
                        nyi("AdvancedSimd_ThreeRegisters - U = 1, opc=0b0001 size=10"),
                        nyi("AdvancedSimd_ThreeRegisters - U = 1, opc=0b0001 size=11")),
                    nyi("AdvancedSimd_ThreeRegisters - U = 1, opc=0b0010"),
                    nyi("AdvancedSimd_ThreeRegisters - U = 1, opc=0b0011"),

                    Mask(4, 1, // AdvancedSimd_ThreeRegisters - U = 1, opc=0b0100
                        Mask(20, 3, // AdvancedSimd_ThreeRegisters - U=1, opc=0b0100 o1=0
                            Instr(Opcode.vshl, U8, q(6), W(22,1,12,4),W(5,1,0,4),W(7,1,16,4)),
                            Instr(Opcode.vshl, U16, q(6), W(22,1,12,4),W(5,1,0,4),W(7,1,16,4)),
                            Instr(Opcode.vshl, U32, q(6), W(22,1,12,4),W(5,1,0,4),W(7,1,16,4)),
                            Instr(Opcode.vshl, U64, q(6), W(22,1,12,4),W(5,1,0,4),W(7,1,16,4))),
                        Mask(20, 3, // AdvancedSimd_ThreeRegisters - U=1, opc=0b0100 o1=1
                            Instr(Opcode.vqshl, U8, q(6), W(22,1,12,4),W(7,1,16,4),W(5,1,0,4)),
                            Instr(Opcode.vqshl, U16, q(6), W(22,1,12,4),W(7,1,16,4),W(5,1,0,4)),
                            Instr(Opcode.vqshl, U32, q(6), W(22,1,12,4),W(7,1,16,4),W(5,1,0,4)),
                            Instr(Opcode.vqshl, U64, q(6), W(22,1,12,4),W(7,1,16,4),W(5,1,0,4)))),
                    nyi("AdvancedSimd_ThreeRegisters - U = 1, opc=0b0101"),
                    nyi("AdvancedSimd_ThreeRegisters - U = 1, opc=0b0110"),
                    nyi("AdvancedSimd_ThreeRegisters - U = 1, opc=0b0111"),

                    nyi("AdvancedSimd_ThreeRegisters - U = 1, opc=0b1000"),
                    nyi("AdvancedSimd_ThreeRegisters - U = 1, opc=0b1001"),
                    nyi("AdvancedSimd_ThreeRegisters - U = 1, opc=0b1010"),
                    nyi("AdvancedSimd_ThreeRegisters - U = 1, opc=0b1011"),

                    nyi("AdvancedSimd_ThreeRegisters - U = 1, opc=0b1100"),
                    nyi("AdvancedSimd_ThreeRegisters - U = 1, opc=0b1101"),
                    nyi("AdvancedSimd_ThreeRegisters - U = 1, opc=0b1110"),
                    nyi("AdvancedSimd_ThreeRegisters - U = 1, opc=0b1111")));

            var AdvancedSimd_TwoRegistersScalarExtension = nyi("AdvancedSimd_TwoRegistersScalarExtension");

            Decoder SystemRegister_AdvancedSimd_FloatingPoint = Mask(24, 3,
                Select(9, 7, n => n == 7,
                    SystemRegister_LdSt_64bitMove,
                    new PcDecoder(28, 
                        Select(10, 3, n => n == 2, AdvancedSimd_LdSt_64bitmove, SystemRegister_LdSt_64bitMove),
                        AdvancedSimd_ThreeRegisters)),
                Select(9, 7, n => n == 7,
                    SystemRegister_LdSt_64bitMove,
                    new PcDecoder(28,
                        Select(10, 3, n => n == 2, AdvancedSimd_LdSt_64bitmove, SystemRegister_LdSt_64bitMove),
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
                    Instr(Opcode.svc, i(0,24)));


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

            var AdvancedSimd_TwoRegisterOrThreeRegisters = Select(20, 0x3, n => n == 0b11,
                Mask(24, 1, 
                    Instr(Opcode.vext, U8, q(6), W(22,1,12,4),W(7,1,16,4),W(5,1,0,4),i(8,4)),
                    nyi("AdvancedSimd_TwoRegisterOrThreeRegisters op1==0b11 op0=1")),
                nyi("AdvancedSimd_TwoRegisterOrThreeRegisters op1!=0b11"));

            var AdvancedSimd_OneRegisterModifiedImmediate = Mask(8, 3, 5, 1,
                Instr(Opcode.vmov, I32, q(6), W(22,1,12,4),Is(24,1,16,3,0,4)),
                Instr(Opcode.vmvn, I32, q(6), W(22,1,12,4),Is(24,1,16,3,0,4)),
                Instr(Opcode.vorr, x("immediate - A1")),
                Instr(Opcode.vbic, x("immediate - A1")),

                Instr(Opcode.vmov, I32, q(6), W(22,1,12,4),Is(24,1,16,3,0,4)),
                Instr(Opcode.vmvn, I32, q(6), W(22,1,12,4),Is(24,1,16,3,0,4)),
                Instr(Opcode.vorr, x("immediate - A1")),
                Instr(Opcode.vbic, x("immediate - A1")),

                Instr(Opcode.vmov, I32, q(6), W(22,1,12,4),Is(24,1,16,3,0,4)),
                Instr(Opcode.vmvn, I32, q(6), W(22,1,12,4),Is(24,1,16,3,0,4)),
                Instr(Opcode.vorr, x("immediate - A1")),
                Instr(Opcode.vbic, x("immediate - A1")),

                Instr(Opcode.vmov, I32, q(6), W(22,1,12,4),Is(24,1,16,3,0,4)),
                Instr(Opcode.vmvn, I32, W(22,1,12,4),Is(24,1,16,3,0,4)),
                Instr(Opcode.vorr, x("immediate - A1")),
                Instr(Opcode.vbic, x("immediate - A1")),

                Instr(Opcode.vmov, x("immediate - A3")),
                Instr(Opcode.vmvn, x("immediate - A2")),
                Instr(Opcode.vorr, x("immediate - A2")),
                Instr(Opcode.vbic, x("immediate - A2")),

                Instr(Opcode.vmov, x("immediate - A3")),
                Instr(Opcode.vmvn, x("immediate - A2")),
                Instr(Opcode.vorr, x("immediate - A2")),
                Instr(Opcode.vbic, x("immediate - A2")),

                Instr(Opcode.vmov, x("immediate - A4")),
                Instr(Opcode.vmvn, x("immediate - A3")),
                Instr(Opcode.vmov, x("immediate - A4")),
                Instr(Opcode.vmvn, x("immediate - A3")),

                Instr(Opcode.vmov, x("immediate - A4")),
                Instr(Opcode.vmov, x("immediate - A5")),
                Instr(Opcode.vmov, x("immediate - A4")),
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

            var Barriers = Mask(4, 0xF,
                invalid,
                Instr(Opcode.clrex, x("")),
                invalid,
                invalid,

                Instr(Opcode.dsb, Ba(0,4)),
                Instr(Opcode.dmb, Ba(0,4)),
                Instr(Opcode.isb, Ba(0,4)),
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid);

            var MemoryHintsAndBarriers = Mask(25, 1,
                Mask(21, 1,
                    nyi("Preload (immediate)"),
                    Mask(22, 7,
                        invalid,
                        invalid,
                        invalid,
                        invalid,

                        invalid,
                        Barriers,
                        invalid,
                        invalid)),
                Mask(4, 1,
                    Mask(21, 1,
                        nyi("Preload (register)"),
                        invalid),
                    invalid));

            var unconditionalDecoder = new MaskDecoder(25, 7,
                UncMiscellaneous,
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
