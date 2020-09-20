using Reko.Core;
using Reko.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.Pef
{
    /// <summary>
    /// reference: https://developer.apple.com/library/archive/documentation/mac/pdf/MacOS_RT_Architectures.pdf
    /// </summary>
    public class PefLoader : ImageLoader
    {
        private ImageReader rdr;
        private PEFContainer container;

        public PefLoader(IServiceProvider services, string filename, byte[]rawImage) : base(services, filename, rawImage)
        {
        }

        public override Address PreferredBaseAddress
        {
            get
            {
                return null;
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        private void LoadSections()
        {
            //this.header.sectionCount
        }

        private void LoadHeader()
        {
            this.container = new PEFContainer(rdr);
        }

        public override Program Load(Address? addrLoad)
        {
            var cfgSvc = Services.RequireService<IConfigurationService>();
            var arch = cfgSvc.GetArchitecture("ppc-be-32");
            var platform = cfgSvc.GetEnvironment("macOs").Load(Services, arch);

            rdr = new BeImageReader(RawImage, 0);
            this.LoadHeader();
            this.LoadSections();

            Address32 addrBase = new Address32(0xdeadbeef);


            var program = new Program(new SegmentMap(addrBase, container.GetImageSegments().ToArray()), arch, platform);
            return program;
        }

        public override RelocationResults Relocate(Program program, Address addrLoad)
        {
            //$TODO
            return new RelocationResults(new List<ImageSymbol>(), new SortedList<Address, ImageSymbol>());
        }
    }
}
