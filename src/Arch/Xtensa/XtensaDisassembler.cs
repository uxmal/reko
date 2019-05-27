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
        private static readonly Decoder[] deocders;
        private static readonly int[] b4const;
        private static readonly int[] b4constu;

        private readonly XtensaArchitecture arch;
        private readonly EndianImageReader rdr;
        private State state;

        public XtensaDisassembler(XtensaArchitecture arch, EndianImageReader rdr)
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
            state.addr = this.rdr.Address;
            if (!this.rdr.TryReadByte(out byte b0)) return null;
            if (!this.rdr.TryReadByte(out byte b1)) return null;
            if (!this.rdr.TryReadByte(out byte b2)) return null;

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
                instr = deocders[state.op0].Decode(this);
            }
            catch
            {
                Debug.Print("instr: {0:X2} {1:X2} {2:X2}", b0, b1, b2);
                Debug.Print("{0}", state);
                instr = new XtensaInstruction { Opcode = Opcodes.invalid };
            };
            instr.Address = state.addr;
            instr.Length = (int)(this.rdr.Address - state.addr);
            return instr;
        }

        private XtensaInstruction DecodeOperands(Opcodes opcode, string fmt)
        {
            var ops = new List<MachineOperand>();
            for (int i = 0; i < fmt.Length; ++i)
            {
                MachineOperand op;
                switch (fmt[i++])
                {
                default: throw new NotImplementedException(
                    string.Format("Operand type {0} not implemented.", fmt[i - 1]));
                case ',':
                    continue;
                case '4': op = ImmediateOperand.Byte((byte)(state.r << fmt[i++] - '0')); break;
                case '8': op = ImmediateOperand.UInt16((ushort)(state.imm8 << fmt[i++] - '0')); break;
                case 'B': op = GetBoolRegister(fmt[i++]); break;
                case 'F': op = GetFpuRegister(fmt[i++]); break;
                case 'I': op = ImmediateOperand.Byte(FieldValue(fmt[i++])); break;
                case 'J': op = LongJumpOffset(); break;
                case 'R': op = GetAluRegister(fmt[i++]); break;
                case 'a': op = ImmediateOperand.SByte(state.t == 0 ?(sbyte) -1 : (sbyte)state.t); break;
                case 'b': op = ImmediateBranchOperand(fmt, ref i); break;
                case 'c': op = CallOffset(state.offset); break;
                case 'e': op = L32e_Value(state.r); break;
                case 'i': op = SplitImmediate(); break;
                case 'j': op = JumpOffset(state.imm8); break;
                case 'm': op = Scaled256Immediate(fmt, ref i); break;
                case 'p':
                    op = NegativePcRelativeAddress();
                    break;
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

        private MachineOperand L32e_Value(byte r)
        {
            var off = ~0x3F | (r << 2);
            return ImmediateOperand.Int32(off);
        }

        private MachineOperand ImmediateBranchOperand(string fmt, ref int i)
        {
            switch (fmt[i++])
            {
            case 'u': return ImmediateOperand.Word32(b4constu[state.r]);
            case 's': return ImmediateOperand.Int32(b4const[state.r]);
            default: throw new NotImplementedException();
            }
        }

        private byte FieldValue(char f)
        {
            switch (f)
            {
            default:  throw new NotImplementedException();
            case 'r': return state.r;
            case 's': return state.s;
            case 't': return state.t;
            case '1': return state.op1;
            case '2': return state.op2;
            case 'S': return (byte)(state.t | ((state.op2 & 1) << 4)); // used by slli
            case 'R': return (byte)(state.s | ((state.op2 & 1) << 4)); // used by srai
            case 'I': return (byte)(state.s | ((state.r & 1) << 4));   // used by ssai
            }
        }

        private ImmediateOperand Scaled256Immediate(string fmt, ref int i)
        {
            int n = (sbyte)state.imm8;
            n <<= (fmt[i++] - '0');
            return ImmediateOperand.Word32(n);
        }

        private MachineOperand CallOffset(int offset)
        {
            return AddressOperand.Ptr32((uint)((state.addr.ToUInt32() & ~3) + (offset << 2) + 4));
        }

        private AddressOperand LongJumpOffset()
        {
            return AddressOperand.Ptr32((uint)((int)state.addr.ToLinear() + (state.offset + 4)));
        }

        private MachineOperand JumpOffset(byte offset)
        {
            int dst = (int)state.addr.ToUInt32() + (sbyte)offset + 4;
            return AddressOperand.Ptr32((uint)dst);
        }

        private ImmediateOperand SplitImmediate()
        {
            int n = ((state.imm8 | (state.s << 8)) << 20) >> 20;
            return new ImmediateOperand(Constant.Word32(n));
        }

        private MachineOperand GetAluRegister(char pos)
        {
            RegisterStorage reg = arch.GetAluRegister(this.FieldValue(pos));
            return new RegisterOperand(reg);
        }
    
        private MachineOperand GetFpuRegister(char pos)
        {
            var freg = arch.GetFpuRegister(this.FieldValue(pos));
            return new RegisterOperand(freg);
        }

        private MachineOperand GetAluRegister(byte r)
        {
            return new RegisterOperand(arch.GetAluRegister(r));
        }

        private MachineOperand GetBoolRegister(char pos)
        {
            RegisterStorage reg;
            switch (pos)
            {
            case 'r': reg =  arch.GetBoolRegister(state.r); break;
            case 's': reg =  arch.GetBoolRegister(state.s); break;
            case 't': reg =  arch.GetBoolRegister(state.t); break;
            default: throw new ArgumentOutOfRangeException();
            }
            return new RegisterOperand(reg);
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

        public abstract class Decoder
        {
            public abstract XtensaInstruction Decode(XtensaDisassembler dasm);
        }

        public class Op1Decoder : Decoder
        {
            private Decoder[] aOprecs;

            public Op1Decoder(params Decoder [] aOprecs)
            {
                Debug.Assert(aOprecs.Length == 16);
                this.aOprecs = aOprecs;
            }

            public override XtensaInstruction Decode(XtensaDisassembler dasm)
            {
                return aOprecs[dasm.state.op1].Decode(dasm);
            }
        }

        public class Op2Decoder : Decoder
        {
            private Decoder[] aOprecs;

            public Op2Decoder(params Decoder[] aOprecs)
            {
                Debug.Assert(aOprecs.Length == 16);
                this.aOprecs = aOprecs;
            }

            public override XtensaInstruction Decode(XtensaDisassembler dasm)
            {
                return aOprecs[dasm.state.op2].Decode(dasm);
            }
        }


        public class r_Rec : Decoder
        {
            private Decoder[] aOprecs;

            public r_Rec(params Decoder[] aOprecs)
            {
                Debug.Assert(aOprecs.Length == 16);
                this.aOprecs = aOprecs;
            }

            public override XtensaInstruction Decode(XtensaDisassembler dasm)
            {
                return aOprecs[dasm.state.r].Decode(dasm);
            }
        }

        public class m_Rec : Decoder
        {
            private Decoder[] aOprecs;

            public m_Rec(params Decoder[] aOprecs)
            {
                Debug.Assert(aOprecs.Length == 4);
                this.aOprecs = aOprecs;
            }

            public override XtensaInstruction Decode(XtensaDisassembler dasm)
            {
                return aOprecs[dasm.state.t >> 2].Decode(dasm);
            }
        }

        public class n_Rec : Decoder
        {
            private Decoder[] aOprecs;

            public n_Rec(params Decoder[] aOprecs)
            {
                Debug.Assert(aOprecs.Length == 4);
                this.aOprecs = aOprecs;
            }

            public override XtensaInstruction Decode(XtensaDisassembler dasm)
            {
                return aOprecs[dasm.state.t & 0x03].Decode(dasm);
            }
        }

        public class s_Rec : Decoder
        {
            private Decoder[] aOprecs;

            public s_Rec(params Decoder[] aOprecs)
            {
                Debug.Assert(aOprecs.Length == 16);
                this.aOprecs = aOprecs;
            }

            public override XtensaInstruction Decode(XtensaDisassembler dasm)
            {
                return aOprecs[dasm.state.s].Decode(dasm);
            }
        }

        public class t_Rec : Decoder
        {
            private Decoder[] aOprecs;

            public t_Rec(params Decoder[] aOprecs)
            {
                Debug.Assert(aOprecs.Length == 16);
                this.aOprecs = aOprecs;
            }

            public override XtensaInstruction Decode(XtensaDisassembler dasm)
            {
                return aOprecs[dasm.state.t].Decode(dasm);
            }
        }

        public class OpRec : Decoder
        {
            private Opcodes opcode;
            private InstrClass iclass;
            private string fmt;
            private bool twoByte;

            public OpRec(Opcodes opcode, string fmt)
            {
                this.opcode = opcode;
                this.fmt = fmt;
            }

            public OpRec(Opcodes opcode, InstrClass iclass, string fmt)
            {
                this.opcode = opcode;
                this.iclass = iclass;
                this.fmt = fmt;
            }

            public OpRec(Opcodes opcode, string fmt, bool twoByte)
            {
                this.opcode = opcode;
                this.fmt = fmt;
                this.twoByte = twoByte;
            }

            public override XtensaInstruction Decode(XtensaDisassembler dasm)
            {
                if (this.twoByte)
                    dasm.rdr.Offset -= 1;
                return dasm.DecodeOperands(opcode, fmt);
            }
        }

        public class OpRecMovi_n : Decoder
        {
            public override XtensaInstruction Decode(XtensaDisassembler dasm)
            {
                var n = 
                    dasm.state.r |
                    ((dasm.state.t & 0x7) << 4) |
                    (((dasm.state.t & 0x6) == 0x6)
                        ? ~0x1F
                        : 0);
                var imm = ImmediateOperand.SByte((sbyte)n);

                // this is a 2-byte instruction, so back up one byte.
                dasm.rdr.Offset -= 1;

                return new XtensaInstruction
                {
                    Opcode = Opcodes.movi_n,
                    Operands = new MachineOperand[]
                    {
                        dasm.GetAluRegister(dasm.state.s),
                        imm,
                    }
                };
            }
        }

        public class OpRec_bz : Decoder
        {
            private Opcodes opcode;

            public OpRec_bz(Opcodes opcode)
            {
                this.opcode = opcode;
            }

            public override XtensaInstruction Decode(XtensaDisassembler dasm)
            {
                int shoff = dasm.state.imm12;
                if (shoff > 0x7FF)
                    shoff |= ~0x7FF;
                var dst = AddressOperand.Ptr32((uint)
                    ((int)dasm.state.addr.ToUInt32() + 4 + shoff));

                return new XtensaInstruction
                {
                    Opcode = opcode,
                    Operands = new MachineOperand[]
                    {
                        dasm.GetAluRegister(dasm.state.s),
                        dst,
                    }
                };

            }
        }

        public class OpRecBeqxx_n : Decoder
        {
            private Opcodes opcode;

            public OpRecBeqxx_n(Opcodes opcode)
            {
                this.opcode = opcode;
            }

            public override XtensaInstruction Decode(XtensaDisassembler dasm)
            {
                var n =
                    dasm.state.r |
                    ((dasm.state.t & 0x3) << 4);
                var dst = dasm.JumpOffset((byte)n);

                // this is a 2-byte instruction, so back up one byte.
                dasm.rdr.Offset -= 1;

                return new XtensaInstruction
                {
                    Opcode = opcode,
                    Operands = new MachineOperand[]
                    {
                        dasm.GetAluRegister(dasm.state.s),
                        dst,
                    }
                };
            }
        }

        public class OpRec_bbxi : Decoder
        {
            private Opcodes opcode;

            public OpRec_bbxi(Opcodes opcode)
            {
                this.opcode = opcode;
            }

            public override XtensaInstruction Decode(XtensaDisassembler dasm)
            {
                return new XtensaInstruction
                {
                    Opcode = this.opcode,
                    Operands = new MachineOperand[] {
                        dasm.GetAluRegister(dasm.state.s),
                        ImmediateOperand.Byte((byte)(((dasm.state.r & 1) << 4) | dasm.state.t)),
                        dasm.JumpOffset(dasm.state.imm8)
                    }
                };
            }
        }

        public class ExtuiOpRec : Decoder
        {
            public override XtensaInstruction Decode(XtensaDisassembler dasm)
            {
                return new XtensaInstruction
                {
                    Opcode = Opcodes.extui,
                    Operands = new MachineOperand[]
                    {
                        dasm.GetAluRegister(dasm.state.r),
                        dasm.GetAluRegister(dasm.state.t),
                        ImmediateOperand.Byte((byte)((dasm.state.s | ((dasm.state.op1 & 1) << 4)))),
                        ImmediateOperand.Byte((byte)(dasm.state.op2 + 1))
                    }
                };
            }
        }

        static XtensaDisassembler()
        {
            b4const = new int[16]
            {
                -1, 1, 2, 3, 4, 5, 6, 7, 8, 10, 12, 16, 32, 64, 128, 256
            };

            b4constu = new int[16]
            {
                0x8000,
                0x10000,
                0x2,
                0x3,
                0x4,
                0x5,
                0x6,
                0x7,

                0x8,
                0xA,
                0xC,
                0x10,
                0x20,
                0x40,
                0x80,
                0x100,
            };

            var reserved = new OpRec(Opcodes.reserved, "");

            var oprecLSCX = new Op1Decoder(
                new OpRec(Opcodes.lsx,"T,Rs,Rr"),
                null,
                reserved,
                reserved,

                null,
                null,
                reserved,
                reserved,

                reserved,
                reserved,
                reserved,
                reserved,

                reserved,
                reserved,
                reserved,
                reserved);

            var oprecLSC4 = new Op2Decoder(
                new OpRec(Opcodes.l32e, "Rt,Rs,e"),
                reserved,
                reserved,
                reserved,

                new OpRec(Opcodes.s32e, "Rt,Rs,e"),
                reserved,
                reserved,
                reserved,

                reserved,
                reserved,
                reserved,
                reserved,

                reserved,
                reserved,
                reserved,
                reserved);

            var oprecFP0 = new Op2Decoder(
                new OpRec(Opcodes.add_s, "Fr,Fs,Ft"),
                new OpRec(Opcodes.sub_s, "Fr,Fs,Ft"),
                new OpRec(Opcodes.mul_s, "Fr,Fs,Ft"),
                reserved,

                null,
                null,
                null,
                null,

                null,
                null,
                null,
                new OpRec(Opcodes.floor_s, "Rr,Fs,It"),

                null,
                null,
                null,
                null);

            var oprecFP1 = new Op2Decoder(
                reserved,
                null,
                null,
                new OpRec(Opcodes.ueq_s, "Br,Fs,Ft"),

                null,
                null,
                null,
                null,

                new OpRec(Opcodes.moveqz_s, "Fr,Fs,Rt"),
                null,
                null,
                null,

                null,
                null,
                reserved,
                reserved);

            var oprecJR = new n_Rec(
                new OpRec(Opcodes.ret, ""),
                null,
                new OpRec(Opcodes.jx, "Rs"),
                reserved);

            var oprecCALLX = new n_Rec(
                new OpRec(Opcodes.callx0, "Rs"),
                new OpRec(Opcodes.callx4, "Rs"),
                new OpRec(Opcodes.callx8, "Rs"),
                new OpRec(Opcodes.callx12, "Rs"));

            var oprecSNM0 = new m_Rec(
                new OpRec(Opcodes.ill, InstrClass.Invalid|InstrClass.Zero, ""),
                null,
                oprecJR,
                oprecCALLX);

            var oprecSYNC = new t_Rec(
                new OpRec(Opcodes.isync, ""),
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

                new OpRec(Opcodes.memw, ""),
                null,
                null,
                null);

            var oprecRFET = new s_Rec(
                new OpRec(Opcodes.rfe, ""),
                null,
                null,
                reserved,

                null,
                null,
                reserved,
                reserved,

                reserved,
                reserved,
                reserved,
                reserved,

                reserved,
                reserved,
                reserved,
                reserved);

            var oprecRFEI = new t_Rec(
                oprecRFET,
                new OpRec(Opcodes.rfi, "Is"),
                null,
                reserved,

                reserved,
                reserved,
                reserved,
                reserved,

                reserved,
                reserved,
                reserved,
                reserved,

                reserved,
                reserved,
                reserved,
                reserved);


            var oprecBZ = new m_Rec(
                new OpRec_bz(Opcodes.beqz),
                new OpRec_bz(Opcodes.bnez),
                new OpRec_bz(Opcodes.bltz),
                new OpRec_bz(Opcodes.bgez));

            var oprecBI0 = new m_Rec(
                new OpRec(Opcodes.beqi, "Rs,bs,j"),
                new OpRec(Opcodes.bnei, "Rs,bs,j"),
                new OpRec(Opcodes.blti, "Rs,bs,j"),
                new OpRec(Opcodes.bgei, "Rs,bs,j"));

            var oprecBI1 = new m_Rec(
                null,
                null,
                new OpRec(Opcodes.bltui, "Rs,bu,j"),
                new OpRec(Opcodes.bgeui, "Rs,bu,j"));

            var oprecSI = new n_Rec(
                new OpRec(Opcodes.j, "J"),
                oprecBZ,
                oprecBI0,
                oprecBI1);

            var oprecB = new r_Rec(
               new OpRec(Opcodes.bnone, "Rs,Rt,j"),
               new OpRec(Opcodes.beq, "Rs,Rt,j"),
               new OpRec(Opcodes.blt, "Rs,Rt,j"),
               new OpRec(Opcodes.bltu, "Rs,Rt,j"),

               new OpRec(Opcodes.ball, "Rs,Rt,j"),
               new OpRec(Opcodes.bbc, "Rs,Rt,j"),
               new OpRec_bbxi(Opcodes.bbci),
               new OpRec_bbxi(Opcodes.bbci),

               new OpRec(Opcodes.bany, "Rs,Rt,j"),
               new OpRec(Opcodes.bne, "Rs,Rt,j"),
               new OpRec(Opcodes.bge, "Rs,Rt,j"),
               new OpRec(Opcodes.bgeu, "Rs,Rt,j"),

               new OpRec(Opcodes.bnall, "Rs,Rt,j"),
               new OpRec(Opcodes.bbs, "Rs,Rt,j"),
               new OpRec_bbxi(Opcodes.bbsi),
               new OpRec_bbxi(Opcodes.bbsi));

            var oprecST0 = new r_Rec(
                oprecSNM0,
                null,
                oprecSYNC,
                oprecRFEI,

                new OpRec(Opcodes.@break, "Is,It"),
                null,
                new OpRec(Opcodes.rsil, "Rt,Is"),
                null,

                null,
                null,
                null,
                null,

                null,
                null,
                null,
                null);

            var oprecST1 = new r_Rec(
                new OpRec(Opcodes.ssr, "Rs"),
                new OpRec(Opcodes.ssl, "Rs"),
                new OpRec(Opcodes.ssa8l, "Rs"),
                null,

                new OpRec(Opcodes.ssai, "II"),
                reserved,
                null,
                null,

                null,
                reserved,
                reserved,
                reserved,

                reserved,
                reserved,
                new OpRec(Opcodes.nsa, "Rt,Rs"),
                new OpRec(Opcodes.nsau, "Rt,Rs"));

            var oprecST2 = new t_Rec(
                new OpRecMovi_n(),
                new OpRecMovi_n(),
                new OpRecMovi_n(),
                new OpRecMovi_n(),

                new OpRecMovi_n(),
                new OpRecMovi_n(),
                new OpRecMovi_n(),
                new OpRecMovi_n(),

                new OpRecBeqxx_n(Opcodes.beqz_n),
                new OpRecBeqxx_n(Opcodes.beqz_n),
                new OpRecBeqxx_n(Opcodes.beqz_n),
                new OpRecBeqxx_n(Opcodes.beqz_n),

                new OpRecBeqxx_n(Opcodes.bnez_n),
                new OpRecBeqxx_n(Opcodes.bnez_n),
                new OpRecBeqxx_n(Opcodes.bnez_n),
                new OpRecBeqxx_n(Opcodes.bnez_n));

            var oprecS3 = new t_Rec(
                new OpRec(Opcodes.ret_n, "", true),
                reserved,
                reserved,
                reserved,

                reserved,
                reserved,
                reserved,
                reserved,

                reserved,
                reserved,
                reserved,
                reserved,

                reserved,
                reserved,
                reserved,
                reserved);

            var oprecST3 = new r_Rec(
                new OpRec(Opcodes.mov_n, "Rt,Rs", true),
                reserved,
                reserved,
                reserved,

                reserved,
                reserved,
                reserved,
                reserved,

                reserved,
                reserved,
                reserved,
                reserved,

                reserved,
                reserved,
                reserved,
                oprecS3);

            var oprecRT0 = new s_Rec(
                new OpRec(Opcodes.neg, "Rr,Rt"),
                new OpRec(Opcodes.abs, "Rr,Rt"),
                reserved,
                reserved,
                
                reserved,
                reserved,
                reserved,
                reserved,

                reserved,
                reserved,
                reserved,
                reserved,
                
                reserved,
                reserved,
                reserved,
                reserved);

            var oprecRST0 = new Op2Decoder(
                oprecST0,
                new OpRec(Opcodes.and, "Rr,Rs,Rt"),
                new OpRec(Opcodes.or, "Rr,Rs,Rt"),
                new OpRec(Opcodes.xor, "Rr,Rs,Rt"),

                oprecST1,
                null,
                oprecRT0,
                reserved,

                new OpRec(Opcodes.add, "Rr,Rs,Rt"),
                new OpRec(Opcodes.addx2, "Rr,Rs,Rt"),
                new OpRec(Opcodes.addx4, "Rr,Rs,Rt"),
                new OpRec(Opcodes.addx8, "Rr,Rs,Rt"),

                new OpRec(Opcodes.sub, "Rr,Rs,Rt"),
                new OpRec(Opcodes.subx2, "Rr,Rs,Rt"),
                new OpRec(Opcodes.subx4, "Rr,Rs,Rt"),
                new OpRec(Opcodes.subx8, "Rr,Rs,Rt"));

            var oprecIMP = new r_Rec(
                reserved,
                reserved,
                reserved,
                reserved,

                reserved,
                reserved,
                reserved,
                reserved,

                reserved,
                reserved,
                reserved,
                reserved,

                reserved,
                reserved,
                reserved,
                new OpRec(Opcodes.ldpte, ""));       //$TODO: doesn't appear to be documented

            var oprecRST1 = new Op2Decoder(
                new OpRec(Opcodes.slli, "Rr,Rs,IS"),
                new OpRec(Opcodes.slli, "Rr,Rs,IS"),
                new OpRec(Opcodes.srai, "Rr,Rt,IR"),
                new OpRec(Opcodes.srai, "Rr,Rt,IR"),

                new OpRec(Opcodes.srli, "Rr,Rt,Is"),
                reserved,
                null,
                null,

                new OpRec(Opcodes.src, "Rr,Rs,Rt"),
                new OpRec(Opcodes.srl, "Rr,Rt"),
                new OpRec(Opcodes.sll, "Rr,Rs"),
                new OpRec(Opcodes.sra, "Rr,Rs"),

                new OpRec(Opcodes.mul16u, "Rr,Rs,Rt"),
                new OpRec(Opcodes.mul16s, "Rr,Rs,Rt"),
                reserved,
                oprecIMP);

            var oprecRST2 = new Op2Decoder(
                new OpRec(Opcodes.andb, "Br,Bs,Bt"),
                new OpRec(Opcodes.andbc, "Br,Bs,Bt"),
                new OpRec(Opcodes.orb, "Br,Bs,Bt"),
                new OpRec(Opcodes.orbc, "Br,Bs,Bt"),

                new OpRec(Opcodes.xorb, "Br,Bs,Bt"),
                reserved,
                reserved,
                reserved,

                new OpRec(Opcodes.mull, "Rr,Rs,Rt"),
                reserved,
                null,
                null,

                new OpRec(Opcodes.quou, "Rr,Rs,Rt"),
                new OpRec(Opcodes.quos, "Rr,Rs,Rt"),
                new OpRec(Opcodes.remu, "Rr,Rs,Rt"),
                new OpRec(Opcodes.rems, "Rr,Rs,Rt"));

            var oprecRST3 = new Op2Decoder(
               new OpRec(Opcodes.rsr, "Rt,S"),
               new OpRec(Opcodes.wsr, "Rt,S"),
               null,
               null,

               new OpRec(Opcodes.min, "Rr,Rs,Rt"),
               new OpRec(Opcodes.max, "Rr,Rs,Rt"),
               new OpRec(Opcodes.minu, "Rr,Rs,Rt"),
               new OpRec(Opcodes.maxu, "Rr,Rs,Rt"),

               new OpRec(Opcodes.moveqz, "Rr,Rs,Rt"),
               new OpRec(Opcodes.movnez, "Rr,Rs,Rt"),
               new OpRec(Opcodes.movltz, "Rr,Rs,Rt"),
               new OpRec(Opcodes.movgez, "Rr,Rs,Rt"),

               null,
               null,
               null,
               null);

            var oprecQRST = new Op1Decoder(
                oprecRST0,
                oprecRST1,
                oprecRST2,
                oprecRST3,

                new ExtuiOpRec(),
                new ExtuiOpRec(),
                new OpRec(Opcodes.cust0, ""),
                new OpRec(Opcodes.cust1, ""),

                oprecLSCX,
                oprecLSC4,
                oprecFP0,
                oprecFP1,

                new OpRec(Opcodes.reserved, ""),
                new OpRec(Opcodes.reserved, ""),
                new OpRec(Opcodes.reserved, ""),
                new OpRec(Opcodes.reserved, ""));

            var oprecLSAI = new r_Rec(
                new OpRec(Opcodes.l8ui, "Rt,Rs,80"),
                new OpRec(Opcodes.l16ui, "Rt,Rs,81"),
                new OpRec(Opcodes.l32i, "Rt,Rs,82"),
                reserved,

                new OpRec(Opcodes.s8i, "Rt,Rs,80"),
                new OpRec(Opcodes.s16i, "Rt,Rs,81"),
                new OpRec(Opcodes.s32i, "Rt,Rs,82"),
                null,

                null,
                new OpRec(Opcodes.l16si, "Rt,Rs,81"),
                new OpRec(Opcodes.movi, "Rt,i"),
                null,

                new OpRec(Opcodes.addi, "Rt,Rs,m0"),
                new OpRec(Opcodes.addmi, "Rt,Rs,m8"),
                null,
                new OpRec(Opcodes.s32ri, "Rt,Rs,82"));

            var oprecLSCI = new r_Rec(
                reserved,
                reserved,
                reserved,
                reserved,

                new OpRec(Opcodes.ssi, "Ft,Rs,82"),
                reserved,
                reserved,
                reserved,

                new OpRec(Opcodes.lsiu, "Ft,Rs,82"),
                reserved,
                reserved,
                reserved,

                reserved,
                reserved,
                reserved,
                reserved);

            var oprecMACID = new Op1Decoder(
                reserved,
                reserved,
                reserved,
                reserved,

                reserved,
                reserved,
                reserved,
                reserved,

                null,
                null,
                null,
                null,

                reserved,
                reserved,
                reserved,
                reserved);
            var oprecMACDD = new Op1Decoder(
                reserved,
                reserved,
                reserved,
                reserved,

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

            var oprecMACC = new Op1Decoder(
                reserved,
                reserved,
                reserved,
                reserved,

                reserved,
                reserved,
                reserved,
                reserved,

                reserved,
                reserved,
                reserved,
                reserved,

                reserved,
                reserved,
                reserved,
                reserved);

            var oprecMAC16 = new Op2Decoder(
                oprecMACID,
                null,
                oprecMACDD,
                null,

                null,
                null,
                null,
                null,

                null,
                oprecMACC,
                reserved,
                reserved,

                reserved,
                reserved,
                reserved,
                reserved);

            var oprecCALLN = new n_Rec(
                new OpRec(Opcodes.call0, "c"),
                new OpRec(Opcodes.call4, "c"),
                new OpRec(Opcodes.call8, "c"),
                new OpRec(Opcodes.call12, "c"));

            deocders = new Decoder[]
            {
                oprecQRST,
                new OpRec(Opcodes.l32r, "Rt,p"),
                oprecLSAI,
                oprecLSCI,

                oprecMAC16,
                oprecCALLN,
                oprecSI,
                oprecB,

                new OpRec(Opcodes.l32i_n, "Rt,Rs,42", true),
                new OpRec(Opcodes.s32i_n, "Rt,Rs,42", true),
                new OpRec(Opcodes.add_n, "Rr,Rs,Rt", true),
                new OpRec(Opcodes.addi_n, "Rr,Rs,a", true),

                oprecST2,
                oprecST3,
                reserved,
                reserved,
            };
        }
    }
}
