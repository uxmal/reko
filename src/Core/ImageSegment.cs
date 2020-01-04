#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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

using Reko.Core.Expressions;
using System;
using System.ComponentModel;

namespace Reko.Core
{
	/// <summary>
	/// Represents a segment of memory, corresponding to an 16-bit segment for
    /// x86 real and protected modes, and sections of executable files for for
    /// flat processor modes.
	/// </summary>
    [Designer("Reko.Gui.Design.ImageMapSegmentNodeDesigner,Reko.Gui")]
	public class ImageSegment
	{
        private uint ctSize;

        /// <summary>
        /// Use this constructor when the segment shares the MemoryArea with
        /// other segments.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="addr"></param>
        /// <param name="mem"></param>
        /// <param name="access"></param>
		public ImageSegment(string name, Address addr, MemoryArea mem, AccessMode access) : base() 
		{
			if (name == null)
				throw new ArgumentNullException(nameof(name), "Segments must have names.");
			this.Name = name;
            this.Address = addr ?? throw new ArgumentException(nameof(addr));
            this.MemoryArea = mem ?? throw new ArgumentNullException(nameof(mem));
			this.Access = access;
		}

		public ImageSegment(string name, Address addr, uint size, AccessMode access)
		{
			if (name == null)
				throw new ArgumentNullException(nameof(name), "Segments must have names.");
            this.Name = name;
            this.Size = size;
            this.Address = addr ?? throw new ArgumentNullException(nameof(addr));
            this.MemoryArea = new MemoryArea(addr, new byte[size]);
			this.Access = access;
		}

        /// <summary>
        /// Use this constructor when the segment's memory area is completely 
        /// disjoint fromother segments. This is usually the case in PE or ELF
        /// binaries.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="mem"></param>
        /// <param name="access"></param>
        public ImageSegment(string name, MemoryArea mem, AccessMode access)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name), "Segments must have names.");
            this.Name = name;
            this.MemoryArea = mem ?? throw new ArgumentNullException(nameof(mem));
            this.Size = (uint)mem.Length;
            this.Address = mem.BaseAddress;
            this.Access = access;
        }

        /// <summary>
        /// Start address of the segment
        /// </summary>
        public Address Address { get; private set; }

        /// <summary>
        /// Size of the segment address space (content may be smaller)
        /// </summary>
        public uint Size { get; set; }

        public uint ContentSize { get { return (ctSize != 0) ? ctSize : Size; } set { ctSize = value; } }

        /// <summary>
        /// The memory loaded into this segment.
        /// </summary>
        /// <remarks>
        /// Imageloaders are responsible for providing the MemoryArea for each
        /// segment being loaded.
        /// </remarks>
        public MemoryArea MemoryArea { get; set; }

        /// <summary>
        /// Access mode of the segment.
        /// </summary>
		public AccessMode Access { get; set; }

        public ImageSegmentRenderer Designer { get; set; }

		public string Name { get; set; }

        //$TODO: remove this property. EndAddress becomes undefined when
        // the segment base address + size hit the end of the address space.
        // E.g. a Z80 ROM program whose base adress is 0xFF00 and whose size
        // is 0x100 would have an end address of 0xFF00 + 0x100 = 0x10000,
        // which can't be represented as a Address16.
        public Address EndAddress { get { return Address + ContentSize; } }

        public bool IsDiscardable { get; set; }

        /// <summary>
        /// If set to true, this segment should not be emitted as source code.
        /// </summary>
        public bool IsHidden { get; set; }

        public bool IsExecutable { get { return (this.Access & AccessMode.Execute) != 0; } }

        /// <summary>
        /// If true, the segment's contents may change over the execution of
        /// program.
        /// </summary>
        public bool IsWriteable => (this.Access & AccessMode.Write) != 0;

        /// <summary>
        /// The identifier used in the program to refer to the segment.
        /// </summary>
        /// <remarks>
        /// Used primarily on architectures with segmented address spaces
        /// like x86.
        /// </remarks>
        public Identifier Identifier { get; set; }

        /// <summary>
        /// Creates an image reader that scans all available memory in the segment.
        /// </summary>
        /// <param name="platform"></param>
        /// <returns></returns>
        public EndianImageReader CreateImageReader(IProcessorArchitecture arch)
        {
            var addrBegin = Address.Max(this.Address, this.MemoryArea.BaseAddress);
            var addrEnd = Address.Min(this.Address + this.Size, this.MemoryArea.EndAddress);
            return arch.CreateImageReader(this.MemoryArea, addrBegin, addrEnd);
        }

        public bool IsInRange(Address addr)
        {
            return IsInRange(addr.ToLinear());
        }

        public virtual bool IsInRange(ulong linearAddress)
        {
            ulong linItem = this.Address.ToLinear();
            return (linItem <= linearAddress && linearAddress < linItem + Size);
        }

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
