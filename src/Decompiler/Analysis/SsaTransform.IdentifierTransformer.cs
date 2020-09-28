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

using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Analysis
{
    public partial class SsaTransform
    {
        /// <summary>
        /// <see cref="IdentifierTransformer" /> subclasses generate <see cref="SsaIdentifier" />s
        /// for different <see cref="Storage"/> types.
        /// </summary>
        public abstract class IdentifierTransformer
        {
            protected Identifier id;
            protected readonly Statement stm;
            protected readonly SsaTransform outer;
            protected readonly SsaIdentifierCollection ssaIds;
            protected readonly IDictionary<Block, SsaBlockState> blockstates;

            public IdentifierTransformer(Identifier id, Statement stm, SsaTransform outer)
            {
                this.id = id;
                this.stm = stm;
                this.ssaIds = outer.ssa.Identifiers;
                this.blockstates = outer.blockstates;
                this.outer = outer;
            }

            public int Offset { get; internal set; }

            public abstract Identifier WriteVariable(SsaBlockState bs, SsaIdentifier sid);

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
                if (bs.Terminates)
                {
                    // Reko has determined that this block diverges. We fall back to 
                    // getting a variable from the entry block, which is guaranteed to
                    // dominate every other block in the procedure.
                    bs = outer.blockstates[bs.Block.Procedure.EntryBlock];
                }
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
                    WriteVariable(bs, val);
                    val = AddPhiOperands(val);
                }
                if (val != null)
                    WriteVariable(bs, val);
                return val;
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
                    ulong uAddr = block.Address.ToLinear();
                    int i = -1;
                    if (block.Statements.Count > 0)
                    {
                        i = block.Statements.Count - 1;
                        if (block.Statements[i].Instruction.IsControlFlow)
                        {
                            --i;
                        }
                        if (i >= 0)
                        {
                            uAddr = block.Statements[i].LinearAddress;
                        }
                    }
                    stmNew = block.Statements.Insert(i + 1, uAddr, ass);
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
                DebugEx.Verbose(trace, "  Checking {0} for triviality", phiFunc);
                if (phiFunc.Arguments.All(a => a.Value == phi.Identifier))
                {
                    DebugEx.Verbose(trace, "  {0} is a def", phi.Identifier);
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
                DebugEx.Verbose(trace, "Removing {0} and uses {1}", phi.Identifier.Name, string.Join(",", users));
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
                var param = ReadParameter(b, sig, id.Storage);
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
                return outer.ssa.EnsureDefInstruction(id, b);
            }

            private Expression ReadParameter(Block b, FunctionType sig, Storage stg)
            {
                if (!sig.ParametersValid)
                    return null;
                var param = sig.Parameters
                    .FirstOrDefault(p => p.Storage.OverlapsWith(stg));
                if (param == null)
                    return null;
                var sidParam = outer.SsaState.EnsureDefInstruction(param, b);
                var idParam = sidParam.Identifier;
                if (idParam.Storage.BitSize == stg.BitSize)
                    return idParam;
                var dt = PrimitiveType.CreateWord((int) stg.BitSize);
                if (param.Storage.Covers(stg))
                {
                    var slice = this.outer.arch.Endianness.MakeSlice(dt, idParam, idParam.Storage.OffsetOf(stg));
                    return slice;
                }
                else
                {
                    var sidWider = outer.SsaState.EnsureDefInstruction(id, b);
                    // The procedure is reading more bits than the signature 
                    // specifies. This pattern is common; typically the high bits
                    // are ignored in the later parts of the procedure. We 
                    // arbitrarily fill the unused part of the word with zeros. 
                    return outer.m.Dpb(sidWider.Identifier, idParam, 0);
                }
            }

            private void ReplaceBy(SsaIdentifier sidOld, SsaIdentifier sidNew)
            {
                foreach (var use in sidOld.Uses.ToList())
                {
                    use.Instruction.Accept(new IdentifierReplacer(this.ssaIds, use, sidOld.Identifier, sidNew.Identifier, false));
                }
                foreach (var bs in outer.blockstates.Values)
                {
                    if (bs.currentDef.TryGetValue(sidOld.Identifier.Storage.Domain, out var alias))
                    {
                        for (int i = 0; i < alias.Definitions.Count; ++i)
                        {
                            var (sid, range, offset) = alias.Definitions[i];
                            if (sid == sidOld)
                                alias.Definitions[i] = (sidNew, range, offset);
                        }
                        var newDict = alias.ExactAliases
                            .Select(kv => (kv.Key,
                                           kv.Value == sidOld ? sidNew : kv.Value))
                            .ToDictionary(kv => kv.Item1, kv => kv.Item2);
                        alias.ExactAliases = newDict;
                    }
                    if (bs.currentFlagDef.TryGetValue(sidOld.Identifier.Storage.Domain, out var flagAlias))
                    {
                        for (int i = 0; i < flagAlias.Definitions.Count; ++i)
                        {
                            var (sid, grf) = flagAlias.Definitions[i];
                            if (sid == sidOld)
                            {
                                flagAlias.Definitions[i] = (sidNew, grf);
                            }
                        }
                        var newDict = flagAlias.ExactAliases
                           .Select(kv => (kv.Key,
                                          kv.Value == sidOld ? sidNew : kv.Value))
                           .ToDictionary(kv => kv.Item1, kv => kv.Item2);
                        flagAlias.ExactAliases = newDict;
                    }
                    ReplaceStackDefs(bs, sidOld, sidNew);
                    foreach (var de in bs.currentSimpleDef.ToList())
                    {
                        if (de.Value == sidOld)
                        {
                            bs.currentSimpleDef[de.Key] = sidNew;
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
                    .GetIntervalsOverlappingWith(offsetInterval).ToArray();
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
            protected BitRange liveBits;

            public RegisterTransformer(Identifier id, Statement stm, SsaTransform outer)
                : base(id, stm, outer)
            {
                this.liveBits = id.Storage.GetBitRange();
            }

            /// <summary>
            /// Registers the fact that identifier <paramref name="id"/> is
            /// modified in the block <paramref name="b" /> and generates a 
            /// fresh SSA identifier. 
            /// </summary>
            /// <param name="bs">The block in which the identifier was changed</param>
            /// <param name="sid">The identifier after being SSA transformed.</param>
            /// <returns>The new SSA identifier</returns>
            public override Identifier WriteVariable(SsaBlockState bs, SsaIdentifier sid)
            {
                DebugEx.Verbose(trace, "  WriteBlockLocalVariable: ({0}, {1}, ({2})", bs.Block.Name, id, this.liveBits);
                if (!bs.currentDef.TryGetValue(id.Storage.Domain, out var aliasState))
                {
                    aliasState = new AliasState();
                    bs.currentDef.Add(id.Storage.Domain, aliasState);
                }
                var stgDef = id.Storage;
                var defRange = stgDef.GetBitRange();
                for (int i = 0; i < aliasState.Definitions.Count; ++i)
                {
                    var (sidPrev, prevRange, offset) = aliasState.Definitions[i];
                    var stgPrev = sidPrev.Identifier.Storage;
                    if (defRange.Covers(prevRange))
                    {
                        DebugEx.Verbose(trace, "     overwriting: {0}", sidPrev.Identifier);
                        aliasState.Definitions.RemoveAt(i);
                        --i;
                    }
                }
                aliasState.Definitions.Add((sid, defRange, this.Offset));
                DebugEx.Verbose(trace, "     writing: {0}", sid.Identifier);

                var newDict = aliasState.ExactAliases
                    .Where(kv => !kv.Key.OverlapsWith(id.Storage))
                    .ToDictionary(kv => kv.Key, kv => kv.Value);

                if (id.Storage == sid.Identifier.Storage)
                {
                    newDict[id.Storage] = sid;
                }
                aliasState.ExactAliases = newDict;
                return sid.Identifier;
            }

            public override SsaIdentifier ReadBlockLocalVariable(SsaBlockState bs)
            {
                DebugEx.Verbose(trace, "  ReadBlockLocalVariable: ({0}, {1}, ({2})", bs.Block.Name, id, this.liveBits);
                if (!bs.currentDef.TryGetValue(id.Storage.Domain, out var alias))
                    return null;

                // Identifier id is defined locally in this block.
                // Has an exact alias already been calculated?
                if (alias.ExactAliases.TryGetValue(id.Storage, out var sid))
                {
                    DebugEx.Verbose(trace, "    found alias ({0}, {1})", bs.Block.Name, sid.Identifier.Name);
                    return sid;
                }

                // At least some of the bits of 'id' are defined locally in this 
                // block. Walk across the bits of 'id', collecting all parts
                // defined into a sequence.
                int offsetLo = this.liveBits.Lsb;
                int offsetHi = this.liveBits.Msb;
                var sids = new List<(SsaIdentifier, BitRange)>();
                while (offsetLo < offsetHi)
                {
                    var useRange = new BitRange(offsetLo, offsetHi);
                    var (sidElem, usedRange, defRange) = FindIntersectingRegister(alias.Definitions, useRange);
                    if (sidElem == null || offsetLo < usedRange.Lsb)
                    {
                        // Found a gap in the register that wasn't defined in
                        // this basic block. Seek backwards
                        var bitrangeR = sidElem == null
                            ? useRange
                            : new BitRange(offsetLo, usedRange.Lsb);
                        var idR = MakeTmpIdentifier(sidElem, bitrangeR);
                        var rx = new RegisterTransformer(idR, stm, this.outer)
                        {
                            liveBits = bitrangeR
                        };
                        var sidR = rx.ReadVariableRecursive(bs);
                        sids.Add((sidR, bitrangeR));
                        offsetLo = bitrangeR.Msb;
                    }
                    if (sidElem != null)
                    {
                        sids.Add((sidElem, defRange));
                        offsetLo = usedRange.Msb;
                    }
                }
                if (sids.Count == 1)
                {
                    var sidSlice = MakeSlice(sids[0].Item1, sids[0].Item2, this.id);
                    alias.ExactAliases[this.id.Storage] = sidSlice;
                    return sidSlice;
                }
                else
                {
                    sids.Reverse(); // Order sids in big-endian order
                    var elems = new List<Expression>();
                    foreach (var (sidElem, bitrange) in sids)
                    {
                        var idSlice = MakeTmpIdentifier(sidElem, bitrange);
                        var sidSlice = MakeSlice(sidElem, bitrange, idSlice);
                        alias.ExactAliases[sidSlice.OriginalIdentifier.Storage] = sidSlice;
                        elems.Add(sidSlice.Identifier);
                    }
                    var seq = outer.m.Seq(elems.ToArray());
                    var assSeq = new AliasAssignment(id, seq);
                    var sidTo = InsertBeforeStatement(bs.Block, this.stm, assSeq);
                    seq.Accept(new InstructionUseAdder(sidTo.DefStatement, ssaIds));
                    alias.ExactAliases[this.id.Storage] = sidTo;
                    return sidTo;
                }
            }

            /// <summary>
            /// Sweep from the most recently written register to the least, and locate
            /// the first intersection of the read interval in <paramref name="bitLo"/> and 
            /// <paramref name="bitHi"> intersects the written register.
            /// </summary>
            public (SsaIdentifier, BitRange, BitRange) FindIntersectingRegister(List<(SsaIdentifier,BitRange,int)> definitions, BitRange useRange)
            {
                var result = ((SsaIdentifier)null, useRange, default(BitRange));
                var stgUse = this.id.Storage;
                for (int i = definitions.Count-1; i >= 0; --i)
                {
                    var (sid, defRange, offset) = definitions[i];
                    var intersection = defRange.Intersect(useRange);
                    if (!intersection.IsEmpty && (result.Item1 == null || result.Item2.Lsb > intersection.Lsb))
                    {
                        defRange = new BitRange(intersection.Lsb + offset, intersection.Msb + offset);
                        result = (sid, intersection, defRange);
                    }
                }
                return result;
            }

            private Identifier MakeTmpIdentifier(SsaIdentifier sidElem, BitRange bitrange)
            {
                var dtSlice = PrimitiveType.CreateWord(bitrange.Extent);
                var reg = outer.ssa.Procedure.Architecture.GetRegister(id.Storage.Domain, bitrange);
                var frame = outer.ssa.Procedure.Frame;
                if (reg.GetBitRange() != bitrange)
                {
                    // We're slicing an architectural register, so we need another name for it.
                    reg = new RegisterStorage(
                        $"{reg.Name}_{bitrange.Extent}_{bitrange.Lsb}",
                        (int) reg.Domain,
                        (uint)bitrange.Lsb,
                        PrimitiveType.CreateWord(bitrange.Extent));
                }
                return frame.EnsureRegister(reg);
            }

            private SsaIdentifier MakeSlice(SsaIdentifier sidSrc, BitRange range, Identifier idSlice)
            {
                if (range.Covers(sidSrc.Identifier.Storage.GetBitRange()))
                    return sidSrc;
                var e = outer.m.Slice(idSlice.DataType, sidSrc.Identifier, range.Lsb);
                var ass = new AliasAssignment(idSlice, e);
                var sidAlias = InsertAfterDefinition(sidSrc.DefStatement, ass);
                sidSrc.Uses.Add(sidAlias.DefStatement);
                return sidAlias;
            }
        }

        public class FlagGroupTransformer : IdentifierTransformer
        {
            private readonly uint flagMask;
            private FlagGroupStorage flagGroup;

            public FlagGroupTransformer(Identifier id, FlagGroupStorage flagGroup, Statement stm, SsaTransform outer)
                : this(id, flagGroup, stm, outer, flagGroup.FlagGroupBits)
            {
            }

            public FlagGroupTransformer(Identifier id, FlagGroupStorage flagGroup, Statement stm, SsaTransform outer, uint flagMask)
                : base(id, stm, outer)
            {
                this.flagGroup = flagGroup;
                this.flagMask = flagMask;
            }

            /// <summary>
            /// Registers the fact that identifier <paramref name="id"/> is
            /// modified in the block <paramref name="b" /> and generates a 
            /// fresh SSA identifier. 
            /// </summary>
            /// <param name="bs">The block in which the identifier was changed</param>
            /// <param name="sid">The identifier after being SSA transformed.</param>
            /// <returns>The new SSA identifier</returns>
            public override Identifier WriteVariable(SsaBlockState bs, SsaIdentifier sid)
            {
                DebugEx.Verbose(trace, "  WriteBlockLocalVariable: ({0}, {1}, ({2:X8})", bs.Block.Name, id, this.flagMask);
                if (!bs.currentFlagDef.TryGetValue(id.Storage.Domain, out var aliasState))
                {
                    aliasState = new FlagAliasState();
                    bs.currentFlagDef.Add(id.Storage.Domain, aliasState);
                }
                for (int i = 0; i < aliasState.Definitions.Count; ++i)
                {
                    if ((aliasState.Definitions[i].Item2 & ~flagMask) == 0)
                    {
                        DebugEx.Verbose(trace, "     overwriting: {0} {1:X8}",
                            aliasState.Definitions[i].Item1.Identifier,
                            aliasState.Definitions[i].Item2);
                        aliasState.Definitions.RemoveAt(i);
                        --i;
                    }
                }
                aliasState.Definitions.Add((sid, flagMask));
                DebugEx.Verbose(trace, "     writing: {0} {1:X8}", sid.Identifier, flagMask);

                var newDict = aliasState.ExactAliases
                    .Where(kv => !kv.Key.Storage.OverlapsWith(id.Storage))
                    .ToDictionary(kv => kv.Key, kv => kv.Value);
                newDict[id] = sid;
                aliasState.ExactAliases = newDict;
                return sid.Identifier;
            }

            public override SsaIdentifier ReadBlockLocalVariable(SsaBlockState bs)
            {
                if (!bs.currentFlagDef.TryGetValue(flagGroup.FlagRegister.Domain, out var alias))
                    return null;

                // Defined locally in this block.
                // Has the alias already been calculated?
                if (alias.ExactAliases.TryGetValue(this.id, out var sid))
                    return sid;
                var sids = new List<(FlagAliasState, SsaIdentifier,uint)>();
                var mask = this.flagMask;
                for (int i = alias.Definitions.Count - 1; i >= 0; --i)
                {
                    var (sidElem, maskElem) = alias.Definitions[i];
                    if ((maskElem & mask) != 0)
                    {
                        var flagGroup = outer.arch.GetFlagGroup(this.flagGroup.FlagRegister, maskElem & mask);
                        var idElem = outer.ssa.Procedure.Frame.EnsureFlagGroup(flagGroup);
                        sids.Add((alias, sidElem, maskElem & mask));
                        mask &= ~maskElem;
                    }
                }
                if (mask != 0)
                {
                    var fx = new FlagGroupTransformer(this.id, this.flagGroup, this.stm, this.outer, mask);
                    var sidR = fx.ReadVariableRecursive(bs);
                    sids.Add((alias, sidR, mask));
                }
                if (sids.Count == 1)
                {
                    var sidSlice = MakeSlice(sids[0]);
                    alias.ExactAliases[sidSlice.OriginalIdentifier] = sidSlice;
                    return sidSlice;
                }
                else
                {
                    // Or'em
                    var slices = sids.Select(MakeSlice).ToArray();
                    var e = slices.Skip(1).Aggregate(
                        (Expression) slices.First().Identifier, 
                        (a, b) => outer.m.Or(a, b.Identifier));
                    var ass = new AliasAssignment(id, e);
                    var sidTo = InsertBeforeStatement(bs.Block, stm, ass);
                    e.Accept(new InstructionUseAdder(sidTo.DefStatement, ssaIds));
                    return sidTo;
                }
            }

            private SsaIdentifier MakeSlice((FlagAliasState alias, SsaIdentifier sid, uint mask) elem)
            {
                var grfFrom = (FlagGroupStorage) elem.sid.Identifier.Storage;
                if (grfFrom.FlagGroupBits == elem.mask)
                    return elem.sid;
                if (elem.alias.ExactAliases.TryGetValue(elem.sid.Identifier, out var sid))
                    return sid;
                int offset = Bits.Log2(this.flagMask);
                var e = outer.m.Slice(PrimitiveType.Bool, elem.sid.Identifier, offset);
                this.flagGroup = outer.arch.GetFlagGroup(grfFrom.FlagRegister, elem.mask);
                var idSlice = outer.ssa.Procedure.Frame.EnsureFlagGroup(this.flagGroup);
                var ass = new AliasAssignment(idSlice, e);
                var sidSlice = InsertAfterDefinition(elem.sid.DefStatement, ass);
                elem.sid.Uses.Add(sidSlice.DefStatement);
                return sidSlice;
            }

            /// <summary>
            /// If the defining statement doesn't exactly match the bits of
            /// the using statements, we have to generate an alias assignment
            /// after the defining statement.
            /// </summary>
            /// <returns></returns>
            protected SsaIdentifier MaybeGenerateAliasStatement(SsaIdentifier sidFrom, FlagGroupStorage stgUse)
            {
                var stgFrom = (FlagGroupStorage) sidFrom.Identifier.Storage;
                if (stgFrom == stgUse)
                {
                    // Exact match, no need for alias statement.
                    return sidFrom;
                }
                throw new NotImplementedException();
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

            public override SsaIdentifier ReadBlockLocalVariable(SsaBlockState bs)
            {
                var ints = bs.currentStackDef.GetIntervalsOverlappingWith(offsetInterval)
                    .OrderBy(i => i.Key.Start)
                    .ThenBy(i => i.Key.End - i.Key.Start)
                    .Select(SliceAndShift)
                    .ToArray();
                if (ints.Length == 0)
                    return null;

                // Try to find an existing alias first. If we can find one, we 
                // return that and quit early.
                foreach (var use in ints.SelectMany(i => i.Item1.Uses))
                {
                    if (use.Instruction is AliasAssignment alias && 
                        alias.Dst.Storage is StackStorage stg &&
                        stg.StackOffset == offsetInterval.Start &&
                        stg.DataType.Size == offsetInterval.End - offsetInterval.Start)
                    {
                        return ssaIds[alias.Dst];
                    }
                }

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
                    var assSeq = new AliasAssignment(id, seq);
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

            private (SsaIdentifier, Expression, Interval<int>) SliceAndShift(KeyValuePair<Interval<int>, Alias> arg)
            {
                return (
                    arg.Value.SsaId,
                    arg.Value.SsaId.Identifier,
                    arg.Key.Intersect(this.offsetInterval));
            }

            public override Identifier WriteVariable(SsaBlockState bs, SsaIdentifier sid)
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
                bs.currentStackDef.Add(this.offsetInterval, new Alias { SsaId = sid });
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
                var sids = new SsaIdentifier[seq.Elements.Length];
                var offset = seq.BitSize;
                int i = 0;
                foreach (var e in seq.Elements)
                {
                    offset -= e.BitSize;
                    var ss = outer.factory.Create(outer.ssa.Procedure.Frame.EnsureIdentifier(e), stm);
                    ss.Offset = (int) offset;
                    var sid = ss.ReadVariable(bs);
                    sids[i++] = sid;
                }
                return Fuse(sids);
            }

            public override Identifier WriteVariable(SsaBlockState bs, SsaIdentifier sid)
            {
                var offset = seq.BitSize;
                foreach (var stg in seq.Elements)
                {
                    offset -= stg.BitSize;
                    var ss = outer.factory.Create(outer.ssa.Procedure.Frame.EnsureIdentifier(stg), stm);
                    ss.Offset = (int) offset;
                    ss.WriteVariable(bs, sid);
                }
                return sid.Identifier;
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
                    if (aassHead.Src is Slice eHead && aassTail.Src is Slice eTail)
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
                    // If x_2 = Slice(y_3); z_4 = (Slice) y_3 return y_3
                    if (assHead.Src is Slice slHead &&
                        assTail.Src is Slice caTail &&
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
        }

        public class SimpleTransformer : IdentifierTransformer
        {
            private Storage stg;

            public SimpleTransformer(Identifier id, Storage stg, Statement stm, SsaTransform outer) : base(id, stm, outer)
            {
                this.stg = stg;
            }

            public override SsaIdentifier ReadBlockLocalVariable(SsaBlockState bs)
            {
                bs.currentSimpleDef.TryGetValue(stg, out var sid);
                return sid;
            }

            public override Identifier WriteVariable(SsaBlockState bs, SsaIdentifier sid)
            {
                bs.currentSimpleDef[stg] = sid;
                return sid.Identifier;
            }
        }
    }
}
