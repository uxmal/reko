#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using Reko.Gui;
using Reko.Gui.Forms;
using Reko.UnitTests.Mocks;
using System.ComponentModel.Design;

namespace Reko.UnitTests.Gui.Forms
{
    [TestFixture]
	public class FinalPageInteractorTests
	{
        private FinalPageInteractor interactor;
        private IServiceContainer sc;
        private FakeComponentSite site;

        [SetUp]
        public void Setup()
        {
            sc = new ServiceContainer();
            site = new FakeComponentSite(sc);
            site.AddService<IDecompilerService>(new Mock<IDecompilerService>().Object);
            site.AddService<IDecompilerShellUiService>(new Mock<IDecompilerShellUiService>().Object);
            site.AddService<IWorkerDialogService>(new Mock<IWorkerDialogService>().Object);
        }

		[Test]
        public void Fpi_ConnectToBrowserService()
		{
            interactor = new FinalPageInteractor(sc);

            interactor.ConnectToBrowserService();
		}
	}
}
