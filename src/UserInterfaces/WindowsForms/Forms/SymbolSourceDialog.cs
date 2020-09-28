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

using Reko.Gui.Controls;
using Reko.Gui.Forms;
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
    public partial class SymbolSourceDialog : Form, ISymbolSourceDialog
    {
        public SymbolSourceDialog()
        {
            InitializeComponent();

            AssemblyFile = new TextBoxWrapper(txtAssembly);
            BrowseAssemblyFile = new ButtonWrapper(btnPickAssembly);
            OkButton = new ButtonWrapper(btnOk);
            SymbolFileUrl = new TextBoxWrapper(txtSymbolFile);
            BrowseSymbolFile = new ButtonWrapper(btnSymbolFile);
            SymbolSourceClasses = new ListboxWrapper(listClasses);
            SymbolSourceList = new ListViewWrapper(listSources);
            CustomSourceCheckbox = new CheckBoxWrapper(chkCustomSource);

            new SymbolSourceInteractor().Attach(this);
        }

        public IServiceProvider Services { get; set; }

        public ITextBox AssemblyFile { get; private set; }
        public IButton BrowseAssemblyFile { get; private set; }
        public IButton BrowseSymbolFile { get; private set; }
        public IButton OkButton { get; private set; }
        public ITextBox SymbolFileUrl { get; private set; }
        public IListBox SymbolSourceClasses { get; private set; }
        public IListView SymbolSourceList { get; private set; }
        public ICheckBox CustomSourceCheckbox { get; private set; }
    }
}
