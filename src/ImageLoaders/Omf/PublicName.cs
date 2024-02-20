using System;

namespace Reko.ImageLoaders.Omf
{
    public class PublicName
    {
        private ushort typeIndex;

        public PublicName(string s, ushort offset, ushort typeIndex)
        {
            this.Name = s;
            this.Offset = offset;
            this.typeIndex = typeIndex;
        }

        public string Name { get; }
        public ushort Offset { get; }

        public override string ToString()
        {
            return $"Offset: 0x{Offset:X4} Type:{typeIndex:X2} {Name}";
        }
    }
}