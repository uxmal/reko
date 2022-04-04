using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ScannerV2
{
    public class ShingleScanner
	{
        private Reko.Core.Program program;

        public ShingleScanner(Core.Program program)
		{
			this.program = program;
		}

		public Cfg Scan(object gaps)
        {
			var sections = MakeScanSections(gaps);
			return ExecuteInParallel(sections!);
        }

        private object? MakeScanSections(object gaps)
        {
			return null;
        }

        private Cfg ExecuteInParallel(object o)
        {
			throw new NotImplementedException();
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
