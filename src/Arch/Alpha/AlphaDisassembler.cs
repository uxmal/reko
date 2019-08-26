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
        private readonly AlphaArchitecture arch;
        private readonly EndianImageReader rdr;
        private Address addr;

        public AlphaDisassembler(AlphaArchitecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
        }

        public override AlphaInstruction DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadUInt32(out uint uInstr))
                return null;
            var op = uInstr >> 26;
            var instr = decoders[op].Decode(uInstr, this);
            instr.Address = addr;
            instr.Length = 4;
            instr.InstructionClass |= uInstr == 0 ? InstrClass.Zero : 0;
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
            return new AlphaInstruction { Opcode = Opcode.invalid, InstructionClass = InstrClass.Invalid };
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
        private AlphaInstruction Nyi(uint uInstr, string message)
        {
            var instrHex = $"{0:X8}";
            base.EmitUnitTest("Alpha", instrHex, message, "AlphaDis", this.addr, w =>
            {
                w.WriteLine("    var instr = DisassembleWord(0x{0:X8});", uInstr);
                w.WriteLine("    AssertCode(\"AlphaDis_{1}\", 0x{0:X8}, \"@@@\", instr.ToString());", uInstr, instrHex);
            });
            return Invalid();
        }

        private abstract class Decoder
        {
            public abstract AlphaInstruction Decode(uint uInstr, AlphaDisassembler dasm);
        }

        private class MemDecoder : Decoder
        {
            private readonly Opcode opcode;

            public MemDecoder(Opcode opcode)
            {
                this.opcode = opcode;
            }

            public override AlphaInstruction Decode(uint uInstr, AlphaDisassembler dasm)
            {
                return new AlphaInstruction
                {
                    Opcode = this.opcode,
                    InstructionClass = InstrClass.Linear,
                    op1 = dasm.AluRegister(uInstr >> 21),
                    op2 = new MemoryOperand(
                        PrimitiveType.Word32,    // Dummy value
                        dasm.AluRegister(uInstr >> 16).Register,
                        (short)uInstr)
                };
            }
        }

        private class FMemDecoder : Decoder
        {
            private readonly Opcode opcode;

            public FMemDecoder(Opcode opcode)
            {
                this.opcode = opcode;
            }

            public override AlphaInstruction Decode(uint uInstr, AlphaDisassembler dasm)
            {
                return new AlphaInstruction
                {
                    Opcode = this.opcode,
                    InstructionClass = InstrClass.Linear,
                    op1 = dasm.FpuRegister(uInstr >> 21),
                    op2 = new MemoryOperand(
                        PrimitiveType.Word32,    // Dummy value
                        dasm.AluRegister(uInstr >> 16).Register,
                        (short)uInstr)
                };
            }
        }

        private class JMemDecoder : Decoder
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

            public JMemDecoder()
            {
            }

            public override AlphaInstruction Decode(uint uInstr, AlphaDisassembler dasm)
            {
                return new AlphaInstruction
                {
                    Opcode = opcodes[(uInstr >> 14) & 0x3],
                    InstructionClass = iclasses[(uInstr>> 14) & 0x3],
                    op1 = dasm.AluRegister(uInstr >> 21),
                    op2 = dasm.AluRegister(uInstr >> 16)
                };
            }
        }


        private class MemFnDecoder : Decoder
        {
            public override AlphaInstruction Decode(uint uInstr, AlphaDisassembler dasm)
            {
                throw new NotImplementedException();
            }
        }

        private class BranchDecoder : Decoder
        {
            private readonly Opcode opcode;

            public BranchDecoder(Opcode opcode)
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
                    InstructionClass = InstrClass.ConditionalTransfer,
                    op1 = op1,
                    op2 = op2,
                };
            }
        }

        private class FBranchDecoder : Decoder
        {
            private readonly Opcode opcode;

            public FBranchDecoder(Opcode opcode)
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
                    InstructionClass = InstrClass.ConditionalTransfer,
                    op1 = op1,
                    op2 = op2,
                };
            }
        }

        private class RegDecoder : Decoder
        {
            private readonly Opcode opcode;

            public RegDecoder(Opcode opcode)
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
                    InstructionClass = InstrClass.Linear,
                    op1 = op1,
                    op2 = op2,
                    op3 = op3,
                };
            }
        }

        private class OperateDecoder : Decoder
        {
            private readonly Dictionary<int, RegDecoder> decoders;

            public OperateDecoder(Dictionary<int, RegDecoder> decoders)
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

        private class FOperateDecoder : Decoder
        {
            private readonly Dictionary<int, Decoder> decoders;

            public FOperateDecoder(Dictionary<int, Decoder> decoders)
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

        private class FRegDecoder : Decoder
        {
            private readonly Opcode opcode;

            public FRegDecoder(Opcode opcode)
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
                    InstructionClass = InstrClass.Linear,
                    op1 = op1,
                    op2 = op2,
                    op3 = op3,
                };
            }
        }

        private class PalDecoder : Decoder
        {
            private readonly Dictionary<uint, (Opcode,InstrClass)> opcodes;

            public PalDecoder(Dictionary<uint, (Opcode,InstrClass)> opcodes)
            {
                this.opcodes = opcodes;
            }

            public override AlphaInstruction Decode(uint uInstr, AlphaDisassembler dasm)
            {
                if (!opcodes.TryGetValue(uInstr & 0x0000FFFF, out var opcode))
                    return dasm.Invalid();
                return new AlphaInstruction {
                    Opcode = opcode.Item1,
                    InstructionClass = opcode.Item2
                };
            }
        }

        private class CvtDecoder : Decoder
        {
            private readonly Opcode opcode;
            public CvtDecoder(Opcode opcode)
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
                    InstructionClass = InstrClass.Linear,
                    op1 = op1,
                    op2 = op2,
                };
            }
        }

        private class InvalidDecoder : Decoder
        {
            public override AlphaInstruction Decode(uint uInstr, AlphaDisassembler dasm)
            {
                return dasm.Invalid();
            }
        }

        private class NyiDecoder : Decoder
        {
            public override AlphaInstruction Decode(uint uInstr, AlphaDisassembler dasm)
            {
                return dasm.Invalid();
            }
        }

        private static readonly Decoder[] decoders = new Decoder[64]
        {
            // 00
            new PalDecoder(new Dictionary<uint, (Opcode, InstrClass)>
            {       //$REVIEW: these palcodes are OS-dependent....
                { 0, (Opcode.halt,InstrClass.System|InstrClass.Terminates) }
            }),
            new InvalidDecoder(),
            new InvalidDecoder(),
            new InvalidDecoder(),
            new InvalidDecoder(),
            new InvalidDecoder(),
            new InvalidDecoder(),
            new InvalidDecoder(),

            new MemDecoder(Opcode.lda),
            new MemDecoder(Opcode.ldah),
            new MemDecoder(Opcode.ldbu),
            new MemDecoder(Opcode.ldq_u),

            new MemDecoder(Opcode.ldwu),
            new MemDecoder(Opcode.stw),
            new MemDecoder(Opcode.stb),
            new MemDecoder(Opcode.stq_u),
            // 10
            new OperateDecoder(new Dictionary<int, RegDecoder>
            {
                { 0x00, new RegDecoder(Opcode.addl) },
                { 0x02, new RegDecoder(Opcode.s4addl) },
                { 0x09, new RegDecoder(Opcode.subl) },
                { 0x0B, new RegDecoder(Opcode.s4subl) },
                { 0x0F, new RegDecoder(Opcode.cmpbge) },
                { 0x12, new RegDecoder(Opcode.s8addl) },
                { 0x1B, new RegDecoder(Opcode.s8subl) },
                { 0x1D, new RegDecoder(Opcode.cmpult) },
                { 0x20, new RegDecoder(Opcode.addq) },
                { 0x22, new RegDecoder(Opcode.s4addq) },
                { 0x29, new RegDecoder(Opcode.subq) },
                { 0x2B, new RegDecoder(Opcode.s4subq) },
                { 0x2D, new RegDecoder(Opcode.cmpeq) },
                { 0x32, new RegDecoder(Opcode.s8addq) },
                { 0x3B, new RegDecoder(Opcode.s8subq) },
                { 0x3D, new RegDecoder(Opcode.cmpule) },
                { 0x40, new RegDecoder(Opcode.addl_v) },
                { 0x49, new RegDecoder(Opcode.subl_v) },
                { 0x4D, new RegDecoder(Opcode.cmplt) },
                { 0x60, new RegDecoder(Opcode.addq_v) },
                { 0x69, new RegDecoder(Opcode.subq_v) },
                { 0x6D, new RegDecoder(Opcode.cmple) },
            }),
            new OperateDecoder(new Dictionary<int, RegDecoder> // 11
            {
                { 0x00, new RegDecoder(Opcode.and) },
                { 0x08, new RegDecoder(Opcode.bic) },
                { 0x14, new RegDecoder(Opcode.cmovlbs) },
                { 0x16, new RegDecoder(Opcode.cmovlbc) },
                { 0x20, new RegDecoder(Opcode.bis) },
                { 0x24, new RegDecoder(Opcode.cmovne) },
                { 0x26, new RegDecoder(Opcode.cmovne) },
                { 0x28, new RegDecoder(Opcode.ornot) },
                { 0x40, new RegDecoder(Opcode.xor) },
                { 0x44, new RegDecoder(Opcode.cmovlt) },
                { 0x46, new RegDecoder(Opcode.cmovge) },
                { 0x66, new RegDecoder(Opcode.cmovgt) },
                { 0x6C, new RegDecoder(Opcode.implver) },
            }),
            new OperateDecoder(new Dictionary<int, RegDecoder> // 12
            {
                { 0x02, new RegDecoder(Opcode.mskbl) },
                { 0x06, new RegDecoder(Opcode.extbl) },
                { 0x0B, new RegDecoder(Opcode.insbl) },
                { 0x12, new RegDecoder(Opcode.mskwl) },
                { 0x16, new RegDecoder(Opcode.extwl) },
                { 0x1B, new RegDecoder(Opcode.inswl) },
                { 0x22, new RegDecoder(Opcode.mskll) },
                { 0x26, new RegDecoder(Opcode.extll) },
                { 0x2B, new RegDecoder(Opcode.insll) },
                { 0x30, new RegDecoder(Opcode.zap) },
                { 0x31, new RegDecoder(Opcode.zapnot) },
                { 0x32, new RegDecoder(Opcode.mskql) },
                { 0x34, new RegDecoder(Opcode.srl) },
                { 0x36, new RegDecoder(Opcode.extql) },
                { 0x39, new RegDecoder(Opcode.sll) },
                { 0x3B, new RegDecoder(Opcode.insql) },
                { 0x3C, new RegDecoder(Opcode.src) },
                { 0x52, new RegDecoder(Opcode.mskwh) },
                { 0x57, new RegDecoder(Opcode.inswh) },
                { 0x5A, new RegDecoder(Opcode.extwh) },
                { 0x62, new RegDecoder(Opcode.msklh) },
                { 0x67, new RegDecoder(Opcode.inslh) },
                { 0x6A, new RegDecoder(Opcode.extlh) },
                { 0x72, new RegDecoder(Opcode.mskqh) },
                { 0x77, new RegDecoder(Opcode.insqh) },
                { 0x7A, new RegDecoder(Opcode.extqh) },
            }),
            new OperateDecoder(new Dictionary<int, RegDecoder>  // 13
            {
                { 0x00, new RegDecoder(Opcode.mull) },
                { 0x20, new RegDecoder(Opcode.mulq) },
                { 0x30, new RegDecoder(Opcode.umulh) },
                { 0x40, new RegDecoder(Opcode.mull_v) },
                { 0x60, new RegDecoder(Opcode.mulq_v) },
            }),

            new FOperateDecoder(new Dictionary<int, Decoder> // 14
            {
                { 0x004, new FRegDecoder(Opcode.itofs) },
                { 0x00A, new FRegDecoder(Opcode.sqrtf_c) },
                { 0x00B, new FRegDecoder(Opcode.sqrts_c) },
                { 0x014, new FRegDecoder(Opcode.itoff) },
                { 0x024, new FRegDecoder(Opcode.itoft) },
                { 0x02A, new FRegDecoder(Opcode.sqrtg_c) },
                { 0x02B, new FRegDecoder(Opcode.sqrtt_c) },
                { 0x04B, new FRegDecoder(Opcode.sqrts_m) },
                { 0x06B, new FRegDecoder(Opcode.sqrtt_m) },
                { 0x08A, new FRegDecoder(Opcode.sqrtf) },
                { 0x08B, new FRegDecoder(Opcode.sqrts) },
                { 0x0AA, new FRegDecoder(Opcode.sqrtg) },
                { 0x0AB, new FRegDecoder(Opcode.sqrtt) },
                { 0x0CB, new FRegDecoder(Opcode.sqrts_d) },
                { 0x0EB, new FRegDecoder(Opcode.sqrtt_d) },
                { 0x10A, new FRegDecoder(Opcode.sqrtf_uc) },
                { 0x10B, new FRegDecoder(Opcode.sqrts_uc) },
                { 0x12A, new FRegDecoder(Opcode.sqrtg_uc) },
                { 0x12B, new FRegDecoder(Opcode.sqrtt_uc) },
                { 0x14B, new FRegDecoder(Opcode.sqrts_um) },
                { 0x16B, new FRegDecoder(Opcode.sqrtt_um) },
                { 0x18A, new FRegDecoder(Opcode.sqrtf_u) },
                { 0x18B, new FRegDecoder(Opcode.sqrts_u) },
                { 0x1AA, new FRegDecoder(Opcode.sqrtg_u) },
                { 0x1AB, new FRegDecoder(Opcode.sqrtt_u) },
                { 0x1CB, new FRegDecoder(Opcode.sqrts_ud) },
                { 0x1EB, new FRegDecoder(Opcode.sqrtt_ud) },
                { 0x40A, new FRegDecoder(Opcode.sqrtf_sc) },
                { 0x42A, new FRegDecoder(Opcode.sqrtg_sc) },
                { 0x48A, new FRegDecoder(Opcode.sqrtf_s) },
                { 0x4AA, new FRegDecoder(Opcode.sqrtg_s) },
                { 0x50A, new FRegDecoder(Opcode.sqrtf_suc) },
                { 0x50B, new FRegDecoder(Opcode.sqrts_suc) },
                { 0x52A, new FRegDecoder(Opcode.sqrtg_suc) },
                { 0x52B, new FRegDecoder(Opcode.sqrtt_suc) },
                { 0x54B, new FRegDecoder(Opcode.sqrts_sum) },
                { 0x56B, new FRegDecoder(Opcode.sqrtt_sum) },
                { 0x58A, new FRegDecoder(Opcode.sqrtf_su) },
                { 0x58B, new FRegDecoder(Opcode.sqrts_su) },
                { 0x5AA, new FRegDecoder(Opcode.sqrtg_su) },
                { 0x5AB, new FRegDecoder(Opcode.sqrtt_su) },
                { 0x5CB, new FRegDecoder(Opcode.sqrts_sud) },
                { 0x5EB, new FRegDecoder(Opcode.sqrtt_sud) },
                { 0x70B, new FRegDecoder(Opcode.sqrts_suic) },
                { 0x72B, new FRegDecoder(Opcode.sqrtt_suic) },
                { 0x74B, new FRegDecoder(Opcode.sqrts_suim) },
                { 0x76B, new FRegDecoder(Opcode.sqrtt_suim) },
                { 0x78B, new FRegDecoder(Opcode.sqrts_sui) },
                { 0x7AB, new FRegDecoder(Opcode.sqrtt_sui) },
                { 0x7CB, new FRegDecoder(Opcode.sqrts_suid) },
                { 0x7EB, new FRegDecoder(Opcode.sqrtt_suid) },

            }),
            new FOperateDecoder(new Dictionary<int, Decoder> // 15
            {
                { 0x000, new FRegDecoder(Opcode.addf_c) },
                { 0x001, new FRegDecoder(Opcode.subf_c) },
                { 0x002, new FRegDecoder(Opcode.mulf_c) },
                { 0x003, new FRegDecoder(Opcode.divf_c) },
                { 0x01E, new CvtDecoder(Opcode.cvtdg_c) },
                { 0x020, new FRegDecoder(Opcode.addg_c) },
                { 0x021, new FRegDecoder(Opcode.subg_c) },
                { 0x022, new FRegDecoder(Opcode.mulg_c) },
                { 0x023, new FRegDecoder(Opcode.divg_c) },
                { 0x02C, new CvtDecoder(Opcode.cvtgf_c) },
                { 0x02D, new CvtDecoder(Opcode.cvtgd_c) },
                { 0x02F, new CvtDecoder(Opcode.cvtgq_c) },
                { 0x03C, new CvtDecoder(Opcode.cvtqf_c) },
                { 0x03E, new CvtDecoder(Opcode.cvtqg_c) },
                { 0x080, new FRegDecoder(Opcode.addf) },
                //{ 0x081, new FRegDecoder(Opcode.negf) },	_* /*pse*/udo */
                { 0x081, new FRegDecoder(Opcode.subf) },
                { 0x082, new FRegDecoder(Opcode.mulf) },
                { 0x083, new FRegDecoder(Opcode.divf) },
                { 0x09E, new CvtDecoder(Opcode.cvtdg) },
                { 0x0A0, new FRegDecoder(Opcode.addg) },
                //{ 0x0A1, new FRegDecoder(Opcode.negg) },	_* pseudo */
                { 0x0A1, new FRegDecoder(Opcode.subg) },
                { 0x0A2, new FRegDecoder(Opcode.mulg) },
                { 0x0A3, new FRegDecoder(Opcode.divg) },
                { 0x0A5, new FRegDecoder(Opcode.cmpgeq) },
                { 0x0A6, new FRegDecoder(Opcode.cmpglt) },
                { 0x0A7, new FRegDecoder(Opcode.cmpgle) },
                { 0x0AC, new CvtDecoder(Opcode.cvtgf) },
                { 0x0AD, new CvtDecoder(Opcode.cvtgd) },
                { 0x0AF, new CvtDecoder(Opcode.cvtgq) },
                { 0x0BC, new CvtDecoder(Opcode.cvtqf) },
                { 0x0BE, new CvtDecoder(Opcode.cvtqg) },
                { 0x100, new FRegDecoder(Opcode.addf_uc) },
                { 0x101, new FRegDecoder(Opcode.subf_uc) },
                { 0x102, new FRegDecoder(Opcode.mulf_uc) },
                { 0x103, new FRegDecoder(Opcode.divf_uc) },
                { 0x11E, new CvtDecoder(Opcode.cvtdg_uc) },
                { 0x120, new FRegDecoder(Opcode.addg_uc) },
                { 0x121, new FRegDecoder(Opcode.subg_uc) },
                { 0x122, new FRegDecoder(Opcode.mulg_uc) },
                { 0x123, new FRegDecoder(Opcode.divg_uc) },
                { 0x12C, new CvtDecoder(Opcode.cvtgf_uc) },
                { 0x12D, new CvtDecoder(Opcode.cvtgd_uc) },
                { 0x12F, new CvtDecoder(Opcode.cvtgq_vc) },
                { 0x180, new FRegDecoder(Opcode.addf_u) },
                { 0x181, new FRegDecoder(Opcode.subf_u) },
                { 0x182, new FRegDecoder(Opcode.mulf_u) },
                { 0x183, new FRegDecoder(Opcode.divf_u) },
                { 0x19E, new CvtDecoder(Opcode.cvtdg_u) },
                { 0x1A0, new FRegDecoder(Opcode.addg_u) },
                { 0x1A1, new FRegDecoder(Opcode.subg_u) },
                { 0x1A2, new FRegDecoder(Opcode.mulg_u) },
                { 0x1A3, new FRegDecoder(Opcode.divg_u) },
                { 0x1AC, new CvtDecoder(Opcode.cvtgf_u) },
                { 0x1AD, new CvtDecoder(Opcode.cvtgd_u) },
                { 0x1AF, new CvtDecoder(Opcode.cvtgq_v) },
                { 0x400, new FRegDecoder(Opcode.addf_sc) },
                { 0x401, new FRegDecoder(Opcode.subf_sc) },
                { 0x402, new FRegDecoder(Opcode.mulf_sc) },
                { 0x403, new FRegDecoder(Opcode.divf_sc) },
                { 0x41E, new CvtDecoder(Opcode.cvtdg_sc) },
                { 0x420, new FRegDecoder(Opcode.addg_sc) },
                { 0x421, new FRegDecoder(Opcode.subg_sc) },
                { 0x422, new FRegDecoder(Opcode.mulg_sc) },
                { 0x423, new FRegDecoder(Opcode.divg_sc) },
                { 0x42C, new CvtDecoder(Opcode.cvtgf_sc) },
                { 0x42D, new CvtDecoder(Opcode.cvtgd_sc) },
                { 0x42F, new CvtDecoder(Opcode.cvtgq_sc) },
                { 0x480, new FRegDecoder(Opcode.addf_s) },
                //{ 0x481, new FRegDecoder(Opcode.negf_s) },	_* pseudo */
                { 0x481, new FRegDecoder(Opcode.subf_s) },
                { 0x482, new FRegDecoder(Opcode.mulf_s) },
                { 0x483, new FRegDecoder(Opcode.divf_s) },
                { 0x49E, new CvtDecoder(Opcode.cvtdg_s) },
                { 0x4A0, new FRegDecoder(Opcode.addg_s) },
                //{ 0x4A1, new FRegDecoder(Opcode.negg_s) },	_* pseudo */
                { 0x4A1, new FRegDecoder(Opcode.subg_s) },
                { 0x4A2, new FRegDecoder(Opcode.mulg_s) },
                { 0x4A3, new FRegDecoder(Opcode.divg_s) },
                { 0x4A5, new FRegDecoder(Opcode.cmpgeq_s) },
                { 0x4A6, new FRegDecoder(Opcode.cmpglt_s) },
                { 0x4A7, new FRegDecoder(Opcode.cmpgle_s) },
                { 0x4AC, new CvtDecoder(Opcode.cvtgf_s) },
                { 0x4AD, new CvtDecoder(Opcode.cvtgd_s) },
                { 0x4AF, new CvtDecoder(Opcode.cvtgq_s) },
                { 0x500, new FRegDecoder(Opcode.addf_suc) },
                { 0x501, new FRegDecoder(Opcode.subf_suc) },
                { 0x502, new FRegDecoder(Opcode.mulf_suc) },
                { 0x503, new FRegDecoder(Opcode.divf_suc) },
                { 0x51E, new CvtDecoder(Opcode.cvtdg_suc) },
                { 0x520, new FRegDecoder(Opcode.addg_suc) },
                { 0x521, new FRegDecoder(Opcode.subg_suc) },
                { 0x522, new FRegDecoder(Opcode.mulg_suc) },
                { 0x523, new FRegDecoder(Opcode.divg_suc) },
                { 0x52C, new CvtDecoder(Opcode.cvtgf_suc) },
                { 0x52D, new CvtDecoder(Opcode.cvtgd_suc) },
                { 0x52F, new CvtDecoder(Opcode.cvtgq_svc) },
                { 0x580, new FRegDecoder(Opcode.addf_su) },
                { 0x581, new FRegDecoder(Opcode.subf_su) },
                { 0x582, new FRegDecoder(Opcode.mulf_su) },
                { 0x583, new FRegDecoder(Opcode.divf_su) },
                { 0x59E, new CvtDecoder(Opcode.cvtdg_su) },
                { 0x5A0, new FRegDecoder(Opcode.addg_su) },
                { 0x5A1, new FRegDecoder(Opcode.subg_su) },
                { 0x5A2, new FRegDecoder(Opcode.mulg_su) },
                { 0x5A3, new FRegDecoder(Opcode.divg_su) },
                { 0x5AC, new CvtDecoder(Opcode.cvtgf_su) },
                { 0x5AD, new CvtDecoder(Opcode.cvtgd_su) },
                { 0x5AF, new CvtDecoder(Opcode.cvtgq_sv) },

            }),
            new FOperateDecoder(new Dictionary<int, Decoder> // 16
            {
                { 0x000, new FRegDecoder(Opcode.adds_c) },
                { 0x001, new FRegDecoder(Opcode.subs_c) },
                { 0x002, new FRegDecoder(Opcode.muls_c) },
                { 0x003, new FRegDecoder(Opcode.divs_c) },
                { 0x020, new FRegDecoder(Opcode.addt_c) },
                { 0x021, new FRegDecoder(Opcode.subt_c) },
                { 0x022, new FRegDecoder(Opcode.mult_c) },
                { 0x023, new FRegDecoder(Opcode.divt_c) },
                { 0x02C, new CvtDecoder(Opcode.cvtts_c) },
                { 0x02F, new CvtDecoder(Opcode.cvttq_c) },
                { 0x03C, new CvtDecoder(Opcode.cvtqs_c) },
                { 0x03E, new CvtDecoder(Opcode.cvtqt_c) },
                { 0x040, new FRegDecoder(Opcode.adds_m) },
                { 0x041, new FRegDecoder(Opcode.subs_m) },
                { 0x042, new FRegDecoder(Opcode.muls_m) },
                { 0x043, new FRegDecoder(Opcode.divs_m) },
                { 0x060, new FRegDecoder(Opcode.addt_m) },
                { 0x061, new FRegDecoder(Opcode.subt_m) },
                { 0x062, new FRegDecoder(Opcode.mult_m) },
                { 0x063, new FRegDecoder(Opcode.divt_m) },
                { 0x06C, new CvtDecoder(Opcode.cvtts_m) },
                { 0x06F, new CvtDecoder(Opcode.cvttq_m) },
                { 0x07C, new CvtDecoder(Opcode.cvtqs_m) },
                { 0x07E, new CvtDecoder(Opcode.cvtqt_m) },
                { 0x080, new FRegDecoder(Opcode.adds) },
                //{ 0x081, new FRegDecoder(Opcode.negs) },	_* pseudo */
                { 0x081, new FRegDecoder(Opcode.subs) },
                { 0x082, new FRegDecoder(Opcode.muls) },
                { 0x083, new FRegDecoder(Opcode.divs) },
                { 0x0A0, new FRegDecoder(Opcode.addt) },
                //{ 0x0A1, new FRegDecoder(Opcode.negt) },	_* pseudo */
                { 0x0A1, new FRegDecoder(Opcode.subt) },
                { 0x0A2, new FRegDecoder(Opcode.mult) },
                { 0x0A3, new FRegDecoder(Opcode.divt) },
                { 0x0A4, new FRegDecoder(Opcode.cmptun) },
                { 0x0A5, new FRegDecoder(Opcode.cmpteq) },
                { 0x0A6, new FRegDecoder(Opcode.cmptlt) },
                { 0x0A7, new FRegDecoder(Opcode.cmptle) },
                { 0x0AC, new CvtDecoder(Opcode.cvtts) },
                { 0x0AF, new CvtDecoder(Opcode.cvttq) },
                { 0x0BC, new CvtDecoder(Opcode.cvtqs) },
                { 0x0BE, new CvtDecoder(Opcode.cvtqt) },
                { 0x0C0, new FRegDecoder(Opcode.adds_d) },
                { 0x0C1, new FRegDecoder(Opcode.subs_d) },
                { 0x0C2, new FRegDecoder(Opcode.muls_d) },
                { 0x0C3, new FRegDecoder(Opcode.divs_d) },
                { 0x0E0, new FRegDecoder(Opcode.addt_d) },
                { 0x0E1, new FRegDecoder(Opcode.subt_d) },
                { 0x0E2, new FRegDecoder(Opcode.mult_d) },
                { 0x0E3, new FRegDecoder(Opcode.divt_d) },
                { 0x0EC, new CvtDecoder(Opcode.cvtts_d) },
                { 0x0EF, new CvtDecoder(Opcode.cvttq_d) },
                { 0x0FC, new CvtDecoder(Opcode.cvtqs_d) },
                { 0x0FE, new CvtDecoder(Opcode.cvtqt_d) },
                { 0x100, new FRegDecoder(Opcode.adds_uc) },
                { 0x101, new FRegDecoder(Opcode.subs_uc) },
                { 0x102, new FRegDecoder(Opcode.muls_uc) },
                { 0x103, new FRegDecoder(Opcode.divs_uc) },
                { 0x120, new FRegDecoder(Opcode.addt_uc) },
                { 0x121, new FRegDecoder(Opcode.subt_uc) },
                { 0x122, new FRegDecoder(Opcode.mult_uc) },
                { 0x123, new FRegDecoder(Opcode.divt_uc) },
                { 0x12C, new CvtDecoder(Opcode.cvtts_uc) },
                { 0x12F, new CvtDecoder(Opcode.cvttq_vc) },
                { 0x140, new FRegDecoder(Opcode.adds_um) },
                { 0x141, new FRegDecoder(Opcode.subs_um) },
                { 0x142, new FRegDecoder(Opcode.muls_um) },
                { 0x143, new FRegDecoder(Opcode.divs_um) },
                { 0x160, new FRegDecoder(Opcode.addt_um) },
                { 0x161, new FRegDecoder(Opcode.subt_um) },
                { 0x162, new FRegDecoder(Opcode.mult_um) },
                { 0x163, new FRegDecoder(Opcode.divt_um) },
                { 0x16C, new CvtDecoder(Opcode.cvtts_um) },
                { 0x16F, new CvtDecoder(Opcode.cvttq_vm) },
                { 0x180, new FRegDecoder(Opcode.adds_u) },
                { 0x181, new FRegDecoder(Opcode.subs_u) },
                { 0x182, new FRegDecoder(Opcode.muls_u) },
                { 0x183, new FRegDecoder(Opcode.divs_u) },
                { 0x1A0, new FRegDecoder(Opcode.addt_u) },
                { 0x1A1, new FRegDecoder(Opcode.subt_u) },
                { 0x1A2, new FRegDecoder(Opcode.mult_u) },
                { 0x1A3, new FRegDecoder(Opcode.divt_u) },
                { 0x1AC, new CvtDecoder(Opcode.cvtts_u) },
                { 0x1AF, new CvtDecoder(Opcode.cvttq_v) },
                { 0x1C0, new FRegDecoder(Opcode.adds_ud) },
                { 0x1C1, new FRegDecoder(Opcode.subs_ud) },
                { 0x1C2, new FRegDecoder(Opcode.muls_ud) },
                { 0x1C3, new FRegDecoder(Opcode.divs_ud) },
                { 0x1E0, new FRegDecoder(Opcode.addt_ud) },
                { 0x1E1, new FRegDecoder(Opcode.subt_ud) },
                { 0x1E2, new FRegDecoder(Opcode.mult_ud) },
                { 0x1E3, new FRegDecoder(Opcode.divt_ud) },
                { 0x1EC, new CvtDecoder(Opcode.cvtts_ud) },
                { 0x1EF, new CvtDecoder(Opcode.cvttq_vd) },
                { 0x2AC, new CvtDecoder(Opcode.cvtst) },
                { 0x500, new FRegDecoder(Opcode.adds_suc) },
                { 0x501, new FRegDecoder(Opcode.subs_suc) },
                { 0x502, new FRegDecoder(Opcode.muls_suc) },
                { 0x503, new FRegDecoder(Opcode.divs_suc) },
                { 0x520, new FRegDecoder(Opcode.addt_suc) },
                { 0x521, new FRegDecoder(Opcode.subt_suc) },
                { 0x522, new FRegDecoder(Opcode.mult_suc) },
                { 0x523, new FRegDecoder(Opcode.divt_suc) },
                { 0x52C, new CvtDecoder(Opcode.cvtts_suc) },
                { 0x52F, new CvtDecoder(Opcode.cvttq_svc) },
                { 0x540, new FRegDecoder(Opcode.adds_sum) },
                { 0x541, new FRegDecoder(Opcode.subs_sum) },
                { 0x542, new FRegDecoder(Opcode.muls_sum) },
                { 0x543, new FRegDecoder(Opcode.divs_sum) },
                { 0x560, new FRegDecoder(Opcode.addt_sum) },
                { 0x561, new FRegDecoder(Opcode.subt_sum) },
                { 0x562, new FRegDecoder(Opcode.mult_sum) },
                { 0x563, new FRegDecoder(Opcode.divt_sum) },
                { 0x56C, new CvtDecoder(Opcode.cvtts_sum) },
                { 0x56F, new CvtDecoder(Opcode.cvttq_svm) },
                { 0x580, new FRegDecoder(Opcode.adds_su) },
                //{ 0x581, new FRegDecoder(Opcode.negs_su) },	_* pseudo */
                { 0x581, new FRegDecoder(Opcode.subs_su) },
                { 0x582, new FRegDecoder(Opcode.muls_su) },
                { 0x583, new FRegDecoder(Opcode.divs_su) },
                { 0x5A0, new FRegDecoder(Opcode.addt_su) },
                //{ 0x5A1, new FRegDecoder(Opcode.negt_su) },	_* pseudo */
                { 0x5A1, new FRegDecoder(Opcode.subt_su) },
                { 0x5A2, new FRegDecoder(Opcode.mult_su) },
                { 0x5A3, new FRegDecoder(Opcode.divt_su) },
                { 0x5A4, new FRegDecoder(Opcode.cmptun_su) },
                { 0x5A5, new FRegDecoder(Opcode.cmpteq_su) },
                { 0x5A6, new FRegDecoder(Opcode.cmptlt_su) },
                { 0x5A7, new FRegDecoder(Opcode.cmptle_su) },
                { 0x5AC, new CvtDecoder(Opcode.cvtts_su) },
                { 0x5AF, new CvtDecoder(Opcode.cvttq_sv) },
                { 0x5C0, new FRegDecoder(Opcode.adds_sud) },
                { 0x5C1, new FRegDecoder(Opcode.subs_sud) },
                { 0x5C2, new FRegDecoder(Opcode.muls_sud) },
                { 0x5C3, new FRegDecoder(Opcode.divs_sud) },
                { 0x5E0, new FRegDecoder(Opcode.addt_sud) },
                { 0x5E1, new FRegDecoder(Opcode.subt_sud) },
                { 0x5E2, new FRegDecoder(Opcode.mult_sud) },
                { 0x5E3, new FRegDecoder(Opcode.divt_sud) },
                { 0x5EC, new CvtDecoder(Opcode.cvtts_sud) },
                { 0x5EF, new CvtDecoder(Opcode.cvttq_svd) },
                { 0x6AC, new CvtDecoder(Opcode.cvtst_s) },
                { 0x700, new FRegDecoder(Opcode.adds_suic) },
                { 0x701, new FRegDecoder(Opcode.subs_suic) },
                { 0x702, new FRegDecoder(Opcode.muls_suic) },
                { 0x703, new FRegDecoder(Opcode.divs_suic) },
                { 0x720, new FRegDecoder(Opcode.addt_suic) },
                { 0x721, new FRegDecoder(Opcode.subt_suic) },
                { 0x722, new FRegDecoder(Opcode.mult_suic) },
                { 0x723, new FRegDecoder(Opcode.divt_suic) },
                { 0x72C, new CvtDecoder(Opcode.cvtts_suic) },
                { 0x72F, new CvtDecoder(Opcode.cvttq_svic) },
                { 0x73C, new CvtDecoder(Opcode.cvtqs_suic) },
                { 0x73E, new CvtDecoder(Opcode.cvtqt_suic) },
                { 0x740, new FRegDecoder(Opcode.adds_suim) },
                { 0x741, new FRegDecoder(Opcode.subs_suim) },
                { 0x742, new FRegDecoder(Opcode.muls_suim) },
                { 0x743, new FRegDecoder(Opcode.divs_suim) },
                { 0x760, new FRegDecoder(Opcode.addt_suim) },
                { 0x761, new FRegDecoder(Opcode.subt_suim) },
                { 0x762, new FRegDecoder(Opcode.mult_suim) },
                { 0x763, new FRegDecoder(Opcode.divt_suim) },
                { 0x76C, new CvtDecoder(Opcode.cvtts_suim) },
                { 0x76F, new CvtDecoder(Opcode.cvttq_svim) },
                { 0x77C, new CvtDecoder(Opcode.cvtqs_suim) },
                { 0x77E, new CvtDecoder(Opcode.cvtqt_suim) },
                { 0x780, new FRegDecoder(Opcode.adds_sui) },
                //{ 0x781, new FRegDecoder(Opcode.negs_sui) },	_* pseudo */
                { 0x781, new FRegDecoder(Opcode.subs_sui) },
                { 0x782, new FRegDecoder(Opcode.muls_sui) },
                { 0x783, new FRegDecoder(Opcode.divs_sui) },
                { 0x7A0, new FRegDecoder(Opcode.addt_sui) },
                //{ 0x7A1, new FRegDecoder(Opcode.negt_sui) },	_* pseudo */
                { 0x7A1, new FRegDecoder(Opcode.subt_sui) },
                { 0x7A2, new FRegDecoder(Opcode.mult_sui) },
                { 0x7A3, new FRegDecoder(Opcode.divt_sui) },
                { 0x7AC, new CvtDecoder(Opcode.cvtts_sui) },
                { 0x7AF, new CvtDecoder(Opcode.cvttq_svi) },
                { 0x7BC, new CvtDecoder(Opcode.cvtqs_sui) },
                { 0x7BE, new CvtDecoder(Opcode.cvtqt_sui) },
                { 0x7C0, new FRegDecoder(Opcode.adds_suid) },
                { 0x7C1, new FRegDecoder(Opcode.subs_suid) },
                { 0x7C2, new FRegDecoder(Opcode.muls_suid) },
                { 0x7C3, new FRegDecoder(Opcode.divs_suid) },
                { 0x7E0, new FRegDecoder(Opcode.addt_suid) },
                { 0x7E1, new FRegDecoder(Opcode.subt_suid) },
                { 0x7E2, new FRegDecoder(Opcode.mult_suid) },
                { 0x7E3, new FRegDecoder(Opcode.divt_suid) },
                { 0x7EC, new CvtDecoder(Opcode.cvtts_suid) },
                { 0x7EF, new CvtDecoder(Opcode.cvttq_svid) },
                { 0x7FC, new CvtDecoder(Opcode.cvtqs_suid) },
                { 0x7FE, new CvtDecoder(Opcode.cvtqt_suid) },
            }),
            new FOperateDecoder(new Dictionary<int, Decoder>
            {
                  { 0x010, new CvtDecoder(Opcode.cvtlq) },
                  //{ 0x020, new FRegDecoder(Opcode.fnop) },	_* pseudo */
                  //{ 0x020, new FRegDecoder(Opcode.fclr) },	_* pseudo */
                  //{ 0x020, new FRegDecoder(Opcode.fabs) },	_* pseudo */
                  //{ 0x020, new FRegDecoder(Opcode.fmov) }, _* pseudo */
                  { 0x020, new FRegDecoder(Opcode.cpys) },
                  //{ 0x021, new FRegDecoder(Opcode.fneg) }, _* pseudo */
                  { 0x021, new FRegDecoder(Opcode.cpysn) },
                  { 0x022, new FRegDecoder(Opcode.cpyse) },
                  { 0x024, new FRegDecoder(Opcode.mt_fpcr) },
                  { 0x025, new FRegDecoder(Opcode.mf_fpcr) },
                  { 0x02A, new FRegDecoder(Opcode.fcmoveq) },
                  { 0x02B, new FRegDecoder(Opcode.fcmovne) },
                  { 0x02C, new FRegDecoder(Opcode.fcmovlt) },
                  { 0x02D, new FRegDecoder(Opcode.fcmovge) },
                  { 0x02E, new FRegDecoder(Opcode.fcmovle) },
                  { 0x02F, new FRegDecoder(Opcode.fcmovgt) },
                  { 0x030, new CvtDecoder(Opcode.cvtql) },
                  { 0x130, new CvtDecoder(Opcode.cvtql_v) },
                  { 0x530, new CvtDecoder(Opcode.cvtql_sv) },
            }),

            new PalDecoder(new Dictionary<uint, (Opcode,InstrClass)> // 18
            {
                { 0x0000, (Opcode.trapb,InstrClass.Transfer|InstrClass.Call) }
            }),
            new PalDecoder(new Dictionary<uint, (Opcode,InstrClass)> // 19 
            {
            }),
            new JMemDecoder(),
            new PalDecoder(new Dictionary<uint, (Opcode,InstrClass)> // 1B 
            {
            }),

            new NyiDecoder(),
            new NyiDecoder(),
            new PalDecoder(new Dictionary<uint, (Opcode,InstrClass)>
            {       //$REVIEW: these palcodes are OS-dependent....
            }),
            new NyiDecoder(),
            // 20
            new FMemDecoder(Opcode.ldf),
            new FMemDecoder(Opcode.ldg),
            new FMemDecoder(Opcode.lds),
            new FMemDecoder(Opcode.ldt),

            new FMemDecoder(Opcode.stf),
            new FMemDecoder(Opcode.stg),
            new FMemDecoder(Opcode.sts),
            new FMemDecoder(Opcode.stt),

            new MemDecoder(Opcode.ldl),
            new MemDecoder(Opcode.ldq),
            new MemDecoder(Opcode.ldl_l),
            new MemDecoder(Opcode.ldq_l),

            new MemDecoder(Opcode.stl),
            new MemDecoder(Opcode.stq),
            new MemDecoder(Opcode.stl_c),
            new MemDecoder(Opcode.stq_c),
            // 30
            new BranchDecoder(Opcode.br),
            new FBranchDecoder(Opcode.fbeq),
            new FBranchDecoder(Opcode.fblt),
            new FBranchDecoder(Opcode.fble),

            new BranchDecoder(Opcode.bsr),
            new FBranchDecoder(Opcode.fbne),
            new FBranchDecoder(Opcode.fbge),
            new FBranchDecoder(Opcode.fbgt),

            new BranchDecoder(Opcode.blbc),
            new BranchDecoder(Opcode.beq),
            new BranchDecoder(Opcode.blt),
            new BranchDecoder(Opcode.ble),

            new BranchDecoder(Opcode.blbs),
            new BranchDecoder(Opcode.bne),
            new BranchDecoder(Opcode.bge),
            new BranchDecoder(Opcode.bgt),
        };
    }
}