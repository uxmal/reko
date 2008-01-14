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

using Decompiler.Core.Absyn;
using Decompiler.Core.Code;
using Decompiler.Core.Operators;
using System;
using System.Collections;
using System.IO;


namespace Decompiler.Core.Output
{
	/// <summary>
	/// Formats intermediate-level instructions or abstract syntax statements.
	/// </summary>
	public class CodeFormatter : Formatter, InstructionVisitor, IAbsynVisitor, IExpressionVisitor
	{
		private int precedenceCur = PrecedenceLeast;

		private static Hashtable precedences;

		private const int PrecedenceApplication = 1;
		private const int PrecedenceArrayAccess = 1;
		private const int PrecedenceFieldAccess = 1;
		private const int PrecedenceDereference = 2;
		private const int PrecedenceMemberPointerSelector = 3;
		private const int PrecedenceCase = 2;
		private const int PrecedenceLeast = 20;

		public CodeFormatter(TextWriter writer) : base(writer)
		{
		}

		static CodeFormatter()
		{
			precedences = new Hashtable();
			precedences[Operator.not] = 2;			//$REFACTOR: precedence is a property of the output language; these are the C/C++ precedences
			precedences[Operator.neg] = 2;			
			precedences[Operator.comp] = 2;
			precedences[Operator.addrOf] = 2;
			precedences[Operator.muls] = 4;
			precedences[Operator.mulu] = 4;
			precedences[Operator.mul] = 4;
			precedences[Operator.divs] = 4;
			precedences[Operator.divu] = 4;
			precedences[Operator.mod] = 4;
			precedences[Operator.add] = 5;
			precedences[Operator.sub] = 5;
			precedences[Operator.sar] = 6;
			precedences[Operator.shr] = 6;
			precedences[Operator.shl] = 6;
			precedences[Operator.lt] = 7;
			precedences[Operator.le] = 7;
			precedences[Operator.gt] = 7;
			precedences[Operator.ge] = 7;
			precedences[Operator.rlt] = 7;
			precedences[Operator.rle] = 7;
			precedences[Operator.rgt] = 7;
			precedences[Operator.rge] = 7;
			precedences[Operator.ult] = 7;
			precedences[Operator.ule] = 7;
			precedences[Operator.ugt] = 7;
			precedences[Operator.uge] = 7;
			precedences[Operator.eq] = 8;
			precedences[Operator.ne] = 8;
			precedences[Operator.and] = 9;
			precedences[Operator.xor] = 10;
			precedences[Operator.or] = 11;
			precedences[Operator.cand] = 12;
			precedences[Operator.cor] = 13;
		}

		private void ResetPresedence(int precedenceOld)
		{
			if (precedenceOld < precedenceCur)
			{
				writer.Write(")");
			}
			precedenceCur = precedenceOld;
		}

		private int SetPrecedence(int precedence)
		{
			if (precedenceCur < precedence)
			{
				writer.Write("(");
			}
			int precedenceOld = precedenceCur;
			precedenceCur = precedence;
			return precedenceOld;
		}


		#region IExpressionVisitor members ///////////////////////

		public void VisitApplication(Application appl)
		{
			int prec = SetPrecedence(PrecedenceApplication);
			appl.Procedure.Accept(this);
			ResetPresedence(prec);
			WriteActuals(appl.Arguments);
		}

		public void VisitArrayAccess(ArrayAccess acc)
		{
			int prec = SetPrecedence(PrecedenceArrayAccess);
			acc.Array.Accept(this);
			ResetPresedence(prec);
			writer.Write("[");
			WriteExpression(acc.Index);
			writer.Write("]");
		}

		public void VisitBinaryExpression(BinaryExpression binExp)
		{
			int prec = SetPrecedence((int) precedences[binExp.op]);
			binExp.Left.Accept(this);
			writer.Write(BinaryExpression.OperatorToString(binExp.op));
			binExp.Right.Accept(this);
			ResetPresedence(prec);
		}

