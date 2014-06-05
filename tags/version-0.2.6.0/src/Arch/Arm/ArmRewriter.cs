#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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

        public ArmRewriter(ArmProcessorArchitecture arch, ImageReader rdr, ArmProcessorState state, Frame frame)
        {
            this.arch = arch;
            this.instrs = CreateInstructionStream(rdr);
            this.state = state;
            this.frame = frame;
        }

        private IEnumerator<ArmInstruction> CreateInstructionStream(ImageReader rdr)
        {
            return new ArmDisassembler2(arch, rdr);
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
                    throw NYI();
                case Opcode.add: RewriteBinOp(Operator.IAdd); break;
                case Opcode.b: RewriteB(false); break;
                case Opcode.mov:
                    emitter.Assign(Operand(instr.Dst), Operand(instr.Src1)); break;
                case Opcode.orr: RewriteBinOp(Operator.Or); break;
                case Opcode.sub: RewriteBinOp(Operator.ISub); break;
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
                throw NYI();
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
                default: throw new NotSupportedException(string.Format("Unsupported shift operation {0}.", shOp.Opcode));
                }
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
                return new TestCondition(ConditionCode.EQ, frame.EnsureFlagGroup(0xF, "SCZO", PrimitiveType.Byte));
            case Condition.gt:
                return new TestCondition(ConditionCode.GT, frame.EnsureFlagGroup(0xF, "SCZO", PrimitiveType.Byte));
            }
        }
    }
}
