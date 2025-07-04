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

using System;
using Reko.Core.Analysis;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Code;
using Reko.Core;
using System.Linq;
using Reko.Core.Lib;

namespace Reko.Analysis
{
    /// <summary>
    /// Undoes the transformation compilers sometimes do on integer constant
    /// divisions.
    /// </summary>
    /// <remarks>
    /// Compilers may transform a division 
    /// <code>
    /// a = b / c 
    /// </code>
    /// where c is a (small) integer constant into the corresponding
    /// <code>
    /// hi:lo = b * d 
    /// a = hi
    /// a = a >> sh
    /// </code>
    /// for speed. This transformation undoes such transformations into
    /// multiplications by a rational.
    /// </remarks>
    public class ConstDivisionImplementedByMultiplication
    {
        private readonly SsaState ssa;
        private readonly ExpressionEmitter m;
        private Identifier? idDst;
        private Expression? dividend;
        private Rational bestRational;
        private Identifier? idOrig;

        /// <summary>
        /// Creates an instance of the <see cref="ConstDivisionImplementedByMultiplication"/> class.
        /// </summary>
        /// <param name="ssa"><see cref="SsaState"/> of the procedure to be transformed.</param>
        public ConstDivisionImplementedByMultiplication(SsaState ssa)
        {
            this.ssa = ssa;
            this.m = new ExpressionEmitter();
        }

        /// <summary>
        /// Replaces occurrences of multiplications with constants with
        /// their corresponding division equivalents.
        /// </summary>
        public void Transform()
        {
            foreach (var stm in ssa.Procedure.Statements)
            {
                if (Match(stm.Instruction))
                {
                    TransformInstruction();
                }
            }
        }

        /// <summary>
        /// Find the best rational that approximates the fraction 
        /// </summary>
        /// <param name="factor">Divisor &lt;1 1 scaled by 2^32.</param>
        /// <returns></returns>
        public static Rational FindBestRational(uint factor)
        {
            return Rational.FromDouble(factor * Math.Pow(2.0, -32));
        }

        /// <summary>
        /// Determine whether <paramref name="instr"/> is a match for 
        /// the pattern that is a constant division
        /// </summary>
        /// <remarks>
        /// The pattern is:
        /// <code>
        /// hi:lo = r * const
        /// hi = slice(hi:lo) (alias)
        /// hi = hi >> shift  (optional)
        /// </code>
        /// This transformation must be carried out before value propagation
        /// pushes the shift inside other operations.
        /// </remarks>
        /// <param name="instr"></param>
        /// <returns></returns>
        public bool Match(Instruction instr)
        {
            // Look for hi:lo = a * C
            if (instr is not Assignment ass ||
                ass.Dst.Storage is not SequenceStorage dst ||
                dst.Elements.Length != 2 ||
                ass.Src is not BinaryExpression bin ||
                bin.Operator is not IMulOperator ||
                bin.Right is not Constant cRight ||
                ass.Dst.DataType.Size <= bin.Left.DataType.Size)
            {
                return false;
            }

            this.idOrig = ass.Dst;
            this.idDst = FindAlias(ass.Dst, dst.Elements[0]);
            if (idDst is null)
                return false;

            var best = FindBestRational(cRight.ToUInt32());

            // There may be a subsequent SAR / SHR to increase 
            // the divisor.

            var idSlice = idDst;
            Constant? rShift;
            if (idSlice is not null)
            {
                rShift = FindShiftUse(idSlice);
                if (rShift is not null)
                {
                    best /= (1 << rShift.ToInt32());
                }
            }
            this.bestRational = best;

            this.dividend = bin.Left;
            return true;
        }

        private Identifier? FindAlias(Identifier id, Storage regHead)
        {
            return (ssa.Identifiers[id].Uses
                .Select(u => u.Instruction)
                .OfType<AliasAssignment>()
                .Where(a => a.Dst.Storage == regHead)
                .Select(a => a.Dst)
                .FirstOrDefault());
        }

        private Constant? FindShiftUse(Identifier idSlice)
        {
            return ssa.Identifiers[idSlice]
                .Uses.Select(u => u.Instruction)
                .OfType<Assignment>()
                .Select(a =>
                {
                    if (a.Src is BinaryExpression b &&
                        b.Left == idSlice &&
                        (b.Operator.Type == OperatorType.Sar || b.Operator.Type == OperatorType.Shr))
                    {
                        return b.Right as Constant;
                    }
                    else
                    {
                        return null;
                    }
                })
                .Where(x => x is not null)
                .FirstOrDefault();
        }

        /// <summary>
        /// Transforms the current instruction into an assignment operation that divides the dividend by the best
        /// rational denominator, applying the numerator as a multiplier if necessary.
        /// </summary>
        /// <remarks>This method modifies the SSA (Static Single Assignment) identifiers by updating the
        /// use and definition relationships between the source and destination identifiers. The resulting assignment
        /// operation is returned for further processing or analysis.</remarks>
        /// <returns>An <see cref="Assignment"/> object representing the transformed instruction, where the dividend is divided
        /// by the best rational denominator.</returns>
        public Assignment TransformInstruction()
        {
            var eNum = dividend!;
            if (bestRational.Numerator != 1)
            {
                eNum = m.IMul(
                    eNum,
                    Constant.Int32((int)bestRational.Numerator));
            }
            var sidOrig = ssa.Identifiers[idOrig!];
            var sidDst = ssa.Identifiers[idDst!];
            sidOrig.Uses.Remove(sidDst.DefStatement);
            var ass = new Assignment(
                idDst!,
                m.SDiv(
                    eNum,
                    Constant.Int32((int)bestRational.Denominator)));

            sidDst.DefStatement.Instruction = ass;
            return ass;
        }
    }
}