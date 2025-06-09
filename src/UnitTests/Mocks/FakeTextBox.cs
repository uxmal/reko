#region License
/* 
 * Copyright (C) 1999-2025 Pavel Tomin.
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

using Reko.Core;
using Reko.Gui.Controls;
using System;
using System.Drawing;

namespace Reko.UnitTests.Mocks
{
    public class FakeTextBox : ITextBox
    {
        private string text;

        public bool Enabled { get; set; }
        public string Text
        {
            get { return text is null ? "" : text; }
            set { text = value; TextChanged?.Invoke(this, EventArgs.Empty); }
        }
        public Color BackColor { get; set; }
        public Color ForeColor { get; set; }

        public bool Modified { get; set; }

        public void BringToFront()
        {
            throw new NotImplementedException();
        }

        public void SelectAll()
        {
            throw new NotImplementedException();
        }

        public void ScrollToEnd()
        {
            throw new NotImplementedException();
        }

        public void Focus()
        {
            throw new NotImplementedException();
        }

        public event EventHandler<KeyEventArgs> KeyDown;
        public event EventHandler<KeyEventArgs> KeyUp;
        public event EventHandler TextChanged;
        public event EventHandler LostFocus;
        public event EventHandler GotFocus;

        public void FireLostFocus()
        {
            LostFocus?.Invoke(this, EventArgs.Empty);
        }
        public void FireGotFocus()
        {
            GotFocus?.Invoke(this, EventArgs.Empty);
        }

        public void FireKeyDown(KeyEventArgs e)
        {
            KeyDown(this, e);
        }

        public void FireKeUp(KeyEventArgs e)
        {
            KeyUp(this, e);
        }
    }
}
