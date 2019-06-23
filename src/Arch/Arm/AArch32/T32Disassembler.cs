#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
using static Reko.Arch.Arm.AArch32.ArmVectorData;

namespace Reko.Arch.Arm.AArch32
{

    /// <summary>
    /// Disassembles machine code in the ARM T32 encoding into 
    /// ARM32 instructions.
    /// </summary>
    public partial class T32Disassembler : DisassemblerBase<AArch32Instruction>
    {
        private const uint ArmRegPC = 0xFu;

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
            instr.InstructionClass |= wInstr == 0 ? InstrClass.Zero : 0;
            instr.InstructionClass |= instr.condition != ArmCondition.AL ? InstrClass.Conditional : 0;
            instr.Address = addr;
            instr.Length = (int) (rdr.Address - addr);
            if ((itState & 0x1F) == 0x10)
            {
                // No more IT bits, reset condition back to normal.
                itCondition = ArmCondition.AL;
                itState = 0;
            }
            else if (itState != 0 && instr.opcode != Opcode.it)
            {
                // We're still under the influence of the IT instruction.
                var bit = ((itState >> 4) ^ ((int) this.itCondition)) & 1;
                instr.condition = (ArmCondition) ((int) this.itCondition ^ bit);
                itState <<= 1;
            }
            return instr;
        }

        private class DasmState
        {
            public Opcode opcode;
            public InstrClass iclass;
            public List<MachineOperand> ops = new List<MachineOperand>();
            public ArmCondition cc = ArmCondition.AL;
            public bool updateFlags = false;
            public bool wide = false;
            public bool writeback = false;
            public Opcode shiftType = Opcode.Invalid;
            public MachineOperand shiftValue = null;
            public ArmVectorData vectorData = ArmVectorData.INVALID;
            public bool useQ = false;

            internal void Invalid()
            {
                throw new NotImplementedException();
            }

