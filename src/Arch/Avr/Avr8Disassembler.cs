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

using System;
using System.Collections.Generic;
using Reko.Core;
using Reko.Core.Machine;

namespace Reko.Arch.Avr
{
    // Opcode map: http://lyons42.com/AVR/Opcodes/AVRAllOpcodes.html

    public class Avr8Disassembler : DisassemblerBase<AvrInstruction>
    {
        private static OpRec[] oprecs;

        private Avr8Architecture arch;
        private Address addr;
        private ImageReader rdr;

        public Avr8Disassembler(Avr8Architecture arch, ImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
        }

        public override AvrInstruction DisassembleInstruction()
        {
            ushort wInstr;
            this.addr = rdr.Address;
            if (!rdr.TryReadUInt16(out wInstr))
                return null;
            var instr = oprecs[wInstr >> 12].Decode(this, wInstr);
            instr.Address = addr;
            var length = rdr.Address - addr;
            instr.Length = (int)length;
            return instr;
        }

        public AvrInstruction Decode(ushort wInstr, Opcode opcode, string fmt)
        {
            var ops = new List<MachineOperand>();
            for (int i = 0; i < fmt.Length; ++i)
            {
                MachineOperand op;
                switch (fmt[i++])
                {
                case ',':
                    continue;
                case 'A': // I/O location
                    op = ImmediateOperand.Byte((byte)(((wInstr >> 5) & 0x30) | (wInstr & 0xF)));
                    break;
                case 'i': // 16-bit immediate 
                    op = ImmediateOperand.Byte((byte)((wInstr >> 4) & 0x0F));
                    break;
                case 'J': // Relative jump
                    int offset = ((wInstr & 0xFFF) << 4) >> 3;
                    op = AddressOperand.Create(this.addr + 2 + offset);
                    break;
                case 'D': // Destination register
                    op = Register((wInstr >> 4) & 0x1F);
                    break;
                case 'd': // Destination register (r16-r31)
                    op = Register(0x10 | (wInstr >> 4) & 0x1F);
                    break;
                case 'R': // source register (from 32)
                    op = Register((wInstr >> 5) & 0x10 | (wInstr >> 4) & 0x0F);
                    break;
                case 'r': // source register (from 16)
                    op = Register((wInstr >> 4) & 0x10 | (wInstr >> 4) & 0x0F);
                    break;
                case 'K':
                    op = ImmediateOperand.Byte((byte)(((wInstr >> 4) & 0xF0) | (wInstr & 0xF)));
                    break;
                case 'Q':   // absolute address used by jump and call.
                    ushort w2;
                    if (!rdr.TryReadLeUInt16(out w2))
                        return null;
                    op = AddressOperand.Ptr32(
                        (uint)(((wInstr >> 4) & 0x1F) << 17) |
                        (uint)((wInstr & 1) << 16) |
                        w2);
                    break;
                default:
                    throw new NotImplementedException();
                }
                ops.Add(op);
            }
            return new AvrInstruction
            {
                opcode = opcode,
                operands = ops.ToArray(),
            };
        }

        private MachineOperand Register(int v)
        {
            return new RegisterOperand(arch.GetRegister(v & 0x1F));
        }

