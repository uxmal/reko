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

using Decompiler.Analysis;
using Decompiler.Core;
using Decompiler.Core.Expressions;
using Decompiler.Core.Types;
using Decompiler.Typing;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Diagnostics;

namespace Decompiler.UnitTests.Typing
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

        [SetUp]
        public void Setup()
        {
            this.m = new ExpressionEmitter();
            this.store = new TypeStore();
            this.factory = new TypeFactory();
            this.arch = new FakeArchitecture();
            var prog = new Program { Architecture = arch , Platform = new DefaultPlatform(null,arch)};
            this.exa = new ExpressionTypeAscender(prog.Platform, store, factory);
            this.exd = new ExpressionTypeDescender(prog, store, factory);
            store.EnsureExpressionTypeVariable(factory, prog.Globals, "globals_t");
        }

        private Pointer PointerTo(DataType dt)
        {
            return new Pointer(dt, arch.PointerType.Size);
        }
        private static Identifier Id(string name, DataType dt)
        {
            return new Identifier(name, dt, TemporaryStorage.None);
        }

        private void Verify(string outputFileName)
        {
            using (FileUnitTester fut = new FileUnitTester(outputFileName))
            {
                store.Write(fut.TextWriter);
                fut.AssertFilesEqual();
            }
        }

        private void RunTest(Expression e, DataType dt)
        {
            var eq = new EquivalenceClassBuilder(factory, store);
            e.Accept(eq);

            var result = e.Accept(exa);
            Debug.Print("After exa: {0}", result);
            exd.MeetDataType(e, dt);
            e.Accept(exd, e.TypeVariable);

            var outputFileName = string.Format("Typing/{0}.txt", new StackTrace().GetFrame(1).GetMethod().Name);
            Verify(outputFileName);
        }

        private void RunTest(params Tuple<Expression,DataType> [] tests)
        {
            foreach (var t in tests)
            {
                var eq = new EquivalenceClassBuilder(factory, store);
                t.Item1.Accept(eq);
            }

            foreach (var t in tests)
            {
                var result = t.Item1.Accept(exa);
                Debug.Print("After exa: {0}", result);
                exd.MeetDataType(t.Item1, t.Item2);
                t.Item1.Accept(exd, t.Item1.TypeVariable);
            }
            var outputFileName = string.Format("Typing/{0}.txt", new StackTrace().GetFrame(1).GetMethod().Name);
            Verify(outputFileName);
        }

        private Tuple<Expression, DataType> Test(Expression e, DataType dt)
        {
            return Tuple.Create(e, dt);
        }

        [Test]
        public void ExdConstant()
        {
            RunTest(Constant.Word32(3), PrimitiveType.Int32);
        }

        [Test]
        public void ExdIdentifier()
        {
            RunTest(Id("x", PrimitiveType.Byte), PrimitiveType.Char);
        }

        [Test]
        public void ExdAnd()
        {
            RunTest(
                m.And(
                    Id("x", PrimitiveType.Byte),
                    3),
                PrimitiveType.Char);
        }

        [Test]
        public void ExdMem()
        {
            RunTest(
                m.LoadW(
                    Id("x", PrimitiveType.Word32)),
                PrimitiveType.WChar);
        }

        [Test]
        public void ExdAddPtrInt()
        {
            RunTest(
                m.IAdd(
                    Id("p", PrimitiveType.Word32),
                    Constant.Word32(4)),
                PrimitiveType.Pointer32);
        }

        [Test]
        public void ExdFieldAccess()
        {
            RunTest(
                m.LoadDw(
                    m.IAdd(
                        Id("p", PrimitiveType.Word32),
                        Constant.Word32(4))),
                PrimitiveType.Word32);
        }

        [Test]
        public void ExdSeqWithSelector()
        {
            RunTest(
                m.Seq(
                    Id("ds", PrimitiveType.SegmentSelector),
                    Constant.Word16(4)),
                PointerTo(store.CreateTypeVariable(factory)));
        }

        [Test]
        public void ExdSegmem()
        {
            RunTest(
                m.SegMem(
                    PrimitiveType.Byte,
                    Id("ds", PrimitiveType.SegmentSelector),
                    Constant.Word16(0x123)),
                PrimitiveType.Byte);
        }

        [Test]
        public void ExdTwoFieldAccesses()
        {
            var p = Id("p", PrimitiveType.Word32);
            RunTest(
                Test(m.LoadDw(m.IAdd(p, 8)), PrimitiveType.Int32),
                Test(m.LoadDw(m.IAdd(p, 12)), PrimitiveType.Real32));
        }

        [Test]
        public void ExdUnion()
        {
            var p = Id("p", PrimitiveType.Word32);
            RunTest(
                Test(m.LoadDw(m.IAdd(p, 12)), PrimitiveType.Int32),
                Test(m.LoadDw(m.IAdd(p, 12)), PrimitiveType.Real32));
        }
    }
}
