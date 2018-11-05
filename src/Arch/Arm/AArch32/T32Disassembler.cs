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

namespace Reko.Arch.Arm.AArch32
{

    using Mutator = Func<uint, T32Disassembler, bool>;

    /// <summary>
    /// Disassembles machine code in the ARM T32 encoding into 
    /// ARM32 instructions.
    /// </summary>
    public partial class T32Disassembler : DisassemblerBase<AArch32Instruction>
    {
        private static readonly Decoder[] decoders;
        private static readonly Decoder invalid;

        private readonly ImageReader rdr;
        private readonly ThumbArchitecture arch;
        private Address addr;
        private int itState;
        private ArmCondition itCondition;
        private DasmState state;

        public T32Disassembler(ThumbArchitecture arch, ImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.itState = 0;
            this.itCondition = ArmCondition.AL;
        }

        public override AArch32Instruction DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadLeUInt16(out var wInstr))
                return null;
            this.state = new DasmState();

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

        private class DasmState
        {
            public Opcode opcode;
            public List<MachineOperand> ops = new List<MachineOperand>();
            public ArmCondition cc = ArmCondition.AL;
            public bool updateFlags = false;
            public bool wide = false;
            public bool writeback = false;
            public Opcode shiftType = Opcode.Invalid;
            public MachineOperand shiftValue = null;
            public ArmVectorData vectorData = ArmVectorData.INVALID;
            public uint n;

            internal void Invalid()
            {
                throw new NotImplementedException();
            }

            public AArch32Instruction MakeInstruction()
            {
                return new AArch32Instruction
                {
                    opcode = opcode,
                    ops = ops.ToArray(),
                    condition = cc,
                    SetFlags = updateFlags,
                    Wide = wide,
                    Writeback = writeback,
                    ShiftType = shiftType,
                    ShiftValue = shiftValue,
                    vector_data = vectorData,
                };
            }
        }

        private AArch32Instruction DecodeFormat(uint wInstr, Opcode opcode, string format)
        {
            this.state.opcode = opcode;
            for (int i = 0; i < format.Length; ++i)
            {
                int offset;
                int size;
                RegisterStorage baseReg;
                MachineOperand op = null;
                switch (format[i])
                {
                case ',':
                case ' ':
                    continue;
                    // The following case are modifiers, they don't generate operands.
                    // The cases should end with a 'continue' rather than a 'break'.
                case '.':
                    // This instruction always sets the flags.
                    state.updateFlags = true;
                    continue;
                case 'q':
                    // This is the wide form of an ARM Thumb instruction.
                    state.wide = true;
                    continue;
                case ':':
                    // This instructions sets the flags if it's outside an IT block.
                    state.updateFlags = this.itCondition == ArmCondition.AL;
                    continue;
                case 'v': // vector element size
                    ++i;
                    switch (format[i])
                    {
                    case 'i': // Force  integer
                        ++i;
                        if (Char.IsDigit(format[i]))
                        {
                            uint n = ReadBitfields(wInstr, format, ref i);
                            state.vectorData = VectorIntUIntData(0, n);
                        }
                        else
                        {
                            state.vectorData = VectorIntUIntData(format, ref i);
                        }
                        if (state.vectorData == ArmVectorData.INVALID)
                            return Invalid();
                        continue;
                    case 'u':   // signed or unsigned integer
                        ++i;
                        uint nn = ReadBitfields(wInstr, format, ref i);
                        state.vectorData = VectorIntUIntData(wInstr, nn);
                        continue;
                    case 'r':
                        {
                            uint n = ReadBitfields(wInstr, format, ref i);
                            throw new NotImplementedException();
                        }
                    case 'c':       // conversion 
                        {
                            state.vectorData = VectorConvertData(wInstr);
                            continue;
                        }
                    case 'C':       // conversion2 
                        {
                            state.vectorData = VectorConvertData2(wInstr);
                            continue;
                        }
                    case 'f':       // floating point vector
                        {
                            ++i;
                            state.vectorData = VectorFloatData(format, ref i);
                            if (state.vectorData == ArmVectorData.INVALID)
                                return Invalid();
                        }
                        continue;
                    case 'F':       // floating point elements specified by a bitfield
                        ++i;
                        {
                            uint n = ReadBitfields(wInstr, format, ref i);
                            state.vectorData = VectorFloatElementData(n);
                            if (state.vectorData == ArmVectorData.INVALID)
                                return Invalid();
                        }
                        continue;
                    }
                    throw new InvalidOperationException();
                case 'w':   // Writeback bit.
                    ++i;
                    offset = ReadDecimal(format, ref i);
                    state.writeback = SBitfield(wInstr, offset, 1) != 0;
                    continue;

                    // The following cases generate operands of different types.
                    // They should generate a value in 'op'.
                case 's':
                    ++i;
                    if (PeekAndDiscard('p', format, ref i))
                    {
                        if (PeekAndDiscard('s', format, ref i))
                        {
                            Expect('r', format, ref i);
                            op = new RegisterOperand(Registers.spsr);
                        }
                        else
                        {
                            // 'sp': explict stack register reference.
                            op = new RegisterOperand(arch.StackRegister);
                        }
                    }
                    else // Signed immediate (in bitfields)
                    {
                        uint n = ReadBitfields(wInstr, format, ref i);
                        op = ImmediateOperand.Int32((int)n);
                    }
                    break;
                case 'S':   // shift amount in bitfield.
                    ++i;
                    if (PeekAndDiscard('r', format, ref i))
                    {
                        // 'Sr' = rotate
                        uint n = ReadBitfields(wInstr, format, ref i);
                        state.shiftType = Opcode.ror;
                        state.shiftValue = ImmediateOperand.Int32((int)n);
                        continue;
                    }
                    else if (PeekAndDiscard('i', format, ref i))
                    {
                        // 'Si' = shift immediate
                        (state.shiftType, state.shiftValue) = DecodeImmShift(wInstr, format, ref i);
                        continue;
                    }
                    else
                    {
                        offset = ReadDecimal(format, ref i);
                        Expect(':', format, ref i);
                        size = ReadDecimal(format, ref i);
                        op = ImmediateOperand.Int32(SBitfield(wInstr, offset, size));
                    }
                    break;
                case 'i':   // immediate value in bitfield(s)
                    ++i;
                    {
                        uint n = ReadBitfields(wInstr, format, ref i);
                        if (PeekAndDiscard('h', format, ref i))
                        {
                            op = ImmediateOperand.Word16((ushort)n);
                        }
                        else if (PeekAndDiscard('-', format, ref i))
                        {
                            var minuend = ReadDecimal(format, ref i);
                            op = ImmediateOperand.Word32(minuend - (int)n);
                        }
                        else
                        {
                            op = ImmediateOperand.Word32(n);
                        }
                    }
                    break;
                case 'M':
                    ++i;
                    if (PeekAndDiscard('S', format, ref i))
                    {
                        uint n = ReadBitfields(wInstr, format, ref i);
                        op = ModifiedSimdImmediate(wInstr, n);
                    }
                    else
                    {
                        --i;
                        op = ModifiedImmediate(wInstr);
                    }
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
                case 'R':   // 4-bit register.
                    ++i;
                    offset = ReadDecimal(format, ref i);
                    op = new RegisterOperand(Registers.GpRegs[
                        ((int)wInstr >> offset) & 0x0F]);
                    break;
                case 'T':   // GP register, specified by bits 7 || 2..0
                    var tReg = ((wInstr & 0x80) >> 4) | (wInstr & 7);
                    op = new RegisterOperand(Registers.GpRegs[tReg]);
                    break;
                case 'F':   // Sn register
                    ++i;
                    {
                        uint n = ReadBitfields(wInstr, format, ref i);
                        op = new RegisterOperand(Registers.SRegs[n]);
                    }
                    break;
                case 'D':   // Dn register
                    ++i;
                    {
                        uint n = ReadBitfields(wInstr, format, ref i);
                        op = new RegisterOperand(Registers.DRegs[n]);
                    }
                    break;
                case 'Q':   // Qn register
                    ++i;
                    {
                        uint n = ReadBitfields(wInstr, format, ref i);
                        op = new RegisterOperand(Registers.QRegs[n >> 1]);
                    }
                    break;
                case '[':   // Memory access
                    ++i;
                    op = ReadMemoryAccess(wInstr, format, ref i);
                    break;
                case 'P': // PC-relative offset, aligned by 4 bytes
                    ++i;
                    offset = ReadDecimal(format, ref i);
                    Expect(':', format, ref i);
                    size = ReadDecimal(format, ref i);
                    op = AddressOperand.Create(addr.Align(4) + (SBitfield(wInstr, offset, size) << 2));
                    break;
                case 'p':   // PC-relative offset.
                    ++i;
                    offset = (int)ReadBitfields(wInstr, format, ref i);
                    op = AddressOperand.Create(addr + offset);
                    break;
                case 'c':  // Condition code
                    ++i;
                    if (PeekAndDiscard('p', format, ref i))
                    {
                        Expect('s', format, ref i);
                        Expect('r', format, ref i);
                        op = new RegisterOperand(Registers.cpsr);
                        break;
                    }
                    else
                    {
                        offset = ReadDecimal(format, ref i);
                        state.cc = (ArmCondition)SBitfield(wInstr, offset, 4);
                        --i;
                    }
                    continue;
                case 'C':   // Coprocessor
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
                case 'B':   // barrier operation
                    ++i;
                    {
                        uint n = ReadBitfields(wInstr, format, ref i);
                        op = MakeBarrierOperand(n);
                        if (op == null)
                            return Invalid();
                    }
                    break;
                default:
                    return NotYetImplemented($"Unknown format specifier {format[i]} in {format} when decoding {opcode}", wInstr);
                }
                state.ops.Add(op);
            }

            return new AArch32Instruction
            {
                opcode = state.opcode,
                condition = state.cc,
                SetFlags = state.updateFlags,
                ops = state.ops.ToArray(),
                Writeback = state.writeback,
                Wide = state.wide,
                ShiftType = state.shiftType,
                ShiftValue = state.shiftValue,
                vector_data = state.vectorData,
            };
        }

