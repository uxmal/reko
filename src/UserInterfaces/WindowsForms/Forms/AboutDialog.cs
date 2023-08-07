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

using Reko.Gui;
using Reko.Gui.Forms;
using Reko.Gui.ViewModels.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms.Forms
{
    partial class AboutDialog : Form, IAboutDialog
    {
        private readonly AboutViewModel viewModel;

        public AboutDialog()
        {
            InitializeComponent();
            this.viewModel = new AboutViewModel();
            this.Text = viewModel.Caption;
            this.lblVersion.Text = viewModel.Version;
            lblGitHash.Text = viewModel.GitHash;
            linkLabel1.LinkClicked += linkLabel_LinkClicked;
        }

        private void linkLabel_LinkClicked(object o, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    UseShellExecute = true,
                    FileName = linkLabel1.Text
                });
            }
            catch (Exception)
            {
            }
        }


        private void AboutDialog_Load(object sender, EventArgs e)
        {

        }
    }
}
