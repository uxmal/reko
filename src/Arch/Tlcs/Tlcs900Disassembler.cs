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

using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Architectures.Tlcs
{
    public class Tlcs900Disassembler : DisassemblerBase<Tlcs900Instruction>
    {
        private Tlcs900Instruction instr;
        private ImageReader rdr;

        public Tlcs900Disassembler(ImageReader rdr)
        {
            this.rdr = rdr;
        }

        public override Tlcs900Instruction DisassembleInstruction()
        {
            this.instr = new Tlcs900Instruction
            {
                Address = rdr.Address,
            };

            this.instr.Length = (int)(rdr.Address - instr.Address);
            return this.instr;
        }
    }
}
