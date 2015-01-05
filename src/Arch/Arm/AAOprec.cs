#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Core.Expressions;
using Decompiler.Core.Machine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Decompiler.Arch.Arm
{
     internal abstract class OpDecoder
     {
          public abstract AArch64Instruction Decode(AArch64Disassembler dasm, uint opcode) ;
     }

    internal class AAOprec : OpDecoder
    {
        private AA64Opcode opcode;
        protected string format;

        public AAOprec(AA64Opcode opcode, string fmt)
        {
            this.opcode = opcode;
            this.format = fmt;
        }

        public override AArch64Instruction Decode(AArch64Disassembler dasm, uint instr)
        {
            return Decode(dasm, instr, format);
        }

        public AArch64Instruction Decode(AArch64Disassembler dasm, uint instr, string fmt)
        {
            AA64Opcode opcode = this.opcode;
            List<MachineOperand> ops = new List<MachineOperand>();
            MachineOperand op = null;
            int i = 0;
            int off;
            while (i < fmt.Length)
            {
                switch (fmt[i++])
                {
                default: throw new InvalidOperationException(string.Format("Bad format character {0}.", fmt[i - 1]));
                case ',':
                    continue;
                case 'B':   // Logical Bitmask
                    op = LogicalBitmask(instr, fmt[i++] == 'x');
                    break;
                case 'H':   // 16-bit Immediate constant
                    off = GetOffset(fmt, ref i);
                    op = new ImmediateOperand(Constant.Word32(GetImm(instr, off, 16)));
                    break;
                case 'I':   // 12-bit Immediate constant
                    off = GetOffset(fmt, ref i);
                    op = new ImmediateOperand(Constant.Word32(GetImm(instr, off, 12)));
                    break;
                case 'J':   // long relative branch
                    int offset = (((int) instr) << 6) >> 4;
                    op = new AddressOperand(dasm.rdr.Address + offset);
                    break;
                case 'W':   // (32-bit) W register operand
                    op = GetWReg(instr, GetOffset(fmt, ref i));
                    break;
                case 'X':   // (64-bit) X register operand 
                    op = GetXReg(instr, GetOffset(fmt, ref i));
                    break;
                case 's':   // Shift operand by 12
                    off = GetOffset(fmt, ref i);
                    op = ops[ops.Count - 1];
                    ops.RemoveAt(ops.Count - 1);
                    uint shiftCode = GetImm(instr, off, 2);
                    switch (shiftCode)
                    {
                    case 0: break;
                    case 1:
                        op = new ShiftOperand(op, Opcode.lsl, 12); break;
                    default: throw new FormatException("Reserved value for shift code.");
                    }
                    break;
                }
                ops.Add(op);
            }
            return AArch64Instruction.Create(opcode, ops);
        }

        private int GetOffset(string fmt, ref int i)
        {
            int offset = 0;
            while (i < fmt.Length)
            {
                char ch = fmt[i];
                if (!Char.IsDigit(ch))
                    return offset;
                offset = offset * 10 + (ch - '0');
                ++i;
            }
            return offset;
        }

        private uint GetImm(uint instr, int off, int length)
        {
            return (instr >> off) & ((1u << length) - 1u);
        }

        private MachineOperand GetXReg(uint instr, int regPos)
        {
            int reg = (int) ((instr >> regPos) & 0x1F);
            return new RegisterOperand(A64Registers.GetXReg(reg));
        }

        private MachineOperand GetWReg(uint instr, int regPos)
        {
            int reg = (int) ((instr >> regPos) & 0x1F);
            return new RegisterOperand(A64Registers.GetWReg(reg));
        }

    // https://sourceware.org/git/gitweb.cgi?p=binutils-gdb.git;a=blob;f=opcodes/aarch64-dis.c;h=c11f78f78ab8569db6ebf5295d8047aeeee338bd;hb=master
    // line 694

        /* Decode logical immediate for e.g. ORR <Wd|WSP>, <Wn>, #<imm>.  */

        ImmediateOperand LogicalBitmask(uint value, bool is64)
        {
            ulong imm, mask;
            uint N, R, S;
            uint simd_size;

            // value = extract_fields (code, 0, 3, FLD_N, FLD_immr, FLD_imms);
            //sf = aarch64_get_qualifier_esize (inst->operands[0].qualifier) != 4;

            /* value is N:immr:imms.  */
            S = (value >> 10) & 0x3Fu;
            R = (value >> 16) & 0x3F;
            N = (value >> 22) & 0x1;

            if (!is64 && N == 1)
                throw new InvalidOperationException();

            /* The immediate value is S+1 bits to 1, left rotated by SIMDsize - R
               (in other words, right rotated by R), then replicated.  */
            if (N != 0)
            {
                simd_size = 64;
                mask = 0xFFFFFFFFFFFFFFFFul;
            }
            else
            {
                switch (S)
                {
                case 0x00:
                case 0x01:
                case 0x02:
                case 0x03:
                case 0x04:
                case 0x05:
                case 0x06:
                case 0x07:
                case 0x08:
                case 0x09:
                case 0x0A:
                case 0x0B:
                case 0x0C:
                case 0x0D:
                case 0x0E:
                case 0x0F:
                case 0x10:
                case 0x11:
                case 0x12:
                case 0x13:
                case 0x14:
                case 0x15:
                case 0x16:
                case 0x17:
                case 0x18:
                case 0x19:
                case 0x1A:
                case 0x1B:
                case 0x1C:
                case 0x1D:
                case 0x1E:
                case 0x1F:
                    /* 0xxxxx */
                    simd_size = 32;
                    break;
                case 0x20:
                case 0x21:
                case 0x22:
                case 0x23:
                case 0x24:
                case 0x25:
                case 0x26:
                case 0x27:
                case 0x28:
                case 0x29:
                case 0x2A:
                case 0x2B:
                case 0x2C:
                case 0x2D:
                case 0x2E:
                case 0x2F:
                    /* 10xxxx */
                    simd_size = 16; S &= 0xf; break;
                case 0x30:
                case 0x31:
                case 0x32:
                case 0x33:
                case 0x34:
                case 0x35:
                case 0x36:
                case 0x37:
                    /* 110xxx */
                    simd_size = 8; S &= 0x7; break;
                case 0x38:
                case 0x39:
                case 0x3A:
                case 0x3B:
                    /* 1110xx */
                    simd_size = 4; S &= 0x3; break;
                case 0x3C:
                case 0x3D:
                case 0x3E:
                case 0x3F:
                    /* 11110x */
                    simd_size = 2; S &= 0x1; break;
                default: throw new InvalidOperationException();
                }
                mask = ((1ul << (int) simd_size) - 1);
                /* Top bits are IGNORED.  */
                R &= simd_size - 1;
            }
            /* NOTE: if S = simd_size - 1 we get 0xf..f which is rejected.  */
            if (S == simd_size - 1)
                throw new InvalidOperationException();
            /* S+1 consecutive bits to 1.  */
            /* NOTE: S can't be 63 due to detection above.  */
            imm = (1ul << (int) (S + 1)) - 1;
            /* Rotate to the left by simd_size - R.  */
            if (R != 0)
                imm = ((imm << (int) (simd_size - R)) & mask) | (imm >> (int) R);
            /* Replicate the value according to SIMD size.  */
            switch (simd_size)
            {
            case 2: imm = (imm << 2) | imm; goto case 4;
            case 4: imm = (imm << 4) | imm; goto case 8;
            case 8: imm = (imm << 8) | imm; goto case 16;
            case 16: imm = (imm << 16) | imm; goto case 32;
            case 32: imm = (imm << 32) | imm; break;
            case 64: break;
            default: throw new InvalidOperationException();
            }

            return is64
                ? ImmediateOperand.Word64(imm)
                : ImmediateOperand.Word32((int) imm);
        }
    }

    internal class LogMovImmOprec : OpDecoder
    {
        private AAOprec logical;
        private AAOprec mov;

        internal LogMovImmOprec(AA64Opcode opcodeLog, string fmtLog, AA64Opcode opcodeMov, string fmtMov)
        {
            this.logical = new AAOprec(opcodeLog, fmtLog);
            this.mov = new AAOprec(opcodeMov, fmtMov);
        }

        public override AArch64Instruction Decode(AArch64Disassembler dasm, uint instr)
        {
            return ((instr & (1 << 23)) != 0 ? mov : logical).Decode(dasm, instr);
        }
    }
}
