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

using Decompiler.Arch.M68k;
using Decompiler.Environments.AmigaOS;
using Decompiler.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Decompiler.ImageLoaders.Hunk
{
    /// <summary>
    /// This class knows how to load and how to relocate AmigaOS Hunk files.
    /// </summary>
    /// <remarks>
    /// http://amiga-dev.wikidot.com/file-format:hunk
    /// </remarks>
    public partial class HunkLoader: ImageLoader
    {
        private List<Hunk> hunks;
        private HeaderHunk header;
        public List<Segment> segments { get; private set; }
        private OverlayHunk overlay;
        private List<HeaderHunk> overlay_headers;
        private List<Segment> overlay_segments;
        private List<LibHunk> libs;
        private List<Unit> units;

        private static TraceSwitch Trace = new TraceSwitch("HunkLoader", "Trace Amiga Hunk loader", "Verbose");

        public HunkLoader(IServiceProvider services, byte[] imgRaw)
            : base(services, imgRaw)
        {
            this.hunks = new List<Hunk>();
            this.FileType = 0;
            this.header = null;
            this.segments = new List<Segment>();
            this.overlay = null;
            this.overlay_headers = new List<HeaderHunk>();
            this.overlay_segments = new List<Segment>();
            this.libs = null;
            this.units = null;
        }

        public int FileType { get; private set; } 
        
        public override Address PreferredBaseAddress
        {
            get { throw new NotImplementedException(); }
        }

        public override LoaderResults Load(Address addrLoad)
        {
            var imgReader = new BeImageReader(RawImage, 0);
            var arch = new M68kArchitecture();
            var parse = new HunkFileParser(imgReader, false);
            this.hunks = parse.Parse();
            BuildSegments();
            var image = RelocateBytes(addrLoad);

            return new LoaderResults(
                new LoadedImage(addrLoad, image),
                new M68kArchitecture(),
                new AmigaOSPlatform(Services, arch));
        }

        private string get_struct_summary(object obj)
        {
            throw new NotImplementedException();
            /*
    if type(obj) == ListType:
      result = []
      for a in obj:
        v = this.get_struct_summary(a)
        if v != null:
          result.append(v)
      return "[" + ",".join(result) + "]"
    elif type(obj) == DictType:
      if obj.has_key('type_name'):
        type_name = obj['type_name']
        return type_name.replace('HUNK_','')
      else:
        result = []
        for k in obj.keys():
          v = this.get_struct_summary(obj[k])
          if v != null:
            result.append(k + ":" + v)
        return '{' + ",".join(result) + '}'
    else:
      return null*/
        }

        ///
        ///  Read a hunk file and build internal hunk structure
        ///Return status and set this.error_string on failure
        ///
        public void ParseHunkFile(BeImageReader f, bool? v37_compat)
        {
            var parser = new HunkFileParser(f, v37_compat);
            parser.Parse();
        }

        // Return a list with all the hunk type names that were found
        public string get_hunk_summary()
        {
            return this.get_struct_summary(this.hunks);
        }

        private HunkType[] loadseg_valid_begin_hunks = new[] {
            HunkType.HUNK_CODE,
            HunkType.HUNK_DATA,
            HunkType.HUNK_BSS,
            HunkType.HUNK_PPC_CODE
        };

        private HunkType[] loadseg_valid_extra_hunks = new[] {
            HunkType.HUNK_ABSRELOC32,
            HunkType.HUNK_DREL32,
            HunkType.HUNK_DEBUG,
            HunkType.HUNK_SYMBOL,
            HunkType.HUNK_NAME
        };

        public bool BuildLoadSegments()
        {
            bool inHeader = true;
            bool beginSeek = false;
            Segment segment = null;
            int hunk_no = 0;
            foreach (Hunk e in this.hunks)
            {
                var hunk_type = e.HunkType;

                if (inHeader)
                {
                    if (loadseg_valid_begin_hunks.Contains(hunk_type))
                    inHeader = false;
                    beginSeek = true;
                } 
                if (inHeader)
                {
                    if (hunk_type == HunkType.HUNK_HEADER)
                    {
                        Debug.WriteLineIf(Trace.TraceVerbose, "Loading Header into a segment.");
                        var hdr = e as HeaderHunk;
                        // we are in an overlay!
                        if (this.overlay != null)
                        {
                            segments = new List<Segment>();
                            this.overlay_segments.AddRange(segments);
                            this.overlay_headers.Add(hdr);
                        }
                        else
                        {
                            // set load_seg() header
                            this.header = hdr;
                        }
                        // start a new segment
                        segment = new Segment { };

                        // setup hunk counter
                        hunk_no = (int) hdr.FirstHunkId;
                    }
                    else if (hunk_type == HunkType.HUNK_DEBUG)
                    {
                        // we allow a debug hunk in header for SAS compatibility
                        segment.hunks.Add(e);
                    }
                    else
                    {
                        throw new BadImageFormatException(
                            string.Format(
                                "Expected header in loadseg: {0} {1}/{1:X}", e.HunkType, (int) e.HunkType));
                    }
                }
                else if (beginSeek)
                {
                    if (loadseg_valid_begin_hunks.Contains(hunk_type))
                    {
                        Debug.WriteLineIf(Trace.TraceVerbose, string.Format("Hunk {0} starting new segment because it is {1}.", e.Name, hunk_type));
                        segment = new Segment { hunks = { e } };
                        segments.Add(segment);
                        beginSeek = false;
                        e.hunk_no = hunk_no;
                        e.alloc_size = this.header.HunkSizes[hunk_no].Size;
                        ++hunk_no;
                        break;
                    }
                    else if (hunk_type == HunkType.HUNK_OVERLAY)
                    {
                        // add an extra overlay "hunk"
                        // assume hunk to be empty
                        if (this.overlay != null)
                            throw new BadImageFormatException(
                                string.Format(
                                  "Multiple overlay in loadseg: {0} {1}/{1:X}.", e.HunkType, (int) e.HunkType));
                        this.overlay = (OverlayHunk) e;
                        this.overlay_headers = new List<HeaderHunk>();
                        this.overlay_segments = new List<Segment>();
                        inHeader = true;
                        // break
                    }
                    else if (hunk_type == HunkType.HUNK_BREAK)
                    {
                        // Assume hunk to be empty
                        inHeader = true;
                    }
                    else if (hunk_type == HunkType.HUNK_END 
                      || hunk_type == HunkType.HUNK_NAME
                      || hunk_type == HunkType.HUNK_DEBUG)
                    {
                        // broken hunk: multiple END or other hunks
                        continue;
                    }
                    else
                    {
                        throw new BadImageFormatException(
                            string.Format(
                                "Expected hunk start in loadseg: {0} {1}/{1:X}.", e.HunkType, (int) e.HunkType));
                    }
                }
                else
                {
                    // an extra block in hunk or end is expected
                    if (hunk_type == HunkType.HUNK_END)
                    {
                        beginSeek = true;
                    }
                    else if (loadseg_valid_extra_hunks.Contains(hunk_type))
                    {
                        // contents of hunk
                        Debug.WriteLineIf(Trace.TraceVerbose, string.Format("...adding hunk {0} ({1})", e.hunk_no, e.Name));
                        segment.hunks.Add(e);
                    }
                    else if (loadseg_valid_begin_hunks.Contains(hunk_type))
                    {
                        // broken hunk file without END tag
                        segment = new Segment { hunks = { e } };
                        segments.Add(segment);
                        beginSeek = false;
                        e.hunk_no = hunk_no;
                        e.alloc_size = this.header.HunkSizes[hunk_no].Size;
                        ++hunk_no;
                        // unexpected hunk?!
                    }
                    else
                    {
                        throw new BadImageFormatException(
                            string.Format(
                                "Unexpected hunk extra in loadseg: {0} {1}/{1:X}.", e.HunkType, (int) e.HunkType));
                    }
                }
            }
            return true;
        }

        public bool BuildUnit()
        {
            var force_unit = true;
            var in_hunk = false;
            string name = null;
            Segment segment = null;
            Unit unit = null;
            this.units = new List<Unit>();
            var unit_no = 0;
            var hunk_no = 0;
            foreach (var e in this.hunks)
            {
                var hunk_type = e.HunkType;

                // optional unit as first entry
                if (hunk_type == HunkType.HUNK_UNIT)
                {
                    unit = new Unit();
                    unit.name = e.Name;
                    unit.unit_no = unit_no;
                    unit.segments = new List<Segment>();
                    unit.unit = e;
                    unit_no += 1;
                    this.units.Add(unit);
                    force_unit = false;
                    hunk_no = 0;
                }
                else if (force_unit)
                {
                    throw new BadImageFormatException(string.Format(
                        "Expected name hunk in unit: {0} {1}/{1:X}", e.HunkType, (int) hunk_type));
                }
                else if (!in_hunk)
                {
                    // begin a named hunk
                    if (hunk_type == HunkType.HUNK_NAME)
                    {
                        name = e.Name;
                    }
                    // main hunk block
                    else if (unit_valid_main_hunks.Contains(hunk_type))
                    {
                        segment = new Segment { hunks = { e } };
                        unit.segments.Add(segment);
                        // give main block the NAME
                        if (name != null)
                        {
                            e.Name = name;
                            name = null;
                        }
                        e.hunk_no = hunk_no;
                        ++hunk_no;
                        in_hunk = true;
                    }
                    // broken hunk: ignore multi ENDs
                    else if (hunk_type == HunkType.HUNK_END)
                    {
                        continue;
                    }
                    else
                        throw new BadImageFormatException(string.Format(
                            "Expected main hunk in unit: {0} {1}/{1:X}", e.HunkType, (int) hunk_type));
                }
                else
                {
                    // a hunk is finished
                    if (hunk_type == HunkType.HUNK_END)
                    {
                        in_hunk = false;
                    }
                    // contents of hunk
                    else if (unit_valid_extra_hunks.Contains(hunk_type))
                    {
                        segment.hunks.Add(e);
                    }
                    else
                        throw new BadImageFormatException(string.Format(
                          "Unexpected hunk in unit: {0} {1}/{1:X}", e.HunkType, (int) hunk_type));
                }
            }
            return true;
        }

        private bool BuildLib()
        {
            this.libs = new List<LibHunk>();
            var lib_segments = new List<Segment>();
            var segment_list = new List<Segment>();
            Segment segment = null;
            bool seekForLibHunk = true;
            bool seek_main = false;
            int hunk_no = 0;
            uint lib_file_offset = 0;
            foreach (var e in this.hunks)
            {
                var hunk_type = e.HunkType;

                if (seekForLibHunk)
                {
                    if (hunk_type == HunkType.HUNK_LIB)
                    {
                        var libHunk = (LibHunk) e;
                        segment_list = new List<Segment>();
                        lib_segments.AddRange(segment_list);
                        seek_main = true;
                        hunk_no = 0;

                        // get start address of lib hunk in file
                        lib_file_offset = libHunk.lib_file_offset;

                        seekForLibHunk = false;
                    }
                    else
                    {
                        throw new BadImageFormatException(string.Format(
                            "Expected lib hunk in lib: {0} {1}/{1:X}", hunk_type, (int) hunk_type));
                    }
                }
                else if (seek_main)
                {
                    if (hunk_type == HunkType.HUNK_INDEX)
                    {
                        // End of lib? -> index!
                        seek_main = false;
                        seekForLibHunk = true;
                        var lib_units = new List<Unit>();
                        if (!this.ResolveIndexHunks((IndexHunk)e, segment_list, lib_units))
                            throw new BadImageFormatException("Error resolving index hunks.");
                        var lib = new LibHunk();
                        lib.units = lib_units;
                        lib.lib_no = this.libs.Count;
                        lib.index = e;
                        this.libs.Add(lib);
                    }
                    else if (unit_valid_main_hunks.Contains(hunk_type))
                    {
                        // start of a hunk
                        segment = new Segment { hunks = { e } };
                        e.hunk_no = hunk_no;
                        ++hunk_no;
                        segment_list.Add(segment);
                        seek_main = false;

                        // calc relative lib address
                        var hunk_lib_offset = (int)( e.FileOffset - lib_file_offset);
                        e.hunk_lib_offset = hunk_lib_offset;
                    }
                    else
                    {
                        throw new BadImageFormatException(string.Format(
                            "Expected main hunk in lib: {0} {1}/{1:X}", e.HunkType, (int) e.HunkType));
                    }
                }
                else
                {
                    if (hunk_type == HunkType.HUNK_END)
                    {
                        seek_main = true;
                    }
                    else if (unit_valid_extra_hunks.Contains(hunk_type))
                    {
                        // extra contents
                        segment.hunks.Add(e);
                    }
                    else
                        throw new BadImageFormatException(string.Format(
                            "Unexpected hunk in lib: {0} {1}/{1:X}.", hunk_type, (int) hunk_type));
                }
            }
            return true;
        }

        /// <summary>
        /// Resolve hunks referenced in the index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="segment_list"></param>
        /// <param name="lib_units"></param>
        /// <returns></returns>
        private bool ResolveIndexHunks(IndexHunk index, List<Segment> segment_list, List<Unit> lib_units)
        {
            var units = index.units;
            int no = 0;
            foreach (var unit in units)
            {
                var lib_unit = new Unit();
                var unit_segments = new List<Segment>();
                lib_unit.segments = unit_segments;
                lib_unit.name = unit.name;
                lib_unit.unit_no = no;
                lib_unit.index_unit = unit;
                lib_units.Add(lib_unit);
                ++no;

                // Try to find segment with start offset
                uint hunk_offset = unit.hunk_begin_offset;
                bool found = false;
                foreach (var segment in segment_list)
                {
                    int hunk_no = segment.hunks[0].hunk_no;
                    int lib_off = segment.hunks[0].hunk_lib_offset / 4; // is in longwords
                    if (lib_off == hunk_offset)
                    {
                        // found segment
                        int num_segs = unit.hunk_infos.Count;
                        for (int i = 0; i < num_segs; ++i)
                        {
                            var info = unit.hunk_infos[i];
                            var seg = segment_list[hunk_no + i];
                            unit_segments.Add(seg);
                            // renumber hunk
                            seg.hunks[0].hunk_no = i;
                            seg.hunks[0].Name = info.Name;
                            seg.hunks[0].index_hunk = info;
                        }
                        found = true;
                    }
                }
                if (!found)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Build segments from the hunk list.
        /// </summary>
        /// <returns></returns>
        public bool BuildSegments()
        {
            this.segments = new List<Segment>();
            if (this.hunks.Count == 0)
            {
                this.FileType = TYPE_UNKNOWN;
                return false;
            }

            //  determine type of file from first hunk
            HunkType firstHunkType = this.hunks[0].HunkType;
            switch (firstHunkType)
            {
            case HunkType.HUNK_HEADER:
                this.FileType = TYPE_LOADSEG;
                return this.BuildLoadSegments();
            case HunkType.HUNK_UNIT:
                this.FileType = TYPE_UNIT;
                return this.BuildUnit();
            case HunkType.HUNK_LIB:
                this.FileType = TYPE_LIB;
                return this.BuildLib();
            default:
                this.FileType = TYPE_UNKNOWN;
                return false;
            }
        }

        // Return a summary of the created segment structure
        public string get_segment_summary()
        {
            return this.get_struct_summary(this.segments);
        }

        public string get_overlay_segment_summary()
        {
            if (this.overlay_segments != null)
                return this.get_struct_summary(this.overlay_segments);
            else
                return null;
        }

        public string get_libs_summary()
        {
            if (this.libs != null)
                return this.get_struct_summary(this.libs);
            else
                return null;
        }

        public string get_units_summary()
        {
            if (this.units != null)
                return this.get_struct_summary(this.units);
            else
                return null;
        }

		public override RelocationResults Relocate(Address addrLoad)
        {
            return new RelocationResults(new List<EntryPoint>(), new RelocationDictionary());
        }

        private byte[] RelocateBytes(Address addrLoad)
        {
            var rel = new HunkRelocator(this);
            // get sizes of all segments
            var sizes = rel.GetSegmentSizes();
            // calc begin addrs for all segments
            uint base_addr = addrLoad.Linear;
            var addrs = rel.GetSegmentRelocationAddresses(base_addr);
            //  relocate and return data of segments
            var datas = rel.Relocate(addrs);
            if (datas == null)
                throw new BadImageFormatException("Relocation failed.");
            return datas;
            //print "Relocate to base address",base_addr
            //print "Bases: "," ".join(map(lambda x:"%06x"%(x),addrs))
            //print "Sizes: "," ".join(map(lambda x:"%06x"%(x),sizes))
            //print "Data:  "," ".join(map(lambda x:"%06x"%(len(x)),datas))
            //print "Total: ","%06x"%(rel.get_total_size())
        }
    }
}