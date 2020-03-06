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
using System.Linq;

#pragma warning disable IDE1006 // Naming Styles


namespace Reko.Arch.Xtensa
{
    using Decoder = Reko.Core.Machine.Decoder<XtensaDisassembler, Mnemonic, XtensaInstruction>;

    public class XtensaDisassembler : DisassemblerBase<XtensaInstruction, Mnemonic>
    {
        private static readonly Decoder[] rootDecoder;
        private static readonly int[] b4const;
        private static readonly int[] b4constu;

        private readonly XtensaArchitecture arch;
        private readonly EndianImageReader rdr;
        private readonly List<MachineOperand> ops;
        private State state;

        public XtensaDisassembler(XtensaArchitecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.ops = new List<MachineOperand>();
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
            ops.Clear();

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
                instr = rootDecoder[state.op0].Decode(0, this);
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

        public override XtensaInstruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            return new XtensaInstruction
            {
                InstructionClass = iclass,
                Mnemonic = mnemonic,
                Operands = ops.ToArray()
            };
        }

        public override XtensaInstruction NotYetImplemented(uint wInstr, string message)
        {
            var len = rdr.Address - this.state.addr;
            var rdr2 = rdr.Clone();
            rdr2.Offset -= len;
            var hexBytes = string.Join("", rdr2.ReadBytes((int) len).Select(b => b.ToString("X2")));

            base.EmitUnitTest("Xtensa", hexBytes, message, "Xtdasm", this.state.addr, w =>
            {
                w.WriteLine("AssertCode(\"@@@\", \"{0}\");", hexBytes);
            });
            return CreateInvalidInstruction();
        }

        // GP register
        private static bool Rr(uint wInstr, XtensaDisassembler dasm)
        {
            var reg = dasm.arch.GetAluRegister(dasm.state.r);
            dasm.ops.Add(new RegisterOperand(reg));
            return true;
        }

        private static bool Rs(uint wInstr, XtensaDisassembler dasm)
        {
            var reg = dasm.arch.GetAluRegister(dasm.state.s);
            dasm.ops.Add(new RegisterOperand(reg));
            return true;
        }

        private static bool Rt(uint wInstr, XtensaDisassembler dasm)
        {
            var reg = dasm.arch.GetAluRegister(dasm.state.t);
            dasm.ops.Add(new RegisterOperand(reg));
            return true;
        }

        // FPU register
        private static bool Fr(uint wInstr, XtensaDisassembler dasm)
        {
            var freg = dasm.arch.GetFpuRegister(dasm.state.r);
            dasm.ops.Add(new RegisterOperand(freg));
            return true;
        }

        private static bool Fs(uint wInstr, XtensaDisassembler dasm)
        {
            var freg = dasm.arch.GetFpuRegister(dasm.state.s);
            dasm.ops.Add(new RegisterOperand(freg));
            return true;
        }

        private static bool Ft(uint wInstr, XtensaDisassembler dasm)
        {
            var freg = dasm.arch.GetFpuRegister(dasm.state.t);
            dasm.ops.Add(new RegisterOperand(freg));
            return true;
        }

        // Special register
        private static bool S(uint wInstr, XtensaDisassembler dasm)
        {
            var reg = dasm.arch.GetSpecialRegister((dasm.state.r << 4) | dasm.state.s); ;
            dasm.ops.Add(new RegisterOperand(reg));
            return true;

        }


        // 'e'
        private static bool e(uint wInstr, XtensaDisassembler dasm)
        {
            var off = ~0x3F | (dasm.state.r << 2);
            dasm.ops.Add(ImmediateOperand.Int32(off));
            return true;
        }

        // Bool register
        private static bool Br(uint wInstr, XtensaDisassembler dasm)
        {
            var reg = dasm.arch.GetBoolRegister(dasm.state.r);
            dasm.ops.Add(new RegisterOperand(reg));
            return true;
        }

        private static bool Bs(uint wInstr, XtensaDisassembler dasm)
        {
            var reg = dasm.arch.GetBoolRegister(dasm.state.s);
            dasm.ops.Add(new RegisterOperand(reg));
            return true;
        }

