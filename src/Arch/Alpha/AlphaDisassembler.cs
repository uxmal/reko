#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
            uint uInstr;
            if (!rdr.TryReadUInt32(out uInstr))
                return null;
            var op = uInstr >> 26;
            var instr = Oprecs[op].Decode(uInstr, this);
            instr.Address = addr;
            instr.Length = 4;
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
            return new AlphaInstruction { Opcode = Opcode.invalid };
        }

        private AlphaInstruction Nyi(uint uInstr)
        {
            return Nyi(uInstr, string.Format("{0:X2}", uInstr >> 26));

        }
        private AlphaInstruction Nyi(uint uInstr, int functionCode)
        {
            return Nyi(uInstr, string.Format("{0:X2}_{1:X2}", uInstr >> 26, functionCode));
        }

        private AlphaInstruction Nyi(uint uInstr, string pattern)
        {
#if DEBUG
            if (!false && seen.Contains(pattern))
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
#endif
            }
            return Invalid();
        }

        private abstract class OpRecBase
        {
            public abstract AlphaInstruction Decode(uint uInstr, AlphaDisassembler dasm);
        }

        private class MemOpRec : OpRecBase
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
                    op1 = dasm.AluRegister(uInstr >> 21),
                    op2 = new MemoryOperand(
                        PrimitiveType.Word32,    // Dummy value
                        dasm.AluRegister(uInstr >> 16).Register,
                        (short)uInstr)
                };
            }
        }

        private class FMemOpRec : OpRecBase
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
                    op1 = dasm.FpuRegister(uInstr >> 21),
                    op2 = new MemoryOperand(
                        PrimitiveType.Word32,    // Dummy value
                        dasm.AluRegister(uInstr >> 16).Register,
                        (short)uInstr)
                };
            }
        }

        private class JMemOpRec : OpRecBase
        {
            private readonly static Opcode[] opcodes = {
                Opcode.jmp, Opcode.jsr, Opcode.ret, Opcode.jsr_coroutine
            };

            public JMemOpRec()
            {
            }

            public override AlphaInstruction Decode(uint uInstr, AlphaDisassembler dasm)
            {
                return new AlphaInstruction
                {
                    Opcode = opcodes[(uInstr >> 14) & 0x3],
                    op1 = dasm.AluRegister(uInstr >> 21),
                    op2 = dasm.AluRegister(uInstr >> 16)
                };
            }
        }


        private class MemFnOpRec : OpRecBase
        {
            public override AlphaInstruction Decode(uint uInstr, AlphaDisassembler dasm)
            {
                throw new NotImplementedException();
            }
        }

        private class BranchOpRec : OpRecBase
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
                    op1 = op1,
                    op2 = op2,
                };
            }
        }

        private class FBranchOpRec : OpRecBase
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
                    op1 = op1,
                    op2 = op2,
                };
            }
        }



        private class RegOpRec : OpRecBase
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
                    op1 = op1,
                    op2 = op2,
                    op3 = op3,
                };
            }
        }

        private class OperateOpRec : OpRecBase
        {
            private Dictionary<int, RegOpRec> decoders;

            public OperateOpRec(Dictionary<int, RegOpRec> decoders)
            {
                this.decoders = decoders;
            }

            public override AlphaInstruction Decode(uint uInstr, AlphaDisassembler dasm)
            {
                RegOpRec decoder;
                var functionCode = ((int)uInstr >> 5) & 0x7F;
                if (!decoders.TryGetValue(functionCode, out decoder))
                    return dasm.Invalid();
                else
                    return decoder.Decode(uInstr, dasm);
            }
        }

        private class FOperateOpRec : OpRecBase
        {
            private Dictionary<int, FRegOpRec> decoders;

            public FOperateOpRec(Dictionary<int, FRegOpRec> decoders)
            {
                this.decoders = decoders;
            }

            public override AlphaInstruction Decode(uint uInstr, AlphaDisassembler dasm)
            {
                FRegOpRec decoder;
                var functionCode = ((int)uInstr >> 5) & 0x7FF;
                if (!decoders.TryGetValue(functionCode, out decoder))
                    return dasm.Nyi(uInstr, functionCode);
                else
                    return decoder.Decode(uInstr, dasm);
            }
        }


        private class FRegOpRec : OpRecBase
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
                    op1 = op1,
                    op2 = op2,
                    op3 = op3,
                };
            }
        }

        private class PalOpRec : OpRecBase
        {
            private Dictionary<uint, Opcode> opcodes;

            public PalOpRec(Dictionary<uint, Opcode> opcodes)
            {
                this.opcodes = opcodes;
            }

            public override AlphaInstruction Decode(uint uInstr, AlphaDisassembler dasm)
            {
                Opcode opcode;
                if (!opcodes.TryGetValue(uInstr & 0x03FFFFFF, out opcode))
                    return dasm.Invalid();
                return new AlphaInstruction { Opcode = opcode };
            }
        }

        private class InvalidOpRec : OpRecBase
        {
            public override AlphaInstruction Decode(uint uInstr, AlphaDisassembler dasm)
            {
                return dasm.Invalid();
            }
        }

        private class NyiOpRec : OpRecBase
        {
            public override AlphaInstruction Decode(uint uInstr, AlphaDisassembler dasm)
            {
                return dasm.Invalid();
            }
        }

        private static OpRecBase[] oprecs = new OpRecBase[64]
        {
            // 00
            new PalOpRec(new Dictionary<uint, Opcode>
            {       //$REVIEW: these palcodes are OS-dependent....
                { 0, Opcode.halt }
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

            new FOperateOpRec(new Dictionary<int, FRegOpRec> // 14
            {
  { 0x004, new FRegOpRec(Opcode.itofs) },   // 14
  { 0x00A, new FRegOpRec(Opcode.sqrtf_c) },   // 14
  { 0x00B, new FRegOpRec(Opcode.sqrts_c) },   // 14
  { 0x014, new FRegOpRec(Opcode.itoff) },   // 14
  { 0x024, new FRegOpRec(Opcode.itoft) },   // 14
  { 0x02A, new FRegOpRec(Opcode.sqrtg_c) },   // 14
  { 0x02B, new FRegOpRec(Opcode.sqrtt_c) },   // 14
  { 0x04B, new FRegOpRec(Opcode.sqrts_m) },   // 14
  { 0x06B, new FRegOpRec(Opcode.sqrtt_m) },   // 14
  { 0x08A, new FRegOpRec(Opcode.sqrtf) },   // 14
  { 0x08B, new FRegOpRec(Opcode.sqrts) },   // 14
  { 0x0AA, new FRegOpRec(Opcode.sqrtg) },   // 14
  { 0x0AB, new FRegOpRec(Opcode.sqrtt) },   // 14
  { 0x0CB, new FRegOpRec(Opcode.sqrts_d) },   // 14
  { 0x0EB, new FRegOpRec(Opcode.sqrtt_d) },   // 14
  { 0x10A, new FRegOpRec(Opcode.sqrtf_uc) },   // 14
  { 0x10B, new FRegOpRec(Opcode.sqrts_uc) },   // 14
  { 0x12A, new FRegOpRec(Opcode.sqrtg_uc) },   // 14
  { 0x12B, new FRegOpRec(Opcode.sqrtt_uc) },   // 14
  { 0x14B, new FRegOpRec(Opcode.sqrts_um) },   // 14
  { 0x16B, new FRegOpRec(Opcode.sqrtt_um) },   // 14
  { 0x18A, new FRegOpRec(Opcode.sqrtf_u) },   // 14
  { 0x18B, new FRegOpRec(Opcode.sqrts_u) },   // 14
  { 0x1AA, new FRegOpRec(Opcode.sqrtg_u) },   // 14
  { 0x1AB, new FRegOpRec(Opcode.sqrtt_u) },   // 14
  { 0x1CB, new FRegOpRec(Opcode.sqrts_ud) },   // 14
  { 0x1EB, new FRegOpRec(Opcode.sqrtt_ud) },   // 14
  { 0x40A, new FRegOpRec(Opcode.sqrtf_sc) },   // 14
  { 0x42A, new FRegOpRec(Opcode.sqrtg_sc) },   // 14
  { 0x48A, new FRegOpRec(Opcode.sqrtf_s) },   // 14
  { 0x4AA, new FRegOpRec(Opcode.sqrtg_s) },   // 14
  { 0x50A, new FRegOpRec(Opcode.sqrtf_suc) },   // 14
  { 0x50B, new FRegOpRec(Opcode.sqrts_suc) },   // 14
  { 0x52A, new FRegOpRec(Opcode.sqrtg_suc) },   // 14
  { 0x52B, new FRegOpRec(Opcode.sqrtt_suc) },   // 14
  { 0x54B, new FRegOpRec(Opcode.sqrts_sum) },   // 14
  { 0x56B, new FRegOpRec(Opcode.sqrtt_sum) },   // 14
  { 0x58A, new FRegOpRec(Opcode.sqrtf_su) },   // 14
  { 0x58B, new FRegOpRec(Opcode.sqrts_su) },   // 14
  { 0x5AA, new FRegOpRec(Opcode.sqrtg_su) },   // 14
  { 0x5AB, new FRegOpRec(Opcode.sqrtt_su) },   // 14
  { 0x5CB, new FRegOpRec(Opcode.sqrts_sud) },   // 14
  { 0x5EB, new FRegOpRec(Opcode.sqrtt_sud) },   // 14
  { 0x70B, new FRegOpRec(Opcode.sqrts_suic) },   // 14
  { 0x72B, new FRegOpRec(Opcode.sqrtt_suic) },   // 14
  { 0x74B, new FRegOpRec(Opcode.sqrts_suim) },   // 14
  { 0x76B, new FRegOpRec(Opcode.sqrtt_suim) },   // 14
  { 0x78B, new FRegOpRec(Opcode.sqrts_sui) },   // 14
  { 0x7AB, new FRegOpRec(Opcode.sqrtt_sui) },   // 14
  { 0x7CB, new FRegOpRec(Opcode.sqrts_suid) },   // 14
  { 0x7EB, new FRegOpRec(Opcode.sqrtt_suid) },   // 14

            }),
            new FOperateOpRec(new Dictionary<int, FRegOpRec> // 15
            {
  { 0x000, new FRegOpRec(Opcode.addf_c) },   // 15
  { 0x001, new FRegOpRec(Opcode.subf_c) },   // 15
  { 0x002, new FRegOpRec(Opcode.mulf_c) },   // 15
  { 0x003, new FRegOpRec(Opcode.divf_c) },   // 15
  { 0x01E, new FRegOpRec(Opcode.cvtdg_c) },   // 15
  { 0x020, new FRegOpRec(Opcode.addg_c) },   // 15
  { 0x021, new FRegOpRec(Opcode.subg_c) },   // 15
  { 0x022, new FRegOpRec(Opcode.mulg_c) },   // 15
  { 0x023, new FRegOpRec(Opcode.divg_c) },   // 15
  { 0x02C, new FRegOpRec(Opcode.cvtgf_c) },   // 15
  { 0x02D, new FRegOpRec(Opcode.cvtgd_c) },   // 15
  { 0x02F, new FRegOpRec(Opcode.cvtgq_c) },   // 15
  { 0x03C, new FRegOpRec(Opcode.cvtqf_c) },   // 15
  { 0x03E, new FRegOpRec(Opcode.cvtqg_c) },   // 15
  { 0x080, new FRegOpRec(Opcode.addf) },   // 15
  //{ 0x081, new FRegOpRec(Opcode.negf) },   // 15	_* /*pse*/udo */
  { 0x081, new FRegOpRec(Opcode.subf) },   // 15
  { 0x082, new FRegOpRec(Opcode.mulf) },   // 15
  { 0x083, new FRegOpRec(Opcode.divf) },   // 15
  { 0x09E, new FRegOpRec(Opcode.cvtdg) },   // 15
  { 0x0A0, new FRegOpRec(Opcode.addg) },   // 15
  //{ 0x0A1, new FRegOpRec(Opcode.negg) },   // 15	_* pseudo */
  { 0x0A1, new FRegOpRec(Opcode.subg) },   // 15
  { 0x0A2, new FRegOpRec(Opcode.mulg) },   // 15
  { 0x0A3, new FRegOpRec(Opcode.divg) },   // 15
  { 0x0A5, new FRegOpRec(Opcode.cmpgeq) },   // 15
  { 0x0A6, new FRegOpRec(Opcode.cmpglt) },   // 15
  { 0x0A7, new FRegOpRec(Opcode.cmpgle) },   // 15
  { 0x0AC, new FRegOpRec(Opcode.cvtgf) },   // 15
  { 0x0AD, new FRegOpRec(Opcode.cvtgd) },   // 15
  { 0x0AF, new FRegOpRec(Opcode.cvtgq) },   // 15
  { 0x0BC, new FRegOpRec(Opcode.cvtqf) },   // 15
  { 0x0BE, new FRegOpRec(Opcode.cvtqg) },   // 15
  { 0x100, new FRegOpRec(Opcode.addf_uc) },   // 15
  { 0x101, new FRegOpRec(Opcode.subf_uc) },   // 15
  { 0x102, new FRegOpRec(Opcode.mulf_uc) },   // 15
  { 0x103, new FRegOpRec(Opcode.divf_uc) },   // 15
  { 0x11E, new FRegOpRec(Opcode.cvtdg_uc) },   // 15
  { 0x120, new FRegOpRec(Opcode.addg_uc) },   // 15
  { 0x121, new FRegOpRec(Opcode.subg_uc) },   // 15
  { 0x122, new FRegOpRec(Opcode.mulg_uc) },   // 15
  { 0x123, new FRegOpRec(Opcode.divg_uc) },   // 15
  { 0x12C, new FRegOpRec(Opcode.cvtgf_uc) },   // 15
  { 0x12D, new FRegOpRec(Opcode.cvtgd_uc) },   // 15
  { 0x12F, new FRegOpRec(Opcode.cvtgq_vc) },   // 15
  { 0x180, new FRegOpRec(Opcode.addf_u) },   // 15
  { 0x181, new FRegOpRec(Opcode.subf_u) },   // 15
  { 0x182, new FRegOpRec(Opcode.mulf_u) },   // 15
  { 0x183, new FRegOpRec(Opcode.divf_u) },   // 15
  { 0x19E, new FRegOpRec(Opcode.cvtdg_u) },   // 15
  { 0x1A0, new FRegOpRec(Opcode.addg_u) },   // 15
  { 0x1A1, new FRegOpRec(Opcode.subg_u) },   // 15
  { 0x1A2, new FRegOpRec(Opcode.mulg_u) },   // 15
  { 0x1A3, new FRegOpRec(Opcode.divg_u) },   // 15
  { 0x1AC, new FRegOpRec(Opcode.cvtgf_u) },   // 15
  { 0x1AD, new FRegOpRec(Opcode.cvtgd_u) },   // 15
  { 0x1AF, new FRegOpRec(Opcode.cvtgq_v) },   // 15
  { 0x400, new FRegOpRec(Opcode.addf_sc) },   // 15
  { 0x401, new FRegOpRec(Opcode.subf_sc) },   // 15
  { 0x402, new FRegOpRec(Opcode.mulf_sc) },   // 15
  { 0x403, new FRegOpRec(Opcode.divf_sc) },   // 15
  { 0x41E, new FRegOpRec(Opcode.cvtdg_sc) },   // 15
  { 0x420, new FRegOpRec(Opcode.addg_sc) },   // 15
  { 0x421, new FRegOpRec(Opcode.subg_sc) },   // 15
  { 0x422, new FRegOpRec(Opcode.mulg_sc) },   // 15
  { 0x423, new FRegOpRec(Opcode.divg_sc) },   // 15
  { 0x42C, new FRegOpRec(Opcode.cvtgf_sc) },   // 15
  { 0x42D, new FRegOpRec(Opcode.cvtgd_sc) },   // 15
  { 0x42F, new FRegOpRec(Opcode.cvtgq_sc) },   // 15
  { 0x480, new FRegOpRec(Opcode.addf_s) },   // 15
  //{ 0x481, new FRegOpRec(Opcode.negf_s) },   // 15	_* pseudo */
  { 0x481, new FRegOpRec(Opcode.subf_s) },   // 15
  { 0x482, new FRegOpRec(Opcode.mulf_s) },   // 15
  { 0x483, new FRegOpRec(Opcode.divf_s) },   // 15
  { 0x49E, new FRegOpRec(Opcode.cvtdg_s) },   // 15
  { 0x4A0, new FRegOpRec(Opcode.addg_s) },   // 15
  //{ 0x4A1, new FRegOpRec(Opcode.negg_s) },   // 15	_* pseudo */
  { 0x4A1, new FRegOpRec(Opcode.subg_s) },   // 15
  { 0x4A2, new FRegOpRec(Opcode.mulg_s) },   // 15
  { 0x4A3, new FRegOpRec(Opcode.divg_s) },   // 15
  { 0x4A5, new FRegOpRec(Opcode.cmpgeq_s) },   // 15
  { 0x4A6, new FRegOpRec(Opcode.cmpglt_s) },   // 15
  { 0x4A7, new FRegOpRec(Opcode.cmpgle_s) },   // 15
  { 0x4AC, new FRegOpRec(Opcode.cvtgf_s) },   // 15
  { 0x4AD, new FRegOpRec(Opcode.cvtgd_s) },   // 15
  { 0x4AF, new FRegOpRec(Opcode.cvtgq_s) },   // 15
  { 0x500, new FRegOpRec(Opcode.addf_suc) },   // 15
  { 0x501, new FRegOpRec(Opcode.subf_suc) },   // 15
  { 0x502, new FRegOpRec(Opcode.mulf_suc) },   // 15
  { 0x503, new FRegOpRec(Opcode.divf_suc) },   // 15
  { 0x51E, new FRegOpRec(Opcode.cvtdg_suc) },   // 15
  { 0x520, new FRegOpRec(Opcode.addg_suc) },   // 15
  { 0x521, new FRegOpRec(Opcode.subg_suc) },   // 15
  { 0x522, new FRegOpRec(Opcode.mulg_suc) },   // 15
  { 0x523, new FRegOpRec(Opcode.divg_suc) },   // 15
  { 0x52C, new FRegOpRec(Opcode.cvtgf_suc) },   // 15
  { 0x52D, new FRegOpRec(Opcode.cvtgd_suc) },   // 15
  { 0x52F, new FRegOpRec(Opcode.cvtgq_svc) },   // 15
  { 0x580, new FRegOpRec(Opcode.addf_su) },   // 15
  { 0x581, new FRegOpRec(Opcode.subf_su) },   // 15
  { 0x582, new FRegOpRec(Opcode.mulf_su) },   // 15
  { 0x583, new FRegOpRec(Opcode.divf_su) },   // 15
  { 0x59E, new FRegOpRec(Opcode.cvtdg_su) },   // 15
  { 0x5A0, new FRegOpRec(Opcode.addg_su) },   // 15
  { 0x5A1, new FRegOpRec(Opcode.subg_su) },   // 15
  { 0x5A2, new FRegOpRec(Opcode.mulg_su) },   // 15
  { 0x5A3, new FRegOpRec(Opcode.divg_su) },   // 15
  { 0x5AC, new FRegOpRec(Opcode.cvtgf_su) },   // 15
  { 0x5AD, new FRegOpRec(Opcode.cvtgd_su) },   // 15
  { 0x5AF, new FRegOpRec(Opcode.cvtgq_sv) },   // 15

            }),
            new FOperateOpRec(new Dictionary<int, FRegOpRec> // 16
            {
  { 0x000, new FRegOpRec(Opcode.adds_c) },   // 16
  { 0x001, new FRegOpRec(Opcode.subs_c) },   // 16
  { 0x002, new FRegOpRec(Opcode.muls_c) },   // 16
  { 0x003, new FRegOpRec(Opcode.divs_c) },   // 16
  { 0x020, new FRegOpRec(Opcode.addt_c) },   // 16
  { 0x021, new FRegOpRec(Opcode.subt_c) },   // 16
  { 0x022, new FRegOpRec(Opcode.mult_c) },   // 16
  { 0x023, new FRegOpRec(Opcode.divt_c) },   // 16
  { 0x02C, new FRegOpRec(Opcode.cvtts_c) },   // 16
  { 0x02F, new FRegOpRec(Opcode.cvttq_c) },   // 16
  { 0x03C, new FRegOpRec(Opcode.cvtqs_c) },   // 16
  { 0x03E, new FRegOpRec(Opcode.cvtqt_c) },   // 16
  { 0x040, new FRegOpRec(Opcode.adds_m) },   // 16
  { 0x041, new FRegOpRec(Opcode.subs_m) },   // 16
  { 0x042, new FRegOpRec(Opcode.muls_m) },   // 16
  { 0x043, new FRegOpRec(Opcode.divs_m) },   // 16
  { 0x060, new FRegOpRec(Opcode.addt_m) },   // 16
  { 0x061, new FRegOpRec(Opcode.subt_m) },   // 16
  { 0x062, new FRegOpRec(Opcode.mult_m) },   // 16
  { 0x063, new FRegOpRec(Opcode.divt_m) },   // 16
  { 0x06C, new FRegOpRec(Opcode.cvtts_m) },   // 16
  { 0x06F, new FRegOpRec(Opcode.cvttq_m) },   // 16
  { 0x07C, new FRegOpRec(Opcode.cvtqs_m) },   // 16
  { 0x07E, new FRegOpRec(Opcode.cvtqt_m) },   // 16
  { 0x080, new FRegOpRec(Opcode.adds) },   // 16
  //{ 0x081, new FRegOpRec(Opcode.negs) },   // 16	_* pseudo */
  { 0x081, new FRegOpRec(Opcode.subs) },   // 16
  { 0x082, new FRegOpRec(Opcode.muls) },   // 16
  { 0x083, new FRegOpRec(Opcode.divs) },   // 16
  { 0x0A0, new FRegOpRec(Opcode.addt) },   // 16
  //{ 0x0A1, new FRegOpRec(Opcode.negt) },   // 16	_* pseudo */
  { 0x0A1, new FRegOpRec(Opcode.subt) },   // 16
  { 0x0A2, new FRegOpRec(Opcode.mult) },   // 16
  { 0x0A3, new FRegOpRec(Opcode.divt) },   // 16
  { 0x0A4, new FRegOpRec(Opcode.cmptun) },   // 16
  { 0x0A5, new FRegOpRec(Opcode.cmpteq) },   // 16
  { 0x0A6, new FRegOpRec(Opcode.cmptlt) },   // 16
  { 0x0A7, new FRegOpRec(Opcode.cmptle) },   // 16
  { 0x0AC, new FRegOpRec(Opcode.cvtts) },   // 16
  { 0x0AF, new FRegOpRec(Opcode.cvttq) },   // 16
  { 0x0BC, new FRegOpRec(Opcode.cvtqs) },   // 16
  { 0x0BE, new FRegOpRec(Opcode.cvtqt) },   // 16
  { 0x0C0, new FRegOpRec(Opcode.adds_d) },   // 16
  { 0x0C1, new FRegOpRec(Opcode.subs_d) },   // 16
  { 0x0C2, new FRegOpRec(Opcode.muls_d) },   // 16
  { 0x0C3, new FRegOpRec(Opcode.divs_d) },   // 16
  { 0x0E0, new FRegOpRec(Opcode.addt_d) },   // 16
  { 0x0E1, new FRegOpRec(Opcode.subt_d) },   // 16
  { 0x0E2, new FRegOpRec(Opcode.mult_d) },   // 16
  { 0x0E3, new FRegOpRec(Opcode.divt_d) },   // 16
  { 0x0EC, new FRegOpRec(Opcode.cvtts_d) },   // 16
  { 0x0EF, new FRegOpRec(Opcode.cvttq_d) },   // 16
  { 0x0FC, new FRegOpRec(Opcode.cvtqs_d) },   // 16
  { 0x0FE, new FRegOpRec(Opcode.cvtqt_d) },   // 16
  { 0x100, new FRegOpRec(Opcode.adds_uc) },   // 16
  { 0x101, new FRegOpRec(Opcode.subs_uc) },   // 16
  { 0x102, new FRegOpRec(Opcode.muls_uc) },   // 16
  { 0x103, new FRegOpRec(Opcode.divs_uc) },   // 16
  { 0x120, new FRegOpRec(Opcode.addt_uc) },   // 16
  { 0x121, new FRegOpRec(Opcode.subt_uc) },   // 16
  { 0x122, new FRegOpRec(Opcode.mult_uc) },   // 16
  { 0x123, new FRegOpRec(Opcode.divt_uc) },   // 16
  { 0x12C, new FRegOpRec(Opcode.cvtts_uc) },   // 16
  { 0x12F, new FRegOpRec(Opcode.cvttq_vc) },   // 16
  { 0x140, new FRegOpRec(Opcode.adds_um) },   // 16
  { 0x141, new FRegOpRec(Opcode.subs_um) },   // 16
  { 0x142, new FRegOpRec(Opcode.muls_um) },   // 16
  { 0x143, new FRegOpRec(Opcode.divs_um) },   // 16
  { 0x160, new FRegOpRec(Opcode.addt_um) },   // 16
  { 0x161, new FRegOpRec(Opcode.subt_um) },   // 16
  { 0x162, new FRegOpRec(Opcode.mult_um) },   // 16
  { 0x163, new FRegOpRec(Opcode.divt_um) },   // 16
  { 0x16C, new FRegOpRec(Opcode.cvtts_um) },   // 16
  { 0x16F, new FRegOpRec(Opcode.cvttq_vm) },   // 16
  { 0x180, new FRegOpRec(Opcode.adds_u) },   // 16
  { 0x181, new FRegOpRec(Opcode.subs_u) },   // 16
  { 0x182, new FRegOpRec(Opcode.muls_u) },   // 16
  { 0x183, new FRegOpRec(Opcode.divs_u) },   // 16
  { 0x1A0, new FRegOpRec(Opcode.addt_u) },   // 16
  { 0x1A1, new FRegOpRec(Opcode.subt_u) },   // 16
  { 0x1A2, new FRegOpRec(Opcode.mult_u) },   // 16
  { 0x1A3, new FRegOpRec(Opcode.divt_u) },   // 16
  { 0x1AC, new FRegOpRec(Opcode.cvtts_u) },   // 16
  { 0x1AF, new FRegOpRec(Opcode.cvttq_v) },   // 16
  { 0x1C0, new FRegOpRec(Opcode.adds_ud) },   // 16
  { 0x1C1, new FRegOpRec(Opcode.subs_ud) },   // 16
  { 0x1C2, new FRegOpRec(Opcode.muls_ud) },   // 16
  { 0x1C3, new FRegOpRec(Opcode.divs_ud) },   // 16
  { 0x1E0, new FRegOpRec(Opcode.addt_ud) },   // 16
  { 0x1E1, new FRegOpRec(Opcode.subt_ud) },   // 16
  { 0x1E2, new FRegOpRec(Opcode.mult_ud) },   // 16
  { 0x1E3, new FRegOpRec(Opcode.divt_ud) },   // 16
  { 0x1EC, new FRegOpRec(Opcode.cvtts_ud) },   // 16
  { 0x1EF, new FRegOpRec(Opcode.cvttq_vd) },   // 16
  { 0x2AC, new FRegOpRec(Opcode.cvtst) },   // 16
  { 0x500, new FRegOpRec(Opcode.adds_suc) },   // 16
  { 0x501, new FRegOpRec(Opcode.subs_suc) },   // 16
  { 0x502, new FRegOpRec(Opcode.muls_suc) },   // 16
  { 0x503, new FRegOpRec(Opcode.divs_suc) },   // 16
  { 0x520, new FRegOpRec(Opcode.addt_suc) },   // 16
  { 0x521, new FRegOpRec(Opcode.subt_suc) },   // 16
  { 0x522, new FRegOpRec(Opcode.mult_suc) },   // 16
  { 0x523, new FRegOpRec(Opcode.divt_suc) },   // 16
  { 0x52C, new FRegOpRec(Opcode.cvtts_suc) },   // 16
  { 0x52F, new FRegOpRec(Opcode.cvttq_svc) },   // 16
  { 0x540, new FRegOpRec(Opcode.adds_sum) },   // 16
  { 0x541, new FRegOpRec(Opcode.subs_sum) },   // 16
  { 0x542, new FRegOpRec(Opcode.muls_sum) },   // 16
  { 0x543, new FRegOpRec(Opcode.divs_sum) },   // 16
  { 0x560, new FRegOpRec(Opcode.addt_sum) },   // 16
  { 0x561, new FRegOpRec(Opcode.subt_sum) },   // 16
  { 0x562, new FRegOpRec(Opcode.mult_sum) },   // 16
  { 0x563, new FRegOpRec(Opcode.divt_sum) },   // 16
  { 0x56C, new FRegOpRec(Opcode.cvtts_sum) },   // 16
  { 0x56F, new FRegOpRec(Opcode.cvttq_svm) },   // 16
  { 0x580, new FRegOpRec(Opcode.adds_su) },   // 16
  //{ 0x581, new FRegOpRec(Opcode.negs_su) },   // 16	_* pseudo */
  { 0x581, new FRegOpRec(Opcode.subs_su) },   // 16
  { 0x582, new FRegOpRec(Opcode.muls_su) },   // 16
  { 0x583, new FRegOpRec(Opcode.divs_su) },   // 16
  { 0x5A0, new FRegOpRec(Opcode.addt_su) },   // 16
  //{ 0x5A1, new FRegOpRec(Opcode.negt_su) },   // 16	_* pseudo */
  { 0x5A1, new FRegOpRec(Opcode.subt_su) },   // 16
  { 0x5A2, new FRegOpRec(Opcode.mult_su) },   // 16
  { 0x5A3, new FRegOpRec(Opcode.divt_su) },   // 16
  { 0x5A4, new FRegOpRec(Opcode.cmptun_su) },   // 16
  { 0x5A5, new FRegOpRec(Opcode.cmpteq_su) },   // 16
  { 0x5A6, new FRegOpRec(Opcode.cmptlt_su) },   // 16
  { 0x5A7, new FRegOpRec(Opcode.cmptle_su) },   // 16
  { 0x5AC, new FRegOpRec(Opcode.cvtts_su) },   // 16
  { 0x5AF, new FRegOpRec(Opcode.cvttq_sv) },   // 16
  { 0x5C0, new FRegOpRec(Opcode.adds_sud) },   // 16
  { 0x5C1, new FRegOpRec(Opcode.subs_sud) },   // 16
  { 0x5C2, new FRegOpRec(Opcode.muls_sud) },   // 16
  { 0x5C3, new FRegOpRec(Opcode.divs_sud) },   // 16
  { 0x5E0, new FRegOpRec(Opcode.addt_sud) },   // 16
  { 0x5E1, new FRegOpRec(Opcode.subt_sud) },   // 16
  { 0x5E2, new FRegOpRec(Opcode.mult_sud) },   // 16
  { 0x5E3, new FRegOpRec(Opcode.divt_sud) },   // 16
  { 0x5EC, new FRegOpRec(Opcode.cvtts_sud) },   // 16
  { 0x5EF, new FRegOpRec(Opcode.cvttq_svd) },   // 16
  { 0x6AC, new FRegOpRec(Opcode.cvtst_s) },   // 16
  { 0x700, new FRegOpRec(Opcode.adds_suic) },   // 16
  { 0x701, new FRegOpRec(Opcode.subs_suic) },   // 16
  { 0x702, new FRegOpRec(Opcode.muls_suic) },   // 16
  { 0x703, new FRegOpRec(Opcode.divs_suic) },   // 16
  { 0x720, new FRegOpRec(Opcode.addt_suic) },   // 16
  { 0x721, new FRegOpRec(Opcode.subt_suic) },   // 16
  { 0x722, new FRegOpRec(Opcode.mult_suic) },   // 16
  { 0x723, new FRegOpRec(Opcode.divt_suic) },   // 16
  { 0x72C, new FRegOpRec(Opcode.cvtts_suic) },   // 16
  { 0x72F, new FRegOpRec(Opcode.cvttq_svic) },   // 16
  { 0x73C, new FRegOpRec(Opcode.cvtqs_suic) },   // 16
  { 0x73E, new FRegOpRec(Opcode.cvtqt_suic) },   // 16
  { 0x740, new FRegOpRec(Opcode.adds_suim) },   // 16
  { 0x741, new FRegOpRec(Opcode.subs_suim) },   // 16
  { 0x742, new FRegOpRec(Opcode.muls_suim) },   // 16
  { 0x743, new FRegOpRec(Opcode.divs_suim) },   // 16
  { 0x760, new FRegOpRec(Opcode.addt_suim) },   // 16
  { 0x761, new FRegOpRec(Opcode.subt_suim) },   // 16
  { 0x762, new FRegOpRec(Opcode.mult_suim) },   // 16
  { 0x763, new FRegOpRec(Opcode.divt_suim) },   // 16
  { 0x76C, new FRegOpRec(Opcode.cvtts_suim) },   // 16
  { 0x76F, new FRegOpRec(Opcode.cvttq_svim) },   // 16
  { 0x77C, new FRegOpRec(Opcode.cvtqs_suim) },   // 16
  { 0x77E, new FRegOpRec(Opcode.cvtqt_suim) },   // 16
  { 0x780, new FRegOpRec(Opcode.adds_sui) },   // 16
  //{ 0x781, new FRegOpRec(Opcode.negs_sui) },   // 16	_* pseudo */
  { 0x781, new FRegOpRec(Opcode.subs_sui) },   // 16
  { 0x782, new FRegOpRec(Opcode.muls_sui) },   // 16
  { 0x783, new FRegOpRec(Opcode.divs_sui) },   // 16
  { 0x7A0, new FRegOpRec(Opcode.addt_sui) },   // 16
  //{ 0x7A1, new FRegOpRec(Opcode.negt_sui) },   // 16	_* pseudo */
  { 0x7A1, new FRegOpRec(Opcode.subt_sui) },   // 16
  { 0x7A2, new FRegOpRec(Opcode.mult_sui) },   // 16
  { 0x7A3, new FRegOpRec(Opcode.divt_sui) },   // 16
  { 0x7AC, new FRegOpRec(Opcode.cvtts_sui) },   // 16
  { 0x7AF, new FRegOpRec(Opcode.cvttq_svi) },   // 16
  { 0x7BC, new FRegOpRec(Opcode.cvtqs_sui) },   // 16
  { 0x7BE, new FRegOpRec(Opcode.cvtqt_sui) },   // 16
  { 0x7C0, new FRegOpRec(Opcode.adds_suid) },   // 16
  { 0x7C1, new FRegOpRec(Opcode.subs_suid) },   // 16
  { 0x7C2, new FRegOpRec(Opcode.muls_suid) },   // 16
  { 0x7C3, new FRegOpRec(Opcode.divs_suid) },   // 16
  { 0x7E0, new FRegOpRec(Opcode.addt_suid) },   // 16
  { 0x7E1, new FRegOpRec(Opcode.subt_suid) },   // 16
  { 0x7E2, new FRegOpRec(Opcode.mult_suid) },   // 16
  { 0x7E3, new FRegOpRec(Opcode.divt_suid) },   // 16
  { 0x7EC, new FRegOpRec(Opcode.cvtts_suid) },   // 16
  { 0x7EF, new FRegOpRec(Opcode.cvttq_svid) },   // 16
  { 0x7FC, new FRegOpRec(Opcode.cvtqs_suid) },   // 16
  { 0x7FE, new FRegOpRec(Opcode.cvtqt_suid) },   // 16

            }),
            new FOperateOpRec(new Dictionary<int, FRegOpRec> // 17
            {
                  { 0x010, new FRegOpRec(Opcode.cvtlq) },   // 17
                  //{ 0x020, new FRegOpRec(Opcode.fnop) },   // 17	_* pseudo */
                  //{ 0x020, new FRegOpRec(Opcode.fclr) },   // 17	_* pseudo */
                  //{ 0x020, new FRegOpRec(Opcode.fabs) },   // 17	_* pseudo */
                  //{ 0x020, new FRegOpRec(Opcode.fmov) },   // 17 _* pseudo */
                  { 0x020, new FRegOpRec(Opcode.cpys) },   // 17
                  //{ 0x021, new FRegOpRec(Opcode.fneg) },   // 17 _* pseudo */
                  { 0x021, new FRegOpRec(Opcode.cpysn) },   // 17
                  { 0x022, new FRegOpRec(Opcode.cpyse) },   // 17
                  { 0x024, new FRegOpRec(Opcode.mt_fpcr) },   // 17
                  { 0x025, new FRegOpRec(Opcode.mf_fpcr) },   // 17
                  { 0x02A, new FRegOpRec(Opcode.fcmoveq) },   // 17
                  { 0x02B, new FRegOpRec(Opcode.fcmovne) },   // 17
                  { 0x02C, new FRegOpRec(Opcode.fcmovlt) },   // 17
                  { 0x02D, new FRegOpRec(Opcode.fcmovge) },   // 17
                  { 0x02E, new FRegOpRec(Opcode.fcmovle) },   // 17
                  { 0x02F, new FRegOpRec(Opcode.fcmovgt) },   // 17
                  { 0x030, new FRegOpRec(Opcode.cvtql) },   // 17
                  { 0x130, new FRegOpRec(Opcode.cvtql_v) },   // 17
                  { 0x530, new FRegOpRec(Opcode.cvtql_sv) },   // 17
            }),

            new PalOpRec(new Dictionary<uint, Opcode> // 18
            {
            }),
            new PalOpRec(new Dictionary<uint, Opcode> // 19 
            {
            }),
            new JMemOpRec(),
            new PalOpRec(new Dictionary<uint, Opcode> // 1B 
            {
            }),

            new NyiOpRec(),
            new NyiOpRec(),
            new PalOpRec(new Dictionary<uint, Opcode>
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

        private static OpRecBase[] Oprecs
        {
            get
            {
                return oprecs;
            }

            set
            {
                oprecs = value;
            }
        }
    }
}