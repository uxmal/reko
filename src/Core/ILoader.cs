#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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

        /// <summary>
        /// Opens the specified files and loads the contents of the file,
        /// starting at file offset <paramref name="offset"/>. No interpretation
        /// of the file data is done.
        /// </summary>
        /// <param name="fileName">Name of the file to read.</param>
        /// <param name="offset">Offset at which to start reading.</param>
        /// <returns>An array of bytes containing the file data.</returns>
        byte[] LoadImageBytes(string fileName, int offset);

        /// <summary>
        /// Given a executable file image in <param name="bytes">, determines which file 
        /// format the file has and delegates loading to a specific image loader.
        /// </summary>
        /// <param name="fileName">The file name of the executable.</param>
        /// <param name="bytes">The contents of the executable file.</param>
        /// <param name="loader">The (optional) name of a specific loader. Providing a non-zero loader will
        /// override the file format determination process.</param>
        /// <param name="loadAddress">Address at which to load the binary. This may be null,
        /// in which case the default address of the image loader will be used.</param>
        /// <returns>
        /// Either a successfully loaded <see cref="Reko.Core.Program"/>, or null if 
        /// an appropriate image loader could not be determined or loaded.
        /// </returns>
        Program LoadExecutable(string fileName, byte[] bytes, string loader, Address loadAddress);

        /// <summary>
        /// Given a sequence of raw bytes, loads it into memory and applies the 
        /// <paramref name="details"/> to it. Use this method if the binary has no known file
        /// format.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="image">The raw contents of the file.</param>
        /// <param name="loadAddress">The address at which the raw contents are to be loaded.</param>
        /// <param name="details">Details about the contents of the file.</param>
        /// <returns>A <see cref="Reko.Core.Program"/>.
        /// </returns>
        Program LoadRawImage(string fileName, byte[] image, Address loadAddress, LoadDetails details);


        Program AssembleExecutable(string fileName, string asmName, Address loadAddress);
        Program AssembleExecutable(string fileName, Assembler asm, Address loadAddress);
        Program AssembleExecutable(string fileName, byte[] bytes, Assembler asm, Address loadAddress);

        /// <summary>
        /// Loads a file containing symbolic, type, or other metadata into a <see cref="Reko.Core.TypeLibrary>"/>.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="platform">The operating environment for the file.</param>
        /// <param name="typeLib">A type library into which the metadata will be added.</param>
        /// <returns>The updated <paramref name="typeLib"/>.
        /// </returns>
        TypeLibrary LoadMetadata(string fileName, IPlatform platform, TypeLibrary typeLib);
    }

    /// <summary>
    /// Auxiliary details used when the file format itself doesn't provide 
    /// enough information to be loaded correctly.
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
        /// Architecture specific options. Each architecture defines its own
        /// set of options, like endianness, processor models etc.
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
        public EntryPointDefinition EntryPoint;
    }
}
