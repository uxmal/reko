#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
using Reko.Core.Operators;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Reko.Arch.Arm
{
    public partial class ArmRewriter : IEnumerable<RtlInstructionCluster>
    {
        private ArmProcessorArchitecture arch;
        private IEnumerator<ArmInstruction> instrs;
        private ArmProcessorState state;
        private Frame frame;
        private ArmInstruction instr;
        private RtlInstructionCluster ric;
        private RtlEmitter emitter;
        private IRewriterHost host;

        public ArmRewriter(ArmProcessorArchitecture arch, ImageReader rdr, ArmProcessorState state, Frame frame, IRewriterHost host)
        {
            this.arch = arch;
            this.instrs = CreateInstructionStream(rdr);
            this.state = state;
            this.frame = frame;
            this.host = host;
        }

        private IEnumerator<ArmInstruction> CreateInstructionStream(ImageReader rdr)
        {
            return new ArmDisassembler(arch, rdr).GetEnumerator();
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (instrs.MoveNext())
            {
                this.instr = instrs.Current;
                this.ric = new RtlInstructionCluster(instr.Address, instr.Length);
                this.emitter = new RtlEmitter(ric.Instructions);
                switch (instr.Opcode)
                {
                default:
                    throw new AddressCorrelatedException(
                        instr.Address,
                        "Rewriting ARM opcode '{0}' is not supported yet.",
                        instr.Opcode);
                case Opcode.and: RewriteBinOp(Operator.And, false); break;
                case Opcode.ands: RewriteBinOp(Operator.And, true); break;
                case Opcode.add: RewriteBinOp(Operator.IAdd, false); break;
                case Opcode.adds: RewriteBinOp(Operator.IAdd, true); break;
                case Opcode.eor: RewriteBinOp(Operator.Xor, false); break;
                case Opcode.eors: RewriteBinOp(Operator.Xor, true); break;
                case Opcode.mvn: RewriteUnaryOp(Operator.Not); break;
                case Opcode.b: RewriteB(false); break;
                case Opcode.bic: RewriteBic(); break;
                case Opcode.bl: RewriteB(true); break;
                case Opcode.cmn: RewriteCmn(); break;
                case Opcode.cmp: RewriteCmp(); break;
                case Opcode.ldr: RewriteLdr(PrimitiveType.Word32); break;
                case Opcode.ldrb: RewriteLdr(PrimitiveType.Byte); break;
                case Opcode.ldrh: RewriteLdr(PrimitiveType.UInt16); break;
                case Opcode.ldrsb: RewriteLdr(PrimitiveType.SByte); break;
                case Opcode.ldrsh: RewriteLdr(PrimitiveType.Int16); break;
                case Opcode.ldm: RewriteLdm(); break;
                case Opcode.ldmdb: RewriteLdm(); break;
                case Opcode.ldmfd: RewriteLdm(); break;
                case Opcode.mov: RewriteMov(); break;
                case Opcode.orr: RewriteBinOp(Operator.Or, false); break;
                case Opcode.rsb: RewriteRevBinOp(Operator.ISub, false); break;
                case Opcode.rsbs: RewriteRevBinOp(Operator.ISub, true); break;
                case Opcode.stm: RewriteStm(); break;
                case Opcode.stmdb: RewriteStm(); break;
                case Opcode.stmib: RewriteStmib(); break;
                case Opcode.str: RewriteStr(PrimitiveType.Word32); break;
                case Opcode.strb: RewriteStr(PrimitiveType.Byte); break;
                case Opcode.strh: RewriteStr(PrimitiveType.UInt16); break;
                case Opcode.sub: RewriteBinOp(Operator.ISub, false); break;
                case Opcode.subs: RewriteBinOp(Operator.ISub, true); break;
                case Opcode.svc: RewriteSvc(); break;
                case Opcode.teq: RewriteTeq(); break;
                case Opcode.tst: RewriteTst(); break;
                }
                yield return ric;
            }
        }

        private AddressCorrelatedException NYI()
        {
            return new AddressCorrelatedException(
                instr.Address,
                "Rewriting ARM opcode '{0}' is not supported yet.",
                instr.Opcode);
        }

        private void RewriteB(bool link)
        {
            Address addr = ((AddressOperand)instr.Dst).Address;
            if (link)
            {
                if (instr.Cond == Condition.al)
                {
                    emitter.Call(addr, 0);
                }
                else
                {
                    emitter.If(TestCond(instr.Cond), new RtlCall(addr, 0, RtlClass.Transfer));
                }
            }
            else
            {
                if (instr.Cond == Condition.al)
                {
                    emitter.Goto(addr);
                }
                else
                {
                    emitter.Branch(TestCond(instr.Cond), addr, RtlClass.ConditionalTransfer);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void AddConditional(RtlInstruction rtlInstr)
        {
            if (instr.Cond != Condition.al)
            {
                rtlInstr = new RtlIf(TestCond(instr.Cond), rtlInstr);
            }
            ric.Instructions.Add(rtlInstr);
        }

        private void ConditionalAssign(Expression dst, Expression src)
        {
            RtlInstruction rtlInstr = new RtlAssignment(dst, src);
            if (instr.Cond != Condition.al)
            {
                rtlInstr = new RtlIf(TestCond(instr.Cond), rtlInstr);
            }
            ric.Instructions.Add(rtlInstr);
        }

        private Expression Operand(MachineOperand op)
        {
            var rop = op as RegisterOperand;
            if (rop != null)
            {
                return frame.EnsureRegister(rop.Register);
            }
            var immOp = op as ImmediateOperand;
            if (immOp != null)
            {
                return immOp.Value;
            }
            var shOp = op as ShiftOperand;
            if (shOp != null)
            {
                var r = Operand(shOp.Operand);
                var sh = Operand(shOp.Shift);
                switch (shOp.Opcode)
                {
                case Opcode.lsl: return emitter.Shl(r, sh);
                case Opcode.lsr: return emitter.Shr(r, sh);
                case Opcode.asr: return emitter.Sar(r, sh);
                case Opcode.ror: return host.PseudoProcedure(PseudoProcedure.Ror, PrimitiveType.Word32, r, sh);
                default: throw new NotSupportedException(string.Format("Unsupported shift operation {0}.", shOp.Opcode));
                }
            }
            var memOp = op as ArmMemoryOperand;
            if (memOp != null)
            {
                Expression baseReg = frame.EnsureRegister(memOp.Base);
                Expression ea = baseReg;
                if (memOp.Base.Number == 0x0F)  // PC-relative address
                {
                    var imm = memOp.Offset as ArmImmediateOperand;
                    if (imm != null)
                    {
                        if (memOp.Writeback)
                            throw new NotImplementedException();
                        var dst = (uint)((int)instr.Address.ToUInt32() + imm.Value.ToInt32()) + 8u;

                        return emitter.Load(memOp.Width, Address.Ptr32(dst));
                    }
                }
                if (memOp.Offset != null && memOp.Preindexed)
                {
                    var offset = Operand(memOp.Offset);
                    ea = memOp.Subtract
                        ? emitter.ISub(ea, offset)
                        : emitter.IAdd(ea, offset);
                }
                if (memOp.Preindexed && memOp.Writeback)
                {
                    emitter.Assign(baseReg, ea);
                    ea = baseReg;
                }
                return emitter.Load(memOp.Width, ea);
            }
            throw new NotSupportedException(string.Format("Unsupported operand {0}.", op));
        }

        private void MaybePostOperand(MachineOperand op)
        {
            var memOp = op as ArmMemoryOperand;
            if (memOp == null || memOp.Offset  == null)
                return;
            if (memOp.Preindexed)
                return;
            Expression baseReg = frame.EnsureRegister(memOp.Base);
            var offset = Operand(memOp.Offset);
            var ea = memOp.Subtract
                ? emitter.ISub(baseReg, offset)
                : emitter.IAdd(baseReg, offset);
            emitter.Assign(baseReg, ea);
        }

        private TestCondition TestCond(Condition cond)
        {
            switch (cond)
            {
            default:
                throw new NotImplementedException(string.Format("ARM condition code {0} not implemented.", cond));
            case Condition.cc:
                return new TestCondition(ConditionCode.UGE, FlagGroup(FlagM.CF, "C", PrimitiveType.Byte));
            case Condition.cs:
                return new TestCondition(ConditionCode.ULT, FlagGroup(FlagM.CF, "C", PrimitiveType.Byte));
            case Condition.eq:
                return new TestCondition(ConditionCode.EQ, FlagGroup(FlagM.ZF, "Z", PrimitiveType.Byte));
            case Condition.ge:
                return new TestCondition(ConditionCode.GE, FlagGroup(FlagM.NF | FlagM.ZF | FlagM.VF, "NZV", PrimitiveType.Byte));
            case Condition.gt:
                return new TestCondition(ConditionCode.GT, FlagGroup(FlagM.NF | FlagM.ZF | FlagM.VF, "NZV", PrimitiveType.Byte));
            case Condition.hi:
                return new TestCondition(ConditionCode.UGT, FlagGroup(FlagM.ZF | FlagM.CF, "ZC", PrimitiveType.Byte));
            case Condition.le:
                return new TestCondition(ConditionCode.LE, FlagGroup(FlagM.ZF | FlagM.CF | FlagM.VF, "NZV", PrimitiveType.Byte));
            case Condition.ls:
                return new TestCondition(ConditionCode.ULE, FlagGroup(FlagM.ZF | FlagM.CF, "ZC", PrimitiveType.Byte));
            case Condition.lt:
                return new TestCondition(ConditionCode.LT, FlagGroup(FlagM.NF | FlagM.VF, "NV", PrimitiveType.Byte));
            case Condition.mi:
                return new TestCondition(ConditionCode.LT, FlagGroup(FlagM.NF, "N", PrimitiveType.Byte));
            case Condition.pl:
                return new TestCondition(ConditionCode.GT, FlagGroup(FlagM.NF | FlagM.ZF, "NZ", PrimitiveType.Byte));
            case Condition.ne:
                return new TestCondition(ConditionCode.NE, FlagGroup(FlagM.ZF, "Z", PrimitiveType.Byte));
            case Condition.vs:
                return new TestCondition(ConditionCode.OV, FlagGroup(FlagM.VF, "V", PrimitiveType.Byte));
            }
        }

        private Identifier FlagGroup(FlagM bits, string name, PrimitiveType type)
        {
            return frame.EnsureFlagGroup((uint) bits, name, type);
        }

        private void RewriteSvc()
        {
            emitter.SideEffect(emitter.Fn(
                host.EnsurePseudoProcedure("__syscall", VoidType.Instance, 2), 
                Operand(instr.Dst)));
        }
    }
}
