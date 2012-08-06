#region License
/* 
 * Copyright (C) 1999-2012 John Källén.
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
using Decompiler.Evaluation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Scanning
{
    /// <summary>
    /// Used by the Scanner and related classes to evaluate possibly constant expressions.
    /// </summary>
    /// <remarks>
    /// This class keeps track of whether registers have been assigned constant values. If so, then
    /// occurrences of r1 = c; r2 = r1 can be replaced with r1 = c; r2 = c. The class also keeps track of derived registers; 
    /// that is, all registers rd which have been assigned rs + c. This allows sequences like r1 = r2 + c; r3 = r2 + d
    /// to be replaced with r1 = r2 + c; r3 = (r1 + c) + d => r1 + (c + d) = r1 + e.
    /// </remarks>
    public class ScannerEvaluationContext : EvaluationContext
    {
        private IProcessorArchitecture arch;
        private Dictionary<RegisterStorage, Expression> linearDerived;
        private SortedList<int, Expression> stackState;

        public ScannerEvaluationContext(IProcessorArchitecture arch)
            : this(arch, arch.CreateProcessorState())
        {
        }

        public ScannerEvaluationContext(IProcessorArchitecture arch, ProcessorState state)
        {
            this.arch = arch;
            this.State = state;
            this.linearDerived = new Dictionary<RegisterStorage, Expression>();
            this.stackState = new SortedList<int, Expression>();
        }

        private ScannerEvaluationContext(ScannerEvaluationContext original)
        {
            this.arch = original.arch;
            this.State = original.State.Clone();
            this.linearDerived = new Dictionary<RegisterStorage, Expression>(original.linearDerived);
            this.stackState = new SortedList<int, Expression>(original.stackState);
        }

        public ProcessorState State { get; private set; }

        public ScannerEvaluationContext Clone()
        {
            return new ScannerEvaluationContext(this);
        }

        #region EvaluationContext Members

        public Expression GetValue(Identifier id)
        {
            var reg = id.Storage as RegisterStorage;
            if (reg == null)
                return Constant.Invalid;
         
            Expression exp = State.GetRegister(reg);
            if (exp != Constant.Invalid)
                return exp;
            if (linearDerived.TryGetValue(reg, out exp))
                return exp;
            return Constant.Invalid;
        }

        public Expression GetValue(MemoryAccess access)
        {
            int stackOffset;
            if (GetStackOffset(access.EffectiveAddress, out stackOffset))
            {
                Expression value;
                if (stackState.TryGetValue(stackOffset, out value))
                    return value;
            }
            return Constant.Invalid;
        }

        public Expression GetValue(SegmentedAccess access)
        {
            int stackOffset;
            if (GetStackOffset(access.EffectiveAddress, out stackOffset))
            {
                Expression value;
                if (stackState.TryGetValue(stackOffset, out value))
                    return value;
            }
            return Constant.Invalid;
        }

        public Expression GetValue(Application appl)
        {
            return Constant.Invalid;
        }

        public void RemoveIdentifierUse(Identifier id)
        {
        }

        public void UseExpression(Expression expr)
        {
        }

        public void SetValue(Identifier id, Expression value)
        {
            var reg = id.Storage as RegisterStorage;
            if (reg == null)
                return;
            var constVal = value as Constant;
            if (constVal != null)
            {
                State.SetRegister(reg, constVal);
                linearDerived.Remove(reg);
                return;
            }
            var binVal = value as BinaryExpression;
            if (binVal != null)
            {
                if ((binVal.Operator == Operator.Add || binVal.Operator == Operator.Sub) &&
                    binVal.Left is Identifier &&
                    binVal.Right is Constant)
                {
                    linearDerived[reg] = binVal;
                    return;
                }
            }
            State.SetRegister(reg, Constant.Invalid);
            linearDerived.Remove(reg); 
        }

        public void SetValueEa(Expression ea, Expression value)
        {
            int stackOffset;
            if (GetStackOffset(ea, out stackOffset))
                stackState[stackOffset] = value;
        }

        private bool IsStackRegister(Expression ea)
        {
            var id = ea as Identifier;
            if (id == null)
                return false;
            var reg = id.Storage as RegisterStorage;
            if (reg == null)
                return false;
            return (reg == arch.StackRegister);
        }

        public void SetValueEa(Expression basePtr, Expression ea, Expression value)
        {
            int stackOffset;
            if (GetStackOffset(ea, out stackOffset))
                stackState[stackOffset] = value;
        }

        private bool GetStackOffset(Expression ea, out int offset)
        {
            if (IsStackRegister(ea))
            {
                offset = 0;
                return true;
            }

            var bin = ea as BinaryExpression;
            if (bin != null && (bin.Operator == Operator.Add || bin.Operator == Operator.Sub) && IsStackRegister(bin.Left))
            {
                offset = ((Constant) bin.Right).ToInt32();
                if (bin.Operator == Operator.Sub)
                    offset = -offset;
                return true;
            }
            offset = 0;
            return false;
        }
        #endregion
    }
}
