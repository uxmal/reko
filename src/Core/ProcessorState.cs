#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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

using Reko.Core.Code;
using Reko.Core.Operators;
using Reko.Core.Expressions;
using Reko.Core.Rtl;
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using Reko.Core.Types;

namespace Reko.Core
{
    /// <summary>
    /// ProcessorState simulates the state of the processor and a part of the
    /// stack during scanning.
    /// </summary>
    public abstract class ProcessorState : EvaluationContext
    {
        private Dictionary<RegisterStorage, Expression> linearDerived;
        private SortedList<int, Expression> stackState;

        public ProcessorState()
        {
            this.linearDerived = new Dictionary<RegisterStorage, Expression>();
            this.stackState = new SortedList<int, Expression>();
        }

        public ProcessorState(ProcessorState orig)
        {
            this.linearDerived = new Dictionary<RegisterStorage, Expression>(orig.linearDerived);
            this.stackState = new SortedList<int, Expression>(orig.stackState);
            this.ErrorListener = this.ErrorListener;
        }

        /// <summary>
        /// Method to call if an error occurs within the processor state object (such as stack over/underflows).
        /// </summary>
        public Action<string> ErrorListener { get; set; }

        public abstract IProcessorArchitecture Architecture { get; }
        public abstract ProcessorState Clone();

        public abstract Constant GetRegister(RegisterStorage r);
        public abstract void SetRegister(RegisterStorage r, Constant v);
        public abstract void SetInstructionPointer(Address addr);

        public abstract void OnProcedureEntered();                 // Some registers need to be updated when a procedure is entered.
        public abstract void OnProcedureLeft(FunctionType procedureSignature);

        /// <summary>
        /// Captures the the processor's state before calling a procedure.
        /// </summary>
        /// <param name="stackReg">An identifier for the stack register of the processor.</param>
        /// <param name="returnAddressSize">The size of the return address on stack.</param>
        /// <returns>A CallSite object that abstracts the processor state right before the call.</returns>
        public abstract CallSite OnBeforeCall(Identifier stackReg, int returnAddressSize);

        /// <summary>
        /// Perform any adjustments to the processor's state after returning from a procedure call with the
        /// specified signature.
        /// </summary>
        /// <param name="sigCallee">The signature of the called procedure.</param>
        public abstract void OnAfterCall(FunctionType sigCallee);

        private bool IsStackRegister(Expression ea)
        {
            var id = ea as Identifier;
            if (id == null)
                return false;
            var reg = id.Storage as RegisterStorage;
            if (reg == null)
                return false;
            return (reg == Architecture.StackRegister);
        }

        private bool GetStackOffset(Expression ea, out int offset)
        {
            if (IsStackRegister(ea))
            {
                offset = 0;
                return true;
            }

            if (ea is BinaryExpression bin && (bin.Operator == Operator.IAdd || bin.Operator == Operator.ISub) && IsStackRegister(bin.Left))
            {
                if (bin.Right is Constant cOffset)
                {
                    offset = cOffset.ToInt32();
                    if (bin.Operator == Operator.ISub)
                        offset = -offset;
                    return true;
                }
            }
            offset = 0;
            return false;
        }

        public Expression GetValue(Identifier id)
        {
            if (id.Storage is TemporaryStorage)
                return Constant.Invalid;
            var reg = id.Storage as RegisterStorage;
            if (reg == null)
                return Constant.Invalid;

            return GetValue(reg);
        }

        public Expression GetValue(RegisterStorage reg)
        {
            Expression exp = GetRegister(reg);
            if (exp != Constant.Invalid)
                return exp;
            if (linearDerived.TryGetValue(reg, out exp))
                return exp;
            return Constant.Invalid;
        }

        public Expression GetValue(MemoryAccess access, SegmentMap segmentMap)
        {
            if (access.EffectiveAddress is Constant constAddr)
            {
                if (constAddr == Constant.Invalid)
                    return constAddr;
                var ea = Architecture.MakeAddressFromConstant(constAddr, false);
                return GetMemoryValue(ea, access.DataType, segmentMap);
            }
            var addr = access.EffectiveAddress as Address;
            if (addr != null)
            {
                return GetMemoryValue(addr, access.DataType, segmentMap);
            }
            if (GetStackOffset(access.EffectiveAddress, out var stackOffset))
            {
                if (stackState.TryGetValue(stackOffset, out var value))
                    return value;
            }
            return Constant.Invalid;
        }

        public Expression GetValue(SegmentedAccess access, SegmentMap segmentMap)
        {
            if (GetStackOffset(access.EffectiveAddress, out var stackOffset))
            {
                if (stackState.TryGetValue(stackOffset, out var value))
                    return value;
            }
            return Constant.Invalid;
        }

        private Expression GetMemoryValue(Address addr, DataType dt, SegmentMap segmentMap)
        {
            if (!(dt is PrimitiveType pt))
                return Constant.Invalid;
            else if (pt.Domain == Domain.Real && pt.BitSize > 80)
            {
                //$TODO: we can't represent 96- or 128-bit floats quite yet.
                return Constant.Invalid;
            }
            else if (pt.BitSize > 64)
            {
                //$TODO: we can't represent integer constants larger than 64 bits yet.
                return Constant.Invalid;
            }
            if (!segmentMap.TryFindSegment(addr, out ImageSegment seg) || seg.IsWriteable)
                return Constant.Invalid;
            if (!Architecture.TryRead(seg.MemoryArea, addr, pt, out Constant c))
                return Constant.Invalid;
            return c;
        }

        public Expression GetStackValue(int offset)
        {
            if (stackState.TryGetValue(offset, out var value))
                return value;
            return Constant.Invalid;
        }

        public Expression GetValue(Application appl)
        {
            return Constant.Invalid;
        }

        public Expression GetDefiningExpression(Identifier id)
        {
            return null;
        }

        public List<Statement> GetDefiningStatementClosure(Identifier id)
        {
            return new List<Statement>();
        }

        public Expression MakeSegmentedAddress(Constant seg, Constant off)
        {
            return Architecture.MakeSegmentedAddress(seg, off);
        }

        public void RemoveIdentifierUse(Identifier id)
        {
        }

        public void UseExpression(Expression expr)
        {
        }

        public void RemoveExpressionUse(Expression expr)
        {
        }

        public void SetValue(Identifier id, Expression value)
        {
            if (id.Storage is TemporaryStorage)
                return;
            var reg = id.Storage as RegisterStorage;
            if (reg == null)
                return;
            SetValue(reg, value);
        }

        public Constant SetValue(RegisterStorage reg, Expression value)
        {
            if (value is Constant constVal)
            {
                SetRegister(reg, constVal);
                linearDerived.Remove(reg);
                return constVal;
            }
            if (value is BinaryExpression binVal)
            {
                if ((binVal.Operator == Operator.IAdd || binVal.Operator == Operator.ISub) &&
                    binVal.Left is Identifier &&
                    binVal.Right is Constant)
                {
                    SetRegister(reg, Constant.Invalid);
                    linearDerived[reg] = binVal;
                    return Constant.Invalid;
                }
            }
            SetRegister(reg, Constant.Invalid);
            linearDerived.Remove(reg);
            return Constant.Invalid;
        }

        public void SetValueEa(Expression ea, Expression value)
        {
            if (GetStackOffset(ea, out int stackOffset))
                stackState[stackOffset] = value;
        }

        public void SetValueEa(Expression basePtr, Expression ea, Expression value)
        {
            if (GetStackOffset(ea, out int stackOffset))
                stackState[stackOffset] = value;
        }

        public bool IsUsedInPhi(Identifier id)
        {
            return false;
        }
    }
}
