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
using System.Diagnostics;

namespace Reko.Arch.MN103
{
    public class MemoryOperand : AbstractMachineOperand
    {
        private MemoryOperand(DataType width, RegisterStorage? baseReg, RegisterStorage? indexReg, int displacement) : base(width)
        {
            this.Base = baseReg;
            this.Index = indexReg;
            this.Displacement = displacement;
        }

        public static MemoryOperand Absolute(uint uAddr)
        {
            return new MemoryOperand(PrimitiveType.Byte, null, null, (int) uAddr);
        }

        public static MemoryOperand Relative(RegisterStorage baseReg, int displacement)
        {
            return new MemoryOperand(PrimitiveType.Byte, baseReg, null, displacement);
        }

        public static MemoryOperand Indexed(RegisterStorage baseReg, RegisterStorage indexReg)
        {
            return new MemoryOperand(PrimitiveType.Byte, baseReg, indexReg, 0);
        }

        public RegisterStorage? Base { get; }
        public RegisterStorage? Index { get; }
        public int Displacement { get; }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            renderer.WriteChar('(');
            int disp = Displacement;
            if (disp != 0)
            {
                if (Base is null)
                {
                    renderer.WriteFormat("{0:X8}", disp);
                }
                else
                {
                    if (disp < 0)
                    {
                        renderer.WriteChar('-');
                        disp = -disp;
                    }
                    renderer.WriteFormat("{0:X}", disp);
                    renderer.WriteChar(',');
                    renderer.WriteString(Base.Name);
                }
            }
            else
            {
                Debug.Assert(this.Base is not null);
                if (Index is not null)
                {
                    renderer.WriteString(Index.Name);
                    renderer.WriteChar(',');
                }
                renderer.WriteString(Base.Name);
            }
            renderer.WriteChar(')');
        }
    }
}
