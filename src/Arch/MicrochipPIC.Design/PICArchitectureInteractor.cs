#region License
/* 
 * Copyright (C) 2017-2020 Christian Hostelet.
 * inspired by work from:
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

using Reko.Libraries.Microchip;
using System;
using System.Linq;
using System.Windows.Forms;

namespace Reko.Arch.MicrochipPIC.Design
{
    using Common;

    /// <summary>
    /// Interactor used by <see cref="PICArchitecturePicker"/> form.
    /// </summary>
    public class PICArchitectureInteractor
    {
        private PICArchitecturePicker form;
        private PICArchitectureOptionsPicker result;

        internal void Attach(PICArchitecturePicker pickerForm)
        {
            this.form = pickerForm;
            form.Load += Form_Load;
            form.FormClosing += Form_FormClosing;
        }

        private void Form_Load(object sender, EventArgs e)
        {
            result = form.Value ?? new PICArchitectureOptionsPicker() { PICName = String.Empty, AllowExtended = false };
            InitValues();
            form.ModelComboBox.SelectedIndexChanged += ModelComboBox_SelectedIndexChanged;
            form.ExtendedModeCheckBox.CheckedChanged += ExtendedModeCheckbox_CheckedChanged;
            form.PIC16RadioButton.CheckedChanged += PICRadioButtons_CheckedChanged;
            form.PIC18RadioButton.CheckedChanged += PICRadioButtons_CheckedChanged;
        }

        private void Form_FormClosing(object sender, EventArgs e)
        {
            if (form.DialogResult == DialogResult.OK)
            {
                form.Value = result;
            }
        }

        private void ExtendedModeCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            result.AllowExtended = form.ExtendedModeCheckBox.Checked;
        }

        private void ModelComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cmb = sender as ComboBox;
            result.PICName = cmb.Text;
            form.OKButton.Enabled = form.ModelComboBox.SelectedIndex >= 0;
        }

        private void PICRadioButtons_CheckedChanged(object sender, EventArgs e)
        {
            string family;
            if (form.PIC16RadioButton.Checked)
            {
                form.ExtendedModeCheckBox.Enabled = false;
                result.AllowExtended = false;
                family = "PIC16";
            }
            else
            {
                form.ExtendedModeCheckBox.Enabled = true;
                result.AllowExtended = form.ExtendedModeCheckBox.Checked;
                family = "PIC18";
            }
            form.ExtendedModeCheckBox.Checked = result.AllowExtended;
            form.ModelComboBox.BeginUpdate();
            form.ModelComboBox.Items.Clear();
            form.ModelComboBox.Items.AddRange(PICCrownking.GetDB().EnumPICList(s => s.StartsWith(family)).ToArray());
            form.ModelComboBox.Text = null;
            form.ModelComboBox.EndUpdate();
            result.PICName = String.Empty;
        }

        private void InitValues()
        {
            string family;
            if (result.PICName.Length < 5)
            {
                result.PICName = String.Empty;
                result.AllowExtended = false;
                family = "PIC18";
            }
            else
            {
                family = result.PICName.Substring(0, 5);
            }

            switch (family)
            {
                case "PIC16":
                    form.PIC16RadioButton.Checked = true;
                    form.PIC18RadioButton.Checked = false;
                    form.ExtendedModeCheckBox.Checked = false;
                    form.ExtendedModeCheckBox.Enabled = false;
                    break;
                case "PIC18":
                    form.PIC16RadioButton.Checked = false;
                    form.PIC18RadioButton.Checked = true;
                    form.ExtendedModeCheckBox.Checked = result.AllowExtended;
                    form.ExtendedModeCheckBox.Enabled = true;
                    break;
            }
            form.ModelComboBox.BeginUpdate();
            form.ModelComboBox.Items.AddRange(PICCrownking.GetDB().EnumPICList(s => s.StartsWith(family)).ToArray());
            int picindex = -1;
            try
            {
                picindex = form.ModelComboBox.FindStringExact(result.PICName);
            }
            catch
            {
            }
            if (picindex != -1)
            {
                form.ModelComboBox.Text = result.PICName;
                form.ModelComboBox.SelectedIndex = picindex;
            }
            else
            {
                form.ModelComboBox.Text = string.Empty;
            }
            form.ModelComboBox.EndUpdate();
        }

    }

}
