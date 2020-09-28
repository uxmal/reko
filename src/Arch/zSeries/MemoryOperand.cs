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

using Reko.Core;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.zSeries
{
    public class MemoryOperand : MachineOperand
    {
        public RegisterStorage Base;
        public RegisterStorage Index;
        public int Length;
        public int Offset;

        public MemoryOperand(PrimitiveType dt) : base(dt)
        {
        }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            if (Offset != 0)
            {
                writer.WriteFormat("{0}", Offset);
            }
            writer.WriteFormat("(");
            if (Length != 0)
            {
                writer.WriteFormat("{0},", Length);
            }
            if (Index != null && Index.Number != 0)
            {
                writer.WriteString(Index.Name);
                if (Base != null && Base.Number != 0)
                {
                    writer.WriteString(",");
                    writer.WriteString(Base.Name);
                }
            }
            else
            {
                writer.WriteString(Base.Name);
            }
            writer.WriteFormat(")");
        }
    }
}
