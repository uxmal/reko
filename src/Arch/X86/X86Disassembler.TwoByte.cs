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

using Reko.Core;
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.X86
{
    public partial class X86Disassembler
    {
        private static Decoder[] CreateTwobyteDecoders()
        {
            var reservedNop = Instr(Mnemonic.nop, InstrClass.Linear | InstrClass.Padding, Ev);

            return new Decoder[]
            {
				// 0F 00
				new GroupDecoder(Grp6),
                new GroupDecoder(Grp7),
                Instr(Mnemonic.lar, InstrClass.System, Gv,Ew),
                Instr(Mnemonic.lsl, InstrClass.System, Gv,Ew),
                s_invalid,
                Amd64Instr(
                    s_invalid,
                    Instr(Mnemonic.syscall, InstrClass.Transfer|InstrClass.Call)),
                Instr(Mnemonic.clts),
                Amd64Instr(
                    s_invalid,
                    Instr(Mnemonic.sysret, InstrClass.Transfer)),

                Instr(Mnemonic.invd, InstrClass.System),
                Instr(Mnemonic.wbinvd, InstrClass.System),
                s_invalid,
                Instr(Mnemonic.ud2, InstrClass.Invalid),
                s_invalid,
                Instr(Mnemonic.prefetchw, Ev),
                Instr(Mnemonic.femms),    // AMD-specific
                s_invalid, // nyi("AMD 3D-Now instructions"), //$TODO: this requires adding separate processor model for AMD

				// 0F 10
				new PrefixedDecoder(
                    VexInstr(Mnemonic.movups,Mnemonic.vmovups, Vps,Wps),
                    VexInstr(Mnemonic.movupd,Mnemonic.vmovupd, Vpd,Wpd),
                    VexInstr(Mnemonic.movss, Mnemonic.vmovss,  Vx,Wss),
                    VexInstr(Mnemonic.movsd, Mnemonic.vmovsd,  Vx,Wsd)),
                new PrefixedDecoder(
                    VexInstr(Mnemonic.movups, Mnemonic.vmovups, Wps,Vps),
                    VexInstr(Mnemonic.movupd, Mnemonic.vmovupd, Wpd,Vpd),
                    VexInstr(Mnemonic.movss,  Mnemonic.vmovss, Wss,Vss),
                    VexInstr(Mnemonic.movsd,  Mnemonic.vmovsd, Wsd,Vsd)),
                new PrefixedDecoder(
                    VexInstr(Mnemonic.movlps,   Mnemonic.vmovlps,   Vq,Hq,Mq),
                    VexInstr(Mnemonic.movlpd,   Mnemonic.vmovlpd,   Vq,Hq,Mq),
                    VexInstr(Mnemonic.movsldup, Mnemonic.vmovsldup, Vx,Wx),
                    VexInstr(Mnemonic.movddup,  Mnemonic.vmovddup,  Vx,Wx)),
                new PrefixedDecoder(
                    VexInstr(Mnemonic.movlps,   Mnemonic.vmovlps, Mq,Vq),
                    dec66:VexInstr(Mnemonic.movlpd, Mnemonic.vmovlpd, Mq,Vq)),

                new PrefixedDecoder(
                    VexInstr(Mnemonic.unpcklps, Mnemonic.vunpcklps, Vx,Hx,Wx),
                    VexInstr(Mnemonic.unpcklpd, Mnemonic.vunpcklpd, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    VexInstr(Mnemonic.unpckhps, Mnemonic.vunpckhps, Vx,Hx,Wx),
                    VexInstr(Mnemonic.unpckhpd, Mnemonic.vunpckhpd, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    VexInstr(Mnemonic.movlhps, Mnemonic.vmovlhps, Vx,Wx),
                    VexInstr(Mnemonic.movhpd, Mnemonic.vmovhpd, Vx,Wx),
                    VexInstr(Mnemonic.movshdup, Mnemonic.vmovshdup, Vx,Wx)),
                new PrefixedDecoder(
                    VexInstr(Mnemonic.movhps, Mnemonic.vmovhps, Mq,Vq),
                    VexInstr(Mnemonic.movhpd, Mnemonic.vmovhpd, Mq,Vq)),

                new GroupDecoder(Grp16),
                Instr(Mnemonic.nop, InstrClass.Linear|InstrClass.Padding, Ev),
                Instr(Mnemonic.nop, InstrClass.Linear|InstrClass.Padding, Ev),
                Instr(Mnemonic.nop, InstrClass.Linear|InstrClass.Padding, Ev),

                Instr(Mnemonic.cldemote, Eb),
                Instr(Mnemonic.nop, InstrClass.Linear|InstrClass.Padding, Ev),
                new PrefixedDecoder(
                    dec: reservedNop,
                    dec66: reservedNop,
                    decF3: new ModRmOpcodeDecoder(
                        reservedNop,
                        (0xFA, Instr(Mnemonic.endbr64, InstrClass.Linear)),
                        (0xFB, Instr(Mnemonic.endbr32, InstrClass.Linear))),
                    decF2: reservedNop,
                    decWide: reservedNop,
                    dec66Wide: reservedNop),
                Instr(Mnemonic.nop, InstrClass.Linear|InstrClass.Padding, Ev),

				// 0F 20
                Amd64Instr(
				    Instr(Mnemonic.mov, Rd,Cd),
				    Instr(Mnemonic.mov, Rq,Cd)),
                Amd64Instr(
                    Instr(Mnemonic.mov, Rd,Dd),
                    Instr(Mnemonic.mov, Rq,Dd)),
                Amd64Instr(
                    Instr(Mnemonic.mov, Cd,Rd),
                    Instr(Mnemonic.mov, Cd,Rq)),
                Amd64Instr(
                    Instr(Mnemonic.mov, Dd,Rd),
                    Instr(Mnemonic.mov, Dd,Rq)),
				s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,

                new PrefixedDecoder(
                    dec:  VexInstr(Mnemonic.movaps, Mnemonic.vmovaps, Vps,Wps),
                    dec66:VexInstr(Mnemonic.movapd, Mnemonic.vmovapd, Vpd,Wpd)),
                new PrefixedDecoder(
                    dec:  VexInstr(Mnemonic.movaps, Mnemonic.vmovaps, Wps,Vps),
                    dec66:VexInstr(Mnemonic.movapd, Mnemonic.vmovapd, Wpd,Vpd)),
                new PrefixedDecoder(
                    dec:  VexInstr(Mnemonic.cvtpi2ps, Mnemonic.vcvtpi2ps, Vps,Qpi),
                    dec66:VexInstr(Mnemonic.cvtpi2pd, Mnemonic.vcvtpi2pd, Vpd,Qpi),
                    decF3:VexInstr(Mnemonic.cvtsi2ss, Mnemonic.vcvtsi2ss, Vss,Hss,Ey),
                    decF2:VexInstr(Mnemonic.cvtsi2sd, Mnemonic.vcvtsi2sd, Vsd,Hsd,Ey)),
                new PrefixedDecoder(
                    dec:  VexInstr(Mnemonic.movntps, Mnemonic.vmovntps, Mps,Vps),
                    dec66:VexInstr(Mnemonic.movntpd, Mnemonic.vmovntpd, Mpd,Vpd)),

                new PrefixedDecoder(
                    dec:  VexInstr(Mnemonic.cvttps2pi, Mnemonic.vcvttps2pi, Ppi,Wps),
                    dec66:VexInstr(Mnemonic.cvttpd2pi, Mnemonic.vcvttpd2pi, Ppi,Wpd),
                    decF3:VexInstr(Mnemonic.cvttss2si, Mnemonic.vcvttss2si, Gd,Wss),
                    decF2:VexInstr(Mnemonic.cvttsd2si, Mnemonic.vcvttsd2si, Gd,Wsd)),
                new PrefixedDecoder(
                    dec:  VexInstr(Mnemonic.cvtps2pi, Mnemonic.vcvtps2pi, Ppi,Wps),
                    dec66:VexInstr(Mnemonic.cvtpd2si, Mnemonic.vcvtpd2si, Qpi,Wpd),
                    decF3:VexInstr(Mnemonic.cvtss2si, Mnemonic.vcvtss2si, Gy,Wss),
                    decF2:VexInstr(Mnemonic.cvtsd2si, Mnemonic.vcvtsd2si, Gy,Wsd)),
                new PrefixedDecoder(
                    dec:  VexInstr(Mnemonic.ucomiss, Mnemonic.vucomiss, Vss,Wss),
                    dec66:VexInstr(Mnemonic.ucomisd, Mnemonic.vucomisd, Vsd,Wsd)),
                new PrefixedDecoder(
                    dec:  VexInstr(Mnemonic.comiss, Mnemonic.vcomiss, Vss,Wss),
                    dec66:VexInstr(Mnemonic.comisd, Mnemonic.vcomisd, Vsd,Wsd)),

				// 0F 30
				Instr(Mnemonic.wrmsr, InstrClass.System),
                Instr(Mnemonic.rdtsc),
                Instr(Mnemonic.rdmsr, InstrClass.System),
                Instr(Mnemonic.rdpmc),
                Instr(Mnemonic.sysenter),
                Instr(Mnemonic.sysexit, InstrClass.Transfer),
                s_invalid,
                Instr(Mnemonic.getsec, InstrClass.System),

                new ThreeByteDecoder(), // 0F 38
                s_invalid,
                new ThreeByteDecoder(), // 0F 3A
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,

				// 0F 40
				Instr(Mnemonic.cmovo,  InstrClass.Linear|InstrClass.Conditional, Gv,Ev),
                Instr(Mnemonic.cmovno, InstrClass.Linear|InstrClass.Conditional, Gv,Ev),
                Instr(Mnemonic.cmovc,  InstrClass.Linear|InstrClass.Conditional, Gv,Ev),
                Instr(Mnemonic.cmovnc, InstrClass.Linear|InstrClass.Conditional, Gv,Ev),
                Instr(Mnemonic.cmovz,  InstrClass.Linear|InstrClass.Conditional, Gv,Ev),
                Instr(Mnemonic.cmovnz, InstrClass.Linear|InstrClass.Conditional, Gv,Ev),
                Instr(Mnemonic.cmovbe, InstrClass.Linear|InstrClass.Conditional, Gv,Ev),
                Instr(Mnemonic.cmova,  InstrClass.Linear|InstrClass.Conditional, Gv,Ev),

                Instr(Mnemonic.cmovs,  InstrClass.Linear|InstrClass.Conditional, Gv,Ev),
                Instr(Mnemonic.cmovns, InstrClass.Linear|InstrClass.Conditional, Gv,Ev),
                Instr(Mnemonic.cmovpe, InstrClass.Linear|InstrClass.Conditional, Gv,Ev),
                Instr(Mnemonic.cmovpo, InstrClass.Linear|InstrClass.Conditional, Gv,Ev),
                Instr(Mnemonic.cmovl,  InstrClass.Linear|InstrClass.Conditional, Gv,Ev),
                Instr(Mnemonic.cmovge, InstrClass.Linear|InstrClass.Conditional, Gv,Ev),
                Instr(Mnemonic.cmovle, InstrClass.Linear|InstrClass.Conditional, Gv,Ev),
                Instr(Mnemonic.cmovg,  InstrClass.Linear|InstrClass.Conditional, Gv,Ev),

				// 0F 50
                new PrefixedDecoder(
                    VexInstr(Mnemonic.movmskps, Mnemonic.vmovmskps, Gy,Ups),
                    VexInstr(Mnemonic.movmskpd, Mnemonic.vmovmskpd, Gy,Upd)),
                new PrefixedDecoder(
                    dec:  VexInstr(Mnemonic.sqrtps, Mnemonic.vsqrtps, Vps,Wps),
                    dec66:VexInstr(Mnemonic.sqrtpd, Mnemonic.vsqrtpd, Vpd,Wpd),
                    decF3:VexInstr(Mnemonic.sqrtss, Mnemonic.vsqrtss, Vss,Hss,Wss),
                    decF2:VexInstr(Mnemonic.sqrtsd, Mnemonic.vsqrtsd, Vsd,Hsd,Wsd)),
                new PrefixedDecoder(
                    dec:  VexInstr(Mnemonic.rsqrtps, Mnemonic.vrsqrtps, Vps,Wps),
                    dec66:s_invalid,
                    decF3:VexInstr(Mnemonic.rsqrtss, Mnemonic.vrsqrtss, Vss,Hss,Wss),
                    decF2:s_invalid),
                new PrefixedDecoder(
                    dec:  VexInstr(Mnemonic.rcpps, Mnemonic.vrcpps, Vps,Wps),
                    dec66:s_invalid,
                    decF3:VexInstr(Mnemonic.rcpss, Mnemonic.vrcpss, Vss,Hss,Wss),
                    decF2:s_invalid),

                new PrefixedDecoder(
                    dec: VexInstr(Mnemonic.andps, Mnemonic.vandps, Vps,Hps,Wps),
                    dec66:VexInstr(Mnemonic.andpd, Mnemonic.vandpd, Vpd,Hpd,Wpd)),
                new PrefixedDecoder(
                    dec:  VexInstr(Mnemonic.andnps, Mnemonic.vandnps, Vps,Hps,Wps),
                    dec66:VexInstr(Mnemonic.andnpd, Mnemonic.vandnpd, Vpd,Hpd,Wpd)),
                new PrefixedDecoder(
                    dec:  VexInstr(Mnemonic.orps, Mnemonic.vorps, Vps,Hps,Wps),
                    dec66:VexInstr(Mnemonic.orpd, Mnemonic.vorpd, Vpd,Hpd,Wpd)),
                new PrefixedDecoder(
                    dec:  VexInstr(Mnemonic.xorps, Mnemonic.vxorps, Vps,Hps,Wps),
                    dec66:VexInstr(Mnemonic.xorpd, Mnemonic.vxorpd, Vpd,Hpd,Wpd)),

                new PrefixedDecoder(
                    dec:  VexInstr(Mnemonic.addps, Mnemonic.vaddps, Vps,Hps,Wps),
                    dec66:VexInstr(Mnemonic.addpd, Mnemonic.vaddpd, Vpd,Hpd,Wpd),
                    decF3:VexInstr(Mnemonic.addss, Mnemonic.vaddss, Vss,Hss,Wss),
                    decF2:VexInstr(Mnemonic.addsd, Mnemonic.vaddsd, Vsd,Hsd,Wsd)),
                new PrefixedDecoder(
                    dec:  VexInstr(Mnemonic.mulps, Mnemonic.vmulps, Vps,Hps,Wps),
                    dec66:VexInstr(Mnemonic.mulpd, Mnemonic.vmulpd, Vpd,Hpd,Wpd),
                    decF3:VexInstr(Mnemonic.mulss, Mnemonic.vmulss, Vss,Hss,Wss),
                    decF2:VexInstr(Mnemonic.mulsd, Mnemonic.vmulsd, Vsd,Hsd,Wsd)),
				new PrefixedDecoder(
                    dec:  VexInstr(Mnemonic.cvtps2pd, Mnemonic.vcvtps2pd, Vpd,Wps),
                    dec66:VexInstr(Mnemonic.cvtpd2ps, Mnemonic.vcvtpd2ps, Vps,Wpd),
                    decF3:VexInstr(Mnemonic.cvtss2sd, Mnemonic.vcvtss2sd, Vsd,Hx,Wss),
                    decF2:VexInstr(Mnemonic.cvtsd2ss, Mnemonic.vcvtsd2ss, Vss,Hx,Wsd)),
                new PrefixedDecoder(
                    dec:  VexInstr(Mnemonic.cvtdq2ps, Mnemonic.vcvtdq2ps, Vps,Wdq),
                    dec66:VexInstr(Mnemonic.cvtps2dq, Mnemonic.vcvtps2dq, Vdq,Wps),
                    decF3:VexInstr(Mnemonic.cvttps2dq, Mnemonic.vcvttps2dq, Vdq,Wps),
                    decF2:s_invalid),
                new PrefixedDecoder(
                    dec:  VexInstr(Mnemonic.subps, Mnemonic.vsubps, Vps,Hps,Wps),
                    dec66:VexInstr(Mnemonic.subpd, Mnemonic.vsubpd, Vpd,Hpd,Wpd),
                    decF3:VexInstr(Mnemonic.subss, Mnemonic.vsubss, Vss,Hss,Wss),
                    decF2:VexInstr(Mnemonic.subsd, Mnemonic.vsubsd, Vsd,Hsd,Wsd)),
                new PrefixedDecoder(
                    dec:  VexInstr(Mnemonic.minps, Mnemonic.vminps, Vps,Hps,Wps),
                    dec66:VexInstr(Mnemonic.minpd, Mnemonic.vminpd, Vpd,Hpd,Wpd),
                    decF3:VexInstr(Mnemonic.minss, Mnemonic.vminss, Vss,Hss,Wss),
                    decF2:VexInstr(Mnemonic.minsd, Mnemonic.vminsd, Vsd,Hsd,Wsd)),
                new PrefixedDecoder(
                    dec:  VexInstr(Mnemonic.divps, Mnemonic.vdivps, Vps,Hps,Wps),
                    dec66:VexInstr(Mnemonic.divpd, Mnemonic.vdivpd, Vpd,Hpd,Wpd),
                    decF3:VexInstr(Mnemonic.divss, Mnemonic.vdivss, Vss,Hss,Wss),
                    decF2:VexInstr(Mnemonic.divsd, Mnemonic.vdivsd, Vsd,Hsd,Wsd)),
                new PrefixedDecoder(
                    dec:  VexInstr(Mnemonic.maxps, Mnemonic.vmaxps, Vps,Hps,Wps),
                    dec66:VexInstr(Mnemonic.maxpd, Mnemonic.vmaxpd, Vpd,Hpd,Wpd),
                    decF3:VexInstr(Mnemonic.maxss, Mnemonic.vmaxss, Vss,Hss,Wss),
                    decF2:VexInstr(Mnemonic.maxsd, Mnemonic.vmaxsd, Vsd,Hsd,Wsd)),
					
				// 0F 60
				new PrefixedDecoder(
                    Instr(Mnemonic.punpcklbw, Pq,Qd),
                    dec66:VexInstr(Mnemonic.punpcklbw, Mnemonic.vpunpcklbw, Vx,Wx)),
				new PrefixedDecoder(
                    Instr(Mnemonic.punpcklwd, Pq,Qd),
                    VexInstr(Mnemonic.punpcklwd, Mnemonic.vpunpcklwd, Vx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.punpckldq, Pq,Qd),
                    VexInstr(Mnemonic.punpckldq, Mnemonic.vpunpckldq, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.packsswb, Pq,Qd),
                    VexInstr(Mnemonic.packsswb, Mnemonic.vpacksswb, Vx,Hx,Wx)),

                new PrefixedDecoder(
                    Instr(Mnemonic.pcmpgtb, Pq,Qd),
                    VexInstr(Mnemonic.pcmpgtb, Mnemonic.vpcmpgtb, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.pcmpgtw, Pq,Qd),
                    VexInstr(Mnemonic.pcmpgtw, Mnemonic.vpcmpgtw, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.pcmpgtd, Pq,Qd),
                    VexInstr(Mnemonic.pcmpgtd, Mnemonic.vpcmpgtd, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.packuswb, Pq,Qd),
                    VexInstr(Mnemonic.punpckhbw, Mnemonic.vpunpckhbw, Vx,Hx,Wx)),

                new PrefixedDecoder(
                    Instr(Mnemonic.punpckhbw, Pq,Qd),
                    VexInstr(Mnemonic.punpckhbw, Mnemonic.vpunpckhbw, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    VexInstr(Mnemonic.punpckhwd, Mnemonic.vpunpckhwd, Pq,Qd),
                    VexInstr(Mnemonic.punpckhwd, Mnemonic.vpunpckhwd, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    VexInstr(Mnemonic.punpckhdq, Mnemonic.vpunpckhdq, Pq,Qd),
                    VexInstr(Mnemonic.punpckhdq, Mnemonic.vpunpckhdq, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    VexInstr(Mnemonic.packssdw, Mnemonic.vpackssdw, Pq,Qd),
                    VexInstr(Mnemonic.packssdw, Mnemonic.vpackssdw, Vx,Hx,Wx)),

                 new PrefixedDecoder(
                    s_invalid,
                    VexInstr(Mnemonic.punpcklqdq, Mnemonic.vpunpcklqdq, Vx,Hx,Wx)),
                 new PrefixedDecoder(
                    s_invalid,
                    VexInstr(Mnemonic.punpckhqdq, Mnemonic.vpunpckhqdq, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.movd, Py,Ey),
                    dec66:VexInstr(Mnemonic.movd,Mnemonic.vmovd, Vy,Ey)),
				new PrefixedDecoder(
                    Instr(Mnemonic.movq, Pq,Qq),
                    dec66:VexInstr(Mnemonic.movdqa, Mnemonic.vmovdqa, Vx,Wx),
                    decF3:VexInstr(Mnemonic.movdqu, Mnemonic.vmovdqu, Vx,Wx)),

				// 0F 70
				new PrefixedDecoder(
                    Instr(Mnemonic.pshufw, Pq,Qq,Ib),
                    dec66:VexInstr(Mnemonic.pshufd, Mnemonic.vpshufd, Vx,Wx,Ib),
                    decF3:VexInstr(Mnemonic.pshufhw, Mnemonic.vpshufhw, Vx,Wx,Ib),
                    decF2:VexInstr(Mnemonic.pshuflw, Mnemonic.vpshuflw, Vx,Wx,Ib)),
                new GroupDecoder(Grp12),
                new GroupDecoder(Grp13),
                new GroupDecoder(Grp14),

				new PrefixedDecoder(
                    Instr(Mnemonic.pcmpeqb, Pq,Qq),
                    dec66:VexInstr(Mnemonic.pcmpeqb, Mnemonic.vpcmpeqb, Vx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.pcmpeqw, Pq,Qq),
                    dec66:VexInstr(Mnemonic.pcmpeqw, Mnemonic.vpcmpeqw, Vx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.pcmpeqd, Pq,Qq),
                    dec66:VexInstr(Mnemonic.pcmpeqd, Mnemonic.vpcmpeqd, Vx,Wx)),
                VexInstr(
                Instr(Mnemonic.emms, InstrClass.System),
                    s_nyi),//$TODO: vzeroupper, vzeroall

				new PrefixedDecoder(
                    dec:Instr(Mnemonic.vmread, InstrClass.System, Ey,Gy)),
				new PrefixedDecoder(
                    dec:Instr(Mnemonic.vmwrite, InstrClass.System, Gy,Ey)),
				s_invalid,
				s_invalid,

                new PrefixedDecoder(
                    dec:s_invalid,
                    dec66:VexInstr(Mnemonic.haddpd, Mnemonic.vhaddpd,  Vpd,Hpd,Wpd),
                    decF2:VexInstr(Mnemonic.haddps, Mnemonic.vhaddps, Vps,Hps,Wps)),
                new PrefixedDecoder(
                    dec:s_invalid,
                    dec66:VexInstr(Mnemonic.hsubpd, Mnemonic.vhsubpd, Vpd,Hpd,Wpd),
                    decF2:VexInstr(Mnemonic.hsubps, Mnemonic.vhsubps, Vps,Hps,Wps)),
                new PrefixedDecoder(
                    dec: Instr(Mnemonic.movd, Ey,Pd),
                    decWide: Instr(Mnemonic.movq, Ey,Pd),
                    dec66: VexInstr(Mnemonic.movd, Mnemonic.vmovd, Ey,Vy),
                    dec66Wide: VexInstr(Mnemonic.movq, Mnemonic.vmovq, Ey,Vy),
                    decF3: VexInstr(Mnemonic.movq, Mnemonic.vmovq, Vy,Wy)),
				new PrefixedDecoder(
                    dec:Instr(Mnemonic.movq, Qq,Pq),
                    dec66:VexInstr(Mnemonic.movdqa, Mnemonic.vmovdqa, Wx,Vx),
                    decF3:VexInstr(Mnemonic.movdqu, Mnemonic.vmovdqu, Wx,Vx)),

				// 0F 80
				Instr(Mnemonic.jo,	InstrClass.ConditionalTransfer, Jv),
				Instr(Mnemonic.jno, InstrClass.ConditionalTransfer, Jv),
				Instr(Mnemonic.jc,	InstrClass.ConditionalTransfer, Jv),
				Instr(Mnemonic.jnc,	InstrClass.ConditionalTransfer, Jv),
				Instr(Mnemonic.jz,	InstrClass.ConditionalTransfer, Jv),
				Instr(Mnemonic.jnz, InstrClass.ConditionalTransfer, Jv),
				Instr(Mnemonic.jbe, InstrClass.ConditionalTransfer, Jv),
				Instr(Mnemonic.ja,  InstrClass.ConditionalTransfer, Jv),

				Instr(Mnemonic.js,  InstrClass.ConditionalTransfer, Jv),
				Instr(Mnemonic.jns, InstrClass.ConditionalTransfer, Jv),
				Instr(Mnemonic.jpe, InstrClass.ConditionalTransfer, Jv),
				Instr(Mnemonic.jpo, InstrClass.ConditionalTransfer, Jv),
				Instr(Mnemonic.jl,  InstrClass.ConditionalTransfer, Jv),
				Instr(Mnemonic.jge, InstrClass.ConditionalTransfer, Jv),
				Instr(Mnemonic.jle, InstrClass.ConditionalTransfer, Jv),
				Instr(Mnemonic.jg,  InstrClass.ConditionalTransfer, Jv),

				// 0F 90
				Instr(Mnemonic.seto, Eb),
				Instr(Mnemonic.setno,Eb),
				Instr(Mnemonic.setc, Eb),
				Instr(Mnemonic.setnc,Eb),
				Instr(Mnemonic.setz, Eb),
				Instr(Mnemonic.setnz,Eb),
				Instr(Mnemonic.setbe,Eb),
				Instr(Mnemonic.seta, Eb),

				Instr(Mnemonic.sets,  Eb),
				Instr(Mnemonic.setns, Eb),
				Instr(Mnemonic.setpe, Eb),
				Instr(Mnemonic.setpo, Eb),
				Instr(Mnemonic.setl,  Eb),
				Instr(Mnemonic.setge, Eb),
				Instr(Mnemonic.setle, Eb),
				Instr(Mnemonic.setg,  Eb),

				// 0F A0
				Instr(Mnemonic.push, s4),
				Instr(Mnemonic.pop, s4),
				Instr(Mnemonic.cpuid),
				Instr(Mnemonic.bt, Ev,Gv),
				Instr(Mnemonic.shld, Ev,Gv,Ib),
				Instr(Mnemonic.shld, Ev,Gv,c),
				s_invalid,
                s_invalid,

				Instr(Mnemonic.push, s5),
				Instr(Mnemonic.pop, s5),
				Instr(Mnemonic.rsm, InstrClass.System),
                Instr(Mnemonic.bts, Ev,Gv),
				Instr(Mnemonic.shrd, Ev,Gv,Ib),
				Instr(Mnemonic.shrd, Ev,Gv,c),
				new GroupDecoder(Grp15),
				Instr(Mnemonic.imul, Gv,Ev),

				// 0F B0
				Instr(Mnemonic.cmpxchg, Eb,Gb),
				Instr(Mnemonic.cmpxchg, Ev,Gv),
				Instr(Mnemonic.lss, Gv,Mp),
				Instr(Mnemonic.btr, Ev,Gv),
                Instr(Mnemonic.lfs, Gv,Mp),
				Instr(Mnemonic.lgs, Gv,Mp),
				Instr(Mnemonic.movzx, Gv,Eb),
				Instr(Mnemonic.movzx, Gv,Ew),

				new PrefixedDecoder(
                    dec:Instr(Mnemonic.jmpe),
                    decF3:Instr(Mnemonic.popcnt, Gv,Ev)),
				new GroupDecoder(Grp10, Gv,Ev), 
				new GroupDecoder(Grp8, Ev,Ib),
				Instr(Mnemonic.btc, Gv,Ev),

                new PrefixedDecoder(
                    dec: Instr(Mnemonic.bsf, Gv,Ev),
                    dec66: Instr(Mnemonic.bsf, Gw,Ew),
                    decF3: Instr(Mnemonic.tzcnt, Gv,Ev)),
                new PrefixedDecoder(
                    dec: Instr(Mnemonic.bsr, Gv,Ev),
                    dec66: Instr(Mnemonic.bsr, Gw,Ew),
                    decF3: Instr(Mnemonic.lzcnt, Gv,Ev)),
				Instr(Mnemonic.movsx, Gv,Eb),
				Instr(Mnemonic.movsx, Gv,Ew),

				// 0F C0
				Instr(Mnemonic.xadd, Eb,Gb),
				Instr(Mnemonic.xadd, Ev,Gv),
				new PrefixedDecoder(
                    dec:  VexInstr(Mnemonic.cmpps, Mnemonic.vcmpps, Vps,Hps,Wps,Ib),
                    dec66:VexInstr(Mnemonic.cmppd, Mnemonic.vcmppd, Vpd,Hpd,Wpd,Ib),
                    decF3:VexInstr(Mnemonic.cmpss, Mnemonic.vcmpss, Vss,Hss,Wss,Ib),
                    decF2:VexInstr(Mnemonic.cmpsd, Mnemonic.vcmpsd, Vpd,Hpd,Wpd,Ib)),
				new PrefixedDecoder(
                    Instr(Mnemonic.movnti, My,Gy),
                    s_invalid),
                new PrefixedDecoder(
                    VexInstr(Mnemonic.pinsrw, Mnemonic.vpinsrw, Pq,Ry),     //$TODO: encoding is weird.
                    VexInstr(Mnemonic.pinsrw, Mnemonic.vpinsrw, Vdq,Hdq,Ry)),
                new PrefixedDecoder(
                    Instr(Mnemonic.pextrw, Gd,Nq,Ib),
                    VexInstr(Mnemonic.pextrw, Mnemonic.vpextrw, Gd,Udq,Ib)),
                new PrefixedDecoder(
                    VexInstr(Mnemonic.shufps, Mnemonic.vshufps, Vps,Hps,Wps,Ib),
                    VexInstr(Mnemonic.shufpd, Mnemonic.vshufpd, Vpd,Hpd,Wpd,Ib)),
				new GroupDecoder(Grp9),

				Instr(Mnemonic.bswap, rv),
				Instr(Mnemonic.bswap, rv),
				Instr(Mnemonic.bswap, rv),
				Instr(Mnemonic.bswap, rv),
				Instr(Mnemonic.bswap, rv),
				Instr(Mnemonic.bswap, rv),
				Instr(Mnemonic.bswap, rv),
				Instr(Mnemonic.bswap, rv),

				// 0F D0
				new PrefixedDecoder(
                    dec:  s_invalid,
					dec66:VexInstr(Mnemonic.addsubpd, Mnemonic.vaddsubpd, Vpd,Hpd,Wpd),
					decF3:s_invalid,
					decF2:VexInstr(Mnemonic.addsubps, Mnemonic.vaddsubps, Vps,Hps,Wps)),
				new PrefixedDecoder(
                    Instr(Mnemonic.psrlw, Pq,Qq),
                    VexInstr(Mnemonic.psrlw, Mnemonic.vpsrlw, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.psrld, Pq,Qq),
                    VexInstr(Mnemonic.psrld, Mnemonic.vpsrld, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.psrlq, Pq,Qq),
                    VexInstr(Mnemonic.psrlq, Mnemonic.vpsrlq, Vx,Hx,Wx)),

                new PrefixedDecoder(
                    Instr(Mnemonic.paddq, Pq,Qq),
                    VexInstr(Mnemonic.paddq, Mnemonic.vpaddq, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.pmullw, Pq,Qq),
                    VexInstr(Mnemonic.pmullw, Mnemonic.vpmullw, Vx,Hx,Wx)),
				Instr(Mnemonic.movq, Wx,Vx),
                new PrefixedDecoder(
                    Instr(Mnemonic.pmovmskb, Gd,Nq),
                    VexInstr(Mnemonic.pmovmskb, Mnemonic.vpmovmskb, Gd,Ux)),

                new PrefixedDecoder(
                    Instr(Mnemonic.psubusb, Pq,Qq),
                    VexInstr(Mnemonic.psubusb, Mnemonic.vpsubusb, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.psubusw, Pq,Qq),
                    VexInstr(Mnemonic.psubusw, Mnemonic.vpsubusw, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.pminub, Pq,Qq),
                    VexInstr(Mnemonic.pminub, Mnemonic.vpminub, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.pand, Pq,Qq),
                    VexInstr(Mnemonic.pand, Mnemonic.vpand, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.paddusb, Pq,Qq),
                    VexInstr(Mnemonic.paddusb, Mnemonic.vpaddusb, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.paddusw, Pq,Qq),
                    VexInstr(Mnemonic.paddusw, Mnemonic.vpaddusw, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.pmaxub, Pq,Qq),
                    VexInstr(Mnemonic.pmaxub, Mnemonic.vpmaxub, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.pandn, Pq,Qq),
                    VexInstr(Mnemonic.pandn, Mnemonic.vpandn, Vx,Hx,Wx)),

				// 0F E0
                new PrefixedDecoder(
                    Instr(Mnemonic.pavgb, Pq,Qq),
                    VexInstr(Mnemonic.pavgb, Mnemonic.vpavgb, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.psraw, Pq,Qq),
                    VexInstr(Mnemonic.psraw, Mnemonic.vpsraw, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.psrad, Pq,Qq),
                    VexInstr(Mnemonic.psrad, Mnemonic.vpsrad, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.pavgw, Pq,Qq),
                    VexInstr(Mnemonic.pavgw, Mnemonic.vpavgw, Vx,Hx,Wx)),

                new PrefixedDecoder(
                    Instr(Mnemonic.pmulhuw, Pq,Qq),
                    VexInstr(Mnemonic.pmulhuw, Mnemonic.vpmulhuw, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.pmulhw, Pq,Qq),
                    VexInstr(Mnemonic.pmulhw, Mnemonic.vpmulhw, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    dec:  s_invalid,
                    dec66:VexInstr(Mnemonic.cvttpd2dq, Mnemonic.vcvttpd2dq, Vdq,Wpd),
                    decF3:VexInstr(Mnemonic.cvtdq2pd, Mnemonic.vcvtdq2pd, Vx,Wpd),
                    decF2:VexInstr(Mnemonic.cvtpd2dq, Mnemonic.vcvtpd2dq, Vdq,Wpd)),
                new PrefixedDecoder(
                    Instr(Mnemonic.movntq, Mq,Pq),
                    VexInstr(Mnemonic.movntq, Mnemonic.vmovntq, Mx,Vx)),

                new PrefixedDecoder(
                    Instr(Mnemonic.psubsb, Pq,Qq),
                    VexInstr(Mnemonic.psubsb, Mnemonic.vpsubsb, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.psubsw, Pq,Qq),
                    VexInstr(Mnemonic.psubsw, Mnemonic.vpsubsw, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.pminsw, Pq,Qq),
                    VexInstr(Mnemonic.pminsw, Mnemonic.vpminsw, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.por, Pq,Qq),
                    VexInstr(Mnemonic.por, Mnemonic.vpor, Vx,Hx,Wx)),

                new PrefixedDecoder(
                    Instr(Mnemonic.paddsb, Pq,Qq),
                    VexInstr(Mnemonic.paddsb, Mnemonic.vpaddsb, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.paddsw, Pq,Qq),
                    VexInstr(Mnemonic.paddsw, Mnemonic.vpaddsw, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.pmaxsw, Pq,Qq),
                    VexInstr(Mnemonic.pmaxsw, Mnemonic.vpmaxsw, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.pxor, Pq,Qq),
                    VexInstr(Mnemonic.pxor, Mnemonic.vpxor, Vx,Hx,Wx)),

				// 0F F0
                new PrefixedDecoder(
                    s_invalid,
                    decF2:VexInstr(Mnemonic.lddqu, Mnemonic.vlddqu, Vx,Mx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.psllw, Pq,Qq), 
                    dec66:VexInstr(Mnemonic.psllw, Mnemonic.vpsllw, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.pslld, Pq,Qq),
                    dec66:VexInstr(Mnemonic.pslld, Mnemonic.vpslld, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.psllq, Pq,Qq),
                    dec66:VexInstr(Mnemonic.psllq, Mnemonic.vpsllq, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.pmuludq, Pq,Qq),
                    dec66:VexInstr(Mnemonic.pmuludq, Mnemonic.vpmuludq, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.pmaddwd, Pq,Qq),
                    dec66:VexInstr(Mnemonic.pmaddwd, Mnemonic.vpmaddwd, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.psadbw, Pq,Qq),
                    dec66:VexInstr(Mnemonic.psadbw, Mnemonic.vpsadbw, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.maskmovq, Pq,Qq),
                    dec66:VexInstr(Mnemonic.maskmovdqu, Mnemonic.vmaskmovdqu, Vdq,Udq)),

                new PrefixedDecoder(
                    Instr(Mnemonic.psubb, Pq,Qq),
                    VexInstr(Mnemonic.psubb, Mnemonic.vpsubb, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.psubw, Pq,Qq),
                    VexInstr(Mnemonic.psubw, Mnemonic.vpsubw, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.psubd, Pq,Qq),
                    VexInstr(Mnemonic.psubd, Mnemonic.vpsubd, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.psubq, Pq,Qq),
                    VexInstr(Mnemonic.psubq, Mnemonic.vpsubq, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.paddb, Pq,Qq),
                    VexInstr(Mnemonic.paddb, Mnemonic.vpaddb, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.paddw, Pq,Qq),
                    VexInstr(Mnemonic.paddw, Mnemonic.vpaddw, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.paddd, Pq,Qq),
                    VexInstr(Mnemonic.paddd, Mnemonic.vpaddd, Vx,Hx,Wx)),
				Instr(Mnemonic.ud0, InstrClass.Invalid,  Gv,Ev)
			};
        }
    }
}
