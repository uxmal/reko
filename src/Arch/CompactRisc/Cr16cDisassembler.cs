#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using System.Collections.Generic;

namespace Reko.Arch.CompactRisc
{
    using Decoder = Decoder<Cr16cDisassembler, Mnemonic, Cr16Instruction>;

    public class Cr16cDisassembler : DisassemblerBase<Cr16Instruction, Mnemonic>
    {
        private static readonly Decoder rootDecoder;

        private readonly Cr16Architecture arch;
        private readonly EndianImageReader rdr;
        private List<MachineOperand> ops;
        private Address addr;

        public Cr16cDisassembler(Cr16Architecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.ops = new List<MachineOperand>();
            this.addr = rdr.Address;
        }

        public override Cr16Instruction? DisassembleInstruction()
        {
            if (!rdr.TryReadLeUInt16(out ushort uInstr))
                return null;
            var instr = rootDecoder.Decode(uInstr, this);
            var addrNew = rdr.Address;
            instr.Address = this.addr;
            instr.Length = (int) (addrNew - this.addr);
            this.addr = addrNew;
            this.ops.Clear();
            return instr;
        }

        public override Cr16Instruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            return new Cr16Instruction
            {
                InstructionClass = iclass,
                Mnemonic = mnemonic,
                Operands = ops.ToArray()
            };
        }

