﻿#region License
/* 
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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Reko.Core;
using Reko.Core.Types;
using Reko.Scanning;

namespace Reko.Gui.Windows.Forms
{
    public partial class FindStringsDialog : Form, IFindStringsDialog
    {
        private FindStringsDialogInteractor interactor;

        public FindStringsDialog()
        {
            InitializeComponent();
            this.interactor = new FindStringsDialogInteractor();
            this.interactor.Attach(this);
        }

        public int MinLength { get { return Convert.ToInt32(this.numericUpDown1.Value); } set { } }

        public ComboBox CharacterSizeList { get { return ddlCharSize; } }

        public ComboBox StringKindList { get { return ddlStringKind; } }

        public StringFinderCriteria GetCriteria()
        {
            return interactor.GetCriteria();
        }
    }
}
