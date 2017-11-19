#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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

namespace Reko.Core
{
	/// <summary>
	/// Describes an address range [begin...end)
	/// </summary>
	public class AddressRange
	{
		private Address addrBegin;
		private Address addrEnd;

		public AddressRange(Address addrBegin, Address addrEnd)
		{
			if (addrBegin == null)
				throw new ArgumentNullException("addrBegin");
			if (addrEnd == null)
				throw new ArgumentNullException("addrEnd");
			this.addrBegin = addrBegin;
			this.addrEnd = addrEnd;
		}

		public Address Begin
		{
			get { return addrBegin; }
		}

		public Address End
		{
			get { return addrEnd; }
		}

        public bool IsValid
        {
            get { return this != e; }
        }

        public static AddressRange Empty
        {
            get { return e; }
        }

        private static AddressRange e = new AddressRange(Address.Ptr32(0), Address.Ptr32(0));
    }
}
