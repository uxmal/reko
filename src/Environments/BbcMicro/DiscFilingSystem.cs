#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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
using Reko.Core.Archives;
using Reko.Core.Loading;
using Reko.Core.Memory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

/*

9.2 Disc Catalogue

In Watford Electronics DFS there can be either one or two catalogues on 
the disc.  They are both in track 0, one occupying sectors 0 and 1, the
other occubying 2 and 3.  The two catalogues are identical in structure:

Sector 00:
00-07 First 8 bytes of the 12 byte disc title
08-0E First filename
0F    Directory of the first filename, top bit set if file is locked
10-1E Second filename
1F    Directory of second filename
.
.
.
Repeated for up to 31 files

Sector 01:
((H) indicates high order two bits of a number that doesn't fit one
or two byes.)
00-03 Last 4 bytes of disc title
04    Count of total number of writes to disc in packed BCD
05    Number of catalogue entries * 8
06    bits 0,1 - number of sectors (H)
      bits 4,5 - !BOOT start option (*OPT4 value)
07    Number of sectors on the disc
08-09 First file's load address (Low byte, High byte)
0A-0B First file's execute address
0C-0D First file's length in bytes
0E    bits 0,1 - First file's start sector (H)
      bits 2,3 - First file's load address (H)
      bits 4,5 - First file's length (H)
      bits 6,7 - First file's start sector (H)
0F    First file's start sector, 8 low bits of a 10 bit number
.
.
.
Repeated for up to 31 files

Sector 02:
This will contain the first file on a 31 file disc.  If this is a 62
file disc then it will contain:
00-07 8 * &AA recognition bytes
08..... As for sector 00

Sector 03:
If this is a 62 file disc:
00-03 5 * nulls
04..... As for sector 01


The disc is recognized as being a 62 file disc by the 8 &AA recognition 
bytes in the disc title area of the second catalogue.  This area would
otherwise be unused.

Standard DFS will not recognize the second catalogue, and will allow it
to be overwritten.
*/
namespace Reko.Environments.BbcMicro
{
    public class DiscFilingSystem : ImageLoader
    {
        public DiscFilingSystem(IServiceProvider services, string filename, byte[] rawBytes) : base(services, filename, rawBytes)
        {

        }

        public override Address PreferredBaseAddress
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public override Program Load(Address? addrLoad)
        {
            LoadDirectory();
            throw new NotImplementedException();
        }

        public override RelocationResults Relocate(Program program, Address addrLoad)
        {
            throw new NotImplementedException();
        }

        public ArchiveDirectoryEntry[] LoadDirectory()
        {
            var rdr = new LeImageReader(this.RawImage);
            rdr.Offset += 8;  // Skip title
            var entries = new DFSEntry[31];
            for (int i = 0; i < 31; ++i)
            {
                var bytes = rdr.ReadBytes(7);
                var fname = new string(bytes.Select(b => (char) b).ToArray());
                var bDir = rdr.ReadByte();
                var dir = (char) bDir & 0x7F;
                var locked = (bDir & 0x80) != 0;
                var de = new DFSEntry(fname)
                {
                };
                entries[i] = de;
            }

            // Sector 1
            Debug.Assert(rdr.Offset == 0x0100);
            rdr.Offset += 4; // Skip last 4 bytes of title
            rdr.ReadByte(); // # of writes
            int cEntries = rdr.ReadByte() / 8;
            byte cSectHi = rdr.ReadByte();
            byte cSectLo = rdr.ReadByte();

            for (int i = 0; i < 31; ++i)
            {
                var uAddrLoad = rdr.ReadUInt16();
                var uAddrExec = rdr.ReadUInt16();
                var cbLength = rdr.ReadUInt16();
                var sectHi = rdr.ReadByte();
                var sectLo = rdr.ReadByte();
                var de = entries[i];
                de.LoadAddress = Address.Ptr16(uAddrLoad);
                de.ExecAddress = Address.Ptr16(uAddrLoad);
                de.Length = cbLength;
            }
            return entries.Take(cEntries).ToArray();
        }
    }

    public class DFSEntry : ArchiveDirectoryEntry
    {
        public DFSEntry(string name)
        {
            this.Name = name;
            this.LoadAddress = null!;
            this.ExecAddress = null!;
        }

        public Address LoadAddress { get; set; }
        public Address ExecAddress { get; set; }
        public uint Length { get; set; }
        public string Name { get; set; }
    }
}
