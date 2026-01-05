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

using System;

namespace Reko.Core.Lib
{
    /// <summary>
    /// Simplistic implementation of rational numbers.
    /// </summary>
    public readonly struct Rational
    {
        /// <summary>
        /// Creates a rational number from two long integers.
        /// </summary>
        /// <param name="num">Numerator.</param>
        /// <param name="den">Denominator.</param>
        /// <returns>A rational number.</returns>
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

        /// <summary>
        /// Use a sequence of continued fractions to find a good 
        /// rational approximation to the double precision provided number
        /// </summary>
        /// <remarks>
        /// http://jonisalonen.com/2012/converting-decimal-numbers-to-ratios/
        /// </remarks>
        /// <param name="x"></param>
        /// <returns></returns>
        public static Rational FromDouble(double x)
        {
            double tolerance = 1.0E-6;
            int h1 = 1; int h2 = 0;
            int k1 = 0; int k2 = 1;
            var b = x;
            do
            {
                var a = (int)Math.Floor(b);
                var aux = h1;
                h1 = a * h1 + h2;
                h2 = aux;

                aux = k1;
                k1 = a * k1 + k2;
                k2 = aux;

                b = 1 / (b - a);
            } while (Math.Abs(x - h1 / (double)k1) > x * tolerance);
            return Rational.FromIntegers(h1, k1);
        }

        /// <summary>
        /// Numerator of this rational number.
        /// </summary>
        public long Numerator { get; }

        /// <summary>
        /// Denominator of this rational number.
        /// </summary>
        public long Denominator { get; }

        private Rational(long num, long den)
        {
            this.Numerator = num;
            this.Denominator = den;
        }

#pragma warning disable CS1591

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

        /// <summary>
        /// Computes the reciprocal of this number.
        /// </summary>
        public Rational Reciprocal()
        {
            return FromIntegers(Denominator, Numerator);
        }

        /// <summary>
        /// Converts this number to a floating-point number.
        /// </summary>
        /// <returns></returns>
        public double ToDouble()
        {
            return (double)Numerator / (double)Denominator;
        }

        /// <summary>
        /// Converts this number to its string representation.
        /// </summary>
        public override readonly string ToString()
        {
            return string.Format("{0}/{1}", Numerator, Denominator);
        }
    }
}
