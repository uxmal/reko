#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Expressions;
using Decompiler.Core.Machine;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Decompiler.Arch.Arm
{
    public class RegisterRangeOperand : MachineOperand
    {
        private uint instr;

        public RegisterRangeOperand(uint instr): base(PrimitiveType.Word32)
        {
            this.instr = instr;
        }

        public override void Write(bool fExplicit, TextWriter writer)
        {
            writer.Write('{');
            {
                uint w = instr & 0xFFFF;
                int i = 0;
                while (w != 0)
                {
                    int j;
                    while ((w & (1u << i)) == 0)
                        ++i;
                    for (j = i + 1; (w & (1u << j)) != 0; ++j)
                        ;
                    --j;
                    // registers [i..j] 
                    writer.Write(A32Registers.GpRegs[i].Name);
                    if (j - i != 0)
                    {
                        writer.Write((j - i > 1) ? '-' : ',');
                        writer.Write(A32Registers.GpRegs[i].Name);
                    }
                    i = j; w = (w >> (j + 1)) << (j + 1);
                    if (w != 0) writer.Write(',');
                }
            }
            writer.Write('}');
            if ((instr & (1 << 22)) != 0) writer.Write('^');
        }
    }
}
