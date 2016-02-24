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
        string Cursor { get; }
        string Width { get; }
        string TextAlign { get; }
    }

    public class UiStyleElement : ConfigurationElement, UiStyle
    {
        [ConfigurationProperty("Name", IsRequired = true)]
        public string Name
        {
            get { return (string)this["Name"]; }
            set { this["Name"] = value; }
        }

        [ConfigurationProperty("Font", IsRequired = false)]
        public string FontName
        {
            get { return (string)this["Font"]; }
            set { this["Font"] = value; }
        }

        [ConfigurationProperty("ForeColor", IsRequired = false)]
        public string ForeColor
        {
            get { return (string)this["ForeColor"]; }
            set { this["ForeColor"] = value; }
        }

        [ConfigurationProperty("BackColor", IsRequired = false)]
        public string BackColor
        {
            get { return (string)this["BackColor"]; }
            set { this["BackColor"] = value; }
        }

        [ConfigurationProperty("Cursor", IsRequired = false)]
        public string Cursor
        {
            get { return (string)this["Cursor"]; }
            set { this["Cursor"] = value; }
        }


        [ConfigurationProperty("Width", IsRequired = false)]
        public string Width
        {
            get { return (string)this["Width"]; }
            set { this["Width"] = value; }
        }

        [ConfigurationProperty("TextAlign", IsRequired = false)]
        public string TextAlign
        {
            get { return (string)this["TextAlign"]; }
            set { this["TextAlign"] = value; }
        }
    }
}
