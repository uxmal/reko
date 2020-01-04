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
using Reko.Scanning;
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
    public partial class SearchDialog : Form, ISearchDialog
    {
        public SearchDialog()
        {
            InitializeComponent();
            Patterns = new ComboBoxWrapper(ddlPatterns);
            RegexCheckbox = new CheckBoxWrapper(chkRegexp);
            Encodings = new ComboBoxWrapper(ddlEncoding);
            Scopes = new ComboBoxWrapper(ddlScope);
            SearchButton = new ButtonWrapper(btnSearch);
            StartAddress = new TextBoxWrapper(txtStartAddress);
            EndAddress = new TextBoxWrapper(txtEndAddress);
            ScannedMemory = new CheckBoxWrapper(chkScanned);
            UnscannedMemory = new CheckBoxWrapper(chkUnscanned);
            new SearchDialogInteractor().Attach(this);
        }

        public IServiceProvider Services { get; set; }
        public string InitialPattern { get; set; }

        public IComboBox Patterns { get; private set; }
        public ICheckBox RegexCheckbox { get; private set; }
        public IComboBox Encodings { get; private set; }
        public IComboBox Scopes { get; private set; }
        public ITextBox StartAddress { get; private set; }
        public ITextBox EndAddress { get; private set; }
        public IButton SearchButton { get; private set; }
        public ICheckBox ScannedMemory { get; private set; }
        public ICheckBox UnscannedMemory { get; private set;}

        public StringSearch<byte> ImageSearcher { get; set; }
    }
}
