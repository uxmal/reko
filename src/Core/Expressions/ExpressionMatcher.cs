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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

namespace Reko.Core.Expressions
{
    /// <summary>
    /// Used to match expressions to a pattern and possibly capture 
    /// identifiers and constants.
    /// </summary>
    public class ExpressionMatcher : ExpressionVisitor<bool, ExpressionMatch>
    {
        private Expression pattern;

        /// <summary>
        /// Constructs an expression matcher.
        /// </summary>
        /// <param name="pattern">Pattern to match.</param>
        public ExpressionMatcher(Expression pattern)
        {
            this.pattern = pattern;
        }


        /// <summary>
        /// Builds a matcher from the given builder function.
        /// </summary>
        /// <param name="builder">Delegate that builds a matcher</param>
        /// <returns>Resulting <see cref="ExpressionMatcher"/>.</returns>
        public static ExpressionMatcher Build(Func<ExpressionMatcherEmitter, Expression> builder)
        {
            var pattern = builder(new ExpressionMatcherEmitter());
            return new ExpressionMatcher(pattern);
        }

        /// <summary>
        /// Matches the given expression to the pattern.
        /// </summary>
        /// <param name="expr">Expression to match.</param>
        /// <returns>An <see cref="ExpressionMatch"/> instance.</returns>
        public ExpressionMatch Match(Expression expr)
        {
            var m = new ExpressionMatch();
            m.Success = Match(pattern, expr, m);
            return m;
        }

        internal bool Match(Expression pattern, Expression expr, ExpressionMatch m)
        {
            if (pattern is WildExpression w)
            {
                if (w.Label is { })
                    m.Capture(w.Label, expr);
                return true;
            }
            m.Pattern = pattern;
            m.Success = expr.Accept(this, m);
            return m.Success;
        }

        private bool Match(Operator opPattern, Operator op, ExpressionMatch m)
        {
            if (opPattern is WildBinaryOperator wildOp)
            {
                if (wildOp.Label is { })
                    m.Capture(wildOp.Label, op);
                return true;
            }
            return opPattern == op;
        }

        #region ExpressionVisitor<bool> Members

        bool ExpressionVisitor<bool, ExpressionMatch>.VisitAddress(Address addr, ExpressionMatch m)
        {
            if (m.Pattern is WildConstant anyC)
            {
                if (!string.IsNullOrEmpty(anyC.Label))
                    m.Capture(anyC.Label!, addr);
                return true;
            }
            return m.Pattern is Address addrP && addr.ToLinear() == addrP.ToLinear();
        }

        bool ExpressionVisitor<bool, ExpressionMatch>.VisitApplication(Application appl, ExpressionMatch m)
        {
            if (m.Pattern is not Application appP)
                return false;
            if (!Match(appP.Procedure, appl.Procedure, m))
                return false;
            if (appP.Arguments.Length != appl.Arguments.Length)
                return false;
            for (int i =0; i < appP.Arguments.Length; ++i)
            {
                if (!Match(appP.Arguments[i], appl.Arguments[i], m))
                    return false;
            }
            return true;
        }

        bool ExpressionVisitor<bool, ExpressionMatch>.VisitArrayAccess(ArrayAccess acc, ExpressionMatch m)
        {
            if (m.Pattern is not ArrayAccess)
                return false;
            throw new NotImplementedException();
        }

        bool ExpressionVisitor<bool, ExpressionMatch>.VisitBinaryExpression(BinaryExpression binExp, ExpressionMatch m)
        {
            if (m.Pattern is not BinaryExpression bP)
                return false;
            if (!Match(bP.Operator, binExp.Operator, m))
                return false;

            return (Match(bP.Left, binExp.Left, m) && Match(bP.Right, binExp.Right, m));
        }

        bool ExpressionVisitor<bool, ExpressionMatch>.VisitCast(Cast cast, ExpressionMatch m)
        {
            return
                m.Pattern is Cast castP &&
                Match(castP.Expression, cast.Expression, m);
        }

