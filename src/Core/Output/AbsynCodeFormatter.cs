#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Reko.Core.Output
{
    public class AbsynCodeFormatter : CodeFormatter
    {
        public AbsynCodeFormatter(Formatter writer) : base(writer)
        {

        }

        public override void VisitConstant(Constant c)
        {
            var pt = c.DataType as PrimitiveType;
            if (pt != null)
            {
                switch (pt.Domain)
                {
                case Domain.Boolean:
                case Domain.Character:
                case Domain.Real:
                    base.VisitConstant(c);
                    return;
                case Domain.SignedInt:
                    InnerFormatter.Write(SignedRepresentation(c.ToInt64());
                    return;
                default:
                    InnerFormatter.Write(UnsignedRepresentation(c.ToUInt64());
                    break;
                }
            }
            base.VisitConstant(c);
        }

        protected override string SignedFormatString(PrimitiveType type, long value)
        {
            return ChooseFormatStringBasedOnPattern(
                value.ToString(CultureInfo.InvariantCulture),
                value.ToString("X", CultureInfo.InvariantCulture),
        }

        public string ChooseFormatStringBasedOnPattern(string decRep, string hexRep, string decInverted, string hexInverted)
        {
            var decEntropy = Entropy(decRep, "0123456789");
            var hexEntropy = Entropy(hexRep, "0123456789ABCDEF");
            if (hexEntropy < decEntropy)
            {
                int length = hexRep.Length;
                if (length < 2)
                    length = 2;
                else if (length < 4)
                    length = 4;
                else
                    length = (length + 1) & ~1;
                return string.Format("0x{0}0:X{1}{2}", "{", length, "}");
            }
            else
            {
                return "{0}";
            }
        }

        protected override string UnsignedFormatString(PrimitiveType type, ulong value)
        {
            return ChooseFormatStringBasedOnPattern(
                value.ToString(CultureInfo.InvariantCulture),
                value.ToString("X", CultureInfo.InvariantCulture));
        }
        
        // Entropy =  -\sum prob_i * ln prob_i
        /// <summary>
        /// Calculates the entropy of the sequence <paramref name="seq"/> 
        /// given the set of states <paramref name="states"/>. The more negative
        /// the entropy, the less disordered the sequence appears to be.
        /// </summary>
        /// <typeparam name="T">Symbol type</typeparam>
        /// <param name="seq"></param>
        /// <param name="states"></param>
        /// <returns></returns>
        private static double Entropy<T>(IEnumerable<T> seq, IEnumerable<T> states)
        {
            var frequencies = states.ToDictionary(k => k, v => 0);
            foreach (var state in seq)
                frequencies[state] += 1;
            var entropy = 0.0;
            var nstates = seq.Count();
            foreach (var freq in frequencies.Values)
            {
                var prob = (double)freq / nstates;
                if (prob > 0)
                    entropy -= prob * Math.Log(prob);
            }
            return entropy;
        }
    }
}
