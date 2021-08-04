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
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.Core.Types;
using System.Collections.Generic;

namespace Reko.Arch.MCore
{
    using Decoder = Reko.Core.Machine.Decoder<MCoreDisassembler, Mnemonic, MCoreInstruction>;

    public class MCoreDisassembler : Reko.Core.Machine.DisassemblerBase<MCoreInstruction, Mnemonic>
    {
        private static readonly Decoder rootDecoder;

        private readonly MCoreArchitecture arch;
        private readonly EndianImageReader rdr;
        private readonly List<MachineOperand> ops;
        private Address addr;

        public MCoreDisassembler(MCoreArchitecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.ops = new List<MachineOperand>();
            this.addr = Address.Ptr32(0);
        }

        public override MCoreInstruction CreateInvalidInstruction()
        {
            return new MCoreInstruction
            {
                Mnemonic = Mnemonic.invalid,
                Operands = MachineInstruction.NoOperands
            };
        }

        public override MCoreInstruction? DisassembleInstruction()
        {
            this.addr = rdr.Address;
            this.ops.Clear();
            if (!rdr.TryReadBeUInt16(out ushort uInstr))
                return null;
            var instr = rootDecoder.Decode(uInstr, this);
            instr.Address = addr;
            instr.Length = 2;
            return instr;
        }

        public override MCoreInstruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            return new MCoreInstruction
            {
                InstructionClass = iclass,
                Mnemonic = mnemonic,
                Operands = ops.ToArray()
            };
        }

