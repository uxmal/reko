#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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

using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.X86
{
    using Decoder = Decoder<X86Disassembler, Mnemonic, X86Instruction>;

    public partial class X86Disassembler
    {
        public partial class InstructionSet
        {
            private static void Create0F3ADecoders(Decoder[] d)
            {
                // 00
                d[0x00] = new PrefixedDecoder(dec66: VexInstr(Mnemonic.vpermq, Vqq,Wqq,Ib));
                d[0x01] = new PrefixedDecoder(dec66: VexInstr(Mnemonic.vpermpd, Vqq,Wqq,Ib));
                d[0x02] = new PrefixedDecoder(dec66: VexInstr(Mnemonic.vpblendd, Vx,Hx,Wx,Ib));
                d[0x03] = s_invalid;

                d[0x04] = new PrefixedDecoder(dec66: VexInstr(Mnemonic.vpermilps, Vx,Wx,Ib));
                d[0x05] = new PrefixedDecoder(dec66: VexInstr(Mnemonic.vpermilpd, Vx,Wx,Ib));
                d[0x06] = new PrefixedDecoder(dec66: VexInstr(Mnemonic.vperm2f128, Vqq,Hqq,Wqq,Ib));
                d[0x07] = s_invalid;

                d[0x08] = new PrefixedDecoder(dec66: EvexInstr(
                    Instr(Mnemonic.roundps, Vx,Ib),
                    Instr(Mnemonic.vroundps, Vx,Wx,Ib),
                    MemReg(
                        Instr(Mnemonic.vrndscaleps, Vx,Md,Sae,Ib),
                        Instr(Mnemonic.vrndscaleps, Vx,WBx_q,Sae,Ib))));
                d[0x09] = new PrefixedDecoder(dec66: EvexInstr(
                    Instr(Mnemonic.roundpd, Vx,Ib),
                    Instr(Mnemonic.vroundpd, Vx,Wx,Ib),
                    Instr(Mnemonic.vrndscalepd, Vx,WBx_q,Sae,Ib)));
                d[0x0A] = new PrefixedDecoder(dec66: EvexInstr(
                    Instr(Mnemonic.roundss, Vss,Wss,Ib),
                    Instr(Mnemonic.vroundss, Vss,Wss,Ib),
                    Instr(Mnemonic.vrndscaless, Vss,Wss,Sae,Ib)));
                d[0x0B] = new PrefixedDecoder(dec66: EvexInstr(
                    Instr(Mnemonic.roundsd, Vsd,Ib),
                    Instr(Mnemonic.vroundsd, Vsd,Wsd,Ib),
                    Instr(Mnemonic.vrndscalesd, Vsd,Wsd,Sae,Ib)));

                d[0x0C] = new PrefixedDecoder(dec66: VexInstr(Mnemonic.blendps, Mnemonic.vblendps, Vx,Hx,Wx,Ib));
                d[0x0D] = new PrefixedDecoder(dec66: VexInstr(Mnemonic.blendpd, Mnemonic.vblendpd, Vx,Hx,Wx,Ib));
                d[0x0E] = new PrefixedDecoder(dec66: VexInstr(Mnemonic.pblendw, Mnemonic.vpblendw, Vx,Hx,Wx,Ib));
                d[0x0F] = new PrefixedDecoder(
                    dec:Instr(Mnemonic.palignr, Pq,Qq,Ib),
                    dec66:Instr(Mnemonic.palignr, Vx,Hx,Wx,Ib));

                // 10
                d[0x10] = s_invalid;
                d[0x11] = s_invalid;
                d[0x12] = s_invalid;
                d[0x13] = s_invalid;
                d[0x14] = new PrefixedDecoder(dec66: MemReg(
                    VexInstr(Mnemonic.pextrb, Mnemonic.vpextrb, Mb,Vdq,Ib),
                    VexInstr(Mnemonic.pextrb, Mnemonic.vpextrb, Rd,Vdq,Ib)));
                d[0x15] = new PrefixedDecoder(dec66: MemReg(
                    VexInstr(Mnemonic.pextrw, Mnemonic.vpextrw, Mw,Vdq,Ib),
                    VexInstr(Mnemonic.pextrw, Mnemonic.vpextrw, Rd,Vdq,Ib)));
                d[0x16] = new PrefixedDecoder(dec66: DataWidthDependent(
                    bit32: VexInstr(Mnemonic.pextrd, Mnemonic.vpextrd, Ey,Vdq,Ib),
                    bit64: VexInstr(Mnemonic.pextrq, Mnemonic.vpextrq, Ey,Vdq,Ib)));
                d[0x17] = new PrefixedDecoder(dec66: VexInstr(Mnemonic.extractps, Mnemonic.vextractps, Ed,Vdq,Ib));

                d[0x18] = new PrefixedDecoder(
                    dec66: EvexInstr(
                        s_invalid,
                        Instr(Mnemonic.vinsertf128, Vqq,Hqq,Wqq,Ib),
                        VexLong(
                            s_invalid,
                            Instr(Mnemonic.vinsertf32x4, Vx,Hx,Wdq,Ib))),
                    dec66Wide: EvexInstr(
                        s_invalid,
                        Instr(Mnemonic.vinsertf128, Vqq,Hqq,Wqq,Ib),
                        VexLong(
                            s_invalid,
                            Instr(Mnemonic.vinsertf64x2, Vx,Hx,Wqq,Ib))));
                d[0x19] = new PrefixedDecoder(dec66: VexInstr(Mnemonic.vextractf128, Wdq,Vqq,Ib));
                d[0x1A] = s_invalid;
                d[0x1B] = s_invalid;

                d[0x1C] = s_invalid;
                d[0x1D] = new PrefixedDecoder(dec66: VexInstr(Mnemonic.vcvtps2ph, Wx,Vx,Ib));
                d[0x1E] = s_invalid;
                d[0x1F] = s_invalid;

                // 20
                d[0x20] = new PrefixedDecoder(dec66: MemReg(
                    VexInstr(Mnemonic.pinsrb, Mnemonic.vpinsrb, Vdq,Hdq,Mb,Ib),
                    VexInstr(Mnemonic.pinsrb, Mnemonic.vpinsrb, Vdq,Hdq,Ry,Ib)));
                d[0x21] = new PrefixedDecoder(dec66: MemReg(
                    VexInstr(Mnemonic.inserps, Mnemonic.vinserps, Vdq,Hdq,Md,Ib),
                    VexInstr(Mnemonic.inserps, Mnemonic.vinserps, Vdq,Hdq,Udq,Ib)));
                d[0x22] = new PrefixedDecoder(dec66: DataWidthDependent(
                    bit32: VexInstr(Mnemonic.pinsrd, Mnemonic.vpinsrd, Vdq,Hdq,Ey,Ib),
                    bit64: VexInstr(Mnemonic.pinsrq, Mnemonic.vpinsrq, Vdq,Hdq,Ey,Ib)));
                d[0x23] = s_invalid;

                d[0x24] = s_invalid;
                d[0x25] = s_invalid;
                d[0x26] = s_invalid;
                d[0x27] = s_invalid;

                d[0x28] = s_invalid;
                d[0x29] = s_invalid;
                d[0x2A] = s_invalid;
                d[0x2B] = s_invalid;
                d[0x2C] = s_invalid;
                d[0x2D] = s_invalid;
                d[0x2E] = s_invalid;
                d[0x2F] = s_invalid;

                // 30
                d[0x30] = s_invalid;
                d[0x31] = s_invalid;
                d[0x32] = s_invalid;
                d[0x33] = s_invalid;
                d[0x34] = s_invalid;
                d[0x35] = s_invalid;
                d[0x36] = s_invalid;
                d[0x37] = s_invalid;

                d[0x38] = new PrefixedDecoder(
                    dec66: EvexInstr(
                        s_invalid,
                        Instr(Mnemonic.vinserti128, Vqq,Hqq,Wqq,Ib),
                        VexLong(
                            s_invalid,
                            Instr(Mnemonic.vinserti32x4, Vx,Hx,Wdq,Ib),
                            Instr(Mnemonic.vinserti32x4, Vx,Hx,Wdq,Ib))),
                    dec66Wide: EvexInstr(
                        s_invalid,
                        s_invalid,
                        VexLong(
                            s_invalid,
                            Instr(Mnemonic.vinserti64x2, Vx, Hx, Wx, Ib),
                            Instr(Mnemonic.vinserti64x2, Vx, Hx, Wx, Ib))));
                d[0x39] = new PrefixedDecoder(dec66: VexInstr(Mnemonic.vextracti128, Wdq,Vqq,Ib));
                d[0x3A] = s_invalid;
                d[0x3B] = s_invalid;
                d[0x3C] = s_invalid;
                d[0x3D] = s_invalid;
                d[0x3E] = s_invalid;
                d[0x3F] = s_invalid;

                // 40
                d[0x40] = new PrefixedDecoder(dec66:VexInstr(Mnemonic.dpps, Mnemonic.vdpps, Vx,Hx,Wx,Ib));
                d[0x41] = new PrefixedDecoder(dec66:VexInstr(Mnemonic.dppd, Mnemonic.vdppd, Vdq,Hdq,Wdq,Ib));
                d[0x42] = new PrefixedDecoder(dec66:VexInstr(Mnemonic.mpsadbw, Mnemonic.vmpsadbw, Vx,Hx,Wx,Ib));
                d[0x43] = s_invalid;
                d[0x44] = new PrefixedDecoder(dec66:VexInstr(Mnemonic.pclmulqdq, Mnemonic.vpclmulqdq, Vdq,Hdq,Wdq,Ib));
                d[0x45] = s_invalid;
                d[0x46] = new PrefixedDecoder(dec66:VexInstr(Mnemonic.vperm2i128, Vqq,Hqq,Wqq,Ib));
                d[0x47] = s_invalid;

                d[0x48] = s_invalid;
                d[0x49] = s_invalid;
                d[0x4A] = new PrefixedDecoder(dec66:VexInstr(Mnemonic.vblendvps, Vx,Hx,Wx,Lx));
                d[0x4B] = new PrefixedDecoder(dec66:VexInstr(Mnemonic.vblendvpd, Vx,Hx,Wx,Lx));
                d[0x4C] = new PrefixedDecoder(dec66:VexInstr(Mnemonic.vblendvb, Vx,Hx,Wx,Lx));
                d[0x4D] = s_invalid;
                d[0x4E] = s_invalid;
                d[0x4F] = s_invalid;

                // 50
                d[0x50] = s_invalid;
                d[0x51] = s_invalid;
                d[0x52] = s_invalid;
                d[0x53] = s_invalid;
                d[0x54] = s_invalid;
                d[0x55] = s_invalid;
                d[0x56] = s_invalid;
                d[0x57] = s_invalid;

                d[0x58] = s_invalid;
                d[0x59] = s_invalid;
                d[0x5A] = s_invalid;
                d[0x5B] = s_invalid;
                d[0x5C] = s_invalid;
                d[0x5D] = s_invalid;
                d[0x5E] = s_invalid;
                d[0x5F] = s_invalid;

                // 60
                d[0x60] = new PrefixedDecoder(dec66: VexInstr(Mnemonic.pcmpestrm, Mnemonic.vpcmpestrm, Vdq,Wdq,Ib));
                d[0x61] = new PrefixedDecoder(dec66: VexInstr(Mnemonic.pcmpestri, Mnemonic.vpcmpestri, Vdq,Wdq,Ib));
                d[0x62] = new PrefixedDecoder(dec66: VexInstr(Mnemonic.pcmpistrm, Mnemonic.vpcmpistrm, Vdq,Wdq,Ib));
                d[0x63] = new PrefixedDecoder(dec66: VexInstr(Mnemonic.pcmpistri, Mnemonic.vpcmpistri, Vdq,Wdq,Ib));
                d[0x64] = s_invalid;
                d[0x65] = s_invalid;
                d[0x66] = s_invalid;
                d[0x67] = s_invalid;

                d[0x68] = s_invalid;
                d[0x69] = s_invalid;
                d[0x6A] = s_invalid;
                d[0x6B] = s_invalid;
                d[0x6C] = s_invalid;
                d[0x6D] = s_invalid;
                d[0x6E] = s_invalid;
                d[0x6F] = s_invalid;

                // 70
                d[0x70] = s_invalid;
                d[0x71] = s_invalid;
                d[0x72] = s_invalid;
                d[0x73] = s_invalid;
                d[0x74] = s_invalid;
                d[0x75] = s_invalid;
                d[0x76] = s_invalid;
                d[0x77] = s_invalid;

                d[0x78] = s_invalid;
                d[0x79] = s_invalid;
                d[0x7A] = s_invalid;
                d[0x7B] = s_invalid;
                d[0x7C] = s_invalid;
                d[0x7D] = s_invalid;
                d[0x7E] = s_invalid;
                d[0x7F] = s_invalid;

                // 80
                d[0x80] = s_invalid;
                d[0x81] = s_invalid;
                d[0x82] = s_invalid;
                d[0x83] = s_invalid;
                d[0x84] = s_invalid;
                d[0x85] = s_invalid;
                d[0x86] = s_invalid;
                d[0x87] = s_invalid;

                d[0x88] = s_invalid;
                d[0x89] = s_invalid;
                d[0x8A] = s_invalid;
                d[0x8B] = s_invalid;
                d[0x8C] = s_invalid;
                d[0x8D] = s_invalid;
                d[0x8E] = s_invalid;
                d[0x8F] = s_invalid;

                // 90
                d[0x90] = s_invalid;
                d[0x91] = s_invalid;
                d[0x92] = s_invalid;
                d[0x93] = s_invalid;
                d[0x94] = s_invalid;
                d[0x95] = s_invalid;
                d[0x96] = s_invalid;
                d[0x97] = s_invalid;

                d[0x98] = s_invalid;
                d[0x99] = s_invalid;
                d[0x9A] = s_invalid;
                d[0x9B] = s_invalid;
                d[0x9C] = s_invalid;
                d[0x9D] = s_invalid;
                d[0x9E] = s_invalid;
                d[0x9F] = s_invalid;

                // A0
                d[0xA0] = s_invalid;
                d[0xA1] = s_invalid;
                d[0xA2] = s_invalid;
                d[0xA3] = s_invalid;
                d[0xA4] = s_invalid;
                d[0xA5] = s_invalid;
                d[0xA6] = s_invalid;
                d[0xA7] = s_invalid;

                d[0xA8] = s_invalid;
                d[0xA9] = s_invalid;
                d[0xAA] = s_invalid;
                d[0xAB] = s_invalid;
                d[0xAC] = s_invalid;
                d[0xAD] = s_invalid;
                d[0xAE] = s_invalid;
                d[0xAF] = s_invalid;

                // B0
                d[0xB0] = s_invalid;
                d[0xB1] = s_invalid;
                d[0xB2] = s_invalid;
                d[0xB3] = s_invalid;
                d[0xB4] = s_invalid;
                d[0xB5] = s_invalid;
                d[0xB6] = s_invalid;
                d[0xB7] = s_invalid;

                d[0xB8] = s_invalid;
                d[0xB9] = s_invalid;
                d[0xBA] = s_invalid;
                d[0xBB] = s_invalid;
                d[0xBC] = s_invalid;
                d[0xBD] = s_invalid;
                d[0xBE] = s_invalid;
                d[0xBF] = s_invalid;

                // C0
                d[0xC0] = s_invalid;
                d[0xC1] = s_invalid;
                d[0xC2] = s_invalid;
                d[0xC3] = s_invalid;
                d[0xC4] = s_invalid;
                d[0xC5] = s_invalid;
                d[0xC6] = s_invalid;
                d[0xC7] = s_invalid;

                d[0xC8] = s_invalid;
                d[0xC9] = s_invalid;
                d[0xCA] = s_invalid;
                d[0xCB] = s_invalid;
                d[0xCC] = Instr(Mnemonic.sha1rnds4, Vdq,Wdq,Ib);
                d[0xCD] = s_invalid;
                d[0xCE] = s_invalid;
                d[0xCF] = s_invalid;

                // D0
                d[0xD0] = s_invalid;
                d[0xD1] = s_invalid;
                d[0xD2] = s_invalid;
                d[0xD3] = s_invalid;
                d[0xD4] = s_invalid;
                d[0xD5] = s_invalid;
                d[0xD6] = s_invalid;
                d[0xD7] = s_invalid;

                d[0xD8] = s_invalid;
                d[0xD9] = s_invalid;
                d[0xDA] = s_invalid;
                d[0xDB] = s_invalid;
                d[0xDC] = s_invalid;
                d[0xDD] = s_invalid;
                d[0xDE] = s_invalid;
                d[0xDF] = new PrefixedDecoder(dec66: VexInstr(Mnemonic.aeskeygen, Mnemonic.vaeskeygen, Vdq,Wdq,Ib));

                // E0
                d[0xE0] = s_invalid;
                d[0xE1] = s_invalid;
                d[0xE2] = s_invalid;
                d[0xE3] = s_invalid;
                d[0xE4] = s_invalid;
                d[0xE5] = s_invalid;
                d[0xE6] = s_invalid;
                d[0xE7] = s_invalid;

                d[0xE8] = s_invalid;
                d[0xE9] = s_invalid;
                d[0xEA] = s_invalid;
                d[0xEB] = s_invalid;
                d[0xEC] = s_invalid;
                d[0xED] = s_invalid;
                d[0xEE] = s_invalid;
                d[0xEF] = s_invalid;

                // F0
                d[0xF0] = new PrefixedDecoder(decF2: VexInstr(Mnemonic.rorx, Gy,Ey,Ib));
                d[0xF1] = s_invalid;
                d[0xF2] = s_invalid;
                d[0xF3] = s_invalid;
                d[0xF4] = s_invalid;
                d[0xF5] = s_invalid;
                d[0xF6] = s_invalid;
                d[0xF7] = s_invalid;

                d[0xF8] = s_invalid;
                d[0xF9] = s_invalid;
                d[0xFA] = s_invalid;
                d[0xFB] = s_invalid;
                d[0xFC] = s_invalid;
                d[0xFD] = s_invalid;
                d[0xFE] = s_invalid;
                d[0xFF] = s_invalid;
            }
        }
    }
}