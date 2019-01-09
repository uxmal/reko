#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
using Reko.Loading;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Loading
{
    [TestFixture]
    public class FlirtoidSignatureLoaderTests
    {
        private readonly string nl = Environment.NewLine;

        private FlirtoidSignatureLoader fsl;
        private MockRepository mr;

        [SetUp]
        public void Setup()
        {
            this.mr = new MockRepository();
            this.fsl = null;
        }

        private void Given_FlirtoidSignatureLoader(string file)
        {
            fsl = mr.PartialMock<FlirtoidSignatureLoader>();
            fsl.Stub(f => f.CreateReader(null)).IgnoreArguments()
                .Return(new MemoryStream(Encoding.UTF8.GetBytes(file)));
        }

        [Test]
        public void Fsl_SingleLine()
        {
            var file =
                "01234521aF  Single_Line";
            Given_FlirtoidSignatureLoader(file);
            mr.ReplayAll();

            var sigs = fsl.Load("foo.foo").ToArray();
            Assert.AreEqual(1, sigs.Length);
            Assert.AreEqual("01234521aF", sigs[0].ImagePattern);
            Assert.AreEqual("Single_Line", sigs[0].Name);

        }
    }
}
