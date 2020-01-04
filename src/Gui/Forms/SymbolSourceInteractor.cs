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
using Reko.Core.Configuration;
using Reko.Core.Services;
using Reko.Gui.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Gui.Forms
{
    public class SymbolSourceInteractor
    {
        private ISymbolSourceDialog dlg;
        private IFileSystemService fsSvc;
        private IDecompilerShellUiService uiSvc;

        public void Attach(ISymbolSourceDialog dlg)
        {
            this.dlg = dlg;
            this.dlg.Load += Dlg_Load;
            this.dlg.SymbolFileUrl.LostFocus += SymbolFileUrl_LostFocus;
            this.dlg.CustomSourceCheckbox.CheckedChanged += CustomSourceCheckbox_CheckedChanged;
            this.dlg.AssemblyFile.TextChanged += AssemblyFile_TextChanged;
            this.dlg.BrowseAssemblyFile.Click += BrowseAssemblyFile_Click;
        }

       
        private void Dlg_Load(object sender, EventArgs e)
        {
            this.fsSvc = dlg.Services.RequireService<IFileSystemService>();
            this.uiSvc = dlg.Services.RequireService<IDecompilerShellUiService>();
            PopulateSymbolSources();
            EnableControls();
        }

        private void SymbolFileUrl_LostFocus(object sender, EventArgs e)
        {
            EnableControls();
        }

        private void CustomSourceCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            EnableControls();
        }

        private void AssemblyFile_TextChanged(object sender, EventArgs e)
        {
            var asmFilename = dlg.AssemblyFile.Text;
            if (asmFilename.Length == 0)
            {
                dlg.SymbolSourceClasses.Items.Clear();
                return;
            }
            if (!fsSvc.FileExists(asmFilename))
            {
                return;
            }
            dlg.SymbolSourceClasses.Items.Clear();
            dlg.SymbolSourceClasses.Items.Add("Loading...");
            dlg.Update();

            var symClasses = LoadCompatibleClassesFromAssembly(asmFilename, typeof(ISymbolSource));

            dlg.SymbolSourceClasses.Items.Clear();
            foreach (var symClass in symClasses)
            {
                dlg.SymbolSourceClasses.Items.Add(symClass);
            }
        }

        private void BrowseAssemblyFile_Click(object sender, EventArgs e)
        {
            var newFile = uiSvc.ShowOpenFileDialog(dlg.AssemblyFile.Text);
            if (newFile != null)
            {
                dlg.AssemblyFile.Text = newFile;
            }
        }


        private object[] LoadCompatibleClassesFromAssembly(string asmFilename, Type type)
        {
            try
            {
                var ass = Assembly.LoadFrom(asmFilename);
                var typeNames = ass.DefinedTypes
                    .Where(t => t.ImplementedInterfaces.Contains(type))
                    .Select(t => t.AssemblyQualifiedName)
                    .ToArray();
                return typeNames;
            }
            catch
            {
                return new object[0];
            }
        }

        private void PopulateSymbolSources()
        {
            var cfgSvc = dlg.Services.RequireService<IConfigurationService>();
            var items = cfgSvc.GetSymbolSources();
            //dlg.SymbolSourceList.DataSource = 
            dlg.SymbolSourceList.DataSource = (object)items
                .Select(ss => new string[] { ss.Name, ss.Description });
        }

        private void EnableControls()
        {
            dlg.SymbolSourceList.Enabled =
                dlg.SymbolFileUrl.Text.Length > 0 &&
                !dlg.CustomSourceCheckbox.Checked;
            dlg.SymbolSourceClasses.Enabled =
                dlg.CustomSourceCheckbox.Checked;
            dlg.AssemblyFile.Enabled =
                dlg.CustomSourceCheckbox.Checked;
            dlg.BrowseAssemblyFile.Enabled =
                dlg.CustomSourceCheckbox.Checked;
            dlg.SymbolSourceClasses.Enabled =
                dlg.CustomSourceCheckbox.Checked;

            dlg.OkButton.Enabled =
                dlg.SymbolSourceList.Enabled &&
                dlg.SymbolSourceList.SelectedItems.Count == 1
                ||
                dlg.SymbolSourceClasses.Enabled &&
                dlg.SymbolSourceClasses.SelectedItems.Count == 1;


        }
    }
}
