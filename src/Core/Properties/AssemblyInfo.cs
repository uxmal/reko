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

using System.Reflection;
using System.Runtime.CompilerServices;

//
// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
//
[assembly: AssemblyTitle("Reko core")]
[assembly: AssemblyDescription("Core functionality for Reko decompiler.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(Reko.AssemblyMetadata.Company)]
[assembly: AssemblyProduct(Reko.AssemblyMetadata.Product)]
[assembly: AssemblyCopyright(Reko.AssemblyMetadata.Copyright)]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: AssemblyVersion(Reko.AssemblyMetadata.AssemblyVersion)]
[assembly: AssemblyFileVersion(Reko.AssemblyMetadata.AssemblyFileVersion)]

namespace Reko
{
    public static class AssemblyMetadata
    {
        public const string AssemblyVersion = "0.9.0.0";
        public const string AssemblyFileVersion = "0.9.0.0";
        public const string Product = "Reko decompiler";
        public const string Copyright = "Copyright © 1999-2020 John Källén";
        public const string Company = "John Källén Konsult AB";
    }
}