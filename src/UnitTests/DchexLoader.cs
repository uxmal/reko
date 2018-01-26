﻿#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
using Reko.ImageLoaders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Reko.UnitTests
{
    class DchexLoader : ImageLoader
    {
        private Address addrStart;
        private MemoryStream memStm;
        private Program results;

        public DchexLoader(string filename, IServiceProvider services, byte[] imgRaw) :
            base(services, filename, imgRaw)
        {
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

        public override Program Load(Address addrLoad)
        {
            return results;
        }

        private void LoadFromFile(TextReader rdr)
        {
            var arch = GetArchitecture(ReadLine(rdr).Trim());
            for (; ; )
            {
                var line = ReadLine(rdr);
                if (line == null)
                    break;
                ProcessLine(line);
            }
            var mem = new MemoryArea(addrStart, memStm.ToArray());
            results = new Program(
                new SegmentMap(
                    mem.BaseAddress,
                    new ImageSegment("code", mem, AccessMode.ReadWriteExecute)),
                arch,
                new DefaultPlatform(Services, arch));
        }

        private IProcessorArchitecture GetArchitecture(string archName)
        {
            switch (archName)
            {
            case "m68k": return new Reko.Arch.M68k.M68kArchitecture();
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
                if (this.addrStart == null)
                {
                    addrStart = address;
                    memStm = new MemoryStream();
                }
                else
                {
                    memStm.Position = address - addrStart;
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
                if (line == null)
                    return null;
                var i = line.IndexOf(';');
                if (i != 0)
                    return line;
            }
        }

        public override RelocationResults Relocate(Program program, Address addrLoad)
        {
            return new RelocationResults(new List<ImageSymbol>(), new SortedList<Address, ImageSymbol>());
        }
    }
}
