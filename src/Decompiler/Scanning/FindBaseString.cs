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
using Reko.Core.Diagnostics;
using Reko.Core.Lib;
using Reko.Core.Memory;
using Reko.Core.Output;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace Reko.Scanning
{
    /// <summary>
    /// Finds the base address of a memory area by correlating 
    /// the starts of strings with the 
    /// </summary>
    public class FindBaseString : AbstractBaseAddressFinder
    {
        private static readonly TraceSwitch trace = new TraceSwitch(nameof(FindBaseString), nameof(FindBaseString))
        {
            Level = TraceLevel.Verbose
        };

        private readonly IProcessorArchitecture arch;
        private readonly IProgressIndicator progressIndicator;

        public FindBaseString(
            IProcessorArchitecture arch,
            ByteMemoryArea mem,
            IProgressIndicator progress)
            : base(arch.Endianness, mem)
        {
            this.arch = arch;
            Stride = 0x1000;
            Threads = Environment.ProcessorCount;
            this.progressIndicator = progress;
        }

        public int min_str_len = 10;           // Minimum string search length (default is 10)'
        public int max_matches = 10;           // Maximum matches to display (default is 10)'



        /// <summary>
        /// Number of threads to spawn; the default is # of CPU cores.
        /// </summary>
        public int Threads { get; set; }
        public ulong MinAddress { get; set; }

        public override BaseAddressCandidate[] Run()
        {
            // Find indices of strings and locations of pointers in parallel.
            var stringsTask = Task.Run(() => PatternFinder.FindAsciiStrings(Memory, min_str_len)
                .Select(s => s.uAddress)
                .ToHashSet());
            var pointersTask = Task.Run(() => ReadPointers(Memory, 4));
            var x = Memory.ReadLeUInt32(0x0300);
            var strings = stringsTask.Result;
            var pointers = pointersTask.Result;

            if (strings.Count == 0)
            {
                return Array.Empty<BaseAddressCandidate>();
            }
            trace.Inform("    Located {0} strings", strings.Count);
            trace.Inform("    Located {0} pointers", pointers.Count);

            var shared_strings = strings;
            var shared_pointers = pointers;

            Debug.WriteLine("Scanning with {0} Threads...", Threads);
            ConcurrentQueue<List<BaseAddressCandidate>> subresults = new();
            var sw = new Stopwatch();
            sw.Start();
            Parallel.For(0, this.Threads, (n) =>
            {
                var progress = n == 0
                    ? this.progressIndicator
                    : NullProgressIndicator.Instance;
                var result = FindMatches(strings, pointers, n, progress);
                subresults.Enqueue(result);
            });

            var result = MergeResults(subresults);

            sw.Stop();

            Console.WriteLine("Elapsed time: {0}ms", (int) sw.Elapsed.TotalMilliseconds);
            return result;
        }

        private BaseAddressCandidate[] MergeResults(ConcurrentQueue<List<BaseAddressCandidate>> queue)
        {
            return queue
                .SelectMany(c => c)
                .OrderByDescending(c => c.Confidence)
                .ThenBy(c => c.Address)
                .Take(max_matches)
                .ToArray();
        }

        public struct Interval
        {
            public ulong BeginAddress;
            public ulong EndAddress;

            public Interval(uint start_addr, uint end_addr)
            {
                this.BeginAddress = start_addr;
                this.EndAddress = end_addr;
            }

            public static Interval GetRange(
                ulong uAddrMin,
                nint index,
                nint max_threads,
                uint offset)
            { 
                if (index >= max_threads)
                {
                    throw new ArgumentException("Invalid index specified.");
                }

                if (BitOperations.PopCount(offset) != 1)
                    throw new ArgumentException("Invalid additive offset.");

                var addrSpan = (ulong) (uint.MaxValue) - uAddrMin;
                var start_addr = uAddrMin + (ulong) index
                    * (addrSpan + (ulong) max_threads - 1) / (ulong) max_threads;
                var end_addr = uAddrMin + ((ulong)index + 1)
                    * (addrSpan  + (ulong)max_threads - 1) / (ulong)max_threads;

                // Mask the address such that it's aligned to the 2^N offset.
                start_addr &= ~(((ulong)offset) - 1);
                if (end_addr >= uint.MaxValue)
                {
                    end_addr = uint.MaxValue;
                }
                else
                {
                    end_addr &= ~(((ulong)offset) - 1);
                }

                var interval = new Interval((uint)start_addr, (uint)end_addr);
                return interval;
            }
        }

        public List<BaseAddressCandidate> FindMatches(
            IReadOnlySet<ulong> strings,
            IReadOnlySet<ulong> pointers,
            int threadIndex,
            IProgressIndicator pb)
        {
            var interval = Interval.GetRange(this.MinAddress, threadIndex, Threads, Stride);
            trace.Inform("{0,3} Processing range: {1:X8}-{2:X8}",
                threadIndex,
                interval.BeginAddress,
                interval.EndAddress);
            var uBaseAddr = interval.BeginAddress;
            var heap = new List<BaseAddressCandidate>();
            var steps = (int)((interval.EndAddress - interval.BeginAddress) / Stride);
            pb.ShowProgress("Finding string pointers", 0, steps);
            var news = new HashSet<ulong>(strings.Count);
            var wordMask = Bits.Mask(0, arch.PointerType.BitSize);
            while (uBaseAddr <= interval.EndAddress)
            {
                news.Clear();
                foreach (var s in strings)
                {
                    if (!AddOverflow(s, uBaseAddr,  wordMask, out var addrRebased))
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
                pb.Advance(1);
            }
            pb.Finish();
            return heap;
        }

        private void DumpStrings(HashSet<ulong> strings)
        {
            foreach (ulong offset in strings)
            {
                Console.Write("{0:X8} ", offset);
                var i = (uint)offset;
                var bytes = Memory.Bytes;
                for (; i < bytes.Length; ++i)
                {
                    var b = bytes[i];
                    if (b == 0)
                    {
                        Console.WriteLine();
                        break;
                    }
                    if (' ' <= b && b <= '~')
                    {
                        Console.Write((char)b);
                    }
                    else
                    {
                        Console.Write("{0:X2}", (uint)b);
                    }
                }
            }
        }

        /*
#[cfg(test)]
    mod tests
    {
        use super::*;

#[test]
#[should_panic]
        fn find_matches_invalid_interval() {
            let _ = Interval::get_range(1, 1, 0x1000).unwrap();
        }

#[test]
        fn find_matches_single_cpu_interval_0() {
            let interval = Interval::get_range(0, 1, 0x1000).unwrap();
            assert_eq!(interval.start_addr, u32::min_value());
            assert_eq!(interval.end_addr, u32::max_value());
        }

#[test]
        fn find_matches_double_cpu_interval_0() {
            let interval = Interval::get_range(0, 2, 0x1000).unwrap();
            assert_eq!(interval.start_addr, u32::min_value());
            assert_eq!(interval.end_addr, 0x80000000);
        }

#[test]
        fn find_matches_double_cpu_interval_1() {
            let interval = Interval::get_range(1, 2, 0x1000).unwrap();
            assert_eq!(interval.start_addr, 0x80000000);
            assert_eq!(interval.end_addr, u32::max_value());
        }

#[test]
        fn find_matches_triple_cpu_interval_0() {
            let interval = Interval::get_range(0, 3, 0x1000).unwrap();
            assert_eq!(interval.start_addr, u32::min_value());
            assert_eq!(interval.end_addr, 0x55555000);
        }

#[test]
        fn find_matches_triple_cpu_interval_1() {
            let interval = Interval::get_range(1, 3, 0x1000).unwrap();
            assert_eq!(interval.start_addr, 0x55555000);
            assert_eq!(interval.end_addr, 0xAAAAA000);
        }

#[test]
        fn find_matches_triple_cpu_interval_2() {
            let interval = Interval::get_range(2, 3, 0x1000).unwrap();
            assert_eq!(interval.start_addr, 0xAAAAA000);
            assert_eq!(interval.end_addr, u32::max_value());
        }
    }
    */
    }
}
