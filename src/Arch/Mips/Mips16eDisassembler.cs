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

using Reko.Core;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Mips
{
    using Decoder = Reko.Core.Machine.Decoder<Mips16eDisassembler, Mnemonic, MipsInstruction>;

    public class Mips16eDisassembler : DisassemblerBase<MipsInstruction, Mnemonic>
    {
        private static readonly Decoder rootDecoder;

        private readonly MipsProcessorArchitecture arch;
        private readonly EndianImageReader rdr;
        private readonly List<MachineOperand> ops;
        private Address addr;

        public Mips16eDisassembler(MipsProcessorArchitecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.ops = new List<MachineOperand>();
        }

        public override MipsInstruction DisassembleInstruction()
        {
            this.addr = rdr.Address;
            this.ops.Clear();
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

        public override MipsInstruction NotYetImplemented(uint wInstr, string message)
        {
            var len = rdr.Address - this.addr;
            var rdr2 = rdr.Clone();
            rdr2.Offset -= len;
            var hex = string.Join("", rdr2.ReadBytes((int) len).Select(b => b.ToString("X2")));
            base.EmitUnitTest("Mips16e", hex, message, "Mips16eDis", this.addr, w =>
            {
                w.WriteLine("           AssertCode(\"@@@\", \"{0}\");", hex);
            });

            return base.NotYetImplemented(wInstr, message);
        }

        public override MipsInstruction CreateInvalidInstruction()
        {
            return new MipsInstruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.illegal,
                Operands = MachineInstruction.NoOperands,
            };
        }

        #region Bitfields

        private static readonly Bitfield bf0_8 = new Bitfield(0, 8);
        private static readonly Bitfield bf2_3 = new Bitfield(2, 3);
        private static readonly Bitfield[] bf_jal = Bf((16, 5), (20, 5), (0, 16));

        #endregion

        private static readonly RegisterOperand[] registerEncoding = new uint[8] { 16, 17, 2, 3, 4, 5, 6, 7 }
            .Select(iReg => new RegisterOperand(Registers.generalRegs[iReg]))
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
                d.ops.Add(new RegisterOperand(reg));
                return true;
            };
        }
        /// <summary>
        /// Stack register
        /// </summary>
        private static bool sp(uint uInstr, Mips16eDisassembler dasm)
        {
            dasm.ops.Add(new RegisterOperand(dasm.arch.StackRegister));
            return true;
        }

        /// <summary>
        /// Program counter.
        /// </summary>
        private static bool pc(uint uInstr, Mips16eDisassembler dasm)
        {
            dasm.ops.Add(new RegisterOperand(dasm.arch.pc));
            return true;
        }

        /// <summary>
        /// Return address register.
        /// </summary>
        private static bool ra(uint uInstr, Mips16eDisassembler dasm)
        {
            dasm.ops.Add(new RegisterOperand(dasm.arch.GeneralRegs[31]));
            return true;
        }

        /// <summary>
        /// Decode a single bit corresponding to a register.
        /// </summary>
        public static bool MultiRegs(uint uInstr, Mips16eDisassembler dasm)
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

        /// <summary>
        /// A shifted unsigned immediate.
        /// </summary>
        public static Mutator<Mips16eDisassembler> UImmSh(int bitpos, int bitlength, int shift)
        {
            var field = new Bitfield(bitpos, bitlength);
            return (u, d) =>
            {
                var uImm = field.Read(u) << shift;
                d.ops.Add(ImmediateOperand.Word32(uImm));
                return true;
            };
        }
        public static readonly Mutator<Mips16eDisassembler> UImm4 = UImmSh(0, 4, 0);
        public static readonly Mutator<Mips16eDisassembler> UImm8 = UImmSh(0, 8, 0);
        public static readonly Mutator<Mips16eDisassembler> UImm8s2 = UImmSh(0, 8, 2);

        /// <summary>
        /// A shifted signed immediate.
        /// </summary>
        public static Mutator<Mips16eDisassembler> SImmSh(int bitpos, int bitlength, int shift)
        {
            var field = new Bitfield(bitpos, bitlength);
            return (u, d) =>
            {
                var uImm = field.ReadSigned(u) << shift;
                d.ops.Add(ImmediateOperand.Int32(uImm));
                return true;
            };
        }
        public static readonly Mutator<Mips16eDisassembler> SImm8s3 = SImmSh(0, 8, 3);

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
        private static readonly ImmediateOperand []shortShiftAmts = new int[8] { 8, 1, 2, 3, 4, 5, 6, 7 }
            .Select(n => ImmediateOperand.Int32(n))
            .ToArray();
        
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
        private static Mutator<Mips16eDisassembler> SaveFramesize = Framesize(Bf((0, 4)));

        private static readonly ImmediateOperand[] frameSizeEncoding = new int[16]
        {
            16, 1, 2, 3,  4, 5, 6, 7,  8, 9, 10, 11,  12, 13, 14, 15
        }
            .Select(n => ImmediateOperand.Int32(n * 8))
            .ToArray();

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
                d.ops.Add(AddressOperand.Create(addr));
                return true;
            };
        }
        public static readonly Mutator<Mips16eDisassembler> ARel8 = ARel(0, 8);
        public static readonly Mutator<Mips16eDisassembler> ARel11 = ARel(0, 11);

        /// <summary>
        /// Region-relative address.
        /// </summary>
        private static bool ARegRel(uint uInstr, Mips16eDisassembler dasm)
        {
            var offset = Bitfield.ReadFields(bf_jal, uInstr);
            var uDst = dasm.rdr.Address.ToLinear() & ~((1u << 28) - 1u);
            uDst |= offset << 2;
            var addrDst = Address.Create(dasm.arch.PointerType, uDst);
            dasm.ops.Add(AddressOperand.Create(addrDst));
            return true;
        }

        private static Mutator<Mips16eDisassembler> Msp(PrimitiveType dt, Bitfield fieldOffset)
        {
            return (u, d) =>
            {
                var offset = (int) (fieldOffset.Read(u) * dt.Size);
                var mem = new IndirectOperand(dt, offset, d.arch.StackRegister);
                d.ops.Add(mem);
                return true;
            };
        }
        private static readonly Mutator<Mips16eDisassembler> MspW8 = Msp(PrimitiveType.Word32, bf0_8);


        private static Mutator<Mips16eDisassembler> Mpc(PrimitiveType dt, Bitfield fieldOffset)
        {
            return (u, d) =>
            {
                var offset = (int) (fieldOffset.Read(u) * dt.Size);
                var mem = new IndirectOperand(dt, offset, d.arch.pc);
                d.ops.Add(mem);
                return true;
            };
        }
        private static readonly Mutator<Mips16eDisassembler> MpcW8 = Mpc(PrimitiveType.Word32, bf0_8);

        /// <summary>
        /// Stack-based memory access of a pointer.
        /// </summary>
        private static bool MptrSp8(uint uInstr, Mips16eDisassembler dasm)
        {
            var offset = bf0_8.Read(uInstr) << 2;
            var mem = new IndirectOperand(dasm.arch.PointerType, (int) offset, dasm.arch.StackRegister);
            dasm.ops.Add(mem);
            return true;
        }

        private static Mutator<Mips16eDisassembler> M(PrimitiveType dt, Bitfield fieldreg, Bitfield fieldOffset)
        {
            return (u, d) =>
            {
                var offset = (int) (fieldOffset.Read(u) * dt.Size);
                var encReg = fieldreg.Read(u);
                var reg = registerEncoding[encReg].Register;
                var mem = new IndirectOperand(dt, offset, reg);
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

        #endregion

        static Mips16eDisassembler()
        {
            var invalid = Instr(Mnemonic.illegal, InstrClass.Invalid);

            var svrsDecoders = Mask(7, 1,
                Instr(Mnemonic.restore),
                Instr(Mnemonic.save, MultiRegs, SaveFramesize));

            var i8decoders = Mask(8, 3, "  I8",
                Instr(Mnemonic.bteqz, ARel8),
                Instr(Mnemonic.btnez, ARel8),
                Instr(Mnemonic.sw, ra,MptrSp8),
                Instr(Mnemonic.addi,sp,SImm8s3),

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
                Instr(Mnemonic.addiu, R8, sp, UImm8s2),
                Instr(Mnemonic.addiu, R8, pc, UImm8s2),
                Instr(Mnemonic.b, InstrClass.Transfer, ARel11),
                new Read32Decoder(Mask(26, 1, "  JAL(X)",
                    Instr(Mnemonic.jal, InstrClass.Transfer|InstrClass.Call|InstrClass.Delay, ARegRel),
                    Instr(Mnemonic.jalx, InstrClass.Transfer|InstrClass.Call|InstrClass.Delay, ARegRel))),

                Instr(Mnemonic.beqz, InstrClass.ConditionalTransfer, R8, ARel8),
                Instr(Mnemonic.bnez, InstrClass.ConditionalTransfer, R8, ARel8),
                Mask(0, 2, "  SHIFT",
                    Instr(Mnemonic.sll, R8,R5,ShAmt),
                    invalid,    // reserved
                    Instr(Mnemonic.srl, R8,R5,ShAmt),
                    Instr(Mnemonic.sra, R8,R5,ShAmt)),
                invalid,        // reserved

                Mask(4, 1, "  RRI - A ",
                    Instr(Mnemonic.addiu, R8, R5, UImm4),
                    invalid),  // reserved
                Instr(Mnemonic.addiu, R8, UImm8),
                Instr(Mnemonic.slti, R8, UImm8),
                Instr(Mnemonic.sltiu, R8, UImm8),

                i8decoders,
                Instr(Mnemonic.li, R8, UImm8),
                Instr(Mnemonic.cmpi, R8, UImm8),
                invalid,      // reserved

                Instr(Mnemonic.lb, R5, Mb5),
                Instr(Mnemonic.lh, R5, Mh5),
                Instr(Mnemonic.lw, R8, MspW8),
                Instr(Mnemonic.lw, R5, Mw5),

                Instr(Mnemonic.lbu, R5, Mbu5),
                Instr(Mnemonic.lhu, R5, Mhu5),
                Instr(Mnemonic.lw, R8, MpcW8),
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
                Nyi("EXTEND     "),
                invalid);      // reserved
        }
    }
}
