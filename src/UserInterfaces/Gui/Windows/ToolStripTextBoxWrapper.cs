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

using Reko.Gui.Controls;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Reko.Gui.Windows
{
    public class ToolStripTextBoxWrapper : ITextBox
    {
        private ToolStripTextBox textbox;

        public ToolStripTextBoxWrapper(ToolStripTextBox textbox)
        {
            this.textbox = textbox;
        }

        public bool Enabled { get { return textbox.Enabled; } set { textbox.Enabled = value; } }
        public string Text { get { return textbox.Text; } set { textbox.Text = value; } }
        public Color BackColor { get { return textbox.BackColor; } set { textbox.BackColor = value; } }
        public Color ForeColor { get { return textbox.ForeColor; } set { textbox.ForeColor = value; } }

        public void SelectAll()
        {
            textbox.SelectAll();
        }

        public void Focus()
        {
            textbox.Focus();
        }

        public event KeyEventHandler KeyDown
        {
            add { textbox.KeyDown += value; }
            remove { textbox.KeyDown -= value; }
        }

        public event EventHandler TextChanged
        {
            add { textbox.TextChanged += value; }
            remove { textbox.TextChanged -= value; }
        }

        public event EventHandler LostFocus
        {
            add { textbox.LostFocus += value; }
            remove { textbox.LostFocus -= value; }
        }
    }
}
