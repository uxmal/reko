#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

using Reko.Core.Loading;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

#nullable enable

namespace Reko.UserInterfaces.WindowsForms.Forms
{
    public class ArchiveBrowserInteractor
    {
        private ArchiveBrowserDialog dlg;

        public ArchiveBrowserInteractor()
        {
            dlg = default!;
        }

        private void EnableControls()
        {
            dlg.OkButton.Enabled =
                dlg.SelectedArchiveEntry is ArchivedFile;
        }

        public void Attach(ArchiveBrowserDialog dlg)
        {
            this.dlg = dlg;
            dlg.Load += new EventHandler(dlg_Load);
            dlg.ArchiveTree.DoubleClick += ArchiveTree_DoubleClick;
            dlg.ArchiveTree.AfterSelect += ArchiveTree_AfterSelect;
        }

        private void ArchiveTree_AfterSelect(object? sender, TreeViewEventArgs e)
        {
            EnableControls();
        }

        void ArchiveTree_DoubleClick(object? sender, EventArgs e)
        {
            if (dlg.SelectedArchiveEntry is not null)
            {
                dlg.DialogResult = (System.Windows.Forms.DialogResult) Gui.Services.DialogResult.OK;
                dlg.Close();
            }
        }

        void dlg_Load(object? sender, EventArgs e)
        {
            Populate(dlg.ArchiveEntries, dlg.ArchiveTree.Nodes);
        }

        private void Populate(ICollection<ArchiveDirectoryEntry> archiveEntries, TreeNodeCollection treeNodeCollection)
        {
            foreach (ArchiveDirectoryEntry entry in archiveEntries)
            {
                TreeNode node = new TreeNode();
                if (entry is ArchivedFile file)
                {
                    node.Text = $"{file.Name} - {file.Length}";
                }
                else
                {
                    node.Text = entry.Name;
                }
                node.Tag = entry;
                if (entry is ArchivedFolder folder)
                {
                    Populate(folder.Entries, node.Nodes);
                }
                treeNodeCollection.Add(node);
            }
        }
    }
}
