/* 
 * Copyright (C) 1999-2010 John Källén.
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

using Decompiler.Arch.Intel;
using Decompiler.Core.Types;
using Decompiler.Core.Serialization;
using Decompiler.Gui;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Decompiler.UnitTests.Gui
{
    [TestFixture]
    class SignatureParserTests
    {
        private SignatureParser sp;
        private IntelArchitecture arch = new IntelArchitecture(ProcessorMode.ProtectedFlat);

        [SetUp]
        public void Setup()
        {
            sp = new SignatureParser(arch);
        }

        [Test]
        public void ParseVoidFn()
        {
            sp.Parse("void foo()");
            Assert.AreEqual("foo", sp.ProcedureName);
            Assert.IsTrue(sp.IsValid);
            Assert.AreEqual(0, sp.Signature.Arguments.Length);
            Assert.IsNull(sp.Signature.ReturnValue);
        }

        [Test]
        public void ParseRegisterFn()
        {
            sp.Parse("eax foo()");
            Assert.AreEqual("foo", sp.ProcedureName);
            Assert.IsTrue(sp.IsValid);
            Assert.AreEqual(0, sp.Signature.Arguments.Length);
            Assert.IsNotNull(sp.Signature.ReturnValue);
            Assert.AreEqual("eax", ((SerializedRegister) sp.Signature.ReturnValue.Kind).Name);
        }

        [Test]
        public void ParseFnName()
        {
            sp.Parse("cx bar()");
            Assert.AreEqual("bar", sp.ProcedureName);
        }

        [Test]
        public void ParseNoArg()
        {
            sp.Parse("void zzz()");
            Assert.IsNotNull(sp.Signature.Arguments);
            Assert.AreEqual(0, sp.Signature.Arguments.Length);
        }

        [Test]
        public void ParseSingleRegisterArg()
        {
            sp.Parse("void zzz(word32 eax)");
            Assert.IsNotNull(sp.Signature.Arguments);
            Assert.AreEqual(1, sp.Signature.Arguments.Length);
            Assert.IsAssignableFrom(typeof (SerializedRegister), sp.Signature.Arguments[0].Kind);
            Assert.AreEqual("eax", sp.Signature.Arguments[0].Name);
            Assert.AreEqual("word32", sp.Signature.Arguments[0].Type);
        }

        [Test]
        public void ParseTwoArgs()
        {
            sp.Parse("void zzz(word32 eax, byte cl)");
            Assert.IsNotNull(sp.Signature.Arguments);
            Assert.AreEqual(2, sp.Signature.Arguments.Length);
            Assert.IsAssignableFrom(typeof(SerializedRegister), sp.Signature.Arguments[0].Kind);
            Assert.AreEqual("eax", sp.Signature.Arguments[0].Name);
            Assert.AreEqual("word32", sp.Signature.Arguments[0].Type);
            Assert.AreEqual("cl", sp.Signature.Arguments[1].Name);
            Assert.AreEqual("byte", sp.Signature.Arguments[1].Type);
        }

        [Test]
        public void ParseStackParameter()
        {
            sp.Parse("void zzz(word32 stackie, byte stackb)");
            Assert.IsNotNull(sp.Signature.Arguments);
            Assert.AreEqual(2, sp.Signature.Arguments.Length);
            Assert.IsAssignableFrom<SerializedStackVariable>(sp.Signature.Arguments[0].Kind);
            Assert.IsAssignableFrom<SerializedStackVariable>(sp.Signature.Arguments[1].Kind);
            Assert.AreEqual("stackie", sp.Signature.Arguments[0].Name);
            Assert.AreEqual("stackb", sp.Signature.Arguments[1].Name);
            var k0 = (SerializedStackVariable)sp.Signature.Arguments[0].Kind;
            var k1 = (SerializedStackVariable)sp.Signature.Arguments[1].Kind;
            Assert.AreEqual(4, k0.ByteSize);
            Assert.AreEqual(4, k1.ByteSize);
        }

    }
}
