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
using Reko.Core.Memory;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace Reko.Scanning
{
    public class FindBaseString : IBaseAddressFinder
    {
        private readonly ByteMemoryArea mem;
        private uint _stride;

        public FindBaseString(ByteMemoryArea mem)
        {
            this.mem = mem;
            this.Endianness = EndianServices.Little;
            Stride = 0x1000;
            Threads = Environment.ProcessorCount;
        }

        public EndianServices Endianness { get; set; }   // Interpret as big-endian (default is little)' 
        public int min_str_len = 10;           // Minimum string search length (default is 10)'
        public int max_matches = 10;           // Maximum matches to display (default is 10)'

        /// <summary>
        /// Scan every N (power of 2) addresses. (default is 0x1000)'
        /// </summary>
        public uint Stride
        {
            get => _stride;
            set
            {
                if (BitOperations.PopCount(value) != 1)
                    throw new ArgumentException("Value must be a power of 2.");
                _stride = value;
            }
        }

        public bool ShowProgress { get; set; }              // Show progress

        /// <summary>
        /// Number of threads to spawn. (default is # of cpu cores)'",
        /// </summary>
        public int Threads { get; set; }


        public Task Run()
        {
            // Find indices of strings.
            var strings = PatternFinder.FindAsciiStrings(mem, min_str_len)
                .Select(s => s.uAddress)
                .ToHashSet();
            //DumpStrings(strings);

            if (strings.Count == 0)
            {
                throw new Exception("No strings found in target binary.");
            }
            Console.WriteLine("Located {0} strings", strings.Count);

            var pointers = ReadPointers(mem, 4);
            Console.WriteLine("Located {0} pointers", pointers.Count);

            var children = new List<Task<List<(int, ulong)>>>();
            var shared_strings = strings;
            var shared_pointers = pointers;

            var mb = new MultiBar(ShowProgress
                        ? new Progress<int>(Console.Error)
                        : new NullProgress<int>());

            Debug.WriteLine("Scanning with {0} Threads...", Threads);
            for (int i = 0; i < this.Threads; ++i)
            {
                var pb = mb.create_bar(100);
                pb.ShowMessage = true;
                pb.MaxRefreshRate = 100;
            }
            var semaphore = new CountdownEvent(Threads);
            ConcurrentQueue<List<(int, ulong)>> queue = new();
#if RAW_THREADS
            var threads = new List<Thread>();
            for (int i = 0; i < this.Threads; ++i)
            {
                int n = i;
                var t = new Thread(() =>
                {
                    var result = FindMatches(strings, pointers, n, new NullProgress<int>());
                    //       Console.WriteLine("{0,3} Thread completed, {1} matches", n, result.Count);
                    queue.Enqueue(result);
                    semaphore.Signal();
                });
                threads.Add(t);
            }
            var sw = new Stopwatch();
            sw.Start();
            foreach (var t in threads)
            {
                t.Start();
            }
#else
            var sw = new Stopwatch();
            sw.Start();
            Parallel.For(0, this.Threads, (n) =>
            {
                var result = FindMatches(strings, pointers, n, new NullProgress<int>());
                //Console.WriteLine("{0,3} Thread completed, {1} matches", n, result.Count);
                queue.Enqueue(result);
                semaphore.Signal();
            });
#endif
            semaphore.Wait();

            mb.listen();

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


        public struct Interval
        {
            public ulong start_addr;
            public ulong end_addr;

            public Interval(uint start_addr, uint end_addr)
            {
                this.start_addr = start_addr;
                this.end_addr = end_addr;
            }

            public static Interval GetRange(
                nint index,
                nint max_threads,
                uint offset)
            { // -> Result<Interval, Box<dyn Error + Send + Sync>> {
                if (index >= max_threads)
                {
                    throw new ArgumentException("Invalid index specified.");
                }

                if (BitOperations.PopCount(offset) != 1)
                    throw new ArgumentException("Invalid additive offset.");

                var start_addr = (ulong)index
                    * ((ulong)(uint.MaxValue) + (ulong)max_threads - 1) / (ulong)max_threads;
                var end_addr = ((ulong)index + 1)
                    * ((ulong)(uint.MaxValue) + (ulong)max_threads - 1) / (ulong)max_threads;

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

        public List<(int, ulong)> FindMatches(
            IReadOnlySet<ulong> strings,
            IReadOnlySet<ulong> pointers,
            int threadIndex,
            ProgressBar pb)
        {
            var interval = Interval.GetRange(threadIndex, Threads, _stride);
            //Console.WriteLine("Thread {0} in range: {1:X8}-{2:X8}",
            //    threadIndex,
            //    interval.start_addr,
            //    interval.end_addr);
            var uBaseAddr = interval.start_addr;
            var heap = new List<(int, ulong)>();
            pb.Total = ((interval.end_addr - interval.start_addr) / _stride);
            var news = new HashSet<ulong>(strings.Count);
            while (uBaseAddr <= interval.end_addr)
            {
                //if ((uBaseAddr & 0x7FF_FFF0) == 0)
                //{
                //    Console.WriteLine($"{threadIndex,3} 0x{uBaseAddr:X8}");
                //}
                news.Clear();
                foreach (var s in strings)
                {
                    if (!AddOverflow(s, uBaseAddr, out var addrRebased))
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
                if (AddOverflow(uBaseAddr, Stride, out var new_addr))
                {
                    Console.WriteLine($"{threadIndex,3} Ending at {uBaseAddr:X8}, _stride = 0x{Stride}");
                    break;
                }
                uBaseAddr = new_addr;
                pb.inc();
            }
            pb.finish();
            return heap;
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



        private void DumpStrings(HashSet<ulong> strings)
        {
            foreach (ulong offset in strings)
            {
                Console.Write("{0:X8} ", offset);
                var i = (uint)offset;
                for (; i < mem.Bytes.Length; ++i)
                {
                    var b = mem.Bytes[i];
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

        private interface IProgress<T>
        { }

        private class Progress<T> : IProgress<T>
        {
            private TextWriter error;

            public Progress(TextWriter stm)
            {
                this.error = stm;
            }
        }

        private class NullProgress<T> : IProgress<T>, ProgressBar
        {
            public NullProgress()
            {
            }

            public ulong Total { get; set; }
            public bool ShowMessage { get; set; }

            public void finish()
            {
            }

            public void inc()
            {
            }

            public int MaxRefreshRate { get; set; }
        }


        private class MultiBar
        {
            private IProgress<int> progress;

            public MultiBar(IProgress<int> nullProgress)
            {
                this.progress = nullProgress;
            }

            internal ProgressBar create_bar(int v)
            {
                return new NullProgress<int>();
            }

            internal void listen()
            {
            }
        }

        public interface ProgressBar
        {
            ulong Total { get; set; }
            bool ShowMessage { get; set; }
            int MaxRefreshRate { get; set; }

            void inc();
            void finish();
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
