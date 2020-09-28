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

using Moq;
using NUnit.Framework;
using Reko.Analysis;
using Reko.Core;
using Reko.Core.CLanguage;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Serialization;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CommonMockFactory = Reko.UnitTests.Mocks.CommonMockFactory;

namespace Reko.UnitTests.Analysis
{
    [TestFixture]
    public class UserSignatureBuilderTests
    {
        private CommonMockFactory mockFactory;
        private Program program;
        private Procedure proc;
        private IPlatform platform;

        [SetUp]
        public void Setup()
        {
            this.mockFactory = new CommonMockFactory();
            this.platform = mockFactory.CreateMockPlatform().Object;
            this.program = mockFactory.CreateProgram();
        }

        private void Given_UserSignature(uint address, string str)
        {
            program.User.Procedures.Add(Address.Ptr32(address), new Procedure_v1
            {
                 CSignature = str
            });
        }

        private void Given_UserProcDecompileFlag(uint address, bool decompile)
        {
            program.User.Procedures[Address.Ptr32(address)].Decompile = decompile;
        }

        private void Given_Procedure(uint address)
        {
            var m = new ProcedureBuilder("fnTest");
            m.Return();
            this.proc = m.Procedure;
            this.program.Procedures[Address.Ptr32(address)] = this.proc;
        }

        private void Given_UnscannedProcedure(uint address)
        {
            this.proc = Procedure.Create(program.Architecture, "fnTest", Address.Ptr32(address), new Frame(PrimitiveType.Ptr32));
            this.program.Procedures[Address.Ptr32(address)] = this.proc;
        }

        [Test(Description = "Empty user signature shouldn't affect procedure signature")]
        public void Usb_EmptyUserSignature()
        {
            Given_Procedure(0x1000);

            var oldSig = proc.Signature;
            var usb = new UserSignatureBuilder(program);
            usb.BuildSignatures(new FakeDecompilerEventListener());
            Assert.AreSame(oldSig, proc.Signature);
        }

        [Test]
        public void Usb_ParseFunctionDeclaration()
        {
            Given_Procedure(0x1000);

            var usb = new UserSignatureBuilder(program);
            var sProc = usb.ParseFunctionDeclaration("int foo(char *)");

            Assert.AreEqual(
                "fn(arg(prim(SignedInt,4)),(arg(ptr(prim(Character,1)))))",
                sProc.Signature.ToString());
        }

        [Test(Description = "Verifies stack delta of stdcall function.")]
        public void Usb_BuildSignature_Stdcall()
        {
            Given_Procedure(0x1000);
            Given_UserSignature(0x01000, "char * __stdcall test(int i, float f, double d)");

            var usb = new UserSignatureBuilder(program);
            usb.BuildSignature(Address.Ptr32(0x1000), proc);

            Assert.AreEqual(20, proc.Signature.StackDelta);
        }

        [Test]
        public void Usb_ParseFunctionDeclaration_PlatfromTypes()
        {
            program.EnvironmentMetadata.Types.Add(
                "BYTE",
                PrimitiveType.Create(PrimitiveType.Byte.Domain, 8));
            Given_Procedure(0x1000);

            var usb = new UserSignatureBuilder(program);
            var sProc = usb.ParseFunctionDeclaration("BYTE foo(BYTE a, BYTE b)");

            Assert.AreEqual(
                "fn(arg(BYTE),(arg(a,BYTE),arg(b,BYTE)))",
                sProc.Signature.ToString());
        }

        [Test]
        public void Usb_ParseFunctionDeclaration_UserDefinedTypes()
        {
            program.EnvironmentMetadata.Types.Add(
                "BYTE", PrimitiveType.Create(PrimitiveType.Byte.Domain, 8));
         
            Given_Procedure(0x1000);

            var usb = new UserSignatureBuilder(program);

            //should accept user defined type USRDEF1
            program.EnvironmentMetadata.Types.Add(
                "USRDEF1", PrimitiveType.Create(PrimitiveType.Int16.Domain, 16));

            var sProc = usb.ParseFunctionDeclaration("BYTE foo(USRDEF1 a, BYTE b)");

            Assert.AreEqual(
                "fn(arg(BYTE),(arg(a,USRDEF1),arg(b,BYTE)))",
                sProc.Signature.ToString());

            //should not accept undefined type USRDEF2
            sProc = usb.ParseFunctionDeclaration("BYTE foo(USRDEF1 a, USRDEF2 b)");
            Assert.AreEqual(null, sProc);

            //define USRDEF2 so parser should accept it

            program.EnvironmentMetadata.Types.Add(
               "USRDEF2", PrimitiveType.Create(PrimitiveType.Int16.Domain, 16));

            sProc = usb.ParseFunctionDeclaration("BYTE foo(USRDEF1 a, USRDEF2 b)");

            Assert.AreEqual(
                "fn(arg(BYTE),(arg(a,USRDEF1),arg(b,USRDEF2)))",
                sProc.Signature.ToString());
        }

