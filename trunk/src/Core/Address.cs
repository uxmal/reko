#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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
using System.Collections.Generic;

namespace Decompiler.Core
{
	public class Address : Expression, IComparable
	{
		public readonly ushort Selector;			// Segment selector.
		public readonly uint Offset;

		public Address(uint off) : base(PrimitiveType.Pointer32)
		{
			this.Selector = 0;
			this.Offset = off;
		}

        public Address(PrimitiveType size, uint bitPattern) : base(size)
        {
            this.Offset = bitPattern;
        }

        public static Address Ptr16(ushort addr)
        {
            return new Address(PrimitiveType.Ptr16, addr);
        }

        public static Address Ptr64(ulong addr)
        {
            throw new NotImplementedException("We need separate classes for addresses.");
        }

		public Address(ushort seg, uint off) : base(PrimitiveType.Pointer32)
		{
			this.Selector = seg;
			this.Offset = off;
			if (seg != 0)
				this.Offset = (ushort) off;
		}

        public override T Accept<T, C>(ExpressionVisitor<T, C> v, C context)
        {
            return v.VisitAddress(this, context);
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
            string strFmt;
            if (Selector == 0)
            {
                switch (base.DataType.Size)
                {
                case 2: strFmt = "{0}{2:X4}{3}"; break;
                case 4: strFmt = "{0}{2:X8}{3}"; break;
                default: throw new NotSupportedException(string.Format("Address size of {0} bytes not supported.", DataType.Size));
                }
            }
            else
            {
                strFmt = "{0}{1:X4}_{2:X4}{3}"; 
            }
			return string.Format(strFmt, prefix, Selector, Offset, suffix);
		}

		public override string ToString()
		{
			string strFmt;
            if (Selector == 0)
            {
                switch (base.DataType.Size)
                {
                    case 2: strFmt = "{1:X4}"; break;
                    case 4: strFmt = "{1:X8}"; break;
                    default: throw new NotSupportedException(string.Format("Address size of {0} bytes not supported.", DataType.Size));
                }
            }
            else 
            {
                strFmt = "{0:X4}:{1:X4}";
            }
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
			return a.Linear >= b.Linear;
		}

		public static Address operator + (Address a, uint off)
		{
			return a + (int) off;
		}

		public static Address operator + (Address a, int off)
		{
            ushort sel = a.Selector;
			uint newOff = (uint) (a.Offset + off);
			if (a.Selector != 0 && newOff > 0xFFFF)
			{
				sel += 0x1000;
				newOff &= 0xFFFF;
			}
			return new Address(sel, newOff);
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
		/// Converts a string representation of an address to an Address.
		/// </summary>
		/// <param name="s">The string representation of the Address</param>
		/// <param name="radix">The radix used in the  representation, typically 16 for hexadecimal address representation.</param>
		/// <returns></returns>
		public static Address Parse(string s, int radix)
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

        public static bool TryParse(string s, int radix, out Address result)
        {
            if (s != null)
            {
                try
                {
                    var a = s.Split(':');
                    if (a.Length == 2)
                    {
                        var seg = Convert.ToUInt16(a[0], radix);
                        var off = Convert.ToUInt32(a[1], radix);
                        result = new Address(seg, off);
                    }
                    else
                    {
                        result = new Address(Convert.ToUInt32(a[0], radix));
                    }
                    return true;
                }
                catch
                {
                }
            }
            result = null;
            return false;
        }
            

        /// <summary>
        /// Converts a hexadecimal string representation of an address to an Address.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Address Parse(string s) { return Parse(s, 16); }

        public class Comparer : IEqualityComparer<Address>
        {
            public bool Equals(Address x, Address y)
            {
                return x.Linear == y.Linear;
            }

            public int GetHashCode(Address obj)
            {
                return obj.Linear.GetHashCode();
            }
        }
    }
}
