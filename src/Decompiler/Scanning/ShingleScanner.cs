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
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Reko.Scanning
{
    public class ShingleScanner : AbstractScanner
	{
        public ShingleScanner(Core.Program program, ScanResultsV2 cfg, DecompilerEventListener listener)
            : base(program, cfg, listener)
        {
        }

        public ScanResultsV2 ScanProgram()
        {
            var chunks = MakeScanChunks();
            var cfg = ExecuteChunks(chunks);
            var (cfg2, blocksSplit) = EnsureBlocks(cfg);
            return cfg2;
        }

        public List<ChunkWorker> MakeScanChunks()
        {
            var sortedBlocks = new BTreeDictionary<Address, RtlBlock>();
            foreach (var block in cfg.Blocks.Values)
            {
                sortedBlocks.Add(block.Address, block);
            }
            return program.SegmentMap.Segments.Values
                .Where(s => s.IsExecutable)
                .SelectMany(s => PartitionSegment(s, sortedBlocks))
                .ToList();
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
                chunk = MakeChunkWorker(segment, addrGapStart, gapSize);
                if (chunk is not null)
                {
                    yield return chunk;
                }
                iGapOffset = Align(iGapOffset + gapSize + nextBlock.Length, unitAlignment);
            }

            // Consume the remainder of the segment.
            length = segment.Size - iGapOffset;
            chunk = MakeChunkWorker(segment, segment.Address + iGapOffset, length);
            if (chunk is not null)
            {
                yield return chunk;
            }
        }

        private ChunkWorker? MakeChunkWorker(ImageSegment segment, Address addr, long length)
        {
            if (length <= 0)
            {
                return null;
            }
            else
            {
                return new ChunkWorker(
                   this,
                   program.Architecture,
                   segment.MemoryArea,
                   addr,
                   (int)length,
                   listener);
            }
        }

        public override void SplitBlockEndingAt(RtlBlock block, Address lastAddr)
        {
            throw new NotImplementedException();
        }

        private ScanResultsV2 ExecuteChunks(List<ChunkWorker> chunks)
        {
            //$TODO: this should be parallelizable, but we
            // do it first in series to make it correct.
            foreach (var chunk in chunks)
            {
                chunk.Run();
            }
            return cfg;
        }

        private T Time<T>(string caption, Func<T> fn)
        {
            var sw = new Stopwatch();
            sw.Start();
            var result = fn();
            sw.Stop();
            Console.WriteLine("{0}: {1} msec", caption, sw.ElapsedMilliseconds);
            return result; 
        }

        /// <summary>
        /// Ensure there are <see cref="Block"/>s at 
        /// </summary>
        private (ScanResultsV2, int) EnsureBlocks(ScanResultsV2 cfg)
        {
            var blocks = Time("EnsureBlocks (BTree)", () => 
                new BTreeDictionary<Address, RtlBlock>(cfg.Blocks));

            var edges = cfg.Successors.Values
                .SelectMany(e => e)
                .OrderBy(e => e.To)
                .ToList();
            int blocksSplit = 0;
            foreach (var e in edges)
            {
                if (!blocks.TryGetLowerBound(e.To, out var block))
                {
                    Debug.Fail("Edge going to hyperspace");
                    continue; 
                }

                long offset = e.To - block.Address;
                if (0 < offset && offset < block.Length)
                {
                    var newBlock = SplitBlockAt(block, e.To);
                    if (newBlock is not null)
                    {
                        blocks.Add(newBlock.Address, newBlock);
                        ++blocksSplit;
                    }
                }
            }
            return (cfg, blocksSplit);
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

        //if (at zero)
        //	eatzeros until done
        //if (at unc jump or call)
        //	if (next instr is zeros)
        //		add_zeros_to_block();
        //	if (next inst is pad)
        //		add_padding_to_block(Core.Program program)
        //		block_type(padded)


        //- ShingleScan
        //	- Break image into[block..blockend)
        //	-InParallel(
        //		BlocksInParallel,
        //		AsciiStrings / TextStrings)
        //        all speculative
        //	- Gather "soup" of all possible blocks and edges
        //		remember indirects

        //	- Find protoprocedures(ICFG)
        //		- use recursive algorithm first on blocks. (see above)
        //			yields: callees_traced
        //		- for remaining blocks, use Nucleus algorithm
        //			- pick start blocks by private object gaps;
    }
}
