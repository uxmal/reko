#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using System.Linq;
using System.Text;
using Opcode = Reko.Arch.Tlcs.Tlcs900Opcode;

namespace Reko.Arch.Tlcs
{
    public partial class Tlcs900Disassembler : DisassemblerBase<Tlcs900Instruction>
    {
        private Tlcs900Architecture arch;
        private ImageReader rdr;
        private Address addr;

        private MachineOperand opSrc;
        private MachineOperand opDst;
        private char opSize;

        public Tlcs900Disassembler(Tlcs900Architecture arch, ImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
        }

        public override Tlcs900Instruction DisassembleInstruction()
        {
            this.addr = rdr.Address;
            byte b;
            if (!rdr.TryReadByte(out b))
                return null;
            this.opSrc = null;
            this.opDst = null;
            this.opSize = '\0';
            var instr = opRecs[b].Decode(b, this);
            instr.Length = (int)(rdr.Address - instr.Address);
            return instr;
        }

        private Tlcs900Instruction Decode(byte b, Opcode opcode, string fmt)
        {
            var instr = new Tlcs900Instruction
            {
                Opcode = opcode,
                Address = this.addr,

            };
            var ops = new List<MachineOperand>();
            MachineOperand op = null;
            for (int i = 0; i< fmt.Length; ++i)
            {
                switch (fmt[i++])
                {
                case ',': continue;
                case 'R':   // 16 bit register encoded in lsb
                    op = new RegisterOperand( Reg(fmt[i++], b & 0x7));
                    break;
                }
                ops.Add(op);
            }
            if (ops.Count > 0)
            {
                instr.op1 = ops[0];
                if (ops.Count > 1)
                {
                    instr.op2 = ops[1];
                    if (ops.Count > 2)
                    {
                        instr.op3 = ops[2];
                    }
                }
            }
            return instr;
        }

        private MachineOperand DecodeOperand(byte b, string fmt)
        {
            MachineOperand op;
            byte r;
            switch (fmt[0])
            {
            case '+': // Predecrement
                if (!rdr.TryReadByte(out r))
                    return null;
                op = MemoryOperand.PostIncrement(Size(fmt[1]), incDecSize[r & 3], Reg('x', (r >> 2) & 0x3F));
                SetSize(fmt[1]);
                return op;
            case '-':
                if (!rdr.TryReadByte(out r))
                    return null;
                op = MemoryOperand.PreDecrement(Size(fmt[1]), incDecSize[r & 3], Reg('x', (r >> 2) & 0x3F));
                SetSize(fmt[1]);
                return op;
            case '3': // Immediate encoded in low 3 bits
                var c = Constant.Create(Size(fmt[1]), imm3Const[b & 7]);
                SetSize(fmt[1]);
                return new ImmediateOperand(c);
            case 'r': // Register
            case 'R':
                //$TODO: 'r' may encode other registers. manual is dense
                op = new RegisterOperand(Reg(fmt[1], b & 0x7));
                SetSize(fmt[1]);
                return op;
            case 'M': // Register indirect
                op = MemoryOperand.Indirect(Size(fmt[1]), Reg('x', b & 7));
                SetSize(fmt[1]);
                return op;
            case 'N': // indexed (8-bit offset)
                byte o8;
                if (!rdr.TryReadByte(out o8))
                    return null;
                op = MemoryOperand.Indexed8(Size(fmt[1]), Reg('x', b & 7), (sbyte)o8);
                SetSize(fmt[1]);
                return op;
            case 'm': // various mem formats
                byte m;
                if (!rdr.TryReadByte(out m))
                    return null;
                switch (m & 3)
                {
                case 0: // Register indirect
                    op = MemoryOperand.Indirect(Size(fmt[1]), Reg('x', (m >> 2) & 0x3F));
                    break;
                case 1: // indexed (16-bit offset)
                    short o16;
                    if (!rdr.TryReadLeInt16(out o16))
                        return null;
                    op = MemoryOperand.Indexed16(Size(fmt[1]), Reg('x', (m >> 2) & 0x3F), o16);
                    SetSize(fmt[1]);
                    return op;
                case 3:
                    if (m != 3 && m != 7)
                        return null;
                    byte rBase;
                    if (!rdr.TryReadByte(out rBase))
                        return null;
                    byte rIdx;
                    if (!rdr.TryReadByte(out rIdx))
                        return null;
                    var regBase = Reg('x', rBase);
                    var regIdx = Reg(m == 3 ? 'b' : 'w', rIdx);
                    op = MemoryOperand.RegisterIndexed(Size(fmt[1]), regBase, regIdx);
                    SetSize(fmt[1]);
                    return op;
                default:
                    throw new FormatException(string.Format(
                        "Unknown format {0] decoding bytes {1:X2}{2:X2}.",
                            fmt[0], (int)b, (int)m));
                }
                SetSize(fmt[1]);
                return op;
            case 'O': return Absolute(1, fmt[1]);
            case 'P': return Absolute(2, fmt[1]);
            case 'Q': return Absolute(3, fmt[1]);
            default: throw new FormatException(
                string.Format(
                    "Unknown format {0} decoding byte {1:X2}.", fmt[0], (int)b));
            }
        }

