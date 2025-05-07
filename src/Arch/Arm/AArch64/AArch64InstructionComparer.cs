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

namespace Reko.Arch.Arm.AArch64
{
    public class AArch64InstructionComparer : InstructionComparer
    {
        private const int MemCode = 0x10;
        private const int VectorCode = 0x11;
        private const int BarrierCode = 0x12;
        private const int VectorMultiCode = 0x13;

        public AArch64InstructionComparer(Normalize norm) : base(norm)
        {
        }

        public override bool DoCompareOperands(MachineOperand op1, MachineOperand op2)
        {
            return op1 switch
            {
                RegisterStorage reg => CompareRegisters(reg, (RegisterStorage) op2),
                Constant c => CompareConstants(c, (Constant) op2),
                Address a => CompareAddresses(a, (Address) op2),
                MemoryOperand m => CompareMemoryOperands(m, (MemoryOperand) op2),
                ConditionOperand<ArmCondition> cond => cond.Condition == ((ConditionOperand<ArmCondition>) op2).Condition,
                BarrierOperand bop => CompareBarrierOperands(bop, (BarrierOperand) op2),
                VectorRegisterOperand vr => CompareVector(vr, (VectorRegisterOperand) op2),
                VectorMultipleRegisterOperand vmr => CompareMultiVector(vmr, (VectorMultipleRegisterOperand) op2),
                _ => throw new NotImplementedException(op1.GetType().Name)
            };
        }

        private bool CompareMemoryOperands(MemoryOperand m1, MemoryOperand m2)
        {
            if (!CompareRegisters(m1.Base, m2.Base))
                return false;
            if (!CompareConstants(m1.Offset, m2.Offset))
                return false;
            if (!CompareRegisters(m1.Index, m2.Index))
                return false;
            if (m1.IndexExtend != m2.IndexExtend)
                return false;
            if (m1.IndexShift != m2.IndexShift)
                return false;
            return
                m1.PreIndex == m2.PreIndex &&
                m1.PostIndex == m2.PostIndex;
        }

        private bool CompareMultiVector(VectorMultipleRegisterOperand vmr1, VectorMultipleRegisterOperand vmr2)
        {
            var e1 = vmr1.GetRegisters().GetEnumerator();
            var e2 = vmr2.GetRegisters().GetEnumerator();
            for (; ;)
            {
                if (!e1.MoveNext())
                    return !e2.MoveNext();
                if (!e2.MoveNext())
                    return false;
                if (!CompareRegisters(e1.Current, e2.Current))
                    return false;
            }
        }

        private bool CompareBarrierOperands(BarrierOperand bop1, BarrierOperand bop2)
        {
            return bop1.Option == bop2.Option;
        }

        private bool CompareVector(VectorRegisterOperand vr1, VectorRegisterOperand vr2)
        {
            if (!CompareRegisters(vr1.VectorRegister, vr2.VectorRegister))
                return false;
            return vr1.Index == vr2.Index;
        }

        public override int GetOperandHash(MachineOperand op)
        {
            return op switch
            {
                RegisterStorage reg => GetRegisterHash(reg),
                Constant c => GetConstantHash(c),
                Address a => GetAddressHash(a),
                MemoryOperand m => GetMemoryHash(m),
                VectorRegisterOperand vr => GetVectorRegisterHash(vr),
                VectorMultipleRegisterOperand vmr => GetVectorMultipleRegisterHash(vmr),
                ConditionOperand<ArmCondition> cond => (int) cond.Condition,
                BarrierOperand bop => GetBarrierHash(bop),
                _ => throw new NotImplementedException(op.GetType().Name)
            };
        }

        private int GetMemoryHash(MemoryOperand m)
        {
            int hash = MemCode;
            hash = hash * 17 ^ GetRegisterHash(m.Base);
            hash = hash * 17 ^ GetRegisterHash(m.Index);
            hash = hash * 17 ^ GetConstantHash(m.Offset);
            hash = hash * 17 ^ (int) m.IndexExtend;
            hash = hash * 3 ^ m.IndexShift;
            hash = hash ^ (m.PreIndex ? 2 : 0);
            hash = hash ^ (m.PreIndex ? 1 : 0);
            return hash;
        }

        private int GetVectorMultipleRegisterHash(VectorMultipleRegisterOperand vmr)
        {
            int hash = VectorMultiCode;
            foreach (var reg in vmr.GetRegisters())
            {
                hash = (hash * 31) ^ GetRegisterHash(reg);
            }
            return hash;
        }

        private int GetBarrierHash(BarrierOperand bop)
        {
            int hash = BarrierCode;
            hash = hash * 17 ^ (int)bop.Option;
            return hash;
        }

        private int GetVectorRegisterHash(VectorRegisterOperand vr)
        {
            int hash = VectorCode;
            hash = (hash * 31) ^ GetRegisterHash(vr.VectorRegister);
            hash = (hash * 31) ^ vr.Index;
            return hash;
        }
    }
}
