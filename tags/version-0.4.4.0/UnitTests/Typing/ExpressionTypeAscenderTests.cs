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
    public class ExpressionTypeAscenderTests 
    {
        private ExpressionEmitter m;
        private TypeStore store;
        private TypeFactory factory;
        private ExpressionTypeAscender exa;

        [SetUp]
        public void Setup()
        {
            this.m = new ExpressionEmitter();
            this.store = new TypeStore();
            this.factory = new TypeFactory();
            var arch = new FakeArchitecture();
            var platform = new DefaultPlatform(null, arch);
            this.exa = new ExpressionTypeAscender(platform, store, factory);
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

        private void RunTest(Expression e)
        {
            var globals = new Identifier("globals", PrimitiveType.Pointer32, TemporaryStorage.None);
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
                m.LoadB(
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
    }
}
