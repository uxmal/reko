#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
        private static Decoder[] Create0F3ADecoders()
        {
            return new Decoder[] {

                // 00
                new PrefixedDecoder(dec66: Instr(Mnemonic.vpermq, Vqq,Wqq,Ib)),
                new PrefixedDecoder(dec66: Instr(Mnemonic.vpermpd, Vqq,Wqq,Ib)),
                new PrefixedDecoder(dec66: Instr(Mnemonic.vpblendd, Vx,Hx,Wx,Ib)),
                s_invalid,

                new PrefixedDecoder(dec66: Instr(Mnemonic.vpermilps, Vx,Wx,Ib)),
                new PrefixedDecoder(dec66: Instr(Mnemonic.vpermilpd, Vx,Wx,Ib)),
                new PrefixedDecoder(dec66: Instr(Mnemonic.vperm2f128, Vqq,Hqq,Wqq,Ib)),
                s_invalid,

                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                new PrefixedDecoder(
                    dec:Instr(Mnemonic.palignr, Pq,Qq,Ib),
                    dec66:Instr(Mnemonic.palignr, Vx,Wx,Ib)),

                // 10
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
                s_invalid,
                s_invalid,
                s_invalid,
                s_nyi,
                s_invalid,
                s_invalid,

                // 20
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
                s_invalid,
                s_invalid,
                s_invalid,

                // 30
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

                // 40
                s_nyi,
                s_nyi,
                s_nyi,
                s_invalid,
                s_nyi,
                s_invalid,
                s_nyi,
                s_invalid,

                s_invalid,
                s_invalid,
                Instr(Mnemonic.vblendvpsv, Vx,Hx,Wx,Lx),
                Instr(Mnemonic.vblendvpdv, Vx,Hx,Wx,Lx),
                s_nyi,
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

                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,

                // 60
                s_nyi,
                s_nyi,
                s_nyi,
                new PrefixedDecoder(
                    dec66: Instr(Mnemonic.pcmpistri, Vx,Wx,Ib)),
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

                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,

                // 80
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

                // 90
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

                // A0
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

                // B0
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

                // C0
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
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
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
            };
        }
    }
}
