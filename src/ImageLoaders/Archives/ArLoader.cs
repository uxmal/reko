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
using Reko.Core.Services;
using Reko.ImageLoaders.Archives.Ar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Reko.ImageLoaders.Archives
{
    public class ArLoader : ImageLoader
    {
        private const string HeaderSignature = "!<arch>\n";
        private readonly Dictionary<string, uint> symbols;
        private readonly IEventListener listener;
        private byte[]? longFilenames;

        public ArLoader(IServiceProvider services, ImageLocation location, byte[] imgRaw)
            : base(services, location, imgRaw)
        {
            symbols = new Dictionary<string, uint>();
            this.listener = services.GetService<IEventListener>() ?? NullEventListener.Instance;
        }

        public override ILoadedImage Load(Address? addrLoad)
        {
            var rdr = new ByteImageReader(RawImage);
            ReadHeader(rdr);
            var archive = new ArArchive(this.ImageLocation);
            ReadFiles(rdr, archive);
            return archive;
        }

        public void ReadHeader(ImageReader rdr)
        {
            var aSignature = rdr.ReadBytes(8);
            var sSignature = Encoding.ASCII.GetString(aSignature);
            if (string.Compare(sSignature, HeaderSignature, StringComparison.InvariantCulture) != 0)
                throw new BadImageFormatException("Invalid Unix archive file signature.");
        }

        private void ReadFiles(ByteImageReader rdr, ArArchive archive)
        {
            for (; ;)
            {
                var header = ArFileHeader.Load(rdr);
                if (header is null)
                    return;
                ReadFile(header, rdr, archive);
            }
        }

        private void ReadFile(ArFileHeader fileHeader, ByteImageReader rdr, ArArchive archive)
        {
            if (!int.TryParse(fileHeader.FileSize, out int dataSize))
                throw new BadImageFormatException("Invalid archive file header.");
            if (dataSize + rdr.Offset > rdr.Bytes.Length)
                throw new BadImageFormatException("The archive file is corrupt.");

            string name = fileHeader.Name;
            if (name.StartsWith("// "))
            {
                if (longFilenames is not null)
                    throw new BadImageFormatException("Long file names 'file' appears more than once.");
                longFilenames = rdr.ReadBytes(dataSize);
                AlignReader(rdr);
                return;
            }
            else if (name.StartsWith('/') && name.Length > 1 && char.IsDigit(name[0]))
            {
                if (longFilenames is null)
                    throw new BadImageFormatException("Cannot read long file names.");
                if (!int.TryParse(name.AsSpan(1), out var nameOffset))
                    throw new BadImageFormatException("Non-numeric long file name reference.");
                int iStart = nameOffset;
                int i;
                for (i = nameOffset; i < longFilenames.Length && longFilenames[i] != '\n'; ++i)
                    ;
                name = Encoding.ASCII.GetString(longFilenames, iStart, i - iStart);
            }
            else if (name.StartsWith("/ ") || name.StartsWith("__.SYMDEF"))
            {
                // System V symbol lookup table.
                var symbolData = rdr.ReadBytes(dataSize);
                ReadSymbolTable(symbolData);
                return;
            }
            else if (name.StartsWith("#1/ "))
            {
                // File name length follows #1/ as decimal digits.
                // This variant is used by Mac and some versions of BSD.
                var fileNameLength = Convert.ToInt32(name + 3);
                long fileDataOffset = rdr.Offset + fileNameLength;
                name = Encoding.ASCII.GetString(rdr.ReadBytes(fileNameLength));
                if (dataSize > fileNameLength)
                {
                    // The length of the name is included in the dataSize.
                    dataSize -= fileNameLength;
                }
                rdr.Offset = fileDataOffset;
            }
            else
            {
                // Ordinary short name
                char[] charsToTrim = { '/', ' ' };
                name = name.TrimEnd(charsToTrim);
            }

            try
            {
                archive.AddFile(name, (a, p, name) => new ArFile(a, p, name, rdr, rdr.Offset, dataSize));
            }
            catch (DuplicateFilenameException ex)
            {
                listener.Warn(ex.Message);
            }
            rdr.Offset += dataSize;
            AlignReader(rdr);
        }

        private static void AlignReader(ByteImageReader rdr)
        {
            if ((rdr.Offset & 1) != 0)
                rdr.Offset += 1;
        }

        private Dictionary<string, uint> ReadSymbolTable(byte[] fileData)
        {
            var rdr = new ByteImageReader(fileData);
            var nEntries = rdr.ReadBeUInt32();
            var offsets = new uint[nEntries];
            var dict = new Dictionary<string, uint>();
            for (int i = 0; i < nEntries; ++i)
            {
                offsets[i] = rdr.ReadBeUInt32();
            }
            for (int i = 0; i < nEntries; ++i)
            {
                var name = ReadNullTerminatedString(rdr);
                dict[name] = offsets[i];
            }
            return dict;
        }

        private static string ReadNullTerminatedString(ByteImageReader rdr)
        {
            var iStart = rdr.Offset;
            while (rdr.IsValid && rdr.ReadByte() != 0)
                ;
            return Encoding.ASCII.GetString(rdr.Bytes, (int) iStart, (int) (rdr.Offset - iStart) - 1);
        }
    }
}
