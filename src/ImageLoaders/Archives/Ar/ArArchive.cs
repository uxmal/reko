using Reko.Core.Loading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Reko.ImageLoaders.Archives.Ar
{
    public class ArArchive : IArchive
    {
        private Dictionary<string, ArFile> root;

        public ArArchive()
        {
            this.root = new Dictionary<string, ArFile>();
        }

        public List<ArchiveDirectoryEntry> Load(Stream stm)
        {
            throw new NotImplementedException();
        }

        public void AddFile(string path, ArFile arfile)
        {
            root[path] = arfile;
        }
    }
}
