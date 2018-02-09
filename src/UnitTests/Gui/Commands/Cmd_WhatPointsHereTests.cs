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
using Reko.Gui;
using Reko.Gui.Commands;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Gui.Commands
{
    [TestFixture]
    public class Cmd_WhatPointsHereTests
    {
        private MockRepository mr;
        private MemoryArea mem;
        private SegmentMap segmentMap;
        private ServiceContainer sc;
        private Program program;
        private IProcessorArchitecture arch;
        private IPlatform platform;
        private ISearchResultService searchSvc;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
            sc = new ServiceContainer();
            mem = new MemoryArea(Address.Ptr32(0x00400000), new byte[2000]);
            segmentMap = new SegmentMap(mem.BaseAddress);
            arch = mr.Stub<IProcessorArchitecture>();
            platform = mr.Stub<IPlatform>();
            searchSvc = mr.Stub<ISearchResultService>();

            sc.AddService<ISearchResultService>(searchSvc);

            program = new Program
            {
                Architecture = arch,
                Platform = platform,
                SegmentMap = segmentMap,
            };
        }

        private void Given_Segment(string name, Address address, uint size)
        {
            segmentMap.AddSegment(new ImageSegment(name, address, mem, AccessMode.ReadWriteExecute)
            {
                Size = size, 
            });
        }

        private void Given_Pointers(Address[] addresses)
        {
            platform.Stub(s => s.CreatePointerScanner(null, null, null, PointerScannerFlags.All))
                .IgnoreArguments()
                .Return(addresses);
        }


        [Test(Description = "Test when a segment doesn't cover the program image")]
        public void Cmdwph_SmallSegment()
        {
            Given_Segment(".text", Address.Ptr32(0x00401000), 0x0800);
            Given_Pointers(new[] { Address.Ptr32(0x00401800), Address.Ptr32(0x00401804) });
            mr.ReplayAll();

            var cmd = new Cmd_ViewWhatPointsHere(sc, program, new[] { Address.Ptr32(0x00401400) });
            cmd.DoIt();

            mr.VerifyAll();
        }
    }
}
