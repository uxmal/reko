#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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

using Reko.Core.Lib;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.Core
{
    public class SegmentMap
    {
        public event EventHandler MapChanged;

        private SortedList<Address, ImageSegment> segments;

        public SegmentMap(Address addrBase, params ImageSegment[] segments)
        {
            if (addrBase == null)
                throw new ArgumentNullException("addrBase");
            this.BaseAddress = addrBase;
            this.segments = new SortedList<Address, ImageSegment>();
            foreach (var seg in segments)
            {
                this.AddSegment(seg);
            }
        }

        public Address BaseAddress { get; private set; }

        public SortedList<Address, ImageSegment> Segments
        {
            get { return segments; }
        }

        public ImageSegment AddSegment(MemoryArea mem, string segmentName, AccessMode mode)
        {
            var segment = new ImageSegment(
                    segmentName,
                    mem,
                    mode);
            AddSegment(segment);
            return segment;
        }

        public ImageSegment AddSegment(Address addr, string segmentName, AccessMode access, uint contentSize)
        {
            return AddSegment(new ImageSegment(segmentName, addr, contentSize, access));
        }

        public ImageSegment AddSegment(ImageSegment segNew)
        {
            ImageSegment seg;
            if (!TryFindSegment(segNew.Address, out seg))
            {
                EnsureSegmentSize(segNew);
                segments.Add(segNew.Address, segNew);
                MapChanged.Fire(this);
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
                segments.Add(segNew.Address, segSplit);

                // And split any items in the segment

                MapChanged.Fire(this);
                //DumpSections();
                return segSplit;
            }
            return seg;
        }

        private void EnsureSegmentSize(ImageSegment seg)
        {
            Address addrAbove;
            if (seg.Size == 0)
            {
                if (!Segments.TryGetUpperBoundKey(seg.Address, out addrAbove))
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
            if (segments.Count == 0)
                return 0;
            var lastMem = segments.Values.Last().MemoryArea;
            return (lastMem.BaseAddress - BaseAddress) + lastMem.Length;
        }

        public bool IsValidAddress(Address address)
        {
            ImageSegment seg;
            return TryFindSegment(address, out seg);
        }

        public bool IsReadOnlyAddress(Address addr)
        {
            ImageSegment seg;
            return (TryFindSegment(addr, out seg) && (seg.Access & AccessMode.Write) == 0);
        }

        public bool IsExecutableAddress(Address addr)
        {
            ImageSegment seg;
            return (TryFindSegment(addr, out seg) && (seg.Access & AccessMode.Execute) != 0);
        }

        /// <summary>
        /// Given a linear address, returns an address whose selector, if any, contains the linear address.
        /// </summary>
        /// <param name="linearAddress"></param>
        /// <returns></returns>
		public Address MapLinearAddressToAddress(ulong linearAddress)
        {
            //$REVIEW: slow; use binary search at least?
            foreach (ImageSegment seg in segments.Values)
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
        /// Returns the segment that contains the specified address.
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        public bool TryFindSegment(Address addr, out ImageSegment segment)
        {
            if (!segments.TryGetLowerBound(addr, out segment))
                return false;
            if (segment.Address.ToLinear() == addr.ToLinear())
                return true;
            return segment.IsInRange(addr);
        }

        [Conditional("DEBUG")]
        public void DumpSections()
        {
            foreach (var item in Segments)
            {
                Debug.Print("Key: {0}, Value: name:{1,-18} size: {2:X8}, Access: {3}", item.Key, item.Value.Name, item.Value.Size, item.Value.Access);
            }
        }

        public ImageMap CreateImageMap()
        {
            var imageMap = new ImageMap(this.BaseAddress);
            foreach (var segment in segments.Values)
            {
                imageMap.AddItem(segment.Address, new ImageMapItem(segment.Size) { DataType = new UnknownType() });
            }
            return imageMap;
        }
    }
}
