#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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

using System.Collections.Generic;
using System.Linq;
using Reko.Core;
using Reko.Core.Lib;
using Reko.Core.Machine;
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
        }

        public override XCoreInstruction DisassembleInstruction()
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

        // Two register with immediate long.
        private static bool l2rus(uint uInstr, XCore200Disassembler dasm)
        {
            return false;
        }

        //Register with 6-bit immediate
        private static bool ru6(uint uInstr, XCore200Disassembler dasm)
        {
            return false;
        }

        //Register with 16-bit immediate
        private static bool lru6(uint uInstr, XCore200Disassembler dasm)
        {
            return false;
        }

        // 6-bit immediate
        private static bool u6(uint uInstr, XCore200Disassembler dasm)
        {
            return false;
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
            return false;
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
            return false;
        }

        // Register.
        private static bool r1(uint uInstr, XCore200Disassembler dasm)
        {
            var iReg = uInstr & 0xF;
            dasm.ops.Add(new RegisterOperand(Registers.GpRegs[iReg]));
            return true;
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
            
            rootDecoder = Mask(11, 5, "XCore",
                Nyi("00"),
                Mask(4, 1, "  01",
                    Instr(Mnemonic.clz, r2),
                    Nyi("01")),
                Instr(Mnemonic.add, r3),
                Nyi("03"),

                Mask(4, 1, "  04",
                    Select((5, 6), u => u == 0x3F, "  04 - 0 0x3F",
                        Instr(Mnemonic.bla, r1),
                        Nyi("04 - 0")),
                    Instr(Mnemonic.bau, InstrClass.Transfer, r1)),
                Mask(4, 1, "  05",
                    Select((6, 5), u => u == 0x1F,
                        Instr(Mnemonic.bru, r1),
                        Instr(Mnemonic.andnot, r2)),
                    Instr(Mnemonic.eef, r2)),
                Instr(Mnemonic.eq, r3),
                Instr(Mnemonic.and, r3),

                Select((6, 5), u => u == 0x1F,
                    Instr(Mnemonic.setv, r1),
                    Instr(Mnemonic.or, r3)),
                Nyi("09"),
                Nyi("0A"),
                Nyi("0B"),

                Nyi("0C"),
                Nyi("0D"),
                Nyi("0E"),
                Nyi("0F"),
                // 10
                Instr(Mnemonic.ld16s, r3),
                Instr(Mnemonic.ld8u, r3),
                Instr(Mnemonic.addi, r2us),
                Nyi("13"),
                
                Nyi("14"),
                Nyi("15"),
                Instr(Mnemonic.eqi, r2us),
                Nyi("17"),
                
                Nyi("18"),
                Nyi("19"),
                Nyi("1A"),
                Mask(10, 1, "  1B",
                    Instr(Mnemonic.ldapb, PcRel11_2),
                    Instr(Mnemonic.ldapf, PcRel11_2)),

                Nyi("1C"),
                Nyi("1D"),
                Nyi("1E"),
                new W32Decoder(longDecoder));
        }
    }
}