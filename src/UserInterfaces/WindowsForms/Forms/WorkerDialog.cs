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

using Reko.Gui;
using Reko.Gui.Controls;
using Reko.Gui.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms.Forms
{
    public partial class WorkerDialog : Form, IWorkerDialog
    {
        public WorkerDialog()
        {
            InitializeComponent();
            CancellationButton = new ButtonWrapper(btnCancel);
            Caption = new LabelWrapper(lblCaption);
            Detail = new LabelWrapper(lblDetailText);
            ProgressBar = new ProgressbarWrapper(progressBar);
        }

        public IButton CancellationButton { get; private set; }

        public BackgroundWorker Worker
        {
            get { return backgroundWorker; }
        }

        public IProgressBar ProgressBar { get; private set; }

        public ILabel Caption { get; private set; }

        public ILabel Detail { get; private set; }

        public void Invoke(Action action)
        {
            base.Invoke(action);
        }

        Gui.DialogResult IWorkerDialog.DialogResult
        {
            get { return (Gui.DialogResult)(int)this.DialogResult; }
            set { this.DialogResult = (System.Windows.Forms.DialogResult)(int)value; }
        }
    }
}
