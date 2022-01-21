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
using Reko.Core.Services;
using System;
using System.Collections.Generic;

namespace Reko.Arch.Pdp10.Disassembler
{
    using Decoder = WideDecoder<Pdp10Disassembler, Mnemonic, Pdp10Instruction>;

    public partial class Pdp10Disassembler : DisassemblerBase<Pdp10Instruction, Mnemonic>
    {
        private static readonly Decoder rootDecoder;
        private const int IndirectBit = 35 - 13;
        private const ulong LowWordMask = (1ul << 18) - 1ul;

        private readonly Pdp10Architecture arch;
        private readonly Word36BeImageReader rdr;
        private readonly List<MachineOperand> ops;
        private Address addr;
        private ulong uInstr;
        private uint opc;
        private uint ac;
        private bool ind;
        private uint idx;
        private uint imm;

        public Pdp10Disassembler(Pdp10Architecture arch, Word36BeImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.ops = new List<MachineOperand>();
            this.addr = default!;
        }

        public override Pdp10Instruction? DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadBeUInt36(out ulong uInstr))
                return null;
            this.opc = opcField.Read(uInstr);
            this.ac = acField.Read(uInstr);
            this.ind = Bits.IsBitSet(uInstr, IndirectBit);
            this.idx = idxField.Read(uInstr);
            this.imm = immField.Read(uInstr);
            this.uInstr = uInstr;
            var instr = rootDecoder.Decode(uInstr, this);
            ops.Clear();
            instr.Address = addr;
            instr.Length = 1;
            return instr;
        }

        public override Pdp10Instruction CreateInvalidInstruction()
        {
            return new Pdp10Instruction
            {
                Mnemonic = Mnemonic.Invalid,
                InstructionClass = InstrClass.Invalid,
            };
        }

        public override Pdp10Instruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            return new Pdp10Instruction
            {
                InstructionClass = iclass,
                Mnemonic = mnemonic,
                Operands = ops.ToArray(),
            };
        }

        public override Pdp10Instruction NotYetImplemented(string message)
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            var sOpcode = Convert.ToString((long)uInstr, 8).PadLeft(12, '0');
            testGenSvc?.ReportMissingDecoder("Pdp10Dis", this.addr, message, sOpcode);
            return new Pdp10Instruction
            {
                Mnemonic = Mnemonic.Nyi,
                InstructionClass = InstrClass.Invalid,
            };
        }

        /// <summary>
        /// Create a <see cref="Bitfield"/> for the PDP-10 instruction word.
        /// Bits are numbered big-endian, and the word is 36 bits long.
        /// </summary>
        private static Bitfield bf(int bitPos, int bitlength)
        {
            return new Bitfield(36 - (bitPos + bitlength), bitlength);
        }

        private static readonly Bitfield opcField = bf(0, 9);
        private static readonly Bitfield acField = bf(9, 4);
        private static readonly Bitfield idxField = bf(14, 4);
        private static readonly Bitfield immField = bf(18, 18);


        /// <summary>
        /// A reference to the 'ac' accumulator field.
        /// </summary>
        private static bool AC(ulong u, Pdp10Disassembler dasm)
        {
            var reg = Registers.Accumulators[dasm.ac];
            dasm.ops.Add(new RegisterOperand(reg));
            return true;
        }

        /// <summary>
        /// Interpret E as a jump or call target.
        /// </summary>
        private static bool J(ulong u, Pdp10Disassembler dasm)
        {
            if (dasm.idx == 0)
            {
                var addr = new Address18(dasm.imm);
                dasm.ops.Add(AddressOperand.Create(addr));
                return true;
            }
            dasm.NotYetImplemented("Indexed J");
            return false;
        }

        /// <summary>
        /// Interpret E as a zero-extended immediate value.
        /// </summary>
        private static bool Imm(ulong uInstr, Pdp10Disassembler dasm)
        {
            var imm = Constant.Create(Pdp10Architecture.Word36, uInstr & LowWordMask);
            dasm.ops.Add(new ImmediateOperand(imm));
            return true;
        }

        /// <summary>
        /// Immediate value stored in the AC field.
        /// </summary>
        private static bool ImmAc(ulong uInstr, Pdp10Disassembler dasm)
        {
            var imm = acField.Read(uInstr);
            dasm.ops.Add(ImmediateOperand.Byte((byte)imm));
            return true;
        }

        static Pdp10Disassembler()
        {
            rootDecoder = InstructionSet.Create();
        }
    }
}
