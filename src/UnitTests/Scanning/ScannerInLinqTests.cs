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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Scanning
{
    [TestFixture]
    public class ScannerInLinqTests
    {
        private ScanResults sr;

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

        private void Link(int addrFrom ,int addrTo)
        {
            sr.FlatEdges.Add(new ScanResults.link { first = addrFrom, second = addrTo });
        }

        [Test]
        public void Siq_Blocks()
        {
            Inst(100, 2, RtlClass.Linear);
            Link(100, 102);
            Inst(101, 2, RtlClass.Linear);
            Link(101, 103);
            Inst(102, 2, RtlClass.Linear);
            Link(102, 104);
            Inst(103, 2, RtlClass.Invalid);
            Inst(104, 2, RtlClass.Transfer);

            var siq = new ScannerInLinq(null, null, null, null);
            var blocks =siq.BuildBasicBlocks(sr);

            Assert.AreEqual(2, blocks.Count);
        }
    }
}
