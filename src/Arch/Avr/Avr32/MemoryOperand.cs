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
using System.Text;

namespace Reko.Arch.Avr.Avr32
{
    public class MemoryOperand : AbstractMachineOperand
    {
        private MemoryOperand(PrimitiveType dt) 
            : base(dt)
        {
        }

        public RegisterStorage? Base { get; private set; }
        public int Offset { get; private set; }
        public RegisterStorage? Index { get; private set; }
        public RegisterPart IndexPart { get; private set; }
        public int Shift { get; private set; }
        public bool PostIncrement { get; private set; }
        public bool PreDecrement { get; private set; }

        public static MemoryOperand Displaced(PrimitiveType dt, RegisterStorage baseReg, int offset)
        {
            var mem = new MemoryOperand(dt)
            {
                Base = baseReg,
                Offset = offset
            };
            return mem;
        }

        public static MemoryOperand PostInc(PrimitiveType dt, RegisterStorage reg)
        {
            var mem = new MemoryOperand(dt)
            {
                Base = reg,
                PostIncrement = true,
            };
            return mem;
        }

        internal static MachineOperand PreDec(PrimitiveType dt, RegisterStorage reg)
        {
            var mem = new MemoryOperand(dt)
            {
                Base = reg,
                PreDecrement = true,
            };
            return mem;
        }

        public static MachineOperand Indexed(
            PrimitiveType dt, 
            RegisterStorage baseReg, 
            RegisterStorage x,
            int shift,
            RegisterPart part = RegisterPart.All)
        {
            var mem = new MemoryOperand(dt)
            {
                Base = baseReg,
                Index = x,
                Shift = shift,
                IndexPart = part,
            };
            return mem;
        }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            if (this.PreDecrement)
            {
                renderer.WriteString("--");
                renderer.WriteString(Base!.Name);
                return;
            }
            if (this.PostIncrement)
            {
                renderer.WriteString(Base!.Name);
                renderer.WriteString("++");
                return;
            }
            renderer.WriteString(Base!.Name);
            renderer.WriteString("[");
            if (Index is not null)
            {
                renderer.WriteString(Index.Name);
                if (IndexPart != RegisterPart.All)
                {
                    renderer.WriteString(IndexPart.Format());
                }
                if (Shift > 0)
                {
                    renderer.WriteFormat("<<{0}", Shift);
                }
            }
            else
            {
                renderer.WriteFormat("{0}", Offset);
            }
            renderer.WriteString("]");
        }
    }
}
