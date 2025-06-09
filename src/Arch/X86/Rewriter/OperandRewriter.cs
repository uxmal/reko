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

using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Operators;
using Reko.Core.Types;
using Reko.Core;
using System;
using Reko.Core.Lib;

namespace Reko.Arch.X86.Rewriter
{
    /// <summary>
    /// Helper class used by the X86 rewriter to turn machine code operands into
    /// <see cref="Expression"/>s.
    /// </summary>
    public abstract class OperandRewriter
    {
        protected readonly IntelArchitecture arch;
        private readonly ExpressionEmitter m;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly X86State state;

        public OperandRewriter(
            IntelArchitecture arch,
            ExpressionEmitter emitter,
            IStorageBinder binder,
            IRewriterHost host,
            X86State state)
        {
            this.arch = arch;
            this.m = emitter;
            this.binder = binder;
            this.host = host;
            this.state = state;
        }

        public Expression Transform(X86Instruction instr, MachineOperand op, DataType opWidth)
        {
            switch (op)
            {
            case RegisterStorage reg: return AluRegister(reg);
            case MemoryOperand mem: return CreateMemoryAccess(instr, mem, opWidth);
            case Constant imm: return CreateConstant(imm, (PrimitiveType) opWidth);
            case FpuOperand fpu: return FpuRegister(fpu.StNumber);
            case Address addr: return addr;
            default: throw new NotImplementedException($"Operand {op}");
            }
        }

        public Identifier AluRegister(RegisterStorage reg)
        {
            return binder.EnsureRegister(reg);
        }

        public Identifier AluRegister(RegisterStorage reg, DataType vt)
        {
            return binder.EnsureRegister(arch.GetRegister(reg.Domain, new BitRange(0, vt.BitSize))!);
        }

        public Constant CreateConstant(Constant imm, PrimitiveType dataWidth)
        {
            if (dataWidth.BitSize > imm.DataType.BitSize)
                return Constant.Create(dataWidth, imm.ToInt64());
            else
                return Constant.Create(imm.DataType, imm.ToUInt64());
        }

        public Expression CreateMemoryAccess(X86Instruction instr, MemoryOperand mem, DataType dt)
        {
            Expression expr = EffectiveAddressExpression(instr, mem);
            //$REVIEW: perhaps the code below could be moved to Scanner since it is arch-independent?
            if (expr is Address addrThunk)
            {
                var exg = host.GetImport(addrThunk, instr.Address);
                if (exg is ProcedureConstant)
                {
                    return exg;
                }
                else if (exg is not null)
                {
                    return m.Unary(Operator.AddrOf, dt, exg);
                }
                var exp = host.GetImportedProcedure(arch, addrThunk, instr.Address);
                if (exp is not null)
                {
                    return new ProcedureConstant(arch.PointerType, exp);
                }
            }
            if (IsSegmentedAccessRequired ||
                (mem.DefaultSegment != Registers.cs &&
                 mem.DefaultSegment != Registers.ds && 
                 mem.DefaultSegment != Registers.ss))
            {
                Expression seg;
                if (mem.DefaultSegment == Registers.cs)
                {
                    var cs = state.GetRegister(Registers.cs);
                    if (cs is not InvalidConstant)
                    {
                        seg = cs;
                    }
                    else
                    {
                        seg = Constant.Create(PrimitiveType.SegmentSelector, instr.Address.Selector!.Value);
                    }
                }
                else
                {
                    seg = AluRegister(mem.DefaultSegment);
                }
                return m.SegMem(dt, seg, expr);
            }
            else
            {
                return m.Mem(dt, expr);
            }
        }

        public virtual bool IsSegmentedAccessRequired => false;

        public Expression CreateMemoryAccess(X86Instruction instr, MemoryOperand memoryOperand)
        {
            return CreateMemoryAccess(instr, memoryOperand, memoryOperand.DataType);
        }

        public virtual MemoryAccess StackAccess(Expression expr, DataType dt)
        {
            return new MemoryAccess(MemoryStorage.GlobalMemory, expr, dt);
        }

