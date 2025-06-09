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

namespace Reko.Arch.MilStd1750
{
    public class MemoryOperand : AbstractMachineOperand
    {
        private MemoryOperand(PrimitiveType dt) : base(dt)
        {
        }
        
        public ushort Displacement { get; set; }
        public RegisterStorage? Index { get; set; }
        public bool Indirected { get; set; }

        public static MemoryOperand Direct(PrimitiveType dt, ushort disp, RegisterStorage? xReg)
        {
            return new MemoryOperand(dt)
            {
                Displacement = disp,
                Index = xReg,
            };
        }

        public static MemoryOperand Indirect(PrimitiveType dt, ushort disp, RegisterStorage? xReg)
        {
            return new MemoryOperand(dt)
            {
                Displacement = disp,
                Index = xReg,
                Indirected = true,
            };
        }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            var sep = "";
            if (Displacement != 0 || Index is null)
            {
                Instruction.WriteHex(Displacement, renderer);
                sep = ",";
            }
            if (Index is not null)
            {
                renderer.WriteFormat("{0}{1}", sep, Index.Name);
            }
        }
    }
}