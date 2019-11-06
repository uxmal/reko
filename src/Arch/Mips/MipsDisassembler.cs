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

        private static readonly MaskDecoder<MipsDisassembler, Opcode, MipsInstruction> rootDecoder;

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
                Mnemonic = Opcode.illegal,
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


        private static NyiDecoder<MipsDisassembler, Opcode, MipsInstruction> Nyi(string message)
        {
            return new NyiDecoder<MipsDisassembler, Opcode, MipsInstruction>(message);
        }

        private static InstrDecoder Instr(Opcode mnemonic, params Mutator<MipsDisassembler> [] mutators)
        {
            return new InstrDecoder(InstrClass.Linear, mnemonic, mutators);
        }

        private static InstrDecoder Instr(InstrClass iclass, Opcode mnemonic, params Mutator<MipsDisassembler>[] mutators)
        {
            return new InstrDecoder(iclass, mnemonic, mutators);
        }

        static MipsDisassembler()
        {
            var invalid = new InstrDecoder(Opcode.illegal);

            var cop1_s = Mask(0, 6, "FPU (single)",
                new InstrDecoder(Opcode.add_s, F4,F3,F2),
                new InstrDecoder(Opcode.sub_s, F4,F3,F2),
                new InstrDecoder(Opcode.mul_s, F4,F3,F2),
                new InstrDecoder(Opcode.div_s, F4,F3,F2),
                invalid,
                invalid,
                new InstrDecoder(Opcode.mov_s, F4,F3),
                new InstrDecoder(Opcode.neg_s, F4,F3),

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
                new InstrDecoder(Opcode.c_eq_s, c8,F3,F2),
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,
                new InstrDecoder(Opcode.c_lt_s, c8,F3,F2),
                invalid,
                new InstrDecoder(Opcode.c_le_s, c8,F3,F2),
                invalid);

            var cop1_d = Mask(0, 6, "FPU (double)",
                // fn 00
                new InstrDecoder(Opcode.add_d, F4,F3,F2),
                new InstrDecoder(Opcode.sub_d, F4,F3,F2),
                new InstrDecoder(Opcode.mul_d, F4,F3,F2),
                new InstrDecoder(Opcode.div_d, F4,F3,F2),
                invalid,
                invalid,
                new InstrDecoder(Opcode.mov_d, F4,F3),
                new InstrDecoder(Opcode.neg_d, F4,F3),

                invalid,
                new InstrDecoder(Opcode.trunc_l_d, F4,F3),
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
                new InstrDecoder(Opcode.cvt_s_d, F4,F3),
                invalid,
                invalid,
                invalid,
                new InstrDecoder(Opcode.cvt_w_d, F4,F3),
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
                new InstrDecoder(Opcode.c_eq_d, c8,F3,F2),
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,
                new InstrDecoder(Opcode.c_lt_d, c8,F3,F2),
                invalid,
                new InstrDecoder(Opcode.c_le_d, c8,F3,F2),
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
                new InstrDecoder(Opcode.c_lt_d, c8,F3,F2),
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
                new InstrDecoder(Opcode.cvt_s_l, F4,F3),
                new InstrDecoder(Opcode.cvt_d_l, F4,F3),
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
                ( 0x01, new InstrDecoder(Opcode.tlbr ) ),
                ( 0x02, new InstrDecoder(Opcode.tlbwi) ),
                ( 0x06, new InstrDecoder(Opcode.tlbwr) ),
                ( 0x08, new InstrDecoder(Opcode.tlbp ) ),
                ( 0x18, new InstrDecoder(Opcode.eret ) ),
                ( 0x20, new InstrDecoder(Opcode.wait ) ));
            var cop1 = Mask(21, 5, "COP1",
                new InstrDecoder(Opcode.mfc1, R2,F3),
                new A64Decoder(Opcode.dmfc1, R2,F3),
                new InstrDecoder(Opcode.cfc1, R2,f3),
                invalid,
                new InstrDecoder(Opcode.mtc1, R2,F3),
                new A64Decoder(Opcode.dmtc1, R2,F3),
                new InstrDecoder(Opcode.ctc1, R2,f3),
                invalid,

                Mask(16, 1,
                    new InstrDecoder(InstrClass.ConditionalTransfer | InstrClass.Delay, Opcode.bc1f, c18,j),
                    new InstrDecoder(InstrClass.ConditionalTransfer | InstrClass.Delay, Opcode.bc1t, c18,j)),
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
                 new InstrDecoder(Opcode.lwc2,R2,E11w),
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
                (0x0, new InstrDecoder(Opcode.madd, R1, R2)),
                (0x1, new InstrDecoder(Opcode.maddu, R1, R2)),
                (0x2, new InstrDecoder(Opcode.mul, R3, R1, R2)),
                (0x4, new InstrDecoder(Opcode.msub, R1, R2)),
                (0x5, new InstrDecoder(Opcode.msubu, R1, R2)),

                (0x20, new InstrDecoder(Opcode.clz, R3, R1)),
                (0x21, new InstrDecoder(Opcode.clo, R3, R1)),
                (0x3F, new InstrDecoder(Opcode.sdbbp, Imm(PrimitiveType.UInt32, 6, 20))));

            var condDecoders = Mask(16, 5, "CondDecoders",
                new InstrDecoder(DCT, Opcode.bltz, R1, j),
                new InstrDecoder(DCT, Opcode.bgez, R1, j),
                new InstrDecoder(DCT, Opcode.bltzl, R1, j),
                new InstrDecoder(DCT, Opcode.bgezl, R1, j),

                invalid,
                invalid,
                invalid,
                invalid,

                new InstrDecoder(CTD, Opcode.tgei, R1, I),
                new InstrDecoder(CTD, Opcode.tgeiu, R1, I),
                new InstrDecoder(CTD, Opcode.tlti, R1, I),
                new InstrDecoder(CTD, Opcode.tltiu, R1, I),

                new InstrDecoder(CTD, Opcode.teqi, R1, I),
                invalid,
                new InstrDecoder(CTD, Opcode.tnei, R1, I),
                invalid,

                new InstrDecoder(CTD, Opcode.bltzal, R1, j),
                new InstrDecoder(CTD, Opcode.bgezal, R1, j),
                new InstrDecoder(CTD, Opcode.bltzall, R1, j),
                new InstrDecoder(CTD, Opcode.bgezall, R1, j),

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
                new InstrDecoder(Opcode.wsbh, x("")),
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
                new InstrDecoder(Opcode.seb, R3, R2),
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                new InstrDecoder(Opcode.seh, R3, R2),
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
                new InstrDecoder(Opcode.ll, R2,ew)),
            new Version6Decoder(
                invalid,
                new A64Decoder(Opcode.lld, R2,el)),

            invalid,
            invalid,
            invalid,
            new InstrDecoder(Opcode.rdhwr, R2,H),
            invalid,
            invalid,
            invalid,
            invalid);

            var special = Mask(0, 6, "Special",
                Select((6, 5), n => n == 0,
                    new InstrDecoder(InstrClass.Linear|InstrClass.Padding, Opcode.nop),
                    new InstrDecoder(Opcode.sll, R3, R2, s)),
                Mask(16, 1,
                    new InstrDecoder(Opcode.movf, R2, R1, C18),
                    new InstrDecoder(Opcode.movt, R2, R1, C18)),
                new InstrDecoder(Opcode.srl, R3, R2, s),
                new InstrDecoder(Opcode.sra, R3, R2, s),

                new InstrDecoder(Opcode.sllv, R3, R2, R1),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.srlv, R3, R2, R1),
                new InstrDecoder(Opcode.srav, R3, R2, R1),

                new InstrDecoder(TD, Opcode.jr, R1),
                new InstrDecoder(CTD, Opcode.jalr, R3, R1),
                new InstrDecoder(Opcode.movz, R3, R1, R2),
                new InstrDecoder(Opcode.movn, R3, R1, R2),
                new InstrDecoder(Opcode.syscall, B),
                new InstrDecoder(Opcode.@break, B),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.sync, s),
                // 10
                new InstrDecoder(Opcode.mfhi, R3),
                new InstrDecoder(Opcode.mthi, R1),
                new InstrDecoder(Opcode.mflo, R3),
                new InstrDecoder(Opcode.mtlo, R1),
                new A64Decoder(Opcode.dsllv, R3, R2, R1),
                new InstrDecoder(Opcode.illegal),
                new A64Decoder(Opcode.dsrlv, R3, R2, R1),
                new A64Decoder(Opcode.dsrav, R3, R2, R1),

                new InstrDecoder(Opcode.mult, R1, R2),
                new InstrDecoder(Opcode.multu, R1, R2),
                new InstrDecoder(Opcode.div, R1, R2),
                new InstrDecoder(Opcode.divu, R1, R2),
                new A64Decoder(Opcode.dmult, R1, R2),
                new A64Decoder(Opcode.dmultu, R1, R2),
                new A64Decoder(Opcode.ddiv, R1, R2),
                new A64Decoder(Opcode.ddivu, R1, R2),
                // 20
                new InstrDecoder(Opcode.add, R3, R1, R2),
                new InstrDecoder(Opcode.addu, R3, R1, R2),
                new InstrDecoder(Opcode.sub, R3, R1, R2),
                new InstrDecoder(Opcode.subu, R3, R1, R2),
                new InstrDecoder(Opcode.and, R3, R1, R2),
                new InstrDecoder(Opcode.or, R3, R1, R2),
                new InstrDecoder(Opcode.xor, R3, R1, R2),
                new InstrDecoder(Opcode.nor, R3, R1, R2),

                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(Opcode.slt, R3, R1, R2),
                new InstrDecoder(Opcode.sltu, R3, R1, R2),
                new A64Decoder(Opcode.dadd, R3, R1, R2),
                new A64Decoder(Opcode.daddu, R3, R1, R2),
                new A64Decoder(Opcode.dsub, R3, R1, R2),
                new A64Decoder(Opcode.dsubu, R3, R1, R2),
                // 30
                new InstrDecoder(CTD, Opcode.tge, R1, R2, T),
                new InstrDecoder(CTD, Opcode.tgeu, R1, R2, T),
                new InstrDecoder(CTD, Opcode.tlt, R1, R2, T),
                new InstrDecoder(CTD, Opcode.tltu, R1, R2, T),
                new InstrDecoder(CTD, Opcode.teq, R1, R2, T),
                new InstrDecoder(Opcode.illegal),
                new InstrDecoder(CTD, Opcode.tne, R1, R2, T),
                new InstrDecoder(Opcode.illegal),

                new A64Decoder(Opcode.dsll, R3, R2, s),
                new InstrDecoder(Opcode.illegal),
                new A64Decoder(Opcode.dsrl, R3, R2, s),
                new A64Decoder(Opcode.dsra, R3, R2, s),
                new A64Decoder(Opcode.dsll32, R3, R2, s),
                new InstrDecoder(Opcode.illegal),
                new A64Decoder(Opcode.dsrl32, R3, R2, s),
                new A64Decoder(Opcode.dsra32, R3, R2, s));

            var cop1x = Mask(0, 6,
                Instr(Opcode.lwxc1, F3,Mxw),
                Instr(Opcode.ldxc1, F3,Mxd),
                invalid,
                invalid,

                invalid,
                Instr(Opcode.luxc1, F4,Mxw),
                invalid,
                invalid,

                Instr(Opcode.swxc1, F3,Mxw),
                Instr(Opcode.sdxc1, F3,Mxd),
                invalid,
                invalid,

                invalid,
                Instr(Opcode.suxc1, F4,Mxw),
                invalid,
                new InstrDecoder(Opcode.prefx, Imm(PrimitiveType.Byte, 11,5), Mxw),
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
                new InstrDecoder(Opcode.alnv_ps, F4,F3,F2,R1),
                invalid,
                // 20
                new InstrDecoder(Opcode.madd_s, F4,F1,F3,F2),
                new InstrDecoder(Opcode.madd_d, F4,F1,F3,F2),
                invalid,
                invalid,

                invalid,
                invalid,
                new InstrDecoder(Opcode.madd_ps, F4, F1, F3, F2),
                invalid,

                new InstrDecoder(Opcode.msub_s, F4, F1, F3, F2),
                new InstrDecoder(Opcode.msub_d, F4, F1, F3, F2),
                invalid,
                invalid,

                invalid,
                invalid,
                new InstrDecoder(Opcode.msub_ps, F4, F1, F3, F2),
                invalid,
                // 30
                new InstrDecoder(Opcode.nmadd_s, F4, F1, F3, F2),
                new InstrDecoder(Opcode.nmadd_d, F4, F1, F3, F2),
                invalid,
                invalid,

                invalid,
                invalid,
                new InstrDecoder(Opcode.nmadd_ps, F4, F1, F3, F2),
                invalid,

                new InstrDecoder(Opcode.nmsub_s, F4, F1, F3, F2),
                new InstrDecoder(Opcode.nmsub_d, F4, F1, F3, F2),
                invalid,
                invalid,

                invalid,
                invalid,
                new InstrDecoder(Opcode.nmsub_ps, F4, F1, F3, F2),
                invalid);


        rootDecoder = Mask(26, 6,
                special,
                condDecoders,
                new InstrDecoder(TD, Opcode.j, J),
                new InstrDecoder(CTD, Opcode.jal, J),
                new InstrDecoder(DCT, Opcode.beq, R1,R2,j),
                new InstrDecoder(DCT, Opcode.bne, R1,R2,j),
                new InstrDecoder(DCT, Opcode.blez, R1,j),
                new InstrDecoder(DCT, Opcode.bgtz, R1,j),

                new InstrDecoder(Opcode.addi, R2,R1,I),
                new InstrDecoder(Opcode.addiu, R2,R1,I),
                new InstrDecoder(Opcode.slti, R2,R1,I),
                new InstrDecoder(Opcode.sltiu, R2,R1,I),

                new InstrDecoder(Opcode.andi, R2,R1,U),
                new InstrDecoder(Opcode.ori, R2,R1,U),
                new InstrDecoder(Opcode.xori, R2,R1,U),
                new InstrDecoder(Opcode.lui, R2,i),
                // 10
                Mask(21, 5, "Coprocessor",
                    new InstrDecoder(Opcode.mfc0, R2,R3),
                    new InstrDecoder(Opcode.dmfc0, R2,R3),
                    invalid,
                    invalid,
                    new InstrDecoder(Opcode.mtc0, R2,R3),
                    new InstrDecoder(Opcode.dmtc0, R2,R3),
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
                
                new InstrDecoder(DCT, Opcode.beql, R1,R2,j),
                new InstrDecoder(DCT, Opcode.bnel, R1,R2,j),
                new InstrDecoder(DCT, Opcode.blezl, R1,j),
                new InstrDecoder(DCT, Opcode.bgtzl, R1,j),

                new A64Decoder(Opcode.daddi, R2,R1,I),
                new A64Decoder(Opcode.daddiu, R2,R1,I),
                new A64Decoder(Opcode.ldl, R2,El),
                new A64Decoder(Opcode.ldr, R2,El),

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
                new InstrDecoder(Opcode.lb, R2,EB),
                new InstrDecoder(Opcode.lh, R2,EH),
                new InstrDecoder(Opcode.lwl, R2,Ew),
                new InstrDecoder(Opcode.lw, R2,Ew),

                new InstrDecoder(Opcode.lbu, R2,Eb),
                new InstrDecoder(Opcode.lhu, R2,Eh),
                new InstrDecoder(Opcode.lwr, R2,Ew),
                new A64Decoder(Opcode.lwu, R2,Ew),

                new InstrDecoder(Opcode.sb, R2,Eb),
                new InstrDecoder(Opcode.sh, R2,Eh),
                new InstrDecoder(Opcode.swl, R2,Ew),
                new InstrDecoder(Opcode.sw, R2,Ew),

                new InstrDecoder(Opcode.sdl, R2,Ew),
                new InstrDecoder(Opcode.sdr, R2,Ew),
                new InstrDecoder(Opcode.swr, R2,Ew),
                new Version6Decoder(
                    new InstrDecoder(Opcode.cache, Imm(PrimitiveType.Byte, 16, 5), Ew),
                    invalid),

                // 30
                new Version6Decoder(
                    new InstrDecoder(Opcode.ll, R2,Ew),
                    invalid),
                new InstrDecoder(Opcode.lwc1, F2,Ew),
                new Version6Decoder(
                    new InstrDecoder(Opcode.lwc2, R2, El),
                    Nyi("BC-v6")),
                new InstrDecoder(Opcode.pref, R2,Ew),

                new Version6Decoder(
                    new A64Decoder(Opcode.lld, R2,El),
                    invalid),
                new InstrDecoder(Opcode.ldc1, F2,El),
                new Version6Decoder(
                    new InstrDecoder(Opcode.ldc2, R2,El),
                    Nyi("POP76")),
                new A64Decoder(Opcode.ld, R2,El),

                new Version6Decoder(
                    new InstrDecoder(Opcode.sc, R2,Ew),
                    invalid),
                new InstrDecoder(Opcode.swc1, F2,Ew),
                new Version6Decoder(
                    new InstrDecoder(Opcode.swc2, R2, Ew),
                    Nyi("BALC-v6")),
                new Version6Decoder(
                    invalid,
                    Nyi("PCREL-v6")),

                new Version6Decoder(
                    new A64Decoder(Opcode.scd, R2,El),
                    invalid),
                new A64Decoder(Opcode.sdc1, F2,El),
                new Version6Decoder(
                    new InstrDecoder(Opcode.sdc2, R2,El),
                    Nyi("POP76")),
                new A64Decoder(Opcode.sd, R2,El));
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
