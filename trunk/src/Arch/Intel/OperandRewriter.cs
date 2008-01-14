/* 
 * Copyright (C) 1999-2008 John Källén.
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

using Decompiler.Arch.Intel;
using Decompiler.Core.Code;
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using Decompiler.Core;
using System;

namespace Decompiler.Arch.Intel
{
	public class OperandRewriter
	{
		private IRewriterHost host;
		private IntelArchitecture arch;
		private Frame frame;

		public OperandRewriter(IRewriterHost host, IntelArchitecture arch, Frame frame)
		{
			this.host = host;
			this.arch = arch;
			this.frame = frame;
		}

		private Address AbsoluteAddress(MemoryOperand mem)
		{
			if (arch.ProcessorMode != ProcessorMode.ProtectedFlat)
				return null;
			if (mem.Base != Registers.None || mem.Index != Registers.None)
				return null;
			return new Address(0, mem.Offset.Unsigned);
		}

		public UnaryExpression AddrOf(Expression expr)
		{
			return new UnaryExpression(Operator.addrOf, PrimitiveType.Pointer, expr);
		}

		public Identifier AluRegister(MachineRegister reg)
		{
			return frame.EnsureRegister(reg);
		}

		public Identifier AluRegister(IntelRegister reg, PrimitiveType vt)
		{
			return frame.EnsureRegister(reg.GetPart(vt));
		}
		
		/// <summary>
		/// Converts operands from Intel to ILCode. When registers are encountered,
		/// Variables are allocated from the procedure's frame. When stack-based or
		/// frame-based memory accesses are encountered, temporaries are allocated for
		/// them.
		/// </summary>
		/// <param name="opSrc"></param>
		/// <param name="fDefined"></param>
		/// <returns></returns>
		private Expression ConvertNonMemoryOperand(Operand op, PrimitiveType dataWidth, RewriterState state)
		{
			RegisterOperand reg = op as RegisterOperand;
			if (reg != null)
			{
				Constant codeSeg = ReplaceCodeSegment(reg.Register, state);
				if (codeSeg != null)
					return codeSeg;
#if NO_UNNECESSARY_ESCAPES	// A reference to the stack pointer can be left as is.
				if (IsFrameRegisterReference(reg.Register, state))
					frame.Escapes = true;
#endif
				return AluRegister(reg.Register);
			}
			ImmediateOperand imm = op as ImmediateOperand;
			if (imm != null)
			{
				if (dataWidth.BitSize > imm.Width.BitSize)
					return new Constant(dataWidth, imm.val.SignExtend(dataWidth));
				else
					return new Constant(imm.Width, imm.val.Unsigned);
			}
			FpuOperand fpu = op as FpuOperand;
			if (fpu != null)
			{
				return FpuRegister(fpu.StNumber, state);
			}
			MemoryOperand mem = op as MemoryOperand;
			if (mem != null)
			{
				// Stack-based (or frame-based) accesses should be converted to temp variable
				// accesses, but only if there is no index register involved.

				if (IsFrameRegisterReference(mem.Base, state) && mem.Index == Registers.None)
				{
					return frame.EnsureStackVariable(
						mem.Offset,
						IsStackRegister(mem.Base) 
							? state.StackBytes
							: frame.FrameOffset,
						mem.Width);
				}
				return null;
			}
			throw new ArgumentException("Illegal operand type: " + op.GetType().Name);
		}

		public Identifier FlagGroup(FlagM flags)
		{
			return frame.EnsureFlagGroup((uint) flags, arch.GrfToString((uint)flags), PrimitiveType.Byte);
		}

		public Address OperandAsCodeAddress(Operand op, RewriterState state)
		{
			AddressOperand ado = op as AddressOperand;
			if (ado != null)
				return ado.addr;
			ImmediateOperand imm = op as ImmediateOperand;
			if (imm != null)
			{
				if (arch.ProcessorMode == ProcessorMode.ProtectedFlat)
				{
					return new Address(imm.val.Unsigned);
				}
				else
					return new Address(state.CodeSegment, imm.val.Word);
			}
			return null;
		}

		public Expression Transform(Operand opSrc, PrimitiveType dataWidth, PrimitiveType addrWidth, RewriterState state)
		{
			Expression e = ConvertNonMemoryOperand(opSrc, dataWidth, state);
			if (e != null)
				return e;
			MemoryOperand mem = (MemoryOperand) opSrc;
			PseudoProcedure ppp = ImportedProcedureName(addrWidth, mem);
			if (ppp != null)
				return new ProcedureConstant(PrimitiveType.Pointer, ppp);
			Address addr = AbsoluteAddress(mem);
			if (addr != null && host.Image.Map.IsReadOnlyAddress(addr) && mem.Width.Domain == Domain.Real)
			{
				if (mem.Width == PrimitiveType.Real32)
					return host.Image.ReadFloat(addr - host.Image.BaseAddress);
				if (mem.Width == PrimitiveType.Real64)
					return host.Image.ReadDouble(addr - host.Image.BaseAddress);
			}
			return CreateMemoryAccess(mem, state);
		}

		// b => b
		// b,o => (+ b o)
		// o => o


		public MemoryAccess CreateMemoryAccess(MemoryOperand mem, RewriterState state)
		{
			return CreateMemoryAccess(mem, mem.Width, state);
		}

		public MemoryAccess CreateMemoryAccess(MemoryOperand mem, DataType dt, RewriterState state)
		{
			Expression expr = EffectiveAddressExpression(mem, state);
			if (arch.ProcessorMode != ProcessorMode.ProtectedFlat)
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

		public Expression EffectiveAddressExpression(MemoryOperand mem, RewriterState state)
		{
			// Memory accesses are translated into expressions.

			Expression eIndex = null;
			Expression eBase = null;
			Expression expr = null;
			PrimitiveType type = PrimitiveType.CreateWord(mem.Width.Size);

			if (mem.Base != Registers.None)
			{
				eBase = AluRegister(mem.Base);
				if (expr != null)
				{
					expr = new BinaryExpression(Operator.add, eBase.DataType, eBase, expr);
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
					BinaryOperator op = Operator.add;
					long l = mem.Offset.AsLong();
					if (l < 0)
					{
						l = -l;
						op = Operator.sub;
					}
					
					DataType dt = (eBase != null) ? eBase.DataType : eIndex.DataType;
					Constant cOffset = new Constant(dt, l);
					expr = new BinaryExpression(op, dt, expr, cOffset);
				}
				else
				{
					expr = new Constant(mem.Offset);
				}
			}

			if (mem.Index != Registers.None)
			{
				eIndex = AluRegister(mem.Index);
				if (mem.Scale != 0 && mem.Scale != 1)
				{
					eIndex = new BinaryExpression(
						Operator.mul, eIndex.DataType, eIndex, new Constant(mem.Width, mem.Scale));
				}
				if (IsFrameRegisterReference(mem.Base, state))
				{
					frame.Escapes = true;
					Identifier fp = frame.FramePointer;
					int cbOffset = mem.Offset.IsValid ? mem.Offset.Signed : 0;
					Expression fpOff = new BinaryExpression(Operator.add, fp.DataType,
						fp,
						new Constant(fp.DataType, cbOffset -
						(IsStackRegister(mem.Base) ? state.StackBytes : frame.FrameOffset)));
					return new BinaryExpression(Operator.add, mem.Width, fpOff, eIndex);
				}
				expr = new BinaryExpression(Operator.add, expr.DataType, expr, eIndex);
			}
			return expr;
		}

		public MemoryAccess MemoryAccess(Expression ea, PrimitiveType width)
		{
			return new MemoryAccess(MemoryIdentifier.GlobalMemory, ea, width);
		}

		/// <summary>
		/// Changes the stack-relative address 'reg' into a frame-relative operand.
		/// If the register number is larger than the stack depth, then
		/// the register was passed on the stack when the function was called.
		/// </summary>
		/// <param name="reg"></param>
		/// <returns></returns>
		public Identifier FpuRegister(int reg, RewriterState state)
		{
			return frame.EnsureFpuStackVariable(reg - state.FpuStackItems, PrimitiveType.Real64);
		}

		public PseudoProcedure ImportedProcedureName(PrimitiveType addrWidth, MemoryOperand mem)
		{
			if (mem != null && addrWidth == PrimitiveType.Word32 && mem.Base == Registers.None && 
				mem.Index == Registers.None)
			{
				return (PseudoProcedure) host.GetImportThunkAtAddress(new Address(mem.Offset.Unsigned));
			}
			return null;
		}

		public bool IsFrameRegisterReference(MachineRegister reg, RewriterState state)
		{
			return IsStackRegister(reg) || 
				(state.FrameRegister != Registers.None && state.FrameRegister == reg);

		}

		public static bool IsStackRegister(MachineRegister reg)
		{
			return (reg == Registers.sp || reg == Registers.esp);
		}

		public Identifier CreateTemporary(PrimitiveType width)
		{
			return frame.CreateTemporary(width);
		}
	
		public Constant ReplaceCodeSegment(IntelRegister reg, RewriterState state)
		{
			if (reg == Registers.cs && arch.WordWidth == PrimitiveType.Word16)
				return new Constant(PrimitiveType.Word16, state.CodeSegment);
			else 
				return null;

		}
	}
}
