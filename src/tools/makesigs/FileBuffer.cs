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
        internal enum FileTypes
        {
            UNKNOWN,
            FILETYPE_LIBRARY,        //0x1000         // UNIX-style library/archive
            FILETYPE_OMFLIBRARY      //0x2000         // OMF-style  library
        };

        const byte OMF_LIBHEAD = 0xF0;  // Library Header Record

        protected byte[] dataStream;
        protected int WordSize;
        protected bool Executable;
        protected int DataSize;
        protected FileTypes FileType;
        protected string InputFilePath;

        public FileBuffer(string filename)
        {
            FileType = FileTypes.UNKNOWN;
            InputFilePath = filename;
            DataSize = 0;
            dataStream = null;
            WordSize = 0;
            Executable = false;
        }

        public FileBuffer()
        {
            FileType = FileTypes.UNKNOWN;
            DataSize = 0;
            dataStream = null;
            WordSize = 0;
            Executable = false;
        }

        public string Filename
        {
            get
            {
                return InputFilePath;
            }
            set
            {
                InputFilePath = value;
            }
        }

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
            {
                return true;
            }
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
            else if (GetUint8(0) == OMF_LIBHEAD)
            {
                // OMF Library 16 or 32
                FileType = FileTypes.FILETYPE_OMFLIBRARY;
            }

            return FileType;
        }
    }
}
