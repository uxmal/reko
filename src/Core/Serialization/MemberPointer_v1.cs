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

namespace Reko.Core.Serialization
{
    /// <summary>
    /// Serialization of a member pointer type.
    /// </summary>
    public class MemberPointer_v1 : SerializedType
    {
        /// <summary>
        /// The class that contains the member pointer.
        /// </summary>
        public TypeReference_v1? DeclaringClass;

        /// <summary>
        /// The member that is pointed to.
        /// </summary>
        public SerializedType? MemberType;

        /// <summary>
        /// Size of the pointer in storage units.
        /// </summary>
        public int Size;

        /// <inheritdoc/>
        public override T Accept<T>(ISerializedTypeVisitor<T> visitor)
        {
            return visitor.VisitMemberPointer(this);
        }
    }
}
