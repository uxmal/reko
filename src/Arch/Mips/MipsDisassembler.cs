#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
    public class MipsDisassembler : DisassemblerBase<MipsInstruction>
    {
        internal readonly MipsProcessorArchitecture arch;
        internal readonly bool isVersion6OrLater;
        private MipsInstruction instrCur;
        private Address addr;
        private readonly EndianImageReader rdr;
        private readonly PrimitiveType signedWord;

        public MipsDisassembler(MipsProcessorArchitecture arch, EndianImageReader imageReader, bool isVersion6OrLater)
        {
            this.arch = arch;
            this.rdr = imageReader;
            this.isVersion6OrLater = isVersion6OrLater;
            this.signedWord = PrimitiveType.CreateB(Domain.SignedInt, arch.WordWidth.BitSize);
        }

        public override MipsInstruction DisassembleInstruction()
        {
            if (!rdr.IsValid)
                return null;
            this.addr = rdr.Address;
            if (!rdr.TryReadUInt32(out uint wInstr))
            {
                return null;
            }
            var opRec = opRecs[wInstr >> 26];
            try
            {
                if (opRec == null)
                    instrCur = new MipsInstruction { opcode = Opcode.illegal };
                else
                    instrCur = opRec.Decode(wInstr, this);
            }
            catch
            {
                instrCur = null;
            }
            if (instrCur == null)
            {
                instrCur = new MipsInstruction { opcode = Opcode.illegal };
            }
            EmitUnitTest(wInstr, instrCur);
            instrCur.Address = this.addr;
            instrCur.Length = 4;
            return instrCur;
        }

        [Conditional("DEBUG")]
        public void EmitUnitTest(uint wInstr, MipsInstruction instr)
        {
#if DEBUG
            if (instr.opcode != Opcode.illegal)
                return;
            if (seen.Contains(wInstr))
                return;
            var op = (wInstr >> 26);
            if (op == 0 || op == 1)
                return;
            seen.Add(wInstr);
            Debug.Print(
@"        [Test]
        public void MipsDis_{0:X8}()
        {{
            AssertCode(""@@@"", 0x{0:X8}); // {1:X2}
        }}
",
            wInstr, op);
#endif
        }
#if DEBUG
        private static HashSet<uint> seen = new HashSet<uint>();
#endif

        private static OpRec[] opRecs;

        static MipsDisassembler()
        {
            var cop1_s = new FpuOpRec(PrimitiveType.Real32,
                new AOpRec(Opcode.add_s, "F4,F3,F2"),
                new AOpRec(Opcode.sub_s, "F4,F3,F2"),
                new AOpRec(Opcode.mul_s, "F4,F3,F2"),
                new AOpRec(Opcode.div_s, "F4,F3,F2"),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.mov_s, "F4,F3"),
                new AOpRec(Opcode.neg_s, "F4,F3"),

                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),

                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),

                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),

                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),

                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),

                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.c_eq_s, "c8,F3,F2"),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),

                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.c_lt_s, "c8,F3,F2"),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.c_le_s, "c8,F3,F2"),
                new AOpRec(Opcode.illegal, ""));

            var cop1_d = new FpuOpRec(PrimitiveType.Real64,
                // fn 00
                new AOpRec(Opcode.add_d, "F4,F3,F2"),
                new AOpRec(Opcode.sub_d, "F4,F3,F2"),
                new AOpRec(Opcode.mul_d, "F4,F3,F2"),
                new AOpRec(Opcode.div_d, "F4,F3,F2"),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.mov_d, "F4,F3"),
                new AOpRec(Opcode.neg_d, "F4,F3"),

                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.trunc_l_d, "F4,F3"),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),

                // fn 10
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),

                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),

                // fn 20
                new AOpRec(Opcode.cvt_s_d, "F4,F3"),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.cvt_w_d, "F4,F3"),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),

                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),

                // fn 30
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.c_eq_d, "c8,F3,F2"),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),

                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.c_lt_d, "c8,F3,F2"),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.c_le_d, "c8,F3,F2"),
                new AOpRec(Opcode.illegal, ""));

            var cop1_w = new FpuOpRec(PrimitiveType.Int64,
                // fn 00
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),

                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),

                // fn 10
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),

                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),

                // fn 20
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),

                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),

                // fn 30
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),

                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.c_lt_d, "c8,F3,F2"),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""));

            var cop1_l = new FpuOpRec(PrimitiveType.Int64,
                // fn 00
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),

                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),

                // fn 10
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),

                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),

                // fn 20
                new AOpRec(Opcode.cvt_s_l, "F4,F3"),
                new AOpRec(Opcode.cvt_d_l, "F4,F3"),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),

                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),

                // fn 30
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),

                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""));

            var cop0_C0_decoder = new SparseMaskDecoder(0, 0x3F, new Dictionary<uint, OpRec>
            {
                { 0x01, new AOpRec(Opcode.tlbr , "") },
                { 0x02, new AOpRec(Opcode.tlbwi, "") },
                { 0x06, new AOpRec(Opcode.tlbwr, "") },
                { 0x08, new AOpRec(Opcode.tlbp , "") },
                { 0x18, new AOpRec(Opcode.eret , "") },
                { 0x20, new AOpRec(Opcode.wait , "") },
            });
            var cop1 = new CoprocessorOpRec(
                new AOpRec(Opcode.mfc1, "R2,F3"),
                new A64OpRec(Opcode.dmfc1, "R2,F3"),
                new AOpRec(Opcode.cfc1, "R2,f3"),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.mtc1, "R2,F3"),
                new A64OpRec(Opcode.dmtc1, "R2,F3"),
                new AOpRec(Opcode.ctc1, "R2,f3"),
                new AOpRec(Opcode.illegal, ""),

                new BcNRec(Opcode.bc1f, Opcode.bc1t),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),

                cop1_s,
                cop1_d,
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                cop1_w,
                cop1_l,
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),

                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""),
                new AOpRec(Opcode.illegal, ""));

            var special2 = new SparseMaskDecoder(0, 0x3F, new Dictionary<uint, OpRec>
            {
                { 0x2, new Version6OpRec(
                    new AOpRec(Opcode.mul, "R3,R1,R2"),
                    new AOpRec(Opcode.illegal, ""))
                }
            });
            opRecs = new OpRec[]
            {
                new SpecialOpRec(),
                new CondOpRec(),
                new AOpRec(Opcode.j, "J"),
                new AOpRec(Opcode.jal, "J"),
                new AOpRec(Opcode.beq, "R1,R2,j"),
                new AOpRec(Opcode.bne, "R1,R2,j"),
                new AOpRec(Opcode.blez, "R1,j"),
                new AOpRec(Opcode.bgtz, "R1,j"),

                new AOpRec(Opcode.addi, "R2,R1,I"),
                new AOpRec(Opcode.addiu, "R2,R1,I"),
                new AOpRec(Opcode.slti, "R2,R1,I"),
                new AOpRec(Opcode.sltiu, "R2,R1,I"),

                new AOpRec(Opcode.andi, "R2,R1,U"),
                new AOpRec(Opcode.ori, "R2,R1,U"),
                new AOpRec(Opcode.xori, "R2,R1,U"),
                new AOpRec(Opcode.lui, "R2,i"),
                // 10
                new CoprocessorOpRec(
                    new AOpRec(Opcode.mfc0, "R2,R3"),
                    new AOpRec(Opcode.dmfc0, "R2,R3"),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.mtc0, "R2,R3"),
                    new AOpRec(Opcode.dmtc0, "R2,R3"),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),

                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),

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
               // 11: COP1 encodings
                cop1,

               new CoprocessorOpRec(  // 12: COP2 
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),

                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),

                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),

                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, ""),
                    new AOpRec(Opcode.illegal, "")),
                null,   // COP1X
                new AOpRec(Opcode.beql, "R1,R2,j"),
                new AOpRec(Opcode.bnel, "R1,R2,j"),
                new AOpRec(Opcode.blezl, "R1,j"),
                new AOpRec(Opcode.bgtzl, "R1,j"),

                new A64OpRec(Opcode.daddi, "R2,R1,I"),
                new A64OpRec(Opcode.daddiu, "R2,R1,I"),
                new A64OpRec(Opcode.ldl, "R2,El"),
                new A64OpRec(Opcode.ldr, "R2,El"),

                special2,
                null,
                null,
                new Special3OpRec(), 

                // 20
                new AOpRec(Opcode.lb, "R2,EB"),
                new AOpRec(Opcode.lh, "R2,EH"),
                new AOpRec(Opcode.lwl, "R2,Ew"),
                new AOpRec(Opcode.lw, "R2,Ew"),

                new AOpRec(Opcode.lbu, "R2,Eb"),
                new AOpRec(Opcode.lhu, "R2,Eh"),
                new AOpRec(Opcode.lwr, "R2,Ew"),
                new A64OpRec(Opcode.lwu, "R2,Ew"),

                new AOpRec(Opcode.sb, "R2,Eb"),
                new AOpRec(Opcode.sh, "R2,Eh"),
                new AOpRec(Opcode.swl, "R2,Ew"),
                new AOpRec(Opcode.sw, "R2,Ew"),

                new AOpRec(Opcode.sdl, "R2,Ew"),
                new AOpRec(Opcode.sdr, "R2,Ew"),
                new AOpRec(Opcode.swr, "R2,Ew"),
                null,

                // 30
                new Version6OpRec(
                    new AOpRec(Opcode.ll, "R2,Ew"),
                    new AOpRec(Opcode.illegal, "")),
                new AOpRec(Opcode.lwc1, "F2,Ew"),
                null,
                new AOpRec(Opcode.pref, "R2,Ew"),

                new Version6OpRec(
                    new A64OpRec(Opcode.lld, "R2,El"),
                    new AOpRec(Opcode.illegal, "")),
                new AOpRec(Opcode.ldc1, "F2,El"),
                null,
                new A64OpRec(Opcode.ld, "R2,El"),

                new Version6OpRec(
                    new AOpRec(Opcode.sc, "R2,Ew"),
                    new AOpRec(Opcode.illegal, "")),
                new AOpRec(Opcode.swc1, "F2,Ew"),
                null,
                null,

                new Version6OpRec(
                    new A64OpRec(Opcode.scd, "R2,El"),
                    new AOpRec(Opcode.illegal, "")),
                new A64OpRec(Opcode.sdc1, "F2,El"),
                null,
                new A64OpRec(Opcode.sd, "R2,El")
            };
        }

        public MipsInstruction DecodeOperands(Opcode opcode, uint wInstr, string opFmt)
        {
            var ops = new List<MachineOperand>();
            MachineOperand op = null;
            for (int i = 0; i < opFmt.Length; ++i)
            {
                switch (opFmt[i])
                {
                default: throw new NotImplementedException(string.Format("Operator format {0}", opFmt[i]));
                case ',':
                    continue;
                case 'R':
                    switch (opFmt[++i])
                    {
                    case '1': op = Reg(wInstr >> 21); break;
                    case '2': op = Reg(wInstr >> 16); break;
                    case '3': op = Reg(wInstr >> 11); break;
                    //case '4': op = MemOff(wInstr >> 6, wInstr); break;
                    default: throw new NotImplementedException(string.Format("Register field {0}.", opFmt[i]));
                    }
                    break;
                case 'F':
                    switch (opFmt[++i])
                    {
                    case '1': op = FReg(wInstr >> 21); break;
                    case '2': op = FReg(wInstr >> 16); break;
                    case '3': op = FReg(wInstr >> 11); break;
                    case '4': op = FReg(wInstr >> 6); break;
                    default: throw new NotImplementedException(string.Format("Register field {0}.", opFmt[i]));
                    }
                    break;
                case 'f': // FPU control register
                    RegisterOperand fcreg;
                    switch (opFmt[++i])
                    {
                    case '1': if (!TryGetFCReg(wInstr >> 21, out fcreg)) return null; op = fcreg; break;
                    case '2': if (!TryGetFCReg(wInstr >> 16, out fcreg)) return null; op = fcreg; break;
                    case '3': if (!TryGetFCReg(wInstr >> 11, out fcreg)) return null; op = fcreg; break;
                    //case '4': op = MemOff(wInstr >> 6, wInstr); break;
                    default: throw new NotImplementedException(string.Format("Register field {0}.", opFmt[i]));
                    }
                    break;
                case 'I':
                    op = new ImmediateOperand(Constant.Create(this.signedWord, (short)wInstr));
                    break;
                case 'U':
                    op = new ImmediateOperand(Constant.Create(arch.WordWidth, (ushort) wInstr));
                    break;
                case 'i':
                    op = ImmediateOperand.Int16((short) wInstr);
                    break;
                case 'j':
                    op = RelativeBranch(wInstr);
                    break;
                case 'J':
                    op = LargeBranch(wInstr);
                    break;
                case 'B':
                    op = ImmediateOperand.Word32((wInstr >> 6) & 0xFFFFF);
                    break;
                case 's':   // Shift amount or sync type
                    op = ImmediateOperand.Byte((byte)((wInstr >> 6) & 0x1F));
                    break;
                case 'E':   // effective address w 16-bit offset
                    op = Ea(wInstr, opFmt[++i], 21, (short)wInstr);
                    break;
                case 'e':   // effective address w 9-bit offset
                    op = Ea(wInstr, opFmt[++i], 21, (short)(((short)wInstr) >> 7));
                    break;
                case 'T':   // trap code
                    op = ImmediateOperand.Word16((ushort)((wInstr >> 6) & 0x03FF));
                    break;
                case 'c':   // condition code
                    op = CCodeFlag(wInstr, opFmt, ref i);
                    break;
                case 'C': // FPU condition code
                    op = FpuCCodeFlag(wInstr, opFmt, ref i);
                    break;
                case 'H':   // hardware register, see instruction rdhwr
                    op = ImmediateOperand.Byte((byte)((wInstr >> 11) & 0x1f));
                    break;
                }
                ops.Add(op);
            }
            return new MipsInstruction
            {
                opcode = opcode,
                Address = addr,
                Length = 4,
                op1 = ops.Count > 0 ? ops[0] : null,
                op2 = ops.Count > 1 ? ops[1] : null,
                op3 = ops.Count > 2 ? ops[2] : null,
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
            RegisterStorage fcreg;
            if (arch.fpuCtrlRegs.TryGetValue(regNumber & 0x1F, out fcreg))
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

        private RegisterOperand CCodeFlag(uint wInstr, string fmt, ref int i)
        {
            int pos = 0;
            while (Char.IsDigit(fmt[++i]))
            {
                pos = pos * 10 + fmt[i] - '0';
            }
            var regNo = (wInstr >> pos) & 0x7;
            return new RegisterOperand(arch.ccRegs[regNo]);
        }

        private RegisterOperand FpuCCodeFlag(uint wInstr, string fmt, ref int i)
        {
            int pos = 0;
            ++i;
            while (i < fmt.Length && Char.IsDigit(fmt[i]))
            {
                pos = pos * 10 + fmt[i] - '0';
                ++i;
            }
            var regNo = (wInstr >> pos) & 0x7;
            --i;
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

        private IndirectOperand Ea(uint wInstr, char wCode, int shift, short offset)
        {
            PrimitiveType dataWidth;
            switch (wCode)
            {
            default: throw new NotSupportedException(string.Format("Unknown width code '{0}'.", wCode));
            case 'b': dataWidth = PrimitiveType.Byte; break;
            case 'B': dataWidth = PrimitiveType.SByte; break;
            case 'h': dataWidth = PrimitiveType.Word16; break;
            case 'H': dataWidth = PrimitiveType.Int16; break;
            case 'w': dataWidth = PrimitiveType.Word32; break;
            case 'W': dataWidth = PrimitiveType.Int32; break;
            case 'l': dataWidth = PrimitiveType.Word64; break;
            case 'L': dataWidth = PrimitiveType.Int64; break;
            }
            var baseReg = arch.GetRegister((int)(wInstr >> shift) & 0x1F);
            return new IndirectOperand(dataWidth, offset, baseReg);
        }
    }
}