        bool ExpressionVisitor<bool, ExpressionMatch>.VisitConditionalExpression(ConditionalExpression cond, ExpressionMatch m)
        {
            if (m.Pattern is not ConditionalExpression condP)
                return false;
            if (!Match(condP.Condition, cond.Condition, m))
                return false;
            if (!Match(condP.ThenExp, cond.ThenExp, m))
                return false;
            return Match(condP.FalseExp, cond.FalseExp, m);
        }

        bool ExpressionVisitor<bool, ExpressionMatch>.VisitConditionOf(ConditionOf cof, ExpressionMatch m)
        {
            return
                m.Pattern is ConditionOf condP &&
                Match(condP.Expression, cof.Expression, m);
        }

        bool ExpressionVisitor<bool, ExpressionMatch>.VisitConstant(Constant c, ExpressionMatch m)
        {
            if (m.Pattern is WildConstant anyC)
            {
                if (!string.IsNullOrEmpty(anyC.Label))
                    m.Capture(anyC.Label, c);
                return true;
            }
            if (m.Pattern is not Constant cP)
                return false;
            return (c.ToInt64() == cP.ToInt64());
        }

        bool ExpressionVisitor<bool, ExpressionMatch>.VisitConversion(Conversion conversion, ExpressionMatch m)
        {
            return
                m.Pattern is Conversion convP &&
                Match(convP.Expression, conversion.Expression, m);
        }

        bool ExpressionVisitor<bool, ExpressionMatch>.VisitDereference(Dereference deref, ExpressionMatch m)
        {
            throw new NotImplementedException();
        }

        bool ExpressionVisitor<bool, ExpressionMatch>.VisitFieldAccess(FieldAccess acc, ExpressionMatch m)
        {
            throw new NotImplementedException();
        }

        bool ExpressionVisitor<bool, ExpressionMatch>.VisitIdentifier(Identifier id, ExpressionMatch m)
        {
            if (m.Pattern is WildId anyId)
            {
                return m.Capture(anyId.Label!, id);
            }
            if (m.Pattern is not Identifier idP)
                return false;
            return (id.Name == idP.Name);
        }

        bool ExpressionVisitor<bool, ExpressionMatch>.VisitMemberPointerSelector(MemberPointerSelector mps, ExpressionMatch m)
        {
            throw new NotImplementedException();
        }

        bool ExpressionVisitor<bool, ExpressionMatch>.VisitMemoryAccess(MemoryAccess access, ExpressionMatch m)
        {
            if (m.Pattern is not MemoryAccess mp)
                return false;
            if (mp.DataType is not WildDataType &&
                mp.DataType.BitSize != access.DataType.BitSize)
                return false;
            return Match(mp.EffectiveAddress, access.EffectiveAddress, m);
        }

        bool ExpressionVisitor<bool, ExpressionMatch>.VisitMkSequence(MkSequence seq, ExpressionMatch m)
        {
            if (m.Pattern is not MkSequence mk)
                return false;
            if (seq.Expressions.Length != mk.Expressions.Length)
                return false;
            for (int i = 0; i < seq.Expressions.Length; ++i)
            {
                if (!Match(mk.Expressions[i], seq.Expressions[i], m))
                    return false;
            }
            return true;
        }

        bool ExpressionVisitor<bool, ExpressionMatch>.VisitOutArgument(OutArgument outArg, ExpressionMatch m)
        {
            if (m.Pattern is not OutArgument op || outArg.DataType.BitSize != op.DataType.BitSize)
                return false;
            return Match(op.Expression, outArg.Expression, m);
        }

        bool ExpressionVisitor<bool, ExpressionMatch>.VisitPhiFunction(PhiFunction phi, ExpressionMatch m)
        {
            throw new NotImplementedException();
        }

