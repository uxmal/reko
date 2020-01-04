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

using Reko.Core;
using Reko.Core.CLanguage;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private readonly ParserState parserState;
        private readonly IPlatform platform;

        public XmlConverter(TextReader rdr, XmlWriter writer, IPlatform platform)
        {
            this.rdr = rdr;
            this.writer = writer;
            this.platform = platform ?? throw new ArgumentNullException(nameof(platform));
            this.parserState = new ParserState();
       }

        public void Convert()
        {
            var lexer = new CLexer(rdr);
            var parser = new CParser(parserState, lexer);
            var declarations = parser.Parse();
            var symbolTable = new SymbolTable(platform)
            {
                NamedTypes = {
                    { "off_t", new PrimitiveType_v1 { Domain = Domain.SignedInt, ByteSize = 4 } },          //$BUGBUG: arch-dependent!
                    { "ssize_t", new PrimitiveType_v1 { Domain = Domain.SignedInt, ByteSize = 4 } },        //$BUGBUG: arch-dependent!
                    { "size_t", new PrimitiveType_v1 { Domain = Domain.UnsignedInt, ByteSize = 4 } },       //$BUGBUG: arch-dependent!
                    { "va_list", new PrimitiveType_v1 { Domain = Domain.Pointer, ByteSize = platform.PointerType.Size } }
                }
            };

            foreach (var decl in declarations)
            {
                symbolTable.AddDeclaration(decl);
            }

            var lib = new SerializedLibrary
            {
                Types = symbolTable.Types.ToArray(),
                Procedures = symbolTable.Procedures.ToList(),
            };
            var ser = SerializedLibrary.CreateSerializer();
            ser.Serialize(writer, lib);
        }
    }
}
