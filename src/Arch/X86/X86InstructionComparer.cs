#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
        private const int MemCode = 0x10;
        private const int FpuCode = 0x11;

        public X86InstructionComparer(Normalize norm) 
            : base(norm)
        {
        }

        public override bool DoCompareOperands(MachineOperand opA, MachineOperand opB)
		{
            return opA switch
            {
                RegisterStorage regOpA => CompareRegisters(regOpA, (RegisterStorage) opB),
                Constant immOpA => CompareConstants(immOpA, (Constant) opB),
                Address addrA => CompareAddresses(addrA, (Address) opB),
                MemoryOperand memOpA => CompareMemoryOperand(memOpA, (MemoryOperand) opB),
                FpuOperand fpuA => CompareFpuOperand(fpuA, (FpuOperand) opB),
                _ => throw new NotImplementedException(string.Format("NYI: {0}", opA.GetType()))
            };
            }

        private bool CompareFpuOperand(FpuOperand fpuA, FpuOperand fpuB)
        {
            return NormalizeRegisters || fpuA.StNumber == fpuB.StNumber;
        }

        private bool CompareMemoryOperand(MemoryOperand memOpA, MemoryOperand memOpB)
        {
            if (!base.CompareRegisters(memOpA.Base, memOpB.Base))
                return false;
            if (!base.CompareRegisters(memOpA.Index, memOpB.Index))
                return false;
            if (memOpA.Scale != memOpB.Scale)
                return false;
            return base.CompareConstants(memOpA.Offset, memOpB.Offset);
        }

        public override int GetOperandHash(MachineOperand op)
		{
            return op switch
            {
                RegisterStorage regOp => base.GetRegisterHash(regOp),
                Constant immOp => base.GetConstantHash(immOp),
                Address addr => base.GetAddressHash(addr),
                MemoryOperand memOp => GetMemoryOperandHash(memOp),
                FpuOperand fpuOp => FpuCode * fpuOp.StNumber,
                _ => throw new NotImplementedException("Unhandled operand type: " + op.GetType().FullName)
            };
        }

        private int GetMemoryOperandHash(MemoryOperand memOp)
        {
            int h = MemCode;
            h = h * 7 ^ GetRegisterHash(memOp.Base);
            if (memOp.Index is not null)
            {
                h = 13 * h ^ base.GetRegisterHash(memOp.Index);
                h = 17 * h ^ memOp.Scale;
            }
            h = 23 * h ^ GetConstantHash(memOp.Offset);
            h = 29 * h ^ GetRegisterHash(memOp.SegOverride);
            return h;
        }
    }
}
