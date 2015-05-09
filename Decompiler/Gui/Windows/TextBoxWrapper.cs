#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Gui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows
{
    /// <summary>
    /// Wraps the Windows Forms textbox with the ITextBox interface.
    /// </summary>
    public class TextBoxWrapper : ITextBox
    {
        private TextBox text;

        public TextBoxWrapper(TextBox text)
        {
            this.text = text;
        }

        public bool Enabled { get { return text.Enabled; } set { text.Enabled = value; } }
        public string Text { get { return text.Text; } set { text.Text = value;  } }

        public void SelectAll() {
            text.SelectAll();
        }

        public void Focus()
        {
            text.Focus();
        }

        public event KeyEventHandler KeyDown
        {
            add { text.KeyDown += value; }
            remove { text.KeyDown -= value; }
        }

        public event EventHandler TextChanged
        {
            add { text.TextChanged += value; }
            remove { text.TextChanged -= value; }
        }
    }
}
