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
using System.Configuration;
using System.Linq;
using System.Text;

namespace Decompiler.Core.Configuration
{
    public interface RawFileElement
    {
        string Name { get; set; }
        string Description { get; set; }
        string Architecture { get; set; }
        string Environment { get; set; }
        string BaseAddress { get; set; }
        EntryPointElement EntryPoint { get; }
    }

    //<!-- Raw files have no headers, so we need a hint from the user -->
    //<RawFiles>
    //  <RawFile Name="ms-dos-com" Arch="x86-real-16" Env="ms-dos" Base="0C00:0100">
    //    <Entry Addr="0C00:0000" Name="MsDosCom_Start">
    //      <Register Name="ax" Value="0" />
    //    </Entry>
    //  </RawFile>
    //</RawFiles>

    public class RawFileElementImpl : ConfigurationElement, RawFileElement
    {
        [ConfigurationProperty("Name", IsRequired = true)]
        public string Name
        {
            get { return (string)this["Name"]; }
            set { this["Name"] = value; }
        }

        [ConfigurationProperty("Description", IsRequired = true)]
        public string Description
        {
            get { return (string)this["Description"]; }
            set { this["Description"] = value; }
        }

        [ConfigurationProperty("Arch", IsRequired = true)]
        public string Architecture
        {
            get { return (string)this["Arch"]; }
            set { this["Arch"] = value; }
        }

        [ConfigurationProperty("Env", IsRequired = false)]
        public string Environment
        {
            get { return (string)this["Env"]; }
            set { this["Env"] = value; }
        }

        [ConfigurationProperty("Base", IsRequired = true)]
        public string BaseAddress
        {
            get { return (string)this["Base"]; }
            set { this["Base"] = value; }
        }

        [ConfigurationProperty("Entry")]
        public EntryPointElement EntryPoint 
        {
            get { return (EntryPointElement)this["Entry"];}
        }
    }

    //    <Entry Addr="0C00:0000" Name="MsDosCom_Start">
    //      <Register Name="ax" Value="0" />
    //    </Entry>
    public class EntryPointElement : ConfigurationElement
    {
        [ConfigurationProperty("Name", IsRequired=true)]
        public string Name
        {
            get { return (string)this["Name"]; }
            set { this["Name"] = value; }
        }

        [ConfigurationProperty("Addr", IsRequired = true)]
        public string Address
        {
            get { return (string)this["Addr"]; }
            set { this["Addr"] = value; }
        }
    }
}