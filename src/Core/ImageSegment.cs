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

using Reko.Core.Expressions;
using Reko.Core.Memory;
using Reko.Core.Types;
using System;
using System.ComponentModel;
using System.Text;

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
            this.Name = name ?? throw new ArgumentNullException(nameof(name), "Segments must have names.");
            this.Address = addr ?? throw new ArgumentException(nameof(addr));
            this.MemoryArea = mem ?? throw new ArgumentNullException(nameof(mem));
			this.Access = access;
            this.Fields = CreateFields(0);
            this.Identifier = CreateIdentifier(Address);
        }

		public ImageSegment(string name, Address addr, uint size, AccessMode access)
		{
            this.Name = name ?? throw new ArgumentNullException(nameof(name), "Segments must have names.");
            this.Size = size;
            this.Address = addr ?? throw new ArgumentNullException(nameof(addr));
            this.MemoryArea = new ByteMemoryArea(addr, new byte[size]);
			this.Access = access;
            this.Fields = CreateFields((int)size);
            this.Identifier = CreateIdentifier(Address);
        }

        /// <summary>
        /// Use this constructor when the segment's memory area is completely 
        /// disjoint from other segments. This is usually the case in PE or ELF
        /// binaries.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="mem"></param>
        /// <param name="access"></param>
        public ImageSegment(string name, MemoryArea mem, AccessMode access)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name), "Segments must have names.");
            this.MemoryArea = mem ?? throw new ArgumentNullException(nameof(mem));
            this.Size = (uint)mem.Length;
            this.Address = mem.BaseAddress;
            this.Access = access;
            this.Fields = CreateFields((int)mem.Length);
            this.Identifier = CreateIdentifier(Address);
        }

        /// <summary>
        /// Start address of the segment
        /// </summary>
        public Address Address { get; private set; }

        /// <summary>
        /// The offset within a file other other source of data from
        /// which this segment is loaded.
        /// </summary>
        public ulong FileOffset { get; set; }

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

        /// <summary>
        /// The layout of this segment.
        /// </summary>
        public StructureType Fields { get; }

        /// <summary>
        /// Optional designer
        /// </summary>
        public ImageSegmentRenderer? Designer { get; set; }

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

        /// <summary>
        /// If true, the image segment contains executable code.
        /// </summary>
        public bool IsExecutable => (this.Access & AccessMode.Execute) != 0;

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
        /// like x86. It is not necessarily the same as the segment name in the
        /// image. In particular it has to be a valid C-like identifier, so 
        /// leading '.' are illegal.
        /// </remarks>
        public Identifier Identifier { get; set; }

        public ProvenanceType Provenance { get; set; }

        private StructureType CreateFields(int size)
        {
            string name = GenerateTypeName();
            var segType = new StructureType(name, size)
            {
                IsSegment = true
            };
            segType.IsSegment = true;
            return segType;
        }

        private string GenerateTypeName()
        {
            var sb = new StringBuilder("seg");
            if (this.Address.Selector.HasValue)
            {
                sb.AppendFormat("{0:X4}", Address.Selector.Value);
            }
            else
            {
                foreach (var ch in this.Name)
                {
                    if (char.IsDigit(ch) || char.IsLetter(ch) || ch == '_')
                        sb.Append(ch);
                    else
                        sb.Append('_');
                }
            }
            sb.Append("_t");
            return sb.ToString();
        }

        /// <summary>
        /// Creates an image reader that scans all available memory in the segment.
        /// </summary>
        /// <param name="platform"></param>
        /// <returns></returns>
        public EndianImageReader CreateImageReader(IProcessorArchitecture arch)
        {
            var offsetBegin = Math.Max(this.Address - this.MemoryArea.BaseAddress, 0);
            var offsetEnd = Math.Min(this.Size, this.MemoryArea.Length);
            return arch.CreateImageReader(this.MemoryArea, offsetBegin, offsetEnd);
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

        private static Identifier CreateIdentifier(Address addr)
        {
            string name;
            if (addr.Selector.HasValue)
            {
                name = $"seg{addr.Selector.Value:X4}";
            }
            else
            {
                name = $"seg{addr}";
            }
            var dt = PrimitiveType.SegmentSelector;
            var stg = new TemporaryStorage(name, 0, dt);
            return Identifier.Create(stg);
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
