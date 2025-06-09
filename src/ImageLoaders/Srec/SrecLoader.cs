#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Loading;
using Reko.Core.Memory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Reko.ImageLoaders.Srec
{
    public class SrecLoader : ProgramImageLoader
    {
        public SrecLoader(IServiceProvider services, ImageLocation imageLocation, byte[] rawBytes) 
            : base(services, imageLocation, rawBytes)
        {
        }

        public override Address PreferredBaseAddress
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public override Program LoadProgram(Address? addrLoad) => throw new NotSupportedException();

        public override Program LoadProgram(
            Address addrLoad, 
            IProcessorArchitecture arch, 
            IPlatform platform,
            List<UserSegment> userSegments)
        {
            var srecParser = new SrecParser(RawImage);
            var fragments = new Dictionary<Address, byte[]>();
            ImageSymbol? start = null;
            for (; ;)
            {
                var record = srecParser.ParseRecord();
                if (record is null)
                    break;
                switch (record.Type)
                {
                case SrecType.Header:
                    break;  // Ignore 
                case SrecType.Data16:
                case SrecType.Data24:
                case SrecType.Data32:
                    fragments[record.Address] = record.Data!;
                    break;
                case SrecType.StartAddress:
                    start = ImageSymbol.Procedure(arch, record.Address!);
                    break;
                }
            }
            var segments = CollectFragments(fragments);
            var program = new Program(new ByteProgramMemory(segments), arch, platform);
            if (start is not null)
            {
                program.EntryPoints.Add(start.Address!, start);
            }
            return program;
        }

        private SegmentMap CollectFragments(Dictionary<Address, byte[]> fragments)
        {
            MemoryStream? dataCur = null;
            Address? addrSegmentCur = null;
            List<ImageSegment> segments = new List<ImageSegment>();
            foreach (var de in fragments.OrderBy(de => de.Key))
            {
                if (addrSegmentCur is null)
                {
                    addrSegmentCur = de.Key;
                    dataCur = new MemoryStream();
                    dataCur.Write(de.Value, 0, de.Value.Length);
                }
                else
                {
                    var data = dataCur!;
                    if (IsFragmentAdjacent(addrSegmentCur.Value, data, de.Key))
                    {
                        data.Write(de.Value, 0, de.Value.Length);
                    }
                    else
                    {
                        var segment = new ImageSegment(
                            $"seg{addrSegmentCur}",
                            new ByteMemoryArea(addrSegmentCur.Value, dataCur!.ToArray()),
                            AccessMode.ReadWriteExecute);
                        segments.Add(segment);

                        addrSegmentCur = de.Key;
                        dataCur = new MemoryStream();
                        dataCur.Write(de.Value, 0, de.Value.Length);
                    }
                }
            }
            if (addrSegmentCur is null)
                throw new BadImageFormatException("There was no actual data in file.");

            var lastSegment = new ImageSegment(
                $"seg{addrSegmentCur}",
                new ByteMemoryArea(addrSegmentCur.Value, dataCur!.ToArray()),
                AccessMode.ReadWriteExecute);
            segments.Add(lastSegment);
            var map = new SegmentMap(
                segments.ToArray());
            return map;
        }

        private bool IsFragmentAdjacent(Address addrSegmentCur, MemoryStream data, Address addrNext)
        {
            return (data.Position == addrNext - addrSegmentCur);
        }
    }
}
