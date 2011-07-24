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
    public class ScannerEvaluationContext : EvaluationContext
    {
        private IProcessorArchitecture arch;
        private Dictionary<MachineRegister, Expression> offsets;
        private SortedList<int, Expression> stackState;

        public ScannerEvaluationContext(IProcessorArchitecture arch)
            : this(arch, arch.CreateProcessorState())
        {
        }

        public ScannerEvaluationContext(IProcessorArchitecture arch, ProcessorState state)
        {
            this.arch = arch;
            this.State = state;
            this.offsets = new Dictionary<MachineRegister, Expression>();
            this.stackState = new SortedList<int, Expression>();
        }

        private ScannerEvaluationContext(ScannerEvaluationContext original)
        {
            this.arch = original.arch;
            this.State = original.State.Clone();
            this.offsets = new Dictionary<MachineRegister, Expression>(original.offsets);
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
         
            Expression exp = State.GetRegister(reg.Register);
            if (exp != Constant.Invalid)
                return exp;
            if (offsets.TryGetValue(reg.Register, out exp))
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
                State.SetRegister(reg.Register, constVal);
                offsets.Remove(reg.Register);
                return;
            }
            var binVal = value as BinaryExpression;
            if (binVal != null)
            {
                if ((binVal.op == Operator.Add || binVal.op == Operator.Sub) &&
                    binVal.Left is Identifier &&
                    binVal.Right is Constant)
                {
                    offsets[reg.Register] = binVal;
                    return;
                }
            }
            State.SetRegister(reg.Register, Constant.Invalid);
            offsets.Remove(reg.Register); 
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
            return (reg.Register == arch.StackRegister);
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
            if (bin != null && (bin.op == Operator.Add || bin.op == Operator.Sub) && IsStackRegister(bin.Left))
            {
                offset = ((Constant) bin.Right).ToInt32();
                if (bin.op == Operator.Sub)
                    offset = -offset;
                return true;
            }
            offset = 0;
            return false;
        }
        #endregion
    }
}
