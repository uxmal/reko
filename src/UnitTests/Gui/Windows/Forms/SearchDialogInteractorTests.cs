#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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

using NUnit.Framework;
using Reko.Gui;
using Reko.Gui.Forms;
using Reko.UserInterfaces.WindowsForms.Forms;
using Rhino.Mocks;
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
        private MockRepository mr;
        private ISettingsService setSvc;
        private ISearchDialog dlg;
        private ServiceContainer sc;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
            sc = new ServiceContainer();

            setSvc = mr.DynamicMock<ISettingsService>();
            sc.AddService(typeof(ISettingsService), setSvc);
        }

        [TearDown]
        public void TearDown()
        {
            if (dlg != null)
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
            mr.ReplayAll();

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
            mr.VerifyAll();
        }

        private void Given_UiSettings(string settingName, string[] settings)
        {
            setSvc.Expect(s => s.GetList(settingName)).Return(settings);
        }

        private void Given_UiSettings(string settingName, object defaultValue, int value)
        {
            setSvc.Expect(s => s.Get(settingName, defaultValue)).Return(value);
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
            mr.ReplayAll();

            When_CreateDialog();
            When_ShowDialog();
            When_EnterPattern("bar");
            When_SearchButtonPushed();

            mr.VerifyAll();
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
            mr.ReplayAll();

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
        private void Then_DialogHasBinarySearcher(params byte[] expectedBytes)
        {
            Assert.IsNotNull(dlg.ImageSearcher);
            Assert.AreEqual(expectedBytes, dlg.ImageSearcher.Pattern);
        }

        private void Expect_UiSettingsSet(string name, string[] expected)
        {
            setSvc.Expect(s => s.SetList(
                Arg<string>.Is.Equal(name),
                Arg<string[]>.Is.NotNull)).Do(
                    new Action<string, IEnumerable<string>>((n, s) =>
                    {
                        var actual = s.ToArray();;
                        Assert.AreEqual(expected.Length, actual.Length);
                        for (int i = 0; i < actual.Length; ++i)
                        {
                            Assert.AreEqual(expected[i], actual[i]);
                        }
                    }));
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