            public AArch32Instruction MakeInstruction()
            {
                return new T32Instruction
                {
                    opcode = opcode,
                    InstructionClass = iclass,
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

        private AArch32Instruction DecodeFormat(uint wInstr, Opcode opcode, InstrClass iclass, string format)
        {
            this.state.opcode = opcode;
            this.state.iclass = iclass;
            for (int i = 0; i < format.Length; ++i)
            {
                int offset;
                int size;
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
                        op = ImmediateOperand.Int32((int) n);
                    }
                    break;
                case 'S':   // shift amount in bitfield.
                    ++i;
                    if (PeekAndDiscard('r', format, ref i))
                    {
                        // 'Sr' = rotate
                        uint n = ReadBitfields(wInstr, format, ref i);
                        state.shiftType = Opcode.ror;
                        state.shiftValue = ImmediateOperand.Int32((int) n);
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
                            op = ImmediateOperand.Word16((ushort) n);
                        }
                        else if (PeekAndDiscard('-', format, ref i))
                        {
                            var minuend = ReadDecimal(format, ref i);
                            op = ImmediateOperand.Word32(minuend - (int) n);
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
                case 'm':
                    ++i;
                    uint regmask = wInstr & 0xFF;
                    if (PeekAndDiscard('w', format, ref i))
                    {
                        // 'mw': 16-bit instruction register mask used by push
                        regmask |= (wInstr & 0x100) << 6;
                    }
                    else
                    {
                        // 'mr': 16-bit instruction register mask used by pop
                        Expect('r', format, ref i);
                        regmask |= (wInstr & 0x100) << 7;
                    }
                    op = new MultiRegisterOperand(Registers.GpRegs, PrimitiveType.Word16, regmask);
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
                        ((int) wInstr >> offset) & 0x0F]);
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
                case 'p':   // PC-relative offset, 
                    ++i;
                    offset = (int) ReadBitfields(wInstr, format, ref i);
                    op = AddressOperand.Create(addr + offset + 4);
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
                        state.cc = (ArmCondition) SBitfield(wInstr, offset, 4);
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

            return new T32Instruction
            {
                opcode = state.opcode,
                InstructionClass = state.iclass,
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

        private (Opcode, MachineOperand) DecodeImmShift(uint wInstr, Bitfield bfType, Bitfield[] bfImm)
        {
            var type = bfType.Read(wInstr);
            var imm = Bitfield.ReadFields(bfImm, wInstr);
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
                return (((ulong) value) << 32) | value;
            }

            ulong Replicate4(uint value)
            {
                var v = (ulong) (ushort) value;
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

        private static MachineOperand MakeBarrierOperand(uint n)
        {
            var bo = (BarrierOption) n;
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

        private T32Instruction Invalid()
        {
            return new T32Instruction
            {
                opcode = Opcode.Invalid,
                ops = new MachineOperand[0]
            };
        }

        private AArch32Instruction NotYetImplemented(string message, uint wInstr)
        {
            string instrHexBytes;
            if (wInstr > 0xFFFF)
            {
                instrHexBytes = $"{wInstr >> 16:X4}{ wInstr & 0xFFFF:X4}";
            }
            else
            {
                instrHexBytes = $"{wInstr:X4}";
            }
            var rev = $"{Bits.Reverse(wInstr):X8}";
            message = (string.IsNullOrEmpty(message))
                ? rev
                : $"{rev} - {message}";
            base.EmitUnitTest("T32", instrHexBytes, message, "ThumbDis", this.addr, Console =>
            {
                if (wInstr > 0xFFFF)
                {
                    Console.WriteLine($"    Given_Instructions(0x{wInstr >> 16:X4}, 0x{wInstr & 0xFFFF:X4});");
                }
                else
                {
                    Console.WriteLine($"    Given_Instructions(0x{wInstr:X4});");
                }
                Console.WriteLine("    Expect_Code(\"@@@\");");
            });
            return Invalid();
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
                n = (uint) Bits.SignExtend(n, bits);
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
            return ((int) word >> offset) & ((1 << size) - 1);
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

        private static void Expect(char c, string format, ref int i)
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
            default: throw new InvalidOperationException($"{format[i - 1]}");
            }
        }

        private static Decoder DecodeBfcBfi(Opcode opcode, params Mutator<T32Disassembler>[] mutators)
        {
            return new BfcBfiDecoder(opcode, mutators);
        }

        #region Mutators

        /// <summary>
        /// If present sets the updateflags bit of the instruction.
        /// </summary>
        private static bool uf(uint u, T32Disassembler dasm)
        {
            dasm.state.updateFlags = true;
            return true;
        }

        /// <summary>
        /// This instructions sets the flags if it's outside an IT block.
        /// </summary>
        private static bool ufit(uint u, T32Disassembler dasm)
        {
            dasm.state.updateFlags = dasm.itCondition == ArmCondition.AL;
            return true;
        }

        private static Bitfield[] Bf(params (int pos, int len)[] fields)
        {
            return fields.Select(f => new Bitfield(f.pos, f.len)).ToArray();
        }

        /// <summary>
        /// This is the wide form of an ARM Thumb instruction.
        /// </summary>
        private static bool wide(uint wInstr, T32Disassembler dasm)
        {
            dasm.state.wide = true;
            return true;
        }
    
        /// <summary>
        /// Conditional instruction encoded at bit position <paramref name="bitPos" />
        /// </summary>
        private static Mutator<T32Disassembler> ConditionCode(int bitPos)
        {
            var field = new Bitfield(bitPos, 4);
            return (u, d) =>
            {
                d.state.cc = (ArmCondition) field.Read(u);
                return true;
            };
        }
        private static readonly Mutator<T32Disassembler> c8 = ConditionCode(8);

        private static Mutator<T32Disassembler> q(int bitPos)
        {
            return (u, d) =>
            {
                d.state.useQ = Bits.IsBitSet(u, bitPos);
                return true;
            };
        }

        /// <summary>
        /// Writeback bit.
        /// </summary>
        private static Mutator<T32Disassembler> w(int bitPos)
        {
            return (u, d) =>
            {
                d.state.writeback = Bits.IsBitSet(u, bitPos);
                return true;
            };
        }
        private static readonly Mutator<T32Disassembler> w21 = w(21);

        private static Bitfield[] vifFields = {
            new Bitfield(10,1), new Bitfield(18, 2)
        };
        private static bool vif(uint uInstr, T32Disassembler dasm)
        {
            var code = Bitfield.ReadFields(vifFields, uInstr);
            switch (code)
            {
            case 0b0_00: dasm.state.vectorData = ArmVectorData.S8; return true;
            case 0b0_01: dasm.state.vectorData = ArmVectorData.S16; return true;
            case 0b0_10: dasm.state.vectorData = ArmVectorData.S32; return true;
            case 0b1_01: dasm.state.vectorData = ArmVectorData.F16; return true;
            case 0b1_10: dasm.state.vectorData = ArmVectorData.F32; return true;
            }
            return false;
        }

        private static Mutator<T32Disassembler> vi(int bitpos)
        {
            var field = new Bitfield(bitpos, 2);
            var sizes = new ArmVectorData[] { I8, I16, I32, INVALID };
            return (u, d) =>
            {
                d.state.vectorData = sizes[field.Read(u)];
                return true;
            };
        }

        // signed or unsigned integer
        private static Mutator<T32Disassembler> vu(int bitpos)
        {
            var field = new Bitfield(bitpos, 2);
            return (u, d) =>
            {
                uint nn = field.Read(u);
                d.state.vectorData = d.VectorIntUIntData(u, nn);
                return true;
            };
        }

        private static Mutator<T32Disassembler> vr(int bitpos)
        {
            var field = new Bitfield(bitpos, 2);
            return (u, d) =>
            {
                uint nn = field.Read(u);
                throw new NotImplementedException();
                //d.state.vectorData = d.VectorIntUIntData(u, nn);
                //return true;
            };
        }

        // conversion 
        private static bool vc(uint wInstr, T32Disassembler dasm)
        {
            dasm.state.vectorData = dasm.VectorConvertData(wInstr);
            return true;
        }

        // conversion2 
        private static bool vC(uint wInstr, T32Disassembler dasm)
        {
            dasm.state.vectorData = dasm.VectorConvertData2(wInstr);
            return true;
        }

        // floating point elements specified by a bitfield
        private static Mutator<T32Disassembler> vF(int bitPos)
        {
            var bf = new Bitfield(bitPos, 2);
            return (u, d) =>
            {
                uint n = bf.Read(u);
                d.state.vectorData = d.VectorFloatElementData(n);
                return d.state.vectorData != ArmVectorData.INVALID;
            };
        }

        private static ArmVectorData[] _hw_ = new[]
        {
            INVALID,
            I16,
            I32,
            INVALID
        };

        private static Mutator<T32Disassembler> v_hw_(int bitPos)
        {
            var bf = new Bitfield(bitPos, 2);
            return (u, d) =>
            {
                uint n = bf.Read(u);
                d.state.vectorData = _hw_[n];
                return d.state.vectorData != INVALID;
            };
        }



        /// <summary>
        /// Register bitfield
        /// </summary>
        private static Mutator<T32Disassembler> R(int bitOffset)
        {
            var field = new Bitfield(bitOffset, 4);
            return (u, d) =>
            {
                var iReg = field.Read(u);
                d.state.ops.Add(new RegisterOperand(Registers.GpRegs[iReg]));
                return true;
            };
        }
        private static readonly Mutator<T32Disassembler> R0 = R(0);
        private static readonly Mutator<T32Disassembler> R3 = R(3);
        private static readonly Mutator<T32Disassembler> R8 = R(8);
        private static readonly Mutator<T32Disassembler> R9 = R(9);
        private static readonly Mutator<T32Disassembler> R12 = R(12);
        private static readonly Mutator<T32Disassembler> R16 = R(16);

        /// <summary>
        /// Register bitfield, but don't allow PC
        /// </summary>
        /// 
        private static Mutator<T32Disassembler> Rnp(int bitOffset)
        {
            var field = new Bitfield(bitOffset, 4);
            return (u, d) =>
            {
                var iReg = field.Read(u);
                if (iReg == ArmRegPC)
                    return false;
                d.state.ops.Add(new RegisterOperand(Registers.GpRegs[iReg]));
                return true;
            };
        }

        private static readonly Mutator<T32Disassembler> Rnp0 = Rnp(0);
        private static readonly Mutator<T32Disassembler> Rnp8 = Rnp(8);
        private static readonly Mutator<T32Disassembler> Rnp12 = Rnp(12);
        private static readonly Mutator<T32Disassembler> Rnp16 = Rnp(16);


        /// <summary>
        /// GP register specified by 3 bits (r0..r7)
        /// </summary>
        private static Mutator<T32Disassembler> r(int bitpos)
        {
            var field = new Bitfield(bitpos, 3);
            return (u, d) =>
            {
                var iReg = field.Read(u);
                d.state.ops.Add(new RegisterOperand(Registers.GpRegs[iReg]));
                return true;
            };
        }
        private static readonly Mutator<T32Disassembler> r0 = r(0);
        private static readonly Mutator<T32Disassembler> r3 = r(3);
        private static readonly Mutator<T32Disassembler> r6 = r(6);
        private static readonly Mutator<T32Disassembler> r8 = r(8);

        /// <summary>
        /// GP register, specified by bits 7 || 2..0
        /// </summary>
        private static bool T(uint wInstr, T32Disassembler dasm)
        {
            var tReg = ((wInstr & 0x80) >> 4) | (wInstr & 7);
            dasm.state.ops.Add(new RegisterOperand(Registers.GpRegs[tReg]));
            return true;
        }

        private static Mutator<T32Disassembler> Reg(RegisterStorage reg)
        {
            return (u, d) =>
            {
                d.state.ops.Add(new RegisterOperand(reg));
                return true;
            };
        }
        private static Mutator<T32Disassembler> sp = Reg(Registers.sp);
        private static Mutator<T32Disassembler> cpsr = Reg(Registers.cpsr);
        private static Mutator<T32Disassembler> spsr = Reg(Registers.spsr);


        // Multiple regs

        // 'mw': 16-bit instruction register mask used by push
        private static bool mw(uint wInstr, T32Disassembler dasm)
        {

            uint regmask = wInstr & 0xFF;
            regmask |= (wInstr & 0x100) << 6;
            dasm.state.ops.Add(new MultiRegisterOperand(Registers.GpRegs, PrimitiveType.Word16, regmask));
            return true;
        }

        // 'mr': 16-bit instruction register mask used by pop
        private static bool mr(uint wInstr, T32Disassembler dasm)
        {
            uint regmask = wInstr & 0xFF;
            regmask |= (wInstr & 0x100) << 7;
            dasm.state.ops.Add(new MultiRegisterOperand(Registers.GpRegs, PrimitiveType.Word16, regmask));
            return true;
        }




        private static bool F12_22(uint wInstr, T32Disassembler dasm)
        {
            var d = ((wInstr >> 11) & 0x1E) | ((wInstr >> 22) & 1);
            dasm.state.ops.Add(new RegisterOperand(Registers.SRegs[d]));
            return true;
        }

        private static bool D22_12(uint wInstr, T32Disassembler dasm)
        {
            var d = ((wInstr >> 18) & 0x10) | ((wInstr >> 12) & 0xF);
            dasm.state.ops.Add(new RegisterOperand(Registers.DRegs[d]));
            return true;
        }

        private static bool Q22_12(uint wInstr, T32Disassembler dasm)
        {
            var q = ((wInstr >> 18) & 0x10) | ((wInstr >> 12) & 0xF);
            dasm.state.ops.Add(new RegisterOperand(Registers.QRegs[q >> 1]));
            return true;
        }

        /// <summary>
        /// Vector register (depends on useQ being set)
        /// </summary>
        private static bool W22_12(uint wInstr, T32Disassembler dasm)
        {
            uint iReg = ((wInstr >> 18) & 0x10) | ((wInstr >> 12) & 0xF);
            if (dasm.state.useQ && (iReg & 1) == 1)
                return false;
            var reg = (dasm.state.useQ ? Registers.QRegs : Registers.DRegs)[iReg];
            dasm.state.ops.Add(new RegisterOperand(reg));
            return true;
        }

        private static bool Q22_12_times2(uint wInstr, T32Disassembler dasm)
        {
            var q = ((wInstr >> 18) & 0x10) | ((wInstr >> 12) & 0xF);
            dasm.state.ops.Add(new RegisterOperand(Registers.QRegs[q >> 1]));
            return true;
        }


        private static bool F16_7(uint wInstr, T32Disassembler dasm)
        {
            var s = ((wInstr >> 15) & 0x1E) | ((wInstr >> 7) & 0x1);
            dasm.state.ops.Add(new RegisterOperand(Registers.SRegs[s]));
            return true;
        }

        private static bool D7_16(uint wInstr, T32Disassembler dasm)
        {
            var d = ((wInstr >> 3) & 0x10) | ((wInstr >> 16) & 0xF);
            dasm.state.ops.Add(new RegisterOperand(Registers.DRegs[d]));
            return true;
        }

        private static bool Q7_16(uint wInstr, T32Disassembler dasm)
        {
            var q = ((wInstr >> 3) & 0x10) | ((wInstr >> 16) & 0xF);
            dasm.state.ops.Add(new RegisterOperand(Registers.QRegs[q >> 1]));
            return true;
        }


        private static bool F0_5(uint wInstr, T32Disassembler dasm)
        {
            var s = ((wInstr & 0xF) << 1) | ((wInstr >> 0x5) & 1);
            dasm.state.ops.Add(new RegisterOperand(Registers.SRegs[s]));
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
            dasm.state.ops.Add(new RegisterOperand(Registers.QRegs[q >> 1]));
            return true;
        }

        /// <summary>
        /// Vector register (depends on useQ being set)
        /// </summary>
        private static bool W5_0(uint wInstr, T32Disassembler dasm)
        {
            uint iReg = ((wInstr >> 1) & 0x10) | (wInstr & 0xF);
            if (dasm.state.useQ && (iReg & 1) == 1)
                return false;
            var reg = (dasm.state.useQ ? Registers.QRegs : Registers.DRegs)[iReg];
            dasm.state.ops.Add(new RegisterOperand(reg));
            return true;
        }

        private static bool Q5_0_times2(uint wInstr, T32Disassembler dasm)
        {
            var q = ((wInstr >> 1) & 0x10) | (wInstr & 0xF);
            dasm.state.ops.Add(new RegisterOperand(Registers.QRegs[q >> 1]));
            return true;
        }

        // Coprocessor registers

        private static Mutator<T32Disassembler> CP(int n)
        {
            return (u, d) =>
            {
                //if (PeekAndDiscard('#', format, ref i))   // Literal
                //{
                //    offset = ReadDecimal(format, ref i);
                //    var cp = Registers.Coprocessors[offset];
                //    op = new RegisterOperand(cp);
                //}
                //else
                var op = d.Coprocessor(u, n);
                d.state.ops.Add(op);
                return true;
            };
        }
        private static readonly Mutator<T32Disassembler> CP8 = CP(8);

        private static Mutator<T32Disassembler> CPn(int n)
        {
            return (u, d) =>
            {
                var cp = Registers.Coprocessors[n];
                var op = new RegisterOperand(cp);
                d.state.ops.Add(op);
                return true;
            };
        }
        private static readonly Mutator<T32Disassembler> CPn14 = CPn(14);


        private static Mutator<T32Disassembler> CR(int n)
        {
            return (u, d) =>
            {
                var op = d.CoprocessorRegister(u, n);
                d.state.ops.Add(op);
                return true;
            };
        }
        private static readonly Mutator<T32Disassembler> CR0 = CR(0);
        private static readonly Mutator<T32Disassembler> CR12 = CR(12);
        private static readonly Mutator<T32Disassembler> CR16 = CR(16);


        // Immediate mutators

        private static Mutator<T32Disassembler> Imm(int pos, int length)
        {
            var bitfield = new Bitfield(pos, length);
            return (u, d) =>
            {
                var imm = bitfield.Read(u);
                d.state.ops.Add(ImmediateOperand.Word32(imm));
                return true;
            };
        }

        private static Mutator<T32Disassembler> Simm(int pos, int length, int shift = 0)
        {
            var bitfield = new Bitfield(pos, length);
            return (u, d) =>
            {
                var imm = bitfield.ReadSigned(u) << shift;
                d.state.ops.Add(ImmediateOperand.Int32(imm));
                return true;
            };
        }

        private static Mutator<T32Disassembler> Imm(int pos1, int length1, int pos2, int length2)
        {
            var bitfields = new[]
            {
                new Bitfield(pos1, length1),
                new Bitfield(pos2, length2),
            };
            return (u, d) =>
            {
                var imm = Bitfield.ReadFields(bitfields, u);
                d.state.ops.Add(ImmediateOperand.Word32(imm));
                return true;
            };
        }

        private static Mutator<T32Disassembler> Imm(PrimitiveType dt = null, uint minuend = 0, params Bitfield[] fields)
        {
            var dataType = dt ?? PrimitiveType.Word32;
            return (u, d) =>
            {
                var imm = Bitfield.ReadFields(fields, u);
                if (minuend != 0)
                {
                    imm = minuend - imm;
                }
                d.state.ops.Add(new ImmediateOperand(Constant.Create(dataType, imm)));
                return true;
            };
        }
        private static readonly Mutator<T32Disassembler> Imm26_12_0 = Imm(fields: Bf((26, 1), (12, 3), (0, 8)));

        private static Mutator<T32Disassembler> ImmM1(int pos, int length)
        {
            var bitfield = new Bitfield(pos, length);
            return (u, d) =>
            {
                var n = bitfield.Read(u);
                d.state.ops.Add(ImmediateOperand.Word32(n + 1));
                return true;
            };
        }

        private static Mutator<T32Disassembler> S(int pos, int len)
        {
            var bf = new Bitfield(pos, len);
            return (u, d) =>
            {
                d.state.ops.Add(ImmediateOperand.Int32((int)bf.Read(u)));
                return true;
            };
        }

        // 'Si' = shift immediate
        private static Mutator<T32Disassembler> Si((int pos, int len) bfType, Bitfield[] bfCount)
        {
            var fType = new Bitfield(bfType.pos, bfType.len);
            return (u, d) =>
            {
                (d.state.shiftType, d.state.shiftValue) = d.DecodeImmShift(u, fType, bfCount);
                return true;
            };
        }

        // Sr = rotate
        private static Mutator<T32Disassembler> SrBy8(int pos, int len)
        {
            var field = new Bitfield(pos, len);
            return (u, d) =>
            {
                int n = (int) field.Read(u);
                d.state.shiftType = n != 0 ? Opcode.ror : Opcode.Invalid;
                d.state.shiftValue = ImmediateOperand.Int32(n * 8);
                return true;
            };
        }
        private static readonly Mutator<T32Disassembler> SrBy8_4_2 = SrBy8(4, 2);

        private static Mutator<T32Disassembler> LslImm(int pos1, int length1, int pos2, int length2)
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
                    d.state.shiftValue = ImmediateOperand.Int32((int) imm);
                }
                return true;
            };
        }

        private static readonly Bitfield[] modifiedImmediateFields = new[]
        {
            new Bitfield(10 + 16, 1),
            new Bitfield(12, 3),
            new Bitfield(7, 1)
        };
        private static bool M(uint wInstr, T32Disassembler dasm)
        {
            var i_imm3_a = Bitfield.ReadFields(modifiedImmediateFields, wInstr);
            var abcdefgh = wInstr & 0xFF;
            MachineOperand op;
            switch (i_imm3_a)
            {
            case 0:
            case 1:
                op = ImmediateOperand.Word32(abcdefgh);
                break;
            case 2:
            case 3:
                op = ImmediateOperand.Word32((abcdefgh << 16) | abcdefgh);
                break;
            case 4:
            case 5:
                op = ImmediateOperand.Word32((abcdefgh << 24) | (abcdefgh << 8));
                break;
            case 6:
            case 7:
                op = ImmediateOperand.Word32(
                    (abcdefgh << 24) |
                    (abcdefgh << 16) |
                    (abcdefgh << 8) |
                    (abcdefgh));
                break;
            default:
                abcdefgh |= 0x80;
                op = ImmediateOperand.Word32(abcdefgh << (int) (0x20 - i_imm3_a));
                break;
            }
            dasm.state.ops.Add(op);
            return true;
        }

        private static Mutator<T32Disassembler> MS(params Bitfield[] fields)
        {
            return (u, d) =>
            {
                var n = Bitfield.ReadFields(fields, u);
                d.state.ops.Add(d.ModifiedSimdImmediate(u, n));
                return true;
            };
        }
        private static readonly Mutator<T32Disassembler> MS_28_16_0 = MS(Bf((28, 1), (16, 3), (0, 4)));


        /// <summary>
        /// Modified SIMD immediate
        /// </summary>
        private static Mutator<T32Disassembler> Is(int pos1, int size1, int pos2, int size2, int pos3, int size3)
        {
            var fields = new[]
            {
                new Bitfield(pos1, size1),
                new Bitfield(pos2, size2),
                new Bitfield(pos3, size3),
            };
            var op0size = new[,]
            {
                {
                 ArmVectorData.I32, ArmVectorData.I32, ArmVectorData.I32, ArmVectorData.I32,
                 ArmVectorData.I32, ArmVectorData.I32, ArmVectorData.I32, ArmVectorData.I32,
                 ArmVectorData.I16, ArmVectorData.I16, ArmVectorData.I16, ArmVectorData.I16,
                 ArmVectorData.I32, ArmVectorData.I32, ArmVectorData.I8, ArmVectorData.F32,
                },
            {
                 ArmVectorData.I32, ArmVectorData.I32, ArmVectorData.I32, ArmVectorData.I32,
                 ArmVectorData.I32, ArmVectorData.I32, ArmVectorData.I32, ArmVectorData.I32,
                 ArmVectorData.I16, ArmVectorData.I16, ArmVectorData.I16, ArmVectorData.I16,
                 ArmVectorData.I32, ArmVectorData.I32, ArmVectorData.I64, ArmVectorData.INVALID,
            } };
            return (u, d) =>
            {
                var imm = Bitfield.ReadFields(fields, u);
                var cmode = (u >> 8) & 0xF;
                var op = (u >> 5) & 1;
                d.state.vectorData = op0size[op, cmode];
                d.state.ops.Add(ImmediateOperand.Word64(A32Disassembler.SimdExpandImm(op, cmode, (uint) imm)));
                return true;
            };
        }

        /// <summary>
        /// PC-relative offset, aligned by 4 bytes
        /// </summary>
        private static Mutator<T32Disassembler> P(int bitOffset, int length)
        {
            var field = new Bitfield(bitOffset, length);
            return (u, d) =>
            {
                var offset = field.ReadSigned(u) << 2;
                var op = AddressOperand.Create(d.addr.Align(4) + offset);
                d.state.ops.Add(op);
                return true;
            };
        }

        private static Mutator<T32Disassembler> PcRelative(int shift = 0, params Bitfield[] fields)
        {
            return (u, d) =>
            {
                var offset = Bitfield.ReadSignedFields(fields, u) << shift;
                var op = AddressOperand.Create(d.addr + (offset + 4));
                d.state.ops.Add(op);
                return true;
            };
        }

        // Jump displacement in bits 9:3..7, shifted left by 1.
        private static bool x(uint wInstr, T32Disassembler dasm)
        {
            var offset = (SBitfield(wInstr, 9, 1) << 6) |
                         (SBitfield(wInstr, 3, 5) << 1);
            dasm.state.ops.Add(AddressOperand.Create(dasm.addr + (offset + 4)));
            return true;
        }

        /// <summary>
        /// Vector immediate quantity.
        /// </summary>
        private static bool IW0(uint uInstr, T32Disassembler dasm)
        {
            dasm.state.ops.Add(dasm.state.useQ
                ? ImmediateOperand.Word128(0)
                : ImmediateOperand.Word64(0));
            return true;
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
            dasm.state.ops.Add(ImmediateOperand.Int32((int) imm));
            return true;
        }

        // Memory access mutators

        private static Mutator<T32Disassembler> MemOff(
            PrimitiveType dt,
            int baseRegBitoffset = 0,
            RegisterStorage baseReg = null,
            int offsetShift = 0,
            IndexSpec indexSpec = null,
            params (int bitOffset, int length)[] offsetFields)
        {
            var brf = new Bitfield(baseRegBitoffset, 4);
            var bfs = offsetFields.Select(f => new Bitfield(f.bitOffset, f.length)).ToArray();
            return (u, d) =>
            {
                var b = baseReg ?? Registers.GpRegs[brf.Read(u)];
                var offset = bfs.Length > 0
                    ? (int) Bitfield.ReadFields(bfs, u)
                    : 0;
                bool preIndex = false;
                bool add = true;
                if (indexSpec != null)
                {
                    preIndex = indexSpec.preIndex.Read(u) != 0;
                    add = indexSpec.add.Read(u) != 0;
                    d.state.writeback = indexSpec.writeback.Read(u) != 0;
                }

                var mem = new MemoryOperand(dt)
                {
                    BaseRegister = b,
                    Offset = Constant.Int32(offset << offsetShift),
                    Add = add,
                    PreIndex = preIndex,
                };
                d.state.ops.Add(mem);
                return true;
            };
        }

        /// <summary>
        /// Indexed addressing using 3-bit fields for registers
        /// </summary>
        private static Mutator<T32Disassembler> MemOff_r(
            PrimitiveType dt,
            int baseRegBitoffset = 0,
            RegisterStorage baseReg = null,
            int shift = 0,
            params (int bitOffset, int length)[] fields)
        {
            var brf = new Bitfield(baseRegBitoffset, 3);
            var bfs = fields.Select(f => new Bitfield(f.bitOffset, f.length)).ToArray();
            return (u, d) =>
            {
                var b = baseReg ?? Registers.GpRegs[brf.Read(u)];
                var offset = Bitfield.ReadFields(bfs, u);
                var mem = new MemoryOperand(dt)
                {
                    BaseRegister = b,
                    Offset = Constant.Int32((int)offset << shift),
                    Add = true,
                };
                d.state.ops.Add(mem);
                return true;
            };
        }

        private static Mutator<T32Disassembler> MemIdx(PrimitiveType dt, int posBaseReg, int posIdxReg, (int,int)? sh = null)
        {
            Bitfield? field = sh.HasValue
                ? new Bitfield(sh.Value.Item1, sh.Value.Item2)
                : (Bitfield?)null;

            return (u, d) =>
            {
                var baseReg = Registers.GpRegs[(u >> posBaseReg) & 0xF];
                var idxReg = Registers.GpRegs[(u >> posIdxReg) & 0xF];

                int shiftAmt = 0;
                Opcode shiftType = Opcode.Invalid;
                if (field.HasValue)
                {
                    shiftAmt = (int)field.Value.Read(u);
                    shiftType = shiftAmt != 0 ? Opcode.lsl : Opcode.Invalid;
                }
                var mem = new MemoryOperand(dt)
                {
                    BaseRegister = baseReg,
                    Index = idxReg,
                    ShiftType = shiftType,
                    Shift = shiftAmt,
                    Add = true,
                };
                d.state.ops.Add(mem);
                return true;
            };
        }

        /// <summary>
        /// Indexed addressing using 3-bit fields for registers
        /// </summary>
        private static Mutator<T32Disassembler> MemIdx_r(PrimitiveType dt, int posBaseReg, int posIdxReg)
        {
            return (u, d) =>
            {
                var baseReg = Registers.GpRegs[(u >> posBaseReg) & 0x7];
                var idxReg = Registers.GpRegs[(u >> posIdxReg) & 0x7];
                var mem = new MemoryOperand(dt)
                {
                    BaseRegister = baseReg,
                    Index = idxReg,
                    Add = true
                };
                d.state.ops.Add(mem);
                return true;
            };
        }

        private class IndexSpec
        {
            public Bitfield preIndex;
            public Bitfield add;
            public Bitfield writeback;
        }

        // Indexing bits in P=10, W=8
        // Negative bit in U=9
        private static readonly IndexSpec idx10 = new IndexSpec
        {
            preIndex = new Bitfield(10, 1),
            add = new Bitfield(9, 1),
            writeback = new Bitfield(8, 1)
        };

        private static readonly IndexSpec idx24 = new IndexSpec
        {
            preIndex = new Bitfield(24, 1),
            add = new Bitfield(23, 1),
            writeback = new Bitfield(21, 1)
        };

        // Branch targets

        private static Bitfield[] B_T4_fields = new Bitfield[]
        {
            new Bitfield(26, 1),
            new Bitfield(13, 1),
            new Bitfield(11, 1),
            new Bitfield(16, 10),
            new Bitfield(0, 11)
        };
        private static bool B_T4(uint wInstr, T32Disassembler dasm)
        {
            // The T4 encoding of the 'b' instruction is incredibly
            // hairy....
            var mask = 5u << 11;
            var ssss = Bits.SignExtend(wInstr >> 26, 1) & mask;
            wInstr = (wInstr & ~mask) | (~(wInstr ^ ssss) & mask);
            int offset = Bitfield.ReadSignedFields(B_T4_fields, wInstr) << 1;
            var op = AddressOperand.Create(dasm.addr + (offset + 4));
            dasm.state.ops.Add(op);
            return true;
        }

        // Miscellaneous

        private static Mutator<T32Disassembler> B(int pos)
        {
            var field = new Bitfield(pos, 4);
            return (u, d) =>
            {
                uint n = field.Read(u);
                d.state.ops.Add(MakeBarrierOperand(n));
                return true;
            };
        }
        private static readonly Mutator<T32Disassembler> B0_4 = B(0);

        private static Mutator<T32Disassembler> nyi(string message)
        {
            return (u, d) =>
            {
                d.NotYetImplemented($"Unimplemented format specifier '{message}' when decoding {u:X4}", u);
                return false;
            };
        }
        #endregion


        // Factory methods

        private static InstrDecoder Instr(Opcode opcode, params Mutator<T32Disassembler>[] mutators)
        {
            return new InstrDecoder(opcode, InstrClass.Linear, ArmVectorData.INVALID, mutators);
        }

        private static InstrDecoder Instr(Opcode opcode, InstrClass iclass, params Mutator<T32Disassembler>[] mutators)
        {
            return new InstrDecoder(opcode, iclass, ArmVectorData.INVALID, mutators);
        }


        private static InstrDecoder Instr(Opcode opcode, ArmVectorData vec, params Mutator<T32Disassembler>[] mutators)
        {
            return new InstrDecoder(opcode, InstrClass.Linear, vec, mutators);
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

        private static SelectFieldDecoder Select(Bitfield[] fields, Func<uint, bool> predicate, Decoder decoderTrue, Decoder decoderFalse)
        {
            return new SelectFieldDecoder(fields, predicate, decoderTrue, decoderFalse);
        }

        private static NyiDecoder Nyi(string msg)
        {
            return new NyiDecoder(msg);
        }


        static T32Disassembler()
        {
            invalid = Instr(Opcode.Invalid);

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
                    Instr(Opcode.b, PcRelative(1, Bf((0, 11)))),
                    dec32bit,
                    dec32bit,
                    dec32bit)
            };
        }

