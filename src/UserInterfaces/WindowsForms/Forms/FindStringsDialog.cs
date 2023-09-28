#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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

using Reko.Core.Types;
using Reko.Gui;
using Reko.Scanning;
using System;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms.Forms
{
    public partial class FindStringsDialog : Form, IDialog<StringFinderCriteria>
    {
        private FindStringsDialogInteractor interactor;

        public FindStringsDialog()
        {
            InitializeComponent();
        }

        public FindStringsDialogInteractor DataContext
        {
            get => interactor;
            set
            {
                this.interactor = value;
                interactor.Attach(this);
            }
        }

        public int MinLength => Convert.ToInt32(this.numericUpDown1.Value);

        public ComboBox CharacterSizeList => ddlCharSize;

        public ComboBox StringKindList => ddlStringKind;

        public StringFinderCriteria Value {
            get {
                return (DialogResult == DialogResult.Cancel)
                    ? null : interactor.GetCriteria();
            }
        }

        public ComboBox SearchAreas => ddlSearchArea;

        public Button SearchAreaButton => btnSearchArea;
    }
}
