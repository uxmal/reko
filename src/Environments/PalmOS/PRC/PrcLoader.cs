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
using Reko.Core.Configuration;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.Libraries.MacsBug;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Environments.PalmOS.PRC
{
    /// <summary>
    /// Loads PalmOS PRC files.
    /// </summary>

    public class PrcLoader : ProgramImageLoader
    {
        // https://web.mit.edu/tytso/www/pilot/prc-format.html

        public PrcLoader(IServiceProvider services, ImageLocation imageLocation, byte[] imgRaw)
            : base(services, imageLocation, imgRaw)
        {
        }

        public override Address PreferredBaseAddress
        {
            get { return Address.Ptr32(0x00100000); }
            set { throw new NotImplementedException(); }
        }


        public override Program LoadProgram(Address? loadAddress, string? sPlatformOverride)
        {
            var addrLoad = loadAddress ?? PreferredBaseAddress;
            var rdr = new BeImageReader(base.RawImage);
            var cfgSvc = Services.RequireService<IConfigurationService>();
            var arch = cfgSvc.GetArchitecture("m68k")!;

            var header = LoadHeader(rdr);
            if (header is null)
                throw new BadImageFormatException();
            var rsrcHeaders = LoadResourceHeaders(rdr, header.num_records);
            if (rsrcHeaders is null)
                throw new BadImageFormatException();
            var (segments, ep, symbols) = LoadResources(arch, rdr, rsrcHeaders, addrLoad);
            if (segments is null)
                throw new BadImageFormatException();

            var platform = Platform.Load(Services, "palmOS", sPlatformOverride, arch);
            var program = new Program(new ByteProgramMemory(segments), arch, platform);
            if (ep is not null)
            {
                program.EntryPoints.Add(ep.Value, ImageSymbol.Procedure(arch, ep.Value));
            }
            foreach (var sym in symbols)
            {
                program.ImageSymbols[sym.Address] = sym;
            }
            return program;
        }

        private PrcHeader? LoadHeader(BeImageReader rdr)
        {
            var header = new PrcHeader();
            var abName = rdr.ReadBytes(0x20);
            if (abName.Length != 0x20)
                return null;
            int i = Array.IndexOf(abName, 0);
            if (i < 0)
                header.name = Encoding.ASCII.GetString(abName);
            else
                header.name = Encoding.ASCII.GetString(abName, 0, i);

            if (!rdr.TryReadBeUInt16(out header.flags))
                return null;
            if (!rdr.TryReadBeUInt16(out header.version))
                return null;
            if (!rdr.TryReadBeUInt32(out header.create_time))
                return null;
            if (!rdr.TryReadBeUInt32(out header.mod_time))
                return null;
            if (!rdr.TryReadBeUInt32(out header.backup_time))
                return null;

            if (!rdr.TryReadBeUInt32(out header.mod_num))
                return null;
            if (!rdr.TryReadBeUInt32(out header.app_info))
                return null;
            if (!rdr.TryReadBeUInt32(out header.sort_info))
                return null;
            if (!rdr.TryReadBeUInt32(out header.type))
                return null;

            if (!rdr.TryReadBeUInt32(out header.id))
                return null;
            if (!rdr.TryReadBeUInt32(out header.unique_id_seed))
                return null;
            if (!rdr.TryReadBeUInt32(out header.next_record_list))
                return null;
            if (!rdr.TryReadBeUInt16(out header.num_records))
                return null;

            return header;
        }

        private List<ResourceHeader>? LoadResourceHeaders(BeImageReader rdr, uint cRecords)
        {
            var headers = new List<ResourceHeader>();
            for (uint i = 0; i < cRecords; ++i)
            {
                var rhdr = new ResourceHeader();
                var abName = rdr.ReadBytes(4);
                if (abName.Length != 4)
                    return null;
                rhdr.name = Encoding.ASCII.GetString(abName);
                if (!rdr.TryReadBeUInt16(out rhdr.id))
                    return null;
                if (!rdr.TryReadBeUInt32(out rhdr.offset))
                    return null;
                headers.Add(rhdr);
            }
            return headers;
        }

        private (SegmentMap, Address?, List<ImageSymbol>) LoadResources(
            IProcessorArchitecture arch,
            BeImageReader rdr, 
            List<ResourceHeader> rsrcHeaders, 
            Address addrBase)
        {
            var segments = new List<ImageSegment>();
            var symbols = new List<ImageSymbol>();
            var addr = addrBase;
            Address? addrEntrypoint = null;
            for (int i = 0; i < rsrcHeaders.Count - 1;  ++i)
            {
                var hdr = rsrcHeaders[i];
                var length = rsrcHeaders[i+1].offset - hdr.offset;
                rdr.Offset = hdr.offset;
                var bytes = rdr.ReadBytes(length);
                var mem = new ByteMemoryArea(addr, bytes);
                string? name = hdr.name;
                if (name is null)
                    name = $"#{hdr.id}";
                else
                    name = $"{name}#{hdr.id}";
                var accessMode = AccessMode.Read;
                if (hdr.name == "code")
                {
                    if (hdr.id == 0)
                    {
                        LoadCode0Resource();
                    }
                    else
                    {
                        accessMode = AccessMode.ReadExecute;
                        if (hdr.id == 1)
                        {
                            addrEntrypoint = addr;
                        }
                        var symScanner = new SymbolScanner(arch, mem);
                        symbols.AddRange(symScanner.ScanForSymbols());
                    }
                }
                var seg = new ImageSegment(name, mem, accessMode);
                segments.Add(seg);
                addr += length; //$TODO: align to even paragraph boundary?
            }
            var map = new SegmentMap(segments.ToArray());
            return (map, addrEntrypoint, symbols);
        }

        private void LoadCode0Resource()
        {
            //$TODO: according to the doc page, the code0 resource sometimes
            // is patterend after MacOS, with a jump table above the "a5 line",
            // and sometimes follows a different convention. Needs more investigation,
            // and access to different binaries build by different toolchains.
        }
    }
}
