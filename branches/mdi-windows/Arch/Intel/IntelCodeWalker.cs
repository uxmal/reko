/* 
 * Copyright (C) 1999-2010 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Machine;
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using System;
using System.Diagnostics;

namespace Decompiler.Arch.Intel
{
	public class IntelCodeWalker : CodeWalker
	{
		private IntelArchitecture arch;
		private IntelDisassembler dasm; 
		private IntelState state;
		private Platform platform;

		public IntelCodeWalker(
			IntelArchitecture arch,
			Platform platform,
			IntelDisassembler dasm,
			IntelState state)
		{
			this.arch = arch;
			this.platform = platform;
			this.dasm = dasm;
			this.state = state;
		}

		public override Address Address
		{
			get { return dasm.Address; }
		}

		private Address AddressFromSegOffset(MachineRegister seg, uint offset)
		{
			return arch.AddressFromSegOffset(state, seg, offset);
		}

		//$REVIEW: consider passing an operator instead, and use "Apply".
		private Constant ExecuteAluOp(Opcode code, MachineOperand op1, MachineOperand op2)
		{
			Constant v1 = GetValue(op1);
			Constant v2 = GetValue(op2);
			if (!v1.IsValid || !v2.IsValid)
				return Constant.Invalid;

			int v;
			switch (code)
			{
			case Opcode.add: v = v1.ToInt32() + v2.ToInt32(); break;
			case Opcode.sub: v = v1.ToInt32() - v2.ToInt32();	break;
			case Opcode.and: v = v1.ToInt32() & v2.ToInt32();	break;
			case Opcode.or:  v = v1.ToInt32() | v2.ToInt32();	break;
			case Opcode.xor: v = v1.ToInt32() ^ v2.ToInt32();	break;
			case Opcode.adc:  case Opcode.sbb: return Constant.Invalid;
			case Opcode.shl: v = v1.ToInt32() << v2.ToInt32(); break;
			case Opcode.shr: v = (int) (v1.ToUInt32() >> v2.ToInt32()); break;
			case Opcode.sar: v = v1.ToInt32() << v2.ToInt32(); break;
			case Opcode.rcl: case Opcode.rcr:
			case Opcode.rol: case Opcode.ror:
				return Constant.Invalid;
			default:
				throw new ArgumentOutOfRangeException();
			}
			return new Constant(v1.DataType, v);
		}

		public Constant GetValue(MachineOperand op)
		{
			RegisterOperand regOp = op as RegisterOperand;
			if (regOp != null)
				return state.Get(regOp.Register);
			ImmediateOperand immOp = op as ImmediateOperand;
			if (immOp != null)
				return immOp.Value;

			return Constant.Invalid;
		}

		private void HandleBranch(Address addrInstr, IntelInstruction instr, Address addrTerm, ICodeWalkerListener listener)
		{
			Address addrBranch = new Address(
				addrTerm.Selector,
				((ImmediateOperand) instr.op1).Value.ToUInt32());
			listener.OnBranch(state, addrInstr, addrTerm, addrBranch);
		}

		// Called upon the parsing of a 'call' opcode.

		private void HandleCallInstruction(IntelInstruction instr, Address addrFrom, Address addrTerm, ICodeWalkerListener listener)
		{
			// Compute address of the callee.

			Address addr; 

			MachineOperand op = instr.op1;
			AddressOperand addrOp = op as AddressOperand;
			if (addrOp != null)
			{
				listener.OnProcedure(state, addrOp.addr);
				return;
			}
			ImmediateOperand immOp = op as ImmediateOperand;
			if (immOp != null)
			{
				addr = AddressFromSegOffset(Registers.cs, immOp.Value.ToUInt32());
				if (addr != null)
				{
					listener.OnProcedure(state, addr);
				}
				return;
			}
			RegisterOperand regOp = op as RegisterOperand;
			if (regOp != null)
			{
				addr = state.AddressFromSegReg(Registers.cs, regOp.Register);
				if (addr != null)
				{
					listener.OnProcedure(state, addr);
					return;
				}
			}

			MemoryOperand memOp = op as MemoryOperand;
			if (memOp != null)
			{
				HandleTransfer(instr, addrFrom, addrTerm, memOp, true, listener);
				return;
			}
		}

		private void HandleIntInstruction(IntelInstruction instr, Address addrStart, ICodeWalkerListener listener)
		{
			int vector = ((ImmediateOperand) instr.op1).Value.ToInt32();
            if (platform == null)
            {
                listener.Warn(addrStart, "Unknown service: INT {0:X2}.", vector);
                return;
            }
            SystemService svc = platform.FindService(vector, state);
			if (svc == null)
			{
				listener.Warn(addrStart, "Unknown service: INT {0:X2}.", vector);
				return;
			}
			listener.OnSystemServiceCall(addrStart, svc);
			switch (vector)
			{
			case 0x20:
				listener.OnProcessExit(dasm.Address);		//$REFACTOR: use platform instead.
				break;
			case 0x21:				// MS-DOS interrupt.
			{
				Constant ah = state.Get(Registers.ah);
				if (ah.IsValid)
				{
					switch (ah.ToUInt32())
					{
					case 0x25:				// Set interrupt vector
					{
						// DS:DX contain the address of an interrupt routine.
						Constant ds = state.Get(Registers.ds);
						Constant dx = state.Get(Registers.dx);
						if (ds.IsValid && dx.IsValid)
						{
							listener.OnProcedure(
								new IntelState(),
								new Address((ushort)ds.ToUInt32(), dx.ToUInt32()));
						}
						break;
					}
					case 0x4C:				// Terminate process.
						listener.OnProcessExit(dasm.Address);
						break;
					default:
						break;
					}
				}
				break;
			}
			}

			// Trash registers as appropriate.

			TrashVariable(svc.Signature.ReturnValue);
			for (int i = 0; i < svc.Signature.FormalArguments.Length; ++i)
			{
				OutArgumentStorage os = svc.Signature.FormalArguments[i].Storage as OutArgumentStorage;
				if (os != null)
				{
					TrashVariable(os.OriginalIdentifier);
				}
			}
		}
		

		public void HandleJmpInstruction(IntelInstruction instr, Address addrFrom, Address addrTerm, ICodeWalkerListener listener)
		{
			AddressOperand addrOp = instr.op1 as AddressOperand;
			if (addrOp != null)
			{
				if (IsBiosRebootAddress(addrOp.addr))
				{
					listener.OnProcessExit(addrTerm);
				}
				else
				{
					// FAR jmps are treated like calls. 

					listener.OnProcedure(state, addrOp.addr);
					listener.OnJump(state, addrFrom, addrTerm, addrTerm);
				}
				return;
			}

			ImmediateOperand immOp = instr.op1 as ImmediateOperand;
			if (immOp != null)
			{
				// local jump within this segment.
				listener.OnJump(state, addrFrom, addrTerm, AddressFromSegOffset(Registers.cs, immOp.Value.ToUInt32()));
				return;
			}

			MemoryOperand memOp = instr.op1 as MemoryOperand;
			if (memOp != null)
			{
				listener.OnJump(state, addrFrom, addrTerm, null);
				HandleTransfer(instr, addrFrom, addrTerm, memOp, false, listener);
				return;
			}

			RegisterOperand regOp = instr.op1 as RegisterOperand;
			if (regOp != null)
			{
				listener.OnJump(state, addrFrom, addrTerm, null);
				return;
			}
			throw new NotImplementedException();
		}

		private void HandleNegInstruction(IntelInstruction instr)
		{
			Constant v = GetValue(instr.op2);
			if (v.IsValid)
			{
				SetValue(instr.op1, Operator.neg.ApplyConstant(v));
			}
		}

		private void HandleLxsInstruction(IntelInstruction instr, MachineRegister seg)
		{
			//$REVIEW: can do more; we know the source operand is a memory address
			// to a pointer value.
			SetValue(instr.op1, Constant.Invalid);
			state.Set(seg, Constant.Invalid);
		}

		private void HandleRepInstruction(Address addrTerm, ICodeWalkerListener listener)
		{
			Address addrBegin = addrTerm - 1;	// To the start of the REPx instruction.
			IntelInstruction instr = dasm.Disassemble();					// Now consume the following opcode.
			switch (instr.code)
			{
			case Opcode.cmps: case Opcode.cmpsb: case Opcode.scas: case Opcode.scasb:
				state.Set(Registers.ecx, Constant.Invalid);			
				break;
			}
			listener.OnBranch(state, addrBegin, dasm.Address, addrBegin);
		}

		private void HandleLeaInstruction(IntelInstruction instr)
		{
			MemoryOperand mem = (MemoryOperand) instr.op2;
			if (mem.IsAbsolute)
			{
				SetValue(instr.op1, mem.Offset);
			}
		}

		private void HandleTransfer(IntelInstruction instr, Address addrInstr, Address addrTerm, MemoryOperand memOp, bool fCall, ICodeWalkerListener listener)
		{
			// Is it indexed or fixed?

			if (memOp.Offset.IsValid)
			{
				Address addrGlob = AddressFromSegOffset(memOp.DefaultSegment, memOp.Offset.ToUInt32());
				ushort segBase = (memOp.Width == PrimitiveType.Word16)
					? (ushort) state.Get(Registers.cs).ToUInt32()
					: (ushort) 0;
				if (memOp.IsAbsolute)
				{
					if (addrGlob != null)
					{
						if (!fCall)
						{
							listener.OnTrampoline(state, addrInstr, addrGlob);
							return;
						}
						listener.OnProcedurePointer(state, addrInstr, addrGlob, memOp.Width);
					}
				}
				else
				{
					if (memOp.Offset.IsValid)
					{
						if (fCall)
						{
							listener.OnProcedureTable(state, addrInstr, addrGlob, segBase, memOp.Width);
						}
						else
						{
							listener.OnJumpTable(state, addrInstr, addrGlob, segBase, memOp.Width);
						}
					}
				}
			}
		}

		private bool IsBiosRebootAddress(Address addr)
		{
			return (addr.Selector == 0xFFFF && addr.Offset == 0x0000);
		}

		private bool IsSameRegister(MachineOperand op1, MachineOperand op2)
		{
			RegisterOperand r1 = op1 as RegisterOperand;
			RegisterOperand r2 = op2 as RegisterOperand;
			return (r1 != null && r2 != null && r1.Register == r2.Register);
		}

		public void SetValue(MachineOperand op, Constant c)
		{
			RegisterOperand regOp = op as RegisterOperand;
			if (regOp != null)
			{
				state.Set(regOp.Register, c);
			}
		}

		public void TrashVariable(Identifier id)
		{
			if (id == null)
				return;
			RegisterStorage reg = id.Storage as RegisterStorage;
			if (reg != null)
			{	
				state.Set(reg.Register, Constant.Invalid);
			}
			SequenceStorage seq = id.Storage as SequenceStorage;
			if (seq != null)
			{
				TrashVariable(seq.Head);
				TrashVariable(seq.Tail);
			}
		}

		/// <summary>
		/// Simulates the execution of an Intel x86 instruction.
		/// </summary>
		/// <returns>The simulated instruction.</returns>
		public override MachineInstruction WalkInstruction(ICodeWalkerListener listener)
		{
			Address addrStart = dasm.Address;
			IntelInstruction instr = dasm.Disassemble();
			Address addrTerm = dasm.Address;
            return WalkInstruction(addrStart, instr, addrTerm, listener);
		}

		public MachineInstruction WalkInstruction(Address addrStart, IntelInstruction instr, Address addrTerm, ICodeWalkerListener listener)
		{
			switch (instr.code)
			{
			default:
				Debug.Assert(false, string.Format("IntelCodeWalker: unhandled opcode '{0}' at address {1}", instr.code, addrStart));
				break;
			case Opcode.aaa:
			case Opcode.aam:
			case Opcode.daa:
				state.Set(Registers.ax, Constant.Invalid);
				break;
			case Opcode.add:
			case Opcode.adc:
			case Opcode.sbb:
			case Opcode.and:
			case Opcode.or:
			case Opcode.sar:
			case Opcode.shr:
			case Opcode.shl:
			case Opcode.rcl:
			case Opcode.rcr:
			case Opcode.rol:
			case Opcode.ror:
				SetValue(instr.op1, ExecuteAluOp(instr.code, instr.op1, instr.op2));
				break;
			case Opcode.xor:
				if (IsSameRegister(instr.op1, instr.op2))
					SetValue(instr.op1, new Constant(instr.op2.Width, 0));
				else
					SetValue(instr.op1, ExecuteAluOp(instr.code, instr.op1, instr.op2));
				break;
			case Opcode.sub:
				if (IsSameRegister(instr.op1, instr.op2))
					SetValue(instr.op1, new Constant(instr.op2.Width, 0));
				else
					SetValue(instr.op1, ExecuteAluOp(instr.code, instr.op1, instr.op2));
				break;
			case Opcode.arpl:
				break;
			case Opcode.bsr:
				SetValue(instr.op1, Constant.Invalid);
				break;
			case Opcode.bswap:
				SetValue(instr.op1, Constant.Invalid);
				break;
			case Opcode.bt:
				SetValue(instr.op1, Constant.Invalid);
				break;
			case Opcode.call:
				HandleCallInstruction(instr, addrStart, addrTerm, listener);
				break;
			case Opcode.cbw:
			{
				Constant t = state.Get(Registers.al);
				if (t.IsValid)
				{
					t = new Constant(PrimitiveType.Word16, t.ToInt32());
				}
				state.Set(Registers.ax, t);
				break;
			}
			case Opcode.cli:
			case Opcode.sti:
			case Opcode.clc:
			case Opcode.cmc:
			case Opcode.stc:
			case Opcode.cld:
			case Opcode.std:
				break;
			case Opcode.cmp:
				break;
			case Opcode.cmps:
			case Opcode.cmpsb:
			case Opcode.movs:
			case Opcode.movsb:
			case Opcode.lods:
			case Opcode.lodsb:
			case Opcode.stos:
			case Opcode.stosb:
			case Opcode.scasb:
			case Opcode.scas:
				break;
			case Opcode.cwd:
			{
				//$BUG: 32-bit registers?
				Constant t = state.Get(Registers.ax);
				if (t.IsValid)
				{
					t = new Constant(PrimitiveType.Word16, t.ToInt32() < 0 ? -1 : 0);
				}
				state.Set(Registers.dx, t);
				break;
			}
			case Opcode.das:
				state.Set(Registers.al, Constant.Invalid);
				state.Set(Registers.C, Constant.Invalid);
				break;
			case Opcode.div:
			case Opcode.idiv:
				break;		//$BUGBUG: trashes many registers in some cases!
			case Opcode.enter:
				state.Push(instr.dataWidth, state.Get(Registers.bp));
				state.Set(Registers.bp, state.Get(Registers.sp));
				break;
			case Opcode.fadd:
			case Opcode.faddp:
			case Opcode.fchs:
			case Opcode.fclex:
			case Opcode.fcom:
			case Opcode.fcomp:
			case Opcode.fcompp:
			case Opcode.fdiv:
			case Opcode.fdivp:
			case Opcode.fdivr:
			case Opcode.fdivrp:
			case Opcode.fiadd:
			case Opcode.fidiv:
			case Opcode.fild:
			case Opcode.fimul:
			case Opcode.fistp:
			case Opcode.fisub:
			case Opcode.fisubr:
			case Opcode.fld:
			case Opcode.fldcw:
			case Opcode.fld1:
			case Opcode.fldln2:
			case Opcode.fldpi:
			case Opcode.fldz:
			case Opcode.fmul:
			case Opcode.fmulp:
			case Opcode.fpatan:
			case Opcode.frndint:
			case Opcode.fsqrt:
			case Opcode.fsin:
			case Opcode.fcos:
			case Opcode.fsincos:
			case Opcode.fst:
			case Opcode.fstcw:
			case Opcode.fstp:
			case Opcode.fstsw:
			case Opcode.fsub:
			case Opcode.fsubp:
			case Opcode.fsubr:
			case Opcode.fsubrp:
			case Opcode.ftst:
			case Opcode.fxch:
			case Opcode.fxam:
			case Opcode.fyl2x:
				break;
			case Opcode.imul:
			case Opcode.mul:
				break;
			case Opcode.@in:
				SetValue(instr.op1, Constant.Invalid);
				break;
			case Opcode.@ins:
			case Opcode.@insb:
				state.Set(Registers.edi, Constant.Invalid);
				break;
			case Opcode.inc:
			case Opcode.dec:
				break;
			case Opcode.@int:
				HandleIntInstruction(instr, addrStart, listener);
				break;
			case Opcode.jnz:
			case Opcode.jz:
			case Opcode.ja:
			case Opcode.jbe:
			case Opcode.jc:
			case Opcode.jcxz:
			case Opcode.jg:
			case Opcode.jge:
			case Opcode.jl:
			case Opcode.jle:
			case Opcode.jno:
			case Opcode.jnc:
			case Opcode.jns:
			case Opcode.jo:
			case Opcode.jpe:
			case Opcode.jpo:
			case Opcode.js:
				HandleBranch(addrStart, instr, addrTerm, listener);
				break;
			case Opcode.jmp:
				HandleJmpInstruction(instr, addrStart, addrTerm, listener);
				break;
			case Opcode.lahf:
				state.Set(Registers.ah, Constant.Invalid);
				break;
			case Opcode.lds:
				HandleLxsInstruction(instr, Registers.ds);
				break;
			case Opcode.lea:
				HandleLeaInstruction(instr);
				break;
			case Opcode.leave:
				state.Set(Registers.bp, state.Pop(instr.dataWidth));
				state.Set(Registers.sp, state.Get(Registers.bp));
				break;
			case Opcode.les:
				HandleLxsInstruction(instr, Registers.es);
				break;
			case Opcode.lfs:
				HandleLxsInstruction(instr, Registers.fs);
				break;
			case Opcode.lgs:
				HandleLxsInstruction(instr, Registers.gs);
				break;
			case Opcode.lss:
				HandleLxsInstruction(instr, Registers.ss);
				break;
			case Opcode.loop:
			case Opcode.loope:
			case Opcode.loopne:
				state.Set(Registers.ecx.GetPart(instr.dataWidth), Constant.Invalid);
				HandleBranch(addrStart, instr, addrTerm, listener);
				break;
			case Opcode.mov:
				SetValue(instr.op1, GetValue(instr.op2));
				break;
			case Opcode.movsx:
			case Opcode.movzx:
				SetValue(instr.op1, Constant.Invalid);		//$REVIEW: could do more here.
				break;
			case Opcode.neg:
				HandleNegInstruction(instr);
				break;
			case Opcode.nop:
				break;
			case Opcode.not:
			{
				Constant t = GetValue(instr.op1);
				if (t.IsValid)
				{
					t = Operator.comp.ApplyConstant(t);
				}
				SetValue(instr.op1, t);
				break;
			}
			case Opcode.@out:
				break;
			case Opcode.outs:
			case Opcode.outsb:
				state.Set(Registers.si, Constant.Invalid);
				break;
			case Opcode.pop:
				SetValue(instr.op1, state.Pop(instr.op1.Width));
				break;
			case Opcode.popa:
				//$BUGBUG: should ram a load of regs off stack.
				break;
			case Opcode.popf:
				state.Pop(instr.dataWidth);
				break;
			case Opcode.push:
				state.Push(instr.op1.Width, GetValue(instr.op1));
				break;
			case Opcode.pusha:
				//$BUGBUG: should ram a load of regs on to stack.
				break;
			case Opcode.pushf:
				state.Push(instr.dataWidth, Constant.Invalid);
				break;
			case Opcode.rep:
			case Opcode.repne:
				HandleRepInstruction(addrTerm, listener);
				break;
			case Opcode.iret:
			case Opcode.ret:
			case Opcode.retf:
				listener.OnReturn(addrTerm);
				break;
			case Opcode.sahf:
				break;
			case Opcode.seta:
			case Opcode.setbe:
			case Opcode.setc:
			case Opcode.setg:
			case Opcode.setge:
			case Opcode.setl:
			case Opcode.setle:
			case Opcode.setnc:
			case Opcode.setno:
			case Opcode.setns:
			case Opcode.setnz:
			case Opcode.seto:
			case Opcode.setpe:
			case Opcode.setpo:
			case Opcode.sets:
			case Opcode.setz:
				SetValue(instr.op1, Constant.Invalid);
				break;
			case Opcode.shrd:
			case Opcode.shld:
				SetValue(instr.op1, Constant.Invalid);
				break;
			case Opcode.test:
				break;
			case Opcode.wait:
				break;
			case Opcode.xchg:
			{
				Constant c = GetValue(instr.op1);
				SetValue(instr.op1, GetValue(instr.op2));
				SetValue(instr.op2, c);
				break;
			}
			case Opcode.xlat:
				break;
			}
			return instr;
		}
	}
}
