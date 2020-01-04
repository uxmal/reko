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

using System;
using System.Collections.Generic;
using Reko.Core;
using Reko.Core.Machine;

namespace Reko.Arch.Pdp11
{
    public class Pdp11InstructionComparer : InstructionComparer
    {
        public Pdp11InstructionComparer(Normalize norm) : base(norm)
        {
        }

        public override bool CompareOperands(MachineInstruction x, MachineInstruction y)
        {
            var a = (Pdp11Instruction)x;
            var b = (Pdp11Instruction)y;
            return
                Compare(Op(x, 0), Op(y, 0)) &&
                Compare(Op(x, 1), Op(y, 1));
        }

        private MachineOperand Op(MachineInstruction instr, int iOp)
        {
            return iOp < instr.Operands.Length ? instr.Operands[iOp] : null;
        }

        private bool Compare(MachineOperand opA, MachineOperand opB)
        {
            if (opA == null && opB == null)
                return true;
            if (opA == null || opB == null)
                return false;
            if (opA.GetType() != opB.GetType())
                return false;
            var ropA = opA as RegisterOperand;
            if (ropA != null)
            {
                var ropB = (RegisterOperand)opB;
                return ropA.Register == ropB.Register; 
            }
            var addrA = opA as AddressOperand;
            if (addrA != null)
            {
                if (NormalizeConstants)
                    return true;
                var addrB = opB as AddressOperand;
                return addrA.Address.ToLinear() == addrB.Address.ToLinear();
            }
            var immA = opA as ImmediateOperand;
            if (immA != null)
            {
                var immB = (ImmediateOperand)opB;
                return CompareValues(immA.Value, immB.Value);
            }
            var memA = opA as MemoryOperand;
            if (memA != null)
            {
                var memB = (MemoryOperand)opB;
                if (memA.PreDec != memB.PreDec)
                    return false;
                if (memA.PostInc != memB.PostInc)
                    return false;
                if (memA.Mode != memB.Mode)
                    return false;
                if (!NormalizeRegisters && !CompareRegisters(memA.Register, memB.Register))
                    return false;
                if (!NormalizeConstants && memA.EffectiveAddress != memB.EffectiveAddress)
                    return false;
                return true;
            }
            throw new NotImplementedException(opA.GetType().FullName);
        }

        public override int GetOperandsHash(MachineInstruction i)
        {
            return Hash(i, 0) * 23 ^ Hash(i, 1);
        }

        private int Hash(MachineInstruction instr, int iOp)
        {
            if (iOp >= instr.Operands.Length)
                return 0;
            var op = instr.Operands[iOp];
            int hash = op.GetType().GetHashCode();
            var rop = op as RegisterOperand;
            if (rop != null)
            {
                if (NormalizeRegisters)
                    return 0;
                else 
                    return rop.Register.GetHashCode();
            }
            var immop = op as ImmediateOperand;
            if (immop != null)
            {
                return base.GetConstantHash(immop.Value);
            }
            var addrop = op as AddressOperand;
            if (addrop != null)
            {
                if (NormalizeRegisters)
                    return 0;
                else
                    return addrop.Address.GetHashCode();
            }
            var mem = op as MemoryOperand;
            if (mem != null)
            {
                var r = NormalizeRegisters || mem.Register == null
                    ? 0
                    : mem.Register.GetHashCode();
                var o = NormalizeConstants
                    ? 0
                    : mem.EffectiveAddress.GetHashCode();
                if (mem.PreDec)
                    r ^= 167;
                if (mem.PostInc)
                    r ^= 3163;
                return mem.Mode.GetHashCode() ^
                    r * 17 ^
                    o * 5;
            }
            throw new NotImplementedException();
        }
    }
}