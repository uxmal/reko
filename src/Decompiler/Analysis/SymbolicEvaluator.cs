#region License
/* 
 * Copyright (C) 1999-2011 John Källén.
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
using Decompiler.Core.Lib;
using Decompiler.Core.Operators;
using System;
using System.Collections.Generic;

namespace Decompiler.Analysis
{
    /// <summary>
    /// Before we have the luxury of SSA, we need to perform some simplifications. This class keeps a context of symbolic
    /// expressions for the different registers.
    /// </summary>
    public class SymbolicEvaluator : InstructionVisitor
    {
        private ExpressionSimplifier eval;
        private EvaluationContext ctx;

        public SymbolicEvaluator(ExpressionSimplifier expSimp, EvaluationContext ctx)
        {
            this.eval = expSimp;
            this.ctx = ctx;
        }

        public SymbolicEvaluator(EvaluationContext ctx) : this(new ExpressionSimplifier(ctx), ctx)
        {
        }


        public void Evaluate(Instruction instr)
        {
            instr.Accept(this);
        }

        //public bool IsTrashed(Storage storage)
        //{
        //    RegisterStorage reg = storage as RegisterStorage;
        //    if (reg != null)
        //    {
        //        Expression exp;
        //        if (RegisterState.TryGetValue(reg, out exp))
        //        {
        //            return exp == Constant.Invalid;
        //        }
        //    }
        //    throw new NotImplementedException();
        //}


        #region InstructionVisitor Members

        void InstructionVisitor.VisitAssignment(Assignment a)
        {
            var valSrc = a.Src.Accept(eval);
            ctx.SetValue(a.Dst, valSrc);
        }

        void InstructionVisitor.VisitBranch(Branch b)
        {
            b.Condition.Accept(eval);
        }

        void InstructionVisitor.VisitCallInstruction(CallInstruction ci)
        {
        }

        void InstructionVisitor.VisitDeclaration(Declaration decl)
        {
            if (decl.Expression != null)
            {
                var value = decl.Expression.Accept(eval);
                ctx.SetValue(decl.Identifier, value);
            }
        }

        void InstructionVisitor.VisitDefInstruction(DefInstruction def)
        {
            throw new NotSupportedException("Def instructions shouldn't have been generated yet.");
        }

        void InstructionVisitor.VisitGotoInstruction(GotoInstruction gotoInstruction)
        {
            // Goto instructions always go to a constant label.
        }

        void InstructionVisitor.VisitPhiAssignment(PhiAssignment phi)
        {
            throw new NotSupportedException("PhiAssignments shouldn't have been generated yet.");
        }

        void InstructionVisitor.VisitIndirectCall(IndirectCall ic)
        {
            ic.Callee.Accept(eval);
        }

        void InstructionVisitor.VisitReturnInstruction(ReturnInstruction ret)
        {
            if (ret.Expression != null)
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
                ctx.SetValueEa(ea, valSrc);
            }
        }

        void InstructionVisitor.VisitSwitchInstruction(SwitchInstruction si)
        {
            si.Expression.Accept(eval);
        }

        void InstructionVisitor.VisitUseInstruction(UseInstruction u)
        {
            throw new NotSupportedException("Use expressions shouldn't have been generated yet.");
        }

        #endregion
    }

    public class SymbolicEvaluationContext : EvaluationContext
    {
        private IProcessorArchitecture arch;

        public SymbolicEvaluationContext(IProcessorArchitecture arch)
        {
            this.arch = arch;
            RegisterState = new Dictionary<RegisterStorage, Expression>();
            StackState = new Map<int, Expression>();
            TemporaryState = new Dictionary<Storage, Expression>();
        }

        //$REVIEW: make all states a single collection indexed by storage, and eliminate the map?
        public Dictionary<RegisterStorage, Expression> RegisterState { get; private set; }
        public Map<int, Expression> StackState { get; private set; }
        public Dictionary<Storage, Expression> TemporaryState { get; private set; }
        public uint TrashedFlags { get; set; }
        

        #region EvaluationContext Members
        /// <summary>
        /// Used by the symbolic evaluator to obtain the symbolic value of <paramref name="id"/>.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Expression GetValue(Identifier id)
        {
            var reg = id.Storage as RegisterStorage;
            Expression value;
            if (reg != null && RegisterState.TryGetValue(reg, out value))
                return value;
            var tmp = id.Storage as TemporaryStorage;
            if (tmp != null && TemporaryState.TryGetValue(tmp, out value))
                return value;
            return id;
        }

        public Expression GetValue(MemoryAccess access)
        {
            return GetValueEa(access);
        }

        public Expression GetValue(SegmentedAccess access)
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
                    // Example: word32 fetch from SP+04, where SP+04 is a word16 and SP+06 is a word16
                    int remainder = offset + value.DataType.Size;
                    Expression v2;
                    if (StackState.TryGetValue(remainder, out v2))
                    {
                        //$BUGBUG: should evaluate the MkSequence, possibly creating a longer constant.
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

        /// <summary>
        /// Used primarily to make sure that register out parameters are trashed appropriately.
        /// </summary>
        /// <param name="appl"></param>
        /// <returns></returns>
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

        public void SetValue(Identifier id, Expression value)
        {
            var reg = id.Storage as RegisterStorage;
            if (reg != null)
            {
                RegisterState[reg] = value;
                return;
            }
            var tmp = id.Storage as TemporaryStorage;
            if (tmp != null)
            {
                TemporaryState[id.Storage] = value;
                return;
            }
            var grf = id.Storage as FlagGroupStorage;
            if (grf != null)
            {
                TrashedFlags |= grf.FlagGroup;
                return;
            }
            throw new NotImplementedException("Unable to set value for identifier " + id);
        }

        public void SetValueEa(Expression ea, Expression value)
        {
            int offset;
            if (GetStackAddressOffset(ea, out offset))
                StackState[offset] = value;
        }

        public void UseExpression(Expression e)
        {
        }

        #endregion

        /// <summary>
        /// Stack addresses are of the pattern <code>stackpointer+/-const</code>.
        /// </summary>
        /// <param name="value">The byte offset of the parameter</param>
        /// <returns>True if the effective address was a stack access, false if not.</returns>
        private bool GetStackAddressOffset(Expression effectiveAddress, out int offset)
        {
            offset = 0;
            var ea = effectiveAddress as BinaryExpression;
            if (ea != null)
            {
                if (!IsStackRegister(ea.Left))
                    return false;
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
    }
}
