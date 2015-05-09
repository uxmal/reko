#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
namespace Decompiler.Core
{
	/// <summary>
	/// Represent a segment of memory, corresponding to an 16-bit segment for intel real and protected modes, and 
	/// executable sections for flat processor modes.
	/// </summary>
    [Designer("Decompiler.Gui.Design.ImageMapSegmentDesigner,Decompiler")]
	public class ImageMapSegment : ImageMapItem
	{
		public ImageMapSegment(string name, AccessMode access) : base() 
		{
			if (name == null)
				throw new ArgumentNullException("name", "Segments must have names.");
			this.Name = name;
			this.Access = access;
		}

		public ImageMapSegment(string name, uint size, AccessMode access) : base(size) 
		{
			if (name == null)
				throw new ArgumentNullException("name", "Segments must have names.");
			this.Name = name;
			this.Access = access;
		}

		public AccessMode Access { get; private set; }

        public bool IsDiscardable { get; set; }

        public ImageMapSegmentRenderer Renderer { get; set; }

		public string Name { get;set; }

		public override string ToString()
		{
			return string.Format("Segment {0} at {1}, {2} bytes", Name, Address.ToString(), Size);
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