        static Avr8Disassembler()
        {
            var oprecs2 = new OpRec[]
            {
                new BOpRec(Opcode.invalid, ""),
                new BOpRec(Opcode.eor, "D,R"),
                new BOpRec(Opcode.or, "D,R"),
                new BOpRec(Opcode.mov, "R,r"),
            };

            var oprecs94_8 = new OpRec[]
            {
                new BOpRec(Opcode.sec, ""),
                new BOpRec(Opcode.sez, ""),
                new BOpRec(Opcode.sen, ""),
                new BOpRec(Opcode.sev, ""),

                new BOpRec(Opcode.ses, ""),
                new BOpRec(Opcode.seh, ""),
                new BOpRec(Opcode.set, ""),
                new BOpRec(Opcode.sei, ""),

                new BOpRec(Opcode.clc, ""),
                new BOpRec(Opcode.clz, ""),
                new BOpRec(Opcode.cln, ""),
                new BOpRec(Opcode.clv, ""),

                new BOpRec(Opcode.cls, ""),
                new BOpRec(Opcode.clh, ""),
                new BOpRec(Opcode.clt, ""),
                new BOpRec(Opcode.cli, ""),

                new BOpRec(Opcode.ret, ""),
                new BOpRec(Opcode.reti, ""),
                new BOpRec(Opcode.invalid, ""),
                new BOpRec(Opcode.invalid, ""),

                new BOpRec(Opcode.invalid, ""),
                new BOpRec(Opcode.invalid, ""),
                new BOpRec(Opcode.invalid, ""),
                new BOpRec(Opcode.invalid, ""),

                new BOpRec(Opcode.sleep, ""),
                new BOpRec(Opcode.@break, ""),
                new BOpRec(Opcode.wdr, ""),
                new BOpRec(Opcode.invalid, ""),

                new BOpRec(Opcode.lpm, ""),
                new BOpRec(Opcode.elpm, ""),
                new BOpRec(Opcode.spm, ""),
                new BOpRec(Opcode.spm, ""),
            };

            var oprecs94_9 = new Dictionary<int, OpRec>
            {
                { 0, new BOpRec(Opcode.ijmp, "") },
                { 1, new BOpRec(Opcode.eijmp, "") },
                { 16, new BOpRec(Opcode.icall, "") },
                { 17, new BOpRec(Opcode.eicall, "") },
            };

            var oprecs94 = new OpRec[]
            {
                new BOpRec(Opcode.com, "R"),
                new BOpRec(Opcode.neg, "R"),
                new BOpRec(Opcode.swap, "R"),
                new BOpRec(Opcode.inc, "R"),

                new BOpRec(Opcode.invalid, ""),
                new BOpRec(Opcode.asr, "R"),
                new BOpRec(Opcode.lsr, "R"),
                new BOpRec(Opcode.ror, "R"),

                new GrpOpRec(4, 5, oprecs94_8),
                new SparseOpRec(4, 5, oprecs94_9),
                new BOpRec(Opcode.dec, "R"),
                new BOpRec(Opcode.des, "i"),

                new BOpRec(Opcode.jmp, "Q"),
                new BOpRec(Opcode.jmp, "Q"),
                new BOpRec(Opcode.call, "Q"),
                new BOpRec(Opcode.call, "Q"),
            };

            var oprecs9 = new OpRec[]
            {
                new BOpRec(Opcode.pop, "D"),
                new BOpRec(Opcode.push, "D"),
                new GrpOpRec(0, 4, oprecs94),
                new BOpRec(Opcode.invalid, ""),

                new BOpRec(Opcode.invalid, ""),
                new BOpRec(Opcode.invalid, ""),
                new BOpRec(Opcode.invalid, ""),
                new BOpRec(Opcode.invalid, ""),
            };

            var oprecsB = new OpRec[]
            {
                new BOpRec(Opcode.@in, "D,A"),
                new BOpRec(Opcode.@out, "A,D"),
            };

            oprecs = new OpRec[]
            {
                new BOpRec(Opcode.invalid, ""),
                new BOpRec(Opcode.invalid, ""),
                new GrpOpRec(10, 2, oprecs2),
                new BOpRec(Opcode.invalid, ""),

                new BOpRec(Opcode.invalid, ""),
                new BOpRec(Opcode.invalid, ""),
                new BOpRec(Opcode.invalid, ""),
                new BOpRec(Opcode.invalid, ""),

                new BOpRec(Opcode.invalid, ""),
                new GrpOpRec(9, 3, oprecs9),
                new BOpRec(Opcode.invalid, ""),
                new GrpOpRec(0xB, 1, oprecsB),

                new BOpRec(Opcode.rjmp, "J"),
                new BOpRec(Opcode.rcall, "J"),
                new BOpRec(Opcode.ldi, "d,K"),
                new BOpRec(Opcode.invalid, ""),
            };
        }

        public abstract class OpRec
        {
            public abstract AvrInstruction Decode(Avr8Disassembler dasm, ushort wInstr);
        }

        public class BOpRec : OpRec
        {
            private Opcode opcode;
            private string fmt;

            public BOpRec(Opcode opcode, string fmt)
            {
                this.opcode = opcode;
                this.fmt = fmt;
            }

            public override AvrInstruction Decode(Avr8Disassembler dasm, ushort wInstr)
            {
                return dasm.Decode(wInstr, opcode, fmt);
            }
        }

        public class GrpOpRec : OpRec
        {
            private int shift;
            private int mask;
            private OpRec[] oprecs;

            public GrpOpRec(int shift, int length, OpRec[] oprecs)
            {
                this.shift = shift;
                this.mask = (1 << length) - 1;
                this.oprecs = oprecs;
            }

            public override AvrInstruction Decode(Avr8Disassembler dasm, ushort wInstr)
            {
                int slot = (wInstr >> shift) & mask;
                return oprecs[slot].Decode(dasm, wInstr);
            }
        }

        public class SparseOpRec : OpRec
        {
            private int shift;
            private int mask;
            private Dictionary<int, OpRec> oprecs;

            public SparseOpRec(int shift, int length, Dictionary<int, OpRec> oprecs)
            {
                this.shift = shift;
                this.mask = (1 << length) - 1;
                this.oprecs = oprecs;
            }

            public override AvrInstruction Decode(Avr8Disassembler dasm, ushort wInstr)
            {
                int slot = (wInstr >> shift) & mask;
                OpRec oprec;
                if (!oprecs.TryGetValue(slot, out oprec))
                {
                    return dasm.Decode(wInstr, Opcode.invalid, "");
                }
                return oprec.Decode(dasm, wInstr);
            }
        }
    }
}