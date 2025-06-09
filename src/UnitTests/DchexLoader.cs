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
using Reko.ImageLoaders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Reko.UnitTests
{
    class DchexLoader : ProgramImageLoader
    {
        private Address? addrStart;
        private MemoryStream memStm;
        private Program results;

        public DchexLoader(IServiceProvider services, ImageLocation imageUri, byte[] imgRaw) :
            base(services, imageUri, imgRaw)
        {
            var filename = imageUri.FilesystemPath;
            using (TextReader rdr = new StreamReader(filename))
            {
                LoadFromFile(rdr);
            }
        }

        public override Address PreferredBaseAddress
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public override Program LoadProgram(Address? addrLoad)
        {
            return results;
        }

        private void LoadFromFile(TextReader rdr)
        {
            var arch = GetArchitecture(ReadLine(rdr).Trim());
            for (; ; )
            {
                var line = ReadLine(rdr);
                if (line is null)
                    break;
                ProcessLine(line);
            }
            if (addrStart is null)
                throw new BadImageFormatException("Address has not been set.");
            var mem = new ByteMemoryArea(addrStart.Value, memStm.ToArray());
            var segmentMap = new SegmentMap(
                mem.BaseAddress,
                new ImageSegment("code", mem, AccessMode.ReadWriteExecute));
            results = new Program(
                new ByteProgramMemory(segmentMap),
                arch,
                new DefaultPlatform(Services, arch));
        }

        private IProcessorArchitecture GetArchitecture(string archName)
        {
            switch (archName)
            {
            case "m68k": return new Reko.Arch.M68k.M68kArchitecture(Services, "m68k", new Dictionary<string, object>());
            default: throw new NotImplementedException();
            }
        }

        private void ProcessLine(string line)
        {
            // Get rid of comments.
            var segs = line.Split(';'); 

            // Tokenize
            var tokens = Regex.Replace(line.TrimEnd(), " +", " ").Split(' ');
            // If line didn't start with a space, the first token is address.
            int i = 0;
            if (tokens.Length > 1 && line[0] != ' ')
            {
                Address.TryParse32(tokens[0], out var address);
                if (this.addrStart is null)
                {
                    addrStart = address;
                    memStm = new MemoryStream();
                }
                else
                {
                    memStm.Position = address - addrStart.Value;
                }
                i = 1;
            }
            for (; i < tokens.Length; ++i)
            {
                memStm.WriteByte(ToByte(tokens[i]));
            }
        }

        private byte ToByte(string p)
        {
            return (byte) ((HexDigit(p[0]) << 4) | HexDigit(p[1]));
        }

        private int HexDigit(int ch)
        {
            if ('0' <= ch && ch <= '9')
                return (ch - '0');
            else if ('A' <= ch && ch <= 'F')
                return (10 + ch - 'A');
            else if ('a' <= ch && ch <= 'f')
                return (10 + ch - 'a');

            throw new BadImageFormatException();
        }

        private string ReadLine(TextReader rdr)
        {
            for (; ; )
            {
                var line = rdr.ReadLine();
                if (line is null)
                    return null;
                var i = line.IndexOf(';');
                if (i != 0)
                    return line;
            }
        }
    }
}
