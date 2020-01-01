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
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;

namespace Reko.ImageLoaders.IntelHex
{
    public class HexLoader : ImageLoader
    {
        private DecompilerEventListener listener;

        public HexLoader(IServiceProvider services, string filename, byte[] imgRaw) : base(services, filename, imgRaw)
        {
        }

        public override Address PreferredBaseAddress { get; set; }

        public override Program Load(Address addrLoad)
        {
            throw new NotImplementedException();
        }

        public override Program Load(Address addrLoad, IProcessorArchitecture arch, IPlatform platform)
        {
            this.listener = Services.RequireService<DecompilerEventListener>();
            var loaded = new MemoryStream();
            int lineNo = 0;
            using (var rdr = new StreamReader(new MemoryStream(RawImage)))
            {
                for (;;)
                {
                    string line = rdr.ReadLine();
                    if (line == null)
                        break;
                    ++lineNo;
                    if (!ProcessLine(line, lineNo, loaded))
                        break;
                }
            }

            var mem = new MemoryArea(addrLoad, loaded.ToArray());
            var seg = new ImageSegment("CODE", mem, AccessMode.ReadWriteExecute);
            var segs = new SegmentMap(addrLoad, seg);
            return new Program(segs, arch, platform);
        }

        private bool ProcessLine(string line, int num, MemoryStream loaded)
        {
            line = line.TrimEnd();
            if (line.Length == 0 || line[0] != ':')
            {
                listener.Error(new NullCodeLocation(""), "Line {0} is invalid.", num);
                return false;
            }
            uint? byteCount = Unhex(line, 1, 2);
            if (byteCount == null)
            {
                // Invalid byte count.
                listener.Error(new NullCodeLocation(""), "Line {0} is invalid.", num);
                return false;
            }
            if (line.Length != 1 + 2 * (1 + 2 + 1 + byteCount + 1))
            {
                // Invalid line length
                listener.Error(new NullCodeLocation(""), "Line {0} is invalid.", num);
                return false;
            }
            uint? address = Unhex(line, 3, 4);
            if (address == null)
            {
                // Invalid address
                listener.Error(new NullCodeLocation(""), "Line {0} is invalid.", num);
                return false;
            }
            uint? recordType = Unhex(line, 7, 2);
            if (recordType == null)
            {
                // Invalid record type
                listener.Error(new NullCodeLocation(""), "Line {0} is invalid.", num);
                return false;
            }
            for (int i = 0; i < byteCount; ++i)
            {
                uint? b = Unhex(line, 9 + i * 2, 2);
                if (b == null)
                {
                    // invalid data
                    listener.Error(new NullCodeLocation(""), "Line {0} is invalid.", num);
                    return false;
                }
                loaded.WriteByte((byte)b);
            }
            return true;
        }

        private uint? Unhex(string line, int istart, int digits)
        {
            if (istart + digits >= line.Length)
                return null;
            uint u = 0;
            for (int i = istart; i < istart + digits; i += 2)
            {
                uint? hi = HexDigit(line[i]);
                uint? lo = HexDigit(line[i+1]);
                if (hi == null || lo == null)
                    return null;
                u = (u << 8) | (hi.Value << 4) | lo.Value;
            }
            return u;
        }

        public static uint? HexDigit(char digit)
        {
            switch (digit)
            {
            case '0': case '1': case '2': case '3': case '4': 
            case '5': case '6': case '7': case '8': case '9':
                return (uint) (digit - '0');
            case 'A': case 'B': case 'C': case 'D': case 'E': case 'F':
                return (uint) ((digit - 'A') + 10);
            case 'a': case 'b': case 'c': case 'd': case 'e': case 'f':
                return (uint) ((digit - 'a') + 10);
            default:
                return null;
            }
        }

        public override RelocationResults Relocate(Program program, Address addrLoad)
        {
            return new RelocationResults(new List<ImageSymbol>(), new SortedList<Address, ImageSymbol>());
        }
    }
}
