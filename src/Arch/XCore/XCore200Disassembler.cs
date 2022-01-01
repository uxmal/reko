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

#pragma warning disable IDE1006

using System.Collections.Generic;
using System.Linq;
using Reko.Core;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Services;

namespace Reko.Arch.XCore
{
    using Decoder = Decoder<XCore200Disassembler, Mnemonic, XCoreInstruction>;

    public class XCore200Disassembler : DisassemblerBase<XCoreInstruction, Mnemonic>
    {
        private static readonly Decoder rootDecoder;

        private readonly XCore200Architecture arch;
        private readonly EndianImageReader rdr;
        private readonly List<MachineOperand> ops;
        private Address addr;

        public XCore200Disassembler(XCore200Architecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.ops = new List<MachineOperand>();
            this.addr = null!;
        }

        public override XCoreInstruction? DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadLeUInt16(out ushort uInstr))
                return null;
            ops.Clear();
            var instr = rootDecoder.Decode(uInstr, this);
            instr.Address = addr;
            instr.Length = (int) (rdr.Address - addr);
            return instr;
        }

        public override XCoreInstruction CreateInvalidInstruction()
        {
            return new XCoreInstruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.Invalid,
                Operands = new MachineOperand[0]
            };
        }

        public override XCoreInstruction NotYetImplemented(string message)
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingDecoder("XCoreDis", this.addr, this.rdr, message);
            return CreateInvalidInstruction();
        }

        #region Mutators

        // Three register encoding.
        private static bool r3(uint uInstr, XCore200Disassembler dasm)
        {
            // Get low bits first.
            var iReg3 = uInstr & 0x03;
            var iReg2 = (uInstr >> 2) & 0x03;
            var iReg1 = (uInstr >> 4) & 0x03;
            // Extract base3 field.

            var base3 = (uInstr >> 6) & 0x1F;
            iReg3 += (base3 % 3) << 2;
            base3 = base3 / 3;
            iReg2 += (base3 % 3) << 2;
            base3 = base3 / 3;
            iReg1 += (base3 % 3) << 2;
            if (iReg1 >= 12)
                return false;
            dasm.ops.Add(new RegisterOperand(Registers.GpRegs[iReg1]));
            dasm.ops.Add(new RegisterOperand(Registers.GpRegs[iReg2]));
            dasm.ops.Add(new RegisterOperand(Registers.GpRegs[iReg3]));
            return true;
        }

        // Three register long.
        private static bool l3r(uint uInstr, XCore200Disassembler dasm)
        {
            return false;
        }

        // Two register with immediate.
        private static bool r2us(uint uInstr, XCore200Disassembler dasm)
        {
            // Get low bits first.
            var imm3 = uInstr & 0x03;
            var iReg2 = (uInstr >> 2) & 0x03;
            var iReg1 = (uInstr >> 4) & 0x03;
            // Extract base3 field.

            var base3 = (uInstr >> 6) & 0x1F;
            imm3 += (base3 % 3) << 2;
            base3 = base3 / 3;
            iReg2 += (base3 % 3) << 2;
            base3 = base3 / 3;
            iReg1 += (base3 % 3) << 2;
            if (iReg1 >= 12)
                return false;
            dasm.ops.Add(new RegisterOperand(Registers.GpRegs[iReg1]));
            dasm.ops.Add(new RegisterOperand(Registers.GpRegs[iReg2]));
            dasm.ops.Add(ImmediateOperand.Word32(imm3));
            return true;
        }

        // Two register with immediate translated to bitpos
        private static readonly uint[] bitpos32 =
        {
            32, 1, 2, 3,  4, 5, 6, 7,  8, 16, 24, 32
        };
        private static readonly uint[] bitpos64 =
        {
            32, 1, 2, 3,  4, 5, 6, 7,  8, 16, 24, 32
        };
        private static bool r2us_bitp(uint uInstr, XCore200Disassembler dasm)
        {
            // Get low bits first.
            var imm3 = uInstr & 0x03;
            var iReg2 = (uInstr >> 2) & 0x03;
            var iReg1 = (uInstr >> 4) & 0x03;
            // Extract base3 field.

            var base3 = (uInstr >> 6) & 0x1F;
            imm3 += (base3 % 3) << 2;
            base3 = base3 / 3;
            iReg2 += (base3 % 3) << 2;
            base3 = base3 / 3;
            iReg1 += (base3 % 3) << 2;
            if (iReg1 >= 12)
                return false;
            imm3 = bitpos32[imm3];
            dasm.ops.Add(new RegisterOperand(Registers.GpRegs[iReg1]));
            dasm.ops.Add(new RegisterOperand(Registers.GpRegs[iReg2]));
            dasm.ops.Add(ImmediateOperand.Word32(imm3));
            return true;
        }

        // Two register with immediate long.
        private static bool l2rus(uint uInstr, XCore200Disassembler dasm)
        {
            return false;
        }

        //Register with 6-bit immediate
        private static bool ru6(uint uInstr, XCore200Disassembler dasm)
        {
            var iReg = (uInstr >> 6) & 0xF;
            var imm = (uInstr & 0x3F);
            dasm.ops.Add(new RegisterOperand(Registers.GpRegs[iReg]));
            dasm.ops.Add(ImmediateOperand.Word32(imm));
            return true;
        }

        //Register with 6-bit immediate added to PC
        private static bool ru6_p_pc(uint uInstr, XCore200Disassembler dasm)
        {
            var iReg = (uInstr >> 6) & 0xF;
            var imm = (uInstr & 0x3F) << 1;
            var addr = dasm.addr + imm;
            dasm.ops.Add(new RegisterOperand(Registers.GpRegs[iReg]));
            dasm.ops.Add(AddressOperand.Create(addr));
            return true;
        }

        //Register with 6-bit immediate subtracted from PC
        private static bool ru6_m_pc(uint uInstr, XCore200Disassembler dasm)
        {
            var iReg = (uInstr >> 6) & 0xF;
            var imm = uInstr & 0x3F;
            if ((imm & 1) != 0)
                return false;       // Illegal address.
            var addr = dasm.addr - imm;
            dasm.ops.Add(new RegisterOperand(Registers.GpRegs[iReg]));
            dasm.ops.Add(AddressOperand.Create(addr));
            return true;
        }

        //Register with 16-bit immediate
        private static bool lru6(uint uInstr, XCore200Disassembler dasm)
        {
            return false;
        }

        // 6-bit immediate
        private static bool u6(uint uInstr, XCore200Disassembler dasm)
        {
            var u = uInstr & 0x3F;
            dasm.ops.Add(ImmediateOperand.Word32(u));
            return false;
        }

        // 6-bit pc-relative positive displacement
        private static bool u6_p_pc(uint uInstr, XCore200Disassembler dasm)
        {
            var displacement = (uInstr & Bits.Mask(0, 6)) << 1;
            var addr = dasm.addr + displacement;
            dasm.ops.Add(AddressOperand.Create(addr));
            return true;
        }

        // 6-bit pc-relative negative displacement
        private static bool u6_m_pc(uint uInstr, XCore200Disassembler dasm)
        {
            var displacement = (int)(uInstr & Bits.Mask(0, 6)) << 1;
            var addr = dasm.addr - displacement;
            dasm.ops.Add(AddressOperand.Create(addr));
            return true;
        }

        // 16-bit immediate
        private static bool lu6(uint uInstr, XCore200Disassembler dasm)
        {
            return false;
        }

        // 10-bit immediate
        private static bool u10(uint uInstr, XCore200Disassembler dasm)
        {
            return false;
        }

        // 10-bit pc-relative positive displacement
        private static bool u10_p_pc(uint uInstr, XCore200Disassembler dasm)
        {
            var displacement = (uInstr & Bits.Mask(0, 10)) << 1;
            var addr = dasm.addr + displacement;
            dasm.ops.Add(AddressOperand.Create(addr));
            return true;
        }

        // 10-bit pc-relative negative displacement
        private static bool u10_m_pc(uint uInstr, XCore200Disassembler dasm)
        {
            var displacement = (int)(uInstr & Bits.Mask(0, 10)) << 1;
            var addr = dasm.addr - displacement;
            dasm.ops.Add(AddressOperand.Create(addr));
            return true;
        }

        // PC-relative
        private static readonly Bitfield pcRe11Field = new Bitfield(0, 11);
        private static bool PcRel11_2(uint uInstr, XCore200Disassembler dasm)
        {
            var offset = pcRe11Field.ReadSigned(uInstr);
            var addr = dasm.addr + 2 * offset;
            dasm.ops.Add(AddressOperand.Create(addr));
            return true;
        }

        // 20-bit immediate.
        private static bool lu10(uint uInstr, XCore200Disassembler dasm)
        {
            return false;
        }

        // two register.
        private static bool r2(uint uInstr, XCore200Disassembler dasm)
        {
            var base3 = (uInstr >> 6) & 0x1F;
            if (base3 < 27)
                return false;
            if (Bits.IsBitSet(uInstr, 5))
            {
                if (base3 == 31)
                    return false;
                base3 += 5;
            }
            base3 -= 27;
            var iReg2 = uInstr & 0x03;
            var iReg1 = (uInstr >> 2) & 0x03;

            iReg2 += (base3 % 3) << 2;
            base3 = base3 / 3;
            iReg1 += (base3 % 3) << 2;

            dasm.ops.Add(new RegisterOperand(Registers.GpRegs[iReg1]));
            dasm.ops.Add(new RegisterOperand(Registers.GpRegs[iReg2]));
            return true;
        }

        // two register reversed.
        private static bool r2r(uint uInstr, XCore200Disassembler dasm)
        {
            var base3 = (uInstr >> 6) & 0x1F;
            if (base3 < 27)
                return false;
            if (Bits.IsBitSet(uInstr, 5))
            {
                if (base3 == 31)
                    return false;
                base3 += 5;
            }
            base3 -= 27;
            var iReg2 = uInstr & 0x03;
            var iReg1 = (uInstr >> 2) & 0x03;

            iReg2 += (base3 % 3) << 2;
            base3 = base3 / 3;
            iReg1 += (base3 % 3) << 2;

            dasm.ops.Add(new RegisterOperand(Registers.GpRegs[iReg2]));
            dasm.ops.Add(new RegisterOperand(Registers.GpRegs[iReg1]));
            return true;
        }

        // two register long.
        private static bool l2r(uint uInstr, XCore200Disassembler dasm)
        {
            return false;
        }

        // Two register reversed long.
        private static bool lr2r(uint uInstr, XCore200Disassembler dasm)
        {
            return false;
        }

        // Register with immediate.
        private static bool rus(uint uInstr, XCore200Disassembler dasm)
        {
            var base3 = (uInstr >> 6) & 0x1F;
            if (base3 < 27)
                return false;
            if (Bits.IsBitSet(uInstr, 5))
            {
                if (base3 == 31)
                    return false;
                base3 += 5;
            }
            base3 -= 27;
            var imm = uInstr & 0x03;
            var iReg = (uInstr >> 2) & 0x03;

            imm += (base3 % 3) << 2;
            base3 = base3 / 3;
            iReg += (base3 % 3) << 2;
            if (iReg >= 12)
                return false;
            dasm.ops.Add(new RegisterOperand(Registers.GpRegs[iReg]));
            dasm.ops.Add(new RegisterOperand(Registers.GpRegs[imm]));
            return true;
        }

        // Register with immediate.
        private static bool rus_bitp(uint uInstr, XCore200Disassembler dasm)
        {
            var base3 = (uInstr >> 6) & 0x1F;
            if (base3 < 27)
                return false;
            if (Bits.IsBitSet(uInstr, 5))
            {
                if (base3 == 31)
                    return false;
                base3 += 5;
            }
            base3 -= 27;
            var imm = uInstr & 0x03;
            var iReg = (uInstr >> 2) & 0x03;

            imm += (base3 % 3) << 2;
            base3 = base3 / 3;
            iReg += (base3 % 3) << 2;
            if (iReg >= 12 || imm >= bitpos32.Length)
                return false;
            imm = bitpos32[imm];
            dasm.ops.Add(new RegisterOperand(Registers.GpRegs[iReg]));
            dasm.ops.Add(ImmediateOperand.Word32(imm));
            return true;
        }

        // Register.
        private static bool r1(uint uInstr, XCore200Disassembler dasm)
        {
            var iReg = uInstr & 0xF;
            dasm.ops.Add(new RegisterOperand(Registers.GpRegs[iReg]));
            return true;
        }

        // Specific register
        private static Mutator<XCore200Disassembler> Reg(RegisterStorage reg)
        {
            var op = new RegisterOperand(reg);
            return (u, d) =>
            {
                d.ops.Add(op);
                return true;
            };
        }

        // No operands.
        private static bool r0(uint uInstr, XCore200Disassembler dasm)
        {
            return false;
        }

        // No operands long.
        private static bool l0r(uint uInstr, XCore200Disassembler dasm)
        {
            return false;
        }

        // Four registers long.
        private static bool l4r(uint uInstr, XCore200Disassembler dasm)
        {
            return false;
        }

        // Three registers with immediate long.
        private static bool l3rus(uint uInstr, XCore200Disassembler dasm)
        {
            return false;
        }

        // Four registers with immediate long.
        private static bool l4rus(uint uInstr, XCore200Disassembler dasm)
        {
            return false;
        }

        // Five registers long.
        private static bool l5r(uint uInstr, XCore200Disassembler dasm)
        {
            return false;
        }

        // Six registers long.
        private static bool l6r(uint uInstr, XCore200Disassembler dasm)
        {
            return false;
        }

        #endregion

        #region Decoders

        private class W32Decoder :Decoder
        {
            private readonly Decoder subDecoder;

            public W32Decoder(Decoder subDecoder)
            {
                this.subDecoder = subDecoder;
            }

            public override XCoreInstruction Decode(uint wInstr, XCore200Disassembler dasm)
            {
                if (!dasm.rdr.TryReadLeUInt16(out ushort u2))
                    return dasm.CreateInvalidInstruction();
                wInstr = (wInstr << 16) | u2;
                return subDecoder.Decode(wInstr, dasm);
            }
        }

        private class InstrDecoder : Decoder
        {
            private readonly InstrClass iclass;
            private readonly Mnemonic mnemonic;
            private readonly Mutator<XCore200Disassembler>[] mutators;

            public InstrDecoder(InstrClass iclass, Mnemonic mnemonic, params Mutator<XCore200Disassembler>[] mutators)
            {
                this.iclass = iclass;
                this.mnemonic = mnemonic;
                this.mutators = mutators;
            }

            public override XCoreInstruction Decode(uint wInstr, XCore200Disassembler dasm)
            {
                foreach (var m in mutators)
                {
                    if (!m(wInstr, dasm))
                        return dasm.CreateInvalidInstruction();
                }
                var instr = new XCoreInstruction
                {
                    InstructionClass = iclass,
                    Mnemonic = mnemonic,
                    Operands = dasm.ops.ToArray()
                };
                return instr;
            }
        }

        private static InstrDecoder Instr(Mnemonic mnemonic, params Mutator<XCore200Disassembler> [] mutators)
        {
            return new InstrDecoder(InstrClass.Linear, mnemonic, mutators);
        }

        private static InstrDecoder Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<XCore200Disassembler>[] mutators)
        {
            return new InstrDecoder(iclass, mnemonic, mutators);
        }

        private static Decoder Nyi(string msg)
        {
            return new NyiDecoder<XCore200Disassembler, Mnemonic, XCoreInstruction>(msg);
        }

        #endregion

        static XCore200Disassembler()
        {
            var invalid = Instr(Mnemonic.Invalid, InstrClass.Invalid);


            bool is_0x1F(uint u) => u == 0x1F;
            bool is_lt27(uint u) => u < 27;


            var longDecoder = Mask(11, 5, "  long",
                Nyi("00"),
                Nyi("01"),
                Nyi("02"),
                Nyi("03"),

                Nyi("04"),
                Nyi("05"),
                Nyi("06"),
                Nyi("07"),

                Nyi("08"),
                Nyi("09"),
                Nyi("0A"),
                Nyi("0B"),

                Nyi("0C"),
                Nyi("0D"),
                Nyi("0E"),
                Nyi("0F"),
                // 10
                Nyi("10"),
                Nyi("11"),
                Nyi("12"),
                Nyi("13"),
                
                Nyi("14"),
                Nyi("15"),
                Nyi("16"),
                Nyi("17"),
                
                Nyi("18"),
                Nyi("19"),
                Nyi("1A"),
                Nyi("1B"),

                Nyi("1C"),
                Nyi("1D"),
                Nyi("1E"),
                Sparse(11, 5, "  long 1F", Nyi(""),
                    (2, Instr(Mnemonic.ashr, l3r))));

            var decode_0F_0 = Nyi("0F_0");
            var decode_0F_1 = Nyi("0F_1");

            rootDecoder = Mask(11, 5, "XCore",
                Select((6, 5), is_lt27,
                    Instr(Mnemonic.stwi, r2us),
                    Mask(4, 2, "  00",
                        Instr(Mnemonic.byterev, r2),
                        Select((6, 5), is_0x1F,
                            Nyi("00 01  11111"),
                            Instr(Mnemonic.getst, r2)),
                        Select((6, 5), is_0x1F,
                            Sparse(0, 4, Instr(Mnemonic.edu, r1),
                                (0xC, Instr(Mnemonic.waiteu)),
                                (0xD, Instr(Mnemonic.clre)),
                                (0xE, Instr(Mnemonic.ssync)),
                                (0xF, Instr(Mnemonic.freet))),
                            Instr(Mnemonic.getst, r2)),
                        Select((6, 5), is_0x1F,
                            Sparse(0, 4, Instr(Mnemonic.eeu, r1),
                                (0xD, Instr(Mnemonic.dcall)), 
                                (0xF, Instr(Mnemonic.setkep))), 
                            Instr(Mnemonic.getst, r2)))),
                Select((6, 5), is_lt27, "  01",
                    Instr(Mnemonic.ldwi, r2us),
                    Mask(4, 2, "  01",
                        Instr(Mnemonic.clz, r2),
                        Select((6, 5), is_0x1F, "  01 01",
                            Nyi("01 01 0x1F"),
                            Instr(Mnemonic.outt, r2r)),
                        Select((6, 5), is_0x1F, "  01 10",
                            Sparse(0, 4, "  01 10 0x1F",
                                Instr(Mnemonic.waitet, r1),
                                (0xC, Instr(Mnemonic.ldspc)),
                                (0xD, Instr(Mnemonic.stspc)),
                                (0xE, Instr(Mnemonic.stssr)),
                                (0xF, Instr(Mnemonic.ldssr))),
                            Instr(Mnemonic.clz, r2)),
                        Select((6, 5), is_0x1F, "  01 11",
                            Sparse(0, 4, "  01 11 0x1F", 
                                Instr(Mnemonic.waitef, r1),
                                (0xC, Instr(Mnemonic.stsed)),
                                (0xD, Instr(Mnemonic.stet)),
                                (0xE, Instr(Mnemonic.geted)),
                                (0xF, Instr(Mnemonic.getet))),
                            Instr(Mnemonic.outt, r2r)))),
                Select((6, 5), is_lt27, "  02",
                    Instr(Mnemonic.add, r3),
                    Mask(4, 1, "  02 >=27",
                        Select((6, 5), is_0x1F, "  02 0",
                            Sparse(0, 4, "  02 0 0x1F",
                                Instr(Mnemonic.freer, r1),
                                (0xC, Instr(Mnemonic.getps)),
                                (0xD, invalid),
                                (0xE, Instr(Mnemonic.getid)),
                                (0xF, Instr(Mnemonic.getkep))),
                            Instr(Mnemonic.bitrev, r2)),
                        Select((6, 5), is_0x1F, "  02 1",
                            Sparse(0, 4, "  02 1 0x1F",
                                Instr(Mnemonic.mjoin, r1),
                                (0xC, Instr(Mnemonic.getksp)),
                                (0xD, Instr(Mnemonic.ldsed)),
                                (0xE, Instr(Mnemonic.ldset)),
                                (0xF, Instr(Mnemonic.nop))),
                            Instr(Mnemonic.setd, r2r)))),
                Select((6, 5), is_lt27, "  03",
                    Instr(Mnemonic.sub, r3),
                    Mask(4, 1, "  03 >= 27",
                        Select((6, 5), is_0x1F, "  03 0",
                            Sparse(0, 4, "  03 0 0x1F",
                                Instr(Mnemonic.tstart, r1),
                                (0xC, Nyi("03 0 0x1F 0xC")),
                                (0xD, Nyi("03 0 0x1F 0xD")),
                                (0xE, Nyi("03 0 0x1F 0xE")),
                                (0xF, Nyi("03 0 0x1F 0xF"))),
                            Nyi("  03 0 !0x1F")),
                        Select((6, 5), is_0x1F, "  03 0",
                            Sparse(0, 4, "  03 1 0x1F",
                                Instr(Mnemonic.msync, r1),
                                (0xC, Nyi("03 1 0x1F 0xC")),
                                (0xD, Nyi("03 1 0x1F 0xD")),
                                (0xE, Nyi("03 1 0x1F 0xE")),
                                (0xF, Nyi("03 1 0x1F 0xF"))),
                            Nyi("  03 0 !0x1F")))),

                Mask(4, 1, "  04",
                    Select((5, 6), u => u == 0x3F, "  04 - 0 0x3F",
                        Instr(Mnemonic.bla, InstrClass.Transfer|InstrClass.Call, r1),
                        Nyi("04 - 0")),
                    Select((5, 6), u => u == 0x3F, "  04 - 1 0x3F",
                        Instr(Mnemonic.bau, InstrClass.Transfer, r1),
                        Instr(Mnemonic.eet, r2))),
                Mask(4, 1, "  05",
                    Select((6, 5), u => u == 0x1F,
                        Instr(Mnemonic.bru, r1),
                        Instr(Mnemonic.andnot, r2)),
                    Instr(Mnemonic.eef, r2)),
                Instr(Mnemonic.eq, r3),
                Instr(Mnemonic.and, r3),

                Select((6, 5), is_0x1F,
                    Instr(Mnemonic.setv, r1),
                    Instr(Mnemonic.or, r3)),
                Select((10, 1), u => u == 0, "  0A",
                    Instr(Mnemonic.outct, r2),
                    Instr(Mnemonic.outcti, rus)),
                Select((10, 1), u => u == 0, "  0A",
                    Instr(Mnemonic.stwdp, ru6),
                    Instr(Mnemonic.stwsp, ru6)),
                Select((10, 1), u => u == 0, "  0B",
                    Instr(Mnemonic.ldwdp, ru6),
                    Instr(Mnemonic.ldwsp, ru6)),

                Select((10, 1), u => u == 0, "  0C",
                    Instr(Mnemonic.ldawdp, ru6),
                    Instr(Mnemonic.ldawsp, ru6)),
                Select((10, 1), u => u == 0, "  0D",
                    Instr(Mnemonic.ldc, ru6),
                    Instr(Mnemonic.ldwcp, ru6)),
                Select((10, 1), u => u == 0, "  0E",
                    Sparse(6, 4, "  0E_0",
                        Instr(Mnemonic.brft, InstrClass.ConditionalTransfer, ru6_p_pc),
                        (0xC, Instr(Mnemonic.brfu, InstrClass.Transfer, u6_p_pc)),
                        (0xD, Instr(Mnemonic.blat, InstrClass.Transfer|InstrClass.Call, u6)),
                        (0xE, Instr(Mnemonic.extdp, u6)),
                        (0xF, Instr(Mnemonic.kcalli, InstrClass.Transfer|InstrClass.Call, u6))),
                    Sparse(6, 4, "  0E_1",
                        Instr(Mnemonic.brbt, InstrClass.ConditionalTransfer, ru6_m_pc),
                        (0xC, Instr(Mnemonic.brbu, InstrClass.Transfer, u6_m_pc)),
                        (0xD, Instr(Mnemonic.entsp, u6)),
                        (0xE, Instr(Mnemonic.extsp, u6)),
                        (0xF, Instr(Mnemonic.retsp, InstrClass.Transfer | InstrClass.Return, u6)))),
                Select((10, 1), u => u == 0, "  0F",
                    Sparse(6, 4, "  0F_0", 
                        Instr(Mnemonic.brff, InstrClass.ConditionalTransfer, ru6_p_pc),
                        (0xC, Instr(Mnemonic.clrsr, u6)),
                        (0xD, Instr(Mnemonic.setsr, u6)),
                        (0xE, Instr(Mnemonic.kentsp, u6)),
                        (0xF, Nyi("0F_0 F"))),
                    Sparse(6, 4, "  0F_0",
                        Instr(Mnemonic.brbf, InstrClass.ConditionalTransfer, ru6_m_pc),
                        (0xC, Instr(Mnemonic.getsr, u6)),
                        (0xD, Instr(Mnemonic.ldawcp, u6)),
                        (0xE, Instr(Mnemonic.dualentsp, u6)),
                        (0xF, Nyi("0F_0 F")))),

                // 10
                Instr(Mnemonic.ld16s, r3),
                Instr(Mnemonic.ld8u, r3),
                Select((6, 5), is_lt27, "  12",
                    Instr(Mnemonic.addi, r2us),
                    Mask(4, 1, "  12 >=27",
                        Instr(Mnemonic.neg, r2),
                        Instr(Mnemonic.endin, r2))),
                Select((6, 5), is_lt27, "  13",
                    Instr(Mnemonic.subi, r2us),
                    Nyi("  13 >=27")),

                Select((6, 5), is_lt27,
                    Instr(Mnemonic.shli, r2us_bitp),
                    Mask(4, 1, "  14 >= 27",
                        Instr(Mnemonic.mkmsk, r2),
                        Instr(Mnemonic.mkmski, rus_bitp))),
                Select((6, 5), is_lt27,
                    Instr(Mnemonic.shri, r2us_bitp),
                    Mask(4, 1, "  15 >= 27",
                        Instr(Mnemonic.@out, r2),
                        Instr(Mnemonic.outshr, r2r))),
                Select((6, 5), is_lt27,
                    Instr(Mnemonic.eqi, r2us),
                    Mask(4, 1, "  16 >= 27",
                        Instr(Mnemonic.@in, r2),
                        Instr(Mnemonic.inshr, r2))),
                Mask(4, 1, "  17",
                    Instr(Mnemonic.peek, r2),
                    Instr(Mnemonic.testct, r2)),

                Select((6, 5), is_lt27,
                    Instr(Mnemonic.lss, r3),
                    Mask(4, 1, "  18 >= 27",
                        Instr(Mnemonic.setpsc, r2r),
                        Instr(Mnemonic.testwct, r2))),
                Select((6, 5), is_lt27,
                    Instr(Mnemonic.lsu, r3),
                    Mask(4, 1, "  19 >= 27",
                        Instr(Mnemonic.chkct, r2),
                        Instr(Mnemonic.chkcti, rus))),
                Mask(10, 1, "  1A",
                    Instr(Mnemonic.blrf, InstrClass.Transfer|InstrClass.Call, u10_p_pc),
                    Instr(Mnemonic.blrb, InstrClass.Transfer|InstrClass.Call, u10_m_pc)),
                Mask(10, 1, "  1B",
                    Instr(Mnemonic.ldapb, PcRel11_2),
                    Instr(Mnemonic.ldapf, PcRel11_2)),

                Mask(10, 1, "  1C",
                    Instr(Mnemonic.blacp, InstrClass.Transfer|InstrClass.Call, u10),
                    Instr(Mnemonic.ldwcp, u10)),
                Mask(10, 1, "  1D",
                    Instr(Mnemonic.setci, ru6),
                    Nyi("  1D 1")),
                Nyi("1E"),
                new W32Decoder(longDecoder));
        }
    }
}