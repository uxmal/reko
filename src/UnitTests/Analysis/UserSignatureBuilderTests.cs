#region License
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

using NUnit.Framework;
using Reko.Analysis;
using Reko.Core;
using Reko.Core.Types;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Reko.UnitTests.Analysis
{
    [TestFixture]
    public class UserSignatureBuilderTests
    {
        private IProcessorArchitecture arch;
        private MockRepository mr;
        private Program program;
        private Procedure proc;

        [SetUp]
        public void Setup()
        {
            this.mr = new MockRepository();
            this.arch = mr.Stub<IProcessorArchitecture>();

            this.program = new Program
            {
                Architecture = arch,
            };
        }

        private void Given_UserSignature(uint address, string str)
        {
            program.UserProcedures.Add(Address.Ptr32(address), new Reko.Core.Serialization.Procedure_v1
            {
                 CSignature = str
            });
        }

        private void Given_Procedure(uint address)
        {
            this.proc = Procedure.Create(Address.Ptr32(address), new Frame(PrimitiveType.Pointer32));
        }

        [Test(Description = "Empty user signature should't affect procedure signature")]
        public void Usb_EmptyUserSignature()
        {
            Given_Procedure(0x1000);

            var oldSig = proc.Signature;
            var usb = new UserSignatureBuilder(program);
            usb.BuildSignatures();
            Assert.AreSame(oldSig, proc.Signature);
        }

        [Test(Description = "If a user supplies a signature, it should overwrite the old signature")]
        public void Usb_NonemptyUserSignature_OverwriteSignature()
        {
            Given_Procedure(0x1000);
            Given_UserSignature(0x1000, "void foo(void)");
            var usb = new UserSignatureBuilder(program);
            usb.BuildSignatures();
            Assert.AreEqual("@@@", proc.Signature.ToString());
        }

        [Test]
        public void Usb_BuildSignature()
        {
            var usb = new UserSignatureBuilder(program);
            var sig = usb.BuildSignature("int foo(char *)");
            Assert.AreEqual("@@@", sig.ToString());

        }
    }
}