        /// <summary>
        /// Memory accesses are translated into expressions, performing simplifications
        /// where possible.
        /// </summary>
        public Expression EffectiveAddressExpression(X86Instruction instr, MemoryOperand mem)
        {
            Expression? eIndex;
            Expression? eBase;
            Expression? expr = null;
            bool ripRelative = false;

            if (mem.Base != RegisterStorage.None)
            {
                if (mem.Base == Registers.rip)
                {
                    ripRelative = true;
                }
                else
                {
                    eBase = AluRegister(mem.Base);
                    expr = eBase;
                }
            }

            if (mem.Offset is not null)
            {
                if (ripRelative)
                {
                    expr = instr.Address + (instr.Length + mem.Offset.ToInt64());
                }
                else if (expr is not null)
                {
                    if (expr.DataType.BitSize > mem.Offset.DataType.BitSize)
                    {
                        expr = m.AddSubSignedInt(expr, mem.Offset.ToInt32());
                    }
                    else
                    {
                        BinaryOperator op = Operator.IAdd;
                        long l = mem.Offset.ToInt64();
                        if (l < 0 && l > -0x800)
                        {
                            l = -l;
                            op = Operator.ISub;
                        }

                        DataType dt = expr.DataType;
                        Constant cOffset = Constant.Create(dt, l);
                        expr = m.Bin(op, dt, expr, cOffset);
                    }
                }
                else
                {
                    // expr is null, so there was no base register. But was there 
                    // an index register? If so extend the offset to the samesize.
                    if (mem.Index != RegisterStorage.None && (int)mem.Index.BitSize != mem.Offset.DataType.BitSize)
                    {
                        var dt = PrimitiveType.Create(Domain.SignedInt, (int)mem.Index.BitSize);
                        expr = Constant.Create(dt, mem.Offset.ToInt64());
                    }
                    else
                    {
                        // There was no base or index.
                        expr = mem.Offset;
                    }
                }
            }

            if (mem.Index != RegisterStorage.None && mem.Index.Number != Registers.riz.Number)
            {
                eIndex = AluRegister(mem.Index);
                if (mem.Scale != 0 && mem.Scale != 1)
                {
                    eIndex = m.IMul(eIndex, Constant.Create(mem.Index.DataType, mem.Scale));
                }
                expr = m.IAdd(expr!, eIndex);
            }
            if (!IsSegmentedAccessRequired && expr is Constant c && mem.SegOverride == RegisterStorage.None)
            {
                return arch.MakeAddressFromConstant(c, false)!;
            }
            return expr!;
        }

        public Identifier FlagGroup(FlagGroupStorage flags) => binder.EnsureFlagGroup(flags);

        /// <summary>
        /// Changes the stack-relative address 'reg' into a frame-relative operand.
        /// If the register number is larger than the stack depth, then
        /// the register was passed on the stack when the function was called.
        /// </summary>
        /// <param name="reg"></param>
        /// <returns></returns>
        public Expression FpuRegister(int reg)
        {
            Expression idx = binder.EnsureRegister(Registers.Top);
            if (reg != 0)
            {
                idx = m.IAddS(idx, reg);
            }
            return new MemoryAccess(Registers.ST, idx, PrimitiveType.Real64);
        }

        public UnaryExpression AddrOf(Expression expr)
        {
            return m.Unary(Operator.AddrOf,
                PrimitiveType.Create(Domain.Pointer, arch.WordWidth.BitSize), expr);
        }

        public abstract Address ImmediateAsAddress(Address address, Constant imm);
    }

    public class OperandRewriter16 : OperandRewriter
    {
        public OperandRewriter16(IntelArchitecture arch, ExpressionEmitter m, IStorageBinder binder, IRewriterHost host, X86State state) : base(arch, m, binder, host, state) { }

        public override bool IsSegmentedAccessRequired { get { return true; } }

        public override Address ImmediateAsAddress(Address address, Constant imm)
        {
            return this.arch.ProcessorMode.CreateSegmentedAddress(address.Selector!.Value, imm.ToUInt32())!.Value;
        }

        public override MemoryAccess StackAccess(Expression expr, DataType dt)
        {
            return new MemoryAccess(MemoryStorage.GlobalMemory, 
                new SegmentedPointer(
                    arch.ProcessorMode.PointerType,
                    AluRegister(Registers.ss), expr),
                dt);
        }
    }

    public class OperandRewriter32 : OperandRewriter
    {
        public OperandRewriter32(IntelArchitecture arch, ExpressionEmitter m, IStorageBinder binder, IRewriterHost host, X86State state) : base(arch,m, binder, host, state) { }

        public override Address ImmediateAsAddress(Address address, Constant imm)
        {
            return Address.Ptr32(imm.ToUInt32());
        }
    }

    public class OperandRewriter64 : OperandRewriter
    {
        public OperandRewriter64(IntelArchitecture arch, ExpressionEmitter m, IStorageBinder binder, IRewriterHost host, X86State state) : base(arch, m, binder, host, state) { }

        public override Address ImmediateAsAddress(Address address, Constant imm)
        {
            return Address.Ptr64(imm.ToUInt64());
        }
    }
}
