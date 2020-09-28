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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms
{
    public class ComboBoxWrapper : ControlWrapper, IComboBox
    {
        private ComboBox ddl;

        public event EventHandler TextChanged 
        {
            add { this.ddl.TextChanged += value; } 
            remove { this.ddl.TextChanged -= value; }
        }
        
        public event EventHandler SelectedIndexChanged
        {
            add { this.ddl.SelectedIndexChanged += value; }
            remove { this.ddl.SelectedIndexChanged -= value; }
        }

        public ComboBoxWrapper(ComboBox ddl) : base(ddl) { this.ddl = ddl; }

        public object DataSource { get { return ddl.DataSource; } set { ddl.DataSource = value; } }
        public IList Items { get { return ddl.Items; } }
        public int SelectedIndex { get { return ddl.SelectedIndex; } set { ddl.SelectedIndex = value; } }
        public object SelectedItem { get { return ddl.SelectedItem; } set { ddl.SelectedItem = value; } }
        public object SelectedValue { get { return ddl.SelectedValue; } set { ddl.SelectedValue = value; } }
        public string Text { get { return ddl.Text; } set { ddl.Text = value; } }
    }
}
