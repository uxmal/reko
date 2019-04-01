using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.Unix
{
    public class UnixLoader : ImageLoader
    {

        UNIXLibraryHeader header;
             
        public UnixLoader(IServiceProvider services, string filename, byte[] rawBytes)
           : base(services, filename, rawBytes)
        {
            header = new UNIXLibraryHeader();

        }


        public override Address PreferredBaseAddress { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override Program Load(Address addrLoad)
        {
            throw new NotImplementedException();
        }

        public override RelocationResults Relocate(Program program, Address addrLoad)
        {
            throw new NotImplementedException();
        }
    }
}
