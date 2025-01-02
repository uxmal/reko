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
using Reko.Core.IO;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Services;
using System;
using System.Runtime.InteropServices;

namespace Reko.ImageLoaders.AOut
{
    // System calls: https://github.com/ryanwoodsmall/oldsysv/blob/master/sysvr2-vax/include/sys.s
    public class AOutLoader : ProgramImageLoader
    {
        public AOutLoader(
            IServiceProvider services,
            ImageLocation imageLocation,
            byte[] bytes) 
            : base(services, imageLocation, bytes)
        {
            this.PreferredBaseAddress = Address.Ptr32(0x00000000);
        }

        public override Address PreferredBaseAddress { get; set; }

        public override Program LoadProgram(Address? address)
        {
            ushort magic = ByteMemoryArea.ReadLeUInt16(base.RawImage, 0);
            var cfgSvc = Services.RequireService<IConfigurationService>();
            if (magic == 0x010B)
            {
                var rdr = new ByteImageReader(RawImage);
                var vaxHeader = rdr.ReadStruct<vax_header>();
                rdr.Offset = 0x400;
                var textBytes = rdr.ReadBytes(vaxHeader.a_text);
                var uAddrData = (uint) rdr.Offset;
                var dataBytes = rdr.ReadBytes(vaxHeader.a_data);
                var uAddrBss = (uint) rdr.Offset;
                var bssBytes = new byte[vaxHeader.a_bss];

                var segmentMap = new SegmentMap(
                    Seg(".text", 0x400, textBytes, AccessMode.ReadExecute),
                    Seg(".data", uAddrData, dataBytes, AccessMode.ReadWrite),
                    Seg(".bss", uAddrBss, bssBytes, AccessMode.ReadWrite));

                var arch = cfgSvc.GetArchitecture("vax")!;

                var uAddrEntry = vaxHeader.a_entry != 0
                    ? vaxHeader.a_entry
                    : 0x404u;
                var entry = ImageSymbol.Location(arch, Address.Ptr32(uAddrEntry));   // Entry point

                var program = new Program(
                    new ByteProgramMemory(segmentMap),
                    arch,
                    new VaxUnix(Services, arch));
                program.EntryPoints.Add(entry.Address, entry);

                return program;
            }

            throw new NotSupportedException("This a.out variant is not supported yet.");
        }

        private static ImageSegment Seg(string segmentName, uint uAddr, byte[] bytes, AccessMode access)
        {
            var addr = Address.Ptr32(uAddr);
            var mem = new ByteMemoryArea(addr, bytes);
            var segment = new ImageSegment(segmentName, mem, access);
            return segment;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Endian(Endianness.LittleEndian)]

    public struct vax_header
    {
        public uint a_magic;   // Magic number
        public uint a_text;    // Size of text segment
        public uint a_data;    // Size of data segment
        public uint a_bss;     // Size of bss segment
        public uint a_syms;    // Size of symbol table
        public uint a_entry;   // Entry point
        public uint a_trsize;  // Size of text relocation
        public uint a_drsize;  // Size of data relocation
    };
}
