#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion

using Reko.Core;
using System.Collections.Generic;
using System.Text;

namespace Reko.ImageLoaders.Coff
{
    public class SectionHeader
    {
        // Section flags values
        const uint PE_SCN_CNT_CODE = 0x00000020;  // section contains executable code
        const uint PE_SCN_CNT_INIT_DATA = 0x00000040;  // section contains initialized data
        const uint PE_SCN_CNT_UNINIT_DATA = 0x00000080;  // section contains unintialized data
        const uint PE_SCN_LNK_INFO = 0x00000200;  // section contains comments or .drectve
        const uint PE_SCN_LNK_REMOVE = 0x00000800;  // will not be part of the image. object files only
        const uint PE_SCN_LNK_COMDAT = 0x00001000;  // section contains communal data
        const uint PE_SCN_ALIGN_1 = 0x00100000;  // Align data by 1
        const uint PE_SCN_ALIGN_2 = 0x00200000;  // Align data by 2
        const uint PE_SCN_ALIGN_4 = 0x00300000;  // Align data by 4
        const uint PE_SCN_ALIGN_8 = 0x00400000;  // Align data by 8
        const uint PE_SCN_ALIGN_16 = 0x00500000;  // Align data by 16
        const uint PE_SCN_ALIGN_32 = 0x00600000;  // Align data by 32
        const uint PE_SCN_ALIGN_64 = 0x00700000;  // Align data by 64
        const uint PE_SCN_ALIGN_128 = 0x00800000;  // Align data by 128
        const uint PE_SCN_ALIGN_256 = 0x00900000;  // Align data by 256
        const uint PE_SCN_ALIGN_512 = 0x00a00000;  // Align data by 512
        const uint PE_SCN_ALIGN_1024 = 0x00b00000;  // Align data by 1024
        const uint PE_SCN_ALIGN_2048 = 0x00c00000;  // Align data by 2048
        const uint PE_SCN_ALIGN_4096 = 0x00d00000;  // Align data by 4096
        const uint PE_SCN_ALIGN_8192 = 0x00e00000;  // Align data by 8192
        const uint PE_SCN_ALIGN_MASK = 0x00f00000;  // Mask for extracting alignment info
        const uint PE_SCN_LNK_NRELOC_OVFL = 0x01000000;  // section contains extended relocation
        const uint PE_SCN_MEM_DISCARDABLE = 0x02000000;  // section is discardable
        const uint PE_SCN_MEM_NOT_CACHED = 0x04000000;  // section cannot be cached
        const uint PE_SCN_MEM_NOT_PAGED = 0x08000000;  // section is not pageable
        const uint PE_SCN_MEM_SHARED = 0x10000000;  // section can be shared
        const uint PE_SCN_MEM_EXECUTE = 0x20000000;  // section is executable
        const uint PE_SCN_MEM_READ = 0x40000000;  // section is readable
        const uint PE_SCN_MEM_WRITE = 0x80000000;  // section is writeable

        internal string Name;         // section name char Name[8]; 
        internal uint VirtualSize;    // size of section when loaded. (Should be 0 for object files, but it seems to be accumulated size of all sections)
        internal uint VirtualAddress; // subtracted from offsets during relocation. preferably 0
        internal uint SizeOfRawData;  // section size in file
        internal uint PRawData;       // file  to raw data for section
        internal uint PRelocations;   // file  to relocation entries
        internal uint PLineNumbers;   // file  to line number entries
        internal ushort NRelocations; // number of relocation entries
        internal ushort NLineNumbers; // number of line number entries
        internal uint Flags;          // flags

        public const int Size = 40;
        internal ushort MachineType;

        internal List<Relocation> relocationList;

        public SectionHeader()
        {
            relocationList = new List<Relocation>();
        }

        public static SectionHeader Load(LeImageReader rdr, ushort machineType)
        {
            SectionHeader header = new SectionHeader();

            header.MachineType = machineType;

            header.Name = Encoding.UTF8.GetString(rdr.Bytes, (int)rdr.Offset, 8);   // section name char Name[8]; 
            rdr.Offset += 8;
            string tmp = header.Name.Replace('\0', ' ');
            char[] charsToTrim = { ',', '.', ' ' };

            header.Name = tmp.TrimEnd(charsToTrim);
            header.VirtualSize = rdr.ReadUInt32();       // size of section when loaded. (Should be 0 for object files, but it seems to be accumulated size of all sections)
            header.VirtualAddress = rdr.ReadUInt32();    // subtracted from offsets during relocation. preferably 0
            header.SizeOfRawData = rdr.ReadUInt32();     // section size in file
            header.PRawData = rdr.ReadUInt32();          // file  to raw data for section
            header.PRelocations = rdr.ReadUInt32();      // file  to relocation entries
            header.PLineNumbers = rdr.ReadUInt32();      // file  to line number entries
            header.NRelocations = rdr.ReadLeUInt16();    // number of relocation entries
            header.NLineNumbers = rdr.ReadLeUInt16();    // number of line number entries
            header.Flags = rdr.ReadUInt32();             // flags  

            return header;
        }

