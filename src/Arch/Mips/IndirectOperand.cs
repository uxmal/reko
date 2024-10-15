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
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Reko.Arch.Mips
{
    public class IndirectOperand : AbstractMachineOperand
    {
        public IndirectOperand(PrimitiveType dataWidth, MachineOperand offset, RegisterStorage baseReg) : base(dataWidth)
        {
            this.Offset = offset;
            this.Base = baseReg;
        }

        public MachineOperand Offset { get; set; }

        public RegisterStorage Base { get; }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            if (Offset is SliceOperand slice)
            {
                slice.Render(renderer, options);
                renderer.WriteFormat("({0})", Base);
                return;
            }
            string fmt;
            int offset = ((ImmediateOperand) Offset).Value.ToInt32();
            if (offset >= 0)
            {
                fmt = "{0:X4}({1})";
            }
            else 
            {
                fmt = "-{0:X4}({1})";
                offset = -offset;
            }
            renderer.WriteFormat(fmt, offset, Base);
        }

        public int IntOffset()
        {
            ImmediateOperand offset;
            if (Offset is SliceOperand slice)
                offset = slice.Value;
            else
                offset = (ImmediateOperand) Offset;
            return offset.Value.ToInt32();
        }
    }
}
