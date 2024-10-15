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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Types;
using System.Diagnostics;

namespace Reko.Arch.Sparc
{
    public class MemoryOperand : AbstractMachineOperand
    {
        private MemoryOperand(RegisterStorage b, MachineOperand? offset, RegisterStorage? index, PrimitiveType width) : base(width)
        {
            this.Base = b;
            this.Offset = offset;
            this.Index = index;
        }

        public static MemoryOperand Indirect(RegisterStorage baseRg, Constant offset, PrimitiveType dt)
        {
            return new MemoryOperand(baseRg, new ImmediateOperand(offset), null, dt);
        }

        public static MemoryOperand Indexed(RegisterStorage baseReg, RegisterStorage index, PrimitiveType dt)
        {
            return new MemoryOperand(baseReg, null, index, dt);
        }

        public RegisterStorage Base { get; }
        public MachineOperand? Offset { get; set; }
        public RegisterStorage? Index { get; }


        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            renderer.WriteFormat("[%{0}", Base.Name);
            if (Offset is not null)
            {
                int offset = IntOffset();
                if (offset >= 0)
                {
                    renderer.WriteString("+");
                }
                renderer.WriteString(offset.ToString());
            }
            else
            {
                Debug.Assert(Index is not null);
                renderer.WriteFormat("+%{0}", Index);
            }
            renderer.WriteString("]");
        }

        public int IntOffset()
        {
            if (Offset is null)
                return 0;
            ImmediateOperand offset;
            if (Offset is SliceOperand slice)
            {
                offset = slice.Value;
            }
            else
            {
                offset = (ImmediateOperand) Offset;
            }
            return offset.Value.ToInt32();
        }
    }
}
