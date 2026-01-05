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
using System.Diagnostics;

namespace Reko.Arch.Mips
{
    public class MemoryOperand : AbstractMachineOperand
    {
        private MemoryOperand(
            PrimitiveType dataWidth,
            RegisterStorage baseReg,
            MachineOperand? offset,
            RegisterStorage? index)
            : base(dataWidth)
        {
            this.Base = baseReg;
            this.Offset = offset;
            this.Index = index;
        }

        public static MemoryOperand Indirect(PrimitiveType dataWidth, MachineOperand offset, RegisterStorage baseReg)
        {
            return new MemoryOperand(dataWidth, baseReg, offset, null);
        }

        public static MemoryOperand Indexed(PrimitiveType dataWidth, RegisterStorage baseReg, RegisterStorage index)
        {
            return new MemoryOperand(dataWidth, baseReg, null, index);
        }

        public RegisterStorage Base { get; }

        public MachineOperand? Offset { get; set;}

        public RegisterStorage? Index { get; }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            if (Offset is not null)
            {
                if (Offset is SliceOperand slice)
                {
                    slice.Render(renderer, options);
                    renderer.WriteFormat("({0})", Base);
                    return;
                }
                string fmt;
                int offset = ((Constant) Offset).ToInt32();
                if (offset >= 0)
                {
                    fmt = "{0:X4}({1})";
                }
                else
                {
                    fmt = "-{0:X4}({1})";
                    offset = -offset;
                }
                renderer.WriteFormat(fmt, offset, Base);
            }
            else
            {
                Debug.Assert(Index is not null);
                renderer.WriteString(Index.Name);
                renderer.WriteString("(");
                renderer.WriteString(Base.Name);
                renderer.WriteString(")");
            }
        }

        public int IntOffset()
        {
            if (Offset is SliceOperand slice)
                return slice.Value.ToInt32();
            else if (Offset is Constant c)
                return c.ToInt32();
            return 0;
        }
    }
}
