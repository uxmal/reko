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

using Decompiler.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.ImageLoaders.Hunk
{
    public class HunkFileParser
    {
        private const int HUNK_TYPE_MASK = 0xffff;
        private const uint HUNK_FLAGS_MASK = 0xffff0000;

        private const int HUNKF_ADVISORY = 1 << 29;
        private const int HUNKF_CHIP = 1 << 30;
        private const int HUNKF_FAST = 1 << 31;
        private const int HUNKF_ALL = (HUNKF_ADVISORY | HUNKF_CHIP | HUNKF_FAST);

        private const int EXT_TYPE_SHIFT = 24;
        private const int EXT_TYPE_SIZE_MASK = 0xffffff;

        private Dictionary<HunkType, string> knownHunkTypes = new Dictionary<HunkType, string>
        {
            { HunkType.HUNK_UNIT, "HUNK_UNIT" },
            { HunkType.HUNK_NAME, "HUNK_NAME" },
            { HunkType.HUNK_CODE, "HUNK_CODE" },
            { HunkType.HUNK_DATA, "HUNK_DATA" },
            { HunkType.HUNK_BSS, "HUNK_BSS" },
            { HunkType.HUNK_ABSRELOC32, "HUNK_ABSRELOC32" },
            { HunkType.HUNK_RELRELOC16, "HUNK_RELRELOC16" },
            { HunkType.HUNK_RELRELOC8, "HUNK_RELRELOC8" },
            { HunkType.HUNK_EXT, "HUNK_EXT" },
            { HunkType.HUNK_SYMBOL, "HUNK_SYMBOL" },
            { HunkType.HUNK_DEBUG, "HUNK_DEBUG" },
            { HunkType.HUNK_END, "HUNK_END" },
            { HunkType.HUNK_HEADER, "HUNK_HEADER" },
            { HunkType.HUNK_OVERLAY, "HUNK_OVERLAY" },
            { HunkType.HUNK_BREAK, "HUNK_BREAK" },
            { HunkType.HUNK_DREL32, "HUNK_DREL32" },
            { HunkType.HUNK_DREL16, "HUNK_DREL16" },
            { HunkType.HUNK_DREL8, "HUNK_DREL8" },
            { HunkType.HUNK_LIB, "HUNK_LIB" },
            { HunkType.HUNK_INDEX, "HUNK_INDEX" },
            { HunkType.HUNK_RELOC32SHORT, "HUNK_RELOC32SHORT" },
            { HunkType.HUNK_RELRELOC32, "HUNK_RELRELOC32" },
            { HunkType.HUNK_ABSRELOC16, "HUNK_ABSRELOC16" },
            { HunkType.HUNK_PPC_CODE, "HUNK_PPC_CODE" },
            { HunkType.HUNK_RELRELOC26, "HUNK_RELRELOC26" },
        };


        private HunkType[] reloc_hunks = new[] {
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

        private const int EXT_SYMB = 0;
        private const int EXT_DEF = 1;
        private const int EXT_ABS = 2;
        private const int EXT_RES = 3;
        private const int EXT_ABSREF32 = 129;
        private const int EXT_ABSCOMMON = 130;
        private const int EXT_RELREF16 = 131;
        private const int EXT_RELREF8 = 132;
        private const int EXT_DEXT32 = 133;
        private const int EXT_DEXT16 = 134;
        private const int EXT_DEXT8 = 135;
        private const int EXT_RELREF32 = 136;
        private const int EXT_RELCOMMON = 137;
        private const int EXT_ABSREF16 = 138;
        private const int EXT_ABSREF8 = 139;
        private const int EXT_RELREF26 = 229;

        private Dictionary<int, string> ext_names = new Dictionary<int, string> {
            { EXT_SYMB       , "EXT_SYMB"},
            { EXT_DEF        , "EXT_DEF"},
            { EXT_ABS        , "EXT_ABS"},
            { EXT_RES        , "EXT_RES"},
            { EXT_ABSREF32   , "EXT_ABSREF32"},
            { EXT_ABSCOMMON  , "EXT_ABSCOMMON"},
            { EXT_RELREF16   , "EXT_RELREF16"},
            { EXT_RELREF8    , "EXT_RELREF8"},
            { EXT_DEXT32     , "EXT_DEXT32"},
            { EXT_DEXT16     , "EXT_DEXT16"},
            { EXT_DEXT8      , "EXT_DEXT8"},
            { EXT_RELREF32   , "EXT_RELREF32"},
            { EXT_RELCOMMON  , "EXT_RELCOMMON"},
            { EXT_ABSREF16   , "EXT_ABSREF16"},
            { EXT_ABSREF8    , "EXT_ABSREF8"},
            { EXT_RELREF26   , "EXT_RELREF26"}
        };

        private BeImageReader f;
        private bool? v37_compat;
        private List<Hunk> hunks;
        private Encoding textEncoding;

        public HunkFileParser(Core.BeImageReader f, bool? v37_compat= null)
        {
            this.f = f;
            this.v37_compat = v37_compat;
            this.hunks = new List<Hunk>();
            // Commodore famously used ISO 8859-1 for the Amiga. Different
            // national variants may need to override this.
            this.textEncoding = Encoding.GetEncoding("ISO_8859-1");
        }

        public List<Hunk> Parse()
        {
            bool isFirstHunk = true;
            bool sawEndHunk = false;
            bool potential_v37_hunk = false;
            bool sawOverlay = false;
            uint libSize = 0;
            uint prevOffset = 0;

            while (true)
            {
                uint hunkFileOffset = f.Offset;

                // read hunk type
                int bytesLeft = (int) (f.Bytes.Length - f.Offset);
                if (bytesLeft < 4)
                {
                    // tolerate a few extra bytes at end
                    if (isFirstHunk)
                        throw new BadImageFormatException("Not a valid hunk file. The file is empty.");
                    break;
                }
                int rawHunkType = f.ReadBeInt32();
                if (rawHunkType < 0)
                {
                    if (isFirstHunk)
                        throw new BadImageFormatException("Not a valid hunk file. The file is too short.");
                    else
                        throw new BadImageFormatException(string.Format("Error reading hunk type at offset {0:X8}.", hunkFileOffset));
                }

                var hunkType = (HunkType) (rawHunkType & (int) HUNK_TYPE_MASK);
                int hunkFlags = rawHunkType & unchecked((int) HUNK_FLAGS_MASK);

                // check range of hunk type
                if (!knownHunkTypes.ContainsKey(hunkType))
                {
                    // no hunk file?
                    if (isFirstHunk)
                        throw new BadImageFormatException(
                            string.Format("Not a valid hunk file. Unknown hunk type {0} (${0:X8})", (int) hunkType));
                    else if (sawEndHunk)
                    {
                        // garbage after an end tag is ignored
                        return hunks;
                    }
                    else if (potential_v37_hunk)
                    {
                        // auto fix v37 -> reread whole file
                        f.Offset = 0;
                        v37_compat = true;
                        return Parse();
                    }
                    else if (sawOverlay)
                    {
                        // seems to be a custom overlay -> read to end of file
                        byte[] ovCustomData = ReadToEnd(f);
                        hunks.Last().custom_data = ovCustomData;
                    }
                    else
                    {
                        throw new BadImageFormatException(string.Format("Invalid hunk type {0}/{0:X8} found at @{0:X8}.", (int) hunkType, (int) hunkType, prevOffset));
                    }
                    return hunks;
                }

                // Validate first hunk.
                if (isFirstHunk && !IsValidFirstHunkType(hunkType))
                    throw new BadImageFormatException(
                            string.Format("First hunk of type {0} is not allowed.", hunkType));

                isFirstHunk = false;
                sawEndHunk = false;
                potential_v37_hunk = false;
                sawOverlay = false;

                Action<Hunk> hunk = h =>
                {
                    h.HunkType = hunkType;
                    h.FileOffset = hunkFileOffset;
                    hunks.Add(h);

                    //hunk.HunkType = hunk_names[hunk_type]
                    SetMemoryFlags(h, hunkFlags, 30);
                    // account for lib
                    uint lastHunkSize = hunkFileOffset - prevOffset;
                    if (libSize > 0)
                        libSize -= lastHunkSize;
                    if (libSize > 0)
                        h.inLib = true;

                    // V37 fix?
                    if (hunkType == HunkType.HUNK_DREL32)
                    {
                        // Try automatic fix
                        if (!v37_compat.HasValue || !v37_compat.Value)
                            potential_v37_hunk = true;
                        // fix was forced
                        else if (v37_compat.Value)
                        {
                            hunkType = HunkType.HUNK_RELOC32SHORT;
                            h.fixes = "v37";
                        }
                    }
                };

                switch (hunkType)
                {
                case HunkType.HUNK_HEADER:
                    this.ParseHeader(f, hunk);
                    break;
                case HunkType.HUNK_CODE:
                case HunkType.HUNK_DATA:
                case HunkType.HUNK_PPC_CODE:
                    this.ParseText(f, hunk);
                    break;
                case HunkType.HUNK_BSS:
                    this.ParseBss(f, hunk);
                    break;
                case HunkType.HUNK_RELRELOC32:
                case HunkType.HUNK_ABSRELOC16:
                case HunkType.HUNK_RELRELOC8:
                case HunkType.HUNK_RELRELOC16:
                case HunkType.HUNK_ABSRELOC32:
                case HunkType.HUNK_DREL32:
                case HunkType.HUNK_DREL16:
                case HunkType.HUNK_DREL8:
                case HunkType.HUNK_RELRELOC26:
                    try
                    {
                        this.ParseReloc(f, hunk);
                    }
                    catch
                    {
                        // auto fix v37 bug?
                        if (hunkType == HunkType.HUNK_DREL32 && v37_compat.HasValue && v37_compat.Value == false)
                        {
                            f.Offset = 0;
                            v37_compat = true;
                            return Parse();
                        }
                        throw;
                    }
                    break;
                case HunkType.HUNK_RELOC32SHORT:
                    this.ParseRelocShort(f, hunk);
                    break;
                case HunkType.HUNK_SYMBOL:
                    this.ParseSymbol(f, hunk);
                    break;
                case HunkType.HUNK_DEBUG:
                    this.ParseDebug(f, hunk);
                    break;
                case HunkType.HUNK_END:
                    sawEndHunk = true;
                    break;
                case HunkType.HUNK_OVERLAY:
                    this.ParseOverlay(f, hunk);
                    sawOverlay = true;
                    break;
                case HunkType.HUNK_BREAK:
                    break;
                case HunkType.HUNK_LIB:
                    libSize = (uint) this.ParseLib(f, hunk);
                    libSize += 8; // add size of HUNK_LIB itself
                    break;
                case HunkType.HUNK_INDEX:
                    this.ParseIndex(f, hunk);
                    break;
                case HunkType.HUNK_EXT:
                    this.ParseExt(f, hunk);
                    break;
                case HunkType.HUNK_UNIT:
                case HunkType.HUNK_NAME:
                    this.ParseUnitOrName(f, hunk);
                    break;
                default:
                    throw new BadImageFormatException(string.Format("Unsupported hunk {0}", hunkType));
                }
                prevOffset = hunkFileOffset;
            }
            return hunks;
        }

        private bool IsValidFirstHunkType(HunkType hunk_type)
        {
            return hunk_type == HunkType.HUNK_HEADER
                    || hunk_type == HunkType.HUNK_LIB
                    || hunk_type == HunkType.HUNK_UNIT;
        }

        private byte[] ReadToEnd(BeImageReader f)
        {
            var data = new byte[f.Bytes.Length - f.Offset];
            Array.Copy(f.Bytes, f.Offset, data, 0, data.Length);
            return data;
        }

        public HeaderHunk ParseHeader(BeImageReader f)
        {
            return ParseHeader(f, q => { });
        }

        private HeaderHunk ParseHeader(BeImageReader f, Action<Hunk> fn)
        {
            var hunk = new HeaderHunk();
            fn(hunk);
            var names = new List<string>();
            hunk.HunkNames = names;
            while (true)
            {
                string s = this.ReadString(f);
                if (s.Length == 0)
                    break;
                names.Add(s);
            }

            // table size and hunk range
            uint table_size = f.ReadBeUInt32();
            uint first_hunk = f.ReadBeUInt32();
            uint last_hunk = f.ReadBeUInt32();

            hunk.table_size = table_size;
            hunk.FirstHunkId = first_hunk;
            hunk.LastHunkId = last_hunk;

            // determine number of hunks in size table
            int cHunks = (int) (last_hunk - first_hunk + 1);
            List<HunkInfo> hunkTable = new List<HunkInfo>();
            for (int a = 0; a < cHunks; ++a)
            {
                var hunk_info = new HunkInfo();
                int hunkSize = f.ReadBeInt32();
                if (hunkSize < 0)
                    throw new BadImageFormatException("HUNK_HEADER contains invalid hunk_size.");
                uint hunk_bytes = (uint) (hunkSize & ~HUNKF_ALL);
                hunk_bytes *= 4; // longs to bytes
                hunk_info.size = hunk_bytes;
                this.SetMemoryFlags(hunk_info, hunkSize & HUNKF_ALL, 30);
                hunkTable.Add(hunk_info);
            }
            hunk.HunkSizes = hunkTable;
            return hunk;
        }

        public TextHunk ParseText(BeImageReader f)
        {
            return ParseText(f, q => { });
        }

        private TextHunk ParseText(BeImageReader f, Action<Hunk> fn)
        {
            var hunk = new TextHunk();
            fn(hunk);
            int cLongs = f.ReadBeInt32();
            if (cLongs < 0)
            {
                throw new BadImageFormatException(
                    string.Format("{0} has invalid size.", hunk.HunkType));
            }

            // read in hunk data
            uint size = (uint) cLongs * 4;

            hunk.size = size & ~HUNKF_ALL;
            uint flags = (uint) (size & HUNKF_ALL);
            this.SetMemoryFlags(hunk, (int) flags, 30);
            hunk.dataFileOffset = f.Offset;
            var data = f.ReadBytes(hunk.size);
            hunk.Data = data;
            return hunk;
        }

        private void ParseBss(BeImageReader f, Action<Hunk> fn)
        {
            var hunk = new BssHunk();
            fn(hunk);
            int num_longs = f.ReadBeInt32();
            if (num_longs < 0)
            {
                throw new BadImageFormatException(string.Format(
                    "{0} has invalid size.", hunk.HunkType));
            }
            uint size = (uint) num_longs * 4;
            hunk.size = size & ~HUNKF_ALL;
            uint flags = (uint) (size & HUNKF_ALL);
            this.SetMemoryFlags(hunk, (int) flags, 30);
        }

        private void ParseReloc(BeImageReader f, Action<Hunk> fn)
        {
            var hunk = new RelocHunk();
            fn(hunk);
            var reloc = new Dictionary<int, List<uint>>();
            hunk.reloc = reloc;
            for (; ; )
            {
                int cRelocations = f.ReadBeInt32();
                if (cRelocations < 0)
                {
                    throw new BadImageFormatException(
                        string.Format(
                          "{0} has invalid number of relocations.", hunk.HunkType));
                }
                else if (cRelocations == 0)
                {
                    // last relocation found
                    break;
                }
                cRelocations &= 0xFFFF;
                // build reloc map
                int hunkNo = f.ReadBeInt32();
                if (hunkNo < 0)
                {
                    throw new BadImageFormatException(string.Format(
                        "{0} has invalid hunk nummber.", hunk.HunkType));
                }
                var offsets = new List<uint>();
                for (int a = 0; a < cRelocations; ++a)
                {
                    int offset = f.ReadBeInt32();
                    if (offset < 0)
                    {
                        throw new NotImplementedException(
                            string.Format(
                            "{0} has invalid relocation #{1} offset {2} (num_relocs={3} hunk_num={4}, offset={5})",
                            hunk.HunkType, a, offset, cRelocations, hunkNo, f.Offset));
                    }
                    offsets.Add((uint) offset);
                }
                reloc[hunkNo] = offsets;
            }
        }

        private void ParseRelocShort(BeImageReader f, Action<Hunk> fn)
        {
            var hunk = new RelocHunk();
            fn(hunk);
            int num_relocs = 1;
            var reloc = new Dictionary<int, List<uint>>();
            hunk.reloc = reloc;
            int total_words = 0;
            while (num_relocs != 0)
            {
                num_relocs = f.ReadBeInt16();
                if (num_relocs < 0)
                {
                    throw new BadImageFormatException(string.Format(
                        "{0} has invalid number of relocations", hunk.HunkType));
                }
                else if (num_relocs == 0)
                {
                    // last relocation found
                    ++total_words;
                    break;
                }

                // build reloc map
                short hunk_num = f.ReadBeInt16();
                if (hunk_num < 0)
                {
                    throw new BadImageFormatException(string.Format(
                      "{0} has invalid hunk num", hunk.HunkType));
                }
                var offsets = new List<uint>();
                int count = num_relocs & 0xffff;
                total_words += count + 2;
                for (int a = 0; a < count; ++a)
                {
                    int offset = f.ReadBeInt16();
                    if (offset < 0)
                    {
                        throw new FormatException(
                            string.Format(
                                "{0} has invalid relocation #{1} offset {2} (num_relocs={3} hunk_num={4}, offset={5:X})",
                         hunk.HunkType, a, offset, num_relocs, hunk_num, f.Offset));
                    }
                    offsets.Add((uint) offset);
                }
                reloc[hunk_num] = offsets;
            }
            // padding
            if ((total_words & 1) == 1)
            {
                f.ReadBeUInt16();
            }
        }

        private void ParseSymbol(BeImageReader f, Action<Hunk> fn)
        {
            var hunk = new SymbolHunk();
            fn(hunk);
            int name_len = 1;
            var symbols = new Dictionary<string, int>();
            hunk.symbols = symbols;
            while (name_len > 0)
            {
                string name = this.ReadString(f);
                if (name_len < 0)
                {
                    throw new BadImageFormatException(string.Format(
                        "{0} has invalid symbol name.", hunk.HunkType));
                }
                else if (name_len == 0)
                    // last name occurred
                    break;
                int value = f.ReadBeInt32();
                if (value < 0)
                {
                    throw new BadImageFormatException(string.Format(
                        "{0} has invalid symbol value.", hunk.HunkType));
                }
                symbols.Add(name, value);
            }
        }

        private void ParseDebug(BeImageReader f, Action<Hunk> fn)
        {
            var hunk = new DebugHunk();
            fn(hunk);
            int num_longs = f.ReadBeInt32();
            if (num_longs < 0)
                throw new BadImageFormatException(string.Format(
                    "{0} has invalid size.", hunk.HunkType));

            int size = num_longs * 4;
            uint offset = f.ReadBeUInt32();
            hunk.debug_offset = offset;
            string tag = this.ReadTag(f);
            hunk.debug_type = tag;
            size -= 8;

            if (tag == "LINE")
            {
                // parse LINE: source line -> code offset mapping
                int l = f.ReadBeInt32();
                size -= l * 4 + 4;
                string n = this.ReadSizedString(f, l);
                var src_map = new Dictionary<int, uint>();
                hunk.src_file = n;
                hunk.src_map = src_map;
                while (size > 0)
                {
                    int line_no = f.ReadBeInt32();
                    offset = f.ReadBeUInt32();
                    size -= 8;
                    src_map.Add(line_no, (uint) offset);
                }
            }
            else
            {
                // read unknown DEBUG hunk
                hunk.data = f.ReadBytes((uint) size);
            }
        }

        private void ParseOverlay(BeImageReader f, Action<Hunk> fn)
        {
            var hunk = new OverlayHunk();
            fn(hunk);

            // read size of overlay hunk
            int overlaySize = f.ReadBeInt32();
            if (overlaySize < 0)
            {
                throw new BadImageFormatException(string.Format(
                    "{0} has invalid size.", hunk.HunkType));
            }
            // read data of overlay
            int byteSize = (overlaySize + 1) * 4;
            byte[] ov_data = f.ReadBytes((uint) byteSize);
            hunk.ov_data = ov_data;

            // check: first get header hunk
            var hdr_hunk = this.hunks[0];
            if (hdr_hunk.HunkType != HunkType.HUNK_HEADER)
            {
                throw new BadImageFormatException(string.Format(
                    "{0} has no header hunk.", hunk.HunkType));
            }
            // first find the code segment of the overlay manager
            TextHunk overlayManagerHunk = this.FindFirstCodeHunk();
            if (overlayManagerHunk == null)
            {
                throw new BadImageFormatException(string.Format(
                    "{0} has no overlay manager hunk", hunk.HunkType));
            }
            // check overlay manager
            var overlay_mgr_data = overlayManagerHunk.Data;
            uint magic = LoadedImage.ReadBeUInt32(overlay_mgr_data, 4);
            if (magic != 0xABCD)
                throw new BadImageFormatException("No valid overlay manager found.");

            // check for standard overlay manager
            uint magic2 = LoadedImage.ReadBeUInt32(overlay_mgr_data, 24);
            uint magic3 = LoadedImage.ReadBeUInt32(overlay_mgr_data, 28);
            uint magic4 = LoadedImage.ReadBeUInt32(overlay_mgr_data, 32);
            bool std_overlay = (magic2 == 0x5ba0) && (magic3 == 0x074f7665) && (magic4 == 0x726c6179);
            hunk.ov_std = std_overlay;
        }

        private uint ParseLib(BeImageReader f, Action<Hunk> fn)
        {
            var hunk = new LibHunk();
            fn(hunk);
            var libSize = f.ReadBeInt32();
            hunk.lib_file_offset = f.Offset;
            return (uint)libSize * 4;
        }

        private void ParseIndex(BeImageReader f, Action<Hunk> fn)
        {
            IndexHunk hunk = new IndexHunk();
            fn(hunk);

            int index_size = f.ReadBeInt32();
            int total_size = index_size * 4;

            // first read string table
            uint strtab_size = f.ReadBeUInt16();
            byte[] strtab = f.ReadBytes(strtab_size);
            total_size -= (int) strtab_size + 2;

            // read units
            var units = new List<Unit>();
            hunk.units = units;
            int unit_no = 0;
            while (total_size > 2)
            {
                // read name of unit
                int name_offset = f.ReadBeUInt16();
                total_size -= 2;
                if (name_offset == 0)
                    break;

                var unit = new Unit();
                units.Add(unit);
                unit.unit_no = unit_no;
                ++unit_no;

                // generate unit name
                unit.name = this.ReadIndexName(strtab, name_offset);

                // hunks in unit
                uint hunk_begin = f.ReadBeUInt16();
                int num_hunks = f.ReadBeInt16();
                total_size -= 4;
                unit.hunk_begin_offset = hunk_begin;

                // for all hunks in unit
                var ihunks = new List<Hunk>();
                unit.hunk_infos = ihunks;
                for (int a = 0; a < num_hunks; ++a)
                {
                    var ihunk = new Hunk();
                    ihunks.Add(ihunk);

                    // get hunk info
                    name_offset = f.ReadBeInt16();
                    int hunk_size = f.ReadBeInt16(); ;
                    int hunk_type = f.ReadBeInt16();
                    total_size -= 6;
                    ihunk.name = this.ReadIndexName(strtab, name_offset);
                    ihunk.size = (uint) hunk_size;
                    ihunk.HunkType = (HunkType) (hunk_type & 0x3fff);
                    this.SetMemoryFlags(ihunk, hunk_type & 0xc000, 14);
                    ihunk.HunkType = (HunkType) (hunk_type & 0x3FFF);

                    // get references
                    int num_refs = f.ReadBeInt16();
                    total_size -= 2;
                    if (num_refs > 0)
                    {
                        var refs = new List<Reference>();
                        ihunk.refs = refs;
                        for (int b = 0; b < num_refs; ++b)
                        {
                            var @ref = new Reference();
                            name_offset = f.ReadBeInt16();
                            total_size -= 2;
                            string name = this.ReadIndexName(strtab, name_offset);
                            if (name == "")
                            {
                                // 16 bit refs point to the previous zero byte before the string entry...
                                name = this.ReadIndexName(strtab, name_offset + 1);
                                @ref.bits = 16;
                            }
                            else
                            {
                                @ref.bits = 32;
                            }
                            @ref.name = name;
                            refs.Add(@ref);
                        }
                    }

                    // get definitions
                    int num_defs = f.ReadBeInt16();
                    total_size -= 2;
                    if (num_defs > 0)
                    {
                        var defs = new List<Hunk>();
                        ihunk.defs = defs;
                        for (int b = 0; b < num_defs; ++b)
                        {
                            name_offset = f.ReadBeInt16();
                            short def_value = f.ReadBeInt16();
                            short def_type_flags = f.ReadBeInt16();
                            var def_type = (HunkType) (def_type_flags & 0x3fff);
                            var def_flags = def_type_flags & 0xc000;
                            total_size -= 6;
                            string name = this.ReadIndexName(strtab, name_offset);
                            var d = new DefHunk { name = name, value = def_value, HunkType = def_type };
                            this.SetMemoryFlags(d, def_flags, 14);
                            defs.Add(d);
                        }
                    }
                }
            }
            // align hunk
            if (total_size == 2)
            {
                f.ReadBeUInt16();
            }
            else if (total_size != 0)
            {
                throw new BadImageFormatException(string.Format(
                    "{0} has invalid padding.", hunk.HunkType));
            }
        }

        private void ParseExt(BeImageReader f, Action<Hunk> fn)
        {
            var hunk = new ExtHunk();
            fn(hunk);
            var ext_def = new List<ExtObject>();
            var ext_ref = new List<ExtObject>();
            var ext_common = new List<ExtObject>();
            hunk.ext_def = ext_def;
            hunk.ext_ref = ext_ref;
            hunk.ext_common = ext_common;
            int ext_type_size = 1;
            while (ext_type_size > 0)
            {
                // ext type | size
                ext_type_size = f.ReadBeInt32();
                if (ext_type_size < 0)
                {
                    throw new BadImageFormatException(
                        string.Format(
                            "{0} has invalid size.", hunk.HunkType));
                }
                int ext_type = ext_type_size >> EXT_TYPE_SHIFT;
                int ext_size = ext_type_size & EXT_TYPE_SIZE_MASK;

                // ext name
                string ext_name = this.ReadSizedString(f, ext_size);
                if (ext_name.Length < 0)
                {
                    throw new BadImageFormatException(string.Format(
                        "{0} has invalid name.", hunk.HunkType));
                }
                else if (ext_name.Length == 0)
                    break;

                // create local ext object
                var ext = new ExtObject { type = ext_type, name = ext_name };

                // check and setup type name
                if (!ext_names.ContainsKey(ext_type))
                {
                    throw new BadImageFormatException(string.Format(
                        "{0} has unspported ext entr {1}.", hunk.HunkType, ext_type));
                }
                ext.type_name = ext_names[ext_type];

                // ext common
                if (ext_type == EXT_ABSCOMMON || ext_type == EXT_RELCOMMON)
                {
                    ext.common_size = f.ReadBeInt32();
                    ext_common.Add(ext);
                }
                // ext def
                else if (ext_type == EXT_DEF || ext_type == EXT_ABS || ext_type == EXT_RES)
                {
                    ext.def = f.ReadBeInt32();
                    ext_def.Add(ext);
                }
                // ext ref
                else
                {
                    int num_refs = f.ReadBeInt32();
                    if (num_refs == 0)
                    {
                        num_refs = 1;
                    }
                    var refs = new List<uint>();
                    for (int a = 0; a < num_refs; ++a)
                    {
                        var @ref = f.ReadBeUInt32();
                        refs.Add(@ref);
                    }
                    ext.refs = refs;
                    ext_ref.Add(ext);
                }
            }
        }

        private void ParseUnitOrName(BeImageReader f, Action<Hunk> fn)
        {
            Hunk hunk = new Hunk();
            fn(hunk);
            string n = this.ReadString(f);
            if (n.Length > 0)
                hunk.name = n;
            else
                hunk.name = "";
        }

        private TextHunk FindFirstCodeHunk()
        {
            foreach (var hunk in this.hunks)
            {
                if (hunk.HunkType == HunkType.HUNK_CODE)
                    return (TextHunk) hunk;
            }
            return null;
        }

        string ReadIndexName(byte[] strtab, int iStart)
        {
            int iEnd = iStart;
            // Find next 0 byte.
            while (iEnd < strtab.Length)
            {
                if (strtab[iEnd] == 0)
                    break;
                ++iEnd;
            }
            return textEncoding.GetString(strtab, iStart, iEnd - iStart);
        }

        string ReadTag(BeImageReader f)
        {
            if (f.Offset + 4 <= f.Bytes.Length)
            {
                var s = textEncoding.GetString(f.Bytes, (int) f.Offset, 4);
                f.Offset += 4;
                return s;
            }
            throw new BadImageFormatException();
        }

        public string ReadSizedString(BeImageReader f, int cLongs)
        {
            uint size = (uint) (cLongs & 0xffffff) * 4;
            byte[] data = f.ReadBytes(size);
            if (data.Length < size)
            {
                return null;
            }
            int endpos;
            for (endpos = data.Length; endpos > 0; --endpos)
            {
                if (data[endpos - 1] != 0)
                    break;
            }
            return textEncoding.GetString(data, 0, endpos);
        }

        /// <summary>
        /// Read a string prefixed by its space occupancy in 4-byte long words:
        ///  +-+-+-+-+------....
        ///  | size  |string
        ///  +-+-+-+-+------....
        /// </summary>
        /// <param name="rdr"></param>
        /// <returns></returns>
        public string ReadString(BeImageReader rdr)
        {
            int cWords = rdr.ReadBeInt32();
            if (cWords <= 0)
                return "";

            var str = textEncoding.GetString(rdr.Bytes, (int) rdr.Offset, cWords * 4);
            rdr.Offset += (uint) cWords * 4;
            return str.TrimEnd('\0');
        }
        private void SetMemoryFlags(Hunk hunk, int flags, int shift)
        {
            int f = flags >> shift;
            if ((f & 1) == 1)
                hunk.memf = "chip";
            else if ((f & 2) == 2)
                hunk.memf = "fast";
            else
                hunk.memf = "";
        }
    }
}