        private static MaskDecoder Create16bitDecoders()
        {
            var AddSpRegisterT1 = Instr(Opcode.add, uf,T,sp);
            var AddSpRegisterT2 = Instr(Opcode.add, sp,T);
            var decAlu = CreateAluDecoder();
            var decDataLowRegisters = CreateDataLowRegisters();
            var decDataHiRegisters = Mask(8, 0x03, // Add, subtract, compare, move (two high registers)
                Select(Bf((7,1),(0,3)), n => n != 13, 
                    Select((3,4), n => n != 13,
                        Instr(Opcode.add, uf,T,R3),
                        AddSpRegisterT1),
                    Select((3,4), n => n != 13,
                        AddSpRegisterT2, 
                        AddSpRegisterT1)),
                Instr(Opcode.cmp, uf,T,R3),
                Instr(Opcode.mov, T,R3), // mov,movs
                invalid);

            var LdrLiteral = Instr(Opcode.ldr,r8,MemOff(PrimitiveType.Word32, baseReg:Registers.pc, offsetShift:2, offsetFields:(0,8)));

            var LdStRegOffset = Mask(9, 7,
                Instr(Opcode.str, r0,MemIdx_r(PrimitiveType.Word32,3,6)),
                Instr(Opcode.strh, r0, MemIdx_r(PrimitiveType.Word16, 3, 6)),
                Instr(Opcode.strb, r0, MemIdx_r(PrimitiveType.Byte, 3, 6)),
                Instr(Opcode.ldrsb, r0, MemIdx_r(PrimitiveType.SByte, 3, 6)),

                Instr(Opcode.ldr, r0, MemIdx_r(PrimitiveType.Word32, 3, 6)),
                Instr(Opcode.ldrh, r0, MemIdx_r(PrimitiveType.Word16, 3, 6)),
                Instr(Opcode.ldrb, r0, MemIdx_r(PrimitiveType.Byte, 3, 6)),
                Instr(Opcode.ldrsh, r0, MemIdx_r(PrimitiveType.Int16, 3, 6)));

            var decLdStWB = Nyi("LdStWB");
            var decLdStHalfword = Nyi("LdStHalfWord");
            var decLdStSpRelative = Nyi("LdStSpRelative");
            var decAddPcSp = Mask(11, 1,
                Instr(Opcode.adr, r8,P(0,8)),
                Instr(Opcode.add, r8,sp,Simm(0, 8, 2)));
            var decMisc16Bit = CreateMisc16bitDecoder();
            var decLdmStm = new LdmStmDecoder16();
            var decCondBranch = Mask(8, 0xF, "CondBranch",
                Instr(Opcode.b, c8,PcRelative(1, Bf((0, 8)))),
                Instr(Opcode.b, c8, PcRelative(1, Bf((0, 8)))),
                Instr(Opcode.b, c8, PcRelative(1, Bf((0, 8)))),
                Instr(Opcode.b, c8, PcRelative(1, Bf((0, 8)))),

                Instr(Opcode.b, c8, PcRelative(1, Bf((0, 8)))),
                Instr(Opcode.b, c8, PcRelative(1, Bf((0, 8)))),
                Instr(Opcode.b, c8, PcRelative(1, Bf((0, 8)))),
                Instr(Opcode.b, c8, PcRelative(1, Bf((0, 8)))),

                Instr(Opcode.b, c8, PcRelative(1, Bf((0, 8)))),
                Instr(Opcode.b, c8, PcRelative(1, Bf((0, 8)))),
                Instr(Opcode.b, c8, PcRelative(1, Bf((0, 8)))),
                Instr(Opcode.b, c8, PcRelative(1, Bf((0, 8)))),

                Instr(Opcode.b, c8, PcRelative(1, Bf((0, 8)))),
                Instr(Opcode.b, c8, PcRelative(1, Bf((0, 8)))),
                Instr(Opcode.udf, Imm(0,8)),
                Instr(Opcode.svc, InstrClass.Transfer | InstrClass.Call, Imm(0, 8)));

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
                            Instr(Opcode.bx, R3),
                            Instr(Opcode.blx, R3))),
                    LdrLiteral,
                    LdrLiteral,

