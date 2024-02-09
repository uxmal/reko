#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Core.Services;
using Reko.Gui;
using Reko.Gui.Forms;
using Reko.Gui.Services;
using Reko.Services;
using Reko.UnitTests.Mocks;
using System;
using System.ComponentModel.Design;
using System.Threading.Tasks;

namespace Reko.UnitTests.Gui.Forms
{
    [TestFixture]
    [Category(Categories.UserInterface)]
	public class InitialPageInteractorTests
    {
        private Mock<IMainForm> form;
		private TestInitialPageInteractor i;
        private Mock<IDecompilerShellUiService> uiSvc;
        private ServiceContainer sc;
        private Mock<IProjectBrowserService> browserSvc;
        private Mock<ILoader> loader;
        private Mock<IDecompiler> dec;
        private Mock<IDecompiledFileService> host;
        private Mock<ILowLevelViewService> memSvc;
        private Mock<IFileSystemService> fsSvc;
        private Mock<IArchiveBrowserService> abSvc;
        private Program program;
        private Project project;

		[SetUp]
		public void Setup()
		{
            form = new Mock<IMainForm>();
            sc = new ServiceContainer();
            loader = new Mock<ILoader>();
            dec = new Mock<IDecompiler>();
            sc = new ServiceContainer();
            uiSvc = new Mock<IDecompilerShellUiService>();
            host = new Mock<IDecompiledFileService>();
            memSvc = new Mock<ILowLevelViewService>();
            fsSvc = new Mock<IFileSystemService>();
            abSvc = new Mock<IArchiveBrowserService>();

            var mem = new ByteMemoryArea(Address.Ptr32(0x10000), new byte[1000]);
            var segmentMap = new SegmentMap(
                mem.BaseAddress,
                new ImageSegment("code", mem, AccessMode.ReadWriteExecute));
            var arch = new Mock<IProcessorArchitecture>();
            arch.Setup(a => a.Name).Returns("FakeArch");
            var platform = new Mock<IPlatform>();
            program = new Program(new ProgramMemory(segmentMap), arch.Object, platform.Object);
            project = new Project { Programs = { program } };

            browserSvc = new Mock<IProjectBrowserService>();

            sc.AddService<IDecompilerUIService>(uiSvc.Object);
            sc.AddService<IDecompilerShellUiService>(new Mock<IDecompilerShellUiService>().Object);
            sc.AddService<IDecompilerService>(new DecompilerService());
            sc.AddService<IWorkerDialogService>(new FakeWorkerDialogService());
            sc.AddService<IEventListener>(new FakeDecompilerEventListener());
            sc.AddService<IProjectBrowserService>(browserSvc.Object);
            sc.AddService<ILowLevelViewService>(memSvc.Object);
            sc.AddService<ILoader>(loader.Object);
            sc.AddService<IDecompiledFileService>(host.Object);
            sc.AddService<IFileSystemService>(fsSvc.Object);
            sc.AddService<IArchiveBrowserService>(abSvc.Object);
            sc.AddService<IProcedureListService>(new Mock<IProcedureListService>().Object);
            i = new TestInitialPageInteractor(sc, dec.Object);
		}

		[TearDown]
		public void Teardown()
		{
			form.Object.Dispose();
		}

        [Test]
        public async Task Ipi_OpenBinary_CanAdvance()
        {
            Assert.IsFalse(i.CanAdvance);

            loader.Setup(l => l.Load(ImageLocation.FromUri("file:floxe.exe"), null, null)).Returns(project);
            dec.Setup(d => d.Project).Returns(project);
            browserSvc.Setup(b => b.Load(project));
            memSvc.Setup(m => m.ViewImage(program));

            await i.OpenBinary("floxe.exe");

            Assert.IsTrue(i.CanAdvance, "Page should be ready to advance");
        }

        [Test]
        public async Task Ipi_OpenBinary_ShouldPopulateFields()
        {
            dec.Setup(d => d.Project).Returns(project);
            browserSvc.Setup(b => b.Load(project));

            Assert.IsFalse(i.CanAdvance, "Page should not be ready to advance");

            loader.Setup(l => l.Load(ImageLocation.FromUri("file:floxe.exe"), null, null)).Returns(project);
            memSvc.Setup(m => m.ViewImage(program));

            await i.OpenBinary("floxe.exe");

            Assert.IsTrue(i.CanAdvance, "Page should be ready to advance");
        }

