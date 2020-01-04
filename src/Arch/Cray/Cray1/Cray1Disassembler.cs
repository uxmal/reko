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
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Arch.Cray.Cray1
{
    using Decoder = Reko.Core.Machine.Decoder<Cray1Disassembler, Mnemonic, CrayInstruction>;

    public class Cray1Disassembler : DisassemblerBase<CrayInstruction, Mnemonic>
    {
        private static readonly Decoder rootDecoder;

        private readonly Cray1Architecture arch;
        private readonly EndianImageReader rdr;
        private Address addr;
        private List<MachineOperand> ops;


        public Cray1Disassembler(Cray1Architecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.ops = new List<MachineOperand>();
        }

        public override CrayInstruction DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadBeUInt16(out ushort hInstr))
                return null;
            ops.Clear();
            var instr = rootDecoder.Decode(hInstr, this);
            instr.Address = addr;
            instr.Length = (int) (rdr.Address - addr);
            return instr;
        }

        public override CrayInstruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            var instr = new CrayInstruction
            {
                InstructionClass = iclass,
                Mnemonic = mnemonic,
                Operands = this.ops.ToArray(),
            };
            return instr;
        }

        public override CrayInstruction CreateInvalidInstruction()
        {
            return new CrayInstruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.Invalid,
                Operands = MachineInstruction.NoOperands
            };
        }

        #region Decoders

        private static Decoder Instr(Mnemonic mnemonic, params Mutator<Cray1Disassembler> [] mutators)
        {
            return new InstrDecoder<Cray1Disassembler, Mnemonic, CrayInstruction>(InstrClass.Linear, mnemonic, mutators);
        }

        #endregion

        static Cray1Disassembler()
        {
            rootDecoder = Instr(Mnemonic.err);
        }
    }
}
