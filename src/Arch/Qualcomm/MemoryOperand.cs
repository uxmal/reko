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

namespace Reko.Arch.Qualcomm
{
    public class MemoryOperand : MachineOperand
    {

        public MemoryOperand(PrimitiveType width) : base(width)
        {
        }

        public RegisterStorage Base { get; set; }
        public int Offset { get; set; }

        protected override void DoRender(MachineInstructionRenderer writer, MachineInstructionRendererOptions options)
        {
            writer.WriteString("mem");
            if (Width.Size == 1)
                writer.WriteString("b");
            else if (Width.Size == 4)
                writer.WriteString("w");
            else 
                throw new System.NotImplementedException();
            writer.WriteChar('(');
            writer.WriteString(Base.Name);
            if (Offset < 0)
            {
                writer.WriteFormat("-{0}", -Offset);
            } else if (Offset > 0)
            {
                writer.WriteFormat("+{0}", Offset);
            }
            writer.WriteChar(')');
        }
    }
}