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

using Reko.Core.Operators;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core.Serialization.Json
{
    public class JsonSymbols
    {
        public readonly static Dictionary<DataType, string> PrimitiveNames = new Dictionary<DataType, string>
        {
            { PrimitiveType.Bool, "f" },

            { PrimitiveType.Char, "c8" },
            { PrimitiveType.WChar, "c16" },

            { PrimitiveType.Word16, "w16" },
            { PrimitiveType.Word32, "w32" },
            { PrimitiveType.Word64, "w64" },
            { PrimitiveType.Word128, "w128" },
            { PrimitiveType.Word256, "w256" },

            { PrimitiveType.SByte, "i8" },
            { PrimitiveType.Int16, "i16" },
            { PrimitiveType.Int32, "i32" },
            { PrimitiveType.Int64, "i64" },

            { PrimitiveType.Byte, "u8" },
            { PrimitiveType.UInt16, "u16" },
            { PrimitiveType.UInt32, "u32" },
            { PrimitiveType.UInt64, "u64" },

            { PrimitiveType.Real32, "r32" },
            { PrimitiveType.Real64, "r64" },
        };

        public static readonly Dictionary<string,DataType> PrimitivesByName = PrimitiveNames.ToDictionary(k => k.Value, v => v.Key);


        public static readonly Dictionary<Operator, string> OpNames = new Dictionary<Operator, string>
        {
            { Operator.AddrOf, "addr" },
            { Operator.And, "&" },
            { Operator.Cand, "&&" },
            { Operator.Comma, "," },
            { Operator.Comp, "~" },
            { Operator.Cor, "||" },
            { Operator.Eq, "==" },
            { Operator.FAdd, "+f" },
            { Operator.FDiv, "/f" },
            { Operator.Feq, "==f" },
            { Operator.Fge, ">=f" },
            { Operator.Fgt, ">f" },
            { Operator.Fle, "<=f" },
            { Operator.Flt, "<f" },
            { Operator.Fne, "!=f" },
            { Operator.FMul, "*f" },
            { Operator.FSub, "-f" },
            { Operator.Ge, ">=" },
            { Operator.IAdd, "+" },
            { Operator.IMod, "%" },
            { Operator.IMul, "*" },
            { Operator.ISub, "-" },
            { Operator.Le, "<=" },
            { Operator.Lt, "<" },
            { Operator.Ne, "!=" },
            { Operator.Neg, "neg" },
            { Operator.Not, "!" },
            { Operator.Or, "|" },
            { Operator.Sar, ">>s" },
            { Operator.SDiv, "/s" },
            { Operator.Shl, "<<" },
            { Operator.Shr, ">>u" },
            { Operator.SMul, "*s" },
            { Operator.UDiv, "/u" },
            { Operator.Uge, ">=u" },
            { Operator.Ugt, ">u" },
            { Operator.Ule, "<=u" },
            { Operator.Ult, "<u" },
            { Operator.UMul, "*u" },
            { Operator.USub, "-u" },
            { Operator.Xor, "^" },
        };

        public static readonly Dictionary<string, Operator> OpsByName = OpNames.ToDictionary(k => k.Value, v => v.Key);

    }
}
