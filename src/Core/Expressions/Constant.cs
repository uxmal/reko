#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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

using Reko.Core.Lib;
using Reko.Core.Types;
using System;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Xml.Schema;
using System.Numerics;

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
            Debug.Assert(dt.BitSize > 0, "Bad constant size; this should never happen.");
            PrimitiveType p = (PrimitiveType)dt;
            int bitSize = p.BitSize;
            switch (bitSize)
            {
            case 1:
                return new ConstantBool(p, value != 0);
            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
            case 7:
                value &= (long)Bits.Mask(0, bitSize);
                goto case 8;
            case 8:
                switch (p.Domain)
                {
                case Domain.Boolean: return new ConstantBool(p, value != 0);
                case Domain.SignedInt: return new ConstantSByte(p, (sbyte) value);
                case Domain.Character: return new ConstantChar(p,(char) (byte) value);
                default: return new ConstantByte(p, (byte) value);
                }
            case 9:
            case 10:
            case 11:
            case 12:
            case 13:
            case 14:
            case 15:
                value &= (long)Bits.Mask(0, bitSize);
                goto case 16;
            case 16:
                switch (p.Domain)
                {
                case Domain.SignedInt: return new ConstantInt16(p, (short) value);
                case Domain.Character: return new ConstantChar(p, (char) value);
                default: return new ConstantUInt16(p, (ushort) value);
                }
            case 17:
            case 18:
            case 19:
            case 20:
            case 21:
            case 22:
            case 23:
            case 24:
            case 25:
            case 26:
            case 27:
            case 28:
            case 29:
            case 30:
            case 31:
                value &= (long)Bits.Mask(0, bitSize);
                goto case 32;
            case 32:
                switch (p.Domain)
                {
                case Domain.SignedInt: return new ConstantInt32(p, (int) value);
                case Domain.Real: return FloatFromBitpattern(value);
                default: return new ConstantUInt32(p, (uint) value);
                }
            case 36:        // PDP-10 <3
            case 40:
            case 48:
            case 56:
                value &= (long)Bits.Mask(0, bitSize);
                goto case 64;
            case 64:
                switch (p.Domain)
                {
                case Domain.SignedInt: return new ConstantInt64(p, (long) value);
                case Domain.Real: return DoubleFromBitpattern(value);
                default: return new ConstantUInt64(p, (ulong) value);
                }
            case 96:
            case 128:
                switch (p.Domain)
                {
                case Domain.SignedInt: return new BigConstant(p, (long)value);
                default: return BigConstant.CreateUnsigned(p, value);
                }
            case 136:
            case 144:
            case 192:
            case 224:
            case 256:
                switch (p.Domain)
                {
                case Domain.SignedInt: return new BigConstant(p, (long)value);
                default: return BigConstant.CreateUnsigned(p, (ulong)value);
                }
            default:
                //$TODO: if we encounter less common bit sizes, do them in a cascading if statement.
                break;
            }
            throw new NotSupportedException($"Constants of type {dt} are not supported.");
        }

        public static Constant Create(DataType dt, BigInteger value)
        {
            if (dt.BitSize > 64)
                return new BigConstant(dt, value);
            else
            {
                var uValue = value & Bits.Mask(dt.BitSize);
                return Create(dt, (ulong) uValue);
            }
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

        /// <summary>
        /// Given a raw bitpattern <paramref name="bits"/>, reinterpret
        /// it as an IEEE floating point number.
        /// //$REVIEW: this needs to handle non-IEEE floating point bit 
        /// patterns. It's likely it will have to be moved to IPlatform.
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="bits"></param>
        /// <returns></returns>
		public static Constant RealFromBitpattern(PrimitiveType dt, Constant bits)
		{
            if (dt == PrimitiveType.Real80)
            {
                var rawBits = bits.ToBigInteger();
                var exponent = (int) (rawBits >> 64);
                var mantissa = rawBits & ((BigInteger.One << 64) - BigInteger.One);
                return ConstantReal.Real80(new Float80((ushort) exponent, (ulong) mantissa));
            }
			if (dt == PrimitiveType.Real32)
				return FloatFromBitpattern(bits.ToInt64());
			else if (dt == PrimitiveType.Real64)
				return DoubleFromBitpattern(bits.ToInt64());
			else
				throw new ArgumentException(string.Format("Data type {0} is not a floating point type.", dt));
		}

		public static Constant FloatFromBitpattern(long bits)
		{
            return ConstantReal32.CreateFromBits(PrimitiveType.Real32, (int)bits);
		}

        public static float Int32BitsToFloat(int bits)
        {
            return ConstantReal32.FloatToInt.ConvertFromBits(bits);
        }

        public static Constant DoubleFromBitpattern(long bits)
		{
            return Constant.Real64(BitConverter.Int64BitsToDouble(bits));
		}

		public static double MakeReal(int exponent, int expBias, long mantissa, int mantissaSize)
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

        protected virtual Constant DoSlice(DataType dt, int offset)
        {
            var val = this.ToUInt64();
            ulong mask = Bits.Mask(offset, dt.BitSize);
            return Create(dt, (val & mask) >> offset);
        }

        protected virtual Constant DoDepositBits(Constant bits, int offset)
        {
            var val = this.ToUInt64();
            ulong mask = Bits.Mask(offset, bits.DataType.BitSize);
            var newVal = (val & ~mask) | ((bits.ToUInt64() << offset) & mask);
            return Constant.Create(this.DataType, newVal);
        }

        public abstract object GetValue();

        // Get the hash code of the value. We do it this way to avoid incurring the
        // cost of a boxing operation.
        public abstract int GetHashOfValue();

        //$REVIEW: move to Reko.Core.Lib?
        public static double IntPow(double b, int e)
		{
			double acc = 1.0;
            bool negativeExp = e < 0;
            if (negativeExp)
                e = -e;
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
			return negativeExp ? 1.0 / acc : acc;
		}

        public override bool IsZero => IsIntegerZero;

		public virtual bool IsIntegerZero
		{
			get 
			{
                if (!(DataType is PrimitiveType p) || p.Domain == Domain.Real)
                    return false;
                return ToInt64() == 0;
			}
		}

        public abstract bool IsMaxUnsigned { get; }

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
                return DataType is PrimitiveType p && p.Domain == Domain.Real;
            }
		}

        public virtual bool IsValid => true;

        /// <summary>
        /// Create a new Constant whose bits are the inverse of
        /// the bits of this Constant.
        /// </summary>
        public abstract Constant Complement();
 
        public Constant DepositBits(Constant newBits, int bitOffset)
        {
            if (bitOffset < 0 || newBits.DataType.BitSize + bitOffset > this.DataType.BitSize)
                throw new ArgumentException();
            if (!newBits.IsValid)
                return InvalidConstant.Create(this.DataType);
            return DoDepositBits(newBits, bitOffset);
        }

        public virtual Constant Negate()
		{
			PrimitiveType p = (PrimitiveType) DataType;
            var c = GetValue();
			if ((p.Domain & (Domain.SignedInt|Domain.UnsignedInt)) != 0)
			{
                p = PrimitiveType.Create(Domain.SignedInt, p.BitSize);
				if (p.BitSize <= 8)				
					return Constant.Create(p, (sbyte) -Convert.ToInt32(c));
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

        /// <summary>
        /// Return a bit slice of this constant, treating the contents as bits.
        /// </summary>
        /// <param name="dt">Data type of the slice</param>
        /// <param name="offset">Bit offset from which to take the slice.</param>
        /// <returns></returns>
        public virtual Constant Slice(DataType dt, int offset)
        {
            if (offset < 0 || offset + dt.BitSize > this.DataType.BitSize)
                throw new ArgumentException(nameof(dt), "Invalid bit size.");
            return DoSlice(dt, offset);
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
        public abstract BigInteger ToBigInteger();


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

        /// <summary>
        /// Creates a constant signed integer whose bit size is the same
        /// as <paramref name="dt"/>'s bit size.
        /// </summary>
        /// <param name="dt">Data type whose bit size is used to build 
        /// the constant.</param>
        /// <param name="l">Constant value.</param>
        public static Constant Int(DataType dt, long l)
        {
            var dtInt = PrimitiveType.Create(Domain.SignedInt, dt.BitSize);
            return Create(dtInt, l);
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

        public static Constant Real80(Float80 f80)
        {
            return new ConstantReal80(PrimitiveType.Real80, f80);
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

        public static Constant Word(int bitSize, long value)
        {
            return Create(PrimitiveType.CreateWord(bitSize), value);
        }

        public static Constant Word(int bitSize, ulong value)
        {
            return Create(PrimitiveType.CreateWord(bitSize), value);
        }

        public static Constant Zero(DataType dataType)
        {
            return Constant.Create(dataType, 0);
        }

        public static Constant String(string str, StringType strType)
        {
            return new StringConstant(strType, str);
        }

		//public static readonly Constant Invalid = new ConstantUInt32(VoidType.Instance, 0xBADDCAFE);
        //public static readonly Constant Unknown = new ConstantUInt32(VoidType.Instance, 0xDEADFACE);

        //public abstract string ToString(string format, IFormatProvider formatProvider)
        //{
        //}
    }

    internal class ConstantBool : Constant
    {
        private readonly bool value;

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

        public override int GetHashOfValue()
        {
            return value.GetHashCode();
        }

        public override Constant Complement()
        {
            return new ConstantBool(DataType, !value);
        }

        public override Expression Invert()
        {
            return new ConstantBool(DataType, !value);
        }

        public override bool IsMaxUnsigned => value;

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

        public override BigInteger ToBigInteger() => ToInt32();
    }

    internal class ConstantSByte : Constant
    {
        private readonly sbyte value;

        public ConstantSByte(DataType dt, sbyte value)
            : base(dt)
        {
            this.value = value;
        }

        public override Expression CloneExpression()
        {
            return new ConstantSByte(DataType, value);
        }

        public override Constant Complement()
        {
            return new ConstantSByte(DataType, (sbyte)~value);
        }

        public override object GetValue()
        {
            return value;
        }

        public override int GetHashOfValue()
        {
            return value.GetHashCode();
        }

        public override bool IsMaxUnsigned => value == -1;
        
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

        public override BigInteger ToBigInteger() => value;
    }

    internal class ConstantChar : Constant
    {
        private readonly char value;

        public ConstantChar(DataType dt, char value)
            : base(dt)
        {
            this.value = value;
        }

        public override Expression CloneExpression()
        {
            return new ConstantChar(DataType, value);
        }

        public override Constant Complement()
        {
            return new ConstantChar(DataType, (char)~value);
        }

        public override object GetValue()
        {
            return value;
        }

        public override int GetHashOfValue()
        {
            return value.GetHashCode();
        }

        public override bool IsMaxUnsigned => value == 0xFFFF;

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

        public override BigInteger ToBigInteger() => value;

    }

    internal class ConstantByte : Constant
    {
        private readonly byte value;

        public ConstantByte(DataType dt, byte value)
            : base(dt)
        {
            this.value = value;
        }

        public override Expression CloneExpression()
        {
            return new ConstantByte(DataType, value);
        }

        public override Constant Complement()
        {
            return new ConstantByte(DataType, (byte)~value);
        }


        public override object GetValue()
        {
            return value;
        }

        public override int GetHashOfValue()
        {
            return value.GetHashCode();
        }

        public override bool IsMaxUnsigned => value == 0xFF;

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

        public override BigInteger ToBigInteger() => value;

    }

    internal class ConstantInt16 : Constant
    {
        private readonly short value;

        public ConstantInt16(DataType dt, short value)
            : base(dt)
        {
            this.value = value;
        }

        public override Expression CloneExpression()
        {
            return new ConstantInt16(DataType, value);
        }

        public override Constant Complement()
        {
            return new ConstantInt16(DataType, (short)~value);
        }

        public override object GetValue()
        {
            return value;
        }

        public override int GetHashOfValue()
        {
            return value.GetHashCode();
        }

        public override bool IsMaxUnsigned => value == (short)-1;

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

        public override BigInteger ToBigInteger() => value;

    }

    internal class ConstantUInt16 : Constant
    {
        private readonly ushort value;

        public ConstantUInt16(DataType dt, ushort value)
            : base(dt)
        {
            this.value = value;
        }

        public override Expression CloneExpression()
        {
            return new ConstantUInt16(DataType, value);
        }

        public override Constant Complement()
        {
            return new ConstantUInt16(DataType, (ushort)~value);
        }

        public override object GetValue()
        {
            return value;
        }

        public override int GetHashOfValue()
        {
            return value.GetHashCode();
        }

        public override bool IsMaxUnsigned => value == 0xFFFFu;

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

        public override BigInteger ToBigInteger() => value;

    }

    internal class ConstantInt32 : Constant
    {
        private readonly int value;

        public ConstantInt32(DataType dt, int value)
            : base(dt)
        {
            this.value = value;
        }

        public override Expression CloneExpression()
        {
            return new ConstantInt32(DataType, value);
        }

        public override Constant Complement()
        {
            return new ConstantInt32(DataType, ~value);
        }

        public override object GetValue()
        {
            return value;
        }

        public override int GetHashOfValue()
        {
            return value.GetHashCode();
        }

        public override bool IsMaxUnsigned => value == -1;

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

        public override BigInteger ToBigInteger() => value;

    }

    internal class ConstantUInt32 : Constant
    {
        private readonly uint value;

        public ConstantUInt32(DataType dt, uint value)
            : base(dt)
        {
            this.value = value;
        }

        public override Expression CloneExpression()
        {
            return new ConstantUInt32(DataType, value);
        }

        public override Constant Complement()
        {
            return new ConstantUInt32(DataType, ~value);
        }


        public override object GetValue()
        {
            return value;
        }

        public override int GetHashOfValue()
        {
            return value.GetHashCode();
        }

        public override bool IsMaxUnsigned => value == ~0u;

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

        public override BigInteger ToBigInteger() => value;

    }


    internal class ConstantInt64 : Constant
    {
        private readonly long value;

        public ConstantInt64(DataType dt, long value)
            : base(dt)
        {
            this.value = value;
        }

        public override Expression CloneExpression()
        {
            return new ConstantInt64(DataType, value);
        }

        public override Constant Complement()
        {
            return new ConstantInt64(DataType, ~value);
        }


        public override object GetValue()
        {
            return value;
        }

        public override int GetHashOfValue()
        {
            return value.GetHashCode();
        }

        public override bool IsMaxUnsigned => value == -1L;

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

        public override BigInteger ToBigInteger() => value;

    }

    internal class ConstantUInt64 : Constant
    {
        private readonly ulong value;

        public ConstantUInt64(DataType dt, ulong value)
            : base(dt)
        {
            this.value = value;
        }

        public override Expression CloneExpression()
        {
            return new ConstantUInt64(DataType, value);
        }

        public override Constant Complement()
        {
            return new ConstantUInt64(DataType, ~value);
        }

        public override object GetValue()
        {
            return value;
        }

        public override int GetHashOfValue()
        {
            return value.GetHashCode();
        }

        public override bool IsMaxUnsigned => value == ~0UL;

        public override Constant Negate()
        {
            return new ConstantUInt64(this.DataType, ~value + 1);
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

        public override BigInteger ToBigInteger() => value;

    }

    public abstract class ConstantReal : Constant
    {
        public ConstantReal(DataType dt) : base(dt)
        {
        }

        public static Constant Create(DataType dt, double value)
        {
            var pt = PrimitiveType.Create(Domain.Real, dt.BitSize);
            switch (dt.BitSize)
            {
            case 16: return new ConstantReal16(pt, value);
            case 32: return new ConstantReal32(pt, (float)value);
            case 48: return new ConstantReal64(pt, value);
            case 64: return new ConstantReal64(pt, value);
            }
            // Unsupported floating point constant sizes cannot be represented yet.
            return InvalidConstant.Create(dt);
        }
    }

    public class ConstantReal16 : ConstantReal
    {
        private readonly Float16 value;

        public ConstantReal16(DataType dt, double value)
            : base(dt)
        {
            this.value = new Float16(value);
        }

        public ConstantReal16(DataType dt, Float16 value)
           : base(dt)
        {
            this.value = value;
        }

        public override Expression CloneExpression()
        {
            throw new NotImplementedException();
        }

        public override Constant Complement()
        {
            throw new NotSupportedException("Cannot complement a real value.");
        }

        protected override Constant DoSlice(DataType dt, int offset)
        {
            throw new NotImplementedException();
        }

        public override object GetValue()
        {
            throw new NotImplementedException();
        }

        public override int GetHashOfValue()
        {
            return value.GetHashCode();
        }

        public override bool IsMaxUnsigned => false;

        public override bool IsValid => !Float16.IsNaN(value);


        public override Constant Negate()
        {
            return new ConstantReal16(this.DataType, -value);
        }

        public override byte ToByte()
        {
            throw new NotImplementedException();
        }

        public override ushort ToUInt16()
        {
            throw new NotImplementedException();
        }

        public override uint ToUInt32()
        {
            throw new NotImplementedException();
        }

        public override ulong ToUInt64()
        {
            throw new NotImplementedException();
        }

        public override short ToInt16()
        {
            throw new NotImplementedException();
        }

        public override int ToInt32()
        {
            throw new NotImplementedException();
        }

        public override long ToInt64()
        {
            throw new NotImplementedException();
        }

        public override float ToFloat()
        {
            throw new NotImplementedException();
        }

        public override double ToReal64()
        {
            return value.ToDouble(CultureInfo.InvariantCulture);
        }

        public override BigInteger ToBigInteger() => throw new InvalidCastException();

    }

    internal class ConstantReal32 : ConstantReal
    {
        private readonly float value;

        // .NET doesn't seem to have a way of doing this for 32-bit floats.
        // Code from one of the answers to https://stackoverflow.com/questions/27237776/convert-int-bits-to-float-bits
        [StructLayout(LayoutKind.Explicit)]
        internal struct FloatToInt
        {
            [FieldOffset(0)] private float f;
            [FieldOffset(0)] private int i;
            public static int ConvertToBits(float value)
            {
                var inst = new FloatToInt();
                inst.f = value;
                return inst.i;
            }

            public static float ConvertFromBits(int bits)
            {
                var inst = new FloatToInt();
                inst.i = bits;
                return inst.f;
            }

        }

        public ConstantReal32(DataType dt, float value)
            : base(dt)
        {
            this.value = value;
        }

        public static ConstantReal32 CreateFromBits(DataType dt, int bits)
        {
            var value = FloatToInt.ConvertFromBits(bits);
            return new ConstantReal32(dt, value);
        }

        public override Expression CloneExpression()
        {
            return new ConstantReal32(DataType, value);
        }

        public override Constant Complement()
        {
            throw new NotSupportedException("Cannot complement a real value.");
        }

        protected override Constant DoSlice(DataType dt, int offset)
        {
            var bits = (uint) FloatToInt.ConvertToBits(this.value);
            var mask = Bits.Mask(0, dt.BitSize);
            return Constant.Create(dt, bits >> offset);
        }

        public override object GetValue()
        {
            return value;
        }

        public override int GetHashOfValue()
        {
            return value.GetHashCode();
        }

        public override bool IsMaxUnsigned => false;

        public override bool IsValid => !float.IsNaN(value);

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

        public override BigInteger ToBigInteger() => throw new InvalidCastException();

    }

    internal class ConstantReal80 : ConstantReal
    {
        private readonly Float80 value;

        public ConstantReal80(DataType dt, Float80 value)
            : base(dt)
        {
            this.value = value;
        }

        public override Expression CloneExpression()
        {
            return new ConstantReal80(DataType, value);
        }

        public override Constant Complement()
        {
            throw new NotSupportedException("Cannot complement a real value.");
        }

        protected override Constant DoSlice(DataType dt, int offset)
        {
            throw new NotSupportedException();
        }

        public override object GetValue()
        {
            return value;
        }

        public override int GetHashOfValue()
        {
            return value.GetHashCode();
        }

        public override bool IsMaxUnsigned => false;


        public override Constant Negate()
        {
            return new ConstantReal80(DataType, -value);
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

        public override BigInteger ToBigInteger() => throw new InvalidCastException();

    }

    internal class ConstantReal64 : ConstantReal
    {
        private readonly double value;

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

        public override int GetHashOfValue()
        {
            return value.GetHashCode();
        }

        public override Constant Complement()
        {
            throw new NotSupportedException("Cannot complement a real value.");
        }

        protected override Constant DoSlice(DataType dt, int offset)
        {
            var bits = (ulong) BitConverter.DoubleToInt64Bits(this.value);
            var mask = Bits.Mask(0, dt.BitSize);
            return Constant.Create(dt, bits >> offset);
        }

        public override bool IsMaxUnsigned => false;

        public override bool IsValid => !double.IsNaN(value);


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

        public override BigInteger ToBigInteger() => throw new InvalidCastException();

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

        public override Constant Complement()
        {
            throw new NotSupportedException("Cannot complement a real value.");
        }

        protected override Constant DoSlice(DataType dt, int offset)
        {
            throw new NotSupportedException();
        }

        public override object GetValue()
        {
            return str;
        }

        public override int GetHashOfValue()
        {
            return str.GetHashCode();
        }

        public override bool IsMaxUnsigned => false;

        public override Constant Negate()
        {
            throw new NotSupportedException("Cannot negate a string value.");
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

        public override BigInteger ToBigInteger() => throw new InvalidCastException();


        public override string ToString()
        {
            return str;
        }
    }
}
