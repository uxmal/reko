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

using Decompiler.Core.Types;
using System;
using System.IO;
using System.Globalization;

namespace Decompiler.Core.Code
{
	public class Constant : Expression
	{
		private object c;

		public Constant(DataType t, object v) : base(t)
		{
			PrimitiveType p = t as PrimitiveType;
			if (p != null)
			{
				switch (p.Size)
				{
				case 1: 
					v = p.Domain == Domain.SignedInt ? (object) (sbyte) Convert.ToInt64(v) : (object) (byte) (Convert.ToInt64(v) & 0xFF);
					break;
				case 2:
					v = p.Domain == Domain.SignedInt ? (object) (short) Convert.ToInt64(v) : (object) (ushort) (Convert.ToInt64(v) & 0xFFFF);
					break;
				case 4:
					v = p.Domain == Domain.SignedInt ? (object) (int) Convert.ToInt64(v) : (object) (uint) (Convert.ToInt64(v) & 0xFFFFFFFF);
					break;
				}
			}
			this.c = v;
		}

		public Constant(ushort us) : base(PrimitiveType.Word16)
		{
			this.c = us;
		}

		public Constant(double d) : base(PrimitiveType.Real64)
		{
			this.c = d;
		}

		public Constant(float f) : base(PrimitiveType.Real32)
		{
			this.c = f;
		}

		public override Expression Accept(IExpressionTransformer xform)
		{
			return xform.TransformConstant(this);
		}

		public override void Accept(IExpressionVisitor v)
		{
			v.VisitConstant(this);
		}

        public static Constant Byte(byte c)
        {
            return new Constant(PrimitiveType.Byte, c);
        }


		public override Expression CloneExpression()
		{
			return new Constant(DataType, c);
		}


		public static Constant RealFromBitpattern(PrimitiveType dt, long bits)
		{
			if (dt == PrimitiveType.Real32)
				return FloatFromBitpattern(bits);
			else if (dt == PrimitiveType.Real64)
				return DoubleFromBitpattern(bits);
			else
				throw new ArgumentException(string.Format("Data type {0} is not a floating point type.", dt));
		}

		public static Constant FloatFromBitpattern(long bits)
		{
			long mant = bits & 0x007FFFFF;
			int exp =  (int) (bits >> 23) & 0xFF;
			float sign = (bits < 0) ? -1.0F : 1.0F;
			if (mant == 0 && exp == 0)
				return new Constant(0.0F);
			return new Constant(sign * (float) MakeReal(exp, 0x7F, mant, 23));
		}

		public static Constant DoubleFromBitpattern(long bits)
		{
			long mant = bits & 0x000FFFFFFFFFFFFF;
			int exp =   (int) (bits >> 52) & 0x7FF;
			double sign = (bits < 0) ? -1.0 : 1.0;
			if (mant == 0 && exp == 0)
				return new Constant(0.0);
			return new Constant(MakeReal(exp, 0x3FF, mant, 52) * sign);

		}

		private static double MakeReal(int exponent, int expBias, long mantissa, int mantissaSize)
		{
			if (exponent == 0)
			{
				// denormalised number.
			}
			else
			{
				mantissa |= 1L << mantissaSize; // implicit 1 for normal 
			}

			exponent -= (expBias + mantissaSize);

			double m;
			if (exponent < 0)
			{
				m = 0.5;
				exponent = -exponent;
			}
			else
			{
				m = 2.0;
			}
			return mantissa * IntPow(m, exponent);
		}


		public static Constant False()
		{
			return new Constant(PrimitiveType.Bool, 0);
		}

		private string FormatString(PrimitiveType type)
		{
			switch (type.Domain)
			{
			case Domain.SignedInt:
				return "{0}";
			case Domain.Real:
				switch (type.Size)
				{
				case 4: return "{0:g}F";
				case 8: return "{0:g}";
				default: throw new ArgumentOutOfRangeException("Only real types of size 4 and 8 are supported.");
				}
			default:
				switch (type.Size)
				{
				case 1: return "0x{0:X2}";
				case 2: return "0x{0:X4}";
				case 4: return "0x{0:X8}";
                    case 5: case 3: return "$$0x{0:X16}$$";
				case 8: return "0x{0:X16}";
				default: throw new ArgumentOutOfRangeException("type", type.Size, string.Format("Integral types of size {0} are not supported.", type.Size));
				}
			}
		}

