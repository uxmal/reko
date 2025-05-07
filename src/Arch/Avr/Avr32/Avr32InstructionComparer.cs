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

namespace Reko.Arch.Avr.Avr32;

internal class Avr32InstructionComparer : InstructionComparer
{
    private const int MemCode = 0x10;
    private const int RegImmCode = 0x11;
    private const int RegPartCode = 0x12;
    private const int RegPairCode = 0x13;
    private const int RegRangeCode = 0x14;
    private const int LitCode = 0x15;

    public Avr32InstructionComparer(Normalize norm) : base(norm)
    {

    }

    public override bool DoCompareOperands(MachineOperand op1, MachineOperand op2)
    {
        return op1 switch
        {
            RegisterStorage reg1 => CompareRegisters(reg1, (RegisterStorage) op2),
            Constant c1 => CompareConstants(c1, (Constant) op2),
            Address a1 => CompareAddresses(a1, (Address) op2),
            MemoryOperand m1 => CompareMemoryOperands(m1, (MemoryOperand) op2),
            RegisterImmediateOperand ri1 => CompareRegisterImmediates(ri1, (RegisterImmediateOperand) op2),
            RegisterPartOperand rpo1 => CompareRegisterParts(rpo1, (RegisterPartOperand) op2),
            RegisterPairOperand rp1 => CompareRegisterPairs(rp1, (RegisterPairOperand) op2),
            RegisterRange rr1 => CompareRegisterRanges(rr1, (RegisterRange) op2),
            SequenceStorage s1 => CompareSequences(s1, (SequenceStorage) op2),
            LiteralOperand lit1 => CompareLiterals(lit1, (LiteralOperand) op2),
            _ => throw new NotImplementedException(op1.GetType().Name)
        };
    }

    private bool CompareRegisterRanges(RegisterRange rr1, RegisterRange rr2)
    {
        if (base.NormalizeRegisters)
            return true;
        return 
            rr1.RegisterIndex == rr2.RegisterIndex &&
            rr1.Count == rr2.Count;
    }

    private bool CompareRegisterPairs(RegisterPairOperand rp1, RegisterPairOperand op2)
    {
        return
            CompareRegisters(rp1.HiRegister, op2.HiRegister) &&
            CompareRegisters(rp1.LoRegister, op2.LoRegister);
    }

    private bool CompareLiterals(LiteralOperand lit1, LiteralOperand lit2)
    {
        return lit1.Value == lit2.Value;
    }

    private bool CompareMemoryOperands(MemoryOperand m1, MemoryOperand m2)
    {
        if (!CompareRegisters(m1.Base, m2.Base))
            return false;
        if (!CompareConstants(m1.Offset, m2.Offset))
            return false;
        if (!CompareRegisters(m1.Index, m2.Index))
            return false;
        if (m1.IndexPart != m2.IndexPart)
            return false;
        if (m1.Shift != m2.Shift)
            return false;
        return
            m1.PostIncrement == m2.PostIncrement &&
            m2.PreDecrement == m2.PreDecrement;
    }

    private bool CompareRegisterParts(RegisterPartOperand rpo1, RegisterPartOperand rpo2)
    {
        return
            CompareRegisters(rpo1.Register, rpo2.Register) &&
            rpo1.Part == rpo2.Part;
    }

    private bool CompareRegisterImmediates(RegisterImmediateOperand ri1, RegisterImmediateOperand ri2)
    {
        return
            CompareRegisters(ri1.Register, ri2.Register) &&
            ri1.Mnemonic == ri2.Mnemonic &&
            CompareConstants(ri1.Value, ri2.Value);
    }

    public override int GetOperandHash(MachineOperand op)
    {
        return op switch
        {
            RegisterStorage reg => GetRegisterHash(reg),
            Constant c => GetConstantHash(c),
            Address a => GetAddressHash(a),
            MemoryOperand m => GetMemoryHash(m),
            RegisterImmediateOperand ri => GetRegisterImmediateHash(ri),
            RegisterPartOperand rpo => GetRegisterHash(rpo),
            RegisterPairOperand rp => GetRegisterPairHash(rp),
            RegisterRange rr => GetRegisterRangeHash(rr),
            SequenceStorage s => GetSequenceHash(s),
            LiteralOperand lit => GetStringHash(lit.Value),
            _ => throw new NotImplementedException(op.GetType().Name)
        };
    }

    private int GetRegisterRangeHash(RegisterRange rr)
    {
        int hash = RegRangeCode;
        hash = hash * 17 ^ (NormalizeRegisters ? 0 : rr.RegisterIndex);
        hash = hash * 17 ^ (NormalizeRegisters ? 0 : rr.Count);
        return hash;
    }

    private int GetRegisterPairHash(RegisterPairOperand rp)
    {
        int hash = RegPairCode;
        hash = hash * 17 ^ GetRegisterHash(rp.HiRegister);
        hash = hash * 17 ^ GetRegisterHash(rp.LoRegister);
        return hash;
    }

    private int GetStringHash(string value)
    {
        int hash = LitCode;
        foreach (char c in value)
        {
            hash = hash * 17 ^ c;
        }
        return hash;
    }

    private int GetMemoryHash(MemoryOperand m)
    {
        int hash = MemCode;
        hash = hash * 17 ^ GetRegisterHash(m.Base);
        hash = hash * 17 ^ GetConstantHash(m.Offset);
        hash = hash * 17 ^ GetRegisterHash(m.Index);
        hash = hash * 17 ^ (int) m.IndexPart;
        hash = hash * 3 ^ m.Shift;
        hash = hash ^ (m.PostIncrement ? 2 : 0);
        hash = hash ^ (m.PreDecrement ? 1 : 0);
        return hash;
    }

    private int GetRegisterHash(RegisterPartOperand rpo)
    {
        int hash = RegPartCode;
        hash = hash * 17 ^ GetRegisterHash(rpo.Register);
        hash = hash * 17 ^ (int) rpo.Part;
        return hash;
    }

    private int GetRegisterImmediateHash(RegisterImmediateOperand ri)
    {
        int hash = RegImmCode;
        hash = (hash * 17) ^ GetRegisterHash(ri.Register);
        hash = (hash * 31) ^ ri.Mnemonic.GetHashCode();
        hash = (hash * 21) ^ GetConstantHash(ri.Value);
        return hash;
    }
}