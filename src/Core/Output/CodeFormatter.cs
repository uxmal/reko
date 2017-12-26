#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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

using Reko.Core.Absyn;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Types;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Reko.Core.Output
{
	/// <summary>
	/// Formats intermediate-level instructions or abstract syntax statements.
	/// </summary>
	public class CodeFormatter : InstructionVisitor, IAbsynVisitor, IExpressionVisitor
	{
        private enum Assoc { None, Left, Right };

		private int precedenceCur = PrecedenceLeast;
        private bool forceParensIfSamePrecedence = false;
        private Formatter writer;

        //$TODO: move this to a language-specific class.
		private static Dictionary<Operator,int> precedences;
        private static HashSet<Type> singleStatements;

        private const int PrecedenceApplication = 1;
		private const int PrecedenceArrayAccess = 1;
		private const int PrecedenceFieldAccess = 1;
		private const int PrecedenceDereference = 2;
		private const int PrecedenceMemberPointerSelector = 3;
		private const int PrecedenceCase = 2;
        private const int PrecedenceConditional = 14;
        private const int PrecedenceLeast = 20;

        private TypeGraphWriter typeWriter;

		public CodeFormatter(Formatter writer)
		{
            this.writer = writer;
		}

        public Formatter InnerFormatter
        {
            get { return writer; }
        }

		static CodeFormatter()
		{
            precedences = new Dictionary<Operator, int>();
			precedences[Operator.Not] = 2;			//$REFACTOR: precedence is a property of the output language; these are the C/C++ precedences
			precedences[Operator.Neg] = 2;			
			precedences[Operator.FNeg] = 2;
            precedences[Operator.Comp] = 2;
			precedences[Operator.AddrOf] = 2;
			precedences[Operator.SMul] = 4;
			precedences[Operator.UMul] = 4;
			precedences[Operator.IMul] = 4;
			precedences[Operator.SDiv] = 4;
			precedences[Operator.UDiv] = 4;
			precedences[Operator.IMod] = 4;
            precedences[Operator.FMul] = 4;
            precedences[Operator.FDiv] = 4;
            precedences[Operator.IAdd] = 5;
            precedences[Operator.ISub] = 5;
            precedences[Operator.USub] = 5;
            precedences[Operator.FAdd] = 5;
            precedences[Operator.FSub] = 5;
            precedences[Operator.Sar] = 6;
			precedences[Operator.Shr] = 6;
			precedences[Operator.Shl] = 6;
			precedences[Operator.Lt] = 7;
			precedences[Operator.Le] = 7;
			precedences[Operator.Gt] = 7;
			precedences[Operator.Ge] = 7;
			precedences[Operator.Feq] = 7;
			precedences[Operator.Fne] = 7;
			precedences[Operator.Flt] = 7;
            precedences[Operator.Fle] = 7;
			precedences[Operator.Fgt] = 7;
			precedences[Operator.Fge] = 7;
			precedences[Operator.Fne] = 7;
            precedences[Operator.Ult] = 7;
			precedences[Operator.Ule] = 7;
			precedences[Operator.Ugt] = 7;
			precedences[Operator.Uge] = 7;
			precedences[Operator.Eq] = 8;
			precedences[Operator.Ne] = 8;
			precedences[Operator.And] = 9;
			precedences[Operator.Xor] = 10;
			precedences[Operator.Or] = 11;
			precedences[Operator.Cand] = 12;
			precedences[Operator.Cor] = 13;

            singleStatements = new HashSet<Type>
            {
                typeof(AbsynReturn),
                typeof(AbsynGoto),
                typeof(AbsynBreak),
                typeof(AbsynContinue),
                typeof(AbsynAssignment),
                typeof(AbsynSideEffect)
            };
        }

		private void ResetPresedence(int precedenceOld)
		{
			if (precedenceOld < precedenceCur ||
                (forceParensIfSamePrecedence && precedenceCur == precedenceOld))
			{
				writer.Write(")");
			}
			precedenceCur = precedenceOld;
		}

		private int SetPrecedence(int precedence)
		{
			if (precedenceCur < precedence ||
                (forceParensIfSamePrecedence && precedenceCur == precedence))
			{
				writer.Write("(");
			}
			int precedenceOld = precedenceCur;
			precedenceCur = precedence;
			return precedenceOld;
		}

		#region IExpressionVisitor members ///////////////////////

        public void VisitAddress(Address addr)
        {
            if (addr.IsNull)
            {
                WriteNull();
            }
            else
            {
                var s = addr.ToString();
                if (!s.Contains(':'))
                    s = string.Format("0x{0}", s);
                writer.Write(s);
            }
        }

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
			int prec = SetPrecedence((int) precedences[binExp.Operator]);
			binExp.Left.Accept(this);
			writer.Write(binExp.Operator.ToString());
            var old = forceParensIfSamePrecedence;
            forceParensIfSamePrecedence = true;
            binExp.Right.Accept(this);
            forceParensIfSamePrecedence = old;
			ResetPresedence(prec);
		}

		public void VisitCast(Cast cast)
		{
			int prec = SetPrecedence(PrecedenceCase);
			writer.Write("(");
            new TypeReferenceFormatter(writer).WriteTypeReference(cast.DataType);
			writer.Write(") ");
			cast.Expression.Accept(this);
			ResetPresedence(prec);
		}

        public void VisitConditionalExpression(ConditionalExpression cond)
        {
            int prec = SetPrecedence(PrecedenceConditional);
            cond.Condition.Accept(this);
            writer.Write(" ? ");
            cond.ThenExp.Accept(this);
            writer.Write(" : ");
            cond.FalseExp.Accept(this);
            ResetPresedence(prec);
        }

        public void VisitConditionOf(ConditionOf cond)
		{
			writer.Write("cond(");
			WriteExpression(cond.Expression);
			writer.Write(")");
		}

        private static readonly char[] nosuffixRequired = new[] { '.', 'E', 'e' };

        public virtual void VisitConstant(Constant c)
        {
            if (!c.IsValid)
            {
                writer.Write("<invalid>");
                return;
            }
            var pt = c.DataType.ResolveAs<PrimitiveType>();
            if (pt != null)
            {
                if (pt.Domain == Domain.Boolean)
                {
                    writer.Write(Convert.ToBoolean(c.GetValue()) ? "true" : "false");
                }
                else if (pt.Domain == Domain.Real)
                {
                    string sr;

                    if (pt.Size == 4)
                    {
                        sr = c.ToFloat().ToString("g", CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        sr = c.ToReal64().ToString("g", CultureInfo.InvariantCulture);
                    }
                    if (sr.IndexOfAny(nosuffixRequired) < 0)
                    {
                        sr += ".0";
                    }
                    if (pt.Size == 4)
                    {
                        sr += "F";
                    }
                    writer.Write(sr);
                }
                else 
                {
                    object v = c.GetValue();
                    writer.Write(FormatString(pt, v), v);
                }
                return;
            }
            StringConstant s = c as StringConstant;
            if (s != null)
            {
                writer.Write('"');
                foreach (var ch in (string)s.GetValue())
                {
                    switch (ch)
                    {
                    case '\0': writer.Write("\\0"); break;
                    case '\a': writer.Write("\\a"); break;
                    case '\b': writer.Write("\\b"); break;
                    case '\f': writer.Write("\\f"); break;
                    case '\n': writer.Write("\\n"); break;
                    case '\r': writer.Write("\\r"); break;
                    case '\t': writer.Write("\\t"); break;
                    case '\v': writer.Write("\\v"); break;
                    case '\"': writer.Write("\\\""); break;
                    case '\\': writer.Write("\\\\"); break;
                    default:
                        //$REVIEW: these are ASCII codes. EBCDIC?
                        if (0 <= ch && ch < ' ' || ch >= 0x7F)
                            writer.Write("\\x{0:X2}", (int)ch);
                        else
                            writer.Write(ch);
                        break;
                    }
                }
                writer.Write('"');
            }
        }

		public void VisitDepositBits(DepositBits d)
		{
			writer.Write("DPB(");
			WriteExpression(d.Source);
			writer.Write(", ");
			WriteExpression(d.InsertedBits);
			writer.Write(", {0})", d.BitPosition);
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
			Dereference d = acc.Structure as Dereference;
            if (d != null)
            {
                d.Expression.Accept(this);
                writer.Write("->{0}", acc.Field.Name);
            }
            else
            {
                var scope = acc.Structure as ScopeResolution;
                if (scope != null)
                {
                    scope.Accept(this);
                    writer.Write("::{0}", acc.Field.Name);
                }
                else
                {
                    acc.Structure.Accept(this);
                    writer.Write(".{0}", acc.Field.Name);
                }
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
				writer.Write("->*");
			}
			else
			{
				mps.BasePointer.Accept(this);
				writer.Write(".*");
			}
            var old = forceParensIfSamePrecedence;
            forceParensIfSamePrecedence = true;
			mps.MemberPointer.Accept(this);
            forceParensIfSamePrecedence = old;
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
			writer.Write(access.DataType.ToString());
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
			Debug.Assert(access.DataType != null);
            writer.Write(access.DataType.ToString());
			writer.Write("]");
		}

        public void VisitOutArgument(OutArgument outArg)
        {
            writer.WriteKeyword("out");
            writer.Write(" ");
            WriteExpression(outArg.Expression);
        }

		public void VisitPhiFunction(PhiFunction phi)
		{
			writer.WriteKeyword("PHI");
			WriteActuals(phi.Arguments);
		}

		public void VisitPointerAddition(PointerAddition pa)
		{
            writer.Write("PTRADD(");
            WriteExpression(pa.Pointer);
            writer.Write(",{0})", pa.Offset);
		}

		public virtual void VisitProcedureConstant(ProcedureConstant pc)
		{
			writer.WriteHyperlink(pc.Procedure.Name, pc.Procedure);
		}

		public void VisitTestCondition(TestCondition tc)
		{
			writer.Write("Test({0},", tc.ConditionCode);
			WriteExpression(tc.Expression);
			writer.Write(")");
		}

        public void VisitScopeResolution(ScopeResolution scope)
        {
            writer.WriteType(scope.DataType.Name, scope.DataType);
        }

		public void VisitSlice(Slice slice)
		{
			writer.Write("SLICE(");
			WriteExpression(slice.Expression);
			writer.Write(", {0}, {1})", slice.DataType, slice.Offset);
		}

		public void VisitUnaryExpression(UnaryExpression unary)
		{
			int prec = SetPrecedence((int) precedences[unary.Operator]);
			writer.Write(unary.Operator.ToString());
			unary.Expression.Accept(this);
			ResetPresedence(prec);
		}

		#endregion


		#region InstructionVisitor ///////////////////////////////
		
		public void VisitAssignment(Assignment a)
		{
			writer.Indent();
			if (a.Dst != null)
			{
				a.Dst.Accept(this);
				writer.Write(" = "); 
			}
			a.Src.Accept(this);
			if (a.IsAlias)
				writer.Write(" (alias)");
			writer.Terminate();
		}

		public void VisitBranch(Branch b)
		{
			writer.Indent();
			writer.WriteKeyword("branch");
            writer.Write(" ");
			b.Condition.Accept(this);
            writer.Write(" ");
            writer.Write(b.Target.Name);
			writer.Terminate();
		}

		public void VisitCallInstruction(CallInstruction ci)
		{
			writer.Indent();
			writer.WriteKeyword("call");
            writer.Write(" ");
            ci.Callee.Accept(this);
            writer.Write(" ({0})", ci.CallSite);
			writer.Terminate();
            if (ci.Uses.Count > 0)
            {
                writer.Indentation += writer.TabSize;
                writer.Indent();
                writer.Write("uses: ");
                writer.Write(string.Join(",", ci.Uses.OrderBy(u => ((Identifier)(u.Expression)).Name).Select(u => u.Expression)));
                writer.Terminate();
                writer.Indentation -= writer.TabSize;
            }
            if (ci.Definitions.Count > 0)
            {
                writer.Indentation += writer.TabSize;
                writer.Indent();
                writer.Write("defs: ");
                writer.Write(string.Join(",", ci.Definitions.OrderBy(d => ((Identifier)d.Identifier).Name).Select(d => d.Identifier)));
                writer.Terminate();
                writer.Indentation -= writer.TabSize;
            }
		}

		public void VisitDeclaration(Declaration decl)
		{
			writer.Indent();
            Debug.Assert(decl.Identifier.DataType != null, "The DataType property can't ever be null");

#if OLD
            TypeFormatter tf = new TypeFormatter(writer, true);
            tf.Write(decl.Identifier.DataType, decl.Identifier.Name);
#else
            TypeReferenceFormatter tf = new TypeReferenceFormatter(writer);
            tf.WriteDeclaration(decl.Identifier.DataType, decl.Identifier.Name);
#endif
            if (decl.Expression != null)
			{
				writer.Write(" = ");
				decl.Expression.Accept(this);
			}
			writer.Terminate();
		}

		public void VisitDefInstruction(DefInstruction def)
		{
			writer.Indent();
            writer.WriteKeyword("def");
            writer.Write(" ");
			def.Identifier.Accept(this);
			writer.Terminate();
		}

        public void VisitGotoInstruction(GotoInstruction g)
        {
            writer.Indent();
            if (!(g.Condition is Constant))
            {
                writer.WriteKeyword("if");
                writer.Write(" (");
                g.Condition.Accept(this);
                writer.Write(") ");
            }
            writer.WriteKeyword("goto");
            writer.WriteKeyword(" ");
            g.Target.Accept(this);
            writer.Terminate();
        }

		public void VisitPhiAssignment(PhiAssignment phi)
		{
			writer.Indent();
			WriteExpression(phi.Dst);
			writer.Write(" = ");
			WriteExpression(phi.Src);
			writer.Terminate();
		}

		public void VisitReturnInstruction(ReturnInstruction ret)
		{
			writer.Indent();
			writer.WriteKeyword("return");
			if (ret.Expression != null)
			{
				writer.Write(" ");
				WriteExpression(ret.Expression);
			}
			writer.Terminate();
		}

		public void VisitSideEffect(SideEffect side)
		{
			writer.Indent();
			side.Expression.Accept(this);
			writer.Terminate();
		}

		public void VisitStore(Store store)
		{
			writer.Indent();
			WriteExpression(store.Dst);
			writer.Write(" = ");
			WriteExpression(store.Src);
			writer.Terminate();
		}

		public void VisitSwitchInstruction(SwitchInstruction si)
		{
			writer.Indent();
			writer.WriteKeyword("switch");
            writer.Write(" (");
			si.Expression.Accept(this);
			writer.Write(") { ");
			foreach (Block b in si.Targets)
			{
				writer.Write("{0} ", b.Name);
			}
			writer.Write("}");
			writer.Terminate();
		}

		public void VisitUseInstruction(UseInstruction u)
		{
			writer.Indent();
			writer.WriteKeyword("use");
            writer.Write(" ");
			WriteExpression(u.Expression);
			if (u.OutArgument != null)
			{
				writer.Write(" (=> {0})", u.OutArgument);
			}
			writer.Terminate();
		}
#endregion


		#region IAbsynStatementVisitor //////////////////////

		public void VisitAssignment(AbsynAssignment a)
		{
			writer.Indent();
			a.Dst.Accept(this);
			writer.Write(" = ");
			a.Src.Accept(this);
			writer.Terminate(";");
		}

		public void VisitBreak(AbsynBreak brk)
		{
			writer.Indent();
			writer.WriteKeyword("break");
			writer.Terminate(";");
		}

        public void VisitCase(AbsynCase c)
        {
            writer.Indentation -= writer.TabSize;
            writer.Indent();
            writer.WriteKeyword("case");
            writer.Write(" ");
            c.Constant.Accept(this);
            writer.Terminate(":");
            writer.Indentation += writer.TabSize;
        }

        public void VisitDefault(AbsynDefault d)
        {
            writer.Indentation -= writer.TabSize;
            writer.Indent();
            writer.WriteKeyword("default");
            writer.Terminate(":");
            writer.Indentation += writer.TabSize;
        }
		
        public void VisitContinue(AbsynContinue cont)
		{
            writer.Indent();
			writer.WriteKeyword("continue");
            writer.Terminate(";");
		}

		public void VisitDeclaration(AbsynDeclaration decl)
		{
			writer.Indent();
			if (decl.Identifier.DataType != null)
			{
                TypeReferenceFormatter tf = new TypeReferenceFormatter(writer);
                tf.WriteDeclaration(decl.Identifier.DataType, decl.Identifier.Name);
			}
			else
			{
                writer.Write("?unknown?");
                writer.Write(" ");
                decl.Identifier.Accept(this);
            }
			if (decl.Expression != null)
			{
				writer.Write(" = ");
				decl.Expression.Accept(this);
			}
			writer.Terminate(";");
		}

		public void VisitDoWhile(AbsynDoWhile loop)
		{
			writer.Indent();
			writer.WriteKeyword("do");
			writer.Terminate();
			WriteIndentedStatements(loop.Body, true);
			
			if (HasSmallBody(loop.Body))
                writer.Indent();
			writer.WriteKeyword("while");
            writer.Write(" (");
			WriteExpression(loop.Condition);
			writer.Terminate(");");
		}

		public void VisitGoto(AbsynGoto g)
		{
			writer.Indent();
			writer.WriteKeyword("goto");
            writer.Write(" ");
			writer.Write(g.Label);
			writer.Terminate(";");
		}

		public void VisitIf(AbsynIf ifs)
		{
			writer.Indent();
			WriteIf(ifs);
		}

		private void WriteIf(AbsynIf ifs)
		{
			writer.WriteKeyword("if");
            writer.Write(" (");
			WriteExpression(ifs.Condition);
			writer.Write(")");
			writer.Terminate();

			WriteIndentedStatements(ifs.Then, false);

			if (ifs.Else != null && ifs.Else.Count > 0)
			{
				writer.Indent();
				writer.WriteKeyword("else");
                AbsynIf elseIf;
                if (IsSingleIfStatement(ifs.Else, out elseIf))
				{
					writer.Write(" ");
					WriteIf(elseIf);
				}
				else
				{
					writer.Terminate();
					WriteIndentedStatements(ifs.Else, false);
				}
			}
		}

        protected virtual string UnsignedFormatString(PrimitiveType type, ulong value)
        {
            switch (type.Size)
            {
            case 1: return "0x{0:X2}";
            case 2: return "0x{0:X4}";
            case 4: return "0x{0:X8}";
            case 8: return "0x{0:X16}";
            case 16: return "0x{0:X16}";
            default: throw new ArgumentOutOfRangeException("type", type.Size, string.Format("Integral types of size {0} bytes are not supported.", type.Size));
            }
        }

        private string FormatString(PrimitiveType type, object value)
        {
            string format;
            switch (type.Domain)
            {
            case Domain.SignedInt:
                return "{0}";
            case Domain.Character:
                switch (type.Size)
                {
                case 1: format = "'{0}'"; break;
                case 2: format = "L'{0}'"; break;
                default: throw new ArgumentOutOfRangeException("Only character types of size 1 and 2 are supported.");
                }
                var ch = Convert.ToChar(value);
                if (Char.IsControl(ch))
                    return string.Format(format, string.Format("\\x{0:X2}", (int) ch));
                else if (ch == '\'' || ch == '\\')
                    return string.Format(format, string.Format("\\{0}", ch));
                return format;
            default:
                return UnsignedFormatString(type, Convert.ToUInt64(value));
            }
        }

        private bool IsSingleIfStatement(List<AbsynStatement> stms, out AbsynIf elseIf)
        {
            elseIf = null;
            if (stms.Count != 1)
                return false;
            elseIf = stms[0] as AbsynIf;
            return elseIf != null;
        }

		public void VisitLabel(AbsynLabel lbl)
		{
			writer.Write(lbl.Name);
			writer.Terminate(":");
		}

        public void VisitLineComment(AbsynLineComment comment)
        {
            writer.WriteComment("// " + comment.Comment);
            writer.Terminate();
        }

		public void VisitReturn(AbsynReturn ret)
		{
			writer.Indent();
			writer.WriteKeyword("return");
			if (ret.Value != null)
			{
				writer.Write(" ");
				WriteExpression(ret.Value);
			}
			writer.Terminate(";");
		}

		public void VisitSideEffect(AbsynSideEffect side)
		{
			writer.Indent();
			side.Expression.Accept(this);
			writer.Terminate(";");
		}

        public void VisitSwitch(AbsynSwitch s)
        {
            writer.Indent();
            writer.WriteKeyword("switch");
            writer.Write(" (");
            WriteExpression(s.Expression);
            writer.Terminate(")");
            WriteIndentedStatements(s.Statements, false);
        }

		public void VisitWhile(AbsynWhile loop)
		{
			writer.Indent();
			writer.WriteKeyword("while");
            writer.Write(" (");
			WriteExpression(loop.Condition);
			writer.Terminate(")");

			WriteIndentedStatements(loop.Body, false);
		}

#endregion

		public void Write(Procedure proc)
		{
			proc.Signature.Emit(proc.QualifiedName(), FunctionType.EmitFlags.None, writer, this, new TypeFormatter(writer));
			writer.WriteLine();
			writer.Write("{");
            writer.WriteLine();
			if (proc.Body != null)
			{
				for (int i = 0; i < proc.Body.Count; ++i)
				{
					proc.Body[i].Accept(this);
				}
			}
			else
			{
                new ProcedureFormatter(proc, new BlockDecorator { ShowEdges=false }, this).WriteProcedureBlocks();
			}
			writer.Write("}");
            writer.WriteLine();
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

        public void WriteFormalArgument(Identifier arg, bool writeStorage, TypeFormatter t)
        {
            if (writeStorage)
            {
                WriteFormalArgumentType(arg, writeStorage);
                writer.Write(" ");
                writer.Write(arg.Name);
            }
            else
            {
                if (arg.Storage is OutArgumentStorage)
                {
                    t.Write(new ReferenceTo(arg.DataType), arg.Name);
                }
                else
                {
                    t.Write(arg.DataType, arg.Name);
                }
            }
        }

        public void WriteFormalArgumentType(Identifier arg, bool writeStorage)
        {
            if (writeStorage)
            {
                OutArgumentStorage os = arg.Storage as OutArgumentStorage;
                if (os != null)
                {
                    writer.Write(os.OriginalIdentifier.Storage.Kind);
                    writer.Write(" out ");
                }
                else
                {
                    writer.Write(arg.Storage.Kind);
                    writer.Write(" ");
                }
            }
            typeWriter = new TypeGraphWriter(writer);
            typeWriter.WriteReference(arg.DataType);
        }

		public void WriteIndentedStatement(AbsynStatement stm)
		{
			writer.Indentation += writer.TabSize;
			if (stm != null)
				stm.Accept(this);
			else
			{
				writer.Indent();				
				writer.Terminate(";");
			}
			writer.Indentation -= writer.TabSize;
		}

        public void WriteIndentedStatements(List<AbsynStatement> stms, bool suppressNewline)
        {
            if (HasSmallBody(stms))
            {
                writer.Indentation += writer.TabSize;
                if (stms.Count == 0)
                {
                    writer.Indent();
                    writer.Terminate(";");
                }
                else
                {
                    stms[0].Accept(this);
                }
                writer.Indentation -= writer.TabSize;
            }
            else
            {
                writer.Indent();
                writer.Write("{");
                writer.Terminate();

                writer.Indentation += writer.TabSize;
                foreach (AbsynStatement stm in stms)
                {
                    stm.Accept(this);
                }
                writer.Indentation -= writer.TabSize;

                writer.Indent();
                writer.Write("}");
                if (suppressNewline)
                    writer.Write(" ");
                else
                    writer.Terminate();
            }
        }

        private bool HasSmallBody(List<AbsynStatement> stms)
        {
            return stms.Count == 0 ||
                   (stms.Count == 1 && IsIrregularStatement(stms[0]));
        }

        private bool IsIrregularStatement(AbsynStatement stm)
        {
            return singleStatements.Contains(stm.GetType());
        }

        public void WriteStatementList(List<AbsynStatement> list)
		{
			foreach (AbsynStatement s in list)
			{
				s.Accept(this);
			}
		}

        public void WriteNull()
        {
            writer.WriteKeyword("null");
        }
    }
}
