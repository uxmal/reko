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
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Zilog.Z80
{
    public class MemoryOperand : AbstractMachineOperand
    {
        public RegisterStorage? Base;
        public Constant? Offset;

        public MemoryOperand(RegisterStorage baseReg, PrimitiveType type): base(type)
        {
            this.Base = baseReg;
        }

        public MemoryOperand(Constant offset, PrimitiveType type) : base(type)
        {
            this.Offset = offset;
        }

        public MemoryOperand(RegisterStorage baseReg, sbyte offset, PrimitiveType type) : base(type)
        {
            this.Base = baseReg;
            this.Offset = Constant.SByte(offset);
        }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            if (Base is not null)
            {
                if (Offset is not null)
                {
                    int offset = Offset.ToInt32();
                    string fmt;
                    if (offset > 0)
                    {
                        fmt = "({0}+{1:X2})";
                    } 
                    else if (offset < 0)
                    {
                        offset = -offset;
                        fmt = "({0}-{1:X2})";
                    }
                    else 
                    {
                        fmt = "({0})";
                    }
                    renderer.WriteString(string.Format(fmt, Base, offset));
                }
                else
                {
                    renderer.WriteString(string.Format("({0})", Base));
                }
            }
            else
            {
                renderer.WriteString("(");
                renderer.WriteAddress(string.Format("{0:X4}", Offset!.ToUInt16()), Address.Ptr16(Offset.ToUInt16()));
                renderer.WriteString(")");
            }
        }
    }
}
