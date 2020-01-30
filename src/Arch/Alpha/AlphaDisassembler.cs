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

using System;
using System.Collections.Generic;
using Reko.Core;
using Reko.Core.Machine;
using System.Diagnostics;
using Reko.Core.Types;

namespace Reko.Arch.Alpha
{
    using Decoder = Decoder<AlphaDisassembler, Mnemonic, AlphaInstruction>;

    public class AlphaDisassembler : DisassemblerBase<AlphaInstruction, Mnemonic>
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

        public override AlphaInstruction CreateInvalidInstruction()
        {
            return new AlphaInstruction {
                Mnemonic = Mnemonic.invalid,
                InstructionClass = InstrClass.Invalid,
                Operands = new MachineOperand[0]
            };
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
            return CreateInvalidInstruction();
        }

        private class MemDecoder : Decoder
        {
            private readonly Mnemonic mnemonic;

            public MemDecoder(Mnemonic mnemonic)
            {
                this.mnemonic = mnemonic;
            }

            public override AlphaInstruction Decode(uint uInstr, AlphaDisassembler dasm)
            {
                return new AlphaInstruction
                {
                    Mnemonic = this.mnemonic,
                    InstructionClass = InstrClass.Linear,
                    Operands = new MachineOperand[] {
                        dasm.AluRegister(uInstr >> 21),
                        new MemoryOperand(
                            PrimitiveType.Word32,    // Dummy value
                            dasm.AluRegister(uInstr >> 16).Register,
                            (short)uInstr)
                    }
                };
            }
        }

        private class FMemDecoder : Decoder
        {
            private readonly Mnemonic mnemonic;

            public FMemDecoder(Mnemonic mnemonic)
            {
                this.mnemonic = mnemonic;
            }

            public override AlphaInstruction Decode(uint uInstr, AlphaDisassembler dasm)
            {
                return new AlphaInstruction
                {
                    Mnemonic = this.mnemonic,
                    InstructionClass = InstrClass.Linear,
                    Operands = new MachineOperand[]
                    {
                        dasm.FpuRegister(uInstr >> 21),
                        new MemoryOperand(
                            PrimitiveType.Word32,    // Dummy value
                            dasm.AluRegister(uInstr >> 16).Register,
                            (short)uInstr)
                    }
                };
            }
        }

        private class JMemDecoder : Decoder
        {
            private readonly static Mnemonic[] mnemonics = {
                Mnemonic.jmp, Mnemonic.jsr, Mnemonic.ret, Mnemonic.jsr_coroutine
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
                    Mnemonic = mnemonics[(uInstr >> 14) & 0x3],
                    InstructionClass = iclasses[(uInstr>> 14) & 0x3],
                    Operands = new MachineOperand[]
                    {
                        dasm.AluRegister(uInstr >> 21),
                        dasm.AluRegister(uInstr >> 16)
                    }
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
            private readonly Mnemonic mnemonic;

            public BranchDecoder(Mnemonic mnemonic)
            {
                this.mnemonic = mnemonic;
            }

            public override AlphaInstruction Decode(uint uInstr, AlphaDisassembler dasm)
            {
                int offset = ((int)uInstr << 11) >> 9;
                var op1 = dasm.AluRegister(uInstr >> 21);
                var op2 = AddressOperand.Create(dasm.rdr.Address + offset);
                return new AlphaInstruction
                {
                    Mnemonic = this.mnemonic,
                    InstructionClass = InstrClass.ConditionalTransfer,
                    Operands = new MachineOperand[] { op1, op2 }
                };
            }
        }

        private class FBranchDecoder : Decoder
        {
            private readonly Mnemonic mnemonic;

            public FBranchDecoder(Mnemonic mnemonic)
            {
                this.mnemonic = mnemonic;
            }

            public override AlphaInstruction Decode(uint uInstr, AlphaDisassembler dasm)
            {
                int offset = ((int)uInstr << 11) >> 9;
                var op1 = dasm.FpuRegister(uInstr >> 21);
                var op2 = AddressOperand.Create(dasm.rdr.Address + offset);
                return new AlphaInstruction
                {
                    Mnemonic = this.mnemonic,
                    InstructionClass = InstrClass.ConditionalTransfer,
                    Operands = new MachineOperand[] { op1, op2 }
                };
            }
        }

        private class RegDecoder : Decoder
        {
            private readonly Mnemonic mnemonic;

            public RegDecoder(Mnemonic mnemonic)
            {
                this.mnemonic = mnemonic;
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
                    Mnemonic = this.mnemonic,
                    InstructionClass = InstrClass.Linear,
                    Operands = new MachineOperand[] { op1, op2, op3 }
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
                    return dasm.CreateInvalidInstruction();
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
            private readonly Mnemonic mnemonic;

            public FRegDecoder(Mnemonic mnemonic)
            {
                this.mnemonic = mnemonic;
            }

            public override AlphaInstruction Decode(uint uInstr, AlphaDisassembler dasm)
            {
                var op1 = dasm.FpuRegister(uInstr >> 21);
                var op2 = dasm.FpuRegister(uInstr >> 16);
                var op3 = dasm.FpuRegister(uInstr);
                return new AlphaInstruction
                {
                    Mnemonic = this.mnemonic,
                    InstructionClass = InstrClass.Linear,
                    Operands = new MachineOperand[] { op1, op2, op3 }
                };
            }
        }

        private class PalDecoder : Decoder
        {
            private readonly Dictionary<uint, (Mnemonic,InstrClass)> mnemonics;

            public PalDecoder(Dictionary<uint, (Mnemonic,InstrClass)> opcodes)
            {
                this.mnemonics = opcodes;
            }

