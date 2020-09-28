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

using Reko.Core.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.LLVM
{
    public class ConversionExpr : Value
    {
        public TokenType Operator;
        public TypedValue Value;
        public LLVMType Type;

        public override void Write(Formatter w)
        {
            w.WriteKeyword(Operator.ToString());
            w.Write(" (");
            Value.Write(w);
            w.Write(' ');
            w.WriteKeyword("to");
            w.Write(' ');
            Type.Write(w);
            w.Write(")");
        }
    }

    public class GetElementPtrExpr : Value
    {
        public bool Inbounds;
        public LLVMType BaseType;
        public LLVMType PointerType;
        public Value Pointer;
        public List<Tuple<LLVMType, Value>> Indices;

        public override void Write(Formatter w)
        {
            w.WriteKeyword("getelementptr");
            if (Inbounds)
            {
                w.Write(' ');
                w.WriteKeyword("inbounds");
            }
            w.Write(" (");
            BaseType.Write(w);
            w.Write(", ");
            PointerType.Write(w);
            w.Write(' ');
            Pointer.Write(w);
            foreach (var index in Indices)
            {
                w.Write(", ");
                index.Item1.Write(w);
                w.Write(' ');
                index.Item2.Write(w);
            }
            w.Write(")");
        }
    }
}