        private static bool Bt(uint wInstr, XtensaDisassembler dasm)
        {
            var reg = dasm.arch.GetBoolRegister(dasm.state.t);
            dasm.ops.Add(new RegisterOperand(reg));
            return true;
        }


        // Immediate
        private static bool Is(uint wInstr, XtensaDisassembler dasm)
        {
            var op = ImmediateOperand.Byte(dasm.state.s);
            dasm.ops.Add(op);
            return true;
        }

        private static bool It(uint wInstr, XtensaDisassembler dasm)
        {
            var op = ImmediateOperand.Byte(dasm.state.t);
            dasm.ops.Add(op);
            return true;
        }

        // Immediate shift amount used by slli
        private static bool IS(uint wInstr, XtensaDisassembler dasm)
        {
            var op = ImmediateOperand.Byte((byte) (dasm.state.t | ((dasm.state.op2 & 1) << 4)));
            dasm.ops.Add(op);
            return true;
        }

        // Immediate shift amount used by srai
        private static bool IR(uint wInstr, XtensaDisassembler dasm)
        {
            var op = ImmediateOperand.Byte((byte) (dasm.state.s | ((dasm.state.op2 & 1) << 4)));
            dasm.ops.Add(op);
            return true;
        }


        // Immediate shift amount used by ssai
        private static bool II(uint wInstr, XtensaDisassembler dasm)
        { 
            var op = ImmediateOperand.Byte((byte) (dasm.state.s | ((dasm.state.r & 1) << 4)));
            dasm.ops.Add(op);
            return true;
        }


        private static bool I4_2(uint wInstr, XtensaDisassembler dasm)
        {
            var op = ImmediateOperand.Byte((byte)(dasm.state.r << 2)); 
            dasm.ops.Add(op);
            return true;
        }

        private static bool I8_0(uint wInstr, XtensaDisassembler dasm)
        {
            var op = ImmediateOperand.UInt16((ushort)dasm.state.imm8);
            dasm.ops.Add(op);
            return true;
        }

        private static bool I8_1(uint wInstr, XtensaDisassembler dasm)
        {
            var op = ImmediateOperand.UInt16((ushort) (dasm.state.imm8 << 1));
            dasm.ops.Add(op);
            return true;
        }

        private static bool I8_2(uint wInstr, XtensaDisassembler dasm)
        {
            var op = ImmediateOperand.UInt16((ushort) (dasm.state.imm8 << 2));
            dasm.ops.Add(op);
            return true;
        }

        private static bool Irt(uint wInstr, XtensaDisassembler dasm)
        {
            var op = ImmediateOperand.Byte((byte) (((dasm.state.r & 1) << 4) | dasm.state.t));
            dasm.ops.Add(op);
            return true;
        }

        private static bool a(uint wInstr, XtensaDisassembler dasm)
        {
            var op = ImmediateOperand.SByte(dasm.state.t == 0 ? (sbyte) -1 : (sbyte) dasm.state.t);
            dasm.ops.Add(op);
            return true;
        }

        // Scaled immediates
        private static bool m0(uint wInstr, XtensaDisassembler dasm)
        {
            int n = (sbyte) dasm.state.imm8;
            var op = ImmediateOperand.Word32(n);
            dasm.ops.Add(op);
            return true;
        }

        private static bool m8(uint wInstr, XtensaDisassembler dasm)
        {
            int n = (sbyte) dasm.state.imm8;
            n <<= 8;
            var op = ImmediateOperand.Word32(n);
            dasm.ops.Add(op);
            return true;
        }

        // Split immediate
        private static bool i(uint wInstr, XtensaDisassembler dasm)
        {
            int n = ((dasm.state.imm8 | (dasm.state.s << 8)) << 20) >> 20;
            dasm.ops.Add(ImmediateOperand.Word32(n));
            return true;
        }