            public override AlphaInstruction Decode(uint uInstr, AlphaDisassembler dasm)
            {
                if (!mnemonics.TryGetValue(uInstr & 0x0000FFFF, out var opcode))
                    return dasm.CreateInvalidInstruction();
                return new AlphaInstruction {
                    Mnemonic = opcode.Item1,
                    InstructionClass = opcode.Item2,
                    Operands = new MachineOperand[0]
                };
            }
        }

        private class CvtDecoder : Decoder
        {
            private readonly Mnemonic mnemonic;

            public CvtDecoder(Mnemonic mnemonic)
            {
                this.mnemonic = mnemonic;
            }

            public override AlphaInstruction Decode(uint uInstr, AlphaDisassembler dasm)
            {
                var op1 = dasm.FpuRegister(uInstr >> 21);
                var op2 = dasm.FpuRegister(uInstr >> 16);
                return new AlphaInstruction
                {
                    Mnemonic = this.mnemonic,
                    InstructionClass = InstrClass.Linear,
                    Operands = new MachineOperand[] { op1, op2 }
                };
            }
        }

        private class InvalidDecoder : Decoder
        {
            public override AlphaInstruction Decode(uint uInstr, AlphaDisassembler dasm)
            {
                return dasm.CreateInvalidInstruction();
            }
        }

        private class NyiDecoder : Decoder
        {
            public override AlphaInstruction Decode(uint uInstr, AlphaDisassembler dasm)
            {
                return dasm.CreateInvalidInstruction();
            }
        }

