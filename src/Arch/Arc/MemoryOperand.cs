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
using System.Text;
using Reko.Core;
using Reko.Core.Machine;
using Reko.Core.Types;

namespace Reko.Arch.Arc
{
    public class MemoryOperand : MachineOperand
    {
        public MemoryOperand(PrimitiveType dt) : base(dt)
        {
        }

        public RegisterStorage Base { get; set; }
        public int Offset { get;  set; }
        public RegisterStorage Index { get; set; }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteChar('[');
            var sep = "";
            if (Base != null)
            {
                writer.WriteString(Base.Name);
                sep = ",";
            }
            if (Offset != 0)
            {
                writer.WriteFormat("{0}{1}", sep, Offset);
            }
            else if (Index != null)
            {
                writer.WriteFormat("{0}{1}", sep, Offset);
            }
            writer.WriteChar(']');
        }
    }

    public enum AddressWritebackMode
    {
        None,
        aw,
        ab,
        @as,
    }
}
