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
using Reko.Core.Rtl;
using Reko.Core.Types;
using Reko.Arch.M68k;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Arch.M68k
{
    [TestFixture]
    public class OperandRewriterTests
    {
        private M68kArchitecture arch;
        private Rewriter rw;
        private Address addrInstr;

        [SetUp]
        public void Setup()
        {
            this.arch = new M68kArchitecture("m68k");
            this.addrInstr = Address.Ptr32(0x0012340C);
            this.rw = new Rewriter(this.arch, null, new M68kState(arch), new Frame(arch.FramePointerType), null);
        }

        [Test]
        public void M68korw_MemOp()
        {
            var orw = new OperandRewriter(
                arch,
                new RtlEmitter(new List<RtlInstruction>()),
                new Frame(arch.FramePointerType),
                PrimitiveType.Byte);
            var exp = orw.RewriteSrc(new MemoryOperand(PrimitiveType.Byte, Registers.a1, Constant.Int16(4)), addrInstr);

            var ea = (BinaryExpression) ((MemoryAccess) exp).EffectiveAddress;
            Assert.AreEqual(ea.Left.DataType.Size, ea.Right.DataType.Size);
        }
    }
}
