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
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;

namespace Reko.UserInterfaces.WindowsForms
{
    public class ListViewWrapper : ControlWrapper, IListView
    {
        private ListView list;

        public ListViewWrapper(ListView list) : base(list)
        {
            this.list = list;
        }


        public object DataSource
        {
            get { throw new NotImplementedException();  }
            set
            {
                list.Items.Clear();
                foreach (string[] strs in (IEnumerable)value)
                {
                    var item = new ListViewItem(strs);
                    list.Items.Add(item);
                } 
            }
        }

        public ICollection SelectedItems { get { return list.SelectedItems; } }
        public T GetTagOf<T>(object item) { return (T)((ListViewItem)item).Tag; }
    }
}
