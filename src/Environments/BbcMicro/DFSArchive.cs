using Reko.Core;
using Reko.Core.Loading;
using System.Collections.Generic;

namespace Reko.Environments.BbcMicro
{
    public class DFSArchive : IArchive
    {
        private List<ArchiveDirectoryEntry> files;

        public DFSArchive(ArchiveDirectoryEntry[] dir, ImageLocation location)
        {
            this.files = new(dir);
            this.Location = location;
        }

        public ArchiveDirectoryEntry? this[string path] => throw new System.NotImplementedException();

        public List<ArchiveDirectoryEntry> RootEntries => files;

        public ImageLocation Location { get; }

        public T Accept<T, C>(ILoadedImageVisitor<T, C> visitor, C context)
        {
            return visitor.VisitArchive(this, context);
        }

        /// <inheritdoc/>
        public string GetRootPath(ArchiveDirectoryEntry? entry)
        {
            return entry?.Name ?? "";
        }
    }
}