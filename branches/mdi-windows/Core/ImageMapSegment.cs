/* 
 * Copyright (C) 1999-2010 John Källén.
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

using System;

namespace Decompiler.Core
{
	/// <summary>
	/// Represent a segment of memory, corresponding to an 16-bit segment for intel real and protected modes, and 
	/// executable sections for flat processor modes.
	/// </summary>
	public class ImageMapSegment : ImageMapItem
	{
		private string name;
		private AccessMode access; 

		public ImageMapSegment(string name, AccessMode access) : base() 
		{
			if (name == null)
				throw new ArgumentNullException("name", "Segments must have names.");
			this.name = name;
			this.access = access;
		}

		public ImageMapSegment(string name, int size, AccessMode access) : base(size) 
		{
			if (name == null)
				throw new ArgumentNullException("name", "Segments must have names.");
			this.name = name;
			this.access = access;
		}

		public AccessMode Access
		{
			get { return access; }
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public override string ToString()
		{
			return string.Format("Segment {0} at {1}, {2} bytes", Name, Address.ToString(), Size);
		}
	}

	[Flags]
	public enum AccessMode
	{
		Read = 1,
		Write = 2,
		Execute = 4,

		ReadWrite = Read|Write,
	}
}
