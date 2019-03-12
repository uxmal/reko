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
        private static Decoder[] CreateOnebyteDecoders()
        {
            return new Decoder[] { 
				// 00
				Instr(Opcode.add, InstrClass.Linear|InstrClass.Zero, Eb,Gb),
				Instr(Opcode.add, Ev,Gv),
				Instr(Opcode.add, Gb,Eb),
				Instr(Opcode.add, Gv,Ev),
				Instr(Opcode.add, ab,Ib),
				Instr(Opcode.add, av,Iz),
				new Alternative64Decoder(
                    Instr(Opcode.push, s0),
                    s_invalid),
				new Alternative64Decoder(
                    Instr(Opcode.pop, s0),
                    s_invalid),

				Instr(Opcode.or, Eb,Gb),
				Instr(Opcode.or, Ev,Gv),
				Instr(Opcode.or, Gb,Eb),
				Instr(Opcode.or, Gv,Ev),
				Instr(Opcode.or, ab,Ib),
				Instr(Opcode.or, av,Iz),
				new Alternative64Decoder(
                    Instr(Opcode.push, s1),
                    s_invalid),
				new AdditionalByteDecoder(),

				// 10
				Instr(Opcode.adc, Eb,Gb),
				Instr(Opcode.adc, Ev,Gv),
				Instr(Opcode.adc, Gb,Eb),
				Instr(Opcode.adc, Gv,Ev),
				Instr(Opcode.adc, ab,Ib),
				Instr(Opcode.adc, av,Iz),
				new Alternative64Decoder(
                    Instr(Opcode.push, s2),
                    s_invalid),
				new Alternative64Decoder(
                    Instr(Opcode.pop, s2),
                    s_invalid),

				Instr(Opcode.sbb, Eb,Gb),
				Instr(Opcode.sbb, Ev,Gv),
				Instr(Opcode.sbb, Gb,Eb),
				Instr(Opcode.sbb, Gv,Ev),
				Instr(Opcode.sbb, ab,Ib),
				Instr(Opcode.sbb, av,Iz),
				new Alternative64Decoder(
                    Instr(Opcode.push, s3),
                    s_invalid),
				new Alternative64Decoder(
                    Instr(Opcode.pop, s3),
                    s_invalid),

				// 20
				Instr(Opcode.and, Eb,Gb), 
				Instr(Opcode.and, Ev,Gv),
				Instr(Opcode.and, Gb,Eb),
				Instr(Opcode.and, Gv,Ev),
				Instr(Opcode.and, ab,Ib),
				Instr(Opcode.and, av,Iz),
				new SegmentOverrideDecoder(0),
				new Alternative64Decoder(
                    Instr(Opcode.daa),
                    s_invalid),

				Instr(Opcode.sub, Eb,Gb),
				Instr(Opcode.sub, Ev,Gv),
				Instr(Opcode.sub, Gb,Eb),
				Instr(Opcode.sub, Gv,Ev),
				Instr(Opcode.sub, ab,Ib),
				Instr(Opcode.sub, av,Iz),
                new SegmentOverrideDecoder(1),
				new Alternative64Decoder(
                    Instr(Opcode.das),
                    s_invalid),

				// 30
				Instr(Opcode.xor, Eb,Gb),
				Instr(Opcode.xor, Ev,Gv),
				Instr(Opcode.xor, Gb,Eb),
				Instr(Opcode.xor, Gv,Ev),
				Instr(Opcode.xor, ab,Ib),
				Instr(Opcode.xor, av,Iz),
                new SegmentOverrideDecoder(2),
				new Alternative64Decoder(
                    Instr(Opcode.aaa),
                    s_invalid),

				Instr(Opcode.cmp, Eb,Gb),
				Instr(Opcode.cmp, Ev,Gv),
				Instr(Opcode.cmp, Gb,Eb),
				Instr(Opcode.cmp, Gv,Ev),
				Instr(Opcode.cmp, ab,Ib),
				Instr(Opcode.cmp, av,Iz),
                new SegmentOverrideDecoder(3),
				new Alternative64Decoder(
                    Instr(Opcode.aas),
                    s_invalid),

				// 40
				new Rex_or_InstructionDecoder(Opcode.inc, rv),
				new Rex_or_InstructionDecoder(Opcode.inc, rv),
				new Rex_or_InstructionDecoder(Opcode.inc, rv),
				new Rex_or_InstructionDecoder(Opcode.inc, rv),
				new Rex_or_InstructionDecoder(Opcode.inc, rv),
				new Rex_or_InstructionDecoder(Opcode.inc, rv),
				new Rex_or_InstructionDecoder(Opcode.inc, rv),
				new Rex_or_InstructionDecoder(Opcode.inc, rv),

				new Rex_or_InstructionDecoder(Opcode.dec, rv),
				new Rex_or_InstructionDecoder(Opcode.dec, rv),
				new Rex_or_InstructionDecoder(Opcode.dec, rv),
				new Rex_or_InstructionDecoder(Opcode.dec, rv),
				new Rex_or_InstructionDecoder(Opcode.dec, rv),
				new Rex_or_InstructionDecoder(Opcode.dec, rv),
				new Rex_or_InstructionDecoder(Opcode.dec, rv),
				new Rex_or_InstructionDecoder(Opcode.dec, rv),

				// 50
				new Alternative64Decoder(
                    Instr(Opcode.push, rv),
                    Instr(Opcode.push, rq)),
				new Alternative64Decoder(
                    Instr(Opcode.push, rv),
                    Instr(Opcode.push, rq)),
				new Alternative64Decoder(
                    Instr(Opcode.push, rv),
                    Instr(Opcode.push, rq)),
				new Alternative64Decoder(
                    Instr(Opcode.push, rv),
                    Instr(Opcode.push, rq)),
				new Alternative64Decoder(
                    Instr(Opcode.push, rv),
                    Instr(Opcode.push, rq)),
				new Alternative64Decoder(
                    Instr(Opcode.push, rv),
                    Instr(Opcode.push, rq)),
				new Alternative64Decoder(
                    Instr(Opcode.push, rv),
                    Instr(Opcode.push, rq)),
				new Alternative64Decoder(
                    Instr(Opcode.push, rv),
                    Instr(Opcode.push, rq)),

				new Alternative64Decoder(
                    Instr(Opcode.pop, rv),
                    Instr(Opcode.pop, rq)),
				new Alternative64Decoder(
                    Instr(Opcode.pop, rv),
                    Instr(Opcode.pop, rq)),
				new Alternative64Decoder(
                    Instr(Opcode.pop, rv),
                    Instr(Opcode.pop, rq)),
				new Alternative64Decoder(
                    Instr(Opcode.pop, rv),
                    Instr(Opcode.pop, rq)),
				new Alternative64Decoder(
                    Instr(Opcode.pop, rv),
                    Instr(Opcode.pop, rq)),
				new Alternative64Decoder(
                    Instr(Opcode.pop, rv),
                    Instr(Opcode.pop, rq)),
				new Alternative64Decoder(
                    Instr(Opcode.pop, rv),
                    Instr(Opcode.pop, rq)),
				new Alternative64Decoder(
                    Instr(Opcode.pop, rv),
                    Instr(Opcode.pop, rq)),

				// 60
				new Alternative64Decoder(
                    Instr(Opcode.pusha),
                    s_invalid),
				new Alternative64Decoder(
                    Instr(Opcode.popa),
                    s_invalid),
				new Alternative64Decoder(
                    Instr(Opcode.bound, Gv,Mv),
                    s_invalid),
                new Alternative64Decoder(
    				Instr(Opcode.arpl, Ew,rw),
    				Instr(Opcode.movsx, Gv,Ed)),
				new SegmentOverrideDecoder(4),
				new SegmentOverrideDecoder(5),
				new ChangeDataWidth(),
				new ChangeAddressWidth(),

				Instr(Opcode.push, Iz),
				Instr(Opcode.imul, Gv,Ev,Iz),
				Instr(Opcode.push, Ib),
				Instr(Opcode.imul, Gv,Ev,Ib),
				Instr(Opcode.insb, b),
				Instr(Opcode.ins),
				Instr(Opcode.outsb, b),
				Instr(Opcode.outs),

				// 70
				Instr(Opcode.jo, InstrClass.Transfer|InstrClass.Conditional, Jb),
				Instr(Opcode.jno, InstrClass.Transfer|InstrClass.Conditional, Jb),
				Instr(Opcode.jc, InstrClass.Transfer|InstrClass.Conditional, Jb),
				Instr(Opcode.jnc, InstrClass.Transfer|InstrClass.Conditional, Jb),
				Instr(Opcode.jz, InstrClass.Transfer|InstrClass.Conditional, Jb),
				Instr(Opcode.jnz, InstrClass.Transfer|InstrClass.Conditional, Jb),
				Instr(Opcode.jbe, InstrClass.Transfer|InstrClass.Conditional, Jb),
				Instr(Opcode.ja, InstrClass.Transfer|InstrClass.Conditional, Jb),

				Instr(Opcode.js, InstrClass.Transfer|InstrClass.Conditional, Jb),
				Instr(Opcode.jns, InstrClass.Transfer|InstrClass.Conditional, Jb),
				Instr(Opcode.jpe, InstrClass.Transfer|InstrClass.Conditional, Jb),
				Instr(Opcode.jpo, InstrClass.Transfer|InstrClass.Conditional, Jb),
				Instr(Opcode.jl, InstrClass.Transfer|InstrClass.Conditional, Jb),
				Instr(Opcode.jge, InstrClass.Transfer|InstrClass.Conditional, Jb),
				Instr(Opcode.jle, InstrClass.Transfer|InstrClass.Conditional, Jb),
				Instr(Opcode.jg, InstrClass.Transfer|InstrClass.Conditional, Jb),

				// 80
				new GroupDecoder(1, Eb,Ib),
				new GroupDecoder(1, Ev,Iz),
				new Alternative64Decoder(
                    new GroupDecoder(1, Eb,Ib),
                    s_invalid),
				new GroupDecoder(1, Ev,Ib),
				Instr(Opcode.test, Eb,Gb),
				Instr(Opcode.test, Ev,Gv),
				Instr(Opcode.xchg, Eb,Gb),
				Instr(Opcode.xchg, Ev,Gv),

				Instr(Opcode.mov, Eb,Gb),
				Instr(Opcode.mov, Ev,Gv),
				Instr(Opcode.mov, Gb,Eb),
				Instr(Opcode.mov, Gv,Ev),
				Instr(Opcode.mov, Ew,Sw),
				Instr(Opcode.lea, Gv,Mv),
				Instr(Opcode.mov, Sw,Ew),
				Instr(Opcode.pop, Ev),

				// 90
				new PrefixedDecoder(
                    iclass:InstrClass.Linear|InstrClass.Padding,
                    dec:Instr(Opcode.nop),
                    dec66:Instr(Opcode.nop),
                    decF3:Instr(Opcode.pause)),
				Instr(Opcode.xchg, av,rv),
				Instr(Opcode.xchg, av,rv),
				Instr(Opcode.xchg, av,rv),
				Instr(Opcode.xchg, av,rv),
				Instr(Opcode.xchg, av,rv),
				Instr(Opcode.xchg, av,rv),
				Instr(Opcode.xchg, av,rv),

				Instr(Opcode.cbw),
				Instr(Opcode.cwd),
				new Alternative64Decoder(
                    Instr(Opcode.call, InstrClass.Transfer|InstrClass.Call, Ap),
                    s_invalid),
				Instr(Opcode.wait),
				Instr(Opcode.pushf),
				Instr(Opcode.popf),
				Instr(Opcode.sahf),
				Instr(Opcode.lahf),

				// A0
				Instr(Opcode.mov, ab,Ob),
				Instr(Opcode.mov, av,Ov),
				Instr(Opcode.mov, Ob,ab),
				Instr(Opcode.mov, Ov,av),
				Instr(Opcode.movsb, b),
				Instr(Opcode.movs),
				Instr(Opcode.cmpsb, b),
				Instr(Opcode.cmps),

				Instr(Opcode.test, ab,Ib),
				Instr(Opcode.test, av,Iz),
				Instr(Opcode.stosb, b),
				Instr(Opcode.stos),
				Instr(Opcode.lodsb, b),
				Instr(Opcode.lods),
				Instr(Opcode.scasb, b),
				Instr(Opcode.scas),

				// B0
				Instr(Opcode.mov, rb,Ib),
				Instr(Opcode.mov, rb,Ib),
				Instr(Opcode.mov, rb,Ib),
				Instr(Opcode.mov, rb,Ib),
				Instr(Opcode.mov, rb,Ib),
				Instr(Opcode.mov, rb,Ib),
				Instr(Opcode.mov, rb,Ib),
				Instr(Opcode.mov, rb,Ib),

				Instr(Opcode.mov, rv,Iv),
				Instr(Opcode.mov, rv,Iv),
				Instr(Opcode.mov, rv,Iv),
				Instr(Opcode.mov, rv,Iv),
				Instr(Opcode.mov, rv,Iv),
				Instr(Opcode.mov, rv,Iv),
				Instr(Opcode.mov, rv,Iv),
				Instr(Opcode.mov, rv,Iv),

				// C0
				new GroupDecoder(2, Eb,Ib),
				new GroupDecoder(2, Ev,Ib),
				Instr(Opcode.ret, InstrClass.Transfer, Iw),
				Instr(Opcode.ret, InstrClass.Transfer),
				new Alternative64Decoder(
                    Instr(Opcode.les,	Gv,Mp),
                    new VexDecoder3()),
				new Alternative64Decoder(
                    Instr(Opcode.lds,	Gv,Mp),
                    new VexDecoder2()),
                Instr(Opcode.mov,	Eb,Ib),
				Instr(Opcode.mov,	Ev,Iz),

				Instr(Opcode.enter, Iw,Ib),
				Instr(Opcode.leave),
				Instr(Opcode.retf, InstrClass.Transfer, Iw),
				Instr(Opcode.retf, InstrClass.Transfer),
				Instr(Opcode.@int, InstrClass.Linear|InstrClass.Padding, n3),
				new InterruptDecoder(Opcode.@int, Ib),
				new Alternative64Decoder(
                    Instr(Opcode.into),
                    s_invalid),
				Instr(Opcode.iret, InstrClass.Transfer),

				// D0
				new GroupDecoder(2, Eb,n1),
				new GroupDecoder(2, Ev,n1),
				new GroupDecoder(2, Eb,c),
				new GroupDecoder(2, Ev,c),
				new Alternative64Decoder(
                    Instr(Opcode.aam, Ib),
                    s_invalid),
				new Alternative64Decoder(
                    Instr(Opcode.aad, Ib),
				    s_invalid),
				s_invalid,
				Instr(Opcode.xlat, b),

				new X87Decoder(),
				new X87Decoder(),
				new X87Decoder(),
				new X87Decoder(),
				new X87Decoder(),
				new X87Decoder(),
				new X87Decoder(),
				new X87Decoder(),

				// E0
				Instr(Opcode.loopne, InstrClass.ConditionalTransfer, Jb),
				Instr(Opcode.loope, InstrClass.ConditionalTransfer, Jb),
				Instr(Opcode.loop, InstrClass.ConditionalTransfer, Jb),
				Instr(Opcode.jcxz, InstrClass.ConditionalTransfer, Jb),
				Instr(Opcode.@in, ab,Ib),
				Instr(Opcode.@in, av,Ib),
				Instr(Opcode.@out, Ib,ab),
				Instr(Opcode.@out, Ib,av),

				Instr(Opcode.call, InstrClass.Transfer|InstrClass.Call, Jv),
				Instr(Opcode.jmp, InstrClass.Transfer, Jv),
				new Alternative64Decoder(
                    Instr(Opcode.jmp, InstrClass.Transfer, Ap),
                    s_invalid),
				Instr(Opcode.jmp, InstrClass.Transfer, Jb),
				Instr(Opcode.@in, ab,dw),
				Instr(Opcode.@in, av,dw),
				Instr(Opcode.@out, dw,ab),
				Instr(Opcode.@out, dw,av),

				// F0
				Instr(Opcode.@lock),
				s_invalid,
				new F2PrefixDecoder(),
				new F3PrefixDecoder(),
				Instr(Opcode.hlt, InstrClass.Terminates),
				Instr(Opcode.cmc),
				new GroupDecoder(3, Eb),
				new GroupDecoder(3, Ev),

				Instr(Opcode.clc),
				Instr(Opcode.stc),
				Instr(Opcode.cli),
				Instr(Opcode.sti),
				Instr(Opcode.cld),
				Instr(Opcode.std),
				new GroupDecoder(4),
				new GroupDecoder(5)
			};
        }
    }
}
