#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Memory;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace Reko.Scanning
{
    // Based on: "Determining Image Base of Firmware Files for ARM Devices"
    // Ruijin ZHU, Yu-an TAN, Quanxin ZHANG, Fei WU, Jun ZHENG
    // Yuan XUE
    public class FetFinder : AbstractBaseAddressFinder
    {
        private const int MaxGap = 3;
        //$TODO: what about small machines? 0x200?
        private const uint AddrDistance = 0x1_0000;

        private readonly IProcessorArchitecture arch;
        private readonly ByteMemoryArea mem;
        private readonly ulong alignMask;
        private readonly ulong maskedValue;
        private readonly BigInteger wordMask;

        private uint word_size;
        private uint uAddrMax;
        private uint uAddrMin;


        public FetFinder(
            IProcessorArchitecture arch, 
            ByteMemoryArea mem,
            ulong alignMask, 
            ulong maskedValue)
            : base(arch.Endianness, mem)
        {
            this.arch = arch;
            this.mem = mem;
            this.alignMask = alignMask;
            this.maskedValue = maskedValue;
            this.Endianness = EndianServices.Little;
            this.wordMask = Bits.Mask(arch.PointerType.BitSize);
            this.word_size = (uint)(arch.WordWidth.BitSize / arch.MemoryGranularity);
        }

        public override BaseAddressCandidate[] Run(CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        private static bool IsNearby(Constant wAddrCandidate, Constant wPrev)
        {
            // instrs are within 64kb of each other
            var uCandidate = wAddrCandidate.ToUInt64();
            var uPrev = wPrev.ToUInt64();
            if (uCandidate > uPrev)
                return uCandidate - uPrev < AddrDistance;
            else
                return uPrev - uCandidate < AddrDistance;
        }

        private static readonly ulong[] fillerWords = new ulong[]
        {
            0ul,
            ~0ul,
            0xCCCCCCCC_CCCCCCCCul,
        };

        private bool IsFiller(Constant w, Constant? _)
        {
            var uw = w.ToUInt64();
            foreach (var filler in fillerWords)
                if (((uw ^ filler) & wordMask) == 0x0)
                    return true;
            return false;
        }

        public bool IsAligned(Constant addr)
        {
            var uAddr = addr.ToUInt64();
            return (uAddr & this.alignMask) == maskedValue;
        }


        public List<FET> FindFETs(uint start,  uint wnd)
        {
            var result = new List<FET>();
            uint pos = 0;
            uint end = (uint)mem.Length;
            while (start <= pos && pos < end)
            {
                for (uint gapWords = 0; gapWords < MaxGap; ++gapWords)
                {
                    var rdr = arch.CreateImageReader(mem, pos);
                    if (!read_word(rdr, out var w))
                        break;
                    if (!IsFiller(w, null) && IsAligned(w))
                    {
                        uint head = pos;
                        uint tableSize = wnd;
                        MoveWindow(rdr, pos, gapWords);
                        var wPrev = w;
                        while (rdr.Offset < end)
                        {
                            if (!read_word(rdr, out w))
                                break; ;
                            if (!IsNearby(w, wPrev) || IsFiller(w, wPrev) || !IsAligned(w))
                                break;
                            ++tableSize;
                            MoveWindow(rdr, pos, gapWords);
                        }
                        result.Add(new FET(head, gapWords, tableSize));
                    }
                }
                pos += word_size;
            }
            return result;
        }

        public record FET(uint head, uint gap, uint tableSize);

        private bool read_word(EndianImageReader rdr, [MaybeNullWhen(false)] out Constant ptrValue)
        {
            return rdr.TryRead(arch.PointerType, out ptrValue);
        }

        private void MoveWindow(EndianImageReader rdr, uint pos, uint gapWords)
        {
            // We've alread read a word, so advance by 
            // (gapWords - 1) words
            rdr.Offset = (pos + gapWords) * word_size;
        }

        public List<(uint, double)> FindBaseCandidates(byte[] bin, List<FET> function_entry_table, double threshold)
        {
            uint fileSize = (uint)bin.Length;
            var entryAddresses = function_entry_table.Select(fet => fet.head).Distinct().OrderBy(f => f).ToArray();
            var n = entryAddresses.Length;
            int thumbCount = 0;
            int armCount = 0;
            var candidates = new List<(uint, double)>();
            for (uint x = entryAddresses[^1] - fileSize; x >= entryAddresses[0]; --x)
            {
                foreach (uint entryAddress in entryAddresses)
                {
                    if ((entryAddress & 1) == 1)
                    {
                        if (bin[entryAddress - x] == 0xB5)
                            ++thumbCount;
                    } else if ((entryAddress - x + 2) == 0x2D &&
                               (entryAddress - x + 3) == 0xE9)
                    {
                        ++armCount;
                    }
                }
                double matchRate = (thumbCount + armCount) / (double)n;
                if (matchRate >= threshold)
                {
                    candidates.Add((x, matchRate));
                }
            }
            return candidates;
        }
    }
}