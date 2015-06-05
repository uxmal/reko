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
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Reflection;

namespace Decompiler.Core
{
    /// <summary>
    /// Reads in a structure field by field from an image reader.
    /// </summary>
    public class StructureReader
    {
        private object structure;
        private FieldInfo[] fields;

        public StructureReader(object structure)
        {
            this.structure = structure;
            this.fields = structure.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
        }

        public void Read(ImageReader rdr)
        {
            foreach (var f in fields)
            {
                var attr = GetFieldAttribute(f);
                uint alignment = (uint) attr.Align;
                rdr.Offset = (rdr.Offset + alignment - 1u) & ~(alignment - 1u);
                Debug.Print("At offset: {0:X8} reading field '{1}.{2}' after alignment of {3}.", rdr.Offset, f.DeclaringType.Name, f.Name, alignment);

                object value = attr.ReadValue(f, rdr, null);
                f.SetValue(structure, value);
            }
        }

        private FieldAttribute GetFieldAttribute(FieldInfo f)
        {
            var attrs = f.GetCustomAttributes(typeof(FieldAttribute), true);
            if (attrs == null || attrs.Length == 0)
            {
                return new FieldAttribute { Align = 1 };
            }
            return (FieldAttribute) attrs[0]; 
        }
    }
}
