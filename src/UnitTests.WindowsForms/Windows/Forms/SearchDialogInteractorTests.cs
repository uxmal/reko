#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

using Moq;
using NUnit.Framework;
using Reko.Gui;
using Reko.Gui.Forms;
using Reko.UserInterfaces.WindowsForms.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Windows.Forms;

namespace Reko.UnitTests.Gui.Windows.Forms
{
    [TestFixture]
    [Category(Categories.UserInterface)]
    public class SearchDialogInteractorTests
    {
        private Mock<ISettingsService> setSvc;
        private ISearchDialog dlg;
        private ServiceContainer sc;

        [SetUp]
        public void Setup()
        {
            sc = new ServiceContainer();

            setSvc = new Mock<ISettingsService>();
            sc.AddService(typeof(ISettingsService), setSvc.Object);
        }

        [TearDown]
        public void TearDown()
        {
            if (dlg is not null)
                dlg.Dispose();
            dlg = null;
        }

        [Test]
        public void SrchDlg_LoadDialogWithSettings()
        {
            Given_UiSettings("SearchDialog/Patterns", new[] {"foo", "bar"});
            Given_UiSettings("SearchDialog/Encoding", 0, 1);
            Given_UiSettings("SearchDialog/Scope", 0, 0);
            Given_UiSettings("SearchDialog/Regexp", 0, 1);
            Given_UiSettings("SearchDialog/Scanned", 1, 0);
            Given_UiSettings("SearchDialog/Unscanned", 1, 0);

            When_CreateDialog();
            When_ShowDialog();

            Then_PatternListHasChoices("foo", "bar");
            Assert.AreEqual(1, dlg.Encodings.SelectedIndex);
            Assert.AreEqual(0, dlg.Scopes.SelectedIndex);
            Assert.IsTrue(dlg.RegexCheckbox.Checked);
            Assert.IsFalse(dlg.StartAddress.Enabled);
            Assert.IsFalse(dlg.EndAddress.Enabled);
            Assert.IsFalse(dlg.ScannedMemory.Checked);
            Assert.IsFalse(dlg.UnscannedMemory.Checked);
            setSvc.VerifyAll();
        }

        private void Given_UiSettings(string settingName, string[] settings)
        {
            setSvc.Setup(s => s.GetList(settingName))
                .Returns(settings)
                .Verifiable();
        }

        private void Given_UiSettings(string settingName, object defaultValue, int value)
        {
            setSvc.Setup(s => s.Get(settingName, defaultValue))
                .Returns(value)
                .Verifiable();
        }

        private void Then_PatternListHasChoices(params string [] expected)
        {
            var actual = dlg.Patterns.Items.Cast<string>().ToArray();
            Assert.AreEqual(expected.Length, actual.Length);
            for (int i = 0; i < expected.Length; ++i)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }
        }

        [Test]
        public void SrchDlg_ClosingDialogShouldAddMruSetting()
        {
            Given_SettingsService();
            Given_UiSettings("SearchDialog/Patterns", new[] { "foo" });
            Given_UiSettings("SearchDialog/Regexp", 0, 0);
            Given_UiSettings("SearchDialog/Encoding", 0, 0);
            Given_UiSettings("SearchDialog/Scope", 0, 0);
            Expect_UiSettingsSet("SearchDialog/Patterns", new[] { "bar", "foo" });

            When_CreateDialog();
            When_ShowDialog();
            When_EnterPattern("bar");
            When_SearchButtonPushed();

            setSvc.VerifyAll();
        }

        [Test]
        public void SrchDlg_Foo()
        {
            var arr = new System.Collections.BitArray(60);
            arr[14] = true;
            int i = 0;
            foreach (bool x in arr)
            {
                if (x)
                System.Diagnostics.Debug.Print("X: {0}", i);
                ++i;
            }
        }

        [Test]
        public void SrchDlg_SearchForBinary()
        {
            Given_SettingsService();
            Given_UiSettings("SearchDialog/Patterns", null);
            Given_UiSettings("SearchDialog/Regexp", 0, 0);
            Given_UiSettings("SearchDialog/Encoding", 0, 0);
            Given_UiSettings("SearchDialog/Scope", 0, 0);
            Expect_UiSettingsSet("SearchDialog/Patterns", new[] { "00 11 22 33" });

            When_CreateDialog();
            When_ShowDialog();
            When_EnterPattern("00 11 22 33");
            When_SearchButtonPushed();

            Then_DialogHasBinarySearcher(00, 0x11, 0x22, 0x33);
        }

        [Test]
        public void SrchDlg_Hexize()
        {
            var bytes = SearchDialogInteractor.Hexize("FA").ToArray();
            Assert.AreEqual(0xFA, bytes[0]);
        }

        [Test]
        public void SrchDlg_Octize()
        {
            var bytes = SearchDialogInteractor.Octize("010 032 377").ToArray();
            Assert.AreEqual(0x08, bytes[0]);
            Assert.AreEqual(0x1A, bytes[1]);
            Assert.AreEqual(0xFF, bytes[2]);
        }

        private void Then_DialogHasBinarySearcher(params byte[] expectedBytes)
        {
            Assert.IsNotNull(dlg.ImageSearcher);
            Assert.AreEqual(expectedBytes, dlg.ImageSearcher.Pattern);
        }

        private void Expect_UiSettingsSet(string name, string[] expected)
        {
            setSvc.Setup(s => s.SetList(
                name,
                It.IsNotNull<IEnumerable<string>>()))
                    .Callback((string n, IEnumerable<string> s) =>
                    {
                        var actual = s.ToArray();
                        Assert.AreEqual(expected.Length, actual.Length);
                        for (int i = 0; i < actual.Length; ++i)
                        {
                            Assert.AreEqual(expected[i], actual[i]);
                        }
                    }).Verifiable();
        }

        private void When_SearchButtonPushed()
        {
            dlg.SearchButton.PerformClick();
        }

        private void When_EnterPattern(string pattern)
        {
            dlg.Patterns.Text = pattern;
        }

        private void When_ShowDialog()
        {
            ((Form) dlg).Show();
        }

        private void When_CreateDialog()
        {
            dlg = new SearchDialog()
            {
                Services = sc
            };
        }

        private void Given_SettingsService()
        {

        }

        private void Given_UiService()
        {
            throw new NotImplementedException();
        }

        private void Then_DialogHasPatternsChoices()
        {
            throw new NotImplementedException();
        }
    }
}
