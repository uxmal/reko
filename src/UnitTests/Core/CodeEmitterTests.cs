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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Code;
using Reko.Core.Types;
using NUnit.Framework;
using System;

namespace Reko.UnitTests.Core
{
	[TestFixture]
	public class CodeEmitterTests
	{
		[Test]
		public void AddIncrement()
		{
			var id = new Identifier("id", PrimitiveType.Word16, null);
            var emitter = new CodeEmitterImpl();
			var add = emitter.IAdd(id, 3);
			Assert.AreEqual(PrimitiveType.Word16, add.DataType);
			Assert.AreEqual(PrimitiveType.Word16, add.Right.DataType);
			Assert.AreEqual("id + 0x0003", add.ToString());
		}

		[Test]
		public void SubIncrement()
		{
			var id = new Identifier("id", PrimitiveType.Word16, null);
            var emitter = new CodeEmitterImpl();
			var add = emitter.ISub(id, 3);
			Assert.AreEqual(PrimitiveType.Word16, add.DataType);
			Assert.AreEqual(PrimitiveType.Word16, add.Right.DataType);
			Assert.AreEqual("id - 0x0003", add.ToString());
		}

        [Test]
        public void SubPointer()
        {
            var ptr = new Pointer(new StructureType("tmp", 16), 32);
            var id = new Identifier("id", ptr, null);
            var emitter = new CodeEmitterImpl();
            var sub = emitter.ISub(id, 3);
            Assert.AreEqual("(ptr32 (struct \"tmp\" 0010))", sub.DataType.ToString());
            Assert.AreEqual(PrimitiveType.Word32, sub.Right.DataType);
            Assert.AreEqual("id - 0x00000003", sub.ToString());
        }

        [Test]
        public void AddPointer()
        {
            var ptr = new Pointer(new StructureType("tmp", 16), 32);
            var id = new Identifier("id", ptr, null);
            var emitter = new CodeEmitterImpl();
            var add = emitter.IAdd(id, 3);
            Assert.AreEqual(PrimitiveType.Word32, add.DataType);
            Assert.AreEqual(PrimitiveType.Word32, add.Right.DataType);
            Assert.AreEqual("id + 0x00000003", add.ToString());
        }


        [Test]
        public void Cond()
        {
            var emitter = new CodeEmitterImpl();
            var cond = emitter.Cond(new Identifier("id", PrimitiveType.Word32, null));
            Assert.AreEqual("cond(id)", cond.ToString());
        }

        private class CodeEmitterImpl : CodeEmitter
        {
            private Frame frame = new Frame(PrimitiveType.Word32);
            private Block block = new Block(null, "test");

            public override Statement Emit(Instruction instr)
            {
                return new Statement(0, instr, block);
            }

            public override Frame Frame
            {
                get { return frame;  }
            }
        }
	}
}
