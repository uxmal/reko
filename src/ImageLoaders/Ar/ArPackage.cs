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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.Ar
{
    public class ArPackage
    {
        // https://en.wikipedia.org/wiki/Ar_(Unix)

        ArHeader header;
        byte[] packageData;

        public static ArPackage Load(LeImageReader rdr)
        {
            ArPackage package = new ArPackage();

            package.header = ArHeader.Load(rdr);

            // load the data
            int MemberSize = 0;           // Size of member
            int HeaderExtra = 0;          // Extra added to size of header
            //int LongNames;                // Offset to long names member
            //int LongNamesSize = 16;       // Size of long names member
            string Name = "";             // Name of member

            HeaderExtra = 0;

            // See Wiki link for information about the magic string here

            // Size of member
            MemberSize = Convert.ToInt32(package.header.FileSize);
            if (MemberSize + rdr.Offset > rdr.Bytes.Length)
            {
                Console.WriteLine("Library/archive file is corrupt");
                return null;
            }
     
            // Member name
            Name = package.header.Name;
            if (String.Compare(Name, 0, "// ", 0, 3, true) == 0)
            {
                // This is the long names member. Remember its position
                //LongNames = CurrentOffset + ArHeader.Size;
                //LongNamesSize = MemberSize;

                // The long names are terminated by '/' or 0, depending on system,
                // but may contain non-terminating '/'. Find out which type we have:
                // Pointer to LongNames record

                // Find out whether we have terminating zeroes:
                //dataStream

                //if ((LongNamesSize > 1 && p[LongNamesSize - 1] == '/') || (p[LongNamesSize - 1] <= ' ' && p[LongNamesSize - 2] == '/'))
                //{
                // Names are terminated by '/'. Replace all '/' by 0 in the longnames record
                //    for (uint j = 0; j < LongNamesSize; j++, p++)
                //    {
                //        if (*p == '/') *p = 0;
                //    }
                //}
            }
            else if ((String.Compare(Name, 0, "/ ", 0, 2, true) == 0) || (String.Compare(Name, 0, "__.SYMDEF", 0, 9, true) == 0))
            {
                // This is a symbol index member.
                // The symbol index is not used because we are always building a new symbol index.
            }
            /*else if((String.Compare(Name, 0, "/", 0, 1, true) == 0)
                && (Name[1] >= '0'))
                && (Name[1] <= '9')
                && (LongNames > 0))
            {   
                // Name contains index into LongNames record
                NameIndex =(uint) Convert.ToInt32(Name + 1);
                if (NameIndex < LongNamesSize)
                {
                    Name = Buf() + LongNames + NameIndex;
                }
                else
                {
                    Name = "NoName!";
                }
            }*/
            else if (String.Compare(Name, 0, "#1/", 0, 3, true) == 0)
            {
                // Name refers to long name after the header
                // This variant is used by Mac and some versions of BSD
                HeaderExtra = Convert.ToInt32(Name + 3);
                Name += ArHeader.Size;
                if (MemberSize > HeaderExtra)
                {
                    // The length of the name, HeaderExtra, is included in the 
                    // Header->FileSize field. Subtract to get the real file size
                    MemberSize -= HeaderExtra;
                }
            }
            else
            {
                // Ordinary short name
                // Name may be terminated by '/' or space. Replace termination char by 0
                string tmp = Name.Replace('/', ' ');
                char[] charsToTrim = { ',', '.', ' ' };
                Name = tmp.TrimEnd(charsToTrim);
            }


            // Save member as raw data      
            // Create the space for the data
            package.packageData = new byte[MemberSize];

            // Copy the data from the rdr into the local package 
            Buffer.BlockCopy(rdr.Bytes, (int) rdr.Offset, package.packageData, 0, MemberSize);
            rdr.Offset += MemberSize;
            return package;
        }


        public string PackageName
        {
            get
            {
                return header.Name;
            }
        }

        public ArHeader Header
        {
            get
            {
                return header;
            }
        }


        public byte[] PackageData
        {
            get
            {
                return packageData;
            }
        }

        public ArFileType TypeOfData
        {
            get
            {
                return DataTypes.GetFileType(packageData);
            }
        }
    }
}
