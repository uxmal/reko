#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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

using NUnit.Framework;
using Reko.Gui;
using Reko.UserInterfaces.WindowsForms;
using Rhino.Mocks;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace Reko.UnitTests.Gui.Windows
{
    [TestFixture]
    [Category(Categories.UserInterface)]
    public class SearchResultServiceTests
    {
        private MockRepository mr;
        private Form form;
        private ListView listSearchResults;
        private SearchResultServiceImpl svc;
        private IWindowFrame frame;
        private ServiceContainer sc;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
            frame = mr.DynamicMock<IWindowFrame>();
            sc = new ServiceContainer();
            sc.AddService(typeof(IWindowFrame), frame);
        }

        private void CreateUI()
        {
            form = new Form();
            listSearchResults = new ListView();
            form.Controls.Add(listSearchResults);
            svc = new SearchResultServiceImpl(sc, listSearchResults);
        }

        [TearDown]
        public void TearDown()
        {
            if (form != null)
                form.Dispose();
        }

        [Test]
        public void SRS_Creation()
        {
            mr.ReplayAll();

            CreateUI();

            Assert.IsTrue(listSearchResults.VirtualMode);
            Assert.AreEqual(View.Details, listSearchResults.View);
        }

        [Test]
        public void SRS_ShowSingleItem()
        {
            var result = mr.StrictMock<ISearchResult>();
            result.Expect(s => s.ContextMenuID).Return(0);
            result.Expect(s => s.Count).Return(1);
            result.Expect(s => s.View = Arg<ISearchResultView>.Is.NotNull);
            result.Expect(s => s.CreateColumns());
            result.Expect(s => s.Count).Return(1);
            result.Expect(s => s.GetItem(0)).Return(new SearchResultItem { Items = new[] { "foo", "bar" }, ImageIndex = -1 });
            result.Expect(s => s.Count).Return(1);
            result.Expect(s => s.GetItem(0)).Return(new SearchResultItem { Items = new[] { "foo", "bar" }, ImageIndex = -1 });
            result.Expect(s => s.Count).Return(1);
            result.Expect(s => s.GetItem(0)).Return(new SearchResultItem { Items = new[] { "foo", "bar" }, ImageIndex = -1 });
            mr.ReplayAll();

            CreateUI();
            form.Show();
            svc.ShowSearchResults(result);

            Assert.AreEqual(1, listSearchResults.Items.Count);
            Assert.AreEqual(1, listSearchResults.VirtualListSize);
            Assert.AreEqual(2, listSearchResults.Items[0].SubItems.Count);
            Assert.AreEqual("foo", listSearchResults.Items[0].SubItems[0].Text);
            Assert.AreEqual("bar", listSearchResults.Items[0].SubItems[1].Text);

            mr.VerifyAll();
        }

        [Test]
        public void SRS_CreateColumns()
        {
            var result = mr.StrictMock<ISearchResult>();
            result.Expect(s => s.View = Arg<ISearchResultView>.Is.NotNull);
            result.Expect(s => s.ContextMenuID).Return(0);
            result.Expect(s => s.Count).Return(0);
            result.Expect(s => s.CreateColumns());
            mr.ReplayAll();

            CreateUI();
            form.Show();
            svc.ShowSearchResults(result);

            mr.VerifyAll();
        }


        [Test]
        public void DoubleClickShouldNavigate()
        {
            var result = mr.DynamicMock<ISearchResult>();
            result.Expect(s => s.NavigateTo(1));
            mr.ReplayAll();

            CreateUI();
            form.Show();
            svc.ShowSearchResults(result);
            svc.DoubleClickItem(1);

            mr.VerifyAll();
        }

        [Test]
        public void SRS_ShowResults_ChangesContextMenu()
        {
            var result = mr.DynamicMock<ISearchResult>();
            var uiSvc = mr.DynamicMock<IDecompilerShellUiService>();
            result.Expect(r => r.ContextMenuID).Return(42);
            uiSvc.Expect(u => u.SetContextMenu(
                Arg<object>.Is.NotNull,
                Arg<int>.Is.Equal(42)));
            sc.AddService(typeof(IDecompilerShellUiService), uiSvc);
            mr.ReplayAll();

            CreateUI();
            form.Show();
            svc.ShowSearchResults(result);

            mr.VerifyAll();
        }
    }
}
