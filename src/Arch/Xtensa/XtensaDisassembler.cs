#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
    public class XtensaDisassembler : DisassemblerBase<XtensaInstruction, Mnemonic>
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
                instr = CreateInvalidInstruction();
            };
            instr.Address = state.addr;
            instr.Length = (int)(this.rdr.Address - state.addr);
            return instr;
        }

        public override XtensaInstruction CreateInvalidInstruction()
        {
            return new XtensaInstruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.invalid,
                Operands = MachineInstruction.NoOperands
            };
        }

        private XtensaInstruction DecodeOperands(Mnemonic opcode, string fmt)
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
                Mnemonic = opcode,
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
            private Decoder[] decoders;

            public Op1Decoder(params Decoder [] decoders)
            {
                Debug.Assert(decoders.Length == 16);
                this.decoders = decoders;
            }

            public override XtensaInstruction Decode(XtensaDisassembler dasm)
            {
                return decoders[dasm.state.op1].Decode(dasm);
            }
        }

        public class Op2Decoder : Decoder
        {
            private Decoder[] decoders;

            public Op2Decoder(params Decoder[] decoders)
            {
                Debug.Assert(decoders.Length == 16);
                this.decoders = decoders;
            }

            public override XtensaInstruction Decode(XtensaDisassembler dasm)
            {
                return decoders[dasm.state.op2].Decode(dasm);
            }
        }


        public class r_Rec : Decoder
        {
            private Decoder[] decoders;

            public r_Rec(params Decoder[] decoders)
            {
                Debug.Assert(decoders.Length == 16);
                this.decoders = decoders;
            }

            public override XtensaInstruction Decode(XtensaDisassembler dasm)
            {
                return decoders[dasm.state.r].Decode(dasm);
            }
        }

        public class m_Rec : Decoder
        {
            private Decoder[] decoders;

            public m_Rec(params Decoder[] decoders)
            {
                Debug.Assert(decoders.Length == 4);
                this.decoders = decoders;
            }

            public override XtensaInstruction Decode(XtensaDisassembler dasm)
            {
                return decoders[dasm.state.t >> 2].Decode(dasm);
            }
        }

        public class n_Rec : Decoder
        {
            private Decoder[] decoders;

            public n_Rec(params Decoder[] decoders)
            {
                Debug.Assert(decoders.Length == 4);
                this.decoders = decoders;
            }

            public override XtensaInstruction Decode(XtensaDisassembler dasm)
            {
                return decoders[dasm.state.t & 0x03].Decode(dasm);
            }
        }

        public class s_Rec : Decoder
        {
            private readonly Decoder[] decoders;

            public s_Rec(params Decoder[] decoders)
            {
                Debug.Assert(decoders.Length == 16);
                this.decoders = decoders;
            }

            public override XtensaInstruction Decode(XtensaDisassembler dasm)
            {
                return decoders[dasm.state.s].Decode(dasm);
            }
        }

        public class t_Rec : Decoder
        {
            private Decoder[] decoders;

            public t_Rec(params Decoder[] decoders)
            {
                Debug.Assert(decoders.Length == 16);
                this.decoders = decoders;
            }

            public override XtensaInstruction Decode(XtensaDisassembler dasm)
            {
                return decoders[dasm.state.t].Decode(dasm);
            }
        }

        public class InstrDecoder : Decoder
        {
            private Mnemonic mnemonic;
            private InstrClass iclass;
            private string fmt;
            private bool twoByte;

            public InstrDecoder(Mnemonic mnemonic, string fmt)
            {
                this.mnemonic = mnemonic;
                this.fmt = fmt;
            }

            public InstrDecoder(Mnemonic mnemonic, InstrClass iclass, string fmt)
            {
                this.mnemonic = mnemonic;
                this.iclass = iclass;
                this.fmt = fmt;
            }

            public InstrDecoder(Mnemonic mnemonic, string fmt, bool twoByte)
            {
                this.mnemonic = mnemonic;
                this.fmt = fmt;
                this.twoByte = twoByte;
            }

            public override XtensaInstruction Decode(XtensaDisassembler dasm)
            {
                if (this.twoByte)
                    dasm.rdr.Offset -= 1;
                return dasm.DecodeOperands(mnemonic, fmt);
            }
        }

        public class Movi_nDecoder : Decoder
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
                    Mnemonic = Mnemonic.movi_n,
                    Operands = new MachineOperand[]
                    {
                        dasm.GetAluRegister(dasm.state.s),
                        imm,
                    }
                };
            }
        }

        public class bz_Decoder : Decoder
        {
            private Mnemonic mnemonic;

            public bz_Decoder(Mnemonic mnemonic)
            {
                this.mnemonic = mnemonic;
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
                    Mnemonic = mnemonic,
                    Operands = new MachineOperand[]
                    {
                        dasm.GetAluRegister(dasm.state.s),
                        dst,
                    }
                };

            }
        }

        public class Beqxx_n_Decoder : Decoder
        {
            private Mnemonic mnemonic;

            public Beqxx_n_Decoder(Mnemonic mnemonic)
            {
                this.mnemonic = mnemonic;
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
                    Mnemonic = mnemonic,
                    Operands = new MachineOperand[]
                    {
                        dasm.GetAluRegister(dasm.state.s),
                        dst,
                    }
                };
            }
        }

        public class bbxi_Decoder : Decoder
        {
            private Mnemonic mnemonic;

            public bbxi_Decoder(Mnemonic mnemonic)
            {
                this.mnemonic = mnemonic;
            }

            public override XtensaInstruction Decode(XtensaDisassembler dasm)
            {
                return new XtensaInstruction
                {
                    Mnemonic = this.mnemonic,
                    Operands = new MachineOperand[] {
                        dasm.GetAluRegister(dasm.state.s),
                        ImmediateOperand.Byte((byte)(((dasm.state.r & 1) << 4) | dasm.state.t)),
                        dasm.JumpOffset(dasm.state.imm8)
                    }
                };
            }
        }

        public class ExtuiDecoder : Decoder
        {
            public override XtensaInstruction Decode(XtensaDisassembler dasm)
            {
                return new XtensaInstruction
                {
                    Mnemonic = Mnemonic.extui,
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

            var reserved = new InstrDecoder(Mnemonic.reserved, "");

            var decoderLSCX = new Op1Decoder(
                new InstrDecoder(Mnemonic.lsx,"T,Rs,Rr"),
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

            var decoderLSC4 = new Op2Decoder(
                new InstrDecoder(Mnemonic.l32e, "Rt,Rs,e"),
                reserved,
                reserved,
                reserved,

                new InstrDecoder(Mnemonic.s32e, "Rt,Rs,e"),
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

            var decoderFP0 = new Op2Decoder(
                new InstrDecoder(Mnemonic.add_s, "Fr,Fs,Ft"),
                new InstrDecoder(Mnemonic.sub_s, "Fr,Fs,Ft"),
                new InstrDecoder(Mnemonic.mul_s, "Fr,Fs,Ft"),
                reserved,

                null,
                null,
                null,
                null,

                null,
                null,
                null,
                new InstrDecoder(Mnemonic.floor_s, "Rr,Fs,It"),

                null,
                null,
                null,
                null);

            var decoderFP1 = new Op2Decoder(
                reserved,
                null,
                null,
                new InstrDecoder(Mnemonic.ueq_s, "Br,Fs,Ft"),

                null,
                null,
                null,
                null,

                new InstrDecoder(Mnemonic.moveqz_s, "Fr,Fs,Rt"),
                null,
                null,
                null,

                null,
                null,
                reserved,
                reserved);

            var decoderJR = new n_Rec(
                new InstrDecoder(Mnemonic.ret, ""),
                null,
                new InstrDecoder(Mnemonic.jx, "Rs"),
                reserved);

            var decoderCALLX = new n_Rec(
                new InstrDecoder(Mnemonic.callx0, "Rs"),
                new InstrDecoder(Mnemonic.callx4, "Rs"),
                new InstrDecoder(Mnemonic.callx8, "Rs"),
                new InstrDecoder(Mnemonic.callx12, "Rs"));

            var decoderSNM0 = new m_Rec(
                new InstrDecoder(Mnemonic.ill, InstrClass.Invalid|InstrClass.Zero, ""),
                null,
                decoderJR,
                decoderCALLX);

            var decoderSYNC = new t_Rec(
                new InstrDecoder(Mnemonic.isync, ""),
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

                new InstrDecoder(Mnemonic.memw, ""),
                null,
                null,
                null);

            var decoderRFET = new s_Rec(
                new InstrDecoder(Mnemonic.rfe, ""),
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

            var decoderRFEI = new t_Rec(
                decoderRFET,
                new InstrDecoder(Mnemonic.rfi, "Is"),
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


            var decoderBZ = new m_Rec(
                new bz_Decoder(Mnemonic.beqz),
                new bz_Decoder(Mnemonic.bnez),
                new bz_Decoder(Mnemonic.bltz),
                new bz_Decoder(Mnemonic.bgez));

            var decoderBI0 = new m_Rec(
                new InstrDecoder(Mnemonic.beqi, "Rs,bs,j"),
                new InstrDecoder(Mnemonic.bnei, "Rs,bs,j"),
                new InstrDecoder(Mnemonic.blti, "Rs,bs,j"),
                new InstrDecoder(Mnemonic.bgei, "Rs,bs,j"));

            var decoderBI1 = new m_Rec(
                null,
                null,
                new InstrDecoder(Mnemonic.bltui, "Rs,bu,j"),
                new InstrDecoder(Mnemonic.bgeui, "Rs,bu,j"));

            var decoderSI = new n_Rec(
                new InstrDecoder(Mnemonic.j, "J"),
                decoderBZ,
                decoderBI0,
                decoderBI1);

            var decoderB = new r_Rec(
               new InstrDecoder(Mnemonic.bnone, "Rs,Rt,j"),
               new InstrDecoder(Mnemonic.beq, "Rs,Rt,j"),
               new InstrDecoder(Mnemonic.blt, "Rs,Rt,j"),
               new InstrDecoder(Mnemonic.bltu, "Rs,Rt,j"),

               new InstrDecoder(Mnemonic.ball, "Rs,Rt,j"),
               new InstrDecoder(Mnemonic.bbc, "Rs,Rt,j"),
               new bbxi_Decoder(Mnemonic.bbci),
               new bbxi_Decoder(Mnemonic.bbci),

               new InstrDecoder(Mnemonic.bany, "Rs,Rt,j"),
               new InstrDecoder(Mnemonic.bne, "Rs,Rt,j"),
               new InstrDecoder(Mnemonic.bge, "Rs,Rt,j"),
               new InstrDecoder(Mnemonic.bgeu, "Rs,Rt,j"),

               new InstrDecoder(Mnemonic.bnall, "Rs,Rt,j"),
               new InstrDecoder(Mnemonic.bbs, "Rs,Rt,j"),
               new bbxi_Decoder(Mnemonic.bbsi),
               new bbxi_Decoder(Mnemonic.bbsi));

            var decoderST0 = new r_Rec(
                decoderSNM0,
                null,
                decoderSYNC,
                decoderRFEI,

                new InstrDecoder(Mnemonic.@break, "Is,It"),
                null,
                new InstrDecoder(Mnemonic.rsil, "Rt,Is"),
                null,

                null,
                null,
                null,
                null,

                null,
                null,
                null,
                null);

            var decoderST1 = new r_Rec(
                new InstrDecoder(Mnemonic.ssr, "Rs"),
                new InstrDecoder(Mnemonic.ssl, "Rs"),
                new InstrDecoder(Mnemonic.ssa8l, "Rs"),
                null,

                new InstrDecoder(Mnemonic.ssai, "II"),
                reserved,
                null,
                null,

                null,
                reserved,
                reserved,
                reserved,

                reserved,
                reserved,
                new InstrDecoder(Mnemonic.nsa, "Rt,Rs"),
                new InstrDecoder(Mnemonic.nsau, "Rt,Rs"));

            var decoderST2 = new t_Rec(
                new Movi_nDecoder(),
                new Movi_nDecoder(),
                new Movi_nDecoder(),
                new Movi_nDecoder(),

                new Movi_nDecoder(),
                new Movi_nDecoder(),
                new Movi_nDecoder(),
                new Movi_nDecoder(),

                new Beqxx_n_Decoder(Mnemonic.beqz_n),
                new Beqxx_n_Decoder(Mnemonic.beqz_n),
                new Beqxx_n_Decoder(Mnemonic.beqz_n),
                new Beqxx_n_Decoder(Mnemonic.beqz_n),

                new Beqxx_n_Decoder(Mnemonic.bnez_n),
                new Beqxx_n_Decoder(Mnemonic.bnez_n),
                new Beqxx_n_Decoder(Mnemonic.bnez_n),
                new Beqxx_n_Decoder(Mnemonic.bnez_n));

            var decoderS3 = new t_Rec(
                new InstrDecoder(Mnemonic.ret_n, "", true),
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

            var decoderST3 = new r_Rec(
                new InstrDecoder(Mnemonic.mov_n, "Rt,Rs", true),
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
                decoderS3);

            var decoderRT0 = new s_Rec(
                new InstrDecoder(Mnemonic.neg, "Rr,Rt"),
                new InstrDecoder(Mnemonic.abs, "Rr,Rt"),
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

            var decoderRST0 = new Op2Decoder(
                decoderST0,
                new InstrDecoder(Mnemonic.and, "Rr,Rs,Rt"),
                new InstrDecoder(Mnemonic.or, "Rr,Rs,Rt"),
                new InstrDecoder(Mnemonic.xor, "Rr,Rs,Rt"),

                decoderST1,
                null,
                decoderRT0,
                reserved,

                new InstrDecoder(Mnemonic.add, "Rr,Rs,Rt"),
                new InstrDecoder(Mnemonic.addx2, "Rr,Rs,Rt"),
                new InstrDecoder(Mnemonic.addx4, "Rr,Rs,Rt"),
                new InstrDecoder(Mnemonic.addx8, "Rr,Rs,Rt"),

                new InstrDecoder(Mnemonic.sub, "Rr,Rs,Rt"),
                new InstrDecoder(Mnemonic.subx2, "Rr,Rs,Rt"),
                new InstrDecoder(Mnemonic.subx4, "Rr,Rs,Rt"),
                new InstrDecoder(Mnemonic.subx8, "Rr,Rs,Rt"));

            var decoderIMP = new r_Rec(
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
                new InstrDecoder(Mnemonic.ldpte, ""));       //$TODO: doesn't appear to be documented

            var decoderRST1 = new Op2Decoder(
                new InstrDecoder(Mnemonic.slli, "Rr,Rs,IS"),
                new InstrDecoder(Mnemonic.slli, "Rr,Rs,IS"),
                new InstrDecoder(Mnemonic.srai, "Rr,Rt,IR"),
                new InstrDecoder(Mnemonic.srai, "Rr,Rt,IR"),

                new InstrDecoder(Mnemonic.srli, "Rr,Rt,Is"),
                reserved,
                null,
                null,

                new InstrDecoder(Mnemonic.src, "Rr,Rs,Rt"),
                new InstrDecoder(Mnemonic.srl, "Rr,Rt"),
                new InstrDecoder(Mnemonic.sll, "Rr,Rs"),
                new InstrDecoder(Mnemonic.sra, "Rr,Rs"),

                new InstrDecoder(Mnemonic.mul16u, "Rr,Rs,Rt"),
                new InstrDecoder(Mnemonic.mul16s, "Rr,Rs,Rt"),
                reserved,
                decoderIMP);

            var decoderRST2 = new Op2Decoder(
                new InstrDecoder(Mnemonic.andb, "Br,Bs,Bt"),
                new InstrDecoder(Mnemonic.andbc, "Br,Bs,Bt"),
                new InstrDecoder(Mnemonic.orb, "Br,Bs,Bt"),
                new InstrDecoder(Mnemonic.orbc, "Br,Bs,Bt"),

                new InstrDecoder(Mnemonic.xorb, "Br,Bs,Bt"),
                reserved,
                reserved,
                reserved,

                new InstrDecoder(Mnemonic.mull, "Rr,Rs,Rt"),
                reserved,
                null,
                null,

                new InstrDecoder(Mnemonic.quou, "Rr,Rs,Rt"),
                new InstrDecoder(Mnemonic.quos, "Rr,Rs,Rt"),
                new InstrDecoder(Mnemonic.remu, "Rr,Rs,Rt"),
                new InstrDecoder(Mnemonic.rems, "Rr,Rs,Rt"));

            var decoderRST3 = new Op2Decoder(
               new InstrDecoder(Mnemonic.rsr, "Rt,S"),
               new InstrDecoder(Mnemonic.wsr, "Rt,S"),
               null,
               null,

               new InstrDecoder(Mnemonic.min, "Rr,Rs,Rt"),
               new InstrDecoder(Mnemonic.max, "Rr,Rs,Rt"),
               new InstrDecoder(Mnemonic.minu, "Rr,Rs,Rt"),
               new InstrDecoder(Mnemonic.maxu, "Rr,Rs,Rt"),

               new InstrDecoder(Mnemonic.moveqz, "Rr,Rs,Rt"),
               new InstrDecoder(Mnemonic.movnez, "Rr,Rs,Rt"),
               new InstrDecoder(Mnemonic.movltz, "Rr,Rs,Rt"),
               new InstrDecoder(Mnemonic.movgez, "Rr,Rs,Rt"),

               null,
               null,
               null,
               null);

            var decoderQRST = new Op1Decoder(
                decoderRST0,
                decoderRST1,
                decoderRST2,
                decoderRST3,

                new ExtuiDecoder(),
                new ExtuiDecoder(),
                new InstrDecoder(Mnemonic.cust0, ""),
                new InstrDecoder(Mnemonic.cust1, ""),

                decoderLSCX,
                decoderLSC4,
                decoderFP0,
                decoderFP1,

                new InstrDecoder(Mnemonic.reserved, ""),
                new InstrDecoder(Mnemonic.reserved, ""),
                new InstrDecoder(Mnemonic.reserved, ""),
                new InstrDecoder(Mnemonic.reserved, ""));

            var decoderLSAI = new r_Rec(
                new InstrDecoder(Mnemonic.l8ui, "Rt,Rs,80"),
                new InstrDecoder(Mnemonic.l16ui, "Rt,Rs,81"),
                new InstrDecoder(Mnemonic.l32i, "Rt,Rs,82"),
                reserved,

                new InstrDecoder(Mnemonic.s8i, "Rt,Rs,80"),
                new InstrDecoder(Mnemonic.s16i, "Rt,Rs,81"),
                new InstrDecoder(Mnemonic.s32i, "Rt,Rs,82"),
                null,

                null,
                new InstrDecoder(Mnemonic.l16si, "Rt,Rs,81"),
                new InstrDecoder(Mnemonic.movi, "Rt,i"),
                null,

                new InstrDecoder(Mnemonic.addi, "Rt,Rs,m0"),
                new InstrDecoder(Mnemonic.addmi, "Rt,Rs,m8"),
                null,
                new InstrDecoder(Mnemonic.s32ri, "Rt,Rs,82"));

            var decoderLSCI = new r_Rec(
                reserved,
                reserved,
                reserved,
                reserved,

                new InstrDecoder(Mnemonic.ssi, "Ft,Rs,82"),
                reserved,
                reserved,
                reserved,

                new InstrDecoder(Mnemonic.lsiu, "Ft,Rs,82"),
                reserved,
                reserved,
                reserved,

                reserved,
                reserved,
                reserved,
                reserved);

            var decoderMACID = new Op1Decoder(
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
            var decoderMACDD = new Op1Decoder(
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

            var decoderMACC = new Op1Decoder(
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

            var decoderMAC16 = new Op2Decoder(
                decoderMACID,
                null,
                decoderMACDD,
                null,

                null,
                null,
                null,
                null,

                null,
                decoderMACC,
                reserved,
                reserved,

                reserved,
                reserved,
                reserved,
                reserved);

            var decoderCALLN = new n_Rec(
                new InstrDecoder(Mnemonic.call0, "c"),
                new InstrDecoder(Mnemonic.call4, "c"),
                new InstrDecoder(Mnemonic.call8, "c"),
                new InstrDecoder(Mnemonic.call12, "c"));

            deocders = new Decoder[]
            {
                decoderQRST,
                new InstrDecoder(Mnemonic.l32r, "Rt,p"),
                decoderLSAI,
                decoderLSCI,

                decoderMAC16,
                decoderCALLN,
                decoderSI,
                decoderB,

                new InstrDecoder(Mnemonic.l32i_n, "Rt,Rs,42", true),
                new InstrDecoder(Mnemonic.s32i_n, "Rt,Rs,42", true),
                new InstrDecoder(Mnemonic.add_n, "Rr,Rs,Rt", true),
                new InstrDecoder(Mnemonic.addi_n, "Rr,Rs,a", true),

                decoderST2,
                decoderST3,
                reserved,
                reserved,
            };
        }
    }
}
