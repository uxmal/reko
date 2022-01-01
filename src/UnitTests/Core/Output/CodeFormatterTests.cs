#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Core.Output
{
    [TestFixture]
    public class CodeFormatterTests
    {
        private ProcedureBuilder m;

        [SetUp]
        public void Setup()
        {
            m = new ProcedureBuilder();
        }

        [Test]
        public void CfmtMultiply_Same_Sized_Args()
        {
            var r1 = m.Reg32("r1");
            var r2 = m.Reg32("r2");
            Assert.AreEqual("r1 *s r2", m.SMul(r1, r2).ToString());
        }

        [Test]
        public void CfmtMultiply_Different_Sized_Args()
        {
            var r1 = m.Reg32("r1");
            var r2 = m.Reg32("r2");
            var product = m.SMul(r1, r2);
            product.DataType = PrimitiveType.Word64;
            Assert.AreEqual("r1 *s64 r2", product.ToString());
        }
    }
}
