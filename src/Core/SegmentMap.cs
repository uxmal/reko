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

using Reko.Core.Collections;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Reko.Core
{
    /// <summary>
    /// Describes the address space of a loaded executable.
    /// </summary>
    public class SegmentMap : IReadOnlySegmentMap
    {
        /// <summary>
        /// Event that is raised when the segment map changes.
        /// </summary>
        public event EventHandler? MapChanged;

        /// <summary>
        /// Constructs an segment map with the given segments. The <see cref="BaseAddress"/>
        /// is initialized to the minimium address of all segments.
        /// </summary>
        /// <param name="segments">Image segments to add.
        /// </param>
        public SegmentMap(params ImageSegment[] segments) : this(MinBaseAddr(segments), segments)
        {
        }

        /// <summary>
        /// Computes the base address as the minimum address of all segments.
        /// </summary>
        /// <param name="segments">At least one image segment.</param>
        /// <returns>The minimum starting address of all segments.</returns>
        private static Address MinBaseAddr(ImageSegment[] segments)
        {
            if (segments.Length == 0)
                throw new ArgumentException("At least one ImageSegment must be provided.", nameof(segments));
            var addr = segments[0].Address;
            for (int i = 1; i < segments.Length; ++i)
            {
                addr = Address.Min(addr, segments[i].Address);
            }
            return addr;
        }

        /// <summary>
        /// Creates an instance of the <see cref="SegmentMap"/> class.
        /// </summary>
        /// <param name="addrBase">Base address of the segment map.</param>
        /// <param name="segments">The image segments constituting the segment map.
        /// </param>
        public SegmentMap(Address addrBase, params ImageSegment[] segments)
        {
            this.BaseAddress = addrBase;
            this.Segments = new SortedList<Address, ImageSegment>();
            this.SegmentByLinAddress = new SortedList<ulong, ImageSegment>();
            this.Selectors = new Dictionary<ushort, ImageSegment>();
            foreach (var seg in segments)
            {
                this.AddSegment(seg);
            }
        }

        /// <summary>
        /// The base address of the segment map.
        /// </summary>
        public Address BaseAddress { get; }

        /// <summary>
        /// The segments in the segment map, sorted by their starting address.
        /// </summary>
        public SortedList<Address, ImageSegment> Segments { get; }

        private SortedList<ulong, ImageSegment> SegmentByLinAddress { get; }

        public Dictionary<ushort, ImageSegment> Selectors { get; }

        IReadOnlyDictionary<ushort, ImageSegment> IReadOnlySegmentMap.Selectors => this.Selectors;

        /// <summary>
        /// Creates an <see cref="EndianImageReader"/> that starts reading at the <see cref="Address"/>
        /// <paramref name="address"/>. The endianness of the image reader is controlled by the
        /// <paramref name="arch"/>. 
        /// </summary>
        /// <param name="address">Address at which to start reading.</param>
        /// <param name="arch"><see cref="IProcessorArchitecture"/> that determines the 
        /// endianness, byte granularity and other processor-specific details.
        /// </param>
        /// <returns>The resulting <see cref="EndianImageReader"/> instance.</returns>
        public EndianImageReader CreateImageReader(Address address, IProcessorArchitecture arch)
        {
            if (!TryFindSegment(address, out var segment))
            {
                throw new ArgumentException($"Address {address} not found in program segments.");
            }
            var rdr = segment.CreateImageReader(arch);
            rdr.Seek(address - segment.Address);
            return rdr;
        }

        /// <summary>
        /// Adds a segment to the segment map. The segment is assumed to be disjoint
        /// from other segments already present, so the <paramref name="mem"/> memory
        /// area is not shared with other segments.
        /// </summary>
        /// <param name="mem">The memory area corresponding to the segment.</param>
        /// <param name="segmentName">The name of the segment.</param>
        /// <param name="mode">The access mode of the segment.</param>
        /// <returns>The resulting image segment.</returns>
        public ImageSegment AddSegment(MemoryArea mem, string segmentName, AccessMode mode)
        {
            var segment = new ImageSegment(
                    segmentName,
                    mem,
                    mode);
            AddSegment(segment);
            return segment;
        }

        /// <summary>
        /// Adds a potentially overlapping segment to the segment map. The segment
        /// extends over the shared memory area <paramref name="mem"/>, and starts at
        /// address <paramref name="addr" />.
        /// </summary>
        /// <param name="segmentName">The name of the segment.</param>
        /// <param name="mem">The shared memory area the segment overlaps.</param>
        /// <param name="addr">The address at which the segment starts.</param>
        /// <param name="mode">The access mode of the segment.</param>
        /// <returns>The resulting image segment.</returns>
        public ImageSegment AddOverlappingSegment(string segmentName, MemoryArea mem, Address addr, AccessMode mode)
        {
            var segment = new ImageSegment(
                    segmentName,
                    addr,
                    mem,
                    mode);
            AddSegment(segment);
            return segment;
        }

        public ImageSegment AddSegment(ImageSegment segNew)
        {
            if (!TryFindSegment(segNew.Address, out ImageSegment? seg))
            {
                EnsureSegmentSize(segNew);
                Segments.Add(segNew.Address, segNew);
                SegmentByLinAddress.Add(segNew.Address.ToLinear(), segNew);
                MapChanged?.Invoke(this, EventArgs.Empty);
                //DumpSections();
                return segNew;
            }
            long delta = segNew.Address - seg.Address;
            Debug.Assert(delta >= 0);
            if (delta > 0)
            {
                // Need to split the segment if it has a size
                // x86 real mode segments don't have sizes, and can overlap.

                var segSplit = new ImageSegment(segNew.Name, segNew.Address, segNew.MemoryArea, segNew.Access);
                segSplit.Size = (uint)(seg.Size - delta);
                seg.Size = (uint)delta;
                Segments.Add(segNew.Address, segSplit);
                SegmentByLinAddress.Add(segNew.Address.ToLinear(), segSplit);

                // And split any items in the segment

                MapChanged?.Invoke(this, EventArgs.Empty);
                //DumpSections();
                return segSplit;
            }
            return seg;
        }

        private void EnsureSegmentSize(ImageSegment seg)
        {
            if (seg.Size == 0)
            {
                if (!Segments.TryGetUpperBoundKey(seg.Address, out var addrAbove))
                {
                    // No segment above this one, consume all remaining space.
                    seg.Size = (uint)((seg.MemoryArea.BaseAddress - seg.Address) + seg.MemoryArea.Length);
                }
                else
                {
                    seg.Size = (uint)(addrAbove - seg.Address);
                }
            }
        }

        public long GetExtent()
        {
            if (Segments.Count == 0)
                return 0;
            var lastMem = Segments.Values.Last().MemoryArea;
            return (lastMem.BaseAddress - BaseAddress) + lastMem.Length;
        }

        public bool IsValidAddress(Address address)
        {
            return TryFindSegment(address, out var _);
        }

        public bool IsReadOnlyAddress(Address addr)
        {
            return TryFindSegment(addr, out var seg) && seg.IsWriteable;
        }

        public bool IsExecutableAddress(Address addr)
        {
            return TryFindSegment(addr, out var seg) && seg.IsExecutable;
        }

        /// <summary>
        /// Given a linear address, returns an address whose selector, if any,
        /// contains the linear address.
        /// </summary>
        /// <param name="linearAddress"></param>
        /// <returns></returns>
		public Address MapLinearAddressToAddress(ulong linearAddress)
        {
            //$REVIEW: slow; use binary search at least?
            foreach (ImageSegment seg in Segments.Values)
            {
                if (seg.IsInRange(linearAddress))
                {
                    long offset = (long)linearAddress - (long)seg.Address.ToLinear();
                    return seg.Address + offset;
                }
            }
            throw new ArgumentOutOfRangeException(
                string.Format("Linear address {0:X8} is not in known segment.",
                linearAddress));
        }

        /// <summary>
        /// Creates an <see cref="EndianImageReader"/> that starts reading at the <see cref="Address"/>
        /// <paramref name="address"/>. The endianness of the image reader is controlled by the
        /// <paramref name="arch"/>. 
        /// </summary>
        /// <param name="address">Address at which to start reading.</param>
        /// <param name="arch"><see cref="IProcessorArchitecture"/> that determines the 
        /// endianness, byte granularity and other processor-specific details.
        /// </param>
        /// <param name="rdr">The resulting <see cref="EndianImageReader"/> instance.</param>
        /// <returns>True if an image reader could be created at the requested address,
        /// otherwise false.
        /// </returns>
        public bool TryCreateImageReader(
            Address address,
            IProcessorArchitecture arch,
            [MaybeNullWhen(false)] out EndianImageReader rdr)
        {
            if (!TryFindSegment(address, out var segment))
            {
                rdr = null;
                return false;
            }
            rdr = segment.CreateImageReader(arch);
            rdr.Seek(address - segment.Address);
            return true;
        }

        /// <summary>
        /// Returns the segment that contains the specified address.
        /// </summary>
        public bool TryFindSegment(Address addr, [MaybeNullWhen(false)] out ImageSegment segment)
        {
            if (!Segments.TryGetLowerBound(addr, out segment))
                return false;
            if (segment.Address.ToLinear() == addr.ToLinear())
                return true;
            return segment.IsInRange(addr);
        }

        /// <summary>
        /// Returns the segment that contains the specified linear address.
        /// </summary>
        public bool TryFindSegment(ulong linAddress, [MaybeNullWhen(false)] out ImageSegment segment)
        {
            if (!this.SegmentByLinAddress.TryGetLowerBound(linAddress, out segment))
                return false;
            if (segment.Address.ToLinear() == linAddress)
                return true;
            return segment.IsInRange(linAddress);
        }

        /// <summary>
        /// Returns the segment named <paramref name="segmentName"/>.
        /// </summary>
        /// <param name="segmentName">Name of the <see cref="ImageSegment"/> being sought.
        /// </param>
        /// <param name="segment">The <see cref="ImageSegment"/> of that name, if present.</param>
        /// <returns>True if a segment with that name exists, false if not.
        /// </returns>
        public bool TryFindSegment(
            string segmentName, 
            [MaybeNullWhen(false)] out ImageSegment segment)
        {
            segment = this.Segments.Values.FirstOrDefault(s => s.Name == segmentName);
            return segment is not null;
        }

        [Conditional("DEBUG")]
        public void DumpSections()
        {
            foreach (var item in Segments)
            {
                Debug.Print("Key: {0}, Value: name:{1,-18} size: {2:X8}, Access: {3}", item.Key, item.Value.Name, item.Value.Size, item.Value.Access);
            }
        }

        /// <summary>
        /// Creates an <see cref="ImageMap"/> based on this <see cref="SegmentMap"/>.
        /// </summary>
        /// <returns>A new <see cref="ImageMap"/> instance.
        /// </returns>
        public ImageMap CreateImageMap()
        {
            var imageMap = new ImageMap(this.BaseAddress);
            foreach (var segment in Segments.Values)
            {
                imageMap.AddItem(segment.Address, new ImageMapItem(segment.Address, segment.Size) { DataType = new UnknownType() });
            }
            return imageMap;
        }
    }
}
