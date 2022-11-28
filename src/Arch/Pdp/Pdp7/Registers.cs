#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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

using Reko.Arch.Pdp.Memory;
using Reko.Core;
using System.Collections.Generic;

namespace Reko.Arch.Pdp.Pdp7
{
    public static class Registers
    {
        public static RegisterStorage ac;

        public static Dictionary<string, RegisterStorage> RegistersByName { get; }
        public static Dictionary<StorageDomain, RegisterStorage> RegistersByDomain { get; }

        static Registers()
        {
            ac = new RegisterStorage("ac", 0, 0, PdpTypes.Word18);

            RegistersByName = new Dictionary<string, RegisterStorage>()
            {
                { "ac", ac }
            };
            RegistersByDomain = new Dictionary<StorageDomain, RegisterStorage>
            {
                { ac.Domain, ac },
            };
        }
    }
}
