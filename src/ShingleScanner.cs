using Reko.Core;
using Reko.Core.Lib;
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return ExecuteInParallel(chunks);
        }

        public List<ChunkWorker> MakeScanChunks()
        {
            var sortedBlocks = new BTreeDictionary<Address, Block>();
            foreach (var block in cfg.Blocks.Values)
            {
                sortedBlocks.Add(block.Address, block);
            }
            return program.SegmentMap.Segments.Values
                .Where(s => s.IsExecutable)
                .SelectMany(s => PartitionSegment(s, sortedBlocks))
                .ToList();
        }

        private IEnumerable<ChunkWorker> PartitionSegment(
            ImageSegment segment, 
            BTreeDictionary<Address, Block> sortedBlocks)
        {
            long iGapOffset = 0;
            long length;
            ChunkWorker? chunk;
            int unitAlignment = program.Architecture.InstructionBitSize / program.Architecture.MemoryGranularity;
            while (iGapOffset < segment.Size)
            {
                var addrGapStart = segment.Address + iGapOffset;
                if (!sortedBlocks.TryGetUpperBoundIndex(addrGapStart, out int iBlock))
                    break;
                var nextBlock = sortedBlocks.Values[iBlock];
                var gapSize = nextBlock.Address - addrGapStart;
                chunk = MakeChunkWorker(segment, addrGapStart, gapSize);
                if (chunk is not null)
                {
                    yield return chunk;
                }
                iGapOffset += gapSize + nextBlock.Length;
                iGapOffset = unitAlignment * ((iGapOffset + (unitAlignment - 1)) / unitAlignment);
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
                   (int)length);
            }
        }

        private Cfg ExecuteInParallel(List<ChunkWorker> chunks)
        {
            //$TODO: this should be parallelizable, but we
            // do it first in series to make it correct.
            foreach (var chunk in chunks)
            {
                chunk.Run();
            }
            return cfg;
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
