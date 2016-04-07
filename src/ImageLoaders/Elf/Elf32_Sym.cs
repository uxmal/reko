using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.Elf
{
    public class Elf32_Sym
    {
        public const int Size = 16; // byte size in image.

        public uint st_name;
        public uint st_value;
        public uint st_size;
        public byte st_info;
        public byte st_other;
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

    public enum SymbolType
    {
        STT_NOTYPE = 0,
        STT_OBJECT = 1,
        STT_FUNC = 2,
        STT_SECTION = 3,
        STT_FILE = 4,
        STT_LOPROC = 13,
        STT_HIPROC = 15,
    }
}