#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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

                // 0F 38 00
                new PrefixedDecoder(
                    Opcode.pshufb, "Pq,Qq",
                    Opcode.vpshufb, "Vx,Hx,Wx"),
                new PrefixedDecoder(
                    Opcode.phaddw, "Pq,Qq",
                    Opcode.vphaddw, "Vx,Hx,Wx"),
                new PrefixedDecoder(
                    Opcode.phaddd, "Pq,Qq",
                    Opcode.vphaddd, "Vx,Hx,Wx"),
                new PrefixedDecoder(
                    Opcode.phaddsw, "Pq,Qq",
                    Opcode.vphaddsw, "Vx,Hx,Wx"),

                new PrefixedDecoder(
                    Opcode.pmaddubsw, "Pq,Qq",
                    Opcode.vpmaddubsw, "Vx,Hx,Wx"),
                new PrefixedDecoder(
                    Opcode.phsubw, "Pq,Qq",
                    Opcode.vphsubw, "Vx,Hx,Wx"),
                new PrefixedDecoder(
                    Opcode.phsubd, "Pq,Qq",
                    Opcode.vphsubd, "Vx,Hx,Wx"),
                new PrefixedDecoder(
                    Opcode.phsubsw, "Pq,Qq",
                    Opcode.vphsubsw, "Vx,Hx,Wx"),

                new PrefixedDecoder(
                    Opcode.psignb, "Pq,Qq",
                    Opcode.vpsignb, "Vx,Hx,Wx"),
                new PrefixedDecoder(
                    Opcode.psignw, "Pq,Qq",
                    Opcode.vpsignw, "Vx,Hx,Wx"),
                new PrefixedDecoder(
                    Opcode.psignd, "Pq,Qq",
                    Opcode.vpsignd, "Vx,Hx,Wx"),
                new PrefixedDecoder(
                    Opcode.pmulhrsw, "Pq,Qq",
                    Opcode.vpmulhrsw, "Vx,Hx,Wx"),

                new PrefixedDecoder(
                    Opcode.illegal, "",
                    Opcode.vpermilps, "Vx,Hx,Wx"),
                new PrefixedDecoder(
                    Opcode.illegal, "",
                    Opcode.vpermilpd, "Vx,Hx,Wx"),
                new PrefixedDecoder(
                    Opcode.illegal, "",
                    Opcode.vtestps, "Vx,Wx"),
                new PrefixedDecoder(
                    Opcode.illegal, "",
                    Opcode.vtestpd, "Vx,Wx"),

                // 0F 38 10
                new PrefixedDecoder(
                    Opcode.illegal, "",
                    Opcode.pblendvb, "Vdq,Wdq"),
                s_invalid,
                s_invalid,
                new PrefixedDecoder(
                    Opcode.illegal, "",
                    Opcode.vcvtph2ps, "Vx,Wx,Ib"),

                new PrefixedDecoder(
                    Opcode.illegal, "",
                    Opcode.blendvps, "Vdq,Wdq"),
                new PrefixedDecoder(
                    Opcode.illegal, "",
                    Opcode.blendvpd, "Vdq,Wdq"),
                new PrefixedDecoder(
                    Opcode.illegal, "",
                    Opcode.vpermps, "Vqq,Hqq,Wqq"),
                new PrefixedDecoder(
                    Opcode.illegal, "",
                    Opcode.vptest, "Vx,Wx"),

                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,

                // 0F 38 20
                new PrefixedDecoder(
                    Opcode.illegal, "",
                    Opcode.pblendvdb, "Vdq,Wdq"),
                new PrefixedDecoder(
                    Opcode.illegal, "",
                    Opcode.vpmovsxbd, "Vx,Wx,Ib"),
                new PrefixedDecoder(
                    Opcode.illegal, "",
                    Opcode.vpmovsxbq, "Vx,Wx,Ib"),
                new PrefixedDecoder(
                    Opcode.illegal, "",
                    Opcode.vpmovsxwd, "Vx,Wx,Ib"),

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

                // 0F 38 80
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
                new PrefixedDecoder(Opcode.illegal, "", Opcode.vfnmsub231ps, "Vx,Hx,Wx"),
                new PrefixedDecoder(Opcode.illegal, "", Opcode.vfnmsub231ss, "Vx,Hx,Wx"),

                // 0F 38 C0
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,

                new PrefixedDecoder(Opcode.sha1nexte, "Vdq,Wdq"),
                new PrefixedDecoder(Opcode.sha1msg1, "Vdq,Wdq"),
                new PrefixedDecoder(Opcode.sha1msg2, "Vdq,Wdq"),
                new PrefixedDecoder(Opcode.sha256mds2, "Vdq,Wdq"),
                new PrefixedDecoder(Opcode.sha256msg1, "Vdq,Wdq"),
                new PrefixedDecoder(Opcode.sha256msg2, "Vdq,Wdq"),
                s_invalid,
                s_invalid,

                // 0F 38 D0
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

                new PrefixedDecoder(
                    Opcode.illegal, "",
                    Opcode.vaesenc, "Vdq,Hdq,Wdq"),
                new PrefixedDecoder(
                    Opcode.illegal, "",
                    Opcode.vaesenclast, "Vdq,Hdq,Wdq"),
                new PrefixedDecoder(
                    Opcode.illegal, "",
                    Opcode.vaesdec, "Vdq,Hdq,Wdq"),
                new PrefixedDecoder(
                    Opcode.illegal, "",
                    Opcode.vaesdeclast, "Vdq,Hdq,Wdq"),

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
