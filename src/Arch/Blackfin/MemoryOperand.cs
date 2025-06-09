#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

namespace Reko.Arch.Blackfin
{
    public class MemoryOperand : AbstractMachineOperand
    {
        public RegisterStorage? Base;
        public RegisterStorage? Index;
        public int Offset;
        public bool PreDecrement;
        public bool PostDecrement;
        public bool PostIncrement;

        public MemoryOperand(PrimitiveType dt) : base(dt)
        {
        }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            if (base.DataType.BitSize == 8)
            {
                renderer.WriteChar('B');
            } 
            else if (base.DataType.BitSize == 16)
            {
                renderer.WriteChar('W');
            }
            renderer.WriteChar('[');
            if (PreDecrement)
            {
                renderer.WriteString("--");
            }
            renderer.WriteString(Base!.Name);
            if (Index is not null)
            {
                renderer.WriteString(" + ");
                renderer.WriteString(Index.Name);
            }
            else if (Offset > 0)
            {
                renderer.WriteString($" + 0x{Offset:X4}");
            }
            else if (Offset < 0)
            {
                renderer.WriteString($" - 0x{-Offset:X4}");
            }
            if (PostIncrement)
            {
                renderer.WriteString("++");
            }
            else if (PostDecrement)
            {
                renderer.WriteString("--");
            }
            renderer.WriteChar(']');
        }
    }
}