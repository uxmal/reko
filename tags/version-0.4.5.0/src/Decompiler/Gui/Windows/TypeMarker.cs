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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows
{
    public class TypeMarker : IDisposable
    {
        private TextBox text;
        private Label label;

        public event EventHandler<TypeMarkerEventArgs> TextChanged;
        public event EventHandler<TypeMarkerEventArgs> TextAccepted;

        private const string TypeMarkerEnterType = "<Enter type>";

        public TypeMarker(Control bgControl)
        {
            Debug.Print(bgControl.GetType().FullName);
            text = new TextBox
            {
                BorderStyle = BorderStyle.FixedSingle,
                Parent = bgControl,
                Visible = false,
            };
            label = new Label
            {
                ForeColor = SystemColors.GrayText,
                BackColor = SystemColors.Info,
                BorderStyle = BorderStyle.FixedSingle,
                AutoSize = true,
                Parent = bgControl,
                Text = TypeMarkerEnterType,
                Visible = false,
            };

            text.LostFocus += text_LostFocus;
            text.TextChanged += text_TextChanged;
            text.KeyDown += text_KeyDown;
        }

        void text_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
            case Keys.Escape:
                HideControls();
                e.SuppressKeyPress = true;
                e.Handled = true;
                break;
            case Keys.Enter:
                if (TextAccepted != null)
                    TextAccepted(this, new TypeMarkerEventArgs(text.Text));
                HideControls();
                e.SuppressKeyPress = true;
                e.Handled = true;
                return;
            }
        }

        public void Show(Point location)
        {
            text.Location = location;
            label.Location = new Point(location.X, location.Y + text.Height + 3);
            label.BringToFront();
            text.Visible = true;
            label.Visible = true;
            text.BringToFront();
            text.Focus();
        }

        public void HideControls()
        {
            text.Visible = false;
            label.Visible = false;
        }

        public string FormatText(string text)
        {
            return text;
        }

        void text_TextChanged(object sender, EventArgs e)
        {
            if (TextChanged != null)
            {
                var ee = new TypeMarkerEventArgs(text.Text);
                TextChanged(this, ee);
                var formattedText = ee.FormattedType;
                if (formattedText.Length > 0)
                {
                    label.ForeColor = SystemColors.ControlText;
                    label.Text = ee.FormattedType;
                }
                else
                {
                    label.ForeColor = SystemColors.GrayText;
                    label.Text = TypeMarkerEnterType; 
                }
            }
        }

        void text_LostFocus(object sender, EventArgs e)
        {
            text.Visible = false;
            label.Visible = false;
        }

        public void Dispose()
        {
            if (text != null) text.Dispose();
            if (label != null) label.Dispose();
            text = null;
            label = null;
        }
    }

    public class TypeMarkerEventArgs : EventArgs
    {
        public TypeMarkerEventArgs(string userText) { UserText = userText; }
        public string UserText { get; private set; }
        public string FormattedType { get; set; }
    }
}
