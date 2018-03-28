#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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

using Reko.Analysis;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Operators;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.IO;
namespace Reko.Evaluation
{
    public class SymbolicEvaluationContext : EvaluationContext
    {
        private IProcessorArchitecture arch;
        private StorageValueSetter setter;

        public SymbolicEvaluationContext(IProcessorArchitecture arch, Frame frame)
        {
            this.arch = arch;
            this.Frame = frame;
            this.RegisterState = new Dictionary<Storage, Expression>();
            this.StackState = new SortedList<int, Expression>();
            this.TemporaryState = new Dictionary<Storage, Expression>();
            this.setter = new StorageValueSetter(this);
        }

        private SymbolicEvaluationContext(SymbolicEvaluationContext old)
        {
            this.arch = old.arch;
            this.Frame = old.Frame;
            this.RegisterState = new Dictionary<Storage, Expression>(old.RegisterState);
            this.StackState = new SortedList<int, Expression>(old.StackState);
            this.TemporaryState = new Dictionary<Storage, Expression>(old.TemporaryState);
            this.TrashedFlags = old.TrashedFlags;
            this.setter = new StorageValueSetter(this);
        }

        //$REVIEW: make all states a single collection indexed by storage, and eliminate the map?

        public Dictionary<Storage, Expression> RegisterState { get; private set; }
        public SortedList<int, Expression> StackState { get; private set; }
        public Dictionary<Storage, Expression> TemporaryState { get; private set; }
        public uint TrashedFlags { get; set; }
        public Frame Frame { get; private set; }

        public SymbolicEvaluationContext Clone()
        {
            return new SymbolicEvaluationContext(this);
        }

        public void Emit(IProcessorArchitecture arch, string prefix, TextWriter writer)
        {
            writer.Write(prefix);
            foreach (var de in RegisterState)
            {
                writer.Write(" {0}:{1}", de.Key, de.Value);
            }
            foreach (var de in StackState)
            {
                writer.Write(" {0}(fp):{1}", de.Key, de.Value);
            }
        }

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
            var local = id.Storage as StackLocalStorage;
            if (local != null)
                return GetStackValue(local.StackOffset, local.DataType);

            //$REVIEW: this is cheating a little; some flags could
            // actually have been set to 0 or 1. The problem is we
            // are doing "poor man's value propagation", and should
            // really be doing this after SSA transformation has been
            // done on the code.
            if (id.Storage is FlagGroupStorage)
                return Constant.Invalid;
            return id;
        }

        public Expression GetValue(MemoryAccess access, SegmentMap segmentMap)
        {
            return GetValueEa(access);
        }

        public Expression GetValue(SegmentedAccess access, SegmentMap segmentMap)
        {
            return GetValueEa(access);
        }

        private Expression GetValueEa(MemoryAccess access)
        {
            int offset;
            if (!GetStackAddressOffset(access.EffectiveAddress, out offset))
                return Constant.Invalid;

            var accessDataType = access.DataType;
            return GetStackValue(offset, access.DataType);
        }