        public override Cr16Instruction CreateInvalidInstruction()
        {
            return new Cr16Instruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.Invalid,
                Operands = MachineInstruction.NoOperands,
            };
        }

        public override Cr16Instruction NotYetImplemented(string message)
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingDecoder("Cr16Dasm", this.addr, this.rdr, message);
            return CreateInvalidInstruction();
        }

        #region Mutator

        private static Mutator<Cr16cDisassembler> Imm(int bitpos)
        {
            var immField = new Bitfield(bitpos, 4);
            return (u, d) =>
            {
                var imm = (ushort) immField.Read(u);
                if (imm == 9)
                {
                    imm = 0xFFFF;
                }
                else if (imm == 11)
                {
                    if (!d.rdr.TryReadLeUInt16(out imm))
                        return false;
                }
                d.ops.Add(ImmediateOperand.Word16(imm));
                return true;
            };
        }

        private static readonly Mutator<Cr16cDisassembler> Imm4 = Imm(4);

        private static Mutator<Cr16cDisassembler> Imm20(int bitpos)
        {
            var immField = new Bitfield(bitpos, 4);
            return (u, d) =>
            {
                var high = immField.Read(u);
                if (!d.rdr.TryReadLeUInt16(out var low))
                    return false;
                var imm = (high << 16) | low;
                d.ops.Add(new ImmediateOperand(Constant.Create(Cr16Architecture.Word24, imm)));
                return true;
            };
        }
        private static readonly Mutator<Cr16cDisassembler> Imm20_0 = Imm20(0);

        private static Mutator<Cr16cDisassembler> Reg(int bitpos)
        {
            var regfield = new Bitfield(bitpos, 4);
            return (u, d) =>
            {
                var ireg = regfield.Read(u);
                d.ops.Add(Registers.GpRegisters[ireg]);
                return true;
            };
        }

        private static readonly Mutator<Cr16cDisassembler> R0 = Reg(0);
        private static readonly Mutator<Cr16cDisassembler> R4 = Reg(4);

        private static Mutator<Cr16cDisassembler> Cache(CacheFlag cacheFlag)
        {
            var cop = new CacheFlagOperand(cacheFlag);
            return (u, d) =>
            {
                d.ops.Add(cop);
                return true;
            };
        }

        #endregion


        #region Decoders

        private class Next32Decoder : Decoder<Cr16cDisassembler, Mnemonic, Cr16Instruction>
        {
            public readonly Decoder<Cr16cDisassembler, Mnemonic, Cr16Instruction> decoder;

            public Next32Decoder(Decoder<Cr16cDisassembler, Mnemonic, Cr16Instruction> decoder)
            {
                this.decoder = decoder;
            }

            public override Cr16Instruction Decode(uint wInstr, Cr16cDisassembler dasm)
            {
                if (!dasm.rdr.TryReadLeUInt32(out uint uInstr))
                    return dasm.CreateInvalidInstruction();
                return decoder.Decode(uInstr, dasm);
            }
        }

        private class MemAccessDecoder : Decoder<Cr16cDisassembler, Mnemonic, Cr16Instruction>
        {
            private readonly Mnemonic mnemonic;

            public MemAccessDecoder(Mnemonic mnemonic)
            {
                this.mnemonic = mnemonic;
            }

            public override Cr16Instruction Decode(uint wInstr, Cr16cDisassembler dasm)
            {
                var op = (wInstr >> 8) & 0xF;
                if (op == 0xE)
                    throw new System.Exception();
                else if (op == 0xF)
                    throw new System.Exception();
                else
                {
                    throw new System.Exception();
                }
                return dasm.MakeInstruction(InstrClass.Linear, mnemonic);
                
                /*
                1001 1110 xxxx xxxx  Fmt18 1 ZZ ope 0  loadw (prp) disp0 4 src (prp) 4 dest reg 4E 
                1001 1111 xxxx xxxx  Fmt19 2 ZZ ope 0  loadw (rp) disp16 4 src (rp)  4 dest reg 4F 16 src disp 
                1001 xxxx xxxx xxxx  Fmt18 1 ZZ        loadw (rp) disp4  4 src (rp)  4 dest reg 4  src disp*2 
*/
            }
        }

        private static Decoder Next32(Decoder<Cr16cDisassembler, Mnemonic, Cr16Instruction> decoder)
        {
            return new Next32Decoder(decoder);
        }

        private static Decoder MemAccess(Mnemonic mnemonic)
        {
            return new MemAccessDecoder(mnemonic);
        }

        private static Decoder Nyi(string message)
        {
            return new NyiDecoder<Cr16cDisassembler, Mnemonic, Cr16Instruction>(message);
        }

        #endregion

        private static Decoder<Cr16cDisassembler, Mnemonic, Cr16Instruction> Instr(
            Mnemonic mnemonic,
            params Mutator<Cr16cDisassembler>[] mutators)
        {
            return Instr(mnemonic, InstrClass.Linear, mutators);
        }

        private static Decoder<Cr16cDisassembler, Mnemonic, Cr16Instruction> Instr(
            Mnemonic mnemonic,
            InstrClass iclass,
            params Mutator<Cr16cDisassembler>[] mutators)
        {
            return new InstrDecoder<Cr16cDisassembler, Mnemonic, Cr16Instruction>(
                iclass,
                mnemonic,
                mutators);
        }

        private static Decoder<Cr16cDisassembler, Mnemonic, Cr16Instruction> param0(
            Mnemonic mnemonic,
            InstrClass iclass = InstrClass.Linear)
        {
            return new InstrDecoder<Cr16cDisassembler, Mnemonic, Cr16Instruction>(iclass, mnemonic);
        }

        static Cr16cDisassembler()
        {
            var invalid = Instr(Mnemonic.res, InstrClass.Invalid);

            var decode000 = Mask(0, 4, "  000",
                param0(Mnemonic.res, InstrClass.Invalid),
                param0(Mnemonic.res, InstrClass.Invalid),
                param0(Mnemonic.res, InstrClass.Invalid),
                param0(Mnemonic.retx, InstrClass.Transfer | InstrClass.Return),

                param0(Mnemonic.di, InstrClass.Invalid),
                param0(Mnemonic.ei, InstrClass.Invalid),
                param0(Mnemonic.wait, InstrClass.Invalid),
                param0(Mnemonic.eiwait, InstrClass.Invalid),

                param0(Mnemonic.res, InstrClass.Invalid),
                param0(Mnemonic.res, InstrClass.Invalid),
                Instr(Mnemonic.cinv, Cache(CacheFlag.I)),
                Instr(Mnemonic.cinv, Cache(CacheFlag.I | CacheFlag.U)),

                Instr(Mnemonic.cinv, Cache(CacheFlag.D)),
                Instr(Mnemonic.cinv, Cache(CacheFlag.D | CacheFlag.U)),
                Instr(Mnemonic.cinv, Cache(CacheFlag.D | CacheFlag.I)),
                Instr(Mnemonic.cinv, Cache(CacheFlag.D | CacheFlag.I | CacheFlag.U)));

            var decode0010 = Mask(12, 4, "  0010",
                    Nyi("Fmr3a 3 bra cond disp24 24 dest disp*2 4 cond imm 4 ope 0"),
                    Nyi("Fmt3 3 ZZ ope 1  res - no operation 4"),
                    Nyi("Fmr3a 3 bal (rp) disp24 24 dest disp*2 4 link rp  4 ope 2"),
                    Nyi("Fmt3 3 ZZ ope 3  res - no operation 4"),

                    Nyi("Fmt2 3 ZZ ope 4  cbitb (reg) disp20 4 dest (reg) 3 pos imm 20 dest disp 4"),
                    Nyi("Fmt2 3 ZZ ope 5  cbitb (rp) disp20 4 dest (rp) 3 pos imm 20 dest disp 4"),
                    Nyi("Fmt2 3 ZZ ope 6  cbitb (rrp) disp20 4 dest (rrp) 3 pos imm 20 dest disp 4"),
                    Nyi("Fmt3 3 ZZ ope 7  cbitb abs24 24 dest abs 3 pos imm 4"),

                    Nyi("Fmt2 3 ZZ ope 8  sbitb (reg) disp20 4 dest (reg) 3 pos imm 20 dest disp 4"),
                    Nyi("Fmt2 3 ZZ ope 9  sbitb (rp) disp20 4 dest (rp) 3 pos imm 20 dest disp 4"),
                    Nyi("Fmt2 3 ZZ ope 10  sbitb (rrp) disp20 4 dest (rrp) 3 pos imm 20 dest disp 4"),
                    Nyi("Fmt3 3 ZZ ope 11  sbitb abs24 24 dest abs 3 pos imm 4"),

                    Nyi("Fmt2 3 ZZ ope 12  tbitb (reg) disp20 4 dest (reg) 3 pos imm 20 dest disp 4"),
                    Nyi("Fmt2 3 ZZ ope 13  tbitb (rp) disp20 4 dest (rp) 3 pos imm 20 dest disp 4"),
                    Nyi("Fmt2 3 ZZ ope 14  tbitb (rrp) disp20 4 dest (rrp) 3 pos imm 20 dest disp 4"),
                    Nyi("Fmt3 3 ZZ ope 15  tbitb abs24 24 dest abs 3 pos imm 4"));

            var decode0011 = Mask(12, 4, "  0011",
                Nyi("Fmt3 3 ZZ ope 0  res - no operation 4"),
                Nyi("Fmt3 3 ZZ ope 1  res - no operation 4"),
                Nyi("Fmt3 3 ZZ ope 2  res - no operation 4"),
                Nyi("Fmt3 3 ZZ ope 3  res - no operation 4"),
                Nyi("Fmt2 3 ZZ ope 4  cbitw(reg) disp20 4 dest(reg) 4 pos imm 20 dest disp 4"),
                Nyi("Fmt2 3 ZZ ope 5  cbitw(rp) disp20 4 dest(rp) 4 pos imm 20 dest disp 4"),
                Nyi("Fmt2 3 ZZ ope 6  cbitw(rrp) disp20 4 dest(rrp) 4 pos imm 20 dest disp 4"),
                Nyi("Fmt3 3 ZZ ope 7  cbitw abs24 24 dest abs 4 pos imm 4"),
                Nyi("Fmt2 3 ZZ ope 8  sbitw(reg) disp20 4 dest(reg) 4 pos imm 20 dest disp 4"),
                Nyi("Fmt2 3 ZZ ope 9  sbitw(rp) disp20 4 dest(rp) 4 pos imm 20 dest disp 4"),
                Nyi("Fmt2 3 ZZ ope 10  sbitw(rrp) disp20 4 dest(rrp) 4 pos imm 20 dest disp 4"),
                Nyi("Fmt3 3 ZZ ope 11  sbitw abs24 24 dest abs 4 pos imm 4"),
                Nyi("Fmt2 3 ZZ ope 12  tbitw(reg) disp20 4 dest(reg) 4 pos imm 20 dest disp 4"),
                Nyi("Fmt2 3 ZZ ope 13  tbitw(rp) disp20 4 dest(rp) 4 pos imm 20 dest disp 4"),
                Nyi("Fmt2 3 ZZ ope 14  tbitw(rrp) disp20 4 dest(rrp) 4 pos imm 20 dest disp 4"),
                Nyi("Fmt3 3 ZZ ope 15  tbitw abs24 24 dest abs 4 pos imm 4"));

            var decode0012 = Mask(12, 4, "  0012",
                Nyi("Fmt2 3 ZZ ope 0  storb imm(reg) disp20 4 dest(reg) 4 src imm 20 dest disp 4"),
                Nyi("Fmt2 3 ZZ ope 1  storb imm(rp) disp20 4 dest(rp) 4 src imm 20 dest disp 4"),
                Nyi("Fmt2 3 ZZ ope 2  storb imm(rrp) disp20 4 dest(rrp) 4 src imm 20 dest disp 4"),
                Nyi("Fmt3 3 ZZ ope 3  storb imm abs24 24 dest abs 4 src imm 4"),
                Nyi("Fmt2 3 ZZ ope 4  loadb (reg) disp20 4 src (reg) 4 dest reg 20 src disp 4"),
                Nyi("Fmt2 3 ZZ ope 5  loadb (rp) disp20 4 src (rp) 4 dest reg 20 src disp 4"),
                Nyi("Fmt2 3 ZZ ope 6  loadb (rrp) disp20 4 src (rrp) 4 dest reg 20 src disp 4"),
                Nyi("Fmt3 3 ZZ ope 7  loadb abs24 24 src abs 4 dest reg 4"),
                Nyi("Fmt2 3 ZZ ope 8  loadd (reg) disp20 4 src (reg) 4 dest rp 20 src disp 4"),
                Nyi("Fmt2 3 ZZ ope 9  loadd (rp) disp20 4 src (rp) 4 dest rp 20 src disp 4"),
                Nyi("Fmt2 3 ZZ ope 10  loadd (rrp) disp20 4 src (rrp) 4 dest rp 20 src disp 4"),
                Nyi("Fmt3 3 ZZ ope 11  loadd abs24 24 src abs 4 dest rp 4"),
                Nyi("Fmt2 3 ZZ ope 12  loadw (reg) disp20 4 src (reg) 4 dest reg 20 src disp 4"),
                Nyi("Fmt2 3 ZZ ope 13  loadw (rp) disp20 4 src (rp) 4 dest reg 20 src disp 4"),
                Nyi("Fmt2 3 ZZ ope 14  loadw (rrp) disp20 4 src (rrp) 4 dest reg 20 src disp 4"),
                Nyi("Fmt3 3 ZZ ope 15  loadw abs24 24 src abs 4 dest reg 4"));

            var decode0013 = Mask(12, 4, "  0013",
                Nyi("Fmt2 3 ZZ ope 0  storw imm (reg) disp20 4 dest (reg) 4 src imm 20 dest disp 4"),
                Nyi("Fmt2 3 ZZ ope 1  storw imm (rp) disp20 4 dest (rp) 4 src imm 20 dest disp 4"),
                Nyi("Fmt2 3 ZZ ope 2  storw imm (rrp) disp20 4 dest (rrp) 4 src imm 20 dest disp 4"),
                Nyi("Fmt3 3 ZZ ope 3  storw imm abs24 24 dest abs 4 src imm 4"),
                Nyi("Fmt2 3 ZZ ope 4  storb (reg) disp20 4 dest (reg) 4 src reg 20 dest disp 4"),
                Nyi("Fmt2 2 ZZ ope 5  storb (rp) disp20 4 dest (rp) 4 src reg 20 dest disp 4"),
                Nyi("Fmt2 2 ZZ ope 6  storb (rrp) disp20 4 dest (rrp) 4 src reg 20 dest disp 4"),
                Nyi("Fmt3 3 ZZ ope 7  storb abs24 24 dest abs 4 src reg 4"),
                Nyi("Fmt2 3 ZZ ope 8  stord (reg) disp20 4 dest (reg) 4 src rp 20 dest disp 4"),
                Nyi("Fmt2 3 ZZ ope 9  stord (rp) disp20 4 dest (rp) 4 src rp 20 dest disp 4"),
                Nyi("Fmt2 3 ZZ ope 10  stord (rrp) disp20 4 dest (rrp) 4 src rp 20 dest disp 4"),
                Nyi("Fmt3 3 ZZ ope 11  stord abs24 24 dest abs 4 src rp 4"),
                Nyi("Fmt2 3 ZZ ope 12  storw (reg) disp20 4 dest (reg) 4 src reg 20 dest disp 4"),
                Nyi("Fmt2 3 ZZ ope 13  storw (rp) disp20 4 dest (rp) 4 src reg 20 dest disp 4"),
                Nyi("Fmt2 3 ZZ ope 14  storw (rrp) disp20 4 dest (rrp) 4 src reg 20 dest disp 4"),
                Nyi("Fmt3 3 ZZ ope 15  storw abs24 24 dest abs 4 src reg 4"));

            var decode0014 = Mask(12, 4, "  0014",
                Nyi("Fmt1 2 ZZ ope 0  lpr 4 src reg 4 dest pr 4 res 0 4"),
                Nyi("Fmt1 2 ZZ ope 1  lprd 4 src rp 4 dest prd 4 res 0 4"),
                Nyi("Fmt1 2 ZZ ope 2  spr 4 dest reg 4 src pr 4 res 0 4"),
                Nyi("Fmt1 2 ZZ ope 3  sprd 4 dest rp 4 src prd 4 res 0 4"),
                Nyi("Fmt1 2 ZZ ope 4  res - no operation 4"),
                Nyi("Fmt1 2 ZZ ope 5  res - no operation 4"),
                Nyi("Fmt1 2 ZZ ope 6  res - no operation 4"),
                Nyi("Fmt1 2 ZZ ope 7  res - no operation 4"),
                Nyi("Fmt1 2 ZZ ope 8  jal(rp, rp) 4 link rp 4 dest rp * 2 4 res 0 4"),
                Nyi("Fmt1 2 ZZ ope 9  ord rp, rp 4 dest rp 4 src rp 4"),
                Nyi("Fmt1 2 ZZ ope 10  xord rp, rp 4 dest rp 4 src rp 4"),
                Nyi("Fmt1 2 ZZ ope 11  andd rp, rp 4 dest rp 4 src rp 4"),
                Nyi("Fmt1 2 ZZ ope 12  subd rp, rp 4 dest rp 4 src rp 4 res 0 4"),
                Nyi("Fmt1 2 ZZ ope 13  macqw 4 src2 reg 4 src1 reg 4 dest rp 4"),
                Nyi("Fmt1 2 ZZ ope 14  macuw 4 src2 reg 4 src1 reg 4 dest rp 4"),
                Nyi("Fmt1 2 ZZ ope 15  macsw 4 src2 reg 4 src1 reg 4 dest rp 4"));

            var decode0018 = Sparse(12, 4, "  0018", invalid,
                (4, Nyi("loadb(reg) - disp20 4 src(reg) 4 dest reg 20 src disp 4")),
                (5, Nyi("loadb(rp) - disp20 4 src(rp) 4 dest reg 20 src disp 4")),
                (8, Nyi("loadd(reg) - disp20 4 src(reg) 4 dest rp 20 src disp 4")),
                (9, Nyi("loadd(rp) - disp20 4 src(rp) 4 dest rp 20 src disp 4")),
                (12, Nyi(" loadw(reg) - disp20 4 src(reg) 4 dest reg 20 src disp 4")),
                (13, Nyi(" loadw(rp) - disp20 4 src(rp) 4 dest reg 20 src disp 4")));

            var decode0019 = Sparse(12, 4, "  0019", invalid,
                (4, Nyi("storb(reg) - disp20 4 dest(reg) 4 src reg 20 dest disp 4")),
                (5, Nyi("storb(rp) - disp20 4 dest(rp) 4 src reg 20 dest disp 4")),
                (8, Nyi("stord(reg) - disp20 4 dest(reg) 4 src rp 20 dest disp 4")),
                (9, Nyi("stord(rp) - disp20 4 dest(rp) 4 src rp 20 dest disp 4")),
                (12, Nyi("storw(reg) - disp20 4 dest(reg) 4 src reg 20 dest disp 4")),
                (13, Nyi("storw(rp) - disp20 4 dest(rp) 4 src reg 20 dest disp 4")));

            var decode001 = Mask(0, 4, "  001",
                Next32(decode0010),
                Next32(decode0011),
                Next32(decode0012),
                Next32(decode0013),

                decode0014,
                invalid,
                invalid,
                invalid,

                decode0018,
                decode0019,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid);

            var decode00 = Mask(4, 4, "  00",
                decode000,
                decode001,
                Nyi("Fmt23 3 ZZ  addd imm32, rp 4 dest rp 32 src imm"),
                Nyi("Fmt23 3 ZZ  subd imm32,rp 4 dest rp 32 src imm"),

                Nyi("Fmt23 3 ZZ  andd imm32,rp 4 dest rp 32 src imm"),
                Nyi("Fmt23 3 ZZ  ord imm32,rp 4 dest rp 32 src imm"),
                Nyi("Fmt23 3 ZZ  xord imm32,rp 4 dest rp 32 src imm"),
                Nyi("Fmt23 3 ZZ  movd imm32 4 dest imm 32 src rp"),

                Nyi("Fmt23 3 ZZ  res - undefined trap for coprocessor inst 4 ci imm 32 cinst imm"),
                Nyi("Fmt23 3 ZZ  cmpd imm32 4 dest rp 32 src imm"),
                Mask(3, 1, " 00A",
                    Nyi("Fmt6 1 ZZ  loadm (reg) 3 count imm"),
                    Nyi("Fmt6 1 ZZ  loadmp (rp) 3 count imm")),
                Mask(3, 1, " 00B",
                    Nyi("Fmt6 1 ZZ  storm (reg) 3 count imm"),
                    Nyi("Fmt6 1 ZZ  stormp (rp) 3 count imm")),

                Nyi("Fmt11 1 ZZ  excp 4 vect imm"),
                Nyi("Fmt11 1 ZZ  jal (ra,rp) 4 dest rp*2"),
                invalid,
                invalid);
            var decode0 = Mask(8, 4,
                decode00,
                Nyi("Fmt14 1 ZZ  push 4 src reg 3 count imm 1 RA imm"),
                Nyi("Fmt14 1 ZZ  pop 4 dest reg 3 count imm 1 RA imm"),
                Nyi("Fmt14 1 ZZ  pop ret 4 dest reg 3 count imm 1 RA imm"),
                Nyi("Fmt12 2 ZZ  addd imm20, rp 20 src imm 4 dest rp"),
                Instr(Mnemonic.movd, Imm20_0, R4),
                Nyi("Fmt15 1 ZZ  tbit cnt 4 src reg 4 pos imm"),
                Nyi("Fmt15 1 ZZ  tbit reg, reg 4 src reg 4 pos reg"),

                Nyi("Fmt15 1 ZZ  Scond (reg) 4 dest reg 4 cond imm"),
                Mask(7, 1, "  09",
                    invalid, //        0000 1001 0xxx xxxx  ZZ res - undefined trap 2
                    Nyi("Fmt9 1 ZZ  lshb cnt(right -), reg 4 dest reg 3 count imm")),
                Nyi("Fmt15 1 ZZ  Jcondb (rp) 4 dest rp*2 4 cond imm"),
                Nyi("Fmt15 1 ZZ  mulsb reg,reg 4 dest reg 4 src reg"),

                Nyi("Fmt15 1 ZZ  beq0b disp4 (2-32) 4 src reg 4 dest disp*2+"),
                Nyi("Fmt15 1 ZZ  bne0b disp4 (2-32) 4 src reg 4 dest disp*2+"),
                Nyi("Fmt15 1 ZZ  beq0w disp4 (2-32) 4 src reg 4 dest disp*2+"),
                Nyi("Fmt15 1 ZZ  bne0w disp4 (2-32) 4 src reg 4 dest disp*2+"));

            var decode2 = Mask(8, 4,
                Nyi("ZZ andb imm4 / 16, reg 4 dest reg 4 src imm 15 / 16 1 / 2"),
                Nyi("Fmt15 1 ZZ  andb reg, reg 4 dest reg 4 src reg"),
                Nyi("ZZ andw imm4 / 16, reg 4 dest reg 4 src imm 15 / 16 1 / 2"),
                Nyi("Fmt15 1 ZZ  andw reg, reg 4 dest reg 4 src reg"),

                Nyi("ZZ orb imm4 / 16, reg 4 dest reg 4 src imm 15 / 16 1 / 2"),
                Nyi("Fmt15 1 ZZ  orb reg, reg 4 dest reg 4 src reg"),
                Nyi("ZZ orw imm4 / 16, reg 4 dest reg 4 src imm 15 / 16 1 / 2"),
                Nyi("Fmt15 1 ZZ  orw reg, reg 4 dest reg 4 src reg"),

                Nyi("ZZ xorb imm4 / 16, reg 4 dest reg 4 src imm 15 / 16 1 / 2"),
                Nyi("Fmt15 1 ZZ  xorb reg, reg 4 dest reg 4 src reg"),
                Nyi("ZZ xorw imm4 / 16, reg 4 dest reg 4 src imm 15 / 16 1 / 2"),
                Nyi("Fmt15 1 ZZ  xorw reg, reg 4 dest reg 4 src reg"),

                Nyi("ZZ addub imm4 / 16, reg 4 dest reg 4 src imm 15 / 16 1 / 2"),
                Nyi("Fmt15 1 ZZ  addub reg, reg 4 dest reg 4 src reg"),
                Nyi("ZZ adduw imm4 / 16, reg 4 dest reg 4 src imm 15 / 16 1 / 2"),
                Nyi("Fmt15 1 ZZ  adduw reg, reg 4 dest reg 4 src reg"));

            var decode3 = Mask(8, 4, "  03",
                Nyi("ZZ addb imm4/16,reg 4 dest reg 4 src imm 15/16 1/2"),
                Nyi("Fmt15 1 ZZ  addb reg, reg 4 dest reg 4 src reg"),
                Nyi("ZZ addw imm4 / 16, reg 4 dest reg 4 src imm 15 / 16 1 / 2"),
                Nyi("Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg"),

                Nyi("ZZ addcb imm4 / 16, reg 4 dest reg 4 src imm 15 / 16 1 / 2"),
                Nyi("Fmt15 1 ZZ  addcb reg, reg 4 dest reg 4 src reg"),
                Nyi("ZZ addcw imm4 / 16, reg 4 dest reg 4 src imm 15 / 16 1 / 2"),
                Nyi("Fmt15 1 ZZ  addcw reg, reg 4 dest reg 4 src reg"),

                Nyi("ZZ subb imm4 / 16, reg 4 dest reg 4 src imm 15 / 16 1 / 2"),
                Nyi("Fmt15 1 ZZ  subb reg, reg 4 dest reg 4 src reg"),
                Nyi("ZZ subw imm4 / 16, reg 4 dest reg 4 src imm 15 / 16 1 / 2"),
                Nyi("Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg"),

                Nyi("ZZ subcb imm4 / 16, reg 4 dest reg 4 src imm 15 / 16 1 / 2"),
                Nyi("Fmt15 1 ZZ  subcb reg, reg 4 dest reg 4 src reg"),
                Nyi("ZZ subcw imm4 / 16, reg 4 dest reg 4 src imm 15 / 16 1 / 2"),
                Nyi("Fmt15 1 ZZ  subcw reg, reg 4 dest reg 4 src reg"));

            var decode4 = Mask(8, 4, "  4",
                Mask(7, 1, "  40",
                    Nyi("0100 0000 0xxx xxxx  Fmt9 1 ZZ  ashub cnt(left +), reg 4 dest reg 3 count imm"),
                    Nyi("0100 0000 1xxx xxxx  Fmt9 1 ZZ  ashub cnt(right -), reg 4 dest reg 3 count imm")),
                Nyi("0100 0001 xxxx xxxx  Fmt15 1 ZZ  ashub reg, reg 4 dest reg 4 count reg"),
                Nyi("0100 0010 xxxx xxxx  Fmt15 1 ZZ  ashuw cnt(left +), reg 4 dest reg 4 count imm"),
                Nyi("0100 0011 xxxx xxxx  Fmt15 1 ZZ  ashuw cnt(right -), reg 4 dest reg 4 count imm"),

                Nyi("0100 0100 xxxx xxxx  Fmt15 1 ZZ  lshb reg, reg 4 dest reg 4 count reg"),
                Nyi("0100 0101 xxxx xxxx  Fmt15 1 ZZ  ashuw reg, reg 4 dest reg 4 count reg"),
                Nyi("0100 0110 xxxx xxxx  Fmt15 1 ZZ  lshw reg, reg 4 dest reg 4 count reg"),
                Nyi("0100 0111 xxxx xxxx  Fmt15 1 ZZ  lshd reg, rp 4 dest rp 4 count reg"),

                Nyi("0100 1000 xxxx xxxx  Fmt15 1 ZZ  ashud reg, rp 4 dest rp 4 count reg"),
                Nyi("0100 1001 xxxx xxxx  Fmt15 1 ZZ  lshw cnt(right -), reg 4 dest reg 4 count imm"),
                Nyi("0100 101x xxxx xxxx  Fmt20 1 ZZ  lshd cnt(right -), rp 4 dest rp 5 count imm"),
                Nyi("0100 101x xxxx xxxx  Fmt20 1 ZZ  lshd cnt(right -), rp 4 dest rp 5 count imm"),

                Nyi("0100 110x xxxx xxxx  Fmt20 1 ZZ  ashud cnt(left +), rp 4 dest rp 5 count imm"),
                Nyi("0100 110x xxxx xxxx  Fmt20 1 ZZ  ashud cnt(left +), rp 4 dest rp 5 count imm"),
                Nyi("0100 111x xxxx xxxx  Fmt20 1 ZZ  ashud cnt(right -), rp 4 dest rp 5 count imm"),
                Nyi("0100 111x xxxx xxxx  Fmt20 1 ZZ  ashud cnt(right -), rp 4 dest rp 5 count imm"));
            var decode5 = Mask(8, 4, "  5",
                Nyi("ZZ cmpb imm4 / 16, reg 4 src reg 4 src imm 15 / 16 1 / 2"),
                Nyi("Fmt15 1 ZZ  cmpb reg, reg 4 src reg 4 src reg"),
                Nyi("ZZ cmpw imm4 / 16, reg 4 src reg 4 src imm 15 / 16 1 / 2"),
                Nyi("Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg"),

                Nyi("ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2"),
                Nyi("Fmt15 1 ZZ  movd rp, rp 4 dest rp 4 src rp"),
                Nyi("ZZ cmpd imm4 / 16, rp 4 src rp 4 src imm 15 / 16 1 / 2"),
                Nyi("Fmt15 1 ZZ  cmpd rp, rp 4 src rp 4 src rp"),

                Instr(Mnemonic.movb, Imm4, R0),
                Instr(Mnemonic.movb, R4, R0),
                Nyi("ZZ movw imm4 / 16, reg 4 dest reg 4 src imm 15 / 16 1 / 2"),
                Nyi("Fmt15 1 ZZ  movw reg, reg 4 dest reg 4 src reg"),

                Nyi("Fmt15 1 ZZ  movxb 4 dest reg 4 src reg"),
                Nyi("Fmt15 1 ZZ  movzb 4 dest reg 4 src reg"),
                Nyi("Fmt15 1 ZZ  movxw 4 dest rp 4 src reg"),
                Nyi("Fmt15 1 ZZ  movzw 4 dest rp 4 src reg"));
            var decode6 = Mask(8, 4, "  6",
                Nyi("ZZ addd imm4 / 16, rp 4 dest rp 4 src imm 15 / 16 1 / 2"),
                Nyi("Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp"),
                Nyi("Fmt15 1 ZZ  mulsw reg, rp 4 dest rp 4 src reg"),
                Nyi("Fmt15 1 ZZ  muluw reg, rp 4 dest rp 4 src reg"),

                Nyi("ZZ mulb imm4 / 16, reg 4 dest reg 4 src imm 15 / 16 1 / 2"),
                Nyi("Fmt15 1 ZZ  mulb reg, reg 4 dest reg 4 src reg"),
                Nyi("ZZ mulw imm4 / 16, reg 4 dest reg 4 src imm 15 / 16 1 / 2"),
                Nyi("Fmt15 1 ZZ  mulw reg, reg 4 dest reg 4 src reg"),

                Nyi("Fmt8 2 ZZ  cbitb abs20 rel 20 dest abs 3 pos imm 1 dest rs"),
                Nyi("Fmt16 2 ZZ  cbitw(rp) disp16 4 dest(rp) 4 pos imm 16 dest disp"),
                Mask(6, 2, "  6A",
                    Nyi("Fmt9 1 ZZ  cbitb(rp) disp0 4 dest(rp) 3 pos imm"),
                    Nyi("Fmt9 1 ZZ  cbitb(rp) disp0 4 dest(rp) 3 pos imm"),
                    Nyi("Fmt17 2 ZZ  cbitb(prp) disp14 4 dest(prp) 3 pos imm 14 dest disp"),
                    Nyi("Fmt17 2 ZZ  cbitw(prp) disp14 4 dest(prp) 4 pos imm 14 dest disp")),
                Mask(7, 1, "  6B",
                    Nyi("Fmt10 2 ZZ  cbitb(rp) disp16 4 dest(rp) 3 pos imm 16 dest disp"),
                    Nyi("Fmt7 2 ZZ  cbitb abs20 20 dest abs 3 pos imm")),
                Nyi("Fmt13 2 ZZ  cbitw abs20 rel 20 dest abs 4 pos imm 1 dest rs"),
                Nyi("Fmt13 2 ZZ  cbitw abs20 rel 20 dest abs 4 pos imm 1 dest rs"),
                Nyi("Fmt15 1 ZZ  cbitw(rp) disp0 4 dest(rp) 4 pos imm"),
                Nyi("Fmt12 2 ZZ  cbitw abs20 20 dest abs 4 pos imm"));

            var decode7 = Mask(8, 4, "  7",
                Nyi("Fmt8 2 ZZ  sbitb abs20 rel 20 dest abs 3 pos imm 1 dest rs"),
                Nyi("Fmt16 2 ZZ  sbitw(rp) disp16 4 dest(rp) 4 pos imm 16 dest disp"),
                Mask(6, 2, "  72",
                    Nyi("Fmt9 1 ZZ  sbitb(rp) disp0 4 dest(rp) 3 pos imm"),
                    Nyi("Fmt9 1 ZZ  sbitb(rp) disp0 4 dest(rp) 3 pos imm"),
                    Nyi("Fmt17 2 ZZ  sbitb(prp) disp14 4 dest(prp) 3 pos imm 14 dest disp"),
                    Nyi("Fmt17 2 ZZ  sbitw(prp) disp14 4 dest(prp) 4 pos imm 14 dest disp")),
                Mask(7, 1, "  73",
                    Nyi("Fmt10 2 ZZ  sbitb(rp) disp16 4 dest(rp) 3 pos imm 16 dest disp"),
                    Nyi("Fmt7 2 ZZ  sbitb abs20 20 dest abs 3 pos imm")),

                Nyi("Fmt13 2 ZZ  sbitw abs20 rel 20 dest abs 4 pos imm 1 dest rs"),
                Nyi("Fmt13 2 ZZ  sbitw abs20 rel 20 dest abs 4 pos imm 1 dest rs"),
                Nyi("Fmt15 1 ZZ  sbitw(rp) disp0 4 dest(rp) 4 pos imm"),
                Nyi("Fmt12 2 ZZ  sbitw abs20 20 dest abs 4 pos imm"),

                Nyi("Fmt8 2 ZZ  tbitb abs20 rel 20 dest abs 3 pos imm 1 dest rs"),
                Nyi("Fmt16 2 ZZ  tbitw(rp) disp16 4 dest(rp) 4 pos imm 16 dest disp"),
                Mask(6, 2, "  7A",
                    Nyi("Fmt9 1 ZZ  tbitb(rp) disp0 4 dest(rp) 3 pos imm"),
                    Nyi("Fmt9 1 ZZ  tbitb(rp) disp0 4 dest(rp) 3 pos imm"),
                    Nyi("Fmt17 2 ZZ  tbitb(prp) disp14 4 dest(prp) 3 pos imm 14 dest disp"),
                    Nyi("Fmt17 2 ZZ  tbitw(prp) disp14 4 dest(prp) 4 pos imm 14 dest disp")),
                Mask(7, 1, "  7B",
                    Nyi("Fmt10 2 ZZ  tbitb(rp) disp16 4 dest(rp) 3 pos imm 16 dest disp"),
                    Nyi("Fmt7 2 ZZ  tbitb abs20 20 dest abs 3 pos imm")),
                Nyi("Fmt13 2 ZZ  tbitw abs20 rel 20 dest abs 4 pos imm 1 dest rs"),
                Nyi("Fmt13 2 ZZ  tbitw abs20 rel 20 dest abs 4 pos imm 1 dest rs"),
                Nyi("Fmt15 1 ZZ  tbitw(rp) disp0 4 dest(rp) 4 pos imm"),
                Nyi("Fmt12 2 ZZ  tbitw abs20 20 dest abs 4 pos imm"));

            var decode8 = Mask(8, 4, "  8",

                Nyi("Fmt15 1 ZZ  res - undefined trap"),
                Nyi("Fmt12 2 ZZ  storb imm abs20 20 dest abs 4 src imm"),
                Nyi("Fmt15 1 ZZ  storb imm(rp) disp0 4 dest(rp) 4 src imm"),
                Nyi("Fmt16 2 ZZ  storb imm(rp) disp16 4 dest(rp) 4 src imm 16 dest disp"),

                Nyi("Fmt13 2 ZZ  storb imm abs20 rel 20 dest abs 4 src imm 1 dest rs"),
                Nyi("Fmt13 2 ZZ  storb imm abs20 rel 20 dest abs 4 src imm 1 dest rs"),
                Mask(6, 2, "  86",
                    Nyi("Fmt17 2 ZZ  storb imm(prp) disp14 4 dest(prp) 4 src imm 14 dest disp"),
                    Nyi("Fmt17 2 ZZ  loadb(prp) disp14 4 src(prp) 4 dest reg 14 src disp"),
                    Nyi("Fmt17 2 ZZ  loadd(prp) disp14 4 src(prp) 4 dest rp 14 src disp"),
                    Nyi("Fmt17 2 ZZ  loadw(prp) disp14 4 src(prp) 4 dest reg 14 src disp")),
                Nyi("Fmt12 2 ZZ  loadd abs20 20 src abs 4 dest rp"),

                Nyi("Fmt12 2 ZZ  loadb abs20 20 src abs 4 dest reg"),
                Nyi("Fmt12 2 ZZ  loadw abs20 20 src abs 4 dest reg"),
                Nyi("Fmt13 2 ZZ  loadb abs20 rel 20 src abs 4 dest reg 1 src rs"),
                Nyi("Fmt13 2 ZZ  loadb abs20 rel 20 src abs 4 dest reg 1 src rs"),

                Nyi("Fmt13 2 ZZ  loadd abs20 rel 20 src abs 4 dest rp 1 src rs"),
                Nyi("Fmt13 2 ZZ  loadd abs20 rel 20 src abs 4 dest rp 1 src rs"),
                Nyi("Fmt13 2 ZZ  loadw abs20 rel 20 src abs 4 dest reg 1 src rs"),
                Nyi("Fmt13 2 ZZ  loadw abs20 rel 20 src abs 4 dest reg 1 src rs"));

            var decodeC = Mask(8, 4, "  C",
                Nyi("Fmt5  2 ZZ  bal(ra) disp24 23 dest disp * 2"),
                Nyi("Fmt12 2 ZZ  storw imm abs20 20 dest abs 4 src imm"),
                Nyi("Fmt15 1 ZZ  storw imm(rp) disp0 4 dest(rp) 4 src imm"),
                Nyi("Fmt16 2 ZZ  storw imm(rp) disp16 4 dest(rp) 4 src imm 16 dest disp"),

                Nyi("Fmt13 2 ZZ  storw imm abs20 rel 20 dest abs 4 src imm 1 dest rs"),
                Nyi("Fmt13 2 ZZ  storw imm abs20 rel 20 dest abs 4 src imm 1 dest rs"),
                Mask(6, 2, "  C6",
                    Nyi("Fmt17 2 ZZ  storw imm(prp) disp14 4 dest(prp) 4 src imm 14 dest disp"),
                    Nyi("Fmt17 2 ZZ  storb(prp) disp14 4 dest(prp) 4 src reg 14 dest disp"),
                    Nyi("Fmt17 2 ZZ  stord(prp) disp14 4 dest(prp) 4 src rp 14 dest disp"),
                    Nyi("Fmt17 2 ZZ  storw(prp) disp14 4 dest(prp) 4 src reg 14 dest disp")),
                Nyi("Fmt12 2 ZZ  stord abs20 20 dest abs 4 src rp"),
                Nyi("Fmt12 2 ZZ  storb abs20 20 dest abs 4 src reg"),
                Nyi("Fmt12 2 ZZ  storw abs20 20 dest abs 4 src reg"),
                Nyi("Fmt13 2 ZZ  storb abs20 rel 20 dest abs 4 src reg 1 dest rs"),
                Nyi("Fmt13 2 ZZ  storb abs20 rel 20 dest abs 4 src reg 1 dest rs"),
                Nyi("Fmt13 2 ZZ  stord abs20 rel 20 dest abs 4 src rp 1 dest rs"),
                Nyi("Fmt13 2 ZZ  stord abs20 rel 20 dest abs 4 src rp 1 dest rs"),
                Nyi("Fmt13 2 ZZ  storw abs20 rel 20 dest abs 4 src reg 1 dest rs"),
                Nyi("Fmt13 2 ZZ  storw abs20 rel 20 dest abs 4 src reg 1 dest rs"));

            rootDecoder = Mask(12, 4, "CR16C",
                decode0,
                Select(u => (u & 0xF0F) == 0x800,
                    Nyi("Fmt22 2 ZZ  bra cond disp16 4 cond imm 16 dest disp * 2"),
                    Nyi("Fmt21 1 ZZ  bra cond disp8 8 dest disp * 2 4 cond imm")),
                decode2,
                decode3,

                decode4,
                decode5,
                decode6,
                decode7,

                decode8,
                MemAccess(Mnemonic.loadw),
                MemAccess(Mnemonic.loadd),
                MemAccess(Mnemonic.loadb),

                decodeC,
                MemAccess(Mnemonic.storw),
                MemAccess(Mnemonic.stord),
                If(u => u != 0xFFFF, MemAccess(Mnemonic.storb)));
        }
        /*
        1001 1110 xxxx xxxx  Fmt18 1 ZZ ope 0  loadw (prp) disp0 4 src (prp) 4 dest reg 4E 
        1001 1111 xxxx xxxx  Fmt19 2 ZZ ope 0  loadw (rp) disp16 4 src (rp) 4 dest reg 4F 16 src disp 
        1001 xxxx xxxx xxxx  Fmt18 1 ZZ  loadw (rp) disp4 4 src (rp) 4 dest reg 4 src disp*2 

        1010 1110 xxxx xxxx  Fmt18 1 ZZ ope 0  loadd (prp) disp0 4 src (prp) 4 dest rp 4E 
        1010 1111 xxxx xxxx  Fmt19 2 ZZ ope 0  loadd (rp) disp16 4 src (rp) 4 dest rp 4F 16 src disp 
        1010 xxxx xxxx xxxx  Fmt18 1 ZZ  loadd (rp) disp4 4 src (rp) 4 dest rp 4 src disp*2 

        1011 1110 xxxx xxxx  Fmt18 1 ZZ ope 0  loadb (prp) disp0 4 src (prp) 4 dest reg 4E 
        1011 1111 xxxx xxxx  Fmt19 2 ZZ ope 0  loadb (rp) disp16 4 src (rp) 4 dest reg 4F 16 src disp 
        1011 xxxx xxxx xxxx  Fmt18 1 ZZ  loadb (rp) disp4 4 src (rp) 4 dest reg 4 src disp 

        1101 1110 xxxx xxxx  Fmt18 1 ZZ ope 0  storw (prp) disp0 4 dest (prp) 4 src reg 4E 
        1101 1111 xxxx xxxx  Fmt19 2 ZZ ope 0  storw (rp) disp16 4 dest (rp) 4 src reg 4F 16 dest disp 
        1101 xxxx xxxx xxxx  Fmt18 1 ZZ  storw (rp) disp4 4 dest (rp) 4 src reg 4 dest disp*2 

        1110 1110 xxxx xxxx  Fmt18 1 ZZ ope 0  stord (prp) disp0 4 dest (prp) 4 src rp 4E 
        1110 1111 xxxx xxxx  Fmt19 2 ZZ ope 0  stord (rp) disp16 4 dest (rp) 4 src rp 4F 16 dest disp 
        1110 xxxx xxxx xxxx  Fmt18 1 ZZ  stord (rp) disp4 4 dest (rp) 4 src rp 4 dest disp*2 

        1111 1110 xxxx xxxx  Fmt18 1 ZZ ope 0  storb (prp) disp0 4 dest (prp) 4 src reg 4E 
        1111 1111 xxxx xxxx  Fmt19 2 ZZ  storb (rp) disp16 4 dest (rp) 4 src reg 4 ope  0F 16 dest disp 
        1111 xxxx xxxx xxxx  Fmt18 1 ZZ  storb (rp) disp4a 4 dest (rp) 4 src reg 4 dest disp 
        1111 1111 1111 1111  Fmt1 2 ZZ  res - undefined trap 
        */
    }
}