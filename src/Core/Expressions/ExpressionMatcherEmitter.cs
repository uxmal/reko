using Reko.Core.Types;

namespace Reko.Core.Expressions
{
    public class ExpressionMatcherEmitter : ExpressionEmitter
    {
        public Identifier AnyId(string? label = null)
        {
            return ExpressionMatcher.AnyId(label);
        }

        public Expression AnyConst(string? label = null)
        {
            return ExpressionMatcher.AnyConstant(label);
        }

        public DataType AnyDataType(string? label)
        {
            return ExpressionMatcher.AnyDataType(label);
        }

        public Expression AnyExpr(string? label = null)
        {
            return ExpressionMatcher.AnyExpression(label);
        }
    }
}