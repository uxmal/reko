#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using System.Text;

namespace Reko.Core.Intrinsics
{
    /// <summary>
    /// Common intrinsics.
    /// </summary>
    public static class CommonOps
    {
        public static readonly IntrinsicProcedure Rol = new IntrinsicBuilder("__rol", false)
            .GenericTypes("TData", "TCount")
            .Param("TData").Param("TCount")
            .Returns("TData");
        public static readonly IntrinsicProcedure RolC = new IntrinsicBuilder("__rcl", false)
            .GenericTypes("TData", "TCount", "TCy")
            .Param("TData").Param("TCount").Param("TCy")
            .Returns("TData");
        public static readonly IntrinsicProcedure Ror = new IntrinsicBuilder("__ror", false)
            .GenericTypes("TData", "TCount")
            .Param("TData").Param("TCount")
            .Returns("TData");
        public static readonly IntrinsicProcedure RorC = new IntrinsicBuilder("__rcr", false)
            .GenericTypes("TData", "TCount", "TCy")
            .Param("TData").Param("TCount").Param("TCy")
            .Returns("TData");
        public static readonly IntrinsicProcedure Syscall = new IntrinsicBuilder("__syscall", true)
            .Param(PrimitiveType.Word32)
            .Void();
    }
}
