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

using System;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;

namespace Reko.Arch.PowerPC;

public class PowerPcInstructionComparer : InstructionComparer
{
    private const int MemCode = 0x10;
    private const int CondCode = 0x11;
    private const int FuncCode = 0x12;

        public PowerPcInstructionComparer(Normalize norm) : base(norm)
        {
        }

    public override bool DoCompareOperands(MachineOperand opA, MachineOperand opB)
        {
            switch (opA)
            {
            case RegisterStorage regA:
                var regB = (RegisterStorage) opB;
                return CompareRegisters(regA, regB);
            case Constant immA:
                var immB = (Constant) opB;
            return CompareConstants(immA, immB);
            case Address addrA:
                var addrB = (Address) opB;
                return NormalizeConstants || addrA == addrB;
            case MemoryOperand memA:
                var memB = (MemoryOperand) opB;
                return CompareRegisters(memA.BaseRegister, memB.BaseRegister) &&
                    memA.Offset == memB.Offset;
            case ConditionOperand cA:
                var cB = (ConditionOperand) opB;
                return cA.condition == cB.condition;
        case FunctionCodeOperand fcA:
            var fcB = (FunctionCodeOperand) opB;
            return fcA.FunctionCode == fcB.FunctionCode;
            }
            throw new NotImplementedException(string.Format("PowerPC operand type {0} not implemented.", opA.GetType().Name));
        }

    public override int GetOperandHash(MachineOperand op)
        {
            if (op is null)
                return 0;
            int h = op.GetType().GetHashCode();
            switch (op)
            {
            case RegisterStorage reg:
                if (!NormalizeRegisters)
                    h ^= GetRegisterHash(reg);
                return h;
            case Constant imm:
            return GetConstantHash(imm);
            case Address addr:
            return GetAddressHash(addr);
            case MemoryOperand mem:
            h = MemCode;
                if (!NormalizeRegisters)
                    h ^= GetRegisterHash(mem.BaseRegister);
                if (!NormalizeConstants)
                h = h*17 ^ GetOperandHash(mem.Offset);
                return h;
            case ConditionOperand c:
            h ^= CondCode ^ 17 * (int)c.condition;
                return h;
        case FunctionCodeOperand fc:
            h ^= FuncCode ^ 17 * (int) fc.FunctionCode;
            return h;
            }
            throw new NotImplementedException(string.Format("PowerPC operand type {0} not implemented.", op.GetType().Name));
        }
}
