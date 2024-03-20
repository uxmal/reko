using Reko.Core.Diagnostics;
using Reko.Core.Memory;
using Reko.Core.Services;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.MzExe.Pdb
{
    /// <summary>
    /// Reads the Multi-Stream File format underlying the PDB format.
    /// </summary>
    public class MsfReader
    {
        private static readonly byte[] signature = Encoding.UTF8.GetBytes("Microsoft C/C++ MSF 7.00\r\n\x001ADS\0\0\0");
        private static readonly TraceSwitch trace = new(nameof(MsfReader), nameof(MsfReader)) { Level = TraceLevel.Verbose };

        private readonly IEventListener listener;

        public MsfReader(PdbStream[] streams, IEventListener listener)
        {
            this.Streams = streams;
            this.listener = listener;
        }

        public static MsfReader? Create(byte[] fileContents, IEventListener listener)
        {
            Debug.Assert(signature.Length == 0x20);
            var rdr = new LeImageReader(fileContents);
            var fileSig = rdr.Read(signature.Length);
            if (!ByteMemoryArea.CompareArrays(fileSig, 0, signature, 0x20))
            {
                listener.Error("Invalid PDB file header.");
                return null;
            }

            // The 'superblock' starts right after the header.

            if (!rdr.TryReadLeUInt32(out uint cbBlock))
                return null;
            if (!rdr.TryReadLeUInt32(out uint FreeBlockMapBlock))
                return null;
            if (!rdr.TryReadLeUInt32(out uint NumBlocks))
                return null;
            if (!rdr.TryReadLeUInt32(out uint NumDirectoryBytes))
                return null;
            if (!rdr.TryReadLeUInt32(out uint Unknown))
                return null;
            if (!rdr.TryReadLeUInt32(out uint BlockMapAddr))
                return null;

            trace.Verbose("MSF: Superblock: size={0:X}; count={1:X} free={2:X} Root={3:X} ({4:X} pages)",
                cbBlock,
                NumBlocks,
                FreeBlockMapBlock,
                BlockMapAddr,
                NumDirectoryBytes);

            // Read the root stream.

            uint offsetBlockMap = BlockMapAddr * cbBlock;
            rdr.Offset = offsetBlockMap;

            var pageIdx = ReadPageIndices(rdr, NumDirectoryBytes, cbBlock);
            var stm1 = new PdbStream(fileContents, pageIdx, NumDirectoryBytes, cbBlock);
            byte[] rootStreamBytes = stm1.ReadWholeStream();

            // Read the stream directory.

            var rdrRoot = new ByteImageReader(rootStreamBytes);
            if (!rdrRoot.TryReadLeUInt32(out uint cStreams))
                return null;
            var stmSizes = new uint[cStreams];
            for (uint i = 0; i < cStreams; ++i)
            {
                if (!rdrRoot.TryReadLeUInt32(out stmSizes[i]))
                    return null;
            }
            var sum = stmSizes.AsEnumerable().Sum(x => x);
            var streams = new PdbStream[cStreams];
            for (uint i = 0; i < cStreams; ++i)
            {
                var pageIndices = ReadPageIndices(rdrRoot, stmSizes[i], cbBlock);
                streams[i] = new PdbStream(fileContents, pageIndices, stmSizes[i], cbBlock);
            }
            return new MsfReader(streams, listener);
        }

        public PdbStream[] Streams { get; }

        public static uint[] ReadPageIndices(ByteImageReader rdr, uint byteSize, uint cbBlock)
        {
            uint cBlocksStream = (byteSize + cbBlock - 1) / cbBlock;
            var pageIndices = new uint[cBlocksStream];
            for (uint i = 0; i < cBlocksStream; ++i)
            {
                pageIndices[i] = rdr.ReadLeUInt32();
            }
            return pageIndices;
        }
    }
}
