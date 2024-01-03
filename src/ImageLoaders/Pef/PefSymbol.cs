#region License
/* 
 * Copyright (C) 2018-2024 Stefano Moioli <smxdev4@gmail.com>.
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

namespace Reko.ImageLoaders.Pef
{
    public class PefSymbolClass
    {
        public readonly byte Flags;
        public readonly PEFSymbolClassType Class;

        public PefSymbolClass(byte val)
        {
            Class = (PEFSymbolClassType) (val & 0x0F);
            Flags = (byte) ((val >> 4) & 0x0F);

            if(!Enum.IsDefined(typeof(PEFSymbolClassType), Class))
            {
                throw new BadImageFormatException($"Invalid PEF symbol class {Class:X}");
            }
        }
    }

    public class PefSymbol
    {
        public readonly PefSymbolClass SymbolClass;
        public readonly uint NameOffset;

        public PefSymbol(uint sym)
        {
            NameOffset = sym & 0xFFF;
            SymbolClass = new PefSymbolClass((byte) ((sym >> 24) & 0xFF));
        }
    }
}