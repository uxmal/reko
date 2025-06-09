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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Reko.Core.Output
{
    /// <summary>
    /// Code formatter for Absyn instructions.
    /// </summary>
    public class AbsynCodeFormatter : CodeFormatter
    {
        private const string DecimalSymbols = "0123456789";
        private const string HexSymbols = "0123456789ABCDEF";

        /// <summary>
        /// Constructs an instance of <see cref="AbsynCodeFormatter"/>.
        /// </summary>
        /// <param name="writer">Output sink.</param>
        public AbsynCodeFormatter(Formatter writer) : base(writer)
        {

        }

        /// <inheritdoc/>
        public override void VisitAddress(Address addr)
        {
            if (addr.IsNull)
            {
                WriteNull();
            }
            else
            {
                base.VisitAddress(addr);
            }
        }

        /// <inheritdoc/>
        public override void VisitConstant(Constant c)
        {
            if (c.IsValid)
            {
                var pt = c.DataType.ResolveAs<PrimitiveType>();
                if (pt is not null)
                {
                    switch (pt.Domain)
                    {
                    case Domain.Boolean:
                    case Domain.Character:
                    case Domain.Real:
                        base.VisitConstant(c);
                        return;
                    case Domain.SignedInt:
                        InnerFormatter.Write(SignedRepresentation(c.ToInt64()));
                        return;
                    default:
                        InnerFormatter.Write(UnsignedRepresentation(c));
                        return;
                    }
                }
            }
            base.VisitConstant(c);
        }

        private string UnsignedRepresentation(Constant u)
        {
            if (u is BigConstant bc)
            {
                return bc.Value.ToString("X");
            }
            ulong m;
            var b = u.DataType.BitSize;
            if (b == 0x40)
            {
                m = ~0ul;
            }
            else 
            {
                m = (1ul << b)-1;
            }
            ulong msb = (1ul << (b - 1));

            ulong p = u.ToUInt64();
            var decRep = p.ToString(CultureInfo.InvariantCulture);
            var hexRep = p.ToString("X", CultureInfo.InvariantCulture);
            var padHexRep = hexRep;
            if (hexRep.Length < decRep.Length)
            {
                padHexRep = new string('0', decRep.Length - hexRep.Length) + hexRep;
            }
            var decEntropy = Entropy(decRep, DecimalSymbols);
            var hexEntropy = Entropy(padHexRep, HexSymbols);
            if (decEntropy < hexEntropy)
            {
                return decRep;
            }
            else
            {
                var sb = new StringBuilder();
                if ((p & msb) != 0 &&
                    BitOperations.PopCount(m & p) > BitOperations.PopCount(m & ~p))
                {
                    sb.Append('~');
                    p = m & ~p;
                    hexRep = p.ToString("X", CultureInfo.InvariantCulture);
                }
                sb.Append("0x");
                int length = hexRep.Length;
                int pad;
                if (length <= 2)
                    pad = 2 - length;
                else if (length <= 4)
                    pad = 4 - length;
                else if (length <= 8)
                    pad = 8 - length;
                else
                    pad = 0;
                sb.Append('0', pad);
                sb.Append(hexRep);
                return sb.ToString();
            }
        }

        private string SignedRepresentation(long p)
        {
            var n = Math.Abs(p);
            var decRep = n.ToString(CultureInfo.InvariantCulture);
            var hexRep = n.ToString("X", CultureInfo.InvariantCulture);
            var decEntropy = Entropy(decRep, DecimalSymbols);
            var hexEntropy = Entropy(hexRep, HexSymbols);
            if (decEntropy <= hexEntropy)
            {
                return Convert.ToString(p, 10);
            }
            else
            {
                var sb = new StringBuilder();
                if (p < 0)
                    sb.Append('-');
                sb.Append("0x");
                int length = hexRep.Length;
                int pad;
                if (length < 2)
                    pad = 2 - length;
                else if (length < 4)
                    pad = 4 - length;
                else if (length < 8)
                    pad = 8 - length;
                else
                    pad = 0;
                sb.Append('0', pad);
                sb.Append(hexRep);
                return sb.ToString();
            }
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
            where T : notnull
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
