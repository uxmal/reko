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

using Reko.Core.Configuration;
using Reko.Gui;
using Reko.Gui.Controls;
using Reko.Gui.Forms;
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
    public partial class AssembleFileDialog : Form, IAssembleFileDialog
    {
        public AssembleFileDialog()
        {
            InitializeComponent();
            FileName = new TextBoxWrapper(txtFileName);
            AssemblerList = ddlAssembler;
            BrowseButton = new ButtonWrapper(btnBrowse);
            new AssembleFileInteractor().Attach(this);
        }

        public IServiceProvider Services { get; set; }
        public ITextBox FileName {get; private set;}
        public ComboBox AssemblerList { get; private set; }
        public IButton BrowseButton { get; private set; }

        public string SelectedArchitectureTypeName
        {
            get { return ((AssemblerDefinition)((ListOption)ddlAssembler.SelectedValue).Value).TypeName; }
        }
    }
}
