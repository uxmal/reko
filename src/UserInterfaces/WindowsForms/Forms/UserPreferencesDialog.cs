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
using Reko.UserInterfaces.WindowsForms.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms.Forms
{
    public partial class UserPreferencesDialog : Form, IUserPreferencesDialog
    {
        public UserPreferencesDialog()
        {
            InitializeComponent();

            new UserPreferencesInteractor().Attach(this);
        }

        public IServiceProvider Services { get; set; }

        public TreeView WindowTree     { get { return treeView1; } }
        public Button WindowFontButton { get { return btnWindowFont; } }
        public Button WindowFgButton   { get { return btnWindowFgColor; } }
        public Button WindowBgButton   { get { return btnWindowBgColor; } }
        public Button ResetButton      { get { return btnReset; } }

        public ListBox ImagebarList    { get { return lbxUiElements; } }
        public Button ImagebarFgButton { get { return btnElementFgColor; } }
        public Button ImagebarBgButton { get { return btnElementBgColor; } }

        public TextView CodeControl { get { return codeCtl; } }
        public MemoryControl MemoryControl { get { return memCtl; } }
        public DisassemblyControl DisassemblyControl { get { return dasmCtl; } }
        public TreeView Browser {  get { return treeBrowser; } }
        public ListView List { get { return listView; } }

        public ColorDialog ColorPicker { get { return colorPicker; } }
        public FontDialog FontPicker { get { return fontPicker; } }
    }
}
