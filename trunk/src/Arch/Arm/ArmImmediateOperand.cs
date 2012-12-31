#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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
using System.Linq;
using System.IO;
using System.Text;

namespace Decompiler.Arch.Arm
{
    public class ArmImmediateOperand : ImmediateOperand
    {
        private ArmImmediateOperand(Constant c)
            : base(c)
        {
        }

        private new static ArmImmediateOperand Word32(int w) { return new ArmImmediateOperand(Constant.Word32(w)); }

        public override void Write(bool fExplicit, TextWriter writer)
        {
            int imm8 = Value.ToInt32();
            if (imm8 > 256 && ((imm8 & (imm8 - 1)) == 0))
            {
                /* only one bit set, and that later than bit 8.
                 * Represent as 1<<... .
                 */
                writer.Write("1<<");
                {
                    uint n = 0;
                    while ((imm8 & 15) == 0)
                    {
                        n += 4; imm8 = imm8 >> 4;
                    }
                    // Now imm8 is 1, 2, 4 or 8. 
                    n += (uint)((0x30002010 >> (int)(4 * (imm8 - 1))) & 15);
                    writer.Write(n);
                }
            }
            else
            {
                if (((int)imm8) < 0 && ((int)imm8) > -100)
                {
                    writer.Write('-'); imm8 = -imm8;
                }
                writer.Write(imm8);
            }
        }
    }
}
