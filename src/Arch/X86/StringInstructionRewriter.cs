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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Types;
using System;

namespace Reko.Arch.X86
{
	/// <summary>
	/// Assists in rewriting X86 string instructions.
	/// </summary>
	public class StringInstructionRewriter
	{
		private CodeEmitter emitter;
		private OperandRewriter orw;
		private IntelArchitecture arch;

		private X86Instruction instrCur;

		public StringInstructionRewriter(IntelArchitecture arch, OperandRewriter orw)
		{
			this.arch = arch;
			this.orw = orw;
		}


		public void EmitStringInstruction(X86Instruction instr, CodeEmitter emitter)
		{
            this.emitter = emitter;

			bool incSi = false;
			bool incDi = false;
			this.instrCur = instr;
			switch (instrCur.code)
			{
				default:
					throw new ApplicationException("NYI");
				case Mnemonic.cmps:
				case Mnemonic.cmpsb:
					emitter.Assign(
						orw.FlagGroup(X86Instruction.DefCc(Mnemonic.cmp)),
						new ConditionOf(
						new BinaryExpression(Operator.ISub, instrCur.dataWidth, MemSi(), MemDi())));
					incSi = true;
					incDi = true;
					break;
				case Mnemonic.lods:
				case Mnemonic.lodsb:
					emitter.Assign(RegAl, MemSi());
					incSi = true;
					break;
				case Mnemonic.movs:
				case Mnemonic.movsb:
				{
					Identifier tmp = emitter.Frame.CreateTemporary(instrCur.dataWidth);
					emitter.Assign(tmp, MemSi());
					emitter.Store(MemDi(), tmp);
					incSi = true;
					incDi = true;
					break;
				}
				case Mnemonic.ins:
				case Mnemonic.insb:
				{
					Identifier regDX = orw.AluRegister(Registers.edx, instrCur.addrWidth);
					emitter.Store(MemDi(), host.PseudoProc("__in", instrCur.dataWidth, regDX));
					incDi = true;
					break;
				}
				case Mnemonic.outs:
				case Mnemonic.outsb:
				{
					Identifier regDX = orw.AluRegister(Registers.edx, instrCur.addrWidth);
					emitter.SideEffect("__out" + RegAl.DataType.Prefix, regDX, RegAl);
					incSi = true;
					break;
				}
				case Mnemonic.scas:
				case Mnemonic.scasb:
					emitter.Assign(
						orw.FlagGroup(X86Instruction.DefCc(Mnemonic.cmp)),
						new ConditionOf(
						new BinaryExpression(Operator.ISub, 
						instrCur.dataWidth,
						RegAl, 
						MemDi())));
					incDi = true;
					break;
				case Mnemonic.stos:
				case Mnemonic.stosb:
                    emitter.Store(MemDi(), RegAl);
					incDi = true;
					break;
			}

			if (incSi)
			{
				emitter.Assign(RegSi,
					new BinaryExpression(Operator.IAdd, 
					instrCur.addrWidth,
					RegSi, 
					Constant.Create(instrCur.addrWidth, instrCur.dataWidth.Size)));
			}

			if (incDi)
			{
				emitter.Assign(RegDi,
					new BinaryExpression(Operator.IAdd, 
					instrCur.addrWidth,
					RegDi, 
					Constant.Create(instrCur.addrWidth, instrCur.dataWidth.Size)));
			}
		}

		public MemoryAccess MemDi()
		{
			if (arch.ProcessorMode != ProcessorMode.ProtectedFlat)
			{
				return new SegmentedAccess(MemoryIdentifier.GlobalMemory, orw.AluRegister(Registers.es), RegDi, instrCur.dataWidth);
			}
			else
				return new MemoryAccess(MemoryIdentifier.GlobalMemory, RegDi, instrCur.addrWidth);
		}

		public MemoryAccess MemSi()
		{
			if (arch.ProcessorMode != ProcessorMode.ProtectedFlat)
			{
				return new SegmentedAccess(MemoryIdentifier.GlobalMemory, orw.AluRegister(Registers.ds), RegSi, instrCur.dataWidth);
			}
			else
				return new MemoryAccess(MemoryIdentifier.GlobalMemory, RegSi, instrCur.dataWidth);
		}
		
		public Identifier RegAl
		{
			get { return orw.AluRegister(Registers.eax, instrCur.dataWidth); }
		}

		public Identifier RegDi
		{
			get { return orw.AluRegister(Registers.edi, instrCur.addrWidth); }
		}

		public Identifier RegSi
		{
			get { return orw.AluRegister(Registers.esi, instrCur.addrWidth); }
		}
	}
}
