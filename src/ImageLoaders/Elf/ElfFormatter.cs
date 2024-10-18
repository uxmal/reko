using Reko.Core.Loading;
using Reko.ImageLoaders.Elf.Relocators;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Reko.ImageLoaders.Elf
{
    public class ElfFormatter : IBinaryFormatter
    {
        private readonly ElfBinaryImage binary;
        private readonly ElfRelocator relocator;

        public ElfFormatter(ElfBinaryImage binary, ElfRelocator relocator)
        {
            this.binary = binary;
            this.relocator = relocator;
        }

        public void DisassembleAllSections(TextWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void DisassembleExecutableSections(TextWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void FormatAllHeaders(TextWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void FormatAllSectionContents(TextWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void FormatArchiveHeaders(TextWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void FormatDebugInformation(TextWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void FormatDynamicRelocations(TextWriter writer)
        {
            Debug.Print("{0,-16} {1,-16} {2}", "OFFSET", "TYPE", "VALUE");
            foreach (var relocation in binary.DynamicRelocations)
            {
                string value = relocation.Symbol?.Name ?? "?";
                var addend = relocation.Addend ?? 0;
                var type = relocator.RelocationTypeToString(relocation.Type);
                if (addend > 0)
                {
                    value = $"{value}+0x{addend:X8}";
                }
                else if (addend < 0)
                {
                    value = $"{value}-0x{addend:X8}";
                }

                writer.WriteLine("{0:X16} {1,-18} {2}",
                    relocation.Offset,
                    type,
                    value);
            }
        }

        public void FormatDynamicSymbols(TextWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void FormatPrivateHeaders(TextWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void FormatRelocations(TextWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void FormatSectionHeaders(TextWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void FormatSymbols(TextWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void FormatWithSource(string prefix, TextWriter writer)
        {
            throw new System.NotImplementedException();
        }
    }
}