#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.Core.Types;
using System.Collections.Generic;
using System.Diagnostics;

namespace Reko.Arch.Renesas.Rx
{
    using Decoder = Decoder<RxDisassembler, Mnemonic, RxInstruction>;

    public class RxDisassembler : DisassemblerBase<RxInstruction, Mnemonic>
    {
        private static readonly Decoder rootDecoder;
        private static readonly PrimitiveType dt3bits = PrimitiveType.CreateWord(3);
        private static readonly Bitfield bf0_4 = new Bitfield(0, 4);
        private static readonly Bitfield bf4_3 = new Bitfield(4, 3);
        private static readonly Bitfield bf4_4 = new Bitfield(4, 4);
        private static readonly Bitfield[] bf7_1_0_4 = Bf((7, 1), (0, 4));
        private static readonly Bitfield[] bf7_4_3_1 = Bf((7, 4), (3, 1));
        private static readonly Bitfield bf8_2 = new Bitfield(8, 2);
        private static readonly Bitfield bf8_4 = new Bitfield(8, 4);
        private static readonly Bitfield bf12_2 = new Bitfield(12, 2);
        private static readonly Bitfield bf14_2 = new Bitfield(14, 2);
        private static readonly Bitfield bf22_2 = new Bitfield(22, 2);
        private static readonly int[] disp3codes =
        [
            8, 9, 10, 3, 4, 5, 6, 7
        ];
        private static readonly PrimitiveType?[] dtBWL_ =
        [
            PrimitiveType.SByte, PrimitiveType.Int16, PrimitiveType.Word32, null
        ];
        private static readonly FlagGroupStorage?[] pswbits =
        [
            Registers.C,
            Registers.Z,
            Registers.S,
            Registers.O,
            null,
            null,
            null,
            null,
            Registers.I,
            Registers.U,
            null,
            null,
            null,
            null,
            null,
            null,
        ];
        private static readonly RegisterStorage?[] cRegisters = [
            Registers.PSW,
            Registers.PC,
            Registers.USP,
            Registers.FPSW,
            null,
            null,
            null,
            null,
            Registers.BPSW,
            Registers.BPC,
            Registers.ISP,
            Registers.FINTV,
            Registers.INTB,
            Registers.EXTB,
            null,
            null
        ]; 

        private readonly RxArchitecture arch;
        private readonly EndianImageReader rdr;
        private readonly List<MachineOperand> ops;
        private Address addr;
        private PrimitiveType? dataWidth;

        public RxDisassembler(RxArchitecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.ops = [];
            this.dataWidth = default;
        }