        // 'bs'
        private static bool bs(uint wInstr, XtensaDisassembler dasm)
        {
            var op = ImmediateOperand.Int32(b4const[dasm.state.r]);
            dasm.ops.Add(op);
            return true;
        }

        private static bool bu(uint wInstr, XtensaDisassembler dasm)
        {
            var op = ImmediateOperand.Word32(b4constu[dasm.state.r]);
            dasm.ops.Add(op);
            return true;
        }

        // Jump
        private static bool j(uint wInstr, XtensaDisassembler dasm)
        { 
            int dst = (int) dasm.state.addr.ToUInt32() + (sbyte) dasm.state.imm8 + 4;
            dasm.ops.Add(AddressOperand.Ptr32((uint) dst));
            return true;
        }

        private static bool jrt(uint wInstr, XtensaDisassembler dasm)
        {
            var n =
                dasm.state.r |
                ((dasm.state.t & 0x3) << 4);
            int dst = (int) dasm.state.addr.ToUInt32() + (sbyte) n + 4;
            dasm.ops.Add(AddressOperand.Ptr32((uint) dst));
            return true;
        }



        private static bool J(uint wInstr, XtensaDisassembler dasm)
        {
            var op = AddressOperand.Ptr32((uint) ((int) dasm.state.addr.ToLinear() + (dasm.state.offset + 4)));
            dasm.ops.Add(op);
            return true;
        }

        // Call offset
        private static bool c(uint wInstr, XtensaDisassembler dasm)
        {
            var op = AddressOperand.Ptr32((uint) ((dasm.state.addr.ToUInt32() & ~3) + (dasm.state.offset << 2) + 4));
            dasm.ops.Add(op);
            return true;
        }

        // NegativePcRelativeAddress();
        private static bool p(uint wInstr, XtensaDisassembler dasm)
        {
            var nAddr =
                ((0xFFFF0000 | dasm.state.imm16) << 2) +
                 (int) (dasm.rdr.Address.ToUInt32() & ~3);
            var op = AddressOperand.Ptr32((uint) nAddr);
            dasm.ops.Add(op);
            return true;
        }

        private MachineOperand GetAluRegister(byte r)
        {
            return new RegisterOperand(arch.GetAluRegister(r));
        }

        public class Op1Decoder : Decoder
        {
            private readonly Decoder[] decoders;

            public Op1Decoder(params Decoder [] decoders)
            {
                Debug.Assert(decoders.Length == 16);
                this.decoders = decoders;
            }

            public override XtensaInstruction Decode(uint wInstr, XtensaDisassembler dasm)
            {
                return decoders[dasm.state.op1].Decode(wInstr, dasm);
            }
        }

        public class Op2Decoder : Decoder
        {
            private readonly Decoder[] decoders;

            public Op2Decoder(params Decoder[] decoders)
            {
                Debug.Assert(decoders.Length == 16);
                this.decoders = decoders;
            }

            public override XtensaInstruction Decode(uint wInstr, XtensaDisassembler dasm)
            {
                return decoders[dasm.state.op2].Decode(wInstr, dasm);
            }
        }


        public class r_Rec : Decoder
        {
            private readonly Decoder[] decoders;

            public r_Rec(params Decoder[] decoders)
            {
                Debug.Assert(decoders.Length == 16);
                this.decoders = decoders;
            }

            public override XtensaInstruction Decode(uint wInstr, XtensaDisassembler dasm)
            {
                return decoders[dasm.state.r].Decode(wInstr, dasm);
            }
        }

        public class m_Rec : Decoder
        {
            private readonly Decoder[] decoders;

            public m_Rec(params Decoder[] decoders)
            {
                Debug.Assert(decoders.Length == 4);
                this.decoders = decoders;
            }

            public override XtensaInstruction Decode(uint wInstr, XtensaDisassembler dasm)
            {
                return decoders[dasm.state.t >> 2].Decode(wInstr, dasm);
            }
        }

        public class n_Rec : Decoder
        {
            private readonly Decoder[] decoders;

            public n_Rec(params Decoder[] decoders)
            {
                Debug.Assert(decoders.Length == 4);
                this.decoders = decoders;
            }

