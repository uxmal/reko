using Reko.Core.Memory;
using System;

namespace Reko.ImageLoaders.MzExe.Pdb
{
    internal class PdbParser
    {
        private MsfReader msfReader;

        public PdbParser(MsfReader msfReader)
        {
            this.msfReader = msfReader;
        }

        public PdbInformation? GetPdbInfo()
        {
            var stm = msfReader.Streams[1];
            var data = stm.ReadWholeStream();
            var rdr = new LeImageReader(data);
            if (!rdr.TryReadLeUInt32(out var version))
                return null;
            if (!rdr.TryReadLeUInt32(out var signature))
                return null;
            if (!rdr.TryReadLeUInt32(out var age))
                return null;
            var abGuid = rdr.ReadBytes(16);
            var guid = new Guid(abGuid);
            return new PdbInformation(age, guid);
        }
    }
}