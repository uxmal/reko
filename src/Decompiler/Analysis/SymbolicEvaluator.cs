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
using Decompiler.Core.Expressions;
using Decompiler.Core.Operators;
using System;
using System.Collections.Generic;
using System.Linq;
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
            StackState = new Dictionary<int, Expression>();
            eval = new ExpressionSimplifier(this);
        }

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

        public Dictionary<Expression, Expression> RegisterState { get; private set; }
        public Dictionary<int, Expression> StackState { get; private set; }

        #region InstructionVisitor Members

        void InstructionVisitor.VisitAssignment(Assignment a)
        {
            var valSrc = a.Src.Accept(eval);
            if (valSrc is MemoryAccess)
            {
                RegisterState[a.Dst] = Constant.Invalid;
            }
            else
            {
                RegisterState[a.Dst] = valSrc;
            }
        }

        void InstructionVisitor.VisitBranch(Branch b)
        {
            throw new NotImplementedException();
        }

        void InstructionVisitor.VisitCallInstruction(CallInstruction ci)
        {
            throw new NotImplementedException();
        }

        void InstructionVisitor.VisitDeclaration(Declaration decl)
        {
            throw new NotImplementedException();
        }

        void InstructionVisitor.VisitDefInstruction(DefInstruction def)
        {
            throw new NotImplementedException();
        }

        void InstructionVisitor.VisitGotoInstruction(GotoInstruction gotoInstruction)
        {
            throw new NotImplementedException();
        }

        void InstructionVisitor.VisitPhiAssignment(PhiAssignment phi)
        {
            throw new NotImplementedException();
        }

        void InstructionVisitor.VisitIndirectCall(IndirectCall ic)
        {
            throw new NotImplementedException();
        }

        void InstructionVisitor.VisitReturnInstruction(ReturnInstruction ret)
        {
            throw new NotImplementedException();
        }

        void InstructionVisitor.VisitSideEffect(SideEffect side)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        void InstructionVisitor.VisitUseInstruction(UseInstruction u)
        {
            throw new NotImplementedException();
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
            int offset;
            if (GetStackAddressOffset(access.EffectiveAddress, out offset))
            {
                Expression value;
                if (StackState.TryGetValue(offset, out value))
                    return value;
            }
            return Constant.Invalid;
        }

        public Expression GetValue(SegmentedAccess access)
        {
            int offset;
            if (GetStackAddressOffset(access.EffectiveAddress, out offset))
            {
                Expression value;
                if (StackState.TryGetValue(offset, out value))
                    return value;
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
