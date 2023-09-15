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

using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Types;
using System;
using System.Diagnostics;

namespace Reko.Core.Intrinsics
{
    /// <summary>
    /// This class represends a Single Instruction Multiple Data machine instruction,
    /// which performs parallel operations on data.
    /// </summary>
    /// <remarks>
    /// One particular feature of a SIMD instruction is that if it sliced across 
    /// a single lane, the results can be simplified to operations like IAdd, IMul
    /// etc. This may aid in better results when decompiling code emitted by compilers
    /// that use SIMD even for scalar values.
    /// </remarks>
    public class SimdIntrinsic : IntrinsicProcedure
    {
        public SimdIntrinsic(
            string name,
            Operator op,
            DataType[] genericTypes,
            bool isConcrete,
            FunctionType signature) :
            base(name, genericTypes, isConcrete, false, signature)
        {
            this.Operator = op;
        }

        public int LaneCount { get; }
        public Operator Operator { get; }

        protected override IntrinsicProcedure DoMakeInstance(DataType[] concreteTypes, FunctionType sig)
        {
            return new SimdIntrinsic(
                this.Name,
                this.Operator,
                concreteTypes,
                true,
                sig)
            { 
                Characteristics = this.Characteristics,
                EnclosingType = this.EnclosingType
            };
        }

        public DataType InputLaneType(int iParameter)
        {
            throw new NotImplementedException();
        }

        public DataType OutputLaneType()
        {
            var laneType = ((ArrayType) base.Signature.ReturnValue!.DataType).ElementType;
            return laneType;
        }

        public Expression MakeSlice(Expression[] arguments, int lane)
        {
            //$TODO: n-ary? intrinsics like max, sqrt?
            var laneType = ((ArrayType) base.Signature.Parameters![0].DataType).ElementType;
            Debug.Assert(this.Operator is BinaryOperator, "Slicing non-binary SIMD operators not implemented yet");
            var left = new Slice(laneType, arguments[0], lane * laneType.BitSize);
            var right = new Slice(laneType, arguments[1], lane * laneType.BitSize);
            return new BinaryExpression((BinaryOperator)Operator, laneType, left, right);
        }
    }
}
