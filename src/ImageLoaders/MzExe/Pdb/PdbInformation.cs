using System;

namespace Reko.ImageLoaders.MzExe.Pdb
{
    public class PdbInformation
    {
        public PdbInformation(uint age, Guid guid)
        {
            Age = age;
            Guid = guid;
        }

        public uint Age { get; }
        public Guid Guid { get; }
    }
}