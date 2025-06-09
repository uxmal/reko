using Reko.Arch.M68k.Disassembler;
using Reko.Core.Collections;
using Reko.Core.Memory;
using Reko.ImageLoaders.Hunk;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;

namespace Reko.Tools.HunkTool
{
    public class HunkShow
    {
        private HunkFile hunk_file;
        private HeaderHunk header;
        private List<List<Hunk>> segments;
        private Hunk overlay;
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

        public HunkShow(HunkFile hunk_file, bool show_relocs = false, bool show_debug = false, bool disassemble = false, uint disassemble_start = 0, bool hexdump = false, bool brief = false, bool use_objdump = false, string cpu = "68000")
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
                Console.WriteLine("Library #{0}", lib.lib_no);
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
                foreach (var o in Enumerable.Range(0, num_ov))
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
            int? alloc_size;
            object data_file_offset;
            object hunk_file_offset;
            object name;
            object size;
            var main = hunk[0];
            // unit hunks are named
            name = "";
            if (!string.IsNullOrEmpty(hunk[0].Name))
            {
                name = String.Format("'{0}'", main.Name);
            }
            var type_name = main.HunkType.ToString().Replace("HUNK_", "");
            size = main.size;
            data_file_offset = null;
            hunk_file_offset = null;
            if (main.alloc_size > 0)
            {
                alloc_size = main.alloc_size;
            }
            else
            {
                alloc_size = null;
            }
            this.print_segment_header(main.hunk_no, type_name, size, name, data_file_offset, hunk_file_offset, alloc_size);
            if (this.hexdump && main.Data is not null)
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
            if (main.HunkType == HunkType.HUNK_CODE && this.disassemble && main.Data is not null && main.Data.Length > 0)
            {
                var dasm = M68kDisassembler.Create68000(new ServiceContainer(), new BeImageReader(main.Data, 0));
                Console.WriteLine();
                foreach (var instr in dasm)
                {
                    //disas.show_disassembly(hunk, seg_list, this.disassemble_start);

                }
                Console.WriteLine();
            }
        }

        public virtual void show_index_info(IHunk info)
        {
            // references from index
            if (info.refs is not null && info.refs.Count > 0)
            {
                this.print_extra("refs", string.Format("#{0}", info.refs.Count));
                if (!this.brief)
                {
                    foreach (var @ref in info.refs)
                    {
                        this.print_symbol(~0u, @ref.name, string.Format("({0} bits)", @ref.bits));
                    }
                }
            }
                        // defines from index
            if (info.defs is not null && info.defs.Count > 0)
            {
                this.print_extra("defs", string.Format("#{0}", info.defs.Count));
                if (!this.brief)
                {
                    foreach (var d in info.defs)
                    {
                        this.print_symbol((uint) d.value, d.name, string.Format("(type {0})", d.type));
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
                this.print_extra("reloc", String.Format("{0} #{1}", type_name, ((RelocHunk)hunk).reloc));
                if (!this.brief)
                {
                    this.show_reloc_hunk((RelocHunk)hunk);
                }
            }
            else if (hunk_type == HunkType.HUNK_DEBUG)
            {
                this.print_extra("debug", String.Format("{0}  Offset={1:X8}", ((DebugHunk)hunk).debug_type, ((DebugHunk)hunk).debug_offset));
                if (!this.brief)
                {
                    this.show_debug_hunk((DebugHunk)hunk);
                }
            }
            else if (hunk_type == HunkType.HUNK_SYMBOL)
            {
                var h = (SymbolHunk)hunk;
                this.print_extra("symbol", String.Format("#{0}", h.symbols.Count));
                if (!this.brief)
                {
                    this.show_symbol_hunk(h);
                }
            }
            else if (hunk_type == HunkType.HUNK_EXT)
            {
                var h = (ExtHunk)hunk;
                this.print_extra("ext", String.Format("def #{0}  ref #{1}  common #{1}",
                    h.ext_def, h.ext_ref.Count, h.ext_common.Count));
                if (!this.brief)
                {
                    this.show_ext_hunk(h);
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
            foreach (var hunk_num in reloc.Keys)
            {
                var offsets = reloc[hunk_num];
                if (this.show_relocs)
                {
                    foreach (var offset in offsets)
                    {
                        this.print_symbol(offset, string.Format("Segment #{0}", hunk_num), "");
                    }
                }
                else
                {
                    this.print_extra_sub(String.Format("To Segment #{0}: {1,4} entries", hunk_num, offsets.Count));
                    foreach (var off in offsets)
                    {
                        this.print_extra_sub(string.Format("\t{0:X8}", off));
                    }
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
                        var addr = src_off.Value;
                        var line = src_off.Key;
                        this.print_symbol(addr, String.Format("line {0}", line), "");
                    }
                }
            }
            else if (this.show_debug)
            {
                print_hex(hunk.Data, indent : 8);
            }
        }

        public virtual void show_symbol_hunk(SymbolHunk hunk)
        {
            foreach (var symbol in hunk.symbols)
            {
                this.print_symbol((uint)symbol.Value, symbol.Key, "");
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
                // ----- printing -----
            }
        }

        public virtual void print_header(HeaderHunk hdr)
        {
            Console.WriteLine("\t      header (segments: first={0}, last={1}, table size={2})", hdr.FirstHunkId, hdr.LastHunkId, hdr.table_size);
        }

        public virtual void print_extra(object type_name, object info)
        {
            Console.WriteLine("\t\t{0,-8}  {0}", type_name, info);
        }

        public virtual void print_extra_sub(string text)
        {
            Console.WriteLine("\t\t\t{0}", text);
        }

        public virtual void print_segment_header(int hunk_no, string type_name, object size, object name, object data_file_offset, object hunk_file_offset, int? alloc_size)
        {
            var extra = new StringBuilder();
            if (alloc_size is not null)
            {
                extra.AppendFormat("alloc size {0:X8}  ", alloc_size);
            }
            extra.AppendFormat("file header @{0:X8}", hunk_file_offset);
            if (data_file_offset is not null)
            {
                extra.AppendFormat("  data @{0:X8}", data_file_offset);
            }
            Console.WriteLine("\t#{0:3}  {1,5}  size {2:X8}  {3}  {4}", hunk_no, type_name, size, extra, name);
        }

        public virtual void print_symbol(uint addr, string name, object extra)
        {
            string a;
            if (addr == ~0u)
            {
                a = "xxxxxxxx";
            }
            else
            {
                a = String.Format("{0:X8}", addr);
            }
            Console.WriteLine(String.Format("\t\t\t{0}  {1,-32}  {2}", a, name, extra));
        }

        public virtual void print_unit(object no, object name)
        {
            Console.WriteLine(String.Format("  #{0,3}  UNIT  {1}", no, name));
        }

        private void print_hex(byte[] bytes, int indent = 0)
        {
            foreach (var line in bytes.Chunks(16))
            {
                Console.WriteLine("{0}{1}",
                    new string(' ', indent),
                    string.Join(" ", line.Select(b => string.Format("{0:X2}", (int)b))));
            }
        }
    }
}