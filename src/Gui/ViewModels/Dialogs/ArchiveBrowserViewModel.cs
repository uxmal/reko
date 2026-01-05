#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

using DynamicData;
using Reko.Core.Loading;
using Reko.Gui.Reactive;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Reko.Gui.ViewModels.Dialogs
{
    public class ArchiveBrowserViewModel : ChangeNotifyingObject
    {
        public ArchiveBrowserViewModel(ICollection<ArchiveDirectoryEntry> archiveEntries)
        {
            ArchiveEntries = archiveEntries
                .Select(e => Create(e))
                .ToList();
        }

        public ICollection<DirectoryEntry> ArchiveEntries { get; }

        public bool OkEnabled
        {
            get => okEnabled;
            set => RaiseAndSetIfChanged(ref okEnabled, value);
        }
        private bool okEnabled;

        public DirectoryEntry? SelectedArchiveEntry
        {
            get => selectedArchiveEntry;
            set
            {
                base.RaiseAndSetIfChanged(ref selectedArchiveEntry, value);
                EnableControls();
            }
        }
        private DirectoryEntry? selectedArchiveEntry;

        private void EnableControls()
        {
            OkEnabled = SelectedArchiveEntry is ArchiveFile;
        }


        public static DirectoryEntry Create(ArchiveDirectoryEntry entry)
        {
            if (entry is Core.Loading.ArchivedFile file)
                return new ArchiveFile($"{file.Name} - {file.Length}", file);
            else if (entry is Core.Loading.ArchivedFolder folder)
                return new ArchiveFolder(folder.Name, folder);
            else
                throw new NotSupportedException();
        }
    }

    public abstract class DirectoryEntry : ChangeNotifyingObject
    {
        public DirectoryEntry(string name, ArchiveDirectoryEntry e)
        {
            this.Name = name;
            this.Entry = e;
        }

        public string Name { get; set; }
        public ArchiveDirectoryEntry Entry { get; }
    }

    public class ArchiveFile : DirectoryEntry
    {
        public ArchiveFile(string name, ArchiveDirectoryEntry e) : base(name, e)
        {
        }
    }

    public class ArchiveFolder : DirectoryEntry
    {
        private Core.Loading.ArchivedFolder folder;
        private ObservableCollection<DirectoryEntry>? entries;

        public ArchiveFolder(string name, Core.Loading.ArchivedFolder f) : base(name, f)
        {
            this.folder = f;
        }

        public ObservableCollection<DirectoryEntry> Entries
        {
            get
            {
                if (this.entries is null)
                {
                    this.entries = new ObservableCollection<DirectoryEntry>(
                        folder.Entries.Select(e => ArchiveBrowserViewModel.Create(e)));
                }
                return this.entries;
            }
        }
    }
}
