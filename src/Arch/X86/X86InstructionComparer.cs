#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
	public class X86InstructionComparer : InstructionComparer
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
                var immOpB = (ImmediateOperand) opB;
				return NormalizeConstants || immOpA.Value.Equals(immOpB.Value);			// disregard immediate values.
			}
            var addrOpA = opA as AddressOperand;
            if (addrOpA != null)
            {
                var addrOpB = (AddressOperand)opB;
                return NormalizeConstants || addrOpA.Address == addrOpB.Address;
            }
            var memOpA = opA as MemoryOperand;
            if (memOpA != null)
            {
                var memOpB = (MemoryOperand)opB;
                if (!base.CompareRegisters(memOpA.Base, memOpB.Base))
                    return false;
                if (!base.CompareRegisters(memOpA.Index, memOpB.Index))
                    return false;
                if (memOpA.Scale != memOpB.Scale)
                    return false;
                return base.CompareValues(memOpA.Offset, memOpB.Offset);
            }
            var fpuA = opA as FpuOperand;
            if (fpuA != null)
            {
                var fpuB = opB as FpuOperand;
                return NormalizeRegisters || fpuA.StNumber == fpuB.StNumber;
            }
			throw new NotImplementedException(string.Format("NYI: {0}", opA.GetType()));
		}


		/// <summary>
		/// Implementation of IComparer.Compare. In reality, 
		/// </summary>
		/// <param name="oInstrA"></param>
		/// <param name="oInstrB"></param>
		/// <returns></returns>
		public override bool CompareOperands(MachineInstruction a, MachineInstruction b)
		{
            var instrA = (X86Instruction)a;
            var instrB = (X86Instruction)b;

			if (instrA.Mnemonic != instrB.Mnemonic)
				return false;
			if (instrA.Operands.Length != instrB.Operands.Length)
				return false;

			bool retval = true;
			if (instrA.Operands.Length > 0)
			{
				retval = CompareOperands(instrA.Operands[0], instrB.Operands[0]);
				if (retval && instrA.Operands.Length > 1)
				{
					retval = CompareOperands(instrA.Operands[1], instrB.Operands[1]);
					if (retval && instrA.Operands.Length > 2)
					{
						retval = CompareOperands(instrA.Operands[2], instrB.Operands[2]);
					}
				}
			}
			return retval;
		}

        public override int GetOperandsHash(MachineInstruction inst)
        {
            var instr = (X86Instruction)inst;
			int hash = instr.Operands.Length.GetHashCode();
			if (instr.Operands.Length > 0)
			{
				hash = hash * 23 + GetHashCode(instr.Operands[0]);
				if (instr.Operands.Length > 1)
				{
					hash = hash * 17 + GetHashCode(instr.Operands[1]);
					if (instr.Operands.Length > 2)
					{
						hash = hash * 13 + GetHashCode(instr.Operands[2]);
					}
				}
			}
			return hash;
		}

		private int GetHashCode(MachineOperand op)
		{
            int h;
			RegisterOperand regOp = op as RegisterOperand;
			if (regOp != null)
			{
				return base.GetRegisterHash(regOp.Register);
			}
			ImmediateOperand immOp = op as ImmediateOperand;
			if (immOp != null)
			{
                return base.GetConstantHash(immOp.Value);
			}
            var addrOp = op as AddressOperand;
            if (addrOp != null)
            {
                return base.NormalizeConstants
                    ? 1
                    : addrOp.Address.GetHashCode();
            }
            var memOp = op as MemoryOperand;
            if (memOp != null)
            {
                h = 0;
                if (memOp.Base != null)
                {
                    h = base.GetRegisterHash(memOp.Base);
                }
                if (memOp.Index != null)
                {
                    h = 13 * h ^ base.GetRegisterHash(memOp.Index);
                    h = 17 * h ^ memOp.Scale;
                }
                if (memOp.Offset != null)
                {
                    h = 23 * h ^ GetConstantHash(memOp.Offset);
                }
                if (memOp.SegOverride != null)
                {
                    h = 29 * h ^ GetRegisterHash(memOp.SegOverride);
                }
                return h;
            }
            var fpuOp = op as FpuOperand;
            if (fpuOp != null)
            {
                return h = 59 * fpuOp.StNumber.GetHashCode();
            }
			throw new NotImplementedException("Unhandled operand type: " + op.GetType().FullName);
		}
	}
}
