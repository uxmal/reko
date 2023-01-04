#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
            { PrimitiveType.Word512, "w512" },

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


        public static readonly Dictionary<OperatorType, string> OpNames = new()
        {
            { OperatorType.AddrOf, "addr" },
            { OperatorType.And, "&" },
            { OperatorType.Cand, "&&" },
            { OperatorType.Comma, "," },
            { OperatorType.Comp, "~" },
            { OperatorType.Cor, "||" },
            { OperatorType.Eq, "==" },
            { OperatorType.FAdd, "+f" },
            { OperatorType.FDiv, "/f" },
            { OperatorType.Feq, "==f" },
            { OperatorType.Fge, ">=f" },
            { OperatorType.Fgt, ">f" },
            { OperatorType.Fle, "<=f" },
            { OperatorType.Flt, "<f" },
            { OperatorType.Fne, "!=f" },
            { OperatorType.FMul, "*f" },
            { OperatorType.FSub, "-f" },
            { OperatorType.Ge, ">=" },
            { OperatorType.IAdd, "+" },
            { OperatorType.IMod, "%" },
            { OperatorType.IMul, "*" },
            { OperatorType.ISub, "-" },
            { OperatorType.Le, "<=" },
            { OperatorType.Lt, "<" },
            { OperatorType.Ne, "!=" },
            { OperatorType.Neg, "neg" },
            { OperatorType.Not, "!" },
            { OperatorType.Or, "|" },
            { OperatorType.Sar, ">>s" },
            { OperatorType.SDiv, "/s" },
            { OperatorType.Shl, "<<" },
            { OperatorType.Shr, ">>u" },
            { OperatorType.SMod, "%s" },
            { OperatorType.SMul, "*s" },
            { OperatorType.UDiv, "/u" },
            { OperatorType.Uge, ">=u" },
            { OperatorType.Ugt, ">u" },
            { OperatorType.Ule, "<=u" },
            { OperatorType.Ult, "<u" },
            { OperatorType.UMod, "%u" },
            { OperatorType.UMul, "*u" },
            { OperatorType.USub, "-u" },
            { OperatorType.Xor, "^" },
        };

        public static readonly Dictionary<string, OperatorType> OpsByName = OpNames.ToDictionary(k => k.Value, v => v.Key);

    }
}
