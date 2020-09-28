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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.Core;
using System.Diagnostics;
using Reko.Core.Operators;

namespace Reko.Analysis
{
    /// <summary>
    /// Attempts to propagate sequence identifiers to the inputs
    /// and outputs of a procedure.
    /// </summary>
    public class SequenceIdentifierGenerator : InstructionTransformer
    {
        private readonly SsaTransform sst;
        private readonly SsaState ssa;
        private Statement stmCur;

        public SequenceIdentifierGenerator(SsaTransform sst)
        {
            this.sst = sst;
            this.ssa = sst.SsaState;
        }

        public void Transform()
        {
            foreach (var stm in sst.SsaState.Procedure.Statements.ToList())
            {
                this.stmCur = stm;
                stm.Instruction.Accept(this);
            }
        }

        public override Instruction TransformAssignment(Assignment ass)
        {
            if (!(ass.Dst.Storage is SequenceStorage seq))
                return ass;
            var sid = ssa.Identifiers[ass.Dst];
            var stores = sid.Uses
                .Select(u => ClassifyStore(u))
                .Where(u => u != null);
            var grps = from u in stores
                       orderby u.Statement.LinearAddress
                       group u by u.Statement.Block;
            foreach (var grp in grps)
            {
                ProcessAdjacentStores(sid, grp.ToArray());
            }
            return ass;
        }

        private void ProcessAdjacentStores(SsaIdentifier sid, StoreOffset[] storeOffset)
        {
            for (int i = 0; i < storeOffset.Length; ++i)
            {
                if (storeOffset[i] == null)
                    continue;
                var cast1 = GetCastRhs(storeOffset[i].Store);
                var slice1 = GetSliceRhs(storeOffset[i].Store);
                if (cast1 != null || slice1 != null)
                {
                    for (int j = i + 1; j < storeOffset.Length; ++j)
                    {
                        var cast2 = GetCastRhs(storeOffset[j].Store);
                        var slice2 = GetSliceRhs(storeOffset[j].Store);
                        if (cast1 != null && slice2 != null)
                        {
                            ReplaceStores(sid, storeOffset[i], storeOffset[j]);
                            storeOffset[i] = null;
                            storeOffset[j] = null;
                        }
                        else if (slice1 != null && cast2 != null)
                        {
                            throw new NotImplementedException();
                        }
                    }
                }
            }
        }

        private void ReplaceStores(SsaIdentifier sid, StoreOffset stoTail, StoreOffset stoHead)
        {
            if (Operator.Lt.ApplyConstants(stoTail.Offset, stoHead.Offset).ToBoolean())
            {
                stoTail.Store.Dst.DataType = sid.Identifier.DataType;
                stoTail.Store.Src = sid.Identifier;

                ssa.DeleteStatement(stoHead.Statement);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private Slice GetSliceRhs(Store store)
        {
            var slice = store.Src as Slice;
            return slice;
        }

        private Cast GetCastRhs(Store store)
        {
            var cast = store.Src as Cast;
            return cast;
        }

        private class StoreOffset
        {
            public Statement Statement;
            public Store Store;
            public Constant Offset;
        }

        private StoreOffset ClassifyStore(Statement stm)
        {
            if (!(stm.Instruction is Store store))
                return null;
            Expression ea;
            if (store.Dst is MemoryAccess access)
                ea = access.EffectiveAddress;
            else
            {
                if (store.Dst is SegmentedAccess segAccess)
                    ea = segAccess.EffectiveAddress;
                else
                    return null;
            }
            Constant offset = null;
            if (ea is Identifier)
                offset = Constant.Zero(ea.DataType);
            else
            {
                if (ea is BinaryExpression bin)
                {
                    if (bin.Operator == Operator.IAdd || bin.Operator == Operator.ISub)
                    {
                        offset = bin.Right as Constant;
                    }
                }
            }
            if (offset == null)
                return null;
            return new StoreOffset {
                Statement = stm,
                Store = store,
                Offset = offset };
        }

        public override Expression VisitMkSequence(MkSequence seq)
        {
            if (seq.Expressions.Length != 2)
                return seq; //$TODO: do this for longer sequences?
            var idHead = seq.Expressions[0] as Identifier;
            var idTail = seq.Expressions[1] as Identifier;
            if (idHead != null && idTail != null)
            {
                var sidHead = ssa.Identifiers[idHead];
                var sidTail = ssa.Identifiers[idTail];
                if (sidHead.DefStatement.Instruction is DefInstruction &&
                    sidTail.DefStatement.Instruction is DefInstruction)
                {
                    return ReplaceMkSequence(seq, stmCur, sidHead, sidTail);
                }
            }
            return seq;
        }

        private Expression ReplaceMkSequence(MkSequence seq, Statement stmCur, SsaIdentifier sidHead, SsaIdentifier sidTail)
        {
            var idSeq = ssa.Procedure.Frame.EnsureSequence(
                seq.DataType,
                sidHead.OriginalIdentifier.Storage,
                sidTail.OriginalIdentifier.Storage);
            SsaIdentifier sidSeq = EnsureSequenceArgument(idSeq);
            sidSeq.Uses.Add(stmCur);
            RemoveUse(sidHead);
            RemoveUse(sidTail);
            return sidSeq.Identifier;
        }

        private SsaIdentifier EnsureSequenceArgument(Identifier idSeq)
        {
            if (!ssa.Identifiers.TryGetValue(idSeq, out SsaIdentifier sidSeq))
            {
                var b = ssa.Procedure.EntryBlock;
                var def = b.Statements.Add(b.Address.ToLinear(), null);
                sidSeq = ssa.Identifiers.Add(idSeq, null, null, false);
                sidSeq.DefStatement = def;
                def.Instruction = new DefInstruction(sidSeq.Identifier);
            }
            return sidSeq;
        }

        private void RemoveUse(SsaIdentifier sid)
        {
            sid.Uses.Remove(stmCur);
        }
    }
}
