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
        private static void CreateGroupDecoders()
        {
            Grp1 = new Decoder[8]
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
            };

            Grp1A = new Decoder[8]
            {
                Instr(Mnemonic.pop, Ev),
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
            };

            Grp2 = new Decoder[8]
            {
                Instr(Mnemonic.rol),
                Instr(Mnemonic.ror),
                Instr(Mnemonic.rcl),
                Instr(Mnemonic.rcr),
                Instr(Mnemonic.shl),
                Instr(Mnemonic.shr),
                Instr(Mnemonic.shl),
                Instr(Mnemonic.sar),
            };

            Grp3 = new Decoder[8]
            {
                Instr(Mnemonic.test, Ix),
                Instr(Mnemonic.test, Ix),
                Instr(Mnemonic.not),
                Instr(Mnemonic.neg),
                Instr(Mnemonic.mul),
                Instr(Mnemonic.imul),
                Instr(Mnemonic.div),
                Instr(Mnemonic.idiv),
            };

            Grp4 = new Decoder[8]
            {
                Instr(Mnemonic.inc, Eb),
                Instr(Mnemonic.dec, Eb),
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
            };

            Grp5 = new Decoder[8]
            {
                Instr(Mnemonic.inc, Ev),
                Instr(Mnemonic.dec, Ev),
                Amd64Instr(
                    Instr(Mnemonic.call, InstrClass.Transfer|InstrClass.Call, Ev),
                    Instr(Mnemonic.call, InstrClass.Transfer|InstrClass.Call, Eq)),
                Instr(Mnemonic.call, InstrClass.Transfer|InstrClass.Call, Ep),
                Amd64Instr(
                    Instr(Mnemonic.jmp, InstrClass.Transfer, Ev),
                    Instr(Mnemonic.jmp, InstrClass.Transfer, Eq)),
                Instr(Mnemonic.jmp, InstrClass.Transfer, Ep),
                Amd64Instr(
                    Instr(Mnemonic.push, Ev),
                    Instr(Mnemonic.push, Eq)),
                s_invalid,
            };

            // 0F 00
            Grp6 = new Decoder[8]
            {
                MemReg(
                    Instr(Mnemonic.sldt, InstrClass.System, Ew),
                    Instr(Mnemonic.sldt, InstrClass.System, Rv)),
                MemReg(
                    Instr(Mnemonic.str, InstrClass.System, Ew),
                    Instr(Mnemonic.str, InstrClass.System, Rw)),
                MemReg(
                    Instr(Mnemonic.lldt, InstrClass.System, Ms),
                    Instr(Mnemonic.lldt, InstrClass.System, Rw)),
                Instr(Mnemonic.ltr, InstrClass.System, Ew),
                Instr(Mnemonic.verr, Ew),
                Instr(Mnemonic.verw, Ew),
                s_invalid,
                s_invalid,
            };

            // 0F 01
            Grp7 = new Decoder[8]
            {
                new Group7Decoder(
                    Instr(Mnemonic.sgdt, Ms),
                    Instr(Mnemonic.monitor),
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
                MemReg(
                    Instr(Mnemonic.lidt, InstrClass.System, Ms),
                    s_invalid),

                MemReg(
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
                    Instr(Mnemonic.invlpg, InstrClass.System, Mb),

                    Instr(Mnemonic.swapgs),
                    Instr(Mnemonic.rdtscp),
                    Instr(Mnemonic.monitorx),
                    Instr(Mnemonic.mwaitx),

                    s_invalid,
                    s_invalid,
                    s_invalid,
                    s_invalid),
            };

            // 0F BA
            Grp8 = new Decoder[8]
            {
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                Instr(Mnemonic.bt),
                Instr(Mnemonic.bts),
                Instr(Mnemonic.btr),
                Instr(Mnemonic.btc),
            };

            // 0F C7
            Grp9 = new Decoder[8]
            {
                s_invalid,
                MemReg(
                    new PrefixedDecoder(
                        Amd64Instr(
                            Instr(Mnemonic.cmpxchg8b, Mq),
                            Instr(Mnemonic.cmpxchg16b, Mdq))),
                    s_invalid),
                s_invalid,
                s_invalid,

                s_invalid,
                s_invalid,
                MemReg(
                    new PrefixedDecoder(
                        Instr(Mnemonic.vmptrld, Mq),
                        dec66:Instr(Mnemonic.vmclear, Mq),
                        decF3:Instr(Mnemonic.vmxon, Mq)),
                    Instr(Mnemonic.rdrand, Rv)),
                MemReg(
                    new PrefixedDecoder(
                        dec:Instr(Mnemonic.vmptrst, Mq),
                        decF3:Instr(Mnemonic.vmptrst, Mq)),
                    Instr(Mnemonic.rdseed, Rv)),
            };

            // 0F B9
            Grp10 = new Decoder[8]
            {
                Instr(Mnemonic.ud1, InstrClass.Invalid),
				Instr(Mnemonic.ud1, InstrClass.Invalid),
				Instr(Mnemonic.ud1, InstrClass.Invalid),
				Instr(Mnemonic.ud1, InstrClass.Invalid),
				Instr(Mnemonic.ud1, InstrClass.Invalid),
				Instr(Mnemonic.ud1, InstrClass.Invalid),
				Instr(Mnemonic.ud1, InstrClass.Invalid),
				Instr(Mnemonic.ud1, InstrClass.Invalid),
            };

            // C6/C7
            Grp11b = new Decoder[8]
            {
                Instr(Mnemonic.mov, Eb,Ib),
                s_invalid,
                s_invalid,      // Some of these appear to be "secret" instructions.
                s_invalid,
                s_invalid,
                s_invalid,
                s_invalid,
                new Group7Decoder(
                    s_invalid,
                    Instr(Mnemonic.xabort, InstrClass.Terminates, Ib))
            };
            Grp11z = new Decoder[8]
            {
                Instr(Mnemonic.mov, Ev,Iz),
                s_invalid,
                s_invalid,
                s_invalid,      // Some of these appear to be "secret" instructions.
                s_invalid,
                s_invalid,
                s_invalid,
                new Group7Decoder(
                    s_invalid,
                    Instr(Mnemonic.xabort, InstrClass.Terminates, Ib))
            };

            // 0F 71
            Grp12 = new Decoder[8]
            {
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
            };

            // 0F 72
            Grp13 = new Decoder[8]
            {
                s_invalid,
                s_invalid,
                new PrefixedDecoder(
                    Instr(Mnemonic.psrld, Nq,Ib),
                    VexInstr(Mnemonic.psrld, Mnemonic.vpsrld, Hx,Ux,Ib)),
                s_invalid,

                new PrefixedDecoder(
                    Instr(Mnemonic.psrad, Nq,Ib),
                    Instr(Mnemonic.vpsrad, Hx,Ux,Ib)),
                s_invalid,
                new PrefixedDecoder(
                    Instr(Mnemonic.pslld, Nq,Ib),
                    VexInstr(Mnemonic.pslld, Mnemonic.vpslld, Hx,Ux,Ib)),
                s_invalid,
            };

            // 0F 73
            Grp14 = new Decoder[8]
            {
                s_invalid,
                s_invalid,
                new PrefixedDecoder(
                    Instr(Mnemonic.psrlq, Nq,Ib),
                    VexInstr(Mnemonic.psrlq, Mnemonic.vpsrlq, Hx,Ux,Ib)),
                new PrefixedDecoder(
                    s_invalid,
                    VexInstr(Mnemonic.psrldq, Mnemonic.vpsrldq, Hx,Ux,Ib)),

                s_invalid,
                s_invalid,
                new PrefixedDecoder(
                    Instr(Mnemonic.psllq, Nq,Ib),
                    VexInstr(Mnemonic.psllq, Mnemonic.vpsllq, Hx,Ux,Ib)),
                new PrefixedDecoder(
                    s_invalid,
                    VexInstr(Mnemonic.pslldq, Mnemonic.vpslldq, Hx,Ux,Ib)),
            };

            // 0F AE
            Grp15 = new Decoder[8]
            {
                new Group7Decoder(Instr(Mnemonic.fxsave)),
                new Group7Decoder(Instr(Mnemonic.fxrstor)),
                Instr(Mnemonic.ldmxcsr, Md),
                Instr(Mnemonic.stmxcsr, Md),

                Amd64Instr(
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
            };

            // 0F 18
            Grp16 = new Decoder[8]
            {
                Instr(Mnemonic.prefetchnta, Mb),
				Instr(Mnemonic.prefetcht0, Mb),
				Instr(Mnemonic.prefetcht1, Mb),
				Instr(Mnemonic.prefetcht2, Mb),
                Instr(Mnemonic.nop, InstrClass.Linear|InstrClass.Padding),
                Instr(Mnemonic.nop, InstrClass.Linear|InstrClass.Padding),
                Instr(Mnemonic.nop, InstrClass.Linear|InstrClass.Padding),
                Instr(Mnemonic.nop, InstrClass.Linear|InstrClass.Padding)
            };

            // VEX.0F38 F3
            Grp17 = new Decoder[8]
            {
                s_invalid,
				VexInstr(Mnemonic.illegal, Mnemonic.blsr, By, Ey),
                VexInstr(Mnemonic.illegal, Mnemonic.blsmsk, By, Ey),
                VexInstr(Mnemonic.illegal, Mnemonic.blsi, By, Ey),
				s_invalid,
				s_invalid,
				s_invalid,
				s_invalid,
			};
        }
    }
}
