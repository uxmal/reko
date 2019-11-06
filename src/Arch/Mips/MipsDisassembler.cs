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
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Reko.Arch.Mips
{
    public partial class MipsDisassembler : DisassemblerBase<MipsInstruction>
    {
        private const InstrClass TD = InstrClass.Transfer | InstrClass.Delay;
        private const InstrClass CTD = InstrClass.Call | InstrClass.Transfer | InstrClass.Delay;
        private const InstrClass DCT = InstrClass.ConditionalTransfer | InstrClass.Delay;

        private static readonly MaskDecoder<MipsDisassembler, Mnemonic, MipsInstruction> rootDecoder;

        private readonly MipsProcessorArchitecture arch;
        private readonly bool isVersion6OrLater;
        private readonly EndianImageReader rdr;
        private readonly PrimitiveType signedWord;
        private readonly List<MachineOperand> ops;
        private MipsInstruction instrCur;
        private Address addr;

        public MipsDisassembler(MipsProcessorArchitecture arch, EndianImageReader imageReader, bool isVersion6OrLater)
        {
            this.arch = arch;
            this.rdr = imageReader;
            this.isVersion6OrLater = isVersion6OrLater;
            this.signedWord = PrimitiveType.Create(Domain.SignedInt, arch.WordWidth.BitSize);
            this.ops = new List<MachineOperand>();
        }

        public override MipsInstruction DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadUInt32(out uint wInstr))
            {
                return null;
            }
            this.ops.Clear();
            instrCur = rootDecoder.Decode(wInstr, this);
            instrCur.Address = this.addr;
            instrCur.Length = 4;
            return instrCur;
        }

        protected override MipsInstruction CreateInvalidInstruction()
        {
            return new MipsInstruction {
                Mnemonic = Mnemonic.illegal,
                InstructionClass = InstrClass.Invalid,
                Operands = new MachineOperand[0]
            };
        }

        public override MipsInstruction NotYetImplemented(uint wInstr, string message)
        {
            var instr = CreateInvalidInstruction();
            EmitUnitTest(wInstr, message);
            return instr;
        }

        [Conditional("DEBUG")]
        public void EmitUnitTest(uint wInstr, string message)
        {
            var op = (wInstr >> 26);
            if (op == 0 || op == 1)
                return;
            var instrHex = $"{wInstr:X8}";
            base.EmitUnitTest("MIPS", instrHex, message, "MipsDis", this.addr, w =>
            {
                w.WriteLine("    AssertCode(\"@@@\", \"0x{0:X8}\");", wInstr);
            });
        }


        private static NyiDecoder<MipsDisassembler, Mnemonic, MipsInstruction> Nyi(string message)
        {
            return new NyiDecoder<MipsDisassembler, Mnemonic, MipsInstruction>(message);
        }

        private static InstrDecoder Instr(Mnemonic mnemonic, params Mutator<MipsDisassembler> [] mutators)
        {
            return new InstrDecoder(InstrClass.Linear, mnemonic, mutators);
        }

        private static InstrDecoder Instr(InstrClass iclass, Mnemonic mnemonic, params Mutator<MipsDisassembler>[] mutators)
        {
            return new InstrDecoder(iclass, mnemonic, mutators);
        }

        static MipsDisassembler()
        {
            var invalid = new InstrDecoder(Mnemonic.illegal);

            var cop1_s = Mask(0, 6, "FPU (single)",
                new InstrDecoder(Mnemonic.add_s, F4,F3,F2),
                new InstrDecoder(Mnemonic.sub_s, F4,F3,F2),
                new InstrDecoder(Mnemonic.mul_s, F4,F3,F2),
                new InstrDecoder(Mnemonic.div_s, F4,F3,F2),
                invalid,
                invalid,
                new InstrDecoder(Mnemonic.mov_s, F4,F3),
                new InstrDecoder(Mnemonic.neg_s, F4,F3),

                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                new InstrDecoder(Mnemonic.c_eq_s, c8,F3,F2),
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,
                new InstrDecoder(Mnemonic.c_lt_s, c8,F3,F2),
                invalid,
                new InstrDecoder(Mnemonic.c_le_s, c8,F3,F2),
                invalid);

            var cop1_d = Mask(0, 6, "FPU (double)",
                // fn 00
                new InstrDecoder(Mnemonic.add_d, F4,F3,F2),
                new InstrDecoder(Mnemonic.sub_d, F4,F3,F2),
                new InstrDecoder(Mnemonic.mul_d, F4,F3,F2),
                new InstrDecoder(Mnemonic.div_d, F4,F3,F2),
                invalid,
                invalid,
                new InstrDecoder(Mnemonic.mov_d, F4,F3),
                new InstrDecoder(Mnemonic.neg_d, F4,F3),

                invalid,
                new InstrDecoder(Mnemonic.trunc_l_d, F4,F3),
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,

                // fn 10
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,

                // fn 20
                new InstrDecoder(Mnemonic.cvt_s_d, F4,F3),
                invalid,
                invalid,
                invalid,
                new InstrDecoder(Mnemonic.cvt_w_d, F4,F3),
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,

                // fn 30
                invalid,
                invalid,
                new InstrDecoder(Mnemonic.c_eq_d, c8,F3,F2),
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,
                new InstrDecoder(Mnemonic.c_lt_d, c8,F3,F2),
                invalid,
                new InstrDecoder(Mnemonic.c_le_d, c8,F3,F2),
                invalid);

            var cop1_w = Mask(0, 6, "FPU (word)",
                // fn 00
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,

                // fn 10
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,

                // fn 20
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,

                // fn 30
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,
                new InstrDecoder(Mnemonic.c_lt_d, c8,F3,F2),
                invalid,
                invalid,
                invalid);

            var cop1_l = Mask(0, 6, "FPU (dword)",
                // fn 00
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,

                // fn 10
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,

                // fn 20
                new InstrDecoder(Mnemonic.cvt_s_l, F4,F3),
                new InstrDecoder(Mnemonic.cvt_d_l, F4,F3),
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,

                // fn 30
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid);

            var cop0_C0_decoder = Sparse(0, 6, "COP0 C0", 
                invalid,
                ( 0x01, new InstrDecoder(Mnemonic.tlbr ) ),
                ( 0x02, new InstrDecoder(Mnemonic.tlbwi) ),
                ( 0x06, new InstrDecoder(Mnemonic.tlbwr) ),
                ( 0x08, new InstrDecoder(Mnemonic.tlbp ) ),
                ( 0x18, new InstrDecoder(Mnemonic.eret ) ),
                ( 0x20, new InstrDecoder(Mnemonic.wait ) ));
            var cop1 = Mask(21, 5, "COP1",
                new InstrDecoder(Mnemonic.mfc1, R2,F3),
                new A64Decoder(Mnemonic.dmfc1, R2,F3),
                new InstrDecoder(Mnemonic.cfc1, R2,f3),
                invalid,
                new InstrDecoder(Mnemonic.mtc1, R2,F3),
                new A64Decoder(Mnemonic.dmtc1, R2,F3),
                new InstrDecoder(Mnemonic.ctc1, R2,f3),
                invalid,

                Mask(16, 1,
                    new InstrDecoder(InstrClass.ConditionalTransfer | InstrClass.Delay, Mnemonic.bc1f, c18,j),
                    new InstrDecoder(InstrClass.ConditionalTransfer | InstrClass.Delay, Mnemonic.bc1t, c18,j)),
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,

                cop1_s,
                cop1_d,
                invalid,
                invalid,
                cop1_w,
                cop1_l,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid);

            var cop2 = Mask(21, 5, "COP2",   // 12: COP2 
                 Nyi("mfc2"),
                 invalid,
                 Nyi("cfc2"),
                 Nyi("mfhc2"),
                 Nyi("mtc2"),
                 invalid,
                 Nyi("ctc2"),
                 Nyi("mthc2"),

                 Nyi("bc2"),
                 Nyi("bc2eqz"),
                 new InstrDecoder(Mnemonic.lwc2,R2,E11w),
                 Nyi("swc2"),
                 invalid,
                 Nyi("bc2nez"),
                 Nyi("ldc2"),
                 Nyi("sdc2"),

                 invalid,
                 invalid,
                 invalid,
                 invalid,
                 invalid,
                 invalid,
                 invalid,
                 invalid,

                 invalid,
                 invalid,
                 invalid,
                 invalid,
                 invalid,
                 invalid,
                 invalid,
                 invalid);

            var special2 = Sparse(0, 6, "Special2",
                invalid,
                (0x0, new InstrDecoder(Mnemonic.madd, R1, R2)),
                (0x1, new InstrDecoder(Mnemonic.maddu, R1, R2)),
                (0x2, new InstrDecoder(Mnemonic.mul, R3, R1, R2)),
                (0x4, new InstrDecoder(Mnemonic.msub, R1, R2)),
                (0x5, new InstrDecoder(Mnemonic.msubu, R1, R2)),

                (0x20, new InstrDecoder(Mnemonic.clz, R3, R1)),
                (0x21, new InstrDecoder(Mnemonic.clo, R3, R1)),
                (0x3F, new InstrDecoder(Mnemonic.sdbbp, Imm(PrimitiveType.UInt32, 6, 20))));

            var condDecoders = Mask(16, 5, "CondDecoders",
                new InstrDecoder(DCT, Mnemonic.bltz, R1, j),
                new InstrDecoder(DCT, Mnemonic.bgez, R1, j),
                new InstrDecoder(DCT, Mnemonic.bltzl, R1, j),
                new InstrDecoder(DCT, Mnemonic.bgezl, R1, j),

                invalid,
                invalid,
                invalid,
                invalid,

                new InstrDecoder(CTD, Mnemonic.tgei, R1, I),
                new InstrDecoder(CTD, Mnemonic.tgeiu, R1, I),
                new InstrDecoder(CTD, Mnemonic.tlti, R1, I),
                new InstrDecoder(CTD, Mnemonic.tltiu, R1, I),

                new InstrDecoder(CTD, Mnemonic.teqi, R1, I),
                invalid,
                new InstrDecoder(CTD, Mnemonic.tnei, R1, I),
                invalid,

                new InstrDecoder(CTD, Mnemonic.bltzal, R1, j),
                new InstrDecoder(CTD, Mnemonic.bgezal, R1, j),
                new InstrDecoder(CTD, Mnemonic.bltzall, R1, j),
                new InstrDecoder(CTD, Mnemonic.bgezall, R1, j),

                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid);


           var bshfl = Mask(6, 5,
                invalid,
                invalid,
                new InstrDecoder(Mnemonic.wsbh, x("")),
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                // 10
                new InstrDecoder(Mnemonic.seb, R3, R2),
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                new InstrDecoder(Mnemonic.seh, R3, R2),
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid);

        var special3 = Mask(0, 6, "Special3",
            // 00
            invalid,
            invalid,
            invalid,
            invalid,
            invalid,
            invalid,
            invalid,
            invalid,

            invalid,
            invalid,
            invalid,
            invalid,
            invalid,
            invalid,
            invalid,
            invalid,

            // 10
            invalid,
            invalid,
            invalid,
            invalid,
            invalid,
            invalid,
            invalid,
            invalid,

            invalid,
            invalid,
            invalid,
            invalid,
            invalid,
            invalid,
            invalid,
            invalid,

            // 20
            bshfl,
            invalid,
            invalid,
            invalid,
            invalid,
            invalid,
            invalid,
            invalid,

            invalid,
            invalid,
            invalid,
            invalid,
            invalid,
            invalid,
            invalid,
            invalid,

            // 30
            invalid,
            invalid,
            invalid,
            invalid,
            invalid,
            invalid,
            new Version6Decoder(
                invalid,
                new InstrDecoder(Mnemonic.ll, R2,ew)),
            new Version6Decoder(
                invalid,
                new A64Decoder(Mnemonic.lld, R2,el)),

            invalid,
            invalid,
            invalid,
            new InstrDecoder(Mnemonic.rdhwr, R2,H),
            invalid,
            invalid,
            invalid,
            invalid);

            var special = Mask(0, 6, "Special",
                Select((6, 5), n => n == 0,
                    new InstrDecoder(InstrClass.Linear|InstrClass.Padding, Mnemonic.nop),
                    new InstrDecoder(Mnemonic.sll, R3, R2, s)),
                Mask(16, 1,
                    new InstrDecoder(Mnemonic.movf, R2, R1, C18),
                    new InstrDecoder(Mnemonic.movt, R2, R1, C18)),
                new InstrDecoder(Mnemonic.srl, R3, R2, s),
                new InstrDecoder(Mnemonic.sra, R3, R2, s),

                new InstrDecoder(Mnemonic.sllv, R3, R2, R1),
                new InstrDecoder(Mnemonic.illegal),
                new InstrDecoder(Mnemonic.srlv, R3, R2, R1),
                new InstrDecoder(Mnemonic.srav, R3, R2, R1),

                new InstrDecoder(TD, Mnemonic.jr, R1),
                new InstrDecoder(CTD, Mnemonic.jalr, R3, R1),
                new InstrDecoder(Mnemonic.movz, R3, R1, R2),
                new InstrDecoder(Mnemonic.movn, R3, R1, R2),
                new InstrDecoder(Mnemonic.syscall, B),
                new InstrDecoder(Mnemonic.@break, B),
                new InstrDecoder(Mnemonic.illegal),
                new InstrDecoder(Mnemonic.sync, s),
                // 10
                new InstrDecoder(Mnemonic.mfhi, R3),
                new InstrDecoder(Mnemonic.mthi, R1),
                new InstrDecoder(Mnemonic.mflo, R3),
                new InstrDecoder(Mnemonic.mtlo, R1),
                new A64Decoder(Mnemonic.dsllv, R3, R2, R1),
                new InstrDecoder(Mnemonic.illegal),
                new A64Decoder(Mnemonic.dsrlv, R3, R2, R1),
                new A64Decoder(Mnemonic.dsrav, R3, R2, R1),

                new InstrDecoder(Mnemonic.mult, R1, R2),
                new InstrDecoder(Mnemonic.multu, R1, R2),
                new InstrDecoder(Mnemonic.div, R1, R2),
                new InstrDecoder(Mnemonic.divu, R1, R2),
                new A64Decoder(Mnemonic.dmult, R1, R2),
                new A64Decoder(Mnemonic.dmultu, R1, R2),
                new A64Decoder(Mnemonic.ddiv, R1, R2),
                new A64Decoder(Mnemonic.ddivu, R1, R2),
                // 20
                new InstrDecoder(Mnemonic.add, R3, R1, R2),
                new InstrDecoder(Mnemonic.addu, R3, R1, R2),
                new InstrDecoder(Mnemonic.sub, R3, R1, R2),
                new InstrDecoder(Mnemonic.subu, R3, R1, R2),
                new InstrDecoder(Mnemonic.and, R3, R1, R2),
                new InstrDecoder(Mnemonic.or, R3, R1, R2),
                new InstrDecoder(Mnemonic.xor, R3, R1, R2),
                new InstrDecoder(Mnemonic.nor, R3, R1, R2),

                new InstrDecoder(Mnemonic.illegal),
                new InstrDecoder(Mnemonic.illegal),
                new InstrDecoder(Mnemonic.slt, R3, R1, R2),
                new InstrDecoder(Mnemonic.sltu, R3, R1, R2),
                new A64Decoder(Mnemonic.dadd, R3, R1, R2),
                new A64Decoder(Mnemonic.daddu, R3, R1, R2),
                new A64Decoder(Mnemonic.dsub, R3, R1, R2),
                new A64Decoder(Mnemonic.dsubu, R3, R1, R2),
                // 30
                new InstrDecoder(CTD, Mnemonic.tge, R1, R2, T),
                new InstrDecoder(CTD, Mnemonic.tgeu, R1, R2, T),
                new InstrDecoder(CTD, Mnemonic.tlt, R1, R2, T),
                new InstrDecoder(CTD, Mnemonic.tltu, R1, R2, T),
                new InstrDecoder(CTD, Mnemonic.teq, R1, R2, T),
                new InstrDecoder(Mnemonic.illegal),
                new InstrDecoder(CTD, Mnemonic.tne, R1, R2, T),
                new InstrDecoder(Mnemonic.illegal),

                new A64Decoder(Mnemonic.dsll, R3, R2, s),
                new InstrDecoder(Mnemonic.illegal),
                new A64Decoder(Mnemonic.dsrl, R3, R2, s),
                new A64Decoder(Mnemonic.dsra, R3, R2, s),
                new A64Decoder(Mnemonic.dsll32, R3, R2, s),
                new InstrDecoder(Mnemonic.illegal),
                new A64Decoder(Mnemonic.dsrl32, R3, R2, s),
                new A64Decoder(Mnemonic.dsra32, R3, R2, s));

            var cop1x = Mask(0, 6,
                Instr(Mnemonic.lwxc1, F3,Mxw),
                Instr(Mnemonic.ldxc1, F3,Mxd),
                invalid,
                invalid,

                invalid,
                Instr(Mnemonic.luxc1, F4,Mxw),
                invalid,
                invalid,

                Instr(Mnemonic.swxc1, F3,Mxw),
                Instr(Mnemonic.sdxc1, F3,Mxd),
                invalid,
                invalid,

                invalid,
                Instr(Mnemonic.suxc1, F4,Mxw),
                invalid,
                new InstrDecoder(Mnemonic.prefx, Imm(PrimitiveType.Byte, 11,5), Mxw),
                // 10
                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                new InstrDecoder(Mnemonic.alnv_ps, F4,F3,F2,R1),
                invalid,
                // 20
                new InstrDecoder(Mnemonic.madd_s, F4,F1,F3,F2),
                new InstrDecoder(Mnemonic.madd_d, F4,F1,F3,F2),
                invalid,
                invalid,

                invalid,
                invalid,
                new InstrDecoder(Mnemonic.madd_ps, F4, F1, F3, F2),
                invalid,

                new InstrDecoder(Mnemonic.msub_s, F4, F1, F3, F2),
                new InstrDecoder(Mnemonic.msub_d, F4, F1, F3, F2),
                invalid,
                invalid,

                invalid,
                invalid,
                new InstrDecoder(Mnemonic.msub_ps, F4, F1, F3, F2),
                invalid,
                // 30
                new InstrDecoder(Mnemonic.nmadd_s, F4, F1, F3, F2),
                new InstrDecoder(Mnemonic.nmadd_d, F4, F1, F3, F2),
                invalid,
                invalid,

                invalid,
                invalid,
                new InstrDecoder(Mnemonic.nmadd_ps, F4, F1, F3, F2),
                invalid,

                new InstrDecoder(Mnemonic.nmsub_s, F4, F1, F3, F2),
                new InstrDecoder(Mnemonic.nmsub_d, F4, F1, F3, F2),
                invalid,
                invalid,

                invalid,
                invalid,
                new InstrDecoder(Mnemonic.nmsub_ps, F4, F1, F3, F2),
                invalid);


        rootDecoder = Mask(26, 6,
                special,
                condDecoders,
                new InstrDecoder(TD, Mnemonic.j, J),
                new InstrDecoder(CTD, Mnemonic.jal, J),
                new InstrDecoder(DCT, Mnemonic.beq, R1,R2,j),
                new InstrDecoder(DCT, Mnemonic.bne, R1,R2,j),
                new InstrDecoder(DCT, Mnemonic.blez, R1,j),
                new InstrDecoder(DCT, Mnemonic.bgtz, R1,j),

                new InstrDecoder(Mnemonic.addi, R2,R1,I),
                new InstrDecoder(Mnemonic.addiu, R2,R1,I),
                new InstrDecoder(Mnemonic.slti, R2,R1,I),
                new InstrDecoder(Mnemonic.sltiu, R2,R1,I),

                new InstrDecoder(Mnemonic.andi, R2,R1,U),
                new InstrDecoder(Mnemonic.ori, R2,R1,U),
                new InstrDecoder(Mnemonic.xori, R2,R1,U),
                new InstrDecoder(Mnemonic.lui, R2,i),
                // 10
                Mask(21, 5, "Coprocessor",
                    new InstrDecoder(Mnemonic.mfc0, R2,R3),
                    new InstrDecoder(Mnemonic.dmfc0, R2,R3),
                    invalid,
                    invalid,
                    new InstrDecoder(Mnemonic.mtc0, R2,R3),
                    new InstrDecoder(Mnemonic.dmtc0, R2,R3),
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    cop0_C0_decoder,
                    cop0_C0_decoder,
                    cop0_C0_decoder,
                    cop0_C0_decoder,
                    cop0_C0_decoder,
                    cop0_C0_decoder,
                    cop0_C0_decoder,
                    cop0_C0_decoder,

                    cop0_C0_decoder,
                    cop0_C0_decoder,
                    cop0_C0_decoder,
                    cop0_C0_decoder,
                    cop0_C0_decoder,
                    cop0_C0_decoder,
                    cop0_C0_decoder,
                    cop0_C0_decoder),
               cop1,
               new Version6Decoder(
                   invalid,
                   cop2),
               new Version6Decoder(
                    cop1x,
                    invalid),       // removed in MIPS v6
                
                new InstrDecoder(DCT, Mnemonic.beql, R1,R2,j),
                new InstrDecoder(DCT, Mnemonic.bnel, R1,R2,j),
                new InstrDecoder(DCT, Mnemonic.blezl, R1,j),
                new InstrDecoder(DCT, Mnemonic.bgtzl, R1,j),

                new A64Decoder(Mnemonic.daddi, R2,R1,I),
                new A64Decoder(Mnemonic.daddiu, R2,R1,I),
                new A64Decoder(Mnemonic.ldl, R2,El),
                new A64Decoder(Mnemonic.ldr, R2,El),

                new Version6Decoder(
                    special2,
                    invalid),
                new Version6Decoder(
                    invalid,
                    Nyi("POP6")),
                new Version6Decoder(
                    invalid, 
                    Nyi("POP7")),
                special3,

                // 20
                new InstrDecoder(Mnemonic.lb, R2,EB),
                new InstrDecoder(Mnemonic.lh, R2,EH),
                new InstrDecoder(Mnemonic.lwl, R2,Ew),
                new InstrDecoder(Mnemonic.lw, R2,Ew),

                new InstrDecoder(Mnemonic.lbu, R2,Eb),
                new InstrDecoder(Mnemonic.lhu, R2,Eh),
                new InstrDecoder(Mnemonic.lwr, R2,Ew),
                new A64Decoder(Mnemonic.lwu, R2,Ew),

                new InstrDecoder(Mnemonic.sb, R2,Eb),
                new InstrDecoder(Mnemonic.sh, R2,Eh),
                new InstrDecoder(Mnemonic.swl, R2,Ew),
                new InstrDecoder(Mnemonic.sw, R2,Ew),

                new InstrDecoder(Mnemonic.sdl, R2,Ew),
                new InstrDecoder(Mnemonic.sdr, R2,Ew),
                new InstrDecoder(Mnemonic.swr, R2,Ew),
                new Version6Decoder(
                    new InstrDecoder(Mnemonic.cache, Imm(PrimitiveType.Byte, 16, 5), Ew),
                    invalid),

                // 30
                new Version6Decoder(
                    new InstrDecoder(Mnemonic.ll, R2,Ew),
                    invalid),
                new InstrDecoder(Mnemonic.lwc1, F2,Ew),
                new Version6Decoder(
                    new InstrDecoder(Mnemonic.lwc2, R2, El),
                    Nyi("BC-v6")),
                new InstrDecoder(Mnemonic.pref, R2,Ew),

                new Version6Decoder(
                    new A64Decoder(Mnemonic.lld, R2,El),
                    invalid),
                new InstrDecoder(Mnemonic.ldc1, F2,El),
                new Version6Decoder(
                    new InstrDecoder(Mnemonic.ldc2, R2,El),
                    Nyi("POP76")),
                new A64Decoder(Mnemonic.ld, R2,El),

                new Version6Decoder(
                    new InstrDecoder(Mnemonic.sc, R2,Ew),
                    invalid),
                new InstrDecoder(Mnemonic.swc1, F2,Ew),
                new Version6Decoder(
                    new InstrDecoder(Mnemonic.swc2, R2, Ew),
                    Nyi("BALC-v6")),
                new Version6Decoder(
                    invalid,
                    Nyi("PCREL-v6")),

                new Version6Decoder(
                    new A64Decoder(Mnemonic.scd, R2,El),
                    invalid),
                new A64Decoder(Mnemonic.sdc1, F2,El),
                new Version6Decoder(
                    new InstrDecoder(Mnemonic.sdc2, R2,El),
                    Nyi("POP76")),
                new A64Decoder(Mnemonic.sd, R2,El));
        }

        internal static Mutator<MipsDisassembler> R(int offset)
        {
            return (u, d) =>
            {
                var op = d.Reg(u >> offset);
                d.ops.Add(op);
                return true;
            };
        }
        internal static readonly Mutator<MipsDisassembler> R1 = R(21);
        internal static readonly Mutator<MipsDisassembler> R2 = R(16);
        internal static readonly Mutator<MipsDisassembler> R3 = R(11);
        internal static readonly Mutator<MipsDisassembler> R4 = R(6);

        internal static Mutator<MipsDisassembler> F(int offset)
        {
            return (u, d) =>
            {
                var op = d.FReg(u >> offset);
                d.ops.Add(op);
                return true;
            };
        }
        internal static readonly Mutator<MipsDisassembler> F1 = F(21);
        internal static readonly Mutator<MipsDisassembler> F2 = F(16);
        internal static readonly Mutator<MipsDisassembler> F3 = F(11);
        internal static readonly Mutator<MipsDisassembler> F4 = F(6);
        
        // FPU control register
        internal static Mutator<MipsDisassembler> Fcreg(int offset)
        {
            return (u, d) =>
            {
                if (!d.TryGetFCReg(u >> offset, out RegisterOperand fcreg))
                    return false;
                d.ops.Add(fcreg);
                return true;
            };
        }
        internal static readonly Mutator<MipsDisassembler> f1 = Fcreg(21);
        internal static readonly Mutator<MipsDisassembler> f2 = Fcreg(16);
        internal static readonly Mutator<MipsDisassembler> f3 = Fcreg(11);

        internal static bool I(uint wInstr, MipsDisassembler dasm)
        {
            var op = new ImmediateOperand(Constant.Create(dasm.signedWord, (short) wInstr));
            dasm.ops.Add(op);
            return true;
        }

        internal static bool U(uint wInstr, MipsDisassembler dasm)
        {
            var op = new ImmediateOperand(Constant.Create(dasm.arch.WordWidth, (ushort) wInstr));
            dasm.ops.Add(op);
            return true;
        }

        internal static bool i(uint wInstr, MipsDisassembler dasm)
        {
            var op = ImmediateOperand.Int16((short) wInstr);
            dasm.ops.Add(op);
            return true;
        }

        private static Mutator<MipsDisassembler> Imm(PrimitiveType dt, int bitPos, int bitlen)
        {
            var field = new Bitfield(bitPos, bitlen);
            return (u, d) =>
            {
                var n = field.Read(u);
                d.ops.Add(new ImmediateOperand(Constant.Create(dt, n)));
                return true;
            };
        }

        internal static bool j(uint wInstr, MipsDisassembler dasm)
        {
            var op = dasm.RelativeBranch(wInstr);
            dasm.ops.Add(op);
            return true;
        }

        internal static bool J(uint wInstr, MipsDisassembler dasm)
        {
            var op = dasm.LargeBranch(wInstr);
            dasm.ops.Add(op);
            return true;
        }

        internal static bool B(uint wInstr, MipsDisassembler dasm)
        {
            var op = ImmediateOperand.Word32((wInstr >> 6) & 0xFFFFF);
            dasm.ops.Add(op);
            return true;
        }

        // Shift amount or sync type
        internal static bool s(uint wInstr, MipsDisassembler dasm)
        {
            var op = ImmediateOperand.Byte((byte) ((wInstr >> 6) & 0x1F));
            dasm.ops.Add(op);
            return true;
        }

        // effective address w 16-bit offset
        internal static Mutator<MipsDisassembler> E(PrimitiveType size)
        {
            return (u, d) =>
            {
                var op = d.Ea(u, size, 21, (short) u);
                d.ops.Add(op);
                return true;
            };
        }
        internal static readonly Mutator<MipsDisassembler> Eb = E(PrimitiveType.Byte);
        internal static readonly Mutator<MipsDisassembler> EB = E(PrimitiveType.SByte);
        internal static readonly Mutator<MipsDisassembler> Eh = E(PrimitiveType.Word16);
        internal static readonly Mutator<MipsDisassembler> EH = E(PrimitiveType.Int16); 
        internal static readonly Mutator<MipsDisassembler> Ew = E(PrimitiveType.Word32);
        internal static readonly Mutator<MipsDisassembler> EW = E(PrimitiveType.Int32); 
        internal static readonly Mutator<MipsDisassembler> El = E(PrimitiveType.Word64);
        internal static readonly Mutator<MipsDisassembler> EL = E(PrimitiveType.Int64);

        // effective address w 9-bit offset
        internal static Mutator<MipsDisassembler> e(PrimitiveType size)
        {
            return (u, d) =>
            {
                var op = d.Ea(u, size, 21, (short) (((short) u) >> 7));
                d.ops.Add(op);
                return true;
            };
        }
        internal static readonly Mutator<MipsDisassembler> ew = e(PrimitiveType.Word32);
        internal static readonly Mutator<MipsDisassembler> el = e(PrimitiveType.Word64);

        // effective address w 11-bit offset
        internal static Mutator<MipsDisassembler> E11(PrimitiveType size)
        {
            var offsetField = new Bitfield(0, 11);
            return (u, d) =>
            {
                var offset = (short)offsetField.ReadSigned(u);
                var op = d.Ea(u, size, 11, offset);
                d.ops.Add(op);
                return true;
            };
        }
        internal static readonly Mutator<MipsDisassembler> E11w = E11(PrimitiveType.Word32);


        // Indexed memory address
        private static Mutator<MipsDisassembler> Mx(PrimitiveType dt, int posBase, int posIdx)
        {
            var baseField = new Bitfield(posBase, 5);
            var idxField = new Bitfield(posIdx, 5);
            return (u, d) =>
            {
                var iBase = (int) baseField.Read(u);
                var iIndex = (int) idxField.Read(u);
                var rBase = d.arch.GetRegister(iBase);
                var rIndex = d.arch.GetRegister(iIndex);
                d.ops.Add(new IndexedOperand(dt, rBase, rIndex));
                return true;
            };
        }
        private static readonly Mutator<MipsDisassembler> Mxbu = Mx(PrimitiveType.Byte, 21, 16);
        private static readonly Mutator<MipsDisassembler> Mxh = Mx(PrimitiveType.Word16, 21, 16);
        private static readonly Mutator<MipsDisassembler> Mxw = Mx(PrimitiveType.Word32, 21, 16);
        private static readonly Mutator<MipsDisassembler> Mxd = Mx(PrimitiveType.Word64, 21, 16);

        // trap code
        internal static bool T(uint wInstr, MipsDisassembler dasm)
        {
            var op = ImmediateOperand.Word16((ushort) ((wInstr >> 6) & 0x03FF));
            dasm.ops.Add(op);
            return true;
        }

        // condition code
        internal static Mutator<MipsDisassembler> c(int bitPos)
        {
            return (u, d) =>
            {
                var op = d.CCodeFlag(u, bitPos);
                d.ops.Add(op);
                return true;
            };
        }
        internal static readonly Mutator<MipsDisassembler> c8 = c(8);
        internal static readonly Mutator<MipsDisassembler> c18 = c(18);

        // FPU condition code
        internal static Mutator<MipsDisassembler> C(int bitPos)
        {
            return (u, d) =>
            {
                var op = d.FpuCCodeFlag(u, bitPos);
                d.ops.Add(op);
                return true;
            };
        }
        internal static Mutator<MipsDisassembler> C18 = C(18);

        // hardware register, see instruction rdhwr
        internal static bool H(uint wInstr, MipsDisassembler dasm)
        {
            var op = ImmediateOperand.Byte((byte) ((wInstr >> 11) & 0x1f));
            dasm.ops.Add(op);
            return true;
        }

        internal static Mutator<MipsDisassembler> x(string message)
        {
            return (u, d) =>
            {
                d.NotYetImplemented(u, message);
                return false;
            };
        }

        private RegisterOperand Reg(uint regNumber)
        {
            return new RegisterOperand(arch.GetRegister((int) regNumber & 0x1F));
        }

        private RegisterOperand FReg(uint regNumber)
        {
            return new RegisterOperand(arch.fpuRegs[regNumber & 0x1F]);
        }

        private bool TryGetFCReg(uint regNumber, out RegisterOperand op)
        {
            if (arch.fpuCtrlRegs.TryGetValue(regNumber & 0x1F, out RegisterStorage fcreg))
            {
                op = new RegisterOperand(fcreg);
                return true;
            }
            else
            {
                op = null;
                return false;
            }
        }

        private RegisterOperand CCodeFlag(uint wInstr, int regPos)
        {
            var regNo = (wInstr >> regPos) & 0x7;
            return new RegisterOperand(arch.ccRegs[regNo]);
        }

        private RegisterOperand FpuCCodeFlag(uint wInstr, int regPos)
        {
            var regNo = (wInstr >> regPos) & 0x7;
            return new RegisterOperand(arch.fpuCcRegs[regNo]);
        }

        private AddressOperand RelativeBranch(uint wInstr)
        {
            int off = (short) wInstr;
            off <<= 2;
            return AddressOperand.Create(rdr.Address + off);
        }

        private AddressOperand LargeBranch(uint wInstr)
        {
            var off = (wInstr & 0x03FFFFFF) << 2;
            ulong linAddr = (rdr.Address.ToLinear() & ~0x0FFFFFFFul) | off;
            return AddressOperand.Create(
                Address.Create(arch.PointerType, linAddr));
        }

        private IndirectOperand Ea(uint wInstr, PrimitiveType dataWidth, int shift, short offset)
        {
            var baseReg = arch.GetRegister((int)(wInstr >> shift) & 0x1F);
            return new IndirectOperand(dataWidth, offset, baseReg);
        }
    }
}
