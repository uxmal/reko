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
 
namespace Reko.Core.Lib
{
    public enum UbjsonMarker
    {
        None = 0,

        Null = 'Z',
        Noop = 'N',
        False = 'F',
        True = 'T',
        Int8 = 'i',
        UInt8 = 'U',
        Int16 = 'I',
        Int32 = 'l',
        Int64 = 'L',
        Float32 = 'd',
        Float64 = 'D',
        Char = 'C',
        String = 'S',

        Array = '[',
        ArrayEnd = ']',
        Object = '{',
        ObjectEnd = '}',

        ElementType = '$',
        ElementCount = '#',
    }
}