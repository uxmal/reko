#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Reko.Core.Loading
{
    /// <summary>
    /// Handy implementation class for archives that support hierarchical structure.
    /// </summary>
    public abstract class AbstractHierarchicalArchive : IArchive
    {
        private readonly char pathSeparator;
        private readonly Dictionary<string, ArchiveDirectoryEntry> root;

        public AbstractHierarchicalArchive(char pathSeparator)
        {
            this.pathSeparator = pathSeparator;
            this.root = new Dictionary<string, ArchiveDirectoryEntry>();
        }

        public virtual void AddFile(string path, ArchivedFile file)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException(nameof(path));
            var pathSegments = path.Split(pathSeparator);
            var curDir = this.root;
            for (int i = 0; i < pathSegments.Length - 1; ++i)
            {
                string pathSegment = pathSegments[i];
                if (!curDir.TryGetValue(pathSegment, out var entry))
                {
                    entry = new ArchiveDictionary(pathSegment);
                    curDir.Add(pathSegment, entry);
                }
                if (entry is not ArchiveDictionary nextDir)
                {
                    var badPath = string.Join(pathSeparator, pathSegments.Take(i));
                    throw new InvalidOperationException($"The path {badPath} is not a directory.");
                }
                curDir = nextDir.entries;
            }
            var filename = pathSegments[^1];
            if (!curDir.TryAdd(filename, file))
                throw new InvalidOperationException($"The path {path} already exists.");
        }

        public class ArchiveDictionary : ArchivedFolder
        {
            internal readonly Dictionary<string, ArchiveDirectoryEntry> entries;

            public ArchiveDictionary(string name)
            {
                this.Name = name;
                this.entries = new Dictionary<string, ArchiveDirectoryEntry>();
            }

            public ICollection<ArchiveDirectoryEntry> Items => entries.Values;

            public string Name { get; }

            public void AddEntry(string name, ArchiveDirectoryEntry entry)
            {
                entries[name] = entry;
            }
        }

        public List<ArchiveDirectoryEntry> Load(Stream stm)
        {
            throw new NotImplementedException();
        }
    }
}
