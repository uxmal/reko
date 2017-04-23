#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
using Reko.Core.NativeInterface;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Core.NativeInterface
{
    [TestFixture]
    public class RtlNativeEmitterTests
    {
        private RtlInstructionCluster rtlc;
        private RtlNativeEmitter m;

        [SetUp]
        public void Setup()
        {
            this.rtlc = new RtlInstructionCluster(Address.Ptr32(0x00123400), 4);
            this.m = new RtlNativeEmitter(new RtlEmitter(null));
        }

        [Test]
        public void Rtlne_Int32()
        {
            var hExp = m.Int32(42);

            var c = (Constant) m.GetExpression(hExp);
            Assert.AreEqual(PrimitiveType.Int32, c.DataType);
            Assert.AreEqual(42, c.ToInt32());
        }

        [Test]
        public void Rtlne_Mem32()
        {
            var hExp = m.Mem32(m.Ptr32(0x00123400));

            var mem = (MemoryAccess)m.GetExpression(hExp);
            var ea = (Address)mem.EffectiveAddress;
            Assert.AreEqual("00123400", ea.ToString());
        }
    }
}
