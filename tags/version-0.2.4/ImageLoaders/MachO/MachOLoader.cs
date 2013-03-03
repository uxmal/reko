using Decompiler.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.ImageLoaders.MachO
{
    public class MachOLoader : ImageLoader
    {
        public MachOLoader(IServiceProvider services, byte[] rawBytes) : base(services, rawBytes) { }

        public override IProcessorArchitecture Architecture
        {
            get { throw new NotImplementedException(); }
        }

        public override Platform Platform
        {
            get { throw new NotImplementedException(); }
        }

        public override Address PreferredBaseAddress
        {
            get { throw new NotImplementedException(); }
        }

        public override ProgramImage Load(Address addrLoad)
        {
            throw new NotImplementedException();
        }

        public override void Relocate(Address addrLoad, List<EntryPoint> entryPoints, RelocationDictionary relocations)
        {
            throw new NotImplementedException();
        }
    }
}
