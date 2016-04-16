#region License
/* 
 * Copyright (C) 1999-2016 Pavel Tomin.
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
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using Reko.Gui.Controls;
using Reko.Gui.Forms;

namespace Reko.Gui.Windows.Forms
{
    public class DeclarationForm : IDeclarationForm
    {
        private TextBox text;
        private Label label;

        private ITextBox textWrapped;

        public DeclarationForm()
        {
            text = new TextBox
            {
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false,
            };
            label = new Label
            {
                ForeColor = SystemColors.ControlText,
                BackColor = SystemColors.Info,
                BorderStyle = BorderStyle.FixedSingle,
                AutoSize = true,
                Visible = false,
            };
            textWrapped = new TextBoxWrapper(text);
        }

        // $TODO remove
        public void SetBgControl(Control bgControl)
        {
            text.Parent = bgControl;
            label.Parent = bgControl;
        }

        public string HintText {
            get
            {
                return label.Text;
            }
            set
            {
                label.Text = value;
            }
        }

        public virtual ITextBox TextBox { get { return textWrapped; } }


        public void ShowAt(Point location)
        {
            label.Location = new Point(location.X, location.Y - text.Height - label.Height);
            text.Location = new Point(location.X, location.Y - text.Height);
            text.Width = label.Width;
            label.BringToFront();
            text.Visible = true;
            label.Visible = true;
            text.BringToFront();
            text.Focus();
        }

        public void Hide()
        {
            label.Visible = false;
            text.Visible = false;
        }

        public void Dispose()
        {
            if (text != null) text.Dispose();
            if (label != null) label.Dispose();
            text = null;
            label = null;
        }
    }
}
