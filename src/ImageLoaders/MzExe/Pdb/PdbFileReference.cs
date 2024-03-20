using Reko.Core.Memory;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.MzExe.Pdb
{
    public class PdbFileReference
    {

        private const int PDB_MAGIC = 0x53445352;

        public PdbFileReference(Guid guid, uint age, string v)
        {
            Guid = guid;
            Age = age;
            Filename = v;
        }

        public Guid Guid { get; }
        public uint Age { get; }
        public string Filename { get; }

        public static PdbFileReference? Load(LeImageReader rdr)
        {
            if (!rdr.TryReadLeUInt32(out var pdbMagic) ||
                 pdbMagic != PDB_MAGIC)
            {
                return null;
            }
            var bytesGuid = rdr.ReadBytes(16);
            if (bytesGuid.Length != 16)
                return null;
            if (!rdr.TryReadLeUInt32(out var word))
                return null;
            var str = rdr.ReadCString(PrimitiveType.Char, Encoding.UTF8);
            return new PdbFileReference(new Guid(bytesGuid), word, str.ToString());

        }
    }
}
