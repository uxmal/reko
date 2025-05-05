#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Operators;
using Reko.Core.Types;
using System.Numerics;

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
        /// <summary>
        /// Constructs a SIMD intrinsic.
        /// </summary>
        /// <param name="name">Name of the function to create.</param>
        /// <param name="laneOp">Operation for each lane.</param>
        /// <param name="genericTypes">Generic types used.</param>
        /// <param name="isConcrete">True if this instance is concrete.</param>
        /// <param name="signature">Type signature.</param>
        public SimdIntrinsic(
            string name,
            IFunctionalUnit laneOp,
            DataType[] genericTypes,
            bool isConcrete,
            FunctionType signature) :
            base(name, genericTypes, isConcrete, false, null, signature)
        {
            this.Operator = laneOp;
        }

        /// <summary>
        /// Operator used for each lane.
        /// </summary>
        public IFunctionalUnit Operator { get; }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public override Constant? ApplyConstants(DataType dt, params Constant[] cs)
        {
            return ApplyConstants(this.Operator, dt, cs);
        }

        /// <summary>
        /// Apply the given operation to the given constants, repaeating across all lanes.
        /// </summary>
        /// <param name="operation">Operation to perform.</param>
        /// <param name="dt">Size of a lane result.</param>
        /// <param name="cs">Constant inputs for each lane.</param>
        /// <returns>Possible constant result.</returns>
        public Constant? ApplyConstants(
            IFunctionalUnit operation,
            DataType dt,
            params Constant[] cs)
        {
            var dtInput = cs[0].DataType;
            var bitsInput = dtInput.BitSize;
            var dtInputLane = this.InputLaneType(0);    //$REVIEW: do these vary by input #?
            var dtOutputLane = this.OutputLaneType();
            var bitsInputLane = dtInputLane .BitSize;
            var maskInputLane = (BigInteger.One << bitsInputLane) - 1;
            var laneInputs = new Constant[cs.Length];
            int cLanes = bitsInput / bitsInputLane;
            var output = BigInteger.Zero;
            var laneOutputs = new Constant[cLanes];
            var laneMask = Bits.Mask(0, bitsInputLane);
            for (int iLane = cLanes - 1; iLane >= 0; --iLane)
            {
                for (int i = 0; i < cs.Length; ++i)
                {
                    int sh = iLane * bitsInputLane;
                    laneInputs[i] = Constant.Create(dtInputLane, (cs[i].ToBigInteger() >> sh) & maskInputLane);
                }
                var laneResult = operation.ApplyConstants(dtOutputLane, laneInputs);
                if (laneResult is null)
                    return null;
                output <<= dtOutputLane.BitSize;
                output |= laneResult.ToUInt64() & laneMask;
            }
            return Constant.Create(dt, output);
        }

        /// <inheritdoc/>
        public override Expression Create(DataType dt, params Expression[] exprs)
        {
            return base.Create(dt, exprs);
        }

        /// <summary>
        /// Returns the type of the input lane for the given parameter.
        /// </summary>
        /// <param name="iParameter">0-based index of the paramter.</param>
        /// <returns>Data type for the input lane of the parameter.
        /// </returns>
        public DataType InputLaneType(int iParameter)
        {
            var arrayType = (ArrayType) Signature.Parameters![iParameter].DataType;
            return arrayType.ElementType;
        }

        /// <summary>
        /// Returns the type of the output lane for the given parameter.
        /// </summary>
        /// <returns>Data type for the output lane of the parameter.
        /// </returns>
        public DataType OutputLaneType()
        {
            var laneType = ((ArrayType) base.Signature.ReturnValue!.DataType).ElementType;
            return laneType;
        }

        /// <summary>
        /// Slices a SIMD intrinsic into one of its component lanes.
        /// </summary>
        /// <param name="arguments">Arguments to be sliced.</param>
        /// <param name="lane">Lane number.</param>
        /// <returns>Sliced arguments.
        /// </returns>
        public Expression MakeSlice(Expression[] arguments, int lane)
        {
            var laneType = ((ArrayType) base.Signature.Parameters![0].DataType).ElementType;
            var slices = new Expression[arguments.Length];
            for (int i = 0; i < arguments.Length; ++i)
            {
                slices[i] = new Slice(laneType, arguments[i], lane * laneType.BitSize);
            }
            return Operator.Create(laneType, slices);
        }
    }
}
