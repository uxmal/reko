using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core;
using Reko.Core.Configuration;

namespace Reko.ImageLoaders.MachO.Arch
{
    class PowerPCSpecific  : ArchSpecific
    {
        public PowerPCSpecific(IProcessorArchitecture arch) : base(arch)
        {
        }

        public override Address ReadStub(Address addrStub, MemoryArea mem)
        {
            throw new NotImplementedException();
        }

    }
}
