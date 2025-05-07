using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using System;

namespace Reko.Arch.SuperH;

internal class SuperHInstructionComparer : InstructionComparer
{
    private const int MemCode = 0x10;

    public SuperHInstructionComparer(Normalize norm) : base(norm)
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
            _ => throw new NotImplementedException(op1.GetType().Name)
        };
    }

    private bool CompareMemoryOperands(MemoryOperand m1, MemoryOperand m2)
    {
        if (m1.mode != m2.mode)
            return false;
        if (!CompareRegisters(m1.reg, m2.reg))
            return false;
        return CompareConstants(m1.disp, m2.disp);
    }

    public override int GetOperandHash(MachineOperand op)
    {
        return op switch
        {
            RegisterStorage reg => GetRegisterHash(reg),
            Constant c => GetConstantHash(c),
            Address a => GetAddressHash(a),
            MemoryOperand m => GetMemoryHash(m),
            _ => throw new NotImplementedException(op.GetType().Name)
        };
    }

    private int GetMemoryHash(MemoryOperand m)
    {
        int hash = MemCode;
        hash = hash * 17 ^ (int) m.mode;
        hash = hash * 17 ^ GetRegisterHash(m.reg);
        hash = hash * 17 ^ m.disp;
        return hash;
    }
}