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

using System;
using System.Collections.Generic;
using Reko.Core;
using Reko.Core.Machine;
using System.Diagnostics;
using Reko.Core.Types;

namespace Reko.Arch.Alpha
{
    public class AlphaDisassembler : DisassemblerBase<AlphaInstruction>
    {
#if DEBUG
        private static HashSet<string> seen = new HashSet<string>();
#endif
        private AlphaArchitecture arch;
        private EndianImageReader rdr;

        public AlphaDisassembler(AlphaArchitecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
        }

        public override AlphaInstruction DisassembleInstruction()
        {
            var addr = rdr.Address;
            if (!rdr.TryReadUInt32(out uint uInstr))
                return null;
            var op = uInstr >> 26;
            var instr = decoders[op].Decode(uInstr, this);
            instr.Address = addr;
            instr.Length = 4;
            instr.iclass |= uInstr == 0 ? InstrClass.Zero : 0;
            return instr;
        }

        private RegisterOperand AluRegister(uint n)
        {
            return new RegisterOperand(Registers.AluRegisters[n & 0x1F]);
        }

        private RegisterOperand FpuRegister(uint n)
        {
            return new RegisterOperand(Registers.FpuRegisters[n & 0x1F]);
        }

        private AlphaInstruction Invalid()
        {
            return new AlphaInstruction { Opcode = Opcode.invalid, iclass = InstrClass.Invalid };
        }

        private AlphaInstruction Nyi(uint uInstr)
        {
            return Nyi(uInstr, string.Format("{0:X2}", uInstr >> 26));

        }
        private AlphaInstruction Nyi(uint uInstr, int functionCode)
        {
            return Nyi(uInstr, string.Format("{0:X2}_{1:X2}", uInstr >> 26, functionCode));
        }

