#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Types;
using System;
using System.IO;
using System.Text;

namespace Reko.Core.Machine
{
    /// <summary>
    /// Abstraction of a processor instruction operand.
    /// </summary>
	public abstract class MachineOperand
	{
        public PrimitiveType Width { get { return width; } set { width = value; } }
        private PrimitiveType width;

		protected MachineOperand(PrimitiveType width)
		{
			this.width = width;
		}

        public sealed override string ToString()
        {
            return ToString(false);
        }

		public string ToString(bool fExplicit)
		{
            var sr = new StringRenderer();
            Write(fExplicit, sr);
			return sr.ToString();
		}

        public abstract void Write(bool fExplicit, MachineInstructionWriter writer);

		public static string FormatSignedValue(Constant c)
		{
			string s = "+";
			int tmp = c.ToInt32();
			if (tmp < 0)
			{
				s = "-";
				tmp = -tmp;
			}
			return s + tmp.ToString(FormatString(c.DataType));
		}

        public static string FormatValue(Constant c)
        {
            var pt = (PrimitiveType)c.DataType;
            if (pt.Domain == Domain.SignedInt)
            {
                return FormatSignedValue(c);
            }
            else if (pt.Domain == Domain.Real)
            {
                return c.ToReal64().ToString("G");
            }
            else
                return FormatUnsignedValue(c);
        }

		private static string FormatString(DataType dt)
		{
			switch (dt.Size)
			{
			case 1: return "X2";
			case 2: return "X4";
			case 4: return "X8";
			case 8: return "X8";
			default: throw new InvalidOperationException();
			}
		}

		public static string FormatUnsignedValue(Constant c)
		{
			return c.ToUInt64().ToString(FormatString(c.DataType));
		}
	}

    /// <summary>
    /// Represents an immediate constant value used by a MachineInstruction.
    /// </summary>
	public class ImmediateOperand : MachineOperand
	{
		private Constant value;

		public ImmediateOperand(Constant c) : base((PrimitiveType)c.DataType)
		{
			value = c;
		}

        public override void Write(bool fExplicit, MachineInstructionWriter writer)
        {
            var s = FormatValue(value);
            var pt = value.DataType as PrimitiveType;
            if (pt != null)
            {
                if (pt.Domain == Domain.Pointer)
                    writer.WriteAddress(s, Address.FromConstant(value));
                else
                    writer.Write(s);
            }
            else if (value.DataType is Pointer)
                writer.WriteAddress(s, Address.FromConstant(value));
            else 
                writer.Write(s);
        }

		public Constant Value
		{
			get { return value; }
		}

        public static ImmediateOperand Byte(byte value)
        {
            return new ImmediateOperand(Constant.Byte(value));
        }

        public static ImmediateOperand Word32(int value)
        {
            return new ImmediateOperand(Constant.Word32(value));
        }

        public static ImmediateOperand Word32(uint value) { return Word32((int) value); }

        public static ImmediateOperand Word64(long value)
        {
            return new ImmediateOperand(Constant.Word64(value));
        }

        public static ImmediateOperand Word64(ulong value) { return Word64((long) value); }


        public static ImmediateOperand Int32(int value)
        {
            return new ImmediateOperand(Constant.Int32(value));
        }

        public static MachineOperand Int16(short value)
        {
            return new ImmediateOperand(Constant.Int16(value));
        }

        public static MachineOperand Word16(ushort value)
        {
            return new ImmediateOperand(Constant.Word16(value));
        }
    }

    /// <summary>
    /// Represents a machine address.
    /// </summary>
    public class AddressOperand : MachineOperand
    {
        public Address Address;

        protected AddressOperand(Address a, PrimitiveType type)
            : base(type)
        {
            if (a == null)
                throw new ArgumentNullException("a");
            Address = a;
        }

        public static AddressOperand Ptr16(ushort a)
        {
            return new AddressOperand(Address.Ptr16(a), PrimitiveType.Ptr16);
        }

        public static AddressOperand Ptr32(uint a)
        {
            return new AddressOperand(Address.Ptr32(a), PrimitiveType.Pointer32);
        }

        public static AddressOperand Ptr64(ulong a)
        {
            return new AddressOperand(Address.Ptr64(a), PrimitiveType.Pointer64);
        }

        public override void Write(bool fExplicit, MachineInstructionWriter writer)
        {
            writer.WriteAddress(Address.ToString(), Address);
        }
    }

    /// <summary>
    /// Represents a FPU operand.
    /// </summary>
	public class FpuOperand : MachineOperand
	{
		private int fpuReg;

		public FpuOperand(int f) : base(PrimitiveType.Real64)
		{
			fpuReg = f;
		}

		public int StNumber
		{
			get { return fpuReg; }
		}

		public override void Write(bool fExplicit, MachineInstructionWriter writer)
		{
			writer.Write("st(" + fpuReg + ")");
		}
	}
}


