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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core.Intrinsics
{
    /// <summary>
    /// Common Single-Instruction, Multiple Data instructions.
    /// </summary>
    public static class Simd
    {
        public static readonly IntrinsicProcedure Add = IntrinsicBuilder.SimdBinary("__simd_add", Operator.IAdd);
        public static readonly IntrinsicProcedure FAdd = IntrinsicBuilder.SimdBinary("__simd_fadd", Operator.FAdd);
        public static readonly IntrinsicProcedure FDiv = IntrinsicBuilder.GenericBinary("__simd_fdiv");
        public static readonly IntrinsicProcedure FMul = IntrinsicBuilder.SimdBinary("__simd_fmul", Operator.FMul);
        public static readonly IntrinsicProcedure FSub = IntrinsicBuilder.SimdBinary("__simd_fsub", Operator.FSub);
        public static readonly IntrinsicProcedure Max = IntrinsicBuilder.GenericBinary("__simd_max");
        public static readonly IntrinsicProcedure Min = IntrinsicBuilder.GenericBinary("__simd_min");
        public static readonly IntrinsicProcedure Mul = IntrinsicBuilder.GenericBinary("__simd_mul");
        public static readonly IntrinsicProcedure Not = IntrinsicBuilder.GenericUnary("__simd_not");
        public static readonly IntrinsicProcedure Sub = IntrinsicBuilder.GenericBinary("__simd_add");
    }
}
