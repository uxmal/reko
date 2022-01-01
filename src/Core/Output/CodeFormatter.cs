#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using System.Numerics;

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
        private readonly TypeGraphWriter typeWriter;

        //$TODO: move this to a language-specific class.
        private static readonly Dictionary<Operator,int> precedences;
        private static readonly HashSet<Type> singleStatements;

        /// <summary>
        /// Maps # of nybbles to an appropriate format string.
        /// </summary>
        private static readonly string[] unsignedConstantFormatStrings; 

        private const int PrecedenceApplication = 1;
		private const int PrecedenceArrayAccess = 1;
		private const int PrecedenceFieldAccess = 1;
		private const int PrecedenceDereference = 2;
		private const int PrecedenceMemberPointerSelector = 3;
		private const int PrecedenceCase = 2;
        private const int PrecedenceConditional = 14;
        private const int PrecedenceLeast = 20;

		public CodeFormatter(Formatter writer)
		{
            this.InnerFormatter = writer;
            this.typeWriter = new TypeGraphWriter(writer);
		}

        public Formatter InnerFormatter { get; }

		static CodeFormatter()
		{
            precedences = new Dictionary<Operator, int>
            {
                [Operator.Not] = 2,         //$REFACTOR: precedence is a property of the output language; these are the C/C++ precedences
                [Operator.Neg] = 2,
                [Operator.FNeg] = 2,
                [Operator.Comp] = 2,
                [Operator.AddrOf] = 2,
                [Operator.SMul] = 4,
                [Operator.UMul] = 4,
                [Operator.IMul] = 4,
                [Operator.SDiv] = 4,
                [Operator.UDiv] = 4,
                [Operator.IMod] = 4,
                [Operator.FMul] = 4,
                [Operator.FDiv] = 4,
                [Operator.IAdd] = 5,
                [Operator.ISub] = 5,
                [Operator.USub] = 5,
                [Operator.FAdd] = 5,
                [Operator.FSub] = 5,
                [Operator.Sar] = 6,
                [Operator.Shr] = 6,
                [Operator.Shl] = 6,
                [Operator.Lt] = 7,
                [Operator.Le] = 7,
                [Operator.Gt] = 7,
                [Operator.Ge] = 7,
                [Operator.Feq] = 7,
                [Operator.Fne] = 7,
                [Operator.Flt] = 7,
                [Operator.Fle] = 7,
                [Operator.Fgt] = 7,
                [Operator.Fge] = 7,
                [Operator.Fne] = 7,
                [Operator.Ult] = 7,
                [Operator.Ule] = 7,
                [Operator.Ugt] = 7,
                [Operator.Uge] = 7,
                [Operator.Eq] = 8,
                [Operator.Ne] = 8,
                [Operator.And] = 9,
                [Operator.Xor] = 10,
                [Operator.Or] = 11,
                [Operator.Cand] = 12,
                [Operator.Cor] = 13
            };

            singleStatements = new HashSet<Type>
            {
                typeof(AbsynReturn),
                typeof(AbsynGoto),
                typeof(AbsynBreak),
                typeof(AbsynContinue),
                typeof(AbsynAssignment),
                typeof(AbsynCompoundAssignment),
                typeof(AbsynSideEffect)
            };

            unsignedConstantFormatStrings = Enumerable.Range(0, 17)
                .Select(n => $"0x{{0:X{n*2}}}")
                .ToArray();
        }

        private void ResetPresedence(int precedenceOld)
		{
			if (precedenceOld < precedenceCur ||
                (forceParensIfSamePrecedence && precedenceCur == precedenceOld))
			{
				InnerFormatter.Write(")");
			}
			precedenceCur = precedenceOld;
		}

		private int SetPrecedence(int precedence)
		{
			if (precedenceCur < precedence ||
                (forceParensIfSamePrecedence && precedenceCur == precedence))
			{
				InnerFormatter.Write("(");
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
                {
                    s = string.Format("0x{0}<p{1}>", s, addr.DataType.BitSize);
            }
                InnerFormatter.Write(s);
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
			InnerFormatter.Write("[");
			WriteExpression(acc.Index);
			InnerFormatter.Write("]");
		}

		public void VisitBinaryExpression(BinaryExpression binExp)
		{
			int prec = SetPrecedence((int) precedences[binExp.Operator]);
			binExp.Left.Accept(this);
            FormatOperator(binExp);
            var old = forceParensIfSamePrecedence;
            forceParensIfSamePrecedence = true;
            binExp.Right.Accept(this);
            forceParensIfSamePrecedence = old;
			ResetPresedence(prec);
		}

        private void FormatOperator(BinaryExpression binExp)
        {
            var sOperator = binExp.Operator.ToString();
            var resultSize = binExp.DataType.BitSize;
            if (binExp.Operator is IMulOperator || binExp.Operator is FMulOperator ||
                binExp.Operator is SDivOperator || binExp.Operator is SDivOperator ||
                binExp.Operator is FDivOperator)
            {
                // Multiplication and division are peculiar on many processors because the product/
                // quotient may be different size from either of the operands. It's unclear what
                // the C/C++ standards say about this.
                if (resultSize != binExp.Left.DataType.BitSize ||
                    resultSize != binExp.Right.DataType.BitSize)
                {
                    InnerFormatter.Write(sOperator.TrimEnd());
                    InnerFormatter.Write($"{resultSize} ");
                    return;
                }
            }
            InnerFormatter.Write(sOperator);
        }

        public void VisitCast(Cast cast)
		{
			int prec = SetPrecedence(PrecedenceCase);
			InnerFormatter.Write("(");
            new TypeReferenceFormatter(InnerFormatter).WriteTypeReference(cast.DataType);
			InnerFormatter.Write(") ");
			cast.Expression.Accept(this);
			ResetPresedence(prec);
		}

        public void VisitConditionalExpression(ConditionalExpression cond)
        {
            int prec = SetPrecedence(PrecedenceConditional);
            cond.Condition.Accept(this);
            InnerFormatter.Write(" ? ");
            cond.ThenExp.Accept(this);
            InnerFormatter.Write(" : ");
            cond.FalseExp.Accept(this);
            ResetPresedence(prec);
        }

        public void VisitConditionOf(ConditionOf cond)
		{
			InnerFormatter.Write("cond(");
			WriteExpression(cond.Expression);
			InnerFormatter.Write(")");
		}

        private static readonly char[] nosuffixRequired = new[] { '.', 'E', 'e' };

        public virtual void VisitConstant(Constant c)
        {
            var pt = c.DataType.ResolveAs<PrimitiveType>();
            if (!c.IsValid && (c is InvalidConstant || pt is null || pt.Domain != Domain.Real))
            {
                InnerFormatter.Write("<invalid>");
                return;
            }
            if (c is StringConstant s)
            {
                InnerFormatter.Write('"');
                foreach (var ch in (string) s.GetValue())
                {
                    WriteEscapedCharacter(ch);
                }
                InnerFormatter.Write('"');
                return;
            }

            if (pt != null)
            {
                switch (pt.Domain)
                {
                case Domain.Boolean:
                    InnerFormatter.Write(Convert.ToBoolean(c.GetValue()) ? "true" : "false");
                    break;
                case Domain.Real:
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
                    InnerFormatter.Write(sr);
                    break;
                }
                case Domain.Character:
                    if (pt.Size == 1)
                    {
                        InnerFormatter.Write("'");
                    }
                    else
                    {
                        InnerFormatter.Write("L'"); 
                    }
                    WriteEscapedCharacter(Convert.ToChar(c.GetValue()));
                    InnerFormatter.Write("'");
                    break;
                default:
                    object v = c.GetValue();
                    var (fmtNumber, fmtSigil) = FormatStrings(pt, v);
                    InnerFormatter.Write(fmtNumber, v);
                    InnerFormatter.Write(fmtSigil, pt.BitSize);
                    break;
                }
                return;
            }
        }


        private void WriteEscapedCharacter(char ch)
        {
            switch (ch)
            {
            case '\0': InnerFormatter.Write("\\0"); break;
            case '\a': InnerFormatter.Write("\\a"); break;
            case '\b': InnerFormatter.Write("\\b"); break;
            case '\f': InnerFormatter.Write("\\f"); break;
            case '\n': InnerFormatter.Write("\\n"); break;
            case '\r': InnerFormatter.Write("\\r"); break;
            case '\t': InnerFormatter.Write("\\t"); break;
            case '\v': InnerFormatter.Write("\\v"); break;
            case '\"': InnerFormatter.Write("\\\""); break;
            case '\\': InnerFormatter.Write("\\\\"); break;
            default:
                // The awful hack allows us to reuse .NET encodings
                // while encoding the original untranslateable 
                // code points into the Private use area.
                //$TODO: Clearly if the string was UTF8 or 
                // UTF-16 to begin with, we want to preserve the
                // private use area points.
                if (0xE000 <= ch && ch <= 0xE100)
                    InnerFormatter.Write("\\x{0:X2}", (ch - 0xE000));
                else if (0 <= ch && ch < ' ' || ch >= 0x7F)
                    InnerFormatter.Write("\\x{0:X2}", (int) ch);
                else
                    InnerFormatter.Write(ch);
                break;
            }
        }

        public void VisitConversion(Conversion conversion)
		{
            InnerFormatter.Write("CONVERT(");
            var trf = new TypeReferenceFormatter(InnerFormatter);
            WriteExpression(conversion.Expression);
            InnerFormatter.Write(", ");
            trf.WriteTypeReference(conversion.SourceDataType);
            InnerFormatter.Write(", ");
            trf.WriteTypeReference(conversion.DataType);
			InnerFormatter.Write(")");
		}

		public void VisitDereference(Dereference deref)
		{
			int prec = SetPrecedence(PrecedenceDereference);
			InnerFormatter.Write("*");
			deref.Expression.Accept(this);
			ResetPresedence(prec);
		}

		public void VisitFieldAccess(FieldAccess acc)
		{
			int prec = SetPrecedence(PrecedenceFieldAccess);
            if (acc.Structure is Dereference d)
            {
                d.Expression.Accept(this);
                InnerFormatter.Write("->{0}", acc.Field.Name);
            }
            else
            {
                if (acc.Structure is ScopeResolution scope)
                {
                    scope.Accept(this);
                    InnerFormatter.Write("::{0}", acc.Field.Name);
                }
                else
                {
                    acc.Structure.Accept(this);
                    InnerFormatter.Write(".{0}", acc.Field.Name);
                }
            }
            ResetPresedence(prec);
		}

		public void VisitMemberPointerSelector(MemberPointerSelector mps)
		{
			int prec = SetPrecedence(PrecedenceMemberPointerSelector);
            if (mps.BasePointer is Dereference d)
            {
                d.Expression.Accept(this);
                InnerFormatter.Write("->*");
            }
            else
            {
                mps.BasePointer.Accept(this);
                InnerFormatter.Write(".*");
            }
            var old = forceParensIfSamePrecedence;
            forceParensIfSamePrecedence = true;
			mps.MemberPointer.Accept(this);
            forceParensIfSamePrecedence = old;
            ResetPresedence(prec);
		}

		public void VisitIdentifier(Identifier id)
		{
			InnerFormatter.Write(id.Name);
		}

		public void VisitMemoryAccess(MemoryAccess access)
		{
			access.MemoryId.Accept(this);
			InnerFormatter.Write("[");
			WriteExpression(access.EffectiveAddress);
			InnerFormatter.Write(":");
			InnerFormatter.Write(access.DataType.ToString());
			InnerFormatter.Write("]");
		}

        public void VisitSegmentedAccess(SegmentedAccess access)
        {
            access.MemoryId.Accept(this);
            InnerFormatter.Write("[");
            WriteExpression(access.BasePointer);
            InnerFormatter.Write(":");
            WriteExpression(access.EffectiveAddress);
            InnerFormatter.Write(":");
            Debug.Assert(access.DataType != null);
            InnerFormatter.Write(access.DataType?.ToString() ?? "");
            InnerFormatter.Write("]");
        }

        public void VisitMkSequence(MkSequence seq)
        {
            InnerFormatter.Write("SEQ(");
            var sep = "";
            foreach (var e in seq.Expressions)
            {
                InnerFormatter.Write(sep);
                sep = ", ";
                WriteExpression(e);
            }
            InnerFormatter.Write(")");
        }

        public void VisitOutArgument(OutArgument outArg)
        {
            InnerFormatter.WriteKeyword("out");
            InnerFormatter.Write(" ");
            WriteExpression(outArg.Expression);
        }

		public void VisitPhiFunction(PhiFunction phi)
		{
			InnerFormatter.WriteKeyword("PHI");
            InnerFormatter.Write("(");
            var sep = "";
            foreach (var arg in phi.Arguments)
            {
                InnerFormatter.Write(sep);
                sep = ", ";
                InnerFormatter.Write("(");
                arg.Value.Accept(this);
                InnerFormatter.Write(", ");
                InnerFormatter.Write(arg.Block.DisplayName);
                InnerFormatter.Write(")");
            }
            InnerFormatter.Write(")");
        }

        public void VisitPointerAddition(PointerAddition pa)
		{
            InnerFormatter.Write("PTRADD(");
            WriteExpression(pa.Pointer);
            InnerFormatter.Write(",{0})", pa.Offset);
		}

		public virtual void VisitProcedureConstant(ProcedureConstant pc)
		{
			InnerFormatter.WriteHyperlink(pc.Procedure.Name, pc.Procedure);
		}

		public void VisitTestCondition(TestCondition tc)
		{
			InnerFormatter.Write("Test({0},", tc.ConditionCode);
			WriteExpression(tc.Expression);
			InnerFormatter.Write(")");
		}

        public void VisitScopeResolution(ScopeResolution scope)
        {
            InnerFormatter.WriteType(scope.DataType.Name, scope.DataType);
        }

		public void VisitSlice(Slice slice)
		{
			InnerFormatter.Write("SLICE(");
			WriteExpression(slice.Expression);
			InnerFormatter.Write(", {0}, {1})", slice.DataType, slice.Offset);
		}

		public void VisitUnaryExpression(UnaryExpression unary)
		{
			int prec = SetPrecedence((int) precedences[unary.Operator]);
			InnerFormatter.Write(unary.Operator.ToString());
			unary.Expression.Accept(this);
			ResetPresedence(prec);
		}

		#endregion


		#region InstructionVisitor ///////////////////////////////
		
		public void VisitAssignment(Assignment a)
		{
			InnerFormatter.Indent();
			if (a.Dst != null)
			{
				a.Dst.Accept(this);
				InnerFormatter.Write(" = "); 
			}
			a.Src.Accept(this);
			if (a.IsAlias)
				InnerFormatter.Write(" (alias)");
			InnerFormatter.Terminate();
		}

		public void VisitBranch(Branch b)
		{
			InnerFormatter.Indent();
			InnerFormatter.WriteKeyword("branch");
            InnerFormatter.Write(" ");
			b.Condition.Accept(this);
            InnerFormatter.Write(" ");
            InnerFormatter.Write(b.Target.DisplayName);
			InnerFormatter.Terminate();
		}

		public void VisitCallInstruction(CallInstruction ci)
		{
			InnerFormatter.Indent();
			InnerFormatter.WriteKeyword("call");
            InnerFormatter.Write(" ");
            ci.Callee.Accept(this);
            InnerFormatter.Write(" ({0})", ci.CallSite);
			InnerFormatter.Terminate();
            if (ci.Uses.Count > 0)
            {
                InnerFormatter.Indentation += InnerFormatter.TabSize;
                InnerFormatter.Indent();
                InnerFormatter.Write("uses: ");
                WriteCallBindings(ci.Uses);
                InnerFormatter.Terminate();
                InnerFormatter.Indentation -= InnerFormatter.TabSize;
            }
            if (ci.Definitions.Count > 0)
            {
                InnerFormatter.Indentation += InnerFormatter.TabSize;
                InnerFormatter.Indent();
                InnerFormatter.Write("defs: ");
                WriteCallBindings(ci.Definitions);
                InnerFormatter.Terminate();
                InnerFormatter.Indentation -= InnerFormatter.TabSize;
            }
		}

        private void WriteCallBindings(IEnumerable<CallBinding> bindings)
        {
            var sep = "";
            foreach (var binding in bindings.OrderBy(b => b.Storage.ToString()))
            {
                InnerFormatter.Write(sep);
                sep = ",";
                InnerFormatter.Write(binding.Storage.ToString());
                InnerFormatter.Write(":");
                binding.Expression.Accept(this);
            }
        }
        public void VisitComment(CodeComment comment)
        {
            foreach (var line in Lines(comment.Text))
            {
                InnerFormatter.Indent();
                InnerFormatter.WriteComment($"// {line}");
                InnerFormatter.Terminate();
            }
        }

        /// <summary>
        /// //$REVIEW: naturally for non-C++ like languages, this needs to be 
        /// done differently. 
        /// </summary>
        /// <param name="compound"></param>
        public void VisitCompoundAssignment(AbsynCompoundAssignment compound)
        {
            InnerFormatter.Indent();
            WriteCompoundAssignment(compound);
            InnerFormatter.Terminate(";");
        }

        public void WriteCompoundAssignment(AbsynCompoundAssignment compound)
        { 
            if (compound.Src.Right is Constant c &&
                !c.IsReal && c.ToInt64() == 1)
            {
                if (compound.Src.Operator == Operator.IAdd)
                {
                    InnerFormatter.Write("++");
                    compound.Dst.Accept(this);
                    return;
                } else if (compound.Src.Operator == Operator.ISub)
                {
                    InnerFormatter.Write("--");
                    compound.Dst.Accept(this);
                    return;
                }
            }
            compound.Dst.Accept(this);
            InnerFormatter.Write(compound.Src.Operator.AsCompound());
            compound.Src.Right.Accept(this);
        }

        public void VisitDeclaration(Declaration decl)
		{
			InnerFormatter.Indent();
            Debug.Assert(decl.Identifier.DataType != null, "The DataType property can't ever be null");

            var tf = new TypeReferenceFormatter(InnerFormatter);
            tf.WriteDeclaration(decl.Identifier.DataType ?? new UnknownType(), decl.Identifier.Name);
            
            if (decl.Expression != null)
			{
				InnerFormatter.Write(" = ");
				decl.Expression.Accept(this);
			}
			InnerFormatter.Terminate();
		}

		public void VisitDefInstruction(DefInstruction def)
		{
			InnerFormatter.Indent();
            InnerFormatter.WriteKeyword("def");
            InnerFormatter.Write(" ");
			def.Identifier.Accept(this);
			InnerFormatter.Terminate();
		}

        public void VisitGotoInstruction(GotoInstruction g)
        {
            InnerFormatter.Indent();
            if (g.Condition != null)
            {
                InnerFormatter.WriteKeyword("if");
                InnerFormatter.Write(" (");
                g.Condition.Accept(this);
                InnerFormatter.Write(") ");
            }
            InnerFormatter.WriteKeyword("goto");
            InnerFormatter.WriteKeyword(" ");
            g.Target.Accept(this);
            InnerFormatter.Terminate();
        }

		public void VisitPhiAssignment(PhiAssignment phi)
		{
			InnerFormatter.Indent();
			WriteExpression(phi.Dst);
			InnerFormatter.Write(" = ");
			WriteExpression(phi.Src);
			InnerFormatter.Terminate();
		}

		public void VisitReturnInstruction(ReturnInstruction ret)
		{
			InnerFormatter.Indent();
			InnerFormatter.WriteKeyword("return");
			if (ret.Expression != null)
			{
				InnerFormatter.Write(" ");
				WriteExpression(ret.Expression);
			}
			InnerFormatter.Terminate();
		}

		public void VisitSideEffect(SideEffect side)
		{
			InnerFormatter.Indent();
			side.Expression.Accept(this);
			InnerFormatter.Terminate();
		}

		public void VisitStore(Store store)
		{
			InnerFormatter.Indent();
			WriteExpression(store.Dst);
			InnerFormatter.Write(" = ");
			WriteExpression(store.Src);
			InnerFormatter.Terminate();
		}

		public void VisitSwitchInstruction(SwitchInstruction si)
		{
			InnerFormatter.Indent();
			InnerFormatter.WriteKeyword("switch");
            InnerFormatter.Write(" (");
			si.Expression.Accept(this);
			InnerFormatter.Write(") { ");
			foreach (Block b in si.Targets)
			{
				InnerFormatter.Write("{0} ", b.DisplayName);
			}
			InnerFormatter.Write("}");
			InnerFormatter.Terminate();
		}

		public void VisitUseInstruction(UseInstruction u)
		{
			InnerFormatter.Indent();
			InnerFormatter.WriteKeyword("use");
            InnerFormatter.Write(" ");
			WriteExpression(u.Expression);
			if (u.OutArgument != null)
			{
				InnerFormatter.Write(" (=> {0})", u.OutArgument);
			}
			InnerFormatter.Terminate();
		}
        #endregion


        #region IAbsynStatementVisitor //////////////////////

        public void VisitAssignment(AbsynAssignment a)
        {
            InnerFormatter.Indent();
            WriteAssignment(a);
			InnerFormatter.Terminate(";");
        }

        private void WriteAssignment(AbsynAssignment a)
        { 
			a.Dst.Accept(this);
			InnerFormatter.Write(" = ");
			a.Src.Accept(this);
		}

		public void VisitBreak(AbsynBreak brk)
		{
			InnerFormatter.Indent();
			InnerFormatter.WriteKeyword("break");
			InnerFormatter.Terminate(";");
		}

        public void VisitCase(AbsynCase c)
        {
            InnerFormatter.Indentation -= InnerFormatter.TabSize;
            InnerFormatter.Indent();
            InnerFormatter.WriteKeyword("case");
            InnerFormatter.Write(" ");
            c.Constant.Accept(this);
            InnerFormatter.Terminate(":");
            InnerFormatter.Indentation += InnerFormatter.TabSize;
        }

        public void VisitDefault(AbsynDefault d)
        {
            InnerFormatter.Indentation -= InnerFormatter.TabSize;
            InnerFormatter.Indent();
            InnerFormatter.WriteKeyword("default");
            InnerFormatter.Terminate(":");
            InnerFormatter.Indentation += InnerFormatter.TabSize;
        }
		
        public void VisitContinue(AbsynContinue cont)
		{
            InnerFormatter.Indent();
			InnerFormatter.WriteKeyword("continue");
            InnerFormatter.Terminate(";");
		}

		public void VisitDeclaration(AbsynDeclaration decl)
		{
			InnerFormatter.Indent();
			if (decl.Identifier.DataType != null)
			{
                TypeReferenceFormatter tf = new TypeReferenceFormatter(InnerFormatter);
                tf.WriteDeclaration(decl.Identifier.DataType, decl.Identifier.Name);
			}
			else
			{
                InnerFormatter.Write("?unknown?");
                InnerFormatter.Write(" ");
                decl.Identifier.Accept(this);
            }
			if (decl.Expression != null)
			{
				InnerFormatter.Write(" = ");
				decl.Expression.Accept(this);
			}
			InnerFormatter.Terminate(";");
		}

		public void VisitDoWhile(AbsynDoWhile loop)
		{
			InnerFormatter.Indent();
			InnerFormatter.WriteKeyword("do");
			InnerFormatter.Terminate();
			WriteIndentedStatements(loop.Body, true);
			
			if (HasSmallBody(loop.Body))
                InnerFormatter.Indent();
			InnerFormatter.WriteKeyword("while");
            InnerFormatter.Write(" (");
			WriteExpression(loop.Condition);
			InnerFormatter.Terminate(");");
		}

        public void VisitFor(AbsynFor forLoop)
        {
            InnerFormatter.Indent();
            InnerFormatter.WriteKeyword("for");
            InnerFormatter.Write(" (");
            MaybeWriteAssignment(forLoop.Initialization);
            InnerFormatter.Write("; ");
            forLoop.Condition.Accept(this);
            InnerFormatter.Write("; ");
            MaybeWriteAssignment(forLoop.Iteration);
            InnerFormatter.Terminate(")");

            WriteIndentedStatements(forLoop.Body, false);
        }

        private void MaybeWriteAssignment(AbsynAssignment ass)
        {
            if (ass != null)
            {
                if (ass is AbsynCompoundAssignment cass)
                {
                    WriteCompoundAssignment(cass);
                }
                else
                {
                    WriteAssignment(ass);
                }
            }
        }

        public void VisitGoto(AbsynGoto g)
		{
			InnerFormatter.Indent();
			InnerFormatter.WriteKeyword("goto");
            InnerFormatter.Write(" ");
			InnerFormatter.Write(g.Label);
			InnerFormatter.Terminate(";");
		}

		public void VisitIf(AbsynIf ifs)
		{
			InnerFormatter.Indent();
			WriteIf(ifs);
		}

		private void WriteIf(AbsynIf ifs)
		{
			InnerFormatter.WriteKeyword("if");
            InnerFormatter.Write(" (");
			WriteExpression(ifs.Condition);
			InnerFormatter.Write(")");
			InnerFormatter.Terminate();

			WriteIndentedStatements(ifs.Then, false);

			if (ifs.Else != null && ifs.Else.Count > 0)
			{
				InnerFormatter.Indent();
				InnerFormatter.WriteKeyword("else");
                if (IsSingleIfStatement(ifs.Else, out AbsynIf elseIf))
                {
                    InnerFormatter.Write(" ");
                    WriteIf(elseIf);
                }
                else
                {
                    InnerFormatter.Terminate();
                    WriteIndentedStatements(ifs.Else, false);
                }
            }
		}

        protected virtual string UnsignedFormatString(PrimitiveType type, ulong value)
        {
            if (value < 10)
                return "{0}";
            var nybbles = Nybbles(type.BitSize);
            if (nybbles < unsignedConstantFormatStrings.Length)
                return unsignedConstantFormatStrings[nybbles];
            else
                return "0x{0}_";
        }

        private static int Nybbles(int bitSize)
        {
            return (bitSize + 3) / 4;
        }

        private (string,string) FormatStrings(PrimitiveType type, object value)
        {
            string format;
            switch (type.Domain)
            {
            case Domain.SignedInt:
                return ("{0}", "<i{0}>");
            case Domain.Character:
                switch (type.Size)
                {
                case 1: format = "'{0}'"; break;
                case 2: format = "L'{0}'"; break;
                default: throw new ArgumentOutOfRangeException("Only character types of size 1 and 2 are supported.");
                }
                var ch = Convert.ToChar(value);
                if (Char.IsControl(ch))
                    return (string.Format(format, string.Format("\\x{0:X2}", (int) ch)), "");
                else if (ch == '\'' || ch == '\\')
                    return (string.Format(format, string.Format("\\{0}", ch)), "");
                return (format, "");
            case Domain.UnsignedInt:
                if (value is BigInteger ubig)
                {
                    if (ubig > 9)
                    {
                        format = "0x{0:X}";
                    }
                    else
                    {
                        format = "{0}";
                    }
                }
                else
                {
                    if (value is not ulong n)
                        n = Convert.ToUInt64(value);
                    if (n > 9)
                    {
                        format = "0x{0:X}";
                    }
                    else
                    {
                        format = "{0}";
                    }
                }
                return (format, "<u{0}>");
            case Domain.Pointer:
            case Domain.Offset:
                return (unsignedConstantFormatStrings[type.Size], "<p{0}>");
            case Domain.SegPointer:
                return ("{0:X}", "p{0}");
            default:
                if (value is BigInteger sbig)
                {
                    if (sbig > 9)
                    {
                        format = "0x{0:X}";
                    }
                    else
                    {
                        format = "{0}";
                    }
                }
                else
                {
                    if (!(value is ulong w))
                    {
                        w = (ulong) Convert.ToInt64(value);
                    }
                    if (w > 9)
                    {
                        format = "0x{0:X}";
                    }
                    else
                    {
                        format = "{0}";
                    }
                }
                return (format, "<{0}>");
            }
        }

        //$TODO: .NET 5 output parameter is non-null if the method returns true.
        private bool IsSingleIfStatement(List<AbsynStatement> stms, out AbsynIf elseIf)
        {
            elseIf = default!;
            if (stms.Count != 1)
                return false;
            elseIf = (stms[0] as AbsynIf)!;
            return elseIf != null;
        }

		public void VisitLabel(AbsynLabel lbl)
		{
			InnerFormatter.Write(lbl.Name);
			InnerFormatter.Terminate(":");
		}

        public void VisitLineComment(AbsynLineComment comment)
        {
            foreach (var line in Lines(comment.Comment))
            {
                InnerFormatter.Indent();
                InnerFormatter.WriteComment($"// {line}");
                InnerFormatter.Terminate();
            }
        }

		public void VisitReturn(AbsynReturn ret)
		{
			InnerFormatter.Indent();
			InnerFormatter.WriteKeyword("return");
			if (ret.Value != null)
			{
				InnerFormatter.Write(" ");
				WriteExpression(ret.Value);
			}
			InnerFormatter.Terminate(";");
		}

		public void VisitSideEffect(AbsynSideEffect side)
		{
			InnerFormatter.Indent();
			side.Expression.Accept(this);
			InnerFormatter.Terminate(";");
		}

        public void VisitSwitch(AbsynSwitch s)
        {
            InnerFormatter.Indent();
            InnerFormatter.WriteKeyword("switch");
            InnerFormatter.Write(" (");
            WriteExpression(s.Expression);
            InnerFormatter.Terminate(")");
            WriteIndentedStatements(s.Statements, false);
        }

		public void VisitWhile(AbsynWhile loop)
		{
			InnerFormatter.Indent();
			InnerFormatter.WriteKeyword("while");
            InnerFormatter.Write(" (");
			WriteExpression(loop.Condition);
			InnerFormatter.Terminate(")");

			WriteIndentedStatements(loop.Body, false);
		}

