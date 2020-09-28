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
using Reko.UserInterfaces.WindowsForms;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms.Forms
{
    public partial class KeyBindingsDialog : Form, IKeyBindingsDialog
    {
        public KeyBindingsDialog()
        {
            InitializeComponent();
            Commands = new ListboxWrapper(listCommands);
            CommandName = new TextBoxWrapper(txtCommandName);
            Windows = new ComboBoxWrapper(ddlWindows);
            CommandKeys = new ComboBoxWrapper(ddlCommandShortcuts);
            Shortcut = new TextBoxWrapper(txtShortCut);

            new KeyBindingsInteractor().Attach(this);
        }

        public Dictionary<string, Dictionary<int, CommandID>> KeyBindings { get; set; }

        public IListBox Commands { get; }
        public ITextBox CommandName { get; }

        public IComboBox CommandKeys { get; }

        public IComboBox Windows { get; }

        public ITextBox Shortcut { get; }

        public string RenderKey(int iKey)
        {
            return ((System.Windows.Forms.Keys)iKey).ToString();
        }
    }
}
