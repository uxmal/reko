#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.ImageLoaders.Elf.Relocators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Core.Expressions
{
    [TestFixture]
    public class MemoryAccessTests
    {
        private ExpressionEmitter m = new ExpressionEmitter();
        private Identifier ptr = Identifier.CreateTemporary("tmp", PrimitiveType.Ptr32);
        private Identifier seg = Identifier.CreateTemporary("seg", PrimitiveType.Ptr16);

        [Test]
        public void MemAcc_Unpack_ConstantEa()
        {
            var mem = m.Mem16(m.Word32(0x123400));
            var (s, o, d) = mem.Unpack();
            Assert.IsNull(s);
            Assert.IsNull(o);
            Assert.AreEqual(0x123400, d);
        }

        [Test]
        public void MemAcc_Unpack_LinearAddress()
        {
            var mem = m.Mem8(Address.Ptr64(0x00123400));
            var (s, o, d) = mem.Unpack();
            Assert.IsNull(s);
            Assert.IsNull(o);
            Assert.AreEqual(0x123400, d);
        }

        [Test]
        public void MemAcc_Unpack_SegmentedAddress()
        {
            var mem = m.Mem8(Address.SegPtr(0x1234, 0x5678));
            var (s, o, d) = mem.Unpack();
            Assert.AreEqual("0x1234<16>", s.ToString());
            Assert.IsNull(o);
            Assert.AreEqual(0x5678, d);
        }

        [Test]
        public void MemAcc_Unpack_Dereference()
        {
            var mem = m.Mem8(ptr);
            var (s, o, d) = mem.Unpack();
            Assert.IsNull(s);
            Assert.AreSame(ptr, o);
            Assert.AreEqual(0, d);
        }

        [Test]
        public void MemAcc_Unpack_Dereference_with_displacement()
        {
            var mem = m.Mem8(m.AddSubSignedInt(ptr, -1));
            var (s, o, d) = mem.Unpack();
            Assert.IsNull(s);
            Assert.AreSame(ptr, o);
            Assert.AreEqual(-1L, d);
        }

    }
}