        bool ExpressionVisitor<bool, ExpressionMatch>.VisitPointerAddition(PointerAddition pa, ExpressionMatch m)
        {
            throw new NotImplementedException();
        }

        bool ExpressionVisitor<bool, ExpressionMatch>.VisitProcedureConstant(ProcedureConstant pc, ExpressionMatch m)
        {
            if (m.Pattern is not ProcedureConstant pcOther)
                return false;
            return pcOther.Procedure == pc.Procedure;
        }

        bool ExpressionVisitor<bool, ExpressionMatch>.VisitScopeResolution(ScopeResolution scopeResolution, ExpressionMatch m)
        {
            throw new NotImplementedException();
        }

        bool ExpressionVisitor<bool, ExpressionMatch>.VisitSegmentedAddress(SegmentedPointer address, ExpressionMatch m)
        {
            if (m.Pattern is not SegmentedPointer msa)
                return false;
            if (msa.DataType is not WildDataType &&
                msa.DataType.BitSize != address.DataType.BitSize)
                return false;

            return
                Match(msa.BasePointer , address.BasePointer, m) &&
                Match(msa.Offset, address.Offset, m);
        }

        bool ExpressionVisitor<bool, ExpressionMatch>.VisitSlice(Slice slice, ExpressionMatch m)
        {
            if (m.Pattern is not Slice)
                return false;
            throw new NotImplementedException();
        }

        bool ExpressionVisitor<bool, ExpressionMatch>.VisitStringConstant(StringConstant s, ExpressionMatch m)
        {
            if (m.Pattern is WildConstant anyC)
            {
                if (!string.IsNullOrEmpty(anyC.Label))
                    m.Capture(anyC.Label, s);
                return true;
            }
            if (m.Pattern is not StringConstant sP)
                return false;
            return (s.ToString() == sP.ToString());
        }

        bool ExpressionVisitor<bool, ExpressionMatch>.VisitTestCondition(TestCondition tc, ExpressionMatch m)
        {
            if (m.Pattern is not TestCondition tp)
                return false;
            return tp.ConditionCode == tc.ConditionCode &&
                Match(tp.Expression, tc.Expression, m);
        }

        bool ExpressionVisitor<bool, ExpressionMatch>.VisitUnaryExpression(UnaryExpression unary, ExpressionMatch m)
        {
            if (m.Pattern is not UnaryExpression unaryPat)
                return false;
            if (!Match(unaryPat.Operator, unary.Operator, m))
                return false;

            return Match(unaryPat.Expression, unary.Expression, m);
        }

        #endregion


        /// <summary>
        /// Matches any contant.
        /// </summary>
        /// <param name="label">Optional label for this match.</param>
        public static Expression AnyConstant(string? label)
        {
            return new WildConstant(label);
        }

        /// <summary>
        /// Matches any expression.
        /// </summary>
        /// <param name="label">Optional label for this match.</param>
        public static Expression AnyExpression(string? label)
        {
            return new WildExpression(label);
        }

        /// <summary>
        /// Matches any identifier.
        /// </summary>
        /// <param name="label">Optional label for this match.</param>
        public static Identifier AnyId(string? label = null)
        {
            return new WildId(label);
        }

        /// <summary>
        /// Matches any unary expression.
        /// </summary>
        /// <param name="label">Optional label for this match.</param>
        public static UnaryOperator AnyUnaryOperator(string label)
        {
            return new WildUnaryOperator(label);
        }

        /// <summary>
        /// Matches any binary expression.
        /// </summary>
        /// <param name="label">Optional label for this match.</param>
        public static BinaryOperator AnyBinaryOperator(string label)
        {
            return new WildBinaryOperator(label);
        }

        /// <summary>
        /// Matches any data type.
        /// </summary>
        /// <param name="label">Optional label for this match.</param>
        public static DataType AnyDataType(string? label)
        {
            return new WildDataType(label);
        }

