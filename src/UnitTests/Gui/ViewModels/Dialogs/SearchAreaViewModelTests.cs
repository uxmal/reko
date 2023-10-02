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

using Moq;
using NUnit.Framework;
using Reko.Core;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Gui.Services;
using Reko.Gui.ViewModels.Dialogs;
using Reko.Scanning;
using Reko.UnitTests.Mocks;
using System.ComponentModel.Design;

namespace Reko.UnitTests.Gui.ViewModels.Dialogs
{
    [TestFixture]
    public class SearchAreaViewModelTests
    {
        private readonly SegmentMap segmentMap;
        private readonly FakeArchitecture arch;
        private readonly DefaultPlatform platform;
        private readonly Program program;
        private readonly Mock<IUiPreferencesService> uiPreferencesSvc;

        public SearchAreaViewModelTests()
        {
            this.segmentMap = new SegmentMap(
                new ImageSegment(".text", new ByteMemoryArea(Address.Ptr32(0x1000), new byte[0x200]), AccessMode.ReadExecute),
                new ImageSegment(".data", new ByteMemoryArea(Address.Ptr32(0x2000), new byte[0x200]), AccessMode.ReadWrite));
            this.arch = new FakeArchitecture();
            var sc = new ServiceContainer();
            this.platform = new DefaultPlatform(sc, arch);
            this.program = new Program(segmentMap, arch, platform);
            this.uiPreferencesSvc = new Mock<IUiPreferencesService>();
        }

        [Test]
        public void Savm_Parse_ClosedRange()
        {
            var savm = new SearchAreaViewModel(program, new(), uiPreferencesSvc.Object);

            savm.FreeFormAreas = " [1000 - 103F]";

            Assert.AreEqual(1, savm.Areas.Count);
            Assert.AreEqual(0x1000, savm.Areas[0].Address.Offset);
            Assert.AreEqual(0x40, savm.Areas[0].Length);
            Assert.AreEqual("", savm.FreeFormError);
        }

        [Test]
        public void Savm_Parse_IncompleteRange()
        {
            var savm = new SearchAreaViewModel(program, new(), uiPreferencesSvc.Object);

            savm.FreeFormAreas = " [1000 - 103F";

            Assert.AreEqual(0, savm.Areas.Count);
            Assert.AreEqual("Invalid range syntax.", savm.FreeFormError);
        }
    }
}
