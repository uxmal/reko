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

using System;
using System.Collections.Generic;
using Reko.Core;
using Reko.Core.Machine;
using System.Diagnostics;
using System.Text;
using Reko.Core.Expressions;

namespace Reko.Arch.Xtensa
{
    public class XtensaDisassembler : DisassemblerBase<XtensaInstruction>
    {
        private XtensaArchitecture arch;
        private ImageReader rdr;
        private State state;

        public XtensaDisassembler(XtensaArchitecture arch, ImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
        }

        private struct State
        {
            public Address addr;
            public byte op0;
            public byte t;
            public byte s;
            public byte r;

            public byte op1;
            public byte op2;
            public byte imm8;
            public ushort imm12;
            public ushort imm16;
            public int offset;

            public override string ToString()
            {

                var sb = new StringBuilder();
                sb.AppendFormat("op0: {0:X2}", op0);
                sb.AppendLine();
                sb.AppendFormat("t:   {0:X2}", t);
                sb.AppendLine();
                sb.AppendFormat("s:   {0:X2}", s);
                sb.AppendLine();
                sb.AppendFormat("r:   {0:X2}", r);
                sb.AppendLine();
                sb.AppendFormat("op1: {0:X2}", op1);
                sb.AppendLine();
                sb.AppendFormat("op2: {0:X2}", op2);
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendFormat("imm8:  {0:X2}", imm8);
                sb.AppendFormat("imm12: {0:X2}", imm12);
                sb.AppendFormat("imm16: {0:X2}", imm16);
                return sb.ToString();
            }
        }
        public override XtensaInstruction DisassembleInstruction()
        {
            byte b0;
            byte b1;
            byte b2;
            this.state = new State();
            state.addr = this.rdr.Address;
            if (!this.rdr.TryReadByte(out b0)) return null;
            if (!this.rdr.TryReadByte(out b1)) return null;
            if (!this.rdr.TryReadByte(out b2)) return null;

            // Extract little endian pieces.
            state.op0 = (byte)(b0 & 0x0F);
            state.t = (byte)((b0 & 0xF0) >> 4);
            state.s = (byte)((b1 & 0x0F));
            state.r = (byte)((b1 & 0xF0) >> 4);
            state.op1 = (byte)((b2 & 0x0F));
            state.op2 = (byte)((b2 & 0xF0) >> 4);
            state.imm8 = b2;
            state.imm12 = (ushort)((b2 << 4) | (b1 >> 4));
            state.imm16 = (ushort)((b2 << 8) | b1);
            state.offset = (((b2 << 16) | (b1 << 8) | b0) >> 6);

            XtensaInstruction instr;
            try
            {
                instr = oprecs[state.op0].Decode(this);
            }
            catch
            {
                Debug.Print("state: {0}", state);
                instr = new XtensaInstruction { Opcode = Opcodes.invalid };
            };
            instr.Address = state.addr;
            instr.Length = (int)(this.rdr.Address - state.addr);
            return instr;
        }

        private XtensaInstruction DecodeOperands(Opcodes opcode, string fmt)
        {
            var ops = new List<MachineOperand>();
            for (int i =0; i < fmt.Length; ++i)
            {
                MachineOperand op;
                switch (fmt[i++])
                {
                default: throw new NotImplementedException(
                    string.Format("Operand type {0} not implemented.", fmt[i - 1]));
                case ',':
                    continue;
                case 'c': op = CallOffset(state.offset); break;
                case 'i': op = SplitImmediate(); break;
                case 'p':
                    op = NegativePcRelativeAddress();
                    break;
                case 'r': op = GetAluRegister(state.r); break;
                case 's': op = GetAluRegister(state.s); break;
                case 't': op = GetAluRegister(state.t); break;
                case 'S': op = SpecialRegister((state.r << 4) | state.s); break;
                }
                ops.Add(op);
            }
            return new XtensaInstruction
            {
                Opcode = opcode,
                Operands = ops.ToArray()
            };
        }

        private MachineOperand CallOffset(int offset)
        {
             return AddressOperand.Ptr32((uint)((state.addr.ToUInt32() & ~3) + (offset << 2) + 4));
        }

        private ImmediateOperand SplitImmediate()
        {
            int n = ((state.imm8 | (state.s << 8)) << 20) >> 20;
            return new ImmediateOperand(Constant.Word32(n));
        }

        private MachineOperand GetAluRegister(byte r)
        {
            return new RegisterOperand(arch.GetAluRegister(r));
        }

        private MachineOperand SpecialRegister(int sr)
        {
            return new RegisterOperand(arch.GetSpecialRegister(sr));
        }

        private MachineOperand NegativePcRelativeAddress()
        {
            var nAddr = 
                ((0xFFFF0000 | state.imm16) << 2) +
                 (int)(rdr.Address.ToUInt32() & ~3);
            return AddressOperand.Ptr32((uint)nAddr);
        }

