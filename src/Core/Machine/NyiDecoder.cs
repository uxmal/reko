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

namespace Reko.Core.Machine
{
    /// <summary>
    /// Special subclass of <see cref="Decoder"/>, used as a placeholder
    /// for machine instructions that haven't been implemented yet. When 
    /// the <see cref="Decode(uint, TDasm)"/> method is called, the 
    /// <see cref="DisassemblerBase{TInstr, TMnemonic}.NotYetImplemented(string)"/>
    /// hook method is called. 
    /// </summary>
    public class NyiDecoder<TDasm, TMnemonic, TInstr> : Decoder<TDasm, TMnemonic, TInstr>
        where TInstr : MachineInstruction
        where TMnemonic : struct
        where TDasm : DisassemblerBase<TInstr, TMnemonic>
    {
        private readonly string message;

        /// <summary>
        /// Constructs a not-yet-implemented decoder.
        /// </summary>
        /// <param name="message">Diagnostic message.</param>
        public NyiDecoder(string message)
        {
            this.message = message;
        }

        /// <summary>
        /// By decoding a NYI instruction, the disassembler will generate a unit test.
        /// </summary>
        /// <param name="wInstr"></param>
        /// <param name="dasm"></param>
        /// <returns></returns>
        public override TInstr Decode(uint wInstr, TDasm dasm)
        {
            return dasm.NotYetImplemented(message);
        }
    }

    /// <summary>
    /// Wide version of the <see cref="NyiDecoder{TDasm, TMnemonic, TInstr}"/> class.
    /// </summary>
    public class WideNyiDecoder<TDasm, TMnemonic, TInstr> : WideDecoder<TDasm, TMnemonic, TInstr>
        where TInstr : MachineInstruction
        where TMnemonic : struct
        where TDasm : DisassemblerBase<TInstr, TMnemonic>
    {
        private readonly string message;

        /// <summary>
        /// Constructs a not-yet-implemented decoder.
        /// </summary>
        /// <param name="message">Diagnostic message.</param>
        public WideNyiDecoder(string message)
        {
            this.message = message;
        }

        /// <summary>
        /// By decoding a NYI instruction, the disassembler will generate a unit test.
        /// </summary>
        /// <param name="wInstr"></param>
        /// <param name="dasm"></param>
        /// <returns></returns>
        public override TInstr Decode(ulong wInstr, TDasm dasm)
        {
            return dasm.NotYetImplemented(message);
        }
    }

}
