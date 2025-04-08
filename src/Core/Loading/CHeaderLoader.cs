#region License
/* 
 * Copyright (C) 1999-2025 Pavel Tomin.
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

using Reko.Core.Hll.C;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Reko.Core.Loading
{
    /// <summary>
    /// This class loads C header files -- with optional Reko-specific
    /// attribute annotations -- into a <see cref="TypeLibrary"/>.
    /// </summary>
    public class CHeaderLoader : MetadataLoader
    {
        private readonly Stream stream;

        public CHeaderLoader(IServiceProvider services, ImageLocation imagelocation, byte[] bytes)
            : base(services, imagelocation, bytes)
        {
            this.stream = new MemoryStream(bytes);
        }

        /// <summary>
        /// Loads the contents of the header file into the given <see cref="TypeLibrary"/>.
        /// </summary>
        /// <param name="platform"><see cref="IPlatform"/> instance needed to resolve
        /// C primitive types etc.</param>
        /// <param name="dstLib">An existing <see cref="TypeLibrary"/> that will be 
        /// augmented with metadata extracted from the C file.
        /// </param>
        /// <returns>The given typelibrary, mutated with the contents of the C file.</returns>
        public override TypeLibrary Load(IPlatform platform, TypeLibrary dstLib)
        {
            var rdr = new StreamReader(stream);
            var parser = platform.CreateCParser(rdr);
            var symbolTable = CreateSymbolTable(platform, dstLib);
            var declarations = parser.Parse();
            foreach (var decl in declarations)
            {
                if (IsAttributeOnlyDecl(decl))
                    symbolTable.ProcessDeclarationAttributes(decl.attribute_list);
                else 
                    symbolTable.AddDeclaration(decl);
            }
            var slib = new SerializedLibrary
            {
                Types = symbolTable.Types.ToArray(),
                Procedures = symbolTable.Procedures.ToList(),
                Globals = symbolTable.Variables.Select(ConvertToGlobalVariable).ToList(),
                Annotations = symbolTable.Annotations,
                Segments = symbolTable.Segments,
            };
            var tldser = new TypeLibraryDeserializer(platform, true, dstLib);
            var tlib = tldser.Load(slib);
            return tlib;
        }

        private static bool IsAttributeOnlyDecl(Decl decl)
        {
            return 
                (decl.decl_specs is null ||
                 decl.decl_specs.Count == 0) &&
                (decl.init_declarator_list is null ||
                 decl.init_declarator_list.Count == 0);
        }

        private static GlobalVariable_v1 ConvertToGlobalVariable(GlobalDataItem_v2 item)
        {
            return new GlobalVariable_v1
            {
                Name = item.Name,
                DataType = item.DataType,
                Ordinal = GlobalVariable_v1.NoOrdinal,
                Address = item.Address,
            };
        }

        private static SymbolTable CreateSymbolTable(
            IPlatform platform,
            TypeLibrary typeLib)
        {
            var dtSer = new DataTypeSerializer();
            var primitiveTypes = PrimitiveType.AllTypes
                .ToDictionary(
                    d => d.Key,
                    d => (PrimitiveType_v1) d.Value.Accept(dtSer));
            var namedTypes = new Dictionary<string, SerializedType>();
            var typedefs = typeLib.Types;
            foreach (var typedef in typedefs)
            {
                namedTypes[typedef.Key] = typedef.Value.Accept(dtSer);
            }
            return new SymbolTable(platform, primitiveTypes, namedTypes);
        }
    }
}
