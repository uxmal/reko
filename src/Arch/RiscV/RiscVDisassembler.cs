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

using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Reko.Core;
using Reko.Core.Expressions;

namespace Reko.Arch.RiscV
{
    public class RiscVDisassembler : DisassemblerBase<RiscVInstruction>
    {
        private static OpRec[] opRecs;
        private static OpRec[] wideOpRecs;
        private static OpRec[] compactOpRecs;
    
        private RiscVArchitecture arch;
        private ImageReader rdr;
        private Address addrInstr;

        public RiscVDisassembler(RiscVArchitecture arch, ImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
        }

        public override RiscVInstruction DisassembleInstruction()
        {
            this.addrInstr = rdr.Address;
            ushort hInstr;
            if (!rdr.TryReadLeUInt16(out hInstr))
            {
                var instr = new RiscVInstruction
                {
                    opcode = Opcode.invalid,
                };
                return instr;
            }
            return opRecs[hInstr & 0x3].Decode(this, hInstr);
        }

        private RiscVInstruction DecodeWideOperands(Opcode opcode, string fmt, uint wInstr)
        {
            var ops = new List<MachineOperand>();
            for (int i = 0; i < fmt.Length; ++i)
            {
                MachineOperand op;
                switch (fmt[i++])
                {
                default: throw new InvalidOperationException(string.Format("Unsupported operand code {0}", fmt[i - 1]));
                case ',': continue;
                case 'd': op = GetRegister(wInstr, 7); break;
                case 'I': op = GetImmediate(wInstr, 12, fmt[i++]); break;
                }
                ops.Add(op);
            }
            var instr = new RiscVInstruction
            {
                opcode = opcode,
            };
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

        private RegisterOperand GetRegister(uint wInstr, int bitPos)
        {
            var reg = arch.GetRegister((int)(wInstr >> bitPos) & 0x1F);
            return new RegisterOperand(reg);
        }

        private ImmediateOperand GetImmediate(uint wInstr, int bitPos, char sign)
        {
            if (sign == 's')
            {
                int s = ((int)wInstr) >> bitPos;
                return ImmediateOperand.Int32(s);
            }
            else
            {
                uint u = wInstr >> bitPos;
                return ImmediateOperand.Word32(u);
            }
        }

        public abstract class OpRec
        {
            public abstract RiscVInstruction Decode(RiscVDisassembler dasm, uint hInstr);
        }

        public class WOpRec : OpRec
        {
            private Opcode opcode;
            private string fmt;

            public WOpRec(Opcode opcode, string fmt)
            {
                this.opcode = opcode;
                this.fmt = fmt;
            }

            public override RiscVInstruction Decode(RiscVDisassembler dasm, uint wInstr)
            {
                return dasm.DecodeWideOperands(opcode, fmt, wInstr);
            }
        }

        public class MaskOpRec : OpRec
        {
            private readonly int mask;
            private readonly int shift;
            private readonly OpRec[] subcodes;

            public MaskOpRec(int shift, int mask, OpRec[] subcodes)
            {
                this.mask = mask;
                this.shift = shift;
                this.subcodes = subcodes;
            }

            public override RiscVInstruction Decode(RiscVDisassembler dasm, uint wInstr)
            {
                return subcodes[(wInstr >> shift) & mask].Decode(dasm, wInstr);
            }
        }

        public class WideOpRec : OpRec
        {
            public WideOpRec()
            {
            }

            public override RiscVInstruction Decode(RiscVDisassembler dasm, uint hInstr)
            {
                ushort hiword;
                if (!dasm.rdr.TryReadUInt16(out hiword))
                {
                    return new RiscVInstruction { opcode = Opcode.invalid, Address = dasm.addrInstr };
                }
                uint wInstr = (uint)hiword << 16;
                wInstr |= hInstr;
                return wideOpRecs[(wInstr >> 2) & 0x1F].Decode(dasm, wInstr);
            }
        }

        static RiscVDisassembler()
        {
            wideOpRecs = new OpRec[]
            {
                // 00
                new WOpRec(Opcode.invalid, ""),
                new WOpRec(Opcode.invalid, ""),
                new WOpRec(Opcode.invalid, ""),
                new WOpRec(Opcode.invalid, ""),

                new WOpRec(Opcode.invalid, ""),
                new WOpRec(Opcode.invalid, ""),
                new WOpRec(Opcode.invalid, ""),
                new WOpRec(Opcode.invalid, ""),

                new WOpRec(Opcode.invalid, ""),
                new WOpRec(Opcode.invalid, ""),
                new WOpRec(Opcode.invalid, ""),
                new WOpRec(Opcode.invalid, ""),

                new WOpRec(Opcode.invalid, ""),
                new WOpRec(Opcode.lui, "d,Iu"),
                new WOpRec(Opcode.invalid, ""),
                new WOpRec(Opcode.invalid, ""),

                // 10
                new WOpRec(Opcode.invalid, ""),
                new WOpRec(Opcode.invalid, ""),
                new WOpRec(Opcode.invalid, ""),
                new WOpRec(Opcode.invalid, ""),

                new WOpRec(Opcode.invalid, ""),
                new WOpRec(Opcode.invalid, ""),
                new WOpRec(Opcode.invalid, ""),
                new WOpRec(Opcode.invalid, ""),

                new WOpRec(Opcode.invalid, ""),
                new WOpRec(Opcode.invalid, ""),
                new WOpRec(Opcode.invalid, ""),
                new WOpRec(Opcode.invalid, ""),

                new WOpRec(Opcode.invalid, ""),
                new WOpRec(Opcode.invalid, ""),
                new WOpRec(Opcode.invalid, ""),
                new WOpRec(Opcode.invalid, ""),
            };

            compactOpRecs = new OpRec[]
            {

            };

            var c = new MaskOpRec(2, 0x1F, compactOpRecs);
            var w = new WideOpRec();
            opRecs = new OpRec[] { c, c, c, w };
        }
    }
}
