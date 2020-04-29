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
				Instr(Mnemonic.add, AL,Ib),
				Instr(Mnemonic.add, rAX,Iz),
				Amd64Instr(
                    Instr(Mnemonic.push, s0),
                    s_invalid),
				Amd64Instr(
                    Instr(Mnemonic.pop, s0),
                    s_invalid),

				Instr(Mnemonic.or, Eb,Gb),
				Instr(Mnemonic.or, Ev,Gv),
				Instr(Mnemonic.or, Gb,Eb),
				Instr(Mnemonic.or, Gv,Ev),
				Instr(Mnemonic.or, AL,Ib),
				Instr(Mnemonic.or, rAX,Iz),
				Amd64Instr(
                    Instr(Mnemonic.push, s1),
                    s_invalid),
				new AdditionalByteDecoder(),

				// 10
				Instr(Mnemonic.adc, Eb,Gb),
				Instr(Mnemonic.adc, Ev,Gv),
				Instr(Mnemonic.adc, Gb,Eb),
				Instr(Mnemonic.adc, Gv,Ev),
				Instr(Mnemonic.adc, AL,Ib),
				Instr(Mnemonic.adc, rAX,Iz),
				Amd64Instr(
                    Instr(Mnemonic.push, s2),
                    s_invalid),
				Amd64Instr(
                    Instr(Mnemonic.pop, s2),
                    s_invalid),

				Instr(Mnemonic.sbb, Eb,Gb),
				Instr(Mnemonic.sbb, Ev,Gv),
				Instr(Mnemonic.sbb, Gb,Eb),
				Instr(Mnemonic.sbb, Gv,Ev),
				Instr(Mnemonic.sbb, AL,Ib),
				Instr(Mnemonic.sbb, rAX,Iz),
				Amd64Instr(
                    Instr(Mnemonic.push, s3),
                    s_invalid),
				Amd64Instr(
                    Instr(Mnemonic.pop, s3),
                    s_invalid),

				// 20
				Instr(Mnemonic.and, Eb,Gb), 
				Instr(Mnemonic.and, Ev,Gv),
				Instr(Mnemonic.and, Gb,Eb),
				Instr(Mnemonic.and, Gv,Ev),
				Instr(Mnemonic.and, AL,Ib),
				Instr(Mnemonic.and, rAX,Iz),
				new SegmentOverrideDecoder(0),
				Amd64Instr(
                    Instr(Mnemonic.daa),
                    s_invalid),

				Instr(Mnemonic.sub, Eb,Gb),
				Instr(Mnemonic.sub, Ev,Gv),
				Instr(Mnemonic.sub, Gb,Eb),
				Instr(Mnemonic.sub, Gv,Ev),
				Instr(Mnemonic.sub, AL,Ib),
				Instr(Mnemonic.sub, rAX,Iz),
                new SegmentOverrideDecoder(1),
				Amd64Instr(
                    Instr(Mnemonic.das),
                    s_invalid),

				// 30
				Instr(Mnemonic.xor, Eb,Gb),
				Instr(Mnemonic.xor, Ev,Gv),
				Instr(Mnemonic.xor, Gb,Eb),
				Instr(Mnemonic.xor, Gv,Ev),
				Instr(Mnemonic.xor, AL,Ib),
				Instr(Mnemonic.xor, rAX,Iz),
                new SegmentOverrideDecoder(2),
				Amd64Instr(
                    Instr(Mnemonic.aaa),
                    s_invalid),

				Instr(Mnemonic.cmp, Eb,Gb),
				Instr(Mnemonic.cmp, Ev,Gv),
				Instr(Mnemonic.cmp, Gb,Eb),
				Instr(Mnemonic.cmp, Gv,Ev),
				Instr(Mnemonic.cmp, AL,Ib),
				Instr(Mnemonic.cmp, rAX,Iz),
                new SegmentOverrideDecoder(3),
				Amd64Instr(
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
				Amd64Instr(
                    Instr(Mnemonic.push, rv),
                    Instr(Mnemonic.push, rq)),
				Amd64Instr(
                    Instr(Mnemonic.push, rv),
                    Instr(Mnemonic.push, rq)),
				Amd64Instr(
                    Instr(Mnemonic.push, rv),
                    Instr(Mnemonic.push, rq)),
				Amd64Instr(
                    Instr(Mnemonic.push, rv),
                    Instr(Mnemonic.push, rq)),
				Amd64Instr(
                    Instr(Mnemonic.push, rv),
                    Instr(Mnemonic.push, rq)),
				Amd64Instr(
                    Instr(Mnemonic.push, rv),
                    Instr(Mnemonic.push, rq)),
				Amd64Instr(
                    Instr(Mnemonic.push, rv),
                    Instr(Mnemonic.push, rq)),
				Amd64Instr(
                    Instr(Mnemonic.push, rv),
                    Instr(Mnemonic.push, rq)),

				Amd64Instr(
                    Instr(Mnemonic.pop, rv),
                    Instr(Mnemonic.pop, rq)),
				Amd64Instr(
                    Instr(Mnemonic.pop, rv),
                    Instr(Mnemonic.pop, rq)),
				Amd64Instr(
                    Instr(Mnemonic.pop, rv),
                    Instr(Mnemonic.pop, rq)),
				Amd64Instr(
                    Instr(Mnemonic.pop, rv),
                    Instr(Mnemonic.pop, rq)),
				Amd64Instr(
                    Instr(Mnemonic.pop, rv),
                    Instr(Mnemonic.pop, rq)),
				Amd64Instr(
                    Instr(Mnemonic.pop, rv),
                    Instr(Mnemonic.pop, rq)),
				Amd64Instr(
                    Instr(Mnemonic.pop, rv),
                    Instr(Mnemonic.pop, rq)),
				Amd64Instr(
                    Instr(Mnemonic.pop, rv),
                    Instr(Mnemonic.pop, rq)),

				// 60
				Amd64Instr(
                    Instr(Mnemonic.pusha),
                    s_invalid),
				Amd64Instr(
                    Instr(Mnemonic.popa),
                    s_invalid),
				Amd64Instr(
                    Instr(Mnemonic.bound, Gv,Mv),
                    new EvexDecoder()),
                Amd64Instr(
    				Instr(Mnemonic.arpl, Ew,rw),
    				Instr(Mnemonic.movsxd, Gv,Ed)),
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
				new GroupDecoder(Grp1, Eb,Ib),
				new GroupDecoder(Grp1, Ev,Iz),
				Amd64Instr(
                    new GroupDecoder(Grp1, Eb,Ib),
                    s_invalid),
				new GroupDecoder(Grp1, Ev,Ib),
				Instr(Mnemonic.test, Eb,Gb),
				Instr(Mnemonic.test, Ev,Gv),
				Instr(Mnemonic.xchg, Eb,Gb),
				Instr(Mnemonic.xchg, Ev,Gv),

				Instr(Mnemonic.mov, Eb,Gb),
				Instr(Mnemonic.mov, Ev,Gv),
				Instr(Mnemonic.mov, Gb,Eb),
				Instr(Mnemonic.mov, Gv,Ev),

				Instr(Mnemonic.mov, Ewv,Sw),
				Instr(Mnemonic.lea, Gv,Mv),
				Instr(Mnemonic.mov, Sw,Ew),
				Amd64Instr(
                    Instr(Mnemonic.pop, Ev),
                    new GroupDecoder(Grp1A)),

				// 90
				new PrefixedDecoder(
                    iclass:InstrClass.Linear|InstrClass.Padding,
                    dec:Instr(Mnemonic.nop),
                    dec66:Instr(Mnemonic.nop),
                    decF3:Instr(Mnemonic.pause)),
				Instr(Mnemonic.xchg, rv,rAX),
				Instr(Mnemonic.xchg, rv,rAX),
				Instr(Mnemonic.xchg, rv,rAX),
				Instr(Mnemonic.xchg, rv,rAX),
				Instr(Mnemonic.xchg, rv,rAX),
				Instr(Mnemonic.xchg, rv,rAX),
				Instr(Mnemonic.xchg, rv,rAX),

				DataWidthDependent(
                    bit16:Instr(Mnemonic.cbw),
                    bit32:Instr(Mnemonic.cwde),
                    bit64:Instr(Mnemonic.cdqe)),
				DataWidthDependent(
                    bit16:Instr(Mnemonic.cwd),
                    bit32:Instr(Mnemonic.cdq),
                    bit64:Instr(Mnemonic.cqo)),
				Amd64Instr(
                    Instr(Mnemonic.call, InstrClass.Transfer|InstrClass.Call, Ap),
                    s_invalid),
				Instr(Mnemonic.wait),
				Instr(Mnemonic.pushf),
				Instr(Mnemonic.popf),
				Instr(Mnemonic.sahf),
				Instr(Mnemonic.lahf),

				// A0
				Instr(Mnemonic.mov, AL,Ob),
				Instr(Mnemonic.mov, rAX,Ov),
				Instr(Mnemonic.mov, Ob,AL),
				Instr(Mnemonic.mov, Ov,rAX),
				Instr(Mnemonic.movsb, b),
				Instr(Mnemonic.movs),
				Instr(Mnemonic.cmpsb, b),
				Instr(Mnemonic.cmps),

				Instr(Mnemonic.test, AL,Ib),
				Instr(Mnemonic.test, rAX,Iz),
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
				new GroupDecoder(Grp2, Eb,Ib),
				new GroupDecoder(Grp2, Ev,Ib),
				Instr(Mnemonic.ret, InstrClass.Transfer, Iw),
				Instr(Mnemonic.ret, InstrClass.Transfer),
				Amd64Instr(
                    Instr(Mnemonic.les,	Gv,Mp),
                    new VexDecoder3()),
				Amd64Instr(
                    Instr(Mnemonic.lds,	Gv,Mp),
                    new VexDecoder2()),
                Amd64Instr(
                    Instr(Mnemonic.mov, Eb,Ib),
                    new GroupDecoder(Grp11, Eb,Ib)),
                Amd64Instr(
                    Instr(Mnemonic.mov, Ev,Iz),
                    new GroupDecoder(Grp11, Ev,Iz)),

                Instr(Mnemonic.enter, Iw,Ib),
				Instr(Mnemonic.leave),
				Instr(Mnemonic.retf, InstrClass.Transfer, Iw),
				Instr(Mnemonic.retf, InstrClass.Transfer),
				Instr(Mnemonic.@int, InstrClass.Linear|InstrClass.Padding, n3),
				new InterruptDecoder(Mnemonic.@int, Ib),
				Amd64Instr(
                    Instr(Mnemonic.into),
                    s_invalid),
				Instr(Mnemonic.iret, InstrClass.Transfer),

				// D0
				new GroupDecoder(Grp2, Eb,n1),
				new GroupDecoder(Grp2, Ev,n1),
				new GroupDecoder(Grp2, Eb,c),
				new GroupDecoder(Grp2, Ev,c),
				Amd64Instr(
                    Instr(Mnemonic.aam, Ib),
                    s_invalid),
				Amd64Instr(
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
				AddrWidthDependent(
                    bit16:Instr(Mnemonic.jcxz, InstrClass.ConditionalTransfer, Jb),
                    bit32:Instr(Mnemonic.jecxz, InstrClass.ConditionalTransfer, Jb),
                    bit64:Instr(Mnemonic.jrcxz, InstrClass.ConditionalTransfer, Jb)),
                Instr(Mnemonic.@in, AL,Ib),
				Instr(Mnemonic.@in, eAX,Ib),
				Instr(Mnemonic.@out, Ib,AL),
				Instr(Mnemonic.@out, Ib,eAX),

				Instr(Mnemonic.call, InstrClass.Transfer|InstrClass.Call, Jv),
				Instr(Mnemonic.jmp, InstrClass.Transfer, Jv),
				Amd64Instr(
                    Instr(Mnemonic.jmp, InstrClass.Transfer, Ap),
                    s_invalid),
				Instr(Mnemonic.jmp, InstrClass.Transfer, Jb),
				Instr(Mnemonic.@in, AL,DX),
				Instr(Mnemonic.@in, eAX,DX),
				Instr(Mnemonic.@out, DX,AL),
				Instr(Mnemonic.@out, DX,eAX),

				// F0
				Instr(Mnemonic.@lock),
				Instr(Mnemonic.icebp, InstrClass.Invalid),
				new F2PrefixDecoder(),
				new F3PrefixDecoder(),
				Instr(Mnemonic.hlt, InstrClass.Terminates),
				Instr(Mnemonic.cmc),
				new GroupDecoder(Grp3, Eb),
				new GroupDecoder(Grp3, Ev),

				Instr(Mnemonic.clc),
				Instr(Mnemonic.stc),
				Instr(Mnemonic.cli),
				Instr(Mnemonic.sti),
				Instr(Mnemonic.cld),
				Instr(Mnemonic.std),
				new GroupDecoder(Grp4),
				new GroupDecoder(Grp5)
			};
        }
    }
}
