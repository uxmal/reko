/* 
 * Copyright (C) 1999-2009 John Källén.
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

using Decompiler.Arch.M68k;
using Decompiler.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace Decompiler.ImageLoaders.BinHex
{
    public class BinHexImageLoader : ImageLoader
    {
        public BinHexImageLoader(byte [] imgRaw) : base(imgRaw)
        {
        }

        public override IProcessorArchitecture Architecture
        {
            get { return new M68kArchitecture(); }
        }

        public override ProgramImage Load(Address addrLoad)
        {
            BinHexDecoder dec = new BinHexDecoder(new StringReader(Encoding.ASCII.GetString(RawImage)));
            IEnumerator<byte> stm = dec.GetBytes().GetEnumerator();
            BinHexHeader hdr = LoadBinHexHeader(stm);
            byte[] dataFork = LoadFork(hdr.DataForkLength, stm);
            byte[] rsrcFork = LoadFork(hdr.ResourceForkLength, stm);

            if (hdr.FileType == "PACT")
            {
//                Cpt.CptCompressor comp = new Cpt.CptCompressor(dataFork);
//                comp.cpt();
            }
            return new ProgramImage(addrLoad, dataFork);
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

        public override ProgramImage LoadAtPreferredAddress()
        {
            throw new NotImplementedException();
        }

        public override Platform Platform
        {
            get { return null; }        //$TODO: should be MacOsClassic
        }

        public override Address PreferredBaseAddress
        {
            get { return new Address(0x10000000); }
        }

        public override void Relocate(Address addrLoad, List<EntryPoint> entryPoints, RelocationDictionary relocations)
        {
        }

        [Obsolete]
        public BinHexHeader LoadBinHexHeader(BinHexDecoder decoder)
        {
            IEnumerator<byte> stm = decoder.GetBytes().GetEnumerator();
            return LoadBinHexHeader(stm);
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