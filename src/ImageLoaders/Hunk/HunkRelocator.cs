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
    /// Relocates any pointer
    /// </summary>
    public partial class HunkRelocator
    {
        public static TraceSwitch Trace = new TraceSwitch("HunkRelocation", "Hunk relocation");
        private HunkFile hunk_file;

        public HunkRelocator(HunkFile hunk_file)
        {
            this.hunk_file = hunk_file;
        }

        public IEnumerable<int> GetSegmentSizes()
        {
            return this.hunk_file.segments.Select(s => s[0].alloc_size);
        }

        private uint GetTotalSize()
        {
            return (uint) this.GetSegmentSizes().Sum(x => x);
        }

        private IEnumerable<string> GetTypeNames()
        {
            return this.hunk_file.segments
                .Select(s => s[0].HunkType.ToString());
        }

        // generate a sequence of addresses suitable for relocation
        // in a single block
        public List<uint> GetSegmentRelocationAddresses(uint baseAddress, uint padding = 0)
        {
            var sizes = this.GetSegmentSizes();
            return GetSegmentRelocationAddresses(baseAddress, padding, sizes)
                .ToList();
        }

        private IEnumerable<uint> GetSegmentRelocationAddresses(uint baseAddress, uint padding, IEnumerable<int> sizes)
        {
            var addr = baseAddress;
            return sizes.Select(s =>
            {
                uint a = addr;
                addr += (uint) s;
                return a;
            });
        }

        public byte[] Relocate(List<uint> addr)
        {
            var datas = new System.IO.MemoryStream();
            foreach (List<Hunk> segment in this.hunk_file.segments)
            {
                Debug.WriteLineIf(Trace.TraceVerbose, string.Format("Relocating segment {0}", segment[0]));
                var mainHunk = segment[0];
                int hunk_no = mainHunk.hunk_no;
                int alloc_size = mainHunk.alloc_size;
                int size = mainHunk.size;

                // Fill in segment data
                byte[] data;
                var txt = mainHunk as TextHunk;
                if (txt is not null)
                {
                    Debug.Assert(txt.size <= alloc_size);
                    data = txt.Data!;
                }
                else
                {
                    data = new byte[alloc_size];
                }
                Debug.WriteLineIf(Trace.TraceVerbose, string.Format("#{0:X2} @ {1:X6}", hunk_no, addr[hunk_no]));

                // Find relocation hunks
                foreach (var relocHunk in segment.Skip(1)
                    .Where(h => h.HunkType == HunkType.HUNK_ABSRELOC32)
                    .Cast<RelocHunk>())
                {
                    foreach (var hunkNo in relocHunk.reloc!.Keys)
                    {
                        // Get address of other hunk
                        var hunk_addr = addr[hunkNo];
                        var offsets = relocHunk.reloc[hunkNo];
                        foreach (var offset in offsets)
                        {
                            this.relocate32(hunk_no, data, offset, hunk_addr);
                        }
                    }
                }
                datas.Write(data, 0, data.Length);
            }
            return datas.ToArray();
        }

        public void relocate32(int hunk_no, byte[] data, uint offset, uint hunk_addr)
        {
            var delta = ByteMemoryArea.ReadBeUInt32(data, offset);
            var addr = hunk_addr + delta;
            ByteMemoryArea.WriteBeUInt32(data, offset, addr);
            Debug.WriteIf(Trace.TraceVerbose, string.Format("#{0,2} + {1:X8}: {2:X6} (delta) + {3:X6} (hunk_addr) -> {4:X6}", hunk_no, offset, delta, hunk_addr, addr));
        }
    }
}
