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

        public Address(DataType size, uint bitPattern) : base(size)
        {
            this.Offset = bitPattern;
        }

        public static Address Ptr16(ushort addr)
        {
            return new Address16(addr);
        }

        public static Address Ptr32(uint uAddr)
        {
            return new Address32(uAddr);
        }

        public static Address Ptr64(ulong addr)
        {
            if (addr >= (1ul << 32))
                throw new NotImplementedException("We need separate classes for addresses.");
            return new Address64(addr);
        }

        public static Address SegPtr(ushort seg, uint off)
        {
            return new Address(seg, off);
        }
        
        public static Address FromConstant(Constant value)
        {
            switch (value.DataType.BitSize)
            {
            case 16: return Ptr16(value.ToUInt16());
            case 32: return Ptr32(value.ToUInt32());
            default: throw new NotImplementedException();
            }
        }

		protected Address(ushort seg, uint off) : base(PrimitiveType.Pointer32)
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

        [Obsolete]
		public uint Linear
		{
			get { return (((uint) Selector << 4) + Offset); }
		}

        public virtual ulong ToLinear()
        {
            return ((ulong) Selector << 4) + Offset;
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
                case 8: strFmt = "{0}{2:X16}{3}"; break;
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
                    case 8: strFmt = "{1:X16}"; break;
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

        public static Address operator + (Address a, ulong off)
		{
			return a.Add((long) off);
		}

		public static Address operator + (Address a, long off)
		{
            return a.Add(off);
        }

        public virtual Address Add(long offset)
        {
            ushort sel = this.Selector;
			uint newOff = (uint) (this.Offset + offset);
			if (this.Selector != 0 && newOff > 0xFFFF)
			{
				sel += 0x1000;
				newOff &= 0xFFFF;
			}
			return new Address(sel, newOff);
		}

		public static Address operator - (Address a, int delta)
		{
			return a.Add(-delta);
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

        public static bool TryParse16(string s, out Address result)
        {
            if (s != null)
            {
                try
                {
                    result = Ptr16(Convert.ToUInt16(s, 16));
                    return true;
                }
                catch { }
            }
            result = null;
            return false;
        }

        public static bool TryParse32(string s, out Address result)
        {
            if (s != null)
            {
                try
                {
                    result = Ptr32(Convert.ToUInt32(s, 16));
                    return true;
                }
                catch { }
            }
            result = null;
            return false;
        }

        public static bool TryParse64(string s, out Address result)
        {
            if (s != null)
            {
                try
                {
                    result = Ptr64(Convert.ToUInt64(s, 16));
                    return true;
                }
                catch { }
            }
            result = null;
            return false;
        }

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

    public class Address16 : Address
    {
        private ushort uValue;

        public Address16(ushort addr)
            : base(PrimitiveType.Ptr16, addr)
        {
            this.uValue = addr;
        }

        public override ulong ToLinear()
        {
            return uValue;
        }
    }

    public class Address32 : Address
    {
        private uint uValue;

        public Address32(uint addr)
            : base(PrimitiveType.Pointer32, addr)
        {
            this.uValue = addr;
        }

        public override ulong ToLinear()
        {
            return uValue;
        }
    }

    public class SegAddress32 : Address
    {
        private ushort uSegment;
        private ushort uOffset;

        public SegAddress32(ushort segment, ushort offset)
            : base(segment, offset)
        {
            this.uSegment = segment;
            this.uOffset = offset;
        }

        public override ulong ToLinear()
        {
            return (((ulong)uSegment) << 4) + uOffset;
        }
    }

    public class Address64 : Address
    {
        private readonly ulong uValue;

        public Address64(ulong addr)
            : base(PrimitiveType.Pointer64, (uint)addr)
        {
            this.uValue = addr;
        }

        public override ulong ToLinear()
        {
            return uValue;
        }

        public override Address Add(long offset)
        {
            return new Address64(uValue + (ulong) offset);
        }
    }
}
