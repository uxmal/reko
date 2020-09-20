using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.ImageLoaders.Pef
{
    public class PEFSection {
        public readonly string? Name;
        public readonly PEFSectionHeader SectionHeader;
        public readonly byte[] SectionData;

        public PEFSection(string? sectionName, PEFSectionHeader sectionHeader, byte[] sectionData) {
            Name = sectionName;
            SectionHeader = sectionHeader;
            SectionData = sectionData;
        }
    }
}