            public override XtensaInstruction Decode(uint wInstr, XtensaDisassembler dasm)
            {
                return decoders[dasm.state.t & 0x03].Decode(wInstr, dasm);
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

            public override XtensaInstruction Decode(uint wInstr, XtensaDisassembler dasm)
            {
                return decoders[dasm.state.s].Decode(wInstr, dasm);
            }
        }

        public class t_Rec : Decoder
        {
            private readonly Decoder[] decoders;

            public t_Rec(params Decoder[] decoders)
            {
                Debug.Assert(decoders.Length == 16);
                this.decoders = decoders;
            }

            public override XtensaInstruction Decode(uint wInstr, XtensaDisassembler dasm)
            {
                return decoders[dasm.state.t].Decode(wInstr, dasm);
            }
        }

        public class InstrDecoder : Decoder<XtensaDisassembler, Mnemonic, XtensaInstruction>
        {
            private readonly InstrClass iclass;
            private readonly Mnemonic mnemonic;
            private readonly bool twoByte;
            private readonly Mutator<XtensaDisassembler>[] mutators;

            public InstrDecoder(InstrClass iclass, Mnemonic mnemonic, bool twoByte, params Mutator<XtensaDisassembler> [] mutators)
            {
                this.iclass = iclass;
                this.mnemonic = mnemonic;
                this.twoByte = twoByte;
                this.mutators = mutators;
            }

            public override XtensaInstruction Decode(uint wInstr, XtensaDisassembler dasm)
            {
                if (this.twoByte)
                    dasm.rdr.Offset -= 1;
                foreach (var m in mutators)
                {
                    if (!m(wInstr, dasm))
                        return dasm.CreateInvalidInstruction();
                }
                return dasm.MakeInstruction(iclass, mnemonic);
            }
        }



        public class Movi_nDecoder : Decoder
        {
            public override XtensaInstruction Decode(uint wInstr, XtensaDisassembler dasm)
            {
                Rs(wInstr, dasm);
                var n = 
                    dasm.state.r |
                    ((dasm.state.t & 0x7) << 4) |
                    (((dasm.state.t & 0x6) == 0x6)
                        ? ~0x1F
                        : 0);
                var imm = ImmediateOperand.SByte((sbyte)n);
                dasm.ops.Add(imm);

                // this is a 2-byte instruction, so back up one byte.
                dasm.rdr.Offset -= 1;
                return dasm.MakeInstruction(InstrClass.Linear, Mnemonic.movi_n);
            }
        }

        public class bz_Decoder : Decoder
        {
            private readonly Mnemonic mnemonic;

            public bz_Decoder(Mnemonic mnemonic)
            {
                this.mnemonic = mnemonic;
            }

            public override XtensaInstruction Decode(uint wInstr, XtensaDisassembler dasm)
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

        public class ExtuiDecoder : Decoder
        {
            public override XtensaInstruction Decode(uint wInstr, XtensaDisassembler dasm)
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

        private static InstrDecoder Instr(Mnemonic mnemonic, params Mutator<XtensaDisassembler> [] mutators)
        {
            return new InstrDecoder(InstrClass.Linear, mnemonic, false, mutators);
        }

        private static InstrDecoder Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<XtensaDisassembler>[] mutators)
        {
            return new InstrDecoder(iclass, mnemonic, false, mutators);
        }

        private static InstrDecoder Instr2byte(Mnemonic mnemonic, InstrClass iclass, params Mutator<XtensaDisassembler>[] mutators)
        {
            return new InstrDecoder(iclass, mnemonic, true, mutators);
        }


