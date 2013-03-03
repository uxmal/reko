#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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

using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Decompiler.Gui
{
    /// <summary>
    /// Parses "hungarian strings" into DataTypes.
    /// </summary>
    public class HungarianParser
    {
        public DataType Parse(string hungarianString)
        {
            if (hungarianString == null)
                return null;
            Debug.Print("Parsing " + hungarianString);
            var s = hungarianString;
            if (s.StartsWith("a"))
            {
                if (s.Length == 1)
                    return null;
                var elemType = Parse(s.Substring(1));
                if (elemType == null)
                    return null;
                return new ArrayType(elemType, 0);
            }
            if (s == "pfn")
            {
                return PrimitiveType.PtrCode32;
            }
            if (s.StartsWith("p"))
            {
                if (s.Length == 1)
                    return PrimitiveType.Pointer32;     //$ARch-dependent?
                var pointee = Parse(s.Substring(1));
                if (pointee == null)
                    return null;
                return new Pointer(pointee, 4);
            }
            if (s == "b")
                return PrimitiveType.Byte;
            if (s == "i32")
                return PrimitiveType.Int32;
            if (s == "i16")
                return PrimitiveType.Int16;
            if (s == "f")
                return PrimitiveType.Bool;
            if (s == "ch")
                return PrimitiveType.Char;
            if (s == "wch")
                return PrimitiveType.WChar;
            if (s == "sz")
                return new ArrayType(PrimitiveType.Char, 0);
            return null;
        }
    }
}
