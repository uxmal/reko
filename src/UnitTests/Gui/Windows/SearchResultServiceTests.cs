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

using Decompiler.Gui;
using Decompiler.Gui.Windows;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.UnitTests.Gui.Windows
{
    [TestFixture]
    public class SearchResultServiceTests
    {
        private MockRepository repository;
        private Form form;
        private ListView listSearchResults;
        private SearchResultServiceImpl svc;
        private IWindowFrame frame;
        private ServiceContainer sc;

        [SetUp]
        public void Setup()
        {
            repository = new MockRepository();
            frame = repository.DynamicMock<IWindowFrame>();
            sc = new ServiceContainer();
        }

        private void CreateUI()
        {
            form = new Form();
            listSearchResults = new ListView();
            form.Controls.Add(listSearchResults);
            svc = new SearchResultServiceImpl(sc, frame, listSearchResults);
        }

        [TearDown]
        public void TearDown()
        {
            form.Dispose();
        }

        [Test]
        public void SRS_Creation()
        {
            repository.ReplayAll();

            CreateUI();

            Assert.IsTrue(listSearchResults.VirtualMode);
            Assert.AreEqual(View.Details, listSearchResults.View);
        }

        [Test]
        public void SRS_ShowSingleItem()
        {
            var result = repository.StrictMock<ISearchResult>();
            result.Expect(s => s.ContextMenuID).Return(0);
            result.Expect(s => s.Count).Return(1);
            result.Expect(s => s.CreateColumns(
                Arg<ISearchResultView>.Is.NotNull));
            result.Expect(s => s.Count).Return(1);
            result.Expect(s => s.GetItemStrings(0)).Return(new string[] { "foo", "bar" });
            result.Expect(s => s.GetItemImageIndex(0)).Return(-1);
            result.Expect(s => s.Count).Return(1);
            result.Expect(s => s.GetItemStrings(0)).Return(new string[] { "foo", "bar" });
            result.Expect(s => s.GetItemImageIndex(0)).Return(-1);
            result.Expect(s => s.Count).Return(1);
            result.Expect(s => s.GetItemStrings(0)).Return(new string[] { "foo", "bar" });
            result.Expect(s => s.GetItemImageIndex(0)).Return(-1);
            repository.ReplayAll();

            CreateUI();
            form.Show();
            svc.ShowSearchResults(result);

            Assert.AreEqual(1, listSearchResults.Items.Count);
            Assert.AreEqual(1, listSearchResults.VirtualListSize);
            Assert.AreEqual(2, listSearchResults.Items[0].SubItems.Count);
            Assert.AreEqual("foo", listSearchResults.Items[0].SubItems[0].Text);
            Assert.AreEqual("bar", listSearchResults.Items[0].SubItems[1].Text);

            repository.VerifyAll();
        }

        [Test]
        public void SRS_CreateColumns()
        {
            var result = repository.StrictMock<ISearchResult>();
            result.Expect(s => s.ContextMenuID).Return(0);
            result.Expect(s => s.Count).Return(0);
            result.Expect(s => s.CreateColumns(
                Arg<ISearchResultView>.Is.NotNull));
            repository.ReplayAll();

            CreateUI();
            form.Show();
            svc.ShowSearchResults(result);

            repository.VerifyAll();
        }


        [Test]
        public void DoubleClickShouldNavigate()
        {
            var result = repository.DynamicMock<ISearchResult>();
            result.Expect(s => s.NavigateTo(1));
            repository.ReplayAll();

            CreateUI();
            form.Show();
            svc.ShowSearchResults(result);
            svc.DoubleClickItem(1);

            repository.VerifyAll();
        }

        [Test]
        public void SRS_ShowResults_ChangesContextMenu()
        {
            var ctxMenu = new ContextMenu();
            var result = repository.DynamicMock<ISearchResult>();
            var uiSvc = repository.DynamicMock<IDecompilerShellUiService>();
            result.Expect(r => r.ContextMenuID).Return(42);
            uiSvc.Expect(u => u.GetContextMenu(42)).Return(ctxMenu);
            sc.AddService(typeof(IDecompilerShellUiService), uiSvc);
            repository.ReplayAll();

            CreateUI();
            form.Show(); svc.ShowSearchResults(result);

            repository.VerifyAll();
            Assert.AreEqual(ctxMenu, listSearchResults.ContextMenu);
        }
    }
}
