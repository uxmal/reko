#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;

namespace Reko.Core.Expressions
{
    /// <summary>
    /// Models a constant value.
    /// </summary>
	public abstract class Constant : AbstractExpression, MachineOperand
    {
        /// <summary>
        /// Initializes the <see cref="DataType"/> property.
        /// </summary>
        /// <param name="dt">Datatype of the constant.</param>
        protected Constant(DataType dt)
            : base(dt)
        {
        }

        /// <summary>
        /// Create and return a new integer constant.
        /// </summary>
        /// <param name="dt">Datatype of the constant.</param>
        /// <param name="value">Raw bit representation of the constant.</param>
        /// <returns>An instance of the <see cref="Constant"/> class.</returns>
        public static Constant Create(DataType dt, ulong value)
        {
            return Create(dt, (long) value);
        }

        /// <summary>
        /// Create and return a new integer constant.
        /// </summary>
        /// <param name="dt">Datatype of the constant.</param>
        /// <param name="value">Raw bit representation of the constant.</param>
        /// <returns>An instance of the <see cref="Constant"/> class.</returns>
        public static Constant Create(DataType dt, long value)
        {
            Debug.Assert(dt.BitSize > 0, "Bad constant size; this should never happen.");
            int bitSize = dt.BitSize;
            switch (bitSize)
            {
            case 1:
                return new ConstantBool(dt, value != 0);
            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
            case 7:
                value &= (long) Bits.Mask(0, bitSize);
                goto case 8;
            case 8:
                switch (dt.Domain)
                {
                case Domain.Boolean: return new ConstantBool(dt, value != 0);
                case Domain.SignedInt: return new ConstantSByte(dt, (sbyte) value);
                case Domain.Character: return new ConstantChar(dt, (char) (byte) value);
                default: return new ConstantByte(dt, (byte) value);
                }
            case 9:
            case 10:
            case 11:
            case 12:
            case 13:
            case 14:
            case 15:
                value &= (long) Bits.Mask(0, bitSize);
                goto case 16;
            case 16:
                switch (dt.Domain)
                {
                case Domain.SignedInt: return new ConstantInt16(dt, (short) value);
                case Domain.Character: return new ConstantChar(dt, (char) value);
                default: return new ConstantUInt16(dt, (ushort) value);
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
                if (dt.Domain != Domain.SignedInt)
                    value &= (long) Bits.Mask(0, bitSize);
                goto case 32;
            case 32:
                switch (dt.Domain)
                {
                case Domain.SignedInt: return new ConstantInt32(dt, (int) value);
                case Domain.Real: return FloatFromBitpattern(value);
                default: return new ConstantUInt32(dt, (uint) value);
                }
            case 36:        // PDP-10 <3
            case 40:
            case 48:
            case 56:
                value &= (long) Bits.Mask(0, bitSize);
                goto case 64;
            case 64:
                switch (dt.Domain)
                {
                case Domain.SignedInt: return new ConstantInt64(dt, (long) value);
                case Domain.Real: return DoubleFromBitpattern(value);
                default: return new ConstantUInt64(dt, (ulong) value);
                }
            default:
                switch (dt.Domain)
                {
                case Domain.SignedInt: return new BigConstant(dt, (long) value);
                default: return BigConstant.CreateUnsigned(dt, (ulong) value);
                }
            }
            throw new NotSupportedException($"Constants of type {dt} are not supported.");
        }

        /// <summary>
        /// Create a <see cref="Constant"/> of the given data type.
        /// </summary>
        /// <param name="dt">Datatype of the constant.</param>
        /// <param name="value">The numeric value of the constant.</param>
        /// <returns>Resulting constant.</returns>
        public static Constant Create(DataType dt, BigInteger value)
        {
            if (dt.BitSize > 64)
                return new BigConstant(dt, value);
            var uValue = value & Bits.Mask(dt.BitSize);
            return Create(dt, (ulong) uValue);
        }

        /// <summary>
        /// Replicates the given value to fill the entire size of <paramref name="dt"/>.
        /// </summary>
        /// <param name="dt">Resulting size.</param>
        /// <param name="valueToReplicate">Value to replicate.</param>
        /// <returns>A <see cref="Constant"/> with the replicated value.</returns>
        public static Constant Replicate(DataType dt, Constant valueToReplicate)
        {
            if (dt.BitSize > 64)
                return BigConstant.Replicate(dt, valueToReplicate);
            var n = valueToReplicate.ToUInt64();
            int bits = valueToReplicate.DataType.BitSize;
            int times = dt.BitSize / bits;

            ulong result = 0;
            for (int i = 0; i < times; ++i)
            {
                result = (result << bits) | n;
            }
            return Create(dt, result);
        }

        /// <inheritdoc/>
        public override T Accept<T, C>(ExpressionVisitor<T, C> v, C context)
        {
            return v.VisitConstant(this, context);
        }

        /// <inheritdoc/>
        public override T Accept<T>(ExpressionVisitor<T> v)
        {
            return v.VisitConstant(this);
        }

        /// <inheritdoc/>
        public override void Accept(IExpressionVisitor v)
        {
            v.VisitConstant(this);
        }

        /// <summary>
        /// Creates a byte-sized <see cref="Constant"/>.
        /// </summary>
        /// <param name="c">Value.</param>
        /// <returns>The resulting constant.</returns>
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
		public static Constant RealFromBitpattern(DataType dt, Constant bits)
        {
            if (dt.Domain == Domain.Real)
            {
                if (dt.BitSize == 80)
                {
                    var rawBits = bits.ToBigInteger();
                    var exponent = (int) (rawBits >> 64);
                    var mantissa = rawBits & ((BigInteger.One << 64) - BigInteger.One);
                    return ConstantReal.Real80(new Float80((ushort) exponent, (ulong) mantissa));
                }
                if (dt.BitSize == 32)
                    return FloatFromBitpattern(bits.ToInt64());
                if (dt.BitSize == 64)
                    return DoubleFromBitpattern(bits.ToInt64());
            }
            throw new ArgumentException($"Data type {dt} is not a floating point type.");
        }

        /// <summary>
        /// Creates a 32-bit floating point constant from an IEEE-785 bit pattern.
        /// </summary>
        /// <param name="bits">Raw bit pattern.</param>
        /// <returns>Constant wrapping a 32-bit float.</returns>
        public static Constant FloatFromBitpattern(long bits)
        {
            return ConstantReal32.CreateFromBits(PrimitiveType.Real32, (int) bits);
        }

        /// <summary>
        /// Creates a 64-bit floating point constant from an IEEE-785 bit pattern.
        /// </summary>
        /// <param name="bits">Raw bit pattern.</param>
        /// <returns>Constant wrapping a 64-bit float.</returns>
        public static Constant DoubleFromBitpattern(long bits)
        {
            return Constant.Real64(BitConverter.Int64BitsToDouble(bits));
        }

        /// <summary>
        /// Constructs a real number from its components
        /// </summary>
        /// <param name="exponent">Exponent to use.</param>
        /// <param name="expBias">Exponent bias to use.</param>
        /// <param name="mantissa">Mantissa of value.</param>
        /// <param name="mantissaSize">Size of mantissa.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Creates a constant representing the boolean value 'false'
        /// </summary>
        public static Constant False()
        {
            return new ConstantBool(PrimitiveType.Bool, false);
        }

        /// <inheritdoc/>
        public override IEnumerable<Expression> Children
        {
            get { yield break; }
        }

        /// <summary>
        /// Slices this constant of size <paramref name="dt"/> at bit position 
        /// <paramref name="offset"/>.
        /// </summary>
        /// <param name="dt">Size of the requested slice.</param>
        /// <param name="offset">Bit offset of the slice.</param>
        /// <returns>A constant respresenting the requested slice.
        /// </returns>
        protected virtual Constant DoSlice(DataType dt, int offset)
        {
            var val = this.ToUInt64();
            ulong mask = Bits.Mask(offset, dt.BitSize);
            return Create(dt, (val & mask) >> offset);
        }

        /// <summary>
        /// Deposits the <paramref name="newBits"/> at bit offset
        /// <paramref name="bitOffset"/> in this constant.
        /// </summary>
        /// <param name="newBits">Bits to deposit.</param>
        /// <param name="bitOffset">Position at which to deposit them.</param>
        /// <returns>Resulting constant.</returns>
        protected virtual Constant DoDepositBits(Constant newBits, int bitOffset)
        {
            var val = this.ToUInt64();
            ulong mask = Bits.Mask(bitOffset, newBits.DataType.BitSize);
            var newVal = (val & ~mask) | ((newBits.ToUInt64() << bitOffset) & mask);
            return Constant.Create(this.DataType, newVal);
        }

        /// <summary>
        /// The CLR representation of the constant value.
        /// </summary>
        public abstract object GetValue();

        /// <summary>
        /// Get the hash code of the value. We do it this way to avoid incurring the
        /// cost of a boxing operation.
        /// </summary>
        /// <returns>Resulting hash value.</returns>
        public abstract int GetHashOfValue();


        /// <summary>
        /// Raise the floating point value <paramref name="b"/> to the integer power
        /// <paramref name="e"/>.
        /// </summary>
        /// <param name="b">Number to exponentiate.</param>
        /// <param name="e">Exponent to use.</param>
        /// <returns>Exponentiated value.</returns>
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

        /// <inheritdoc/>
        public override Expression Invert()
        {
            return new ConstantBool(DataType, IsZero);
        }

        /// <inheritdoc/>
        public override bool IsZero => IsIntegerZero;

        /// <summary>
        /// Returns true if this is a zero value that isn't
        /// a floating point value.
        /// </summary>
        public virtual bool IsIntegerZero
        {
            get
            {
                if (DataType.Domain == Domain.Real)
                    return false;
                return ToInt64() == 0;
            }
        }

        /// <summary>
        /// Returns true if this is a one value that isn't floating-point.
        /// </summary>
        public virtual bool IsIntegerOne
        {
            get
            {
                return !DataType.IsReal && ToInt64() == 1;
            }
        }

        /// <summary>
        /// Returns true if this is the maximum unsigned number
        /// of the data type given in <see cref="Expression.DataType"/>.
        /// </summary>
        public virtual bool IsMaxUnsigned
        {
            get
            {
                var mask = Bits.Mask(0, DataType.BitSize);
                return (this.ToUInt64() & mask) == mask;
            }
        }

        /// <summary>
        /// Returns true if this constant is negative.
        /// </summary>
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
                else 
                    return false;
            }
        }

