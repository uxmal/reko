#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using System.Text;

namespace Reko.Core.Expressions
{
    /// <summary>
    /// Used to match expressions to a pattern and possibly capture 
    /// identifiers and constants.
    /// </summary>
    public class ExpressionMatcher : ExpressionVisitor<bool>
    {
        private Expression p;
        private Dictionary<string, Expression> capturedExpressions;
        private Dictionary<string, Operator> capturedOperators;

        public ExpressionMatcher(Expression pattern)
        {
            this.Pattern = pattern;
            this.capturedExpressions = new Dictionary<string, Expression>();
            this.capturedOperators = new Dictionary<string, Operator>();
        }

        public Expression CapturedExpression(string label)
        {
            Expression value;
            if (string.IsNullOrEmpty(label) || !capturedExpressions.TryGetValue(label, out value))
                return null;
            return value;
        }

        public Operator CapturedOperators(string label)
        {
            Operator value;
            if (string.IsNullOrEmpty(label) || !capturedOperators.TryGetValue(label, out value))
                return null;
            return value;
        }

        public void Clear()
        {
            this.capturedExpressions.Clear();
            this.capturedOperators.Clear();
        }

        public bool Match(Expression expr)
        {
            return Match(Pattern, expr);
        }

        private bool Match(Expression pattern, Expression expr)
        {
            this.p = pattern;
            var w = pattern as WildExpression;
            if (w != null)
            {
                capturedExpressions[w.Label] = expr;
                return true;
            }
            return expr.Accept(this);
        }

        private bool Match(Operator opPattern, Operator op)
        {
            var wildOp = opPattern as WildOperator;
            if (wildOp != null)
            {
                capturedOperators[wildOp.Label] = op;
                return true;
            }
            return opPattern == op;
        }

        #region ExpressionVisitor<bool> Members

        bool ExpressionVisitor<bool>.VisitAddress(Reko.Core.Address addr)
        {
            var anyC = p as WildConstant;
            if (anyC != null)
            {
                if (!string.IsNullOrEmpty(anyC.Label))
                    capturedExpressions[anyC.Label] = addr;
                return true;
            }
            var cP = p as Address;
            if (cP == null)
                return false;
            return addr.ToLinear() == cP.ToLinear();
        }

        bool ExpressionVisitor<bool>.VisitApplication(Application appl)
        {
            var appP = p as Application;
            if (appP == null)
                return false;
            if (!Match(appP.Procedure, appl.Procedure))
                return false;
            if (appP.Arguments.Length != appl.Arguments.Length)
                return false;
            for (int i =0; i < appP.Arguments.Length; ++i)
            {
                if (!Match(appP.Arguments[i], appl.Arguments[i]))
                    return false;
            }
            return true;
        }

        bool ExpressionVisitor<bool>.VisitArrayAccess(ArrayAccess acc)
        {
            var arrayP = p as ArrayAccess;
            if (arrayP == null)
                return false;
            throw new NotImplementedException();
        }

        bool ExpressionVisitor<bool>.VisitBinaryExpression(BinaryExpression binExp)
        {
            var bP = p as BinaryExpression;
            if (bP == null)
                return false;
            if (!Match(bP.Operator, binExp.Operator))
                return false;

            return (Match(bP.Left, binExp.Left) && Match(bP.Right, binExp.Right));
        }

        bool ExpressionVisitor<bool>.VisitCast(Cast cast)
        {
            var castP = p as Cast;
            return 
                castP != null &&
                Match(castP.Expression, cast.Expression);
        }

        bool ExpressionVisitor<bool>.VisitConditionalExpression(ConditionalExpression cond)
        {
            var condP = p as ConditionalExpression;
            if (condP == null)
                return false;
            if (!Match(condP.Condition, cond.Condition))
                return false;
            if (!Match(condP.ThenExp, cond.ThenExp))
                return false;
            return Match(condP.FalseExp, cond.FalseExp);
        }

        bool ExpressionVisitor<bool>.VisitConditionOf(ConditionOf cof)
        {
            var condP = p as ConditionOf;
            return
                condP != null &&
                Match(condP.Expression, cof.Expression);
        }

        bool ExpressionVisitor<bool>.VisitConstant(Constant c)
        {
            var anyC = p as WildConstant;
            if (anyC != null)
            {
                if (!string.IsNullOrEmpty(anyC.Label))
                    capturedExpressions[anyC.Label] = c;
                return true;
            }
            var cP = p as Constant;
            if (cP == null)
                return false;
            return (c.ToInt64() == cP.ToInt64());
        }

        bool ExpressionVisitor<bool>.VisitDepositBits(DepositBits d)
        {
            var dP = p as DepositBits;
            if (dP == null)
                return false;
            if (!Match(dP.Source, d.Source))
                return false;
            if (!Match(dP.InsertedBits, d.InsertedBits))
                return false;
            return (dP.BitPosition == d.BitPosition);
        }

        bool ExpressionVisitor<bool>.VisitDereference(Dereference deref)
        {
            throw new NotImplementedException();
        }

        bool ExpressionVisitor<bool>.VisitFieldAccess(FieldAccess acc)
        {
            throw new NotImplementedException();
        }

        bool ExpressionVisitor<bool>.VisitIdentifier(Identifier id)
        {
            var anyId = p as WildId;
            if (anyId != null)
            {
                if (!string.IsNullOrEmpty(anyId.Label))
                    capturedExpressions.Add(anyId.Label, id);
                return true;
            }
            var idP = p as Identifier;
            if (idP == null)
                return false;
            return (id.Name == idP.Name);
        }

        bool ExpressionVisitor<bool>.VisitMemberPointerSelector(MemberPointerSelector mps)
        {
            throw new NotImplementedException();
        }

