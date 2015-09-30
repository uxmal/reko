using System;
using Reko.Analysis;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Code;
using Reko.Core;

namespace Reko.UnitTests.Evaluation
{
    internal class ConstDivisionImplementedByMultiplication
    {
        private SsaEvaluationContext ctx;
        private int divisor;
        private Identifier idDst;
        private Expression lhs;

        public ConstDivisionImplementedByMultiplication(SsaEvaluationContext ctx)
        {
            this.ctx = ctx;
        }

        public bool Match(Assignment ass)
        {
            var dst = ass.Dst.Storage as SequenceStorage;
            if (dst == null)
                return false;
            this.idDst = dst.Tail;

            BinaryExpression bin;
            Constant cRight;
            if (!ass.Dst.As(out bin) ||
                bin.Operator != Operator.UMul |
                !bin.Right.As(out cRight))
                return false;
            if (bin.DataType.Size <= bin.Left.DataType.Size)
                return false;

            double q = (1L << 32) / (double)cRight.ToUInt32();
            var rem = Math.Floor(q + 0.5);
            if (Math.Abs(q - rem) >= 0.0001)        // not very close to integer
                return false;
            this.lhs = bin.Left;
            this.divisor = (int)rem;
            return true;
        }

        public Assignment Transform()
        {
            throw new NotImplementedException();
        }
    }
}