#region License
/* 
 * Copyright (C) 1999-2011 John Källén.
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

using Decompiler.Gui;
using Decompiler.Gui.Windows;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.UnitTests.Gui.Windows
{
    [TestFixture]
    public class ProgramImageBrowserServiceTests
    {
        private ListView listView;

        [Test]
        public void CaptionProperty()
        {
            listView = new ListView();
            listView.View = View.Details;
            listView.Columns.Add("");
            IProgramImageBrowserService svc = new ProgramImageBrowserService(listView);
            svc.Caption = "Test";
            Assert.AreEqual("Test", listView.Columns[0].Text);
        }
    }
}
