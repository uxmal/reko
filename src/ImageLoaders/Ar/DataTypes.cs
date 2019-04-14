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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.Ar
{
    public enum ArFileType
    {
        UNKNOWN,
        FILETYPE_COFF,           //   1         // Windows COFF/PE file
        FILETYPE_ELF,            //   3         // Linux or BSD ELF file
        FILETYPE_MACHO_LE,       //   4         // Mach-O file, little endian
    };

    internal class DataTypes
    {

        const uint ELF_MAGIC = 0x464C457F;  // 0x7F followed by ELF(45 4c 46)

        // Constant for the magic field of the MAC_header (32-bit architectures)
        const uint MAC_MAGIC_32 = 0xFEEDFACE;  // 32 bit little endian
        const uint MAC_MAGIC_64 = 0xFEEDFACF;  // 64 bit little endian
        const uint MAC_CIGAM_32 = 0xCEFAEDFE;  // 32 bit big endian
        const uint MAC_CIGAM_64 = 0xCFFAEDFE;  // 64 bit big endian
        const uint MAC_CIGAM_UNIV = 0xBEBAFECA;  // MacIntosh universal binary


        // Values of Machine:
        const uint PE_MACHINE_I386 = 0x14c;
        const uint PE_MACHINE_X8664 = 0x8664;


        static bool CompareWithString(byte[] data, string str, int length, int offset)
        {
            // From byte array to string
            string s = System.Text.Encoding.UTF8.GetString(data, 0, length);

            if (s == str)
            {
                return true;
            }
            return false;
        }

        internal static uint GetUint8(byte[] data, int offset)
        {
            return data[offset];
        }

        internal static uint GetUint16(byte[] data, int offset)
        {
            return BitConverter.ToUInt16(data, offset);
        }

        internal static uint GetUint32(byte[] data, int offset)
        {
            return BitConverter.ToUInt32(data, offset);
        }

        public static ArFileType GetFileType(byte[] data)
        {
            uint t = GetUint32(data, 0);
            if (GetUint32(data, 0) == ELF_MAGIC)
            {
                // ELF file
                return ArFileType.FILETYPE_ELF;
            }
            else if (GetUint32(data, 0) == MAC_MAGIC_32)
            {
                // Mach-O 32 little endian
                return ArFileType.FILETYPE_MACHO_LE;
            }
            else if (GetUint32(data, 0) == MAC_MAGIC_64)
            {
                // Mach-O 64 little endian
                return ArFileType.FILETYPE_MACHO_LE;
            } 
            else if (GetUint16(data, 0) == PE_MACHINE_I386)
            {
                // COFF/PE 32
                return ArFileType.FILETYPE_COFF;
            }
            else if (GetUint16(data, 0) == PE_MACHINE_X8664)
            {
                // COFF64/PE32+
                return ArFileType.FILETYPE_COFF;
            }          
            else if ((GetUint16(data, 0) & 0xFFF9) == 0x5A49)
            {
                // DOS file or file with DOS stub
                int Signature = (int) GetUint32(data, 0x3C);
                if (Signature + 8 < data.Length)
                {
                    if (GetUint16(data, Signature) == 0x454E)
                    {
                        // Windows 3.x file
                        return ArFileType.UNKNOWN;
                    }
                    else if (GetUint16(data, Signature) == 0x4550)
                    {
                        // COFF file
                        uint MachineType = GetUint16(data, Signature + 4);
                        if (MachineType == PE_MACHINE_I386)
                        {
                            return ArFileType.FILETYPE_COFF;
                        }
                        else if (MachineType == PE_MACHINE_X8664)
                        {
                            return ArFileType.FILETYPE_COFF;
                        }
                    }
                }
            }
            
            // Unknown file type
            return ArFileType.UNKNOWN;
            
        }
    }
}
