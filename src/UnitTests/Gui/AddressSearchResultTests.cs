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

using Moq;
using NUnit.Framework;
using Reko.Core;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Gui;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;

namespace Reko.UnitTests.Gui
{
    [TestFixture]
    public class AddressSearchResultTests
    {
        private ServiceContainer sc;
        private Program program;

        [SetUp]
        public void Setup()
        {
            sc = new ServiceContainer();
            var mem = new ByteMemoryArea(Address.SegPtr(0xC00, 0), Enumerable.Range(0x0, 0x100).Select(b => (byte)b).ToArray());
            var imageMap = new SegmentMap(
                    mem.BaseAddress,
                    new ImageSegment(
                        "code", mem, AccessMode.ReadWriteExecute));
            var arch = new Mocks.FakeArchitecture(sc);
            this.program = new Program(new ProgramMemory(imageMap), arch, new DefaultPlatform(sc, arch));
        }

        [Test]
        public void Asr_Create()
        {
            var results = new AddressSearchResult(sc, new List<AddressSearchHit>(), new CodeSearchDetails());
        }
    }
}