#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Collections;
using Reko.Core.Graphs;
using Reko.Gui;
using Reko.Scanning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms.Forms
{
    public class JumpTableInteractor
    {
        private JumpTableDialog dlg;

        public void Attach(JumpTableDialog dlg)
        {
            this.dlg = dlg;
            dlg.IsIndirectTable.CheckedChanged += IsIndirectTable_CheckedChanged;
            dlg.Load += Dlg_Load;
            dlg.FormClosing += Dlg_FormClosing;
            dlg.EntryCount.ValueChanged += EntryCount_ValueChanged;
            dlg.Entries.SelectedIndexChanged += Entries_SelectedIndexChanged;
            dlg.FarAddress.CheckedChanged += FarAddress_CheckedChanged;
            dlg.RelativeAddress.CheckedChanged += RelativeAddress_CheckedChanged;
            dlg.SegmentOffsets.CheckedChanged += Offsets_CheckedChanged;
        }

        private void EnableControls()
        {
            dlg.IndirectTable.Enabled = dlg.IsIndirectTable.Checked;
            dlg.IndirectLabel.Enabled = dlg.IsIndirectTable.Checked;
        }

        private void BuildAddressTable()
        {
            var vectorBuilder = new VectorBuilder(null, dlg.Program, null);
            var addresses = new List<Address>();
            if (dlg.Program.Platform.TryParseAddress(dlg.JumpTableStartAddress.Text, out Address addrTable))
            {
                var stride = TableStride();
                var state = dlg.Program.Architecture.CreateProcessorState();
                state.InstructionPointer = dlg.Instruction.Address;
                addresses = vectorBuilder.BuildTable(addrTable, stride * (int)dlg.EntryCount.Value, null, stride, state);
            }
            dlg.Entries.DataSource = addresses;
            dlg.Entries.SelectedIndex = addresses.Count - 1;
        }

        private void EnableSegmentedPanel(bool hasValue)
        {
            //foreach (Control control in dlg.SegmentedAddressPanel.Controls)
            //{
            //    control.Enabled = hasValue;
            //}
            if (hasValue)
            {
                dlg.SegmentList.DataSource = dlg.Program.SegmentMap.Segments.Values
                    .Select(seg => new ListOption(seg.Name, seg.Address))
                    .ToList();
            }
        }

        public UserIndirectJump GetResults()
        {
            var vb = new VectorBuilder(dlg.Services, dlg.Program, new DirectedGraphImpl<object>());
            var stride = 4; //$TODO: get from dialog
            var entries = vb.BuildTable(dlg.VectorAddress.Value, stride * (int)dlg.EntryCount.Value, null, stride, null);
            var table = new ImageMapVectorTable(dlg.VectorAddress.Value, entries.ToArray(), 0);
            return new UserIndirectJump
            {
                Address = dlg.Instruction.Address,
                Table = table,
                IndexRegister = dlg.Program.Architecture.GetRegister(dlg.IndexRegister.SelectedValue.ToString())
            };
        }

        private void SetRadioButtons()
        {
            if (dlg.Stride == dlg.Program.Platform.PointerType.Size)
            {
                dlg.FarAddress.Checked = true;
            }
            else if (dlg.Stride == dlg.Program.Platform.FramePointerType.Size)
            {
                dlg.SegmentOffsets.Checked = true;
            }
        }

        private int TableStride()
        {
            if (dlg.FarAddress.Checked || dlg.RelativeAddress.Checked)
            {
                return dlg.Program.Platform.PointerType.Size;
            }
            else
            {
                return 2;
            }
        }

        private void Dlg_Load(object sender, EventArgs e)
        {
            dlg.CaptionLabel.Text = $"Jump table for {dlg.Instruction.Address}";
            dlg.JumpInstructionAddress.Text = dlg.Instruction.Address.ToString();
            dlg.InstructionLabel.Text = dlg.Instruction.ToString().Replace('\t', ' ');
            if (dlg.VectorAddress is not null)
            {
                dlg.JumpTableStartAddress.Text = dlg.VectorAddress.ToString();
            }
            EnableSegmentedPanel(dlg.Program.SegmentMap.BaseAddress.Selector.HasValue);
            dlg.IndexRegister.DataSource = dlg.Program.Architecture.GetRegisters().ToList();
            dlg.SegmentList.DataSource = dlg.Program.SegmentMap.Segments.Values
                .Select(s => s.Name)
                .OrderBy(s => s)
                .ToList();
            SetRadioButtons();
            BuildAddressTable();
            EnableControls();
        }

        private void Dlg_FormClosing(object sender, FormClosingEventArgs e)
        {
            if ((Gui.Services.DialogResult)dlg.DialogResult != Gui.Services.DialogResult.OK)
                return;
            if (dlg.Program.Platform.TryParseAddress(dlg.JumpTableStartAddress.Text, out Address addr))
            {
                dlg.VectorAddress = addr;
            }
        }

        private void EntryCount_ValueChanged(object sender, EventArgs e)
        {
            BuildAddressTable();
        }

        private void Entries_SelectedIndexChanged(object sender, EventArgs e)
        {
            string text;
            if (dlg.Entries.SelectedItem is not null)
            {
                var addr = (Address) dlg.Entries.SelectedItem;
                var dasm = dlg.Program.CreateDisassembler(dlg.Program.Architecture, addr);
                text = string.Join(
                    Environment.NewLine,
                    dasm.TakeWhile(i => (i.InstructionClass & InstrClass.Transfer) == 0)
                        .Take(400)
                        .Select(i => i.ToString()));
            }
            else
            {
                text = "";
            }
            dlg.Disassembly.Text = text;
        }

        private void IsIndirectTable_CheckedChanged(object sender, EventArgs e)
        {
            BuildAddressTable();
            EnableControls();
        }

        private void FarAddress_CheckedChanged(object sender, EventArgs e)
        {
            BuildAddressTable();
            EnableControls();
        }
        private void RelativeAddress_CheckedChanged(object sender, EventArgs e)
        {
            BuildAddressTable();
            EnableControls();
        }

        private void Offsets_CheckedChanged(object sender, EventArgs e)
        {
            BuildAddressTable();
            EnableControls();
        }
    }
}
