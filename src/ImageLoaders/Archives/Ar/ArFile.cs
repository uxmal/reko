using Reko.Core.Loading;
using Reko.Core.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.ImageLoaders.Archives.Ar
{
    public class ArFile : ArchivedFile
    {
        private ArFileHeader fileHeader;
        private ByteImageReader rdr;
        private long fileDataStart;
        private int fileSize;

        public ArFile(ArFileHeader fileHeader, ByteImageReader rdr, long fileDataStart, int filesize)
        {
            this.fileHeader = fileHeader;
            this.rdr = rdr;
            this.fileDataStart = fileDataStart;
            this.fileSize = filesize;
        }

        public string Name => throw new NotImplementedException();

        public byte[] GetBytes()
        {
            rdr.Offset = fileDataStart;
            return rdr.ReadBytes(fileSize);
        }

        internal void AddFile(string sTrimmedFileId)
        {
            throw new NotImplementedException();
        }
    }
}
