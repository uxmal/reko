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

using Reko.Core;
using Reko.Gui.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms.Controls
{
    /// <summary>
    /// Provides a unified view of Memory and Disassembly.
    /// </summary>
    public partial class LowLevelView : UserControl, INavigableControl<Address>
    {
        ITextBox txtAddressWrapped;
        IButton btnGoWrapped;
        IButton btnBackWrapped;
        IButton btnFwdWrapped;
        IComboBox ddlVisualizerWrapped;
        Address addrCurrent;

        public LowLevelView()
        {
            InitializeComponent();
            txtAddressWrapped = new ToolStripTextBoxWrapper(txtAddress);
            btnBackWrapped = new ToolStripButtonWrapper(btnBack);
            btnFwdWrapped = new ToolStripButtonWrapper(btnForward);
            btnGoWrapped = new ToolStripButtonWrapper(btnGo);
            ddlVisualizerWrapped = new ComboBoxWrapper(ddlVisualizer);
        }

        public IButton ToolbarBackButton { get { return btnBackWrapped; } }
        public IButton ToolbarForwardButton { get { return btnFwdWrapped; } }
        public ITextBox ToolBarAddressTextbox { get { return txtAddressWrapped; } }
        public IButton ToolBarGoButton { get { return btnGoWrapped; } }
        public ImageMapView ImageMapView { get { return imageMapControl1; } }
        public MemoryControl MemoryView { get { return this.memCtrl; } }
        public DisassemblyControl DisassemblyView { get { return this.dasmCtrl; } }
        public VisualizerControl VisualizerControl { get { return this.visualizerControl; } }
        public IComboBox VisualizerList { get { return ddlVisualizerWrapped; } }
        IButton INavigableControl<Address>.BackButton { get { return btnBackWrapped; } }
        IButton INavigableControl<Address>.ForwardButton { get { return btnFwdWrapped; } }
        public Address CurrentAddress { get { return addrCurrent; } set { addrCurrent = value; CurrentAddressChanged.Fire(this); } }

        public event EventHandler CurrentAddressChanged;
    }
}
