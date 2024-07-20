#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Core.IRFormat;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Core.IRFormat
{
    [TestFixture]
    public class IRParserTests
    {
        private IRProcedureBuilder m;

        [SetUp]
        public void SetUp()
        {
            var arch = new FakeArchitecture();
            this.m = new IRProcedureBuilder("test", arch, Address.Ptr32(0x1000));

        }

        private void TestStatement(string sExpected, string source)
        {
            var parser = new IRFormatParser(new StringReader(source));
            var stm = parser.ParseStatement(m);
            Assert.AreEqual(sExpected, stm.ToString());
        }

        [Test]
        public void Irfp_Assignment()
        {
            TestStatement("rax = 42<i64>", "rax = 42<i64>");
        }


        [Test]
        public void Irfp_Assignment_sum()
        {
            TestStatement("rax = rax + 0x2A<64>", "rax = rax + 42<64>");
        }

        [Test]
        public void Irfp_Store()
        {
            TestStatement("Mem0[eax + ebx * 2<i32>:word32] = ecx", "Mem0[eax + ebx * 2<i32>:word32] = ecx | 1<32>");
        }

        [Test]
        public void Irfp_Shl()
        {
            TestStatement("eax = eax << 2<i32>", "eax = eax << 2<i32>");
        }

        [Test]
        public void Irfp_label()
        {
            TestStatement("eax = eax << 2<i32>", "label: eax = eax << 2<i32>");
            var block = m.BlocksByName["label"];
            Assert.AreEqual(1, block.Statements.Count);
        }
    }
}
