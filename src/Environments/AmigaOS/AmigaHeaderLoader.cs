#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.Environments.AmigaOS
{
    /// <summary>
    /// Used to load Reko-specific Amiga header files (*.rh)
    /// </summary>
    public class AmigaHeaderLoader : MetadataLoader
    {
        private byte[] bytes;

        public AmigaHeaderLoader(IServiceProvider services, string filename, byte[] bytes)
            : base(services, filename, bytes)
        {
            this.bytes = bytes;
            this.SystemServices = new Dictionary<int, SystemService>();
        }

        public Dictionary<int, SystemService> SystemServices { get; private set; }

        public override TypeLibrary Load(IPlatform platform, TypeLibrary dstLib)
        {
            var rdr = new StreamReader(new MemoryStream(bytes));
            var lexer = new CLexer(rdr, CLexer.StdKeywords);
            var state = new ParserState();
            var parser = new CParser(state, lexer);
            var symbolTable = new SymbolTable(platform, 4);
            var declarations = parser.Parse();
            var tldser = new TypeLibraryDeserializer(platform, true, dstLib);
            foreach (var decl in declarations)
            {
                ProcessDeclaration(decl, platform, tldser, symbolTable);
            }
            return dstLib;
        }

        public void ProcessDeclaration(Decl declaration, IPlatform platform, TypeLibraryDeserializer tldser, SymbolTable symbolTable)
        {
            var types = symbolTable.AddDeclaration(declaration);
            var type = types[0];
            int? vectorOffset = GetVectorOffset(declaration);
            if (vectorOffset.HasValue)
            {
                var ntde = new NamedDataTypeExtractor(platform, declaration.decl_specs, symbolTable, 4);
                foreach (var declarator in declaration.init_declarator_list)
                {
                    var nt = ntde.GetNameAndType(declarator.Declarator);
                    var ssig = (SerializedSignature?)nt.DataType;
                    if (ssig != null && ssig.ReturnValue != null)
                    {
                        ssig.ReturnValue.Kind = ntde.GetArgumentKindFromAttributes(
                            "returns", declaration.attribute_list);
                    }
                    var sser = new ProcedureSerializer(platform, tldser, platform.DefaultCallingConvention);
                    var sig = sser.Deserialize(ssig, platform.Architecture.CreateFrame());
                    SystemServices.Add(
                        vectorOffset.Value,
                        new SystemService
                        {
                            Name = nt.Name,
                            SyscallInfo = new SyscallInfo
                            {
                                Vector = vectorOffset.Value,
                            },
                            Signature = sig,
                        });
                }
            }
        }

        private int? GetVectorOffset(Decl declaration)
        {
            if (declaration.attribute_list == null)
                return null;
            return declaration.attribute_list
                .Where(a =>
                    a.Name.Components[0] == "reko" &&
                    a.Name.Components[1] == "amiga_function_vector")
                .Select(a => (int?)(int)a.Tokens[2].Value!)
                .FirstOrDefault();
        }
    }
}
