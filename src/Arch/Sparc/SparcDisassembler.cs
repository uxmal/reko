#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Expressions;
using Decompiler.Core.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Arch.Sparc
{
    public class SparcDisassembler : Disassembler
    {
        private SparcArchitecture arch;
        private ImageReader imageReader;

        public SparcDisassembler(SparcArchitecture arch, ImageReader imageReader)
        {
            this.arch = arch;
            this.imageReader = imageReader;
        }

        public Address Address  { get { return imageReader.Address; } }

        public MachineInstruction DisassembleInstruction()
        {
            throw new NotImplementedException();
        }

        public SparcInstruction Disassemble()
        {
            if (!imageReader.IsValid)
                return null;
            uint wInstr = imageReader.ReadBeUInt32();
            switch (wInstr >> 30)
            {
            case 0:
                throw new NotImplementedException();
            case 1:
                return new SparcInstruction
                {
                    Opcode = Opcode.call,
                    Op1 = new AddressOperand((imageReader.Address - 4) + (wInstr << 2)),
                };
            case 2:
            case 3:
                throw new NotImplementedException();
            }
            return new SparcInstruction
            {
            };
        }
    }
}
