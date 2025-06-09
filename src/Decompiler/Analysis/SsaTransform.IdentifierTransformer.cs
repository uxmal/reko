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
using Reko.Core.Collections;
using Reko.Core.Code;
using Reko.Core.Diagnostics;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Reko.Core.Analysis;
using Reko.Core.Operators;

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
            private Stack<Block> bloxx = new Stack<Block>();

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
                trace.Verbose("ReadVariable {0} in block {1}", this.id, bs.Block.DisplayName);
                if (bs.Terminates)
                {
                    // Reko has determined that this block diverges. We fall back to 
                    // getting a variable from the entry block, which is guaranteed to
                    // dominate every other block in the procedure.
                    bs = outer.blockstates[bs.Block.Procedure.EntryBlock];
                }
                var sid = ReadBlockLocalVariable(bs);
                if (sid is not null)
                    return sid;
                // Keep probin'.
                // The commented-out code below is for assistance when troubleshooting
                // stack overflows in the code. These are always caused by the
                // CFG of the Procedure being malformed due to errors in the Scanning
                // phase. Once #726 is addressed, there should be no need for this
                // code.
                bloxx.Push(bs.Block);
                if (this.bloxx.Count > 1000)
                {
                    var proc = bs.Block.Procedure;
                    proc.Dump(true);
                    Debug.Print("SSATransform: Blocks in the call stack:");
                    foreach (var block in bloxx)
                    {
                        Debug.Print("  {0}", block.DisplayName);
                    }
                    throw new StackOverflowException($"Boundless recursion in {proc.Name} while finding definitions of {this.id}.");
                }
                sid = ReadVariableRecursive(bs);
                bloxx.Pop();
                return sid!;
            }

            /// <summary>
            /// If the `id` has an SsaIdentifier available (not necessarily
            /// defined) in this block, return that SsaIdentifier.
            /// </summary>
            /// <param name="bs">SsaBlockState we look in.</param>
            /// <returns>An SsaIdentifier if it is available, 
            /// otherwise null.
            /// </returns>
            public abstract SsaIdentifier? ReadBlockLocalVariable(SsaBlockState bs);

            public SsaIdentifier? ReadVariableRecursive(SsaBlockState bs)
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
                if (val is not null)
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
                    stmNew = new Statement(stm.Address, ass, stm.Block);
                    stm.Block.Statements.Insert(i, stmNew);
                }
                else
                {
                    Address addr = block.Address;
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
                            addr = block.Statements[i].Address;
                        }
                    }
                    stmNew = block.Statements.Insert(i + 1, addr, ass);
                }

                var sidTo = ssaIds.Add(ass.Dst, stmNew, false);
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
                var stm = new Statement(b.Address, phiAss, b);
                b.Statements.Insert(0, stm);
                var sid = ssaIds.Add(phiAss.Dst, stm, false);
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
                trace.Verbose("  Checking {0} for triviality", phiFunc);
                if (phiFunc.Arguments.All(a => a.Value == phi.Identifier))
                {
                    trace.Verbose("  {0} is a def", phi.Identifier);
                    // Undef'ined or unreachable parameter; assume it's a def.
                    sid = NewDefInstruction(phi.OriginalIdentifier, phi.DefStatement.Block);
                }
                else
                {
                    sid = SamePhiArgument(phi)!;
                    if (sid is null)
                        return false;
                }

                // Remember all users except for phi
                var users = phi.Uses.Where(u => u != phi.DefStatement).ToList();

                // Reroute all uses of phi to use same. Remove phi.
                ReplaceBy(phi, sid);

                sid.Uses.RemoveAll(u => u == phi.DefStatement);

                // Remove all phi uses which may have become trivial now.
                trace.Verbose("Removing {0} and uses {1}", phi.Identifier.Name, string.Join(",", users));
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
                while (this.outer.sidsToRemove.Contains(sid!))
                {
                    sid = SamePhiArgument(sid)!;
                }
                return sid!;
            }

            /// <summary>
            /// If the arguments of phi function are equal, return it.
            /// </summary>
            /// <param name="phi">SSA identifier of phi function</param>
            /// <returns>
            /// If the arguments of phi function are equal, return it.
            /// Return null otherwise.
            /// </returns>
            private SsaIdentifier? SamePhiArgument(SsaIdentifier phi)
            {
                Identifier? same = null;
                var phiFunc = ((PhiAssignment) phi.DefStatement.Instruction).Src;
                foreach (var de in phiFunc.Arguments)
                {
                    var op = (Identifier) de.Value;
                    if (op == same || op == phi.Identifier)
                        continue;
                    if (same is not null)
                        return null;
                    same = op;
                }
                return ssaIds[same!];
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

            protected int MeasureBitSize(DataType dt)
            {
                return dt.MeasureBitSize(outer.arch.MemoryGranularity);
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
                if (param is not null)
                {
                    var copy = new Assignment(id, param);
                    var stmCopy = b.Statements.Add(b.Address, copy);
                    var sidCopy = ssaIds.Add(id, stmCopy, false);
                    copy.Dst = sidCopy.Identifier;

                    outer.ssa.AddUses(stmCopy);
                    return sidCopy;
                }
                return outer.ssa.EnsureDefInstruction(id, b);
            }

            private Expression? ReadParameter(Block b, FunctionType sig, Storage stg)
            {
                if (!sig.ParametersValid)
                    return null;
                var param = sig.Parameters!
                    .FirstOrDefault(p => p.Storage.OverlapsWith(stg));
                if (param is null)
                    return null;
                var sidParam = outer.SsaState.EnsureDefInstruction(param, b);
                var idParam = sidParam.Identifier;
                if (idParam.Storage.BitSize == stg.BitSize)
                    return idParam;
                var dt = PrimitiveType.CreateWord((int) stg.BitSize);
                if (param.Storage.Covers(stg))
                {
                    var arch = this.outer.arch;
                    var slice = arch.Endianness.MakeSlice(dt, idParam, idParam.Storage.OffsetOf(stg), arch.MemoryGranularity);
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
                            .ToDictionary(kv => kv.Key, kv => kv.Item2);
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
                           .ToDictionary(kv => kv.Key, kv => kv.Item2);
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
                if (sidOld.Identifier.Storage is not StackStorage stack)
                    return;
                var offsetInterval = CreateBitInterval(
                    stack.StackOffset,
                    sidOld.Identifier.DataType);
                var ints = bs.currentStackDef.GetIntervalsOverlappingWith(offsetInterval)
                    .ToArray();
                foreach (var de in ints)
                {
                    if (de.Value.SsaId == sidOld)
                    {
                        de.Value.SsaId = sidNew;
                    }
                }
            }

            protected Interval<int> CreateBitInterval(int unitStackOffset, DataType dt)
            {
                var bitsPerUnit = outer.arch.MemoryGranularity;
                var bitOffset = unitStackOffset * bitsPerUnit;
                return Interval.Create(
                    bitOffset,
                    bitOffset + MeasureBitSize(dt));
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
                trace.Verbose("  WriteBlockLocalVariable: ({0}, {1}, ({2})", bs.Block.DisplayName, id, this.liveBits);
                if (!bs.currentDef.TryGetValue(id.Storage.Domain, out var aliasState))
                {
                    aliasState = new AliasState();
                    bs.currentDef.Add(id.Storage.Domain, aliasState);
                }
                if (sid.DefStatement is not null && sid.DefStatement.Instruction is not AliasAssignment)
                {
                    // Only store a definition if it isn't an alias.
                    var stgDef = id.Storage;
                    var defRange = stgDef.GetBitRange();
                    for (int i = 0; i < aliasState.Definitions.Count; ++i)
                    {
                        var (sidPrev, prevRange, offset) = aliasState.Definitions[i];
                        var stgPrev = sidPrev.Identifier.Storage;
                        if (defRange.Covers(prevRange))
                        {
                            trace.Verbose("     overwriting: {0}", sidPrev.Identifier);
                            aliasState.Definitions.RemoveAt(i);
                            --i;
                        }
                    }
                    aliasState.Definitions.Add((sid, defRange, this.Offset));
                }
                trace.Verbose("     writing: {0}", sid.Identifier);

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

            public override SsaIdentifier? ReadBlockLocalVariable(SsaBlockState bs)
            {
                trace.Verbose("  ReadBlockLocalVariable: ({0}, {1}, ({2})", bs.Block.DisplayName, id, this.liveBits);
                if (!bs.currentDef.TryGetValue(id.Storage.Domain, out var alias))
                    return null;

                // Identifier id is available in this block.
                // Has an exact alias already been calculated?
                if (alias.ExactAliases.TryGetValue(id.Storage, out var sid))
                {
                    trace.Verbose("    found alias ({0}, {1})", bs.Block.DisplayName, sid.Identifier.Name);
                    return sid;
                }

                // At least some of the bits of 'id' are available locally in this 
                // block. Walk across the bits of 'id', collecting all parts
                // defined into a sequence.
                int offsetLo = this.liveBits.Lsb;
                int offsetHi = this.liveBits.Msb;
                var sids = new List<(SsaIdentifier, BitRange)>();
                while (offsetLo < offsetHi)
                {
                    var useRange = new BitRange(offsetLo, offsetHi);
                    var (sidElem, usedRange, defRange) = FindIntersectingRegister(alias.Definitions, useRange);
                    if (sidElem is null || offsetLo < usedRange.Lsb)
                    {
                        // Found a gap in the register that wasn't defined in
                        // this basic block. Seek backwards
                        var bitrangeR = sidElem is null
                            ? useRange
                            : new BitRange(offsetLo, usedRange.Lsb);
                        var idR = MakeTmpIdentifier(id.Storage, bitrangeR);
                        var rx = new RegisterTransformer(idR, stm, this.outer)
                        {
                            liveBits = bitrangeR
                        };
                        var sidR = rx.ReadVariableRecursive(bs);
                        sids.Add((sidR!, bitrangeR!));
                        offsetLo = bitrangeR.Msb;
                    }
                    if (sidElem is not null)
                    {
                        sids.Add((sidElem, defRange));
                        offsetLo = usedRange.Msb;
                    }
                }
                if (sids.Count == 1)
                {
                    var sidSlice = outer.MakeSlice(sids[0].Item1, sids[0].Item2, this.id);
                    alias.ExactAliases[this.id.Storage] = sidSlice;
                    return sidSlice;
                }
                else
                {
                    sids.Reverse(); // Order sids in big-endian order
                    var elems = new List<Expression>();
                    foreach (var (sidElem, bitrange) in sids)
                    {
                        var idSlice = MakeTmpIdentifier(sidElem.Identifier.Storage, bitrange);
                        var sidSlice = outer.MakeSlice(sidElem, bitrange, idSlice);
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
            public static (SsaIdentifier?, BitRange, BitRange) FindIntersectingRegister(
                List<(SsaIdentifier,BitRange,int)> definitions,
                BitRange useRange)
            {
                var result = ((SsaIdentifier?)null, useRange, default(BitRange));
                for (int i = definitions.Count - 1; i >= 0; --i)
                {
                    var (sid, defRange, offset) = definitions[i];
                    var intersection = defRange.Intersect(useRange);
                    if (!intersection.IsEmpty && (result.Item1 is null || result.useRange.Lsb > intersection.Lsb))
                    {
                        defRange = new BitRange(intersection.Lsb + offset, intersection.Msb + offset);
                        result = (sid, intersection, defRange);
                        useRange = new BitRange(useRange.Lsb, defRange.Lsb);
                    }
                }
                return result;
            }

            private Identifier MakeTmpIdentifier(Storage storage, BitRange bitrange)
            {
                RegisterStorage? reg;
                if (storage is SequenceStorage seq)
                {
                    (reg, bitrange) = FindRegisterInSequence(bitrange, seq);
                }
                else
                {
                    Debug.Assert(id.Storage is RegisterStorage);
                    reg = outer.ssa.Procedure.Architecture.GetRegister(id.Storage.Domain, bitrange);
                    Debug.Assert(reg is { });
                }
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

            private (RegisterStorage, BitRange) FindRegisterInSequence(BitRange bitrange, SequenceStorage seq)
            {
                int offset = 0;
                for (int i = seq.Elements.Length-1; i >= 0; --i)
                {
                    var stg = seq.Elements[i];
                    if (!stg.GetBitRange().Offset(offset).Intersect(bitrange).IsEmpty)
                    {
                        var subBitrange = bitrange.Offset(-offset);
                        var reg = (RegisterStorage) stg;
                        reg = outer.ssa.Procedure.Architecture.GetRegister(reg.Domain, subBitrange)!;
                        return (reg, subBitrange);
                    }
                    offset += (int)stg.BitSize;
                }
                throw new InvalidOperationException("Expected the bitrange to intersect with a subregister of the sequence.");
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
                trace.Verbose("  WriteBlockLocalVariable: ({0}, {1}, ({2:X8})", bs.Block.DisplayName, id, this.flagMask);
                if (!bs.currentFlagDef.TryGetValue(id.Storage.Domain, out var aliasState))
                {
                    aliasState = new FlagAliasState();
                    bs.currentFlagDef.Add(id.Storage.Domain, aliasState);
                }
                for (int i = 0; i < aliasState.Definitions.Count; ++i)
                {
                    if ((aliasState.Definitions[i].Item2 & ~flagMask) == 0)
                    {
                        trace.Verbose("     overwriting: {0} {1:X8}",
                            aliasState.Definitions[i].Item1.Identifier,
                            aliasState.Definitions[i].Item2);
                        aliasState.Definitions.RemoveAt(i);
                        --i;
                    }
                }
                aliasState.Definitions.Add((sid, flagMask));
                trace.Verbose("     writing: {0} {1:X8}", sid.Identifier, flagMask);

                var newDict = aliasState.ExactAliases
                    .Where(kv => !kv.Key.Storage.OverlapsWith(id.Storage))
                    .ToDictionary(kv => kv.Key, kv => kv.Value);
                newDict[id] = sid;
                aliasState.ExactAliases = newDict;
                return sid.Identifier;
            }

            public override SsaIdentifier? ReadBlockLocalVariable(SsaBlockState bs)
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
                        var flagGroup = outer.arch.GetFlagGroup(this.flagGroup.FlagRegister, maskElem & mask)!;
                        var idElem = outer.ssa.Procedure.Frame.EnsureFlagGroup(flagGroup);
                        sids.Add((alias, sidElem, maskElem & mask));
                        mask &= ~maskElem;
                    }
                }
                if (mask != 0)
                {
                    var fx = new FlagGroupTransformer(this.id, this.flagGroup, this.stm, this.outer, mask);
                    var sidR = fx.ReadVariableRecursive(bs);
                    sids.Add((alias, sidR!, mask));
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
                        (a, b) => outer.m.Bin(Operator.Or, flagGroup.FlagRegister.Width, a, b.Identifier));
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
                    return elem.sid;    // Don't need to slice if the bits match exactly.
                if (elem.alias.ExactAliases.TryGetValue(elem.sid.Identifier, out var sid))
                    return sid;
                var e = outer.m.And(elem.sid.Identifier, elem.mask);
                this.flagGroup = outer.arch.GetFlagGroup(grfFrom.FlagRegister, elem.mask)!;
                var idSlice = outer.ssa.Procedure.Frame.EnsureFlagGroup(this.flagGroup);
                var ass = new AliasAssignment(idSlice, e);
                var sidSlice = outer.ssa.InsertAfterDefinition(elem.sid.DefStatement, ass);
                elem.sid.Uses.Add(sidSlice.DefStatement);
                return sidSlice;
            }

            /// <summary>
            /// If the defining statement doesn't exactly match the bits of
            /// the using statements, we have to generate an alias assignment
            /// after the defining statement.
            /// </summary>
            /// <returns></returns>
            protected static SsaIdentifier MaybeGenerateAliasStatement(
                SsaIdentifier sidFrom,
                FlagGroupStorage stgUse)
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
            private Interval<int> bitOffsetInterval;
            private int stackOffset;            // measured in units (not bits or bytes)

            public StackTransformer(
                Identifier id,
                int stackOffset,
                Statement stm,
                SsaTransform outer)
                : base(id, stm, outer)
            {
                this.stackOffset = stackOffset;
                this.bitOffsetInterval = CreateBitInterval(stackOffset, id.DataType);
            }

            public override SsaIdentifier? ReadBlockLocalVariable(SsaBlockState bs)
            {
                var ints = bs.currentStackDef.GetIntervalsOverlappingWith(bitOffsetInterval)
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
                        stg.StackOffset == stackOffset &&
                        MeasureBitSize(stg.DataType) == bitOffsetInterval.End - bitOffsetInterval.Start)
                    {
                        return ssaIds[alias.Dst];
                    }
                }

                // Part of 'id' is defined locally in this block. We now 
                // walk across the bits of 'id'.
                int offsetLo = this.bitOffsetInterval.Start;
                int offsetHi = this.bitOffsetInterval.End;
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
                    var sidTo = MakeSequenceElement(sequence[0]);
                    return sidTo;
                }
                else
                {
                     var seq = outer.arch.Endianness.MakeSequence(
                        PrimitiveType.CreateWord(MeasureBitSize(this.id.DataType)), 
                        sequence.Select(e => (Expression) MakeSequenceElement(e).Identifier)
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
                var curInterval = this.bitOffsetInterval;
                var curOffset = this.stackOffset;
                var curId = this.id;
                var bitsPerUnit = outer.arch.MemoryGranularity;
                var bitSize = (intv.End - intv.Start);
                this.stackOffset = intv.Start / bitsPerUnit;
                this.id = outer.ssa.Procedure.Frame.EnsureStackVariable(
                    stackOffset,
                    PrimitiveType.CreateWord(bitSize));
                this.bitOffsetInterval = intv;
                var sid = ReadVariableRecursive(bs);
                this.id = curId;
                this.stackOffset = curOffset;
                this.bitOffsetInterval = curInterval;
                return sid!;
            }

            private SsaIdentifier MakeSequenceElement((SsaIdentifier sid, Interval<int> interval) elem)
            {
                var stg = (StackStorage) elem.sid.Identifier.Storage;
                var bitsPerUnit = outer.arch.MemoryGranularity;
                var bitStart = stg.StackOffset * bitsPerUnit;
                var end = bitStart + MeasureBitSize(stg.DataType);
                if (bitStart == elem.interval.Start &&
                    end == elem.interval.End)
                {
                    // Exact match
                    return elem.sid;
                }
                var sidSlice = EnsureSliceStatement(elem.sid, elem.interval, elem.interval.Start - bitStart);
                return sidSlice;
            }

            /// <summary>
            /// Create a slice statement to extract a subinterval from, unless one already exists, in
            /// which case we reuse it.
            /// <paramref name="sidFrom"/>.
            /// </summary>
            /// <remarks>
            /// The source, or defined identifier <paramRef name=sidFrom" /> is "wider"
            /// than the destination or used storage. We must provide a slice of the
            /// defined identifier.
            /// </remarks>
            private SsaIdentifier EnsureSliceStatement(SsaIdentifier sidFrom, Interval<int> intv, int bitOffset)
            {
                var arch = this.outer.arch;
                var bitSize = intv.End - intv.Start;
                var idSlice = outer.ssa.Procedure.Frame.EnsureStackVariable(intv.Start / outer.arch.MemoryGranularity, PrimitiveType.CreateWord(bitSize));
                var e = arch.Endianness.MakeSlice(idSlice.DataType, sidFrom.Identifier, bitOffset, arch.MemoryGranularity);

                // Find existing alias of that size.
                foreach (var use in sidFrom.Uses)
                {
                    if (use.Instruction is AliasAssignment alias && 
                        alias.Dst.Storage is StackStorage &&
                        alias.Src is Slice slice && 
                        slice.Offset == e.Offset && 
                        slice.DataType.BitSize == e.DataType.BitSize)
                    {
                        return ssaIds[alias.Dst];
                    }
                }
                var sidUse = sidFrom;

                var ass = new AliasAssignment(idSlice, e);
                var sidAlias = outer.ssa.InsertAfterDefinition(sidFrom.DefStatement, ass);
                sidUse.Uses.Add(sidAlias.DefStatement);
                return sidAlias;
            }

            private (SsaIdentifier, Expression, Interval<int>) SliceAndShift(KeyValuePair<Interval<int>, Alias> arg)
            {
                return (
                    arg.Value.SsaId,
                    arg.Value.SsaId.Identifier,
                    arg.Key.Intersect(this.bitOffsetInterval));
            }

            public override Identifier WriteVariable(SsaBlockState bs, SsaIdentifier sid)
            {
                var ints = bs.currentStackDef
                    .GetIntervalsOverlappingWith(bitOffsetInterval)
                    .ToArray();
                foreach (var i in ints)
                {
                    bs.currentStackDef.Delete(i.Key);
                    if (!this.bitOffsetInterval.Covers(i.Key))
                    {
                        // Some of the bits of interval `i` will shine through
                        if (i.Key.End > bitOffsetInterval.Start && i.Key.Start < bitOffsetInterval.Start)
                        {
                            var newInt = Interval.Create(i.Key.Start, bitOffsetInterval.Start);
                            bs.currentStackDef.Add(newInt, i.Value);
                        }
                        if (i.Key.Start < bitOffsetInterval.End && bitOffsetInterval.End < i.Key.End)
                        {
                            var newInt = Interval.Create(bitOffsetInterval.End, i.Key.End);
                            bs.currentStackDef.Add(newInt, i.Value);
                        }
                    }
                }
                bs.currentStackDef.Add(this.bitOffsetInterval, new Alias(sid));
                return sid.Identifier;
            }
        }

        public class SequenceTransformer : IdentifierTransformer
        {
            private readonly SequenceStorage seq;

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
                    ss.Offset = (int) (offset - e.BitAddress);
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
                    if (aassHead.Src is Slice eHead && aassTail.Src is Slice eTail &&
                        eHead.Expression == eTail.Expression)
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
                        sids[0].DefStatement.Address,
                        ass);
                    var sidTo = ssaIds.Add(ass.Dst, stm, false);
                    ass.Dst = sidTo.Identifier;
                    foreach (var sid in sids)
                    {
                        sid.Uses.Add(stm);
                    }
                    return sidTo;
                }

                if (sids.Length == 2 && 
                    sids[0]!.DefStatement.Instruction is Assignment assHead &&
                    sids[1]!.DefStatement.Instruction is Assignment assTail)
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
                var stm = this.stm.Block.Statements.Insert(iStm, this.stm.Address, ass);
                var sidTo = ssaIds.Add(ass.Dst, stm, false);
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
            private readonly Storage stg;

            public SimpleTransformer(Identifier id, Storage stg, Statement stm, SsaTransform outer) : base(id, stm, outer)
            {
                this.stg = stg;
            }

            public override SsaIdentifier? ReadBlockLocalVariable(SsaBlockState bs)
            {
                bs.currentSimpleDef.TryGetValue(stg, out SsaIdentifier? sid);
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
