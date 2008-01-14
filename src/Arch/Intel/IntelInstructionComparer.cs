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

using System;

using IComparer = System.Collections.IComparer;
#if VS2003 || MONO
using IHashCodeProvider = System.Collections.IHashCodeProvider;
#else
using IEqualityComparer = System.Collections.IEqualityComparer;
#endif

namespace Decompiler.Arch.Intel
{
	/// <summary>
	/// Compares pairs of Intel instructions -- for equality only.
	/// </summary>
	/// <remarks>
	/// Used by the InstructionTrie class.
	/// </remarks>
	/// 
	public class IntelInstructionComparer : 
		IComparer,
#if VS2003 || MONO
		IHashCodeProvider
#else
		IEqualityComparer
#endif
	{
		private int CompareOperands(Operand opA, Operand opB)
		{
			if (!opA.GetType().IsAssignableFrom(opB.GetType()))
				return -1;

			RegisterOperand regOpA = opA as RegisterOperand;
			if (regOpA != null)
			{
				RegisterOperand regOpB = (RegisterOperand) opB;
				return (int) regOpB.Register.Number - (int) regOpA.Register.Number;
			}
			ImmediateOperand immOpA = opA as ImmediateOperand;
			if (immOpA != null)
			{
				ImmediateOperand immOpB = (ImmediateOperand) opB;
				return 0;			// disregard immediate values.
			}
			throw new NotImplementedException("NYI");
		}

		/// <summary>
		/// Implementation of IComparer.Compare. In reality, 
		/// </summary>
		/// <param name="oInstrA"></param>
		/// <param name="oInstrB"></param>
		/// <returns></returns>
		public int Compare(object oInstrA, object oInstrB)
		{
			IntelInstruction instrA = (IntelInstruction) oInstrA;
			IntelInstruction instrB = (IntelInstruction) oInstrB;
			if (instrA.code != instrB.code)
				return (int) instrB.code - (int) instrA.code;
			if (instrA.cOperands != instrB.cOperands)
				return instrB.cOperands - instrA.cOperands;;

			int retval = 0;
			if (instrA.cOperands > 0)
			{
				retval = CompareOperands(instrA.op1, instrB.op1);
				if (retval == 0 && instrA.cOperands > 1)
				{
					retval = CompareOperands(instrA.op2, instrB.op2);
					if (retval == 0 && instrA.cOperands > 2)
					{
						retval = CompareOperands(instrA.op3, instrB.op3);
					}
				}
			}
			return retval;
		}

		private int GetHashCodeInner(object obj)
		{
			IntelInstruction instr = (IntelInstruction) obj;
			int hash = instr.code.GetHashCode();
			hash = hash * 47 + instr.cOperands.GetHashCode();

			if (instr.cOperands > 0)
			{
				hash = hash * 23 + GetHashCode(instr.op1);
				if (instr.cOperands > 1)
				{
					hash = hash * 17 + GetHashCode(instr.op2);
					if (instr.cOperands > 2)
					{
						hash = hash * 13 + GetHashCode(instr.op3);
					}
				}
			}
			return hash;
		}


		private int GetHashCode(Operand op)
		{
			RegisterOperand regOp = op as RegisterOperand;
			if (regOp != null)
			{
				return regOp.Register.Number;
			}
			ImmediateOperand immOp = op as ImmediateOperand;
			if (immOp != null)
			{
				return 0;			// disregard immediate values.
			}
			throw new NotImplementedException("NYI");
		}


#if VS2003 || MONO

		#region IHashCodeProvider Members

		public int GetHashCode(object obj)
		{
			return GetHashCodeInner(obj);
		}

		#endregion
#else
		#region IEqualityComparer Members

		bool  IEqualityComparer.Equals(object x, object y)
		{
			return Compare(x, y) == 0;
		}

		int  IEqualityComparer.GetHashCode(object obj)
		{
			return GetHashCodeInner(obj);
		}
		#endregion
#endif
	}
}
