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
using System.IO;
using System.Text;

namespace Decompiler.Core.Services
{
    public interface ITypeLibraryLoaderService
    {
        TypeLibrary LoadLibrary(IProcessorArchitecture arch, string name);
        CharacteristicsLibrary LoadCharacteristics(string name);
    }

    public class TypeLibraryLoaderServiceImpl : ITypeLibraryLoaderService
    {
        public TypeLibrary LoadLibrary(IProcessorArchitecture arch, string name)
        {
            try
            {
                string libFileName = ImportFileLocation(name);
                if (!File.Exists(libFileName))
                    return null;

                var lib = TypeLibrary.Load(arch, libFileName);
                lib.Filename = libFileName;
                return lib;
            }
            catch
            {
                return null;
            }
        }

        public CharacteristicsLibrary LoadCharacteristics(string name)
        {
            var filename = InstalledFileLocation(name);
            if (!File.Exists(filename))
                return new CharacteristicsLibrary();
            var lib = CharacteristicsLibrary.Load(filename);
            return lib;
        }

        [Obsolete("Let the configuration service do this.")]
        public string InstalledFileLocation(string name)
        {
            string assemblyDir = Path.GetDirectoryName(GetType().Assembly.Location);
            return Path.Combine(assemblyDir, name);
        }

        public string ImportFileLocation(string dllName)
        {
            string assemblyDir = Path.GetDirectoryName(GetType().Assembly.Location);
            return Path.Combine(assemblyDir, Path.ChangeExtension(dllName, ".xml"));
        }
    }
}
