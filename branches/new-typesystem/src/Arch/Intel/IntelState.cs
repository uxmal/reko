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
using System.Collections;

namespace Decompiler.Arch.Intel
{
	public class IntelState : ProcessorState
	{
		private ulong [] regs;
		private bool [] valid;
		private Stack stack;

		public IntelState()
		{
			regs = new ulong[(int)Registers.Max];
			valid = new bool[(int)Registers.Max];
			stack = new Stack();
		}

		public IntelState(IntelState st)
		{
			regs = (ulong []) st.regs.Clone();
			valid = (bool []) st.valid.Clone();
			stack = (Stack) st.stack.Clone();
		}

		public Address AddressFromSegOffset(MachineRegister seg, uint offset)
		{
			Value v = Get(seg);
			if (v.IsValid)
			{
				return new Address(v.Word, offset & 0xFFFF);
			}
			else
				return null;
		}

		public Address AddressFromSegReg(MachineRegister seg, IntelRegister reg)
		{
			Value v = Get(reg);
			if (v.IsValid)
			{
				return AddressFromSegOffset(seg, v.Unsigned);
			}
			else 
				return null;
		}

		public override object Clone()
		{
			return new IntelState(this);
		}

		public override void Set(MachineRegister reg, Value v)
		{
			if (!v.IsValid)
			{
				valid[reg.Number] = false;
			}
			else
			{
				reg.SetRegisterFileValues(regs, v.Unsigned, valid);
			}
		}

		public override void SetInstructionPointer(Address addr)
		{
			Set(Registers.cs, new Value(PrimitiveType.Word16, addr.seg));
		}

		public override Value Get(MachineRegister reg)
		{
			if (valid[reg.Number])
				return new Value(reg.DataType, (uint) regs[reg.Number]);
			else
				return Value.Invalid;
		}

		public void Push(PrimitiveType t, Value v)
		{
			switch (t.Size)
			{
			default:
				throw new ArgumentOutOfRangeException();
			case 1:
			case 2:
				stack.Push(v);
				break;
			case 4:
				if (!v.IsValid)
				{
					stack.Push(v);
					stack.Push(v);
				}				 
				else
				{
					stack.Push(new Value(PrimitiveType.Word16, v.Unsigned >> 16));
					stack.Push(new Value(PrimitiveType.Word16, v.Unsigned & 0xFFFF));
				}
				break;
			}
		}

		public Value Pop(PrimitiveType t)
		{
			try
			{
				switch (t.Size)
				{
				default:
					throw new ArgumentOutOfRangeException();
				case 2:
					if (stack.Count < 1)
					{
						System.Diagnostics.Debug.WriteLine("bad pop");
						return Value.Invalid;
					}
					return (Value) stack.Pop();
				case 4:
					if (stack.Count < 2)
					{
						System.Diagnostics.Debug.WriteLine("bad pop");
						return Value.Invalid;
					}
					Value v = (Value) stack.Pop();
					Value v2 = (Value) stack.Pop();
					if (v.IsValid && v2.IsValid)
					{
						return new Value(t, (v.Unsigned & 0x0000FFFF) | (v2.Unsigned << 16));
					}
					else
					{
						return Value.Invalid;
					}
				}
			}
			catch (InvalidOperationException)
			{
				//$NYI:				Log.Warn("Stack underflow");
				return Value.Invalid;
			}
		}
	}
}
