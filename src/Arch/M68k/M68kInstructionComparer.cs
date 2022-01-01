#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
                Compare(a.Operands[0], a.Operands[0]) &&
                Compare(a.Operands[1], a.Operands[1]) &&
                Compare(a.Operands[2], a.Operands[2]);
        }

        private bool Compare(MachineOperand opA, MachineOperand opB)
        {
            if (opA == null && opB == null)
                return true;
            if (opA == null || opB == null)
                return false;

            if (opA.GetType() != opB.GetType())
                return false;
            if (opA is RegisterOperand regA)
            {
                var regB = (RegisterOperand) opB;
                return NormalizeRegisters || regA.Register == regB.Register;
            }
            if (opA is M68kImmediateOperand immA)
            {
                var immB = (M68kImmediateOperand) opB;
                return CompareValues(immA.Constant, immB.Constant);
            }
            if (opA is PredecrementMemoryOperand preA)
            {
                var preB = (PredecrementMemoryOperand) opB;
                return CompareRegisters(preA.Register, preB.Register);
            }
            if (opA is PostIncrementMemoryOperand postA)
            {
                var postB = (PostIncrementMemoryOperand) opB;
                return CompareRegisters(postA.Register, postB.Register);
            }
            if (opA is RegisterSetOperand regsetA)
            {
                var regsetB = (RegisterSetOperand) opB;
                return NormalizeRegisters || regsetA.BitSet == regsetB.BitSet;
            }
            if (opA is MemoryOperand memA)
            {
                var memB = (MemoryOperand) opB;
                if (!NormalizeRegisters && !CompareRegisters(memA.Base, memB.Base))
                    return false;
                return NormalizeConstants || CompareValues(memA.Offset, memB.Offset);
            }
            if (opA is M68kAddressOperand addrA)
            {
                var addrB = (M68kAddressOperand) opB;
                return NormalizeConstants || addrA.Address == addrB.Address;
            }
            if (opA is IndirectIndexedOperand idxA)
            {
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
            if (op == null)
                return 0;
            if (op is RegisterOperand rop)
            {
                if (NormalizeRegisters)
                    return 0;
                else
                    return rop.Register.GetHashCode();
            }
            if (op is M68kImmediateOperand immop)
            {
                if (NormalizeConstants)
                    return 0;
                else
                    return immop.Constant.GetHashCode();
            }
            if (op is M68kAddressOperand addrOp)
            {
                if (NormalizeConstants)
                    return 0;
                else
                    return addrOp.Address.GetHashCode();
            }
            if (op is MemoryOperand memOp)
            {
                int h = 0;
                if (!NormalizeConstants && memOp.Offset != null)
                {
                    h = memOp.Offset.GetHashCode();
                }
                if (!NormalizeRegisters)
                {
                    h = h * 9 ^ memOp.Base.GetHashCode();
                }
                return h;
            }
            if (op is IndirectIndexedOperand ind)
            {
                int h = 0;
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
            }
            if (op is PredecrementMemoryOperand pre)
            {
                int h = 43;
                if (!NormalizeRegisters)
                {
                    h = h * 5 ^ base.GetRegisterHash(pre.Register);
                }
                return h;
            }
            if (op is PostIncrementMemoryOperand post)
            {
                int h = 47;
                if (!NormalizeRegisters)
                {
                    h = h * 7 ^ base.GetRegisterHash(post.Register);
                }
                return h;
            }
            if (op is RegisterSetOperand regset)
            {
                int h = 29;
                if (!NormalizeRegisters)
                {
                    h = h ^ regset.BitSet.GetHashCode();
                }
                return h;
            }
            if (op is IndexedOperand indexOp)
            {
                int h = 53;
                if (!NormalizeRegisters)
                {
                    if (indexOp.Base != null)
                    {
                        h = h * 7 ^ GetRegisterHash(indexOp.Base);
                    }
                    if (indexOp.Index != null)
                    {
                        h = h * 11 ^ GetRegisterHash(indexOp.Index);
                        h = h * 13 ^ indexOp.index_reg_width!.GetHashCode();
                    }
                }
                if (!NormalizeConstants)
                {
                    if (indexOp.BaseDisplacement != null)
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