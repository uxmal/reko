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
    public class ScannerEvaluator : EvaluationContext
    {
        private IProcessorArchitecture arch;
        private ProcessorState state;
        private Dictionary<MachineRegister, Expression> offsets;

        public ScannerEvaluator(IProcessorArchitecture arch, ProcessorState state)
        {
            this.arch = arch;
            this.state = state;
            this.offsets = new Dictionary<MachineRegister, Expression>();
        }

        #region EvaluationContext Members

        public Expression GetValue(Identifier id)
        {
            var reg = id.Storage as RegisterStorage;
            if (reg == null)
                return Constant.Invalid;
         
            Expression exp = state.Get(reg.Register);
            if (exp != Constant.Invalid)
                return exp;
            if (offsets.TryGetValue(reg.Register, out exp))
                return exp;
            return Constant.Invalid;
        }

        public Expression GetValue(MemoryAccess access)
        {
            return Constant.Invalid;
        }

        public Expression GetValue(SegmentedAccess access)
        {
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
                state.Set(reg.Register, constVal);
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
            state.Set(reg.Register, Constant.Invalid);
            offsets.Remove(reg.Register); 
        }

        public void SetValueEa(Expression ea, Expression value)
        {
            var id = ea as Identifier;
            if (id != null)
            {
                var reg = id.Storage as RegisterStorage;
                if (reg == null)
                    return;
                if (reg.Register == arch.StackRegister)
                {
                    throw new NotImplementedException("state.WriteOnStack(0, value);");
                }
            }
        }

        public void SetValueEa(Expression basePtr, Expression ea, Expression value)
        {
        }

        #endregion
    }
}
