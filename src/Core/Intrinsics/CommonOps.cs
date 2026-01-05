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

using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;

namespace Reko.Core.Intrinsics
{
    /// <summary>
    /// Common intrinsics.
    /// </summary>
    public static class CommonOps
    {
#pragma warning disable CS1591

        // Bit manipulation
        public static readonly IntrinsicProcedure Bit = new IntrinsicBuilder("__bit", false)
            .GenericTypes("TData", "TBit")
            .Param("TData").Param("TBit")
            .Returns(PrimitiveType.Bool);
        public static readonly IntrinsicProcedure ClearBit = new IntrinsicBuilder("__clear_bit", false)
            .GenericTypes("TData", "TBit")
            .Param("TData").Param("TBit")
            .Returns("TData");
        public static readonly IntrinsicProcedure CountLeadingOnes = new IntrinsicBuilder("__count_leading_ones", false)
            .GenericTypes("T")
            .Param("T")
            .Returns("T");
        public static readonly IntrinsicProcedure CountLeadingZeros = new IntrinsicBuilder("__count_leading_zeros", false)
            .GenericTypes("T")
            .Param("T")
            .Returns("T");
        public static readonly IntrinsicProcedure CountTrailingOnes = new IntrinsicBuilder("__count_trailing_ones", false)
            .GenericTypes("T")
            .Param("T")
            .Returns("T");
        public static readonly IntrinsicProcedure CountTrailingZeros = new IntrinsicBuilder("__count_trailing_zeros", false)
            .GenericTypes("T")
            .Param("T")
            .Returns("T");
        public static readonly IntrinsicProcedure FindFirstOne = new IntrinsicBuilder("__find_first_one", false)
            .GenericTypes("T")
            .Param("T")
            .Returns("T");
        public static readonly IntrinsicProcedure FindFirstZero = new IntrinsicBuilder("__find_first_zero", false)
            .GenericTypes("T")
            .Param("T")
            .Returns("T");
        public static readonly IntrinsicProcedure FindLastOne = new IntrinsicBuilder("__find_last_one", false)
            .GenericTypes("T")
            .Param("T")
            .Returns("T");
        public static readonly IntrinsicProcedure InvertBit = new IntrinsicBuilder("__invert_bit", false)
            .GenericTypes("TData", "TBit")
            .Param("TData").Param("TBit")
            .Returns("TData");
        public static readonly IntrinsicProcedure PopCount = new IntrinsicBuilder("__population_count", false)
            .GenericTypes("T")
            .Param("T")
            .Returns("T");
        public static readonly IntrinsicProcedure ReinterpretCast = new IntrinsicBuilder("reinterpret_cast", false)
            .GenericTypes("T")
            .Param(new UnknownType())
            .Returns("T");
        public static readonly IntrinsicProcedure ReverseBits = IntrinsicBuilder.GenericUnary("__reverse_bits");
        public static readonly IntrinsicProcedure SetBit = new IntrinsicBuilder("__set_bit", false)
            .GenericTypes("TData", "TBit")
            .Param("TData").Param("TBit")
            .Returns("TData");
        public static readonly IntrinsicProcedure WriteBit = new IntrinsicBuilder("__write_bit", false)
            .GenericTypes("TData", "TBit")
            .Param("TData").Param("TBit").Param(PrimitiveType.Bool)
            .Returns("TData");

        // Byte manipulations
        public static readonly IntrinsicProcedure ReverseBytes = IntrinsicBuilder.GenericUnary("__reverse_bytes");
        public static readonly IntrinsicProcedure ReverseHalfwords = IntrinsicBuilder.GenericUnary("__reverse_word16s");
        public static readonly IntrinsicProcedure ReverseWords = IntrinsicBuilder.GenericUnary("__reverse_word32s");

        // Integer math
        //$REVIEW: generate a reference to math.h
        public static readonly IntrinsicProcedure Abs = IntrinsicBuilder.GenericUnary("abs", EvalAbs);
        public static readonly IntrinsicProcedure Max = IntrinsicBuilder.GenericBinary("max", EvalMax); //$REVIEW: math.h
        public static readonly IntrinsicProcedure Min = IntrinsicBuilder.GenericBinary("min"); //$REVIEW: math.h

        private static Constant EvalAbs(DataType dt, params Constant[] inputs)
        {
            if (inputs.Length != 1)
                throw new ArgumentException(nameof(inputs), "One argument is required for 'abs'.");
            if ((dt.Domain & (Domain.SignedInt|Domain.UnsignedInt)) != 0)
            {
                int iSignbit = dt.BitSize - 1;
                ulong signExtended = Bits.SignExtend(inputs[0].ToUInt64(), dt.BitSize);
                ulong allOnesIfSigned = (ulong) ((long) signExtended >> iSignbit);
                ulong result =
                    (signExtended & ~allOnesIfSigned) |
                    ((0-signExtended) & allOnesIfSigned);
                return Constant.Create(dt, result);
            }
            throw new NotImplementedException($"abs not implemented for {dt} yet.");
        }

