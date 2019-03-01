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
        private static Decoder[] CreateTwobyteOprecs()
        {
            return new Decoder[]
            {
				// 0F 00
				new GroupDecoder(6),
                new GroupDecoder(7),
                Instr(Opcode.lar, InstrClass.System, Gv,Ew),
                Instr(Opcode.lsl, InstrClass.System, Gv,Ew),
                s_invalid,
                new Alternative64Decoder(
                    s_invalid,
                    Instr(Opcode.syscall, InstrClass.Transfer|InstrClass.Call)),
                Instr(Opcode.clts),
                new Alternative64Decoder(
                    s_invalid,
                    Instr(Opcode.sysret, InstrClass.Transfer)),

                Instr(Opcode.invd, InstrClass.System),
                Instr(Opcode.wbinvd, InstrClass.System),
                s_invalid,
                Instr(Opcode.ud2, InstrClass.Invalid),
                s_invalid,
                Instr(Opcode.prefetchw, Ev),
                Instr(Opcode.femms),    // AMD-specific
                s_invalid, // nyi("AMD 3D-Now instructions"), //$TODO: this requires adding separate processor model for AMD

				// 0F 10
				new PrefixedDecoder(
                    Instr(Opcode.movups, Vps,Wps),
                    Instr(Opcode.movupd, Vpd,Wpd),
                    Instr(Opcode.movss,  Vx,Wss),
                    Instr(Opcode.movsd,  Vx,Wsd)),
                new PrefixedDecoder(
                    Instr(Opcode.movups, Wps,Vps),
                    Instr(Opcode.movupd, Wpd,Vpd),
                    Instr(Opcode.movss,  Wss,Vss),
                    Instr(Opcode.movsd,  Wsd,Vsd)),
                new PrefixedDecoder(
                    Instr(Opcode.movlps,   Vq,Hq,Mq),
                    Instr(Opcode.movlpd,   Vq,Hq,Mq),
                    Instr(Opcode.movsldup, Vx,Wx),
                    Instr(Opcode.movddup,  Vx,Wx)),
                new PrefixedDecoder(
					Instr(Opcode.vmovlps, Mq,Vq),
                    dec66:Instr(Opcode.vmovlpd, Mq,Vq)),

                new PrefixedDecoder(
                    Instr(Opcode.unpcklps, Vx,Hx,Wx),
                    Instr(Opcode.unpcklpd, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Opcode.unpckhps, Vx,Hx,Wx),
                    Instr(Opcode.unpckhpd, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Opcode.movlhps, Vx,Wx),
                    Instr(Opcode.movhpd, Vx,Wx),
                    Instr(Opcode.movshdup, Vx,Wx)),
                new PrefixedDecoder(
                    Instr(Opcode.movhps, Mq,Vq),
                    Instr(Opcode.movhpd, Mq,Vq)),

                new GroupDecoder(16),
                Instr(Opcode.nop, InstrClass.Linear|InstrClass.Padding, Ev),
                Instr(Opcode.nop, InstrClass.Linear|InstrClass.Padding, Ev),
                Instr(Opcode.nop, InstrClass.Linear|InstrClass.Padding, Ev),
                Instr(Opcode.cldemote, Eb),
                Instr(Opcode.nop, InstrClass.Linear|InstrClass.Padding, Ev),
                Instr(Opcode.nop, InstrClass.Linear|InstrClass.Padding, Ev),
                Instr(Opcode.nop, InstrClass.Linear|InstrClass.Padding, Ev),

				// 0F 20
				Instr(Opcode.mov, Rv,Cd),
                Instr(Opcode.mov, Rv,Dd),
                Instr(Opcode.mov, Cd,Rv),
                Instr(Opcode.mov, Dd,Rv),
				s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,

                new PrefixedDecoder(
                    dec:  Instr(Opcode.movaps, Vps,Wps),
                    dec66:Instr(Opcode.movapd, Vpd,Wpd)),
                new PrefixedDecoder(
                    dec:  Instr(Opcode.movaps, Wps,Vps),
                    dec66:Instr(Opcode.movapd, Wpd,Vpd)),
                new PrefixedDecoder(
                    dec:  Instr(Opcode.cvtpi2ps, Vps,Qpi),
                    dec66:Instr(Opcode.cvtpi2pd, Vpd,Qpi),
                    decF3:Instr(Opcode.cvtsi2ss, Vss,Hss,Ey),
                    decF2:Instr(Opcode.cvtsi2sd, Vsd,Hsd,Ey)),
                new PrefixedDecoder(
                    dec:  Instr(Opcode.movntps, Mps,Vps),
                    dec66:Instr(Opcode.movntpd, Mpd,Vpd)),
                new PrefixedDecoder(
                    dec:  Instr(Opcode.cvttps2pi, Ppi,Wps),
                    dec66:Instr(Opcode.cvttpd2pi, Ppi,Wpq),
                    decF3:Instr(Opcode.cvttss2si, Gd,Wss),
                    decF2:Instr(Opcode.cvttsd2si, Gd,Wsd)),
                new PrefixedDecoder(
                    dec:  Instr(Opcode.cvtps2pi, Ppi,Wps),
                    dec66:Instr(Opcode.cvtpd2si, Qpi,Wpd),
                    decF3:Instr(Opcode.cvtss2si, Gy,Wss),
                    decF2:Instr(Opcode.cvtsd2si, Gy,Wsd)),
                new PrefixedDecoder(
                    dec:  Instr(Opcode.ucomiss, Vss,Wss),
                    dec66:Instr(Opcode.ucomisd, Vsd,Wsd)),
                new PrefixedDecoder(
                    dec:  Instr(Opcode.comiss, Vss,Wss),
                    dec66:Instr(Opcode.comisd, Vsd,Wsd)),

				// 0F 30
				Instr(Opcode.wrmsr, InstrClass.System),
                Instr(Opcode.rdtsc),
                Instr(Opcode.rdmsr, InstrClass.System),
                Instr(Opcode.rdpmc),
                Instr(Opcode.sysenter),
                Instr(Opcode.sysexit, InstrClass.Transfer),
                s_invalid,
                Instr(Opcode.getsec, InstrClass.System),

                new ThreeByteOpRec(), // 0F 38
                s_invalid,
                new ThreeByteOpRec(), // 0F 3A
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,

				// 0F 40
				Instr(Opcode.cmovo,  InstrClass.Linear|InstrClass.Conditional, Gv,Ev),
                Instr(Opcode.cmovno, InstrClass.Linear|InstrClass.Conditional, Gv,Ev),
                Instr(Opcode.cmovc,  InstrClass.Linear|InstrClass.Conditional, Gv,Ev),
                Instr(Opcode.cmovnc, InstrClass.Linear|InstrClass.Conditional, Gv,Ev),
                Instr(Opcode.cmovz,  InstrClass.Linear|InstrClass.Conditional, Gv,Ev),
                Instr(Opcode.cmovnz, InstrClass.Linear|InstrClass.Conditional, Gv,Ev),
                Instr(Opcode.cmovbe, InstrClass.Linear|InstrClass.Conditional, Gv,Ev),
                Instr(Opcode.cmova,  InstrClass.Linear|InstrClass.Conditional, Gv,Ev),

                Instr(Opcode.cmovs,  InstrClass.Linear|InstrClass.Conditional, Gv,Ev),
                Instr(Opcode.cmovns, InstrClass.Linear|InstrClass.Conditional, Gv,Ev),
                Instr(Opcode.cmovpe, InstrClass.Linear|InstrClass.Conditional, Gv,Ev),
                Instr(Opcode.cmovpo, InstrClass.Linear|InstrClass.Conditional, Gv,Ev),
                Instr(Opcode.cmovl,  InstrClass.Linear|InstrClass.Conditional, Gv,Ev),
                Instr(Opcode.cmovge, InstrClass.Linear|InstrClass.Conditional, Gv,Ev),
                Instr(Opcode.cmovle, InstrClass.Linear|InstrClass.Conditional, Gv,Ev),
                Instr(Opcode.cmovg,  InstrClass.Linear|InstrClass.Conditional, Gv,Ev),

				// 0F 50
                new PrefixedDecoder(
                    Instr(Opcode.movmskps, Gy,Ups),
                    Instr(Opcode.movmskpd, Gy,Upd)),
                new PrefixedDecoder(
                    dec:  Instr(Opcode.sqrtps, Vps,Wps),
                    dec66:Instr(Opcode.sqrtpd, Vpd,Wpd),
                    decF3:Instr(Opcode.sqrtss, Vss,Hss,Wss),
                    decF2:Instr(Opcode.sqrtsd, Vsd,Hsd,Wsd)),
                new PrefixedDecoder(
                    dec:  Instr(Opcode.rsqrtps, Vps,Wps),
                    dec66:s_invalid,
                    decF3:Instr(Opcode.rsqrtss, Vss,Hss,Wss),
                    decF2:s_invalid),
                new PrefixedDecoder(
                    dec:  Instr(Opcode.rcpps, Vps,Wps),
                    dec66:s_invalid,
                    decF3:Instr(Opcode.rcpss, Vss,Hss,Wss),
                    decF2:s_invalid),
                new PrefixedDecoder(
                    dec: Instr(Opcode.andps, Vps,Hps,Wps),
                    dec66:Instr(Opcode.andpd, Vpd,Hpd,Wpd)),
                new PrefixedDecoder(
                    dec:  Instr(Opcode.andnps, Vps,Hps,Wps),
                    dec66:Instr(Opcode.andnpd, Vpd,Hpd,Wpd)),
                new PrefixedDecoder(
                    dec:  Instr(Opcode.orps, Vps,Hps,Wps),
                    dec66:Instr(Opcode.orpd, Vpd,Hpd,Wpd)),
                new PrefixedDecoder(
                    dec:  Instr(Opcode.xorps, Vps,Hps,Wps),
                    dec66:Instr(Opcode.xorpd, Vpd,Hpd,Wpd)),

                new PrefixedDecoder(
                    dec:  Instr(Opcode.addps, Vps,Hps,Wps),
                    dec66:Instr(Opcode.addpd, Vpd,Hpd,Wpd),
                    decF3:Instr(Opcode.addss, Vss,Hss,Wss),
                    decF2:Instr(Opcode.addsd, Vsd,Hsd,Wsd)),
                new PrefixedDecoder(
                    dec:  Instr(Opcode.mulps, Vps,Hps,Wps),
                    dec66:Instr(Opcode.mulpd, Vpd,Hpd,Wpd),
                    decF3:Instr(Opcode.mulss, Vss,Hss,Wss),
                    decF2:Instr(Opcode.mulsd, Vsd,Hsd,Wsd)),
				new PrefixedDecoder(
                    dec:  Instr(Opcode.cvtps2pd, Vpd,Wps),
                    dec66:Instr(Opcode.cvtpd2ps, Vps,Wpd),
                    decF3:Instr(Opcode.cvtss2sd, Vsd,Hx,Wss),
                    decF2:Instr(Opcode.cvtsd2ss, Vss,Hx,Wsd)),
                new PrefixedDecoder(
                    dec:  Instr(Opcode.cvtdq2ps, Vps,Wdq),
                    dec66:Instr(Opcode.cvtps2dq, Vdq,Wps),
                    decF3:Instr(Opcode.cvttps2dq, Vdq,Wps),
                    decF2:s_invalid),
                new PrefixedDecoder(
                    dec:  Instr(Opcode.subps, Vps,Hps,Wps),
                    dec66:Instr(Opcode.subpd, Vpd,Hpd,Wpd),
                    decF3:Instr(Opcode.subss, Vss,Hss,Wss),
                    decF2:Instr(Opcode.subsd, Vsd,Hsd,Wsd)),
                new PrefixedDecoder(
                    dec:  Instr(Opcode.minps, Vps,Hps,Wps),
                    dec66:Instr(Opcode.minpd, Vpd,Hpd,Wpd),
                    decF3:Instr(Opcode.minss, Vss,Hss,Wss),
                    decF2:Instr(Opcode.minsd, Vsd,Hsd,Wsd)),
                new PrefixedDecoder(
                    dec:  Instr(Opcode.divps, Vps,Hps,Wps),
                    dec66:Instr(Opcode.divpd, Vpd,Hpd,Wpd),
                    decF3:Instr(Opcode.divss, Vss,Hss,Wss),
                    decF2:Instr(Opcode.divsd, Vsd,Hsd,Wsd)),
                new PrefixedDecoder(
                    dec:  Instr(Opcode.maxps, Vps,Hps,Wps),
                    dec66:Instr(Opcode.maxpd, Vpd,Hpd,Wpd),
                    decF3:Instr(Opcode.maxss, Vss,Hss,Wss),
                    decF2:Instr(Opcode.maxsd, Vsd,Hsd,Wsd)),
					
				// 0F 60
				new PrefixedDecoder(
                    Instr(Opcode.punpcklbw, Pq,Qd),
                    dec66:Instr(Opcode.punpcklbw, Vx,Wx)),
				new PrefixedDecoder(
                    Instr(Opcode.punpcklwd, Pq,Qd),
                    Instr(Opcode.punpcklwd, Vx,Wx)),
                new PrefixedDecoder(
                    Instr(Opcode.punpckldq, Pq,Qd),
                    Instr(Opcode.punpckldq, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Opcode.packsswb, Pq,Qd),
                    Instr(Opcode.vpacksswb, Vx,Hx,Wx)),

                new PrefixedDecoder(
                    Instr(Opcode.pcmpgtb, Pq,Qd),
                    Instr(Opcode.pcmpgtb, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Opcode.pcmpgtw, Pq,Qd),
                    Instr(Opcode.pcmpgtw, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Opcode.pcmpgtd, Pq,Qd),
                    Instr(Opcode.pcmpgtd, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Opcode.packuswb, Pq,Qd),
                    Instr(Opcode.vpunpckhbw, Vx,Hx,Wx)),

                new PrefixedDecoder(
                    Instr(Opcode.punpckhbw, Pq,Qd),
                    Instr(Opcode.vpunpckhbw, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Opcode.punpckhwd, Pq,Qd),
                    Instr(Opcode.vpunpckhwd, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Opcode.punpckhdq, Pq,Qd),
                    Instr(Opcode.vpunpckhdq, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Opcode.packssdw, Pq,Qd),
                    Instr(Opcode.vpackssdw, Vx,Hx,Wx)),

                 new PrefixedDecoder(
                    s_invalid,
                    Instr(Opcode.vpunpcklqdq, Vx,Hx,Wx)),
                 new PrefixedDecoder(
                    s_invalid,
                    Instr(Opcode.vpunpckhqdq, Vx,Hx,Wx)),
                Instr(Opcode.movd, Vy,Ey),
				Instr(Opcode.movdqa, Vx,Wx),

				// 0F 70
				new PrefixedDecoder(
                    Instr(Opcode.pshufw, Pq,Qq,Ib),
                    dec66:Instr(Opcode.pshufd, Vx,Wx,Ib),
                    decF3:Instr(Opcode.pshufhw, Vx,Wx,Ib),
                    decF2:Instr(Opcode.pshuflw, Vx,Wx,Ib)),
                new GroupDecoder(12),
                new GroupDecoder(13),
                new GroupDecoder(14),

				new PrefixedDecoder(
                    Instr(Opcode.pcmpeqb, Pq,Qq),
                    dec66:Instr(Opcode.pcmpeqb, Vx,Wx)),
                new PrefixedDecoder(
                    Instr(Opcode.pcmpeqw, Pq,Qq),
                    dec66:Instr(Opcode.pcmpeqw, Vx,Wx)),
                new PrefixedDecoder(
                    Instr(Opcode.pcmpeqd, Pq,Qq),
                    dec66:Instr(Opcode.pcmpeqd, Vx,Wx)),
                Instr(Opcode.emms, InstrClass.System),

				Instr(Opcode.vmread, Ey,Gy),
				Instr(Opcode.vmwrite, Gy,Ey),
				s_invalid,
				s_invalid,

                new PrefixedDecoder(
                    dec:s_invalid,
                    dec66:Instr(Opcode.vhaddpd, Vpd,Hpd,Wpd),
                    decF2:Instr(Opcode.vhaddps, Vps,Hps,Wps)),
                new PrefixedDecoder(
                    dec:s_invalid,
                    dec66:Instr(Opcode.vhsubpd, Vpd,Hpd,Wpd),
                    decF2:Instr(Opcode.vhsubps, Vps,Hps,Wps)),
                new PrefixedDecoder(
                    dec: Instr(Opcode.movd, Ey,Pd), decWide: Instr(Opcode.movq, Ey,Pd),
                    dec66: Instr(Opcode.movd, Ey,Vy), dec66Wide: Instr(Opcode.movq, Ey,Vy),
                    decF3: Instr(Opcode.movq, Vy,Wy)),
				new PrefixedDecoder(
                    dec:Instr(Opcode.movq, Qq,Pq),
                    dec66:Instr(Opcode.movdqa, Wx,Vx),
                    decF3:Instr(Opcode.movdqu, Wx,Vx)),

				// 0F 80
				Instr(Opcode.jo,	InstrClass.ConditionalTransfer, Jv),
				Instr(Opcode.jno, InstrClass.ConditionalTransfer, Jv),
				Instr(Opcode.jc,	InstrClass.ConditionalTransfer, Jv),
				Instr(Opcode.jnc,	InstrClass.ConditionalTransfer, Jv),
				Instr(Opcode.jz,	InstrClass.ConditionalTransfer, Jv),
				Instr(Opcode.jnz, InstrClass.ConditionalTransfer, Jv),
				Instr(Opcode.jbe, InstrClass.ConditionalTransfer, Jv),
				Instr(Opcode.ja,  InstrClass.ConditionalTransfer, Jv),

				Instr(Opcode.js,  InstrClass.ConditionalTransfer, Jv),
				Instr(Opcode.jns, InstrClass.ConditionalTransfer, Jv),
				Instr(Opcode.jpe, InstrClass.ConditionalTransfer, Jv),
				Instr(Opcode.jpo, InstrClass.ConditionalTransfer, Jv),
				Instr(Opcode.jl,  InstrClass.ConditionalTransfer, Jv),
				Instr(Opcode.jge, InstrClass.ConditionalTransfer, Jv),
				Instr(Opcode.jle, InstrClass.ConditionalTransfer, Jv),
				Instr(Opcode.jg,  InstrClass.ConditionalTransfer, Jv),

				// 0F 90
				Instr(Opcode.seto, Eb),
				Instr(Opcode.setno,Eb),
				Instr(Opcode.setc, Eb),
				Instr(Opcode.setnc,Eb),
				Instr(Opcode.setz, Eb),
				Instr(Opcode.setnz,Eb),
				Instr(Opcode.setbe,Eb),
				Instr(Opcode.seta, Eb),

				Instr(Opcode.sets,  Eb),
				Instr(Opcode.setns, Eb),
				Instr(Opcode.setpe, Eb),
				Instr(Opcode.setpo, Eb),
				Instr(Opcode.setl,  Eb),
				Instr(Opcode.setge, Eb),
				Instr(Opcode.setle, Eb),
				Instr(Opcode.setg,  Eb),

				// 0F A0
				Instr(Opcode.push, s4),
				Instr(Opcode.pop, s4),
				Instr(Opcode.cpuid),
				Instr(Opcode.bt, Ev,Gv),
				Instr(Opcode.shld, Ev,Gv,Ib),
				Instr(Opcode.shld, Ev,Gv,c),
				s_invalid,
                s_invalid,

				Instr(Opcode.push, s5),
				Instr(Opcode.pop, s5),
				Instr(Opcode.rsm),
                Instr(Opcode.bts, Ev,Gv),
				Instr(Opcode.shrd, Ev,Gv,Ib),
				Instr(Opcode.shrd, Ev,Gv,c),
				new GroupDecoder(15),
				Instr(Opcode.imul, Gv,Ev),

				// 0F B0
				Instr(Opcode.cmpxchg, Eb,Gb),
				Instr(Opcode.cmpxchg, Ev,Gv),
				Instr(Opcode.lss, Gv,Mp),
				Instr(Opcode.btr, Ev,Gv),
                Instr(Opcode.lfs, Gv,Mp),
				Instr(Opcode.lgs, Gv,Mp),
				Instr(Opcode.movzx, Gv,Eb),
				Instr(Opcode.movzx, Gv,Ew),

				new PrefixedDecoder(
                    dec:Instr(Opcode.jmpe),
                    decF3:Instr(Opcode.popcnt, Gv,Ev)),
				Instr(Opcode.ud1, InstrClass.Invalid, Gv,Ev),
				new GroupDecoder(8, Ev,Ib),
				Instr(Opcode.btc, Gv,Ev),

                new PrefixedDecoder(
                    dec: Instr(Opcode.bsf, Gv,Ev),
                    decF3: Instr(Opcode.tzcnt, Gv,Ev)),
                new PrefixedDecoder(
                    dec: Instr(Opcode.bsr, Gv,Ev),
                    dec66: Instr(Opcode.bsr, Gv,Ev),
                    decF3: Instr(Opcode.lzcnt, Gv,Ev)),
				Instr(Opcode.movsx, Gv,Eb),
				Instr(Opcode.movsx, Gv,Ew),

				// 0F C0
				Instr(Opcode.xadd, Eb,Gb),
				Instr(Opcode.xadd, Ev,Gv),
				new PrefixedDecoder(
                    dec:  Instr(Opcode.cmpps, Vps,Hps,Wps,Ib),
                    dec66:Instr(Opcode.cmppd, Vpd,Hpd,Wpd,Ib),
                    decF3:Instr(Opcode.cmpss, Vss,Hss,Wss,Ib),
                    decF2:Instr(Opcode.cmpsd, Vpd,Hpd,Wpd,Ib)),
				new PrefixedDecoder(
                    Instr(Opcode.movnti, My,Gy),
                    s_invalid),
                new PrefixedDecoder(
                    Instr(Opcode.pinsrw, Pq,Ry),     //$TODO: encoding is weird.
                    Instr(Opcode.vpinsrw, Vdq,Hdq,Ry)),
                new PrefixedDecoder(
                    Instr(Opcode.pextrw, Gd,Nq,Ib),
                    Instr(Opcode.vextrw, Gd,Udq,Ib)),
                new PrefixedDecoder(
                    Instr(Opcode.vshufps, Vps,Hps,Wps,Ib),
                    Instr(Opcode.vshufpd, Vpd,Hpd,Wpd,Ib)),
				new GroupDecoder(9),

				Instr(Opcode.bswap, rv),
				Instr(Opcode.bswap, rv),
				Instr(Opcode.bswap, rv),
				Instr(Opcode.bswap, rv),
				Instr(Opcode.bswap, rv),
				Instr(Opcode.bswap, rv),
				Instr(Opcode.bswap, rv),
				Instr(Opcode.bswap, rv),

				// 0F D0
				new PrefixedDecoder(
                    dec:  s_invalid,
					dec66:Instr(Opcode.addsubpd, Vpd,Hpd,Wpd),
					decF3:s_invalid,
					decF2:Instr(Opcode.addsubps, Vps,Hps,Wps)),
				new PrefixedDecoder(
                    Instr(Opcode.psrlw, Pq,Qq),
                    Instr(Opcode.vpsrlw, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Opcode.psrld, Pq,Qq),
                    Instr(Opcode.vpsrld, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Opcode.psrlq, Pq,Qq),
                    Instr(Opcode.vpmullw, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Opcode.paddq, Pq,Qq),
                    Instr(Opcode.vpaddq, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Opcode.pmullw, Pq,Qq),
                    Instr(Opcode.vpmullw, Vx,Hx,Wx)),
				Instr(Opcode.movq, Wx,Vx),
                new PrefixedDecoder(
                    Instr(Opcode.pmovmskb, Gd,Nq),
                    Instr(Opcode.vpmovmskb, Gd,Ux)),

                new PrefixedDecoder(
                    Instr(Opcode.psubusb, Pq,Qq),
                    Instr(Opcode.vpsubusb, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Opcode.psubusw, Pq,Qq),
                    Instr(Opcode.vpsubusw, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Opcode.pminub, Pq,Qq),
                    Instr(Opcode.vpminub, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Opcode.pand, Pq,Qq),
                    Instr(Opcode.vpand, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Opcode.paddusb, Pq,Qq),
                    Instr(Opcode.vpaddusb, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Opcode.paddusw, Pq,Qq),
                    Instr(Opcode.vpaddusw, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Opcode.pmaxub, Pq,Qq),
                    Instr(Opcode.vpmaxub, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Opcode.pandn, Pq,Qq),
                    Instr(Opcode.vpandn, Vx,Hx,Wx)),

				// 0F E0
                new PrefixedDecoder(
                    Instr(Opcode.pavgb, Pq,Qq),
                    Instr(Opcode.vpavgb, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Opcode.psraw, Pq,Qq),
                    Instr(Opcode.vpsraw, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Opcode.psrad, Pq,Qq),
                    Instr(Opcode.vpsrad, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Opcode.pavgw, Pq,Qq),
                    Instr(Opcode.vpavgw, Vx,Hx,Wx)),

                new PrefixedDecoder(
                    Instr(Opcode.pmulhuw, Pq,Qq),
                    Instr(Opcode.vpmulhuw, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Opcode.pmulhw, Pq,Qq),
                    Instr(Opcode.vpmulhw, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    dec:  s_invalid,
                    dec66:Instr(Opcode.cvttpd2dq, Vdq,Wpd),
                    decF3:Instr(Opcode.cvtdq2pd, Vx,Wpd),
                    decF2:Instr(Opcode.cvtpd2dq, Vdq,Wpd)),
                new PrefixedDecoder(
                    Instr(Opcode.movntq, Mq,Pq),
                    Instr(Opcode.vmovntq, Mx,Vx)),

                new PrefixedDecoder(
                    Instr(Opcode.psubsb, Pq,Qq),
                    Instr(Opcode.vpsubsb, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Opcode.psubsw, Pq,Qq),
                    Instr(Opcode.vpsubsw, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Opcode.pminsw, Pq,Qq),
                    Instr(Opcode.vpminsw, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Opcode.por, Pq,Qq),
                    Instr(Opcode.vpor, Vx,Hx,Wx)),

                new PrefixedDecoder(
                    Instr(Opcode.paddsb, Pq,Qq),
                    Instr(Opcode.vpaddsb, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Opcode.paddsw, Pq,Qq),
                    Instr(Opcode.vpaddsw, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Opcode.pmaxsw, Pq,Qq),
                    Instr(Opcode.vpmaxsw, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Opcode.pxor, Pq,Qq),
                    Instr(Opcode.vpxor, Vx,Hx,Wx)),

				// 0F F0
                new PrefixedDecoder(
                    s_invalid,
                    decF2:Instr(Opcode.vlddqu, Vx,Mx)),
                new PrefixedDecoder(
                    Instr(Opcode.psllw, Pq,Qq),
                    dec66:Instr(Opcode.vpsllw, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Opcode.pslld, Pq,Qq),
                    dec66:Instr(Opcode.vpslld, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Opcode.psllq, Pq,Qq),
                    dec66:Instr(Opcode.vpsllq, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Opcode.pmuludq, Pq,Qq),
                    dec66:Instr(Opcode.vpmuludq, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Opcode.pmaddwd, Pq,Qq),
                    dec66:Instr(Opcode.vpmaddwd, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Opcode.psadbw, Pq,Qq),
                    dec66:Instr(Opcode.vpsadbw, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Opcode.maskmovq, Pq,Qq),
                    dec66:Instr(Opcode.vmaskmovdqu, Vdq,Udq)),

                new PrefixedDecoder(
                    Instr(Opcode.psubb, Pq,Qq),
                    Instr(Opcode.vpsubb, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Opcode.psubw, Pq,Qq),
                    Instr(Opcode.vpsubw, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Opcode.psubd, Pq,Qq),
                    Instr(Opcode.vpsubd, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Opcode.psubq, Pq,Qq),
                    Instr(Opcode.vpsubq, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Opcode.paddb, Pq,Qq),
                    Instr(Opcode.vpaddb, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Opcode.paddw, Pq,Qq),
                    Instr(Opcode.vpaddw, Vx,Hx,Wx)),
                new PrefixedDecoder(
                    Instr(Opcode.paddd, Pq,Qq),
                    Instr(Opcode.vpaddd, Vx,Hx,Wx)),
				Instr(Opcode.ud0, InstrClass.Invalid,  Gv,Ev),
			};
        }
    }
}
