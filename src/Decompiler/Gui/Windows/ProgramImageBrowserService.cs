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
using System;
using System.Collections;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows
{
    /// <summary>
    /// Windows Forms implementation of IProgramImageBrowserService.
    /// </summary>
    public class ProgramImageBrowserService : IProgramImageBrowserService
    {
        private ListView list;
        public ProgramImageBrowserService(ListView list)
        {
            this.list = list;
            list.SelectedIndexChanged += new EventHandler(OnBrowserListItemSelected);
        }

        #region IProgramImageBrowserService Members

        public event EventHandler SelectionChanged;

        public bool Enabled
        {
            get { return list.Enabled; }
            set { list.Enabled = value; }
        }

        public object FocusedItem
        {
            get { return list.FocusedItem.Tag; }
        }

        public bool IsItemSelected
        {
            get { return list.SelectedItems.Count > 0; }
        }

        public void Populate(IEnumerable items, ListViewItemDecoratorHandler handler)
        {
            list.Items.Clear();
            ListViewItemWrapper wli = new ListViewItemWrapper();
			foreach (object item in items)
			{
                wli.Item = new ListViewItem();
                wli.Item.Tag = item;
                handler(item, wli);
				list.Items.Add(wli.Item);
			}
        }

        public object SelectedItem
        {
            get
            {
                return list.SelectedItems.Count > 0
                  ? list.SelectedItems[0].Tag
                  : null;
            }
        }

        #endregion

        // Event handlers ///////////////

        public void OnBrowserListItemSelected(object sender, EventArgs e)
        {
            if (SelectionChanged != null)
                SelectionChanged(this, e);
        }

    }
}