#endregion

		public void Write(Procedure proc)
		{
			proc.Signature.Emit(proc.QualifiedName(), FunctionType.EmitFlags.None, InnerFormatter, this, new TypeReferenceFormatter(InnerFormatter));
			InnerFormatter.WriteLine();
			InnerFormatter.Write("{");
            InnerFormatter.WriteLine();
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
			InnerFormatter.Write("}");
            InnerFormatter.WriteLine();
		}

    	private void WriteActuals(Expression [] arguments)
		{
			InnerFormatter.Write("(");
			if (arguments.Length >= 1)
			{
				WriteExpression(arguments[0]);
				for (int i = 1; i < arguments.Length; ++i)
				{
					InnerFormatter.Write(", ");
					WriteExpression(arguments[i]);
				}
			}
			InnerFormatter.Write(")");
		}

		/// <summary>
		/// Writes an expression in a context where it needs no parentheses.
		/// </summary>
		/// <param name="expr"></param>
		public void WriteExpression(Expression expr)
		{
            if (expr == null)
            {
                InnerFormatter.Write("<NULL>");
                return;
            }
			int prec = precedenceCur;
			precedenceCur = PrecedenceLeast;
			expr.Accept(this);
			precedenceCur = prec;
		}

        public void WriteFormalArgument(Identifier arg, bool writeStorage, TypeReferenceFormatter t)
        {
            if (writeStorage)
            {
                WriteFormalArgumentType(arg, writeStorage);
                InnerFormatter.Write(" ");
                InnerFormatter.Write(arg.Name);
            }
            else
            {
                if (arg.Storage is OutArgumentStorage)
                {
                    t.WriteDeclaration(new ReferenceTo(arg.DataType), arg.Name);
                }
                else
                {
                    t.WriteDeclaration(arg.DataType, arg.Name);
                }
            }
        }

        public void WriteFormalArgumentType(Identifier arg, bool writeStorage)
        {
            if (writeStorage)
            {
                if (arg.Storage is OutArgumentStorage os)
                {
                    InnerFormatter.Write(os.OriginalIdentifier.Storage.Kind);
                    InnerFormatter.Write(" out ");
                }
                else
                {
                    InnerFormatter.Write(arg.Storage.Kind);
                    InnerFormatter.Write(" ");
                }
            }
            typeWriter.WriteReference(arg.DataType);
        }

		public void WriteIndentedStatement(AbsynStatement stm)
		{
			InnerFormatter.Indentation += InnerFormatter.TabSize;
			if (stm != null)
				stm.Accept(this);
			else
			{
				InnerFormatter.Indent();				
				InnerFormatter.Terminate(";");
			}
			InnerFormatter.Indentation -= InnerFormatter.TabSize;
		}

        public void WriteIndentedStatements(List<AbsynStatement> stms, bool suppressNewline)
        {
            if (HasSmallBody(stms))
            {
                InnerFormatter.Indentation += InnerFormatter.TabSize;
                if (stms.Count == 0)
                {
                    InnerFormatter.Indent();
                    InnerFormatter.Terminate(";");
                }
                else
                {
                    stms[0].Accept(this);
                }
                InnerFormatter.Indentation -= InnerFormatter.TabSize;
            }
            else
            {
                InnerFormatter.Indent();
                InnerFormatter.Write("{");
                InnerFormatter.Terminate();

                InnerFormatter.Indentation += InnerFormatter.TabSize;
                foreach (AbsynStatement stm in stms)
                {
                    stm.Accept(this);
                }
                InnerFormatter.Indentation -= InnerFormatter.TabSize;

                InnerFormatter.Indent();
                InnerFormatter.Write("}");
                if (suppressNewline)
                    InnerFormatter.Write(" ");
                else
                    InnerFormatter.Terminate();
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
            InnerFormatter.WriteKeyword("null");
        }

        private static string[] Lines(string s)
        {
            return s.Split(
                new string[] { Environment.NewLine },
                StringSplitOptions.None);
        }
    }
}
