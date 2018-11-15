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
        private static OpRec[] CreateOnebyteOprecs()
        {
            return new OpRec[] { 
				// 00
				new SingleByteOpRec(Opcode.add, InstrClass.Linear|InstrClass.Zero, "Eb,Gb"),
				new SingleByteOpRec(Opcode.add, "Ev,Gv"),
				new SingleByteOpRec(Opcode.add, "Gb,Eb"),
				new SingleByteOpRec(Opcode.add, "Gv,Ev"),
				new SingleByteOpRec(Opcode.add, "ab,Ib"),
				new SingleByteOpRec(Opcode.add, "av,Iz"),
				new Alternative64OpRec(
                    new SingleByteOpRec(Opcode.push, "s0"),
                    s_invalid),
				new Alternative64OpRec(
                    new SingleByteOpRec(Opcode.pop, "s0"),
                    s_invalid),

				new SingleByteOpRec(Opcode.or, "Eb,Gb"),
				new SingleByteOpRec(Opcode.or, "Ev,Gv"),
				new SingleByteOpRec(Opcode.or, "Gb,Eb"),
				new SingleByteOpRec(Opcode.or, "Gv,Ev"),
				new SingleByteOpRec(Opcode.or, "ab,Ib"),
				new SingleByteOpRec(Opcode.or, "av,Iz"),
				new Alternative64OpRec(
                    new SingleByteOpRec(Opcode.push, "s1"),
                    s_invalid),
				new TwoByteOpRec(),

				// 10
				new SingleByteOpRec(Opcode.adc, "Eb,Gb"),
				new SingleByteOpRec(Opcode.adc, "Ev,Gv"),
				new SingleByteOpRec(Opcode.adc, "Gb,Eb"),
				new SingleByteOpRec(Opcode.adc, "Gv,Ev"),
				new SingleByteOpRec(Opcode.adc, "ab,Ib"),
				new SingleByteOpRec(Opcode.adc, "av,Iz"),
				new Alternative64OpRec(
                    new SingleByteOpRec(Opcode.push, "s2"),
                    s_invalid),
				new Alternative64OpRec(
                    new SingleByteOpRec(Opcode.pop, "s2"),
                    s_invalid),

				new SingleByteOpRec(Opcode.sbb, "Eb,Gb"),
				new SingleByteOpRec(Opcode.sbb, "Ev,Gv"),
				new SingleByteOpRec(Opcode.sbb, "Gb,Eb"),
				new SingleByteOpRec(Opcode.sbb, "Gv,Ev"),
				new SingleByteOpRec(Opcode.sbb, "ab,Ib"),
				new SingleByteOpRec(Opcode.sbb, "av,Iz"),
				new Alternative64OpRec(
                    new SingleByteOpRec(Opcode.push, "s3"),
                    s_invalid),
				new Alternative64OpRec(
                    new SingleByteOpRec(Opcode.pop, "s3"),
                    s_invalid),

				// 20
				new SingleByteOpRec(Opcode.and, "Eb,Gb"), 
				new SingleByteOpRec(Opcode.and, "Ev,Gv"),
				new SingleByteOpRec(Opcode.and, "Gb,Eb"),
				new SingleByteOpRec(Opcode.and, "Gv,Ev"),
				new SingleByteOpRec(Opcode.and, "ab,Ib"),
				new SingleByteOpRec(Opcode.and, "av,Iz"),
				new SegmentOverrideOprec(0),
				new Alternative64OpRec(
                    new SingleByteOpRec(Opcode.daa),
                    s_invalid),

				new SingleByteOpRec(Opcode.sub, "Eb,Gb"),
				new SingleByteOpRec(Opcode.sub, "Ev,Gv"),
				new SingleByteOpRec(Opcode.sub, "Gb,Eb"),
				new SingleByteOpRec(Opcode.sub, "Gv,Ev"),
				new SingleByteOpRec(Opcode.sub, "ab,Ib"),
				new SingleByteOpRec(Opcode.sub, "av,Iz"),
                new SegmentOverrideOprec(1),
				new Alternative64OpRec(
                    new SingleByteOpRec(Opcode.das),
                    s_invalid),

				// 30
				new SingleByteOpRec(Opcode.xor, "Eb,Gb"),
				new SingleByteOpRec(Opcode.xor, "Ev,Gv"),
				new SingleByteOpRec(Opcode.xor, "Gb,Eb"),
				new SingleByteOpRec(Opcode.xor, "Gv,Ev"),
				new SingleByteOpRec(Opcode.xor, "ab,Ib"),
				new SingleByteOpRec(Opcode.xor, "av,Iz"),
                new SegmentOverrideOprec(2),
				new Alternative64OpRec(
                    new SingleByteOpRec(Opcode.aaa),
                    s_invalid),

				new SingleByteOpRec(Opcode.cmp, "Eb,Gb"),
				new SingleByteOpRec(Opcode.cmp, "Ev,Gv"),
				new SingleByteOpRec(Opcode.cmp, "Gb,Eb"),
				new SingleByteOpRec(Opcode.cmp, "Gv,Ev"),
				new SingleByteOpRec(Opcode.cmp, "ab,Ib"),
				new SingleByteOpRec(Opcode.cmp, "av,Iz"),
                new SegmentOverrideOprec(3),
				new Alternative64OpRec(
                    new SingleByteOpRec(Opcode.aas),
                    s_invalid),

				// 40
				new Rex_SingleByteOpRec(Opcode.inc, "rv"),
				new Rex_SingleByteOpRec(Opcode.inc, "rv"),
				new Rex_SingleByteOpRec(Opcode.inc, "rv"),
				new Rex_SingleByteOpRec(Opcode.inc, "rv"),
				new Rex_SingleByteOpRec(Opcode.inc, "rv"),
				new Rex_SingleByteOpRec(Opcode.inc, "rv"),
				new Rex_SingleByteOpRec(Opcode.inc, "rv"),
				new Rex_SingleByteOpRec(Opcode.inc, "rv"),

				new Rex_SingleByteOpRec(Opcode.dec, "rv"),
				new Rex_SingleByteOpRec(Opcode.dec, "rv"),
				new Rex_SingleByteOpRec(Opcode.dec, "rv"),
				new Rex_SingleByteOpRec(Opcode.dec, "rv"),
				new Rex_SingleByteOpRec(Opcode.dec, "rv"),
				new Rex_SingleByteOpRec(Opcode.dec, "rv"),
				new Rex_SingleByteOpRec(Opcode.dec, "rv"),
				new Rex_SingleByteOpRec(Opcode.dec, "rv"),

				// 50
				new Alternative64OpRec(
                    new SingleByteOpRec(Opcode.push, "rv"),
                    new SingleByteOpRec(Opcode.push, "rq")),
				new Alternative64OpRec(
                    new SingleByteOpRec(Opcode.push, "rv"),
                    new SingleByteOpRec(Opcode.push, "rq")),
				new Alternative64OpRec(
                    new SingleByteOpRec(Opcode.push, "rv"),
                    new SingleByteOpRec(Opcode.push, "rq")),
				new Alternative64OpRec(
                    new SingleByteOpRec(Opcode.push, "rv"),
                    new SingleByteOpRec(Opcode.push, "rq")),
				new Alternative64OpRec(
                    new SingleByteOpRec(Opcode.push, "rv"),
                    new SingleByteOpRec(Opcode.push, "rq")),
				new Alternative64OpRec(
                    new SingleByteOpRec(Opcode.push, "rv"),
                    new SingleByteOpRec(Opcode.push, "rq")),
				new Alternative64OpRec(
                    new SingleByteOpRec(Opcode.push, "rv"),
                    new SingleByteOpRec(Opcode.push, "rq")),
				new Alternative64OpRec(
                    new SingleByteOpRec(Opcode.push, "rv"),
                    new SingleByteOpRec(Opcode.push, "rq")),

				new Alternative64OpRec(
                    new SingleByteOpRec(Opcode.pop, "rv"),
                    new SingleByteOpRec(Opcode.pop, "rq")),
				new Alternative64OpRec(
                    new SingleByteOpRec(Opcode.pop, "rv"),
                    new SingleByteOpRec(Opcode.pop, "rq")),
				new Alternative64OpRec(
                    new SingleByteOpRec(Opcode.pop, "rv"),
                    new SingleByteOpRec(Opcode.pop, "rq")),
				new Alternative64OpRec(
                    new SingleByteOpRec(Opcode.pop, "rv"),
                    new SingleByteOpRec(Opcode.pop, "rq")),
				new Alternative64OpRec(
                    new SingleByteOpRec(Opcode.pop, "rv"),
                    new SingleByteOpRec(Opcode.pop, "rq")),
				new Alternative64OpRec(
                    new SingleByteOpRec(Opcode.pop, "rv"),
                    new SingleByteOpRec(Opcode.pop, "rq")),
				new Alternative64OpRec(
                    new SingleByteOpRec(Opcode.pop, "rv"),
                    new SingleByteOpRec(Opcode.pop, "rq")),
				new Alternative64OpRec(
                    new SingleByteOpRec(Opcode.pop, "rv"),
                    new SingleByteOpRec(Opcode.pop, "rq")),

				// 60
				new Alternative64OpRec(
                    new SingleByteOpRec(Opcode.pusha),
                    s_invalid),
				new Alternative64OpRec(
                    new SingleByteOpRec(Opcode.popa),
                    s_invalid),
				new Alternative64OpRec(
                    new SingleByteOpRec(Opcode.bound, "Gv,Mv"),
                    s_invalid),
                new Alternative64OpRec(
    				new SingleByteOpRec(Opcode.arpl, "Ew,rw"),
    				new SingleByteOpRec(Opcode.movsx, "Gv,Ed")),
				new SegmentOverrideOprec(4),
				new SegmentOverrideOprec(5),
				new ChangeDataWidth(),
				new ChangeAddressWidth(),

				new SingleByteOpRec(Opcode.push, "Iz"),
				new SingleByteOpRec(Opcode.imul, "Gv,Ev,Iz"),
				new SingleByteOpRec(Opcode.push, "Ib"),
				new SingleByteOpRec(Opcode.imul, "Gv,Ev,Ib"),
				new SingleByteOpRec(Opcode.insb, "b"),
				new SingleByteOpRec(Opcode.ins,  ""),
				new SingleByteOpRec(Opcode.outsb, "b"),
				new SingleByteOpRec(Opcode.outs),

				// 70
				new SingleByteOpRec(Opcode.jo, InstrClass.Transfer|InstrClass.Conditional, "Jb"),
				new SingleByteOpRec(Opcode.jno, InstrClass.Transfer|InstrClass.Conditional, "Jb"),
				new SingleByteOpRec(Opcode.jc, InstrClass.Transfer|InstrClass.Conditional, "Jb"),
				new SingleByteOpRec(Opcode.jnc, InstrClass.Transfer|InstrClass.Conditional, "Jb"),
				new SingleByteOpRec(Opcode.jz, InstrClass.Transfer|InstrClass.Conditional, "Jb"),
				new SingleByteOpRec(Opcode.jnz, InstrClass.Transfer|InstrClass.Conditional, "Jb"),
				new SingleByteOpRec(Opcode.jbe, InstrClass.Transfer|InstrClass.Conditional, "Jb"),
				new SingleByteOpRec(Opcode.ja, InstrClass.Transfer|InstrClass.Conditional, "Jb"),

				new SingleByteOpRec(Opcode.js, InstrClass.Transfer|InstrClass.Conditional, "Jb"),
				new SingleByteOpRec(Opcode.jns, InstrClass.Transfer|InstrClass.Conditional, "Jb"),
				new SingleByteOpRec(Opcode.jpe, InstrClass.Transfer|InstrClass.Conditional, "Jb"),
				new SingleByteOpRec(Opcode.jpo, InstrClass.Transfer|InstrClass.Conditional, "Jb"),
				new SingleByteOpRec(Opcode.jl, InstrClass.Transfer|InstrClass.Conditional, "Jb"),
				new SingleByteOpRec(Opcode.jge, InstrClass.Transfer|InstrClass.Conditional, "Jb"),
				new SingleByteOpRec(Opcode.jle, InstrClass.Transfer|InstrClass.Conditional, "Jb"),
				new SingleByteOpRec(Opcode.jg, InstrClass.Transfer|InstrClass.Conditional, "Jb"),

				// 80
				new GroupOpRec(1, "Eb,Ib"),
				new GroupOpRec(1, "Ev,Iz"),
				new Alternative64OpRec(
                    new GroupOpRec(1, "Eb,Ib"),
                    s_invalid),
				new GroupOpRec(1, "Ev,Ib"),
				new SingleByteOpRec(Opcode.test, "Eb,Gb"),
				new SingleByteOpRec(Opcode.test, "Ev,Gv"),
				new SingleByteOpRec(Opcode.xchg, "Eb,Gb"),
				new SingleByteOpRec(Opcode.xchg, "Ev,Gv"),

				new SingleByteOpRec(Opcode.mov, "Eb,Gb"),
				new SingleByteOpRec(Opcode.mov, "Ev,Gv"),
				new SingleByteOpRec(Opcode.mov, "Gb,Eb"),
				new SingleByteOpRec(Opcode.mov, "Gv,Ev"),
				new SingleByteOpRec(Opcode.mov, "Ew,Sw"),
				new SingleByteOpRec(Opcode.lea, "Gv,Mv"),
				new SingleByteOpRec(Opcode.mov, "Sw,Ew"),
				new SingleByteOpRec(Opcode.pop, "Ev"),

				// 90
				new PrefixedOpRec(
                    Opcode.nop, "",
                    Opcode.nop, "",
                    Opcode.pause, "",
                    iclass:InstrClass.Linear|InstrClass.Padding),
				new SingleByteOpRec(Opcode.xchg, "av,rv"),
				new SingleByteOpRec(Opcode.xchg, "av,rv"),
				new SingleByteOpRec(Opcode.xchg, "av,rv"),
				new SingleByteOpRec(Opcode.xchg, "av,rv"),
				new SingleByteOpRec(Opcode.xchg, "av,rv"),
				new SingleByteOpRec(Opcode.xchg, "av,rv"),
				new SingleByteOpRec(Opcode.xchg, "av,rv"),

				new SingleByteOpRec(Opcode.cbw),
				new SingleByteOpRec(Opcode.cwd),
				new Alternative64OpRec(
                    new SingleByteOpRec(Opcode.call, InstrClass.Transfer|InstrClass.Call, "Ap"),
                    s_invalid),
				new SingleByteOpRec(Opcode.wait),
				new SingleByteOpRec(Opcode.pushf),
				new SingleByteOpRec(Opcode.popf),
				new SingleByteOpRec(Opcode.sahf),
				new SingleByteOpRec(Opcode.lahf),

				// A0
				new SingleByteOpRec(Opcode.mov, "ab,Ob"),
				new SingleByteOpRec(Opcode.mov, "av,Ov"),
				new SingleByteOpRec(Opcode.mov, "Ob,ab"),
				new SingleByteOpRec(Opcode.mov, "Ov,av"),
				new SingleByteOpRec(Opcode.movsb, "b"),
				new SingleByteOpRec(Opcode.movs),
				new SingleByteOpRec(Opcode.cmpsb, "b"),
				new SingleByteOpRec(Opcode.cmps),

				new SingleByteOpRec(Opcode.test, "ab,Ib"),
				new SingleByteOpRec(Opcode.test, "av,Iz"),
				new SingleByteOpRec(Opcode.stosb, "b"),
				new SingleByteOpRec(Opcode.stos),
				new SingleByteOpRec(Opcode.lodsb, "b"),
				new SingleByteOpRec(Opcode.lods),
				new SingleByteOpRec(Opcode.scasb, "b"),
				new SingleByteOpRec(Opcode.scas),

				// B0
				new SingleByteOpRec(Opcode.mov, "rb,Ib"),
				new SingleByteOpRec(Opcode.mov, "rb,Ib"),
				new SingleByteOpRec(Opcode.mov, "rb,Ib"),
				new SingleByteOpRec(Opcode.mov, "rb,Ib"),
				new SingleByteOpRec(Opcode.mov, "rb,Ib"),
				new SingleByteOpRec(Opcode.mov, "rb,Ib"),
				new SingleByteOpRec(Opcode.mov, "rb,Ib"),
				new SingleByteOpRec(Opcode.mov, "rb,Ib"),

				new SingleByteOpRec(Opcode.mov, "rv,Iv"),
				new SingleByteOpRec(Opcode.mov, "rv,Iv"),
				new SingleByteOpRec(Opcode.mov, "rv,Iv"),
				new SingleByteOpRec(Opcode.mov, "rv,Iv"),
				new SingleByteOpRec(Opcode.mov, "rv,Iv"),
				new SingleByteOpRec(Opcode.mov, "rv,Iv"),
				new SingleByteOpRec(Opcode.mov, "rv,Iv"),
				new SingleByteOpRec(Opcode.mov, "rv,Iv"),

				// C0
				new GroupOpRec(2, "Eb,Ib"),
				new GroupOpRec(2, "Ev,Ib"),
				new SingleByteOpRec(Opcode.ret, InstrClass.Transfer, "Iw"),
				new SingleByteOpRec(Opcode.ret, InstrClass.Transfer, ""),
				new Alternative64OpRec(
                    new SingleByteOpRec(Opcode.les,	"Gv,Mp"),
                    new VexDecoder3()),
				new Alternative64OpRec(
                    new SingleByteOpRec(Opcode.lds,	"Gv,Mp"),
                    new VexDecoder2()),
                new SingleByteOpRec(Opcode.mov,	"Eb,Ib"),
				new SingleByteOpRec(Opcode.mov,	"Ev,Iz"),

				new SingleByteOpRec(Opcode.enter, "Iw,Ib"),
				new SingleByteOpRec(Opcode.leave),
				new SingleByteOpRec(Opcode.retf, InstrClass.Transfer, "Iw"),
				new SingleByteOpRec(Opcode.retf, InstrClass.Transfer, ""),
				new SingleByteOpRec(Opcode.@int, InstrClass.Linear|InstrClass.Padding, "3"),
				new InterruptOpRec(Opcode.@int, "Ib"),
				new Alternative64OpRec(
                    new SingleByteOpRec(Opcode.into, ""),
                    s_invalid),
				new SingleByteOpRec(Opcode.iret, InstrClass.Transfer, ""),

				// D0
				new GroupOpRec(2, "Eb,1"),
				new GroupOpRec(2, "Ev,1"),
				new GroupOpRec(2, "Eb,c"),
				new GroupOpRec(2, "Ev,c"),
				new Alternative64OpRec(
                    new SingleByteOpRec(Opcode.aam, "Ib"),
                    s_invalid),
				new Alternative64OpRec(
                    new SingleByteOpRec(Opcode.aad, "Ib"),
				    s_invalid),
				s_invalid,
				new SingleByteOpRec(Opcode.xlat, "b"),

				new FpuOpRec(),
				new FpuOpRec(),
				new FpuOpRec(),
				new FpuOpRec(),
				new FpuOpRec(),
				new FpuOpRec(),
				new FpuOpRec(),
				new FpuOpRec(),

				// E0
				new SingleByteOpRec(Opcode.loopne, InstrClass.ConditionalTransfer, "Jb"),
				new SingleByteOpRec(Opcode.loope, InstrClass.ConditionalTransfer, "Jb"),
				new SingleByteOpRec(Opcode.loop, InstrClass.ConditionalTransfer, "Jb"),
				new SingleByteOpRec(Opcode.jcxz, InstrClass.ConditionalTransfer, "Jb"),
				new SingleByteOpRec(Opcode.@in, "ab,Ib"),
				new SingleByteOpRec(Opcode.@in, "av,Ib"),
				new SingleByteOpRec(Opcode.@out, "Ib,ab"),
				new SingleByteOpRec(Opcode.@out, "Ib,av"),

				new SingleByteOpRec(Opcode.call, InstrClass.Transfer|InstrClass.Call, "Jv"),
				new SingleByteOpRec(Opcode.jmp, InstrClass.Transfer, "Jv"),
				new Alternative64OpRec(
                    new SingleByteOpRec(Opcode.jmp, InstrClass.Transfer, "Ap"),
                    s_invalid),
				new SingleByteOpRec(Opcode.jmp, InstrClass.Transfer, "Jb"),
				new SingleByteOpRec(Opcode.@in, "ab,dw"),
				new SingleByteOpRec(Opcode.@in, "av,dw"),
				new SingleByteOpRec(Opcode.@out, "dw,ab"),
				new SingleByteOpRec(Opcode.@out, "dw,av"),

				// F0
				new SingleByteOpRec(Opcode.@lock),
				s_invalid,
				new F2ByteOpRec(),
				new F3ByteOpRec(),
				new SingleByteOpRec(Opcode.hlt, InstrClass.Terminates, ""),
				new SingleByteOpRec(Opcode.cmc),
				new GroupOpRec(3, "Eb"),
				new GroupOpRec(3, "Ev"),

				new SingleByteOpRec(Opcode.clc),
				new SingleByteOpRec(Opcode.stc),
				new SingleByteOpRec(Opcode.cli),
				new SingleByteOpRec(Opcode.sti),
				new SingleByteOpRec(Opcode.cld),
				new SingleByteOpRec(Opcode.std),
				new GroupOpRec(4, ""),
				new GroupOpRec(5, "")
			};
        }
    }
}