		public void VisitCast(Cast cast)
		{
			int prec = SetPrecedence(PrecedenceCase);
			writer.Write("(");
			writer.Write(cast.DataType.ToString());
			writer.Write(") ");
			cast.Expression.Accept(this);
			ResetPresedence(prec);
		}

		public void VisitConditionOf(ConditionOf cond)
		{
			writer.Write("cond(");
			WriteExpression(cond.Expression);
			writer.Write(")");
		}

		public void VisitConstant(Constant c)
		{
			writer.Write(c.ToString());
		}

		public void VisitDepositBits(DepositBits d)
		{
			writer.Write("DPB(");
			WriteExpression(d.Source);
			writer.Write(", ");
			WriteExpression(d.InsertedBits);
			writer.Write(", {0}, {1})", d.BitPosition, d.BitCount);
		}

		public void VisitMkSequence(MkSequence seq)
		{
			writer.Write("SEQ(");
			WriteExpression(seq.Head);
			writer.Write(", ");
			WriteExpression(seq.Tail);
			writer.Write(")");
		}

		public void VisitDereference(Dereference deref)
		{
			int prec = SetPrecedence(PrecedenceDereference);
			writer.Write("*");
			deref.Expression.Accept(this);
			ResetPresedence(prec);
		}

		public void VisitFieldAccess(FieldAccess acc)
		{
			int prec = SetPrecedence(PrecedenceFieldAccess);
			Dereference d = acc.structure as Dereference;
			if (d != null)
			{
				d.Expression.Accept(this);
				writer.Write("->{0}", acc.FieldName);
			}
			else
			{
				acc.structure.Accept(this);
				writer.Write(".{0}", acc.FieldName);
			}
			ResetPresedence(prec);
		}

		public void VisitMemberPointerSelector(MemberPointerSelector mps)
		{
			int prec = SetPrecedence(PrecedenceMemberPointerSelector);
			mps.Ptr.Accept(this);
			writer.Write("->*");
			mps.MemberPtr.Accept(this);
			ResetPresedence(prec);
		}

		public void VisitIdentifier(Identifier id)
		{
			writer.Write(id.Name);
		}

		public void VisitMemoryAccess(MemoryAccess access)
		{
			access.MemoryId.Accept(this);
			writer.Write("[");
			WriteExpression(access.EffectiveAddress);
			writer.Write(":");
			writer.Write((access.DataType != null) ? access.DataType.ToString() : "??");
			writer.Write("]");
		}

		public void VisitSegmentedAccess(SegmentedAccess access)
		{
			access.MemoryId.Accept(this);
			writer.Write("[");
			WriteExpression(access.BasePointer);
			writer.Write(":");
			WriteExpression(access.EffectiveAddress);
			writer.Write(":");
			writer.Write((access.DataType != null) ? access.DataType.ToString() : "??");
			writer.Write("]");
		}

		public void VisitPhiFunction(PhiFunction phi)
		{
			writer.Write("PHI");
			WriteActuals(phi.Arguments);
		}

		public void VisitPointerAddition(PointerAddition pa)
		{
			throw new NotImplementedException("NYI");
		}

		public void VisitProcedureConstant(ProcedureConstant pc)
		{
			writer.Write(pc.Procedure.Name);
		}

		public void VisitTestCondition(TestCondition tc)
		{
			writer.Write("Test({0},", tc.Cc);
			WriteExpression(tc.Expression);
			writer.Write(")");
		}

		public void VisitSlice(Slice slice)
		{
			writer.Write("SLICE(");
			WriteExpression(slice.Expression);
			writer.Write(", {0}, {1})", slice.DataType, slice.Offset);
		}

		public void VisitUnaryExpression(UnaryExpression unary)
		{
			int prec = SetPrecedence((int) precedences[unary.op]);
			writer.Write(UnaryExpression.OperatorToString(unary.op));
			unary.Expression.Accept(this);
			ResetPresedence(prec);
		}

		#endregion


		#region InstructionVisitor ///////////////////////////////
		
