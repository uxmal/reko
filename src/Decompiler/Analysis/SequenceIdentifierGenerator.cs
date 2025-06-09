#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

using Reko.Core;
using Reko.Core.Analysis;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using System;
using System.Linq;

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
        private Statement? stmCur;      //$REFACTOR: context var.

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
            if (ass.Dst.Storage is not SequenceStorage seq)
                return ass;
            var sid = ssa.Identifiers[ass.Dst];
            var stores = sid.Uses
                .Select(u => ClassifyStore(u))
                .Where(u => u is not null);
            var grps = from u in stores
                       orderby u.Statement.Address
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
                if (storeOffset[i] is null)
                    continue;
                var slice1a = GetSliceRhs(storeOffset[i].Store);
                var slice1b = GetSliceRhs(storeOffset[i].Store);
                if (slice1a is not null || slice1b is not null)
                {
                    for (int j = i + 1; j < storeOffset.Length; ++j)
                    {
                        var slice2a = GetSliceRhs(storeOffset[j].Store);
                        var slice2b = GetSliceRhs(storeOffset[j].Store);
                        if (slice1a is not null && slice2b is not null)
                        {
                            ReplaceStores(sid, storeOffset[i], storeOffset[j]);
                            storeOffset[i] = null!;
                            storeOffset[j] = null!;
                        }
                        else if (slice1b is not null && slice2a is not null)
                        {
                            throw new NotImplementedException();
                        }
                    }
                }
            }
        }

        private void ReplaceStores(SsaIdentifier sid, StoreOffset stoTail, StoreOffset stoHead)
        {
            if (Operator.Lt.ApplyConstants(stoTail.Offset.DataType, stoTail.Offset, stoHead.Offset).ToBoolean())
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

        private static Slice? GetSliceRhs(Store store)
        {
            var slice = store.Src as Slice;
            return slice;
        }

        private class StoreOffset
        {
            public StoreOffset(Statement stm, Store store, Constant Offset)
            {
                this.Statement = stm;
                this.Store = store;
                this.Offset = Offset;
            }

            public readonly Statement Statement;
            public readonly Store Store;
            public readonly Constant Offset;
        }

        private static StoreOffset? ClassifyStore(Statement stm)
        {
            if (stm.Instruction is not Store store)
                return null;
            Expression ea;
            if (store.Dst is MemoryAccess access)
            {
                ea = access.EffectiveAddress;
                if (ea is SegmentedPointer segptr)
                {
                    ea = segptr.Offset;
                }
            }
            else
                return null;
            Constant? offset = null;
            if (ea is Identifier)
                offset = Constant.Zero(ea.DataType);
            else
            {
                if (ea is BinaryExpression bin)
                {
                    if (bin.Operator.Type.IsAddOrSub())
                    {
                        offset = bin.Right as Constant;
                    }
                }
            }
            if (offset is null)
                return null;
            return new StoreOffset(stm, store, offset);
        }

        public override Expression VisitMkSequence(MkSequence seq)
        {
            if (seq.Expressions.Length != 2)
                return seq; //$TODO: do this for longer sequences?
            if (seq.Expressions[0] is Identifier idHead && 
                seq.Expressions[1] is Identifier idTail)
            {
                var sidHead = ssa.Identifiers[idHead];
                var sidTail = ssa.Identifiers[idTail];
                if (sidHead.DefStatement.Instruction is DefInstruction &&
                    sidTail.DefStatement.Instruction is DefInstruction)
                {
                    return ReplaceMkSequence(seq, stmCur!, sidHead, sidTail);
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
            if (!ssa.Identifiers.TryGetValue(idSeq, out SsaIdentifier? sidSeq))
            {
                var b = ssa.Procedure.EntryBlock;
                var def = b.Statements.Add(b.Address, null!);    //$REFACTOR this to SsaState.AddDefineStatement
                sidSeq = ssa.Identifiers.Add(idSeq, null, false);
                sidSeq.DefStatement = def;
                def.Instruction = new DefInstruction(sidSeq.Identifier);
            }
            return sidSeq;
        }

        private void RemoveUse(SsaIdentifier sid)
        {
            sid.Uses.Remove(stmCur!);
        }
    }
}