        /// <summary>
        /// Returns true if this constant is floating point.
        /// </summary>
        public bool IsReal => DataType.Domain == Domain.Real;

        /// <summary>
        /// Returns true if this constant is not invalid.
        /// </summary>
        public virtual bool IsValid => true;

        /// <summary>
        /// Create a new Constant whose bits are the inverse of
        /// the bits of this Constant.
        /// </summary>
        public abstract Constant Complement();

        /// <summary>
        /// Deposits the <paramref name="newBits"/> at bit offset
        /// <paramref name="bitOffset"/> in this constant.
        /// </summary>
        /// <param name="newBits">Bits to deposit.</param>
        /// <param name="bitOffset">Position at which to deposit them.</param>
        /// <returns>Resulting constant.</returns>
        public Constant DepositBits(Constant newBits, int bitOffset)
        {
            if (bitOffset < 0 || newBits.DataType.BitSize + bitOffset > this.DataType.BitSize)
                throw new ArgumentException();
            if (!newBits.IsValid)
                return InvalidConstant.Create(this.DataType);
            return DoDepositBits(newBits, bitOffset);
        }

        /// <summary>
        /// Computes the negative of this constant.
        /// </summary>
        /// <returns>The negation of the constant.</returns>
        public virtual Constant Negate()
        {
            var p = DataType;
            var c = GetValue();
            if ((p.Domain & (Domain.SignedInt | Domain.UnsignedInt)) != 0)
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
            else if (p.BitSize <= 64)
            {
                ulong n = Convert.ToUInt64(c);
                c = Bits.Mask(0, p.BitSize);
                return Constant.Create(PrimitiveType.CreateWord(p.BitSize), ~n + 1);
            }
            else
                throw new InvalidOperationException($"Type {p} doesn't support negation.");
        }

