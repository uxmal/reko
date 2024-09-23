#region License
/* 
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>.
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
using Reko.Core.Services;
using System;
using System.Collections.Generic;

namespace Reko.ImageLoaders.Nro
{
    /// <summary>
    /// Nintendo Switch Executable Loader
    /// </summary>
    /// <remarks>
    /// Reference: https://switchbrew.org/wiki/NRO
    /// </remarks>
    public class NroLoader : ProgramImageLoader
    {
        public const UInt32 MAGIC = 0x304f524e; //NRO0 in Little Endian

        private IEventListener eventListener;
        private LeImageReader rdr;

        public NroLoader(IServiceProvider services, ImageLocation imageLocation, byte[] imgRaw)
            : base(services, imageLocation, imgRaw)
        {
            eventListener = services.RequireService<IEventListener>();
            rdr = new LeImageReader(RawImage, 0);
        }

        public override Address PreferredBaseAddress
        {
            get
            {
                return new Address64(0x80000000);
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        private readonly List<ImageSegment> segments = new List<ImageSegment>();

        private string GetNroSegmentName(NroSegmentType type)
        {
            switch (type)
            {
            case NroSegmentType.ApiInfo: return ".apiinfo";
            case NroSegmentType.Data:    return ".data";
            case NroSegmentType.DynStr:  return ".dynstr";
            case NroSegmentType.DynSym:  return ".dynsym";
            case NroSegmentType.Ro:      return ".ro";
            case NroSegmentType.Text:    return ".text";
            default:
                throw new ArgumentException("Invalid NRO Segment Type");
            }
        }

        public AccessMode GetNroSegmentAccess(NroSegmentType type)
        {
            switch (type)
            {
            case NroSegmentType.ApiInfo: return AccessMode.Read;
            case NroSegmentType.Data:    return AccessMode.ReadWrite;
            case NroSegmentType.DynStr:  return AccessMode.Read;
            case NroSegmentType.DynSym:  return AccessMode.Read;
            case NroSegmentType.Ro:      return AccessMode.Read;
            case NroSegmentType.Text:    return AccessMode.ReadExecute;
            default:
                throw new ArgumentException("Invalid NRO Segment Type");
            }
        }

        private void HandleSegment(NroSegmentType type, NroSegmentHeader nroSeg)
        {
            ByteMemoryArea bmem = new ByteMemoryArea(
                PreferredBaseAddress + nroSeg.file_offset,
                rdr.ReadAt<byte[]>(nroSeg.file_offset, rdr => rdr.ReadBytes(nroSeg.size))
            );

            ImageSegment seg = new ImageSegment(
                GetNroSegmentName(type),
                bmem,
                GetNroSegmentAccess(type)
            );

            segments.Add(seg);
        }

        public override Program LoadProgram(Address? addrLoad)
        {
            var cfgSvc = Services.RequireService<IConfigurationService>();
            var arch = cfgSvc.GetArchitecture("arm-64")!;
            var platform = cfgSvc.GetEnvironment("switch").Load(Services, arch);

            NroStart start = rdr.ReadStruct<NroStart>();
            NroHeader header = rdr.ReadStruct<NroHeader>();
            if(header.magic != MAGIC)
            {
                throw new BadImageFormatException("Invalid NRO Magic.");
            }

            HandleSegment(NroSegmentType.Text, header.segments0[0]);
            HandleSegment(NroSegmentType.Ro, header.segments0[1]);
            HandleSegment(NroSegmentType.Data, header.segments0[2]);

            HandleSegment(NroSegmentType.ApiInfo, header.segments1[0]);
            HandleSegment(NroSegmentType.DynStr, header.segments1[1]);
            HandleSegment(NroSegmentType.DynSym, header.segments1[2]);

            SegmentMap segMap = new SegmentMap(PreferredBaseAddress, segments.ToArray());
            Program program = new Program(new ByteProgramMemory(segMap), arch, platform);
            return program;
        }
    }
}
