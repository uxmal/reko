#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.MzExe
{
    public class NeImageLoader : ImageLoader
    {
        private ushort cbFileAlignmentShift;
        private ushort cSeg;
        private IDiagnosticsService diags;
        private uint lfaNew;
        private LoadedImage image;
        private ImageMap imageMap;

        public NeImageLoader(IServiceProvider services, string filename, byte[] rawBytes, uint e_lfanew)
            : base(services, filename, rawBytes)
        {
            ImageReader rdr = new LeImageReader(RawImage, e_lfanew);
            diags = Services.RequireService<IDiagnosticsService>();
            if (!LoadNeHeader(rdr))
                throw new BadImageFormatException("Unable to read NE header.");
            this.lfaNew = e_lfanew;
        }

        public override Address PreferredBaseAddress
        {
            get { return Address.ProtectedSegPtr(0xF, 0); }
            set
            {
                throw new NotImplementedException();
            }
        }

        private bool LoadNeHeader(ImageReader rdr)
        {
            ushort magic;
            if (!rdr.TryReadLeUInt16(out magic) || magic != 0x454E)
                throw new BadImageFormatException("Not a valid NE header.");
            ushort linker;
            if (!rdr.TryReadLeUInt16(out linker))
                return false;
            ushort offEntryTable;
            if (!rdr.TryReadLeUInt16(out offEntryTable))
                return false;
            ushort cbEntryTable;
            if (!rdr.TryReadLeUInt16(out cbEntryTable))
                return false;
            uint crc;
            if (!rdr.TryReadLeUInt32(out crc))
                return false;
            byte bProgramFlags;
            if (!rdr.TryReadByte(out bProgramFlags))
                return false;
            byte bAppFlags;
            if (!rdr.TryReadByte(out bAppFlags))
                return false;
            ushort iSegAutoData;
            if (!rdr.TryReadUInt16(out iSegAutoData))
                return false;
            ushort cbHeapSize;
            if (!rdr.TryReadUInt16(out cbHeapSize))
                return false;
            ushort cbStackSize;
            if (!rdr.TryReadUInt16(out cbStackSize))
                return false;
            ushort cs, ip;
            if (!rdr.TryReadUInt16(out ip) || !rdr.TryReadUInt16(out cs))
                return false;
            ushort ss, sp;
            if (!rdr.TryReadUInt16(out sp) || !rdr.TryReadUInt16(out ss))
                return false;
            if (!rdr.TryReadUInt16(out cSeg))
                return false;
            ushort cModules;
            if (!rdr.TryReadUInt16(out cModules))
                return false;
            ushort cbNonResidentNames;
            if (!rdr.TryReadUInt16(out cbNonResidentNames))
                return false;
            ushort offSegTable;
            if (!rdr.TryReadUInt16(out offSegTable))
                return false;
            ushort offRsrcTable;
            if (!rdr.TryReadUInt16(out offRsrcTable))
                return false;
            ushort offResidentNameTable;
            if (!rdr.TryReadUInt16(out offResidentNameTable))
                return false;
            ushort offModuleReferenceTable;
            if (!rdr.TryReadUInt16(out offModuleReferenceTable))
                return false;
            ushort offImportedNamesTable;
            if (!rdr.TryReadUInt16(out offImportedNamesTable))
                return false;
            uint offNonResidentNameTable;
            if (!rdr.TryReadUInt32(out offNonResidentNameTable))
                return false;
            ushort cMoveableEntryPoints;
            if (!rdr.TryReadUInt16(out cMoveableEntryPoints))
                return false;
            if (!rdr.TryReadUInt16(out cbFileAlignmentShift))
                return false;
            ushort cResourceTableEntries;
            if (!rdr.TryReadUInt16(out cResourceTableEntries))
                return false;
            byte bTargetOs;
            if (!rdr.TryReadByte(out bTargetOs))
                return false;
            byte bOsExeFlags;
            if (!rdr.TryReadByte(out bOsExeFlags))
                return false;
            ushort offGanglands;
            if (!rdr.TryReadUInt16(out offGanglands))
                return false;
            ushort cbGanglands;
            if (!rdr.TryReadUInt16(out cbGanglands))
                return false;
            ushort cbMinCodeSwapArea;
            if (!rdr.TryReadUInt16(out cbMinCodeSwapArea))
                return false;
            ushort wWindowsVersion;
            if (!rdr.TryReadUInt16(out wWindowsVersion))
                return false;

            LoadSegments(this.lfaNew + offSegTable);

            return true;
        }

        public override Program Load(Address addrLoad)
        {
            var cfgSvc = Services.RequireService<IConfigurationService>();
            var arch = cfgSvc.GetArchitecture("x86-protected-16");
            var platform = cfgSvc.GetEnvironment("win16").Load(Services, arch);

            return new Program(
                this.image,
                this.imageMap,
                arch,
                platform);
        }

        public override RelocationResults Relocate(Program program, Address addrLoad)
        {
            return new RelocationResults(
                new List<EntryPoint>(),
                new RelocationDictionary(),
                new List<Address>());
        }

        public class NeSegment
        {
            public ushort DataOffset;
            public ushort DataLength;
            public ushort Flags;
            public ushort Alloc;

            public uint LinearAddress;
            public Address Address;
        }

        void LoadSegments(uint offset)
        {
            NeSegment [] segments = ReadSegmentTable(offset, cSeg);
            var segFirst = segments[0];
            var segLast = segments[segments.Length - 1];
            this.image = new LoadedImage(
                segFirst.Address,
                new byte[segLast.LinearAddress + segLast.DataLength]);
            this.imageMap = image.CreateImageMap();
            foreach (var segment in segments)
            {
                LoadSegment(segment, image, imageMap);
            }
        }

        private NeSegment[] ReadSegmentTable(uint offset, int cSeg)
        {
            var segs = new List<NeSegment>(cSeg);
            var rdr = new LeImageReader(RawImage, offset);
            uint linAddress = 0x1000;
            for (int iSeg = 0; iSeg < cSeg; ++iSeg)
            {
                var seg = new NeSegment
                {
                    DataOffset = rdr.ReadLeUInt16(),
                    DataLength = rdr.ReadLeUInt16(),
                    Flags = rdr.ReadLeUInt16(),
                    Alloc = rdr.ReadLeUInt16()
                };
                uint cbSegmentPage = Math.Max(seg.Alloc, seg.DataLength);
                // Align to 4kb boundary.
                cbSegmentPage = (cbSegmentPage + 0xFFFu) & ~0xFFFu;   
                seg.LinearAddress = linAddress;
                seg.Address = Address.ProtectedSegPtr((ushort)((linAddress >> 9) | 7), 0);
                segs.Add(seg);
                linAddress += cbSegmentPage;
            }
            return segs.ToArray();
        }

        bool LoadSegment(NeSegment seg, LoadedImage loadedImage, ImageMap imageMap)
        {
            Array.Copy(
                RawImage,
                (uint)seg.DataOffset << this.cbFileAlignmentShift,
                loadedImage.Bytes,
                seg.LinearAddress,
                seg.DataLength);
            AccessMode access =
                (seg.Flags & 1) != 0
                    ? AccessMode.ReadWrite
                    : AccessMode.ReadExecute;
            imageMap.AddSegment(
                seg.Address,
                seg.Address.Selector.ToString("X4"),
                access,
                seg.DataLength);
            return true;
        } 
    }
}
