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

using NUnit.Framework;
using Reko.Core;
using Reko.Gui.ViewModels.Dialogs;
using Reko.UnitTests.Mocks;
using System.Linq;

namespace Reko.UnitTests.Gui.ViewModels.Dialogs
{
    internal class SearchAreaTests
    {
        private readonly Program program;

        public SearchAreaTests()
        {
            this.program = new Program()
            {
                Architecture = new FakeArchitecture()
            };
        }

        [Test]
        public void Sa_TryParse_ClosedRange()
        {
            Assert.IsTrue(SearchArea.TryParse(program, " [1000 - 103F]", out var sa));

            Assert.AreEqual(1, sa.Areas.Count);
            Assert.AreEqual(0x1000, sa.Areas[0].Address.Offset);
            Assert.AreEqual(0x40, sa.Areas[0].Length);
        }

        [Test]
        public void Sa_TryParse_IncompleteRange()
        {
            Assert.IsFalse(SearchArea.TryParse(program, " [1000 - 103F", out _));
        }
    }
}
