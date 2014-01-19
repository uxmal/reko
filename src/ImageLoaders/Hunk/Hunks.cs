#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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
using System.Linq;
using System.Text;

namespace Decompiler.ImageLoaders.Hunk
{
    public enum HunkType
    {
        HUNK_UNIT = 999,
        HUNK_NAME = 1000,
        HUNK_CODE = 1001,
        HUNK_DATA = 1002,
        HUNK_BSS = 1003,
        HUNK_ABSRELOC32 = 1004,
        HUNK_RELRELOC16 = 1005,
        HUNK_RELRELOC8 = 1006,
        HUNK_EXT = 1007,
        HUNK_SYMBOL = 1008,
        HUNK_DEBUG = 1009,
        HUNK_END = 1010,
        HUNK_HEADER = 1011,

        HUNK_OVERLAY = 1013,
        HUNK_BREAK = 1014,
        HUNK_DREL32 = 1015,
        HUNK_DREL16 = 1016,
        HUNK_DREL8 = 1017,
        HUNK_LIB = 1018,
        HUNK_INDEX = 1019,
        HUNK_RELOC32SHORT = 1020,
        HUNK_RELRELOC32 = 1021,
        HUNK_ABSRELOC16 = 1022,

        HUNK_PPC_CODE = 1257,
        HUNK_RELRELOC26 = 1260,
    }

    partial class HunkLoader
    {
        //private const int RESULT_UNSUPPORTED_HUNKS = 3;



        private const int TYPE_UNKNOWN = 0;
        private const int TYPE_LOADSEG = 1;
        private const int TYPE_UNIT = 2;
        private const int TYPE_LIB = 3;

        private Dictionary<int, string> type_names = new Dictionary<int, string>
        {
            { TYPE_UNKNOWN, "TYPE_UNKNOWN"},
            { TYPE_LOADSEG, "TYPE_LOADSEG"},
            { TYPE_UNIT, "TYPE_UNIT"},
            { TYPE_LIB, "TYPE_LIB"}
        };

        private HunkType[] unit_valid_main_hunks = new[] {
            HunkType.HUNK_CODE,
            HunkType.HUNK_DATA,
            HunkType.HUNK_BSS,
            HunkType.HUNK_PPC_CODE,
        };

        private HunkType[] unit_valid_extra_hunks = new[] {
            HunkType.HUNK_DEBUG,
            HunkType.HUNK_SYMBOL,
            HunkType.HUNK_NAME,
            HunkType.HUNK_EXT,
            HunkType.HUNK_ABSRELOC32,
            HunkType.HUNK_RELRELOC16,
            HunkType.HUNK_RELRELOC8,
            HunkType.HUNK_DREL32,
            HunkType.HUNK_DREL16,
            HunkType.HUNK_DREL8,
            HunkType.HUNK_RELOC32SHORT,
            HunkType.HUNK_RELRELOC32,
            HunkType.HUNK_ABSRELOC16,
            HunkType.HUNK_RELRELOC26,
        };
    }

    public class Hunk
    {
        public HunkType HunkType;
        public string Name;
        public uint FileOffset;
        public bool inLib;
        public string fixes;
        public uint Size;
        public uint alloc_size;
        public List<Reference> refs;
        public string MemoryFlags;
        public int hunk_no;
        public Hunk index_hunk;
        public List<Hunk> defs;
        public int hunk_lib_offset;
        public byte[] custom_data;

        public override string ToString()
        {
            return string.Format("{0} ({1})", hunk_no, string.IsNullOrEmpty(Name) ? "<unnamed>" : Name);
        }
    }

    public class HeaderHunk : Hunk
    {
        public List<string> HunkNames;
        public uint table_size;
        public uint FirstHunkId;
        public uint LastHunkId;
        public List<HunkInfo> HunkSizes;
    }

    public class BssHunk : Hunk 
    {
    }

    class DebugHunk : Hunk
    {
        public uint debug_offset;
        public string debug_type;
        public string src_file;
        public Dictionary<int, uint> src_map;
        public byte[] data;
    }

    class DefHunk : Hunk
    {
        public short value;
    }

    class ExtHunk : Hunk
    {
        public List<ExtObject> ext_def;
        public List<ExtObject> ext_ref;
        public List<ExtObject> ext_common;
    }

    class IndexHunk : Hunk
    {
        public List<Unit> units;
    }

    public class LibHunk : Hunk
    {
        //public uint lib_file_offset;
        public List<Unit> units;
        public int lib_no;
        public Hunk index;
        public uint lib_file_offset;
    }

    class OverlayHunk : Hunk
    {
        public byte[] ov_data;
        public bool ov_std;
    }

    public class RelocHunk : Hunk
    {
        public Dictionary<int, List<uint>> reloc;
    }


    class SymbolHunk : Hunk
    {
        public Dictionary<string, int> symbols;
    }

    public class TextHunk : Hunk
    {
        public byte[] Data;
        public uint dataFileOffset;
    }


    //$TODO: make this not derive from hunk
    public class HunkInfo // : Hunk
    {
        public uint Size;
        public string MemoryFlags;
    }
}