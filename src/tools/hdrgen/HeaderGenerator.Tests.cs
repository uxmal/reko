#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
using Reko.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if DEBUG || TRAVIS_RELEASE

namespace Reko.Tools.HdrGen
{
    [TestFixture]
    public class HeaderGeneratorTests
    {
        private StringWriter sw;

        [SetUp]
        public void Setup()
        {
            this.sw = new StringWriter();
        }

        [Test]
        public void Hdrgen_Enum()
        {
            var hdrgen = new HeaderGenerator(typeof(Address).Assembly, sw);
            hdrgen.WriteEnumDefinition(typeof(Reko.Core.AccessMode));
            var sExp =
            #region Expected
@"enum class AccessMode
{
    Execute = 1,
    Write = 2,
    Read = 4,
    ReadExecute = 5,
    ReadWrite = 6,
    ReadWriteExecute = 7,
}
";
            #endregion
            Assert.AreEqual(sExp, sw.ToString());
        }
    }
}

#endif
