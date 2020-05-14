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
using Reko.Core.CLanguage;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;

namespace Reko.Arch.Avr.Avr32
{
    using Decoder = Reko.Core.Machine.Decoder<Avr32Disassembler, Mnemonic, Avr32Instruction>;

    public class Avr32Disassembler : DisassemblerBase<Avr32Instruction, Mnemonic>
    {
        private static readonly Decoder rootDecoder;

        private readonly Avr32Architecture arch;
        private readonly EndianImageReader rdr;
        private readonly List<MachineOperand> ops;
        private Address addr;
        private InstrClass iclass;

        public Avr32Disassembler(Avr32Architecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.ops = new List<MachineOperand>();
        }

        public override Avr32Instruction DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadBeUInt16(out ushort uInstr))
                return null;
            this.iclass = InstrClass.None;
            ops.Clear();
            var instr = rootDecoder.Decode(uInstr, this);
            instr.Address = addr;
            instr.Length = (int) (rdr.Address - addr);
            return instr;
        }

        public override Avr32Instruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            return new Avr32Instruction
            {
                InstructionClass = (this.iclass != InstrClass.None)
                    ? this.iclass
                    : iclass,
                Mnemonic = mnemonic,
                Operands = ops.ToArray()
            };
        }

        public override Avr32Instruction CreateInvalidInstruction()
        {
            return new Avr32Instruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.invalid,
                Operands = new MachineOperand[0],
            };
        }

        public override Avr32Instruction NotYetImplemented(uint wInstr, string message)
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingDecoder("Avr32Dis", this.addr, rdr, message);
            return CreateInvalidInstruction();
        }

        #region Mutators
        private static Mutator<Avr32Disassembler> Register(int bitpos)
        {
            var bitfield = new Bitfield(bitpos, 4);
            return (u, d) =>
            {
                var iReg = bitfield.Read(u);
                var reg = Registers.GpRegisters[iReg];
                d.ops.Add(new RegisterOperand(reg));
                return true;
            };
        }
        private static readonly Mutator<Avr32Disassembler> R0 = Register(0);
        private static readonly Mutator<Avr32Disassembler> R9 = Register(9);
        private static readonly Mutator<Avr32Disassembler> R16 = Register(16);
        private static readonly Mutator<Avr32Disassembler> R25 = Register(25);


        private static Mutator<Avr32Disassembler> RegisterWrite(int bitpos)
        {
            var bitfield = new Bitfield(bitpos, 4);
            return (u, d) =>
            {
                var iReg = bitfield.Read(u);
                if (iReg == 0xF)
                    d.iclass = InstrClass.Transfer;
                var reg = Registers.GpRegisters[iReg];
                d.ops.Add(new RegisterOperand(reg));
                return true;
            };
        }
        private static readonly Mutator<Avr32Disassembler> Rw0 = RegisterWrite(0);
        private static readonly Mutator<Avr32Disassembler> Rw16 = RegisterWrite(16);


        private static Mutator<Avr32Disassembler> MunsignedDisplacement(PrimitiveType dt, int regPos, int bitpos, int length)
        {
            var regField = new Bitfield(regPos, 4);
            var bitfield = new Bitfield(bitpos, length);
            return (u, d) =>
            {
                var iReg = regField.Read(u);
                var reg = Registers.GpRegisters[iReg];
                var disp = bitfield.ReadSigned(u) * dt.Size;
                d.ops.Add(MemoryOperand.Displaced(dt, reg, disp));
                return true;
            };
        }

        private static readonly Mutator<Avr32Disassembler> Mdisp_w0_16 = MunsignedDisplacement(PrimitiveType.Word32, 16, 0, 16);
        private static readonly Mutator<Avr32Disassembler> Mdisp_w4_5 = MunsignedDisplacement(PrimitiveType.Word32, 9, 4, 5);
        private static readonly Mutator<Avr32Disassembler> Mdisp_w4_4 = MunsignedDisplacement(PrimitiveType.Word32, 9, 4, 4);
        private static readonly Mutator<Avr32Disassembler> Mdisp_ub4_3 = MunsignedDisplacement(PrimitiveType.Byte, 9, 4, 3);
        private static readonly Mutator<Avr32Disassembler> Mdisp_sh4_3 = MunsignedDisplacement(PrimitiveType.Int16, 9, 4, 3);
        private static readonly Mutator<Avr32Disassembler> Mdisp_uh4_3 =  MunsignedDisplacement(PrimitiveType.UInt16, 9, 4, 3);

        private static Mutator<Avr32Disassembler> MsignedDisplacement(PrimitiveType dt, int regPos, int bitpos, int length)
        {
            var regField = new Bitfield(regPos, 4);
            var bitfield = new Bitfield(bitpos, length);
            return (u, d) =>
            {
                var iReg = regField.Read(u);
                var reg = Registers.GpRegisters[iReg];
                var disp = bitfield.ReadSigned(u);
                d.ops.Add(MemoryOperand.Displaced(dt, reg, disp));
                return true;
            };
        }

        private static readonly Mutator<Avr32Disassembler> Msdisp_w0_16 = MsignedDisplacement(PrimitiveType.Word32, 16, 0, 16);


        private static Mutator<Avr32Disassembler> Mpost(PrimitiveType dt, int bitpos)
        {
            var bitfield = new Bitfield(bitpos, 4);
            return (u, d) =>
            {
                var iReg = bitfield.Read(u);
                var reg = Registers.GpRegisters[iReg];
                d.ops.Add(MemoryOperand.PostInc(dt, reg));
                return true;
            };
        }

        private static readonly Mutator<Avr32Disassembler> Mpost_w9 = Mpost(PrimitiveType.Word32, 9);

        private static Mutator<Avr32Disassembler> Mpre(PrimitiveType dt, int bitpos)
        {
            var bitfield = new Bitfield(bitpos, 4);
            return (u, d) =>
            {
                var iReg = bitfield.Read(u);
                var reg = Registers.GpRegisters[iReg];
                d.ops.Add(MemoryOperand.PreDec(dt, reg));
                return true;
            };
        }

        private static readonly Mutator<Avr32Disassembler> Mpre_w9 = Mpre(PrimitiveType.Word32, 9);


        private static Mutator<Avr32Disassembler> Mrelative(RegisterStorage baseReg, int bitpos, int length)
        {
            var bitfield = new Bitfield(bitpos, length);
            return (u, d) =>
            {
                var iOffset = (int) bitfield.Read(u) << 2;
                var mem = MemoryOperand.Displaced(PrimitiveType.Word32, baseReg, iOffset);
                d.ops.Add(mem);
                return true;
            };
        }

        private static readonly Mutator<Avr32Disassembler> Mpc4_7 = Mrelative(Registers.pc, 4, 7);
        private static readonly Mutator<Avr32Disassembler> Msp4_7 = Mrelative(Registers.sp, 4, 7);

        private static Mutator<Avr32Disassembler> Imm_signed(int bitPos, int length)
        {
            var bitfield = new Bitfield(bitPos, length);
            return (u, d) =>
            {
                var imm = bitfield.ReadSigned(u);
                d.ops.Add(ImmediateOperand.Word32(imm));
                return true;
            };
        }

        private static readonly Mutator<Avr32Disassembler> Is0_16 = Imm_signed(0, 16);
        private static readonly Mutator<Avr32Disassembler> Is4_6 = Imm_signed(4, 6);
        private static readonly Mutator<Avr32Disassembler> Is4_8 = Imm_signed(4, 8);

        private static Mutator<Avr32Disassembler> Imm_unsigned(PrimitiveType dt, int bitPos, int length)
        {
            var bitfield = new Bitfield(bitPos, length);
            return (u, d) =>
            {
                var imm = bitfield.Read(u);
                d.ops.Add(new ImmediateOperand(Constant.Create(dt, imm)));
                return true;
            };
        }

        private static readonly Mutator<Avr32Disassembler> Iu_h0_16 = Imm_unsigned(PrimitiveType.UInt16, 0, 16);


        private static Mutator<Avr32Disassembler> multiRegisterList(bool pop, bool setr12)
        {
            return (uint uInstr, Avr32Disassembler dasm) =>
            {
                var regMask = Bits.ZeroExtend(uInstr >> 4, 8);
                if (regMask == 0)
                    return false;
                var gp = Registers.GpRegisters;
                if ((regMask & 1) != 0)
                {
                    dasm.ops.Add(new RegisterRange(gp, 0, 4));
                }
                if ((regMask & 2) != 0)
                {
                    dasm.ops.Add(new RegisterRange(gp, 4, 4));
                }
                if ((regMask & 4) != 0)
                {
                    dasm.ops.Add(new RegisterRange(gp, 8, 2));
                }
                if ((regMask & 8) != 0)
                {
                    dasm.ops.Add(new RegisterOperand(gp[10]));
                }
                if ((regMask & 16) != 0)
                {
                    dasm.ops.Add(new RegisterOperand(gp[11]));
                }
                if ((regMask & 32) != 0)
                {
                    dasm.ops.Add(new RegisterOperand(gp[12]));
                }
                if ((regMask & 64) != 0 && !(pop && setr12))
                {
                    dasm.ops.Add(new RegisterOperand(gp[14]));
                }
                if ((regMask & 128) != 0)
                {
                    if (pop)
                        dasm.iclass = InstrClass.Transfer;
                    dasm.ops.Add(new RegisterOperand(gp[15]));
                    if (setr12)
                    {
                        switch (Bits.ZeroExtend(regMask >> 5, 2))
                        {
                        case 0: dasm.ops.Add(new AssignOperand(gp[12], 0)); break;
                        case 1: dasm.ops.Add(new AssignOperand(gp[12], 1)); break;
                        default: dasm.ops.Add(new AssignOperand(gp[12], -1)); break;
                        }
                    }
                }
                return true;
            };
        }

        private static Mutator<Avr32Disassembler> PCRelative(Bitfield bfOffset)
        {
            return (u, d) =>
            {
                var offset = bfOffset.ReadSigned(u) << 1;
                var addr = d.addr + offset;
                d.ops.Add(AddressOperand.Create(addr));
                return true;
            };
        }

        private static readonly Mutator<Avr32Disassembler> Pcrel4_8 = PCRelative(new Bitfield(4,8));

        #endregion

        #region Decoders

        private class UInt32Decoder : Decoder
        {
            private readonly Decoder decoder;

            public UInt32Decoder(Decoder decoder)
            {
                this.decoder = decoder;
            }

            public override Avr32Instruction Decode(uint wInstr, Avr32Disassembler dasm)
            {
                if (!dasm.rdr.TryReadBeUInt16(out ushort uLow))
                    return null;
                uint wInstrLong = (wInstr << 16) | uLow;
                return decoder.Decode(wInstrLong, dasm);
            }
        }

        private static Decoder Nyi(string message)
        {
            return new NyiDecoder<Avr32Disassembler, Mnemonic, Avr32Instruction>(message);
        }

        #endregion

        static Avr32Disassembler()
        {
            var ld_ub = Instr(Mnemonic.ld_ub, Mdisp_ub4_3);
            var twoRegs = Mask(4, 5, "  twoRegs",
                Nyi("0b00000"),
                Nyi("0b00001"),
                Instr(Mnemonic.rsub, Rw0, R9),
                Instr(Mnemonic.cp_w, R0, R9),
                Nyi("0b00100"),
                Instr(Mnemonic.eor, Rw0, R9),
                Nyi("0b00110"),
                Nyi("0b00111"),
                Nyi("0b01000"),
                Instr(Mnemonic.mov, Rw0, R9),
                Nyi("0b01010"),
                Nyi("0b01011"),
                Nyi("0b01100"),
                Instr(Mnemonic.st_w, Mpre_w9, R0),
                Nyi("0b01110"),
                Nyi("0b01111"),
                Instr(Mnemonic.ld_w, Rw0, Mpost_w9),
                Nyi("0b10001"),
                Nyi("0b10010"),
                Nyi("0b10011"),
                Nyi("0b10100"),
                Nyi("0b10101"),
                Nyi("0b10110"),
                Nyi("0b10111"),

                ld_ub,
                ld_ub,
                ld_ub,
                ld_ub,

                ld_ub,
                ld_ub,
                ld_ub,
                ld_ub);

            var dispLdStK3imm = Mask(7, 2, "  Displacement load / store with k3 immediate",
                Instr(Mnemonic.ld_sh, Rw0, Mdisp_sh4_3),
                Instr(Mnemonic.ld_uh, Rw0, Mdisp_uh4_3),
                Instr(Mnemonic.st_w, Mdisp_w4_4, R0),
                Instr(Mnemonic.st_w, Mdisp_w4_4, R0));

            var singleReg = Sparse(4, 5, "Single register instructions", Nyi("Single register instructions"),
                (0b10001, Instr(Mnemonic.icall, R0)));

            var spPcRelativeLdSt = Mask(11, 2, "  spPcRelativeLdSt",
                Instr(Mnemonic.lddsp, Rw0, Msp4_7),
                Instr(Mnemonic.lddpc, Rw0, Mpc4_7),
                Instr(Mnemonic.stdsp, Msp4_7, R0),
                Mask(9, 2, "  11",
                    Instr(Mnemonic.cp_w, R0, Is4_6),
                    Instr(Mnemonic.cp_w, R0, Is4_6),
                    singleReg,
                    Nyi("return and test")));
                    //Nyi("1")););

            var wide00000 = Sparse(8, 8, "  wide00000", Nyi(""),
                (0x18, Sparse(0, 8, "  0x18", Nyi(""),
                    (0x00, Instr(Mnemonic.cp_b, R16, R25)))));
            var wide00001 = Mask(25, 4, "  00001",
                Nyi("0b0000"),
                Nyi("0b0001"),
                Nyi("0b0010"),
                Nyi("0b0011"),
                Nyi("0b0100"),
                Nyi("0b0101"),
                Nyi("0b0110"),
                Nyi("0b0111"),
                Instr(Mnemonic.mcall, Mdisp_w0_16),
                Nyi("0b1001"),
                Nyi("0b1010"),
                Nyi("0b1011"),
                Nyi("0b1100"),
                Nyi("0b1101"),
                Instr(Mnemonic.movh, Iu_h0_16),
                Nyi("0b1111"));

            var wideInstr = new UInt32Decoder(Mask(20, 5, "  wide",
                wide00000,
                wide00001,
                Nyi("0b00010"),
                Nyi("0b00011"),
                Nyi("0b00100"),
                Nyi("0b00101"),
                Nyi("0b00110"),
                Nyi("0b00111"),
                Nyi("0b01000"),
                Nyi("0b01001"),
                Nyi("0b01010"),
                Nyi("0b01011"),
                Instr(Mnemonic.sub, Rw16, R25, Is0_16),
                Nyi("0b01101"),
                Nyi("0b01110"),
                Instr(Mnemonic.ld_w, Rw16, Msdisp_w0_16),
                Nyi("0b10000"),
                Nyi("0b10001"),
                Nyi("0b10010"),
                Nyi("0b10011"),
                Nyi("0b10100"),
                Nyi("0b10101"),
                Nyi("0b10110"),
                Nyi("0b10111"),
                Nyi("0b11000"),
                Nyi("0b11001"),
                Nyi("0b11010"),
                Nyi("0b11011"),
                Nyi("0b11100"),
                Nyi("0b11101"),
                Nyi("0b11110"),
                Nyi("0b11111")));

            var shortBranch = Mask(0, 3, "  Short branch",
                Instr(Mnemonic.breq, InstrClass.ConditionalTransfer, Pcrel4_8),
                Instr(Mnemonic.brne, InstrClass.ConditionalTransfer, Pcrel4_8),
                Instr(Mnemonic.brcc, InstrClass.ConditionalTransfer, Pcrel4_8),
                Instr(Mnemonic.brcs, InstrClass.ConditionalTransfer, Pcrel4_8),

                Instr(Mnemonic.brge, InstrClass.ConditionalTransfer, Pcrel4_8),
                Instr(Mnemonic.brlt, InstrClass.ConditionalTransfer, Pcrel4_8),
                Instr(Mnemonic.brmi, InstrClass.ConditionalTransfer, Pcrel4_8),
                Instr(Mnemonic.brpl, InstrClass.ConditionalTransfer, Pcrel4_8));
            
            rootDecoder = Mask(13, 3, "AVR32",
                twoRegs,
                Mask(12, 1, "  001",
                    Nyi("0"),
                    Instr(Mnemonic.mov, Rw0, Is4_8)),
                spPcRelativeLdSt,
                Instr(Mnemonic.ld_w, Rw0, Mdisp_w4_5),
                dispLdStK3imm,
                Mask(7, 2, "  101",
                    Nyi("0b00"),
                    Instr(Mnemonic.st_b, Mdisp_ub4_3, R0),
                    Nyi("0b10"),
                    Nyi("0b11")),
                Mask(12, 1, "  110",
                    Mask(3, 1, "  0",
                        shortBranch,
                        Nyi("Relative jump and call")),
                    Mask(0, 4, "  1",
                        Nyi("acall"),
                        Instr(Mnemonic.pushm, multiRegisterList(false, false)),
                        Instr(Mnemonic.popm, multiRegisterList(true, false)),
                        Nyi("0b0011"),
                        Nyi("0b0100"),
                        Nyi("0b0101"),
                        Nyi("0b0110"),
                        Nyi("0b0111"),
                        Nyi("0b1000"),
                        Nyi("0b1001"),
                        Instr(Mnemonic.popm, multiRegisterList(true, true)),
                        Nyi("0b1011"),
                        Nyi("0b1100"),
                        Nyi("0b1101"),
                        Nyi("0b1110"),
                        Nyi("0b1111"))),
                wideInstr);
        }
    }
}