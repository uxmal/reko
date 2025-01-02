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

using Reko.Core;
using Reko.Core.Hll.C;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Reko.Tools.C2Xml
{
    /// <summary>
    /// Parses a C compilation unit and transforms all declarations to the 
    /// XML syntax used by Reko.
    /// </summary>
    public class XmlConverter
    {
        private readonly TextReader rdr;
        private readonly XmlWriter writer;
        private readonly bool explicitPointerSizes;
        private readonly string dialect;
        private readonly ParserState parserState;
        private readonly IPlatform platform;

        public XmlConverter(TextReader rdr, XmlWriter writer, IPlatform platform, bool explicitPointerSizes, string dialect)
        {
            this.rdr = rdr;
            this.writer = writer;
            this.platform = platform ?? throw new ArgumentNullException(nameof(platform));
            this.explicitPointerSizes = explicitPointerSizes;
            this.dialect = dialect;
            this.parserState = CreateParserState(dialect);
        }

        private ParserState CreateParserState(string dialect)
        {
            var state = new ParserState();
            switch (dialect)
            {
            case "gcc":
                state.Typedefs.UnionWith(
                    new[] {
                        "_Float32",
                        "_Float64",
                        "_Float128",
                        "_Float32x",
                        "_Float64x",
                        "__int128_t"
                    });
                break;
            }
            return state;
        }

        private CLexer CreateLexer()
        {
            switch (dialect)
            {
            case "gcc": return new CLexer(rdr, CLexer.GccKeywords);
            case "msvc": return new CLexer(rdr, CLexer.MsvcKeywords);
            case "msvcce": return new CLexer(rdr, CLexer.MsvcCeKeywords);
            default: return new CLexer(rdr, CLexer.StdKeywords);
            }
        }

        public void Convert()
        {
            var lexer = CreateLexer();
            var parser = new CParser(parserState, lexer);
            var declarations = parser.Parse();
            var symbolTable = CreateSymbolTable();

            foreach (var decl in declarations)
            {
                try
                {
                    symbolTable.AddDeclaration(decl);
                } catch (Exception ex)
                {
                    Console.WriteLine("Error when handling declaration {0}: {1}", decl, ex.Message);
                    throw;
                }
            }

            var lib = new SerializedLibrary
            {
                Types = symbolTable.Types.ToArray(),
                Procedures = symbolTable.Procedures.ToList(),
            };
            var ser = SerializedLibrary.CreateSerializer();
            ser.Serialize(writer, lib);
        }

        private SymbolTable CreateSymbolTable()
        {
            int pointerSize = this.explicitPointerSizes
                ? platform.PointerType.Size
                : 0;
            var symtab = new SymbolTable(platform, pointerSize)
            {
                NamedTypes = {
                    { "off_t", new PrimitiveType_v1 { Domain = Domain.SignedInt, ByteSize = platform.PointerType.Size } },
                    { "ssize_t", new PrimitiveType_v1 { Domain = Domain.SignedInt, ByteSize = platform.PointerType.Size } },
                    { "size_t", new PrimitiveType_v1 { Domain = Domain.UnsignedInt, ByteSize = platform.PointerType.Size } },
                    { "va_list", new PrimitiveType_v1 { Domain = Domain.Pointer, ByteSize = platform.PointerType.Size } }
                }
            };
            switch (dialect)
            {
            case "gcc":
                symtab.NamedTypes.Add("__builtin_va_list", new PrimitiveType_v1 { Domain = Domain.Pointer, ByteSize = platform.PointerType.Size });
                symtab.NamedTypes.Add("_Float32", new PrimitiveType_v1 { Domain = Domain.Real, ByteSize = 4 });
                symtab.NamedTypes.Add("_Float64", new PrimitiveType_v1 { Domain = Domain.Real, ByteSize = 8 });
                symtab.NamedTypes.Add("_Float128", new PrimitiveType_v1 { Domain = Domain.Real, ByteSize = 16 });
                symtab.NamedTypes.Add("_Float32x", new PrimitiveType_v1 { Domain = Domain.Real, ByteSize = 4 });
                symtab.NamedTypes.Add("_Float64x", new PrimitiveType_v1 { Domain = Domain.Real, ByteSize = 8 });
                symtab.NamedTypes.Add("__int128_t", new PrimitiveType_v1 { Domain = Domain.Real, ByteSize = 16 });
                break;
            }
            return symtab;
        }
    }
}
