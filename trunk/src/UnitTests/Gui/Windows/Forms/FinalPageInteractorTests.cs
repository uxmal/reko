#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Types;
using Decompiler.Gui;
using Decompiler.Gui.Windows.Forms;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.ComponentModel.Design;

namespace Decompiler.UnitTests.Gui.Windows.Forms
{
	[TestFixture]
	public class FinalPageInteractorTests
	{
        private FinalPageInteractor interactor;
        private IServiceContainer sc;
        private MockRepository repository;
        private FakeComponentSite site;

        [SetUp]
        public void Setup()
        {
            repository = new MockRepository();
            sc = new ServiceContainer();
            site = new FakeComponentSite(sc);
            site.AddService<IDecompilerService>(repository.Stub<IDecompilerService>());
            site.AddService<IDecompilerShellUiService>(repository.Stub<IDecompilerShellUiService>());
            site.AddService<IWorkerDialogService>(repository.Stub<IWorkerDialogService>());
        }

		[Test]
		public void ConnectToBrowserService()
		{
            interactor = new FinalPageInteractor();
            repository.ReplayAll();

            interactor.Site = site;
            interactor.ConnectToBrowserService();

            repository.VerifyAll();
		}

        [Test]
        public void ShowCodeWindowWhenUserSelectsFunction()
        {
            interactor = new FinalPageInteractor();
            var proc = Procedure.Create(new Address(0x12345), new Frame(PrimitiveType.Word32));
            var codeService = repository.DynamicMock<ICodeViewerService>();
            codeService.Expect(x => x.DisplayProcedure(
                Arg<Procedure>.Is.Same(proc)));
            site.AddService<ICodeViewerService>(codeService);

            repository.ReplayAll();

            interactor.Site = site;
            interactor.ConnectToBrowserService();
            repository.VerifyAll();
        }
	}
}
