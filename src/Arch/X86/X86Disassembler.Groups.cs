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

namespace Reko.Arch.X86
{
    public partial class X86Disassembler
    {
        private static Decoder [] CreateGroupDecoders()
        {
            return new Decoder[]
            {
				// group 1
				Instr(Opcode.add),
                Instr(Opcode.or),
                Instr(Opcode.adc),
                Instr(Opcode.sbb),
                Instr(Opcode.and),
                Instr(Opcode.sub),
                Instr(Opcode.xor),
                Instr(Opcode.cmp),

				// group 2
				Instr(Opcode.rol),
                Instr(Opcode.ror),
                Instr(Opcode.rcl),
                Instr(Opcode.rcr),
                Instr(Opcode.shl),
                Instr(Opcode.shr),
                Instr(Opcode.shl),
                Instr(Opcode.sar),

				// group 3
				Instr(Opcode.test, Ix),
                Instr(Opcode.test, Ix),
                Instr(Opcode.not),
                Instr(Opcode.neg),
                Instr(Opcode.mul),
                Instr(Opcode.imul),
                Instr(Opcode.div),
                Instr(Opcode.idiv),
				
				// group 4
				Instr(Opcode.inc, Eb),
                Instr(Opcode.dec, Eb),
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid, 

				// group 5
				Instr(Opcode.inc, Ev),
                Instr(Opcode.dec, Ev),
                new Alternative64Decoder(
                    Instr(Opcode.call, InstrClass.Transfer|InstrClass.Call, Ev),
                    Instr(Opcode.call, InstrClass.Transfer|InstrClass.Call, Eq)),
                Instr(Opcode.call, InstrClass.Transfer|InstrClass.Call, Ep),
                new Alternative64Decoder(
                    Instr(Opcode.jmp, InstrClass.Transfer, Ev),
                    Instr(Opcode.jmp, InstrClass.Transfer, Eq)),
                Instr(Opcode.jmp, InstrClass.Transfer, Ep),
                new Alternative64Decoder(
                    Instr(Opcode.push, Ev),
                    Instr(Opcode.push, Eq)),
                s_invalid,

				// group 6
				new Group6Decoder(
                    Instr(Opcode.sldt, InstrClass.System, Ew),
                    Instr(Opcode.sldt, InstrClass.System, Rv)),
                new Group6Decoder(
                    Instr(Opcode.str, Ew),
                    Instr(Opcode.str, Rv)),
                Instr(Opcode.lldt, InstrClass.System, Ms),
                Instr(Opcode.ltr, Ew),
                Instr(Opcode.verr, Ew),
                Instr(Opcode.verw, Ew),
                s_invalid,
                s_invalid,

				// group 7
				new Group7Decoder(
                    Instr(Opcode.sgdt, Ms),
                    s_invalid,
                    Instr(Opcode.vmcall),
                    Instr(Opcode.vmlaunch),
                    Instr(Opcode.vmresume),
                    Instr(Opcode.vmxoff),
                    s_invalid,
                    s_invalid,
                    s_invalid),
                new Group7Decoder(
                    Instr(Opcode.sidt, Ms),
                    Instr(Opcode.monitor),
                    Instr(Opcode.mwait),
                    Instr(Opcode.clac),
                    Instr(Opcode.stac),
                    s_invalid,
                    s_invalid,
                    s_invalid,
                    s_invalid),
                new Group7Decoder(
                    Instr(Opcode.lgdt, InstrClass.System, Ms),

                    Instr(Opcode.xgetbv),
                    Instr(Opcode.xsetbv),
                    s_invalid,
                    s_invalid,

                    Instr(Opcode.vmfunc),
                    Instr(Opcode.xend),
                    Instr(Opcode.xtest),
                    s_invalid),
                new Group6Decoder(
                    Instr(Opcode.lidt, InstrClass.System, Ms),
                    s_invalid),

                new Group6Decoder(
                    Instr(Opcode.smsw, Ew),
                    Instr(Opcode.smsw, Rv)),
                new Group7Decoder(
                    s_invalid,

                    s_invalid,
                    s_invalid,
                    s_invalid,
                    s_invalid,
                    s_invalid,
                    s_invalid,
                    Instr(Opcode.rdpkru),
                    Instr(Opcode.wrpkru)),
                Instr(Opcode.lmsw, Ew),
                new Group7Decoder(
                    Instr(Opcode.invlpg, Mb),

                    Instr(Opcode.swapgs),
                    Instr(Opcode.rdtscp),
                    Instr(Opcode.monitorx),
                    Instr(Opcode.mwaitx),

                    s_invalid,
                    s_invalid,
                    s_invalid,
                    s_invalid),

				// group 8
				s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                Instr(Opcode.bt),
                Instr(Opcode.bts),
                Instr(Opcode.btr),
                Instr(Opcode.btc),

				// group 9
				s_invalid,
                new Group6Decoder(
                    new PrefixedDecoder(
                        new Alternative64Decoder(
                            Instr(Opcode.cmpxchg8b, Mq),
                            Instr(Opcode.cmpxchg16b, Mdq))),
                    s_invalid),
                s_invalid,
                s_invalid,

                s_invalid,
                s_invalid,
                new Group6Decoder(
                    new PrefixedDecoder(
                        Instr(Opcode.vmptrld, Mq),
                        dec66:Instr(Opcode.vmclear, Mq),
                        decF3:Instr(Opcode.vmxon, Mq)),
                    Instr(Opcode.rdrand, Rv)),
                new Group6Decoder(
                    new PrefixedDecoder(
                        dec:Instr(Opcode.vmptrst, Mq),
                        decF3:Instr(Opcode.vmptrst, Mq)),
                    Instr(Opcode.rdseed, Rv)),

				// group 10
				s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,

				// group 11
				s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,
                s_nyi,

				// group 12
				s_invalid,
                s_invalid,
                new PrefixedDecoder(
                    Instr(Opcode.psrlw, Nq,Ib),
                    Instr(Opcode.vpsrlw, Hx,Ux,Ib)),
                s_invalid,
                new PrefixedDecoder(
                    Instr(Opcode.psraw, Nq,Ib),
                    Instr(Opcode.vpsraw, Hx,Ux,Ib)),
                s_invalid,
                new PrefixedDecoder(
                    Instr(Opcode.psllw, Nq,Ib),
                    Instr(Opcode.vpsllw, Hx,Ux,Ib)),
                s_invalid,

				// group 13
				s_invalid,
                s_invalid,
                new PrefixedDecoder(
                    Instr(Opcode.psrld, Nq,Ib),
                    Instr(Opcode.vpsrld, Hx,Ux,Ib)),
                s_invalid,

                new PrefixedDecoder(
                    Instr(Opcode.psrad, Nq,Ib),
                    Instr(Opcode.vpsrad, Hx,Ux,Ib)),
                s_invalid,
                new PrefixedDecoder(
                    Instr(Opcode.pslld, Nq,Ib),
                    Instr(Opcode.vpslld, Hx,Ux,Ib)),
                s_invalid,

				// group 14
				s_invalid,
                s_invalid,
                new PrefixedDecoder(
                    Instr(Opcode.psrlq, Nq,Ib),
                    Instr(Opcode.vpsrlq, Hx,Ux,Ib)),
                new PrefixedDecoder(
                    s_invalid,
                    Instr(Opcode.vpsrldq, Hx,Ux,Ib)),

                s_invalid,
                s_invalid,
                new PrefixedDecoder(
                    Instr(Opcode.psllq, Nq,Ib),
                    Instr(Opcode.vpsllq, Hx,Ux,Ib)),
                new PrefixedDecoder(
                    s_invalid,
                    Instr(Opcode.vpslldq, Hx,Ux,Ib)),

				// group 15
				new Group7Decoder(Instr(Opcode.fxsave)),
                new Group7Decoder(Instr(Opcode.fxrstor)),
                Instr(Opcode.ldmxcsr, Md),
                Instr(Opcode.stmxcsr, Md),

                new Alternative64Decoder(
                    Instr(Opcode.xsave, Mb),
                    Instr(Opcode.xsave64, Mb)),
				new Group7Decoder(
                    Instr(Opcode.xrstor, Md),

                    Instr(Opcode.lfence),
                    Instr(Opcode.lfence),
                    Instr(Opcode.lfence),
                    Instr(Opcode.lfence),

                    Instr(Opcode.lfence),
                    Instr(Opcode.lfence),
                    Instr(Opcode.lfence),
                    Instr(Opcode.lfence)),
                new Group7Decoder(
                    Instr(Opcode.xsaveopt, Md),

                    Instr(Opcode.mfence),
                    Instr(Opcode.mfence),
                    Instr(Opcode.mfence),
                    Instr(Opcode.mfence),

                    Instr(Opcode.mfence),
                    Instr(Opcode.mfence),
                    Instr(Opcode.mfence),
                    Instr(Opcode.mfence)),
                new Group7Decoder(
                    Instr(Opcode.clflush, Md),

                    Instr(Opcode.sfence),
                    Instr(Opcode.sfence),
                    Instr(Opcode.sfence),
                    Instr(Opcode.sfence),

                    Instr(Opcode.sfence),
                    Instr(Opcode.sfence),
                    Instr(Opcode.sfence),
                    Instr(Opcode.sfence)),

				// group 16
				Instr(Opcode.prefetchnta, Mb),
				Instr(Opcode.prefetcht0, Mb),
				Instr(Opcode.prefetcht1, Mb),
				Instr(Opcode.prefetcht2, Mb),
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,

				// group 17
				s_invalid,
				s_nyi,
				s_nyi,
				s_nyi,
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
			};
        }
    }
}