        /// <summary>
        /// The constant value π.
        /// </summary>
        public static Constant Pi()
        {
            return Constant.Real64(Math.PI);
        }

        /// <summary>
        /// log(2) of 10.
        /// </summary>
        public static Constant Lg10()
        {
            return Constant.Real64(3.3219280948873623478703194294894);
        }

        /// <summary>
        /// log(2) of e.
        /// </summary>
        public static Constant LgE()
        {
            // log(2) of e.
            return Constant.Real64(1.4426950408889634073599246810019);
        }

        /// <summary>
        /// Natural logarithm of 2.
        /// </summary>
        public static Constant Ln2()
        {
            return Constant.Real64(0.69314718055994530941723212145818);
        }

        /// <summary>
        /// log(10) of 2
        /// </summary>
        public static Constant Log2()
        {
            return Constant.Real64(0.30102999566398119521373889472449);
        }

        /// <summary>
        /// Return a bit slice of this constant, treating the contents as bits.
        /// </summary>
        /// <param name="dt">Data type of the slice</param>
        /// <param name="offset">Bit offset from which to take the slice.</param>
        /// <returns>
        /// A possible new <see cref="Constant"/> having the 
        /// data type <paramref name="dt"/>.
        /// </returns>
        public virtual Constant Slice(DataType dt, int offset)
        {
            if (offset < 0 || offset + dt.BitSize > this.DataType.BitSize)
                throw new ArgumentException("Invalid bit size.", nameof(dt));
            return DoSlice(dt, offset);
        }

