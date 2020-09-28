#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Environments.AtariTOS
{
    /// <summary>
    /// Loads Atari TOS PRG files.
    /// </summary>
    /// // https://github.com/yegord/snowman/issues/131
    public class PrgLoader : ImageLoader
    {
        public PrgLoader(IServiceProvider services, string filename, byte[] imgRaw) : base(services, filename, imgRaw)
        {
        }

        public override Address PreferredBaseAddress
        {
            get { return Address.Ptr32(0x00100000); }
            set { throw new NotImplementedException(); }
        }

        public override Program Load(Address addrLoad)
        {
            var rdr = new BeImageReader(RawImage);
            if (!TryLoadHeader(rdr, out var hdr))
                throw new BadImageFormatException();

            var mem = new MemoryArea(addrLoad, new byte[ hdr.TextSize + hdr.DataSize + hdr.BssSize ]);
            int cRead = rdr.ReadBytes(mem.Bytes, 0, hdr.TextSize + hdr.DataSize);
            if (cRead != hdr.TextSize + hdr.DataSize)
                throw new BadImageFormatException();

            var text = new ImageSegment(".text", addrLoad, mem, AccessMode.ReadExecute) { Size = hdr.TextSize };
            var data = new ImageSegment(".data", addrLoad + hdr.TextSize, mem, AccessMode.ReadWrite) { Size = hdr.DataSize };
            var bss = new ImageSegment(".bss", addrLoad + hdr.TextSize + hdr.DataSize, mem, AccessMode.ReadWrite) { Size = hdr.BssSize };
            //$TODO: Implement symbols. For now just skip over them.
            rdr.Offset += hdr.SymbolsSize;

            PerformRelocations(mem, rdr);


            var cfgSvc = Services.RequireService<IConfigurationService>();
            var arch = cfgSvc.GetArchitecture("m68k");
            var env = cfgSvc.GetEnvironment("atariTOS");
            var platform = env.Load(Services, arch);
            var map = new SegmentMap(
                addrLoad,
                text, data, bss);
            return new Program(map, arch, platform);
        }

        private bool TryLoadHeader(BeImageReader rdr, out PrgHeader hdr)
        {
            var h = rdr.ReadStruct<PrgHeader>();
            if (h.Magic != 0x601A)
            {
                hdr = default(PrgHeader);
                return false;
            }
            hdr = h;
            return true;
        }

        public override RelocationResults Relocate(Program program, Address addrLoad)
        {
            return new RelocationResults(
                new List<ImageSymbol> { ImageSymbol.Location(program.Architecture, addrLoad) },
                new SortedList<Address, ImageSymbol>());
        }

        bool PerformRelocations(MemoryArea mem, ImageReader rdr)
        {
            if (!rdr.TryReadBeUInt32(out uint fixup))
                return false;
            if (fixup == 0)
                return true;    // no relocations to do.
            uint offset = fixup;
            for (;;)
            {
                var dst = mem.BaseAddress + offset;
                uint l = mem.ReadBeUInt32(offset);
                l += mem.BaseAddress.ToUInt32();
                mem.WriteBeUInt32(offset, l);
                mem.Relocations.AddPointerReference(mem.BaseAddress.ToLinear() + offset, l);

                for(;;)
                {
                    if (!rdr.TryReadByte(out byte b))
                        return false;
                    if (b == 0)
                        return true;
                    else if (b == 1)
                    {
                        offset += 254;
                    }
                    else
                    {
                        offset += b;
                        break;
                    }
                }
            }
        }

        [Endian(Endianness.BigEndian)]
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PrgHeader
        {
            public ushort Magic;
            public uint TextSize;
            public uint DataSize;
            public uint BssSize;
            public uint SymbolsSize;
            public uint Reserved1;
            public uint ProgramFlags;
            public ushort IsAbsolute;
        }
    }
}
