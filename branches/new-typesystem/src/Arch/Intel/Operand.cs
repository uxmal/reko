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

using Decompiler.Core;
using Decompiler.Core.Types;
using System;
using System.Text;

namespace Decompiler.Arch.Intel
{
	public abstract class Operand
	{
		public PrimitiveType Width;

		protected Operand(PrimitiveType width)
		{
			Width = width;
		}

		public virtual string ToString(bool fExplicit)
		{
			return ToString();
		}
	}


	public interface OperandVisitor
	{
		void VisitRegisterOperand(RegisterOperand regOp);
		void VisitImmediateOperand(ImmediateOperand immOp);
		void VisitMemoryOperand(MemoryOperand memOp);
	}

	public class RegisterOperand : Operand
	{
		private IntelRegister reg;
		
		public RegisterOperand(IntelRegister reg) :
			base(reg.DataType)
		{
			this.reg = reg;
		}

		public IntelRegister Register
		{
			get { return reg; }
		}

		public override string ToString()
		{
			return reg.ToString();
		}
	}

	public class ImmediateOperand : Operand
	{
		public Value val;

		public ImmediateOperand(PrimitiveType  t, int v) : base(t)
		{
			val = new Value(t, v);
		}

		public ImmediateOperand(PrimitiveType t, uint v) : base(t)
		{
			val = new Value(t, v);
		}

		public ImmediateOperand(Value v) : base(v.Width)
		{
			val = v;
		}

		public byte Byte
		{
			get { return val.Byte; }
		}

		public int Signed
		{
			get { return val.Signed; }
		}

		public override string ToString()
		{
			return val.ToString();
		}

		public ushort Word
		{
			get { return val.Word; }
		}
	}

	public class AddressOperand : Operand
	{
		public Address addr;

		public AddressOperand(Address a) : base(PrimitiveType.Pointer32)	//$BUGBUG: P6 pointers?
		{
			addr = a;
		}

		public override string ToString()
		{
			return "far " + addr.ToString();
		}
	}

	public class FpuOperand : Operand
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

		public override string ToString()
		{
			return "st(" + fpuReg + ")";
		}
	}
}


