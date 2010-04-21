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
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Decompiler.Arch.Intel
{
	public class IntelBackWalker : BackWalker
	{
		private IntelArchitecture arch;
		private bool returnToCaller;
		private MachineRegister regIdxDetected;
        private static TraceSwitch trace = new TraceSwitch("IntelBackWalker", "Traces the progress of x86 backward instruction walking");

		public IntelBackWalker(IntelArchitecture arch, ProgramImage img) : base(img)
		{
			this.arch = arch;
		}

		private void DumpInstructions(List<IntelInstruction> instrs, int idx)
		{
			for (int i = 0; i < instrs.Count; ++i)
			{
				Debug.WriteLineIf(trace.TraceInfo,
					string.Format("{0} {1}", 
					idx == i ? '*' : ' ',
					instrs[i]));
			}
		}

		private MachineRegister HandleAddition(
			MachineRegister regIdx,
			List<BackwalkOperation> operations, 
			RegisterOperand ropDst,
			RegisterOperand ropSrc, 
			ImmediateOperand immSrc, 
			bool add)
		{
			if (ropSrc != null)
			{
				if (ropSrc.Register == ropDst.Register && add)
				{
					operations.Add(new BackwalkOperation(BackwalkOperator.mul, 2));
					return regIdx;
				}		
				else
				{
					return MachineRegister.None;
				}
			} 
			else if (immSrc != null)
			{
				operations.Add(new BackwalkOperation(
					add ? BackwalkOperator.add : BackwalkOperator.sub,
					immSrc.Value.ToInt32()));
				return regIdx;
			}
			else
				return MachineRegister.None;
		}

		public MachineRegister BackwalkInstructions(
            MachineRegister regIdx,
            List<IntelInstruction> instrs, 
            int i, 
            List<BackwalkOperation> operations)
		{
			for (; i >= 0; --i)
			{
				IntelInstruction instr = instrs[i];
				RegisterOperand ropDst = instr.op1 as RegisterOperand;
				RegisterOperand ropSrc = instr.op2 as RegisterOperand;
				ImmediateOperand immSrc = instr.op2 as ImmediateOperand;
				MemoryOperand memSrc = instr.op2 as MemoryOperand;
				switch (instr.code)
				{
				case Opcode.add:
				case Opcode.sub:
					if (ropDst != null && ropDst.Register == regIdx)
					{
						regIdx = HandleAddition(regIdx, operations, ropDst, ropSrc, immSrc, instr.code == Opcode.add);					
					}
					break;
				case Opcode.adc:
					if (ropDst != null && ropDst.Register == regIdx)
					{
						return MachineRegister.None;
					}
					break;
				case Opcode.and:
					if (ropDst != null && ropDst.Register == regIdx && immSrc != null)
					{
						if (IsEvenPowerOfTwo(immSrc.Value.ToInt32() + 1))
						{
							operations.Add(new BackwalkOperation(BackwalkOperator.cmp, immSrc.Value.ToInt32() + 1));
							returnToCaller = true;
						}
						else
						{
							regIdx = MachineRegister.None;
						}
						return regIdx;
					}
					break;
				case Opcode.inc:
				case Opcode.dec:
					if (ropDst != null && ropDst.Register == regIdx)
					{
						BackwalkOperator op = instr.code == Opcode.inc ? BackwalkOperator.add : BackwalkOperator.sub;
						operations.Add(new BackwalkOperation(op, 1));
					}
					break;
				case Opcode.cmp:
					if (ropDst != null && 
						(ropDst.Register == regIdx || ropDst.Register == regIdx.GetPart(PrimitiveType.Byte)))
					{
						if (immSrc != null)
						{
							operations.Add(new BackwalkOperation(BackwalkOperator.cmp, immSrc.Value.ToInt32()));
							return regIdx;
						}
					}
					break;
				case Opcode.ja:
					operations.Add(new BackwalkBranch(ConditionCode.UGT));
					break;
				case Opcode.mov:
					if (ropDst != null)
					{
						if (ropDst.Register == regIdx)
						{
							if (ropSrc != null)
								regIdx = ropSrc.Register;
							else
								regIdx = MachineRegister.None;	// haven't seen an immediate compare yet.
						}
						else if (ropDst.Register == regIdx.GetSubregister(0, 8) &&
							memSrc != null && memSrc.Offset != null &&
							memSrc.Base != MachineRegister.None)
						{
							operations.Add(new BackwalkDereference(memSrc.Offset.ToInt32(), memSrc.Scale));
							regIdx = memSrc.Base;
						}
					}
					break;
				case Opcode.movzx:
					if (ropDst != null && ropDst.Register == regIdx)
					{
						if (memSrc != null)
						{
							return MachineRegister.None;
						}
					}
					break;
				case Opcode.pop:
					if (ropDst != null && ropDst.Register == regIdx)
					{
						return MachineRegister.None;
					}
					break;
				case Opcode.push:
					break;
				case Opcode.shl:
					if (ropDst != null && ropDst.Register == regIdx)
					{
						if (immSrc != null)
						{
							operations.Add(new BackwalkOperation(BackwalkOperator.mul, 1<<immSrc.Value.ToInt32()));
						}
						else
							return MachineRegister.None;
					}
					break;
				case Opcode.test:
					return MachineRegister.None;
				case Opcode.xchg:
					if (ropDst != null && ropSrc != null)
					{
						if (ropDst.Register == regIdx||
							ropSrc.Register == regIdx)
						{
							regIdx = ropDst.Register;
						}
					}
					else
					{
						regIdx = MachineRegister.None;
					}
					break;
				case Opcode.xor:
					if (ropDst != null && ropSrc != null &&
						ropSrc.Register == ropDst.Register &&
						ropDst.Register == regIdx.GetSubregister(8, 8))
					{
						operations.Add(new BackwalkOperation(BackwalkOperator.and, 0xFF));
						regIdx = regIdx.GetSubregister(0, 8);
					}
					break;
				default:
					System.Diagnostics.Debug.WriteLine("Backwalking not supported: " + instr.code);
					DumpInstructions(instrs, i);
					break;
				}
				if (regIdx == MachineRegister.None)
					break;
			}
			return regIdx;
		}

		public MachineRegister FindIndexRegister(MemoryOperand mem)
		{
			if (mem.Base != MachineRegister.None)
			{
				if (mem.Index != MachineRegister.None)
				{
					return MachineRegister.None; // Address expression too complex. //$REVIEW: Emit warning and barf.
				}
				return mem.Base;
			}
			else 
				return mem.Index;
		}

		public override List<BackwalkOperation> BackWalk(Address addrCallJump, IBackWalkHost host)
		{
			Address addrBegin = host.GetBlockStartAddress(addrCallJump);
			if (addrBegin >= addrCallJump)
				throw new ArgumentException("Invalid address range");
	
			List<IntelInstruction> instrs = DisassembleRange(addrBegin, addrCallJump, true);
			IntelInstruction instr = instrs[instrs.Count - 1];

			if (instr.code != Opcode.jmp && instr.code != Opcode.call)
				throw new ArgumentException("Expected backwalk to start at JMP or CALL opcode at address: " + addrCallJump);
			MemoryOperand mem = instr.op1 as MemoryOperand;
			if (mem == null)
				throw new ArgumentException("Expected an indirect JMP or CALL");

			MachineRegister regIdx = FindIndexRegister(mem);
			if (regIdx == MachineRegister.None)
				return null;

            List<BackwalkOperation> operations = new List<BackwalkOperation>();

			// Record the operations done to the idx register.

			if (mem.Scale > 1)
				operations.Add(new BackwalkOperation(BackwalkOperator.mul, mem.Scale));

			returnToCaller = false;

			regIdx = BackwalkInstructions(regIdx, instrs, instrs.Count - 2, operations);
			if (regIdx == MachineRegister.None)
			{
				return null;	// unable to find guard. //$REVIEW: return warning.
			}
			if (!returnToCaller)
			{
				AddressRange range = host.GetSinglePredecessorAddressRange(addrBegin);
				if (range == null)
				{
					return null;	// seems unguarded to me.	//$REVIEW: emit warning.
				}

				instrs = DisassembleRange(range.Begin, range.End, false);
				regIdx = BackwalkInstructions(regIdx, instrs, instrs.Count - 1, operations);
				if (regIdx == MachineRegister.None)
				{
					return null;
				}
			}
			operations.Reverse();
			regIdxDetected = regIdx;
            return operations;
		}

		private List<IntelInstruction> DisassembleRange(Address addrBegin, Address addrEnd, bool inclusive)
		{
			IntelDisassembler dasm = new IntelDisassembler(Image.CreateReader(addrBegin), arch.WordWidth);
            List<IntelInstruction> instrs = new List<IntelInstruction>();
			do
			{
				instrs.Add(dasm.Disassemble());
			} while ((inclusive && dasm.Address <= addrEnd) || dasm.Address < addrEnd);
			return instrs;
		}

		public override MachineRegister IndexRegister
		{
			get { return regIdxDetected; }
		}

		public override Address MakeAddress(PrimitiveType size, ImageReader rdr, ushort segBase)
		{
			if (arch.WordWidth == PrimitiveType.Word16)
			{
				if (size == PrimitiveType.Word16)
				{
					return new Address(segBase, rdr.ReadLeUint16());
				}
				else
				{
					ushort off = rdr.ReadLeUint16();
					ushort seg = rdr.ReadLeUint16();
					return new Address(seg, off);
				}
			} 
			else if (arch.WordWidth == PrimitiveType.Word32)
			{
				Debug.Assert(segBase == 0);
				return new Address(rdr.ReadLeUint32());
			}
			else 
				throw new ApplicationException("Unexpected word width: " + size);
		}
	}
}
