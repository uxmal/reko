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
                new PrefixedDecoder(dec66: VexInstr(Mnemonic.vpermq, Vqq,Wqq,Ib)),
                new PrefixedDecoder(dec66: VexInstr(Mnemonic.vpermpd, Vqq,Wqq,Ib)),
                new PrefixedDecoder(dec66: VexInstr(Mnemonic.vpblendd, Vx,Hx,Wx,Ib)),
                s_invalid,

                new PrefixedDecoder(dec66: VexInstr(Mnemonic.vpermilps, Vx,Wx,Ib)),
                new PrefixedDecoder(dec66: VexInstr(Mnemonic.vpermilpd, Vx,Wx,Ib)),
                new PrefixedDecoder(dec66: VexInstr(Mnemonic.vperm2f128, Vqq,Hqq,Wqq,Ib)),
                s_invalid,

                new PrefixedDecoder(dec66: VexInstr(Mnemonic.roundps, Mnemonic.vroundps, Vx,Wx,Ib)),
                new PrefixedDecoder(dec66: VexInstr(Mnemonic.roundpd, Mnemonic.vroundpd, Vx,Wx,Ib)),
                new PrefixedDecoder(dec66: VexInstr(Mnemonic.roundss, Mnemonic.vroundss, Vss,Wss,Ib)),
                new PrefixedDecoder(dec66: VexInstr(Mnemonic.roundsd, Mnemonic.vroundsd, Vsd,Wsd,Ib)),

                new PrefixedDecoder(dec66: VexInstr(Mnemonic.blendps, Mnemonic.vblendps, Vx,Hx,Wx,Ib)),
                new PrefixedDecoder(dec66: VexInstr(Mnemonic.blendpd, Mnemonic.vblendpd, Vx,Hx,Wx,Ib)),
                new PrefixedDecoder(dec66: VexInstr(Mnemonic.blendw, Mnemonic.vblendw, Vx,Hx,Wx,Ib)),
                new PrefixedDecoder(
                    dec:Instr(Mnemonic.palignr, Pq,Qq,Ib),
                    dec66:Instr(Mnemonic.palignr, Vx,Hx,Wx,Ib)),

                // 10
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                new PrefixedDecoder(dec66: MemReg(
                    VexInstr(Mnemonic.pextrb, Mnemonic.vpextrb, Mb,Vdq,Ib),
                    VexInstr(Mnemonic.pextrb, Mnemonic.vpextrb, Rd,Vdq,Ib))),
                new PrefixedDecoder(dec66: MemReg( 
                    VexInstr(Mnemonic.pextrw, Mnemonic.vpextrw, Mw,Vdq,Ib),
                    VexInstr(Mnemonic.pextrw, Mnemonic.vpextrw, Rd,Vdq,Ib))),
                new PrefixedDecoder(dec66: DataWidthDependent(
                    bit32: VexInstr(Mnemonic.pextrd, Mnemonic.vpextrd, Ey,Vdq,Ib),
                    bit64: VexInstr(Mnemonic.pextrq, Mnemonic.vpextrq, Ey,Vdq,Ib))),
                new PrefixedDecoder(dec66: VexInstr(Mnemonic.extractps, Mnemonic.vextractps, Ed,Vdq,Ib)),

                new PrefixedDecoder(dec66: VexInstr(Mnemonic.vinsertf128, Vqq,Hqq,Wqq,Ib)),
                new PrefixedDecoder(dec66: VexInstr(Mnemonic.vextractf128, Wdq,Vqq,Ib)),
                s_invalid,
                s_invalid,

                s_invalid,
                new PrefixedDecoder(dec66: VexInstr(Mnemonic.vcvtps2ph, Wx,Vx,Ib)),
                s_invalid,
                s_invalid,

                // 20
                new PrefixedDecoder(dec66: MemReg(
                    VexInstr(Mnemonic.pinsrb, Mnemonic.vpinsrb, Vdq,Hdq,Mb,Ib),
                    VexInstr(Mnemonic.pinsrb, Mnemonic.vpinsrb, Vdq,Hdq,Ry,Ib))),
                new PrefixedDecoder(dec66: MemReg(
                    VexInstr(Mnemonic.inserps, Mnemonic.vinserps, Vdq,Hdq,Md,Ib),
                    VexInstr(Mnemonic.inserps, Mnemonic.vinserps, Vdq,Hdq,Udq,Ib))),
                new PrefixedDecoder(dec66: DataWidthDependent(
                    bit32: VexInstr(Mnemonic.pinsrd, Mnemonic.vpinsrd, Vdq,Hdq,Ey,Ib),
                    bit64: VexInstr(Mnemonic.pinsrq, Mnemonic.vpinsrq, Vdq,Hdq,Ey,Ib))),
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

                // 30
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,

                new PrefixedDecoder(dec66: VexInstr(Mnemonic.vinserti128, Vqq,Hqq,Wqq,Ib)),
                new PrefixedDecoder(dec66: VexInstr(Mnemonic.vextracti128, Wdq,Vqq,Ib)),
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,

                // 40
                new PrefixedDecoder(dec66:VexInstr(Mnemonic.dpps, Mnemonic.vdpps, Vx,Hx,Wx,Ib)),
                new PrefixedDecoder(dec66:VexInstr(Mnemonic.dppd, Mnemonic.vdppd, Vdq,Hdq,Wdq,Ib)),
                new PrefixedDecoder(dec66:VexInstr(Mnemonic.mpsadbw, Mnemonic.vmpsadbw, Vx,Hx,Wx,Ib)),
                s_invalid,
                new PrefixedDecoder(dec66:VexInstr(Mnemonic.pclmulqdq, Mnemonic.vpclmulqdq, Vdq,Hdq,Wdq,Ib)),
                s_invalid,
                new PrefixedDecoder(dec66:VexInstr(Mnemonic.vperm2i128, Vqq,Hqq,Wqq,Ib)),
                s_invalid,

                s_invalid,
                s_invalid,
                new PrefixedDecoder(dec66:VexInstr(Mnemonic.vblendvps, Vx,Hx,Wx,Lx)),
                new PrefixedDecoder(dec66:VexInstr(Mnemonic.vblendvpd, Vx,Hx,Wx,Lx)),
                new PrefixedDecoder(dec66:VexInstr(Mnemonic.vblendvb, Vx,Hx,Wx,Lx)),
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
                new PrefixedDecoder(dec66: VexInstr(Mnemonic.pcmpestrm, Mnemonic.vpcmpestrm, Vdq,Wdq,Ib)),
                new PrefixedDecoder(dec66: VexInstr(Mnemonic.pcmpestri, Mnemonic.vpcmpestri, Vdq,Wdq,Ib)),
                new PrefixedDecoder(dec66: VexInstr(Mnemonic.pcmpistrm, Mnemonic.vpcmpistrm, Vdq,Wdq,Ib)),
                new PrefixedDecoder(dec66: VexInstr(Mnemonic.pcmpistri, Mnemonic.vpcmpistri, Vdq,Wdq,Ib)),
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
                Instr(Mnemonic.sha1rnds4, Vdq,Wdq,Ib),
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
                new PrefixedDecoder(dec66: VexInstr(Mnemonic.aeskeygen, Mnemonic.vaeskeygen, Vdq,Wdq,Ib)),

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
                new PrefixedDecoder(decF2: VexInstr(Mnemonic.rorx, Gy,Ey,Ib)),
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
