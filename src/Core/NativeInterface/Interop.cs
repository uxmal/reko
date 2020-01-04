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

using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core.NativeInterface
{
    public static class Interop
    {
        public static readonly Dictionary<BaseType, DataType> DataTypes = new Dictionary<BaseType, DataType>
        {
            { BaseType.Void, VoidType.Instance },

            { BaseType.Bool, PrimitiveType.Bool },

            { BaseType.Byte, PrimitiveType.Byte },
            { BaseType.SByte, PrimitiveType.SByte },
            { BaseType.Char8, PrimitiveType.Char },

            { BaseType.Int16, PrimitiveType.Int16 },
            { BaseType.UInt16, PrimitiveType.UInt16 },
            { BaseType.Ptr16, PrimitiveType.Ptr16 },
            { BaseType.Word16, PrimitiveType.Word16 },

            { BaseType.Int32, PrimitiveType.Int32 },
            { BaseType.UInt32, PrimitiveType.UInt32 },
            { BaseType.Ptr32, PrimitiveType.Ptr32},
            { BaseType.Word32, PrimitiveType.Word32 },

            { BaseType.Int64, PrimitiveType.Int64 },
            { BaseType.UInt64, PrimitiveType.UInt64 },
            { BaseType.Ptr64, PrimitiveType.Ptr64 },
            { BaseType.Word64, PrimitiveType.Word64 },

            { BaseType.Word128, PrimitiveType.Word128 },

            { BaseType.Real32, PrimitiveType.Real32 },
            { BaseType.Real64, PrimitiveType.Real64 },
        };

    }
}
