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

using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Core
{
    /// <summary>
    /// An (external) module is described by a name and the services it 
    /// provides.
    /// </summary>
    public class ModuleDescriptor
    {
        public ModuleDescriptor(string name)
        {
            this.ModuleName = name;
            this.ServicesByName = new Dictionary<string, SystemService>();
            this.ServicesByVector = new Dictionary<int, SystemService>();
            this.Globals = new Dictionary<string, DataType>();
        }

        public ModuleDescriptor(ModuleDescriptor other)
        {
            this.ModuleName = other.ModuleName;
            this.ServicesByName = new Dictionary<string, SystemService>(other.ServicesByName);
            this.ServicesByVector = new Dictionary<int, SystemService>(other.ServicesByVector);
            this.Globals = new Dictionary<string, DataType>();
        }

        public string ModuleName { get; private set; }
        public IDictionary<string, SystemService> ServicesByName { get; private set; }
        public IDictionary<int, SystemService> ServicesByVector { get; private set; }
        public IDictionary<string, DataType> Globals { get; private set; }

        public ModuleDescriptor Clone()
        {
            return new ModuleDescriptor(this);
        }
    }
}
