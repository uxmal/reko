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

using Reko.Gui.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Reko.UiPrototype.WinForms
{
    public partial class PatternSearchDialog : Form, IPatternSearchDialog
    {
        public PatternSearchDialog()
        {
            InitializeComponent();

            comboBox1.Items.AddRange(new object[] {
                new Wrapper("Hex-encoded binary data"),
                new Wrapper(Encoding.ASCII),
                new Wrapper(Encoding.GetEncoding("ISO_8859-1")),
                new Wrapper(Encoding.UTF8),
                new Wrapper(Encoding.GetEncoding("UTF-16LE")),
                new Wrapper(Encoding.GetEncoding("UTF-16BE"))
            });
            comboBox1.SelectedIndex = 0;
        }

        public StringSearcher CreateStringSearcher()
        {
            return new StringSearcher(textBox1.Text);
        }

        private class Wrapper
        {
            private string name;
            private Encoding e;

            public Wrapper(string name)
            {
                this.name = name;
            }

            public Wrapper(Encoding e)
            {
                this.e = e;
                this.name = e.WebName.ToUpper() + "-encoded string";
            }

            public override string ToString()
            {
                return name;
            }
        }
    }
}
