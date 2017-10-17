#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
using System.Threading.Tasks;

namespace Reko.Arch.Msp430
{
    public class Msp430Disassembler : DisassemblerBase<Msp430Instruction>
    {
        private EndianImageReader rdr;
        private Msp430Architecture arch;
        private ushort uExtension;

        public Msp430Disassembler(Msp430Architecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
        }

        public override Msp430Instruction DisassembleInstruction()
        {
            ushort uInstr;
            var addr = rdr.Address;
            if (!rdr.TryReadLeUInt16(out uInstr))
                return null;
            uExtension = 0;
            var instr = s_decoders[uInstr >> 12].Decode(this, uInstr);
            if (instr != null)
            {
                instr.Address = addr;
                instr.Length = (int)(rdr.Address - addr);
            }
            return instr;
        }

        private Msp430Instruction Decode(ushort uInstr, Opcode opcode, string fmt)
        {
            PrimitiveType dataWidth = null;
            int i = 0;
            if (i < fmt.Length)
            {
                switch (fmt[i++])
                {
                case 'w': // use width bit
                    dataWidth = (uInstr & 0x40) != 0 ? PrimitiveType.Byte : PrimitiveType.Word16;
                    break;
                case 'a':   // a/w bit 4.
                    dataWidth = (uInstr & 0x04) != 0 ? PrimitiveType.Word16 : Msp430Architecture.Word20;
                    break;
                case 'x':
                    dataWidth = (uExtension != 0) && (uExtension & 0x40) == 0 ? Msp430Architecture.Word20 : PrimitiveType.Word16;
                    break;
                case 'W': // b/w/a combined from the op and the extension
                    var w = ((this.uExtension & 0x40) >> 5) | (uInstr & 0x040) >> 6;
                    switch (w)
                    {
                    case 0: return Invalid();
                    case 1: dataWidth = Msp430Architecture.Word20; break;
                    case 2: dataWidth = PrimitiveType.Word16;      break;
                    case 3: dataWidth = PrimitiveType.Byte;        break;
                    }
                    break;
                default:
                    --i;
                    break;
                }
            }
            MachineOperand op1 = null;
            MachineOperand op2 = null;
            int aS;
            int iReg;
            if (i < fmt.Length)
            {
                switch (fmt[i++])
                {
                case 'J':
                    int offset = (short)(uInstr << 6) >> 5;
                    op1 = AddressOperand.Create(rdr.Address + offset);
                    break;
                case 'r':
                    op1 = SourceOperand(0, uInstr & 0x0F, dataWidth);
                    break;
                case 'S':
                    aS = (uInstr >> 4) & 0x03;
                    iReg = (uInstr >> 8) & 0x0F;
                    op1 = SourceOperand(aS, iReg, dataWidth);
                    if (op1 == null)
                        return null;
                    break;
                case 's':
                    aS = (uInstr >> 4) & 0x03;
                    iReg = uInstr & 0x0F;
                    op1 = SourceOperand(aS, iReg, dataWidth);
                    if (op1 == null)
                        return null;
                    break;
                case 'n':
                    int n = 1 + ((uInstr >> 4) & 0x0F);
                    op1 = ImmediateOperand.Byte((byte)n);
                    break;
                default:
                    --i;
                    break;
                }
            }
            if (i < fmt.Length)
            {
                switch (fmt[i])
                {
                case 'D':
                    var aD = (uInstr >> 7) & 0x01;
                    iReg = uInstr & 0x0F;
                    if (iReg == 3)
                    {
                        return Invalid();
                    }
                    var reg = Registers.GpRegisters[iReg];
                    if (aD == 0)
                    {
                        if (iReg == 2 || iReg == 3)
                            return Invalid();
                        op2 = new RegisterOperand(reg);
                    }
                    else
                    {
                        op2 = Indexed(reg, dataWidth);
                        if (op2 == null)
                            return null;
                    }
                    break;
                case 'r':
                    op2 = SourceOperand(0, uInstr & 0x0F, dataWidth);
                    break;
                }
            }
                int rep = (uExtension & 0x0F);
            return new Msp430Instruction
            {
                opcode = opcode,
                dataWidth = dataWidth,
                op1 = op1,
                op2 = op2,
                repeatImm = (uExtension & 0x80) != 0 ? 0 : rep + 1,
                repeatReg = (uExtension & 0x80) != 0 ? Registers.GpRegisters[rep] : null,
            };
        }

