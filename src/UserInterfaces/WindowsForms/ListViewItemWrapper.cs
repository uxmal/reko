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

using Reko.Gui;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms
{
    /// <summary>
    /// Windows Forms implementation of the IListViewItem interface.
    /// </summary>
    public class ListViewItemWrapper : IListViewItem
    {
        private ListViewItem item;

        public ListViewItemWrapper(ListViewItem item)
        {
            this.item = item;
        }

        public ListViewItem Item { get { return item; } }

        #region IListViewItem Members

        public string Text { get { return item.Text; } set { item.Text = value; } }
        public object Tag { get { return item.Tag; } set { item.Tag = value; } }

        public void AddSubItem(string text)
        {
            item.SubItems.Add(text);
        }

        #endregion

        private class ListViewSubItem : IListViewSubItem
        {
            public ListViewSubItem(ListViewItem.ListViewSubItem subItem)
            {
                this.SubItem = subItem;
            }

            public ListViewItem.ListViewSubItem SubItem { get; private set; }
            public string Text { get { return SubItem.Text; } set { SubItem.Text = value; } }
        }

        private class SubItemList : IList<IListViewSubItem>
        {
            private ListViewItem.ListViewSubItemCollection collection;

            public SubItemList(ListViewItem.ListViewSubItemCollection listViewSubItemCollection)
            {
                // TODO: Complete member initialization
                this.collection = listViewSubItemCollection;
            }

            public int IndexOf(IListViewSubItem item)
            {
                throw new NotSupportedException();
            }

            public void Insert(int index, IListViewSubItem item)
            {
                throw new NotSupportedException();
            }

            public void RemoveAt(int index)
            {
                collection.RemoveAt(index);
            }

            public IListViewSubItem this[int index] { get { return new ListViewSubItem(collection[index]); } set { collection[index] = ((ListViewSubItem) value).SubItem; } } 

            public void Add(string text)
            {
                collection.Add(text);
            }

            public void Add(IListViewSubItem item)
            {
                throw new NotSupportedException();
            }

            public void Clear()
            {
                collection.Clear();
            }

            public bool Contains(IListViewSubItem item)
            {
                throw new NotImplementedException();
            }

            public void CopyTo(IListViewSubItem[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public int Count { get { return collection.Count; } }
            public bool IsReadOnly { get { return collection.IsReadOnly; } }

            public bool Remove(IListViewSubItem item)
            {
                collection.Remove(((ListViewSubItem)item).SubItem);
                return true;
            }

            public IEnumerator<IListViewSubItem> GetEnumerator()
            {
                foreach (ListViewItem.ListViewSubItem item in collection)
                {
                    yield return new ListViewSubItem(item);
                }
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}
