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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.Omf
{
    /// <summary>
    /// Loads OMF object files.
    /// </summary>
    public class OmfLoader : ImageLoader
    {
        // http://www.azillionmonkeys.com/qed/Omfg.pdf
        public OmfLoader(IServiceProvider services, string filename, byte[] rawImage) : base(services, filename, rawImage)
        {
        }

        public override Address PreferredBaseAddress
        {
            get
            {
                return Address.SegPtr(0x0800, 0);
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public override Program Load(Address addrLoad)
        {
            throw new NotImplementedException();
        }

        private (byte, byte[]) ReadRecord(LeImageReader rdr)
        {
            if (!rdr.TryReadByte(out var type))
                throw new BadImageFormatException();
            if (!rdr.TryReadUInt16(out var length))
                throw new BadImageFormatException();
            var bytes = rdr.ReadBytes(length);
            return (type, bytes);
        }

        public override RelocationResults Relocate(Program program, Address addrLoad)
        {
            throw new NotImplementedException();
        }

        private const byte THEADR = 0x80;    // Translator Header Record
        private const byte LHEADR = 0x82;    // Library Module Header Record
        private const byte COMENT = 0x88;    // Comment Record(Including all comment class extensions)
        private const byte MODEND = 0x8A/0x8B;    // Module End Record
        private const byte EXTDEF = 0x8C;    // External Names Definition Record
        private const byte PUBDEF = 0x90/0x91;    // Public Names Definition Record
        private const byte LINNUM = 0x94/0x95;    // Line Numbers Record
        private const byte LNAMES = 0x96;    // List of Names Record
        private const byte SEGDEF = 0x98/0x99;    // Segment Definition Record
        private const byte GRPDEF = 0x9A;    // Group Definition Record
        private const byte FIXUPP = 0x9C/0x9D;    // Fixup Record
        private const byte LEDATA = 0xA0/0xA1;    // Logical Enumerated Data Record
        private const byte LIDATA = 0xA2/0xA3;    // Logical Iterated Data Record
        private const byte COMDEF = 0xB0;    // Communal Names Definition Record
        private const byte BAKPAT = 0xB2/0xB3;    // Backpatch Record
        private const byte LEXTDEF = 0xB4;    // Local External Names Definition Record
        private const byte LPUBDEF = 0xB6/0xB7;    // Local Public Names Definition Record
        private const byte LCOMDEF = 0xB8;    // Local Communal Names Definition Record
        private const byte CEXTDEF = 0xBC;    // COMDAT External Names Definition Record
        private const byte COMDAT = 0xC2/0xC3;    // Initialized Communal Data Record
        private const byte LINSYM = 0xC4/0xC5;    // Symbol Line Numbers Record
        private const byte ALIAS = 0xC6;    // Alias Definition Record
        private const byte NBKPAT = 0xC8/0xC9;    // Named Backpatch Record
        private const byte LLNAMES = 0xCA;    // Local Logical Names Definition Record
        private const byte VERNUM = 0xCC;    // OMF Version Number Record
        private const byte VENDEXT = 0xCE;    // Vendor-specific OMF Extension Record
        private const byte LibraryHeader = 0xF0;    // Header Record
        private const byte LibraryEnd = 0xF1;    // End Record
    }
}