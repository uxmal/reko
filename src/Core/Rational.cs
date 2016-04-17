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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Core
{
    /// <summary>
    /// Simplistic implementation of rational numbers.
    /// </summary>
    public struct Rational
    {
        public static Rational FromIntegers(long num, long den)
        {
            var g = gcd(num, den);
            if (g > 1)
            {
                num /= g;
                den /= g;
            }
            return new Rational(num, den);
        }

        public readonly long Numerator;
        public readonly long Denominator;

        public Rational(long num, long den)
        {
            this.Numerator = num;
            this.Denominator = den;
        }

        public static Rational operator +(Rational r, long n)
        {
            return FromIntegers(r.Numerator + n * r.Denominator, r.Denominator);
        }

        public static Rational operator +(int n, Rational r)
        {
            return FromIntegers(r.Numerator + n * r.Denominator, r.Denominator);
        }

        public static Rational operator /(Rational r, long n)
        {
            return FromIntegers(r.Numerator, r.Denominator * n);
        }

        private static long gcd(long a, long b)
        {
            while (b != 0)
            {
                long t = b;
                b = a % b;
                a = t;
            }
            return a;
        }

        public Rational Reciprocal()
        {
            return FromIntegers(Denominator, Numerator);
        }

        public double ToDouble()
        {
            return (double)Numerator / (double)Denominator;
        }

        public override string ToString()
        {
            return string.Format("{0}/{1}", Numerator, Denominator);
        }
    }
}
