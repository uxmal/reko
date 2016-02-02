#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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

using System;
using System.ComponentModel;

namespace Reko.Core
{
	/// <summary>
	/// Represent a segment of memory, corresponding to an 16-bit segment for x86
    ///  real and protected modes, and executable sections for flat processor modes.
	/// </summary>
    [Designer("Reko.Gui.Design.ImageMapSegmentNodeDesigner,Reko.Gui")]
	public class ImageSegment : ImageMapItem        //$REVIEW: why inherit from ImageMapItem?
	{
        private uint ctSize;

		public ImageSegment(string name, Address addr, AccessMode access) : base() 
		{
			if (name == null)
				throw new ArgumentNullException("name", "Segments must have names.");
			this.Name = name;
            this.Address = addr;
			this.Access = access;
		}

		public ImageSegment(string name, Address addr, uint size, AccessMode access) : base(size) 
		{
			if (name == null)
				throw new ArgumentNullException("name", "Segments must have names.");
            this.Name = name;
            this.Address = addr;
			this.Access = access;
		}

        public ImageSegment(string name, Address addr, long size, AccessMode access) : base((uint)size)
        {
            if (name == null)
                throw new ArgumentNullException("name", "Segments must have names.");
            this.Name = name;
            this.Address = addr;
            this.Access = access;
        }

        public ImageSegment(string name, MemoryArea mem, AccessMode access) : base((uint)mem.Length)
        {
            if (name == null)
                throw new ArgumentNullException("name", "Segments must have names.");
            this.Name = name;
            this.Address = mem.BaseAddress;
            this.MemoryArea = mem;
            this.Access = access;
        }

        /// <summary>
        /// The memory loaded into this segment.
        /// </summary>
        /// <remarks>
        /// Imageloaders are responsible for providing the MemoryArea for each
        /// segment being loaded.
        /// </remarks>
        public MemoryArea MemoryArea { get; set; }

		public AccessMode Access { get; set; }

        public uint ContentSize { get { return (ctSize != 0) ? ctSize : Size; } set { ctSize = value; } }

        public bool IsDiscardable { get; set; }

        public ImageSegmentRenderer Designer { get; set; }

		public string Name { get;set; }

        public override string ToString()
		{
			return string.Format("Segment {0} at {1}, {2} / {3} bytes", Name, Address.ToString(), ContentSize, Size);
		}
    }

	[Flags]
	public enum AccessMode
	{
		Read = 4,
		Write = 2,
		Execute = 1,

        ReadExecute = Read | Execute,
        ReadWrite = Read | Write,
        ReadWriteExecute = Read|Write|Execute,
	}
}
