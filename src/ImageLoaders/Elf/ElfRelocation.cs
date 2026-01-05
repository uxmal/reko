#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

using Reko.Core.Loading;
using System.Text;

namespace Reko.ImageLoaders.Elf
{
    public class ElfRelocation : IRelocation
    {
        public int SymbolIndex;


        public ulong Info { get; set; }
        public ulong Offset { get; set; }
        public long? Addend { get; set; }    // Will have value for a SHT_RELA relocation, and not for SHT_REL

        public ElfSymbol? Symbol { get; set; }
        public uint Type { get; set; }

        IBinarySymbol? IRelocation.Symbol => Symbol;

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("offs: {0:X}", Offset);
            sb.AppendFormat(" info: {0:X}", Info);
            if (Addend.HasValue)
            {
                sb.AppendFormat(" addend: {0:X}", Addend);
            };
            sb.AppendFormat(" symbol index: {0}", SymbolIndex);
            return sb.ToString();
        }
    }
}
