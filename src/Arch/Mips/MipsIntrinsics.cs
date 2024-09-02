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
            bit = new IntrinsicBuilder("__bit", false)
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
            clear_hazard_barrier = new IntrinsicBuilder("__clear_hazard_barrier", true)
                .Void();
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
            ldl = new IntrinsicBuilder("__ldl", true)
                .Param(arch.PointerType)
                .Param(PrimitiveType.Int32)
                .Returns(PrimitiveType.Word64);
            ldr = new IntrinsicBuilder("__ldr", true)
                .Param(arch.PointerType)
                .Param(PrimitiveType.Int32)
                .Returns(PrimitiveType.Word64);
            load_linked = new IntrinsicBuilder("__load_linked", true)
                .GenericTypes("T")
                .PtrParam("T")
                .Returns("T");
            load_ub_EVA = new IntrinsicBuilder("__load_ub_EVA", true)
                .GenericTypes("T")
                .PtrParam("T")
                .Returns("T");
            lwl = new IntrinsicBuilder("__lwl", true)
                .Param(arch.PointerType)
                .Param(PrimitiveType.Int32)
                .Returns(PrimitiveType.Word64);
            lwr = new IntrinsicBuilder("__lwr", true)
                .Param(arch.PointerType)
                .Param(PrimitiveType.Int32)
                .Returns(PrimitiveType.Word64);

            read_cpr2 = new IntrinsicBuilder("__read_cpr2", true)
                .GenericTypes("T")
                .Param(PrimitiveType.Byte)
                .Returns("T");
            read_cpu_number = new IntrinsicBuilder("__read_cpu_number", true)
                .Returns(PrimitiveType.UInt32);
            read_user_local = new IntrinsicBuilder("__read_user_local", true)
                .Returns(PrimitiveType.Int32);
            read_hardware_register = new IntrinsicBuilder("__read_hardware_register", true)
                .Param(PrimitiveType.UInt32)
                .Returns(PrimitiveType.UInt32);
            rotx = IntrinsicBuilder.Pure("__rotx")
                .GenericTypes("T")
                .Params("T", "T", "T", "T")
                .Returns("T");

            sdbbp = new IntrinsicBuilder("__software_debug_breakpoint", true)
                .Param(arch.WordWidth)
                .Void();
            sdl = new IntrinsicBuilder("__sdl", true)
                .Param(arch.PointerType)
                .Param(PrimitiveType.Int32)
                .Param(PrimitiveType.Word64)
                .Void();
            sdr = new IntrinsicBuilder("__sdr", true)
                .Param(arch.PointerType)
                .Param(PrimitiveType.Int32)
                .Param(PrimitiveType.Word64)
                .Void();
            store_EVA = new IntrinsicBuilder("__store_EVA", true)
                .GenericTypes("T")
                .PtrParam("T")
                .Param("T")
                .Void();
            swl = new IntrinsicBuilder("__swl", true)
                .Param(PrimitiveType.Word32)
                .Param(PrimitiveType.Word32)
                .Returns(PrimitiveType.Word32);
            swr = new IntrinsicBuilder("__swr", true)
                .Param(PrimitiveType.Word32)
                .Param(PrimitiveType.Word32)
                .Returns(PrimitiveType.Word32);

            store_conditional = new IntrinsicBuilder("__store_conditional", true)
                .GenericTypes("T")
                .PtrParam("T")
                .Param("T")
                .Returns("T");
            sync = new IntrinsicBuilder("__sync", true)
                .Param(arch.WordWidth)
                .Void();
            tlbp = new IntrinsicBuilder("__tlbp", true)
                .Void();
            tlbr = new IntrinsicBuilder("__tlbr", true)
                .Void();
            tlbwi = new IntrinsicBuilder("__tlbwi", true)
                .Void();
            tlbwr = new IntrinsicBuilder("__tlbwr", true)
                .Void();
            wait = new IntrinsicBuilder("__wait", true)
                .Void();
            write_cpf2 = new IntrinsicBuilder("__write_cpr2", true)
                .Param(PrimitiveType.Byte)
                .Param(PrimitiveType.Word32)
                .Void();

            wsbh = IntrinsicBuilder.GenericUnary("__word_swap_bytes_in_halfwords");
        }

        public readonly IntrinsicProcedure bit;
        public readonly IntrinsicProcedure break_intrinsic;
        public readonly IntrinsicProcedure cache_intrinsic;
        public readonly IntrinsicProcedure cache_EVA_intrinsic;
        public readonly IntrinsicProcedure clear_hazard_barrier;

        public readonly IntrinsicProcedure ext;
        public readonly IntrinsicProcedure ins;
        
        public readonly IntrinsicProcedure load_linked;
        public readonly IntrinsicProcedure load_ub_EVA;
        public readonly IntrinsicProcedure ldl;
        public readonly IntrinsicProcedure ldr;
        public readonly IntrinsicProcedure lwl;
        public readonly IntrinsicProcedure lwr;

        public readonly IntrinsicProcedure read_cpr2;
        public readonly IntrinsicProcedure read_cpu_number;
        public readonly IntrinsicProcedure read_hardware_register;
        public readonly IntrinsicProcedure read_user_local;
        public readonly IntrinsicProcedure rotx;
        
        public readonly IntrinsicProcedure sdl;
        public readonly IntrinsicProcedure sdr;
        public readonly IntrinsicProcedure swl;
        public readonly IntrinsicProcedure sync;
        public readonly IntrinsicProcedure sdbbp;
        public readonly IntrinsicProcedure store_conditional;
        public readonly IntrinsicProcedure store_EVA;
        public readonly IntrinsicProcedure swr;
        
        public readonly IntrinsicProcedure tlbp;
        public readonly IntrinsicProcedure tlbr;
        public readonly IntrinsicProcedure tlbwi;
        public readonly IntrinsicProcedure tlbwr;
        
        public readonly IntrinsicProcedure wait;
        public readonly IntrinsicProcedure write_cpf2;
        public readonly IntrinsicProcedure wsbh;
        
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