		public void VisitAssignment(Assignment a)
		{
			Indent();
			if (a.Dst != null)
			{
				a.Dst.Accept(this);
				writer.Write(" = "); 
			}
			a.Src.Accept(this);
			if (a.IsAlias)
				writer.Write(" (alias)");
			Terminate();
		}

		public void VisitBranch(Branch b)
		{
			Indent();
			writer.Write("branch ");
			b.Condition.Accept(this);
			Terminate();
		}

		public void VisitCallInstruction(CallInstruction ci)
		{
			Indent();
			writer.Write("call {0} ({1})", ci.Callee.Name, ci.CallSite);
			Terminate();
		}

		public void VisitDeclaration(Declaration decl)
		{
			Indent();
			if (decl.Id.DataType != null)
			{
				writer.Write(decl.Id.DataType.ToString());
			}
			else
			{
				decl.Id.DataType.Write(writer);
			}
			writer.Write(" ");
			decl.Id.Accept(this);
			if (decl.Expression != null)
			{
				writer.Write(" = ");
				decl.Expression.Accept(this);
			}
			Terminate();
		}

		public void VisitDefInstruction(DefInstruction def)
		{
			Indent();
			writer.Write("def ");
			def.Expression.Accept(this);
			Terminate();
		}


		public void VisitIndirectCall(IndirectCall ic)
		{
			Indent();
			writer.Write("icall ");
			WriteExpression(ic.Callee);
			Terminate();
		}

		public void VisitPhiAssignment(PhiAssignment phi)
		{
			Indent();
			WriteExpression(phi.Dst);
			writer.Write(" = ");
			WriteExpression(phi.Src);
			Terminate();
		}

		public void VisitReturnInstruction(ReturnInstruction ret)
		{
			Indent();
			writer.Write("return");
			if (ret.Value != null)
			{
				writer.Write(" ");
				WriteExpression(ret.Value);
			}
			Terminate();
		}

		public void VisitSideEffect(SideEffect side)
		{
			Indent();
			side.Expression.Accept(this);
			Terminate();
		}

		public void VisitStore(Store store)
		{
			Indent();
			writer.Write("store(");
			WriteExpression(store.Dst);
			writer.Write(") = ");
			WriteExpression(store.Src);
			Terminate();
		}

		public void VisitSwitchInstruction(SwitchInstruction si)
		{
			Indent();
			writer.Write("switch (");
			si.expr.Accept(this);
			writer.Write(") { ");
			foreach (Block b in si.targets)
			{
				writer.Write("{0} ", b.RpoNumber);
			}
			writer.Write("}");
			Terminate();
		}

		public void VisitUseInstruction(UseInstruction u)
		{
			Indent();
			writer.Write("use ");
			WriteExpression(u.Expression);
			if (u.OutArgument != null)
			{
				writer.Write(" (=> {0})", u.OutArgument);
			}
			Terminate();
		}
		#endregion



		#region IAbsynStatementVisitor //////////////////////

		public void VisitAssignment(AbsynAssignment a)
		{
			Indent();
			a.Dst.Accept(this);
			writer.Write(" = ");
			a.Src.Accept(this);
			Terminate(";");
		}

		public void VisitBreak(AbsynBreak brk)
		{
			Indent();
			writer.Write("break");
			Terminate(";");
		}

		public void VisitCompoundStatement(AbsynCompoundStatement compound)
		{
			Indentation -= TabSize;
			Indent();
			writer.Write("{");
			Terminate();
			Indentation += TabSize;

			WriteStatementList(compound.Statements);

			Indentation -= TabSize;
			Indent();
			writer.Write("}");
			Terminate();
			Indentation += TabSize;
		}

		public void VisitContinue(AbsynContinue cont)
		{
			writer.WriteLine("continue;");
		}

