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
using Decompiler.Core.Lib;
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;

namespace Decompiler.Evaluation
{
    public class SymbolicEvaluationContext : EvaluationContext
    {
        private IProcessorArchitecture arch;
        private StorageValueSetter setter;

        public SymbolicEvaluationContext(IProcessorArchitecture arch)
        {
            this.arch = arch;
            RegisterState = new Dictionary<RegisterStorage, Expression>();
            StackState = new Map<int, Expression>();
            TemporaryState = new Dictionary<Storage, Expression>();
            this.setter = new StorageValueSetter(this);
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
            var local = id.Storage as StackLocalStorage;
            if (local != null)
                return GetStackValue(local.StackOffset, local.DataType);
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

            var accessDataType = access.DataType;
            return GetStackValue(offset, access.DataType);
        }

        private Expression GetStackValue(int offset, DataType accessDataType)
        {
            Expression value;
            if (StackState.TryGetValue(offset, out value))
            {
                int excess = accessDataType.Size - value.DataType.Size;
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
                        return new MkSequence(accessDataType, v2, value);
                    }
                }
                else
                {
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
                        return new Slice(accessDataType, StackState[offset2], (uint)((offset - offset2) * 8));
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
                ctx.TrashedFlags |= grf.FlagGroup;
                return grf;
            }

            public Storage VisitFpuStackStorage(FpuStackStorage fpu)
            {
                throw new NotImplementedException();
            }

            public Storage VisitMemoryStorage(MemoryStorage global)
            {
                throw new NotImplementedException();
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
                ctx.RegisterState[reg] = value;
                return reg;
            }

            public Storage VisitSequenceStorage(SequenceStorage seq)
            {
                throw new NotImplementedException();
            }

            public Storage VisitStackArgumentStorage(StackArgumentStorage stack)
            {
                throw new NotImplementedException();
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
