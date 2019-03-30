#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Operators;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Analysis
{
    public partial class SsaTransform
    {
        public abstract class IdentifierTransformer
        {
            protected Identifier id;
            protected BitRange liveBits;
            protected readonly Statement stm;
            protected readonly SsaTransform outer;
            protected readonly SsaIdentifierCollection ssaIds;
            protected readonly IDictionary<Block, SsaBlockState> blockstates;

            public IdentifierTransformer(Identifier id, Statement stm, SsaTransform outer)
            {
                this.id = id;
                this.liveBits = id.Storage.GetBitRange();
                this.stm = stm;
                this.ssaIds = outer.ssa.Identifiers;
                this.blockstates = outer.blockstates;
                this.outer = outer;
            }

            public virtual Expression NewUse(SsaBlockState bs)
            {
                var sid = ReadVariable(bs);
                sid.Uses.Add(stm);
                return sid.Identifier;
            }

            public virtual Identifier NewDef(SsaBlockState bs, SsaIdentifier sid)
            {
                return WriteVariable(bs, sid, true);
            }

            /// <summary>
            /// Registers the fact that identifier <paramref name="id"/> is
            /// modified in the block <paramref name="b" /> and generates a 
            /// fresh SSA identifier. 
            /// </summary>
            /// <param name="bs">The block in which the identifier was changed</param>
            /// <param name="sid">The identifier after being SSA transformed.</param>
            /// <param name="performProbe">if true, looks "backwards" to see
            ///   if <paramref name="id"/> overlaps with another identifier</param>
            /// <returns>The new SSA identifier</returns>
            public virtual Identifier WriteVariable(SsaBlockState bs, SsaIdentifier sid, bool performProbe)
            {
                if (bs.currentDef.TryGetValue(id.Storage.Domain, out var prevState))
                {
                    while (prevState != null && id.Storage.Covers(prevState.SsaId.Identifier.Storage))
                    {
                        prevState = prevState.PrevState;
                    }
                }
                bs.currentDef[id.Storage.Domain] = new AliasState(sid, prevState);
                return sid.Identifier;
            }

            /// <summary>
            /// Reaches "backwards" to locate the SSA identifier that defines
            /// the identifier <paramref name="id"/>, starting in block <paramref name="b"/>.
            /// </summary>
            /// If no definition of <paramref name="id"/> is found, a new 
            /// DefStatement is created in the entry block of the procedure.
            /// </summary>
            /// <param name="bs"></param>
            /// <returns>The SSA name of the identifier that was read.</returns>
            public virtual SsaIdentifier ReadVariable(SsaBlockState bs)
            {
                var sid = ReadBlockLocalVariable(bs);
                if (sid != null)
                    return sid;
                // Keep probin'.
                return ReadVariableRecursive(bs);
            }

            /// <summary>
            /// If the `id` has an SsaIdentifier available (not necessarily
            /// defined) in this block, return that SsaIdentifier.
            /// </summary>
            /// <param name="bs">SsaBlockState we look in.</param>
            /// <returns>An SsaIdentifier if it is available, 
            /// otherwise null.
            /// </returns>
            public abstract SsaIdentifier ReadBlockLocalVariable(SsaBlockState bs);

            public SsaIdentifier ReadVariableRecursive(SsaBlockState bs)
            {
                SsaIdentifier val;
                if (bs.Block.Pred.Any(p => !blockstates[p].Visited))
                {
                    // Incomplete CFG
                    val = NewPhi(id, bs.Block);
                    outer.incompletePhis.Add(val);
                }
                else if (bs.Block.Pred.Count == 0)
                {
                    // Undef'ined or unreachable parameter; assume it's a def.
                    val = NewDefInstruction(id, bs.Block);
                }
                else if (bs.Block.Pred.Count == 1)
                {
                    // Search for the variable in the single predecessor.
                    val = ReadVariable(blockstates[bs.Block.Pred[0]]);
                }
                else
                {
                    // Break potential cycles with operandless phi
                    val = NewPhi(id, bs.Block);
                    WriteVariable(bs, val, false);
                    val = AddPhiOperands(val);
                }
                if (val != null)
                    WriteVariable(bs, val, false);
                return val;
            }

            /// <summary>
            /// If <paramref name="idTo"/> is smaller than <paramref name="sidFrom" />, then
            /// it doesn't cover it completely. Therefore, we must generate a SLICE / cast 
            /// statement.
            /// </summary>
            /// <param name="idTo"></param>
            /// <param name="sidFrom"></param>
            /// <returns></returns>
            protected SsaIdentifier MaybeGenerateAliasStatement(AliasState aliasFrom, SsaBlockState bsTo)
            {
                var sidFrom = aliasFrom.SsaId;
                var blockFrom = sidFrom.DefStatement.Block;
                var stgFrom = sidFrom.Identifier.Storage;
                var stgTo = id.Storage;
                DebugEx.PrintIf(trace.TraceVerbose, "  MaybeGenerateAliasStatement({0},{1})", sidFrom.Identifier.Name, id.Name);

                if (stgFrom == stgTo)
                {
                    aliasFrom.Aliases[id] = sidFrom;
                    return aliasFrom.SsaId;
                }

                Expression e = null;
                SsaIdentifier sidUse;
                if (stgFrom.Covers(stgTo))
                {
                    // Defined identifier is "wider" than the storage being 
                    // read. The reader gets a slice of the defined identifier.
                    int offset = stgFrom.OffsetOf(stgTo);
                    if (offset > 0)
                        e = new Slice(id.DataType, sidFrom.Identifier, offset);
                    else
                        e = new Cast(id.DataType, sidFrom.Identifier);
                    sidUse = aliasFrom.SsaId;
                }
                else if (aliasFrom.PrevState != null && aliasFrom.PrevState.SsaId.DefStatement != null)
                {
                    // There is a previous alias, try using that.
                    sidUse = MaybeGenerateAliasStatement(aliasFrom.PrevState, bsTo);
                    e = new DepositBits(sidUse.Identifier, aliasFrom.SsaId.Identifier, (int) stgFrom.BitAddress);
                }
                else
                {
                    this.liveBits = this.liveBits - stgFrom.GetBitRange();
                    DebugEx.PrintIf(trace.TraceVerbose, "  MaybeGenerateAliasStatement proceeding to {0}", blockFrom.Name);
                    sidUse = ReadVariableRecursive(bsTo);
                    e = new DepositBits(sidUse.Identifier, aliasFrom.SsaId.Identifier, (int) stgFrom.BitAddress);
                }

                var ass = new AliasAssignment(id, e);
                var sidAlias = InsertAfterDefinition(sidFrom.DefStatement, ass);
                sidUse.Uses.Add(sidAlias.DefStatement);
                if (e is DepositBits)
                    sidFrom.Uses.Add(sidAlias.DefStatement);
                aliasFrom.Aliases[id] = sidAlias;
                return sidAlias;
            }

            /// <summary>
            /// Inserts the assignment <paramref name="ass"/> before the statement
            /// <paramref name="stm"/> if it is in the same block. Otherwise append
            /// it to the end of the statements in the block.
            /// </summary>
            public SsaIdentifier InsertBeforeStatement(Block block, Statement stm, Assignment ass)
            {
                Statement stmNew;
                if (stm.Block == block)
                {
                    int i = stm.Block.Statements.IndexOf(stm);
                    stmNew = new Statement(stm.LinearAddress, ass, stm.Block);
                    stm.Block.Statements.Insert(i, stmNew);
                }
                else
                {
                    ulong uAddr;
                    int i = -1;
                    if (block.Statements.Count == 0)
                    {
                        uAddr = block.Address.ToLinear();
                    }
                    else
                    {
                        i = block.Statements.Count - 1;
                        if (block.Statements[i].Instruction.IsControlFlow)
                        {
                            --i;
                        }
                        uAddr = block.Statements[i].LinearAddress;
                    }
                    stmNew = block.Statements.Insert(i+1, uAddr, ass);
                }

                var sidTo = ssaIds.Add(ass.Dst, stmNew, ass.Src, false);
                ass.Dst = sidTo.Identifier;
                return sidTo;
            }

            /// <summary>
            /// Inserts the statement <paramref name="ass"/> after the statement
            /// <paramref name="stmBefore"/>, skipping any AliasAssignments that
            /// statements that may have been added after 
            /// <paramref name="stmBefore"/>.
            /// </summary>
            /// <param name="stmBefore"></param>
            /// <param name="ass"></param>
            /// <returns></returns>
            public SsaIdentifier InsertAfterDefinition(Statement stmBefore, AliasAssignment ass)
            {
                var b = stmBefore.Block;
                int i = b.Statements.IndexOf(stmBefore);
                // Skip alias statements
                while (i < b.Statements.Count - 1 && b.Statements[i + 1].Instruction is AliasAssignment)
                    ++i;
                var stm = new Statement(stmBefore.LinearAddress, ass, stmBefore.Block);
                stmBefore.Block.Statements.Insert(i + 1, stm);

                var sidTo = ssaIds.Add(ass.Dst, stm, ass.Src, false);
                ass.Dst = sidTo.Identifier;
                return sidTo;
            }

            /// <summary>
            /// Creates a phi statement with no slots for the predecessor blocks, then
            /// inserts the phi statement as the first statement of the block.
            /// </summary>
            /// <param name="b">Block into which the phi statement is inserted</param>
            /// <param name="v">Destination variable for the phi assignment</param>
            /// <returns>The inserted phi Assignment</returns>
            private SsaIdentifier NewPhi(Identifier id, Block b)
            {
                var phiAss = new PhiAssignment(id);
                var stm = new Statement(b.Address.ToLinear(), phiAss, b);
                b.Statements.Insert(0, stm);
                var sid = ssaIds.Add(phiAss.Dst, stm, phiAss.Src, false);
                phiAss.Dst = sid.Identifier;
                return sid;
            }

            public SsaIdentifier AddPhiOperands(SsaIdentifier phi)
            {
                var preds = phi.DefStatement.Block.Pred;
                var args = preds.Select(p => new PhiArgument(p, ReadVariable(blockstates[p]).Identifier))
                    .ToArray();
                GeneratePhiFunction(phi, args);

                if (!TryRemoveTrivial(phi, out var newSid))
                {
                    // A real phi; use all its arguments.
                    UsePhiArguments(phi);
                    return phi;
                }
                return newSid;
            }

            private static void GeneratePhiFunction(SsaIdentifier phi, PhiArgument[] args)
            {
                ((PhiAssignment) phi.DefStatement.Instruction).Src =
                new PhiFunction(
                        phi.Identifier.DataType,
                        args);
            }

            /// <summary>
            /// If the phi function is trivial, remove it.
            /// </summary>
            /// <param name="phi">SSA identifier of phi function</param>
            /// <param name="sid">
            /// Returns SSA identifier for simplified phi function if it was
            /// trivial, otherwise returns null
            /// </param>
            /// <returns>true if the phi function was trivial</returns>
            private bool TryRemoveTrivial(SsaIdentifier phi, out SsaIdentifier sid)
            {
                var phiFunc = ((PhiAssignment) phi.DefStatement.Instruction).Src;
                DebugEx.PrintIf(trace.TraceVerbose, "  Checking {0} for triviality", phiFunc);
                if (phiFunc.Arguments.All(a => a.Value == phi.Identifier))
                {
                    DebugEx.PrintIf(trace.TraceVerbose, "  {0} is a def", phi.Identifier);
                    // Undef'ined or unreachable parameter; assume it's a def.
                    sid = NewDefInstruction(phi.OriginalIdentifier, phi.DefStatement.Block);
                }
                else
                {
                    sid = SamePhiArgument(phi);
                    if (sid == null)
                        return false;
                }

                // Remember all users except for phi
                var users = phi.Uses.Where(u => u != phi.DefStatement).ToList();

                // Reroute all uses of phi to use same. Remove phi.
                ReplaceBy(phi, sid);

                sid.Uses.RemoveAll(u => u == phi.DefStatement);

                // Remove all phi uses which may have become trivial now.
                DebugEx.PrintIf(trace.TraceVerbose, "Removing {0} and uses {1}", phi.Identifier.Name, string.Join(",", users));
                foreach (var use in users)
                {
                    if (use.Instruction is PhiAssignment phiAss)
                    {
                        var sidU = ssaIds[phiAss.Dst];
                        TryRemoveTrivial(sidU, out var sidNew);
                    }
                }
                sid = EnsureLiveIdentifier(sid);
                phi.DefStatement.Block.Statements.Remove(phi.DefStatement);
                this.outer.sidsToRemove.Add(phi);
                return true;
            }

            /// <summary>
            /// Ensure that identifier is alive. Simplify phi function until
            /// identifier is not in the list of removed phi identifiers
            /// </summary>
            /// <param name="sid">SSA identifier of phi function</param>
            /// <returns>
            /// Returns original SSA identifier or result of phi function
            /// simplification
            /// </returns>
            private SsaIdentifier EnsureLiveIdentifier(SsaIdentifier sid)
            {
                while (this.outer.sidsToRemove.Contains(sid))
                {
                    sid = SamePhiArgument(sid);
                }
                return sid;
            }

            /// <summary>
            /// If the arguments of phi function are equal, return it.
            /// </summary>
            /// <param name="phi">SSA identifier of phi function</param>
            /// <returns>
            /// If the arguments of phi function are equal, return it.
            /// Return null otherwise.
            /// </returns>
            private SsaIdentifier SamePhiArgument(SsaIdentifier phi)
            {
                Identifier same = null;
                var phiFunc = ((PhiAssignment) phi.DefStatement.Instruction).Src;
                foreach (var de in phiFunc.Arguments)
                {
                    var op = (Identifier) de.Value;
                    if (op == same || op == phi.Identifier)
                        continue;
                    if (same != null)
                        return null;
                    same = op;
                }
                return ssaIds[same];
            }

            private void UsePhiArguments(SsaIdentifier phi)
            {
                var phiFunc = ((PhiAssignment) phi.DefStatement.Instruction).Src;
                foreach (var de in phiFunc.Arguments)
                {
                    var id = (Identifier) de.Value;
                    ssaIds[id].Uses.Add(phi.DefStatement);
                }
            }

            /// <summary>
            /// Generate a 'def' instruction for identifiers that are used
            /// without any previous definitions inside the current procedure.
            /// </summary>
            /// <remarks>
            /// The generated def statements show us what identifiers are live-in
            /// to the procedure. Some of the generated def statements are later
            /// eliminated. Typically this is because the identifiers are copied
            /// to the stack and the restored on exit.
            /// </remarks>
            /// <param name="id">Identifier whose definition we wish to generate.</param>
            /// <param name="b">The entry block of the procedure.</param>
            /// <returns></returns>
            public SsaIdentifier NewDefInstruction(Identifier id, Block b)
            {
                var sig = outer.ssa.Procedure.Signature;
                var param = outer.ReadParameter(b, sig, id.Storage);
                if (param != null)
                {
                    var copy = new Assignment(id, param);
                    var stmCopy = b.Statements.Add(b.Address.ToLinear(), copy);
                    var sidCopy = ssaIds.Add(id, stmCopy, null, false);
                    copy.Dst = sidCopy.Identifier;
                    sidCopy.DefExpression = param;

                    outer.ssa.AddUses(stmCopy);
                    return sidCopy;
                }
                return outer.ssa.EnsureSsaIdentifier(id, b);
            }

            private void ReplaceBy(SsaIdentifier sidOld, SsaIdentifier idNew)
            {
                foreach (var use in sidOld.Uses.ToList())
                {
                    use.Instruction.Accept(new IdentifierReplacer(this.ssaIds, use, sidOld.Identifier, idNew.Identifier, false));
                }
                foreach (var bs in outer.blockstates.Values)
                {
                    if (bs.currentDef.TryGetValue(sidOld.Identifier.Storage.Domain, out var alias))
                    {
                        if (alias.SsaId == sidOld)
                        {
                            alias.SsaId = idNew;
                        }
                    }
                    ReplaceStackDefs(bs, sidOld, idNew);
                    foreach (var de in bs.currentFpuDef.ToList())
                    {
                        if (de.Value == sidOld)
                        {
                            bs.currentFpuDef[de.Key] = idNew;
                        }
                    }
                }
            }

            private void ReplaceStackDefs(
                SsaBlockState bs,
                SsaIdentifier sidOld,
                SsaIdentifier sidNew)
            {
                if (!(sidOld.Identifier.Storage is StackStorage stack))
                    return;
                var offsetInterval = Interval.Create(
                    stack.StackOffset,
                    stack.StackOffset + sidOld.Identifier.DataType.Size);
                var ints = bs.currentStackDef
                    .GetIntervalsOverlappingWith(offsetInterval);
                foreach (var de in ints)
                {
                    if (de.Value.SsaId == sidOld)
                    {
                        de.Value.SsaId = sidNew;
                    }
                }
            }
        }



        public class RegisterTransformer : IdentifierTransformer
        {
            public RegisterTransformer(Identifier id, Statement stm, SsaTransform outer)
                : base(id, stm, outer)
            {
            }

            public override SsaIdentifier ReadBlockLocalVariable(SsaBlockState bs)
            {
                DebugEx.PrintIf(trace.TraceVerbose, "  ReadBlockLocalVariable: ({0}, {1}, ({2})", bs.Block.Name, id, this.liveBits);
                if (!bs.currentDef.TryGetValue(id.Storage.Domain, out var alias))
                    return null;

                // Identifier id is defined locally in this block.
                // Has the alias already been calculated?
                for (var a = alias; a != null; a = a.PrevState)
                {
                    DebugEx.PrintIf(trace.TraceVerbose, "    found alias ({0}, {1}, ({2})", bs.Block.Name, a.SsaId.Identifier.Name, string.Join(",", a.Aliases.Select(aa => aa.Value.Identifier.Name)));
                    SsaIdentifier ssaId = a.SsaId;
                    if (a.SsaId.OriginalIdentifier == id ||
                        a.Aliases.TryGetValue(id, out ssaId))
                    {
                        return ssaId;
                    }

                    // Does the alias overlap the probed value?
                    if (a.SsaId.Identifier.Storage.OverlapsWith(id.Storage))
                    {
                        var sid = MaybeGenerateAliasStatement(a, bs);
                        if (sid != null)
                        {
                            bs.currentDef[id.Storage.Domain] = a;
                            return sid;
                        }
                    }
                }
                return null;
            }
        }

        public class FlagGroupTransformer : IdentifierTransformer
        {
            private readonly uint flagMask;
            private FlagGroupStorage flagGroup;

            public FlagGroupTransformer(Identifier id, FlagGroupStorage flagGroup, Statement stm, SsaTransform outer)
                : base(id, stm, outer)
            {
                this.flagGroup = flagGroup;
                this.flagMask = flagGroup.FlagGroupBits;
            }

            private Expression OrTogether(IEnumerable<SsaIdentifier> sids, Statement stm)
            {
                Expression e = null;
                foreach (var sid in sids.OrderBy(id => id.Identifier.Name))
                {
                    sid.Uses.Add(stm);
                    if (e == null)
                        e = sid.Identifier;
                    else
                        e = new BinaryExpression(Operator.Or, PrimitiveType.Byte, e, sid.Identifier);
                }
                return e;
            }

            public override SsaIdentifier ReadBlockLocalVariable(SsaBlockState bs)
            {
                if (!bs.currentDef.TryGetValue(flagGroup.FlagRegister.Domain, out var alias))
                    return null;

                // Defined locally in this block.
                // Has the alias already been calculated?
                for (var a = alias; a != null; a = a.PrevState)
                {
                    SsaIdentifier ssaId = a.SsaId;
                    if (a.SsaId.OriginalIdentifier == id ||
                        a.Aliases.TryGetValue(id, out ssaId))
                    {
                        return ssaId;
                    }

                    // Does ssaId cover the probed value?
                    if (a.SsaId.Identifier.Storage.OverlapsWith(this.flagGroup))
                    {
                        var sid = MaybeGenerateAliasStatement(a);
                        return sid;
                    }
                }
                return null;
            }

            /// <summary>
            /// If the defining statement doesn't exactly match the bits of
            /// the using statements, we have to generate an alias assignment
            /// after the defining statement.
            /// </summary>
            /// <returns></returns>
            protected SsaIdentifier MaybeGenerateAliasStatement(AliasState aliasFrom)
            {
                var sidFrom = aliasFrom.SsaId;
                var b = sidFrom.DefStatement.Block;
                var stgUse = id.Storage;
                var stgFrom = (FlagGroupStorage) sidFrom.Identifier.Storage;
                if (stgFrom == stgUse)
                {
                    // Exact match, no need for alias statement.
                    aliasFrom.Aliases[id] = sidFrom;
                    return sidFrom;
                }

                Expression e = null;
                SsaIdentifier sidUse;
                if (stgFrom.Covers(stgUse))
                {
                    // No merge needed, since all bits used 
                    // are defined by sidDef.
                    int offset = Bits.Log2(this.flagMask);
                    e = new Slice(PrimitiveType.Bool, sidFrom.Identifier, offset);
                    sidUse = sidFrom;
                }
                else
                {
                    // Not all bits were set by the definition, find
                    // the remaining bits by masking off the 
                    // defined ones.
                    var grf = this.flagGroup.FlagGroupBits & ~stgFrom.FlagGroupBits;
                    if (grf == 0)
                        return null;

                    var oldGrf = this.flagGroup;
                    var oldId = this.id;
                    this.flagGroup = outer.arch.GetFlagGroup(oldGrf.FlagRegister, grf);
                    this.id = outer.ssa.Procedure.Frame.EnsureFlagGroup(this.flagGroup);
                    if (aliasFrom.PrevState != null && aliasFrom.PrevState.SsaId.DefStatement != null)
                    {
                        sidUse = MaybeGenerateAliasStatement(aliasFrom.PrevState);
                    }
                    else
                    {
                        this.liveBits = this.liveBits - stgFrom.GetBitRange();
                        sidUse = ReadVariableRecursive(blockstates[aliasFrom.SsaId.DefStatement.Block]);
                    }

                    this.flagGroup = oldGrf;
                    this.id = oldId;
                    e = new BinaryExpression(
                        Operator.Or,
                        PrimitiveType.Bool,
                        sidFrom.Identifier,
                        sidUse.Identifier);
                }

                var ass = new AliasAssignment(id, e);
                var sidAlias = InsertAfterDefinition(sidFrom.DefStatement, ass);
                sidUse.Uses.Add(sidAlias.DefStatement);
                if (e is BinaryExpression)
                    sidFrom.Uses.Add(sidAlias.DefStatement);
                aliasFrom.Aliases[id] = sidAlias;
                return sidAlias;
            }
        }

        public class StackTransformer : IdentifierTransformer
        {
            private Interval<int> offsetInterval;

            public StackTransformer(
                Identifier id,
                int stackOffset,
                Statement stm,
                SsaTransform outer)
                : base(id, stm, outer)
            {
                this.offsetInterval = Interval.Create(
                    stackOffset,
                    stackOffset + id.DataType.Size);
            }

            public override Expression NewUse(SsaBlockState bs)
            {
                var sid = ReadVariable(bs);
                sid.Uses.Add(stm);
                return sid.Identifier;
            }

            public override Identifier NewDef(SsaBlockState bs, SsaIdentifier sid)
            {
                return WriteVariable(bs, sid, true);
            }

            public override SsaIdentifier ReadBlockLocalVariable(SsaBlockState bs)
            {
                var ints = bs.currentStackDef.GetIntervalsOverlappingWith(offsetInterval)
                    .OrderBy(i => i.Key.Start)
                    .ThenBy(i => i.Key.End - i.Key.Start)
                    .Select(SliceAndShift)
                    .ToArray();
                if (ints.Length == 0)
                    return null;


                // Part of 'id' is defined locally in this block. We now 
                // walk across the bits of 'id'.
                int offsetLo = this.offsetInterval.Start;
                int offsetHi = this.offsetInterval.End;
                var sequence = new List<(SsaIdentifier sid, Interval<int> interval)>();
                foreach (var (sid, expr, intFrom) in ints)
                {
                    if (offsetLo < intFrom.Start)
                    {
                        // Found a gap in the interval tree that isn't defined
                        // in this block. Look for the gap in another block.
                        var intv = Interval.Create(offsetLo, intFrom.Start);
                        var sidR = ReadIntervalRecursive(bs, intv);
                        sequence.Add((sidR, intv));
                        offsetLo = intFrom.Start;
                    }
                    if (offsetLo < intFrom.End)
                    {
                        var subInt = new Interval<int>(offsetLo, Math.Min(intFrom.End, offsetHi));
                        sequence.Add((sid, subInt));
                        offsetLo = intFrom.End;
                    }
                }
                if (offsetLo < offsetHi)
                {
                    var intv = Interval.Create(offsetLo, offsetHi);
                    var sidR = ReadIntervalRecursive(bs, intv);
                    sequence.Add((sidR, intv));
                }
                if (sequence.Count == 1)
                {
                    var sidTo = MakeSequenceElement(bs, sequence[0]);
                    return sidTo;
                }
                else
                {
                     var seq = outer.arch.Endianness.MakeSequence(
                        this.id.DataType, 
                        sequence.Select(e => (Expression) MakeSequenceElement(bs, e).Identifier)
                                            .ToArray());
                    var assSeq = new Assignment(id, seq);
                    SsaIdentifier sidTo = InsertBeforeStatement(bs.Block, this.stm, assSeq);

                    foreach (Identifier item in seq.Expressions)
                    {
                        outer.ssa.Identifiers[item].Uses.Add(sidTo.DefStatement);
                    }
                    return sidTo;
                }
            }

            private SsaIdentifier ReadIntervalRecursive(
                SsaBlockState bs,
                Interval<int> intv)
            {
                var curInteval = this.offsetInterval;
                var curId = this.id;
                var bitSize = (intv.End - intv.Start) * DataType.BitsPerByte;
                this.id = outer.ssa.Procedure.Frame.EnsureStackVariable(
                    intv.Start,
                    PrimitiveType.CreateWord(bitSize));
                this.offsetInterval = intv;
                var sid = ReadVariableRecursive(bs);
                this.id = curId;
                this.offsetInterval = curInteval;
                return sid;
            }

            private SsaIdentifier MakeSequenceElement(SsaBlockState bs, (SsaIdentifier sid, Interval<int> interval) elem)
            {
                var stg = (StackStorage) elem.sid.Identifier.Storage;
                var start = stg.StackOffset;
                var end = start + stg.DataType.Size;
                if (start == elem.interval.Start &&
                    end == elem.interval.End)
                {
                    // Exact match
                    return elem.sid;
                }
                var sidSlice = EnsureSliceStatement(elem.sid, elem.interval, elem.interval.Start - start, bs);
                return sidSlice;
            }

            /// <summary>
            /// Create a slice statement to extract a subinterval from, unless one already exists, in
            /// which case we reuse it.
            /// <paramref name="sidFrom"/>.
            /// </summary>
            /// <remarks>
            /// The source, or defined identifer is "wider" than the destinatior or
            /// used storage. We must provide a slice of the defined identifier.
            /// </remarks>
            private SsaIdentifier EnsureSliceStatement(SsaIdentifier sidFrom, Interval<int> intv, int offset, SsaBlockState bs)
            {
                var bitSize = (intv.End - intv.Start) * DataType.BitsPerByte;
                var bitOffset = offset * DataType.BitsPerByte;
                var idSlice = outer.ssa.Procedure.Frame.EnsureStackVariable(intv.Start, PrimitiveType.CreateWord(bitSize));
                var e = this.outer.arch.Endianness.MakeSlice(idSlice.DataType, sidFrom.Identifier, bitOffset);

                // Find existing alias of that size.
                foreach (var use in sidFrom.Uses)
                {
                    if (use.Instruction is AliasAssignment alias && 
                        alias.Src is Slice slice && 
                        slice.Offset == e.Offset && 
                        slice.DataType.BitSize == e.DataType.BitSize)
                    {
                        return ssaIds[alias.Dst];
                    }
                }
                var sidUse = sidFrom;

                //$TODO: perhaps this alias has already been computed?
                var ass = new AliasAssignment(idSlice, e);
                var sidAlias = InsertAfterDefinition(sidFrom.DefStatement, ass);
                sidUse.Uses.Add(sidAlias.DefStatement);
                return sidAlias;
            }

            private (SsaIdentifier, Expression, Interval<int>) SliceAndShift(KeyValuePair<Interval<int>, AliasState> arg)
            {
                return (
                    arg.Value.SsaId,
                    arg.Value.SsaId.Identifier,
                    arg.Key.Intersect(this.offsetInterval));
            }

            public override Identifier WriteVariable(SsaBlockState bs, SsaIdentifier sid, bool performProbe)
            {
                var ints = bs.currentStackDef
                    .GetIntervalsOverlappingWith(offsetInterval)
                    .ToArray();
                foreach (var i in ints)
                {
                    bs.currentStackDef.Delete(i.Key);
                    if (!this.offsetInterval.Covers(i.Key))
                    {
                        // Some of the bits of interval `i` will shine through
                        if (i.Key.End > offsetInterval.Start && i.Key.Start < offsetInterval.Start)
                        {
                            var newInt = Interval.Create(i.Key.Start, offsetInterval.Start);
                            bs.currentStackDef.Add(newInt, i.Value);
                        }
                        if (i.Key.Start < offsetInterval.End && offsetInterval.End < i.Key.End)
                        {
                            var newInt = Interval.Create(offsetInterval.End, i.Key.End);
                            bs.currentStackDef.Add(newInt, i.Value);
                        }
                    }
                }
                bs.currentStackDef.Add(this.offsetInterval, new AliasState(sid, null));
                return sid.Identifier;
            }
        }

        public class SequenceTransformer : IdentifierTransformer
        {
            private SequenceStorage seq;

            public SequenceTransformer(
                Identifier id,
                SequenceStorage seq,
                Statement stm,
                SsaTransform outer)
                : base(id, stm, outer)
            {
                this.seq = seq;
            }

            public override SsaIdentifier ReadVariable(SsaBlockState bs)
            {
                var sids = seq.Elements
                    .Select(e =>
                    {
                        var ss = outer.factory.Create(outer.ssa.Procedure.Frame.EnsureIdentifier(e), stm);
                        var sid = ss.ReadVariable(bs);
                        return sid;
                    })
                    .ToArray();
                return Fuse(sids);
            }

            public override SsaIdentifier ReadBlockLocalVariable(SsaBlockState bs)
            {
                // We shouldn't reach this, as ReadVariable above should have 
                // broken the sequence into ReadVariable calls to the components.
                throw new InvalidOperationException();
            }

            public SsaIdentifier Fuse(SsaIdentifier [] sids)
            {
                if (sids.Length == 2 && 
                    sids[0].DefStatement.Instruction is AliasAssignment aassHead &&
                    sids[1].DefStatement.Instruction is AliasAssignment aassTail)
                {
                    if (aassHead.Src is Slice eHead && aassTail.Src is Cast eTail)
                    {
                        return ssaIds[(Identifier) eHead.Expression];
                    }
                }
                if (sids.All(s => s.DefStatement.Instruction is DefInstruction))
                {
                    // All subregisters came in from caller, so create an
                    // alias statement.
                    var seq = new MkSequence(this.id.DataType, sids.Select(s => s.Identifier).ToArray());
                    var ass = new AliasAssignment(id, seq);
                    var stm = sids[0].DefStatement.Block.Statements.Add(
                        sids[0].DefStatement.LinearAddress,
                        ass);
                    var sidTo = ssaIds.Add(ass.Dst, stm, ass.Src, false);
                    ass.Dst = sidTo.Identifier;
                    foreach (var sid in sids)
                    {
                        sid.Uses.Add(stm);
                    }
                    return sidTo;
                }

                if (sids.Length == 2 && 
                    sids[0].DefStatement.Instruction is Assignment assHead &&
                    sids[1].DefStatement.Instruction is Assignment assTail)
                {
                    // If x_2 = Slice(y_3); z_4 = (cast) y_3 return y_3
                    if (assHead.Src is Slice slHead &&
                        assTail.Src is Cast caTail &&
                        slHead.Expression == caTail.Expression &&
                        slHead.Expression is Identifier id)
                    {
                        return ssaIds[id];
                    }
                }

                // Unrelated assignments; insert alias right before use.
                return FuseIntoMkSequence(sids);
            }

            private SsaIdentifier FuseIntoMkSequence(SsaIdentifier [] sids)
            {
                var seq = new MkSequence(this.id.DataType, sids.Select(s => s.Identifier).ToArray());
                var ass = new AliasAssignment(this.id, seq);
                var iStm = this.stm.Block.Statements.IndexOf(this.stm);
                var stm = this.stm.Block.Statements.Insert(iStm, this.stm.LinearAddress, ass);
                var sidTo = ssaIds.Add(ass.Dst, stm, ass.Src, false);
                ass.Dst = sidTo.Identifier;
                foreach (var sid in sids)
                {
                    sid.Uses.Add(stm);
                }
                return sidTo;
            }

            public override Identifier WriteVariable(SsaBlockState bs, SsaIdentifier sid, bool performProbe)
            {
                foreach (var stg in seq.Elements)
                {
                    var ss = outer.factory.Create(outer.ssa.Procedure.Frame.EnsureIdentifier(stg), stm);
                    ss.WriteVariable(bs, sid, performProbe);
                }
                return sid.Identifier;
            }
        }

        public class FpuStackTransformer : IdentifierTransformer
        {
            private FpuStackStorage fpu;

            public FpuStackTransformer(Identifier id, FpuStackStorage fpu, Statement stm, SsaTransform outer) : base(id, stm, outer)
            {
                this.fpu = fpu;
            }

            public override Identifier NewDef(SsaBlockState bs, SsaIdentifier sid)
            {
                bs.currentFpuDef[fpu.FpuStackOffset] = sid;
                return base.NewDef(bs, sid);
            }

            public override SsaIdentifier ReadBlockLocalVariable(SsaBlockState bs)
            {
                bs.currentFpuDef.TryGetValue(fpu.FpuStackOffset, out var sid);
                return sid;
            }

            public override Identifier WriteVariable(SsaBlockState bs, SsaIdentifier sid, bool performProbe)
            {
                bs.currentFpuDef[fpu.FpuStackOffset] = sid;
                return sid.Identifier;
            }
        }
    }
}
