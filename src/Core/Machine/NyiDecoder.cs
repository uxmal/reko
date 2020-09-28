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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core.Machine
{
    public class NyiDecoder<TDasm, TMnemonic, TInstr> : Decoder<TDasm, TMnemonic, TInstr>
        where TInstr : MachineInstruction
        where TDasm : DisassemblerBase<TInstr, TMnemonic>
    {
        private readonly string message;

        public NyiDecoder(string message)
        {
            this.message = message;
        }

        public override TInstr Decode(uint wInstr, TDasm dasm)
        {
            return dasm.NotYetImplemented(wInstr, message);
        }
    }
}
