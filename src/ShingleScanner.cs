using Reko.Core;
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

        private List<ChunkWorker> MakeScanChunks()
        {
            return program.SegmentMap.Segments.Values
                .Where(s => s.IsExecutable)
                .Select(s => new ChunkWorker(
                    this,
                    program.Architecture,
                    s.MemoryArea,
                    s.Address,
                    (int)s.Size))
                .ToList();
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
