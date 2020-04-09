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

namespace Reko.Arch.X86
{
    public partial class X86Disassembler
    {
        private static Decoder [] CreateGroupDecoders()
        {
            return new Decoder[]
            {
				// group 1
				Instr(Mnemonic.add),
                Instr(Mnemonic.or),
                Instr(Mnemonic.adc),
                Instr(Mnemonic.sbb),
                Instr(Mnemonic.and),
                Instr(Mnemonic.sub),
                Instr(Mnemonic.xor),
                Instr(Mnemonic.cmp),

				// group 2
				Instr(Mnemonic.rol),
                Instr(Mnemonic.ror),
                Instr(Mnemonic.rcl),
                Instr(Mnemonic.rcr),
                Instr(Mnemonic.shl),
                Instr(Mnemonic.shr),
                Instr(Mnemonic.shl),
                Instr(Mnemonic.sar),

				// group 3
				Instr(Mnemonic.test, Ix),
                Instr(Mnemonic.test, Ix),
                Instr(Mnemonic.not),
                Instr(Mnemonic.neg),
                Instr(Mnemonic.mul),
                Instr(Mnemonic.imul),
                Instr(Mnemonic.div),
                Instr(Mnemonic.idiv),
				
				// group 4
				Instr(Mnemonic.inc, Eb),
                Instr(Mnemonic.dec, Eb),
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid, 

				// group 5
				Instr(Mnemonic.inc, Ev),
                Instr(Mnemonic.dec, Ev),
                new Alternative64Decoder(
                    Instr(Mnemonic.call, InstrClass.Transfer|InstrClass.Call, Ev),
                    Instr(Mnemonic.call, InstrClass.Transfer|InstrClass.Call, Eq)),
                Instr(Mnemonic.call, InstrClass.Transfer|InstrClass.Call, Ep),
                new Alternative64Decoder(
                    Instr(Mnemonic.jmp, InstrClass.Transfer, Ev),
                    Instr(Mnemonic.jmp, InstrClass.Transfer, Eq)),
                Instr(Mnemonic.jmp, InstrClass.Transfer, Ep),
                new Alternative64Decoder(
                    Instr(Mnemonic.push, Ev),
                    Instr(Mnemonic.push, Eq)),
                s_invalid,

				// group 6
				new Group6Decoder(
                    Instr(Mnemonic.sldt, InstrClass.System, Ew),
                    Instr(Mnemonic.sldt, InstrClass.System, Rv)),
                new Group6Decoder(
                    Instr(Mnemonic.str, Ew),
                    Instr(Mnemonic.str, Rw)),
                new Group6Decoder(
                    Instr(Mnemonic.lldt, InstrClass.System, Ms),
                    Instr(Mnemonic.lldt, InstrClass.System, Rw)),
                Instr(Mnemonic.ltr, Ew),
                Instr(Mnemonic.verr, Ew),
                Instr(Mnemonic.verw, Ew),
                s_invalid,
                s_invalid,

				// group 7
				new Group7Decoder(
                    Instr(Mnemonic.sgdt, Ms),
                    s_invalid,
                    Instr(Mnemonic.vmcall),
                    Instr(Mnemonic.vmlaunch),
                    Instr(Mnemonic.vmresume),
                    Instr(Mnemonic.vmxoff),
                    s_invalid,
                    s_invalid,
                    s_invalid),
                new Group7Decoder(
                    Instr(Mnemonic.sidt, Ms),
                    Instr(Mnemonic.monitor),
                    Instr(Mnemonic.mwait),
                    Instr(Mnemonic.clac),
                    Instr(Mnemonic.stac),
                    s_invalid,
                    s_invalid,
                    s_invalid,
                    s_invalid),
                new Group7Decoder(
                    Instr(Mnemonic.lgdt, InstrClass.System, Ms),

                    Instr(Mnemonic.xgetbv),
                    Instr(Mnemonic.xsetbv),
                    s_invalid,
                    s_invalid,

                    Instr(Mnemonic.vmfunc),
                    Instr(Mnemonic.xend),
                    Instr(Mnemonic.xtest),
                    s_invalid),
                new Group6Decoder(
                    Instr(Mnemonic.lidt, InstrClass.System, Ms),
                    s_invalid),

                new Group6Decoder(
                    Instr(Mnemonic.smsw, Ew),
                    Instr(Mnemonic.smsw, Rv)),
                new Group7Decoder(
                    s_invalid,

                    s_invalid,
                    s_invalid,
                    s_invalid,
                    s_invalid,
                    s_invalid,
                    s_invalid,
                    Instr(Mnemonic.rdpkru),
                    Instr(Mnemonic.wrpkru)),
                Instr(Mnemonic.lmsw, InstrClass.System, Ew),
                new Group7Decoder(
                    Instr(Mnemonic.invlpg, Mb),

                    Instr(Mnemonic.swapgs),
                    Instr(Mnemonic.rdtscp),
                    Instr(Mnemonic.monitorx),
                    Instr(Mnemonic.mwaitx),

                    s_invalid,
                    s_invalid,
                    s_invalid,
                    s_invalid),

				// group 8
				s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                Instr(Mnemonic.bt),
                Instr(Mnemonic.bts),
                Instr(Mnemonic.btr),
                Instr(Mnemonic.btc),

				// group 9
				s_invalid,
                new Group6Decoder(
                    new PrefixedDecoder(
                        new Alternative64Decoder(
                            Instr(Mnemonic.cmpxchg8b, Mq),
                            Instr(Mnemonic.cmpxchg16b, Mdq))),
                    s_invalid),
                s_invalid,
                s_invalid,

                s_invalid,
                s_invalid,
                new Group6Decoder(
                    new PrefixedDecoder(
                        Instr(Mnemonic.vmptrld, Mq),
                        dec66:Instr(Mnemonic.vmclear, Mq),
                        decF3:Instr(Mnemonic.vmxon, Mq)),
                    Instr(Mnemonic.rdrand, Rv)),
                new Group6Decoder(
                    new PrefixedDecoder(
                        dec:Instr(Mnemonic.vmptrst, Mq),
                        decF3:Instr(Mnemonic.vmptrst, Mq)),
                    Instr(Mnemonic.rdseed, Rv)),

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
                    Instr(Mnemonic.psrlw, Nq,Ib),
                    Instr(Mnemonic.vpsrlw, Hx,Ux,Ib)),
                s_invalid,
                new PrefixedDecoder(
                    Instr(Mnemonic.psraw, Nq,Ib),
                    Instr(Mnemonic.vpsraw, Hx,Ux,Ib)),
                s_invalid,
                new PrefixedDecoder(
                    Instr(Mnemonic.psllw, Nq,Ib),
                    Instr(Mnemonic.vpsllw, Hx,Ux,Ib)),
                s_invalid,

				// group 13
				s_invalid,
                s_invalid,
                new PrefixedDecoder(
                    Instr(Mnemonic.psrld, Nq,Ib),
                    Instr(Mnemonic.vpsrld, Hx,Ux,Ib)),
                s_invalid,

                new PrefixedDecoder(
                    Instr(Mnemonic.psrad, Nq,Ib),
                    Instr(Mnemonic.vpsrad, Hx,Ux,Ib)),
                s_invalid,
                new PrefixedDecoder(
                    Instr(Mnemonic.pslld, Nq,Ib),
                    Instr(Mnemonic.vpslld, Hx,Ux,Ib)),
                s_invalid,

				// group 14
				s_invalid,
                s_invalid,
                new PrefixedDecoder(
                    Instr(Mnemonic.psrlq, Nq,Ib),
                    Instr(Mnemonic.vpsrlq, Hx,Ux,Ib)),
                new PrefixedDecoder(
                    s_invalid,
                    Instr(Mnemonic.vpsrldq, Hx,Ux,Ib)),

                s_invalid,
                s_invalid,
                new PrefixedDecoder(
                    Instr(Mnemonic.psllq, Nq,Ib),
                    Instr(Mnemonic.vpsllq, Hx,Ux,Ib)),
                new PrefixedDecoder(
                    s_invalid,
                    Instr(Mnemonic.vpslldq, Hx,Ux,Ib)),

				// group 15
				new Group7Decoder(Instr(Mnemonic.fxsave)),
                new Group7Decoder(Instr(Mnemonic.fxrstor)),
                Instr(Mnemonic.ldmxcsr, Md),
                Instr(Mnemonic.stmxcsr, Md),

                new Alternative64Decoder(
                    Instr(Mnemonic.xsave, Mb),
                    Instr(Mnemonic.xsave64, Mb)),
				new Group7Decoder(
                    Instr(Mnemonic.xrstor, Md),

                    Instr(Mnemonic.lfence),
                    Instr(Mnemonic.lfence),
                    Instr(Mnemonic.lfence),
                    Instr(Mnemonic.lfence),

                    Instr(Mnemonic.lfence),
                    Instr(Mnemonic.lfence),
                    Instr(Mnemonic.lfence),
                    Instr(Mnemonic.lfence)),
                new Group7Decoder(
                    Instr(Mnemonic.xsaveopt, Md),

                    Instr(Mnemonic.mfence),
                    Instr(Mnemonic.mfence),
                    Instr(Mnemonic.mfence),
                    Instr(Mnemonic.mfence),

                    Instr(Mnemonic.mfence),
                    Instr(Mnemonic.mfence),
                    Instr(Mnemonic.mfence),
                    Instr(Mnemonic.mfence)),
                new Group7Decoder(
                    Instr(Mnemonic.clflush, Md),

                    Instr(Mnemonic.sfence),
                    Instr(Mnemonic.sfence),
                    Instr(Mnemonic.sfence),
                    Instr(Mnemonic.sfence),

                    Instr(Mnemonic.sfence),
                    Instr(Mnemonic.sfence),
                    Instr(Mnemonic.sfence),
                    Instr(Mnemonic.sfence)),

				// group 16
				Instr(Mnemonic.prefetchnta, Mb),
				Instr(Mnemonic.prefetcht0, Mb),
				Instr(Mnemonic.prefetcht1, Mb),
				Instr(Mnemonic.prefetcht2, Mb),
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
