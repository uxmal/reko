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
using System.Threading.Tasks;

namespace Reko.Arch.X86
{
    public partial class X86Disassembler
    {
        private static Decoder[] Create0F38Decoders()
        {
            return new Decoder[] {

                // 0F 38 00
                new PrefixedDecoder(
                    Instr(Mnemonic.pshufb, Pq,Qq),
                    Instr(Mnemonic.vpshufb, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.phaddw, Pq,Qq),
                    Instr(Mnemonic.vphaddw, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.phaddd, Pq,Qq),
                    Instr(Mnemonic.vphaddd, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.phaddsw, Pq,Qq),
                    Instr(Mnemonic.vphaddsw, Vx,Hx,Wx)),

                new PrefixedDecoder(
                    Instr(Mnemonic.pmaddubsw, Pq,Qq),
                    Instr(Mnemonic.vpmaddubsw, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.phsubw, Pq,Qq),
                    Instr(Mnemonic.vphsubw, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.phsubd, Pq,Qq),
                    Instr(Mnemonic.vphsubd, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.phsubsw, Pq,Qq),
                    Instr(Mnemonic.vphsubsw, Vx,Hx,Wx)),

                new PrefixedDecoder(
                    Instr(Mnemonic.psignb, Pq,Qq),
                    Instr(Mnemonic.vpsignb, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.psignw, Pq,Qq),
                    Instr(Mnemonic.vpsignw, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.psignd, Pq,Qq),
                    Instr(Mnemonic.vpsignd, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.pmulhrsw, Pq,Qq),
                    Instr(Mnemonic.vpmulhrsw, Vx,Hx,Wx)),

                new PrefixedDecoder(
                    s_invalid,
                    Instr(Mnemonic.vpermilps, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    s_invalid,
                    Instr(Mnemonic.vpermilpd, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    s_invalid,
                    Instr(Mnemonic.vtestps, Vx,Wx)),
                new PrefixedDecoder(
                    s_invalid,
                    Instr(Mnemonic.vtestpd, Vx,Wx)),

                // 0F 38 10
                new PrefixedDecoder(
                    s_invalid,
                    Instr(Mnemonic.pblendvb, Vdq,Wdq)),
                s_invalid,
                s_invalid,
                new PrefixedDecoder(
                    s_invalid,
                    Instr(Mnemonic.vcvtph2ps, Vx,Wx,Ib)),

                new PrefixedDecoder(
                    s_invalid,
                    Instr(Mnemonic.blendvps, Vdq,Wdq)),
                new PrefixedDecoder(
                    s_invalid,
                    Instr(Mnemonic.blendvpd, Vdq,Wdq)),
                new PrefixedDecoder(
                    s_invalid,
                    Instr(Mnemonic.vpermps, Vqq,Hqq,Wqq)),
                new PrefixedDecoder(
                    s_invalid,
                    Instr(Mnemonic.vptest, Vx,Wx)),

                new PrefixedDecoder(
                    dec66: Instr(Mnemonic.vbroadcastss, Vx,Wd)),
                new PrefixedDecoder(
                    dec66: Instr(Mnemonic.vbroadcastsd, Vqq,Wq)),
                new PrefixedDecoder(
                    dec66: Instr(Mnemonic.vbroadcastf128, Vqq,Mdq)),
                s_invalid,

                new PrefixedDecoder(
                    dec: Instr(Mnemonic.pabsb, Pq,Qq),
                    dec66: Instr(Mnemonic.vpabsb, Vx,Wx)),
                new PrefixedDecoder(
                    dec: Instr(Mnemonic.pabsw, Pq,Qq),
                    dec66: Instr(Mnemonic.vpabsw, Vx,Wx)),
                new PrefixedDecoder(
                    dec: Instr(Mnemonic.pabsd, Pq,Qq),
                    dec66: Instr(Mnemonic.vpabsd, Vx,Wx)),
                s_invalid,

                // 0F 38 20
                new PrefixedDecoder(dec66: Instr(Mnemonic.vpmovsxbw, Vx,Mq)),
                new PrefixedDecoder(dec66: Instr(Mnemonic.vpmovsxbd, Vx,Mq)),
                new PrefixedDecoder(dec66: Instr(Mnemonic.vpmovsxbq, Vx,Mq)),
                new PrefixedDecoder(dec66: Instr(Mnemonic.vpmovsxwd, Vx,Mq)),

                new PrefixedDecoder(dec66: Instr(Mnemonic.vpmovsxwq, Vx,Mq)),
                new PrefixedDecoder(dec66: Instr(Mnemonic.vpmovsxdq, Vx,Mq)),
                s_invalid,
                s_invalid,

                new PrefixedDecoder(dec66: Instr(Mnemonic.vpmuldq, Vx,Hx,Wx)),
                new PrefixedDecoder(dec66: Instr(Mnemonic.vpcmpeqq, Vx,Hx,Wx)),
                new PrefixedDecoder(dec66: Instr(Mnemonic.vmovntdqa, Vx,Mx,Wx)),
                new PrefixedDecoder(dec66: Instr(Mnemonic.vpackusdw, Vx,Hx,Wx)),

                new PrefixedDecoder(dec66: Instr(Mnemonic.vmaskmovps, Vx,Hx,Mx)),
                new PrefixedDecoder(dec66: Instr(Mnemonic.vmaskmovpd, Vx,Hx,Mx)),
                new PrefixedDecoder(dec66: Instr(Mnemonic.vmaskmovps, Mx,Hx,Vx)),
                new PrefixedDecoder(dec66: Instr(Mnemonic.vmaskmovpd, Mx,Hx,Vx)),

                // 30
                new PrefixedDecoder(dec66: Instr(Mnemonic.vpmovzxbw, Vx,Mq)),
                new PrefixedDecoder(dec66: Instr(Mnemonic.vpmovzxbd, Vx,Mq)),
                new PrefixedDecoder(dec66: Instr(Mnemonic.vpmovzxbq, Vx,Mq)),
                new PrefixedDecoder(dec66: Instr(Mnemonic.vpmovzxwd, Vx,Mq)),

                new PrefixedDecoder(dec66: Instr(Mnemonic.vpmovzxwq, Vx,Mq)),
                new PrefixedDecoder(dec66: Instr(Mnemonic.vpmovzxdq, Vx,Mq)),
                new PrefixedDecoder(dec66: Instr(Mnemonic.vpermd, Vqq,Hqq,Wqq)),
                new PrefixedDecoder(dec66: Instr(Mnemonic.vpcmpgtq, Vx,Hx,Wx)),

                new PrefixedDecoder(dec66: Instr(Mnemonic.vpminsb, Vx,Hx,Wx)),
                new PrefixedDecoder(dec66: Instr(Mnemonic.vpminsd, Vx,Hx,Wx)),
                new PrefixedDecoder(dec66: Instr(Mnemonic.vpminuw, Vx,Hx,Wx)),
                new PrefixedDecoder(dec66: Instr(Mnemonic.vpminud, Vx,Hx,Wx)),

                new PrefixedDecoder(dec66: Instr(Mnemonic.vpmaxsb, Vx,Hx,Wx)),
                new PrefixedDecoder(dec66: Instr(Mnemonic.vpmaxsd, Vx,Hx,Wx)),
                new PrefixedDecoder(dec66: Instr(Mnemonic.vpmaxuw, Vx,Hx,Wx)),
                new PrefixedDecoder(dec66: Instr(Mnemonic.vpmaxud, Vx,Hx,Wx)),

                // 40
                new PrefixedDecoder(dec66: Instr(Mnemonic.vpmulld, Vx,Hx,Wx)),
                new PrefixedDecoder(dec66: Instr(Mnemonic.vphminposuw, Vdq,Wdq)),
                s_invalid,
                s_invalid,
                s_invalid,
                new PrefixedDecoder(
                    dec66: Instr(Mnemonic.vpsrlvd, Vx,Hx,Wx),
                    dec66Wide: Instr(Mnemonic.vpsrlvq, Vx,Hx,Wx)),
                new PrefixedDecoder(dec66: Instr(Mnemonic.vpsravd, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    dec66: Instr(Mnemonic.vpsllvd, Vx,Hx,Wx),
                    dec66Wide: Instr(Mnemonic.vpsllvq, Vx,Hx,Wx)),

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

                new PrefixedDecoder(dec66:Instr(Mnemonic.vpbroadcastd, Vx,Wx)),
                new PrefixedDecoder(dec66:Instr(Mnemonic.vpbroadcastq, Vx,Wx)),
                new PrefixedDecoder(dec66:Instr(Mnemonic.vpbroadcasti128, Vqq,Mdq)),
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

                new PrefixedDecoder(dec66: Instr(Mnemonic.vbroadcastb, Vx,Eb)),
                new PrefixedDecoder(dec66: Instr(Mnemonic.vbroadcastw, Vx,Wx)),
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,

                // 0F 38 80
                new PrefixedDecoder(
                    s_invalid,
                    Instr(Mnemonic.invept, Gy,Mdq)),
                new PrefixedDecoder(
                    s_invalid,
                    Instr(Mnemonic.invvpid, Gy,Mdq)),
                new PrefixedDecoder(
                    s_invalid,
                    Instr(Mnemonic.invpcid, Gy,Mdq)),
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
                    dec66: Instr(Mnemonic.vgatherdd, Vx,Hx,Wx),
                    dec66Wide: Instr(Mnemonic.vgatherdq, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    dec66: Instr(Mnemonic.vgatherqd, Vx,Hx,Wx),
                    dec66Wide: Instr(Mnemonic.vgatherqq, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    dec66: Instr(Mnemonic.vgatherdps, Vx,Hx,Wx),
                    dec66Wide: Instr(Mnemonic.vgatherdpd, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    dec66: Instr(Mnemonic.vgatherqps, Vx,Hx,Wx),
                    dec66Wide: Instr(Mnemonic.vgatherqpd, Vx,Hx,Wx)),

                s_invalid,
                s_invalid,
                new PrefixedDecoder(
                    dec66: Instr(Mnemonic.vfmaddsub132ps, Vx,Hx,Wx),
                    dec66Wide: Instr(Mnemonic.vfmaddsub132pd, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    dec66: Instr(Mnemonic.vfmsubadd132ps, Vx,Hx,Wx),
                    dec66Wide: Instr(Mnemonic.vfmsubadd132pd, Vx,Hx,Wx)),

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
                    dec66: Instr(Mnemonic.vfmadd213ps, Vx,Hx,Wx),
                    dec66Wide: Instr(Mnemonic.vfmadd213pd, Vx,Hx,Wx)),
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
                    dec66:Instr(Mnemonic.vfnmsub231ps, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    s_invalid,
                    dec66:Instr(Mnemonic.vfnmsub231ss, Vx,Hx,Wx)),

                // 0F 38 C0
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,

                new PrefixedDecoder(Instr(Mnemonic.sha1nexte, Vdq,Wdq)),
                new PrefixedDecoder(Instr(Mnemonic.sha1msg1, Vdq,Wdq)),
                new PrefixedDecoder(Instr(Mnemonic.sha1msg2, Vdq,Wdq)),
                new PrefixedDecoder(Instr(Mnemonic.sha256mds2, Vdq,Wdq)),
                new PrefixedDecoder(Instr(Mnemonic.sha256msg1, Vdq,Wdq)),
                new PrefixedDecoder(Instr(Mnemonic.sha256msg2, Vdq,Wdq)),
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
                    Instr(Mnemonic.aesimc, Vdq,Wdq)),

                new PrefixedDecoder(
                    s_invalid,
                    Instr(Mnemonic.vaesenc, Vdq,Hdq,Wdq)),
                new PrefixedDecoder(
                    s_invalid,
                    Instr(Mnemonic.vaesenclast, Vdq,Hdq,Wdq)),
                new PrefixedDecoder(
                    s_invalid,
                    Instr(Mnemonic.vaesdec, Vdq,Hdq,Wdq)),
                new PrefixedDecoder(
                    s_invalid,
                    Instr(Mnemonic.vaesdeclast, Vdq,Hdq,Wdq)),

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
