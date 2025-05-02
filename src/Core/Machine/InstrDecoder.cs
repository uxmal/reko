#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

using System.Diagnostics;

namespace Reko.Core.Machine
{
    /// <summary>
    /// This class uses 0 or more <see cref="Mutator{TDasm}"/>s to mutate the disassembler
    /// state, finaly producing a <see cref="MachineInstruction"/>.
    /// </summary>
    /// <typeparam name="TDasm"></typeparam>
    /// <typeparam name="TMnemonic"></typeparam>
    /// <typeparam name="TInstr"></typeparam>
    public class InstrDecoder<TDasm, TMnemonic, TInstr> : Decoder<TDasm, TMnemonic, TInstr>
        where TDasm : DisassemblerBase<TInstr, TMnemonic>
        where TMnemonic : struct
        where TInstr : MachineInstruction
    {
        private readonly InstrClass iclass;
        private readonly TMnemonic mnemonic;
        private readonly Mutator<TDasm>[] mutators;

        /// <summary>
        /// Constructs a intruction decoder.
        /// </summary>
        /// <param name="iclass">InstrClass </param>
        /// <param name="mnemonic">Mnemonic for this instruction.</param>
        /// <param name="mutators">Operand decoders.</param>
        public InstrDecoder(InstrClass iclass, TMnemonic mnemonic, params Mutator<TDasm>[] mutators)
        {
            this.iclass = iclass;
            this.mnemonic = mnemonic;
            this.mutators = mutators;
        }

        /// <summary>
        /// Decodes the instruction by applying the mutators to the disassembler,
        /// resulting in a <see cref="MachineInstruction"/>.
        /// </summary>
        /// <param name="wInstr">Opcode to decode.</param>
        /// <param name="dasm">Disassembler instance.</param>
        /// <returns>The decoded machine instruction.
        /// </returns>
        public override TInstr Decode(uint wInstr, TDasm dasm)
        {
            DumpMaskedInstruction(wInstr, 0, this.mnemonic);
            foreach (var m in mutators)
            {
                if (!m(wInstr, dasm))
                    return dasm.CreateInvalidInstruction();
            }
            return dasm.MakeInstruction(this.iclass, this.mnemonic);
        }

        /// <summary>
        /// Returns a string representation of the instruction decoder.
        /// </summary>
        public override string ToString()
        {
            return $"{iclass}:{mnemonic}";
        }
    }

    /// <summary>
    /// A 64-bit version of the <see cref="InstrDecoder{TDasm, TMnemonic, TInstr}"/>.
    /// </summary>
    /// <typeparam name="TDasm"></typeparam>
    /// <typeparam name="TMnemonic"></typeparam>
    /// <typeparam name="TInstr"></typeparam>
    public class WideInstrDecoder<TDasm, TMnemonic, TInstr> : WideDecoder<TDasm, TMnemonic, TInstr>
        where TDasm : DisassemblerBase<TInstr, TMnemonic>
        where TMnemonic : struct
        where TInstr : MachineInstruction
    {
        private readonly InstrClass iclass;
        private readonly TMnemonic mnemonic;
        private readonly WideMutator<TDasm>[] mutators;

        /// <summary>
        /// Constructs a intruction decoder.
        /// </summary>
        /// <param name="iclass">InstrClass </param>
        /// <param name="mnemonic">Mnemonic for this instruction.</param>
        /// <param name="mutators">Operand decoders.</param>
        public WideInstrDecoder(InstrClass iclass, TMnemonic mnemonic, params WideMutator<TDasm>[] mutators)
        {
            this.iclass = iclass;
            this.mnemonic = mnemonic;
            this.mutators = mutators;
        }

        /// <summary>
        /// Decodes the instruction by applying the mutators to the disassembler,
        /// resulting in a <see cref="MachineInstruction"/>.
        /// </summary>
        /// <param name="ulInstr">Opcode to decode.</param>
        /// <param name="dasm">Disassembler instance.</param>
        /// <returns>The decoded machine instruction.
        /// </returns>
        public override TInstr Decode(ulong ulInstr, TDasm dasm)
        {
            foreach (var m in mutators)
            {
                if (!m(ulInstr, dasm))
                    return dasm.CreateInvalidInstruction();
            }
            return dasm.MakeInstruction(this.iclass, this.mnemonic);
        }

        /// <summary>
        /// Returns a string representation of the instruction decoder.
        /// </summary>
        public override string ToString()
        {
            return $"{iclass}:{mnemonic}";
        }
    }
}
