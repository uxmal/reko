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
        private Dictionary<RegisterStorage, Expression> linearDerived;
        private SortedList<int, Expression> stackState;

        public ScannerEvaluationContext(IProcessorArchitecture arch)
            : this(arch.CreateProcessorState())
        {
        }

        public ScannerEvaluationContext(ProcessorState state)
        {
            this.State = state;
            this.linearDerived = new Dictionary<RegisterStorage, Expression>();
            this.stackState = new SortedList<int, Expression>();
        }

        private ScannerEvaluationContext(ScannerEvaluationContext original)
        {
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
            return State.GetValue(id);
        }

        public Expression GetValue(MemoryAccess access)
        {
            return State.GetValue(access);
        }

        public Expression GetValue(SegmentedAccess access)
        {
            return State.GetValue(access);
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
            State.SetValue(id, value);
        }

        public void SetValueEa(Expression ea, Expression value)
        {
            State.SetValueEa(ea, value);
        }
        

        [Obsolete]
        private bool IsStackRegister(Expression ea)
        {
            return State.IsStackRegister(ea);
        }


        public void SetValueEa(Expression basePtr, Expression ea, Expression value)
        {
            State.SetValueEa(basePtr, ea, value);
        }

        [Obsolete]
        private bool GetStackOffset(Expression ea, out int offset)
        {
            return State.GetStackOffset(ea, out offset);
        }

        #endregion
    }
}
