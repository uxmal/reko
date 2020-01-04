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

using NUnit.Framework;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.NativeInterface;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Core.NativeInterface
{
    [TestFixture]
    public class NativeRtlEmitterTests
    {
        private List<RtlInstruction> instrs;
        private RtlInstructionCluster rtlc;
        private NativeRtlEmitter m;
        private NativeTypeFactory ntf;

        [SetUp]
        public void Setup()
        {
            this.instrs = new List<RtlInstruction>();
            this.rtlc = new RtlInstructionCluster(Address.Ptr32(0x00123400), 4);
            this.ntf = new NativeTypeFactory();
            this.m = new NativeRtlEmitter(new RtlEmitter(instrs), ntf, null);
        }

        private void AssertInstructions(string sExp, RtlInstructionCluster rtlc)
        {
            var sw = new StringWriter();
            rtlc.Write(sw);
            if (sw.ToString() != sExp)
            {
                Debug.Print(sw.ToString());
            }
            Assert.AreEqual(sExp, sw.ToString());
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

        [Test]
        public void Rtlne_Assign_ForgotSetInstrClass()
        {
            var hDst = m.Mem16(m.Ptr32(0x00123400));
            var hSrc = m.UInt16(0x5678);
            m.Assign(hDst, hSrc);
            try
            {
                var rtlc = m.ExtractCluster();
                Assert.Fail("Expected an exception because we forgot to set the InstrClass of the expression");
            }
            catch (InvalidOperationException)
            {
                return;
            }
        }

        [Test]
        public void Rtlne_Assign()
        {
            var hDst = m.Mem16(m.Ptr32(0x00123400));
            var hSrc = m.UInt16(0x5678);
            m.Assign(hDst, hSrc);
            m.FinishCluster(InstrClass.Linear, 0x00111100, 4);
            var rtlc = m.ExtractCluster();
            var sExp = 
@"00111100(4):
Mem0[0x00123400:word16] = 0x5678
";
            AssertInstructions(sExp, rtlc);
        }

        [Test]
        public void Rtlne_AssignBinExp()
        {
            var hDst = m.Mem16(m.Ptr32(0x00123400));
            var hLeft = m.Mem16(m.Ptr32(0x00123400));
            var hRight = m.UInt16(0x5678);
            m.Assign(hDst, m.IAdd(hLeft,hRight));
            m.FinishCluster(InstrClass.Linear, 0x00111100, 4);
            var rtlc = m.ExtractCluster();
            var sExp =
@"00111100(4):
Mem0[0x00123400:word16] = Mem0[0x00123400:word16] + 0x5678
";
            AssertInstructions(sExp, rtlc);
        }

        [Test]
        public void Rtlne_AddArgsToFn()
        {
            var hArg1 = m.Int16(3);
            var hArg2 = m.Int16(4);
            var hArg3 = m.Int16(5);
            var fn = m.MapToHandle(
                new ProcedureConstant(
                    PrimitiveType.Ptr32,
                    new ExternalProcedure("RightTriangle", new FunctionType())));

            m.AddArg(hArg1);
            m.AddArg(hArg2);
            m.AddArg(hArg3);
            m.SideEffect(m.Fn(fn));

            m.FinishCluster(InstrClass.Linear, 0x00111100, 4);
            var rtlc = m.ExtractCluster();
            var sExp =
@"00111100(4):
RightTriangle(3, 4, 5)
";
            AssertInstructions(sExp, rtlc);
        }
    }
}
