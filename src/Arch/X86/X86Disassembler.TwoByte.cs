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
				new GroupDecoder(6, ""),
                new GroupDecoder(7, ""),
                Instr(Opcode.lar, InstrClass.System, "Gv,Ew"),
                Instr(Opcode.lsl, InstrClass.System, "Gv,Ew"),
                s_invalid,
                new Alternative64Decoder(
                    s_invalid,
                    Instr(Opcode.syscall, InstrClass.Transfer|InstrClass.Call, "")),
                Instr(Opcode.clts),
                new Alternative64Decoder(
                    s_invalid,
                    Instr(Opcode.sysret, InstrClass.Transfer, "")),

                Instr(Opcode.invd, InstrClass.System, ""),
                Instr(Opcode.wbinvd, InstrClass.System, ""),
                s_invalid,
                Instr(Opcode.ud2),
                s_invalid,
                Instr(Opcode.prefetchw, "Ev"),
                Instr(Opcode.femms),    // AMD-specific
                s_invalid, // nyi("AMD 3D-Now instructions"), //$TODO: this requires adding separate processor model for AMD

				// 0F 10
				new PrefixedDecoder(
                    Instr(Opcode.movups, "Vps,Wps"),
                    Instr(Opcode.movups, "Vps,Wps"),
                    Instr(Opcode.movupd, "Vpd,Wpd"),
                    Instr(Opcode.movupd, "Vpd,Wpd"),
                    Instr(Opcode.movss,  "Vx,Wss"),
                    Instr(Opcode.movsd,  "Vx,Wsd")),
                new PrefixedDecoder(
                    Opcode.movups, "Wps,Vps",
                    Opcode.movupd, "Wpd,Vpd",
                    Opcode.movss,  "Wss,Vss",
                    Opcode.movsd,  "Wsd,Vsd"),
                new PrefixedDecoder(
                    Opcode.movlps,   "Vq,Hq,Mq",
                    Opcode.movlpd,   "Vq,Hq,Mq",
                    Opcode.movsldup, "Vx,Wx",
                    Opcode.movddup,  "Vx,Wx"),
                new PrefixedDecoder(
					Opcode.vmovlps, "Mq,Vq",
					Opcode.vmovlpd, "Mq,Vq"),

                new PrefixedDecoder(
                    Opcode.unpcklps, "Vx,Hx,Wx",
                    Opcode.unpcklpd, "Vx,Hx,Wx"),
                new PrefixedDecoder(
                    Opcode.unpckhps, "Vx,Hx,Wx",
                    Opcode.unpckhpd, "Vx,Hx,Wx"),
                new PrefixedDecoder(
                    Opcode.movlhps, "Vx,Wx",
                    Opcode.movhpd, "Vx,Wx",
                    Opcode.movshdup, "Vx,Wx"),
                new PrefixedDecoder(
                    Opcode.movhps, "Mq,Vq",
                    Opcode.movhpd, "Mq,Vq"),

                new GroupDecoder(16, ""),
                Instr(Opcode.nop, InstrClass.Linear|InstrClass.Padding, "Ev"),
                Instr(Opcode.nop, InstrClass.Linear|InstrClass.Padding, "Ev"),
                Instr(Opcode.nop, InstrClass.Linear|InstrClass.Padding, "Ev"),
                Instr(Opcode.cldemote, "Eb"),
                Instr(Opcode.nop, InstrClass.Linear|InstrClass.Padding, "Ev"),
                Instr(Opcode.nop, InstrClass.Linear|InstrClass.Padding, "Ev"),
                Instr(Opcode.nop, InstrClass.Linear|InstrClass.Padding, "Ev"),

				// 0F 20
				Instr(Opcode.mov, "Rv,Cd"),
                Instr(Opcode.mov, "Rv,Dd"),
                Instr(Opcode.mov, "Cd,Rv"),
                Instr(Opcode.mov, "Dd,Rv"),
				s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,

                new PrefixedDecoder(
                    Opcode.movaps, "Vps,Wps",
                    Opcode.movapd, "Vpd,Wpd"),
                new PrefixedDecoder(
                    Opcode.movaps, "Wps,Vps",
                    Opcode.movapd, "Wpd,Vpd"),
                new PrefixedDecoder(
                    Opcode.cvtpi2ps, "Vps,Qpi",
                    Opcode.cvtpi2pd, "Vpd,Qpi",
                    Opcode.cvtsi2ss, "Vss,Hss,Ey",
                    Opcode.cvtsi2sd, "Vsd,Hsd,Ey"),
                new PrefixedDecoder(
                    Opcode.movntps, "Mps,Vps",
                    Opcode.movntpd, "Mpd,Vpd"),
                new PrefixedDecoder(
                    Opcode.cvttps2pi, "Ppi,Wps",
                    Opcode.cvttpd2pi, "Ppi,Wpq",
                    Opcode.cvttss2si, "Gd,Wss",
                    Opcode.cvttsd2si, "Gd,Wsd"),
                new PrefixedDecoder(
                    Opcode.cvtps2pi, "Ppi,Wps",
                    Opcode.cvtpd2si, "Qpi,Wpd",
                    Opcode.cvtss2si, "Gy,Wss",
                    Opcode.cvtsd2si, "Gy,Wsd"),
                new PrefixedDecoder(
                    Opcode.ucomiss, "Vss,Wss",
                    Opcode.ucomisd, "Vsd,Wsd"),
                new PrefixedDecoder(
                    Opcode.comiss, "Vss,Wss",
                    Opcode.comisd, "Vsd,Wsd"),

				// 0F 30
				Instr(Opcode.wrmsr, InstrClass.System, ""),
                Instr(Opcode.rdtsc),
                Instr(Opcode.rdmsr, InstrClass.System, ""),
                Instr(Opcode.rdpmc),
                Instr(Opcode.sysenter),
                Instr(Opcode.sysexit, InstrClass.Transfer, ""),
                s_invalid,
                Instr(Opcode.getsec, InstrClass.System, ""),

                new ThreeByteOpRec(), // 0F 38
                s_invalid,
                new ThreeByteOpRec(), // 0F 3A
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,

				// 0F 40
				Instr(Opcode.cmovo,  InstrClass.Linear|InstrClass.Conditional, "Gv,Ev"),
                Instr(Opcode.cmovno, InstrClass.Linear|InstrClass.Conditional, "Gv,Ev"),
                Instr(Opcode.cmovc,  InstrClass.Linear|InstrClass.Conditional, "Gv,Ev"),
                Instr(Opcode.cmovnc, InstrClass.Linear|InstrClass.Conditional, "Gv,Ev"),
                Instr(Opcode.cmovz,  InstrClass.Linear|InstrClass.Conditional, "Gv,Ev"),
                Instr(Opcode.cmovnz, InstrClass.Linear|InstrClass.Conditional, "Gv,Ev"),
                Instr(Opcode.cmovbe, InstrClass.Linear|InstrClass.Conditional, "Gv,Ev"),
                Instr(Opcode.cmova,  InstrClass.Linear|InstrClass.Conditional, "Gv,Ev"),

                Instr(Opcode.cmovs,  InstrClass.Linear|InstrClass.Conditional, "Gv,Ev"),
                Instr(Opcode.cmovns, InstrClass.Linear|InstrClass.Conditional, "Gv,Ev"),
                Instr(Opcode.cmovpe, InstrClass.Linear|InstrClass.Conditional, "Gv,Ev"),
                Instr(Opcode.cmovpo, InstrClass.Linear|InstrClass.Conditional, "Gv,Ev"),
                Instr(Opcode.cmovl,  InstrClass.Linear|InstrClass.Conditional, "Gv,Ev"),
                Instr(Opcode.cmovge, InstrClass.Linear|InstrClass.Conditional, "Gv,Ev"),
                Instr(Opcode.cmovle, InstrClass.Linear|InstrClass.Conditional, "Gv,Ev"),
                Instr(Opcode.cmovg,  InstrClass.Linear|InstrClass.Conditional, "Gv,Ev"),

				// 0F 50
                new PrefixedDecoder(
                    Opcode.movmskps, "Gy,Ups",
                    Opcode.movmskpd, "Gy,Upd"),
                new PrefixedDecoder(
                    Opcode.sqrtps, "Vps,Wps",
                    Opcode.sqrtpd, "Vpd,Wpd",
                    Opcode.sqrtss, "Vss,Hss,Wss",
                    Opcode.sqrtsd, "Vsd,Hsd,Wsd"),
                new PrefixedDecoder(
                    Opcode.rsqrtps, "Vps,Wps",
                    Opcode.illegal, "",
                    Opcode.rsqrtss, "Vss,Hss,Wss",
                    Opcode.illegal, ""),
                new PrefixedDecoder(
                    Opcode.rcpps, "Vps,Wps",
                    Opcode.illegal, "",
                    Opcode.rcpss, "Vss,Hss,Wss",
                    Opcode.illegal, ""),
                new PrefixedDecoder(
                    Opcode.andps, "Vps,Hps,Wps",
                    Opcode.andpd, "Vpd,Hpd,Wpd"),
                new PrefixedDecoder(
                    Opcode.andnps, "Vps,Hps,Wps",
                    Opcode.andnpd, "Vpd,Hpd,Wpd"),
                new PrefixedDecoder(
                    Opcode.orps, "Vps,Hps,Wps",
                    Opcode.orpd, "Vpd,Hpd,Wpd"),
                new PrefixedDecoder(
                    Opcode.xorps, "Vps,Hps,Wps",
                    Opcode.xorpd, "Vpd,Hpd,Wpd"),

                new PrefixedDecoder(
                    Opcode.addps, "Vps,Hps,Wps",
                    Opcode.addpd, "Vpd,Hpd,Wpd",
                    Opcode.addss, "Vss,Hss,Wss",
                    Opcode.addsd, "Vsd,Hsd,Wsd"),
                new PrefixedDecoder(
                    Opcode.mulps, "Vps,Hps,Wps",
                    Opcode.mulpd, "Vpd,Hpd,Wpd",
                    Opcode.mulss, "Vss,Hss,Wss",
                    Opcode.mulsd, "Vsd,Hsd,Wsd"),
				new PrefixedDecoder(
                    Opcode.cvtps2pd, "Vpd,Wps",
                    Opcode.cvtpd2ps, "Vps,Wpd",
                    Opcode.cvtss2sd, "Vsd,Hx,Wss",
                    Opcode.cvtsd2ss, "Vss,Hx,Wsd"),
                new PrefixedDecoder(
                    Opcode.cvtdq2ps, "Vps,Wdq",
                    Opcode.cvtps2dq, "Vdq,Wps",
                    Opcode.cvttps2dq, "Vdq,Wps",
                    Opcode.illegal, ""),
                new PrefixedDecoder(
                    Opcode.subps, "Vps,Hps,Wps",
                    Opcode.subpd, "Vpd,Hpd,Wpd",
                    Opcode.subss, "Vss,Hss,Wss",
                    Opcode.subsd, "Vsd,Hsd,Wsd"),
                new PrefixedDecoder(
                    Opcode.minps, "Vps,Hps,Wps",
                    Opcode.minpd, "Vpd,Hpd,Wpd",
                    Opcode.minss, "Vss,Hss,Wss",
                    Opcode.minsd, "Vsd,Hsd,Wsd"),
                new PrefixedDecoder(
                    Opcode.divps, "Vps,Hps,Wps",
                    Opcode.divpd, "Vpd,Hpd,Wpd",
                    Opcode.divss, "Vss,Hss,Wss",
                    Opcode.divsd, "Vsd,Hsd,Wsd"),
                new PrefixedDecoder(
                    Opcode.maxps, "Vps,Hps,Wps",
                    Opcode.maxpd, "Vpd,Hpd,Wpd",
                    Opcode.maxss, "Vss,Hss,Wss",
                    Opcode.maxsd, "Vsd,Hsd,Wsd"),
					
				// 0F 60
				new PrefixedDecoder(
                    Opcode.punpcklbw, "Pq,Qd",
                    Opcode.punpcklbw, "Vx,Wx"),
				new PrefixedDecoder(
                    Opcode.punpcklwd, "Pq,Qd",
                    Opcode.punpcklwd, "Vx,Wx"),
                new PrefixedDecoder(
                    Opcode.punpckldq, "Pq,Qd",
                    Opcode.punpckldq, "Vx,Hx,Wx"),
                new PrefixedDecoder(
                    Opcode.packsswb, "Pq,Qd",
                    Opcode.vpacksswb, "Vx,Hx,Wx"),

                new PrefixedDecoder(
                    Opcode.pcmpgtb, "Pq,Qd",
                    Opcode.pcmpgtb, "Vx,Hx,Wx"),
                new PrefixedDecoder(
                    Opcode.pcmpgtw, "Pq,Qd",
                    Opcode.pcmpgtw, "Vx,Hx,Wx"),
                new PrefixedDecoder(
                    Opcode.pcmpgtd, "Pq,Qd",
                    Opcode.pcmpgtd, "Vx,Hx,Wx"),
                new PrefixedDecoder(
                    Opcode.packuswb, "Pq,Qd",
                    Opcode.vpunpckhbw, "Vx,Hx,Wx"),

                new PrefixedDecoder(
                    Opcode.punpckhbw, "Pq,Qd",
                    Opcode.vpunpckhbw, "Vx,Hx,Wx"),
                new PrefixedDecoder(
                    Opcode.punpckhwd, "Pq,Qd",
                    Opcode.vpunpckhwd, "Vx,Hx,Wx"),
                new PrefixedDecoder(
                    Opcode.punpckhdq, "Pq,Qd",
                    Opcode.vpunpckhdq, "Vx,Hx,Wx"),
                new PrefixedDecoder(
                    Opcode.packssdw, "Pq,Qd",
                    Opcode.vpackssdw, "Vx,Hx,Wx"),

                 new PrefixedDecoder(
                    Opcode.illegal, "",
                    Opcode.vpunpcklqdq, "Vx,Hx,Wx"),
                 new PrefixedDecoder(
                    Opcode.illegal, "",
                    Opcode.vpunpckhqdq, "Vx,Hx,Wx"),
                Instr(Opcode.movd, "Vy,Ey"),
				Instr(Opcode.movdqa, "Vx,Wx"),

				// 0F 70
				new PrefixedDecoder(
                    Opcode.pshufw, "Pq,Qq,Ib",
                    Opcode.pshufd, "Vx,Wx,Ib",
                    Opcode.pshufhw, "Vx,Wx,Ib",
                    Opcode.pshuflw, "Vx,Wx,Ib"),
                new GroupDecoder(12, ""),
                new GroupDecoder(13, ""),
                new GroupDecoder(14, ""),

				new PrefixedDecoder(
                    Opcode.pcmpeqb, "Pq,Qq",
                    Opcode.pcmpeqb, "Vx,Wx"),
                new PrefixedDecoder(
                    Opcode.pcmpeqw, "Pq,Qq",
                    Opcode.pcmpeqw, "Vx,Wx"),
                new PrefixedDecoder(
                    Opcode.pcmpeqd, "Pq,Qq",
                    Opcode.pcmpeqd, "Vx,Wx"),
                Instr(Opcode.emms, InstrClass.System, ""),

				Instr(Opcode.vmread, "Ey,Gy"),
				Instr(Opcode.vmwrite, "Gy,Ey"),
				s_invalid,
				s_invalid,

                new PrefixedDecoder(
                    Opcode.illegal, "",
                    Opcode.vhaddpd, "Vpd,Hpd,Wpd",
                    opF2:Opcode.vhaddps, opF2Fmt:"Vps,Hps,Wps"),
                new PrefixedDecoder(
                    Opcode.illegal, "",
                    Opcode.vhsubpd, "Vpd,Hpd,Wpd",
                    opF2:Opcode.vhsubps, opF2Fmt:"Vps,Hps,Wps"),
                new PrefixedDecoder(
                    dec: Instr(Opcode.movd, "Ey,Pd"), decWide: Instr(Opcode.movq, "Ey,Pd"),
                    dec66: Instr(Opcode.movd, "Ey,Vy"), dec66Wide: Instr(Opcode.movq, "Ey,Vy"),
                    decF3: Instr(Opcode.movq, "Vy,Wy")),
				new PrefixedDecoder(
                    Opcode.movq, "Qq,Pq",
                    Opcode.movdqa, "Wx,Vx",
                    Opcode.movdqu, "Wx,Vx"),

				// 0F 80
				Instr(Opcode.jo,	InstrClass.ConditionalTransfer, "Jv"),
				Instr(Opcode.jno, InstrClass.ConditionalTransfer, "Jv"),
				Instr(Opcode.jc,	InstrClass.ConditionalTransfer, "Jv"),
				Instr(Opcode.jnc,	InstrClass.ConditionalTransfer, "Jv"),
				Instr(Opcode.jz,	InstrClass.ConditionalTransfer, "Jv"),
				Instr(Opcode.jnz, InstrClass.ConditionalTransfer, "Jv"),
				Instr(Opcode.jbe, InstrClass.ConditionalTransfer, "Jv"),
				Instr(Opcode.ja,  InstrClass.ConditionalTransfer, "Jv"),

				Instr(Opcode.js,  InstrClass.ConditionalTransfer, "Jv"),
				Instr(Opcode.jns, InstrClass.ConditionalTransfer, "Jv"),
				Instr(Opcode.jpe, InstrClass.ConditionalTransfer, "Jv"),
				Instr(Opcode.jpo, InstrClass.ConditionalTransfer, "Jv"),
				Instr(Opcode.jl,  InstrClass.ConditionalTransfer, "Jv"),
				Instr(Opcode.jge, InstrClass.ConditionalTransfer, "Jv"),
				Instr(Opcode.jle, InstrClass.ConditionalTransfer, "Jv"),
				Instr(Opcode.jg,  InstrClass.ConditionalTransfer, "Jv"),

				// 0F 90
				Instr(Opcode.seto, "Eb"),
				Instr(Opcode.setno,"Eb"),
				Instr(Opcode.setc, "Eb"),
				Instr(Opcode.setnc,"Eb"),
				Instr(Opcode.setz, "Eb"),
				Instr(Opcode.setnz,"Eb"),
				Instr(Opcode.setbe,"Eb"),
				Instr(Opcode.seta, "Eb"),

				Instr(Opcode.sets,  "Eb"),
				Instr(Opcode.setns, "Eb"),
				Instr(Opcode.setpe, "Eb"),
				Instr(Opcode.setpo, "Eb"),
				Instr(Opcode.setl,  "Eb"),
				Instr(Opcode.setge, "Eb"),
				Instr(Opcode.setle, "Eb"),
				Instr(Opcode.setg,  "Eb"),

				// 0F A0
				Instr(Opcode.push, "s4"),
				Instr(Opcode.pop, "s4"),
				Instr(Opcode.cpuid, ""),
				Instr(Opcode.bt, "Ev,Gv"),
				Instr(Opcode.shld, "Ev,Gv,Ib"),
				Instr(Opcode.shld, "Ev,Gv,c"),
				s_invalid,
                s_invalid,

				Instr(Opcode.push, "s5"),
				Instr(Opcode.pop, "s5"),
				Instr(Opcode.rsm, ""),
                Instr(Opcode.bts, "Ev,Gv"),
				Instr(Opcode.shrd, "Ev,Gv,Ib"),
				Instr(Opcode.shrd, "Ev,Gv,c"),
				new GroupDecoder(15, ""),
				Instr(Opcode.imul, "Gv,Ev"),

				// 0F B0
				Instr(Opcode.cmpxchg, "Eb,Gb"),
				Instr(Opcode.cmpxchg, "Ev,Gv"),
				Instr(Opcode.lss, "Gv,Mp"),
				Instr(Opcode.btr, "Ev,Gv"),
                Instr(Opcode.lfs, "Gv,Mp"),
				Instr(Opcode.lgs, "Gv,Mp"),
				Instr(Opcode.movzx, "Gv,Eb"),
				Instr(Opcode.movzx, "Gv,Ew"),

				new PrefixedDecoder(
                    Opcode.jmpe, "",
                    opF3:Opcode.popcnt, opF3Fmt: "Gv,Ev"),
				Instr(Opcode.ud1, "Gv,Ev"),
				new GroupDecoder(8, "Ev,Ib"),
				Instr(Opcode.btc, "Gv,Ev"),

                new PrefixedDecoder(
                    Opcode.bsf, "Gv,Ev",
                    opF3:Opcode.tzcnt, opF3Fmt:"Gv,Ev"),
                new PrefixedDecoder(
                    dec: Instr(Opcode.bsr, "Gv,Ev"),
                    dec66: Instr(Opcode.bsr, "Gv,Ev"),
                    decF3: Instr(Opcode.lzcnt, "Gv,Ev")),
				Instr(Opcode.movsx, "Gv,Eb"),
				Instr(Opcode.movsx, "Gv,Ew"),

				// 0F C0
				Instr(Opcode.xadd, "Eb,Gb"),
				Instr(Opcode.xadd, "Ev,Gv"),
				new PrefixedDecoder(
                    Opcode.cmpps, "Vps,Hps,Wps,Ib",
                    Opcode.cmppd, "Vpd,Hpd,Wpd,Ib",
                    Opcode.cmpss, "Vss,Hss,Wss,Ib",
                    Opcode.cmpsd, "Vpd,Hpd,Wpd,Ib"),
				new PrefixedDecoder(
                    Opcode.movnti, "My,Gy",
                    Opcode.illegal, ""),
                new PrefixedDecoder(
                    Opcode.pinsrw, "Pq,Ry",     //$TODO: encoding is weird.
                    Opcode.vpinsrw, "Vdq,Hdq,Ry"),
                new PrefixedDecoder(
                    Opcode.pextrw, "Gd,Nq,Ib",
                    Opcode.vextrw, "Gd,Udq,Ib"),
                new PrefixedDecoder(
                    Opcode.vshufps, "Vps,Hps,Wps,Ib",
                    Opcode.vshufpd, "Vpd,Hpd,Wpd,Ib"),
				new GroupDecoder(9, ""),

				Instr(Opcode.bswap, "rv"),
				Instr(Opcode.bswap, "rv"),
				Instr(Opcode.bswap, "rv"),
				Instr(Opcode.bswap, "rv"),
				Instr(Opcode.bswap, "rv"),
				Instr(Opcode.bswap, "rv"),
				Instr(Opcode.bswap, "rv"),
				Instr(Opcode.bswap, "rv"),

				// 0F D0
				new PrefixedDecoder(
					Opcode.illegal, "",
					Opcode.addsubpd, "Vpd,Hpd,Wpd",
					Opcode.illegal, "",
					Opcode.addsubps, "Vps,Hps,Wps"),
				new PrefixedDecoder(
                    Opcode.psrlw, "Pq,Qq",
                    Opcode.vpsrlw, "Vx,Hx,Wx"),
                new PrefixedDecoder(
                    Opcode.psrld, "Pq,Qq",
                    Opcode.vpsrld, "Vx,Hx,Wx"),
                new PrefixedDecoder(
                    Opcode.psrlq, "Pq,Qq",
                    Opcode.vpmullw, "Vx,Hx,Wx"),
                new PrefixedDecoder(
                    Opcode.paddq, "Pq,Qq",
                    Opcode.vpaddq, "Vx,Hx,Wx"),
                new PrefixedDecoder(
                    Opcode.pmullw, "Pq,Qq",
                    Opcode.vpmullw, "Vx,Hx,Wx"),
				Instr(Opcode.movq, "Wx,Vx"),
                new PrefixedDecoder(
                    Opcode.pmovmskb, "Gd,Nq",
                    Opcode.vpmovmskb, "Gd,Ux"),

                new PrefixedDecoder(
                    Opcode.psubusb, "Pq,Qq",
                    Opcode.vpsubusb, "Vx,Hx,Wx"),
                new PrefixedDecoder(
                    Opcode.psubusw, "Pq,Qq",
                    Opcode.vpsubusw, "Vx,Hx,Wx"),
                new PrefixedDecoder(
                    Opcode.pminub, "Pq,Qq",
                    Opcode.vpminub, "Vx,Hx,Wx"),
                new PrefixedDecoder(
                    Opcode.pand, "Pq,Qq",
                    Opcode.vpand, "Vx,Hx,Wx"),
                new PrefixedDecoder(
                    Opcode.paddusb, "Pq,Qq",
                    Opcode.vpaddusb, "Vx,Hx,Wx"),
                new PrefixedDecoder(
                    Opcode.paddusw, "Pq,Qq",
                    Opcode.vpaddusw, "Vx,Hx,Wx"),
                new PrefixedDecoder(
                    Opcode.pmaxub, "Pq,Qq",
                    Opcode.vpmaxub, "Vx,Hx,Wx"),
                new PrefixedDecoder(
                    Opcode.pandn, "Pq,Qq",
                    Opcode.vpandn, "Vx,Hx,Wx"),

				// 0F E0
                new PrefixedDecoder(
                    Opcode.pavgb, "Pq,Qq",
                    Opcode.vpavgb, "Vx,Hx,Wx"),
                new PrefixedDecoder(
                    Opcode.psraw, "Pq,Qq",
                    Opcode.vpsraw, "Vx,Hx,Wx"),
                new PrefixedDecoder(
                    Opcode.psrad, "Pq,Qq",
                    Opcode.vpsrad, "Vx,Hx,Wx"),
                new PrefixedDecoder(
                    Opcode.pavgw, "Pq,Qq",
                    Opcode.vpavgw, "Vx,Hx,Wx"),

                new PrefixedDecoder(
                    Opcode.pmulhuw, "Pq,Qq",
                    Opcode.vpmulhuw, "Vx,Hx,Wx"),
                new PrefixedDecoder(
                    Opcode.pmulhw, "Pq,Qq",
                    Opcode.vpmulhw, "Vx,Hx,Wx"),
                new PrefixedDecoder(
                    Opcode.illegal, "",
                    Opcode.cvttpd2dq, "Vdq,Wpd",
                    Opcode.cvtdq2pd, "Vx,Wpd",
                    Opcode.cvtpd2dq, "Vdq,Wpd"),
                new PrefixedDecoder(
                    Opcode.movntq, "Mq,Pq",
                    Opcode.vmovntq, "Mx,Vx"),

                new PrefixedDecoder(
                    Opcode.psubsb, "Pq,Qq",
                    Opcode.vpsubsb, "Vx,Hx,Wx"),
                new PrefixedDecoder(
                    Opcode.psubsw, "Pq,Qq",
                    Opcode.vpsubsw, "Vx,Hx,Wx"),
                new PrefixedDecoder(
                    Opcode.pminsw, "Pq,Qq",
                    Opcode.vpminsw, "Vx,Hx,Wx"),
                new PrefixedDecoder(
                    Opcode.por, "Pq,Qq",
                    Opcode.vpor, "Vx,Hx,Wx"),

                new PrefixedDecoder(
                    Opcode.paddsb, "Pq,Qq",
                    Opcode.vpaddsb, "Vx,Hx,Wx"),
                new PrefixedDecoder(
                    Opcode.paddsw, "Pq,Qq",
                    Opcode.vpaddsw, "Vx,Hx,Wx"),
                new PrefixedDecoder(
                    Opcode.pmaxsw, "Pq,Qq",
                    Opcode.vpmaxsw, "Vx,Hx,Wx"),
                new PrefixedDecoder(
                    Opcode.pxor, "Pq,Qq",
                    Opcode.vpxor, "Vx,Hx,Wx"),

				// 0F F0
                new PrefixedDecoder(
                    Opcode.illegal, "",
                    opF2: Opcode.vlddqu, opF2Fmt:"Vx,Mx"),
                new PrefixedDecoder(
                    Opcode.psllw, "Pq,Qq",
                    Opcode.vpsllw, "Vx,Hx,Wx"),
                new PrefixedDecoder(
                    Opcode.pslld, "Pq,Qq",
                    Opcode.vpslld, "Vx,Hx,Wx"),
                new PrefixedDecoder(
                    Opcode.psllq, "Pq,Qq",
                    Opcode.vpsllq, "Vx,Hx,Wx"),
                new PrefixedDecoder(
                    Opcode.pmuludq, "Pq,Qq",
                    Opcode.vpmuludq, "Vx,Hx,Wx"),
                new PrefixedDecoder(
                    Opcode.pmaddwd, "Pq,Qq",
                    Opcode.vpmaddwd, "Vx,Hx,Wx"),
                new PrefixedDecoder(
                    Opcode.psadbw, "Pq,Qq",
                    Opcode.vpsadbw, "Vx,Hx,Wx"),
                new PrefixedDecoder(
                    Opcode.maskmovq, "Pq,Qq",
                    Opcode.vmaskmovdqu, "Veq,Udq"),

                new PrefixedDecoder(
                    Opcode.psubb, "Pq,Qq",
                    Opcode.vpsubb, "Vx,Hx,Wx"),
                new PrefixedDecoder(
                    Opcode.psubw, "Pq,Qq",
                    Opcode.vpsubw, "Vx,Hx,Wx"),
                new PrefixedDecoder(
                    Opcode.psubd, "Pq,Qq",
                    Opcode.vpsubd, "Vx,Hx,Wx"),
                new PrefixedDecoder(
                    Opcode.psubq, "Pq,Qq",
                    Opcode.vpsubq, "Vx,Hx,Wx"),
                new PrefixedDecoder(
                    Opcode.paddb, "Pq,Qq",
                    Opcode.vpaddb, "Vx,Hx,Wx"),
                new PrefixedDecoder(
                    Opcode.paddw, "Pq,Qq",
                    Opcode.vpaddw, "Vx,Hx,Wx"),
                new PrefixedDecoder(
                    Opcode.paddd, "Pq,Qq",
                    Opcode.vpaddd, "Vx,Hx,Wx"),
				Instr(Opcode.ud0, "Gv,Ev"),
			};
        }
    }
}
