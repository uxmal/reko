#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.X86
{
    public partial class X86Disassembler
    {
        private static OpRec[] Create0F3AOprecs()
        {
            return new OpRec[] {

                // 00
                new PrefixedOpRec(
                    Opcode.illegal, "",
                    Opcode.vpermq, "Vqq,Wqq,Ib"),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),

                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new PrefixedOpRec(
                    Opcode.palignr, "Pq,Qq,Ib",
                    Opcode.palignr, "Vx,Wx,Ib"),

                // 10
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),

                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),

                // 20
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),

                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),

                // 30
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),

                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),

                // 40
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),

                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),

                // 50
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),

                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),

                // 60
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new PrefixedOpRec(
                    Opcode.illegal, null,
                    Opcode.pcmpistri, "Vx,Wx,Ib"),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),

                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),

                // 70
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),

                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),

                // 80
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),

                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),

                // 90
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),

                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),

                // A0
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),

                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),


                // B0
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),

                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),

                // C0
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),

                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),

                // D0
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),

                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),

                // E0
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),

                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),

                // F0
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),

                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),

            };
        }
    }
}
