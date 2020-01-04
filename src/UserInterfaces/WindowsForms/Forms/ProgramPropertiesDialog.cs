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
using Reko.Gui.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms.Forms
{
    public partial class ProgramPropertiesDialog : Form, IProgramPropertiesDialog
    {
        public ProgramPropertiesDialog() : this(new ProgramPropertiesInteractor())
        {

        }

        public ProgramPropertiesDialog(ProgramPropertiesInteractor interactor)
        {
            InitializeComponent();
            interactor.Attach(this);
        }

        public IServiceProvider Services { get; set; }

        public Program Program { get; set; }

        public TextBox LoadScript { get { return txtScript; } }
        public CheckBox EnableScript { get { return chkRunScript; } }
        public ListBox Heuristics { get { return listHeuristics; } }
        public Button OkButton { get { return btnOk; } }
    }
}
