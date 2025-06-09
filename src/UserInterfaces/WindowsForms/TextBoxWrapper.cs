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
using System.Drawing;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms
{
    /// <summary>
    /// Wraps the Windows Forms textbox with the ITextBox interface.
    /// </summary>
    public class TextBoxWrapper : ControlWrapper, ITextBox
    {
        private TextBoxBase text;

        public event EventHandler<Gui.Controls.KeyEventArgs> KeyDown;
        public event EventHandler<Gui.Controls.KeyEventArgs> KeyUp;

        public TextBoxWrapper(TextBoxBase text)
            : base(text)
        {
            this.text = text;
            this.text.KeyDown += Text_KeyDown;
            this.text.KeyUp += Text_KeyUp;
        }

        public string Text { get { return text.Text; } set { text.Text = value;  } }

        public bool Modified
        {
            get => text.Modified;
            set => text.Modified = value;
        }

        public void SelectAll() {
            text.SelectAll();
        }

        public void ScrollToEnd()
        {
            text.SelectionStart = text.TextLength;
            text.ScrollToCaret();
        }

        public event EventHandler TextChanged
        {
            add { text.TextChanged += value; }
            remove { text.TextChanged -= value; }
        }

        private void Text_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            var eh = KeyDown;
            if (eh is not null)
            {
                var ee = new Gui.Controls.KeyEventArgs((Gui.Controls.Keys)e.KeyData);
                eh(sender, ee);
                e.SuppressKeyPress = ee.SuppressKeyPress;
                e.Handled = ee.Handled;
            }
        }

        private void Text_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            var eh = KeyUp;
            if (eh is not null)
            {
                var ee = new Gui.Controls.KeyEventArgs((Gui.Controls.Keys)e.KeyData);
                eh(sender, ee);
                e.SuppressKeyPress = ee.SuppressKeyPress;
                e.Handled = ee.Handled;
            }
        }
    }
}
