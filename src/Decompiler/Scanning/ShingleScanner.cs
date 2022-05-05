#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.Core.Types;
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
        public ShingleScanner(Program program, ScanResultsV2 sr, DecompilerEventListener listener)
            : base(program, sr, listener)
        {
        }

        public ScanResultsV2 ScanProgram()
        {
            var chunks = MakeScanChunks();
            var sr = ExecuteChunks(chunks);
            sr = RegisterPredecessors();
            sr = RemoveInvalidBlocks(sr);
            var (sr3, blocksSplit) = EnsureBlocks(sr);
            return sr3;
        }

        public List<ChunkWorker> MakeScanChunks()
        {
            var sortedBlocks = new BTreeDictionary<Address, RtlBlock>();
            foreach (var block in sr.Blocks.Values)
            {
                sortedBlocks.Add(block.Address, block);
            }
            return program.SegmentMap.Segments.Values
                .Where(s => s.IsExecutable)
                .SelectMany(s => PartitionSegment(s, sortedBlocks))
                .ToList();
        }

        private record MemoryGap(Address Address, long Length, IProcessorArchitecture? Architecture);

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
        private (IProcessorArchitecture, MemoryArea, Address, long)? CreateUnscannedArea((MemoryGap, MemoryGap, MemoryGap) triple)
        {
            var (prev, item, next) = triple;
            if (!this.program.SegmentMap.TryFindSegment(item.Address, out ImageSegment? seg))
                return null;
            if (!seg.IsExecutable)
                return null;

            // Determine an architecture for the item
            var prevArch = prev?.Architecture;
            var nextArch = next?.Architecture;
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
                    arch = (prev!.Length < next!.Length)
                        ? nextArch
                        : prevArch;
                }
            }

            return (
                arch,
                seg.MemoryArea,
                item.Address,
                item.Length);
        }

        /// <summary>
        /// Break up an <see cref="ImageSegment"/> into blocks that
        /// aren't present in the <see cref="ScanResultsV2"/>.
        /// </summary>
        /// <param name="segment"></param>
        /// <param name="sortedBlocks"></param>
        /// <returns></returns>
        private IEnumerable<ChunkWorker> PartitionSegment(
            ImageSegment segment, 
            BTreeDictionary<Address, RtlBlock> sortedBlocks)
        {
            long Align(long value, int alignment)
            {
                return alignment * ((value + (alignment - 1)) / alignment);
            }

            long iGapOffset = 0;
            long length;
            ChunkWorker? chunk;
            int unitAlignment = program.Architecture.InstructionBitSize / program.Architecture.MemoryGranularity;
            while (iGapOffset < segment.Size)
            {
                var addrGapStart = segment.Address + iGapOffset;
                if (!sortedBlocks.TryGetUpperBound(addrGapStart, out var nextBlock))
                    break;
                var gapSize = nextBlock.Address - addrGapStart;
                chunk = MakeChunkWorker(addrGapStart, gapSize);
                if (chunk is not null)
                {
                    yield return chunk;
                }
                iGapOffset = Align(iGapOffset + gapSize + nextBlock.Length, unitAlignment);
            }

            // Consume the remainder of the segment.
            length = segment.Size - iGapOffset;
            chunk = MakeChunkWorker(segment.Address + iGapOffset, length);
            if (chunk is not null)
            {
                yield return chunk;
            }
        }

        private ChunkWorker? MakeChunkWorker(Address addr, long length)
        {
            if (length <= 0)
                return null;
            return new ChunkWorker(
                this,
                program.Architecture,
                addr,
                (int) length,
                listener);
        }

        private ScanResultsV2 ExecuteChunks(List<ChunkWorker> chunks)
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

            var succs = cfg.Successors.Values
                .SelectMany(e => e)
                .Concat(cfg.SpeculativeProcedures.Keys)
                .Distinct()
                .OrderBy(a => a)
                .ToList();
            int blocksSplit = 0;
            foreach (var s in succs)
            {
                if (!blocks.TryGetLowerBound(s, out var block))
                {
                    Debug.Fail("Edge going to hyperspace");
                    continue; 
                }

                long offset = s - block.Address;
                if (0 < offset && offset < block.Length)
                {
                    var newBlock = SplitBlockAt(block, s);
                    if (newBlock is not null)
                    {
                        blocks.Add(newBlock.Address, newBlock);
                        ++blocksSplit;
                    }
                }
            }
            return (cfg, blocksSplit);
        }

        /// <summary>
        /// From the candidate set of <paramref name="blocks"/>, remove blocks that 
        /// are invalid.
        /// </summary>
        /// <returns>A (hopefully smaller) set of blocks.</returns>
        public static ScanResultsV2 RemoveInvalidBlocks(ScanResultsV2 sr)
        {
            // Find transitive closure of bad blocks 

            var bad_blocks = new HashSet<Address>(
                (from b in sr.Blocks.Values
                 where !b.IsValid || b.Instructions[^1].Class == InstrClass.Invalid
                 select b.Address));
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
                if (sr.Predecessors.TryGetValue(bad, out var preds))
                {
                    foreach (var pred in preds)
                    {
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


        private RtlBlock? SplitBlockAt(RtlBlock block, Address to)
        {
            int c = block.Instructions.Count;
            for (int i = 0; i < c; ++i)
            {
                //$PERF Binary search only makes sense if blocks are 
                // really large. Needs measurement
                var addrInstr = block.Instructions[i].Address;
                if (addrInstr == to)
                {
                    var newInstrs = block.Instructions.Skip(i).ToList();
                    var offset = (int)(to - block.Address);
                    block.Instructions.RemoveRange(i, c - i);
                    var newBlock = RegisterBlock(
                        block.Architecture,
                        to,
                        block.Length - offset,
                        block.FallThrough,
                        newInstrs);
                    block.FallThrough = newBlock.Address;
                    block.Length = offset;
                    RegisterEdge(new Edge(block.Address, newBlock.Address, EdgeType.Fallthrough));
                    return newBlock;
                }
            }
            //$TODO this shouldn't happen, since shingle scanning
            // scans every possible byte.
            return null;
        }
    }
}
