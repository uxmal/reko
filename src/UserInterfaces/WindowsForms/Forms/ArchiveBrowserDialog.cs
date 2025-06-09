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
using Reko.Gui.Forms;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms.Forms
{
    public partial class ArchiveBrowserDialog : Form, IArchiveBrowserDialog
    {
        public ArchiveBrowserDialog()
        {
            InitializeComponent();
            var interactor = new ArchiveBrowserInteractor();
            interactor.Attach(this);
        }

        public ICollection<ArchiveDirectoryEntry> ArchiveEntries { get; set; }
        public TreeView ArchiveTree { get { return archiveTree; } }
        public Button OkButton{ get { return btnOK; } }

        public ArchiveDirectoryEntry SelectedArchiveEntry
        {
            get
            {
                return ArchiveTree.SelectedNode is not null
                    ? (ArchiveDirectoryEntry) ArchiveTree.SelectedNode.Tag
                    : null;
            }
        }

        public ArchivedFile Value => SelectedArchiveEntry as ArchivedFile;
    }
}
