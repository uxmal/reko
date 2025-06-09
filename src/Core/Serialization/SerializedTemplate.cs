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

using System.Text;

namespace Reko.Core.Serialization
{
    /// <summary>
    /// Serialization format for a templated type.
    /// </summary>
    public class SerializedTemplate : SerializedType
    {
        /// <summary>
        /// Namespace scope of the type.
        /// </summary>
        public string[] Scope;

        /// <summary>
        /// Name of the type.
        /// </summary>
        public string Name;

        /// <summary>
        /// Generic type arguments.
        /// </summary>
        public SerializedType[] TypeArguments;

        /// <summary>
        /// Constructs a serialized template type.
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="name"></param>
        /// <param name="typeArguments"></param>
        public SerializedTemplate(string[] scope, string name, SerializedType[] typeArguments)
        {
            this.Scope = scope;
            this.Name = name;
            this.TypeArguments = typeArguments;
        }

        /// <inheritdoc/>
        public override T Accept<T>(ISerializedTypeVisitor<T> visitor)
        {
            return visitor.VisitTemplate(this);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            var sb = new StringBuilder();
            if (Scope is not null)
            {
                foreach (var seg in Scope)
                {
                    sb.Append(seg);
                    sb.Append("::");
                }
            }
            sb.Append(Name);
            sb.Append("<");
            var sep = "";
            foreach (var tyArg in TypeArguments)
            {
                sb.Append(sep);
                sb.Append(tyArg);
                sep = ",";
            }
            sb.Append(">");
            return sb.ToString();
        }
    }
}
