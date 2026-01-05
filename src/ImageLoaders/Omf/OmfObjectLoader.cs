#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using Reko.Core.Collections;
using Reko.Core.Configuration;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.Omf
{
    /// <summary>
    /// Loads an Intel OMF object file.
    /// </summary>
    public class OmfObjectLoader : ProgramImageLoader
    {
        public OmfObjectLoader(IServiceProvider services, ImageLocation imageLocation, byte[] imgRaw) : base(services, imageLocation, imgRaw)
        {
        }

        public override Address PreferredBaseAddress
        {
            get => Address.SegPtr(0x800, 0);
            set => throw new NotSupportedException();
        }

        public override Program LoadProgram(Address? address, string? sPlatformOverride)
        {
            var configSvc = Services.RequireService<IConfigurationService>();
            var arch = configSvc.GetArchitecture("x86-real-16");
            if (arch is null)
                throw new InvalidOperationException("Expected X86 real mode architecture to be available.");
            var platform = Platform.Load(Services, "msdos", sPlatformOverride, arch);
            return LoadProgram(
                GetRecordEnumerator(RawImage),
                address ?? PreferredBaseAddress,
                arch,
                platform);
        }

        private IEnumerable<OmfRecord> GetRecordEnumerator(byte[] rawImage)
        {
            var parser = new OmfParser(new LeImageReader(RawImage));
            while (parser.TryReadRecord(out var record))
                yield return record;
        }

        public Program LoadProgram(
            IEnumerable<OmfRecord> omfRecords, 
            Address address,
            IProcessorArchitecture arch,
            IPlatform platform)
        {
            var e = omfRecords.GetEnumerator();
            var theader = Expect<TheaderRecord>(e);
            //$TODO: name the program after contents in theader?

            // Lnames are 1-indexed, so put a dummy in position 0
            var labelNames = new List<string> { "" };
            // Segments are also 1-indexed, so put a dummy in position 0
            var segments = new List<ImageSegment> { null! };
            var segmems = new List<byte[]> { null! };
            var symbols = new List<ImageSymbol>();
            bool modendSeen = false;
            while (e.MoveNext())
            {
                if (modendSeen)
                    throw new BadImageFormatException("Unexpected data after MODEND OMF record.");
                switch (e.Current)
                {
                case UnknownRecord _:
                    break;
                case LnamesRecord lnames:
                    labelNames.AddRange(lnames.Names);
                    break;
                case SegdefRecord segdef:
                    if (segdef.NameIndex == 0 || segdef.NameIndex >= labelNames.Count)
                        throw new BadImageFormatException("Invalid segment name index.");
                    var bytes = new byte[segdef.Length];
                    segmems.Add(bytes);
                    var memarea = new ByteMemoryArea(address, bytes);
                    var segment = new ImageSegment(labelNames[segdef.NameIndex], memarea, AccessMode.ReadWriteExecute);
                    segments.Add(segment);
                    break;
                case PubdefRecord pubdef:
                    if (pubdef.SegmentIndex == 0 || pubdef.SegmentIndex >= segments.Count)
                        throw new BadImageFormatException("Invalid segment index.");
                    segment = segments[pubdef.SegmentIndex]!;
                    foreach (var name in pubdef.Names)
                    {
                        var addrSym = segment.Address + name.Offset;
                        var symbol = ImageSymbol.Create(SymbolType.Unknown, arch, addrSym, name.Name);
                        symbols.Add(symbol);
                    }
                    break;
                case DataRecord data:
                    if (data.SegmentIndex == 0 || data.SegmentIndex >= segments.Count)
                        throw new BadImageFormatException("Invalid segment index.");
                    segment = segments[data.SegmentIndex];
                    Array.Copy(data.Data, 0, segmems[data.SegmentIndex]!, data.Offset, data.Data.Length);
                    break;
                case ModendRecord:
                    modendSeen = true;
                    break;
                }
            }
            var segmentMap = new SegmentMap(segments.Skip(1).ToArray()!);
            var program = new Program(
                new ByteProgramMemory(segmentMap),
                arch,
                platform,
                symbols.ToSortedList(s => s.Address),
                new());
            return program;
        }

        private T Expect<T>(IEnumerator<OmfRecord> records)
            where T : OmfRecord
        {
            if (!records.MoveNext())
                throw new BadImageFormatException($"Expected an OMF record of type {typeof(T).Name}");
            if (records.Current is not T result)
                throw new BadImageFormatException($"Expected an OMF record of type {typeof(T).Name}," +
                    $"but read a record of type {records.Current.GetType().Name}.");
            return result;
        }
    }
}