        string MachineOperand.ToString(MachineInstructionRendererOptions options)
        {
            var sr = new StringRenderer();
            RenderMachineOperand(sr, options);
            return sr.ToString();
        }

        void MachineOperand.Render(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            RenderMachineOperand(renderer, options);
        }

        private void RenderMachineOperand(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            renderer.BeginOperand();
            var s = AbstractMachineOperand.FormatValue(this);
            if (this.DataType.Domain == Domain.Pointer)
                renderer.WriteAddress(s, Address.FromConstant(this));
            else
                renderer.WriteString(s);
            renderer.EndOperand();
        }

        /// <summary>
        /// Coerces this constant to a CLR boolean.
        /// </summary>
        public virtual bool ToBoolean()
        {
            return Convert.ToBoolean(GetValue());
        }

        /// <summary>
        /// Coerces this constant to a CLR 8-bit unsigned integer.
        /// </summary>
        public abstract byte ToByte();

        /// <summary>
        /// Coerces this constant to a CLR 16-bit unsigned integer.
        /// </summary>
        public abstract ushort ToUInt16();

        /// <summary>
        /// Coerces this constant to a CLR 32-bit unsigned integer.
        /// </summary>
        public abstract uint ToUInt32();

        /// <summary>
        /// Coerces this constant to a CLR 64-bit unsigned integer.
        /// </summary>
        public abstract ulong ToUInt64();

        /// <summary>
        /// Coerces this constant to a CLR 16-bit signed integer.
        /// </summary>
        public abstract short ToInt16();

        /// <summary>
        /// Coerces this constant to a CLR 32-bit signed integer.
        /// </summary>
        public abstract int ToInt32();

        /// <summary>
        /// Coerces this constant to a CLR 64-bit signed integer.
        /// </summary>
        public abstract long ToInt64();

        /// <summary>
        /// Coerces this constant to a CLR <see cref="BigInteger"/>.
        /// </summary>
        public abstract BigInteger ToBigInteger();

        /// <summary>
        /// Coerces this constant to a CLR double precision floating point number.
        /// </summary>
        public double ToDouble()
        {
            return Convert.ToDouble(GetValue());
        }

        /// <summary>
        /// Coerces this constant to a CLR single precision floating point number.
        /// </summary>
        public virtual float ToFloat()
        {
            return Convert.ToSingle(GetValue());
        }

        /// <summary>
        /// Coerces this constant to a CLR double precision floating point number.
        /// </summary>
        public virtual double ToReal64()
        {
            return Convert.ToDouble(GetValue());
        }

        /// <summary>
        /// Creates a constant representing the boolean value 'true'.
        /// </summary>
        public static Constant True()
        {
            return new ConstantBool(PrimitiveType.Bool, true);
        }

        /// <summary>
        /// Creates a boolean constant.
        /// </summary>
        /// <param name="f">Boolean value.</param>
        public static Constant Bool(bool f)
        {
            return new ConstantBool(PrimitiveType.Bool, f);
        }

        /// <summary>
        /// Creates a constant representing an 8-bit signed integer.
        /// </summary>
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

        /// <summary>
        /// Creates a constant representing an 16-bit signed integer.
        /// </summary>
        public static Constant Int16(short s)
        {
            return new ConstantInt16(PrimitiveType.Int16, s);
        }

