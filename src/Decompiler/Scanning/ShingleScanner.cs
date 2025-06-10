#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
 .
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
using Reko.Core.Diagnostics;
using Reko.Core.Types;
using Reko.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Reko.Scanning
{
    /// <summary>
    /// This scanner disassembles all possible instruction locations of 
    /// an image, and discards instructions that (transitively) result 
    /// in conflicts.
    /// </summary>
    /// <remarks>
    /// Inspired by the paper "Shingled Graph Disassembly:
    /// Finding the Undecidable Path" by Richard Wartell, Yan Zhou, 
    /// Kevin W.Hamlen, and Murat Kantarcioglu.
    /// </remarks>
    public class ShingleScanner : AbstractScanner
	{
        public ShingleScanner(
            Program program,
            ScanResultsV2 sr,
            IDynamicLinker dynamicLinker,
            IDecompilerEventListener listener,
            IServiceProvider services)
            : base(program, sr, ProvenanceType.Heuristic, dynamicLinker, listener, services)
        {
        }

        /// <summary>
        /// During shingle scanning we could potentially be disassembling the 
        /// middles of instructions, so we are at risk of generating invalid addresses.
        /// </summary>
        public override bool AllowSpeculativeTransfers => false;

        public ScanResultsV2 ScanProgram()
        {
            var gaps = FindUnscannedExecutableGaps();
            return ScanProgram(gaps);
        }

        public ScanResultsV2 ScanProgram(IEnumerable<Chunk> gaps)
        {
            var chunks = MakeScanChunks();
            var sr = ProcessChunks(chunks);
            sr = RegisterPredecessors();
            sr = RemoveInvalidBlocks(sr);
            var (sr3, blocksSplit) = EnsureBlocks(sr);
            return sr3;
        }

        public List<ChunkWorker> MakeScanChunks()
        {
            var gaps = FindUnscannedExecutableGaps();
            return gaps.Select(g =>
            {
                Debug.Assert(g.Architecture is not null);
                return this.MakeChunkWorker(g.Architecture, g.Address, g.Length);
            }).ToList();
        }

        public List<Chunk> FindUnscannedExecutableGaps()
        {
            var sortedBlocks = sr.Blocks.Values.OrderBy(b => b.Address);
            var chunkEnumerator = new ChunkEnumerator();
            var execSegments = program.SegmentMap.Segments.Values
                .Where(seg => seg.IsExecutable);
            var dataBlocks = program.ImageMap?.Items.Values
                .Where(i => i.DataType is not CodeType) ?? [];
            var fragments = chunkEnumerator.EnumerateFragments(
                execSegments,
                sortedBlocks,
                dataBlocks);
            return MakeTriples(fragments).Select(CreateUnscannedArea)
                .Where(f => f is not null)
                .Select(f => f!.Value)
                .ToList();
        }

        /// <summary>
        /// From an <see cref="IEnumerable{T}"/> of items, generate an <see cref="IEnumerable{T}"/> of 
        /// triples, where the middle item of each triple corresponds to the items from <paramref name="items"/>.
        /// </summary>
        /// <remarks>
        /// Consider the sequence A B C D..X Y Z. The output of this method is:
        /// (_ A B)
        /// (A B C)
        /// (B C D)
        /// ...
        /// (X Y Z)
        /// (Y Z _)
        /// (where '_' is the default element.
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns>A sequence of triples</returns>
        private static IEnumerable<(T, T, T)> MakeTriples<T>(IEnumerable<T> items)
        {
            T prev = default(T)!;
            var e = items.GetEnumerator();
            if (!e.MoveNext())
                yield break;
            T item = e.Current;
            while (e.MoveNext())
            {
                var next = e.Current;
                yield return (prev!, item, next);
                prev = item;
                item = next;
            }
            yield return (prev!, item, default(T)!);
        }

        /// <summary>
        /// Given a triple of blocks, decides whether the middle block is an unscanned area
        /// that could be fed to the shingle scanner.
        /// </summary>
        /// <param name="triple"></param>
        /// <returns></returns>
        private Chunk? CreateUnscannedArea((Chunk, Chunk, Chunk) triple)
        {
            var (prev, item, next) = triple;
            if (item.Architecture is not null)
            {
                return default;
            }

            // Determine an architecture for the item.
            var prevArch = prev.Architecture;
            var nextArch = next.Architecture;
            IProcessorArchitecture arch;
            if (prevArch is null)
            {
                arch = nextArch ?? program.Architecture;
            }
            else if (nextArch is null)
            {
                arch = prevArch ?? program.Architecture;
            }
            else
            {
                // Both prev and next have an architecture.
                if (prevArch == nextArch)
                {
                    arch = prevArch;
                }
                else
                {
                    // Different architectures on both sides. 
                    // Arbitrarily pick the architecture of the largest 
                    // adjacent block. If they're the same size, default 
                    // to the predecessor.
                    arch = (prev.Length < next.Length)
                        ? nextArch
                        : prevArch;
                }
            }

            return new Chunk(
                arch,
                null!,
                item.Address,
                item.Length);
        }


        public ChunkWorker MakeChunkWorker(IProcessorArchitecture arch, Address addr, long length)
        {
            return new ChunkWorker(
                this,
                arch,
                addr,
                (int) length,
                base.rejectMask,
                listener);
        }

        private ScanResultsV2 ProcessChunks(IEnumerable<ChunkWorker> chunks)
        {
            //$TODO: this should be parallelizable, but we
            // do it first in series to make it correct.
            foreach (var chunk in chunks)
            {
                chunk.Run();
            }
            return sr;
        }

        /// <summary>
        /// Ensure there are <see cref="Block"/>s at all successor
        /// target addresses.
        /// </summary>
        private (ScanResultsV2, int) EnsureBlocks(ScanResultsV2 cfg)
        {
            var blocks = new BTreeDictionary<Address, RtlBlock>(cfg.Blocks);

            // These are the blocks that we need.
            var succs = cfg.Successors.Values
                .SelectMany(e => e)
                .Concat(cfg.SpeculativeProcedures.Keys)
                .Distinct()
                .OrderBy(a => a)
                .ToList();
            int blocksSplit = 0;
            foreach (var s in succs)
            {
                var (block, isSplit) = EnsureBlock(s, blocks);
                if (block is not null && isSplit)
                {
                    blocks.TryAdd(block.Address, block);
                    ++blocksSplit;
                }
            }
            return (cfg, blocksSplit);
        }

        private (RtlBlock?, bool) EnsureBlock(
            Address addrBlock, 
            BTreeDictionary<Address, RtlBlock> blocks)
        {
            if (!blocks.TryGetLowerBoundIndex(addrBlock, out var iBlock))
            {
                Debug.Fail("Edge going to hyperspace");
                return (null, false);
            }

            for (; iBlock >= 0; --iBlock)
            {
                var block = blocks.Values[iBlock];
                long offset = addrBlock - block.Address;
                if (0 < offset && offset < block.Length)
                {
                    var newBlock = SplitBlockAt(block, addrBlock);
                    if (newBlock is not null)
                    {
                        return (newBlock, true);
                    }
                }
            }
            return (null, false);
        }

        /// <summary>
        /// From the candidate set of <paramref name="blocks"/>, remove blocks that 
        /// are invalid.
        /// </summary>
        /// <returns>A (hopefully smaller) set of blocks.</returns>
        public static ScanResultsV2 RemoveInvalidBlocks(ScanResultsV2 sr)
        {
            // Find transitive closure of bad blocks 
            sr.Successors.TryGetValue(Address.Ptr32(0xA578), out var succes);
            _ = sr; //$DEBUG
            sr.Blocks.TryGetValue(Address.Ptr32(0xA578), out var xxx);    //$DEBUG
            var bad_blocks = new HashSet<Address>(
                (from b in sr.Blocks.Values
                 where !b.IsValid || b.Instructions[^1].Class == InstrClass.Invalid
                 select b.Address).Concat(
                    sr.InvalidBlocks.Keys));
            var new_bad = bad_blocks;
            //Debug.Print("Bad {0}",

            //    string.Join(
            //        "\r\n      ",
            //        bad_blocks
            //            .OrderBy(x => x)
            //            .Select(x => string.Format("{0:X8}", x))));
            for (; ; )
            {
                // Find all blocks that are reachable from blocks
                // that already are known to be "bad", but that don't
                // end in a call.
                //$TODO: delay slots. @#$#@
                new_bad = new HashSet<Address>(new_bad
                    .SelectMany(bad => sr.Predecessors.TryGetValue(bad, out var badPreds)
                        ? badPreds
                        : (IEnumerable<Address>) Array.Empty<Address>())
                    .Where(l =>
                        !bad_blocks.Contains(l)
                        &&
                        !BlockEndsWithCall(sr.Blocks[l])));

                if (new_bad.Count == 0)
                    break;

                //Debug.Print("new {0}",
                //    string.Join(
                //        "\r\n      ",
                //        bad_blocks
                //            .OrderBy(x => x)
                //            .Select(x => string.Format("{0:X8}", x))));

                bad_blocks.UnionWith(new_bad);
            }
            trace.Inform("Bad blocks: {0} of {1}", bad_blocks.Count, sr.Blocks.Count);
            //DumpBadBlocks(sr, blocks, sr.FlatEdges, bad_blocks);

            // Remove edges to bad blocks and bad blocks.
            foreach (var bad in bad_blocks)
            {
                if (bad.Offset == 0xA578)
                    _ = bad; //$DEBUG

                if (sr.Predecessors.TryGetValue(bad, out var preds))
                {
                    foreach (var pred in preds)
                    {
                        if (bad.Offset == 0xA578)
                            _ = bad; //$DEBUG
                        if (sr.Successors.TryGetValue(pred, out var pss))
                        {
                            pss.Remove(bad);
                        }
                    }
                }
                sr.Successors.TryRemove(bad, out _);
                sr.Blocks.TryRemove(bad, out _);
            }
            return sr;
        }

        private static bool BlockEndsWithCall(RtlBlock block)
        {
            int len = block.Instructions.Count;
            if (len < 1)
                return false;
            if (block.Instructions[len - 1].Class == (InstrClass.Call | InstrClass.Transfer))
                return true;
            return false;
        }

        /// <summary>
        /// Splits the given <see cref="RtlBlock" /> <paramref name="block"/>
        /// at the address <paramref name="addr"/> if the block contains
        /// an instruction exactly at that address.
        /// </summary>
        /// <param name="block">Block to split.</param>
        /// <param name="addr">Address at which to split the given <paramref name="block"/>.</param>
        /// <returns>If an instruction exactly at <paramref name="addr"/> 
        /// was found, returns a newly created <see cref="Block"/>, starting
        /// at that address. If no such instruction was found, 
        /// returns null.</returns>
        public RtlBlock? SplitBlockAt(RtlBlock block, Address addr)
        {
            int c = block.Instructions.Count;
            for (int i = 0; i < c; ++i)
            {
                //$PERF Binary search only makes sense if blocks are 
                // really large. Needs measurement
                var addrInstr = block.Instructions[i].Address;
                if (addrInstr == addr)
                {
                    var newInstrs = block.Instructions.GetRange(i, c - i);
                    block.Instructions.RemoveRange(i, c - i);
                    var offset = (int)(addr - block.Address);
                    var newBlock = RegisterBlock(
                        block.Architecture,
                        addr,
                        block.Length - offset,
                        block.FallThrough,
                        newInstrs);
                    StealEdges(block.Address, newBlock.Address);
                    block.FallThrough = newBlock.Address;
                    block.Length = offset;
                    RegisterEdge(new Edge(block.Address, newBlock.Address, EdgeType.Fallthrough));
                    return newBlock;
                }
            }
            return null;
        }
    }
}
