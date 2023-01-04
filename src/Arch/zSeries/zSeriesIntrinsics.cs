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

using Reko.Core;
using Reko.Core.Intrinsics;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.zSeries
{
    public class zSeriesIntrinsics
    {
        private readonly zSeriesArchitecture arch;

        public zSeriesIntrinsics(zSeriesArchitecture arch)
        {
            this.arch = arch;
            compare_and_swap = new IntrinsicBuilder("__compare_and_swap", true)
                .GenericTypes("T")
                .Param("T")
                .Param("T")
                .PtrParam("T")
                .OutParam("T")
                .Returns(PrimitiveType.Byte);
            lra = new IntrinsicBuilder("__load_real_address", true)
                .GenericTypes("T")
                .PtrParam("T")
                .OutParam("T")
                .Returns(PrimitiveType.Byte);

            mvcle = new IntrinsicBuilder("__mvcle", true)
                .GenericTypes("T")
                .PtrParam("T")
                .Returns(PrimitiveType.Word128);
        }

        public readonly IntrinsicProcedure bprp = new IntrinsicBuilder("__branch_prediction_relative_preload", true)
            .GenericTypes("T")
            .Param("T")
            .Param("T")
            .Param("T")
            .Void();
        public readonly IntrinsicProcedure compare_and_swap;
        public readonly IntrinsicProcedure cvb = new IntrinsicBuilder("__convert_decimal_to_int", false)
            .GenericTypes("T")
            .PtrParam(PrimitiveType.Byte)
            .Returns("T");
        public readonly IntrinsicProcedure cvd = new IntrinsicBuilder("__convert_int_to_decimal", false)
            .GenericTypes("T")
            .Param("T")
            .PtrParam(PrimitiveType.Byte)
            .Void();
        public readonly IntrinsicProcedure dp = new IntrinsicBuilder("__packed_divide", false)
            .PtrParam(PrimitiveType.Byte)
            .Param(PrimitiveType.Int32)
            .PtrParam(PrimitiveType.Byte)
            .Param(PrimitiveType.Int32)
            .PtrParam(PrimitiveType.Byte)
            .Returns(PrimitiveType.Byte);

        public readonly IntrinsicProcedure execute = new IntrinsicBuilder("__execute", true)
            .GenericTypes("T")
            .Param("T")
            .Param("T")
            .Returns(PrimitiveType.Byte);

        public readonly IntrinsicProcedure fabs = IntrinsicBuilder.GenericUnary("fabs");

        public readonly IntrinsicProcedure lra;
        public readonly IntrinsicProcedure lrv = IntrinsicBuilder.GenericUnary("__load_reverse");

        public readonly IntrinsicProcedure mvcle;
        public readonly IntrinsicProcedure move_zones = new IntrinsicBuilder("__move_zones", true)
            .GenericTypes("T")
            .Param("T")
            .Param("T")
            .Returns("T");

        public readonly IntrinsicProcedure risbg = new IntrinsicBuilder("__risbg", false)
            .Param(PrimitiveType.Word64)
            .Param(PrimitiveType.Word64)
            .Param(PrimitiveType.Word64)
            .Param(PrimitiveType.Word64)
            .Returns(PrimitiveType.Word64);
        public readonly IntrinsicProcedure risbgn = new IntrinsicBuilder("__risbgn", false)
            .Param(PrimitiveType.Word64)
            .Param(PrimitiveType.Word64)
            .Param(PrimitiveType.Word64)
            .Param(PrimitiveType.Word64)
            .Returns(PrimitiveType.Word64);

        public readonly IntrinsicProcedure stctl = new IntrinsicBuilder("__store_control", true)
            .GenericTypes("T1","T2")
            .Param("T1")
            .Param("T2")
            .Void();


        public readonly IntrinsicProcedure ts = new IntrinsicBuilder("__test_and_set", true)
            .GenericTypes("T")
            .PtrParam("T")
            .Returns(PrimitiveType.Byte);

        public readonly IntrinsicProcedure vll = new IntrinsicBuilder("__vll", false)
            .Param(PrimitiveType.UInt32)
            .Param(PrimitiveType.Word128)
            .Returns(PrimitiveType.Word128);
        public readonly IntrinsicProcedure vavg = IntrinsicBuilder.GenericBinary("__vavg");
        public readonly IntrinsicProcedure vfa = IntrinsicBuilder.GenericBinary("__vfa");
        public readonly IntrinsicProcedure vfd = IntrinsicBuilder.GenericBinary("__vfd");
        public readonly IntrinsicProcedure vfs = IntrinsicBuilder.GenericBinary("__vfs");
        public readonly IntrinsicProcedure verim = IntrinsicBuilder.GenericTernary("__verim");
        public readonly IntrinsicProcedure vmao = IntrinsicBuilder.GenericTernary("__vmao");
        public readonly IntrinsicProcedure vmah = IntrinsicBuilder.GenericTernary("__vmah");
        public readonly IntrinsicProcedure vmrh = IntrinsicBuilder.GenericBinary("__vmrh");
        public readonly IntrinsicProcedure vmrl = IntrinsicBuilder.GenericBinary("__vmrl");
        public readonly IntrinsicProcedure vmx = IntrinsicBuilder.GenericBinary("__vmx");
        public readonly IntrinsicProcedure vn = IntrinsicBuilder.Binary("__vn", PrimitiveType.Word128);
        public readonly IntrinsicProcedure vpk = IntrinsicBuilder.GenericBinary("__vpk");
        public readonly IntrinsicProcedure vpkls = IntrinsicBuilder.GenericTernary("__vpkls");
        public readonly IntrinsicProcedure vslb = new IntrinsicBuilder("__vslb", false)
            .Param(PrimitiveType.Word128)
            .Param(PrimitiveType.Word128)
            .Returns(PrimitiveType.Word128);
        public readonly IntrinsicProcedure vsel = new IntrinsicBuilder("__vsel", false)
            .Param(PrimitiveType.Word128)
            .Param(PrimitiveType.Word128)
            .Param(PrimitiveType.Word128)
            .Returns(PrimitiveType.Word128);
        public readonly IntrinsicProcedure vx = IntrinsicBuilder.Binary("__vx", PrimitiveType.Word128);
    }
}