        // The correct decoder for this instruction has _n_ot _y_et been
        // _i_mplemented, so fall back on generating an Invalid instruction.
        private AlphaInstruction Nyi(uint uInstr, string pattern)
        {
#if DEBUG
            // Emit the text of a unit test that can be pasted into the unit tests 
            // for this disassembler.
            if (false && !seen.Contains(pattern))
            {
                seen.Add(pattern);
                Debug.Print(
@"        [Test]
        public void AlphaDis_{1}()
        {{
            var instr = DisassembleWord(0x{0:X8});
            AssertCode(""AlphaDis_{1}"", 0x{0:X8}, ""@@@"", instr.ToString());
        }}
", uInstr, pattern);
            }
#endif
            return Invalid();
        }

        private abstract class Decoder
        {
            public abstract AlphaInstruction Decode(uint uInstr, AlphaDisassembler dasm);
        }

        private class MemOpRec : Decoder
        {
            private Opcode opcode;

            public MemOpRec(Opcode opcode)
            {
                this.opcode = opcode;
            }

            public override AlphaInstruction Decode(uint uInstr, AlphaDisassembler dasm)
            {
                return new AlphaInstruction
                {
                    Opcode = this.opcode,
                    iclass = InstrClass.Linear,
                    op1 = dasm.AluRegister(uInstr >> 21),
                    op2 = new MemoryOperand(
                        PrimitiveType.Word32,    // Dummy value
                        dasm.AluRegister(uInstr >> 16).Register,
                        (short)uInstr)
                };
            }
        }

        private class FMemOpRec : Decoder
        {
            private Opcode opcode;

            public FMemOpRec(Opcode opcode)
            {
                this.opcode = opcode;
            }

            public override AlphaInstruction Decode(uint uInstr, AlphaDisassembler dasm)
            {
                return new AlphaInstruction
                {
                    Opcode = this.opcode,
                    iclass = InstrClass.Linear,
                    op1 = dasm.FpuRegister(uInstr >> 21),
                    op2 = new MemoryOperand(
                        PrimitiveType.Word32,    // Dummy value
                        dasm.AluRegister(uInstr >> 16).Register,
                        (short)uInstr)
                };
            }
        }

        private class JMemOpRec : Decoder
        {
            private readonly static Opcode[] opcodes = {
                Opcode.jmp, Opcode.jsr, Opcode.ret, Opcode.jsr_coroutine
            };

            private readonly static InstrClass[] iclasses =
            {
                InstrClass.Transfer,
                InstrClass.Transfer|InstrClass.Call,
                InstrClass.Transfer,
                InstrClass.Transfer|InstrClass.Call,
            };

            public JMemOpRec()
            {
            }

            public override AlphaInstruction Decode(uint uInstr, AlphaDisassembler dasm)
            {
                return new AlphaInstruction
                {
                    Opcode = opcodes[(uInstr >> 14) & 0x3],
                    iclass = iclasses[(uInstr>> 14) & 0x3],
                    op1 = dasm.AluRegister(uInstr >> 21),
                    op2 = dasm.AluRegister(uInstr >> 16)
                };
            }
        }


        private class MemFnOpRec : Decoder
        {
            public override AlphaInstruction Decode(uint uInstr, AlphaDisassembler dasm)
            {
                throw new NotImplementedException();
            }
        }

        private class BranchOpRec : Decoder
        {
            private Opcode opcode;

            public BranchOpRec(Opcode opcode)
            {
                this.opcode = opcode;
            }

            public override AlphaInstruction Decode(uint uInstr, AlphaDisassembler dasm)
            {
                int offset = ((int)uInstr << 11) >> 9;
                var op1 = dasm.AluRegister(uInstr >> 21);
                var op2 = AddressOperand.Create(dasm.rdr.Address + offset);
                return new AlphaInstruction
                {
                    Opcode = this.opcode,
                    iclass = InstrClass.ConditionalTransfer,
                    op1 = op1,
                    op2 = op2,
                };
            }
        }

        private class FBranchOpRec : Decoder
        {
            private Opcode opcode;

            public FBranchOpRec(Opcode opcode)
            {
                this.opcode = opcode;
            }

            public override AlphaInstruction Decode(uint uInstr, AlphaDisassembler dasm)
            {
                int offset = ((int)uInstr << 11) >> 9;
                var op1 = dasm.FpuRegister(uInstr >> 21);
                var op2 = AddressOperand.Create(dasm.rdr.Address + offset);
                return new AlphaInstruction
                {
                    Opcode = this.opcode,
                    iclass = InstrClass.ConditionalTransfer,
                    op1 = op1,
                    op2 = op2,
                };
            }
        }



        private class RegOpRec : Decoder
        {
            private Opcode opcode;

            public RegOpRec(Opcode opcode)
            {
                this.opcode = opcode;
            }

            public override AlphaInstruction Decode(uint uInstr, AlphaDisassembler dasm)
            {
                var op1 = dasm.AluRegister(uInstr >> 21);
                var op2 = (uInstr & (1 << 12)) != 0
                    ? ImmediateOperand.Byte((byte)(uInstr >> 13))
                    : (MachineOperand) dasm.AluRegister(uInstr >> 13);
                var op3 = dasm.AluRegister(uInstr);
                return new AlphaInstruction
                {
                    Opcode = this.opcode,
                    iclass = InstrClass.Linear,
                    op1 = op1,
                    op2 = op2,
                    op3 = op3,
                };
            }
        }

        private class OperateOpRec : Decoder
        {
            private Dictionary<int, RegOpRec> decoders;

            public OperateOpRec(Dictionary<int, RegOpRec> decoders)
            {
                this.decoders = decoders;
            }

            public override AlphaInstruction Decode(uint uInstr, AlphaDisassembler dasm)
            {
                var functionCode = ((int)uInstr >> 5) & 0x7F;
                if (!decoders.TryGetValue(functionCode, out var decoder))
                    return dasm.Invalid();
                else
                    return decoder.Decode(uInstr, dasm);
            }
        }

        private class FOperateOpRec : Decoder
        {
            private Dictionary<int, Decoder> decoders;

            public FOperateOpRec(Dictionary<int, Decoder> decoders)
            {
                this.decoders = decoders;
            }

            public override AlphaInstruction Decode(uint uInstr, AlphaDisassembler dasm)
            {
                var functionCode = ((int)uInstr >> 5) & 0x7FF;
                if (!decoders.TryGetValue(functionCode, out var decoder))
                    return dasm.Nyi(uInstr, functionCode);
                else
                    return decoder.Decode(uInstr, dasm);
            }
        }

        private class FRegOpRec : Decoder
        {
            private Opcode opcode;
            public FRegOpRec(Opcode opcode)
            {
                this.opcode = opcode;
            }

            public override AlphaInstruction Decode(uint uInstr, AlphaDisassembler dasm)
            {
                var op1 = dasm.FpuRegister(uInstr >> 21);
                var op2 = dasm.FpuRegister(uInstr >> 16);
                var op3 = dasm.FpuRegister(uInstr);
                return new AlphaInstruction
                {
                    Opcode = this.opcode,
                    iclass = InstrClass.Linear,
                    op1 = op1,
                    op2 = op2,
                    op3 = op3,
                };
            }
        }

        private class PalOpRec : Decoder
        {
            private Dictionary<uint, (Opcode,InstrClass)> opcodes;

            public PalOpRec(Dictionary<uint, (Opcode,InstrClass)> opcodes)
            {
                this.opcodes = opcodes;
            }

            public override AlphaInstruction Decode(uint uInstr, AlphaDisassembler dasm)
            {
                if (!opcodes.TryGetValue(uInstr & 0x0000FFFF, out var opcode))
                    return dasm.Invalid();
                return new AlphaInstruction {
                    Opcode = opcode.Item1,
                    iclass = opcode.Item2
                };
            }
        }

        private class CvtOpRec : Decoder
        {
            private Opcode opcode;
            public CvtOpRec(Opcode opcode)
            {
                this.opcode = opcode;
            }

            public override AlphaInstruction Decode(uint uInstr, AlphaDisassembler dasm)
            {
                var op1 = dasm.FpuRegister(uInstr >> 21);
                var op2 = dasm.FpuRegister(uInstr >> 16);
                return new AlphaInstruction
                {
                    Opcode = this.opcode,
                    iclass = InstrClass.Linear,
                    op1 = op1,
                    op2 = op2,
                };
            }
        }

        private class InvalidOpRec : Decoder
        {
            public override AlphaInstruction Decode(uint uInstr, AlphaDisassembler dasm)
            {
                return dasm.Invalid();
            }
        }

        private class NyiOpRec : Decoder
        {
            public override AlphaInstruction Decode(uint uInstr, AlphaDisassembler dasm)
            {
                return dasm.Invalid();
            }
        }

        private static Decoder[] decoders = new Decoder[64]
        {
            // 00
            new PalOpRec(new Dictionary<uint, (Opcode, InstrClass)>
            {       //$REVIEW: these palcodes are OS-dependent....
                { 0, (Opcode.halt,InstrClass.System|InstrClass.Terminates) }
            }),
            new InvalidOpRec(),
            new InvalidOpRec(),
            new InvalidOpRec(),
            new InvalidOpRec(),
            new InvalidOpRec(),
            new InvalidOpRec(),
            new InvalidOpRec(),

            new MemOpRec(Opcode.lda),
            new MemOpRec(Opcode.ldah),
            new MemOpRec(Opcode.ldbu),
            new MemOpRec(Opcode.ldq_u),

            new MemOpRec(Opcode.ldwu),
            new MemOpRec(Opcode.stw),
            new MemOpRec(Opcode.stb),
            new MemOpRec(Opcode.stq_u),
            // 10
            new OperateOpRec(new Dictionary<int, RegOpRec>
            {
                { 0x00, new RegOpRec(Opcode.addl) },
                { 0x02, new RegOpRec(Opcode.s4addl) },
                { 0x09, new RegOpRec(Opcode.subl) },
                { 0x0B, new RegOpRec(Opcode.s4subl) },
                { 0x0F, new RegOpRec(Opcode.cmpbge) },
                { 0x12, new RegOpRec(Opcode.s8addl) },
                { 0x1B, new RegOpRec(Opcode.s8subl) },
                { 0x1D, new RegOpRec(Opcode.cmpult) },
                { 0x20, new RegOpRec(Opcode.addq) },
                { 0x22, new RegOpRec(Opcode.s4addq) },
                { 0x29, new RegOpRec(Opcode.subq) },
                { 0x2B, new RegOpRec(Opcode.s4subq) },
                { 0x2D, new RegOpRec(Opcode.cmpeq) },
                { 0x32, new RegOpRec(Opcode.s8addq) },
                { 0x3B, new RegOpRec(Opcode.s8subq) },
                { 0x3D, new RegOpRec(Opcode.cmpule) },
                { 0x40, new RegOpRec(Opcode.addl_v) },
                { 0x49, new RegOpRec(Opcode.subl_v) },
                { 0x4D, new RegOpRec(Opcode.cmplt) },
                { 0x60, new RegOpRec(Opcode.addq_v) },
                { 0x69, new RegOpRec(Opcode.subq_v) },
                { 0x6D, new RegOpRec(Opcode.cmple) },
            }),
            new OperateOpRec(new Dictionary<int, RegOpRec> // 11
            {
                { 0x00, new RegOpRec(Opcode.and) },
                { 0x08, new RegOpRec(Opcode.bic) },
                { 0x14, new RegOpRec(Opcode.cmovlbs) },
                { 0x16, new RegOpRec(Opcode.cmovlbc) },
                { 0x20, new RegOpRec(Opcode.bis) },
                { 0x24, new RegOpRec(Opcode.cmovne) },
                { 0x26, new RegOpRec(Opcode.cmovne) },
                { 0x28, new RegOpRec(Opcode.ornot) },
                { 0x40, new RegOpRec(Opcode.xor) },
                { 0x44, new RegOpRec(Opcode.cmovlt) },
                { 0x46, new RegOpRec(Opcode.cmovge) },
                { 0x66, new RegOpRec(Opcode.cmovgt) },
                { 0x6C, new RegOpRec(Opcode.implver) },
            }),
            new OperateOpRec(new Dictionary<int, RegOpRec> // 12
            {
                { 0x02, new RegOpRec(Opcode.mskbl) },
                { 0x06, new RegOpRec(Opcode.extbl) },
                { 0x0B, new RegOpRec(Opcode.insbl) },
                { 0x12, new RegOpRec(Opcode.mskwl) },
                { 0x16, new RegOpRec(Opcode.extwl) },
                { 0x1B, new RegOpRec(Opcode.inswl) },
                { 0x22, new RegOpRec(Opcode.mskll) },
                { 0x26, new RegOpRec(Opcode.extll) },
                { 0x2B, new RegOpRec(Opcode.insll) },
                { 0x30, new RegOpRec(Opcode.zap) },
                { 0x31, new RegOpRec(Opcode.zapnot) },
                { 0x32, new RegOpRec(Opcode.mskql) },
                { 0x34, new RegOpRec(Opcode.srl) },
                { 0x36, new RegOpRec(Opcode.extql) },
                { 0x39, new RegOpRec(Opcode.sll) },
                { 0x3B, new RegOpRec(Opcode.insql) },
                { 0x3C, new RegOpRec(Opcode.src) },
                { 0x52, new RegOpRec(Opcode.mskwh) },
                { 0x57, new RegOpRec(Opcode.inswh) },
                { 0x5A, new RegOpRec(Opcode.extwh) },
                { 0x62, new RegOpRec(Opcode.msklh) },
                { 0x67, new RegOpRec(Opcode.inslh) },
                { 0x6A, new RegOpRec(Opcode.extlh) },
                { 0x72, new RegOpRec(Opcode.mskqh) },
                { 0x77, new RegOpRec(Opcode.insqh) },
                { 0x7A, new RegOpRec(Opcode.extqh) },
            }),
            new OperateOpRec(new Dictionary<int, RegOpRec>  // 13
            {
                { 0x00, new RegOpRec(Opcode.mull) },
                { 0x20, new RegOpRec(Opcode.mulq) },
                { 0x30, new RegOpRec(Opcode.umulh) },
                { 0x40, new RegOpRec(Opcode.mull_v) },
                { 0x60, new RegOpRec(Opcode.mulq_v) },
            }),

            new FOperateOpRec(new Dictionary<int, Decoder> // 14
            {
                { 0x004, new FRegOpRec(Opcode.itofs) },
                { 0x00A, new FRegOpRec(Opcode.sqrtf_c) },
                { 0x00B, new FRegOpRec(Opcode.sqrts_c) },
                { 0x014, new FRegOpRec(Opcode.itoff) },
                { 0x024, new FRegOpRec(Opcode.itoft) },
                { 0x02A, new FRegOpRec(Opcode.sqrtg_c) },
                { 0x02B, new FRegOpRec(Opcode.sqrtt_c) },
                { 0x04B, new FRegOpRec(Opcode.sqrts_m) },
                { 0x06B, new FRegOpRec(Opcode.sqrtt_m) },
                { 0x08A, new FRegOpRec(Opcode.sqrtf) },
                { 0x08B, new FRegOpRec(Opcode.sqrts) },
                { 0x0AA, new FRegOpRec(Opcode.sqrtg) },
                { 0x0AB, new FRegOpRec(Opcode.sqrtt) },
                { 0x0CB, new FRegOpRec(Opcode.sqrts_d) },
                { 0x0EB, new FRegOpRec(Opcode.sqrtt_d) },
                { 0x10A, new FRegOpRec(Opcode.sqrtf_uc) },
                { 0x10B, new FRegOpRec(Opcode.sqrts_uc) },
                { 0x12A, new FRegOpRec(Opcode.sqrtg_uc) },
                { 0x12B, new FRegOpRec(Opcode.sqrtt_uc) },
                { 0x14B, new FRegOpRec(Opcode.sqrts_um) },
                { 0x16B, new FRegOpRec(Opcode.sqrtt_um) },
                { 0x18A, new FRegOpRec(Opcode.sqrtf_u) },
                { 0x18B, new FRegOpRec(Opcode.sqrts_u) },
                { 0x1AA, new FRegOpRec(Opcode.sqrtg_u) },
                { 0x1AB, new FRegOpRec(Opcode.sqrtt_u) },
                { 0x1CB, new FRegOpRec(Opcode.sqrts_ud) },
                { 0x1EB, new FRegOpRec(Opcode.sqrtt_ud) },
                { 0x40A, new FRegOpRec(Opcode.sqrtf_sc) },
                { 0x42A, new FRegOpRec(Opcode.sqrtg_sc) },
                { 0x48A, new FRegOpRec(Opcode.sqrtf_s) },
                { 0x4AA, new FRegOpRec(Opcode.sqrtg_s) },
                { 0x50A, new FRegOpRec(Opcode.sqrtf_suc) },
                { 0x50B, new FRegOpRec(Opcode.sqrts_suc) },
                { 0x52A, new FRegOpRec(Opcode.sqrtg_suc) },
                { 0x52B, new FRegOpRec(Opcode.sqrtt_suc) },
                { 0x54B, new FRegOpRec(Opcode.sqrts_sum) },
                { 0x56B, new FRegOpRec(Opcode.sqrtt_sum) },
                { 0x58A, new FRegOpRec(Opcode.sqrtf_su) },
                { 0x58B, new FRegOpRec(Opcode.sqrts_su) },
                { 0x5AA, new FRegOpRec(Opcode.sqrtg_su) },
                { 0x5AB, new FRegOpRec(Opcode.sqrtt_su) },
                { 0x5CB, new FRegOpRec(Opcode.sqrts_sud) },
                { 0x5EB, new FRegOpRec(Opcode.sqrtt_sud) },
                { 0x70B, new FRegOpRec(Opcode.sqrts_suic) },
                { 0x72B, new FRegOpRec(Opcode.sqrtt_suic) },
                { 0x74B, new FRegOpRec(Opcode.sqrts_suim) },
                { 0x76B, new FRegOpRec(Opcode.sqrtt_suim) },
                { 0x78B, new FRegOpRec(Opcode.sqrts_sui) },
                { 0x7AB, new FRegOpRec(Opcode.sqrtt_sui) },
                { 0x7CB, new FRegOpRec(Opcode.sqrts_suid) },
                { 0x7EB, new FRegOpRec(Opcode.sqrtt_suid) },

            }),
            new FOperateOpRec(new Dictionary<int, Decoder> // 15
            {
                { 0x000, new FRegOpRec(Opcode.addf_c) },
                { 0x001, new FRegOpRec(Opcode.subf_c) },
                { 0x002, new FRegOpRec(Opcode.mulf_c) },
                { 0x003, new FRegOpRec(Opcode.divf_c) },
                { 0x01E, new CvtOpRec(Opcode.cvtdg_c) },
                { 0x020, new FRegOpRec(Opcode.addg_c) },
                { 0x021, new FRegOpRec(Opcode.subg_c) },
                { 0x022, new FRegOpRec(Opcode.mulg_c) },
                { 0x023, new FRegOpRec(Opcode.divg_c) },
                { 0x02C, new CvtOpRec(Opcode.cvtgf_c) },
                { 0x02D, new CvtOpRec(Opcode.cvtgd_c) },
                { 0x02F, new CvtOpRec(Opcode.cvtgq_c) },
                { 0x03C, new CvtOpRec(Opcode.cvtqf_c) },
                { 0x03E, new CvtOpRec(Opcode.cvtqg_c) },
                { 0x080, new FRegOpRec(Opcode.addf) },
                //{ 0x081, new FRegOpRec(Opcode.negf) },	_* /*pse*/udo */
                { 0x081, new FRegOpRec(Opcode.subf) },
                { 0x082, new FRegOpRec(Opcode.mulf) },
                { 0x083, new FRegOpRec(Opcode.divf) },
                { 0x09E, new CvtOpRec(Opcode.cvtdg) },
                { 0x0A0, new FRegOpRec(Opcode.addg) },
                //{ 0x0A1, new FRegOpRec(Opcode.negg) },	_* pseudo */
                { 0x0A1, new FRegOpRec(Opcode.subg) },
                { 0x0A2, new FRegOpRec(Opcode.mulg) },
                { 0x0A3, new FRegOpRec(Opcode.divg) },
                { 0x0A5, new FRegOpRec(Opcode.cmpgeq) },
                { 0x0A6, new FRegOpRec(Opcode.cmpglt) },
                { 0x0A7, new FRegOpRec(Opcode.cmpgle) },
                { 0x0AC, new CvtOpRec(Opcode.cvtgf) },
                { 0x0AD, new CvtOpRec(Opcode.cvtgd) },
                { 0x0AF, new CvtOpRec(Opcode.cvtgq) },
                { 0x0BC, new CvtOpRec(Opcode.cvtqf) },
                { 0x0BE, new CvtOpRec(Opcode.cvtqg) },
                { 0x100, new FRegOpRec(Opcode.addf_uc) },
                { 0x101, new FRegOpRec(Opcode.subf_uc) },
                { 0x102, new FRegOpRec(Opcode.mulf_uc) },
                { 0x103, new FRegOpRec(Opcode.divf_uc) },
                { 0x11E, new CvtOpRec(Opcode.cvtdg_uc) },
                { 0x120, new FRegOpRec(Opcode.addg_uc) },
                { 0x121, new FRegOpRec(Opcode.subg_uc) },
                { 0x122, new FRegOpRec(Opcode.mulg_uc) },
                { 0x123, new FRegOpRec(Opcode.divg_uc) },
                { 0x12C, new CvtOpRec(Opcode.cvtgf_uc) },
                { 0x12D, new CvtOpRec(Opcode.cvtgd_uc) },
                { 0x12F, new CvtOpRec(Opcode.cvtgq_vc) },
                { 0x180, new FRegOpRec(Opcode.addf_u) },
                { 0x181, new FRegOpRec(Opcode.subf_u) },
                { 0x182, new FRegOpRec(Opcode.mulf_u) },
                { 0x183, new FRegOpRec(Opcode.divf_u) },
                { 0x19E, new CvtOpRec(Opcode.cvtdg_u) },
                { 0x1A0, new FRegOpRec(Opcode.addg_u) },
                { 0x1A1, new FRegOpRec(Opcode.subg_u) },
                { 0x1A2, new FRegOpRec(Opcode.mulg_u) },
                { 0x1A3, new FRegOpRec(Opcode.divg_u) },
                { 0x1AC, new CvtOpRec(Opcode.cvtgf_u) },
                { 0x1AD, new CvtOpRec(Opcode.cvtgd_u) },
                { 0x1AF, new CvtOpRec(Opcode.cvtgq_v) },
                { 0x400, new FRegOpRec(Opcode.addf_sc) },
                { 0x401, new FRegOpRec(Opcode.subf_sc) },
                { 0x402, new FRegOpRec(Opcode.mulf_sc) },
                { 0x403, new FRegOpRec(Opcode.divf_sc) },
                { 0x41E, new CvtOpRec(Opcode.cvtdg_sc) },
                { 0x420, new FRegOpRec(Opcode.addg_sc) },
                { 0x421, new FRegOpRec(Opcode.subg_sc) },
                { 0x422, new FRegOpRec(Opcode.mulg_sc) },
                { 0x423, new FRegOpRec(Opcode.divg_sc) },
                { 0x42C, new CvtOpRec(Opcode.cvtgf_sc) },
                { 0x42D, new CvtOpRec(Opcode.cvtgd_sc) },
                { 0x42F, new CvtOpRec(Opcode.cvtgq_sc) },
                { 0x480, new FRegOpRec(Opcode.addf_s) },
                //{ 0x481, new FRegOpRec(Opcode.negf_s) },	_* pseudo */
                { 0x481, new FRegOpRec(Opcode.subf_s) },
                { 0x482, new FRegOpRec(Opcode.mulf_s) },
                { 0x483, new FRegOpRec(Opcode.divf_s) },
                { 0x49E, new CvtOpRec(Opcode.cvtdg_s) },
                { 0x4A0, new FRegOpRec(Opcode.addg_s) },
                //{ 0x4A1, new FRegOpRec(Opcode.negg_s) },	_* pseudo */
                { 0x4A1, new FRegOpRec(Opcode.subg_s) },
                { 0x4A2, new FRegOpRec(Opcode.mulg_s) },
                { 0x4A3, new FRegOpRec(Opcode.divg_s) },
                { 0x4A5, new FRegOpRec(Opcode.cmpgeq_s) },
                { 0x4A6, new FRegOpRec(Opcode.cmpglt_s) },
                { 0x4A7, new FRegOpRec(Opcode.cmpgle_s) },
                { 0x4AC, new CvtOpRec(Opcode.cvtgf_s) },
                { 0x4AD, new CvtOpRec(Opcode.cvtgd_s) },
                { 0x4AF, new CvtOpRec(Opcode.cvtgq_s) },
                { 0x500, new FRegOpRec(Opcode.addf_suc) },
                { 0x501, new FRegOpRec(Opcode.subf_suc) },
                { 0x502, new FRegOpRec(Opcode.mulf_suc) },
                { 0x503, new FRegOpRec(Opcode.divf_suc) },
                { 0x51E, new CvtOpRec(Opcode.cvtdg_suc) },
                { 0x520, new FRegOpRec(Opcode.addg_suc) },
                { 0x521, new FRegOpRec(Opcode.subg_suc) },
                { 0x522, new FRegOpRec(Opcode.mulg_suc) },
                { 0x523, new FRegOpRec(Opcode.divg_suc) },
                { 0x52C, new CvtOpRec(Opcode.cvtgf_suc) },
                { 0x52D, new CvtOpRec(Opcode.cvtgd_suc) },
                { 0x52F, new CvtOpRec(Opcode.cvtgq_svc) },
                { 0x580, new FRegOpRec(Opcode.addf_su) },
                { 0x581, new FRegOpRec(Opcode.subf_su) },
                { 0x582, new FRegOpRec(Opcode.mulf_su) },
                { 0x583, new FRegOpRec(Opcode.divf_su) },
                { 0x59E, new CvtOpRec(Opcode.cvtdg_su) },
                { 0x5A0, new FRegOpRec(Opcode.addg_su) },
                { 0x5A1, new FRegOpRec(Opcode.subg_su) },
                { 0x5A2, new FRegOpRec(Opcode.mulg_su) },
                { 0x5A3, new FRegOpRec(Opcode.divg_su) },
                { 0x5AC, new CvtOpRec(Opcode.cvtgf_su) },
                { 0x5AD, new CvtOpRec(Opcode.cvtgd_su) },
                { 0x5AF, new CvtOpRec(Opcode.cvtgq_sv) },

            }),
            new FOperateOpRec(new Dictionary<int, Decoder> // 16
            {
                { 0x000, new FRegOpRec(Opcode.adds_c) },
                { 0x001, new FRegOpRec(Opcode.subs_c) },
                { 0x002, new FRegOpRec(Opcode.muls_c) },
                { 0x003, new FRegOpRec(Opcode.divs_c) },
                { 0x020, new FRegOpRec(Opcode.addt_c) },
                { 0x021, new FRegOpRec(Opcode.subt_c) },
                { 0x022, new FRegOpRec(Opcode.mult_c) },
                { 0x023, new FRegOpRec(Opcode.divt_c) },
                { 0x02C, new CvtOpRec(Opcode.cvtts_c) },
                { 0x02F, new CvtOpRec(Opcode.cvttq_c) },
                { 0x03C, new CvtOpRec(Opcode.cvtqs_c) },
                { 0x03E, new CvtOpRec(Opcode.cvtqt_c) },
                { 0x040, new FRegOpRec(Opcode.adds_m) },
                { 0x041, new FRegOpRec(Opcode.subs_m) },
                { 0x042, new FRegOpRec(Opcode.muls_m) },
                { 0x043, new FRegOpRec(Opcode.divs_m) },
                { 0x060, new FRegOpRec(Opcode.addt_m) },
                { 0x061, new FRegOpRec(Opcode.subt_m) },
                { 0x062, new FRegOpRec(Opcode.mult_m) },
                { 0x063, new FRegOpRec(Opcode.divt_m) },
                { 0x06C, new CvtOpRec(Opcode.cvtts_m) },
                { 0x06F, new CvtOpRec(Opcode.cvttq_m) },
                { 0x07C, new CvtOpRec(Opcode.cvtqs_m) },
                { 0x07E, new CvtOpRec(Opcode.cvtqt_m) },
                { 0x080, new FRegOpRec(Opcode.adds) },
                //{ 0x081, new FRegOpRec(Opcode.negs) },	_* pseudo */
                { 0x081, new FRegOpRec(Opcode.subs) },
                { 0x082, new FRegOpRec(Opcode.muls) },
                { 0x083, new FRegOpRec(Opcode.divs) },
                { 0x0A0, new FRegOpRec(Opcode.addt) },
                //{ 0x0A1, new FRegOpRec(Opcode.negt) },	_* pseudo */
                { 0x0A1, new FRegOpRec(Opcode.subt) },
                { 0x0A2, new FRegOpRec(Opcode.mult) },
                { 0x0A3, new FRegOpRec(Opcode.divt) },
                { 0x0A4, new FRegOpRec(Opcode.cmptun) },
                { 0x0A5, new FRegOpRec(Opcode.cmpteq) },
                { 0x0A6, new FRegOpRec(Opcode.cmptlt) },
                { 0x0A7, new FRegOpRec(Opcode.cmptle) },
                { 0x0AC, new CvtOpRec(Opcode.cvtts) },
                { 0x0AF, new CvtOpRec(Opcode.cvttq) },
                { 0x0BC, new CvtOpRec(Opcode.cvtqs) },
                { 0x0BE, new CvtOpRec(Opcode.cvtqt) },
                { 0x0C0, new FRegOpRec(Opcode.adds_d) },
                { 0x0C1, new FRegOpRec(Opcode.subs_d) },
                { 0x0C2, new FRegOpRec(Opcode.muls_d) },
                { 0x0C3, new FRegOpRec(Opcode.divs_d) },
                { 0x0E0, new FRegOpRec(Opcode.addt_d) },
                { 0x0E1, new FRegOpRec(Opcode.subt_d) },
                { 0x0E2, new FRegOpRec(Opcode.mult_d) },
                { 0x0E3, new FRegOpRec(Opcode.divt_d) },
                { 0x0EC, new CvtOpRec(Opcode.cvtts_d) },
                { 0x0EF, new CvtOpRec(Opcode.cvttq_d) },
                { 0x0FC, new CvtOpRec(Opcode.cvtqs_d) },
                { 0x0FE, new CvtOpRec(Opcode.cvtqt_d) },
                { 0x100, new FRegOpRec(Opcode.adds_uc) },
                { 0x101, new FRegOpRec(Opcode.subs_uc) },
                { 0x102, new FRegOpRec(Opcode.muls_uc) },
                { 0x103, new FRegOpRec(Opcode.divs_uc) },
                { 0x120, new FRegOpRec(Opcode.addt_uc) },
                { 0x121, new FRegOpRec(Opcode.subt_uc) },
                { 0x122, new FRegOpRec(Opcode.mult_uc) },
                { 0x123, new FRegOpRec(Opcode.divt_uc) },
                { 0x12C, new CvtOpRec(Opcode.cvtts_uc) },
                { 0x12F, new CvtOpRec(Opcode.cvttq_vc) },
                { 0x140, new FRegOpRec(Opcode.adds_um) },
                { 0x141, new FRegOpRec(Opcode.subs_um) },
                { 0x142, new FRegOpRec(Opcode.muls_um) },
                { 0x143, new FRegOpRec(Opcode.divs_um) },
                { 0x160, new FRegOpRec(Opcode.addt_um) },
                { 0x161, new FRegOpRec(Opcode.subt_um) },
                { 0x162, new FRegOpRec(Opcode.mult_um) },
                { 0x163, new FRegOpRec(Opcode.divt_um) },
                { 0x16C, new CvtOpRec(Opcode.cvtts_um) },
                { 0x16F, new CvtOpRec(Opcode.cvttq_vm) },
                { 0x180, new FRegOpRec(Opcode.adds_u) },
                { 0x181, new FRegOpRec(Opcode.subs_u) },
                { 0x182, new FRegOpRec(Opcode.muls_u) },
                { 0x183, new FRegOpRec(Opcode.divs_u) },
                { 0x1A0, new FRegOpRec(Opcode.addt_u) },
                { 0x1A1, new FRegOpRec(Opcode.subt_u) },
                { 0x1A2, new FRegOpRec(Opcode.mult_u) },
                { 0x1A3, new FRegOpRec(Opcode.divt_u) },
                { 0x1AC, new CvtOpRec(Opcode.cvtts_u) },
                { 0x1AF, new CvtOpRec(Opcode.cvttq_v) },
                { 0x1C0, new FRegOpRec(Opcode.adds_ud) },
                { 0x1C1, new FRegOpRec(Opcode.subs_ud) },
                { 0x1C2, new FRegOpRec(Opcode.muls_ud) },
                { 0x1C3, new FRegOpRec(Opcode.divs_ud) },
                { 0x1E0, new FRegOpRec(Opcode.addt_ud) },
                { 0x1E1, new FRegOpRec(Opcode.subt_ud) },
                { 0x1E2, new FRegOpRec(Opcode.mult_ud) },
                { 0x1E3, new FRegOpRec(Opcode.divt_ud) },
                { 0x1EC, new CvtOpRec(Opcode.cvtts_ud) },
                { 0x1EF, new CvtOpRec(Opcode.cvttq_vd) },
                { 0x2AC, new CvtOpRec(Opcode.cvtst) },
                { 0x500, new FRegOpRec(Opcode.adds_suc) },
                { 0x501, new FRegOpRec(Opcode.subs_suc) },
                { 0x502, new FRegOpRec(Opcode.muls_suc) },
                { 0x503, new FRegOpRec(Opcode.divs_suc) },
                { 0x520, new FRegOpRec(Opcode.addt_suc) },
                { 0x521, new FRegOpRec(Opcode.subt_suc) },
                { 0x522, new FRegOpRec(Opcode.mult_suc) },
                { 0x523, new FRegOpRec(Opcode.divt_suc) },
                { 0x52C, new CvtOpRec(Opcode.cvtts_suc) },
                { 0x52F, new CvtOpRec(Opcode.cvttq_svc) },
                { 0x540, new FRegOpRec(Opcode.adds_sum) },
                { 0x541, new FRegOpRec(Opcode.subs_sum) },
                { 0x542, new FRegOpRec(Opcode.muls_sum) },
                { 0x543, new FRegOpRec(Opcode.divs_sum) },
                { 0x560, new FRegOpRec(Opcode.addt_sum) },
                { 0x561, new FRegOpRec(Opcode.subt_sum) },
                { 0x562, new FRegOpRec(Opcode.mult_sum) },
                { 0x563, new FRegOpRec(Opcode.divt_sum) },
                { 0x56C, new CvtOpRec(Opcode.cvtts_sum) },
                { 0x56F, new CvtOpRec(Opcode.cvttq_svm) },
                { 0x580, new FRegOpRec(Opcode.adds_su) },
                //{ 0x581, new FRegOpRec(Opcode.negs_su) },	_* pseudo */
                { 0x581, new FRegOpRec(Opcode.subs_su) },
                { 0x582, new FRegOpRec(Opcode.muls_su) },
                { 0x583, new FRegOpRec(Opcode.divs_su) },
                { 0x5A0, new FRegOpRec(Opcode.addt_su) },
                //{ 0x5A1, new FRegOpRec(Opcode.negt_su) },	_* pseudo */
                { 0x5A1, new FRegOpRec(Opcode.subt_su) },
                { 0x5A2, new FRegOpRec(Opcode.mult_su) },
                { 0x5A3, new FRegOpRec(Opcode.divt_su) },
                { 0x5A4, new FRegOpRec(Opcode.cmptun_su) },
                { 0x5A5, new FRegOpRec(Opcode.cmpteq_su) },
                { 0x5A6, new FRegOpRec(Opcode.cmptlt_su) },
                { 0x5A7, new FRegOpRec(Opcode.cmptle_su) },
                { 0x5AC, new CvtOpRec(Opcode.cvtts_su) },
                { 0x5AF, new CvtOpRec(Opcode.cvttq_sv) },
                { 0x5C0, new FRegOpRec(Opcode.adds_sud) },
                { 0x5C1, new FRegOpRec(Opcode.subs_sud) },
                { 0x5C2, new FRegOpRec(Opcode.muls_sud) },
                { 0x5C3, new FRegOpRec(Opcode.divs_sud) },
                { 0x5E0, new FRegOpRec(Opcode.addt_sud) },
                { 0x5E1, new FRegOpRec(Opcode.subt_sud) },
                { 0x5E2, new FRegOpRec(Opcode.mult_sud) },
                { 0x5E3, new FRegOpRec(Opcode.divt_sud) },
                { 0x5EC, new CvtOpRec(Opcode.cvtts_sud) },
                { 0x5EF, new CvtOpRec(Opcode.cvttq_svd) },
                { 0x6AC, new CvtOpRec(Opcode.cvtst_s) },
                { 0x700, new FRegOpRec(Opcode.adds_suic) },
                { 0x701, new FRegOpRec(Opcode.subs_suic) },
                { 0x702, new FRegOpRec(Opcode.muls_suic) },
                { 0x703, new FRegOpRec(Opcode.divs_suic) },
                { 0x720, new FRegOpRec(Opcode.addt_suic) },
                { 0x721, new FRegOpRec(Opcode.subt_suic) },
                { 0x722, new FRegOpRec(Opcode.mult_suic) },
                { 0x723, new FRegOpRec(Opcode.divt_suic) },
                { 0x72C, new CvtOpRec(Opcode.cvtts_suic) },
                { 0x72F, new CvtOpRec(Opcode.cvttq_svic) },
                { 0x73C, new CvtOpRec(Opcode.cvtqs_suic) },
                { 0x73E, new CvtOpRec(Opcode.cvtqt_suic) },
                { 0x740, new FRegOpRec(Opcode.adds_suim) },
                { 0x741, new FRegOpRec(Opcode.subs_suim) },
                { 0x742, new FRegOpRec(Opcode.muls_suim) },
                { 0x743, new FRegOpRec(Opcode.divs_suim) },
                { 0x760, new FRegOpRec(Opcode.addt_suim) },
                { 0x761, new FRegOpRec(Opcode.subt_suim) },
                { 0x762, new FRegOpRec(Opcode.mult_suim) },
                { 0x763, new FRegOpRec(Opcode.divt_suim) },
                { 0x76C, new CvtOpRec(Opcode.cvtts_suim) },
                { 0x76F, new CvtOpRec(Opcode.cvttq_svim) },
                { 0x77C, new CvtOpRec(Opcode.cvtqs_suim) },
                { 0x77E, new CvtOpRec(Opcode.cvtqt_suim) },
                { 0x780, new FRegOpRec(Opcode.adds_sui) },
                //{ 0x781, new FRegOpRec(Opcode.negs_sui) },	_* pseudo */
                { 0x781, new FRegOpRec(Opcode.subs_sui) },
                { 0x782, new FRegOpRec(Opcode.muls_sui) },
                { 0x783, new FRegOpRec(Opcode.divs_sui) },
                { 0x7A0, new FRegOpRec(Opcode.addt_sui) },
                //{ 0x7A1, new FRegOpRec(Opcode.negt_sui) },	_* pseudo */
                { 0x7A1, new FRegOpRec(Opcode.subt_sui) },
                { 0x7A2, new FRegOpRec(Opcode.mult_sui) },
                { 0x7A3, new FRegOpRec(Opcode.divt_sui) },
                { 0x7AC, new CvtOpRec(Opcode.cvtts_sui) },
                { 0x7AF, new CvtOpRec(Opcode.cvttq_svi) },
                { 0x7BC, new CvtOpRec(Opcode.cvtqs_sui) },
                { 0x7BE, new CvtOpRec(Opcode.cvtqt_sui) },
                { 0x7C0, new FRegOpRec(Opcode.adds_suid) },
                { 0x7C1, new FRegOpRec(Opcode.subs_suid) },
                { 0x7C2, new FRegOpRec(Opcode.muls_suid) },
                { 0x7C3, new FRegOpRec(Opcode.divs_suid) },
                { 0x7E0, new FRegOpRec(Opcode.addt_suid) },
                { 0x7E1, new FRegOpRec(Opcode.subt_suid) },
                { 0x7E2, new FRegOpRec(Opcode.mult_suid) },
                { 0x7E3, new FRegOpRec(Opcode.divt_suid) },
                { 0x7EC, new CvtOpRec(Opcode.cvtts_suid) },
                { 0x7EF, new CvtOpRec(Opcode.cvttq_svid) },
                { 0x7FC, new CvtOpRec(Opcode.cvtqs_suid) },
                { 0x7FE, new CvtOpRec(Opcode.cvtqt_suid) },
            }),
            new FOperateOpRec(new Dictionary<int, Decoder>
            {
                  { 0x010, new CvtOpRec(Opcode.cvtlq) },
                  //{ 0x020, new FRegOpRec(Opcode.fnop) },	_* pseudo */
                  //{ 0x020, new FRegOpRec(Opcode.fclr) },	_* pseudo */
                  //{ 0x020, new FRegOpRec(Opcode.fabs) },	_* pseudo */
                  //{ 0x020, new FRegOpRec(Opcode.fmov) }, _* pseudo */
                  { 0x020, new FRegOpRec(Opcode.cpys) },
                  //{ 0x021, new FRegOpRec(Opcode.fneg) }, _* pseudo */
                  { 0x021, new FRegOpRec(Opcode.cpysn) },
                  { 0x022, new FRegOpRec(Opcode.cpyse) },
                  { 0x024, new FRegOpRec(Opcode.mt_fpcr) },
                  { 0x025, new FRegOpRec(Opcode.mf_fpcr) },
                  { 0x02A, new FRegOpRec(Opcode.fcmoveq) },
                  { 0x02B, new FRegOpRec(Opcode.fcmovne) },
                  { 0x02C, new FRegOpRec(Opcode.fcmovlt) },
                  { 0x02D, new FRegOpRec(Opcode.fcmovge) },
                  { 0x02E, new FRegOpRec(Opcode.fcmovle) },
                  { 0x02F, new FRegOpRec(Opcode.fcmovgt) },
                  { 0x030, new CvtOpRec(Opcode.cvtql) },
                  { 0x130, new CvtOpRec(Opcode.cvtql_v) },
                  { 0x530, new CvtOpRec(Opcode.cvtql_sv) },
            }),

            new PalOpRec(new Dictionary<uint, (Opcode,InstrClass)> // 18
            {
                { 0x0000, (Opcode.trapb,InstrClass.Transfer|InstrClass.Call) }
            }),
            new PalOpRec(new Dictionary<uint, (Opcode,InstrClass)> // 19 
            {
            }),
            new JMemOpRec(),
            new PalOpRec(new Dictionary<uint, (Opcode,InstrClass)> // 1B 
            {
            }),

            new NyiOpRec(),
            new NyiOpRec(),
            new PalOpRec(new Dictionary<uint, (Opcode,InstrClass)>
            {       //$REVIEW: these palcodes are OS-dependent....
            }),
            new NyiOpRec(),
            // 20
            new FMemOpRec(Opcode.ldf),
            new FMemOpRec(Opcode.ldg),
            new FMemOpRec(Opcode.lds),
            new FMemOpRec(Opcode.ldt),

            new FMemOpRec(Opcode.stf),
            new FMemOpRec(Opcode.stg),
            new FMemOpRec(Opcode.sts),
            new FMemOpRec(Opcode.stt),

            new MemOpRec(Opcode.ldl),
            new MemOpRec(Opcode.ldq),
            new MemOpRec(Opcode.ldl_l),
            new MemOpRec(Opcode.ldq_l),

            new MemOpRec(Opcode.stl),
            new MemOpRec(Opcode.stq),
            new MemOpRec(Opcode.stl_c),
            new MemOpRec(Opcode.stq_c),
            // 30
            new BranchOpRec(Opcode.br),
            new FBranchOpRec(Opcode.fbeq),
            new FBranchOpRec(Opcode.fblt),
            new FBranchOpRec(Opcode.fble),

            new BranchOpRec(Opcode.bsr),
            new FBranchOpRec(Opcode.fbne),
            new FBranchOpRec(Opcode.fbge),
            new FBranchOpRec(Opcode.fbgt),

            new BranchOpRec(Opcode.blbc),
            new BranchOpRec(Opcode.beq),
            new BranchOpRec(Opcode.blt),
            new BranchOpRec(Opcode.ble),

            new BranchOpRec(Opcode.blbs),
            new BranchOpRec(Opcode.bne),
            new BranchOpRec(Opcode.bge),
            new BranchOpRec(Opcode.bgt),
        };
    }
}