        /// <summary>
        /// Creates a constant representing an 32-bit signed integer.
        /// </summary>
        public static Constant Int32(int i)
        {
            return new ConstantInt32(PrimitiveType.Int32, i);
        }

        /// <summary>
        /// Creates a constant representing an 64-bit signed integer.
        /// </summary>
        public static Constant Int64(long l)
        {
            return new ConstantInt64(PrimitiveType.Int64, l);
        }

        /// <summary>
        /// Creates a constant representing an 32-bit floating point number.
        /// </summary>
        public static Constant Real32(float f)
        {
            return new ConstantReal32(PrimitiveType.Real32, f);
        }

        /// <summary>
        /// Creates a constant representing an 64-bit floating point number.
        /// </summary>
        public static Constant Real64(double d)
        {
            return new ConstantReal64(PrimitiveType.Real64, d);
        }

        /// <summary>
        /// Creates a constant representing an 80-bit floating point number.
        /// </summary>
        public static Constant Real80(Float80 f80)
        {
            return new ConstantReal80(PrimitiveType.Real80, f80);
        }

        /// <summary>
        /// Creates a constant representing a 16-bit unsigned integer.
        /// </summary>
        public static Constant UInt16(ushort u)
        {
            return new ConstantUInt16(PrimitiveType.UInt16, u);
        }

        /// <summary>
        /// Creates a constant representing a 32-bit unsigned integer.
        /// </summary>
        public static Constant UInt32(uint w)
        {
            return new ConstantUInt32(PrimitiveType.UInt32, w);
        }

        /// <summary>
        /// Creates a constant representing a 64-bit unsigned integer.
        /// </summary>
        public static Constant UInt64(ulong ul)
        {
            return new ConstantUInt64(PrimitiveType.UInt64, ul);
        }

        /// <summary>
        /// Creates a constant representing an 16-bit word.
        /// </summary>
        public static Constant Word16(ushort n)
        {
            return new ConstantUInt16(PrimitiveType.Word16, n);
        }

        /// <summary>
        /// Creates a constant representing an 32-bit word.
        /// </summary>
        public static Constant Word32(int n)
        {
            return new ConstantUInt32(PrimitiveType.Word32, (uint) n);
        }

        /// <summary>
        /// Creates a constant representing an 32-bit word.
        /// </summary>
        public static Constant Word32(uint n)
        {
            return new ConstantUInt32(PrimitiveType.Word32, n);
        }

        /// <summary>
        /// Creates a constant representing an 64-bit word.
        /// </summary>
        public static Constant Word64(long n)
        {
            return new ConstantUInt64(PrimitiveType.Word64, (ulong) n);
        }

        /// <summary>
        /// Creates a constant representing an 64-bit word.
        /// </summary>
        public static Constant Word64(ulong n)
        {
            return new ConstantUInt64(PrimitiveType.Word64, n);
        }

        /// <summary>
        /// Creates a constant representing a word of the given size.
        /// </summary>
        /// <param name="bitSize">Bit size of the word.</param>
        /// <param name="value">CLR value to encapsulate.</param>
        public static Constant Word(int bitSize, long value)
        {
            return Create(PrimitiveType.CreateWord(bitSize), value);
        }

        /// <summary>
        /// Creates a constant representing a word of the given size.
        /// </summary>
        /// <param name="bitSize">Bit size of the word.</param>
        /// <param name="value">CLR value to encapsulate.</param>
        public static Constant Word(int bitSize, ulong value)
        {
            return Create(PrimitiveType.CreateWord(bitSize), value);
        }

        /// <summary>
        /// Create a zero-valued constant of the given data type.
        /// </summary>
        /// <param name="dataType">Datatype to give the constant.</param>
        public static Constant Zero(DataType dataType)
        {
            return Constant.Create(dataType, 0);
        }

        /// <summary>
        /// Creates a string literal of the given type.
        /// </summary>
        /// <param name="str">String literal.</param>
        /// <param name="strType">Data type to use for the string constant.</param>
        public static StringConstant String(string str, StringType strType)
        {
            return new StringConstant(strType, str);
        }
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

        public override bool ToBoolean()
        {
            return value;
        }