        private static Decoder Nyi(string message)
        {
            return new NyiDecoder<XtensaDisassembler, Mnemonic, XtensaInstruction>(message);
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

            var reserved = Instr(Mnemonic.reserved);

            var decoderLSCX = new Op1Decoder(
                Instr(Mnemonic.lsx, Fr,Rs,Rt),
                Nyi("lsxu"),
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
                Instr(Mnemonic.l32e, Rt,Rs,e),
                reserved,
                reserved,
                reserved,

                Instr(Mnemonic.s32e, Rt,Rs,e),
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
                Instr(Mnemonic.add_s, Fr,Fs,Ft),
                Instr(Mnemonic.sub_s, Fr,Fs,Ft),
                Instr(Mnemonic.mul_s, Fr,Fs,Ft),
                reserved,

                null,
                null,
                null,
                null,

                null,
                null,
                null,
                Instr(Mnemonic.floor_s, Rr,Fs,It),

                null,
                null,
                null,
                null);

            var decoderFP1 = new Op2Decoder(
                reserved,
                null,
                null,
                Instr(Mnemonic.ueq_s, Br,Fs,Ft),

                null,
                null,
                null,
                null,

                Instr(Mnemonic.moveqz_s, Fr,Fs,Rt),
                null,
                null,
                null,

                null,
                null,
                reserved,
                reserved);

            var decoderJR = new n_Rec(
                Instr2byte(Mnemonic.ret, InstrClass.Transfer),
                null,
                Instr(Mnemonic.jx, Rs),
                reserved);

            var decoderCALLX = new n_Rec(
                Instr(Mnemonic.callx0, Rs),
                Instr(Mnemonic.callx4, Rs),
                Instr(Mnemonic.callx8, Rs),
                Instr(Mnemonic.callx12, Rs));

            var decoderSNM0 = new m_Rec(
                Instr(Mnemonic.ill, InstrClass.Invalid|InstrClass.Zero),
                null,
                decoderJR,
                decoderCALLX);

            var decoderSYNC = new t_Rec(
                Instr(Mnemonic.isync),
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

                Instr(Mnemonic.memw),
                null,
                null,
                null);

            var decoderRFET = new s_Rec(
                Instr(Mnemonic.rfe),
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
                Instr(Mnemonic.rfi, Is),
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
                Instr(Mnemonic.beqi, Rs,bs,j),
                Instr(Mnemonic.bnei, Rs,bs,j),
                Instr(Mnemonic.blti, Rs,bs,j),
                Instr(Mnemonic.bgei, Rs,bs,j));

            var decoderBI1 = new m_Rec(
                null,
                null,
                Instr(Mnemonic.bltui, Rs,bu,j),
                Instr(Mnemonic.bgeui, Rs,bu,j));

            var decoderSI = new n_Rec(
                Instr(Mnemonic.j, J),
                decoderBZ,
                decoderBI0,
                decoderBI1);

            var decoderB = new r_Rec(
               Instr(Mnemonic.bnone, Rs,Rt,j),
               Instr(Mnemonic.beq, Rs,Rt,j),
               Instr(Mnemonic.blt, Rs,Rt,j),
               Instr(Mnemonic.bltu, Rs,Rt,j),

               Instr(Mnemonic.ball, Rs,Rt,j),
               Instr(Mnemonic.bbc, Rs,Rt,j),
               Instr(Mnemonic.bbci, InstrClass.ConditionalTransfer, Rs, Irt, j),
               Instr(Mnemonic.bbci, InstrClass.ConditionalTransfer, Rs, Irt, j),

               Instr(Mnemonic.bany, Rs,Rt,j),
               Instr(Mnemonic.bne, Rs,Rt,j),
               Instr(Mnemonic.bge, Rs,Rt,j),
               Instr(Mnemonic.bgeu, Rs,Rt,j),

               Instr(Mnemonic.bnall, Rs,Rt,j),
               Instr(Mnemonic.bbs, Rs,Rt,j),
               Instr(Mnemonic.bbsi, InstrClass.ConditionalTransfer, Rs, Irt, j),
               Instr(Mnemonic.bbsi, InstrClass.ConditionalTransfer, Rs, Irt, j));

            var decoderST0 = new r_Rec(
                decoderSNM0,
                null,
                decoderSYNC,
                decoderRFEI,

                Instr(Mnemonic.@break, Is,It),
                null,
                Instr(Mnemonic.rsil, Rt,Is),
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
                Instr(Mnemonic.ssr, Rs),
                Instr(Mnemonic.ssl, Rs),
                Instr(Mnemonic.ssa8l, Rs),
                null,

                Instr(Mnemonic.ssai, II),
                reserved,
                null,
                null,

                null,
                reserved,
                reserved,
                reserved,

                reserved,
                reserved,
                Instr(Mnemonic.nsa, Rt,Rs),
                Instr(Mnemonic.nsau, Rt,Rs));

            var decoderST2 = new t_Rec(
                new Movi_nDecoder(),
                new Movi_nDecoder(),
                new Movi_nDecoder(),
                new Movi_nDecoder(),

                new Movi_nDecoder(),
                new Movi_nDecoder(),
                new Movi_nDecoder(),
                new Movi_nDecoder(),

                Instr2byte(Mnemonic.beqz_n, InstrClass.ConditionalTransfer, Rs, jrt),
                Instr2byte(Mnemonic.beqz_n, InstrClass.ConditionalTransfer, Rs, jrt),
                Instr2byte(Mnemonic.beqz_n, InstrClass.ConditionalTransfer, Rs, jrt),
                Instr2byte(Mnemonic.beqz_n, InstrClass.ConditionalTransfer, Rs, jrt),

                Instr2byte(Mnemonic.bnez_n, InstrClass.ConditionalTransfer, Rs, jrt),
                Instr2byte(Mnemonic.bnez_n, InstrClass.ConditionalTransfer, Rs, jrt),
                Instr2byte(Mnemonic.bnez_n, InstrClass.ConditionalTransfer, Rs, jrt),
                Instr2byte(Mnemonic.bnez_n, InstrClass.ConditionalTransfer, Rs, jrt));

            var decoderS3 = new t_Rec(
                Instr2byte(Mnemonic.ret_n, InstrClass.Transfer),
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
                Instr2byte(Mnemonic.mov_n, InstrClass.Linear, Rt,Rs),
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
                Instr(Mnemonic.neg, Rr,Rt),
                Instr(Mnemonic.abs, Rr,Rt),
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
                Instr(Mnemonic.and, Rr,Rs,Rt),
                Instr(Mnemonic.or, Rr,Rs,Rt),
                Instr(Mnemonic.xor, Rr,Rs,Rt),

                decoderST1,
                null,
                decoderRT0,
                reserved,

                Instr(Mnemonic.add, Rr,Rs,Rt),
                Instr(Mnemonic.addx2, Rr,Rs,Rt),
                Instr(Mnemonic.addx4, Rr,Rs,Rt),
                Instr(Mnemonic.addx8, Rr,Rs,Rt),

                Instr(Mnemonic.sub, Rr,Rs,Rt),
                Instr(Mnemonic.subx2, Rr,Rs,Rt),
                Instr(Mnemonic.subx4, Rr,Rs,Rt),
                Instr(Mnemonic.subx8, Rr,Rs,Rt));

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
                Instr(Mnemonic.ldpte));       //$TODO: doesn't appear to be documented

