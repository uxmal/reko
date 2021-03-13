using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Memory;
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
        private PEFContainer container;

        public PefLoader(IServiceProvider services, string filename, byte[]rawImage) :
            base(services, filename, rawImage)
        {
        }

        public override Address? PreferredBaseAddress
        {
            get
            {
                return Address.Ptr32(0x0010_0000);
            }
            set
            {
                throw new NotSupportedException();
            }
        }


        private PEFContainer LoadContainer(EndianByteImageReader rdr)
        {
            return PEFContainer.Load(rdr);
        }

        public override Program Load(Address? addrLoad)
        {
            var rdr = new BeImageReader(RawImage, 0);
            this.container = this.LoadContainer(rdr);
            Program program = MakeProgram(rdr);
            return program;
        }

        private Program MakeProgram(EndianByteImageReader rdr)
        {
            var cfgSvc = Services.RequireService<IConfigurationService>();
            var arch = GetArchitecture(cfgSvc);
            var platform = GetPlatform(cfgSvc, arch);

            var segments = container.GetImageSegments(rdr, PreferredBaseAddress!).ToArray();
            var addrBase = segments.Min(s => s.Address);
            var program = new Program(new SegmentMap(addrBase, segments), arch, platform);
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
