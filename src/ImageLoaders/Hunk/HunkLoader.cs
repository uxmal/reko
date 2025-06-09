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

using Reko.Arch.M68k;
using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.ImageLoaders.Hunk
{
    /// <summary>
    /// This class knows how to load and relocate AmigaOS Hunk files.
    /// </summary>
    /// <remarks>
    /// http://amiga-dev.wikidot.com/file-format:hunk
    /// </remarks>
    public partial class HunkLoader : ProgramImageLoader
    {
        private M68kArchitecture arch;
        private TextHunk? firstCodeHunk;
        private HunkFile hunkFile;

        public HunkLoader(IServiceProvider services, ImageLocation imageLocation, byte[] imgRaw)
            : base(services, imageLocation, imgRaw)
        {
            this.arch = null!;
            this.hunkFile = null!;
        }

        public HunkFile HunkFile { get { return hunkFile; } }

        //$REVIEW: is this a sane value? AmigaOS apparently didn't load at a specific
        // address. Emulators seem to like this value.
        public override Address PreferredBaseAddress
        {
            get { return Address.Ptr32(0x1000); }
            set { throw new NotImplementedException(); }
        }

        public override Program LoadProgram(Address? loadAddress)
        {
            var addrLoad = loadAddress ?? PreferredBaseAddress;
            var cfgSvc = Services.RequireService<IConfigurationService>();
            arch = (M68kArchitecture) cfgSvc.GetArchitecture("m68k")!;
            var imgReader = new BeImageReader(RawImage, 0);
            var parse = new HunkFileParser(imgReader, false);
            this.hunkFile = parse.Parse();
            BuildSegments();
            this.firstCodeHunk = parse.FindFirstCodeHunk();
            var platform = cfgSvc.GetEnvironment("amigaOS").Load(Services, arch);
            var imageMap = platform.CreateAbsoluteMemoryMap();
            var mem = new ByteMemoryArea(addrLoad, RelocateBytes(addrLoad));
            var segmentMap = new SegmentMap(
                mem.BaseAddress,
                new ImageSegment(
                    "code", mem, AccessMode.ReadWriteExecute));
            var program = new Program(
                new ByteProgramMemory(segmentMap),
                arch,
                platform);

            var sym = ImageSymbol.Procedure(program.Architecture, addrLoad, state: arch.CreateProcessorState());
            program.ImageSymbols[sym.Address] = sym;
            program.EntryPoints[sym.Address] = sym;
            return program;
        }

        public bool BuildLoadSegments()
        {
            bool inHeader = true;
            bool beginSeek = false;
            List<Hunk>? segment = null;
            var segments = this.hunkFile.segments;
            int hunk_no = 0;
            foreach (Hunk e in this.hunkFile.hunks)
            {
                var hunk_type = e.HunkType;
                // check for end of header
                if (inHeader && loadseg_valid_begin_hunks.Contains(hunk_type))
                {
                    inHeader = false;
                    beginSeek = true;
                }
                if (inHeader)
                {
                    if (hunk_type == HunkType.HUNK_HEADER)
                    {
                        // we are in an overlay!
                        if (this.hunkFile.overlay is not null)
                        {
                            segments = new List<List<Hunk>>();
                            this.hunkFile.overlay_segments!.Add(segments);
                            this.hunkFile.overlay_headers!.Add((HeaderHunk) e);
                        }
                        else
                        {
                            // set load_seg() header
                            this.hunkFile.header = (HeaderHunk) e;
                        }
                        // start a new segment
                        segment = new List<Hunk>();
                        // setup hunk counter
                        hunk_no = ((HeaderHunk) e).FirstHunkId;
                        // we allow a debug hunk in header for SAS compatibility
                    }
                    else if (hunk_type == HunkType.HUNK_DEBUG)
                    {
                        segment!.Add(e);
                    }
                    else
                        throw new BadImageFormatException(string.Format("Expected header in loadseg: {0}. {1}/{1:X}", e.HunkType, (int) hunk_type));
                }
                else if (beginSeek)
                {
                    // a new hunk shall begin
                    if (loadseg_valid_begin_hunks.Contains(hunk_type))
                    {
                        segment = new List<Hunk> { e };
                        segments.Add(segment);
                        beginSeek = false;
                        e.hunk_no = hunk_no;
                        e.alloc_size = this.hunkFile.header!.HunkInfos![hunk_no].Size;
                        hunk_no += 1;
                    }
                    else if (hunk_type == HunkType.HUNK_OVERLAY)
                    {
                        // add an extra overlay "hunk"
                        // assume hunk to be empty
                        if (this.hunkFile.overlay is not null)
                            throw new BadImageFormatException(String.Format("Multiple overlay in loadseg: {0} {1}/{1:X}.", e.HunkType, (int) hunk_type));
                        this.hunkFile.overlay = e;
                        this.hunkFile.overlay_headers = new List<HeaderHunk>();
                        this.hunkFile.overlay_segments = new List<List<List<Hunk>>>();
                        inHeader = true;
                    }
                    else if (hunk_type == HunkType.HUNK_BREAK)
                    {
                        // assume hunk to be empty
                        inHeader = true;
                        // broken hunk: multiple END or other hunks
                    }
                    else if (new List<object> {
                        HunkType.HUNK_END,
                        HunkType.HUNK_NAME,
                        HunkType.HUNK_DEBUG
                    }.Contains(hunk_type))
                    {
                    }
                    else
                        throw new BadImageFormatException(string.Format("Expected hunk start in loadseg: {0} {1}/{1:X}", e.HunkType, (int) hunk_type));
                }
                else
                {
                    if (hunk_type == HunkType.HUNK_END)
                    {
                        // an extra block in hunk or end is expected
                        beginSeek = true;
                    }
                    else if (loadseg_valid_extra_hunks.Contains(hunk_type))
                    {
                        // contents of hunk
                        segment!.Add(e);
                    }
                    else if (loadseg_valid_begin_hunks.Contains(hunk_type))
                    {
                        // broken hunk file without END tag
                        segment = new List<Hunk> { e };
                        segments.Add(segment);
                        beginSeek = false;
                        e.hunk_no = hunk_no;
                        e.alloc_size = this.hunkFile.header!.HunkInfos![hunk_no].Size;
                        ++hunk_no;
                    }
                    else
                    {
                        // unexpected hunk?!
                        throw new BadImageFormatException(string.Format("Unexpected hunk extra in loadseg: {0} {1}/{1:X}.", e.HunkType, (int) hunk_type));
                    }
                }
            }
            return true;
        }

        public static List<HunkType> loadseg_valid_begin_hunks = new List<HunkType> {
            HunkType.HUNK_CODE,
            HunkType.HUNK_DATA,
            HunkType.HUNK_BSS,
            HunkType.HUNK_PPC_CODE
        };

        public static List<HunkType> loadseg_valid_extra_hunks = new List<HunkType> {
            HunkType.HUNK_ABSRELOC32,
            HunkType.HUNK_DREL32,
            HunkType.HUNK_DEBUG,
            HunkType.HUNK_SYMBOL,
            HunkType.HUNK_NAME
        };

        //$TODO: move this to HunkFile
        public bool BuildUnit()
        {
            var force_unit = true;
            var in_hunk = false;
            string? name = null;
            List<Hunk>? segment = null;
            Unit? unit = null;
            this.hunkFile.units = new List<Unit>();
            var unit_no = 0;
            int hunk_no = 0;
            foreach (var e in this.hunkFile.hunks)
            {
                var hunk_type = e.HunkType;
                // optional unit as first entry
                if (hunk_type == HunkType.HUNK_UNIT)
                {
                    unit = new Unit
                    {
                        name = e.Name,
                        unit_no = unit_no,
                        segments = new List<List<Hunk>>(),
                        unit = e,
                    };
                    ++unit_no;
                    this.hunkFile.units.Add(unit);
                    force_unit = false;
                    hunk_no = 0;
                }
                else if (force_unit)
                    throw new BadImageFormatException(string.Format("Expected name hunk in unit: {0} {1}/{1:X}.", e.HunkType, (int) hunk_type));
                else if (!in_hunk)
                {
                    // begin a named hunk
                    if (hunk_type == HunkType.HUNK_NAME)
                    {
                        name = e.Name;
                        // main hunk block
                    }
                    else if (unit_valid_main_hunks.Contains(hunk_type))
                    {
                        segment = new List<Hunk> {
                            e
                        };
                        unit!.segments!.Add(segment);
                        // give main block the NAME
                        if (name is not null)
                        {
                            e.Name = name;
                            name = null;
                        }
                        e.hunk_no = hunk_no;
                        hunk_no += 1;
                        in_hunk = true;
                        // broken hunk: ignore multi ENDs
                    }
                    else if (hunk_type == HunkType.HUNK_END)
                    {
                    }
                    else
                        throw new BadImageFormatException(string.Format("Expected main hunk in unit: {0} {1}/{1:X}.", e.HunkType, hunk_type));
                }
                else
                {
                    // a hunk is finished
                    if (hunk_type == HunkType.HUNK_END)
                    {
                        in_hunk = false;
                    }
                    else if (HunkLoader.unit_valid_extra_hunks.Contains(hunk_type))
                    {
                        // contents of hunk
                        segment!.Add(e);
                    }
                    // unexpected hunk?!
                    else
                        throw new BadImageFormatException(string.Format("Unexpected hunk in unit: {0} {1}/{1:X}", e.HunkType, hunk_type));
                }
            }
            return true;
        }

        public static HunkType[] unit_valid_main_hunks = new [] 
        {
            HunkType.HUNK_CODE,
            HunkType.HUNK_DATA,
            HunkType.HUNK_BSS,
            HunkType.HUNK_PPC_CODE
        };

        public static HunkType[] unit_valid_extra_hunks = new[]
        {
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
            HunkType.HUNK_RELRELOC26
        };

        public virtual bool build_lib()
        {
            this.hunkFile.libs = new List<Lib>();
            var lib_segments = new List<object>();
            var seek_lib = true;
            var seek_main = false;
            int hunk_no = -1;
            uint lib_file_offset = ~0u;
            List<List<Hunk>>? segment_list = null;
            List<Hunk>? segment = null;
            foreach (var e in this.hunkFile.hunks)
            {
                var hunk_type = e.HunkType;
                // seeking for a LIB hunk
                if (seek_lib)
                {
                    if (hunk_type == HunkType.HUNK_LIB)
                    {
                        segment_list = new List<List<Hunk>>();
                        lib_segments.Add(segment_list);
                        seek_lib = false;
                        seek_main = true;
                        hunk_no = 0;
                        // get start address of lib hunk in file
                        lib_file_offset = ((LibraryHunk) e).lib_file_offset;
                    }
                    else
                        throw new BadImageFormatException(string.Format("Expected lib hunk in lib: {0} {1}/{1:X}", e.HunkType, hunk_type));
                }
                else if (seek_main)
                {
                    // end of lib? -> index!
                    if (hunk_type == HunkType.HUNK_INDEX)
                    {
                        seek_main = false;
                        seek_lib = true;
                        var lib_units = new List<LibUnit>();
                        if (!this.resolve_index_hunks((IndexHunk) e, segment_list!, lib_units))
                            throw new BadImageFormatException("Error resolving index hunks.");
                        var lib = new Lib
                        {
                            units = lib_units,
                            lib_no = this.hunkFile.libs.Count,
                            index = (IndexHunk) e
                        };
                        this.hunkFile.libs.Add(lib);
                    }
                    else if (unit_valid_main_hunks.Contains(hunk_type))
                    {
                        // start of a hunk
                        segment = new List<Hunk> { e };
                        e.hunk_no = hunk_no;
                        ++hunk_no;
                        segment_list!.Add(segment);
                        seek_main = false;
                        // calculate relative lib address
                        var hunk_lib_offset = e.FileOffset - lib_file_offset;
                        e.hunk_lib_offset = hunk_lib_offset;
                    }
                    else
                        throw new BadImageFormatException(string.Format("Expected main hunk in lib: {0} {1}/{1:X}.", e.HunkType, (int) hunk_type));
                }
                else
                {
                    // end hunk
                    if (hunk_type == HunkType.HUNK_END)
                    {
                        seek_main = true;
                        // extra contents
                    }
                    else if (unit_valid_extra_hunks.Contains(hunk_type))
                    {
                        segment!.Add(e);
                    }
                    else
                        throw new BadImageFormatException(string.Format("Unexpected hunk in lib: {0} {1}/{1:X}.", e.HunkType, (int) hunk_type));
                }
            }
            return true;
        }

        public virtual bool resolve_index_hunks(IndexHunk index, List<List<Hunk>> segment_list, List<LibUnit> lib_units)
        {
            var units = index.units;
            var no = 0;
            bool found = false;
            foreach (var unit in units!)
            {
                var unit_segments = new List<List<Hunk>>();
                var lib_unit = new LibUnit
                {
                    segments = unit_segments,
                    name = unit.name,
                    unit_no = no,
                    index_unit = unit,
                };
                lib_units.Add(lib_unit);
                no += 1;
                // try to find segment with start offset
                var hunk_offset = unit.hunk_begin_offset;
                found = false;
                foreach (var segment in segment_list)
                {
                    int hunk_no = segment[0].hunk_no;
                    uint lib_off = segment[0].hunk_lib_offset / 4;
                    if (lib_off == hunk_offset)
                    {
                        // found segment
                        int num_segs = unit.hunk_infos!.Count;
                        for (var i = 0; i < num_segs; ++i)
                        {
                            var info = unit.hunk_infos[i];
                            var seg = segment_list[hunk_no + i];
                            unit_segments.Add(seg);
                            // renumber hunk
                            seg[0].hunk_no = i;
                            seg[0].Name = info.name;
                            seg[0].index_hunk = info;
                        }
                        found = true;
                    }
                }
                if (!found)
                {
                    return false;
                }
            }
            return true;
        }

        public bool BuildSegments()
        {
            if (this.hunkFile.hunks.Count == 0)
            {
                this.hunkFile.type = FileType.TYPE_UNKNOWN;
                return false;
            }
            // determine type of file from first hunk
            switch (this.hunkFile.hunks[0].HunkType)
            {
            case HunkType.HUNK_HEADER:
                this.hunkFile.type = FileType.TYPE_LOADSEG;
                return this.BuildLoadSegments();
            case HunkType.HUNK_UNIT:
                this.hunkFile.type = FileType.TYPE_UNIT;
                return this.BuildUnit();
            case HunkType.HUNK_LIB:
                this.hunkFile.type = FileType.TYPE_LIB;
                return this.build_lib();
            default:
                this.hunkFile.type = FileType.TYPE_UNKNOWN;
                return false;
            }
        }

        // ---------- Build Segments from Hunks ----------

        public virtual object get_hunk_summary()
        {
            return this.get_struct_summary(hunkFile.hunks);
        }

        public virtual object get_struct_summary(object obj)
        {
            /*
                        object type_name;
                        object v;
                        if (obj.GetType().IsGenericType) {
                            {
                                if (obj.GetType().GetGenericTypeDefinition().Name == "List")
                                {
                                var result = new List<object>();
                                foreach (var a in obj) {
                                    v = this.get_struct_summary(a);
                                    if (v is not null) {
                                        result.append(v);
                                    }
                                }
                                return "{" + string.Join(",", result) + "]";
                            } else if (type(obj) == DictType) {
                                if (obj.has_key("type_name")) {
                                    type_name = obj["type_name"];
                                    return type_name.replace("HUNK_", "");
                                }
                            } else {
                                result = new List<object>();
                                foreach (var k in obj.keys()) {
                                    v = this.get_struct_summary(obj[k]);
                                    if (v is not null) {
                                        result.append(k + ":" + v);
                                    }
                                }
                                return "{" + ",".join(result) + "}";
                            }
                        } else {
                            return null;
                        }
             * */
            throw new NotImplementedException();
        }

        public virtual object get_segment_summary()
        {
            return this.get_struct_summary(this.hunkFile.segments);
        }

        public virtual object? get_overlay_segment_summary()
        {
            if (this.hunkFile.overlay_segments is not null)
            {
                return this.get_struct_summary(this.hunkFile.overlay_segments);
            }
            else
            {
                return null;
            }
        }

        public virtual object? get_libs_summary()
        {
            if (this.hunkFile.libs is not null)
            {
                return this.get_struct_summary(this.hunkFile.libs);
            }
            else
            {
                return null;
            }
        }

        public virtual object? get_units_summary()
        {
            if (this.hunkFile.units is not null)
            {
                return this.get_struct_summary(this.hunkFile.units);
            }
            else
            {
                return null;
            }
        }
        /*
bin = args.bin
bin_args = args.args
print "vamos: %s %s" % (bin, bin_args)
 
# --- load binary ---
hunk_file = HunkReader.HunkReader()
fobj = file(bin,"rb")
result = hunk_file.read_file_obj(bin,fobj,None)
if result != Hunk.RESULT_OK:
  print "Error loading '%s'" % (bin)
  sys.exit(1)
# build segments
ok = hunk_file.build_segments()
if not ok:
  print "Error building segments for '%s'" % (bin)
  sys.exit(2)
# make sure its a loadseg()
if hunk_file.type != Hunk.TYPE_LOADSEG:
  print "File not loadSeg()able: '%s'" % (bin)
  sys.exit(3)
 
# --- create memory layout ---
print "setting up memory layout"
layout = MemoryLayout.MemoryLayout(verbose=True)
context = MachineContext.MachineContext(MusashiCPU(),layout)
 
# place program segments
prog_base = 0x010000
prog_start = prog_base
off = prog_base
relocator = HunkRelocate.HunkRelocate(hunk_file)
prog_data = relocator.relocate_one_block(prog_base, padding=8)
prog_size = len(prog_data)
prog_mem = MemoryBlock.MemoryBlock("prog", prog_base, prog_size)
prog_mem.write_data(prog_base, prog_data)
layout.add_range(prog_mem)
print prog_mem
 
# some segment info
seg_sizes = relocator.get_sizes()
seg_addrs = relocator.get_seq_addrs(prog_base, padding=8)
for i in xrange(len(seg_sizes)):
  print "  seg:  @%06x  +%06x" % (seg_addrs[i], seg_sizes[i])
 
# setup stack
magic_end = 0xff0000
stack_base = 0x080000
stack_size = 0x001000
stack_end = stack_base + stack_size
stack_mem = MemoryBlock.MemoryBlock("stack", stack_base, stack_size)
stack_initial = stack_end - 4
stack_mem.w32(stack_initial, magic_end)
layout.add_range(stack_mem)
 
# setup argument
arg_base = 0x1000
arg_text = " ".join(bin_args)
arg_len  = len(arg_text)
arg_size = arg_len + 1
arg_mem  = MemoryBlock.MemoryBlock("args", arg_base, arg_size)
arg_mem.write_data(arg_base, arg_text)
arg_mem.write_mem(0, arg_base + arg_len, 0)
layout.add_range(arg_mem)
print "args: %s (%d)" % (arg_text, arg_len)
print arg_mem
*/

        private byte[] RelocateBytes(Address addrLoad)
        {
            var rel = new HunkRelocator(this.hunkFile);
            // Get sizes of all segments
            var sizes = rel.GetSegmentSizes();
            // Determine begin addrs for all segments
            uint base_addr = addrLoad.ToUInt32();
            var addrs = rel.GetSegmentRelocationAddresses(base_addr);
            //  Relocate and return data of segments
            var datas = rel.Relocate(addrs);
            if (datas is null)
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
