using System;
using System.Collections.Generic;
using System.Linq;

namespace Decompiler.ImageLoaders.Hunk
{
    public class HunkShow
    {
        private HunkFile hunk_file;

        public HunkShow(HunkFile hunk_file)
            : this(hunk_file, false, false, false, 0, false, false, false, "68000")
        {
        }

        public HunkShow(HunkFile hunk_file, bool show_relocs)
            : this(hunk_file, show_relocs, false, false, 0, false, false, false, "68000")
        {
        }

        public HunkShow(HunkFile hunk_file, bool show_relocs, bool show_debug)
            : this(hunk_file, show_relocs, show_debug, false, 0, false, false, false, "68000")
        {
        }

        public HunkShow(HunkFile hunk_file, bool show_relocs, bool show_debug, bool disassemble)
            : this(hunk_file, show_relocs, show_debug, disassemble, 0, false, false, false, "68000")
        {
        }

        public HunkShow(HunkFile hunk_file, bool show_relocs, bool show_debug, bool disassemble, uint disassemble_start)
            : this(hunk_file, show_relocs, show_debug, disassemble, disassemble_start, false, false, false, "68000")
        {
        }

        public HunkShow(HunkFile hunk_file, bool show_relocs, bool show_debug, bool disassemble, uint disassemble_start, bool hexdump)
            : this(hunk_file, show_relocs, show_debug, disassemble, disassemble_start, hexdump, false, false, "68000")
        {
        }

        public HunkShow(HunkFile hunk_file, bool show_relocs, bool show_debug, bool disassemble, uint disassemble_start, bool hexdump, bool brief)
            : this(hunk_file, show_relocs, show_debug, disassemble, disassemble_start, hexdump, brief, false, "68000")
        {
        }

        public HunkShow(HunkFile hunk_file, bool show_relocs, bool show_debug, bool disassemble, uint disassemble_start, bool hexdump, bool brief, bool use_objdump)
            : this(hunk_file, show_relocs, show_debug, disassemble, disassemble_start, hexdump, brief, use_objdump, "68000")
        {
        }

        private HeaderHunk header;
        private List<List<Hunk>> segments;
        private object overlay;
        private List<HeaderHunk> overlay_headers;
        private List<List<List<Hunk>>> overlay_segments;
        private List<Lib> libs;
        private List<Unit> units;
        private bool show_relocs;
        private bool show_debug;
        private bool disassemble;
        private uint disassemble_start;
        private bool use_objdump;
        private string cpu;
        private bool hexdump;
        private bool brief;

        public HunkShow(HunkFile hunk_file, bool show_relocs, bool show_debug, bool disassemble, uint disassemble_start, bool hexdump, bool brief, bool use_objdump, string cpu)
        {
            this.hunk_file = hunk_file;
            // clone file refs
            this.header = hunk_file.header;
            this.segments = hunk_file.segments;
            this.overlay = hunk_file.overlay;
            this.overlay_headers = hunk_file.overlay_headers;
            this.overlay_segments = hunk_file.overlay_segments;
            this.libs = hunk_file.libs;
            this.units = hunk_file.units;
            this.show_relocs = show_relocs;
            this.show_debug = show_debug;
            this.disassemble = disassemble;
            this.disassemble_start = disassemble_start;
            this.use_objdump = use_objdump;
            this.cpu = cpu;
            this.hexdump = hexdump;
            this.brief = brief;
        }

        public virtual void show_segments()
        {
            var hunk_type = this.hunk_file.type;
            if (hunk_type == FileType.TYPE_LOADSEG)
            {
                this.show_loadseg_segments();
            }
            else if (hunk_type == FileType.TYPE_UNIT)
            {
                this.show_unit_segments();
            }
            else if (hunk_type == FileType.TYPE_LIB)
            {
                this.show_lib_segments();
            }
        }

        public virtual void show_lib_segments()
        {
            foreach (var lib in this.libs)
            {
                Console.WriteLine(String.Format("Library #{0}", lib.lib_no));
                foreach (var unit in lib.units)
                {
                    this.print_unit(unit.unit_no, unit.name);
                    foreach (var segment in unit.segments)
                    {
                        this.show_segment(segment, unit.segments);
                    }
                }
            }
        }

        public virtual void show_unit_segments()
        {
            foreach (var unit in this.units)
            {
                this.print_unit(unit.unit_no, unit.name);
                foreach (var segment in unit.segments)
                {
                    this.show_segment(segment, unit.segments);
                }
            }
        }

