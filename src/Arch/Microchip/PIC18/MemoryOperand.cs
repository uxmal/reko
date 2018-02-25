#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet.
 * inspired by work of:
 * Copyright (C) 1999-2017 John Källén.
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
using Reko.Core.Types;
using Reko.Core.Expressions;

namespace Reko.Arch.Microchip.PIC18
{
    public class MemoryOperand : MachineOperand
    {
        public RegisterStorage Base;
        public Constant IsAccess;

        public MemoryOperand(PrimitiveType width) : base(width)
        {
            Base = RegisterStorage.None;
        }

        public MemoryOperand(PrimitiveType width, RegisterStorage baseReg) : base(width)
        {
            Base = baseReg;
        }

        public MemoryOperand(PrimitiveType width, RegisterStorage baseReg, Constant access) : base(width)
        {
            Base = baseReg;
            IsAccess = access;
        }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.Write(Base.Name);
        }
    }
}