        public override byte ToByte()
        {
            return value ? (byte) 1 : (byte) 0;
        }

        public override ushort ToUInt16()
        {
            return value ? (ushort) 1u : (ushort) 0u;
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
            return value ? (short) 1 : (short) 0;
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
            return new ConstantSByte(DataType, (sbyte) ~value);
        }

        public override object GetValue()
        {
            return value;
        }

        public override int GetHashOfValue()
        {
            return value.GetHashCode();
        }

        public override byte ToByte()
        {
            return (byte) value;
        }

        public override ushort ToUInt16()
        {
            return (ushort) (short) value;
        }

        public override uint ToUInt32()
        {
            return (uint) (int) value;
        }

        public override ulong ToUInt64()
        {
            return (ulong) (long) value;
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
            return new ConstantChar(DataType, (char) ~value);
        }

        public override object GetValue()
        {
            return value;
        }

        public override int GetHashOfValue()
        {
            return value.GetHashCode();
        }

        public override byte ToByte()
        {
            return (byte) value;
        }

        public override ushort ToUInt16()
        {
            return (ushort) value;
        }

        public override uint ToUInt32()
        {
            return (uint) value;
        }

        public override ulong ToUInt64()
        {
            return (ulong) value;
        }

        public override short ToInt16()
        {
            return (short) value;
        }

        public override int ToInt32()
        {
            return (int) value;
        }

        public override long ToInt64()
        {
            return (long) value;
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
            return new ConstantByte(DataType, (byte) ~value);
        }


        public override object GetValue()
        {
            return value;
        }

        public override int GetHashOfValue()
        {
            return value.GetHashCode();
        }

        public override Expression Invert()
        {
            return new ConstantByte(DataType, value == 0 ? (byte)1 : (byte)0);
        }

        public override byte ToByte()
        {
            return value;
        }

        public override ushort ToUInt16()
        {
            return (ushort) value;
        }

        public override uint ToUInt32()
        {
            return (uint) value;
        }

        public override ulong ToUInt64()
        {
            return value;
        }

        public override short ToInt16()
        {
            return (sbyte) value;
        }

        public override int ToInt32()
        {
            return (sbyte) value;
        }

        public override long ToInt64()
        {
            return (sbyte) value;
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
            return new ConstantInt16(DataType, (short) ~value);
        }

        public override object GetValue()
        {
            return value;
        }

        public override int GetHashOfValue()
        {
            return value.GetHashCode();
        }

        public override byte ToByte()
        {
            return (byte) value;
        }

        public override ushort ToUInt16()
        {
            return (ushort) value;
        }

        public override uint ToUInt32()
        {
            return (uint) value;
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
            return new ConstantUInt16(DataType, (ushort) ~value);
        }

        public override object GetValue()
        {
            return value;
        }

        public override int GetHashOfValue()
        {
            return value.GetHashCode();
        }

        public override byte ToByte()
        {
            return (byte) value;
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
            return (short) value;
        }

        public override int ToInt32()
        {
            return (short) value;
        }

