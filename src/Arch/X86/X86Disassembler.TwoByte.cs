#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
        private static OpRec[] CreateTwobyteOprecs()
        {
            return new OpRec[]
            {
				// 00
				new SingleByteOpRec(Opcode.illegal),
                new GroupOpRec(7, ""),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new Alternative64OpRec(
                    new SingleByteOpRec(Opcode.illegal),
                    new SingleByteOpRec(Opcode.syscall)),
                new SingleByteOpRec(Opcode.illegal),
                new Alternative64OpRec(
                    new SingleByteOpRec(Opcode.illegal),
                    new SingleByteOpRec(Opcode.sysret)),

                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),

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
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new PrefixedOpRec(
                    Opcode.movlhps, "Vx,Wx",
                    Opcode.movhpd, "Vx,Wx",
                    Opcode.movshdup, "Vx,Wx"),
                new SingleByteOpRec(Opcode.illegal),

                new GroupOpRec(16, ""),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.nop, "Ev"),

				// 20
				new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),

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
                new SingleByteOpRec(Opcode.illegal),
                new PrefixedOpRec(
                    Opcode.cvttps2pi, "Pq,Wq",
                    Opcode.cvttpd2si, "Pd,Wq",
                    Opcode.cvttss2si, "Gd,Wq",
                    Opcode.cvttsd2si, "Gd,Wq"),
                new SingleByteOpRec(Opcode.illegal),
                new PrefixedOpRec(
                    Opcode.ucomiss, "Vss,Wss",
                    Opcode.ucomisd, "Vsd,Wsd"),
                new SingleByteOpRec(Opcode.illegal),

				// 30
				new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.rdtsc),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),

                new ThreeByteOpRec(),
                new SingleByteOpRec(Opcode.illegal),
                new ThreeByteOpRec(),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),

				// 40
				new SingleByteOpRec(Opcode.cmovo,  "Gv,Ev"),
                new SingleByteOpRec(Opcode.cmovno, "Gv,Ev"),
                new SingleByteOpRec(Opcode.cmovc,  "Gv,Ev"),
                new SingleByteOpRec(Opcode.cmovnc, "Gv,Ev"),
                new SingleByteOpRec(Opcode.cmovz,  "Gv,Ev"),
                new SingleByteOpRec(Opcode.cmovnz, "Gv,Ev"),
                new SingleByteOpRec(Opcode.cmovbe, "Gv,Ev"),
                new SingleByteOpRec(Opcode.cmova,  "Gv,Ev"),

                new SingleByteOpRec(Opcode.cmovs,  "Gv,Ev"),
                new SingleByteOpRec(Opcode.cmovns, "Gv,Ev"),
                new SingleByteOpRec(Opcode.cmovpe, "Gv,Ev"),
                new SingleByteOpRec(Opcode.cmovpo, "Gv,Ev"),
                new SingleByteOpRec(Opcode.cmovl,  "Gv,Ev"),
                new SingleByteOpRec(Opcode.cmovge, "Gv,Ev"),
                new SingleByteOpRec(Opcode.cmovle, "Gv,Ev"),
                new SingleByteOpRec(Opcode.cmovg,  "Gv,Ev"),

				// 50
				new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
                new SingleByteOpRec(Opcode.illegal),
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
                new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new PrefixedOpRec(
                    Opcode.subps, "Vps,Hps,Wps",
                    Opcode.subpd, "Vpd,Hpd,Wpd",
                    Opcode.subss, "Vss,Hss,Wss",
                    Opcode.subsd, "Vsd,Hsd,Wsd"),
				new SingleByteOpRec(Opcode.illegal),
                new PrefixedOpRec(
                    Opcode.divps, "Vps,Hps,Wps",
                    Opcode.divpd, "Vpd,Hpd,Wpd",
                    Opcode.divss, "Vss,Hss,Wss",
                    Opcode.divsd, "Vsd,Hsd,Wsd"),
                new SingleByteOpRec(Opcode.illegal),
					
				// 60
				new PrefixedOpRec(
                    Opcode.punpcklbw, "Pq,Qd",
                    Opcode.punpcklbw, "Vx,Wx"),
				new PrefixedOpRec(
                    Opcode.punpcklwd, "Pq,Qd",
                    Opcode.punpcklwd, "Vx,Wx"),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),

				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.movd, "Vy,Ey"),
				new SingleByteOpRec(Opcode.movdqa, "Vx,Wx"),

				// 70
				new PrefixedOpRec(
                    Opcode.pshufw, "Pq,Qq,Ib",
                    Opcode.pshufd, "Vx,Wx,Ib",
                    Opcode.pshufhw, "Vx,Wx,Ib",
                    Opcode.pshuflw, "Vx,Wx,Ib"),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new PrefixedOpRec(
                    Opcode.pcmpeqb, "Pq,Qq",
                    Opcode.pcmpeqb, "Vx,Wx"),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),

				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new PrefixedOpRec(
                    Opcode.movd, Opcode.movq, "Ey,Pd", 
                    Opcode.movd, Opcode.movq, "Ey,Vy",
                    Opcode.movq,              "Vy,Wy"),
				new SingleByteOpRec(Opcode.movdqa, "Wx,Vx"),

				// 80
				new SingleByteOpRec(Opcode.jo,	"Jv"),
				new SingleByteOpRec(Opcode.jno,   "Jv"),
				new SingleByteOpRec(Opcode.jc,	"Jv"),
				new SingleByteOpRec(Opcode.jnc,	"Jv"),
				new SingleByteOpRec(Opcode.jz,	"Jv"),
				new SingleByteOpRec(Opcode.jnz,   "Jv"),
				new SingleByteOpRec(Opcode.jbe,   "Jv"),
				new SingleByteOpRec(Opcode.ja,    "Jv"),

				new SingleByteOpRec(Opcode.js,    "Jv"),
				new SingleByteOpRec(Opcode.jns,   "Jv"),
				new SingleByteOpRec(Opcode.jpe,   "Jv"),
				new SingleByteOpRec(Opcode.jpo,   "Jv"),
				new SingleByteOpRec(Opcode.jl,    "Jv"),
				new SingleByteOpRec(Opcode.jge,   "Jv"),
				new SingleByteOpRec(Opcode.jle,   "Jv"),
				new SingleByteOpRec(Opcode.jg,    "Jv"),

				// 90
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

				// A0
				new SingleByteOpRec(Opcode.push, "s4"),
				new SingleByteOpRec(Opcode.pop, "s4"),
				new SingleByteOpRec(Opcode.cpuid, ""),
				new SingleByteOpRec(Opcode.bt, "Ev,Gv"),
				new SingleByteOpRec(Opcode.shld, "Ev,Gv,Ib"),
				new SingleByteOpRec(Opcode.shld, "Ev,Gv,c"),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),

				new SingleByteOpRec(Opcode.push, "s5"),
				new SingleByteOpRec(Opcode.pop, "s5"),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.bts, "Ev,Gv"),
				new SingleByteOpRec(Opcode.shrd, "Ev,Gv,Ib"),
				new SingleByteOpRec(Opcode.shrd, "Ev,Gv,c"),
				new GroupOpRec(15, ""),
				new SingleByteOpRec(Opcode.imul, "Gv,Ev"),

				// B0
				new SingleByteOpRec(Opcode.cmpxchg, "Eb,Gb"),
				new SingleByteOpRec(Opcode.cmpxchg, "Ev,Gv"),
				new SingleByteOpRec(Opcode.lss, "Gv,Mp"),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.lfs, "Gv,Mp"),
				new SingleByteOpRec(Opcode.lgs, "Gv,Mp"),
				new SingleByteOpRec(Opcode.movzx, "Gv,Eb"),
				new SingleByteOpRec(Opcode.movzx, "Gv,Ew"),

				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new GroupOpRec(8, "Ev,Ib"),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.bsr, "Gv,Ev"),
				new SingleByteOpRec(Opcode.movsx, "Gv,Eb"),
				new SingleByteOpRec(Opcode.movsx, "Gv,Ew"),

				// C0
				new SingleByteOpRec(Opcode.xadd, "Eb,Gb"),
				new SingleByteOpRec(Opcode.xadd, "Ev,Gv"),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),

				new SingleByteOpRec(Opcode.bswap, "rv"),
				new SingleByteOpRec(Opcode.bswap, "rv"),
				new SingleByteOpRec(Opcode.bswap, "rv"),
				new SingleByteOpRec(Opcode.bswap, "rv"),
				new SingleByteOpRec(Opcode.bswap, "rv"),
				new SingleByteOpRec(Opcode.bswap, "rv"),
				new SingleByteOpRec(Opcode.bswap, "rv"),
				new SingleByteOpRec(Opcode.bswap, "rv"),

				// D0
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.movq, "Wx,Vx"),
				new SingleByteOpRec(Opcode.illegal),

				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),

				// E0
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),

				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.pxor, "Pq,Qq"),

				// F0
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),

				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
			};
        }
    }
}
