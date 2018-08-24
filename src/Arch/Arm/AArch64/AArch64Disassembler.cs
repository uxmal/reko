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

namespace Reko.Arch.Arm.AArch64
{
    using Mutator = Func<uint, AArch64Disassembler, bool>;

    public partial class AArch64Disassembler : DisassemblerBase<AArch64Instruction>
    {
        private const uint RegisterMask = 0b11111;

        private static readonly Decoder rootDecoder;
        private static readonly Decoder invalid;

        private Arm64Architecture arch;
        private EndianImageReader rdr;
        private Address addr;
        private DasmState state;

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
            this.state = new DasmState();
            var instr = rootDecoder.Decode(wInstr, this);
            instr.Address = addr;
            instr.Length = 4;
            return instr;
        }

        private class DasmState
        {
            public Opcode opcode;
            public List<MachineOperand> ops = new List<MachineOperand>();
            public Opcode shiftCode = Opcode.Invalid;
            public MachineOperand shiftAmount = null;

            public void Clear()
            {
                this.opcode = Opcode.Invalid;
                this.ops.Clear();
                this.shiftCode = Opcode.Invalid;
                this.shiftAmount = null;
            }
            public void Invalid()
            {
                Clear();
                opcode = Opcode.Invalid;
            }

            internal AArch64Instruction MakeInstruction()
            {
                var instr = new AArch64Instruction
                {
                    opcode = opcode,
                    ops = ops.ToArray(),
                    shiftCode = shiftCode,
                    shiftAmount = shiftAmount
                };
                return instr;

            }
        }

