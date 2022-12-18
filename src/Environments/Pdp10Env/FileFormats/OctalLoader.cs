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

using Reko.Arch.Pdp;
using Reko.Arch.Pdp.Memory;
using Reko.Core;
using Reko.Core.Loading;
using System;
using System.Collections.Generic;
using System.IO;

namespace Reko.Environments.Pdp10Env.FileFormats
{
    /// <summary>
    /// Loads a file where words have been encoded as ASCII octal digits. Each line is one word.
    /// </summary>
    public class OctalLoader : ProgramImageLoader
    {
        public OctalLoader(IServiceProvider services, ImageLocation imgLocation, byte[] imgRaw)
            : base(services, imgLocation, imgRaw)
        {
        }

        public override Address PreferredBaseAddress { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override Program LoadProgram(
            Address addrLoad, 
            IProcessorArchitecture arch, 
            IPlatform platform,
            List<UserSegment> userSegments)
        {
            var words = new List<ulong>();
            using var f = new StreamReader(new MemoryStream(this.RawImage));
            for (; ; )
            {
                var word = ReadWord(f);
                if (word == ~0ul)
                    break;
                words.Add(word);
            }
            var mem = new Word36MemoryArea(addrLoad, words.ToArray());
            var seg = new ImageSegment("core", mem, AccessMode.ReadWriteExecute);
            var map = new SegmentMap(seg);
            return new Program(map, arch, platform);
        }

        public override Program LoadProgram(Address? address)
        {
            var arch = new Pdp10Architecture(Services, "pdp10", new Dictionary<string, object>());
            var platform = new Pdp10Platform(Services, arch);
            return LoadProgram(address!, arch, platform, new());
        }

        private ulong ReadWord(TextReader f)
        {
            for (; ; )
            {
                var line = f.ReadLine();
                if (line is null)
                    return ~0ul;

                int i = 0;
                while (char.IsWhiteSpace(line[i]))
                    ++i;

                ulong word = 0;
                for (i = 0; i < 12; i++)
                {
                    var digit = (uint) (line[i] - '0');
                    if (digit >= 8)
                        continue;
                    word = (word << 3) + digit;
                }
                if (i < line.Length && (uint) (line[i] - '0') >= 8)
                    continue;
                return word;
            }
        }
    }
}
