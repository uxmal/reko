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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core.Configuration
{
    /// <summary>
    /// A property option describes a configurable setting that the user
    /// can manipulate.
    /// </summary>
    public class PropertyOption
    {
        /// <summary>
        /// Technical name for the option.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Display name of the option.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Help text for the option.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Set to true if a non-blank value must be provided.
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// If non-blank, use the type name to load a user-interface element for 
        /// this option. This makes it possble to show custom user interfaces
        /// for options.
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// If non-empty, provides a fixed list of available values for the option.
        /// This list is typically displayed in a drop down list.
        /// </summary>
        public ListOption_v1[] Choices { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }
}
