#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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

using Reko.Core;
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Arch.X86
{
    public partial class X86Disassembler
    {
        public partial class InstructionSet
        {
            private void CreateTwobyteDecoders(Decoder [] d)
            {
                var reservedNop = Instr(Mnemonic.nop, InstrClass.Linear | InstrClass.Padding, Ev);

				// 0F 00
				d[0x00] = new GroupDecoder(Grp6);
                d[0x01] = new GroupDecoder(Grp7);
                d[0x02] = Instr(Mnemonic.lar, InstrClass.Privileged, Gv,Ew);
                d[0x03] = Instr(Mnemonic.lsl, InstrClass.Privileged, Gv,Ew);
                d[0x04] = s_invalid;
                d[0x05] = Amd64Instr(
                    s_invalid,
                    Instr(Mnemonic.syscall, InstrClass.Transfer|InstrClass.Call));
                d[0x06] = Instr(Mnemonic.clts);
                d[0x07] = Amd64Instr(
                    s_invalid,
                    Instr(Mnemonic.sysret, InstrClass.Transfer));

                d[0x08] = Instr(Mnemonic.invd, InstrClass.Privileged);
                d[0x09] = Instr(Mnemonic.wbinvd, InstrClass.Privileged);
                d[0x0A] = s_invalid;
                d[0x0B] = Instr(Mnemonic.ud2, InstrClass.Invalid);
                d[0x0C] = s_invalid;
                d[0x0D] = Instr(Mnemonic.prefetchw, Ev);
                d[0x0E] = Instr(Mnemonic.femms);    // AMD-specific
                d[0x0F] = s_invalid; // nyi("AMD 3D-Now instructions"); //$TODO: this requires adding separate processor model for AMD

                // 0F 10
                d[0x10] = new PrefixedDecoder(
                    VexInstr(Mnemonic.movups,Mnemonic.vmovups, Vps,Wps),
                    VexInstr(Mnemonic.movupd,Mnemonic.vmovupd, Vpd,Wpd),
                    VexInstr(Mnemonic.movss, Mnemonic.vmovss,  Vx,Wss),
                    VexInstr(Mnemonic.movsd, Mnemonic.vmovsd,  Vx,Wsd));
                d[0x11] = new PrefixedDecoder(
                    VexInstr(Mnemonic.movups, Mnemonic.vmovups, Wps,Vps),
                    VexInstr(Mnemonic.movupd, Mnemonic.vmovupd, Wpd,Vpd),
                    VexInstr(Mnemonic.movss,  Mnemonic.vmovss, Wss,Vss),
                    VexInstr(Mnemonic.movsd,  Mnemonic.vmovsd, Wsd,Vsd));
                d[0x12] = new PrefixedDecoder(
                    VexInstr(Mnemonic.movlps,   Mnemonic.vmovlps,   Vq,Hq,Mq),
                    VexInstr(Mnemonic.movlpd,   Mnemonic.vmovlpd,   Vq,Hq,Mq),
                    VexInstr(Mnemonic.movsldup, Mnemonic.vmovsldup, Vx,Wx),
                    VexInstr(Mnemonic.movddup,  Mnemonic.vmovddup,  Vx,Wx));
                d[0x13] = new PrefixedDecoder(
                    VexInstr(Mnemonic.movlps,   Mnemonic.vmovlps, Mq,Vq),
                    dec66:VexInstr(Mnemonic.movlpd, Mnemonic.vmovlpd, Mq,Vq));

                d[0x14] = new PrefixedDecoder(
                    VexInstr(Mnemonic.unpcklps, Mnemonic.vunpcklps, Vx,Hx,Wx),
                    VexInstr(Mnemonic.unpcklpd, Mnemonic.vunpcklpd, Vx,Hx,Wx));
                d[0x15] = new PrefixedDecoder(
                    VexInstr(Mnemonic.unpckhps, Mnemonic.vunpckhps, Vx,Hx,Wx),
                    VexInstr(Mnemonic.unpckhpd, Mnemonic.vunpckhpd, Vx,Hx,Wx));
                d[0x16] = new PrefixedDecoder(
                    VexInstr(Mnemonic.movlhps, Mnemonic.vmovlhps, Vx,Wx),
                    VexInstr(Mnemonic.movhpd, Mnemonic.vmovhpd, Vx,Wx),
                    VexInstr(Mnemonic.movshdup, Mnemonic.vmovshdup, Vx,Wx));
                d[0x17] = new PrefixedDecoder(
                    VexInstr(Mnemonic.movhps, Mnemonic.vmovhps, Mq,Vq),
                    VexInstr(Mnemonic.movhpd, Mnemonic.vmovhpd, Mq,Vq));

                d[0x18] = new GroupDecoder(Grp16);
                d[0x19] = Instr(Mnemonic.nop, InstrClass.Linear|InstrClass.Padding, Ev);
                d[0x1A] = Instr(Mnemonic.nop, InstrClass.Linear|InstrClass.Padding, Ev);
                d[0x1B] = Instr(Mnemonic.nop, InstrClass.Linear|InstrClass.Padding, Ev);

                d[0x1C] = Instr(Mnemonic.cldemote, Eb);
                d[0x1D] = Instr(Mnemonic.nop, InstrClass.Linear|InstrClass.Padding, Ev);
                d[0x1E] = new PrefixedDecoder(
                    dec: reservedNop,
                    dec66: reservedNop,
                    decF3: new ModRmOpcodeDecoder(
                        reservedNop,
                        (0xFA, Instr(Mnemonic.endbr64, InstrClass.Linear)),
                        (0xFB, Instr(Mnemonic.endbr32, InstrClass.Linear))),
                    decF2: reservedNop,
                    decWide: reservedNop,
                    dec66Wide: reservedNop);
                d[0x1F] = Instr(Mnemonic.nop, InstrClass.Linear|InstrClass.Padding, Ev);

				// 0F 20
                d[0x20] = Amd64Instr(
				    Instr(Mnemonic.mov, Rd,Cd),
				    Instr(Mnemonic.mov, Rq,Cd));
                d[0x21] = Amd64Instr(
                    Instr(Mnemonic.mov, Rd,Dd),
                    Instr(Mnemonic.mov, Rq,Dd));
                d[0x22] = Amd64Instr(
                    Instr(Mnemonic.mov, Cd,Rd),
                    Instr(Mnemonic.mov, Cd,Rq));
                d[0x23] = Amd64Instr(
                    Instr(Mnemonic.mov, Dd,Rd),
                    Instr(Mnemonic.mov, Dd,Rq));
                d[0x24] = s_invalid;
                d[0x25] = s_invalid;
                d[0x26] = s_invalid;
                d[0x27] = s_invalid;

                d[0x28] = new PrefixedDecoder(
                    dec:  VexInstr(Mnemonic.movaps, Mnemonic.vmovaps, Vps,Wps),
                    dec66:VexInstr(Mnemonic.movapd, Mnemonic.vmovapd, Vpd,Wpd));
                d[0x29] = new PrefixedDecoder(
                    dec:  VexInstr(Mnemonic.movaps, Mnemonic.vmovaps, Wps,Vps),
                    dec66:VexInstr(Mnemonic.movapd, Mnemonic.vmovapd, Wpd,Vpd));
                d[0x2A] = new PrefixedDecoder(
                    dec:  VexInstr(Mnemonic.cvtpi2ps, Mnemonic.vcvtpi2ps, Vps,Qpi),
                    dec66:VexInstr(Mnemonic.cvtpi2pd, Mnemonic.vcvtpi2pd, Vpd,Qpi),
                    decF3:VexInstr(Mnemonic.cvtsi2ss, Mnemonic.vcvtsi2ss, Vss,Hss,Ey),
                    decF2:VexInstr(Mnemonic.cvtsi2sd, Mnemonic.vcvtsi2sd, Vsd,Hsd,Ey));
                d[0x2B] = new PrefixedDecoder(
                    dec:  VexInstr(Mnemonic.movntps, Mnemonic.vmovntps, Mps,Vps),
                    dec66:VexInstr(Mnemonic.movntpd, Mnemonic.vmovntpd, Mpd,Vpd));

                d[0x2C] = new PrefixedDecoder(
                    dec:  VexInstr(Mnemonic.cvttps2pi, Mnemonic.vcvttps2pi, Ppi,Wps),
                    dec66:VexInstr(Mnemonic.cvttpd2pi, Mnemonic.vcvttpd2pi, Ppi,Wpd),
                    decF3:VexInstr(Mnemonic.cvttss2si, Mnemonic.vcvttss2si, Gd,Wss),
                    decF2:VexInstr(Mnemonic.cvttsd2si, Mnemonic.vcvttsd2si, Gd,Wsd));
                d[0x2D] = new PrefixedDecoder(
                    dec:  VexInstr(Mnemonic.cvtps2pi, Mnemonic.vcvtps2pi, Ppi,Wps),
                    dec66:VexInstr(Mnemonic.cvtpd2si, Mnemonic.vcvtpd2si, Qpi,Wpd),
                    decF3:VexInstr(Mnemonic.cvtss2si, Mnemonic.vcvtss2si, Gy,Wss),
                    decF2:VexInstr(Mnemonic.cvtsd2si, Mnemonic.vcvtsd2si, Gy,Wsd));
                d[0x2E] = new PrefixedDecoder(
                    dec:  VexInstr(Mnemonic.ucomiss, Mnemonic.vucomiss, Vss,Wss),
                    dec66:VexInstr(Mnemonic.ucomisd, Mnemonic.vucomisd, Vsd,Wsd));
                d[0x2F] = new PrefixedDecoder(
                    dec:  VexInstr(Mnemonic.comiss, Mnemonic.vcomiss, Vss,Wss),
                    dec66:VexInstr(Mnemonic.comisd, Mnemonic.vcomisd, Vsd,Wsd));

				// 0F 30
				d[0x30] = Instr(Mnemonic.wrmsr, InstrClass.Privileged);
                d[0x31] = Instr(Mnemonic.rdtsc);
                d[0x32] = Instr(Mnemonic.rdmsr, InstrClass.Privileged);
                d[0x33] = Instr(Mnemonic.rdpmc);
                d[0x34] = Instr(Mnemonic.sysenter);
                d[0x35] = Instr(Mnemonic.sysexit, InstrClass.Transfer);
                d[0x36] = s_invalid;
                d[0x37] = Instr(Mnemonic.getsec, InstrClass.Privileged);

                d[0x38] = new AdditionalByteDecoder(s_decoders0F38); // 0F 38
                d[0x39] = s_invalid;
                d[0x3A] = new AdditionalByteDecoder(s_decoders0F3A); // 0F 3A
                d[0x3B] = s_invalid;
                d[0x3C] = s_invalid;
                d[0x3D] = s_invalid;
                d[0x3E] = s_invalid;
                d[0x3F] = s_invalid;

				// 0F 40
				d[0x40] = Instr(Mnemonic.cmovo,  InstrClass.Linear|InstrClass.Conditional, Gv,Ev);
                d[0x41] = Instr(Mnemonic.cmovno, InstrClass.Linear|InstrClass.Conditional, Gv,Ev);
                d[0x42] = Instr(Mnemonic.cmovc,  InstrClass.Linear|InstrClass.Conditional, Gv,Ev);
                d[0x43] = Instr(Mnemonic.cmovnc, InstrClass.Linear|InstrClass.Conditional, Gv,Ev);
                d[0x44] = Instr(Mnemonic.cmovz,  InstrClass.Linear|InstrClass.Conditional, Gv,Ev);
                d[0x45] = Instr(Mnemonic.cmovnz, InstrClass.Linear|InstrClass.Conditional, Gv,Ev);
                d[0x46] = Instr(Mnemonic.cmovbe, InstrClass.Linear|InstrClass.Conditional, Gv,Ev);
                d[0x47] = Instr(Mnemonic.cmova,  InstrClass.Linear|InstrClass.Conditional, Gv,Ev);

                d[0x48] = Instr(Mnemonic.cmovs,  InstrClass.Linear|InstrClass.Conditional, Gv,Ev);
                d[0x49] = Instr(Mnemonic.cmovns, InstrClass.Linear|InstrClass.Conditional, Gv,Ev);
                d[0x4A] = Instr(Mnemonic.cmovpe, InstrClass.Linear|InstrClass.Conditional, Gv,Ev);
                d[0x4B] = Instr(Mnemonic.cmovpo, InstrClass.Linear|InstrClass.Conditional, Gv,Ev);
                d[0x4C] = Instr(Mnemonic.cmovl,  InstrClass.Linear|InstrClass.Conditional, Gv,Ev);
                d[0x4D] = Instr(Mnemonic.cmovge, InstrClass.Linear|InstrClass.Conditional, Gv,Ev);
                d[0x4E] = Instr(Mnemonic.cmovle, InstrClass.Linear|InstrClass.Conditional, Gv,Ev);
                d[0x4F] = Instr(Mnemonic.cmovg,  InstrClass.Linear|InstrClass.Conditional, Gv,Ev);

				// 0F 50
                d[0x50] = new PrefixedDecoder(
                    VexInstr(Mnemonic.movmskps, Mnemonic.vmovmskps, Gy,Ups),
                    VexInstr(Mnemonic.movmskpd, Mnemonic.vmovmskpd, Gy,Upd));
                d[0x51] = new PrefixedDecoder(
                    dec:  VexInstr(Mnemonic.sqrtps, Mnemonic.vsqrtps, Vps,Wps),
                    dec66:VexInstr(Mnemonic.sqrtpd, Mnemonic.vsqrtpd, Vpd,Wpd),
                    decF3:VexInstr(Mnemonic.sqrtss, Mnemonic.vsqrtss, Vss,Hss,Wss),
                    decF2:VexInstr(Mnemonic.sqrtsd, Mnemonic.vsqrtsd, Vsd,Hsd,Wsd));
                d[0x52] = new PrefixedDecoder(
                    dec:  VexInstr(Mnemonic.rsqrtps, Mnemonic.vrsqrtps, Vps,Wps),
                    dec66:s_invalid,
                    decF3:VexInstr(Mnemonic.rsqrtss, Mnemonic.vrsqrtss, Vss,Hss,Wss),
                    decF2:s_invalid);
                d[0x53] = new PrefixedDecoder(
                    dec:  VexInstr(Mnemonic.rcpps, Mnemonic.vrcpps, Vps,Wps),
                    dec66:s_invalid,
                    decF3:VexInstr(Mnemonic.rcpss, Mnemonic.vrcpss, Vss,Hss,Wss),
                    decF2:s_invalid);

                d[0x54] = new PrefixedDecoder(
                    dec: VexInstr(Mnemonic.andps, Mnemonic.vandps, Vps,Hps,Wps),
                    dec66:VexInstr(Mnemonic.andpd, Mnemonic.vandpd, Vpd,Hpd,Wpd));
                d[0x55] = new PrefixedDecoder(
                    dec:  VexInstr(Mnemonic.andnps, Mnemonic.vandnps, Vps,Hps,Wps),
                    dec66:VexInstr(Mnemonic.andnpd, Mnemonic.vandnpd, Vpd,Hpd,Wpd));
                d[0x56] = new PrefixedDecoder(
                    dec:  VexInstr(Mnemonic.orps, Mnemonic.vorps, Vps,Hps,Wps),
                    dec66:VexInstr(Mnemonic.orpd, Mnemonic.vorpd, Vpd,Hpd,Wpd));
                d[0x57] = new PrefixedDecoder(
                    dec:  VexInstr(Mnemonic.xorps, Mnemonic.vxorps, Vps,Hps,Wps),
                    dec66:VexInstr(Mnemonic.xorpd, Mnemonic.vxorpd, Vpd,Hpd,Wpd));

                d[0x58] = new PrefixedDecoder(
                    dec:  VexInstr(Mnemonic.addps, Mnemonic.vaddps, Vps,Hps,Wps),
                    dec66:VexInstr(Mnemonic.addpd, Mnemonic.vaddpd, Vpd,Hpd,Wpd),
                    decF3:VexInstr(Mnemonic.addss, Mnemonic.vaddss, Vss,Hss,Wss),
                    decF2:VexInstr(Mnemonic.addsd, Mnemonic.vaddsd, Vsd,Hsd,Wsd));
                d[0x59] = new PrefixedDecoder(
                    dec:  VexInstr(Mnemonic.mulps, Mnemonic.vmulps, Vps,Hps,Wps),
                    dec66:VexInstr(Mnemonic.mulpd, Mnemonic.vmulpd, Vpd,Hpd,Wpd),
                    decF3:VexInstr(Mnemonic.mulss, Mnemonic.vmulss, Vss,Hss,Wss),
                    decF2:VexInstr(Mnemonic.mulsd, Mnemonic.vmulsd, Vsd,Hsd,Wsd));
                d[0x5A] = new PrefixedDecoder(
                    dec:  VexInstr(Mnemonic.cvtps2pd, Mnemonic.vcvtps2pd, Vpd,Wps),
                    dec66:VexInstr(Mnemonic.cvtpd2ps, Mnemonic.vcvtpd2ps, Vps,Wpd),
                    decF3:VexInstr(Mnemonic.cvtss2sd, Mnemonic.vcvtss2sd, Vsd,Hx,Wss),
                    decF2:VexInstr(Mnemonic.cvtsd2ss, Mnemonic.vcvtsd2ss, Vss,Hx,Wsd));
                d[0x5B] = new PrefixedDecoder(
                    dec:  VexInstr(Mnemonic.cvtdq2ps, Mnemonic.vcvtdq2ps, Vps,Wdq),
                    dec66:VexInstr(Mnemonic.cvtps2dq, Mnemonic.vcvtps2dq, Vdq,Wps),
                    decF3:VexInstr(Mnemonic.cvttps2dq, Mnemonic.vcvttps2dq, Vdq,Wps),
                    decF2:s_invalid);
                d[0x5C] = new PrefixedDecoder(
                    dec:  VexInstr(Mnemonic.subps, Mnemonic.vsubps, Vps,Hps,Wps),
                    dec66:VexInstr(Mnemonic.subpd, Mnemonic.vsubpd, Vpd,Hpd,Wpd),
                    decF3:VexInstr(Mnemonic.subss, Mnemonic.vsubss, Vss,Hss,Wss),
                    decF2:VexInstr(Mnemonic.subsd, Mnemonic.vsubsd, Vsd,Hsd,Wsd));
                d[0x5D] = new PrefixedDecoder(
                    dec:  VexInstr(Mnemonic.minps, Mnemonic.vminps, Vps,Hps,Wps),
                    dec66:VexInstr(Mnemonic.minpd, Mnemonic.vminpd, Vpd,Hpd,Wpd),
                    decF3:VexInstr(Mnemonic.minss, Mnemonic.vminss, Vss,Hss,Wss),
                    decF2:VexInstr(Mnemonic.minsd, Mnemonic.vminsd, Vsd,Hsd,Wsd));
                d[0x5E] = new PrefixedDecoder(
                    dec:  VexInstr(Mnemonic.divps, Mnemonic.vdivps, Vps,Hps,Wps),
                    dec66:VexInstr(Mnemonic.divpd, Mnemonic.vdivpd, Vpd,Hpd,Wpd),
                    decF3:VexInstr(Mnemonic.divss, Mnemonic.vdivss, Vss,Hss,Wss),
                    decF2:VexInstr(Mnemonic.divsd, Mnemonic.vdivsd, Vsd,Hsd,Wsd));
                d[0x5F] = new PrefixedDecoder(
                    dec:  VexInstr(Mnemonic.maxps, Mnemonic.vmaxps, Vps,Hps,Wps),
                    dec66:VexInstr(Mnemonic.maxpd, Mnemonic.vmaxpd, Vpd,Hpd,Wpd),
                    decF3:VexInstr(Mnemonic.maxss, Mnemonic.vmaxss, Vss,Hss,Wss),
                    decF2:VexInstr(Mnemonic.maxsd, Mnemonic.vmaxsd, Vsd,Hsd,Wsd));
					
				// 0F 60
				d[0x60] = new PrefixedDecoder(
                    Instr(Mnemonic.punpcklbw, Pq,Qd),
                    dec66:VexInstr(Mnemonic.punpcklbw, Mnemonic.vpunpcklbw, Vx,Wx));
				d[0x61] = new PrefixedDecoder(
                    Instr(Mnemonic.punpcklwd, Pq,Qd),
                    VexInstr(Mnemonic.punpcklwd, Mnemonic.vpunpcklwd, Vx,Wx));
                d[0x62] = new PrefixedDecoder(
                    Instr(Mnemonic.punpckldq, Pq,Qd),
                    VexInstr(Mnemonic.punpckldq, Mnemonic.vpunpckldq, Vx,Hx,Wx));
                d[0x63] = new PrefixedDecoder(
                    Instr(Mnemonic.packsswb, Pq,Qd),
                    VexInstr(Mnemonic.packsswb, Mnemonic.vpacksswb, Vx,Hx,Wx));

                d[0x64] = new PrefixedDecoder(
                    Instr(Mnemonic.pcmpgtb, Pq,Qd),
                    VexInstr(Mnemonic.pcmpgtb, Mnemonic.vpcmpgtb, Vx,Hx,Wx));
                d[0x65] = new PrefixedDecoder(
                    Instr(Mnemonic.pcmpgtw, Pq,Qd),
                    VexInstr(Mnemonic.pcmpgtw, Mnemonic.vpcmpgtw, Vx,Hx,Wx));
                d[0x66] = new PrefixedDecoder(
                    Instr(Mnemonic.pcmpgtd, Pq,Qd),
                    VexInstr(Mnemonic.pcmpgtd, Mnemonic.vpcmpgtd, Vx,Hx,Wx));
                d[0x67] = new PrefixedDecoder(
                    Instr(Mnemonic.packuswb, Pq,Qd),
                    VexInstr(Mnemonic.punpckhbw, Mnemonic.vpunpckhbw, Vx,Hx,Wx));

                d[0x68] = new PrefixedDecoder(
                    Instr(Mnemonic.punpckhbw, Pq,Qd),
                    VexInstr(Mnemonic.punpckhbw, Mnemonic.vpunpckhbw, Vx,Hx,Wx));
                d[0x69] = new PrefixedDecoder(
                    VexInstr(Mnemonic.punpckhwd, Mnemonic.vpunpckhwd, Pq,Qd),
                    VexInstr(Mnemonic.punpckhwd, Mnemonic.vpunpckhwd, Vx,Hx,Wx));
                d[0x6A] = new PrefixedDecoder(
                    VexInstr(Mnemonic.punpckhdq, Mnemonic.vpunpckhdq, Pq,Qd),
                    VexInstr(Mnemonic.punpckhdq, Mnemonic.vpunpckhdq, Vx,Hx,Wx));
                d[0x6B] = new PrefixedDecoder(
                    VexInstr(Mnemonic.packssdw, Mnemonic.vpackssdw, Pq,Qd),
                    VexInstr(Mnemonic.packssdw, Mnemonic.vpackssdw, Vx,Hx,Wx));

                d[0x6C] = new PrefixedDecoder(
                    s_invalid,
                    VexInstr(Mnemonic.punpcklqdq, Mnemonic.vpunpcklqdq, Vx, Hx, Wx));
                d[0x6D] = new PrefixedDecoder(
                    s_invalid,
                    VexInstr(Mnemonic.punpckhqdq, Mnemonic.vpunpckhqdq, Vx,Hx,Wx));
                d[0x6E] = new PrefixedDecoder(
                    Instr(Mnemonic.movd, Py,Ey),
                    dec66:VexInstr(Mnemonic.movd,Mnemonic.vmovd, Vy,Ey));
				d[0x6F] = new PrefixedDecoder(
                    Instr(Mnemonic.movq, Pq,Qq),
                    dec66:VexInstr(Mnemonic.movdqa, Mnemonic.vmovdqa, Vx,Wx),
                    decF3:VexInstr(Mnemonic.movdqu, Mnemonic.vmovdqu, Vx,Wx));

				// 0F 70
				d[0x70] = new PrefixedDecoder(
                    Instr(Mnemonic.pshufw, Pq,Qq,Ib),
                    dec66:VexInstr(Mnemonic.pshufd, Mnemonic.vpshufd, Vx,Wx,Ib),
                    decF3:VexInstr(Mnemonic.pshufhw, Mnemonic.vpshufhw, Vx,Wx,Ib),
                    decF2:VexInstr(Mnemonic.pshuflw, Mnemonic.vpshuflw, Vx,Wx,Ib));
                d[0x71] = new GroupDecoder(Grp12);
                d[0x72] = new GroupDecoder(Grp13);
                d[0x73] = new GroupDecoder(Grp14);

				d[0x74] = new PrefixedDecoder(
                    Instr(Mnemonic.pcmpeqb, Pq,Qq),
                    dec66:VexInstr(Mnemonic.pcmpeqb, Mnemonic.vpcmpeqb, Vx,Wx));
                d[0x75] = new PrefixedDecoder(
                    Instr(Mnemonic.pcmpeqw, Pq,Qq),
                    dec66:VexInstr(Mnemonic.pcmpeqw, Mnemonic.vpcmpeqw, Vx,Wx));
                d[0x76] = new PrefixedDecoder(
                    Instr(Mnemonic.pcmpeqd, Pq,Qq),
                    dec66:VexInstr(Mnemonic.pcmpeqd, Mnemonic.vpcmpeqd, Vx,Wx));
                d[0x77] = VexInstr(
                    Instr(Mnemonic.emms, InstrClass.Privileged),
                    VexLong(
                        Instr(Mnemonic.vzeroupper),
                        Instr(Mnemonic.vzeroall)));

				d[0x78] = new PrefixedDecoder(
                    dec:Instr(Mnemonic.vmread, InstrClass.Privileged, Ey,Gy));
				d[0x79] = new PrefixedDecoder(
                    dec:Instr(Mnemonic.vmwrite, InstrClass.Privileged, Gy,Ey));
				d[0x7A] = s_invalid;
				d[0x7B] = s_invalid;

                d[0x7C] = new PrefixedDecoder(
                    dec:s_invalid,
                    dec66:VexInstr(Mnemonic.haddpd, Mnemonic.vhaddpd,  Vpd,Hpd,Wpd),
                    decF2:VexInstr(Mnemonic.haddps, Mnemonic.vhaddps, Vps,Hps,Wps));
                d[0x7D] = new PrefixedDecoder(
                    dec:s_invalid,
                    dec66:VexInstr(Mnemonic.hsubpd, Mnemonic.vhsubpd, Vpd,Hpd,Wpd),
                    decF2:VexInstr(Mnemonic.hsubps, Mnemonic.vhsubps, Vps,Hps,Wps));
                d[0x7E] = new PrefixedDecoder(
                    dec: Instr(Mnemonic.movd, Ey,Pd),
                    decWide: Instr(Mnemonic.movq, Ey,Pd),
                    dec66: VexInstr(Mnemonic.movd, Mnemonic.vmovd, Ey,Vy),
                    dec66Wide: VexInstr(Mnemonic.movq, Mnemonic.vmovq, Ey,Vy),
                    decF3: VexInstr(Mnemonic.movq, Mnemonic.vmovq, Vy,Wy));
				d[0x7F] = new PrefixedDecoder(
                    dec:Instr(Mnemonic.movq, Qq,Pq),
                    dec66:VexInstr(Mnemonic.movdqa, Mnemonic.vmovdqa, Wx,Vx),
                    decF3:VexInstr(Mnemonic.movdqu, Mnemonic.vmovdqu, Wx,Vx));

                // 0F 80
                d[0x80] = Instr(Mnemonic.jo, InstrClass.ConditionalTransfer, Jv);
				d[0x81] = Instr(Mnemonic.jno, InstrClass.ConditionalTransfer, Jv);
				d[0x82] = Instr(Mnemonic.jc,	InstrClass.ConditionalTransfer, Jv);
				d[0x83] = Instr(Mnemonic.jnc,	InstrClass.ConditionalTransfer, Jv);
				d[0x84] = Instr(Mnemonic.jz,	InstrClass.ConditionalTransfer, Jv);
				d[0x85] = Instr(Mnemonic.jnz, InstrClass.ConditionalTransfer, Jv);
				d[0x86] = Instr(Mnemonic.jbe, InstrClass.ConditionalTransfer, Jv);
				d[0x87] = Instr(Mnemonic.ja,  InstrClass.ConditionalTransfer, Jv);

				d[0x88] = Instr(Mnemonic.js,  InstrClass.ConditionalTransfer, Jv);
				d[0x89] = Instr(Mnemonic.jns, InstrClass.ConditionalTransfer, Jv);
				d[0x8A] = Instr(Mnemonic.jpe, InstrClass.ConditionalTransfer, Jv);
				d[0x8B] = Instr(Mnemonic.jpo, InstrClass.ConditionalTransfer, Jv);
				d[0x8C] = Instr(Mnemonic.jl,  InstrClass.ConditionalTransfer, Jv);
				d[0x8D] = Instr(Mnemonic.jge, InstrClass.ConditionalTransfer, Jv);
				d[0x8E] = Instr(Mnemonic.jle, InstrClass.ConditionalTransfer, Jv);
				d[0x8F] = Instr(Mnemonic.jg,  InstrClass.ConditionalTransfer, Jv);

				// 0F 90
				d[0x90] = Instr(Mnemonic.seto, Eb);
				d[0x91] = Instr(Mnemonic.setno,Eb);
				d[0x92] = Instr(Mnemonic.setc, Eb);
				d[0x93] = Instr(Mnemonic.setnc,Eb);
				d[0x94] = Instr(Mnemonic.setz, Eb);
				d[0x95] = Instr(Mnemonic.setnz,Eb);
				d[0x96] = Instr(Mnemonic.setbe,Eb);
				d[0x97] = Instr(Mnemonic.seta, Eb);

				d[0x98] = Instr(Mnemonic.sets,  Eb);
				d[0x99] = Instr(Mnemonic.setns, Eb);
				d[0x9A] = Instr(Mnemonic.setpe, Eb);
				d[0x9B] = Instr(Mnemonic.setpo, Eb);
				d[0x9C] = Instr(Mnemonic.setl,  Eb);
				d[0x9D] = Instr(Mnemonic.setge, Eb);
				d[0x9E] = Instr(Mnemonic.setle, Eb);
				d[0x9F] = Instr(Mnemonic.setg,  Eb);

				// 0F A0
				d[0xA0] = Instr(Mnemonic.push, s4);
				d[0xA1] = Instr(Mnemonic.pop, s4);
				d[0xA2] = Instr(Mnemonic.cpuid);
				d[0xA3] = Instr(Mnemonic.bt, Ev,Gv);
				d[0xA4] = Instr(Mnemonic.shld, Ev,Gv,Ib);
				d[0xA5] = Instr(Mnemonic.shld, Ev,Gv,c);
				d[0xA6] = s_invalid;
                d[0xA7] = s_invalid;

				d[0xA8] = Instr(Mnemonic.push, s5);
				d[0xA9] = Instr(Mnemonic.pop, s5);
				d[0xAA] = Instr(Mnemonic.rsm, InstrClass.Privileged);
                d[0xAB] = Instr(Mnemonic.bts, Ev,Gv);
				d[0xAC] = Instr(Mnemonic.shrd, Ev,Gv,Ib);
				d[0xAD] = Instr(Mnemonic.shrd, Ev,Gv,c);
				d[0xAE] = new GroupDecoder(Grp15);
				d[0xAF] = Instr(Mnemonic.imul, Gv,Ev);

				// 0F B0
				d[0xB0] = Instr(Mnemonic.cmpxchg, Eb,Gb);
				d[0xB1] = Instr(Mnemonic.cmpxchg, Ev,Gv);
				d[0xB2] = Instr(Mnemonic.lss, Gv,Mp);
				d[0xB3] = Instr(Mnemonic.btr, Ev,Gv);
                d[0xB4] = Instr(Mnemonic.lfs, Gv,Mp);
				d[0xB5] = Instr(Mnemonic.lgs, Gv,Mp);
				d[0xB6] = Instr(Mnemonic.movzx, Gv,Eb);
				d[0xB7] = Instr(Mnemonic.movzx, Gv,Ew);

				d[0xB8] = new PrefixedDecoder(
                    dec:Instr(Mnemonic.jmpe),
                    decF3:Instr(Mnemonic.popcnt, Gv,Ev));
				d[0xB9] = new GroupDecoder(Grp10, Gv,Ev); 
				d[0xBA] = new GroupDecoder(Grp8, Ev,Ib);
				d[0xBB] = Instr(Mnemonic.btc, Gv,Ev);

                d[0xBC] = new PrefixedDecoder(
                    dec: Instr(Mnemonic.bsf, Gv,Ev),
                    dec66: Instr(Mnemonic.bsf, Gw,Ew),
                    decF3: Instr(Mnemonic.tzcnt, Gv,Ev));
                d[0xBD] = new PrefixedDecoder(
                    dec: Instr(Mnemonic.bsr, Gv,Ev),
                    dec66: Instr(Mnemonic.bsr, Gw,Ew),
                    decF3: Instr(Mnemonic.lzcnt, Gv,Ev));
				d[0xBE] = Instr(Mnemonic.movsx, Gv,Eb);
				d[0xBF] = Instr(Mnemonic.movsx, Gv,Ew);

				// 0F C0
				d[0xC0] = Instr(Mnemonic.xadd, Eb,Gb);
				d[0xC1] = Instr(Mnemonic.xadd, Ev,Gv);
				d[0xC2] = new PrefixedDecoder(
                    dec:  VexInstr(Mnemonic.cmpps, Mnemonic.vcmpps, Vps,Hps,Wps,Ib),
                    dec66:VexInstr(Mnemonic.cmppd, Mnemonic.vcmppd, Vpd,Hpd,Wpd,Ib),
                    decF3:VexInstr(Mnemonic.cmpss, Mnemonic.vcmpss, Vss,Hss,Wss,Ib),
                    decF2:VexInstr(Mnemonic.cmpsd, Mnemonic.vcmpsd, Vpd,Hpd,Wpd,Ib));
				d[0xC3] = new PrefixedDecoder(
                    Instr(Mnemonic.movnti, My,Gy),
                    s_invalid);
                d[0xC4] = new PrefixedDecoder(
                    VexInstr(Mnemonic.pinsrw, Mnemonic.vpinsrw, Pq,Ry),     //$TODO: encoding is weird.
                    VexInstr(Mnemonic.pinsrw, Mnemonic.vpinsrw, Vdq,Hdq,Ry));
                d[0xC5] = new PrefixedDecoder(
                    Instr(Mnemonic.pextrw, Gd,Nq,Ib),
                    VexInstr(Mnemonic.pextrw, Mnemonic.vpextrw, Gd,Udq,Ib));
                d[0xC6] = new PrefixedDecoder(
                    VexInstr(Mnemonic.shufps, Mnemonic.vshufps, Vps,Hps,Wps,Ib),
                    VexInstr(Mnemonic.shufpd, Mnemonic.vshufpd, Vpd,Hpd,Wpd,Ib));
				d[0xC7] = new GroupDecoder(Grp9);

				d[0xC8] = Instr(Mnemonic.bswap, rv);
				d[0xC9] = Instr(Mnemonic.bswap, rv);
				d[0xCA] = Instr(Mnemonic.bswap, rv);
				d[0xCB] = Instr(Mnemonic.bswap, rv);
				d[0xCC] = Instr(Mnemonic.bswap, rv);
				d[0xCD] = Instr(Mnemonic.bswap, rv);
				d[0xCE] = Instr(Mnemonic.bswap, rv);
				d[0xCF] = Instr(Mnemonic.bswap, rv);

				// 0F D0
				d[0xD0] = new PrefixedDecoder(
                    dec:  s_invalid,
					dec66:VexInstr(Mnemonic.addsubpd, Mnemonic.vaddsubpd, Vpd,Hpd,Wpd),
					decF3:s_invalid,
					decF2:VexInstr(Mnemonic.addsubps, Mnemonic.vaddsubps, Vps,Hps,Wps));
				d[0xD1] = new PrefixedDecoder(
                    Instr(Mnemonic.psrlw, Pq,Qq),
                    VexInstr(Mnemonic.psrlw, Mnemonic.vpsrlw, Vx,Hx,Wx));
                d[0xD2] = new PrefixedDecoder(
                    Instr(Mnemonic.psrld, Pq,Qq),
                    VexInstr(Mnemonic.psrld, Mnemonic.vpsrld, Vx,Hx,Wx));
                d[0xD3] = new PrefixedDecoder(
                    Instr(Mnemonic.psrlq, Pq,Qq),
                    VexInstr(Mnemonic.psrlq, Mnemonic.vpsrlq, Vx,Hx,Wx));

                d[0xD4] = new PrefixedDecoder(
                    Instr(Mnemonic.paddq, Pq,Qq),
                    VexInstr(Mnemonic.paddq, Mnemonic.vpaddq, Vx,Hx,Wx));
                d[0xD5] = new PrefixedDecoder(
                    Instr(Mnemonic.pmullw, Pq,Qq),
                    VexInstr(Mnemonic.pmullw, Mnemonic.vpmullw, Vx,Hx,Wx));
				d[0xD6] = Instr(Mnemonic.movq, Wx,Vx);
                d[0xD7] = new PrefixedDecoder(
                    Instr(Mnemonic.pmovmskb, Gd,Nq),
                    VexInstr(Mnemonic.pmovmskb, Mnemonic.vpmovmskb, Gd,Ux));

                d[0xD8] = new PrefixedDecoder(
                    Instr(Mnemonic.psubusb, Pq,Qq),
                    VexInstr(Mnemonic.psubusb, Mnemonic.vpsubusb, Vx,Hx,Wx));
                d[0xD9] = new PrefixedDecoder(
                    Instr(Mnemonic.psubusw, Pq,Qq),
                    VexInstr(Mnemonic.psubusw, Mnemonic.vpsubusw, Vx,Hx,Wx));
                d[0xDA] = new PrefixedDecoder(
                    Instr(Mnemonic.pminub, Pq,Qq),
                    VexInstr(Mnemonic.pminub, Mnemonic.vpminub, Vx,Hx,Wx));
                d[0xDB] = new PrefixedDecoder(
                    Instr(Mnemonic.pand, Pq,Qq),
                    VexInstr(Mnemonic.pand, Mnemonic.vpand, Vx,Hx,Wx));
                d[0xDC] = new PrefixedDecoder(
                    Instr(Mnemonic.paddusb, Pq,Qq),
                    VexInstr(Mnemonic.paddusb, Mnemonic.vpaddusb, Vx,Hx,Wx));
                d[0xDD] = new PrefixedDecoder(
                    Instr(Mnemonic.paddusw, Pq,Qq),
                    VexInstr(Mnemonic.paddusw, Mnemonic.vpaddusw, Vx,Hx,Wx));
                d[0xDE] = new PrefixedDecoder(
                    Instr(Mnemonic.pmaxub, Pq,Qq),
                    VexInstr(Mnemonic.pmaxub, Mnemonic.vpmaxub, Vx,Hx,Wx));
                d[0xDF] = new PrefixedDecoder(
                    Instr(Mnemonic.pandn, Pq,Qq),
                    VexInstr(Mnemonic.pandn, Mnemonic.vpandn, Vx,Hx,Wx));

				// 0F E0
                d[0xE0] = new PrefixedDecoder(
                    Instr(Mnemonic.pavgb, Pq,Qq),
                    VexInstr(Mnemonic.pavgb, Mnemonic.vpavgb, Vx,Hx,Wx));
                d[0xE1] = new PrefixedDecoder(
                    Instr(Mnemonic.psraw, Pq,Qq),
                    VexInstr(Mnemonic.psraw, Mnemonic.vpsraw, Vx,Hx,Wx));
                d[0xE2] = new PrefixedDecoder(
                    Instr(Mnemonic.psrad, Pq,Qq),
                    VexInstr(Mnemonic.psrad, Mnemonic.vpsrad, Vx,Hx,Wx));
                d[0xE3] = new PrefixedDecoder(
                    Instr(Mnemonic.pavgw, Pq,Qq),
                    VexInstr(Mnemonic.pavgw, Mnemonic.vpavgw, Vx,Hx,Wx));

                d[0xE4] = new PrefixedDecoder(
                    Instr(Mnemonic.pmulhuw, Pq,Qq),
                    VexInstr(Mnemonic.pmulhuw, Mnemonic.vpmulhuw, Vx,Hx,Wx));
                d[0xE5] = new PrefixedDecoder(
                    Instr(Mnemonic.pmulhw, Pq,Qq),
                    VexInstr(Mnemonic.pmulhw, Mnemonic.vpmulhw, Vx,Hx,Wx));
                d[0xE6] = new PrefixedDecoder(
                    dec:  s_invalid,
                    dec66:VexInstr(Mnemonic.cvttpd2dq, Mnemonic.vcvttpd2dq, Vdq,Wpd),
                    decF3:VexInstr(Mnemonic.cvtdq2pd, Mnemonic.vcvtdq2pd, Vx,Wpd),
                    decF2:VexInstr(Mnemonic.cvtpd2dq, Mnemonic.vcvtpd2dq, Vdq,Wpd));
                d[0xE7] = new PrefixedDecoder(
                    Instr(Mnemonic.movntq, Mq,Pq),
                    VexInstr(Mnemonic.movntq, Mnemonic.vmovntq, Mx,Vx));

                d[0xE8] = new PrefixedDecoder(
                    Instr(Mnemonic.psubsb, Pq,Qq),
                    VexInstr(Mnemonic.psubsb, Mnemonic.vpsubsb, Vx,Hx,Wx));
                d[0xE9] = new PrefixedDecoder(
                    Instr(Mnemonic.psubsw, Pq,Qq),
                    VexInstr(Mnemonic.psubsw, Mnemonic.vpsubsw, Vx,Hx,Wx));
                d[0xEA] = new PrefixedDecoder(
                    Instr(Mnemonic.pminsw, Pq,Qq),
                    VexInstr(Mnemonic.pminsw, Mnemonic.vpminsw, Vx,Hx,Wx));
                d[0xEB] = new PrefixedDecoder(
                    Instr(Mnemonic.por, Pq,Qq),
                    VexInstr(Mnemonic.por, Mnemonic.vpor, Vx,Hx,Wx));

                d[0xEC] = new PrefixedDecoder(
                    Instr(Mnemonic.paddsb, Pq,Qq),
                    VexInstr(Mnemonic.paddsb, Mnemonic.vpaddsb, Vx,Hx,Wx));
                d[0xED] = new PrefixedDecoder(
                    Instr(Mnemonic.paddsw, Pq,Qq),
                    VexInstr(Mnemonic.paddsw, Mnemonic.vpaddsw, Vx,Hx,Wx));
                d[0xEE] = new PrefixedDecoder(
                    Instr(Mnemonic.pmaxsw, Pq,Qq),
                    VexInstr(Mnemonic.pmaxsw, Mnemonic.vpmaxsw, Vx,Hx,Wx));
                d[0xEF] = new PrefixedDecoder(
                    Instr(Mnemonic.pxor, Pq,Qq),
                    VexInstr(Mnemonic.pxor, Mnemonic.vpxor, Vx,Hx,Wx));

				// 0F F0
                d[0xF0] = new PrefixedDecoder(
                    s_invalid,
                    decF2:VexInstr(Mnemonic.lddqu, Mnemonic.vlddqu, Vx,Mx));
                d[0xF1] = new PrefixedDecoder(
                    Instr(Mnemonic.psllw, Pq,Qq),
                    dec66:VexInstr(Mnemonic.psllw, Mnemonic.vpsllw, Vx,Hx,Wx));
                d[0xF2] = new PrefixedDecoder(
                    Instr(Mnemonic.pslld, Pq,Qq),
                    dec66:VexInstr(Mnemonic.pslld, Mnemonic.vpslld, Vx,Hx,Wx));
                d[0xF3] = new PrefixedDecoder(
                    Instr(Mnemonic.psllq, Pq,Qq),
                    dec66:VexInstr(Mnemonic.psllq, Mnemonic.vpsllq, Vx,Hx,Wx));
                d[0xF4] = new PrefixedDecoder(
                    Instr(Mnemonic.pmuludq, Pq,Qq),
                    dec66:VexInstr(Mnemonic.pmuludq, Mnemonic.vpmuludq, Vx,Hx,Wx));
                d[0xF5] = new PrefixedDecoder(
                    Instr(Mnemonic.pmaddwd, Pq,Qq),
                    dec66:VexInstr(Mnemonic.pmaddwd, Mnemonic.vpmaddwd, Vx,Hx,Wx));
                d[0xF6] = new PrefixedDecoder(
                    Instr(Mnemonic.psadbw, Pq,Qq),
                    dec66:VexInstr(Mnemonic.psadbw, Mnemonic.vpsadbw, Vx,Hx,Wx));
                d[0xF7] = new PrefixedDecoder(
                    Instr(Mnemonic.maskmovq, Pq,Qq),
                    dec66:VexInstr(Mnemonic.maskmovdqu, Mnemonic.vmaskmovdqu, Vdq,Udq));

                d[0xF8] = new PrefixedDecoder(
                    Instr(Mnemonic.psubb, Pq,Qq),
                    VexInstr(Mnemonic.psubb, Mnemonic.vpsubb, Vx,Hx,Wx));
                d[0xF9] = new PrefixedDecoder(
                    Instr(Mnemonic.psubw, Pq,Qq),
                    VexInstr(Mnemonic.psubw, Mnemonic.vpsubw, Vx,Hx,Wx));
                d[0xFA] = new PrefixedDecoder(
                    Instr(Mnemonic.psubd, Pq,Qq),
                    VexInstr(Mnemonic.psubd, Mnemonic.vpsubd, Vx,Hx,Wx));
                d[0xFB] = new PrefixedDecoder(
                    Instr(Mnemonic.psubq, Pq,Qq),
                    VexInstr(Mnemonic.psubq, Mnemonic.vpsubq, Vx,Hx,Wx));
                d[0xFC] = new PrefixedDecoder(
                    Instr(Mnemonic.paddb, Pq,Qq),
                    VexInstr(Mnemonic.paddb, Mnemonic.vpaddb, Vx,Hx,Wx));
                d[0xFD] = new PrefixedDecoder(
                    Instr(Mnemonic.paddw, Pq,Qq),
                    VexInstr(Mnemonic.paddw, Mnemonic.vpaddw, Vx,Hx,Wx));
                d[0xFE] = new PrefixedDecoder(
                    Instr(Mnemonic.paddd, Pq,Qq),
                    VexInstr(Mnemonic.paddd, Mnemonic.vpaddd, Vx,Hx,Wx));
                d[0xFF] = Instr(Mnemonic.ud0, InstrClass.Invalid, Gv, Ev);
			}
        }
    }
}
