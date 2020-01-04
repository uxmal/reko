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
using Reko.Core;
using Reko.Core.Machine;
using Reko.Gui.Controls;

namespace Reko.UserInterfaces.WindowsForms.Forms
{
    public partial class JumpTableDialog : Form, IJumpTableDialog
    {
        private JumpTableInteractor interactor;

        public JumpTableDialog()
        {
            InitializeComponent();
            this.interactor = new JumpTableInteractor();

            this.CaptionLabel = new LabelWrapper(lblCaption);
            this.InstructionLabel = new LabelWrapper(lblInstruction);
            this.JumpTableStartAddress = new TextBoxWrapper(txtStartAddress);
            this.EntryCount = new NumericUpDownWrapper(numEntries);
            this.IndexRegister = new ComboBoxWrapper(ddlRegisters);

            this.IsIndirectTable = new CheckBoxWrapper(chkIndirectTable);
            this.IndirectLabel = new LabelWrapper(lblIndirectTable);
            this.IndirectTable = new TextBoxWrapper(txtIndirectTable);

            this.FarAddress = new RadioButtonWrapper(rdbFarAddresses);
            this.RelativeAddress = new RadioButtonWrapper(rdbRelativeOffsets);
            this.SegmentOffsets = new RadioButtonWrapper(rdbOffsets);
            this.SegmentList = new ComboBoxWrapper(ddlSegments);

            this.Entries = new ListboxWrapper(listEntries);
            this.Disassembly = new TextBoxWrapper(txtDasm);

            interactor.Attach(this);
        }

        public IServiceProvider Services { get; set; }
        public MachineInstruction Instruction { get; set; }
        public Address VectorAddress { get;  set; }
        public Program Program { get; set; }
        public int Stride { get; set; }

        public ILabel CaptionLabel { get; private set; }
        public IComboBox IndexRegister { get; private set; }
        public ILabel InstructionLabel { get; private set; }
        public ITextBox JumpTableStartAddress { get; private set; }
        public INumericUpDown EntryCount { get; private set; }
        public ICheckBox IsIndirectTable { get; private set; }
        public ILabel IndirectLabel { get; private set; }
        public ITextBox IndirectTable { get; private set; }
        public IRadioButton FarAddress { get; private set; }
        public IRadioButton RelativeAddress { get; private set; }
        public IRadioButton SegmentOffsets { get; private set; }
        public IComboBox SegmentList { get; private set; }

        public IListBox Entries { get; private set; }
        public ITextBox Disassembly { get; private set; }

        public UserIndirectJump GetResults()
        {
            return interactor.GetResults();
        }

    }
}
