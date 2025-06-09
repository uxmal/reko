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

using Reko.Arch.M68k.Machine;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using System;

namespace Reko.Arch.M68k
{
    internal class M68kInstructionComparer : InstructionComparer
    {
        public M68kInstructionComparer(Normalize norm) : base(norm)
        {
        }

        public override bool CompareOperands(MachineInstruction x, MachineInstruction y)
        {
            var a = (M68kInstruction)x;
            var b = (M68kInstruction)y;
            return
                Compare(a.Operands[0], b.Operands[0]) &&
                Compare(a.Operands[1], b.Operands[1]) &&
                Compare(a.Operands[2], b.Operands[2]);
        }

        private bool Compare(MachineOperand opA, MachineOperand opB)
        {
            if (opA is null && opB is null)
                return true;
            if (opA is null || opB is null)
                return false;

            if (opA.GetType() != opB.GetType())
                return false;
            switch (opA)
            {
            case RegisterStorage regA:
                var regB = (RegisterStorage) opB;
                return NormalizeRegisters || regA == regB;
            case Constant immA:
                var immB = (Constant) opB;
                return CompareValues(immA, immB);
            case PredecrementMemoryOperand preA:
                var preB = (PredecrementMemoryOperand) opB;
                return CompareRegisters(preA.Register, preB.Register);
            case PostIncrementMemoryOperand postA:
                var postB = (PostIncrementMemoryOperand) opB;
                return CompareRegisters(postA.Register, postB.Register);
            case RegisterSetOperand regsetA:
                var regsetB = (RegisterSetOperand) opB;
                return NormalizeRegisters || regsetA.BitSet == regsetB.BitSet;
            case MemoryOperand memA:
                var memB = (MemoryOperand) opB;
                if (!NormalizeRegisters && !CompareRegisters(memA.Base, memB.Base))
                    return false;
                return NormalizeConstants || CompareValues(memA.Offset, memB.Offset);
            case Address addrA:
                var addrB = (Address) opB;
                return NormalizeConstants || addrA == addrB;
            case IndirectIndexedOperand idxA:
                var idxB = (IndirectIndexedOperand) opB;
                if (!NormalizeRegisters)
                {
                    if (!CompareRegisters(idxA.ARegister, idxB.ARegister))
                        return false;
                    if (!CompareRegisters(idxA.XRegister, idxB.XRegister))
                        return false;
                }
                if (!NormalizeConstants)
                {
                    if (idxA.Imm8 != idxB.Imm8)
                        return false;
                    if (idxA.Scale != idxB.Scale)
                        return false;
                }
                return true;
            }
            throw new NotImplementedException(opA.GetType().FullName);
        }

        public override int GetOperandsHash(MachineInstruction instr)
        {
            var i = (M68kInstruction)instr;
            return
                OperandHash(i.Operands[0]) * 23 ^
                OperandHash(i.Operands[1]) * 29 ^
                OperandHash(i.Operands[2]) * 9;
        }

        private int OperandHash(MachineOperand op)
        {
            if (op is null)
                return 0;
            switch (op)
            {
            case RegisterStorage rop:
                if (NormalizeRegisters)
                    return 0;
                else
                    return rop.GetHashCode();
            case Constant immop:
                if (NormalizeConstants)
                    return 0;
                else
                    return immop.GetHashCode();
            case Address addrOp:
                if (NormalizeConstants)
                    return 0;
                else
                    return addrOp.GetHashCode();
            case MemoryOperand memOp:
                int h = 0;
                if (!NormalizeConstants && memOp.Offset is not null)
                {
                    h = memOp.Offset.GetHashCode();
                }
                if (!NormalizeRegisters)
                {
                    h = h * 9 ^ memOp.Base.GetHashCode();
                }
                return h;
            case IndirectIndexedOperand ind:
                h = 0;
                if (!NormalizeConstants)
                {
                    h = ind.Imm8.GetHashCode();
                    h = h * 11 ^ ind.Scale.GetHashCode();
                    h = h * 13 ^ ind.Imm8.GetHashCode();
                }
                if (!NormalizeRegisters)
                {
                    h = h * 5 ^ ind.ARegister.GetHashCode();
                    h = h * 17 ^ ind.XRegister.GetHashCode();
                }
                return h;
            case PredecrementMemoryOperand pre:
                h = 43;
                if (!NormalizeRegisters)
                {
                    h = h * 5 ^ base.GetRegisterHash(pre.Register);
                }
                return h;
            case PostIncrementMemoryOperand post:
                h = 47;
                if (!NormalizeRegisters)
                {
                    h = h * 7 ^ base.GetRegisterHash(post.Register);
                }
                return h;
            case RegisterSetOperand regset:
                h = 29;
                if (!NormalizeRegisters)
                {
                    h = h ^ regset.BitSet.GetHashCode();
                }
                return h;
            case IndexedOperand indexOp:
                h = 53;
                if (!NormalizeRegisters)
                {
                    if (indexOp.Base is not null)
                    {
                        h = h * 7 ^ GetRegisterHash(indexOp.Base);
                    }
                    if (indexOp.Index is not null)
                    {
                        h = h * 11 ^ GetRegisterHash(indexOp.Index);
                        h = h * 13 ^ indexOp.index_reg_width!.GetHashCode();
                    }
                }
                if (!NormalizeConstants)
                {
                    if (indexOp.BaseDisplacement is not null)
                    {
                        h = h * 17 ^ indexOp.BaseDisplacement.GetHashCode();
                    }
                    if (indexOp.IndexScale != 0)
                    {
                        h = h * 19 ^ indexOp.IndexScale;
                    }
                }
                return h;
            }
            throw new NotImplementedException();
        }
    }
}