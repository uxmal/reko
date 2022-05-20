#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
        public partial class InstructionSet
        {
            private void CreateGroupDecoders()
            {
                Grp1[0] = Instr(Mnemonic.add);
                Grp1[1] = Instr386(Mnemonic.or);
                Grp1[2] = Instr(Mnemonic.adc);
                Grp1[3] = Instr(Mnemonic.sbb);
                Grp1[4] = Instr386(Mnemonic.and);
                Grp1[5] = Instr(Mnemonic.sub);
                Grp1[6] = Instr386(Mnemonic.xor);
                Grp1[7] = Instr(Mnemonic.cmp);

                Grp1A[0] = Instr(Mnemonic.pop, Ev);
                Grp1A[1] = s_invalid;
                Grp1A[2] = s_invalid;
                Grp1A[3] = s_invalid;
                Grp1A[4] = s_invalid;
                Grp1A[5] = s_invalid;
                Grp1A[6] = s_invalid;
                Grp1A[7] = s_invalid;

                Grp2[0] = Instr(Mnemonic.rol);
                Grp2[1] = Instr(Mnemonic.ror);
                Grp2[2] = Instr(Mnemonic.rcl);
                Grp2[3] = Instr(Mnemonic.rcr);
                Grp2[4] = Instr(Mnemonic.shl);
                Grp2[5] = Instr(Mnemonic.shr);
                Grp2[6] = Instr(Mnemonic.shl);
                Grp2[7] = Instr(Mnemonic.sar);

                Grp3[0] = Instr(Mnemonic.test, Ix);
                Grp3[1] = Instr(Mnemonic.test, Ix);
                Grp3[2] = Instr(Mnemonic.not);
                Grp3[3] = Instr(Mnemonic.neg);
                Grp3[4] = Instr(Mnemonic.mul);
                Grp3[5] = Instr(Mnemonic.imul);
                Grp3[6] = Instr(Mnemonic.div);
                Grp3[7] = Instr(Mnemonic.idiv);

                Grp4[0] = Instr(Mnemonic.inc, Eb);
                Grp4[1] = Instr(Mnemonic.dec, Eb);
                Grp4[2] = s_invalid;
                Grp4[3] = s_invalid;
                Grp4[4] = s_invalid;
                Grp4[5] = s_invalid;
                Grp4[6] = s_invalid;
                Grp4[7] = s_invalid;


                Grp5[0] = Instr(Mnemonic.inc, Ev);
                Grp5[1] = Instr(Mnemonic.dec, Ev);
                Grp5[2] = Amd64Instr(
                        Instr(Mnemonic.call, InstrClass.Transfer|InstrClass.Call, Ev),
                        Instr(Mnemonic.call, InstrClass.Transfer|InstrClass.Call, Eq));
                Grp5[3] = Instr(Mnemonic.call, InstrClass.Transfer|InstrClass.Call, Ep);
                Grp5[4] = Amd64Instr(
                        Instr(Mnemonic.jmp, InstrClass.Transfer, Ev),
                        Instr(Mnemonic.jmp, InstrClass.Transfer, Eq));
                Grp5[5] = Instr(Mnemonic.jmp, InstrClass.Transfer, Ep);
                Grp5[6] = Amd64Instr(
                        Instr(Mnemonic.push, Ev),
                        Instr(Mnemonic.push, Eq));
                Grp5[7] = s_invalid;

                // 0F 00
                Grp6[0] = MemReg(
                        Instr(Mnemonic.sldt, InstrClass.Linear | InstrClass.Privileged, Ms),
                        Instr(Mnemonic.sldt, InstrClass.Linear | InstrClass.Privileged, Rv));
                Grp6[1] = MemReg(
                        Instr(Mnemonic.str, InstrClass.Linear | InstrClass.Privileged, Ew),
                        Instr(Mnemonic.str, InstrClass.Linear | InstrClass.Privileged, Rw));
                Grp6[2] = MemReg(
                        Instr(Mnemonic.lldt, InstrClass.Linear | InstrClass.Privileged, Ms),
                        Instr(Mnemonic.lldt, InstrClass.Linear | InstrClass.Privileged, Rw));
                Grp6[3] = Instr(Mnemonic.ltr, InstrClass.Linear | InstrClass.Privileged, Ew);
                Grp6[4] = Instr(Mnemonic.verr, Ew);
                Grp6[5] = Instr(Mnemonic.verw, Ew);
                Grp6[6] = s_invalid;
                Grp6[7] = s_invalid;

                // 0F 01
                Grp7[0] = new Group7Decoder(
                        Instr(Mnemonic.sgdt, InstrClass.Linear | InstrClass.Privileged, Ms),
                        Instr(Mnemonic.monitor, InstrClass.Linear | InstrClass.Privileged),
                        Instr(Mnemonic.vmcall, InstrClass.Linear | InstrClass.Privileged),
                        Instr(Mnemonic.vmlaunch, InstrClass.Linear | InstrClass.Privileged),
                        Instr(Mnemonic.vmresume, InstrClass.Linear | InstrClass.Privileged),
                        Instr(Mnemonic.vmxoff, InstrClass.Linear | InstrClass.Privileged),
                        s_invalid,
                        s_invalid,
                        s_invalid);
                Grp7[1] = new Group7Decoder(
                        Instr(Mnemonic.sidt, InstrClass.Linear|InstrClass.Privileged, Ms),
                        Instr(Mnemonic.monitor, InstrClass.Linear | InstrClass.Privileged),
                        Instr(Mnemonic.mwait, InstrClass.Linear | InstrClass.Privileged),
                        Instr(Mnemonic.clac),
                        Instr(Mnemonic.stac),
                        s_invalid,
                        s_invalid,
                        s_invalid,
                        s_invalid);
                Grp7[2] = new Group7Decoder(
                        Instr(Mnemonic.lgdt, InstrClass.Linear | InstrClass.Privileged, Ms),

                        Instr(Mnemonic.xgetbv),
                        Instr(Mnemonic.xsetbv, InstrClass.Linear | InstrClass.Privileged),
                        s_invalid,
                        s_invalid,

                        Instr(Mnemonic.vmfunc),
                        Instr(Mnemonic.xend),
                        Instr(Mnemonic.xtest),
                        s_invalid);
                Grp7[3] = MemReg(
                        Instr(Mnemonic.lidt, InstrClass.Linear | InstrClass.Privileged, Ms),
                        s_invalid);

                Grp7[4] = MemReg(
                        Instr(Mnemonic.smsw, Ew),
                        Instr(Mnemonic.smsw, Rv));
                Grp7[5] = new Group7Decoder(
                        s_invalid,

                        s_invalid,
                        s_invalid,
                        s_invalid,
                        s_invalid,
                        s_invalid,
                        s_invalid,
                        Instr(Mnemonic.rdpkru),
                        Instr(Mnemonic.wrpkru));
                Grp7[6] = Instr(Mnemonic.lmsw, InstrClass.Linear | InstrClass.Privileged, Ew);
                Grp7[7] = new Group7Decoder(
                        Instr(Mnemonic.invlpg, InstrClass.Linear | InstrClass.Privileged, Mb),

                        Instr(Mnemonic.swapgs),
                        Instr(Mnemonic.rdtscp),
                        Instr(Mnemonic.monitorx),
                        Instr(Mnemonic.mwaitx),

                        s_invalid,
                        s_invalid,
                        s_invalid,
                        s_invalid);

                // 0F BA
                Grp8[0] = s_invalid;
                Grp8[1] = s_invalid;
                Grp8[2] = s_invalid;
                Grp8[3] = s_invalid;
                Grp8[4] = Instr(Mnemonic.bt);
                Grp8[5] = Instr(Mnemonic.bts);
                Grp8[6] = Instr(Mnemonic.btr);
                Grp8[7] = Instr(Mnemonic.btc);

                // 0F C7
                Grp9[0] = s_invalid;
                Grp9[1] = MemReg(
                        new PrefixedDecoder(
                            Amd64Instr(
                                Instr(Mnemonic.cmpxchg8b, Mq),
                                Instr(Mnemonic.cmpxchg16b, Mdq))),
                        s_invalid);
                Grp9[2] = s_invalid;
                Grp9[3] = s_invalid;

                Grp9[4] = s_invalid;
                Grp9[5] = s_invalid;
                Grp9[6] = MemReg(
                        new PrefixedDecoder(
                            Instr(Mnemonic.vmptrld, Mq),
                            dec66: Instr(Mnemonic.vmclear, InstrClass.Linear|InstrClass.Privileged, Mq),
                            decF3: Instr(Mnemonic.vmxon, InstrClass.Linear | InstrClass.Privileged, Mq)),
                        Instr(Mnemonic.rdrand, RBv));
                Grp9[7] = MemReg(
                        new PrefixedDecoder(
                            dec:Instr(Mnemonic.vmptrst, InstrClass.Linear | InstrClass.Privileged, Mq),
                            decF3:Instr(Mnemonic.vmptrst, InstrClass.Linear | InstrClass.Privileged, Mq)),
                        Instr(Mnemonic.rdseed, RBv));

                // 0F B9
                Grp10[0] = Instr(Mnemonic.ud1, InstrClass.Invalid);
                Grp10[1] = Instr(Mnemonic.ud1, InstrClass.Invalid);
                Grp10[2] = Instr(Mnemonic.ud1, InstrClass.Invalid);
                Grp10[3] = Instr(Mnemonic.ud1, InstrClass.Invalid);
                Grp10[4] = Instr(Mnemonic.ud1, InstrClass.Invalid);
                Grp10[5] = Instr(Mnemonic.ud1, InstrClass.Invalid);
                Grp10[6] = Instr(Mnemonic.ud1, InstrClass.Invalid);
                Grp10[7] = Instr(Mnemonic.ud1, InstrClass.Invalid);

                // C6/C7
                Grp11b[0] = Instr(Mnemonic.mov, Eb, Ib);
                Grp11b[1] = s_invalid;
                Grp11b[2] = s_invalid;      // Some of these appear to be "secret" instructions.
                Grp11b[3] = s_invalid;
                Grp11b[4] = s_invalid;
                Grp11b[5] = s_invalid;
                Grp11b[6] = s_invalid;
                Grp11b[7] = new Group7Decoder(
                    s_invalid,
                    Instr(Mnemonic.xabort, InstrClass.Terminates, Ib));

                Grp11z[0] = Instr(Mnemonic.mov, Ev, Iz);
                Grp11z[1] = s_invalid;
                Grp11z[2] = s_invalid;
                Grp11z[3] = s_invalid;      // Some of these appear to be "secret" instructions.
                Grp11z[4] = s_invalid;
                Grp11z[5] = s_invalid;
                Grp11z[6] = s_invalid;
                Grp11z[7] = new Group7Decoder(
                    s_invalid,
                    Instr(Mnemonic.xabort, InstrClass.Terminates, Ib));

                // 0F 71
                Grp12[0] = s_invalid;
                Grp12[1] = s_invalid;
                Grp12[2] = new PrefixedDecoder(
                        Instr(Mnemonic.psrlw, Nq, Ib),
                        Instr(Mnemonic.vpsrlw, Hx, Ux, Ib));
                Grp12[3] = s_invalid;
                Grp12[4] = new PrefixedDecoder(
                        Instr(Mnemonic.psraw, Nq, Ib),
                        Instr(Mnemonic.vpsraw, Hx, Ux, Ib));
                Grp12[5] = s_invalid;
                Grp12[6] = new PrefixedDecoder(
                        Instr(Mnemonic.psllw, Nq, Ib),
                        Instr(Mnemonic.vpsllw, Hx, Ux, Ib));
                Grp12[7] = s_invalid;

                // 0F 72
                Grp13[0] = s_invalid;
                Grp13[1] = s_invalid;
                Grp13[2] = new PrefixedDecoder(
                    Instr(Mnemonic.psrld, Nq,Ib),
                    VexInstr(Mnemonic.psrld, Mnemonic.vpsrld, Hx,Ux,Ib));
                Grp13[3] = s_invalid;

                Grp13[4] = new PrefixedDecoder(
                    Instr(Mnemonic.psrad, Nq,Ib),
                    Instr(Mnemonic.vpsrad, Hx,Ux,Ib));
                Grp13[5] = s_invalid;
                Grp13[6] = new PrefixedDecoder(
                    Instr(Mnemonic.pslld, Nq,Ib),
                    VexInstr(Mnemonic.pslld, Mnemonic.vpslld, Hx,Ux,Ib));
                Grp13[7] = s_invalid;

                // 0F 73
                Grp14[0] = s_invalid;
                Grp14[1] = s_invalid;
                Grp14[2] = new PrefixedDecoder(
                        Instr(Mnemonic.psrlq, Nq,Ib),
                        VexInstr(Mnemonic.psrlq, Mnemonic.vpsrlq, Hx,Ux,Ib));
                Grp14[3] = new PrefixedDecoder(
                        s_invalid,
                        VexInstr(Mnemonic.psrldq, Mnemonic.vpsrldq, Hx,Ux,Ib));

                Grp14[4] = s_invalid;
                Grp14[5] = s_invalid;
                Grp14[6] = new PrefixedDecoder(
                    Instr(Mnemonic.psllq, Nq,Ib),
                    VexInstr(Mnemonic.psllq, Mnemonic.vpsllq, Hx,Ux,Ib));
                Grp14[7] = new PrefixedDecoder(
                        s_invalid,
                        VexInstr(Mnemonic.pslldq, Mnemonic.vpslldq, Hx,Ux,Ib));

                // 0F AE
                Grp15[0] = new Group7Decoder(Instr(Mnemonic.fxsave));
                Grp15[1] = new Group7Decoder(Instr(Mnemonic.fxrstor));
                Grp15[2] = VexInstr(Mnemonic.ldmxcsr, Mnemonic.vldmxcsr, Md);
                Grp15[3] = VexInstr(Mnemonic.stmxcsr, Mnemonic.vstmxcsr, Md);

                Grp15[4] = Amd64Instr(
                    Instr(Mnemonic.xsave, Mb),
                    Instr(Mnemonic.xsave64, Mb));
                Grp15[5] = new Group7Decoder(
                    Instr(Mnemonic.xrstor, Md),

                    Instr(Mnemonic.lfence),
                    Instr(Mnemonic.lfence),
                    Instr(Mnemonic.lfence),
                    Instr(Mnemonic.lfence),
                 
                    Instr(Mnemonic.lfence),
                    Instr(Mnemonic.lfence),
                    Instr(Mnemonic.lfence),
                    Instr(Mnemonic.lfence));
                Grp15[6] = new Group7Decoder(
                    Instr(Mnemonic.xsaveopt, Md),

                    Instr(Mnemonic.mfence),
                    Instr(Mnemonic.mfence),
                    Instr(Mnemonic.mfence),
                    Instr(Mnemonic.mfence),

                    Instr(Mnemonic.mfence),
                    Instr(Mnemonic.mfence),
                    Instr(Mnemonic.mfence),
                    Instr(Mnemonic.mfence));
                Grp15[7] = new Group7Decoder(
                    Instr(Mnemonic.clflush, Md),

                    Instr(Mnemonic.sfence),
                    Instr(Mnemonic.sfence),
                    Instr(Mnemonic.sfence),
                    Instr(Mnemonic.sfence),

                    Instr(Mnemonic.sfence),
                    Instr(Mnemonic.sfence),
                    Instr(Mnemonic.sfence),
                    Instr(Mnemonic.sfence));

                // 0F 18
                Grp16[0] = Instr(Mnemonic.prefetchnta, Mb);
                Grp16[1] = Instr(Mnemonic.prefetcht0, Mb);
                Grp16[2] = Instr(Mnemonic.prefetcht1, Mb);
                Grp16[3] = Instr(Mnemonic.prefetcht2, Mb);
                Grp16[4] = Instr(Mnemonic.nop, InstrClass.Linear|InstrClass.Padding);
                Grp16[5] = Instr(Mnemonic.nop, InstrClass.Linear|InstrClass.Padding);
                Grp16[6] = Instr(Mnemonic.nop, InstrClass.Linear|InstrClass.Padding);
                Grp16[7] = Instr(Mnemonic.nop, InstrClass.Linear|InstrClass.Padding);

                // VEX.0F38 F3
                Grp17[0] = s_invalid;
                Grp17[1] = VexInstr(Mnemonic.illegal, Mnemonic.blsr, By, Ey);
                Grp17[2] = VexInstr(Mnemonic.illegal, Mnemonic.blsmsk, By, Ey);
                Grp17[3] = VexInstr(Mnemonic.illegal, Mnemonic.blsi, By, Ey);
                Grp17[4] = s_invalid;
                Grp17[5] = s_invalid;
                Grp17[6] = s_invalid;
                Grp17[7] = s_invalid;
            }
        }
    }
}
