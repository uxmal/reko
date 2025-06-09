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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Scanning
{
    /// <summary>
    /// Given a program, returns all addresses within the program that
    /// _appear_ to be strings.
    /// </summary>
    public class StringFinder
    {
        private readonly Program program;

        public StringFinder(Program program)
        {
            this.program = program;
        }

        public IEnumerable<AddressSearchHit> FindStrings(StringFinderCriteria criteria)
        {
            foreach (var rdr in GenerateReaders(program, criteria))
            {
                Address? addrStartRun = null;
                int cValid = 0;
                var charType = (PrimitiveType)criteria.StringType.ElementType;
                while (rdr.TryRead(charType, out var c))
                {
                    var ch = (char)c.ToInt32();
                    if (!IsValid(ch))
                    {
                        if (ch == 0 && cValid >= criteria.MinimumLength)
                        {
                            yield return new AddressSearchHit(program, addrStartRun!.Value, cValid * charType.Size);
                        }
                        addrStartRun = null;
                        cValid = 0;
                    }
                    else
                    {
                        if (addrStartRun is null)
                            addrStartRun = rdr.Address - charType.Size;
                        ++cValid;
                    }
                }
            }
        }

        private IEnumerable<EndianImageReader> GenerateReaders(Program program, StringFinderCriteria criteria)
        {
            if (criteria.Areas is null || criteria.Areas.Count == 0)
            {
                foreach (var segment in program.SegmentMap.Segments.Values)
                {
                    if (segment.MemoryArea is not ByteMemoryArea mem)
                        continue; //$TODO: what to do with odd archs?
                    Address segEnd = Address.Min(
                        segment.Address + segment.Size,
                        segment.MemoryArea.BaseAddress + mem.Bytes.Length);
                    var rdr = criteria.CreateReader(mem, segment.Address, segment.Size);
                    yield return rdr;
                }
            }
            else
            {
                foreach (var area in criteria.Areas)
                {
                    if (area.Program.SegmentMap.TryFindSegment(area.Address, out var segment) &&
                        segment.MemoryArea is ByteMemoryArea mem)
                    {
                        var rdr = criteria.CreateReader(mem, area.Address, area.Length);
                        yield return rdr;
                    }
                }
            }
        }


        //$TODO: This assumes only ASCII values are valid.
        // How to deal with Swedish? Cyrillic? Chinese?
        // Added common escaped characters for C strings.
        public static bool IsValid(char ch)
        {
            return ((ch >= ' ' && ch <= '~') || ( ch >= '\a' && ch <= '\r'));
        }
    }

    public record StringFinderCriteria(
        StringType StringType,
        Encoding Encoding,
        int MinimumLength,
        List<SearchArea>? Areas, // Optional search areas; null means search entire program image.
        Func<ByteMemoryArea, Address, long, EndianImageReader> CreateReader);
}
