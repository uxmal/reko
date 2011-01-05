using Decompiler.Core.Expressions;
using Decompiler.Core.Operators;
 
namespace Decompiler.Analysis.Simplification
{
    public class Sub_Xor_Zero_Rule
    {
        private BinaryExpression binExp;

        public bool Match(BinaryExpression binExp)
        {
            if (binExp.op != Operator.Sub && binExp.op != Operator.Xor)
                return false;
            this.binExp = binExp;
            return (binExp.Left is Identifier) && binExp.Left == binExp.Right;
        }

        public Expression Transform()
        {
            return Constant.Zero(binExp.DataType);
        }
    }
}