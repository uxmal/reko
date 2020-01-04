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

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Reko.Core.Configuration
{
    //<!-- Raw files have no headers, so we need a hint from the user -->
    //<RawFiles>
    //  <RawFile Name="ms-dos-com" Arch="x86-real-16" Env="ms-dos" Base="0C00:0100">
    //    <Entry Addr="0C00:0000" Name="MsDosCom_Start">
    //      <Register Name="ax" Value="0" />
    //    </Entry>
    //  </RawFile>
    //</RawFiles>

    public class RawFileDefinition
    {
        public RawFileDefinition()
        {
            this.EntryPoint = new EntryPointDefinition();
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Architecture { get; set; }

        public string Environment { get; set; }

        public string BaseAddress { get; set; }

        public string Loader { get; set; }

        public EntryPointDefinition EntryPoint { get; set; }
    }

    //    <Entry Addr="0C00:0000" Name="MsDosCom_Start">
    //      <Register Name="ax" Value="0" />
    //    </Entry>
    public class EntryPointDefinition
    {
        /// <summary>
        /// Optional name of the entry point.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The address of the entry point serialized as a string.
        /// The architecture is responsible for converting the string
        /// to a <see cref="Reko.Core.Address"/>.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// If true, the <see cref="Address"/> property indicates an 
        /// indirect entry point. Reko will use the address located at
        /// <see cref="Address"/> as the actual entry point.
        /// </summary>
        public bool Follow { get; set; }
    }
}