        public override RxInstruction? DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadByte(out byte uInstr))
                return null;
            this.ops.Clear();
            this.dataWidth = null;
            var instr = rootDecoder.Decode(uInstr, this);
            this.ops.Clear();
            if (uInstr == 0)
                instr.InstructionClass |= InstrClass.Zero;
            instr.Address = this.addr;
            instr.Length = (int) (rdr.Address - addr);
            return instr;
        }

        public override RxInstruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            return new RxInstruction
            {
                InstructionClass = iclass,
                Mnemonic = mnemonic,
                DataType = this.dataWidth,
                Operands = ops.ToArray()
            };
        }

        public override RxInstruction CreateInvalidInstruction()
        {
            return new RxInstruction { InstructionClass = InstrClass.Invalid, Mnemonic = Mnemonic.Invalid };
        }

        public override RxInstruction NotYetImplemented(string message)
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingDecoder("Rl78Dis", this.addr, this.rdr, message);
            return new RxInstruction { InstructionClass = InstrClass.Invalid, Mnemonic = Mnemonic.nyi };
        }

        #region Mutators

        private static Mutator<RxDisassembler> GpRegister(int bitoffset, int bitlength)
        {
            var field = new Bitfield(bitoffset, bitlength);
            return (u, d) =>
            {
                var ireg = field.Read(u);
                d.ops.Add(Registers.GpRegisters[ireg]);
                return true;
            };
        }
        private static readonly Mutator<RxDisassembler> R0 = GpRegister(0, 4);
        private static readonly Mutator<RxDisassembler> R4 = GpRegister(4, 4);
        private static readonly Mutator<RxDisassembler> R8 = GpRegister(8, 4);
        private static readonly Mutator<RxDisassembler> R0_3 = GpRegister(0, 3);
        private static readonly Mutator<RxDisassembler> R4_3 = GpRegister(4, 3);

        private static bool A0(uint uInstr, RxDisassembler dasm)
        {
            dasm.ops.Add(Registers.ACC0);
            return true;
        }

        private static bool A1(uint uInstr, RxDisassembler dasm)
        {
            dasm.ops.Add(Registers.ACC1);
            return true;
        }

        private static bool RegRange(uint uInstr, RxDisassembler dasm)
        {
            int iMin = (int) bf4_4.Read(uInstr);
            int iMax = (int) bf0_4.Read(uInstr) + 1;
            dasm.ops.Add(new RegisterRange(Registers.GpRegisters, iMin, iMax - iMin));
            return true;
        }

        private static Mutator<RxDisassembler> ConditionRegister(int bitoffset)
        {
            var field = new Bitfield(bitoffset, 4);
            return (u, d) =>
            {
                var ireg = field.Read(u);
                var reg = cRegisters[ireg];
                if (reg is null)
                    return false;
                d.ops.Add(reg);
                return true;
            };
        }
        private static readonly Mutator<RxDisassembler> CR0 = ConditionRegister(0);
        private static readonly Mutator<RxDisassembler> CR4 = ConditionRegister(4);

        /// <summary>
        /// Unsigned immediate.
        /// </summary>
        private static Mutator<RxDisassembler> UImm(int bitpos, int bitlength, PrimitiveType dt)
        {
            var field = new Bitfield(bitpos, bitlength);
            return (u, d) =>
            {
                var op = Constant.Create(dt, field.Read(u));
                d.ops.Add(op);
                return true;
            };
        }
        private static readonly Mutator<RxDisassembler> UI_0_3 = UImm(0, 3, PrimitiveType.Byte);
        private static readonly Mutator<RxDisassembler> UI_0_4 = UImm(0, 4, PrimitiveType.Byte);
        private static readonly Mutator<RxDisassembler> UI4_4_32 = UImm(4, 4, PrimitiveType.Word32);
        private static readonly Mutator<RxDisassembler> UI4_5 = UImm(4, 5, PrimitiveType.Word32);
        private static readonly Mutator<RxDisassembler> UI8_5 = UImm(8, 5, PrimitiveType.Word32);
        private static readonly Mutator<RxDisassembler> UI_10_3 = UImm(10, 3, PrimitiveType.Byte);


        private static Mutator<RxDisassembler> Simm(int bitpos)
        {
            var field = new Bitfield(bitpos, 2);
            return (u, d) =>
            {
                var sizeCode = field.Read(u);
                int imm;
                switch (sizeCode)
                {
                case 0b01:
                    if (!d.rdr.TryReadByte(out byte b8))
                        return false;
                    imm = (sbyte) b8;
                    break;
                case 0b10:
                    if (!d.rdr.TryReadInt16(out short w16))
                        return false;
                    imm = w16;
                    break;
                case 0b11:
                    if (!d.TryReadInt24(out imm))
                        return false;
                    break;
                default:
                    if (!d.rdr.TryReadInt32(out int w32))
                        return false;
                    imm = w32;
                    break;
                }
                var op = Constant.Create(PrimitiveType.Word32, imm);
                d.ops.Add(op);
                return true;
            };
        }
        private static readonly Mutator<RxDisassembler> Simm2 = Simm(2);
        private static readonly Mutator<RxDisassembler> Simm8 = Simm(8);
        private static readonly Mutator<RxDisassembler> Simm10 = Simm(10);

        private static Mutator<RxDisassembler> Uimm(int bitpos)
        {
            var field = new Bitfield(bitpos, 2);
            return (u, d) =>
            {
                var sizeCode = field.Read(u);
                uint imm;
                switch (sizeCode)
                {
                case 0b01:
                    if (!d.rdr.TryReadByte(out byte b8))
                        return false;
                    imm = b8;
                    break;
                case 0b10:
                    if (!d.rdr.TryReadUInt16(out ushort w16))
                        return false;
                    imm = w16;
                    break;
                case 0b11:
                    if (!d.TryReadUInt24(out imm))
                        return false;
                    break;
                default:
                    if (!d.rdr.TryReadUInt32(out uint w32))
                        return false;
                    imm = w32;
                    break;
                }
                var op = Constant.Create(PrimitiveType.Word32, imm);
                d.ops.Add(op);
                return true;
            };
        }
        private static readonly Mutator<RxDisassembler> Uimm_8 = Uimm(8);


        /// <summary>
        /// Always a 8-bit uimm.
        /// </summary>
        /// <param name="uInstrm"></param>
        /// <param name="dasm"></param>
        /// <returns></returns>
        public static bool Uimm8(uint uInstrm, RxDisassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out byte b8))
                return false;
            dasm.ops.Add(Constant.Byte(b8));
            return true;
        }

        private static Mutator<RxDisassembler> C(uint n)
        {
            var c = Constant.Create(PrimitiveType.Word32, n);
            return (u, d) =>
            {
                d.ops.Add(c);
                return true;
            };
        }
        private static readonly Mutator<RxDisassembler> C0 = C(0);
        private static readonly Mutator<RxDisassembler> C1 = C(1);
        private static readonly Mutator<RxDisassembler> C2 = C(2);

        private static bool Fimm(uint uInstr, RxDisassembler dasm)
        {
            if (!dasm.rdr.TryReadUInt32(out uint ieeeFloat))
                return false;
            var c = Constant.FloatFromBitpattern(ieeeFloat);
            dasm.ops.Add(c);
            return true;
        }

        private static bool pswbit(uint uInstr, RxDisassembler dasm)
        {
            var index = bf0_4.Read(uInstr);
            var flag = pswbits[index];
            if (flag is null)
                return false;
            dasm.ops.Add(flag);
            return true;
        }

        private bool TryReadInt24(out int imm)
        {
                imm = 0;
            if (arch.Endianness == EndianServices.Little)
            {
                if (!rdr.TryReadByte(out byte b0))
                    return false;
                if (!rdr.TryReadByte(out byte b1))
                    return false;
                if (!rdr.TryReadByte(out byte b2))
                    return false;
                imm = (sbyte)b2 << 16 | b1 << 8 | b0;
            }
            else
            {
                if (!rdr.TryReadByte(out byte b0))
                    return false;
                if (!rdr.TryReadByte(out byte b1))
                    return false;
                if (!rdr.TryReadByte(out byte b2))
                    return false;
                imm = (sbyte) b0 << 16 | b1 << 8 | b2;
            }
            return true;
        }

        private bool TryReadUInt24(out uint imm)
        {
            imm = 0;
            if (arch.Endianness == EndianServices.Little)
            {
                if (!rdr.TryReadByte(out byte b0))
                    return false;
                if (!rdr.TryReadByte(out byte b1))
                    return false;
                if (!rdr.TryReadByte(out byte b2))
                    return false;
                imm = (uint) b2 << 16 | (uint)b1 << 8 | b0;
            }
            else
            {
                if (!rdr.TryReadByte(out byte b0))
                    return false;
                if (!rdr.TryReadByte(out byte b1))
                    return false;
                if (!rdr.TryReadByte(out byte b2))
                    return false;
                imm = (uint) b0 << 16 | (uint) b1 << 8 | b2;
            }
            return true;
        }


        private static bool Mr4(uint uInstr, RxDisassembler dasm)
        {
            var iReg = bf4_4.Read(uInstr);
            var reg = Registers.GpRegisters[iReg];
            Debug.Assert(dasm.dataWidth is not null);
            var mem = new MemoryOperand(dasm.dataWidth, reg, 0);
            dasm.ops.Add(mem);
            return true;
        }

        private static bool Mr4_d8(uint uInstr, RxDisassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out byte b))
                return false;
            var iReg = bf4_4.Read(uInstr);
            var reg = Registers.GpRegisters[iReg];
            Debug.Assert(dasm.dataWidth is not null);
            var mem = new MemoryOperand(dasm.dataWidth, reg, dasm.dataWidth.Size * (sbyte)b);
            dasm.ops.Add(mem);
            return true;
        }

        private static bool Mr4_d16(uint uInstr, RxDisassembler dasm)
        {
            if (!dasm.rdr.TryReadBeInt16(out short s))
                return false;
            var iReg = bf4_4.Read(uInstr);
            var reg = Registers.GpRegisters[iReg];
            Debug.Assert(dasm.dataWidth is not null);
            var mem = new MemoryOperand(dasm.dataWidth, reg, s);
            dasm.ops.Add(mem);
            return true;
        }

        private static Mutator<RxDisassembler> Mr(int regPos, int szPos)
        {
            var regField = new Bitfield(regPos, 4);
            var szField = new Bitfield(szPos, 2);
            return (u, d) =>
            {
                var iReg = regField.Read(u);
                var reg = Registers.GpRegisters[iReg];
                var dsp = szField.Read(u);
                Decoder.DumpMaskedInstruction(32, u, szField.Mask << szField.Position, "mr4");
                int displacement;
                switch (dsp)
                {
                case 0: displacement = 0; break;
                case 1:
                    if (!d.rdr.TryReadByte(out var b))
                        return false;
                    displacement = (sbyte) b;
                    break;
                case 2:
                    if (!d.rdr.TryReadInt16(out var s))
                        return false;
                    displacement = s;
                    break;
                default:
                    d.ops.Add(reg);
                    d.dataWidth = null;
                    return true;
                }
                Debug.Assert(d.dataWidth is not null);
                var mem = new MemoryOperand(d.dataWidth, reg, displacement);
                d.ops.Add(mem);
                return true;
            };
        }
        private static readonly Mutator<RxDisassembler> Mr0_10 = Mr(0, 10);
        private static readonly Mutator<RxDisassembler> Mr4_8 = Mr(4, 8);
        private static readonly Mutator<RxDisassembler> Mr4_10 = Mr(4, 10);
        private static readonly Mutator<RxDisassembler> Mr4_16 = Mr(4, 16);

        private static bool Midx(uint uInstr, RxDisassembler dasm)
        {
            var rBase = Registers.GpRegisters[bf4_4.Read(uInstr)];
            var rIndex = Registers.GpRegisters[bf8_4.Read(uInstr)];
            Debug.Assert(dasm.dataWidth is not null);
            var mem = new MemoryOperand(dasm.dataWidth, rBase, rIndex);
            dasm.ops.Add(mem);
            return true;
        }

        private static bool Mpostinc(uint uInstr, RxDisassembler dasm)
        {
            var reg = Registers.GpRegisters[bf4_4.Read(uInstr)];
            Debug.Assert(dasm.dataWidth is not null);
            var mem = MemoryOperand.PostIncrement(dasm.dataWidth, reg);
            dasm.ops.Add(mem);
            return true;
        }

        private static bool Mpredec(uint uInstr, RxDisassembler dasm)
        {
            var reg = Registers.GpRegisters[bf4_4.Read(uInstr)];
            Debug.Assert(dasm.dataWidth is not null);
            var mem = MemoryOperand.PreDecrement(dasm.dataWidth, reg);
            dasm.ops.Add(mem);
            return true;
        }


        private static bool Mdsp5_0(uint uInstr, RxDisassembler dasm)
        {
            var ireg = bf4_3.Read(uInstr);
            var disp = (int) Bitfield.ReadFields(bf7_1_0_4, uInstr);
            var dt = dtBWL_[bf8_2.Read(uInstr)];
            if (dt is null)
                return false;
            var mem = new MemoryOperand(dt, Registers.GpRegisters[ireg], disp);
            dasm.ops.Add(mem);
            return true;
        }

        private static bool Mdsp5_3(uint uInstr, RxDisassembler dasm)
        {
            var ireg = bf4_3.Read(uInstr);
            var disp = (int) Bitfield.ReadFields(bf7_4_3_1, uInstr);
            Debug.Assert(dasm.dataWidth != null);
            var mem = new MemoryOperand(dasm.dataWidth, Registers.GpRegisters[ireg], disp);
            dasm.ops.Add(mem);
            return true;
        }

        private static bool B(uint uInstr, RxDisassembler dasm)
        {
            dasm.dataWidth = PrimitiveType.Int8;
            return true;
        }

        private static bool L(uint uInstr, RxDisassembler dasm)
        {
            dasm.dataWidth = PrimitiveType.Word32;
            return true;
        }

        private static bool S(uint uInstr, RxDisassembler dasm)
        {
            dasm.dataWidth = dt3bits;
            return true;
        }

        private static bool UB(uint uInstr, RxDisassembler dasm)
        {
            dasm.dataWidth = PrimitiveType.Byte;
            return true;
        }
        private static bool W(uint uInstr, RxDisassembler dasm)
        {
            dasm.dataWidth = PrimitiveType.Int16;
            return true;
        }

        private static Mutator<RxDisassembler> bwluw(Bitfield field)
        {
            return (uint uInstr, RxDisassembler dasm) =>
            {
                var b = field.Read(uInstr);
                dasm.dataWidth = b switch
                {
                    0 => PrimitiveType.Int8,
                    1 => PrimitiveType.Int16,
                    2 => PrimitiveType.Word32,
                    _ => PrimitiveType.Word16,
                };
                return true;
            };
        }
        private static Mutator<RxDisassembler> bwluw14 = bwluw(bf14_2);
        private static Mutator<RxDisassembler> bwluw22 = bwluw(bf22_2);

        private static Mutator<RxDisassembler> bwl_(int bitpos)
        {
            var field = new Bitfield(bitpos, 2);
            return (u, d) =>
            {
                var sz = field.Read(u);
                var dt = dtBWL_[sz];
                if (dt is null)
                    return false;
                d.dataWidth = dt;
                return true;
            };
        }
        private static readonly Mutator<RxDisassembler> bwl_0 = bwl_(0);
        private static readonly Mutator<RxDisassembler> bwl_10 = bwl_(10);
        private static readonly Mutator<RxDisassembler> bwl_12 = bwl_(12);

        private static bool pcdisp3(uint uInstr, RxDisassembler dasm)
        {
            var dispCode = uInstr & 0b111;
            var addrDst = dasm.addr + disp3codes[dispCode];
            dasm.ops.Add(addrDst);
            return true;
        }

        private static bool pcdisp8(uint uInstr, RxDisassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out byte bDisp))
                return false;
            var addrDst = dasm.addr + (sbyte) bDisp;
            dasm.ops.Add(addrDst);
            return true;
        }

        private static bool pcdisp16(uint uInstr, RxDisassembler dasm)
        {
            if (!dasm.rdr.TryReadInt16(out short sDisp))
                return false;
            var addrDst = dasm.addr + sDisp;
            dasm.ops.Add(addrDst);
            return true;
        }

        private static bool pcdisp24(uint uInstr, RxDisassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out byte bDisp))
                return false;
            if (!dasm.rdr.TryReadInt16(out short wDisp))
                return false;
            var addrDst =
                dasm.addr +
                ((((int)(sbyte)bDisp) << 16) |
                (int)wDisp);
            dasm.ops.Add(addrDst);
            return true;
        }

        private static bool SwapOperands(uint uInstr, RxDisassembler dasm)
        {
            Debug.Assert(dasm.ops.Count >= 2);
            var iLast = dasm.ops.Count - 1;
            var tmp = dasm.ops[iLast - 1];
            dasm.ops[iLast - 1] = dasm.ops[iLast];
            dasm.ops[iLast] = tmp;
            return true;
        }

        #endregion

        #region Decoders

        private class Fetch8Decoder : Decoder<RxDisassembler, Mnemonic, RxInstruction>
        {
            private readonly Decoder<RxDisassembler, Mnemonic, RxInstruction> nextDecoder;

            public Fetch8Decoder(Decoder<RxDisassembler, Mnemonic, RxInstruction> nextDecoder)
            {
                this.nextDecoder = nextDecoder;
            }

            public override RxInstruction Decode(uint wInstr, RxDisassembler dasm)
            {
                if (!dasm.rdr.TryReadByte(out byte b2))
                    return dasm.CreateInvalidInstruction();
                return nextDecoder.Decode((wInstr << 8) | b2, dasm);
            }
        }

        private class Instr24Decoder : Decoder<RxDisassembler, Mnemonic, RxInstruction>
        {
            private readonly Decoder<RxDisassembler, Mnemonic, RxInstruction> nextDecoder;

            public Instr24Decoder(Decoder<RxDisassembler, Mnemonic, RxInstruction> nextDecoder)
            {
                this.nextDecoder = nextDecoder;
            }

            public override RxInstruction Decode(uint uInstr, RxDisassembler dasm)
            {
                if (!dasm.rdr.TryReadBeUInt16(out ushort b2))
                    return dasm.CreateInvalidInstruction();
                var wInstr = (uInstr << 16) | b2;
                return nextDecoder.Decode(wInstr, dasm);
            }
        }

        private class Fetch16Decoder : Decoder<RxDisassembler, Mnemonic, RxInstruction>
        {
            private readonly Decoder<RxDisassembler, Mnemonic, RxInstruction> nextDecoder;

            public Fetch16Decoder(Decoder<RxDisassembler, Mnemonic, RxInstruction> nextDecoder)
            {
                this.nextDecoder = nextDecoder;
            }

            public override RxInstruction Decode(uint uInstr, RxDisassembler dasm)
            {
                if (!dasm.rdr.TryReadBeUInt16(out ushort w))
                    return dasm.CreateInvalidInstruction();
                uint wInstr = (uInstr << 16) | w;
                return nextDecoder.Decode(wInstr, dasm);
            }
        }


        #endregion

        private static InstrDecoder<RxDisassembler, Mnemonic, RxInstruction> Instr(Mnemonic mnemonic, params Mutator<RxDisassembler>[] mutators)
            => new InstrDecoder<RxDisassembler, Mnemonic, RxInstruction>(InstrClass.Linear, mnemonic, mutators);

        private static InstrDecoder<RxDisassembler, Mnemonic, RxInstruction> Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<RxDisassembler>[] mutators)
            => new InstrDecoder<RxDisassembler, Mnemonic, RxInstruction>(iclass, mnemonic, mutators);

        private static Fetch8Decoder Instr16(Mnemonic mnemonic, params Mutator<RxDisassembler>[] mutators)
            => new Fetch8Decoder(Instr(mnemonic, InstrClass.Linear, mutators));

        private static Fetch8Decoder Fetch8(Decoder<RxDisassembler, Mnemonic, RxInstruction> decoder)
            => new Fetch8Decoder(decoder);

        private static Fetch16Decoder Fetch16(Decoder<RxDisassembler, Mnemonic, RxInstruction> decoder)
            => new Fetch16Decoder(decoder);

        private static Decoder Nyi(string message)
            => new NyiDecoder<RxDisassembler, Mnemonic, RxInstruction>(message);

        static RxDisassembler()
        {
            var invalid = Instr(Mnemonic.Invalid, InstrClass.Invalid);
            var nyi = Nyi("nyi");

            var adc_Msrc_dst = Mask(16, 2, "  adc",
                invalid,
                Instr(Mnemonic.adc, L, Mr4_d8, R0),
                Instr(Mnemonic.adc, L, Mr4_d16, R0),
                invalid);

            var add_Msrc_dst = Mask(8, 2, "  add",
                Instr(Mnemonic.add, bwluw14, Mr4, R0),
                Instr(Mnemonic.add, bwluw14, Mr4_d8, R0),
                Instr(Mnemonic.add, bwluw14, Mr4_d16, R0),
                Instr(Mnemonic.add, bwluw14, R4, R0));

            var and_Msrc_dst = Mask(8, 2, "  and",
                Instr(Mnemonic.and, bwluw14, Mr4, R0),
                Instr(Mnemonic.and, bwluw14, Mr4_d8, R0),
                Instr(Mnemonic.and, bwluw14, Mr4_d16, R0),
                Instr(Mnemonic.and, bwluw14, R4, R0));

            var cmp_Msrc_dst = Mask(8, 2, "  cmp",
                Instr(Mnemonic.cmp, bwluw14, Mr4, R0),
                Instr(Mnemonic.cmp, bwluw14, Mr4_d8, R0),
                Instr(Mnemonic.cmp, bwluw14, Mr4_d16, R0),
                Instr(Mnemonic.cmp, bwluw14, R4, R0));

            var shar_3op = Instr(Mnemonic.shar, UI8_5, R4, R0);
            var shll_3op = Instr(Mnemonic.shll, UI8_5, R4, R0);
            var shlr_3op = Instr(Mnemonic.shlr, UI8_5, R4, R0);

            var decode06 = Fetch8(Sparse(2, 4, "  06", Nyi("06"),
                (0b0000, Fetch8(Instr(Mnemonic.sub, bwluw22, Mr4_8, R0))),
                (0b0001, Fetch8(cmp_Msrc_dst)),
                (0b0010, Fetch8(add_Msrc_dst)),
                (0b0011, Fetch8(Instr(Mnemonic.mul, bwluw22, Mr4_8, R0))),
                (0b0100, Fetch8(and_Msrc_dst)),
                (0b0101, Fetch8(Instr(Mnemonic.or, bwluw22, Mr4_8, R0))),
                (0b1000, Fetch16(Sparse(8, 8, "  1000", Nyi("06 1000"),
                    (0x00, Instr(Mnemonic.sbb, L, Mr4_16, R0)),
                    (0x02, adc_Msrc_dst),
                    (0x04, Instr(Mnemonic.max, bwluw22, Mr4_16, R0)),
                    (0x05, Instr(Mnemonic.min, bwluw22, Mr4_16, R0)),
                    (0x06, Instr(Mnemonic.emul, bwluw22, Mr4_16, R0)),
                    (0x07, Instr(Mnemonic.emulu, bwluw22, Mr4_16, R0)),
                    (0x08, Instr(Mnemonic.div, bwluw22, Mr4_16, R0)),
                    (0x09, Instr(Mnemonic.divu, bwluw22, Mr4_16, R0)),
                    (0x0C, Instr(Mnemonic.tst, bwluw22, Mr4_16, R0)),
                    (0x0D, Instr(Mnemonic.xor, bwluw22, Mr4_16, R0)),
                    (0x10, Instr(Mnemonic.xchg, bwluw22, Mr4_16, R0)),
                    (0x11, Instr(Mnemonic.itof, bwluw22, Mr4_16, R0)),
                    (0x15, Instr(Mnemonic.utof, bwluw22, Mr4_16, R0))
                    )))));

            var decode74 = Fetch8(Sparse(4, 4, "  74", Nyi("74"),
                (0b0000, Instr(Mnemonic.cmp, Simm8, R0)),
                (0b0010, Instr(Mnemonic.and, Simm8, R0))));

            var decode75 = Fetch8(Sparse(4, 4, "  75", Nyi("75"),
                (0b0000, Instr(Mnemonic.cmp, Simm8, R0)),
                (0b0001, Fetch8(cmp_Msrc_dst)),
                (0b0010, Instr(Mnemonic.and, Simm8, R0)),
                (0b0100, Instr(Mnemonic.mov, Uimm8, R0)),
                (0b0101, Instr(Mnemonic.cmp, Uimm8, R0)),
                (0b0110, If(0, 4, u => u == 0, Instr(Mnemonic.@int, Uimm8))),
                (0b0111, Fetch8(Instr(Mnemonic.mvtipl, UI_0_4)))));

            var decode76 = Fetch8(Sparse(4, 4, "  76", Nyi("76"),
                (0b0000, Instr(Mnemonic.cmp, Simm8, R0)),
                (0b0001, Fetch8(cmp_Msrc_dst)),
                (0b0010, Instr(Mnemonic.and, Simm8, R0)),
                (0b0011, Instr(Mnemonic.or, Simm8, R0))));

            var decode77 = Fetch8(Sparse(4, 4, "  77", Nyi("77"),
                (0b0000, Instr(Mnemonic.cmp, Simm8, R0)),
                (0b0001, Instr(Mnemonic.mul, Simm8, R0)),
                (0b0010, Instr(Mnemonic.and, Simm8, R0))));

            var decode7E = Fetch8(Sparse(4, 4, "  7E", Nyi("7E"),
                (0b0000, Instr(Mnemonic.not, R0)),
                (0b0001, Instr(Mnemonic.neg, R0)),
                (0b0010, Instr(Mnemonic.abs, R0)),
                (0b0011, Instr(Mnemonic.sat, R0)),
                (0b0100, Instr(Mnemonic.rorc, R0)),
                (0b0101, Instr(Mnemonic.rolc, R0)),
                (0b1001, Instr(Mnemonic.push, R0)),
                (0b1011, Instr(Mnemonic.pop, R0)),
                (0b1100, Instr(Mnemonic.pushc, CR0)),
                (0b1110, Instr(Mnemonic.popc, CR0))
                ));

            var decode7F = Fetch8(Sparse(4, 4, "  7F", Nyi("7F"),
                (0x0, Instr(Mnemonic.jmp, InstrClass.Transfer, R0)),
                (0x1, Instr(Mnemonic.jsr, InstrClass.Transfer | InstrClass.Call, R0)),
                (0x4, Instr(Mnemonic.bra, InstrClass.Transfer, L, R0)),
                (0x5, Instr(Mnemonic.bsr, InstrClass.Transfer | InstrClass.Call, L, R0)),
                (0x8, Mask(0, 4, "  8",
                        Instr(Mnemonic.suntil, bwl_0),
                        Instr(Mnemonic.suntil, bwl_0),
                        Instr(Mnemonic.suntil, bwl_0),
                        Instr(Mnemonic.scmpu),
                        Instr(Mnemonic.swhile, bwl_0),
                        Instr(Mnemonic.swhile, bwl_0),
                        Instr(Mnemonic.swhile, bwl_0),
                        Instr(Mnemonic.smovu),
                        Instr(Mnemonic.sstr, bwl_0),
                        Instr(Mnemonic.sstr, bwl_0),
                        Instr(Mnemonic.sstr, bwl_0),
                        Instr(Mnemonic.smovb),
                        Instr(Mnemonic.rmpa, bwl_0),
                        Instr(Mnemonic.rmpa, bwl_0),
                        Instr(Mnemonic.rmpa, bwl_0),
                        Instr(Mnemonic.smovf))),
                (0x9, Sparse(0, 4, "  9", Nyi("9"),
                    (0x3, Instr(Mnemonic.satr)),
                    (0x4, Instr(Mnemonic.rtfi, InstrClass.Transfer|InstrClass.Return)),
                    (0x5, Instr(Mnemonic.rte, InstrClass.Transfer|InstrClass.Return)),
                    (0x6, Instr(Mnemonic.wait)),
                    (0x7, Instr(Mnemonic.smovu)),
                    (0xB, Instr(Mnemonic.smovb)),
                    (0xF, Instr(Mnemonic.smovf)))),
                (0xA, Instr(Mnemonic.setpsw, pswbit)),
                (0xB, Instr(Mnemonic.clrpsw, pswbit))));

            var sccnd = Mask(0, 4, "  sccnd",
                Instr(Mnemonic.sceq,  bwl_10, Mr4_8),
                Instr(Mnemonic.scne,  bwl_10, Mr4_8),
                Instr(Mnemonic.scgeu, bwl_10, Mr4_8),
                Instr(Mnemonic.scltu, bwl_10, Mr4_8),
                Instr(Mnemonic.scgtu, bwl_10, Mr4_8),
                Instr(Mnemonic.scleu, bwl_10, Mr4_8),
                Instr(Mnemonic.scpz,  bwl_10, Mr4_8),
                Instr(Mnemonic.scn,   bwl_10, Mr4_8),
                Instr(Mnemonic.scge,  bwl_10, Mr4_8),
                Instr(Mnemonic.sclt,  bwl_10, Mr4_8),
                Instr(Mnemonic.scgt,  bwl_10, Mr4_8),
                Instr(Mnemonic.scle,  bwl_10, Mr4_8),
                Instr(Mnemonic.sco,   bwl_10, Mr4_8),
                Instr(Mnemonic.scno,  bwl_10, Mr4_8),
                invalid,
                invalid);

            var decodeFC = new Instr24Decoder(Sparse(13, 3, "  FC", Nyi("FC"),
                (0b000, Sparse(8, 5, "  FC 0", Nyi("FC 0"),
                    (0x03, Instr(Mnemonic.sbb, R4, R0)),
                    (0x07, Instr(Mnemonic.neg, R4, R0)),
                    (0x0B, Instr(Mnemonic.adc, R4, R0)),
                    (0x0F, Instr(Mnemonic.abs, R4, R0)),
                    (0x10, Instr(Mnemonic.max, bwluw22, Mr4_8, R0)),
                    (0x11, Instr(Mnemonic.max, bwluw22, Mr4_8, R0)),
                    (0x12, Instr(Mnemonic.max, bwluw22, Mr4_8, R0)),
                    (0x13, Instr(Mnemonic.max, bwluw22, Mr4_8, R0)),
                    (0x14, Instr(Mnemonic.min, bwluw22, Mr4_8, R0)),
                    (0x15, Instr(Mnemonic.min, bwluw22, Mr4_8, R0)),
                    (0x16, Instr(Mnemonic.min, bwluw22, Mr4_8, R0)),
                    (0x17, Instr(Mnemonic.min, bwluw22, Mr4_8, R0)),
                    (0x1C, Instr(Mnemonic.emulu, L, Mr4_8, R0)),
                    (0x1D, Instr(Mnemonic.emulu, L, Mr4_8, R0)),
                    (0x1E, Instr(Mnemonic.emulu, L, Mr4_8, R0)),
                    (0x1F, Instr(Mnemonic.emulu, L, Mr4_8, R0)))),
                (0b001, Sparse(10, 3, "  FC 1", Nyi("FC 1"),
                    (0b100, Instr(Mnemonic.tst, UB, Mr4_8, R0)),
                    (0b101, Instr(Mnemonic.xor, UB, Mr4_8, R0)),
                    (0b110, Instr(Mnemonic.not, UB, Mr4_8, R0)))),
                (0b010, Sparse(10, 3, "  FC 2", Nyi("FC 2"),
                    (0b000, Instr(Mnemonic.xchg, UB, Mr4_8, R0)),
                    (0b001, Instr(Mnemonic.itof, L, Mr4_8, R0)),
                    (0b010, Instr(Mnemonic.stnz, R4, R0)),
                    (0b101, Instr(Mnemonic.utof, UB, Mr4_8, R0)))),
                (0b011, Sparse(8, 5, "  FC 3", Nyi("FC 3"),
                    (0x00, Instr(Mnemonic.bset, W, R0, Mr4)),
                    (0x01, Instr(Mnemonic.bset, W, R0, Mr4_d8)),
                    (0x02, Instr(Mnemonic.bset, W, R0, Mr4_d16)),
                    (0x03, Instr(Mnemonic.bset, R0, R4)),
                    (0x04, Instr(Mnemonic.bclr, W, R0, Mr4)),
                    (0x05, Instr(Mnemonic.bclr, W, R0, Mr4_d8)),
                    (0x06, Instr(Mnemonic.bclr, W, R0, Mr4_d16)),
                    (0x07, Instr(Mnemonic.bclr, R0, R4)),
                    (0x08, Instr(Mnemonic.btst, W, R0, Mr4)),
                    (0x09, Instr(Mnemonic.btst, W, R0, Mr4_d8)),
                    (0x0A, Instr(Mnemonic.btst, W, R0, Mr4_d16)),
                    (0x0B, Instr(Mnemonic.btst, R0, R4)),
                    (0x0C, Instr(Mnemonic.bnot, W, R0, Mr4)),
                    (0x0D, Instr(Mnemonic.bnot, W, R0, Mr4_d8)),
                    (0x0E, Instr(Mnemonic.bnot, W, R0, Mr4_d16)),
                    (0x0F, Instr(Mnemonic.bnot, R0, R4))
                    )),
                (0b100, Sparse(10, 3, "  FC 4", Nyi("FC 4"),
                    (0b000, Instr(Mnemonic.fsub, L, Mr4_8, R0)),
                    (0b001, Instr(Mnemonic.fcmp, L, Mr4_8, R0)),
                    (0b010, Instr(Mnemonic.fadd, L, Mr4_8, R0)),
                    (0b011, Instr(Mnemonic.fmul, L, Mr4_8, R0)),
                    (0b101, Instr(Mnemonic.ftoi, L, Mr4_8, R0)),
                    (0b110, Instr(Mnemonic.round, L, Mr4_8, R0)))),
                (0b101, Sparse(10, 3, "  FC 5", Nyi("FC 5"),
                    (0b000, Instr(Mnemonic.fsqrt, L, Mr4_8, R0)),
                    (0b001, Instr(Mnemonic.ftou, L, Mr4_8, R0)))),
                (0b110, Mask(12, 1, "  FC 6",
                    nyi,
                    sccnd)),
                (0b111, Mask(0, 4, "  FC 7",
                    Instr(Mnemonic.bmeq, B, UI_10_3, Mr4_8),
                    Instr(Mnemonic.bmne, B, UI_10_3, Mr4_8),
                    Instr(Mnemonic.bmgeu, B, UI_10_3, Mr4_8),
                    Instr(Mnemonic.bmltu, B, UI_10_3, Mr4_8),

                    Instr(Mnemonic.bmgtu, B, UI_10_3, Mr4_8),
                    Instr(Mnemonic.bmleu, B, UI_10_3, Mr4_8),
                    Instr(Mnemonic.bmpz, B, UI_10_3, Mr4_8),
                    Instr(Mnemonic.bmn, B, UI_10_3, Mr4_8),

                    Instr(Mnemonic.bmge, B, UI_10_3, Mr4_8),
                    Instr(Mnemonic.bmlt, B, UI_10_3, Mr4_8),
                    Instr(Mnemonic.bmgt, B, UI_10_3, Mr4_8),
                    Instr(Mnemonic.bmle, B, UI_10_3, Mr4_8),

                    Instr(Mnemonic.bmo, B, UI_10_3, Mr4_8),
                    Instr(Mnemonic.bmno, B, UI_10_3, Mr4_8),
                    invalid,
                    Instr(Mnemonic.bnot, B, UI_10_3, Mr4_8)))
                ));

            var decodeFD_0001 = Sparse(9, 3, "  0001", Nyi("FD 0001"),
                (0b011, Mask(4, 4, "  011",
                    Instr(Mnemonic.mvtachi, R0, A0),
                    Instr(Mnemonic.mvtaclo, R0, A0),
                    nyi,
                    Instr(Mnemonic.mvtacgu, R0, A0),
                    nyi,
                    nyi,
                    nyi,
                    nyi,

                    Instr(Mnemonic.mvtachi, R0, A1),
                    Instr(Mnemonic.mvtaclo, R0, A1),
                    nyi,
                    Instr(Mnemonic.mvtacgu, R0, A1),
                    nyi,
                    nyi,
                    nyi,
                    nyi)),
                (0b100, Mask(6, 3, 4, 1, "  100",
                        Instr(Mnemonic.racw, C1, A0),
                        Instr(Mnemonic.racw, C2, A0),
                        Instr(Mnemonic.rdacw, C1, A0),
                        Instr(Mnemonic.rdacw, C2, A0),
                        nyi,
                        nyi,
                        Instr(Mnemonic.rdacw, C1, A0),
                        Instr(Mnemonic.rdacw, C2, A0),

                        Instr(Mnemonic.racl, C1, A1),
                        Instr(Mnemonic.racl, C2, A1),
                        Instr(Mnemonic.rdacl, C1, A1),
                        Instr(Mnemonic.rdacl, C2, A1),
                        Instr(Mnemonic.racl, C1, A1),
                        Instr(Mnemonic.racl, C2, A1),
                        Instr(Mnemonic.rdacl, C1, A1),
                        Instr(Mnemonic.rdacl, C2, A1)
                    )),
                (0b111, Mask(4, 2, "  111",
                    Mask(6, 3, "  mvfachi",
                        Instr(Mnemonic.mvfachi, C2, A0, R0),
                        Instr(Mnemonic.mvtachi, R0, A0),
                        Instr(Mnemonic.mvfachi, C2, A1, R0),
                        Instr(Mnemonic.mvtachi, R0, A1),
                        Instr(Mnemonic.mvfachi, C0, A0, R0),
                        Instr(Mnemonic.mvfachi, C1, A0, R0),
                        Instr(Mnemonic.mvfachi, C0, A1, R0),
                        Instr(Mnemonic.mvfachi, C1, A1, R0)),
                    Mask(6, 3, "  mvfaclo",
                        Instr(Mnemonic.mvfaclo, C2, A0, R0),
                        Instr(Mnemonic.mvtaclo, R0, A0),
                        Instr(Mnemonic.mvfaclo, C2, A1, R0),
                        Instr(Mnemonic.mvtaclo, R0, A1),
                        Instr(Mnemonic.mvfaclo, C0, A0, R0),
                        Instr(Mnemonic.mvfaclo, C1, A0, R0),
                        Instr(Mnemonic.mvfaclo, C0, A1, R0),
                        Instr(Mnemonic.mvfaclo, C1, A1, R0)),
                    Mask(6, 3, "  mvfacmi",
                        Instr(Mnemonic.mvfacmi, C2, A0, R0),
                        Instr(Mnemonic.mvtacmi, R0, A0),
                        Instr(Mnemonic.mvfacmi, C2, A1, R0),
                        Instr(Mnemonic.mvtacmi, R0, A1),
                        Instr(Mnemonic.mvfacmi, C0, A0, R0),
                        Instr(Mnemonic.mvfacmi, C1, A0, R0),
                        Instr(Mnemonic.mvfacmi, C0, A1, R0),
                        Instr(Mnemonic.mvfacmi, C1, A1, R0)),
                    Mask(6, 3, "  mvfacgu",
                        Instr(Mnemonic.mvfacgu, C2, A0, R0),
                        Instr(Mnemonic.mvtacgu, R0, A0),
                        Instr(Mnemonic.mvfacgu, C2, A1, R0),
                        Instr(Mnemonic.mvtacgu, R0, A1),
                        Instr(Mnemonic.mvfacgu, C0, A0, R0),
                        Instr(Mnemonic.mvfacgu, C1, A0, R0),
                        Instr(Mnemonic.mvfacgu, C0, A1, R0),
                        Instr(Mnemonic.mvfacgu, C1, A1, R0)
                ))));

            var decodeFD = new Instr24Decoder(Sparse(12, 4, "  FD", Nyi("FD"),
                (0b0000, Mask(8, 4, "  0000",
                    Instr(Mnemonic.mulhi, R4, R0, A0),
                    Instr(Mnemonic.mullo, R4, R0, A0),
                    Instr(Mnemonic.mullh, R4, R0, A0),
                    Instr(Mnemonic.emula, R4, R0, A0),
                    Instr(Mnemonic.machi, R4, R0, A0),
                    Instr(Mnemonic.maclo, R4, R0, A0),
                    Instr(Mnemonic.maclh, R4, R0, A0),
                    Instr(Mnemonic.emaca, R4, R0, A0),
                    Instr(Mnemonic.mulhi, R4, R0, A1),
                    Instr(Mnemonic.mullo, R4, R0, A1),
                    Instr(Mnemonic.mullh, R4, R0, A1),
                    Instr(Mnemonic.emula, R4, R0, A1),
                    Instr(Mnemonic.machi, R4, R0, A1),
                    Instr(Mnemonic.maclo, R4, R0, A1),
                    Instr(Mnemonic.maclh, R4, R0, A1),
                    Instr(Mnemonic.emaca, R4, R0, A1))),
                (0b0001, decodeFD_0001),

                (0b0010, Mask(8, 4, "  0010",
                    Instr(Mnemonic.mov, B, Mpostinc, R0),
                    Instr(Mnemonic.mov, W, Mpostinc, R0),
                    Instr(Mnemonic.mov, L, Mpostinc, R0),
                    nyi,

                    Instr(Mnemonic.mov, B, Mpredec, R0),
                    Instr(Mnemonic.mov, W, Mpredec, R0),
                    Instr(Mnemonic.mov, L, Mpredec, R0),
                    Instr(Mnemonic.movco, R0, R4),

                    Instr(Mnemonic.mov, B, R0, Mpostinc),
                    Instr(Mnemonic.mov, W, R0, Mpostinc),
                    Instr(Mnemonic.mov, L, R0, Mpostinc),
                    nyi,

                    Instr(Mnemonic.mov, B, R0, Mpredec),
                    Instr(Mnemonic.mov, W, R0, Mpredec),
                    Instr(Mnemonic.mov, L, R0, Mpredec),
                    Instr(Mnemonic.movli, R0, R4)
                                        )),
                (0b0011, Mask(10, 2, 8, 1, "  0011",
                    nyi,
                    nyi,
                    nyi,
                    nyi,
                    Instr(Mnemonic.movu, B, Mpostinc, R0),
                    Instr(Mnemonic.movu, W, Mpostinc, R0),
                    Instr(Mnemonic.movu, B, Mpredec, R0),
                    Instr(Mnemonic.movu, W, Mpredec, R0))),

                (0b0100, Sparse(8, 4, "  0100", Nyi("FD 0100"),
                    (0b0100, Instr(Mnemonic.msbhi, R4, R0, A0)),
                    (0b0101, Instr(Mnemonic.msblo, R4, R0, A0)),
                    (0b0110, Instr(Mnemonic.msblh, R4, R0, A0)),
                    (0b0111, Instr(Mnemonic.emsba, R4, R0, A0)),
                    (0b1100, Instr(Mnemonic.msbhi, R4, R0, A1)),
                    (0b1101, Instr(Mnemonic.msblo, R4, R0, A1)),
                    (0b1110, Instr(Mnemonic.msblh, R4, R0, A1)),
                    (0b1111, Instr(Mnemonic.emsba, R4, R0, A1)))),
                (0b0110, Sparse(8, 4, "  0110", Nyi("FD 0110"),
                    (0b0000, Instr(Mnemonic.shlr, R4, R0)),
                    (0b0001, Instr(Mnemonic.shar, R4, R0)),
                    (0b0010, Instr(Mnemonic.shll, R4, R0)),
                    (0b0100, Instr(Mnemonic.rotr, R4, R0)),
                    (0b0101, Instr(Mnemonic.revw, R4, R0)),
                    (0b0110, Instr(Mnemonic.rotl, R4, R0)),
                    (0b0111, Instr(Mnemonic.revl, R4, R0)),
                    (0b1000, Instr(Mnemonic.mvtc, R4, CR0)),
                    (0b1010, Instr(Mnemonic.mvfc, CR4, R0)),
                    (0b1100, Instr(Mnemonic.rotr, UI4_5, R0)),
                    (0b1101, Instr(Mnemonic.rotr, UI4_5, R0)),
                    (0b1110, Instr(Mnemonic.rotl, UI4_5, R0)),
                    (0b1111, Instr(Mnemonic.rotl, UI4_5, R0)))),
                (0b0111, Sparse(4, 6, "  0111", Nyi("FD 0111"),
                    (0b000010, Instr(Mnemonic.adc, Simm10, R0)),
                    (0b000100, Instr(Mnemonic.max, Simm10, R0)),
                    (0b000101, Instr(Mnemonic.min, Simm10, R0)),
                    (0b000110, Instr(Mnemonic.emul, Simm10, R0)),
                    (0b000111, Instr(Mnemonic.emulu, Simm10, R0)),
                    (0b001000, Instr(Mnemonic.div, Simm10, R0)),
                    (0b001001, Instr(Mnemonic.divu, Simm10, R0)),
                    (0b001100, Instr(Mnemonic.tst, Simm10, R0)),
                    (0b001101, Instr(Mnemonic.xor, Simm10, R0)),
                    (0b001110, Instr(Mnemonic.stz, Simm10, R0)),
                    (0b001111, Instr(Mnemonic.stnz, Simm10, R0)),
                    (0b100000, Instr(Mnemonic.fsub, Fimm, R0)),
                    (0b100001, Instr(Mnemonic.fcmp, Fimm, R0)),
                    (0b100010, Instr(Mnemonic.fadd, Fimm, R0)),
                    (0b100011, Instr(Mnemonic.fmul, Fimm, R0)),
                    (0b100100, Instr(Mnemonic.fdiv, Fimm, R0)),
                    (0b110000, Instr(Mnemonic.mvtc, Simm10, CR0)))),
                (0b1000, shlr_3op),
                (0b1001, shlr_3op),
                (0b1010, shar_3op),
                (0b1011, shar_3op),
                (0b1100, shll_3op),
                (0b1101, shll_3op),
                (0b1111, Sparse(4, 4, "  1111", Nyi("FD 1111"),
                    (0b1111, Instr(Mnemonic.bnot, UI8_5, R0))))));

            var decodeFE = Fetch16(Sparse(13, 3, "  FE", Nyi("FE"),
                (0b000, Instr(Mnemonic.mov, bwl_12, R0, Midx)),
                (0b001, Instr(Mnemonic.mov, bwl_12, R0, Midx)),
                (0b010, Instr(Mnemonic.mov, bwl_12, Midx, R0)),
                (0b011, Instr(Mnemonic.mov, bwl_12, Midx, R0)),
                (0b110, Mask(12, 1, 
                    Instr(Mnemonic.movu, B, Midx, R0),
                    Instr(Mnemonic.movu, W, Midx, R0))),
                (0b111, Mask(4, 4, "  bmcnd",
                    Instr(Mnemonic.bmeq, UI8_5, R0),
                    Instr(Mnemonic.bmne, UI8_5, R0),
                    Instr(Mnemonic.bmgeu, UI8_5, R0),
                    Instr(Mnemonic.bmltu, UI8_5, R0),
                    Instr(Mnemonic.bmgtu, UI8_5, R0),
                    Instr(Mnemonic.bmleu, UI8_5, R0),
                    Instr(Mnemonic.bmpz, UI8_5, R0),
                    Instr(Mnemonic.bmn, UI8_5, R0),

                    Instr(Mnemonic.bmge, UI8_5, R0),
                    Instr(Mnemonic.bmlt, UI8_5, R0),
                    Instr(Mnemonic.bmgt, UI8_5, R0),
                    Instr(Mnemonic.bmle, UI8_5, R0),
                    Instr(Mnemonic.bmo, UI8_5, R0),
                    Instr(Mnemonic.bmno, UI8_5, R0),
                    invalid,
                    invalid))));

            var decodeFF = Fetch8(Sparse(4, 4, "  FF", Nyi("FF"),
                (0b0000, Fetch8(Instr(Mnemonic.sub, R8,R4,R0))),
                (0b0010, Fetch8(Instr(Mnemonic.add, R8,R4,R0))),
                (0b0011, Fetch8(Instr(Mnemonic.mul, R8,R4,R0))),
                (0b0100, Fetch8(Instr(Mnemonic.and, R8,R4,R0))),
                (0b0101, Fetch8(Instr(Mnemonic.or, R8,R4,R0))),
                (0b1000, Fetch8(Instr(Mnemonic.fsub, R8,R4,R0))),
                (0b1010, Fetch8(Instr(Mnemonic.fadd, R8,R4,R0))),
                (0b1011, Fetch8(Instr(Mnemonic.fmul, R8,R4,R0)))
                ));

            var add_ub = new Fetch8Decoder(Mask(8, 2, "  add_ub",
                invalid,
                Instr(Mnemonic.add, UB, Mr4_d8, R0),
                Instr(Mnemonic.add, UB, Mr4_d16, R0),
                invalid));
            var and_ub = new Fetch8Decoder(Mask(8, 2, "  and_ub",
                Instr(Mnemonic.and, UB, Mr4, R0),
                Instr(Mnemonic.and, UB, Mr4_d8, R0),
                Instr(Mnemonic.and, UB, Mr4_d16, R0),
                Instr(Mnemonic.and, UB, R4, R0)));
            var add_imm = Fetch8(Instr(Mnemonic.add, Simm8, R4, R0));

            var bclr_imm3 = Fetch8(Mask(3, 1, " bclr/bset?",
                Instr(Mnemonic.bset, UB, UI_0_3, Mr4_8),
                Instr(Mnemonic.bclr, UB, UI_0_3, Mr4_8)));
            var bclr_imm5 = Fetch8(Instr(Mnemonic.bclr, UI4_5, R0));
            var bset_imm5 = Fetch8(Instr(Mnemonic.bset, UI4_5, R0));
            var beq_pcdisp3 = Instr(Mnemonic.beq, InstrClass.ConditionalTransfer, S, pcdisp3);
            var bne_pcdisp3 = Instr(Mnemonic.bne, InstrClass.ConditionalTransfer, S, pcdisp3);
            var bra_pcdisp3 = Instr(Mnemonic.bra, InstrClass.Transfer, S, pcdisp3);
            var bra_pcdisp24 = Instr(Mnemonic.bra, InstrClass.Transfer, pcdisp24);
            var btst_imm3 = Fetch8(Mask(3, 1, " btst/?",
                Instr(Mnemonic.btst, UI_0_3, R0),
                Instr(Mnemonic.push, bwl_0, Mr4_8)));
            var btst_imm5 = Fetch8(Instr(Mnemonic.btst, UI4_5, R0));
            var cmp_ub = Fetch8(Mask(8, 2, "  cmp.ub",
                Instr(Mnemonic.cmp, UB, Mr4, R0),
                Instr(Mnemonic.cmp, UB, Mr4_d8, R0),
                Instr(Mnemonic.cmp, UB, Mr4_d16, R0),
                invalid));
            var mov_B_reg3 = Fetch8(Mask(11, 1, "  mov.b reg3",
                Instr(Mnemonic.mov, B, R0_3, Mdsp5_3),
                Instr(Mnemonic.mov, B, Mdsp5_3, R0_3)));
            var mov_W_reg3 = Fetch8(Mask(11, 1, "  mov.w reg3",
                Instr(Mnemonic.mov, W, R0_3, Mdsp5_3),
                Instr(Mnemonic.mov, W, Mdsp5_3, R0_3)));
            var mov_L_reg3 = Fetch8(Mask(11, 1, "  mov.l reg3",
                Instr(Mnemonic.mov, L, R0_3, Mdsp5_3),
                Instr(Mnemonic.mov, L, Mdsp5_3, R0_3)));
            var movSimmMem = Fetch8(Mask(0, 2, "  F8",
                Instr(Mnemonic.mov, B, Mr4_8, Simm2, SwapOperands),
                Instr(Mnemonic.mov, W, Mr4_8, Simm2, SwapOperands),
                Instr(Mnemonic.mov, L, Mr4_8, Simm2, SwapOperands),
                invalid));
            var movu_b = Fetch8(Instr(Mnemonic.movu, B, Mdsp5_3, R0_3));
            var movu_w = Fetch8(Instr(Mnemonic.movu, W, Mdsp5_3, R0_3));
            var mul_ub = Fetch8(Instr(Mnemonic.mul, UB, Mr4_8, R0));
            var or_ub = Fetch8(Instr(Mnemonic.or, UB, Mr4_8, R0));
            var sub_ub = Fetch8(Mask(8, 2, "  sub.ub",
                Instr(Mnemonic.sub, UB, Mr4, R0),
                Instr(Mnemonic.sub, UB, Mr4_d8, R0),
                Instr(Mnemonic.sub, UB, Mr4_d16, R0),
                invalid));

            rootDecoder = Sparse(0, 8, "Rx", Nyi("Rx"),
                // 00
                (0x00, Instr(Mnemonic.brk, InstrClass.Terminates|InstrClass.Zero)),
                (0x02, Instr(Mnemonic.rts, InstrClass.Transfer|InstrClass.Return)),
                (0x03, Instr(Mnemonic.nop, InstrClass.Padding|InstrClass.Linear)),
                (0x04, bra_pcdisp24),
                (0x06, decode06),
                (0x08, bra_pcdisp3),

                // 10
                (0x10, beq_pcdisp3),
                (0x11, beq_pcdisp3),
                (0x12, beq_pcdisp3),
                (0x13, beq_pcdisp3),
                (0x14, beq_pcdisp3),
                (0x15, beq_pcdisp3),
                (0x16, beq_pcdisp3),
                (0x17, beq_pcdisp3),
                (0x18, bne_pcdisp3),
                (0x19, bne_pcdisp3),
                (0x1A, bne_pcdisp3),
                (0x1B, bne_pcdisp3),
                (0x1C, bne_pcdisp3),
                (0x1D, bne_pcdisp3),
                (0x1E, bne_pcdisp3),
                (0x1F, bne_pcdisp3),

                // 20
                (0x20, Instr(Mnemonic.beq,  InstrClass.ConditionalTransfer, B, pcdisp8)),
                (0x21, Instr(Mnemonic.bne,  InstrClass.ConditionalTransfer, B, pcdisp8)),
                (0x22, Instr(Mnemonic.bgeu, InstrClass.ConditionalTransfer, B, pcdisp8)),
                (0x23, Instr(Mnemonic.bltu, InstrClass.ConditionalTransfer, B, pcdisp8)),
                (0x24, Instr(Mnemonic.bgtu, InstrClass.ConditionalTransfer, B, pcdisp8)),
                (0x25, Instr(Mnemonic.bleu, InstrClass.ConditionalTransfer, B, pcdisp8)),
                (0x26, Instr(Mnemonic.bpz,  InstrClass.ConditionalTransfer, B, pcdisp8)),
                (0x27, Instr(Mnemonic.bn,   InstrClass.ConditionalTransfer, B, pcdisp8)),
                (0x28, Instr(Mnemonic.bge,  InstrClass.ConditionalTransfer, B, pcdisp8)),
                (0x29, Instr(Mnemonic.blt,  InstrClass.ConditionalTransfer, B, pcdisp8)),
                (0x2A, Instr(Mnemonic.bgt,  InstrClass.ConditionalTransfer, B, pcdisp8)),
                (0x2B, Instr(Mnemonic.ble,  InstrClass.ConditionalTransfer, B, pcdisp8)),
                (0x2C, Instr(Mnemonic.bo,   InstrClass.ConditionalTransfer, B, pcdisp8)),
                (0x2D, Instr(Mnemonic.bno,  InstrClass.ConditionalTransfer, B, pcdisp8)),
                (0x2E, Instr(Mnemonic.bra,  InstrClass.Transfer, B, pcdisp8)),
                (0x2F, invalid),

                // 30
                (0x38, Instr(Mnemonic.bra, InstrClass.Transfer, W, pcdisp16)),
                (0x39, Instr(Mnemonic.bsr, InstrClass.Transfer, W, pcdisp16)),
                (0x3A, Instr(Mnemonic.beq, InstrClass.ConditionalTransfer, W, pcdisp16)),
                (0x3B, Instr(Mnemonic.bne, InstrClass.ConditionalTransfer, W, pcdisp16)),
                (0x3C, Fetch8(Instr(Mnemonic.mov, B, Uimm8, Mdsp5_0))),
                (0x3D, Fetch8(Instr(Mnemonic.mov, W, Uimm8, Mdsp5_0))),
                (0x3E, Fetch8(Instr(Mnemonic.mov, L, Uimm8, Mdsp5_0))),
                (0x3F, Fetch8(Instr(Mnemonic.rtsd, Uimm8, RegRange))),

                // 40
                (0x40, sub_ub),
                (0x41, sub_ub),
                (0x42, sub_ub),
                (0x43, sub_ub),
                (0x44, cmp_ub),
                (0x45, cmp_ub),
                (0x46, cmp_ub),
                (0x47, cmp_ub),
                (0x48, add_ub),
                (0x49, add_ub),
                (0x4A, add_ub),
                (0x4B, add_ub),
                (0x4C, mul_ub),
                (0x4D, mul_ub),
                (0x4E, mul_ub),
                (0x4F, mul_ub),

                // 50
                (0x50, and_ub),
                (0x51, and_ub),
                (0x52, and_ub),
                (0x53, and_ub),
                (0x54, or_ub),
                (0x55, or_ub),
                (0x56, or_ub),
                (0x57, or_ub),
                (0x58, Fetch8(Instr(Mnemonic.movu, B, Mr4_8, R0))),
                (0x59, Fetch8(Instr(Mnemonic.movu, B, Mr4_8, R0))),
                (0x5A, Fetch8(Instr(Mnemonic.movu, B, Mr4_8, R0))),
                (0x5B, Fetch8(Instr(Mnemonic.movu, B, Mr4_8, R0))),
                (0x5C, Fetch8(Instr(Mnemonic.movu, W, Mr4_8, R0))),
                (0x5D, Fetch8(Instr(Mnemonic.movu, W, Mr4_8, R0))),
                (0x5E, Fetch8(Instr(Mnemonic.movu, W, Mr4_8, R0))),
                (0x5F, Fetch8(Instr(Mnemonic.movu, W, Mr4_8, R0))),
                // 60
                (0x60, Instr16(Mnemonic.sub, UI4_4_32, R0)),
                (0x61, Instr16(Mnemonic.cmp, UI4_4_32, R0)),
                (0x62, Instr16(Mnemonic.add, UI4_4_32, R0)),
                (0x63, Instr16(Mnemonic.mul, UI4_4_32, R0)),
                (0x64, Instr16(Mnemonic.and, UI4_4_32, R0)),
                (0x65, Instr16(Mnemonic.or, UI4_4_32, R0)),
                (0x66, Fetch8(Instr(Mnemonic.mov, UI4_4_32, R0))),
                (0x67, Instr(Mnemonic.rtsd, Uimm8)),
                (0x68, Fetch8(Instr(Mnemonic.shlr, UI4_5, R0))),
                (0x69, Fetch8(Instr(Mnemonic.shlr, UI4_5, R0))),
                (0x6A, Fetch8(Instr(Mnemonic.shar, UI4_5, R0))),
                (0x6B, Fetch8(Instr(Mnemonic.shar, UI4_5, R0))),
                (0x6C, Fetch8(Instr(Mnemonic.shll, UI4_5, R0))),
                (0x6D, Fetch8(Instr(Mnemonic.shll, UI4_5, R0))),
                (0x6E, Fetch8(Instr(Mnemonic.pushm, RegRange))),
                (0x6F, Fetch8(Instr(Mnemonic.popm, RegRange))),

                // 70
                (0x70, add_imm),
                (0x71, add_imm),
                (0x72, add_imm),
                (0x73, add_imm),
                (0x74, decode74),
                (0x75, decode75),
                (0x76, decode76),
                (0x77, decode77),
                (0x78, bset_imm5),
                (0x79, bset_imm5),
                (0x7A, bclr_imm5),
                (0x7B, bclr_imm5),
                (0x7C, btst_imm5),
                (0x7D, btst_imm5),
                (0x7E, decode7E),
                (0x7F, decode7F),

                // 80
                (0x80, mov_B_reg3),
                (0x81, mov_B_reg3),
                (0x82, mov_B_reg3),
                (0x83, mov_B_reg3),
                (0x84, mov_B_reg3),
                (0x85, mov_B_reg3),
                (0x86, mov_B_reg3),
                (0x87, mov_B_reg3),
                (0x88, mov_B_reg3),
                (0x89, mov_B_reg3),
                (0x8A, mov_B_reg3),
                (0x8B, mov_B_reg3),
                (0x8C, mov_B_reg3),
                (0x8D, mov_B_reg3),
                (0x8E, mov_B_reg3),
                (0x8F, mov_B_reg3),

                // 90
                (0x90, mov_W_reg3),
                (0x91, mov_W_reg3),
                (0x92, mov_W_reg3),
                (0x93, mov_W_reg3),
                (0x94, mov_W_reg3),
                (0x95, mov_W_reg3),
                (0x96, mov_W_reg3),
                (0x97, mov_W_reg3),
                (0x98, mov_W_reg3),
                (0x99, mov_W_reg3),
                (0x9A, mov_W_reg3),
                (0x9B, mov_W_reg3),
                (0x9C, mov_W_reg3),
                (0x9D, mov_W_reg3),
                (0x9E, mov_W_reg3),
                (0x9F, mov_W_reg3),

                // A0
                (0xA0, mov_L_reg3),
                (0xA1, mov_L_reg3),
                (0xA2, mov_L_reg3),
                (0xA3, mov_L_reg3),
                (0xA4, mov_L_reg3),
                (0xA5, mov_L_reg3),
                (0xA6, mov_L_reg3),
                (0xA7, mov_L_reg3),
                (0xA8, mov_L_reg3),
                (0xA9, mov_L_reg3),
                (0xAA, mov_L_reg3),
                (0xAB, mov_L_reg3),
                (0xAC, mov_L_reg3),
                (0xAD, mov_L_reg3),
                (0xAE, mov_L_reg3),
                (0xAF, mov_L_reg3),

                // B0
                (0xB0, movu_b),
                (0xB1, movu_b),
                (0xB2, movu_b),
                (0xB3, movu_b),
                (0xB4, movu_b),
                (0xB5, movu_b),
                (0xB6, movu_b),
                (0xB7, movu_b),
                (0xB8, movu_w),
                (0xB9, movu_w),
                (0xBA, movu_w),
                (0xBB, movu_w),
                (0xBC, movu_w),
                (0xBD, movu_w),
                (0xBE, movu_w),
                (0xBF, movu_w),

                // C0
                (0xC0, Fetch8(Instr(Mnemonic.mov, B, Mr4_8, Mr0_10))),
                (0xC1, Fetch8(Instr(Mnemonic.mov, B, Mr4_8, Mr0_10))),
                (0xC2, Fetch8(Instr(Mnemonic.mov, B, Mr4_8, Mr0_10))),
                (0xC3, Fetch8(Instr(Mnemonic.mov, B, R0, Mr4_10))),
                (0xC4, Fetch8(Instr(Mnemonic.mov, B, Mr4_8, Mr0_10))),
                (0xC5, Fetch8(Instr(Mnemonic.mov, B, Mr4_8, Mr0_10))),
                (0xC6, Fetch8(Instr(Mnemonic.mov, B, Mr4_8, Mr0_10))),
                (0xC7, Fetch8(Instr(Mnemonic.mov, B, R0, Mr4_10))),
                (0xC8, Fetch8(Instr(Mnemonic.mov, B, Mr4_8, Mr0_10))),
                (0xC9, Fetch8(Instr(Mnemonic.mov, B, Mr4_8, Mr0_10))),
                (0xCA, Fetch8(Instr(Mnemonic.mov, B, Mr4_8, Mr0_10))),
                (0xCB, Fetch8(Instr(Mnemonic.mov, B, R0, Mr4_10))),
                (0xCC, Fetch8(Instr(Mnemonic.mov, B, Mr4_8, R0))),
                (0xCD, Fetch8(Instr(Mnemonic.mov, B, Mr4_8, R0))),
                (0xCE, Fetch8(Instr(Mnemonic.mov, B, Mr4_8, R0))),
                (0xCF, Fetch8(Instr(Mnemonic.mov, B, R4, R0))),

                // D0
                (0xD0, Fetch8(Instr(Mnemonic.mov, W, Mr4_8, Mr0_10))),
                (0xD1, Fetch8(Instr(Mnemonic.mov, W, Mr4_8, Mr0_10))),
                (0xD2, Fetch8(Instr(Mnemonic.mov, W, Mr4_8, Mr0_10))),
                (0xD3, Fetch8(Instr(Mnemonic.mov, W, R0, Mr4_10))),
                (0xD4, Fetch8(Instr(Mnemonic.mov, W, Mr4_8, Mr0_10))),
                (0xD5, Fetch8(Instr(Mnemonic.mov, W, Mr4_8, Mr0_10))),
                (0xD6, Fetch8(Instr(Mnemonic.mov, W, Mr4_8, Mr0_10))),
                (0xD7, Fetch8(Instr(Mnemonic.mov, W, R0, Mr4_10))),
                (0xD8, Fetch8(Instr(Mnemonic.mov, W, Mr4_8, Mr0_10))),
                (0xD9, Fetch8(Instr(Mnemonic.mov, W, Mr4_8, Mr0_10))),
                (0xDA, Fetch8(Instr(Mnemonic.mov, W, Mr4_8, Mr0_10))),
                (0xDB, Fetch8(Instr(Mnemonic.mov, W, R0, Mr4_10))),
                (0xDC, Fetch8(Instr(Mnemonic.mov, W, Mr4_8, R0))),
                (0xDD, Fetch8(Instr(Mnemonic.mov, W, Mr4_8, R0))),
                (0xDE, Fetch8(Instr(Mnemonic.mov, W, Mr4_8, R0))),
                (0xDF, Fetch8(Instr(Mnemonic.mov, W, R4, R0))),

                // E0
                (0xE0, Fetch8(Instr(Mnemonic.mov, L, Mr4_8, Mr0_10))),
                (0xE1, Fetch8(Instr(Mnemonic.mov, L, Mr4_8, Mr0_10))),
                (0xE2, Fetch8(Instr(Mnemonic.mov, L, Mr4_8, Mr0_10))),
                (0xE3, Fetch8(Instr(Mnemonic.mov, L, R0, Mr4_10))),
                (0xE4, Fetch8(Instr(Mnemonic.mov, L, Mr4_8, Mr0_10))),
                (0xE5, Fetch8(Instr(Mnemonic.mov, L, Mr4_8, Mr0_10))),
                (0xE6, Fetch8(Instr(Mnemonic.mov, L, Mr4_8, Mr0_10))),
                (0xE7, Fetch8(Instr(Mnemonic.mov, L, R0, Mr4_10))),
                (0xE8, Fetch8(Instr(Mnemonic.mov, L, Mr4_8, Mr0_10))),
                (0xE9, Fetch8(Instr(Mnemonic.mov, L, Mr4_8, Mr0_10))),
                (0xEA, Fetch8(Instr(Mnemonic.mov, L, Mr4_8, Mr0_10))),
                (0xEB, Fetch8(Instr(Mnemonic.mov, L, R0, Mr4_10))),
                (0xEC, Fetch8(Instr(Mnemonic.mov, L, Mr4_8, R0))),
                (0xED, Fetch8(Instr(Mnemonic.mov, L, Mr4_8, R0))),
                (0xEE, Fetch8(Instr(Mnemonic.mov, L, Mr4_8, R0))),
                (0xEF, Fetch8(Instr(Mnemonic.mov, L, R4, R0))),
    
                // F0
                (0xF0, bclr_imm3),
                (0xF1, bclr_imm3),
                (0xF2, bclr_imm3),
                (0xF3, bclr_imm3),
                (0xF4, btst_imm3),
                (0xF5, btst_imm3),
                (0xF6, btst_imm3),
                (0xF7, btst_imm3),
                (0xF8, movSimmMem),
                (0xF9, movSimmMem),
                (0xFA, movSimmMem),
                (0xFB, Fetch8(Mask(0,2, "  FB",
                    nyi,
                    nyi,
                    Instr(Mnemonic.mov, Simm2, R4),
                    nyi))),
                (0xFC, decodeFC),
                (0xFD, decodeFD),
                (0xFE, decodeFE),
                (0xFF, decodeFF));
        }

        /*0
         FSQRT Floating-point square root
FTOU Floating point to integer conversion
UTOF Integer to floating-point conversion
Data transfer
instructions
MOVCO Storing with LI flag clear
MOVLI Loading with LI flag set
DSP instructions EMACA Extend multiply-accumulate to the accumulator
EMSBA Extended multiply-subtract to the accumulator
EMULA Extended multiply to the accumulator
MACLH Multiply-Accumulate the lower-order word and higher-order word
MSBHI Multiply-Subtract the higher-order word
MSBLH Multiply-Subtract the lower-order word and higher-order word
MSBLO Multiply-Subtract the lower-order word
MULLH Multiply the lower-order word and higher-order word
MVFACGU Move the guard longword from the accumulator
MVFACLO Move the lower-order longword from the accumulator
MVTACGU Move the guard longword to the accumulator
RACL Round the accumulator longword
RDACL Round the accumulator longword
RDACW Round the accumulator word
F*/

    }
}