        private interface IWildExpression
        {
            string? Label { get; }
        }

        private class WildConstant : Constant, IWildExpression
        {
            public WildConstant(string? label) : base(PrimitiveType.UInt32)
            {
                this.Label = label;
            }

            public override IEnumerable<Expression> Children
            {
                get { yield break; }
            }

            public string? Label { get; private set; }

            public override Expression CloneExpression()
            {
                return new WildConstant(Label);
            }

            public override Constant Complement()
            {
                throw new InvalidOperationException();
            }

            public override object GetValue()
            {
                return "<wild>";
            }

            public override int GetHashOfValue()
            {
                throw new InvalidOperationException();
            }

            public override bool IsMaxUnsigned => throw new InvalidOperationException();

            public override byte ToByte()
            {
                throw new InvalidOperationException();
            }

            public override ushort ToUInt16()
            {
                throw new InvalidOperationException();
            }

            public override uint ToUInt32()
            {
                throw new InvalidOperationException();
            }

            public override ulong ToUInt64()
            {
                throw new InvalidOperationException();
            }

            public override short ToInt16()
            {
                throw new InvalidOperationException();
            }

            public override int ToInt32()
            {
                throw new InvalidOperationException();
            }

            public override long ToInt64()
            {
                throw new InvalidOperationException();
            }

            public override BigInteger ToBigInteger() => throw new InvalidOperationException();
        }

        private class WildExpression : Expression, IWildExpression
        {
            public WildExpression(string? label)
            {
                this.Label = label;
                this.DataType = VoidType.Instance;
            }

            public DataType DataType { get; set; }

            public bool IsZero => false;
            public string? Label { get; }

            public IEnumerable<Expression> Children => Array.Empty<Expression>();

            public T Accept<T, C>(ExpressionVisitor<T, C> v, C context)
            {
                throw new NotImplementedException();
            }

            public T Accept<T>(ExpressionVisitor<T> visitor)
            {
                throw new NotImplementedException();
            }

            public void Accept(IExpressionVisitor visit)
            {
            }

            public Expression CloneExpression()
            {
                throw new NotImplementedException();
            }

            public Expression Invert()
            {
                throw new NotImplementedException();
            }
        }

        private class WildId : Identifier
        {
            //$TODO: the MemoryStorage.INstance will never be used and is there because
            // there is no `UnknownStorage` data type.
            public WildId(string? label) : base(label ?? "", VoidType.Instance, MemoryStorage.Instance)
            {
                this.Label = label;
            }

            public string? Label { get; }
        }

        private class WildUnaryOperator : UnaryOperator
        {
            public WildUnaryOperator(string Label) : base((OperatorType)(-1))
            {
                this.Label = Label;
            }

            public string? Label { get; }

            public override Constant ApplyConstant(Constant c)
            {
                throw new InvalidOperationException();
            }

            public override string ToString()
            {
                return string.Format("[{0}]", Label);
            }
        }

        private class WildBinaryOperator : BinaryOperator
        {
            public WildBinaryOperator(string Label) : base((OperatorType) (-1))
            {
                this.Label = Label;
            }

            public string? Label { get; }

            public override Constant ApplyConstants(DataType dt, Constant c1, Constant c2)
            {
                throw new InvalidOperationException();
            }

            public override string ToString()
            {
                return string.Format("[{0}]", Label);
            }
        }


        private class WildDataType : DataType
        {
            public WildDataType(string? label)
                : base(Domain.None)
            {
                this.Label = label;
            }

            public override int Size { get; set; }

            public string? Label { get; private set; }

            public override T Accept<T>(IDataTypeVisitor<T> v)
            {
                throw new NotImplementedException();
            }

            public override void Accept(IDataTypeVisitor v)
            {
                throw new NotImplementedException();
            }

            public override DataType Clone(IDictionary<DataType, DataType>? clonedTypes)
            {
                throw new NotImplementedException();
            }
        }
    }
}