        private RegisterStorage Reg(char size, int regNum)
        {
            int r = regNum & 7;
            if (size == 'z')
            {
                size = this.opSize;
            }
            switch (size)
            {
            case 'x': return Tlcs900Registers.regs[r];
            case 'w': return Tlcs900Registers.regs[8 + r];
            case 'b': return Tlcs900Registers.regs[16 + r];
            default: throw new FormatException();
            }
        }

        private PrimitiveType Size(char size)
        {
            if (size == 'z')
            {
                size = this.opSize;
            }
            switch (size)
            {
            
            case 'x': this.opSize = size; return PrimitiveType.Word32;
            case 'w': this.opSize = size; return PrimitiveType.Word16;
            case 'b': this.opSize = size; return PrimitiveType.Byte;
            case '?':
                // Don't know the size yet, second operand will 
                // provide size.
                this.opSize = size; return null;
            default: throw new FormatException();
            }
        }

        private void SetSize(char size)
        {
            if (size != 'z')
                this.opSize = size;
        }

        private MachineOperand Absolute(int addrBytes, char size)
        {
            uint uAddr = 0;
            int sh = 0;
            while (--addrBytes >= 0)
            {
                byte b;
                if (!rdr.TryReadByte(out b))
                    return null;
                uAddr |= (uint)b << sh;
                sh += 8;
            }
            SetSize(size);
            return MemoryOperand.Absolute(Size(size), uAddr);
        }


        public abstract class OpRecBase
        {
            public abstract Tlcs900Instruction Decode(byte b, Tlcs900Disassembler dasm);
        }

        public class OpRec : OpRecBase
        {
            private Opcode opcode;
            private string fmt;

            public OpRec(Opcode opcode, string fmt)
            {
                this.opcode = opcode;
                this.fmt = fmt;
            }

            public override Tlcs900Instruction Decode(byte b, Tlcs900Disassembler dasm)
            {
                return dasm.Decode(b, opcode, fmt);
            }
        }

        private class RegOpRec : OpRecBase
        {
            private string fmt;

            public RegOpRec(string fmt)
            {
                this.fmt = fmt;
            }

            public override Tlcs900Instruction Decode(byte b, Tlcs900Disassembler dasm)
            {
                dasm.opSrc = dasm.DecodeOperand(b, this.fmt);
                if (dasm.opSrc == null || !dasm.rdr.TryReadByte(out b))
                    return dasm.Decode(b, Opcode.invalid, "");
                return regOpRecs[b].Decode(b, dasm);
            }
        }

        private class MemOpRec : OpRecBase
        {
            private string fmt;

            public MemOpRec(string fmt)
            {
                this.fmt = fmt;
            }

            public override Tlcs900Instruction Decode(byte b, Tlcs900Disassembler dasm)
            {
                dasm.opSrc = dasm.DecodeOperand(b, this.fmt);
                if (dasm.opSrc == null || !dasm.rdr.TryReadByte(out b))
                    return dasm.Decode(b, Opcode.invalid, "");
                return memOpRecs[b].Decode(b, dasm);
            }
        }

        private class DstOpRec : OpRecBase
        {
            private string fmt;

            public DstOpRec(string fmt)
            {
                this.fmt = fmt;
            }

            public override Tlcs900Instruction Decode(byte b, Tlcs900Disassembler dasm)
            {
                dasm.opSrc = dasm.DecodeOperand(b, this.fmt);
                if (dasm.opSrc == null || !dasm.rdr.TryReadByte(out b))
                    return dasm.Decode(b, Opcode.invalid, "");
                var instr = dstOpRecs[b].Decode(b, dasm);
                instr.op1.Width = instr.op2.Width;
                return instr;
            }
        }

        private class SecondOpRec : OpRecBase
        {
            private Opcode opcode;
            private string fmt;

            public SecondOpRec(Opcode opcode, string fmt)
            {
                this.opcode = opcode;
                this.fmt = fmt;
            }

            public override Tlcs900Instruction Decode(byte b, Tlcs900Disassembler dasm)
            {
                dasm.opDst = dasm.DecodeOperand(b, this.fmt);
                if (dasm.opDst == null)
                    return dasm.Decode(b, Opcode.invalid, "");
                return new Tlcs900Instruction
                {
                    Opcode = this.opcode,
                    Address = dasm.addr,
                    op1 = dasm.opDst,
                    op2 = dasm.opSrc,
                };
            }
        }

        // Inverts the order of the decoded operands
        private class InvOpRec : OpRecBase
        {
            private Opcode opcode;
            private string fmt;

            public InvOpRec(Opcode opcode, string fmt)
            {
                this.opcode = opcode;
                this.fmt = fmt;
            }

