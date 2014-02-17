using Decompiler.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Decompiler.ImageLoaders.Hunk
{
    /// <summary>
    /// Relocates any pointer
    /// </summary>
    public class HunkRelocator
    {
        public static TraceSwitch Trace = new TraceSwitch("HunkRelocation", "Hunk relocation");
        private HunkLoader hunk_file;

        public HunkRelocator(HunkLoader hunk_file)
        {
            this.hunk_file = hunk_file;
        }

        public IEnumerable<uint> GetSegmentSizes()
        {
            return this.hunk_file.segments.Select(s => s.hunks[0].alloc_size);
        }

        private uint GetTotalSize()
        {
            return (uint) this.GetSegmentSizes().Sum(x => x);
        }

        private IEnumerable<string> GetTypeNames()
        {
            return this.hunk_file.segments
                .Select(s => s.hunks[0].HunkType.ToString());
        }

        // generate a sequence of addresses suitable for relocation
        // in a single block
        public List<uint> GetSegmentRelocationAddresses(uint baseAddress, uint padding = 0)
        {
            var sizes = this.GetSegmentSizes();
            return GetSegmentRelocationAddresses(baseAddress, padding, sizes)
                .ToList();
        }

        private IEnumerable<uint> GetSegmentRelocationAddresses(uint baseAddress, uint padding, IEnumerable<uint> sizes)
        {
            var addr = baseAddress;
            return sizes.Select(s =>
            {
                uint a = addr;
                addr += s;
                return a;
            });
        }

        public byte[] Relocate(List<uint> addr)
        {
            var datas = new System.IO.MemoryStream();
            foreach (Segment segment in this.hunk_file.segments)
            {
                Debug.WriteLineIf(Trace.TraceVerbose, string.Format("Relocating segment {0}", segment.hunks[0]));
                var mainHunk = segment.hunks[0];
                int hunk_no = mainHunk.hunk_no;
                uint alloc_size = mainHunk.alloc_size;
                uint size = mainHunk.Size;

                // Fill in segment data
                BeImageWriter data;
                var txt = mainHunk as TextHunk;
                if (txt != null)
                {
                    Debug.Assert(txt.Size <= alloc_size);
                    data = new BeImageWriter(txt.Data);
                }
                else
                {
                    data = new BeImageWriter(new byte[alloc_size]);
                }
                Debug.WriteLineIf(Trace.TraceVerbose, string.Format("#{0:X2} @ {1:X6}", hunk_no, addr[hunk_no]));

                // Find relocation hunks
                foreach (var relocHunk in segment.hunks
                    .Where(h => h.HunkType == HunkType.HUNK_ABSRELOC32)
                    .Cast<RelocHunk>())
                {
                    foreach (var hunk_num in relocHunk.reloc.Keys)
                    {
                        // get address of other hunk
                        var hunk_addr = addr[hunk_num];
                        var offsets = relocHunk.reloc[hunk_num];
                        foreach (var offset in offsets)
                        {
                            throw new NotImplementedException();    //$TODO:
                            //this.relocate32(hunk_no, data, offset, hunk_addr);
                        }
                    }
                }
                datas.Write(data.Bytes, 0, data.Bytes.Length);
            }
            return datas.ToArray();
        }

        public void relocate32(int hunk_no, byte[] data, uint offset, uint hunk_addr)
        {
            throw new NotImplementedException();    //$TODO
            //var delta = this.read_long(data, offset);
            //var addr = hunk_addr + delta;
            //this.write_long(data, offset, addr);
            //Debug.WriteLineIf(Trace.TraceVerbose, string.Format("#{0:2} + {1:X6}: {2:X6} (delta) + {3:X6} (hunk_addr) -> {4:X6}",
            //    hunk_no, offset, delta, hunk_addr, addr));
        }
    }

    //public void read_long(byte [] data, offset) {
    //  bytes = data[offset:offset+4];
    //  return struct.unpack(">i",bytes)[0];
    //}

    //public void write_long(this, data, offset, value) {
    //  bytes = struct.pack(">i",value);
    //  data[offset:offset+4] = bytes;
    //  }
    //  }
}