        [Test]
        public void Usb_BuildSignatures_UserDefinedTypes()
        {
            program.EnvironmentMetadata.Types.Add(
                "PLATFORMDEF",
                PrimitiveType.Create(PrimitiveType.Byte.Domain, 8));
            program.EnvironmentMetadata.Types.Add(
                "USRDEF",
                PrimitiveType.Create(PrimitiveType.Int16.Domain, 16));
            Given_Procedure(0x1000);
            Given_UserSignature(0x01000, "int test(PLATFORMDEF a, USRDEF b)");

            var usb = new UserSignatureBuilder(program);
            usb.BuildSignatures(new FakeDecompilerEventListener());

            var sigExp =
@"Register int32 test(Stack PLATFORMDEF a, Stack USRDEF b)
// stackDelta: 4; fpuStackDelta: 0; fpuMaxParam: -1
";
            Assert.AreEqual(sigExp, proc.Signature.ToString("test", FunctionType.EmitFlags.AllDetails));

            Assert.AreEqual(2, proc.Signature.Parameters.Length);
            Assert.AreEqual("int32", proc.Signature.ReturnValue.DataType.ToString());
            Assert.AreEqual("a", proc.Signature.Parameters[0].Name);
            Assert.AreEqual("byte", (proc.Signature.Parameters[0].DataType as TypeReference).Referent.ToString());
            Assert.AreEqual("b", proc.Signature.Parameters[1].Name);
            Assert.AreEqual("int16", (proc.Signature.Parameters[1].DataType as TypeReference).Referent.ToString());
        }

        [Test]
        public void Usb_ParseGlobalDeclaration_Int()
        {
            var usb = new UserSignatureBuilder(program);
            var gbl = usb.ParseGlobalDeclaration("int test123");
            Assert.AreEqual("test123", gbl.Name);
            Assert.AreEqual("prim(SignedInt,4)", gbl.DataType.ToString());
        }

        [Test]
        public void Usb_ParseGlobalDeclaration_ArrayOfDouble()
        {
            var usb = new UserSignatureBuilder(program);
            var gbl = usb.ParseGlobalDeclaration("double dArr[12]");
            Assert.AreEqual("dArr", gbl.Name);
            Assert.AreEqual("arr(prim(Real,8),12)", gbl.DataType.ToString());
        }

        [Test]
        public void Usb_ParseGlobalDeclaration_PointerToUnsignedInt()
        {
            var usb = new UserSignatureBuilder(program);
            var gbl = usb.ParseGlobalDeclaration("unsigned int *uiPtr");
            Assert.AreEqual("uiPtr", gbl.Name);
            Assert.AreEqual("ptr(prim(UnsignedInt,4))", gbl.DataType.ToString());
        }

        [Test(Description ="Reko was crashing when a user-defined procedure was marked no-decompile.")]
        public void Usb_NoDecompileProcedure()
        {
            Given_UnscannedProcedure(0x1000);
            Given_UserSignature(0x01000, "void test([[reko::arg(register,\"ecx\")]] float f)");
            Given_UserProcDecompileFlag(0x1000, false);

            var usb = new UserSignatureBuilder(program);
            usb.BuildSignature(Address.Ptr32(0x1000), proc);
        }

        [Test]
        public void Usb_Word32()
        {
            Given_Procedure(0x1000);
            Given_UserSignature(0x1000, "void test(word32 foo)");
            var usb = new UserSignatureBuilder(program);
            usb.BuildSignature(Address.Ptr32(0x1000), proc);
            Assert.AreEqual("(fn void (word32))", proc.Signature.ToString());
            Assert.AreSame(PrimitiveType.Word32, proc.Signature.Parameters[0].DataType);
        }
    }
}
