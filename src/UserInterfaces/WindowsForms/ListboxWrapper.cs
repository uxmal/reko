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

using Reko.Gui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Drawing;

namespace Reko.UserInterfaces.WindowsForms
{
    public class ListboxWrapper : ControlWrapper, IListBox
    {
        private ListBox lbx;

        public ListboxWrapper(ListBox lbx) : base(lbx)
        {
            this.lbx = lbx;
        }

        public event EventHandler SelectedIndexChanged
        {
            add { lbx.SelectedIndexChanged += value; }
            remove { lbx.SelectedIndexChanged -= value; }
        }

        public object DataSource { get { return lbx.DataSource; } set { lbx.DataSource = value; } }
        public IList Items { get { return lbx.Items; } }
        public ICollection SelectedItems { get { return lbx.SelectedItems; } }
        public int SelectedIndex { get { return lbx.SelectedIndex; } set { lbx.SelectedIndex = value; } }
        public object SelectedItem { get { return lbx.SelectedItem; } set { lbx.SelectedItem = value; } }

        public void AddItems(IEnumerable items)
        {
            foreach (var item in items)
            {
                lbx.Items.Add(item);
            }
        }
    }
}
