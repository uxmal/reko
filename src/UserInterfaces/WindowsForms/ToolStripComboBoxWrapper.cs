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

using Reko.Gui.Controls;
using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

#nullable enable

namespace Reko.UserInterfaces.WindowsForms
{
    internal class ToolStripComboBoxWrapper : IComboBox
    {
        private ToolStripComboBox ddl;

        public ToolStripComboBoxWrapper(ToolStripComboBox ddl)
        {
            this.ddl = ddl;
        }

        public object? DataSource { get; set; }

        public IList Items => ddl.Items;

        public int SelectedIndex
        {
            get { return ddl.SelectedIndex; } 
            set { ddl.SelectedIndex = value; }
        }

        public object? SelectedItem
        {
            get { return ddl.SelectedItem; }
            set { ddl.SelectedItem= value; }
        }

        public object SelectedValue
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public string Text
        {
            get { return ddl.Text; }
            set { ddl.Text = value; }
        }
        
        public Color ForeColor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Color BackColor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool Enabled
        {
            get { return ddl.Enabled; }
            set { ddl.Enabled = value; }
        }

        public void BringToFront()
        {   
            // No-op.
        }

        public void Focus()
        {
            ddl.Focus();
        }

        public event EventHandler SelectedIndexChanged
        {
            add { ddl.SelectedIndexChanged += value; }
            remove { ddl.SelectedIndexChanged -= value; }
        }

        public event EventHandler? TextChanged
        {
            add { ddl.TextChanged += value; }
            remove { ddl.TextChanged -= value; }
        }

        public event EventHandler? GotFocus
        {
            add { ddl.GotFocus += value; }
            remove { ddl.GotFocus -= value; }
        }

        public event EventHandler? LostFocus
        {
            add { ddl.LostFocus += value; }
            remove { ddl.LostFocus -= value; }
        }

    }
}
