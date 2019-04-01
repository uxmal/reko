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


namespace makesigs
{
    internal class FileBuffer
    {
        public enum FileTypes
        {
            UNKNOWN,
            FILETYPE_COFF,           //   1         // Windows COFF/PE file
            FILETYPE_OMF,            //   2         // Windows OMF file
            FILETYPE_ELF,            //   3         // Linux or BSD ELF file
            FILETYPE_MACHO_LE,       //   4         // Mach-O file, little endian
            FILETYPE_MACHO_BE,       //   5         // Mach-O file, big endian
            FILETYPE_DOS,            //   6         // DOS file
            FILETYPE_WIN3X,          //   7         // Windows 3.x file
            IMPORT_LIBRARY_MEMBER,   //0x10         // Member of import library, Windows
            FILETYPE_MAC_UNIVBIN,    //0x11         // Macintosh universal binary
            FILETYPE_MS_WPO,         //0x20         // Object file for whole program optimization, MS
            FILETYPE_INTEL_WPO,      //0x21         // Object file for whole program optimization, Intel
            FILETYPE_WIN_UNKNOWN,    //0x29         // Unknown subtype, Windows
            FILETYPE_ASM,            //0x100        // Disassembly output
            FILETYPE_LIBRARY,        //0x1000       // UNIX-style library/archive
            FILETYPE_OMFLIBRARY      //0x2000       // OMF-style  library
        };

        // Constant for the magic field of the MAC_header (32-bit architectures)
        const uint MAC_MAGIC_32 = 0xFEEDFACE;  // 32 bit little endian
        const uint MAC_MAGIC_64 = 0xFEEDFACF;  // 64 bit little endian
        const uint MAC_CIGAM_32 = 0xCEFAEDFE;  // 32 bit big endian
        const uint MAC_CIGAM_64 = 0xCFFAEDFE;  // 64 bit big endian
        const uint MAC_CIGAM_UNIV = 0xBEBAFECA;  // MacIntosh universal binary


        // Values of Machine:
        const uint PE_MACHINE_I386 = 0x14c;
        const uint PE_MACHINE_X8664 = 0x8664;

        protected byte[] dataStream;
        protected int WordSize;  
        protected int DataSize;
        protected FileTypes FileType;



        public void SetData(byte[] data, int offset, int size)
        {
            dataStream = new byte[size];
            DataSize = size;
            Buffer.BlockCopy(data, offset, dataStream, 0, size);
        }

        internal byte[] GetData()
        {
            return dataStream;
        }
        bool CompareWithString(string str, int length, int offset)
        {
            // From byte array to string
            string s = System.Text.Encoding.UTF8.GetString(dataStream, 0, length);

            if (s == str)
                return true;
            return false;
        }


        internal uint GetUint8(int offset)
        {
            return dataStream[offset];
        }
        internal uint GetUint16(int offset)
        {
            return BitConverter.ToUInt16(dataStream, offset);
        }

        internal uint GetUint32(int offset)
        {
            return BitConverter.ToUInt32(dataStream, offset);
        }

        internal int GetDataSize()
        {
            return DataSize;
        }


