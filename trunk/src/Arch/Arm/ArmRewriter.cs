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

using Decompiler.Core;
using Decompiler.Core.Expressions;
using Decompiler.Core.Operators;
using Decompiler.Core.Machine;
using Decompiler.Core.Rtl;
using Decompiler.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Decompiler.Arch.Arm
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
                case Opcode.add: RewriteBinOp(Operator.IAdd, false); break;
                case Opcode.adds: RewriteBinOp(Operator.IAdd, true); break;
                case Opcode.mvn: RewriteUnaryOp(Operator.Not); break;
                case Opcode.b: RewriteB(false); break;
                case Opcode.bic: RewriteBic(); break;
                case Opcode.bl: RewriteB(true); break;
                case Opcode.cmn: RewriteCmn(); break;
                case Opcode.cmp: RewriteCmp(); break;
                case Opcode.ldr: RewriteLdr(PrimitiveType.Word32); break;
                case Opcode.ldrsb: RewriteLdr(PrimitiveType.SByte); break;
                case Opcode.ldrb: RewriteLdr(PrimitiveType.Byte); break;
                case Opcode.ldm: RewriteLdm(); break;
                case Opcode.mov: RewriteMov(); break;
                case Opcode.orr: RewriteBinOp(Operator.Or, false); break;
                case Opcode.stm: RewriteStm(); break;
                case Opcode.stmdb: RewriteStm(); break;
                case Opcode.str: RewriteStr(PrimitiveType.Word32); break;
                case Opcode.strb: RewriteStr(PrimitiveType.Byte); break;
                case Opcode.sub: RewriteBinOp(Operator.ISub, false); break;
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
                    NYI();
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
                case Opcode.asr: return emitter.Sar(r, sh);
                default: throw new NotSupportedException(string.Format("Unsupported shift operation {0}.", shOp.Opcode));
                }
            }
            var memOp = op as ArmMemoryOperand;
            if (memOp != null)
            {
                Expression baseReg = frame.EnsureRegister(memOp.Base);
                Expression ea = baseReg;
                if (memOp.Offset != null)
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

        private TestCondition TestCond(Condition cond)
        {
            switch (cond)
            {
            default:
                throw new NotImplementedException(string.Format("ARM condition code {0} not implemented.", cond));
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
