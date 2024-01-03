#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using System.Text;

namespace Reko.Core.Loading
{
    /// <summary>
    /// Describes an embedded resource type.
    /// </summary>
    public struct ResourceType
    {
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

        public static bool operator ==(ResourceType a, ResourceType b) =>
            a.Value == b.Value;
        public static bool operator !=(ResourceType a, ResourceType b) =>
            a.Value != b.Value;

        public override bool Equals(object? obj)
        {
            return obj is ResourceType that &&
                this.Value == that.Value;
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

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
