/* 
 * Copyright (C) 1999-2007 John Källén.
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
	/// Represent a segment of memory, corresponding to an 16 segment for intel real and protected modes, and 
	/// executable sections for flat processor modes.
	/// </summary>
	public class ImageMapSegment : ImageMapItem
	{
		private AccessMode access; 

		public ImageMapSegment(AccessMode access) : base() 
		{
			this.access = access;
		}

		public ImageMapSegment(int size, AccessMode access) : base(size) 
		{
			this.access = access;
		}

		public AccessMode Access
		{
			get { return access; }
		}

		public override string ToString()
		{
			return "Segment at " + Address.ToString() + ", size: " + Size;
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
