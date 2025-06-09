#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Text;

namespace Reko.Core
{
    /// <summary>
    /// This class provides methods that generate automatic names for Procedures, 
    /// variables, and types.
    /// </summary>
    /// <remarks>
    /// The intent is that this class can be subclassed and modified to suit
    /// the user's preferences.
    /// </remarks>
    public class NamingPolicy
    {
        /// <summary>
        /// Creates an instance of the <see cref="NamingPolicy"/> class.
        /// </summary>
        public NamingPolicy()
        {
            this.Types = new TypeNamingPolicy();
        }

        /// <summary>
        /// The naming policy used for automatically naming types.
        /// </summary>
        public TypeNamingPolicy Types { get; }

        /// <summary>
        /// Generates the name for a <see cref="Procedure"/> starting at address <paramref name="addr"/>.
        /// </summary>
        /// <param name="addr">Address of the procedure.</param>
        /// <returns>Name for the procedure.</returns>
        public virtual string ProcedureName(Address addr)
        {
            return addr.GenerateName("fn", "");
        }

        /// <summary>
        /// Generates the name for a basic block starting at address <paramref name="addr"/>.
        /// </summary>
        /// <returns>The name as a string.</returns>
        public virtual string BlockName(Address addr)
        {
            return addr.GenerateName("l", "");
        }

        /// <summary>
        /// Generates the name for a basic block starting at address <paramref name="addr"/>
        /// with a numeric suffix <paramref name="suffix"/>.
        /// </summary>
        /// <returns>The name as a string.</returns>
        /// 
        public virtual string BlockName(Address addr, int suffix)
        {
            return addr.GenerateName("l", suffix.ToString());
        }

        /// <summary>
        /// Generates the name for a basic block based on its <see cref="RtlLocation"/>.
        /// </summary>
        /// <param name="loc">Location of the basic block.</param>
        /// <returns>The name of the basic block as a string.</returns>
        public virtual string BlockName(RtlLocation loc)
        {
            if (loc.Index == 0)
                return BlockName(loc.Address);
            return loc.Address.GenerateName("l", $"_{loc.Index}");
        }

        /// <summary>
        /// Generates the name of a global variable.
        /// </summary>
        /// <param name="field">Global variable field.</param>
        /// <returns></returns>
        public virtual string GlobalName(StructureField field)
        {
            if (field.IsNameSet)
                return field.Name;
            var fieldName = Types.StructureFieldName(field, null);
            return string.Format("g_{0}", fieldName);
        }

        /// <summary>
        /// Generates the name of an argument to a procedure that is passed on the stack.
        /// </summary>
        /// <param name="type">Type of the argument.</param>
        /// <param name="cbOffset">Offset from the top of the frame of the called procedure.</param>
        /// <param name="nameOverride">If not null, use this string instead of synthesizing a name.</param>
        /// <returns></returns>
        public virtual string StackArgumentName(DataType type, int cbOffset, string? nameOverride)
        {
            return GenerateStackAccessName(type, "Arg", cbOffset, nameOverride);
        }

        /// <summary>
        /// Generates the name of a local stack-based variable in a procedure.
        /// </summary>
        /// <param name="type">Type of the argument.</param>
        /// <param name="cbOffset">Offset from the top of the frame of the called procedure.</param>
        /// <param name="nameOverride">If not null, use this string instead of synthesizing a name.</param>
        /// <returns></returns>
        public virtual string StackLocalName(DataType type, int cbOffset, string? nameOverride)
        {
            return GenerateStackAccessName(type, "Loc", cbOffset, nameOverride);
        }

        /// <summary>
        /// Makes a string that can serve as a valid identifer from a string that might
        /// contain non-identifier characters.
        /// </summary>
        /// <param name="name">String to sanitize.</param>
        /// <returns>Sanitized identifier name.</returns>
        public static string SanitizeIdentifierName(string name)
        {
            var sb = new StringBuilder();
            int n = name.Length;
            int i;

            // Skip leading unprintables.
            for (i = 0; i < n; ++i)
            {
                if (char.IsLetterOrDigit(name[i]))
                    break;
            }

            bool emitUnderscore = false;
            for (; i < n; ++i)
            {
                char ch = name[i];
                if (char.IsLetterOrDigit(ch))
                {
                    if (emitUnderscore)
                    {
                        sb.Append('_');
                        emitUnderscore = false;
                    }
                    sb.Append(ch);
                }
                else
                {
                    emitUnderscore = true;
                }
            }
            return sb.ToString();
        }

        private string GenerateStackAccessName(DataType type, string prefix, int cbOffset, string? nameOverride)
        {
            if (nameOverride is not null)
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

        /// <summary>
        /// Default instance of the <see cref="NamingPolicy"/> class.
        /// </summary>
        public static readonly NamingPolicy Instance = new();
    }
}
