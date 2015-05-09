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

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Expressions;
using Decompiler.Core.Types;
using Decompiler.Typing;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;

namespace Decompiler.UnitTests.Typing
{
	[TestFixture]
	public class TypedConstantRewriterTests
	{
		private TypeStore store;
		private TypeFactory factory;
		private TypedConstantRewriter tcr;
		private Identifier globals;

		[SetUp]
		public void Setup()
		{
            var image = new LoadedImage(Address.Ptr32(0x00100000), new byte[1024]);
            var arch = new FakeArchitecture();
            var program = new Program
            {
                Image = image,
                Architecture = arch,
                ImageMap = image.CreateImageMap(),
                Platform = new DefaultPlatform(null, arch),
            };
            store = program.TypeStore;
            factory = program.TypeFactory;
            globals = program.Globals;
			store.EnsureExpressionTypeVariable(factory, globals);

			StructureType s = new StructureType(null, 0);
			s.Fields.Add(0x00100000, PrimitiveType.Word32, null);

			TypeVariable tvGlobals = store.EnsureExpressionTypeVariable(factory, globals);
			EquivalenceClass eqGlobals = new EquivalenceClass(tvGlobals);
			eqGlobals.DataType = s;
			globals.TypeVariable.DataType = new Pointer(eqGlobals, 4);
			globals.DataType = globals.TypeVariable.DataType;

            tcr = new TypedConstantRewriter(program);
		}

		[Test]
		public void Tcr_RewriteWord32()
		{
			Constant c = Constant.Word32(0x0131230);
			store.EnsureExpressionTypeVariable(factory, c);
			c.TypeVariable.DataType = PrimitiveType.Word32;
			c.TypeVariable.OriginalDataType = PrimitiveType.Word32;
			Expression e = tcr.Rewrite(c, false);
			Assert.AreEqual("0x00131230" , e.ToString());
		}

		[Test]
        public void Tcr_RewriterRealBitpattern()
		{
			Constant c = Constant.Word32(0x3F800000);
			store.EnsureExpressionTypeVariable(factory, c);
			c.TypeVariable.DataType = PrimitiveType.Real32;
			c.TypeVariable.OriginalDataType = c.DataType;
			Expression e = tcr.Rewrite(c, false);
			Assert.AreEqual("1F", e.ToString());
		}

		[Test]
        public void Tcr_RewritePointer()
		{
			Constant c = Constant.Word32(0x00100000);
			store.EnsureExpressionTypeVariable(factory, c);
			c.TypeVariable.DataType = new Pointer(PrimitiveType.Word32, 4);
			c.TypeVariable.OriginalDataType = PrimitiveType.Word32;
			Expression e = tcr.Rewrite(c, false);
			Assert.AreEqual("&globals->dw100000", e.ToString());
		}


        [Test]
        public void Tcr_RewriteNullPointer()
        {
            Constant c = Constant.Word32(0x00000000);
            store.EnsureExpressionTypeVariable(factory, c);
            c.TypeVariable.DataType = new Pointer(PrimitiveType.Word32, 4);
            c.TypeVariable.OriginalDataType = PrimitiveType.Word32;
            Expression e = tcr.Rewrite(c, false);
            Assert.AreEqual("00000000", e.ToString());
        }

        [Test]
        public void Tcr_OffImagePointer()
        {
            Constant c = Constant.Word32(0xFFFFFFFF);
            store.EnsureExpressionTypeVariable(factory, c);
            c.TypeVariable.DataType = new Pointer(PrimitiveType.Word32, 4);
            c.TypeVariable.OriginalDataType = PrimitiveType.Word32;
            Expression e = tcr.Rewrite(c, false);
            Assert.AreEqual("(word32 *) 0xFFFFFFFF", e.ToString());
        }
	}
}
