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
using Reko.Core.Machine;
using Reko.Core.Types;

namespace Reko.Arch.CompactRisc
{
    public class MemoryOperand : AbstractMachineOperand
    {
        private MemoryOperand(PrimitiveType width, Storage? b, Storage? i, int d) : base(width)
        {
            this.Base = b;
            this.Index = i;
            this.Displacement = d;
        }

        public static MemoryOperand Absolute(PrimitiveType dt, uint uAddr)
        {
            return new MemoryOperand(dt, null, null, (int) uAddr);
        }

        public static MemoryOperand Relative(PrimitiveType dt, Storage? b, int disp)
        {
            return new MemoryOperand(dt, b, null, disp);
        }

        public static MemoryOperand Indexed(PrimitiveType dt, Storage? i, Storage? b, int disp)
        {
            return new MemoryOperand(dt, b, i, disp);
        }


        public Storage? Base { get; }
        public Storage? Index { get; }
        public int Displacement { get; }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            if (Index is not null || Base is not null)
            {
                if (Index is not null)
                {
                    renderer.WriteFormat("[{0}]", Index);
                }
                if (Displacement != 0)
                {
                    var d = Displacement;
                    if (d < 0)
                    {
                        renderer.WriteChar('-');
                        d = -d;
                    }
                    renderer.WriteFormat(d > 9 ? "0x{0:X}" : "{0}", d);
                }
                if (Base is not null)
                {
                    renderer.WriteChar('(');
                    if (Base is SequenceStorage seq)
                    {
                        Cr16Instruction.RenderRegisterPair(seq, renderer);
                    }
                    else
                    {
                        renderer.WriteString(Base.ToString());
                    }
                    renderer.WriteChar(')');
                }
            }
            else
            {
                renderer.WriteFormat("(0x{0:X6})", Displacement);
            }
        }
    }
}
