using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.LLVM
{
    public partial class LLVMParser
    {
        private Value ParseGetElementPtrExpr()
        {
            Expect(TokenType.getelementptr);
            bool inbounds = false;
            if (PeekAndDiscard(TokenType.inbounds))
            {
                inbounds = true;
            }
            Expect(TokenType.LPAREN);
            var baseType = ParseType();
            Expect(TokenType.COMMA);
            var ptrType = ParseType();
            var ptr = ParseValue();
            var indices = new List<Tuple<LLVMType, Value>>();
            while (PeekAndDiscard(TokenType.COMMA))
            {
                var type = ParseType();
                var val = ParseValue();
                indices.Add(Tuple.Create(type, val));
            }
            Expect(TokenType.RPAREN);
            return new GetElementPtrExpr
            {
                Inbounds = inbounds,
                BaseType = baseType,
                PointerType = ptrType,
                Pointer = ptr,
                Indices = indices,
            };
        }
    }
}
