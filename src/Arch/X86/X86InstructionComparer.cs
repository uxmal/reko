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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using System;

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

            if (opA is RegisterStorage regOpA)
            {
                RegisterStorage regOpB = (RegisterStorage) opB;
                return NormalizeRegisters || regOpA == regOpB;
            }
            if (opA is Constant immOpA)
            {
                var immOpB = (Constant) opB;
                return NormalizeConstants || immOpA.Equals(immOpB);         // disregard immediate values.
            }
            if (opA is Address addrOpA)
            {
                var addrOpB = (Address) opB;
                return NormalizeConstants || addrOpA == addrOpB;
            }
            if (opA is MemoryOperand memOpA)
            {
                var memOpB = (MemoryOperand) opB;
                if (!base.CompareRegisters(memOpA.Base, memOpB.Base))
                    return false;
                if (!base.CompareRegisters(memOpA.Index, memOpB.Index))
                    return false;
                if (memOpA.Scale != memOpB.Scale)
                    return false;
                return base.CompareValues(memOpA.Offset, memOpB.Offset);
            }
            if (opA is FpuOperand fpuA)
            {
                var fpuB = (FpuOperand) opB;
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
            return op switch
            {
                RegisterStorage regOp => base.GetRegisterHash(regOp),
                Constant immOp => base.GetConstantHash(immOp),
                Address addr => base.NormalizeConstants
                        ? 1
                        : addr.GetHashCode(),
                MemoryOperand memOp => GetMemoryOperandHash(memOp),
                FpuOperand fpuOp => 59 * fpuOp.StNumber.GetHashCode(),
                _ => throw new NotImplementedException("Unhandled operand type: " + op.GetType().FullName)
            };
        }
            

        private int GetMemoryOperandHash(MemoryOperand memOp)
        {
            int h = 0;
            if (memOp.Base is not null)
            {
                h = base.GetRegisterHash(memOp.Base);
            }
            if (memOp.Index is not null)
            {
                h = 13 * h ^ base.GetRegisterHash(memOp.Index);
                h = 17 * h ^ memOp.Scale;
            }
            if (memOp.Offset is not null)
            {
                h = 23 * h ^ GetConstantHash(memOp.Offset);
            }
            if (memOp.SegOverride is not null)
            {
                h = 29 * h ^ GetRegisterHash(memOp.SegOverride);
            }
            return h;
        }
    }
}
