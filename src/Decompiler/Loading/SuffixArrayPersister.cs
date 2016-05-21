#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text;
using Reko.Core;
using Reko.Core.Lib;

namespace Reko.Loading
{
    public class SuffixArrayPersister
    {
        private IServiceProvider services;

        public SuffixArrayPersister(IServiceProvider services)
        {
            this.services = services;
        }

        public void Save(Dictionary<MemoryArea, SuffixArray<byte>> sufas, Stream stream)
        {
            WriteMagic(stream);
         
        }

        private void WriteMagic(Stream stream)
        {
            stream.Write(new byte[] { 0x52, 0x65, 0x6B, 0x6F, 0x53, 0x66, 0x78, 0x1A }, 0, 8);
        }
        
        private void WriteHeader(Stream stream)
        {
            WriteBeUInt32(0, stream);
        }

        private void WriteBeUInt32(uint w, Stream stm)
        {
            stm.WriteByte((byte)(w >> 24));
            stm.WriteByte((byte)(w >> 16));
            stm.WriteByte((byte)(w >> 8));
            stm.WriteByte((byte)w);
        }
    }
}