            var decoderRST1 = new Op2Decoder(
                Instr(Mnemonic.slli, Rr,Rs,IS),
                Instr(Mnemonic.slli, Rr,Rs,IS),
                Instr(Mnemonic.srai, Rr,Rt,IR),
                Instr(Mnemonic.srai, Rr,Rt,IR),

                Instr(Mnemonic.srli, Rr,Rt,Is),
                reserved,
                null,
                null,

                Instr(Mnemonic.src, Rr,Rs,Rt),
                Instr(Mnemonic.srl, Rr,Rt),
                Instr(Mnemonic.sll, Rr,Rs),
                Instr(Mnemonic.sra, Rr,Rs),

                Instr(Mnemonic.mul16u, Rr,Rs,Rt),
                Instr(Mnemonic.mul16s, Rr,Rs,Rt),
                reserved,
                decoderIMP);

            var decoderRST2 = new Op2Decoder(
                Instr(Mnemonic.andb, Br,Bs,Bt),
                Instr(Mnemonic.andbc, Br,Bs,Bt),
                Instr(Mnemonic.orb, Br,Bs,Bt),
                Instr(Mnemonic.orbc, Br,Bs,Bt),

                Instr(Mnemonic.xorb, Br,Bs,Bt),
                reserved,
                reserved,
                reserved,

                Instr(Mnemonic.mull, Rr,Rs,Rt),
                reserved,
                null,
                null,

                Instr(Mnemonic.quou, Rr,Rs,Rt),
                Instr(Mnemonic.quos, Rr,Rs,Rt),
                Instr(Mnemonic.remu, Rr,Rs,Rt),
                Instr(Mnemonic.rems, Rr,Rs,Rt));

