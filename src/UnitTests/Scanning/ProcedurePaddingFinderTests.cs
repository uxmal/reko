#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
    public class ProcedurePaddingFinderTests
    {
        private ScanResults sr;

        [SetUp]
        public void Setup()
        {
            this.sr = new ScanResults
            {
                FlatInstructions = new SortedList<Address, ScanResults.instr>(),
                FlatEdges = new List<ScanResults.link>(),
                KnownProcedures = new HashSet<Address>(),
                DirectlyCalledAddresses = new Dictionary<Address, int>(),
            };
        }

        private void Ret(uint uAddr, int byteSize)
        {
            var addr = Address.Ptr32(uAddr);
            Instr(addr, byteSize, RtlClass.Transfer, new RtlReturn(0, 0, RtlClass.Transfer));
        }

        private void Pad(uint uAddr, int byteSize)
        {
            var addr = Address.Ptr32(uAddr);
            Instr(addr, byteSize, RtlClass.Padding | RtlClass.Linear, new RtlNop());
            sr.FlatEdges.Add(new ScanResults.link { first = addr, second = addr + byteSize });
        }

        private void Lin(uint uAddr, int byteSize)
        {
            var addr = Address.Ptr32(uAddr);
            Instr(addr, byteSize, RtlClass.Linear, new RtlAssignment(null,null));
            sr.FlatEdges.Add(new ScanResults.link { first = addr, second = addr + byteSize });
        }

        void Instr(Address addr, int byteSize, RtlClass rtlc, params RtlInstruction [] rtls)
        {
            var instr = new ScanResults.instr
            {
                addr = addr,
                size = byteSize,
                type = (ushort)rtlc,
                block_id = addr,
                rtl = new RtlInstructionCluster(addr, byteSize, new RtlReturn(0, 0, RtlClass.Transfer))
                {
                    Class = rtlc,
                }
            };
            sr.FlatInstructions.Add(addr, instr);
            addr += byteSize;
        }

        private void BuildTest()
        {
            var blocks = ScannerInLinq.BuildBasicBlocks(sr);
            blocks = ScannerInLinq.RemoveInvalidBlocks(sr, blocks);
            sr.ICFG = ScannerInLinq.BuildIcfg(sr, blocks);
        }

        private void RunTest(string sExp)
        {
            var ppf = new ProcedurePaddingFinder(sr);
            var padding = ppf.FindPaddingBlocks();
            ppf.Remove(padding);
        }

        [Test]
        public void Ppf_NopPadding()
        {
            Ret(0x1000, 1); // end of preceding function.
            Pad(0x1001, 1);
            Pad(0x1002, 1);
            Lin(0x1003, 1);

            BuildTest();
            var ppf = new ProcedurePaddingFinder(sr);
            var padding = ppf.FindPaddingBlocks().ToList();
            Assert.AreEqual(1, padding.Count, "There should be one padding block...");
            Assert.AreEqual(2, padding[0].Instructions.Count, "...with two instructions inside.");
        }

        [Test]
        public void Ppf_OverlappingPadding()
        {
            Ret(0x1000, 1); // end of preceding function.
            Pad(0x1001, 1); // Unreachable padding.
            Pad(0x1002, 1);
            Lin(0x1003, 1);

            BuildTest();
            var ppf = new ProcedurePaddingFinder(sr);
            var padding = ppf.FindPaddingBlocks().ToList();
            Assert.AreEqual(1, padding.Count, "There should be one padding block...");
            Assert.AreEqual(2, padding[0].Instructions.Count, "...with two instructions inside.");
        }
    }
}
