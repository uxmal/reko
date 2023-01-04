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
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Arch.Mips
{
    public class MipsIntrinsics
    {
        public MipsIntrinsics(MipsProcessorArchitecture arch)
        {
            bit_intrinsic = new IntrinsicBuilder("__bit", false)
                .GenericTypes("TValue", "TPos")
                .Param("TValue")
                .Param("TPos")
                .Returns(PrimitiveType.Bool);
            break_intrinsic = new IntrinsicBuilder("__break", true)
                .Param(arch.WordWidth)
                .Void();
            cache_intrinsic = new IntrinsicBuilder("__cache", true)
                .Param(arch.WordWidth)
                .Param(arch.PointerType)
                .Void();
            cache_EVA_intrinsic = new IntrinsicBuilder("__cache_EVA", true)
                .Param(arch.WordWidth)
                .Param(arch.PointerType)
                .Void();
            clo = new IntrinsicBuilder("__clo", false)
                .Param(arch.WordWidth)
                .Returns(PrimitiveType.Int32);
            clz = new IntrinsicBuilder("__clz", false)
                .Param(arch.WordWidth)
                .Returns(PrimitiveType.Int32);
            ext = new IntrinsicBuilder("__ext", true)
                .GenericTypes("TValue", "TPos")
                .Param("TValue")
                .Param("TPos")
                .Param("TPos")
                .Returns("TValue");
            ins = new IntrinsicBuilder("__ins", true)
                .GenericTypes("TValue", "TPos")
                .Param("TValue")
                .Param("TValue")
                .Param("TPos")
                .Param("TPos")
                .Returns("TValue");
        }

        public readonly IntrinsicProcedure bit_intrinsic;
        public readonly IntrinsicProcedure break_intrinsic;
        public readonly IntrinsicProcedure cache_intrinsic;
        public readonly IntrinsicProcedure cache_EVA_intrinsic;
        public readonly IntrinsicProcedure clo;
        public readonly IntrinsicProcedure clz;
        public readonly IntrinsicProcedure ext;
        public readonly IntrinsicProcedure ins;
        public readonly IntrinsicProcedure reserved_instruction = new IntrinsicBuilder(
            "__reserved_instruction", true, new ProcedureCharacteristics
            {
                Terminates = true
            })
            .Param(PrimitiveType.Word32)
            .Void();
        public readonly IntrinsicProcedure trunc_intrinsic = IntrinsicBuilder.GenericUnary("trunc");
    }
}