        private AArch64Instruction Decode(uint wInstr, Opcode opcode, string format)
        {
            int i = 0;
            RegisterStorage reg;
            int n;
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
                    state.ops.Add(new RegisterOperand(reg));
                    break;
                case 'X':
                    // 64-bit register.
                    n = ReadUnsignedBitField(wInstr, format, ref i);
                    reg = Registers.GpRegs64[n];
                    state.ops.Add(new RegisterOperand(reg));
                    break;
                case 'S':
                    // 32-bit SIMD/FPU register.
                    n = ReadUnsignedBitField(wInstr, format, ref i);
                    reg = Registers.SimdRegs32[n];
                    state.ops.Add(new RegisterOperand(reg));
                    break;
                case 'U':
                    ImmediateOperand op = DecodeImmediateOperand(wInstr, format, ref i);
                    if (op == null)
                        return Invalid();
                    state.ops.Add(op);
                    break;
                case 'I':
                    state.ops.Add(DecodeSignedImmediateOperand(wInstr, format, ref i));
                    break;
                case 'J':
                    // Jump displacement from address of current instruction
                    n = ReadSignedBitField(wInstr, format, ref i);
                    AddressOperand aop = AddressOperand.Create(addr + (n << 2));
                    state.ops.Add(aop);
                    break;

                case '[':
                    // Memory access
                    state.ops.Add(ReadMemoryAccess(wInstr, format, ref i));
                    break;
                case 'C':
                    // Condition field
                    var cop = ReadConditionField(wInstr, format, ref i);
                    state.ops.Add(new ConditionOperand(cop));
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
                            state.shiftCode = Opcode.lsl;
                            state.shiftAmount = ImmediateOperand.Int32(12);
                            break;
                        }
                        break;
                    case 'h': // 16-bit shifts
                        n = ReadUnsignedBitField(wInstr, format, ref i);
                        state.shiftCode = Opcode.lsl;
                        state.shiftAmount = ImmediateOperand.Int32(16 * n);
                        break;
                    case 'i': // code + immediate 
                        n = ReadUnsignedBitField(wInstr, format, ref i);
                        switch (n)
                        {
                        case 0: state.shiftCode = Opcode.lsl; break;
                        case 1: state.shiftCode = Opcode.lsr; break;
                        case 2: state.shiftCode = Opcode.asr; break;
                        case 3: state.shiftCode = Opcode.ror;  break;
                        }
                        Expect(',', format, ref i);
                        n = ReadUnsignedBitField(wInstr, format, ref i);
                        state.shiftAmount = ImmediateOperand.Int32(n);
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
                ops = state.ops.ToArray(),
                shiftCode = state.shiftCode,
                shiftAmount = state.shiftAmount
            };
            return instr;
        }

        /// <summary>
        /// 64-bit register.
        /// </summary>
        private static Action<List<MachineOperand>, AArch64Disassembler, uint> X(int regnumberOffset)
        {
            return (ops, dasm, w) =>
            {
                var reg = Registers.GpRegs64[(w >> regnumberOffset) & RegisterMask];
                ops.Add(new RegisterOperand(reg));
            };
        }


        private ArmCondition ReadConditionField(uint wInstr, string format, ref int i)
        {
            return (ArmCondition) ReadSignedBitField(wInstr, format, ref i);
        }

        private MemoryOperand ReadMemoryAccess(uint wInstr, string format, ref int i)
        {
            Expect('X', format, ref i);
            int n = ReadUnsignedBitField(wInstr, format, ref i);
            RegisterStorage regBase = Registers.GpRegs64[n];
            RegisterStorage regIndex = null;
            Constant offset = null;
            Opcode extend = Opcode.Invalid;
            int amount = 0;
            //Ms(xp,xs,Is,Ip)
            //Mu(xp,xs,Is,Ip)
            //MR(xp,xs,rs,rp)
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
                else if (PeekAndDiscard('R', format, ref i))
                {
                    var reg = ReadUnsignedBitField(wInstr, format, ref i);
                    var opt = (wInstr >> 13) & 7;
                    
                    switch (opt)
                    {
                    case 2:
                        regIndex = Registers.GpRegs32[reg];
                        extend = Opcode.uxtw; break;
                    case 3:
                        regIndex = Registers.GpRegs64[reg];
                        extend = Opcode.lsl; break;
                    case 6:
                        regIndex = Registers.GpRegs32[reg];
                        extend = Opcode.sxtw; break;
                    case 7:
                        regIndex = Registers.GpRegs64[reg];
                        extend = Opcode.sxtx; break;
                    }
                    var size = (wInstr >> 30) & 1;
                    if (size == 0) // 32-bit
                    {
                        amount = ((wInstr >> 12) & 1) != 0 ? 2 : 0;
                    }
                    else
                    {
                        amount = ((wInstr >> 12) & 1) != 0 ? 3 : 0;
                    }
                }
            }
            Expect(',', format, ref i);
            var dt = ReadBitSize(format, ref i);
            Expect(']', format, ref i);
            var preIndex = PeekAndDiscard('!', format, ref i);
            var postIndex = PeekAndDiscard('P', format, ref i);
            return new MemoryOperand(dt)
            {
                Base = regBase,
                Offset = offset,
                Index = regIndex,
                IndexExtend = extend,
                IndexShift = amount,
                PreIndex = preIndex,
                PostIndex = postIndex,
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
        private ImmediateOperand DecodeSignedImmediateOperand(uint wInstr, Bitfield[] fields, DataType dt, int sh =0 )
        {
            int n = Bitfield.ReadSignedFields(fields, wInstr);
            if (sh > 0)
            {
                n <<= sh;
            }
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
            case 'b': return PrimitiveType.Byte;
            case 'h': return PrimitiveType.Word16;
            case 'w': return PrimitiveType.Word32;
            case 'l': return PrimitiveType.Word64;
            case 'q': return PrimitiveType.Word128;
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









        // 32-bit register.
        private static Mutator W(int pos, int size) {
            var fields = new[]
            {
                new Bitfield(pos, size)
            };
            return (u, d) =>
            {
                uint iReg = Bitfield.ReadFields(fields, u);
                d.state.ops.Add(new RegisterOperand(Registers.GpRegs32[iReg]));
                return true;
            };
        }

        // 64-bit register.
        private static Mutator X(int pos, int size)
        {
            var fields = new[]
            {
                new Bitfield(pos, size)
            };
            return (u, d) =>
            {
                uint iReg = Bitfield.ReadFields(fields, u);
                d.state.ops.Add(new RegisterOperand(Registers.GpRegs64[iReg]));
                return true;
            };
        }

        // 8-bit SIMD register.
        private static Mutator B(int pos, int size)
        {
            var fields = new[]
            {
                new Bitfield(pos, size)
            };
            return (u, d) =>
            {
                uint iReg = Bitfield.ReadFields(fields, u);
                d.state.ops.Add(new RegisterOperand(Registers.SimdRegs8[iReg]));
                return true;
            };
        }

        // 16-bit SIMD register.
        private static Mutator H(int pos, int size)
        {
            var field = new Bitfield(pos, size);
            return (u, d) =>
            {
                uint iReg = field.Read(u);
                d.state.ops.Add(new RegisterOperand(Registers.SimdRegs16[iReg]));
                return true;
            };
        }

        // 32-bit SIMD/FPU register.
        private static Mutator S(int pos, int size)
        {
            var fields = new[]
            {
                new Bitfield(pos, size)
            };
            return (u, d) =>
            {
                uint iReg = Bitfield.ReadFields(fields, u);
                d.state.ops.Add(new RegisterOperand(Registers.SimdRegs32[iReg]));
                return true;
            };
        }

        // 64-bit SIMD register.
        private static Mutator D(int pos, int size)
        {
            var field = new Bitfield(pos, size);
            return (u, d) =>
            {
                uint iReg = field.Read(u);
                d.state.ops.Add(new RegisterOperand(Registers.SimdRegs64[iReg]));
                return true;
            };
        }

        // 128-bit SIMD register.
        private static Mutator Q(int pos, int size)
        {
            var fields = new[]
            {
                new Bitfield(pos, size)
            };
            return (u, d) =>
            {
                uint iReg = Bitfield.ReadFields(fields, u);
                d.state.ops.Add(new RegisterOperand(Registers.SimdRegs128[iReg]));
                return true;
            };
        }




        // Extended register, depending on the option field.
        private static Mutator Rx(int pos, int size, int optionPos, int optionSize)
        {
            var regField = new Bitfield(pos, size);
            var optionField = new Bitfield(optionPos, optionSize);
            return (u, d) =>
            {
                var iReg = regField.Read(u);
                var opt = optionField.Read(u);
                var reg = (opt == 0b011 || opt == 0b111)
                    ? Registers.GpRegs64[iReg]
                    : Registers.GpRegs32[iReg];
                d.state.ops.Add(new RegisterOperand(reg));
                return true;
            };
        }

        // Extension to apply.
        private static Mutator Ex(int posOption, int sizeOption, int posSh, int sizeSh)
        {
            var optionField = new Bitfield(posOption, sizeOption);
            var shField = new Bitfield(posSh, sizeSh);
            return (u, d) =>
            {
                var opt = optionField.Read(u);
                var sh = shField.Read(u);
                Opcode ext = Opcode.Invalid;
                switch (opt)
                {
                case 0: ext = Opcode.uxtb; break; 
                case 1: ext = Opcode.uxth; break;
                case 2: ext = Opcode.uxtw; break;
                case 3: ext = Opcode.uxtx; break;
                case 4: ext = Opcode.sxtb; break;
                case 5: ext = Opcode.sxth; break;
                case 6: ext = Opcode.sxtw; break;
                case 7: ext = Opcode.sxtx; break;
                }
                d.state.shiftCode = ext;
                d.state.shiftAmount = ImmediateOperand.Int32((int)sh);
                return true;
            };
        }

 

        // Unsigned immediate
        private static Mutator U(int pos, int size, PrimitiveType dt)
        {
            //ImmediateOperand op = DecodeImmediateOperand(wInstr, format, ref i);
            //if (op == null)
            //    return Invalid();
            //state.ops.Add(op);
            throw new NotImplementedException();
        }

        // Signed immediate
        private static Mutator I(int pos, int size, PrimitiveType dt, int sh = 0)
        {
            var fields = new Bitfield[]
            {
                new Bitfield(pos, size)
            };
            return (u, d) =>
            {
                var i = d.DecodeSignedImmediateOperand(u, fields, dt, sh);
                d.state.ops.Add(i);
                return true;
            };
        }

        // Jump displacement from address of current instruction
        private static Mutator J(int pos, int size)
        {
            var fields = new Bitfield[]
            {
                new Bitfield(pos, size)
            };
            return (u, d) =>
            {
                var n = Bitfield.ReadSignedFields(fields, u);
                AddressOperand aop = AddressOperand.Create(d.addr + (n << 2));
                d.state.ops.Add(aop);
                return true;
            };
        }

        // Scaled immediate offset
        private static Mutator Mo(PrimitiveType dt, int baseRegOff, int posOff, int lenOff)
        {
            var offsetField = new Bitfield(posOff, lenOff);
            int shift = ShiftFromSize(dt);
            return (u, d) =>
            {
                var iReg = (u >> baseRegOff) & 0x1F;
                var baseReg = Registers.AddrRegs64[iReg];
                var offset = offsetField.Read(u);
                offset <<= shift;
                var mem = new MemoryOperand(dt)
                {
                    Base = baseReg,
                    Offset = Constant.Int64(offset)
                };
                d.state.ops.Add(mem);
                return true;
            };
        }

        private static int ShiftFromSize(PrimitiveType dt)
        {
            int shift = 0;
            switch (dt.Size)
            {
            case 1: shift = 0; break;
            case 2: shift = 1; break;
            case 4: shift = 2; break;
            case 8: shift = 3; break;
            case 16: shift = 4; break;
            }

            return shift;
        }

        // Unscaled immediate offset
        private static Mutator Mu(PrimitiveType dt, int baseRegOff, int posOff, int lenOff)
        {
            var offsetField = new Bitfield(posOff, lenOff);
            return (u, d) =>
            {
                var iReg = (u >> baseRegOff) & 0x1F;
                var baseReg = Registers.AddrRegs64[iReg];
                var offset = (int)Bits.SignExtend(offsetField.Read(u), offsetField.Length);
                var mem = new MemoryOperand(dt)
                {
                    Base = baseReg,
                    Offset = Constant.Int64(offset)
                };
                d.state.ops.Add(mem);
                return true;
            };
        }


        private static Mutator Mpost(PrimitiveType dt)
        {
            return (u, d) =>
            {
                var mem = new MemoryOperand(dt);
                var iReg = (u >> 5) & 0x1F;
                mem.Base = Registers.AddrRegs64[iReg];

                int offset = (int)Bits.SignExtend(u >> 12, 9);
                mem.Offset = offset != 0 ? Constant.Int32(offset) : null;
                mem.PostIndex = true;
                d.state.ops.Add(mem);
                return true;
            };
        }

        private static Mutator Mpre(PrimitiveType dt)
        {
            return (u, d) =>
            {
                var mem = new MemoryOperand(dt);
                var iReg = (u >> 5) & 0x1F;
                mem.Base = Registers.AddrRegs64[iReg];

                int offset = (int)Bits.SignExtend(u >> 12, 9);
                mem.Offset = offset != 0 ? Constant.Int32(offset) : null;
                mem.PreIndex = true;
                d.state.ops.Add(mem);
                return true;
            };
        }

        private static Mutator Mlit(PrimitiveType dt)
        {
            return (u, d) =>
            {
                int offset = (int)Bits.SignExtend(u >> 5, 19) << 2;
                var addr = d.addr + offset;
                d.state.ops.Add(AddressOperand.Create(addr));
                return true;
            };
        }

        // [Xn,Xn] or [Xn,Wn,sxtb] indexed mode
        private static Mutator Mr(PrimitiveType dt)
        {
            var sh = ShiftFromSize(dt);
            return (u, d) =>
            {
                var mem = new MemoryOperand(dt);
                var iReg = (u >> 5) & 0x1F;
                mem.Base = Registers.AddrRegs64[iReg];

                iReg = (u >> 16) & 0x1F;
                var option = (u >> 13) & 0x7;
                mem.Index = ((option & 1) == 1 ? Registers.GpRegs64 : Registers.GpRegs32)[iReg];

                switch (option)
                {
                case 2: mem.IndexExtend = Opcode.uxtw; break;
                case 3: mem.IndexExtend = Opcode.lsl; break;
                case 6: mem.IndexExtend = Opcode.sxtw; break;
                case 7: mem.IndexExtend = Opcode.sxtx; break;
                default: d.state.Invalid(); return false;
                }
                sh = (int)((u >> 12) & 1) * sh;
                mem.IndexShift = sh;
                d.state.ops.Add(mem);
                return true;
            };
        }

        private static Mutator C(int pos, int size)
        {
            var field = new Bitfield(pos, size);
            return (u, d) =>
            {
                var cond = (ArmCondition)field.Read(u);
                d.state.ops.Add(new ConditionOperand(cond));
                return true;
            };
        }

        //    case 's':
        //        // Shift type
        //        switch (format[i++])
        //        {
        //        case 'c': // code
        //            n = ReadUnsignedBitField(wInstr, format, ref i);
        //            switch (n)
        //            {
        //            case 1:
        //                state.shiftCode = Opcode.lsl;
        //                state.shiftAmount = ImmediateOperand.Int32(12);
        //                break;
        //            }
        //            break;
        //        case 'h': // 16-bit shifts
        //            n = ReadUnsignedBitField(wInstr, format, ref i);
        //            state.shiftCode = Opcode.lsl;
        //            state.shiftAmount = ImmediateOperand.Int32(16 * n);
        //            break;
        private static Mutator si(int pos1, int len1, int pos2, int len2)
        {
            var bfShtype = new Bitfield(pos1, len1);
            var bfShamt = new Bitfield(pos2, len2);
            return (u, d) =>
            {
                var n = bfShtype.Read(u);
                switch (n)
                {
                case 0: d.state.shiftCode = Opcode.lsl; break;
                case 1: d.state.shiftCode = Opcode.lsr; break;
                case 2: d.state.shiftCode = Opcode.asr; break;
                case 3: d.state.shiftCode = Opcode.ror; break;
                }
                n = bfShamt.Read(u);
                d.state.shiftAmount = ImmediateOperand.Int32((int)n);
                return true;
            };
        }

        //        case 'i': // code + immediate 
        //            n = ReadUnsignedBitField(wInstr, format, ref i);
        //            switch (n)
        //            {
        //            case 0: state.shiftCode = Opcode.lsl; break;
        //            case 1: state.shiftCode = Opcode.lsr; break;
        //            case 2: state.shiftCode = Opcode.asr; break;
        //            case 3: state.shiftCode = Opcode.ror; break;
        //            }
        //            Expect(',', format, ref i);
        //            n = ReadUnsignedBitField(wInstr, format, ref i);
        //            state.shiftAmount = ImmediateOperand.Int32(n);
        //            break;
        //        default:
        //            NotYetImplemented($"Unknown format character '{format[i - 1]}' in '{format}' decoding {opcode} shift", wInstr);
        //            break;
        //        }
        //        break;
        //    default:
        //        NotYetImplemented($"Unknown format character '{format[i - 1]}' in '{format}' decoding {opcode}", wInstr);
        //        return Invalid();
        //    }
        //}
        private static Mutator Bm(int posS, int posR)
        {
            return (u, d) =>
            {
                var imms = (int)(u >> posS) & 0x3F;
                var immr = (int)(u >> posR) & 0x3F;
                uint n = (u >> 22) & 1;
                if ((u & 0x80000000u) == 0 && n == 1)
                {
                    return false;
                }
                d.state.ops.Add(ImmediateOperand.Int32(immr));
                d.state.ops.Add(ImmediateOperand.Int32(imms));
                return true;
            };
        }


        private static Mutator x(string message)
        {
            return (u, d) =>
            {
                var op = d.state.opcode.ToString();
                string m;
                if (message == "")
                    m = op;
                else
                    m = $"{op} - {message}";
                d.NotYetImplemented(m, u);
                d.Invalid();
                return false;
            };
        }

        private static PrimitiveType i8 => PrimitiveType.SByte;
        private static PrimitiveType i16 => PrimitiveType.Int16;
        private static PrimitiveType i32 => PrimitiveType.Int32;
        private static PrimitiveType w8 => PrimitiveType.Byte;
        private static PrimitiveType w16 => PrimitiveType.Word16;
        private static PrimitiveType w32 => PrimitiveType.Word32;
        private static PrimitiveType w64 => PrimitiveType.Word64;
        private static PrimitiveType w128 => PrimitiveType.Word128;





        private static Decoder Instr(Opcode opcode, string format)
        {
            return new InstrDecoder(opcode, format);
        }

        private static Decoder Instr(Opcode opcode, params Mutator [] mutators)
        {
            return new InstrDecoder2(opcode, mutators);
        }

        private static Decoder Mask(int pos, uint mask, params Decoder[] decoders)
        {
            return new MaskDecoder(pos, mask, decoders);
        }

        private static Decoder Mask(
            int pos1, int length1,
            int pos2, int length2,
            params Decoder[] decoders)
        {
            var bitfields = new[]
            {
                new Bitfield(pos1, length1),
                new Bitfield(pos2, length2),
            };
            return new BitfieldDecoder(bitfields, decoders);
        }

        private static Decoder Mask(
            int pos1, int length1,
            int pos2, int length2,
            int pos3, int length3,
            params Decoder[] decoders)
        {
            var bitfields = new[]
            {
                new Bitfield(pos1, length1),
                new Bitfield(pos2, length2),
                new Bitfield(pos3, length3),
            };
            return new BitfieldDecoder(bitfields, decoders);
        }


        private static Decoder Sparse(int pos, uint mask, Decoder @default, params (uint, Decoder)[] decoders)
        {
            return new SparseMaskDecoder(pos, mask, decoders.ToDictionary(k => k.Item1, v => v.Item2), @default);
        }

        private static Decoder Select(int pos, int length, Predicate<int> predicate, Decoder trueDecoder, Decoder falseDecoder)
        {
            var bitfields = new[]
            {
                new Bitfield(pos, length)
            };
            return new SelectDecoder(bitfields, predicate, trueDecoder, falseDecoder);
        }

        private static Decoder Select(
            int pos1, int length1,
            int pos2, int length2,
            Predicate<int> predicate, Decoder trueDecoder, Decoder falseDecoder)
        {
            var bitfields = new[]
            {
                new Bitfield(pos1, length1),
                new Bitfield(pos2, length2),
            };
            return new SelectDecoder(bitfields, predicate, trueDecoder, falseDecoder);
        }

        private static Decoder Select(
            int pos1, int length1,
            int pos2, int length2,
            int pos3, int length3,
            Predicate<int> predicate, Decoder trueDecoder, Decoder falseDecoder)
        {
            var bitfields = new[]
            {
                new Bitfield(pos1, length1),
                new Bitfield(pos2, length2),
                new Bitfield(pos3, length3),
            };
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
                LdStRegUImm = Mask(30,2, 26,1, 22,2, // size V opc
                    Instr(Opcode.strb, W(0,5), Mo(i8,5, 10,12)),
                    Instr(Opcode.ldrb, W(0,5), Mo(i8,5, 10,12)),
                    Instr(Opcode.ldrsb, X(0,5), Mo(i8,5, 10,12)),
                    Instr(Opcode.ldrsb, W(0,5), Mo(i8,5, 10,12)),
                    // 00 1 00
                    Instr(Opcode.str, B(0,5), Mo(w8, 5, 10, 12)),
                    Instr(Opcode.ldr, B(0,5), Mo(w8, 5, 10, 12)),
                    Instr(Opcode.str, Q(0,5), Mo(w128, 5, 10, 12)),
                    Instr(Opcode.ldr, Q(0,5), Mo(w128, 5, 10, 12)),
                    // 01 0 00
                    Instr(Opcode.strh, W(0, 5), Mo(w16, 5, 10, 12)),
                    Instr(Opcode.ldrh, W(0, 5), Mo(w16, 5, 10, 12)),
                    Instr(Opcode.ldrsh, X(0, 5), Mo(i16, 5, 10, 12)),
                    Instr(Opcode.ldrsh, W(0, 5), Mo(i16, 5, 10, 12)),
                    // 01 1 00
                    Nyi("LdStRegUImm size V opc = 01 1 00"),
                    Nyi("LdStRegUImm size V opc = 01 1 01"),
                    invalid,
                    invalid,
                    // 10 0 00
                    Instr(Opcode.str, W(0, 5), Mo(w32, 5, 10, 12)),
                    Instr(Opcode.ldr, W(0, 5), Mo(w32, 5, 10, 12)),
                    Instr(Opcode.ldrsw, X(0, 5), Mo(i16, 5, 10, 12)),
                    invalid,
                    // 10 1 00
                    Instr(Opcode.str, "*immediate SIMD&FP 32-bit"),
                    Instr(Opcode.ldr, "*immediate SIMD&FP 32-bit"),
                    invalid,
                    invalid,
                    // 11 0 00
                    Instr(Opcode.str, "X0:5,[X5:5,U10:12l<3,l]"),
                    Instr(Opcode.ldr, "X0:5,[X5:5,U10:12l<3,l]"),
                    Instr(Opcode.prfm, "*"),
                    invalid,
                    // 11 1 00
                    Instr(Opcode.str, x("immediate SIMD&FP 32-bit")),
                    Instr(Opcode.ldr, x("immediate SIMD&FP 32-bit")),
                    invalid,
                    invalid);
            }

            Decoder LdStRegisterRegOff;
            {
                LdStRegisterRegOff = Mask(14, 1,
                    invalid,
                    Mask(30, 2, 26, 1, 22, 2,   // //LoadStoreRegisterRegOff sz V opc
                        Instr(Opcode.strb, W(0,5),Mr(w8)),
                        Instr(Opcode.ldrb, W(0,5),Mr(w8)),
                        Instr(Opcode.ldrsb, x("register - 64-bit Extended variant")),
                        Instr(Opcode.ldrsb, x("register - 32-bit Extended variant")),

                        // LoadStoreRegisterRegOff sz:V:opc=00 1 00
                        Instr(Opcode.str, x("register - SIMD&FP")),
                        Instr(Opcode.ldr, x("register - SIMD&FP")),
                        Instr(Opcode.str, x("register - SIMD&FP")),
                        Instr(Opcode.ldr, x("register - SIMD&FP")),

                        // LoadStoreRegisterRegOff sz:V:opc=01 0 00
                        Instr(Opcode.strh, x("register")),
                        Instr(Opcode.ldrh, W(0,5),Mr(w16)),
                        Instr(Opcode.ldrsh, x("register - 64-bit")),
                        Instr(Opcode.ldrsh, x("register - 32-bit")),

                        // LoadStoreRegisterRegOff sz:V:opc=01 1 00
                        Instr(Opcode.str, x("register - SIMD&FP")),
                        Instr(Opcode.ldr, x("register - SIMD&FP")),
                        invalid,
                        invalid,

                        // LoadStoreRegisterRegOff sz:V:opc=10 0 00
                        Instr(Opcode.str, "W0:5,[X5:5,R16:5,w]"),
                        Instr(Opcode.ldr, "W0:5,[X5:5,R16:5,w]"),
                        Instr(Opcode.ldrsw, "W0:5,[X5:5,R16:5,w]"),
                        invalid,

                        // LoadStoreRegisterRegOff sz:V:opc=10 1 00
                        Instr(Opcode.str, x("register - SIMD&FP")),
                        Instr(Opcode.ldr, x("register - SIMD&FP")),
                        invalid,
                        invalid,

                        // LoadStoreRegisterRegOff sz:V:opc=11 0 00
                        Instr(Opcode.str, "X0:5,[X5:5,R16:5,l]"),
                        Instr(Opcode.ldr, "X0:5,[X5:5,R16:5,l]"),
                        Instr(Opcode.prfm, x("register")),
                        invalid,

                        // LoadStoreRegisterRegOff sz:V:opc=11 1 00
                        Instr(Opcode.str, x("register - SIMD&FP")),
                        Instr(Opcode.ldr, x("register - SIMD&FP")),
                        invalid,
                        invalid));

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
                    Mask(26, 1, // V
                        Mask(22, 1,  // L
                            Instr(Opcode.stp, "X0:5,X10:5,[X5:5,I15:7<2l,q]"),
                            Instr(Opcode.ldp, "X0:5,X10:5,[X5:5,I15:7<2l,q]")),
                        Mask(22, 1,  // L
                            Instr(Opcode.stp, "*SIMD&FP - 128bit"),
                            Instr(Opcode.ldp, "*SIMD&FP - 128bit"))),
                    invalid);
            }

            Decoder LdStRegPairPre;
            {
                LdStRegPairPre = Mask(30, 3,     // opc
                    Nyi("LdStRegPairPre opc=0b00"),
                    Nyi("LdStRegPairPre opc=0b01"),
                    Mask(26, 1, // V
                        Mask(22, 1, // L
                            Instr(Opcode.stp, "X0:5,X10:5,[X5:5,I15:7<2l,q]!"),
                            Instr(Opcode.ldp, "X0:5,X10:5,[X5:5,I15:7<2l,q]!")),
                        Mask(22, 1, // L
                            Instr(Opcode.stp, "*SIMD&FP - 128bit"),
                            Instr(Opcode.ldp, "*SIMD&FP - 128bit"))),
                    Nyi("LdStRegPairPre opc=0b11"));
            }

            Decoder LdStRegPairPost;
            {
                LdStRegPairPost = Mask(30, 3,     // opc
                    Nyi("LdStRegPairPre opc=0b00"),
                    Nyi("LdStRegPairPre opc=0b01"),
                    Mask(26, 1, // V
                        Mask(22, 1, // L
                            Instr(Opcode.stp, "X0:5,X10:5,[X5:5,I15:7<2l,q]P"),
                            Instr(Opcode.ldp, "X0:5,X10:5,[X5:5,I15:7<2l,q]P")),
                        Mask(22, 1, // L
                            Instr(Opcode.stp, "*SIMD&FP - 128bit"),
                            Instr(Opcode.ldp, "*SIMD&FP - 128bit"))),
                    Nyi("LdStRegPairPre opc=0b11"));
            }

            Decoder LdStNoallocatePair = Nyi("LdStNoallocatePair");

            Decoder LoadsAndStores;
            {
                var LdStRegUnscaledImm = Mask(30, 2, 26, 1, 22, 2,
                    Instr(Opcode.sturb, W(0, 5), Mu(w8, 5, 12, 9)),
                    Instr(Opcode.ldurb, W(0, 5), Mu(w8, 5, 12, 9)),
                    Instr(Opcode.ldursb, X(0, 5), Mu(i8, 5, 12, 9)),
                    Instr(Opcode.ldursb, W(0, 5), Mu(i8, 5, 12, 9)),

                    // LdStRegUnscaledImm size=00 V=1 opc=00
                    Instr(Opcode.stur, B(0,5), Mu(w8,5,12,9)),
                    Instr(Opcode.ldur, B(0,5), Mu(w8,5,12,9)),
                    Instr(Opcode.stur, Q(0,5), Mu(w128,5,12,9)),
                    Instr(Opcode.ldur, Q(0,5), Mu(w128,5,12,9)),

                    // LdStRegUnscaledImm size=01 V=0 opc=00
                    Instr(Opcode.sturh, W(0, 5), Mo(w16, 5, 12, 9)),
                    Instr(Opcode.ldurh, W(0, 5), Mo(w16, 5, 12, 9)),
                    Instr(Opcode.ldursh, X(0,5), Mu(i16,5,12,9)),
                    Instr(Opcode.ldursh, W(0,5), Mu(i16,5,12,9)),

                    // LdStRegUnscaledImm size=01 V=1 opc=00
                    Instr(Opcode.stur, H(0,5), Mu(w16,5,12,9)),
                    Instr(Opcode.ldur, H(0,5), Mu(w16,5,12,9)),
                    invalid,
                    invalid,

                    // LdStRegUnscaledImm size=10 V=0 opc=00
                    Instr(Opcode.stur, W(0,5), Mu(w32,5,12,9)),
                    Instr(Opcode.ldur, W(0,5), Mu(w32,5,12,9)),
                    Instr(Opcode.ldursw, X(0,5), Mu(w32,5,12,9)),
                    invalid,

                    // LdStRegUnscaledImm size=10 V=1 opc=00
                    Instr(Opcode.stur, S(0,5), Mu(w32,5,12,9)),
                    Instr(Opcode.ldur, S(0,5), Mu(w32,5,12,9)),
                    invalid,
                    invalid,

                    // LdStRegUnscaledImm size=11 V=0 opc=00
                    Instr(Opcode.stur, X(0,5), Mu(w64,5,12,9)),
                    Instr(Opcode.ldur, X(0,5), Mu(w64,5,12,9)),
                    Instr(Opcode.prfm, x("unscaled offset")),
                    invalid,

                    // LdStRegUnscaledImm size=11 V=0 opc=00
                    Instr(Opcode.stur, D(0,5), Mu(w64,5,12,9)),
                    Instr(Opcode.ldur, D(0,5), Mu(w64,5,12,9)),
                    invalid,
                    invalid);

                Decoder LdStRegImmPostIdx;
                {
                    LdStRegImmPostIdx = Mask(30, 2, 26, 1, 22, 2,
                        Instr(Opcode.strb, W(0,5), Mpost(w8)),
                        Instr(Opcode.ldrb, W(0,5), Mpost(w8)),
                        Instr(Opcode.ldrsb, X(0,5), Mpost(i8)),
                        Instr(Opcode.ldrsb, W(0,5), Mpost(i8)),

                        Nyi("LdStRegImmPostIdx size:V:opc = 00 1 00"),
                        Nyi("LdStRegImmPostIdx size:V:opc = 00 1 01"),
                        Nyi("LdStRegImmPostIdx size:V:opc = 00 1 10"),
                        Nyi("LdStRegImmPostIdx size:V:opc = 00 1 11"),

                        Nyi("LdStRegImmPostIdx size:V:opc = 01 0 00"),
                        Nyi("LdStRegImmPostIdx size:V:opc = 01 0 01"),
                        Nyi("LdStRegImmPostIdx size:V:opc = 01 0 10"),
                        Nyi("LdStRegImmPostIdx size:V:opc = 01 0 11"),

                        Nyi("LdStRegImmPostIdx size:V:opc = 01 1 00"),
                        Nyi("LdStRegImmPostIdx size:V:opc = 01 1 01"),
                        Nyi("LdStRegImmPostIdx size:V:opc = 01 1 10"),
                        Nyi("LdStRegImmPostIdx size:V:opc = 01 1 11"),

                        Instr(Opcode.str, W(0,5), Mpost(w32)),
                        Instr(Opcode.ldr, W(0,5), Mpost(w32)),
                        Instr(Opcode.ldrsw, X(0,5), Mpost(i32)),
                        invalid,

                        Nyi("LdStRegImmPostIdx size:V:opc = 10 1 00"),
                        Nyi("LdStRegImmPostIdx size:V:opc = 10 1 01"),
                        Nyi("LdStRegImmPostIdx size:V:opc = 10 1 10"),
                        Nyi("LdStRegImmPostIdx size:V:opc = 10 1 11"),

                        Instr(Opcode.str, X(0,5), Mpost(w64)),
                        Instr(Opcode.ldr, X(0,5), Mpost(w64)),
                        invalid,
                        invalid,

                        Instr(Opcode.str, X(0,5), x("postidx SIMD&FP 64-bit")),
                        Instr(Opcode.ldr, X(0,5), x("postidx SIMD&FP 64-bit")),
                        invalid,
                        invalid);
                }

            var LdStRegUnprivileged = Nyi("LdStRegUnprivileged");

                Decoder LdStRegImmPreIdx;
                {
                    LdStRegImmPreIdx = Mask(30, 2, 26, 1, 22, 2,
                        Instr(Opcode.strb, W(0, 5), Mpre(w8)),
                        Instr(Opcode.ldrb, W(0, 5), Mpre(w8)),
                        Instr(Opcode.ldrsb, X(0, 5), Mpre(i8)),
                        Instr(Opcode.ldrsb, W(0, 5), Mpre(i8)),

                        Instr(Opcode.str, B(0, 5), Mpre(w8)),
                        Instr(Opcode.ldr, B(0, 5), Mpre(w8)),
                        Instr(Opcode.str, Q(0, 5), Mpre(w128)),
                        Instr(Opcode.ldr, Q(0, 5), Mpre(w128)),

                        Instr(Opcode.strh, W(0, 5), Mpre(w16)),
                        Instr(Opcode.ldrh, W(0, 5), Mpre(w16)),
                        Instr(Opcode.ldrsh, X(0, 5), Mpre(i16)),
                        Instr(Opcode.ldrsh, W(0, 5), Mpre(i16)),

                        Instr(Opcode.str, H(0, 5), Mpre(w16)),
                        Instr(Opcode.ldr, H(0, 5), Mpre(w16)),
                        invalid,
                        invalid,

                        Instr(Opcode.str, W(0, 5), Mpre(w32)),
                        Instr(Opcode.ldr, W(0, 5), Mpre(w32)),
                        Instr(Opcode.ldrsw, X(0, 5), Mpre(i32)),
                        invalid,

                        Instr(Opcode.str, S(0, 5), Mpre(w32)),
                        Instr(Opcode.ldr, S(0, 5), Mpre(w32)),
                        invalid,
                        invalid,

                        Instr(Opcode.str, X(0,5), Mpre(w64)),
                        Instr(Opcode.ldr, X(0,5), Mpre(w64)),
                        invalid,
                        invalid,

                        Instr(Opcode.str, D(0,5), Mpre(w64)),
                        Instr(Opcode.ldr, D(0,5), Mpre(w64)),
                        invalid,
                        invalid);
                }

                Decoder LoadRegLit;
                {
                    LoadRegLit = Mask(30,2,26,1,    // opc:V
                        Instr(Opcode.ldr, W(0,5), Mlit(w32)),
                        Instr(Opcode.ldr, S(0,5), Mlit(w32)),
                        Instr(Opcode.ldr, X(0,5), Mlit(w64)),
                        Instr(Opcode.ldr, D(0,5), Mlit(w64)),
                        Instr(Opcode.ldrsw, X(0,5), Mlit(i32)),
                        Instr(Opcode.ldr, Q(0,5), Mlit(w128)),
                        Instr(Opcode.prfm, x("literal")),
                        invalid);
                }

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
                            LoadRegLit,
                            LoadRegLit,
                            invalid,
                            invalid),
                        new MaskDecoder(23, 3,      // op0 = 0, op1 = 2
                            LdStNoallocatePair,
                            LdStRegPairPost,
                            LdStRegPairOffset,
                            LdStRegPairPre),
                        Mask(23, 3, // op0 = 0, op1 = 3
                            Mask(21, 1,     // LdSt op0 = 0, op1 = 3, op3 = 0, high bit of op4
                                Mask(10, 0x3, 
                                    LdStRegUnscaledImm,
                                    LdStRegImmPostIdx,
                                    LdStRegUnprivileged,
                                    LdStRegImmPreIdx),
                                Mask(10, 3, // op1 = 3, op3 = 0x, op4=1xxxx
                                    Nyi("*AtomicMemoryOperations"),
                                    Nyi("*LoadStoreRegister PAC"),
                                    LdStRegisterRegOff,
                                    Nyi("*LoadStoreRegister PAC"))),
                            Nyi("LdSt op0 = 0, op1 = 3, op3 = 1"),
                            LdStRegUImm,
                            LdStRegUImm)),
                    new MaskDecoder(28, 3,          // op0 = 1 
                        Nyi("op1 = 0"),
                        Nyi("op1 = 1"),
                        Mask(23, 3, // op1 = 2 op3
                            LdStNoallocatePair,
                            LdStRegPairPost,
                            LdStRegPairOffset,
                            LdStRegPairPre),
                        Mask(24, 1,
                            Mask(21, 1,     // high bit of op4
                                Mask(10, 3, // LoadsAndStores op1 = 3, op3 = 0x, op4=0xxxx
                                    LdStRegUnscaledImm,
                                    LdStRegImmPostIdx,
                                    LdStRegUnprivileged,
                                    LdStRegImmPreIdx),
                                Mask(10, 3, // LoadsAndStores op1 = 3, op3 = 0x, op4=1xxxx
                                    Nyi("*AtomicMemoryOperations"),
                                    Nyi("*LoadStoreRegister PAC"),
                                    LdStRegisterRegOff,
                                    Nyi("*LoadStoreRegister PAC"))),
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
                    Instr(Opcode.movn, "W0:5,U5:16w sh21:2"),
                    invalid),
                invalid,
                Mask(22, 1,
                    Instr(Opcode.movz, "W0:5,U5:16w sh21:2"),
                    invalid),
                Mask(22, 1,
                    Instr(Opcode.movk, "W0:5,U5:16h sh21:2"),
                    invalid),

                Instr(Opcode.movn, "X0:5,U5:16l sh21:2"),
                invalid,
                Instr(Opcode.movz, "X0:5,U5:16l sh21:2"),
                Instr(Opcode.movk, "X0:5,U5:16h sh21:2"));


            var PcRelativeAddressing = Mask(31, 1,
                Instr(Opcode.adr, "*"),
                Instr(Opcode.adrp, "X0:5,I5:19:29:2<12w"));

            Decoder Bitfield;
            {
                Bitfield = Mask(22, 1,
                    Mask(29, 7,
                        Instr(Opcode.sbfm, W(0,5),W(5,5),Bm(10,16)),
                        Instr(Opcode.bfm, W(0,5),W(5,5),Bm(10,16)),
                        Instr(Opcode.ubfm, W(0,5),W(5,5),Bm(10,16)),
                        invalid,

                        invalid,
                        Instr(Opcode.Invalid, "*BOGOTRON"),
                        invalid,
                        invalid),
                    Mask(29, 7,
                        invalid,
                        invalid,
                        invalid,
                        invalid,

                        Instr(Opcode.sbfm, X(0,5),X(5,5),I(16,6,i32),I(10,6,i32)),
                        Instr(Opcode.bfm, X(0,5),X(5,5),I(16,6,i32),I(10,6,i32)),
                        Instr(Opcode.ubfm, X(0,5),X(5,5),I(16,6,i32),I(10,6,i32)), //$BUG: l h, look at encoding
                        invalid));
            }
            Decoder Extract = Nyi("Extract");

            var DataProcessingImm = new MaskDecoder(23, 0x7,
                PcRelativeAddressing,
                PcRelativeAddressing,
                AddSubImmediate,
                AddSubImmediate,

                LogicalImmediate,
                MoveWideImmediate,
                Bitfield,
                Extract);

            var UncondBranchImm = Mask(31, 1,
                Instr(Opcode.b, J(0,26)),
                Instr(Opcode.bl, J(0,26)));

            var UncondBranchReg = Select(16,5, n => n != 0x1F,
                invalid,
                Mask(21, 0xF,
                    Sparse(10, 6,
                        invalid,
                        (0, Select(0,5, n => n == 0, Instr(Opcode.br, "X5:5"), invalid)),
                        (2, Select(0,5, n => n == 0x1F, Nyi("BRAA,BRAAZ... Key A"), invalid)),
                        (3, Select(0,5, n => n == 0x1F, Nyi("BRAA,BRAAZ... Key B"), invalid))),
                    Sparse(10, 6,
                        invalid,
                        (0, Select(0,5, n => n == 0, Instr(Opcode.blr, "X5:5"), invalid)),
                        (2, Select(0,5, n => n == 0x1F, Nyi("BlRAA,BlRAAZ... Key A"), invalid)),
                        (3, Select(0,5, n => n == 0x1F, Nyi("BlRAA,BlRAAZ... Key B"), invalid))),
                    Sparse(10, 6,
                        invalid,
                        (0, Select(0,5, n => n == 0, Instr(Opcode.ret, "X5:5"), invalid)),
                        (2, Select(0,5, n => n == 0x1F, Nyi("RETAA,RETAAZ... Key A"), invalid)),
                        (3, Select(0,5, n => n == 0x1F, Nyi("RETAA,RETAAZ... Key B"), invalid))),
                    invalid,

                    Select(5,5, n => n == 0x1F,
                        Sparse(10, 6,
                            invalid,
                            (0, Select(0,5, n => n == 0, Instr(Opcode.eret, ""), invalid)),
                            (2, Select(0,5, n => n == 0x1F, Nyi("ERETAA,RETAAZ... Key A"), invalid)),
                            (3, Select(0,5, n => n == 0x1F, Nyi("ERETAA,RETAAZ... Key B"), invalid))),
                        invalid),
                    Select(10,6,5,5,0,5, n => n == 0b000000_11111_00000,
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

            var CompareBranchImm = Mask(31, 1, 
                Mask(24, 1,
                    Instr(Opcode.cbz, "W0:5,J5:19"),
                    Instr(Opcode.cbnz, "W0:5,J5:19")),
                Mask(24, 1,
                    Instr(Opcode.cbz, "X0:5,J5:19"),
                    Instr(Opcode.cbnz, "X0:5,J5:19")));

            var TestBranchImm = Mask(24, 1,
                Mask(31, 1,
                    Instr(Opcode.tbz, "W0:5,I19:5w,J5:14"),
                    Instr(Opcode.tbnz, "W0:5,I19:5w,J5:14")),
                Mask(31, 1,
                    Instr(Opcode.tbz, "W0:5,I19:5w,J5:14"),
                    Instr(Opcode.tbnz, "W0:5,I19:5w,J5:14")));

            var CondBranchImm = Mask(24,1,4,1,
                Instr(Opcode.b, C(0,4),J(5,19)),
                invalid,
                invalid,
                invalid);

            var System = Mask(19, 7,  // L:op0
                Mask(16, 7,  // System L:op0 = 0b000
                    Nyi("System L:op0 = 0b000 op1=0b000"),
                    Nyi("System L:op0 = 0b000 op1=0b001"),
                    Nyi("System L:op0 = 0b000 op1=0b010"),
                    Mask(12, 0xF, // System L:op0 = 0b000 op1=0b011
                        Nyi("System L:op0 = 0b000 op1=0b011 crN=0000"),
                        Nyi("System L:op0 = 0b000 op1=0b011 crN=0001"),
                        Mask(8, 0xF, // System L:op0 = 0b000 op1=0b011 crN=0010 crM
                            Mask(5,7, // System L:op0 = 0b000 op1=0b011 crN=0010 crM=0000 op2
                                Select(0,5, n => n == 0x1F, Instr(Opcode.nop, ""), invalid),
                                Select(0,5, n => n == 0x1F, Instr(Opcode.yield, "*"), invalid),
                                Select(0,5, n => n == 0x1F, Instr(Opcode.wfe, "*"), invalid),
                                Select(0,5, n => n == 0x1F, Instr(Opcode.wfi, "*"), invalid),

                                Select(0,5, n => n == 0x1F, Instr(Opcode.sev, "*"), invalid),
                                Select(0,5, n => n == 0x1F, Instr(Opcode.sevl, "*"), invalid),
                                Nyi("System L:op0 = 0b000 op1=0b011 crN=0010 crM=0000 op2=110"),
                                Nyi("System L:op0 = 0b000 op1=0b011 crN=0010 crM=0000 op2=111")),
                            Nyi("System L:op0 = 0b000 op1=0b011 crN=0010 crM=0001"),
                            Nyi("System L:op0 = 0b000 op1=0b011 crN=0010 crM=0010"),
                            Nyi("System L:op0 = 0b000 op1=0b011 crN=0010 crM=0011"),

                            Nyi("System L:op0 = 0b000 op1=0b011 crN=0010 crM=0100"),
                            Nyi("System L:op0 = 0b000 op1=0b011 crN=0010 crM=0110"),
                            Nyi("System L:op0 = 0b000 op1=0b011 crN=0010 crM=0101"),
                            Nyi("System L:op0 = 0b000 op1=0b011 crN=0010 crM=0111"),

                            Nyi("System L:op0 = 0b000 op1=0b011 crN=0010 crM=1000"),
                            Nyi("System L:op0 = 0b000 op1=0b011 crN=0010 crM=1001"),
                            Nyi("System L:op0 = 0b000 op1=0b011 crN=0010 crM=1010"),
                            Nyi("System L:op0 = 0b000 op1=0b011 crN=0010 crM=1011"),

                            Nyi("System L:op0 = 0b000 op1=0b011 crN=0010 crM=1100"),
                            Nyi("System L:op0 = 0b000 op1=0b011 crN=0010 crM=1101"),
                            Nyi("System L:op0 = 0b000 op1=0b011 crN=0010 crM=1110"),
                            Nyi("System L:op0 = 0b000 op1=0b011 crN=0010 crM=1111")),
                        Nyi("System L:op0 = 0b000 op1=0b011 crN=0011"),

                        Nyi("System L:op0 = 0b000 op1=0b011 crN=0100"),
                        Nyi("System L:op0 = 0b000 op1=0b011 crN=0110"),
                        Nyi("System L:op0 = 0b000 op1=0b011 crN=0101"),
                        Nyi("System L:op0 = 0b000 op1=0b011 crN=0111"),

                        Nyi("System L:op0 = 0b000 op1=0b011 crN=1000"),
                        Nyi("System L:op0 = 0b000 op1=0b011 crN=1001"),
                        Nyi("System L:op0 = 0b000 op1=0b011 crN=1010"),
                        Nyi("System L:op0 = 0b000 op1=0b011 crN=1011"),

                        Nyi("System L:op0 = 0b000 op1=0b011 crN=1100"),
                        Nyi("System L:op0 = 0b000 op1=0b011 crN=1101"),
                        Nyi("System L:op0 = 0b000 op1=0b011 crN=1110"),
                        Nyi("System L:op0 = 0b000 op1=0b011 crN=1111")),
                    Nyi("System L:op0 = 0b000 op1=0b100"),
                    Nyi("System L:op0 = 0b000 op1=0b101"),
                    Nyi("System L:op0 = 0b000 op1=0b110"),
                    Nyi("System L:op0 = 0b000 op1=0b111")),
                Nyi("System L:op0 = 0b001"),
                Nyi("System L:op0 = 0b010"),
                Nyi("System L:op0 = 0b011"),

                Nyi("System L:op0 = 0b100"),
                Nyi("System L:op0 = 0b101"),
                Nyi("System L:op0 = 0b110"),
                Nyi("System L:op0 = 0b111"));

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
                    Select(15,1, n => n == 1,
                        invalid,
                        Mask(29,2,21,1,
                            Instr(Opcode.and, "W0:5,W5:5,W16:5 si22:2,10:6"),
                            Instr(Opcode.bic, "*shifted register, 32-bit"),
                            Select(22,2,10,6,5,5, n => n == 0x1F,
                                Instr(Opcode.mov, "W0:5,W16:5 si22:2,10:6"),
                                Instr(Opcode.orr, "W0:5,W5:5,W16:5 si22:2,10:6")),
                            Instr(Opcode.orn, "*shifted register, 32-bit"),

                            Instr(Opcode.eor, "*shifted register, 32-bit"),
                            Instr(Opcode.eon, "*shifted register, 32-bit"),
                            Select(0,5, n => n == 0x1F,
                                Instr(Opcode.test, W(5,5),W(16,5),si(22,2,10,6)),
                                Instr(Opcode.ands, W(0,5),W(5,5),W(16,5),si(22,2,10,6))),
                            Instr(Opcode.bics, "*shifted register, 32-bit"))),
                    Mask(29,2,21,1,
                        Instr(Opcode.and, "X0:5,X5:5,X16:5 si22:2,10:6"),
                        Instr(Opcode.bic, "*shifted register, 64-bit"),
                        Select(22,2,10,6,5,5, n => n == 0x1F,
                            Instr(Opcode.mov, "X0:5,X16:5 si22:2,10:6"),
                            Instr(Opcode.orr, "X0:5,X5:5,X16:5 si22:2,10:6")),
                        Instr(Opcode.orn, "*shifted register, 64-bit"),

                        Instr(Opcode.eor, "*shifted register, 64-bit"),
                        Instr(Opcode.eon, "*shifted register, 64-bit"),
                        Select(0,5, n => n == 0x1F,
                            Instr(Opcode.test, X(5,5),X(16,5),si(22,2,10,6)),
                            Instr(Opcode.ands, X(0,5),X(5,5),X(16,5),si(22,2,10,6))),
                        Instr(Opcode.bics, "*shifted register, 64-bit")));
            }
            Decoder AddSubShiftedRegister;
            {
                AddSubShiftedRegister = Mask(31,1,  // size
                    Select(15,1, n => n == 1,
                        invalid,
                        Mask(29, 3,
                            Instr(Opcode.add, "W0:5,W5:5,W16:5 si22:2,10:6"),
                            Instr(Opcode.adds, "W0:5,W5:5,W16:5 si22:2,10:6"),
                            Instr(Opcode.sub, "W0:5,W5:5,W16:5 si22:2,10:6"),
                            Select(0, 5, n => n == 0x1F,
                                Instr(Opcode.cmp, W(5,5),W(16,5),si(22,2,10,6)),
                                Instr(Opcode.subs, W(0,5),W(5,5),W(16,5),si(22,2,10,6))))),
                    Mask(29, 3,
                        Instr(Opcode.add, "X0:5,X5:5,X16:5 si22:2,10:6"),
                        Instr(Opcode.adds, "X0:5,X5:5,X16:5 si22:2,10:6"),
                        Instr(Opcode.sub, "X0:5,X5:5,X16:5 si22:2,10:6"),
                        Instr(Opcode.subs, "X0:5,X5:5,X16:5 si22:2,10:6")));
            }

            var AddSubExtendedRegister = Select(22, 2, n => n != 0,
                invalid,
                Mask(29, 0b111,
                    Instr(Opcode.add, x("add (32 extended register)")),
                    Instr(Opcode.adds, x("adds (32 extended register)")),
                    Instr(Opcode.sub, W(0,5),W(5,5),Rx(16,5,13,3),Ex(13,3,10,3)),
                    Select(0,5, n => n == 0x1F,
                        Instr(Opcode.cmp, W(5,5),Rx(16,5,13,3),Ex(13,3,10,3)),
                        Instr(Opcode.subs, W(0,5),W(5,5),Rx(16,5,13,3),Ex(13,3,10,3))),

                    Instr(Opcode.add, X(0,5),X(5,5),Rx(16,5,13,3),Ex(13,3,10,3)),
                    Instr(Opcode.adds, x("adds (64 extended register)")),
                    Instr(Opcode.sub, x("sub (64 extended register)")),
                    Select(0,5, n => n == 0x1F,
                        Instr(Opcode.cmp, X(5,5),Rx(16,5,13,3),Ex(13,3,10,3)),
                        Instr(Opcode.subs, X(0,5),X(5,5),Rx(16,5,13,3),Ex(13,3,10,3)))));

            Decoder DataProcessing3Source;
            {
                DataProcessing3Source = Mask(29, 0b111,
                    Mask(21, 0x7,
                        Mask(15, 1,
                            Select(10, 5, n => n == 0x1F,
                                Instr(Opcode.mul, W(0,5),W(5,5),W(16,5)),
                                Instr(Opcode.madd, W(0,5),W(5,5),W(16,5),W(10,5))),
                            Select(10, 5, n => n == 0x1F,
                                Instr(Opcode.mneg, W(0,5),W(5,5),W(16,5)),
                                Instr(Opcode.msub, W(0,5),W(5,5),W(16,5),W(10,5)))),
                        invalid,
                        invalid,
                        invalid,

                        invalid,
                        invalid,
                        invalid,
                        invalid),
                    invalid,
                    invalid,
                    invalid,

                    Mask(21, 0x7,
                        Mask(15, 1,
                            Select(10, 5, n => n == 0x1F,
                                Instr(Opcode.mul, X(0,5),X(5,5),X(16,5)),
                                Instr(Opcode.madd, X(0,5),X(5,5),X(16,5),X(10,5))),
                            Select(10, 5, n => n == 0x1F,
                                Instr(Opcode.mneg, X(0,5),X(5,5),X(16,5)),
                                Instr(Opcode.msub, X(0,5),X(5,5),X(16,5),X(10,5)))),
                        Mask(15, 1,
                            Instr(Opcode.smaddl, x("")),
                            Instr(Opcode.smsubl, x(""))),
                        Mask(15, 1,
                            Instr(Opcode.smulh, x("")),
                            invalid),
                        invalid,

                        invalid,
                        Mask(15, 1,
                            Instr(Opcode.umaddl, x("")),
                            Instr(Opcode.umsubl, x(""))),
                        Mask(15, 1,
                            Instr(Opcode.umulh, x("")),
                            invalid),
                        invalid),
                    invalid,
                    invalid,
                    invalid);
            }

            Decoder ConditionalSelect;
            {
                ConditionalSelect = Mask(29, 7,
                    Mask(10, 3,
                        Instr(Opcode.csel, "W0:5,W5:5,W16:5,C12:4"),
                        Instr(Opcode.csinc, "W0:5,W5:5,W16:5,C12:4"),
                        invalid,
                        invalid),
                    invalid,
                    Mask(10, 3,
                        Instr(Opcode.csinv, "W0:5,W5:5,W16:5,C12:4"),
                        Instr(Opcode.csneg, "W0:5,W5:5,W16:5,C12:4"),
                        invalid,
                        invalid),
                    invalid,
                    Mask(10, 3,
                        Instr(Opcode.csel, "X0:5,X5:5,X16:5,C12:4"),
                        Instr(Opcode.csinc, "X0:5,X5:5,X16:5,C12:4"),
                        invalid,
                        invalid),
                    invalid,
                    Mask(10, 3,
                        Instr(Opcode.csinv, "X0:5,X5:5,X16:5,C12:4"),
                        Instr(Opcode.csneg, "X0:5,X5:5,X16:5,C12:4"),
                        invalid,
                        invalid),
                    invalid);
            }

            Decoder ConditionalCompareImm;
            {
                ConditionalCompareImm = Select(10,1,4,1, n => n != 0,
                    invalid,
                    Mask(29, 7,
                        invalid,
                        Instr(Opcode.ccmn, "* 32=bit"),
                        invalid,
                        Instr(Opcode.ccmp, "* 32-bit"),
                        invalid,
                        Instr(Opcode.ccmn, "* - 64-bit"),
                        invalid,
                        Instr(Opcode.ccmp, "X5:5,U16:1l,U0:4b,C12:4")));
            }

            Decoder DataProcessing2source;
            {
                DataProcessing2source = Mask(31, 1, 29, 1,
                    Mask(12,0b1111,
                        Mask(10, 0b11, // sf:S=0:0 opcode=0000xx
                            invalid,
                            Nyi("* Data Processing 2 source - sf:S=0:0 opcode=000001"),
                            Nyi("* Data Processing 2 source - sf:S=0:0 opcode=000010"),
                            Instr(Opcode.sdiv, W(0,5),W(5,5),W(16,5))),
                        Nyi("* Data Processing 2 source - sf:S=0:0 opcode=0001xx"),
                        Nyi("* Data Processing 2 source - sf:S=0:0 opcode=0010xx"),
                        Nyi("* Data Processing 2 source - sf:S=0:0 opcode=0011xx"),

                        Nyi("* Data Processing 2 source - sf:S=0:0 opcode=0100xx"),
                        Nyi("* Data Processing 2 source - sf:S=0:0 opcode=0101xx"),
                        Nyi("* Data Processing 2 source - sf:S=0:0 opcode=0110xx"),
                        Nyi("* Data Processing 2 source - sf:S=0:0 opcode=0111xx"),

                        invalid,
                        invalid,
                        invalid,
                        invalid,

                        invalid,
                        invalid,
                        invalid,
                        invalid),

                    Nyi("* Data Processing 2 source - sf:S=0:1"),
                    Nyi("* Data Processing 2 source - sf:S=1:0"),
                    Nyi("* Data Processing 2 source - sf:S=1:1"));
            }

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
                            ConditionalCompareImm),
                        invalid,

                        ConditionalSelect,
                        invalid,
                        Mask(30, 1,         // op1 = 1, op2 = 6, op0
                            DataProcessing2source,
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

            Decoder DataProcessingScalarFpAdvancedSimd;
            {
                DataProcessingScalarFpAdvancedSimd = Mask(28, 0xF,
                    Nyi("DataProcessingScalarFpAdvancedSimd - op0"),
                    Nyi("DataProcessingScalarFpAdvancedSimd - op1"),
                    Nyi("DataProcessingScalarFpAdvancedSimd - op2"),
                    Nyi("DataProcessingScalarFpAdvancedSimd - op3"),

                    Nyi("DataProcessingScalarFpAdvancedSimd - op4"),
                    Nyi("DataProcessingScalarFpAdvancedSimd - op5"),
                    Mask(23, 3, // DataProcessingScalarFpAdvancedSimd - op6
                        Nyi("DataProcessingScalarFpAdvancedSimd - op6 op1=0"),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op6 op1=1"),
                        Mask(19, 0xF,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op6 op1=2 op2=0"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op6 op1=2 op2=1"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op6 op1=2 op2=2"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op6 op1=2 op2=3"),
                            
                            Nyi("DataProcessingScalarFpAdvancedSimd - op6 op1=2 op2=4"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op6 op1=2 op2=5"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op6 op1=2 op2=6"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op6 op1=2 op2=7"),
                            
                            Nyi("DataProcessingScalarFpAdvancedSimd - op6 op1=2 op2=8"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op6 op1=2 op2=9"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op6 op1=2 op2=A"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op6 op1=2 op2=B"),

                            Nyi("DataProcessingScalarFpAdvancedSimd - op6 op1=2 op2=C"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op6 op1=2 op2=D"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op6 op1=2 op2=E"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op6 op1=2 op2=F")),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op6 op1=3")),
                    Nyi("DataProcessingScalarFpAdvancedSimd - op7"),

                    Nyi("DataProcessingScalarFpAdvancedSimd - op8"),
                    Nyi("DataProcessingScalarFpAdvancedSimd - op9"),
                    Nyi("DataProcessingScalarFpAdvancedSimd - opA"),
                    Nyi("DataProcessingScalarFpAdvancedSimd - opB"),

                    Nyi("DataProcessingScalarFpAdvancedSimd - opC"),
                    Nyi("DataProcessingScalarFpAdvancedSimd - opD"),
                    Nyi("DataProcessingScalarFpAdvancedSimd - opE"),
                    Nyi("DataProcessingScalarFpAdvancedSimd - opF"));
            }

            rootDecoder = new MaskDecoder(25, 0x0F,
                invalid,
                invalid,
                invalid,
                invalid,

                LoadsAndStores,
                DataProcessingReg,
                LoadsAndStores,
                DataProcessingScalarFpAdvancedSimd,
                
                DataProcessingImm,
                DataProcessingImm,
                BranchesExceptionsSystem,
                BranchesExceptionsSystem,
                
                LoadsAndStores,
                DataProcessingReg,
                LoadsAndStores,
                DataProcessingScalarFpAdvancedSimd);
        }

    }
}
