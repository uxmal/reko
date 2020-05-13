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
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Reko.Arch.Avr.Avr32
{
    using Decoder = Reko.Core.Machine.Decoder<Avr32Disassembler, Mnemonic, Avr32Instruction>;

    public class Avr32Disassembler : DisassemblerBase<Avr32Instruction, Mnemonic>
    {
        private static Decoder rootDecoder;

        private readonly Avr32Architecture arch;
        private readonly EndianImageReader rdr;
        private readonly List<MachineOperand> ops;
        private Address addr;

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
                InstructionClass = iclass,
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
#if DEBUG
            var hexBytes = (wInstr >> 29) == 0b111
                ? wInstr.ToString("X8")
                : wInstr.ToString("X4");
            EmitUnitTest(
                "Avr32",
                hexBytes,
                message,
                "AvrDis",
                this.addr,
                m => m.WriteLine("AssertCode(\"@@@\", \"{0}\");", hexBytes));
#endif
            return base.NotYetImplemented(wInstr, message);
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

        private static readonly Mutator<Avr32Disassembler> Is4_8 = Imm_signed(4, 8);

        #endregion

        #region Decoders
        private static Decoder Nyi(string message)
        {
            return new NyiDecoder<Avr32Disassembler, Mnemonic, Avr32Instruction>(message);
        }

        #endregion

        static Avr32Disassembler()
        {
            rootDecoder = Mask(13, 3, "AVR32",
                Nyi("0b000"),
                Mask(12, 1, "  001",
                    Nyi("0"),
                    Instr(Mnemonic.mov, R0, Is4_8)),
                Nyi("0b010"),
                Nyi("0b011"),
                Nyi("0b100"),
                Nyi("0b101"),
                Nyi("0b110"),
                Nyi("0b111"));
        }
    }
}