        public virtual void show_loadseg_segments()
        {
            // header + segments
            if (!this.brief)
            {
                this.print_header(this.header);
            }
            foreach (var segment in this.segments)
            {
                this.show_segment(segment, this.segments);
                // overlay
            }
            if (this.overlay is not null)
            {
                Console.WriteLine("Overlay");
                int num_ov = this.overlay_headers.Count;
                for (int o = 0; o < num_ov; ++o)
                {
                    if (!this.brief)
                    {
                        this.print_header(this.overlay_headers[o]);
                    }
                    foreach (var segment in this.overlay_segments[o])
                    {
                        this.show_segment(segment, this.overlay_segments[o]);
                    }
                }
            }
        }

        public virtual void show_segment(List<Hunk> hunk, List<List<Hunk>> seg_list)
        {
            var main = hunk[0];
            // unit hunks are named
            var name = "";
            if (main.Name is not null)
            {
                name = String.Format("'{0}'", main.Name);
            }
            var type_name = main.HunkType.ToString().Replace("HUNK_", "");
            var size = main.size;
            var hunk_no = main.hunk_no;
            uint data_file_offset = 0;
            if (main is TextHunk)
            {
                data_file_offset = ((TextHunk) main).data_file_offset;
            }
            var hunk_file_offset = main.FileOffset;
            var alloc_size = (main.alloc_size != 0) ? main.alloc_size : 0;
            this.print_segment_header(hunk_no, type_name, size, name, data_file_offset, hunk_file_offset, alloc_size);
            if (this.hexdump)
            {
                print_hex(main.Data, indent: 8);
            }
            foreach (var extra in hunk.Skip(1))
            {
                this.show_extra_hunk(extra);
            }
            // index hunk info is embedded if its in a lib
            if (main.index_hunk is not null)
            {
                this.show_index_info(main.index_hunk);
            }
            if (main.HunkType == HunkType.HUNK_CODE && this.disassemble && main.Data.Length > 0)
            {
                var disas = new HunkDisassembler(this.use_objdump, this.cpu);
                Console.WriteLine();
                disas.show_disassembly(hunk, seg_list, this.disassemble_start);
                Console.WriteLine();
            }
        }

        private void print_hex(byte[] p, int indent = 4)
        {
            throw new NotImplementedException();
        }

        public virtual void show_index_info(IHunk info)
        {
            // references from index
            if (info.refs is not null)
            {
                this.print_extra("refs", String.Format("#{0}", info.refs.Count));
                if (!this.brief)
                {
                    foreach (var @ref in info.refs)
                    {
                        this.print_symbol(~0u, @ref.name, String.Format("({0} bits)", @ref.bits));
                        // defines from index
                    }
                }
            }
            if (info.defs is not null)
            {
                this.print_extra("defs", String.Format("#{0}", info.defs.Count));
                if (!this.brief)
                {
                    foreach (var d in info.defs)
                    {
                        this.print_symbol((uint) d.value, d.name, String.Format("(type {0})", d.type));
                    }
                }
            }
        }

        public virtual void show_extra_hunk(Hunk hunk)
        {
            var hunk_type = hunk.HunkType;
            if (HunkFileParser.reloc_hunks.Contains(hunk_type))
            {
                var type_name = hunk.HunkType.ToString().Replace("HUNK_", "").ToLower();
                var rhunk = (RelocHunk) hunk;
                this.print_extra("reloc", String.Format("{0} #{1}", type_name, rhunk.reloc.Count));
                if (!this.brief)
                {
                    this.show_reloc_hunk(rhunk);
                }
            }
            else if (hunk_type == HunkType.HUNK_DEBUG)
            {
                var dhunk = (DebugHunk) hunk;
                this.print_extra("debug", String.Format("{0}  offset={1:X8}", dhunk.debug_type, dhunk.debug_offset));
                if (!this.brief)
                {
                    this.show_debug_hunk(dhunk);
                }
            }
            else if (hunk_type == HunkType.HUNK_SYMBOL)
            {
                var shunk = (SymbolHunk) hunk;
                this.print_extra("symbol", String.Format("#{0}", shunk.symbols));
                if (!this.brief)
                {
                    this.show_symbol_hunk(shunk);
                }
            }
            else if (hunk_type == HunkType.HUNK_EXT)
            {
                var ehunk = (ExtHunk) hunk;
                this.print_extra("ext", String.Format("def #{0}  ref #{1}  common #{2}", ehunk.ext_def.Count, ehunk.ext_ref.Count, ehunk.ext_common.Count));
                if (!this.brief)
                {
                    this.show_ext_hunk(ehunk);
                }
            }
            else
            {
                this.print_extra("extra", String.Format("{0}", hunk.HunkType));
            }
        }

