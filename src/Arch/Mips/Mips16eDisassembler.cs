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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Reko.Arch.Mips
{
    using Decoder = Reko.Core.Machine.Decoder<Mips16eDisassembler, Mnemonic, MipsInstruction>;

    public class Mips16eDisassembler : DisassemblerBase<MipsInstruction, Mnemonic>
    {
#pragma warning disable IDE1006
        private static readonly Decoder rootDecoder;

        private readonly MipsArchitecture arch;
        private readonly EndianImageReader rdr;
        private readonly List<MachineOperand> ops;
        private Address addr;
        private uint? extend;

        public Mips16eDisassembler(MipsArchitecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.ops = new List<MachineOperand>();
        }

        public override MipsInstruction? DisassembleInstruction()
        {
            this.addr = rdr.Address;
            this.ops.Clear();
            this.extend = null;
            if (!rdr.TryReadUInt16(out ushort wInstr))
                return null;
            var instr = rootDecoder.Decode(wInstr, this);
            instr.Address = addr;
            instr.Length = (int) (rdr.Address - addr);
            return instr;
        }

        public override MipsInstruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            return new MipsInstruction
            {
                InstructionClass = iclass,
                Mnemonic = mnemonic,
                Operands = ops.ToArray()
            };
        }

        public override MipsInstruction NotYetImplemented(string message)
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingDecoder("Mips16eDis", this.addr, this.rdr, message);
            return CreateInvalidInstruction();
        }

        public override MipsInstruction CreateInvalidInstruction()
        {
            return new MipsInstruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.illegal,
                Operands = Array.Empty<MachineOperand>(),
            };
        }

        #region Bitfields

        private static readonly Bitfield bf0_8 = new Bitfield(0, 8);
        private static readonly Bitfield bf0_4 = new Bitfield(0, 4);
        private static readonly Bitfield bf0_5 = new Bitfield(0, 5);
        private static readonly Bitfield bf2_3 = new Bitfield(2, 3);
        private static readonly Bitfield bf4_4 = new Bitfield(4, 4);
        private static readonly Bitfield bf6_5 = new Bitfield(6, 5);
        private static readonly Bitfield bf8_3 = new Bitfield(8, 3);
        private static readonly Bitfield[] bf_jal = Bf((16, 5), (20, 5), (0, 16));
        private static readonly Bitfield[] bf_extend4 = Bf((0, 4), (4, 7));
        private static readonly Bitfield[] bf_extend5 = Bf((0, 5), (5, 6));
        #endregion

        private static readonly RegisterStorage[] registerEncoding = new uint[8] { 16, 17, 2, 3, 4, 5, 6, 7 }
            .Select(iReg => Registers.generalRegs[iReg])
            .ToArray();

        /// <summary>
        /// Decode a 3-bit register field.
        /// </summary>
        private static Mutator<Mips16eDisassembler> Reg(int bitpos)
        {
            var field = new Bitfield(bitpos, 3);
            return (u, d) =>
            {
                var encReg = field.Read(u);
                var reg = registerEncoding[encReg];
                d.ops.Add(reg);
                return true;
            };
        }
        private static readonly Mutator<Mips16eDisassembler> R0 = Reg(0);
        private static readonly Mutator<Mips16eDisassembler> R2 = Reg(2);
        private static readonly Mutator<Mips16eDisassembler> R5 = Reg(5);
        private static readonly Mutator<Mips16eDisassembler> R8 = Reg(8);

        private static Mutator<Mips16eDisassembler> Reg(params (int bitpos, int bitlen)[] fieldSpecs)
        {
            var bitfields = Bf(fieldSpecs);
            return (u, d) =>
            {
                var ireg = Bitfield.ReadFields(bitfields, u);
                var reg = d.arch.GeneralRegs[ireg];
                d.ops.Add(reg);
                return true;
            };
        }
        /// <summary>
        /// Stack register
        /// </summary>
        private static bool sp(uint uInstr, Mips16eDisassembler dasm)
        {
            dasm.ops.Add(dasm.arch.StackRegister);
            return true;
        }

        /// <summary>
        /// Program counter.
        /// </summary>
        private static bool pc(uint uInstr, Mips16eDisassembler dasm)
        {
            dasm.ops.Add(dasm.arch.pc);
            return true;
        }

        /// <summary>
        /// Return address register.
        /// </summary>
        private static bool ra(uint uInstr, Mips16eDisassembler dasm)
        {
            dasm.ops.Add(dasm.arch.GeneralRegs[31]);
            return true;
        }

        /// <summary>
        /// Decode a single bit corresponding to a register.
        /// </summary>
        private static bool MultiRegs(uint uInstr, Mips16eDisassembler dasm)
        {
            uint regMask = 0;
            if (Bits.IsBitSet(uInstr, 6))
                regMask |= (1u << 31); // ra
            if (Bits.IsBitSet(uInstr, 4))
                regMask |= (1u << 17);
            if (Bits.IsBitSet(uInstr, 5))
                regMask |= (1u << 16);
            var mop = new MultiRegisterOperand(dasm.arch.GeneralRegs, dasm.arch.WordWidth, regMask);
            dasm.ops.Add(mop);
            return true;
        }

        private static readonly uint[] aregsEncoding = new uint[16]
        {
            0,
            0b1000_0000,
            0b1100_0000,
            0b1110_0000,

            0,
            0b1000_0000,
            0b1100_0000,
            0b1110_0000,

            0,
            0b1000_0000,
            0b1100_0000,
            0b1111_0000,

            0,
            0b1000_0000,
            0,
            ~0u,        // Invalid
        };

        private static readonly uint[] xregsEncoding = new uint[8]
        {
            0,
            0x0004_0000,
            0x000C_0000,
            0x001C_0000,

            0x003C_0000,
            0x007C_0000,
            0x00FC_0000,
            0x40FC_0000,
        };

        private static bool MultiRegsEx(uint uInstr, Mips16eDisassembler dasm)
        {
            Debug.Assert(dasm.extend.HasValue);
            uint uExtend = dasm.extend.Value;
            uint aregs = aregsEncoding[bf0_4.Read(uExtend)];
            if (aregs == ~0u)
                return false;
            uint regMask = aregs | xregsEncoding[bf8_3.Read(uExtend)];
            if (Bits.IsBitSet(uInstr, 6))
                regMask |= (1u << 31); // ra
            if (Bits.IsBitSet(uInstr, 4))
                regMask |= (1u << 17);
            if (Bits.IsBitSet(uInstr, 5))
                regMask |= (1u << 16);
            var mop = new MultiRegisterOperand(dasm.arch.GeneralRegs, dasm.arch.WordWidth, regMask);
            dasm.ops.Add(mop);
            return true;
        }

        /// <summary>
        /// A shifted unsigned immediate.
        /// </summary>
        public static Mutator<Mips16eDisassembler> UImmSh(int bitpos, int bitlength, int shift)
        {
            var field = new Bitfield(bitpos, bitlength);
            return (u, d) =>
            {
                var uImm = field.Read(u) << shift;
                d.ops.Add(Constant.Word32(uImm));
                return true;
            };
        }
        public static readonly Mutator<Mips16eDisassembler> UImm4 = UImmSh(0, 4, 0);
        public static readonly Mutator<Mips16eDisassembler> UImm8 = UImmSh(0, 8, 0);
        public static readonly Mutator<Mips16eDisassembler> UImm8s2 = UImmSh(0, 8, 2);


        /// <summary>
        /// A unsigned immediate involving the extended bits and the 5 least significant bits.
        /// </summary>
        public static Mutator<Mips16eDisassembler> UImmEx(Bitfield[] extFields, int bitfieldLsb, int significantDigits)
        {
            var fieldLsb = new Bitfield(0, bitfieldLsb);
            return (u, d) =>
            {
                Debug.Assert(d.extend.HasValue);
                var uImm =
                    (Bitfield.ReadFields(extFields, d.extend.Value) << fieldLsb.Length) |
                    fieldLsb.Read(u);
                d.ops.Add(Constant.Word32(uImm));
                return true;
            };
        }
        private static readonly Mutator<Mips16eDisassembler> UImmEx5 = UImmEx(bf_extend5, 5, 16);


        /// <summary>
        /// A shifted signed immediate.
        /// </summary>
        public static Mutator<Mips16eDisassembler> SImmSh(int bitpos, int bitlength, int shift)
        {
            var field = new Bitfield(bitpos, bitlength);
            return (u, d) =>
            {
                var uImm = field.ReadSigned(u) << shift;
                d.ops.Add(Constant.Int32(uImm));
                return true;
            };
        }
        public static readonly Mutator<Mips16eDisassembler> SImm8s3 = SImmSh(0, 8, 3);


        /// <summary>
        /// A sign-extended immediate involving the extended bits and the 5 least significant bits.
        /// </summary>
        public static Mutator<Mips16eDisassembler> SImmEx(Bitfield[] extFields, int bitfieldLsb, int significantDigits)
        {
            var fieldLsb = new Bitfield(0, bitfieldLsb);
            return (u, d) =>
            {
                Debug.Assert(d.extend.HasValue);
                var uImm =
                    (Bitfield.ReadFields(extFields, d.extend.Value) << fieldLsb.Length) |
                    fieldLsb.Read(u);
                var sImm = Bits.SignExtend(uImm, significantDigits);
                d.ops.Add(Constant.Word32(sImm));
                return true;
            };
        }
        private static readonly Mutator<Mips16eDisassembler> SImmEx4 = SImmEx(bf_extend4, 4, 15);
        private static readonly Mutator<Mips16eDisassembler> SImmEx5 = SImmEx(bf_extend5, 5, 16);

        /// <summary>
        /// PC-Relative address from the 8 lsb
        /// </summary>
        public static bool AImm8(uint uInstr, Mips16eDisassembler dasm)
        {
            var addr = dasm.addr.Align(4) + (bf0_8.Read(uInstr) << 2);
            dasm.ops.Add(addr);
            return true;
        }

        private static bool AImmEx5(uint uInstr, Mips16eDisassembler dasm)
        {
            Debug.Assert(dasm.extend.HasValue);
            var uExt = ((Bitfield.ReadFields(bf_extend5, dasm.extend.Value) << bf0_5.Length | bf0_5.Read(uInstr)));
            var addr = dasm.addr.Align(4) + Bits.SignExtend(uExt, 16);
            dasm.ops.Add(addr);
            return true;
        }

        /// <summary>
        /// Shift amount
        /// </summary>
        private static bool ShAmt(uint uInstr, Mips16eDisassembler dasm)
        {
            var encShamt = bf2_3.Read(uInstr);
            var shAmt = shortShiftAmts[encShamt];
            dasm.ops.Add(shAmt);
            return true;
        }
        private static readonly Constant[] shortShiftAmts = new int[8] { 8, 1, 2, 3, 4, 5, 6, 7 }
            .Select(n => Constant.Int32(n))
            .ToArray();
        
        private static bool ShAmtEx(uint uInstr, Mips16eDisassembler dasm)
        {
            Debug.Assert(dasm.extend.HasValue);
            var shAmt = (int) bf6_5.Read(dasm.extend.Value);
            dasm.ops.Add(Constant.Int32(shAmt));
            return true;
        }

        /// <summary>
        /// Decode the frame size used in SAVE and RESTORE instructions.
        /// </summary>
        private static Mutator<Mips16eDisassembler> Framesize(Bitfield[] bitfields)
        {
            return (u, d) =>
            {
                var encFramesize = Bitfield.ReadFields(bitfields, u);
                var imm = frameSizeEncoding[encFramesize];
                d.ops.Add(imm);
                return true;
            };
        }
        private static readonly Mutator<Mips16eDisassembler> SaveFramesize = Framesize(Bf((0, 4)));

        private static readonly Constant[] frameSizeEncoding = new int[16]
        {
            16, 1, 2, 3,  4, 5, 6, 7,  8, 9, 10, 11,  12, 13, 14, 15
        }
            .Select(n => Constant.Int32(n * 8))
            .ToArray();

        private static bool SaveFramesizeEx(uint uInstr, Mips16eDisassembler dasm)
        {
            Debug.Assert(dasm.extend.HasValue);
            var hi = (int) bf4_4.Read(dasm.extend.Value);
            var lo = (int) bf0_4.Read(uInstr);
            var imm = Constant.Int32(((hi << 4) | lo) << 3);
            dasm.ops.Add(imm);
            return true;
        }

        /// <summary>
        /// PC-relative address.
        /// </summary>
        private static Mutator<Mips16eDisassembler> ARel(int bitpos, int bitlength)
        {
            var field = new Bitfield(bitpos, bitlength);
            return (u, d) =>
            {
                var offset = field.ReadSigned(u) << 1;
                var addr = d.rdr.Address + offset;
                d.ops.Add(addr);
                return true;
            };
        }
        private static readonly Mutator<Mips16eDisassembler> ARel8 = ARel(0, 8);
        private static readonly Mutator<Mips16eDisassembler> ARel11 = ARel(0, 11);

        private static bool ARelEx(uint uInstr, Mips16eDisassembler dasm)
        {
            Debug.Assert(dasm.extend.HasValue);
            var uOffset =
                (Bitfield.ReadFields(bf_extend5, dasm.extend.Value) << bf0_5.Length) |
                bf0_5.Read(uInstr);
            var offset = Bits.SignExtend(uOffset, 16) << 1;
            dasm.ops.Add(dasm.rdr.Address + offset);
            return true;
        }

        /// <summary>
        /// Region-relative address.
        /// </summary>
        private static bool ARegRel(uint uInstr, Mips16eDisassembler dasm)
        {
            var offset = Bitfield.ReadFields(bf_jal, uInstr);
            var uDst = dasm.rdr.Address.ToLinear() & ~((1u << 28) - 1u);
            uDst |= offset << 2;
            var addrDst = Address.Create(dasm.arch.PointerType, uDst);
            dasm.ops.Add(addrDst);
            return true;
        }

        private static Mutator<Mips16eDisassembler> Msp(PrimitiveType dt, Bitfield fieldOffset)
        {
            return (u, d) =>
            {
                var offset = (int) (fieldOffset.Read(u) * dt.Size);
                var mem = MemoryOperand.Indirect(
                    dt,
                    Constant.Int32(offset),
                    d.arch.StackRegister);
                d.ops.Add(mem);
                return true;
            };
        }
        private static readonly Mutator<Mips16eDisassembler> MspW8 = Msp(PrimitiveType.Word32, bf0_8);

        private static bool MspEx(uint uInstr, Mips16eDisassembler dasm)
        {
            Debug.Assert(dasm.extend.HasValue);
            var uExt = ((Bitfield.ReadFields(bf_extend5, dasm.extend.Value) << bf0_5.Length | bf0_5.Read(uInstr)));
            var addr = dasm.addr.Align(4) + Bits.SignExtend(uExt, 16);
            dasm.ops.Add(addr);
            return true;
        }

        private static Mutator<Mips16eDisassembler> Mpc(PrimitiveType dt, Bitfield fieldOffset)
        {
            return (u, d) =>
            {
                var offset = (int) (fieldOffset.Read(u) * dt.Size);
                var mem = MemoryOperand.Indirect(
                    dt,
                    Constant.Int32(offset),
                    d.arch.pc);
                d.ops.Add(mem);
                return true;
            };
        }
        private static readonly Mutator<Mips16eDisassembler> MpcW8 = Mpc(PrimitiveType.Word32, bf0_8);

        private static Mutator<Mips16eDisassembler> MpcEx(PrimitiveType dt, Bitfield fieldOffset)
        {
            return (u, d) =>
            {
                Debug.Assert(d.extend.HasValue);
                var uExt = ((Bitfield.ReadFields(bf_extend5, d.extend.Value) << bf0_5.Length | bf0_5.Read(u)));
                var addr = d.addr.Align(4) + Bits.SignExtend(uExt, 16);
                d.ops.Add(addr);
                return true;
            };
        }

        /// <summary>
        /// Stack-based memory access of a pointer.
        /// </summary>
        private static bool MptrSp8(uint uInstr, Mips16eDisassembler dasm)
        {
            //    var uExtend = Bitfield.ReadFields(bf_extend, dasm.extend.Value) << bf0_5.Length;
            //    offset = (int) Bits.SignExtend(uExtend | bf0_5.Read(uInstr), bf0_5.Length + 11);
            int offset = (int) bf0_8.Read(uInstr) << 2;
            var mem = MemoryOperand.Indirect(
                dasm.arch.PointerType,
                Constant.Int32(offset),
                dasm.arch.StackRegister);
            dasm.ops.Add(mem);
            return true;
        }

        private static bool MptrSp8Ex(uint uInstr, Mips16eDisassembler dasm)
        {
            var uExtend = Bitfield.ReadFields(bf_extend5, dasm.extend!.Value) << bf0_5.Length;
            var offset = (int) Bits.SignExtend(uExtend | bf0_5.Read(uInstr), bf0_5.Length + 11);
            var mem = MemoryOperand.Indirect(
                dasm.arch.PointerType,
                Constant.Int32(offset),
                dasm.arch.StackRegister);
            dasm.ops.Add(mem);
            return true;
        }


        private static Mutator<Mips16eDisassembler> M(PrimitiveType dt, Bitfield fieldreg, Bitfield fieldOffset)
        {
            return (u, d) =>
            {
                int offset;
                if (d.extend.HasValue)
                {
                    var uExtend = Bitfield.ReadFields(bf_extend5, d.extend.Value) << fieldOffset.Length;
                    offset = (int) Bits.SignExtend(uExtend | fieldOffset.Read(u), fieldOffset.Length + 11);
                }
                else
                {
                    offset = (int) (fieldOffset.Read(u) * dt.Size);
                }
                var encReg = fieldreg.Read(u);
                var reg = registerEncoding[encReg];
                var mem = MemoryOperand.Indirect(
                    dt,
                    Constant.Int32(offset),
                    reg);
                d.ops.Add(mem);
                return true;
            };
        }
        private static readonly Mutator<Mips16eDisassembler> Mb5 = M(PrimitiveType.SByte, new Bitfield(5, 3), new Bitfield(0, 5));
        private static readonly Mutator<Mips16eDisassembler> Mh5 = M(PrimitiveType.Int16, new Bitfield(5, 3), new Bitfield(0, 5));
        private static readonly Mutator<Mips16eDisassembler> Mbu5 = M(PrimitiveType.Byte, new Bitfield(5, 3), new Bitfield(0, 5));
        private static readonly Mutator<Mips16eDisassembler> Mhu5 = M(PrimitiveType.Word16, new Bitfield(5, 3), new Bitfield(0, 5));
        private static readonly Mutator<Mips16eDisassembler> Mw5 = M(PrimitiveType.Word32, new Bitfield(5, 3), new Bitfield(0, 5));




        #region  Decoders

        /// <summary>
        /// This decoder is used to read the second 16-bit chunk of a 32-bit instruction;
        /// the previously read bits are then concatenated with the read chunk and 
        /// control of the newly minted 32-bit instruction is handed to the inner decoder.
        /// </summary>
        private class Read32Decoder : Decoder
        {
            private readonly Decoder decoder;

            public Read32Decoder(Decoder decoder)
            {
                this.decoder = decoder;
            }

            public override MipsInstruction Decode(uint wInstr, Mips16eDisassembler dasm)
            {
                if (!dasm.rdr.TryReadUInt16(out ushort uLow16bits))
                    return dasm.CreateInvalidInstruction();
                var uInstr32 = (wInstr << 16) | uLow16bits;
                return decoder.Decode(uInstr32, dasm);
            }
        }

        /// <summary>
        /// This decoder remembers the extension word in the <see cref="Mips16eDisassembler.extend"/>
        /// member variable.
        /// </summary>
        private class ExtendDecoder : Decoder
        {
            public override MipsInstruction Decode(uint wInstr, Mips16eDisassembler dasm)
            {
                if (!dasm.rdr.TryReadUInt16(out ushort uLow16bits))
                    return dasm.CreateInvalidInstruction();
                dasm.extend = wInstr;
                return rootDecoder.Decode(uLow16bits, dasm);
            }
        }

        private class SelectExtendDecoder : Decoder
        {
            private readonly Decoder nonExtended;
            private readonly Decoder extended;

            public SelectExtendDecoder(Decoder nonExtended, Decoder extended)
            {
                this.nonExtended = nonExtended;
                this.extended = extended;
            }

            public override MipsInstruction Decode(uint wInstr, Mips16eDisassembler dasm)
            {
                if (dasm.extend.HasValue)
                    return extended.Decode(wInstr, dasm);
                else 
                    return nonExtended.Decode(wInstr, dasm);
            }
        }


        public static Decoder Instr(Mnemonic mnemonic, params Mutator<Mips16eDisassembler> [] mutators)
        {
            return new InstrDecoder<Mips16eDisassembler, Mnemonic, MipsInstruction>(InstrClass.Linear, mnemonic, mutators);
        }

        public static Decoder Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<Mips16eDisassembler>[] mutators)
        {
            return new InstrDecoder<Mips16eDisassembler, Mnemonic, MipsInstruction>(iclass, mnemonic, mutators);
        }

        public static Decoder Nyi(string message)
        {
            return new NyiDecoder<Mips16eDisassembler, Mnemonic, MipsInstruction>(message);
        }

        /// <summary>
        /// Selects one of two instruction formats depending on whether the EXTEND 
        /// instruction has been seen or not.
        /// </summary>
        private static Decoder Ex(Decoder nonExtended, Decoder extended)
        {
            return new SelectExtendDecoder(nonExtended, extended);
        }

        #endregion

        static Mips16eDisassembler()
        {
            var invalid = Instr(Mnemonic.illegal, InstrClass.Invalid);

            var svrsDecoders = Mask(7, 1,
                Ex(
                    Instr(Mnemonic.restore, MultiRegs, SaveFramesize),
                    Instr(Mnemonic.restore, MultiRegsEx, SaveFramesizeEx)),
                Ex(
                    Instr(Mnemonic.save, MultiRegs, SaveFramesize),
                    Instr(Mnemonic.save, MultiRegsEx, SaveFramesizeEx)));

            var i8decoders = Mask(8, 3, "  I8",
                Ex(
                    Instr(Mnemonic.bteqz, ARel8),
                    Instr(Mnemonic.bteqz, ARelEx)),
                Ex(
                    Instr(Mnemonic.btnez, ARel8),
                    Instr(Mnemonic.btnez, ARelEx)),
                Ex(
                    Instr(Mnemonic.sw, ra,MptrSp8),
                    Instr(Mnemonic.sw, ra,MptrSp8Ex)),
                Ex(
                    Instr(Mnemonic.addi,sp,SImm8s3),
                    Instr(Mnemonic.addi,sp,SImmEx5)),

                svrsDecoders,
                Instr(Mnemonic.move,Reg((3,2),(5,3)),R0),
                invalid,
                Instr(Mnemonic.move,R5,Reg((0,5))));

            var cnvtDecoders = Mask(5, 3, "  CNVT",
                Instr(Mnemonic.zeb, R8),
                Instr(Mnemonic.zeh, R8),
                invalid,
                invalid,
                Instr(Mnemonic.seb, R8),
                Instr(Mnemonic.seh, R8),
                invalid,
                invalid);

            var jrcDecoders = Mask(5, 3, "  JRC",
                Instr(Mnemonic.jr, InstrClass.Transfer|InstrClass.Delay, R8),
                Instr(Mnemonic.jr, InstrClass.Transfer | InstrClass.Delay, ra),
                Instr(Mnemonic.jalr, InstrClass.Transfer | InstrClass.Delay | InstrClass.Call, R8),
                invalid,

                Instr(Mnemonic.jrc, InstrClass.Transfer, R8),
                Instr(Mnemonic.jrc, InstrClass.Transfer, ra),
                Instr(Mnemonic.jalrc, InstrClass.Transfer | InstrClass.Call, R8),
                invalid);


            var rrDecoders = Mask(0, 5, "  RR",
                jrcDecoders,
                Instr(Mnemonic.sdbbp, UImmSh(5, 6, 0)),
                Instr(Mnemonic.slt, R8, R5),
                Instr(Mnemonic.sltu, R8, R5),

                Instr(Mnemonic.sllv, R5,R8),
                Instr(Mnemonic.@break, UImmSh(5, 6, 0)),
                Instr(Mnemonic.srlv, R5,R8),
                Instr(Mnemonic.srav, R5,R8),

                invalid,  // reserved
                invalid,  // reserved
                Instr(Mnemonic.cmp, R8,R5),
                Instr(Mnemonic.neg, R8,R5),

                Instr(Mnemonic.and, R8,R5),
                Instr(Mnemonic.or, R8,R5),
                Instr(Mnemonic.xor, R8,R5),
                Instr(Mnemonic.not, R8,R5),

                Instr(Mnemonic.mfhi, R8),  // reserved
                cnvtDecoders,  // reserved
                Instr(Mnemonic.mflo, R8),  // reserved
                invalid,  // reserved

                invalid,  // reserved
                invalid,  // reserved
                invalid,  // reserved
                invalid,  // reserved

                Instr(Mnemonic.mult, R8,R5),
                Instr(Mnemonic.multu, R8,R5),
                Instr(Mnemonic.div, R8, R5),
                Instr(Mnemonic.divu, R8, R5),

                invalid,  // reserved
                invalid,  // reserved
                invalid,  // reserved
                invalid);  // reserved

            rootDecoder = Mask(11, 5, "MIPS16e",
                Ex(
                    Instr(Mnemonic.addiu, R8, sp, UImm8s2),
                    Instr(Mnemonic.addiu, R8, sp, SImmEx5)),
                Ex(
                    Instr(Mnemonic.la, R8, AImm8),
                    Instr(Mnemonic.la, R8, AImmEx5)),
                Ex(
                    Instr(Mnemonic.b, InstrClass.Transfer, ARel11),
                    Instr(Mnemonic.b, InstrClass.Transfer, ARelEx)),
                new Read32Decoder(Mask(26, 1, "  JAL(X)",
                    Instr(Mnemonic.jal, InstrClass.Transfer|InstrClass.Call|InstrClass.Delay, ARegRel),
                    Instr(Mnemonic.jalx, InstrClass.Transfer|InstrClass.Call|InstrClass.Delay, ARegRel))),

                Ex(
                    Instr(Mnemonic.beqz, InstrClass.ConditionalTransfer, R8, ARel8),
                    Instr(Mnemonic.beqz, InstrClass.ConditionalTransfer, R8, ARelEx)),
                Ex(
                    Instr(Mnemonic.bnez, InstrClass.ConditionalTransfer, R8, ARel8),
                    Instr(Mnemonic.bnez, InstrClass.ConditionalTransfer, R8, ARelEx)),
                Mask(0, 2, "  SHIFT",
                    Ex(
                        Instr(Mnemonic.sll, R8,R5,ShAmt),
                        Instr(Mnemonic.sll, R8,R5,ShAmtEx)),
                    invalid,    // reserved
                    Ex(
                        Instr(Mnemonic.srl, R8,R5,ShAmt),
                        Instr(Mnemonic.srl, R8,R5,ShAmtEx)),
                    Ex(
                        Instr(Mnemonic.sra, R8,R5,ShAmt),
                        Instr(Mnemonic.sra, R8,R5,ShAmtEx))),
                invalid,        // reserved

                Mask(4, 1, "  RRI - A ",
                    Ex(
                        Instr(Mnemonic.addiu, R8, R5, UImm4),
                        Instr(Mnemonic.addiu, R8, R5, SImmEx4)),
                    invalid),  // reserved
                Ex(
                    Instr(Mnemonic.addiu, R8, UImm8), 
                    Instr(Mnemonic.addiu, R8, SImmEx5)),
                Ex(
                    Instr(Mnemonic.slti, R8, UImm8),
                    Instr(Mnemonic.slti, R8, SImmEx5)),
                Ex(
                    Instr(Mnemonic.sltiu, R8, UImm8),
                    Instr(Mnemonic.sltiu, R8, SImmEx5)),

                i8decoders,
                Ex(
                    Instr(Mnemonic.li, R8, UImm8),
                    Instr(Mnemonic.li, R8, UImmEx5)),
                Ex(
                    Instr(Mnemonic.cmpi, R8, UImm8),
                    Instr(Mnemonic.cmpi, R8, UImmEx5)),
                invalid,      // reserved

                Instr(Mnemonic.lb, R5, Mb5),
                Instr(Mnemonic.lh, R5, Mh5),
                Ex(
                    Instr(Mnemonic.lw, R8, MspW8),
                    Instr(Mnemonic.lw, R8, MspEx)),
                Instr(Mnemonic.lw, R5, Mw5),

                Instr(Mnemonic.lbu, R5, Mbu5),
                Instr(Mnemonic.lhu, R5, Mhu5),
                Ex(
                    Instr(Mnemonic.lw, R8, MpcW8),
                    Instr(Mnemonic.lw, R8, MpcEx(PrimitiveType.Word32, bf0_5))),
                invalid,      // reserved

                Instr(Mnemonic.sb, R5, Mbu5),
                Instr(Mnemonic.sh, R5, Mhu5),
                Instr(Mnemonic.sw, R8, MspW8),
                Instr(Mnemonic.sw, R5, Mw5),

                Mask(0, 2, "  RRR",
                    invalid,    // reserved,
                    Instr(Mnemonic.addu, R2,R8,R5),
                    invalid,    // reserved,
                    Instr(Mnemonic.subu, R2,R8,R5)),
                rrDecoders,
                new ExtendDecoder(),
                invalid);      // reserved
        }
    }
}
