using Reko.Core;
using Reko.Core.Lib;
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using RtlBlock = Reko.Scanning.RtlBlock;

namespace Reko.ScannerV2
{
    public class ShingleScanner : AbstractScanner
	{
        public ShingleScanner(Core.Program program, Cfg cfg, DecompilerEventListener listener)
            : base(program, cfg, listener)
        {
        }

        public Cfg ScanProgram()
        {
            var chunks = MakeScanChunks();
            var cfg = ExecuteChunks(chunks);
            cfg = EnsureBlocks(cfg);
            return cfg;
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
        /// aren't present in the <see cref="Cfg"/>.
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

        public override bool TryRegisterBlockEnd(Address addrStart, Address addrLast)
        {
            //$TODO: when two ShingleScanners are leapfrogging.
            return true;
        }

        private Cfg ExecuteChunks(List<ChunkWorker> chunks)
        {
            //$TODO: this should be parallelizable, but we
            // do it first in series to make it correct.
            foreach (var chunk in chunks)
            {
                chunk.Run();
            }
            return cfg;
        }

        /// <summary>
        /// Ensure there are <see cref="Block"/>s at 
        /// </summary>
        private Cfg EnsureBlocks(Cfg cfg)
        {
            var blocks = cfg.Blocks.Values.ToSortedList(b => b.Address);
            var edges = cfg.Successors.Values
                .SelectMany(e => e)
                .OrderBy(e => e.To)
                .ToList();
            foreach (var e in edges)
            {
                if (!blocks.TryGetLowerBound(e.To, out var block))
                {
                    Debug.Fail("Edge going to hyperspace");
                    continue; 
                }
                if (block.Address != e.To)
                {
                    SplitBlockAt(block, e.To);
                }
            }
            return cfg;
        }

        private RtlBlock SplitBlockAt(RtlBlock block, Address to)
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
                    block.Instructions.RemoveRange(i, c - i);
                    var newBlock = RegisterBlock(
                        block.Architecture,
                        to,
                        (int)(addrInstr - block.Address),
                        addrInstr,
                        newInstrs);
                    RegisterEdge(new Edge(block.Address, newBlock.Address, EdgeType.Fallthrough));
                    return newBlock;
                }
            } 
            throw new NotImplementedException("Couldn't find split.");
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
