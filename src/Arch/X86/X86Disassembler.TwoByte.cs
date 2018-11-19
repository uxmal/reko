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
        private static OpRec[] CreateTwobyteOprecs()
        {
            return new OpRec[]
            {
				// 00
				s_nyi,
                new GroupOpRec(7, ""),
                new SingleByteOpRec(Opcode.lar, InstrClass.System, "Gv,Ew"),
                new SingleByteOpRec(Opcode.lsl, InstrClass.System, "Gv,Ew"),
                s_nyi,
                new Alternative64OpRec(
                    s_nyi,
                    new SingleByteOpRec(Opcode.syscall, InstrClass.Transfer|InstrClass.Call, "")),
                new SingleByteOpRec(Opcode.clts),
                new Alternative64OpRec(
                    s_nyi,
                    new SingleByteOpRec(Opcode.sysret, InstrClass.Transfer, "")),

                new SingleByteOpRec(Opcode.invd, InstrClass.System, ""),
                new SingleByteOpRec(Opcode.wbinvd, InstrClass.System, ""),
                s_nyi,
                new SingleByteOpRec(Opcode.ud2),
                s_nyi,
                new SingleByteOpRec(Opcode.prefetchw, "Ev"),
                s_nyi,
                s_nyi,

				// 10
				new PrefixedOpRec(
                    Opcode.movups, "Vps,Wps",
                    Opcode.movupd, "Vpd,Wpd",
                    Opcode.movss,  "Vx,Wss",
                    Opcode.movsd,  "Vx,Wsd"),
                new PrefixedOpRec(
                    Opcode.movups, "Wps,Vps",
                    Opcode.movupd, "Wpd,Vpd",
                    Opcode.movss,  "Wss,Vss",
                    Opcode.movsd,  "Wsd,Vsd"),
                new PrefixedOpRec(
                    Opcode.movlps,   "Vq,Hq,Mq",
                    Opcode.movlpd,   "Vq,Hq,Mq",
                    Opcode.movsldup, "Vx,Wx",
                    Opcode.movddup,  "Vx,Wx"),
                s_nyi,
                new PrefixedOpRec(
                    Opcode.unpcklps, "Vx,Hx,Wx",
                    Opcode.unpcklpd, "Vx,Hx,Wx"),
                s_nyi,
                new PrefixedOpRec(
                    Opcode.movlhps, "Vx,Wx",
                    Opcode.movhpd, "Vx,Wx",
                    Opcode.movshdup, "Vx,Wx"),
                new PrefixedOpRec(
                    Opcode.movhps, "Mq,Vq",
                    Opcode.movhpd, "Mq,Vq"),

                new GroupOpRec(16, ""),
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                new SingleByteOpRec(Opcode.nop, InstrClass.Linear|InstrClass.Padding, "Ev"),

				// 0F 20
				new SingleByteOpRec(Opcode.mov, "Rv,Cd"),
                new SingleByteOpRec(Opcode.mov, "Rv,Dd"),
                new SingleByteOpRec(Opcode.mov, "Cd,Rv"),
                new SingleByteOpRec(Opcode.mov, "Dd,Rv"),
				s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,

                new PrefixedOpRec(
                    Opcode.movaps, "Vps,Wps",
                    Opcode.movapd, "Vpd,Wpd"),
                new PrefixedOpRec(
                    Opcode.movaps, "Wps,Vps",
                    Opcode.movapd, "Wpd,Vpd"),
                new PrefixedOpRec(
                    Opcode.cvtpi2ps, "Vps,Qpi",
                    Opcode.cvtpi2pd, "Vpd,Qpi",
                    Opcode.cvtsi2ss, "Vss,Hss,Ey",
                    Opcode.cvtsi2sd, "Vsd,Hsd,Ey"),
                new PrefixedOpRec(
                    Opcode.movntps, "Mps,Vps",
                    Opcode.movntpd, "Mpd,Vpd"),
                new PrefixedOpRec(
                    Opcode.cvttps2pi, "Ppi,Wps",
                    Opcode.cvttpd2pi, "Ppi,Wpq",
                    Opcode.cvttss2si, "Gd,Wss",
                    Opcode.cvttsd2si, "Gd,Wsd"),
                new PrefixedOpRec(
                    Opcode.cvtps2pi, "Ppi,Wps",
                    Opcode.cvtpd2si, "Qpi,Wpd",
                    Opcode.cvtss2si, "Gy,Wss",
                    Opcode.cvtsd2si, "Gy,Wsd"),
                new PrefixedOpRec(
                    Opcode.ucomiss, "Vss,Wss",
                    Opcode.ucomisd, "Vsd,Wsd"),
                new PrefixedOpRec(
                    Opcode.comiss, "Vss,Wss",
                    Opcode.comisd, "Vsd,Wsd"),

				// 0F 30
				new SingleByteOpRec(Opcode.wrmsr, InstrClass.System, ""),
                new SingleByteOpRec(Opcode.rdtsc),
                new SingleByteOpRec(Opcode.rdmsr, InstrClass.System, ""),
                new SingleByteOpRec(Opcode.rdpmc),
                new SingleByteOpRec(Opcode.sysenter),
                new SingleByteOpRec(Opcode.sysexit, InstrClass.Transfer, ""),
                s_nyi,
                new SingleByteOpRec(Opcode.getsec, InstrClass.System, ""),

                new ThreeByteOpRec(), // 0F 38
                s_nyi,
                new ThreeByteOpRec(), // 0F 3A
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,

				// 0F 40
				new SingleByteOpRec(Opcode.cmovo,  InstrClass.Linear|InstrClass.Conditional, "Gv,Ev"),
                new SingleByteOpRec(Opcode.cmovno, InstrClass.Linear|InstrClass.Conditional, "Gv,Ev"),
                new SingleByteOpRec(Opcode.cmovc,  InstrClass.Linear|InstrClass.Conditional, "Gv,Ev"),
                new SingleByteOpRec(Opcode.cmovnc, InstrClass.Linear|InstrClass.Conditional, "Gv,Ev"),
                new SingleByteOpRec(Opcode.cmovz,  InstrClass.Linear|InstrClass.Conditional, "Gv,Ev"),
                new SingleByteOpRec(Opcode.cmovnz, InstrClass.Linear|InstrClass.Conditional, "Gv,Ev"),
                new SingleByteOpRec(Opcode.cmovbe, InstrClass.Linear|InstrClass.Conditional, "Gv,Ev"),
                new SingleByteOpRec(Opcode.cmova,  InstrClass.Linear|InstrClass.Conditional, "Gv,Ev"),

                new SingleByteOpRec(Opcode.cmovs,  InstrClass.Linear|InstrClass.Conditional, "Gv,Ev"),
                new SingleByteOpRec(Opcode.cmovns, InstrClass.Linear|InstrClass.Conditional, "Gv,Ev"),
                new SingleByteOpRec(Opcode.cmovpe, InstrClass.Linear|InstrClass.Conditional, "Gv,Ev"),
                new SingleByteOpRec(Opcode.cmovpo, InstrClass.Linear|InstrClass.Conditional, "Gv,Ev"),
                new SingleByteOpRec(Opcode.cmovl,  InstrClass.Linear|InstrClass.Conditional, "Gv,Ev"),
                new SingleByteOpRec(Opcode.cmovge, InstrClass.Linear|InstrClass.Conditional, "Gv,Ev"),
                new SingleByteOpRec(Opcode.cmovle, InstrClass.Linear|InstrClass.Conditional, "Gv,Ev"),
                new SingleByteOpRec(Opcode.cmovg,  InstrClass.Linear|InstrClass.Conditional, "Gv,Ev"),

				// 0F 50
                new PrefixedOpRec(
                    Opcode.movmskps, "Gy,Ups",
                    Opcode.movmskpd, "Gy,Upd"),
                new PrefixedOpRec(
                    Opcode.sqrtps, "Vps,Wps",
                    Opcode.sqrtpd, "Vpd,Wpd",
                    Opcode.sqrtss, "Vss,Hss,Wss",
                    Opcode.sqrtsd, "Vsd,Hsd,Wsd"),
                new PrefixedOpRec(
                    Opcode.rsqrtps, "Vps,Wps",
                    Opcode.illegal, "",
                    Opcode.rsqrtss, "Vss,Hss,Wss",
                    Opcode.illegal, ""),
                new PrefixedOpRec(
                    Opcode.rcpps, "Vps,Wps",
                    Opcode.illegal, "",
                    Opcode.rcpss, "Vss,Hss,Wss",
                    Opcode.illegal, ""),
                new PrefixedOpRec(
                    Opcode.andps, "Vps,Hps,Wps",
                    Opcode.andpd, "Vpd,Hpd,Wpd"),
                new PrefixedOpRec(
                    Opcode.andnps, "Vps,Hps,Wps",
                    Opcode.andnpd, "Vpd,Hpd,Wpd"),
                new PrefixedOpRec(
                    Opcode.orps, "Vps,Hps,Wps",
                    Opcode.orpd, "Vpd,Hpd,Wpd"),
                new PrefixedOpRec(
                    Opcode.xorps, "Vps,Hps,Wps",
                    Opcode.xorpd, "Vpd,Hpd,Wpd"),

                new PrefixedOpRec(
                    Opcode.addps, "Vps,Hps,Wps",
                    Opcode.addpd, "Vpd,Hpd,Wpd",
                    Opcode.addss, "Vss,Hss,Wss",
                    Opcode.addsd, "Vsd,Hsd,Wsd"),
                new PrefixedOpRec(
                    Opcode.mulps, "Vps,Hps,Wps",
                    Opcode.mulpd, "Vpd,Hpd,Wpd",
                    Opcode.mulss, "Vss,Hss,Wss",
                    Opcode.mulsd, "Vsd,Hsd,Wsd"),
				new PrefixedOpRec(
                    Opcode.cvtps2pd, "Vpd,Wps",
                    Opcode.cvtpd2ps, "Vps,Wpd",
                    Opcode.cvtss2sd, "Vsd,Hx,Wss",
                    Opcode.cvtsd2ss, "Vss,Hx,Wsd"),
                new PrefixedOpRec(
                    Opcode.cvtdq2ps, "Vps,Wdq",
                    Opcode.cvtps2dq, "Vdq,Wps",
                    Opcode.cvttps2dq, "Vdq,Wps",
                    Opcode.illegal, ""),
                new PrefixedOpRec(
                    Opcode.subps, "Vps,Hps,Wps",
                    Opcode.subpd, "Vpd,Hpd,Wpd",
                    Opcode.subss, "Vss,Hss,Wss",
                    Opcode.subsd, "Vsd,Hsd,Wsd"),
                new PrefixedOpRec(
                    Opcode.minps, "Vps,Hps,Wps",
                    Opcode.minpd, "Vpd,Hpd,Wpd",
                    Opcode.minss, "Vss,Hss,Wss",
                    Opcode.minsd, "Vsd,Hsd,Wsd"),
                new PrefixedOpRec(
                    Opcode.divps, "Vps,Hps,Wps",
                    Opcode.divpd, "Vpd,Hpd,Wpd",
                    Opcode.divss, "Vss,Hss,Wss",
                    Opcode.divsd, "Vsd,Hsd,Wsd"),
                new PrefixedOpRec(
                    Opcode.maxps, "Vps,Hps,Wps",
                    Opcode.maxpd, "Vpd,Hpd,Wpd",
                    Opcode.maxss, "Vss,Hss,Wss",
                    Opcode.maxsd, "Vsd,Hsd,Wsd"),
					
				// 0F 60
				new PrefixedOpRec(
                    Opcode.punpcklbw, "Pq,Qd",
                    Opcode.punpcklbw, "Vx,Wx"),
				new PrefixedOpRec(
                    Opcode.punpcklwd, "Pq,Qd",
                    Opcode.punpcklwd, "Vx,Wx"),
                new PrefixedOpRec(
                    Opcode.punpckldq, "Pq,Qd",
                    Opcode.punpckldq, "Vx,Hx,Wx"),
				s_nyi,
                new PrefixedOpRec(
                    Opcode.pcmpgtb, "Pq,Qd",
                    Opcode.pcmpgtb, "Vx,Hx,Wx"),
                new PrefixedOpRec(
                    Opcode.pcmpgtw, "Pq,Qd",
                    Opcode.pcmpgtw, "Vx,Hx,Wx"),
                new PrefixedOpRec(
                    Opcode.pcmpgtd, "Pq,Qd",
                    Opcode.pcmpgtd, "Vx,Hx,Wx"),
                new PrefixedOpRec(
                    Opcode.packuswb, "Pq,Qd",
                    Opcode.vpunpckhbw, "Vx,Hx,Wx"),

                new PrefixedOpRec(
                    Opcode.punpckhbw, "Pq,Qd",
                    Opcode.vpunpckhbw, "Vx,Hx,Wx"),
                new PrefixedOpRec(
                    Opcode.punpckhwd, "Pq,Qd",
                    Opcode.vpunpckhwd, "Vx,Hx,Wx"),
                new PrefixedOpRec(
                    Opcode.punpckhdq, "Pq,Qd",
                    Opcode.vpunpckhdq, "Vx,Hx,Wx"),
                new PrefixedOpRec(
                    Opcode.packssdw, "Pq,Qd",
                    Opcode.vpackssdw, "Vx,Hx,Wx"),
				s_nyi,
				s_nyi,
				new SingleByteOpRec(Opcode.movd, "Vy,Ey"),
				new SingleByteOpRec(Opcode.movdqa, "Vx,Wx"),

				// 0F 70
				new PrefixedOpRec(
                    Opcode.pshufw, "Pq,Qq,Ib",
                    Opcode.pshufd, "Vx,Wx,Ib",
                    Opcode.pshufhw, "Vx,Wx,Ib",
                    Opcode.pshuflw, "Vx,Wx,Ib"),
				s_nyi,
				s_nyi,
				s_nyi,
				new PrefixedOpRec(
                    Opcode.pcmpeqb, "Pq,Qq",
                    Opcode.pcmpeqb, "Vx,Wx"),
                new PrefixedOpRec(
                    Opcode.pcmpeqw, "Pq,Qq",
                    Opcode.pcmpeqw, "Vx,Wx"),
                new PrefixedOpRec(
                    Opcode.pcmpeqd, "Pq,Qq",
                    Opcode.pcmpeqd, "Vx,Wx"),
                new SingleByteOpRec(Opcode.emms, InstrClass.System, ""),

				new SingleByteOpRec(Opcode.vmread, "Ey,Gy"),
				new SingleByteOpRec(Opcode.vmwrite, "Gy,Ey"),
				s_nyi,
				s_nyi,
				s_nyi,
				s_nyi,
				new PrefixedOpRec(
                    dec: Instr(Opcode.movd, "Ey,Pd"), decWide: Instr(Opcode.movq, "Ey,Pd"),
                    dec66: Instr(Opcode.movd, "Ey,Vy"), dec66Wide: Instr(Opcode.movq, "Ey,Vy"),
                    decF3: Instr(Opcode.movq, "Vy,Wy")),
				new SingleByteOpRec(Opcode.movdqa, "Wx,Vx"),

				// 0F 80
				new SingleByteOpRec(Opcode.jo,	InstrClass.ConditionalTransfer, "Jv"),
				new SingleByteOpRec(Opcode.jno, InstrClass.ConditionalTransfer, "Jv"),
				new SingleByteOpRec(Opcode.jc,	InstrClass.ConditionalTransfer, "Jv"),
				new SingleByteOpRec(Opcode.jnc,	InstrClass.ConditionalTransfer, "Jv"),
				new SingleByteOpRec(Opcode.jz,	InstrClass.ConditionalTransfer, "Jv"),
				new SingleByteOpRec(Opcode.jnz, InstrClass.ConditionalTransfer, "Jv"),
				new SingleByteOpRec(Opcode.jbe, InstrClass.ConditionalTransfer, "Jv"),
				new SingleByteOpRec(Opcode.ja,  InstrClass.ConditionalTransfer, "Jv"),

				new SingleByteOpRec(Opcode.js,  InstrClass.ConditionalTransfer, "Jv"),
				new SingleByteOpRec(Opcode.jns, InstrClass.ConditionalTransfer, "Jv"),
				new SingleByteOpRec(Opcode.jpe, InstrClass.ConditionalTransfer, "Jv"),
				new SingleByteOpRec(Opcode.jpo, InstrClass.ConditionalTransfer, "Jv"),
				new SingleByteOpRec(Opcode.jl,  InstrClass.ConditionalTransfer, "Jv"),
				new SingleByteOpRec(Opcode.jge, InstrClass.ConditionalTransfer, "Jv"),
				new SingleByteOpRec(Opcode.jle, InstrClass.ConditionalTransfer, "Jv"),
				new SingleByteOpRec(Opcode.jg,  InstrClass.ConditionalTransfer, "Jv"),

				// 0F 90
				new SingleByteOpRec(Opcode.seto, "Eb"),
				new SingleByteOpRec(Opcode.setno,"Eb"),
				new SingleByteOpRec(Opcode.setc, "Eb"),
				new SingleByteOpRec(Opcode.setnc,"Eb"),
				new SingleByteOpRec(Opcode.setz, "Eb"),
				new SingleByteOpRec(Opcode.setnz,"Eb"),
				new SingleByteOpRec(Opcode.setbe,"Eb"),
				new SingleByteOpRec(Opcode.seta, "Eb"),

				new SingleByteOpRec(Opcode.sets,  "Eb"),
				new SingleByteOpRec(Opcode.setns, "Eb"),
				new SingleByteOpRec(Opcode.setpe, "Eb"),
				new SingleByteOpRec(Opcode.setpo, "Eb"),
				new SingleByteOpRec(Opcode.setl,  "Eb"),
				new SingleByteOpRec(Opcode.setge, "Eb"),
				new SingleByteOpRec(Opcode.setle, "Eb"),
				new SingleByteOpRec(Opcode.setg,  "Eb"),

				// 0F A0
				new SingleByteOpRec(Opcode.push, "s4"),
				new SingleByteOpRec(Opcode.pop, "s4"),
				new SingleByteOpRec(Opcode.cpuid, ""),
				new SingleByteOpRec(Opcode.bt, "Ev,Gv"),
				new SingleByteOpRec(Opcode.shld, "Ev,Gv,Ib"),
				new SingleByteOpRec(Opcode.shld, "Ev,Gv,c"),
				s_nyi,
				s_nyi,

				new SingleByteOpRec(Opcode.push, "s5"),
				new SingleByteOpRec(Opcode.pop, "s5"),
				s_nyi,
				new SingleByteOpRec(Opcode.bts, "Ev,Gv"),
				new SingleByteOpRec(Opcode.shrd, "Ev,Gv,Ib"),
				new SingleByteOpRec(Opcode.shrd, "Ev,Gv,c"),
				new GroupOpRec(15, ""),
				new SingleByteOpRec(Opcode.imul, "Gv,Ev"),

				// 0F B0
				new SingleByteOpRec(Opcode.cmpxchg, "Eb,Gb"),
				new SingleByteOpRec(Opcode.cmpxchg, "Ev,Gv"),
				new SingleByteOpRec(Opcode.lss, "Gv,Mp"),
				s_nyi,
				new SingleByteOpRec(Opcode.lfs, "Gv,Mp"),
				new SingleByteOpRec(Opcode.lgs, "Gv,Mp"),
				new SingleByteOpRec(Opcode.movzx, "Gv,Eb"),
				new SingleByteOpRec(Opcode.movzx, "Gv,Ew"),

				s_nyi,
				s_nyi,
				new GroupOpRec(8, "Ev,Ib"),
				new SingleByteOpRec(Opcode.btc, "Gv,Ev"),
				new SingleByteOpRec(Opcode.bsf, "Gv,Ev"),
				new SingleByteOpRec(Opcode.bsr, "Gv,Ev"),
				new SingleByteOpRec(Opcode.movsx, "Gv,Eb"),
				new SingleByteOpRec(Opcode.movsx, "Gv,Ew"),

				// 0F C0
				new SingleByteOpRec(Opcode.xadd, "Eb,Gb"),
				new SingleByteOpRec(Opcode.xadd, "Ev,Gv"),
				new PrefixedOpRec(
                    Opcode.cmpps, "Vps,Hps,Wps,Ib",
                    Opcode.cmppd, "Vpd,Hpd,Wpd,Ib",
                    Opcode.cmpss, "Vss,Hss,Wss,Ib",
                    Opcode.cmpsd, "Vpd,Hpd,Wpd,Ib"),
				new PrefixedOpRec(
                    Opcode.movnti, "My,Gy",
                    Opcode.illegal, ""),
                new PrefixedOpRec(
                    Opcode.pinsrw, "Pq,Ry",     //$TODO: encoding is weird.
                    Opcode.vpinsrw, "Vdq,Hdq,Ry"),
                new PrefixedOpRec(
                    Opcode.pextrw, "Gd,Nq,Ib",
                    Opcode.vextrw, "Gd,Udq,Ib"),
                new PrefixedOpRec(
                    Opcode.vshufps, "Vps,Hps,Wps,Ib",
                    Opcode.vshufpd, "Vpd,Hpd,Wpd,Ib"),
				s_nyi,

				new SingleByteOpRec(Opcode.bswap, "rv"),
				new SingleByteOpRec(Opcode.bswap, "rv"),
				new SingleByteOpRec(Opcode.bswap, "rv"),
				new SingleByteOpRec(Opcode.bswap, "rv"),
				new SingleByteOpRec(Opcode.bswap, "rv"),
				new SingleByteOpRec(Opcode.bswap, "rv"),
				new SingleByteOpRec(Opcode.bswap, "rv"),
				new SingleByteOpRec(Opcode.bswap, "rv"),

				// 0F D0
				s_nyi,
				new PrefixedOpRec(
                    Opcode.psrlw, "Pq,Qq",
                    Opcode.vpsrlw, "Vx,Hx,Wx"),
                new PrefixedOpRec(
                    Opcode.psrld, "Pq,Qq",
                    Opcode.vpsrld, "Vx,Hx,Wx"),
                new PrefixedOpRec(
                    Opcode.psrlq, "Pq,Qq",
                    Opcode.vpmullw, "Vx,Hx,Wx"),
                new PrefixedOpRec(
                    Opcode.paddq, "Pq,Qq",
                    Opcode.vpaddq, "Vx,Hx,Wx"),
                new PrefixedOpRec(
                    Opcode.pmullw, "Pq,Qq",
                    Opcode.vpmullw, "Vx,Hx,Wx"),
				new SingleByteOpRec(Opcode.movq, "Wx,Vx"),
                new PrefixedOpRec(
                    Opcode.pmovmskb, "Gd,Nq",
                    Opcode.vpmovmskb, "Gd,Ux"),

                new PrefixedOpRec(
                    Opcode.psubusb, "Pq,Qq",
                    Opcode.vpsubusb, "Vx,Hx,Wx"),
                new PrefixedOpRec(
                    Opcode.psubusw, "Pq,Qq",
                    Opcode.vpsubusw, "Vx,Hx,Wx"),
                new PrefixedOpRec(
                    Opcode.pminub, "Pq,Qq",
                    Opcode.vpminub, "Vx,Hx,Wx"),
                new PrefixedOpRec(
                    Opcode.pand, "Pq,Qq",
                    Opcode.vpand, "Vx,Hx,Wx"),
                new PrefixedOpRec(
                    Opcode.paddusb, "Pq,Qq",
                    Opcode.vpaddusb, "Vx,Hx,Wx"),
                new PrefixedOpRec(
                    Opcode.paddusw, "Pq,Qq",
                    Opcode.vpaddusw, "Vx,Hx,Wx"),
                new PrefixedOpRec(
                    Opcode.pmaxub, "Pq,Qq",
                    Opcode.vpmaxub, "Vx,Hx,Wx"),
                new PrefixedOpRec(
                    Opcode.pandn, "Pq,Qq",
                    Opcode.vpandn, "Vx,Hx,Wx"),

				// E0
                new PrefixedOpRec(
                    Opcode.pavgb, "Pq,Qq",
                    Opcode.vpavgb, "Vx,Hx,Wx"),
                new PrefixedOpRec(
                    Opcode.psraw, "Pq,Qq",
                    Opcode.vpsraw, "Vx,Hx,Wx"),
                new PrefixedOpRec(
                    Opcode.psrad, "Pq,Qq",
                    Opcode.vpsrad, "Vx,Hx,Wx"),
                new PrefixedOpRec(
                    Opcode.pavgw, "Pq,Qq",
                    Opcode.vpavgw, "Vx,Hx,Wx"),
                new PrefixedOpRec(
                    Opcode.pmulhuw, "Pq,Qq",
                    Opcode.vpmulhuw, "Vx,Hx,Wx"),
                new PrefixedOpRec(
                    Opcode.pmulhw, "Pq,Qq",
                    Opcode.vpmulhw, "Vx,Hx,Wx"),
				s_nyi,
                new PrefixedOpRec(
                    Opcode.movntq, "Mq,Pq",
                    Opcode.vmovntq, "Mx,Vx"),

                new PrefixedOpRec(
                    Opcode.psubsb, "Pq,Qq",
                    Opcode.vpsubsb, "Vx,Hx,Wx"),
                new PrefixedOpRec(
                    Opcode.psubsw, "Pq,Qq",
                    Opcode.vpsubsw, "Vx,Hx,Wx"),
                new PrefixedOpRec(
                    Opcode.pminsw, "Pq,Qq",
                    Opcode.vpminsw, "Vx,Hx,Wx"),
                new PrefixedOpRec(
                    Opcode.por, "Pq,Qq",
                    Opcode.vpor, "Vx,Hx,Wx"),
                new PrefixedOpRec(
                    Opcode.paddsb, "Pq,Qq",
                    Opcode.vpaddsb, "Vx,Hx,Wx"),
                new PrefixedOpRec(
                    Opcode.paddsw, "Pq,Qq",
                    Opcode.vpaddsw, "Vx,Hx,Wx"),
                new PrefixedOpRec(
                    Opcode.pmaxsw, "Pq,Qq",
                    Opcode.vpmaxsw, "Vx,Hx,Wx"),
                new PrefixedOpRec(
                    Opcode.pxor, "Pq,Qq",
                    Opcode.vpxor, "Vx,Hx,Wx"),

				// F0
				s_nyi,
                new PrefixedOpRec(
                    Opcode.psllw, "Pq,Qq",
                    Opcode.vpsllw, "Vx,Hx,Wx"),
                new PrefixedOpRec(
                    Opcode.pslld, "Pq,Qq",
                    Opcode.vpslld, "Vx,Hx,Wx"),
                new PrefixedOpRec(
                    Opcode.psllq, "Pq,Qq",
                    Opcode.vpsllq, "Vx,Hx,Wx"),
                new PrefixedOpRec(
                    Opcode.pmuludq, "Pq,Qq",
                    Opcode.vpmuludq, "Vx,Hx,Wx"),
                new PrefixedOpRec(
                    Opcode.pmaddwd, "Pq,Qq",
                    Opcode.vpmaddwd, "Vx,Hx,Wx"),
                new PrefixedOpRec(
                    Opcode.psadbw, "Pq,Qq",
                    Opcode.vpsadbw, "Vx,Hx,Wx"),
                new PrefixedOpRec(
                    Opcode.maskmovq, "Pq,Qq",
                    Opcode.vmaskmovdqu, "Veq,Udq"),

                new PrefixedOpRec(
                    Opcode.psubb, "Pq,Qq",
                    Opcode.vpsubb, "Vx,Hx,Wx"),
                new PrefixedOpRec(
                    Opcode.psubw, "Pq,Qq",
                    Opcode.vpsubw, "Vx,Hx,Wx"),
                new PrefixedOpRec(
                    Opcode.psubd, "Pq,Qq",
                    Opcode.vpsubd, "Vx,Hx,Wx"),
                new PrefixedOpRec(
                    Opcode.psubq, "Pq,Qq",
                    Opcode.vpsubq, "Vx,Hx,Wx"),
                new PrefixedOpRec(
                    Opcode.paddb, "Pq,Qq",
                    Opcode.vpaddb, "Vx,Hx,Wx"),
                new PrefixedOpRec(
                    Opcode.paddw, "Pq,Qq",
                    Opcode.vpaddw, "Vx,Hx,Wx"),
                new PrefixedOpRec(
                    Opcode.paddd, "Pq,Qq",
                    Opcode.vpaddd, "Vx,Hx,Wx"),
				s_nyi,
			};
        }
    }
}