        private Expression GetStackValue(int offset, DataType accessDataType)
        {
            Expression value;
            if (StackState.TryGetValue(offset, out value))
            {
                if (value == Constant.Invalid)
                    return value;
                int excess = accessDataType.Size - value.DataType.Size;
                if (excess == 0)
                    return value;
                if (excess > 0)
                {
                    // Example: word32 fetch from SP+04, where SP+04 is a word16 and SP+06 is a word16
                    int remainder = offset + value.DataType.Size;
                    Expression v2;
                    if (StackState.TryGetValue(remainder, out v2))
                    {
                        if (v2 == Constant.Invalid || value == Constant.Invalid)
                            return Constant.Invalid;
                        if (v2.DataType.Size + value.DataType.Size != accessDataType.Size)
                            return Constant.Invalid;

                        //$BUGBUG: should evaluate the MkSequence, possibly creating a longer constant if v2 and value are 
                        // constant.
                        //$BUGBUG: the sequence below is little-endian!!!
                        return new MkSequence(accessDataType, v2, value);
                    }
                }
                else
                {
                    // Example: fetching a byte from SP+04, which previously was assigned an int32.
                    return new Cast(accessDataType, value);
                }
            }
            else
            {
                int offset2;
                if (StackState.TryGetLowerBoundKey(offset, out offset2))
                {
                    var value2 = StackState[offset2];
                    if (offset2 + value2.DataType.Size > offset)
                        return new Slice(accessDataType, StackState[offset2], ((offset - offset2) * 8));
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
                var outArg = args[i] as OutArgument;
                if (outArg == null)
                    continue;
                var outId = outArg.Expression as Identifier;
                if (outId != null)
                    SetValue(outId, Constant.Invalid);
            } 
            return Constant.Invalid;
        }

        public Expression GetDefiningExpression(Identifier id)
        {
            return null;
        }

        public Expression MakeSegmentedAddress(Constant seg, Constant off)
        {
            return arch.MakeSegmentedAddress(seg, off);
        }

        public void RemoveIdentifierUse(Identifier id)
        {
        }

        public void SetValue(Identifier id, Expression value)
        {
            setter.Set(id.Storage, value);
        }

        public void SetValueEa(Expression ea, Expression value)
        {
            int offset;
            if (GetStackAddressOffset(ea, out offset))
                StackState[offset] = value;
        }

        public void SetValueEa(Expression basePtr, Expression ea, Expression value)
        {
            int offset;
            if (GetStackAddressOffset(ea, out offset))
                StackState[offset] = value;
        }

        public void UseExpression(Expression e)
        {
        }

        public void RemoveExpressionUse(Expression e)
        {
        }

        public bool IsUsedInPhi(Identifier id)
        {
            return false;
        }
        
        /// <summary>
        /// Update the symbolic context with all the registers
        /// modified by calling the function.
        /// </summary>
        /// <param name="pf"></param>
        public void UpdateRegistersTrashedByProcedure(ProcedureFlow pf)
        {
            foreach (var reg in pf.Trashed)
            {
                Constant c;
                if (!pf.Constants.TryGetValue(reg, out c))
                {
                    c = Constant.Invalid;
                }
                RegisterState[reg] = c;
            }
            TrashedFlags |= pf.grfTrashed;
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
                if (!IsFramePointer(ea.Left))
                    return false;
                var o = ea.Right as Constant;
                if (o == null) return false;
                offset = o.ToInt32();
                if (ea.Operator == Operator.ISub)
                    offset = -offset;
                return true;
            }
            else
            {
                return IsFramePointer(effectiveAddress);
            }
        }

        public bool IsFramePointer(Expression exp)
        {
            return exp == Frame.FramePointer;
        }

        private class StorageValueSetter : StorageVisitor<Storage>
        {
            private SymbolicEvaluationContext ctx;
            private Expression value;

            public StorageValueSetter(SymbolicEvaluationContext ctx)
            {
                this.ctx = ctx;
            }

            public void Set(Storage stg, Expression value)
            {
                this.value = value;
                stg.Accept(this);
            }

            #region StorageVisitor<Storage> Members

            public Storage VisitFlagGroupStorage(FlagGroupStorage grf)
            {
                ctx.TrashedFlags |= grf.FlagGroupBits;
                return grf;
            }

            public Storage VisitFpuStackStorage(FpuStackStorage fpu)
            {
                return fpu;
            }

            public Storage VisitMemoryStorage(MemoryStorage global)
            {
                return global;
            }

            public Storage VisitStackLocalStorage(StackLocalStorage local)
            {
                ctx.StackState[local.StackOffset] = value;
                return local;
            }

            public Storage VisitOutArgumentStorage(OutArgumentStorage arg)
            {
                throw new NotImplementedException();
            }

            public Storage VisitRegisterStorage(RegisterStorage reg)
            {
                var validConstant = value is Constant  && value != Constant.Invalid 
                    ? value as Constant
                    : null;
                //$BUGBUG: Must also clear other registers (aliased subregisters)
                //$BUGBUG: but the performance implications are terrible
                var stateToSet = new Dictionary<RegisterStorage, Expression>();
                foreach (Storage other in ctx.RegisterState.Keys)
                {
                    var otherReg = other as RegisterStorage;
                    if (otherReg != null)
                    {
                        if (otherReg.IsSubRegisterOf(reg))
                        {
                            stateToSet[otherReg] = otherReg.GetSlice(value);
                        }
                        else if (reg.IsSubRegisterOf(otherReg))
                        {
                            stateToSet[otherReg] = Constant.Invalid;
                        }
                    }
                }
                if (stateToSet.Count > 0)
                {
                    foreach (var s in stateToSet)
                    {
                        ctx.RegisterState[s.Key] = s.Value;
                    }
                }
                ctx.RegisterState[reg] = value;
                return reg;
            }

            public Storage VisitSequenceStorage(SequenceStorage seq)
            {
                ctx.RegisterState[seq] = value;
                return seq;
            }

            public Storage VisitStackArgumentStorage(StackArgumentStorage stack)
            {
                ctx.StackState[stack.StackOffset] = value;
                return stack;
            }

            public Storage VisitTemporaryStorage(TemporaryStorage temp)
            {
                ctx.TemporaryState[temp] = value;
                return temp;
            }
            #endregion
        }
    }
}