                    LdStRegOffset,
                    LdStRegOffset,
                    LdStRegOffset,
                    LdStRegOffset),
                Mask(11, 0x03,   // decLdStWB,
                    Instr(Opcode.str, r0, MemOff_r(PrimitiveType.Word32, 3, shift:2, fields: (6,5))),
                    Instr(Opcode.ldr, r0, MemOff_r(PrimitiveType.Word32, 3, shift:2, fields: (6,5))),
                    Instr(Opcode.strb, r0, MemOff_r(PrimitiveType.Byte, 3, fields: (6,5))),
                    Instr(Opcode.ldrb, r0, MemOff_r(PrimitiveType.Byte, 3, fields: (6,5)))),

                Mask(12, 0x01,
                    Mask(11, 0x01,
                        Instr(Opcode.strh, r0, MemOff_r(PrimitiveType.Word16, 3, shift:1, fields: (6,5))),
                        Instr(Opcode.ldrh, r0, MemOff_r(PrimitiveType.Word16, 3, shift:1, fields: (6,5)))),
                    Mask(11, 0x01,   // load store SP-relative
                        Instr(Opcode.str, r8, MemOff_r(PrimitiveType.Word32, baseReg:Registers.sp, shift:2, fields: (0,8))),
                        Instr(Opcode.ldr, r8, MemOff_r(PrimitiveType.Word32, baseReg:Registers.sp, shift:2, fields: (0,8))))),
                Mask(12, 0x01,
                    decAddPcSp,
                    decMisc16Bit),
                Mask(12, 0x01,
                    decLdmStm,
                    decCondBranch),
                Instr(Opcode.Invalid));
        }

        private static Decoder CreateAluDecoder()
        {
            var decAddSub3 = Nyi("addsub3");
            var decAddSub3Imm = Nyi("AddSub3Imm");
            var decMovMovs = Mask(11, 3,
                Select((6,5), n => n != 0,
                    new MovMovsDecoder(Opcode.lsl, uf,r0,r3,S(6,5)),
                    Instr(Opcode.mov, r0,r3)),
                new MovMovsDecoder(Opcode.lsr, uf,r0,r3,S(6,5)),
                Instr(Opcode.asrs, uf,r0,r3,S(6,5)),
                invalid);
            var decAddSub = Mask(11, 3,
                Instr(Opcode.mov, r8,Imm(0,8)),
                Instr(Opcode.cmp, uf,r8,Imm(0,8)),
                Instr(Opcode.add, uf,r8,Imm(0,8)),
                Instr(Opcode.sub, uf,r8,Imm(0,8)));
            return Mask(10, 0xF,
                decMovMovs,
                decMovMovs,
                decMovMovs,
                decMovMovs,

                decMovMovs,
                decMovMovs,
                Mask(9, 1,
                    Instr(Opcode.add, r0,r3,r6),
                    Instr(Opcode.sub, r0,r3,r6)),
                Mask(9, 1,
                    Instr(Opcode.add, r0,r3,Imm(6,3)),
                    Instr(Opcode.sub, r0,r3,Imm(6,3))),
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
                Instr(Opcode.and, ufit, r0, r3),
                Instr(Opcode.eor, ufit, r0, r3),
                Instr(Opcode.lsl, ufit, r0, r3),
                Instr(Opcode.lsr, ufit, r0, r3),

                Instr(Opcode.asr, ufit, r0, r3),
                Instr(Opcode.adc, ufit, r0, r3),
                Instr(Opcode.sbc, ufit, r0, r3),
                Instr(Opcode.ror, ufit, r0, r3),

                Instr(Opcode.adc, ufit, r0, r3),
                Instr(Opcode.rsb, ufit, r0, r3),
                Instr(Opcode.cmp, uf, r0, r3),
                Instr(Opcode.cmn, uf, r0, r3),

                Instr(Opcode.orr, ufit, r0, r3),
                Instr(Opcode.mul, ufit, r0, r3),
                Instr(Opcode.bic, ufit, r0, r3),
                Instr(Opcode.mvn, ufit, r0, r3));
        }

        private static Decoder CreateMisc16bitDecoder()
        {
            var pushAndPop = Mask(11, 1,
                Instr(Opcode.push, mw),
                Instr(Opcode.pop, mr));

            var cbnzCbz = Mask(11, 1,
                Instr(Opcode.cbz, r0,x),
                Instr(Opcode.cbnz, r0,x));

            return Mask(8, 0xF,
                Mask(7, 1,  // Adjust SP
                    Instr(Opcode.add, sp,Simm(0,7, 2)),
                    Instr(Opcode.sub, sp,Simm(0,7, 2))),
                cbnzCbz,
                Mask(6, 3,
                    Instr(Opcode.sxth, r0,r3),
                    Instr(Opcode.sxtb, r0,r3),
                    Instr(Opcode.uxth, r0,r3),
                    Instr(Opcode.uxtb, r0,r3)),
                cbnzCbz,

                pushAndPop,
                pushAndPop,
                Mask(5, 0x7,
                    Instr(Opcode.setpan, Imm(3,1)),
                    invalid,
                    Instr(Opcode.setend, Imm(3, 1)),
                    Instr(Opcode.cps, Imm(3, 1)),

                    invalid,
                    invalid,
                    invalid,
                    invalid),
                invalid,

                invalid,
                cbnzCbz,
                Mask(6, 0x3,
                    Instr(Opcode.rev, r0,r3),
                    Instr(Opcode.rev, r0,r3),
                    Instr(Opcode.hlt),
                    Instr(Opcode.rev, r0,r3)),
                cbnzCbz,

                pushAndPop,
                pushAndPop,
                Instr(Opcode.bkpt),
                Select(w => (w & 0xF) == 0,
                    Mask(4, 0xF, // Hints
                        Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear),
                        Instr(Opcode.yield),
                        Instr(Opcode.wfe),
                        Instr(Opcode.wfi),

                        Instr(Opcode.sev),
                        Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hints, behaves as NOP.
                        Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear),
                        Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear),

                        Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hints, behaves as NOP.
                        Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear),
                        Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear),
                        Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear),

                        Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear),
                        Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear),
                        Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear),
                        Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear)),
                    new ItDecoder()));
        }

        private static LongDecoder CreateLongDecoder()
        {
            var branchesMiscControl = CreateBranchesMiscControl();
            var loadStoreMultipleTableBranch = CreateLoadStoreDualMultipleBranchDecoder();

            var LdStMultiple = Mask(7 + 16, 3,
                Mask(4 + 16, 1,
                    Instr(Opcode.srsdb, w21,sp,Imm(0,5)),
                    Instr(Opcode.rfedb, w21,R16)),
                Mask(4 + 16, 1,
                    new LdmStmDecoder32(Opcode.stm),
                    new LdmStmDecoder32(Opcode.ldm)),
                Mask(4 + 16, 1,
                    new LdmStmDecoder32(Opcode.stmdb),
                    new LdmStmDecoder32(Opcode.ldmdb)),
                Mask(4 + 16, 1,
                    Instr(Opcode.srsia, w21,sp,Imm(0,5)),
                    Instr(Opcode.rfeia, w21,R16)));

            var DataProcessingModifiedImmediate = Mask(4 + 16, 0x1F,
                Instr(Opcode.and, R8,R16,M),
                Select(wInstr => SBitfield(wInstr, 8, 4) != 0xF,
                    Instr(Opcode.and, uf,R8,R16,M),
                    Instr(Opcode.tst, R16,M)),
                Instr(Opcode.bic, R8,R16,M),
                Instr(Opcode.bic, uf,R8,R16,M),
                // 4
                Select(wInstr => SBitfield(wInstr, 16, 4) != 0xF,
                    Instr(Opcode.orr, R8,R16,M),
                    Instr(Opcode.mov, R8,M)),
                Select(wInstr => SBitfield(wInstr, 16, 4) != 0xF,
                    Instr(Opcode.orr, uf,R8,R16,M),
                    Instr(Opcode.mov, uf,R8,M)),
                Select(wInstr => SBitfield(wInstr, 16, 4) != 0xF,
                    Instr(Opcode.orn, R8,R16,M),
                    Instr(Opcode.mvn, R8,M)),
                Select(wInstr => SBitfield(wInstr, 16, 4) != 0xF,
                    Instr(Opcode.orn, uf,R8,R16,M),
                    Instr(Opcode.mvn, uf,R8,M)),
                // 8
                Instr(Opcode.eor, R8,R16,M),
                Select(wInstr => SBitfield(wInstr, 8, 4) != 0xF,
                    Instr(Opcode.eor, uf,R8,R16,M),
                    Instr(Opcode.teq, uf,R8,M)),
                invalid,
                invalid,
                // C
                invalid,
                invalid,
                invalid,
                invalid,
                // 10
                Select(wInstr => SBitfield(wInstr, 16, 4) != 0xD,
                    Instr(Opcode.add, R8,R16,M),
                    Instr(Opcode.add, R9,R16,M)), //$REVIEW: check this
                Select(wInstr => SBitfield(wInstr, 8, 4) != 0xF,
                    Select(wInstr => SBitfield(wInstr, 16, 4) != 0xD,
                        Instr(Opcode.add, uf,R8,R16,M),
                        Instr(Opcode.add, uf,R9,R16,M)), //$REVIEW: check this
                    Instr(Opcode.cmn, R16,M)),
                invalid,
                invalid,
                // 14
                Instr(Opcode.adc, R8,R16,M),
                Instr(Opcode.adc, uf,R9,R16,M),
                Instr(Opcode.sbc, R8,R16,M),
                Instr(Opcode.sbc, uf,R9,R16,M),
                // 18
                invalid,
                invalid,
                Select(wInstr => SBitfield(wInstr, 16, 4) != 0xD,
                    Instr(Opcode.sub, R8,R16,M),
                    Instr(Opcode.sub, R9,R16,M)), //$REVIEW: check this
                Select(wInstr => SBitfield(wInstr, 8, 4) != 0xF,
                    Select(wInstr => SBitfield(wInstr, 16, 4) != 0xD,
                        Instr(Opcode.sub, uf,R8,R16,M),
                        Instr(Opcode.sub, uf,R9,R16,M)), //$REVIEW: check this
                    Instr(Opcode.cmp, R16,M)),
                // 1C
                Instr(Opcode.rsb, R8,R16,M),
                Instr(Opcode.rsb, uf,R9,R16,M),
                invalid,
                invalid);

            var DataProcessingSimpleImm = Mask(7 + 16, 1,
                Mask(5 + 16, 1,
                    Select(w => (SBitfield(w, 16, 4) & 0xD) != 0xD,
                        Mask(10+16, 1,
                            Instr(Opcode.add, R8,R16,Imm26_12_0),
                            Instr(Opcode.add, uf,R8,R16,Imm26_12_0)),
                        Mask(17, 1,
                            Instr(Opcode.add, R8,R16,Imm26_12_0),
                            Nyi("ADR - T3"))),
                    invalid),
                Mask(5 + 16, 1,
                    invalid,
                    Select(w => (SBitfield(w, 16, 4) & 0xD) != 0xD,
                        Mask(10 + 16, 1,
                            Instr(Opcode.sub, R8,R16,Imm26_12_0),
                            Instr(Opcode.sub, uf,R8,R16,Imm26_12_0)),
                        Mask(17, 1,
                            Instr(Opcode.sub, R8,R16,Imm26_12_0),
                            Nyi("ADR - T2")))));

            var SaturateBitfield = Mask(5 + 16, 0x7,
                Instr(Opcode.ssat, R8, ImmM1(0,5), R16, LslImm(12, 3, 6, 2)),
                Select(w => SBitfield(w, 12, 3) != 0 || SBitfield(w, 6, 2) != 0,
                    Nyi("ssatAsrVariant"),
                    Nyi("ssat16")),
                Instr(Opcode.sbfx, R8, R16,Imm(12,3,6,2), ImmM1(0, 5)),
                Select(w => SBitfield(w, 16, 4) != 0xF,
                    DecodeBfcBfi(Opcode.bfi, R8,R16,Imm(12,3,6,2),Imm(0,5)),
                    DecodeBfcBfi(Opcode.bfc, R8, Imm(12, 3, 6, 2), Imm(0, 5))),
                // 4
                Instr(Opcode.usat, R8, ImmM1(0,5), R16, LslImm(12, 3, 6, 2)),
                Select(w => SBitfield(w, 12, 3) != 0 || SBitfield(w, 6, 2) != 0,
                    Nyi("usatAsrVariant"),
                    Nyi("usat16")),
                Instr(Opcode.ubfx, R8, R16,Imm(12,3,6,2), ImmM1(0, 5)),
                invalid);

            var MoveWide16BitImm = Mask(7 + 16, 1,
                Instr(Opcode.mov, R8,Imm(PrimitiveType.Word32, fields: Bf((16,4),(26,1),(12,3),(0,8)))),
                Instr(Opcode.movt, R8,Imm(PrimitiveType.Word16, fields: Bf((16,4),(26,1),(12,3),(0,8)))));

            var DataProcessingPlainImm = Mask(8 + 16, 1,
                Mask(5 + 16, 3,
                    DataProcessingSimpleImm,
                    DataProcessingSimpleImm,
                    MoveWide16BitImm,
                    invalid),
                SaturateBitfield);

            var LoadStoreSignedPositiveImm = Select(w => SBitfield(w, 12, 4) != 0xF,
                Mask(5 + 16, 3,
                    Instr(Opcode.ldrsb, R12,MemOff(PrimitiveType.SByte, 16, indexSpec:idx10, offsetFields:(0,12))),
                    Instr(Opcode.ldrsh, R12,MemOff(PrimitiveType.Int16, 16, indexSpec:idx10, offsetFields:(0, 12))),
                    invalid,
                    invalid),
                Mask(5 + 16, 3,
                    Nyi("PLI"),
                    Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear),
                    invalid,
                    invalid));   // reserved hint

            var LoadStoreSignedImmediatePostIndexed = Mask(5 + 16, 3,
                Instr(Opcode.ldrsb, R12,MemOff(PrimitiveType.SByte, 16, indexSpec:idx10, offsetFields:(0,8))),
                Instr(Opcode.ldrsh, R12,MemOff(PrimitiveType.Int16, 16, indexSpec:idx10, offsetFields:(0,8))),
                invalid,
                invalid);

            var LoadStoreSignedNegativeImm = Mask(5 + 16, 3,
                Select((12, 4), w=> w != 0xF,
                    Instr(Opcode.ldrsb, R12,MemOff(PrimitiveType.SByte, 16, offsetFields:(0,8))),
                    Instr(Opcode.pli, nyi("*"))),
                Select((12, 4), w=> w != 0xF,
                    Instr(Opcode.ldrsh, R12, MemOff(PrimitiveType.Int16, 16, offsetFields: (0,8))),
                    Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear)),        // Reserved hint
                invalid,
                invalid);

            var LoadStoreUnsignedImmediatePostIndexed = Mask(4 + 16, 7,
                Instr(Opcode.strb, R12, MemOff(PrimitiveType.Byte, 16, indexSpec: idx10, offsetFields: (0, 8))),
                Instr(Opcode.ldrb, R12, MemOff(PrimitiveType.Byte, 16, indexSpec: idx10, offsetFields: (0, 8))),
                Instr(Opcode.strh, R12, MemOff(PrimitiveType.Word16, 16, indexSpec: idx10, offsetFields: (0, 8))),
                Instr(Opcode.ldrh, R12, MemOff(PrimitiveType.Word16, 16, indexSpec: idx10, offsetFields: (0, 8))),
                Instr(Opcode.str, R12, MemOff(PrimitiveType.Word32, 16, indexSpec: idx10, offsetFields: (0, 8))),
                Instr(Opcode.ldr, R12, MemOff(PrimitiveType.Word32, 16, indexSpec: idx10, offsetFields: (0, 8))),
                invalid,
                invalid);

            var LoadStoreUnsignedPositiveImm = Mask(4 + 16, 7,
                Instr(Opcode.strb, R12,MemOff(PrimitiveType.Byte, 16, offsetFields: (0,12))),
                Select(w => SBitfield(w, 12, 4) != 0xF,
                    Instr(Opcode.ldrb, R12, MemOff(PrimitiveType.Byte, 16, offsetFields: (0, 12))),
                    Nyi("PLD,PLDW immediate preloadread")),
                Instr(Opcode.strh, R12, MemOff(PrimitiveType.Word16, 16, offsetFields: (0, 12))),
                Select(w => SBitfield(w, 12, 4) != 0xF,
                    Instr(Opcode.ldrh, R12, MemOff(PrimitiveType.Word16, 16, offsetFields: (0, 12))),
                    Nyi("*PLD,PLDW immediate preloadwrite")),
                // 4
                Instr(Opcode.str, R12, MemOff(PrimitiveType.Word32, 16, offsetFields: (0, 12))),
                Instr(Opcode.ldr, R12, MemOff(PrimitiveType.Word16, 16, offsetFields: (0, 12))),
                invalid,
                invalid);

            var LoadStoreUnsignedImmediatePreIndexed = Mask(4 + 16, 7,
                Instr(Opcode.strb, R12, MemOff(PrimitiveType.Byte, 16, indexSpec: idx10, offsetFields: (0, 8))),
                Instr(Opcode.ldrb, R12, MemOff(PrimitiveType.Byte, 16, indexSpec: idx10, offsetFields: (0, 8))),
                Instr(Opcode.strh, R12, MemOff(PrimitiveType.Word16, 16, indexSpec: idx10, offsetFields: (0, 8))),
                Instr(Opcode.ldrh, R12, MemOff(PrimitiveType.Word16, 16, indexSpec: idx10, offsetFields: (0, 8))),
                Instr(Opcode.str, R12, MemOff(PrimitiveType.Word32, 16, indexSpec: idx10, offsetFields: (0, 8))),
                Instr(Opcode.str, R12, MemOff(PrimitiveType.Word32, 16, indexSpec: idx10, offsetFields: (0, 8))),
                invalid,
                invalid);

            var LoadStoreUnsignedRegisterOffset = Mask(4 + 16, 7,
                Instr(Opcode.strb, R12,MemIdx(PrimitiveType.Byte,16,0,(4,2))),
                Select((16, 4), n => n != 0xF,
                    Instr(Opcode.ldrb, wide,R12,MemIdx(PrimitiveType.Byte,16,0,(4,2))),
                    Instr(Opcode.pld, nyi("*"))),
                Instr(Opcode.strh, MemIdx(PrimitiveType.Word16, 16, 0, (4, 2))),
                Select((16, 4), n => n != 0xF,
                    Instr(Opcode.ldrh, wide, R12,MemIdx(PrimitiveType.Word16, 16, 0, (4, 2))),
                    Instr(Opcode.pld, nyi("*"))),
                Instr(Opcode.str, wide, R12,MemIdx(PrimitiveType.Word32, 16, 0, (4, 2))),
                Instr(Opcode.ldr, wide, R12,MemIdx(PrimitiveType.Word32, 16, 0, (4, 2))),
                invalid,
                invalid);

            var LoadStoreUnsignedNegativeImm = Mask(4 + 16, 7,
                Instr(Opcode.strb, R12, MemOff(PrimitiveType.Byte, 16, indexSpec: idx10, offsetFields: (0, 8))),
                Select((16, 4), n => n != 0xF,
                    Instr(Opcode.ldrb, wide,R12,MemOff(PrimitiveType.Byte,16,indexSpec:idx10, offsetFields:(0, 8))),
                    Instr(Opcode.pld, nyi("*"))),
                Instr(Opcode.strh, R12, MemOff(PrimitiveType.Word16, 16, indexSpec: idx10, offsetFields: (0, 8))),
                Select((16, 4), n => n != 0xF,
                    Instr(Opcode.ldrh, wide, R12, MemOff(PrimitiveType.Word16, 16, indexSpec: idx10, offsetFields: (0, 8))),
                    Instr(Opcode.pld, nyi("*"))),
                Instr(Opcode.str, wide, R12, MemOff(PrimitiveType.Word32, 16, indexSpec: idx10, offsetFields: (0, 8))),
                Instr(Opcode.ldr, wide, R12, MemOff(PrimitiveType.Word32, 16, indexSpec: idx10, offsetFields: (0, 8))),
                invalid,
                invalid);

            var LoadStoreUnsignedUnprivileged = Mask(4 + 16, 7,
                Instr(Opcode.strbt, R12, MemOff(PrimitiveType.Byte, 16, offsetFields:(0,8))),
                Instr(Opcode.ldrbt, R12, MemOff(PrimitiveType.Byte, 16, offsetFields:(0,8))),
                Instr(Opcode.strht, R12, MemOff(PrimitiveType.Word16, 16, offsetFields:(0,8))),
                Instr(Opcode.ldrht, R12, MemOff(PrimitiveType.Word16, 16, offsetFields:(0,8))),
                Instr(Opcode.strt, R12, MemOff(PrimitiveType.Word32, 16, offsetFields:(0,8))),
                Instr(Opcode.ldrt, R12, MemOff(PrimitiveType.Word32, 16, offsetFields:(0,8))),
                invalid,
                invalid);

            var LoadUnsignedLiteral = Select((12,4), n => n != 0xF,
                Mask(5 + 16, 3,
                    Instr(Opcode.ldrb, R12, MemOff(PrimitiveType.Byte, baseReg:Registers.pc, offsetFields:(0,12))),
                    Instr(Opcode.ldrh, R12, MemOff(PrimitiveType.Word16, baseReg: Registers.pc, offsetFields: (0, 12))),
                    Instr(Opcode.ldr, R12, MemOff(PrimitiveType.Word32, baseReg:Registers.pc, offsetFields:(0,12))),
                    invalid),
                Mask(5 + 16, 3,
                    Instr(Opcode.pld, nyi("* literal")),
                    Instr(Opcode.pld, nyi("* literal")),
                    invalid,
                    invalid));

            var LoadSignedLiteral = Select((12,4), n => n != 0xF,
                Mask(5 + 16, 3,
                    Instr(Opcode.ldrsb, R12, MemOff(PrimitiveType.SByte, baseReg: Registers.pc, offsetFields: new[] { (8, 4), (0, 4) })),
                    Instr(Opcode.ldrsh, R12, MemOff(PrimitiveType.Int16, baseReg: Registers.pc, offsetFields: new[] { (8, 4), (0, 4) })),
                    invalid,
                    invalid),
                Mask(5 + 16, 3,
                    Instr(Opcode.pli, nyi("* literal")),
                    Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear),
                    invalid,
                    invalid));

            var LoadStoreSignedRegisterOffset = Select((12,4), n => n != 0xF,
                Mask(5 + 16, 3,
                    Instr(Opcode.ldrsb, nyi("*register")),
                    Instr(Opcode.ldrsh, nyi("*register")),
                    invalid,
                    invalid),
                Mask(5 + 16, 3,
                    Instr(Opcode.pli, nyi("*register")),
                    Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear),
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
                Select((16, 4), w  => w != 0xF,
                    Mask(10, 3,
                        Select(w => SBitfield(w, 6, 6) == 0,
                            LoadStoreSignedRegisterOffset,
                            invalid),
                        invalid,
                        Select(w => SBitfield(w, 8, 1) == 0,
                            invalid,
                            LoadStoreSignedImmediatePostIndexed),
                        Mask(8, 3,
                            LoadStoreSignedNegativeImm,
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
                            Instr(Opcode.stc, nyi("*post-indexed")),
                            Select((16,4), n => n != 15,
                                Instr(Opcode.ldc, nyi("*imm")),
                                ldc_literal),

                            Instr(Opcode.stc, nyi("*unindexed variant")),
                            Select((16,4), n => n != 15,
                                Instr(Opcode.ldc, nyi("*immediate - unindexed variant")),
                                ldc_literal),
                            invalid,
                            invalid, 

                            Instr(Opcode.stc, nyi("*offset variant")),
                            Select((16,4), n => n != 15,
                                Instr(Opcode.ldc, nyi("*offset variant")),
                                ldc_literal),
                            Instr(Opcode.stc, nyi("*preindexed variant")),
                            Select((16,4), n => n != 15,
                                Instr(Opcode.ldc, nyi("*preindexed variant")),
                                ldc_literal),

                            Instr(Opcode.stc, CPn14,CR12,MemOff(PrimitiveType.Word32, 16, offsetShift:2, offsetFields:(0,8))),
                            Select((16,4), n => n != 15,
                                Instr(Opcode.ldc, nyi("*offset variant")),
                                ldc_literal),
                            Instr(Opcode.stc, nyi("*preindexed variant")),
                            Select((16,4), n => n != 15,
                                Instr(Opcode.ldc, nyi("*preindexed variant")),
                                ldc_literal)))));

            var SystemRegisterLdStAnd64bitMove = Select(Bf((23,2),(21,1)), n => (n & 0xD) == 0,
                Nyi("SystemRegister64bitMove"),
                SystemRegisterLdSt);

            var vstmia = Mask(8, 0x3, // size
                    invalid,
                    invalid,
                    Instr(Opcode.vstmia, nyi("*")),
                    Mask(0, 1,
                        Instr(Opcode.vstmia, nyi("*")),
                        Instr(Opcode.fstmiax, nyi("*"))));

            var vldmia = Mask(8, 0x3, // size
                    invalid,
                    invalid,
                    Instr(Opcode.vldmia, nyi("*")),
                    Mask(0, 1,
                        Instr(Opcode.vldmia, nyi("*")),
                        Instr(Opcode.fldmiax, nyi("*"))));
            var vstr = Mask(8, 3,  // size
                invalid,
                Instr(Opcode.vstr, F12_22,MemOff(PrimitiveType.Real16, 16, offsetShift:2, indexSpec:idx24, offsetFields:(0,8))),
                Instr(Opcode.vstr, F12_22,MemOff(PrimitiveType.Real32, 16, offsetShift:2, indexSpec:idx24, offsetFields:(0,8))),
                Instr(Opcode.vstr, D22_12,MemOff(PrimitiveType.Real64, 16, offsetShift:2, indexSpec:idx24, offsetFields:(0,8))));
            var vldr = Select(w => SBitfield(w, 16, 4) != 0xF,
                Mask(8, 3,
                    invalid,
                    Instr(Opcode.vldr, F12_22,MemOff(PrimitiveType.Real16, 16, offsetShift:2, indexSpec:idx24, offsetFields:(0,8))),
                    Instr(Opcode.vldr, F12_22,MemOff(PrimitiveType.Real32, 16, offsetShift:2, indexSpec:idx24, offsetFields:(0,8))),
                    Instr(Opcode.vldr, D22_12,MemOff(PrimitiveType.Real64, 16, offsetShift:2, indexSpec:idx24, offsetFields:(0,8)))),
                Instr(Opcode.vldr, nyi("*lit")));
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
                        Instr(Opcode.vnmul, F16, F12_22,F16_7,F0_5),
                        Instr(Opcode.vnmul, F32, F12_22,F16_7,F0_5),
                        Instr(Opcode.vnmul, F64, D22_12,D7_16,D5_0)),
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
                    Instr(Opcode.vseleq, F64, D22_12,D7_16,D5_0),
                    Instr(Opcode.vselvs, F64, D22_12,D7_16,D5_0),
                    Instr(Opcode.vselge, F64, D22_12,D7_16,D5_0),
                    Instr(Opcode.vselgt, F64, D22_12,D7_16,D5_0)));

            var FloatingPointMinNumMaxNum =
                Mask(6, 1,
                    Mask(8, 3,
                        invalid,
                        Instr(Opcode.vmaxnm, F16, F12_22,F16_7,F0_5),
                        Instr(Opcode.vmaxnm, F32, F12_22,F16_7,F0_5),
                        Instr(Opcode.vmaxnm, F64, D22_12,D7_16,D5_0)),
                    Mask(8, 3,
                        invalid,
                        Instr(Opcode.vminnm, F16, F12_22,F16_7,F0_5),
                        Instr(Opcode.vminnm, F32, F12_22,F16_7,F0_5),
                        Instr(Opcode.vminnm, F64, D22_12,D7_16,D5_0)));

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
                    Instr(Opcode.vmov, nyi("*between GPR and single prec")),
                    Select((21,3), n => n == 7,
                        Mask(20, 1,
                            Instr(Opcode.vmsr, nyi("*")),
                            Instr(Opcode.vmrs, nyi("*"))),
                        invalid)),
                Nyi("AdvancedSimd8_16_32_bitElementMove"));

            var AdvancedSimdLdStMultipleStructures = Mask(21, 1,
                Mask(8, 15,
                    Instr(Opcode.vst4, nyi("*")),
                    Instr(Opcode.vst4, nyi("*")),
                    Instr(Opcode.vst1, nyi("*multiple single elements - T4")),
                    Instr(Opcode.vst2, nyi("*multiple 2-element structures - T2")),

                    Instr(Opcode.vst3, nyi("*multiple 3-element structures")),
                    Instr(Opcode.vst3, nyi("*multiple 3-element structures")),
                    Instr(Opcode.vst1, nyi("*multiple single elements - T3")),
                    Instr(Opcode.vst1, nyi("*multiple single elements - T1")),

                    Instr(Opcode.vst2, nyi("*multiple 2-element structures - T1")),
                    Instr(Opcode.vst2, nyi("*multiple 2-element structures - T1")),
                    Instr(Opcode.vst1, nyi("*multiple single elements - T2")),
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
                    Instr(Opcode.mcr, CP8,Imm(21,3),R12,CR16,CR0,Imm(5,3)),
                    Instr(Opcode.mrc, CP8,Imm(21,3),R12,CR16,CR0,Imm(5,3))),
                invalid);

            var AdvancedSimd3RegistersSameLength = Mask(8, 0xF, // opc
                Mask(4, 1, // o1
                    Mask(6, 1,
                        Instr(Opcode.vhadd, vu(20), D22_12,D7_16,D5_0),
                        Instr(Opcode.vhadd, vu(20), Q22_12,Q7_16,Q5_0)),
                    Mask(6, 1,
                        Instr(Opcode.vqadd, vu(20), D22_12,D7_16,D5_0),
                        Instr(Opcode.vqadd, vu(20), Q22_12,Q7_16,Q5_0))),
                Mask(12 + 16, 1,  // U
                    Mask(4, 1,      // o1
                        Instr(Opcode.vrhadd, nyi("*")),
                        Mask(4 + 16, 3, // size
                            Instr(Opcode.vand, nyi("*register")),
                            Instr(Opcode.vbic, nyi("*register")),
                            Instr(Opcode.vorr, nyi("*register")),
                            Instr(Opcode.vorn, nyi("*register")))),
                    Mask(4, 1,      // o1),
                        Instr(Opcode.vrhadd, nyi("*")),
                        Mask(4 + 16, 3, // size
                            Mask(6, 1, // Q
                                Instr(Opcode.veor, D22_12,D7_16,D5_0),
                                Instr(Opcode.veor, Q22_12,Q7_16,Q5_0)),
                            Instr(Opcode.vbsl, nyi("*register")),
                            Instr(Opcode.vbit, nyi("*register")),
                            Instr(Opcode.vbif, nyi("*register"))))),
                Mask(4, 1, // o1
                    Mask(6, 1,
                        Instr(Opcode.vhsub, vu(20), D22_12,D7_16,D5_0),
                        Instr(Opcode.vhsub, vu(20), Q22_12,Q7_16,Q5_0)),
                    Instr(Opcode.vqsub, nyi("*"))),
                Nyi("AdvancedSimd3RegistersSameLength_opc3"),

                Nyi("AdvancedSimd3RegistersSameLength_opc4"),
                Mask(4, 1,
                    Mask(6, 1, // Q
                        Instr(Opcode.vrshl, vu(20),D22_12,D7_16,D5_0),
                        Instr(Opcode.vrshl, vu(20),Q22_12,Q7_16,Q5_0)),
                    Mask(6, 1, // Q
                        Instr(Opcode.vqrshl, vu(20),D22_12,D7_16,D5_0),
                        Instr(Opcode.vqrshl, vu(20),Q22_12,Q7_16,Q5_0))),
                Mask(4, 1,
                    Mask(6, 1, // Q
                        Instr(Opcode.vmax, vu(20),D22_12,D7_16,D5_0),
                        Instr(Opcode.vmax, vu(20),Q22_12,Q7_16,Q5_0)),
                    Mask(6, 1, // Q
                        Instr(Opcode.vmin, vu(20),D22_12,D7_16,D5_0),
                        Instr(Opcode.vmin, vu(20),Q22_12,Q7_16,Q5_0))),
                Nyi("AdvancedSimd3RegistersSameLength_opc7"),

                Mask(12 + 16, 1,  // U
                    Mask(4, 1, // op1
                        Mask(6, 1, // Q
                            Instr(Opcode.vadd, vi(20),D22_12,D7_16,D5_0),
                            Instr(Opcode.vadd, vi(20),Q22_12,Q7_16,Q5_0)),
                        Mask(6, 1, // Q
                            Instr(Opcode.vtst, vi(20),D22_12,D7_16,D5_0),
                            Instr(Opcode.vtst, vi(20),Q22_12,Q7_16,Q5_0))),
                    Mask(4, 1, "opc=8 U=1 op1",
                        Mask(6, 1, "opc=8 U=1 op1=0 Q",
                            Instr(Opcode.vsub, vi(20),D22_12,D7_16,D5_0),
                            Instr(Opcode.vsub, vi(20),Q22_12,Q7_16,Q5_0)),
                        Mask(6, 1, "opc=8 U=1 op1=0 Q",
                            Instr(Opcode.vceq, vi(20), D22_12,D7_16,D5_0),
                            Instr(Opcode.vceq, vi(20), Q22_12,Q7_16,Q5_0)))),
                // opc9
                Mask(12 + 16, 1,  // U
                    Mask(4, 1,      // op1
                        Mask(6, 1, // Q
                            Instr(Opcode.vmla, vi(20),D22_12,D7_16,D5_0),
                            Instr(Opcode.vmla, vi(20),Q22_12,Q7_16,Q5_0)),
                        Nyi("*vmul (integer and polynomial")),
                    Mask(4, 1,      // op1
                        Mask(6, 1, // Q
                            Instr(Opcode.vmls, vi(20),D22_12,D7_16,D5_0),
                            Instr(Opcode.vmls, vi(20),Q22_12,Q7_16,Q5_0)),
                        Nyi("*vmul (integer and polynomial"))),
                Mask(6, 1, // Q
                    Mask(4, 1, // op1
                        Instr(Opcode.vpmax, vu(20), D22_12,D7_16,D5_0),
                        Instr(Opcode.vpmin, vu(20), D22_12,D7_16,D5_0)),
                    invalid),
                Nyi("AdvancedSimd3RegistersSameLength_opcB"),

                Nyi("AdvancedSimd3RegistersSameLength_opcC"),
                // opcD
                Mask(12 + 16, 1,  // U
                    Mask(4, 1,      // op1
                        Mask(6, 1,      // Q
                            Mask(20, 3,  // size
                                Instr(Opcode.vadd, F32, D22_12,D7_16,D5_0),
                                Instr(Opcode.vadd, F16, D22_12,D7_16,D5_0),
                                Instr(Opcode.vsub, F32, D22_12,D7_16,D5_0),
                                Instr(Opcode.vsub, F16, D22_12,D7_16,D5_0)),
                            Mask(20, 3,  // size
                                Instr(Opcode.vadd, F32, Q22_12,Q7_16,Q5_0),
                                Instr(Opcode.vadd, F16, Q22_12,Q7_16,Q5_0),
                                Instr(Opcode.vsub, F32, Q22_12,Q7_16,Q5_0),
                                Instr(Opcode.vsub, F16, Q22_12,Q7_16,Q5_0))),
                        Mask(20, 3,  // high-bit of size
                            Nyi("*vmla (floating point)"),
                            Nyi("*vmla (floating point)"),
                            Nyi("*vmls (floating point)"),
                            Nyi("*vmls (floating point)"))),
                    Mask(4, 1,      // op1
                        Mask(20, 3,  // size
                            Instr(Opcode.vpadd, F32, D22_12,D7_16,D5_0),
                            Instr(Opcode.vpadd, F16, D22_12,D7_16,D5_0),
                            Nyi("*vabd (floating point)"),
                            Nyi("*vabd (floating point)")),
                        Mask(21, 1,  // high-bit of size
                            Mask(6, 1,      // Q
                                Instr(Opcode.vmul, F32, D22_12,D7_16,D5_0),
                                Instr(Opcode.vmul, F16, Q22_12,Q7_16,Q5_0)),
                            invalid))),

                // opc = E
                Mask(12 + 16, 1,  // U
                    Nyi("AdvancedSimd3RegistersSameLength_opcE U=0"),
                    Mask(21, 1,  // high-bit of size
                        Mask(4, 1,      // op1
                            Mask(6, 1,      // Q
                                Instr(Opcode.vcge, F32, D22_12,D7_16,D5_0),
                                Instr(Opcode.vcge, F16, Q22_12,Q7_16,Q5_0)),
                            Instr(Opcode.vacge, nyi("*"))),
                        Mask(4, 1, // op1
                            Mask(6, 1,    // Q
                                Instr(Opcode.vcgt, F32, D22_12,D7_16,D5_0),
                                Instr(Opcode.vcgt, F16, Q22_12,Q7_16,Q5_0)),
                            Nyi("AdvancedSimd3RegistersSameLength_opcE U=1 size=1x o1=1")))),
                // opc = F
                Mask(12 + 16, 1,  // U
                    Nyi("AdvancedSimd3RegistersSameLength_opcF U=0"),
                    Mask(4, 1,      // op1
                        Mask(6, 1,      // Q
                            Mask(20, 3,  // size
                                Instr(Opcode.vpmax, F32, D22_12,D7_16,D5_0),
                                Instr(Opcode.vpmax, F16, D22_12,D7_16,D5_0),
                                Instr(Opcode.vpmin, F32, D22_12,D7_16,D5_0),
                                Instr(Opcode.vpmin, F16, D22_12,D7_16,D5_0)),
                        Nyi("AdvancedSimd3RegistersSameLength_opcF U=0 op1 = 0 Q=1")),
                    Nyi("AdvancedSimd3RegistersSameLength_opcF U=0 op1 = 1"))));

            var vclt_imm0 = Instr(Opcode.vclt, q(6), vif, W22_12, W5_0, IW0);


            var AdvancedSimd2RegsMisc = Mask(16, 3,
                Mask(7, 0xF,
                    Instr(Opcode.vrev64, nyi("*")),
                    Instr(Opcode.vrev32, nyi("*")),
                    Instr(Opcode.vrev16, nyi("*")),
                    invalid,

                    Instr(Opcode.vpaddl, nyi("*")),
                    Instr(Opcode.vpaddl, nyi("*")),
                    Mask(6, 1,
                        Instr(Opcode.aese, nyi("*")),
                        Instr(Opcode.aesd, nyi("*"))),
                    Mask(6, 1,
                        Instr(Opcode.aesmc, nyi("*")),
                        Instr(Opcode.aesimc, nyi("*"))),

                    invalid, //$REVIEW VSWP looks odd.
                    Instr(Opcode.vclz, nyi("*")),
                    Instr(Opcode.vcnt, nyi("*")),
                    Instr(Opcode.vmvn, nyi("*reg")),

                    Instr(Opcode.vpadal, nyi("*")),
                    Instr(Opcode.vpadal, nyi("*")),
                    Instr(Opcode.vqabs, nyi("*")),
                    Instr(Opcode.vqneg, nyi("*"))),
                Mask(7, 0xF,
                    Instr(Opcode.vcgt, nyi("*imm0")),
                    Instr(Opcode.vcge, nyi("*imm0")),
                    Instr(Opcode.vceq, nyi("*imm0")),
                    Instr(Opcode.vcle, nyi("*imm0")),

                    vclt_imm0,
                    Mask(6, 1,
                        invalid,
                        Instr(Opcode.sha1h, nyi("*"))),
                    Mask(6, 1,
                        Mask(10, 1,
                            Instr(Opcode.vabs, vi(18),D22_12,D5_0),
                            Instr(Opcode.vabs, vr(18),D22_12,D5_0)),
                        Mask(10, 1,
                            Instr(Opcode.vabs, vi(18),Q22_12,Q5_0),
                            Instr(Opcode.vabs, vr(18),Q22_12,Q5_0))),
                    Instr(Opcode.vneg, nyi("*")),

                    Instr(Opcode.vcgt, nyi("*imm0")),
                    Instr(Opcode.vcge, nyi("*imm0")),
                    Instr(Opcode.vceq, nyi("*imm0")),
                    Instr(Opcode.vcle, nyi("*imm0")),

                    vclt_imm0,
                    invalid,
                    Mask(6, 1,
                        Mask(10, 1,
                            Instr(Opcode.vabs, vi(18),D22_12,D5_0),
                            Instr(Opcode.vabs, vr(18),D22_12,D5_0)),
                        Mask(10, 1,
                            Instr(Opcode.vabs, vi(18),Q22_12,Q5_0),
                            Instr(Opcode.vabs, vr(18),Q22_12,Q5_0))),
                    Instr(Opcode.vqneg, nyi("*"))),
                Mask(7, 0xF,
                    invalid,
                    Instr(Opcode.vtrn, nyi("*")),
                    Instr(Opcode.vuzp, nyi("*")),
                    Instr(Opcode.vzip, nyi("*")),

                    Mask(6, 1,
                        Instr(Opcode.vmovn, nyi("*")),
                        Instr(Opcode.vqmovn, nyi("*unsigned"))),
                    Instr(Opcode.vqmovn, nyi("*signed")),
                    Mask(6, 1,
                        Instr(Opcode.vshll, nyi("*")),
                        invalid),
                    Mask(6, 1,
                        Instr(Opcode.sha1su1, nyi("*")),
                        Instr(Opcode.sha256su0, nyi("*"))),

                    Instr(Opcode.vrintn, nyi("*")),
                    Instr(Opcode.vrintx, nyi("*")),
                    Instr(Opcode.vrinta, nyi("*")),
                    Instr(Opcode.vrintz, nyi("*")),

                    Mask(6, 1,
                        Instr(Opcode.vcvt, vc,D22_12,D5_0),
                        invalid),
                    Instr(Opcode.vrintm, nyi("*")),
                    Mask(6, 1,
                        Instr(Opcode.vcvt, vc,Q22_12,Q5_0),
                        invalid),
                    Instr(Opcode.vrintp, nyi("*"))),
                Mask(4 + 16, 0xF,
                    Instr(Opcode.vcvta, nyi("*")),
                    Instr(Opcode.vcvta, nyi("*")),
                    Instr(Opcode.vcvtn, nyi("*")),
                    Instr(Opcode.vcvtn, nyi("*")),

                    Instr(Opcode.vcvtp, nyi("*")),
                    Instr(Opcode.vcvtp, nyi("*")),
                    Instr(Opcode.vcvtm, nyi("*")),
                    Instr(Opcode.vcvtm, nyi("*")),

                    Instr(Opcode.vrecpe, nyi("*")),
                    Instr(Opcode.vrsqrte, nyi("*")),
                    Instr(Opcode.vrecpe, nyi("*")),
                    Instr(Opcode.vrsqrte, nyi("*")),

                    Mask(6, 1,
                        Instr(Opcode.vcvt, vc,D22_12,D5_0),
                        Instr(Opcode.vcvt, vc,Q22_12,Q5_0)),
                    Mask(6, 1,
                        Instr(Opcode.vcvt, vc,D22_12,D5_0),
                        Instr(Opcode.vcvt, vc,Q22_12,Q5_0)),
                    Mask(6, 1,
                        Instr(Opcode.vcvt, vc,D22_12,D5_0),
                        Instr(Opcode.vcvt, vc,Q22_12,Q5_0)),
                    Mask(6, 1,
                        Instr(Opcode.vcvt, vc,D22_12,D5_0),
                        Instr(Opcode.vcvt, vc,Q22_12,Q5_0))));

            var AdvancedSimd3DiffLength = Mask(8, 0xF,  // opc
                Instr(Opcode.vaddl, nyi("*")),
                Instr(Opcode.vaddw, nyi("*")),
                Instr(Opcode.vsubl, nyi("*")),
                Instr(Opcode.vsubw, nyi("*")),

                Mask(12 + 16, 1,
                    Instr(Opcode.vaddhn, nyi("*")),
                    Instr(Opcode.vraddhn, nyi("*"))),
                Instr(Opcode.vabal, nyi("*")),
                Mask(12 + 16, 1,
                    Instr(Opcode.vsubhn, nyi("*")),
                    Instr(Opcode.vrsubhn, nyi("*"))),
                Instr(Opcode.vabdl, vi(20), Q22_12,D7_16,D5_0),

                Instr(Opcode.vmlal, vi(20), Q22_12,D7_16,D5_0),
                Mask(12 + 16, 1,
                    Instr(Opcode.vqdmlal, nyi("*integer")),
                    invalid),
                Instr(Opcode.vmlsl, nyi("*integer")),
                Mask(12 + 16, 1,
                    Instr(Opcode.vqdmlsl, nyi("*integer")),
                    invalid),

                Instr(Opcode.vmull, nyi("*integer and polynomial")),
                Mask(12 + 16, 1,
                    Instr(Opcode.vqdmull, nyi("*integer")),
                    invalid),
                invalid,
                invalid);

            var AdvancedSimd2RegsScalar = Mask(8, 0xF, // opc
                Mask(12 +16, 1,
                    Instr(Opcode.vmla, v_hw_(20), D22_12,D7_16,D5_0),
                    Instr(Opcode.vmla, v_hw_(20), Q22_12,Q7_16,Q5_0)),
                Mask(12 + 16, 1,
                    Instr(Opcode.vmla, vF(20), D22_12,D7_16,D5_0),
                    Instr(Opcode.vmla, vF(20), Q22_12,Q7_16,Q5_0)),
                Instr(Opcode.vmlal, nyi("*scalar")),
                Mask(12 + 16, 1, // Q
                    Instr(Opcode.vqdmlal, nyi("*")),
                    invalid),

                Instr(Opcode.vmls, nyi("*scalar")),
                Instr(Opcode.vmls, nyi("*scalar")),
                Instr(Opcode.vmlsl, nyi("*scalar")),
                Mask(12 + 16, 1, // Q
                    Instr(Opcode.vqdmlsl, nyi("*")),
                    invalid),

                Instr(Opcode.vmul, nyi("*scalar")),
                Instr(Opcode.vmul, nyi("*scalar")),
                Instr(Opcode.vmull, nyi("*")),
                Mask(12 + 16, 1, // Q
                    Instr(Opcode.vqdmull, nyi("*")),
                    invalid),

                Instr(Opcode.vqdmulh, nyi("*")),
                Instr(Opcode.vqrdmlah, nyi("*")),
                Instr(Opcode.vqrdmlah, nyi("*")),
                Instr(Opcode.vqrdmlsh, nyi("*")));

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
                    Instr(Opcode.vext, nyi("*"))),
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
                        Mask(6, 1,
                            Instr(Opcode.vtbl, I8, D22_12, nyi("*")),
                            Instr(Opcode.vtbx, I8, D22_12, nyi("*"))),
                        AdvancedSimdDuplicateScalar)));

            var AdvancedSimdTwoScalarsAndExtension = Nyi("AdvancedSimdTwoScalarsAndExtension");

            var vmov_t1_d = Instr(Opcode.vmov, I32, D22_12, MS_28_16_0);
            var vmov_t1_q = Instr(Opcode.vmov, I32, Q22_12, MS_28_16_0);
            var vmvn_t1_d = Instr(Opcode.vmvn, I32, D22_12, MS_28_16_0);
            var vmvn_t1_q = Instr(Opcode.vmvn, I32, Q22_12, MS_28_16_0);
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
                    Instr(Opcode.vmov, nyi("*immediate - T3")),
                    Instr(Opcode.vmvn, nyi("*immediate - T2"))),
                Mask(5, 1,  // op
                    Instr(Opcode.vorr, nyi("*immediate - T2")),
                    Instr(Opcode.vbic, nyi("*immediate - T2"))),
                Mask(5, 1,  // op
                    Instr(Opcode.vmov, nyi("*immediate - T3")),
                    Instr(Opcode.vmvn, nyi("*immediate - T2"))),
                Mask(5, 1,  // op
                    Instr(Opcode.vorr, q(6), vif, W22_12, Is(28,1,16,3,0,4)),
                    Instr(Opcode.vbic, nyi("*immediate - T2"))),

                Mask(5, 1,  // op
                    Instr(Opcode.vmov, nyi("*immediate - T4")),
                    Instr(Opcode.vmvn, nyi("*immediate - T3"))),
                Mask(5, 1,  // op
                    Instr(Opcode.vmov, nyi("*immediate - T4")),
                    Instr(Opcode.vmvn, nyi("*immediate - T3"))),
                Mask(5, 1,  // op
                    Instr(Opcode.vmov, nyi("*immediate - T4")),
                    Instr(Opcode.vmov, nyi("*immediate - T5"))),
                Mask(5, 1,  // op
                    Instr(Opcode.vmov, nyi("*immediate - T4")),
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
                    Instr(Opcode.vsli, nyi("*immediate"))),
                Nyi("AdvancedSimdTwoRegistersAndShiftAmount_opc6"),
                Mask(6, 1, // Q
                    Instr(Opcode.vqshl, VshImmSize, D22_12, D5_0, VshImm),
                    Instr(Opcode.vqshl, VshImmSize, Q22_12_times2, Q5_0_times2, VshImm)),

                Mask(12+16,1,     // U
                    Mask(12+16,0b11,     // L:Q
                        Instr(Opcode.vshrn, nyi("*AdvancedSimdTwoRegistersAndShiftAmount_opc8 U=0 L:Q=00")),
                        Nyi("AdvancedSimdTwoRegistersAndShiftAmount_opc8 U=0 L:Q=00"),
                        Nyi("AdvancedSimdTwoRegistersAndShiftAmount_opc8 U=0 L:Q=00"),
                        Nyi("AdvancedSimdTwoRegistersAndShiftAmount_opc8 U=0 L:Q=00")),
                    Nyi("AdvancedSimdTwoRegistersAndShiftAmount_opc8 U=1")),
                Mask(7, 1, // opc= 9 L
                    Mask(6, 1, //  L= 0 Q
                        Instr(Opcode.vqshrn, nyi("*signed result variant")),
                        Instr(Opcode.vqrshrn, nyi("D22_12,Q5_0,*signed result variant"))),   //$TODO hairy encoding.
                    invalid),
                Nyi("AdvancedSimdTwoRegistersAndShiftAmount_opcA"),
                Nyi("AdvancedSimdTwoRegistersAndShiftAmount_opcB"),

                Nyi("AdvancedSimdTwoRegistersAndShiftAmount_opcC"),
                Nyi("AdvancedSimdTwoRegistersAndShiftAmount_opcD"),
                Mask(7, 1, // L
                    Mask(6, 1, // Q
                        Instr(Opcode.vcvt, vC,D22_12,D5_0,Imm(minuend:64, fields:Bf((16,6)))),
                        Instr(Opcode.vcvt, vC,Q22_12,Q5_0,Imm(minuend:64, fields:Bf((16,6))))),
                    invalid),
                Nyi("AdvancedSimdTwoRegistersAndShiftAmount_opcF"));


            var AdvancedSimdShiftImm = Select(Bf((19,3),(7,1)), n => n == 0,
                AdvancedSimdOneRegisterAndModifiedImmediate,
                AdvancedSimdTwoRegistersAndShiftAmount);

            var AdvancedSimdDataProcessing = Mask(7 + 16, 1,
                AdvancedSimd3RegistersSameLength,
                Mask(4, 1,
                    AdvancedSimd2RegsOr3RegsDiffLength,
                    AdvancedSimdShiftImm));

            var SystemRegisterAccessAdvSimdFpu = Mask(12 + 16, 1, "SystemRegisterAccessAdvSimdFpu",
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
                    Instr(Opcode.qadd, nyi("*")),
                    Instr(Opcode.qdadd, R8,R0,R16),
                    Instr(Opcode.qsub, nyi("*")),
                    Instr(Opcode.qdsub, R8,R0,R16)),
                Mask(4, 3,
                    Instr(Opcode.rev, nyi("*")),
                    Instr(Opcode.rev16, nyi("*")),
                    Instr(Opcode.rbit, nyi("*")),
                    Instr(Opcode.revsh, nyi("*"))),
                Mask(4, 3,
                    Instr(Opcode.sel, nyi("*")),
                    invalid,
                    invalid,
                    invalid),
                Mask(4, 3,
                    Instr(Opcode.clz, R8,R0),
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
                    Instr(Opcode.sxtah, R8,R16,R0,SrBy8_4_2),
                    Instr(Opcode.sxth, R8,R0,SrBy8_4_2)),
                Select(w => SBitfield(w, 16, 4) != 0xF,
                    Instr(Opcode.uxtah, R8,R16,R0,SrBy8_4_2),
                    Instr(Opcode.uxth, R8,R0,SrBy8_4_2)),
                Select(w => SBitfield(w, 16, 4) != 0xF,
                    Instr(Opcode.sxtab16, R8,R16,R0,SrBy8_4_2),
                    Instr(Opcode.sxtb16, R8,R0,SrBy8_4_2)),
                Select(w => SBitfield(w, 16, 4) != 0xF,
                    Instr(Opcode.uxtab16, R8,R16,R0,SrBy8_4_2),
                    Instr(Opcode.uxtb16, R8,R0,SrBy8_4_2)),

                Select(w => SBitfield(w, 16, 4) != 0xF,
                    Instr(Opcode.sxtab, R8,R16,R0,SrBy8_4_2),
                    Instr(Opcode.sxtb, R8,R0,SrBy8_4_2)),
                Select(w => SBitfield(w, 16, 4) != 0xF,
                    Instr(Opcode.uxtab, R8,R16,R0,SrBy8_4_2),
                    Instr(Opcode.uxtb, R8,R0,SrBy8_4_2)),
                invalid,
                invalid);

            var ParallelAddSub = Mask(4 + 16, 7,
                Mask(4, 7,
                    Instr(Opcode.sadd8, Rnp8,Rnp16,Rnp0),
                    Instr(Opcode.qadd8, Rnp8,Rnp16,Rnp0),
                    Instr(Opcode.shadd8, Rnp8,Rnp16,Rnp0),
                    invalid,
                    Instr(Opcode.uadd8, Rnp8,Rnp16,Rnp0),
                    Instr(Opcode.uqadd8, Rnp8,Rnp16,Rnp0),
                    Instr(Opcode.uhadd8, Rnp8,Rnp16,Rnp0),
                    invalid),
                Mask(4, 7,
                    Instr(Opcode.sadd16, Rnp8,Rnp16,Rnp0),
                    Instr(Opcode.qadd16, Rnp8,Rnp16,Rnp0),
                    Instr(Opcode.shadd16, Rnp8,Rnp16,Rnp0),
                    invalid,
                    Instr(Opcode.uadd16, Rnp8,Rnp16,Rnp0),
                    Instr(Opcode.uqadd16, Rnp8,Rnp16,Rnp0),
                    Instr(Opcode.uhadd16, Rnp8,Rnp16,Rnp0),
                    invalid),
                Mask(4, 7,
                    Instr(Opcode.sasx, Rnp8,Rnp16,Rnp0),
                    Instr(Opcode.qasx, Rnp8,Rnp16,Rnp0),
                    Instr(Opcode.shasx, Rnp8,Rnp16,Rnp0),
                    invalid,
                    Instr(Opcode.uasx, Rnp8,Rnp16,Rnp0),
                    Instr(Opcode.uqasx, Rnp8,Rnp16,Rnp0),
                    Instr(Opcode.uhasx, Rnp8,Rnp16,Rnp0),
                    invalid),
                invalid,

                Mask(4, 7,
                    Instr(Opcode.ssub8, Rnp8,Rnp16,Rnp0),
                    Instr(Opcode.qsub8, Rnp8,Rnp16,Rnp0),
                    Instr(Opcode.shsub8, Rnp8,Rnp16,Rnp0),
                    invalid,
                    Instr(Opcode.usub8, Rnp8,Rnp16,Rnp0),
                    Instr(Opcode.uqsub8, Rnp8,Rnp16,Rnp0),
                    Instr(Opcode.uhsub8, Rnp8,Rnp16,Rnp0),
                    invalid),
                Mask(4, 7,
                    Instr(Opcode.ssub16, Rnp8,Rnp16,Rnp0),
                    Instr(Opcode.qsub16, Rnp8,Rnp16,Rnp0),
                    Instr(Opcode.shsub16, Rnp8,Rnp16,Rnp0),
                    invalid,
                    Instr(Opcode.usub16, Rnp8,Rnp16,Rnp0),
                    Instr(Opcode.uqsub16, Rnp8,Rnp16,Rnp0),
                    Instr(Opcode.uhsub16, Rnp8,Rnp16,Rnp0),
                    invalid),
                Mask(4, 7,
                    Instr(Opcode.ssax, Rnp8,Rnp16,Rnp0),
                    Instr(Opcode.qsax, Rnp8,Rnp16,Rnp0),
                    Instr(Opcode.shsax, Rnp8,Rnp16,Rnp0),
                    invalid,
                    Instr(Opcode.usax, Rnp8,Rnp16,Rnp0),
                    Instr(Opcode.uqsax, Rnp8,Rnp16,Rnp0),
                    Instr(Opcode.uhsax, Rnp8,Rnp16,Rnp0),
                    invalid),
                invalid);

            var MovMovsRegisterShiftedRegister = Mask(20, 1,
                Mask(5 + 16, 3,
                    Instr(Opcode.lsl, R8,R16,R0),
                    Instr(Opcode.lsr, R8,R16,R0),
                    Instr(Opcode.asr, R8,R16,R0),
                    Instr(Opcode.ror, R8,R16,R0)),
                Mask(5 + 16, 3,
                    Instr(Opcode.lsl, uf,R8,R16,R0),
                    Instr(Opcode.lsr, uf,R8,R16,R0),
                    Instr(Opcode.asr, uf,R8,R16,R0),
                    Instr(Opcode.ror, uf,R8,R16,R0)));

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

            var MultiplyAbsDifference = Mask(4 + 16, 7, "MultiplyAbsDifference",
                Mask(4, 3,
                    Select(w => SBitfield(w, 12, 4) != 0xF,
                        Instr(Opcode.mla, R8,R16,R0,R12),
                        Instr(Opcode.mul, R8,R16,R0)),
                    Instr(Opcode.mls, R8,R16,R0,R12),
                    invalid,
                    invalid),
                Mask(4, 3,      // op1 = 0b001
                    Select(w => SBitfield(w, 12, 4) != 0xF,
                        Instr(Opcode.smlabb, R8,R16,R0,R12),
                        Instr(Opcode.smulbb, R8,R16,R0)),
                    Select(w => SBitfield(w, 12, 4) != 0xF,
                        Instr(Opcode.smlabt, R8,R16,R0,R12),
                        Instr(Opcode.smulbt, R8,R16,R0)),
                    Select(w => SBitfield(w, 12, 4) != 0xF,
                        Instr(Opcode.smlatb, R8,R16,R0,R12),
                        Instr(Opcode.smultb, R8,R16,R0)),
                    Select(w => SBitfield(w, 12, 4) != 0xF,
                        Instr(Opcode.smlatt, R8,R16,R0,R12),
                        Instr(Opcode.smultt, R8,R16,R0))),
                Mask(4, 3,      // op1 = 0b010
                    Select(w => SBitfield(w, 12, 4) != 0xF,
                        Instr(Opcode.smlad, R8,R16,R0,R12),
                        Instr(Opcode.smuad, R8,R16,R0)),
                    Select(w => SBitfield(w, 12, 4) != 0xF,
                        Instr(Opcode.smladx, R8,R16,R0,R12),
                        Instr(Opcode.smuadx, R8,R16,R0)),
                    invalid,
                    invalid),
                Mask(4, 3,      // op1 = 0b011
                    Select(w => SBitfield(w, 12, 4) != 0xF,
                        Instr(Opcode.smlawb, R8,R16,R0,R12),
                        Instr(Opcode.smulwb, R8,R16,R0)),
                    Select(w => SBitfield(w, 12, 4) != 0xF,
                        Instr(Opcode.smlawt, nyi("*")),
                        Instr(Opcode.smulwt, nyi("*"))),
                    invalid,
                    invalid),
                Mask(4, 3, "op1 = 0b100",
                    Select(w => SBitfield(w, 12, 4) != 0xF,
                        Instr(Opcode.smlsd, R8,R16,R0,R12),
                        Instr(Opcode.smusd, R8,R16,R0)),
                    Select(w => SBitfield(w, 12, 4) != 0xF,
                        Instr(Opcode.smlsdx, R8,R16,R0,R12),
                        Instr(Opcode.smusdx, R8,R16,R0)),
                    invalid,
                    invalid),
                Mask(4, 3,      // op1 = 0b101
                    Select(w => SBitfield(w, 12, 4) != 0xF,
                        Instr(Opcode.smmla, R8,R16,R0,R12),
                        Instr(Opcode.smmul, Rnp8, Rnp16, Rnp0)),
                    Select(w => SBitfield(w, 12, 4) != 0xF,
                        Instr(Opcode.smmlar, Rnp8,Rnp16,Rnp0, R12),
                        Instr(Opcode.smmulr, Rnp8,Rnp16,Rnp0)),
                    invalid,
                    invalid),
                Mask(4, 3,      // op1 = 0b110
                    Instr(Opcode.smmls, Rnp8,Rnp16,Rnp0, R12),
                    Instr(Opcode.smmlsr, Rnp8,Rnp16,Rnp0, R12),
                    invalid,
                    invalid),
                Mask(4, 3,      // op1 = 0b111
                    Select(w => SBitfield(w, 12, 4) != 0xF,
                        Instr(Opcode.usada8, nyi("*")),
                        Instr(Opcode.usad8, nyi("*"))),
                    invalid,
                    invalid,
                    invalid));

            var MultiplyRegister = Select(w => SBitfield(w, 6, 2) == 0,
                MultiplyAbsDifference,
                invalid);

            var LongMultiplyDivide = Mask(4 + 16, 7, "LongMultiplyDivide",
                Select(w => SBitfield(w, 4, 4) != 0,
                    invalid,
                    Instr(Opcode.smull, R12,R8,R16,R0)),
                Select(w => SBitfield(w, 4, 4) != 0xF,
                    invalid,
                    Instr(Opcode.sdiv, R8,R16,R0)),
                Select(w => SBitfield(w, 4, 4) != 0,
                    invalid,
                    Instr(Opcode.umull, R12,R8,R16,R0)),
                Select(w => SBitfield(w, 4, 4) != 0xF,
                    invalid,
                    Instr(Opcode.udiv, R8,R16,R0)),
                // 4
                Mask(4, 0xF,
                    Instr(Opcode.smlal, R12,R8,R16,R0),
                    invalid,
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    Instr(Opcode.smlalbb, R12,R8,R16,R0),
                    Instr(Opcode.smlalbt, R12,R8,R16,R0),
                    Instr(Opcode.smlaltb, R12,R8,R16,R0),
                    Instr(Opcode.smlaltt, R12,R8,R16,R0),

                    Instr(Opcode.smlald, Rnp12,Rnp8,Rnp16,Rnp0),
                    Instr(Opcode.smlaldx, R12,R8,R16,R0),
                    invalid,
                    invalid),
                Mask(4, 0x0F, "LongMultiplyDivide op=4",
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

                    Instr(Opcode.smlsld, Rnp12,Rnp8,Rnp16,Rnp0),
                    Instr(Opcode.smlsldx, Rnp12,Rnp8,Rnp16,Rnp0),
                    invalid,
                    invalid),
                Mask(4, 0x0F,   // op1 = 0b110
                    Instr(Opcode.umlal, R12,R8,R16,R0),
                    invalid,
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    Instr(Opcode.umaal, nyi("*")),
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
                    Instr(Opcode.and, wide,R8,R16,R0,Si((4,2),Bf((12,3),(6,2)))),
                    Nyi("DataProcessingShiftedRegister_opc0 s=1")),
                Mask(20, 1,
                    Instr(Opcode.bic, wide,R8,R16,R0,Si((4,2),Bf((12,3),(6,2)))),
                    Instr(Opcode.bic, uf,wide,R8,R16,R0,Si((4,2),Bf((12,3),(6,2))))),
                Mask(20, 1,
                    Select((16,4), n => n != 15,
                        Instr(Opcode.orr, R8,R16,R0,Si((4,2),Bf((12,3),(6,2)))),
                        Instr(Opcode.mov, wide,R8,R0,Si((4,2),Bf((12,3),(6,2))))),
                    Select((16,4), n => n != 15,
                        Instr(Opcode.orr, nyi("*.")),
                        Instr(Opcode.mov, nyi("*.")))),
                Mask(20, 1,
                    Select((16,4), n => n != 15,
                        Instr(Opcode.orn, R8,R16,R0,Si((4,2),Bf((12,3),(6,2)))),
                        Instr(Opcode.mvn, uf, R8, R16, R0, Si((4, 2), Bf((12,3), (6, 2))))),
                    Select((16,4), n => n != 15,
                        Instr(Opcode.orn, nyi("*.")),
                        Instr(Opcode.mvn, nyi("*.")))),

                Nyi("DataProcessingShiftedRegister_opc4"),
                invalid,
                Mask(20, 1,
                    Mask(4, 3,
                        Instr(Opcode.pkhbt, nyi("*NYI")),
                        invalid,
                        Instr(Opcode.pkhtb, nyi("*NYI")),
                        invalid),
                    invalid),
                invalid,

                Mask(20, 1,
                    Select((16, 4), n => n != 13,
                        Instr(Opcode.add, wide,R8,R16,R0,Si((4,2),Bf((12,3),(6,2)))),
                        Instr(Opcode.add, nyi("S*"))),
                    Select((8,4), n => n != 15,
                        Select((16,4), n => n != 13,
                            Instr(Opcode.add, nyi(".*")),
                            Instr(Opcode.add, nyi(".S*"))),
                        Instr(Opcode.cmn, nyi("*register")))),
                invalid,
                Nyi("DataProcessingShiftedRegister_opcA"),
                Mask(20, 1,
                    Instr(Opcode.sbc, R8,R16,R0, Si((4,2),Bf((12,3),(6,2)))),
                    Instr(Opcode.sbc, uf,R8,R16,R0, Si((4,2), Bf((12,3), (6,2))))),

                invalid,
                Mask(20, 1,
                    Nyi("DataProcessingShiftedRegister_opcD s=0"),
                    Nyi("DataProcessingShiftedRegister_opcD s=1")),
                Mask(20, 1,
                    Instr(Opcode.rsb, R8,R16,R0,Si((4,2),Bf((12,3),(6,2)))),
                    Instr(Opcode.rsb, uf,R8,R16,R0,Si((4,2),Bf((12,3),(6,2))))),
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

                Select(Bf((24,1),(20,1)), n => n != 2,
                    LoadStoreSingle,
                    AdvancedSimdElementOrStructureLdSt),
                Mask(7 + 16, 3, "LongDecoder 23:2",
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
            var ldrd = Instr(Opcode.ldrd, R12, R8, MemOff(PrimitiveType.Word64, baseReg: Registers.pc, offsetShift:2, offsetFields: (0, 8)));

            var LoadAcquireStoreRelease = Mask(20, 1,
                Mask(4, 7,
                    Instr(Opcode.stlb, nyi("*")),
                    Instr(Opcode.stlh, nyi("*")),
                    Instr(Opcode.stl, nyi("*")),
                    invalid,

                    Instr(Opcode.stlexb, R0,R12,MemOff(PrimitiveType.Byte,16)),
                    Instr(Opcode.stlexh, R0,R12,MemOff(PrimitiveType.Word16,16)),
                    Instr(Opcode.stlex, R0,R12,MemOff(PrimitiveType.Word32,16)),
                    Instr(Opcode.stlexd, R0,R12,MemOff(PrimitiveType.Word64,16))),
                Mask(4, 7,
                    Instr(Opcode.ldab, nyi("*")),
                    Instr(Opcode.ldah, nyi("*")),
                    Instr(Opcode.lda, nyi("*")),
                    invalid,

                    Instr(Opcode.ldaexb, nyi("*")),
                    Instr(Opcode.ldaexh, nyi("*")),
                    Instr(Opcode.ldaex, R12,MemOff(PrimitiveType.Word32,16)),
                    Instr(Opcode.ldaexd, nyi("*"))));

            var ldStExclusive = Mask(20, 1,
                Instr(Opcode.strex, R8,R12,MemOff(PrimitiveType.Word32, 16, offsetShift:2, offsetFields:(0,8))),
                Instr(Opcode.ldrex, R12,MemOff(PrimitiveType.Word32, 16, offsetShift:2, offsetFields:(0,8))));

            var ldStDual = Mask(20, 1,
                Instr(Opcode.strd, R12,R8, MemOff(PrimitiveType.Word64, 16, offsetShift:2, indexSpec:idx24, offsetFields:(0,8))),
                Instr(Opcode.ldrd, R12,R8, MemOff(PrimitiveType.Word64, 16, offsetShift:2, indexSpec:idx24, offsetFields:(0,8))));
            var ldStDualImm = Mask(4 + 16, 1,
                Instr(Opcode.strd, R12,R8, MemOff(PrimitiveType.Word64, 16, offsetShift: 2, indexSpec: idx24, offsetFields: (0, 8))),
                Instr(Opcode.ldrd, R12,R8, MemOff(PrimitiveType.Word64, 16, offsetShift: 2, indexSpec: idx24, offsetFields: (0, 8))));
            var ldStDualPre = Mask(20, 1,
                Instr(Opcode.strd, MemOff(PrimitiveType.Word64, 16, offsetShift: 2, indexSpec: idx24, offsetFields: (0, 8))),
                Instr(Opcode.ldrd, MemOff(PrimitiveType.Word64, 16, offsetShift: 2, indexSpec: idx24, offsetFields: (0, 8))));

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
                            Instr(Opcode.tbb, MemIdx(PrimitiveType.Byte, 16, 0)),
                            Instr(Opcode.tbh, MemIdx(PrimitiveType.Word16, 16, 0)))),
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
            var branch_T3_variant = Instr(Opcode.b, PcRelative(1, Bf((26,1),(11,1),(13,1),(16,6),(0,11))));
            var branch_T4_variant = Instr(Opcode.b, B_T4);
            var branch = Nyi("Branch");

            var MiscellaneousSystem = Mask(4, 0xF,
                invalid,
                invalid,
                Instr(Opcode.clrex, nyi("*")),
                invalid,

                Instr(Opcode.dsb, B0_4),
                Instr(Opcode.dmb, B0_4),
                Instr(Opcode.isb, B0_4),
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
                    Instr(Opcode.nop, wide),
                    Instr(Opcode.yield, nyi("*")),
                    Instr(Opcode.wfe, nyi("*")),
                    Instr(Opcode.wfi, nyi("*")),

                    Instr(Opcode.sev, nyi("*")),
                    Instr(Opcode.sevl, nyi("*")),
                    Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint
                    Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint

                    Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint
                    Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint
                    Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint
                    Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint

                    Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint
                    Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint
                    Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint
                    Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear)), // Reserved hint
                Select((0, 4), n => n != 0, 
                    Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint
                    Instr(Opcode.esb, nyi("*"))),
                Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint
                Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint

                Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint
                Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint
                Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint
                Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint

                Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint
                Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint
                Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint
                Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint

                Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint
                Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint
                Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint
                Instr(Opcode.dbg, nyi("*")));

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
                                Instr(Opcode.msr, cpsr,R16),
                                Instr(Opcode.msr, spsr,R16)),
                            Instr(Opcode.msr, nyi("*banked register"))),
                        Mask(5, 1,  // op5
                            Instr(Opcode.msr, nyi("*register")),
                            Instr(Opcode.msr, nyi("*banked register"))),
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
                        Instr(Opcode.bxj, nyi("*")),
                        Nyi("ExceptionReturn"),
                        Mask(5, 1,  // op5
                            Mask(20, 1, // read spsr
                                Instr(Opcode.mrs, R8,cpsr),
                                Instr(Opcode.mrs, R8,spsr)),
                            Instr(Opcode.mrs, nyi("*banked register"))),
                        Mask(5, 1,  // op5
                            Instr(Opcode.mrs, nyi("*register")),
                            Instr(Opcode.mrs, nyi("*banked register")))),
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
