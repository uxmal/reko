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

using System.Windows.Forms;

namespace Reko.Arch.MicrochipPIC.Design
{
    using Common;

    /// <summary>
    /// Form to ask the user for PIC model and PIC Execution mode.
    /// </summary>
    public partial class PICArchitecturePicker : Form
    {
        public PICArchitecturePicker()
        {
            InitializeComponent();
            new PICArchitectureInteractor().Attach(this);
        }

        public PICArchitectureOptionsPicker Value { get; set; }

        public CheckBox ExtendedModeCheckBox => chkExtendedMode;
        public RadioButton PIC16RadioButton => rdbPIC16;
        public RadioButton PIC18RadioButton => rdbPIC18;
        public ComboBox ModelComboBox => cbbModel;
        public Button OKButton => btnOK;

    }
}