        public override long ToInt64()
        {
            return (short) value;
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

        public override byte ToByte()
        {
            return (byte) value;
        }

        public override ushort ToUInt16()
        {
            return (ushort) value;
        }

        public override uint ToUInt32()
        {
            return (uint) value;
        }

        public override ulong ToUInt64()
        {
            return (uint) value;
        }

        public override short ToInt16()
        {
            return (short) value;
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

        public override Expression Invert()
        {
            return Constant.Bool(value == 0);
        }

        public override byte ToByte()
        {
            return (byte) value;
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
            return (short) value;
        }

        public override int ToInt32()
        {
            return (int) value;
        }

        public override long ToInt64()
        {
            return (int) value;
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

        public override byte ToByte()
        {
            return (byte) value;
        }

        public override ushort ToUInt16()
        {
            return (ushort) value;
        }

        public override uint ToUInt32()
        {
            return (uint) value;
        }

        public override ulong ToUInt64()
        {
            return (ulong) value;
        }

        public override short ToInt16()
        {
            return (short) value;
        }

        public override int ToInt32()
        {
            return (int) value;
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

        public override Constant Negate()
        {
            return new ConstantUInt64(this.DataType, ~value + 1);
        }

        public override byte ToByte()
        {
            return (byte) value;
        }

        public override ushort ToUInt16()
        {
            return (ushort) value;
        }

        public override uint ToUInt32()
        {
            return (uint) value;
        }

        public override ulong ToUInt64()
        {
            return value;
        }

        public override short ToInt16()
        {
            return (short) value;
        }

        public override int ToInt32()
        {
            return (int) value;
        }

        public override long ToInt64()
        {
            return (long) value;
        }

        public override BigInteger ToBigInteger() => value;

    }

    /// <summary>
    /// Abstract base class for floating point constants.
    /// </summary>
    public abstract class ConstantReal : Constant
    {
        /// <summary>
        /// Initializes the data type property of this class.
        /// </summary>
        /// <param name="dt">Datatype of this property.</param>
        protected ConstantReal(DataType dt) : base(dt)
        {
        }

        /// <summary>
        /// Creates a real constant of the size <paramref name="dt"/>.
        /// </summary>
        /// <param name="dt">Requested data type.</param>
        /// <param name="value">Floating point value to encapsulate.</param>
        /// <returns>Resulting constant.</returns>
        public static Constant Create(DataType dt, double value)
        {
            var pt = PrimitiveType.Create(Domain.Real, dt.BitSize);
            switch (dt.BitSize)
            {
            case 16: return new ConstantReal16(pt, value);
            case 32: return new ConstantReal32(pt, (float) value);
            case 48: return new ConstantReal64(pt, value);
            case 64: return new ConstantReal64(pt, value);
            }
            // Unsupported floating point constant sizes cannot be represented yet.
            return InvalidConstant.Create(dt);
        }

        /// <inheritdoc/>
        public override bool IsIntegerOne => false;

        /// <inheritdoc/>
        public override sealed bool IsMaxUnsigned => false;

    }

    /// <summary>
    /// Constant encapsulating 16-bit floating point numbers.
    /// </summary>
    public class ConstantReal16 : ConstantReal
    {
        private readonly Half value;

        /// <summary>
        /// Constructs a constant of the given data type and value.
        /// </summary>
        /// <param name="dt">Data type to use.</param>
        /// <param name="value">64-bit floating point number to convert 
        /// to 16-bits.</param>
        public ConstantReal16(DataType dt, double value)
            : base(dt)
        {
            this.value = (Half) value;
        }

        /// <summary>
        /// Constructs a constant of the given data type and value.
        /// </summary>
        /// <param name="dt">Data type to use.</param>
        /// <param name="value">16-bit floating point number to
        /// encapsulate.</param>
        public ConstantReal16(DataType dt, Half value)
           : base(dt)
        {
            this.value = value;
        }

        /// <inheritdoc/>
        public override Expression CloneExpression()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override Constant Complement()
        {
            throw new NotSupportedException("Cannot complement a real value.");
        }

        /// <inheritdoc/>
        protected override Constant DoSlice(DataType dt, int offset)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override object GetValue()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override int GetHashOfValue()
        {
            return value.GetHashCode();
        }

        /// <inheritdoc/>
        public override bool IsValid => !Half.IsNaN(value);

        /// <inheritdoc/>
        public override Constant Negate()
        {
            return new ConstantReal16(this.DataType, -((float)value));
        }

        /// <inheritdoc/>
        public override byte ToByte()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override ushort ToUInt16()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override uint ToUInt32()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override ulong ToUInt64()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override short ToInt16()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override int ToInt32()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override long ToInt64()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override float ToFloat()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override double ToReal64()
        {
            return (double) value;
        }

        /// <inheritdoc/>
        public override BigInteger ToBigInteger() => throw new InvalidCastException();
    }

    /// <summary>
    /// Constant encapsulating 32-bit floating point numbers.
    /// </summary>
    internal class ConstantReal32 : ConstantReal
    {
        private readonly float value;

        public ConstantReal32(DataType dt, float value)
            : base(dt)
        {
            this.value = value;
        }

        public static ConstantReal32 CreateFromBits(DataType dt, int bits)
        {
            var value = BitConverter.Int32BitsToSingle(bits);
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
            var bits = (uint) BitConverter.SingleToInt32Bits(this.value);
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

    /// <summary>
    /// Constant encapsulating 80-bit floating point numbers.
    /// </summary>
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

    /// <summary>
    /// Constant encapsulating 64-bit floating point numbers.
    /// </summary>
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
}
