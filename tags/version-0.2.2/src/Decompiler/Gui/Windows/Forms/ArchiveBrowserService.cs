/* 
 * Copyright (C) 1999-2010 John Källén.
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

using Decompiler.Core.Archives;
using Decompiler.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows.Forms
{
    public class ArchiveBrowserService : IArchiveBrowserService
    {
        private IServiceProvider sp;

        public ArchiveBrowserService(IServiceProvider sp)
        {
            this.sp = sp;
        }

        public byte[] UserSelectFileFromArchive(ICollection<ArchiveDirectoryEntry> archiveEntries)
        {
            IDecompilerUIService uiSvc = (IDecompilerUIService)sp.GetService(typeof(IDecompilerUIService));
            if (uiSvc == null)
                return null;
            using (ArchiveBrowserDialog dlg = new ArchiveBrowserDialog())
            {
                ArchiveBrowserInteractor interactor = new ArchiveBrowserInteractor(archiveEntries);
                interactor.Attach(dlg);
                if (uiSvc.ShowModalDialog(dlg) == DialogResult.OK)
                    return interactor.GetSelectedFileBytes();
                else
                    return null;
            }
        }

        public class ArchiveBrowserInteractor
        {
            ICollection<ArchiveDirectoryEntry> archiveEntries;
            private ArchiveBrowserDialog dlg;

            public ArchiveBrowserInteractor(ICollection<ArchiveDirectoryEntry> archiveEntries)
            {
                this.archiveEntries = archiveEntries;
            }

            public byte[] GetSelectedFileBytes()
            {
                ArchivedFile file = SelectedArchiveEntry as ArchivedFile;
                return file != null
                    ? file.GetBytes()
                    : null;
                    
            }

            private void EnableControls()
            {
                dlg.OkButton.Enabled =
                    SelectedArchiveEntry != null &&
                    SelectedArchiveEntry is ArchivedFile;
            }

            public void Attach(ArchiveBrowserDialog dlg)
            {
                this.dlg = dlg;
                dlg.Load += new EventHandler(dlg_Load);
                dlg.ArchiveTree.DoubleClick += new EventHandler(ArchiveTree_DoubleClick);
            }

            void ArchiveTree_DoubleClick(object sender, EventArgs e)
            {
                if (SelectedArchiveEntry != null)
                {
                    dlg.DialogResult = DialogResult.OK;
                    dlg.Close();
                }
            }

            private ArchiveDirectoryEntry SelectedArchiveEntry
            {
                get
                {
                    return dlg.ArchiveTree.SelectedNode != null
                        ? (ArchiveDirectoryEntry)dlg.ArchiveTree.SelectedNode.Tag
                        : null;
                }
            }

            void dlg_Load(object sender, EventArgs e)
            {
                Populate(archiveEntries, dlg.ArchiveTree.Nodes);
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
