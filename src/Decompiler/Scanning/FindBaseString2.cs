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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// https://gist.github.com/rpw/2c4064712638bce602755a938991e5e9
namespace Reko.Scanning
{
    /// <summary>
    /// Determine load addresses using differences between string references.
    /// </summary>
    public class FindBaseString2 : AbstractBaseAddressFinder
    {
        private const int ATTEMPTS = 10;

        private int min_str_length = 10;

        private Func<ulong, ulong> read_dword;

        public FindBaseString2(IProcessorArchitecture arch, ByteMemoryArea mem)
            : base(arch.Endianness, mem)
        {
            this.read_dword = ReadDwordLe;
            this.Endianness = EndianServices.Little;
        }

        public override BaseAddressCandidate[] Run()
        {
            this.read_dword = Endianness == EndianServices.Big
                ? ReadDwordBe
                : ReadDwordLe;
            var result = this.FindBaseAddresses();
            foreach (var uAddr in result)
            {
                Console.WriteLine($"0x{uAddr:X8}");
            }
            return result
                .Select(a => new BaseAddressCandidate(a, 1))
                .ToArray();
        }

        private ulong ReadDwordLe(ulong offset)
        {
            if (Memory.TryReadLeUInt32((long)offset, out var value))
                return value;
            else
                return 0;
        }

        private ulong ReadDwordBe(ulong offset)
        {
            if (Memory.TryReadBeUInt32((long)offset, out var value))
                return value;
            else
                return 0;
        }

        public ulong[] MemoryWordsAsSortedList(uint start, uint end)
        {
            Console.WriteLine("Gathering dwords in memory. This may take a while...");
            var values = new HashSet<ulong>();
            var rdr = Endianness.CreateImageReader(Memory, start);
            while (rdr.Offset < end)
            {
                if (!rdr.TryReadUInt32(out uint w))
                    break;
                values.Add(w);
            }
            var sortedValues = values.ToArray();
            Array.Sort(sortedValues);
            return sortedValues;
        }

        public long[] ComputeDifferences(ulong[] values)
        {
            long[] differences = new long[values.Length];
            for (int i = 0; i < values.Length - 1; ++i)
            {
                differences[i] = (long)values[i + 1] - (long)values[i];
            }
            return differences;
        }

        // Knuth Morris Pratt algorithm to find all subsequences
        // https://www.safaribooksonline.com/library/view/python-cookbook-2nd/0596007973/ch05s14.html
        public IEnumerable<int> KMP(long[] text, long[] pattern) {
            // ensure we can index into pattern, and also make a copy to protect
            // against changes to 'pattern' while we're suspended by `yield'
            //pattern = list(pattern)
            var length = pattern.Length;
            // build the KMP "table of shift amounts" and name it 'shifts'
            var shifts = new int[length + 1];
            Array.Fill(shifts, 1);
            int shift = 1;
            foreach (var (pat, pos) in pattern.Select((pat, pos) => (pat, pos))) {
                while (shift <= pos && pat != pattern[pos - shift]) {
                    shift += shifts[pos - shift];
                    shifts[pos + 1] = shift;
                }
            }
            // perform the actual search
            var startPos = 0;
            var matchLen = 0;
            foreach (var c in text) {
                while (matchLen == length || matchLen >= 0 && pattern[matchLen] != c) {
                    startPos += shifts[matchLen];
                    matchLen -= shifts[matchLen];
                    matchLen += 1;
                    if (matchLen == length)
                        yield return startPos;
                }
            }
        }

        public List<ulong> IdentifyBaseAddresses(ulong[] values, ulong[] strs)
        {
            var string_differences = ComputeDifferences(strs);
            var differences = ComputeDifferences(values);

            foreach (var v in string_differences)
            {
                if (v <= 0)
                {
                    Console.Write("Invalid sequence of strings. Memory addresses have to be in strictly increasing order.");
                    return new List<ulong>();
                }
            }

            var matches = KMP(differences, string_differences);
            var baseaddrs = new List<ulong>();

            foreach (var pos in matches)
            {
                baseaddrs.Add(values[pos] - strs[0]);
            }
            return baseaddrs;
        }

        public ulong[] FindAdjacentStrings(List<(ulong uAddress, uint length)> strs) {
            const int nSamples = 6;

            var rng = new Random();
            for (var attempt = 0; attempt < ATTEMPTS; ++attempt)
            {
                var start_off = rng.Next(strs.Count - nSamples);
                var matches = new List<ulong>();
                Console.WriteLine("Looking for adjacent strings (attempt {0} at offset {1})", attempt + 1, start_off);
                for (var i = start_off; i < strs.Count - 1; ++i)
                {
                    if (matches.Count == nSamples)
                        return matches.ToArray();
                    if (strs[i].uAddress + strs[i].length + 1 == strs[i + 1].uAddress)
                        matches.Add(strs[i].uAddress);
                    else
                        matches.Clear();
                }
            }

            Console.Write("No adjacent strings found...");
            return Array.Empty<ulong>();
        }
        public List<ulong> FindBaseAddresses()
        {
            var strings = PatternFinder.FindAsciiStrings(Memory, min_str_length);
            var values = MemoryWordsAsSortedList(0, (uint)Memory.Length);

            var baseaddrs = new List<ulong>();
            for (var attempt = 0; attempt < ATTEMPTS; ++attempt)
            {
                // lists of start addresses of strings that are consecutively placed in memory
                var stringaddrs = FindAdjacentStrings(strings);
                if (stringaddrs.Length == 0)
                    continue;
                Console.WriteLine("using adjacent strings {0}",
                    string.Join(",", stringaddrs.Select(s => $"0x{s:X8}")));

                baseaddrs = IdentifyBaseAddresses(values, stringaddrs);
                if (baseaddrs.Count > 0)
                    break;
            }
            return baseaddrs;
        }
    }
}
