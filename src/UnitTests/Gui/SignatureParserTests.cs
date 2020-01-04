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

using Reko.Arch.X86;
using Reko.Core.Types;
using Reko.Core.Serialization;
using Reko.Gui;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Reko.UnitTests.Gui
{
    [TestFixture]
    class SignatureParserTests
    {
        private SignatureParser sp;
        private IntelArchitecture arch = new X86ArchitectureFlat32("x86-protected-32");

        [SetUp]
        public void Setup()
        {
            sp = new SignatureParser(arch);
        }

        [Test]
        public void SigparseVoidFn()
        {
            sp.Parse("void foo()");
            Assert.AreEqual("foo", sp.ProcedureName);
            Assert.IsTrue(sp.IsValid);
            Assert.AreEqual(0, sp.Signature.Arguments.Length);
            Assert.IsNull(sp.Signature.ReturnValue);
        }

        [Test]
        public void SigparseRegisterFn()
        {
            sp.Parse("eax foo()");
            Assert.AreEqual("foo", sp.ProcedureName);
            Assert.IsTrue(sp.IsValid);
            Assert.AreEqual(0, sp.Signature.Arguments.Length);
            Assert.IsNotNull(sp.Signature.ReturnValue);
            Assert.AreEqual("eax", ((Register_v1) sp.Signature.ReturnValue.Kind).Name);
        }

        [Test]
        public void SigparseFnName()
        {
            sp.Parse("cx bar()");
            Assert.AreEqual("bar", sp.ProcedureName);
        }

        [Test]
        public void SigparseNoArg()
        {
            sp.Parse("void zzz()");
            Assert.IsNotNull(sp.Signature.Arguments);
            Assert.AreEqual(0, sp.Signature.Arguments.Length);
        }

        [Test]
        public void SigparseSingleRegisterArg()
        {
            sp.Parse("void zzz(word32 eax)");
            Assert.IsNotNull(sp.Signature.Arguments);
            Assert.AreEqual(1, sp.Signature.Arguments.Length);
            Assert.IsAssignableFrom(typeof (Register_v1), sp.Signature.Arguments[0].Kind);
            Assert.AreEqual("eax", sp.Signature.Arguments[0].Name);
            Assert.AreEqual("word32", sp.Signature.Arguments[0].Type.ToString());
        }

        [Test]
        public void SigparseTwoArgs()
        {
            sp.Parse("void zzz(word32 eax, byte cl)");
            Assert.IsNotNull(sp.Signature.Arguments);
            Assert.AreEqual(2, sp.Signature.Arguments.Length);
            Assert.IsAssignableFrom(typeof(Register_v1), sp.Signature.Arguments[0].Kind);
            Assert.AreEqual("eax", sp.Signature.Arguments[0].Name);
            Assert.AreEqual("word32", sp.Signature.Arguments[0].Type.ToString());
            Assert.AreEqual("cl", sp.Signature.Arguments[1].Name);
            Assert.AreEqual("byte", sp.Signature.Arguments[1].Type.ToString());
        }

        [Test]
        public void SigparseStackParameter()
        {
            sp.Parse("void zzz(word32 stackie, byte stackb)");
            Assert.IsNotNull(sp.Signature.Arguments);
            Assert.AreEqual(2, sp.Signature.Arguments.Length);
            Assert.IsAssignableFrom<StackVariable_v1>(sp.Signature.Arguments[0].Kind);
            Assert.IsAssignableFrom<StackVariable_v1>(sp.Signature.Arguments[1].Kind);
            Assert.AreEqual("stackie", sp.Signature.Arguments[0].Name);
            Assert.AreEqual("stackb", sp.Signature.Arguments[1].Name);
            var k0 = (StackVariable_v1)sp.Signature.Arguments[0].Kind;
            var k1 = (StackVariable_v1)sp.Signature.Arguments[1].Kind;
            Assert.AreEqual("word32", sp.Signature.Arguments[0].Type.ToString());
            Assert.AreEqual("byte", sp.Signature.Arguments[1].Type.ToString());
        }

        [Test]
        public void SigparseSequence()
        {
            sp.Parse("void foo(ptr32 ss:di)");
            Assert.IsTrue(sp.IsValid);
            Assert.AreEqual(1, sp.Signature.Arguments.Length);
            Assert.AreEqual("ss_di", sp.Signature.Arguments[0].Name);
            var seq = (SerializedSequence) sp.Signature.Arguments[0].Kind;
            Assert.AreEqual("ss", seq.Registers[0].Name);
            Assert.AreEqual("di", seq.Registers[1].Name);
        }

        [Test]
        public void SigparseUnderscoreSequence()
        {
            sp.Parse("void foo(ptr32 ss_di)");
            Assert.IsTrue(sp.IsValid);
            Assert.AreEqual(1, sp.Signature.Arguments.Length);
            Assert.AreEqual("ss_di", sp.Signature.Arguments[0].Name);
            var seq = (SerializedSequence)sp.Signature.Arguments[0].Kind;
            Assert.AreEqual("ss", seq.Registers[0].Name);
            Assert.AreEqual("di", seq.Registers[1].Name);
        }
        [Test]
        public void SigparseReturnedSequence()
        {
            sp.Parse("dx_ax foo()");
            Assert.IsTrue(sp.IsValid);
            var ret = sp.Signature.ReturnValue;
            Assert.IsNotNull(ret);
            Assert.AreEqual("dx_ax", ret.Name);
            var seq = (SerializedSequence) ret.Kind;
            Assert.AreEqual("dx", seq.Registers[0].Name);
            Assert.AreEqual("ax", seq.Registers[1].Name);
        }


    }
}
