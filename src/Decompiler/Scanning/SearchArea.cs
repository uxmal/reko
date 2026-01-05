#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using Reko.Core.Loading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Scanning
{
    /// <summary>
    /// A search area is either a continuous address range or a named segment.
    /// </summary>
    public class SearchArea
    {
        private ImageSegment? segment;
        private AddressRange? addressRange;

        /// <summary>
        /// Constructs a new <see cref="SearchArea"/> from a continuous address range.
        /// </summary>
        /// <param name="program">Program being searched.</param>
        /// <param name="address">Start address.</param>
        /// <param name="length">Length of the search area.</param>
        /// <returns>A <see cref="SearchArea"/> instance.</returns>
        public static SearchArea FromAddressRange(Program program, Address address, long length)
        {
            return new SearchArea(program, null, new AddressRange(address, address + length));
        }

        /// <summary>
        /// Constructs a new <see cref="SearchArea"/> from the segment whose name
        /// is given by <paramref name="segmentName"/>.
        /// </summary>
        /// <param name="program">Program being searched.</param>
        /// <param name="segmentName">Name of the segment to search.</param>
        /// <returns>A <see cref="SearchArea"/> instance.</returns>
        public static SearchArea? FromSegment(Program program, string segmentName)
        {
            if (!program.SegmentMap.TryFindSegment(segmentName, out var segment))
                return null;
            return new SearchArea(program, segment, null);
        }

        /// <summary>
        /// Constructs a new <see cref="SearchArea"/> from an <see cref="ImageSegment"/>.
        /// </summary>
        /// <param name="program">Program being searched.</param>
        /// <param name="segment">Segment to search.</param>
        /// <returns>A <see cref="SearchArea"/> instance.</returns>
        public static SearchArea FromSegment(Program program, ImageSegment segment)
        {
            return new SearchArea(program, segment, null);
        }

        private SearchArea(Program program, ImageSegment? segment, AddressRange? addressRange)
        {
            if (segment is null) {
                if (addressRange is null) throw new ArgumentNullException(nameof(segment));
            }
            else
            {
                if (addressRange is not null) throw new ArgumentException();
            }

            this.Program = program;
            this.segment = segment;
            this.addressRange = addressRange;
        }

        /// <summary>
        /// Program being searched.
        /// </summary>
        public Program Program { get; }

        /// <summary>
        /// Start address of the search area.
        /// </summary>
        public Address Address => segment is not null ? segment.Address : addressRange!.Begin;

        /// <summary>
        /// Length of the search area in storage units.
        /// </summary>
        public long Length => segment is not null ? segment.Size : addressRange!.Length;


        private enum State
        {
            Start,
            InBeginRange,
            InEndRange,
            SegmentName,
        }

        /// <summary>
        /// Parse a string containing free-form search areas expressed as either
        /// address ranges or segment names.
        /// </summary>
        /// <param name="program">Program to search.</param>
        /// <param name="freeFormAreas">Search areas expressed as a string.</param>
        /// <param name="result">Resulting list of <see cref="SearchArea"/>s if the
        /// <paramref name="freeFormAreas"/> were syntactically correct; otherwise nothing.
        /// </param>
        /// <returns>True if the <paramref name="freeFormAreas"/> were provided syntactially corretly;
        /// otherwise false.
        /// </returns>
        public static bool TryParse(Program program, string freeFormAreas, [MaybeNullWhen(false)] out List<SearchArea> result)
        {
            var arch = program.Architecture;
            var i = 0;
            var iBegin = -1;
            result = null;
            Address? addrBegin = null;
            State state = State.Start;
            string segName;
            SearchArea? area;
            var areas = new List<SearchArea>();
            for (; ; )
            {
                i = SkipWs(freeFormAreas, i);
                if (i >= freeFormAreas.Length)
                    break;
                char c = freeFormAreas[i++];
                switch (state)
                {
                case State.Start:
                    switch (c)
                    {
                    case '[':
                        state = State.InBeginRange;
                        iBegin = i;
                        break;
                    case ',':
                        break;
                    default:
                        // Assume it's a segment name
                        state = State.SegmentName;
                        iBegin = i - 1;
                        break;
                    }
                    break;
                case State.InBeginRange:
                    switch (c)
                    {
                    case '-':
                        if (!TryParseAddress(arch, freeFormAreas, iBegin, i - 1, out var a))
                            return false;
                        addrBegin = a;
                        state = State.InEndRange;
                        iBegin = i;
                        break;
                    default:
                        break;
                    }
                    break;
                case State.InEndRange:
                    switch (c)
                    {
                    case ']':
                        int adjustment = 1;
                        Debug.Assert(addrBegin is not null);
                        if (!TryParseAddress(arch, freeFormAreas, iBegin, i - 1, out var addrEnd))
                            return false;
                        if (addrBegin >= addrEnd)
                            return false;
                        areas.Add(new SearchArea(program, null, new AddressRange(
                            addrBegin.Value,
                            (addrEnd + adjustment))));
                        state = State.Start;
                        break;
                    case ')':
                        if (!TryParseAddress(arch, freeFormAreas, iBegin, i - 1, out addrEnd))
                            return false;
                        Debug.Assert(addrBegin.HasValue);
                        if (addrBegin! >= addrEnd)
                            return false;
                        areas.Add(new SearchArea(program, null, new AddressRange(
                            addrBegin.Value,
                            addrEnd)));
                        break;
                    default:
                        break;
                    }
                    break;
                case State.SegmentName:
                    switch (c)
                    {
                    case ',':
                        segName = freeFormAreas.Substring(iBegin, i - iBegin);
                        area = SearchArea.FromSegment(program, segName);
                        if (area is not null)
                        {
                            areas.Add(area);
                        }
                        state = State.Start;
                        break;
                    }
                    break;
                }
            }
            if (state == State.Start)
            {
                result = areas;
                return true;
            }
            if (state == State.SegmentName)
            {
                segName = freeFormAreas.Substring(iBegin, i - iBegin);
                area = SearchArea.FromSegment(program, segName);
                if (area is not null)
                {
                    areas.Add(area);
                }
                result = areas;
                return true;
            }
            return false;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (obj is not SearchArea that)
                return false;
            if (this == that)
                return true;
            return this.Address == that.Address && 
                this.Length == that.Length;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(this.Address, this.Length);
        }

        private static int SkipWs(string str, int i)
        {
            for (; i < str.Length; ++i)
            {
                char c = str[i];
                if (!char.IsWhiteSpace(c))
                    return i;
            }
            return i;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            var sb = new StringBuilder();
            if (this.segment is not null)
                return segment.Name;
            else
            {
                Debug.Assert(addressRange is not null);
                return $"[{addressRange.Begin}-{addressRange.Begin + (addressRange.Length - 1)}]";
            }
        }

        private static bool TryParseAddress(IProcessorArchitecture arch, string freeFormAreas, int iBegin, int iEnd, [MaybeNullWhen(false)] out Address addr)
        {
            return arch.TryParseAddress(
                freeFormAreas.Substring(iBegin, iEnd - iBegin),
                out addr);
        }

        /// <summary>
        /// Formats a list of search areas as a string. The string can be parsed 
        /// in <see cref="TryParse(Program, string, out List{SearchArea})"/>.
        /// </summary>
        /// <param name="searchAreas">List of <see cref="SearchArea"/>s to format.</param>
        /// <returns>A parseable string.</returns>
        public static string Format(List<SearchArea>? searchAreas)
        {
            if (searchAreas is null || searchAreas.Count == 0)
                return "";
            return string.Join(", ", searchAreas);
        }

    }
}
