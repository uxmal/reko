#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Memory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.Hunk
{
    /// <summary>
    /// Parses an AmigaOS HUNK file.
    /// </summary>
    public class HunkFileParser
    {
        public HunkFile hunk_file;
        public List<Hunk> hunks;

        private readonly ByteImageReader f;
        private bool? v37_compat;
        private Encoding textEncoding;

        private static readonly TraceSwitch trace = new TraceSwitch("HunkLoader", "Traces the progress of the Amiga Hunk loader");

        public HunkFileParser(ByteImageReader f, bool? v37_compat = null)
        {
            this.f = f;
            this.v37_compat = v37_compat;
            // Commodore famously used ISO 8859-1 for the Amiga. Different
            // national variants may need to override this.
            this.textEncoding = Encoding.GetEncoding("ISO_8859-1");
            this.hunk_file = new HunkFile();
            this.hunks = hunk_file.hunks;
        }

        // Resolve hunks referenced in the index";
        // From the hunk list build a set of segments that form the actual binary.
        // Return a summary of the created segment structure.
        public virtual HunkFile Parse()
        {
            this.hunks = new List<Hunk>();
            bool isFirstHunk = true;
            bool sawEndHunk = false;
            bool was_potential_v37_hunk = false;
            bool sawOverlay = false;
            var lib_size = 0;
            uint last_file_offset = 0;
            while (true)
            {
                var hunkFileOffset = f.Offset;
                // read hunk type
                var rawHunkType = this.read_long();
                if (rawHunkType == -1 || rawHunkType == -2)
                {
                    // tolerate extra byte at end
                    if (isFirstHunk)
                        throw new BadImageFormatException("Invalid hunk file. The file is empty.");
                    break;
                }
                var hunkType = (HunkType) (rawHunkType & Hunk.HUNK_TYPE_MASK);
                var hunkFlags = rawHunkType & Hunk.HUNK_FLAGS_MASK;

                // Validate that hunk type is known.
                if (!knownHunkTypes.ContainsKey(hunkType))
                {
                    // no hunk file?
                    if (isFirstHunk)
                        throw new BadImageFormatException(String.Format("Unknown hunk type: {0}.", hunkType));

                    if (sawEndHunk)
                    {
                        // garbage after an end tag is ignored
                        return this.hunk_file;
                    }
                    if (was_potential_v37_hunk)
                    {
                        // auto fix v37 -> reread whole file
                        f.Offset = 0;
                        v37_compat = true;
                        return this.Parse();
                    }
                    if (sawOverlay)
                    {
                        // seems to be a custom overlay -> read to end of file
                        var ov_custom_data = f.ReadToEnd();
                        ((OverlayHunk) this.hunk_file.hunks[hunks.Count - 1]).custom_data = ov_custom_data;
                        return this.hunk_file;
                    }
                    else
                        throw new BadImageFormatException(
                            string.Format("Invalid hunk type {0}/{0:X} found at @{1:X8}.", hunkType, (int) hunkType, f.Offset));
                }
                // check for valid first hunk type
                if (isFirstHunk && !this.IsValidFirstHunkType(hunkType))
                    throw new BadImageFormatException($"No hunk file. The first hunk type was {hunkType}.");
                isFirstHunk = false;
                sawEndHunk = false;
                was_potential_v37_hunk = false;
                sawOverlay = false;

                Action<Hunk> hunk = h =>
                {
                    h.HunkType = hunkType;
                    h.FileOffset = (uint)hunkFileOffset;
                    hunk_file.hunks.Add(h);
                    h.memf = this.SetMemoryFlags(hunkFlags, 30);
                    
                    // Account for lib
                    var last_hunk_size = (int) (hunkFileOffset - last_file_offset);
                    if (lib_size > 0)
                    {
                        lib_size -= last_hunk_size;
                    }
                    if (lib_size > 0)
                    {
                        h.in_lib = true;
                    }
                    // V37 fix?
                    if (hunkType == HunkType.HUNK_DREL32)
                    {
                        // try to fix automatically...
                        if (v37_compat is null)
                        {
                            was_potential_v37_hunk = true;
                            // fix was forced
                        }
                        else if (v37_compat.HasValue && v37_compat.Value)
                        {
                            hunkType = HunkType.HUNK_RELOC32SHORT;
                            h.fixes = "v37";
                        }
                    }
                };
                Debug.WriteLineIf(trace.TraceVerbose, string.Format("Loading hunk type: {0}", hunkType));
                switch (hunkType)
                {
                case HunkType.HUNK_HEADER:
                    this.ParseHeader(hunk);
                    break;
                case HunkType.HUNK_CODE:
                case HunkType.HUNK_DATA:
                case HunkType.HUNK_PPC_CODE:
                    this.ParseText(hunk);
                    break;
                case HunkType.HUNK_BSS:
                    this.ParseBss(hunk);
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
                        this.ParseReloc(hunk);
                    }
                    catch (BadImageFormatException)
                    {
                        // auto fix v37 bug?
                        if (hunkType != HunkType.HUNK_DREL32 || v37_compat is not null)
                            throw;
                        f.Offset = 0;
                        v37_compat = true;
                        return Parse();
                    }
                    break;
                case HunkType.HUNK_RELOC32SHORT:
                    this.ParseRelocShort(hunk);
                    break;
                case HunkType.HUNK_SYMBOL:
                    this.ParseSymbol(hunk);
                    break;
                case HunkType.HUNK_DEBUG:
                    this.ParseDebug(hunk);
                    break;
                case HunkType.HUNK_END:
                    hunk(new Hunk());
                    sawEndHunk = true;
                    break;
                case HunkType.HUNK_OVERLAY:
                    this.ParseOverlay(hunk);
                    sawOverlay = true;
                    break;
                case HunkType.HUNK_BREAK:
                    break;
                case HunkType.HUNK_LIB:
                    lib_size = this.ParseLib(hunk);
                    lib_size += 8;
                    break;
                case HunkType.HUNK_INDEX:
                    this.ParseIndex(hunk);
                    break;
                case HunkType.HUNK_EXT:
                    this.ParseExt(hunk);
                    break;
                case HunkType.HUNK_UNIT:
                case HunkType.HUNK_NAME:
                    this.parse_unit_or_name(hunk);
                    break;
                default:
                    throw new BadImageFormatException(string.Format("Unsupported hunk {0}.", (int) hunkType));
                }
                last_file_offset = (uint)hunkFileOffset;
            }
            return hunk_file;
        }

        private bool IsValidFirstHunkType(HunkType hunkType)
        {
            return hunkType == HunkType.HUNK_HEADER || hunkType == HunkType.HUNK_LIB || hunkType == HunkType.HUNK_UNIT;
        }

        public virtual int read_long()
        {
            if (!f.IsValid)
                return -1;
            if (!f.IsValidOffset(1))
                return -2;
            if (!f.IsValidOffset(2))
                return -3;
            if (!f.IsValidOffset(3))
                return -4;
            return f.ReadBeInt32();
        }

        public virtual short read_word(ByteImageReader f)
        {
            if (!f.IsValid)
                return -1;
            if (!f.IsValidOffset(1))
                return -2;
            return f.ReadBeInt16();
        }

        /// <summary>
        /// Read a length-prefixed string.
        /// </summary>
        /// <returns>null if the string runs off the end of the image.</returns>
        public virtual string? ReadString()
        {
            int num_longs;
            num_longs = this.read_long();
            if (num_longs < 0)
            {
                return null;
            }
            else if (num_longs == 0)
            {
                return "";
            }
            else
            {
                return this.ReadSizedString(num_longs);
            }
        }

        public string? ReadSizedString(int num_longs)
        {
            int size = (num_longs & 0xFFFFFF) * 4;
            var data = f.ReadBytes(size);
            if (data.Length < size)
            {
                return null;
            }
            int endpos = Array.IndexOf(data, (byte) 0);
            if (endpos < 0)
            {
                endpos = data.Length;
            }
            if (endpos == 0)
            {
                return "";
            }
            else
            {
                return this.textEncoding.GetString(data, 0, endpos);
            }
        }

        public virtual string get_index_name(byte[] strtab, int offset)
        {
            int end = Array.IndexOf(strtab, (byte) 0, offset);
            if (end == -1)
            {
                return Encoding.GetEncoding("ISO_8859-1").GetString(strtab, offset, strtab.Length - offset);
            }
            else
            {
                return Encoding.GetEncoding("ISO_8859-1").GetString(strtab, offset, end - offset);
            }
        }

        public HeaderHunk ParseHeader(Action<Hunk> h)
        {
            var hunk = new HeaderHunk();
            h(hunk);
            var names = new List<string>();
            hunk.HunkNames = names;
            while (true)
            {
                var t = this.ReadString();
                if (t is null)
                    throw new BadImageFormatException("Error parsing header hunk names.");
                else if (t.Length == 0)
                {
                    break;
                }
                names.Add(t);
            }

            // Table size and hunk range
            var table_size = f.ReadBeInt32();
            var first_hunk = f.ReadBeInt32();
            var last_hunk = f.ReadBeInt32();
            if (table_size < 0 || first_hunk < 0 || last_hunk < 0)
                throw new BadImageFormatException("Invalid header hunk.");
            
            hunk.table_size = table_size;
            hunk.FirstHunkId = first_hunk;
            hunk.LastHunkId = last_hunk;

            // Determine number of hunks in size table
            int num_hunks = last_hunk - first_hunk + 1;
            var hunkInfos = new List<HunkInfo>();
            for (int a = 0; a < num_hunks; ++a)
            {
                var hunk_info = new HunkInfo();
                int hunk_size = this.read_long();
                hunk_size &= 0x3FFFFFFF;           // Top 2 bits not handled yet.
                if (hunk_size < 0)
                    throw new BadImageFormatException("Head hunk contains invalid hunk_size.");
                int hunk_bytes = (hunk_size & ~Hunk.HUNKF_ALL) * 4;
                hunk_info.Size = hunk_bytes;
                hunk_info.Flags = this.SetMemoryFlags(hunk_size & Hunk.HUNKF_ALL, 30);
                hunkInfos.Add(hunk_info);
            }
            hunk.HunkInfos = hunkInfos;
            return hunk;
        }

        public TextHunk ParseText(Action<Hunk> fn)
        {
            var hunk = new TextHunk();
            fn(hunk);
            var num_longs = this.read_long();
            if (num_longs < 0)
                throw new BadImageFormatException(string.Format("{0} has invalid size.", hunk.HunkType));

            // Read in the hunk data
            var size = num_longs * 4;
            hunk.alloc_size = size & ~Hunk.HUNKF_ALL;
            var flags = size & Hunk.HUNKF_ALL;
            hunk.memf = this.SetMemoryFlags(flags, 30);
            hunk.data_file_offset = (uint)f.Offset;
            hunk.Data = f.ReadBytes(hunk.alloc_size);
            Debug.WriteLineIf(trace.TraceVerbose, string.Format("  alloc_size:  {0:X8}", hunk.alloc_size));
            Debug.WriteLineIf(trace.TraceVerbose, string.Format("  file_offset: {0:X8}", hunk.data_file_offset));
            return hunk;
        }

        public BssHunk ParseBss(Action<Hunk> fn)
        {
            var hunk = new BssHunk();
            fn(hunk);
            var num_longs = this.read_long();
            if (num_longs < 0)
                throw new BadImageFormatException(string.Format("{0} has invalid size.", hunk.HunkType));
            // read in hunk data
            var size = num_longs * 4;
            hunk.size = size & ~Hunk.HUNKF_ALL;
            var flags = size & Hunk.HUNKF_ALL;
            hunk.memf = this.SetMemoryFlags(flags, 30);
            return hunk;
        }

        public RelocHunk ParseReloc(Action<Hunk> fn)
        {
            var hunk = new RelocHunk();
            fn(hunk);
            var num_relocs = 1;
            var reloc = new Dictionary<int, List<uint>>();
            hunk.reloc = reloc;
            while (num_relocs != 0)
            {
                num_relocs = this.read_long();
                if (num_relocs < 0)
                    throw new BadImageFormatException(string.Format("{0} has invalid number of relocations.", hunk.HunkType));
                else if (num_relocs == 0)
                {
                    // last relocation found
                    break;
                }
                // build reloc map
                var hunkNo = this.read_long();
                if (hunkNo < 0)
                    throw new BadImageFormatException(string.Format("{0} has invalid hunk num.", hunk.HunkType));
                Debug.WriteLineIf(trace.TraceVerbose, string.Format("  hunk: {0}, relocs: {1}", hunkNo, num_relocs));
                var offsets = new List<uint>();
                num_relocs &= 0xFFFF;
                for (var a = 0; a < num_relocs; ++a)
                {
                    var offset = this.read_long();
                    if (offset < 0)
                        throw new BadImageFormatException(
                            string.Format(
                                "{0} has invalid relocation #{1} Offset {2} (num_relocs={3} hunkNo={4}, Offset={5})", 
                                hunk.HunkType, 
                                a, offset,
                                num_relocs,
                                hunkNo, 
                                f.Offset));
                    offsets.Add((uint) offset);
                }
                reloc[hunkNo] = offsets;
            }
            return hunk;
        }

        public void ParseRelocShort(Action<Hunk> h)
        {
            var hunk = new RelocHunk();
            h(hunk);

            var num_relocs = 1;
            var reloc = new Dictionary<int, List<uint>>();
            hunk.reloc = reloc;
            var total_words = 0;
            while (num_relocs != 0)
            {
                num_relocs = this.read_word(f);
                if (num_relocs < 0)
                    throw new BadImageFormatException(string.Format("{0} has invalid number of relocations.", hunk.HunkType));
                else if (num_relocs == 0)
                {
                    // last relocation found
                    ++total_words;
                    break;
                }
                // build reloc map
                var hunkNo = this.read_word(f);
                if (hunkNo < 0)
                    throw new BadImageFormatException(string.Format("{0} has invalid hunk num.", hunk.HunkType));
                var offsets = new List<uint>();
                var count = num_relocs & 0xFFFF;
                total_words += count + 2;
                for (var a = 0; a < count; ++a)
                {
                    var offset = f.ReadBeInt16();
                    if (offset < 0)
                        throw new BadImageFormatException(string.Format(
                            "{0} has invalid relocation #{1} Offset {2} (num_relocs={3} hunkNo={4}, Offset={5}).", hunk.HunkType, a, offset, num_relocs, hunkNo, f.Offset));
                    offsets.Add((uint) offset);
                }
                reloc[hunkNo] = offsets;
                // padding
            }
            if ((total_words & 1) == 1)
            {
                f.ReadBeInt16();
            }
        }

        public void ParseSymbol(Action<Hunk> h)
        {
            var hunk = new SymbolHunk();
            h(hunk);
            var symbols = new Dictionary<string, int>();
            int name_len = 1;
            hunk.symbols = symbols;
            while (name_len > 0)
            {
                var x = this.ReadString();
                if (x is null)
                    throw new BadImageFormatException(string.Format("{0} has invalid symbol name", hunk.HunkType));
                else if (x.Length == 0)
                {
                    // last name occurred
                    break;
                }
                var value = this.read_long();
                if (value < 0)
                    throw new NotImplementedException(string.Format("{0} has invalid symbol value", hunk.HunkType));
                symbols[x] = value;
            }
        }

        public void ParseDebug(Action<Hunk> h)
        {
            var hunk = new DebugHunk();
            var num_longs = this.read_long();
            if (num_longs < 0)
                throw new BadImageFormatException(string.Format("{0} has invalid size.", hunk.HunkType));
            var size = num_longs * 4;
            if (num_longs < 2)
            {
                f.Offset += size;
                return;
            }
            var offset = (uint) this.read_long();
            hunk.debug_offset = offset;
            var tag = this.ReadTag();
            hunk.debug_type = tag;
            size -= 8;  //  skip offset and tag.
            if (tag == "LINE")
            {
                // LINE
                // parse LINE: source line -> code offset mapping
                var l = f.ReadBeInt32();
                size -= l * 4 + 4;
                var n = this.ReadSizedString(l);
                var src_map = new Dictionary<int, uint>();
                hunk.src_file = n;
                hunk.src_map = src_map;
                while (size > 0)
                {
                    var line_no = this.read_long();
                    offset = (uint) this.read_long();
                    size -= 8;
                    src_map.Add(line_no, offset);
                }
            }
            else
            {
                // read unknown DEBUG hunk
                hunk.Data = f.ReadBytes(size);
            }
        }

        public virtual TextHunk? FindFirstCodeHunk()
        {
            foreach (var hunk in this.hunks)
            {
                if (hunk.HunkType == HunkType.HUNK_CODE)
                {
                    return (TextHunk) hunk;
                }
            }
            return null;
        }

        public virtual void ParseOverlay(Action<Hunk> h)
        {
            OverlayHunk hunk = new OverlayHunk();
            h(hunk);
            // read size of overlay hunk
            var ov_size = this.read_long();
            if (ov_size < 0)
                throw new BadImageFormatException(string.Format("{0} has invalid size.", hunk.HunkType));
            
            // read data of overlay
            var byte_size = (ov_size + 1) * 4;
            var ov_data = f.ReadBytes(byte_size);
            hunk.ov_data = ov_data;
            // check: first get header hunk
            var hdr_hunk = this.hunks[0];
            if (hdr_hunk.HunkType != HunkType.HUNK_HEADER)
                throw new BadImageFormatException(string.Format("{0} has no header hunk.", hunk.HunkType));
            
            // first find the code segment of the overlay manager
            var overlayManagerHunk = this.FindFirstCodeHunk();
            if (overlayManagerHunk is null)
                throw new BadImageFormatException(string.Format("{0} has no overlay manager hunk.", hunk.HunkType));
            
            // check overlay manager
            var overlay_mgr_data = overlayManagerHunk.Data!;
            uint magic = ByteMemoryArea.ReadBeUInt32(overlay_mgr_data, 4);
            if (magic != 0xABCD)
                throw new BadImageFormatException("No valid overlay manager found.");

            // check for standard overlay manager
            var magic2 = ByteMemoryArea.ReadBeUInt32(overlay_mgr_data, 24);
            var magic3 = ByteMemoryArea.ReadBeUInt32(overlay_mgr_data, 20);
            var magic4 = ByteMemoryArea.ReadBeUInt32(overlay_mgr_data, 32);
            var std_overlay = magic2 == 23456 && magic3 == 122648165 && magic4 == 1919705465;
            hunk.ov_std = std_overlay;
        }

        public virtual int ParseLib(Action<Hunk> h)
        {
            var hunk = new LibraryHunk();
            h(hunk);
            var lib_size = this.read_long();
            hunk.lib_file_offset = (uint)f.Offset;
            return lib_size * 4;
        }

        public virtual void ParseIndex(Action<Hunk> h)
        {
            var hunk = new IndexHunk();
            h(hunk);
            var index_size = this.read_long();
            var total_size = index_size * 4;

            // First read string table
            var strtab_size = f.ReadBeInt16();
            var strtab = f.ReadBytes(strtab_size);
            total_size -= strtab_size + 2;

            // Read units
            var units = new List<Unit>();
            hunk.units = units;
            var unit_no = 0;
            while (total_size > 2)
            {
                // Read name of unit
                var name_offset = f.ReadBeInt16();
                total_size -= 2;
                if (name_offset == 0)
                    break;
                
                var unit = new Unit { unit_no = unit_no };
                units.Add(unit);
                ++unit_no;

                // Generate unit name
                unit.name = this.get_index_name(strtab, name_offset);

                // hunks in unit
                var hunk_begin = f.ReadBeInt16();
                var num_hunks = f.ReadBeInt16();
                total_size -= 4;
                unit.hunk_begin_offset = hunk_begin;

                // For all hunks in unit
                var ihunks = new List<IHunk>();
                unit.hunk_infos = ihunks;
                for (var a = 0; a < num_hunks; ++a)
                {
                    var ihunk = new IHunk();
                    ihunks.Add(ihunk);
                    // get hunk info
                    name_offset = f.ReadBeInt16();
                    var hunk_size = f.ReadBeInt16();
                    var hunk_type = f.ReadBeInt16();
                    total_size -= 6;
                    ihunk.name = this.get_index_name(strtab, name_offset);
                    ihunk.size = hunk_size;
                    ihunk.type = hunk_type & 16383;
                    ihunk.memf = this.SetMemoryFlags(hunk_type & 49152, 14);
                    // get references
                    var num_refs = f.ReadBeInt16();
                    total_size -= 2;
                    if (num_refs > 0)
                    {
                        var refs = new List<Reference>();
                        ihunk.refs = refs;
                        for (var b = 0; b < num_refs; ++b)
                        {
                            var @ref = new Reference();
                            name_offset = f.ReadBeInt16();
                            total_size -= 2;
                            var name = this.get_index_name(strtab, name_offset);
                            if (name == "")
                            {
                                // 16 bit refs point to the previous zero byte before the string entry...
                                name = this.get_index_name(strtab, name_offset + 1);
                                @ref.bits = 16;
                            }
                            else
                            {
                                @ref.bits = 32;
                            }
                            @ref.name = name;
                            refs.Add(@ref);
                            // get definitions
                        }
                    }
                    var num_defs = f.ReadBeInt16();
                    total_size -= 2;
                    if (num_defs > 0)
                    {
                        var defs = new List<Definition>();
                        ihunk.defs = defs;
                        for (var b = 0; b < num_defs; ++b)
                        {
                            name_offset = f.ReadBeInt16();
                            var def_value = f.ReadBeInt16();
                            var def_type_flags = f.ReadBeInt16();
                            var def_type = def_type_flags & 16383;
                            var def_flags = def_type_flags & 49152;
                            total_size -= 6;
                            var name = this.get_index_name(strtab, name_offset);
                            var d = new Definition
                            {
                                name = name,
                                value = def_value,
                                type = def_type
                            };
                            d.memf = this.SetMemoryFlags(def_flags, 14);
                            defs.Add(d);
                        }
                    }
                }
            }
            // Align hunk to long-word.
            if (total_size == 2)
            {
                f.ReadBeInt16();
            }
            else if (total_size != 0)
                throw new BadImageFormatException(string.Format("{0} has invalid padding.", hunk.HunkType));
        }

        public void ParseExt(Action<Hunk> h)
        {
            var hunk = new ExtHunk();
            h(hunk);
            var ext_def = new List<ExtObject>();
            var ext_ref = new List<ExtObject>();
            var ext_common = new List<ExtObject>();
            hunk.ext_def = ext_def;
            hunk.ext_ref = ext_ref;
            hunk.ext_common = ext_common;
            var ext_size = 1;
            while (ext_size > 0)
            {
                // ext type | size
                var ext_type_size = this.read_long();
                var ext_type = (ExtType) (byte) (ext_type_size >> (int) ExtType.EXT_TYPE_SHIFT);
                ext_size = ext_type_size & (int) ExtType.EXT_TYPE_SIZE_MASK;
                if (ext_size < 0)
                    throw new BadImageFormatException(string.Format("{0} has invalid size.", hunk.HunkType));
                // ext name
                string? ext_name = this.ReadSizedString(ext_size);
                if (ext_name is null)
                    throw new BadImageFormatException(string.Format("{0} has invalid name.", hunk.HunkType));
                else if (ext_name.Length == 0)
                    break;
                
                // create local ext object
                var ext = new ExtObject { type = ext_type, name = ext_name };

                // check and setup type name
                if (!ext_names.ContainsKey(ext_type))
                    throw new BadImageFormatException(string.Format("{0} has unspported ext entry {1}.", hunk.HunkType, ext_type));
                // ext common
                if (ext_type == ExtType.EXT_ABSCOMMON || ext_type == ExtType.EXT_RELCOMMON)
                {
                    ext.common_size = (uint) this.read_long();
                    ext_common.Add(ext);
                    // ext def
                }
                else if (ext_type == ExtType.EXT_DEF || ext_type == ExtType.EXT_ABS || ext_type == ExtType.EXT_RES)
                {
                    ext.def = f.ReadBeUInt32();
                    ext_def.Add(ext);
                }
                else
                {
                    // ext ref
                    var num_refs = this.read_long();
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

        public void parse_unit_or_name(Action<Hunk> h)
        {
            var hunk = new UnitHunk();
            h(hunk);
            var n = this.ReadString();
            if (n is null)
                throw new BadImageFormatException(string.Format("{0} has invalid name.", hunk.HunkType));
            hunk.Name = n;
        }

        public virtual string SetMemoryFlags(int flags, int shift)
        {
            var f = flags >> shift;
            if ((f & 1) == 1)
            {
                return "chip";
            }
            else if ((f & 2) == 2)
            {
                return "fast";
            }
            else
            {
                return "";
            }
        }

        private string ReadTag()
        {
            if (!f.IsValidOffset(3))
                throw new BadImageFormatException();
            var s = textEncoding.GetString(f.Bytes, (int) f.Offset, 4);
            f.Offset += 4;
            return s;
        }

        public static List<HunkType> reloc_hunks = new List<HunkType> {
            HunkType.HUNK_ABSRELOC32,
            HunkType.HUNK_RELRELOC16,
            HunkType.HUNK_RELRELOC8,
            HunkType.HUNK_DREL32,
            HunkType.HUNK_DREL16,
            HunkType.HUNK_DREL8,
            HunkType.HUNK_RELOC32SHORT,
            HunkType.HUNK_RELRELOC32,
            HunkType.HUNK_ABSRELOC16,
            HunkType.HUNK_RELRELOC26
        };

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

        private Dictionary<ExtType, string> ext_names = new Dictionary<ExtType, string> {
            { ExtType.EXT_SYMB       , "EXT_SYMB"},
            { ExtType.EXT_DEF        , "EXT_DEF"},
            { ExtType.EXT_ABS        , "EXT_ABS"},
            { ExtType.EXT_RES        , "EXT_RES"},
            { ExtType.EXT_ABSREF32   , "EXT_ABSREF32"},
            { ExtType.EXT_ABSCOMMON  , "EXT_ABSCOMMON"},
            { ExtType.EXT_RELREF16   , "EXT_RELREF16"},
            { ExtType.EXT_RELREF8    , "EXT_RELREF8"},
            { ExtType.EXT_DEXT32     , "EXT_DEXT32"},
            { ExtType.EXT_DEXT16     , "EXT_DEXT16"},
            { ExtType.EXT_DEXT8      , "EXT_DEXT8"},
            { ExtType.EXT_RELREF32   , "EXT_RELREF32"},
            { ExtType.EXT_RELCOMMON  , "EXT_RELCOMMON"},
            { ExtType.EXT_ABSREF16   , "EXT_ABSREF16"},
            { ExtType.EXT_ABSREF8    , "EXT_ABSREF8"},
            { ExtType.EXT_RELREF26   , "EXT_RELREF26"}
        };
    }

    public class Lib
    {
        public List<LibUnit>? units;
        public int lib_no;
        public IndexHunk? index;
    }
}
