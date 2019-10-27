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

using System;
using System.Collections.Generic;
using System.Text;
using Reko.Core;
using Reko.Core.Lib;
using Reko.Core.Machine;

namespace Reko.Arch.Cray.Ymp
{
    using Decoder = Reko.Core.Machine.Decoder<YmpDisassembler, Mnemonic, CrayInstruction>;

    public class YmpDisassembler : DisassemblerBase<CrayInstruction>
    {
        private static readonly Decoder rootDecoder;
        
        private CrayYmpArchitecture arch;
        private EndianImageReader rdr;
        private List<MachineOperand>  ops;

        public YmpDisassembler(CrayYmpArchitecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.ops = new List<MachineOperand>();
        }

        public override CrayInstruction DisassembleInstruction()
        {
            var addr = rdr.Address;
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
                Mnemonic = Mnemonic.Invalid
            };
        }

        #region Mutators
        private static Mutator<YmpDisassembler> S(int bitOffset)
        {
            var field = new Bitfield(bitOffset, 3);
            return (u, d) =>
            {
                var iReg = field.Read(u);
                var reg = Registers.SRegs[iReg];
                d.ops.Add(new RegisterOperand(reg));
                return true;
            };
        }
        private static readonly Mutator<YmpDisassembler> Si = S(6);
        private static readonly Mutator<YmpDisassembler> Sj = S(3);
        private static readonly Mutator<YmpDisassembler> Sk = S(0);

        #endregion

        #region Decoders
        private class InstrDecoder : Decoder
        {
            private readonly InstrClass iclass;
            private readonly Mnemonic mnemonic;
            private readonly Mutator<YmpDisassembler>[] mutators;

            public InstrDecoder(InstrClass iclass, Mnemonic mnemonic, Mutator<YmpDisassembler>[] mutators)
            {
                this.iclass = iclass;
                this.mnemonic = mnemonic;
                this.mutators = mutators;
            }

            public override CrayInstruction Decode(uint wInstr, YmpDisassembler dasm)
            {
                foreach (var m in mutators)
                {
                    if (!m(wInstr, dasm))
                        return dasm.CreateInvalidInstruction();
                }
                var instr = new CrayInstruction
                {
                    InstructionClass = iclass,
                    Mnemonic = mnemonic,
                    Operands = dasm.ops.ToArray()
                };
                return instr;
            }
        }

        private static InstrDecoder Instr(Mnemonic mnemonic, params Mutator<YmpDisassembler>[] mutators)
        {
            return new InstrDecoder(InstrClass.Linear, mnemonic, mutators);
        }

        protected static NyiDecoder<YmpDisassembler, Mnemonic, CrayInstruction> Nyi(string message)
        {
            return new NyiDecoder<YmpDisassembler, Mnemonic, CrayInstruction>(message);
        }


        #endregion

        static YmpDisassembler()
        {
            rootDecoder = Sparse(9, 7, "YMP",
                Nyi("YMP"),
                (0x24, Instr(Mnemonic._and, Si, Sj, Sk)));
        }
    }
}