        private static readonly Decoder[] decoders = new Decoder[64]
        {
            // 00
            new PalDecoder(new Dictionary<uint, (Mnemonic, InstrClass)>
            {       //$REVIEW: these palcodes are OS-dependent....
                { 0, (Mnemonic.halt,InstrClass.System|InstrClass.Terminates) }
            }),
            new InvalidDecoder(),
            new InvalidDecoder(),
            new InvalidDecoder(),
            new InvalidDecoder(),
            new InvalidDecoder(),
            new InvalidDecoder(),
            new InvalidDecoder(),

            new MemDecoder(Mnemonic.lda),
            new MemDecoder(Mnemonic.ldah),
            new MemDecoder(Mnemonic.ldbu),
            new MemDecoder(Mnemonic.ldq_u),

            new MemDecoder(Mnemonic.ldwu),
            new MemDecoder(Mnemonic.stw),
            new MemDecoder(Mnemonic.stb),
            new MemDecoder(Mnemonic.stq_u),
            // 10
            new OperateDecoder(new Dictionary<int, RegDecoder>
            {
                { 0x00, new RegDecoder(Mnemonic.addl) },
                { 0x02, new RegDecoder(Mnemonic.s4addl) },
                { 0x09, new RegDecoder(Mnemonic.subl) },
                { 0x0B, new RegDecoder(Mnemonic.s4subl) },
                { 0x0F, new RegDecoder(Mnemonic.cmpbge) },
                { 0x12, new RegDecoder(Mnemonic.s8addl) },
                { 0x1B, new RegDecoder(Mnemonic.s8subl) },
                { 0x1D, new RegDecoder(Mnemonic.cmpult) },
                { 0x20, new RegDecoder(Mnemonic.addq) },
                { 0x22, new RegDecoder(Mnemonic.s4addq) },
                { 0x29, new RegDecoder(Mnemonic.subq) },
                { 0x2B, new RegDecoder(Mnemonic.s4subq) },
                { 0x2D, new RegDecoder(Mnemonic.cmpeq) },
                { 0x32, new RegDecoder(Mnemonic.s8addq) },
                { 0x3B, new RegDecoder(Mnemonic.s8subq) },
                { 0x3D, new RegDecoder(Mnemonic.cmpule) },
                { 0x40, new RegDecoder(Mnemonic.addl_v) },
                { 0x49, new RegDecoder(Mnemonic.subl_v) },
                { 0x4D, new RegDecoder(Mnemonic.cmplt) },
                { 0x60, new RegDecoder(Mnemonic.addq_v) },
                { 0x69, new RegDecoder(Mnemonic.subq_v) },
                { 0x6D, new RegDecoder(Mnemonic.cmple) },
            }),
            new OperateDecoder(new Dictionary<int, RegDecoder> // 11
            {
                { 0x00, new RegDecoder(Mnemonic.and) },
                { 0x08, new RegDecoder(Mnemonic.bic) },
                { 0x14, new RegDecoder(Mnemonic.cmovlbs) },
                { 0x16, new RegDecoder(Mnemonic.cmovlbc) },
                { 0x20, new RegDecoder(Mnemonic.bis) },
                { 0x24, new RegDecoder(Mnemonic.cmovne) },
                { 0x26, new RegDecoder(Mnemonic.cmovne) },
                { 0x28, new RegDecoder(Mnemonic.ornot) },
                { 0x40, new RegDecoder(Mnemonic.xor) },
                { 0x44, new RegDecoder(Mnemonic.cmovlt) },
                { 0x46, new RegDecoder(Mnemonic.cmovge) },
                { 0x66, new RegDecoder(Mnemonic.cmovgt) },
                { 0x6C, new RegDecoder(Mnemonic.implver) },
            }),
            new OperateDecoder(new Dictionary<int, RegDecoder> // 12
            {
                { 0x02, new RegDecoder(Mnemonic.mskbl) },
                { 0x06, new RegDecoder(Mnemonic.extbl) },
                { 0x0B, new RegDecoder(Mnemonic.insbl) },
                { 0x12, new RegDecoder(Mnemonic.mskwl) },
                { 0x16, new RegDecoder(Mnemonic.extwl) },
                { 0x1B, new RegDecoder(Mnemonic.inswl) },
                { 0x22, new RegDecoder(Mnemonic.mskll) },
                { 0x26, new RegDecoder(Mnemonic.extll) },
                { 0x2B, new RegDecoder(Mnemonic.insll) },
                { 0x30, new RegDecoder(Mnemonic.zap) },
                { 0x31, new RegDecoder(Mnemonic.zapnot) },
                { 0x32, new RegDecoder(Mnemonic.mskql) },
                { 0x34, new RegDecoder(Mnemonic.srl) },
                { 0x36, new RegDecoder(Mnemonic.extql) },
                { 0x39, new RegDecoder(Mnemonic.sll) },
                { 0x3B, new RegDecoder(Mnemonic.insql) },
                { 0x3C, new RegDecoder(Mnemonic.src) },
                { 0x52, new RegDecoder(Mnemonic.mskwh) },
                { 0x57, new RegDecoder(Mnemonic.inswh) },
                { 0x5A, new RegDecoder(Mnemonic.extwh) },
                { 0x62, new RegDecoder(Mnemonic.msklh) },
                { 0x67, new RegDecoder(Mnemonic.inslh) },
                { 0x6A, new RegDecoder(Mnemonic.extlh) },
                { 0x72, new RegDecoder(Mnemonic.mskqh) },
                { 0x77, new RegDecoder(Mnemonic.insqh) },
                { 0x7A, new RegDecoder(Mnemonic.extqh) },
            }),
            new OperateDecoder(new Dictionary<int, RegDecoder>  // 13
            {
                { 0x00, new RegDecoder(Mnemonic.mull) },
                { 0x20, new RegDecoder(Mnemonic.mulq) },
                { 0x30, new RegDecoder(Mnemonic.umulh) },
                { 0x40, new RegDecoder(Mnemonic.mull_v) },
                { 0x60, new RegDecoder(Mnemonic.mulq_v) },
            }),

            new FOperateDecoder(new Dictionary<int, Decoder> // 14
            {
                { 0x004, new FRegDecoder(Mnemonic.itofs) },
                { 0x00A, new FRegDecoder(Mnemonic.sqrtf_c) },
                { 0x00B, new FRegDecoder(Mnemonic.sqrts_c) },
                { 0x014, new FRegDecoder(Mnemonic.itoff) },
                { 0x024, new FRegDecoder(Mnemonic.itoft) },
                { 0x02A, new FRegDecoder(Mnemonic.sqrtg_c) },
                { 0x02B, new FRegDecoder(Mnemonic.sqrtt_c) },
                { 0x04B, new FRegDecoder(Mnemonic.sqrts_m) },
                { 0x06B, new FRegDecoder(Mnemonic.sqrtt_m) },
                { 0x08A, new FRegDecoder(Mnemonic.sqrtf) },
                { 0x08B, new FRegDecoder(Mnemonic.sqrts) },
                { 0x0AA, new FRegDecoder(Mnemonic.sqrtg) },
                { 0x0AB, new FRegDecoder(Mnemonic.sqrtt) },
                { 0x0CB, new FRegDecoder(Mnemonic.sqrts_d) },
                { 0x0EB, new FRegDecoder(Mnemonic.sqrtt_d) },
                { 0x10A, new FRegDecoder(Mnemonic.sqrtf_uc) },
                { 0x10B, new FRegDecoder(Mnemonic.sqrts_uc) },
                { 0x12A, new FRegDecoder(Mnemonic.sqrtg_uc) },
                { 0x12B, new FRegDecoder(Mnemonic.sqrtt_uc) },
                { 0x14B, new FRegDecoder(Mnemonic.sqrts_um) },
                { 0x16B, new FRegDecoder(Mnemonic.sqrtt_um) },
                { 0x18A, new FRegDecoder(Mnemonic.sqrtf_u) },
                { 0x18B, new FRegDecoder(Mnemonic.sqrts_u) },
                { 0x1AA, new FRegDecoder(Mnemonic.sqrtg_u) },
                { 0x1AB, new FRegDecoder(Mnemonic.sqrtt_u) },
                { 0x1CB, new FRegDecoder(Mnemonic.sqrts_ud) },
                { 0x1EB, new FRegDecoder(Mnemonic.sqrtt_ud) },
                { 0x40A, new FRegDecoder(Mnemonic.sqrtf_sc) },
                { 0x42A, new FRegDecoder(Mnemonic.sqrtg_sc) },
                { 0x48A, new FRegDecoder(Mnemonic.sqrtf_s) },
                { 0x4AA, new FRegDecoder(Mnemonic.sqrtg_s) },
                { 0x50A, new FRegDecoder(Mnemonic.sqrtf_suc) },
                { 0x50B, new FRegDecoder(Mnemonic.sqrts_suc) },
                { 0x52A, new FRegDecoder(Mnemonic.sqrtg_suc) },
                { 0x52B, new FRegDecoder(Mnemonic.sqrtt_suc) },
                { 0x54B, new FRegDecoder(Mnemonic.sqrts_sum) },
                { 0x56B, new FRegDecoder(Mnemonic.sqrtt_sum) },
                { 0x58A, new FRegDecoder(Mnemonic.sqrtf_su) },
                { 0x58B, new FRegDecoder(Mnemonic.sqrts_su) },
                { 0x5AA, new FRegDecoder(Mnemonic.sqrtg_su) },
                { 0x5AB, new FRegDecoder(Mnemonic.sqrtt_su) },
                { 0x5CB, new FRegDecoder(Mnemonic.sqrts_sud) },
                { 0x5EB, new FRegDecoder(Mnemonic.sqrtt_sud) },
                { 0x70B, new FRegDecoder(Mnemonic.sqrts_suic) },
                { 0x72B, new FRegDecoder(Mnemonic.sqrtt_suic) },
                { 0x74B, new FRegDecoder(Mnemonic.sqrts_suim) },
                { 0x76B, new FRegDecoder(Mnemonic.sqrtt_suim) },
                { 0x78B, new FRegDecoder(Mnemonic.sqrts_sui) },
                { 0x7AB, new FRegDecoder(Mnemonic.sqrtt_sui) },
                { 0x7CB, new FRegDecoder(Mnemonic.sqrts_suid) },
                { 0x7EB, new FRegDecoder(Mnemonic.sqrtt_suid) },

            }),
            new FOperateDecoder(new Dictionary<int, Decoder> // 15
            {
                { 0x000, new FRegDecoder(Mnemonic.addf_c) },
                { 0x001, new FRegDecoder(Mnemonic.subf_c) },
                { 0x002, new FRegDecoder(Mnemonic.mulf_c) },
                { 0x003, new FRegDecoder(Mnemonic.divf_c) },
                { 0x01E, new CvtDecoder(Mnemonic.cvtdg_c) },
                { 0x020, new FRegDecoder(Mnemonic.addg_c) },
                { 0x021, new FRegDecoder(Mnemonic.subg_c) },
                { 0x022, new FRegDecoder(Mnemonic.mulg_c) },
                { 0x023, new FRegDecoder(Mnemonic.divg_c) },
                { 0x02C, new CvtDecoder(Mnemonic.cvtgf_c) },
                { 0x02D, new CvtDecoder(Mnemonic.cvtgd_c) },
                { 0x02F, new CvtDecoder(Mnemonic.cvtgq_c) },
                { 0x03C, new CvtDecoder(Mnemonic.cvtqf_c) },
                { 0x03E, new CvtDecoder(Mnemonic.cvtqg_c) },
                { 0x080, new FRegDecoder(Mnemonic.addf) },
                //{ 0x081, new FRegDecoder(Mnemonic.negf) },	_* /*pse*/udo */
                { 0x081, new FRegDecoder(Mnemonic.subf) },
                { 0x082, new FRegDecoder(Mnemonic.mulf) },
                { 0x083, new FRegDecoder(Mnemonic.divf) },
                { 0x09E, new CvtDecoder(Mnemonic.cvtdg) },
                { 0x0A0, new FRegDecoder(Mnemonic.addg) },
                //{ 0x0A1, new FRegDecoder(Mnemonic.negg) },	_* pseudo */
                { 0x0A1, new FRegDecoder(Mnemonic.subg) },
                { 0x0A2, new FRegDecoder(Mnemonic.mulg) },
                { 0x0A3, new FRegDecoder(Mnemonic.divg) },
                { 0x0A5, new FRegDecoder(Mnemonic.cmpgeq) },
                { 0x0A6, new FRegDecoder(Mnemonic.cmpglt) },
                { 0x0A7, new FRegDecoder(Mnemonic.cmpgle) },
                { 0x0AC, new CvtDecoder(Mnemonic.cvtgf) },
                { 0x0AD, new CvtDecoder(Mnemonic.cvtgd) },
                { 0x0AF, new CvtDecoder(Mnemonic.cvtgq) },
                { 0x0BC, new CvtDecoder(Mnemonic.cvtqf) },
                { 0x0BE, new CvtDecoder(Mnemonic.cvtqg) },
                { 0x100, new FRegDecoder(Mnemonic.addf_uc) },
                { 0x101, new FRegDecoder(Mnemonic.subf_uc) },
                { 0x102, new FRegDecoder(Mnemonic.mulf_uc) },
                { 0x103, new FRegDecoder(Mnemonic.divf_uc) },
                { 0x11E, new CvtDecoder(Mnemonic.cvtdg_uc) },
                { 0x120, new FRegDecoder(Mnemonic.addg_uc) },
                { 0x121, new FRegDecoder(Mnemonic.subg_uc) },
                { 0x122, new FRegDecoder(Mnemonic.mulg_uc) },
                { 0x123, new FRegDecoder(Mnemonic.divg_uc) },
                { 0x12C, new CvtDecoder(Mnemonic.cvtgf_uc) },
                { 0x12D, new CvtDecoder(Mnemonic.cvtgd_uc) },
                { 0x12F, new CvtDecoder(Mnemonic.cvtgq_vc) },
                { 0x180, new FRegDecoder(Mnemonic.addf_u) },
                { 0x181, new FRegDecoder(Mnemonic.subf_u) },
                { 0x182, new FRegDecoder(Mnemonic.mulf_u) },
                { 0x183, new FRegDecoder(Mnemonic.divf_u) },
                { 0x19E, new CvtDecoder(Mnemonic.cvtdg_u) },
                { 0x1A0, new FRegDecoder(Mnemonic.addg_u) },
                { 0x1A1, new FRegDecoder(Mnemonic.subg_u) },
                { 0x1A2, new FRegDecoder(Mnemonic.mulg_u) },
                { 0x1A3, new FRegDecoder(Mnemonic.divg_u) },
                { 0x1AC, new CvtDecoder(Mnemonic.cvtgf_u) },
                { 0x1AD, new CvtDecoder(Mnemonic.cvtgd_u) },
                { 0x1AF, new CvtDecoder(Mnemonic.cvtgq_v) },
                { 0x400, new FRegDecoder(Mnemonic.addf_sc) },
                { 0x401, new FRegDecoder(Mnemonic.subf_sc) },
                { 0x402, new FRegDecoder(Mnemonic.mulf_sc) },
                { 0x403, new FRegDecoder(Mnemonic.divf_sc) },
                { 0x41E, new CvtDecoder(Mnemonic.cvtdg_sc) },
                { 0x420, new FRegDecoder(Mnemonic.addg_sc) },
                { 0x421, new FRegDecoder(Mnemonic.subg_sc) },
                { 0x422, new FRegDecoder(Mnemonic.mulg_sc) },
                { 0x423, new FRegDecoder(Mnemonic.divg_sc) },
                { 0x42C, new CvtDecoder(Mnemonic.cvtgf_sc) },
                { 0x42D, new CvtDecoder(Mnemonic.cvtgd_sc) },
                { 0x42F, new CvtDecoder(Mnemonic.cvtgq_sc) },
                { 0x480, new FRegDecoder(Mnemonic.addf_s) },
                //{ 0x481, new FRegDecoder(Mnemonic.negf_s) },	_* pseudo */
                { 0x481, new FRegDecoder(Mnemonic.subf_s) },
                { 0x482, new FRegDecoder(Mnemonic.mulf_s) },
                { 0x483, new FRegDecoder(Mnemonic.divf_s) },
                { 0x49E, new CvtDecoder(Mnemonic.cvtdg_s) },
                { 0x4A0, new FRegDecoder(Mnemonic.addg_s) },
                //{ 0x4A1, new FRegDecoder(Mnemonic.negg_s) },	_* pseudo */
                { 0x4A1, new FRegDecoder(Mnemonic.subg_s) },
                { 0x4A2, new FRegDecoder(Mnemonic.mulg_s) },
                { 0x4A3, new FRegDecoder(Mnemonic.divg_s) },
                { 0x4A5, new FRegDecoder(Mnemonic.cmpgeq_s) },
                { 0x4A6, new FRegDecoder(Mnemonic.cmpglt_s) },
                { 0x4A7, new FRegDecoder(Mnemonic.cmpgle_s) },
                { 0x4AC, new CvtDecoder(Mnemonic.cvtgf_s) },
                { 0x4AD, new CvtDecoder(Mnemonic.cvtgd_s) },
                { 0x4AF, new CvtDecoder(Mnemonic.cvtgq_s) },
                { 0x500, new FRegDecoder(Mnemonic.addf_suc) },
                { 0x501, new FRegDecoder(Mnemonic.subf_suc) },
                { 0x502, new FRegDecoder(Mnemonic.mulf_suc) },
                { 0x503, new FRegDecoder(Mnemonic.divf_suc) },
                { 0x51E, new CvtDecoder(Mnemonic.cvtdg_suc) },
                { 0x520, new FRegDecoder(Mnemonic.addg_suc) },
                { 0x521, new FRegDecoder(Mnemonic.subg_suc) },
                { 0x522, new FRegDecoder(Mnemonic.mulg_suc) },
                { 0x523, new FRegDecoder(Mnemonic.divg_suc) },
                { 0x52C, new CvtDecoder(Mnemonic.cvtgf_suc) },
                { 0x52D, new CvtDecoder(Mnemonic.cvtgd_suc) },
                { 0x52F, new CvtDecoder(Mnemonic.cvtgq_svc) },
                { 0x580, new FRegDecoder(Mnemonic.addf_su) },
                { 0x581, new FRegDecoder(Mnemonic.subf_su) },
                { 0x582, new FRegDecoder(Mnemonic.mulf_su) },
                { 0x583, new FRegDecoder(Mnemonic.divf_su) },
                { 0x59E, new CvtDecoder(Mnemonic.cvtdg_su) },
                { 0x5A0, new FRegDecoder(Mnemonic.addg_su) },
                { 0x5A1, new FRegDecoder(Mnemonic.subg_su) },
                { 0x5A2, new FRegDecoder(Mnemonic.mulg_su) },
                { 0x5A3, new FRegDecoder(Mnemonic.divg_su) },
                { 0x5AC, new CvtDecoder(Mnemonic.cvtgf_su) },
                { 0x5AD, new CvtDecoder(Mnemonic.cvtgd_su) },
                { 0x5AF, new CvtDecoder(Mnemonic.cvtgq_sv) },

            }),
            new FOperateDecoder(new Dictionary<int, Decoder> // 16
            {
                { 0x000, new FRegDecoder(Mnemonic.adds_c) },
                { 0x001, new FRegDecoder(Mnemonic.subs_c) },
                { 0x002, new FRegDecoder(Mnemonic.muls_c) },
                { 0x003, new FRegDecoder(Mnemonic.divs_c) },
                { 0x020, new FRegDecoder(Mnemonic.addt_c) },
                { 0x021, new FRegDecoder(Mnemonic.subt_c) },
                { 0x022, new FRegDecoder(Mnemonic.mult_c) },
                { 0x023, new FRegDecoder(Mnemonic.divt_c) },
                { 0x02C, new CvtDecoder(Mnemonic.cvtts_c) },
                { 0x02F, new CvtDecoder(Mnemonic.cvttq_c) },
                { 0x03C, new CvtDecoder(Mnemonic.cvtqs_c) },
                { 0x03E, new CvtDecoder(Mnemonic.cvtqt_c) },
                { 0x040, new FRegDecoder(Mnemonic.adds_m) },
                { 0x041, new FRegDecoder(Mnemonic.subs_m) },
                { 0x042, new FRegDecoder(Mnemonic.muls_m) },
                { 0x043, new FRegDecoder(Mnemonic.divs_m) },
                { 0x060, new FRegDecoder(Mnemonic.addt_m) },
                { 0x061, new FRegDecoder(Mnemonic.subt_m) },
                { 0x062, new FRegDecoder(Mnemonic.mult_m) },
                { 0x063, new FRegDecoder(Mnemonic.divt_m) },
                { 0x06C, new CvtDecoder(Mnemonic.cvtts_m) },
                { 0x06F, new CvtDecoder(Mnemonic.cvttq_m) },
                { 0x07C, new CvtDecoder(Mnemonic.cvtqs_m) },
                { 0x07E, new CvtDecoder(Mnemonic.cvtqt_m) },
                { 0x080, new FRegDecoder(Mnemonic.adds) },
                //{ 0x081, new FRegDecoder(Mnemonic.negs) },	_* pseudo */
                { 0x081, new FRegDecoder(Mnemonic.subs) },
                { 0x082, new FRegDecoder(Mnemonic.muls) },
                { 0x083, new FRegDecoder(Mnemonic.divs) },
                { 0x0A0, new FRegDecoder(Mnemonic.addt) },
                //{ 0x0A1, new FRegDecoder(Mnemonic.negt) },	_* pseudo */
                { 0x0A1, new FRegDecoder(Mnemonic.subt) },
                { 0x0A2, new FRegDecoder(Mnemonic.mult) },
                { 0x0A3, new FRegDecoder(Mnemonic.divt) },
                { 0x0A4, new FRegDecoder(Mnemonic.cmptun) },
                { 0x0A5, new FRegDecoder(Mnemonic.cmpteq) },
                { 0x0A6, new FRegDecoder(Mnemonic.cmptlt) },
                { 0x0A7, new FRegDecoder(Mnemonic.cmptle) },
                { 0x0AC, new CvtDecoder(Mnemonic.cvtts) },
                { 0x0AF, new CvtDecoder(Mnemonic.cvttq) },
                { 0x0BC, new CvtDecoder(Mnemonic.cvtqs) },
                { 0x0BE, new CvtDecoder(Mnemonic.cvtqt) },
                { 0x0C0, new FRegDecoder(Mnemonic.adds_d) },
                { 0x0C1, new FRegDecoder(Mnemonic.subs_d) },
                { 0x0C2, new FRegDecoder(Mnemonic.muls_d) },
                { 0x0C3, new FRegDecoder(Mnemonic.divs_d) },
                { 0x0E0, new FRegDecoder(Mnemonic.addt_d) },
                { 0x0E1, new FRegDecoder(Mnemonic.subt_d) },
                { 0x0E2, new FRegDecoder(Mnemonic.mult_d) },
                { 0x0E3, new FRegDecoder(Mnemonic.divt_d) },
                { 0x0EC, new CvtDecoder(Mnemonic.cvtts_d) },
                { 0x0EF, new CvtDecoder(Mnemonic.cvttq_d) },
                { 0x0FC, new CvtDecoder(Mnemonic.cvtqs_d) },
                { 0x0FE, new CvtDecoder(Mnemonic.cvtqt_d) },
                { 0x100, new FRegDecoder(Mnemonic.adds_uc) },
                { 0x101, new FRegDecoder(Mnemonic.subs_uc) },
                { 0x102, new FRegDecoder(Mnemonic.muls_uc) },
                { 0x103, new FRegDecoder(Mnemonic.divs_uc) },
                { 0x120, new FRegDecoder(Mnemonic.addt_uc) },
                { 0x121, new FRegDecoder(Mnemonic.subt_uc) },
                { 0x122, new FRegDecoder(Mnemonic.mult_uc) },
                { 0x123, new FRegDecoder(Mnemonic.divt_uc) },
                { 0x12C, new CvtDecoder(Mnemonic.cvtts_uc) },
                { 0x12F, new CvtDecoder(Mnemonic.cvttq_vc) },
                { 0x140, new FRegDecoder(Mnemonic.adds_um) },
                { 0x141, new FRegDecoder(Mnemonic.subs_um) },
                { 0x142, new FRegDecoder(Mnemonic.muls_um) },
                { 0x143, new FRegDecoder(Mnemonic.divs_um) },
                { 0x160, new FRegDecoder(Mnemonic.addt_um) },
                { 0x161, new FRegDecoder(Mnemonic.subt_um) },
                { 0x162, new FRegDecoder(Mnemonic.mult_um) },
                { 0x163, new FRegDecoder(Mnemonic.divt_um) },
                { 0x16C, new CvtDecoder(Mnemonic.cvtts_um) },
                { 0x16F, new CvtDecoder(Mnemonic.cvttq_vm) },
                { 0x180, new FRegDecoder(Mnemonic.adds_u) },
                { 0x181, new FRegDecoder(Mnemonic.subs_u) },
                { 0x182, new FRegDecoder(Mnemonic.muls_u) },
                { 0x183, new FRegDecoder(Mnemonic.divs_u) },
                { 0x1A0, new FRegDecoder(Mnemonic.addt_u) },
                { 0x1A1, new FRegDecoder(Mnemonic.subt_u) },
                { 0x1A2, new FRegDecoder(Mnemonic.mult_u) },
                { 0x1A3, new FRegDecoder(Mnemonic.divt_u) },
                { 0x1AC, new CvtDecoder(Mnemonic.cvtts_u) },
                { 0x1AF, new CvtDecoder(Mnemonic.cvttq_v) },
                { 0x1C0, new FRegDecoder(Mnemonic.adds_ud) },
                { 0x1C1, new FRegDecoder(Mnemonic.subs_ud) },
                { 0x1C2, new FRegDecoder(Mnemonic.muls_ud) },
                { 0x1C3, new FRegDecoder(Mnemonic.divs_ud) },
                { 0x1E0, new FRegDecoder(Mnemonic.addt_ud) },
                { 0x1E1, new FRegDecoder(Mnemonic.subt_ud) },
                { 0x1E2, new FRegDecoder(Mnemonic.mult_ud) },
                { 0x1E3, new FRegDecoder(Mnemonic.divt_ud) },
                { 0x1EC, new CvtDecoder(Mnemonic.cvtts_ud) },
                { 0x1EF, new CvtDecoder(Mnemonic.cvttq_vd) },
                { 0x2AC, new CvtDecoder(Mnemonic.cvtst) },
                { 0x500, new FRegDecoder(Mnemonic.adds_suc) },
                { 0x501, new FRegDecoder(Mnemonic.subs_suc) },
                { 0x502, new FRegDecoder(Mnemonic.muls_suc) },
                { 0x503, new FRegDecoder(Mnemonic.divs_suc) },
                { 0x520, new FRegDecoder(Mnemonic.addt_suc) },
                { 0x521, new FRegDecoder(Mnemonic.subt_suc) },
                { 0x522, new FRegDecoder(Mnemonic.mult_suc) },
                { 0x523, new FRegDecoder(Mnemonic.divt_suc) },
                { 0x52C, new CvtDecoder(Mnemonic.cvtts_suc) },
                { 0x52F, new CvtDecoder(Mnemonic.cvttq_svc) },
                { 0x540, new FRegDecoder(Mnemonic.adds_sum) },
                { 0x541, new FRegDecoder(Mnemonic.subs_sum) },
                { 0x542, new FRegDecoder(Mnemonic.muls_sum) },
                { 0x543, new FRegDecoder(Mnemonic.divs_sum) },
                { 0x560, new FRegDecoder(Mnemonic.addt_sum) },
                { 0x561, new FRegDecoder(Mnemonic.subt_sum) },
                { 0x562, new FRegDecoder(Mnemonic.mult_sum) },
                { 0x563, new FRegDecoder(Mnemonic.divt_sum) },
                { 0x56C, new CvtDecoder(Mnemonic.cvtts_sum) },
                { 0x56F, new CvtDecoder(Mnemonic.cvttq_svm) },
                { 0x580, new FRegDecoder(Mnemonic.adds_su) },
                //{ 0x581, new FRegDecoder(Mnemonic.negs_su) },	_* pseudo */
                { 0x581, new FRegDecoder(Mnemonic.subs_su) },
                { 0x582, new FRegDecoder(Mnemonic.muls_su) },
                { 0x583, new FRegDecoder(Mnemonic.divs_su) },
                { 0x5A0, new FRegDecoder(Mnemonic.addt_su) },
                //{ 0x5A1, new FRegDecoder(Mnemonic.negt_su) },	_* pseudo */
                { 0x5A1, new FRegDecoder(Mnemonic.subt_su) },
                { 0x5A2, new FRegDecoder(Mnemonic.mult_su) },
                { 0x5A3, new FRegDecoder(Mnemonic.divt_su) },
                { 0x5A4, new FRegDecoder(Mnemonic.cmptun_su) },
                { 0x5A5, new FRegDecoder(Mnemonic.cmpteq_su) },
                { 0x5A6, new FRegDecoder(Mnemonic.cmptlt_su) },
                { 0x5A7, new FRegDecoder(Mnemonic.cmptle_su) },
                { 0x5AC, new CvtDecoder(Mnemonic.cvtts_su) },
                { 0x5AF, new CvtDecoder(Mnemonic.cvttq_sv) },
                { 0x5C0, new FRegDecoder(Mnemonic.adds_sud) },
                { 0x5C1, new FRegDecoder(Mnemonic.subs_sud) },
                { 0x5C2, new FRegDecoder(Mnemonic.muls_sud) },
                { 0x5C3, new FRegDecoder(Mnemonic.divs_sud) },
                { 0x5E0, new FRegDecoder(Mnemonic.addt_sud) },
                { 0x5E1, new FRegDecoder(Mnemonic.subt_sud) },
                { 0x5E2, new FRegDecoder(Mnemonic.mult_sud) },
                { 0x5E3, new FRegDecoder(Mnemonic.divt_sud) },
                { 0x5EC, new CvtDecoder(Mnemonic.cvtts_sud) },
                { 0x5EF, new CvtDecoder(Mnemonic.cvttq_svd) },
                { 0x6AC, new CvtDecoder(Mnemonic.cvtst_s) },
                { 0x700, new FRegDecoder(Mnemonic.adds_suic) },
                { 0x701, new FRegDecoder(Mnemonic.subs_suic) },
                { 0x702, new FRegDecoder(Mnemonic.muls_suic) },
                { 0x703, new FRegDecoder(Mnemonic.divs_suic) },
                { 0x720, new FRegDecoder(Mnemonic.addt_suic) },
                { 0x721, new FRegDecoder(Mnemonic.subt_suic) },
                { 0x722, new FRegDecoder(Mnemonic.mult_suic) },
                { 0x723, new FRegDecoder(Mnemonic.divt_suic) },
                { 0x72C, new CvtDecoder(Mnemonic.cvtts_suic) },
                { 0x72F, new CvtDecoder(Mnemonic.cvttq_svic) },
                { 0x73C, new CvtDecoder(Mnemonic.cvtqs_suic) },
                { 0x73E, new CvtDecoder(Mnemonic.cvtqt_suic) },
                { 0x740, new FRegDecoder(Mnemonic.adds_suim) },
                { 0x741, new FRegDecoder(Mnemonic.subs_suim) },
                { 0x742, new FRegDecoder(Mnemonic.muls_suim) },
                { 0x743, new FRegDecoder(Mnemonic.divs_suim) },
                { 0x760, new FRegDecoder(Mnemonic.addt_suim) },
                { 0x761, new FRegDecoder(Mnemonic.subt_suim) },
                { 0x762, new FRegDecoder(Mnemonic.mult_suim) },
                { 0x763, new FRegDecoder(Mnemonic.divt_suim) },
                { 0x76C, new CvtDecoder(Mnemonic.cvtts_suim) },
                { 0x76F, new CvtDecoder(Mnemonic.cvttq_svim) },
                { 0x77C, new CvtDecoder(Mnemonic.cvtqs_suim) },
                { 0x77E, new CvtDecoder(Mnemonic.cvtqt_suim) },
                { 0x780, new FRegDecoder(Mnemonic.adds_sui) },
                //{ 0x781, new FRegDecoder(Mnemonic.negs_sui) },	_* pseudo */
                { 0x781, new FRegDecoder(Mnemonic.subs_sui) },
                { 0x782, new FRegDecoder(Mnemonic.muls_sui) },
                { 0x783, new FRegDecoder(Mnemonic.divs_sui) },
                { 0x7A0, new FRegDecoder(Mnemonic.addt_sui) },
                //{ 0x7A1, new FRegDecoder(Mnemonic.negt_sui) },	_* pseudo */
                { 0x7A1, new FRegDecoder(Mnemonic.subt_sui) },
                { 0x7A2, new FRegDecoder(Mnemonic.mult_sui) },
                { 0x7A3, new FRegDecoder(Mnemonic.divt_sui) },
                { 0x7AC, new CvtDecoder(Mnemonic.cvtts_sui) },
                { 0x7AF, new CvtDecoder(Mnemonic.cvttq_svi) },
                { 0x7BC, new CvtDecoder(Mnemonic.cvtqs_sui) },
                { 0x7BE, new CvtDecoder(Mnemonic.cvtqt_sui) },
                { 0x7C0, new FRegDecoder(Mnemonic.adds_suid) },
                { 0x7C1, new FRegDecoder(Mnemonic.subs_suid) },
                { 0x7C2, new FRegDecoder(Mnemonic.muls_suid) },
                { 0x7C3, new FRegDecoder(Mnemonic.divs_suid) },
                { 0x7E0, new FRegDecoder(Mnemonic.addt_suid) },
                { 0x7E1, new FRegDecoder(Mnemonic.subt_suid) },
                { 0x7E2, new FRegDecoder(Mnemonic.mult_suid) },
                { 0x7E3, new FRegDecoder(Mnemonic.divt_suid) },
                { 0x7EC, new CvtDecoder(Mnemonic.cvtts_suid) },
                { 0x7EF, new CvtDecoder(Mnemonic.cvttq_svid) },
                { 0x7FC, new CvtDecoder(Mnemonic.cvtqs_suid) },
                { 0x7FE, new CvtDecoder(Mnemonic.cvtqt_suid) },
            }),
            new FOperateDecoder(new Dictionary<int, Decoder>
            {
                  { 0x010, new CvtDecoder(Mnemonic.cvtlq) },
                  //{ 0x020, new FRegDecoder(Mnemonic.fnop) },	_* pseudo */
                  //{ 0x020, new FRegDecoder(Mnemonic.fclr) },	_* pseudo */
                  //{ 0x020, new FRegDecoder(Mnemonic.fabs) },	_* pseudo */
                  //{ 0x020, new FRegDecoder(Mnemonic.fmov) }, _* pseudo */
                  { 0x020, new FRegDecoder(Mnemonic.cpys) },
                  //{ 0x021, new FRegDecoder(Mnemonic.fneg) }, _* pseudo */
                  { 0x021, new FRegDecoder(Mnemonic.cpysn) },
                  { 0x022, new FRegDecoder(Mnemonic.cpyse) },
                  { 0x024, new FRegDecoder(Mnemonic.mt_fpcr) },
                  { 0x025, new FRegDecoder(Mnemonic.mf_fpcr) },
                  { 0x02A, new FRegDecoder(Mnemonic.fcmoveq) },
                  { 0x02B, new FRegDecoder(Mnemonic.fcmovne) },
                  { 0x02C, new FRegDecoder(Mnemonic.fcmovlt) },
                  { 0x02D, new FRegDecoder(Mnemonic.fcmovge) },
                  { 0x02E, new FRegDecoder(Mnemonic.fcmovle) },
                  { 0x02F, new FRegDecoder(Mnemonic.fcmovgt) },
                  { 0x030, new CvtDecoder(Mnemonic.cvtql) },
                  { 0x130, new CvtDecoder(Mnemonic.cvtql_v) },
                  { 0x530, new CvtDecoder(Mnemonic.cvtql_sv) },
            }),

            new PalDecoder(new Dictionary<uint, (Mnemonic,InstrClass)> // 18
            {
                { 0x0000, (Mnemonic.trapb,InstrClass.Transfer|InstrClass.Call) }
            }),
            new PalDecoder(new Dictionary<uint, (Mnemonic,InstrClass)> // 19 
            {
            }),
            new JMemDecoder(),
            new PalDecoder(new Dictionary<uint, (Mnemonic,InstrClass)> // 1B 
            {
            }),

            new NyiDecoder(),
            new NyiDecoder(),
            new PalDecoder(new Dictionary<uint, (Mnemonic,InstrClass)>
            {       //$REVIEW: these palcodes are OS-dependent....
            }),
            new NyiDecoder(),
            // 20
            new FMemDecoder(Mnemonic.ldf),
            new FMemDecoder(Mnemonic.ldg),
            new FMemDecoder(Mnemonic.lds),
            new FMemDecoder(Mnemonic.ldt),

            new FMemDecoder(Mnemonic.stf),
            new FMemDecoder(Mnemonic.stg),
            new FMemDecoder(Mnemonic.sts),
            new FMemDecoder(Mnemonic.stt),

            new MemDecoder(Mnemonic.ldl),
            new MemDecoder(Mnemonic.ldq),
            new MemDecoder(Mnemonic.ldl_l),
            new MemDecoder(Mnemonic.ldq_l),

            new MemDecoder(Mnemonic.stl),
            new MemDecoder(Mnemonic.stq),
            new MemDecoder(Mnemonic.stl_c),
            new MemDecoder(Mnemonic.stq_c),
            // 30
            new BranchDecoder(Mnemonic.br),
            new FBranchDecoder(Mnemonic.fbeq),
            new FBranchDecoder(Mnemonic.fblt),
            new FBranchDecoder(Mnemonic.fble),

            new BranchDecoder(Mnemonic.bsr),
            new FBranchDecoder(Mnemonic.fbne),
            new FBranchDecoder(Mnemonic.fbge),
            new FBranchDecoder(Mnemonic.fbgt),

            new BranchDecoder(Mnemonic.blbc),
            new BranchDecoder(Mnemonic.beq),
            new BranchDecoder(Mnemonic.blt),
            new BranchDecoder(Mnemonic.ble),

            new BranchDecoder(Mnemonic.blbs),
            new BranchDecoder(Mnemonic.bne),
            new BranchDecoder(Mnemonic.bge),
            new BranchDecoder(Mnemonic.bgt),
        };
    }
}