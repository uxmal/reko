#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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

using Reko.Arch.Pdp10;
using Reko.Core;
using Reko.Core.Loading;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Environments.Pdp10Env.FileFormats
{
    /// <summary>
    /// This class reads 36-bit words encoded in the ANSI/ASCII format.
    ///
    /// A 36-bit word AAAAAAABBBBBBBCCCCCCCDDDDDDDEEEEEEEF is stored as five
    /// octets.  X means written as zero, ignored when read.
    ///
    /// XAAAAAAA
    /// XBBBBBBB
    /// XCCCCCCC
    /// XDDDDDDD
    /// FEEEEEEE
    /// </summary>
    public class AsciiLoader : ProgramImageLoader
    {
        public AsciiLoader(IServiceProvider services, ImageLocation imgLocation, byte[] imgRaw)
            : base(services, imgLocation, imgRaw)
        {
        }

        public override Address PreferredBaseAddress { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override Program LoadProgram(Address? address)
        {
            var arch = new Pdp10Architecture(Services, "pdp10", new Dictionary<string, object>());
            var platform = new Pdp10Platform(Services, arch);
            return LoadProgram(address!, arch, platform);
        }

        public override Program LoadProgram(Address addrLoad, IProcessorArchitecture arch, IPlatform platform)
        {
            var words = new List<ulong>();
            int f = 0;
            for (; ; )
            {
                var word = ReadWord(ref f);
                if (word == ~0ul)
                    break;
                words.Add(word);
            }
            var mem = new Word36MemoryArea(addrLoad, words.ToArray());
            var seg = new ImageSegment("core", mem, AccessMode.ReadWriteExecute);
            var map = new SegmentMap(seg);
            return new Program(map, arch, platform);
        }

        private uint ReadByte(ref int f)
        {
            if (f >= RawImage.Length)
                return 0;
            return RawImage[f++];
        }

        private ulong ReadWord(ref int f)
        {
            ulong word = 0;
            ulong x;

            if (f >= RawImage.Length)
                return ~0ul;

            x = ReadByte(ref f); word += (x & 0x7F) << 29;
            if (f >= RawImage.Length)
                return ~0ul;
            x = ReadByte(ref f); word += (x & 0x7F) << 22;
            x = ReadByte(ref f); word += (x & 0x7F) << 15;
            x = ReadByte(ref f); word += (x & 0x7F) << 8;
            x = ReadByte(ref f); word += (x & 0x7F) << 1;
            word += (x & 0x80) >> 7;

            return word;
        }
    }
}