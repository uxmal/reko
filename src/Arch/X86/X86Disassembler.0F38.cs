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
                    Instr(Opcode.pshufb, Pq,Qq),
                    Instr(Opcode.vpshufb, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Opcode.phaddw, Pq,Qq),
                    Instr(Opcode.vphaddw, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Opcode.phaddd, Pq,Qq),
                    Instr(Opcode.vphaddd, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Opcode.phaddsw, Pq,Qq),
                    Instr(Opcode.vphaddsw, Vx,Hx,Wx)),

                new PrefixedDecoder(
                    Instr(Opcode.pmaddubsw, Pq,Qq),
                    Instr(Opcode.vpmaddubsw, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Opcode.phsubw, Pq,Qq),
                    Instr(Opcode.vphsubw, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Opcode.phsubd, Pq,Qq),
                    Instr(Opcode.vphsubd, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Opcode.phsubsw, Pq,Qq),
                    Instr(Opcode.vphsubsw, Vx,Hx,Wx)),

                new PrefixedDecoder(
                    Instr(Opcode.psignb, Pq,Qq),
                    Instr(Opcode.vpsignb, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Opcode.psignw, Pq,Qq),
                    Instr(Opcode.vpsignw, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Opcode.psignd, Pq,Qq),
                    Instr(Opcode.vpsignd, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Opcode.pmulhrsw, Pq,Qq),
                    Instr(Opcode.vpmulhrsw, Vx,Hx,Wx)),

                new PrefixedDecoder(
                    s_invalid,
                    Instr(Opcode.vpermilps, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    s_invalid,
                    Instr(Opcode.vpermilpd, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    s_invalid,
                    Instr(Opcode.vtestps, Vx,Wx)),
                new PrefixedDecoder(
                    s_invalid,
                    Instr(Opcode.vtestpd, Vx,Wx)),

                // 0F 38 10
                new PrefixedDecoder(
                    s_invalid,
                    Instr(Opcode.pblendvb, Vdq,Wdq)),
                s_invalid,
                s_invalid,
                new PrefixedDecoder(
                    s_invalid,
                    Instr(Opcode.vcvtph2ps, Vx,Wx,Ib)),

                new PrefixedDecoder(
                    s_invalid,
                    Instr(Opcode.blendvps, Vdq,Wdq)),
                new PrefixedDecoder(
                    s_invalid,
                    Instr(Opcode.blendvpd, Vdq,Wdq)),
                new PrefixedDecoder(
                    s_invalid,
                    Instr(Opcode.vpermps, Vqq,Hqq,Wqq)),
                new PrefixedDecoder(
                    s_invalid,
                    Instr(Opcode.vptest, Vx,Wx)),

                new PrefixedDecoder(
                    dec66: Instr(Opcode.vbroadcastss, Vx,Wd)),
                new PrefixedDecoder(
                    dec66: Instr(Opcode.vbroadcastsd, Vqq,Wq)),
                new PrefixedDecoder(
                    dec66: Instr(Opcode.vbroadcastf128, Vqq,Mdq)),
                s_invalid,

                new PrefixedDecoder(
                    dec: Instr(Opcode.pabsb, Pq,Qq),
                    dec66: Instr(Opcode.vpabsb, Vx,Wx)),
                new PrefixedDecoder(
                    dec: Instr(Opcode.pabsw, Pq,Qq),
                    dec66: Instr(Opcode.vpabsw, Vx,Wx)),
                new PrefixedDecoder(
                    dec: Instr(Opcode.pabsd, Pq,Qq),
                    dec66: Instr(Opcode.vpabsd, Vx,Wx)),
                s_invalid,

                // 0F 38 20
                new PrefixedDecoder(dec66: Instr(Opcode.vpmovsxbw, Vx,Mq)),
                new PrefixedDecoder(dec66: Instr(Opcode.vpmovsxbd, Vx,Mq)),
                new PrefixedDecoder(dec66: Instr(Opcode.vpmovsxbq, Vx,Mq)),
                new PrefixedDecoder(dec66: Instr(Opcode.vpmovsxwd, Vx,Mq)),

                new PrefixedDecoder(dec66: Instr(Opcode.vpmovsxwq, Vx,Mq)),
                new PrefixedDecoder(dec66: Instr(Opcode.vpmovsxdq, Vx,Mq)),
                s_invalid,
                s_invalid,

                new PrefixedDecoder(dec66: Instr(Opcode.vpmuldq, Vx,Hx,Wx)),
                new PrefixedDecoder(dec66: Instr(Opcode.vpcmpeqq, Vx,Hx,Wx)),
                new PrefixedDecoder(dec66: Instr(Opcode.vmovntdqa, Vx,Mx,Wx)),
                new PrefixedDecoder(dec66: Instr(Opcode.vpackusdw, Vx,Hx,Wx)),

                new PrefixedDecoder(dec66: Instr(Opcode.vmaskmovps, Vx,Hx,Mx)),
                new PrefixedDecoder(dec66: Instr(Opcode.vmaskmovpd, Vx,Hx,Mx)),
                new PrefixedDecoder(dec66: Instr(Opcode.vmaskmovps, Mx,Hx,Vx)),
                new PrefixedDecoder(dec66: Instr(Opcode.vmaskmovpd, Mx,Hx,Vx)),

                // 30
                new PrefixedDecoder(dec66: Instr(Opcode.vpmovzxbw, Vx,Mq)),
                new PrefixedDecoder(dec66: Instr(Opcode.vpmovzxbd, Vx,Mq)),
                new PrefixedDecoder(dec66: Instr(Opcode.vpmovzxbq, Vx,Mq)),
                new PrefixedDecoder(dec66: Instr(Opcode.vpmovzxwd, Vx,Mq)),

                new PrefixedDecoder(dec66: Instr(Opcode.vpmovzxwq, Vx,Mq)),
                new PrefixedDecoder(dec66: Instr(Opcode.vpmovzxdq, Vx,Mq)),
                new PrefixedDecoder(dec66: Instr(Opcode.vpermd, Vqq,Hqq,Wqq)),
                new PrefixedDecoder(dec66: Instr(Opcode.vpcmpgtq, Vx,Hx,Wx)),

                new PrefixedDecoder(dec66: Instr(Opcode.vpminsb, Vx,Hx,Wx)),
                new PrefixedDecoder(dec66: Instr(Opcode.vpminsd, Vx,Hx,Wx)),
                new PrefixedDecoder(dec66: Instr(Opcode.vpminuw, Vx,Hx,Wx)),
                new PrefixedDecoder(dec66: Instr(Opcode.vpminud, Vx,Hx,Wx)),

                new PrefixedDecoder(dec66: Instr(Opcode.vpmaxsb, Vx,Hx,Wx)),
                new PrefixedDecoder(dec66: Instr(Opcode.vpmaxsd, Vx,Hx,Wx)),
                new PrefixedDecoder(dec66: Instr(Opcode.vpmaxuw, Vx,Hx,Wx)),
                new PrefixedDecoder(dec66: Instr(Opcode.vpmaxud, Vx,Hx,Wx)),

                // 40
                new PrefixedDecoder(dec66: Instr(Opcode.vpmulld, Vx,Hx,Wx)),
                new PrefixedDecoder(dec66: Instr(Opcode.vphminposuw, Vdq,Wdq)),
                s_invalid,
                s_invalid,
                s_invalid,
                new PrefixedDecoder(
                    dec66: Instr(Opcode.vpsrlvd, Vx,Hx,Wx),
                    dec66Wide: Instr(Opcode.vpsrlvq, Vx,Hx,Wx)),
                new PrefixedDecoder(dec66: Instr(Opcode.vpsravd, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    dec66: Instr(Opcode.vpsllvd, Vx,Hx,Wx),
                    dec66Wide: Instr(Opcode.vpsllvq, Vx,Hx,Wx)),

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

                new PrefixedDecoder(dec66:Instr(Opcode.vpbroadcastd, "Vx,Wx")),
                new PrefixedDecoder(dec66:Instr(Opcode.vpbroadcastq, "Vx,Wx")),
                new PrefixedDecoder(dec66:Instr(Opcode.vpbroadcasti128, "Vqq,Mdq")),
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

                new PrefixedDecoder(dec66: Instr(Opcode.vbroadcastb, Vx,Eb)),
                new PrefixedDecoder(dec66: Instr(Opcode.vbroadcastw, Vx,Wx)),
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,

                // 0F 38 80
                new PrefixedDecoder(
                    s_invalid,
                    Instr(Opcode.invept, Gy,Mdq)),
                new PrefixedDecoder(
                    s_invalid,
                    Instr(Opcode.invvpid, Gy,Mdq)),
                new PrefixedDecoder(
                    s_invalid,
                    Instr(Opcode.invpcid, Gy,Mdq)),
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
                new PrefixedDecoder(
                    dec66: Instr(Opcode.vgatherdd, Vx,Hx,Wx),
                    dec66Wide: Instr(Opcode.vgatherdq, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    dec66: Instr(Opcode.vgatherqd, Vx,Hx,Wx),
                    dec66Wide: Instr(Opcode.vgatherqq, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    dec66: Instr(Opcode.vgatherdps, Vx,Hx,Wx),
                    dec66Wide: Instr(Opcode.vgatherdpd, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    dec66: Instr(Opcode.vgatherqps, Vx,Hx,Wx),
                    dec66Wide: Instr(Opcode.vgatherqpd, Vx,Hx,Wx)),

                s_invalid,
                s_invalid,
                new PrefixedDecoder(
                    dec66: Instr(Opcode.vfmaddsub132ps, Vx,Hx,Wx),
                    dec66Wide: Instr(Opcode.vfmaddsub132pd, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    dec66: Instr(Opcode.vfmsubadd132ps, Vx,Hx,Wx),
                    dec66Wide: Instr(Opcode.vfmsubadd132pd, Vx,Hx,Wx)),

                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_invalid,
                s_invalid,
                new PrefixedDecoder(dec66:nyi("vfmaddsub132ps/dv")),
                new PrefixedDecoder(dec66:nyi("vfmsubadd132ps/dv")),

                // A0
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_nyi,
                s_nyi,

                new PrefixedDecoder(
                    dec66: Instr(Opcode.vfmadd213ps, Vx,Hx,Wx),
                    dec66Wide: Instr(Opcode.vfmadd213pd, Vx,Hx,Wx)),
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
                new PrefixedDecoder(
                    s_invalid,
                    dec66:Instr(Opcode.vfnmsub231ps, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    s_invalid,
                    dec66:Instr(Opcode.vfnmsub231ss, Vx,Hx,Wx)),

                // 0F 38 C0
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,

                new PrefixedDecoder(Instr(Opcode.sha1nexte, Vdq,Wdq)),
                new PrefixedDecoder(Instr(Opcode.sha1msg1, Vdq,Wdq)),
                new PrefixedDecoder(Instr(Opcode.sha1msg2, Vdq,Wdq)),
                new PrefixedDecoder(Instr(Opcode.sha256mds2, Vdq,Wdq)),
                new PrefixedDecoder(Instr(Opcode.sha256msg1, Vdq,Wdq)),
                new PrefixedDecoder(Instr(Opcode.sha256msg2, Vdq,Wdq)),
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
                    s_invalid,
                    Instr(Opcode.aesimc, Vdq,Wdq)),

                new PrefixedDecoder(
                    s_invalid,
                    Instr(Opcode.vaesenc, Vdq,Hdq,Wdq)),
                new PrefixedDecoder(
                    s_invalid,
                    Instr(Opcode.vaesenclast, Vdq,Hdq,Wdq)),
                new PrefixedDecoder(
                    s_invalid,
                    Instr(Opcode.vaesdec, Vdq,Hdq,Wdq)),
                new PrefixedDecoder(
                    s_invalid,
                    Instr(Opcode.vaesdeclast, Vdq,Hdq,Wdq)),

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
