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
using Decompiler.Core.Services;
using Decompiler.Gui;
using Decompiler.Gui.Forms;
using Decompiler.Loading;
using Decompiler.UnitTests.Mocks;
using Decompiler.Gui.Controls;
using Decompiler.Gui.Windows;
using Decompiler.Gui.Windows.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.ComponentModel.Design;

namespace Decompiler.UnitTests.Gui.Windows.Forms
{
	[TestFixture]
	public class InitialPageInteractorTests
	{
        private MockRepository mr;
        private IMainForm form;
		private TestInitialPageInteractor i;
        private FakeUiService uiSvc;
        private ServiceContainer site;
        private IProjectBrowserService browserSvc;
        private LoaderBase loader;
        private IDecompiler dec;
        private DecompilerHost host;
        private ILowLevelViewService memSvc;
        private Program program;
        private Project project;

		[SetUp]
		public void Setup()
		{
            mr = new MockRepository();
            form = new MainForm();
            site = new ServiceContainer();
            loader = mr.StrictMock<LoaderBase>(site);
            dec = mr.StrictMock<IDecompiler>();
            site = new ServiceContainer();
            uiSvc = new FakeShellUiService();
            host = mr.StrictMock<DecompilerHost>();
            memSvc = mr.StrictMock<ILowLevelViewService>();
            var image = new LoadedImage(new Address(0x10000), new byte[1000]);
            var imageMap = new ImageMap(image);
            var arch = mr.StrictMock<IProcessorArchitecture>();
            var platform = mr.StrictMock<Platform>(null, null);
            program = new Program(image, imageMap, arch, platform);
            project = new Project();

            browserSvc = mr.StrictMock<IProjectBrowserService>();

            site.AddService<IDecompilerUIService>(uiSvc);
            site.AddService(typeof(IDecompilerShellUiService), uiSvc);
            site.AddService(typeof(IDecompilerService), new DecompilerService());
            site.AddService(typeof(IWorkerDialogService), new FakeWorkerDialogService());
            site.AddService(typeof(DecompilerEventListener), new FakeDecompilerEventListener());
            site.AddService(typeof(IProjectBrowserService), browserSvc);
            site.AddService(typeof(ILowLevelViewService), memSvc);

            i = new TestInitialPageInteractor(site, loader, dec);

		}

		[TearDown]
		public void Teardown()
		{
			form.Dispose();
		}

        [Test]
        public void Ipi_OpenBinary_CanAdvance()
        {
            mr.ReplayAll();

            Assert.IsFalse(i.CanAdvance);

            mr.Record();

            dec.Stub(d => d.LoadProgram("floxe.exe"));
            dec.Stub(d => d.Project).Return(project);
            dec.Stub(d => d.Program).Return(program);
            browserSvc.Stub(b => b.Load(project, program));
            memSvc.Stub(m => m.ViewImage(program));
            mr.ReplayAll();

            i.OpenBinary("floxe.exe", host);
            Assert.IsTrue(i.CanAdvance, "Page should be ready to advance");
            mr.VerifyAll();
        }

        [Test]
        public void Ipi_OpenBinary_ShouldPopulateFields()
        {
            dec.Stub(d => d.Project).Return(null);
            dec.Stub(d => d.Program).Return(null);
            mr.ReplayAll();

            Assert.IsFalse(i.CanAdvance, "Page should not be ready to advance");

            mr.Record();
            dec.Stub(d => d.LoadProgram("floxe.exe"));
            mr.ReplayAll();

            i.OpenBinary("floxe.exe", host);

            Assert.IsTrue(i.CanAdvance, "Page should be ready to advance");
            mr.VerifyAll();
        }

        [Test]
        public void Ipi_OpenBinary_ShouldShowMemoryWindow()
        {
            dec.Stub(d => d.LoadProgram("floxe.exe"));
            dec.Stub(d => d.Program).Return(program);
            dec.Stub(d => d.Project).Return(project);
            browserSvc.Stub(d => d.Load(project, program));
            memSvc.Expect(s => s.ViewImage(program));
            mr.ReplayAll();

            i.OpenBinary("floxe.exe", host);

            mr.VerifyAll();
        }

        [Test]
        public void Ipi_OpenBinary_ShouldBrowseProject()
        {
            dec.Stub(d => d.LoadProgram("foo.exe"));
            dec.Stub(d => d.Program).Return(program);
            dec.Stub(d => d.Project).Return(project);
            browserSvc.Expect(b => b.Load(project, program));
            memSvc.Stub(m => m.ViewImage(program));
            mr.ReplayAll();

            i.OpenBinary("foo.exe", new FakeDecompilerHost());

            mr.VerifyAll();
        }

        [Test]
        public void Ipi_LeavePage()
        {
            dec.Expect(d => d.LoadProgram("foo.exe"));
            dec.Stub(d => d.Program).Return(program);
            dec.Stub(d => d.Project).Return(project);
            browserSvc.Stub(b => b.Load(project, program));
            memSvc.Stub(m => m.ViewImage(program));
            mr.ReplayAll();

            i.OpenBinary("foo.exe", host);
            Assert.IsTrue(i.LeavePage());

            mr.VerifyAll();
        }

        //$REFACTOR: copied from LoadedPageInteractor, should
        // push to base class or utility class.
        private MenuStatus QueryStatus(int cmdId)
        {
            CommandStatus status = new CommandStatus();
            i.QueryStatus(new CommandID(CmdSets.GuidDecompiler, cmdId), status, null);
            return status.Status;
        }

        private ILowLevelViewService AddFakeMemoryViewService()
        {
            return memSvc;
        }

        private class TestInitialPageInteractor : InitialPageInteractorImpl
        {
            private LoaderBase loader;
            public IDecompiler decompiler;

            public TestInitialPageInteractor(IServiceProvider services, LoaderBase loader, IDecompiler decompiler) : base(services)
            {
                this.loader = loader;
                this.decompiler = decompiler;
            }

            protected override LoaderBase CreateLoader()
            {
                return loader;
            }

            protected override IDecompiler CreateDecompiler(LoaderBase ldr, DecompilerHost host)
            {
                return decompiler;
            }
        }
	}
}
