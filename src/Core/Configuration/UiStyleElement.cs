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

namespace Reko.Core.Configuration
{
    public interface UiStyle
    {
        string Name { get; }
        string FontName { get; }
        string ForeColor { get; }
        string BackColor { get; }
    }

    public class UiStyleElement : ConfigurationElement, UiStyle
    {
        [ConfigurationProperty("name", IsRequired=true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set {  this["name"] = value; }
        }

        [ConfigurationProperty("font", IsRequired = true)]
        public string FontName
        {
            get { return (string)this["font"]; }
            set {  this["font"] = value; }
        }        

        [ConfigurationProperty("color", IsRequired = true)]
        public string ForeColor
        {
             get { return (string)this["color"]; }
            set {  this["color"] = value; }  
        }

        [ConfigurationProperty("background-color", IsRequired = true)]
        public string BackColor
        {
            get { return (string)this["background-color"]; }
            set { this["background-color"] = value; }
        }
    }
}