            public override Tlcs900Instruction Decode(byte b, Tlcs900Disassembler dasm)
            {
                dasm.opDst = dasm.DecodeOperand(b, this.fmt);
                if (dasm.opDst == null)
                    return dasm.Decode(b, Opcode.invalid, "");
                return new Tlcs900Instruction
                {
                    Opcode = this.opcode,
                    Address = dasm.addr,
                    op1 = dasm.opSrc,
                    op2 = dasm.opDst,
                };
            }
        }

        private static int[] imm3Const = new int[8]
        {
            8, 1, 2, 3, 4, 5, 6, 7,
        };

        private static int[] incDecSize = new int[3]
        {
            1, 2, 4
        };

        private static OpRecBase[] opRecs = {
            // 00
            new OpRec(Opcode.nop, ""),    
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            // 10
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            // 20
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.push, "Rw"),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            // 30
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            // 40
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            // 50
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            // 60
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            // 70
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            // 80
            new MemOpRec("Mb"),
            new MemOpRec("Mb"),
            new MemOpRec("Mb"),
            new MemOpRec("Mb"),

            new MemOpRec("Mb"),
            new MemOpRec("Mb"),
            new MemOpRec("Mb"),
            new MemOpRec("Mb"),

            new MemOpRec("Nb"),
            new MemOpRec("Nb"),
            new MemOpRec("Nb"),
            new MemOpRec("Nb"),

            new MemOpRec("Nb"),
            new MemOpRec("Nb"),
            new MemOpRec("Nb"),
            new MemOpRec("Nb"),
            // 90
            new MemOpRec("Mw"),
            new MemOpRec("Mw"),
            new MemOpRec("Mw"),
            new MemOpRec("Mw"),

            new MemOpRec("Mw"),
            new MemOpRec("Mw"),
            new MemOpRec("Mw"),
            new MemOpRec("Mw"),

            new MemOpRec("Nw"),
            new MemOpRec("Nw"),
            new MemOpRec("Nw"),
            new MemOpRec("Nw"),

            new MemOpRec("Nw"),
            new MemOpRec("Nw"),
            new MemOpRec("Nw"),
            new MemOpRec("Nw"),
            // A0
            new MemOpRec("Mx"),
            new MemOpRec("Mx"),
            new MemOpRec("Mx"),
            new MemOpRec("Mx"),

            new MemOpRec("Mx"),
            new MemOpRec("Mx"),
            new MemOpRec("Mx"),
            new MemOpRec("Mx"),

            new MemOpRec("Nx"),
            new MemOpRec("Nx"),
            new MemOpRec("Nx"),
            new MemOpRec("Nx"),

            new MemOpRec("Nx"),
            new MemOpRec("Nx"),
            new MemOpRec("Nx"),
            new MemOpRec("Nx"),
            // B0
            new DstOpRec("M?"),
            new DstOpRec("M?"),
            new DstOpRec("M?"),
            new DstOpRec("M?"),

            new DstOpRec("M?"),
            new DstOpRec("M?"),
            new DstOpRec("M?"),
            new DstOpRec("M?"),

            new DstOpRec("N?"),
            new DstOpRec("N?"),
            new DstOpRec("N?"),
            new DstOpRec("N?"),

            new DstOpRec("N?"),
            new DstOpRec("N?"),
            new DstOpRec("N?"),
            new DstOpRec("N?"),
            // C0
            new MemOpRec("Ob"),
            new MemOpRec("Pb"),
            new MemOpRec("Qb"),
            new MemOpRec("mb"),

            new MemOpRec("-b"),
            new MemOpRec("+b"),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new RegOpRec("rb"),
            new RegOpRec("rb"),
            new RegOpRec("rb"),
            new RegOpRec("rb"),

            new RegOpRec("rb"),
            new RegOpRec("rb"),
            new RegOpRec("rb"),
            new RegOpRec("rb"),
            // D0
            new MemOpRec("Ow"),
            new MemOpRec("Pw"),
            new MemOpRec("Qw"),
            new MemOpRec("mw"),

            new MemOpRec("-w"),
            new MemOpRec("+w"),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new RegOpRec("rw"),
            new RegOpRec("rw"),
            new RegOpRec("rw"),
            new RegOpRec("rw"),

            new RegOpRec("rw"),
            new RegOpRec("rw"),
            new RegOpRec("rw"),
            new RegOpRec("rw"),
            // E0
            new MemOpRec("Ox"),
            new MemOpRec("Px"),
            new MemOpRec("Qx"),
            new MemOpRec("mx"),

            new MemOpRec("-x"),
            new MemOpRec("+x"),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new RegOpRec("rx"),
            new RegOpRec("rx"),
            new RegOpRec("rx"),
            new RegOpRec("rx"),

            new RegOpRec("rx"),
            new RegOpRec("rx"),
            new RegOpRec("rx"),
            new RegOpRec("rx"),

            // F0
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
        };
    }
}