        public override MCoreInstruction NotYetImplemented(string message)
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingDecoder("MCoreDis", this.addr, this.rdr, message);
            return CreateInvalidInstruction();
        }

        #region Fields
        private static readonly Bitfield bf0_4 = new Bitfield(0, 4);
        private static readonly Bitfield bf4_4 = new Bitfield(4, 4);
        private static readonly Bitfield bf8_4 = new Bitfield(8, 4);
        #endregion

        #region Mutators
        private static Mutator<MCoreDisassembler> Reg(int bitpos, int bitLength, RegisterStorage[] regs)
        {
            var regField = new Bitfield(bitpos, 4);
            return (u, d) =>
            {
                var iReg = regField.Read(u);
                var reg = regs[iReg];
                d.ops.Add(new RegisterOperand(reg));
                return true;
            };
        }
        private static readonly Mutator<MCoreDisassembler> R0 = Reg(0, 4, Registers.GpRegisters);
        private static readonly Mutator<MCoreDisassembler> R4 = Reg(4, 4, Registers.GpRegisters);
        private static readonly Mutator<MCoreDisassembler> R8 = Reg(8, 4, Registers.GpRegisters);
        private static readonly Mutator<MCoreDisassembler> CR4 = Reg(4, 5, Registers.CrRegisters);

        private static Mutator<MCoreDisassembler> Imm(int bitpos, int bitlen)
        {
            var immField = new Bitfield(bitpos, bitlen);
            return (u, d) =>
            {
                var imm = immField.Read(u);
                d.ops.Add(ImmediateOperand.Word32(imm));
                return true;
            };
        }
        private static readonly Mutator<MCoreDisassembler> Imm4_7 = Imm(4, 7);
        private static readonly Mutator<MCoreDisassembler> Imm4_5 = Imm(4, 5);

        private static Mutator<MCoreDisassembler> Mdisp(PrimitiveType dt)
        {
            return (u, d) =>
            {
                var iBaseReg = bf0_4.Read(u);
                var reg = Registers.GpRegisters[iBaseReg];
                var disp = (int) bf4_4.Read(u) * dt.Size;
                var op = new MemoryOperand(dt, reg, disp);
                d.ops.Add(op);
                return true;
            };
        }
        private static readonly Mutator<MCoreDisassembler> MdispB = Mdisp(PrimitiveType.Byte);
        private static readonly Mutator<MCoreDisassembler> MdispH = Mdisp(PrimitiveType.Word16);
        private static readonly Mutator<MCoreDisassembler> MdispW = Mdisp(PrimitiveType.Word32);

        /// <summary>
        /// PC-relative jump displacement.
        /// </summary>
        private static Mutator<MCoreDisassembler> Disp(int bitlen)
        {
            var dispField = new Bitfield(0, bitlen);
            return (u, d) =>
            {
                var disp = dispField.ReadSigned(u) << 1;
                var addr = d.addr + (2 + disp);
                d.ops.Add(AddressOperand.Create(addr));
                return true;
            };
        }
        private static readonly Mutator<MCoreDisassembler> D11 = Disp(11);

        private static Mutator<MCoreDisassembler> Disp(int bitlen, int shift)
        {
            var dispField = new Bitfield(0, bitlen);
            return (u, d) =>
            {
                var disp = dispField.ReadSigned(u) << shift;
                var addr = Address.Ptr32((uint)(d.addr.ToUInt32() + (2 + disp)) & ~3u);
                d.ops.Add(AddressOperand.Create(addr));
                return true;
            };
        }
        private static readonly Mutator<MCoreDisassembler> D8 = Disp(8, 2);

        #endregion

        private static bool Is0(uint u) => u == 0;

        private static Decoder Nyi(string message)
        {
            return new NyiDecoder<MCoreDisassembler, Mnemonic, MCoreInstruction>(message);
        }

        static MCoreDisassembler()
        {
            var invalid = Instr(Mnemonic.invalid, InstrClass.Invalid, new Mutator<MCoreDisassembler>[0]);

            /*
            // CK-610 C-Sky
            // https://github.com/c-sky/tools/raw/master/gx6605s/
             Legend:
            rrrr - RX field
            ssss - RY field
            zzzz - RZ field
            ffff - Rfirst field
            ccccc - control register specifier
            iii..i - one of several immediate fields
            ddddddddddd - branch displacement
            bbbb - loopt displacement
            uu- accelerator unit
            ee..e - execution code
            nnn - register count
            p - update option
            xx..x - undefined fields
            */
            var decode_00 = Mask(4, 4, "  0000",
                    Nyi("  0000"),
                    /*
                0000 0000 0000 0000 bkpt
                0000 0000 0000 0001 sync
                0000 0000 0000 0010 rte
                0000 0000 0000 0011 rfi
                0000 0000 0000 0100 stop
                0000 0000 0000 0101 wait
                0000 0000 0000 0110 doze
                0000 0000 0000 0111
                0000 0000 0000 10ii trap #ii
                0000 0000 0000 11xx
                */
                    Instr(Mnemonic.mvc, R0),
                    invalid,
                    Instr(Mnemonic.mvcv, R0),
                    Instr(Mnemonic.ldq, R0),
                    Instr(Mnemonic.stq, R0),
                    Instr(Mnemonic.ldm, R0),
                    Instr(Mnemonic.stm, R0),
                    Instr(Mnemonic.dect, R0),
                    Instr(Mnemonic.decf, R0),
                    Instr(Mnemonic.inct, R0),
                    Instr(Mnemonic.incf, R0),
                    Instr(Mnemonic.jmp, R0),
                    Instr(Mnemonic.jsr, R0),
                    Instr(Mnemonic.ff1, R0),
                    Instr(Mnemonic.brev, R0));

            var decode_01 = Mask(4, 4, "  0001",
                Instr(Mnemonic.xtrb3, R0),
                Instr(Mnemonic.xtrb2, R0),
                Instr(Mnemonic.xtrb1, R0),
                Instr(Mnemonic.xtrb0, R0),
                Instr(Mnemonic.zextb, R0),
                Instr(Mnemonic.sextb, R0),
                Instr(Mnemonic.zexth, R0),
                Instr(Mnemonic.sexth, R0),
                Instr(Mnemonic.declt, R0),
                Instr(Mnemonic.tstnbz, R0),
                Instr(Mnemonic.decgt, R0),
                Instr(Mnemonic.decne, R0),
                Instr(Mnemonic.clrt, R0),
                Instr(Mnemonic.clrf, R0),
                Instr(Mnemonic.abs, R0),
                Instr(Mnemonic.not, R0));

            var decode_0 = Mask(8, 4, "  0000",
                decode_00,
                decode_01,
                Instr(Mnemonic.movt, R0, R4),
                Instr(Mnemonic.mult, R0, R4),
                Nyi("0000 0100 ssssbbbb loopt "),
                Instr(Mnemonic.subu, R0, R4),
                Instr(Mnemonic.addc, R0, R4),
                Instr(Mnemonic.subc, R0, R4),
                Nyi("0000 1000 ssssrrrr       "),
                Nyi("0000 1001 ssssrrrr       "),
                Instr(Mnemonic.movf, R0, R4),
                Instr(Mnemonic.lsr, R0, R4),
                Instr(Mnemonic.cmphs, R0, R4),
                Instr(Mnemonic.cmplt, R0, R4),
                Instr(Mnemonic.tst, R0, R4),
                Instr(Mnemonic.cmpne, R0, R4));

            var decode_1 = Mask(8, 4, "  0001",
                Instr(Mnemonic.mfcr, InstrClass.Privileged, R0, CR4),
                Instr(Mnemonic.mfcr, InstrClass.Privileged, R0, CR4),
                Instr(Mnemonic.mov, R0, R4),
                Instr(Mnemonic.bgenr, R0, R4),

                Instr(Mnemonic.rsub, R0, R4),
                Instr(Mnemonic.ixw, R0, R4),
                Instr(Mnemonic.and, R0, R4),
                Instr(Mnemonic.xor, R0, R4),

                Instr(Mnemonic.mtcr, InstrClass.Privileged, R0, CR4),
                Instr(Mnemonic.mtcr, InstrClass.Privileged, R0, CR4),
                Instr(Mnemonic.asr, R0, R4),
                Instr(Mnemonic.lsl, R0, R4),

                Instr(Mnemonic.addu, R0, R4),
                Instr(Mnemonic.ixh, R0, R4),
                Instr(Mnemonic.or, R0, R4),
                Instr(Mnemonic.andn, R0, R4));

            var decode_2 = Mask(9, 3, "  0010",
                Instr(Mnemonic.addi, R0, Imm4_5),
                Instr(Mnemonic.cmplti, R0, Imm4_5),
                Instr(Mnemonic.subi, R0, Imm4_5),
                invalid,        // 0010 011 iiiiirrrr

                Instr(Mnemonic.rsubi, R0, Imm4_5),
                Instr(Mnemonic.cmpnei, R0, Imm4_5),
                Nyi("  110"),
                //0010 110 00000rrrr bmaski #32 (set)
                //0010 110 00001rrrr divu
                //0010 110 0001xrrrr
                //0010 110 001xxrrrr
                //0010 110 01iiirrrr bmaski
                //0010 110 1iiiirrrr bmaski
                Instr(Mnemonic.andi, R0, Imm4_5));

            var decode_3 = Mask(9, 3, "  0011",
                Instr(Mnemonic.bclri, R0, Imm4_5),
                Nyi("  001"),
                Instr(Mnemonic.bseti, R0, Imm4_5),
                Instr(Mnemonic.btsti, R0, Imm4_5),
                Select((4, 5), Is0, "  100",
                    Instr(Mnemonic.xsr, R0),
                    Instr(Mnemonic.rotli, R0, Imm4_5)),
                Select((4, 5), Is0, "  101",
                    Instr(Mnemonic.asrc, R0),
                    Instr(Mnemonic.asri, R0, Imm4_5)),
                Select((4, 5), Is0, "  110",
                    Instr(Mnemonic.lslc, R0),
                    Instr(Mnemonic.lsli, R0, Imm4_5)),
                Select((4, 5), Is0, "  111",
                    Instr(Mnemonic.lsrc, R0),
                    Instr(Mnemonic.lsri, R0, Imm4_5)));

            /*
                    0011 001 00000rrrr
                    0011 001 00001rrrr divs
                    0011 001 00111rrrr bgeni
                    0011 001 01iiirrrr bgeni
                    0011 001 1iiiirrrr bgeni
                             
                    0011 010 iiiiirrrr bseti
                    0011 010 iiiiirrrr bseti

                    0011 011 iiiiirrrr btsti
                    0011 011 iiiiirrrr btsti
                             
                    0011 1100001xrrrr
                    0011 1100010xrrrr
                    0011 11000110rrrr

             
             */
            var decode_4 = Mask(7, 3, "  0100",
                Nyi("0100uu00e eeeeeee h_exec  "),
                Nyi("0100uu00e eeeeeee h_exec  "),
                Nyi("0100uu010 nnneeee h_ret   "),
                Nyi("0100uu011 nnneeee h_call  "),
                Nyi("0100uu100 piirrrr h_ld    "),
                Nyi("0100uu101 piirrrr h_st    "),
                Nyi("0100uu110 piirrrr h_ld.h  "),
                Nyi("0100uu111 piirrrr h_st.h  "));
            var decode_5 = invalid;  // 0101xxxxxxxxxxxx

            var decode_6 = Mask(11, 1, "  0110",
                Instr(Mnemonic.movi, R0, Imm4_7),
                invalid);           // 01101xxxxxxxxxxx

            var decode_7 = Mask(8, 4, "  0111",
                Instr(Mnemonic.jmpi, D8),
                Instr(Mnemonic.lrw, D8),
                Instr(Mnemonic.lrw, D8),
                Instr(Mnemonic.lrw, D8),

                Instr(Mnemonic.lrw, D8),
                Instr(Mnemonic.lrw, D8),
                Instr(Mnemonic.lrw, D8),
                Instr(Mnemonic.lrw, D8),

                Instr(Mnemonic.lrw, D8),
                Instr(Mnemonic.lrw, D8),
                Instr(Mnemonic.lrw, D8),
                Instr(Mnemonic.lrw, D8),

                Instr(Mnemonic.lrw, D8),
                Instr(Mnemonic.lrw, D8),
                Instr(Mnemonic.lrw, D8),
                Instr(Mnemonic.jsri, D8));

            var decode_8 = Instr(Mnemonic.ld, R8, MdispW);  
            var decode_9 = Instr(Mnemonic.st, R8, MdispW);  
            var decode_A = Instr(Mnemonic.ld_b, R8, MdispB);
            var decode_B = Instr(Mnemonic.st_b, R8, MdispB);
            var decode_C = Instr(Mnemonic.ld_h, R8, MdispH);
            var decode_D = Instr(Mnemonic.st_h, R8, MdispH);
            var decode_E = Mask(11, 1, "  1110",
                Instr(Mnemonic.bt, InstrClass.ConditionalTransfer, D11),
                Instr(Mnemonic.bf, InstrClass.ConditionalTransfer, D11));
            var decode_F = Mask(11, 1, "  1111",
                Instr(Mnemonic.br, InstrClass.Transfer, D11),
                Instr(Mnemonic.bsr, InstrClass.Transfer | InstrClass.Call, D11));

            rootDecoder = Mask(12, 4, "MCore",
                decode_0,
                decode_1,
                decode_2,
                decode_3,
                decode_4,
                decode_5,
                decode_6,
                decode_7,
                decode_8,
                decode_9,
                decode_A,
                decode_B,
                decode_C,
                decode_D,
                decode_E,
                decode_F);
        }
    }
}