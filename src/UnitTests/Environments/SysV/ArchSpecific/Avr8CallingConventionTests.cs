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

using static Reko.Core.Types.PrimitiveType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Reko.Environments.SysV.ArchSpecific;
using Reko.Arch.Avr;
using Reko.Core;
using Reko.Core.Types;

namespace Reko.UnitTests.Environments.SysV.ArchSpecific
{
    [TestFixture]
    public class Avr8CallingConventionTests
    {
        private Avr8Architecture arch;
        private Avr8CallingConvention cc;
        private CallingConventionEmitter ccr;

        [SetUp]
        public void Setup()
        {
            this.arch = new Avr8Architecture("avr8");
        }

        private void Given_CallingConvention()
        {
            this.cc = new Avr8CallingConvention(arch);
            this.ccr = new CallingConventionEmitter();
        }

        [Test] 
        public void SvAvr8cc_ByteArg()
        {
            Given_CallingConvention();
            cc.Generate(ccr, VoidType.Instance, null, new List<DataType>
            {
                PrimitiveType.Byte
            });
            Assert.AreEqual("Stk: 0 void (r24)", ccr.ToString());
        }

        [Test]
        public void SvAvr8cc_Byte_Byte_args()
        {
            Given_CallingConvention();
            cc.Generate(ccr, VoidType.Instance, null, new List<DataType>
            {
                PrimitiveType.Byte, PrimitiveType.Byte
            });
            Assert.AreEqual("Stk: 0 void (r24, r22)", ccr.ToString());
        }

        [Test]
        public void SvAvr8cc_word16_args()
        {
            Given_CallingConvention();
            cc.Generate(ccr, VoidType.Instance, null, new List<DataType>
            {
                Word16
            });
            Assert.AreEqual("Stk: 0 void (Sequence r25:r24)", ccr.ToString());
        }

        [Test]
        public void SvAvr8cc_word32_args()
        {
            Given_CallingConvention();
            cc.Generate(ccr, VoidType.Instance, null, new List<DataType>
            {
                Word32
            });
            Assert.AreEqual("Stk: 0 void (Sequence r25:r24:r23:r22)", ccr.ToString());
        }

        [Test]
        public void SvAvr8cc_mem_args()
        {
            Given_CallingConvention();
            cc.Generate(ccr, VoidType.Instance, null, new List<DataType>
            {
                Word32,
                Word32,
                Word32,
                Word32,
                Word32,
                Word16,
            });
            Assert.AreEqual("Stk: 0 void ("+
                "Sequence r25:r24:r23:r22, " +
                "Sequence r21:r20:r19:r18, " +
                "Sequence r17:r16:r15:r14, " +
                "Sequence r13:r12:r11:r10, " +
                "Stack +0002, " +
                "Stack +0006" +
                ")", ccr.ToString());
        }

        [Test]
        public void SvAvr8cc_return_byte()
        {
            Given_CallingConvention();
            cc.Generate(ccr, PrimitiveType.SByte, null, new List<DataType>());
            Assert.AreEqual("Stk: 0 r24 ()", ccr.ToString());
        }

        [Test]
        public void SvAvr8cc_return_word32()
        {
            Given_CallingConvention();
            cc.Generate(ccr, Word32, null, new List<DataType>());
            Assert.AreEqual("Stk: 0 Sequence r25:r24:r23:r22 ()", ccr.ToString());
        }
    }
}
