using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core;
using Reko.Core.Machine;
using Reko.Core.Types;

namespace Reko.Environments.Windows
{
    public class M68kCallingConvention : AbstractCallingConvention
	{
		private IProcessorArchitecture arch;

		public M68kCallingConvention(IProcessorArchitecture arch) : base("")
        {
			this.arch = arch;
		}

        public override void Generate(
            ICallingConventionEmitter ccr,
            int retAddressOnStack,
            DataType? dtRet,
            DataType? dtThis,
            List<DataType> dtParams)
        {
        }

        public override bool IsArgument(Storage stg)
        {
            return stg is StackStorage;
        }

        public override bool IsOutArgument(Storage stg)
        {
            if (stg is RegisterStorage reg)
            {
                return reg.Number == 0;
            }
            return false;
        }
    }
}
