#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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

using Decompiler.Arch.X86;
using Decompiler.Core.Expressions;
using Decompiler.Core.Machine;
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using Decompiler.Core;
using System;

namespace Decompiler.Arch.X86
{
    public class OperandRewriter
    {
        private IntelArchitecture arch;
        private Frame frame;
        private IRewriterHost host;

        public OperandRewriter(IntelArchitecture arch, Frame frame, IRewriterHost host)
        {
            this.arch = arch;
            this.frame = frame;
            this.host = host;
        }

        public Expression Transform(MachineOperand op, PrimitiveType opWidth, X86State state)
        {
            var reg = op as RegisterOperand;
            if (reg != null)
                return AluRegister(reg);
            var mem = op as MemoryOperand;
            if (mem != null)
                return CreateMemoryAccess(mem, opWidth, state);
            var imm = op as ImmediateOperand;
            if (imm != null)
                return CreateConstant(imm, opWidth);
            var fpu = op as FpuOperand;
            if (fpu != null)
                return FpuRegister(fpu.StNumber, state);
            var addr = op as AddressOperand;
            if (addr != null)
                return addr.Address;
            throw new NotImplementedException(string.Format("Operand {0}", op));
        }

        public Identifier AluRegister(RegisterOperand reg)
        {
            return frame.EnsureRegister(reg.Register);
        }

        public Identifier AluRegister(RegisterStorage reg)
        {
            return frame.EnsureRegister(reg);
        }

        public Identifier AluRegister(IntelRegister reg, PrimitiveType vt)
        {
            return frame.EnsureRegister(reg.GetPart(vt));
        }

        public Constant CreateConstant(ImmediateOperand imm, PrimitiveType dataWidth)
        {
            if (dataWidth.BitSize > imm.Width.BitSize)
                return Constant.Create(dataWidth, imm.Value.ToInt64());
            else
                return Constant.Create(imm.Width, imm.Value.ToUInt32());
        }

        public Expression CreateMemoryAccess(MemoryOperand mem, DataType dt, X86State state)
        {
            PseudoProcedure ppp = ImportedProcedureName(mem.Width, mem);
            if (ppp != null)
                return new ProcedureConstant(arch.PointerType, ppp);

            Expression expr = EffectiveAddressExpression(mem, state);
            if (arch.ProcessorMode != ProcessorMode.Protected32)
            {
                Expression seg = ReplaceCodeSegment(mem.DefaultSegment, state);
                if (seg == null)
                    seg = AluRegister(mem.DefaultSegment);
                return new SegmentedAccess(MemoryIdentifier.GlobalMemory, seg, expr, dt);
            }
            else
            {
                return new MemoryAccess(MemoryIdentifier.GlobalMemory, expr, dt);
            }
        }

        public Expression CreateMemoryAccess(MemoryOperand memoryOperand, X86State state)
        {
            return CreateMemoryAccess(memoryOperand, memoryOperand.Width, state);
        }

        public MemoryAccess StackAccess(Expression expr, DataType dt)
        {
            if (arch.ProcessorMode != ProcessorMode.Protected32)
            {
                return new SegmentedAccess(MemoryIdentifier.GlobalMemory, AluRegister(Registers.ss), expr, dt);
            }
            else
            {
                return new MemoryAccess(MemoryIdentifier.GlobalMemory, expr, dt);
            }
        }

        /// <summary>
        /// Memory accesses are translated into expressions.
        /// </summary>
        /// <param name="mem"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Expression EffectiveAddressExpression(MemoryOperand mem, X86State state)
        {
            Expression eIndex = null;
            Expression eBase = null;
            Expression expr = null;
            PrimitiveType type = PrimitiveType.CreateWord(mem.Width.Size);

            if (mem.Base != RegisterStorage.None)
            {
                eBase = AluRegister(mem.Base);
                if (expr != null)
                {
                    expr = new BinaryExpression(Operator.IAdd, eBase.DataType, eBase, expr);
                }
                else
                {
                    expr = eBase;
                }
            }

            if (mem.Offset.IsValid)
            {
                if (expr != null)
                {
                    BinaryOperator op = Operator.IAdd;
                    long l = mem.Offset.ToInt64();
                    if (l < 0)
                    {
                        l = -l;
                        op = Operator.ISub;
                    }

                    DataType dt = (eBase != null) ? eBase.DataType : eIndex.DataType;
                    Constant cOffset = Constant.Create(dt, l);
                    expr = new BinaryExpression(op, dt, expr, cOffset);
                }
                else
                {
                    expr = mem.Offset;
                }
            }

            if (mem.Index != RegisterStorage.None)
            {
                eIndex = AluRegister(mem.Index);
                if (mem.Scale != 0 && mem.Scale != 1)
                {
                    eIndex = new BinaryExpression(
                        Operator.IMul, eIndex.DataType, eIndex, Constant.Create(mem.Width, mem.Scale));
                }
                expr = new BinaryExpression(Operator.IAdd, expr.DataType, expr, eIndex);
            }
            return expr;
        }

        public Identifier FlagGroup(FlagM flags)
        {
            return frame.EnsureFlagGroup((uint)flags, arch.GrfToString((uint)flags), PrimitiveType.Byte);
        }


        /// <summary>
        /// Changes the stack-relative address 'reg' into a frame-relative operand.
        /// If the register number is larger than the stack depth, then
        /// the register was passed on the stack when the function was called.
        /// </summary>
        /// <param name="reg"></param>
        /// <returns></returns>
        public Identifier FpuRegister(int reg, X86State state)
        {
            return frame.EnsureFpuStackVariable(reg - state.FpuStackItems, PrimitiveType.Real64);
        }

        public PseudoProcedure ImportedProcedureName(PrimitiveType addrWidth, MemoryOperand mem)
        {
            if (mem != null && addrWidth == PrimitiveType.Word32 && mem.Base == RegisterStorage.None &&
                mem.Index == RegisterStorage.None)
            {
                return (PseudoProcedure)host.GetImportThunkAtAddress(mem.Offset.ToUInt32());
            }
            return null;
        }

        public Constant ReplaceCodeSegment(RegisterStorage reg, X86State state)
        {
            if (reg == Registers.cs && arch.WordWidth == PrimitiveType.Word16)
                return state.GetRegister(reg);
            else
                return null;
        }

        public UnaryExpression AddrOf(Expression expr)
        {
            return new UnaryExpression(Operator.AddrOf,
                PrimitiveType.Create(Domain.Pointer, arch.WordWidth.Size), expr);
        }
    }
}
