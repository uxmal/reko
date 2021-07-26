#region License
/* 
 * Copyright (C) 1999-2020 Pavel Tomin.
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

using Reko.Core.CLanguage;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.Core
{
    public class CHeaderLoader : MetadataLoader
    {
        private readonly Stream stream;

        public CHeaderLoader(IServiceProvider services, string filename, byte[] bytes)
            : base(services, filename, bytes)
        {
            this.stream = new MemoryStream(bytes);
        }

        public override TypeLibrary Load(IPlatform platform, TypeLibrary dstLib)
        {
            var rdr = new StreamReader(stream);
            var lexer = new CLexer(rdr, CLexer.StdKeywords);
            var state = new ParserState();
            var parser = new CParser(state, lexer);
            var symbolTable = CreateSymbolTable(platform, dstLib);
            var declarations = parser.Parse();
            foreach (var decl in declarations)
            {
                symbolTable.AddDeclaration(decl);
            }
            var slib = new SerializedLibrary
            {
                Types = symbolTable.Types.ToArray(),
                Procedures = symbolTable.Procedures.ToList(),
            };
            var tldser = new TypeLibraryDeserializer(platform, true, dstLib);
            var tlib = tldser.Load(slib);
            return tlib;
        }

        private SymbolTable CreateSymbolTable(
            IPlatform platform, TypeLibrary typeLib)
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
            return new SymbolTable(platform, primitiveTypes, namedTypes, platform.PointerType.Size);
        }
    }
}
