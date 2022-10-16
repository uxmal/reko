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

        public ProcedurePrologFinder(ByteMemoryArea mem)
        {
            //$TODO: get this info from the Reko.config file
            this.mem = mem;
            this.trie = new ByteTrie<object>();
            trie.Add(new byte[] { 0x55, 0x89, 0xE5, 0x83 }, 4);
            trie.Add(new byte[] { 0x55, 0x89, 0xE5 }, 3);
            trie.Add(new byte[] { 0x55, 0x8B, 0xEC }, 3);
            //this.trie = BuildTrie(patterns, arch);
            this.Endianness = EndianServices.Little;
        }

        public EndianServices Endianness { get; set; }

        public Task Run()
        {
            int threadIndex = 0;
            var prologs = PatternFinder.FindProcedurePrologs(mem, trie);
            Console.WriteLine($"Found {prologs.Count} possible prologs");
            var sw = new Stopwatch();
            sw.Start();
            var pointers = ReadPointers(mem, 4);
            uint stride = 0x1000;
            var heap = new List<(int, ulong)>();
            var news = new HashSet<ulong>(prologs.Count);
            var queue = new Queue<List<(int, ulong)>>();
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
                //var intersection = news.Intersect(pointers).ToHashSet();
                if (intersection.Count > 0)
                {
                    heap.Add((intersection.Count, uBaseAddr));
                }
                if (AddOverflow(uBaseAddr, stride, out var new_addr))
                {
                    Console.WriteLine($"{threadIndex,3} Ending at {uBaseAddr:X8}, _stride = 0x{stride}");
                    break;
                }
                uBaseAddr = new_addr;
            }
            queue.Enqueue(heap);

            // Merge all of the heaps.
            var result = queue
                .SelectMany(c => c)
                .OrderByDescending(c => c.Item1)
                .ThenBy(c => c.Item2)
                .Take(max_matches);

            sw.Stop();

            // Print (up to) top N results.
            foreach (var child in result)
            {
                Console.WriteLine("0x{0:X8}: {1}", child.Item2, child.Item1);
            }
            Console.WriteLine("Elapsed time: {0}ms", (int)sw.Elapsed.TotalMilliseconds);
            return Task.CompletedTask;
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