        private MemoryOperand ReadMemoryAccess(uint wInstr, string format, ref int i)
        {
            int offset, size;
            RegisterStorage baseReg;
            bool add = true;
            RegisterStorage index = null;
            Opcode shiftType = Opcode.Invalid;
            int shiftAmt = 0;

            if (PeekAndDiscard('s', format, ref i))
            {
                // [s = stack register access
                baseReg = arch.StackRegister;
            }
            else if (PeekAndDiscard('r', format, ref i))
            {
                // [r = low 8 register
                // Only 3 bits for register
                var reg = ReadDecimal(format, ref i);
                baseReg = Registers.GpRegs[SBitfield(wInstr, reg, 3)];
            }
            else if (PeekAndDiscard('R', format, ref i))
            {
                // [R = GP register
                var reg = ReadDecimal(format, ref i);
                baseReg = Registers.GpRegs[SBitfield(wInstr, reg, 4)];
            }
            else if (PeekAndDiscard('P', format, ref i))
            {
                // [P = PC-relative
                baseReg = Registers.pc;
            }
            else
            {
                throw new NotImplementedException();
            }
            offset = 0;
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
                    Expect(',', format, ref i);
                }
                else if (PeekAndDiscard('r', format, ref i))
                {
                    // Only 3 bits for register
                    var reg = ReadDecimal(format, ref i);
                    index = Registers.GpRegs[SBitfield(wInstr, reg, 3)];
                    Expect(',', format, ref i);
                }
                else if (PeekAndDiscard('R', format, ref i))
                {
                    // 4 bits for register
                    var reg = ReadDecimal(format, ref i);
                    index = Registers.GpRegs[SBitfield(wInstr, reg, 4)];
                    if (PeekAndDiscard('<', format, ref i))
                    {
                        var shOffset = ReadDecimal(format, ref i);
                        Expect(':', format, ref i);
                        var shSize = ReadDecimal(format, ref i);
                        shiftAmt = SBitfield(wInstr, shOffset, shSize);
                        shiftType = shiftAmt != 0 ? Opcode.lsl : Opcode.Invalid;
                    }
                    add = true;
                    Expect(',', format, ref i);
                }
                else if (PeekAndDiscard('i', format, ref i))
                {
                    // Unshifted offset.
                    offset = (int) ReadBitfields(wInstr, format, ref i);
                    add = true;
                    Expect(',', format, ref i);
                }
            }
            var dt = DataType(format, ref i);
            var preindex = false;
            if (PeekAndDiscard('x', format, ref i))
            {
                // Indexing bits in P=10, W=8
                // Negative bit in U=9
                preindex = SBitfield(wInstr, 10, 1) != 0;
                add = (SBitfield(wInstr, 9, 1) != 0);
                state.writeback = SBitfield(wInstr, 8, 1) != 0;
            }
            else if (PeekAndDiscard('X', format, ref i))
            {
                preindex = SBitfield(wInstr, 24, 1) != 0;
                add = SBitfield(wInstr, 23, 1) != 0;
                state.writeback = SBitfield(wInstr, 21, 1) != 0;
            }

            Expect(']', format, ref i);
            var op = new MemoryOperand(dt)
            {
                BaseRegister = baseReg,
                Offset = Constant.Int32(offset),
                Index = index,
                PreIndex = preindex,
                ShiftType = shiftType,
                Shift = shiftAmt,
                Add = add,
            };
            return op;
        }

        private (Opcode, MachineOperand) DecodeImmShift(uint wInstr, string format, ref int i)
        {
            var type = ReadBitfields(wInstr, format, ref i);
            Expect(';', format, ref i);
            var imm = ReadBitfields(wInstr, format, ref i);
            switch (type)
            {
            case 0: return (Opcode.lsl, ImmediateOperand.UInt32(imm));
            case 1: return (Opcode.lsr, ImmediateOperand.UInt32(imm == 0 ? 32 : imm));
            case 2: return (Opcode.asr, ImmediateOperand.UInt32(imm == 0 ? 32 : imm));
            case 3:
                if (imm == 0)
                    return (Opcode.rrx, ImmediateOperand.UInt32(1));
                else
                    return (Opcode.ror, ImmediateOperand.UInt32(imm));
            }
            throw new InvalidOperationException("Type must be [0..3].");
        }

        private ArmVectorData VectorIntUIntData(string format, ref int i)
        {
            switch (format[i++])
            {
            case 'w': return ArmVectorData.I32;
            case 'h': return ArmVectorData.I16;
            case 'H': return ArmVectorData.S16;
            case 'b': return ArmVectorData.I8;
            case 'B': return ArmVectorData.S8;
            default: throw new InvalidOperationException("");
            }
        }

        private MachineOperand ModifiedSimdImmediate(uint wInstr, uint imm8)
        {
            ulong Replicate2(uint value)
            {
                return (((ulong)value) << 32) | value;
            }

            ulong Replicate4(uint value)
            {
                var v = (ulong)(ushort)value;
                return (v << 48) | (v << 32) | (v << 16) | v;
            }

            int op = SBitfield(wInstr, 5, 1);
            int cmode = SBitfield(wInstr, 8, 4);
            ulong imm64 = 0;
            switch (cmode >> 1)
            {
            case 0:
                imm64 = Replicate2(imm8); break;
            case 1:
                imm64 = Replicate2(imm8 << 8); break;
            case 2:
                imm64 = Replicate2(imm8 << 16); break;
            case 3:
                imm64 = Replicate2(imm8 << 24); break;
            case 4:
                imm64 = Replicate4(imm8); break;
            case 5:
                imm64 = Replicate4(imm8 << 8); break;
            case 6:
                if ((cmode & 1) == 0) {
                    imm64 = Replicate2((imm8 << 8) | 0xFF);
                } else {
                    imm64 = Replicate2((imm8 << 16) | 0xFFFF);
                }
                break;
            case 7:
                throw new NotImplementedException();
                /*
                if (cmode < 0 > == '0' && op == '0') {
                    imm64 = Replicate(imm8, 8);
                }
                if (cmode < 0 > == '0' && op == '1') {
                    imm8a = Replicate(imm8 < 7 >, 8); imm8b = Replicate(imm8 < 6 >, 8);
                    imm8c = Replicate(imm8 < 5 >, 8); imm8d = Replicate(imm8 < 4 >, 8);
                    imm8e = Replicate(imm8 < 3 >, 8); imm8f = Replicate(imm8 < 2 >, 8);
                    imm8g = Replicate(imm8 < 1 >, 8); imm8h = Replicate(imm8 < 0 >, 8);
                    imm64 = imm8a:imm8b: imm8c: imm8d: imm8e: imm8f: imm8g: imm8h;
                }
                if (cmode < 0 > == '1' && op == '0') {
                    imm32 = imm8 < 7 >:NOT(imm8 < 6 >):Replicate(imm8 < 6 >, 5):imm8 < 5:0 >:Zeros(19);
                    imm64 = Replicate(imm32, 2);
                }
                if (cmode < 0 > == '1' && op == '1') {
                    if UsingAArch32() then ReservedEncoding();
                    imm64 = imm8 < 7 >:NOT(imm8 < 6 >):Replicate(imm8 < 6 >, 8):imm8 < 5:0 >:Zeros(48);
                }
                break;
                */
            }
            return ImmediateOperand.Word64(imm64);
        }

        private MachineOperand MakeBarrierOperand(uint n)
        {
            var bo = (BarrierOption)n;
            switch (bo)
            {
            case BarrierOption.OSHLD:
            case BarrierOption.OSHST:
            case BarrierOption.OSH:
            case BarrierOption.NSHLD:
            case BarrierOption.NSHST:
            case BarrierOption.NSH:
            case BarrierOption.ISHLD:
            case BarrierOption.ISHST:
            case BarrierOption.ISH:
            case BarrierOption.LD:
            case BarrierOption.ST:
            case BarrierOption.SY:
                return new BarrierOperand(bo);

            }
            return null;
        }

        private AArch32Instruction Invalid()
        {
            return new AArch32Instruction
            {
                opcode = Opcode.Invalid,
                ops = new MachineOperand[0]
            };
        }

        private AArch32Instruction NotYetImplemented(string message, uint wInstr)
        {
            Console.WriteLine($"// A T32 decoder for the instruction {wInstr:X} ({Bits.Reverse(wInstr):X8}) - ({message}) has not been implemented yet.");
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


        private ArmVectorData VectorIntUIntData(uint wInstr, uint n)
        {
            if (SBitfield(wInstr, 28, 1) == 0)
            {
                switch (n)
                {
                case 0: return ArmVectorData.I8;
                case 1: return ArmVectorData.I16;
                case 2: return ArmVectorData.I32;
                default: return ArmVectorData.INVALID;
                }
            }
            else
            {
                switch (n)
                {
                case 0: return ArmVectorData.U8;
                case 1: return ArmVectorData.U16;
                case 2: return ArmVectorData.U32;
                default: return ArmVectorData.INVALID;
                }
            }
        }

        private ArmVectorData VectorFloatData(string format, ref int i)
        {
            switch (format[i++])
            {
            case 'h': return ArmVectorData.F16;
            case 's': return ArmVectorData.F32;
            case 'd': return ArmVectorData.F64;
            default: return ArmVectorData.INVALID;
            }
        }

        private ArmVectorData VectorFloatElementData(uint n)
        {
            switch (n)
            {
            case 1: return ArmVectorData.F16;
            case 2: return ArmVectorData.F32;
            default: return ArmVectorData.INVALID;
            }
        }


        private ArmVectorData VectorConvertData(uint wInstr)
        {
            var op = SBitfield(wInstr, 7, 2);
            switch (SBitfield(wInstr, 18, 2))
            {
            case 1:
                switch (op)
                {
                case 0: return ArmVectorData.F16S16;
                case 1: return ArmVectorData.F16U16;
                case 2: return ArmVectorData.S16F16;
                case 3: return ArmVectorData.U16F16;
                }
                break;
            case 2:
                switch (op)
                {
                case 0: return ArmVectorData.F32S32;
                case 1: return ArmVectorData.F32U32;
                case 2: return ArmVectorData.S32F32;
                case 3: return ArmVectorData.U32F32;
                }
                break;
            }
            return ArmVectorData.INVALID;
        }

        private ArmVectorData VectorConvertData2(uint wInstr)
        {
            var op = SBitfield(wInstr, 8, 2);
            var u = SBitfield(wInstr, 28, 1);
            switch (op)
            {
            case 0:
                return u == 0 ? ArmVectorData.F16S16 : ArmVectorData.F16U16;
            case 1:
                return u == 0 ? ArmVectorData.S16F16 : ArmVectorData.U16F16;
            case 2:
                return u == 0 ? ArmVectorData.F32S32 : ArmVectorData.F32U32;
            case 3:
                return u == 0 ? ArmVectorData.S32F32 : ArmVectorData.U32F32;
            }
            return ArmVectorData.INVALID;
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
            int bits = 0;
            bool signExtend = PeekAndDiscard('+', format, ref i);
            do
            {
                var offset = ReadDecimal(format, ref i);
                Expect(':', format, ref i);
                var size = ReadDecimal(format, ref i);
                n = (n << size) | ((wInstr >> offset) & ((1u << size) - 1));
                bits += size;
            } while (PeekAndDiscard(':', format, ref i));
            if (PeekAndDiscard('<', format, ref i))
            {
                var shift = ReadDecimal(format, ref i);
                n <<= shift;
                bits += shift;
            }
            if (signExtend)
            {
                n = (uint)Bits.SignExtend(n, bits);
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

        private static bool PeekAndDiscard(char c, string format, ref int i)
        {
            if (i >= format.Length)
                return false;
            if (format[i] != c)
                return false;
            ++i;
            return true;
        }

        private  static void Expect(char c, string format, ref int i)
        {
            Debug.Assert(format[i] == c);
            ++i;
        }

        private static int ReadDecimal(string format, ref int i)
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
            case 'r':
                var n = ReadDecimal(format, ref i);
                return PrimitiveType.Create(Domain.Real, n);
            default: throw new InvalidOperationException($"{format[i-1]}");
            }
        }

        private static Decoder DecodeBfcBfi(Opcode opcode, string format)
        {
            return new BfcBfiDecoder(opcode, format);
        }

        #region Mutators

        private static bool R0(uint wInstr, T32Disassembler dasm)
        {
            dasm.state.ops.Add(new RegisterOperand(Registers.GpRegs[
                (wInstr & 0xF)]));
            return true;
        }

        private static bool R8(uint wInstr, T32Disassembler dasm)
        {
            dasm.state.ops.Add(new RegisterOperand(Registers.GpRegs[
                ((wInstr >> 8) & 0xF)]));
            return true;
        }

        private static bool R12(uint wInstr, T32Disassembler dasm)
        {
            dasm.state.ops.Add(new RegisterOperand(Registers.GpRegs[
                ((wInstr >> 12) & 0xF)]));
            return true;
        }

        private static bool R16(uint wInstr, T32Disassembler dasm)
        {
            dasm.state.ops.Add(new RegisterOperand(Registers.GpRegs[
                ((wInstr >> 16) & 0xF)]));
            return true;
        }

        private static bool D22_12(uint wInstr, T32Disassembler dasm)
        {
            dasm.state.ops.Add(new RegisterOperand(Registers.DRegs[
                ((wInstr >> 18) & 0x10) | ((wInstr >> 12) & 0xF)]));
            return true;
        }

        private static bool Q22_12(uint wInstr, T32Disassembler dasm)
        {
            dasm.state.ops.Add(new RegisterOperand(Registers.QRegs[
                ((wInstr >> 18) & 0x10) | ((wInstr >> 12) & 0xF)]));
            return true;
        }

        private static bool Q22_12_times2(uint wInstr, T32Disassembler dasm)
        {
            var q = ((wInstr >> 18) & 0x10) | ((wInstr >> 12) & 0xF);
            dasm.state.ops.Add(new RegisterOperand(Registers.QRegs[q >> 1]));
            return true;
        }

        private static bool D5_0(uint wInstr, T32Disassembler dasm)
        {
            dasm.state.ops.Add(new RegisterOperand(Registers.DRegs[
                ((wInstr >> 1) & 0x10) | (wInstr & 0xF)]));
            return true;
        }

        private static bool Q5_0(uint wInstr, T32Disassembler dasm)
        {
            var q = ((wInstr >> 1) & 0x10) | (wInstr & 0xF);
            dasm.state.ops.Add(new RegisterOperand(Registers.QRegs[q]));
            return true;
        }

        private static bool Q5_0_times2(uint wInstr, T32Disassembler dasm)
        {
            var q = ((wInstr >> 1) & 0x10) | (wInstr & 0xF);
            dasm.state.ops.Add(new RegisterOperand(Registers.QRegs[q >> 1]));
            return true;
        }

        // Immediate mutators

        private static Mutator Imm(int pos1, int length1, int pos2, int length2)
        {
            var bitfields = new []
            {
                new Bitfield(pos1, length1),
                new Bitfield(pos2, length2),
            };
            return (u,d) =>
            {
                var imm = Bitfield.ReadFields(bitfields, u);
                d.state.ops.Add(ImmediateOperand.Word32(imm));
                return true;
            };
        }

        private static Mutator ImmM1(int pos, int length)
        {
            var bitfield = new Bitfield(pos, length);
            return (u, d) =>
            {
                var n = bitfield.Read(u);
                d.state.ops.Add(ImmediateOperand.Word32(n + 1));
                return true;
            };
        }

        private static Mutator LslImm(int pos1, int length1, int pos2, int length2)
        {
            var bitfields = new[]
            {
                new Bitfield(pos1, length1),
                new Bitfield(pos2, length2),
            };
            return (u, d) =>
            {
                var imm = Bitfield.ReadFields(bitfields, u);
                if (imm != 0)
                {
                    d.state.shiftType = Opcode.lsl;
                    d.state.shiftValue = ImmediateOperand.Int32((int)imm);
                }
                return true;
            };
        }

        private static (ArmVectorData, uint)[] vectorImmediateShiftSize = new[]
        {
           (ArmVectorData.INVALID, 0u),
           (ArmVectorData.I8,  8u),

           (ArmVectorData.I16, 16u),
           (ArmVectorData.I16, 16u),

           (ArmVectorData.I32, 32u),
           (ArmVectorData.I32, 32u),
           (ArmVectorData.I32, 32u),
           (ArmVectorData.I32, 32u),

           (ArmVectorData.I64, 0u),
           (ArmVectorData.I64, 0u),
           (ArmVectorData.I64, 0u),
           (ArmVectorData.I64, 0u),
           (ArmVectorData.I64, 0u),
           (ArmVectorData.I64, 0u),
           (ArmVectorData.I64, 0u),
           (ArmVectorData.I64, 0u),
        };

        private static bool VshImmSize(uint wInstr, T32Disassembler dasm)
        {
            var immL_6 = ((wInstr >> 1) & 0x40) | (wInstr >> 16) & 0b111111;
            dasm.state.vectorData = vectorImmediateShiftSize[immL_6 >> 3].Item1;
            return true;
        }

        private static bool VshImm(uint wInstr, T32Disassembler dasm)
        {
            var immL_6 = ((wInstr >> 1) & 0x40) | (wInstr >> 16) & 0b111111;
            var imm = immL_6 - vectorImmediateShiftSize[immL_6 >> 3].Item2;
            dasm.state.ops.Add(ImmediateOperand.Int32((int)imm));
            return true;
        }

        // Memory access mutators

        private static Mutator MemIdx(int posBaseReg, int posIdxReg, PrimitiveType dt)
        {
            return (u, d) =>
            {
                var baseReg = Registers.GpRegs[(u >> posBaseReg) & 0xF];
                var idxReg = Registers.GpRegs[(u >> posIdxReg) & 0xF];
                var mem = new MemoryOperand(dt)
                {
                    BaseRegister = baseReg,
                    Index = idxReg
                };
                d.state.ops.Add(mem);
                return true;
            };
        }
        #endregion


        // Factory methods
        private static InstrDecoder Instr(Opcode opcode, string format)
        {
            return new InstrDecoder(opcode, format);
        }

        private static InstrDecoder2 Instr(Opcode opcode, params Mutator[] mutators)
        {
            return new InstrDecoder2(opcode, mutators);
        }

        private static MaskDecoder Mask(int shift, uint mask, params Decoder [] decoders)
        {
            return new MaskDecoder(shift, mask, decoders);
        }

        private static MaskDecoder Mask(int shift, uint mask, string debugString, params Decoder[] decoders)
        {
            return new MaskDecoder(shift, mask, debugString, decoders);
        }

        private static BitFieldsDecoder Bitfields(string fieldSpecifier, params Decoder[] decoders)
        {
            return new BitFieldsDecoder(fieldSpecifier, decoders);
        }

        private static SelectDecoder Select(Func<uint, bool> predicate, Decoder decoderTrue, Decoder decoderFalse)
        {
            return new SelectDecoder(predicate, decoderTrue, decoderFalse);
        }

        private static SelectFieldDecoder Select((int,int) fieldSpecifier, Func<uint, bool> predicate, Decoder decoderTrue, Decoder decoderFalse)
        {
            var fields = new[]
            {
                new Bitfield(fieldSpecifier.Item1, fieldSpecifier.Item2)
            };
            return new SelectFieldDecoder(fields, predicate, decoderTrue, decoderFalse);
        }

        private static SelectFieldDecoder Select((int,int,int,int) fieldSpecifier, Func<uint, bool> predicate, Decoder decoderTrue, Decoder decoderFalse)
        {
            var fields = new[]
            {
                new Bitfield(fieldSpecifier.Item1, fieldSpecifier.Item2),
                new Bitfield(fieldSpecifier.Item3, fieldSpecifier.Item4),
            };
            return new SelectFieldDecoder(fields, predicate, decoderTrue, decoderFalse);
        }

        private static NyiDecoder Nyi(string msg)
        {
            return new NyiDecoder(msg);
        }


        static T32Disassembler()
        {
            invalid = Instr(Opcode.Invalid, "");

            // Build the decoder decision tree.
            var dec16bit = Create16bitDecoders();
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
                    Instr(Opcode.b, "p+0:11<1"),
                    dec32bit,
                    dec32bit,
                    dec32bit)
            };
        }

        private static MaskDecoder Create16bitDecoders()
        {
            var AddSpRegisterT1 = Instr(Opcode.add, ".T,sp");
            var AddSpRegisterT2 = Instr(Opcode.add, "sp,T");
            var decAlu = CreateAluDecoder();
            var decDataLowRegisters = CreateDataLowRegisters();
            var decDataHiRegisters = Mask(8, 0x03, // Add, subtract, compare, move (two high registers)
                Select((7,1,0,3), n => n != 13, 
                    Select((3,4), n => n != 13,
                        Instr(Opcode.add, ".T,R3"),
                        AddSpRegisterT1),
                    Select((3,4), n => n != 13,
                        AddSpRegisterT2, 
                        AddSpRegisterT1)),
                Instr(Opcode.cmp, ".T,R3"),
                Instr(Opcode.mov, "T,R3"), // mov,movs
                invalid);

            var LdrLiteral = Instr(Opcode.ldr, "r8,[P,I0:8,w]");

            var LdStRegOffset = Mask(9, 7,
                Instr(Opcode.str, "r0,[r3,r6,w]"),
                Instr(Opcode.strh, "r0,[r3,r6,h]"),
                Instr(Opcode.strb, "r0,[r3,r6,b]"),
                Instr(Opcode.ldrsb, "r0,[r3,r6,B]"),

                Instr(Opcode.ldr, "r0,[r3,r6,w]"),
                Instr(Opcode.ldrh, "r0,[r3,r6,h]"),
                Instr(Opcode.ldrb, "r0,[r3,r6,b]"),
                Instr(Opcode.ldrsh, "r0,[r3,r6,H]"));

            var decLdStWB = Nyi("LdStWB");
            var decLdStHalfword = Nyi("LdStHalfWord");
            var decLdStSpRelative = Nyi("LdStSpRelative");
            var decAddPcSp = Mask(11, 1,
                Instr(Opcode.adr, "r8,P0:8"),
                Instr(Opcode.add, "r8,sp,s0:8<2"));
            var decMisc16Bit = CreateMisc16bitDecoder();
            var decLdmStm = new LdmStmDecoder16();
            var decCondBranch = Mask(8, 0xF, // "CondBranch"
                Instr(Opcode.b, "c8p0:8<1"),
                Instr(Opcode.b, "c8p0:8<1"),
                Instr(Opcode.b, "c8p0:8<1"),
                Instr(Opcode.b, "c8p0:8<1"),

                Instr(Opcode.b, "c8p0:8<1"),
                Instr(Opcode.b, "c8p0:8<1"),
                Instr(Opcode.b, "c8p0:8<1"),
                Instr(Opcode.b, "c8p0:8<1"),

                Instr(Opcode.b, "c8p0:8<1"),
                Instr(Opcode.b, "c8p0:8<1"),
                Instr(Opcode.b, "c8p0:8<1"),
                Instr(Opcode.b, "c8p0:8<1"),

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
                    LdrLiteral,
                    LdrLiteral,

                    LdStRegOffset,
                    LdStRegOffset,
                    LdStRegOffset,
                    LdStRegOffset),
                Mask(11, 0x03,   // decLdStWB,
                    Instr(Opcode.str, "r0,[r3,I6:5,w]"),
                    Instr(Opcode.ldr, "r0,[r3,I6:5,w]"),
                    Instr(Opcode.strb, "r0,[r3,i6:5,b]"),
                    Instr(Opcode.ldrb, "r0,[r3,i6:5,b]")),

                Mask(12, 0x01,
                    Mask(11, 0x01,
                        Instr(Opcode.strh, "r0,[r3,I6:5,h]"),
                        Instr(Opcode.ldrh, "r0,[r3,I6:5,h]")),
                    Mask(11, 0x01,   // load store SP-relative
                        Instr(Opcode.str, "r8,[s,I0:8,w]"),
                        Instr(Opcode.ldr, "r8,[s,I0:8,w]"))),
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
                Select((6,5), n => n != 0,
                    new MovMovsDecoder(Opcode.lsl, ".r0,r3,S6:5"),
                    Instr(Opcode.mov, "r0,r3")),
                new MovMovsDecoder(Opcode.lsr, ".r0,r3,S6:5"),
                Instr(Opcode.asrs, ".r0,r3,S6:5"),
                invalid);
            var decAddSub = Mask(11, 3,
                Instr(Opcode.mov, "r8,i0:8"),
                Instr(Opcode.cmp, ".r8,i0:8"),
                Instr(Opcode.add, ".r8,i0:8"),
                Instr(Opcode.sub, ".r8,i0:8"));
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
                Instr(Opcode.and, ":r0,r3"),
                Instr(Opcode.eor, ":r0,r3"),
                Instr(Opcode.lsl, ":r0,r3"),
                Instr(Opcode.lsr, ":r0,r3"),

                Instr(Opcode.asr, ":r0,r3"),
                Instr(Opcode.adc, ":r0,r3"),
                Instr(Opcode.sbc, ":r0,r3"),
                Instr(Opcode.ror, ":r0,r3"),

                Instr(Opcode.adc, ":r0,r3"),
                Instr(Opcode.rsb, ":r0,r3"),
                Instr(Opcode.cmp, ".r0,r3"),
                Instr(Opcode.cmn, ".r0,r3"),

                Instr(Opcode.orr, ":r0,r3"),
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
                    Instr(Opcode.setpan, "i3:1"),
                    Nyi("ChangeProcessorState"),
                    invalid,
                    invalid,

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
            var loadStoreMultipleTableBranch = CreateLoadStoreDualMultipleBranchDecoder();

            var LdStMultiple = Mask(7 + 16, 3,
                Mask(4 + 16, 1,
                    Instr(Opcode.srsdb, "w21 sp,i0:5"),
                    Instr(Opcode.rfedb, "w21 R16")),
                Mask(4 + 16, 1,
                    new LdmStmDecoder32(Opcode.stm, "R16,M"),
                    new LdmStmDecoder32(Opcode.ldm, "R16,M")),
                Mask(4 + 16, 1,
                    new LdmStmDecoder32(Opcode.stmdb, "R16,M"),
                    new LdmStmDecoder32(Opcode.ldmdb, "R16,M")),
                Mask(4 + 16, 1,
                    Instr(Opcode.srsia, "w21 sp,i0:5"),
                    Instr(Opcode.rfeia, "w21 R16")));

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
                Instr(Opcode.ssat, R8, ImmM1(0,5), R16, LslImm(12, 3, 6, 2)),
                Select(w => SBitfield(w, 12, 3) != 0 || SBitfield(w, 6, 2) != 0,
                    Nyi("ssatAsrVariant"),
                    Nyi("ssat16")),
                Instr(Opcode.sbfx, R8, R16,Imm(12,3,6,2), ImmM1(0, 5)),
                Select(w => SBitfield(w, 16, 4) != 0xF,
                    DecodeBfcBfi(Opcode.bfi, "R8,R16,i12:3:6:2,i0:5"),
                    DecodeBfcBfi(Opcode.bfc, "R8,i12:3:6:2,i0:5")),
                // 4
                Instr(Opcode.usat, R8, ImmM1(0,5), R16, LslImm(12, 3, 6, 2)),
                Select(w => SBitfield(w, 12, 3) != 0 || SBitfield(w, 6, 2) != 0,
                    Nyi("usatAsrVariant"),
                    Nyi("usat16")),
                Instr(Opcode.ubfx, R8, R16,Imm(12,3,6,2), ImmM1(0, 5)),
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
                    Instr(Opcode.ldrsb, "R12,[R16,i0:12,Bx]"),
                    Instr(Opcode.ldrsh, "R12,[R16,i0:12,Hx]"),
                    invalid,
                    invalid),
                Mask(5 + 16, 3,
                    Nyi("PLI"),
                    Instr(Opcode.nop, ""),
                    invalid,
                    invalid));   // reserved hint

            var LoadStoreSignedImmediatePostIndexed = Mask(5 + 16, 3,
                Instr(Opcode.ldrsb, "R12,[R16,i0:8,Bx]"),
                Instr(Opcode.ldrsh, "R12,[R16,i0:8,Hx]"),
                invalid,
                invalid);

            var LoadStoreUnsignedImmediatePostIndexed = Mask(4 + 16, 7,
                Instr(Opcode.strb, "R12,[R16,i0:8,bx]"),
                Instr(Opcode.ldrb, "R12,[R16,i0:8,bx]"),
                Instr(Opcode.strh, "R12,[R16,i0:8,hx]"),
                Instr(Opcode.ldrh, "R12,[R16,i0:8,hx]"),
                Instr(Opcode.str, "R12,[R16,i0:8,wx]"),
                Instr(Opcode.ldr, "R12,[R16,i0:8,wx]"),
                invalid,
                invalid);

            var LoadStoreUnsignedPositiveImm = Mask(4 + 16, 7,
                Instr(Opcode.strb, "R12,[R16,i0:12,b]"),
                Select(w => SBitfield(w, 12, 4) != 0xF,
                    Instr(Opcode.ldrb, "R12,[R16,i0:8,b]"),
                    Nyi("PLD,PLDW immediate preloadread")),
                Instr(Opcode.strh, "R12,[R16,i0:12,h]"),
                Select(w => SBitfield(w, 12, 4) != 0xF,
                    Instr(Opcode.ldrh, "R12,[R16,i0:12,h]"),
                    Nyi("*PLD,PLDW immediate preloadwrite")),
                // 4
                Instr(Opcode.str, "R12,[R16,i0:12,w]"),
                Instr(Opcode.ldr, "R12,[R16,i0:12,w]"),
                invalid,
                invalid);

            var LoadStoreUnsignedImmediatePreIndexed = Mask(4 + 16, 7,
                Instr(Opcode.strb, "R12,[R16,i0:8,bx]"),
                Instr(Opcode.ldrb, "R12,[R16,i0:8,bx]"),
                Instr(Opcode.strh, "R12,[R16,i0:8,hx]"),
                Instr(Opcode.ldrh, "R12,[R16,i0:8,hx]"),
                Instr(Opcode.str, "R12,[R16,i0:8,wx]"),
                Instr(Opcode.str, "R12,[R16,i0:8,wx]"),
                invalid,
                invalid);

            var LoadStoreUnsignedRegisterOffset = Mask(4 + 16, 7,
                Instr(Opcode.strb, "R12,[R16,R0<4:2,b]"),
                Select((16, 4), n => n != 0xF,
                    Instr(Opcode.ldrb, "q R12,[R16,R0<4:2,b]"),
                    Instr(Opcode.pld, "*")),
                Instr(Opcode.strh, "R12,[R16,R0<4:2,h]"),
                Select((16, 4), n => n != 0xF,
                    Instr(Opcode.ldrh, "q R12,[R16,R0<4:2,h]"),
                    Instr(Opcode.pld, "*")),
                Instr(Opcode.str, "q R12,[R16,R0<4:2,w]"),
                Instr(Opcode.ldr, "q R12,[R16,R0<4:2,w]"),
                invalid,
                invalid);

            var LoadStoreUnsignedNegativeImm = Mask(4 + 16, 7,
                Instr(Opcode.strb, "R12,[R16,i0:8,bx]"),
                Select((16, 4), n => n != 0xF,
                    Instr(Opcode.ldrb, "q R12,[R16,i0:8,bx]"),
                    Instr(Opcode.pld, "*")),
                Instr(Opcode.strh, "R12,[R16,i0:8,hx]"),
                Select((16, 4), n => n != 0xF,
                    Instr(Opcode.ldrh, "q R12,[R16,i0:8,hx]"),
                    Instr(Opcode.pld, "*")),
                Instr(Opcode.str, "q R12,[R16,i0:8,wx]"),
                Instr(Opcode.ldr, "q R12,[R16,i0:8,wx]"),
                invalid,
                invalid);

            var LoadStoreUnsignedUnprivileged = Mask(4 + 16, 7,
                Instr(Opcode.strbt, "R12,[R16,i0:8,b]"),
                Instr(Opcode.ldrbt, "R12,[R16,i0:8,b]"),
                Instr(Opcode.strht, "R12,[R16,i0:8,h]"),
                Instr(Opcode.ldrht, "R12,[R16,i0:8,h]"),
                Instr(Opcode.strt, "R12,[R16,i0:8,w]"),
                Instr(Opcode.ldrt, "R12,[R16,i0:8,w]"),
                invalid,
                invalid);

            var LoadUnsignedLiteral = Select((12,4), n => n != 0xF,
                Mask(5 + 16, 3,
                    Instr(Opcode.ldrb, "R12,[P,i0:12,b]"),
                    Instr(Opcode.ldrh, "R12,[P,i0:12,h]"),
                    Instr(Opcode.ldr, "R12,[P,i0:12,w]"),
                    invalid),
                Mask(5 + 16, 3,
                    Instr(Opcode.pld, "* literal"),
                    Instr(Opcode.pld, "* literal"),
                    invalid,
                    invalid));

            var LoadSignedLiteral = Select((12,4), n => n != 0xF,
                Mask(5 + 16, 3,
                    Instr(Opcode.ldrsb, "R12,[P,i8:4:0:4,B]"),
                    Instr(Opcode.ldrsh, "R12,[P,i8:4:0:4,H]"),
                    invalid,
                    invalid),
                Mask(5 + 16, 3,
                    Instr(Opcode.pli, "* literal"),
                    Instr(Opcode.nop, ""),
                    invalid,
                    invalid));

            var LoadStoreSignedRegisterOffset = Select((12,4), n => n != 0xF,
                Mask(5 + 16, 3,
                    Instr(Opcode.ldrsb, "*register"),
                    Instr(Opcode.ldrsh, "*register"),
                    invalid,
                    invalid),
                Mask(5 + 16, 3,
                    Instr(Opcode.pli, "*register"),
                    Instr(Opcode.nop, ""),
                    invalid,
                    invalid));

            var LoadStoreSingle = Mask(7 + 16, 3,
                Select((16,4), n => n != 0xF,
                    Mask(10, 3,
                        Select(w => SBitfield(w, 6, 6) == 0,
                            LoadStoreUnsignedRegisterOffset,
                            invalid),
                        invalid,
                        Select(w => SBitfield(w, 8, 1) == 0,
                            invalid,
                            LoadStoreUnsignedImmediatePostIndexed),
                        Mask(8, 3,
                            LoadStoreUnsignedNegativeImm,
                            LoadStoreUnsignedImmediatePreIndexed,
                            LoadStoreUnsignedUnprivileged,
                            LoadStoreUnsignedImmediatePreIndexed)),
                    LoadUnsignedLiteral),
                Select((16,4), n => n != 0xF,
                    LoadStoreUnsignedPositiveImm,
                    LoadUnsignedLiteral),
                Select(w => SBitfield(w, 16, 4) != 0xF,
                    Mask(10, 3,
                        Select(w => SBitfield(w, 6, 6) == 0,
                            LoadStoreSignedRegisterOffset,
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
                    LoadSignedLiteral),
                Select(w => SBitfield(w, 16, 4) != 0xF,
                    LoadStoreSignedPositiveImm,
                    LoadSignedLiteral));

            var ldc_literal = Nyi("LDC (literal)");

            var SystemRegisterLdSt = Select((8,1), n => n != 0,
                invalid,
                Select((12,4), n => n != 5,
                    invalid,
                    Select((22,1), n => n != 0,
                        invalid,
                        Bitfields("23:2:20:2", // PU-WL 
                            invalid,
                            invalid,
                            Instr(Opcode.stc, "*post-indexed"),
                            Select((16,4), n => n != 15,
                                Instr(Opcode.ldc, "*imm"),
                                ldc_literal),

                            Instr(Opcode.stc, "*unindexed variant"),
                            Select((16,4), n => n != 15,
                                Instr(Opcode.ldc, "*immediate - unindexed variant"),
                                ldc_literal),
                            invalid,
                            invalid, 

                            Instr(Opcode.stc, "*offset variant"),
                            Select((16,4), n => n != 15,
                                Instr(Opcode.ldc, "*offset variant"),
                                ldc_literal),
                            Instr(Opcode.stc, "*preindexed variant"),
                            Select((16,4), n => n != 15,
                                Instr(Opcode.ldc, "*preindexed variant"),
                                ldc_literal),

                            Instr(Opcode.stc, "CP#14,CR12,[R16,I0:8,w]"),
                            Select((16,4), n => n != 15,
                                Instr(Opcode.ldc, "*offset variant"),
                                ldc_literal),
                            Instr(Opcode.stc, "*preindexed variant"),
                            Select((16,4), n => n != 15,
                                Instr(Opcode.ldc, "*preindexed variant"),
                                ldc_literal)))));

            var SystemRegisterLdStAnd64bitMove = Select((23,2,21,1), n => (n & 0xD) == 0,
                Nyi("SystemRegister64bitMove"),
                SystemRegisterLdSt);

            var vstmia = Mask(8, 0x3, // size
                    invalid,
                    invalid,
                    Instr(Opcode.vstmia, "*"),
                    Mask(0, 1,
                        Instr(Opcode.vstmia, "*"),
                        Instr(Opcode.fstmiax, "*")));

            var vldmia = Mask(8, 0x3, // size
                    invalid,
                    invalid,
                    Instr(Opcode.vldmia, "*"),
                    Mask(0, 1,
                        Instr(Opcode.vldmia, "*"),
                        Instr(Opcode.fldmiax, "*")));
            var vstr = Mask(8, 3,  // size
                invalid,
                Instr(Opcode.vstr, "F12:4:22:1,[R16,I0:8,r16X]"),
                Instr(Opcode.vstr, "F12:4:22:1,[R16,I0:8,r32X]"),
                Instr(Opcode.vstr, "F22:1:12:4,[R16,I0:8,r64X]"));
            var vldr = Select(w => SBitfield(w, 16, 4) != 0xF,
                Mask(8, 3,
                    invalid,
                    Instr(Opcode.vldr, "F12:4:22:1,[R16,I0:8,r16X]"),
                    Instr(Opcode.vldr, "F12:4:22:1,[R16,I0:8,r32X]"),
                    Instr(Opcode.vldr, "D22:1:12:4,[R16,I0:8,r64X]")),
                Instr(Opcode.vldr, "*lit"));
            var AdvancedSimdAndFpLdSt = Mask(4 + 16, 0x1F,
                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                vstmia,
                vldmia,
                vstmia,
                vldmia,

                vstmia,
                vldmia,
                vstmia,
                vldmia,
                // 0x10
                vstr,
                vldr,
                invalid,
                invalid,

                vstr,
                vldr,
                invalid,
                invalid,

                vstr,
                vldr,
                invalid,
                invalid,

                vstr,
                vldr,
                invalid,
                invalid);


            var AvancedSimdLdStAnd64bitMove = Select(w => (SBitfield(w, 5 + 16, 4) & 0b1101) == 0,
                Nyi("AdvancedSimdAndFp64bitMove"),
                AdvancedSimdAndFpLdSt);

            var FloatingPointDataProcessing3Regs = Mask(7+16, 1,
                Mask(4, 0b11,
                    Nyi("FloatingPointDataProcessing3Regs - o0:o1 = 000"),
                    Nyi("FloatingPointDataProcessing3Regs - o0:o1 = 001"),
                    Mask(8, 0b11,
                        invalid,
                        Instr(Opcode.vnmul, "vfh F12:4:22:1,F16:4:7:1,F0:4:5:1"),
                        Instr(Opcode.vnmul, "vfs F12:4:22:1,F16:4:7:1,F0:4:5:1"),
                        Instr(Opcode.vnmul, "vfd D22:1:12:4,D7:1:16:4,D5:1:0:4")),
                    Nyi("FloatingPointDataProcessing3Regs - o0:o1 = 011")),
                Mask(4, 0b11,
                    Nyi("FloatingPointDataProcessing3Regs - o0:o1 = 100"),
                    Nyi("FloatingPointDataProcessing3Regs - o0:o1 = 101"),
                    Nyi("FloatingPointDataProcessing3Regs - o0:o1 = 110"),
                    Nyi("FloatingPointDataProcessing3Regs - o0:o1 = 111")));

            var FloatingPointMoveImm = Nyi("FloatingPointMoveImm");

            var FloatingPointConditionalSelect = Select((8,2), n => n == 1,
                invalid,
                Mask(20, 3,
                    Instr(Opcode.vseleq, "vfd D22:1:12:4,D7:1:16:4,D5:1:0:4"),
                    Instr(Opcode.vselvs, "vfd D22:1:12:4,D7:1:16:4,D5:1:0:4"),
                    Instr(Opcode.vselge, "vfd D22:1:12:4,D7:1:16:4,D5:1:0:4"),
                    Instr(Opcode.vselgt, "vfd D22:1:12:4,D7:1:16:4,D5:1:0:4")));

            var FloatingPointMinNumMaxNum =
                Mask(6, 1,
                    Mask(8, 3,
                        invalid,
                        Instr(Opcode.vmaxnm, "vfh F12:4:22:1,F16:4:7:1,F0:4:5:1"),
                        Instr(Opcode.vmaxnm, "vfs F12:4:22:1,F16:4:7:1,F0:4:5:1"),
                        Instr(Opcode.vmaxnm, "vfd D22:1:12:4,D7:1:16:4,D5:1:0:4")),
                    Mask(8, 3,
                        invalid,
                        Instr(Opcode.vminnm, "vfh F12:4:22:1,F16:4:7:1,F0:4:5:1"),
                        Instr(Opcode.vminnm, "vfs F12:4:22:1,F16:4:7:1,F0:4:5:1"),
                        Instr(Opcode.vminnm, "vfd D22:1:12:4,D7:1:16:4,D5:1:0:4")));

            var FloatingPointExtIns = Nyi("FloatingPointExtIns");
            var FloatingPointDirectedCvt2Int = Nyi("FloatingPointDirectedCvt2Int");

            var FloatingPointDataProcessing = Mask(12 + 16, 1, // op0
                Mask(4 + 16, 0xF, // op1
                    FloatingPointDataProcessing3Regs,
                    FloatingPointDataProcessing3Regs,
                    FloatingPointDataProcessing3Regs,
                    FloatingPointDataProcessing3Regs,

                    FloatingPointDataProcessing3Regs,
                    FloatingPointDataProcessing3Regs,
                    FloatingPointDataProcessing3Regs,
                    FloatingPointDataProcessing3Regs,

                    FloatingPointDataProcessing3Regs,
                    FloatingPointDataProcessing3Regs,
                    FloatingPointDataProcessing3Regs,
                    Mask(6, 1,
                        FloatingPointMoveImm,
                        FloatingPointDataProcessing3Regs),

                    FloatingPointDataProcessing3Regs,
                    FloatingPointDataProcessing3Regs,
                    FloatingPointDataProcessing3Regs,
                    Mask(6, 1,
                        FloatingPointMoveImm,
                        FloatingPointDataProcessing3Regs)),
                Select((8,2), n => n != 0,
                    Mask(4 + 16, 0xF, // op1
                        FloatingPointConditionalSelect,
                        FloatingPointConditionalSelect,
                        FloatingPointConditionalSelect,
                        FloatingPointConditionalSelect,

                        FloatingPointConditionalSelect,
                        FloatingPointConditionalSelect,
                        FloatingPointConditionalSelect,
                        FloatingPointConditionalSelect,

                        FloatingPointMinNumMaxNum,
                        invalid,
                        invalid,
                        Mask(6, 1,
                            invalid,
                            Select((16,4), n => n == 0,
                                FloatingPointExtIns,
                                Mask(19, 1,
                                    invalid,
                                    FloatingPointDirectedCvt2Int))),

                        FloatingPointMinNumMaxNum,
                        invalid,
                        invalid,
                        Mask(6, 1,
                            invalid,
                            Select((16,4), n => n == 0,
                                FloatingPointExtIns,
                                Mask(19, 1,
                                    invalid,
                                    FloatingPointDirectedCvt2Int)))),
                    invalid));

            var AdvancedSimdAndFloatingPoint32bitMove = Mask(8, 1,
                Select((21,3), n => n == 0,
                    Instr(Opcode.vmov, "*between GPR and single prec"),
                    Select((21,3), n => n == 7,
                        Mask(20, 1,
                            Instr(Opcode.vmsr, "*"),
                            Instr(Opcode.vmrs, "*")),
                        invalid)),
                Nyi("AdvancedSimd8_16_32_bitElementMove"));

            var AdvancedSimdLdStMultipleStructures = Mask(21, 1,
                Mask(8, 15,
                    Instr(Opcode.vst4, "*"),
                    Instr(Opcode.vst4, "*"),
                    Instr(Opcode.vst1, "*multiple single elements - T4"),
                    Instr(Opcode.vst2, "*multiple 2-element structures - T2"),

                    Instr(Opcode.vst3, "*multiple 3-element structures"),
                    Instr(Opcode.vst3, "*multiple 3-element structures"),
                    Instr(Opcode.vst1, "*multiple single elements - T3"),
                    Instr(Opcode.vst1, "*multiple single elements - T1"),

                    Instr(Opcode.vst2, "*multiple 2-element structures - T1"),
                    Instr(Opcode.vst2, "*multiple 2-element structures - T1"),
                    Instr(Opcode.vst1, "*multiple single elements - T2"),
                    invalid,

                    invalid,
                    invalid,
                    invalid,
                    invalid),
                Nyi("AdvancedSimdLdStMultipleStructures - 1"));

            var AdvancedSimdElementOrStructureLdSt = Mask(7 + 16, 1,
                AdvancedSimdLdStMultipleStructures,
                Mask(10, 3,
                    Nyi("AdvancedSimdLdStSingleStructureOneLane"),
                    Nyi("AdvancedSimdLdStSingleStructureOneLane"),
                    Nyi("AdvancedSimdLdStSingleStructureOneLane"),
                    Nyi("AdvancedSimdLdSingleStructureToAllLanes")));

            var SystemRegister32bitMove = Mask(12 + 16, 1, 
                Mask(4 + 16, 1,
                    Instr(Opcode.mcr, "CP8,i21:3,R12,CR16,CR0,i5:3"),
                    Instr(Opcode.mrc, "CP8,i21:3,R12,CR16,CR0,i5:3")),
                invalid);

            var AdvancedSimd3RegistersSameLength = Mask(8, 0xF, // opc
                Mask(4, 1, // o1
                    Mask(6, 1,
                        Instr(Opcode.vhadd, "vu20:2 D22:1:12:4,D7:1:16:4,D5:1:0:4"),
                        Instr(Opcode.vhadd, "vu20:2 Q22:1:12:4,Q7:1:16:4,Q5:1:0:4")),
                    Mask(6, 1,
                        Instr(Opcode.vqadd, "vu20:2 D22:1:12:4,D7:1:16:4,D5:1:0:4"),
                        Instr(Opcode.vqadd, "vu20:2 Q22:1:12:4,Q7:1:16:4,Q5:1:0:4"))),
                Mask(12 + 16, 1,  // U
                    Mask(4, 1,      // o1
                        Instr(Opcode.vrhadd, "*"),
                        Mask(4 + 16, 3, // size
                            Instr(Opcode.vand, "*register"),
                            Instr(Opcode.vbic, "*register"),
                            Instr(Opcode.vorr, "*register"),
                            Instr(Opcode.vorn, "*register"))),
                    Mask(4, 1,      // o1),
                        Instr(Opcode.vrhadd, "*"),
                        Mask(4 + 16, 3, // size
                            Mask(6, 1, // Q
                                Instr(Opcode.veor, "D22:1:12:4,D7:1:16:4,D5:1:0:4"),
                                Instr(Opcode.veor, "Q22:1:12:4,Q7:1:16:4,Q5:1:0:4")),
                            Instr(Opcode.vbsl, "*register"),
                            Instr(Opcode.vbit, "*register"),
                            Instr(Opcode.vbif, "*register")))),
                Mask(4, 1, // o1
                    Mask(6, 1,
                        Instr(Opcode.vhsub, "vu20:2 D22:1:12:4,D7:1:16:4,D5:1:0:4"),
                        Instr(Opcode.vhsub, "vu20:2 Q22:1:12:4,Q7:1:16:4,Q5:1:0:4")),
                    Instr(Opcode.vqsub, "*")),
                Nyi("AdvancedSimd3RegistersSameLength_opc3"),

                Nyi("AdvancedSimd3RegistersSameLength_opc4"),
                Mask(4, 1,
                    Mask(6, 1, // Q
                        Instr(Opcode.vrshl, "vu20:2,D22:1:12:4,D7:1:16:4,D5:1:0:4"),
                        Instr(Opcode.vrshl, "vu20:2,Q22:1:12:4,Q7:1:16:4,Q5:1:0:4")),
                    Mask(6, 1, // Q
                        Instr(Opcode.vqrshl, "vu20:2,D22:1:12:4,D7:1:16:4,D5:1:0:4"),
                        Instr(Opcode.vqrshl, "vu20:2,Q22:1:12:4,Q7:1:16:4,Q5:1:0:4"))),
                Mask(4, 1,
                    Mask(6, 1, // Q
                        Instr(Opcode.vmax, "vu20:2,D22:1:12:4,D7:1:16:4,D5:1:0:4"),
                        Instr(Opcode.vmax, "vu20:2,Q22:1:12:4,Q7:1:16:4,Q5:1:0:4")),
                    Mask(6, 1, // Q
                        Instr(Opcode.vmin, "vu20:2,D22:1:12:4,D7:1:16:4,D5:1:0:4"),
                        Instr(Opcode.vmin, "vu20:2,Q22:1:12:4,Q7:1:16:4,Q5:1:0:4"))),
                Nyi("AdvancedSimd3RegistersSameLength_opc7"),

                Mask(12 + 16, 1,  // U
                    Mask(4, 1, // op1
                        Mask(6, 1, // Q
                            Instr(Opcode.vadd, "vi20:2,D22:1:12:4,D7:1:16:4,D5:1:0:4"),
                            Instr(Opcode.vadd, "vi20:2,Q22:1:12:4,Q7:1:16:4,Q5:1:0:4")),
                        Mask(6, 1, // Q
                            Instr(Opcode.vtst, "vi20:2,D22:1:12:4,D7:1:16:4,D5:1:0:4"),
                            Instr(Opcode.vtst, "vi20:2,Q22:1:12:4,Q7:1:16:4,Q5:1:0:4"))),
                    Mask(4, 1, // op1
                        Mask(6, 1, // Q
                            Instr(Opcode.vsub, "vi20:2,D22:1:12:4,D7:1:16:4,D5:1:0:4"),
                            Instr(Opcode.vsub, "vi20:2,Q22:1:12:4,Q7:1:16:4,Q5:1:0:4")),
                        Instr(Opcode.vceq, "*"))),

                // opc9
                Mask(12 + 16, 1,  // U
                    Mask(4, 1,      // op1
                        Mask(6, 1, // Q
                            Instr(Opcode.vmla, "vi20:2,D22:1:12:4,D7:1:16:4,D5:1:0:4"),
                            Instr(Opcode.vmla, "vi20:2,Q22:1:12:4,Q7:1:16:4,Q5:1:0:4")),
                        Nyi("*vmul (integer and polynomial")),
                    Mask(4, 1,      // op1
                        Mask(6, 1, // Q
                            Instr(Opcode.vmls, "vi20:2,D22:1:12:4,D7:1:16:4,D5:1:0:4"),
                            Instr(Opcode.vmls, "vi20:2,Q22:1:12:4,Q7:1:16:4,Q5:1:0:4")),
                        Nyi("*vmul (integer and polynomial"))),
                Mask(6, 1, // Q
                    Mask(4, 1, // op1
                        Instr(Opcode.vpmax, "vu20:2 D22:1:12:4,D7:1:16:4,D5:1:0:4"),
                        Instr(Opcode.vpmin, "vu20:2 D22:1:12:4,D7:1:16:4,D5:1:0:4")),
                    invalid),
                Nyi("AdvancedSimd3RegistersSameLength_opcB"),

                Nyi("AdvancedSimd3RegistersSameLength_opcC"),
                // opcD
                Mask(12 + 16, 1,  // U
                    Mask(4, 1,      // op1
                        Mask(6, 1,      // Q
                            Mask(20, 3,  // size
                                Instr(Opcode.vadd, "vfs D22:1:12:4,D7:1:16:4,D5:1:0:4"),
                                Instr(Opcode.vadd, "vfh D22:1:12:4,D7:1:16:4,D5:1:0:4"),
                                Instr(Opcode.vsub, "vfs D22:1:12:4,D7:1:16:4,D5:1:0:4"),
                                Instr(Opcode.vsub, "vfh D22:1:12:4,D7:1:16:4,D5:1:0:4")),
                            Mask(20, 3,  // size
                                Instr(Opcode.vadd, "vfs Q22:1:12:4,Q7:1:16:4,Q5:1:0:4"),
                                Instr(Opcode.vadd, "vfh Q22:1:12:4,Q7:1:16:4,Q5:1:0:4"),
                                Instr(Opcode.vsub, "vfs Q22:1:12:4,Q7:1:16:4,Q5:1:0:4"),
                                Instr(Opcode.vsub, "vfh Q22:1:12:4,Q7:1:16:4,Q5:1:0:4"))),
                        Mask(20, 3,  // high-bit of size
                            Nyi("*vmla (floating point)"),
                            Nyi("*vmla (floating point)"),
                            Nyi("*vmls (floating point)"),
                            Nyi("*vmls (floating point)"))),
                    Mask(4, 1,      // op1
                        Mask(20, 3,  // size
                            Instr(Opcode.vpadd, "vfs D22:1:12:4,D7:1:16:4,D5:1:0:4"),
                            Instr(Opcode.vpadd, "vfh D22:1:12:4,D7:1:16:4,D5:1:0:4"),
                            Nyi("*vabd (floating point)"),
                            Nyi("*vabd (floating point)")),
                        Mask(21, 1,  // high-bit of size
                            Mask(6, 1,      // Q
                                Instr(Opcode.vmul, "vfs D22:1:12:4,D7:1:16:4,D5:1:0:4"),
                                Instr(Opcode.vmul, "vfh Q22:1:12:4,Q7:1:16:4,Q5:1:0:4")),
                            invalid))),

                // opc = E
                Mask(12 + 16, 1,  // U
                    Nyi("AdvancedSimd3RegistersSameLength_opcE U=0"),
                    Mask(21, 1,  // high-bit of size
                        Mask(4, 1,      // op1
                            Mask(6, 1,      // Q
                                Instr(Opcode.vcge, "vfs D22:1:12:4,D7:1:16:4,D5:1:0:4"),
                                Instr(Opcode.vcge, "vfh Q22:1:12:4,Q7:1:16:4,Q5:1:0:4")),
                            Instr(Opcode.vacge, "*")),
                        Nyi("AdvancedSimd3RegistersSameLength_opcE U=1 size=1x"))),
                Mask(12 + 16, 1,  // U
                    Nyi("AdvancedSimd3RegistersSameLength_opcF U=0"),
                    Mask(4, 1,      // op1
                        Mask(6, 1,      // Q
                            Mask(20, 3,  // size
                                Instr(Opcode.vpmax, "vfs D22:1:12:4,D7:1:16:4,D5:1:0:4"),
                                Instr(Opcode.vpmax, "vfh D22:1:12:4,D7:1:16:4,D5:1:0:4"),
                                Instr(Opcode.vpmin, "vfs D22:1:12:4,D7:1:16:4,D5:1:0:4"),
                                Instr(Opcode.vpmin, "vfh D22:1:12:4,D7:1:16:4,D5:1:0:4")),
                        Nyi("AdvancedSimd3RegistersSameLength_opcF U=0 op1 = 0 Q=1")),
                    Nyi("AdvancedSimd3RegistersSameLength_opcF U=0 op1 = 1"))));

            var AdvancedSimd2RegsMisc = Mask(16, 3,
                Mask(7, 0xF,
                    Instr(Opcode.vrev64, "*"),
                    Instr(Opcode.vrev32, "*"),
                    Instr(Opcode.vrev16, "*"),
                    invalid,

                    Instr(Opcode.vpaddl, "*"),
                    Instr(Opcode.vpaddl, "*"),
                    Mask(6, 1,
                        Instr(Opcode.aese, "*"),
                        Instr(Opcode.aesd, "*")),
                    Mask(6, 1,
                        Instr(Opcode.aesmc, "*"),
                        Instr(Opcode.aesimc, "*")),

                    invalid, //$REVIEW VSWP looks odd.
                    Instr(Opcode.vclz, "*"),
                    Instr(Opcode.vcnt, "*"),
                    Instr(Opcode.vmvn, "*reg"),

                    Instr(Opcode.vpadal, "*"),
                    Instr(Opcode.vpadal, "*"),
                    Instr(Opcode.vqabs, "*"),
                    Instr(Opcode.vqneg, "*")),
                Mask(7, 0xF,
                    Instr(Opcode.vcgt, "*imm0"),
                    Instr(Opcode.vcge, "*imm0"),
                    Instr(Opcode.vceq, "*imm0"),
                    Instr(Opcode.vcle, "*imm0"),

                    Instr(Opcode.vclt, "*imm0"),
                    Mask(6, 1,
                        invalid,
                        Instr(Opcode.sha1h, "*")),
                    Mask(6, 1,
                        Mask(10, 1,
                            Instr(Opcode.vabs, "vi18:2,D22:1:12:4,D5:1:0:4"),
                            Instr(Opcode.vabs, "vr18:2,D22:1:12:4,D5:1:0:4")),
                        Mask(10, 1,
                            Instr(Opcode.vabs, "vi18:2,Q22:1:12:4,Q5:1:0:4"),
                            Instr(Opcode.vabs, "vr18:2,Q22:1:12:4,Q5:1:0:4"))),
                    Instr(Opcode.vneg, "*"),

                    Instr(Opcode.vcgt, "*imm0"),
                    Instr(Opcode.vcge, "*imm0"),
                    Instr(Opcode.vceq, "*imm0"),
                    Instr(Opcode.vcle, "*imm0"),

                    Instr(Opcode.vclt, "*imm0"),
                    invalid,
                    Mask(6, 1,
                        Mask(10, 1,
                            Instr(Opcode.vabs, "vi18:2,D22:1:12:4,D5:1:0:4"),
                            Instr(Opcode.vabs, "vr18:2,D22:1:12:4,D5:1:0:4")),
                        Mask(10, 1,
                            Instr(Opcode.vabs, "vi18:2,Q22:1:12:4,Q5:1:0:4"),
                            Instr(Opcode.vabs, "vr18:2,Q22:1:12:4,Q5:1:0:4"))),
                    Instr(Opcode.vqneg, "*")),
                Mask(7, 0xF,
                    invalid,
                    Instr(Opcode.vtrn, "*"),
                    Instr(Opcode.vuzp, "*"),
                    Instr(Opcode.vzip, "*"),

                    Mask(6, 1,
                        Instr(Opcode.vmovn, "*"),
                        Instr(Opcode.vqmovn, "*unsigned")),
                    Instr(Opcode.vqmovn, "*signed"),
                    Mask(6, 1,
                        Instr(Opcode.vshll, "*"),
                        invalid),
                    Mask(6, 1,
                        Instr(Opcode.sha1su1, "*"),
                        Instr(Opcode.sha256su0, "*")),

                    Instr(Opcode.vrintn, "*"),
                    Instr(Opcode.vrintx, "*"),
                    Instr(Opcode.vrinta, "*"),
                    Instr(Opcode.vrintz, "*"),

                    Mask(6, 1,
                        Instr(Opcode.vcvt, "vc,D22:1:12:4,D5:1:0:4"),
                        invalid),
                    Instr(Opcode.vrintm, "*"),
                    Mask(6, 1,
                        Instr(Opcode.vcvt, "vc,Q22:1:12:4,Q5:1:0:4"),
                        invalid),
                    Instr(Opcode.vrintp, "*")),
                Mask(4 + 16, 0xF,
                    Instr(Opcode.vcvta, "*"),
                    Instr(Opcode.vcvta, "*"),
                    Instr(Opcode.vcvtn, "*"),
                    Instr(Opcode.vcvtn, "*"),

                    Instr(Opcode.vcvtp, "*"),
                    Instr(Opcode.vcvtp, "*"),
                    Instr(Opcode.vcvtm, "*"),
                    Instr(Opcode.vcvtm, "*"),

                    Instr(Opcode.vrecpe, "*"),
                    Instr(Opcode.vrsqrte, "*"),
                    Instr(Opcode.vrecpe, "*"),
                    Instr(Opcode.vrsqrte, "*"),

                    Mask(6, 1,
                        Instr(Opcode.vcvt, "vc,D22:1:12:4,D5:1:0:4"),
                        Instr(Opcode.vcvt, "vc,Q22:1:12:4,Q5:1:0:4")),
                    Mask(6, 1,
                        Instr(Opcode.vcvt, "vc,D22:1:12:4,D5:1:0:4"),
                        Instr(Opcode.vcvt, "vc,Q22:1:12:4,Q5:1:0:4")),
                    Mask(6, 1,
                        Instr(Opcode.vcvt, "vc,D22:1:12:4,D5:1:0:4"),
                        Instr(Opcode.vcvt, "vc,Q22:1:12:4,Q5:1:0:4")),
                    Mask(6, 1,
                        Instr(Opcode.vcvt, "vc,D22:1:12:4,D5:1:0:4"),
                        Instr(Opcode.vcvt, "vc,Q22:1:12:4,Q5:1:0:4"))));

            var AdvancedSimd3DiffLength = Mask(8, 0xF,  // opc
                Instr(Opcode.vaddl, "*"),
                Instr(Opcode.vaddw, "*"),
                Instr(Opcode.vsubl, "*"),
                Instr(Opcode.vsubw, "*"),

                Mask(12 + 16, 1,
                    Instr(Opcode.vaddhn, "*"),
                    Instr(Opcode.vraddhn, "*")),
                Instr(Opcode.vabal, "*"),
                Mask(12 + 16, 1,
                    Instr(Opcode.vsubhn, "*"),
                    Instr(Opcode.vrsubhn, "*")),
                Instr(Opcode.vabdl, "vi20:2 Q22:1:12:4,D7:1:16:4,D5:1:0:4"),

                Instr(Opcode.vmlal, "vi20:2 Q22:1:12:4,D7:1:16:4,D5:1:0:4"),
                Mask(12 + 16, 1,
                    Instr(Opcode.vqdmlal, "*integer"),
                    invalid),
                Instr(Opcode.vmlsl, "*integer"),
                Mask(12 + 16, 1,
                    Instr(Opcode.vqdmlsl, "*integer"),
                    invalid),

                Instr(Opcode.vmull, "*integer and polynomial"),
                Mask(12 + 16, 1,
                    Instr(Opcode.vqdmull, "*integer"),
                    invalid),
                invalid,
                invalid);

            var AdvancedSimd2RegsScalar = Mask(8, 0xF, // opc
                Mask(12 +16, 1,
                    Instr(Opcode.vmla, "vi D22:1:12:4,D7:1:16:4,D5:1:0:4"),
                    Instr(Opcode.vmla, "vi Q22:1:12:4,Q7:1:16:4,Q5:1:0:4")),
                Mask(12 + 16, 1,
                    Instr(Opcode.vmla, "vF20:2 D22:1:12:4,D7:1:16:4,D5:1:0:4"),
                    Instr(Opcode.vmla, "vF20:2 Q22:1:12:4,Q7:1:16:4,Q5:1:0:4")),
                Instr(Opcode.vmlal, "*scalar"),
                Mask(12 + 16, 1, // Q
                    Instr(Opcode.vqdmlal, "*"),
                    invalid),

                Instr(Opcode.vmls, "*scalar"),
                Instr(Opcode.vmls, "*scalar"),
                Instr(Opcode.vmlsl, "*scalar"),
                Mask(12 + 16, 1, // Q
                    Instr(Opcode.vqdmlsl, "*"),
                    invalid),

                Instr(Opcode.vmul, "*scalar"),
                Instr(Opcode.vmul, "*scalar"),
                Instr(Opcode.vmull, "*"),
                Mask(12 + 16, 1, // Q
                    Instr(Opcode.vqdmull, "*"),
                    invalid),

                Instr(Opcode.vqdmulh, "*"),
                Instr(Opcode.vqrdmlah, "*"),
                Instr(Opcode.vqrdmlah, "*"),
                Instr(Opcode.vqrdmlsh, "*"));

            var AdvancedSimdDuplicateScalar = Nyi("AdvancedSimdDuplicateScalar");

            var AdvancedSimd2RegsOr3RegsDiffLength = Mask(12 + 16, 1,
                Mask(4 + 16, 3,
                    Mask(6, 1,
                        AdvancedSimd3DiffLength,
                        AdvancedSimd2RegsScalar),
                    Mask(6, 1,
                        AdvancedSimd3DiffLength,
                        AdvancedSimd2RegsScalar),
                    Mask(6, 1,
                        AdvancedSimd3DiffLength,
                        AdvancedSimd2RegsScalar),
                    Instr(Opcode.vext, "*")),
                Mask(4 + 16, 3,
                    Mask(6, 1,
                        AdvancedSimd3DiffLength,
                        AdvancedSimd2RegsScalar),
                    Mask(6, 1,
                        AdvancedSimd3DiffLength,
                        AdvancedSimd2RegsScalar),
                    Mask(6, 1,
                        AdvancedSimd3DiffLength,
                        AdvancedSimd2RegsScalar),

                    Mask(10, 3,
                        AdvancedSimd2RegsMisc,
                        AdvancedSimd2RegsMisc,
                        Nyi("VTBL,VTBX"),
                        AdvancedSimdDuplicateScalar)));

            var AdvancedSimdTwoScalarsAndExtension = Nyi("AdvancedSimdTwoScalarsAndExtension");

            var vmov_t1_d = Instr(Opcode.vmov, "viw D22:1:12:4,MS28:1:16:3:0:4");
            var vmov_t1_q = Instr(Opcode.vmov, "viw Q22:1:12:4,MS28:1:16:3:0:4");
            var vmvn_t1_d = Instr(Opcode.vmvn, "viw D22:1:12:4,MS28:1:16:3:0:4");
            var vmvn_t1_q = Instr(Opcode.vmvn, "viw Q22:1:12:4,MS28:1:16:3:0:4");
            var AdvancedSimdOneRegisterAndModifiedImmediate = Mask(8, 0xF,
                Mask(6, 1, // Q
                    Mask(5, 1, vmov_t1_d, vmvn_t1_d),
                    Mask(5, 1, vmov_t1_q, vmvn_t1_q)),
                Mask(6, 1, // Q
                    Mask(5, 1, vmov_t1_d, vmvn_t1_d),
                    Mask(5, 1, vmov_t1_q, vmvn_t1_q)),
                Mask(6, 1, // Q
                    Mask(5, 1, vmov_t1_d, vmvn_t1_d),
                    Mask(5, 1, vmov_t1_q, vmvn_t1_q)),
                Mask(6, 1, // Q
                    Mask(5, 1, vmov_t1_d, vmvn_t1_d),
                    Mask(5, 1, vmov_t1_q, vmvn_t1_q)),

                Mask(6, 1, // Q
                    Mask(5, 1, vmov_t1_d, vmvn_t1_d),
                    Mask(5, 1, vmov_t1_q, vmvn_t1_q)),
                Mask(6, 1, // Q
                    Mask(5, 1, vmov_t1_d, vmvn_t1_d),
                    Mask(5, 1, vmov_t1_q, vmvn_t1_q)),
                Mask(6, 1, // Q
                    Mask(5, 1, vmov_t1_d, vmvn_t1_d),
                    Mask(5, 1, vmov_t1_q, vmvn_t1_q)),
                Mask(6, 1, // Q
                    Mask(5, 1, vmov_t1_d, vmvn_t1_d),
                    Mask(5, 1, vmov_t1_q, vmvn_t1_q)),

                Mask(5, 1,  // op
                    Instr(Opcode.vmov, "*immediate - T3"),
                    Instr(Opcode.vmvn, "*immediate - T2")),
                Mask(5, 1,  // op
                    Instr(Opcode.vorr, "*immediate - T2"),
                    Instr(Opcode.vbic, "*immediate - T2")),
                Mask(5, 1,  // op
                    Instr(Opcode.vmov, "*immediate - T3"),
                    Instr(Opcode.vmvn, "*immediate - T2")),
                Mask(5, 1,  // op
                    Instr(Opcode.vorr, "*immediate - T2"),
                    Instr(Opcode.vbic, "*immediate - T2")),

                Mask(5, 1,  // op
                    Instr(Opcode.vmov, "*immediate - T4"),
                    Instr(Opcode.vmvn, "*immediate - T3")),
                Mask(5, 1,  // op
                    Instr(Opcode.vmov, "*immediate - T4"),
                    Instr(Opcode.vmvn, "*immediate - T3")),
                Mask(5, 1,  // op
                    Instr(Opcode.vmov, "*immediate - T4"),
                    Instr(Opcode.vmov, "*immediate - T5")),
                Mask(5, 1,  // op
                    Instr(Opcode.vmov, "*immediate - T4"),
                    invalid));

            var AdvancedSimdTwoRegistersAndShiftAmount = Mask(8, 0xF, // Opc
                Mask(6, 1, // Q
                    Instr(Opcode.vshr, VshImmSize, D22_12, D5_0, VshImm),
                    Instr(Opcode.vshr, VshImmSize, Q22_12, Q5_0, VshImm)),
                Mask(6, 1, // Q
                    Instr(Opcode.vsra, VshImmSize, D22_12, D5_0, VshImm),
                    Instr(Opcode.vsra, VshImmSize, Q22_12, Q5_0, VshImm)),
                Nyi("AdvancedSimdTwoRegistersAndShiftAmount_opc2"),
                Nyi("AdvancedSimdTwoRegistersAndShiftAmount_opc3"),

                Nyi("AdvancedSimdTwoRegistersAndShiftAmount_opc4"),
                Mask(12+16,1,   // U
                    Mask(6, 1, // Q
                        Instr(Opcode.vshl, VshImmSize, D22_12, D5_0, VshImm),
                        Instr(Opcode.vshl, VshImmSize, Q22_12, Q5_0, VshImm)),
                    Instr(Opcode.vsli, "*immediate")),
                Nyi("AdvancedSimdTwoRegistersAndShiftAmount_opc6"),
                Mask(6, 1, // Q
                    Instr(Opcode.vqshl, VshImmSize, D22_12, D5_0, VshImm),
                    Instr(Opcode.vqshl, VshImmSize, Q22_12_times2, Q5_0_times2, VshImm)),

                Mask(12+16,1,     // U
                    Mask(12+16,0b11,     // L:Q
                        Instr(Opcode.vshrn, "*AdvancedSimdTwoRegistersAndShiftAmount_opc8 U=0 L:Q=00"),
                        Nyi("AdvancedSimdTwoRegistersAndShiftAmount_opc8 U=0 L:Q=00"),
                        Nyi("AdvancedSimdTwoRegistersAndShiftAmount_opc8 U=0 L:Q=00"),
                        Nyi("AdvancedSimdTwoRegistersAndShiftAmount_opc8 U=0 L:Q=00")),
                    Nyi("AdvancedSimdTwoRegistersAndShiftAmount_opc8 U=1")),
                Mask(7, 1, // opc= 9 L
                    Mask(6, 1, //  L= 0 Q
                        Instr(Opcode.vqshrn, "*signed result variant"),
                        Instr(Opcode.vqrshrn, "D22:1:12:4,Q5:1:0:4,*signed result variant")),   //$TODO hairy encoding.
                    invalid),
                Nyi("AdvancedSimdTwoRegistersAndShiftAmount_opcA"),
                Nyi("AdvancedSimdTwoRegistersAndShiftAmount_opcB"),

                Nyi("AdvancedSimdTwoRegistersAndShiftAmount_opcC"),
                Nyi("AdvancedSimdTwoRegistersAndShiftAmount_opcD"),
                Mask(7, 1, // L
                    Mask(6, 1, // Q
                        Instr(Opcode.vcvt, "vC D22:1:12:4,D5:1:0:4,i16:6-64"),
                        Instr(Opcode.vcvt, "vC Q22:1:12:4,Q5:1:0:4,i16:6-64")),
                    invalid),
                Nyi("AdvancedSimdTwoRegistersAndShiftAmount_opcF"));


            var AdvancedSimdShiftImm = Select((19,3,7,1), n => n == 0,
                AdvancedSimdOneRegisterAndModifiedImmediate,
                AdvancedSimdTwoRegistersAndShiftAmount);

            var AdvancedSimdDataProcessing = Mask(7 + 16, 1,
                AdvancedSimd3RegistersSameLength,
                Mask(4, 1,
                    AdvancedSimd2RegsOr3RegsDiffLength,
                    AdvancedSimdShiftImm));

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
                    Instr(Opcode.qdadd, "R8,R0,R16"),
                    Instr(Opcode.qsub, "*"),
                    Instr(Opcode.qdsub, "R8,R0,R16")),
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

            var MovMovsRegisterShiftedRegister = Mask(20, 1,
                Mask(5 + 16, 3,
                    Instr(Opcode.lsl, "R8,R16,R0"),
                    Instr(Opcode.lsr, "R8,R16,R0"),
                    Instr(Opcode.asr, "R8,R16,R0"),
                    Instr(Opcode.ror, "R8,R16,R0")),
                Mask(5 + 16, 3,
                    Instr(Opcode.lsl, ".R8,R16,R0"),
                    Instr(Opcode.lsr, ".R8,R16,R0"),
                    Instr(Opcode.asr, ".R8,R16,R0"),
                    Instr(Opcode.ror, ".R8,R16,R0")));

            var DataProcessingRegister = Mask(7 + 16, 1,
                Mask(7, 1,
                    Select(w => SBitfield(w, 4, 4) == 0,
                        MovMovsRegisterShiftedRegister,
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
                        Instr(Opcode.smlawb, R8,R16,R0,R12),
                        Instr(Opcode.smulwb, R8,R16,R0)),
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
                        Instr(Opcode.smmla, R8,R16,R0,R12),
                        Instr(Opcode.smmul, R8, R16, R0)),
                    Select(w => SBitfield(w, 12, 4) != 0xF,
                        Instr(Opcode.smmlar, R8, R16, R0, R12),
                        Instr(Opcode.smmulr, R8, R16, R0)),
                    invalid,
                    invalid),
                Mask(4, 3,      // op1 = 0b110
                    Instr(Opcode.smmls, R8, R16, R0, R12),
                    Instr(Opcode.smmlsr, R8, R16, R0, R12),
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

            var DataProcessingShiftedRegister = Mask(21, 0xF,
                Mask(20, 1,
                    Instr(Opcode.and, "q R8,R16,R0,Si4:2;12:3:6:2"),
                    Nyi("DataProcessingShiftedRegister_opc0 s=1")),
                Mask(20, 1,
                    Instr(Opcode.bic, "q R8,R16,R0,Si4:2;12:3:6:2"),
                    Instr(Opcode.bic, ".q R8,R16,R0,Si4:2;12:3:6:2")),
                Mask(20, 1,
                    Select((16,4), n => n != 15,
                        Instr(Opcode.orr, "R8,R16,R0,Si4:2;12:3:6:2"),
                        Instr(Opcode.mov, "q R8,R0,Si4:2;12:3:6:2")),
                    Select((16,4), n => n != 15,
                        Instr(Opcode.orr, "*."),
                        Instr(Opcode.mov, "*."))),
                Mask(20, 1,
                    Select((16,4), n => n != 15,
                        Instr(Opcode.orn, "R8,R16,R0,Si4:2;12:3:6:2"),
                        Instr(Opcode.mvn, ".R8,R16,R0,Si4:2;12:3:6:2")),
                    Select((16,4), n => n != 15,
                        Instr(Opcode.orn, "*."),
                        Instr(Opcode.mvn, "*."))),

                Nyi("DataProcessingShiftedRegister_opc4"),
                invalid,
                Mask(20, 1,
                    Mask(4, 3,
                        Instr(Opcode.pkhbt, "*NYI"),
                        invalid,
                        Instr(Opcode.pkhtb, "*NYI"),
                        invalid),
                    invalid),
                invalid,

                Mask(20, 1,
                    Select((16, 4), n => n != 13,
                        Instr(Opcode.add, "q R8,R16,R0,Si4:2;12:3:6:2"),
                        Instr(Opcode.add, "S*")),
                    Select((8,4), n => n != 15,
                        Select((16,4), n => n != 13,
                            Instr(Opcode.add, ".*"),
                            Instr(Opcode.add, ".S*")),
                        Instr(Opcode.cmn, "*register"))),
                invalid,
                Nyi("DataProcessingShiftedRegister_opcA"),
                Mask(20, 1,
                    Instr(Opcode.sbc, "R8,R16,R0,Si4:2;12:3:6:2"),
                    Instr(Opcode.sbc, ".R8,R16,R0,Si4:2;12:3:6:2")),

                invalid,
                Mask(20, 1,
                    Nyi("DataProcessingShiftedRegister_opcD s=0"),
                    Nyi("DataProcessingShiftedRegister_opcD s=1")),
                Mask(20, 1,
                    Instr(Opcode.rsb, "R8,R16,R0,Si4:2;12:3:6:2"),
                    Instr(Opcode.rsb, ".R8,R16,R0,Si4:2;12:3:6:2")),
                invalid);

            return new LongDecoder(new Decoder[16]
            {
                invalid,
                invalid,
                invalid,
                invalid,

                Mask(6+16, 1,
                    LdStMultiple,
                    loadStoreMultipleTableBranch),
                DataProcessingShiftedRegister,
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

                Select((24,1,20,1), n => n != 2,
                    LoadStoreSingle,
                    AdvancedSimdElementOrStructureLdSt),
                Mask(7 + 16, 3,
                    DataProcessingRegister,
                    DataProcessingRegister,
                    MultiplyRegister,
                    LongMultiplyDivide),
                SystemRegisterAccessAdvSimdFpu,
                SystemRegisterAccessAdvSimdFpu
            });
        }

        private static MaskDecoder CreateLoadStoreDualMultipleBranchDecoder()
        {
            var ldrd = Instr(Opcode.ldrd, "R12,R8,[P,I0:8,d]");

            var LoadAcquireStoreRelease = Mask(20, 1,
                Mask(4, 7,
                    Instr(Opcode.stlb, "*"),
                    Instr(Opcode.stlh, "*"),
                    Instr(Opcode.stl, "*"),
                    invalid,

                    Instr(Opcode.stlexb, "R0,R12,[R16,b]"),
                    Instr(Opcode.stlexh, "R0,R12,[R16,h]"),
                    Instr(Opcode.stlex, "R0,R12,[R16,w]"),
                    Instr(Opcode.stlexd, "R0,R12,[R16,d]")),
                Mask(4, 7,
                    Instr(Opcode.ldab, "*"),
                    Instr(Opcode.ldah, "*"),
                    Instr(Opcode.lda, "*"),
                    invalid,

                    Instr(Opcode.ldaexb, "*"),
                    Instr(Opcode.ldaexh, "*"),
                    Instr(Opcode.ldaex, "R12,[R16,w]"),
                    Instr(Opcode.ldaexd, "*")));

            var ldStExclusive = Mask(20, 1,
                    Instr(Opcode.strex, "R8,R12,[R16,I0:8,w]"),
                    Instr(Opcode.ldrex, "R12,[R16,I0:8,w]"));

            var ldStDual = Mask(20, 1,
                Instr(Opcode.strd, "R12,R8,[R16,I0:8,dX]"),
                Instr(Opcode.ldrd, "R12,R8,[R16,I0:8,dX]"));
            var ldStDualImm = Mask(4 + 16, 1,
                Instr(Opcode.strd, "R12,R8,[R16,I0:8,dX]"),
                Instr(Opcode.ldrd, "R12,R8,[R16,I0:8,dX]"));
            var ldStDualPre = Mask(20, 1,
                Instr(Opcode.strd, "R12,R8,[R16,I0:8,dX]"),
                Instr(Opcode.ldrd, "R12,R8,[R16,I0:8,dX]"));

            return Mask(5 + 16, 0xF, "Load/store (multiple, dual, exclusive) table branch",
                invalid,
                invalid,
                ldStExclusive,
                Select((16,4), n => n != 15, ldStDual, ldrd),

                invalid,
                invalid,
                Mask(5, 7, // op0 = 0b0110, op3 
                    Mask(20, 1,
                        invalid,
                        Mask(4, 1,
                            Instr(Opcode.tbb, MemIdx(16,0, PrimitiveType.Byte)),
                            Instr(Opcode.tbh, MemIdx(16, 0, PrimitiveType.Word16)))),
                    invalid,
                    Nyi("load/store exclusive byte/half/dual"),
                    Nyi("load/store exclusive byte/half/dual"),

                    LoadAcquireStoreRelease,
                    LoadAcquireStoreRelease,
                    LoadAcquireStoreRelease,
                    LoadAcquireStoreRelease),
                Select((16,4), n => n != 15, ldStDual,ldrd),

                Select((16,4), n => n != 15, invalid, ldrd),
                Select((16,4), n => n != 15, invalid, ldrd),
                Select((16,4), n => n != 15, ldStDualImm, ldrd),
                Select((16,4), n => n != 15, ldStDualPre, ldrd),

                Select((16,4), n => n != 15, invalid, ldrd),
                Select((16,4), n => n != 15, invalid, ldrd),
                Select((16,4), n => n != 15, ldStDualImm, ldrd),
                Select((16,4), n => n != 15, ldStDualPre, ldrd));
        }

        private static Decoder CreateBranchesMiscControl()
        {
            var branch_T3_variant = Instr(Opcode.b, "p+26:1:11:1:13:1:16:6:0:11<1");
            var branch_T4_variant = Instr(Opcode.b, "p+26:1:13:1:11:1:16:10:0:11<1");
            var branch = Nyi("Branch");

            var MiscellaneousSystem = Mask(4, 0xF,
                invalid,
                invalid,
                Instr(Opcode.clrex, "*"),
                invalid,

                Instr(Opcode.dsb, "B0:4"),
                Instr(Opcode.dmb, "B0:4"),
                Instr(Opcode.isb, "B0:4"),
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid);

            var Hints = Mask(4, 0xF,
                Mask(0, 0xF,
                    Instr(Opcode.nop, "q"),
                    Instr(Opcode.yield, "*"),
                    Instr(Opcode.wfe, "*"),
                    Instr(Opcode.wfi, "*"),

                    Instr(Opcode.sev, "*"),
                    Instr(Opcode.sevl, "*"),
                    Instr(Opcode.nop, ""), // Reserved hint
                    Instr(Opcode.nop, ""), // Reserved hint

                    Instr(Opcode.nop, ""), // Reserved hint
                    Instr(Opcode.nop, ""), // Reserved hint
                    Instr(Opcode.nop, ""), // Reserved hint
                    Instr(Opcode.nop, ""), // Reserved hint

                    Instr(Opcode.nop, ""), // Reserved hint
                    Instr(Opcode.nop, ""), // Reserved hint
                    Instr(Opcode.nop, ""), // Reserved hint
                    Instr(Opcode.nop, "")), // Reserved hint
                Select((0, 4), n => n != 0, 
                    Instr(Opcode.nop, ""), // Reserved hint
                    Instr(Opcode.esb, "*")),
                Instr(Opcode.nop, ""), // Reserved hint
                Instr(Opcode.nop, ""), // Reserved hint

                Instr(Opcode.nop, ""), // Reserved hint
                Instr(Opcode.nop, ""), // Reserved hint
                Instr(Opcode.nop, ""), // Reserved hint
                Instr(Opcode.nop, ""), // Reserved hint

                Instr(Opcode.nop, ""), // Reserved hint
                Instr(Opcode.nop, ""), // Reserved hint
                Instr(Opcode.nop, ""), // Reserved hint
                Instr(Opcode.nop, ""), // Reserved hint

                Instr(Opcode.nop, ""), // Reserved hint
                Instr(Opcode.nop, ""), // Reserved hint
                Instr(Opcode.nop, ""), // Reserved hint
                Instr(Opcode.dbg, "*"));

            var mixedDecoders = Mask(6 + 16, 0xF,
                branch_T3_variant,
                branch_T3_variant,
                branch_T3_variant,
                branch_T3_variant,

                branch_T3_variant,
                branch_T3_variant,
                branch_T3_variant,
                branch_T3_variant,

                branch_T3_variant,
                branch_T3_variant,
                branch_T3_variant,
                branch_T3_variant,

                branch_T3_variant,
                branch_T3_variant,
                Mask(26, 1,     // op0
                    Mask(20, 3,     // op2
                        Mask(5, 1,  // op5
                            Mask(20, 1, // write spsr
                                Instr(Opcode.msr, "cpsr,R16"),
                                Instr(Opcode.msr, "spsr,R16")),
                            Instr(Opcode.msr, "*banked register")),
                        Mask(5, 1,  // op5
                            Instr(Opcode.msr, "*register"),
                            Instr(Opcode.msr, "*banked register")),
                        Select((8,3), n => n == 0,
                            Hints,
                            Nyi("ChangeProcessorState")),
                        MiscellaneousSystem),
                    Mask(20, 3,     // op2
                        Select((12,7), n => n == 0,
                            Nyi("Dcps"),
                            invalid),
                        invalid,
                        invalid,
                        invalid)),
                Mask(26, 1,         // op0
                    Mask(20, 3,     // op2
                        Instr(Opcode.bxj, "*"),
                        Nyi("ExceptionReturn"),
                        Mask(5, 1,  // op5
                            Mask(20, 1, // read spsr
                                Instr(Opcode.mrs, "R8,cpsr"),
                                Instr(Opcode.mrs, "R8,spsr")),
                            Instr(Opcode.mrs, "*banked register")),
                        Mask(5, 1,  // op5
                            Instr(Opcode.mrs, "*register"),
                            Instr(Opcode.mrs, "*banked register"))),
                    Mask(21, 1,
                        invalid,
                        Nyi("ExceptionGeneration"))));

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
