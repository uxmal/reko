#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using Reko.Core.Expressions;
using Reko.Core.Intrinsics;
using Reko.Core.Serialization;
using Reko.Core.Types;

namespace Reko.Arch.Mips.Rewriter
{
    public class MipsIntrinsics
    {
        public MipsIntrinsics(MipsArchitecture arch)
        {
            var a2_of_w64 = new ArrayType(PrimitiveType.Word64, 2);
            var a4_of_w32 = new ArrayType(PrimitiveType.Word32, 4);
            var a8_of_w16 = new ArrayType(PrimitiveType.Word16, 8);
            var a4_of_r32 = new ArrayType(PrimitiveType.Real32, 4);

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

            di = IntrinsicBuilder.SideEffect("__disable_interrupts")
                .Void();

            ei = IntrinsicBuilder.SideEffect("__enable_interrupts")
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

            mf0 = new IntrinsicBuilder("__move_from_breakpoint_control_register", true)
                .Returns(PrimitiveType.Word32);


            pand = IntrinsicBuilder.Binary("__pand", PrimitiveType.Word128);
            pcpyh = new IntrinsicBuilder("__p_copy_halfword", false)
                .Param(a2_of_w64)
                .Returns(a8_of_w16);
            pcpyld = new IntrinsicBuilder("__p_copy_lower_dword", false)
                .Param(a2_of_w64)
                .Param(a2_of_w64)
                .Returns(a2_of_w64);
            pcpyud = new IntrinsicBuilder("__p_copy_upper_dword", false)
                .Param(a2_of_w64)
                .Param(a2_of_w64)
                .Returns(a2_of_w64);
            pnor = IntrinsicBuilder.Binary("__pnor", PrimitiveType.Word128);
            pxor = IntrinsicBuilder.Binary("__pxor", PrimitiveType.Word128);

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

            vabs = IntrinsicBuilder.Unary("__vabs", a4_of_r32);
            vadd = IntrinsicBuilder.Binary("__vadd", a4_of_r32);
            vdiv = IntrinsicBuilder.Binary("__vdiv", a4_of_r32);
            vcallms = IntrinsicBuilder.SideEffect("__vcallms")
                .Param(PrimitiveType.Word32)
                .Void();
            vcallmsr = IntrinsicBuilder.SideEffect("__vcallmsr")
                .Void();

            vcallmsr = IntrinsicBuilder.SideEffect("__vcallmsr")
                .Void();
            vclipw = IntrinsicBuilder.Pure("__vclipw")
                .Param(a4_of_r32)
                .Param(a4_of_r32)
                .Returns(PrimitiveType.Bool);
            vilwr = IntrinsicBuilder.Pure("__vilwr")
                .Param(PrimitiveType.Ptr32)
                .Returns(a4_of_r32);
            viswr = IntrinsicBuilder.Pure("__viswr")
                .Param(PrimitiveType.Ptr32)
                .Param(a4_of_r32)
                .Void();
            vlqd = IntrinsicBuilder.Pure("__vlqd")
                .Param(PrimitiveType.Word32)
                .Returns(a4_of_w32);
            vlqi = IntrinsicBuilder.Pure("__vlqi")
                .Param(PrimitiveType.Word32)
                .Returns(a4_of_w32);
            vmr32 = IntrinsicBuilder.Pure("__vmr32")
                .Param(a4_of_w32)
                .Returns(a4_of_w32);
            vmaddax = IntrinsicBuilder.Pure("__vmaddax")
                .Param(PrimitiveType.Real32)
                .Param(a4_of_r32)
                .Param(a4_of_r32)
                .Returns(PrimitiveType.Real32);
            vmadday = IntrinsicBuilder.Pure("__vmadday")
                .Param(PrimitiveType.Real32)
                .Param(a4_of_r32)
                .Param(a4_of_r32)
                .Returns(PrimitiveType.Real32);
            vmaddaz = IntrinsicBuilder.Pure("__vmaddaz")
                .Param(PrimitiveType.Real32)
                .Param(a4_of_r32)
                .Param(a4_of_r32)
                .Returns(PrimitiveType.Real32);
            vmulax = IntrinsicBuilder.Binary("__vmulax", a4_of_r32);
            vrget = IntrinsicBuilder.SideEffect("__vrget")
                .Returns(PrimitiveType.Word32);
            vrinit = IntrinsicBuilder.SideEffect("__vrinit")
                .Param(PrimitiveType.Word32)
                .Void();
            vrnext = IntrinsicBuilder.SideEffect("__vrnext")
                .Returns(PrimitiveType.Word32);
            vrsqrt = IntrinsicBuilder.Pure("__vsqrt")
                .Param(a4_of_r32)
                .Param(a4_of_r32)
                .Returns(PrimitiveType.Real32);
            vrxor = IntrinsicBuilder.SideEffect("__vrxor")
                .Param(PrimitiveType.Word32)
                .Void();
            vsqd = IntrinsicBuilder.SideEffect("__vsqd")
                .Param(PrimitiveType.Word32)
                .Param(PrimitiveType.Word32)
                .Void();
            vsqi = IntrinsicBuilder.SideEffect("__vsqd")
                .Param(PrimitiveType.Word32)
                .Param(PrimitiveType.Word32)
                .Void();
            vsqrt = IntrinsicBuilder.Pure("__vsqrt")
                .Param(a4_of_r32)
                .Returns(PrimitiveType.Real32);

            waitq = IntrinsicBuilder.SideEffect("__waitq")
                .Void();
        }

