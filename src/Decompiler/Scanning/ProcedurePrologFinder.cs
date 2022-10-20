#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using Reko.Core.Collections;
using Reko.Core.Diagnostics;
using Reko.Core.Lib;
using Reko.Core.Memory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace Reko.Scanning
{
    public class ProcedurePrologFinder : AbstractBaseAddressFinder
    {
        private static readonly TraceSwitch trace = new TraceSwitch(nameof(ProcedurePrologFinder), nameof(ProcedurePrologFinder));
        private const int max_matches = 10;

        private readonly IProcessorArchitecture arch;
        private readonly ByteTrie<object> trie;

        public ProcedurePrologFinder(
            IProcessorArchitecture arch,
            IEnumerable<MaskedPattern> patterns,
            ByteMemoryArea mem) 
            : base(arch.Endianness, mem)
        {
            this.arch = arch;
            this.trie = BuildTrie(patterns, arch);
            Stride = 0x1000;
        }

        public override BaseAddressCandidate[] Run()
        {
            int threadIndex = 0;
            var prologsOffsets = PatternFinder.FindProcedurePrologs(Memory, trie);
            trace.Inform($"Found {prologsOffsets.Count} possible prolog offsets");
            var sw = new Stopwatch();
            sw.Start();
            var alignment = arch.InstructionBitSize / arch.MemoryGranularity;
            var pointers = ReadPointers(Memory, alignment);
            var heap = new List<BaseAddressCandidate>();
            var news = new HashSet<ulong>(prologsOffsets.Count);
            var queue = new Queue<List<BaseAddressCandidate>>();
            var wordMask = Bits.Mask(0, arch.PointerType.BitSize);
            for (ulong uBaseAddr = 0; uBaseAddr <= ~0u;)
            {
                news.Clear();
                foreach (var p in prologsOffsets)
                {
                    if (!AddOverflow(p, uBaseAddr, wordMask, out var addrRebased))
                    {
                        news.Add(addrRebased);
                    }
                }
                news.IntersectWith(pointers);
                var intersection = news;
                if (intersection.Count > 0)
                {
                    heap.Add(new BaseAddressCandidate(uBaseAddr, intersection.Count));
                }
                if (AddOverflow(uBaseAddr, Stride, wordMask, out var new_addr))
                {
                    trace.Verbose($"{threadIndex,3} Ending at {uBaseAddr:X8}, stride = 0x{Stride}");
                    break;
                }
                uBaseAddr = new_addr;
            }
            queue.Enqueue(heap);

            // Merge all of the heaps.
            var result = queue
                .SelectMany(c => c)
                .OrderByDescending(c => c.Confidence)
                .ThenBy(c => c.Address)
                .Take(max_matches)
                .ToArray();
            sw.Stop();

            trace.Inform("Elapsed time: {0}ms", (int)sw.Elapsed.TotalMilliseconds);
            return result;
        }




        private static ByteTrie<object> BuildTrie(IEnumerable<MaskedPattern> patterns, IProcessorArchitecture arch)
        {
            int unitsPerInstr = arch.InstructionBitSize / arch.MemoryGranularity;
            var trie = new ByteTrie<object>();
            foreach (var pattern in patterns)
            {
                var bytes = pattern.Bytes;
                var mask = pattern.Mask;
                if (bytes is null)
                    continue;
                if (unitsPerInstr > 1 && arch.Endianness != pattern.Endianness)
                {
                    bytes = EndianServices.SwapByGroups(bytes, unitsPerInstr);
                    if (mask is { })
                    {
                        mask = EndianServices.SwapByGroups(mask, unitsPerInstr);
                    }
                }
                if (mask is null)
                {
                    trie.Add(bytes, new object());
                }
                else
                {
                    trie.Add(bytes, mask, new object());
                }
            }
            return trie;
        }

    }
}
