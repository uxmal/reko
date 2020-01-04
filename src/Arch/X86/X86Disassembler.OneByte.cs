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
        private static Decoder[] CreateOnebyteDecoders()
        {
            return new Decoder[] { 
				// 00
				Instr(Mnemonic.add, InstrClass.Linear|InstrClass.Zero, Eb,Gb),
				Instr(Mnemonic.add, Ev,Gv),
				Instr(Mnemonic.add, Gb,Eb),
				Instr(Mnemonic.add, Gv,Ev),
				Instr(Mnemonic.add, ab,Ib),
				Instr(Mnemonic.add, av,Iz),
				new Alternative64Decoder(
                    Instr(Mnemonic.push, s0),
                    s_invalid),
				new Alternative64Decoder(
                    Instr(Mnemonic.pop, s0),
                    s_invalid),

				Instr(Mnemonic.or, Eb,Gb),
				Instr(Mnemonic.or, Ev,Gv),
				Instr(Mnemonic.or, Gb,Eb),
				Instr(Mnemonic.or, Gv,Ev),
				Instr(Mnemonic.or, ab,Ib),
				Instr(Mnemonic.or, av,Iz),
				new Alternative64Decoder(
                    Instr(Mnemonic.push, s1),
                    s_invalid),
				new AdditionalByteDecoder(),

				// 10
				Instr(Mnemonic.adc, Eb,Gb),
				Instr(Mnemonic.adc, Ev,Gv),
				Instr(Mnemonic.adc, Gb,Eb),
				Instr(Mnemonic.adc, Gv,Ev),
				Instr(Mnemonic.adc, ab,Ib),
				Instr(Mnemonic.adc, av,Iz),
				new Alternative64Decoder(
                    Instr(Mnemonic.push, s2),
                    s_invalid),
				new Alternative64Decoder(
                    Instr(Mnemonic.pop, s2),
                    s_invalid),

				Instr(Mnemonic.sbb, Eb,Gb),
				Instr(Mnemonic.sbb, Ev,Gv),
				Instr(Mnemonic.sbb, Gb,Eb),
				Instr(Mnemonic.sbb, Gv,Ev),
				Instr(Mnemonic.sbb, ab,Ib),
				Instr(Mnemonic.sbb, av,Iz),
				new Alternative64Decoder(
                    Instr(Mnemonic.push, s3),
                    s_invalid),
				new Alternative64Decoder(
                    Instr(Mnemonic.pop, s3),
                    s_invalid),

				// 20
				Instr(Mnemonic.and, Eb,Gb), 
				Instr(Mnemonic.and, Ev,Gv),
				Instr(Mnemonic.and, Gb,Eb),
				Instr(Mnemonic.and, Gv,Ev),
				Instr(Mnemonic.and, ab,Ib),
				Instr(Mnemonic.and, av,Iz),
				new SegmentOverrideDecoder(0),
				new Alternative64Decoder(
                    Instr(Mnemonic.daa),
                    s_invalid),

				Instr(Mnemonic.sub, Eb,Gb),
				Instr(Mnemonic.sub, Ev,Gv),
				Instr(Mnemonic.sub, Gb,Eb),
				Instr(Mnemonic.sub, Gv,Ev),
				Instr(Mnemonic.sub, ab,Ib),
				Instr(Mnemonic.sub, av,Iz),
                new SegmentOverrideDecoder(1),
				new Alternative64Decoder(
                    Instr(Mnemonic.das),
                    s_invalid),

				// 30
				Instr(Mnemonic.xor, Eb,Gb),
				Instr(Mnemonic.xor, Ev,Gv),
				Instr(Mnemonic.xor, Gb,Eb),
				Instr(Mnemonic.xor, Gv,Ev),
				Instr(Mnemonic.xor, ab,Ib),
				Instr(Mnemonic.xor, av,Iz),
                new SegmentOverrideDecoder(2),
				new Alternative64Decoder(
                    Instr(Mnemonic.aaa),
                    s_invalid),

				Instr(Mnemonic.cmp, Eb,Gb),
				Instr(Mnemonic.cmp, Ev,Gv),
				Instr(Mnemonic.cmp, Gb,Eb),
				Instr(Mnemonic.cmp, Gv,Ev),
				Instr(Mnemonic.cmp, ab,Ib),
				Instr(Mnemonic.cmp, av,Iz),
                new SegmentOverrideDecoder(3),
				new Alternative64Decoder(
                    Instr(Mnemonic.aas),
                    s_invalid),

				// 40
				new Rex_or_InstructionDecoder(Mnemonic.inc, rv),
				new Rex_or_InstructionDecoder(Mnemonic.inc, rv),
				new Rex_or_InstructionDecoder(Mnemonic.inc, rv),
				new Rex_or_InstructionDecoder(Mnemonic.inc, rv),
				new Rex_or_InstructionDecoder(Mnemonic.inc, rv),
				new Rex_or_InstructionDecoder(Mnemonic.inc, rv),
				new Rex_or_InstructionDecoder(Mnemonic.inc, rv),
				new Rex_or_InstructionDecoder(Mnemonic.inc, rv),

				new Rex_or_InstructionDecoder(Mnemonic.dec, rv),
				new Rex_or_InstructionDecoder(Mnemonic.dec, rv),
				new Rex_or_InstructionDecoder(Mnemonic.dec, rv),
				new Rex_or_InstructionDecoder(Mnemonic.dec, rv),
				new Rex_or_InstructionDecoder(Mnemonic.dec, rv),
				new Rex_or_InstructionDecoder(Mnemonic.dec, rv),
				new Rex_or_InstructionDecoder(Mnemonic.dec, rv),
				new Rex_or_InstructionDecoder(Mnemonic.dec, rv),

				// 50
				new Alternative64Decoder(
                    Instr(Mnemonic.push, rv),
                    Instr(Mnemonic.push, rq)),
				new Alternative64Decoder(
                    Instr(Mnemonic.push, rv),
                    Instr(Mnemonic.push, rq)),
				new Alternative64Decoder(
                    Instr(Mnemonic.push, rv),
                    Instr(Mnemonic.push, rq)),
				new Alternative64Decoder(
                    Instr(Mnemonic.push, rv),
                    Instr(Mnemonic.push, rq)),
				new Alternative64Decoder(
                    Instr(Mnemonic.push, rv),
                    Instr(Mnemonic.push, rq)),
				new Alternative64Decoder(
                    Instr(Mnemonic.push, rv),
                    Instr(Mnemonic.push, rq)),
				new Alternative64Decoder(
                    Instr(Mnemonic.push, rv),
                    Instr(Mnemonic.push, rq)),
				new Alternative64Decoder(
                    Instr(Mnemonic.push, rv),
                    Instr(Mnemonic.push, rq)),

				new Alternative64Decoder(
                    Instr(Mnemonic.pop, rv),
                    Instr(Mnemonic.pop, rq)),
				new Alternative64Decoder(
                    Instr(Mnemonic.pop, rv),
                    Instr(Mnemonic.pop, rq)),
				new Alternative64Decoder(
                    Instr(Mnemonic.pop, rv),
                    Instr(Mnemonic.pop, rq)),
				new Alternative64Decoder(
                    Instr(Mnemonic.pop, rv),
                    Instr(Mnemonic.pop, rq)),
				new Alternative64Decoder(
                    Instr(Mnemonic.pop, rv),
                    Instr(Mnemonic.pop, rq)),
				new Alternative64Decoder(
                    Instr(Mnemonic.pop, rv),
                    Instr(Mnemonic.pop, rq)),
				new Alternative64Decoder(
                    Instr(Mnemonic.pop, rv),
                    Instr(Mnemonic.pop, rq)),
				new Alternative64Decoder(
                    Instr(Mnemonic.pop, rv),
                    Instr(Mnemonic.pop, rq)),

				// 60
				new Alternative64Decoder(
                    Instr(Mnemonic.pusha),
                    s_invalid),
				new Alternative64Decoder(
                    Instr(Mnemonic.popa),
                    s_invalid),
				new Alternative64Decoder(
                    Instr(Mnemonic.bound, Gv,Mv),
                    s_invalid),
                new Alternative64Decoder(
    				Instr(Mnemonic.arpl, Ew,rw),
    				Instr(Mnemonic.movsx, Gv,Ed)),
				new SegmentOverrideDecoder(4),
				new SegmentOverrideDecoder(5),
				new ChangeDataWidth(),
				new ChangeAddressWidth(),

				Instr(Mnemonic.push, Iz),
				Instr(Mnemonic.imul, Gv,Ev,Iz),
				Instr(Mnemonic.push, Ib),
				Instr(Mnemonic.imul, Gv,Ev,Ib),
				Instr(Mnemonic.insb, b),
				Instr(Mnemonic.ins),
				Instr(Mnemonic.outsb, b),
				Instr(Mnemonic.outs),

				// 70
				Instr(Mnemonic.jo, InstrClass.Transfer|InstrClass.Conditional, Jb),
				Instr(Mnemonic.jno, InstrClass.Transfer|InstrClass.Conditional, Jb),
				Instr(Mnemonic.jc, InstrClass.Transfer|InstrClass.Conditional, Jb),
				Instr(Mnemonic.jnc, InstrClass.Transfer|InstrClass.Conditional, Jb),
				Instr(Mnemonic.jz, InstrClass.Transfer|InstrClass.Conditional, Jb),
				Instr(Mnemonic.jnz, InstrClass.Transfer|InstrClass.Conditional, Jb),
				Instr(Mnemonic.jbe, InstrClass.Transfer|InstrClass.Conditional, Jb),
				Instr(Mnemonic.ja, InstrClass.Transfer|InstrClass.Conditional, Jb),

				Instr(Mnemonic.js, InstrClass.Transfer|InstrClass.Conditional, Jb),
				Instr(Mnemonic.jns, InstrClass.Transfer|InstrClass.Conditional, Jb),
				Instr(Mnemonic.jpe, InstrClass.Transfer|InstrClass.Conditional, Jb),
				Instr(Mnemonic.jpo, InstrClass.Transfer|InstrClass.Conditional, Jb),
				Instr(Mnemonic.jl, InstrClass.Transfer|InstrClass.Conditional, Jb),
				Instr(Mnemonic.jge, InstrClass.Transfer|InstrClass.Conditional, Jb),
				Instr(Mnemonic.jle, InstrClass.Transfer|InstrClass.Conditional, Jb),
				Instr(Mnemonic.jg, InstrClass.Transfer|InstrClass.Conditional, Jb),

				// 80
				new GroupDecoder(1, Eb,Ib),
				new GroupDecoder(1, Ev,Iz),
				new Alternative64Decoder(
                    new GroupDecoder(1, Eb,Ib),
                    s_invalid),
				new GroupDecoder(1, Ev,Ib),
				Instr(Mnemonic.test, Eb,Gb),
				Instr(Mnemonic.test, Ev,Gv),
				Instr(Mnemonic.xchg, Eb,Gb),
				Instr(Mnemonic.xchg, Ev,Gv),

				Instr(Mnemonic.mov, Eb,Gb),
				Instr(Mnemonic.mov, Ev,Gv),
				Instr(Mnemonic.mov, Gb,Eb),
				Instr(Mnemonic.mov, Gv,Ev),
				Instr(Mnemonic.mov, Ew,Sw),
				Instr(Mnemonic.lea, Gv,Mv),
				Instr(Mnemonic.mov, Sw,Ew),
				Instr(Mnemonic.pop, Ev),

				// 90
				new PrefixedDecoder(
                    iclass:InstrClass.Linear|InstrClass.Padding,
                    dec:Instr(Mnemonic.nop),
                    dec66:Instr(Mnemonic.nop),
                    decF3:Instr(Mnemonic.pause)),
				Instr(Mnemonic.xchg, av,rv),
				Instr(Mnemonic.xchg, av,rv),
				Instr(Mnemonic.xchg, av,rv),
				Instr(Mnemonic.xchg, av,rv),
				Instr(Mnemonic.xchg, av,rv),
				Instr(Mnemonic.xchg, av,rv),
				Instr(Mnemonic.xchg, av,rv),

				Instr(Mnemonic.cbw),
				Instr(Mnemonic.cwd),
				new Alternative64Decoder(
                    Instr(Mnemonic.call, InstrClass.Transfer|InstrClass.Call, Ap),
                    s_invalid),
				Instr(Mnemonic.wait),
				Instr(Mnemonic.pushf),
				Instr(Mnemonic.popf),
				Instr(Mnemonic.sahf),
				Instr(Mnemonic.lahf),

				// A0
				Instr(Mnemonic.mov, ab,Ob),
				Instr(Mnemonic.mov, av,Ov),
				Instr(Mnemonic.mov, Ob,ab),
				Instr(Mnemonic.mov, Ov,av),
				Instr(Mnemonic.movsb, b),
				Instr(Mnemonic.movs),
				Instr(Mnemonic.cmpsb, b),
				Instr(Mnemonic.cmps),

				Instr(Mnemonic.test, ab,Ib),
				Instr(Mnemonic.test, av,Iz),
				Instr(Mnemonic.stosb, b),
				Instr(Mnemonic.stos),
				Instr(Mnemonic.lodsb, b),
				Instr(Mnemonic.lods),
				Instr(Mnemonic.scasb, b),
				Instr(Mnemonic.scas),

				// B0
				Instr(Mnemonic.mov, rb,Ib),
				Instr(Mnemonic.mov, rb,Ib),
				Instr(Mnemonic.mov, rb,Ib),
				Instr(Mnemonic.mov, rb,Ib),
				Instr(Mnemonic.mov, rb,Ib),
				Instr(Mnemonic.mov, rb,Ib),
				Instr(Mnemonic.mov, rb,Ib),
				Instr(Mnemonic.mov, rb,Ib),

				Instr(Mnemonic.mov, rv,Iv),
				Instr(Mnemonic.mov, rv,Iv),
				Instr(Mnemonic.mov, rv,Iv),
				Instr(Mnemonic.mov, rv,Iv),
				Instr(Mnemonic.mov, rv,Iv),
				Instr(Mnemonic.mov, rv,Iv),
				Instr(Mnemonic.mov, rv,Iv),
				Instr(Mnemonic.mov, rv,Iv),

				// C0
				new GroupDecoder(2, Eb,Ib),
				new GroupDecoder(2, Ev,Ib),
				Instr(Mnemonic.ret, InstrClass.Transfer, Iw),
				Instr(Mnemonic.ret, InstrClass.Transfer),
				new Alternative64Decoder(
                    Instr(Mnemonic.les,	Gv,Mp),
                    new VexDecoder3()),
				new Alternative64Decoder(
                    Instr(Mnemonic.lds,	Gv,Mp),
                    new VexDecoder2()),
                Instr(Mnemonic.mov,	Eb,Ib),
				Instr(Mnemonic.mov,	Ev,Iz),

				Instr(Mnemonic.enter, Iw,Ib),
				Instr(Mnemonic.leave),
				Instr(Mnemonic.retf, InstrClass.Transfer, Iw),
				Instr(Mnemonic.retf, InstrClass.Transfer),
				Instr(Mnemonic.@int, InstrClass.Linear|InstrClass.Padding, n3),
				new InterruptDecoder(Mnemonic.@int, Ib),
				new Alternative64Decoder(
                    Instr(Mnemonic.into),
                    s_invalid),
				Instr(Mnemonic.iret, InstrClass.Transfer),

				// D0
				new GroupDecoder(2, Eb,n1),
				new GroupDecoder(2, Ev,n1),
				new GroupDecoder(2, Eb,c),
				new GroupDecoder(2, Ev,c),
				new Alternative64Decoder(
                    Instr(Mnemonic.aam, Ib),
                    s_invalid),
				new Alternative64Decoder(
                    Instr(Mnemonic.aad, Ib),
				    s_invalid),
				s_invalid,
				Instr(Mnemonic.xlat, b),

				new X87Decoder(),
				new X87Decoder(),
				new X87Decoder(),
				new X87Decoder(),
				new X87Decoder(),
				new X87Decoder(),
				new X87Decoder(),
				new X87Decoder(),

				// E0
				Instr(Mnemonic.loopne, InstrClass.ConditionalTransfer, Jb),
				Instr(Mnemonic.loope, InstrClass.ConditionalTransfer, Jb),
				Instr(Mnemonic.loop, InstrClass.ConditionalTransfer, Jb),
				Instr(Mnemonic.jcxz, InstrClass.ConditionalTransfer, Jb),
				Instr(Mnemonic.@in, ab,Ib),
				Instr(Mnemonic.@in, av,Ib),
				Instr(Mnemonic.@out, Ib,ab),
				Instr(Mnemonic.@out, Ib,av),

				Instr(Mnemonic.call, InstrClass.Transfer|InstrClass.Call, Jv),
				Instr(Mnemonic.jmp, InstrClass.Transfer, Jv),
				new Alternative64Decoder(
                    Instr(Mnemonic.jmp, InstrClass.Transfer, Ap),
                    s_invalid),
				Instr(Mnemonic.jmp, InstrClass.Transfer, Jb),
				Instr(Mnemonic.@in, ab,dw),
				Instr(Mnemonic.@in, av,dw),
				Instr(Mnemonic.@out, dw,ab),
				Instr(Mnemonic.@out, dw,av),

				// F0
				Instr(Mnemonic.@lock),
				s_invalid,
				new F2PrefixDecoder(),
				new F3PrefixDecoder(),
				Instr(Mnemonic.hlt, InstrClass.Terminates),
				Instr(Mnemonic.cmc),
				new GroupDecoder(3, Eb),
				new GroupDecoder(3, Ev),

				Instr(Mnemonic.clc),
				Instr(Mnemonic.stc),
				Instr(Mnemonic.cli),
				Instr(Mnemonic.sti),
				Instr(Mnemonic.cld),
				Instr(Mnemonic.std),
				new GroupDecoder(4),
				new GroupDecoder(5)
			};
        }
    }
}
