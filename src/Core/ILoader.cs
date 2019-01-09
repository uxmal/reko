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
        /// <summary>
        /// Name of the loader to use. Loader names are found in the reko.config file.
        /// </summary>
        public string LoaderName;
        /// <summary>
        /// Name of the processor architecture to use. Architecture names are found 
        /// in the reko.config file.
        /// </summary>
        public string ArchitectureName;
        /// <summary>
        /// Architecture specific options.
        /// </summary>
        public Dictionary<string,object> ArchitectureOptions;    
        /// <summary>
        /// Name of the platform to use. Platform names are found in the 
        /// reko.config file.
        /// </summary>
        public string PlatformName;
        /// <summary>
        /// String representation of the address at which the binary file should
        /// be loaded. The address string is parsed by the architecture when loading.
        /// </summary>
        public string LoadAddress;
        /// <summary>
        /// Entry point of the program.
        /// </summary>
        public EntryPointElement EntryPoint;
    }
}
