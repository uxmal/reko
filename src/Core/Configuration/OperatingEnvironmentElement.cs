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

using Reko.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace Reko.Core.Configuration
{
    public interface OperatingEnvironment
    {
        string Name { get; }
        string Description { get; }
        string TypeName { get; }
        string MemoryMapFile { get; }
        Dictionary<string, object> Options { get; }

        List<ITypeLibraryElement> TypeLibraries { get; }
        List<ITypeLibraryElement> CharacteristicsLibraries { get; }

        IPlatform Load(IServiceProvider services, IProcessorArchitecture arch);
    }

    public class OperatingEnvironmentElement : OperatingEnvironment
    {
        public OperatingEnvironmentElement()
        {
            this.TypeLibraries = new List<ITypeLibraryElement>();
            this.CharacteristicsLibraries = new List<ITypeLibraryElement>();
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public string TypeName { get; set; }

        public string MemoryMapFile { get; set; }

        public List<ITypeLibraryElement> TypeLibraries { get; internal set; }
        public List<ITypeLibraryElement> CharacteristicsLibraries { get; internal set; }
        public Dictionary<string, object> Options { get; internal set; }

        public IPlatform Load(IServiceProvider services, IProcessorArchitecture arch)
        {
            var type = Type.GetType(TypeName);
            if (type == null)
                throw new TypeLoadException(
                    string.Format("Unable to load {0} environment.", Description));
            var platform = (Platform) Activator.CreateInstance(type, services, arch);
            platform.Name = this.Name;
            if (!string.IsNullOrEmpty(MemoryMapFile))
            {
                platform.MemoryMap = MemoryMap_v1.LoadMemoryMapFromFile(services, MemoryMapFile, platform);
            }
            platform.Description = this.Description;
            return platform;
        }

        public override string ToString()
        {
            return Description;
        }
    }
}
