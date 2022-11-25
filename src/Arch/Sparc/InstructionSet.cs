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
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using static Reko.Arch.Sparc.SparcDisassembler;

namespace Reko.Arch.Sparc
{
    using Decoder = Decoder<SparcDisassembler, Mnemonic, SparcInstruction>;

    public class InstructionSet
    {
        private const InstrClass Transfer = InstrClass.Delay | InstrClass.Transfer;
        private const InstrClass CondTransfer = InstrClass.Delay | InstrClass.Transfer | InstrClass.Conditional;
        private const InstrClass LinkTransfer = InstrClass.Delay | InstrClass.Transfer | InstrClass.Call;

        private readonly bool is64Bit;

        public InstructionSet(bool is64Bit)
        {
            this.is64Bit = is64Bit;
        }

        public static Decoder Create32BitDecoder()
        {
            var iset = new InstructionSet(false);
            return iset.CreateDecoder();
        }

        public static Decoder Create64BitDecoder()
        {
            var iset = new InstructionSet(true);
            return iset.CreateDecoder();
        }

        internal static Decoder Instr(Mnemonic mnemonic, params Mutator<SparcDisassembler>[] mutators)
        {
            return new InstrDecoder<SparcDisassembler, Mnemonic, SparcInstruction>(InstrClass.Linear, mnemonic, mutators);
        }

        internal static Decoder Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<SparcDisassembler>[] mutators)
        {
            return new InstrDecoder<SparcDisassembler, Mnemonic, SparcInstruction>(iclass, mnemonic, mutators);
        }

        /// <summary>
        /// Build a 32- or 64-bit decoder depending on the current 64-bitness.
        /// </summary>
        private Decoder Instr64(Decoder decoder64, Decoder decoder32)
        {
            return this.is64Bit ? decoder64 : decoder32;
        }

