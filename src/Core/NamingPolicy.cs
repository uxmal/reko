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

using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core
{
    /// <summary>
    /// This class provides methods that generate automatic names for Procedures, 
    /// variables, and types.
    /// </summary>
    /// <remarks>
    /// The intent is that this class can be subclassed and  modified to suit
    /// the user's preferences.
    /// </remarks>
    public class NamingPolicy
    {
        public NamingPolicy()
        {
            this.Types = new TypeNamingPolicy();
        }

        public TypeNamingPolicy Types { get; }

        public virtual string ProcedureName(Address addr)
        {
            return addr.GenerateName("fn", "");
        }

        /// <summary>
        /// Generates the name for a block stating at address <paramref name="addr"/>.
        /// </summary>
        /// <returns>The name as a string.</returns>
        public virtual string BlockName(Address addr)
        {
            if (addr == null) throw new ArgumentNullException(nameof(addr));
            return addr.GenerateName("l", "");
        }

        public virtual string StackArgumentName(DataType type, int cbOffset, string nameOverride)
        {
            return GenerateStackAccessName(type, "Arg", cbOffset, nameOverride);
        }

        public virtual string StackLocalName(DataType type, int cbOffset, string nameOverride)
        {
            return GenerateStackAccessName(type, "Loc", cbOffset, nameOverride);
        }

        private string GenerateStackAccessName(DataType type, string prefix, int cbOffset, string nameOverride)
        {
            if (nameOverride != null)
                return nameOverride;
            else
                return FormatStackAccessName(type, prefix, cbOffset);
        }

        private string FormatStackAccessName(DataType type, string prefix, int cbOffset)
        {
            cbOffset = Math.Abs(cbOffset);
            string fmt = (cbOffset > 0xFF) ? "{0}{1}{2:X4}" : "{0}{1}{2:X2}";
            return string.Format(fmt, this.Types.ShortPrefix(type), prefix, cbOffset);
        }


        public static readonly NamingPolicy Instance = new NamingPolicy();
    }
}
