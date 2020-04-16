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

using Reko.Core.Configuration;
using Reko.Gui;
using Reko.Gui.Controls;
using Reko.Gui.Forms;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Gui.Windows.Forms
{
    [TestFixture]
    [Ignore("Not worth repairing; unit testing GUIs is hard.")]
    public class OpenAsInteractorTests
    {
#if TEST_GUI
        private IOpenAsDialog dlg;
        private ListOption[] archNames;
        private ListOption[] platformNames;
        private ListOption[] rawFileNames;
        private IConfigurationService dcSvc;
        private IComboBox ddlRawFiles;
        private IComboBox ddlPlatform;
        private IComboBox ddlArchitecture;
        private ServiceContainer sc;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
            dcSvc = mr.Stub<IConfigurationService>();
            sc = new ServiceContainer();
            sc.AddService(typeof(IConfigurationService), dcSvc);
        }

        [Test]
        public void OaiLoad()
        {
            Given_Dialog();
            Given_RawFiles();
            Given_Platforms();
            Given_Architectures();
            Expect_RawFilesDatasourceSet();
            Expect_PlatformDataSourceSet();
            Expect_ArchDatasourceSet();
            mr.ReplayAll();

            var interactor = new OpenAsInteractor();
            interactor.Attach(dlg);
            dlg.Raise(x => x.Load += null,
                dlg,
                EventArgs.Empty);

            Assert.AreEqual(2, archNames.Count());
            Assert.AreEqual(2, platformNames.Length);
            Assert.AreEqual("(Unknown)", rawFileNames[0].Text);
            Assert.IsNull(rawFileNames[0].Value);
            Assert.AreEqual("(None)", platformNames[0].Text);
            Assert.AreEqual("0", dlg.AddressTextBox.Text);
            mr.VerifyAll();
        }

        private void Expect_RawFilesDatasourceSet()
        {
            ddlRawFiles= mr.Stub<IComboBox>();
            ddlRawFiles.Expect(d => d.DataSource = null)
                .IgnoreArguments()
                .WhenCalled(m => { this.rawFileNames = ((IEnumerable)m.Arguments[0]).OfType<ListOption>().ToArray(); });
            dlg.Expect(d => d.RawFileTypes).Return(ddlRawFiles);
            ddlRawFiles.Stub(d => d.TextChanged += null).IgnoreArguments();
        }

        private void Expect_ArchDatasourceSet()
        {
            ddlArchitecture = mr.StrictMock<IComboBox>();
            ddlArchitecture.Expect(d => d.DataSource = null)
                .IgnoreArguments()
                .WhenCalled(m => { this.archNames = ((IEnumerable) m.Arguments[0]).OfType<ListOption>().ToArray(); });
            dlg.Expect(d => d.Architectures).Return(ddlArchitecture);
        }

        private void Expect_PlatformDataSourceSet()
        {
            ddlPlatform = mr.StrictMock<IComboBox>();
            ddlPlatform.Expect(d => d.DataSource = null)
                .IgnoreArguments()
                .WhenCalled(m => { this.platformNames = ((IEnumerable) m.Arguments[0]).OfType<ListOption>().ToArray(); });
            dlg.Expect(d => d.Platforms).Return(ddlPlatform);
        }

        private void Given_Dialog()
        {
            dlg = mr.DynamicMock<IOpenAsDialog>();
            var btnBrowse = mr.Stub<IButton>();
            var btnOk = mr.Stub<IButton>();
            var txtAddress = mr.Stub<ITextBox>();
            var fileName = mr.Stub<ITextBox>();
            dlg.Stub(d => d.AddressTextBox).Return(txtAddress);
            dlg.Stub(d => d.Services).Return(sc);
            dlg.Stub(d => d.FileName).Return(fileName);
            dlg.Stub(d => d.BrowseButton).Return(btnBrowse);
            dlg.Stub(d => d.OkButton).Return(btnOk);
        }

        private void Given_Architectures()
        {
            var arch1 = mr.Stub<Architecture>();
            arch1.Stub(a => a.Name).Return("ARCH1");
            arch1.Stub(a => a.Description).Return("Arch 1");
            arch1.Stub(a => a.TypeName).Return("ArchSpace1.Arch");

            var arch2 = mr.Stub<Architecture>();
            arch2.Stub(a => a.Name).Return("ARCH2");
            arch2.Stub(a => a.Description).Return("Arch 2");
            arch2.Stub(a => a.TypeName).Return("ArchSpace2.Arch");

            dcSvc.Stub(d => d.GetArchitectures()).Return(new List<Architecture> { arch1, arch2 });
        }

        private void Given_Platforms()
        {
            var env1 = mr.Stub<OperatingEnvironment>();
            env1.Stub(e => e.Name).Return("TECH");
            env1.Stub(e => e.Description).Return("Friendly");
            env1.Stub(a => a.TypeName).Return("Env1.Env");

            dcSvc.Stub(d => d.GetEnvironments()).Return(new List<OperatingEnvironment> { env1 });
        }

        private void Given_RawFiles()
        {
            var raw1 = mr.Stub<RawFileElement>();
            raw1.Name = "RawFile1";
            raw1.Description = "First kind of raw file";

            dcSvc.Stub(d => d.GetRawFiles()).Return(new List<RawFileElement> { raw1 });
        }

        [Test]
        public void Oai_OkPressed_ReturnSelectedThings()
        {
            Given_Dialog();
            Given_RawFiles();
            Given_Platforms();
            Given_Architectures();
            Expect_RawFilesDatasourceSet();
            Expect_PlatformDataSourceSet();
            Expect_ArchDatasourceSet();
            mr.ReplayAll();

            var interactor = new OpenAsInteractor();
            interactor.Attach(dlg);

            When_Dialog_Loaded();

            mr.VerifyAll();
            Assert.AreEqual(2, archNames.Count());
            Assert.AreEqual(2, platformNames.Length);
            Assert.AreEqual("(None)", platformNames[0].Text);
        }

        private void When_Dialog_Loaded()
        {
            dlg.Raise(x => x.Load += null,
                dlg,
                EventArgs.Empty);
        }

        [Test]
        public void Oai_NoFileSelected_OkDisabled()
        {
            Given_Dialog();
            Given_RawFiles();
            Given_Platforms();
            Given_Architectures();
            Expect_RawFilesDatasourceSet();
            Expect_PlatformDataSourceSet();
            Expect_ArchDatasourceSet();
            mr.ReplayAll();

            var interactor = new OpenAsInteractor();
            interactor.Attach(dlg);

            When_Dialog_Loaded();

            Assert.IsFalse(dlg.OkButton.Enabled);
            mr.VerifyAll();
        }

        [Test]
        public void Oai_AddressSelected_OkDisabled()
        {
            Given_Dialog();
            Given_RawFiles();
            Given_Platforms();
            Given_Architectures();
            Expect_RawFilesDatasourceSet();
            Expect_PlatformDataSourceSet();
            Expect_ArchDatasourceSet();
            mr.ReplayAll();

            var interactor = new OpenAsInteractor();
            interactor.Attach(dlg);

            When_Dialog_Loaded();
            When_FileSelected("foo.exe");

            Assert.IsFalse(dlg.OkButton.Enabled);
            mr.VerifyAll();
        }

        private void When_FileSelected(string fileName)
        {
            dlg.FileName.Text = fileName;
            dlg.FileName.Raise(t => t.TextChanged += null, dlg.FileName, EventArgs.Empty);
        }
#endif
    }
}
