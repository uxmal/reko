#region License
/* 
 * Copyright (C) 2018-2021 Stefano Moioli <smxdev4@gmail.com>.
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

    public class PefClassAndName
    {
        public readonly PEFSymbolClassType SymbolClass;
        public readonly uint NameOffset;

        public PefClassAndName(uint val)
        {
            NameOffset = val & 0xFFFFFF;
            SymbolClass = (PEFSymbolClassType) ((val >> 24) & 0xFF);

            if (!Enum.IsDefined(typeof(PEFSymbolClassType), SymbolClass))
            {
                throw new BadImageFormatException($"Invalid PEF symbol class {SymbolClass}");
            }
        }
    }

    public class PefExportedSymbol
    {
        public readonly PefClassAndName classAndName;
        public readonly PEFExportedSymbol sym;

        public readonly string Name;

        public PefExportedSymbol(PEFExportedSymbol sym, PefHashWord exportHash, PefLoaderStringTable stringTable)
        {
            this.classAndName = new PefClassAndName(sym.classAndName);
            this.sym = sym;
            this.Name = stringTable.ReadString(classAndName.NameOffset, exportHash.NameLength);
        }

        public override string ToString()
        {
            var klass = Enum.GetName(typeof(PEFSymbolClassType), classAndName.SymbolClass);
            return $"(class={klass}, name={Name})";
        }
    }

    public class PefExportHash
    {
        public readonly ushort ChainCount;
        public readonly ushort FirstExportIndex;

        public PefExportHash(uint val)
        {
            FirstExportIndex = (ushort) (val & 0x3FFFF);
            ChainCount = (ushort) ((val >> 18) & 0x3FFF);
        }
    }

    public class PefHashWord
    {
        public readonly ushort EncodedName;
        public readonly ushort NameLength;

        public readonly uint HashWord;


        public PefHashWord(uint val)
        {
            HashWord = val;
            EncodedName = (ushort) (val & 0xFFFF);
            NameLength = (ushort) ((val >> 16) & 0xFFFF);
        }
    }
}
