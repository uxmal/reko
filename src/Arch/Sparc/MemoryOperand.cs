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
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Sparc
{
    public class MemoryOperand : MachineOperand
    {
        public readonly RegisterStorage Base;
        public readonly Constant Offset;

        public MemoryOperand(RegisterStorage b, Constant offset, PrimitiveType width) : base(width)
        {
            this.Base = b;
            this.Offset = offset;
        }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteFormat("[%{0}", Base.Name);
            if (!Offset.IsNegative)
            {
                writer.WriteString("+");
            }
            writer.WriteString(Offset.ToString());
            writer.WriteString("]");
        }
    }

    public class IndexedMemoryOperand : MachineOperand
    {
        public readonly RegisterStorage Base;
        public readonly RegisterStorage Index;

        public IndexedMemoryOperand(RegisterStorage r1, RegisterStorage r2, PrimitiveType width) : base(width)
        {
            this.Base = r1;
            this.Index = r2;
        }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteFormat("[%{0}+%{1}]", Base, Index);
        }
    }
}