        internal List<uint> FindLocationPoints(uint start, uint end)
        {
            List<uint> exemptList = new List<uint>();

            foreach(Relocation reco in relocationList)
            {
                if((reco.VirtualAddress > start) && (reco.VirtualAddress < end))
                {
                    if (MachineType == FileHeader.PE_MACHINE_I386)
                    {
                        switch (reco.Type)
                        {
                        case Relocation.COFF32_RELOC_DIR32: // 32-bit absolute virtual address
                        case Relocation.COFF32_RELOC_IMGREL:   // 32-bit image relative virtual address
                        case Relocation.COFF32_RELOC_REL32:  // 32-bit EIP-relative
                        case Relocation.COFF32_RELOC_SECREL:   // 32-bit section-relative
                            {
                                exemptList.Add(reco.VirtualAddress - start);
                                exemptList.Add(reco.VirtualAddress + 1 - start);
                                exemptList.Add(reco.VirtualAddress + 2 - start);
                                exemptList.Add(reco.VirtualAddress + 3 - start);
                            }
                            break;

                        case Relocation.COFF32_RELOC_SECTION:   // 16-bit section index in file
                            {
                                exemptList.Add(reco.VirtualAddress - start);
                                exemptList.Add(reco.VirtualAddress + 1 - start);
                            }
                            break;
                        }
                    }
                    else
                    {
                        switch (reco.Type)
                        {
                            case Relocation.COFF64_RELOC_SECTION:     // 16-bit section index in file. For debug purpose     
                            case Relocation.COFF64_RELOC_PPC_GPREL:   // 16 bit signed relative to GP
                            case Relocation.COFF64_RELOC_PPC_SECRELO: // low 16 bits of section relative
                            {
                                exemptList.Add(reco.VirtualAddress - start);
                                exemptList.Add(reco.VirtualAddress + 1 - start);
                            }
                            break;

                            case Relocation.COFF64_RELOC_ABS32:     // 32 bit absolute virtual address
                            case Relocation.COFF64_RELOC_IMGREL:    // 32 bit image-relative
                            case Relocation.COFF64_RELOC_REL32:     // 32 bit, RIP-relative
                            case Relocation.COFF64_RELOC_REL32_1:   // 32 bit, relative to RIP - 1. For instruction with immediate byte operand
                            case Relocation.COFF64_RELOC_REL32_2:   // 32 bit, relative to RIP - 2. For instruction with immediate word operand
                            case Relocation.COFF64_RELOC_REL32_3:   // 32 bit, relative to RIP - 3. (useless)
                            case Relocation.COFF64_RELOC_REL32_4:   // 32 bit, relative to RIP - 4. For instruction with immediate dword operand
                            case Relocation.COFF64_RELOC_REL32_5:   // 32 bit, relative to RIP - 5. (useless)
                            case Relocation.COFF64_RELOC_SECREL:    // 32-bit section-relative
                            case Relocation.COFF64_RELOC_SREL32:    // 32 bit signed span dependent
                            case Relocation.COFF64_RELOC_PPC_REFHI:   // high 16 bits of 32 bit abs addr
                            case Relocation.COFF64_RELOC_PPC_REFLO:   // low  16 bits of 32 bit abs addr
                            {
                                exemptList.Add(reco.VirtualAddress - start);
                                exemptList.Add(reco.VirtualAddress + 1 - start);
                                exemptList.Add(reco.VirtualAddress + 2 - start);
                                exemptList.Add(reco.VirtualAddress + 3 - start);
                            }
                            break;

                            case Relocation.COFF64_RELOC_ABS64:   // 64 bit absolute virtual address
                            {
                                exemptList.Add(reco.VirtualAddress - start);
                                exemptList.Add(reco.VirtualAddress + 1 - start);
                                exemptList.Add(reco.VirtualAddress + 2 - start);
                                exemptList.Add(reco.VirtualAddress + 3 - start);
                                exemptList.Add(reco.VirtualAddress + 4 - start);
                                exemptList.Add(reco.VirtualAddress + 5 - start);
                                exemptList.Add(reco.VirtualAddress + 6 - start);
                                exemptList.Add(reco.VirtualAddress + 7 - start);
                            }
                            break;
                        }
                    }
                }
            }
            return exemptList;
        }
    }
}
