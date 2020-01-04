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
    public class ExpressionTypeDescenderTests
    {
        private ExpressionEmitter m;
        private TypeStore store;
        private TypeFactory factory;
        private ExpressionTypeAscender exa;
        private ExpressionTypeDescender exd;
        private FakeArchitecture arch;
        private Program program;

        [SetUp]
        public void Setup()
        {
            this.m = new ExpressionEmitter();
            this.store = new TypeStore();
            this.factory = new TypeFactory();
            this.arch = new FakeArchitecture();
            this.program = new Program { Architecture = arch, Platform = new DefaultPlatform(null, arch) };
            this.exa = new ExpressionTypeAscender(program, store, factory);
            this.exd = new ExpressionTypeDescender(program, store, factory);
            store.EnsureExpressionTypeVariable(factory, program.Globals, "globals_t");
        }

        private void Given_GlobalVariable(Address addr, DataType dt)
        {
            program.GlobalFields.Fields.Add((int)addr.ToUInt32(), dt);
        }

        private Pointer PointerTo(DataType dt)
        {
            return new Pointer(dt, arch.PointerType.BitSize);
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

        private void RunTest(Expression e, DataType dt, string outputFileName)
        {
            var listener = new FakeDecompilerEventListener();
            var eq = new EquivalenceClassBuilder(factory, store, listener);
            e.Accept(eq);

            e.Accept(exa);
            exd.MeetDataType(e, dt);
            e.Accept(exd, e.TypeVariable);

            Verify(outputFileName);
        }

        private void RunTest(string outputFileName, params Tuple<Expression, DataType>[] tests)
        {
            var listener = new FakeDecompilerEventListener();
            foreach (var t in tests)
            {
                var eq = new EquivalenceClassBuilder(factory, store, listener);
                t.Item1.Accept(eq);
            }

            foreach (var t in tests)
            {
                var result = t.Item1.Accept(exa);
                exd.MeetDataType(t.Item1, t.Item2);
                t.Item1.Accept(exd, t.Item1.TypeVariable);
            }
            Verify(outputFileName);
        }

        private Tuple<Expression, DataType> Test(Expression e, DataType dt)
        {
            return Tuple.Create(e, dt);
        }

        [Test]
        public void ExdConstant()
        {
            RunTest(
                Constant.Word32(3),
                PrimitiveType.Int32,
                "Typing/ExdConstant.txt");
        }

        [Test]
        public void ExdIdentifier()
        {
            RunTest(Id("x", PrimitiveType.Byte), PrimitiveType.Char, "Typing/ExdIdentifier.txt");
        }

        [Test]
        public void ExdAnd()
        {
            RunTest(
                m.And(
                    Id("x", PrimitiveType.Byte),
                    3),
                PrimitiveType.Char,
                "Typing/ExdAnd.txt");
        }

        [Test]
        public void ExdMem()
        {
            RunTest(
                m.Mem16(
                    Id("x", PrimitiveType.Word32)),
                PrimitiveType.WChar,
                "Typing/ExdMem.txt");
        }

        [Test]
        public void ExdAddPtrInt()
        {
            RunTest(
                m.IAdd(
                    Id("p", PrimitiveType.Word32),
                    Constant.Word32(4)),
                PrimitiveType.Ptr32,
                "Typing/ExdAddPtrInt.txt");
        }

        [Test]
        public void ExdFieldAccess()
        {
            RunTest(
                m.Mem32(
                    m.IAdd(
                        Id("p", PrimitiveType.Word32),
                        Constant.Word32(4))),
                PrimitiveType.Word32,
                "Typing/ExdFieldAccess.txt");
        }

        [Test]
        public void ExdSeqWithSelector()
        {
            RunTest(
                m.Seq(
                    Id("ds", PrimitiveType.SegmentSelector),
                    Constant.Word16(4)),
                PointerTo(store.CreateTypeVariable(factory)),
                "Typing/ExdSeqWithSelector.txt");
        }

        [Test]
        public void ExdSegmem()
        {
            RunTest(
                m.SegMem(
                    PrimitiveType.Byte,
                    Id("ds", PrimitiveType.SegmentSelector),
                    Constant.Word16(0x123)),
                PrimitiveType.Byte,
                "Typing/ExdSegmem.txt");
        }

        [Test]
        public void ExdTwoFieldAccesses()
        {
            var p = Id("p", PrimitiveType.Word32);
            RunTest(
                "Typing/ExdTwoFieldAccesses.txt",
                Test(m.Mem32(m.IAdd(p, 8)), PrimitiveType.Int32),
                Test(m.Mem32(m.IAdd(p, 12)), PrimitiveType.Real32));
        }

        [Test]
        public void ExdUnion()
        {
            var p = Id("p", PrimitiveType.Word32);
            RunTest(
                "Typing/ExdUnion.txt",
                Test(m.Mem32(m.IAdd(p, 12)), PrimitiveType.Int32),
                Test(m.Mem32(m.IAdd(p, 12)), PrimitiveType.Real32));
        }

        [Test]
        public void ExdFloatCmp()
        {
            var p = Id("p", PrimitiveType.Word32);
            RunTest(
                "Typing/ExdFloatCmp.txt",
                Test(m.FGe(p, Constant.Real32(-5.5F)), PrimitiveType.Bool));
        }

        [Test]
        public void ExdFloatSub()
        {
            var p = Id("p", PrimitiveType.Word32);
            RunTest(
                "Typing/ExdFloatSub.txt",
                Test(m.FSub(p, Constant.Real32(-5.5F)), PrimitiveType.Real32));
        }

        [Test]
        public void ExdApplication()
        {
            var sig = FunctionType.Action(Id("r", PrimitiveType.Real32));
            var ep = new ExternalProcedure("test", sig);
            RunTest(
                "Typing/ExdApplication.txt",
                Test(m.Fn(ep, m.Mem(PrimitiveType.Word32, m.Word32(0x0300400))), VoidType.Instance));
        }

        [Test]
        public void ExdIndirectCall()
        {
            var p = Id("p", PrimitiveType.Word32);
            var sig = FunctionType.Action(new[] { Id("r", PrimitiveType.Real32) });
            store.EnsureExpressionTypeVariable(factory, p);
            p.TypeVariable.OriginalDataType = PointerTo(sig);
            p.TypeVariable.DataType = PointerTo(sig);
            RunTest(
                "Typing/ExdIndirectCall.txt",
                Test(
                    m.Fn(
                        p,
                        VoidType.Instance,
                        m.Mem(PrimitiveType.Word32, m.Word32(0x0300400))),
                    VoidType.Instance));
        }

        [Test]
        public void ExdSubtraction()
        {
            var p = Id("p", PrimitiveType.Word32);
            RunTest(
                m.Mem(
                    PrimitiveType.Word32,
                    m.IAdd(m.ISub(p, m.Word32(4)), m.Word32(0))),
                PrimitiveType.Word32,
                "Typing/ExdSubtraction.txt");
        }

        [Test]
        public void ExdReferenceToUnknown()
        {
            var p = Id("p", PrimitiveType.Word32);
            store.EnsureExpressionTypeVariable(factory, p);
            p.TypeVariable.OriginalDataType = PointerTo(
                new TypeReference("UNKNOWN_TYPE", new UnknownType()));
            p.TypeVariable.DataType = PointerTo(
                new TypeReference("UNKNOWN_TYPE", new UnknownType()));
            RunTest(
                m.Mem(
                    PrimitiveType.Word32,
                    m.IAdd(p, m.Word32(4))),
                PrimitiveType.Word32,
                "Typing/ExdReferenceToUnknown.txt");
            var ptr = p.TypeVariable.OriginalDataType as Pointer;
            Assert.IsNotNull(ptr, "Should be pointer");
            var tRef = ptr.Pointee as TypeReference;
            Assert.IsNotNull(tRef, "Should be type reference");
            Assert.AreEqual("(struct (4 T_5 t0004))", tRef.Referent.ToString());
        }

        [Test]
        public void ExdConditional()
        {
            var id = Id("id", PrimitiveType.Bool);
            var id1 = Id("id1", PrimitiveType.Int32);
            var id2 = Id("id2", PrimitiveType.Int32);
            RunTest(
                m.Conditional(PrimitiveType.Word32, id, id1, id2),
                PrimitiveType.Word32,
                "Typing/ExdConditional.txt");
        }

        [Test]
        public void ExdWideningFMul()
        {
            var idLeft = Id("idLeft", PrimitiveType.Word64);
            var idRight = Id("idRight", PrimitiveType.Word64);
            var fmul = m.FMul(idLeft, idRight);
            fmul.DataType = PrimitiveType.Real96;
            RunTest(fmul, PrimitiveType.Real96, "Typing/ExdWideningFMul.txt");
        }
    }
}
