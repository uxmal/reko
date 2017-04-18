#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
 .
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
using Reko.Gui.Forms;
using Reko.Gui.Windows.Forms;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Gui.Windows.Forms
{
    [TestFixture]
    [Category(Categories.UserInterface)]
    public class SymbolSourceInteractorTests
    {
        private SymbolSourceDialog dlg;
        private MockRepository mr;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
        }

        [TearDown]
        public void TearDown()
        {
            if (dlg != null)
                dlg.Dispose();
            dlg = null;
        }

        [Test]
        public void SymSrcDlg_Load()
        {
            When_CreateDlg();

            Assert.IsFalse(dlg.SymbolSourceList.Enabled);
            Assert.IsFalse(dlg.AssemblyFile.Enabled);
            Assert.IsFalse(dlg.BrowseAssemblyFile.Enabled);
            Assert.IsFalse(dlg.OkButton.Enabled);
            Assert.IsFalse(dlg.SymbolSourceClasses.Enabled);
        }

        private void When_CreateDlg()
        {
            dlg = new SymbolSourceDialog();
            dlg.Show();
        }
    }
}
