#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Decompiler.ImageLoaders.Elf
{
    public class SectionInfo
    {
        //public string pSectionName;
        //public SectionHeaderType uType;
        //public bool IsCode;
        //public bool IsBss;
        public uint uNativeAddr;      // address in loaded image.
        public uint uHostAddr;        // offset within file
        //public uint uSectionSize;
        //public uint uSectionEntrySize;
        //public bool bData;
        //public bool IsReadOnly;

        [Conditional("DEBUG")]
        public void Dump()
        {
            //Debug.Print("pSectionName: {0}", pSectionName);
            //Debug.Print("uType: {0}", uType);
            //Debug.Print("IsCode: {0}", IsCode);
            //Debug.Print("IsBss: {0}", IsBss);
            //Debug.Print("uNativeAddr: {0:X}", uNativeAddr);
            //Debug.Print("uHostAddr: {0:X}", uHostAddr);
            //Debug.Print("uSectionSize: {0:X}", uSectionSize);
            //Debug.Print("uSectionEntrySize: {0:X}", uSectionEntrySize);
            //Debug.Print("bData: {0}", bData);
            //Debug.Print("IsReadOnly: {0}", IsReadOnly);
        }
    }
}
