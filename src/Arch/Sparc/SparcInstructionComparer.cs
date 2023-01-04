#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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

using System;
using System.Collections.Generic;
using Reko.Core;
using Reko.Core.Machine;

namespace Reko.Arch.Sparc
{
    public class SparcInstructionComparer : InstructionComparer
    {
        public SparcInstructionComparer(Normalize norm) : base(norm)
        {
        }

        public override bool CompareOperands(MachineInstruction x, MachineInstruction y)
        {
            var a = (SparcInstruction)x;
            var b = (SparcInstruction)y;
            return CompareOperands(a.Operands[0], b.Operands[0]) &&
                   CompareOperands(a.Operands[1], b.Operands[1]) &&
                   CompareOperands(a.Operands[2], b.Operands[2]);
        }

        private bool CompareOperands(MachineOperand a, MachineOperand b)
        {
            if (a == null && b == null)
                return true;
            if (a == null || b == null)
                return false;
            if (a.GetType() != b.GetType())
                return false;
            switch (a)
            {
            case RegisterStorage rA:
                return CompareRegisters(rA, (RegisterStorage) b);
            case ImmediateOperand immA:
                return CompareValues(immA.Value, ((ImmediateOperand) b).Value);
            case AddressOperand addrA:
                return NormalizeConstants ||
                    addrA.Address.ToLinear() == ((AddressOperand) b).Address.ToLinear();
            case MemoryOperand mA:
                var mB = (MemoryOperand) b;
                if (!CompareRegisters(mA.Base, mB.Base))
                    return false;
                return CompareValues(mA.Offset, mB.Offset);
            case IndexedMemoryOperand xA:
                var xB = (IndexedMemoryOperand) b;
                if (!CompareRegisters(xA.Base, xB.Base))
                    return false;
                return CompareRegisters(xA.Index, xB.Index);
            }
            throw new NotImplementedException();
        }

        public override int GetOperandsHash(MachineInstruction sInstr)
        {
            var instr = (SparcInstruction)sInstr;
            return
                17 * GetOperandHash(instr.Operands[0]) ^
                23 * GetOperandHash(instr.Operands[1]) ^
                59 * GetOperandHash(instr.Operands[2]);
        }

        private int GetOperandHash(MachineOperand op)
        {
            if (op == null)
                return 0;
            switch (op)
            {
            case RegisterStorage r:
                return GetRegisterHash(r);
            case ImmediateOperand i:
                return GetConstantHash(i.Value);
            case AddressOperand a:
                return a.Address.GetHashCode();
            case MemoryOperand m:
                var h = GetRegisterHash(m.Base);
                h = h ^ 29 * GetConstantHash(m.Offset);
                return h;
            case IndexedMemoryOperand x:
                h = GetRegisterHash(x.Base);
                h = h ^ 59 * GetRegisterHash(x.Index);
                return h;
            }
            throw new NotImplementedException();
        }
    }
}