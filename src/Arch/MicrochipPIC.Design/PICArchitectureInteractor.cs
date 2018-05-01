#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet.
 * inspired by work from:
 * Copyright (C) 1999-2018 John Källén.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Gui;
using System.Windows.Forms;
using Reko.Arch.Microchip.Common;
using Reko.Libraries.Microchip;

namespace Reko.Arch.MicrochipPIC.Design
{
    public class PICPropertiesInteractor : IWindowPane
    {
        private PICArchitecture arch;

        public PICPropertiesInteractor(PICArchitecture arch)
        {
            this.arch = arch;
        }

        public PICArchitecturePanel Control { get; private set; }
        public IServiceProvider Services { get; private set; }
        public IWindowFrame Frame { get; set; }
        public void SetSite(IServiceProvider sp)
        {
            this.Services = sp;
        }

        public void Close()
        {
            if (Control != null)
            {
                Control.Dispose();
                Control = null;
            }
        }

        /// <summary>
        /// Creates the user interface control and connects all event handlers.
        /// </summary>
        /// <returns></returns>
        public Control CreateControl()
        {
            Control = new PICArchitecturePanel();
            var picname = arch.Options?.Mode?.PICName ?? "";
            if (picname.StartsWith("PIC16", StringComparison.InvariantCultureIgnoreCase))
            {
                Control.PIC16RadioButton.Checked = true;
                Control.PIC18RadioButton.Checked = false;
            }
            else
            {
                Control.PIC16RadioButton.Checked = false;
                Control.PIC18RadioButton.Checked = true;
            }
            SetPIC(picname);
            Control.ModelComboBox.Text = picname;
            Control.PIC16RadioButton.CheckedChanged += PIC16RadioButton_CheckedChanged;
            Control.PIC18RadioButton.CheckedChanged += PIC18RadioButton_CheckedChanged;
            Control.ExtendedModeCheckBox.CheckedChanged += ExtendedModeCheckbox_CheckedChanged;
            Control.ModelComboBox.SelectedIndexChanged += ModelComboBox_SelectedIndexChanged;

            return Control;
        }

        private void ExtendedModeCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (arch.Options == null)
                arch.Options = new PICOptions();
            arch.Options.ExecMode = Control.ExtendedModeCheckBox.Checked ? PICExecMode.Extended : PICExecMode.Traditional;
        }

        private void PIC16RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            Control.ModelComboBox.Text = null;
            SetPIC(Control.PIC16RadioButton.Checked ? "PIC16" : "PIC18");
        }

        private void PIC18RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            Control.ModelComboBox.Text = null;
            SetPIC(Control.PIC18RadioButton.Checked ? "PIC18" : "PIC16");
        }

        private void ModelComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cmb = sender as ComboBox;
            string picName = (string)cmb.SelectedItem;
            if (!String.IsNullOrWhiteSpace(picName))
            {
                if (arch.Options == null)
                    arch.Options = new PICOptions();
                arch.Options.Mode = PICProcessorMode.GetMode(picName);
                arch.Options.ExecMode = Control.ExtendedModeCheckBox.Checked ? PICExecMode.Extended : PICExecMode.Traditional;
            }
        }

        private void SetPIC(string picName)
        {
            var family = picName.Substring(0, 5).ToUpperInvariant();

            switch (family)
            {
                case "PIC16":
                    Control.ExtendedModeCheckBox.Enabled = false;
                    Control.ExtendedModeCheckBox.Checked = false;
                    break;

                case "PIC18":
                default:
                    family = "PIC18";
                    Control.ExtendedModeCheckBox.Enabled = true;
                    Control.ExtendedModeCheckBox.Checked = ((arch.Options?.ExecMode ?? PICExecMode.Traditional) == PICExecMode.Extended);
                    break;
            }
            Control.ModelComboBox.BeginUpdate();
            Control.ModelComboBox.Items.Clear();
            Control.ModelComboBox.Items.AddRange(PICCrownking.GetDB().EnumPICList(s => s.StartsWith(family)).ToArray());
            Control.ModelComboBox.EndUpdate();
        }

    }

}