        internal FileTypes GetFileType()
        {
            // Detect file type
            if (FileType != FileTypes.UNKNOWN)
            {
                return FileType;            // File type already known
            }
            if (DataSize == 0)
            {
                return FileTypes.UNKNOWN;                  // No file
            }

            if (dataStream == null)
            {
                return FileTypes.UNKNOWN;                     // No contents
            }


            if (CompareWithString("!<arch>", 7, 0) == true)
            {
                // UNIX style library. Contains members of file type COFF, ELF or MACHO
                FileType = FileTypes.FILETYPE_LIBRARY;
            }
            else if (CompareWithString("\\177ELF", 4, 0) == true)
            {
                // ELF file
                FileType = FileTypes.FILETYPE_ELF;
                /*
                switch (Buf()[EI_CLASS])
                {
                    case ELFCLASS32:
                        WordSize = 32; break;
                    case ELFCLASS64:
                        WordSize = 64; break;
                }
                */
            }
            else if (GetUint32(0) == MAC_MAGIC_32)
            {
                // Mach-O 32 little endian
                FileType = FileTypes.FILETYPE_MACHO_LE;
                WordSize = 32;
            }
            else if (GetUint32(0) == MAC_MAGIC_64)
            {
                // Mach-O 64 little endian
                FileType = FileTypes.FILETYPE_MACHO_LE;
                WordSize = 64;
            }
            else if (GetUint32(0) == MAC_CIGAM_32)
            {
                // Mach-O 32 big endian
                FileType = FileTypes.FILETYPE_MACHO_BE;
                WordSize = 32;
            }
            else if (GetUint32(0) == MAC_CIGAM_64)
            {
                // Mach-O 64 big endian
                FileType = FileTypes.FILETYPE_MACHO_BE;
                WordSize = 64;
            }
            else if (GetUint32(0) == MAC_CIGAM_UNIV)
            {
                // MacIntosh universal binary
                FileType = FileTypes.FILETYPE_MAC_UNIVBIN;
                WordSize = 0;
            }
            else if (GetUint32(0) == 0xFFFF0000 || GetUint32(0) == 0x10000)
            {
                // Windows subtypes:
                if (GetUint16(4) == 0)
                {
                    // This type only occurs when attempting to extract a member from an import library
                    FileType = FileTypes.IMPORT_LIBRARY_MEMBER;
                }
                else if (GetUint16(4) == 1)
                {
                    // Whole program optimization intermediate file for MS compiler. Undocumented
                    FileType = FileTypes.FILETYPE_MS_WPO;
                }
                else
                {
                    // Other subtypes not known
                    FileType = FileTypes.FILETYPE_WIN_UNKNOWN;
                }
                // Get word size
                if (GetUint16(6) == PE_MACHINE_I386)
                {
                    WordSize = 32;
                }
                else if (GetUint16(6) == PE_MACHINE_X8664)
                {
                    WordSize = 64;
                }
                else
                {
                    WordSize = 0;
                }
            }
            else if (GetUint16(0) == PE_MACHINE_I386)
            {
                // COFF/PE 32
                FileType = FileTypes.FILETYPE_COFF;
                WordSize = 32;
            }
            else if (GetUint16(0) == PE_MACHINE_X8664)
            {
                // COFF64/PE32+
                FileType = FileTypes.FILETYPE_COFF;
                WordSize = 64;
            }
            /*else if (GetUint8(0) == OMF_THEADR)
            {
                // OMF 16 or 32
                FileType = FileTypes.FILETYPE_OMF;
                // Word size can only be determined by searching through records in file:
                GetOMFWordSize(); // Determine word size
            }
            else if (GetUint8(0) == OMF_LIBHEAD)
            {
                // OMF Library 16 or 32
                FileType = FileTypes.FILETYPE_OMFLIBRARY;
            }*/
            else if ((GetUint16(0) & 0xFFF9) == 0x5A49)
            {
                // DOS file or file with DOS stub
                FileType = FileTypes.FILETYPE_DOS;
                WordSize = 16;
                int Signature = (int) GetUint32(0x3C);
                if (Signature + 8 < DataSize)
                {
                    if (GetUint16(Signature) == 0x454E)
                    {
                        // Windows 3.x file
                        FileType = FileTypes.FILETYPE_WIN3X;
                    }
                    else if (GetUint16(Signature) == 0x4550)
                    {
                        // COFF file
                        uint MachineType = GetUint16(Signature + 4);
                        if (MachineType == PE_MACHINE_I386)
                        {
                            FileType = FileTypes.FILETYPE_COFF;
                            WordSize = 32;
                        }
                        else if (MachineType == PE_MACHINE_X8664)
                        {
                            FileType = FileTypes.FILETYPE_COFF;
                            WordSize = 64;
                        }
                    }
                }
            }
            /*
            else if (namelen > 4 && stricmp(FileName + namelen - 4, ".com") == 0)
            {
                // DOS .com file recognized only from its extension
                FileType = FileTypes.FILETYPE_DOS;
                WordSize = 16;
            }
            else if (GetUint16(0) == 0 && InputFilePath.Length > 4 && (String.Compare(InputFilePath, InputFilePath.Length - 4, ".obj", 0, 4, true) == 0))
            {
                // Possibly alias record in COFF library
                FileType = FileTypes.FILETYPE_COFF;
                WordSize = 0;
            }*/
            else
            {
                // Unknown file type
                //uint utype = GetUint32(0);
                FileType = FileTypes.UNKNOWN;
            }
            return FileType;

        }
    }
}