            var decoderRST3 = new Op2Decoder(
               Instr(Mnemonic.rsr, Rt,S),
               Instr(Mnemonic.wsr, Rt,S),
               null,
               null,

               Instr(Mnemonic.min, Rr,Rs,Rt),
               Instr(Mnemonic.max, Rr,Rs,Rt),
               Instr(Mnemonic.minu, Rr,Rs,Rt),
               Instr(Mnemonic.maxu, Rr,Rs,Rt),

               Instr(Mnemonic.moveqz, Rr,Rs,Rt),
               Instr(Mnemonic.movnez, Rr,Rs,Rt),
               Instr(Mnemonic.movltz, Rr,Rs,Rt),
               Instr(Mnemonic.movgez, Rr,Rs,Rt),

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
                Instr(Mnemonic.cust0),
                Instr(Mnemonic.cust1),

                decoderLSCX,
                decoderLSC4,
                decoderFP0,
                decoderFP1,

                Instr(Mnemonic.reserved),
                Instr(Mnemonic.reserved),
                Instr(Mnemonic.reserved),
                Instr(Mnemonic.reserved));

            var decoderLSAI = new r_Rec(
                Instr(Mnemonic.l8ui, Rt,Rs,I8_0),
                Instr(Mnemonic.l16ui, Rt,Rs,I8_1),
                Instr(Mnemonic.l32i, Rt,Rs,I8_2),
                reserved,

                Instr(Mnemonic.s8i, Rt,Rs,I8_0),
                Instr(Mnemonic.s16i, Rt,Rs,I8_1),
                Instr(Mnemonic.s32i, Rt,Rs,I8_2),
                null,

                null,
                Instr(Mnemonic.l16si, Rt,Rs,I8_1),
                Instr(Mnemonic.movi, Rt,i),
                null,

                Instr(Mnemonic.addi, Rt,Rs,m0),
                Instr(Mnemonic.addmi, Rt,Rs,m8),
                null,
                Instr(Mnemonic.s32ri, Rt,Rs,I8_2));

            var decoderLSCI = new r_Rec(
                reserved,
                reserved,
                reserved,
                reserved,

                Instr(Mnemonic.ssi, Ft,Rs,I8_2),
                reserved,
                reserved,
                reserved,

                Instr(Mnemonic.lsiu, Ft,Rs,I8_2),
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
                Instr(Mnemonic.call0, c),
                Instr(Mnemonic.call4, c),
                Instr(Mnemonic.call8, c),
                Instr(Mnemonic.call12, c));

            rootDecoder = new Decoder[]
            {
                decoderQRST,
                Instr(Mnemonic.l32r, Rt,p),
                decoderLSAI,
                decoderLSCI,

                decoderMAC16,
                decoderCALLN,
                decoderSI,
                decoderB,

                Instr2byte(Mnemonic.l32i_n,InstrClass.Linear, Rt,Rs,I4_2),
                Instr2byte(Mnemonic.s32i_n,InstrClass.Linear, Rt,Rs,I4_2),
                Instr2byte(Mnemonic.add_n, InstrClass.Linear, Rr,Rs,Rt),
                Instr2byte(Mnemonic.addi_n,InstrClass.Linear, Rr,Rs,a),

                decoderST2,
                decoderST3,
                reserved,
                reserved,
            };
        }
    }
}
