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
				Write(")");
			}
			precedenceCur = precedenceOld;
		}

		private int SetPrecedence(int precedence)
		{
			if (precedenceCur < precedence)
			{
				Write("(");
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
			Write("[");
			WriteExpression(acc.Index);
			Write("]");
		}

		public void VisitBinaryExpression(BinaryExpression binExp)
		{
			int prec = SetPrecedence((int) precedences[binExp.op]);
			binExp.Left.Accept(this);
			Write(BinaryExpression.OperatorToString(binExp.op));
			binExp.Right.Accept(this);
			ResetPresedence(prec);
		}

		public void VisitCast(Cast cast)
		{
			int prec = SetPrecedence(PrecedenceCase);
			Write("(");
			Write(cast.DataType.ToString());        //$TODO: use a TypeFormatter
			Write(") ");
			cast.Expression.Accept(this);
			ResetPresedence(prec);
		}

		public void VisitConditionOf(ConditionOf cond)
		{
			Write("cond(");
			WriteExpression(cond.Expression);
			Write(")");
		}

		public void VisitConstant(Constant c)
		{
			Write(c.ToString());
		}

		public void VisitDepositBits(DepositBits d)
		{
			Write("DPB(");
			WriteExpression(d.Source);
			Write(", ");
			WriteExpression(d.InsertedBits);
			Write(", {0}, {1})", d.BitPosition, d.BitCount);
		}

		public void VisitMkSequence(MkSequence seq)
		{
			Write("SEQ(");
			WriteExpression(seq.Head);
			Write(", ");
			WriteExpression(seq.Tail);
			Write(")");
		}

		public void VisitDereference(Dereference deref)
		{
			int prec = SetPrecedence(PrecedenceDereference);
			Write("*");
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
				Write("->{0}", acc.FieldName);
			}
			else
			{
				acc.structure.Accept(this);
				Write(".{0}", acc.FieldName);
			}
			ResetPresedence(prec);
		}

		public void VisitMemberPointerSelector(MemberPointerSelector mps)
		{
			int prec = SetPrecedence(PrecedenceMemberPointerSelector);
			Dereference d = mps.BasePointer as Dereference;
			if (d != null)
			{
				d.Expression.Accept(this);
				Write("->*");
			}
			else
			{
				mps.BasePointer.Accept(this);
				Write(".*");
			}
			mps.MemberPointer.Accept(this);
			ResetPresedence(prec);
		}

		public void VisitIdentifier(Identifier id)
		{
			Write(id.Name);
		}

		public void VisitMemoryAccess(MemoryAccess access)
		{
			access.MemoryId.Accept(this);
			Write("[");
			WriteExpression(access.EffectiveAddress);
			Write(":");
			Write((access.DataType != null) ? access.DataType.ToString() : "??");
			Write("]");
		}

		public void VisitSegmentedAccess(SegmentedAccess access)
		{
			access.MemoryId.Accept(this);
			Write("[");
			WriteExpression(access.BasePointer);
			Write(":");
			WriteExpression(access.EffectiveAddress);
			Write(":");
			Write((access.DataType != null) ? access.DataType.ToString() : "??");
			Write("]");
		}

		public void VisitPhiFunction(PhiFunction phi)
		{
			Write("PHI");
			WriteActuals(phi.Arguments);
		}

		public void VisitPointerAddition(PointerAddition pa)
		{
			throw new NotImplementedException("NYI");
		}

		public virtual void VisitProcedureConstant(ProcedureConstant pc)
		{
			Write(pc.Procedure.Name);
		}

		public void VisitTestCondition(TestCondition tc)
		{
			Write("Test({0},", tc.Cc);
			WriteExpression(tc.Expression);
			Write(")");
		}

		public void VisitScopeResolution(ScopeResolution scope)
		{
			Write(scope.TypeName);
		}

		public void VisitSlice(Slice slice)
		{
			Write("SLICE(");
			WriteExpression(slice.Expression);
			Write(", {0}, {1})", slice.DataType, slice.Offset);
		}

		public void VisitUnaryExpression(UnaryExpression unary)
		{
			int prec = SetPrecedence((int) precedences[unary.op]);
			Write(UnaryExpression.OperatorToString(unary.op));
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
				Write(" = "); 
			}
			a.Src.Accept(this);
			if (a.IsAlias)
				Write(" (alias)");
			Terminate();
		}

		public void VisitBranch(Branch b)
		{
			Indent();
			WriteKeyword("branch");
            Write(" ");
			b.Condition.Accept(this);
			Terminate();
		}

		public void VisitCallInstruction(CallInstruction ci)
		{
			Indent();
			WriteKeyword("call");
            Write(string.Format(" {0} ({1})", ci.Callee.Name, ci.CallSite));
			Terminate();
		}

		public void VisitDeclaration(Declaration decl)
		{
			Indent();
			if (decl.Id.DataType != null)
			{
				Write(decl.Id.DataType.ToString());
			}
			else
			{
				decl.Id.DataType.Write(writer);
			}
			Write(" ");
			decl.Id.Accept(this);
			if (decl.Expression != null)
			{
				Write(" = ");
				decl.Expression.Accept(this);
			}
			Terminate();
		}

		public void VisitDefInstruction(DefInstruction def)
		{
			Indent();
            WriteKeyword("def");
            Write(" ");
			def.Expression.Accept(this);
			Terminate();
		}


		public void VisitIndirectCall(IndirectCall ic)
		{
			Indent();
			WriteKeyword("icall");
            Write(" ");
			WriteExpression(ic.Callee);
			Terminate();
		}

		public void VisitPhiAssignment(PhiAssignment phi)
		{
			Indent();
			WriteExpression(phi.Dst);
			Write(" = ");
			WriteExpression(phi.Src);
			Terminate();
		}

		public void VisitReturnInstruction(ReturnInstruction ret)
		{
			Indent();
			WriteKeyword("return");
			if (ret.Value != null)
			{
				Write(" ");
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
			WriteKeyword("store");
            Write("(");
			WriteExpression(store.Dst);
			Write(") = ");
			WriteExpression(store.Src);
			Terminate();
		}

		public void VisitSwitchInstruction(SwitchInstruction si)
		{
			Indent();
			WriteKeyword("switch");
            Write(" (");
			si.expr.Accept(this);
			Write(") { ");
			foreach (Block b in si.targets)
			{
				Write("{0} ", b.RpoNumber);
			}
			Write("}");
			Terminate();
		}

		public void VisitUseInstruction(UseInstruction u)
		{
			Indent();
			WriteKeyword("use");
            Write(" ");
			WriteExpression(u.Expression);
			if (u.OutArgument != null)
			{
				Write(" (=> {0})", u.OutArgument);
			}
			Terminate();
		}
		#endregion


		#region IAbsynStatementVisitor //////////////////////

		public void VisitAssignment(AbsynAssignment a)
		{
			Indent();
			a.Dst.Accept(this);
			Write(" = ");
			a.Src.Accept(this);
			Terminate(";");
		}

		public void VisitBreak(AbsynBreak brk)
		{
			Indent();
			WriteKeyword("break");
			Terminate(";");
		}

		public void VisitCompoundStatement(AbsynCompoundStatement compound)
		{
			Indentation -= TabSize;
			Indent();
			Write("{");
			Terminate();
			Indentation += TabSize;

			WriteStatementList(compound.Statements);

			Indentation -= TabSize;
			Indent();
			Write("}");
			Terminate();
			Indentation += TabSize;
		}

		public void VisitContinue(AbsynContinue cont)
		{
			WriteKeyword("continue");
            WriteLine();
		}

		public void VisitDeclaration(AbsynDeclaration decl)
		{
			Indent();
			if (decl.Identifier.DataType != null)
			{
				Write(decl.Identifier.DataType.ToString());
			}
			else
			{
				decl.Identifier.DataType.Write(writer);
			}
			Write(" ");
			decl.Identifier.Accept(this);
			if (decl.Expression != null)
			{
				Write(" = ");
				decl.Expression.Accept(this);
			}
			Terminate(";");
		}

		public void VisitDoWhile(AbsynDoWhile loop)
		{
			Indent();
			WriteKeyword("do");
			Terminate();
			WriteIndentedStatement(loop.Body);
			
			Indent();
			WriteKeyword("while");
            Write(" (");
			WriteExpression(loop.Condition);
			Terminate(");");
		}

		public void VisitGoto(AbsynGoto g)
		{
			Indent();
			WriteKeyword("goto");
            Write(" ");
			Write(g.Label);
			Terminate(";");
		}

		public void VisitIf(AbsynIf ifs)
		{
			Indent();
			WriteIf(ifs);
		}

		private void WriteIf(AbsynIf ifs)
		{
			WriteKeyword("if");
            Write(" (");
			WriteExpression(ifs.Condition);
			Write(")");
			Terminate();

			WriteIndentedStatement(ifs.Then);

			if (ifs.Else != null)
			{
				Indent();
				WriteKeyword("else");
				AbsynIf elseIf = ifs.Else as AbsynIf;
				if (elseIf != null)
				{
					Write(" ");
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
			Write(lbl.Name);
			Terminate(":");
		}

		public void VisitReturn(AbsynReturn ret)
		{
			Indent();
			WriteKeyword("return");
			if (ret.Value != null)
			{
				Write(" ");
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
			WriteKeyword("while");
            Write(" (");
			WriteExpression(loop.Condition);
			Terminate(")");

			WriteIndentedStatement(loop.Body);
		}

		#endregion

		public void Write(Procedure proc)
		{
			proc.Signature.Emit(proc.Name, ProcedureSignature.EmitFlags.None, this);
			WriteLine();
			Write("{");
            WriteLine();
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
                    if (!string.IsNullOrEmpty(b.Name))
                    {
                        Write(b.Name);
                        Write(":");
                        WriteLine();
                    }
					if (b.Statements.Count > 0)
					{
                        int c = b.Statements.Count;
                        Branch br = b.Statements.Last.Instruction as Branch;
                        if (br != null)
                        {
                            --c;
                        }
                        for (int s = 0; s < c; ++s)
						{
							b.Statements[s].Instruction.Accept(this);
						}
                        if (br != null)
                        {
                            Indent();
                            WriteKeyword("if");
                            Write(" (");
                            WriteExpression(br.Condition);
                            Terminate(")");
                            int old = Indentation;
                            Indentation += TabSize;
                            Indent();
                            WriteKeyword("goto");
                            Write(" ");
                            Write(b.Succ[1].Name);
                            Terminate();
                            Indentation = old;
                        }
                        else if (b.Succ.Count == 1 && !(b.Statements.Last.Instruction is ReturnInstruction))
                        {
                            Indent();
                            WriteKeyword("goto");
                            Write(" ");
                            Write(b.Succ[0].Name);
                            Terminate();
                        }
					}
				}
			}
			Write("}");
            WriteLine();
		}

    	private void WriteActuals(Expression [] arguments)
		{
			Write("(");
			if (arguments.Length >= 1)
			{
				WriteExpression(arguments[0]);
				for (int i = 1; i < arguments.Length; ++i)
				{
					Write(", ");
					WriteExpression(arguments[i]);
				}
			}
			Write(")");
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
