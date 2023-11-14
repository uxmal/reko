#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using Reko.Core.Lib;
using Reko.Core.Operators;
using Reko.Core.Types;
using System.Linq;

namespace Reko.UnitTests.Core
{
    [TestFixture]
    public class EndianServicesTests
    {
        private static void AssertHexBytes(string sExp, byte[] actual)
        {
            var sActual = string.Join("", actual.Select(b => $"{b:X2}"));
            Assert.AreEqual(sExp, sActual);
        }

        [Test]
        public void EndSvc_SliceStackIdentifier()
        {
            var stk = new StackStorage(-16, PrimitiveType.Word32);
            var leSlice = EndianServices.Little.SliceStackStorage(stk, new BitRange(0, 8), 8);
            var beSlice = EndianServices.Big.SliceStackStorage(stk, new BitRange(0, 8), 8);
            Assert.AreEqual("Stack -0010", leSlice.ToString());
            Assert.AreEqual("Stack -000D", beSlice.ToString());
        }


        [Test]
        public void EndSvc_ReverseWords()
        {
            var bytes = new byte[] { 0x04, 0x03, 0x02, 0x01, 0x0F, 0xE, 0x0D, 0x0C };
            var newBytes = EndianServices.SwapByGroups(bytes, 4);
            AssertHexBytes("010203040C0D0E0F", newBytes);
        }

        [Test]
        public void EndSvc_SliceMemoryAccess_Address()
        {
            var mem = new MemoryAccess(
                Address.SegPtr(0x1234, 0x0000),
                PrimitiveType.Word32);
            var leSlice = EndianServices.Little.SliceMemoryAccess(
                mem.MemoryId,
                mem.EffectiveAddress,
                mem.DataType,
                new BitRange(0, 16),
                8);
            var beSlice = EndianServices.Big.SliceMemoryAccess(
                mem.MemoryId,
                mem.EffectiveAddress,
                mem.DataType,
                new BitRange(0, 16),
                8);
            Assert.AreEqual("Mem0[1234:0000:word16]", leSlice.ToString());
            Assert.AreEqual("Mem0[1234:0002:word16]", beSlice.ToString());
        }

        [Test]
        public void EndSvc_SliceMemoryAccess_Id()
        {
            var sp = Identifier.Create(RegisterStorage.Reg32("sp", 1));
            var mem = new MemoryAccess(
                sp,
                PrimitiveType.Word32);
            var leSlice = EndianServices.Little.SliceMemoryAccess(
                mem.MemoryId,
                mem.EffectiveAddress,
                mem.DataType,
                new BitRange(0, 16),
                8);
            var beSlice = EndianServices.Big.SliceMemoryAccess(
                mem.MemoryId,
                mem.EffectiveAddress,
                mem.DataType,
                new BitRange(0, 16),
                8);
            Assert.AreEqual("Mem0[sp:word16]", leSlice.ToString());
            Assert.AreEqual("Mem0[sp + 2<i32>:word16]", beSlice.ToString());
        }

        [Test]
        public void EndSvc_SliceMemoryAccess_Id_positive_offset()
        {
            var sp = Identifier.Create(RegisterStorage.Reg32("sp", 1));
            var mem = new MemoryAccess(
                new BinaryExpression(Operator.IAdd, sp.DataType, sp, Constant.Create(sp.DataType, 8)),
                PrimitiveType.Word32);
            var leSlice = EndianServices.Little.SliceMemoryAccess(
                mem.MemoryId,
                mem.EffectiveAddress,
                mem.DataType,
                new BitRange(0, 16),
                8);
            var beSlice = EndianServices.Big.SliceMemoryAccess(
                mem.MemoryId,
                mem.EffectiveAddress,
                mem.DataType,
                new BitRange(0, 16),
                8);
            Assert.AreEqual("Mem0[sp + 8<32>:word16]", leSlice.ToString());
            Assert.AreEqual("Mem0[sp + 8<32> + 2<i32>:word16]", beSlice.ToString());
        }

        [Test]
        public void EndSvc_SliceMemoryAccess_Id_negative_offset()
        {
            var sp = Identifier.Create(RegisterStorage.Reg32("sp", 1));
            var mem = new MemoryAccess(
                new BinaryExpression(Operator.ISub, sp.DataType, sp, Constant.Create(sp.DataType, 8)),
                PrimitiveType.Word32);
            var leSlice = EndianServices.Little.SliceMemoryAccess(
                mem.MemoryId,
                mem.EffectiveAddress,
                mem.DataType,
                new BitRange(0, 16),
                8);
            var beSlice = EndianServices.Big.SliceMemoryAccess(
                mem.MemoryId,
                mem.EffectiveAddress,
                mem.DataType,
                new BitRange(0, 16),
                8);
            Assert.AreEqual("Mem0[sp - 8<32>:word16]", leSlice.ToString());
            Assert.AreEqual("Mem0[sp - 8<32> + 2<i32>:word16]", beSlice.ToString());
        }
    }
}