        public readonly IntrinsicProcedure bit;
        public readonly IntrinsicProcedure break_intrinsic;
        public readonly IntrinsicProcedure cache_intrinsic;
        public readonly IntrinsicProcedure cache_EVA_intrinsic;
        public readonly IntrinsicProcedure clear_hazard_barrier;

        public readonly IntrinsicProcedure di;

        public readonly IntrinsicProcedure ei;
        public readonly IntrinsicProcedure ext;
        public readonly IntrinsicProcedure ins;
        
        public readonly IntrinsicProcedure load_linked;
        public readonly IntrinsicProcedure load_ub_EVA;
        public readonly IntrinsicProcedure ldl;
        public readonly IntrinsicProcedure ldr;
        public readonly IntrinsicProcedure lwl;
        public readonly IntrinsicProcedure lwr;

        public readonly IntrinsicProcedure mf0;

        public readonly IntrinsicProcedure pand;
        public readonly IntrinsicProcedure pcpyh;
        public readonly IntrinsicProcedure pcpyld;
        public readonly IntrinsicProcedure pcpyud;
        public readonly IntrinsicProcedure pnor;
        public readonly IntrinsicProcedure pxor;

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


        public readonly IntrinsicProcedure vabs;
        public readonly IntrinsicProcedure vadd;
        public readonly IntrinsicProcedure vcallms;
        public readonly IntrinsicProcedure vcallmsr;
        public readonly IntrinsicProcedure vclipw;
        public readonly IntrinsicProcedure vdiv;
        public readonly IntrinsicProcedure viswr;
        public readonly IntrinsicProcedure vilwr;
        public readonly IntrinsicProcedure vlqd;
        public readonly IntrinsicProcedure vlqi;
        public readonly IntrinsicProcedure vmr32;
        public readonly IntrinsicProcedure vmaddax;
        public readonly IntrinsicProcedure vmadday;
        public readonly IntrinsicProcedure vmaddaz;
        public readonly IntrinsicProcedure vmulax;
        public readonly IntrinsicProcedure vrget;
        public readonly IntrinsicProcedure vrinit;
        public readonly IntrinsicProcedure vrnext;
        public readonly IntrinsicProcedure vrxor;
        public readonly IntrinsicProcedure vrsqrt;
        public readonly IntrinsicProcedure vsqd;
        public readonly IntrinsicProcedure vsqi;
        public readonly IntrinsicProcedure vsqrt;

        public readonly IntrinsicProcedure  waitq;
    }
}
