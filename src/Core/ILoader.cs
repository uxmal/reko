#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
using Reko.Core.Assemblers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reko.Core.Configuration;

namespace Reko.Core
{
    /// <summary>
    /// Implementors of this interface assume responsibility for loading 
    /// binaries or Reko project files.
    /// </summary>
    public interface ILoader
    {
        string DefaultToFormat { get; set; }

        byte[] LoadImageBytes(string fileName, int offset);
        Program LoadExecutable(string fileName, byte[] bytes, string loader, Address loadAddress);
        Program LoadRawImage(string fileName, byte[] image, Address loadAddress, LoadDetails details);
        Program AssembleExecutable(string fileName, string asmName, Address loadAddress);
        Program AssembleExecutable(string fileName, Assembler asm, Address loadAddress);
        Program AssembleExecutable(string fileName, byte[] bytes, Assembler asm, Address loadAddress);

        TypeLibrary LoadMetadata(string fileName, IPlatform platform, TypeLibrary typeLib);
    }

    /// <summary>
    /// Auxiliary details used when the file format itself doesn't provide 
    /// enough details.
    /// </summary>
    public class LoadDetails
    {
        public string LoaderName;
        public string ArchitectureName;
        public string CPUModelName;
        public string PlatformName;
        public string LoadAddress;
        public EntryPointElement EntryPoint;
    }
}
