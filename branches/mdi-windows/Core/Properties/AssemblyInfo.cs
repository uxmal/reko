/* 
 * Copyright (C) 1999-2010 John Källén.
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

using System.Reflection;
using System.Runtime.CompilerServices;

//
// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
//
[assembly: AssemblyTitle("Decompiler core")]
[assembly: AssemblyDescription("Core functionality for decompiler.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct(Decompiler.AssemblyMetadata.Product)]
[assembly: AssemblyCopyright(Decompiler.AssemblyMetadata.Copyright)]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]		

//
// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Revision and Build Numbers 
// by using the '*' as shown below:

[assembly: AssemblyVersion(Decompiler.AssemblyMetadata.AssemblyVersion)]
[assembly: AssemblyFileVersion(Decompiler.AssemblyMetadata.AssemblyFileVersion)]

namespace Decompiler
{
    public class AssemblyMetadata
    {
        public const string AssemblyVersion = "0.1.1.0";
        public const string AssemblyFileVersion = "0.1.1.0";
        public const string Product = "Decompiler";
        public const string Copyright = "Copyright © 1999-2010 John Källén";
    }
}