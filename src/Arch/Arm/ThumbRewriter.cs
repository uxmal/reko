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

using Gee.External.Capstone;
using Gee.External.Capstone.Arm;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Arm
{
    public partial class ThumbRewriter : IEnumerable<RtlInstructionCluster>
    {
        private IEnumerator<Arm32Instruction> instrs;
        private IStorageBinder frame;
        private IRewriterHost host;
        private Instruction<ArmInstruction,ArmRegister,ArmInstructionGroup,ArmInstructionDetail> instr;
        private ArmInstructionOperand[] ops;
        private RtlClass rtlc;
        private List<RtlInstruction> rtlInstructions;
        private RtlEmitter m;
        private int itState;
        private ArmCodeCondition itStateCondition;

        public ThumbRewriter(ThumbProcessorArchitecture arch, EndianImageReader rdr, ArmProcessorState state, IStorageBinder frame, IRewriterHost host)
        {
            this.instrs = CreateInstructionStream(rdr);
            this.frame = frame;
            this.host = host;
            this.itState = 0;
            this.itStateCondition = ArmCodeCondition.AL;
        }

        private IEnumerator<Arm32Instruction> CreateInstructionStream(EndianImageReader rdr)
        {
            return new ThumbDisassembler(rdr).GetEnumerator();
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (instrs.MoveNext())
            {
                if (!instrs.Current.TryGetInternal(out this.instr))
                {
                    continue;
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
                    throw new AddressCorrelatedException(
                      instrs.Current.Address,
                      "Rewriting ARM Thumb opcode '{0}' ({1}) is not supported yet.",
                      instr.Mnemonic, instr.Id);
                case ArmInstruction.ADD: RewriteBinop((a, b) => m.IAdd(a, b)); break;
                case ArmInstruction.ADDW: RewriteAddw(); break;
                case ArmInstruction.ADR: RewriteAdr(); break;
                case ArmInstruction.AND: RewriteAnd(); break;
                case ArmInstruction.ASR: RewriteShift(m.Sar); break;
                case ArmInstruction.B: RewriteB(); break;
                case ArmInstruction.BIC: RewriteBic(); break;
                case ArmInstruction.BL: RewriteBl(); break;
                case ArmInstruction.BLX: RewriteBlx(); break;
                case ArmInstruction.BX: RewriteBx(); break;
                case ArmInstruction.CBZ: RewriteCbnz(m.Eq0); break;
                case ArmInstruction.CBNZ: RewriteCbnz(m.Ne0); break;
                case ArmInstruction.CMP: RewriteCmp(); break;
                case ArmInstruction.DMB: RewriteDmb(); break;
                case ArmInstruction.EOR: RewriteEor(); break;
                case ArmInstruction.IT: RewriteIt(); continue;  // Don't emit anything yet.;
                case ArmInstruction.LDR: RewriteLdr(PrimitiveType.Word32, PrimitiveType.Word32); break;
                case ArmInstruction.LDRB: RewriteLdr(PrimitiveType.UInt32,PrimitiveType.Byte); break;
                case ArmInstruction.LDRSB: RewriteLdr(PrimitiveType.Int32,PrimitiveType.SByte); break;
                case ArmInstruction.LDREX: RewriteLdrex(); break;
                case ArmInstruction.LDRH: RewriteLdr(PrimitiveType.UInt32, PrimitiveType.Word16); break;
                case ArmInstruction.LSL: RewriteShift(m.Shl); break;
                case ArmInstruction.LSR: RewriteShift(m.Shr); break;
                case ArmInstruction.MOV: RewriteMov(); break;
                case ArmInstruction.MOVT: RewriteMovt(); break;
                case ArmInstruction.MOVW: RewriteMovw(); break;
                case ArmInstruction.MRC: RewriteMrc(); break;
                case ArmInstruction.MVN: RewriteMvn(); break;
                case ArmInstruction.POP: RewritePop(); break;
                case ArmInstruction.PUSH: RewritePush(); break;
                case ArmInstruction.RSB: RewriteRsb(); break;
                case ArmInstruction.STM: RewriteStm(); break;
                case ArmInstruction.STR: RewriteStr(PrimitiveType.Word32); break;
                case ArmInstruction.STRH: RewriteStr(PrimitiveType.Word16); break;
                case ArmInstruction.STRB: RewriteStr(PrimitiveType.Byte); break;
                case ArmInstruction.STREX: RewriteStrex(); break;
                case ArmInstruction.SUB: RewriteBinop((a, b) => m.ISub(a, b)); break;
                case ArmInstruction.SUBW: RewriteSubw(); break;
                case ArmInstruction.TRAP: RewriteTrap(); break;
                case ArmInstruction.TST: RewriteTst(); break;
                case ArmInstruction.UDF: RewriteUdf(); break;
                case ArmInstruction.UXTH: RewriteUxth(); break;
                }
                itState = (itState << 1) & 0x0F;
                if (itState == 0)
                {
                    itStateCondition = ArmCodeCondition.AL;
                }
                yield return new RtlInstructionCluster(
                    instrs.Current.Address,
                    instr.Bytes.Length,
                    rtlInstructions.ToArray())
                {
                    Class = rtlc
                };
                
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private Expression GetReg(ArmRegister armRegister)
        {
            return frame.EnsureRegister(A32Registers.RegisterByCapstoneID[armRegister]);
        }

        private Expression RewriteOp(ArmInstructionOperand op, DataType accessSize= null)
        {
            switch (op.Type)
            {
            case ArmInstructionOperandType.Register:
                return GetReg(op.RegisterValue.Value);
            case ArmInstructionOperandType.Immediate:
                if (accessSize != null)
                    return Constant.Create(accessSize, op.ImmediateValue.Value);
                else 
                    return Constant.Int32(op.ImmediateValue.Value);
            case ArmInstructionOperandType.Memory:
                var mem = op.MemoryValue;
                var ea = EffectiveAddress(mem);
                return m.Mem(accessSize, ea);
            default:
                throw new NotImplementedException(op.Type.ToString());
            }
        }

        private Expression EffectiveAddress(ArmInstructionMemoryOperandValue mem)
        {
            var baseReg = GetReg(mem.BaseRegister);
            var ea = baseReg;
            if (mem.Displacement > 0)
            {
                ea = m.IAdd(ea, Constant.Int32(mem.Displacement));
            }
            else if (mem.Displacement < 0)
            {
                ea = m.ISub(ea, Constant.Int32(-mem.Displacement));
            }
            else if (mem.IndexRegister != ArmRegister.Invalid)
            {
                ea = m.IAdd(ea, GetReg(mem.IndexRegister));
            }
            return ea;
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
            case ArmCodeCondition.VS:
                return new TestCondition(ConditionCode.OV, FlagGroup(FlagM.VF, "V", PrimitiveType.Byte));
            }
        }

        private Identifier FlagGroup(FlagM bits, string name, PrimitiveType type)
        {
            return frame.EnsureFlagGroup(A32Registers.cpsr, (uint)bits, name, type);
        }

        private void Predicate(ArmCodeCondition cond, RtlInstruction instr)
        {
            if (cond == ArmCodeCondition.AL)
                m.Emit(instr);
            else
            {
                m.BranchInMiddleOfInstruction(
                    TestCond(cond).Invert(),
                    Address.Ptr32((uint)(this.instr.Address + this.instr.Bytes.Length)),
                    RtlClass.ConditionalTransfer);
                m.Emit(instr);
            }
        }

        private void Predicate(ArmCodeCondition cond, Expression dst, Expression src)
        {
            RtlInstruction instr;
            Identifier id;
            if (dst.As<Identifier>(out id) && id.Storage == A32Registers.pc)
            {
                rtlc = RtlClass.Transfer;
                instr = new RtlGoto(src, RtlClass.Transfer);
            }
            else
            {
                instr = new RtlAssignment(dst, src);
            }
            Predicate(cond, instr);
        }
    }
}
