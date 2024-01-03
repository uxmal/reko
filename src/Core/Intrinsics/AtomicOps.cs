#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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

namespace Reko.Core.Intrinsics
{
    public class AtomicOps
    {
        //$TODO: standard header file
        public static readonly IntrinsicProcedure atomic_compare_exchange_weak = new IntrinsicBuilder("atomic_compare_exchange_weak", true)
            .GenericTypes("T")
            .PtrParam("T")
            .Param("T")
            .Param("T")
            .Returns(PrimitiveType.Bool);
        public static readonly IntrinsicProcedure atomic_exchange = new IntrinsicBuilder("std::atomic_exchange", true)
            .GenericTypes("T")
            .PtrParam("T")
            .Param("T")
            .Returns("T");

        public static readonly IntrinsicProcedure atomic_test_and_set = new IntrinsicBuilder("atomic_test_and_set", true)
            .GenericTypes("T")
            .PtrParam("T")
            .Returns(PrimitiveType.Bool);

    }
}
