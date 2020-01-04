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

using Reko.Core.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.Core.Services
{
    public interface ITypeLibraryLoaderService
    {
        TypeLibrary LoadMetadataIntoLibrary(IPlatform platform, TypeLibraryDefinition tlElement, TypeLibrary libDst);

        string InstalledFileLocation(string name);

        /// <summary>
        /// Loads the characteristics library for the dynamic library
        /// named 'name'.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        CharacteristicsLibrary LoadCharacteristics(string name);
    }

    public class TypeLibraryLoaderServiceImpl : ITypeLibraryLoaderService
    {
        private IServiceProvider services;

        public TypeLibraryLoaderServiceImpl(IServiceProvider services)
        {
            this.services = services;
        }

        public TypeLibrary LoadMetadataIntoLibrary(IPlatform platform, TypeLibraryDefinition tlElement, TypeLibrary libDst)
        {
            var cfgSvc = services.RequireService<IConfigurationService>();
            var fsSvc = services.RequireService<IFileSystemService>();
            var diagSvc = services.RequireService<IDiagnosticsService>();
            try
            {
                string libFileName = cfgSvc.GetInstallationRelativePath(tlElement.Name);
                if (!fsSvc.FileExists(libFileName)) 
                    return libDst;

                byte[] bytes = fsSvc.ReadAllBytes(libFileName);
                MetadataLoader loader = CreateLoader(tlElement, libFileName, bytes);
                if (loader == null)
                    return libDst;
                var lib = loader.Load(platform, tlElement.Module, libDst);
                return lib;
            }
            catch (Exception ex)
            {
                diagSvc.Error(ex, string.Format("Unable to load metadata file {0}.", tlElement.Name));
                return libDst;
            }
        }

        public MetadataLoader CreateLoader(TypeLibraryDefinition tlElement, string filename, byte[] bytes)
        {
            Type loaderType = null;
            if (string.IsNullOrEmpty(tlElement.Loader))
            {
                // By default, assume TypeLibraryLoader is intended.
                loaderType = typeof(TypeLibraryLoader);
            }
            else
            {
                var cfgSvc = services.RequireService<IConfigurationService>();
                var diagSvc = services.RequireService<IDiagnosticsService>();
                var ldrElement = cfgSvc.GetImageLoader(tlElement.Loader);
                if (ldrElement != null && !string.IsNullOrEmpty(ldrElement.TypeName)) 
                {
                    loaderType = Type.GetType(ldrElement.TypeName, false);
                }
                if (loaderType == null)
                {
                    diagSvc.Warn(
                        "Metadata loader type '{0}' is unknown.", 
                        tlElement.Loader);
                    return null;
                }
            }
            return (MetadataLoader)Activator.CreateInstance(loaderType, services, filename, bytes);
        }

        public CharacteristicsLibrary LoadCharacteristics(string name)
        {
            var filename = InstalledFileLocation(name);
            if (!File.Exists(filename))
                return new CharacteristicsLibrary();
            var fsSvc = services.RequireService<IFileSystemService>();
            var lib = CharacteristicsLibrary.Load(filename, fsSvc);
            return lib;
        }

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
