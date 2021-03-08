using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core;
using Reko.Core.Types;

namespace Reko.Environments.Windows
{
	public class M68kCallingConvention : CallingConvention
	{
		private IProcessorArchitecture arch;

		public M68kCallingConvention(IProcessorArchitecture arch)
		{
			this.arch = arch;
		}

		public void Generate(ICallingConventionEmitter ccr, DataType? dtRet, DataType? dtThis, List<DataType> dtParams)
		{
		}

        public bool IsArgument(Storage stg)
        {
            return stg is StackStorage;
        }

        public bool IsOutArgument(Storage stg)
        {
            if (stg is RegisterStorage reg)
            {
                return reg.Number == 0;
            }
            return false;
        }
    }
}
