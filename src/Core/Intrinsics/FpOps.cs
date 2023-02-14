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

using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Core.Intrinsics
{
    /// <summary>
    /// Floating-point intrinsics.
    /// </summary>

    public static class FpOps
    {
        public static readonly IntrinsicProcedure AcosGeneric = IntrinsicBuilder.GenericUnary("acos");
        public static readonly IntrinsicProcedure AsinGeneric = IntrinsicBuilder.GenericUnary("asin");
        public static readonly IntrinsicProcedure AtanGeneric = IntrinsicBuilder.GenericUnary("atan");
        public static readonly IntrinsicProcedure AtanhGeneric = IntrinsicBuilder.GenericUnary("atanh");
        public static readonly IntrinsicProcedure CosGeneric = IntrinsicBuilder.GenericUnary("cos");
        public static readonly IntrinsicProcedure CoshGeneric = IntrinsicBuilder.GenericUnary("cosh");
        public static readonly IntrinsicProcedure ExpGeneric = IntrinsicBuilder.GenericUnary("exp");
        public static readonly IntrinsicProcedure FAbs64 = IntrinsicBuilder.Unary("fabs", PrimitiveType.Real64);
        public static readonly IntrinsicProcedure FAbs32 = IntrinsicBuilder.Unary("fabsf", PrimitiveType.Real32);
        public static readonly IntrinsicProcedure FMax32 = IntrinsicBuilder.Binary("fmaxf", PrimitiveType.Real32);
        public static readonly IntrinsicProcedure FMax64 = IntrinsicBuilder.Binary("fmax", PrimitiveType.Real64);
        public static readonly IntrinsicProcedure FMin32 = IntrinsicBuilder.Binary("fminf", PrimitiveType.Real32);
        public static readonly IntrinsicProcedure FMin64 = IntrinsicBuilder.Binary("fmin", PrimitiveType.Real64);
        public static readonly IntrinsicProcedure FModGeneric = IntrinsicBuilder.GenericBinary("fmod");
        public static readonly IntrinsicProcedure FRemGeneric = IntrinsicBuilder.GenericBinary("frem");
        public static readonly IntrinsicProcedure IsUnordered_f32 = IntrinsicBuilder.Predicate("isunordered", PrimitiveType.Real32, PrimitiveType.Real32);
        public static readonly IntrinsicProcedure IsUnordered_f64 = IntrinsicBuilder.Predicate("isunordered", PrimitiveType.Real64, PrimitiveType.Real64);
        public static readonly IntrinsicProcedure Log10Generic = IntrinsicBuilder.GenericUnary("log10");
        public static readonly IntrinsicProcedure Log2Generic = IntrinsicBuilder.GenericUnary("log2");
        public static readonly IntrinsicProcedure LogGeneric = IntrinsicBuilder.GenericUnary("log");
        public static readonly IntrinsicProcedure SinGeneric = IntrinsicBuilder.GenericUnary("sin");
        public static readonly IntrinsicProcedure SqrtGeneric = IntrinsicBuilder.GenericUnary("sqrt");
        public static readonly IntrinsicProcedure TanGeneric = IntrinsicBuilder.GenericUnary("tan");
        public static readonly IntrinsicProcedure TanhGeneric = IntrinsicBuilder.GenericUnary("tanh");
        public static readonly IntrinsicProcedure TruncGeneric = IntrinsicBuilder.GenericUnary("trunc");
        //$REVIEW: math.h
        public static readonly IntrinsicProcedure ceil = IntrinsicBuilder.Unary("ceil", PrimitiveType.Real64);
        public static readonly IntrinsicProcedure ceilf = IntrinsicBuilder.Unary("ceilf", PrimitiveType.Real32);
        public static readonly IntrinsicProcedure fabs = IntrinsicBuilder.Unary("fabs", PrimitiveType.Real64);
        public static readonly IntrinsicProcedure fabsf = IntrinsicBuilder.Unary("fabsf", PrimitiveType.Real32);
        public static readonly IntrinsicProcedure floor = IntrinsicBuilder.Unary("floor", PrimitiveType.Real64);
        public static readonly IntrinsicProcedure floorf = IntrinsicBuilder.Unary("floorf", PrimitiveType.Real32);
        public static readonly IntrinsicProcedure fmax = IntrinsicBuilder.Binary("fmax", PrimitiveType.Real64);
        public static readonly IntrinsicProcedure fmaxf = IntrinsicBuilder.Binary("fmaxf", PrimitiveType.Real32);
        public static readonly IntrinsicProcedure fmin = IntrinsicBuilder.Binary("fmin", PrimitiveType.Real64);
        public static readonly IntrinsicProcedure fminf = IntrinsicBuilder.Binary("fminf", PrimitiveType.Real32);
        public static readonly IntrinsicProcedure rint = IntrinsicBuilder.Unary("rint", PrimitiveType.Real64);
        public static readonly IntrinsicProcedure rintf = IntrinsicBuilder.Unary("rintf", PrimitiveType.Real32);
        public static readonly IntrinsicProcedure round = IntrinsicBuilder.Unary("round", PrimitiveType.Real64);
        public static readonly IntrinsicProcedure roundf = IntrinsicBuilder.Unary("roundf", PrimitiveType.Real32);
        public static readonly IntrinsicProcedure sqrt = IntrinsicBuilder.Unary("sqrt", PrimitiveType.Real64);
        public static readonly IntrinsicProcedure sqrtf = IntrinsicBuilder.Unary("sqrtf", PrimitiveType.Real32);
        public static readonly IntrinsicProcedure trunc = IntrinsicBuilder.Unary("trunc", PrimitiveType.Real64);
        public static readonly IntrinsicProcedure truncf = IntrinsicBuilder.Unary("truncf", PrimitiveType.Real32);

    }
}
