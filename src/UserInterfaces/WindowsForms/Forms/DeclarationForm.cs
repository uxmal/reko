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

using System.Drawing;
using System.Windows.Forms;
using Reko.Gui.Controls;
using Reko.Gui.Forms;

namespace Reko.UserInterfaces.WindowsForms.Forms
{
    public partial class DeclarationForm : Form, IDeclarationForm
    {
        private ITextBox textWrapped;

        public DeclarationForm()
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
            text.Width = label.Width;
            this.Show();
            this.Location = new Point(location.X, location.Y - this.Height);
        }
    }
}
