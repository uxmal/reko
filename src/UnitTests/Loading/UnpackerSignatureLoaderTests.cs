﻿#region License
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

using Reko.Loading;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Reko.UnitTests.Loading
{
    [TestFixture]
    public class UnpackerSignatureLoaderTests
    {
        private MockRepository mr;
        private UnpackerSignatureLoader usl;
        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
        }

        private void Given_Unpacker()
        {
            usl = mr.PartialMock<UnpackerSignatureLoader>();
        }

        private void Given_SignatureFile(string filecontents)
        {
            var rdr = new StringReader(filecontents);
            usl.Stub(u => u.CreateFileReader(Arg<string>.Is.NotNull)).Return(rdr);
        }

        [Test]
        public void Usl_LoadSingleEntry()
        {
            Given_Unpacker();
            Given_SignatureFile(
@"<SIGNATURES> 
  <ENTRY>
    <NAME>LZEXE v0.91, v1.00a (1)</NAME>
    <COMMENTS />
    <ENTRYPOINT>060E1F8B??????8BF14E89F7</ENTRYPOINT>
    <ENTIREPE />
  </ENTRY>
</SIGNATURES>");
            mr.ReplayAll();

            var sigs = usl.Load("foo.xml").ToArray();
            Assert.AreEqual(1, sigs.Length);
            var sig = sigs[0];
            Assert.AreEqual("LZEXE v0.91, v1.00a (1)", sig.Name);
            Assert.AreEqual("060E1F8B??????8BF14E89F7", sig.EntryPointPattern);
        }
    }
}
