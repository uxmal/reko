#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using Reko.Core;
using Reko.Core.Machine;

namespace Reko.Arch.Avr
{
    public class Avr8Disassembler : DisassemblerBase<AvrInstruction>
    {
        private Address addr;
        private Avr8Architecture avr8Architecture;
        private ImageReader rdr;

        public Avr8Disassembler(Avr8Architecture avr8Architecture, ImageReader rdr)
        {
            this.avr8Architecture = avr8Architecture;
            this.rdr = rdr;
        }

        public override AvrInstruction DisassembleInstruction()
        {
            ushort wInstr;
            this.addr = rdr.Address;
            if (!rdr.TryReadUInt16(out wInstr))
                return null;
            var length = rdr.Address - addr;
            return new AvrInstruction
            {
                opcode = Opcode.invalid,
                Address = addr,
                Length = (int)length
            };
        }
    }
}