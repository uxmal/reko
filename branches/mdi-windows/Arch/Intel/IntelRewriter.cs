/* 
 * Copyright (C) 1999-2009 John Källén.
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
using System.Collections.Generic;
using System.Diagnostics;

namespace Decompiler.Arch.Intel
{
	public partial class IntelRewriter : Rewriter
	{
		//$REVIEW: accumulating a lot of members; put this class on diet?

		private IntelArchitecture arch;
		private IRewriterHost host;
		private Procedure proc;
		private Frame frame;
		private Address addrEnd;			// address of the end of this block.
		private IntelInstruction instrCur;	// current instruction being rewritten.

		private IntelRewriterState state;
		private int maxFpuStackRead;
		private int maxFpuStackWrite;
		private OperandRewriter orw;
		private StringInstructionRewriter siw;
		private CodeEmitter emitter;
		
		public IntelRewriter(
			IProcedureRewriter prw,
			Procedure proc,
			IRewriterHost host,
			IntelArchitecture arch,
			IntelRewriterState state) : base(prw)
		{
			this.host = host;
			this.proc = proc;
			this.frame = proc.Frame;
			this.arch = arch;
			this.state = state;
			this.orw = new OperandRewriter(host, arch, frame);
			this.siw = new StringInstructionRewriter(arch, orw);
			maxFpuStackRead = -1;
			maxFpuStackWrite = -1;
		}

        public override void GrowStack(int bytes)
        {
            state.GrowStack(bytes);
        }

		/// <summary>
		/// Converts x86 instructions into processor-independent ILCodes. 
		/// Common idioms are translated. 
		/// </summary>
		/// <param name="instrs"></param>
		/// <param name="addrs"></param>
		/// <param name="aDeadFlags"></param>
        public override void ConvertInstructions(MachineInstruction [] instructions,  Address[] addrs, uint[] aDeadFlags, Address addrEnd, CodeEmitter emitter)
        {
            IntelInstruction[] instrs = new IntelInstruction[instructions.Length];
            for (int x = 0; x < instrs.Length; ++x)
            {
                instrs[x] = (IntelInstruction)instructions[x];
            }
            this.emitter = emitter;
            this.addrEnd = addrEnd;


			for (int i = 0; i < instrs.Length; ++i) 
			{
				instrCur = instrs[i];
				state.InstructionAddress = addrs[i];
				Assignment ass = null;
				FlagM useFlags = (FlagM) instrCur.UseCc();
				FlagM defFlags = (FlagM) instrCur.DefCc();
				FlagM deadFlags = (FlagM) aDeadFlags[i];

				switch (instrCur.code)
				{
					default:
						throw new NotImplementedException(string.Format("IntelRewriter doesn't know how to rewrite {0}.", instrCur));
					case Opcode.aaa:
						ass = emitter.Assign(
							orw.FlagGroup(FlagM.CF),
							emitter.PseudoProc("__aaa", PrimitiveType.Bool, orw.AluRegister(Registers.al), orw.AluRegister(Registers.ah), 
							orw.AddrOf(orw.AluRegister(Registers.al)), 
							orw.AddrOf(orw.AluRegister(Registers.ah))));
						break;
					case Opcode.aam:
						ass = emitter.Assign(
							orw.AluRegister(Registers.ax), 
							emitter.PseudoProc("__aam", PrimitiveType.Word16, orw.AluRegister(Registers.ax), 
							SrcOp(instrCur.op1)));
						EmitCcInstr(ass.Dst, defFlags, deadFlags);
						break;
					case Opcode.adc:
						ass = EmitAdcSbb(Operator.add);
						EmitCcInstr(ass.Dst, defFlags, deadFlags);
						break;
					case Opcode.add:
						if (IsStackRegister(instrCur.op1))
						{
							ImmediateOperand imm = instrCur.op2 as ImmediateOperand;
							if (imm != null)
							{
								state.ShrinkStack(imm.Value.ToInt32());
							}
						}
						else 
						{
							Expression dst = EmitAddSub(instrs, i, Opcode.adc, Operator.add);
							EmitCcInstr(dst, defFlags, deadFlags);
						}
						break;
					case Opcode.and:
						ass = EmitBinOp(Operator.and, instrCur.op1, instrCur.op1.Width, instrCur.op1, SrcOp(instrCur.op2));
						EmitCcInstr(ass.Dst, defFlags, deadFlags);
						emitter.Assign(orw.FlagGroup(FlagM.CF), Constant.False());
						break;
					case Opcode.arpl:
						ass = emitter.Assign(
							orw.FlagGroup(FlagM.ZF),
							emitter.PseudoProc("__arpl", PrimitiveType.Bool, SrcOp(instrCur.op1), SrcOp(instrCur.op2),
							orw.AddrOf(SrcOp(instrCur.op1))));
						break;
					case Opcode.bsr:
					{
						Expression src = SrcOp(instrCur.op2);
						emitter.Assign(orw.FlagGroup(FlagM.ZF), emitter.Eq0(src));
						ass = EmitCopy(instrCur.op1, emitter.PseudoProc("__bsr", instrCur.op1.Width, src), false);
						break;
					}
					case Opcode.bswap:
					{
						Identifier reg = (Identifier) orw.AluRegister(((RegisterOperand) instrCur.op1).Register);
						ass = emitter.Assign(reg, emitter.PseudoProc("__bswap", (PrimitiveType) reg.DataType, reg));
						break;
					}
					case Opcode.bt:
						ass = emitter.Assign(orw.FlagGroup(FlagM.CF),
							emitter.PseudoProc("__bt", PrimitiveType.Bool, SrcOp(instrCur.op1), SrcOp(instrCur.op2)));
						break;
					case Opcode.cli:
						// TODO: call EnterCriticalSection
						break;
					case Opcode.call:
						RewriteCall(instrCur.op1, instrCur.op1.Width);
						break;
					case Opcode.cbw:
					{
						if (instrCur.dataWidth == PrimitiveType.Word32)
						{
							emitter.Assign(
								orw.AluRegister(Registers.eax),
								emitter.Cast(PrimitiveType.Int32, orw.AluRegister(Registers.ax)));
						}
						else
						{
							emitter.Assign(
								orw.AluRegister(Registers.ax),
								emitter.Cast(PrimitiveType.Int16, orw.AluRegister(Registers.al)));
						}
						break;
					}
					case Opcode.clc:
						emitter.Assign(orw.FlagGroup(FlagM.CF), Constant.False());
						break;
					case Opcode.cld:
						//$NYI:
						break;
					case Opcode.cmc:
						emitter.Assign(
							orw.FlagGroup(FlagM.CF),
                            emitter.Not(orw.FlagGroup(FlagM.CF)));
						break;
					case Opcode.cmp:
						EmitCmp(defFlags);
						break;
					case Opcode.cmpsb:
					case Opcode.cmps:
						siw.EmitStringInstruction(instrCur, emitter);
						break;
					case Opcode.cwd:
						if (instrCur.dataWidth == PrimitiveType.Word32)
						{
							Identifier edx_eax = frame.EnsureSequence(
								orw.AluRegister(Registers.edx),
								orw.AluRegister(Registers.eax),
								PrimitiveType.Int64);
							emitter.Assign(
								edx_eax, emitter.Cast(edx_eax.DataType, orw.AluRegister(Registers.eax)));
						}
						else
						{
							Identifier dx_ax = frame.EnsureSequence(
								orw.AluRegister(Registers.dx), 
								orw.AluRegister(Registers.ax),
								PrimitiveType.Int32);
							emitter.Assign(
								dx_ax, emitter.Cast(dx_ax.DataType, orw.AluRegister(Registers.ax)));
						}
						break;

					case Opcode.daa:
						EmitDaaDas("__daa");
						break;
					case Opcode.das:
						EmitDaaDas("__das");
						break;
					case Opcode.dec:
						ass = EmitBinOp(
							Operator.sub,
							instrCur.op1,
							instrCur.op1.Width,
							instrCur.op1,
							emitter.Constant(instrCur.op1.Width, 1));
						EmitCcInstr(ass.Dst, defFlags, deadFlags);
						break;
					case Opcode.div:
						ass = EmitDivide(Operator.divu, Domain.UnsignedInt);
						EmitCcInstr(ass.Dst, defFlags, deadFlags);
						break;
					case Opcode.enter:
						RewriteEnter();
						break;
					case Opcode.fadd:
					case Opcode.faddp:
						EmitCommonFpuInstruction(
							Operator.add,
							false,
							instrCur.code == Opcode.faddp);
						break;
                    case Opcode.fchs:
                        {
                            emitter.Assign(
                                orw.FpuRegister(0, state),
                                emitter.Neg(orw.FpuRegister(0, state)));		//$BUGBUG: should be Real, since we don't know the actual size.
                        }
                        WriteFpuStack(0);
                        break;
					case Opcode.fclex:
						emitter.SideEffect(host.EnsurePseudoProcedure("__fclex", PrimitiveType.Void, 0));
						break;
					case Opcode.fcom:
						EmitFcom(0);
						break;
					case Opcode.fcomp:
						EmitFcom(1);
						break;
					case Opcode.fcompp:
						EmitFcom(2);
						break;
					case Opcode.fcos:
						emitter.Assign(
							orw.FpuRegister(0, state),
							emitter.PseudoProc("cos", PrimitiveType.Real64, orw.FpuRegister(0, state)));
						WriteFpuStack(0);
						break;
					case Opcode.fdiv:
					case Opcode.fdivp:
						EmitCommonFpuInstruction(
							Operator.divs, 
							false,
							instrCur.code == Opcode.fdivp);
						break;
					case Opcode.fidiv:
						EmitCommonFpuInstruction(
							Operator.divs, 
							false,
							instrCur.code == Opcode.fdivp,
							PrimitiveType.Real64);
						break;
					case Opcode.fdivr:
					case Opcode.fdivrp:
						EmitCommonFpuInstruction(
							Operator.divs, 
							true,
							instrCur.code == Opcode.fdivrp);
						break;

					case Opcode.fiadd:
						EmitCommonFpuInstruction(
							Operator.add, 
							false,
							false,
							PrimitiveType.Real64);
						break;
					case Opcode.fild:
						state.GrowFpuStack(addrs[i]);
						emitter.Assign(
							orw.FpuRegister(0, state),
							emitter.Cast(PrimitiveType.Real64, SrcOp(instrCur.op1)));
						WriteFpuStack(0);
						break;
					case Opcode.fimul:
						EmitCommonFpuInstruction(
							Operator.muls, 
							false,
							false,
							PrimitiveType.Real64);
						break;
					case Opcode.fistp:
						EmitCopy(instrCur.op1, emitter.Cast(instrCur.op1.Width, orw.FpuRegister(0, state)), false);
						state.ShrinkFpuStack(1);
						break;
					case Opcode.fisub:
					case Opcode.fisubr:
						EmitCommonFpuInstruction(
							Operator.sub,
							instrCur.code == Opcode.fisubr,
							false,
							PrimitiveType.Real64);
						break;

					case Opcode.fld:
						state.GrowFpuStack(addrs[i]);
						emitter.Assign(FpuRegister(0), SrcOp(instrCur.op1));
						WriteFpuStack(0);
						break;
					case Opcode.fld1:
						state.GrowFpuStack(addrs[i]);
						emitter.Assign(FpuRegister(0), new Constant(1.0));
						WriteFpuStack(0);
						break;
					case Opcode.fldcw:
						emitter.SideEffect(
							host.EnsurePseudoProcedure("__fldcw", PrimitiveType.Void, 1),
							SrcOp(instrCur.op1));
						break;
					case Opcode.fstcw:
						EmitCopy(instrCur.op1, emitter.PseudoProc("__fstcw", PrimitiveType.UInt16), false);
						break;
					case Opcode.fldln2:
						state.GrowFpuStack(addrs[i]);
						emitter.Assign(FpuRegister(0), Constant.Ln2());//$REVIEW: look for idioms here.
						WriteFpuStack(0);
						break;
					case Opcode.fldpi:
						state.GrowFpuStack(addrs[i]);
						emitter.Assign(FpuRegister(0), Constant.Pi());
						WriteFpuStack(0);
						break;
					case Opcode.fldz:
						state.GrowFpuStack(addrs[i]);
						emitter.Assign(FpuRegister(0), new Constant(0.0));
						WriteFpuStack(0);
						break;
					case Opcode.fmul:
					case Opcode.fmulp:
						EmitCommonFpuInstruction(
							Operator.muls, 
							false,
							instrCur.code == Opcode.fmulp);
						break;
					case Opcode.fpatan:
					{
						Expression op1 = FpuRegister(1);
						Expression op2 = FpuRegister(0);
						state.ShrinkFpuStack(1);
						emitter.Assign(FpuRegister(0), emitter.PseudoProc("atan", PrimitiveType.Real64, op1, op2));
						WriteFpuStack(0);
						break;
					}
					case Opcode.frndint:
						emitter.Assign(FpuRegister(0), emitter.PseudoProc("__rndint", PrimitiveType.Real64, FpuRegister(0)));
						WriteFpuStack(0);
						break;
					case Opcode.fsin:
						emitter.Assign(FpuRegister(0), emitter.PseudoProc("sin", PrimitiveType.Real64, FpuRegister(0)));
						WriteFpuStack(0);
						break;
					case Opcode.fsincos:
					{
						Identifier itmp = frame.CreateTemporary(PrimitiveType.Real64);
						emitter.Assign(itmp, FpuRegister(0));

						state.GrowFpuStack(addrs[i]);
						emitter.Assign(FpuRegister(1), emitter.PseudoProc("cos", PrimitiveType.Real64, itmp));
						emitter.Assign(FpuRegister(0), emitter.PseudoProc("sin", PrimitiveType.Real64, itmp));
						WriteFpuStack(0);
						WriteFpuStack(1);
						break;
					}
					case Opcode.fsqrt:
						emitter.Assign(FpuRegister(0), emitter.PseudoProc("sqrt", PrimitiveType.Real64, FpuRegister(0)));
						WriteFpuStack(0);
						break;
					case Opcode.fst:
					case Opcode.fstp:
						EmitCopy(instrCur.op1, FpuRegister(0), false);
						if (instrCur.code == Opcode.fstp)
							state.ShrinkFpuStack(1);
						break;
					case Opcode.fstsw:
                        EmitFstsw(instrs, i);
						break;
					case Opcode.fsub:
					case Opcode.fsubp:
						EmitCommonFpuInstruction(
							Operator.sub, 
							false,
							instrCur.code == Opcode.fsubp);
						break;
					case Opcode.fsubr:
					case Opcode.fsubrp:
						EmitCommonFpuInstruction(
							Operator.sub, 
							true,
							instrCur.code == Opcode.fsubrp);
						break;
					case Opcode.ftst:
						emitter.Assign(orw.FlagGroup(FlagM.CF),
							emitter.Sub(FpuRegister(0), new Constant(0.0)));
						break;
					case Opcode.fxam:		//$TODO: need to make this an assignment to C0|C1|C2|C3 = __fxam();
						// idiomatically followed by fstsw &c.
						emitter.SideEffect(host.EnsurePseudoProcedure("__fxam", PrimitiveType.Byte, 0));
						break;

					case Opcode.fyl2x:			//$REVIEW: Candidate for idiom search.
					{
						Identifier op1 = FpuRegister(0);
						Identifier op2 = FpuRegister(1);
						emitter.Assign(op1, emitter.Sub(op2, emitter.PseudoProc("lg2", PrimitiveType.Real64, op1)));
						state.ShrinkFpuStack(1);
						WriteFpuStack(0);
						break;
					}

					case Opcode.idiv:
						ass = EmitDivide(Operator.divs, Domain.SignedInt);
						EmitCcInstr(ass.Dst, defFlags, deadFlags);
						break;

					case Opcode.imul:
						ass = EmitMultiply(Operator.muls, Domain.SignedInt);
						EmitCcInstr(ass.Dst, defFlags, deadFlags);
						break;

					case Opcode.@in:
						EmitCopy(instrCur.op1, emitter.PseudoProc(
							"__in", instrCur.op1.Width, SrcOp(instrCur.op2)), false);
						break;
					case Opcode.inc:
						if (IsStackRegister(instrCur.op1))
						{
							state.ShrinkStack(1);
						}
						else
						{
							ass = EmitBinOp(Operator.add,
								instrCur.op1,
								instrCur.op1.Width, 
								instrCur.op1, 
								emitter.Constant(instrCur.op1.Width, 1));
							EmitCcInstr(ass.Dst, defFlags, deadFlags);
						}
						break;
					case Opcode.ins:
					case Opcode.insb:
						siw.EmitStringInstruction(instrCur, emitter);
						break;
					case Opcode.@int:
						EmitIntInstruction(addrs[i]);
						break;
					case Opcode.iret:
						EmitPop(orw.FlagGroup(FlagM.SF|FlagM.CF|FlagM.ZF|FlagM.OF), instrCur.dataWidth);
						//$BUGBUG: what if in 32-bit mode?
						EmitReturnInstruction(4, 0);
						break;

					case Opcode.jmp:
						EmitJump(addrs[i]);
						break;

					case Opcode.ja:
						EmitBranchInstruction(useFlags, ConditionCode.UGT, instrCur.op1);
						break;
					case Opcode.jbe:
						EmitBranchInstruction(useFlags, ConditionCode.ULE, instrCur.op1);
						break;
					case Opcode.jc:
						EmitBranchInstruction(useFlags, ConditionCode.ULT, instrCur.op1);
						break;
					case Opcode.jge:
						EmitBranchInstruction(useFlags, ConditionCode.GE, instrCur.op1);
						break;
					case Opcode.jg:
						EmitBranchInstruction(useFlags, ConditionCode.GT, instrCur.op1);
						break;
					case Opcode.jl:
						EmitBranchInstruction(useFlags, ConditionCode.LT, instrCur.op1);
						break;
					case Opcode.jle:
						EmitBranchInstruction(useFlags, ConditionCode.LE, instrCur.op1);
						break;
					case Opcode.jnc:
						EmitBranchInstruction(useFlags, ConditionCode.UGE, instrCur.op1);
						break;
					case Opcode.jno:
						EmitBranchInstruction(useFlags, ConditionCode.NO, instrCur.op1);
						break;
					case Opcode.jns:
						EmitBranchInstruction(useFlags, ConditionCode.NS, instrCur.op1);
						break;
					case Opcode.jnz:
						EmitBranchInstruction(useFlags, ConditionCode.NE, instrCur.op1);
						break;
                    case Opcode.jo:
                        EmitBranchInstruction(useFlags, ConditionCode.OV, instrCur.op1);
                        break;
                    case Opcode.jpe:
                        EmitBranchInstruction(useFlags, ConditionCode.PE, instrCur.op1);
                        break;
                    case Opcode.jpo:
                        EmitBranchInstruction(useFlags, ConditionCode.PO, instrCur.op1);
                        break;
                    case Opcode.js:
						EmitBranchInstruction(useFlags, ConditionCode.SG, instrCur.op1);
						break;
					case Opcode.jz:
						EmitBranchInstruction(useFlags, ConditionCode.EQ, instrCur.op1);
						break;
					case Opcode.jcxz:
						EmitBranchInstruction(emitter.Eq0(orw.AluRegister(Registers.ecx, instrCur.dataWidth)), instrCur.op1);
						break;
					case Opcode.lahf:
						emitter.Assign(orw.AluRegister(Registers.ah), orw.FlagGroup(useFlags));
						break;
					case Opcode.lds:
						EmitLxs(Registers.ds);
						break;
					case Opcode.lea:
						EmitLea();
						break;
					case Opcode.leave:
					{
						MachineRegister fp = state.FrameRegister;
						state.LeaveFrame();
						EmitPop(fp);
						break;
					}
					case Opcode.les:
						EmitLxs(Registers.es);
						break;
					case Opcode.lfs:
						EmitLxs(Registers.fs);
						break;
					case Opcode.lgs:
						EmitLxs(Registers.gs);
						break;

					case Opcode.lods:
					case Opcode.lodsb:
						siw.EmitStringInstruction(instrCur, emitter);
						break;

					case Opcode.loope:
						EmitLoopInstruction(FlagM.ZF, ConditionCode.EQ);
						break;
					case Opcode.loopne:
						EmitLoopInstruction(FlagM.ZF, ConditionCode.NE);
						break;
					case Opcode.loop:
						EmitLoopInstruction(0, ConditionCode.EQ);
						break;

					case Opcode.lss:
						EmitLxs(Registers.ss);
						break;
					case Opcode.mov:
                        RewriteMov();
                        break;
					case Opcode.movs:
					case Opcode.movsb:
						siw.EmitStringInstruction(instrCur, emitter);
						break;
					case Opcode.movsx:
						EmitCopy(instrCur.op1, emitter.Cast(PrimitiveType.Create(Domain.SignedInt, instrCur.op1.Width.Size), SrcOp(instrCur.op2)), false);
						break;
					case Opcode.movzx:
						EmitCopy(instrCur.op1, emitter.Cast(instrCur.op1.Width, SrcOp(instrCur.op2)), false);
						break;
					case Opcode.mul:
						ass = EmitMultiply(Operator.mulu, Domain.UnsignedInt);
						EmitCcInstr(ass.Dst, defFlags, deadFlags);
						break;
					case Opcode.neg:
					{
						Expression tmp = EmitUnaryOperator(Operator.neg, instrCur.op1, instrCur.op1);
						defFlags &= ~FlagM.CF;
						EmitCcInstr(tmp, defFlags, deadFlags);
						emitter.Assign(orw.FlagGroup(FlagM.CF), emitter.Eq0(tmp));
						break;
					}
					case Opcode.nop:
						break;

					case Opcode.not:
					{
						Expression tmp = EmitUnaryOperator(Operator.comp, instrCur.op1, instrCur.op1);
						EmitCcInstr(tmp, defFlags, deadFlags);
						break;
					}
					case Opcode.or:
						if (IsSameRegister(instrCur.op1, instrCur.op2))
						{
							// or X,X ==> cmp X,0

							ass = emitter.Assign(orw.FlagGroup(defFlags),
								new ConditionOf(new BinaryExpression(Operator.sub, 
								instrCur.op1.Width,
								SrcOp(instrCur.op1),
								new Constant(instrCur.op1.Width, 0))));
						}
						else
						{
							ass = EmitBinOp(Operator.or, instrCur.op1, instrCur.op1.Width, instrCur.op1, instrCur.op2);
							EmitCcInstr(ass.Dst, defFlags, deadFlags);
						}
						break;

					case Opcode.@out:
						emitter.SideEffect(
							host.EnsurePseudoProcedure("__out" + instrCur.op2.Width.Prefix, PrimitiveType.Void, 2),
							SrcOp(instrCur.op1),
							SrcOp(instrCur.op2));
						break;
					case Opcode.outs:
					case Opcode.outsb:
						siw.EmitStringInstruction(instrCur, emitter);
						break;

					case Opcode.popa:
						Debug.Assert(instrCur.dataWidth == PrimitiveType.Word16 || instrCur.dataWidth == PrimitiveType.Word32);
						if (instrCur.dataWidth == PrimitiveType.Word16)
						{
							EmitPop(Registers.di);
							EmitPop(Registers.si);
							EmitPop(Registers.bp);
							state.ShrinkStack(instrCur.dataWidth.Size);
							EmitPop(Registers.bx);
							EmitPop(Registers.dx);
							EmitPop(Registers.cx);
							EmitPop(Registers.ax);
						}
						else
						{
							EmitPop(Registers.edi);
							EmitPop(Registers.esi);
							EmitPop(Registers.ebp);
							state.ShrinkStack(instrCur.dataWidth.Size);
							EmitPop(Registers.ebx);
							EmitPop(Registers.edx);
							EmitPop(Registers.ecx);
							EmitPop(Registers.eax);
						}
						break;
					case Opcode.pusha:
						if (instrCur.dataWidth == PrimitiveType.Word16)
						{
							EmitPush(Registers.ax);
							EmitPush(Registers.cx);
							EmitPush(Registers.dx);
							EmitPush(Registers.bx);
							EmitPush(Registers.sp);
							EmitPush(Registers.bp);
							EmitPush(Registers.si);
							EmitPush(Registers.di);
						}
						else
						{
							EmitPush(Registers.eax);
							EmitPush(Registers.ecx);
							EmitPush(Registers.edx);
							EmitPush(Registers.ebx);
							EmitPush(Registers.esp);
							EmitPush(Registers.ebp);
							EmitPush(Registers.esi);
							EmitPush(Registers.edi);
						}
						break;
					case Opcode.push:
					{
						RegisterOperand reg = instrCur.op1 as RegisterOperand;
						if (reg != null && reg.Register == Registers.cs)
						{
							if (i + 1 < instrs.Length &&
								instrs[i+1].code == Opcode.call &&
								instrs[i+1].op1.Width == PrimitiveType.Word16)
							{
								state.GrowStack(reg.Register.DataType.Size);
								RewriteCall(instrs[i+1].op1, PrimitiveType.Word32);
								++i;
								break;
							}
							if (i + 3 < instrs.Length &&
								(instrs[i+1].code == Opcode.push && 
								instrs[i+1].op1 is ImmediateOperand) && 
								(instrs[i+2].code == Opcode.push && 
								instrs[i+2].op1 is ImmediateOperand) &&
								(instrs[i+3].code == Opcode.jmp && 
								instrs[i+3].op1 is AddressOperand))
							{
								// That's actually a far call, but the callee thinks its a near call.
								EmitCall(GetProcedureFromInstructionAddr(instrs[i+3].op1, 2, true));
								i += 3; // Skip the pushes and jump that we coalesced.
								break;
							}
						}
						Debug.Assert(instrCur.dataWidth == PrimitiveType.Word16 || instrCur.dataWidth == PrimitiveType.Word32);
						EmitPush(instrCur.dataWidth, instrCur.op1);
						break;
					}
					case Opcode.pop:
						EmitPop(instrCur.op1, instrCur.dataWidth);
						break;

					case Opcode.popf:	//$UNDONE: look for patterns.
						break;
					case Opcode.pushf:
						//$UNDONE: look for patterns (like callf or jump to simulate an ISR).
						break;

					case Opcode.rep:
					case Opcode.repne:
						EmitRepInstruction(instrs, ref i, emitter);
						return;		
					case Opcode.retf:
					case Opcode.ret:
						if (instrCur.Operands == 1)
						{
							// RET pops values off stack: this must be a 'pascal' function.
							proc.Signature.StackDelta = ((ImmediateOperand) instrCur.op1).Value.ToInt32();
						}

						EmitReturnInstruction(
							this.arch.WordWidth.Size + (instrCur.code == Opcode.retf ? PrimitiveType.Word16.Size : 0),
							instrCur.Operands == 1 ? ((ImmediateOperand) instrCur.op1).Value.ToInt32() : 0);
						break;

					case Opcode.rcl:
						EmitRotation("__rcl", true, true, defFlags, deadFlags);
						break;
					case Opcode.rcr:
						EmitRotation("__rcr", true, false, defFlags, deadFlags);
						break;
					case Opcode.rol:
						EmitRotation("__rol", false, true, defFlags, deadFlags);
						break;
					case Opcode.ror:
						EmitRotation("__ror", false, false, defFlags, deadFlags);
						break;
					case Opcode.sahf:
						emitter.Assign(orw.FlagGroup(defFlags), orw.AluRegister(Registers.ah));
						break;
					case Opcode.sar:
						ass = EmitBinOp(Operator.sar, instrCur.op1, instrCur.op1.Width, instrCur.op1, instrCur.op2);
						EmitCcInstr(ass.Dst, defFlags, deadFlags);
						break;
					case Opcode.sbb:
						if (IsSameRegister(instrCur.op1, instrCur.op2))
						{
							// sbb ecx,ecx => ecx = 0 - C
							ass = EmitCopy(
								instrCur.op1,
								new BinaryExpression(Operator.sub,
								instrCur.dataWidth,
								new Constant(instrCur.dataWidth, 0),
								orw.FlagGroup(FlagM.CF)),
								false);
						}
						else
						{
							ass = EmitAdcSbb(Operator.sub);
						}
						EmitCcInstr(ass.Dst, defFlags, deadFlags);
						break;	
					case Opcode.scas:
					case Opcode.scasb:
						siw.EmitStringInstruction(instrCur, emitter);
						break;
					case Opcode.setg:
						EmitSet(instrCur.op1, ConditionCode.GT, useFlags);
						break;
					case Opcode.setge:
						EmitSet(instrCur.op1, ConditionCode.GE, useFlags);
						break;
					case Opcode.setl:
						EmitSet(instrCur.op1, ConditionCode.LT, useFlags);
						break;
					case Opcode.setle:
						EmitSet(instrCur.op1, ConditionCode.LE, useFlags);
						break;
					case Opcode.setnz:
						EmitSet(instrCur.op1, ConditionCode.NE, useFlags);
						break;
					case Opcode.sets:
						EmitSet(instrCur.op1, ConditionCode.SG, useFlags);
						break;
					case Opcode.setz:
						EmitSet(instrCur.op1, ConditionCode.EQ, useFlags);
						break;

					case Opcode.shl:
						ass = EmitBinOp(Operator.shl, instrCur.op1, instrCur.op1.Width, instrCur.op1, SrcOp(instrCur.op2));
						EmitCcInstr(ass.Dst, defFlags, deadFlags);
						break;
					case Opcode.shld:
						EmitCopy(instrCur.op1, emitter.PseudoProc("__shld", instrCur.op1.Width, 
							SrcOp(instrCur.op1),
							SrcOp(instrCur.op2),
							SrcOp(instrCur.op3)), false);
						break;
					case Opcode.shr:
						ass = EmitBinOp(Operator.shr, instrCur.op1, instrCur.op1.Width, instrCur.op1, SrcOp(instrCur.op2));
						EmitCcInstr(ass.Dst, defFlags, deadFlags);
						break;
					case Opcode.shrd:
						EmitCopy(instrCur.op1, emitter.PseudoProc("__shrd", instrCur.op1.Width, 
							SrcOp(instrCur.op1),
							SrcOp(instrCur.op2),
							SrcOp(instrCur.op3)), false);
						break;

					case Opcode.stc:
						emitter.Assign(orw.FlagGroup(FlagM.CF), Constant.True());
						break;
					case Opcode.std:
						break;
					case Opcode.sti:
						break;	//$BUG: leavecriticalsection?
					case Opcode.stos:
					case Opcode.stosb:
						siw.EmitStringInstruction(instrCur, emitter);
						break;			
					case Opcode.sub:
						if (IsStackRegister(instrCur.op1))
						{
							ImmediateOperand imm = instrCur.op2 as ImmediateOperand;
							if (imm != null)
							{
								state.GrowStack(imm.Value.ToInt32());
							}
						}
						else 
						{
							Expression d;
							if (IsSameRegister(instrCur.op1, instrCur.op2))
							{
								// sub r,r ==>  mov r,0
								ass = EmitCopy(instrCur.op1, new Constant(instrCur.op1.Width, 0), false);
								d = ass.Dst;
							}
							else
							{
								d = EmitAddSub(instrs, i, Opcode.sbb, Operator.sub);
							}
							EmitCcInstr(d, defFlags, deadFlags);
						}
						break;

					case Opcode.test:
					{
						Expression src;
						if (IsSameRegister(instrCur.op1, instrCur.op2))
						{
							// test X,X ==> cmp X,0
							src = new BinaryExpression(Operator.sub, 
								instrCur.op1.Width,
								SrcOp(instrCur.op1),
								new Constant(instrCur.dataWidth, 0));
						}
						else
						{
							src = new BinaryExpression(Operator.and,
								instrCur.op1.Width,
								SrcOp(instrCur.op1),
								SrcOp(instrCur.op2));
						}
						emitter.Assign(orw.FlagGroup(defFlags), new ConditionOf(src));
						emitter.Assign(orw.FlagGroup(FlagM.CF), Constant.False());
						break;
					}
					case Opcode.wait:	// used to slow down FPU.
						break;
					case Opcode.fxch:
					case Opcode.xchg:   //$ maybe lock ?
					{
						Identifier itmp = frame.CreateTemporary(instrCur.dataWidth);
						emitter.Assign(itmp, SrcOp(instrCur.op1));
						EmitCopy(instrCur.op1, SrcOp(instrCur.op2), false);
						EmitCopy(instrCur.op2, itmp, false);
						break;
					}

					case Opcode.xlat:
						emitter.Assign(
							orw.AluRegister(Registers.al),
							orw.MemoryAccess(
							emitter.Add(
							orw.AluRegister(Registers.bx),
							orw.AluRegister(Registers.al)), PrimitiveType.Byte));			//$REVIEW: should zero-extend al
						break;
					case Opcode.xor:
						if (IsSameRegister(instrCur.op1, instrCur.op2))
						{
							// xor r,r ==>  mov r,0
							ass = EmitCopy(instrCur.op1, new Constant(instrCur.op1.Width, 0), false);
						}
						else
						{
							ass = EmitBinOp(Operator.xor, instrCur.op1, instrCur.op1.Width, instrCur.op1, SrcOp(instrCur.op2));
						}
						EmitCcInstr(ass.Dst, defFlags, deadFlags);
						break;
				}
			}
		}

		private void BuildApplication(PseudoProcedure ppp)
		{
			Expression e = new ProcedureConstant(PrimitiveType.Create(Domain.Pointer, arch.WordWidth.Size), ppp);
			BuildApplication(e, ppp.Signature);
		}

		private void BuildApplication(Expression fn, ProcedureSignature sig)
		{
            ApplicationBuilder ab = new ApplicationBuilder(
                frame, 
                new CallSite(state.StackBytes, state.FpuStackItems),
                fn,
                sig);
            state.ShrinkStack(sig.StackDelta);
            state.ShrinkFpuStack(sig.FpuStackDelta);
            emitter.Emit(ab.CreateInstruction());
		}

		private Assignment EmitAdcSbb(BinaryOperator opr)
		{
			Identifier tmp = frame.CreateTemporary(instrCur.op1.Width);
			emitter.Assign(tmp, new BinaryExpression(
				opr,
				instrCur.op1.Width,
				SrcOp(instrCur.op1),
				SrcOp(instrCur.op2)));
			Cast c = emitter.Cast(instrCur.op1.Width, orw.FlagGroup(FlagM.CF));
			return EmitCopy(instrCur.op1, new BinaryExpression(opr, tmp.DataType, tmp, c), true);
		}


		/// <summary>
		/// Handles the x86 idiom add ... adc => long add (and 
		/// sub ..sbc => long sub)
		/// </summary>
		/// <param name="i"></param>
		/// <param name="next"></param>
		/// <returns></returns>
		public Expression EmitAddSub(IntelInstruction [] instrs, int i, Opcode next, BinaryOperator op)
		{
            LongAddRewriter larw = new LongAddRewriter(this.frame, orw, state);
			int iUse = larw.IndexOfUsingOpcode(instrs, i, next);
			if (iUse >= 0 && larw.Match(instrCur, instrs[iUse]))
			{
				instrs[iUse].code = Opcode.nop;
                larw.EmitInstruction(op, emitter);
				return larw.Dst;
			}
			Assignment ass = EmitBinOp(op, instrCur.op1, instrCur.op1.Width, instrCur.op1, instrCur.op2);
			return ass.Dst;
		}

		/// <summary>
		/// Breaks up very common case of x86:
		/// <code>
		///		op [memaddr], reg
		/// </code>
		/// into the equivalent:
		/// <code>
		///		tmp := [memaddr] op reg;
		///		store([memaddr], tmp);
		/// </code>
		/// </summary>
		/// <param name="opDst"></param>
		/// <param name="src"></param>
		/// <param name="forceBreak">if true, forcibly splits the assignments in two if the destination is a memory store.</param>
		/// <returns></returns>
		public Assignment EmitCopy(MachineOperand opDst, Expression src, bool forceBreak)
		{
			Expression dst = SrcOp(opDst);
			Identifier idDst = dst as Identifier;
			Assignment ass;
			if (idDst != null || !forceBreak)
			{
				MemoryAccess acc = dst as MemoryAccess;
				if (acc != null)
				{
					emitter.Store(acc, src);
					ass = null;			//$REVIEW: is this ever used?
				}
				else
				{
					ass = emitter.Assign(idDst, src);
				}
			}
			else
			{
				Identifier tmp = frame.CreateTemporary(opDst.Width);
				ass = emitter.Assign(tmp, src);
				MemoryAccess ea = orw.CreateMemoryAccess((MemoryOperand) opDst, state);
				emitter.Store(ea, tmp);
			}
			return ass;
		}

		public Assignment EmitBinOp(BinaryOperator binOp, MachineOperand dst, DataType dtDst, MachineOperand left, Expression right)
		{
			Expression eLeft = SrcOp(left);
			Constant c = right as Constant;
			if (c != null)
			{
				if (c.DataType == PrimitiveType.Byte && eLeft.DataType != c.DataType)
				{
					right = emitter.Constant(eLeft.DataType, c.ToInt32());
				}
			}

			return EmitCopy(dst, new BinaryExpression(binOp, dtDst, eLeft, right), true);
		}

		public Assignment EmitBinOp(BinaryOperator binOp, MachineOperand dst, DataType dtDst, MachineOperand left, MachineOperand right)
		{
			return EmitBinOp(binOp, dst, dtDst, left, SrcOp(right));
		}

		private void EmitBranchInstruction(FlagM usedFlags, ConditionCode cc, MachineOperand opTarget)
		{
			EmitBranchInstruction(new TestCondition(cc, orw.FlagGroup(usedFlags)), opTarget);
		}

		private void EmitBranchInstruction(Identifier idFlag, ConditionCode cc, MachineOperand opTarget)
		{
			EmitBranchInstruction(new TestCondition(cc, idFlag), opTarget);
		}

		
        private Block EmitBranchInstruction(Expression branchCondition, MachineOperand opTarget)
        {
            // The first (0'th) branch is the path taken when "falling through"
            // a conditional expression.
            // The second (1'th) outward bound branch is the path we take 
            // when the condition the opcode tests for is true.

            CodeEmitter e = emitter;
            Block blockHead = e.Block;
            EmitBranchPath(blockHead, addrEnd);
            Block blockTarget = EmitBranchPath(blockHead, orw.OperandAsCodeAddress(opTarget, state));
            e.Branch(branchCondition, blockTarget);
            return blockTarget;
        }

        //$REFACTOR: move to ProcedureRewriter.
		private Block EmitBranchPath(Block blockHead, Address addrTo)
		{
            emitter = ProcedureRewriter.CreateEmitter(blockHead);
			Block blockTo;
			Procedure p = host.GetProcedureAtAddress(addrTo, frame.ReturnAddressSize);
			if (p != null && p != this.proc)
			{
				// A branch to a procedure header other than the current procedure 
				// is translated into a CALL proc, RETURN sequence.
				Block block = emitter.Block;
				blockTo = proc.AddBlock(state.InstructionAddress.GenerateName("l", "_branch"));
				proc.AddEdge(block, blockTo);
                CodeEmitter e = emitter;
                emitter = ProcedureRewriter.CreateEmitter(blockTo);
				EmitCallAndReturn(p);       //$REFACTOR: pass emitter.
                emitter = e;
			}
			else
			{
                blockTo = ProcedureRewriter.RewriteBlock(addrTo, blockHead,
                    new IntelRewriter(
                        this.ProcedureRewriter,
                        proc,
                        host,
                        arch,
                        state.Clone()));
			}
			return blockTo;
		}


		private void EmitCall(Procedure procCallee)
		{
            CallSite site = new CallSite(state.StackBytes, state.FpuStackItems);
			if (procCallee.Characteristics.IsAlloca)
			{
				if (procCallee.Signature == null)
					throw new ApplicationException(string.Format("You must specify a procedure signature for {0} since it has been marked as 'alloca'.", proc.Name));
				Identifier id = 
					procCallee.Signature.FormalArguments[0].Storage.BindFormalArgumentToFrame(this.frame, site);
				Constant c = SearchBackForConstantAssignment(id);
				if (c != null)
				{
					int size = c.ToInt32();
					state.GrowStack(size);
					Identifier idBuf = frame.EnsureStackLocal(state.StackBytes, new StructureType(null, size));	
					emitter.Assign(id, new UnaryExpression(Operator.addrOf, id.DataType, idBuf));
					return;
				}
			}
			emitter.Call(procCallee, site);
			state.ShrinkStack(procCallee.Signature.StackDelta);
			state.ShrinkFpuStack(-procCallee.Signature.FpuStackDelta);
			host.AddCallEdge(this.proc, emitter.Block.Statements.Last, procCallee);
		}


		/// <summary>
		/// Converts a jump to a different procedure to a CALL/RET instruction pair.
		/// </summary>
		/// <param name="proc"></param>
		public override void EmitCallAndReturn(Procedure callee)
		{
			EmitCall(callee);
			EmitReturnInstruction(callee.Frame.ReturnAddressSize, callee.Signature.StackDelta); 
		}

		
		/// <summary>
		/// Emits an assignment to a flag-group pseudoregister if the Intel instruction
		/// we translated modifies flags that may not be dead -- ie. the flags the instruction
		/// *may* be used downstream.
		/// </summary>
		/// <param name="v"></param>
		/// <param name="defFlags">flags defined by the intel instruction</param>
		/// <param name="deadFlags">flags not used by the following instructions</param>
		private void EmitCcInstr(Expression expr, FlagM defFlags, FlagM deadFlags)
		{
			if ((defFlags & deadFlags) != defFlags)
			{
				emitter.Assign(orw.FlagGroup(defFlags), new ConditionOf(expr.CloneExpression()));
			}
		}


		public void EmitCmp(FlagM defFlags)
		{
			Expression op1 = SrcOp(instrCur.op1);
			Expression op2 = SrcOp(instrCur.op2, instrCur.op1.Width);
			Constant c = op2 as Constant;
			if (c != null && op1.DataType != op2.DataType)
			{
				op2 = emitter.Constant(op1.DataType, c.ToInt32());
			}

			emitter.Assign(
				orw.FlagGroup(defFlags),
				new ConditionOf(
				new BinaryExpression(Operator.sub, 
				instrCur.op1.Width,
				op1,
				op2)));
		}

		public void EmitCommonFpuInstruction(
			BinaryOperator op,
			bool fReversed,
			bool fPopStack)
		{
			EmitCommonFpuInstruction(op, fReversed, fPopStack, null);
		}
		
		public void EmitCommonFpuInstruction(
			BinaryOperator op,
			bool fReversed,
			bool fPopStack,
			DataType cast)
		{
			switch (instrCur.Operands)
			{
				default:
					throw new ArgumentOutOfRangeException("instrCur", "Instruction must have 1 or 2 operands");
				case 1:
				{
					// implicit st(0) operand.
					Identifier opLeft = FpuRegister(0);
					Expression opRight = SrcOp(instrCur.op1);
					if (fReversed)
					{
						EmitCopy(instrCur.op1, new BinaryExpression(op, instrCur.dataWidth, opRight, MaybeCast(cast, opLeft)), true);
					}
					else
					{
						emitter.Assign(opLeft, new BinaryExpression(op, instrCur.dataWidth, opLeft, MaybeCast(cast, opRight)));
					}
					break;
				}
				case 2:
				{
					Expression op1 = SrcOp(instrCur.op1);
					Expression op2 = SrcOp(instrCur.op2);
					EmitCopy(
						instrCur.op1,
						new BinaryExpression(op, 
						instrCur.op1.Width,
						fReversed ? op2 : op1,
						fReversed ? op1 : op2), false);
					break;
				}
			}

			if (fPopStack)
			{
				state.ShrinkFpuStack(1);
			}
		}



		private void EmitDaaDas(string fnName)
		{
			emitter.Assign(orw.FlagGroup(FlagM.CF), emitter.PseudoProc(
				fnName,
				PrimitiveType.Bool, 
				orw.AluRegister(Registers.al),
				orw.AddrOf(orw.AluRegister(Registers.al))));
		}

		private Assignment EmitDivide(BinaryOperator op, Domain domain)
		{
			Assignment ass = null;
			if (instrCur.Operands != 1)
				throw new ArgumentOutOfRangeException("Intel DIV/IDIV instructions only take one operand");
			Identifier regDividend;
			Identifier regQuotient;
			Identifier regRemainder;

			switch (instrCur.dataWidth.Size)
			{
				case 1:
					regQuotient  = orw.AluRegister(Registers.al);
					regDividend  = orw.AluRegister(Registers.ax);
					regRemainder = orw.AluRegister(Registers.ah);
					break;
				case 2:
					regQuotient  = orw.AluRegister(Registers.ax);
					regRemainder = orw.AluRegister(Registers.dx);
					regDividend  = frame.EnsureSequence(regRemainder, regQuotient, PrimitiveType.Word32);
					break;
				case 4:
					regQuotient  = orw.AluRegister(Registers.eax);
					regRemainder = orw.AluRegister(Registers.edx);
					regDividend  = frame.EnsureSequence(regRemainder, regQuotient, PrimitiveType.Word64);
					break;
				default:
					throw new ArgumentOutOfRangeException(string.Format("{0}-byte divisions not supported", instrCur.dataWidth.Size));
			};
			PrimitiveType p = ((PrimitiveType) regRemainder.DataType).MaskDomain(domain);
			emitter.Assign(
				regRemainder, new BinaryExpression(Operator.mod, p,
				regDividend,
				SrcOp(instrCur.op1)));
			ass = emitter.Assign(
				regQuotient, new BinaryExpression(op, p, regDividend, 
				SrcOp(instrCur.op1)));
			return ass;
		}

		private void EmitFcom(int pops)
		{
			Identifier op1 = FpuRegister(0);
			Expression op2 = (instrCur.code == Opcode.fcompp)
				? FpuRegister(1)
				: SrcOp(instrCur.op1);
            emitter.Assign(
                orw.FlagGroup(FlagM.FPUF),
                new ConditionOf(
                    new BinaryExpression(Operator.sub, instrCur.dataWidth, op1, op2)));
			state.ShrinkFpuStack(pops);
		}


		private void EmitIndirectCall(Address addrInstr)
		{
			Expression e = SrcOp(instrCur.op1);
			ProcedureConstant pc = e as ProcedureConstant;
			if (pc != null)
			{
				PseudoProcedure ppp = (PseudoProcedure) pc.Procedure;
				BuildApplication(ppp);
				return;
			}

			Identifier id = e as Identifier;
			if (id != null)
			{
				PseudoProcedure ppp = SearchBackForProcedureConstant(id);
				if (ppp != null)
				{
					BuildApplication(ppp);
					return;
				}
			}

            // Near indirect calls must refer to CS.
            if (e.DataType == PrimitiveType.Word16)
            {
                e = new MkSequence(PrimitiveType.Pointer32, orw.AluRegister(Registers.cs), e);
            }

			//$BUGBUG: we need the size of the return address.
			ProcedureSignature sig = host.GetCallSignatureAtAddress(addrInstr);
			if (sig != null)
			{
                BuildApplication(e, sig);
			}
			else
			{
                emitter.IndirectCall(e, new CallSite(state.StackBytes, state.FpuStackItems));
			}
			Procedure [] procs = host.GetProceduresFromVector(addrInstr, instrCur.op1.Width.Size);
			for (int j = 0; j < procs.Length; ++j)
			{
				host.AddCallEdge(proc, emitter.Block.Statements.Last, procs[j]);
			}
		}

		private void EmitIntInstruction(Address addr)
		{
			SystemService svc = host.SystemCallAt(addr);
			if (svc != null)
			{
				ExternalProcedure ep = svc.CreateExternalProcedure(arch);
				ProcedureConstant fn = new ProcedureConstant(arch.PointerType, ep);
				BuildApplication(fn, ep.Signature);
				if (svc.Characteristics.Terminates)
				{
					proc.AddEdge(emitter.Block, proc.ExitBlock);
				}
			}
			else
			{
				//$REVIEW: log a warning: unknown int instruction.
				emitter.SideEffect(host.EnsurePseudoProcedure("__syscall", PrimitiveType.Void, 1), SrcOp(instrCur.op1));
			}
		}
		
		private void EmitJump(Address addrInstr)
		{
			// Hard-code jumps to 0xFFFF:0x0000 in real mode to reboot.

			AddressOperand addrOp = instrCur.op1 as AddressOperand;
			if (addrOp != null && addrOp.addr.Linear == 0xFFFF0)
			{
				PseudoProcedure reboot = host.EnsurePseudoProcedure("__bios_reboot", PrimitiveType.Void, 0);
				reboot.Characteristics = new Decompiler.Core.Serialization.ProcedureCharacteristics();
				reboot.Characteristics.Terminates = true;
				emitter.SideEffect(reboot);
				return;
			}

			// Jumps to procedure are converted to calls -- but not if they are jumps
			// to the same procedure!
	
			Procedure p = GetProcedureFromInstructionAddr(instrCur.op1, frame.ReturnAddressSize, false);
			if (p != null)
			{
				EmitCallAndReturn(p);
				return;
			}
				
			if (instrCur.op1 is ImmediateOperand)
			{
				Address addr = orw.OperandAsCodeAddress(instrCur.op1, state);
				Block blockFrom = emitter.Block;
                Block blockTo = ProcedureRewriter.RewriteBlock(addr, blockFrom,
                    new IntelRewriter(ProcedureRewriter, proc, host, arch, state.Clone()));
				return;
			}

			VectorUse vu = host.VectorUseAt(addrInstr);
			if (vu != null && vu.IndexRegister != null)
			{
                ImageMapItem t;
                if (host.Image.Map.TryFindItemExact(vu.TableAddress, out t))
                {
                    ImageMapVectorTable table = (ImageMapVectorTable) t;
					EmitSwitch(vu.IndexRegister, table);
					return;
				}
			}

			EmitIndirectCall(addrInstr);
			EmitReturnInstruction(0, 0);
		}


		public void EmitLea()
		{
			Expression src;
			MemoryOperand mem = (MemoryOperand) instrCur.op2;
			if (mem.Base == MachineRegister.None && mem.Index == MachineRegister.None)
			{																			   
				src = mem.Offset;
			}
			else
			{
				src = SrcOp(instrCur.op2);
				MemoryAccess load = src as MemoryAccess;
				if (load != null)
				{
					src = load.EffectiveAddress;
				}
				else
				{
					src = orw.AddrOf(src);
				}
			}
			EmitCopy(instrCur.op1, src, false);
		}

		private void EmitLoopInstruction(FlagM useFlags, ConditionCode cc)
		{
			Block blockHead = emitter.Block;
			Block blockNew = null;
	
			Identifier cx = orw.AluRegister(Registers.ecx, instrCur.dataWidth);
			emitter.Assign(cx, emitter.Sub(cx, 1));
			Identifier flag;
			if (useFlags != 0)
			{
                CodeEmitter e = emitter;

				// Splice in a new block.

				blockNew = proc.AddBlock(state.InstructionAddress.GenerateName("l","_loop"));
				proc.AddEdge(blockHead, blockNew);

				emitter = ProcedureRewriter.CreateEmitter(blockNew);
				Block tgt = EmitBranchInstruction(emitter.Eq0(cx), instrCur.op1);
                e.Branch(new TestCondition(cc, orw.FlagGroup(useFlags)), tgt);
			}
			else
			{
				EmitCcInstr(cx, FlagM.ZF, 0);
				flag = orw.FlagGroup(FlagM.ZF);
				EmitBranchInstruction(flag, ConditionCode.NE, instrCur.op1);
			}
			if (blockNew != null)
			{
				proc.AddEdge(blockHead, blockNew.Succ[1]);
			}
		}

		private void EmitLxs(IntelRegister seg)
		{
			RegisterOperand reg = (RegisterOperand) instrCur.op1;
			MemoryOperand mem = (MemoryOperand) instrCur.op2;
			if (!mem.Offset.IsValid)
			{
				mem = new MemoryOperand(mem.Width, mem.Base, mem.Index, mem.Scale, new Constant(instrCur.addrWidth, 0));
			}

			Assignment ass = emitter.Assign(
				frame.EnsureSequence(orw.AluRegister(seg), orw.AluRegister(reg.Register),
				PrimitiveType.Pointer32),
				SrcOp(mem));
			ass.Src.DataType = PrimitiveType.Pointer32;
		}


		private Assignment EmitMultiply(BinaryOperator op, Domain resultDomain)
		{
			Assignment ass; 
			switch (instrCur.Operands)
			{
				case 1:
				{
					Identifier multiplicator;
					Identifier product;

					switch (instrCur.op1.Width.Size)
					{
						case 1:
							multiplicator = orw.AluRegister(Registers.al);
							product = orw.AluRegister(Registers.ax);
							break;
						case 2:
							multiplicator = orw.AluRegister(Registers.ax);
							product = frame.EnsureSequence(
								orw.AluRegister(Registers.dx), multiplicator, PrimitiveType.Word32);
							break;
						case 4:
							multiplicator = orw.AluRegister(Registers.eax);
							product = frame.EnsureSequence(
								orw.AluRegister(Registers.edx), multiplicator, PrimitiveType.Word64);
							break;
						default:
							throw new ApplicationException(string.Format("Unexpected operand size: {0}", instrCur.op1.Width));
					};
					return emitter.Assign(product,
						new BinaryExpression(op, PrimitiveType.Create(resultDomain, product.DataType.Size), SrcOp(instrCur.op1), multiplicator));
				}

				case 2:
					ass = EmitBinOp(op, instrCur.op1, instrCur.op1.Width.MaskDomain(resultDomain), instrCur.op1, SrcOp(instrCur.op2));
					break;
				case 3:
					ass = EmitBinOp(op, instrCur.op1, instrCur.op1.Width.MaskDomain(resultDomain), instrCur.op2, SrcOp(instrCur.op3));
					break;
				default:
					throw new ArgumentException("Invalid number of operands");
			}
			return ass;
		}

		private void EmitPop(MachineRegister reg)
		{
			EmitPop(new RegisterOperand(reg), reg.DataType);
		}

		// A pop is a move from a temporary register to a register, with
		// the side effect that the temporary register is destroyed.
		private void EmitPop(MachineOperand op, PrimitiveType width)
		{
			Identifier local = frame.EnsureStackLocal(-state.StackBytes, width);
			RegisterOperand reg = op as RegisterOperand;
			if (reg != null && orw.IsFrameRegisterReference(reg.Register, state))
				emitter.Assign(orw.AluRegister(reg.Register), local);
			else
				EmitCopy(op, local, false);
			state.ShrinkStack(width.Size);
		}

		private void EmitPop(Identifier idDst, PrimitiveType width)
		{
			Identifier src = frame.EnsureStackLocal(-state.StackBytes, width);
			emitter.Assign(idDst, src);
			state.ShrinkStack(width.Size);
		}

		private void EmitPush(IntelRegister reg)
		{
			EmitPush(reg.DataType, orw.AluRegister(reg));
		}

		private void EmitPush(PrimitiveType width, MachineOperand op)
		{
			EmitPush(width, SrcOp(op));
		}

		private void EmitPush(PrimitiveType width, Expression expr)
		{
			Constant c = expr as Constant;
			if (c != null && c.DataType != width)
			{
				expr = new Constant(width, c.ToInt64());
			}
			
			// Allocate an local variable for the push.

			state.GrowStack(width.Size);
			emitter.Assign(
				frame.EnsureStackLocal(-state.StackBytes, width),
				expr);
		}


		///<summary>
		/// Converts a rep [string instruction] into a loop: 
		/// <code>
		/// while ([e]cx != 0)
		///		[string instruction]
		///		--ecx;
		///		if (zF)				; only cmps[b] and scas[b]
		///			goto follow;
		/// follow: ...	
		/// </code>
		///</summary>
		private void EmitRepInstruction(IntelInstruction [] instrs, ref int i, CodeEmitter emitter)
		{
			// Compare [E]CX to 0. If [E]CX isn't 0, fall through to the next block.

            Block blockHead = emitter.Block;
            Identifier regCX = orw.AluRegister(Registers.ecx, instrCur.addrWidth);
			Identifier tmp = frame.CreateTemporary(PrimitiveType.Byte);

			// Terminate the header block & create a new block for the repeated string instruction.

			Block blockFollow = EmitBranchPath(blockHead, this.addrEnd);
			Block blockStringInstr = proc.AddBlock(state.InstructionAddress.GenerateName("l", "_rep"));
			proc.AddEdge(blockHead, blockStringInstr);
            emitter.Branch(emitter.Eq0(regCX), blockStringInstr);

			// Decrement the [E]CX register.

            emitter = ProcedureRewriter.CreateEmitter(blockStringInstr);
			emitter.Assign(regCX, emitter.Sub(regCX, 1));

			// Advance to the next instruction and since it is a string instruction,
			// convert it.

			instrCur = instrs[++i];
			siw.EmitStringInstruction(instrCur, emitter);

			// If it was a string compare instruction, we should branch back to the top block.

			Opcode stringOp = instrs[i].code;
			switch (stringOp)
			{
				case Opcode.cmps: case Opcode.cmpsb:
				case Opcode.scas: case Opcode.scasb:
				{
					ConditionCode cc = (instrs[i-1].code == Opcode.repne)
						? ConditionCode.NE
						: ConditionCode.EQ;
                    emitter.Branch(new TestCondition(cc, orw.FlagGroup(FlagM.ZF)), blockHead);
					proc.AddEdge(blockStringInstr, blockFollow);
					break;
				}
				default:
					break;
			}
			proc.AddEdge(blockStringInstr, blockHead);
		}

		public void EmitReturnInstruction(int cbReturnAddress, int cbBytesPop)
		{
            if (frame.ReturnAddressSize != cbReturnAddress)
            {
                host.AddDiagnostic(new WarningDiagnostic(
                    state.InstructionAddress,
                    "Caller expects a return address of {0} bytes, but procedure {1} was called with a return address of {2} bytes.",
                    cbReturnAddress, this.proc, frame.ReturnAddressSize));
            }
			emitter.Return();
            if (proc.Signature.StackDelta != 0 && proc.Signature.StackDelta != cbBytesPop)
            {
                host.AddDiagnostic(new WarningDiagnostic(
                    state.InstructionAddress,
                    "Multiple values of stack delta in procedure {0} when processung RET instruction; was {1} previously.", proc.Name, proc.Signature.StackDelta));
            }
            else
            {
                proc.Signature.StackDelta = cbBytesPop;
            }
			proc.Signature.FpuStackDelta = state.FpuStackItems;
			proc.Signature.FpuStackArgumentMax = maxFpuStackRead;
			proc.Signature.FpuStackOutArgumentMax = maxFpuStackWrite;
			proc.AddEdge(emitter.Block, proc.ExitBlock);
		}

		public void EmitRotation(string operation, bool useCarry, bool left, FlagM defFlags, FlagM deadFlags)
		{
			Identifier t = null;
			if ((deadFlags & FlagM.CF) == 0)
			{
				Expression sh; 
				if (left)
				{
					sh = new BinaryExpression(
						Operator.sub,
						instrCur.op2.Width,
						new Constant(instrCur.op2.Width, instrCur.op1.Width.BitSize),
						SrcOp(instrCur.op2));
				}
				else
				{
					sh = SrcOp(instrCur.op2);
				}
				sh = new BinaryExpression(
					Operator.shl,
					instrCur.op1.Width,
					new Constant(instrCur.op1.Width, 1),
					sh);
				t = frame.CreateTemporary(PrimitiveType.Bool);
                emitter.Assign(t, emitter.Ne0(emitter.And(SrcOp(instrCur.op1), sh)));
			}
			Expression p;
			if (useCarry)
			{
				p = emitter.PseudoProc(operation, instrCur.op1.Width, SrcOp(instrCur.op1), SrcOp(instrCur.op2), orw.FlagGroup(FlagM.CF));
			}
			else
			{
				p = emitter.PseudoProc(operation, instrCur.op1.Width,  SrcOp(instrCur.op1), SrcOp(instrCur.op2));
			}
			EmitCopy(instrCur.op1, p, false);
			if (t != null)
				emitter.Assign(orw.FlagGroup(FlagM.CF), t);
		}

		/// Emits an appropriate set condition code.
		
		private void EmitSet(MachineOperand opDst, ConditionCode cc, FlagM useFlags)
		{
			EmitCopy(opDst, new TestCondition(cc, orw.FlagGroup(useFlags)), false);
		}

		private void EmitSwitch(MachineRegister register, ImageMapVectorTable table)
		{
			Block [] jumps = new Block[table.Addresses.Count];
			int i = 0;
			Block block = emitter.Block;
			foreach (Address addrTarget in table.Addresses)
			{
				jumps[i] = EmitBranchPath(block, addrTarget);
				++i;
			}
			emitter = ProcedureRewriter.CreateEmitter(block);
            emitter.Switch(orw.AluRegister(register), jumps);
		}

		private Expression EmitUnaryOperator(UnaryOperator op, MachineOperand opDst, MachineOperand opSrc)
		{
			Expression src = SrcOp(opSrc);
			if (src is MemoryAccess)
			{
				Identifier tmp = frame.CreateTemporary(opDst.Width);
				emitter.Assign(tmp, src);
				MemoryAccess acc = orw.CreateMemoryAccess((MemoryOperand) opDst, state);
				emitter.Store(acc, new UnaryExpression(op, opSrc.Width, tmp));
				return tmp;
			}
			else
			{
				EmitCopy(opDst, new UnaryExpression(op, opSrc.Width, src), false);
				return src;
			}
		}


		private Identifier FpuRegister(int reg)
		{
			return orw.FpuRegister(reg, state);
		}

		private Procedure GetProcedureFromInstructionAddr(MachineOperand op, int cbReturnAddress, bool fSameProc)
		{
			Address addr = orw.OperandAsCodeAddress(op, state);
			Procedure p = host.GetProcedureAtAddress(addr, cbReturnAddress);
			if (p != null)
			{
				if (p != proc || fSameProc) 
				{
					return p;
				}
			}
			return null;
		}

		private bool IsSameRegister(MachineOperand op1, MachineOperand op2)
		{
			RegisterOperand r1 = op1 as RegisterOperand;
			RegisterOperand r2 = op2 as RegisterOperand;
			return (r1 != null && r2 != null && r1.Register == r2.Register);
		}

		private bool IsStackRegister(MachineOperand op)
		{
			RegisterOperand reg = op as RegisterOperand;
			return (reg != null && IsStackRegister(reg.Register));
		}

		private bool IsStackRegister(MachineRegister reg)
		{
			return (reg == Registers.sp || reg == Registers.esp);
		}

		public Expression MaybeCast(DataType type, Expression e)
		{
			if (type != null)
				return new Cast(type, e);
			else
				return e;
		}

		public void RewriteCall(MachineOperand callTarget, PrimitiveType returnAddressSize)
		{
			Address addr = orw.OperandAsCodeAddress(callTarget, state);
			if (addr != null)
			{
				PseudoProcedure ppp = host.TrampolineAt(addr);
				if (ppp != null)
				{
					BuildApplication(ppp);
				}
				else
				{
					Procedure p = GetProcedureFromInstructionAddr(
						callTarget, 
						returnAddressSize.Size, 
						true);
					EmitCall(p);
				}
			} 
			else
			{
				EmitIndirectCall(state.InstructionAddress);
			}
		}

		public void RewriteEnter()
		{
			IntelRegister freg = (IntelRegister) Registers.ebp.GetPart(instrCur.dataWidth);
			EmitPush(freg);
			state.EnterFrame(freg);
			state.GrowStack(
				instrCur.dataWidth.Size * ((ImmediateOperand)instrCur.op2).Value.ToInt32() + 
				((ImmediateOperand)instrCur.op1).Value.ToInt32());
		}

        private void RewriteMov()
        {
            RegisterOperand regOp = instrCur.op1 as RegisterOperand;
            if (regOp != null &&  arch.ProcessorMode.IsPointerRegister(regOp.Register) && IsStackRegister(instrCur.op2))
            {
                state.EnterFrame(regOp.Register);
                return;
            }
            regOp = instrCur.op2 as RegisterOperand;
            if (regOp != null && IsStackRegister(instrCur.op1))
            {
                if (state.FrameRegister != MachineRegister.None)
                {
                    state.LeaveFrame();
                    return;
                }
            }

            EmitCopy(instrCur.op1, SrcOp(instrCur.op2, instrCur.op1.Width), false);
        }

		/// <summary>
		/// Searches backwards to find a ProcedureConstant that is assigned to the identifier id.
		/// </summary>
		/// <remarks>
		/// This is a sleazy hack since we pay absolutely no attention to register liveness &c. However,
		/// the code is written in the spirit of "innocent until proven guilty". If this turns out to be buggy,
		/// and false positives occur, it will have to be canned and a better solution will have to be invented.
		/// </remarks>
		/// <param name="id"></param>
		/// <returns></returns>
		private PseudoProcedure SearchBackForProcedureConstant(Identifier id)
		{
			Block block = emitter.Block;
			while (block != null)
			{
				for (int i = block.Statements.Count - 1; i >= 0; --i)
				{
					Assignment ass = block.Statements[i].Instruction as Assignment;
					if (ass != null)
					{
						Identifier idAss = ass.Dst as Identifier;
						if (idAss != null && idAss == id)
						{
							ProcedureConstant pc = ass.Src as ProcedureConstant;
							if (pc != null)
							{
								return (PseudoProcedure) pc.Procedure;
							}
							else
								return null;
						}
					}
				}
				if (block.Pred.Count == 0)
					return null;
				block = block.Pred[0];
			}
			return null;
		}

		/// <summary>
		/// Looks backward in this basic block to find the constant that
		/// was assigned to the identifier.
		/// </summary>
		/// <param name="id">Identifier whose possible constant assignment is looked for.</param>
		/// <returns>The constant that was assigned to the identifier, or null if no constant assignment was found.</returns>
		private Constant SearchBackForConstantAssignment(Identifier id)
		{
			for (int i = emitter.Block.Statements.Count - 1; i >= 0; --i)
			{
				Assignment ass = emitter.Block.Statements[i].Instruction as Assignment;
				if (ass != null)
				{
					Identifier idAss = ass.Dst as Identifier;
					if (idAss == id)
						return ass.Src as Constant;
				}
			}
			return null;
		}

		private Expression SrcOp(MachineOperand opSrc)
		{
			return orw.Transform(opSrc, opSrc.Width, instrCur.addrWidth, state);
		}

		private Expression SrcOp(MachineOperand opSrc, PrimitiveType dstWidth)
		{
			return orw.Transform(opSrc, dstWidth, instrCur.addrWidth, state);
		}

		private void WriteFpuStack(int offset)
		{
			int o = offset - state.FpuStackItems;
			if (o > maxFpuStackWrite)
				maxFpuStackWrite = o;
		}
	}
}
