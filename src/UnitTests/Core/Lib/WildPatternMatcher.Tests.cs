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

using NUnit.Framework;
using Reko.Core.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Reko.UnitTests.Core.Lib
{
    [TestFixture]
    public class WildPatternMatcherTests
    {
        [Test]
        public void Wpm_NoWildcard()
        {
            var dcp = new WildPatternMatcher();
            var x = dcp.FindDcp("aska", "a");
            Assert.AreEqual(new[] { 0, 3 }, x.ToArray());
        }

        [Test]
        public void Wpm_PatternWithWildcardInTheMiddle()
        {
            var dcp = new WildPatternMatcher();
            var x = dcp.FindDcp("abracadabraaska", "a**a");
            Assert.AreEqual(new[] { 0, 7,11 }, x.ToArray());            
        }

        [Test]
        public void Wpm_PatternWithWildcardAtEnd()
        {
            var dcp = new WildPatternMatcher();
            // test with a pattern that has a wildcard at the end
            var x = dcp.FindDcp("abracadabraaska", "a**a*");
            // the 'aska' at the end of the string should not be matched - it is not followed by any character.
            Assert.AreEqual(new[] { 0, 7 }, x.ToArray());
        }
    }
}