        [Test]
        public async Task Ipi_OpenBinary_ShouldShowMemoryWindow()
        {
            loader.Setup(l => l.Load(ImageLocation.FromUri("file:floxe.exe"), null, null)).Returns(project);
            dec.Setup(d => d.Project).Returns(project);
            browserSvc.Setup(d => d.Load(project));
            memSvc.Setup(s => s.ViewImage(program)).Verifiable();

            await i.OpenBinary("floxe.exe");

            memSvc.VerifyAll();
        }

        [Test]
        public async Task Ipi_OpenBinary_ShouldBrowseProject()
        {
            loader.Setup(l => l.Load(ImageLocation.FromUri("file:foo.exe"), null, null)).Returns(project);
            dec.Setup(d => d.Project).Returns(project);
            browserSvc.Setup(b => b.Load(project)).Verifiable();
            memSvc.Setup(m => m.ViewImage(program));

            await i.OpenBinary("foo.exe");

            browserSvc.VerifyAll();
        }

        [Test]
        public async Task Ipi_OpenBinary_ShouldOpenArchiveDialog()
        {
            var archive = new Mock<IArchive>();
            var archiveFile = new Mock<ArchivedFile>();
            program.Location = ImageLocation.FromUri("file:test.archive#dir/test.exe");

            loader.Setup(l => l.Load(ImageLocation.FromUri("file:test.archive"), null, null))
                .Returns(archive.Object);
            archiveFile.Setup(l => l.LoadImage(sc, null))
                .Returns(program)
                .Verifiable();
            abSvc.Setup(a => a.SelectFileFromArchive(archive.Object))
                .Returns(ValueTask.FromResult(archiveFile.Object))
                .Verifiable();

            await i.OpenBinary("test.archive");

            archiveFile.VerifyAll();
            abSvc.VerifyAll();
        }

        [Test]
        public async Task Ipi_LeavePage()
        {
            loader.Setup(l => l.Load(ImageLocation.FromUri("file:foo.exe"), null, null)).Returns(project);
            dec.Setup(d => d.Project).Returns(project);
            browserSvc.Setup(b => b.Load(project));
            memSvc.Setup(m => m.ViewImage(program));

            await i.OpenBinary("foo.exe");

            Assert.IsTrue(i.LeavePage());
        }

        [Test]
        public async Task Ipi_NextPhaseButton_ScanningNotNeeded()
        {
            program.NeedsScanning = false;
            loader.Setup(l => l.Load(ImageLocation.FromUri("file:foo.exe"), null, null)).Returns(project);
            dec.Setup(d => d.Project).Returns(project);
            browserSvc.Setup(b => b.Load(project));

            await i.OpenBinary("foo.exe");

            var status = new CommandStatus();
            var text = new CommandText("&");
            Assert.IsTrue(i.QueryStatus(new CommandID(CmdSets.GuidReko, (int)CmdIds.ActionNextPhase), status, text));
            Assert.AreEqual("A&nalyze dataflow", text.Text);
        }

        [Test]
        public async Task Ipi_FinishDecompilationButton()
        {
            program.NeedsScanning = false;
            loader.Setup(l => l.Load(ImageLocation.FromUri("file:foo.exe"), null, null)).Returns(project);
            dec.Setup(d => d.Project).Returns(project);
            browserSvc.Setup(b => b.Load(project));
            var status = new CommandStatus();
            var cmd = new CommandID(
                CmdSets.GuidReko, (int)CmdIds.ActionFinishDecompilation);

            Assert.IsTrue(i.QueryStatus(cmd, status, null));
            Assert.AreEqual(MenuStatus.Visible, status.Status);

            await i.OpenBinary("foo.exe");

            Assert.IsTrue(i.QueryStatus(cmd, status, null));
            Assert.AreEqual(MenuStatus.Visible | MenuStatus.Enabled, status.Status);
        }

        private class TestInitialPageInteractor : InitialPageInteractorImpl
        {
            public IDecompiler decompiler;

            public TestInitialPageInteractor(IServiceProvider services, IDecompiler decompiler) : base(services)
            {
                this.decompiler = decompiler;
            }

            protected override IDecompiler CreateDecompiler(Project project)
            {
                return decompiler;
            }
        }
	}
}
