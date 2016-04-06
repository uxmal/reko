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
        string PaddingTop { get; }
        string PaddingLeft { get; }
        string PaddingBottom { get; }
        string PaddingRight { get; }
    }

    public class UiStyleElement : UiStyle
    {
        public string Name { get; set; }

        public string FontName { get; set; }

        public string ForeColor { get; set; }

        public string BackColor { get; set; }

        public string Cursor { get; set; }

        public string Width { get; set; }

        [ConfigurationProperty("TextAlign", IsRequired = false)]
        public string TextAlign
        {
            get { return (string)this["TextAlign"]; }
            set { this["TextAlign"] = value; }
        }

        [ConfigurationProperty("PaddingTop", IsRequired = false)]
        public string PaddingTop
        {
            get { return (string) this["PaddingTop"]; }
            set { this["PaddingTop"] = value; }
        }

        [ConfigurationProperty("PaddingLeft", IsRequired = false)]
        public string PaddingLeft
        {
            get { return (string)this["PaddingLeft"]; }
            set { this["PaddingLeft"] = value; }
        }

        [ConfigurationProperty("PaddingBottom", IsRequired = false)]
        public string PaddingBottom
        {
            get { return (string)this["PaddingBottom"]; }
            set { this["PaddingBottom"] = value; }
        }

        [ConfigurationProperty("PaddingRight", IsRequired = false)]
        public string PaddingRight
        {
            get { return (string)this["PaddingRight"]; }
            set { this["PaddingRight"] = value; }
        }
    }
}