		public void VisitDeclaration(AbsynDeclaration decl)
		{
			Indent();
			if (decl.Identifier.DataType != null)
			{
				writer.Write(decl.Identifier.DataType.ToString());
			}
			else
			{
				decl.Identifier.DataType.Write(writer);
			}
			writer.Write(" ");
			decl.Identifier.Accept(this);
			if (decl.Expression != null)
			{
				writer.Write(" = ");
				decl.Expression.Accept(this);
			}
			Terminate(";");
		}

		public void VisitDoWhile(AbsynDoWhile loop)
		{
			Indent();
			writer.Write("do");
			Terminate();
			WriteIndentedStatement(loop.Body);
			
			Indent();
			writer.Write("while (");
			WriteExpression(loop.Condition);
			Terminate(");");
		}

		public void VisitGoto(AbsynGoto g)
		{
			Indent();
			writer.Write("goto ");
			writer.Write(g.Label);
			Terminate(";");
		}

		public void VisitIf(AbsynIf ifs)
		{
			Indent();
			WriteIf(ifs);
		}

		private void WriteIf(AbsynIf ifs)
		{
			writer.Write("if (");
			WriteExpression(ifs.Condition);
			writer.Write(")");
			Terminate();

			WriteIndentedStatement(ifs.Then);

			if (ifs.Else != null)
			{
				Indent();
				writer.Write("else");
				AbsynIf elseIf = ifs.Else as AbsynIf;
				if (elseIf != null)
				{
					writer.Write(" ");
					WriteIf(elseIf);
				}
				else
				{
					Terminate();
					WriteIndentedStatement(ifs.Else);
				}
			}
		}

		public void VisitLabel(AbsynLabel lbl)
		{
			writer.Write(lbl.Name);
			Terminate(":");
		}

		public void VisitReturn(AbsynReturn ret)
		{
			Indent();
			writer.Write("return");
			if (ret.Value != null)
			{
				writer.Write(" ");
				WriteExpression(ret.Value);
			}
			Terminate(";");
		}

		public void VisitSideEffect(AbsynSideEffect side)
		{
			Indent();
			side.Expression.Accept(this);
			Terminate(";");
		}

		public void VisitWhile(AbsynWhile loop)
		{
			Indent();
			writer.Write("while (");
			WriteExpression(loop.Condition);
			Terminate(")");

			WriteIndentedStatement(loop.Body);
		}

		#endregion

		public void Write(Procedure proc)
		{
			proc.Signature.Emit(proc.Name, ProcedureSignature.EmitFlags.None, writer);
			writer.WriteLine();
			writer.WriteLine("{");

			if (proc.Body.Count > 0)
			{
				for (int i = 0; i < proc.Body.Count; ++i)
				{
					proc.Body[i].Accept(this);
				}
			}
			else
			{
				for (int i = 0; i < proc.RpoBlocks.Count; ++i)
				{
					Block b = proc.RpoBlocks[i];
					if (b.Statements.Count > 0)
					{
						foreach (Statement stm in b.Statements)
						{
							stm.Instruction.Accept(this);
						}
					}
				}
			}
			writer.WriteLine("}");
		}

		private void WriteActuals(Expression [] arguments)
		{
			writer.Write("(");
			if (arguments.Length >= 1)
			{
				WriteExpression(arguments[0]);
				for (int i = 1; i < arguments.Length; ++i)
				{
					writer.Write(", ");
					WriteExpression(arguments[i]);
				}
			}
			writer.Write(")");
		}

		/// <summary>
		/// Writes an expression in a context where it needs no parentheses.
		/// </summary>
		/// <param name="expr"></param>
		public void WriteExpression(Expression expr)
		{
			int prec = precedenceCur;
			precedenceCur = PrecedenceLeast;
			expr.Accept(this);
			precedenceCur = prec;
		}


		public void WriteIndentedStatement(AbsynStatement stm)
		{
			Indentation += TabSize;
			if (stm != null)
				stm.Accept(this);
			else
			{
				Indent();				
				Terminate(";");
			}
			Indentation -= TabSize;
		}

		public void WriteStatementList(AbsynStatementList list)
		{
			foreach (AbsynStatement s in list)
			{
				s.Accept(this);
			}
		}
	}
}
