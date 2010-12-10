#region License
/* 
 * Copyright (C) 1999-2010 John Källén.
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
using Decompiler.Core.Code;
using Decompiler.Core.Lib;
using Decompiler.Core.Expressions;
using Decompiler.Core.Operators;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Analysis
{
    /// <summary>
    /// Before we have the luxury of SSA, we need to perform some simplifications. This class keeps a context of symbolic
    /// expressions for the different registers.
    /// </summary>
    public class SymbolicEvaluator : InstructionVisitor, EvaluationContext
    {
        private IProcessorArchitecture arch;
        private ExpressionSimplifier eval;

        public SymbolicEvaluator(IProcessorArchitecture arch)
        {
            this.arch = arch;
            RegisterState = new Dictionary<Expression, Expression>(new ExpressionValueComparer());
            StackState = new Map<int, Expression>();
            eval = new ExpressionSimplifier(this);
        }

        public Dictionary<Expression, Expression> RegisterState { get; private set; }
        public Map<int, Expression> StackState { get; private set; }

        public void Evaluate(Instruction instr)
        {
            instr.Accept(this);
        }

        /// <summary>
        /// Stack addresses are of the pattern <code>stackpointer+/-const</code>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool GetStackAddressOffset(Expression effectiveAddress, out int offset)
        {
            offset = 0;
            var ea = effectiveAddress as BinaryExpression;
            if (ea != null)
            {
                if (!IsStackRegister(ea.Left)) return false;
                var o = ea.Right as Constant;
                if (o == null) return false;
                offset = o.ToInt32();
                if (ea.op == Operator.Sub)
                    offset = -offset;
                return true;
            }
            else
            {
                return IsStackRegister(effectiveAddress);
            }
        }

        private bool IsStackRegister(Expression exp)
        {
            var sp = exp as Identifier;
            if (sp == null) return false;
            var regSp = sp.Storage as RegisterStorage;
            if (regSp == null) return false;
            return (regSp.Register == arch.StackRegister);
        }

        public void SetValue(Identifier id, Expression value)
        {
            RegisterState[id] = value;
        }

        #region InstructionVisitor Members

        void InstructionVisitor.VisitAssignment(Assignment a)
        {
            var valSrc = a.Src.Accept(eval);
            SetValue(a.Dst, valSrc);
        }

        void InstructionVisitor.VisitBranch(Branch b)
        {
            b.Condition.Accept(eval);
        }

        void InstructionVisitor.VisitCallInstruction(CallInstruction ci)
        {
            throw new NotImplementedException();
        }

        void InstructionVisitor.VisitDeclaration(Declaration decl)
        {
            if (decl.Expression != null)
            {
                var value = decl.Expression.Accept(eval);
                SetValue(decl.Identifier, value);
            }
        }

        void InstructionVisitor.VisitDefInstruction(DefInstruction def)
        {
        }

        void InstructionVisitor.VisitGotoInstruction(GotoInstruction gotoInstruction)
        {
        }

        void InstructionVisitor.VisitPhiAssignment(PhiAssignment phi)
        {
        }

        void InstructionVisitor.VisitIndirectCall(IndirectCall ic)
        {
            ic.Callee.Accept(eval);
        }

        void InstructionVisitor.VisitReturnInstruction(ReturnInstruction ret)
        {
            ret.Expression.Accept(eval);
        }

        void InstructionVisitor.VisitSideEffect(SideEffect side)
        {
            side.Expression.Accept(eval);
        }

        void InstructionVisitor.VisitStore(Store store)
        {
            var valSrc = store.Src.Accept(eval);
            var access = store.Dst as MemoryAccess;
            if (access != null)
            {
                var ea = access.EffectiveAddress.Accept(eval);
                int offset;
                if (GetStackAddressOffset(ea, out offset))
                    StackState[offset] = valSrc;
            }
        }

        void InstructionVisitor.VisitSwitchInstruction(SwitchInstruction si)
        {
            si.Expression.Accept(eval);
        }

        void InstructionVisitor.VisitUseInstruction(UseInstruction u)
        {
            throw new NotSupportedException();
        }

        #endregion


        #region EvaluationContext Members

        public Expression GetValue(Identifier id)
        {
            Expression value;
            if (RegisterState.TryGetValue(id, out value))
                return value;
            return id;
        }

        public Expression GetValue(MemoryAccess access)
        {
            return GetValueEa(access);
        }

        private Expression GetValueEa(MemoryAccess access)
        {
            int offset;
            if (!GetStackAddressOffset(access.EffectiveAddress, out offset))
                return Constant.Invalid;

            Expression value;
            if (StackState.TryGetValue(offset, out value))
            {
                int excess = access.DataType.Size - value.DataType.Size;
                if (excess == 0)
                    return value;
                else if (excess > 0)
                {
                    int remainder = offset + value.DataType.Size;
                    Expression v2;
                    if (StackState.TryGetValue(remainder, out v2))
                    {
                        return new MkSequence(access.DataType, v2, value);
                    }
                }
                else
                {
                    return new Cast(access.DataType, value);
                }
            }
            else
            {
                int offset2;
                if (StackState.TryGetLowerBoundKey(offset, out offset2))
                {
                    var value2 = StackState[offset2];
                    if (offset2 + value2.DataType.Size > offset)
                        return new Slice(access.DataType, StackState[offset2], (uint)((offset - offset2) * 8));
                }
            }
            return Constant.Invalid;
        }

        public Expression GetValue(SegmentedAccess access)
        {
            return GetValueEa(access);
        }

        public Expression GetValue(Application appl)
        {
            var args = appl.Arguments;
            for (int i = 0; i < args.Length; ++i)
            {
                var outArg = args[i] as UnaryExpression;
                if (outArg == null || outArg.op != Operator.AddrOf) continue;
                var outId = outArg.Expression as Identifier;
                if (outId != null)
                    SetValue(outId, Constant.Invalid);

            }
            return Constant.Invalid;
        }

        public void RemoveIdentifierUse(Identifier id)
        {
        }

        public void UseExpression(Expression e)
        {
        }

        #endregion

    }
}
