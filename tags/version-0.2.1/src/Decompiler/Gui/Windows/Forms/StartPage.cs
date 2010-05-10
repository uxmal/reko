/* 
 * Copyright (C) 1999-2010 John Källén.
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

using Decompiler.Gui;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows.Forms
{
    public partial class StartPage : UserControl, IStartPage
    {
        public StartPage()
        {
            InitializeComponent();
        }

        #region IStartPage Members

        public TextBox AssemblerFile
        {
            get { return txtAssembler; }
        }

        public Button BrowseInputFile
        {
            get { return btnBrowseInputFile;  }
        }

        public TextBox HeaderFile
        {
            get { return txtTypes; }
        }

        public TextBox InputFile
        {
            get { return txtInputFile; }
        }

        public TextBox IntermediateFile
        {
            get { return txtIL; }
        }

        public event EventHandler IsDirtyChanged;

        public bool IsDirty
        {
            get
            {
                return false;
            }
            set
            {
            }
        }

        public TextBox LoadAddress
        {
            get { return txtLoadAddress; }
        }

        public TextBox SourceFile
        {
            get { return txtSource; }
        }

        #endregion

    }
}
