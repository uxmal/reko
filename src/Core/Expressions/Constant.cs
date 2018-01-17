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

using Reko.Core.Types;
using System;
using System.IO;
using System.Globalization;
using System.Collections.Generic;

namespace Reko.Core.Expressions
{
    /// <summary>
    /// Models a constant value.
    /// </summary>
	public abstract class Constant : Expression // , IFormattable
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
            switch (p.Size)
            {
            case 1:
                switch (p.Domain)
                {
                case Domain.Boolean: return new ConstantBool(p, value != 0);
                case Domain.SignedInt: return new ConstantSByte(p, (sbyte) value);
                case Domain.Character: return new ConstantChar(p,(char) (byte) value);
                default: return new ConstantByte(p, (byte) value);
                }
            case 2:
                switch (p.Domain)
                {
                case Domain.SignedInt: return new ConstantInt16(p, (short) value);
                case Domain.Character: return new ConstantChar(p, (char) value);
                default: return new ConstantUInt16(p, (ushort) value);
                }
            case 4:
                switch (p.Domain)
                {
                case Domain.SignedInt: return new ConstantInt32(p, (int) value);
                case Domain.Real: return new ConstantUInt32(p,  (uint) value);
                default: return new ConstantUInt32(p, (uint) value);
                }
            case 8:
                switch (p.Domain)
                {
                case Domain.SignedInt: return new ConstantInt64(p, (long) value);
                case Domain.Real: return new ConstantUInt64(p, (ulong) value);
                default: return new ConstantUInt64(p, (ulong) value);
                }
            case 16:
                switch (p.Domain)
                {
                case Domain.SignedInt: return new ConstantInt128(p, (long)value);
                default: return new ConstantUInt128(p, (ulong)value);
                }
            case 32:
                switch (p.Domain)
                {
                case Domain.SignedInt: return new ConstantInt256(p, (long)value);
                default: return new ConstantUInt128(p, (ulong)value);
                }
            }
            throw new NotSupportedException(string.Format("Constants of type {0} are not supported.", dt));
        }

        public override T Accept<T, C>(ExpressionVisitor<T, C> v, C context)
        {
            return v.VisitConstant(this, context);
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
            return new ConstantByte(PrimitiveType.Byte, c);
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
			return new ConstantBool(PrimitiveType.Bool, false);
		}

        public override IEnumerable<Expression> Children
        {
            get { yield break; }
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

        public override bool IsZero
        {
            get
            {
                return IsIntegerZero;
            }
        }

		public bool IsIntegerZero
		{
			get 
			{
				PrimitiveType p = DataType as PrimitiveType;
				if (p == null || p.Domain == Domain.Real)
					return false; 
				return ToInt64() == 0;
			}
		}

		public virtual bool IsNegative
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
                else if (p.Domain == Domain.Pointer)
                    return false;
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

		public virtual Constant Negate()
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
                    return Constant.Create(p, -Convert.ToInt64(c) & -1);
                return Constant.Create(p, -Convert.ToInt64(c));
			}
			else 
				throw new InvalidOperationException(string.Format("Type {0} doesn't support negation.", p));
		}

		public static Constant Pi()
		{
            return Constant.Real64(Math.PI);
		}

        public static Constant Lg10()
        {
            // log(2) of 10.
            return Constant.Real64(3.3219280948873623478703194294894);
        }

        public static Constant LgE()
        {
            // log(2) of e.
            return Constant.Real64(1.4426950408889634073599246810019);
        }

        public static Constant Ln2()
        {
            return Constant.Real64(0.69314718055994530941723212145818);
        }

        public static Constant Log2()
        {
            // log(10) of 2
            return Constant.Real64(0.30102999566398119521373889472449);
        }
       
        public virtual bool ToBoolean()
        {
            return Convert.ToBoolean(GetValue());
        }

        public abstract byte ToByte();
        public abstract ushort ToUInt16();
        public abstract uint ToUInt32();
        public abstract ulong ToUInt64();

        public abstract short ToInt16();
        public abstract int ToInt32();
        public abstract long ToInt64();


		public double ToDouble()
		{
			return Convert.ToDouble(GetValue());
		}

		public virtual float ToFloat()
		{
			return Convert.ToSingle(GetValue());
		}

        public virtual double ToReal64()
        {
            return Convert.ToDouble(GetValue());
        }

		public static Constant True()
		{
			return new ConstantBool(PrimitiveType.Bool, true);
		}

        public static Constant Bool(bool f)
        {
            return f ? True() : False();
        }

        public static Constant SByte(sbyte p)
        {
            return new ConstantSByte(PrimitiveType.SByte, p);
        }

        public static Constant Int16(short s)
        {
            return new ConstantInt16(PrimitiveType.Int16, s);
        }

        public static Constant Int32(int i)
        {
            return new ConstantInt32(PrimitiveType.Int32, i);
        }

        public static Constant Int64(long l)
        {
            return new ConstantInt64(PrimitiveType.Int64, l);
        }

        public static Constant Real32(float f)
        {
            return new ConstantReal32(PrimitiveType.Real32, f);
        }
   
        public static Constant Real64(double d)
        {
            return new ConstantReal64(PrimitiveType.Real64, d);
        }

        public static Constant UInt16(ushort u)
        {
            return new ConstantUInt16(PrimitiveType.UInt16, u);
        }

        public static Constant UInt32(uint w)
        {
            return new ConstantUInt32(PrimitiveType.UInt32, w);
        }

        public static Constant UInt64(ulong ul)
        {
            return new ConstantUInt64(PrimitiveType.UInt64, ul);
        }

        public static Constant Word16(ushort n)
        {
            return new ConstantUInt16(PrimitiveType.Word16, n);
        }

		public static Constant Word32(int n)
		{
			return new ConstantUInt32(PrimitiveType.Word32, (uint) n); 
		}

        public static Constant Word32(uint n)
        {
            return new ConstantUInt32(PrimitiveType.Word32, n);
        }

        public static Constant Word64(long n)
        {
            return new ConstantUInt64(PrimitiveType.Word64, (ulong) n);
        }

        public static Constant Word64(ulong n)
        {
            return new ConstantUInt64(PrimitiveType.Word64, n);
        }

        public static Constant Word(int byteSize, long value)
        {
            return Create(PrimitiveType.CreateWord(byteSize), value);
        }

        public static Constant Zero(DataType dataType)
        {
            return Constant.Create(dataType, 0);
        }

        public static Constant String(string str, StringType strType)
        {
            return new StringConstant(strType, str);
        }

		public static readonly Constant Invalid = new ConstantUInt32(VoidType.Instance, 0xBADDCAFE);
        public static readonly Constant Unknown = new ConstantUInt32(VoidType.Instance, 0xDEADFACE);

        //public abstract string ToString(string format, IFormatProvider formatProvider)
        //{
        //}
    }

    internal class ConstantBool : Constant
    {
        private bool value;

        public ConstantBool(DataType dt, bool value)
            : base(dt)
        {
            this.value = value;
        }

        public override Expression CloneExpression()
        {
            return new ConstantBool(DataType, value);
        }

        public override object GetValue()
        {
            return value;
        }

        public override Expression Invert()
        {
            return new ConstantBool(DataType, !value);
        }

        public override bool ToBoolean()
        {
            return value;
        }

        public override byte ToByte()
        {
            return value ? (byte)1 : (byte)0;
        }

        public override ushort ToUInt16()
        {
            return value ? (ushort)1u : (ushort)0u;
        }

        public override uint ToUInt32()
        {
            return value ? 1u : 0u;
        }

        public override ulong ToUInt64()
        {
            return value ? 1u : 0u;
        }

        public override short ToInt16()
        {
            return value ? (short)1 : (short)0;
        }

        public override int ToInt32()
        {
            return value ? 1 : 0;
        }

        public override long ToInt64()
        {
            return value ? 1 : 0;
        }
    }

    internal class ConstantSByte : Constant
    {
        private sbyte value;

        public ConstantSByte(DataType dt, sbyte value)
            : base(dt)
        {
            this.value = value;
        }

        public override Expression CloneExpression()
        {
            return new ConstantSByte(DataType, value);
        }

        public override object GetValue()
        {
            return value;
        }

        public override byte ToByte()
        {
            return (byte)value;
        }

        public override ushort ToUInt16()
        {
            return (ushort)(short)value;
        }

        public override uint ToUInt32()
        {
            return (uint)(int)value;
        }

        public override ulong ToUInt64()
        {
            return (ulong)(long)value;
        }

        public override short ToInt16()
        {
            return value;
        }

        public override int ToInt32()
        {
            return value;
        }

        public override long ToInt64()
        {
            return value;
        }
    }

    internal class ConstantChar : Constant
    {
        private char value;

        public ConstantChar(DataType dt, char value)
            : base(dt)
        {
            this.value = value;
        }

        public override Expression CloneExpression()
        {
            return new ConstantChar(DataType, value);
        }

        public override object GetValue()
        {
            return value;
        }

        public override byte ToByte()
        {
            return (byte)value;
        }

        public override ushort ToUInt16()
        {
            return (ushort)value;
        }

        public override uint ToUInt32()
        {
            return (uint)value;
        }

        public override ulong ToUInt64()
        {
            return (ulong)value;
        }

        public override short ToInt16()
        {
            return (short)value;
        }

        public override int ToInt32()
        {
            return (int)value;
        }

        public override long ToInt64()
        {
            return (long)value;
        }
    }

    internal class ConstantByte : Constant
    {
        private byte value;

        public ConstantByte(DataType dt, byte value)
            : base(dt)
        {
            this.value = value;
        }

        public override Expression CloneExpression()
        {
            return new ConstantByte(DataType, value);
        }

        public override object GetValue()
        {
            return value;
        }

        public override byte ToByte()
        {
            return value;
        }

        public override ushort ToUInt16()
        {
            return (ushort)value;
        }

        public override uint ToUInt32()
        {
            return (uint)value;
        }

        public override ulong ToUInt64()
        {
            return value;
        }

        public override short ToInt16()
        {
            return (sbyte)value;
        }

        public override int ToInt32()
        {
            return (sbyte)value;
        }

        public override long ToInt64()
        {
            return (sbyte)value;
        }
    }

    internal class ConstantInt16 : Constant
    {
        private short value;

        public ConstantInt16(DataType dt, short value)
            : base(dt)
        {
            this.value = value;
        }

        public override Expression CloneExpression()
        {
            return new ConstantInt16(DataType, value);
        }

        public override object GetValue()
        {
            return value;
        }

        public override byte ToByte()
        {
            return (byte)value;
        }

        public override ushort ToUInt16()
        {
            return (ushort)value;
        }

        public override uint ToUInt32()
        {
            return (uint)value;
        }

        public override ulong ToUInt64()
        {
            return (uint) value;
        }

        public override short ToInt16()
        {
            return value;
        }

        public override int ToInt32()
        {
            return value;
        }

        public override long ToInt64()
        {
            return value;
        }
    }

    internal class ConstantUInt16 : Constant
    {
        private ushort value;

        public ConstantUInt16(DataType dt, ushort value)
            : base(dt)
        {
            this.value = value;
        }

        public override Expression CloneExpression()
        {
            return new ConstantUInt16(DataType, value);
        }

        public override object GetValue()
        {
            return value;
        }

        public override byte ToByte()
        {
            return (byte)value;
        }

        public override ushort ToUInt16()
        {
            return value;
        }

        public override uint ToUInt32()
        {
            return value;
        }

        public override ulong ToUInt64()
        {
            return value;
        }

        public override short ToInt16()
        {
            return (short)value;
        }

        public override int ToInt32()
        {
            return (short)value;
        }

        public override long ToInt64()
        {
            return (short)value;
        }
    }

    internal class ConstantInt32 : Constant
    {
        private int value;

        public ConstantInt32(DataType dt, int value)
            : base(dt)
        {
            this.value = value;
        }

        public override Expression CloneExpression()
        {
            return new ConstantInt32(DataType, value);
        }

        public override object GetValue()
        {
            return value;
        }

        public override byte ToByte()
        {
            return (byte)value;
        }

        public override ushort ToUInt16()
        {
            return (ushort)value;
        }

        public override uint ToUInt32()
        {
            return (uint)value;
        }

        public override ulong ToUInt64()
        {
            return (uint) value;
        }

        public override short ToInt16()
        {
            return (short)value;
        }

        public override int ToInt32()
        {
            return value;
        }

        public override long ToInt64()
        {
            return value;
        }
    }

    internal class ConstantUInt32 : Constant
    {
        private uint value;

        public ConstantUInt32(DataType dt, uint value)
            : base(dt)
        {
            this.value = value;
        }

        public override Expression CloneExpression()
        {
            return new ConstantUInt32(DataType, value);
        }

        public override object GetValue()
        {
            return value;
        }

        public override byte ToByte()
        {
            return (byte)value;
        }

        public override ushort ToUInt16()
        {
            return (ushort) value;
        }

        public override uint ToUInt32()
        {
            return value;
        }

        public override ulong ToUInt64()
        {
            return value;
        }

        public override short ToInt16()
        {
            return (short)value;
        }

        public override int ToInt32()
        {
            return (int)value;
        }

        public override long ToInt64()
        {
            return (int)value;
        }
    }

    internal class ConstantInt64 : Constant
    {
        private long value;

        public ConstantInt64(DataType dt, long value)
            : base(dt)
        {
            this.value = value;
        }

        public override Expression CloneExpression()
        {
            return new ConstantInt64(DataType, value);
        }

        public override object GetValue()
        {
            return value;
        }

        public override byte ToByte()
        {
            return (byte)value;
        }

        public override ushort ToUInt16()
        {
            return (ushort)value;
        }

        public override uint ToUInt32()
        {
            return (uint)value;
        }

        public override ulong ToUInt64()
        {
            return (ulong) value;
        }

        public override short ToInt16()
        {
            return (short)value;
        }

        public override int ToInt32()
        {
            return (int)value;
        }

        public override long ToInt64()
        {
            return value;
        }
    }

    internal class ConstantUInt64 : Constant
    {
        private ulong value;

        public ConstantUInt64(DataType dt, ulong value)
            : base(dt)
        {
            this.value = value;
        }

        public override Expression CloneExpression()
        {
            return new ConstantUInt64(DataType, value);
        }

        public override object GetValue()
        {
            return value;
        }

        public override byte ToByte()
        {
            return (byte)value;
        }

        public override ushort ToUInt16()
        {
            return (ushort)value;
        }

        public override uint ToUInt32()
        {
            return (uint)value;
        }

        public override ulong ToUInt64()
        {
            return value;
        }

        public override short ToInt16()
        {
            return (short)value;
        }

        public override int ToInt32()
        {
            return (int)value;
        }

        public override long ToInt64()
        {
            return (long) value;
        }
    }

    internal class ConstantInt128 : Constant
    {
        private long value;

        public ConstantInt128(DataType dt, long value)
            : base(dt)
        {
            this.value = value;
        }

        public override Expression CloneExpression()
        {
            return new ConstantInt128(DataType, value);
        }

        public override object GetValue()
        {
            return value;
        }

        public override byte ToByte()
        {
            return (byte)value;
        }

        public override ushort ToUInt16()
        {
            return (ushort)value;
        }

        public override uint ToUInt32()
        {
            return (uint)value;
        }

        public override ulong ToUInt64()
        {
            return (ulong)value;
        }

        public override short ToInt16()
        {
            return (short)value;
        }

        public override int ToInt32()
        {
            return (int)value;
        }

        public override long ToInt64()
        {
            return (long)value;
        }
    }

    internal class ConstantUInt128 : Constant
    {
        private ulong value;

        public ConstantUInt128(DataType dt, ulong value)
            : base(dt)
        {
            this.value = value;
        }

        public override Expression CloneExpression()
        {
            return new ConstantUInt64(DataType, value);
        }

        public override object GetValue()
        {
            return value;
        }

        public override byte ToByte()
        {
            return (byte)value;
        }

        public override ushort ToUInt16()
        {
            return (ushort)value;
        }

        public override uint ToUInt32()
        {
            return (uint)value;
        }

        public override ulong ToUInt64()
        {
            return value;
        }

        public override short ToInt16()
        {
            return (short)value;
        }

        public override int ToInt32()
        {
            return (int)value;
        }

        public override long ToInt64()
        {
            return (long)value;
        }
    }

    internal class ConstantInt256 : Constant
    {
        private long value;

        public ConstantInt256(DataType dt, long value)
            : base(dt)
        {
            this.value = value;
        }

        public override Expression CloneExpression()
        {
            return new ConstantInt128(DataType, value);
        }

        public override object GetValue()
        {
            return value;
        }

        public override byte ToByte()
        {
            return (byte)value;
        }

        public override ushort ToUInt16()
        {
            return (ushort)value;
        }

        public override uint ToUInt32()
        {
            return (uint)value;
        }

        public override ulong ToUInt64()
        {
            return (ulong)value;
        }

        public override short ToInt16()
        {
            return (short)value;
        }

        public override int ToInt32()
        {
            return (int)value;
        }

        public override long ToInt64()
        {
            return (long)value;
        }
    }

    internal class ConstantUInt256 : Constant
    {
        private ulong value;

        public ConstantUInt256(DataType dt, ulong value)
            : base(dt)
        {
            this.value = value;
        }

        public override Expression CloneExpression()
        {
            return new ConstantUInt64(DataType, value);
        }

        public override object GetValue()
        {
            return value;
        }

        public override byte ToByte()
        {
            return (byte)value;
        }

        public override ushort ToUInt16()
        {
            return (ushort)value;
        }

        public override uint ToUInt32()
        {
            return (uint)value;
        }

        public override ulong ToUInt64()
        {
            return value;
        }

        public override short ToInt16()
        {
            return (short)value;
        }

        public override int ToInt32()
        {
            return (int)value;
        }

        public override long ToInt64()
        {
            return (long)value;
        }
    }


    public abstract class ConstantReal : Constant
    {
        public ConstantReal(DataType dt) : base(dt)
        {
        }

        public static ConstantReal Create(DataType dt, double value)
        {
            var pt = PrimitiveType.Create(Domain.Real, dt.Size);
            switch (dt.BitSize)
            {
            case 32: return new ConstantReal32(pt, (float)value);
            case 64: return new ConstantReal64(pt, value);
            }
            throw new NotSupportedException(string.Format("Data type {0} not supported.", dt));
        }
    }

    internal class ConstantReal32 : ConstantReal
    {
        private float value;

        public ConstantReal32(DataType dt, float value)
            : base(dt)
        {
            this.value = value;
        }

        public override Expression CloneExpression()
        {
            return new ConstantReal32(DataType, value);
        }

        public override object GetValue()
        {
            return value;
        }

        public override Constant Negate()
        {
            return new ConstantReal32(DataType, -value);
        }

        public override byte ToByte()
        {
            return Convert.ToByte(value);
        }

        public override ushort ToUInt16()
        {
            return Convert.ToUInt16(value);
        }

        public override uint ToUInt32()
        {
            return Convert.ToUInt32(value);
        }

        public override ulong ToUInt64()
        {
            return Convert.ToUInt64(value);
        }

        public override short ToInt16()
        {
            return Convert.ToInt16(value);
        }

        public override int ToInt32()
        {
            return Convert.ToInt32(value);
        }

        public override long ToInt64()
        {
            return Convert.ToInt64(value);
        }

        public override float ToFloat()
        {
            return value;
        }
    }

    internal class ConstantReal64 : ConstantReal
    {
        private double value;

        public ConstantReal64(DataType dt, double value)
            : base(dt)
        {
            this.value = value;
        }

        public override Expression CloneExpression()
        {
            return new ConstantReal64(DataType, value);
        }

        public override object GetValue()
        {
            return value;
        }

        public override Constant Negate()
        {
            return new ConstantReal64(DataType, -value);
        }

        public override byte ToByte()
        {
            return Convert.ToByte(value);
        }

        public override ushort ToUInt16()
        {
            return Convert.ToUInt16(value);
        }

        public override uint ToUInt32()
        {
            return Convert.ToUInt32(value);
        }

        public override ulong ToUInt64()
        {
            return Convert.ToUInt64(value);
        }

        public override short ToInt16()
        {
            return Convert.ToInt16(value);
        }

        public override int ToInt32()
        {
            return Convert.ToInt32(value);
        }

        public override long ToInt64()
        {
            return Convert.ToInt64(value);
        }
    }

    public class StringConstant : Constant
    {
        private string str;

        public StringConstant(DataType type, string str)
            : base(type)
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

        public override byte ToByte()
        {
            throw new InvalidCastException();
        }

        public override ushort ToUInt16()
        {
            throw new InvalidCastException();
        }

        public override uint ToUInt32()
        {
            throw new InvalidCastException();
        }

        public override ulong ToUInt64()
        {
            throw new InvalidCastException();
        }

        public override short ToInt16()
        {
            throw new InvalidCastException();
        }

        public override int ToInt32()
        {
            throw new InvalidCastException();
        }

        public override long ToInt64()
        {
            throw new InvalidCastException();
        }

        public override string ToString()
        {
            return str;
        }
    }
}
