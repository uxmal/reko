#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using Reko.Core.Expressions;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Core.Types
{
    [TestFixture]
    public class DataTypeComparerTests
    {
        [Test]
        public void Dtcmp_Functions()
        {
            var sig1 = new FunctionType(
                new Identifier("", PrimitiveType.Int32, null),
                new Identifier("arg1", PrimitiveType.Real64, null));
            var sig2 = new FunctionType(
                new Identifier("", PrimitiveType.Int32, null),
                new Identifier("arg1", PrimitiveType.Real64, null));
            var dtcmp = new DataTypeComparer();
            Assert.AreEqual(dtcmp.GetHashCode(sig1), dtcmp.GetHashCode(sig2));
            Assert.AreEqual(0, dtcmp.Compare(sig1, sig2));
        }
    }
}
