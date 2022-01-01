#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using System.Text;

namespace Reko.Arch.Etrax
{
    public class MemoryOperand : MachineOperand
    {
        public MemoryOperand(PrimitiveType width)
            : base(width)
        {
        }

        public MachineOperand? Base { get; set; }

        public MachineOperand? Offset { get; set; }

        public PrimitiveType? IndexScale { get; set; }

        public bool PostIncrement { get; set; }

        public RegisterStorage? Assign { get; set; }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            renderer.WriteChar('[');
            if (Base != null)
            {
                Base.Render(renderer, options);
                if (PostIncrement)
                {
                    renderer.WriteChar('+');
                }
                switch (Offset)
                {
                case ImmediateOperand imm:
                    var offset = imm.Value.ToInt32();
                    if (offset < 0)
                    {
                        renderer.WriteChar('-');
                        renderer.WriteFormat("0x{0:X}", Math.Abs(offset));
                    }
                    else if (offset > 0)
                    {
                        renderer.WriteChar('+');
                        renderer.WriteFormat("0x{0:X}", offset);
                    }
                    break;
                case RegisterOperand index:
                    renderer.WriteChar('+');
                    renderer.WriteFormat("{0}:{1}", index.Register.Name, EtraxInstruction.SizeFormat(IndexScale!));
                    break;
                }
            }
            renderer.WriteChar(']');
        }
    }
}
