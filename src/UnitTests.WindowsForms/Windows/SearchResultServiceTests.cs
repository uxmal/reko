#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Services;
using Reko.Gui;
using Reko.UserInterfaces.WindowsForms;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace Reko.UnitTests.Gui.Windows
{
    [TestFixture]
    [Category(Categories.UserInterface)]
    public class SearchResultServiceTests
    {
        private Form form;
        private ListView listSearchResults;
        private SearchResultServiceImpl svc;
        private Mock<IWindowFrame> frame;
        private ServiceContainer sc;

        [SetUp]
        public void Setup()
        {
            frame = new Mock<IWindowFrame>();
            sc = new ServiceContainer();
            sc.AddService(typeof(IWindowFrame), frame.Object);
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
            if (form is not null)
                form.Dispose();
        }

        [Test]
        public void SRS_Creation()
        {
            CreateUI();

            Assert.IsTrue(listSearchResults.VirtualMode);
            Assert.AreEqual(View.Details, listSearchResults.View);
        }

        [Test]
        public void SRS_ShowSingleItem()
        {
            var result = new Mock<ISearchResult>();
            result.Setup(s => s.ContextMenuID).Returns(0).Verifiable();
            result.Setup(s => s.Count).Returns(1).Verifiable();
            result.Setup(s => s.CreateColumns()).Verifiable();
            result.Setup(s => s.Count).Returns(1).Verifiable();
            result.Setup(s => s.GetItem(0)).Returns(new SearchResultItem { Items = new[] { "foo", "bar" }, ImageIndex = -1 }).Verifiable();
            result.Setup(s => s.Count).Returns(1).Verifiable();
            result.Setup(s => s.GetItem(0)).Returns(new SearchResultItem { Items = new[] { "foo", "bar" }, ImageIndex = -1 }).Verifiable();
            result.Setup(s => s.Count).Returns(1).Verifiable();
            result.Setup(s => s.GetItem(0)).Returns(new SearchResultItem { Items = new[] { "foo", "bar" }, ImageIndex = -1 }).Verifiable();

            CreateUI();
            form.Show();
            svc.ShowSearchResults(result.Object);

            Assert.AreEqual(1, listSearchResults.Items.Count);
            Assert.AreEqual(1, listSearchResults.VirtualListSize);
            Assert.AreEqual(2, listSearchResults.Items[0].SubItems.Count);
            Assert.AreEqual("foo", listSearchResults.Items[0].SubItems[0].Text);
            Assert.AreEqual("bar", listSearchResults.Items[0].SubItems[1].Text);

            result.VerifyAll();
        }

        [Test]
        public void SRS_CreateColumns()
        {
            var result = new Mock<ISearchResult>();
            //result.Expect(s => s.View = Arg<ISearchResultView>.Is.NotNull);
            result.Setup(s => s.ContextMenuID).Returns(0).Verifiable();
            result.Setup(s => s.Count).Returns(0).Verifiable();
            result.Setup(s => s.CreateColumns()).Verifiable();

            CreateUI();
            form.Show();
            svc.ShowSearchResults(result.Object);

            result.VerifyAll();
        }


        [Test]
        public void DoubleClickShouldNavigate()
        {
            var result = new Mock<ISearchResult>();
            result.Setup(s => s.NavigateTo(1)).Verifiable();

            CreateUI();
            form.Show();
            svc.ShowSearchResults(result.Object);
            svc.DoubleClickItem(1);

            result.VerifyAll();
        }

        [Test]
        public void SRS_ShowResults_ChangesContextMenu()
        {
            var result = new Mock<ISearchResult>();
            var uiSvc = new Mock<IDecompilerShellUiService>();
            result.Setup(r => r.ContextMenuID).Returns(42);
            uiSvc.Setup(u => u.SetContextMenu(
                It.IsNotNull<object>(),
                42))
                .Verifiable();
            sc.AddService(typeof(IDecompilerShellUiService), uiSvc.Object);

            CreateUI();
            form.Show();
            svc.ShowSearchResults(result.Object);

            result.VerifyAll();
            uiSvc.VerifyAll();
        }
    }
}
