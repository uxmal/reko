#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using Reko.Core.CLanguage;
using Reko.Core.Expressions;
using Reko.Core.Serialization;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.IO;
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
            mr.ReplayAll();

            var oldSig = proc.Signature;
            var usb = new UserSignatureBuilder(program);
            usb.BuildSignatures();
            Assert.AreSame(oldSig, proc.Signature);
        }

        [Test]
        public void Usb_ParseFunctionDeclaration()
        {
            Given_Procedure(0x1000);
            platform.Stub(p => p.PlatformIdentifier).Return("testPlatform");
            platform.Stub(p => p.TypeLibs).Return(new TypeLibrary[0]);
            platform.Stub(p => p.PointerType).Return(PrimitiveType.Pointer32);
            platform.Stub(p => p.GetByteSizeFromCBasicType(CBasicType.Int)).Return(4);
            platform.Stub(p => p.GetByteSizeFromCBasicType(CBasicType.Char)).Return(1);
            platform.Stub(p => p.CreateSymbolTable()).Return(new SymbolTable(platform));
            platform.Stub(p => p.EnsureTypeLibraries(null))
                .IgnoreArguments();
            var ser = mr.Stub<ProcedureSerializer>(arch, null, "cdecl");
            platform.Expect(s => s.CreateProcedureSerializer(null, null)).IgnoreArguments().Return(ser);
            ser.Expect(s => s.Deserialize(
                Arg<SerializedSignature>.Is.NotNull,
                Arg<Frame>.Is.NotNull)).Return(new ProcedureSignature());
            mr.ReplayAll();

            var usb = new UserSignatureBuilder(program);
            var sProc = usb.ParseFunctionDeclaration("int foo(char *)", proc.Frame);
            mr.ReplayAll();

            Assert.AreEqual(
                "fn(arg(prim(SignedInt,4)),(arg(ptr(prim(Character,1))))",
                sProc.Signature.ToString());
        }

        [Test(Description ="Verifies that the user can override register names.")]
        public void Usb_ParseFunctionDeclaration_WithRegisterArgs()
        {
            var arch = new FakeArchitecture();
            var m = new ProcedureBuilder(arch, "test");
            var r1 = m.Reg32("r1", 1);
            var r2 = m.Reg32("r2", 2);
            m.Store(m.Word32(0x123400), m.Cast(PrimitiveType.Byte, r1));
            m.Store(m.Word32(0x123404), m.Cast(PrimitiveType.Real32, r2));
            m.Return();

            var usb = new UserSignatureBuilder(program);
            usb.ApplySignatureToProcedure(
                Address.Create(PrimitiveType.Pointer32, 0x1000),
                new ProcedureSignature(
                    null,
                    new Identifier("r2", PrimitiveType.Char, r1.Storage),  // perverse but legal.
                    new Identifier("r1", PrimitiveType.Real32, r2.Storage)),
                m.Procedure);
            var sExp = @"// test
// Return size: 0
void test(char r2, real32 r1)
test_entry:
	// succ:  l1
l1:
	r1 = r2
	r2 = r1
	Mem0[0x00123400:byte] = (byte) r1
	Mem0[0x00123404:real32] = (real32) r2
	return
	// succ:  test_exit
test_exit:
";
            var sb = new StringWriter();
            m.Procedure.Write(false, sb);
            Assert.AreEqual(sExp, sb.ToString());
        }
    }
}
