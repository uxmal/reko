#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
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
        private static readonly Dictionary<OperatorType,int> precedences;
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

        /// <summary>
        /// Creates an instance of the <see cref="CodeFormatter"/> class.
        /// </summary>
        /// <param name="writer"><see cref="Formatter"/> used as an output sink.</param>
		public CodeFormatter(Formatter writer)
		{
            this.InnerFormatter = writer;
            this.typeWriter = new TypeGraphWriter(writer);
		}

        /// <summary>
        /// The output sink.
        /// </summary>
        public Formatter InnerFormatter { get; }

		static CodeFormatter()
		{
            precedences = new Dictionary<OperatorType, int>
            {
                [(OperatorType)(-1)] = 0,
                [OperatorType.Not] = 2,         //$REFACTOR: precedence is a property of the output language; these are the C/C++ precedences
                [OperatorType.Neg] = 2,
                [OperatorType.FNeg] = 2,
                [OperatorType.Comp] = 2,
                [OperatorType.AddrOf] = 2,
                [OperatorType.SMul] = 4,
                [OperatorType.UMul] = 4,
                [OperatorType.IMul] = 4,
                [OperatorType.SDiv] = 4,
                [OperatorType.UDiv] = 4,
                [OperatorType.IMod] = 4,
                [OperatorType.SMod] = 4,
                [OperatorType.UMod] = 4,
                [OperatorType.FMul] = 4,
                [OperatorType.FDiv] = 4,
                [OperatorType.FMod] = 4,
                [OperatorType.IAdd] = 5,
                [OperatorType.ISub] = 5,
                [OperatorType.USub] = 5,
                [OperatorType.FAdd] = 5,
                [OperatorType.FSub] = 5,
                [OperatorType.Sar] = 6,
                [OperatorType.Shr] = 6,
                [OperatorType.Shl] = 6,
                [OperatorType.Lt] = 7,
                [OperatorType.Le] = 7,
                [OperatorType.Gt] = 7,
                [OperatorType.Ge] = 7,
                [OperatorType.Feq] = 7,
                [OperatorType.Fne] = 7,
                [OperatorType.Flt] = 7,
                [OperatorType.Fle] = 7,
                [OperatorType.Fgt] = 7,
                [OperatorType.Fge] = 7,
                [OperatorType.Ult] = 7,
                [OperatorType.Ule] = 7,
                [OperatorType.Ugt] = 7,
                [OperatorType.Uge] = 7,
                [OperatorType.Eq] = 8,
                [OperatorType.Ne] = 8,
                [OperatorType.And] = 9,
                [OperatorType.Xor] = 10,
                [OperatorType.Or] = 11,
                [OperatorType.Cand] = 12,
                [OperatorType.Cor] = 13
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

        /// <summary>
        /// Renders an address.
        /// </summary>
        /// <param name="addr">Address to render.</param>
        public virtual void VisitAddress(Address addr)
        {
            var s = addr.ToString();
            if (!s.Contains(':'))
            {
                var prefix = addr.PreferredBase == 8 ? "0o" : "0x";
                s = $"{prefix}{s}<p{addr.DataType.BitSize}>";
            }
            InnerFormatter.Write(s);
        }

        /// <summary>
        /// Renders an <see cref="Application"/>.
        /// </summary>
        /// <param name="appl">Application to render.</param>
		public void VisitApplication(Application appl)
		{
			int prec = SetPrecedence(PrecedenceApplication);
			appl.Procedure.Accept(this);
			ResetPresedence(prec);
			WriteActuals(appl.Arguments);
		}

        /// <summary>
        /// Renders an array access.
        /// </summary>
        /// <param name="acc">Array access to render.</param>
		public void VisitArrayAccess(ArrayAccess acc)
		{
			int prec = SetPrecedence(PrecedenceArrayAccess);
			acc.Array.Accept(this);
			ResetPresedence(prec);
			InnerFormatter.Write("[");
			WriteExpression(acc.Index);
			InnerFormatter.Write("]");
		}

        /// <summary>
        /// Renders a binary expression.
        /// </summary>
        /// <param name="binExp">Binary expression to render.</param>
		public void VisitBinaryExpression(BinaryExpression binExp)
		{
			int prec = SetPrecedence((int) precedences[binExp.Operator.Type]);
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
            var sOperator = binExp.Operator.ToString()!;
            var resultSize = binExp.DataType.BitSize;
            if (binExp.Operator is IMulOperator || binExp.Operator is FMulOperator ||
                binExp.Operator is SDivOperator || binExp.Operator is SDivOperator ||
                binExp.Operator is FDivOperator)
            {
                // Multiplication and division are peculiar on many processors because the product/
                // quotient may be different size from either of the operands. It's unclear what
                // the C/C++ standards say about this.
                if (resultSize != 0 && 
                    (resultSize != binExp.Left.DataType.BitSize ||
                     resultSize != binExp.Right.DataType.BitSize))
                {
                    InnerFormatter.Write(sOperator.TrimEnd());
                    InnerFormatter.Write($"{resultSize} ");
                    return;
                }
            }
            InnerFormatter.Write(sOperator);
        }

        /// <summary>
        /// Renders a cast expression.
        /// </summary>
        /// <param name="cast">Cast expression to render.</param>
        public void VisitCast(Cast cast)
		{
			int prec = SetPrecedence(PrecedenceCase);
			InnerFormatter.Write("(");
            new TypeReferenceFormatter(InnerFormatter).WriteTypeReference(cast.DataType);
			InnerFormatter.Write(") ");
			cast.Expression.Accept(this);
			ResetPresedence(prec);
		}

        /// <summary>
        /// Renders a conditional expression.
        /// </summary>
        /// <param name="cond">Conditional expression to render.</param>
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

        /// <summary>
        /// Renders a <see cref="ConditionOf"/>.
        /// </summary>
        /// <param name="cond"><see cref="ConditionOf"/> expression to render.</param>
        public void VisitConditionOf(ConditionOf cond)
		{
			InnerFormatter.Write("cond(");
			WriteExpression(cond.Expression);
			InnerFormatter.Write(")");
		}

        private static readonly char[] nosuffixRequired = new[] { '.', 'E', 'e' };

        /// <summary>
        /// Renders a constant.
        /// </summary>
        /// <param name="c">Constant to render.</param>
        public virtual void VisitConstant(Constant c)
        {
            var dt = c.DataType;
            var pt = dt.ResolveAs<PrimitiveType>();
            if (!c.IsValid && (c is InvalidConstant || pt is null || pt.Domain != Domain.Real))
            {
                InnerFormatter.Write("<invalid>");
                return;
            }
            switch (dt.Domain)
            {
            case Domain.Boolean:
                InnerFormatter.Write(Convert.ToBoolean(c.GetValue()) ? "true" : "false");
                break;
            case Domain.Real:
                {
                    string sr;

                    if (dt.Size == 4)
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
                    if (dt.Size == 4)
                    {
                        sr += "F";
                    }
                    InnerFormatter.Write(sr);
                    break;
                }
            case Domain.Character:
                if (dt.Size == 1)
                {
                    InnerFormatter.Write("'");
                }
                else
                {
                    InnerFormatter.Write("L'");
                }
                WriteEscapedCharacter(Convert.ToChar(c.GetValue()), false);
                InnerFormatter.Write("'");
                break;
            case Domain.Array:
            default:
                object v = c.GetValue();
                var (fmtNumber, fmtSigil) = FormatStrings(dt, v);
                InnerFormatter.Write(fmtNumber, v);
                InnerFormatter.Write(fmtSigil, dt.BitSize);
                break;
            }
            return;
        }

        private void WriteEscapedCharacter(char ch, bool inString)
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
            case '\'': InnerFormatter.Write(inString ? "'" : "\\'"); break;
            case '\"': InnerFormatter.Write(inString ? "\\\"" : "\""); break;
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

        /// <summary>
        /// Renders a conversion.
        /// </summary>
        /// <param name="conversion">Conversion to render.</param>
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

        /// <summary>
        /// Renders a dereference expression.
        /// </summary>
        /// <param name="deref">Dereference to render.</param>
		public void VisitDereference(Dereference deref)
		{
			int prec = SetPrecedence(PrecedenceDereference);
			InnerFormatter.Write("*");
			deref.Expression.Accept(this);
			ResetPresedence(prec);
		}

        /// <summary>
        /// Renders a field access.
        /// </summary>
        /// <param name="acc">Field access to render.</param>
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

        /// <summary>
        /// Renders a <see cref="MemberPointerSelector"/> expression.
        /// </summary>
        /// <param name="mps">Member pointer selector expression to render.</param>
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

        /// <summary>
        /// Renders an identifier.
        /// </summary>
        /// <param name="id">Identifier to render.</param>
		public void VisitIdentifier(Identifier id)
		{
			InnerFormatter.Write(id.Name);
		}

        /// <summary>
        /// Renders a memory access.
        /// </summary>
        /// <param name="access">Memory access to render.</param>
		public void VisitMemoryAccess(MemoryAccess access)
		{
			access.MemoryId.Accept(this);
			InnerFormatter.Write("[");
			WriteExpression(access.EffectiveAddress);
			InnerFormatter.Write(":");
			InnerFormatter.Write(access.DataType.ToString());
			InnerFormatter.Write("]");
		}

        /// <summary>
        /// Renders a segmented address.
        /// </summary>
        /// <param name="segptr"></param>
        public void VisitSegmentedAddress(SegmentedPointer segptr)
        {
            WriteExpression(segptr.BasePointer);
            InnerFormatter.Write(":");
            WriteExpression(segptr.Offset);
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public void VisitOutArgument(OutArgument outArg)
        {
            InnerFormatter.WriteKeyword("out");
            InnerFormatter.Write(" ");
            WriteExpression(outArg.Expression);
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public void VisitPointerAddition(PointerAddition pa)
		{
            InnerFormatter.Write("PTRADD(");
            WriteExpression(pa.Pointer);
            InnerFormatter.Write(",{0})", pa.Offset);
		}

        /// <inheritdoc/>
		public virtual void VisitProcedureConstant(ProcedureConstant pc)
		{
			InnerFormatter.WriteHyperlink(pc.Procedure.Name, pc.Procedure);
            var genArgs = pc.Procedure.GetGenericArguments();
            if (genArgs.Length > 0)
            {
                var sep = '<';
                var tf = new TypeReferenceFormatter(InnerFormatter);
                foreach (var arg in genArgs)
                {
                    InnerFormatter.Write(sep);
                    sep = ',';
                    tf.WriteTypeReference(arg);
                }
                InnerFormatter.Write('>');
            }
		}

        /// <inheritdoc/>
		public void VisitTestCondition(TestCondition tc)
		{
			InnerFormatter.Write("Test({0},", tc.ConditionCode);
			WriteExpression(tc.Expression);
			InnerFormatter.Write(")");
		}

        /// <inheritdoc/>
        public void VisitScopeResolution(ScopeResolution scope)
        {
            InnerFormatter.WriteType(scope.DataType.Name, scope.DataType);
        }

        /// <inheritdoc/>
        public void VisitSlice(Slice slice)
		{
			InnerFormatter.Write("SLICE(");
			WriteExpression(slice.Expression);
			InnerFormatter.Write(", {0}, {1})", slice.DataType, slice.Offset);
		}

        /// <inheritdoc/>
		public void VisitUnaryExpression(UnaryExpression unary)
		{
			int prec = SetPrecedence((int) precedences[unary.Operator.Type]);
			InnerFormatter.Write(unary.Operator.ToString()!);
			unary.Expression.Accept(this);
			ResetPresedence(prec);
		}

		#endregion


		#region InstructionVisitor ///////////////////////////////
		
        /// <inheritdoc/>
		public void VisitAssignment(Assignment a)
		{
			InnerFormatter.Indent();
			if (a.Dst is not null)
			{
				a.Dst.Accept(this);
				InnerFormatter.Write(" = "); 
			}
			a.Src.Accept(this);
			if (a is AliasAssignment)
				InnerFormatter.Write(" (alias)");
			InnerFormatter.Terminate();
		}

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public void WriteCompoundAssignment(AbsynCompoundAssignment compound)
        { 
            if (compound.Src.Right is Constant c &&
                !c.IsReal && c.ToInt64() == 1)
            {
                if (compound.Src.Operator.Type == OperatorType.IAdd)
                {
                    InnerFormatter.Write("++");
                    compound.Dst.Accept(this);
                    return;
                } else if (compound.Src.Operator.Type == OperatorType.ISub)
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

        /// <inheritdoc/>
		public void VisitDefInstruction(DefInstruction def)
		{
			InnerFormatter.Indent();
            InnerFormatter.WriteKeyword("def");
            InnerFormatter.Write(" ");
			def.Identifier.Accept(this);
			InnerFormatter.Terminate();
		}

        /// <inheritdoc/>
        public void VisitGotoInstruction(GotoInstruction g)
        {
            InnerFormatter.Indent();
            if (g.Condition is not null)
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

        /// <inheritdoc/>
		public void VisitPhiAssignment(PhiAssignment phi)
		{
			InnerFormatter.Indent();
			WriteExpression(phi.Dst);
			InnerFormatter.Write(" = ");
			WriteExpression(phi.Src);
			InnerFormatter.Terminate();
		}

        /// <inheritdoc/>
		public void VisitReturnInstruction(ReturnInstruction ret)
		{
			InnerFormatter.Indent();
			InnerFormatter.WriteKeyword("return");
			if (ret.Expression is not null)
			{
				InnerFormatter.Write(" ");
				WriteExpression(ret.Expression);
			}
			InnerFormatter.Terminate();
		}

        /// <inheritdoc/>
		public void VisitSideEffect(SideEffect side)
		{
			InnerFormatter.Indent();
			side.Expression.Accept(this);
			InnerFormatter.Terminate();
		}

        /// <inheritdoc/>
		public void VisitStore(Store store)
		{
			InnerFormatter.Indent();
			WriteExpression(store.Dst);
			InnerFormatter.Write(" = ");
			WriteExpression(store.Src);
			InnerFormatter.Terminate();
        }

        /// <inheritdoc/>
        public void VisitStringConstant(StringConstant s)
        {
            InnerFormatter.Write('"');
            foreach (var ch in s.ToString())
            {
                WriteEscapedCharacter(ch, true);
            }
            InnerFormatter.Write('"');
            return;
        }

        /// <inheritdoc/>
        public void VisitSwitchInstruction(SwitchInstruction si)
		{
			InnerFormatter.Indent();
			InnerFormatter.WriteKeyword("switch");
            InnerFormatter.Write(" (");
			si.Expression.Accept(this);
			InnerFormatter.Write(") { ");
			foreach (Block b in si.Targets)
			{
				InnerFormatter.Write("{0} ", b?.DisplayName ?? "<null>");
			}
			InnerFormatter.Write("}");
			InnerFormatter.Terminate();
		}

        /// <inheritdoc/>
		public void VisitUseInstruction(UseInstruction u)
		{
			InnerFormatter.Indent();
			InnerFormatter.WriteKeyword("use");
            InnerFormatter.Write(" ");
			WriteExpression(u.Expression);
			if (u.OutArgument is not null)
			{
				InnerFormatter.Write(" (=> {0})", u.OutArgument);
			}
			InnerFormatter.Terminate();
		}
        #endregion


        #region IAbsynStatementVisitor //////////////////////

        /// <inheritdoc/>
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

        /// <summary>
        /// Renders a break statement.
        /// </summary>
        /// <param name="brk">Break statement to render.</param>
		public void VisitBreak(AbsynBreak brk)
		{
			InnerFormatter.Indent();
			InnerFormatter.WriteKeyword("break");
			InnerFormatter.Terminate(";");
		}

        /// <summary>
        /// Renders a switch case.
        /// </summary>
        /// <param name="c">Switch case to render.</param>
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

        /// <summary>
        /// Renders a default switch case.
        /// </summary>
        /// <param name="d">Default case to render.</param>
        public void VisitDefault(AbsynDefault d)
        {
            InnerFormatter.Indentation -= InnerFormatter.TabSize;
            InnerFormatter.Indent();
            InnerFormatter.WriteKeyword("default");
            InnerFormatter.Terminate(":");
            InnerFormatter.Indentation += InnerFormatter.TabSize;
        }
		
        /// <summary>
        /// Renders a continue statement.
        /// </summary>
        /// <param name="cont">Continue statement to render.</param>
        public void VisitContinue(AbsynContinue cont)
		{
            InnerFormatter.Indent();
			InnerFormatter.WriteKeyword("continue");
            InnerFormatter.Terminate(";");
		}

        /// <summary>
        /// Renderes a declaration statement.
        /// </summary>
        /// <param name="decl">Declaration statement to render.</param>
		public void VisitDeclaration(AbsynDeclaration decl)
		{
			InnerFormatter.Indent();
			if (decl.Identifier.DataType is not null)
			{
                var tf = new TypeReferenceFormatter(InnerFormatter);
                tf.WriteDeclaration(decl.Identifier.DataType, decl.Identifier.Name);
			}
			else
			{
                InnerFormatter.Write("?unknown?");
                InnerFormatter.Write(" ");
                decl.Identifier.Accept(this);
            }
			if (decl.Expression is not null)
			{
				InnerFormatter.Write(" = ");
				decl.Expression.Accept(this);
			}
			InnerFormatter.Terminate(";");
		}

        /// <summary>
        /// Renders a do-while loop.
        /// </summary>
        /// <param name="loop">The do-while loop to render.</param>
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

        /// <summary>
        /// Renders a for loop.
        /// </summary>
        /// <param name="forLoop">The for loop to render.</param>
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
            if (ass is not null)
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

        /// <summary>
        /// Renders a goto statement.
        /// </summary>
        /// <param name="g">Goto statement to render.</param>
        public void VisitGoto(AbsynGoto g)
		{
			InnerFormatter.Indent();
			InnerFormatter.WriteKeyword("goto");
            InnerFormatter.Write(" ");
			InnerFormatter.Write(g.Label);
			InnerFormatter.Terminate(";");
		}

        /// <summary>
        /// Renders an if statement
        /// </summary>
        /// <param name="ifs">If statement to render.</param>
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

			if (ifs.Else is not null && ifs.Else.Count > 0)
			{
				InnerFormatter.Indent();
				InnerFormatter.WriteKeyword("else");
                if (IsSingleIfStatement(ifs.Else, out AbsynIf? elseIf))
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

        /// <summary>
        /// Formats an unsigned value.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
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

        private (string,string) FormatStrings(DataType type, object value)
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
                return ("{0:X}", "<p{0}>");
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
                    if (value is not ulong w)
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

        private static bool IsSingleIfStatement(List<AbsynStatement> stms, [MaybeNullWhen(false)] out AbsynIf elseIf)
        {
            elseIf = default!;
            if (stms.Count != 1)
                return false;
            elseIf = stms[0] as AbsynIf;
            return elseIf is not null;
        }

        /// <summary>
        /// Renders a label.
        /// </summary>
        /// <param name="lbl">Label to render.</param>
		public void VisitLabel(AbsynLabel lbl)
		{
			InnerFormatter.Write(lbl.Name);
			InnerFormatter.Terminate(":");
		}

        /// <summary>
        /// Renders a line comment.
        /// </summary>
        /// <param name="comment">Line comment to render.</param>
        public void VisitLineComment(AbsynLineComment comment)
        {
            foreach (var line in Lines(comment.Comment))
            {
                InnerFormatter.Indent();
                InnerFormatter.WriteComment($"// {line}");
                InnerFormatter.Terminate();
            }
        }

        /// <summary>
        /// Renders a return statement.
        /// </summary>
        /// <param name="ret">Return statement to render.</param>
		public void VisitReturn(AbsynReturn ret)
		{
			InnerFormatter.Indent();
			InnerFormatter.WriteKeyword("return");
			if (ret.Value is not null)
			{
				InnerFormatter.Write(" ");
				WriteExpression(ret.Value);
			}
			InnerFormatter.Terminate(";");
		}

        /// <summary>
        /// Renders a side effect statement.
        /// </summary>
        /// <param name="side">Side effect statement to render.</param>
		public void VisitSideEffect(AbsynSideEffect side)
		{
			InnerFormatter.Indent();
			side.Expression.Accept(this);
			InnerFormatter.Terminate(";");
		}

        /// <summary>
        /// Renders a switch statement.
        /// </summary>
        /// <param name="s">The switch statment to render.</param>
        public void VisitSwitch(AbsynSwitch s)
        {
            InnerFormatter.Indent();
            InnerFormatter.WriteKeyword("switch");
            InnerFormatter.Write(" (");
            WriteExpression(s.Expression);
            InnerFormatter.Terminate(")");
            WriteIndentedStatements(s.Statements, false);
        }

        /// <summary>
        /// Renders a while statement.
        /// </summary>
        /// <param name="loop">The while statment to render.</param>
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

        /// <summary>
        /// Renders a <see cref="Procedure"/> to the output.
        /// </summary>
        /// <param name="proc">Procedure to render.</param>
		public void Write(Procedure proc)
		{
			proc.Signature.Emit(proc.QualifiedName(), FunctionType.EmitFlags.None, InnerFormatter, this, new TypeReferenceFormatter(InnerFormatter));
			InnerFormatter.WriteLine();
			InnerFormatter.Write("{");
            InnerFormatter.WriteLine();
			if (proc.Body is not null)
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
            if (expr is null)
            {
                InnerFormatter.Write("<NULL>");
                return;
            }
			int prec = precedenceCur;
			precedenceCur = PrecedenceLeast;
			expr.Accept(this);
			precedenceCur = prec;
		}


        /// <summary>
        /// Writes a formal argument to the output.
        /// </summary>
        /// <param name="arg">Argument to write.</param>
        /// <param name="writeStorage">If true, writes the storage type also.</param>
        /// <param name="isOutParameter">If true, the argument is an out parameter.</param>
        /// <param name="t"><see cref="TypeReferenceFormatter"/> used to render types.</param>
        public void WriteFormalArgument(
            Identifier arg,
            bool writeStorage,
            bool isOutParameter,
            TypeReferenceFormatter t)
        {
            if (writeStorage)
            {
                WriteFormalArgumentType(arg, writeStorage, isOutParameter);
                InnerFormatter.Write(" ");
                InnerFormatter.Write(arg.Name);
            }
            else
            {
                if (isOutParameter)
                {
                    t.WriteDeclaration(new ReferenceTo(arg.DataType), arg.Name);
                }
                else
                {
                    t.WriteDeclaration(arg.DataType, arg.Name);
                }
            }
        }

        /// <summary>
        /// Write the type of a formal argument.
        /// </summary>
        /// <param name="arg">The formal argument.</param>
        /// <param name="writeStorage">If true, writes the <see cref="Storage"/> of the
        /// identifier as well.
        /// </param>
        /// <param name="isOutParameter">If true the argument is an out parameter.</param>
        public void WriteFormalArgumentType(Identifier arg, bool writeStorage, bool isOutParameter)
        {
            if (writeStorage && arg.Storage is not null)
            {
                InnerFormatter.Write(arg.Storage.Kind);
                if (isOutParameter)
                {
                    InnerFormatter.Write(" out ");
                }
                else
                {
                    InnerFormatter.Write(" ");
                }
            }
            typeWriter.WriteReference(arg.DataType);
        }

		private void WriteIndentedStatement(AbsynStatement stm)
		{
			InnerFormatter.Indentation += InnerFormatter.TabSize;
			if (stm is not null)
				stm.Accept(this);
			else
			{
				InnerFormatter.Indent();				
				InnerFormatter.Terminate(";");
			}
			InnerFormatter.Indentation -= InnerFormatter.TabSize;
		}

        private void WriteIndentedStatements(List<AbsynStatement> stms, bool suppressNewline)
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

        /// <summary>
        /// Writes a list of statements to the output.
        /// </summary>
        /// <param name="list">List of statements to be written.
        /// </param>
        public void WriteStatementList(List<AbsynStatement> list)
		{
			foreach (AbsynStatement s in list)
			{
				s.Accept(this);
			}
		}

        /// <summary>
        /// Writes "null" as a keyword.
        /// </summary>
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
