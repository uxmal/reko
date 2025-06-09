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
using Reko.Core.Types;
using System;

namespace Reko.Evaluation
{
    public partial class ExpressionSimplifier
    {
        public virtual (Expression, bool) VisitConversion(Conversion conversion)
        {
            var (exp, changed) = conversion.Expression.Accept(this);
            if (exp is not InvalidConstant)
            {
                var ptCvt = conversion.DataType;
                var ptSrc = conversion.SourceDataType;
                if (exp is Constant c && ptCvt is not null)
                {
                    if (ptCvt.Domain == Domain.Real)
                    {
                        if (ptSrc.Domain == Domain.Real)
                        {
                            if (ptCvt.Size < ptSrc.Size)
                            {
                                // Real-to-real conversion.
                                return (ConstantReal.Create(ptCvt, c.ToReal64()), true);
                            }
                        }
                        else if (ptSrc.IsWord)
                        {
                            // Raw bit pattern reinterpretation.
                            return (ReinterpretBitsAsReal(ptCvt, c), true);
                        }
                        else
                        {
                            // integer to real conversion
                            return (ConstantReal.Create(ptCvt, c.ToInt64()), true);
                        }
                    }
                    else if ((ptSrc.Domain & Domain.Integer) != 0)
                    {
                        if (ptSrc.Domain == Domain.SignedInt)
                        {
                            return (Constant.Create(ptCvt, c.ToInt64()), true);
                        }
                        else if (ptSrc.Domain.HasFlag(Domain.SignedInt))
                        {
                            return (Constant.Create(ptCvt, c.ToUInt64()), true);
                        }
                    }
                    else if (ptSrc.Domain == Domain.Boolean)
                        {
                            return (Constant.Create(ptCvt, c.ToUInt64()), true);
                        }
                }
                if (exp is Identifier id &&
                    ctx.GetDefiningExpression(id) is MkSequence seq)
                {
                    // If we are converting a SEQ, and the corresponding element is >= 
                    // the size of the cast, then use deposited part directly.
                    var lsbElem = seq.Expressions[^1];
                    int sizeDiff = lsbElem.DataType.Size - conversion.DataType.Size;
                    if (sizeDiff >= 0)
                    {
                        if (sizeDiff > 0)
                        {
                            return (new Conversion(lsbElem, lsbElem.DataType, conversion.DataType), true);
                        }
                        else
                        {
                            return (lsbElem, true);
                        }
                    }
                }
                if (exp is ProcedureConstant pc && conversion.DataType.BitSize == pc.DataType.BitSize)
                {
                    // (wordnn) procedure_const => procedure_const
                    return (pc, true);
                }
                if (exp.DataType.BitSize == conversion.DataType.BitSize)
                {
                    // Redundant word-casts can be stripped.
                    if (conversion.DataType.IsWord)
                    {
                        return (exp, true);
                    }
                    conversion = m.Convert(exp, exp.DataType, conversion.DataType);
                }
                else
                {
                    if (conversion.SourceDataType.BitSize == exp.DataType.BitSize)
                    {
                        conversion = m.Convert(exp, conversion.SourceDataType, conversion.DataType);
                    }
                    else
                    {
                        conversion = m.Convert(exp, exp.DataType, conversion.DataType);
                    }
                }
            }
            exp = convertConvertRule.Match(conversion);
            if (exp is not null)
            {
                return (exp, true);
            }
            return (conversion, changed);
        }

        /// <summary>
        /// Take a bitvector of type wordXXX and reinterpret it as a floating-point
        /// constant.
        /// </summary>
        /// <param name="ptCast">Floating-point type to which the raw bits are being cast.</param>
        /// <param name="rawBits">The raw bits being cast.</param>
        /// <returns>A floating-point constant, possibly with a <see cref="Cast"/> wrapped around it
        /// if the constant is not 32- or 64-bit.
        /// </returns>
        private Expression ReinterpretBitsAsReal(DataType ptCast, Constant rawBits)
        {
            var bitSize = Math.Min(rawBits.DataType.BitSize, 64);
            var dtImm = PrimitiveType.Create(Domain.Real, bitSize);
            var cImm = Constant.RealFromBitpattern(dtImm, rawBits);
            cImm = ConstantReal.Create(dtImm, cImm.ToReal64());
            if (cImm.DataType.BitSize == ptCast.BitSize)
            {
                return cImm;
            }
            else
            {
                return new Conversion(cImm, cImm.DataType, ptCast);
            }
        }

    }
}
