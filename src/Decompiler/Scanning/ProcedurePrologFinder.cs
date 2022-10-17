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
using Reko.Core.Memory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Reko.Scanning
{
    public class ProcedurePrologFinder : IBaseAddressFinder
    {
        private readonly ByteMemoryArea mem;
        private readonly ByteTrie<object> trie;
        private readonly int max_matches = 10;

        public ProcedurePrologFinder(ByteMemoryArea mem, EndianServices endianness)
        {
            //$TODO: get this info from the Reko.config file
            this.mem = mem;
            this.trie = new ByteTrie<object>();
            trie.Add(new byte[] { 0x55, 0x89, 0xE5, 0x83 }, 4);
            trie.Add(new byte[] { 0x55, 0x89, 0xE5 }, 3);
            trie.Add(new byte[] { 0x55, 0x8B, 0xEC }, 3);
            //this.trie = BuildTrie(patterns, arch);
            this.Endianness = endianness;
        }

        public EndianServices Endianness { get; set; }

        public BaseAddressCandidate[] Run()
        {
            int threadIndex = 0;
            var prologs = PatternFinder.FindProcedurePrologs(mem, trie);
            Console.WriteLine($"Found {prologs.Count} possible prologs");
            var sw = new Stopwatch();
            sw.Start();
            var pointers = ReadPointers(mem, 4);
            uint stride = 0x1000;
            var heap = new List<BaseAddressCandidate>();
            var news = new HashSet<ulong>(prologs.Count);
            var queue = new Queue<List<BaseAddressCandidate>>();
            for (ulong uBaseAddr = 0; uBaseAddr <= ~0u;)
            {
                news.Clear();
                foreach (var p in prologs)
                {
                    if (!AddOverflow(p, uBaseAddr, out var addrRebased))
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
                if (AddOverflow(uBaseAddr, stride, out var new_addr))
                {
                    Console.WriteLine($"{threadIndex,3} Ending at {uBaseAddr:X8}, stride = 0x{stride}");
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

            Console.WriteLine("Elapsed time: {0}ms", (int)sw.Elapsed.TotalMilliseconds);
            return result;
        }

        private static bool AddOverflow(ulong a, ulong b, out ulong result)
        {
            var s = a + b;
            if (s < a)
            {
                result = 0;
                return true;
            }
            else
            {
                result = s;
                return false;
            }
        }
        public HashSet<ulong> ReadPointers(ByteMemoryArea buffer, int alignment)
        {
            var pointers = new HashSet<ulong>();
            var rdr = Endianness.CreateImageReader(buffer, 0);
            var offset = rdr.Offset;
            while (rdr.TryReadUInt32(out uint v))
            {
                pointers.Add(v);
                offset = offset + alignment;
                rdr.Offset = offset;
            }
            return pointers;
        }

        private static ByteTrie<object> BuildTrie(IEnumerable<MaskedPattern> patterns, IProcessorArchitecture arch)
        {
            int unitsPerInstr = arch.InstructionBitSize / arch.InstructionBitSize;
            if (arch.Endianness == EndianServices.Big)
            {
                throw new NotImplementedException();
            }
            else
            {
                var trie = new ByteTrie<object>();
                foreach (var pattern in patterns)
                {
                    trie.Add(pattern.Bytes, pattern.Mask, new Object());
                }
                return trie;
            }
        }

        public List<(Address, int)> FindPrologs(MemoryArea mem)
        {
            throw new NotImplementedException();
        }

    }
}
