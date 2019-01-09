#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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

using Reko.Core;
using Reko.Core.Types;
using Reko.Gui;
using Reko.Gui.Forms;
using Reko.UnitTests.Mocks;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.ComponentModel.Design;

namespace Reko.UnitTests.Gui.Forms
{
	[TestFixture]
	public class FinalPageInteractorTests
	{
        private FinalPageInteractor interactor;
        private IServiceContainer sc;
        private MockRepository mr;
        private FakeComponentSite site;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
            sc = new ServiceContainer();
            site = new FakeComponentSite(sc);
            site.AddService<IDecompilerService>(mr.Stub<IDecompilerService>());
            site.AddService<IDecompilerShellUiService>(mr.Stub<IDecompilerShellUiService>());
            site.AddService<IWorkerDialogService>(mr.Stub<IWorkerDialogService>());
        }

		[Test]
        public void Fpi_ConnectToBrowserService()
		{
            interactor = new FinalPageInteractor(sc);
            mr.ReplayAll();

            interactor.ConnectToBrowserService();

            mr.VerifyAll();
		}
	}
}