        Decoder CreateDecoder()
        {
            Decoder invalid = Instr(Mnemonic.illegal, InstrClass.Invalid, nyi);

            var branchOps = new Decoder[]
            {
                // 00
                Instr(Mnemonic.bn, CondTransfer, J22),
                Instr(Mnemonic.be, CondTransfer, J22),
                Instr(Mnemonic.ble, CondTransfer, J22),
                Instr(Mnemonic.bl, CondTransfer, J22),
                Instr(Mnemonic.bleu, CondTransfer, J22),
                Instr(Mnemonic.bcs, CondTransfer, J22),
                Instr(Mnemonic.bneg, CondTransfer, J22),
                Instr(Mnemonic.bvs, CondTransfer, J22),

                Instr(Mnemonic.ba, CondTransfer, J22),
                Instr(Mnemonic.bne, CondTransfer, J22),
                Instr(Mnemonic.bg, CondTransfer, J22),
                Instr(Mnemonic.bge, CondTransfer, J22),
                Instr(Mnemonic.bgu, CondTransfer, J22),
                Instr(Mnemonic.bcc, CondTransfer, J22),
                Instr(Mnemonic.bpos, CondTransfer, J22),
                Instr(Mnemonic.bvc, CondTransfer, J22),

                // 10
                Instr(Mnemonic.fbn, CondTransfer, J22),
                Instr(Mnemonic.fbne, CondTransfer, J22),
                Instr(Mnemonic.fblg, CondTransfer, J22),
                Instr(Mnemonic.fbul, CondTransfer, J22),
                Instr(Mnemonic.fbug, CondTransfer, J22),
                Instr(Mnemonic.fbg, CondTransfer, J22),
                Instr(Mnemonic.fbu, CondTransfer, J22),
                Instr(Mnemonic.fbug, CondTransfer, J22),

                Instr(Mnemonic.fba, CondTransfer, J22),
                Instr(Mnemonic.fbe, CondTransfer, J22),
                Instr(Mnemonic.fbue, CondTransfer, J22),
                Instr(Mnemonic.fbge, CondTransfer, J22),
                Instr(Mnemonic.fbuge, CondTransfer, J22),
                Instr(Mnemonic.fble, CondTransfer, J22),
                Instr(Mnemonic.fbule, CondTransfer, J22),
                Instr(Mnemonic.fbo, CondTransfer, J22),

                // 20
                Instr(Mnemonic.cbn, J22),
                Instr(Mnemonic.cb123, J22),
                Instr(Mnemonic.cb12, J22),
                Instr(Mnemonic.cb13, J22),
                Instr(Mnemonic.cb1, J22),
                Instr(Mnemonic.cb23, J22),
                Instr(Mnemonic.cb2, J22),
                Instr(Mnemonic.cb3, J22),

                Instr(Mnemonic.cba, J22),
                Instr(Mnemonic.cb0, J22),
                Instr(Mnemonic.cb03, J22),
                Instr(Mnemonic.cb02, J22),
                Instr(Mnemonic.cb023, J22),
                Instr(Mnemonic.cb01, J22),
                Instr(Mnemonic.cb013, J22),
                Instr(Mnemonic.cb012, J22),

                // 30
                Instr(Mnemonic.tn, r14,T),
                Instr(Mnemonic.te, r14,T),
                Instr(Mnemonic.tle, r14,T),
                Instr(Mnemonic.tl, r14,T),
                Instr(Mnemonic.tleu, r14,T),
                Instr(Mnemonic.tcs, r14,T),
                Instr(Mnemonic.tneg, r14,T),
                Instr(Mnemonic.tvs, r14,T),

                Instr(Mnemonic.ta, r14,T),
                Instr(Mnemonic.tne, r14,T),
                Instr(Mnemonic.tg, r14,T),
                Instr(Mnemonic.tge, r14,T),
                Instr(Mnemonic.tgu, r14,T),
                Instr(Mnemonic.tcc, r14,T),
                Instr(Mnemonic.tpos, r14,T),
                Instr(Mnemonic.tvc, r14,T),
            };
            var bp_cc = new Decoder[]
            {
                // 00
                Instr(Mnemonic.bn, CondTransfer, Pred, J19),
                Instr(Mnemonic.be, CondTransfer, Pred, J19),
                Instr(Mnemonic.ble, CondTransfer, Pred, J19),
                Instr(Mnemonic.bl, CondTransfer, Pred, J19),
                Instr(Mnemonic.bleu, CondTransfer, Pred, J19),
                Instr(Mnemonic.bcs, CondTransfer, Pred, J19),
                Instr(Mnemonic.bneg, CondTransfer, Pred, J19),
                Instr(Mnemonic.bvs, CondTransfer, Pred, J19),

                Instr(Mnemonic.ba, CondTransfer, Pred, J19),
                Instr(Mnemonic.bne, CondTransfer, Pred, J19),
                Instr(Mnemonic.bg, CondTransfer, Pred, J19),
                Instr(Mnemonic.bge, CondTransfer, Pred, J19),
                Instr(Mnemonic.bgu, CondTransfer, Pred, J19),
                Instr(Mnemonic.bcc, CondTransfer, Pred, J19),
                Instr(Mnemonic.bpos, CondTransfer, Pred, J19),
                Instr(Mnemonic.bvc, CondTransfer, Pred, J19),
            };
            var bp_bicc = new Decoder[]
            {
                Instr(Mnemonic.bn, CondTransfer, J22),
                Instr(Mnemonic.be, CondTransfer, J22),
                Instr(Mnemonic.ble, CondTransfer, J22),
                Instr(Mnemonic.bl, CondTransfer, J22),
                Instr(Mnemonic.bleu, CondTransfer, J22),
                Instr(Mnemonic.bcs, CondTransfer, J22),
                Instr(Mnemonic.bneg, CondTransfer, J22),
                Instr(Mnemonic.bvs, CondTransfer, J22),

                Instr(Mnemonic.ba, CondTransfer, J22),
                Instr(Mnemonic.bne, CondTransfer, J22),
                Instr(Mnemonic.bg, CondTransfer, J22),
                Instr(Mnemonic.bge, CondTransfer, J22),
                Instr(Mnemonic.bgu, CondTransfer, J22),
                Instr(Mnemonic.bcc, CondTransfer, J22),
                Instr(Mnemonic.bpos, CondTransfer, J22),
                Instr(Mnemonic.bvc, CondTransfer, J22),
            };
            var bp_r = new Decoder[]
            {
                invalid,
                Instr(Mnemonic.brz, CondTransfer, Pred, r14, J2_16),
                Instr(Mnemonic.brlez, CondTransfer, Pred, r14, J2_16),
                Instr(Mnemonic.brlz, CondTransfer, Pred, r14, J2_16),
                invalid,
                Instr(Mnemonic.brnz, CondTransfer, Pred, r14, J2_16),
                Instr(Mnemonic.brgz, CondTransfer, Pred, r14, J2_16),
                Instr(Mnemonic.brgez, CondTransfer, Pred, r14, J2_16),

                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid
            };
            var fbp_fcc = new Decoder[]
            {
                Instr(Mnemonic.fbn, CondTransfer, Pred, J19),
                Instr(Mnemonic.fbne, CondTransfer, Pred, J19),
                Instr(Mnemonic.fblg, CondTransfer, Pred, J19),
                Instr(Mnemonic.fbul, CondTransfer, Pred, J19),
                Instr(Mnemonic.fbl, CondTransfer, Pred, J19),
                Instr(Mnemonic.fbug, CondTransfer, Pred, J19),
                Instr(Mnemonic.fbg, CondTransfer, Pred, J19),
                Instr(Mnemonic.fbu, CondTransfer, Pred, J19),

                Instr(Mnemonic.fba, CondTransfer, Pred, J19),
                Instr(Mnemonic.fbe, CondTransfer, Pred, J19),
                Instr(Mnemonic.fbue, CondTransfer, Pred, J19),
                Instr(Mnemonic.fbge, CondTransfer, Pred, J19),
                Instr(Mnemonic.fbuge, CondTransfer, Pred, J19),
                Instr(Mnemonic.fble, CondTransfer, Pred, J19),
                Instr(Mnemonic.bpos, CondTransfer, Pred, J19),
                Instr(Mnemonic.fbo, CondTransfer, Pred, J19),
            };

            var fb_fcc = new Decoder[]
            {
                Instr(Mnemonic.tn, r14,T),
                Instr(Mnemonic.te, r14,T),
                Instr(Mnemonic.tle, r14,T),
                Instr(Mnemonic.tl, r14,T),
                Instr(Mnemonic.tleu, r14,T),
                Instr(Mnemonic.tcs, r14,T),
                Instr(Mnemonic.tneg, r14,T),
                Instr(Mnemonic.tvs, r14,T),

                Instr(Mnemonic.ta, r14,T),
                Instr(Mnemonic.tne, r14,T),
                Instr(Mnemonic.tg, r14,T),
                Instr(Mnemonic.tge, r14,T),
                Instr(Mnemonic.tgu, r14,T),
                Instr(Mnemonic.tcc, r14,T),
                Instr(Mnemonic.tpos, r14,T),
                Instr(Mnemonic.tvc, r14,T),
            };

            var decoders_0 = new Decoder[]
            {
                Instr(Mnemonic.unimp, InstrClass.Invalid),
                Instr64(
                    new BranchDecoder(bp_cc, 0x00),
                    invalid),
                Instr64(
                    new BranchDecoder(bp_bicc, 0x00),
                    new BranchDecoder(branchOps, 0x00)),
                Instr64(
                    new BranchDecoder(bp_r, 0x00),
                    invalid),

                Instr(Mnemonic.sethi, I,r25),
                Instr64(
                    new BranchDecoder(fbp_fcc, 0x00),
                    invalid),
                Instr64(
                    new BranchDecoder(fb_fcc, 0x00),
                    new BranchDecoder(branchOps, 0x10)),
                Instr64(
                    invalid,
                    new BranchDecoder(branchOps, 0x20))
            };

            var fpDecoders = new (uint, Decoder)[]
            {
                // 00 
                (0x01, Instr(Mnemonic.fmovs, f0, f25)),
                (0x05, Instr(Mnemonic.fnegs, f0, f25)),
                (0x09, Instr(Mnemonic.fabss, f0, f25)),
                (0x29, Instr(Mnemonic.fsqrts, f0, f25)),
                (0x2A, Instr(Mnemonic.fsqrtd, d0, d25)),
                (0x2B, Instr(Mnemonic.fsqrtq, q0, q25)),

                (0x41, Instr(Mnemonic.fadds, f14, f0, f25)),
                (0x42, Instr(Mnemonic.faddd, d14, d0, d25)),
                (0x43, Instr(Mnemonic.faddq, q14, q0, q25)),
                (0x45, Instr(Mnemonic.fsubs, f14, f0, f25)),
                (0x46, Instr(Mnemonic.fsubd, d14, d0, d25)),
                (0x47, Instr(Mnemonic.fsubq, q14, q0, q25)),

                (0xC4, Instr(Mnemonic.fitos, f0, f25)),
                (0xC6, Instr(Mnemonic.fdtos, d0, f25)),
                (0xC7, Instr(Mnemonic.fqtos, q0, f25)),
                (0xC8, Instr(Mnemonic.fitod, f0, d25)),
                (0xC9, Instr(Mnemonic.fstod, f0, d25)),
                (0xCB, Instr(Mnemonic.fqtod, q0, d25)),
                (0xCC, Instr(Mnemonic.fitoq, f0, q25)),
                (0xCD, Instr(Mnemonic.fstoq, f0, q25)),
                (0xCE, Instr(Mnemonic.fdtoq, d0, q25)),
                (0xD1, Instr(Mnemonic.fstoi, f0, f25)),
                (0xD2, Instr(Mnemonic.fdtoi, d0, f25)),
                (0xD3, Instr(Mnemonic.fqtoi, q0, f25)),

                (0x49, Instr(Mnemonic.fmuls, f14, f0, f25)),
                (0x4A, Instr(Mnemonic.fmuld, d14, d0, d25)),
                (0x4B, Instr(Mnemonic.fmulq, q14, q0, q25)),
                (0x4D, Instr(Mnemonic.fdivs, f14, f0, f25)),
                (0x4E, Instr(Mnemonic.fdivd, d14, d0, d25)),
                (0x4F, Instr(Mnemonic.fdivq, q14, q0, q25)),

                (0x69, Instr(Mnemonic.fsmuld, f14, f0, d25)),
                (0x6E, Instr(Mnemonic.fdmulq, d14, d0, q25)),

                (0x51, Instr(Mnemonic.fcmps, f14, f0)),
                (0x52, Instr(Mnemonic.fcmpd, d14, d0)),
                (0x53, Instr(Mnemonic.fcmpq, q14, q0)),
                (0x55, Instr(Mnemonic.fcmpes, f14, f0)),
                (0x56, Instr(Mnemonic.fcmped, d14, d0)),
                (0x57, Instr(Mnemonic.fcmpeq, q14, q0))
            };

            var decoders_2 = new Decoder[]
            {
                // 00
                Instr(Mnemonic.add, r14,R0,r25),
                Instr(Mnemonic.and, r14,R0,r25),
                Instr(Mnemonic.or, r14,R0,r25),
                Instr(Mnemonic.xor, r14,R0,r25),
                Instr(Mnemonic.sub, r14,R0,r25),
                Instr(Mnemonic.andn, r14,R0,r25),
                Instr(Mnemonic.orn, r14,R0,r25),
                Instr(Mnemonic.xnor, r14,R0,r25),

                Instr64(
                    Instr(Mnemonic.addc, r14,R0,r25),
                    Instr(Mnemonic.addx, r14,R0,r25)),
                Instr64(
                    Instr(Mnemonic.mulx, r14,R0,r25),
                    invalid),
                Instr(Mnemonic.umul, r14,R0,r25),
                Instr(Mnemonic.smul, r14,R0,r25),
                Instr64(
                    Instr(Mnemonic.subc, r14,R0,r25),
                    Instr(Mnemonic.subx, r14,R0,r25)),
                Instr64(
                    Instr(Mnemonic.udivx, r14,R0,r25),
                    invalid),
                Instr(Mnemonic.udiv, r14,R0,r25),
                Instr(Mnemonic.sdiv, r14,R0,r25),

                // 10
                Instr(Mnemonic.addcc, r14,R0,r25),
                Instr(Mnemonic.andcc, r14,R0,r25),
                Instr(Mnemonic.orcc, r14,R0,r25),
                Instr(Mnemonic.xorcc, r14,R0,r25),
                Instr(Mnemonic.subcc, r14,R0,r25),
                Instr(Mnemonic.andncc, r14,R0,r25),
                Instr(Mnemonic.orncc, r14,R0,r25),
                Instr(Mnemonic.xnorcc, r14,R0,r25),

                Instr64(
                    Instr(Mnemonic.addccc, r14,R0,r25),
                    Instr(Mnemonic.addxcc, r14,R0,r25)),
                invalid,
                Instr(Mnemonic.umulcc, r14,R0,r25),
                Instr(Mnemonic.smulcc, r14,R0,r25),
                Instr64(
                    Instr(Mnemonic.subxcc, r14,R0,r25),
                    Instr(Mnemonic.subxcc, r14,R0,r25)),
                invalid,
                Instr(Mnemonic.udivcc, r14,R0,r25),
                Instr(Mnemonic.sdivcc, r14,R0,r25),

                // 20
                Instr(Mnemonic.taddcc, r14,R0,r25),
                Instr(Mnemonic.tsubcc, r14,R0,r25),
                Instr(Mnemonic.taddcctv, r14,R0,r25),
                Instr(Mnemonic.tsubcctv, r14,R0,r25),
                Instr(Mnemonic.mulscc, r14,R0,r25),
                Instr64(
                    SparcDisassembler.Mask(12, 1, "  sll",
                        Instr(Mnemonic.sll, r14,S,r25),
                        Instr(Mnemonic.sllx, r14,S,r25)),
                    Instr(Mnemonic.sll, r14,S,r25)),
                Instr64(
                    SparcDisassembler.Mask(12, 1, "  srl",
                        Instr(Mnemonic.srl, r14,S,r25),
                        Instr(Mnemonic.srlx, r14,S,r25)),
                    Instr(Mnemonic.srl, r14,S,r25)),
                Instr64(
                    SparcDisassembler.Mask(12, 1, "  sra",
                        Instr(Mnemonic.sra, r14,S,r25),
                        Instr(Mnemonic.srax, r14,S,r25)),
                    Instr(Mnemonic.sra, r14,S,r25)),

                Instr64(
                    Instr(Mnemonic.rd, ry,r25), // ****
                    Instr(Mnemonic.rd, ry,r25)),
                Instr64(
                    invalid,
                    Instr(Mnemonic.rdpsr, nyi)),
                Instr64(
                    Instr(Mnemonic.rdpr, InstrClass.Linear|InstrClass.Privileged, nyi),
                    Instr(Mnemonic.rdtbr, nyi)),
                Instr64(
                    Instr(Mnemonic.flushw, nyi),
                    invalid),

                Instr64(
                    Instr(Mnemonic.movcc, nyi),
                    invalid),
                Instr64(
                    Instr(Mnemonic.sdivx, r14,R0,r25),
                    invalid),
                Instr64(
                    Instr(Mnemonic.popc, R0,r25),
                    invalid),
                Instr64(
                    SparcDisassembler.Mask(10, 3, "  movr",
                        invalid,
                        Instr(Mnemonic.movrz, r14, Rs10, r25),
                        Instr(Mnemonic.movrlez, r14, Rs10, r25),
                        Instr(Mnemonic.movrlz, r14, Rs10, r25),

                        invalid,
                        Instr(Mnemonic.movrnz, r14, Rs10, r25),
                        Instr(Mnemonic.movrgz, r14, Rs10, r25),
                        Instr(Mnemonic.movrgez, r14, Rs10, r25)),
                    invalid),

                // 30
                Instr(Mnemonic.wrasr, nyi), // ****
                Instr64(
                    SparcDisassembler.Sparse(25, 5, "  31", invalid,
                        (0, Instr(Mnemonic.saved, nyi)),
                        (1, Instr(Mnemonic.restored, nyi))),
                    Instr(Mnemonic.wrpsr, nyi)),
                Instr64(
                    Instr(Mnemonic.wrpr, InstrClass.Linear | InstrClass.Privileged, nyi),
                    Instr(Mnemonic.wrwim, nyi)),
                Instr64(
                    invalid,
                    Instr(Mnemonic.wrtbr, InstrClass.Linear | InstrClass.Privileged, r14, R0)),

                SparcDisassembler.Sparse(5, 9, "  FOp1", invalid, fpDecoders),
                SparcDisassembler.Sparse(5, 9, "  FOp2", invalid, fpDecoders),
                SparcDisassembler.Sparse(4, 9, "  CPop1", invalid, fpDecoders),
                SparcDisassembler.Sparse(4, 9, "  CPop2", invalid, fpDecoders),

                Instr(Mnemonic.jmpl, r14,Rs,r25),
                Instr64(
                    Instr(Mnemonic.@return, Transfer|InstrClass.Return|InstrClass.Delay, r14,Rs),
                    Instr(Mnemonic.rett, Transfer|InstrClass.Return|InstrClass.Delay, r14,Rs)),
                new BranchDecoder(branchOps, 0x30),
                Instr(Mnemonic.flush),
                Instr(Mnemonic.save, r14,R0,r25),
                Instr(Mnemonic.restore, r14,R0,r25),
                Instr64(
                    SparcDisassembler.Sparse(25, 5, "  31", invalid,
                        (0, Instr(Mnemonic.done, nyi)),
                        (1, Instr(Mnemonic.retry, nyi))),
                    invalid),
                invalid,
            };

            var decoders_3 = new Decoder[]
            {
                // 00
                Instr64(
                    Instr(Mnemonic.lduw, Mw,r25),
                    Instr(Mnemonic.ld, Mw,r25)),
                Instr(Mnemonic.ldub, Mb,r25),
                Instr(Mnemonic.lduh, Mh,r25),
                Instr(Mnemonic.ldd, Md,rp25),
                Instr64(
                    Instr(Mnemonic.stw, r25,Mw),
                    Instr(Mnemonic.st, r25,Mw)),
                Instr(Mnemonic.stb, r25,Mb),
                Instr(Mnemonic.sth, r25,Mh),
                Instr(Mnemonic.std, rp25,Md),

                Instr64(
                    Instr(Mnemonic.ldsw, Msw,r25),
                    invalid),
                Instr(Mnemonic.ldsb, Msb,r25),
                Instr(Mnemonic.ldsh, Msh,r25),
                Instr64(
                    Instr(Mnemonic.ldx, Md,r25),
                    invalid),
                invalid,
                Instr(Mnemonic.ldstub, nyi),
                Instr64(
                    Instr(Mnemonic.stx, r25,Md),
                    invalid),
                Instr(Mnemonic.swap, Mw,r25),

                // 10
                Instr64(
                    Instr(Mnemonic.ldua, InstrClass.Linear|InstrClass.Privileged, Aw,r25),
                    Instr(Mnemonic.lda, InstrClass.Linear|InstrClass.Privileged, Aw,r25)),
                Instr(Mnemonic.lduba, InstrClass.Linear|InstrClass.Privileged, Ab,r25),
                Instr(Mnemonic.lduha, InstrClass.Linear|InstrClass.Privileged, Ah,r25),
                Instr(Mnemonic.ldda, InstrClass.Linear | InstrClass.Privileged, Ad,r25),
                Instr64(
                    Instr(Mnemonic.stwa, InstrClass.Linear|InstrClass.Privileged, r25,Aw),
                    Instr(Mnemonic.sta, InstrClass.Linear|InstrClass.Privileged, r25,Aw)),
                Instr(Mnemonic.stba, InstrClass.Linear|InstrClass.Privileged, r25,Ab),
                Instr(Mnemonic.stha, InstrClass.Linear|InstrClass.Privileged, r25,Ah),
                Instr(Mnemonic.stda, InstrClass.Linear|InstrClass.Privileged, r25,Ad),

                Instr64(
                    Instr(Mnemonic.ldswa, InstrClass.Linear|InstrClass.Privileged, r25,Asw),
                    invalid),
                Instr(Mnemonic.ldsba, InstrClass.Linear|InstrClass.Privileged, r25,Asb),
                Instr(Mnemonic.ldsha, InstrClass.Linear|InstrClass.Privileged, r25,Ash),
                Instr64(
                    Instr(Mnemonic.ldxa, InstrClass.Linear|InstrClass.Privileged, r25,Ad),
                    invalid),
                
                invalid,
                Instr(Mnemonic.ldstuba, InstrClass.Linear|InstrClass.Privileged, Ab,r25),
                Instr64(
                    Instr(Mnemonic.stxa, InstrClass.Linear|InstrClass.Privileged, r25,Ad),
                    invalid),
                Instr(Mnemonic.swapa, InstrClass.Linear|InstrClass.Privileged,  Aw,r25),

                // 20
                Instr(Mnemonic.ldf, Mw,f24),
                Instr64(
                    Instr(Mnemonic.ldxfsr, Md,rfsr),
                    Instr(Mnemonic.ldfsr, Mw,rfsr)),
                Instr64(
                    Instr(Mnemonic.ldqf, Mq,f24),
                    invalid),
                Instr(Mnemonic.lddf, Md,f24),
                Instr(Mnemonic.stf, f24,Mw),
                Instr64(
                    Instr(Mnemonic.stxfsr, rfsr,Md),
                    Instr(Mnemonic.stfsr, rfsr,Mw)),
                Instr64(
                    Instr(Mnemonic.stqf, f24,Mq),
                    Instr(Mnemonic.stdfq, nyi)),
                Instr(Mnemonic.stdf, f24,Md),

                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                Instr64(
                    Instr(Mnemonic.prefetch, nyi),
                    invalid),
                invalid,
                invalid,

                // 30
                Instr64(
                    Instr(Mnemonic.ldfa, nyi),
                    Instr(Mnemonic.ldc)),
                Instr64(
                    invalid,
                    Instr(Mnemonic.ldcsr)),
                Instr64(
                    Instr(Mnemonic.ldqfa, nyi),
                    invalid),
                Instr64(
                    Instr(Mnemonic.lddfa, nyi),
                    Instr(Mnemonic.lddc, nyi)),
                Instr64(
                    Instr(Mnemonic.stfa, nyi),
                    Instr(Mnemonic.stc, nyi)),
                Instr64(
                    invalid,
                    Instr(Mnemonic.stcsr, nyi)),
                Instr64(
                    Instr(Mnemonic.stqfa, nyi),
                    Instr(Mnemonic.stdcq, nyi)),
                Instr64(
                    Instr(Mnemonic.stdfa, nyi),
                    Instr(Mnemonic.stdc, nyi)),

                invalid,
                invalid,
                invalid,
                invalid,
                Instr64(
                    Instr(Mnemonic.casa, nyi),
                    invalid),
                Instr64(
                    Instr(Mnemonic.prefetcha, nyi),
                    invalid),
                Instr64(
                    Instr(Mnemonic.casxa, nyi),
                    invalid),
                invalid,
            };


            // Format 1 (op == 1)
            // +----+-------------------------------------------------------------+
            // | op | disp30                                                      |
            // +----+-------------------------------------------------------------+
            //
            // Format 2 (op == 0). SETHI and branches Bicc, FBcc, CBcc
            // +----+---+------+-----+--------------------------------------------+
            // | op | rd       | op2 | imm22                                      |
            // +----+---+------+-----+--------------------------------------------+
            // | op | a | cond | op2 | disp22                                     |
            // +----+---+------+-----+--------------------------------------------+
            // 31   29  28     24    21
            //
            // Format 3 (op = 2, 3)
            // +----+----------+--------+------+-----+--------------------+-------+
            // | op |    rd    |   op3  |  rs1 | i=0 |        asi         |  rs2  |
            // +----+----------+--------+------+-----+--------------------+-------+
            // | op |    rd    |   op3  |  rs1 | i=1 |        simm13              |
            // +----+----------+--------+------+-----+--------------------+-------+
            // | op |    rd    |   op3  |  rs1 |           opf            |  rs2  |
            // +----+----------+--------+------+--------------------------+-------+
            // 31   29         24       18     13    12                   4

            return SparcDisassembler.Mask(30, 2, "SPARC",
                SparcDisassembler.Mask(22, 3, "  Format 0", decoders_0),
                Instr(Mnemonic.call, LinkTransfer, JJ),
                SparcDisassembler.Mask(19, 6, "  Format 2", decoders_2),
                SparcDisassembler.Mask(19, 6, "  Format 3", decoders_3));
        }

        private class BranchDecoder : Decoder
        {
            private readonly Decoder[] branchOps;
            private readonly uint offset;

            public BranchDecoder(Decoder[] branchOps, uint offset)
            {
                this.branchOps = branchOps;
                this.offset = offset;
            }

            public override SparcInstruction Decode(uint wInstr, SparcDisassembler dasm)
            {
                uint i = ((wInstr >> 25) & 0xF) + offset;
                SparcInstruction instr = branchOps[i].Decode(wInstr, dasm);
                instr.InstructionClass |= ((wInstr & (1u << 29)) != 0) ? InstrClass.Annul : 0;
                return instr;
            }
        }
    }
}