        public virtual void show_reloc_hunk(RelocHunk hunk)
        {
            var reloc = hunk.reloc;
            foreach (var hunkNo in reloc.Keys)
            {
                var offsets = reloc[hunkNo];
                if (this.show_relocs)
                {
                    foreach (var offset in offsets)
                    {
                        this.print_symbol(offset, String.Format("Segment {0}", hunkNo), "");
                    }
                }
                else
                {
                    this.print_extra_sub(String.Format("To Segment #{0}: {1,4} entries", hunkNo, offsets));
                }
            }
        }

        public virtual void show_debug_hunk(DebugHunk hunk)
        {
            var debug_type = hunk.debug_type;
            if (debug_type == "LINE")
            {
                this.print_extra_sub(String.Format("line for '{0}'", hunk.src_file));
                if (this.show_debug)
                {
                    foreach (var src_off in hunk.src_map)
                    {
                        uint addr = src_off.Value;
                        int line = src_off.Key;
                        this.print_symbol(addr, String.Format("line {0}", line), "");
                    }
                }
            }
            else if (this.show_debug)
            {
                print_hex(hunk.Data, indent: 8);
            }
        }

        public virtual void show_symbol_hunk(SymbolHunk hunk)
        {
            foreach (var symbol in hunk.symbols)
            {
                this.print_symbol((uint) symbol.Value, symbol.Key, "");
            }
        }

        public virtual void show_ext_hunk(ExtHunk hunk)
        {
            // definition
            foreach (var ext in hunk.ext_def)
            {
                var tname = ext.type.ToString().Replace("EXT_", "").ToLower();
                this.print_symbol(ext.def, ext.name, tname);
            }
            // references
            foreach (var ext in hunk.ext_ref)
            {
                var refs = ext.refs;
                var tname = ext.type.ToString().Replace("EXT_", "").ToLower();
                foreach (var @ref in refs)
                {
                    this.print_symbol(@ref, ext.name, tname);
                }
            }
            // common_base
            foreach (var ext in hunk.ext_common)
            {
                var tname = ext.type.ToString().Replace("EXT_", "").ToLower();
                this.print_symbol(ext.common_size, ext.name, tname);
            }
        }

        // ----- printing -----
        public virtual void print_header(HeaderHunk hdr)
        {
            Console.WriteLine(String.Format("\t      header (segments: first={0}, last={1}, table size={2})", hdr.FirstHunkId, hdr.LastHunkId, hdr.table_size));
        }

        public virtual void print_extra(string type_name, object info)
        {
            Console.WriteLine(String.Format("\t\t%8s  %s", type_name, info));
        }

        public virtual void print_extra_sub(object text)
        {
            Console.WriteLine(String.Format("\t\t\t{0}", text));
        }

        public virtual void print_segment_header(object hunk_no, object type_name, object size, object name, object data_file_offset, object hunk_file_offset, object alloc_size)
        {
            string extra;
            extra = "";
            if (alloc_size is not null)
            {
                extra += String.Format("alloc size %08x  ", alloc_size);
            }
            extra += String.Format("file header @%08x", hunk_file_offset);
            if (data_file_offset is not null)
            {
                extra += String.Format("  data @%08x", data_file_offset);
            }
            Console.WriteLine(String.Format("\t#%03d  %-5s  size %08x  %s  %s", hunk_no, type_name, size, extra, name));
        }

        public virtual void print_symbol(uint addr, object name, object extra)
        {
            object a;
            if (addr == ~0u)
            {
                a = "xxxxxxxx";
            }
            else
            {
                a = String.Format("{0:X8}", addr);
            }
            Console.WriteLine(String.Format("\t\t\t{0}  %{1,32}  {2}", a, name, extra));
        }

        public virtual void print_unit(object no, object name)
        {
            Console.WriteLine(String.Format("  #{0,3}  UNIT  {1}", no, name));
        }
    }
}