        bool ExpressionVisitor<bool>.VisitMemoryAccess(MemoryAccess access)
        {
            var mp = p as MemoryAccess;
            if (mp == null)
                return false;
            if (mp.DataType is WildDataType)
            {
            }
            else if (mp.DataType.Size != access.DataType.Size)
                return false;
            return Match(mp.EffectiveAddress, access.EffectiveAddress);
        }

        bool ExpressionVisitor<bool>.VisitMkSequence(MkSequence seq)
        {
            var m = p as MkSequence;
            if (m == null)
                return false;
            if (seq.Expressions.Length != m.Expressions.Length)
                return false;
            for (int i =0; i < seq.Expressions.Length; ++i)
            {
                if (!Match(m.Expressions[i], seq.Expressions[i]))
                    return false;
            }
            return true;
        }

        bool ExpressionVisitor<bool>.VisitOutArgument(OutArgument outArg)
        {
            var op = p as OutArgument;
            if (outArg.DataType.Size != op.DataType.Size)
                return false;
            return Match(op.Expression, outArg.Expression);
        }
        bool ExpressionVisitor<bool>.VisitPhiFunction(PhiFunction phi)
        {
            throw new NotImplementedException();
        }

        bool ExpressionVisitor<bool>.VisitPointerAddition(PointerAddition pa)
        {
            throw new NotImplementedException();
        }

        bool ExpressionVisitor<bool>.VisitProcedureConstant(ProcedureConstant pc)
        {
            var pcOther = p as ProcedureConstant;
            if (pcOther == null)
                return false;
            return pcOther.Procedure == pc.Procedure;
        }

        bool ExpressionVisitor<bool>.VisitScopeResolution(ScopeResolution scopeResolution)
        {
            throw new NotImplementedException();
        }

        bool ExpressionVisitor<bool>.VisitSegmentedAccess(SegmentedAccess access)
        {
            var smp = p as SegmentedAccess;
            if (smp == null)
                return false;
            if (smp.DataType is WildDataType)
            {
            }
            else if (smp.DataType.Size != access.DataType.Size)
                return false;

            return
                Match(smp.BasePointer, access.BasePointer) &&
                Match(smp.EffectiveAddress, access.EffectiveAddress);
        }

        bool ExpressionVisitor<bool>.VisitSlice(Slice slice)
        {
            var slicePat = p as Slice;
            if (slicePat == null)
                return false;
            throw new NotImplementedException();
        }

        bool ExpressionVisitor<bool>.VisitTestCondition(TestCondition tc)
        {
            var tp = p as TestCondition;
            if (tp == null)
                return false;
            return tp.ConditionCode == tc.ConditionCode &&
                Match(tp.Expression, tc.Expression);
        }

        bool ExpressionVisitor<bool>.VisitUnaryExpression(UnaryExpression unary)
        {
            var unaryPat = p as UnaryExpression;
            if (unaryPat == null)
                return false;
            if (!Match(unaryPat.Operator, unary.Operator))
                return false;

            return Match(unaryPat.Expression, unary.Expression);
        }

        #endregion

        public static Expression AnyConstant()
        {
            return new WildConstant(null);
        }

        public static Expression AnyConstant(string label)
        {
            return new WildConstant(label);
        }

        public static Expression AnyExpression(string label)
        {
            return new WildExpression(label);
        }

        public static Identifier AnyId(string label = null)
        {
            return new WildId(label);
        }

        public static Operator AnyOperator(string label)
        {
            return new WildOperator(label);
        }

        public static DataType AnyDataType(string label)
        {
            return new WildDataType(label);
        }

        private interface IWildExpression
        {
            string Label { get; }
        }

        private class WildConstant : Constant, IWildExpression
        {
            public WildConstant(string label) : base(PrimitiveType.UInt32)
            {
                this.Label = label;
            }

            public override IEnumerable<Expression> Children
            {
                get { yield break; }
            }

            public string Label { get; private set; }

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
                throw new InvalidOperationException();
            }

            public override int GetHashOfValue()
            {
                throw new InvalidOperationException();
            }

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
        }

        private class WildExpression : Expression, IWildExpression
        {
            public WildExpression(string label)
                : base(VoidType.Instance)
            {
                this.Label = label;
            }

            public string Label { get; private set; }

            public override IEnumerable<Expression> Children
            {
                get { yield break; }
            }

            public override T Accept<T, C>(ExpressionVisitor<T, C> v, C context)
            {
                throw new NotImplementedException();
            }

            public override T Accept<T>(ExpressionVisitor<T> visitor)
            {
                throw new NotImplementedException();
            }

            public override void Accept(IExpressionVisitor visit)
            {
                throw new NotImplementedException();
            }

            public override Expression CloneExpression()
            {
                throw new NotImplementedException();
            }
        }

        private class WildId : Identifier
        {
            public WildId(string label) : base(label, VoidType.Instance, null)
            {
                this.Label = label;
            }

            public string Label { get; private set; }
        }

        private class WildOperator : Operator
        {
            public WildOperator(string Label)
            {
                this.Label = Label;
            }

            public string Label { get; private set; }

            public override string ToString()
            {
                return string.Format("[{0}]", Label);
            }
        }

        private class WildDataType : DataType
        {
            public WildDataType(string label)
            {
                this.Label = label;
            }

            public override int Size { get; set; }

            public string Label { get; private set; }

            public override T Accept<T>(IDataTypeVisitor<T> v)
            {
                throw new NotImplementedException();
            }

            public override void Accept(IDataTypeVisitor v)
            {
                throw new NotImplementedException();
            }

            public override DataType Clone(IDictionary<DataType, DataType> clonedTypes)
            {
                throw new NotImplementedException();
            }
        }

        public Expression Pattern { get; set; }
    }
}
