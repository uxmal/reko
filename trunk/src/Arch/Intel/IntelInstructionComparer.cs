/* 
 * Copyright (C) 1999-2009 John Källén.
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
using Decompiler.Core.Machine;
using System;
using System.Collections;

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
		IEqualityComparer
	{
		private int CompareOperands(MachineOperand opA, MachineOperand opB)
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
            IntelInstruction instrA = (IntelInstruction)oInstrA;
            IntelInstruction instrB = (IntelInstruction)oInstrB;
			if (instrA.code != instrB.code)
				return (int) instrB.code - (int) instrA.code;
			if (instrA.Operands != instrB.Operands)
				return instrB.Operands - instrA.Operands;;

			int retval = 0;
			if (instrA.Operands > 0)
			{
				retval = CompareOperands(instrA.op1, instrB.op1);
				if (retval == 0 && instrA.Operands > 1)
				{
					retval = CompareOperands(instrA.op2, instrB.op2);
					if (retval == 0 && instrA.Operands > 2)
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
			hash = hash * 47 + instr.Operands.GetHashCode();

			if (instr.Operands > 0)
			{
				hash = hash * 23 + GetHashCode(instr.op1);
				if (instr.Operands > 1)
				{
					hash = hash * 17 + GetHashCode(instr.op2);
					if (instr.Operands > 2)
					{
						hash = hash * 13 + GetHashCode(instr.op3);
					}
				}
			}
			return hash;
		}


		private int GetHashCode(MachineOperand op)
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
			throw new NotImplementedException();
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
