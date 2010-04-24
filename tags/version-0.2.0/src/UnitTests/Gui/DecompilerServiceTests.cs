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

using Decompiler;
using Decompiler.Core;
using Decompiler.Gui;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;

namespace Decompiler.UnitTests.Gui
{
    [TestFixture]
    public class DecompilerServiceTests
    {
        ServiceContainer sc;
        IDecompilerService svc;

        [SetUp]
        public void Setup()
        {
            sc = new ServiceContainer();
            svc = new DecompilerService();
            sc.AddService(typeof(IDecompilerService), svc);
            sc.AddService(typeof(DecompilerEventListener), new FakeDecompilerEventListener());
        }
        
        [Test]
        public void NotifyOnChangedDecompiler()
        {
            DecompilerDriver d = new DecompilerDriver(null, null, sc);
            bool decompilerChangedEventFired = true;
            svc.DecompilerChanged += delegate(object o, EventArgs e)
            {
                decompilerChangedEventFired = true;
            };

            svc.Decompiler = d;

            Assert.IsTrue(decompilerChangedEventFired, "Should have fired a change event");
        }

        [Test]
        public void EmptyDecompilerProjectName()
        {
            IDecompilerService svc = new DecompilerService();
            Assert.IsEmpty(svc.ProjectName, "Shouldn't have project name available.");
        }

        [Test]
        public void DecompilerProjectName()
        {
            IDecompilerService svc = new DecompilerService();

            svc.Decompiler = new FakeDecompiler(new FakeLoader());
            svc.Decompiler.LoadProgram("foo\\bar\\baz.exe");
            Assert.IsNotNull(svc.Decompiler.Project);
            Assert.AreEqual("baz.exe",  svc.ProjectName, "Should have project name available.");
        }
    }
}