		private static double IntPow(double b, int e)
		{
			double acc = 1.0;

			while (e != 0)
			{
				if ((e & 1) == 1)
				{
					acc *= b;
					--e;
				}
				else
				{
					b *= b;
					e >>= 1;
				}
			}
			return acc;
		}

		public bool IsIntegerZero
		{
			get 
			{
				PrimitiveType p = DataType as PrimitiveType;
				if (p == null || p.Domain == Domain.Real)
					return false; 
				return Convert.ToInt64(c) == 0;
			}
		}

		public bool IsNegative
		{
			get 
			{
				PrimitiveType p = (PrimitiveType) DataType;
				if (p.Domain == Domain.SignedInt)
					return Convert.ToInt64(c) < 0;
				else if (p == PrimitiveType.Real32)
					return ToFloat() < 0.0F;
				else if (p == PrimitiveType.Real64)
					return ToDouble() < 0.0;
				else 
					throw new InvalidOperationException(string.Format("Type {0} can't be negative", DataType));
			}
		}

		public bool IsReal
		{
			get 
			{
				PrimitiveType p = DataType as PrimitiveType;
				return p != null && p.Domain == Domain.Real;
			}
		}

		public bool IsValid
		{
			get { return !Object.ReferenceEquals(this, Constant.Invalid); }
		}

		public static Constant Ln2()
		{
			return new Constant(0.69314718055994530941723212145818);
		}

		public Constant Negate()
		{
			PrimitiveType p = (PrimitiveType) DataType;
			if ((p.Domain & (Domain.SignedInt|Domain.UnsignedInt)) != 0)
			{
                p = PrimitiveType.Create(Domain.SignedInt, p.Size);
				if (p.BitSize <= 8)				
					return new Constant(p, -Convert.ToSByte(c));
				if (p.BitSize <= 16)
					return new Constant(p, -Convert.ToInt32(c) & 0xFFFF);
				if (p.BitSize <= 32)
					return new Constant(p, -Convert.ToInt32(c));
				return new Constant(p, -Convert.ToInt64(c));
			}
			else if (p == PrimitiveType.Real32)
			{
				return new Constant(-ToFloat());
			}
			else if (p == PrimitiveType.Real64)
			{
				return new Constant(-ToDouble());
			}
			else 
				throw new InvalidOperationException(string.Format("Type {0} doesn't support negation.", p));
		}

		public static Constant Pi()
		{
            return new Constant(Math.PI);
		}

		public double ToDouble()
		{
			return Convert.ToDouble(c);
		}

		public float ToFloat()
		{
			return Convert.ToSingle(c);
		}

        public ushort ToUInt16()
        {
            return unchecked((ushort) Convert.ToInt64(c));
        }

        public short ToInt16()
        {
            return unchecked((short)Convert.ToInt64(c));
        }

		public int ToInt32()
		{
			int q = (int) Convert.ToInt64(c);
 			int mask = (0 - (q & (1 << (DataType.BitSize - 1)))) << 1;
			return q | mask;
		}

		public uint ToUInt32()
		{
			return unchecked((uint) Convert.ToInt64(c));
		}

		public long ToInt64()
		{
			return Convert.ToInt64(c);
		}

		public ulong ToUInt64()
		{
			return Convert.ToUInt64(c);
		}

		public override string ToString()
		{
			if (Object.ReferenceEquals(this, Invalid))
				return "<void>";
			PrimitiveType t = (PrimitiveType) DataType;
			if (t.Domain == Domain.Boolean)
			{
				return (Convert.ToBoolean(c)) ? "true" : "false";
			}
			else
			{
				return string.Format(CultureInfo.InvariantCulture, FormatString(t), c);
			}
		}

		public static Constant True()
		{
			return new Constant(PrimitiveType.Bool, 1);
		}

        public static Constant SByte(sbyte p)
        {
            return new Constant(PrimitiveType.SByte, p);
        }

        public static Constant Word16(ushort n)
        {
            return new Constant(PrimitiveType.Word16, n);
        }

		public static Constant Word32(int n)
		{
			return new Constant(PrimitiveType.Word32, n); 
		}

        public static Constant Word32(uint n)
        {
            return new Constant(PrimitiveType.Word32, n);
        }

        public static Expression Zero(DataType dataType)
        {
            return new Constant(dataType, 0);
        }

		public static readonly Constant Invalid = new Constant(PrimitiveType.Void, 0);

    }
}
