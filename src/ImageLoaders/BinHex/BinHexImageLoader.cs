#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Environments.MacOS.Classic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Reko.ImageLoaders.BinHex
{
    public class BinHexImageLoader : ImageLoader
    {
        private static readonly Address PreferredBaseAddress = Address.Ptr32(0x00100000);

        public BinHexImageLoader(IServiceProvider services, ImageLocation imageLocation, byte [] imgRaw) 
            : base(services, imageLocation, imgRaw)
        {
        }

        public override ILoadedImage Load(Address? addrLoad)
        {
            addrLoad ??= PreferredBaseAddress;
            BinHexDecoder dec = new BinHexDecoder(new StringReader(Encoding.ASCII.GetString(RawImage)));
            IEnumerator<byte> stm = dec.GetBytes().GetEnumerator();
            BinHexHeader hdr = LoadBinHexHeader(stm);
            byte[] dataBytes = LoadFork(hdr.DataForkLength, stm);
            byte[] rsrcBytes = LoadFork(hdr.ResourceForkLength, stm);

            var cfgSvc = Services.RequireService<IConfigurationService>();
            var arch = cfgSvc.GetArchitecture("m68k")!;
            var platform = (MacOSClassic) cfgSvc.GetEnvironment("macOs").Load(Services, arch);
            if (hdr.FileType == "PACT")
            {
                Cpt.CompactProArchive archive = new Cpt.CompactProArchive(base.ImageLocation, arch, platform);
                archive.Load(new MemoryStream(dataBytes));
                return archive;
            }
            var rsrcFork = new ResourceFork(platform, rsrcBytes);
            var bmem = new ByteMemoryArea(addrLoad, rsrcBytes);
            Program program;
            SegmentMap segmentMap;
            if (hdr.FileType == "MPST" || hdr.FileType == "APPL")
            {
                segmentMap = new SegmentMap(addrLoad);
            }
            else
            {
                segmentMap = new SegmentMap(
                    bmem.BaseAddress,
                    new ImageSegment("", bmem, AccessMode.ReadWriteExecute));
            }
            var memory = new ProgramMemory(segmentMap);
            program = new Program(memory, arch, platform);
            rsrcFork.AddResourcesToImageMap(addrLoad, bmem, program);
            return program;
        }

        private byte[] LoadFork(int size, IEnumerator<byte> stm)
        {
            byte [] fork = new byte[size];
            for (int i = 0; i < fork.Length; ++i)
            {
                if (!stm.MoveNext())
                    throw FormatError();
                fork[i] = stm.Current;
            }
            ReadUInt16BE(stm);      // CRC
            return fork;
        }

        public BinHexHeader LoadBinHexHeader(IEnumerator<byte> stm)
        {
            BinHexHeader hdr = new BinHexHeader();
            Encoding ascii = Encoding.ASCII;

            if (!stm.MoveNext())
                throw FormatError();
            byte len = stm.Current;

            hdr.FileName = ReadString(len, stm, ascii);

            if (!stm.MoveNext())
                throw FormatError();

            hdr.FileType = ReadString(4, stm, ascii);
            hdr.FileCreator = ReadString(4, stm, ascii);
            hdr.Flags = ReadUInt16BE(stm);
            hdr.DataForkLength = ReadInt32BE(stm);
            hdr.ResourceForkLength = ReadInt32BE(stm);
            hdr.HeaderCRC = ReadUInt16BE(stm);

            return hdr;
        }

        private Exception FormatError()
        {
            throw new FormatException("BinHex header malformed.");
        }

        private string ReadString(int count, IEnumerator<byte> stm, Encoding ascii)
        {
            byte[] bytes = new byte[count];
            for (int i = 0; i < bytes.Length; ++i)
            {
                if (!stm.MoveNext())
                    throw FormatError();
                bytes[i] = stm.Current;
            }
            return ascii.GetString(bytes);
        }

        private int ReadInt32BE(IEnumerator<byte> stm)
        {
            int n = 0;
            if (!stm.MoveNext())
                throw FormatError();
            n = n << 8 | stm.Current;
            if (!stm.MoveNext())
                throw FormatError();
            n = n << 8 | stm.Current;
            if (!stm.MoveNext())
                throw FormatError();
            n = n << 8 | stm.Current;
            if (!stm.MoveNext())
                throw FormatError();
            n = n << 8 | stm.Current;
            return n;
        }

        private ushort ReadUInt16BE(IEnumerator<byte> stm)
        {
            int n = 0;
            if (!stm.MoveNext())
                throw FormatError();
            n = n << 8 | stm.Current;
            if (!stm.MoveNext())
                throw FormatError();
            return (ushort)n;
        }
    }
}