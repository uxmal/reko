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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Reko.Environments.Gameboy
{
    public class MemoryOperand : AbstractMachineOperand
    {
        public MemoryOperand(PrimitiveType size) : base(size)
        {
        }

        public RegisterStorage? Base { get; set; }
        public int Offset { get; set; }

        public bool PostIncrement { get; set; }
        public bool PostDecrement { get; set; }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            Debug.Assert(!PostDecrement || !PostIncrement);
            renderer.WriteChar('(');
            if (Base is not null)
            {
                renderer.WriteString(Base.Name);
            }
            else
            {
                var number = Offset;
                GameboyInstruction.RenderIntelHexNumber(number, renderer);
            }
            if (PostDecrement)
                renderer.WriteChar('-');
            else if (PostIncrement)
                renderer.WriteChar('+');
            renderer.WriteChar(')');
        }
    }
}
