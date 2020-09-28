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
using Reko.Core.Archives;
using Reko.Core.Services;
using Reko.Gui;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms.Forms
{
    public class ArchiveBrowserService : IArchiveBrowserService
    {
        private IServiceProvider services;

        public ArchiveBrowserService(IServiceProvider sp)
        {
            this.services = sp;
        }

        public ArchivedFile UserSelectFileFromArchive(ICollection<ArchiveDirectoryEntry> archiveEntries)
        {
            var dlgFactory = services.GetService<IDialogFactory>();
            if (dlgFactory == null)
                return null;
            var uiSvc = services.GetService<IDecompilerShellUiService>();
            if (uiSvc == null)
                return null;
            using (var dlg = dlgFactory.CreateArchiveBrowserDialog())
            {
                dlg.ArchiveEntries = archiveEntries;
                if (uiSvc.ShowModalDialog(dlg) == Gui.DialogResult.OK)
                    return dlg.GetSelectedFile();
                else
                    return null;
            }
        }

        public class ArchiveBrowserInteractor
        {
            private ArchiveBrowserDialog dlg;

            public ArchiveBrowserInteractor()
            {
            }

            private void EnableControls()
            {
                dlg.OkButton.Enabled =
                    dlg.SelectedArchiveEntry != null &&
                    dlg.SelectedArchiveEntry is ArchivedFile;
            }

            public void Attach(ArchiveBrowserDialog dlg)
            {
                this.dlg = dlg;
                dlg.Load += new EventHandler(dlg_Load);
                dlg.ArchiveTree.DoubleClick += new EventHandler(ArchiveTree_DoubleClick);
            }

            void ArchiveTree_DoubleClick(object sender, EventArgs e)
            {
                if (dlg.SelectedArchiveEntry != null)
                {
                    dlg.DialogResult = (System.Windows.Forms.DialogResult) Gui.DialogResult.OK;
                    dlg.Close();
                }
            }

            void dlg_Load(object sender, EventArgs e)
            {
                Populate(dlg.ArchiveEntries, dlg.ArchiveTree.Nodes);
            }

            private void Populate(ICollection<ArchiveDirectoryEntry> archiveEntries, TreeNodeCollection treeNodeCollection)
            {
                foreach (ArchiveDirectoryEntry entry in archiveEntries)
                {
                    TreeNode node = new TreeNode();
                    node.Text = entry.Name;
                    node.Tag = entry;
                    ArchivedFolder folder = entry as ArchivedFolder;
                    if (folder != null)
                    {
                        Populate(folder.Items, node.Nodes);
                    }
                    treeNodeCollection.Add(node);
                }
            }
        }
    }
}
