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
using Reko.Core.Serialization;
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
        private Platform platform;

        [SetUp]
        public void Setup()
        {
            this.mr = new MockRepository();
            this.arch = mr.Stub<IProcessorArchitecture>();
            this.platform = mr.Stub<Platform>(null, arch);

            platform.Stub(p => p.FramePointerType).Return(PrimitiveType.Pointer32);
            platform.Stub(p => p.DefaultCallingConvention).Return("cdecl");

            this.program = new Program
            {
                Architecture = arch,
                Platform = platform,
            };
        }

        private void Given_UserSignature(uint address, string str)
        {
            program.User.Procedures.Add(Address.Ptr32(address), new Reko.Core.Serialization.Procedure_v1
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

        [Test]
        public void Usb_BuildSignature()
        {
            Given_Procedure(0x1000);
            var ser = mr.Stub<ProcedureSerializer>(arch, null, "cdecl");
            platform.Expect(s => s.CreateProcedureSerializer(null, null)).IgnoreArguments().Return(ser);
            ser.Expect(s => s.Deserialize(
                Arg<SerializedSignature>.Is.NotNull,
                Arg<Frame>.Is.NotNull)).Return(new ProcedureSignature());
            mr.ReplayAll();

            var usb = new UserSignatureBuilder(program);
            var sig = usb.BuildSignature("int foo(char *)", proc.Frame);
            mr.ReplayAll();
        }
    }
}
