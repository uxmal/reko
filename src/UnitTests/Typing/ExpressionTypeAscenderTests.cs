#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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

using Reko.Analysis;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.Typing;
using Reko.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Diagnostics;

namespace Reko.UnitTests.Typing
{
    [TestFixture]
    public class ExpressionTypeAscenderTests
    {
        private ExpressionEmitter m;
        private TypeStore store;
        private TypeFactory factory;
        private ExpressionTypeAscender exa;
        private Program program;

        [SetUp]
        public void Setup()
        {
            this.m = new ExpressionEmitter();
            this.store = new TypeStore();
            this.factory = new TypeFactory();
            var arch = new FakeArchitecture();
            var platform = new DefaultPlatform(null, arch);
            program = new Program { Architecture = arch, Platform = platform };
            this.exa = new ExpressionTypeAscender(program, store, factory);
        }

        private void Given_GlobalVariable(Address addr, DataType dt)
        {
            program.GlobalFields.Fields.Add((int)addr.ToUInt32(), dt);
        }

        private Pointer PointerTo(DataType dt)
        {
            return new Pointer(dt, 4);
        }

        private static Identifier Id(string name, DataType dt)
        {
            return new Identifier(name, dt, RegisterStorage.None);
        }

        private void Verify(string outputFileName)
        {
            using (FileUnitTester fut = new FileUnitTester(outputFileName))
            {
                store.Write(fut.TextWriter);
                fut.AssertFilesEqual();
            }
        }

        private void RunTest(Expression e)
        {
            var globals = new Identifier("globals", PrimitiveType.Pointer32, RegisterStorage.None);
            store.EnsureExpressionTypeVariable(factory, globals, "globals_t");
            var eq = new EquivalenceClassBuilder(factory, store);
            e.Accept(eq);

            e.Accept(exa);

            var outputFileName = string.Format("Typing/{0}.txt", new StackTrace().GetFrame(1).GetMethod().Name);
            Verify(outputFileName);
        }

        [Test]
        public void ExaConstant()
        {
            RunTest(Constant.Int32(3));
        }

        [Test]
        public void ExaIdentifier()
        {
            RunTest(Id("x", PrimitiveType.Byte));
        }

        [Test]
        public void ExaAnd()
        {
            RunTest(
                m.And(
                    Id("x", PrimitiveType.Byte),
                    3));
        }

        [Test]
        public void ExaMem()
        {
            RunTest(
                m.Mem8(
                    Id("x", PrimitiveType.Word16)));
        }

        [Test]
        public void ExaAddPtrInt()
        {
            RunTest(
                m.IAdd(
                    Id("p", new Pointer(PrimitiveType.Word32, 4)),
                    Constant.Int32(4)));
        }

        [Test]
        public void ExaSeqWithSelector()
        {
            RunTest(
                m.Seq(
                    Id("ds", PrimitiveType.SegmentSelector),
                    Constant.Word16(4)));
        }

        [Test]
        public void ExaSegmem()
        {
            RunTest(
                m.SegMem(
                    PrimitiveType.Byte,
                    Id("ds", PrimitiveType.SegmentSelector),
                    Constant.Word16(0x123)));
        }

        [Test(Description = "Duplicate occurrences of same TypeReference should resolve to same TypeVariable")]
        public void ExaTypeReference()
        {
            var a = Id("a", new TypeReference("INT", PrimitiveType.Int32));
            var b = Id("b", new TypeReference("INT", PrimitiveType.Int32));
            RunTest(
                m.IAdd(a, b));
        }

        [Test(Description = "Resilve LPSTRs and the like to their underlying rep")]
        public void ExaTypeReferenceToPointer()
        {
            var psz = Id("psz", new TypeReference("LPSTR", new Pointer(PrimitiveType.Char, 4)));
            RunTest(
                m.Mem8(m.IAdd(psz, Constant.Word32(0))));
        }

        [Test(Description = "Resilve LPSTRs and the like to their underlying rep")]
        public void ExaMkSequence()
        {
            var lpsz = Id("psz", PrimitiveType.Word32);
            RunTest(
                m.Seq(
                    m.Mem16(m.IAdd(lpsz, 4)),
                    Constant.Word16(0x1200)));
        }

        [Test(Description = "Pointers should be processed as globals")]
        public void ExaUsrGlobals_Ptr32()
        {
            Given_GlobalVariable(
                Address.Ptr32(0x10001200), PrimitiveType.Real32);
            RunTest(Constant.Create(PrimitiveType.Pointer32, 0x10001200));
        }

        [Test(Description = "Reals should not be processed as globals")]
        public void ExaUsrGlobals_Real32()
        {
            Given_GlobalVariable(
                Address.Ptr32(0x10001200), PrimitiveType.Real32);
            RunTest(Constant.Create(PrimitiveType.Real32, 0x10001200));
        }

        [Test]
        public void ExaSubtraction()
        {
            var p = Id("p", PointerTo(PrimitiveType.Real64));
            RunTest(m.ISub(p, m.Word32(4)));
        }

        [Test]
        public void ExaAddrOf()
        {
            var p = Id("p", PrimitiveType.Real64);
            RunTest(m.AddrOf(p));
        }

        [Test]
        public void ExaConditional()
        {
            var id = Id("id", PrimitiveType.Bool);
            var id1 = Id("id1", PrimitiveType.Int32);
            var id2 = Id("id2", PrimitiveType.Int32);
            RunTest(m.Conditional(PrimitiveType.Word32, id, id1, id2));
        }
    }
}
