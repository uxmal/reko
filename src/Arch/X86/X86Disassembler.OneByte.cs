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
        private static Decoder[] CreateOnebyteOprecs()
        {
            return new Decoder[] { 
				// 00
				new InstructionDecoder(Opcode.add, InstrClass.Linear|InstrClass.Zero, "Eb,Gb"),
				new InstructionDecoder(Opcode.add, "Ev,Gv"),
				new InstructionDecoder(Opcode.add, "Gb,Eb"),
				new InstructionDecoder(Opcode.add, "Gv,Ev"),
				new InstructionDecoder(Opcode.add, "ab,Ib"),
				new InstructionDecoder(Opcode.add, "av,Iz"),
				new Alternative64Decoder(
                    new InstructionDecoder(Opcode.push, "s0"),
                    s_invalid),
				new Alternative64Decoder(
                    new InstructionDecoder(Opcode.pop, "s0"),
                    s_invalid),

				new InstructionDecoder(Opcode.or, "Eb,Gb"),
				new InstructionDecoder(Opcode.or, "Ev,Gv"),
				new InstructionDecoder(Opcode.or, "Gb,Eb"),
				new InstructionDecoder(Opcode.or, "Gv,Ev"),
				new InstructionDecoder(Opcode.or, "ab,Ib"),
				new InstructionDecoder(Opcode.or, "av,Iz"),
				new Alternative64Decoder(
                    new InstructionDecoder(Opcode.push, "s1"),
                    s_invalid),
				new AdditionalByteDecoder(),

				// 10
				new InstructionDecoder(Opcode.adc, "Eb,Gb"),
				new InstructionDecoder(Opcode.adc, "Ev,Gv"),
				new InstructionDecoder(Opcode.adc, "Gb,Eb"),
				new InstructionDecoder(Opcode.adc, "Gv,Ev"),
				new InstructionDecoder(Opcode.adc, "ab,Ib"),
				new InstructionDecoder(Opcode.adc, "av,Iz"),
				new Alternative64Decoder(
                    new InstructionDecoder(Opcode.push, "s2"),
                    s_invalid),
				new Alternative64Decoder(
                    new InstructionDecoder(Opcode.pop, "s2"),
                    s_invalid),

				new InstructionDecoder(Opcode.sbb, "Eb,Gb"),
				new InstructionDecoder(Opcode.sbb, "Ev,Gv"),
				new InstructionDecoder(Opcode.sbb, "Gb,Eb"),
				new InstructionDecoder(Opcode.sbb, "Gv,Ev"),
				new InstructionDecoder(Opcode.sbb, "ab,Ib"),
				new InstructionDecoder(Opcode.sbb, "av,Iz"),
				new Alternative64Decoder(
                    new InstructionDecoder(Opcode.push, "s3"),
                    s_invalid),
				new Alternative64Decoder(
                    new InstructionDecoder(Opcode.pop, "s3"),
                    s_invalid),

				// 20
				new InstructionDecoder(Opcode.and, "Eb,Gb"), 
				new InstructionDecoder(Opcode.and, "Ev,Gv"),
				new InstructionDecoder(Opcode.and, "Gb,Eb"),
				new InstructionDecoder(Opcode.and, "Gv,Ev"),
				new InstructionDecoder(Opcode.and, "ab,Ib"),
				new InstructionDecoder(Opcode.and, "av,Iz"),
				new SegmentOverrideDecoder(0),
				new Alternative64Decoder(
                    new InstructionDecoder(Opcode.daa),
                    s_invalid),

				new InstructionDecoder(Opcode.sub, "Eb,Gb"),
				new InstructionDecoder(Opcode.sub, "Ev,Gv"),
				new InstructionDecoder(Opcode.sub, "Gb,Eb"),
				new InstructionDecoder(Opcode.sub, "Gv,Ev"),
				new InstructionDecoder(Opcode.sub, "ab,Ib"),
				new InstructionDecoder(Opcode.sub, "av,Iz"),
                new SegmentOverrideDecoder(1),
				new Alternative64Decoder(
                    new InstructionDecoder(Opcode.das),
                    s_invalid),

				// 30
				new InstructionDecoder(Opcode.xor, "Eb,Gb"),
				new InstructionDecoder(Opcode.xor, "Ev,Gv"),
				new InstructionDecoder(Opcode.xor, "Gb,Eb"),
				new InstructionDecoder(Opcode.xor, "Gv,Ev"),
				new InstructionDecoder(Opcode.xor, "ab,Ib"),
				new InstructionDecoder(Opcode.xor, "av,Iz"),
                new SegmentOverrideDecoder(2),
				new Alternative64Decoder(
                    new InstructionDecoder(Opcode.aaa),
                    s_invalid),

				new InstructionDecoder(Opcode.cmp, "Eb,Gb"),
				new InstructionDecoder(Opcode.cmp, "Ev,Gv"),
				new InstructionDecoder(Opcode.cmp, "Gb,Eb"),
				new InstructionDecoder(Opcode.cmp, "Gv,Ev"),
				new InstructionDecoder(Opcode.cmp, "ab,Ib"),
				new InstructionDecoder(Opcode.cmp, "av,Iz"),
                new SegmentOverrideDecoder(3),
				new Alternative64Decoder(
                    new InstructionDecoder(Opcode.aas),
                    s_invalid),

				// 40
				new Rex_or_InstructionDecoder(Opcode.inc, "rv"),
				new Rex_or_InstructionDecoder(Opcode.inc, "rv"),
				new Rex_or_InstructionDecoder(Opcode.inc, "rv"),
				new Rex_or_InstructionDecoder(Opcode.inc, "rv"),
				new Rex_or_InstructionDecoder(Opcode.inc, "rv"),
				new Rex_or_InstructionDecoder(Opcode.inc, "rv"),
				new Rex_or_InstructionDecoder(Opcode.inc, "rv"),
				new Rex_or_InstructionDecoder(Opcode.inc, "rv"),

				new Rex_or_InstructionDecoder(Opcode.dec, "rv"),
				new Rex_or_InstructionDecoder(Opcode.dec, "rv"),
				new Rex_or_InstructionDecoder(Opcode.dec, "rv"),
				new Rex_or_InstructionDecoder(Opcode.dec, "rv"),
				new Rex_or_InstructionDecoder(Opcode.dec, "rv"),
				new Rex_or_InstructionDecoder(Opcode.dec, "rv"),
				new Rex_or_InstructionDecoder(Opcode.dec, "rv"),
				new Rex_or_InstructionDecoder(Opcode.dec, "rv"),

				// 50
				new Alternative64Decoder(
                    new InstructionDecoder(Opcode.push, "rv"),
                    new InstructionDecoder(Opcode.push, "rq")),
				new Alternative64Decoder(
                    new InstructionDecoder(Opcode.push, "rv"),
                    new InstructionDecoder(Opcode.push, "rq")),
				new Alternative64Decoder(
                    new InstructionDecoder(Opcode.push, "rv"),
                    new InstructionDecoder(Opcode.push, "rq")),
				new Alternative64Decoder(
                    new InstructionDecoder(Opcode.push, "rv"),
                    new InstructionDecoder(Opcode.push, "rq")),
				new Alternative64Decoder(
                    new InstructionDecoder(Opcode.push, "rv"),
                    new InstructionDecoder(Opcode.push, "rq")),
				new Alternative64Decoder(
                    new InstructionDecoder(Opcode.push, "rv"),
                    new InstructionDecoder(Opcode.push, "rq")),
				new Alternative64Decoder(
                    new InstructionDecoder(Opcode.push, "rv"),
                    new InstructionDecoder(Opcode.push, "rq")),
				new Alternative64Decoder(
                    new InstructionDecoder(Opcode.push, "rv"),
                    new InstructionDecoder(Opcode.push, "rq")),

				new Alternative64Decoder(
                    new InstructionDecoder(Opcode.pop, "rv"),
                    new InstructionDecoder(Opcode.pop, "rq")),
				new Alternative64Decoder(
                    new InstructionDecoder(Opcode.pop, "rv"),
                    new InstructionDecoder(Opcode.pop, "rq")),
				new Alternative64Decoder(
                    new InstructionDecoder(Opcode.pop, "rv"),
                    new InstructionDecoder(Opcode.pop, "rq")),
				new Alternative64Decoder(
                    new InstructionDecoder(Opcode.pop, "rv"),
                    new InstructionDecoder(Opcode.pop, "rq")),
				new Alternative64Decoder(
                    new InstructionDecoder(Opcode.pop, "rv"),
                    new InstructionDecoder(Opcode.pop, "rq")),
				new Alternative64Decoder(
                    new InstructionDecoder(Opcode.pop, "rv"),
                    new InstructionDecoder(Opcode.pop, "rq")),
				new Alternative64Decoder(
                    new InstructionDecoder(Opcode.pop, "rv"),
                    new InstructionDecoder(Opcode.pop, "rq")),
				new Alternative64Decoder(
                    new InstructionDecoder(Opcode.pop, "rv"),
                    new InstructionDecoder(Opcode.pop, "rq")),

				// 60
				new Alternative64Decoder(
                    new InstructionDecoder(Opcode.pusha),
                    s_invalid),
				new Alternative64Decoder(
                    new InstructionDecoder(Opcode.popa),
                    s_invalid),
				new Alternative64Decoder(
                    new InstructionDecoder(Opcode.bound, "Gv,Mv"),
                    s_invalid),
                new Alternative64Decoder(
    				new InstructionDecoder(Opcode.arpl, "Ew,rw"),
    				new InstructionDecoder(Opcode.movsx, "Gv,Ed")),
				new SegmentOverrideDecoder(4),
				new SegmentOverrideDecoder(5),
				new ChangeDataWidth(),
				new ChangeAddressWidth(),

				new InstructionDecoder(Opcode.push, "Iz"),
				new InstructionDecoder(Opcode.imul, "Gv,Ev,Iz"),
				new InstructionDecoder(Opcode.push, "Ib"),
				new InstructionDecoder(Opcode.imul, "Gv,Ev,Ib"),
				new InstructionDecoder(Opcode.insb, "b"),
				new InstructionDecoder(Opcode.ins,  ""),
				new InstructionDecoder(Opcode.outsb, "b"),
				new InstructionDecoder(Opcode.outs),

				// 70
				new InstructionDecoder(Opcode.jo, InstrClass.Transfer|InstrClass.Conditional, "Jb"),
				new InstructionDecoder(Opcode.jno, InstrClass.Transfer|InstrClass.Conditional, "Jb"),
				new InstructionDecoder(Opcode.jc, InstrClass.Transfer|InstrClass.Conditional, "Jb"),
				new InstructionDecoder(Opcode.jnc, InstrClass.Transfer|InstrClass.Conditional, "Jb"),
				new InstructionDecoder(Opcode.jz, InstrClass.Transfer|InstrClass.Conditional, "Jb"),
				new InstructionDecoder(Opcode.jnz, InstrClass.Transfer|InstrClass.Conditional, "Jb"),
				new InstructionDecoder(Opcode.jbe, InstrClass.Transfer|InstrClass.Conditional, "Jb"),
				new InstructionDecoder(Opcode.ja, InstrClass.Transfer|InstrClass.Conditional, "Jb"),

				new InstructionDecoder(Opcode.js, InstrClass.Transfer|InstrClass.Conditional, "Jb"),
				new InstructionDecoder(Opcode.jns, InstrClass.Transfer|InstrClass.Conditional, "Jb"),
				new InstructionDecoder(Opcode.jpe, InstrClass.Transfer|InstrClass.Conditional, "Jb"),
				new InstructionDecoder(Opcode.jpo, InstrClass.Transfer|InstrClass.Conditional, "Jb"),
				new InstructionDecoder(Opcode.jl, InstrClass.Transfer|InstrClass.Conditional, "Jb"),
				new InstructionDecoder(Opcode.jge, InstrClass.Transfer|InstrClass.Conditional, "Jb"),
				new InstructionDecoder(Opcode.jle, InstrClass.Transfer|InstrClass.Conditional, "Jb"),
				new InstructionDecoder(Opcode.jg, InstrClass.Transfer|InstrClass.Conditional, "Jb"),

				// 80
				new GroupDecoder(1, "Eb,Ib"),
				new GroupDecoder(1, "Ev,Iz"),
				new Alternative64Decoder(
                    new GroupDecoder(1, "Eb,Ib"),
                    s_invalid),
				new GroupDecoder(1, "Ev,Ib"),
				new InstructionDecoder(Opcode.test, "Eb,Gb"),
				new InstructionDecoder(Opcode.test, "Ev,Gv"),
				new InstructionDecoder(Opcode.xchg, "Eb,Gb"),
				new InstructionDecoder(Opcode.xchg, "Ev,Gv"),

				new InstructionDecoder(Opcode.mov, "Eb,Gb"),
				new InstructionDecoder(Opcode.mov, "Ev,Gv"),
				new InstructionDecoder(Opcode.mov, "Gb,Eb"),
				new InstructionDecoder(Opcode.mov, "Gv,Ev"),
				new InstructionDecoder(Opcode.mov, "Ew,Sw"),
				new InstructionDecoder(Opcode.lea, "Gv,Mv"),
				new InstructionDecoder(Opcode.mov, "Sw,Ew"),
				new InstructionDecoder(Opcode.pop, "Ev"),

				// 90
				new PrefixedDecoder(
                    Opcode.nop, "",
                    Opcode.nop, "",
                    Opcode.pause, "",
                    iclass:InstrClass.Linear|InstrClass.Padding),
				new InstructionDecoder(Opcode.xchg, "av,rv"),
				new InstructionDecoder(Opcode.xchg, "av,rv"),
				new InstructionDecoder(Opcode.xchg, "av,rv"),
				new InstructionDecoder(Opcode.xchg, "av,rv"),
				new InstructionDecoder(Opcode.xchg, "av,rv"),
				new InstructionDecoder(Opcode.xchg, "av,rv"),
				new InstructionDecoder(Opcode.xchg, "av,rv"),

				new InstructionDecoder(Opcode.cbw),
				new InstructionDecoder(Opcode.cwd),
				new Alternative64Decoder(
                    new InstructionDecoder(Opcode.call, InstrClass.Transfer|InstrClass.Call, "Ap"),
                    s_invalid),
				new InstructionDecoder(Opcode.wait),
				new InstructionDecoder(Opcode.pushf),
				new InstructionDecoder(Opcode.popf),
				new InstructionDecoder(Opcode.sahf),
				new InstructionDecoder(Opcode.lahf),

				// A0
				new InstructionDecoder(Opcode.mov, "ab,Ob"),
				new InstructionDecoder(Opcode.mov, "av,Ov"),
				new InstructionDecoder(Opcode.mov, "Ob,ab"),
				new InstructionDecoder(Opcode.mov, "Ov,av"),
				new InstructionDecoder(Opcode.movsb, "b"),
				new InstructionDecoder(Opcode.movs),
				new InstructionDecoder(Opcode.cmpsb, "b"),
				new InstructionDecoder(Opcode.cmps),

				new InstructionDecoder(Opcode.test, "ab,Ib"),
				new InstructionDecoder(Opcode.test, "av,Iz"),
				new InstructionDecoder(Opcode.stosb, "b"),
				new InstructionDecoder(Opcode.stos),
				new InstructionDecoder(Opcode.lodsb, "b"),
				new InstructionDecoder(Opcode.lods),
				new InstructionDecoder(Opcode.scasb, "b"),
				new InstructionDecoder(Opcode.scas),

				// B0
				new InstructionDecoder(Opcode.mov, "rb,Ib"),
				new InstructionDecoder(Opcode.mov, "rb,Ib"),
				new InstructionDecoder(Opcode.mov, "rb,Ib"),
				new InstructionDecoder(Opcode.mov, "rb,Ib"),
				new InstructionDecoder(Opcode.mov, "rb,Ib"),
				new InstructionDecoder(Opcode.mov, "rb,Ib"),
				new InstructionDecoder(Opcode.mov, "rb,Ib"),
				new InstructionDecoder(Opcode.mov, "rb,Ib"),

				new InstructionDecoder(Opcode.mov, "rv,Iv"),
				new InstructionDecoder(Opcode.mov, "rv,Iv"),
				new InstructionDecoder(Opcode.mov, "rv,Iv"),
				new InstructionDecoder(Opcode.mov, "rv,Iv"),
				new InstructionDecoder(Opcode.mov, "rv,Iv"),
				new InstructionDecoder(Opcode.mov, "rv,Iv"),
				new InstructionDecoder(Opcode.mov, "rv,Iv"),
				new InstructionDecoder(Opcode.mov, "rv,Iv"),

				// C0
				new GroupDecoder(2, "Eb,Ib"),
				new GroupDecoder(2, "Ev,Ib"),
				new InstructionDecoder(Opcode.ret, InstrClass.Transfer, "Iw"),
				new InstructionDecoder(Opcode.ret, InstrClass.Transfer, ""),
				new Alternative64Decoder(
                    new InstructionDecoder(Opcode.les,	"Gv,Mp"),
                    new VexDecoder3()),
				new Alternative64Decoder(
                    new InstructionDecoder(Opcode.lds,	"Gv,Mp"),
                    new VexDecoder2()),
                new InstructionDecoder(Opcode.mov,	"Eb,Ib"),
				new InstructionDecoder(Opcode.mov,	"Ev,Iz"),

				new InstructionDecoder(Opcode.enter, "Iw,Ib"),
				new InstructionDecoder(Opcode.leave),
				new InstructionDecoder(Opcode.retf, InstrClass.Transfer, "Iw"),
				new InstructionDecoder(Opcode.retf, InstrClass.Transfer, ""),
				new InstructionDecoder(Opcode.@int, InstrClass.Linear|InstrClass.Padding, "3"),
				new InterruptDecoder(Opcode.@int, "Ib"),
				new Alternative64Decoder(
                    new InstructionDecoder(Opcode.into, ""),
                    s_invalid),
				new InstructionDecoder(Opcode.iret, InstrClass.Transfer, ""),

				// D0
				new GroupDecoder(2, "Eb,1"),
				new GroupDecoder(2, "Ev,1"),
				new GroupDecoder(2, "Eb,c"),
				new GroupDecoder(2, "Ev,c"),
				new Alternative64Decoder(
                    new InstructionDecoder(Opcode.aam, "Ib"),
                    s_invalid),
				new Alternative64Decoder(
                    new InstructionDecoder(Opcode.aad, "Ib"),
				    s_invalid),
				s_invalid,
				new InstructionDecoder(Opcode.xlat, "b"),

				new X87Decoder(),
				new X87Decoder(),
				new X87Decoder(),
				new X87Decoder(),
				new X87Decoder(),
				new X87Decoder(),
				new X87Decoder(),
				new X87Decoder(),

				// E0
				new InstructionDecoder(Opcode.loopne, InstrClass.ConditionalTransfer, "Jb"),
				new InstructionDecoder(Opcode.loope, InstrClass.ConditionalTransfer, "Jb"),
				new InstructionDecoder(Opcode.loop, InstrClass.ConditionalTransfer, "Jb"),
				new InstructionDecoder(Opcode.jcxz, InstrClass.ConditionalTransfer, "Jb"),
				new InstructionDecoder(Opcode.@in, "ab,Ib"),
				new InstructionDecoder(Opcode.@in, "av,Ib"),
				new InstructionDecoder(Opcode.@out, "Ib,ab"),
				new InstructionDecoder(Opcode.@out, "Ib,av"),

				new InstructionDecoder(Opcode.call, InstrClass.Transfer|InstrClass.Call, "Jv"),
				new InstructionDecoder(Opcode.jmp, InstrClass.Transfer, "Jv"),
				new Alternative64Decoder(
                    new InstructionDecoder(Opcode.jmp, InstrClass.Transfer, "Ap"),
                    s_invalid),
				new InstructionDecoder(Opcode.jmp, InstrClass.Transfer, "Jb"),
				new InstructionDecoder(Opcode.@in, "ab,dw"),
				new InstructionDecoder(Opcode.@in, "av,dw"),
				new InstructionDecoder(Opcode.@out, "dw,ab"),
				new InstructionDecoder(Opcode.@out, "dw,av"),

				// F0
				new InstructionDecoder(Opcode.@lock),
				s_invalid,
				new F2PrefixDecoder(),
				new F3PrefixDecoder(),
				new InstructionDecoder(Opcode.hlt, InstrClass.Terminates, ""),
				new InstructionDecoder(Opcode.cmc),
				new GroupDecoder(3, "Eb"),
				new GroupDecoder(3, "Ev"),

				new InstructionDecoder(Opcode.clc),
				new InstructionDecoder(Opcode.stc),
				new InstructionDecoder(Opcode.cli),
				new InstructionDecoder(Opcode.sti),
				new InstructionDecoder(Opcode.cld),
				new InstructionDecoder(Opcode.std),
				new GroupDecoder(4, ""),
				new GroupDecoder(5, "")
			};
        }
    }
}
