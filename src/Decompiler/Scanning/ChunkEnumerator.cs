#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
 .
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

namespace Reko.Scanning;

using Reko.Core;
using Reko.Core.Collections;
using Reko.Core.Loading;
using Reko.Core.Memory;
using System;
using System.Collections.Generic;
using System.Diagnostics;

/// <summary>
/// This class enumerates all known <see cref="RtlBlock"/>s and discovers
/// any gaps between blocks, while respecting segment boundaries.
/// </summary>
public class ChunkEnumerator
{

    /// <summary>
    /// Given a sorted sequence of <see cref="ImageSegment"/>s and a sorted
    /// set of <see cref="RtlBlock"/>s, returns a sequence of <see cref="Fragments"/>,
    /// some of which are gaps between blocks.
    /// </summary>
    /// <param name="sortedSegments">A sorted sequence of <see cref="ImageSegment"/>s.</param>
    /// <param name="sortedBlocks"></param>
    /// <param name="dataBlocks">Data blocks that should be excluded from the result.
    /// The routine assumes the data blocks are sorted by increasing addresses.
    /// </param>
    /// <returns>A sequence of </returns>
    public IEnumerable<Chunk> EnumerateFragments(
        IEnumerable<ImageSegment> sortedSegments,
        IEnumerable<RtlBlock> sortedBlocks,
        IEnumerable<ImageMapItem> dataBlocks)
    {
        var fragments = ResolveOverlaps(sortedBlocks);
        var gapFragments = GenerateGapFragments(sortedSegments, fragments);
        var result = ExcludeDataBlocks(gapFragments, dataBlocks);
        return result;
    }

    /// <summary>
    /// Given a sequence of <see cref="Fragments"/>, generate "gap" fragments. 
    /// </summary>
    /// <param name="segments">A sorted sequence of <see cref="ImageSegment">s.</param>
    /// <param name="fragments">A sorted sequence of non-overlapping <see cref="Chunk"/>.
    /// </param>
    /// <returns></returns>
    private static IEnumerable<Chunk> GenerateGapFragments(
        IEnumerable<ImageSegment> segments,
        IEnumerable<Chunk> fragments)
    {
        var eFragments = new LookaheadEnumerator<Chunk>(fragments);
        foreach (var segment in segments)
        {
            Address addrLast = segment.Address;
            while (eFragments.TryPeek(1, out var fragment))
            {
                if (fragment.Address < segment.Address)
                    continue;
                if (fragment.Address - segment.Address >= segment.Size)
                    break;
                eFragments.MoveNext();

                if (addrLast < fragment.Address)
                {
                    yield return new Chunk(null, segment.MemoryArea, addrLast, fragment.Address - addrLast);
                }
                yield return fragment;
                addrLast = Align(fragment.Address + fragment.Length, fragment.Architecture);
            }

            // Rest of segment.
            long cbRemaining = segment.Size - (addrLast - segment.Address);
            if (cbRemaining > 0)
                yield return new Chunk(null, segment.MemoryArea, addrLast, cbRemaining);
        }
    }

    private IEnumerable<Chunk> ExcludeDataBlocks(IEnumerable<Chunk> gapFragments, IEnumerable<ImageMapItem> dataBlocks)
    {
        //$TODO: remove data blocks from list of fragments.
        return gapFragments;
    }

    private static Address Align(Address address, IProcessorArchitecture? arch)
    {
        if (arch is null)
            return address;
        var alignment = (uint)(arch.InstructionBitSize / arch.CodeMemoryGranularity);
        var offset = alignment * ((address.Offset + (alignment - 1)) / alignment);
        return address.NewOffset(offset);
    }

    /// <summary>
    /// Given a sequence of <see cref="RtlBlock"/>s, yields a sequence of <see cref="Chunk"/>s
    /// whose ending addresses increases monotonically.
    /// </summary>
    /// <param name="sortedBlocks"></param>
    /// <returns>A sequence of <see cref="Fragments"/> where no fragments overlap.</returns>
    public IEnumerable<Chunk> ResolveOverlaps(IEnumerable<RtlBlock> sortedBlocks)
    {
        var e = sortedBlocks.GetEnumerator();
        if (!e.MoveNext())
            yield break;
        var fragmentPrev = MakeFragment(e.Current, null!);
        while (e.MoveNext())
        {
            var fragment = MakeFragment(e.Current, null!);
            Debug.Assert(fragmentPrev.Address < fragment.Address);
            var cbSeparation = fragment.Address - fragmentPrev.Address;
            var cbGap = cbSeparation - fragmentPrev.Length;
            if (cbGap > 0)
            {
                // No overlap between the previous fragment and this one.
                yield return fragmentPrev;
                fragmentPrev = fragment;
                continue;
            }
            var cbOverlap = cbSeparation + (fragment.Length - fragmentPrev.Length);
            if (cbOverlap >= 0)
            {
                // Truncate the previous fragment.
                yield return new(fragmentPrev.Architecture, fragmentPrev.MemoryArea, fragmentPrev.Address, cbSeparation);
                fragmentPrev = fragment;
            }
        }
        yield return fragmentPrev;
    }

    private static Chunk MakeFragment(RtlBlock block, MemoryArea mem)
    {
        return new Chunk(block.Architecture, mem, block.Address, block.Length);
    }
}