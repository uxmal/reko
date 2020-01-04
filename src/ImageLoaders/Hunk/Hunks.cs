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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.Hunk
{
    public enum FileType
    {
        TYPE_UNKNOWN = 0,
        TYPE_LOADSEG = 1,
        TYPE_UNIT = 2,
        TYPE_LIB = 3,
    }

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

    public partial class Hunk
    {
        public static int HUNKF_ADVISORY = 1 << 29;
        public static int HUNKF_CHIP = 1 << 30;
        public static int HUNKF_FAST = 1 << 31;
        public static int HUNKF_ALL = HUNKF_ADVISORY | HUNKF_CHIP | HUNKF_FAST;
        public static int HUNK_TYPE_MASK = 65535;
        public static int HUNK_FLAGS_MASK = -65536;

        public HunkType HunkType;
        public uint FileOffset;
        public int size;
        public int alloc_size;
        public string memf;
        public bool in_lib;
        public string fixes;
        public string Name;
        public int hunk_no;
        public byte[] Data;
        public uint hunk_lib_offset;
        public IHunk index_hunk;

        public override string ToString()
        {
            return string.Format("{0} ({1})", hunk_no, string.IsNullOrEmpty(Name) ? "<unnamed>" : Name);
        }
    }

    public class HeaderHunk : Hunk
    {
        public List<string> HunkNames;
        public int table_size;
        public int FirstHunkId;
        public int LastHunkId;
        public List<HunkInfo> HunkInfos;
    }

    public class HunkInfo
    {
        public int Size;
        public string Flags;
    }

    public class BssHunk : Hunk 
    {
    }

    public class LibraryHunk : Hunk
    {
        public uint lib_file_offset;
    }

    public class DebugHunk : Hunk
    {
        public uint debug_offset;
        public string debug_type;
        public string src_file;
        public Dictionary<int, uint> src_map;
    }

    public class UnitHunk : Hunk
    {
    }

    public class DefHunk : Hunk
    {
        public short value;
    }

    public class LibUnit
    {
        public List<List<Hunk>> segments;
        public object name;
        public int unit_no;
        public Unit index_unit;
    }

    public class ExtHunk : Hunk
    {
        public List<ExtObject> ext_def;
        public List<ExtObject> ext_ref;
        public List<ExtObject> ext_common;
    }

    public enum ExtType
    {
        EXT_SYMB = 0,
        EXT_DEF = 1,
        EXT_ABS = 2,

        EXT_RES = 3,

        EXT_ABSREF32 = 129,

        EXT_ABSCOMMON = 130,

        EXT_RELREF16 = 131,

        EXT_RELREF8 = 132,

        EXT_DEXT32 = 133,

        EXT_DEXT16 = 134,

        EXT_DEXT8 = 135,

        EXT_RELREF32 = 136,

        EXT_RELCOMMON = 137,

        EXT_ABSREF16 = 138,

        EXT_ABSREF8 = 139,
        EXT_RELREF26 = 229,

        EXT_TYPE_SHIFT = 24,
        EXT_TYPE_SIZE_MASK = 16777215,
    }

    public class IndexHunk : Hunk
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

    public class OverlayHunk : Hunk
    {
        public byte[] ov_data;
        public bool ov_std;
        public byte[] custom_data;
    }

    public class RelocHunk : Hunk
    {
        public Dictionary<int, List<uint>> reloc;
    }

    public class SymbolHunk : Hunk
    {
        public Dictionary<string, int> symbols;
    }

    public class TextHunk : Hunk
    {
        public uint data_file_offset;
    }

    public class IHunk
    {
        public string name;
        public short size;
        public int type;
        public string memf;
        public List<Reference> refs;
        public List<Definition> defs;
    }
}