        private static Constant EvalMax(DataType dt, params Constant[] inputs)
        {
            if (inputs.Length == 0)
                throw new ArgumentException(nameof(inputs), "Must have at least one input parameter to EvalMax.");
                var mask = Bits.Mask(0, dt.BitSize);
            if (dt.Domain == Domain.SignedInt)
            {
                long max = (long) Bits.SignExtend((ulong) inputs[0].ToInt64(), dt.BitSize);
                for (int i = 1; i < inputs.Length; ++i)
                {
                    var input = (long) Bits.SignExtend((ulong) inputs[i].ToInt64(), dt.BitSize);
                    max = Math.Max(max, input);
                }
                return Constant.Create(dt, max);
            }
            else
            {
                ulong max = inputs[0].ToUInt64() & mask;
                for (int i = 1; i < inputs.Length; ++i)
                {
                    var input = inputs[i].ToUInt64() & mask;
                    max = Math.Max(max, input);
                }
                return Constant.Create(dt, max);
            }
        }

        // Rotations
        public static readonly IntrinsicProcedure Rol = new IntrinsicBuilder("__rol", false)
            .GenericTypes("TData", "TCount")
            .Param("TData").Param("TCount")
            .Returns("TData");
        public static readonly IntrinsicProcedure RolC = new IntrinsicBuilder("__rcl", false)
            .GenericTypes("TData", "TCount")
            .Param("TData").Param("TCount").Param(PrimitiveType.Bool)
            .Returns("TData");
        public static readonly IntrinsicProcedure Ror = new IntrinsicBuilder("__ror", false)
            .GenericTypes("TData", "TCount")
            .Param("TData").Param("TCount")
            .Returns("TData");
        public static readonly IntrinsicProcedure RorC = new IntrinsicBuilder("__rcr", false)
            .GenericTypes("TData", "TCount")
            .Param("TData").Param("TCount").Param(PrimitiveType.Bool)
            .Returns("TData");

        // Overflow
        public static readonly IntrinsicProcedure Overflow = new IntrinsicBuilder("OVERFLOW", false)
            .GenericTypes("T")
            .Param("T")
            .Returns(PrimitiveType.Bool);

        // Saturated arithmetic
        public static readonly IntrinsicProcedure SatAdd = IntrinsicBuilder.GenericBinary("__sat_add");
        public static readonly IntrinsicProcedure SatSub = IntrinsicBuilder.GenericBinary("__sat_sub");
        public static readonly IntrinsicProcedure SatMul = IntrinsicBuilder.GenericBinary("__sat_mul");

        // Halt the processor
        public static readonly IntrinsicProcedure Halt = new IntrinsicBuilder(
            "__halt", true, new ProcedureCharacteristics
            {
                Terminates = true,
            }).Void();
        public static readonly IntrinsicProcedure Halt_1 = new IntrinsicBuilder(
            "__halt", true, new ProcedureCharacteristics
            {
                Terminates = true,
            })
            .GenericTypes("T")
            .Param("T")
            .Void();

        public static readonly IntrinsicProcedure Strlen = new IntrinsicBuilder("strlen", false)
            .PtrParam(PrimitiveType.Char)
            .Returns(PrimitiveType.UInt32);
        public static readonly IntrinsicProcedure Memcpy = new IntrinsicBuilder("memcpy", true)
            .PtrParam(new UnknownType())
            .PtrParam(new UnknownType())
            .Param(PrimitiveType.UInt32)
            .PtrReturns(new UnknownType());
        public static readonly IntrinsicProcedure Memcmp = new IntrinsicBuilder("memcmp", true)
            .PtrParam(new UnknownType())
            .PtrParam(new UnknownType())
            .Param(PrimitiveType.UInt32)
            .Returns(PrimitiveType.Int32);
        public static readonly IntrinsicProcedure Strcpy = new IntrinsicBuilder("strcpy", true)
            .PtrParam(PrimitiveType.Char)
            .PtrParam(PrimitiveType.Char)
            .PtrReturns(PrimitiveType.Char);

        // System calls.
        public static readonly IntrinsicProcedure Syscall = new IntrinsicBuilder("__syscall", true)
            .Void();

        // __syscall overloads

        /// <summary>
        /// System call with no arguments.
        /// </summary>
        public static readonly IntrinsicProcedure Syscall_0 = new IntrinsicBuilder("__syscall", true)
            .Void();

        /// <summary>
        /// System call with one argument.
        /// </summary>
        public static readonly IntrinsicProcedure Syscall_1 = new IntrinsicBuilder("__syscall", true)
            .GenericTypes("T")
            .Param("T")
            .Void();

        /// <summary>
        /// System call with two arguments.
        /// </summary>
        public static readonly IntrinsicProcedure Syscall_2 = new IntrinsicBuilder("__syscall", true)
            .GenericTypes("T1","T2")
            .Param("T1")
            .Param("T2")
            .Void();

        // Memory cache control
        public static readonly IntrinsicProcedure Prefetch = new IntrinsicBuilder("__prefetch", true)
            .GenericTypes("T")
            .PtrParam("T")
            .Void();
        public static readonly IntrinsicProcedure PrefetchInstruction = new IntrinsicBuilder("__prefetch_instruction", true)
            .GenericTypes("T")
            .PtrParam("T")
            .Void();
    }
}
