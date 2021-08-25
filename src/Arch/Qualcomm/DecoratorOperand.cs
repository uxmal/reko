#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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
    public class DecoratorOperand : MachineOperand
    {
        public DecoratorOperand(DataType width, MachineOperand op) : base(width)
        {
            this.Operand = op;
        }

        public MachineOperand Operand { get; }
        public int BitOffset { get; set;  }

        public bool Inverted { get; set; }

        public bool NewValue { get; set; }

        public bool Complement { get; set; }

        public bool Chop { get;  set; }

        public bool Carry { get; set; }

        public bool Sat { get; set; }
        public bool Rnd { get; set; }
        public bool Lsl16 { get; set; }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            if (Complement)
                renderer.WriteChar('~');
            else if (Inverted)
                renderer.WriteChar('!');
            this.Operand.Render(renderer, options);
            if (this.Width.BitSize < (int) Operand.Width.BitSize)
            {
                renderer.WriteChar('.');
                if (BitOffset == 0)
                    renderer.WriteChar('l');
                else
                    renderer.WriteChar('h');
            }
            if (NewValue)
            {
                renderer.WriteString(".new");
            }
            if (Chop)
            {
                renderer.WriteString(":chop");
            }
            if (Carry)
            {
                renderer.WriteString(":carry");
            }
            if (Rnd)
            {
                renderer.WriteString(":rnd");
            }
            if (Sat)
            {
                renderer.WriteString(":sat");
            }
            if (Lsl16)
            {
                renderer.WriteString(":<<16");
            }
        }
    }
}