#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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
using System.Configuration;
using System.Text;

namespace Decompiler.Core.Configuration
{
    public interface OperatingEnvironment
    {
        string Name { get; }
        string Description { get; }
        string TypeName { get; }
        TypeLibraryElementCollection TypeLibraries { get; }
    }

    public class OperatingEnvironmentElement : ConfigurationElement, OperatingEnvironment
    {
        public OperatingEnvironmentElement()
        {
            this["TypeLibraries"] = new TypeLibraryElementCollection();
        }

        [ConfigurationProperty("Name", IsRequired = true)]
        public string Name
        {
            get { return (string) this["Name"]; }
            set { this["Name"] = value; }
        }

        [ConfigurationProperty("Description", IsRequired = true)]
        public string Description
        {
            get { return (string) this["Description"]; }
            set { this["Description"] = value; }
        }

        [ConfigurationProperty("Type", IsRequired = true)]
        public string TypeName
        {
            get { return (string) this["Type"]; }
            set { this["Type"] = value; }
        }

        [ConfigurationProperty("TypeLibraries", IsDefaultCollection = false, IsRequired = false)]
        [ConfigurationCollection(typeof(TypeLibraryElement))]
        public TypeLibraryElementCollection TypeLibraries
        {
            get { return (TypeLibraryElementCollection) this["TypeLibraries"]; }
            set { this["TypeLibraries"] = value; }
        }

        public override string ToString()
        {
            return Description;
        }
    }
}
