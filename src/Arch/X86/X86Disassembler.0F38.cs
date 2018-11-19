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
using System.Threading.Tasks;

namespace Reko.Arch.X86
{
    public partial class X86Disassembler
    {
        private static Decoder[] Create0F38Oprecs()
        {
            return new Decoder[] {

                // 00
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,

                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,

                // 10
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,

                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,

                // 20
                new PrefixedDecoder(
                    Opcode.illegal, "",
                    Opcode.pblendvdb, "Vdq,Wdq"),
                s_invalid,
                s_invalid,
                new PrefixedDecoder(
                    Opcode.illegal, "",
                    Opcode.cvtph2ps, "Vx,Wx,Ib"),
                new PrefixedDecoder(
                    Opcode.illegal, "",
                    Opcode.blendvps, "Vdq,Wdq"),
                new PrefixedDecoder(
                    Opcode.illegal, "",
                    Opcode.blendvpd, "Vdq,Wdq"),
                s_invalid,
                s_invalid,

                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,

                // 30
                new PrefixedDecoder(
                    Opcode.illegal, "",
                    Opcode.vpmovsxbw, "Vx,Mq"),
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,

                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,

                // 40
                s_nyi,
                s_nyi,
                s_invalid,
                s_invalid,
                s_invalid,
                s_nyi,
                s_nyi,
                s_nyi,

                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,

                // 50
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,

                s_nyi,
                s_nyi,
                s_nyi,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,

                // 60
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,

                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,

                // 70
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,

                s_nyi,
                s_nyi,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,

                // 80
                new PrefixedDecoder(
                    Opcode.illegal, "",
                    Opcode.invept, "Gy,Mdq"),
                new PrefixedDecoder(
                    Opcode.illegal, "",
                    Opcode.invvpid, "Gy,Mdq"),
                new PrefixedDecoder(
                    Opcode.illegal, "",
                    Opcode.invpcid, "Gy,Mdq"),
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,

                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_nyi,
                s_invalid,
                s_nyi,
                s_invalid,

                // 90
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,

                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,

                // A0
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_nyi,
                s_nyi,

                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,


                // B0
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_nyi,
                s_nyi,

                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,

                // C0
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,

                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,

                // D0
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,

                s_invalid,
                s_invalid,
                s_invalid,
                new PrefixedDecoder(
                    Opcode.illegal, "",
                    Opcode.aesimc, "Vdq,Wdq"),
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,

                // E0
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,

                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,

                // F0
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_invalid,
                s_nyi,
                s_nyi,
                s_nyi,

                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
            };
        }
    }
}
