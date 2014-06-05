using Decompiler.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.ImageLoaders.Elf
{
    public class Elf32_Sym
    {
        public const int Size = 16; // byte size in image.
        
        public uint st_name;
        public uint st_value;
        uint st_size;
        public byte st_info;
        byte st_other;
        public ushort st_shndx;

        public static Elf32_Sym Load(ImageReader rdr)
        {
            var sym = new Elf32_Sym();
            sym.st_name = rdr.ReadUInt32();
            sym.st_value = rdr.ReadUInt32();
            sym.st_size = rdr.ReadUInt32();
            sym.st_info = rdr.ReadByte();
            sym.st_other = rdr.ReadByte();
            sym.st_shndx = rdr.ReadUInt16();
            return sym;
        }
    }
}