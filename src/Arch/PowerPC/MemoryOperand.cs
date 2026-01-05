#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

namespace Reko.Arch.PowerPC
{
    public class MemoryOperand : AbstractMachineOperand
    {
        public MemoryOperand(DataType size, RegisterStorage reg, MachineOperand offset) : base(size)
        {
            this.BaseRegister = reg;
            this.Offset = offset;
        }

        public RegisterStorage BaseRegister { get; }
        public MachineOperand Offset { get; set; } 

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            if (Offset is SliceOperand slice)
            {
                renderer.WriteString($"{slice}({BaseRegister})");
            }
            else
            {
                var offset = (Constant) this.Offset;
                renderer.WriteString($"{offset.ToInt32()}({BaseRegister.Name})");
            }
        }

        public int IntOffset()
        {
            Constant offset;
            if (this.Offset is SliceOperand slice)
            {
                offset = slice.Value;
            }
            else
            {
                offset = (Constant) this.Offset;
            }
            return offset.ToInt32();
        }
    }
}
