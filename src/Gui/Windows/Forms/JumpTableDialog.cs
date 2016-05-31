#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using Reko.Core;
using Reko.Core.Machine;

namespace Reko.Gui.Windows.Forms
{
    public partial class JumpTableDialog : Form, IJumpTableDialog
    {
        private JumpTableInteractor interactor;

        public JumpTableDialog()
        {
            InitializeComponent();
            this.interactor = new JumpTableInteractor();
            interactor.Attach(this);
        }

        public IServiceProvider Services { get; set; }
        public MachineInstruction IndirectJump { get; set; }
        public Address VectorAddress { get;  set; }
        public Program Program { get; set; }
        
        public Label CaptionLabel { get { return lblCaption; } }
        public ComboBox IndexRegister { get { return ddlRegisters; } }
        public Label IndirectJumpLabel { get { return lblInstruction; } }
        public TextBox JumpTableStartAddress { get { return txtStartAddress; } }
        public NumericUpDown EntryCount { get { return numEntries; } }
        public CheckBox IsIndirectTable { get { return chkIndirectTable; } }
        public TextBox IndirectTable { get { return txtIndirectTable; } }
        public Panel SegmentedAddressPanel { get { return panelSegmentedAddresses; } }
        public RadioButton FarAddress { get { return rdbFarAddresses; } }
        public RadioButton Offsets { get { return rdbOffsets; } }
        public ComboBox SegmentList { get { return ddlSegments; } }

        public UserIndirectJump GetResults()
        {
            return interactor.GetResults();
        }
    }
}