        private MachineOperand SourceOperand(int aS, int iReg, PrimitiveType dataWidth)
        {
            var reg = Registers.GpRegisters[iReg];
            switch (aS)
            {
            case 0:
                if (iReg == 3)
                    return new ImmediateOperand(Constant.Create(dataWidth, 0));
                else
                    return new RegisterOperand(reg);
            case 1:
                if (iReg == 3)
                    return new ImmediateOperand(Constant.Create(dataWidth, 1));
                return Indexed(reg, dataWidth);
            case 2:
                if (iReg == 2)
                    return new ImmediateOperand(Constant.Create(dataWidth, 4));
                else if (iReg == 3)
                    return new ImmediateOperand(Constant.Create(dataWidth, 2));
                else
                    return new MemoryOperand(dataWidth) { Base = reg };
            case 3:
                if (iReg == 2)
                    return new ImmediateOperand(Constant.Create(dataWidth, 8));
                else if (iReg == 3)
                    return new ImmediateOperand(Constant.Create(dataWidth, -1));
                else
                    return PostInc(reg, dataWidth);
            default:
                throw new NotImplementedException();
            }
        }

        private MachineOperand PostInc(RegisterStorage reg, PrimitiveType dataWidth)
        {
            if (reg == Registers.pc)
            {
                short offset;
                if (!rdr.TryReadLeInt16(out offset))
                    return null;
                return ImmediateOperand.Word16((ushort)offset);
            }
            else
            { 
                return new MemoryOperand(dataWidth ?? PrimitiveType.Word16)
                {
                    Base = reg,
                    PostIncrement = true,
                };
            }

        }

        private MachineOperand Indexed(RegisterStorage reg, PrimitiveType dataWidth)
        {
            short offset;
            if (!rdr.TryReadLeInt16(out offset))
                return null;
            if (reg.Number == 2)
            {
                return AddressOperand.Ptr16((ushort)offset);
            }
            else
            {
                return new MemoryOperand(dataWidth ?? PrimitiveType.Word16)
                {
                    Base = reg,
                    Offset = offset
                };
            }
        }

        private Msp430Instruction Invalid()
        {
            return new Msp430Instruction { opcode = Opcode.invalid };
        }

        private abstract class OpRecBase
        {
            public abstract Msp430Instruction Decode(Msp430Disassembler dasm, ushort uInstr);
        }

        private class OpRec : OpRecBase
        {
            private string fmt;
            private Opcode opcode;

            public OpRec(Opcode opcode, string fmt)
            {
                this.opcode = opcode;
                this.fmt = fmt;
            }

            public override Msp430Instruction Decode(Msp430Disassembler dasm, ushort uInstr)
            {
                return dasm.Decode(uInstr, opcode, fmt);
            }
        }

        private class JmpOpRec : OpRecBase
        {
            public override Msp430Instruction Decode(Msp430Disassembler dasm, ushort uInstr)
            {
                return dasm.Decode(uInstr, jmps[(uInstr >> 10) & 7], "J");
            }
        }

        private class SubOpRec : OpRecBase
        {
            private Dictionary<int, OpRecBase> decoders;
            private ushort mask;
            private int sh;

            public SubOpRec(int sh, ushort mask, Dictionary<int, OpRecBase> decoders)
            {
                this.sh = sh;
                this.mask = mask;
                this.decoders = decoders;
            }

            public override Msp430Instruction Decode(Msp430Disassembler dasm, ushort uInstr)
            {
                OpRecBase oprec;
                var key = (uInstr >> sh) & mask;
                if (!decoders.TryGetValue(key, out oprec))
                    return dasm.Invalid();
                return oprec.Decode(dasm, uInstr);
            }
        }

        private class ExtOpRec : OpRecBase
        {
            public override Msp430Instruction Decode(Msp430Disassembler dasm, ushort uInstr)
            {
                ushort u;
                if (!dasm.rdr.TryReadLeUInt16(out u))
                    return dasm.Invalid();
                dasm.uExtension = uInstr;
                uInstr = u;
                return extDecoders[uInstr >> 12].Decode(dasm, uInstr);
            }
        }

        private static ExtOpRec extOpRec = new ExtOpRec();

        private static SubOpRec rotations = new SubOpRec(8, 0x03, new Dictionary<int, OpRecBase>
        {
            { 0x03, new OpRec(Opcode.rrum, "ar") },
        });

