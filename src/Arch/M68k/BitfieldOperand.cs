#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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

using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.M68k
{
    public class BitfieldOperand : AbstractMachineOperand, M68kOperand
    {
        public MachineOperand BitOffset;
        public MachineOperand BitWidth;

        public BitfieldOperand(PrimitiveType width, MachineOperand offset, MachineOperand bitwidth) : base(width)
        {
            this.BitWidth = bitwidth;
            this.BitOffset = offset;
        }

        public T Accept<T>(M68kOperandVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            renderer.WriteChar('{');
            renderer.WriteString(BitOffset.ToString());
            renderer.WriteChar(':');
            renderer.WriteString(BitWidth.ToString());
            renderer.WriteChar('}');
        }
    }
}
