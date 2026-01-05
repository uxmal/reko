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
        private readonly IComparer<string> filenameComparer;
        private readonly Dictionary<string, ArchiveDirectoryEntry> root;

        /// <summary>
        /// Initializes fields of <see cref="AbstractHierarchicalArchive"/>.
        /// </summary>
        /// <param name="location"><see cref="ImageLocation" /> from which the archive was loaded.</param>
        /// <param name="pathSeparator">Path separator to use.</param>
        /// <param name="comparer">Comparer used to compare file names. </param>
        public AbstractHierarchicalArchive(ImageLocation location, char pathSeparator, IComparer<string> comparer)
        {
            this.Location = location;
            this.pathSeparator = pathSeparator;
            this.filenameComparer = comparer;
            this.root = new Dictionary<string, ArchiveDirectoryEntry>();
        }

        /// <inheritdoc/>
        public ArchiveDirectoryEntry? this[string path] => GetEntry(path);

        /// <inheritdoc/>
        public ImageLocation Location { get; }

        /// <inheritdoc/>
        public List<ArchiveDirectoryEntry> RootEntries
            => root.Values
                .OrderBy(e => e.Name, this.filenameComparer)
                .ToList();

        /// <inheritdoc/>
        public T Accept<T, C>(ILoadedImageVisitor<T, C> visitor, C context)
            => visitor.VisitArchive(this, context);

        private ArchiveDirectoryEntry? GetEntry(string path)
        {
            var components = path.Split(pathSeparator);
            if (components.Length == 0)
                return null;
            var curDir = this.root;
            ArchiveDirectoryEntry? entry = null;
            foreach (var component in components)
            {
                if (curDir is null || !curDir.TryGetValue(component, out entry))
                    return null;
                curDir = (entry as ArchiveDictionary)?.entries;
            }
            return entry;
        }

        /// <summary>
        /// Adds a file to the archive.
        /// </summary>
        /// <param name="path">Relative path.</param>
        /// <param name="fileCreator">Delegate which when called returns an <see cref="ArchivedFile"/> instance.
        /// </param>
        /// <returns></returns>
        public virtual ArchivedFile AddFile(
            string path, 
            Func<IArchive, ArchiveDirectoryEntry?, string, ArchivedFile> fileCreator)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException(nameof(path));
            var pathSegments = path.Split(pathSeparator);
            var curDir = this.root;
            ArchivedFolder? parentDir = null;
            for (int i = 0; i < pathSegments.Length - 1; ++i)
            {
                string pathSegment = pathSegments[i];
                if (!curDir.TryGetValue(pathSegment, out var entry))
                {
                    entry = parentDir = new ArchiveDictionary(this, pathSegment, parentDir);
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
            var file = fileCreator(this, parentDir, filename);
            if (!curDir.TryAdd(filename, file))
                ReportDuplicatedFilename(filename);
            return file;
        }

        /// <summary>
        /// Called when a duplicate file name is detected.
        /// </summary>
        /// <param name="filename">Duplicated file name.</param>
        protected virtual void ReportDuplicatedFilename(string filename)
        {
            throw new DuplicateFilenameException($"Duplicated file name '{filename}.");
        }

        /// <inheritdoc/>
        public virtual string GetRootPath(ArchiveDirectoryEntry? entry)
        {
            if (entry is null)
                return "";
            var components = new List<string>();
            while (entry is not null)
            {
                components.Add(entry.Name);
                entry = entry.Parent;
            }
            components.Reverse();
            return string.Join(pathSeparator, components);
        }

        /// <summary>
        /// Represents a directory in an archive.
        /// </summary>
        public class ArchiveDictionary : ArchivedFolder
        {
            private readonly AbstractHierarchicalArchive archive;
            internal readonly Dictionary<string, ArchiveDirectoryEntry> entries;

            /// <summary>
            /// Constructs an instance of <see cref="ArchiveDictionary"/>.
            /// </summary>
            /// <param name="archive">The containing archive.</param>
            /// <param name="name">Name of the directory.</param>
            /// <param name="parent">Parent node of the directory, if any.</param>
            public ArchiveDictionary(AbstractHierarchicalArchive archive, string name, ArchivedFolder? parent)
            {
                this.archive = archive;
                this.Name = name;
                this.Parent = parent;
                this.entries = new Dictionary<string, ArchiveDirectoryEntry>();
            }

            /// <inheritdoc/>
            public ICollection<ArchiveDirectoryEntry> Entries
                => entries.Values
                    .OrderBy(e => e.Name, archive.filenameComparer)
                    .ToList();

            /// <inheritdoc/>
            public string Name { get; }

            /// <inheritdoc/>
            public ArchiveDirectoryEntry? Parent { get; }

            /// <summary>
            /// Adds an entry to the directory.
            /// </summary>
            /// <param name="name">Name of the entry.</param>
            /// <param name="entry">Directory entry.</param>
            public void AddEntry(string name, ArchiveDirectoryEntry entry)
            {
                entries[name] = entry;
            }
        }
    }
}
