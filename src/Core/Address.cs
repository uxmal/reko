#region License
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
#endregion

using Decompiler.Core.Expressions;
using Decompiler.Core.Types;
using System;

namespace Decompiler.Core
{
	public class Address : Expression, IComparable
	{
		public ushort Selector;			// Segment selector.
		public uint Offset;

		public Address(uint off) : base(PrimitiveType.Pointer32)
		{
			this.Selector = 0;
			this.Offset = off;
		}

		public Address(ushort seg, uint off) : base(PrimitiveType.Pointer32)
		{
			this.Selector = seg;
			this.Offset = off;
			if (seg != 0)
				this.Offset = (ushort) off;
		}

        public override Expression Accept(IExpressionTransformer xform)
        {
            return xform.TransformAddress(this);
        }

        public override T Accept<T>(ExpressionVisitor<T> v)
        {
            return v.VisitAddress(this);
        }

        public override void Accept(IExpressionVisitor visit)
        {
            visit.VisitAddress(this);
        }
        public override Expression CloneExpression()
        {
            return new Address(this.Selector, this.Offset);
        }

		public override bool Equals(object obj)
		{
			return CompareTo(obj) == 0;
		}

		public override int GetHashCode()
		{
			return Linear.GetHashCode();
		}

		public uint Linear
		{
			get { return (((uint) Selector << 4) + Offset); }
		}

		public string GenerateName(string prefix, string suffix)
		{
			return string.Format(
				(Selector == 0)
				? "{0}{2:X8}{3}" 
				: "{0}{1:X4}_{2:X4}{3}", 
				prefix, Selector, Offset, suffix);
		}

		public override string ToString()
		{
			string strFmt = (Selector == 0)
				? "{1:X8}"
				: "{0:X4}:{1:X4}";
			return string.Format(strFmt, Selector, Offset);
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
			uint newOff = (uint) (a.Offset + off);
			if (a.Selector != 0 && newOff > 0xFFFF)
			{
				a.Selector += 0x1000;
				newOff &= 0xFFFF;
			}
			return new Address(a.Selector, newOff);
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

        public static Address ToAddress(string s) { return ToAddress(s, 16); }

	}
}
