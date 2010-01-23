/* 
 * Copyright (C) 1999-2010 John Källén.
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

using Decompiler.Gui;
using Decompiler.Gui.Windows;
using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Windows.Forms;

namespace Decompiler.UnitTests.Gui.Windows
{
    [TestFixture]
    public class SearchResultServiceTests
    {
        private Form form;
        private ListView listSearchResults;
        private SearchResultServiceImpl svc;

        [SetUp]
        public void Setup()
        {
            form = new Form();
            listSearchResults = new ListView();
            form.Controls.Add(listSearchResults);
            svc = new SearchResultServiceImpl(listSearchResults);
        }

        [TearDown]
        public void TearDown()
        {
            form.Dispose();
        }

        [Test]
        public void Creation()
        {
            Assert.IsTrue(listSearchResults.VirtualMode);
            Assert.AreEqual(View.Details, listSearchResults.View);
        }

        [Test]
        public void ShowSingleItem()
        {
            form.Show();
            var result = new TestSearchResult();
            result.AddItem(new TestSearchResultItem());

            svc.ShowSearchResults(result);
            Assert.AreEqual(1, listSearchResults.Items.Count);
            Assert.AreEqual(1, listSearchResults.VirtualListSize);
        }

        [Test]
        public void CreateColumns()
        {
            form.Show();
            var result = new TestSearchResult();
            svc.ShowSearchResults(result);
            Assert.AreEqual(2, listSearchResults.Columns.Count);
            Assert.AreEqual("col1", listSearchResults.Columns[0].Text);
            Assert.AreEqual("col2", listSearchResults.Columns[1].Text);
        }

        private class TestSearchResult : SearchResult
        {
            private List<TestSearchResultItem> items = new List<TestSearchResultItem>();

            public void AddItem(TestSearchResultItem item)
            {
                items.Add(item);
            }

            public override int Count
            {
                get { return items.Count; }
            }

            public override SearchResultItem this[int i]
            {
                get { return items[i]; }
            }

            public override IEnumerable<SearchResultColumn> GetColumns()
            {
                var cols = new SearchResultColumn[] {
                    new SearchResultColumn("col1", 4),
                    new SearchResultColumn("col2", 8)
                };
                return cols;
            }
        }

        private class TestSearchResultItem : SearchResultItem
        {
        }

    }
}
