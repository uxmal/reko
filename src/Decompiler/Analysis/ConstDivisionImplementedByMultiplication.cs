#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Code;
using Reko.Core;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

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
        private Identifier idDst;
        private Expression dividend;
        private SsaState ssa;
        private Rational bestRational;
        private Identifier idOrig;

        public ConstDivisionImplementedByMultiplication(SsaState ssa)
        {
            this.ssa = ssa;
        }

        public void Transform()
        {
            foreach (var stm in ssa.Procedure.Statements)
            {
                if (Match(stm.Instruction))
                {
                    stm.Instruction = TransformInstruction();
                }
            }
        }

        /// <summary>
        /// Find the best rational that approximates the fraction 
        /// </summary>
        /// <param name="factor"></param>
        /// <returns></returns>
        public Rational FindBestRational(uint factor)
        {
            double fraction = (double)factor * Math.Pow(2.0, -32);
            return GetMediantSequence(fraction)
                .TakeWhile(x => x.Denominator <= 1000)
                .OrderBy(x => Math.Abs(x.ToDouble() - fraction))
                .FirstOrDefault();
        }

        /// <summary>
        /// Returns a sequence of rationals that brackets the specified 
        /// fraction. Mediants are used to construct Stern-Brocot trees
        /// (https://en.wikipedia.org/wiki/Stern%E2%80%93Brocot_tree)
        /// to locate the closest rational approximantion to a floating 
        /// point number.
        /// </summary>
        /// <param name="fraction"></param>
        /// <returns></returns>
        private IEnumerable<Rational> GetMediantSequence(double fraction)
        { 
            Debug.Assert(fraction > 0);
            Debug.Assert(fraction < 1);
            var lower = Rational.FromIntegers(0, 1);
            var upper = Rational.FromIntegers(1, 1);
            while (true)
            {
                var mediant = Rational.FromIntegers(
                    lower.Numerator + upper.Numerator,
                    lower.Denominator + upper.Denominator);
                //Debug.Print("mediant: {0}", mediant);
                yield return mediant;
                var approx = mediant.ToDouble();
                if (fraction < approx)
                    upper = mediant;
                else if (fraction > approx)
                    lower = mediant;
                else
                    break;
            }
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
            Assignment ass;
            SequenceStorage dst;
            BinaryExpression bin;
            Constant cRight;

            // Look for hi:lo = a * C
            if (!instr.As(out ass) ||
                !ass.Dst.Storage.As(out dst) ||
                !ass.Src.As(out bin) ||
                !(bin.Operator is IMulOperator) ||
                !bin.Right.As(out cRight) ||
                ass.Dst.DataType.Size <= bin.Left.DataType.Size)
            {
                return false;
            }

            this.idOrig = ass.Dst;
            this.idDst = FindAlias(ass.Dst, dst.Head);
            if (idDst == null)
                return false;

            var best = FindBestRational(cRight.ToUInt32());

            // There may be a subsequent SAR / SHR to increase 
            // the divisor.

            var idSlice = idDst;
            Constant rShift = null;
            if (idSlice != null)
            {
                rShift = FindShiftUse(idSlice);
                if (rShift != null)
                {
                    best = best / (1 << rShift.ToInt32());
                }
            }
            this.bestRational = best;

            this.dividend = bin.Left;
            return true;
        }

        private Identifier FindAlias(Identifier id, Storage idHead)
        {
            return (ssa.Identifiers[id].Uses
                .Select(u => u.Instruction)
                .OfType<AliasAssignment>()
                .Where(a => a.Dst.Storage == idHead)
                .Select(a => a.Dst)
                .FirstOrDefault());
        }

        private Constant FindShiftUse(Identifier idSlice)
        {
            return ssa.Identifiers[idSlice]
                .Uses.Select(u => u.Instruction)
                .OfType<Assignment>()
                .Select(a =>
                {
                    var b = a.Src as BinaryExpression;
                    if (b == null ||
                        b.Left != idSlice ||
                        (b.Operator != Operator.Sar &&
                         b.Operator != Operator.Shr))
                        return null;
                    return b.Right as Constant;
                })
                .Where(x => x != null)
                .FirstOrDefault();
        }

        private Identifier FindSliceUse(Identifier id)
        {
            return ssa.Identifiers[id]
                .Uses.Select(u => u.Instruction)
                .OfType<Assignment>()
                .Select(a =>
                {
                    var s = a.Src as Slice;
                    if (s == null || s.Expression != id)
                        return null;
                    else
                        return a.Dst;
                })
                .Where(x => x != null)
                .FirstOrDefault();
        }

        public Assignment TransformInstruction()
        {
            var eNum = dividend;
            if (bestRational.Numerator != 1)
            {
                eNum = new BinaryExpression(
                    Operator.IMul,
                    eNum.DataType,
                    eNum,
                    Constant.Int32((int)bestRational.Numerator));
            }
            var sidOrig = ssa.Identifiers[idOrig];
            var sidDst = ssa.Identifiers[idDst];
            sidOrig.Uses.Remove(sidDst.DefStatement);
            sidDst.DefStatement.Instruction = new Assignment(
                idDst,
                new BinaryExpression(
                    Operator.SDiv,
                    eNum.DataType,
                    eNum,
                    Constant.Int32((int)bestRational.Denominator)));
            return sidDst.DefStatement.Instruction as Assignment;
        }

        public static Rational ContinuedFraction(double x)
        {
            Debug.Assert(0 <= x && x < 1.0);
            int scale = 1;
            while (x < 0.5)
            {
                x *= 2.0;
                scale *= 2;
            }
            var a = new List<int>();
            while (x > 1e-6 && a.Count < 10)
            {
                var r = 1.0 / x;
                int a1 = (int)r;
                x = r - a1;
                a.Add(a1);
            }
            Debug.Print("cfrac [{0}]", string.Join(", ", a));
            var rat = new Rational(0, 1);
            for (int i = a.Count-1; i >= 0; --i)
            {
                rat = a[i] + rat;
                rat = rat.Reciprocal();
            }
            rat = rat / scale;
            Debug.Print("crat: {0}", rat);
            return rat;
        }
    }
}