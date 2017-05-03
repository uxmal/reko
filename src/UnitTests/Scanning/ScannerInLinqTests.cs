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
using Reko.Core.Rtl;
using Reko.Scanning;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Scanning
{
    [TestFixture]
    public class ScannerInLinqTests
    {
        private ScanResults sr;
        private ScannerInLinq siq;

        [SetUp]
        public void Setup()
        {
            this.sr = new ScanResults();
            this.sr.FlatInstructions = new SortedList<long, ScanResults.instr>();
            this.sr.FlatEdges = new List<ScanResults.link>();
        }

        private void Inst(int addr, int len, RtlClass rtlc)
        {
            sr.FlatInstructions.Add(addr, new ScanResults.instr
            {
                addr = addr,
                size = len,
                block_id = addr,
                type = (ushort)rtlc
            });
        }

        private void Lin(int addr, int len, int next)
        {
            sr.FlatInstructions.Add(addr, new ScanResults.instr
            {
                addr = addr,
                size = len,
                block_id = addr,
                type = (ushort)RtlClass.Linear
            });
            Link(addr, next);
        }


        private void Bra(int addr, int len, int a, int b)
        {
            sr.FlatInstructions.Add(addr, new ScanResults.instr
            {
                addr = addr,
                size = len,
                block_id = addr,
                type = (ushort)RtlClass.Linear
            });
            Link(addr, a);
            Link(addr, b);
        }

        private void Bad(int addr, int len)
        {
            sr.FlatInstructions.Add(addr, new ScanResults.instr
            {
                addr = addr,
                size = len,
                block_id = addr,
                type = (ushort)RtlClass.Invalid,
            });
        }

        private void End(int addr, int len)
        {
            sr.FlatInstructions.Add(addr, new ScanResults.instr
            {
                addr = addr,
                size = len,
                block_id = addr,
                type = (ushort)RtlClass.Transfer,
            });
        }

        private void Link(int addrFrom ,int addrTo)
        {
            sr.FlatEdges.Add(new ScanResults.link { first = addrFrom, second = addrTo });
        }

        private void Given_OverlappingLinearTraces()
        {
            Lin(0x100, 2, 0x102);
            Lin(0x101, 2, 0x103);
            Lin(0x102, 2, 0x104);
            Bad(0x103, 2);
            End(0x104, 2);
        }

        private void CreateScanner()
        {
            this.siq = new ScannerInLinq(null, null, null, null);
        }


        [Test]
        public void Siq_Blocks()
        {
            Given_OverlappingLinearTraces();

            CreateScanner();
            var blocks = siq.BuildBasicBlocks(sr);

            Assert.AreEqual(2, blocks.Count);
        }

        [Test]
        public void Siq_InvalidBlocks()
        {
            Given_OverlappingLinearTraces();

            CreateScanner();
            var blocks = siq.BuildBasicBlocks(sr);
            blocks = siq.RemoveInvalidBlocks(sr, blocks);

            var sExp =
            #region Expected
@"00000100-00000106 (6): 
";
            #endregion

            AssertBlocks(sExp, blocks);
        }

        [Test]
        public void Siq_ShingledBlocks()
        {
            Lin(0x0001B0D7, 2, 0x0001B0D9);
            Lin(0x0001B0D8, 6, 0x0001B0DE);
            Lin(0x0001B0D9, 1, 0x0001B0DA);
            Lin(0x0001B0DA, 1, 0x0001B0DB);
            Bra(0x0001B0DB, 2, 0x0001B0DD, 0x0001B0DE);

            Lin(0x0001B0DC, 2, 0x0001B0DE);
            Lin(0x0001B0DD, 1, 0x0001B0DE);

            End(0x0001B0DE, 2);

            CreateScanner();
            var blocks = siq.BuildBasicBlocks(sr);
            var sExp =
            #region Expected
@"0001B0D7-0001B0DD (6): 0001B0DD, 0001B0DE
0001B0D8-0001B0DE (6): 0001B0DE
0001B0DC-0001B0DE (2): 0001B0DE
0001B0DD-0001B0DE (1): 0001B0DE
0001B0DE-0001B0E0 (2): 
";
            #endregion
            AssertBlocks(sExp, blocks);
        }

        private void AssertBlocks(string sExp, Dictionary<long, ScannerInLinq.block> blocks)
        {
            var sw = new StringWriter();
            this.siq.DumpBlocks(sr, blocks, sw.WriteLine);
            var sActual = sw.ToString();
            if (sExp != sActual)
            {
                Debug.WriteLine("* Failed AssertBlocks ***");
                Debug.Write(sActual);
                Assert.AreEqual(sExp, sActual);
            }
        }
    }
}
