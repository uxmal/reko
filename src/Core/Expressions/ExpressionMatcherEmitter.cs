using Reko.Core.Types;

namespace Reko.Core.Expressions
{
    /// <summary>
    /// Provides methods for creating expression matchers.
    /// </summary>
    public class ExpressionMatcherEmitter : ExpressionEmitter
    {
        /// <summary>
        /// Matches any identifier.
        /// </summary>
        /// <param name="label">Optional label for this match.</param>
        public Identifier AnyId(string? label = null)
        {
            return ExpressionMatcher.AnyId(label);
        }

        /// <summary>
        /// Matches any binary expression.
        /// </summary>
        /// <param name="label">Label for the matched binary operator.</param>
        /// <param name="left">Left expression to match.</param>
        /// <param name="right">Right expression to match.</param>
        public BinaryExpression AnyBinary(string label, Expression left, Expression right)
        {
            return base.Bin( ExpressionMatcher.AnyBinaryOperator(label), left, right);
        }

        /// <summary>
        /// Matches any constant.
        /// </summary>
        /// <param name="label">Optional label for this match.</param>
        public Expression AnyConst(string? label = null)
        {
            return ExpressionMatcher.AnyConstant(label);
        }

        /// <summary>
        /// Matches any data type.
        /// </summary>
        /// <param name="label">Optional label for this match.</param>
        public DataType AnyDataType(string? label)
        {
            return ExpressionMatcher.AnyDataType(label);
        }

        /// <summary>
        /// Matches any expression.
        /// </summary>
        /// <param name="label">Optional label for this match.</param>
        public Expression AnyExpr(string? label = null)
        {
            return ExpressionMatcher.AnyExpression(label);
        }
    }
}