        public abstract class OpRecBase
        {
            public abstract XtensaInstruction Decode(XtensaDisassembler dasm);
        }

        public class Op1Rec : OpRecBase
        {
            private OpRecBase[] aOprecs;

            public Op1Rec(params OpRecBase [] aOprecs)
            {
                this.aOprecs = aOprecs;
            }

            public override XtensaInstruction Decode(XtensaDisassembler dasm)
            {
                return aOprecs[dasm.state.op1].Decode(dasm);
            }
        }

        public class Op2Rec : OpRecBase
        {
            private OpRecBase[] aOprecs;

            public Op2Rec(params OpRecBase[] aOprecs)
            {
                this.aOprecs = aOprecs;
            }

            public override XtensaInstruction Decode(XtensaDisassembler dasm)
            {
                return aOprecs[dasm.state.op2].Decode(dasm);
            }
        }


        public class r_Rec : OpRecBase
        {
            private OpRecBase[] aOprecs;

            public r_Rec(params OpRecBase[] aOprecs)
            {
                this.aOprecs = aOprecs;
            }

            public override XtensaInstruction Decode(XtensaDisassembler dasm)
            {
                return aOprecs[dasm.state.r].Decode(dasm);
            }
        }

        public class m_Rec : OpRecBase
        {
            private OpRecBase[] aOprecs;

            public m_Rec(params OpRecBase[] aOprecs)
            {
                this.aOprecs = aOprecs;
            }

            public override XtensaInstruction Decode(XtensaDisassembler dasm)
            {
                return aOprecs[dasm.state.t >> 2].Decode(dasm);
            }
        }


        public class n_Rec : OpRecBase
        {
            private OpRecBase[] aOprecs;

            public n_Rec(params OpRecBase[] aOprecs)
            {
                this.aOprecs = aOprecs;
            }

            public override XtensaInstruction Decode(XtensaDisassembler dasm)
            {
                return aOprecs[dasm.state.t & 0x03].Decode(dasm);
            }
        }

        public class OpRec : OpRecBase
        {
            private Opcodes opcode;
            private string fmt;

            public OpRec(Opcodes opcode, string fmt)
            {
                this.opcode = opcode;
                this.fmt = fmt;
            }

            public override XtensaInstruction Decode(XtensaDisassembler dasm)
            {
                return dasm.DecodeOperands(opcode, fmt);
            }
        }


        private static OpRecBase[] oprecs;

        static XtensaDisassembler()
        {
            var oprecLSCX = new Op1Rec(
                new OpRec(Opcodes.lsx,"T,s,r"),
                null,
                null,
                null,

                null,
                null,
                null,
                null,

                null,
                null,
                null,
                null,

                null,
                null,
                null,
                null);

            var oprecJR = new n_Rec(
                new OpRec(Opcodes.ret, ""),
                null,
                null,
                null);

            var oprecSNM0 = new m_Rec(
                new OpRec(Opcodes.ill, ""),
                null,
                oprecJR,
                null);

            var oprecST0 = new r_Rec(
                oprecSNM0,
                null,
                null,
                null,

                null,
                null,
                null,
                null,

                null,
                null,
                null,
                null,

                null,
                null,
                null,
                null);

            var oprecRST0 = new Op2Rec(
                oprecST0,
                null,
                new OpRec(Opcodes.or, "r,s,t"),
                null,

                null,
                null,
                null,
                null,

                null,
                null,
                null,
                null,

                null,
                null,
                null,
                null);

            var oprecRST3 = new Op2Rec(
               null,
               new OpRec(Opcodes.wsr, "t,S"),
               null,
               null,

               null,
               null,
               null,
               null,

               null,
               null,
               null,
               null,

               null,
               null,
               null,
               null);

            var oprecQRST = new Op1Rec(
                oprecRST0,
                null,
                null,
                oprecRST3,

                null,
                null,
                null,
                null,

                oprecLSCX,
                null,
                null,
                null,

                new OpRec(Opcodes.reserved, ""),
                new OpRec(Opcodes.reserved, ""),
                new OpRec(Opcodes.reserved, ""),
                new OpRec(Opcodes.reserved, ""));

            var oprecLSAI = new r_Rec(
                null,
                null,
                null,
                null,

                null,
                null,
                null,
                null,

                null,
                null,
                new OpRec(Opcodes.movi, "t,i"),
                null,

                null,
                null,
                null,
                null);

            var oprecCALLN = new n_Rec(
                new OpRec(Opcodes.call0, "c"),
                new OpRec(Opcodes.call4, "c"),
                new OpRec(Opcodes.call8, "c"),
                new OpRec(Opcodes.call12, "c"));

            oprecs = new OpRecBase[]
            {
                oprecQRST,
                new OpRec(Opcodes.l32r, "t,p"),
                oprecLSAI,
                null,

                null,
                oprecCALLN,
                null,
                null,

                null,
                null,
                null,
                null,

                null,
                null,
                null,
                null,
            };
        }
    }
}
