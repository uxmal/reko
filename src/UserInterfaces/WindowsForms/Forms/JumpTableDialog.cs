#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using Reko.Core.Machine;
using Reko.Gui.Controls;
using Reko.Gui.Forms;
using System;
using System.Windows.Forms;

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
            this.JumpInstructionAddress = new TextBoxWrapper(txtJumpAddress);
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
        public Address? VectorAddress { get; set; }
        public Program Program { get; set; }
        public IProcessorArchitecture Architecture { get; set; }
        public int Stride { get; set; }

        public ILabel CaptionLabel { get; }
        public ITextBox JumpInstructionAddress { get;}
        public IComboBox IndexRegister { get; }
        public ILabel InstructionLabel { get; }
        public ITextBox JumpTableStartAddress { get; }
        public INumericUpDown EntryCount { get; }
        public ICheckBox IsIndirectTable { get; }
        public ILabel IndirectLabel { get; }
        public ITextBox IndirectTable { get; }
        public IRadioButton FarAddress { get; }
        public IRadioButton RelativeAddress { get; }
        public IRadioButton SegmentOffsets { get; }
        public IComboBox SegmentList { get; }
        public IListBox Entries { get; }
        public ITextBox Disassembly { get; }

        public UserIndirectJump GetResults()
        {
            return interactor.GetResults();
        }

    }
}
