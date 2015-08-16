#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
using Reko.Core.Machine;
using System;
using System.Collections;

namespace Reko.Arch.X86
{
	/// <summary>
	/// Compares pairs of Intel instructions -- for equality only.
	/// </summary>
	/// <remarks>
	/// Used by the InstructionTrie class.
	/// </remarks>
	/// 
	public class X86InstructionComparer : 
		InstructionComparer<IntelInstruction>
	{
        public X86InstructionComparer(Normalize norm) 
            : base(norm)
        {
        }

		public bool CompareOperands(MachineOperand opA, MachineOperand opB)
		{
			if (opA.GetType() != opB.GetType())
				return false;

			RegisterOperand regOpA = opA as RegisterOperand;
			if (regOpA != null)
			{
				RegisterOperand regOpB = (RegisterOperand) opB;
                return NormalizeRegisters || regOpA.Register == regOpB.Register;
			}
			ImmediateOperand immOpA = opA as ImmediateOperand;
			if (immOpA != null)
			{
				return NormalizeConstants || immOpA.Value.Equals(opB);			// disregard immediate values.
			}
			throw new NotImplementedException("NYI");
		}


		/// <summary>
		/// Implementation of IComparer.Compare. In reality, 
		/// </summary>
		/// <param name="oInstrA"></param>
		/// <param name="oInstrB"></param>
		/// <returns></returns>
		public override bool CompareOperands(IntelInstruction instrA, IntelInstruction instrB)
		{
			if (instrA.code != instrB.code)
				return false;
			if (instrA.Operands != instrB.Operands)
				return false;

			bool retval = true;
			if (instrA.Operands > 0)
			{
				retval = CompareOperands(instrA.op1, instrB.op1);
				if (retval && instrA.Operands > 1)
				{
					retval = CompareOperands(instrA.op2, instrB.op2);
					if (retval && instrA.Operands > 2)
					{
						retval = CompareOperands(instrA.op3, instrB.op3);
					}
				}
			}
			return retval;
		}

        public override int GetOperandsHash(IntelInstruction instr)
        {
			int hash = instr.Operands.GetHashCode();
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
				return base.NormalizeRegisters
                    ? 1 
                    : regOp.Register.Number;
			}
			ImmediateOperand immOp = op as ImmediateOperand;
			if (immOp != null)
			{
                return base.NormalizeConstants
                    ? 1 // disregard immediate values.
                    : immOp.GetHashCode();
			}
			throw new NotImplementedException();
		}
	}
}
