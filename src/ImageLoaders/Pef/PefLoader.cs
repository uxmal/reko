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

            rdr = new BeImageReader(RawImage, 0);
            this.LoadHeader();
            this.LoadSections();

            var arch = GetArchitecture(cfgSvc);
            var platform = GetPlatform(cfgSvc, arch);

            var segments = container.GetImageSegments().ToArray();
            var addrBase = segments.Min(s => s.Address);
            var program = new Program(new SegmentMap(addrBase, container.GetImageSegments().ToArray()), arch, platform);
            return program;
        }

        private IProcessorArchitecture GetArchitecture(IConfigurationService cfgSvc)
        {
            string sArch;
            switch (this.container.ContainerHeader.architecture)
            {
            case OSType.kPowerPCCFragArch: sArch = "ppc-be-32"; break;
            case OSType.kMotorola68KCFragArch: sArch = "m68k"; break;
            default: throw new NotSupportedException($"Architecture type {container.ContainerHeader.architecture:X8} is not supported.");
            }
            var arch = cfgSvc.GetArchitecture(sArch);
            if (arch is null)
                throw new InvalidOperationException($"Unable to load {sArch} architecture.");
            return arch;
        }

        private IPlatform GetPlatform(IConfigurationService cfgSvc, IProcessorArchitecture arch)
        {
            string sPlatform;
            switch (this.container.ContainerHeader.architecture)
            {
            case OSType.kPowerPCCFragArch: sPlatform = "macOsPpc"; break;
            case OSType.kMotorola68KCFragArch: sPlatform = "macOs"; break;
            default: throw new NotSupportedException($"Environment type {container.ContainerHeader.architecture:X8} is not supported.");
            }
            var platform = cfgSvc.GetEnvironment(sPlatform).Load(Services, arch);
            return platform;

        }

        public override RelocationResults Relocate(Program program, Address addrLoad)
        {
            //$TODO
            return new RelocationResults(new List<ImageSymbol>(), new SortedList<Address, ImageSymbol>());
        }
    }
}
