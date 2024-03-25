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
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.CSky
{
    public class MemoryOperand : AbstractMachineOperand
    {
        private MemoryOperand(PrimitiveType dt, RegisterStorage? baseRegister, RegisterStorage? indexRegister, int offset, int shift)
            : base(dt)
        {
            this.Base = baseRegister;
            this.Index = indexRegister;
            this.Offset = offset;
            this.Shift = shift;
        }

        internal static MemoryOperand Displacement(PrimitiveType dt, RegisterStorage? baseAddress, int displacement)
        {
            return new MemoryOperand(dt, baseAddress, null, displacement, 0);
        }


        public static MachineOperand Direct(PrimitiveType dt, ulong uAddr)
        {
            return new MemoryOperand(dt, null, null, (int)uAddr, 0);
        }

        public static MemoryOperand Indexed(PrimitiveType dt, RegisterStorage baseRegister, RegisterStorage indexRegister, int shift)
        {
            return new MemoryOperand(dt, baseRegister, indexRegister, 0, shift);
        }



        public RegisterStorage? Base { get; }
        public RegisterStorage? Index { get; }
        public int Offset { get; }
        public int Shift { get; }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            if (Base is null)
            {
                renderer.WriteFormat("[{0:X8}]", (uint) Offset);
            }
            else if (Index is not null)
            {
                if (Shift > 0)
                {
                    renderer.WriteFormat("({0},{1}<<{2})", Base, Index, Shift);
                }
                else
                {
                    renderer.WriteFormat("({0},{1})", Base, Index);
                }
            }
            else
            {
                if (Offset == 0)
                {
                    renderer.WriteFormat("({0})", Base);
                }
                else
                {
                    renderer.WriteFormat("({0},{1})", Base, Offset);
                }
            }
        }
    }
}
