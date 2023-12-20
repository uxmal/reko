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

namespace Reko.Arch.Arm.AArch64
{
    public class Intrinsics
    {
        public record IntrinsicPair(
                IntrinsicProcedure Scalar,
                IntrinsicProcedure Vector);

        public readonly IntrinsicProcedure addhn = IntrinsicBuilder.GenericBinary("__addhn");
        public readonly IntrinsicProcedure addhn2 = IntrinsicBuilder.GenericBinary("__addhn2");
        public readonly IntrinsicProcedure addp = IntrinsicBuilder.GenericBinary("__addp");

        public readonly IntrinsicProcedure bfm = IntrinsicBuilder.Pure("__bfm")
            .GenericTypes("T")
            .Param("T")
            .Param("T")
            .Param(PrimitiveType.Int32)
            .Param(PrimitiveType.Int32)
            .Returns("T");
        public readonly IntrinsicProcedure bif = IntrinsicBuilder.GenericBinary("__bif");
        public readonly IntrinsicProcedure bit = IntrinsicBuilder.GenericBinary("__bit");
        public readonly IntrinsicProcedure brk =
            new IntrinsicBuilder("__brk", true, new Core.Serialization.ProcedureCharacteristics
            {
                Terminates = true,
            })
            .Param(PrimitiveType.UInt16)
            .Void();

    public readonly IntrinsicProcedure ceil = IntrinsicBuilder.GenericUnary("__ceil");
    public readonly IntrinsicProcedure cls = IntrinsicBuilder.GenericUnary("__cls");
    public readonly IntrinsicProcedure cmeq = IntrinsicBuilder.GenericBinary("__cmeq");
    public readonly IntrinsicProcedure cmge = IntrinsicBuilder.GenericBinary("__cmge");
    public readonly IntrinsicProcedure cmgt = IntrinsicBuilder.GenericBinary("__cmgt");
    public readonly IntrinsicProcedure cmhi = IntrinsicBuilder.GenericBinary("__cmhi");
    public readonly IntrinsicProcedure cmhs = IntrinsicBuilder.GenericBinary("__cmhs");
    public readonly IntrinsicProcedure cmle = IntrinsicBuilder.GenericBinary("__cmle");
    public readonly IntrinsicProcedure cmlt = IntrinsicBuilder.GenericBinary("__cmlt");
    public readonly IntrinsicProcedure cmtst = IntrinsicBuilder.GenericBinary("__cmtst");
    public readonly IntrinsicProcedure cnt = IntrinsicBuilder.GenericUnary("__cnt");
    public readonly IntrinsicProcedure cvtf = IntrinsicBuilder.GenericUnary("__cvtf");
    public readonly IntrinsicProcedure cvtf_fixed = IntrinsicBuilder.Pure("__cvtf_fixed")
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc")
            .Param(PrimitiveType.Int32)
            .Returns("TDst");

    public readonly IntrinsicProcedure dmb = new IntrinsicBuilder("__data_memory_barrier", true)
            .Param(new UnknownType())
            .Void();
    public readonly IntrinsicProcedure dsb = new IntrinsicBuilder("__data_sync_barrier", true)
            .Param(new UnknownType())
            .Void();
    public readonly IntrinsicProcedure dup = IntrinsicBuilder.Pure("__dup")
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc")
            .Returns("TDst");

    public readonly IntrinsicProcedure eret = new IntrinsicBuilder("__eret", true)
            .Void();
    public readonly IntrinsicProcedure ext = IntrinsicBuilder.Pure("__ext")
            .GenericTypes("T")
            .Param("T")
            .Param("T")
            .Param(PrimitiveType.Byte)
            .Returns("T");

    public readonly IntrinsicProcedure floor = IntrinsicBuilder.GenericUnary("__floor");
    public readonly IntrinsicProcedure fmadd = IntrinsicBuilder.GenericTernary("__fmadd");
    public readonly IntrinsicProcedure fmsub = IntrinsicBuilder.GenericTernary("__fmsub");
    public readonly IntrinsicProcedure fmov = IntrinsicBuilder.Pure("__fmov")
            .GenericTypes("TSrc", "TDst")
            .Params("TSrc")
            .Returns("TDst");
    public readonly IntrinsicProcedure fnmadd = IntrinsicBuilder.GenericTernary("__fnmadd");
    public readonly IntrinsicProcedure fnmsub = IntrinsicBuilder.GenericTernary("__fnmsub");

