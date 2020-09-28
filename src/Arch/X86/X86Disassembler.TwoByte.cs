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
            return new Decoder[]
            {
				// 0F 00
				new GroupDecoder(6),
                new GroupDecoder(7),
                Instr(Mnemonic.lar, InstrClass.System, Gv,Ew),
                Instr(Mnemonic.lsl, InstrClass.System, Gv,Ew),
                s_invalid,
                new Alternative64Decoder(
                    s_invalid,
                    Instr(Mnemonic.syscall, InstrClass.Transfer|InstrClass.Call)),
                Instr(Mnemonic.clts),
                new Alternative64Decoder(
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
                    Instr(Mnemonic.movups, Vps,Wps),
                    Instr(Mnemonic.movupd, Vpd,Wpd),
                    Instr(Mnemonic.movss,  Vx,Wss),
                    Instr(Mnemonic.movsd,  Vx,Wsd)),
                new PrefixedDecoder(
                    Instr(Mnemonic.movups, Wps,Vps),
                    Instr(Mnemonic.movupd, Wpd,Vpd),
                    Instr(Mnemonic.movss,  Wss,Vss),
                    Instr(Mnemonic.movsd,  Wsd,Vsd)),
                new PrefixedDecoder(
                    Instr(Mnemonic.movlps,   Vq,Hq,Mq),
                    Instr(Mnemonic.movlpd,   Vq,Hq,Mq),
                    Instr(Mnemonic.movsldup, Vx,Wx),
                    Instr(Mnemonic.movddup,  Vx,Wx)),
                new PrefixedDecoder(
					Instr(Mnemonic.vmovlps, Mq,Vq),
                    dec66:Instr(Mnemonic.vmovlpd, Mq,Vq)),

                new PrefixedDecoder(
                    Instr(Mnemonic.unpcklps, Vx,Hx,Wx),
                    Instr(Mnemonic.unpcklpd, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.unpckhps, Vx,Hx,Wx),
                    Instr(Mnemonic.unpckhpd, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.movlhps, Vx,Wx),
                    Instr(Mnemonic.movhpd, Vx,Wx),
                    Instr(Mnemonic.movshdup, Vx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.movhps, Mq,Vq),
                    Instr(Mnemonic.movhpd, Mq,Vq)),

                new GroupDecoder(16),
                Instr(Mnemonic.nop, InstrClass.Linear|InstrClass.Padding, Ev),
                Instr(Mnemonic.nop, InstrClass.Linear|InstrClass.Padding, Ev),
                Instr(Mnemonic.nop, InstrClass.Linear|InstrClass.Padding, Ev),
                Instr(Mnemonic.cldemote, Eb),
                Instr(Mnemonic.nop, InstrClass.Linear|InstrClass.Padding, Ev),
                Instr(Mnemonic.nop, InstrClass.Linear|InstrClass.Padding, Ev),
                Instr(Mnemonic.nop, InstrClass.Linear|InstrClass.Padding, Ev),

				// 0F 20
				Instr(Mnemonic.mov, Rv,Cd),
                Instr(Mnemonic.mov, Rv,Dd),
                Instr(Mnemonic.mov, Cd,Rv),
                Instr(Mnemonic.mov, Dd,Rv),
				s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,

                new PrefixedDecoder(
                    dec:  Instr(Mnemonic.movaps, Vps,Wps),
                    dec66:Instr(Mnemonic.movapd, Vpd,Wpd)),
                new PrefixedDecoder(
                    dec:  Instr(Mnemonic.movaps, Wps,Vps),
                    dec66:Instr(Mnemonic.movapd, Wpd,Vpd)),
                new PrefixedDecoder(
                    dec:  Instr(Mnemonic.cvtpi2ps, Vps,Qpi),
                    dec66:Instr(Mnemonic.cvtpi2pd, Vpd,Qpi),
                    decF3:Instr(Mnemonic.cvtsi2ss, Vss,Hss,Ey),
                    decF2:Instr(Mnemonic.cvtsi2sd, Vsd,Hsd,Ey)),
                new PrefixedDecoder(
                    dec:  Instr(Mnemonic.movntps, Mps,Vps),
                    dec66:Instr(Mnemonic.movntpd, Mpd,Vpd)),
                new PrefixedDecoder(
                    dec:  Instr(Mnemonic.cvttps2pi, Ppi,Wps),
                    dec66:Instr(Mnemonic.cvttpd2pi, Ppi,Wpq),
                    decF3:Instr(Mnemonic.cvttss2si, Gd,Wss),
                    decF2:Instr(Mnemonic.cvttsd2si, Gd,Wsd)),
                new PrefixedDecoder(
                    dec:  Instr(Mnemonic.cvtps2pi, Ppi,Wps),
                    dec66:Instr(Mnemonic.cvtpd2si, Qpi,Wpd),
                    decF3:Instr(Mnemonic.cvtss2si, Gy,Wss),
                    decF2:Instr(Mnemonic.cvtsd2si, Gy,Wsd)),
                new PrefixedDecoder(
                    dec:  Instr(Mnemonic.ucomiss, Vss,Wss),
                    dec66:Instr(Mnemonic.ucomisd, Vsd,Wsd)),
                new PrefixedDecoder(
                    dec:  Instr(Mnemonic.comiss, Vss,Wss),
                    dec66:Instr(Mnemonic.comisd, Vsd,Wsd)),

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
                    Instr(Mnemonic.movmskps, Gy,Ups),
                    Instr(Mnemonic.movmskpd, Gy,Upd)),
                new PrefixedDecoder(
                    dec:  Instr(Mnemonic.sqrtps, Vps,Wps),
                    dec66:Instr(Mnemonic.sqrtpd, Vpd,Wpd),
                    decF3:Instr(Mnemonic.sqrtss, Vss,Hss,Wss),
                    decF2:Instr(Mnemonic.sqrtsd, Vsd,Hsd,Wsd)),
                new PrefixedDecoder(
                    dec:  Instr(Mnemonic.rsqrtps, Vps,Wps),
                    dec66:s_invalid,
                    decF3:Instr(Mnemonic.rsqrtss, Vss,Hss,Wss),
                    decF2:s_invalid),
                new PrefixedDecoder(
                    dec:  Instr(Mnemonic.rcpps, Vps,Wps),
                    dec66:s_invalid,
                    decF3:Instr(Mnemonic.rcpss, Vss,Hss,Wss),
                    decF2:s_invalid),
                new PrefixedDecoder(
                    dec: Instr(Mnemonic.andps, Vps,Hps,Wps),
                    dec66:Instr(Mnemonic.andpd, Vpd,Hpd,Wpd)),
                new PrefixedDecoder(
                    dec:  Instr(Mnemonic.andnps, Vps,Hps,Wps),
                    dec66:Instr(Mnemonic.andnpd, Vpd,Hpd,Wpd)),
                new PrefixedDecoder(
                    dec:  Instr(Mnemonic.orps, Vps,Hps,Wps),
                    dec66:Instr(Mnemonic.orpd, Vpd,Hpd,Wpd)),
                new PrefixedDecoder(
                    dec:  Instr(Mnemonic.xorps, Vps,Hps,Wps),
                    dec66:Instr(Mnemonic.xorpd, Vpd,Hpd,Wpd)),

                new PrefixedDecoder(
                    dec:  Instr(Mnemonic.addps, Vps,Hps,Wps),
                    dec66:Instr(Mnemonic.addpd, Vpd,Hpd,Wpd),
                    decF3:Instr(Mnemonic.addss, Vss,Hss,Wss),
                    decF2:Instr(Mnemonic.addsd, Vsd,Hsd,Wsd)),
                new PrefixedDecoder(
                    dec:  Instr(Mnemonic.mulps, Vps,Hps,Wps),
                    dec66:Instr(Mnemonic.mulpd, Vpd,Hpd,Wpd),
                    decF3:Instr(Mnemonic.mulss, Vss,Hss,Wss),
                    decF2:Instr(Mnemonic.mulsd, Vsd,Hsd,Wsd)),
				new PrefixedDecoder(
                    dec:  Instr(Mnemonic.cvtps2pd, Vpd,Wps),
                    dec66:Instr(Mnemonic.cvtpd2ps, Vps,Wpd),
                    decF3:Instr(Mnemonic.cvtss2sd, Vsd,Hx,Wss),
                    decF2:Instr(Mnemonic.cvtsd2ss, Vss,Hx,Wsd)),
                new PrefixedDecoder(
                    dec:  Instr(Mnemonic.cvtdq2ps, Vps,Wdq),
                    dec66:Instr(Mnemonic.cvtps2dq, Vdq,Wps),
                    decF3:Instr(Mnemonic.cvttps2dq, Vdq,Wps),
                    decF2:s_invalid),
                new PrefixedDecoder(
                    dec:  Instr(Mnemonic.subps, Vps,Hps,Wps),
                    dec66:Instr(Mnemonic.subpd, Vpd,Hpd,Wpd),
                    decF3:Instr(Mnemonic.subss, Vss,Hss,Wss),
                    decF2:Instr(Mnemonic.subsd, Vsd,Hsd,Wsd)),
                new PrefixedDecoder(
                    dec:  Instr(Mnemonic.minps, Vps,Hps,Wps),
                    dec66:Instr(Mnemonic.minpd, Vpd,Hpd,Wpd),
                    decF3:Instr(Mnemonic.minss, Vss,Hss,Wss),
                    decF2:Instr(Mnemonic.minsd, Vsd,Hsd,Wsd)),
                new PrefixedDecoder(
                    dec:  Instr(Mnemonic.divps, Vps,Hps,Wps),
                    dec66:Instr(Mnemonic.divpd, Vpd,Hpd,Wpd),
                    decF3:Instr(Mnemonic.divss, Vss,Hss,Wss),
                    decF2:Instr(Mnemonic.divsd, Vsd,Hsd,Wsd)),
                new PrefixedDecoder(
                    dec:  Instr(Mnemonic.maxps, Vps,Hps,Wps),
                    dec66:Instr(Mnemonic.maxpd, Vpd,Hpd,Wpd),
                    decF3:Instr(Mnemonic.maxss, Vss,Hss,Wss),
                    decF2:Instr(Mnemonic.maxsd, Vsd,Hsd,Wsd)),
					
				// 0F 60
				new PrefixedDecoder(
                    Instr(Mnemonic.punpcklbw, Pq,Qd),
                    dec66:Instr(Mnemonic.punpcklbw, Vx,Wx)),
				new PrefixedDecoder(
                    Instr(Mnemonic.punpcklwd, Pq,Qd),
                    Instr(Mnemonic.punpcklwd, Vx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.punpckldq, Pq,Qd),
                    Instr(Mnemonic.punpckldq, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.packsswb, Pq,Qd),
                    Instr(Mnemonic.vpacksswb, Vx,Hx,Wx)),

                new PrefixedDecoder(
                    Instr(Mnemonic.pcmpgtb, Pq,Qd),
                    Instr(Mnemonic.pcmpgtb, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.pcmpgtw, Pq,Qd),
                    Instr(Mnemonic.pcmpgtw, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.pcmpgtd, Pq,Qd),
                    Instr(Mnemonic.pcmpgtd, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.packuswb, Pq,Qd),
                    Instr(Mnemonic.vpunpckhbw, Vx,Hx,Wx)),

                new PrefixedDecoder(
                    Instr(Mnemonic.punpckhbw, Pq,Qd),
                    Instr(Mnemonic.vpunpckhbw, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.punpckhwd, Pq,Qd),
                    Instr(Mnemonic.vpunpckhwd, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.punpckhdq, Pq,Qd),
                    Instr(Mnemonic.vpunpckhdq, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.packssdw, Pq,Qd),
                    Instr(Mnemonic.vpackssdw, Vx,Hx,Wx)),

                 new PrefixedDecoder(
                    s_invalid,
                    Instr(Mnemonic.vpunpcklqdq, Vx,Hx,Wx)),
                 new PrefixedDecoder(
                    s_invalid,
                    Instr(Mnemonic.vpunpckhqdq, Vx,Hx,Wx)),
                Instr(Mnemonic.movd, Vy,Ey),
				Instr(Mnemonic.movdqa, Vx,Wx),

				// 0F 70
				new PrefixedDecoder(
                    Instr(Mnemonic.pshufw, Pq,Qq,Ib),
                    dec66:Instr(Mnemonic.pshufd, Vx,Wx,Ib),
                    decF3:Instr(Mnemonic.pshufhw, Vx,Wx,Ib),
                    decF2:Instr(Mnemonic.pshuflw, Vx,Wx,Ib)),
                new GroupDecoder(12),
                new GroupDecoder(13),
                new GroupDecoder(14),

				new PrefixedDecoder(
                    Instr(Mnemonic.pcmpeqb, Pq,Qq),
                    dec66:Instr(Mnemonic.pcmpeqb, Vx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.pcmpeqw, Pq,Qq),
                    dec66:Instr(Mnemonic.pcmpeqw, Vx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.pcmpeqd, Pq,Qq),
                    dec66:Instr(Mnemonic.pcmpeqd, Vx,Wx)),
                Instr(Mnemonic.emms, InstrClass.System),

				new PrefixedDecoder(
                    dec:Instr(Mnemonic.vmread, InstrClass.System, Ey,Gy)),
				new PrefixedDecoder(
                    dec:Instr(Mnemonic.vmwrite, InstrClass.System, Gy,Ey)),
				s_invalid,
				s_invalid,

                new PrefixedDecoder(
                    dec:s_invalid,
                    dec66:Instr(Mnemonic.vhaddpd,  Vpd,Hpd,Wpd),
                    decF2:Instr(Mnemonic.vhaddps, Vps,Hps,Wps)),
                new PrefixedDecoder(
                    dec:s_invalid,
                    dec66:Instr(Mnemonic.vhsubpd, Vpd,Hpd,Wpd),
                    decF2:Instr(Mnemonic.vhsubps, Vps,Hps,Wps)),
                new PrefixedDecoder(
                    dec: Instr(Mnemonic.movd, Ey,Pd), decWide: Instr(Mnemonic.movq, Ey,Pd),
                    dec66: Instr(Mnemonic.movd, Ey,Vy), dec66Wide: Instr(Mnemonic.movq, Ey,Vy),
                    decF3: Instr(Mnemonic.movq, Vy,Wy)),
				new PrefixedDecoder(
                    dec:Instr(Mnemonic.movq, Qq,Pq),
                    dec66:Instr(Mnemonic.movdqa, Wx,Vx),
                    decF3:Instr(Mnemonic.movdqu, Wx,Vx)),

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
				Instr(Mnemonic.rsm),
                Instr(Mnemonic.bts, Ev,Gv),
				Instr(Mnemonic.shrd, Ev,Gv,Ib),
				Instr(Mnemonic.shrd, Ev,Gv,c),
				new GroupDecoder(15),
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
				Instr(Mnemonic.ud1, InstrClass.Invalid, Gv,Ev),
				new GroupDecoder(8, Ev,Ib),
				Instr(Mnemonic.btc, Gv,Ev),

                new PrefixedDecoder(
                    dec: Instr(Mnemonic.bsf, Gv,Ev),
                    decF3: Instr(Mnemonic.tzcnt, Gv,Ev)),
                new PrefixedDecoder(
                    dec: Instr(Mnemonic.bsr, Gv,Ev),
                    dec66: Instr(Mnemonic.bsr, Gv,Ev),
                    decF3: Instr(Mnemonic.lzcnt, Gv,Ev)),
				Instr(Mnemonic.movsx, Gv,Eb),
				Instr(Mnemonic.movsx, Gv,Ew),

				// 0F C0
				Instr(Mnemonic.xadd, Eb,Gb),
				Instr(Mnemonic.xadd, Ev,Gv),
				new PrefixedDecoder(
                    dec:  Instr(Mnemonic.cmpps, Vps,Hps,Wps,Ib),
                    dec66:Instr(Mnemonic.cmppd, Vpd,Hpd,Wpd,Ib),
                    decF3:Instr(Mnemonic.cmpss, Vss,Hss,Wss,Ib),
                    decF2:Instr(Mnemonic.cmpsd, Vpd,Hpd,Wpd,Ib)),
				new PrefixedDecoder(
                    Instr(Mnemonic.movnti, My,Gy),
                    s_invalid),
                new PrefixedDecoder(
                    Instr(Mnemonic.pinsrw, Pq,Ry),     //$TODO: encoding is weird.
                    Instr(Mnemonic.vpinsrw, Vdq,Hdq,Ry)),
                new PrefixedDecoder(
                    Instr(Mnemonic.pextrw, Gd,Nq,Ib),
                    Instr(Mnemonic.vextrw, Gd,Udq,Ib)),
                new PrefixedDecoder(
                    Instr(Mnemonic.vshufps, Vps,Hps,Wps,Ib),
                    Instr(Mnemonic.vshufpd, Vpd,Hpd,Wpd,Ib)),
				new GroupDecoder(9),

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
					dec66:Instr(Mnemonic.addsubpd, Vpd,Hpd,Wpd),
					decF3:s_invalid,
					decF2:Instr(Mnemonic.addsubps, Vps,Hps,Wps)),
				new PrefixedDecoder(
                    Instr(Mnemonic.psrlw, Pq,Qq),
                    Instr(Mnemonic.vpsrlw, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.psrld, Pq,Qq),
                    Instr(Mnemonic.vpsrld, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.psrlq, Pq,Qq),
                    Instr(Mnemonic.vpmullw, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.paddq, Pq,Qq),
                    Instr(Mnemonic.vpaddq, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.pmullw, Pq,Qq),
                    Instr(Mnemonic.vpmullw, Vx,Hx,Wx)),
				Instr(Mnemonic.movq, Wx,Vx),
                new PrefixedDecoder(
                    Instr(Mnemonic.pmovmskb, Gd,Nq),
                    Instr(Mnemonic.vpmovmskb, Gd,Ux)),

                new PrefixedDecoder(
                    Instr(Mnemonic.psubusb, Pq,Qq),
                    Instr(Mnemonic.vpsubusb, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.psubusw, Pq,Qq),
                    Instr(Mnemonic.vpsubusw, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.pminub, Pq,Qq),
                    Instr(Mnemonic.vpminub, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.pand, Pq,Qq),
                    Instr(Mnemonic.vpand, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.paddusb, Pq,Qq),
                    Instr(Mnemonic.vpaddusb, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.paddusw, Pq,Qq),
                    Instr(Mnemonic.vpaddusw, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.pmaxub, Pq,Qq),
                    Instr(Mnemonic.vpmaxub, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.pandn, Pq,Qq),
                    Instr(Mnemonic.vpandn, Vx,Hx,Wx)),

				// 0F E0
                new PrefixedDecoder(
                    Instr(Mnemonic.pavgb, Pq,Qq),
                    Instr(Mnemonic.vpavgb, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.psraw, Pq,Qq),
                    Instr(Mnemonic.vpsraw, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.psrad, Pq,Qq),
                    Instr(Mnemonic.vpsrad, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.pavgw, Pq,Qq),
                    Instr(Mnemonic.vpavgw, Vx,Hx,Wx)),

                new PrefixedDecoder(
                    Instr(Mnemonic.pmulhuw, Pq,Qq),
                    Instr(Mnemonic.vpmulhuw, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.pmulhw, Pq,Qq),
                    Instr(Mnemonic.vpmulhw, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    dec:  s_invalid,
                    dec66:Instr(Mnemonic.cvttpd2dq, Vdq,Wpd),
                    decF3:Instr(Mnemonic.cvtdq2pd, Vx,Wpd),
                    decF2:Instr(Mnemonic.cvtpd2dq, Vdq,Wpd)),
                new PrefixedDecoder(
                    Instr(Mnemonic.movntq, Mq,Pq),
                    Instr(Mnemonic.vmovntq, Mx,Vx)),

                new PrefixedDecoder(
                    Instr(Mnemonic.psubsb, Pq,Qq),
                    Instr(Mnemonic.vpsubsb, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.psubsw, Pq,Qq),
                    Instr(Mnemonic.vpsubsw, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.pminsw, Pq,Qq),
                    Instr(Mnemonic.vpminsw, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.por, Pq,Qq),
                    Instr(Mnemonic.vpor, Vx,Hx,Wx)),

                new PrefixedDecoder(
                    Instr(Mnemonic.paddsb, Pq,Qq),
                    Instr(Mnemonic.vpaddsb, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.paddsw, Pq,Qq),
                    Instr(Mnemonic.vpaddsw, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.pmaxsw, Pq,Qq),
                    Instr(Mnemonic.vpmaxsw, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.pxor, Pq,Qq),
                    Instr(Mnemonic.vpxor, Vx,Hx,Wx)),

				// 0F F0
                new PrefixedDecoder(
                    s_invalid,
                    decF2:Instr(Mnemonic.vlddqu, Vx,Mx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.psllw, Pq,Qq), 
                    dec66:Instr(Mnemonic.vpsllw, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.pslld, Pq,Qq),
                    dec66:Instr(Mnemonic.vpslld, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.psllq, Pq,Qq),
                    dec66:Instr(Mnemonic.vpsllq, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.pmuludq, Pq,Qq),
                    dec66:Instr(Mnemonic.vpmuludq, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.pmaddwd, Pq,Qq),
                    dec66:Instr(Mnemonic.vpmaddwd, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.psadbw, Pq,Qq),
                    dec66:Instr(Mnemonic.vpsadbw, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.maskmovq, Pq,Qq),
                    dec66:Instr(Mnemonic.vmaskmovdqu, Vdq,Udq)),

                new PrefixedDecoder(
                    Instr(Mnemonic.psubb, Pq,Qq),
                    Instr(Mnemonic.vpsubb, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.psubw, Pq,Qq),
                    Instr(Mnemonic.vpsubw, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.psubd, Pq,Qq),
                    Instr(Mnemonic.vpsubd, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.psubq, Pq,Qq),
                    Instr(Mnemonic.vpsubq, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.paddb, Pq,Qq),
                    Instr(Mnemonic.vpaddb, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.paddw, Pq,Qq),
                    Instr(Mnemonic.vpaddw, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Mnemonic.paddd, Pq,Qq),
                    Instr(Mnemonic.vpaddd, Vx,Hx,Wx)),
				Instr(Mnemonic.ud0, InstrClass.Invalid,  Gv,Ev),
			};
        }
    }
}
