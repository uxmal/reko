#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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

using Decompiler.Gui.Controls;
using Decompiler.Gui.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows.Controls
{
    /// <summary>
    /// Provides a unified view of Memory and Disassembly.
    /// </summary>
    public partial class LowLevelView : UserControl
    {
        ITextBox txtAddressWrapped;
        IButton btnGoWrapped;

        public LowLevelView()
        {
            InitializeComponent();
            txtAddressWrapped = new ToolStripTextBoxWrapper(txtAddress);
            btnGoWrapped = new ToolStripButtonWrapper(btnGo);
        }

        public ITextBox ToolBarAddressTextbox { get { return txtAddressWrapped; } }
        public IButton ToolBarGoButton { get { return btnGoWrapped; } } 
        public MemoryControl MemoryView { get { return this.memCtrl; } }
        public DisassemblyControl DisassemblyView { get { return this.dasmCtrl; } }
    }
}