        private static OpRecBase[] s_decoders = new OpRecBase[16]
        {
            new SubOpRec(0x4, 0x0F, new Dictionary<int, OpRecBase>
            {
                { 0x04, rotations },
                { 0x05, rotations },
            }),
            new SubOpRec(6, 0x3F, new Dictionary<int, OpRecBase> {
                { 0x00, new OpRec(Opcode.rrc, "ws") },
                { 0x01, new OpRec(Opcode.rrc, "ws") },
                { 0x02, new OpRec(Opcode.swpb, "s") },
                { 0x04, new OpRec(Opcode.rra, "ws") },
                { 0x05, new OpRec(Opcode.rra, "ws") },
                { 0x06, new OpRec(Opcode.sxt, "ws") },
                { 0x08, new OpRec(Opcode.push, "ws") },
                { 0x09, new OpRec(Opcode.push, "ws") },
                { 0x0A, new OpRec(Opcode.call, "s") },
                { 0x0C, new SubOpRec(0, 0x3F, new Dictionary<int, OpRecBase> {
                    { 0x00, new OpRec(Opcode.reti, "") }
                } ) },

                { 0x10, new OpRec(Opcode.pushm, "xnr") },
                { 0x11, new OpRec(Opcode.pushm, "xnr") },
                { 0x12, new OpRec(Opcode.pushm, "xnr") },
                { 0x13, new OpRec(Opcode.pushm, "xnr") },

                { 0x14, new OpRec(Opcode.pushm, "xnr") },
                { 0x15, new OpRec(Opcode.pushm, "xnr") },
                { 0x16, new OpRec(Opcode.pushm, "xnr") },
                { 0x17, new OpRec(Opcode.pushm, "xnr") },

                { 0x18, new OpRec(Opcode.popm, "xnr") },
                { 0x19, new OpRec(Opcode.popm, "xnr") },
                { 0x1A, new OpRec(Opcode.popm, "xnr") },
                { 0x1B, new OpRec(Opcode.popm, "xnr") },

                { 0x1C, new OpRec(Opcode.popm, "xnr") },
                { 0x1D, new OpRec(Opcode.popm, "xnr") },
                { 0x1E, new OpRec(Opcode.popm, "xnr") },
                { 0x1F, new OpRec(Opcode.popm, "xnr") },

                { 0x20, extOpRec },
                { 0x21, extOpRec },
                { 0x22, extOpRec },
                { 0x23, extOpRec },

                { 0x24, extOpRec },
                { 0x25, extOpRec },
                { 0x26, extOpRec },
                { 0x27, extOpRec },

                { 0x28, extOpRec },
                { 0x29, extOpRec },
                { 0x2A, extOpRec },
                { 0x2B, extOpRec },

                { 0x2C, extOpRec },
                { 0x2D, extOpRec },
                { 0x2E, extOpRec },
                { 0x2F, extOpRec },

                { 0x30, extOpRec },
                { 0x31, extOpRec },
                { 0x32, extOpRec },
                { 0x33, extOpRec },

                { 0x34, extOpRec },
                { 0x35, extOpRec },
                { 0x36, extOpRec },
                { 0x37, extOpRec },

                { 0x38, extOpRec },
                { 0x39, extOpRec },
                { 0x3A, extOpRec },
                { 0x3B, extOpRec },

                { 0x3C, extOpRec },
                { 0x3D, extOpRec },
                { 0x3E, extOpRec },
                { 0x3F, extOpRec },
            }),
            new JmpOpRec(),
            new JmpOpRec(),

            new OpRec(Opcode.mov, "wSD"),
            new OpRec(Opcode.add, "wSD"),
            new OpRec(Opcode.addc, "wSD"),
            new OpRec(Opcode.subc, "wSD"),

            new OpRec(Opcode.sub, "wSD"),
            new OpRec(Opcode.cmp, "wSD"),
            new OpRec(Opcode.dadd, "wSD"),
            new OpRec(Opcode.bit, "wSD"),

            new OpRec(Opcode.bic, "wSD"),
            new OpRec(Opcode.bis, "wSD"),
            new OpRec(Opcode.xor, "wSD"),
            new OpRec(Opcode.and, "wSD"),
        };

        private static OpRecBase[] extDecoders = new OpRecBase[16]
        {
            new OpRec(Opcode.invalid, ""),
            new SubOpRec(6, 0x3F, new Dictionary<int, OpRecBase> {
                { 0x00, new OpRec(Opcode.invalid, "") },
                { 0x01, new OpRec(Opcode.invalid, "") },
                { 0x02, new OpRec(Opcode.invalid, "") },
                { 0x04, new OpRec(Opcode.rrax, "Ws") },
                { 0x05, new OpRec(Opcode.rrax, "Ws") },
                { 0x06, new OpRec(Opcode.invalid, "") },
                { 0x08, new OpRec(Opcode.invalid, "") },
                { 0x09, new OpRec(Opcode.invalid, "") },
                { 0x0A, new OpRec(Opcode.invalid, "") },
                { 0x0C, new SubOpRec(0, 0x3F, new Dictionary<int, OpRecBase> {
                    { 0x00, new OpRec(Opcode.reti, "") }
                } ) }
            }),
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

        private static Opcode[] jmps = new Opcode[8]
        {
            Opcode.jnz,
            Opcode.jz,
            Opcode.jnc,
            Opcode.jc,
            Opcode.jn,
            Opcode.jge,
            Opcode.jl,
            Opcode.jmp,
        };
    }
}
