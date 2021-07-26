#region License
/* 
 * Copyright (C) 2018-2021 Stefano Moioli <smxdev4@gmail.com>.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion

using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Loading;
using Reko.Core.Memory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.Pef
{
    /// <summary>
    /// reference: https://developer.apple.com/library/archive/documentation/mac/pdf/MacOS_RT_Architectures.pdf
    /// </summary>
    public class PefLoader : ImageLoader
    {
        private PefContainer? container;

        public PefLoader(IServiceProvider services, string filename, byte[]rawImage) :
            base(services, filename, rawImage)
        {
        }

        public override Address PreferredBaseAddress
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


        private PefContainer LoadContainer(EndianByteImageReader rdr)
        {
            return PefContainer.Load(rdr);
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
            if (container == null) throw new InvalidOperationException();

            var cfgSvc = Services.RequireService<IConfigurationService>();
            var arch = GetArchitecture(cfgSvc);
            var platform = GetPlatform(cfgSvc, arch);

            var pefSegments = container.GetImageSegments(rdr, PreferredBaseAddress!).ToArray();

            var segments = pefSegments.Select(s => s.Segment);
            var addrBase = pefSegments.Min(s => s.Segment.Address);
            var segMap = new SegmentMap(addrBase, segments.ToArray());

            var pefFile = PefFile.Load(container, pefSegments);
            var entryPoints = pefFile.GetEntryPoints(arch).ToSortedList(s => s.Address);
            var symbols = pefFile.GetSymbols(arch).ToArray();

            var program = new Program(segMap, arch, platform)
            {
                EntryPoints = entryPoints
            };
            foreach(var sym in symbols)
            {
                program.ImageSymbols.Add(sym.Address, sym);
            }
            return program;
        }

        private IProcessorArchitecture GetArchitecture(IConfigurationService cfgSvc)
        {
            if (container == null) throw new InvalidOperationException();

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
            if (container == null) throw new InvalidOperationException();

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
