/* 
 * Copyright (C) 1999-2008 John Källén.
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
	public class Address : IComparable
	{
		public ushort seg;
		public uint off;

		public Address(uint off)
		{
			this.seg = 0;
			this.off = off;
		}

		public Address(ushort seg, uint off)
		{
			this.seg = seg;
			this.off = off;
			if (seg != 0)
				this.off = (ushort) off;
		}

		public override bool Equals(object obj)
		{
			return CompareTo(obj) == 0;
		}

		public override int GetHashCode()
		{
			return seg.GetHashCode() ^ off.GetHashCode();
		}

		public int Linear
		{
			get  { return (int) (((uint) seg << 4) + off); }
		}

		public string GenerateName(string prefix, string suffix)
		{
			return string.Format(
				(seg == 0)
				? "{0}{2:X8}{3}" 
				: "{0}{1:X4}_{2:X4}{3}", 
				prefix, seg, off, suffix);
		}

		public override string ToString()
		{
			string strFmt = (seg == 0)
				? "{1:X8}"
				: "{0:X4}:{1:X4}";
			return string.Format(strFmt, seg, off);
		}

		public static bool operator < (Address a, Address b)
		{
			return a.Linear < b.Linear;
		}

		public static bool operator <= (Address a, Address b)
		{
			return a.Linear <= b.Linear;
		}

		public static bool operator > (Address a, Address b)
		{
			return a.Linear > b.Linear;
		}

		public static bool operator >= (Address a, Address b)
		{
			return a.Linear > b.Linear;
		}

		public static Address operator + (Address a, uint off)
		{
			return a + (int) off;
		}

		public static Address operator + (Address a, int off)
		{
			uint newOff = (uint) (a.off + off);
			if (a.seg != 0 && newOff > 0xFFFF)
			{
				a.seg += 0x1000;
				newOff &= 0xFFFF;
			}
			return new Address(a.seg, newOff);
		}

		public static Address operator - (Address a, int delta)
		{
			return a + (-delta);
		}

		public static int operator - (Address a, Address b)
		{
			return (int) a.Linear - (int) b.Linear;
		}

		public int CompareTo(object a)
		{
			return this - ((Address) a);
		}

		/// <summary>
		/// Converts a string representation of an Address to an address.
		/// </summary>
		/// <param name="s">The string representation of the Address</param>
		/// <param name="radix">The radix used in the  representation, typically 16 for hexadecimal address representation.</param>
		/// <returns></returns>
		public static Address ToAddress(string s, int radix)
		{
			if (s == null)
				return null;
			int c = s.IndexOf(':');
			if (c > 0)
			{
				return new Address(
					Convert.ToUInt16(s.Substring(0, c), radix),
					Convert.ToUInt32(s.Substring(c+1), radix));
			}
			else
			{
				return new Address(Convert.ToUInt32(s, radix));
			}
		}
	}
}
