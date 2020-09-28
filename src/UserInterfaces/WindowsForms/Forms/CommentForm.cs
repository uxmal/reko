#region License
/* 
 * Copyright (C) 1999-2020 Pavel Tomin.
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
using Reko.Gui.Forms;
using Reko.UserInterfaces.WindowsForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms.Forms
{
    public partial class CommentForm : Form, IDeclarationForm
    {
        private ITextBox textWrapped;

        public CommentForm()
        {
            InitializeComponent();
            textWrapped = new TextBoxWrapper(text);
        }

        public string HintText
        {
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
            this.Show();
            this.Location = new Point(location.X, location.Y - this.Height);
        }
    }
}
