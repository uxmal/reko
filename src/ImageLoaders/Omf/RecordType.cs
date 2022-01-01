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

using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.ImageLoaders.Omf
{
    public enum RecordType : byte
    {
        THEADR = 0x80,           // Translator Header Record
        LHEADR = 0x82,           // Library Module Header Record
        COMENT = 0x88,           // Comment Record(Including all comment class extensions)
        MODEND = 0x8A,           // Module End Record
        MODEND_a = 0x8B,         // Module End Record
        EXTDEF = 0x8C,           // External Names Definition Record
        PUBDEF = 0x90,           // Public Names Definition Record
        PUBDEF_a = 0x91,         // Public Names Definition Record
        LINNUM = 0x94,           // Line Numbers Record
        LINNUM_a = 0x95,         // Line Numbers Record
        LNAMES = 0x96,           // List of Names Record
        SEGDEF = 0x98,           // Segment Definition Record
        SEGDEF_a = 0x99,         // Segment Definition Record
        GRPDEF = 0x9A,           // Group Definition Record
        FIXUPP = 0x9C,           // Fixup Record
        FIXUPP_a = 0x9D,         // Fixup Record
        LEDATA = 0xA0,           // Logical Enumerated Data Record
        LEDATA_a = 0xA1,         // Logical Enumerated Data Record
        LIDATA = 0xA2,           // Logical Iterated Data Record
        LIDATA_a = 0xA3,         // Logical Iterated Data Record
        COMDEF = 0xB0,           // Communal Names Definition Record
        BAKPAT = 0xB2,           // Backpatch Record
        BAKPAT_a = 0xB3,         // Backpatch Record
        LEXTDEF = 0xB4,          // Local External Names Definition Record
        LPUBDEF = 0xB6,          // Local Public Names Definition Record
        LPUBDEF_a = 0xB7,        // Local Public Names Definition Record
        LCOMDEF = 0xB8,          // Local Communal Names Definition Record
        CEXTDEF = 0xBC,          // COMDAT External Names Definition Record
        COMDAT = 0xC2,           // Initialized Communal Data Record
        COMDAT_a = 0xC3,         // Initialized Communal Data Record
        LINSYM = 0xC4,           // Symbol Line Numbers Record
        LINSYM_a = 0xC5,         // Symbol Line Numbers Record
        ALIAS = 0xC6,            // Alias Definition Record
        NBKPAT = 0xC8,           // Named Backpatch Record
        NBKPAT_a = 0xC9,         // Named Backpatch Record
        LLNAMES = 0xCA,          // Local Logical Names Definition Record
        VERNUM = 0xCC,           // OMF Version Number Record
        VENDEXT = 0xCE,          // Vendor-specific OMF Extension Record
        LibraryHeader = 0xF0,    // Header Record
        LibraryEnd = 0xF1,       // End Record
    }
}
