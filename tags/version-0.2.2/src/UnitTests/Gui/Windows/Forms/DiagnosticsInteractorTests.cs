/* 
 * Copyright (C) 1999-2010 John Källén.
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

using Decompiler.Core;
using Decompiler.Gui;
using Decompiler.Gui.Windows.Forms;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.UnitTests.Gui.Windows.Forms
{
    [TestFixture]
    public class DiagnosticsInteractorTests
    {
        private ListView lv;
        private DiagnosticsInteractor interactor;
        private IDiagnosticsService svc;

        [SetUp]
        public void Setup()
        {
            lv = new ListView();
            interactor = new DiagnosticsInteractor();
            interactor.Attach(lv);
            svc = interactor;
        }

        [TearDown]
        public void Teardown()
        {
            lv.Dispose();
        }

        [Test]
        public void AddDiagnostic()
        {
            svc.AddDiagnostic(new WarningDiagnostic(new Address(0x100), "test"));
            Assert.AreEqual(1, lv.Items.Count);
        }

        [Test]
        public void ClearDiagnostics()
        {
            svc.AddDiagnostic(new WarningDiagnostic(new Address(0x100), "test"));
            svc.ClearDiagnostics();
            Assert.AreEqual(0, lv.Items.Count);
        }
    }
}
