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
            var regA = opA as RegisterOperand;
            if (regA != null)
            {
                var regB = opB as RegisterOperand;
                return NormalizeRegisters || regA.Register == regB.Register;
            }
            var immA = opA as M68kImmediateOperand;
            if (immA != null)
            {
                var immB = opB as M68kImmediateOperand;
                return CompareValues(immA.Constant, immB.Constant);
            }
            var preA = opA as PredecrementMemoryOperand;
            if (preA != null)
            {
                var preB = opB as PredecrementMemoryOperand;
                return CompareRegisters(preA.Register, preB.Register);
            }
            var postA = opA as PostIncrementMemoryOperand;
            if (postA != null)
            {
                var postB = opB as PostIncrementMemoryOperand;
                return CompareRegisters(postA.Register, postB.Register);
            }
            var regsetA = opA as RegisterSetOperand;
            if (regsetA != null)
            {
                var regsetB = opB as RegisterSetOperand;
                return NormalizeRegisters || regsetA.BitSet == regsetB.BitSet;
            }
            var memA = opA as MemoryOperand;
            if (memA != null)
            {
                var memB = (MemoryOperand)opB;
                if (!NormalizeRegisters && !CompareRegisters(memA.Base, memB.Base))
                    return false;
                return NormalizeConstants || CompareValues(memA.Offset, memB.Offset);
            }
            var addrA = opA as M68kAddressOperand;
            if (addrA != null)
            {
                var addrB = (M68kAddressOperand)opB;
                return NormalizeConstants || addrA.Address == addrB.Address;
            }
            var idxA = opA as IndirectIndexedOperand;
            if (idxA != null)
            {
                var idxB = (IndirectIndexedOperand)opB;
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
            var rop = op as RegisterOperand;
            if (rop != null)
            {
                if (NormalizeRegisters)
                    return 0;
                else
                    return rop.Register.GetHashCode();
            }
            var immop = op as M68kImmediateOperand;
            if (immop != null)
            {
                if (NormalizeConstants)
                    return 0;
                else
                    return immop.Constant.GetHashCode();
            }
            var addrOp = op as M68kAddressOperand;
            if (addrOp != null)
            {
                if (NormalizeConstants)
                    return 0;
                else
                    return addrOp.Address.GetHashCode();
            }
            var memOp = op as MemoryOperand;
            if (memOp != null)
            {
                int h = 0;
                if (!NormalizeConstants)
                {
                    h = memOp.Offset.GetHashCode();
                }
                if (!NormalizeRegisters)
                {
                    h = h * 9 ^ memOp.Base.GetHashCode();
                }
                return h;
            }
            var ind = op as IndirectIndexedOperand;
            if (ind != null)
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
            var pre = op as PredecrementMemoryOperand;
            if (pre != null)
            {
                int h = 43;
                if (!NormalizeRegisters)
                {
                    h = h * 5 ^ base.GetRegisterHash(pre.Register);
                }
                return h;
            }
            var post = op as PostIncrementMemoryOperand;
            if (post != null)
            {
                int h = 47;
                if (!NormalizeRegisters)
                {
                    h = h * 7 ^ base.GetRegisterHash(post.Register);
                }
                return h;
            }
            var regset = op as RegisterSetOperand;
            if (regset != null)
            {
                int h = 29;
                if (!NormalizeRegisters)
                {
                    h = h ^ regset.BitSet.GetHashCode();
                }
                return h;
            }
            var indexOp = op as IndexedOperand;
            if (indexOp != null)
            {
                int h = 53;
                if (!NormalizeRegisters)
                {
                    if (indexOp.base_reg != null)
                    {
                        h = h * 7 ^ GetRegisterHash(indexOp.base_reg);
                    }
                    if (indexOp.index_reg != null)
                    {
                        h = h * 11 ^ GetRegisterHash(indexOp.index_reg);
                        h = h * 13 ^ indexOp.index_reg_width.GetHashCode();
                    }
                }
                if (!NormalizeConstants)
                {
                    if (indexOp.Base != null)
                    {
                        h = h * 17 ^ indexOp.Base.GetHashCode();
                    }
                    if (indexOp.index_scale != 0)
                    {
                        h = h * 19 ^ indexOp.index_scale;
                    }
                }
                return h;
            }
            throw new NotImplementedException();

        }
    }
}