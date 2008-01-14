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

using Decompiler.Core;
using Decompiler.Core.Absyn;
using Decompiler.Core.Code;
using System;

namespace Decompiler.Structure
{
	public delegate AbsynStatement StructureExitCreator(Block targetBlock);

	/// <summary>
	/// Adapts a basic block to become an AbsynStatement.
	/// </summary>
	public class BlockLinearizer : InstructionVisitor, IAbsynVisitor
	{
		private AbsynStatement abs;
		private Block breakTarget;

		public BlockLinearizer(Block breakTarget)
		{
			this.breakTarget = breakTarget;
		}

		private AbsynLabel BlockLabel(Block block)
		{
			if (block.Statements.Count != 0)
			{
				return block.Statements[0].Instruction as AbsynLabel;
			}
			return null;
		}

		public AbsynStatement ConvertBlock(Block b)
		{
			AbsynStatementList stms = new AbsynStatementList();
			ConvertBlock(b, stms);
			return stms.MakeAbsynStatement();
		}

		public void ConvertBlock(Block b,  AbsynStatementList stms)
		{
			ConvertBlock(b, true, stms);
		}

		public void ConvertBlock(Block b, bool fConvertInEdges, AbsynStatementList stms)
		{
			if (fConvertInEdges)
				ConvertInboundEdgesToGotos(b);
			foreach (Statement stm in b.Statements)
			{
				stms.Add(ConvertInstruction(stm.Instruction));
			}
		}

		public void ConvertInboundEdges(Block b, StructureExitCreator creator)
		{
			BlockList inboundEdges = new BlockList(b.Pred);
			foreach (Block p in inboundEdges)
			{
				AbsynStatement tgt = creator(b);
				Branch branch = GetBranch(p);
				if (branch != null)
				{
					Block pThen = p.ThenBlock;
					AbsynIf ifs;
					if (pThen == b)
					{
						ifs = new AbsynIf(branch.Condition, tgt);
					}
					else
					{
						ifs = new AbsynIf(branch.Condition.Invert(), tgt);
					}
					p.Statements.Last.Instruction = ifs;
				}
				else
				{
					p.Statements.Add(tgt);
				}
				Block.RemoveEdge(p, b);
			}
		}

		public void ConvertInboundEdgesToGotos(Block b)
		{
			if (b.Pred.Count != 0)
			{
				ConvertInboundEdges(b, new StructureExitCreator(MakeGoto));
			}
		}


		public AbsynStatement ConvertInstruction(Instruction instr)
		{
			abs = null;
			instr.Accept(this);
			return abs;
		}

		public AbsynLabel EnsureLabel(Block b)
		{
			AbsynLabel lbl = BlockLabel(b);
			if (lbl == null)
			{
				lbl = new AbsynLabel(b.Name);
				b.Statements.Insert(0, lbl);
			}
			return lbl;
		}

		protected Branch GetBranch(Block block)
		{
			if (block.Statements.Count == 0)
				return null;
			return block.Statements.Last.Instruction as Branch;
		}

		private void InvalidInstruction(string instrType)
		{
			throw new InvalidOperationException(instrType + " must have been converted to function applications");
		}

		public AbsynStatement MakeGoto(Block block)
		{
			if (block == breakTarget)
				return new AbsynBreak();
			else 
				return new AbsynGoto(EnsureLabel(block).Name);
		}


		#region InstructionVisitor Members
		public void VisitAssignment(Assignment a)
		{
			abs = new AbsynAssignment(a.Dst, a.Src);
		}

		public void VisitBranch(Branch b)
		{
			throw new NotImplementedException("Branch instructions not handled yet");
		}

		public void VisitCallInstruction(CallInstruction ci)
		{
			InvalidInstruction("Call instructions");
		}

		public void VisitDeclaration(Declaration decl)
		{
			abs = new AbsynDeclaration(decl.Id, decl.Expression);
		}

		public void VisitDefInstruction(DefInstruction def)
		{
			InvalidInstruction("Def instructions");
		}

		public void VisitIndirectCall(IndirectCall ic)
		{
			abs = new AbsynSideEffect(new Application(ic.Callee, null));	//$REVIEW: need to translate indirect calls to applications.
		}

		public void VisitPhiAssignment(PhiAssignment phi)
		{
			InvalidInstruction("Phi assignments");
		}

		public void VisitReturnInstruction(ReturnInstruction ret)
		{
			abs = new AbsynReturn(ret.Value);
		}


		public void VisitSideEffect(SideEffect side)
		{
			abs = new AbsynSideEffect(side.Expression);
		}

		public void VisitStore(Store store)
		{
			abs = new AbsynAssignment(store.Dst, store.Src);
		}

		public void VisitSwitchInstruction(SwitchInstruction s)
		{
			InvalidInstruction("Switch instructions");
		}


		public void VisitUseInstruction(UseInstruction use)
		{
			InvalidInstruction("Use instructions");
		}
		#endregion

		#region IAbsynVisitor Members

		public void VisitAssignment(AbsynAssignment expr)
		{
			abs = expr;
		}

		public void VisitBreak(AbsynBreak brk)
		{
			abs = brk;
		}

		public void VisitCompoundStatement(AbsynCompoundStatement compound)
		{
			abs = compound;
		}

		public void VisitContinue(AbsynContinue cont)
		{
			abs = cont;
		}

		public void VisitDeclaration(AbsynDeclaration decl)
		{
			abs = decl;
		}

		public void VisitLabel(AbsynLabel lbl)
		{
			abs = lbl;
		}

		public void VisitIf(AbsynIf ifStm)
		{
			abs = ifStm;
		}

		public void VisitDoWhile(AbsynDoWhile loop)
		{
			abs = loop;
		}

		public void VisitGoto(AbsynGoto loop)
		{
			abs = loop;
		}

		public void VisitReturn(AbsynReturn ret)
		{
			abs = ret;
		}

		public void VisitSideEffect(AbsynSideEffect side)
		{
			abs = side;
		}

		public void VisitWhile(AbsynWhile loop)
		{
			abs = loop;
		}

		#endregion
	}
}
