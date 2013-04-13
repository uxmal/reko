#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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

using Decompiler.Core.Types;
using System;
using System.IO;
using System.Globalization;

namespace Decompiler.Core.Expressions
{
    public class StringConstant : Constant
    {
        private string str;

        public StringConstant(DataType type, string str) : base(type) 
        {
            this.str = str;
        }

        public int Length { get { return str.Length; } }

        public override Expression CloneExpression()
        {
            return new StringConstant(DataType, str);
        }

        public override object GetValue()
        {
            return str;
        }

        public override string ToString()
        {
            return str;
        }
    }

	public abstract class Constant : Expression
	{
        protected Constant(DataType t)
            : base(t)
        {
        }

        public static Constant Create(DataType dt, ulong value)
        {
            return Create(dt, (long) value);
        }

        public static Constant Create(DataType dt, long value)
        {
            PrimitiveType p = (PrimitiveType)dt;
            object v;
            switch (p.Size)
            {
            case 1:
                switch (p.Domain)
                {
                case Domain.Boolean: return new Constant<bool>(p, value != 0);
                case Domain.SignedInt: return new Constant<sbyte>(p, (sbyte) value);
                case Domain.Character: return new Constant<char>(p,(char) (byte) value);
                default: return new Constant<byte>(p, (byte) value);
                }
            case 2:
                switch (p.Domain)
                {
                case Domain.SignedInt: return new Constant<short>(p, (short) value);
                case Domain.Character: return new Constant<ushort>(p, (char) value);
                default: return new Constant<ushort>(p, (ushort) value);
                }
            case 4:
                switch (p.Domain)
                {
                case Domain.SignedInt: return new Constant<int>(p, (int) value);
                case Domain.Real: throw new NotImplementedException();
                default: return new Constant<uint>(p, (uint) value);
                }
            case 8:
                switch (p.Domain)
                {
                case Domain.SignedInt: return new Constant<long>(p, (long) value);
                case Domain.Real: return new Constant<ulong>(p, (ulong) value);
                default: return new Constant<ulong>(p, (ulong) value);
                }
            }
            throw new NotSupportedException(string.Format("Constants of type {0} are not supported.", dt));
        }

        public override T Accept<T>(ExpressionVisitor<T> v)
        {
            return v.VisitConstant(this);
        }

        public override void Accept(IExpressionVisitor v)
		{
			v.VisitConstant(this);
		}

        public static Constant Byte(byte c)
        {
            return new Constant<byte>(PrimitiveType.Byte, c);
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
				return Constant.Real32(0.0F);
			return Constant.Real32(sign * (float) MakeReal(exp, 0x7F, mant, 23));
		}

		public static Constant DoubleFromBitpattern(long bits)
		{
            return Constant.Real64(BitConverter.Int64BitsToDouble(bits));
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
			return new Constant<bool>(PrimitiveType.Bool, false);
		}

        public abstract object GetValue();
	
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
				return Convert.ToInt64(GetValue()) == 0;
			}
		}

		public bool IsNegative
		{
			get 
			{
				PrimitiveType p = (PrimitiveType) DataType;
				if (p.Domain == Domain.SignedInt)
					return Convert.ToInt64(GetValue()) < 0;
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

		public Constant Negate()
		{
			PrimitiveType p = (PrimitiveType) DataType;
            var c = GetValue();
			if ((p.Domain & (Domain.SignedInt|Domain.UnsignedInt)) != 0)
			{
                p = PrimitiveType.Create(Domain.SignedInt, p.Size);
				if (p.BitSize <= 8)				
					return Constant.Create(p, -Convert.ToSByte(c));
				if (p.BitSize <= 16)
                    return Constant.Create(p, -Convert.ToInt32(c) & 0xFFFF);
				if (p.BitSize <= 32)
                    return Constant.Create(p, -Convert.ToInt32(c));
                return Constant.Create(p, -Convert.ToInt64(c));
			}
			else if (p == PrimitiveType.Real32)
			{
                return Constant.Real32(-ToFloat());
			}
			else if (p == PrimitiveType.Real64)
			{
                return Constant.Real64(-ToDouble());
			}
			else 
				throw new InvalidOperationException(string.Format("Type {0} doesn't support negation.", p));
		}

		public static Constant Pi()
		{
            return Constant.Real64(Math.PI);
		}

        public static Constant Ln2()
        {
            return Constant.Real64(0.69314718055994530941723212145818);
        }

        public bool ToBoolean()
        {
            return Convert.ToBoolean(GetValue());
        }

		public double ToDouble()
		{
			return Convert.ToDouble(GetValue());
		}

		public float ToFloat()
		{
			return Convert.ToSingle(GetValue());
		}

        public ushort ToUInt16()
        {
            return unchecked(Convert.ToUInt16(GetValue()));
        }

        public short ToInt16()
        {
            return unchecked(Convert.ToInt16(GetValue()));
        }

		public int ToInt32()
		{
			int q = (int)Convert.ToInt64(GetValue());
            int mask = (0 - (q & (1 << (DataType.BitSize - 1)))) << 1;
            return q | mask;
		}

		public uint ToUInt32()
		{
			return Convert.ToUInt32(GetValue());
		}

		public long ToInt64()
		{
            return Convert.ToInt64(GetValue());
		}

		public ulong ToUInt64()
		{
            return Convert.ToUInt64(GetValue());
		}

		public static Constant True()
		{
			return new Constant<bool>(PrimitiveType.Bool, true);
		}

        public static Constant Bool(bool f)
        {
            return f ? True() : False();
        }

        public static Constant SByte(sbyte p)
        {
            return new Constant<sbyte>(PrimitiveType.SByte, p);
        }

        public static Constant Int16(short s)
        {
            return new Constant<short>(PrimitiveType.Int16, s);
        }

        public static Constant Int32(int i)
        {
            return new Constant<Int32>(PrimitiveType.Int32, i);
        }

        public static Constant Real32(float f)
        {
            return new Constant<Single>(PrimitiveType.Real32, f);
        }

        public static Constant Real64(double d)
        {
            return new Constant<Double>(PrimitiveType.Real64, d);
        }

        public static Constant Word16(ushort n)
        {
            return new Constant<ushort>(PrimitiveType.Word16, n);
        }

		public static Constant Word32(int n)
		{
			return new Constant<uint>(PrimitiveType.Word32, (uint) n); 
		}

        public static Constant Word32(uint n)
        {
            return new Constant<uint>(PrimitiveType.Word32, n);
        }

        public static Constant Zero(DataType dataType)
        {
            return Constant.Create(dataType, 0);
        }

		public static readonly Constant Invalid = new Constant<uint>(PrimitiveType.Void, 0xBADDCAFE);
        public static readonly Constant Unknown = new Constant<uint>(PrimitiveType.Void, 0xDEADFACE);
    }

    internal class Constant<T> : Constant
    {
        private T value;

        public Constant(DataType dt, T value) : base(dt)
        {
            this.value = value;
        }

        public override Expression CloneExpression()
        {
            return new Constant<T>(DataType, value);
        }

        public override object GetValue()
        {
            return value;
        }
    }
}
