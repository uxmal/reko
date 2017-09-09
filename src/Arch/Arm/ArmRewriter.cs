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

using Gee.External.Capstone;
using Gee.External.Capstone.Arm;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Reko.Arch.Arm
{
    using CapstoneArmInstruction = Instruction<ArmInstruction, ArmRegister, ArmInstructionGroup, ArmInstructionDetail>;
    using Opcode = Gee.External.Capstone.Arm.ArmInstruction;

    public partial class ArmRewriter : IEnumerable<RtlInstructionCluster>
    {
        private Arm32ProcessorArchitecture arch;
        private IEnumerator<Arm32Instruction> instrs;
        private IStorageBinder frame;
        private CapstoneArmInstruction instr;
        private ArmInstructionOperand [] ops;
        private RtlClass rtlc;
        private List<RtlInstruction> rtlInstructions;
        private RtlEmitter m;
        private IRewriterHost host;

        public ArmRewriter(Arm32ProcessorArchitecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder frame, IRewriterHost host)
        {
            this.arch = arch;
            this.instrs = CreateInstructionStream(rdr);
            this.frame = frame;
            this.host = host;
        }

        public ArmInstructionOperand Dst { get { return ops[0]; } }
        public ArmInstructionOperand Src1 { get { return ops[1]; } }
        public ArmInstructionOperand Src2 { get { return ops[2]; } }
        public ArmInstructionOperand Src3 { get { return ops[3]; } }

        private IEnumerator<Arm32Instruction> CreateInstructionStream(EndianImageReader rdr)
        {
            return new Arm32Disassembler(arch, rdr).GetEnumerator();
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (instrs.MoveNext())
            {
                if (!instrs.Current.TryGetInternal(out this.instr))
                {
                    throw new AddressCorrelatedException(
                       instrs.Current.Address,
                       "Invalid opcode cannot be rewritten to IR.");
                }
                this.ops = instr.ArchitectureDetail.Operands;

                this.rtlInstructions = new List<RtlInstruction>();
                this.rtlc = RtlClass.Linear;
                this.m = new RtlEmitter(rtlInstructions);
                switch (instr.Id)
                {
                default:

                case Opcode.ADR:
                case Opcode.AESD:
                case Opcode.AESE:
                case Opcode.AESIMC:
                case Opcode.AESMC:
                case Opcode.BKPT:
                case Opcode.BXJ:
                case Opcode.CDP:
                case Opcode.CDP2:
                case Opcode.CLREX:
                case Opcode.CRC32B:
                case Opcode.CRC32CB:
                case Opcode.CRC32CH:
                case Opcode.CRC32CW:
                case Opcode.CRC32H:
                case Opcode.CRC32W:
                case Opcode.DBG:
                case Opcode.DSB:
                case Opcode.FLDMDBX:
                case Opcode.FLDMIAX:
                case Opcode.VMRS:
                case Opcode.FSTMDBX:
                case Opcode.FSTMIAX:
                case Opcode.HINT:
                case Opcode.HLT:
                case Opcode.ISB:
                case Opcode.LDA:
                case Opcode.LDAB:
                case Opcode.LDAEX:
                case Opcode.LDAEXB:
                case Opcode.LDAEXD:
                case Opcode.LDAEXH:
                case Opcode.LDAH:
                case Opcode.LDC2L:
                case Opcode.LDC2:
                case Opcode.LDCL:
                case Opcode.LDC:
                case Opcode.LDMDA:
                case Opcode.LDRBT:
                case Opcode.LDREX:
                case Opcode.LDREXB:
                case Opcode.LDREXD:
                case Opcode.LDREXH:
                case Opcode.LDRHT:
                case Opcode.LDRSBT:
                case Opcode.LDRSHT:
                case Opcode.LDRT:
                case Opcode.MCR2:
                case Opcode.MCRR:
                case Opcode.MCRR2:
                case Opcode.MRC2:
                case Opcode.MRRC:
                case Opcode.MRRC2:
                case Opcode.PKHBT:
                case Opcode.PKHTB:
                case Opcode.PLDW:
                case Opcode.PLD:
                case Opcode.PLI:
                case Opcode.QADD:
                case Opcode.QADD16:
                case Opcode.QADD8:
                case Opcode.QASX:
                case Opcode.QDADD:
                case Opcode.QDSUB:
                case Opcode.QSAX:
                case Opcode.QSUB:
                case Opcode.QSUB16:
                case Opcode.QSUB8:
                case Opcode.RBIT:
                case Opcode.REV16:
                case Opcode.REVSH:
                case Opcode.RFEDA:
                case Opcode.RFEDB:
                case Opcode.RFEIA:
                case Opcode.RFEIB:
                case Opcode.RSC:
                case Opcode.SADD16:
                case Opcode.SADD8:
                case Opcode.SASX:
                case Opcode.SDIV:
                case Opcode.SEL:
                case Opcode.SETEND:
                case Opcode.SHA1C:
                case Opcode.SHA1H:
                case Opcode.SHA1M:
                case Opcode.SHA1P:
                case Opcode.SHA1SU0:
                case Opcode.SHA1SU1:
                case Opcode.SHA256H:
                case Opcode.SHA256H2:
                case Opcode.SHA256SU0:
                case Opcode.SHA256SU1:
                case Opcode.SHADD16:
                case Opcode.SHADD8:
                case Opcode.SHASX:
                case Opcode.SHSAX:
                case Opcode.SHSUB16:
                case Opcode.SHSUB8:
                case Opcode.SMC:
                case Opcode.SMLABB:
                case Opcode.SMLABT:
                case Opcode.SMLAD:
                case Opcode.SMLADX:
                case Opcode.SMLAL:
                case Opcode.SMLALBB:
                case Opcode.SMLALBT:
                case Opcode.SMLALD:
                case Opcode.SMLALDX:
                case Opcode.SMLALTB:
                case Opcode.SMLALTT:
                case Opcode.SMLATB:
                case Opcode.SMLATT:
                case Opcode.SMLAWB:
                case Opcode.SMLAWT:
                case Opcode.SMLSD:
                case Opcode.SMLSDX:
                case Opcode.SMLSLD:
                case Opcode.SMLSLDX:
                case Opcode.SMMLA:
                case Opcode.SMMLAR:
                case Opcode.SMMLS:
                case Opcode.SMMLSR:
                case Opcode.SMMUL:
                case Opcode.SMMULR:
                case Opcode.SMUAD:
                case Opcode.SMUADX:
                case Opcode.SMULBT:
                case Opcode.SMULTB:
                case Opcode.SMULTT:
                case Opcode.SMULWB:
                case Opcode.SMULWT:
                case Opcode.SMUSD:
                case Opcode.SMUSDX:
                case Opcode.SRSDA:
                case Opcode.SRSDB:
                case Opcode.SRSIA:
                case Opcode.SRSIB:
                case Opcode.SSAT:
                case Opcode.SSAT16:
                case Opcode.SSAX:
                case Opcode.SSUB16:
                case Opcode.SSUB8:
                case Opcode.STC2L:
                case Opcode.STC2:
                case Opcode.STCL:
                case Opcode.STC:
                case Opcode.STL:
                case Opcode.STLB:
                case Opcode.STLEX:
                case Opcode.STLEXB:
                case Opcode.STLEXD:
                case Opcode.STLEXH:
                case Opcode.STLH:
                case Opcode.STMDA:
                case Opcode.STRBT:
                case Opcode.STREX:
                case Opcode.STREXB:
                case Opcode.STREXD:
                case Opcode.STREXH:
                case Opcode.STRHT:
                case Opcode.STRT:
                case Opcode.SWP:
                case Opcode.SWPB:
                case Opcode.SXTAB16:
                case Opcode.SXTB16:
                case Opcode.TRAP:
                case Opcode.UADD16:
                case Opcode.UADD8:
                case Opcode.UASX:
                case Opcode.UDF:
                case Opcode.UDIV:
                case Opcode.UHADD16:
                case Opcode.UHADD8:
                case Opcode.UHASX:
                case Opcode.UHSAX:
                case Opcode.UHSUB16:
                case Opcode.UHSUB8:
                case Opcode.UMAAL:
                case Opcode.UQADD16:
                case Opcode.UQADD8:
                case Opcode.UQASX:
                case Opcode.UQSAX:
                case Opcode.UQSUB16:
                case Opcode.UQSUB8:
                case Opcode.USAD8:
                case Opcode.USADA8:
                case Opcode.USAT:
                case Opcode.USAT16:
                case Opcode.USAX:
                case Opcode.USUB16:
                case Opcode.USUB8:
                case Opcode.UXTAB16:
                case Opcode.UXTB16:
                case Opcode.VABAL:
                case Opcode.VABA:
                case Opcode.VABDL:
                case Opcode.VABD:
                case Opcode.VABS:
                case Opcode.VACGE:
                case Opcode.VACGT:
                case Opcode.VADD:
                case Opcode.VADDHN:
                case Opcode.VADDL:
                case Opcode.VADDW:
                case Opcode.VAND:
                case Opcode.VBIC:
                case Opcode.VBIF:
                case Opcode.VBIT:
                case Opcode.VBSL:
                case Opcode.VCEQ:
                case Opcode.VCGE:
                case Opcode.VCGT:
                case Opcode.VCLE:
                case Opcode.VCLS:
                case Opcode.VCLT:
                case Opcode.VCLZ:
                case Opcode.VCMP:
                case Opcode.VCMPE:
                case Opcode.VCNT:
                case Opcode.VCVTA:
                case Opcode.VCVTB:
                case Opcode.VCVT:
                case Opcode.VCVTM:
                case Opcode.VCVTN:
                case Opcode.VCVTP:
                case Opcode.VCVTT:
                case Opcode.VDIV:
                case Opcode.VDUP:
                case Opcode.VEOR:
                case Opcode.VEXT:
                case Opcode.VFMA:
                case Opcode.VFMS:
                case Opcode.VFNMA:
                case Opcode.VFNMS:
                case Opcode.VHADD:
                case Opcode.VHSUB:
                case Opcode.VLD1:
                case Opcode.VLD2:
                case Opcode.VLD3:
                case Opcode.VLD4:
                case Opcode.VLDMDB:
                case Opcode.VLDR:
                case Opcode.VMAXNM:
                case Opcode.VMAX:
                case Opcode.VMINNM:
                case Opcode.VMIN:
                case Opcode.VMLA:
                case Opcode.VMLAL:
                case Opcode.VMLS:
                case Opcode.VMLSL:
                case Opcode.VMOVL:
                case Opcode.VMOVN:
                case Opcode.VMSR:
                case Opcode.VMUL:
                case Opcode.VMULL:
                case Opcode.VMVN:
                case Opcode.VNEG:
                case Opcode.VNMLA:
                case Opcode.VNMLS:
                case Opcode.VNMUL:
                case Opcode.VORN:
                case Opcode.VORR:
                case Opcode.VPADAL:
                case Opcode.VPADDL:
                case Opcode.VPADD:
                case Opcode.VPMAX:
                case Opcode.VPMIN:
                case Opcode.VQABS:
                case Opcode.VQADD:
                case Opcode.VQDMLAL:
                case Opcode.VQDMLSL:
                case Opcode.VQDMULH:
                case Opcode.VQDMULL:
                case Opcode.VQMOVUN:
                case Opcode.VQMOVN:
                case Opcode.VQNEG:
                case Opcode.VQRDMULH:
                case Opcode.VQRSHL:
                case Opcode.VQRSHRN:
                case Opcode.VQRSHRUN:
                case Opcode.VQSHL:
                case Opcode.VQSHLU:
                case Opcode.VQSHRN:
                case Opcode.VQSHRUN:
                case Opcode.VQSUB:
                case Opcode.VRADDHN:
                case Opcode.VRECPE:
                case Opcode.VRECPS:
                case Opcode.VREV16:
                case Opcode.VREV32:
                case Opcode.VREV64:
                case Opcode.VRHADD:
                case Opcode.VRINTA:
                case Opcode.VRINTM:
                case Opcode.VRINTN:
                case Opcode.VRINTP:
                case Opcode.VRINTR:
                case Opcode.VRINTX:
                case Opcode.VRINTZ:
                case Opcode.VRSHL:
                case Opcode.VRSHRN:
                case Opcode.VRSHR:
                case Opcode.VRSQRTE:
                case Opcode.VRSQRTS:
                case Opcode.VRSRA:
                case Opcode.VRSUBHN:
                case Opcode.VSELEQ:
                case Opcode.VSELGE:
                case Opcode.VSELGT:
                case Opcode.VSELVS:
                case Opcode.VSHLL:
                case Opcode.VSHL:
                case Opcode.VSHRN:
                case Opcode.VSHR:
                case Opcode.VSLI:
                case Opcode.VSQRT:
                case Opcode.VSRA:
                case Opcode.VSRI:
                case Opcode.VST1:
                case Opcode.VST2:
                case Opcode.VST3:
                case Opcode.VST4:
                case Opcode.VSTMDB:
                case Opcode.VSTR:
                case Opcode.VSUB:
                case Opcode.VSUBHN:
                case Opcode.VSUBL:
                case Opcode.VSUBW:
                case Opcode.VSWP:
                case Opcode.VTBL:
                case Opcode.VTBX:
                case Opcode.VCVTR:
                case Opcode.VTRN:
                case Opcode.VTST:
                case Opcode.VUZP:
                case Opcode.VZIP:
                case Opcode.ADDW:
                case Opcode.ASR:
                case Opcode.DCPS1:
                case Opcode.DCPS2:
                case Opcode.DCPS3:
                case Opcode.IT:
                case Opcode.LSL:
                case Opcode.LSR:
                case Opcode.ASRS:
                case Opcode.LSRS:
                case Opcode.ORN:
                case Opcode.ROR:
                case Opcode.RRX:
                case Opcode.SUBS:
                case Opcode.SUBW:
                case Opcode.TBB:
                case Opcode.TBH:
                case Opcode.CBNZ:
                case Opcode.CBZ:
                case Opcode.MOVS:
                case Opcode.YIELD:
                case Opcode.WFE:
                case Opcode.WFI:
                case Opcode.SEV:
                case Opcode.SEVL:
                case Opcode.VPUSH:
                case Opcode.VPOP:
                    NotImplementedYet();
                    break;

                case Opcode.ADC: RewriteAdcSbc(m.IAdd); break;
                case Opcode.ADD: RewriteBinOp(m.IAdd, instr.ArchitectureDetail.UpdateFlags); break;
                case Opcode.AND: RewriteBinOp(m.And, instr.ArchitectureDetail.UpdateFlags); break;
                case Opcode.EOR: RewriteBinOp(m.Xor, instr.ArchitectureDetail.UpdateFlags); break;
                case Opcode.B: RewriteB(false); break;
                case Opcode.BFC: RewriteBfc(); break;
                case Opcode.BFI: RewriteBfi(); break;
                case Opcode.BIC: RewriteBic(); break;
                case Opcode.BL: RewriteB(true); break;
                case Opcode.BLX: RewriteB(true); break;
                case Opcode.BX: RewriteB(false); break;
                case Opcode.CLZ: RewriteClz(); break;
                case Opcode.CMN: RewriteCmn(); break;
                case Opcode.CMP: RewriteCmp(); break;
                case Opcode.CPS: RewriteCps(); break;
                case Opcode.DMB: RewriteDmb(); break;
                case Opcode.LDR: RewriteLdr(PrimitiveType.Word32); break;
                case Opcode.LDRB: RewriteLdr(PrimitiveType.Byte); break;
                case Opcode.LDRH: RewriteLdr(PrimitiveType.UInt16); break;
                case Opcode.LDRSB: RewriteLdr(PrimitiveType.SByte); break;
                case Opcode.LDRSH: RewriteLdr(PrimitiveType.Int16); break;
                case Opcode.LDRD: RewriteLdrd(); break;
                case Opcode.LDM: RewriteLdm(0); break;
                case Opcode.LDMDB: RewriteLdm(0); break;
                case Opcode.LDMIB: RewriteLdm(4); break;
                case Opcode.NOP: m.Nop(); break;
                case Opcode.MCR: RewriteMcr(); break;
                case Opcode.MLA: RewriteMultiplyAccumulate(m.IAdd); break;
                case Opcode.MLS: RewriteMultiplyAccumulate(m.ISub); break;
                case Opcode.MOV: RewriteMov(); break;
                case Opcode.MOVT: RewriteMovt(); break;
                case Opcode.MOVW: RewriteMov(); break;
                case Opcode.MRC: RewriteMrc(); break;
                case Opcode.MRS: RewriteMrs(); break;
                case Opcode.MSR: RewriteMsr(); break;
                case Opcode.MUL: RewriteBinOp(m.IMul, instr.ArchitectureDetail.UpdateFlags); break;
                case Opcode.MVN: RewriteUnaryOp(Operator.Not); break;
                case Opcode.ORR: RewriteBinOp(m.Or, false); break;
                case Opcode.POP: RewritePop(); break;
                case Opcode.PUSH: RewritePush(); break;
                case Opcode.REV: RewriteRev(); break;
                case Opcode.RSB: RewriteRevBinOp(Operator.ISub, instr.ArchitectureDetail.UpdateFlags); break;
                case Opcode.SBC: RewriteAdcSbc(m.ISub); break;
                case Opcode.SBFX: RewriteSbfx(); break;
                case Opcode.SMULBB: RewriteMulbb(false, false, PrimitiveType.Int16, m.SMul); break;
                case Opcode.SMULL: RewriteMull(PrimitiveType.Int64, m.SMul); break;
                case Opcode.STM: RewriteStm(); break;
                case Opcode.STMDB: RewriteStm(); break;
                case Opcode.STMIB: RewriteStmib(); break;
                case Opcode.STR: RewriteStr(PrimitiveType.Word32); break;
                case Opcode.STRB: RewriteStr(PrimitiveType.Byte); break;
                case Opcode.STRD: RewriteStrd(); break;
                case Opcode.STRH: RewriteStr(PrimitiveType.UInt16); break;
                case Opcode.SUB: RewriteBinOp(m.ISub, instr.ArchitectureDetail.UpdateFlags); break;
                case Opcode.SVC: RewriteSvc(); break;
                case Opcode.SXTAB: RewriteXtab(PrimitiveType.SByte); break;
                case Opcode.SXTAH: RewriteXtab(PrimitiveType.Int16); break;
                case Opcode.SXTB: RewriteXtb(PrimitiveType.SByte); break;
                case Opcode.SXTH: RewriteXtb(PrimitiveType.Int16); break;
                case Opcode.TEQ: RewriteTeq(); break;
                case Opcode.TST: RewriteTst(); break;
                case Opcode.UBFX: RewriteUbfx(); break;
                case Opcode.UMLAL: RewriteUmlal(); break;
                case Opcode.UMULL: RewriteMull(PrimitiveType.UInt64, m.UMul); break;
                case Opcode.UXTAB: RewriteXtab(PrimitiveType.Byte); break;
                case Opcode.UXTAH: RewriteXtab(PrimitiveType.UInt16); break;
                case Opcode.UXTB: RewriteXtb(PrimitiveType.Byte); break;
                case Opcode.UXTH: RewriteXtb(PrimitiveType.UInt16); break;

                case Opcode.VLDMIA: RewriteVldmia(); break;
                case Opcode.VMOV: RewriteVmov(); break;
                case Opcode.VSTMIA: RewriteVstmia(); break;

                }
                yield return new RtlInstructionCluster(
                    instrs.Current.Address,
                    instr.Bytes.Length,
                    rtlInstructions.ToArray())
                {
                    Class = rtlc
                };
            }
            instrs.Dispose();
        }

        private void NotImplementedYet()
        {
            host.Error(
                instrs.Current.Address,
                string.Format(
                    "Rewriting ARM opcode '{0}' is not supported yet.",
                    instr.Mnemonic));
            m.Invalid();
        }

        private RtlClass Classify(CapstoneArmInstruction instr)
        {
            throw new NotImplementedException();
        }

        private void MaybeUpdateFlags(Expression opDst)
        {
            if (instr.ArchitectureDetail.UpdateFlags)
            {
                m.Assign(frame.EnsureFlagGroup(A32Registers.cpsr, 0x1111, "NZCV", PrimitiveType.Byte), m.Cond(opDst));
            }
        }

        private void RewriteB(bool link)
        {
            Expression dst;
            if (Dst.Type == ArmInstructionOperandType.Immediate)
            {
                dst = Address.Ptr32((uint)Dst.ImmediateValue.Value);
            }
            else
            {
                dst = Operand(Dst);
            }
            if (link)
            {
                rtlc = RtlClass.Transfer | RtlClass.Call;
                if (instr.ArchitectureDetail.CodeCondition != ArmCodeCondition.AL)
                {
                    m.BranchInMiddleOfInstruction(
                        TestCond(instr.ArchitectureDetail.CodeCondition).Invert(),
                        Address.Ptr32((uint)(instr.Address + instr.Bytes.Length)),
                        RtlClass.ConditionalTransfer);
                }
                    m.Call(dst, 0);
                }
                else
                {
                if (instr.ArchitectureDetail.CodeCondition == ArmCodeCondition.AL)
                {
                    rtlc = RtlClass.Transfer;
                    m.Goto(dst);
                }
                else
                {
                    rtlc = RtlClass.ConditionalTransfer;
                    var addr = dst as Address;
                    if (addr != null)
                    {
                        m.Branch(TestCond(instr.ArchitectureDetail.CodeCondition), addr, RtlClass.ConditionalTransfer);
                    }
                    else
                    {
                        m.BranchInMiddleOfInstruction(
                            TestCond(instr.ArchitectureDetail.CodeCondition).Invert(),
                            Address.Ptr32((uint)(instr.Address + instr.Bytes.Length)),
                            RtlClass.ConditionalTransfer);
                        m.Goto(dst, RtlClass.Transfer);
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void AddConditional(RtlInstruction rtlInstr)
        {
            if (instr.ArchitectureDetail.CodeCondition != ArmCodeCondition.AL)
            {
                m.BranchInMiddleOfInstruction(
                    TestCond(instr.ArchitectureDetail.CodeCondition).Invert(),
                    Address.Ptr32((uint)(instr.Address + instr.Bytes.Length)),
                    RtlClass.ConditionalTransfer);
            }
            m.Emit(rtlInstr);
        }

        private void ConditionalAssign(Expression dst, Expression src)
        {
            var rtlInstr = new RtlAssignment(dst, src);
            AddConditional(rtlInstr);
            }

        // If a conditional ARM instruction is encountered, generate an IL
        // instruction to skip the remainder of the instruction cluster.
        private void ConditionalSkip()
        {
            var cc = instr.ArchitectureDetail.CodeCondition;
            if (cc == ArmCodeCondition.AL)
                return; // never skip!
            m.BranchInMiddleOfInstruction(
                TestCond(cc).Invert(),
                Address.Ptr32((uint)instr.Address + 4),
                RtlClass.ConditionalTransfer);
        }

        private Expression Operand(ArmInstructionOperand op)
        {
            switch (op.Type)
            {
            case ArmInstructionOperandType.Register:
                var reg = frame.EnsureRegister(A32Registers.RegisterByCapstoneID[op.RegisterValue.Value]);
                return MaybeShiftOperand(reg, op);
            case ArmInstructionOperandType.SysRegister:
                var sysreg = frame.EnsureRegister(A32Registers.SysRegisterByCapstoneID[op.SysRegisterValue.Value]);
                return sysreg;
            case ArmInstructionOperandType.Immediate:
                return Constant.Word32(op.ImmediateValue.Value);
            case ArmInstructionOperandType.CImmediate:
            case ArmInstructionOperandType.PImmediate:
                return Constant.Byte((byte)op.ImmediateValue.Value);
            case ArmInstructionOperandType.Memory:
                Expression baseReg = Reg(op.MemoryValue.BaseRegister);
                Expression ea = baseReg;
                if (op.MemoryValue.BaseRegister
                    == ArmRegister.PC)  // PC-relative address
                {
                    var dst = (uint)((int)instrs.Current.Address.ToUInt32() + op.MemoryValue.Displacement) + 8u;
                    ea = Address.Ptr32(dst);
                    if (op.MemoryValue.IndexRegister != ArmRegister.Invalid)
                    {
                        var ireg = Reg(op.MemoryValue.IndexRegister);
                        if (op.Shifter.Type == ArmShifterType.LSL)
                        {
                            ea = m.IAdd(ea, m.IMul(ireg, 1 << op.Shifter.Value));
                        }
                        else
                            throw new NotImplementedException();
                    }
                    return m.Load(SizeFromLoadStore(instr), ea);
                }
                if (op.MemoryValue.Displacement != 0 && instrs.Current.IsLastOperand(op))
                {
                    var offset = Constant.Int32(op.MemoryValue.Displacement);
                    ea = op.MemoryValue.IndexRegisterScale < 0
                        ? m.ISub(ea, offset)
                        : m.IAdd(ea, offset);
                }
                if (instrs.Current.IsLastOperand(op) && instr.ArchitectureDetail.WriteBack)
                {
                    m.Assign(baseReg, ea);
                    ea = baseReg;
                }
                return m.Load(SizeFromLoadStore(instr), ea);
            case ArmInstructionOperandType.FloatingPoint:
                return Constant.Real64(op.FloatingPointValue.Value);
            }
            throw new NotImplementedException(op.Type.ToString());
        }

        private DataType SizeFromLoadStore(CapstoneArmInstruction instr)
        {
            switch (instr.Id)
            {
            case Opcode.LDR: return PrimitiveType.Word32;
            case Opcode.LDRB: return PrimitiveType.Byte;
            case Opcode.LDRD: return PrimitiveType.Word64;
            case Opcode.LDRH: return PrimitiveType.Word16;
            case Opcode.LDRSB: return PrimitiveType.SByte;
            case Opcode.LDRSH: return PrimitiveType.Int16;
            case Opcode.STR: return PrimitiveType.Word32;
            case Opcode.STRB: return PrimitiveType.Byte;
            case Opcode.STRD: return PrimitiveType.Word64;
            case Opcode.STRH: return PrimitiveType.Word16;
            }
            throw new NotImplementedException(instr.Id.ToString());
        }

        private Identifier Reg(int nReg)
        {
            return frame.EnsureRegister(A32Registers.RegisterByCapstoneID[(ArmRegister)nReg]);
        }

        private Identifier Reg(ArmRegister reg)
        {
            return frame.EnsureRegister(A32Registers.RegisterByCapstoneID[reg]);
        }

        private Expression MaybeShiftOperand(Expression exp, ArmInstructionOperand op)
        {
            switch (op.Shifter.Type)
            {
            case ArmShifterType.ASR: return m.Sar(exp, op.Shifter.Value);
            case ArmShifterType.LSL: return m.Shl(exp, op.Shifter.Value);
            case ArmShifterType.LSR: return m.Shr(exp, op.Shifter.Value);
            case ArmShifterType.ROR: return host.PseudoProcedure(PseudoProcedure.Ror, PrimitiveType.Word32, exp, Constant.Int32(op.Shifter.Value)); 
            case ArmShifterType.RRX: return host.PseudoProcedure("rrx", PrimitiveType.Word32, exp, Constant.Int32(op.Shifter.Value));
            case ArmShifterType.ASR_REG: return m.Sar(exp, Reg(op.Shifter.Value));
            case ArmShifterType.LSL_REG: return m.Shl(exp, Reg(op.Shifter.Value));
            case ArmShifterType.LSR_REG: return m.Shr(exp, Reg(op.Shifter.Value));
            case ArmShifterType.ROR_REG: return host.PseudoProcedure(PseudoProcedure.Ror, PrimitiveType.Word32, exp, Reg(op.Shifter.Value));
            case ArmShifterType.RRX_REG: return host.PseudoProcedure("rrx", PrimitiveType.Word32, exp, Reg(op.Shifter.Value));
            default: return exp;
            }
        }

        private void MaybePostOperand(ArmInstructionOperand op)
        {
            if (instrs.Current.IsLastOperand(op))
                return;
            if (op.Type != ArmInstructionOperandType.Memory)
                return;
            var lastOp = instr.ArchitectureDetail.Operands[instr.ArchitectureDetail.Operands.Length - 1];
            Expression baseReg = Reg(op.MemoryValue.BaseRegister);
            var offset = Operand(lastOp);
            var ea = lastOp.IsSubtracted
                ? m.ISub(baseReg, offset)
                : m.IAdd(baseReg, offset);
            m.Assign(baseReg, ea);
#if NYI
            if (memOp == null || memOp.Offset == null)
                return;
            if (memOp.Preindexed)
                return;
            Expression baseReg = frame.EnsureRegister(memOp.Base);
            var offset = Operand(memOp.Offset);
            var ea = memOp.Subtract
                ? emitter.ISub(baseReg, offset)
                : emitter.IAdd(baseReg, offset);
            emitter.Assign(baseReg, ea);
#endif
        }

        private TestCondition TestCond(ArmCodeCondition cond)
        {
            switch (cond)
            {
            default:
                throw new NotImplementedException(string.Format("ARM condition code {0} not implemented.", cond));
            case ArmCodeCondition.HS:
                return new TestCondition(ConditionCode.UGE, FlagGroup(FlagM.CF, "C", PrimitiveType.Byte));
            case ArmCodeCondition.LO:
                return new TestCondition(ConditionCode.ULT, FlagGroup(FlagM.CF, "C", PrimitiveType.Byte));
            case ArmCodeCondition.EQ:
                return new TestCondition(ConditionCode.EQ, FlagGroup(FlagM.ZF, "Z", PrimitiveType.Byte));
            case ArmCodeCondition.GE:
                return new TestCondition(ConditionCode.GE, FlagGroup(FlagM.NF | FlagM.ZF | FlagM.VF, "NZV", PrimitiveType.Byte));
            case ArmCodeCondition.GT:
                return new TestCondition(ConditionCode.GT, FlagGroup(FlagM.NF | FlagM.ZF | FlagM.VF, "NZV", PrimitiveType.Byte));
            case ArmCodeCondition.HI:
                return new TestCondition(ConditionCode.UGT, FlagGroup(FlagM.ZF | FlagM.CF, "ZC", PrimitiveType.Byte));
            case ArmCodeCondition.LE:
                return new TestCondition(ConditionCode.LE, FlagGroup(FlagM.ZF | FlagM.CF | FlagM.VF, "NZV", PrimitiveType.Byte));
            case ArmCodeCondition.LS:
                return new TestCondition(ConditionCode.ULE, FlagGroup(FlagM.ZF | FlagM.CF, "ZC", PrimitiveType.Byte));
            case ArmCodeCondition.LT:
                return new TestCondition(ConditionCode.LT, FlagGroup(FlagM.NF | FlagM.VF, "NV", PrimitiveType.Byte));
            case ArmCodeCondition.MI:
                return new TestCondition(ConditionCode.LT, FlagGroup(FlagM.NF, "N", PrimitiveType.Byte));
            case ArmCodeCondition.PL:
                return new TestCondition(ConditionCode.GT, FlagGroup(FlagM.NF | FlagM.ZF, "NZ", PrimitiveType.Byte));
            case ArmCodeCondition.NE:
                return new TestCondition(ConditionCode.NE, FlagGroup(FlagM.ZF, "Z", PrimitiveType.Byte));
            case ArmCodeCondition.VC:
                return new TestCondition(ConditionCode.NO, FlagGroup(FlagM.VF, "V", PrimitiveType.Byte));
            case ArmCodeCondition.VS:
                return new TestCondition(ConditionCode.OV, FlagGroup(FlagM.VF, "V", PrimitiveType.Byte));
            }
        }

        private Identifier FlagGroup(FlagM bits, string name, PrimitiveType type)
        {
            return frame.EnsureFlagGroup(A32Registers.cpsr, (uint) bits, name, type);
        }

        private void RewriteSvc()
        {
            m.SideEffect(m.Fn(
                host.EnsurePseudoProcedure(PseudoProcedure.Syscall, VoidType.Instance, 2), 
                Operand(Dst)));
        }
    }
}
