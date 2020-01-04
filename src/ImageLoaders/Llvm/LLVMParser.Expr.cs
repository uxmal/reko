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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.LLVM
{
    public partial class LLVMParser
    {
        private ConversionExpr ParseConversionExpr()
        {
            var op = Get().Type;
            Expect(TokenType.LPAREN);
            var value = ParseTypedValue();
            Expect(TokenType.to);
            var type = ParseType();
            Expect(TokenType.RPAREN);
            return new ConversionExpr
            {
                Operator = op,
                Value = value,
                Type = type,
            };
        }

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
