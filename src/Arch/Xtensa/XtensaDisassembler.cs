#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Lib;
using Reko.Core.Services;
using Reko.Core.Memory;

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

        public override XtensaInstruction? DisassembleInstruction()
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
            state.offset = (int) Bits.SignExtend((ulong)((b2 << 16) | (b1 << 8) | b0) >> 6, 18);

            var instr = rootDecoder[state.op0].Decode(0, this);
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
                Operands = Array.Empty<MachineOperand>()
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

        public override XtensaInstruction NotYetImplemented(string message)
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingDecoder("Xtdasm", this.state.addr, this.rdr, message);
            return CreateInvalidInstruction();
        }

        // GP register
        private static bool Rr(uint wInstr, XtensaDisassembler dasm)
        {
            var reg = dasm.arch.GetAluRegister(dasm.state.r);
            dasm.ops.Add(reg);
            return true;
        }

        private static bool Rs(uint wInstr, XtensaDisassembler dasm)
        {
            var reg = dasm.arch.GetAluRegister(dasm.state.s);
            dasm.ops.Add(reg);
            return true;
        }

        private static bool Rt(uint wInstr, XtensaDisassembler dasm)
        {
            var reg = dasm.arch.GetAluRegister(dasm.state.t);
            dasm.ops.Add(reg);
            return true;
        }

        // FPU register
        private static bool Fr(uint wInstr, XtensaDisassembler dasm)
        {
            var freg = dasm.arch.GetFpuRegister(dasm.state.r);
            dasm.ops.Add(freg);
            return true;
        }

        private static bool Fs(uint wInstr, XtensaDisassembler dasm)
        {
            var freg = dasm.arch.GetFpuRegister(dasm.state.s);
            dasm.ops.Add(freg);
            return true;
        }

        private static bool Ft(uint wInstr, XtensaDisassembler dasm)
        {
            var freg = dasm.arch.GetFpuRegister(dasm.state.t);
            dasm.ops.Add(freg);
            return true;
        }

        // MAC16 register

        private static bool Mr(uint _, XtensaDisassembler dasm)
        {
            var mac16reg = dasm.arch.GetMac16Register(dasm.state.r & 3);
            if (mac16reg is null)
                return false;
            dasm.ops.Add(mac16reg);
            return true;
        }

        // Mr operand can be m0 or m1.
        private static bool Mr_01(uint _, XtensaDisassembler dasm)
        {
            var mac16reg = dasm.arch.GetMac16Register((dasm.state.r >> 2) & 1);
            if (mac16reg is null)
                return false;
            dasm.ops.Add(mac16reg);
            return true;
        }

        private static bool Mt(uint wInstr, XtensaDisassembler dasm)
        {
            var mac16reg = dasm.arch.GetMac16Register(dasm.state.t);
            if (mac16reg is null)
                return false;
            dasm.ops.Add(mac16reg);
            return true;
        }

        // Mt operand can be m2 or m3.

        private static bool Mt_23(uint wInstr, XtensaDisassembler dasm)
        {
            var mac16reg = dasm.arch.GetMac16Register((dasm.state.t >> 2) | 0b10);
            if (mac16reg is null)
                return false;
            dasm.ops.Add(mac16reg);
            return true;
        }

        // Special register
        private static bool S(uint wInstr, XtensaDisassembler dasm)
        {
            //$REVIEW: Is this correct in bigendian?
            var reg = dasm.arch.GetSpecialRegister((dasm.state.r << 4) | dasm.state.s);
            if (reg is null)
                return false;
            dasm.ops.Add(reg);
            return true;
        }

        // User register
        private static bool Ust(uint wINstr, XtensaDisassembler dasm)
        {
            var reg = dasm.arch.GetUserRegister((dasm.state.s << 4) | dasm.state.t);
            dasm.ops.Add(reg);
            return true;
        }

        private static bool Usr(uint wINstr, XtensaDisassembler dasm)
        {
            var reg = dasm.arch.GetUserRegister((dasm.state.s << 4) | dasm.state.r);
            dasm.ops.Add(reg);
            return true;
        }

        // 'e'
        private static bool e(uint wInstr, XtensaDisassembler dasm)
        {
            var off = ~0x3F | (dasm.state.r << 2);
            dasm.ops.Add(Constant.Int32(off));
            return true;
        }

        // Bool register
        private static bool Br(uint wInstr, XtensaDisassembler dasm)
        {
            var reg = dasm.arch.GetBoolRegister(dasm.state.r);
            dasm.ops.Add(reg);
            return true;
        }

        private static bool Bs(uint wInstr, XtensaDisassembler dasm)
        {
            var reg = dasm.arch.GetBoolRegister(dasm.state.s);
            dasm.ops.Add(reg);
            return true;
        }

        private static Mutator<XtensaDisassembler> Bs_MultipleOf(int multiple)
        {
            return (uint wInstr, XtensaDisassembler dasm) =>
            {
                var iReg = dasm.state.s;
                if (iReg % multiple != 0)
                    return false;
                var reg = dasm.arch.GetBoolRegister(iReg);
                dasm.ops.Add(reg);
                return true;
            };
        }
        
        private static bool Bt(uint wInstr, XtensaDisassembler dasm)
        {
            var reg = dasm.arch.GetBoolRegister(dasm.state.t);
            dasm.ops.Add(reg);
            return true;
        }


        // Immediate
        private static bool Is(uint wInstr, XtensaDisassembler dasm)
        {
            var op = Constant.Byte(dasm.state.s);
            dasm.ops.Add(op);
            return true;
        }

        private static bool It(uint wInstr, XtensaDisassembler dasm)
        {
            var op = Constant.Byte(dasm.state.t);
            dasm.ops.Add(op);
            return true;
        }

        // Immediate from t field, offset by 7.
        private static bool It_7(uint wInstr, XtensaDisassembler dasm)
        {
            var n = (byte) (dasm.state.t + 7);
            var op = Constant.Byte(n);
            dasm.ops.Add(op);
            return true;
        }

        // Immediate from t field, offset by -8.
        private static bool It_m8(uint wInstr, XtensaDisassembler dasm)
        {
            var n = (sbyte) (dasm.state.t - 8);
            var op = Constant.SByte(n);
            dasm.ops.Add(op);
            return true;
        }

        private static bool Iop2(uint wInstr, XtensaDisassembler dasm)
        {
            var n = dasm.state.op2;
            var op = Constant.Byte(n);
            dasm.ops.Add(op);
            return true;
        }

        private static bool Iop2_4(uint wInstr, XtensaDisassembler dasm)
        {
            var n = dasm.state.op2 << 4;
            var op = Constant.Byte((byte)n);
            dasm.ops.Add(op);
            return true;
        }


        // Floating point constant (in 't' field)

        private static readonly float[] floatImms = new float[16]
        {
            1.0F,
            2.0F,
            4.0F,
            8.0F,

            16.0F,
            32.0F,
            64.0F,
            128.0F,

            256.0F,
            512.0F,
            1024.0F,
            2048.0F,

            4096.0F,
            8192.0F,
            16384.0F,
            32768.0F,
        };

        private static bool Ift(uint wInstr, XtensaDisassembler dasm)
        {
            var op = Constant.Real32(floatImms[dasm.state.t]);
            dasm.ops.Add(op);
            return true;
        }

        // Immediate shift amount used by slli
        private static bool IS(uint wInstr, XtensaDisassembler dasm)
        {
            var op = Constant.Byte((byte) (dasm.state.t | ((dasm.state.op2 & 1) << 4)));
            dasm.ops.Add(op);
            return true;
        }

        // Immediate shift amount used by srai
        private static bool IR(uint wInstr, XtensaDisassembler dasm)
        {
            var op = Constant.Byte((byte) (dasm.state.s | ((dasm.state.op2 & 1) << 4)));
            dasm.ops.Add(op);
            return true;
        }


        // Immediate shift amount used by ssai
        private static bool II(uint wInstr, XtensaDisassembler dasm)
        { 
            var op = Constant.Byte((byte) (dasm.state.s | ((dasm.state.r & 1) << 4)));
            dasm.ops.Add(op);
            return true;
        }


        private static bool I4_2(uint wInstr, XtensaDisassembler dasm)
        {
            var op = Constant.Byte((byte)(dasm.state.r << 2)); 
            dasm.ops.Add(op);
            return true;
        }

        private static bool I8_0(uint wInstr, XtensaDisassembler dasm)
        {
            var op = Constant.UInt16((ushort)dasm.state.imm8);
            dasm.ops.Add(op);
            return true;
        }

        private static bool I8_1(uint wInstr, XtensaDisassembler dasm)
        {
            var op = Constant.UInt16((ushort) (dasm.state.imm8 << 1));
            dasm.ops.Add(op);
            return true;
        }

        private static bool I8_2(uint wInstr, XtensaDisassembler dasm)
        {
            var op = Constant.UInt16((ushort) (dasm.state.imm8 << 2));
            dasm.ops.Add(op);
            return true;
        }

        private static bool I12_3(uint wInstr, XtensaDisassembler dasm)
        {
            var op = Constant.UInt16((ushort) (dasm.state.imm12 << 3));
            dasm.ops.Add(op);
            return true;
        }

        private static bool Irt(uint wInstr, XtensaDisassembler dasm)
        {
            var op = Constant.Byte((byte) (((dasm.state.r & 1) << 4) | dasm.state.t));
            dasm.ops.Add(op);
            return true;
        }

        private static bool a(uint wInstr, XtensaDisassembler dasm)
        {
            var op = Constant.SByte(dasm.state.t == 0 ? (sbyte) -1 : (sbyte) dasm.state.t);
            dasm.ops.Add(op);
            return true;
        }

        // Scaled immediates
        private static bool m0(uint wInstr, XtensaDisassembler dasm)
        {
            int n = (sbyte) dasm.state.imm8;
            var op = Constant.Word32(n);
            dasm.ops.Add(op);
            return true;
        }

        private static bool m8(uint wInstr, XtensaDisassembler dasm)
        {
            int n = (sbyte) dasm.state.imm8;
            n <<= 8;
            var op = Constant.Word32(n);
            dasm.ops.Add(op);
            return true;
        }

        // Split immediate
        private static bool i(uint wInstr, XtensaDisassembler dasm)
        {
            int n = ((dasm.state.imm8 | (dasm.state.s << 8)) << 20) >> 20;
            dasm.ops.Add(Constant.Word32(n));
            return true;
        }

        // 'bs'
        private static bool bs(uint wInstr, XtensaDisassembler dasm)
        {
            var op = Constant.Int32(b4const[dasm.state.r]);
            dasm.ops.Add(op);
            return true;
        }

        private static bool bu(uint wInstr, XtensaDisassembler dasm)
        {
            var op = Constant.Word32(b4constu[dasm.state.r]);
            dasm.ops.Add(op);
            return true;
        }

        // Jump
        private static bool j(uint wInstr, XtensaDisassembler dasm)
        { 
            int dst = (int) dasm.state.addr.ToUInt32() + (sbyte) dasm.state.imm8 + 4;
            dasm.ops.Add(Address.Ptr32((uint) dst));
            return true;
        }

        private static bool ju(uint wInstr, XtensaDisassembler dasm)
        {
            uint dst = dasm.state.addr.ToUInt32() + (byte) dasm.state.imm8 + 4;
            dasm.ops.Add(Address.Ptr32(dst));
            return true;
        }


        private static bool jrt(uint wInstr, XtensaDisassembler dasm)
        {
            var n =
                dasm.state.r |
                ((dasm.state.t & 0x3) << 4);
            int dst = (int) dasm.state.addr.ToUInt32() + (sbyte) n + 4;
            dasm.ops.Add(Address.Ptr32((uint) dst));
            return true;
        }



        private static bool J(uint wInstr, XtensaDisassembler dasm)
        {
            var op = Address.Ptr32((uint) ((int) dasm.state.addr.ToLinear() + (dasm.state.offset + 4)));
            dasm.ops.Add(op);
            return true;
        }

        // Call offset
        private static bool c(uint wInstr, XtensaDisassembler dasm)
        {
            var op = Address.Ptr32((uint) ((dasm.state.addr.ToUInt32() & ~3) + (dasm.state.offset << 2) + 4));
            dasm.ops.Add(op);
            return true;
        }

        // NegativePcRelativeAddress();
        private static bool p(uint wInstr, XtensaDisassembler dasm)
        {
            var nAddr =
                ((0xFFFF0000 | dasm.state.imm16) << 2) +
                 (int) (dasm.rdr.Address.ToUInt32() & ~3);
            var op = Address.Ptr32((uint) nAddr);
            dasm.ops.Add(op);
            return true;
        }

        private MachineOperand GetAluRegister(byte r)
        {
            return arch.GetAluRegister(r);
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


        public class r_Decoder : Decoder
        {
            private readonly Decoder[] decoders;

            public r_Decoder(params Decoder[] decoders)
            {
                Debug.Assert(decoders.Length == 16);
                this.decoders = decoders;
            }

            public override XtensaInstruction Decode(uint wInstr, XtensaDisassembler dasm)
            {
                return decoders[dasm.state.r].Decode(wInstr, dasm);
            }
        }

        public class m_Decoder : Decoder
        {
            private readonly Decoder[] decoders;

            public m_Decoder(params Decoder[] decoders)
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

        public class t_Decoder : Decoder
        {
            private readonly Decoder[] decoders;

            public t_Decoder(params Decoder[] decoders)
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
                var imm = Constant.SByte((sbyte)n);
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
                var dst = Address.Ptr32((uint)
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
                    InstructionClass = InstrClass.Linear,
                    Mnemonic = Mnemonic.extui,
                    Operands = new MachineOperand[]
                    {
                        dasm.GetAluRegister(dasm.state.r),
                        dasm.GetAluRegister(dasm.state.t),
                        Constant.Byte((byte)((dasm.state.s | ((dasm.state.op1 & 1) << 4)))),
                        Constant.Byte((byte)(dasm.state.op2 + 1))
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

            var reserved = Instr(Mnemonic.reserved, InstrClass.Invalid);

            var decoderLSCX = new Op1Decoder(
                Instr(Mnemonic.lsx, Fr,Rs,Rt),
                Instr(Mnemonic.lsxu, Fr,Rs,Rt),
                reserved,
                reserved,

                Instr(Mnemonic.ssx, Fr, Rs, Rt),
                Instr(Mnemonic.ssxu, Fr, Rs, Rt),
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

            var decoderFP1OP = new t_Decoder(
                Instr(Mnemonic.mov_s, Fr, Fs),
                Instr(Mnemonic.abs_s, Fr, Fs),
                reserved,
                reserved,
                
                Instr(Mnemonic.rfr, Rr, Fs),
                Instr(Mnemonic.wfr, Fr, Rs),
                Instr(Mnemonic.neg_s, Fr, Fs),
                reserved,

                reserved, reserved, reserved, reserved,
                reserved, reserved, reserved, reserved
                );

            var decoderFP0 = new Op2Decoder(
                Instr(Mnemonic.add_s, Fr, Fs, Ft),
                Instr(Mnemonic.sub_s, Fr, Fs, Ft),
                Instr(Mnemonic.mul_s, Fr, Fs, Ft),
                reserved,

                Instr(Mnemonic.madd_s, Fr, Fs, Ft),
                Instr(Mnemonic.msub_s, Fr, Fs, Ft),
                reserved,
                reserved,

                Instr(Mnemonic.round_s, Rr, Fs, Ift),
                Instr(Mnemonic.trunc_s, Rr, Fs, Ift),
                Instr(Mnemonic.floor_s, Rr, Fs, Ift),
                Instr(Mnemonic.ceil_s, Rr, Fs, Ift),

                Instr(Mnemonic.float_s, Fr, Rs, Ift),
                Instr(Mnemonic.ufloat_s, Fr, Rs, Ift),
                Instr(Mnemonic.utrunc_s, Rr, Fs, Ift),
                decoderFP1OP);

            var decoderFP1 = new Op2Decoder(
                reserved,
                Instr(Mnemonic.un_s, Br,Fs,Ft),
                Instr(Mnemonic.oeq_s, Br,Fs,Ft),
                Instr(Mnemonic.ueq_s, Br,Fs,Ft),

                Instr(Mnemonic.olt_s, Br,Fs,Ft),
                Instr(Mnemonic.ult_s, Br,Fs,Ft),
                Instr(Mnemonic.ole_s, Br,Fs,Ft),
                Instr(Mnemonic.ule_s, Br,Fs,Ft),

                Instr(Mnemonic.moveqz_s, Fr,Fs,Rt),
                Instr(Mnemonic.movnez_s, Fr,Fs,Rt),
                Instr(Mnemonic.movltz_s, Fr,Fs,Rt),
                Instr(Mnemonic.movgez_s, Fr,Fs,Rt),

                Instr(Mnemonic.movf_s, Fr,Fs,Bt),
                Instr(Mnemonic.movt_s, Fr,Fs,Bt),
                reserved,
                reserved);

            var decoderJR = new n_Rec(
                Instr(Mnemonic.ret, InstrClass.Transfer | InstrClass.Return),
                Instr(Mnemonic.retw, InstrClass.Transfer | InstrClass.Return),
                Instr(Mnemonic.jx, InstrClass.Transfer, Rs),
                reserved);

            var decoderCALLX = new n_Rec(
                Instr(Mnemonic.callx0,  InstrClass.Transfer|InstrClass.Call, Rs),
                Instr(Mnemonic.callx4,  InstrClass.Transfer|InstrClass.Call, Rs),
                Instr(Mnemonic.callx8,  InstrClass.Transfer|InstrClass.Call, Rs),
                Instr(Mnemonic.callx12, InstrClass.Transfer|InstrClass.Call, Rs));

            var decoderSNM0 = new m_Decoder(
                Instr(Mnemonic.ill, InstrClass.Invalid|InstrClass.Zero),
                reserved,
                decoderJR,
                decoderCALLX);

            var decoderSYNC = new t_Decoder(
                Instr(Mnemonic.isync),
                Instr(Mnemonic.rsync),
                Instr(Mnemonic.esync),
                Instr(Mnemonic.dsync),

                reserved,
                reserved,
                reserved,
                reserved,

                Instr(Mnemonic.excw),
                reserved,
                reserved,
                reserved,

                Instr(Mnemonic.memw),
                Instr(Mnemonic.extw),
                reserved,
                Instr(Mnemonic.nop, InstrClass.Linear|InstrClass.Padding));

            var decoderRFET = new s_Rec(
                Instr(Mnemonic.rfe, InstrClass.Transfer|InstrClass.Return),
                Instr(Mnemonic.rfue),
                Instr(Mnemonic.rfde),
                reserved,

                Instr(Mnemonic.rfwo),
                Instr(Mnemonic.rfwu),
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

            var decoderRFEI = new t_Decoder(
                decoderRFET,
                Instr(Mnemonic.rfi, InstrClass.Transfer|InstrClass.Return, Is),
                Instr(Mnemonic.rfme),
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


            var decoderBZ = new m_Decoder(
                new bz_Decoder(Mnemonic.beqz),
                new bz_Decoder(Mnemonic.bnez),
                new bz_Decoder(Mnemonic.bltz),
                new bz_Decoder(Mnemonic.bgez));

            var decoderBI0 = new m_Decoder(
                Instr(Mnemonic.beqi, InstrClass.ConditionalTransfer, Rs,bs,j),
                Instr(Mnemonic.bnei, InstrClass.ConditionalTransfer, Rs,bs,j),
                Instr(Mnemonic.blti, InstrClass.ConditionalTransfer, Rs,bs,j),
                Instr(Mnemonic.bgei, InstrClass.ConditionalTransfer, Rs,bs,j));

            var decoderB1 = new r_Decoder(
                Instr(Mnemonic.bf, Bs, j),
                Instr(Mnemonic.bt, Bs, j),
                reserved,
                reserved,

                reserved,
                reserved,
                reserved,
                reserved,

                Instr(Mnemonic.loop, Rs,ju),
                Instr(Mnemonic.loopnez, Rs,ju),
                Instr(Mnemonic.loopgtz, Rs,ju),
                reserved,

                reserved,
                reserved,
                reserved,
                reserved);

            var decoderBI1 = new m_Decoder(
                Instr(Mnemonic.entry, Rs, I12_3),
                decoderB1,
                Instr(Mnemonic.bltui, InstrClass.ConditionalTransfer, Rs,bu,j),
                Instr(Mnemonic.bgeui, InstrClass.ConditionalTransfer, Rs,bu,j));

            var decoderSI = new n_Rec(
                Instr(Mnemonic.j, J),
                decoderBZ,
                decoderBI0,
                decoderBI1);

            var decoderB = new r_Decoder(
               Instr(Mnemonic.bnone, InstrClass.ConditionalTransfer, Rs,Rt,j),
               Instr(Mnemonic.beq, InstrClass.ConditionalTransfer, Rs,Rt,j),
               Instr(Mnemonic.blt, InstrClass.ConditionalTransfer, Rs,Rt,j),
               Instr(Mnemonic.bltu, InstrClass.ConditionalTransfer, Rs,Rt,j),

               Instr(Mnemonic.ball, InstrClass.ConditionalTransfer, Rs, Rt,j),
               Instr(Mnemonic.bbc,  InstrClass.ConditionalTransfer, Rs, Rt,j),
               Instr(Mnemonic.bbci, InstrClass.ConditionalTransfer, Rs, Irt, j),
               Instr(Mnemonic.bbci, InstrClass.ConditionalTransfer, Rs, Irt, j),

               Instr(Mnemonic.bany, InstrClass.ConditionalTransfer, Rs,Rt,j),
               Instr(Mnemonic.bne, InstrClass.ConditionalTransfer, Rs,Rt,j),
               Instr(Mnemonic.bge, InstrClass.ConditionalTransfer, Rs,Rt,j),
               Instr(Mnemonic.bgeu, InstrClass.ConditionalTransfer, Rs,Rt,j),

               Instr(Mnemonic.bnall, InstrClass.ConditionalTransfer, Rs,Rt,j),
               Instr(Mnemonic.bbs,  InstrClass.ConditionalTransfer, Rs,Rt,j),
               Instr(Mnemonic.bbsi, InstrClass.ConditionalTransfer, Rs, Irt, j),
               Instr(Mnemonic.bbsi, InstrClass.ConditionalTransfer, Rs, Irt, j));

            var decoderST0 = new r_Decoder(
                decoderSNM0,
                Instr(Mnemonic.movsp, Rt, Rs),
                decoderSYNC,
                decoderRFEI,

                Instr(Mnemonic.@break, Is,It),
                Instr(Mnemonic.syscall, InstrClass.Transfer|InstrClass.Call),
                Instr(Mnemonic.rsil, Rt,Is),
                Instr(Mnemonic.waiti, Is),

                Instr(Mnemonic.any4, Bt, Bs_MultipleOf(4)),
                Instr(Mnemonic.all4, Bt, Bs_MultipleOf(4)),
                Instr(Mnemonic.any8, Bt, Bs_MultipleOf(8)),
                Instr(Mnemonic.all8, Bt, Bs_MultipleOf(8)),

                reserved,
                reserved,
                reserved,
                reserved);

            var decoderST1 = new r_Decoder(
                Instr(Mnemonic.ssr, Rs),
                Instr(Mnemonic.ssl, Rs),
                Instr(Mnemonic.ssa8l, Rs),
                Instr(Mnemonic.ssa8b, Rs),

                Instr(Mnemonic.ssai, II),
                reserved,
                Instr(Mnemonic.rer, Rt,Rs),
                Instr(Mnemonic.wer, Rt,Rs),

                Instr(Mnemonic.rotw, It_m8),
                reserved,
                reserved,
                reserved,

                reserved,
                reserved,
                Instr(Mnemonic.nsa, Rt,Rs),
                Instr(Mnemonic.nsau, Rt,Rs));

            var decoderST2 = new t_Decoder(
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

            var decoderS3 = new t_Decoder(
                Instr2byte(Mnemonic.ret_n, InstrClass.Transfer | InstrClass.Return),
                Instr2byte(Mnemonic.retw_n, InstrClass.Transfer | InstrClass.Return),
                Instr2byte(Mnemonic.break_n, InstrClass.Transfer|InstrClass.Call, Is),
                Instr2byte(Mnemonic.nop_n, InstrClass.Linear|InstrClass.Padding),

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

            var decoderST3 = new r_Decoder(
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

            var decoderTLB = new r_Decoder(
                reserved,
                reserved,
                reserved,
                Instr(Mnemonic.ritlb0, Rt,Rs),

                Instr(Mnemonic.iitlb, Rs),
                Instr(Mnemonic.pitlb, Rt,Rs),
                Instr(Mnemonic.witlb, Rt,Rs),
                Instr(Mnemonic.ritlb1, Rt,Rs),

                reserved,
                reserved,
                reserved,
                Instr(Mnemonic.rdtlb0, Rt,Rs),

                Instr(Mnemonic.idtlb, Rs),
                Instr(Mnemonic.pdtlb, Rt,Rs),
                Instr(Mnemonic.wdtlb, Rt,Rs),
                Instr(Mnemonic.rdtlb1, Rt,Rs));

            var decoderRST0 = new Op2Decoder(
                decoderST0,
                Instr(Mnemonic.and, Rr,Rs,Rt),
                Instr(Mnemonic.or, Rr,Rs,Rt),
                Instr(Mnemonic.xor, Rr,Rs,Rt),

                decoderST1,
                decoderTLB,
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

            var decoderIMP = new r_Decoder(
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

            var accer = new Op2Decoder(
                Nyi("RER"),
                reserved,
                reserved,
                reserved,

                reserved,
                reserved,
                reserved,
                reserved,

                Nyi("RER"),
                reserved,
                reserved,
                reserved,

                reserved,
                reserved,
                reserved,
                reserved);

            var decoderRST1 = new Op2Decoder(
                Instr(Mnemonic.slli, Rr,Rs,IS),
                Instr(Mnemonic.slli, Rr,Rs,IS),
                Instr(Mnemonic.srai, Rr,Rt,IR),
                Instr(Mnemonic.srai, Rr,Rt,IR),

                Instr(Mnemonic.srli, Rr,Rt,Is),
                reserved,
                Instr(Mnemonic.xsr,Rt,S),
                accer,

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
                Instr(Mnemonic.muluh, Rr,Rs,Rt),
                Instr(Mnemonic.mulsh, Rr,Rs,Rt),

                Instr(Mnemonic.quou, Rr,Rs,Rt),
                Instr(Mnemonic.quos, Rr,Rs,Rt),
                Instr(Mnemonic.remu, Rr,Rs,Rt),
                Instr(Mnemonic.rems, Rr,Rs,Rt));

            var decoderRST3 = new Op2Decoder(
               Instr(Mnemonic.rsr, Rt,S),
               Instr(Mnemonic.wsr, Rt,S),
               Instr(Mnemonic.sext, Rr,Rs,It_7),
               Instr(Mnemonic.clamps, Rr,Rs,It_7),

               Instr(Mnemonic.min, Rr,Rs,Rt),
               Instr(Mnemonic.max, Rr,Rs,Rt),
               Instr(Mnemonic.minu, Rr,Rs,Rt),
               Instr(Mnemonic.maxu, Rr,Rs,Rt),

               Instr(Mnemonic.moveqz, Rr,Rs,Rt),
               Instr(Mnemonic.movnez, Rr,Rs,Rt),
               Instr(Mnemonic.movltz, Rr,Rs,Rt),
               Instr(Mnemonic.movgez, Rr,Rs,Rt),

               Instr(Mnemonic.movf, Rr,Rs,Bt),
               Instr(Mnemonic.movt, Rr,Rs,Bt),
               Instr(Mnemonic.rur, Rr,Ust),
               Instr(Mnemonic.wur, Rt,Usr));

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

            var decoderDCE = new Op1Decoder(
                Instr(Mnemonic.dpfl, Rs, Iop2_4),
                reserved,
                Instr(Mnemonic.dhu, Rs, Iop2),
                Instr(Mnemonic.diu, Rs, Iop2),

                Instr(Mnemonic.diwb, Rs, Iop2_4),
                Instr(Mnemonic.diwbi, Rs, Iop2_4),
                reserved,
                reserved,

                reserved, reserved, reserved, reserved,
                reserved, reserved, reserved, reserved
                );

            var decoderICE = new Op1Decoder(
                Instr(Mnemonic.ipfl, Rs,Iop2_4),
                reserved, 
                Instr(Mnemonic.ihu, Rs,Iop2_4),
                Instr(Mnemonic.iiu, Rs,Iop2_4),
                
                reserved, reserved, reserved, reserved,
                reserved, reserved, reserved, reserved,
                reserved, reserved, reserved, reserved
            );

            var decoderCACHE = new t_Decoder(
                Instr(Mnemonic.dpfr, Rs,I8_2),
                Instr(Mnemonic.dpfw, Rs,I8_2),
                Instr(Mnemonic.dpfro, Rs,I8_2),
                Instr(Mnemonic.dpfwo, Rs,I8_2),

                Instr(Mnemonic.dhwb, Rs,I8_2),
                Instr(Mnemonic.dhwbi, Rs,I8_2),
                Instr(Mnemonic.dhi, Rs,I8_2),
                Instr(Mnemonic.dii, Rs,I8_2),

                decoderDCE,
                reserved,
                reserved,
                reserved,
                
                Instr(Mnemonic.ipf, Rs,I8_2),
                decoderICE,
                Instr(Mnemonic.ihi, Rs, I8_2),
                Instr(Mnemonic.iii, Rs, I8_2));

            var decoderLSAI = new r_Decoder(
                Instr(Mnemonic.l8ui, Rt,Rs,I8_0),
                Instr(Mnemonic.l16ui, Rt,Rs,I8_1),
                Instr(Mnemonic.l32i, Rt,Rs,I8_2),
                reserved,

                Instr(Mnemonic.s8i, Rt,Rs,I8_0),
                Instr(Mnemonic.s16i, Rt,Rs,I8_1),
                Instr(Mnemonic.s32i, Rt,Rs,I8_2),
                decoderCACHE,

                reserved,
                Instr(Mnemonic.l16si, Rt,Rs,I8_1),
                Instr(Mnemonic.movi, Rt,i),
                Instr(Mnemonic.l32ai, Rt,Rs,I8_2),

                Instr(Mnemonic.addi, Rt,Rs,m0),
                Instr(Mnemonic.addmi, Rt,Rs,m8),
                Instr(Mnemonic.s32c1i, Rt,Rs,I8_2),
                Instr(Mnemonic.s32ri, Rt,Rs,I8_2));

            var decoderLSCI = new r_Decoder(
                Instr(Mnemonic.lsi, Ft,Rs,I8_2),
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

                Instr(Mnemonic.mula_da_ll_ldinc, Mr, Rs, Mr_01, Rt),
                Instr(Mnemonic.mula_da_hl_ldinc, Mr, Rs, Mr_01, Rt),
                Instr(Mnemonic.mula_da_lh_ldinc, Mr, Rs, Mr_01, Rt),
                Instr(Mnemonic.mula_da_hh_ldinc, Mr, Rs, Mr_01, Rt),

                reserved,
                reserved,
                reserved,
                reserved);

            var decoderMACAD = new Op1Decoder(
                reserved, reserved, reserved, reserved,
                Instr(Mnemonic.mul_ad_ll, Rs, Mt_23),
                Instr(Mnemonic.mul_ad_hl, Rs, Mt_23),
                Instr(Mnemonic.mul_ad_lh, Rs, Mt_23),
                Instr(Mnemonic.mul_ad_hh, Rs, Mt_23),

                Instr(Mnemonic.mula_ad_ll, Rs, Mt_23),
                Instr(Mnemonic.mula_ad_hl, Rs, Mt_23),
                Instr(Mnemonic.mula_ad_lh, Rs, Mt_23),
                Instr(Mnemonic.mula_ad_hh, Rs, Mt_23),

                Instr(Mnemonic.muls_ad_ll, Rs, Mt_23),
                Instr(Mnemonic.muls_ad_hl, Rs, Mt_23),
                Instr(Mnemonic.muls_ad_lh, Rs, Mt_23),
                Instr(Mnemonic.muls_ad_hh, Rs, Mt_23));

            var decoderMACCD = new Op1Decoder(
                reserved, reserved, reserved, reserved,
                reserved, reserved, reserved, reserved,

                Instr(Mnemonic.mula_dd_ll_lddec, Mr, Rs, Mr_01, Rt),
                Instr(Mnemonic.mula_dd_hl_lddec, Mr, Rs, Mr_01, Rt),
                Instr(Mnemonic.mula_dd_lh_lddec, Mr, Rs, Mr_01, Rt),
                Instr(Mnemonic.mula_dd_hh_lddec, Mr, Rs, Mr_01, Rt),

                reserved, reserved, reserved, reserved);

            var decoderMACDD = new Op1Decoder(
                reserved,
                reserved,
                reserved,
                reserved,

                Instr(Mnemonic.mul_dd_ll, Mr_01, Mt_23),
                Instr(Mnemonic.mul_dd_hl, Mr_01, Mt_23),
                Instr(Mnemonic.mul_dd_lh, Mr_01, Mt_23),
                Instr(Mnemonic.mul_dd_hh, Mr_01, Mt_23),

                Instr(Mnemonic.mula_dd_ll, Mr_01, Mt_23),
                Instr(Mnemonic.mula_dd_hl, Mr_01, Mt_23),
                Instr(Mnemonic.mula_dd_lh, Mr_01, Mt_23),
                Instr(Mnemonic.mula_dd_hh, Mr_01, Mt_23),

                Instr(Mnemonic.muls_dd_ll, Mr_01, Mt_23),
                Instr(Mnemonic.muls_dd_hl, Mr_01, Mt_23),
                Instr(Mnemonic.muls_dd_lh, Mr_01, Mt_23),
                Instr(Mnemonic.muls_dd_hh, Mr_01, Mt_23));

            var decoderMACI = new Op1Decoder(
                Instr(Mnemonic.ldinc, Mr,Rs), reserved, reserved, reserved,
                reserved, reserved, reserved, reserved,
                reserved, reserved, reserved, reserved,
                reserved, reserved, reserved, reserved);

            var decoderMACC = new Op1Decoder(
                Instr(Mnemonic.lddec, Mr,Rs),
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

            var decoderMACIA = new Op1Decoder(
                reserved, reserved, reserved, reserved,
                reserved, reserved, reserved, reserved,
                Instr(Mnemonic.mula_da_ll_ldinc, Mr, Rs, Mr_01, Rt),
                Instr(Mnemonic.mula_da_hl_ldinc, Mr, Rs, Mr_01, Rt),
                Instr(Mnemonic.mula_da_lh_ldinc, Mr, Rs, Mr_01, Rt),
                Instr(Mnemonic.mula_da_hh_ldinc, Mr, Rs, Mr_01, Rt),
                reserved, reserved, reserved, reserved);

            var decoderMACCA = new Op1Decoder(
                reserved, reserved, reserved, reserved,
                reserved, reserved, reserved, reserved,
                Instr(Mnemonic.mula_da_ll_lddec, Mr, Rs, Mr_01, Rt),
                Instr(Mnemonic.mula_da_hl_lddec, Mr, Rs, Mr_01, Rt),
                Instr(Mnemonic.mula_da_lh_lddec, Mr, Rs, Mr_01, Rt),
                Instr(Mnemonic.mula_da_hh_lddec, Mr, Rs, Mr_01, Rt),
                reserved, reserved, reserved, reserved);

            var decoderMACDA = new Op1Decoder(
                reserved, reserved, reserved, reserved,
                Instr(Mnemonic.mul_da_ll, Mr_01, Rt),
                Instr(Mnemonic.mul_da_hl, Mr_01, Rt),
                Instr(Mnemonic.mul_da_lh, Mr_01, Rt),
                Instr(Mnemonic.mul_da_hh, Mr_01, Rt),

                Instr(Mnemonic.mula_da_ll, Mr_01, Rt),
                Instr(Mnemonic.mula_da_hl, Mr_01, Rt),
                Instr(Mnemonic.mula_da_lh, Mr_01, Rt),
                Instr(Mnemonic.mula_da_hh, Mr_01, Rt),

                Instr(Mnemonic.muls_da_ll, Mr_01, Rt),
                Instr(Mnemonic.muls_da_hl, Mr_01, Rt),
                Instr(Mnemonic.muls_da_lh, Mr_01, Rt),
                Instr(Mnemonic.muls_da_hh, Mr_01, Rt));

            var decoderMACAA = new Op1Decoder(
                Instr(Mnemonic.umul_aa_ll, Rs, Rt),
                Instr(Mnemonic.umul_aa_hl, Rs, Rt),
                Instr(Mnemonic.umul_aa_lh, Rs, Rt),
                Instr(Mnemonic.umul_aa_hh, Rs, Rt),
                Instr(Mnemonic.mul_aa_ll, Rs,Rt),
                Instr(Mnemonic.mul_aa_hl, Rs,Rt),
                Instr(Mnemonic.mul_aa_lh, Rs,Rt),
                Instr(Mnemonic.mul_aa_hh, Rs,Rt),
                Instr(Mnemonic.mula_aa_ll, Rs,Rt),
                Instr(Mnemonic.mula_aa_hl, Rs,Rt),
                Instr(Mnemonic.mula_aa_lh, Rs,Rt),
                Instr(Mnemonic.mula_aa_hh, Rs,Rt),
                Instr(Mnemonic.muls_aa_ll, Rs,Rt),
                Instr(Mnemonic.muls_aa_hl, Rs,Rt),
                Instr(Mnemonic.muls_aa_lh, Rs,Rt),
                Instr(Mnemonic.muls_aa_hh, Rs,Rt));

            var decoderMAC16 = new Op2Decoder(
                decoderMACID,
                decoderMACCD,
                decoderMACDD,
                decoderMACAD,

                decoderMACIA,
                decoderMACCA,
                decoderMACDA,
                decoderMACAA,

                decoderMACI,
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