    public readonly IntrinsicProcedure isb = new IntrinsicBuilder("__instruction_sync_barrier", true)
            .Param(new UnknownType())
            .Void();

    public readonly IntrinsicProcedure ld1 = IntrinsicBuilder.Pure("__ld1")
            .GenericTypes("T")
            .PtrParam("T")
            .Variadic()
            .Void();
    public readonly IntrinsicProcedure ld1r = IntrinsicBuilder.Pure("__ld1r")
            .GenericTypes("T")
            .PtrParam("T")
            .Variadic()
            .Void();
    public readonly IntrinsicProcedure ld2 = IntrinsicBuilder.Pure("__ld2")
            .GenericTypes("T")
            .PtrParam("T")
            .Variadic()
            .Void();
    public readonly IntrinsicProcedure ld2r = IntrinsicBuilder.Pure("__ld2r")
          .GenericTypes("T")
          .PtrParam("T")
          .Variadic()
          .Void();
    public readonly IntrinsicProcedure ld3 = IntrinsicBuilder.Pure("__ld3")
            .GenericTypes("T")
            .PtrParam("T")
            .Variadic()
            .Void();
    public readonly IntrinsicProcedure ld3r = IntrinsicBuilder.Pure("__ld3r")
            .GenericTypes("T")
            .PtrParam("T")
            .Variadic()
            .Void();
    public readonly IntrinsicProcedure ld4 = IntrinsicBuilder.Pure("__ld4")
            .GenericTypes("T")
            .PtrParam("T")
            .Variadic()
            .Void();
    public readonly IntrinsicProcedure ld4r = IntrinsicBuilder.Pure("__ld4r")
            .GenericTypes("T")
            .PtrParam("T")
            .Variadic()
            .Void();
    public readonly IntrinsicProcedure load_acquire = new IntrinsicBuilder("__load_acquire", true)
            .GenericTypes("T")
            .PtrParam("T")
            .Returns("T");
    public readonly IntrinsicProcedure load_acquire_exclusive = new IntrinsicBuilder("__load_acquire_exclusive", true)
            .GenericTypes("T")
            .PtrParam("T")
            .Returns("T");
    public readonly IntrinsicProcedure load_exclusive = new IntrinsicBuilder("__load_exclusive", true)
            .GenericTypes("T")
            .PtrParam("T")
            .Returns("T");

