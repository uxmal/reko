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
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace Reko.Core.Services
{
    /// <summary>
    /// Loads fully-qualified types of plugins into the current process.
    /// </summary>
    /// <remarks>
    /// Microsoft in their infinite wisdom completely broke the the way types 
    /// are loaded dynamically with Type.GetType(). Cursing them for a million years
    /// is not enough. But we choose the high path and write this workaround class that
    /// does what Type.GetType used to do.
    /// </remarks>
    public interface IPluginLoaderService
    {
        /// <summary>
        /// Load the .NET type using its fully qualified instance. This method assumes
        /// that the assembly is physically located in the same directory as the
        /// calling assembly.
        /// </summary>
        /// <param name="fullyQualifiedName"></param>
        Type GetType(string fullyQualifiedName);
    }

    /// <summary>
    /// Implementation of <see cref="IPluginLoaderService"/>.
    /// </summary>
    public class PluginLoaderService : IPluginLoaderService
    {
        /// <inheritdoc/>
        public Type GetType(string fullyQualifiedTypeName)
        {
            var components = fullyQualifiedTypeName.Split(',');
            if (components.Length < 2)
                throw new ApplicationException($"reko.config contains malformed type name {fullyQualifiedTypeName}.");
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
            var asmName = Path.Combine(dir, (components[1].Trim() + ".dll"));
            var typeName = components[0].Trim();
            var asm = AssemblyLoadContext.Default.LoadFromAssemblyPath(asmName);
            Type t = asm.GetType(typeName, true)!;
            return t;
        }
    }
}
