#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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

using Decompiler.Gui.Windows;
using Decompiler.Gui;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.WindowsItp
{
    public partial class ItpForm : Form
    {
        public ItpForm()
        {
            InitializeComponent();
        }

        private void memoryControlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var dlg = new MemoryControlDialog())
            {
                dlg.ShowDialog(this);
            }
        }

        private void rTFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var dlg = new RtfDialog())
            {
                dlg.ShowDialog(this);
            }
        }

        private void webControlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var dlg = new WebDialog())
            {
                dlg.ShowDialog(this);
            }
        }

        private void searchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sc = new System.ComponentModel.Design.ServiceContainer();
            sc.AddService(typeof(ISettingsService), new DummySettingsService());
            using (var dlg = new Decompiler.Gui.Windows.Forms.SearchDialog())
            {
                dlg.Services = sc;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    //string.foo();
                }
            }
        }

        private class DummySettingsService : ISettingsService
        {
            public object Get(string settingName, object defaultValue)
            {
                return null;
            }

            public string[] GetList(string settingName)
            {
                return null;
            }

            public void SetList(string name, IEnumerable<string> values)
            {
            }

            public void Set(string name, object value)
            {
            }
        }

        private void treeViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var dlg = new TreeViewDialog())
            {
                dlg.ShowDialog(this);
            }
        }

        private void projectBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var dlg = new ProjectBrowserDialog())
            {
                dlg.ShowDialog(this);
            }
        }
    }
}