    public readonly IntrinsicProcedure mla = IntrinsicBuilder.GenericTernary("__mla");
    public readonly IntrinsicProcedure mla_by_element = IntrinsicBuilder.Pure("__mla_by_element")
            .GenericTypes("TArray", "TElem")
            .Params("TArray", "TArray", "TElem")
            .Returns("TArray");
    public readonly IntrinsicProcedure mls = IntrinsicBuilder.GenericTernary("__mls");
    public readonly IntrinsicProcedure mls_by_element = IntrinsicBuilder.Pure("__mls_by_element")
            .GenericTypes("TArray", "TElem")
            .Params("TArray", "TArray", "TElem")
            .Returns("TArray");
    public readonly IntrinsicProcedure mrs = new IntrinsicBuilder("__mrs", true)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word64);
    public readonly IntrinsicProcedure msr = new IntrinsicBuilder("__msr", true)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word64)
            .Void();

    public readonly IntrinsicProcedure mull = IntrinsicBuilder.Pure("__mull")
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc")
            .Param("TSrc")
            .Returns("TDst");

    public readonly IntrinsicProcedure nearest = IntrinsicBuilder.GenericUnary("__nearest");
    public readonly IntrinsicProcedure neg = IntrinsicBuilder.GenericUnary("__neg");

    public readonly IntrinsicProcedure pmul = IntrinsicBuilder.GenericBinary("__pmul");
    public readonly IntrinsicProcedure pmull = IntrinsicBuilder.GenericBinary("__pmull");
    public readonly IntrinsicProcedure pmull2 = IntrinsicBuilder.GenericBinary("__pmull2");
    public readonly IntrinsicProcedure prfm = new IntrinsicBuilder("__prfm", true)
            .GenericTypes("T")
            .Param("T")
            .PtrParam("T")
            .Void();

    public readonly IntrinsicProcedure raddhn = IntrinsicBuilder.GenericBinary("__raddhn");
    public readonly IntrinsicProcedure raddhn2 = IntrinsicBuilder.GenericBinary("__raddhn2");
    public readonly IntrinsicProcedure rev16 = IntrinsicBuilder.GenericUnary("__rev16");
    public readonly IntrinsicProcedure rev64 = IntrinsicBuilder.GenericUnary("__rev64");
    public readonly IntrinsicProcedure round = IntrinsicBuilder.GenericUnary("__round");
    public readonly IntrinsicProcedure rshrn = IntrinsicBuilder.GenericBinary("__rshrn");
    public readonly IntrinsicProcedure rshrn2 = IntrinsicBuilder.GenericBinary("__rshrn2");
    public readonly IntrinsicProcedure rsubhn = IntrinsicBuilder.Pure("__rsubhn")
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc")
            .Param("TSrc")
            .Returns("TDst");
    public readonly IntrinsicProcedure rsubhn2 = IntrinsicBuilder.Pure("__rsubhn2")
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc")
            .Param("TSrc")
            .Returns("TDst");
    public readonly IntrinsicProcedure saba = IntrinsicBuilder.GenericBinary("__saba");
    public readonly IntrinsicProcedure sabal = IntrinsicBuilder.GenericBinary("__sabal");
    public readonly IntrinsicProcedure sabal2 = IntrinsicBuilder.GenericBinary("__sabal2");
    public readonly IntrinsicProcedure sabd = IntrinsicBuilder.GenericBinary("__sabd");
    public readonly IntrinsicProcedure sabdl = IntrinsicBuilder.Pure("__sabdl")
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc")
            .Param("TSrc")
            .Returns("TDst");
    public readonly IntrinsicProcedure sabdl2 = IntrinsicBuilder.Pure("__sabdl2")
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc")
            .Param("TSrc")
            .Returns("TDst");
    public readonly IntrinsicProcedure sadalp = IntrinsicBuilder.GenericBinary("__sadalp");
    public readonly IntrinsicProcedure saddl = IntrinsicBuilder.Pure("__saddl")
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc")
            .Param("TSrc")
            .Returns("TDst");
    public readonly IntrinsicProcedure saddl2 = IntrinsicBuilder.Pure("__saddl2")
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc")
            .Param("TSrc")
            .Returns("TDst");
    public readonly IntrinsicProcedure saddlp = IntrinsicBuilder.Pure("__saddlp")
            .GenericTypes("TNarrow", "TWide")
            .Params("TNarrow")
            .Returns("TWide");
    public readonly IntrinsicProcedure saddlv = IntrinsicBuilder.GenericUnary("__saddlv");
    public readonly IntrinsicProcedure saddw = IntrinsicBuilder.Pure("__saddw")
            .GenericTypes("TWide", "TNarrow")
            .Params("TWide")
            .Params("TNarrow")
            .Returns("TWide");
    public readonly IntrinsicProcedure saddw2 = IntrinsicBuilder.Pure("__saddw2")
            .GenericTypes("TWide", "TNarrow")
            .Params("TWide")
            .Params("TNarrow")
            .Returns("TWide");
    public readonly IntrinsicProcedure sbfiz = IntrinsicBuilder.Pure("__sbfiz")
            .GenericTypes("T")
            .Param("T")
            .Param(PrimitiveType.Int32)
            .Param(PrimitiveType.Int32)
            .Returns("T");
    public readonly IntrinsicProcedure sbfm = IntrinsicBuilder.Pure("__sbfm")
            .GenericTypes("T")
            .Param("T")
            .Param(PrimitiveType.Int32)
            .Param(PrimitiveType.Int32)
            .Returns("T");
    public readonly IntrinsicProcedure sha1c = new IntrinsicBuilder("__sha1c", false)
        .Param(PrimitiveType.Word32)
        .Param(PrimitiveType.Word128)
        .Returns(PrimitiveType.Word128);
    public readonly IntrinsicProcedure shadd = IntrinsicBuilder.GenericBinary("__shadd");
    public readonly IntrinsicProcedure shl = IntrinsicBuilder.GenericBinary("__shl");
    public readonly IntrinsicProcedure shll = IntrinsicBuilder.GenericBinary("__shll");
    public readonly IntrinsicProcedure shll2 = IntrinsicBuilder.GenericBinary("__shll2");
    public readonly IntrinsicProcedure shrn = IntrinsicBuilder.Pure("__shrn")
            .GenericTypes("T")
            .Param("T")
            .Param(PrimitiveType.Int32)
            .Returns("T");
    public readonly IntrinsicProcedure shsub = IntrinsicBuilder.GenericBinary("__shsub");
    public readonly IntrinsicProcedure sli = IntrinsicBuilder.GenericBinary("__sli");
    public readonly IntrinsicProcedure smax = IntrinsicBuilder.GenericBinary("__smax");
    public readonly IntrinsicProcedure smaxp = IntrinsicBuilder.GenericBinary("__smaxp");
    public readonly IntrinsicProcedure smaxv = IntrinsicBuilder.Pure("__smaxv")
            .GenericTypes("TSrc", "TDst")
            .Params("TSrc")
            .Returns("TDst");
    public readonly IntrinsicProcedure smc =
            new IntrinsicBuilder("__secure_monitor_call", true)
            .Param(PrimitiveType.UInt16)
            .Void();
    public readonly IntrinsicProcedure smin = IntrinsicBuilder.GenericBinary("__smin");
    public readonly IntrinsicProcedure sminp = IntrinsicBuilder.GenericBinary("__sminp");
    public readonly IntrinsicProcedure sminv = IntrinsicBuilder.GenericUnary("__sminv");
    public readonly IntrinsicProcedure smlal = IntrinsicBuilder.Pure("__smlal")
            .GenericTypes("TSrc", "TDst")
            .Params("TDst")
            .Params("TSrc")
            .Params("TSrc")
            .Returns("TDst");
    public readonly IntrinsicProcedure smlal2 = IntrinsicBuilder.Pure("__smlal2")
            .GenericTypes("TSrc", "TDst")
            .Params("TDst")
            .Params("TSrc")
            .Params("TSrc")
            .Returns("TDst");
    public readonly IntrinsicProcedure smlsl = IntrinsicBuilder.Pure("__smlsl")
            .GenericTypes("TSrc", "TDst")
            .Params("TDst")
            .Params("TSrc")
            .Params("TSrc")
            .Returns("TDst");
    public readonly IntrinsicProcedure smlsl2 = IntrinsicBuilder.Pure("__smlsl2")
            .GenericTypes("TSrc", "TDst")
            .Params("TDst")
            .Params("TSrc")
            .Params("TSrc")
            .Returns("TDst");
    public readonly IntrinsicProcedure smull2 = IntrinsicBuilder.GenericBinary("__smull2");
    public readonly IntrinsicProcedure sqabs = IntrinsicBuilder.GenericUnary("__sqabs");
    public readonly IntrinsicProcedure sqadd = IntrinsicBuilder.GenericBinary("__sqadd");
    public readonly IntrinsicPair sqdmulh = new (
        IntrinsicBuilder.GenericBinary("__sqdmulh"),
        IntrinsicBuilder.GenericBinary("__sqdmulh_vec"));
    public readonly IntrinsicProcedure sqdmlal = IntrinsicBuilder.GenericTernary("__sqdmlal");
    public readonly IntrinsicProcedure sqdmlal2 = IntrinsicBuilder.GenericTernary("__sqdmlal2");
    public readonly IntrinsicProcedure sqdmlsl = IntrinsicBuilder.GenericTernary("__sqdmlsl");
    public readonly IntrinsicProcedure sqdmlsl2 = IntrinsicBuilder.GenericTernary("__sqdmlsl2");
    public readonly IntrinsicProcedure sqdmlah = IntrinsicBuilder.GenericTernary("__sqdmlah");
    public readonly IntrinsicProcedure sqdmlsh = IntrinsicBuilder.GenericTernary("__sqdmlsh");
    public readonly IntrinsicProcedure sqdmull = IntrinsicBuilder.Pure("__sqdmull")
            .GenericTypes("TSrc", "TDst")
            .Params("TSrc")
            .Params("TSrc")
            .Returns("TDst");
    public readonly IntrinsicProcedure sqdmull2 = IntrinsicBuilder.Pure("__sqdmull2")
            .GenericTypes("TSrc", "TDst")
            .Params("TSrc")
            .Params("TSrc")
            .Returns("TDst");
    public readonly IntrinsicProcedure sqneg = IntrinsicBuilder.GenericUnary("__sqneg");
    public readonly IntrinsicProcedure sqrdmlah = IntrinsicBuilder.GenericTernary("__sqrdmlah");
    public readonly IntrinsicProcedure sqrdmlsh = IntrinsicBuilder.GenericTernary("__sqrdmlsh");
    public readonly IntrinsicProcedure sqrdmulh = IntrinsicBuilder.GenericBinary("__sqrdmulh");
    public readonly IntrinsicProcedure sqrshl = IntrinsicBuilder.GenericBinary("__sqrshl");
    public readonly IntrinsicProcedure sqrshrn = IntrinsicBuilder.GenericBinary("__sqrshrn");
    public readonly IntrinsicProcedure sqrshrn2 = IntrinsicBuilder.GenericBinary("__sqrshrn2");
    public readonly IntrinsicProcedure sqrshrun = IntrinsicBuilder.GenericBinary("__sqrshrun");
    public readonly IntrinsicProcedure sqrshrun2 = IntrinsicBuilder.GenericBinary("__sqrshrun2");
    public readonly IntrinsicProcedure sqshl = IntrinsicBuilder.GenericBinary("__sqshl");
    public readonly IntrinsicProcedure sqshlu = IntrinsicBuilder.GenericBinary("__sqshlu");
    public readonly IntrinsicProcedure sqshrn = IntrinsicBuilder.GenericBinary("__sqshrn");
    public readonly IntrinsicProcedure sqshrn2 = IntrinsicBuilder.GenericBinary("__sqshrn2");
    public readonly IntrinsicProcedure sqsub = IntrinsicBuilder.GenericBinary("__sqsub");
    public readonly IntrinsicProcedure sqxtn = IntrinsicBuilder.Pure("__sqxtn")
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc")
            .Returns("TDst");
    public readonly IntrinsicProcedure sqxtn2 = IntrinsicBuilder.Pure("__sqxtn2")
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc")
            .Returns("TDst");
    public readonly IntrinsicProcedure sqxtun = IntrinsicBuilder.Pure("__sqxtun")
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc")
            .Returns("TDst");
    public readonly IntrinsicProcedure sqxtun2 = IntrinsicBuilder.Pure("__sqxtun2")
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc")
            .Returns("TDst");
    public readonly IntrinsicProcedure sri = IntrinsicBuilder.GenericBinary("__sri");
    public readonly IntrinsicProcedure srhadd = IntrinsicBuilder.GenericBinary("__srhadd");
    public readonly IntrinsicProcedure srshl = IntrinsicBuilder.GenericBinary("__srshl");
    public readonly IntrinsicProcedure srshr = IntrinsicBuilder.GenericBinary("__srshr");
    public readonly IntrinsicProcedure srsra = IntrinsicBuilder.GenericBinary("__srsra");
    public readonly IntrinsicProcedure sshl = IntrinsicBuilder.Pure("__sshl")
            .GenericTypes("T")
            .Param("T")
            .Param(PrimitiveType.Int32)
            .Returns("T");
    public readonly IntrinsicProcedure sshll = IntrinsicBuilder.GenericBinary("__sshll");
    public readonly IntrinsicProcedure sshll2 = IntrinsicBuilder.GenericBinary("__sshll2");
    public readonly IntrinsicProcedure sshr = IntrinsicBuilder.Pure("__sshr")
            .GenericTypes("T")
            .Param("T")
            .Param(PrimitiveType.Int32)
            .Returns("T");
    public readonly IntrinsicProcedure ssra = IntrinsicBuilder.Pure("__ssra")
            .GenericTypes("T")
            .Param("T")
            .Param(PrimitiveType.Int32)
            .Returns("T");
    public readonly IntrinsicProcedure ssubl = IntrinsicBuilder.GenericBinary("__ssubl");
    public readonly IntrinsicProcedure ssubl2 = IntrinsicBuilder.GenericBinary("__ssubl2");
    public readonly IntrinsicProcedure ssubw = IntrinsicBuilder.GenericBinary("__ssubw");
    public readonly IntrinsicProcedure ssubw2 = IntrinsicBuilder.GenericBinary("__ssubw2");
    public readonly IntrinsicProcedure st1 = IntrinsicBuilder.Pure("__st1")
            .GenericTypes("T")
            .PtrParam("T")
            .Variadic()
            .Void();
    public readonly IntrinsicProcedure st2 = IntrinsicBuilder.Pure("__st2")
            .GenericTypes("T")
            .PtrParam("T")
            .Variadic()
            .Void();
    public readonly IntrinsicProcedure st3 = IntrinsicBuilder.Pure("__st3")
            .GenericTypes("T")
            .PtrParam("T")
            .Variadic()
            .Void();
    public readonly IntrinsicProcedure st4 = IntrinsicBuilder.Pure("__st4")
            .GenericTypes("T")
            .PtrParam("T")
            .Variadic()
            .Void();
    public readonly IntrinsicProcedure stlr = new IntrinsicBuilder("__store_release", true)
            .GenericTypes("T")
            .PtrParam("T")
            .Param("T")
            .Void();
    public readonly IntrinsicProcedure stx = new IntrinsicBuilder("__store_exclusive", true)
            .GenericTypes("T")
            .PtrParam("T")
            .Param("T")
            .Returns(PrimitiveType.Int32);
    public readonly IntrinsicProcedure subhn = IntrinsicBuilder.GenericBinary("__subhn");
    public readonly IntrinsicProcedure subhn2 = IntrinsicBuilder.Pure("__subhn2")
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc")
            .Param("TSrc")
            .Returns("TDst");
    public readonly IntrinsicProcedure sum = IntrinsicBuilder.Pure("__sum")
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc")
            .Returns("TDst");
    public readonly IntrinsicProcedure suqadd = IntrinsicBuilder.GenericBinary("__suqadd");

    public readonly IntrinsicProcedure svc = new IntrinsicBuilder("__supervisor_call", true)
            .Param(PrimitiveType.UInt16)
            .Void();
    public readonly IntrinsicProcedure sxtl = IntrinsicBuilder.Pure("__sxtl")
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc")
            .Returns("TDst");

    public readonly IntrinsicProcedure tbl = IntrinsicBuilder.Pure("__tbl")
            .GenericTypes("T")
            .Param(PrimitiveType.Int32)
            .Returns("T");
    public readonly IntrinsicProcedure tbx = IntrinsicBuilder.Pure("__tbx")
            .GenericTypes("T")
            .Param(PrimitiveType.Int32)
            .Returns("T");

    public readonly IntrinsicProcedure trn1 = IntrinsicBuilder.GenericBinary("__trn1");
    public readonly IntrinsicProcedure trn2 = IntrinsicBuilder.GenericBinary("__trn2");
    public readonly IntrinsicProcedure trunc = IntrinsicBuilder.GenericUnary("__trunc");

    public readonly IntrinsicProcedure uaba = IntrinsicBuilder.GenericBinary("__uaba");
    public readonly IntrinsicProcedure uabal = IntrinsicBuilder.Pure("__uabal")
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc")
            .Param("TSrc")
            .Returns("TDst");
    public readonly IntrinsicProcedure uabal2 = IntrinsicBuilder.Pure("__uabal2")
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc")
            .Param("TSrc")
            .Returns("TDst");
    public readonly IntrinsicProcedure uabd = IntrinsicBuilder.GenericBinary("__uabd");
    public readonly IntrinsicProcedure uabdl = IntrinsicBuilder.GenericBinary("__uabdl");
    public readonly IntrinsicProcedure uabdl2 = IntrinsicBuilder.GenericBinary("__uabdl2");
    public readonly IntrinsicProcedure uadalp = IntrinsicBuilder.GenericBinary("__uadalp");
    public readonly IntrinsicProcedure uaddl = IntrinsicBuilder.Pure("__uaddl")
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc")
            .Param("TSrc")
            .Returns("TDst");
    public readonly IntrinsicProcedure uaddl2 = IntrinsicBuilder.Pure("__uaddl2")
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc")
            .Param("TSrc")
            .Returns("TDst");
    public readonly IntrinsicProcedure uaddlp = IntrinsicBuilder.GenericBinary("__uaddlp");
    public readonly IntrinsicProcedure uaddlv = IntrinsicBuilder.GenericUnary("__uaddlv");
    public readonly IntrinsicProcedure uaddw = IntrinsicBuilder.Pure("__uaddw")
            .GenericTypes("TWide", "TNarrow")
            .Params("TWide")
            .Params("TNarrow")
            .Returns("TWide");
    public readonly IntrinsicProcedure uaddw2 = IntrinsicBuilder.Pure("__uaddw2")
            .GenericTypes("TWide", "TNarrow")
            .Params("TWide")
            .Params("TNarrow")
            .Returns("TWide");
    public readonly IntrinsicProcedure ubfm = IntrinsicBuilder.Pure("__ubfm")
            .GenericTypes("T")
            .Param("T")
            .Param(PrimitiveType.Int32)
            .Param(PrimitiveType.Int32)
            .Returns("T");
    public readonly IntrinsicProcedure uhadd = IntrinsicBuilder.GenericBinary("__uhadd");
    public readonly IntrinsicProcedure uhsub = IntrinsicBuilder.GenericBinary("__uhsub");
    public readonly IntrinsicProcedure umax = IntrinsicBuilder.GenericBinary("__umax");
    public readonly IntrinsicProcedure umaxp = IntrinsicBuilder.GenericBinary("__umaxp");
    public readonly IntrinsicProcedure umaxv = IntrinsicBuilder.GenericUnary("__umaxv");
    public readonly IntrinsicProcedure umin = IntrinsicBuilder.GenericBinary("__umin");
    public readonly IntrinsicProcedure uminp = IntrinsicBuilder.GenericBinary("__uminp");
    public readonly IntrinsicProcedure uminv = IntrinsicBuilder.GenericUnary("__uminv");
    public readonly IntrinsicProcedure umlal = IntrinsicBuilder.Pure("__umlal")
            .GenericTypes("TSrc", "TDst")
            .Params("TDst")
            .Params("TSrc")
            .Params("TSrc")
            .Returns("TDst");
    public readonly IntrinsicProcedure umlal2 = IntrinsicBuilder.Pure("__umlal2")
            .GenericTypes("TSrc", "TDst")
            .Params("TDst")
            .Params("TSrc")
            .Params("TSrc")
            .Returns("TDst");
    public readonly IntrinsicProcedure umlsl = IntrinsicBuilder.Pure("__umlsl")
            .GenericTypes("TSrc", "TDst")
            .Params("TDst")
            .Params("TSrc")
            .Params("TSrc")
            .Returns("TDst");
    public readonly IntrinsicProcedure umlsl2 = IntrinsicBuilder.Pure("__umlsl2")
            .GenericTypes("TSrc", "TDst")
            .Params("TDst")
            .Params("TSrc")
            .Params("TSrc")
            .Returns("TDst");
    public readonly IntrinsicProcedure umull2 = IntrinsicBuilder.GenericBinary("__umull2");
    public readonly IntrinsicProcedure uqadd = IntrinsicBuilder.GenericBinary("__uqadd");
    public readonly IntrinsicProcedure uqrshl = IntrinsicBuilder.GenericBinary("__uqrshl");
    public readonly IntrinsicProcedure uqrshrn = IntrinsicBuilder.GenericBinary("__uqrshrn");
    public readonly IntrinsicProcedure uqrshrn2 = IntrinsicBuilder.GenericBinary("__uqrshrn2");
    public readonly IntrinsicProcedure uqshl = IntrinsicBuilder.GenericBinary("__uqshl");
    public readonly IntrinsicProcedure uqshrn = IntrinsicBuilder.GenericBinary("__uqshrn");
    public readonly IntrinsicProcedure uqshrn2 = IntrinsicBuilder.GenericBinary("__uqshrn2");
    public readonly IntrinsicProcedure uqsub = IntrinsicBuilder.GenericBinary("__uqsub");
    public readonly IntrinsicProcedure uqxtn = IntrinsicBuilder.GenericUnary("__uqxtn");
    public readonly IntrinsicProcedure uqxtn2 = IntrinsicBuilder.GenericUnary("__uqxtn2");
    public readonly IntrinsicProcedure urecpe = IntrinsicBuilder.GenericUnary("__urecpe");
    public readonly IntrinsicProcedure urhadd = IntrinsicBuilder.GenericBinary("__urhadd");
    public readonly IntrinsicProcedure urshl = IntrinsicBuilder.GenericBinary("__urshl");
    public readonly IntrinsicProcedure urshr = IntrinsicBuilder.GenericBinary("__urshr");
    public readonly IntrinsicProcedure ursqrte = IntrinsicBuilder.GenericUnary("__ursqrte");
    public readonly IntrinsicProcedure ursra = IntrinsicBuilder.GenericBinary("__ursra");
    public readonly IntrinsicProcedure ushl = IntrinsicBuilder.GenericBinary("__ushl");
    public readonly IntrinsicProcedure ushll = IntrinsicBuilder.GenericBinary("__ushll");
    public readonly IntrinsicProcedure ushll2 = IntrinsicBuilder.GenericBinary("__ushll2");
    public readonly IntrinsicProcedure ushr = IntrinsicBuilder.GenericBinary("__ushr");
    public readonly IntrinsicProcedure usqadd = IntrinsicBuilder.GenericBinary("__usqadd");
    public readonly IntrinsicProcedure usra = IntrinsicBuilder.GenericBinary("__usra");
    public readonly IntrinsicProcedure usubl = IntrinsicBuilder.GenericBinary("__usubl");
    public readonly IntrinsicProcedure usubl2 = IntrinsicBuilder.GenericBinary("__usubl2");
    public readonly IntrinsicProcedure usubw = IntrinsicBuilder.GenericBinary("__usubw");
    public readonly IntrinsicProcedure usubw2 = IntrinsicBuilder.GenericBinary("__usubw2");
    public readonly IntrinsicProcedure uxtl = IntrinsicBuilder.GenericUnary("__uxtl");
    public readonly IntrinsicProcedure uxtl2 = IntrinsicBuilder.GenericUnary("__uxtl2");
    public readonly IntrinsicProcedure uzp1 = IntrinsicBuilder.GenericBinary("__uzp1");
    public readonly IntrinsicProcedure uzp2 = IntrinsicBuilder.GenericBinary("__uzp2");

    public readonly IntrinsicProcedure xtn = IntrinsicBuilder.GenericUnary("__xtn");
    public readonly IntrinsicProcedure xtn2 = IntrinsicBuilder.GenericUnary("__xtn2");

    public readonly IntrinsicProcedure zip1 = IntrinsicBuilder.GenericBinary("__zip1");
    public readonly IntrinsicProcedure zip2 = IntrinsicBuilder.GenericBinary("__zip2");

}
}
