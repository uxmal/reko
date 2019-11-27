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

using Reko.Core;
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Arch.Cray.Cray1
{
    using Decoder = Reko.Core.Machine.Decoder<Cray1Disassembler, Mnemonic, CrayInstruction>;

    public class Cray1Disassembler : DisassemblerBase<CrayInstruction>
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

        protected override CrayInstruction CreateInvalidInstruction()
        {
            return new CrayInstruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.Invalid,
                Operands = MachineInstruction.NoOperands
            };
        }

        #region Decoders

        private class InstrDecoder : Decoder
        {
            private readonly InstrClass iclass;
            private readonly Mnemonic mnemonic;
            private readonly Mutator<Cray1Disassembler>[] mutators;

            public InstrDecoder(InstrClass iclass, Mnemonic mnemonic, Mutator<Cray1Disassembler>[] mutators)
            {
                this.iclass = iclass;
                this.mnemonic = mnemonic;
                this.mutators = mutators;
            }

            public override CrayInstruction Decode(uint wInstr, Cray1Disassembler dasm)
            {
                foreach (var m in mutators)
                {
                    if (!m(wInstr, dasm))
                        return dasm.CreateInvalidInstruction();
                }
                var instr = new CrayInstruction
                {
                    InstructionClass = this.iclass,
                    Mnemonic = this.mnemonic,
                    Operands = dasm.ops.ToArray(),
                };
                return instr;
            }
        }

        private static InstrDecoder Instr(Mnemonic mn, params Mutator<Cray1Disassembler> [] mutators)
        {
            return new InstrDecoder(InstrClass.Linear, mn, mutators);
        }

        #endregion

        static Cray1Disassembler()
        {
            rootDecoder = Instr(Mnemonic.err);
        }
    }
}
