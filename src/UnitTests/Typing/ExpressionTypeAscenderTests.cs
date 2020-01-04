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
            return new Pointer(dt, 32);
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

        private void RunTest(Expression e, string outputFileName)
        {
            var globals = new Identifier("globals", PrimitiveType.Ptr32, RegisterStorage.None);
            store.EnsureExpressionTypeVariable(factory, globals, "globals_t");
            var eq = new EquivalenceClassBuilder(factory, store, new FakeDecompilerEventListener());
            e.Accept(eq);

            e.Accept(exa);

            Verify(outputFileName);
        }

        [Test]
        public void ExaConstant()
        {
            RunTest(Constant.Int32(3), "Typing/ExaConstant.txt");
        }

        [Test]
        public void ExaIdentifier()
        {
            RunTest(Id("x", PrimitiveType.Byte), "Typing/ExaIdentifier.txt");
        }

        [Test]
        public void ExaAnd()
        {
            RunTest(
                m.And(
                    Id("x", PrimitiveType.Byte),
                    3),
                "Typing/ExaAnd.txt");
        }

        [Test]
        public void ExaMem()
        {
            RunTest(
                m.Mem8(
                    Id("x", PrimitiveType.Word16)),
                "Typing/ExaMem.txt");
        }

        [Test]
        public void ExaAddPtrInt()
        {
            RunTest(
                m.IAdd(
                    Id("p", new Pointer(PrimitiveType.Word32, 32)),
                    Constant.Int32(4)),
                "Typing/ExaAddPtrInt.txt");
        }

        [Test]
        public void ExaSeqWithSelector()
        {
            RunTest(
                m.Seq(
                    Id("ds", PrimitiveType.SegmentSelector),
                    Constant.Word16(4)),
                "Typing/ExaSeqWithSelector.txt");
        }

        [Test]
        public void ExaSegmem()
        {
            RunTest(
                m.SegMem(
                    PrimitiveType.Byte,
                    Id("ds", PrimitiveType.SegmentSelector),
                    Constant.Word16(0x123)),
                "Typing/ExaSegmem.txt");
        }

        [Test(Description = "Duplicate occurrences of same TypeReference should resolve to same TypeVariable")]
        public void ExaTypeReference()
        {
            var a = Id("a", new TypeReference("INT", PrimitiveType.Int32));
            var b = Id("b", new TypeReference("INT", PrimitiveType.Int32));
            RunTest(
                m.IAdd(a, b),
                "Typing/ExaTypeReference.txt");
        }

        [Test(Description = "Resilve LPSTRs and the like to their underlying rep")]
        public void ExaTypeReferenceToPointer()
        {
            var psz = Id("psz", new TypeReference("LPSTR", new Pointer(PrimitiveType.Char, 32)));
            RunTest(
                m.Mem8(m.IAdd(psz, Constant.Word32(0))),
                "Typing/ExaTypeReferenceToPointer.txt");
        }

        public void ExaMkSequence()
        {
            var lpsz = Id("psz", PrimitiveType.Word32);
            RunTest(
                m.Seq(
                    m.Mem16(m.IAdd(lpsz, 4)),
                    Constant.Word16(0x1200)),
                "Typing/ExaMkSequence.txt");
        }

        [Test(Description = "Pointers should be processed as globals")]
        public void ExaUsrGlobals_Ptr32()
        {
            Given_GlobalVariable(
                Address.Ptr32(0x10001200), PrimitiveType.Real32);
            RunTest(Constant.Create(PrimitiveType.Ptr32, 0x10001200),
                "Typing/ExaUsrGlobals_Ptr32.txt");
        }

        [Test(Description = "Reals should not be processed as globals")]
        public void ExaUsrGlobals_Real32()
        {
            Given_GlobalVariable(
                Address.Ptr32(0x10001200), PrimitiveType.Real32);
            RunTest(Constant.Create(PrimitiveType.Real32, 0x10001200),
                "Typing/ExaUsrGlobals_Real32.txt");
        }

        [Test]
        public void ExaSubtraction()
        {
            var p = Id("p", PointerTo(PrimitiveType.Real64));
            RunTest(m.ISub(p, m.Word32(4)),
                "Typing/ExaSubtraction.txt");
        }

        [Test]
        public void ExaAddrOf()
        {
            var p = Id("p", PrimitiveType.Real64);
            RunTest(m.AddrOf(PrimitiveType.Ptr32, p),
                "Typing/ExaAddrOf.txt");
        }

        [Test]
        public void ExaConditional()
        {
            var id = Id("id", PrimitiveType.Bool);
            var id1 = Id("id1", PrimitiveType.Int32);
            var id2 = Id("id2", PrimitiveType.Int32);
            RunTest(m.Conditional(PrimitiveType.Word32, id, id1, id2),
                "Typing/ExaConditional.txt");
        }

        [Test(Description = "Pointers should be processed as globals")]
        public void ExaUsrGlobals_Addr32()
        {
            Given_GlobalVariable(
                Address.Ptr32(0x10001200), PrimitiveType.Real32);
            RunTest(Address.Ptr32(0x10001200),
                "Typing/ExaUsrGlobals_Addr32.txt");
        }

        [Test(Description = "Operand sizes of a widening multiplication shouldn't affect the size of the product.")]
        public void ExaWideningFMul()
        {
            var idLeft = Id("idLeft", PrimitiveType.Word64);
            var idRight = Id("idRight", PrimitiveType.Word64);
            var fmul = m.FMul(idLeft, idRight);
            fmul.DataType = PrimitiveType.Real96;
            RunTest(fmul, "Typing/ExaWideningFMul.txt");
        }

    }
}
