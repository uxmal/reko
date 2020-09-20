using Reko.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.ImageLoaders.Pef
{
    public class PefLoader : ImageLoader
    {
        public PefLoader(IServiceProvider services, string filename, byte[]rawImage) : base(services, filename, rawImage)
        {
        }

        public override Address PreferredBaseAddress
        {
            get
            {
                return Address.Ptr32(0x0010_1000);
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public override Program Load(Address? addrLoad)
        {
            throw new NotImplementedException();
        }

        public override RelocationResults Relocate(Program program, Address addrLoad)
        {
            throw new NotImplementedException();
        }
    }
}
