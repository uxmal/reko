using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ScannerV2
{
    internal class ProcedureDetector
    {
		public ProcedureDetector(Cfg cfg)
        {

        }
	}

  //  	- score conflicts:
		//called_from(callees_traced).Count
		//called_from(unsafe_callees).Count* 0.5
		//has_start_pattern.Length* 0.5
		//proc starts by pattern
		//preceded by zero block
		//preceded by padding block


}
