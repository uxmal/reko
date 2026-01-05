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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Reko.Core.Loading
{
    /// <summary>
    /// Describes an embedded resource type.
    /// </summary>
    public struct ResourceType
    {
        /// <summary>
        /// Constructs a new resource type.
        /// </summary>
        /// <param name="id">Numeric identifier of the resource.</param>
        /// <param name="name">Name of the resource.</param>
        /// <param name="fileExtension">File extension of the resource.</param>
        public ResourceType(int id, string name, string fileExtension)
        {
            this.Value = id;
            this.Name = name;
            this.FileExtension = fileExtension;
        }

        /// <summary>
        /// Numeric identifier for the resource type.
        /// </summary>
        public int Value { get; }

        /// <summary>
        /// Resource name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// File extension to use for this resource type.
        /// </summary>
        public string FileExtension { get; }

        /// <summary>
        /// Equality operator for resource types.
        /// </summary>
        /// <param name="a">First resource.</param>
        /// <param name="b">Second resource.</param>
        /// <returns>True if the resources are equal; otherwise false.</returns>
        public static bool operator ==(ResourceType a, ResourceType b) =>
            a.Value == b.Value;

        /// <summary>
        /// Inquality operator for resource types.
        /// </summary>
        /// <param name="a">First resource.</param>
        /// <param name="b">Second resource.</param>
        /// <returns>False if the resources are equal; otherwise true.</returns>
        public static bool operator !=(ResourceType a, ResourceType b) =>
            a.Value != b.Value;

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return obj is ResourceType that &&
                this.Value == that.Value;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        /// <summary>
        /// Constructs a dictionary of resource types from the given type.
        /// </summary>
        /// <param name="type">Type defining various resource types.</param>
        /// <returns>A dictionary of the various resource types.</returns>
        public static Dictionary<int, ResourceType> MakeDictionary(Type type)
        {
            return type
                .GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(p => p.PropertyType == typeof(ResourceType))
                .Select(p => (ResourceType) p.GetValue(null)!)
                .ToDictionary(rt => rt.Value);
        }
    }
}
