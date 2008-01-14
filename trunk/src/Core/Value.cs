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

using Decompiler.Core.Types;
using System;

namespace Decompiler.Core
{
	public class Value
	{
		private PrimitiveType type;
		private object v;

		public static Value Invalid;

		static Value()
		{
			Invalid = new Value(null, 0);
		}

		public Value(PrimitiveType vt, sbyte s)
		{
			type = vt;
			this.v = s;
		}
		public Value(PrimitiveType vt, long v)
		{
			type = vt;
			this.v = v;
		}

		public Value(int v)
		{
			type = PrimitiveType.Word32;
			this.v = v;
		}

		public Value(uint v)
		{
			type = PrimitiveType.Word32;
			this.v = v;
		}

		public Value(long v)
		{
			type = PrimitiveType.Word64;
			this.v = v;
		}

		public Value(ulong v)
		{
			type = PrimitiveType.Word64;
			this.v = v;
		}

		public Value(double d)
		{
			type = PrimitiveType.Real64;
			this.v = d;
		}

		public Value(float f)
		{
			type = PrimitiveType.Real32;
			this.v = f;
		}

		public double AsDouble()
		{
			return (double) v;
		}

		public float AsFloat()
		{
			return (float) v;
		}

		public long AsLong()
		{
			return Convert.ToInt64(v);
		}

		public sbyte SByte
		{
			get { return Convert.ToSByte(v); }
		}

		public byte Byte
		{
			get { return Convert.ToByte(v); }
		}

		public override bool Equals(object o)
		{
			Value w = o as Value;
			if (w == null)
				return false;
			if (type != w.type)
				 return false;
			if (type.Domain == Domain.Real)
			{
				return v.Equals(w.v);
			}
			else
			{
				return (long) v == (long) w.v;
			}
		}

		private string FormatString()
		{
			switch (type.Domain)
			{
			case Domain.Real:
				switch (type.Size)
				{
				case 4: return "g";
				case 8: return "g";
				default: throw new ArgumentOutOfRangeException();
				}
			default:
				switch (type.Size)
				{
				case 1: return "X2";
				case 2: return "X4";
				case 4: return "X8";
				case 8: return "X8";
				default: throw new ArgumentOutOfRangeException();
				}
			}
		}

		public string FormatSigned()
		{
			if (type.IsIntegral)
			{
				string s = "+";
				long tmp = Convert.ToInt64(v);
				if (tmp < 0)
				{
					s = "-";
					tmp = -tmp;
				}
				return s + tmp.ToString(FormatString());
			}
			else
				throw new NotSupportedException("Signed formatting only supported for integral types");
		}

		public string FormatUnsigned()
		{
			if (type.IsIntegral)
			{
				return Unsigned.ToString(FormatString());
			}
			else if (type.Domain == Domain.Boolean)
			{
				if (Unsigned == 0)
					return "false";
				else
					return "true";
			}
			else
				throw new NotSupportedException("Unsigned formatting only supported for integral types");
		}

		public override int GetHashCode()
		{
			return this.type.GetHashCode() ^ v.GetHashCode() * 47;
		}

		public ushort Word
		{
			get { return (ushort) Convert.ToInt64(v); }
		}


		public bool IsValid
		{
			get { return type != null; }
		}

		public int Signed
		{
			get { return (int) Convert.ToInt64(v); }
		}

		public int SignExtend(PrimitiveType dstSize)
		{
			int q = (int) Convert.ToInt64(v);
			int mask = (0 - (q & (1 << (type.BitSize - 1)))) << 1;
			return q | mask;
		}

		public override string ToString()
		{
			if (type.Domain == Domain.Real)
			{
				return v.ToString();
			}
			else
				return FormatUnsigned();
		}

		public PrimitiveType Width
		{
			get { return type; }
		}

		public uint Unsigned
		{
			get { return (uint) Convert.ToInt64(v); }
		}
	}
}
