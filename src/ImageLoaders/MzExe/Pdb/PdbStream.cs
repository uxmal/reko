using Microsoft.VisualBasic;
using Reko.Core.Memory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.MzExe.Pdb
{
    public class PdbStream
    {
        private readonly byte[] bytes;
        internal readonly uint[] pageIndices;
        internal readonly uint byteSize;        // Total bytes in the stream
        internal readonly uint cbBlock;         // # of bytes in a block

        public PdbStream(byte[] bytes, uint[] pageIndices, uint byteSize, uint cbBlock)
        {
            this.bytes = bytes;
            this.pageIndices = pageIndices;
            this.byteSize = byteSize;
            this.cbBlock = cbBlock;
        }



        public byte[] ReadWholeStream()
        {
            byte[] arrayOfByte = new byte[this.byteSize];
            ReadStreamPages(0, this.byteSize, arrayOfByte, 0);
            return arrayOfByte;
        }

        private void ReadStreamPages(uint streamOffset, uint cbToRead, byte[] result, int thisnt3) /*throws IOException*/
        {
            if (streamOffset > this.byteSize)
            {
                return;
            }
            if (streamOffset + cbToRead > this.byteSize)
            {
                cbToRead = this.byteSize - streamOffset;
            }

            uint cbLeft = cbToRead;
            uint cbReadSoFar = 0;

            int m_iPage = (int)(streamOffset / this.cbBlock);
            uint offsetInPage = (streamOffset % this.cbBlock);
            var rdr = new LeImageReader(bytes, 0);
            for (; cbLeft > 0; m_iPage++)
            {
                uint offset = this.pageIndices[m_iPage] * this.cbBlock + offsetInPage;
                uint cbSmallRead = Math.Min(cbLeft, this.cbBlock - offsetInPage);
                rdr.Offset = offset;
                rdr.ReadBytes(result, (int)cbReadSoFar, cbSmallRead);
                cbReadSoFar += cbSmallRead;
                cbLeft -= cbSmallRead;
                offsetInPage = 0;
            }
        }
    }
}
